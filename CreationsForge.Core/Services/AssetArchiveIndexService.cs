using CreationsForge.Bethesda.Assets.Archives;
using CreationsForge.Bethesda.Assets.Resources;
using CreationsForge.Core.DTOs.Assets;
using CreationsForge.Core.DTOs.Results;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services.Interfaces;
using Serilog;

namespace CreationsForge.Core.Services;

public class AssetArchiveIndexService : IAssetArchiveIndexService
{
    private static readonly string[] ArchiveExtensions =
    {
        ".ba2",
        ".bsa"
    };

    private static readonly string[] ArchiveRootDirectories =
    {
        "Data",
        "Meshes",
        "Textures",
        "Materials",
        "Sound",
        "Music",
        "Scripts",
        "Interface",
        "Strings"
    };

    private readonly IAssetArchiveIndexRepository AssetArchiveIndexRepository;
    private readonly IReadOnlyList<IAssetArchiveReader> ArchiveReaders;
    private readonly ILogger Logger = Log.ForContext<AssetArchiveIndexService>();

    public AssetArchiveIndexService(
        IAssetArchiveIndexRepository assetArchiveIndexRepository,
        IEnumerable<IAssetArchiveReader> archiveReaders)
    {
        AssetArchiveIndexRepository = assetArchiveIndexRepository;
        ArchiveReaders = archiveReaders.ToList();
    }

    public AssetArchiveIndexResultDTO IndexGameArchives(
        SupportedGame game,
        string? dataFolder,
        IProgress<GameImportProgressDTO>? progress = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dataFolder) || !Directory.Exists(dataFolder))
        {
            Logger.Warning(
                "Asset archive indexing skipped for {Game}; data folder is not available: {DataFolder}",
                game,
                dataFolder);
            return new AssetArchiveIndexResultDTO();
        }

        var archives = GetArchives(dataFolder).ToList();
        var result = new AssetArchiveIndexResultDTO
        {
            ArchivesDiscovered = archives.Count
        };
        if (archives.Count == 0)
        {
            progress?.Report(new GameImportProgressDTO
            {
                StatusText = $"No {game} asset archives found.",
                DetailText = "Skipping asset archive index.",
                ProgressValue = 0,
                ProgressMaximum = 0,
                IsIndeterminate = false
            });
            return result;
        }

        for (var index = 0; index < archives.Count; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var archivePath = Path.GetFullPath(archives[index]);
            progress?.Report(new GameImportProgressDTO
            {
                StatusText = $"Indexing {game} asset archives",
                DetailText = Path.GetFileName(archivePath),
                ProgressValue = index + 1,
                ProgressMaximum = archives.Count,
                IsIndeterminate = false
            });

            var archiveReader = ArchiveReaders.FirstOrDefault(reader => reader.CanRead(archivePath));
            if (archiveReader == null)
            {
                result.ArchivesFailed++;
                Logger.Warning(
                    "Asset archive indexing skipped archive {ArchivePath} for {Game}; no registered reader can read it",
                    archivePath,
                    game);
                continue;
            }

            var indexResult = IndexArchive(game, dataFolder, archivePath, archiveReader);
            if (indexResult.Status == AssetArchiveIndexStatus.Current)
            {
                result.ArchivesSkippedCurrent++;
            }
            else if (indexResult.Status == AssetArchiveIndexStatus.Indexed)
            {
                result.ArchivesIndexed++;
                result.EntriesIndexed += indexResult.EntryCount;
            }
            else
            {
                result.ArchivesFailed++;
            }
        }

        Logger.Information(
            "Completed asset archive indexing for {Game}; discovered: {ArchivesDiscovered}, indexed: {ArchivesIndexed}, current: {ArchivesSkippedCurrent}, failed: {ArchivesFailed}, entries indexed: {EntriesIndexed}",
            game,
            result.ArchivesDiscovered,
            result.ArchivesIndexed,
            result.ArchivesSkippedCurrent,
            result.ArchivesFailed,
            result.EntriesIndexed);
        return result;
    }

    public BethesdaAssetReadResult TryReadArchiveAsset(SupportedGame game, string dataFolder, string assetPath)
    {
        var normalizedPath = NormalizeAssetPath(assetPath);
        var candidateEntryPaths = GetRelativePathCandidates(normalizedPath)
            .Select(NormalizeArchiveEntryPath)
            .ToList();
        var archives = GetArchives(dataFolder, normalizedPath).ToList();
        if (archives.Count == 0)
        {
            return new BethesdaAssetReadResult
            {
                OriginalPath = assetPath,
                DataFolder = dataFolder,
                SourceType = BethesdaAssetSourceType.None,
                Status = BethesdaAssetReadStatus.MissingLooseFile,
                StatusMessage = $"No archive files were found while resolving {assetPath}."
            };
        }

        var archiveAttemptMessages = new List<string>();
        foreach (var archive in archives)
        {
            var archivePath = Path.GetFullPath(archive);
            var archiveReader = ArchiveReaders.FirstOrDefault(reader => reader.CanRead(archivePath));
            if (archiveReader == null)
            {
                archiveAttemptMessages.Add($"{Path.GetFileName(archivePath)}: no registered archive reader.");
                continue;
            }

            var indexResult = IndexArchive(game, dataFolder, archivePath, archiveReader);
            if (indexResult.Status == AssetArchiveIndexStatus.Failed)
            {
                archiveAttemptMessages.Add($"{Path.GetFileName(archivePath)}: index failed: {indexResult.StatusMessage}");
                continue;
            }

            var entry = AssetArchiveIndexRepository.FindEntry(game, archivePath, candidateEntryPaths);
            if (entry == null)
            {
                continue;
            }

            var readResult = archiveReader.TryReadEntry(archivePath, entry.NormalizedEntryPath);
            if (readResult.IsSuccess && readResult.Data != null)
            {
                Logger.Information(
                    "Asset archive index resolved {AssetPath} from indexed entry {EntryPath} in {ArchivePath}",
                    assetPath,
                    readResult.EntryPath ?? entry.NormalizedEntryPath,
                    archivePath);
                return new BethesdaAssetReadResult
                {
                    OriginalPath = assetPath,
                    DataFolder = dataFolder,
                    SourceType = BethesdaAssetSourceType.Archive,
                    Status = BethesdaAssetReadStatus.ReadArchiveEntry,
                    Data = readResult.Data,
                    SourceArchivePath = archivePath,
                    NormalizedEntryPath = readResult.EntryPath ?? entry.NormalizedEntryPath,
                    StatusMessage = readResult.StatusMessage ?? $"Read archive asset {entry.NormalizedEntryPath} from {archivePath}."
                };
            }

            if (!string.IsNullOrWhiteSpace(readResult.StatusMessage))
            {
                archiveAttemptMessages.Add($"{Path.GetFileName(archivePath)} [{entry.NormalizedEntryPath}]: {readResult.StatusMessage}");
            }
        }

        return new BethesdaAssetReadResult
        {
            OriginalPath = assetPath,
            DataFolder = dataFolder,
            SourceType = BethesdaAssetSourceType.Archive,
            Status = ArchiveReaders.Count == 0 ? BethesdaAssetReadStatus.ArchiveReaderUnavailable : BethesdaAssetReadStatus.ArchiveEntryMissing,
            StatusMessage = ArchiveReaders.Count == 0
                ? $"Asset path {assetPath} appears archive-backed, but no archive reader is registered."
                : BuildArchiveFailureMessage(assetPath, archiveAttemptMessages)
        };
    }

    private AssetArchiveIndexAttemptResult IndexArchive(
        SupportedGame game,
        string dataFolder,
        string archivePath,
        IAssetArchiveReader archiveReader)
    {
        var fileInfo = new FileInfo(archivePath);
        var existing = AssetArchiveIndexRepository.GetArchiveFile(game, archivePath);
        if (existing != null &&
            existing.SourceLastWriteUTCTicks == fileInfo.LastWriteTimeUtc.Ticks &&
            existing.SourceFileSizeBytes == fileInfo.Length)
        {
            return new AssetArchiveIndexAttemptResult(AssetArchiveIndexStatus.Current, 0, null);
        }

        try
        {
            var entries = archiveReader.ListEntries(archivePath)
                .Select(entry => CreateEntryDTO(game, archivePath, entry))
                .ToList();
            AssetArchiveIndexRepository.SaveArchiveFile(new AssetArchiveFileDTO
            {
                Game = game,
                DataFolder = Path.GetFullPath(dataFolder),
                ArchivePath = Path.GetFullPath(archivePath),
                ArchiveFileName = fileInfo.Name,
                ArchiveExtension = fileInfo.Extension.ToLowerInvariant(),
                ArchiveType = GetArchiveType(fileInfo.Extension),
                SourceLastWriteUTCTicks = fileInfo.LastWriteTimeUtc.Ticks,
                SourceFileSizeBytes = fileInfo.Length,
                IndexedAtUTC = DateTime.UtcNow
            });
            AssetArchiveIndexRepository.ReplaceArchiveEntries(game, Path.GetFullPath(archivePath), entries);
            Logger.Information(
                "Indexed asset archive {ArchivePath} for {Game} with {EntryCount} entries",
                archivePath,
                game,
                entries.Count);
            return new AssetArchiveIndexAttemptResult(AssetArchiveIndexStatus.Indexed, entries.Count, null);
        }
        catch (Exception exception) when (exception is IOException or InvalidDataException or UnauthorizedAccessException or OverflowException)
        {
            AssetArchiveIndexRepository.DeleteArchive(game, archivePath);
            Logger.Warning(
                exception,
                "Unable to index asset archive {ArchivePath} for {Game}",
                archivePath,
                game);
            return new AssetArchiveIndexAttemptResult(AssetArchiveIndexStatus.Failed, 0, exception.Message);
        }
    }

    private static AssetArchiveEntryDTO CreateEntryDTO(SupportedGame game, string archivePath, AssetArchiveEntry entry)
    {
        var normalizedEntryPath = NormalizeArchiveEntryPath(entry.EntryPath);
        return new AssetArchiveEntryDTO
        {
            Game = game,
            ArchivePath = Path.GetFullPath(archivePath),
            NormalizedEntryPath = normalizedEntryPath,
            RootFolder = GetRootFolder(normalizedEntryPath),
            Extension = GetArchiveEntryExtension(normalizedEntryPath),
            PackedSize = entry.PackedSize,
            UnpackedSize = entry.UnpackedSize
        };
    }

    private static string BuildArchiveFailureMessage(string assetPath, IReadOnlyList<string> archiveAttemptMessages)
    {
        if (archiveAttemptMessages.Count == 0)
        {
            return $"Asset path {assetPath} was not found in the asset archive index.";
        }

        var shownMessages = archiveAttemptMessages.Take(8).ToList();
        var message = $"Asset path {assetPath} was not found in the asset archive index. Archive attempts: {string.Join(" | ", shownMessages)}";
        if (archiveAttemptMessages.Count > shownMessages.Count)
        {
            message += $" | {archiveAttemptMessages.Count - shownMessages.Count} more attempt(s).";
        }

        return message;
    }

    private static IEnumerable<string> GetArchives(string dataFolder, string normalizedPath)
    {
        return GetArchives(dataFolder)
            .Where(path => ArchiveExtensions.Contains(Path.GetExtension(path), StringComparer.OrdinalIgnoreCase))
            .OrderBy(path => GetArchivePreferenceScore(path, normalizedPath))
            .ThenBy(path => Path.GetFileName(path), StringComparer.OrdinalIgnoreCase);
    }

    private static IEnumerable<string> GetArchives(string dataFolder)
    {
        return Directory.EnumerateFiles(dataFolder)
            .Where(path => ArchiveExtensions.Contains(Path.GetExtension(path), StringComparer.OrdinalIgnoreCase))
            .OrderBy(path => Path.GetFileName(path), StringComparer.OrdinalIgnoreCase);
    }

    private static int GetArchivePreferenceScore(string archivePath, string normalizedPath)
    {
        var archiveName = Path.GetFileNameWithoutExtension(archivePath);
        if ((StartsWithDirectory(normalizedPath, "Meshes") ||
             StartsWithDirectory(normalizedPath, Path.Combine("Data", "Meshes"))) &&
            archiveName.Contains("meshes", StringComparison.OrdinalIgnoreCase))
        {
            return 0;
        }

        if ((StartsWithDirectory(normalizedPath, "Textures") ||
             StartsWithDirectory(normalizedPath, Path.Combine("Data", "Textures"))) &&
            archiveName.Contains("textures", StringComparison.OrdinalIgnoreCase))
        {
            return 0;
        }

        if ((StartsWithDirectory(normalizedPath, "Materials") ||
             StartsWithDirectory(normalizedPath, Path.Combine("Data", "Materials"))) &&
            archiveName.Contains("materials", StringComparison.OrdinalIgnoreCase))
        {
            return 0;
        }

        return 1;
    }

    private static IReadOnlyList<string> GetRelativePathCandidates(string normalizedPath)
    {
        var candidates = new List<string>
        {
            normalizedPath
        };

        AddRootStrippedPathCandidate(candidates, normalizedPath);
        AddDataPathCandidate(candidates, normalizedPath);
        if (!StartsWithDirectory(normalizedPath, "Meshes"))
        {
            var meshPath = Path.Combine("Meshes", normalizedPath);
            AddPathCandidate(candidates, meshPath);
            AddRootStrippedPathCandidate(candidates, meshPath);
        }

        return candidates;
    }

    private static void AddDataPathCandidate(List<string> candidates, string normalizedPath)
    {
        if (StartsWithDirectory(normalizedPath, "Data"))
        {
            return;
        }

        var dataPath = Path.Combine("Data", normalizedPath);
        AddPathCandidate(candidates, dataPath);
        AddRootStrippedPathCandidate(candidates, dataPath);
    }

    private static void AddRootStrippedPathCandidate(List<string> candidates, string normalizedPath)
    {
        foreach (var rootDirectory in ArchiveRootDirectories)
        {
            if (!StartsWithDirectory(normalizedPath, rootDirectory))
            {
                continue;
            }

            var strippedPath = normalizedPath.Length == rootDirectory.Length
                ? string.Empty
                : normalizedPath[(rootDirectory.Length + 1)..];
            AddPathCandidate(candidates, strippedPath);

            return;
        }
    }

    private static void AddPathCandidate(List<string> candidates, string path)
    {
        if (!string.IsNullOrWhiteSpace(path) &&
            !candidates.Contains(path, StringComparer.OrdinalIgnoreCase))
        {
            candidates.Add(path);
        }
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
        return new string(assetPath.Trim().Select(NormalizeArchiveEntryCharacter).ToArray());
    }

    private static char NormalizeArchiveEntryCharacter(char character)
    {
        if (character == '\\' || character == Path.DirectorySeparatorChar)
        {
            return '/';
        }

        if (character == ':' || character < 0x20 || character >= 0x7F)
        {
            return '_';
        }

        return char.ToLowerInvariant(character);
    }

    private static string GetRootFolder(string normalizedEntryPath)
    {
        var separatorIndex = normalizedEntryPath.IndexOf('/', StringComparison.Ordinal);
        return separatorIndex < 0 ? string.Empty : normalizedEntryPath[..separatorIndex];
    }

    private static string GetArchiveEntryExtension(string normalizedEntryPath)
    {
        var fileName = normalizedEntryPath;
        var separatorIndex = normalizedEntryPath.LastIndexOf('/');
        if (separatorIndex >= 0)
        {
            fileName = normalizedEntryPath[(separatorIndex + 1)..];
        }

        return Path.GetExtension(fileName).ToLowerInvariant();
    }

    private static string GetArchiveType(string archiveExtension)
    {
        return string.Equals(archiveExtension, ".ba2", StringComparison.OrdinalIgnoreCase) ? "BA2" : "BSA";
    }

    private enum AssetArchiveIndexStatus
    {
        Current,
        Indexed,
        Failed
    }

    private readonly struct AssetArchiveIndexAttemptResult
    {
        public AssetArchiveIndexAttemptResult(AssetArchiveIndexStatus status, long entryCount, string? statusMessage)
        {
            Status = status;
            EntryCount = entryCount;
            StatusMessage = statusMessage;
        }

        public AssetArchiveIndexStatus Status { get; }

        public long EntryCount { get; }

        public string? StatusMessage { get; }
    }
}
