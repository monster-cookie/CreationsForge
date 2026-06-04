using Mutagen.Bethesda.Plugins;
using NPoco;

namespace SFRecordCompareEngine.Core.Models.Database;

[TableName("MiscItemModels")]
[PrimaryKey("ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID", AutoIncrement = false)]
public class MiscItemModel
{
    [Column("ModKey_Name")] public string ModKeyName { get; set; } = string.Empty;
    [Column("ModKey_Type")] public int ModKeyType { get; set; } = (int)ModType.Master;
    [Column("ModKey_FileName")] public string ModKeyFileName { get; set; } = string.Empty;
    [Column("FormKey_ModKey_Name")] public string FormKeyModKeyName { get; set; } = string.Empty;
    [Column("FormKey_ModKey_Type")] public int FormKeyModKeyType { get; set; } = (int)ModType.Master;
    [Column("FormKey_ModKey_FileName")] public string FormKeyModKeyFileName { get; set; } = string.Empty;
    [Column("FormKey_ID")] public int FormKeyId { get; set; }
    [Column("File")] public string? File { get; set; }
    [Column("TextureFileHashes")] public string? TextureFileHashes { get; set; }
    [Column("LightLayer")] public long? LightLayer { get; set; }
    [Column("Flags")] public string? Flags { get; set; }
    [Column("ColorRemappingIndex")] public float? ColorRemappingIndex { get; set; }
    [Column("FlagsVestigial")] public string? FlagsVestigial { get; set; }
    [Column("ImportedAtUTC")] public DateTime ImportedAtUTC { get; set; }
}
