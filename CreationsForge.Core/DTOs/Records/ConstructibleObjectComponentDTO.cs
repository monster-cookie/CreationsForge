using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.DTOs.Records;

public class ConstructibleObjectComponentDTO
{
    public required SupportedGame Game { get; set; }

    public required ModKeyDTO ModKey { get; set; }

    public required FormKeyDTO FormKey { get; set; }

    public required FormKeyDTO ComponentFormKey { get; set; }

    public int ComponentIndex { get; set; }

    public int? Count { get; set; }

    public DateTime ImportedAtUTC { get; set; }
}
