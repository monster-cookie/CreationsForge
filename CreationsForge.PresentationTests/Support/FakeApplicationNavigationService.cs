using CreationsForge.Services;
using CreationsForge.Services.Interfaces;

namespace CreationsForge.PresentationTests.Support;

/// <summary>
/// Records plugin shell and settings navigation without creating controls.
/// </summary>
internal sealed class FakeApplicationNavigationService : IApplicationNavigationService
{
    /// <summary>Gets how many times the plugin shell was requested.</summary>
    public int WorkspaceShellCount { get; private set; }

    /// <summary>Gets how many times settings were requested.</summary>
    public int SettingsCount { get; private set; }

    /// <summary>Gets how many times guarded shutdown was requested.</summary>
    public int ShutdownReservationCount { get; private set; }

    /// <summary>Gets or sets whether guarded settings navigation succeeds.</summary>
    public bool SettingsResult { get; set; } = true;

    /// <summary>Gets or sets optional awaited Settings behavior for shell transition tests.</summary>
    public Func<CancellationToken, Task<bool>>? SettingsAction { get; set; }

    /// <summary>Gets or sets the lease returned for guarded shutdown.</summary>
    public ApplicationShutdownLease? ShutdownLease { get; set; } = ApplicationShutdownLease.CreateForSettings();

    /// <summary>Gets or sets optional awaited shutdown-reservation behavior.</summary>
    public Func<CancellationToken, Task<ApplicationShutdownLease?>>? ShutdownAction { get; set; }

    /// <inheritdoc />
    public void ShowWorkspaceShell()
    {
        WorkspaceShellCount++;
    }

    /// <inheritdoc />
    public Task<bool> TryShowSettingsAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        SettingsCount++;
        return SettingsAction?.Invoke(cancellationToken) ?? Task.FromResult(SettingsResult);
    }

    /// <inheritdoc />
    public Task<ApplicationShutdownLease?> ReserveShutdownAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ShutdownReservationCount++;
        return ShutdownAction?.Invoke(cancellationToken) ?? Task.FromResult(ShutdownLease);
    }
}
