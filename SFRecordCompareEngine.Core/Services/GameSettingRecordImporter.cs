using System.Globalization;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine.Core.Services;

public class GameSettingRecordImporter(IGameSettingRepository gameSettingRepository) : ITypedRecordDetailImporter
{
    public string RecordType => RecordTypeImportCatalog.GameSettingRecordType;
    public string TableName => RecordTypeImportCatalog.GameSettingRecordType;

    public void Import(IDatabase database, string modKey, string formId, object record, string importedAtUtc)
    {
        gameSettingRepository.Upsert(database, new GameSettingDTO
        {
            ModKey = modKey,
            FormID = formId,
            SettingType = RecordHeaderMapper.GetStringValue(record, "SettingType"),
            TitleString = RecordHeaderMapper.GetStringValue(record, "TitleString"),
            Data = GetTextValue(record, "Data"),
            RawData = GetDoubleValue(record, "RawData"),
            XALG = RecordHeaderMapper.GetNullableIntValue(record, "XALG"),
            IsCompressed = GetBooleanIntValue(record, "IsCompressed"),
            IsDeleted = GetBooleanIntValue(record, "IsDeleted"),
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
