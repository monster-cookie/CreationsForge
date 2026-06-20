namespace CreationsForge.Core.DTOs.Records;

public class ConditionFormDTO : RecordDTO, Interfaces.IHasConditionsRecordDTO
{
    public IList<ConditionFormConditionDTO> Conditions { get; set; } = new List<ConditionFormConditionDTO>();
}
