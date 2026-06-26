using CreationsForge.Core.DTOs.Records.Interfaces;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.DTOs.Records;

public class GameSettingDTO : RecordDTO, IHasTranslatedFields
{
    public GameSettingDataType DataType { get; set; }

    public string MutagenObjectType => GameSettingDataDTO.GetMutagenObjectType(DataType);

    public GameSettingDataDTO Data { get; set; } = new();

    public IEnumerable<TranslatedFieldDTO> GetTranslatedFields()
    {
        if (DataType == GameSettingDataType.String)
        {
            yield return new TranslatedFieldDTO { SourceField = "Data", Value = Data.String };
        }
    }
}
