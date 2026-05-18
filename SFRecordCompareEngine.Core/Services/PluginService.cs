using System.IO;
using System.Reflection;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records.Mapping;
using Mutagen.Bethesda.Starfield;
using Serilog;
using SFRecordCompareEngine.Core.Configuration.Interfaces;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
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

    private readonly ILogger Logger = Log.ForContext<PluginService>();
    private readonly IGameConfigurationStore GameConfigurationStore;

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

            return records
                .Cast<object>()
                .Select(record => new RecordSummaryDTO
                {
                    RecordType = recordType,
                    FormID = GetStringValue(record, "FormKey") ?? GetStringValue(record, "FormID"),
                    EditorID = GetStringValue(record, "EditorID")
                })
                .ToList();
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

            var fieldsByPlugin = recordsByPlugin
                .Where(item => item.Value is not null)
                .ToDictionary(
                    item => item.Key,
                    item => FlattenRecordFields(item.Value!),
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
                    ValuesByPlugin = comparison.Plugins.ToDictionary(
                        plugin => plugin.PluginName,
                        plugin => fieldsByPlugin.TryGetValue(plugin.PluginName, out var fields)
                                  && fields.TryGetValue(fieldName, out var value)
                            ? value
                            : null,
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

    private static IDictionary<string, string?> FlattenRecordFields(object record)
    {
        var fields = new SortedDictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        FlattenObject(record, string.Empty, fields, 0, new HashSet<object>(ReferenceEqualityComparer.Instance));
        return fields;
    }

    private static void FlattenObject(
        object source,
        string prefix,
        IDictionary<string, string?> fields,
        int depth,
        ISet<object> visited)
    {
        if (depth > MaxFieldDepth || !visited.Add(source))
        {
            return;
        }

        foreach (var property in source.GetType()
                     .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                     .Where(property => property.GetIndexParameters().Length == 0)
                     .Where(property => !ExcludedFieldNames.Contains(property.Name))
                     .OrderBy(property => property.Name))
        {
            var fieldName = string.IsNullOrWhiteSpace(prefix) ? property.Name : $"{prefix}.{property.Name}";
            if (!TryGetPropertyValue(source, property, out var value))
            {
                continue;
            }

            if (value is null)
            {
                fields[fieldName] = null;
                continue;
            }

            var valueType = value.GetType();
            if (IsDisplayValue(valueType))
            {
                fields[fieldName] = value.ToString();
                continue;
            }

            if (value is System.Collections.IEnumerable enumerable && value is not string)
            {
                fields[fieldName] = FormatEnumerable(enumerable);
                continue;
            }

            FlattenObject(value, fieldName, fields, depth + 1, visited);
        }
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

    private static string FormatEnumerable(System.Collections.IEnumerable enumerable)
    {
        var values = enumerable
            .Cast<object?>()
            .Take(MaxCollectionItems + 1)
            .Select(value => value?.ToString())
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

    private static System.Collections.IEnumerable? GetRecordsFromMutagenTypeOption(
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

        return method?.Invoke(null, [new[] { plugin }]) as System.Collections.IEnumerable;
    }

    private static System.Collections.IEnumerable? GetRecordsFromPluginProperty(
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
            if (property?.GetValue(plugin) is System.Collections.IEnumerable records)
            {
                return records;
            }
        }

        return null;
    }
}
