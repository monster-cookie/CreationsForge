using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.DTOs.Records;

public class ModelMaterialSwapDTO
{
    public required SupportedGame Game { get; set; }

    public required ModKeyDTO ModKey { get; set; }

    public required string RecordType { get; set; }

    public required FormKeyDTO FormKey { get; set; }

    public required string ModelSlot { get; set; }

    public string ModelGender { get; set; } = string.Empty;

    public required FormKeyDTO MaterialSwapFormKey { get; set; }

    public required int MaterialSwapIndex { get; set; }

    public required DateTime ImportedAtUTC { get; set; }
}
