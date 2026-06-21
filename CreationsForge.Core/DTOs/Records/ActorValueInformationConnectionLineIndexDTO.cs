using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.DTOs.Records;

public class ActorValueInformationConnectionLineIndexDTO
{
    public SupportedGame Game { get; set; }

    public ModKeyDTO ModKey { get; set; } = new() { Name = string.Empty, Type = 0, FileName = string.Empty };

    public FormKeyDTO FormKey { get; set; } = new()
    {
        ModKey = new ModKeyDTO { Name = string.Empty, Type = 0, FileName = string.Empty },
        Id = 0
    };

    public int PerkTreeIndex { get; set; }

    public int ConnectionLineIndex { get; set; }

    public int TargetIndex { get; set; }

    public DateTime ImportedAtUTC { get; set; }
}
