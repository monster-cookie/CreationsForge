using static CreationsForge.Specification.Records.RecordSpecificationFactory;

namespace CreationsForge.Specification.Records;

/// <summary>
/// Provides the Keyword record specification metadata.
/// </summary>
internal static class KeywordRecordSpecification
{

    /// <summary>
    /// Gets the Keyword record specification.
    /// </summary>
    internal static RecordSpecification Instance { get; } = new()
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
}

