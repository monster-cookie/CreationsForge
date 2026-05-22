namespace SFRecordCompareEngine.Core.DTOs.Plugins;

public class PluginMetadataDTO
{
    public required string ModKey { get; set; }
    public required string GameRelease { get; set; }
    public int? LoadOrderIndex { get; set; }
    public required string PluginFileName { get; set; }
    public string? PluginPath { get; set; }
    public bool Enabled { get; set; } = true;
    public bool ExistsOnDisk { get; set; } = true;
    public string ImportState { get; set; } = PluginImportState.Current.ToString();
    public int? HeaderFlags { get; set; }
    public int? FormVersion { get; set; }
    public string? Author { get; set; }
    public string? Branch { get; set; }
    public int? InteriorCellCount { get; set; }
    public long? SourceLastWriteUtcTicks { get; set; }
    public long? SourceFileSizeBytes { get; set; }
    public required string LastCheckedUtc { get; set; }
    public string? LastImportedUtc { get; set; }
    public string? InvalidatedAtUtc { get; set; }
}
