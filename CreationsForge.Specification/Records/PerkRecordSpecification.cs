using static CreationsForge.Specification.Records.RecordSpecificationFactory;

namespace CreationsForge.Specification.Records;

/// <summary>
/// Provides the Perk record specification metadata.
/// </summary>
internal static class PerkRecordSpecification
{

    /// <summary>
    /// Gets the Perk record specification.
    /// </summary>
    internal static RecordSpecification Instance { get; } = new()
    {
        RecordID = "PERK",
        RecordType = "Perk",
        TableName = "Perks",
        FriendlyName = "Perk",
        GameSupport = CreateCurrentGameSupport("Perks", "Perks"),
        Import = new RecordImportSpecification
        {
            PluginRecordSetPropertyName = "Perks",
            ImportOrder = 10,
            IsRequired = true
        },
        Reader = CreateReaderSpecification("Perks", "Perks"),
        Fields =
        [
            new RecordFieldSpecification
            {
                FieldName = "Name",
                SpriggitPath = "Name",
                ValueKind = RecordFieldValueKind.LocalizedString,
                IsLocalized = true,
                Description = "Optional translated perk display name."
            },
            new RecordFieldSpecification
            {
                FieldName = "Description",
                SpriggitPath = "Description",
                ValueKind = RecordFieldValueKind.LocalizedString,
                IsLocalized = true,
                Description = "Optional translated perk description."
            },
            new RecordFieldSpecification
            {
                FieldName = "Flags",
                SpriggitPath = "Flags",
                ValueKind = RecordFieldValueKind.FlagSet,
                Description = "Formatted perk flags."
            },
            new RecordFieldSpecification
            {
                FieldName = "SkillGroup",
                SpriggitPath = "SkillGroup",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Optional perk skill group text."
            },
            new RecordFieldSpecification
            {
                FieldName = "CrewAssignment",
                SpriggitPath = "CrewAssignment",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Optional perk crew assignment text."
            },
            new RecordFieldSpecification
            {
                FieldName = "PerkIcon",
                SpriggitPath = "PerkIcon",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Optional perk icon identifier."
            },
            new RecordFieldSpecification
            {
                FieldName = "Category",
                SpriggitPath = "Category",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Optional perk category text."
            },
            new RecordFieldSpecification
            {
                FieldName = "RestrictionFormKey",
                SpriggitPath = "RestrictionFormKey",
                ValueKind = RecordFieldValueKind.FormKey,
                Description = "Optional restriction FormKey reference."
            },
            new RecordFieldSpecification
            {
                FieldName = "TrainingFormKey",
                SpriggitPath = "TrainingFormKey",
                ValueKind = RecordFieldValueKind.FormKey,
                Description = "Optional training FormKey reference."
            },
            new RecordFieldSpecification
            {
                FieldName = "Level",
                SpriggitPath = "Level",
                ValueKind = RecordFieldValueKind.Number,
                Description = "Optional perk level scalar."
            },
            new RecordFieldSpecification
            {
                FieldName = "NumRanks",
                SpriggitPath = "NumRanks",
                ValueKind = RecordFieldValueKind.Number,
                Description = "Optional perk rank count scalar."
            },
            new RecordFieldSpecification
            {
                FieldName = "Playable",
                SpriggitPath = "Playable",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Optional playable flag displayed as text."
            },
            new RecordFieldSpecification
            {
                FieldName = "Hidden",
                SpriggitPath = "Hidden",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Optional hidden flag displayed as text."
            },
            new RecordFieldSpecification
            {
                FieldName = "NextPerk",
                SpriggitPath = "NextPerk",
                ValueKind = RecordFieldValueKind.FormKey,
                Description = "Optional next perk FormKey reference."
            },
            new RecordFieldSpecification
            {
                FieldName = "MajorFlags",
                SpriggitPath = "MajorFlags",
                ValueKind = RecordFieldValueKind.FlagSet,
                Description = "Formatted major-record flags when retained on the perk parent row."
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
                    UsesLocalizedDisplay = true
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
                    FieldName = "Flags",
                    SourcePath = "Flags",
                    ValueKind = RecordFieldValueKind.FlagSet
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "SkillGroup",
                    SourcePath = "SkillGroup",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "CrewAssignment",
                    SourcePath = "CrewAssignment",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "PerkIcon",
                    SourcePath = "PerkIcon",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Category",
                    SourcePath = "Category",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "RestrictionFormKey",
                    SourcePath = "RestrictionFormKey",
                    ValueKind = RecordFieldValueKind.FormKey
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "TrainingFormKey",
                    SourcePath = "TrainingFormKey",
                    ValueKind = RecordFieldValueKind.FormKey
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Level",
                    SourcePath = "Level",
                    ValueKind = RecordFieldValueKind.Number
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "NumRanks",
                    SourcePath = "NumRanks",
                    ValueKind = RecordFieldValueKind.Number
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Playable",
                    SourcePath = "Playable",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Hidden",
                    SourcePath = "Hidden",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "NextPerk",
                    SourcePath = "NextPerk",
                    ValueKind = RecordFieldValueKind.FormKey
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "MajorFlags",
                    SourcePath = "MajorFlags",
                    ValueKind = RecordFieldValueKind.FlagSet
                }
            ],
            ChildGroups =
            [
                new RecordComparisonChildGroupSpecification
                {
                    GroupKind = RecordComparisonChildGroupKind.PerkEffects,
                    GroupName = "Effects",
                    Description = "Perk effect rows appended after scalar parent rows."
                },
                new RecordComparisonChildGroupSpecification
                {
                    GroupKind = RecordComparisonChildGroupKind.PerkRanks,
                    GroupName = "Ranks",
                    Description = "Perk rank rows appended after top-level effect rows."
                },
                new RecordComparisonChildGroupSpecification
                {
                    GroupKind = RecordComparisonChildGroupKind.PerkBackgroundSkills,
                    GroupName = "Background Skills",
                    Description = "Perk background skill rows appended after rank rows."
                },
                new RecordComparisonChildGroupSpecification
                {
                    GroupKind = RecordComparisonChildGroupKind.ConditionRules,
                    GroupName = "Conditions",
                    Description = "Shared condition-rule rows appended after background skill rows."
                },
                new RecordComparisonChildGroupSpecification
                {
                    GroupKind = RecordComparisonChildGroupKind.SoundMappings,
                    GroupName = "Sounds",
                    Description = "Shared sound mapping rows appended after condition rows."
                },
                new RecordComparisonChildGroupSpecification
                {
                    GroupKind = RecordComparisonChildGroupKind.ScriptFragments,
                    GroupName = "Script Fragments",
                    Description = "Shared script fragment rows appended after sound rows."
                },
                new RecordComparisonChildGroupSpecification
                {
                    GroupKind = RecordComparisonChildGroupKind.ScriptingAdapterMappings,
                    GroupName = "Scripts",
                    Description = "Shared scripting adapter rows appended after script fragment rows."
                }
            ]
        },
        ImplementationNote = "Comparison metadata drives scalar parent rows and condition/sound/script-fragment/" +
            "script child-group dispatch plus effect, rank, and background skill row dispatch."
    };
}

