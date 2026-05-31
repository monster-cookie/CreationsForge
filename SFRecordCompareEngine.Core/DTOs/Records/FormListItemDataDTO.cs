using Mutagen.Bethesda.Plugins;

namespace SFRecordCompareEngine.Core.DTOs.Records;

public class FormListItemDataDTO
{
    public required ModKey ItemModKey { get; set; }
    public required FormKey ItemFormKey { get; set; }
}