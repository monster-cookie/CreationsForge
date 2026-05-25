using Mutagen.Bethesda.Plugins;

namespace SFRecordCompareEngine.Core.DTOs.Records;

public class RecordHeaderDTO
{
    public required ModKey ModKey { get; set; }
    public required string FormID { get; set; }
    public required string RecordType { get; set; }
    public required string FormKey { get; set; }
    public string? EditorID { get; set; }
    public required string PluginFileName { get; set; }
    public int? FormVersion { get; set; }
    public int? StarfieldMajorRecordFlags { get; set; }
    public int? Version2 { get; set; }
    public string? VersionControl { get; set; }
    public required string ImportedAtUtc { get; set; }
}
