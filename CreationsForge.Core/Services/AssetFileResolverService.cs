using CreationsForge.Bethesda.Assets.Files;
using CreationsForge.Bethesda.Assets.Resources;
using CreationsForge.Core.DTOs.Assets;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Services.Interfaces;
using Serilog;

namespace CreationsForge.Core.Services;

public class AssetFileResolverService : IAssetFileResolverService
{
    private readonly IReadOnlyList<IGameMetadataService> GameMetadataServices;
    private readonly IBethesdaAssetProvider BethesdaAssetProvider;
    private readonly ILogger Logger = Log.ForContext<AssetFileResolverService>();

    public AssetFileResolverService(IEnumerable<IGameMetadataService> gameMetadataServices, IBethesdaAssetProvider bethesdaAssetProvider)
    {
        GameMetadataServices = gameMetadataServices.ToList();
        BethesdaAssetProvider = bethesdaAssetProvider;
    }

    public AssetFileResolutionDTO ResolveAssetFile(AssetPreviewCandidateDTO candidate)
    {
        var dataFolder = Path.IsPathRooted(candidate.MeshPath) ? null : GetDataFolder(candidate.Game);
        var assetReadResult = BethesdaAssetProvider.TryReadAsset(new BethesdaAssetReadRequest
        {
            AssetPath = candidate.MeshPath,
            DataFolder = dataFolder
        });
        var resolution = CreateResolution(assetReadResult, candidate);
        if (resolution.Status == AssetFileResolutionStatus.MissingDataFolder)
        {
            Logger.Warning(
                "Asset file resolver could not locate a data folder for {Game} while resolving {AssetPath}",
                candidate.Game,
                candidate.MeshPath);
        }

        return resolution;
    }

    private static AssetFileResolutionDTO CreateResolution(BethesdaAssetReadResult assetReadResult, AssetPreviewCandidateDTO candidate)
    {
        var result = new AssetFileResolutionDTO
        {
            OriginalPath = assetReadResult.OriginalPath,
            ResolvedPath = assetReadResult.ResolvedPath,
            Data = assetReadResult.Data,
            DataFolder = assetReadResult.DataFolder,
            SourceArchivePath = assetReadResult.SourceArchivePath,
            NormalizedEntryPath = assetReadResult.NormalizedEntryPath,
            Status = MapStatus(assetReadResult.Status),
            StatusMessage = CreateStatusMessage(assetReadResult, candidate)
        };

        foreach (var searchedPath in assetReadResult.SearchedPaths)
        {
            result.SearchedPaths.Add(searchedPath);
        }

        return result;
    }

    private string? GetDataFolder(SupportedGame game)
    {
        var metadataService = GameMetadataServices.FirstOrDefault(service => service.Game == game);
        return metadataService?.GetGame().DataFolder;
    }

    private static AssetFileResolutionStatus MapStatus(BethesdaAssetReadStatus status)
    {
        return status switch
        {
            BethesdaAssetReadStatus.ReadLooseFile => AssetFileResolutionStatus.ResolvedLooseFile,
            BethesdaAssetReadStatus.ReadArchiveEntry => AssetFileResolutionStatus.ResolvedArchiveEntryInMemory,
            BethesdaAssetReadStatus.MissingAbsoluteFile => AssetFileResolutionStatus.MissingAbsoluteFile,
            BethesdaAssetReadStatus.MissingDataFolder => AssetFileResolutionStatus.MissingDataFolder,
            BethesdaAssetReadStatus.MissingLooseFile => AssetFileResolutionStatus.MissingLooseFile,
            BethesdaAssetReadStatus.ArchiveReaderUnavailable => AssetFileResolutionStatus.ArchiveExtractionUnsupported,
            BethesdaAssetReadStatus.ArchiveEntryMissing => AssetFileResolutionStatus.ArchiveExtractionUnsupported,
            _ => AssetFileResolutionStatus.MissingLooseFile
        };
    }

    private static string CreateStatusMessage(BethesdaAssetReadResult assetReadResult, AssetPreviewCandidateDTO candidate)
    {
        if (assetReadResult.Status == BethesdaAssetReadStatus.ReadArchiveEntry)
        {
            return $"Read archive-backed asset path {candidate.MeshPath} into memory. Parser integration is pending.";
        }

        if (assetReadResult.Status == BethesdaAssetReadStatus.ArchiveReaderUnavailable)
        {
            return $"Asset path {candidate.MeshPath} appears archive-backed. BA2/BSA extraction is not implemented yet.";
        }

        return assetReadResult.StatusMessage;
    }
}
