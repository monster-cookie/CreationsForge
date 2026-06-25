using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records.Interfaces;
using CreationsForge.Core.DTOs.Records.Metadata;

namespace CreationsForge.Core.DTOs.Records;

/// <summary>
/// Represents an NPC record and the first-class Spriggit-visible fields imported for validation and comparison.
/// </summary>
public class NPCDTO : RecordDTO, IHasScriptingAdaptersDTO, IKeywords, ISounds
{
    /// <summary>
    /// Gets or sets whether Spriggit reports the record as compressed.
    /// </summary>
    public bool? IsCompressed { get; set; }

    /// <summary>
    /// Gets or sets the object bounds first vector when the game exposes bounds for the NPC.
    /// </summary>
    public string? ObjectBoundsFirst { get; set; }

    /// <summary>
    /// Gets or sets the object bounds second vector when the game exposes bounds for the NPC.
    /// </summary>
    public string? ObjectBoundsSecond { get; set; }

    /// <summary>
    /// Gets or sets the translated display name for the NPC.
    /// </summary>
    public TranslatedStringDTO? Name { get; set; }

    /// <summary>
    /// Gets or sets the translated short display name for the NPC.
    /// </summary>
    public TranslatedStringDTO? ShortName { get; set; }

    /// <summary>
    /// Gets or sets the translated long display name for the NPC.
    /// </summary>
    public TranslatedStringDTO? LongName { get; set; }

    /// <summary>
    /// Gets or sets the NPC flag list as Spriggit names.
    /// </summary>
    public string? Flags { get; set; }

    /// <summary>
    /// Gets or sets the game-specific major flag list as Spriggit names or hexadecimal values.
    /// </summary>
    public string? MajorFlags { get; set; }

    /// <summary>
    /// Gets or sets the NPC level data exported by Spriggit.
    /// </summary>
    public NPCLevelDTO? Level { get; set; }

    /// <summary>
    /// Gets or sets Skyrim configuration fields exported under the Spriggit <c>Configuration</c> object.
    /// </summary>
    public NPCConfigurationDTO? Configuration { get; set; }

    /// <summary>
    /// Gets or sets the base disposition value.
    /// </summary>
    public int DispositionBase { get; set; }

    /// <summary>
    /// Gets or sets the actor aggression setting.
    /// </summary>
    public required string Aggression { get; set; }

    /// <summary>
    /// Gets or sets the actor confidence setting.
    /// </summary>
    public required string Confidence { get; set; }

    /// <summary>
    /// Gets or sets the actor energy level.
    /// </summary>
    public int EnergyLevel { get; set; }

    /// <summary>
    /// Gets or sets the actor responsibility setting.
    /// </summary>
    public required string Responsibility { get; set; }

    /// <summary>
    /// Gets or sets the actor assistance setting.
    /// </summary>
    public required string Assistance { get; set; }

    /// <summary>
    /// Gets or sets the actor mood, or <c>null</c> when Spriggit omits it.
    /// </summary>
    public string? Mood { get; set; }

    /// <summary>
    /// Gets or sets the geared-up weapons count from player skills data.
    /// </summary>
    public int GearedUpWeapons { get; set; }

    /// <summary>
    /// Gets or sets the minimum actor height.
    /// </summary>
    [NumericDisplayPrecision(3)]
    public double HeightMin { get; set; }

    /// <summary>
    /// Gets or sets the maximum actor height.
    /// </summary>
    [NumericDisplayPrecision(3)]
    public double HeightMax { get; set; }

    /// <summary>
    /// Gets or sets the scalar actor height used by Skyrim NPC records.
    /// </summary>
    [NumericDisplayPrecision(3)]
    public double? Height { get; set; }

    /// <summary>
    /// Gets or sets the skin tone index, or <c>null</c> when omitted.
    /// </summary>
    public int? SkinToneIndex { get; set; }

    /// <summary>
    /// Gets or sets the Fallout 4 skin reference used for creature, robot, turret, or specialized actor visuals.
    /// </summary>
    public FormKeyDTO? Skin { get; set; }

    /// <summary>
    /// Gets or sets the pronoun value, or <c>null</c> when omitted.
    /// </summary>
    public string? Pronoun { get; set; }

    /// <summary>
    /// Gets or sets the voice type reference.
    /// </summary>
    public FormKeyDTO? VoiceFormKey { get; set; }

    /// <summary>
    /// Gets or sets the race reference.
    /// </summary>
    public FormKeyDTO? RaceFormKey { get; set; }

    /// <summary>
    /// Gets or sets the attack race reference.
    /// </summary>
    public FormKeyDTO? AttackRace { get; set; }

    /// <summary>
    /// Gets or sets the combat override package list reference.
    /// </summary>
    public FormKeyDTO? CombatOverridePackageListFormKey { get; set; }

    /// <summary>
    /// Gets or sets the combat style reference.
    /// </summary>
    public FormKeyDTO? CombatStyleFormKey { get; set; }

    /// <summary>
    /// Gets or sets the default package list reference.
    /// </summary>
    public FormKeyDTO? DefaultPackageListFormKey { get; set; }

    /// <summary>
    /// Gets or sets the crime faction reference.
    /// </summary>
    public FormKeyDTO? CrimeFactionFormKey { get; set; }

    /// <summary>
    /// Gets or sets the NPC class reference.
    /// </summary>
    public FormKeyDTO? Class { get; set; }

    /// <summary>
    /// Gets or sets the death item reference.
    /// </summary>
    public FormKeyDTO? DeathItem { get; set; }

    /// <summary>
    /// Gets or sets the default outfit reference.
    /// </summary>
    public FormKeyDTO? DefaultOutfit { get; set; }

    /// <summary>
    /// Gets or sets the sleeping outfit reference.
    /// </summary>
    public FormKeyDTO? SleepingOutfit { get; set; }

    /// <summary>
    /// Gets or sets the worn armor reference.
    /// </summary>
    public FormKeyDTO? WornArmor { get; set; }

    /// <summary>
    /// Gets or sets the Fallout 4 power armor stand reference used by actors spawned in power armor.
    /// </summary>
    public FormKeyDTO? PowerArmorStand { get; set; }

    /// <summary>
    /// Gets or sets the space outfit reference.
    /// </summary>
    public FormKeyDTO? SpaceOutfit { get; set; }

    /// <summary>
    /// Gets or sets the head texture reference.
    /// </summary>
    public FormKeyDTO? HeadTexture { get; set; }

    /// <summary>
    /// Gets or sets the template actor reference.
    /// </summary>
    public FormKeyDTO? Template { get; set; }

    /// <summary>
    /// Gets or sets the default template actor reference.
    /// </summary>
    public FormKeyDTO? DefaultTemplate { get; set; }

    /// <summary>
    /// Gets or sets the template actor references grouped by template role.
    /// </summary>
    public NPCTemplateActorsDTO? TemplateActors { get; set; }

    /// <summary>
    /// Gets or sets the raw template-actor flag value or flag names.
    /// </summary>
    public string? UseTemplateActors { get; set; }

    /// <summary>
    /// Gets or sets the actor calculated health, or <c>null</c> when omitted.
    /// </summary>
    public int? CalculatedHealth { get; set; }

    /// <summary>
    /// Gets or sets the actor calculated action points, or <c>null</c> when omitted.
    /// </summary>
    public int? CalculatedActionPoints { get; set; }

    /// <summary>
    /// Gets or sets the experience value offset.
    /// </summary>
    public int? XpValueOffset { get; set; }

    /// <summary>
    /// Gets or sets the unknown value exported by Fallout 4 NPC records.
    /// </summary>
    public int? Unknown { get; set; }

    /// <summary>
    /// Gets or sets the unused NPC data value.
    /// </summary>
    public int? Unused { get; set; }

    /// <summary>
    /// Gets or sets the NAM5 value using Spriggit-compatible formatting.
    /// </summary>
    public string? NAM5 { get; set; }

    /// <summary>
    /// Gets or sets the body weight data exported by Spriggit.
    /// </summary>
    public NPCWeightDTO? Weight { get; set; }

    /// <summary>
    /// Gets or sets the actor sound level.
    /// </summary>
    public string? SoundLevel { get; set; }

    /// <summary>
    /// Gets or sets the texture lighting color value.
    /// </summary>
    public string? TextureLighting { get; set; }

    /// <summary>
    /// Gets or sets the hair color value, either as a form key or game-specific enum name.
    /// </summary>
    public string? HairColor { get; set; }

    /// <summary>
    /// Gets or sets the facial hair color value.
    /// </summary>
    public string? FacialHairColor { get; set; }

    /// <summary>
    /// Gets or sets the eyebrow color value.
    /// </summary>
    public string? EyebrowColor { get; set; }

    /// <summary>
    /// Gets or sets the eye color value.
    /// </summary>
    public string? EyeColor { get; set; }

    /// <summary>
    /// Gets or sets Skyrim face morph slider values.
    /// </summary>
    public NPCFaceMorphDTO? FaceMorph { get; set; }

    /// <summary>
    /// Gets or sets Skyrim face part indices.
    /// </summary>
    public NPCFacePartsDTO? FaceParts { get; set; }

    /// <summary>
    /// Gets or sets the formatted body morph region values until a stable first-class schema is added.
    /// </summary>
    public string? BodyMorphRegionValues { get; set; }

    /// <summary>
    /// Gets or sets the formatted object template data until a stable first-class schema is added.
    /// </summary>
    public string? ObjectTemplates { get; set; }

    /// <summary>
    /// Gets or sets the formatted AI data aggregate for diagnostics; individual fields are modeled separately.
    /// </summary>
    public string? AIData { get; set; }

    /// <summary>
    /// Gets or sets actor faction memberships.
    /// </summary>
    public IList<NPCFactionDTO> Factions { get; set; } = new List<NPCFactionDTO>();

    /// <summary>
    /// Gets or sets actor property rows.
    /// </summary>
    public IList<NPCPropertyDTO> Properties { get; set; } = new List<NPCPropertyDTO>();

    /// <summary>
    /// Gets or sets actor inventory item rows.
    /// </summary>
    public IList<NPCItemDTO> Items { get; set; } = new List<NPCItemDTO>();

    /// <summary>
    /// Gets or sets package references assigned directly to the NPC.
    /// </summary>
    public IList<FormKeyDTO> Packages { get; set; } = new List<FormKeyDTO>();

    /// <summary>
    /// Gets or sets perk rows assigned directly to the NPC.
    /// </summary>
    public IList<NPCPerkDTO> Perks { get; set; } = new List<NPCPerkDTO>();

    /// <summary>
    /// Gets or sets Starfield forced location references.
    /// </summary>
    public IList<FormKeyDTO> ForcedLocations { get; set; } = new List<FormKeyDTO>();

    /// <summary>
    /// Gets or sets head part references.
    /// </summary>
    public IList<FormKeyDTO> HeadParts { get; set; } = new List<FormKeyDTO>();

    /// <summary>
    /// Gets or sets actor effect references.
    /// </summary>
    [SpriggitPath("ActorEffect")]
    public IList<FormKeyDTO> ActorEffects { get; set; } = new List<FormKeyDTO>();

    /// <summary>
    /// Gets or sets Fallout 4 morph scalar rows.
    /// </summary>
    public IList<NPCMorphDTO> Morphs { get; set; } = new List<NPCMorphDTO>();

    /// <summary>
    /// Gets or sets simple face morph positions used by Fallout 4.
    /// </summary>
    public IList<NPCFaceMorphPositionDTO> FaceMorphs { get; set; } = new List<NPCFaceMorphPositionDTO>();

    /// <summary>
    /// Gets or sets Starfield face dial slider positions.
    /// </summary>
    public IList<NPCFaceDialPositionDTO> FaceDialPositions { get; set; } = new List<NPCFaceDialPositionDTO>();

    /// <summary>
    /// Gets or sets Starfield nested face morph group rows.
    /// </summary>
    public IList<NPCFaceMorphGroupSetDTO> FaceMorphGroups { get; set; } = new List<NPCFaceMorphGroupSetDTO>();

    /// <summary>
    /// Gets or sets Starfield morph blend rows.
    /// </summary>
    public IList<NPCMorphBlendDTO> MorphBlends { get; set; } = new List<NPCMorphBlendDTO>();

    /// <summary>
    /// Gets or sets Starfield tint rows.
    /// </summary>
    public IList<NPCTintDTO> Tints { get; set; } = new List<NPCTintDTO>();

    /// <summary>
    /// Gets or sets Skyrim tint layer rows.
    /// </summary>
    public IList<NPCTintLayerDTO> TintLayers { get; set; } = new List<NPCTintLayerDTO>();

    /// <summary>
    /// Gets or sets Fallout 4 face tinting layer rows, including per-layer state flags exported by Spriggit.
    /// </summary>
    public IList<NPCFaceTintingLayerDTO> FaceTintingLayers { get; set; } = new List<NPCFaceTintingLayerDTO>();

    /// <summary>
    /// Gets or sets Skyrim player skill values.
    /// </summary>
    public NPCPlayerSkillsDTO? PlayerSkills { get; set; }

    /// <summary>
    /// Gets or sets keyword references assigned to the NPC.
    /// </summary>
    public IList<KeywordMappingDTO> Keywords { get; set; } = new List<KeywordMappingDTO>();

    /// <summary>
    /// Gets or sets named sound references assigned to the NPC.
    /// </summary>
    public IList<SoundMappingDTO> Sounds { get; set; } = new List<SoundMappingDTO>();

    /// <summary>
    /// Gets or sets Papyrus scripting adapter rows assigned to the NPC.
    /// </summary>
    public IList<ScriptingAdapterDTO> ScriptingAdapters { get; set; } = new List<ScriptingAdapterDTO>();
}
