using Mutagen.Bethesda.Plugins;

namespace SFRecordCompareEngine.Core.DTOs.Records;

public class GameplayOptionsGroupDTO
{
    public required ModKey ModKey { get; set; }
    public required string FormID { get; set; }
    public string? Name { get; set; }
    public required string ImportedAtUtc { get; set; }
}
