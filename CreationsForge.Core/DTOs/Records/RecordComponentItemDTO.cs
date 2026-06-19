using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.DTOs.Records;

public class RecordComponentItemDTO
{
    public SupportedGame Game { get; set; }

    public ModKeyDTO ModKey { get; set; } = new() { Name = string.Empty, Type = 0, FileName = string.Empty };

    public FormKeyDTO FormKey { get; set; } = new()
    {
        ModKey = new ModKeyDTO { Name = string.Empty, Type = 0, FileName = string.Empty },
        Id = 0
    };

    public string RecordType { get; set; } = string.Empty;

    public int ComponentIndex { get; set; }

    public int ItemIndex { get; set; }

    public double? Unknown1 { get; set; }

    public double? Unknown2 { get; set; }

    public double? Unknown3 { get; set; }

    public double? Unknown4 { get; set; }

    public double? Unknown5 { get; set; }

    public DateTime ImportedAtUTC { get; set; }
}
