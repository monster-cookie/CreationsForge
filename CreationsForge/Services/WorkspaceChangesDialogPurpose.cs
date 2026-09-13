namespace CreationsForge.Services;

/// <summary>Identifies the closed workflow hosted by the shared workspace-changes dialog.</summary>
public enum WorkspaceChangesDialogPurpose
{
    /// <summary>Review all request-local and workspace-staged changes.</summary>
    Review,

    /// <summary>Review and save workspace-staged changes.</summary>
    Save,

    /// <summary>Review and discard request-local or workspace-staged changes.</summary>
    Discard,

    /// <summary>Resolve pending work before one named application transition.</summary>
    Leave,
}
