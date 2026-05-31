using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Serilog;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.DTOs.Results;
using SFRecordCompareEngine.Core.Helpers;
using SFRecordCompareEngine.Core.Importers.Interfaces;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine.Core.Services;

public class RecordImportService : IRecordImportService
{
    private readonly ILogger Logger = Log.ForContext<RecordImportService>();

    private readonly Dictionary<(GameRelease GameRelease, RecordType RecordType), ITypedRecordDetailImporter> TypedRecordDetailImporters;
    private readonly IStarfieldRecordReaderService StarfieldRecordReaderService;

    public RecordImportService(
        IEnumerable<ITypedRecordDetailImporter> typedRecordDetailImporters,
        IStarfieldRecordReaderService starfieldRecordReaderService)
    {
        TypedRecordDetailImporters = typedRecordDetailImporters.ToDictionary(importer => (importer.GameRelease, importer.RecordType));
        StarfieldRecordReaderService = starfieldRecordReaderService;
    }

    public RecordImportResultDTO ImportPluginRecords(PluginDTO plugin, IProgress<PluginImportProgressDTO>? progress, int pluginIndex, int pluginCount, CancellationToken cancellationToken)
    {
        var importResult = new RecordImportResultDTO
        {
            ModKey = plugin.ModKey
        };

        Logger.Information("Starting record import for {ModKey}", plugin.ModKey);
        ReportProgress(plugin, progress, pluginIndex, pluginCount, null, 0, 0, $"Starting record import for {plugin.ModKey.FileName}...");
        ImportStarfieldPluginRecords(plugin, importResult, progress, pluginIndex, pluginCount, cancellationToken);
        ReportProgress(plugin, progress, pluginIndex, pluginCount, null, 0, 0, $"Finished record import for {plugin.ModKey.FileName}.");
        Logger.Information(
            "Finished record import for {ModKey}: record headers {RecordHeadersImported}, typed detail rows {TypedRecordDetailRowsImported}, FormList items {FormListItemsImported}, record failures {RecordImportFailures}, unsupported record types {UnsupportedRecordTypes}",
            plugin.ModKey,
            importResult.HeadersImported,
            importResult.DetailRowsImported,
            importResult.FormListItemsImported,
            importResult.RecordsFailed,
            importResult.UnsupportedRecordTypes);

        return importResult;
    }

    private void ImportStarfieldPluginRecords(PluginDTO plugin, RecordImportResultDTO resultDTO, IProgress<PluginImportProgressDTO>? progress, int pluginIndex, int pluginCount, CancellationToken cancellationToken)
    {
        ImportStarfieldPluginRecordType(plugin, resultDTO, RecordTypeCatalog.FormList.RecordID, RecordTypeCatalog.FormList.TableName, () => StarfieldRecordReaderService.GetFormLists(plugin), progress, pluginIndex, pluginCount, cancellationToken);
        ImportStarfieldPluginRecordType(plugin, resultDTO, RecordTypeCatalog.GameSetting.RecordID, RecordTypeCatalog.GameSetting.TableName, () => StarfieldRecordReaderService.GetGameSettings(plugin), progress, pluginIndex, pluginCount, cancellationToken);
    }

    private void ImportStarfieldPluginRecordType<TRecordDTO>(PluginDTO plugin, RecordImportResultDTO resultDTO, string recordID, string tableName, Func<IReadOnlyList<TRecordDTO>> getRecords, IProgress<PluginImportProgressDTO>? progress, int pluginIndex, int pluginCount, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Logger.Information("Discovering {RecordType} records for {ModKey}", recordID, plugin.ModKey);
        ReportProgress(plugin, progress, pluginIndex, pluginCount, recordID, 0, 0, $"Discovering {recordID} records for {plugin.ModKey.FileName}...");
        var records = getRecords();

        var recordTypeResult = new RecordTypeImportResultDTO
        {
            RecordType = recordID,
            HeaderImportSupported = true,
            TypedDetailImportSupported = true,
            DetailTableName = tableName,
            HeadersImported = records.Count
        };
        resultDTO.RecordTypes.Add(recordTypeResult);

        var key = (GameRelease.Starfield, new RecordType(recordID));
        if (!TypedRecordDetailImporters.TryGetValue(key, out var importer) || importer == null)
        {
            recordTypeResult.TypedDetailImportSupported = false;
            recordTypeResult.UnsupportedReason = $"No typed detail importer is registered for Starfield {recordID}.";
            Logger.Warning("Skipping {RecordType} record detail import for {ModKey}: no typed detail importer is registered", recordID, plugin.ModKey);
            ReportProgress(plugin, progress, pluginIndex, pluginCount, recordID, 0, records.Count, $"Skipping {recordID} records for {plugin.ModKey.FileName}; no typed detail importer is registered.");
            return;
        }

        Logger.Information("Discovered {RecordCount} {RecordType} records for {ModKey}", records.Count, recordID, plugin.ModKey);
        ReportProgress(plugin, progress, pluginIndex, pluginCount, recordID, 0, records.Count, $"Discovered {records.Count} {recordID} records for {plugin.ModKey.FileName}.");
        if (!records.Any()) return;

        for (var index = 0; index < records.Count; index++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var record = records[index];
            var recordIndex = index + 1;
            if (recordIndex == 1 || recordIndex == records.Count || recordIndex % 100 == 0)
            {
                ReportProgress(plugin, progress, pluginIndex, pluginCount, recordID, recordIndex, records.Count, $"Importing {recordID} records for {plugin.ModKey.FileName}: {recordIndex} of {records.Count}...");
                Logger.Information("Importing {RecordType} records for {ModKey}: {RecordIndex} of {RecordCount}", recordID, plugin.ModKey, recordIndex, records.Count);
            }

            try
            {
                importer.Import(record!, recordTypeResult);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                recordTypeResult.RecordsFailed++;
                Logger.Error(ex, "Failed to import {RecordType} record for {ModKey}", recordID, plugin.ModKey);
            }
        }

        ReportProgress(plugin, progress, pluginIndex, pluginCount, recordID, records.Count, records.Count, $"Finished importing {records.Count} {recordID} records for {plugin.ModKey.FileName}.");
        Logger.Information(
            "Finished importing {RecordType} records for {ModKey}: headers {HeadersImported}, details {DetailRowsImported}, FormList items {FormListItemsImported}, failures {RecordsFailed}",
            recordID,
            plugin.ModKey,
            recordTypeResult.HeadersImported,
            recordTypeResult.DetailRowsImported,
            recordTypeResult.FormListItemsImported,
            recordTypeResult.RecordsFailed);
    }

    private static void ReportProgress(PluginDTO plugin, IProgress<PluginImportProgressDTO>? progress, int pluginIndex, int pluginCount, string? recordType, int recordIndex, int recordCount, string statusText)
    {
        progress?.Report(new PluginImportProgressDTO
        {
            CurrentPluginName = plugin.ModKey.FileName.ToString(),
            CurrentModKey = plugin.ModKey,
            PluginIndex = pluginIndex,
            PluginCount = pluginCount,
            CurrentRecordType = recordType,
            RecordIndex = recordIndex,
            RecordCount = recordCount,
            StatusText = statusText,
            IsIndeterminate = false
        });
    }
}