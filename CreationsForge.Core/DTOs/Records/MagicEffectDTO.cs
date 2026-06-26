using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records.Interfaces;

namespace CreationsForge.Core.DTOs.Records;

public class MagicEffectDTO : RecordDTO, IHasScriptingAdaptersDTO, IKeywords, ISounds, IHasConditionsDTO
{
    public TranslatedStringDTO? Name { get; set; }

    public TranslatedStringDTO? Description { get; set; }

    public required string Flags { get; set; }

    public string? CastType { get; set; }

    public string? TargetType { get; set; }

    public string? CastingSoundLevel { get; set; }

    public string? DualCastScale { get; set; }

    public string? Unknown1 { get; set; }

    public string? BaseCost { get; set; }

    public string? MagicSkill { get; set; }

    public FormKeyDTO? CastingLightFormKey { get; set; }

    public FormKeyDTO? MenuDisplayObjectFormKey { get; set; }

    public int? MinimumSkillLevel { get; set; }

    public string? SkillUsageMultiplier { get; set; }

    public string? SpellmakingCastingTime { get; set; }

    public string? TaperWeight { get; set; }

    public string? SecondActorValue { get; set; }

    public string? SecondActorValueWeight { get; set; }

    public int? SpellmakingArea { get; set; }

    public FormKeyDTO? EnchantShaderFormKey { get; set; }

    public FormKeyDTO? ActorValue2FormKey { get; set; }

    public FormKeyDTO? ResistValueFormKey { get; set; }

    public string? ResistValue { get; set; }

    public FormKeyDTO? PerkToApplyFormKey { get; set; }

    public FormKeyDTO? EquipAbilityFormKey { get; set; }

    public FormKeyDTO? ExplosionFormKey { get; set; }

    public FormKeyDTO? CastingArtFormKey { get; set; }

    public FormKeyDTO? HitEffectArtFormKey { get; set; }

    public FormKeyDTO? HitShaderFormKey { get; set; }

    public FormKeyDTO? ImageSpaceModifierFormKey { get; set; }

    public FormKeyDTO? ImpactDataFormKey { get; set; }

    public FormKeyDTO? ProjectileFormKey { get; set; }

    public string? Archetype { get; set; }

    public string? ArchetypeActorValue { get; set; }

    public FormKeyDTO? ArchetypeAssociationFormKey { get; set; }

    public float? UnknownFloat1 { get; set; }

    public float? UnknownFloat3 { get; set; }

    public float? UnknownFloat4 { get; set; }

    public int? UnknownInt2 { get; set; }

    public long? UnknownInt3 { get; set; }

    public string? Unknown { get; set; }

    public string? Unknown2 { get; set; }

    public string? DataTypeState { get; set; }

    public IList<KeywordMappingDTO> Keywords { get; set; } = new List<KeywordMappingDTO>();

    public IList<SoundMappingDTO> Sounds { get; set; } = new List<SoundMappingDTO>();

    public IList<ScriptingAdapterDTO> ScriptingAdapters { get; set; } = new List<ScriptingAdapterDTO>();

    public IList<ConditionFormConditionDTO> Conditions { get; set; } = new List<ConditionFormConditionDTO>();
}
