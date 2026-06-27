using static CreationsForge.Specification.Records.RecordSpecificationFactory;

namespace CreationsForge.Specification.Records;

/// <summary>
/// Provides the Static record specification metadata.
/// </summary>
internal static class StaticRecordSpecification
{

    /// <summary>
    /// Gets the Static record specification.
    /// </summary>
    internal static RecordSpecification Instance { get; } = new()
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
        ImplementationNote = "Comparison metadata drives scalar parent rows; navmesh, keyword, property, model, " +
            "and reflection rows remain strategy-based."
    };
}

