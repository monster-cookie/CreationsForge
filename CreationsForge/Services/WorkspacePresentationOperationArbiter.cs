using System.ComponentModel;
using CreationsForge.Services.Interfaces;

namespace CreationsForge.Services;

/// <summary>Provides one thread-safe presentation admission boundary for editor work and workspace transitions.</summary>
public sealed class WorkspacePresentationOperationArbiter : IWorkspacePresentationOperationArbiter
{
    /// <summary>Protects editor ownership and the transition request count.</summary>
    private readonly object StateLock = new();

    /// <summary>Serializes queued workspace-transition reservations.</summary>
    private readonly SemaphoreSlim TransitionGate = new(1, 1);

    /// <summary>The exact editor lease currently holding admission.</summary>
    private WorkspaceEditorOperationLease? ActiveEditorOperation;

    /// <summary>The number of queued or acquired transition requests that collectively close editor admission.</summary>
    private int TransitionRequestCount;

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc />
    public bool IsEditorOperationActive
    {
        get
        {
            lock (StateLock)
            {
                return ActiveEditorOperation is not null;
            }
        }
    }

    /// <inheritdoc />
    public bool IsWorkspaceTransitionPendingOrReserved
    {
        get
        {
            lock (StateLock)
            {
                return TransitionRequestCount > 0;
            }
        }
    }

    /// <inheritdoc />
    public WorkspaceEditorOperationLease? TryBeginEditorOperation()
    {
        WorkspaceEditorOperationLease lease;
        lock (StateLock)
        {
            if (ActiveEditorOperation is not null || TransitionRequestCount > 0)
            {
                return null;
            }

            lease = new WorkspaceEditorOperationLease(ReleaseEditorOperation);
            ActiveEditorOperation = lease;
        }

        NotifyPropertyChanged(nameof(IsEditorOperationActive));
        return lease;
    }

    /// <inheritdoc />
    public async ValueTask<WorkspaceTransitionRequest> BeginWorkspaceTransitionRequestAsync(
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        WorkspaceEditorOperationLease? editorOperation;
        var notifyTransitionPending = false;
        lock (StateLock)
        {
            TransitionRequestCount++;
            notifyTransitionPending = TransitionRequestCount == 1;
            editorOperation = ActiveEditorOperation;
        }

        if (notifyTransitionPending)
        {
            NotifyPropertyChanged(nameof(IsWorkspaceTransitionPendingOrReserved));
        }

        var ownsTransitionGate = false;
        try
        {
            await TransitionGate.WaitAsync(cancellationToken).ConfigureAwait(false);
            ownsTransitionGate = true;
            cancellationToken.ThrowIfCancellationRequested();
            return new WorkspaceTransitionRequest(
                editorOperation,
                CreateTransferredTransitionLease,
                ReleaseAbandonedTransitionRequest);
        }
        catch
        {
            if (ownsTransitionGate)
            {
                TransitionGate.Release();
            }

            RemoveTransitionRequest();
            throw;
        }
    }

    /// <inheritdoc />
    public async ValueTask<WorkspaceTransitionLease> ReserveWorkspaceTransitionAsync(
        WorkspaceTransitionDrainMode drainMode,
        CancellationToken cancellationToken = default)
    {
        if (!Enum.IsDefined(drainMode))
        {
            throw new ArgumentOutOfRangeException(nameof(drainMode), drainMode, "The workspace-transition drain mode is unsupported.");
        }

        var request = await BeginWorkspaceTransitionRequestAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            return await request.CompleteAsync(drainMode, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            await request.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>Creates a lease that receives one pending request's existing gate and transition-count ownership.</summary>
    /// <returns>The active transition lease that becomes the sole release owner.</returns>
    private WorkspaceTransitionLease CreateTransferredTransitionLease()
    {
        return new WorkspaceTransitionLease(ReleaseWorkspaceTransition);
    }

    /// <summary>Releases the gate and transition count owned by one abandoned pending request.</summary>
    private void ReleaseAbandonedTransitionRequest()
    {
        TransitionGate.Release();
        RemoveTransitionRequest();
    }

    /// <summary>Releases the exact active editor lease and opens its terminal-drain signal.</summary>
    /// <param name="lease">The exact editor lease being disposed.</param>
    private void ReleaseEditorOperation(WorkspaceEditorOperationLease lease)
    {
        var released = false;
        lock (StateLock)
        {
            if (ReferenceEquals(ActiveEditorOperation, lease))
            {
                ActiveEditorOperation = null;
                released = true;
            }
        }

        if (released)
        {
            NotifyPropertyChanged(nameof(IsEditorOperationActive));
        }
    }

    /// <summary>Releases one acquired transition lease and preserves admission closure for queued requests.</summary>
    /// <param name="lease">The exact transition lease being disposed.</param>
    private void ReleaseWorkspaceTransition(WorkspaceTransitionLease lease)
    {
        ArgumentNullException.ThrowIfNull(lease);
        TransitionGate.Release();
        RemoveTransitionRequest();
    }

    /// <summary>Removes one canceled or released transition request and publishes admission reopening when it was the last request.</summary>
    private void RemoveTransitionRequest()
    {
        var notifyTransitionReleased = false;
        lock (StateLock)
        {
            TransitionRequestCount--;
            if (TransitionRequestCount < 0)
            {
                TransitionRequestCount = 0;
                throw new InvalidOperationException("The workspace-transition request count became unbalanced.");
            }

            notifyTransitionReleased = TransitionRequestCount == 0;
        }

        if (notifyTransitionReleased)
        {
            NotifyPropertyChanged(nameof(IsWorkspaceTransitionPendingOrReserved));
        }
    }

    /// <summary>Publishes one state-property transition without allowing observer failures to corrupt admission ownership.</summary>
    /// <param name="propertyName">The public state property that changed.</param>
    private void NotifyPropertyChanged(string propertyName)
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
            catch
            {
            }
        }
    }
}
