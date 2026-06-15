namespace CreationsForge.Core.DTOs.Records;

public class ConditionFormDTO : RecordDTO
{
    public int? Version2 { get; set; }

    public IList<ConditionFormConditionDTO> Conditions { get; set; } = new List<ConditionFormConditionDTO>();
}
