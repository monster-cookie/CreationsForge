namespace CreationsForge.ViewModels;

/// <summary>Describes the editor's current single-flight native operation state.</summary>
public enum NativeFormListEditorOperationState
{
    /// <summary>No native editor operation is running or awaiting exact replay.</summary>
    Idle,

    /// <summary>A new, override, or existing-output session is being acquired.</summary>
    Beginning,

    /// <summary>A validated typed command is being applied.</summary>
    Applying,

    /// <summary>The bounded reference picker is selecting a value for one typed FormLink node.</summary>
    PickingReference,

    /// <summary>An immutable uncertain operation is being replayed with its original identity and payload.</summary>
    RetryingPendingOperation,

    /// <summary>A mutation outcome is unresolved and only exact pending-operation replay is permitted.</summary>
    PendingOutcome
}
