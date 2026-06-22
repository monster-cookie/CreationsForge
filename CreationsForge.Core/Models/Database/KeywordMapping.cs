using CreationsForge.Core.DTOs.Records;
using NPoco;

namespace CreationsForge.Core.Models.Database;

[TableName("KeywordMappings")]
[PrimaryKey("Game, ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, Keyword_Index", AutoIncrement = false)]
public class KeywordMapping
{
    public KeywordMapping()
    { }

    public KeywordMapping(KeywordMappingDTO dto)
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
        KeywordModKeyName = dto.Keyword.ModKey.Name;
        KeywordModKeyType = dto.Keyword.ModKey.Type;
        KeywordModKeyFileName = dto.Keyword.ModKey.FileName;
        KeywordFormKeyId = dto.Keyword.Id;
        KeywordIndex = dto.KeywordIndex;
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

    [Column("Keyword_ModKey_Name")] public string KeywordModKeyName { get; set; } = string.Empty;

    [Column("Keyword_ModKey_Type")] public int KeywordModKeyType { get; set; }

    [Column("Keyword_ModKey_FileName")] public string KeywordModKeyFileName { get; set; } = string.Empty;

    [Column("Keyword_FormKey_ID")] public long KeywordFormKeyId { get; set; }

    [Column("Keyword_Index")] public int KeywordIndex { get; set; }

    [Column("ImportedAtUTC")] public DateTime ImportedAtUTC { get; set; }
}
