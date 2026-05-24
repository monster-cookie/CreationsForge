using System.Globalization;
using Mutagen.Bethesda.Plugins.Records;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Importers.Interfaces;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Core.Services;

namespace SFRecordCompareEngine.Core.Importers;

public class WorldspaceRecordImporter(IWorldspaceRepository worldspaceRepository) : ITypedRecordDetailImporter
{
    public string RecordType => "Worldspace";
    public string TableName => "Worldspace";

    public void Import(IDatabase database, string modKey, string formId, RecordEnumerationDTO record, string importedAtUtc)
    {
        var source = record.Record;
        worldspaceRepository.Upsert(database, new WorldspaceDTO
        {
            ModKey = modKey,
            FormID = formId,
            Name = GetTextValue(source, "Name"),
            ParentWorldspaceFormKey = ExtractReferenceText(RecordHeaderMapper.GetPropertyValue(source, "ParentWorldspace")),
            ClimateFormKey = ExtractReferenceText(RecordHeaderMapper.GetPropertyValue(source, "Climate")),
            WaterFormKey = ExtractReferenceText(RecordHeaderMapper.GetPropertyValue(source, "Water")),
            TopCellFormKey = ExtractReferenceText(RecordHeaderMapper.GetPropertyValue(source, "TopCell")),
            WorldMapCellOffset = GetTextValue(source, "WorldMapCellOffset"),
            WorldMapOffsetScale = GetTextValue(source, "WorldMapOffsetScale"),
            ImportedAtUtc = importedAtUtc
        });
    }

    private static string? GetTextValue(object source, string propertyName)
    {
        var value = RecordHeaderMapper.GetPropertyValue(source, propertyName);
        return value switch
        {
            null => null,
            IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
            _ => value.ToString()
        };
    }

    private static string? ExtractReferenceText(object? value)
    {
        if (value is null) return null;

        var rawValue = value is IFormKeyGetter formKeyGetter
            ? formKeyGetter.FormKey.ToString()
            : value.ToString();
        if (string.IsNullOrWhiteSpace(rawValue)) return null;

        var normalizedValue = FormKeyTextNormalizer.NormalizeReferenceValue(rawValue);
        return normalizedValue.Equals("Null", StringComparison.OrdinalIgnoreCase)
               || normalizedValue.Equals("NullReference", StringComparison.OrdinalIgnoreCase)
            ? null
            : normalizedValue;
    }
}
