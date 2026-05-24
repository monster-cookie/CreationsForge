using System.Collections;
using System.Globalization;
using Mutagen.Bethesda.Plugins.Records;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Importers.Interfaces;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Core.Services;

namespace SFRecordCompareEngine.Core.Importers;

public class CellRecordImporter(ICellRepository cellRepository) : ITypedRecordDetailImporter
{
    public string RecordType => "Cell";
    public string TableName => "Cell";

    public void Import(IDatabase database, string modKey, string formId, RecordEnumerationDTO record, string importedAtUtc)
    {
        var source = record.Record;
        cellRepository.Upsert(database, new CellDTO
        {
            ModKey = modKey,
            FormID = formId,
            Name = GetTextValue(source, "Name"),
            Flags = GetTextValue(source, "Flags"),
            MajorFlags = GetTextValue(source, "MajorFlags"),
            LightingTemplateFormKey = ExtractReferenceText(RecordHeaderMapper.GetPropertyValue(source, "LightingTemplate")),
            ImageSpaceFormKey = ExtractReferenceText(RecordHeaderMapper.GetPropertyValue(source, "ImageSpace")),
            LocationFormKey = ExtractReferenceText(RecordHeaderMapper.GetPropertyValue(source, "Location")),
            WaterFormKey = ExtractReferenceText(RecordHeaderMapper.GetPropertyValue(source, "Water")),
            WaterHeight = GetTextValue(source, "WaterHeight"),
            IsLinkedRefTransient = GetBooleanIntValue(source, "IsLinkedRefTransient"),
            ImportedAtUtc = importedAtUtc
        });

        var locations = record.CellGroupLocations
            .Select((location, index) =>
            {
                location.ModKey = modKey;
                location.CellFormID = formId;
                location.LocationIndex = index;
                location.ImportedAtUtc = importedAtUtc;
                return location;
            })
            .ToList();
        cellRepository.ReplaceGroupLocations(database, modKey, formId, locations);

        var placedRecords = GetPlacedRecords(source, modKey, formId, importedAtUtc);
        cellRepository.ReplacePlacedRecords(database, modKey, formId, placedRecords);
    }

    private static IList<CellPlacedRecordDTO> GetPlacedRecords(object source, string modKey, string formId, string importedAtUtc)
    {
        var placedRecords = new List<CellPlacedRecordDTO>();
        AddPlacedRecords(placedRecords, source, modKey, formId, "Persistent", importedAtUtc);
        AddPlacedRecords(placedRecords, source, modKey, formId, "Temporary", importedAtUtc);
        return placedRecords;
    }

    private static void AddPlacedRecords(ICollection<CellPlacedRecordDTO> target, object source, string modKey, string formId, string placementGroup, string importedAtUtc)
    {
        if (RecordHeaderMapper.GetPropertyValue(source, placementGroup) is not IEnumerable records) return;

        foreach (var item in records.Cast<object?>().Select((record, index) => new { record, index }))
        {
            if (item.record is null) continue;

            target.Add(new CellPlacedRecordDTO
            {
                ModKey = modKey,
                CellFormID = formId,
                PlacementGroup = placementGroup,
                ItemIndex = item.index,
                PlacedFormKey = ExtractReferenceText(RecordHeaderMapper.GetPropertyValue(item.record, "FormKey")),
                BaseFormKey = ExtractReferenceText(RecordHeaderMapper.GetPropertyValue(item.record, "Base")),
                EditorID = RecordHeaderMapper.GetStringValue(item.record, "EditorID"),
                Position = GetTextValue(item.record, "Position"),
                Rotation = GetTextValue(item.record, "Rotation"),
                IsDeleted = GetBooleanIntValue(item.record, "IsDeleted"),
                ImportedAtUtc = importedAtUtc
            });
        }
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
