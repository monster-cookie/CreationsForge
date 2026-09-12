namespace CreationsForge.Services.Interfaces;

/// <summary>
/// Navigates between the native workspace shell and application settings without entering legacy import workflows.
/// </summary>
public interface INativeApplicationNavigationService
{
    /// <summary>Shows a fresh native workspace shell in the application window.</summary>
    void ShowWorkspaceShell();

    /// <summary>Shows application settings only after the current native workspace can be left safely.</summary>
    /// <param name="cancellationToken">A token that cancels the guarded navigation attempt.</param>
    /// <returns><see langword="true"/> when Settings was published; otherwise <see langword="false"/>.</returns>
    Task<bool> TryShowSettingsAsync(CancellationToken cancellationToken = default);

    /// <summary>Reserves application shutdown only after the current native workspace can be left safely.</summary>
    /// <param name="cancellationToken">A token that cancels the guarded shutdown attempt.</param>
    /// <returns>A shutdown lease, or <see langword="null"/> when the user keeps editing.</returns>
    Task<NativeApplicationShutdownLease?> ReserveShutdownAsync(CancellationToken cancellationToken = default);
}
