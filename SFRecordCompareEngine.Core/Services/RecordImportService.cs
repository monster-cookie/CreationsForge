using System.Collections;
using System.IO;
using System.Reflection;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;
using Serilog;
using SFRecordCompareEngine.Core.Configuration.Interfaces;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine.Core.Services;

public class RecordImportService(
    IGameConfigurationStore gameConfigurationStore,
    IRecordHeaderRepository recordHeaderRepository,
    IEnumerable<ITypedRecordDetailImporter> typedRecordDetailImporters,
    FormListRecordImporter formListRecordImporter) : IRecordImportService
{
    private readonly ILogger Logger = Log.ForContext<RecordImportService>();
    private readonly IReadOnlyDictionary<string, ITypedRecordDetailImporter> TypedRecordDetailImporters =
        typedRecordDetailImporters.ToDictionary(importer => importer.RecordType, StringComparer.Ordinal);

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

        var records = GetRecordsFromMutagenTypeOption(mod, recordType) ?? GetRecordsFromPluginProperty(mod, recordType);
        if (records is null)
        {
            result.HeaderImportSupported = false;
            result.TypedDetailImportSupported = false;
            result.UnsupportedReason = "Mutagen did not expose an enumerable collection for this record type on the loaded plugin.";
            return result;
        }

        foreach (var record in records.Cast<object>())
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                var header = RecordHeaderMapper.Map(plugin, recordType, record, importedAtUtc);
                recordHeaderRepository.Upsert(database, header);

                if (RecordTypeImportCatalog.UsesExistingTypedDetailTable(recordType))
                {
                    result.FormListItemsImported += formListRecordImporter.Import(database, plugin.ModKey, header.FormID, record, importedAtUtc);
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
                    RecordHeaderMapper.GetStringValue(record, "FormKey"));
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

    private static IEnumerable? GetRecordsFromPluginProperty(IStarfieldModGetter plugin, string recordType)
    {
        var propertyNames = new[]
        {
            recordType,
            $"{recordType}s",
            recordType.EndsWith("y", StringComparison.OrdinalIgnoreCase)
                ? $"{recordType[..^1]}ies"
                : $"{recordType}s"
        };

        foreach (var propertyName in propertyNames.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            var property = plugin.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
            if (property?.GetValue(plugin) is IEnumerable records) return records;
        }

        return null;
    }
}
