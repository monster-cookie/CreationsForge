namespace SFRecordCompareEngine.ViewModels;

public class RecordComparisonValueViewModel
{
    public RecordComparisonValueViewModel(string value, RecordComparisonValueState state)
    {
        Value = value;
        State = state;
    }

    public string Value { get; }
    public RecordComparisonValueState State { get; }
}