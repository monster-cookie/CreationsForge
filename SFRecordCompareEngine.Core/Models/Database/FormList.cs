using Mutagen.Bethesda.Plugins;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Models.Database;

[TableName("FormList")]
[PrimaryKey("ModKeyName, ModKeyType, ModKeyFileName, FormKeyId", AutoIncrement = false)]
public class FormList
{
    public FormList()
    { }

    public FormList(FormListDTO dto)
    {
        ModKeyName = dto.ModKey.Name;
        ModKeyType = (int)dto.ModKey.Type;
        ModKeyFileName = dto.ModKey.FileName;
        FormKeyId = (int)dto.FormKey.ID;
        EditorId = dto.EditorID;
        FormVersion = dto.FormVersion;
        StarfieldMajorRecordFlags = (int)dto.StarfieldMajorRecordFlags;
        Version2 = dto.Version2;
        VersionControl = dto.VersionControl;
        ImportedAtUtc = dto.ImportedAtUtc;
        AddToListFormKey = dto.AddToListFormKey?.ToString();
    }

    [Column("ModKey_Name")]
    public string ModKeyName { get; set; } = string.Empty;

    [Column("ModKey_Type")]
    public int ModKeyType { get; set; } = (int)ModType.Master;

    [Column("ModKey_FileName")]
    public string ModKeyFileName { get; set; } = string.Empty;

    [Column("FormKey_ID")]
    public int FormKeyId { get; set; }

    [Column("EditorID")]
    public string? EditorId { get; set; }

    [Column("FormVersion")]
    public int FormVersion { get; set; }

    [Column("StarfieldMajorRecordFlags")]
    public int StarfieldMajorRecordFlags { get; set; }

    [Column("Version2")]
    public int Version2 { get; set; }

    [Column("VersionControl")]
    public int VersionControl { get; set; }

    [Column("ImportedAtUtc")]
    public DateTime ImportedAtUtc { get; set; }

    [Column("AddToListFormKey")]
    public string? AddToListFormKey { get; set; }
}
