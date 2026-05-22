namespace SFRecordCompareEngine.Core.DTOs.Records;

public class FormListItemDTO
{
    public required string ModKey { get; set; }
    public required string FormID { get; set; }
    public int ItemIndex { get; set; }
    public required string ItemFormKey { get; set; }
    public required string ImportedAtUtc { get; set; }
}
