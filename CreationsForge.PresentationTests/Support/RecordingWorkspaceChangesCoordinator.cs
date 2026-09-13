using System.ComponentModel;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Services;
using CreationsForge.Services.Interfaces;

namespace CreationsForge.PresentationTests.Support;

/// <summary>Publishes and lends one recording workspace while exposing coordinator call counts.</summary>
internal sealed class RecordingWorkspaceChangesCoordinator : IWorkspaceCoordinator
{
    /// <summary>Initializes an empty recording coordinator.</summary>
    internal RecordingWorkspaceChangesCoordinator()
    {
    }

    /// <summary>The current mutable workspace used only by tests.</summary>
    private RecordingWorkspaceChangesWorkspace? WorkspaceValue;

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc />
    public WorkspaceDescriptor? CurrentWorkspace { get; private set; }

    /// <summary>Gets the current recording workspace, or <see langword="null"/> when none is published.</summary>
    internal RecordingWorkspaceChangesWorkspace? Workspace => WorkspaceValue;

    /// <summary>Gets the number of borrowed operations.</summary>
    internal int ExecuteCount { get; private set; }

    /// <summary>Gets the number of close requests.</summary>
    internal int CloseCount { get; private set; }

    /// <summary>Gets the number of disposal requests.</summary>
    internal int DisposeCount { get; private set; }

    /// <summary>Gets the exact open requests in invocation order.</summary>
    internal List<WorkspaceLaunchRequest> OpenRequests { get; } = [];

    /// <summary>Gets or sets a custom open implementation.</summary>
    internal Func<WorkspaceLaunchRequest, CancellationToken, ValueTask<EngineResult<WorkspaceDescriptor>>>? OnOpenAsync { get; set; }

    /// <summary>Publishes a recording workspace and its presentation descriptor.</summary>
    /// <param name="workspace">The recording workspace, or <see langword="null"/> to publish no workspace.</param>
    /// <param name="descriptor">The matching descriptor, or <see langword="null"/> when no workspace is supplied.</param>
    /// <exception cref="ArgumentException">Thrown when workspace and descriptor presence or identity disagree.</exception>
    internal void SetWorkspace(
        RecordingWorkspaceChangesWorkspace? workspace,
        WorkspaceDescriptor? descriptor)
    {
        if ((workspace is null) != (descriptor is null)
            || workspace is not null && descriptor!.WorkspaceId != workspace.WorkspaceId)
        {
            throw new ArgumentException("A recording workspace and descriptor must be present together with the same identity.", nameof(descriptor));
        }

        WorkspaceValue = workspace;
        CurrentWorkspace = descriptor;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentWorkspace)));
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<WorkspaceDescriptor>> OpenAsync(
        WorkspaceLaunchRequest request,
        CancellationToken cancellationToken = default)
    {
        OpenRequests.Add(request);
        cancellationToken.ThrowIfCancellationRequested();
        return OnOpenAsync?.Invoke(request, cancellationToken)
            ?? ValueTask.FromResult(EngineResult<WorkspaceDescriptor>.Failure(
                new EngineError(EngineErrorCode.InvalidRequest, "The recording coordinator open was not configured.")));
    }

    /// <inheritdoc />
    public async ValueTask<EngineResult<T>> ExecuteAsync<T>(
        Func<IFormListWorkspace, CancellationToken, ValueTask<EngineResult<T>>> operation,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(operation);
        ExecuteCount++;
        cancellationToken.ThrowIfCancellationRequested();
        var workspace = WorkspaceValue;
        if (workspace is null)
        {
            return EngineResult<T>.Failure(new EngineError(EngineErrorCode.InvalidRequest, "No recording workspace is open."));
        }

        return await operation(workspace, cancellationToken);
    }

    /// <inheritdoc />
    public ValueTask CloseAsync()
    {
        CloseCount++;
        SetWorkspace(null, null);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        DisposeCount++;
        SetWorkspace(null, null);
        return ValueTask.CompletedTask;
    }
}
