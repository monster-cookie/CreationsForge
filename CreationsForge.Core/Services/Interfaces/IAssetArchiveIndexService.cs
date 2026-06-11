using CreationsForge.Bethesda.Assets.Resources;
using CreationsForge.Core.Enums;
using CreationsForge.Core.DTOs.Results;

namespace CreationsForge.Core.Services.Interfaces;

public interface IAssetArchiveIndexService
{
    AssetArchiveIndexResultDTO IndexGameArchives(
        SupportedGame game,
        string? dataFolder,
        IProgress<GameImportProgressDTO>? progress = null,
        CancellationToken cancellationToken = default);

    BethesdaAssetReadResult TryReadArchiveAsset(SupportedGame game, string dataFolder, string assetPath);
}
