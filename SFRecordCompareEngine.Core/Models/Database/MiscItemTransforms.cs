using Mutagen.Bethesda.Plugins;
using NPoco;

namespace SFRecordCompareEngine.Core.Models.Database;

[TableName("MiscItemTransforms")]
[PrimaryKey("ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID", AutoIncrement = false)]
public class MiscItemTransforms
{
    [Column("ModKey_Name")] public string ModKeyName { get; set; } = string.Empty;
    [Column("ModKey_Type")] public int ModKeyType { get; set; } = (int)ModType.Master;
    [Column("ModKey_FileName")] public string ModKeyFileName { get; set; } = string.Empty;
    [Column("FormKey_ModKey_Name")] public string FormKeyModKeyName { get; set; } = string.Empty;
    [Column("FormKey_ModKey_Type")] public int FormKeyModKeyType { get; set; } = (int)ModType.Master;
    [Column("FormKey_ModKey_FileName")] public string FormKeyModKeyFileName { get; set; } = string.Empty;
    [Column("FormKey_ID")] public int FormKeyId { get; set; }
    [Column("InventoryIcon_FormKey")] public string? InventoryIconFormKey { get; set; }
    [Column("Outpost_FormKey")] public string? OutpostFormKey { get; set; }
    [Column("Ship_FormKey")] public string? ShipFormKey { get; set; }
    [Column("Preview_FormKey")] public string? PreviewFormKey { get; set; }
    [Column("Inventory_FormKey")] public string? InventoryFormKey { get; set; }
    [Column("Workbench_FormKey")] public string? WorkbenchFormKey { get; set; }
    [Column("MainGameUI_FormKey")] public string? MainGameUIFormKey { get; set; }
    [Column("ImportedAtUTC")] public DateTime ImportedAtUTC { get; set; }
}
