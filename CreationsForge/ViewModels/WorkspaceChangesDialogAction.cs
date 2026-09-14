namespace CreationsForge.ViewModels;

/// <summary>Identifies one closed user choice forwarded by the shared change dialog.</summary>
internal enum WorkspaceChangesDialogChoice
{
    /// <summary>Wait for the captured editor operation to publish its exact terminal result.</summary>
    WaitForEditorOperation,

    /// <summary>Request cancellation of the captured editor operation and wait for its exact terminal result.</summary>
    CancelEditorOperationAndWait,

    /// <summary>Close the dialog and retain the current editor and workspace state.</summary>
    KeepEditing,

    /// <summary>Save all staged workspace changes and proceed only after a fresh clean proof.</summary>
    SaveAndProceed,

    /// <summary>Explicitly discard request-local and staged workspace changes and proceed only after a fresh clean proof.</summary>
    DiscardAndProceed,

    /// <summary>Close the dialog so an exact pending editor operation can be resolved in the editor.</summary>
    ReturnToEditor,

    /// <summary>Confirm Open-only abandonment of an externally conflicted current workspace.</summary>
    ConfirmAbandonmentForOpen,
}

/// <summary>Tells the dialog adapter whether a forwarded choice changed or completed the active workflow.</summary>
internal enum WorkspaceLeaveChoiceOutcome
{
    /// <summary>Keep the dialog open because the choice did not establish a terminal disposition.</summary>
    ContinueDialog,

    /// <summary>Close the dialog and keep the current editor and workspace active.</summary>
    KeepEditing,

    /// <summary>Close the dialog so the view model can repeat final proof and transfer its exact transition lease.</summary>
    Proceed,
}
