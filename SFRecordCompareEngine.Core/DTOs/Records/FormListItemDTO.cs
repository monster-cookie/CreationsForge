using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.Models.Database;

namespace SFRecordCompareEngine.Core.DTOs.Records;

public class FormListItemDTO
{
    public FormListItemDTO() 
    { }

    [SetsRequiredMembers]
    public FormListItemDTO(FormListItem model)
    {
        ModKey = new ModKey(model.ModKeyName, (ModType)model.ModKeyType);
        FormKey = new FormKey(ModKey, (uint)model.FormKeyID);
        ItemModKey = new ModKey(model.ItemModKeyName, (ModType)model.ItemModKeyType);
        ItemFormKey = new FormKey(ItemModKey, (uint)model.ItemFormKeyID);
        ImportedAtUTC = model.ImportedAtUTC;
    }
    
    public required ModKey ModKey { get; set; }
    public required FormKey FormKey { get; set; }
    public required ModKey ItemModKey { get; set; }
    public required FormKey ItemFormKey { get; set; }
    public required DateTime ImportedAtUTC { get; set; }
}
