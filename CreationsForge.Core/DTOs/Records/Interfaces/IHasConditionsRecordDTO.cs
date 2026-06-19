namespace CreationsForge.Core.DTOs.Records.Interfaces;

public interface IHasConditionsRecordDTO
{
    IList<ConditionFormConditionDTO> Conditions { get; set; }
}
