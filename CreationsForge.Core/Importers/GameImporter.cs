using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Results;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Importers.Interfaces;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services.Interfaces;
using NPoco;
using Serilog;

namespace CreationsForge.Core.Importers;

public class GameImporter : IGameImporter
{
    private readonly IGamePluginReader PluginReader;
    private readonly IGameRecordReader RecordReader;
    private readonly IGameRepository GameRepository;
    private readonly IPluginRepository PluginRepository;
    private readonly IPluginMasterReferenceRepository PluginMasterReferenceRepository;
    private readonly IEnumerable<IPluginExtensionImporter> PluginExtensionImporters;
    private readonly IRecordImportService RecordImportService;
    private readonly IAssetArchiveIndexService AssetArchiveIndexService;
    private readonly IDatabase? Database;
    private readonly ILogger Logger = Log.ForContext<GameImporter>();

    public GameImporter(
        IGamePluginReader pluginReader,
        IGameRecordReader recordReader,
        IGameRepository gameRepository,
        IPluginRepository pluginRepository,
        IPluginMasterReferenceRepository pluginMasterReferenceRepository,
        IEnumerable<IPluginExtensionImporter> pluginExtensionImporters,
        IRecordImportService recordImportService,
        IAssetArchiveIndexService assetArchiveIndexService,
        IDatabase? database = null)
    {
        PluginReader = pluginReader;
        RecordReader = recordReader;
        GameRepository = gameRepository;
        PluginRepository = pluginRepository;
        PluginMasterReferenceRepository = pluginMasterReferenceRepository;
        PluginExtensionImporters = pluginExtensionImporters;
        RecordImportService = recordImportService;
        AssetArchiveIndexService = assetArchiveIndexService;
        Database = database;
    }

    public SupportedGame Game => PluginReader.Game;

    public GameImportResultDTO Import(
        bool forceFullReimport = false,
        IProgress<GameImportProgressDTO>? progress = null,
        CancellationToken cancellationToken = default)
    {
        if (RecordReader.Game != PluginReader.Game) throw new InvalidOperationException($"Record reader game '{RecordReader.Game}' does not match plugin reader game '{PluginReader.Game}'.");

        cancellationToken.ThrowIfCancellationRequested();
        Logger.Information("Starting import for {Game}", Game);
        using var transaction = Database?.GetTransaction();
        var gameDTO = PluginReader.ReadGame();
        gameDTO.ImportedAtUTC = DateTime.UtcNow;
        GameRepository.Save(gameDTO);
        var result = new GameImportResultDTO { Game = Game };
        result.AssetArchiveIndex = AssetArchiveIndexService.IndexGameArchives(
            Game,
            gameDTO.DataFolder,
            progress,
            cancellationToken);

        cancellationToken.ThrowIfCancellationRequested();
        var loadOrderEntries = PluginReader.ReadLoadOrder();
        result.PluginsDiscovered = loadOrderEntries.Count;
        progress?.Report(new GameImportProgressDTO
        {
            StatusText = $"Discovered {loadOrderEntries.Count} {Game} plugins.",
            DetailText = forceFullReimport ? "Running full import." : "Unchanged plugins will be skipped.",
            ProgressValue = 0,
            ProgressMaximum = loadOrderEntries.Count,
            PluginCount = loadOrderEntries.Count,
            IsIndeterminate = loadOrderEntries.Count == 0
        });
        var pluginsForLaterPhases = new List<PluginDTO>();

        for (var index = 0; index < loadOrderEntries.Count; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var loadOrderEntry = loadOrderEntries[index];
            ReportPluginProgress(progress, loadOrderEntry, index + 1, loadOrderEntries.Count, "Checking plugin", forceFullReimport ? "Full import requested." : "Checking source fingerprint.");
            var plugin = ImportPlugin(loadOrderEntry, result, forceFullReimport);
            if (plugin is not null)
            {
                pluginsForLaterPhases.Add(plugin);
            }
        }

        for (var index = 0; index < pluginsForLaterPhases.Count; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var plugin = pluginsForLaterPhases[index];
            ReportPluginProgress(progress, plugin, index + 1, pluginsForLaterPhases.Count, "Importing master references", "Reading declared plugin masters.");
            ImportMasterReferences(plugin, result, cancellationToken);
        }

        for (var index = 0; index < pluginsForLaterPhases.Count; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var plugin = pluginsForLaterPhases[index];
            ReportPluginProgress(progress, plugin, index + 1, pluginsForLaterPhases.Count, "Importing records", "Reading approved shared record types.");
            var recordImportResult = RecordImportService.ImportPluginRecords(plugin, RecordReader, progress, index + 1, pluginsForLaterPhases.Count, cancellationToken);
            result.Records.Add(recordImportResult);
            SavePartialImportStateWhenNeeded(plugin, recordImportResult);
        }

        transaction?.Complete();
        Logger.Information(
            "Completed import for {Game}; discovered: {PluginsDiscovered}, imported: {PluginsImported}, unchanged: {PluginsUnchanged}, changed: {PluginsChanged}, missing: {PluginsMissing}, failed: {PluginsFailed}, unsupported: {PluginsUnsupported}, invalidated: {PluginsInvalidated}, masters: {MasterReferencesImported}, record headers: {RecordHeadersImported}, record details: {RecordDetailRowsImported}, record failures: {RecordFailures}, unsupported record types: {UnsupportedRecordTypes}, form lists: {FormListsImported}, form list items: {FormListItemsImported}, game settings: {GameSettingsImported}, globals: {GlobalsImported}",
            Game,
            result.PluginsDiscovered,
            result.PluginsImported,
            result.PluginsUnchanged,
            result.PluginsChanged,
            result.PluginsMissing,
            result.PluginsFailed,
            result.PluginsUnsupported,
            result.PluginsInvalidated,
            result.MasterReferencesImported,
            result.Records.HeadersImported,
            result.Records.DetailRowsImported,
            result.Records.RecordsFailed,
            result.Records.UnsupportedRecordTypes,
            result.Records.FormListsImported,
            result.Records.FormListItemsImported,
            result.Records.GameSettingsImported,
            result.Records.GlobalsImported);

        return result;
    }

    private static void ReportPluginProgress(
        IProgress<GameImportProgressDTO>? progress,
        PluginLoadOrderEntryDTO loadOrderEntry,
        int pluginIndex,
        int pluginCount,
        string statusPrefix,
        string detailText)
    {
        progress?.Report(new GameImportProgressDTO
        {
            StatusText = $"{statusPrefix}: {loadOrderEntry.ModKey.FileName}",
            DetailText = detailText,
            ProgressValue = pluginIndex,
            ProgressMaximum = pluginCount,
            CurrentPluginName = loadOrderEntry.ModKey.FileName,
            PluginIndex = pluginIndex,
            PluginCount = pluginCount,
            IsIndeterminate = false
        });
    }

    private static void ReportPluginProgress(
        IProgress<GameImportProgressDTO>? progress,
        PluginDTO plugin,
        int pluginIndex,
        int pluginCount,
        string statusPrefix,
        string detailText)
    {
        progress?.Report(new GameImportProgressDTO
        {
            StatusText = $"{statusPrefix}: {plugin.ModKey.FileName}",
            DetailText = detailText,
            ProgressValue = pluginIndex,
            ProgressMaximum = pluginCount,
            CurrentPluginName = plugin.ModKey.FileName,
            PluginIndex = pluginIndex,
            PluginCount = pluginCount,
            IsIndeterminate = false
        });
    }

    private PluginDTO? ImportPlugin(PluginLoadOrderEntryDTO loadOrderEntry, GameImportResultDTO result, bool forceFullReimport)
    {
        var existingPlugin = PluginRepository.GetByModKey(Game, loadOrderEntry.ModKey);
        var sourceInfo = PluginReader.ReadSourceInfo(loadOrderEntry.ModKey);

        if (PluginReader.IsUnsupported(loadOrderEntry))
        {
            result.PluginsUnsupported++;
            Logger.Information("Skipping unsupported plugin {ModKey} for {Game}", loadOrderEntry.ModKey.FileName, Game);
            PluginRepository.Save(CreatePluginState(loadOrderEntry, existingPlugin, sourceInfo, PluginImportState.Unsupported));
            return null;
        }

        if (!sourceInfo.Exists)
        {
            result.PluginsMissing++;
            PluginRepository.Save(CreatePluginState(loadOrderEntry, existingPlugin, sourceInfo, PluginImportState.Missing));
            return null;
        }

        var isUnchanged = existingPlugin is not null
            && existingPlugin.SourceLastWriteUTCTicks == sourceInfo.LastWriteUTCTicks
            && existingPlugin.SourceFileSizeBytes == sourceInfo.FileSizeBytes;

        if (isUnchanged && !forceFullReimport)
        {
            result.PluginsUnchanged++;
            existingPlugin!.LoadOrderIndex = loadOrderEntry.LoadOrderIndex;
            existingPlugin.Enabled = loadOrderEntry.Enabled;
            existingPlugin.ExistsOnDisk = sourceInfo.Exists;
            existingPlugin.LastCheckedUTC = DateTime.UtcNow;
            existingPlugin.SourceLastWriteUTCTicks = sourceInfo.LastWriteUTCTicks;
            existingPlugin.SourceFileSizeBytes = sourceInfo.FileSizeBytes;
            PluginRepository.Save(existingPlugin);
            Logger.Information("Skipping unchanged plugin {ModKey} for {Game}", loadOrderEntry.ModKey.FileName, Game);
            return null;
        }

        if (existingPlugin is not null)
        {
            result.PluginsInvalidated++;
            if (!isUnchanged)
            {
                result.PluginsChanged++;
                Logger.Information(
                    "Plugin {ModKey} changed for {Game}: stored last write ticks {StoredLastWriteUtcTicks}, current last write ticks {CurrentLastWriteUtcTicks}, stored file size {StoredFileSizeBytes}, current file size {CurrentFileSizeBytes}",
                    loadOrderEntry.ModKey.FileName,
                    Game,
                    existingPlugin.SourceLastWriteUTCTicks,
                    sourceInfo.LastWriteUTCTicks,
                    existingPlugin.SourceFileSizeBytes,
                    sourceInfo.FileSizeBytes);
            }
            else
            {
                Logger.Information("Forcing reimport of unchanged plugin {ModKey} for {Game}", loadOrderEntry.ModKey.FileName, Game);
            }
        }

        try
        {
            var plugin = PluginReader.ReadPluginMetadata(loadOrderEntry, sourceInfo);
            plugin.ImportState = PluginImportState.Current;
            plugin.LastCheckedUTC = DateTime.UtcNow;
            plugin.LastImportedUTC = DateTime.UtcNow;
            plugin.InvalidatedAtUTC = null;

            PluginRepository.Save(plugin);

            foreach (var pluginExtensionImporter in PluginExtensionImporters.Where(importer => importer.CanImport(plugin)))
            {
                pluginExtensionImporter.Import(plugin);
            }

            result.PluginsImported++;
            return plugin;
        }
        catch (Exception ex)
        {
            result.PluginsFailed++;
            Logger.Error(ex, "Unable to import plugin metadata for {ModKey} for {Game}", loadOrderEntry.ModKey.FileName, Game);
            PluginRepository.Save(CreatePluginState(loadOrderEntry, existingPlugin, sourceInfo, PluginImportState.Failed));
            return null;
        }
    }

    private void ImportMasterReferences(PluginDTO plugin, GameImportResultDTO result, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var importedAtUTC = DateTime.UtcNow;
        var masterReferences = PluginReader.ReadMasterReferences(plugin);
        foreach (var masterReference in masterReferences)
        {
            cancellationToken.ThrowIfCancellationRequested();
            masterReference.ImportedAtUTC = importedAtUTC;
            var masterPlugin = PluginRepository.GetByModKey(Game, masterReference.MasterModKey);
            if (masterPlugin is null)
            {
                Logger.Warning("Skipping master reference from {PluginModKey} to missing master {MasterModKey} for {Game}", plugin.ModKey.FileName, masterReference.MasterModKey.FileName, Game);
                continue;
            }

            masterReference.MasterModKey = masterPlugin.ModKey;
            masterReference.PluginModKey = plugin.ModKey;
            PluginMasterReferenceRepository.Save(masterReference);
            result.MasterReferencesImported++;
        }

        PluginMasterReferenceRepository.DeleteStaleByPlugin(Game, plugin.ModKey, importedAtUTC);
    }

    private void SavePartialImportStateWhenNeeded(PluginDTO plugin, RecordImportResultDTO recordImportResult)
    {
        if (recordImportResult.RecordsFailed == 0)
        {
            return;
        }

        plugin.ImportState = PluginImportState.PartiallyImported;
        plugin.InvalidatedAtUTC = DateTime.UtcNow;
        PluginRepository.Save(plugin);
        Logger.Warning(
            "Plugin {ModKey} for {Game} was partially imported with {RecordFailures} record failures",
            plugin.ModKey.FileName,
            Game,
            recordImportResult.RecordsFailed);
    }

    private PluginDTO CreatePluginState(
        PluginLoadOrderEntryDTO loadOrderEntry,
        PluginDTO? existingPlugin,
        PluginSourceInfoDTO sourceInfo,
        PluginImportState importState)
    {
        var plugin = existingPlugin ?? new PluginDTO
        {
            Game = Game,
            ModKey = loadOrderEntry.ModKey,
            LoadOrderIndex = loadOrderEntry.LoadOrderIndex,
            Enabled = loadOrderEntry.Enabled,
            ExistsOnDisk = sourceInfo.Exists,
            ImportState = importState,
            HeaderFlags = 0,
            FormVersion = 0,
            RecordCount = 0,
            SourceLastWriteUTCTicks = sourceInfo.LastWriteUTCTicks,
            SourceFileSizeBytes = sourceInfo.FileSizeBytes,
            LastCheckedUTC = DateTime.UtcNow
        };

        plugin.LoadOrderIndex = loadOrderEntry.LoadOrderIndex;
        plugin.Enabled = loadOrderEntry.Enabled;
        plugin.ExistsOnDisk = sourceInfo.Exists;
        plugin.ImportState = importState;
        plugin.LastCheckedUTC = DateTime.UtcNow;
        plugin.SourceLastWriteUTCTicks = sourceInfo.LastWriteUTCTicks;
        plugin.SourceFileSizeBytes = sourceInfo.FileSizeBytes;
        if (importState is PluginImportState.Failed or PluginImportState.Changed or PluginImportState.PartiallyImported)
        {
            plugin.InvalidatedAtUTC = DateTime.UtcNow;
        }

        return plugin;
    }
}
