using static CreationsForge.Specification.Records.RecordSpecificationFactory;

namespace CreationsForge.Specification.Records;

/// <summary>
/// Provides the Constructible Object record specification metadata.
/// </summary>
internal static class ConstructibleObjectRecordSpecification
{

    /// <summary>
    /// Gets the Constructible Object record specification.
    /// </summary>
    internal static RecordSpecification Instance { get; } = new()
    {
        RecordID = "COBJ",
        RecordType = "ConstructibleObject",
        TableName = "ConstructibleObjects",
        FriendlyName = "Constructible Object",
        GameSupport = CreateCurrentGameSupport("ConstructibleObjects", "ConstructibleObjects"),
        Import = new RecordImportSpecification
        {
            PluginRecordSetPropertyName = "ConstructibleObjects",
            ImportOrder = 13,
            IsRequired = false
        },
        Reader = CreateReaderSpecification("ConstructibleObjects", "ConstructibleObjects"),
        Fields =
        [
            new RecordFieldSpecification
            {
                FieldName = "Description",
                SpriggitPath = "Description",
                ValueKind = RecordFieldValueKind.LocalizedString,
                IsLocalized = true,
                Description = "Optional translated constructible-object description."
            },
            new RecordFieldSpecification
            {
                FieldName = "CreatedObjectFormKey",
                SpriggitPath = "CreatedObjectFormKey",
                ValueKind = RecordFieldValueKind.FormKey,
                Description = "The object produced by the recipe."
            },
            new RecordFieldSpecification
            {
                FieldName = "WorkbenchKeywordFormKey",
                SpriggitPath = "WorkbenchKeywordFormKey",
                ValueKind = RecordFieldValueKind.FormKey,
                Description = "Optional workbench keyword used by the recipe."
            },
            new RecordFieldSpecification
            {
                FieldName = "CreatedObjectCount",
                SpriggitPath = "CreatedObjectCount",
                ValueKind = RecordFieldValueKind.Number,
                Description = "Optional count of objects created by the recipe."
            },
            new RecordFieldSpecification
            {
                FieldName = "AmountProduced",
                SpriggitPath = "AmountProduced",
                ValueKind = RecordFieldValueKind.Number,
                Description = "Optional produced amount retained from the source record."
            },
            new RecordFieldSpecification
            {
                FieldName = "Value",
                SpriggitPath = "Value",
                ValueKind = RecordFieldValueKind.Number,
                Description = "Optional constructible-object value scalar."
            },
            new RecordFieldSpecification
            {
                FieldName = "MenuSortOrder",
                SpriggitPath = "MenuSortOrder",
                ValueKind = RecordFieldValueKind.Number,
                Description = "Optional menu sort order scalar."
            },
            new RecordFieldSpecification
            {
                FieldName = "LearnMethod",
                SpriggitPath = "LearnMethod",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Optional learn-method text."
            },
            new RecordFieldSpecification
            {
                FieldName = "Flags",
                SpriggitPath = "Flags",
                ValueKind = RecordFieldValueKind.FlagSet,
                Description = "Formatted constructible-object flags."
            },
            new RecordFieldSpecification
            {
                FieldName = "MajorFlags",
                SpriggitPath = "MajorFlags",
                ValueKind = RecordFieldValueKind.FlagSet,
                Description = "Named major-record flags persisted for the constructible object."
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
                    FieldName = "Description",
                    SourcePath = "Description",
                    ValueKind = RecordFieldValueKind.LocalizedString,
                    UsesLocalizedDisplay = true
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "CreatedObjectFormKey",
                    SourcePath = "CreatedObjectFormKey",
                    ValueKind = RecordFieldValueKind.FormKey
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "WorkbenchKeywordFormKey",
                    SourcePath = "WorkbenchKeywordFormKey",
                    ValueKind = RecordFieldValueKind.FormKey
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "CreatedObjectCount",
                    SourcePath = "CreatedObjectCount",
                    ValueKind = RecordFieldValueKind.Number
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "AmountProduced",
                    SourcePath = "AmountProduced",
                    ValueKind = RecordFieldValueKind.Number
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Value",
                    SourcePath = "Value",
                    ValueKind = RecordFieldValueKind.Number
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "MenuSortOrder",
                    SourcePath = "MenuSortOrder",
                    ValueKind = RecordFieldValueKind.Number
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "LearnMethod",
                    SourcePath = "LearnMethod",
                    ValueKind = RecordFieldValueKind.Text
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
                }
            ],
            ChildGroups =
            [
                new RecordComparisonChildGroupSpecification
                {
                    GroupKind = RecordComparisonChildGroupKind.ConditionRules,
                    GroupName = "Conditions",
                    Description = "Shared condition-rule rows appended after recipe-filter rows."
                },
                new RecordComparisonChildGroupSpecification
                {
                    GroupKind = RecordComparisonChildGroupKind.SoundMappings,
                    GroupName = "Sounds",
                    Description = "Shared sound mapping rows appended after condition rows."
                },
                new RecordComparisonChildGroupSpecification
                {
                    GroupKind = RecordComparisonChildGroupKind.ScriptingAdapterMappings,
                    GroupName = "Scripts",
                    Description = "Shared scripting adapter rows appended after sound rows."
                }
            ]
        },
        ImplementationNote = "Comparison metadata drives scalar parent rows and condition/sound/script child-group " +
            "dispatch; component, category, and recipe-filter rows remain strategy-based."
    };
}

