using Mutagen.Bethesda.Plugins;

namespace SFRecordCompareEngine.Core.DTOs.Records;

public class ActivatorDTO
{
    public required ModKey ModKey { get; set; }
    public required string FormID { get; set; }
    public string? Name { get; set; }
    public string? ObjectBounds { get; set; }
    public string? Model { get; set; }
    public string? Destructible { get; set; }
    public required string ImportedAtUtc { get; set; }
}
