namespace CreationsForge.Core.DTOs.Records;

public class ConditionFormDTO : RecordDTO, Interfaces.IHasConditionsDTO
{
    public IList<ConditionFormConditionDTO> Conditions { get; set; } = new List<ConditionFormConditionDTO>();
}
