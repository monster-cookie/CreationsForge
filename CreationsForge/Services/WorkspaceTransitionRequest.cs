namespace CreationsForge.Services;

/// <summary>Owns a queued workspace-transition position while its caller chooses how to drain the exact captured editor operation.</summary>
public sealed class WorkspaceTransitionRequest : IAsyncDisposable
{
    /// <summary>Protects the single-use request lifecycle.</summary>
    private readonly object LifecycleLock = new();

    /// <summary>The exact editor lease captured when this request first closed editor admission.</summary>
    private readonly WorkspaceEditorOperationLease? CapturedEditorOperation;

    /// <summary>Creates the transition lease that receives this request's existing admission ownership.</summary>
    private readonly Func<WorkspaceTransitionLease> TransferOwnership;

    /// <summary>Releases this request's gate position and transition count when ownership is abandoned.</summary>
    private readonly Action ReleaseAbandonedOwnership;

    /// <summary>Interrupts an in-progress drain when asynchronous disposal abandons the request.</summary>
    private readonly CancellationTokenSource AbandonmentCancellation = new();

    /// <summary>Signals that completion has transferred or released ownership and finished its cleanup.</summary>
    private readonly TaskCompletionSource Completion = new(TaskCreationOptions.RunContinuationsAsynchronously);

    /// <summary>The current single-use lifecycle state.</summary>
    private WorkspaceTransitionRequestState State = WorkspaceTransitionRequestState.Pending;

    /// <summary>Tracks exactly-once release of ownership that was not transferred.</summary>
    private int IsAbandonedOwnershipReleased;

    /// <summary>Tracks exactly-once disposal of the internal abandonment source.</summary>
    private int IsAbandonmentSourceDisposed;

    /// <summary>Initializes one request that already owns a transition gate position and count.</summary>
    /// <param name="capturedEditorOperation">The exact editor lease captured while admission closed, or <see langword="null"/>.</param>
    /// <param name="transferOwnership">Creates the lease that receives existing gate and count ownership.</param>
    /// <param name="releaseAbandonedOwnership">Releases existing gate and count ownership after abandonment.</param>
    /// <exception cref="ArgumentNullException">Thrown when an ownership callback is <see langword="null"/>.</exception>
    internal WorkspaceTransitionRequest(
        WorkspaceEditorOperationLease? capturedEditorOperation,
        Func<WorkspaceTransitionLease> transferOwnership,
        Action releaseAbandonedOwnership)
    {
        ArgumentNullException.ThrowIfNull(transferOwnership);
        ArgumentNullException.ThrowIfNull(releaseAbandonedOwnership);
        CapturedEditorOperation = capturedEditorOperation;
        TransferOwnership = transferOwnership;
        ReleaseAbandonedOwnership = releaseAbandonedOwnership;
    }

    /// <summary>Gets whether the exact captured editor operation is still active for prompt presentation.</summary>
    public bool IsEditorOperationActive => CapturedEditorOperation?.IsActive == true;

    /// <summary>Applies one drain choice to the exact captured editor and transfers continuous admission to a transition lease.</summary>
    /// <param name="drainMode">Whether to wait naturally or first request cancellation of the captured editor operation.</param>
    /// <param name="cancellationToken">A token that abandons this request if cancellation wins before ownership transfers.</param>
    /// <returns>The transition lease that exclusively owns this request's existing gate position and transition count.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="drainMode"/> is not a defined mode.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the request is no longer pending and cannot be completed again.</exception>
    /// <exception cref="OperationCanceledException">Thrown when caller cancellation or concurrent disposal abandons completion before ownership transfers.</exception>
    public async ValueTask<WorkspaceTransitionLease> CompleteAsync(
        WorkspaceTransitionDrainMode drainMode,
        CancellationToken cancellationToken = default)
    {
        if (!Enum.IsDefined(drainMode))
        {
            throw new ArgumentOutOfRangeException(nameof(drainMode), drainMode, "The workspace-transition drain mode is unsupported.");
        }

        cancellationToken.ThrowIfCancellationRequested();
        lock (LifecycleLock)
        {
            if (State != WorkspaceTransitionRequestState.Pending)
            {
                throw new InvalidOperationException("The workspace-transition request has already been completed or abandoned.");
            }

            State = WorkspaceTransitionRequestState.Completing;
        }

        if (drainMode == WorkspaceTransitionDrainMode.CancelAndWaitForCurrentOperation)
        {
            CapturedEditorOperation?.RequestCancellation();
        }

        try
        {
            if (CapturedEditorOperation is not null)
            {
                using var linkedCancellation = CancellationTokenSource.CreateLinkedTokenSource(
                    cancellationToken,
                    AbandonmentCancellation.Token);
                await CapturedEditorOperation.DrainTask.WaitAsync(linkedCancellation.Token).ConfigureAwait(false);
            }

            cancellationToken.ThrowIfCancellationRequested();
            lock (LifecycleLock)
            {
                if (State != WorkspaceTransitionRequestState.Completing)
                {
                    throw new OperationCanceledException(
                        "The workspace-transition request was abandoned before admission ownership transferred.",
                        AbandonmentCancellation.Token);
                }

                var lease = TransferOwnership();
                State = WorkspaceTransitionRequestState.Transferred;
                return lease;
            }
        }
        catch
        {
            lock (LifecycleLock)
            {
                if (State == WorkspaceTransitionRequestState.Completing)
                {
                    State = WorkspaceTransitionRequestState.Abandoned;
                }
            }

            ReleaseOwnershipOnce();
            throw;
        }
        finally
        {
            Completion.TrySetResult();
            DisposeAbandonmentSourceOnce();
        }
    }

    /// <summary>Abandons an untransferred request, releasing admission without canceling or waiting for a pending captured editor.</summary>
    /// <returns>A value task that completes after concurrent completion has released or transferred ownership.</returns>
    public ValueTask DisposeAsync()
    {
        Task? completionToAwait = null;
        var releaseImmediately = false;
        var interruptCompletion = false;
        lock (LifecycleLock)
        {
            switch (State)
            {
                case WorkspaceTransitionRequestState.Pending:
                    State = WorkspaceTransitionRequestState.Abandoned;
                    releaseImmediately = true;
                    break;
                case WorkspaceTransitionRequestState.Completing:
                    State = WorkspaceTransitionRequestState.Abandoned;
                    interruptCompletion = true;
                    completionToAwait = Completion.Task;
                    break;
                case WorkspaceTransitionRequestState.Abandoned:
                    completionToAwait = Completion.Task;
                    break;
                case WorkspaceTransitionRequestState.Transferred:
                    break;
                default:
                    throw new InvalidOperationException("The workspace-transition request entered an unsupported lifecycle state.");
            }
        }

        if (interruptCompletion)
        {
            RequestCompletionAbandonment();
        }

        if (releaseImmediately)
        {
            try
            {
                ReleaseOwnershipOnce();
            }
            finally
            {
                Completion.TrySetResult();
                DisposeAbandonmentSourceOnce();
            }
        }

        return completionToAwait is null || completionToAwait.IsCompleted
            ? ValueTask.CompletedTask
            : new ValueTask(completionToAwait);
    }

    /// <summary>Signals asynchronous abandonment without exposing the internal token or allowing observer failures to escape.</summary>
    private void RequestCompletionAbandonment()
    {
        try
        {
            AbandonmentCancellation.Cancel();
        }
        catch (ObjectDisposedException)
        {
        }
        catch (AggregateException)
        {
        }
    }

    /// <summary>Releases untransferred gate and count ownership exactly once.</summary>
    private void ReleaseOwnershipOnce()
    {
        if (Interlocked.Exchange(ref IsAbandonedOwnershipReleased, 1) == 0)
        {
            ReleaseAbandonedOwnership();
        }
    }

    /// <summary>Disposes the internal abandonment source exactly once after request cleanup is complete.</summary>
    private void DisposeAbandonmentSourceOnce()
    {
        if (Interlocked.Exchange(ref IsAbandonmentSourceDisposed, 1) == 0)
        {
            AbandonmentCancellation.Dispose();
        }
    }

    /// <summary>Defines the closed single-use lifecycle for request ownership.</summary>
    private enum WorkspaceTransitionRequestState
    {
        /// <summary>The request owns admission and has not accepted a drain choice.</summary>
        Pending,
        /// <summary>The request accepted one drain choice and is waiting to transfer ownership.</summary>
        Completing,
        /// <summary>The request transferred ownership to one transition lease.</summary>
        Transferred,
        /// <summary>The request was consumed without transferring ownership.</summary>
        Abandoned,
    }
}
