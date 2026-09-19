using System.ComponentModel;
using System.Runtime.CompilerServices;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Services.Interfaces;
using Serilog;

namespace CreationsForge.Services;

/// <summary>
/// Owns one root-lifetime workspace and publishes replacements only after source and optional output acquisition succeeds.
/// </summary>
public sealed class WorkspaceCoordinator : IWorkspaceCoordinator
{
    /// <summary>Serializes workspace acquisition, borrowed operations, close, and disposal.</summary>
    private readonly SemaphoreSlim WorkspaceGate = new(1, 1);

    /// <summary>Creates independently owned workspaces.</summary>
    private readonly IPluginWorkspaceFactory WorkspaceFactory;

    /// <summary>Publishes bound state on the Avalonia UI thread.</summary>
    private readonly IUiDispatcher UiDispatcher;

    /// <summary>Records unexpected plugin acquisition, operation, and cleanup failures.</summary>
    private readonly ILogger Logger;

    /// <summary>The workspace owned exclusively by this coordinator.</summary>
    private IPluginWorkspace? OwnedWorkspace;

    /// <summary>Immutable bound identity for <see cref="OwnedWorkspace"/>.</summary>
    private WorkspaceDescriptor? CurrentWorkspaceValue;

    /// <summary>Tracks whether root-lifetime disposal has started.</summary>
    private bool IsDisposed;

    /// <summary>Initializes a root-lifetime workspace coordinator.</summary>
    /// <param name="workspaceFactory">The engine factory used to acquire independently owned workspaces.</param>
    /// <param name="uiDispatcher">The dispatcher used to publish bound state.</param>
    /// <param name="logger">The structured logger for unexpected failures.</param>
    /// <exception cref="ArgumentNullException">Thrown when a required dependency is <see langword="null"/>.</exception>
    public WorkspaceCoordinator(
        IPluginWorkspaceFactory workspaceFactory,
        IUiDispatcher uiDispatcher,
        ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(workspaceFactory);
        ArgumentNullException.ThrowIfNull(uiDispatcher);
        ArgumentNullException.ThrowIfNull(logger);
        WorkspaceFactory = workspaceFactory;
        UiDispatcher = uiDispatcher;
        Logger = logger.ForContext<WorkspaceCoordinator>();
    }

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc />
    public WorkspaceDescriptor? CurrentWorkspace => CurrentWorkspaceValue;

    /// <inheritdoc />
    public async ValueTask<EngineResult<WorkspaceDescriptor>> OpenAsync(
        WorkspaceLaunchRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        await WorkspaceGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        IPluginWorkspace? candidate = null;
        try
        {
            if (IsDisposed)
            {
                return DisposedFailure<WorkspaceDescriptor>();
            }

            var openResult = await WorkspaceFactory.OpenAsync(request.Sources, cancellationToken).ConfigureAwait(false);
            if (!openResult.Succeeded || openResult.Value is null)
            {
                return EngineResult<WorkspaceDescriptor>.Failure(
                    openResult.Error ?? UnexpectedError("The workspace factory returned no workspace or failure reason."),
                    workspaceId: request.Sources.WorkspaceId,
                    warnings: openResult.Warnings);
            }

            candidate = openResult.Value;
            cancellationToken.ThrowIfCancellationRequested();
            OutputSelectionReceipt? outputReceipt = null;
            EngineResult<OutputSelectionReceipt>? outputResult = null;
            SelectOutputRequest? outputRequest = null;
            if (request.Output is not null)
            {
                outputRequest = new SelectOutputRequest(
                    Guid.NewGuid(),
                    candidate.Revision,
                    request.OutputMode,
                    request.Output);
                outputResult = await candidate.SelectOutputAsync(outputRequest, cancellationToken).ConfigureAwait(false);
                if (!outputResult.Succeeded || outputResult.Value is null)
                {
                    return EngineResult<WorkspaceDescriptor>.Failure(
                        outputResult.Error ?? UnexpectedError("The workspace returned no output-selection result or failure reason."),
                        workspaceId: candidate.WorkspaceId,
                        operationId: outputRequest.OperationId,
                        baseRevision: outputRequest.ExpectedRevision,
                        resultRevision: outputResult.ResultRevision,
                        warnings: outputResult.Warnings);
                }

                outputReceipt = outputResult.Value;
            }

            cancellationToken.ThrowIfCancellationRequested();
            var descriptor = new WorkspaceDescriptor(
                candidate.WorkspaceId,
                request.Sources.Game,
                request.Sources.Release,
                request.Sources.SourcePluginPath,
                request.Sources.LoadOrderPluginPaths,
                outputReceipt?.Output,
                outputReceipt?.Revision ?? candidate.Revision);
            var previous = OwnedWorkspace;
            OwnedWorkspace = candidate;
            var publicationCommitted = false;
            try
            {
                await UiDispatcher.InvokeAsync(() =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    CurrentWorkspaceValue = descriptor;
                    publicationCommitted = true;
                    OnPropertyChanged(nameof(CurrentWorkspace));
                }).ConfigureAwait(false);
            }
            catch (Exception exception) when (publicationCommitted)
            {
                Logger.Warning(
                    exception,
                    "Workspace {WorkspaceId} committed, but its dispatcher invocation reported a later failure.",
                    descriptor.WorkspaceId);
            }
            catch
            {
                OwnedWorkspace = previous;
                throw;
            }

            candidate = null;
            await DisposeWorkspaceSafelyAsync(previous, "replaced").ConfigureAwait(false);
            return EngineResult<WorkspaceDescriptor>.Success(
                descriptor,
                workspaceId: descriptor.WorkspaceId,
                operationId: outputRequest?.OperationId,
                baseRevision: outputRequest?.ExpectedRevision,
                resultRevision: descriptor.Revision,
                warnings: openResult.Warnings.Concat(outputResult?.Warnings ?? []).ToArray());
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            Logger.Error(exception, "An unexpected failure prevented workspace activation.");
            return EngineResult<WorkspaceDescriptor>.Failure(
                UnexpectedError("An unexpected failure prevented the workspace from opening."),
                workspaceId: request.Sources.WorkspaceId);
        }
        finally
        {
            await DisposeWorkspaceSafelyAsync(candidate, "failed acquisition").ConfigureAwait(false);
            WorkspaceGate.Release();
        }
    }

    /// <inheritdoc />
    public async ValueTask<EngineResult<T>> ExecuteAsync<T>(
        Func<IPluginWorkspace, CancellationToken, ValueTask<EngineResult<T>>> operation,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(operation);
        await WorkspaceGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (IsDisposed)
            {
                return DisposedFailure<T>();
            }

            if (OwnedWorkspace is null)
            {
                return EngineResult<T>.Failure(new EngineError(
                    EngineErrorCode.InvalidRequest,
                    "No workspace is open."));
            }

            return await operation(OwnedWorkspace, cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            Logger.Error(exception, "An unexpected failure occurred while using workspace {WorkspaceId}.", OwnedWorkspace?.WorkspaceId);
            return EngineResult<T>.Failure(
                UnexpectedError("An unexpected failure occurred while using the workspace."),
                workspaceId: OwnedWorkspace?.WorkspaceId);
        }
        finally
        {
            WorkspaceGate.Release();
        }
    }

    /// <inheritdoc />
    public async ValueTask CloseAsync()
    {
        await WorkspaceGate.WaitAsync().ConfigureAwait(false);
        try
        {
            if (IsDisposed)
            {
                return;
            }

            var previous = OwnedWorkspace;
            OwnedWorkspace = null;
            try
            {
                await PublishCurrentWorkspaceAsync(null).ConfigureAwait(false);
            }
            finally
            {
                await DisposeWorkspaceSafelyAsync(previous, "closed").ConfigureAwait(false);
            }
        }
        finally
        {
            WorkspaceGate.Release();
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await WorkspaceGate.WaitAsync().ConfigureAwait(false);
        try
        {
            if (IsDisposed)
            {
                return;
            }

            IsDisposed = true;
            var previous = OwnedWorkspace;
            OwnedWorkspace = null;
            try
            {
                await PublishCurrentWorkspaceAsync(null).ConfigureAwait(false);
            }
            finally
            {
                await DisposeWorkspaceSafelyAsync(previous, "application shutdown").ConfigureAwait(false);
            }
        }
        finally
        {
            WorkspaceGate.Release();
        }
    }

    /// <summary>Publishes an immutable workspace descriptor on the UI thread.</summary>
    /// <param name="descriptor">The active workspace identity, or <see langword="null"/> after close.</param>
    /// <returns>A task that completes after bound state has been published.</returns>
    private Task PublishCurrentWorkspaceAsync(WorkspaceDescriptor? descriptor)
    {
        return UiDispatcher.InvokeAsync(() =>
        {
            if (ReferenceEquals(CurrentWorkspaceValue, descriptor))
            {
                return;
            }

            CurrentWorkspaceValue = descriptor;
            OnPropertyChanged(nameof(CurrentWorkspace));
        });
    }

    /// <summary>Releases a workspace while keeping cleanup failures from invalidating the published replacement.</summary>
    /// <param name="workspace">The workspace to release, or <see langword="null"/> when no ownership was acquired.</param>
    /// <param name="reason">The lifecycle transition responsible for cleanup.</param>
    /// <returns>A task that completes after cleanup was attempted.</returns>
    private async ValueTask DisposeWorkspaceSafelyAsync(IPluginWorkspace? workspace, string reason)
    {
        if (workspace is null)
        {
            return;
        }

        try
        {
            await workspace.DisposeAsync().ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            Logger.Error(exception, "Unable to dispose workspace {WorkspaceId} after it was {Reason}.", workspace.WorkspaceId, reason);
        }
    }

    /// <summary>Creates the stable result returned when work is requested after shutdown disposal.</summary>
    /// <typeparam name="T">The requested successful result type.</typeparam>
    /// <returns>A typed disposed failure.</returns>
    private static EngineResult<T> DisposedFailure<T>()
    {
        return EngineResult<T>.Failure(new EngineError(
            EngineErrorCode.WorkspaceDisposed,
            "The workspace coordinator has been disposed."));
    }

    /// <summary>Creates a stable unexpected engine error without exposing exception details.</summary>
    /// <param name="message">The presentation-safe failure description.</param>
    /// <returns>The typed unexpected error.</returns>
    private static EngineError UnexpectedError(string message)
    {
        return new EngineError(EngineErrorCode.UnexpectedFailure, message);
    }

    /// <summary>Raises a bound property change notification.</summary>
    /// <param name="propertyName">The changed property name.</param>
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        var handlers = PropertyChanged;
        if (handlers is null)
        {
            return;
        }

        var eventArgs = new PropertyChangedEventArgs(propertyName);
        foreach (PropertyChangedEventHandler handler in handlers.GetInvocationList())
        {
            try
            {
                handler(this, eventArgs);
            }
            catch (Exception exception)
            {
                Logger.Warning(exception, "A workspace state observer failed while handling {PropertyName}.", propertyName);
            }
        }
    }
}
