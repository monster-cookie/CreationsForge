namespace CreationsForge.Specification.Records;

/// <summary>
/// Contains the production record specifications used by shared specification-aware workflows.
/// </summary>
public static class SupportedRecordSpecifications
{
    /// <summary>
    /// Gets the Form List record specification.
    /// </summary>
    public static RecordSpecification FormList { get; } = FormListRecordSpecification.Instance;

    /// <summary>
    /// Gets the Game Setting record specification.
    /// </summary>
    public static RecordSpecification GameSetting { get; } = GameSettingRecordSpecification.Instance;

    /// <summary>
    /// Gets the Global record specification.
    /// </summary>
    public static RecordSpecification Global { get; } = GlobalRecordSpecification.Instance;

    /// <summary>
    /// Gets the Class record specification.
    /// </summary>
    public static RecordSpecification Class { get; } = ClassRecordSpecification.Instance;

    /// <summary>
    /// Gets the Faction record specification.
    /// </summary>
    public static RecordSpecification Faction { get; } = FactionRecordSpecification.Instance;

    /// <summary>
    /// Gets the Misc Item record specification.
    /// </summary>
    public static RecordSpecification MiscItem { get; } = MiscItemRecordSpecification.Instance;

    /// <summary>
    /// Gets the Keyword record specification.
    /// </summary>
    public static RecordSpecification Keyword { get; } = KeywordRecordSpecification.Instance;

    /// <summary>
    /// Gets the Actor Value Information record specification.
    /// </summary>
    public static RecordSpecification ActorValueInformation { get; } = ActorValueInformationRecordSpecification.Instance;

    /// <summary>
    /// Gets the NPC record specification.
    /// </summary>
    public static RecordSpecification NPC { get; } = NPCRecordSpecification.Instance;

    /// <summary>
    /// Gets the Magic Effect record specification.
    /// </summary>
    public static RecordSpecification MagicEffect { get; } = MagicEffectRecordSpecification.Instance;

    /// <summary>
    /// Gets the Perk record specification.
    /// </summary>
    public static RecordSpecification Perk { get; } = PerkRecordSpecification.Instance;

    /// <summary>
    /// Gets the Static record specification.
    /// </summary>
    public static RecordSpecification Static { get; } = StaticRecordSpecification.Instance;

    /// <summary>
    /// Gets the Container record specification.
    /// </summary>
    public static RecordSpecification Container { get; } = ContainerRecordSpecification.Instance;

    /// <summary>
    /// Gets the Constructible Object record specification.
    /// </summary>
    public static RecordSpecification ConstructibleObject { get; } = ConstructibleObjectRecordSpecification.Instance;

    /// <summary>
    /// Gets the Condition Form record specification.
    /// </summary>
    public static RecordSpecification ConditionForm { get; } = ConditionFormRecordSpecification.Instance;

    /// <summary>
    /// Gets the Book record specification.
    /// </summary>
    public static RecordSpecification Book { get; } = BookRecordSpecification.Instance;

    /// <summary>
    /// Gets the Door record specification.
    /// </summary>
    public static RecordSpecification Door { get; } = DoorRecordSpecification.Instance;

    /// <summary>
    /// Gets the Terminal record specification.
    /// </summary>
    public static RecordSpecification Terminal { get; } = TerminalRecordSpecification.Instance;

    /// <summary>
    /// Gets every specification included in the production catalog.
    /// </summary>
    public static IReadOnlyList<RecordSpecification> All { get; } =
    [
        FormList,
        GameSetting,
        Global,
        Class,
        Faction,
        MiscItem,
        Keyword,
        ActorValueInformation,
        NPC,
        MagicEffect,
        Perk,
        Static,
        Container,
        ConstructibleObject,
        ConditionForm,
        Book,
        Door,
        Terminal
    ];
}
