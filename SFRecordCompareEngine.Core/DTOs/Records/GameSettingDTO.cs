namespace SFRecordCompareEngine.Core.DTOs.Records;

public class GameSettingDTO
{
    public required string ModKey { get; set; }
    public required string FormID { get; set; }
    public string? SettingType { get; set; }
    public string? TitleString { get; set; }
    public string? Data { get; set; }
    public double? RawData { get; set; }
    public int? XALG { get; set; }
    public int? IsCompressed { get; set; }
    public int? IsDeleted { get; set; }
    public required string ImportedAtUtc { get; set; }
}
