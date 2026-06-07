using CreationsForge.Core.DTOs.Plugins;

namespace CreationsForge.Core.DTOs.Records;

public class RecordComparisonValueDTO
{
    public required ModKeyDTO ModKey { get; set; }

    public string DisplayValue { get; set; } = string.Empty;

    public RecordComparisonValueState State { get; set; }
}
