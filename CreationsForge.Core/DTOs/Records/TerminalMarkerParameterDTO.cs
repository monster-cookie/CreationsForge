using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.DTOs.Records;

public class TerminalMarkerParameterDTO
{
    public SupportedGame Game { get; set; }

    public ModKeyDTO ModKey { get; set; } = new() { Name = string.Empty, Type = 0, FileName = string.Empty };

    public FormKeyDTO FormKey { get; set; } = new()
    {
        ModKey = new ModKeyDTO { Name = string.Empty, Type = 0, FileName = string.Empty },
        Id = 0
    };

    public int ParameterIndex { get; set; }

    public bool? Enabled { get; set; }

    public string? Offset { get; set; }

    public string? EntryTypes { get; set; }

    public string? ExitTypes { get; set; }

    public string? Unknown { get; set; }

    public DateTime ImportedAtUTC { get; set; }
}
