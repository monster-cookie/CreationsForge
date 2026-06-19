namespace CreationsForge.Core.DTOs.Records;

public class ConditionFormDTO : RecordDTO, Interfaces.IHasConditionsRecordDTO
{
    public int? Version2 { get; set; }

    public IList<ConditionFormConditionDTO> Conditions { get; set; } = new List<ConditionFormConditionDTO>();
}
