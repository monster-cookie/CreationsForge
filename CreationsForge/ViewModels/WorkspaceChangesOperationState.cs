namespace CreationsForge.ViewModels;

/// <summary>Identifies the current user-visible phase of the workspace change lifecycle.</summary>
public enum WorkspaceChangesOperationState
{
    /// <summary>No change-lifecycle operation is active.</summary>
    Idle,

    /// <summary>A workspace transition is waiting for exclusive presentation admission.</summary>
    ReservingTransition,

    /// <summary>The leave workflow is waiting for the current editor operation to finish naturally.</summary>
    WaitingForEditorOperation,

    /// <summary>A fresh atomic workspace state and preview are being captured.</summary>
    Reviewing,

    /// <summary>The selected plugin output is being saved.</summary>
    Saving,

    /// <summary>Staged workspace changes are being discarded through an exact output reopen.</summary>
    Discarding,

    /// <summary>The original save outcome is being inspected without mutating destination files.</summary>
    InspectingRecovery,

    /// <summary>An explicitly selected incomplete-save repair is mutating destination files.</summary>
    Repairing,

    /// <summary>Terminal recovery evidence is being adopted by the live workspace.</summary>
    AdoptingRecovery,

    /// <summary>The browser and editor are being refreshed after a known successful persistence transition.</summary>
    Refreshing,

    /// <summary>Cancellation was requested and the active operation is draining to a safe terminal result.</summary>
    CancelRequestedWaitingForResult,
}
