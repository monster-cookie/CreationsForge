using CreationsForge.Core.DTOs.Results;

namespace CreationsForge.Core.Services.Interfaces;

public interface IProcessTerminationDiagnosticsService : IDisposable
{
    CancellationToken TerminationToken { get; }

    void StartSession(string surfaceName, string? logPath);

    void UpdateHeartbeat(string phaseName, GameImportProgressDTO? progress = null);

    void MarkCleanShutdown(string reason);
}
