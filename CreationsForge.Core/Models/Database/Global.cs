using CreationsForge.Core.DTOs.Records;
using NPoco;

namespace CreationsForge.Core.Models.Database;

[TableName("Globals")]
[PrimaryKey("Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID", AutoIncrement = false)]
public class Global
{
    public Global()
    { }

    public Global(GlobalDTO dto)
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
        Version2 = dto.Version2;
        VersionControl = dto.VersionControl;
        MutagenObjectType = dto.MutagenObjectType;
        MajorFlags = dto.MajorFlags;
        Data = dto.Data;
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

    [Column("Version2")] public int? Version2 { get; set; }

    [Column("VersionControl")] public int? VersionControl { get; set; }

    [Column("MutagenObjectType")] public string? MutagenObjectType { get; set; }

    [Column("MajorFlags")] public string? MajorFlags { get; set; }

    [Column("Data")] public double? Data { get; set; }
}
