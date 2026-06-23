using CreationsForge.Core.DTOs.Plugins;

namespace CreationsForge.Core.DTOs.Records;

public class PerkEffectDTO
{
    public required ModKeyDTO ModKey { get; set; }

    public required FormKeyDTO FormKey { get; set; }

    public int EffectIndex { get; set; }

    public required string MutagenObjectType { get; set; }

    public int? Rank { get; set; }

    public int? Priority { get; set; }

    public int? PerkEntryId { get; set; }

    public string? Flags { get; set; }

    public TranslatedStringDTO? ButtonLabel { get; set; }

    public int? ConditionCount { get; set; }

    public string? EntryPoint { get; set; }

    public int? PerkConditionTabCount { get; set; }

    public string? Modification { get; set; }

    public double? Value { get; set; }

    public string? ActorValue { get; set; }

    public string? Spell { get; set; }

    public string? Quest { get; set; }

    public int? Stage { get; set; }

    public IList<PerkEffectConditionTabDTO> Conditions { get; set; } = new List<PerkEffectConditionTabDTO>();

    public DateTime ImportedAtUTC { get; set; }
}
