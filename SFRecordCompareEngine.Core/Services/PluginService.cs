using System.Collections;
using System.IO;
using System.Reflection;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records.Mapping;
using Mutagen.Bethesda.Starfield;
using Serilog;
using SFRecordCompareEngine.Core.Configuration.Interfaces;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Models.Records;
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

    private readonly ILogger Logger = Log.ForContext<PluginService>();

    public PluginService(IGameConfigurationStore gameConfigurationStore)
    {
        GameConfigurationStore = gameConfigurationStore;
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
            var gameEnvironment = GameConfigurationStore.Game;
            if (gameEnvironment is null)
            {
                Logger.Warning("Unable to load plugins because no game environment is configured");
                return new List<string>();
            }

            var plugins = new List<string>();
            foreach (var plugin in gameEnvironment.LoadOrder.ListedOrder)
            {
                // Exclude the base game database as all other records automatically compare to it.
                if (plugin.FileName.Equals("Starfield.esm", StringComparison.CurrentCultureIgnoreCase)) continue;
                plugins.Add(plugin.FileName);
            }

            return plugins;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Unable to load plugins");
            return new List<string>();
        }
    }

    public PluginHeaderDTO? GetPluginHeader(string pluginName)
    {
        try
        {
            var plugin = LoadPlugin(pluginName);
            return new PluginHeaderDTO(pluginName, plugin.ModHeader);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Unable to load plugin header for {PluginName}", pluginName);
            return null;
        }
    }

    /// <inheritdoc />
    public IList<RecordSummaryDTO> GetRecords(string pluginName, string recordType)
    {
        try
        {
            var plugin = LoadPlugin(pluginName);
            var records = GetRecordsFromMutagenTypeOption(plugin, recordType) ?? GetRecordsFromPluginProperty(plugin, recordType);
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
                    FormID = GetStringValue(record, "FormKey") ?? GetStringValue(record, "FormID"),
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
            var referenceDisplayResolver = new RecordReferenceDisplayResolver(
                () => BuildReferenceDisplayValues(pluginNames),
                ResolveReferenceEditorId);
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

    private IList<string> GetComparisonPluginNames(string pluginName)
    {
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

    private object? LoadRecord(string pluginName, string recordType, string formKey)
    {
        try
        {
            using var plugin = LoadPlugin(pluginName);
            var records = GetRecordsFromMutagenTypeOption(plugin, recordType) ?? GetRecordsFromPluginProperty(plugin, recordType);
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

    private static bool RecordMatches(object record, string formKey)
    {
        return StringValueEquals(record, "FormKey", formKey)
               || StringValueEquals(record, "FormID", formKey);
    }

    private static string? GetStringValue(object source, string propertyName)
    {
        var value = source.GetType().GetProperty(propertyName)?.GetValue(source);
        return value?.ToString();
    }

    private static bool StringValueEquals(object source, string propertyName, string expectedValue)
    {
        var value = GetStringValue(source, propertyName);
        return value is not null && value.Equals(expectedValue, StringComparison.OrdinalIgnoreCase);
    }

    private static IDictionary<string, RecordComparisonFieldValue> FlattenRecordFields(
        object record,
        RecordComparisonRecordTypeOptions recordTypeOptions,
        Func<object?, string?>? displayValueResolver = null)
    {
        var fields = new SortedDictionary<string, RecordComparisonFieldValue>(StringComparer.OrdinalIgnoreCase);
        FlattenObject(record, string.Empty, fields, recordTypeOptions, displayValueResolver, 0, new HashSet<object>(ReferenceEqualityComparer.Instance));
        return fields;
    }

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

            if (value is IEnumerable enumerable && value is not string)
            {
                fields[fieldName] = RecordComparisonFieldValue.ForText(FormatEnumerable(enumerable, displayValueResolver));
                continue;
            }

            FlattenObject(value, fieldName, fields, recordTypeOptions, displayValueResolver, depth + 1, visited);
        }
    }

    private static RecordComparisonFieldDisplayKind GetDisplayKind(
        string fieldName,
        IEnumerable<IDictionary<string, RecordComparisonFieldValue>> fieldsByPlugin)
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

    private static IList<RecordComparisonFieldNodeDTO> ToTreeNodes(
        object? value,
        Func<object?, string?>? displayValueResolver)
    {
        if (value is null)
        {
            return new List<RecordComparisonFieldNodeDTO>();
        }

        if (value is IEnumerable enumerable && value is not string)
        {
            return ToEnumerableTreeNodes(enumerable, displayValueResolver, 0, new HashSet<object>(ReferenceEqualityComparer.Instance));
        }

        return new List<RecordComparisonFieldNodeDTO>
        {
            ToTreeNode("Value", value, displayValueResolver, 0, new HashSet<object>(ReferenceEqualityComparer.Instance))
        };
    }

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
        if (IsDisplayValue(valueType))
        {
            return new RecordComparisonFieldNodeDTO
            {
                Name = name,
                Value = GetDisplayValue(value, displayValueResolver)
            };
        }

        if (depth >= MaxFieldDepth || !visited.Add(value))
        {
            return new RecordComparisonFieldNodeDTO
            {
                Name = name,
                Value = GetDisplayValue(value, displayValueResolver)
            };
        }

        if (value is IEnumerable enumerable && value is not string)
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

    private static string? GetDisplayValue(object? value, Func<object?, string?>? displayValueResolver)
    {
        return displayValueResolver?.Invoke(value) ?? value?.ToString();
    }

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

    private static IEnumerable? GetRecordsFromMutagenTypeOption(
        IStarfieldModGetter plugin,
        string recordType)
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

    private static IEnumerable? GetRecordsFromPluginProperty(
        IStarfieldModGetter plugin,
        string recordType)
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

    private IDictionary<string, string> BuildReferenceDisplayValues(IList<string> pluginNames)
    {
        var displayValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var comparisonPluginName in pluginNames)
        {
            try
            {
                using var plugin = LoadPlugin(comparisonPluginName);
                foreach (var recordType in GetRecordTypes())
                {
                    var records = GetRecordsFromMutagenTypeOption(plugin, recordType) ?? GetRecordsFromPluginProperty(plugin, recordType);
                    if (records is null) continue;

                    foreach (var record in records.Cast<object>())
                    {
                        AddReferenceDisplayValue(displayValues, record);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warning(ex, "Unable to build reference display values for {PluginName}", comparisonPluginName);
            }
        }

        return displayValues;
    }

    private string? ResolveReferenceEditorId(string referenceValue)
    {
        var normalizedReferenceValue = NormalizeReferenceValue(referenceValue);
        if (string.IsNullOrWhiteSpace(normalizedReferenceValue)
            || !FormKey.TryFactory(normalizedReferenceValue, out var formKey))
        {
            return null;
        }

        try
        {
            return GameConfigurationStore.Game?.LinkCache.TryResolveIdentifier<IStarfieldMajorRecordGetter>(
                formKey,
                out var editorId,
                ResolveTarget.Winner) == true
                ? editorId
                : null;
        }
        catch
        {
            return null;
        }
    }

    private static void AddReferenceDisplayValue(IDictionary<string, string> displayValues, object record)
    {
        var editorId = GetStringValue(record, "EditorID");
        if (string.IsNullOrWhiteSpace(editorId))
        {
            return;
        }

        AddReferenceDisplayValue(displayValues, GetStringValue(record, "FormKey"), editorId);
        AddReferenceDisplayValue(displayValues, GetStringValue(record, "FormID"), editorId);
    }

    private static void AddReferenceDisplayValue(IDictionary<string, string> displayValues, string? referenceValue, string editorId)
    {
        if (string.IsNullOrWhiteSpace(referenceValue)) return;

        displayValues.TryAdd(referenceValue, editorId);

        var normalizedReferenceValue = NormalizeReferenceValue(referenceValue);
        if (!string.IsNullOrWhiteSpace(normalizedReferenceValue))
        {
            displayValues.TryAdd(normalizedReferenceValue, editorId);
        }
    }

    private static string NormalizeReferenceValue(string referenceValue)
    {
        var normalizedReferenceValue = referenceValue.Trim();
        if (normalizedReferenceValue.StartsWith("formid:", StringComparison.OrdinalIgnoreCase))
        {
            normalizedReferenceValue = normalizedReferenceValue["formid:".Length..].Trim();
        }

        var mutagenTypeSuffixIndex = normalizedReferenceValue.LastIndexOf('<');
        if (mutagenTypeSuffixIndex > 0 && normalizedReferenceValue.EndsWith(">", StringComparison.Ordinal))
        {
            normalizedReferenceValue = normalizedReferenceValue[..mutagenTypeSuffixIndex].Trim();
        }

        return normalizedReferenceValue;
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
        private readonly Func<IDictionary<string, string>> BuildDisplayValues;
        private readonly IDictionary<string, string?> DirectDisplayValues = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        private readonly Func<string, string?> ResolveDisplayValue;
        private IDictionary<string, string>? DisplayValues;

        public RecordReferenceDisplayResolver(
            Func<IDictionary<string, string>> buildDisplayValues,
            Func<string, string?> resolveDisplayValue)
        {
            BuildDisplayValues = buildDisplayValues;
            ResolveDisplayValue = resolveDisplayValue;
        }

        public string? GetDisplayValue(object? value)
        {
            var rawValue = value?.ToString();
            if (string.IsNullOrWhiteSpace(rawValue) || !LooksLikeReference(value, rawValue))
            {
                return rawValue;
            }

            var normalizedValue = NormalizeReferenceValue(rawValue);
            DisplayValues ??= BuildDisplayValues();
            var outValue = DisplayValues.TryGetValue(rawValue, out var displayValue) ||
                           DisplayValues.TryGetValue(normalizedValue, out displayValue)
                ? displayValue
                : GetDirectDisplayValue(normalizedValue) ?? normalizedValue;
            return outValue;
        }

        private string? GetDirectDisplayValue(string normalizedValue)
        {
            if (!DirectDisplayValues.TryGetValue(normalizedValue, out var displayValue))
            {
                displayValue = ResolveDisplayValue(normalizedValue);
                DirectDisplayValues[normalizedValue] = displayValue;
            }

            return displayValue;
        }

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
