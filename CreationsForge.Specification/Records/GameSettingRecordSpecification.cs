using static CreationsForge.Specification.Records.RecordSpecificationFactory;

namespace CreationsForge.Specification.Records;

/// <summary>
/// Provides the Game Setting record specification metadata.
/// </summary>
internal static class GameSettingRecordSpecification
{

    /// <summary>
    /// Gets the Game Setting record specification.
    /// </summary>
    internal static RecordSpecification Instance { get; } = new()
    {
        RecordID = "GMST",
        RecordType = "GameSetting",
        TableName = "GameSettings",
        FriendlyName = "Game Setting",
        GameSupport = CreateCurrentGameSupport("GameSettings", "GameSettings"),
        Import = new RecordImportSpecification
        {
            PluginRecordSetPropertyName = "GameSettings",
            ImportOrder = 1,
            IsRequired = true
        },
        Reader = CreateReaderSpecification("GameSettings", "GameSettings"),
        Fields =
        [
            new RecordFieldSpecification
            {
                FieldName = "MutagenObjectType",
                SpriggitPath = "MutagenObjectType",
                ValueKind = RecordFieldValueKind.Text,
                Description = "Concrete Mutagen game-setting type used to preserve setting value semantics."
            },
            new RecordFieldSpecification
            {
                FieldName = "Data",
                SpriggitPath = "Data",
                ValueKind = RecordFieldValueKind.Object,
                Description = "Typed scalar or localized string setting value."
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
                    FieldName = "Data",
                    SourcePath = "Data",
                    ValueKind = RecordFieldValueKind.Object,
                    UsesLocalizedDisplay = true,
                    Description = "String settings resolve localized display text; scalar settings use typed value formatting."
                }
            ]
        },
        ImplementationNote = "Comparison metadata drives the simple rows; Data display still uses a localized-value strategy."
    };
}

