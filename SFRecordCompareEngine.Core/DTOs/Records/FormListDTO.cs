using Mutagen.Bethesda.Plugins;

namespace SFRecordCompareEngine.Core.DTOs.Records;

public class FormListDTO
{
    public required ModKey ModKey { get; set; }
    public required string FormID { get; set; }
    public string? AddToListFormKey { get; set; }
    public required string ImportedAtUtc { get; set; }
}
