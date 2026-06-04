using Mutagen.Bethesda.Plugins;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Models.Database;

[TableName("MiscItem")]
[PrimaryKey("ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID", AutoIncrement = false)]
public class MiscItem
{
    public MiscItem()
    { }

    public MiscItem(MiscItemDTO dto)
    {
        ModKeyName = dto.ModKey.Name;
        ModKeyType = (int)dto.ModKey.Type;
        ModKeyFileName = dto.ModKey.FileName;
        FormKeyModKeyName = dto.FormKey.ModKey.Name;
        FormKeyModKeyType = (int)dto.FormKey.ModKey.Type;
        FormKeyModKeyFileName = dto.FormKey.ModKey.FileName;
        FormKeyId = (int)dto.FormKey.ID;
        EditorId = dto.EditorID;
        FormVersion = dto.FormVersion;
        StarfieldMajorRecordFlags = (int)dto.StarfieldMajorRecordFlags;
        Version2 = dto.Version2;
        VersionControl = dto.VersionControl;
        ImportedAtUTC = dto.ImportedAtUTC;
        Name = dto.Name;
        ShortName = dto.ShortName;
        Value = dto.Value;
        Weight = dto.Weight;
        DirtinessScale = dto.DirtinessScale;
        Flag = dto.Flag;
        if (dto.FeaturedItemMessageFormKey.HasValue)
        {
            FeaturedItemMessageModKeyName = dto.FeaturedItemMessageFormKey.Value.ModKey.Name;
            FeaturedItemMessageModKeyType = (int)dto.FeaturedItemMessageFormKey.Value.ModKey.Type;
            FeaturedItemMessageModKeyFileName = dto.FeaturedItemMessageFormKey.Value.ModKey.FileName;
            FeaturedItemMessageFormKeyId = (int)dto.FeaturedItemMessageFormKey.Value.ID;
        }
    }

    [Column("ModKey_Name")] public string ModKeyName { get; set; } = string.Empty;
    [Column("ModKey_Type")] public int ModKeyType { get; set; } = (int)ModType.Master;
    [Column("ModKey_FileName")] public string ModKeyFileName { get; set; } = string.Empty;
    [Column("FormKey_ModKey_Name")] public string FormKeyModKeyName { get; set; } = string.Empty;
    [Column("FormKey_ModKey_Type")] public int FormKeyModKeyType { get; set; } = (int)ModType.Master;
    [Column("FormKey_ModKey_FileName")] public string FormKeyModKeyFileName { get; set; } = string.Empty;
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
    [Column("DirtinessScale")] public float? DirtinessScale { get; set; }
    [Column("FeaturedItemMessage_ModKey_Name")] public string? FeaturedItemMessageModKeyName { get; set; }
    [Column("FeaturedItemMessage_ModKey_Type")] public int? FeaturedItemMessageModKeyType { get; set; }
    [Column("FeaturedItemMessage_ModKey_FileName")] public string? FeaturedItemMessageModKeyFileName { get; set; }
    [Column("FeaturedItemMessage_FormKey_ID")] public int? FeaturedItemMessageFormKeyId { get; set; }
    [Column("FLAG")] public string? Flag { get; set; }
}
