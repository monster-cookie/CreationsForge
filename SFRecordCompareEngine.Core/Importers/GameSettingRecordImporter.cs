using System.Globalization;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Importers.Interfaces;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Core.Services;

namespace SFRecordCompareEngine.Core.Importers;

public class GameSettingRecordImporter(IGameSettingRepository gameSettingRepository) : ITypedRecordDetailImporter
{
    public string RecordType => RecordTypeImportCatalog.GameSettingRecordType;
    public string TableName => RecordTypeImportCatalog.GameSettingRecordType;

    public void Import(IDatabase database, string modKey, string formId, RecordEnumerationDTO record, string importedAtUtc)
    {
        var source = record.Record;
        gameSettingRepository.Upsert(database, new GameSettingDTO
        {
            ModKey = modKey,
            FormID = formId,
            SettingType = RecordHeaderMapper.GetStringValue(source, "SettingType"),
            TitleString = RecordHeaderMapper.GetStringValue(source, "TitleString"),
            Data = GetTextValue(source, "Data"),
            RawData = GetDoubleValue(source, "RawData"),
            XALG = RecordHeaderMapper.GetNullableIntValue(source, "XALG"),
            IsCompressed = GetBooleanIntValue(source, "IsCompressed"),
            IsDeleted = GetBooleanIntValue(source, "IsDeleted"),
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
