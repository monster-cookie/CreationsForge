using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records.Interfaces;

namespace CreationsForge.Core.DTOs.Records;

public class MagicEffectDTO : RecordDTO, IHasScriptingAdaptersRecordDTO, IKeywords, ISounds
{
    public TranslatedStringDTO? Name { get; set; }

    public TranslatedStringDTO? Description { get; set; }

    public required string Flags { get; set; }

    public string? CastType { get; set; }

    public string? TargetType { get; set; }

    public FormKeyDTO? ActorValue2FormKey { get; set; }

    public FormKeyDTO? ResistValueFormKey { get; set; }

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

    public float? UnknownFloat3 { get; set; }

    public int? UnknownInt2 { get; set; }

    public string? Unknown { get; set; }

    public string? Unknown2 { get; set; }

    public string? DataTypeState { get; set; }

    public IList<KeywordMappingDTO> Keywords { get; set; } = new List<KeywordMappingDTO>();

    public IList<SoundMappingDTO> Sounds { get; set; } = new List<SoundMappingDTO>();

    public IList<ScriptingAdapterDTO> ScriptingAdapters { get; set; } = new List<ScriptingAdapterDTO>();
}
