using static CreationsForge.Specification.Records.RecordSpecificationFactory;

namespace CreationsForge.Specification.Records;

/// <summary>
/// Provides the Container record specification metadata.
/// </summary>
internal static class ContainerRecordSpecification
{

    /// <summary>
    /// Gets the Container record specification.
    /// </summary>
    internal static RecordSpecification Instance { get; } = new()
    {
        RecordID = "CONT",
        RecordType = "Container",
        TableName = "Containers",
        FriendlyName = "Container",
        GameSupport = CreateCurrentGameSupport("Containers", "Containers"),
        Import = new RecordImportSpecification
        {
            PluginRecordSetPropertyName = "Containers",
            ImportOrder = 12,
            IsRequired = false
        },
        Reader = CreateReaderSpecification("Containers", "Containers"),
        Fields =
        [
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
                FieldName = "Name",
                SpriggitPath = "Name",
                ValueKind = RecordFieldValueKind.LocalizedString,
                IsLocalized = true,
                Description = "Optional translated container display name."
            },
            new RecordFieldSpecification
            {
                FieldName = "Flags",
                SpriggitPath = "Flags",
                ValueKind = RecordFieldValueKind.FlagSet,
                Description = "Formatted container flags."
            },
            new RecordFieldSpecification
            {
                FieldName = "MajorFlags",
                SpriggitPath = "MajorFlags",
                ValueKind = RecordFieldValueKind.FlagSet,
                Description = "Named major-record flags persisted for the container."
            },
            new RecordFieldSpecification
            {
                FieldName = "NativeTerminalFormKey",
                SpriggitPath = "NativeTerminalFormKey",
                ValueKind = RecordFieldValueKind.FormKey,
                Description = "Optional native terminal FormKey reference."
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
                FieldName = "ContainsOnlyFilter",
                SpriggitPath = "ContainsOnlyFilter",
                ValueKind = RecordFieldValueKind.FormKey,
                Description = "Optional contains-only filter FormKey reference."
            },
            new RecordFieldSpecification
            {
                FieldName = "Transforms",
                SpriggitPath = "Transforms",
                ValueKind = RecordFieldValueKind.Object,
                Description = "Optional transform FormKeys represented by scalar comparison rows."
            },
            new RecordFieldSpecification
            {
                FieldName = "AnimationGraph",
                SpriggitPath = "AnimationGraph",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Optional animation graph path or identifier."
            },
            new RecordFieldSpecification
            {
                FieldName = "AnimationSkeleton",
                SpriggitPath = "AnimationSkeleton",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Optional animation skeleton path or identifier."
            },
            new RecordFieldSpecification
            {
                FieldName = "AnimationDirectory",
                SpriggitPath = "AnimationDirectory",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Optional animation directory path."
            },
            new RecordFieldSpecification
            {
                FieldName = "AnimationFile",
                SpriggitPath = "AnimationFile",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Optional animation file path."
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
                    FieldName = "Name",
                    SourcePath = "Name",
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
                    FieldName = "MajorFlags",
                    SourcePath = "MajorFlags",
                    ValueKind = RecordFieldValueKind.FlagSet
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "NativeTerminalFormKey",
                    SourcePath = "NativeTerminalFormKey",
                    ValueKind = RecordFieldValueKind.FormKey
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "SnapTemplate",
                    SourcePath = "SnapTemplate",
                    ValueKind = RecordFieldValueKind.FormKey
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "ContainsOnlyFilter",
                    SourcePath = "ContainsOnlyFilter",
                    ValueKind = RecordFieldValueKind.FormKey
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Transforms.Outpost",
                    SourcePath = "Transforms.Outpost",
                    ValueKind = RecordFieldValueKind.FormKey
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Transforms.Preview",
                    SourcePath = "Transforms.Preview",
                    ValueKind = RecordFieldValueKind.FormKey
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "AnimationGraph",
                    SourcePath = "AnimationGraph",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "AnimationSkeleton",
                    SourcePath = "AnimationSkeleton",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "AnimationDirectory",
                    SourcePath = "AnimationDirectory",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "AnimationFile",
                    SourcePath = "AnimationFile",
                    ValueKind = RecordFieldValueKind.Text
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
                    GroupKind = RecordComparisonChildGroupKind.SoundMappings,
                    GroupName = "Sounds",
                    Description = "Shared sound mapping rows appended after model rows."
                }
            ]
        },
        ImplementationNote = "Comparison metadata drives scalar parent rows and keyword/sound child-group dispatch; " +
            "item, property, forced-location, model, script, component, and reflection rows remain strategy-based."
    };
}


