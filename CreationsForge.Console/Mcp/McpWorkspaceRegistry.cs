using CreationsForge.Core.Engine.Contracts;

namespace CreationsForge.Console.Mcp;

/// <summary>
/// Owns isolated engine workspace lifetimes for one MCP host process.
/// </summary>
public sealed class McpWorkspaceRegistry : IAsyncDisposable
{
    /// <summary>Synchronizes registry membership, open reservations, and disposal state.</summary>
    private readonly object SyncRoot = new();

    /// <summary>Stores every workspace whose lifetime is owned by this registry.</summary>
    private readonly Dictionary<Guid, McpWorkspaceEntry> Workspaces = [];

    /// <summary>Reserves identifiers while their factories are acquiring native state.</summary>
    private readonly HashSet<Guid> OpeningWorkspaceIds = [];

    /// <summary>Completes after the single registry disposal operation finishes.</summary>
    private readonly TaskCompletionSource DisposalCompletion = new(
        TaskCreationOptions.RunContinuationsAsynchronously);

    /// <summary>Completes when all workspace opens active at shutdown have released their reservations.</summary>
    private readonly TaskCompletionSource PendingOpensDrained = new(
        TaskCreationOptions.RunContinuationsAsynchronously);

    /// <summary>Tracks workspace opens that have reserved an identifier and have not completed cleanup.</summary>
    private int PendingOpenCount;

    /// <summary>Indicates that the registry no longer accepts or publishes workspaces.</summary>
    private bool IsDisposing;

    /// <summary>Indicates that the single disposal operation has been started.</summary>
    private bool DisposalStarted;

    /// <summary>Gets the number of fully opened workspaces currently owned by the registry.</summary>
    public int ActiveWorkspaceCount
    {
        get
        {
            lock (SyncRoot)
            {
                return Workspaces.Values.Count(workspace => !workspace.IsClosing);
            }
        }
    }

    /// <summary>Returns the active workspace identifiers in deterministic lexical order.</summary>
    /// <returns>An immutable snapshot that is unaffected by later registry changes.</returns>
    public IReadOnlyList<Guid> GetWorkspaceIds()
    {
        lock (SyncRoot)
        {
            return Array.AsReadOnly(Workspaces
                .Where(pair => !pair.Value.IsClosing)
                .Select(pair => pair.Key)
                .OrderBy(id => id)
                .ToArray());
        }
    }

    /// <summary>Opens and publishes one independently owned engine workspace.</summary>
    /// <param name="factory">The native workspace factory supplied by host composition.</param>
    /// <param name="request">The explicit native workspace request.</param>
    /// <param name="cancellationToken">A token that cancels acquisition before registry publication.</param>
    /// <returns>The opened workspace revision or a typed failure without publishing native state.</returns>
    /// <exception cref="ArgumentNullException">Thrown when a required argument is <see langword="null"/>.</exception>
    /// <exception cref="ObjectDisposedException">Thrown when shutdown begins before the workspace is published.</exception>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is observed before publication.</exception>
    public async ValueTask<EngineResult<WorkspaceRevision>> OpenAsync(
        IFormListWorkspaceFactory factory,
        WorkspaceOpenRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(factory);
        ArgumentNullException.ThrowIfNull(request);

        var reserved = false;
        IFormListWorkspace? openedWorkspace = null;
        try
        {
            lock (SyncRoot)
            {
                ObjectDisposedException.ThrowIf(IsDisposing, this);
                if (Workspaces.ContainsKey(request.WorkspaceId) || OpeningWorkspaceIds.Contains(request.WorkspaceId))
                {
                    return EngineResult<WorkspaceRevision>.Failure(
                        new EngineError(
                            EngineErrorCode.InvalidRequest,
                            $"Workspace '{request.WorkspaceId:D}' is already open or opening."),
                        workspaceId: request.WorkspaceId);
                }

                OpeningWorkspaceIds.Add(request.WorkspaceId);
                PendingOpenCount++;
                reserved = true;
            }

            cancellationToken.ThrowIfCancellationRequested();
            var openResult = await factory.OpenAsync(request, cancellationToken).ConfigureAwait(false);
            if (!openResult.Succeeded)
            {
                return EngineResult<WorkspaceRevision>.Failure(
                    openResult.Error ?? new EngineError(
                        EngineErrorCode.UnexpectedFailure,
                        "The workspace factory returned a failure without an error."),
                    workspaceId: request.WorkspaceId,
                    operationId: openResult.OperationId,
                    baseRevision: openResult.BaseRevision,
                    resultRevision: openResult.ResultRevision,
                    warnings: openResult.Warnings);
            }

            openedWorkspace = openResult.Value;
            if (openedWorkspace is null)
            {
                return EngineResult<WorkspaceRevision>.Failure(
                    new EngineError(
                        EngineErrorCode.UnexpectedFailure,
                        "The workspace factory reported success without returning a workspace."),
                    workspaceId: request.WorkspaceId,
                    warnings: openResult.Warnings);
            }

            cancellationToken.ThrowIfCancellationRequested();
            if (openedWorkspace.WorkspaceId != request.WorkspaceId)
            {
                return EngineResult<WorkspaceRevision>.Failure(
                    new EngineError(
                        EngineErrorCode.UnexpectedFailure,
                        "The opened workspace identifier does not match the requested identifier."),
                    workspaceId: request.WorkspaceId,
                    warnings: openResult.Warnings);
            }

            var revision = openedWorkspace.Revision;
            lock (SyncRoot)
            {
                ObjectDisposedException.ThrowIf(IsDisposing, this);
                Workspaces.Add(request.WorkspaceId, new McpWorkspaceEntry(openedWorkspace));
            }

            openedWorkspace = null;
            return EngineResult<WorkspaceRevision>.Success(
                revision,
                workspaceId: request.WorkspaceId,
                resultRevision: revision,
                warnings: openResult.Warnings);
        }
        finally
        {
            try
            {
                if (openedWorkspace is not null)
                {
                    await openedWorkspace.DisposeAsync().ConfigureAwait(false);
                }
            }
            finally
            {
                if (reserved)
                {
                    ReleaseOpenReservation(request.WorkspaceId);
                }
            }
        }
    }

    /// <summary>Removes and disposes an active workspace.</summary>
    /// <param name="workspaceId">The exact workspace identifier to close.</param>
    /// <returns><see langword="true"/> when a workspace was removed and disposed; otherwise <see langword="false"/>.</returns>
    public async ValueTask<bool> CloseAsync(Guid workspaceId)
    {
        McpWorkspaceEntry? workspace;
        lock (SyncRoot)
        {
            if (!Workspaces.TryGetValue(workspaceId, out workspace) || !workspace.TryMarkClosing())
            {
                return false;
            }
        }

        try
        {
            await workspace.DisposeAsync().ConfigureAwait(false);
            return true;
        }
        finally
        {
            lock (SyncRoot)
            {
                Workspaces.Remove(workspaceId);
            }
        }
    }

    /// <summary>Runs one operation while preventing the selected workspace from closing underneath it.</summary>
    /// <typeparam name="T">The successful engine result value.</typeparam>
    /// <param name="workspaceId">The exact active workspace identifier.</param>
    /// <param name="operation">The operation that consumes the borrowed workspace within the guarded lifetime.</param>
    /// <param name="cancellationToken">A token that cancels while waiting for or running the workspace operation.</param>
    /// <returns>The operation result or a typed failure when the workspace is not active.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="operation"/> is <see langword="null"/>.</exception>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is observed.</exception>
    public ValueTask<EngineResult<T>> ExecuteAsync<T>(
        Guid workspaceId,
        Func<IFormListWorkspace, CancellationToken, ValueTask<EngineResult<T>>> operation,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(operation);

        McpWorkspaceEntry? workspace;
        lock (SyncRoot)
        {
            Workspaces.TryGetValue(workspaceId, out workspace);
        }

        if (workspace is null)
        {
            return ValueTask.FromResult(EngineResult<T>.Failure(
                new EngineError(
                    EngineErrorCode.WorkspaceDisposed,
                    $"Workspace '{workspaceId:D}' is not active."),
                workspaceId: workspaceId));
        }

        return workspace.ExecuteAsync(operation, cancellationToken);
    }

    /// <summary>Stops accepting workspaces, waits for in-flight opens to clean up, and disposes every active workspace once.</summary>
    /// <returns>A task that completes after every registry-owned workspace lifetime is released.</returns>
    public ValueTask DisposeAsync()
    {
        IReadOnlyList<McpWorkspaceEntry>? workspaceSnapshot = null;
        Task? pendingOpensTask = null;
        lock (SyncRoot)
        {
            if (!DisposalStarted)
            {
                DisposalStarted = true;
                IsDisposing = true;
                workspaceSnapshot = Workspaces.Values.ToArray();
                foreach (var workspace in workspaceSnapshot)
                {
                    workspace.TryMarkClosing();
                }

                Workspaces.Clear();
                pendingOpensTask = PendingOpenCount == 0
                    ? Task.CompletedTask
                    : PendingOpensDrained.Task;
            }
        }

        if (workspaceSnapshot is not null && pendingOpensTask is not null)
        {
            _ = DisposeCoreAsync(workspaceSnapshot, pendingOpensTask);
        }

        return new ValueTask(DisposalCompletion.Task);
    }

    /// <summary>Releases one open reservation and signals shutdown after the final pending open finishes.</summary>
    /// <param name="workspaceId">The reserved workspace identifier.</param>
    private void ReleaseOpenReservation(Guid workspaceId)
    {
        lock (SyncRoot)
        {
            OpeningWorkspaceIds.Remove(workspaceId);
            PendingOpenCount--;
            if (IsDisposing && PendingOpenCount == 0)
            {
                PendingOpensDrained.TrySetResult();
            }
        }
    }

    /// <summary>Performs the single asynchronous registry cleanup operation.</summary>
    /// <param name="workspaceSnapshot">The active workspaces transferred to disposal ownership.</param>
    /// <param name="pendingOpensTask">The task that observes cleanup of opens active at shutdown.</param>
    /// <returns>A task that completes when cleanup has succeeded or faulted.</returns>
    private async Task DisposeCoreAsync(
        IReadOnlyList<McpWorkspaceEntry> workspaceSnapshot,
        Task pendingOpensTask)
    {
        try
        {
            var disposalTasks = workspaceSnapshot
                .Select(TryDisposeWorkspaceAsync)
                .ToArray();
            await pendingOpensTask.ConfigureAwait(false);
            var failures = (await Task.WhenAll(disposalTasks).ConfigureAwait(false))
                .Where(exception => exception is not null)
                .Cast<Exception>()
                .ToArray();

            if (failures.Length != 0)
            {
                throw new AggregateException("One or more MCP workspaces could not be disposed.", failures);
            }

            DisposalCompletion.TrySetResult();
        }
        catch (Exception exception)
        {
            DisposalCompletion.TrySetException(exception);
        }
    }

    /// <summary>Attempts one workspace disposal while allowing independent entries to clean up concurrently.</summary>
    /// <param name="workspace">The registry entry transferred to shutdown ownership.</param>
    /// <returns>The disposal exception, or <see langword="null"/> when cleanup succeeds.</returns>
    private static async Task<Exception?> TryDisposeWorkspaceAsync(McpWorkspaceEntry workspace)
    {
        try
        {
            await workspace.DisposeAsync().ConfigureAwait(false);
            return null;
        }
        catch (Exception exception)
        {
            return exception;
        }
    }
}
