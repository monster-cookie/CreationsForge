using CreationsForge.Core.DTOs.Plugins;

namespace CreationsForge.Core.DTOs.Records;

public class PerkRankActivityDTO
{
    public required ModKeyDTO ModKey { get; set; }

    public required FormKeyDTO FormKey { get; set; }

    public int RankIndex { get; set; }

    public int ActivityIndex { get; set; }

    public string? ATAN { get; set; }

    public TranslatedStringDTO? Name { get; set; }

    public TranslatedStringDTO? Description { get; set; }

    public string? ANAM { get; set; }

    public string? Configuration { get; set; }

    public DateTime ImportedAtUTC { get; set; }

    public IList<PerkRankActivityProgressionEvaluatorDTO> ProgressionEvalutor { get; set; } = new List<PerkRankActivityProgressionEvaluatorDTO>();
}
