using static CreationsForge.Specification.Records.RecordSpecificationFactory;

namespace CreationsForge.Specification.Records;

/// <summary>
/// Provides the Door record specification metadata.
/// </summary>
internal static class DoorRecordSpecification
{

    /// <summary>
    /// Gets the Door record specification.
    /// </summary>
    internal static RecordSpecification Instance { get; } = new()
    {
        RecordID = "DOOR",
        RecordType = "Door",
        TableName = "Doors",
        FriendlyName = "Door",
        GameSupport = CreateCurrentGameSupport("Doors", "Doors"),
        Import = new RecordImportSpecification
        {
            PluginRecordSetPropertyName = "Doors",
            ImportOrder = 16,
            IsRequired = false
        },
        Reader = CreateReaderSpecification("Doors", "Doors"),
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
                Description = "Optional translated door display name."
            },
            new RecordFieldSpecification
            {
                FieldName = "Flags",
                SpriggitPath = "Flags",
                ValueKind = RecordFieldValueKind.FlagSet,
                Description = "Formatted door flags."
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
                FieldName = "SoundLevel",
                SpriggitPath = "SoundLevel",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Optional sound-level text."
            },
            new RecordFieldSpecification
            {
                FieldName = "FacingAxisOverride",
                SpriggitPath = "FacingAxisOverride",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Optional facing-axis override text."
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
                    FieldName = "NativeTerminalFormKey",
                    SourcePath = "NativeTerminalFormKey",
                    ValueKind = RecordFieldValueKind.FormKey
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "SoundLevel",
                    SourcePath = "SoundLevel",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "FacingAxisOverride",
                    SourcePath = "FacingAxisOverride",
                    ValueKind = RecordFieldValueKind.Text
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
                    GroupKind = RecordComparisonChildGroupKind.ReflectionMappings,
                    GroupName = "Reflection",
                    Description = "Shared reflection rows appended after component rows."
                }
            ]
        },
        ImplementationNote = "Comparison metadata drives scalar parent rows and keyword/model/sound/script/" +
            "reflection child-group dispatch; component rows remain strategy-based."
    };
}


