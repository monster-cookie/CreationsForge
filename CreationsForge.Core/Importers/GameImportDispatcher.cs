using CreationsForge.Core.DTOs.Results;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Importers.Interfaces;

namespace CreationsForge.Core.Importers;

public class GameImportDispatcher
{
    private readonly Dictionary<SupportedGame, IGameImporter> Importers;

    public GameImportDispatcher(IEnumerable<IGameImporter> importers)
    {
        Importers = importers.ToDictionary(importer => importer.Game);
    }

    public GameImportResultDTO Import(
        SupportedGame game,
        bool forceFullReimport = false,
        IProgress<GameImportProgressDTO>? progress = null,
        CancellationToken cancellationToken = default)
    {
        if (!Importers.TryGetValue(game, out var importer)) throw new InvalidOperationException($"No importer is registered for game '{game}'.");
        return importer.Import(forceFullReimport, progress, cancellationToken);
    }
}
