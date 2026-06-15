using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.DTOs.Records;

public class ConstructibleObjectCategoryDTO
{
    public required SupportedGame Game { get; set; }

    public required ModKeyDTO ModKey { get; set; }

    public required FormKeyDTO FormKey { get; set; }

    public required FormKeyDTO CategoryFormKey { get; set; }

    public int CategoryIndex { get; set; }

    public DateTime ImportedAtUTC { get; set; }
}
