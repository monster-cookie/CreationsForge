namespace SFRecordCompareEngine.ViewModels;

public class RecordComparisonGroupViewModel : ViewModelBase
{
    public RecordComparisonGroupViewModel(
        string headerText,
        IReadOnlyList<RecordComparisonColumnViewModel> pluginColumns,
        IReadOnlyList<RecordComparisonGroupRowViewModel> rows)
    {
        HeaderText = headerText;
        PluginColumns = pluginColumns;
        Rows = rows;
        IsExpanded = HasChanges;
    }

    public string HeaderText { get; }
    public IReadOnlyList<RecordComparisonColumnViewModel> PluginColumns { get; }
    public IReadOnlyList<RecordComparisonGroupRowViewModel> Rows { get; }
    public bool HasChanges => Rows.Any(row => row.IsChanged);
    public string RowCountText => $"{Rows.Count} rows";

    public string ChangeStatusText => State == RecordComparisonValueState.Conflict
        ? "Changed"
        : State == RecordComparisonValueState.Identical
            ? "Identical"
            : "Single value";

    public RecordComparisonValueState State => Rows.Any(row => row.State == RecordComparisonValueState.Conflict)
        ? RecordComparisonValueState.Conflict
        : Rows.Any(row => row.State == RecordComparisonValueState.Identical)
            ? RecordComparisonValueState.Identical
            : RecordComparisonValueState.Neutral;

    public bool IsExpanded
    {
        get;
        set => SetProperty(ref field, value);
    }
}