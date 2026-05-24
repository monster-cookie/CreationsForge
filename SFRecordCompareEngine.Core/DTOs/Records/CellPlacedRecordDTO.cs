namespace SFRecordCompareEngine.Core.DTOs.Records;

public class CellPlacedRecordDTO
{
    public required string ModKey { get; set; }
    public required string CellFormID { get; set; }
    public required string PlacementGroup { get; set; }
    public int ItemIndex { get; set; }
    public string? PlacedFormKey { get; set; }
    public string? BaseFormKey { get; set; }
    public string? EditorID { get; set; }
    public string? Position { get; set; }
    public string? Rotation { get; set; }
    public int? IsDeleted { get; set; }
    public required string ImportedAtUtc { get; set; }
}
