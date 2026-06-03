using System.Collections.ObjectModel;

namespace SFRecordCompareEngine.ViewModels;

public class RecordComparisonScriptViewModel : ViewModelBase
{
    private readonly IReadOnlyList<RecordComparisonScriptPropertyViewModel> AllProperties;

    public RecordComparisonScriptViewModel(
        int scriptIndex,
        string scriptName,
        IReadOnlyList<RecordComparisonColumnViewModel> pluginColumns,
        IReadOnlyList<RecordComparisonValueViewModel> scriptNameValues,
        IReadOnlyList<RecordComparisonScriptPropertyViewModel> properties,
        RecordComparisonValueState state)
    {
        ScriptIndex = scriptIndex;
        ScriptName = scriptName;
        PluginColumns = pluginColumns;
        ScriptNameValues = scriptNameValues;
        AllProperties = properties;
        State = state;
        IsExpanded = state == RecordComparisonValueState.Conflict;
        ApplyPropertyFilter();
    }

    public int ScriptIndex { get; }
    public string ScriptName { get; }
    public IReadOnlyList<RecordComparisonColumnViewModel> PluginColumns { get; }
    public IReadOnlyList<RecordComparisonValueViewModel> ScriptNameValues { get; }
    public ObservableCollection<RecordComparisonScriptPropertyViewModel> Properties { get; } = new();
    public RecordComparisonValueState State { get; }
    public bool HasChanges => State == RecordComparisonValueState.Conflict || AllProperties.Any(property => property.IsChanged);
    public string HeaderText => $"Script {ScriptIndex} - {ScriptName}";
    public string PropertyCountText => $"{AllProperties.Count} properties";
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

            ApplyPropertyFilter();
        }
    }

    public void SetShowChangedOnly(bool showChangedOnly)
    {
        ShowChangedOnly = showChangedOnly;
    }

    private void ApplyPropertyFilter()
    {
        Properties.Clear();
        foreach (var property in AllProperties.Where(property => !ShowChangedOnly || property.IsChanged))
        {
            Properties.Add(property);
        }
    }
}
