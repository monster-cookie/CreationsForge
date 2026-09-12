using CreationsForge.Core.Engine.Contracts;

namespace CreationsForge.Mcp;

/// <summary>
/// Guards one registry-owned workspace against concurrent close and operation races.
/// </summary>
internal sealed class McpWorkspaceEntry : IAsyncDisposable
{
    /// <summary>Serializes lifetime transitions around borrowed workspace operations.</summary>
    private readonly SemaphoreSlim OperationGate = new(1, 1);

    /// <summary>The opaque engine workspace owned by this entry.</summary>
    private readonly IFormListWorkspace Workspace;

    /// <summary>Shares the single native disposal outcome with every close and shutdown waiter.</summary>
    private readonly TaskCompletionSource DisposalCompletion = new(
        TaskCreationOptions.RunContinuationsAsynchronously);

    /// <summary>Indicates that no new operation may borrow the workspace.</summary>
    private int Closing;

    /// <summary>Indicates that the single workspace disposal operation has started.</summary>
    private int DisposalStarted;

    /// <summary>Gets a value indicating whether close or host shutdown has reserved this entry for disposal.</summary>
    public bool IsClosing => Volatile.Read(ref Closing) != 0;

    /// <summary>Initializes an entry that takes ownership of one engine workspace.</summary>
    /// <param name="workspace">The independently owned workspace.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="workspace"/> is <see langword="null"/>.</exception>
    public McpWorkspaceEntry(IFormListWorkspace workspace)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        Workspace = workspace;
    }

    /// <summary>Prevents future operations from borrowing the workspace before close waits for active work.</summary>
    /// <returns><see langword="true"/> when this call started close; otherwise <see langword="false"/>.</returns>
    public bool TryMarkClosing()
    {
        return Interlocked.CompareExchange(ref Closing, 1, 0) == 0;
    }

    /// <summary>Runs an engine operation while holding this workspace's lifetime gate.</summary>
    /// <typeparam name="T">The successful engine result value.</typeparam>
    /// <param name="operation">The operation that consumes the borrowed workspace.</param>
    /// <param name="cancellationToken">A token that cancels while waiting for or running the operation.</param>
    /// <returns>The engine operation result, or a typed failure when close has started.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="operation"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is observed.</exception>
    public async ValueTask<EngineResult<T>> ExecuteAsync<T>(
        Func<IFormListWorkspace, CancellationToken, ValueTask<EngineResult<T>>> operation,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(operation);
        cancellationToken.ThrowIfCancellationRequested();
        await OperationGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (Volatile.Read(ref Closing) != 0)
            {
                return EngineResult<T>.Failure(
                    new EngineError(
                        EngineErrorCode.WorkspaceDisposed,
                        $"Workspace '{Workspace.WorkspaceId:D}' is closing."),
                    workspaceId: Workspace.WorkspaceId);
            }

            return await operation(Workspace, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            OperationGate.Release();
        }
    }

    /// <summary>Waits for an active operation and then disposes the workspace exactly once.</summary>
    /// <returns>A task that completes after the owned engine workspace is disposed.</returns>
    public ValueTask DisposeAsync()
    {
        TryMarkClosing();
        if (Interlocked.CompareExchange(ref DisposalStarted, 1, 0) == 0)
        {
            _ = DisposeCoreAsync();
        }

        return new ValueTask(DisposalCompletion.Task);
    }

    /// <summary>Waits for active work, attempts native disposal once, and publishes the exact outcome to every waiter.</summary>
    /// <returns>A task that completes after the native disposal outcome has been recorded.</returns>
    private async Task DisposeCoreAsync()
    {
        await OperationGate.WaitAsync().ConfigureAwait(false);
        try
        {
            await Workspace.DisposeAsync().ConfigureAwait(false);
            DisposalCompletion.TrySetResult();
        }
        catch (Exception exception)
        {
            DisposalCompletion.TrySetException(exception);
        }
        finally
        {
            OperationGate.Release();
        }
    }
}
