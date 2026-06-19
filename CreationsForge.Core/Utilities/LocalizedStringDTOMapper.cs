using System.Reflection;
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

    private static IReadOnlyList<LocalizedStringDTO> CreateLocalizedStrings(RecordDTO dto, string sourceField, object? value)
    {
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
}
