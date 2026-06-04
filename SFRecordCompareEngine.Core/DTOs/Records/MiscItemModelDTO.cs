using Mutagen.Bethesda.Plugins;

namespace SFRecordCompareEngine.Core.DTOs.Records;

public class MiscItemModelDTO
{
    public string? File { get; set; }
    public string? TextureFileHashes { get; set; }
    public uint? LightLayer { get; set; }
    public string? Flags { get; set; }
    public float? ColorRemappingIndex { get; set; }
    public string? FlagsVestigial { get; set; }
    public IList<FormKey> MaterialSwaps { get; set; } = new List<FormKey>();
}
