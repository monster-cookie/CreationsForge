namespace SFRecordCompareEngine.Core.DTOs.Plugins;

public class PluginHeaderMetadataDTO
{
    public required string ModKey { get; init; }
    public string? Author { get; init; }
    public int? FormVersion { get; init; }
    public int? HeaderFlags { get; init; }
    public string? Branch { get; init; }
    public int? InteriorCellCount { get; init; }
    public IList<string> MasterModKeys { get; init; } = new List<string>();
}
