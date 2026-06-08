using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Results;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Importers.Interfaces;
using CreationsForge.Core.Services.Interfaces;
using Serilog;

namespace CreationsForge.Core.Services;

public class RecordImportService : IRecordImportService
{
    private readonly Dictionary<(Enums.SupportedGame Game, string RecordType), ITypedRecordImporter> TypedRecordImporters;
    private readonly ILogger Logger = Log.ForContext<RecordImportService>();

    public RecordImportService(IEnumerable<ITypedRecordImporter> typedRecordImporters)
    {
        TypedRecordImporters = typedRecordImporters
            .SelectMany(importer => importer.SupportedGames.Select(game => new { Game = game, Importer = importer }))
            .ToDictionary(entry => (entry.Game, entry.Importer.RecordType), entry => entry.Importer);
    }

    public RecordImportResultDTO ImportPluginRecords(
        PluginDTO plugin,
        IGameRecordReader recordReader,
        IProgress<GameImportProgressDTO>? progress = null,
        int pluginIndex = 0,
        int pluginCount = 0,
        CancellationToken cancellationToken = default)
    {
        if (plugin.Game != recordReader.Game) throw new InvalidOperationException($"Record reader game '{recordReader.Game}' does not match plugin game '{plugin.Game}'.");

        var result = new RecordImportResultDTO();

        Logger.Information("Starting record import for {ModKey} for {Game}", plugin.ModKey.FileName, plugin.Game);
        ReportProgress(progress, plugin, pluginIndex, pluginCount, string.Empty, 0, 0, $"Starting record import: {plugin.ModKey.FileName}", "Loading plugin records.");
        cancellationToken.ThrowIfCancellationRequested();
        var recordSet = recordReader.ReadPluginRecords(plugin, cancellationToken);
        ImportPluginRecordType(plugin, result, RecordTypeCatalog.FormList, recordSet.FormLists, progress, pluginIndex, pluginCount, cancellationToken);
        ImportPluginRecordType(plugin, result, RecordTypeCatalog.GameSetting, recordSet.GameSettings, progress, pluginIndex, pluginCount, cancellationToken);
        ImportPluginRecordType(plugin, result, RecordTypeCatalog.Global, recordSet.Globals, progress, pluginIndex, pluginCount, cancellationToken);
        ImportPluginRecordType(plugin, result, RecordTypeCatalog.MiscObject, recordSet.MiscObjects, progress, pluginIndex, pluginCount, cancellationToken);
        ImportPluginRecordType(plugin, result, RecordTypeCatalog.Keyword, recordSet.Keywords, progress, pluginIndex, pluginCount, cancellationToken);
        ImportPluginRecordType(plugin, result, RecordTypeCatalog.ActorValueInformation, recordSet.ActorValueInformation, progress, pluginIndex, pluginCount, cancellationToken);
        ImportPluginRecordType(plugin, result, RecordTypeCatalog.NPC, recordSet.NPCs, progress, pluginIndex, pluginCount, cancellationToken);
        ImportPluginRecordType(plugin, result, RecordTypeCatalog.MagicEffect, recordSet.MagicEffects, progress, pluginIndex, pluginCount, cancellationToken);
        ImportPluginRecordType(plugin, result, RecordTypeCatalog.Perk, recordSet.Perks, progress, pluginIndex, pluginCount, cancellationToken);
        Logger.Information(
            "Finished record import for {ModKey} for {Game}: headers {HeadersImported}, details {DetailRowsImported}, FormList items {FormListItemsImported}, record failures {RecordsFailed}, unsupported record types {UnsupportedRecordTypes}",
            plugin.ModKey.FileName,
            plugin.Game,
            result.HeadersImported,
            result.DetailRowsImported,
            result.FormListItemsImported,
            result.RecordsFailed,
            result.UnsupportedRecordTypes);

        return result;
    }

    private void ImportPluginRecordType<TRecordDTO>(
        PluginDTO plugin,
        RecordImportResultDTO result,
        RecordTypeData recordType,
        IReadOnlyList<TRecordDTO> records,
        IProgress<GameImportProgressDTO>? progress,
        int pluginIndex,
        int pluginCount,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Logger.Information("Discovering {RecordType} records for {ModKey} for {Game}", recordType.RecordID, plugin.ModKey.FileName, plugin.Game);
        ReportProgress(progress, plugin, pluginIndex, pluginCount, recordType.RecordID, 0, 0, $"Discovering {recordType.RecordID} records", plugin.ModKey.FileName);
        var recordTypeResult = new RecordTypeImportResultDTO
        {
            RecordType = recordType.RecordID,
            HeaderImportSupported = true,
            TypedDetailImportSupported = true,
            DetailTableName = recordType.TableName,
            HeadersImported = records.Count
        };
        result.RecordTypes.Add(recordTypeResult);

        if (!TypedRecordImporters.TryGetValue((plugin.Game, recordType.RecordID), out var importer))
        {
            recordTypeResult.TypedDetailImportSupported = false;
            recordTypeResult.UnsupportedReason = $"No typed detail importer is registered for {plugin.Game} {recordType.RecordID}.";
            Logger.Warning("Skipping {RecordType} record detail import for {ModKey} for {Game}: no typed detail importer is registered", recordType.RecordID, plugin.ModKey.FileName, plugin.Game);
            ReportProgress(progress, plugin, pluginIndex, pluginCount, recordType.RecordID, 0, records.Count, $"Skipping {recordType.RecordID} records", "No typed detail importer is registered.");
            return;
        }

        var importedAtUTC = DateTime.UtcNow;
        Logger.Information("Discovered {RecordCount} {RecordType} records for {ModKey} for {Game}", records.Count, recordType.RecordID, plugin.ModKey.FileName, plugin.Game);
        ReportProgress(progress, plugin, pluginIndex, pluginCount, recordType.RecordID, 0, records.Count, $"Importing {recordType.RecordID} records", $"{records.Count} records discovered.");
        for (var index = 0; index < records.Count; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var record = records[index];
            var recordIndex = index + 1;
            if (recordIndex == 1 || recordIndex == records.Count || recordIndex % 100 == 0)
            {
                ReportProgress(progress, plugin, pluginIndex, pluginCount, recordType.RecordID, recordIndex, records.Count, $"Importing {recordType.RecordID} records", $"{recordIndex} of {records.Count} records.");
            }

            try
            {
                importer.Import(record!, recordTypeResult, importedAtUTC);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                recordTypeResult.RecordsFailed++;
                Logger.Error(ex, "Failed to import {RecordType} record for {ModKey} for {Game}", recordType.RecordID, plugin.ModKey.FileName, plugin.Game);
            }
        }

        if (recordTypeResult.RecordsFailed == 0)
        {
            importer.DeleteStaleRecords(plugin, importedAtUTC);
        }

        Logger.Information(
            "Finished importing {RecordType} records for {ModKey} for {Game}: headers {HeadersImported}, details {DetailRowsImported}, FormList items {FormListItemsImported}, failures {RecordsFailed}",
            recordType.RecordID,
            plugin.ModKey.FileName,
            plugin.Game,
            recordTypeResult.HeadersImported,
            recordTypeResult.DetailRowsImported,
            recordTypeResult.FormListItemsImported,
            recordTypeResult.RecordsFailed);
        ReportProgress(progress, plugin, pluginIndex, pluginCount, recordType.RecordID, records.Count, records.Count, $"Finished {recordType.RecordID} records", $"{recordTypeResult.DetailRowsImported} details imported; {recordTypeResult.RecordsFailed} failures.");
    }

    private static void ReportProgress(
        IProgress<GameImportProgressDTO>? progress,
        PluginDTO plugin,
        int pluginIndex,
        int pluginCount,
        string recordType,
        int recordIndex,
        int recordCount,
        string statusText,
        string detailText)
    {
        progress?.Report(new GameImportProgressDTO
        {
            StatusText = statusText,
            DetailText = detailText,
            ProgressValue = pluginIndex,
            ProgressMaximum = pluginCount <= 0 ? 1 : pluginCount,
            CurrentPluginName = plugin.ModKey.FileName,
            PluginIndex = pluginIndex,
            PluginCount = pluginCount,
            CurrentRecordType = recordType,
            RecordIndex = recordIndex,
            RecordCount = recordCount,
            IsIndeterminate = pluginCount <= 0
        });
    }
}
