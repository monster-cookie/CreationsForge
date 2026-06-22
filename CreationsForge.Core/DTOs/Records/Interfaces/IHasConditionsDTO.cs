namespace CreationsForge.Core.DTOs.Records.Interfaces;

public interface IHasConditionsDTO
{
    IList<ConditionFormConditionDTO> Conditions { get; set; }
}
