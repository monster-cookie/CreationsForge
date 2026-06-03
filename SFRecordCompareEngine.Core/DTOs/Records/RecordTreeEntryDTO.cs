using Mutagen.Bethesda.Plugins;

namespace SFRecordCompareEngine.Core.DTOs.Records;

public class RecordTreeEntryDTO
{
    public required FormKey FormKey { get; set; }
    public required string EditorID { get; set; }
}
