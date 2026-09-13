namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Identifies a stable engine failure category that callers can handle without parsing error text.
/// </summary>
public enum EngineErrorCode
{
    /// <summary>The request is structurally invalid or contains inconsistent inputs.</summary>
    InvalidRequest,

    /// <summary>No registered game adapter supports the requested game and native release.</summary>
    UnsupportedGameRelease,

    /// <summary>Native source state could not be opened.</summary>
    SourceOpenFailed,

    /// <summary>A native source plugin declares a master that is absent from the explicit load order.</summary>
    MissingMaster,

    /// <summary>An explicit native input uses a format or capability unsupported by the selected release.</summary>
    UnsupportedInput,

    /// <summary>Native output state could not be opened.</summary>
    OutputOpenFailed,

    /// <summary>The workspace has already been disposed.</summary>
    WorkspaceDisposed,

    /// <summary>The request expected a different workspace revision.</summary>
    RevisionConflict,

    /// <summary>An operation identifier was reused with a different canonical request payload.</summary>
    OperationIdReuse,

    /// <summary>The workspace has retained its maximum number of replayable operation results.</summary>
    OperationCapacityExceeded,

    /// <summary>The exact operation is known but its bounded finalization replay result has expired.</summary>
    OperationReplayExpired,

    /// <summary>The requested operation requires an output that has not been selected.</summary>
    OutputNotSelected,

    /// <summary>The requested edit identifier is unknown to the workspace.</summary>
    EditNotFound,

    /// <summary>A requested native record was not found in the selected scope.</summary>
    RecordNotFound,

    /// <summary>The requested operation is not supported by the selected game adapter.</summary>
    UnsupportedOperation,

    /// <summary>Native or engine validation rejected the requested operation.</summary>
    ValidationFailed,

    /// <summary>An input or output file changed after its recorded baseline was established.</summary>
    ExternalChangeDetected,

    /// <summary>Another process or workspace currently owns the output directory save guard.</summary>
    OutputDirectoryBusy,

    /// <summary>A save crossed its first destination mutation and its final outcome is not yet known.</summary>
    CommitOutcomeUnknown,

    /// <summary>A recognized incomplete save requires an explicit repair decision.</summary>
    RepairRequired,

    /// <summary>No recognized recovery journal or guard metadata exists for the requested save.</summary>
    NoRecoveryEvidence,

    /// <summary>An unexpected engine or adapter failure prevented the operation from completing.</summary>
    UnexpectedFailure
}
