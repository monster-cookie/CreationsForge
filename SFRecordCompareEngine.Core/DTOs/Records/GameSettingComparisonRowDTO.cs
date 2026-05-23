namespace SFRecordCompareEngine.Core.DTOs.Records;

public class GameSettingComparisonRowDTO : GameSettingDTO
{
    public required string PluginName { get; set; }
    public string? FormKey { get; set; }
    public string? EditorID { get; set; }
    public int? HierarchyLoadOrderIndex { get; set; }
    public bool HasRecord { get; set; }
}
