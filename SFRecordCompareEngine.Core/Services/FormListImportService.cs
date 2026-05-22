using System.Collections;
using System.Globalization;
using System.IO;
using System.Reflection;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;
using Serilog;
using SFRecordCompareEngine.Core.Configuration.Interfaces;
using SFRecordCompareEngine.Core.Database.Interfaces;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine.Core.Services;

public class FormListImportService(
    IDatabaseSchemaInitializer databaseSchemaInitializer,
    ISqliteConnectionFactory connectionFactory,
    IGameConfigurationStore gameConfigurationStore,
    IPluginRepository pluginRepository,
    IRecordHeaderRepository recordHeaderRepository,
    IFormListRepository formListRepository) : IFormListImportService
{
    private const string RecordType = "FormList";
    private readonly ILogger Logger = Log.ForContext<FormListImportService>();

    public Task<FormListImportResultDTO> ImportForPluginHierarchyAsync(string selectedModKey, CancellationToken cancellationToken)
    {
        return Task.Run(() => ImportForPluginHierarchy(selectedModKey, cancellationToken), cancellationToken);
    }

    public IList<FormListRecordDTO> GetFormListsForPlugin(string modKey)
    {
        using var database = connectionFactory.OpenDatabase();
        return formListRepository.GetByModKey(database, modKey);
    }

    public FormListRecordDTO? GetFormList(string modKey, string formId)
    {
        using var database = connectionFactory.OpenDatabase();
        return formListRepository.GetByModKeyAndFormId(database, modKey, FormIdNormalizer.NormalizeFromFormKey(formId));
    }

    public IList<FormListRecordDTO> GetFormListsForHierarchy(string selectedModKey)
    {
        using var database = connectionFactory.OpenDatabase();
        return formListRepository.GetByHierarchy(database, selectedModKey);
    }

    public IList<FormListRecordDTO> GetMatchingFormListsForHierarchy(string selectedModKey, string formId)
    {
        using var database = connectionFactory.OpenDatabase();
        return formListRepository.GetByHierarchyAndFormId(database, selectedModKey, FormIdNormalizer.NormalizeFromFormKey(formId));
    }

    public IList<FormListRecordDTO> SearchFormListsByEditorId(string selectedModKey, string searchText)
    {
        using var database = connectionFactory.OpenDatabase();
        return formListRepository.SearchByEditorId(database, selectedModKey, searchText);
    }

    private FormListImportResultDTO ImportForPluginHierarchy(string selectedModKey, CancellationToken cancellationToken)
    {
        databaseSchemaInitializer.Initialize();

        var result = new FormListImportResultDTO
        {
            SelectedModKey = selectedModKey
        };

        using var database = connectionFactory.OpenDatabase();
        var hierarchy = pluginRepository.GetResolutionHierarchy(database, selectedModKey);
        result.HierarchyModKeys = hierarchy.Select(plugin => plugin.HierarchyModKey).ToList();

        using var transaction = database.GetTransaction();
        foreach (var hierarchyPlugin in hierarchy)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var plugin = pluginRepository.GetByModKey(database, hierarchyPlugin.HierarchyModKey);
            if (plugin is null)
            {
                result.SkippedModKeys.Add(hierarchyPlugin.HierarchyModKey);
                continue;
            }

            if (!string.Equals(plugin.ImportState, PluginImportState.Current.ToString(), StringComparison.Ordinal))
            {
                recordHeaderRepository.DeleteByModKeyAndRecordType(database, plugin.ModKey, RecordType);
                result.PluginsInvalidated++;
                result.SkippedModKeys.Add(plugin.ModKey);
                continue;
            }

            ImportPluginFormLists(database, plugin, result, cancellationToken);
        }

        transaction.Complete();

        Logger.Information(
            "FormList import completed for {SelectedModKey}: hierarchy {HierarchyModKeys}, skipped {SkippedModKeys}, plugins imported {PluginsImported}, invalidated {PluginsInvalidated}, FormLists imported {FormListsImported}, items imported {FormListItemsImported}, failed {FormListsFailed}",
            selectedModKey,
            result.HierarchyModKeys,
            result.SkippedModKeys,
            result.PluginsImported,
            result.PluginsInvalidated,
            result.FormListsImported,
            result.FormListItemsImported,
            result.FormListsFailed);

        return result;
    }

    private void ImportPluginFormLists(
        NPoco.IDatabase database,
        PluginMetadataDTO plugin,
        FormListImportResultDTO result,
        CancellationToken cancellationToken)
    {
        recordHeaderRepository.DeleteByModKeyAndRecordType(database, plugin.ModKey, RecordType);
        result.PluginsInvalidated++;

        if (string.IsNullOrWhiteSpace(plugin.PluginPath))
        {
            result.SkippedModKeys.Add(plugin.ModKey);
            Logger.Warning("Skipping FormList import for {ModKey} because PluginPath is empty", plugin.ModKey);
            return;
        }

        try
        {
            using var mod = LoadPlugin(plugin);
            var records = GetFormListRecords(mod);
            var importedAtUtc = FormatUtc(DateTimeOffset.UtcNow);
            var importedFormLists = 0;
            var importedItems = 0;

            foreach (var record in records)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var formKey = GetStringValue(record, "FormKey")
                              ?? throw new InvalidOperationException($"FormList record in {plugin.ModKey} did not expose FormKey.");
                formKey = FormKeyTextNormalizer.NormalizeReferenceValue(formKey);
                var formId = FormIdNormalizer.NormalizeFromFormKey(formKey);

                recordHeaderRepository.Upsert(database, new RecordHeaderDTO
                {
                    ModKey = plugin.ModKey,
                    FormID = formId,
                    RecordType = RecordType,
                    FormKey = formKey,
                    EditorID = GetStringValue(record, "EditorID"),
                    PluginFileName = plugin.PluginFileName,
                    FormVersion = GetNullableIntValue(record, "FormVersion"),
                    StarfieldMajorRecordFlags = GetNullableIntValue(record, "StarfieldMajorRecordFlags"),
                    Version2 = GetNullableIntValue(record, "Version2"),
                    VersionControl = GetStringValue(record, "VersionControl"),
                    ImportedAtUtc = importedAtUtc
                });

                formListRepository.UpsertFormList(database, new FormListDTO
                {
                    ModKey = plugin.ModKey,
                    FormID = formId,
                    AddToListFormKey = ExtractReferenceText(GetPropertyValue(record, "AddToList")),
                    ImportedAtUtc = importedAtUtc
                });

                var items = GetItems(record, plugin.ModKey, formId, importedAtUtc);
                formListRepository.ReplaceItems(database, plugin.ModKey, formId, items);

                importedFormLists++;
                importedItems += items.Count;
            }

            result.FormListsImported += importedFormLists;
            result.FormListItemsImported += importedItems;
            result.PluginsImported++;

            Logger.Information(
                "Imported {FormListCount} FormList records and {FormListItemCount} items for {ModKey}",
                importedFormLists,
                importedItems,
                plugin.ModKey);
        }
        catch (Exception ex)
        {
            result.FormListsFailed++;
            result.SkippedModKeys.Add(plugin.ModKey);
            plugin.ImportState = PluginImportState.Failed.ToString();
            pluginRepository.UpsertPlugin(database, plugin);
            recordHeaderRepository.DeleteByModKeyAndRecordType(database, plugin.ModKey, RecordType);
            Logger.Error(ex, "Unable to import FormList records for {ModKey} from {PluginPath}", plugin.ModKey, plugin.PluginPath);
        }
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

    private static IList<object> GetFormListRecords(IStarfieldModGetter plugin)
    {
        var records = GetRecordsFromMutagenTypeOption(plugin) ?? GetRecordsFromPluginProperty(plugin);
        return records?.Cast<object>().ToList() ?? new List<object>();
    }

    private static IEnumerable? GetRecordsFromMutagenTypeOption(IStarfieldModGetter plugin)
    {
        var method = typeof(TypeOptionSolidifierMixIns)
            .GetMethods()
            .Where(method => method.Name.Equals(RecordType, StringComparison.Ordinal))
            .FirstOrDefault(method =>
            {
                var parameters = method.GetParameters();
                return parameters.Length == 1 && parameters[0].ParameterType.IsAssignableFrom(typeof(IEnumerable<IStarfieldModGetter>));
            });

        return method?.Invoke(null, [new[] { plugin }]) as IEnumerable;
    }

    private static IEnumerable? GetRecordsFromPluginProperty(IStarfieldModGetter plugin)
    {
        foreach (var propertyName in new[] { "FormList", "FormLists" })
        {
            var property = plugin.GetType().GetProperty(propertyName);
            if (property?.GetValue(plugin) is IEnumerable records) return records;
        }

        return null;
    }

    private static IList<FormListItemDTO> GetItems(object record, string modKey, string formId, string importedAtUtc)
    {
        if (GetPropertyValue(record, "Items") is not IEnumerable items)
        {
            return new List<FormListItemDTO>();
        }

        return items
            .Cast<object?>()
            .Select((item, index) => new FormListItemDTO
            {
                ModKey = modKey,
                FormID = formId,
                ItemIndex = index,
                ItemFormKey = ExtractReferenceText(item) ?? throw new InvalidOperationException($"FormList {modKey}:{formId} item {index} did not expose a FormKey."),
                ImportedAtUtc = importedAtUtc
            })
            .ToList();
    }

    private static object? GetPropertyValue(object source, string propertyName)
    {
        return source.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public)?.GetValue(source);
    }

    private static string? GetStringValue(object source, string propertyName)
    {
        return GetPropertyValue(source, propertyName)?.ToString();
    }

    private static string? ExtractReferenceText(object? value)
    {
        if (value is null) return null;

        var rawValue = value.ToString();
        if (string.IsNullOrWhiteSpace(rawValue)) return null;

        var normalizedValue = FormKeyTextNormalizer.NormalizeReferenceValue(rawValue);
        return normalizedValue.Equals("Null", StringComparison.OrdinalIgnoreCase)
               || normalizedValue.Equals("NullReference", StringComparison.OrdinalIgnoreCase)
            ? null
            : normalizedValue;
    }

    private static int? GetNullableIntValue(object source, string propertyName)
    {
        var value = GetPropertyValue(source, propertyName);
        if (value is null) return null;

        try
        {
            return Convert.ToInt32(value, CultureInfo.InvariantCulture);
        }
        catch
        {
            return null;
        }
    }

    private static string FormatUtc(DateTimeOffset dateTimeOffset)
    {
        return dateTimeOffset.UtcDateTime.ToString("O", CultureInfo.InvariantCulture);
    }
}
