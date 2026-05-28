using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;

namespace SFRecordCompareEngine.Core.DTOs.Records;

public class FormListRecordDataDTO
{
    public required FormKey FormKey { get; set; }
    public required string EditorID { get; set; }
    public required int FormVersion { get; set; }
    public required StarfieldMajorRecord.StarfieldMajorRecordFlag StarfieldMajorRecordFlags  { get; set; }
    public required int Version2 { get; set; }
    public required int VersionControl { get; set; }
    public FormKey? AddToListFormKey { get; set; }
    public IReadOnlyList<FormListItemDataDTO> Items { get; set; } = new List<FormListItemDataDTO>();
}
