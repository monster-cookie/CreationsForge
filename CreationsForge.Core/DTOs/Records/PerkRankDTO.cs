using CreationsForge.Core.DTOs.Plugins;

namespace CreationsForge.Core.DTOs.Records;

public class PerkRankDTO
{
    public required ModKeyDTO ModKey { get; set; }

    public required FormKeyDTO FormKey { get; set; }

    public int RankIndex { get; set; }

    public TranslatedStringDTO? Description { get; set; }

    public FormKeyDTO? UnknownStaticFormKey { get; set; }

    public int ConditionCount { get; set; }

    public int ActivityCount { get; set; }

    public DateTime ImportedAtUTC { get; set; }

    public IList<PerkRankEffectDTO> Effects { get; set; } = new List<PerkRankEffectDTO>();
}
