using System.Collections;
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Results;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Importers.Interfaces;
using CreationsForge.Core.Services.Interfaces;
using CreationsForge.Specification.Records;
using Serilog;

namespace CreationsForge.Core.Services;

/// <summary>
/// Coordinates typed record import for a plugin by reading mapped record DTOs and dispatching them to typed detail
/// importers.
/// </summary>
public class RecordImportService : IRecordImportService
{
    private readonly Dictionary<(Enums.SupportedGame Game, string RecordType), ITypedRecordImporter> TypedRecordImporters;
    private readonly ILogger Logger = Log.ForContext<RecordImportService>();
    private readonly IRecordSpecificationProvider RecordSpecificationProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordImportService"/> class.
    /// </summary>
    /// <param name="typedRecordImporters">The typed detail importers available for record import.</param>
    /// <param name="recordSpecificationProvider">The optional record specification provider used for pilot spec-driven import dispatch.</param>
    public RecordImportService(
        IEnumerable<ITypedRecordImporter> typedRecordImporters,
        IRecordSpecificationProvider? recordSpecificationProvider = null)
    {
        TypedRecordImporters = typedRecordImporters
            .SelectMany(importer => importer.SupportedGames.Select(game => new { Game = game, Importer = importer }))
            .ToDictionary(entry => (entry.Game, entry.Importer.RecordType), entry => entry.Importer);
        RecordSpecificationProvider = recordSpecificationProvider ?? new RecordSpecificationProvider();
    }

    /// <summary>
    /// Imports the approved typed records for one plugin using the supplied game-specific record reader.
    /// </summary>
    /// <param name="plugin">The plugin whose mapped records should be imported.</param>
    /// <param name="recordReader">The game-specific reader that maps Mutagen records into Core DTOs.</param>
    /// <param name="progress">The optional progress sink for long-running import status.</param>
    /// <param name="pluginIndex">The one-based plugin index used for progress reporting.</param>
    /// <param name="pluginCount">The total plugin count used for progress reporting.</param>
    /// <param name="cancellationToken">The token used to cancel record discovery and import.</param>
    /// <returns>The aggregate import result for the plugin's typed records.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the plugin game does not match the record reader game.</exception>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
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
        ImportSpecDrivenPilotRecordTypes(plugin, result, recordSet, progress, pluginIndex, pluginCount, cancellationToken);
        ImportPluginRecordType(plugin, result, RecordTypeCatalog.Class, recordSet.Classes, progress, pluginIndex, pluginCount, cancellationToken);
        ImportPluginRecordType(plugin, result, RecordTypeCatalog.Faction, recordSet.Factions, progress, pluginIndex, pluginCount, cancellationToken);
        ImportPluginRecordType(plugin, result, RecordTypeCatalog.MiscItem, recordSet.MiscItems, progress, pluginIndex, pluginCount, cancellationToken);
        ImportPluginRecordType(plugin, result, RecordTypeCatalog.Keyword, recordSet.Keywords, progress, pluginIndex, pluginCount, cancellationToken);
        ImportPluginRecordType(plugin, result, RecordTypeCatalog.ActorValueInformation, recordSet.ActorValueInformation, progress, pluginIndex, pluginCount, cancellationToken);
        ImportPluginRecordType(plugin, result, RecordTypeCatalog.NPC, recordSet.NPCs, progress, pluginIndex, pluginCount, cancellationToken);
        ImportPluginRecordType(plugin, result, RecordTypeCatalog.MagicEffect, recordSet.MagicEffects, progress, pluginIndex, pluginCount, cancellationToken);
        ImportPluginRecordType(plugin, result, RecordTypeCatalog.Perk, recordSet.Perks, progress, pluginIndex, pluginCount, cancellationToken);
        ImportOptionalPluginRecordType(plugin, result, RecordTypeCatalog.Static, recordSet.Statics, progress, pluginIndex, pluginCount, cancellationToken);
        ImportOptionalPluginRecordType(plugin, result, RecordTypeCatalog.Container, recordSet.Containers, progress, pluginIndex, pluginCount, cancellationToken);
        ImportOptionalPluginRecordType(plugin, result, RecordTypeCatalog.ConstructibleObject, recordSet.ConstructibleObjects, progress, pluginIndex, pluginCount, cancellationToken);
        ImportOptionalPluginRecordType(plugin, result, RecordTypeCatalog.ConditionForm, recordSet.ConditionForms, progress, pluginIndex, pluginCount, cancellationToken);
        ImportOptionalPluginRecordType(plugin, result, RecordTypeCatalog.Book, recordSet.Books, progress, pluginIndex, pluginCount, cancellationToken);
        ImportOptionalPluginRecordType(plugin, result, RecordTypeCatalog.Door, recordSet.Doors, progress, pluginIndex, pluginCount, cancellationToken);
        ImportOptionalPluginRecordType(plugin, result, RecordTypeCatalog.Terminal, recordSet.Terminals, progress, pluginIndex, pluginCount, cancellationToken);
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

    /// <summary>
    /// Imports the first record families whose dispatch metadata lives in the production specification catalog.
    /// </summary>
    /// <param name="plugin">The plugin whose records are being imported.</param>
    /// <param name="result">The aggregate import result being populated.</param>
    /// <param name="recordSet">The mapped record set returned by the game reader.</param>
    /// <param name="progress">The optional progress sink for import status.</param>
    /// <param name="pluginIndex">The one-based plugin index used for progress reporting.</param>
    /// <param name="pluginCount">The total plugin count used for progress reporting.</param>
    /// <param name="cancellationToken">The token used to cancel record import.</param>
    private void ImportSpecDrivenPilotRecordTypes(
        PluginDTO plugin,
        RecordImportResultDTO result,
        PluginRecordSetDTO recordSet,
        IProgress<GameImportProgressDTO>? progress,
        int pluginIndex,
        int pluginCount,
        CancellationToken cancellationToken)
    {
        foreach (var specification in RecordSpecificationProvider.GetAll())
        {
            var recordType = new RecordTypeData
            {
                TableName = specification.TableName,
                RecordType = specification.RecordType,
                RecordID = specification.RecordID,
                FriendlyName = specification.FriendlyName
            };
            var records = GetRecordSetRecords(recordSet, specification);
            if (specification.Import.IsRequired)
            {
                ImportPluginRecordType(plugin, result, recordType, records, progress, pluginIndex, pluginCount, cancellationToken);
                continue;
            }

            ImportOptionalPluginRecordType(plugin, result, recordType, records, progress, pluginIndex, pluginCount, cancellationToken);
        }
    }

    /// <summary>
    /// Reads the mapped record DTO list identified by a record specification from a plugin record set.
    /// </summary>
    /// <param name="recordSet">The record set returned by a game-specific record reader.</param>
    /// <param name="specification">The specification that names the record-set collection property.</param>
    /// <returns>The mapped records as objects while preserving the original DTO instances.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the specification points at a missing or non-enumerable record-set property.</exception>
    private static IReadOnlyList<object> GetRecordSetRecords(
        PluginRecordSetDTO recordSet,
        RecordSpecification specification)
    {
        var property = typeof(PluginRecordSetDTO).GetProperty(specification.Import.PluginRecordSetPropertyName);
        if (property == null)
        {
            throw new InvalidOperationException(
                $"Record specification '{specification.RecordID}' references unknown PluginRecordSetDTO property '{specification.Import.PluginRecordSetPropertyName}'.");
        }

        if (property.GetValue(recordSet) is not IEnumerable values)
        {
            throw new InvalidOperationException(
                $"PluginRecordSetDTO property '{specification.Import.PluginRecordSetPropertyName}' for record specification '{specification.RecordID}' is not enumerable.");
        }

        return values.Cast<object>().ToList();
    }

    private void ImportOptionalPluginRecordType<TRecordDTO>(
        PluginDTO plugin,
        RecordImportResultDTO result,
        RecordTypeData recordType,
        IReadOnlyList<TRecordDTO> records,
        IProgress<GameImportProgressDTO>? progress,
        int pluginIndex,
        int pluginCount,
        CancellationToken cancellationToken)
    {
        if (records.Count == 0 && !TypedRecordImporters.ContainsKey((plugin.Game, recordType.RecordID)))
        {
            return;
        }

        ImportPluginRecordType(plugin, result, recordType, records, progress, pluginIndex, pluginCount, cancellationToken);
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
            Game = plugin.Game,
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
