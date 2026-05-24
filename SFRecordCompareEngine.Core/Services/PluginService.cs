using System.Collections;
using System.Globalization;
using System.IO;
using System.Reflection;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records.Mapping;
using Mutagen.Bethesda.Starfield;
using Serilog;
using SFRecordCompareEngine.Core.Database.Interfaces;
using SFRecordCompareEngine.Core.Configuration.Interfaces;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Models.Records;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine.Core.Services;

public class PluginService : IPluginService
{
    private const string BasePluginName = "Starfield.esm";
    private const int MaxFieldDepth = 3;
    private const int MaxCollectionItems = 20;

    private static readonly HashSet<string> ExcludedFieldNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "BinaryOverlay",
        "DebuggerDisplay",
        "EqualsMask",
        "FormVersionSetter",
        "GameRelease",
        "GetterType",
        "Group",
        "MajorRecordFlagsRaw",
        "SetterType",
        "TranslationMask"
    };

    private readonly IGameConfigurationStore GameConfigurationStore;
    private readonly IRecordService RecordService;
    private readonly IRecordEnumerationService RecordEnumerationService;
    private readonly ISqliteConnectionFactory SqliteConnectionFactory;
    private readonly IPluginRepository PluginRepository;
    private readonly IGameSettingRepository GameSettingRepository;

    private readonly ILogger Logger = Log.ForContext<PluginService>();

    public PluginService(
        IGameConfigurationStore gameConfigurationStore,
        IRecordService recordService,
        IRecordEnumerationService recordEnumerationService,
        ISqliteConnectionFactory sqliteConnectionFactory,
        IPluginRepository pluginRepository,
        IGameSettingRepository gameSettingRepository)
    {
        GameConfigurationStore = gameConfigurationStore;
        RecordService = recordService;
        RecordEnumerationService = recordEnumerationService;
        SqliteConnectionFactory = sqliteConnectionFactory;
        PluginRepository = pluginRepository;
        GameSettingRepository = gameSettingRepository;
    }

    public PluginService(IGameConfigurationStore gameConfigurationStore, IRecordService recordService)
        : this(gameConfigurationStore, recordService, new RecordEnumerationService(), null!, null!, null!)
    {
    }

    /// <inheritdoc />
    public IList<string> GetRecordTypes()
    {
        switch (GameConfigurationStore.SelectedGame)
        {
            case "Starfield":
                return MajorRecordTypeEnumerator
                    .GetMajorRecordTypesFor(GameCategory.Starfield)
                    .OrderBy(x => x.ClassType.Name)
                    .Select(x => x.ClassType.Name)
                    .ToList();
            case "Skyrim":
                return MajorRecordTypeEnumerator
                    .GetMajorRecordTypesFor(GameCategory.Skyrim)
                    .OrderBy(x => x.ClassType.Name)
                    .Select(x => x.ClassType.Name)
                    .ToList();
            case "Fallout4":
                return MajorRecordTypeEnumerator
                    .GetMajorRecordTypesFor(GameCategory.Fallout4)
                    .OrderBy(x => x.ClassType.Name)
                    .Select(x => x.ClassType.Name)
                    .ToList();
            default:
                Logger.Error("Game '{Game}' is not supported. Please select Starfield", GameConfigurationStore.SelectedGame);
                return new List<string>();
        }
    }

    /// <inheritdoc />
    public IList<string> GetPlugins()
    {
        try
        {
            using var database = SqliteConnectionFactory.OpenDatabase();
            return PluginRepository.GetPlugins(database)
                .Select(plugin => plugin.PluginFileName)
                .ToList();
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Unable to load plugins from SQLite database");
            return new List<string>();
        }
    }

    public IList<PluginListItemDTO> GetPluginListItems()
    {
        try
        {
            using var database = SqliteConnectionFactory.OpenDatabase();
            return PluginRepository.GetOpenablePlugins(database)
                .Select(ToPluginListItem)
                .ToList();
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Unable to load plugin list items from SQLite database");
            return new List<PluginListItemDTO>();
        }
    }

    /// <inheritdoc />
    public IList<string> SearchPlugins(string searchText)
    {
        try
        {
            using var database = SqliteConnectionFactory.OpenDatabase();
            var plugins = string.IsNullOrWhiteSpace(searchText)
                ? PluginRepository.GetPlugins(database)
                : PluginRepository.SearchPlugins(database, searchText.Trim());

            return plugins
                .Select(plugin => plugin.PluginFileName)
                .ToList();
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Unable to search plugins from SQLite database");
            return new List<string>();
        }
    }

    public IList<PluginListItemDTO> SearchPluginListItems(string searchText)
    {
        try
        {
            using var database = SqliteConnectionFactory.OpenDatabase();
            var plugins = string.IsNullOrWhiteSpace(searchText)
                ? PluginRepository.GetOpenablePlugins(database)
                : PluginRepository.SearchOpenablePlugins(database, searchText.Trim());

            return plugins
                .Select(ToPluginListItem)
                .ToList();
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Unable to search plugin list items from SQLite database");
            return new List<PluginListItemDTO>();
        }
    }

    /// <inheritdoc />
    public IList<PluginLoadOrderEntryDTO> GetLoadOrder()
    {
        var gameEnvironment = GameConfigurationStore.Game
                              ?? throw new InvalidOperationException("No game environment is configured.");

        return gameEnvironment.LoadOrder.ListedOrder
            .Select((plugin, index) =>
            {
                var pluginFileName = plugin.FileName.ToString();
                var modKey = ModKey.FromFileName(pluginFileName).ToString();
                return new PluginLoadOrderEntryDTO
                {
                    ModKey = modKey,
                    PluginFileName = pluginFileName,
                    PluginPath = Path.Combine(gameEnvironment.DataFolderPath.Path, pluginFileName),
                    LoadOrderIndex = index,
                    Enabled = true
                };
            })
            .ToList();
    }

    /// <inheritdoc />
    public PluginHeaderMetadataDTO ReadHeader(string pluginPath)
    {
        var modKey = ModKey.FromFileName(Path.GetFileName(pluginPath));
        var modPath = new ModPath(modKey, pluginPath);

        using var plugin = StarfieldMod.Create(StarfieldRelease.Starfield)
            .FromPath(modPath)
            .WithLoadOrderFromHeaderMasters()
            .WithDataFolder(Path.GetDirectoryName(pluginPath) ?? string.Empty)
            .Construct();

        var header = plugin.ModHeader;
        return new PluginHeaderMetadataDTO
        {
            ModKey = modKey.ToString(),
            Author = header.Author,
            FormVersion = header.Version,
            HeaderFlags = GetNullableIntValue(header, "Flags"),
            Branch = GetStringValue(header, "Branch"),
            InteriorCellCount = GetNullableIntValue(header, "InteriorCellCount"),
            MasterModKeys = header.MasterReferences
                .Select(masterReference => masterReference.Master.FileName.ToString())
                .Where(masterModKey => !string.IsNullOrWhiteSpace(masterModKey))
                .ToList()
        };
    }

    /// <inheritdoc />
    public PluginHeaderDTO? GetPluginHeader(string pluginName)
    {
        try
        {
            using var database = SqliteConnectionFactory.OpenDatabase();
            var plugin = PluginRepository.GetByModKey(database, pluginName);
            if (plugin is null)
            {
                Logger.Warning("Plugin metadata was not found in SQLite database for {PluginName}", pluginName);
                return null;
            }

            var masterReferences = PluginRepository.GetMasterReferences(database, pluginName);
            return new PluginHeaderDTO(plugin, masterReferences);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Unable to load plugin header from SQLite database for {PluginName}", pluginName);
            return null;
        }
    }

    /// <inheritdoc />
    public IList<RecordSummaryDTO> GetRecords(string pluginName, string recordType)
    {
        try
        {
            if (recordType.Equals(RecordTypeImportCatalog.GameSettingRecordType, StringComparison.Ordinal))
            {
                using var database = SqliteConnectionFactory.OpenDatabase();
                return GameSettingRepository.GetSummaries(database, pluginName);
            }

            using var plugin = LoadPlugin(pluginName);
            var records = RecordEnumerationService.GetRawRecords(plugin, recordType);
            if (records is null)
            {
                Logger.Warning("Record type {RecordType} was not found for plugin {PluginName}", recordType, pluginName);
                return new List<RecordSummaryDTO>();
            }

            var filteredRecords = records
                .Cast<object>()
                .Select(record => new RecordSummaryDTO
                {
                    RecordType = recordType,
                    FormID = RecordHeaderMapper.GetFormKeyValue(record) ?? GetStringValue(record, "FormID"),
                    EditorID = GetStringValue(record, "EditorID")
                })
                .ToList();
            return filteredRecords;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Unable to load {RecordType} records for {PluginName}", recordType, pluginName);
            return new List<RecordSummaryDTO>();
        }
    }

    /// <inheritdoc />
    public RecordComparisonDTO GetRecordComparison(string pluginName, string recordType, string formKey)
    {
        var comparison = new RecordComparisonDTO();
        if (string.IsNullOrWhiteSpace(formKey))
        {
            Logger.Warning("Unable to compare {RecordType} record for {PluginName} because the FormKey is empty", recordType, pluginName);
            return comparison;
        }

        try
        {
            if (recordType.Equals(RecordTypeImportCatalog.GameSettingRecordType, StringComparison.Ordinal))
            {
                return GetGameSettingComparison(pluginName, formKey);
            }

            var pluginNames = GetComparisonPluginNames(pluginName);
            var recordsByPlugin = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

            foreach (var comparisonPluginName in pluginNames)
            {
                var record = LoadRecord(comparisonPluginName, recordType, formKey);
                if (record is null && !comparisonPluginName.Equals(BasePluginName, StringComparison.OrdinalIgnoreCase)
                                   && !comparisonPluginName.Equals(pluginName, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                recordsByPlugin[comparisonPluginName] = record;
                comparison.Plugins.Add(new RecordComparisonPluginDTO
                {
                    PluginName = comparisonPluginName,
                    HasRecord = record is not null
                });
            }

            var recordTypeOptions = RecordComparisonRecordTypeOptions.For(recordType);
            var referenceDisplayResolver = new RecordReferenceDisplayResolver(RecordService.ResolveReferenceDisplayValue);
            var fieldsByPlugin = recordsByPlugin
                .Where(item => item.Value is not null)
                .ToDictionary(
                    item => item.Key,
                    item => FlattenRecordFields(item.Value!, recordTypeOptions, referenceDisplayResolver.GetDisplayValue),
                    StringComparer.OrdinalIgnoreCase);

            var fieldNames = fieldsByPlugin
                .SelectMany(item => item.Value.Keys)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(fieldName => fieldName)
                .ToList();

            comparison.Fields = fieldNames
                .Select(fieldName => new RecordComparisonFieldDTO
                {
                    FieldName = fieldName,
                    DisplayKind = GetDisplayKind(fieldName, fieldsByPlugin.Values),
                    ValuesByPlugin = comparison.Plugins.ToDictionary(
                        plugin => plugin.PluginName,
                        plugin => fieldsByPlugin.TryGetValue(plugin.PluginName, out var fields)
                                  && fields.TryGetValue(fieldName, out var value)
                            ? value.TextValue
                            : null,
                        StringComparer.OrdinalIgnoreCase),
                    BooleanValuesByPlugin = comparison.Plugins.ToDictionary(
                        plugin => plugin.PluginName,
                        plugin => fieldsByPlugin.TryGetValue(plugin.PluginName, out var fields)
                                  && fields.TryGetValue(fieldName, out var value)
                            ? value.BooleanValue
                            : null,
                        StringComparer.OrdinalIgnoreCase),
                    TreeValuesByPlugin = comparison.Plugins.ToDictionary(
                        plugin => plugin.PluginName,
                        plugin => fieldsByPlugin.TryGetValue(plugin.PluginName, out var fields)
                                  && fields.TryGetValue(fieldName, out var value)
                                  && value.TreeNodes is not null
                            ? value.TreeNodes
                            : new List<RecordComparisonFieldNodeDTO>(),
                        StringComparer.OrdinalIgnoreCase)
                })
                .ToList();

            Logger.Information(
                "Loaded comparison for {RecordType} {FormKey} in {PluginName} across {PluginCount} plugins",
                recordType,
                formKey,
                pluginName,
                comparison.Plugins.Count);

            return comparison;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Unable to load comparison for {RecordType} {FormKey} in {PluginName}", recordType, formKey, pluginName);
            return new RecordComparisonDTO();
        }
    }

    private RecordComparisonDTO GetGameSettingComparison(string pluginName, string formKey)
    {
        using var database = SqliteConnectionFactory.OpenDatabase();
        var formId = FormIdNormalizer.NormalizeFromFormKey(FormKeyTextNormalizer.NormalizeReferenceValue(formKey));
        var rows = GameSettingRepository.GetByHierarchy(database, pluginName, formId);
        var comparison = new RecordComparisonDTO
        {
            Plugins = rows
                .Select(row => new RecordComparisonPluginDTO
                {
                    PluginName = row.PluginName,
                    HasRecord = row.HasRecord
                })
                .ToList()
        };

        comparison.Fields = BuildGameSettingFields(comparison.Plugins, rows);

        Logger.Information(
            "Loaded GameSetting comparison for {FormKey} in {PluginName} across {PluginCount} plugins from SQLite",
            formKey,
            pluginName,
            comparison.Plugins.Count);

        return comparison;
    }

    private static IList<RecordComparisonFieldDTO> BuildGameSettingFields(
        IList<RecordComparisonPluginDTO> plugins,
        IList<GameSettingComparisonRowDTO> rows)
    {
        var rowByPlugin = rows.ToDictionary(row => row.PluginName, StringComparer.OrdinalIgnoreCase);
        return
        [
            BuildTextField("SettingType", plugins, rowByPlugin, row => row.SettingType),
            BuildTextField("TitleString", plugins, rowByPlugin, row => row.TitleString),
            BuildTextField("Data", plugins, rowByPlugin, row => row.Data),
            BuildTextField("RawData", plugins, rowByPlugin, row => row.RawData?.ToString(CultureInfo.InvariantCulture)),
            BuildTextField("XALG", plugins, rowByPlugin, row => row.XALG?.ToString(CultureInfo.InvariantCulture)),
            BuildBooleanField("IsCompressed", plugins, rowByPlugin, row => ToBoolean(row.IsCompressed)),
            BuildBooleanField("IsDeleted", plugins, rowByPlugin, row => ToBoolean(row.IsDeleted))
        ];
    }

    private static RecordComparisonFieldDTO BuildTextField(
        string fieldName,
        IList<RecordComparisonPluginDTO> plugins,
        IDictionary<string, GameSettingComparisonRowDTO> rowByPlugin,
        Func<GameSettingComparisonRowDTO, string?> getValue)
    {
        return new RecordComparisonFieldDTO
        {
            FieldName = fieldName,
            DisplayKind = RecordComparisonFieldDisplayKind.Text,
            ValuesByPlugin = plugins.ToDictionary(
                plugin => plugin.PluginName,
                plugin => rowByPlugin.TryGetValue(plugin.PluginName, out var row) && row.HasRecord ? getValue(row) : null,
                StringComparer.OrdinalIgnoreCase)
        };
    }

    private static RecordComparisonFieldDTO BuildBooleanField(
        string fieldName,
        IList<RecordComparisonPluginDTO> plugins,
        IDictionary<string, GameSettingComparisonRowDTO> rowByPlugin,
        Func<GameSettingComparisonRowDTO, bool?> getValue)
    {
        return new RecordComparisonFieldDTO
        {
            FieldName = fieldName,
            DisplayKind = RecordComparisonFieldDisplayKind.Boolean,
            ValuesByPlugin = plugins.ToDictionary(
                plugin => plugin.PluginName,
                plugin => rowByPlugin.TryGetValue(plugin.PluginName, out var row) && row.HasRecord ? getValue(row)?.ToString() : null,
                StringComparer.OrdinalIgnoreCase),
            BooleanValuesByPlugin = plugins.ToDictionary(
                plugin => plugin.PluginName,
                plugin => rowByPlugin.TryGetValue(plugin.PluginName, out var row) && row.HasRecord ? getValue(row) : null,
                StringComparer.OrdinalIgnoreCase)
        };
    }

    private static bool? ToBoolean(int? value)
    {
        return value switch
        {
            null => null,
            0 => false,
            _ => true
        };
    }

    /// <summary>
    ///     Load a Starfield plugin from the selected game's data folder.
    /// </summary>
    /// <param name="pluginName">The plugin file name to load.</param>
    /// <returns>The disposable Mutagen plugin getter.</returns>
    private IStarfieldModDisposableGetter LoadPlugin(string pluginName)
    {
        var gameEnvironment = GameConfigurationStore.Game
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

    private static PluginListItemDTO ToPluginListItem(PluginMetadataDTO plugin)
    {
        return new PluginListItemDTO
        {
            PluginFileName = plugin.PluginFileName,
            ImportState = plugin.ImportState
        };
    }

    /// <summary>
    ///     Get the base plugin, master plugins, and selected plugin used as comparison columns.
    /// </summary>
    /// <param name="pluginName">The selected plugin file name.</param>
    /// <returns>The distinct plugin file names to compare.</returns>
    private IList<string> GetComparisonPluginNames(string pluginName)
    {
        try
        {
            using var database = SqliteConnectionFactory.OpenDatabase();
            var hierarchy = PluginRepository.GetResolutionHierarchy(database, pluginName);
            foreach (var hierarchyPlugin in hierarchy.Where(plugin => plugin.HierarchyLoadOrderIndex is null))
            {
                Logger.Error(
                    "Plugin hierarchy for {PluginName} contained null load-order for {HierarchyPlugin}",
                    pluginName,
                    hierarchyPlugin.HierarchyModKey);
            }

            var hierarchyPlugins = hierarchy
                .Select(plugin => plugin.HierarchyModKey)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (hierarchyPlugins.Count > 0)
            {
                return hierarchyPlugins;
            }
        }
        catch (Exception ex)
        {
            Logger.Warning(ex, "Unable to load plugin hierarchy from SQLite database for {PluginName}; falling back to header masters", pluginName);
        }

        using var plugin = LoadPlugin(pluginName);
        var pluginNames = new List<string> { BasePluginName };

        pluginNames.AddRange(plugin.ModHeader.MasterReferences
            .Select(master => master.Master.FileName.ToString())
            .Where(masterName => !string.IsNullOrWhiteSpace(masterName))
            .Where(masterName => !masterName.Equals(BasePluginName, StringComparison.OrdinalIgnoreCase)));

        pluginNames.Add(pluginName);

        return pluginNames
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    /// <summary>
    ///     Load one record from a plugin by record type and FormKey/FormID value.
    /// </summary>
    /// <param name="pluginName">The plugin file name to inspect.</param>
    /// <param name="recordType">The major record type to load.</param>
    /// <param name="formKey">The FormKey or FormID to match.</param>
    /// <returns>The matching record, or null when it cannot be found or loaded.</returns>
    private object? LoadRecord(string pluginName, string recordType, string formKey)
    {
        try
        {
            using var plugin = LoadPlugin(pluginName);
            var records = RecordEnumerationService.GetRawRecords(plugin, recordType);
            return records?
                .Cast<object>()
                .FirstOrDefault(record => RecordMatches(record, formKey));
        }
        catch (Exception ex)
        {
            Logger.Warning(ex, "Unable to load {RecordType} {FormKey} from {PluginName}", recordType, formKey, pluginName);
            return null;
        }
    }

    /// <summary>
    ///     Determine whether a record's FormKey or FormID equals the expected value.
    /// </summary>
    /// <param name="record">The record to inspect.</param>
    /// <param name="formKey">The expected FormKey or FormID.</param>
    /// <returns>True when either key property matches; otherwise false.</returns>
    private static bool RecordMatches(object record, string formKey)
    {
        return (RecordHeaderMapper.GetFormKeyValue(record)?.Equals(formKey, StringComparison.OrdinalIgnoreCase) == true)
               || StringValueEquals(record, "FormID", formKey);
    }

    /// <summary>
    ///     Read a public property value from an object and convert it to text.
    /// </summary>
    /// <param name="source">The object containing the property.</param>
    /// <param name="propertyName">The public property name to read.</param>
    /// <returns>The property value as text, or null when the property is missing or null.</returns>
    private static string? GetStringValue(object source, string propertyName)
    {
        var value = source.GetType().GetProperty(propertyName)?.GetValue(source);
        return value?.ToString();
    }

    private static int? GetNullableIntValue(object source, string propertyName)
    {
        var value = source.GetType().GetProperty(propertyName)?.GetValue(source);
        if (value is null) return null;

        try
        {
            return Convert.ToInt32(value);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    ///     Compare a public property text value to an expected value using case-insensitive comparison.
    /// </summary>
    /// <param name="source">The object containing the property.</param>
    /// <param name="propertyName">The public property name to read.</param>
    /// <param name="expectedValue">The expected text value.</param>
    /// <returns>True when the property exists and equals the expected value; otherwise false.</returns>
    private static bool StringValueEquals(object source, string propertyName, string expectedValue)
    {
        var value = GetStringValue(source, propertyName);
        return value is not null && value.Equals(expectedValue, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    ///     Flatten a record's public fields into comparison values suitable for grid display.
    /// </summary>
    /// <param name="record">The record to flatten.</param>
    /// <param name="recordTypeOptions">Record-type-specific display options.</param>
    /// <param name="displayValueResolver">Optional resolver for displaying referenced records.</param>
    /// <returns>A field-name keyed map of comparison field values.</returns>
    private static IDictionary<string, RecordComparisonFieldValue> FlattenRecordFields(
        object record,
        RecordComparisonRecordTypeOptions recordTypeOptions,
        Func<object?, string?>? displayValueResolver = null)
    {
        var fields = new SortedDictionary<string, RecordComparisonFieldValue>(StringComparer.OrdinalIgnoreCase);
        FlattenObject(record, string.Empty, fields, recordTypeOptions, displayValueResolver, 0, new HashSet<object>(ReferenceEqualityComparer.Instance));
        return fields;
    }

    /// <summary>
    ///     Recursively flatten an object's public properties into comparison field values.
    /// </summary>
    /// <param name="source">The object to flatten.</param>
    /// <param name="prefix">The field-name prefix for nested values.</param>
    /// <param name="fields">The target field-value map.</param>
    /// <param name="recordTypeOptions">Record-type-specific display options.</param>
    /// <param name="displayValueResolver">Optional resolver for displaying referenced records.</param>
    /// <param name="depth">The current recursion depth.</param>
    /// <param name="visited">The reference-equality set used to prevent cycles.</param>
    private static void FlattenObject(
        object source,
        string prefix,
        IDictionary<string, RecordComparisonFieldValue> fields,
        RecordComparisonRecordTypeOptions recordTypeOptions,
        Func<object?, string?>? displayValueResolver,
        int depth,
        ISet<object> visited)
    {
        if (depth > MaxFieldDepth || !visited.Add(source)) return;

        foreach (var property in source.GetType()
                     .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                     .Where(property => property.GetIndexParameters().Length == 0)
                     .Where(property => !ExcludedFieldNames.Contains(property.Name))
                     .OrderBy(property => property.Name))
        {
            var fieldName = string.IsNullOrWhiteSpace(prefix) ? property.Name : $"{prefix}.{property.Name}";
            if (recordTypeOptions.IsHidden(fieldName)) continue;

            if (!TryGetPropertyValue(source, property, out var value)) continue;

            if (recordTypeOptions.IsTree(fieldName))
            {
                fields[fieldName] = RecordComparisonFieldValue.ForTree(ToTreeNodes(value, displayValueResolver));
                continue;
            }

            if (value is null)
            {
                fields[fieldName] = RecordComparisonFieldValue.ForText(null);
                continue;
            }

            var valueType = value.GetType();
            if ((Nullable.GetUnderlyingType(valueType) ?? valueType) == typeof(bool))
            {
                fields[fieldName] = RecordComparisonFieldValue.ForBoolean((bool)value);
                continue;
            }

            if (IsDisplayValue(valueType))
            {
                fields[fieldName] = RecordComparisonFieldValue.ForText(GetDisplayValue(value, displayValueResolver));
                continue;
            }

            if (value is IEnumerable enumerable and not string)
            {
                fields[fieldName] = RecordComparisonFieldValue.ForText(FormatEnumerable(enumerable, displayValueResolver));
                continue;
            }

            FlattenObject(value, fieldName, fields, recordTypeOptions, displayValueResolver, depth + 1, visited);
        }
    }

    /// <summary>
    ///     Choose the field display kind based on the values available across plugins.
    /// </summary>
    /// <param name="fieldName">The comparison field name.</param>
    /// <param name="fieldsByPlugin">The flattened field maps for each plugin.</param>
    /// <returns>The display kind to use for the field.</returns>
    private static RecordComparisonFieldDisplayKind GetDisplayKind(string fieldName, IEnumerable<IDictionary<string, RecordComparisonFieldValue>> fieldsByPlugin)
    {
        var values = fieldsByPlugin
            .Where(fields => fields.TryGetValue(fieldName, out _))
            .Select(fields => fields[fieldName])
            .ToList();

        if (values.Any(value => value.DisplayKind == RecordComparisonFieldDisplayKind.Tree))
        {
            return RecordComparisonFieldDisplayKind.Tree;
        }

        return values.Any(value => value.DisplayKind == RecordComparisonFieldDisplayKind.Boolean)
            ? RecordComparisonFieldDisplayKind.Boolean
            : RecordComparisonFieldDisplayKind.Text;
    }

    /// <summary>
    ///     Convert a value into tree nodes for fields configured to render hierarchically.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="displayValueResolver">Optional resolver for displaying referenced records.</param>
    /// <returns>The tree node list for display.</returns>
    private static IList<RecordComparisonFieldNodeDTO> ToTreeNodes(
        object? value,
        Func<object?, string?>? displayValueResolver)
    {
        if (value is null)
        {
            return new List<RecordComparisonFieldNodeDTO>();
        }

        if (value is IEnumerable enumerable and not string)
        {
            return ToEnumerableTreeNodes(enumerable, displayValueResolver, 0, new HashSet<object>(ReferenceEqualityComparer.Instance));
        }

        return new List<RecordComparisonFieldNodeDTO>
        {
            ToTreeNode("Value", value, displayValueResolver, 0, new HashSet<object>(ReferenceEqualityComparer.Instance))
        };
    }

    /// <summary>
    ///     Convert an enumerable value into indexed child tree nodes.
    /// </summary>
    /// <param name="enumerable">The enumerable to convert.</param>
    /// <param name="displayValueResolver">Optional resolver for displaying referenced records.</param>
    /// <param name="depth">The current recursion depth.</param>
    /// <param name="visited">The reference-equality set used to prevent cycles.</param>
    /// <returns>The indexed tree nodes.</returns>
    private static IList<RecordComparisonFieldNodeDTO> ToEnumerableTreeNodes(
        IEnumerable enumerable,
        Func<object?, string?>? displayValueResolver,
        int depth,
        ISet<object> visited)
    {
        return enumerable
            .Cast<object?>()
            .Select((value, index) => ToTreeNode($"[{index}]", value, displayValueResolver, depth, visited))
            .ToList();
    }

    /// <summary>
    ///     Convert one object or scalar value into a tree node.
    /// </summary>
    /// <param name="name">The tree node display name.</param>
    /// <param name="value">The value to convert.</param>
    /// <param name="displayValueResolver">Optional resolver for displaying referenced records.</param>
    /// <param name="depth">The current recursion depth.</param>
    /// <param name="visited">The reference-equality set used to prevent cycles.</param>
    /// <returns>The converted tree node.</returns>
    private static RecordComparisonFieldNodeDTO ToTreeNode(
        string name,
        object? value,
        Func<object?, string?>? displayValueResolver,
        int depth,
        ISet<object> visited)
    {
        if (value is null)
        {
            return new RecordComparisonFieldNodeDTO
            {
                Name = name
            };
        }

        var valueType = value.GetType();
        if (IsDisplayValue(valueType) || depth >= MaxFieldDepth || !visited.Add(value))
        {
            return new RecordComparisonFieldNodeDTO
            {
                Name = name,
                Value = GetDisplayValue(value, displayValueResolver)
            };
        }

        if (value is IEnumerable enumerable and not string)
        {
            return new RecordComparisonFieldNodeDTO
            {
                Name = name,
                Children = ToEnumerableTreeNodes(enumerable, displayValueResolver, depth + 1, visited)
            };
        }

        var node = new RecordComparisonFieldNodeDTO
        {
            Name = name
        };

        foreach (var property in value.GetType()
                     .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                     .Where(property => property.GetIndexParameters().Length == 0)
                     .Where(property => !ExcludedFieldNames.Contains(property.Name))
                     .OrderBy(property => property.Name))
        {
            if (TryGetPropertyValue(value, property, out var propertyValue))
            {
                node.Children.Add(ToTreeNode(property.Name, propertyValue, displayValueResolver, depth + 1, visited));
            }
        }

        if (node.Children.Count == 0)
        {
            node.Value = GetDisplayValue(value, displayValueResolver);
        }

        return node;
    }

    /// <summary>
    ///     Get the display text for a value, preferring the supplied reference resolver when available.
    /// </summary>
    /// <param name="value">The value to display.</param>
    /// <param name="displayValueResolver">Optional resolver for displaying referenced records.</param>
    /// <returns>The display value, or null when the input is null.</returns>
    private static string? GetDisplayValue(object? value, Func<object?, string?>? displayValueResolver)
    {
        return displayValueResolver?.Invoke(value) ?? value?.ToString();
    }

    /// <summary>
    ///     Safely read a property value, treating property getter failures as null values.
    /// </summary>
    /// <param name="source">The object containing the property.</param>
    /// <param name="property">The property to read.</param>
    /// <param name="value">The property value when read successfully; otherwise null.</param>
    /// <returns>True when the property getter succeeds; otherwise false.</returns>
    private static bool TryGetPropertyValue(object source, PropertyInfo property, out object? value)
    {
        try
        {
            value = property.GetValue(source);
            return true;
        }
        catch
        {
            value = null;
            return false;
        }
    }

    /// <summary>
    ///     Determine whether a type should be displayed directly instead of recursively flattened.
    /// </summary>
    /// <param name="valueType">The value type to classify.</param>
    /// <returns>True when the type should be displayed as a scalar; otherwise false.</returns>
    private static bool IsDisplayValue(Type valueType)
    {
        var type = Nullable.GetUnderlyingType(valueType) ?? valueType;
        return type.IsPrimitive
               || type.IsEnum
               || type == typeof(string)
               || type == typeof(decimal)
               || type == typeof(DateTime)
               || type == typeof(DateTimeOffset)
               || type == typeof(Guid)
               || type.Namespace?.StartsWith("Mutagen.Bethesda.Plugins", StringComparison.Ordinal) == true;
    }

    /// <summary>
    ///     Format an enumerable as a comma-separated text value with a maximum item count.
    /// </summary>
    /// <param name="enumerable">The enumerable to format.</param>
    /// <param name="displayValueResolver">Optional resolver for displaying referenced records.</param>
    /// <returns>The formatted enumerable value.</returns>
    private static string FormatEnumerable(
        IEnumerable enumerable,
        Func<object?, string?>? displayValueResolver)
    {
        var values = enumerable
            .Cast<object?>()
            .Take(MaxCollectionItems + 1)
            .Select(value => GetDisplayValue(value, displayValueResolver))
            .ToList();

        if (values.Count == 0)
        {
            return string.Empty;
        }

        var hasMore = values.Count > MaxCollectionItems;
        if (hasMore)
        {
            values = values.Take(MaxCollectionItems).ToList();
        }

        var formattedValue = string.Join(", ", values);
        return hasMore ? $"{formattedValue}, ..." : formattedValue;
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
                return parameters.Length == 1
                       && parameters[0].ParameterType.IsAssignableFrom(typeof(IEnumerable<IStarfieldModGetter>));
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
            recordType.EndsWith("y", StringComparison.OrdinalIgnoreCase)
                ? $"{recordType[..^1]}ies"
                : $"{recordType}s"
        };

        foreach (var propertyName in propertyNames.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            var property = plugin.GetType().GetProperty(propertyName);
            if (property?.GetValue(plugin) is IEnumerable records) return records;
        }

        return null;
    }

    internal sealed class RecordComparisonFieldValue
    {
        private RecordComparisonFieldValue(RecordComparisonFieldDisplayKind displayKind)
        {
            DisplayKind = displayKind;
        }

        public RecordComparisonFieldDisplayKind DisplayKind { get; }
        public string? TextValue { get; private init; }
        public bool? BooleanValue { get; private init; }
        public IList<RecordComparisonFieldNodeDTO>? TreeNodes { get; private init; }

        public static RecordComparisonFieldValue ForText(string? value)
        {
            return new RecordComparisonFieldValue(RecordComparisonFieldDisplayKind.Text)
            {
                TextValue = value
            };
        }

        public static RecordComparisonFieldValue ForBoolean(bool value)
        {
            return new RecordComparisonFieldValue(RecordComparisonFieldDisplayKind.Boolean)
            {
                BooleanValue = value,
                TextValue = value.ToString()
            };
        }

        public static RecordComparisonFieldValue ForTree(IList<RecordComparisonFieldNodeDTO> nodes)
        {
            return new RecordComparisonFieldValue(RecordComparisonFieldDisplayKind.Tree)
            {
                TreeNodes = nodes,
                TextValue = nodes.Count == 0 ? string.Empty : $"{nodes.Count} item(s)"
            };
        }
    }

    private sealed class RecordReferenceDisplayResolver
    {
        private readonly Func<string, string?> ResolveDisplayValue;

        public RecordReferenceDisplayResolver(Func<string, string?> resolveDisplayValue)
        {
            ResolveDisplayValue = resolveDisplayValue;
        }

        /// <summary>
        ///     Resolve a reference-like value to a cached display value, falling back to a normalized reference string.
        /// </summary>
        /// <param name="value">The value to resolve.</param>
        /// <returns>The resolved display value or normalized reference text.</returns>
        public string? GetDisplayValue(object? value)
        {
            var rawValue = value?.ToString();
            if (string.IsNullOrWhiteSpace(rawValue) || !LooksLikeReference(value, rawValue))
            {
                return rawValue;
            }

            var normalizedValue = FormKeyTextNormalizer.NormalizeReferenceValue(rawValue);
            return ResolveDisplayValue(rawValue) ?? ResolveDisplayValue(normalizedValue) ?? normalizedValue;
        }

        /// <summary>
        ///     Determine whether a value is likely to represent a plugin record reference.
        /// </summary>
        /// <param name="value">The original value object.</param>
        /// <param name="rawValue">The value converted to text.</param>
        /// <returns>True when the value appears to be a FormKey/FormLink or plugin reference string.</returns>
        private static bool LooksLikeReference(object? value, string rawValue)
        {
            var typeName = value?.GetType().Name;
            return typeName?.Contains("FormKey", StringComparison.OrdinalIgnoreCase) == true
                   || typeName?.Contains("FormLink", StringComparison.OrdinalIgnoreCase) == true
                   || rawValue.StartsWith("formid:", StringComparison.OrdinalIgnoreCase)
                   || rawValue.Contains(".esm", StringComparison.OrdinalIgnoreCase)
                   || rawValue.Contains(".esp", StringComparison.OrdinalIgnoreCase)
                   || rawValue.Contains(".esl", StringComparison.OrdinalIgnoreCase);
        }
    }
}
