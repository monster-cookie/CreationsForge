using static CreationsForge.Specification.Records.RecordSpecificationFactory;

namespace CreationsForge.Specification.Records;

/// <summary>
/// Provides the Book record specification metadata.
/// </summary>
internal static class BookRecordSpecification
{

    /// <summary>
    /// Gets the Book record specification.
    /// </summary>
    internal static RecordSpecification Instance { get; } = new()
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
            ],
            ChildGroups =
            [
                new RecordComparisonChildGroupSpecification
                {
                    GroupKind = RecordComparisonChildGroupKind.KeywordMappings,
                    GroupName = "Keywords",
                    Description = "Shared keyword mapping rows appended after scalar parent and record-specific rows."
                },
                new RecordComparisonChildGroupSpecification
                {
                    GroupKind = RecordComparisonChildGroupKind.ModelMappings,
                    GroupName = "Models",
                    Description = "Shared model rows appended after keyword rows."
                },
                new RecordComparisonChildGroupSpecification
                {
                    GroupKind = RecordComparisonChildGroupKind.SoundMappings,
                    GroupName = "Sounds",
                    Description = "Shared sound mapping rows appended after model rows."
                },
                new RecordComparisonChildGroupSpecification
                {
                    GroupKind = RecordComparisonChildGroupKind.ScriptingAdapterMappings,
                    GroupName = "Scripts",
                    Description = "Shared scripting adapter rows appended after sound rows."
                },
                new RecordComparisonChildGroupSpecification
                {
                    GroupKind = RecordComparisonChildGroupKind.RecordComponents,
                    GroupName = "Components",
                    Description = "Shared record component rows appended after scripting adapter rows."
                },
                new RecordComparisonChildGroupSpecification
                {
                    GroupKind = RecordComparisonChildGroupKind.ReflectionMappings,
                    GroupName = "Reflection",
                    Description = "Shared reflection rows appended after component rows."
                }
            ]
        },
        ImplementationNote = "Comparison metadata drives scalar parent rows and keyword/model/sound/script/" +
            "component/reflection child-group dispatch."
    };
}


