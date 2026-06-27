using static CreationsForge.Specification.Records.RecordSpecificationFactory;

namespace CreationsForge.Specification.Records;

/// <summary>
/// Provides the Form List record specification metadata.
/// </summary>
internal static class FormListRecordSpecification
{
    /// <summary>
    /// Gets the Form List record specification.
    /// </summary>
    internal static RecordSpecification Instance { get; } = new()
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
}

