using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.DTOs.Records;

public class MiscItemResourceDTO
{
    public required SupportedGame Game { get; set; }

    public required ModKeyDTO ModKey { get; set; }

    public required FormKeyDTO FormKey { get; set; }

    public required FormKeyDTO Resource { get; set; }

    public int ResourceIndex { get; set; }

    public int? Count { get; set; }

    public DateTime ImportedAtUTC { get; set; }
}
