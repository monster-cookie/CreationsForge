using static CreationsForge.Specification.Records.RecordSpecificationFactory;

namespace CreationsForge.Specification.Records;

/// <summary>
/// Provides the Global record specification metadata.
/// </summary>
internal static class GlobalRecordSpecification
{

    /// <summary>
    /// Gets the Global record specification.
    /// </summary>
    internal static RecordSpecification Instance { get; } = new()
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
}

