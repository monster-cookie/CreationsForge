namespace CreationsForge.Services;

/// <summary>Defines how a workspace transition drains an editor operation that already owns admission.</summary>
public enum NativeWorkspaceTransitionDrainMode
{
    /// <summary>Waits for the current editor operation to finish without requesting cancellation.</summary>
    WaitForCurrentOperation,

    /// <summary>Requests cancellation through the editor lease and then waits for terminal drain.</summary>
    CancelAndWaitForCurrentOperation,
}
