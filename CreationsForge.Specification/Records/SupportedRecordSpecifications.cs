namespace CreationsForge.Specification.Records;

/// <summary>
/// Contains the first production record specifications used to prove the specification project boundary.
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
    /// Gets every specification included in the first production catalog.
    /// </summary>
    public static IReadOnlyList<RecordSpecification> All { get; } =
    [
        FormList,
        GameSetting,
        Global
    ];

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
        return
        [
            new RecordGameSupportSpecification
            {
                Game = SpecificationGame.Starfield,
                MutagenCollectionName = mutagenCollectionName,
                SpriggitRecordDirectoryName = spriggitRecordDirectoryName
            },
            new RecordGameSupportSpecification
            {
                Game = SpecificationGame.Fallout4,
                MutagenCollectionName = mutagenCollectionName,
                SpriggitRecordDirectoryName = spriggitRecordDirectoryName
            },
            new RecordGameSupportSpecification
            {
                Game = SpecificationGame.Skyrim,
                MutagenCollectionName = mutagenCollectionName,
                SpriggitRecordDirectoryName = spriggitRecordDirectoryName
            }
        ];
    }
}
