namespace SFRecordCompareEngine.ViewModels;

public class RecordComparisonFieldViewModel
{
    public RecordComparisonFieldViewModel(string label, bool isComparable = true)
    {
        Label = label;
        IsComparable = isComparable;
    }

    public string Label { get; }
    public bool IsComparable { get; }
    public RecordComparisonValueState State { get; set; }
}