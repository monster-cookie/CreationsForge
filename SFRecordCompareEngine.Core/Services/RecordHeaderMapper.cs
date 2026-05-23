using System.Globalization;
using System.Reflection;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Services;

public static class RecordHeaderMapper
{
    public static RecordHeaderDTO Map(PluginMetadataDTO plugin, string recordType, object record, string importedAtUtc)
    {
        var formKey = GetStringValue(record, "FormKey")
                      ?? throw new InvalidOperationException($"{recordType} record in {plugin.ModKey} did not expose FormKey.");
        formKey = FormKeyTextNormalizer.NormalizeReferenceValue(formKey);

        return new RecordHeaderDTO
        {
            ModKey = plugin.ModKey,
            FormID = FormIdNormalizer.NormalizeFromFormKey(formKey),
            RecordType = recordType,
            FormKey = formKey,
            EditorID = GetStringValue(record, "EditorID"),
            PluginFileName = plugin.PluginFileName,
            FormVersion = GetNullableIntValue(record, "FormVersion"),
            StarfieldMajorRecordFlags = GetNullableIntValue(record, "StarfieldMajorRecordFlags"),
            Version2 = GetNullableIntValue(record, "Version2"),
            VersionControl = GetStringValue(record, "VersionControl"),
            ImportedAtUtc = importedAtUtc
        };
    }

    public static object? GetPropertyValue(object source, string propertyName)
    {
        return source.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public)?.GetValue(source);
    }

    public static string? GetStringValue(object source, string propertyName)
    {
        return GetPropertyValue(source, propertyName)?.ToString();
    }

    public static int? GetNullableIntValue(object source, string propertyName)
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
}
