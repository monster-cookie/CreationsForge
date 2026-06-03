namespace SFRecordCompareEngine.ViewModels;

public class RecordComparisonScriptPropertyViewModel
{
    public RecordComparisonScriptPropertyViewModel(
        string label,
        string type,
        string listCount,
        IReadOnlyList<RecordComparisonValueViewModel> values,
        RecordComparisonValueState state)
    {
        Label = label;
        Type = type;
        ListCount = listCount;
        Values = values;
        State = state;
    }

    public string Label { get; }
    public string Type { get; }
    public string ListCount { get; }
    public IReadOnlyList<RecordComparisonValueViewModel> Values { get; }
    public RecordComparisonValueState State { get; }
    public bool IsChanged => State == RecordComparisonValueState.Conflict;
}
