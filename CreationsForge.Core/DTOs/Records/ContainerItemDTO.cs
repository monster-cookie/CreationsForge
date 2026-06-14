using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.DTOs.Records;

public class ContainerItemDTO
{
    public required SupportedGame Game { get; set; }

    public required ModKeyDTO ModKey { get; set; }

    public required FormKeyDTO FormKey { get; set; }

    public required int ItemIndex { get; set; }

    public required FormKeyDTO ItemFormKey { get; set; }

    public int? Count { get; set; }

    public required DateTime ImportedAtUTC { get; set; }
}
