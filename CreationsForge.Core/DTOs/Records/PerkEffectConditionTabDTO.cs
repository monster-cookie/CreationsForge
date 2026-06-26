using CreationsForge.Core.DTOs.Plugins;

namespace CreationsForge.Core.DTOs.Records;

public class PerkEffectConditionTabDTO
{
    public required ModKeyDTO ModKey { get; set; }

    public required FormKeyDTO FormKey { get; set; }

    public int? RankIndex { get; set; }

    public int EffectIndex { get; set; }

    public int ConditionTabIndex { get; set; }

    public int? RunOnTabIndex { get; set; }

    public int ConditionCount { get; set; }

    public IList<ConditionFormConditionDTO> Conditions { get; set; } = new List<ConditionFormConditionDTO>();

    public DateTime ImportedAtUTC { get; set; }
}
