using CreationsForge.Core.DTOs.Results;

namespace CreationsForge.Core.Services.Interfaces;

public interface IAllGamesImportWorkflowService
{
    Task<AllGamesImportWorkflowResultDTO> ImportAllAsync(
        bool resetDatabase,
        IProgress<GameImportProgressDTO>? progress = null,
        CancellationToken cancellationToken = default);
}
