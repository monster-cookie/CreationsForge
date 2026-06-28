namespace CreationsForge.Specification.Records;

/// <summary>
/// Identifies a comparison child-row strategy that can be selected by record-family metadata while the row-building
/// implementation remains owned by Core.
/// </summary>
public enum RecordComparisonChildGroupKind
{
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
    ContainerForcedLocations
}
