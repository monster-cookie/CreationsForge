namespace SFRecordCompareEngine.Core.DTOs.Records;

public class RecordReferenceCacheEntryDTO
{
    public string FormKey { get; set; } = string.Empty;
    public string PluginName { get; set; } = string.Empty;
    public string RecordType { get; set; } = string.Empty;
    public string? EditorID { get; set; }
    public string? DisplayName { get; set; }
}
