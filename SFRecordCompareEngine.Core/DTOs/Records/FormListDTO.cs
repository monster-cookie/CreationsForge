using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;

namespace SFRecordCompareEngine.Core.DTOs.Records;

public class FormListDTO
{
    public required ModKey ModKey { get; set; }
    public required FormKey FormKey { get; set; }
    public required string EditorID { get; set; }
    public required int FormVersion { get; set; }
    public required StarfieldMajorRecord.StarfieldMajorRecordFlag StarfieldMajorRecordFlags  { get; set; }
    public required int Version2 { get; set; }
    public required int VersionControl { get; set; }
    public required DateTime ImportedAtUtc { get; set; }
   
    // END HEADER
    
    public FormKey? AddToListFormKey { get; set; }
}
