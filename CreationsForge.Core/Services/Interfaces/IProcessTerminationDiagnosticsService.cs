namespace CreationsForge.Core.Services.Interfaces;

/// <summary>
/// Records process-session diagnostics and exposes cancellation when process termination is requested.
/// </summary>
public interface IProcessTerminationDiagnosticsService : IDisposable
{
    /// <summary>
    /// Gets the token canceled when the process receives a supported termination request.
    /// </summary>
    CancellationToken TerminationToken { get; }

    /// <summary>
    /// Starts a diagnostic session unless one is already active.
    /// </summary>
    /// <param name="surfaceName">The application surface being monitored.</param>
    /// <param name="logPath">The active log path, or <see langword="null"/> when unavailable.</param>
    void StartSession(string surfaceName, string? logPath);

    /// <summary>
    /// Records the current application phase and a fresh process-resource snapshot.
    /// </summary>
    /// <param name="phaseName">The current diagnostic phase.</param>
    void UpdateHeartbeat(string phaseName);

    /// <summary>
    /// Marks the active diagnostic session as having shut down cleanly.
    /// </summary>
    /// <param name="reason">The completed shutdown reason.</param>
    void MarkCleanShutdown(string reason);
}
