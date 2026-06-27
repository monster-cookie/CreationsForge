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
    public static RecordSpecification Keyword { get; } = new()
    {
        RecordID = "KYWD",
        RecordType = "Keyword",
        TableName = "Keywords",
        FriendlyName = "Keyword",
        GameSupport = CreateCurrentGameSupport("Keywords", "Keywords"),
        Import = new RecordImportSpecification
        {
            PluginRecordSetPropertyName = "Keywords",
            ImportOrder = 6,
            IsRequired = true
        },
        Reader = CreateReaderSpecification("Keywords", "Keywords"),
        Fields =
        [
            new RecordFieldSpecification
            {
                FieldName = "Name",
                SpriggitPath = "Name",
                ValueKind = RecordFieldValueKind.LocalizedString,
                IsLocalized = true,
                Description = "Optional translated keyword display name."
            },
            new RecordFieldSpecification
            {
                FieldName = "Color",
                SpriggitPath = "Color",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Formatted keyword color payload when the source game exposes one."
            },
            new RecordFieldSpecification
            {
                FieldName = "Type",
                SpriggitPath = "Type",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Keyword type or category text when the source game exposes one."
            },
            new RecordFieldSpecification
            {
                FieldName = "Notes",
                SpriggitPath = "Notes",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Optional source notes text for the keyword."
            },
            new RecordFieldSpecification
            {
                FieldName = "FlashLinkageName",
                SpriggitPath = "FlashLinkageName",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Optional UI linkage name carried by the keyword."
            },
            new RecordFieldSpecification
            {
                FieldName = "FNAM",
                SpriggitPath = "FNAM",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Optional source FNAM text retained on the keyword row."
            },
            new RecordFieldSpecification
            {
                FieldName = "WAIM",
                SpriggitPath = "WAIM",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Optional source WAIM text retained on the keyword row."
            },
            new RecordFieldSpecification
            {
                FieldName = "WFIR",
                SpriggitPath = "WFIR",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Optional source WFIR text retained on the keyword row."
            },
            new RecordFieldSpecification
            {
                FieldName = "AttractionRule",
                SpriggitPath = "AttractionRule",
                ValueKind = RecordFieldValueKind.FormKey,
                Description = "Optional attraction-rule FormKey reference."
            }
        ],
        Comparison = new RecordComparisonSpecification
        {
            Fields =
            [
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Version2",
                    SourcePath = "Version2",
                    ValueKind = RecordFieldValueKind.Number
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Name",
                    SourcePath = "Name",
                    ValueKind = RecordFieldValueKind.LocalizedString,
                    UsesLocalizedDisplay = true,
                    Description = "Resolved through the localized-record-text strategy when present."
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Color",
                    SourcePath = "Color",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Type",
                    SourcePath = "Type",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Notes",
                    SourcePath = "Notes",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "FlashLinkageName",
                    SourcePath = "FlashLinkageName",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "FNAM",
                    SourcePath = "FNAM",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "WAIM",
                    SourcePath = "WAIM",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "WFIR",
                    SourcePath = "WFIR",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "AttractionRule",
                    SourcePath = "AttractionRule",
                    ValueKind = RecordFieldValueKind.FormKey
                }
            ]
        },
        ImplementationNote = "Comparison metadata drives scalar parent rows; child scripting data remains strategy-based."
    };

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
    public static RecordSpecification Static { get; } = new()
    {
        RecordID = "STAT",
        RecordType = "Static",
        TableName = "Statics",
        FriendlyName = "Static",
        GameSupport = CreateCurrentGameSupport("Statics", "Statics"),
        Import = new RecordImportSpecification
        {
            PluginRecordSetPropertyName = "Statics",
            ImportOrder = 11,
            IsRequired = false
        },
        Reader = CreateReaderSpecification("Statics", "Statics"),
        Fields =
        [
            new RecordFieldSpecification
            {
                FieldName = "Name",
                SpriggitPath = "Name",
                ValueKind = RecordFieldValueKind.LocalizedString,
                IsLocalized = true,
                Description = "Optional translated static display name."
            },
            new RecordFieldSpecification
            {
                FieldName = "ObjectBoundsFirst",
                SpriggitPath = "ObjectBounds.First",
                ValueKind = RecordFieldValueKind.Text,
                Description = "First persisted object-bounds vector text."
            },
            new RecordFieldSpecification
            {
                FieldName = "ObjectBoundsSecond",
                SpriggitPath = "ObjectBounds.Second",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Second persisted object-bounds vector text."
            },
            new RecordFieldSpecification
            {
                FieldName = "MaxAngle",
                SpriggitPath = "MaxAngle",
                ValueKind = RecordFieldValueKind.Number,
                Description = "Maximum angle value imported from the static parent record."
            },
            new RecordFieldSpecification
            {
                FieldName = "UnknownDNAMFloat",
                SpriggitPath = "UnknownDNAMFloat",
                ValueKind = RecordFieldValueKind.Number,
                Description = "Optional DNAM float value retained from the source record."
            },
            new RecordFieldSpecification
            {
                FieldName = "LeafAmplitude",
                SpriggitPath = "LeafAmplitude",
                ValueKind = RecordFieldValueKind.Number,
                Description = "Optional leaf-amplitude scalar when the source game exposes it."
            },
            new RecordFieldSpecification
            {
                FieldName = "LeafFrequency",
                SpriggitPath = "LeafFrequency",
                ValueKind = RecordFieldValueKind.Number,
                Description = "Optional leaf-frequency scalar when the source game exposes it."
            },
            new RecordFieldSpecification
            {
                FieldName = "Unused",
                SpriggitPath = "Unused",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Optional unused text payload retained from the static parent row."
            },
            new RecordFieldSpecification
            {
                FieldName = "DNAMDataTypeState",
                SpriggitPath = "DNAMDataTypeState",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Optional DNAM data-type state text."
            },
            new RecordFieldSpecification
            {
                FieldName = "DirtinessScale",
                SpriggitPath = "DirtinessScale",
                ValueKind = RecordFieldValueKind.Number,
                Description = "Optional Starfield dirtiness scale value."
            },
            new RecordFieldSpecification
            {
                FieldName = "SnapTemplate",
                SpriggitPath = "SnapTemplate",
                ValueKind = RecordFieldValueKind.FormKey,
                Description = "Optional snap-template FormKey reference."
            },
            new RecordFieldSpecification
            {
                FieldName = "PreviewTransform",
                SpriggitPath = "PreviewTransform",
                ValueKind = RecordFieldValueKind.FormKey,
                Description = "Optional preview-transform FormKey reference."
            },
            new RecordFieldSpecification
            {
                FieldName = "Material",
                SpriggitPath = "Material",
                ValueKind = RecordFieldValueKind.FormKey,
                Description = "Optional material FormKey reference."
            },
            new RecordFieldSpecification
            {
                FieldName = "LodLevel0",
                SpriggitPath = "Lod.Level0",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Optional level-of-detail data for LOD level 0."
            },
            new RecordFieldSpecification
            {
                FieldName = "LodLevel1",
                SpriggitPath = "Lod.Level1",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Optional level-of-detail data for LOD level 1."
            },
            new RecordFieldSpecification
            {
                FieldName = "LodLevel2",
                SpriggitPath = "Lod.Level2",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Optional level-of-detail data for LOD level 2."
            },
            new RecordFieldSpecification
            {
                FieldName = "LodLevel3",
                SpriggitPath = "Lod.Level3",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Optional level-of-detail data for LOD level 3."
            }
        ],
        Comparison = new RecordComparisonSpecification
        {
            Fields =
            [
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Name",
                    SourcePath = "Name",
                    ValueKind = RecordFieldValueKind.LocalizedString,
                    UsesLocalizedDisplay = true,
                    Description = "Resolved through the localized-record-text strategy when present."
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Version2",
                    SourcePath = "Version2",
                    ValueKind = RecordFieldValueKind.Number
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "ObjectBoundsFirst",
                    SourcePath = "ObjectBoundsFirst",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "ObjectBoundsSecond",
                    SourcePath = "ObjectBoundsSecond",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "MaxAngle",
                    SourcePath = "MaxAngle",
                    ValueKind = RecordFieldValueKind.Number
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "UnknownDNAMFloat",
                    SourcePath = "UnknownDNAMFloat",
                    ValueKind = RecordFieldValueKind.Number
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "LeafAmplitude",
                    SourcePath = "LeafAmplitude",
                    ValueKind = RecordFieldValueKind.Number
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "LeafFrequency",
                    SourcePath = "LeafFrequency",
                    ValueKind = RecordFieldValueKind.Number
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Unused",
                    SourcePath = "Unused",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "DNAMDataTypeState",
                    SourcePath = "DNAMDataTypeState",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "DirtinessScale",
                    SourcePath = "DirtinessScale",
                    ValueKind = RecordFieldValueKind.Number
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "SnapTemplate",
                    SourcePath = "SnapTemplate",
                    ValueKind = RecordFieldValueKind.FormKey
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "PreviewTransform",
                    SourcePath = "PreviewTransform",
                    ValueKind = RecordFieldValueKind.FormKey
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Material",
                    SourcePath = "Material",
                    ValueKind = RecordFieldValueKind.FormKey
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Lod.Level0",
                    SourcePath = "LodLevel0",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Lod.Level1",
                    SourcePath = "LodLevel1",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Lod.Level2",
                    SourcePath = "LodLevel2",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Lod.Level3",
                    SourcePath = "LodLevel3",
                    ValueKind = RecordFieldValueKind.Text
                }
            ]
        },
        ImplementationNote = "Comparison metadata drives scalar parent rows; navmesh, keyword, property, model, and reflection rows remain strategy-based."
    };

    /// <summary>
    /// Gets the Container record specification.
    /// </summary>
    public static RecordSpecification Container { get; } = new()
    {
        RecordID = "CONT",
        RecordType = "Container",
        TableName = "Containers",
        FriendlyName = "Container",
        GameSupport = CreateCurrentGameSupport("Containers", "Containers"),
        Import = new RecordImportSpecification
        {
            PluginRecordSetPropertyName = "Containers",
            ImportOrder = 12,
            IsRequired = false
        },
        Reader = CreateReaderSpecification("Containers", "Containers"),
        Fields =
        [
            new RecordFieldSpecification
            {
                FieldName = "ObjectBoundsFirst",
                SpriggitPath = "ObjectBounds.First",
                ValueKind = RecordFieldValueKind.Text,
                Description = "First persisted object-bounds vector text."
            },
            new RecordFieldSpecification
            {
                FieldName = "ObjectBoundsSecond",
                SpriggitPath = "ObjectBounds.Second",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Second persisted object-bounds vector text."
            },
            new RecordFieldSpecification
            {
                FieldName = "Name",
                SpriggitPath = "Name",
                ValueKind = RecordFieldValueKind.LocalizedString,
                IsLocalized = true,
                Description = "Optional translated container display name."
            },
            new RecordFieldSpecification
            {
                FieldName = "Flags",
                SpriggitPath = "Flags",
                ValueKind = RecordFieldValueKind.FlagSet,
                Description = "Formatted container flags."
            },
            new RecordFieldSpecification
            {
                FieldName = "MajorFlags",
                SpriggitPath = "MajorFlags",
                ValueKind = RecordFieldValueKind.FlagSet,
                Description = "Named major-record flags persisted for the container."
            },
            new RecordFieldSpecification
            {
                FieldName = "NativeTerminalFormKey",
                SpriggitPath = "NativeTerminalFormKey",
                ValueKind = RecordFieldValueKind.FormKey,
                Description = "Optional native terminal FormKey reference."
            },
            new RecordFieldSpecification
            {
                FieldName = "SnapTemplate",
                SpriggitPath = "SnapTemplate",
                ValueKind = RecordFieldValueKind.FormKey,
                Description = "Optional snap-template FormKey reference."
            },
            new RecordFieldSpecification
            {
                FieldName = "ContainsOnlyFilter",
                SpriggitPath = "ContainsOnlyFilter",
                ValueKind = RecordFieldValueKind.FormKey,
                Description = "Optional contains-only filter FormKey reference."
            },
            new RecordFieldSpecification
            {
                FieldName = "Transforms",
                SpriggitPath = "Transforms",
                ValueKind = RecordFieldValueKind.Object,
                Description = "Optional transform FormKeys represented by scalar comparison rows."
            },
            new RecordFieldSpecification
            {
                FieldName = "AnimationGraph",
                SpriggitPath = "AnimationGraph",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Optional animation graph path or identifier."
            },
            new RecordFieldSpecification
            {
                FieldName = "AnimationSkeleton",
                SpriggitPath = "AnimationSkeleton",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Optional animation skeleton path or identifier."
            },
            new RecordFieldSpecification
            {
                FieldName = "AnimationDirectory",
                SpriggitPath = "AnimationDirectory",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Optional animation directory path."
            },
            new RecordFieldSpecification
            {
                FieldName = "AnimationFile",
                SpriggitPath = "AnimationFile",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Optional animation file path."
            }
        ],
        Comparison = new RecordComparisonSpecification
        {
            Fields =
            [
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Version2",
                    SourcePath = "Version2",
                    ValueKind = RecordFieldValueKind.Number
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "ObjectBoundsFirst",
                    SourcePath = "ObjectBoundsFirst",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "ObjectBoundsSecond",
                    SourcePath = "ObjectBoundsSecond",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Name",
                    SourcePath = "Name",
                    ValueKind = RecordFieldValueKind.LocalizedString,
                    UsesLocalizedDisplay = true
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Flags",
                    SourcePath = "Flags",
                    ValueKind = RecordFieldValueKind.FlagSet
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "MajorFlags",
                    SourcePath = "MajorFlags",
                    ValueKind = RecordFieldValueKind.FlagSet
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "NativeTerminalFormKey",
                    SourcePath = "NativeTerminalFormKey",
                    ValueKind = RecordFieldValueKind.FormKey
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "SnapTemplate",
                    SourcePath = "SnapTemplate",
                    ValueKind = RecordFieldValueKind.FormKey
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "ContainsOnlyFilter",
                    SourcePath = "ContainsOnlyFilter",
                    ValueKind = RecordFieldValueKind.FormKey
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Transforms.Outpost",
                    SourcePath = "Transforms.Outpost",
                    ValueKind = RecordFieldValueKind.FormKey
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Transforms.Preview",
                    SourcePath = "Transforms.Preview",
                    ValueKind = RecordFieldValueKind.FormKey
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "AnimationGraph",
                    SourcePath = "AnimationGraph",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "AnimationSkeleton",
                    SourcePath = "AnimationSkeleton",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "AnimationDirectory",
                    SourcePath = "AnimationDirectory",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "AnimationFile",
                    SourcePath = "AnimationFile",
                    ValueKind = RecordFieldValueKind.Text
                }
            ]
        },
        ImplementationNote = "Comparison metadata drives scalar parent rows; item, property, forced-location, keyword, model, sound, script, component, and reflection rows remain strategy-based."
    };

    /// <summary>
    /// Gets the Constructible Object record specification.
    /// </summary>
    public static RecordSpecification ConstructibleObject { get; } = new()
    {
        RecordID = "COBJ",
        RecordType = "ConstructibleObject",
        TableName = "ConstructibleObjects",
        FriendlyName = "Constructible Object",
        GameSupport = CreateCurrentGameSupport("ConstructibleObjects", "ConstructibleObjects"),
        Import = new RecordImportSpecification
        {
            PluginRecordSetPropertyName = "ConstructibleObjects",
            ImportOrder = 13,
            IsRequired = false
        },
        Reader = CreateReaderSpecification("ConstructibleObjects", "ConstructibleObjects"),
        Fields =
        [
            new RecordFieldSpecification
            {
                FieldName = "Description",
                SpriggitPath = "Description",
                ValueKind = RecordFieldValueKind.LocalizedString,
                IsLocalized = true,
                Description = "Optional translated constructible-object description."
            },
            new RecordFieldSpecification
            {
                FieldName = "CreatedObjectFormKey",
                SpriggitPath = "CreatedObjectFormKey",
                ValueKind = RecordFieldValueKind.FormKey,
                Description = "The object produced by the recipe."
            },
            new RecordFieldSpecification
            {
                FieldName = "WorkbenchKeywordFormKey",
                SpriggitPath = "WorkbenchKeywordFormKey",
                ValueKind = RecordFieldValueKind.FormKey,
                Description = "Optional workbench keyword used by the recipe."
            },
            new RecordFieldSpecification
            {
                FieldName = "CreatedObjectCount",
                SpriggitPath = "CreatedObjectCount",
                ValueKind = RecordFieldValueKind.Number,
                Description = "Optional count of objects created by the recipe."
            },
            new RecordFieldSpecification
            {
                FieldName = "AmountProduced",
                SpriggitPath = "AmountProduced",
                ValueKind = RecordFieldValueKind.Number,
                Description = "Optional produced amount retained from the source record."
            },
            new RecordFieldSpecification
            {
                FieldName = "Value",
                SpriggitPath = "Value",
                ValueKind = RecordFieldValueKind.Number,
                Description = "Optional constructible-object value scalar."
            },
            new RecordFieldSpecification
            {
                FieldName = "MenuSortOrder",
                SpriggitPath = "MenuSortOrder",
                ValueKind = RecordFieldValueKind.Number,
                Description = "Optional menu sort order scalar."
            },
            new RecordFieldSpecification
            {
                FieldName = "LearnMethod",
                SpriggitPath = "LearnMethod",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Optional learn-method text."
            },
            new RecordFieldSpecification
            {
                FieldName = "Flags",
                SpriggitPath = "Flags",
                ValueKind = RecordFieldValueKind.FlagSet,
                Description = "Formatted constructible-object flags."
            },
            new RecordFieldSpecification
            {
                FieldName = "MajorFlags",
                SpriggitPath = "MajorFlags",
                ValueKind = RecordFieldValueKind.FlagSet,
                Description = "Named major-record flags persisted for the constructible object."
            }
        ],
        Comparison = new RecordComparisonSpecification
        {
            Fields =
            [
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Version2",
                    SourcePath = "Version2",
                    ValueKind = RecordFieldValueKind.Number
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Description",
                    SourcePath = "Description",
                    ValueKind = RecordFieldValueKind.LocalizedString,
                    UsesLocalizedDisplay = true
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "CreatedObjectFormKey",
                    SourcePath = "CreatedObjectFormKey",
                    ValueKind = RecordFieldValueKind.FormKey
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "WorkbenchKeywordFormKey",
                    SourcePath = "WorkbenchKeywordFormKey",
                    ValueKind = RecordFieldValueKind.FormKey
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "CreatedObjectCount",
                    SourcePath = "CreatedObjectCount",
                    ValueKind = RecordFieldValueKind.Number
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "AmountProduced",
                    SourcePath = "AmountProduced",
                    ValueKind = RecordFieldValueKind.Number
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Value",
                    SourcePath = "Value",
                    ValueKind = RecordFieldValueKind.Number
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "MenuSortOrder",
                    SourcePath = "MenuSortOrder",
                    ValueKind = RecordFieldValueKind.Number
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "LearnMethod",
                    SourcePath = "LearnMethod",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Flags",
                    SourcePath = "Flags",
                    ValueKind = RecordFieldValueKind.FlagSet
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "MajorFlags",
                    SourcePath = "MajorFlags",
                    ValueKind = RecordFieldValueKind.FlagSet
                }
            ]
        },
        ImplementationNote = "Comparison metadata drives scalar parent rows; component, category, recipe-filter, condition, sound, and script rows remain strategy-based."
    };

    /// <summary>
    /// Gets the Condition Form record specification.
    /// </summary>
    public static RecordSpecification ConditionForm { get; } = new()
    {
        RecordID = "CNDF",
        RecordType = "ConditionForm",
        TableName = "ConditionForms",
        FriendlyName = "Condition Form",
        GameSupport = CreateGameSupport("ConditionForms", "ConditionForms", SpecificationGame.Starfield),
        Import = new RecordImportSpecification
        {
            PluginRecordSetPropertyName = "ConditionForms",
            ImportOrder = 14,
            IsRequired = false
        },
        Reader = CreateReaderSpecification("ConditionForms", "ConditionForms"),
        Fields =
        [
            new RecordFieldSpecification
            {
                FieldName = "OwnerQuest",
                SpriggitPath = "OwnerQuest",
                ValueKind = RecordFieldValueKind.FormKey,
                Description = "Optional owner quest FormKey for the condition form."
            }
        ],
        Comparison = new RecordComparisonSpecification
        {
            Fields =
            [
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Version2",
                    SourcePath = "Version2",
                    ValueKind = RecordFieldValueKind.Number
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "OwnerQuest",
                    SourcePath = "OwnerQuest",
                    ValueKind = RecordFieldValueKind.FormKey
                }
            ]
        },
        ImplementationNote = "Comparison metadata drives scalar parent rows; condition rows remain strategy-based."
    };

    /// <summary>
    /// Gets the Book record specification.
    /// </summary>
    public static RecordSpecification Book { get; } = new()
    {
        RecordID = "BOOK",
        RecordType = "Book",
        TableName = "Books",
        FriendlyName = "Book",
        GameSupport = CreateCurrentGameSupport("Books", "Books"),
        Import = new RecordImportSpecification
        {
            PluginRecordSetPropertyName = "Books",
            ImportOrder = 15,
            IsRequired = false
        },
        Reader = CreateReaderSpecification("Books", "Books"),
        Fields =
        [
            new RecordFieldSpecification
            {
                FieldName = "ObjectBounds",
                SpriggitPath = "ObjectBounds",
                ValueKind = RecordFieldValueKind.Object,
                Description = "Optional object-bounds structure represented by scalar comparison rows."
            },
            new RecordFieldSpecification
            {
                FieldName = "Transforms.Inventory",
                SpriggitPath = "Transforms.Inventory",
                ValueKind = RecordFieldValueKind.FormKey,
                Description = "Optional inventory transform FormKey reference."
            },
            new RecordFieldSpecification
            {
                FieldName = "InventoryArt",
                SpriggitPath = "InventoryArt",
                ValueKind = RecordFieldValueKind.FormKey,
                Description = "Optional inventory-art FormKey reference."
            },
            new RecordFieldSpecification
            {
                FieldName = "PreviewTransform",
                SpriggitPath = "PreviewTransform",
                ValueKind = RecordFieldValueKind.FormKey,
                Description = "Optional preview-transform FormKey reference."
            },
            new RecordFieldSpecification
            {
                FieldName = "FeaturedItemMessage",
                SpriggitPath = "FeaturedItemMessage",
                ValueKind = RecordFieldValueKind.FormKey,
                Description = "Optional featured-item message FormKey reference."
            },
            new RecordFieldSpecification
            {
                FieldName = "XALG",
                SpriggitPath = "XALG",
                ValueKind = RecordFieldValueKind.Number,
                Description = "Optional Starfield scalar retained on the book parent row."
            },
            new RecordFieldSpecification
            {
                FieldName = "Name",
                SpriggitPath = "Name",
                ValueKind = RecordFieldValueKind.LocalizedString,
                IsLocalized = true,
                Description = "Optional translated book display name."
            },
            new RecordFieldSpecification
            {
                FieldName = "Text",
                SpriggitPath = "Text",
                ValueKind = RecordFieldValueKind.LocalizedString,
                IsLocalized = true,
                Description = "Book body text; Fallout 4 and Skyrim use the BookText localized source field."
            },
            new RecordFieldSpecification
            {
                FieldName = "Value",
                SpriggitPath = "Value",
                ValueKind = RecordFieldValueKind.Number,
                Description = "Book value scalar."
            },
            new RecordFieldSpecification
            {
                FieldName = "Weight",
                SpriggitPath = "Weight",
                ValueKind = RecordFieldValueKind.Number,
                Description = "Book weight scalar."
            },
            new RecordFieldSpecification
            {
                FieldName = "Flags",
                SpriggitPath = "Flags",
                ValueKind = RecordFieldValueKind.FlagSet,
                Description = "Formatted book flags."
            },
            new RecordFieldSpecification
            {
                FieldName = "Teaches",
                SpriggitPath = "Teaches",
                ValueKind = RecordFieldValueKind.Object,
                Description = "Optional teaching payload represented by scalar comparison rows."
            },
            new RecordFieldSpecification
            {
                FieldName = "DataSlateType",
                SpriggitPath = "DataSlateType",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Optional Starfield data slate type."
            },
            new RecordFieldSpecification
            {
                FieldName = "Description",
                SpriggitPath = "Description",
                ValueKind = RecordFieldValueKind.LocalizedString,
                IsLocalized = true,
                Description = "Optional translated data slate description."
            },
            new RecordFieldSpecification
            {
                FieldName = "DataSlateHeaderLeft",
                SpriggitPath = "DataSlateHeaderLeft",
                ValueKind = RecordFieldValueKind.LocalizedString,
                IsLocalized = true,
                Description = "Optional translated left data slate header."
            },
            new RecordFieldSpecification
            {
                FieldName = "DataSlateHeaderRight",
                SpriggitPath = "DataSlateHeaderRight",
                ValueKind = RecordFieldValueKind.LocalizedString,
                IsLocalized = true,
                Description = "Optional translated right data slate header."
            }
        ],
        Comparison = new RecordComparisonSpecification
        {
            Fields =
            [
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Version2",
                    SourcePath = "Version2",
                    ValueKind = RecordFieldValueKind.Number
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "ObjectBounds.First",
                    SourcePath = "ObjectBounds.First",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "ObjectBounds.Second",
                    SourcePath = "ObjectBounds.Second",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Transforms.Inventory",
                    SourcePath = "Transforms.Inventory",
                    ValueKind = RecordFieldValueKind.FormKey
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "InventoryArt",
                    SourcePath = "InventoryArt",
                    ValueKind = RecordFieldValueKind.FormKey
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "PreviewTransform",
                    SourcePath = "PreviewTransform",
                    ValueKind = RecordFieldValueKind.FormKey
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "FeaturedItemMessage",
                    SourcePath = "FeaturedItemMessage",
                    ValueKind = RecordFieldValueKind.FormKey
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "XALG",
                    SourcePath = "XALG",
                    ValueKind = RecordFieldValueKind.Number
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Name",
                    SourcePath = "Name",
                    ValueKind = RecordFieldValueKind.LocalizedString,
                    UsesLocalizedDisplay = true
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Text",
                    SourcePath = "Text",
                    ValueKind = RecordFieldValueKind.LocalizedString,
                    UsesLocalizedDisplay = true,
                    Description = "Uses a comparison-service hook because non-Starfield games use BookText as the source field."
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Value",
                    SourcePath = "Value",
                    ValueKind = RecordFieldValueKind.Number
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Weight",
                    SourcePath = "Weight",
                    ValueKind = RecordFieldValueKind.Number
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Flags",
                    SourcePath = "Flags",
                    ValueKind = RecordFieldValueKind.FlagSet
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Teaches.MutagenObjectType",
                    SourcePath = "Teaches.MutagenObjectType",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Teaches.Perk",
                    SourcePath = "Teaches.Perk",
                    ValueKind = RecordFieldValueKind.FormKey
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Teaches.RawContent",
                    SourcePath = "Teaches.RawContent",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "DataSlateType",
                    SourcePath = "DataSlateType",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Description",
                    SourcePath = "Description",
                    ValueKind = RecordFieldValueKind.LocalizedString,
                    UsesLocalizedDisplay = true
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "DataSlateHeaderLeft",
                    SourcePath = "DataSlateHeaderLeft",
                    ValueKind = RecordFieldValueKind.LocalizedString,
                    UsesLocalizedDisplay = true
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "DataSlateHeaderRight",
                    SourcePath = "DataSlateHeaderRight",
                    ValueKind = RecordFieldValueKind.LocalizedString,
                    UsesLocalizedDisplay = true
                }
            ]
        },
        ImplementationNote = "Comparison metadata drives scalar parent rows; keyword, model, sound, script, component, and reflection rows remain strategy-based."
    };

    /// <summary>
    /// Gets the Door record specification.
    /// </summary>
    public static RecordSpecification Door { get; } = new()
    {
        RecordID = "DOOR",
        RecordType = "Door",
        TableName = "Doors",
        FriendlyName = "Door",
        GameSupport = CreateCurrentGameSupport("Doors", "Doors"),
        Import = new RecordImportSpecification
        {
            PluginRecordSetPropertyName = "Doors",
            ImportOrder = 16,
            IsRequired = false
        },
        Reader = CreateReaderSpecification("Doors", "Doors"),
        Fields =
        [
            new RecordFieldSpecification
            {
                FieldName = "ObjectBoundsFirst",
                SpriggitPath = "ObjectBounds.First",
                ValueKind = RecordFieldValueKind.Text,
                Description = "First persisted object-bounds vector text."
            },
            new RecordFieldSpecification
            {
                FieldName = "ObjectBoundsSecond",
                SpriggitPath = "ObjectBounds.Second",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Second persisted object-bounds vector text."
            },
            new RecordFieldSpecification
            {
                FieldName = "Name",
                SpriggitPath = "Name",
                ValueKind = RecordFieldValueKind.LocalizedString,
                IsLocalized = true,
                Description = "Optional translated door display name."
            },
            new RecordFieldSpecification
            {
                FieldName = "Flags",
                SpriggitPath = "Flags",
                ValueKind = RecordFieldValueKind.FlagSet,
                Description = "Formatted door flags."
            },
            new RecordFieldSpecification
            {
                FieldName = "NativeTerminalFormKey",
                SpriggitPath = "NativeTerminalFormKey",
                ValueKind = RecordFieldValueKind.FormKey,
                Description = "Optional native terminal FormKey reference."
            },
            new RecordFieldSpecification
            {
                FieldName = "SoundLevel",
                SpriggitPath = "SoundLevel",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Optional sound-level text."
            },
            new RecordFieldSpecification
            {
                FieldName = "FacingAxisOverride",
                SpriggitPath = "FacingAxisOverride",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Optional facing-axis override text."
            },
            new RecordFieldSpecification
            {
                FieldName = "AnimationGraph",
                SpriggitPath = "AnimationGraph",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Optional animation graph path or identifier."
            },
            new RecordFieldSpecification
            {
                FieldName = "AnimationSkeleton",
                SpriggitPath = "AnimationSkeleton",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Optional animation skeleton path or identifier."
            },
            new RecordFieldSpecification
            {
                FieldName = "AnimationDirectory",
                SpriggitPath = "AnimationDirectory",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Optional animation directory path."
            },
            new RecordFieldSpecification
            {
                FieldName = "AnimationFile",
                SpriggitPath = "AnimationFile",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Optional animation file path."
            }
        ],
        Comparison = new RecordComparisonSpecification
        {
            Fields =
            [
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Version2",
                    SourcePath = "Version2",
                    ValueKind = RecordFieldValueKind.Number
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "ObjectBoundsFirst",
                    SourcePath = "ObjectBoundsFirst",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "ObjectBoundsSecond",
                    SourcePath = "ObjectBoundsSecond",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Name",
                    SourcePath = "Name",
                    ValueKind = RecordFieldValueKind.LocalizedString,
                    UsesLocalizedDisplay = true
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Flags",
                    SourcePath = "Flags",
                    ValueKind = RecordFieldValueKind.FlagSet
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "NativeTerminalFormKey",
                    SourcePath = "NativeTerminalFormKey",
                    ValueKind = RecordFieldValueKind.FormKey
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "SoundLevel",
                    SourcePath = "SoundLevel",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "FacingAxisOverride",
                    SourcePath = "FacingAxisOverride",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "AnimationGraph",
                    SourcePath = "AnimationGraph",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "AnimationSkeleton",
                    SourcePath = "AnimationSkeleton",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "AnimationDirectory",
                    SourcePath = "AnimationDirectory",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "AnimationFile",
                    SourcePath = "AnimationFile",
                    ValueKind = RecordFieldValueKind.Text
                }
            ]
        },
        ImplementationNote = "Comparison metadata drives scalar parent rows; keyword, model, sound, script, component, and reflection rows remain strategy-based."
    };

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
        gamesRequiringFullBinaryMod: new HashSet<SpecificationGame> { SpecificationGame.Fallout4 },
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
    /// <param name="gamesRequiringFullBinaryMod">
    /// The supported games that must read this record family through a full binary Mutagen mod.
    /// </param>
    /// <param name="usesOverlaySafeMod">
    /// A value indicating whether the normal overlay-safe reader path is valid for games without a full-binary
    /// override.
    /// </param>
    /// <param name="isOptionalCollection">
    /// A value indicating whether a missing reader collection is an expected adapter capability gap.
    /// </param>
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
        IReadOnlySet<SpecificationGame>? gamesRequiringFullBinaryMod = null,
        bool usesOverlaySafeMod = true,
        bool isOptionalCollection = false,
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
            Reader = CreateReaderSpecification(
                pluginRecordSetPropertyName,
                pluginRecordSetPropertyName,
                gamesRequiringFullBinaryMod,
                usesOverlaySafeMod,
                isOptionalCollection),
            ImplementationNote = "Import dispatch metadata is active; comparison remains record-specific."
        };
    }

    /// <summary>
    /// Creates reader metadata for the current game-adapter record mapping path.
    /// </summary>
    /// <param name="pluginRecordSetPropertyName">The <c>PluginRecordSetDTO</c> collection property that receives mapped DTOs.</param>
    /// <param name="defaultMutagenCollectionName">The default Mutagen mod collection property read by game adapters.</param>
    /// <param name="gamesRequiringFullBinaryMod">
    /// The supported games that must read this record family through a full binary Mutagen mod.
    /// </param>
    /// <param name="usesOverlaySafeMod">
    /// A value indicating whether the normal overlay-safe reader path is valid for games without a full-binary
    /// override.
    /// </param>
    /// <param name="isOptionalCollection">
    /// A value indicating whether a missing reader collection is an expected adapter capability gap.
    /// </param>
    /// <returns>The reader metadata used as the next specification-driven reader migration target.</returns>
    private static RecordReaderSpecification CreateReaderSpecification(
        string pluginRecordSetPropertyName,
        string defaultMutagenCollectionName,
        IReadOnlySet<SpecificationGame>? gamesRequiringFullBinaryMod = null,
        bool usesOverlaySafeMod = true,
        bool isOptionalCollection = false)
    {
        return new RecordReaderSpecification
        {
            PluginRecordSetPropertyName = pluginRecordSetPropertyName,
            DefaultMutagenCollectionName = defaultMutagenCollectionName,
            GamesRequiringFullBinaryMod = gamesRequiringFullBinaryMod ?? new HashSet<SpecificationGame>(),
            UsesOverlaySafeMod = usesOverlaySafeMod,
            IsOptionalCollection = isOptionalCollection,
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
