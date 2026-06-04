using Mutagen.Bethesda.Plugins;

namespace SFRecordCompareEngine.Core.DTOs.Records;

public class MiscItemDestructibleResistanceDTO
{
    public required FormKey DamageTypeFormKey { get; set; }
    public required uint Value { get; set; }
    public required int ResistanceIndex { get; set; }
}
