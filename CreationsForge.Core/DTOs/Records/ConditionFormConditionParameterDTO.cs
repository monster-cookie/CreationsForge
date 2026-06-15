using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.DTOs.Records;

public class ConditionFormConditionParameterDTO
{
    public SupportedGame Game { get; set; }

    public ModKeyDTO ModKey { get; set; } = new() { Name = string.Empty, Type = 0, FileName = string.Empty };

    public FormKeyDTO FormKey { get; set; } = new()
    {
        ModKey = new ModKeyDTO { Name = string.Empty, Type = 0, FileName = string.Empty },
        Id = 0
    };

    public int ConditionIndex { get; set; }

    public string ParameterName { get; set; } = string.Empty;

    public string? ParameterValue { get; set; }

    public FormKeyDTO? ParameterFormKey { get; set; }

    public DateTime ImportedAtUTC { get; set; }
}
