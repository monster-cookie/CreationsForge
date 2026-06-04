using Mutagen.Bethesda.Plugins;
using NPoco;

namespace SFRecordCompareEngine.Core.Models.Database;

[TableName("MiscItemModelMaterialSwaps")]
[PrimaryKey("ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, MaterialSwap_Index", AutoIncrement = false)]
public class MiscItemModelMaterialSwap
{
    [Column("ModKey_Name")] public string ModKeyName { get; set; } = string.Empty;
    [Column("ModKey_Type")] public int ModKeyType { get; set; } = (int)ModType.Master;
    [Column("ModKey_FileName")] public string ModKeyFileName { get; set; } = string.Empty;
    [Column("FormKey_ModKey_Name")] public string FormKeyModKeyName { get; set; } = string.Empty;
    [Column("FormKey_ModKey_Type")] public int FormKeyModKeyType { get; set; } = (int)ModType.Master;
    [Column("FormKey_ModKey_FileName")] public string FormKeyModKeyFileName { get; set; } = string.Empty;
    [Column("FormKey_ID")] public int FormKeyId { get; set; }
    [Column("MaterialSwap_FormKey")] public string MaterialSwapFormKey { get; set; } = string.Empty;
    [Column("MaterialSwap_Index")] public int MaterialSwapIndex { get; set; }
    [Column("ImportedAtUTC")] public DateTime ImportedAtUTC { get; set; }
}
