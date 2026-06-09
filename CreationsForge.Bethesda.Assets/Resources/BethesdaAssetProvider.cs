using CreationsForge.Bethesda.Assets.Archives;

namespace CreationsForge.Bethesda.Assets.Resources;

public class BethesdaAssetProvider : IBethesdaAssetProvider
{
    private static readonly string[] ArchiveExtensions =
    {
        ".ba2",
        ".bsa"
    };

    private readonly IReadOnlyList<IAssetArchiveReader> ArchiveReaders;

    public BethesdaAssetProvider(IEnumerable<IAssetArchiveReader> archiveReaders)
    {
        ArchiveReaders = archiveReaders.ToList();
    }

    public BethesdaAssetReadResult TryReadAsset(BethesdaAssetReadRequest request)
    {
        var normalizedPath = NormalizeAssetPath(request.AssetPath);
        if (Path.IsPathRooted(normalizedPath))
        {
            return ReadAbsoluteLooseFile(normalizedPath);
        }

        if (string.IsNullOrWhiteSpace(request.DataFolder) || !Directory.Exists(request.DataFolder))
        {
            return new BethesdaAssetReadResult
            {
                OriginalPath = request.AssetPath,
                DataFolder = request.DataFolder,
                SourceType = BethesdaAssetSourceType.None,
                Status = BethesdaAssetReadStatus.MissingDataFolder,
                StatusMessage = "No readable data folder is available."
            };
        }

        var looseResult = ReadDataFolderLooseFile(request, normalizedPath);
        if (looseResult.IsSuccess)
        {
            return looseResult;
        }

        return ReadArchiveBackedAsset(request, normalizedPath, looseResult);
    }

    private static BethesdaAssetReadResult ReadAbsoluteLooseFile(string assetPath)
    {
        if (File.Exists(assetPath))
        {
            return new BethesdaAssetReadResult
            {
                OriginalPath = assetPath,
                ResolvedPath = assetPath,
                SourceType = BethesdaAssetSourceType.LooseFile,
                Status = BethesdaAssetReadStatus.ReadLooseFile,
                Data = File.ReadAllBytes(assetPath),
                StatusMessage = $"Read loose asset file {assetPath}."
            };
        }

        return new BethesdaAssetReadResult
        {
            OriginalPath = assetPath,
            SourceType = BethesdaAssetSourceType.None,
            Status = BethesdaAssetReadStatus.MissingAbsoluteFile,
            StatusMessage = $"Absolute asset file {assetPath} was not found."
        };
    }

    private static BethesdaAssetReadResult ReadDataFolderLooseFile(BethesdaAssetReadRequest request, string normalizedPath)
    {
        var result = new BethesdaAssetReadResult
        {
            OriginalPath = request.AssetPath,
            DataFolder = request.DataFolder,
            SourceType = BethesdaAssetSourceType.None,
            Status = BethesdaAssetReadStatus.MissingLooseFile,
            StatusMessage = $"No loose asset file was found for {request.AssetPath}."
        };

        var dataFolder = Path.GetFullPath(request.DataFolder!);
        foreach (var relativePath in GetRelativePathCandidates(normalizedPath))
        {
            var resolvedPath = Path.GetFullPath(Path.Combine(dataFolder, relativePath));
            result.SearchedPaths.Add(resolvedPath);
            if (!resolvedPath.StartsWith(dataFolder, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (File.Exists(resolvedPath))
            {
                result.ResolvedPath = resolvedPath;
                result.SourceType = BethesdaAssetSourceType.LooseFile;
                result.Status = BethesdaAssetReadStatus.ReadLooseFile;
                result.Data = File.ReadAllBytes(resolvedPath);
                result.StatusMessage = $"Read loose asset file {resolvedPath}.";
                return result;
            }
        }

        return result;
    }

    private BethesdaAssetReadResult ReadArchiveBackedAsset(
        BethesdaAssetReadRequest request,
        string normalizedPath,
        BethesdaAssetReadResult looseResult)
    {
        var archives = GetArchives(request.DataFolder!).ToList();
        if (archives.Count == 0)
        {
            return looseResult;
        }

        foreach (var archivePath in archives)
        {
            var archiveReader = ArchiveReaders.FirstOrDefault(reader => reader.CanRead(archivePath));
            if (archiveReader == null)
            {
                continue;
            }

            foreach (var relativePath in GetRelativePathCandidates(normalizedPath))
            {
                var readResult = archiveReader.TryReadEntry(archivePath, NormalizeArchiveEntryPath(relativePath));
                if (readResult.IsSuccess && readResult.Data != null)
                {
                    return new BethesdaAssetReadResult
                    {
                        OriginalPath = request.AssetPath,
                        DataFolder = request.DataFolder,
                        SourceType = BethesdaAssetSourceType.Archive,
                        Status = BethesdaAssetReadStatus.ReadArchiveEntry,
                        Data = readResult.Data,
                        SourceArchivePath = archivePath,
                        NormalizedEntryPath = readResult.EntryPath ?? NormalizeArchiveEntryPath(relativePath),
                        StatusMessage = readResult.StatusMessage ?? $"Read archive asset {relativePath} from {archivePath}."
                    };
                }
            }
        }

        return new BethesdaAssetReadResult
        {
            OriginalPath = request.AssetPath,
            DataFolder = request.DataFolder,
            SourceType = BethesdaAssetSourceType.Archive,
            Status = ArchiveReaders.Count == 0 ? BethesdaAssetReadStatus.ArchiveReaderUnavailable : BethesdaAssetReadStatus.ArchiveEntryMissing,
            StatusMessage = ArchiveReaders.Count == 0
                ? $"Asset path {request.AssetPath} appears archive-backed, but no archive reader is registered."
                : $"Asset path {request.AssetPath} was not found in registered archive readers."
        };
    }

    private static IEnumerable<string> GetArchives(string dataFolder)
    {
        return Directory.EnumerateFiles(dataFolder)
            .Where(path => ArchiveExtensions.Contains(Path.GetExtension(path), StringComparer.OrdinalIgnoreCase));
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

    private static string NormalizeArchiveEntryPath(string assetPath)
    {
        return assetPath.Trim()
            .Replace('\\', '/')
            .Replace(Path.DirectorySeparatorChar, '/');
    }
}
