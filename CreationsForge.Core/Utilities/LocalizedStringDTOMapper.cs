using System.Reflection;
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using Mutagen.Bethesda.Strings;

namespace CreationsForge.Core.Utilities;

public static class LocalizedStringDTOMapper
{
    public static T AddLocalizedStrings<T>(T dto, object source)
        where T : RecordDTO
    {
        foreach (var property in dto.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                     .Where(property => property.PropertyType == typeof(string)))
        {
            AddLocalizedStringField(dto, source, property.Name);
        }

        foreach (var localizedString in CreateLocalizedStrings(dto))
        {
            dto.LocalizedStrings.Add(localizedString);
        }

        return dto;
    }

    public static void AddLocalizedStringField(RecordDTO dto, object source, string sourceField)
    {
        var value = source.GetType().GetProperty(sourceField)?.GetValue(source);
        foreach (var localizedString in CreateLocalizedStrings(dto, sourceField, value))
        {
            dto.LocalizedStrings.Add(localizedString);
        }
    }

    public static TranslatedStringDTO? ToTranslatedStringDTO(object? value)
    {
        if (value is null)
        {
            return null;
        }

        if (value is TranslatedStringDTO translatedStringDTO)
        {
            return translatedStringDTO;
        }

        if (value is not ITranslatedStringGetter translatedString)
        {
            return FromEnglish(value.ToString());
        }

        var strings = Enum.GetValues<Language>()
            .Select(language => new
            {
                Language = language.ToString(),
                Value = GetLocalizedText(value, language)
            })
            .Where(item => !string.IsNullOrEmpty(item.Value))
            .GroupBy(item => item.Language, StringComparer.OrdinalIgnoreCase)
            .Select(group => group.First())
            .Select(item => new TranslatedStringValueDTO
            {
                Language = item.Language,
                String = item.Value ?? string.Empty
            })
            .ToList();

        return strings.Count == 0
            ? null
            : new TranslatedStringDTO { Strings = strings };
    }

    public static TranslatedStringDTO? FromEnglish(string? value)
    {
        return string.IsNullOrEmpty(value)
            ? null
            : new TranslatedStringDTO
            {
                Strings =
                [
                    new TranslatedStringValueDTO
                    {
                        Language = "English",
                        String = value
                    }
                ]
            };
    }

    public static string? GetEnglishText(TranslatedStringDTO? translatedString)
    {
        return GetLocalizedText(translatedString, "English");
    }

    public static string? GetLocalizedText(TranslatedStringDTO? translatedString, string language)
    {
        if (translatedString is null)
        {
            return null;
        }

        return translatedString.Strings.FirstOrDefault(value => string.Equals(value.Language, language, StringComparison.OrdinalIgnoreCase))?.String ??
               translatedString.Strings.FirstOrDefault(value => string.Equals(value.Language, "English", StringComparison.OrdinalIgnoreCase))?.String ??
               translatedString.Strings.FirstOrDefault()?.String;
    }

    public static string? GetLocalizedText(object? value, Language language)
    {
        if (value is null)
        {
            return null;
        }

        if (value is not ITranslatedStringGetter translatedString)
        {
            return value.ToString();
        }

        return translatedString.TryLookup(language, out var localizedValue)
            ? localizedValue
            : null;
    }

    private static IReadOnlyList<LocalizedStringDTO> CreateLocalizedStrings(RecordDTO dto)
    {
        return EnumerateTranslatedStringFields(dto, string.Empty)
            .SelectMany(field => CreateLocalizedStrings(dto, field.SourceField, field.Value))
            .ToList();
    }

    private static IReadOnlyList<LocalizedStringDTO> CreateLocalizedStrings(RecordDTO dto, string sourceField, object? value)
    {
        if (value is TranslatedStringDTO translatedStringDTO)
        {
            return translatedStringDTO.Strings
                .Where(item => !string.IsNullOrEmpty(item.String))
                .GroupBy(item => item.Language, StringComparer.OrdinalIgnoreCase)
                .Select(group => group.First())
                .Select(item => new LocalizedStringDTO
                {
                    Game = dto.Game,
                    ModKey = dto.ModKey,
                    RecordType = string.Empty,
                    FormKey = dto.FormKey,
                    SourceField = sourceField,
                    Language = item.Language,
                    Value = item.String,
                    ImportedAtUTC = dto.ImportedAtUTC
                })
                .ToList();
        }

        if (value is not ITranslatedStringGetter)
        {
            return [];
        }

        return Enum.GetValues<Language>()
            .Select(language => new
            {
                Language = language.ToString(),
                Value = GetLocalizedText(value, language)
            })
            .Where(item => !string.IsNullOrEmpty(item.Value))
            .GroupBy(item => item.Language, StringComparer.OrdinalIgnoreCase)
            .Select(group => group.First())
            .Select(item => new LocalizedStringDTO
            {
                Game = dto.Game,
                ModKey = dto.ModKey,
                RecordType = string.Empty,
                FormKey = dto.FormKey,
                SourceField = sourceField,
                Language = item.Language,
                Value = item.Value ?? string.Empty,
                ImportedAtUTC = dto.ImportedAtUTC
            })
            .ToList();
    }

    private static IEnumerable<(string SourceField, TranslatedStringDTO Value)> EnumerateTranslatedStringFields(object? value, string path)
    {
        if (value is null)
        {
            yield break;
        }

        if (value is TranslatedStringDTO translatedString)
        {
            yield return (path, translatedString);
            yield break;
        }

        if (IsScalar(value) || value is ModKeyDTO or FormKeyDTO or LocalizedStringDTO)
        {
            yield break;
        }

        if (value is System.Collections.IEnumerable enumerable && value is not string)
        {
            var itemIndex = 0;
            foreach (var item in enumerable)
            {
                var itemPath = path + "[" + itemIndex.ToString(System.Globalization.CultureInfo.InvariantCulture) + "]";
                foreach (var translatedStringField in EnumerateTranslatedStringFields(item, itemPath))
                {
                    yield return translatedStringField;
                }

                itemIndex++;
            }

            yield break;
        }

        foreach (var property in value.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                     .Where(property => property.GetIndexParameters().Length == 0)
                     .Where(property => !string.Equals(property.Name, nameof(RecordDTO.LocalizedStrings), StringComparison.Ordinal)))
        {
            var propertyPath = string.IsNullOrEmpty(path)
                ? property.Name
                : path + "." + property.Name;
            foreach (var translatedStringField in EnumerateTranslatedStringFields(property.GetValue(value), propertyPath))
            {
                yield return translatedStringField;
            }
        }
    }

    private static bool IsScalar(object value)
    {
        var type = value.GetType();
        return type.IsPrimitive ||
               type.IsEnum ||
               value is string or decimal or DateTime or Guid;
    }
}
