using CreationsForge.Core.DTOs.Plugins;

namespace CreationsForge.Core.DTOs.Records;

public class PerkRankActivityProgressionEvaluatorDTO
{
    public required ModKeyDTO ModKey { get; set; }

    public required FormKeyDTO FormKey { get; set; }

    public int RankIndex { get; set; }

    public int ActivityIndex { get; set; }

    public int EvaluatorIndex { get; set; }

    public string? Name { get; set; }

    public DateTime ImportedAtUTC { get; set; }

    public IList<ConditionFormConditionDTO> Conditions { get; set; } = new List<ConditionFormConditionDTO>();
}
