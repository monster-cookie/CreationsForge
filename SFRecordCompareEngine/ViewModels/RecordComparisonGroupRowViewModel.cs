namespace SFRecordCompareEngine.ViewModels;

public class RecordComparisonGroupRowViewModel
{
    public RecordComparisonGroupRowViewModel(
        string label,
        IReadOnlyList<RecordComparisonValueViewModel> values,
        RecordComparisonValueState state)
    {
        Label = label;
        Values = values;
        State = state;
    }

    public string Label { get; }
    public IReadOnlyList<RecordComparisonValueViewModel> Values { get; }
    public RecordComparisonValueState State { get; }
    public bool IsChanged => State == RecordComparisonValueState.Conflict;
}