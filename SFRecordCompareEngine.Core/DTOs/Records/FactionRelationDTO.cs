using Mutagen.Bethesda.Plugins;

namespace SFRecordCompareEngine.Core.DTOs.Records;

public class FactionRelationDTO
{
    public required ModKey ModKey { get; set; }
    public required string FormID { get; set; }
    public int ItemIndex { get; set; }
    public required string TargetFormKey { get; set; }
    public string? Reaction { get; set; }
    public required string ImportedAtUtc { get; set; }
}
