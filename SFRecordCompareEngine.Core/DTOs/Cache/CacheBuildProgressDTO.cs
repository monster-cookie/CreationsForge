namespace SFRecordCompareEngine.Core.DTOs.Cache;

public class CacheBuildProgressDTO
{
    public string? CurrentPluginName { get; set; }
    public int ProcessedPlugins { get; set; }
    public int TotalPlugins { get; set; }
    public string Message { get; set; } = string.Empty;
}
