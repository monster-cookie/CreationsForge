using Mutagen.Bethesda.Plugins;

namespace SFRecordCompareEngine.Core.DTOs.Records;

public class WorldspaceDTO
{
    public required ModKey ModKey { get; set; }
    public required string FormID { get; set; }
    public string? Name { get; set; }
    public string? ParentWorldspaceFormKey { get; set; }
    public string? ClimateFormKey { get; set; }
    public string? WaterFormKey { get; set; }
    public string? TopCellFormKey { get; set; }
    public string? WorldMapCellOffset { get; set; }
    public string? WorldMapOffsetScale { get; set; }
    public required string ImportedAtUtc { get; set; }
}
