using CreationsForge.Core.DTOs.Records;
using NPoco;

namespace CreationsForge.Core.Models.Database;

[TableName("GameSettings")]
[PrimaryKey("Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID", AutoIncrement = false)]
public class GameSetting
{
    public GameSetting()
    { }

    public GameSetting(GameSettingDTO dto)
    {
        Game = dto.Game.ToString();
        ModKeyName = dto.ModKey.Name;
        ModKeyType = dto.ModKey.Type;
        ModKeyFileName = dto.ModKey.FileName;
        FormKeyModKeyName = dto.FormKey.ModKey.Name;
        FormKeyModKeyType = dto.FormKey.ModKey.Type;
        FormKeyModKeyFileName = dto.FormKey.ModKey.FileName;
        FormKeyId = dto.FormKey.Id;
        EditorId = dto.EditorID;
        FormVersion = dto.FormVersion;
        MajorRecordFlags = dto.MajorRecordFlags;
        ImportedAtUTC = dto.ImportedAtUTC;
        SettingType = dto.SettingType;
        Data = dto.Data;
        NumericData = dto.NumericData;
        IntegerData = dto.IntegerData;
        BooleanData = dto.BooleanData.HasValue ? dto.BooleanData.Value ? 1 : 0 : (int?)null;
    }

    [Column("Game")] public string Game { get; set; } = string.Empty;

    [Column("ModKey_Name")] public string ModKeyName { get; set; } = string.Empty;

    [Column("ModKey_Type")] public int ModKeyType { get; set; }

    [Column("ModKey_FileName")] public string ModKeyFileName { get; set; } = string.Empty;

    [Column("FormKey_ModKey_Name")] public string FormKeyModKeyName { get; set; } = string.Empty;

    [Column("FormKey_ModKey_Type")] public int FormKeyModKeyType { get; set; }

    [Column("FormKey_ModKey_FileName")] public string FormKeyModKeyFileName { get; set; } = string.Empty;

    [Column("FormKey_ID")] public long FormKeyId { get; set; }

    [Column("EditorID")] public string EditorId { get; set; } = string.Empty;

    [Column("FormVersion")] public int FormVersion { get; set; }

    [Column("MajorRecordFlags")] public int MajorRecordFlags { get; set; }

    [Column("ImportedAtUTC")] public DateTime ImportedAtUTC { get; set; }

    [Column("SettingType")] public string? SettingType { get; set; }

    [Column("Data")] public string? Data { get; set; }

    [Column("NumericData")] public double? NumericData { get; set; }

    [Column("IntegerData")] public int? IntegerData { get; set; }

    [Column("BooleanData")] public int? BooleanData { get; set; }
}
