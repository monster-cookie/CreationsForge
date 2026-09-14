namespace CreationsForge.Services.Interfaces;

/// <summary>
/// Navigates between the workspace shell and application settings without entering legacy import workflows.
/// </summary>
public interface IApplicationNavigationService
{
    /// <summary>Shows a fresh workspace shell in the application window.</summary>
    void ShowWorkspaceShell();

    /// <summary>Shows application settings only after the current workspace can be left safely.</summary>
    /// <param name="cancellationToken">A token that cancels the guarded navigation attempt.</param>
    /// <returns><see langword="true"/> when Settings was published; otherwise <see langword="false"/>.</returns>
    Task<bool> TryShowSettingsAsync(CancellationToken cancellationToken = default);

    /// <summary>Reserves application shutdown only after the current workspace can be left safely.</summary>
    /// <param name="cancellationToken">A token that cancels the guarded shutdown attempt.</param>
    /// <returns>A shutdown lease, or <see langword="null"/> when the user keeps editing.</returns>
    Task<ApplicationShutdownLease?> ReserveShutdownAsync(CancellationToken cancellationToken = default);
}
