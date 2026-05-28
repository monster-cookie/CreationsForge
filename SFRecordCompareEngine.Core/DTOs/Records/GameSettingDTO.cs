using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Starfield;

namespace SFRecordCompareEngine.Core.DTOs.Records;

public class GameSettingDTO
{
    public required ModKey ModKey { get; set; }
    public required FormKey FormKey { get; set; }
    public required string EditorID { get; set; }
    public required int FormVersion { get; set; }
    public required StarfieldMajorRecord.StarfieldMajorRecordFlag StarfieldMajorRecordFlags  { get; set; }
    public required int Version2 { get; set; }
    public required int VersionControl { get; set; }
    public required DateTime ImportedAtUTC { get; set; }

    // END HEADER

    public string? SettingType { get; set; }
    public string? TitleString { get; set; }
    public string? Data { get; set; }
    public double? RawData { get; set; }
    public int? XALG { get; set; }
    public int IsCompressed { get; set; }
    public int IsDeleted { get; set; }
}
