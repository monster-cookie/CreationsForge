using static CreationsForge.Specification.Records.RecordSpecificationFactory;

namespace CreationsForge.Specification.Records;

/// <summary>
/// Provides the Condition Form record specification metadata.
/// </summary>
internal static class ConditionFormRecordSpecification
{

    /// <summary>
    /// Gets the Condition Form record specification.
    /// </summary>
    internal static RecordSpecification Instance { get; } = new()
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
}

