namespace SFRecordCompareEngine.ViewModels;

public class RecordComparisonRowViewModel
{
    public required string FieldName { get; set; }

    public IDictionary<string, RecordComparisonCellViewModel> Cells { get; set; } =
        new Dictionary<string, RecordComparisonCellViewModel>(StringComparer.OrdinalIgnoreCase);
}
