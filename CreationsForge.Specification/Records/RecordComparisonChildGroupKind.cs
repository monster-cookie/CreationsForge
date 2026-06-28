namespace CreationsForge.Specification.Records;

/// <summary>
/// Identifies a comparison child-row strategy that can be selected by record-family metadata while the row-building
/// implementation remains owned by Core.
/// </summary>
public enum RecordComparisonChildGroupKind
{
    /// <summary>
    /// Indicates that persisted form list item rows should be rendered as indexed <c>Items</c> rows.
    /// </summary>
    FormListItems,

    /// <summary>
    /// Indicates that persisted keyword mapping rows should be rendered as the shared <c>Keywords</c> child group.
    /// </summary>
    KeywordMappings,

    /// <summary>
    /// Indicates that persisted sound mapping rows should be rendered as the shared <c>Sounds</c> child group.
    /// </summary>
    SoundMappings,

    /// <summary>
    /// Indicates that persisted model rows should be rendered as shared model child groups.
    /// </summary>
    ModelMappings,

    /// <summary>
    /// Indicates that persisted scripting adapter rows should be rendered as the shared <c>Scripts</c> child group.
    /// </summary>
    ScriptingAdapterMappings,

    /// <summary>
    /// Indicates that persisted reflection rows should be rendered as the shared <c>Reflection</c> child group.
    /// </summary>
    ReflectionMappings,

    /// <summary>
    /// Indicates that persisted condition rules should be rendered as the shared <c>Conditions</c> child group.
    /// </summary>
    ConditionRules,

    /// <summary>
    /// Indicates that persisted shared record component rows should be rendered as the shared <c>Components</c> child
    /// group.
    /// </summary>
    RecordComponents,

    /// <summary>
    /// Indicates that persisted script fragment rows should be rendered as the shared <c>Script Fragments</c> child
    /// group.
    /// </summary>
    ScriptFragments,

    /// <summary>
    /// Indicates that persisted class property rows should be rendered as the <c>Properties</c> child group.
    /// </summary>
    ClassProperties,

    /// <summary>
    /// Indicates that persisted class skill-weight rows should be rendered as the <c>SkillWeights</c> child group.
    /// </summary>
    ClassSkillWeights,

    /// <summary>
    /// Indicates that persisted class stat-weight rows should be rendered as the <c>StatWeights</c> child group.
    /// </summary>
    ClassStatWeights,

    /// <summary>
    /// Indicates that persisted faction relation rows should be rendered as the <c>Relations</c> child group.
    /// </summary>
    FactionRelations,

    /// <summary>
    /// Indicates that persisted faction rank rows should be rendered as the <c>Ranks</c> child group.
    /// </summary>
    FactionRanks,

    /// <summary>
    /// Indicates that persisted static property rows should be rendered as indexed <c>Property</c> child rows.
    /// </summary>
    StaticProperties,

    /// <summary>
    /// Indicates that persisted constructible object component rows should be rendered as the <c>Components</c> child
    /// group.
    /// </summary>
    ConstructibleObjectComponents,

    /// <summary>
    /// Indicates that persisted constructible object category rows should be rendered as the <c>Categories</c> child
    /// group.
    /// </summary>
    ConstructibleObjectCategories,

    /// <summary>
    /// Indicates that persisted constructible object recipe-filter rows should be rendered as the
    /// <c>RecipeFilters</c> child group.
    /// </summary>
    ConstructibleObjectRecipeFilters,

    /// <summary>
    /// Indicates that persisted container item rows should be rendered as the <c>Items</c> child group.
    /// </summary>
    ContainerItems,

    /// <summary>
    /// Indicates that persisted container property rows should be rendered as indexed <c>Property</c> child rows.
    /// </summary>
    ContainerProperties,

    /// <summary>
    /// Indicates that persisted container forced-location links should be rendered as indexed
    /// <c>ForcedLocations</c> rows.
    /// </summary>
    ContainerForcedLocations,

    /// <summary>
    /// Indicates that persisted terminal forced-location links should be rendered as indexed
    /// <c>ForcedLocations</c> rows.
    /// </summary>
    TerminalForcedLocations,

    /// <summary>
    /// Indicates that persisted terminal marker parameter rows should be rendered as the
    /// <c>Marker Parameters</c> child group.
    /// </summary>
    TerminalMarkerParameters,

    /// <summary>
    /// Indicates that persisted terminal body text rows should be rendered as the <c>BodyTexts</c> child group.
    /// </summary>
    TerminalBodyTexts,

    /// <summary>
    /// Indicates that persisted terminal menu item rows should be rendered as the <c>MenuItems</c> child group.
    /// </summary>
    TerminalMenuItems,

    /// <summary>
    /// Indicates that persisted misc item destructible data should be rendered as the <c>Destructible</c> child group.
    /// </summary>
    MiscItemDestructible,

    /// <summary>
    /// Indicates that persisted misc item component rows should be rendered as the <c>Components</c> child group.
    /// </summary>
    MiscItemComponents,

    /// <summary>
    /// Indicates that persisted misc item resource rows should be rendered as the <c>Resources</c> child group.
    /// </summary>
    MiscItemResources,

    /// <summary>
    /// Indicates that persisted actor value perk-tree rows should be rendered as the <c>PerkTree</c> child group.
    /// </summary>
    ActorValueInformationPerkTree,

    /// <summary>
    /// Indicates that persisted NPC level data should be rendered as the <c>Level</c> child group.
    /// </summary>
    NPCLevel,

    /// <summary>
    /// Indicates that persisted NPC configuration data should be rendered as the <c>Configuration</c> child group.
    /// </summary>
    NPCConfiguration,

    /// <summary>
    /// Indicates that persisted NPC supplemental parent and nested actor data should be rendered after scalar parent rows.
    /// </summary>
    NPCSupplementalFields,

    /// <summary>
    /// Indicates that persisted NPC package links should be rendered as the <c>Packages</c> child group.
    /// </summary>
    NPCPackages,

    /// <summary>
    /// Indicates that persisted NPC forced-location links should be rendered as the <c>ForcedLocations</c> child group.
    /// </summary>
    NPCForcedLocations,

    /// <summary>
    /// Indicates that persisted NPC head-part links should be rendered as the <c>HeadParts</c> child group.
    /// </summary>
    NPCHeadParts,

    /// <summary>
    /// Indicates that persisted NPC actor-effect links should be rendered as the <c>ActorEffects</c> child group.
    /// </summary>
    NPCActorEffects,

    /// <summary>
    /// Indicates that persisted NPC faction rows should be rendered as the <c>Factions</c> child group.
    /// </summary>
    NPCFactions,

    /// <summary>
    /// Indicates that persisted NPC actor-value property rows should be rendered as the <c>Properties</c> child group.
    /// </summary>
    NPCProperties,

    /// <summary>
    /// Indicates that persisted NPC inventory item rows should be rendered as the <c>Items</c> child group.
    /// </summary>
    NPCItems,

    /// <summary>
    /// Indicates that persisted NPC perk rows should be rendered as the <c>Perks</c> child group.
    /// </summary>
    NPCPerks,

    /// <summary>
    /// Indicates that persisted NPC morph rows should be rendered as the <c>Morphs</c> child group.
    /// </summary>
    NPCMorphs,

    /// <summary>
    /// Indicates that persisted NPC face morph position rows should be rendered as the <c>FaceMorphs</c> child group.
    /// </summary>
    NPCFaceMorphs,

    /// <summary>
    /// Indicates that persisted NPC face dial position rows should be rendered as the <c>FaceDialPositions</c> child group.
    /// </summary>
    NPCFaceDialPositions,

    /// <summary>
    /// Indicates that persisted NPC face morph group rows should be rendered as the <c>FaceMorphGroups</c> child group.
    /// </summary>
    NPCFaceMorphGroups,

    /// <summary>
    /// Indicates that persisted NPC morph blend rows should be rendered as the <c>MorphBlends</c> child group.
    /// </summary>
    NPCMorphBlends,

    /// <summary>
    /// Indicates that persisted NPC tint rows should be rendered as the <c>Tints</c> child group.
    /// </summary>
    NPCTints,

    /// <summary>
    /// Indicates that persisted NPC tint layer rows should be rendered as the <c>TintLayers</c> child group.
    /// </summary>
    NPCTintLayers,

    /// <summary>
    /// Indicates that persisted NPC face tinting layer rows should be rendered as the <c>FaceTintingLayers</c> child group.
    /// </summary>
    NPCFaceTintingLayers,

    /// <summary>
    /// Indicates that persisted NPC player skill rows should be rendered as the <c>PlayerSkills</c> child group.
    /// </summary>
    NPCPlayerSkills,

    /// <summary>
    /// Indicates that persisted perk effect rows should be rendered as the <c>Effects</c> child group.
    /// </summary>
    PerkEffects,

    /// <summary>
    /// Indicates that persisted perk rank rows should be rendered as the <c>Ranks</c> child group.
    /// </summary>
    PerkRanks,

    /// <summary>
    /// Indicates that persisted perk background skill rows should be rendered as the <c>Background Skills</c> child group.
    /// </summary>
    PerkBackgroundSkills,

    /// <summary>
    /// Indicates that persisted static navmesh geometry rows should be rendered as the <c>Navmesh Geometry</c> child group.
    /// </summary>
    StaticNavmeshGeometry
}
