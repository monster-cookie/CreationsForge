using Mutagen.Bethesda.Plugins;

namespace SFRecordCompareEngine.Core.DTOs.Records;

public class CellDTO
{
    public required ModKey ModKey { get; set; }
    public required string FormID { get; set; }
    public string? Name { get; set; }
    public string? Flags { get; set; }
    public string? MajorFlags { get; set; }
    public string? LightingTemplateFormKey { get; set; }
    public string? ImageSpaceFormKey { get; set; }
    public string? LocationFormKey { get; set; }
    public string? WaterFormKey { get; set; }
    public string? WaterHeight { get; set; }
    public int? IsLinkedRefTransient { get; set; }
    public required string ImportedAtUtc { get; set; }
}
