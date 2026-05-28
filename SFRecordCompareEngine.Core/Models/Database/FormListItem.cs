using Mutagen.Bethesda.Plugins;
using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Models.Database;

[TableName("FormListItems")]
[PrimaryKey("ModKey_Name, ModKey_Type, ModKey_FileName, Item_ModKey_Name, Item_ModKey_Type, Item_ModKey_FileName, FormKey_ID", AutoIncrement = false)]
public class FormListItem
{
    public FormListItem()
    { }
    
    public FormListItem(FormListItemDTO dto)
    {
        ModKeyName = dto.ModKey.Name;
        ModKeyType = (int)dto.ModKey.Type;
        ModKeyFileName = dto.ModKey.FileName;
        FormKeyID = (int)dto.FormKey.ID;
        ItemModKeyName = dto.ItemModKey.Name;
        ItemModKeyType = (int)dto.ItemModKey.Type;
        ItemModKeyFileName = dto.ItemModKey.FileName;
        ItemFormKeyID = (int)dto.ItemFormKey.ID;
        ImportedAtUTC = dto.ImportedAtUTC;
    }
    
    [Column("ModKey_Name")]
    public string ModKeyName { get; set; } = string.Empty;

    [Column("ModKey_Type")]
    public int ModKeyType { get; set; } = (int)ModType.Master;

    [Column("ModKey_FileName")]
    public string ModKeyFileName { get; set; } = string.Empty;

    [Column("FormKey_ID")]
    public int FormKeyID { get; set; }
    
    [Column("Item_ModKey_Name")]
    public string ItemModKeyName { get; set; } = string.Empty;

    [Column("Item_ModKey_Type")]
    public int ItemModKeyType { get; set; } = (int)ModType.Master;

    [Column("Item_ModKey_FileName")]
    public string ItemModKeyFileName { get; set; } = string.Empty;

    [Column("Item_FormKey_ID")]
    public int ItemFormKeyID { get; set; }

    [Column("ImportedAtUTC")]
    public DateTime ImportedAtUTC { get; set; }
}
