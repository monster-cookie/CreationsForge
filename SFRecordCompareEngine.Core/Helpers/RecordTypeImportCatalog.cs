namespace SFRecordCompareEngine.Core.Helpers;

public static class RecordTypeImportCatalog
{
    public const string FormListRecordType = "FormList";
    public const string GameSettingRecordType = "GameSetting";

    public static readonly IReadOnlyList<string> SupportedRecordTypes =
    [
        "AcousticSpace", "ActionRecord", "Activator", "ActorValueInformation", "ActorValueModulation", "AddonNode", "AffinityEvent", "AimAssistModel",
        "AimAssistPose", "AimModel", "AimOpticalSightMarker", "AmbienceSet", "Ammunition", "AnimatedObject", "AnimationSoundTagSet", "AObjectModification",
        "APlacedTrap", "Armor", "ArmorAddon", "ArtObject", "AStoryManagerNode", "Atmosphere", "AttractionRule", "AudioOcclusionPrimitive", "BendableSpline",
        "Biome", "BiomeMarker", "BodyPartData", "BoneModifier", "Book", "CameraPath", "CameraShot", "Cell", "Challenge", "Class", "Climate", "Clouds",
        "CollisionLayer", "ColorRecord", "CombatStyle", "ConditionRecord", "ConstructibleObject", "Container", "Curve3D", "CurveTable", "DamageType",
        "Debris", "DefaultObject", "DefaultObjectManager", "DialogBranch", "DialogResponses", "DialogTopic", "Door", "EffectSequence", "EffectShader",
        "EquipType", "Explosion", "FacialExpression", "Faction", "Flora", "FogVolume", "Footstep", "FootstepSet", "ForceData", "FormFolderKeywordList",
        "FormList", "Furniture", "GameplayOption", "GameplayOptionsGroup", "GameSetting", "GenericBaseForm", "GenericBaseFormTemplate", "Global", "Grass",
        "GroundCover", "Hazard", "HeadPart", "IdleAnimation", "IdleMarker", "ImageSpace", "ImageSpaceAdapter", "Impact", "ImpactDataSet", "Ingestible",
        "InstanceNamingRules", "Key", "Keyword", "LandscapeTexture", "Layer", "LayeredMaterialSwap", "LegendaryItem", "LensFlare", "LeveledBaseForm",
        "LeveledItem", "LeveledNpc", "LeveledPackIn", "LeveledSpaceCell", "Light", "LightingTemplate", "LoadScreen", "Location", "LocationReferenceType",
        "MagicEffect", "MaterialPath", "MaterialType", "MeleeAimAssistModel", "Message", "MiscItem", "MorphableObject", "MoveableStatic", "MovementType",
        "MusicTrack", "MusicType", "NavigationMesh", "NavigationMeshInfoMap", "NavigationMeshObstacleCoverManager", "Note", "Npc", "ObjectEffect",
        "ObjectModification", "ObjectSwap", "ObjectVisibilityManager", "Outfit", "Package", "PackIn", "ParticleSystemDefineCollision", "Perk", "PERS",
        "PhotoModeFeature", "PlacedNpc", "PlacedObject", "Planet", "PlanetContentManagerBranchNode", "PlanetContentManagerContentNode", "PlanetContentManagerTree",
        "ProjectedDecal", "Projectile", "Quest", "Race", "ReferenceGroup", "Region", "ResearchProject", "Resource", "ResourceGenerationData",
        "ReverbParameters", "Scene", "SceneCollection", "SecondaryDamageList", "ShaderParticleGeometry", "SnapTemplate", "SnapTemplateBehavior",
        "SnapTemplateNode", "SoundEchoMarker", "SoundKeywordMapping", "SoundMarker", "SpeechChallenge", "Spell", "Star", "Static", "StaticCollection",
        "StoryManagerBranchNode", "StoryManagerEventNode", "StoryManagerQuestNode", "SunPreset", "SurfaceBlock", "SurfacePattern", "SurfacePatternConfig",
        "SurfacePatternStyle", "SurfaceTree", "Terminal", "TerminalMenu", "TextureSet", "TimeOfDayRecord", "Transform", "Traversal", "VoiceType",
        "VolumetricLighting", "Water", "Weapon", "WeaponBarrelModel", "Weather", "WeatherSetting", "Worldspace", "WWiseEventData", "WWiseKeywordMapping", "Zoom"
    ];

    public static readonly IReadOnlyList<string> UnsupportedRecordTypes =
    [
        "ArmorModification", "ContainerModification", "FloraModification", "GameSettingBool", "GameSettingFloat", "GameSettingInt", "GameSettingString",
        "GameSettingUInt", "NpcModification", "PlacedArrow", "PlacedBarrier", "PlacedBeam", "PlacedCone", "PlacedFlame", "PlacedHazard", "PlacedMissile",
        "PlacedTrap", "UnknownObjectModification", "WeaponModification"
    ];

    public static readonly IReadOnlyList<string> KnownMajorRecordTypes = SupportedRecordTypes
        .Concat(UnsupportedRecordTypes)
        .Distinct(StringComparer.Ordinal)
        .OrderBy(recordType => recordType, StringComparer.Ordinal)
        .ToList();

    public static bool UsesExistingTypedDetailTable(string recordType)
    {
        return recordType.Equals(FormListRecordType, StringComparison.Ordinal);
    }
}
