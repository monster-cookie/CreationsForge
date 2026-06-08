using CreationsForge.Core.Enums;

namespace CreationsForge.Core.DTOs.Plugins;

public class PluginMasterReferenceDTO
{
    public required SupportedGame Game { get; set; }

    public required ModKeyDTO MasterModKey { get; set; }

    public required ModKeyDTO PluginModKey { get; set; }

    public required DateTime ImportedAtUTC { get; set; }
}
