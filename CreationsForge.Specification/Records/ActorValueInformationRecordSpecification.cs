using static CreationsForge.Specification.Records.RecordSpecificationFactory;

namespace CreationsForge.Specification.Records;

/// <summary>
/// Provides the Actor Value Information record specification metadata.
/// </summary>
internal static class ActorValueInformationRecordSpecification
{

    /// <summary>
    /// Gets the Actor Value Information record specification.
    /// </summary>
    internal static RecordSpecification Instance { get; } = new()
    {
        RecordID = "AVIF",
        RecordType = "ActorValueInformation",
        TableName = "ActorValueInformation",
        FriendlyName = "Actor Value Information",
        GameSupport = CreateCurrentGameSupport("ActorValueInformation", "ActorValueInformation"),
        Import = new RecordImportSpecification
        {
            PluginRecordSetPropertyName = "ActorValueInformation",
            ImportOrder = 7,
            IsRequired = true
        },
        Reader = CreateReaderSpecification("ActorValueInformation", "ActorValueInformation"),
        Fields =
        [
            new RecordFieldSpecification
            {
                FieldName = "Name",
                SpriggitPath = "Name",
                ValueKind = RecordFieldValueKind.LocalizedString,
                IsLocalized = true,
                Description = "Optional translated actor value display name."
            },
            new RecordFieldSpecification
            {
                FieldName = "Abbreviation",
                SpriggitPath = "Abbreviation",
                ValueKind = RecordFieldValueKind.LocalizedString,
                IsLocalized = true,
                Description = "Optional translated actor value abbreviation."
            },
            new RecordFieldSpecification
            {
                FieldName = "Description",
                SpriggitPath = "Description",
                ValueKind = RecordFieldValueKind.LocalizedString,
                IsLocalized = true,
                Description = "Optional translated actor value description."
            },
            new RecordFieldSpecification
            {
                FieldName = "CNAM",
                SpriggitPath = "CNAM",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Optional CNAM text retained on the actor value information parent row."
            },
            new RecordFieldSpecification
            {
                FieldName = "Skill",
                SpriggitPath = "Skill",
                ValueKind = RecordFieldValueKind.Object,
                Description = "Optional skill data represented by scalar comparison rows."
            },
            new RecordFieldSpecification
            {
                FieldName = "ContextNotes",
                SpriggitPath = "ContextNotes",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Optional context notes retained on the actor value information parent row."
            },
            new RecordFieldSpecification
            {
                FieldName = "DefaultValue",
                SpriggitPath = "DefaultValue",
                ValueKind = RecordFieldValueKind.Number,
                Description = "Default actor value scalar."
            },
            new RecordFieldSpecification
            {
                FieldName = "Flags",
                SpriggitPath = "Flags",
                ValueKind = RecordFieldValueKind.FlagSet,
                Description = "Formatted actor value flags."
            },
            new RecordFieldSpecification
            {
                FieldName = "Type",
                SpriggitPath = "Type",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Actor value type text."
            },
            new RecordFieldSpecification
            {
                FieldName = "Min",
                SpriggitPath = "Min",
                ValueKind = RecordFieldValueKind.Number,
                Description = "Minimum actor value scalar."
            },
            new RecordFieldSpecification
            {
                FieldName = "Max",
                SpriggitPath = "Max",
                ValueKind = RecordFieldValueKind.Number,
                Description = "Maximum actor value scalar."
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
                    UsesLocalizedDisplay = true
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Abbreviation",
                    SourcePath = "Abbreviation",
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
                    FieldName = "CNAM",
                    SourcePath = "CNAM",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Skill.ImproveMult",
                    SourcePath = "Skill.ImproveMult",
                    ValueKind = RecordFieldValueKind.Number
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Skill.ImproveOffset",
                    SourcePath = "Skill.ImproveOffset",
                    ValueKind = RecordFieldValueKind.Number
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Skill.UseMult",
                    SourcePath = "Skill.UseMult",
                    ValueKind = RecordFieldValueKind.Number
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "ContextNotes",
                    SourcePath = "ContextNotes",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "DefaultValue",
                    SourcePath = "DefaultValue",
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
                    FieldName = "Type",
                    SourcePath = "Type",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Min",
                    SourcePath = "Min",
                    ValueKind = RecordFieldValueKind.Number
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Max",
                    SourcePath = "Max",
                    ValueKind = RecordFieldValueKind.Number
                }
            ]
        },
        ImplementationNote = "Comparison metadata drives scalar parent rows; perk-tree rows remain strategy-based."
    };
}

