namespace CreationsForge.Services;

/// <summary>Identifies the application transition that must prove a native workspace can be left safely.</summary>
public enum NativeWorkspaceLeaveReason
{
    /// <summary>Replace the current workspace through the complete selection workflow.</summary>
    OpenWorkspace,

    /// <summary>Close the current workspace while keeping the native shell visible.</summary>
    CloseWorkspace,

    /// <summary>Replace the native shell with application settings.</summary>
    ShowSettings,

    /// <summary>Release application-owned resources and exit the desktop application.</summary>
    ExitApplication,
}
