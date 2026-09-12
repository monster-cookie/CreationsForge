namespace CreationsForge.Services;

/// <summary>Identifies whether the shared changes dialog kept the current editor or completed its requested workflow.</summary>
public enum NativeWorkspaceChangesDialogResult
{
    /// <summary>Keep the current shell, workspace, and editor active.</summary>
    KeepEditing,

    /// <summary>Continue only after the view model has accepted and proved the requested workflow outcome.</summary>
    Proceed,
}
