using Mutagen.Bethesda.Plugins;

namespace SFRecordCompareEngine.Core.DTOs.Records;

public class MiscItemSoundDTO
{
    public string? Start { get; set; }
    public string? Stop { get; set; }
    public FormKey? ConditionFormKey { get; set; }
    public FormKey? EventMappingFormKey { get; set; }
}
