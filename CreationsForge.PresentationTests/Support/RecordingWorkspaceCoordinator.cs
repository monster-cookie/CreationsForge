using System.ComponentModel;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Services;
using CreationsForge.Services.Interfaces;

namespace CreationsForge.PresentationTests.Support;

/// <summary>
/// Publishes deterministic workspace generations and lends a browser test workspace without transferring ownership.
/// </summary>
internal sealed class RecordingWorkspaceCoordinator : IWorkspaceCoordinator
{
    /// <summary>The workspace lent to browser operations while a descriptor is active.</summary>
    private IFormListWorkspace? WorkspaceValue;

    /// <summary>The currently published immutable descriptor.</summary>
    private WorkspaceDescriptor? CurrentWorkspaceValue;

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc />
    public WorkspaceDescriptor? CurrentWorkspace => CurrentWorkspaceValue;

    /// <summary>Gets how many operations borrowed the test workspace.</summary>
    public int ExecuteCount { get; private set; }

    /// <summary>Publishes a new descriptor and associated borrowed test workspace.</summary>
    /// <param name="descriptor">The immutable workspace descriptor.</param>
    /// <param name="workspace">The workspace to lend to browser operations.</param>
    public void Publish(WorkspaceDescriptor descriptor, IFormListWorkspace workspace)
    {
        ArgumentNullException.ThrowIfNull(descriptor);
        ArgumentNullException.ThrowIfNull(workspace);
        WorkspaceValue = workspace;
        CurrentWorkspaceValue = descriptor;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentWorkspace)));
    }

    /// <summary>Publishes closure without disposing the test-owned workspace.</summary>
    public void PublishClosed()
    {
        WorkspaceValue = null;
        CurrentWorkspaceValue = null;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentWorkspace)));
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<WorkspaceDescriptor>> OpenAsync(
        WorkspaceLaunchRequest request,
        CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("This browser coordinator is published explicitly by its tests.");
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<T>> ExecuteAsync<T>(
        Func<IFormListWorkspace, CancellationToken, ValueTask<EngineResult<T>>> operation,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(operation);
        ExecuteCount++;
        if (WorkspaceValue is null || CurrentWorkspaceValue is null)
        {
            return ValueTask.FromResult(EngineResult<T>.Failure(new EngineError(
                EngineErrorCode.InvalidRequest,
                "No workspace is open.")));
        }

        return operation(WorkspaceValue, cancellationToken);
    }

    /// <inheritdoc />
    public ValueTask CloseAsync()
    {
        PublishClosed();
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        PublishClosed();
        return ValueTask.CompletedTask;
    }
}
