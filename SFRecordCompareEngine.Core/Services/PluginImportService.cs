using System.Globalization;
using System.IO;
using Mutagen.Bethesda.Plugins;
using Serilog;
using SFRecordCompareEngine.Core.Database.Interfaces;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine.Core.Services;

public class PluginImportService(
    IDatabaseSchemaInitializer databaseSchemaInitializer,
    ISqliteConnectionFactory connectionFactory,
    IPluginRepository pluginRepository,
    IPluginService pluginService,
    IRecordHeaderRepository recordHeaderRepository,
    IRecordImportService recordImportService) : IPluginImportService
{
    private readonly ILogger Logger = Log.ForContext<PluginImportService>();

    public Task<PluginImportResultDTO> InitializeAndImportAsync(CancellationToken cancellationToken)
    {
        return InitializeAndImportAsync(null, cancellationToken);
    }

    public Task<PluginImportResultDTO> InitializeAndImportAsync(IProgress<PluginImportProgressDTO>? progress, CancellationToken cancellationToken)
    {
        return Task.Run(() => InitializeAndImport(progress, cancellationToken), cancellationToken);
    }

    private PluginImportResultDTO InitializeAndImport(IProgress<PluginImportProgressDTO>? progress, CancellationToken cancellationToken)
    {
        progress?.Report(new PluginImportProgressDTO
        {
            StatusText = "Initializing plugin database schema...",
            IsIndeterminate = true
        });

        databaseSchemaInitializer.Initialize();
        Logger.Information("Initialized plugin database {DatabasePath}", connectionFactory.DatabasePath);

        var result = new PluginImportResultDTO();

        var loadOrderEntries = pluginService.GetLoadOrder();
        result.PluginsDiscovered = loadOrderEntries.Count;
        progress?.Report(new PluginImportProgressDTO
        {
            PluginCount = loadOrderEntries.Count,
            StatusText = $"Discovered {loadOrderEntries.Count} plugins.",
            IsIndeterminate = loadOrderEntries.Count == 0
        });

        using var database = connectionFactory.OpenDatabase();
        using var transaction = database.GetTransaction();

        var checkedAtUtc = FormatUtc(DateTimeOffset.UtcNow);
        var currentModKeys = loadOrderEntries
            .Select(entry => entry.ModKey)
            .ToHashSet();

        for (var index = 0; index < loadOrderEntries.Count; index++)
        {
            var loadOrderEntry = loadOrderEntries[index];
            cancellationToken.ThrowIfCancellationRequested();
            progress?.Report(new PluginImportProgressDTO
            {
                CurrentPluginName = loadOrderEntry.PluginFileName,
                CurrentModKey = loadOrderEntry.ModKey.ToString(),
                PluginIndex = index + 1,
                PluginCount = loadOrderEntries.Count,
                StatusText = $"Checking {loadOrderEntry.PluginFileName} ({index + 1} of {loadOrderEntries.Count})...",
                IsIndeterminate = false
            });
            ImportPlugin(database, loadOrderEntry, checkedAtUtc, result, progress, index + 1, loadOrderEntries.Count, cancellationToken);
        }

        DeleteDerivedRowsForMissingPlugins(database, currentModKeys);
        pluginRepository.MarkPluginsNotInLoadOrder(database, currentModKeys, checkedAtUtc);
        pluginRepository.RefreshParentLoadOrderIndexes(database);

        transaction.Complete();

        progress?.Report(new PluginImportProgressDTO
        {
            PluginIndex = loadOrderEntries.Count,
            PluginCount = loadOrderEntries.Count,
            StatusText = $"Plugin database import completed. Imported {result.PluginsImported} plugins.",
            IsIndeterminate = false
        });

        Logger.Information(
            "Plugin import completed: discovered {PluginsDiscovered}, unchanged {PluginsUnchanged}, changed {PluginsChanged}, invalidated {PluginsInvalidated}, imported {PluginsImported}, missing {PluginsMissing}, failed {PluginsFailed}, unsupported {PluginsUnsupported}, master references {MasterReferencesImported}, record headers {RecordHeadersImported}, typed detail rows {TypedRecordDetailRowsImported}, FormList items {FormListItemsImported}, record failures {RecordImportFailures}, unsupported record types {UnsupportedRecordTypes}",
            result.PluginsDiscovered,
            result.PluginsUnchanged,
            result.PluginsChanged,
            result.PluginsInvalidated,
            result.PluginsImported,
            result.PluginsMissing,
            result.PluginsFailed,
            result.PluginsUnsupported,
            result.MasterReferencesImported,
            result.RecordHeadersImported,
            result.TypedRecordDetailRowsImported,
            result.FormListItemsImported,
            result.RecordImportFailures,
            result.UnsupportedRecordTypes);

        return result;
    }

    private void ImportPlugin(
        NPoco.IDatabase database,
        PluginLoadOrderEntryDTO loadOrderEntry,
        string checkedAtUtc,
        PluginImportResultDTO result,
        IProgress<PluginImportProgressDTO>? progress,
        int pluginIndex,
        int pluginCount,
        CancellationToken cancellationToken)
    {
        var existingPlugin = pluginRepository.GetByModKey(database, loadOrderEntry.ModKey);
        var fileInfo = new FileInfo(loadOrderEntry.PluginPath);

        if (IsUnsupportedPlugin(loadOrderEntry))
        {
            result.PluginsUnsupported++;
            recordHeaderRepository.DeleteByModKey(database, loadOrderEntry.ModKey);
            Logger.Information("Skipping unsupported Starfield plugin {ModKey} from {PluginPath}", loadOrderEntry.ModKey, loadOrderEntry.PluginPath);

            pluginRepository.UpsertPlugin(database, new PluginMetadataDTO
            {
                ModKey = loadOrderEntry.ModKey,
                GameRelease = "Starfield",
                LoadOrderIndex = loadOrderEntry.LoadOrderIndex,
                PluginFileName = loadOrderEntry.PluginFileName,
                PluginPath = loadOrderEntry.PluginPath,
                Enabled = loadOrderEntry.Enabled,
                ExistsOnDisk = fileInfo.Exists,
                ImportState = nameof(PluginImportState.Unsupported),
                HeaderFlags = existingPlugin?.HeaderFlags,
                FormVersion = existingPlugin?.FormVersion,
                Author = existingPlugin?.Author,
                Branch = existingPlugin?.Branch,
                InteriorCellCount = existingPlugin?.InteriorCellCount,
                SourceLastWriteUtcTicks = fileInfo.Exists ? fileInfo.LastWriteTimeUtc.Ticks : existingPlugin?.SourceLastWriteUtcTicks,
                SourceFileSizeBytes = fileInfo.Exists ? fileInfo.Length : existingPlugin?.SourceFileSizeBytes,
                LastCheckedUtc = checkedAtUtc,
                LastImportedUtc = existingPlugin?.LastImportedUtc,
                InvalidatedAtUtc = existingPlugin?.InvalidatedAtUtc
            });
            return;
        }

        if (!fileInfo.Exists)
        {
            result.PluginsMissing++;
            recordHeaderRepository.DeleteByModKey(database, loadOrderEntry.ModKey);
            pluginRepository.UpsertPlugin(database, new PluginMetadataDTO
            {
                ModKey = loadOrderEntry.ModKey,
                GameRelease = "Starfield",
                LoadOrderIndex = loadOrderEntry.LoadOrderIndex,
                PluginFileName = loadOrderEntry.PluginFileName,
                PluginPath = loadOrderEntry.PluginPath,
                Enabled = loadOrderEntry.Enabled,
                ExistsOnDisk = false,
                ImportState = nameof(PluginImportState.Missing),
                LastCheckedUtc = checkedAtUtc,
                LastImportedUtc = existingPlugin?.LastImportedUtc,
                SourceLastWriteUtcTicks = existingPlugin?.SourceLastWriteUtcTicks,
                SourceFileSizeBytes = existingPlugin?.SourceFileSizeBytes
            });
            return;
        }

        var sourceLastWriteUtcTicks = fileInfo.LastWriteTimeUtc.Ticks;
        var sourceFileSizeBytes = fileInfo.Length;
        var isUnchanged = existingPlugin is not null
                          && existingPlugin.SourceLastWriteUtcTicks == sourceLastWriteUtcTicks
                          && existingPlugin.SourceFileSizeBytes == sourceFileSizeBytes;

        if (isUnchanged)
        {
            result.PluginsUnchanged++;
            Logger.Information("Skipping unchanged plugin {ModKey}: source last write ticks {SourceLastWriteUtcTicks}, source file size {SourceFileSizeBytes}, import state {ImportState}", loadOrderEntry.ModKey, sourceLastWriteUtcTicks, sourceFileSizeBytes, existingPlugin!.ImportState);
            pluginRepository.UpsertPlugin(database, CopyWithLoadOrderRefresh(existingPlugin, loadOrderEntry, checkedAtUtc, sourceLastWriteUtcTicks, sourceFileSizeBytes));
            return;
        }

        if (existingPlugin is not null)
        {
            result.PluginsChanged++;
            result.PluginsInvalidated++;
            Logger.Information(
                "Plugin {ModKey} changed: stored last write ticks {StoredLastWriteUtcTicks}, current last write ticks {CurrentLastWriteUtcTicks}, stored file size {StoredFileSizeBytes}, current file size {CurrentFileSizeBytes}, stored import state {ImportState}",
                loadOrderEntry.ModKey,
                existingPlugin.SourceLastWriteUtcTicks,
                sourceLastWriteUtcTicks,
                existingPlugin.SourceFileSizeBytes,
                sourceFileSizeBytes,
                existingPlugin.ImportState);
        }

        var invalidatedAtUtc = existingPlugin is null ? null : checkedAtUtc;
        try
        {
            progress?.Report(new PluginImportProgressDTO
            {
                CurrentPluginName = loadOrderEntry.PluginFileName,
                CurrentModKey = loadOrderEntry.ModKey,
                PluginIndex = pluginIndex,
                PluginCount = pluginCount,
                StatusText = $"Importing changed plugin {loadOrderEntry.PluginFileName} ({pluginIndex} of {pluginCount})...",
                IsIndeterminate = false
            });

            var importedAtUtc = FormatUtc(DateTimeOffset.UtcNow);
            var header = pluginService.ReadHeader(loadOrderEntry.PluginPath);
            var plugin = new PluginMetadataDTO
            {
                ModKey = loadOrderEntry.ModKey,
                GameRelease = "Starfield",
                LoadOrderIndex = loadOrderEntry.LoadOrderIndex,
                PluginFileName = loadOrderEntry.PluginFileName,
                PluginPath = loadOrderEntry.PluginPath,
                Enabled = loadOrderEntry.Enabled,
                ExistsOnDisk = true,
                ImportState = nameof(PluginImportState.Current),
                HeaderFlags = header.HeaderFlags,
                FormVersion = header.FormVersion,
                Author = header.Author,
                Branch = header.Branch,
                InteriorCellCount = header.InteriorCellCount,
                SourceLastWriteUtcTicks = sourceLastWriteUtcTicks,
                SourceFileSizeBytes = sourceFileSizeBytes,
                LastCheckedUtc = checkedAtUtc,
                LastImportedUtc = importedAtUtc,
                InvalidatedAtUtc = invalidatedAtUtc
            };

            pluginRepository.UpsertPlugin(database, plugin);

            var masterReferences = BuildMasterReferences(database, header, importedAtUtc);
            pluginRepository.ReplaceMasterReferences(database, loadOrderEntry.ModKey, masterReferences);
            recordHeaderRepository.DeleteByModKey(database, loadOrderEntry.ModKey);
            var recordImportResult = recordImportService.ImportPluginRecords(database, plugin, importedAtUtc, cancellationToken);

            result.MasterReferencesImported += masterReferences.Count;
            result.RecordHeadersImported += recordImportResult.HeadersImported;
            result.TypedRecordDetailRowsImported += recordImportResult.DetailRowsImported;
            result.FormListItemsImported += recordImportResult.FormListItemsImported;
            result.RecordImportFailures += recordImportResult.RecordsFailed;
            result.UnsupportedRecordTypes = Math.Max(result.UnsupportedRecordTypes, recordImportResult.UnsupportedRecordTypes);
            if (recordImportResult.RecordsFailed > 0)
            {
                throw new InvalidOperationException($"Record import failed for {recordImportResult.RecordsFailed} records in {plugin.ModKey}.");
            }

            result.PluginsImported++;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            result.PluginsFailed++;
            Logger.Error(ex, "Unable to import plugin metadata for {ModKey} from {PluginPath}", loadOrderEntry.ModKey, loadOrderEntry.PluginPath);
            recordHeaderRepository.DeleteByModKey(database, loadOrderEntry.ModKey);

            pluginRepository.UpsertPlugin(database, new PluginMetadataDTO
            {
                ModKey = loadOrderEntry.ModKey,
                GameRelease = "Starfield",
                LoadOrderIndex = loadOrderEntry.LoadOrderIndex,
                PluginFileName = loadOrderEntry.PluginFileName,
                PluginPath = loadOrderEntry.PluginPath,
                Enabled = loadOrderEntry.Enabled,
                ExistsOnDisk = true,
                ImportState = nameof(PluginImportState.Failed),
                HeaderFlags = existingPlugin?.HeaderFlags,
                FormVersion = existingPlugin?.FormVersion,
                Author = existingPlugin?.Author,
                Branch = existingPlugin?.Branch,
                InteriorCellCount = existingPlugin?.InteriorCellCount,
                SourceLastWriteUtcTicks = sourceLastWriteUtcTicks,
                SourceFileSizeBytes = sourceFileSizeBytes,
                LastCheckedUtc = checkedAtUtc,
                LastImportedUtc = existingPlugin?.LastImportedUtc,
                InvalidatedAtUtc = invalidatedAtUtc
            });
        }
    }

    private IList<PluginMasterReferenceDTO> BuildMasterReferences(
        NPoco.IDatabase database,
        PluginHeaderMetadataDTO header,
        string importedAtUtc)
    {
        var masterReferences = new List<PluginMasterReferenceDTO>();
        for (var index = 0; index < header.MasterModKeys.Count; index++)
        {
            var parentModKey = header.MasterModKeys[index];
            var parentPlugin = pluginRepository.GetByModKey(database, parentModKey);
            if (parentPlugin is null)
            {
                Logger.Warning("Plugin {ModKey} references missing parent plugin {ParentModKey}", header.ModKey, parentModKey);
                pluginRepository.UpsertMissingPlaceholder(database, parentModKey, importedAtUtc);
                parentPlugin = pluginRepository.GetByModKey(database, parentModKey);
            }

            masterReferences.Add(new PluginMasterReferenceDTO
            {
                ModKey = header.ModKey,
                ParentModKey = parentModKey,
                MasterReferenceIndex = index,
                ParentLoadOrderIndex = parentPlugin?.LoadOrderIndex,
                ImportedAtUtc = importedAtUtc
            });
        }

        return masterReferences;
    }

    private void DeleteDerivedRowsForMissingPlugins(NPoco.IDatabase database, ISet<ModKey> currentModKeys)
    {
        var plugins = pluginRepository.GetAll(database);
        var currentModKeysByCaseInsensitiveKey = currentModKeys.ToHashSet();
        foreach (var plugin in plugins.Where(plugin => !currentModKeysByCaseInsensitiveKey.Contains(plugin.ModKey)))
        {
            recordHeaderRepository.DeleteByModKey(database, plugin.ModKey);
        }
    }

    private static PluginMetadataDTO CopyWithLoadOrderRefresh(
        PluginMetadataDTO existingPlugin,
        PluginLoadOrderEntryDTO loadOrderEntry,
        string checkedAtUtc,
        long sourceLastWriteUtcTicks,
        long sourceFileSizeBytes)
    {
        existingPlugin.LoadOrderIndex = loadOrderEntry.LoadOrderIndex;
        existingPlugin.PluginFileName = loadOrderEntry.PluginFileName;
        existingPlugin.PluginPath = loadOrderEntry.PluginPath;
        existingPlugin.Enabled = loadOrderEntry.Enabled;
        existingPlugin.ExistsOnDisk = true;
        if (!string.Equals(existingPlugin.ImportState, nameof(PluginImportState.Failed), StringComparison.OrdinalIgnoreCase))
        {
            existingPlugin.ImportState = nameof(PluginImportState.Current);
        }
        existingPlugin.SourceLastWriteUtcTicks = sourceLastWriteUtcTicks;
        existingPlugin.SourceFileSizeBytes = sourceFileSizeBytes;
        existingPlugin.LastCheckedUtc = checkedAtUtc;
        return existingPlugin;
    }

    private static bool IsUnsupportedPlugin(PluginLoadOrderEntryDTO loadOrderEntry)
    {
        var pluginFileName = string.IsNullOrWhiteSpace(loadOrderEntry.PluginFileName) ? loadOrderEntry.ModKey.ToString() : loadOrderEntry.PluginFileName;
        return pluginFileName.StartsWith("BlueprintShips", StringComparison.OrdinalIgnoreCase) && pluginFileName.EndsWith(".esm", StringComparison.OrdinalIgnoreCase);
    }

    private static string FormatUtc(DateTimeOffset dateTimeOffset)
    {
        return dateTimeOffset.UtcDateTime.ToString("O", CultureInfo.InvariantCulture);
    }
}
