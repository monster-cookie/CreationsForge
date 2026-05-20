using System.Collections;
using System.IO;
using System.IO.Compression;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records.Mapping;
using Mutagen.Bethesda.Starfield;
using Serilog;
using SFRecordCompareEngine.Core.Configuration.Interfaces;
using SFRecordCompareEngine.Core.DTOs.Cache;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine.Core.Services;

public class CacheService(IGameConfigurationStore gameConfigurationStore) : ICacheService
{
    private const int CacheVersion = 1;
    private const string CacheMagic = "SFRCACHE";
    private readonly ILogger Logger = Log.ForContext<CacheService>();
    private readonly Lock SyncRoot = new();
    private readonly string CacheFilePath = GetDefaultCacheFilePath();
    private PersistedReferenceCacheDTO Cache = new() { CacheVersion = CacheVersion };
    private IDictionary<string, RecordReferenceCacheEntryDTO> LookupByReference = new Dictionary<string, RecordReferenceCacheEntryDTO>(StringComparer.OrdinalIgnoreCase);
    private bool IsDirty;

    public CacheService(IGameConfigurationStore gameConfigurationStore, string cacheFilePath) : this(gameConfigurationStore)
    {
        CacheFilePath = cacheFilePath;
    }

    /// <inheritdoc />
    public void LoadFromDisk()
    {
        lock (SyncRoot)
        {
            if (!File.Exists(CacheFilePath)) return;

            try
            {
                using var fileStream = File.OpenRead(CacheFilePath);
                using var gzipStream = new GZipStream(fileStream, CompressionMode.Decompress);
                using var reader = new BinaryReader(gzipStream);

                var magic = reader.ReadString();
                var version = reader.ReadInt32();
                if (!magic.Equals(CacheMagic, StringComparison.Ordinal) || version != CacheVersion)
                {
                    Logger.Warning("Ignoring unsupported cache file {CacheFilePath}", CacheFilePath);
                    Cache = new PersistedReferenceCacheDTO { CacheVersion = CacheVersion };
                    LookupByReference.Clear();
                    return;
                }

                Cache = ReadCache(reader, version);
                RebuildLookup();
                IsDirty = false;
                Logger.Information("Loaded reference cache from {CacheFilePath} with {PluginCount} plugin cache entries", CacheFilePath, Cache.Plugins.Count);
            }
            catch (Exception ex)
            {
                Logger.Warning(ex, "Unable to load reference cache from {CacheFilePath}", CacheFilePath);
                Cache = new PersistedReferenceCacheDTO { CacheVersion = CacheVersion };
                LookupByReference.Clear();
            }
        }
    }

    /// <inheritdoc />
    public void SaveToDisk()
    {
        lock (SyncRoot)
        {
            if (!IsDirty) return;

            try
            {
                var cacheDirectory = Path.GetDirectoryName(CacheFilePath);
                if (!string.IsNullOrWhiteSpace(cacheDirectory))
                {
                    Directory.CreateDirectory(cacheDirectory);
                }

                var tempFilePath = $"{CacheFilePath}.tmp";

                using (var fileStream = File.Create(tempFilePath))
                using (var gzipStream = new GZipStream(fileStream, CompressionLevel.Optimal))
                using (var writer = new BinaryWriter(gzipStream))
                {
                    writer.Write(CacheMagic);
                    writer.Write(CacheVersion);
                    WriteCache(writer, Cache);
                }

                File.Move(tempFilePath, CacheFilePath, true);
                IsDirty = false;
                Logger.Information("Saved reference cache to {CacheFilePath} with {PluginCount} plugin cache entries", CacheFilePath, Cache.Plugins.Count);
            }
            catch (Exception ex)
            {
                Logger.Warning(ex, "Unable to save reference cache to {CacheFilePath}", CacheFilePath);
            }
        }
    }

    /// <inheritdoc />
    public async Task BuildOrUpdateReferenceCacheAsync(IList<string> pluginNames, IProgress<CacheBuildProgressDTO>? progress, CancellationToken cancellationToken)
    {
        await Task.Run(() => BuildOrUpdateReferenceCache(pluginNames, progress, cancellationToken), cancellationToken);
    }

    /// <inheritdoc />
    public string? ResolveReferenceDisplayValue(string referenceValue)
    {
        if (string.IsNullOrWhiteSpace(referenceValue)) return null;

        lock (SyncRoot)
        {
            foreach (var lookupKey in GetReferenceLookupKeys(referenceValue))
            {
                if (LookupByReference.TryGetValue(lookupKey, out var entry))
                {
                    return GetEntryDisplayValue(entry);
                }
            }
        }

        return null;
    }

    /// <inheritdoc />
    public void Clear()
    {
        lock (SyncRoot)
        {
            Cache = new PersistedReferenceCacheDTO { CacheVersion = CacheVersion };
            LookupByReference.Clear();
            IsDirty = true;
        }
    }

    /// <summary>
    ///     Validate the requested plugin cache entries and rebuild stale or missing entries.
    /// </summary>
    /// <param name="pluginNames">The plugin file names to cache.</param>
    /// <param name="progress">Optional progress reporter for cache UI status.</param>
    /// <param name="cancellationToken">The token used to cancel cache processing.</param>
    private void BuildOrUpdateReferenceCache(IList<string> pluginNames, IProgress<CacheBuildProgressDTO>? progress, CancellationToken cancellationToken)
    {
        var gameEnvironment = gameConfigurationStore.Game;
        if (gameEnvironment is null)
        {
            Logger.Warning("Unable to build reference cache because no game environment is configured");
            return;
        }

        var selectedGame = gameConfigurationStore.SelectedGame;
        lock (SyncRoot)
        {
            if (!string.Equals(Cache.Game, selectedGame, StringComparison.OrdinalIgnoreCase))
            {
                Cache = new PersistedReferenceCacheDTO
                {
                    CacheVersion = CacheVersion,
                    Game = selectedGame
                };
                LookupByReference.Clear();
                IsDirty = true;
            }
        }

        var distinctPluginNames = pluginNames
            .Where(pluginName => !string.IsNullOrWhiteSpace(pluginName))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        Logger.Information("Building reference cache for {PluginCount} plugins", distinctPluginNames.Count);
        var processedPlugins = 0;

        foreach (var pluginName in distinctPluginNames)
        {
            cancellationToken.ThrowIfCancellationRequested();
            processedPlugins++;
            progress?.Report(new CacheBuildProgressDTO
            {
                CurrentPluginName = pluginName,
                ProcessedPlugins = processedPlugins - 1,
                TotalPlugins = distinctPluginNames.Count,
                Message = $"Checking cache: {pluginName}"
            });

            var pluginPath = Path.Combine(gameEnvironment.DataFolderPath.Path, pluginName);
            var pluginFileInfo = new FileInfo(pluginPath);
            if (!pluginFileInfo.Exists)
            {
                Logger.Warning("Unable to cache {PluginName} because {PluginPath} does not exist", pluginName, pluginPath);
                continue;
            }

            PersistedPluginCacheDTO? existingPluginCache;
            lock (SyncRoot)
            {
                existingPluginCache = Cache.Plugins.FirstOrDefault(plugin =>
                    plugin.PluginName.Equals(pluginName, StringComparison.OrdinalIgnoreCase));
            }

            if (existingPluginCache is not null && IsPluginCacheValid(existingPluginCache, pluginFileInfo))
            {
                progress?.Report(new CacheBuildProgressDTO
                {
                    CurrentPluginName = pluginName,
                    ProcessedPlugins = processedPlugins,
                    TotalPlugins = distinctPluginNames.Count,
                    Message = $"Using cache: {pluginName}"
                });
                continue;
            }

            progress?.Report(new CacheBuildProgressDTO
            {
                CurrentPluginName = pluginName,
                ProcessedPlugins = processedPlugins - 1,
                TotalPlugins = distinctPluginNames.Count,
                Message = $"Building cache: {pluginName}"
            });

            var pluginCache = BuildPluginCache(pluginName, pluginFileInfo);
            lock (SyncRoot)
            {
                if (existingPluginCache is not null)
                {
                    Cache.Plugins.Remove(existingPluginCache);
                }

                Cache.Plugins.Add(pluginCache);
                RebuildLookup();
                IsDirty = true;
            }

            progress?.Report(new CacheBuildProgressDTO
            {
                CurrentPluginName = pluginName,
                ProcessedPlugins = processedPlugins,
                TotalPlugins = distinctPluginNames.Count,
                Message = $"Cached {pluginName}"
            });
        }

        Logger.Information(
            "Reference cache build completed for {PluginCount} plugins with {EntryCount} entries",
            distinctPluginNames.Count,
            Cache.Plugins.Sum(plugin => plugin.Entries.Count));
    }

    /// <summary>
    ///     Build the cache payload for one plugin by scanning its major records and capturing reference display fields.
    /// </summary>
    /// <param name="pluginName">The plugin file name to scan.</param>
    /// <param name="pluginFileInfo">File metadata used for future invalidation checks.</param>
    /// <returns>The cache entry for the specified plugin.</returns>
    private PersistedPluginCacheDTO BuildPluginCache(string pluginName, FileInfo pluginFileInfo)
    {
        var pluginCache = new PersistedPluginCacheDTO
        {
            PluginName = pluginName,
            FilePath = pluginFileInfo.FullName,
            FileSize = pluginFileInfo.Length,
            LastWriteTimeUtc = pluginFileInfo.LastWriteTimeUtc
        };

        try
        {
            using var plugin = LoadPlugin(pluginName);
            foreach (var recordType in GetRecordTypes())
            {
                AddRecordTypeCacheEntries(pluginCache, plugin, recordType);
            }
        }
        catch (Exception ex)
        {
            Logger.Warning(ex, "Unable to build reference cache for {PluginName}", pluginName);
        }

        return pluginCache;
    }

    /// <summary>
    ///     Add cache entries for one record type, allowing other record types to continue if enumeration fails.
    /// </summary>
    /// <param name="pluginCache">The plugin cache entry being populated.</param>
    /// <param name="plugin">The loaded plugin to inspect.</param>
    /// <param name="recordType">The major record type to cache.</param>
    private void AddRecordTypeCacheEntries(
        PersistedPluginCacheDTO pluginCache,
        IStarfieldModGetter plugin,
        string recordType)
    {
        try
        {
            var records = GetRecordsFromMutagenTypeOption(plugin, recordType) ?? GetRecordsFromPluginProperty(plugin, recordType);
            if (records is null) return;

            foreach (var record in records.Cast<object>())
            {
                AddRecordCacheEntry(pluginCache, recordType, record);
            }
        }
        catch (Exception ex)
        {
            Logger.Warning(
                ex,
                "Unable to cache {RecordType} records for {PluginName}",
                recordType,
                pluginCache.PluginName);
        }
    }

    /// <summary>
    ///     Add one record cache entry when the record exposes a FormKey or FormID value.
    /// </summary>
    /// <param name="pluginCache">The plugin cache entry being populated.</param>
    /// <param name="recordType">The major record type being cached.</param>
    /// <param name="record">The record to add.</param>
    private static void AddRecordCacheEntry(
        PersistedPluginCacheDTO pluginCache,
        string recordType,
        object record)
    {
        var formKey = GetStringValue(record, "FormKey") ?? GetStringValue(record, "FormID");
        if (string.IsNullOrWhiteSpace(formKey)) return;

        pluginCache.Entries.Add(new RecordReferenceCacheEntryDTO
        {
            FormKey = NormalizeReferenceValue(formKey),
            PluginName = pluginCache.PluginName,
            RecordType = recordType,
            EditorID = GetStringValue(record, "EditorID"),
            DisplayName = GetStringValue(record, "Name") ?? GetStringValue(record, "FullName")
        });
    }

    /// <summary>
    ///     Load a Starfield plugin from the selected game data folder.
    /// </summary>
    /// <param name="pluginName">The plugin file name to load.</param>
    /// <returns>The disposable Mutagen plugin getter.</returns>
    private IStarfieldModDisposableGetter LoadPlugin(string pluginName)
    {
        var gameEnvironment = gameConfigurationStore.Game
                              ?? throw new InvalidOperationException("No game environment is configured.");
        var pluginPath = Path.Combine(gameEnvironment.DataFolderPath.Path, pluginName);
        var modKey = ModKey.FromFileName(Path.GetFileName(pluginPath));
        var modPath = new ModPath(modKey, pluginPath);

        return StarfieldMod.Create(StarfieldRelease.Starfield)
            .FromPath(modPath)
            .WithLoadOrderFromHeaderMasters()
            .WithDataFolder(gameEnvironment.DataFolderPath.Path)
            .Construct();
    }

    /// <summary>
    ///     Get the major record type names supported for the selected game.
    /// </summary>
    /// <returns>The supported record type names or an empty list when the selected game is unsupported.</returns>
    private IList<string> GetRecordTypes()
    {
        return gameConfigurationStore.SelectedGame switch
        {
            "Starfield" => MajorRecordTypeEnumerator
                .GetMajorRecordTypesFor(GameCategory.Starfield)
                .OrderBy(x => x.ClassType.Name)
                .Select(x => x.ClassType.Name)
                .ToList(),
            _ => new List<string>()
        };
    }

    /// <summary>
    ///     Rebuild the in-memory reference lookup dictionary from persisted plugin cache entries.
    /// </summary>
    private void RebuildLookup()
    {
        LookupByReference = new Dictionary<string, RecordReferenceCacheEntryDTO>(StringComparer.OrdinalIgnoreCase);
        foreach (var entry in Cache.Plugins.SelectMany(plugin => plugin.Entries))
        {
            foreach (var lookupKey in GetReferenceLookupKeys(entry.FormKey))
            {
                LookupByReference.TryAdd(lookupKey, entry);
            }
        }
    }

    /// <summary>
    ///     Determine whether a plugin cache entry still matches the source plugin file metadata.
    /// </summary>
    /// <param name="pluginCache">The cached plugin metadata.</param>
    /// <param name="pluginFileInfo">The current plugin file metadata.</param>
    /// <returns>True when the cache entry can be reused; otherwise false.</returns>
    private static bool IsPluginCacheValid(PersistedPluginCacheDTO pluginCache, FileInfo pluginFileInfo)
    {
        return pluginCache.FilePath.Equals(pluginFileInfo.FullName, StringComparison.OrdinalIgnoreCase)
               && pluginCache.FileSize == pluginFileInfo.Length
               && pluginCache.LastWriteTimeUtc == pluginFileInfo.LastWriteTimeUtc;
    }

    /// <summary>
    ///     Generate all lookup keys that should resolve to the same reference cache entry.
    /// </summary>
    /// <param name="referenceValue">The raw reference string.</param>
    /// <returns>Raw, normalized, compact, 6-wide, and 8-wide FormKey lookup keys.</returns>
    private static IEnumerable<string> GetReferenceLookupKeys(string referenceValue)
    {
        var keys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (string.IsNullOrWhiteSpace(referenceValue)) return keys;

        var trimmedValue = referenceValue.Trim();
        keys.Add(trimmedValue);

        var normalizedValue = NormalizeReferenceValue(trimmedValue);
        if (!string.IsNullOrWhiteSpace(normalizedValue))
        {
            keys.Add(normalizedValue);
            foreach (var formKeyVariant in GetFormKeyVariants(normalizedValue))
            {
                keys.Add(formKeyVariant);
            }
        }

        return keys;
    }

    /// <summary>
    ///     Generate compact, 6-wide, and 8-wide variants for FormKey strings whose ID portion may vary in zero padding.
    /// </summary>
    /// <param name="referenceValue">A normalized FormKey string in ID:PluginName format.</param>
    /// <returns>Equivalent FormKey variants for dictionary lookup.</returns>
    private static IEnumerable<string> GetFormKeyVariants(string referenceValue)
    {
        var separatorIndex = referenceValue.IndexOf(':', StringComparison.Ordinal);
        if (separatorIndex <= 0 || separatorIndex == referenceValue.Length - 1) yield break;

        var idPart = referenceValue[..separatorIndex];
        var modPart = referenceValue[(separatorIndex + 1)..];
        if (!idPart.All(Uri.IsHexDigit)) yield break;

        var compactId = idPart.TrimStart('0');
        if (string.IsNullOrEmpty(compactId)) compactId = "0";

        yield return $"{compactId}:{modPart}";

        yield return $"{idPart.PadLeft(6, '0')}:{modPart}";

        if (idPart.Length < 8)
        {
            yield return $"{idPart.PadLeft(8, '0')}:{modPart}";
        }
    }

    /// <summary>
    ///     Normalize Mutagen reference strings by removing FormID prefixes and trailing type display suffixes.
    /// </summary>
    /// <param name="referenceValue">The raw reference string.</param>
    /// <returns>The normalized reference string.</returns>
    private static string NormalizeReferenceValue(string referenceValue)
    {
        var normalizedReferenceValue = referenceValue.Trim();
        if (normalizedReferenceValue.StartsWith("FormID:", StringComparison.OrdinalIgnoreCase))
        {
            normalizedReferenceValue = normalizedReferenceValue["FormID:".Length..].Trim();
        }

        var mutagenTypeSuffixIndex = normalizedReferenceValue.LastIndexOf('<');
        if (mutagenTypeSuffixIndex > 0 && normalizedReferenceValue.EndsWith(">", StringComparison.Ordinal))
        {
            normalizedReferenceValue = normalizedReferenceValue[..mutagenTypeSuffixIndex].Trim();
        }

        return normalizedReferenceValue;
    }

    /// <summary>
    ///     Choose the best user-facing display value from a cache entry.
    /// </summary>
    /// <param name="entry">The cache entry to display.</param>
    /// <returns>The EditorID, display name, or null when neither is available.</returns>
    private static string? GetEntryDisplayValue(RecordReferenceCacheEntryDTO entry)
    {
        return !string.IsNullOrWhiteSpace(entry.EditorID)
            ? entry.EditorID
            : !string.IsNullOrWhiteSpace(entry.DisplayName)
                ? entry.DisplayName
                : null;
    }

    /// <summary>
    ///     Read a public property value from a Mutagen record and convert it to text.
    /// </summary>
    /// <param name="source">The source object containing the property.</param>
    /// <param name="propertyName">The public property name to read.</param>
    /// <returns>The property value as text or null when the property is missing or null.</returns>
    private static string? GetStringValue(object source, string propertyName)
    {
        var value = source.GetType().GetProperty(propertyName)?.GetValue(source);
        return value?.ToString();
    }

    /// <summary>
    ///     Try to enumerate records for a record type using Mutagen's generated type-option helpers.
    /// </summary>
    /// <param name="plugin">The plugin to inspect.</param>
    /// <param name="recordType">The major record type name.</param>
    /// <returns>An enumerable of records, or null when no helper method matches.</returns>
    private static IEnumerable? GetRecordsFromMutagenTypeOption(IStarfieldModGetter plugin, string recordType)
    {
        var method = typeof(TypeOptionSolidifierMixIns)
            .GetMethods()
            .Where(method => method.Name.Equals(recordType, StringComparison.Ordinal))
            .FirstOrDefault(method =>
            {
                var parameters = method.GetParameters();
                return parameters.Length == 1 && parameters[0].ParameterType.IsAssignableFrom(typeof(IEnumerable<IStarfieldModGetter>));
            });

        return method?.Invoke(null, [new[] { plugin }]) as IEnumerable;
    }

    /// <summary>
    ///     Try to enumerate records for a record type from a matching plugin property.
    /// </summary>
    /// <param name="plugin">The plugin to inspect.</param>
    /// <param name="recordType">The major record type name.</param>
    /// <returns>An enumerable of records, or null when no property matches.</returns>
    private static IEnumerable? GetRecordsFromPluginProperty(IStarfieldModGetter plugin, string recordType)
    {
        var propertyNames = new[]
        {
            recordType,
            $"{recordType}s",
            recordType.EndsWith("y", StringComparison.OrdinalIgnoreCase) ? $"{recordType[..^1]}ies" : $"{recordType}s"
        };

        foreach (var propertyName in propertyNames.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            var property = plugin.GetType().GetProperty(propertyName);
            if (property?.GetValue(plugin) is IEnumerable records) return records;
        }

        return null;
    }

    /// <summary>
    ///     Read a versioned persisted reference cache payload from a binary stream.
    /// </summary>
    /// <param name="reader">The binary reader positioned after the cache header.</param>
    /// <param name="version">The validated cache format version.</param>
    /// <returns>The persisted reference cache payload.</returns>
    private static PersistedReferenceCacheDTO ReadCache(BinaryReader reader, int version)
    {
        var cache = new PersistedReferenceCacheDTO
        {
            CacheVersion = version,
            Game = ReadNullableString(reader)
        };

        var pluginCount = reader.ReadInt32();
        for (var pluginIndex = 0; pluginIndex < pluginCount; pluginIndex++)
        {
            var pluginCache = new PersistedPluginCacheDTO
            {
                PluginName = reader.ReadString(),
                FilePath = reader.ReadString(),
                FileSize = reader.ReadInt64(),
                LastWriteTimeUtc = new DateTime(reader.ReadInt64(), DateTimeKind.Utc)
            };

            var entryCount = reader.ReadInt32();
            for (var entryIndex = 0; entryIndex < entryCount; entryIndex++)
            {
                pluginCache.Entries.Add(new RecordReferenceCacheEntryDTO
                {
                    FormKey = reader.ReadString(),
                    PluginName = reader.ReadString(),
                    RecordType = reader.ReadString(),
                    EditorID = ReadNullableString(reader),
                    DisplayName = ReadNullableString(reader)
                });
            }

            cache.Plugins.Add(pluginCache);
        }

        return cache;
    }

    /// <summary>
    ///     Write the persisted reference cache payload to a binary stream.
    /// </summary>
    /// <param name="writer">The binary writer positioned after the cache header.</param>
    /// <param name="cache">The cache payload to write.</param>
    private static void WriteCache(BinaryWriter writer, PersistedReferenceCacheDTO cache)
    {
        WriteNullableString(writer, cache.Game);
        writer.Write(cache.Plugins.Count);

        foreach (var pluginCache in cache.Plugins)
        {
            writer.Write(pluginCache.PluginName);
            writer.Write(pluginCache.FilePath);
            writer.Write(pluginCache.FileSize);
            writer.Write(pluginCache.LastWriteTimeUtc.Ticks);
            writer.Write(pluginCache.Entries.Count);

            foreach (var entry in pluginCache.Entries)
            {
                writer.Write(entry.FormKey);
                writer.Write(entry.PluginName);
                writer.Write(entry.RecordType);
                WriteNullableString(writer, entry.EditorID);
                WriteNullableString(writer, entry.DisplayName);
            }
        }
    }

    /// <summary>
    ///     Read a nullable string written by <see cref="WriteNullableString" />.
    /// </summary>
    /// <param name="reader">The binary reader to read from.</param>
    /// <returns>The string value, or null.</returns>
    private static string? ReadNullableString(BinaryReader reader)
    {
        return reader.ReadBoolean() ? reader.ReadString() : null;
    }

    /// <summary>
    ///     Write a nullable string using a presence flag followed by the value when present.
    /// </summary>
    /// <param name="writer">The binary writer to write to.</param>
    /// <param name="value">The string value to write.</param>
    private static void WriteNullableString(BinaryWriter writer, string? value)
    {
        writer.Write(value is not null);
        if (value is not null)
        {
            writer.Write(value);
        }
    }

    /// <summary>
    ///     Get the default compressed binary reference cache file path for the current user.
    /// </summary>
    /// <returns>The default cache file path.</returns>
    private static string GetDefaultCacheFilePath()
    {
        return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SFRecordCompareEngine", "reference-cache-v1.bin.gz");
    }
}
