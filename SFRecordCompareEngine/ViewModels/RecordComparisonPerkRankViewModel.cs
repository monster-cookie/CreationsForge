using System.Collections.ObjectModel;

namespace SFRecordCompareEngine.ViewModels;

public class RecordComparisonPerkRankViewModel : ViewModelBase
{
    private readonly IReadOnlyList<RecordComparisonPerkRankEffectViewModel> AllRows;

    public RecordComparisonPerkRankViewModel(
        int rankIndex,
        IReadOnlyList<RecordComparisonColumnViewModel> pluginColumns,
        IReadOnlyList<RecordComparisonPerkRankEffectViewModel> rows,
        RecordComparisonValueState state)
    {
        RankIndex = rankIndex;
        PluginColumns = pluginColumns;
        AllRows = rows;
        State = state;
        IsExpanded = state == RecordComparisonValueState.Conflict || rows.Any(row => row.IsChanged);
        ApplyRowFilter();
    }

    public int RankIndex { get; }
    public IReadOnlyList<RecordComparisonColumnViewModel> PluginColumns { get; }
    public ObservableCollection<RecordComparisonPerkRankEffectViewModel> Rows { get; } = new();
    public RecordComparisonValueState State { get; }
    public bool HasChanges => State == RecordComparisonValueState.Conflict || AllRows.Any(row => row.IsChanged);
    public string HeaderText => $"Rank {RankIndex}";
    public string RowCountText => $"{AllRows.Count} rows";
    public string ChangeStatusText => HasChanges ? "Changed" : "Identical";

    public bool IsExpanded
    {
        get;
        set => SetProperty(ref field, value);
    }

    public bool ShowChangedOnly
    {
        get;
        private set
        {
            if (!SetProperty(ref field, value))
            {
                return;
            }

            ApplyRowFilter();
        }
    }

    public void SetShowChangedOnly(bool showChangedOnly)
    {
        ShowChangedOnly = showChangedOnly;
    }

    private void ApplyRowFilter()
    {
        Rows.Clear();
        foreach (var row in AllRows.Where(row => !ShowChangedOnly || row.IsChanged))
        {
            Rows.Add(row);
        }
    }
}

