using CreationsForge.Core.DTOs.Records;
using NPoco;

namespace CreationsForge.Core.Models.Database;

[TableName("FormListItems")]
[PrimaryKey("Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID, Item_ModKey_Name, Item_ModKey_Type, Item_ModKey_FileName, Item_FormKey_ID, Item_Index", AutoIncrement = false)]
public class FormListItem
{
    public FormListItem()
    { }

    public FormListItem(FormListItemDTO dto)
    {
        Game = dto.Game.ToString();
        ModKeyName = dto.ModKey.Name;
        ModKeyType = dto.ModKey.Type;
        ModKeyFileName = dto.ModKey.FileName;
        FormKeyModKeyName = dto.FormKey.ModKey.Name;
        FormKeyModKeyType = dto.FormKey.ModKey.Type;
        FormKeyModKeyFileName = dto.FormKey.ModKey.FileName;
        FormKeyId = dto.FormKey.Id;
        ItemModKeyName = dto.Item.ModKey.Name;
        ItemModKeyType = dto.Item.ModKey.Type;
        ItemModKeyFileName = dto.Item.ModKey.FileName;
        ItemFormKeyId = dto.Item.Id;
        ItemIndex = dto.ItemIndex;
        ImportedAtUTC = dto.ImportedAtUTC;
    }

    [Column("Game")] public string Game { get; set; } = string.Empty;

    [Column("ModKey_Name")] public string ModKeyName { get; set; } = string.Empty;

    [Column("ModKey_Type")] public int ModKeyType { get; set; }

    [Column("ModKey_FileName")] public string ModKeyFileName { get; set; } = string.Empty;

    [Column("FormKey_ModKey_Name")] public string FormKeyModKeyName { get; set; } = string.Empty;

    [Column("FormKey_ModKey_Type")] public int FormKeyModKeyType { get; set; }

    [Column("FormKey_ModKey_FileName")] public string FormKeyModKeyFileName { get; set; } = string.Empty;

    [Column("FormKey_ID")] public long FormKeyId { get; set; }

    [Column("Item_ModKey_Name")] public string ItemModKeyName { get; set; } = string.Empty;

    [Column("Item_ModKey_Type")] public int ItemModKeyType { get; set; }

    [Column("Item_ModKey_FileName")] public string ItemModKeyFileName { get; set; } = string.Empty;

    [Column("Item_FormKey_ID")] public long ItemFormKeyId { get; set; }

    [Column("Item_Index")] public int ItemIndex { get; set; }

    [Column("ImportedAtUTC")] public DateTime ImportedAtUTC { get; set; }
}
