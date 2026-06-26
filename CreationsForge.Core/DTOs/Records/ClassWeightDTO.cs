using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.DTOs.Records;

public class ClassWeightDTO
{
    public SupportedGame Game { get; set; }

    public ModKeyDTO ModKey { get; set; } = new() { Name = string.Empty, Type = 0, FileName = string.Empty };

    public FormKeyDTO FormKey { get; set; } = new()
    {
        ModKey = new ModKeyDTO { Name = string.Empty, Type = 0, FileName = string.Empty },
        Id = 0
    };

    public string WeightType { get; set; } = string.Empty;

    public int WeightIndex { get; set; }

    public string Key { get; set; } = string.Empty;

    public double? Value { get; set; }

    public DateTime ImportedAtUTC { get; set; }
}
