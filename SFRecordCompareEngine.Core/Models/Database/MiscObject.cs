using Mutagen.Bethesda.Plugins;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Models.Database;

[TableName("MiscObject")]
[PrimaryKey("ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_Id", AutoIncrement = false)]
public class MiscObject
{
    public MiscObject()
    { }

    public MiscObject(MiscObjectDTO dto)
    {
        ModKeyName = dto.ModKey.Name; ModKeyType = (int)dto.ModKey.Type; ModKeyFileName = dto.ModKey.FileName; FormKeyId = (int)dto.FormKey.ID;
        EditorId = dto.EditorID; FormVersion = dto.FormVersion; StarfieldMajorRecordFlags = (int)dto.StarfieldMajorRecordFlags; Version2 = dto.Version2; VersionControl = dto.VersionControl; ImportedAtUTC = dto.ImportedAtUTC;
        Name = dto.Name; ShortName = dto.ShortName; Value = dto.Value; Weight = dto.Weight;
    }

    [Column("ModKey_Name")] public string ModKeyName { get; set; } = string.Empty;
    [Column("ModKey_Type")] public int ModKeyType { get; set; } = (int)ModType.Master;
    [Column("ModKey_FileName")] public string ModKeyFileName { get; set; } = string.Empty;
    [Column("FormKey_ID")] public int FormKeyId { get; set; }
    [Column("EditorID")] public string EditorId { get; set; } = string.Empty;
    [Column("FormVersion")] public int FormVersion { get; set; }
    [Column("StarfieldMajorRecordFlags")] public int StarfieldMajorRecordFlags { get; set; }
    [Column("Version2")] public int Version2 { get; set; }
    [Column("VersionControl")] public int VersionControl { get; set; }
    [Column("ImportedAtUTC")] public DateTime ImportedAtUTC { get; set; }
    [Column("Name")] public string? Name { get; set; }
    [Column("ShortName")] public string? ShortName { get; set; }
    [Column("Value")] public int? Value { get; set; }
    [Column("Weight")] public double? Weight { get; set; }
}
