namespace SFRecordCompareEngine.Core.DTOs.Plugins;

public class PluginResolutionHierarchyDTO
{
    public required string ChildModKey { get; set; }
    public required string HierarchyModKey { get; set; }
    public int? HierarchyLoadOrderIndex { get; set; }
    public int? MasterReferenceIndex { get; set; }
    public bool IsChild { get; set; }
}
