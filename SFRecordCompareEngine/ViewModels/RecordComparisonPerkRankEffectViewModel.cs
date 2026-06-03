namespace SFRecordCompareEngine.ViewModels;

public class RecordComparisonPerkRankEffectViewModel
{
    public RecordComparisonPerkRankEffectViewModel(
        string label,
        string type,
        IReadOnlyList<RecordComparisonValueViewModel> values,
        RecordComparisonValueState state)
    {
        Label = label;
        Type = type;
        Values = values;
        State = state;
    }

    public string Label { get; }
    public string Type { get; }
    public IReadOnlyList<RecordComparisonValueViewModel> Values { get; }
    public RecordComparisonValueState State { get; }
    public bool IsChanged => State == RecordComparisonValueState.Conflict;
}

