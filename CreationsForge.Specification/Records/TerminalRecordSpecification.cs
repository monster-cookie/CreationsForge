using static CreationsForge.Specification.Records.RecordSpecificationFactory;

namespace CreationsForge.Specification.Records;

/// <summary>
/// Provides the Terminal record specification metadata.
/// </summary>
internal static class TerminalRecordSpecification
{

    /// <summary>
    /// Gets the Terminal record specification.
    /// </summary>
    internal static RecordSpecification Instance { get; } = new()
    {
        RecordID = "TERM",
        RecordType = "Terminal",
        TableName = "Terminals",
        FriendlyName = "Terminal",
        GameSupport = CreateGameSupport(
            "Terminals",
            "Terminals",
            SpecificationGame.Starfield,
            SpecificationGame.Fallout4),
        Import = new RecordImportSpecification
        {
            PluginRecordSetPropertyName = "Terminals",
            ImportOrder = 17,
            IsRequired = false
        },
        Reader = CreateReaderSpecification(
            "Terminals",
            "Terminals",
            new HashSet<SpecificationGame> { SpecificationGame.Fallout4 }),
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
                FieldName = "MenuFormKey",
                SpriggitPath = "MenuFormKey",
                ValueKind = RecordFieldValueKind.FormKey,
                Description = "Optional terminal menu FormKey reference."
            },
            new RecordFieldSpecification
            {
                FieldName = "Background",
                SpriggitPath = "Background",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Optional background asset or identifier."
            },
            new RecordFieldSpecification
            {
                FieldName = "HeaderText",
                SpriggitPath = "HeaderText",
                ValueKind = RecordFieldValueKind.LocalizedString,
                IsLocalized = true,
                Description = "Optional translated header text."
            },
            new RecordFieldSpecification
            {
                FieldName = "WelcomeText",
                SpriggitPath = "WelcomeText",
                ValueKind = RecordFieldValueKind.LocalizedString,
                IsLocalized = true,
                Description = "Optional translated welcome text."
            },
            new RecordFieldSpecification
            {
                FieldName = "Name",
                SpriggitPath = "Name",
                ValueKind = RecordFieldValueKind.LocalizedString,
                IsLocalized = true,
                Description = "Optional translated terminal display name."
            },
            new RecordFieldSpecification
            {
                FieldName = "Pnam",
                SpriggitPath = "Pnam",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Persisted PNAM scalar text."
            },
            new RecordFieldSpecification
            {
                FieldName = "Fnam",
                SpriggitPath = "Fnam",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Persisted FNAM scalar text."
            },
            new RecordFieldSpecification
            {
                FieldName = "Flags",
                SpriggitPath = "Flags",
                ValueKind = RecordFieldValueKind.FlagSet,
                Description = "Formatted terminal flags."
            },
            new RecordFieldSpecification
            {
                FieldName = "MajorFlags",
                SpriggitPath = "MajorFlags",
                ValueKind = RecordFieldValueKind.FlagSet,
                Description = "Formatted terminal major flags."
            },
            new RecordFieldSpecification
            {
                FieldName = "Jnam",
                SpriggitPath = "Jnam",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Persisted JNAM scalar text."
            },
            new RecordFieldSpecification
            {
                FieldName = "MarkerFlags",
                SpriggitPath = "MarkerFlags",
                ValueKind = RecordFieldValueKind.FlagSet,
                Description = "Marker flag value displayed through the existing hexadecimal formatting hook."
            },
            new RecordFieldSpecification
            {
                FieldName = "Gnam",
                SpriggitPath = "Gnam",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Persisted GNAM scalar text."
            },
            new RecordFieldSpecification
            {
                FieldName = "WorkbenchData",
                SpriggitPath = "WorkbenchData",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Optional workbench data text."
            },
            new RecordFieldSpecification
            {
                FieldName = "FurnitureTemplateFormKey",
                SpriggitPath = "FurnitureTemplateFormKey",
                ValueKind = RecordFieldValueKind.FormKey,
                Description = "Optional furniture template FormKey reference."
            },
            new RecordFieldSpecification
            {
                FieldName = "MarkerModel",
                SpriggitPath = "MarkerModel",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Optional marker model path or identifier."
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
            },
            new RecordFieldSpecification
            {
                FieldName = "ForcedLocations",
                SpriggitPath = "ForcedLocations",
                ValueKind = RecordFieldValueKind.Collection,
                IsCollection = true,
                Description = "Terminal forced-location FormKeys expanded by strategy-based comparison rows."
            },
            new RecordFieldSpecification
            {
                FieldName = "MarkerParameters",
                SpriggitPath = "MarkerParameters",
                ValueKind = RecordFieldValueKind.Collection,
                IsCollection = true,
                Description = "Terminal marker parameter rows expanded by strategy-based comparison rows."
            },
            new RecordFieldSpecification
            {
                FieldName = "BodyTexts",
                SpriggitPath = "BodyTexts",
                ValueKind = RecordFieldValueKind.Collection,
                IsCollection = true,
                Description = "Localized terminal body text rows expanded by strategy-based comparison rows."
            },
            new RecordFieldSpecification
            {
                FieldName = "MenuItems",
                SpriggitPath = "MenuItems",
                ValueKind = RecordFieldValueKind.Collection,
                IsCollection = true,
                Description = "Terminal menu item rows expanded by strategy-based comparison rows."
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
                    FieldName = "MenuFormKey",
                    SourcePath = "MenuFormKey",
                    ValueKind = RecordFieldValueKind.FormKey
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Background",
                    SourcePath = "Background",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "HeaderText",
                    SourcePath = "HeaderText",
                    ValueKind = RecordFieldValueKind.LocalizedString,
                    UsesLocalizedDisplay = true
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "WelcomeText",
                    SourcePath = "WelcomeText",
                    ValueKind = RecordFieldValueKind.LocalizedString,
                    UsesLocalizedDisplay = true
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
                    FieldName = "Pnam",
                    SourcePath = "Pnam",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Fnam",
                    SourcePath = "Fnam",
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
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Jnam",
                    SourcePath = "Jnam",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "MarkerFlags",
                    SourcePath = "MarkerFlags",
                    ValueKind = RecordFieldValueKind.FlagSet,
                    Description = "Displayed through the existing hexadecimal-to-decimal formatting hook."
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "Gnam",
                    SourcePath = "Gnam",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "WorkbenchData",
                    SourcePath = "WorkbenchData",
                    ValueKind = RecordFieldValueKind.Text
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "FurnitureTemplateFormKey",
                    SourcePath = "FurnitureTemplateFormKey",
                    ValueKind = RecordFieldValueKind.FormKey
                },
                new RecordComparisonFieldSpecification
                {
                    FieldName = "MarkerModel",
                    SourcePath = "MarkerModel",
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
                }
            ]
        },
        ImplementationNote = "Comparison metadata drives scalar parent rows and keyword/model child-group dispatch; " +
            "forced-location, script, condition, reflection, marker parameter, body text, and menu item rows remain " +
            "strategy-based."
    };

    /// <summary>
    /// Gets every specification included in the production catalog.
}


