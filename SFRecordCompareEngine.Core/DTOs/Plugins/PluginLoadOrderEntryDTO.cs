using Mutagen.Bethesda.Plugins;

namespace SFRecordCompareEngine.Core.DTOs.Plugins;

public class PluginLoadOrderEntryDTO
{
    public required ModKey ModKey { get; init; }
    public required string PluginFileName { get; init; }
    public required string PluginPath { get; init; }
    public int LoadOrderIndex { get; init; }
    public bool Enabled { get; init; } = true;
}
