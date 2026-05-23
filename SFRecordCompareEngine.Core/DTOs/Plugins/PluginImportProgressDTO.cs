namespace SFRecordCompareEngine.Core.DTOs.Plugins;

public class PluginImportProgressDTO
{
    public string? CurrentPluginName { get; set; }
    public string? CurrentModKey { get; set; }
    public int PluginIndex { get; set; }
    public int PluginCount { get; set; }
    public required string StatusText { get; set; }
    public bool IsIndeterminate { get; set; }
}
