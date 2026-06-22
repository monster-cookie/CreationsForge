using System.Collections;
using System.Globalization;
using System.Reflection;
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Records.Metadata;
using CreationsForge.Core.Enums;

namespace CreationsForge.DataValidationTests.Validation.Parsing;

public class DtoFlattener
{
    private static readonly HashSet<string> IgnoredRootProperties = new(StringComparer.Ordinal)
    {
        "Game",
        "ModKey",
        "ImportedAtUTC"
    };

    public IReadOnlyDictionary<string, string> Flatten(object instance)
    {
        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        FlattenValue(values, string.Empty, instance is RecordDTO record ? record.Game : null, instance);
        return values;
    }

    private static void FlattenValue(IDictionary<string, string> values, string path, SupportedGame? game, object? value)
    {
        if (string.IsNullOrWhiteSpace(path) && value is not null)
        {
            foreach (var property in GetReadableProperties(value.GetType()))
            {
                if (IgnoredRootProperties.Contains(property.Name))
                {
                    continue;
                }

                if (value is GameSettingDTO && string.Equals(property.Name, nameof(GameSettingDTO.DataType), StringComparison.Ordinal))
                {
                    continue;
                }

                if (property.GetValue(value) is IEnumerable<LocalizedStringDTO> localizedStrings)
                {
                    FlattenLocalizedStrings(values, localizedStrings);
                    continue;
                }

                var propertyValue = property.GetValue(value);
                FlattenValue(values, property.Name, game, propertyValue);
                AddSpriggitPathAliases(values, property, property.Name, game, propertyValue);
            }

            return;
        }

        if (value is null)
        {
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

        if (value is GameSettingDataDTO gameSettingData)
        {
            FlattenGameSettingData(values, path, game, gameSettingData);
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
            var propertyPath = path + "." + property.Name;
            var propertyValue = property.GetValue(value);
            FlattenValue(values, propertyPath, game, propertyValue);
            AddSpriggitPathAliases(values, property, propertyPath, game, propertyValue);
        }
    }

    private static void AddSpriggitPathAliases(
        IDictionary<string, string> values,
        PropertyInfo property,
        string propertyPath,
        SupportedGame? game,
        object? propertyValue)
    {
        var attributes = property.GetCustomAttributes<SpriggitPathAttribute>()
            .Where(attribute => attribute.AppliesTo(game))
            .ToList();
        if (attributes.Count == 0 || propertyValue is null)
        {
            return;
        }

        var aliasValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        FlattenValue(aliasValues, propertyPath, game, propertyValue);

        foreach (var attribute in attributes)
        {
            foreach (var pair in aliasValues.ToList())
            {
                if (string.Equals(pair.Key, propertyPath, StringComparison.OrdinalIgnoreCase))
                {
                    values[attribute.Path] = pair.Value;
                    continue;
                }

                if (pair.Key.StartsWith(propertyPath + ".", StringComparison.OrdinalIgnoreCase) ||
                    pair.Key.StartsWith(propertyPath + "[", StringComparison.OrdinalIgnoreCase))
                {
                    values[attribute.Path + pair.Key[propertyPath.Length..]] = pair.Value;
                }
            }
        }
    }

    private static void FlattenTranslatedString(IDictionary<string, string> values, string path, TranslatedStringDTO translatedString, SupportedGame? game)
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

    private static void FlattenGameSettingData(IDictionary<string, string> values, string path, SupportedGame? game, GameSettingDataDTO gameSettingData)
    {
        switch (gameSettingData.DataType)
        {
            case GameSettingDataType.Boolean:
                FlattenValue(values, path, game, gameSettingData.Boolean);
                break;
            case GameSettingDataType.Float:
                FlattenValue(values, path, game, gameSettingData.Float);
                break;
            case GameSettingDataType.Integer:
                FlattenValue(values, path, game, gameSettingData.Integer);
                break;
            case GameSettingDataType.String:
                FlattenValue(values, path, game, gameSettingData.String);
                break;
            case GameSettingDataType.UnsignedInteger:
                FlattenValue(values, path, game, gameSettingData.UnsignedInteger);
                break;
        }
    }

    private static IReadOnlyList<PropertyInfo> GetReadableProperties(Type type)
    {
        return type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(property => property.GetIndexParameters().Length == 0)
            .OrderBy(property => property.Name, StringComparer.Ordinal)
            .ToList();
    }

    private static void FlattenLocalizedStrings(IDictionary<string, string> values, IEnumerable<LocalizedStringDTO> localizedStrings)
    {
        foreach (var sourceFieldGroup in localizedStrings.GroupBy(localizedString => localizedString.SourceField, StringComparer.Ordinal))
        {
            var sourceField = sourceFieldGroup.Key;
            var entries = sourceFieldGroup
                .OrderBy(localizedString => GetSpriggitLanguageOrder(localizedString))
                .ThenBy(localizedString => localizedString.Language, StringComparer.Ordinal)
                .ToList();
            values[sourceField + ".Count"] = entries.Count.ToString(CultureInfo.InvariantCulture);
            values[sourceField + ".TargetLanguage"] = "English";

            for (var entryIndex = 0; entryIndex < entries.Count; entryIndex++)
            {
                values[sourceField + "[" + entryIndex.ToString(CultureInfo.InvariantCulture) + "].Language"] = entries[entryIndex].Language;
                values[sourceField + "[" + entryIndex.ToString(CultureInfo.InvariantCulture) + "].String"] = entries[entryIndex].Value;
            }
        }
    }

    private static int GetSpriggitLanguageOrder(LocalizedStringDTO localizedString)
    {
        var order = localizedString.Game switch
        {
            Core.Enums.SupportedGame.Starfield => StarfieldSpriggitLanguageOrder,
            Core.Enums.SupportedGame.Fallout4 => Fallout4SpriggitLanguageOrder,
            _ => SkyrimSpriggitLanguageOrder
        };

        return order.TryGetValue(localizedString.Language, out var index)
            ? index
            : int.MaxValue;
    }

    private static int GetSpriggitLanguageOrder(string language, SupportedGame? game)
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
