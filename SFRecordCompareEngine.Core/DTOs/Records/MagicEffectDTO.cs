namespace SFRecordCompareEngine.Core.DTOs.Records;

public class MagicEffectDTO
{
    public required string ModKey { get; set; }
    public required string FormID { get; set; }
    public string? Name { get; set; }
    public required string ImportedAtUtc { get; set; }
}
