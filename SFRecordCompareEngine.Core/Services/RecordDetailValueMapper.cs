using System.Collections;
using System.Globalization;
using Mutagen.Bethesda.Plugins.Records;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Services;

internal static class RecordDetailValueMapper
{
    public static string? GetTextValue(object source, string propertyName)
    {
        var value = RecordHeaderMapper.GetPropertyValue(source, propertyName);
        return value switch
        {
            null => null,
            IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
            _ => value.ToString()
        };
    }

    public static string? GetNestedTextValue(object source, string propertyName, string nestedPropertyName)
    {
        var value = RecordHeaderMapper.GetPropertyValue(source, propertyName);
        return value is null ? null : GetTextValue(value, nestedPropertyName);
    }

    public static int? GetNestedIntValue(object source, string propertyName, string nestedPropertyName)
    {
        var value = RecordHeaderMapper.GetPropertyValue(source, propertyName);
        return value is null ? null : GetIntValue(value, nestedPropertyName);
    }

    public static double? GetNestedDoubleValue(object source, string propertyName, string nestedPropertyName)
    {
        var value = RecordHeaderMapper.GetPropertyValue(source, propertyName);
        return value is null ? null : GetDoubleValue(value, nestedPropertyName);
    }

    public static int? GetNestedBooleanIntValue(object source, string propertyName, string nestedPropertyName)
    {
        var value = RecordHeaderMapper.GetPropertyValue(source, propertyName);
        return value is null ? null : GetBooleanIntValue(value, nestedPropertyName);
    }

    public static string? GetCollectionTextValue(object source, string propertyName)
    {
        var value = RecordHeaderMapper.GetPropertyValue(source, propertyName);
        return value switch
        {
            null => null,
            string text => text,
            IEnumerable enumerable => string.Join(", ", enumerable.Cast<object?>().Where(item => item is not null).Select(item => item!.ToString())),
            _ => value.ToString()
        };
    }

    public static IList<RecordKeywordDTO> GetKeywords(object source, string modKey, string formId, string importedAtUtc)
    {
        if (RecordHeaderMapper.GetPropertyValue(source, "Keywords") is not IEnumerable keywords) return [];

        return keywords.Cast<object?>()
            .Select(ExtractReferenceText)
            .Where(keywordFormKey => !string.IsNullOrWhiteSpace(keywordFormKey))
            .Select((keywordFormKey, index) => new RecordKeywordDTO
            {
                ModKey = modKey,
                FormID = formId,
                ItemIndex = index,
                KeywordFormKey = keywordFormKey!,
                ImportedAtUtc = importedAtUtc
            })
            .ToList();
    }

    public static string? ExtractReferenceText(object? value)
    {
        if (value is null) return null;

        var rawValue = value is IFormKeyGetter formKeyGetter
            ? formKeyGetter.FormKey.ToString()
            : RecordHeaderMapper.GetStringValue(value, "FormKey") ?? value.ToString();
        if (string.IsNullOrWhiteSpace(rawValue)) return null;

        var normalizedValue = FormKeyTextNormalizer.NormalizeReferenceValue(rawValue);
        return normalizedValue.Equals("Null", StringComparison.OrdinalIgnoreCase)
               || normalizedValue.Equals("NullReference", StringComparison.OrdinalIgnoreCase)
            ? null
            : normalizedValue;
    }

    private static int? GetIntValue(object source, string propertyName)
    {
        var value = RecordHeaderMapper.GetPropertyValue(source, propertyName);
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

    private static double? GetDoubleValue(object source, string propertyName)
    {
        var value = RecordHeaderMapper.GetPropertyValue(source, propertyName);
        if (value is null) return null;

        try
        {
            return Convert.ToDouble(value, CultureInfo.InvariantCulture);
        }
        catch
        {
            return null;
        }
    }

    private static int? GetBooleanIntValue(object source, string propertyName)
    {
        var value = RecordHeaderMapper.GetPropertyValue(source, propertyName);
        if (value is null) return null;

        try
        {
            return Convert.ToBoolean(value, CultureInfo.InvariantCulture) ? 1 : 0;
        }
        catch
        {
            return null;
        }
    }
}
