using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.DTOs.Records;

public class ConditionFormConditionDTO
{
    public SupportedGame Game { get; set; }

    public ModKeyDTO ModKey { get; set; } = new() { Name = string.Empty, Type = 0, FileName = string.Empty };

    public FormKeyDTO FormKey { get; set; } = new()
    {
        ModKey = new ModKeyDTO { Name = string.Empty, Type = 0, FileName = string.Empty },
        Id = 0
    };

    public int ConditionIndex { get; set; }

    public string ConditionSlot { get; set; } = "Conditions";

    public string MutagenObjectType { get; set; } = string.Empty;

    public string? DataMutagenObjectType { get; set; }

    public string? CompareOperator { get; set; }

    public string? Flags { get; set; }

    public int? Unknown2 { get; set; }

    public string? ComparisonValue { get; set; }

    public FormKeyDTO? ComparisonValueFormKey { get; set; }

    public DateTime ImportedAtUTC { get; set; }

    public IList<ConditionFormConditionParameterDTO> Parameters { get; set; } = new List<ConditionFormConditionParameterDTO>();
}
