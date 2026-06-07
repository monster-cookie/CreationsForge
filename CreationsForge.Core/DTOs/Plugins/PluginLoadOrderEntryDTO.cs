using CreationsForge.Core.Enums;

namespace CreationsForge.Core.DTOs.Plugins;

public class PluginLoadOrderEntryDTO
{
    public required SupportedGame Game { get; init; }

    public required ModKeyDTO ModKey { get; init; }

    public required int LoadOrderIndex { get; init; }

    public required bool Enabled { get; init; }
}
