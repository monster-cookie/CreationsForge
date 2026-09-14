namespace CreationsForge.Services;

/// <summary>Identifies the application transition that must prove a workspace can be left safely.</summary>
public enum WorkspaceLeaveReason
{
    /// <summary>Replace the current workspace through the complete selection workflow.</summary>
    OpenWorkspace,

    /// <summary>Close the current workspace while keeping the plugin shell visible.</summary>
    CloseWorkspace,

    /// <summary>Replace the plugin shell with application settings.</summary>
    ShowSettings,

    /// <summary>Release application-owned resources and exit the desktop application.</summary>
    ExitApplication,
}
