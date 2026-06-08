using CreationsForge.Core.DTOs.Results;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.Services.Interfaces;

public interface IGameImportWorkflowService
{
    Task<GameImportWorkflowResultDTO> ImportAsync(
        SupportedGame game,
        bool forceFullReimport = false,
        IProgress<GameImportProgressDTO>? progress = null,
        CancellationToken cancellationToken = default);
}
