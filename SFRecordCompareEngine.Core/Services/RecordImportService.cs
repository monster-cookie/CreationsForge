using System.IO;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;
using Serilog;
using SFRecordCompareEngine.Core.Configuration.Interfaces;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Importers;
using SFRecordCompareEngine.Core.Importers.Interfaces;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine.Core.Services;

public class RecordImportService(
    IGameConfigurationStore gameConfigurationStore,
    IRecordHeaderRepository recordHeaderRepository,
    IRecordEnumerationService recordEnumerationService,
    IEnumerable<ITypedRecordDetailImporter> typedRecordDetailImporters,
    FormListRecordImporter formListRecordImporter) : IRecordImportService
{
    private readonly ILogger Logger = Log.ForContext<RecordImportService>();
    private readonly Dictionary<string, ITypedRecordDetailImporter> TypedRecordDetailImporters = typedRecordDetailImporters.ToDictionary(importer => importer.RecordType, StringComparer.Ordinal);

    public RecordImportResultDTO ImportPluginRecords(NPoco.IDatabase database, PluginMetadataDTO plugin, string importedAtUtc, CancellationToken cancellationToken)
    {
        var result = new RecordImportResultDTO
        {
            ModKey = plugin.ModKey
        };

        if (string.IsNullOrWhiteSpace(plugin.PluginPath))
        {
            throw new InvalidOperationException($"Plugin {plugin.ModKey} cannot import records because PluginPath is empty.");
        }

        using var mod = LoadPlugin(plugin);
        foreach (var unsupportedRecordType in RecordTypeImportCatalog.UnsupportedRecordTypes)
        {
            result.RecordTypes.Add(new RecordTypeImportResultDTO
            {
                RecordType = unsupportedRecordType,
                HeaderImportSupported = false,
                TypedDetailImportSupported = false,
                DetailTableName = RecordTypeImportCatalog.UsesExistingTypedDetailTable(unsupportedRecordType)
                    ? unsupportedRecordType
                    : null,
                UnsupportedReason = unsupportedRecordType.Equals(RecordTypeImportCatalog.AggregateRecordType, StringComparison.Ordinal)
                    ? "Aggregate/base record type; concrete records are imported through their specific record types."
                    : "No safe enumerable Mutagen API was discovered for this record type."
            });
        }

        foreach (var recordType in RecordTypeImportCatalog.SupportedRecordTypes)
        {
            cancellationToken.ThrowIfCancellationRequested();
            result.RecordTypes.Add(ImportRecordType(database, mod, plugin, recordType, importedAtUtc, cancellationToken));
        }

        Logger.Information(
            "Imported records for {ModKey}: headers {HeadersImported}, typed detail rows {DetailRowsImported}, FormList items {FormListItemsImported}, record failures {RecordsFailed}, unsupported types {UnsupportedRecordTypes}",
            plugin.ModKey,
            result.HeadersImported,
            result.DetailRowsImported,
            result.FormListItemsImported,
            result.RecordsFailed,
            result.UnsupportedRecordTypes);

        return result;
    }

    private RecordTypeImportResultDTO ImportRecordType(
        NPoco.IDatabase database,
        IStarfieldModGetter mod,
        PluginMetadataDTO plugin,
        string recordType,
        string importedAtUtc,
        CancellationToken cancellationToken)
    {
        var result = new RecordTypeImportResultDTO
        {
            RecordType = recordType,
            HeaderImportSupported = true,
            TypedDetailImportSupported = true,
            DetailTableName = RecordTypeImportCatalog.UsesExistingTypedDetailTable(recordType)
                ? recordType
                : TypedRecordDetailImporters.TryGetValue(recordType, out var detailImporter)
                    ? detailImporter.TableName
                    : null
        };
        result.TypedDetailImportSupported = result.DetailTableName is not null;

        var records = recordEnumerationService.GetRecords(mod, recordType);
        if (records is null)
        {
            result.HeaderImportSupported = false;
            result.TypedDetailImportSupported = false;
            result.UnsupportedReason = "Mutagen did not expose an enumerable collection for this record type on the loaded plugin.";
            return result;
        }

        foreach (var record in records)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                var header = RecordHeaderMapper.Map(plugin, recordType, record.Record, importedAtUtc);
                recordHeaderRepository.Upsert(database, header);

                if (RecordTypeImportCatalog.UsesExistingTypedDetailTable(recordType))
                {
                    result.FormListItemsImported += formListRecordImporter.Import(database, plugin.ModKey, header.FormID, record.Record, importedAtUtc);
                    result.DetailRowsImported++;
                }
                else if (TypedRecordDetailImporters.TryGetValue(recordType, out var typedRecordDetailImporter))
                {
                    typedRecordDetailImporter.Import(database, plugin.ModKey, header.FormID, record, importedAtUtc);
                    result.DetailRowsImported++;
                }

                result.HeadersImported++;
            }
            catch (Exception ex)
            {
                result.RecordsFailed++;
                Logger.Error(
                    ex,
                    "Unable to import {RecordType} record for {ModKey} with FormKey {FormKey}",
                    recordType,
                    plugin.ModKey,
                    RecordHeaderMapper.GetFormKeyValue(record.Record));
            }
        }

        return result;
    }

    private IStarfieldModDisposableGetter LoadPlugin(PluginMetadataDTO plugin)
    {
        var gameEnvironment = gameConfigurationStore.Game
                              ?? throw new InvalidOperationException("No game environment is configured.");
        var modKey = ModKey.FromFileName(Path.GetFileName(plugin.PluginPath!));
        var modPath = new ModPath(modKey, plugin.PluginPath!);

        return StarfieldMod.Create(StarfieldRelease.Starfield)
            .FromPath(modPath)
            .WithLoadOrderFromHeaderMasters()
            .WithDataFolder(gameEnvironment.DataFolderPath.Path)
            .Construct();
    }

}
