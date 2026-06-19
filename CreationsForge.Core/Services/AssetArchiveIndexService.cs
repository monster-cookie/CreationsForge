using CreationsForge.Bethesda.Assets.Archives;
using CreationsForge.Bethesda.Assets.Resources;
using CreationsForge.Core.DTOs.Assets;
using CreationsForge.Core.DTOs.Results;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services.Interfaces;
using System.Data.Common;
using System.Diagnostics;
using System.Data.SQLite;
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

    private static readonly string[] ImportArchiveNameTokens =
    {
        "meshes",
        "textures",
        "materials",
        "misc",
        "main"
    };

    private static readonly string[] MeshArchiveNameTokens =
    {
        "meshes",
        "main",
        "misc"
    };

    private static readonly string[] TextureArchiveNameTokens =
    {
        "textures",
        "main",
        "misc"
    };

    private static readonly string[] MaterialArchiveNameTokens =
    {
        "materials",
        "main",
        "misc"
    };

    private static readonly string[] DefaultLazyArchiveNameTokens =
    {
        "main",
        "misc"
    };

    private readonly IAssetArchiveIndexRepository AssetArchiveIndexRepository;
    private readonly IReadOnlyList<IAssetArchiveReader> ArchiveReaders;
    private readonly ILogger Logger = Log.ForContext<AssetArchiveIndexService>();
    private readonly IMemoryPressureService? MemoryPressureService;
    private readonly IProcessTerminationDiagnosticsService? ProcessTerminationDiagnosticsService;

    public AssetArchiveIndexService(
        IAssetArchiveIndexRepository assetArchiveIndexRepository,
        IEnumerable<IAssetArchiveReader> archiveReaders,
        IMemoryPressureService? memoryPressureService = null,
        IProcessTerminationDiagnosticsService? processTerminationDiagnosticsService = null)
    {
        AssetArchiveIndexRepository = assetArchiveIndexRepository;
        ArchiveReaders = archiveReaders.ToList();
        MemoryPressureService = memoryPressureService;
        ProcessTerminationDiagnosticsService = processTerminationDiagnosticsService;
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

        var archives = GetArchives(dataFolder)
            .Where(IsImportArchiveCandidate)
            .ToList();
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
            var fileInfo = new FileInfo(archivePath);
            var progressSnapshot = new GameImportProgressDTO
            {
                StatusText = $"Indexing {game} asset archives",
                DetailText = Path.GetFileName(archivePath),
                ProgressValue = index + 1,
                ProgressMaximum = archives.Count,
                IsIndeterminate = false
            };
            progress?.Report(progressSnapshot);
            ProcessTerminationDiagnosticsService?.UpdateHeartbeat($"{game} asset archive indexing", progressSnapshot);
            Logger.Information(
                "Starting asset archive index for {Game}; archive {ArchiveIndex}/{ArchiveCount}; path: {ArchivePath}; size bytes: {SourceFileSizeBytes}; last write UTC: {SourceLastWriteUTC}",
                game,
                index + 1,
                archives.Count,
                archivePath,
                fileInfo.Exists ? fileInfo.Length : 0,
                fileInfo.Exists ? fileInfo.LastWriteTimeUtc : null);

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

            ClearArchiveReaderCache(archiveReader);
        }

        Logger.Information(
            "Completed asset archive indexing for {Game}; discovered: {ArchivesDiscovered}, indexed: {ArchivesIndexed}, current: {ArchivesSkippedCurrent}, failed: {ArchivesFailed}, entries indexed: {EntriesIndexed}",
            game,
            result.ArchivesDiscovered,
            result.ArchivesIndexed,
            result.ArchivesSkippedCurrent,
            result.ArchivesFailed,
            result.EntriesIndexed);
        MemoryPressureService?.CollectAfterBulkImportPhase($"{game} asset archive indexing");
        return result;
    }

    public BethesdaAssetReadResult TryReadArchiveAsset(SupportedGame game, string dataFolder, string assetPath)
    {
        var normalizedPath = NormalizeAssetPath(assetPath);
        var candidateEntryPaths = GetRelativePathCandidates(normalizedPath)
            .Select(NormalizeArchiveEntryPath)
            .ToList();
        var indexedResult = TryReadCurrentIndexedArchiveAsset(game, dataFolder, assetPath, normalizedPath, candidateEntryPaths);
        if (indexedResult != null)
        {
            return indexedResult;
        }

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

            if (readResult.IsTooLarge)
            {
                return new BethesdaAssetReadResult
                {
                    OriginalPath = assetPath,
                    DataFolder = dataFolder,
                    SourceType = BethesdaAssetSourceType.Archive,
                    Status = BethesdaAssetReadStatus.AssetTooLarge,
                    SourceArchivePath = archivePath,
                    NormalizedEntryPath = entry.NormalizedEntryPath,
                    StatusMessage = readResult.StatusMessage ?? $"Archive asset {entry.NormalizedEntryPath} exceeds the preview read limit."
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

    private static void ClearArchiveReaderCache(IAssetArchiveReader archiveReader)
    {
        if (archiveReader is IAssetArchiveCache assetArchiveCache)
        {
            assetArchiveCache.ClearCache();
        }
    }

    private BethesdaAssetReadResult? TryReadCurrentIndexedArchiveAsset(
        SupportedGame game,
        string dataFolder,
        string assetPath,
        string normalizedPath,
        IReadOnlyList<string> candidateEntryPaths)
    {
        var indexedEntries = AssetArchiveIndexRepository.FindEntries(game, dataFolder, candidateEntryPaths)
            .OrderBy(entry => GetArchivePreferenceScore(entry.ArchivePath, normalizedPath))
            .ThenBy(entry => Path.GetFileName(entry.ArchivePath), StringComparer.OrdinalIgnoreCase)
            .ToList();
        foreach (var entry in indexedEntries)
        {
            var archivePath = Path.GetFullPath(entry.ArchivePath);
            var archiveFile = AssetArchiveIndexRepository.GetArchiveFile(game, archivePath);
            if (!IsArchiveIndexCurrent(archiveFile, archivePath))
            {
                continue;
            }

            var archiveReader = ArchiveReaders.FirstOrDefault(reader => reader.CanRead(archivePath));
            if (archiveReader == null)
            {
                continue;
            }

            var readResult = archiveReader.TryReadEntry(archivePath, entry.NormalizedEntryPath);
            if (readResult.IsSuccess && readResult.Data != null)
            {
                Logger.Information(
                    "Asset archive index directly resolved {AssetPath} from indexed entry {EntryPath} in {ArchivePath}",
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

            if (readResult.IsTooLarge)
            {
                return new BethesdaAssetReadResult
                {
                    OriginalPath = assetPath,
                    DataFolder = dataFolder,
                    SourceType = BethesdaAssetSourceType.Archive,
                    Status = BethesdaAssetReadStatus.AssetTooLarge,
                    SourceArchivePath = archivePath,
                    NormalizedEntryPath = entry.NormalizedEntryPath,
                    StatusMessage = readResult.StatusMessage ?? $"Archive asset {entry.NormalizedEntryPath} exceeds the preview read limit."
                };
            }
        }

        return null;
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
            var listStopwatch = Stopwatch.StartNew();
            var entries = archiveReader.ListEntries(archivePath)
                .Select(entry => CreateEntryDTO(game, archivePath, entry))
                .ToList();
            listStopwatch.Stop();
            Logger.Information(
                "Listed asset archive entries for {Game}; archive path: {ArchivePath}; entry count: {EntryCount}; elapsed ms: {ElapsedMilliseconds}",
                game,
                archivePath,
                entries.Count,
                listStopwatch.ElapsedMilliseconds);
            var archiveFile = new AssetArchiveFileDTO
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
            };
            var replaceStopwatch = Stopwatch.StartNew();
            Logger.Information(
                "Replacing asset archive index entries for {Game}; archive path: {ArchivePath}; entry count: {EntryCount}",
                game,
                archivePath,
                entries.Count);
            var entryCount = AssetArchiveIndexRepository.RefreshArchiveIndex(archiveFile, entries);
            replaceStopwatch.Stop();
            Logger.Information(
                "Replaced asset archive index entries for {Game}; archive path: {ArchivePath}; entry count: {EntryCount}; elapsed ms: {ElapsedMilliseconds}",
                game,
                archivePath,
                entryCount,
                replaceStopwatch.ElapsedMilliseconds);
            Logger.Information(
                "Indexed asset archive {ArchivePath} for {Game} with {EntryCount} entries",
                archivePath,
                game,
                entryCount);
            return new AssetArchiveIndexAttemptResult(AssetArchiveIndexStatus.Indexed, entryCount, null);
        }
        catch (Exception exception) when (IsArchiveIndexFailureException(exception))
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

    private static bool IsArchiveIndexFailureException(Exception exception)
    {
        return exception is IOException
            or InvalidDataException
            or UnauthorizedAccessException
            or OverflowException
            or DbException
            or SQLiteException;
    }

    private static bool IsArchiveIndexCurrent(AssetArchiveFileDTO? archiveFile, string archivePath)
    {
        if (archiveFile == null || !File.Exists(archivePath))
        {
            return false;
        }

        var fileInfo = new FileInfo(archivePath);
        return archiveFile.SourceLastWriteUTCTicks == fileInfo.LastWriteTimeUtc.Ticks &&
            archiveFile.SourceFileSizeBytes == fileInfo.Length;
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
        var lazyArchiveNameTokens = GetLazyArchiveNameTokens(normalizedPath);
        return GetArchives(dataFolder)
            .Where(path => IsLazyArchiveCandidate(path, lazyArchiveNameTokens))
            .OrderBy(path => GetArchivePreferenceScore(path, normalizedPath))
            .ThenBy(path => Path.GetFileName(path), StringComparer.OrdinalIgnoreCase);
    }

    private static IEnumerable<string> GetArchives(string dataFolder)
    {
        return Directory.EnumerateFiles(dataFolder)
            .Where(path => ArchiveExtensions.Contains(Path.GetExtension(path), StringComparer.OrdinalIgnoreCase))
            .OrderBy(path => Path.GetFileName(path), StringComparer.OrdinalIgnoreCase);
    }

    private static bool IsImportArchiveCandidate(string archivePath)
    {
        var fileName = Path.GetFileNameWithoutExtension(archivePath);
        return ImportArchiveNameTokens.Any(token => fileName.Contains(token, StringComparison.OrdinalIgnoreCase));
    }

    private static bool IsLazyArchiveCandidate(string archivePath, IReadOnlyList<string> archiveNameTokens)
    {
        var fileName = Path.GetFileNameWithoutExtension(archivePath);
        return archiveNameTokens.Any(token => fileName.Contains(token, StringComparison.OrdinalIgnoreCase));
    }

    private static IReadOnlyList<string> GetLazyArchiveNameTokens(string normalizedPath)
    {
        if (StartsWithAnyRoot(normalizedPath, "Meshes") ||
            StartsWithAnyRoot(normalizedPath, "Geometries"))
        {
            return MeshArchiveNameTokens;
        }

        if (StartsWithAnyRoot(normalizedPath, "Textures"))
        {
            return TextureArchiveNameTokens;
        }

        if (StartsWithAnyRoot(normalizedPath, "Materials"))
        {
            return MaterialArchiveNameTokens;
        }

        return DefaultLazyArchiveNameTokens;
    }

    private static int GetArchivePreferenceScore(string archivePath, string normalizedPath)
    {
        var archiveName = Path.GetFileNameWithoutExtension(archivePath);
        if ((StartsWithAnyRoot(normalizedPath, "Meshes") ||
             StartsWithAnyRoot(normalizedPath, "Geometries")) &&
            archiveName.Contains("meshes", StringComparison.OrdinalIgnoreCase))
        {
            return 0;
        }

        if (StartsWithAnyRoot(normalizedPath, "Textures") &&
            archiveName.Contains("textures", StringComparison.OrdinalIgnoreCase))
        {
            return 0;
        }

        if (StartsWithAnyRoot(normalizedPath, "Materials") &&
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

    private static bool StartsWithAnyRoot(string path, string directoryName)
    {
        return StartsWithDirectory(path, directoryName) ||
            StartsWithDirectory(path, Path.Combine("Data", directoryName));
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
