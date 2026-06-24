using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.DTOs.Records;

public class ModelDTO
{
    public required SupportedGame Game { get; set; }

    public required ModKeyDTO ModKey { get; set; }

    public required string RecordType { get; set; }

    public required FormKeyDTO FormKey { get; set; }

    public required string ModelSlot { get; set; }

    public string ModelGender { get; set; } = string.Empty;

    public string? File { get; set; }

    public string? Data { get; set; }

    public string? TextureFileHashes { get; set; }

    public uint? LightLayer { get; set; }

    public string? Flags { get; set; }

    public float? ColorRemappingIndex { get; set; }

    public string? FlagsVestigial { get; set; }

    public required DateTime ImportedAtUTC { get; set; }

    public IList<ModelMaterialSwapDTO> MaterialSwaps { get; set; } = new List<ModelMaterialSwapDTO>();
}
