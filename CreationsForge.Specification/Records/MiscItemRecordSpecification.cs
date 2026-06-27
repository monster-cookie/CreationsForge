using static CreationsForge.Specification.Records.RecordSpecificationFactory;

namespace CreationsForge.Specification.Records;

/// <summary>
/// Provides the Misc Item record specification metadata.
/// </summary>
internal static class MiscItemRecordSpecification
{

    /// <summary>
    /// Gets the Misc Item record specification.
    /// </summary>
    internal static RecordSpecification Instance { get; } = new()
    {
        RecordID = "MISC",
        RecordType = "MiscItem",
        TableName = "MiscItems",
        FriendlyName = "Misc Item",
        GameSupport = CreateCurrentGameSupport("MiscItems", "MiscItems"),
        Import = new RecordImportSpecification
        {
            PluginRecordSetPropertyName = "MiscItems",
            ImportOrder = 5,
            IsRequired = true
        },
        Reader = CreateReaderSpecification("MiscItems", "MiscItems"),
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
                FieldName = "PreviewTransform",
                SpriggitPath = "PreviewTransform",
                ValueKind = RecordFieldValueKind.FormKey,
                Description = "Optional preview-transform FormKey reference."
            },
            new RecordFieldSpecification
            {
                FieldName = "Name",
                SpriggitPath = "Name",
                ValueKind = RecordFieldValueKind.LocalizedString,
                IsLocalized = true,
                Description = "Optional translated misc item display name."
            },
            new RecordFieldSpecification
            {
                FieldName = "ShortName",
                SpriggitPath = "ShortName",
                ValueKind = RecordFieldValueKind.LocalizedString,
                IsLocalized = true,
                Description = "Optional translated short display name."
            },
            new RecordFieldSpecification
            {
                FieldName = "Value",
                SpriggitPath = "Value",
                ValueKind = RecordFieldValueKind.Number,
                Description = "Misc item value scalar."
            },
            new RecordFieldSpecification
            {
                FieldName = "Weight",
                SpriggitPath = "Weight",
                ValueKind = RecordFieldValueKind.Number,
                Description = "Misc item weight scalar."
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
                FieldName = "FeaturedItemMessage",
                SpriggitPath = "FeaturedItemMessage",
                ValueKind = RecordFieldValueKind.FormKey,
                Description = "Optional featured-item message FormKey reference."
            },
            new RecordFieldSpecification
            {
                FieldName = "Flag",
                SpriggitPath = "Flag",
                ValueKind = RecordFieldValueKind.FlagSet,
                Description = "Formatted misc item flag value."
            }
        ],
        Comparison = new RecordComparisonSpecification
        {
            Fields =
            [
                new RecordComparisonFieldSpecification
                {
                    FieldName = "ObjectBoundsFirst",
                    SourcePath = "ObjectBounds.First",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "ObjectBoundsSecond",
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
                    FieldName = "PreviewTransform",
                    SourcePath = "PreviewTransform",
                    ValueKind = RecordFieldValueKind.FormKey
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
                    FieldName = "ShortName",
                    SourcePath = "ShortName",
                    ValueKind = RecordFieldValueKind.LocalizedString,
                    UsesLocalizedDisplay = true
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
                    FieldName = "DirtinessScale",
                    SourcePath = "DirtinessScale",
                    ValueKind = RecordFieldValueKind.Number
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "FeaturedItemMessage",
                    SourcePath = "FeaturedItemMessage",
                    ValueKind = RecordFieldValueKind.FormKey
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Flag",
                    SourcePath = "Flag",
                    ValueKind = RecordFieldValueKind.FlagSet
                }
            ],
            ChildGroups =
            [
                new RecordComparisonChildGroupSpecification
                {
                    GroupKind = RecordComparisonChildGroupKind.KeywordMappings,
                    GroupName = "Keywords",
                    Description = "Shared keyword mapping rows appended after scalar parent and record-specific rows."
                }
            ]        },
        ImplementationNote = "Comparison metadata drives scalar parent rows; destructible, keyword, model, sound, " +
            "script, component, and resource rows remain strategy-based."
    };
}


