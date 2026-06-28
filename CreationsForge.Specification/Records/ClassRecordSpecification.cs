using static CreationsForge.Specification.Records.RecordSpecificationFactory;

namespace CreationsForge.Specification.Records;

/// <summary>
/// Provides the Class record specification metadata.
/// </summary>
internal static class ClassRecordSpecification
{

    /// <summary>
    /// Gets the Class record specification.
    /// </summary>
    internal static RecordSpecification Instance { get; } = new()
    {
        RecordID = "CLAS",
        RecordType = "Class",
        TableName = "Classes",
        FriendlyName = "Class",
        GameSupport = CreateCurrentGameSupport("Classes", "Classes"),
        Import = new RecordImportSpecification
        {
            PluginRecordSetPropertyName = "Classes",
            ImportOrder = 3,
            IsRequired = true
        },
        Reader = CreateReaderSpecification("Classes", "Classes"),
        Fields =
        [
            new RecordFieldSpecification
            {
                FieldName = "Name",
                SpriggitPath = "Name",
                ValueKind = RecordFieldValueKind.LocalizedString,
                IsLocalized = true,
                Description = "Optional translated class display name."
            },
            new RecordFieldSpecification
            {
                FieldName = "Description",
                SpriggitPath = "Description",
                ValueKind = RecordFieldValueKind.LocalizedString,
                IsLocalized = true,
                Description = "Optional translated class description."
            },
            new RecordFieldSpecification
            {
                FieldName = "Teaches",
                SpriggitPath = "Teaches",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Class teaching target text when the source game exposes one."
            },
            new RecordFieldSpecification
            {
                FieldName = "MaxTrainingLevel",
                SpriggitPath = "MaxTrainingLevel",
                ValueKind = RecordFieldValueKind.Number,
                Description = "Maximum training level retained on the class parent row."
            },
            new RecordFieldSpecification
            {
                FieldName = "BleedoutDefault",
                SpriggitPath = "BleedoutDefault",
                ValueKind = RecordFieldValueKind.Number,
                Description = "Optional bleedout default scalar retained on the class parent row."
            },
            new RecordFieldSpecification
            {
                FieldName = "VoicePoints",
                SpriggitPath = "VoicePoints",
                ValueKind = RecordFieldValueKind.Number,
                Description = "Optional voice-points scalar retained on the class parent row."
            },
            new RecordFieldSpecification
            {
                FieldName = "Unknown",
                SpriggitPath = "Unknown",
                ValueKind = RecordFieldValueKind.Number,
                Description = "Optional source scalar retained from the class parent row."
            },
            new RecordFieldSpecification
            {
                FieldName = "Unknown2",
                SpriggitPath = "Unknown2",
                ValueKind = RecordFieldValueKind.Number,
                Description = "Second optional source scalar retained from the class parent row."
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
                    FieldName = "Teaches",
                    SourcePath = "Teaches",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "MaxTrainingLevel",
                    SourcePath = "MaxTrainingLevel",
                    ValueKind = RecordFieldValueKind.Number
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "BleedoutDefault",
                    SourcePath = "BleedoutDefault",
                    ValueKind = RecordFieldValueKind.Number
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "VoicePoints",
                    SourcePath = "VoicePoints",
                    ValueKind = RecordFieldValueKind.Number
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Unknown",
                    SourcePath = "Unknown",
                    ValueKind = RecordFieldValueKind.Number
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Unknown2",
                    SourcePath = "Unknown2",
                    ValueKind = RecordFieldValueKind.Number
                }
            ],
            ChildGroups =
            [
                new RecordComparisonChildGroupSpecification
                {
                    GroupKind = RecordComparisonChildGroupKind.ClassProperties,
                    GroupName = "Properties",
                    Description = "Class property rows keyed by property index."
                },
                new RecordComparisonChildGroupSpecification
                {
                    GroupKind = RecordComparisonChildGroupKind.ClassSkillWeights,
                    GroupName = "SkillWeights",
                    Description = "Class skill-weight rows keyed by weight index."
                },
                new RecordComparisonChildGroupSpecification
                {
                    GroupKind = RecordComparisonChildGroupKind.ClassStatWeights,
                    GroupName = "StatWeights",
                    Description = "Class stat-weight rows keyed by weight index."
                }
            ]
        },
        ImplementationNote = "Comparison metadata drives scalar parent rows and class property/weight child-group " +
            "dispatch."
    };
}
