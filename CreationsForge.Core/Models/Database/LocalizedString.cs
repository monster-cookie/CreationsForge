using CreationsForge.Core.DTOs.Records;
using NPoco;

namespace CreationsForge.Core.Models.Database;

[TableName("LocalizedStrings")]
[PrimaryKey("Game, ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, SourceField, Language", AutoIncrement = false)]
public class LocalizedString
{
    public LocalizedString()
    { }

    public LocalizedString(LocalizedStringDTO dto)
    {
        Game = dto.Game.ToString();
        ModKeyName = dto.ModKey.Name;
        ModKeyType = dto.ModKey.Type;
        ModKeyFileName = dto.ModKey.FileName;
        RecordType = dto.RecordType;
        FormKeyModKeyName = dto.FormKey.ModKey.Name;
        FormKeyModKeyType = dto.FormKey.ModKey.Type;
        FormKeyModKeyFileName = dto.FormKey.ModKey.FileName;
        FormKeyId = dto.FormKey.Id;
        SourceField = dto.SourceField;
        Language = dto.Language;
        Value = dto.Value;
        ImportedAtUTC = dto.ImportedAtUTC;
    }

    [Column("Game")] public string Game { get; set; } = string.Empty;

    [Column("ModKey_Name")] public string ModKeyName { get; set; } = string.Empty;

    [Column("ModKey_Type")] public int ModKeyType { get; set; }

    [Column("ModKey_FileName")] public string ModKeyFileName { get; set; } = string.Empty;

    [Column("RecordType")] public string RecordType { get; set; } = string.Empty;

    [Column("FormKey_ModKey_Name")] public string FormKeyModKeyName { get; set; } = string.Empty;

    [Column("FormKey_ModKey_Type")] public int FormKeyModKeyType { get; set; }

    [Column("FormKey_ModKey_FileName")] public string FormKeyModKeyFileName { get; set; } = string.Empty;

    [Column("FormKey_ID")] public long FormKeyId { get; set; }

    [Column("SourceField")] public string SourceField { get; set; } = string.Empty;

    [Column("Language")] public string Language { get; set; } = string.Empty;

    [Column("Value")] public string Value { get; set; } = string.Empty;

    [Column("ImportedAtUTC")] public DateTime ImportedAtUTC { get; set; }
}
