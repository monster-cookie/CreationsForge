using CreationsForge.Core.DTOs.Plugins;

namespace CreationsForge.Starfield.DTOs;

public class StarfieldPluginDTO : PluginDTO
{
    public required string Branch { get; set; }

    public int? InteriorCellCount { get; set; }

    public int? Intv { get; set; }
}
