using System.ComponentModel;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Services;
using CreationsForge.Services.Interfaces;

namespace CreationsForge.PresentationTests.Support;

/// <summary>
/// Supplies controllable workspace state without opening records.
/// </summary>
internal sealed class FakeWorkspaceCoordinator : IWorkspaceCoordinator
{
    /// <summary>The property-change subscribers.</summary>
    private PropertyChangedEventHandler? PropertyChangedHandlers;

    /// <summary>The active workspace descriptor.</summary>
    private WorkspaceDescriptor? CurrentWorkspaceValue;

    /// <summary>Tracks whether the one effective coordinator disposal has begun.</summary>
    private int IsDisposedValue;

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged
    {
        add => PropertyChangedHandlers += value;
        remove => PropertyChangedHandlers -= value;
    }

    /// <inheritdoc />
    public WorkspaceDescriptor? CurrentWorkspace => CurrentWorkspaceValue;

    /// <summary>Gets how many times the active workspace was closed.</summary>
    public int CloseCount { get; private set; }

    /// <summary>Gets how many times the coordinator was disposed.</summary>
    public int DisposeCount { get; private set; }

    /// <summary>Gets or sets optional asynchronous behavior for a coordinator disposal attempt.</summary>
    public Func<ValueTask>? DisposeAction { get; set; }

    /// <summary>Gets the number of active property-change subscribers.</summary>
    public int SubscriberCount => PropertyChangedHandlers?.GetInvocationList().Length ?? 0;

    /// <summary>Publishes the supplied descriptor to presentation subscribers.</summary>
    /// <param name="workspace">The descriptor to publish, or <see langword="null"/>.</param>
    public void SetCurrentWorkspace(WorkspaceDescriptor? workspace)
    {
        CurrentWorkspaceValue = workspace;
        PropertyChangedHandlers?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentWorkspace)));
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<WorkspaceDescriptor>> OpenAsync(
        WorkspaceLaunchRequest request,
        CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("This presentation fake does not open records.");
    }

    /// <inheritdoc />
    public ValueTask<EngineResult<T>> ExecuteAsync<T>(
        Func<IPluginWorkspace, CancellationToken, ValueTask<EngineResult<T>>> operation,
        CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("This presentation fake does not borrow records.");
    }

    /// <inheritdoc />
    public ValueTask CloseAsync()
    {
        CloseCount++;
        SetCurrentWorkspace(null);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref IsDisposedValue, 1) != 0)
        {
            return ValueTask.CompletedTask;
        }

        DisposeCount++;
        SetCurrentWorkspace(null);
        return DisposeAction?.Invoke() ?? ValueTask.CompletedTask;
    }
}
