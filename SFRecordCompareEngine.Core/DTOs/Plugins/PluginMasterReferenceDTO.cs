namespace SFRecordCompareEngine.Core.DTOs.Plugins;

public class PluginMasterReferenceDTO
{
    public required string ModKey { get; set; }
    public required string ParentModKey { get; set; }
    public int MasterReferenceIndex { get; set; }
    public int? ParentLoadOrderIndex { get; set; }
    public required string ImportedAtUtc { get; set; }
}
