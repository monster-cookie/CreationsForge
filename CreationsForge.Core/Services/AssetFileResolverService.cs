using CreationsForge.Bethesda.Assets.Files;
using CreationsForge.Core.DTOs.Assets;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Services.Interfaces;
using Serilog;

namespace CreationsForge.Core.Services;

public class AssetFileResolverService : IAssetFileResolverService
{
    private static readonly string[] ArchiveExtensions =
    {
        ".ba2",
        ".bsa"
    };

    private readonly IReadOnlyList<IGameMetadataService> GameMetadataServices;
    private readonly ILogger Logger = Log.ForContext<AssetFileResolverService>();

    public AssetFileResolverService(IEnumerable<IGameMetadataService> gameMetadataServices)
    {
        GameMetadataServices = gameMetadataServices.ToList();
    }

    public AssetFileResolutionDTO ResolveAssetFile(AssetPreviewCandidateDTO candidate)
    {
        var normalizedPath = NormalizeAssetPath(candidate.MeshPath);
        if (Path.IsPathRooted(normalizedPath))
        {
            return ResolveAbsolutePath(normalizedPath);
        }

        var dataFolder = GetDataFolder(candidate.Game);
        if (string.IsNullOrWhiteSpace(dataFolder) || !Directory.Exists(dataFolder))
        {
            Logger.Warning(
                "Asset file resolver could not locate a data folder for {Game} while resolving {AssetPath}",
                candidate.Game,
                candidate.MeshPath);
            return new AssetFileResolutionDTO
            {
                OriginalPath = candidate.MeshPath,
                DataFolder = dataFolder,
                Status = AssetFileResolutionStatus.MissingDataFolder,
                StatusMessage = $"No readable data folder is available for {candidate.Game}."
            };
        }

        var result = ResolveLooseFile(candidate, normalizedPath, dataFolder);
        if (result.IsResolved)
        {
            return result;
        }

        if (HasArchives(dataFolder))
        {
            result.Status = AssetFileResolutionStatus.ArchiveExtractionUnsupported;
            result.StatusMessage = $"Asset path {candidate.MeshPath} appears archive-backed. BA2/BSA extraction is not implemented yet.";
            return result;
        }

        return result;
    }

    private AssetFileResolutionDTO ResolveAbsolutePath(string assetPath)
    {
        if (File.Exists(assetPath))
        {
            return new AssetFileResolutionDTO
            {
                OriginalPath = assetPath,
                ResolvedPath = assetPath,
                Status = AssetFileResolutionStatus.ResolvedLooseFile,
                StatusMessage = $"Resolved loose asset file {assetPath}."
            };
        }

        return new AssetFileResolutionDTO
        {
            OriginalPath = assetPath,
            Status = AssetFileResolutionStatus.MissingAbsoluteFile,
            StatusMessage = $"Absolute asset file {assetPath} was not found."
        };
    }

    private AssetFileResolutionDTO ResolveLooseFile(AssetPreviewCandidateDTO candidate, string normalizedPath, string dataFolder)
    {
        var result = new AssetFileResolutionDTO
        {
            OriginalPath = candidate.MeshPath,
            DataFolder = dataFolder,
            Status = AssetFileResolutionStatus.MissingLooseFile,
            StatusMessage = $"No loose asset file was found for {candidate.MeshPath}."
        };

        foreach (var relativePath in GetRelativePathCandidates(normalizedPath))
        {
            var resolvedPath = Path.GetFullPath(Path.Combine(dataFolder, relativePath));
            result.SearchedPaths.Add(resolvedPath);
            if (!resolvedPath.StartsWith(Path.GetFullPath(dataFolder), StringComparison.OrdinalIgnoreCase))
            {
                Logger.Warning(
                    "Asset file resolver skipped path outside data folder while resolving {AssetPath}: {ResolvedPath}",
                    candidate.MeshPath,
                    resolvedPath);
                continue;
            }

            if (File.Exists(resolvedPath))
            {
                result.ResolvedPath = resolvedPath;
                result.Status = AssetFileResolutionStatus.ResolvedLooseFile;
                result.StatusMessage = $"Resolved loose asset file {resolvedPath}.";
                return result;
            }
        }

        return result;
    }

    private string? GetDataFolder(SupportedGame game)
    {
        var metadataService = GameMetadataServices.FirstOrDefault(service => service.Game == game);
        return metadataService?.GetGame().DataFolder;
    }

    private static IReadOnlyList<string> GetRelativePathCandidates(string normalizedPath)
    {
        var candidates = new List<string>
        {
            normalizedPath
        };

        if (!StartsWithDirectory(normalizedPath, "Meshes"))
        {
            candidates.Add(Path.Combine("Meshes", normalizedPath));
        }

        return candidates;
    }

    private static bool StartsWithDirectory(string path, string directoryName)
    {
        return string.Equals(path, directoryName, StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith(directoryName + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizeAssetPath(string assetPath)
    {
        return assetPath.Trim()
            .Replace('\\', Path.DirectorySeparatorChar)
            .Replace('/', Path.DirectorySeparatorChar);
    }

    private static bool HasArchives(string dataFolder)
    {
        return Directory.EnumerateFiles(dataFolder)
            .Any(path => ArchiveExtensions.Contains(Path.GetExtension(path), StringComparer.OrdinalIgnoreCase));
    }
}
