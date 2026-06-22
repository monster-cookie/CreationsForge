using CreationsForge.Core.DTOs.Records;
using NPoco;

namespace CreationsForge.Core.Models.Database;

[TableName("FormLists")]
[PrimaryKey("Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID", AutoIncrement = false)]
public class FormList
{
    public FormList()
    { }

    public FormList(FormListDTO dto)
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
        Version2 = dto.Version2;
        VersionControl = dto.VersionControl;
        ImportedAtUTC = dto.ImportedAtUTC;
        AddToListModKeyName = dto.AddToList?.ModKey.Name;
        AddToListModKeyType = dto.AddToList?.ModKey.Type;
        AddToListModKeyFileName = dto.AddToList?.ModKey.FileName;
        AddToListFormKeyId = dto.AddToList?.Id;
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

    [Column("Version2")] public int? Version2 { get; set; }

    [Column("VersionControl")] public int? VersionControl { get; set; }

    [Column("ImportedAtUTC")] public DateTime ImportedAtUTC { get; set; }

    [Column("AddToList_ModKey_Name")] public string? AddToListModKeyName { get; set; }

    [Column("AddToList_ModKey_Type")] public int? AddToListModKeyType { get; set; }

    [Column("AddToList_ModKey_FileName")] public string? AddToListModKeyFileName { get; set; }

    [Column("AddToList_FormKey_ID")] public long? AddToListFormKeyId { get; set; }
}
