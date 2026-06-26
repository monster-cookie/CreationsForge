namespace CreationsForge.Specification.Records;

/// <summary>
/// Contains the production record specifications used by shared specification-aware workflows.
/// </summary>
public static class SupportedRecordSpecifications
{
    /// <summary>
    /// Gets the Form List record specification.
    /// </summary>
    public static RecordSpecification FormList { get; } = new()
    {
        RecordID = "FLST",
        RecordType = "FormList",
        TableName = "FormLists",
        FriendlyName = "Form List",
        GameSupport = CreateCurrentGameSupport("FormLists", "FormLists"),
        Import = new RecordImportSpecification
        {
            PluginRecordSetPropertyName = "FormLists",
            ImportOrder = 0,
            IsRequired = true
        },
        Reader = CreateReaderSpecification("FormLists", "FormLists"),
        Fields =
        [
            new RecordFieldSpecification
            {
                FieldName = "AddToList",
                SpriggitPath = "AddToList",
                ValueKind = RecordFieldValueKind.FormKey,
                Description = "Optional Starfield list reference that is absent for games that do not expose it."
            },
            new RecordFieldSpecification
            {
                FieldName = "Items",
                SpriggitPath = "Items",
                ValueKind = RecordFieldValueKind.Collection,
                IsCollection = true,
                Description = "Indexed FormKey entries owned by the Form List record."
            }
        ],
        Comparison = new RecordComparisonSpecification
        {
            Fields =
            [
                new RecordComparisonFieldSpecification
                {
                    FieldName = "AddToList",
                    SourcePath = "AddToList",
                    ValueKind = RecordFieldValueKind.FormKey,
                    Description = "Compared as a scalar FormKey when present."
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Items",
                    SourcePath = "Items",
                    ValueKind = RecordFieldValueKind.Collection,
                    Description = "Expanded into indexed item rows by the current comparison implementation."
                }
            ]
        },
        ImplementationNote = "Comparison metadata drives the simple scalar rows; indexed item rows remain strategy-based."
    };

    /// <summary>
    /// Gets the Game Setting record specification.
    /// </summary>
    public static RecordSpecification GameSetting { get; } = new()
    {
        RecordID = "GMST",
        RecordType = "GameSetting",
        TableName = "GameSettings",
        FriendlyName = "Game Setting",
        GameSupport = CreateCurrentGameSupport("GameSettings", "GameSettings"),
        Import = new RecordImportSpecification
        {
            PluginRecordSetPropertyName = "GameSettings",
            ImportOrder = 1,
            IsRequired = true
        },
        Reader = CreateReaderSpecification("GameSettings", "GameSettings"),
        Fields =
        [
            new RecordFieldSpecification
            {
                FieldName = "MutagenObjectType",
                SpriggitPath = "MutagenObjectType",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Concrete Mutagen game-setting type used to preserve setting value semantics."
            },
            new RecordFieldSpecification
            {
                FieldName = "Data",
                SpriggitPath = "Data",
                ValueKind = RecordFieldValueKind.Object,
                Description = "Typed scalar or localized string setting value."
            }
        ],
        Comparison = new RecordComparisonSpecification
        {
            Fields =
            [
                new RecordComparisonFieldSpecification
                {
                    FieldName = "MutagenObjectType",
                    SourcePath = "MutagenObjectType",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Data",
                    SourcePath = "Data",
                    ValueKind = RecordFieldValueKind.Object,
                    UsesLocalizedDisplay = true,
                    Description = "String settings resolve localized display text; scalar settings use typed value formatting."
                }
            ]
        },
        ImplementationNote = "Comparison metadata drives the simple rows; Data display still uses a localized-value strategy."
    };

    /// <summary>
    /// Gets the Global record specification.
    /// </summary>
    public static RecordSpecification Global { get; } = new()
    {
        RecordID = "GLOB",
        RecordType = "Global",
        TableName = "Globals",
        FriendlyName = "Global",
        GameSupport = CreateCurrentGameSupport("Globals", "Globals"),
        Import = new RecordImportSpecification
        {
            PluginRecordSetPropertyName = "Globals",
            ImportOrder = 2,
            IsRequired = true
        },
        Reader = CreateReaderSpecification("Globals", "Globals"),
        Fields =
        [
            new RecordFieldSpecification
            {
                FieldName = "MutagenObjectType",
                SpriggitPath = "MutagenObjectType",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Concrete Mutagen global type when the adapter exposes it."
            },
            new RecordFieldSpecification
            {
                FieldName = "MajorFlags",
                SpriggitPath = "MajorFlags",
                ValueKind = RecordFieldValueKind.FlagSet,
                Description = "Formatted major-record flags."
            },
            new RecordFieldSpecification
            {
                FieldName = "Data",
                SpriggitPath = "Data",
                ValueKind = RecordFieldValueKind.Number,
                Description = "Numeric global value imported from Mutagen."
            }
        ],
        Comparison = new RecordComparisonSpecification
        {
            Fields =
            [
                new RecordComparisonFieldSpecification
                {
                    FieldName = "MutagenObjectType",
                    SourcePath = "MutagenObjectType",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "MajorFlags",
                    SourcePath = "MajorFlags",
                    ValueKind = RecordFieldValueKind.FlagSet
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Data",
                    SourcePath = "Data",
                    ValueKind = RecordFieldValueKind.Number
                }
            ]
        },
        ImplementationNote = "Comparison metadata drives the simple scalar rows."
    };

    /// <summary>
    /// Gets the Class record specification.
    /// </summary>
    public static RecordSpecification Class { get; } = CreateImportOnlySpecification(
        "CLAS",
        "Class",
        "Classes",
        "Class",
        "Classes",
        3,
        isRequired: true);

    /// <summary>
    /// Gets the Faction record specification.
    /// </summary>
    public static RecordSpecification Faction { get; } = CreateImportOnlySpecification(
        "FACT",
        "Faction",
        "Factions",
        "Faction",
        "Factions",
        4,
        isRequired: true);

    /// <summary>
    /// Gets the Misc Item record specification.
    /// </summary>
    public static RecordSpecification MiscItem { get; } = CreateImportOnlySpecification(
        "MISC",
        "MiscItem",
        "MiscItems",
        "Misc Item",
        "MiscItems",
        5,
        isRequired: true);

    /// <summary>
    /// Gets the Keyword record specification.
    /// </summary>
    public static RecordSpecification Keyword { get; } = CreateImportOnlySpecification(
        "KYWD",
        "Keyword",
        "Keywords",
        "Keyword",
        "Keywords",
        6,
        isRequired: true);

    /// <summary>
    /// Gets the Actor Value Information record specification.
    /// </summary>
    public static RecordSpecification ActorValueInformation { get; } = CreateImportOnlySpecification(
        "AVIF",
        "ActorValueInformation",
        "ActorValueInformation",
        "Actor Value Information",
        "ActorValueInformation",
        7,
        isRequired: true);

    /// <summary>
    /// Gets the NPC record specification.
    /// </summary>
    public static RecordSpecification NPC { get; } = CreateImportOnlySpecification(
        "NPC_",
        "NPC",
        "NPCs",
        "NPC",
        "NPCs",
        8,
        isRequired: true);

    /// <summary>
    /// Gets the Magic Effect record specification.
    /// </summary>
    public static RecordSpecification MagicEffect { get; } = CreateImportOnlySpecification(
        "MGEF",
        "MagicEffect",
        "MagicEffects",
        "Magic Effect",
        "MagicEffects",
        9,
        isRequired: true);

    /// <summary>
    /// Gets the Perk record specification.
    /// </summary>
    public static RecordSpecification Perk { get; } = CreateImportOnlySpecification(
        "PERK",
        "Perk",
        "Perks",
        "Perk",
        "Perks",
        10,
        isRequired: true);

    /// <summary>
    /// Gets the Static record specification.
    /// </summary>
    public static RecordSpecification Static { get; } = CreateImportOnlySpecification(
        "STAT",
        "Static",
        "Statics",
        "Static",
        "Statics",
        11,
        isRequired: false);

    /// <summary>
    /// Gets the Container record specification.
    /// </summary>
    public static RecordSpecification Container { get; } = CreateImportOnlySpecification(
        "CONT",
        "Container",
        "Containers",
        "Container",
        "Containers",
        12,
        isRequired: false);

    /// <summary>
    /// Gets the Constructible Object record specification.
    /// </summary>
    public static RecordSpecification ConstructibleObject { get; } = CreateImportOnlySpecification(
        "COBJ",
        "ConstructibleObject",
        "ConstructibleObjects",
        "Constructible Object",
        "ConstructibleObjects",
        13,
        isRequired: false);

    /// <summary>
    /// Gets the Condition Form record specification.
    /// </summary>
    public static RecordSpecification ConditionForm { get; } = CreateImportOnlySpecification(
        "CNDF",
        "ConditionForm",
        "ConditionForms",
        "Condition Form",
        "ConditionForms",
        14,
        isRequired: false,
        gameSupport: CreateGameSupport("ConditionForms", "ConditionForms", SpecificationGame.Starfield));

    /// <summary>
    /// Gets the Book record specification.
    /// </summary>
    public static RecordSpecification Book { get; } = CreateImportOnlySpecification(
        "BOOK",
        "Book",
        "Books",
        "Book",
        "Books",
        15,
        isRequired: false);

    /// <summary>
    /// Gets the Door record specification.
    /// </summary>
    public static RecordSpecification Door { get; } = CreateImportOnlySpecification(
        "DOOR",
        "Door",
        "Doors",
        "Door",
        "Doors",
        16,
        isRequired: false);

    /// <summary>
    /// Gets the Terminal record specification.
    /// </summary>
    public static RecordSpecification Terminal { get; } = CreateImportOnlySpecification(
        "TERM",
        "Terminal",
        "Terminals",
        "Terminal",
        "Terminals",
        17,
        isRequired: false,
        gameSupport: CreateGameSupport(
            "Terminals",
            "Terminals",
            SpecificationGame.Starfield,
            SpecificationGame.Fallout4));

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

    /// <summary>
    /// Creates an import-dispatch specification for a current record family whose comparison metadata remains owned by
    /// record-specific Core code.
    /// </summary>
    /// <param name="recordID">The Bethesda record identifier used to resolve the typed detail importer.</param>
    /// <param name="recordType">The canonical CreationsForge record type name.</param>
    /// <param name="tableName">The current typed detail table name used in import results.</param>
    /// <param name="friendlyName">The human-readable record family name used for diagnostics and display.</param>
    /// <param name="pluginRecordSetPropertyName">The <c>PluginRecordSetDTO</c> collection property containing DTOs.</param>
    /// <param name="importOrder">The import order that preserves the existing record-dispatch sequence.</param>
    /// <param name="isRequired">A value indicating whether an import result should be emitted for empty unsupported families.</param>
    /// <param name="gameSupport">The optional game support metadata; all current adapters are used when omitted.</param>
    /// <returns>The specification containing import metadata and no declarative comparison fields.</returns>
    private static RecordSpecification CreateImportOnlySpecification(
        string recordID,
        string recordType,
        string tableName,
        string friendlyName,
        string pluginRecordSetPropertyName,
        int importOrder,
        bool isRequired,
        IReadOnlyList<RecordGameSupportSpecification>? gameSupport = null)
    {
        return new RecordSpecification
        {
            RecordID = recordID,
            RecordType = recordType,
            TableName = tableName,
            FriendlyName = friendlyName,
            GameSupport = gameSupport ?? CreateCurrentGameSupport(pluginRecordSetPropertyName, pluginRecordSetPropertyName),
            Import = new RecordImportSpecification
            {
                PluginRecordSetPropertyName = pluginRecordSetPropertyName,
                ImportOrder = importOrder,
                IsRequired = isRequired
            },
            Reader = CreateReaderSpecification(pluginRecordSetPropertyName, pluginRecordSetPropertyName),
            ImplementationNote = "Import dispatch metadata is active; comparison remains record-specific."
        };
    }

    /// <summary>
    /// Creates reader metadata for the current game-adapter record mapping path.
    /// </summary>
    /// <param name="pluginRecordSetPropertyName">The <c>PluginRecordSetDTO</c> collection property that receives mapped DTOs.</param>
    /// <param name="defaultMutagenCollectionName">The default Mutagen mod collection property read by game adapters.</param>
    /// <returns>The reader metadata used as the next specification-driven reader migration target.</returns>
    private static RecordReaderSpecification CreateReaderSpecification(
        string pluginRecordSetPropertyName,
        string defaultMutagenCollectionName)
    {
        return new RecordReaderSpecification
        {
            PluginRecordSetPropertyName = pluginRecordSetPropertyName,
            DefaultMutagenCollectionName = defaultMutagenCollectionName,
            UsesGameSpecificMapper = true
        };
    }

    /// <summary>
    /// Creates support metadata for the currently implemented CreationsForge game adapters.
    /// </summary>
    /// <param name="mutagenCollectionName">The Mutagen collection property name shared by the current adapters.</param>
    /// <param name="spriggitRecordDirectoryName">The Spriggit record-family directory name used by validation.</param>
    /// <returns>The current Starfield, Fallout 4, and Skyrim support metadata.</returns>
    private static IReadOnlyList<RecordGameSupportSpecification> CreateCurrentGameSupport(
        string mutagenCollectionName,
        string spriggitRecordDirectoryName)
    {
        return CreateGameSupport(
            mutagenCollectionName,
            spriggitRecordDirectoryName,
            SpecificationGame.Starfield,
            SpecificationGame.Fallout4,
            SpecificationGame.Skyrim);
    }

    /// <summary>
    /// Creates support metadata for the specified game adapters.
    /// </summary>
    /// <param name="mutagenCollectionName">The Mutagen collection property name exposed by the selected adapters.</param>
    /// <param name="spriggitRecordDirectoryName">The Spriggit record-family directory name used by validation.</param>
    /// <param name="games">The supported games that expose the record family through current adapters.</param>
    /// <returns>The requested game support metadata.</returns>
    private static IReadOnlyList<RecordGameSupportSpecification> CreateGameSupport(
        string mutagenCollectionName,
        string spriggitRecordDirectoryName,
        params SpecificationGame[] games)
    {
        return
        [
            .. games.Select(game => new RecordGameSupportSpecification
            {
                Game = game,
                MutagenCollectionName = mutagenCollectionName,
                SpriggitRecordDirectoryName = spriggitRecordDirectoryName
            })
        ];
    }
}
