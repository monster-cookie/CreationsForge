namespace SFRecordCompareEngine.Core.DTOs.Plugins;

public class PluginLoadOrderEntryDTO
{
    public required string ModKey { get; init; }
    public required string PluginFileName { get; init; }
    public required string PluginPath { get; init; }
    public int LoadOrderIndex { get; init; }
    public bool Enabled { get; init; } = true;
}
