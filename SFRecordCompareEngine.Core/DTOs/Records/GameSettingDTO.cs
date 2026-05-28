using Mutagen.Bethesda.Plugins;

namespace SFRecordCompareEngine.Core.DTOs.Records;

public class GameSettingDTO
{
    public required ModKey ModKey { get; set; }
    public required string FormID { get; set; }
    public string? SettingType { get; set; }
    public string? TitleString { get; set; }
    public string? Data { get; set; }
    public double? RawData { get; set; }
    public int? XALG { get; set; }
    public int? IsCompressed { get; set; }
    public int? IsDeleted { get; set; }
    public required string ImportedAtUTC { get; set; }
}
