using System.Globalization;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Utilities;
using Mutagen.Bethesda.Strings;

namespace CreationsForge.Core.DTOs.Records;

public class GameSettingDataDTO
{
    public GameSettingDataType DataType { get; set; }

    public bool? Boolean { get; set; }

    public double? Float { get; set; }

    public int? Integer { get; set; }

    public TranslatedStringDTO? String { get; set; }

    public uint? UnsignedInteger { get; set; }

    public string MutagenObjectType => GetMutagenObjectType(DataType);

    public static string GetMutagenObjectType(GameSettingDataType dataType)
    {
        return dataType switch
        {
            GameSettingDataType.Boolean => "GameSettingBool",
            GameSettingDataType.Float => "GameSettingFloat",
            GameSettingDataType.Integer => "GameSettingInt",
            GameSettingDataType.String => "GameSettingString",
            GameSettingDataType.UnsignedInteger => "GameSettingUInt",
            _ => dataType.ToString()
        };
    }

    public string? GetScalarDisplayValue()
    {
        return GetScalarDisplayValue(DataType);
    }

    public string? GetScalarDisplayValue(GameSettingDataType dataType)
    {
        return dataType switch
        {
            GameSettingDataType.Boolean => Boolean?.ToString(CultureInfo.InvariantCulture),
            GameSettingDataType.Float => Float?.ToString(CultureInfo.InvariantCulture),
            GameSettingDataType.Integer => Integer?.ToString(CultureInfo.InvariantCulture),
            GameSettingDataType.UnsignedInteger => UnsignedInteger?.ToString(CultureInfo.InvariantCulture),
            _ => null
        };
    }

    public string? GetLocalizedStringDisplayValue(Language language)
    {
        return LocalizedStringDTOMapper.GetLocalizedText(String, language);
    }
}
