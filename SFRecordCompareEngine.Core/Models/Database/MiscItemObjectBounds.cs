using Mutagen.Bethesda.Plugins;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Models.Database;

[TableName("MiscItemObjectBounds")]
[PrimaryKey("ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID", AutoIncrement = false)]
public class MiscItemObjectBounds
{
    public MiscItemObjectBounds()
    { }

    public MiscItemObjectBounds(MiscItemDTO parent)
    {
        var dto = parent.ObjectBounds!;
        ModKeyName = parent.ModKey.Name;
        ModKeyType = (int)parent.ModKey.Type;
        ModKeyFileName = parent.ModKey.FileName;
        FormKeyModKeyName = parent.FormKey.ModKey.Name;
        FormKeyModKeyType = (int)parent.FormKey.ModKey.Type;
        FormKeyModKeyFileName = parent.FormKey.ModKey.FileName;
        FormKeyId = (int)parent.FormKey.ID;
        FirstX = dto.FirstX;
        FirstY = dto.FirstY;
        FirstZ = dto.FirstZ;
        SecondX = dto.SecondX;
        SecondY = dto.SecondY;
        SecondZ = dto.SecondZ;
        ImportedAtUTC = parent.ImportedAtUTC;
    }

    [Column("ModKey_Name")] public string ModKeyName { get; set; } = string.Empty;
    [Column("ModKey_Type")] public int ModKeyType { get; set; } = (int)ModType.Master;
    [Column("ModKey_FileName")] public string ModKeyFileName { get; set; } = string.Empty;
    [Column("FormKey_ModKey_Name")] public string FormKeyModKeyName { get; set; } = string.Empty;
    [Column("FormKey_ModKey_Type")] public int FormKeyModKeyType { get; set; } = (int)ModType.Master;
    [Column("FormKey_ModKey_FileName")] public string FormKeyModKeyFileName { get; set; } = string.Empty;
    [Column("FormKey_ID")] public int FormKeyId { get; set; }
    [Column("First_X")] public float FirstX { get; set; }
    [Column("First_Y")] public float FirstY { get; set; }
    [Column("First_Z")] public float FirstZ { get; set; }
    [Column("Second_X")] public float SecondX { get; set; }
    [Column("Second_Y")] public float SecondY { get; set; }
    [Column("Second_Z")] public float SecondZ { get; set; }
    [Column("ImportedAtUTC")] public DateTime ImportedAtUTC { get; set; }
}
