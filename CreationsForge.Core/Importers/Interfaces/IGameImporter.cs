using CreationsForge.Core.DTOs.Results;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.Importers.Interfaces;

public interface IGameImporter
{
    SupportedGame Game { get; }

    GameImportResultDTO Import(
        bool forceFullReimport = false,
        IProgress<GameImportProgressDTO>? progress = null,
        CancellationToken cancellationToken = default);
}
