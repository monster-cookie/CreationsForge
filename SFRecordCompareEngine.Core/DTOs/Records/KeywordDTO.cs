namespace SFRecordCompareEngine.Core.DTOs.Records;

public class KeywordDTO
{
    public required string ModKey { get; set; }
    public required string FormID { get; set; }
    public string? Name { get; set; }
    public string? Color { get; set; }
    public string? KeywordType { get; set; }
    public string? FNAM { get; set; }
    public required string ImportedAtUtc { get; set; }
}
