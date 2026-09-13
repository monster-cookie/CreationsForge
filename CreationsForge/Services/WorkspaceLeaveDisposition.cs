namespace CreationsForge.Services;

/// <summary>Identifies the closed proof that authorizes one guarded application transition.</summary>
public enum WorkspaceLeaveDisposition
{
    /// <summary>The current workspace is ready, has no staged changes, and has no request-local editor work.</summary>
    ReadyAndClean,

    /// <summary>No workspace exists while presentation admission remains reserved.</summary>
    NoWorkspace,

    /// <summary>An explicit conflict-abandonment confirmation authorizes only transactional workspace replacement.</summary>
    ConfirmedAbandonmentForOpen,
}
