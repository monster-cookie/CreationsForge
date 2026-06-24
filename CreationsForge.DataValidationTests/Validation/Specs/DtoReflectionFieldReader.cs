using System.Collections;
using System.Globalization;
using System.Reflection;
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;

namespace CreationsForge.DataValidationTests.Validation.Specs;

public class DtoReflectionFieldReader
{
    private static readonly HashSet<string> IgnoredRootProperties = new(StringComparer.Ordinal)
    {
        "Game",
        "ModKey",
        "ImportedAtUTC"
    };

    private static readonly HashSet<string> IgnoredNestedProperties = new(StringComparer.Ordinal)
    {
        "Game",
        "ModKey",
        "RecordType",
        "FormKey",
        "ImportedAtUTC",
        "BodyTextIndex",
        "ComponentIndex",
        "ItemIndex",
        "KeywordIndex",
        "ListItemIndex",
        "MaterialSwapIndex",
        "MenuItemIndex",
        "ModelGender",
        "ModelSlot",
        "ObjectUnused",
        "PayloadIndex",
        "ParameterIndex",
        "PropertyIndex",
        "ResourceIndex",
        "RelationIndex",
        "ScriptIndex",
        "ScriptingAdapterName",
        "SoundIndex",
        "StageIndex",
        "WeightIndex",
        "WeightType"
    };

    public IReadOnlyDictionary<string, string> Read(RecordDTO record)
    {
        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        FlattenValue(values, string.Empty, record.Game, record);
        NormalizeConditionFields(values);
        return values;
    }

    private static void NormalizeConditionFields(IDictionary<string, string> values)
    {
        foreach (var field in values.Keys.Where(field => field.EndsWith(".DataMutagenObjectType", StringComparison.OrdinalIgnoreCase)).ToList())
        {
            var basePath = field[..^".DataMutagenObjectType".Length];
            if (values.TryGetValue(field, out var value) && !IsNullValue(value))
            {
                values[basePath + ".Data.MutagenObjectType"] = NormalizeMutagenObjectTypeName(value);
            }

            AddConditionComparisonValueAlias(values, basePath);
            AddConditionFlagAliases(values, basePath);
            AddConditionParameterAliases(values, basePath);
            RemoveConditionInternalFields(values, basePath);
        }

        foreach (var field in values.Keys.Where(field => field.EndsWith(".MutagenObjectType", StringComparison.OrdinalIgnoreCase)).ToList())
        {
            values[field] = NormalizeMutagenObjectTypeName(values[field]);
        }
    }

    private static void AddConditionComparisonValueAlias(IDictionary<string, string> values, string basePath)
    {
        if (values.TryGetValue(basePath + ".ComparisonValueFormKey", out var formKey) && !IsNullValue(formKey))
        {
            values[basePath + ".ComparisonValue"] = formKey;
        }
    }

    private static void AddConditionFlagAliases(IDictionary<string, string> values, string basePath)
    {
        if (!values.TryGetValue(basePath + ".Flags", out var flags) || IsNullValue(flags) || string.IsNullOrWhiteSpace(flags))
        {
            return;
        }

        var flagValues = flags.Split(", ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        values[basePath + ".Flags.Count"] = flagValues.Length.ToString(CultureInfo.InvariantCulture);
        for (var index = 0; index < flagValues.Length; index++)
        {
            values[basePath + ".Flags[" + index.ToString(CultureInfo.InvariantCulture) + "]"] = flagValues[index];
        }
    }

    private static void AddConditionParameterAliases(IDictionary<string, string> values, string basePath)
    {
        if (!values.TryGetValue(basePath + ".Parameters.Count", out var countValue) ||
            !int.TryParse(countValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var count))
        {
            return;
        }

        for (var index = 0; index < count; index++)
        {
            var parameterPath = basePath + ".Parameters[" + index.ToString(CultureInfo.InvariantCulture) + "]";
            if (!values.TryGetValue(parameterPath + ".ParameterName", out var parameterName) || string.IsNullOrWhiteSpace(parameterName))
            {
                continue;
            }

            if (values.TryGetValue(parameterPath + ".ParameterFormKey", out var parameterFormKey) && !IsNullValue(parameterFormKey))
            {
                values[basePath + ".Data." + parameterName] = parameterFormKey;
                continue;
            }

            if (values.TryGetValue(parameterPath + ".ParameterValue", out var parameterValue) && !IsNullValue(parameterValue))
            {
                values[basePath + ".Data." + parameterName] = parameterValue;
            }
        }
    }

    private static void RemoveConditionInternalFields(IDictionary<string, string> values, string basePath)
    {
        foreach (var field in values.Keys
            .Where(field => string.Equals(field, basePath + ".ConditionSlot", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(field, basePath + ".DataMutagenObjectType", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(field, basePath + ".ComparisonValueFormKey", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(field, basePath + ".Flags", StringComparison.OrdinalIgnoreCase) ||
                field.StartsWith(basePath + ".Parameters", StringComparison.OrdinalIgnoreCase))
            .ToList())
        {
            values.Remove(field);
        }
    }

    private static bool IsNullValue(string value)
    {
        return string.Equals(value, "Null", StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizeMutagenObjectTypeName(string value)
    {
        const string binaryOverlaySuffix = "BinaryOverlay";
        return value.EndsWith(binaryOverlaySuffix, StringComparison.Ordinal)
            ? value[..^binaryOverlaySuffix.Length]
            : value;
    }

    private static void FlattenValue(IDictionary<string, string> values, string path, SupportedGame game, object? value)
    {
        if (string.IsNullOrWhiteSpace(path) && value is not null)
        {
            foreach (var property in GetReadableProperties(value.GetType()))
            {
                if (IgnoredRootProperties.Contains(property.Name))
                {
                    continue;
                }

                FlattenValue(values, property.Name, game, property.GetValue(value));
            }

            return;
        }

        if (value is null)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                values[path] = "Null";
            }

            return;
        }

        if (value is FormKeyDTO formKey)
        {
            values[path] = FormatFormKey(formKey);
            return;
        }

        if (value is TranslatedStringDTO translatedString)
        {
            FlattenTranslatedString(values, path, translatedString, game);
            return;
        }

        if (value is ModKeyDTO modKey)
        {
            values[path] = modKey.FileName;
            return;
        }

        if (IsScalar(value))
        {
            values[path] = ConvertScalar(value);
            return;
        }

        if (value is IEnumerable enumerable && value is not string)
        {
            var index = 0;
            foreach (var item in enumerable)
            {
                FlattenValue(values, path + "[" + index.ToString(CultureInfo.InvariantCulture) + "]", game, item);
                index++;
            }

            values[path + ".Count"] = index.ToString(CultureInfo.InvariantCulture);
            return;
        }

        foreach (var property in GetReadableProperties(value.GetType()))
        {
            if (IgnoredNestedProperties.Contains(property.Name))
            {
                continue;
            }

            FlattenValue(values, path + "." + property.Name, game, property.GetValue(value));
        }
    }

    private static void FlattenTranslatedString(
        IDictionary<string, string> values,
        string path,
        TranslatedStringDTO translatedString,
        SupportedGame game)
    {
        var entries = translatedString.Strings
            .OrderBy(entry => GetSpriggitLanguageOrder(entry.Language, game))
            .ThenBy(entry => entry.Language, StringComparer.Ordinal)
            .ToList();

        values[path + ".Count"] = entries.Count.ToString(CultureInfo.InvariantCulture);
        values[path + ".TargetLanguage"] = translatedString.TargetLanguage;

        for (var entryIndex = 0; entryIndex < entries.Count; entryIndex++)
        {
            values[path + "[" + entryIndex.ToString(CultureInfo.InvariantCulture) + "].Language"] = entries[entryIndex].Language;
            values[path + "[" + entryIndex.ToString(CultureInfo.InvariantCulture) + "].String"] = entries[entryIndex].String;
        }
    }

    private static IReadOnlyList<PropertyInfo> GetReadableProperties(Type type)
    {
        return type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(property => property.GetIndexParameters().Length == 0)
            .OrderBy(property => property.Name, StringComparer.Ordinal)
            .ToList();
    }

    private static int GetSpriggitLanguageOrder(string language, SupportedGame game)
    {
        var order = game switch
        {
            SupportedGame.Fallout4 => Fallout4SpriggitLanguageOrder,
            SupportedGame.Skyrim => SkyrimSpriggitLanguageOrder,
            _ => StarfieldSpriggitLanguageOrder
        };

        return order.TryGetValue(language, out var index)
            ? index
            : int.MaxValue;
    }

    private static readonly IReadOnlyDictionary<string, int> StarfieldSpriggitLanguageOrder = new Dictionary<string, int>(StringComparer.Ordinal)
    {
        ["German"] = 0,
        ["English"] = 1,
        ["Spanish"] = 2,
        ["French"] = 3,
        ["Italian"] = 4,
        ["Japanese"] = 5,
        ["Polish"] = 6,
        ["Portuguese_Brazil"] = 7,
        ["ChineseSimplified"] = 8
    };

    private static readonly IReadOnlyDictionary<string, int> Fallout4SpriggitLanguageOrder = new Dictionary<string, int>(StringComparer.Ordinal)
    {
        ["Chinese"] = 0,
        ["German"] = 1,
        ["English"] = 2,
        ["Spanish"] = 3,
        ["Spanish_Mexico"] = 4,
        ["French"] = 5,
        ["Italian"] = 6,
        ["Japanese"] = 7,
        ["Polish"] = 8,
        ["Portuguese_Brazil"] = 9,
        ["Russian"] = 10
    };

    private static readonly IReadOnlyDictionary<string, int> SkyrimSpriggitLanguageOrder = new Dictionary<string, int>(StringComparer.Ordinal)
    {
        ["German"] = 0,
        ["English"] = 1,
        ["Spanish"] = 2,
        ["Italian"] = 3,
        ["Chinese"] = 4,
        ["Polish"] = 5,
        ["Russian"] = 6,
        ["French"] = 7,
        ["Japanese"] = 8
    };

    private static bool IsScalar(object value)
    {
        var type = value.GetType();
        return type.IsPrimitive ||
               type.IsEnum ||
               value is string or decimal or DateTime or Guid;
    }

    private static string ConvertScalar(object value)
    {
        return value switch
        {
            bool boolean => boolean.ToString(CultureInfo.InvariantCulture),
            IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
            _ => value.ToString() ?? string.Empty
        };
    }

    private static string FormatFormKey(FormKeyDTO formKey)
    {
        return formKey.Id.ToString("X6", CultureInfo.InvariantCulture) + ":" + formKey.ModKey.FileName;
    }
}
