using System.Globalization;
using System.IO;
using Serilog;
using SFRecordCompareEngine.Core.Database;
using SFRecordCompareEngine.Core.Database.Interfaces;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine.Core.Services;

public class PluginImportService(
    IDatabaseSchemaInitializer databaseSchemaInitializer,
    ISqliteConnectionFactory connectionFactory,
    IPluginRepository pluginRepository,
    IPluginService pluginService) : IPluginImportService
{
    private readonly ILogger Logger = Log.ForContext<PluginImportService>();

    public Task<PluginImportResultDTO> InitializeAndImportAsync(CancellationToken cancellationToken)
    {
        return Task.Run(() => InitializeAndImport(cancellationToken), cancellationToken);
    }

    private PluginImportResultDTO InitializeAndImport(CancellationToken cancellationToken)
    {
        var schemaVersion = databaseSchemaInitializer.Initialize();
        Logger.Information(
            "Initialized plugin database {DatabasePath} at schema version {SchemaVersion}",
            connectionFactory.DatabasePath,
            schemaVersion);

        var result = new PluginImportResultDTO
        {
            SchemaVersion = schemaVersion
        };

        var loadOrderEntries = pluginService.GetLoadOrder();
        result.PluginsDiscovered = loadOrderEntries.Count;

        using var database = connectionFactory.OpenDatabase();
        using var transaction = database.GetTransaction();

        var checkedAtUtc = FormatUtc(DateTimeOffset.UtcNow);
        var currentModKeys = loadOrderEntries
            .Select(entry => entry.ModKey)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var loadOrderEntry in loadOrderEntries)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ImportPlugin(database, loadOrderEntry, checkedAtUtc, result);
        }

        pluginRepository.MarkPluginsNotInLoadOrder(database, currentModKeys, checkedAtUtc);
        pluginRepository.RefreshParentLoadOrderIndexes(database);

        transaction.Complete();

        Logger.Information(
            "Plugin import completed: discovered {PluginsDiscovered}, unchanged {PluginsUnchanged}, changed {PluginsChanged}, invalidated {PluginsInvalidated}, imported {PluginsImported}, missing {PluginsMissing}, failed {PluginsFailed}, master references {MasterReferencesImported}",
            result.PluginsDiscovered,
            result.PluginsUnchanged,
            result.PluginsChanged,
            result.PluginsInvalidated,
            result.PluginsImported,
            result.PluginsMissing,
            result.PluginsFailed,
            result.MasterReferencesImported);

        return result;
    }

    private void ImportPlugin(
        NPoco.IDatabase database,
        PluginLoadOrderEntryDTO loadOrderEntry,
        string checkedAtUtc,
        PluginImportResultDTO result)
    {
        var existingPlugin = pluginRepository.GetByModKey(database, loadOrderEntry.ModKey);
        var fileInfo = new FileInfo(loadOrderEntry.PluginPath);

        if (!fileInfo.Exists)
        {
            result.PluginsMissing++;
            pluginRepository.UpsertPlugin(database, new PluginMetadataDTO
            {
                ModKey = loadOrderEntry.ModKey,
                GameRelease = "Starfield",
                LoadOrderIndex = loadOrderEntry.LoadOrderIndex,
                PluginFileName = loadOrderEntry.PluginFileName,
                PluginPath = loadOrderEntry.PluginPath,
                Enabled = loadOrderEntry.Enabled,
                ExistsOnDisk = false,
                ImportState = PluginImportState.Missing.ToString(),
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
            pluginRepository.UpsertPlugin(database, CopyWithLoadOrderRefresh(
                existingPlugin!,
                loadOrderEntry,
                checkedAtUtc,
                sourceLastWriteUtcTicks,
                sourceFileSizeBytes));
            return;
        }

        if (existingPlugin is not null)
        {
            result.PluginsChanged++;
            result.PluginsInvalidated++;
        }

        var invalidatedAtUtc = existingPlugin is null ? null : checkedAtUtc;
        try
        {
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
                ImportState = PluginImportState.Current.ToString(),
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

            result.MasterReferencesImported += masterReferences.Count;
            result.PluginsImported++;
        }
        catch (Exception ex)
        {
            result.PluginsFailed++;
            Logger.Error(ex, "Unable to import plugin metadata for {ModKey} from {PluginPath}", loadOrderEntry.ModKey, loadOrderEntry.PluginPath);

            pluginRepository.UpsertPlugin(database, new PluginMetadataDTO
            {
                ModKey = loadOrderEntry.ModKey,
                GameRelease = "Starfield",
                LoadOrderIndex = loadOrderEntry.LoadOrderIndex,
                PluginFileName = loadOrderEntry.PluginFileName,
                PluginPath = loadOrderEntry.PluginPath,
                Enabled = loadOrderEntry.Enabled,
                ExistsOnDisk = true,
                ImportState = PluginImportState.Failed.ToString(),
                HeaderFlags = existingPlugin?.HeaderFlags,
                FormVersion = existingPlugin?.FormVersion,
                Author = existingPlugin?.Author,
                Branch = existingPlugin?.Branch,
                InteriorCellCount = existingPlugin?.InteriorCellCount,
                SourceLastWriteUtcTicks = existingPlugin?.SourceLastWriteUtcTicks,
                SourceFileSizeBytes = existingPlugin?.SourceFileSizeBytes,
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
        existingPlugin.ImportState = PluginImportState.Current.ToString();
        existingPlugin.SourceLastWriteUtcTicks = sourceLastWriteUtcTicks;
        existingPlugin.SourceFileSizeBytes = sourceFileSizeBytes;
        existingPlugin.LastCheckedUtc = checkedAtUtc;
        return existingPlugin;
    }

    private static string FormatUtc(DateTimeOffset dateTimeOffset)
    {
        return dateTimeOffset.UtcDateTime.ToString("O", CultureInfo.InvariantCulture);
    }
}
