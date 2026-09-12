using Mutagen.Bethesda.Plugins;

namespace CreationsForge.ViewModels;

public sealed partial class NativeFormListBrowserViewModel
{
    /// <summary>The complete unfiltered winning-root tree from the current native snapshot.</summary>
    private IReadOnlyList<NativeFormListRecordViewModel> AllRecordsValue = Array.Empty<NativeFormListRecordViewModel>();

    /// <summary>The case-insensitive hexadecimal FormID filter.</summary>
    private string FormIdFilterValue = string.Empty;

    /// <summary>The case-insensitive EditorID filter.</summary>
    private string EditorIdFilterValue = string.Empty;

    /// <summary>Gets or sets the hexadecimal FormID substring used to filter winning roots.</summary>
    public string FormIdFilter
    {
        get => FormIdFilterValue;
        set
        {
            if (SetProperty(ref FormIdFilterValue, value ?? string.Empty))
            {
                ApplyRecordFilters();
            }
        }
    }

    /// <summary>Gets or sets the EditorID substring used to filter winning roots and their exact contexts.</summary>
    public string EditorIdFilter
    {
        get => EditorIdFilterValue;
        set
        {
            if (SetProperty(ref EditorIdFilterValue, value ?? string.Empty))
            {
                ApplyRecordFilters();
            }
        }
    }

    /// <summary>Replaces the complete native record snapshot and reapplies the current presentation filters.</summary>
    /// <param name="records">The complete winning-root tree in engine order.</param>
    private void SetAllRecords(IReadOnlyList<NativeFormListRecordViewModel> records)
    {
        ArgumentNullException.ThrowIfNull(records);
        AllRecordsValue = records;
        ApplyRecordFilters();
    }

    /// <summary>Filters roots and exact context children while preserving engine order and complete comparison options.</summary>
    private void ApplyRecordFilters()
    {
        ClearEditorSelection();
        var formIdFilter = FormIdFilterValue.Trim();
        var editorIdFilter = EditorIdFilterValue.Trim();
        IReadOnlyList<NativeFormListRecordViewModel> filteredRecords;
        if (formIdFilter.Length == 0 && editorIdFilter.Length == 0)
        {
            filteredRecords = AllRecordsValue;
        }
        else
        {
            var matches = new List<NativeFormListRecordViewModel>();
            foreach (var root in AllRecordsValue)
            {
                var filteredRoot = FilterRoot(root, formIdFilter, editorIdFilter);
                if (filteredRoot is not null)
                {
                    matches.Add(filteredRoot);
                }
            }

            filteredRecords = Array.AsReadOnly(matches.ToArray());
        }

        var selectedFormKey = SelectedRecordValue?.FormKey;
        SetRecords(filteredRecords);
        if (!selectedFormKey.HasValue)
        {
            return;
        }

        var visibleSelection = RecordsValue.FirstOrDefault(record => record.FormKey == selectedFormKey.Value);
        if (visibleSelection is null)
        {
            ClearHiddenSelection();
            return;
        }

        SelectedRecordValue = visibleSelection;
        ContextOptionsValue = visibleSelection.ContextOptions;
        OnPropertyChanged(nameof(SelectedRecord));
        OnPropertyChanged(nameof(ContextOptions));
    }

    /// <summary>Creates one filtered winning root with only exact contexts matching the current EditorID filter.</summary>
    /// <param name="root">The complete winning root.</param>
    /// <param name="formIdFilter">The trimmed hexadecimal FormID filter.</param>
    /// <param name="editorIdFilter">The trimmed EditorID filter.</param>
    /// <returns>The matching root with filtered children, or <see langword="null"/> when it does not match.</returns>
    private static NativeFormListRecordViewModel? FilterRoot(
        NativeFormListRecordViewModel root,
        string formIdFilter,
        string editorIdFilter)
    {
        if (!root.FormIdText.Contains(formIdFilter, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        if (editorIdFilter.Length == 0)
        {
            return root;
        }

        var matchingChildren = root.Children
            .Where(child => MatchesEditorId(child, editorIdFilter))
            .ToArray();
        if (!MatchesEditorId(root, editorIdFilter) && matchingChildren.Length == 0)
        {
            return null;
        }

        return new NativeFormListRecordViewModel(
            root.FormKey,
            root.EditorId,
            root.OverrideCount,
            root.Context,
            matchingChildren,
            root.ContextOptions)
        {
            IsExpanded = root.IsExpanded,
        };
    }

    /// <summary>Checks one root or context EditorID using the established case-insensitive substring behavior.</summary>
    /// <param name="record">The record node to inspect.</param>
    /// <param name="filter">The trimmed non-empty EditorID filter.</param>
    /// <returns><see langword="true"/> when the observed EditorID contains the filter.</returns>
    private static bool MatchesEditorId(NativeFormListRecordViewModel record, string filter)
    {
        return record.EditorId?.Contains(filter, StringComparison.OrdinalIgnoreCase) == true;
    }

    /// <summary>Cancels comparison work and clears exact selection state hidden by the current filters.</summary>
    private void ClearHiddenSelection()
    {
        ComparisonGeneration++;
        CancelAndDispose(ref ComparisonCancellation);
        SelectedRecordValue = null;
        ContextOptionsValue = Array.Empty<NativeFormListContextOption>();
        SelectedBeforeContextValue = null;
        SelectedAfterContextValue = null;
        ClearEditorSelection();
        OnPropertyChanged(nameof(SelectedRecord));
        OnPropertyChanged(nameof(ContextOptions));
        OnPropertyChanged(nameof(SelectedBeforeContext));
        OnPropertyChanged(nameof(SelectedAfterContext));
        ClearComparisonPresentation();
        SetStatus("The selected FormList is hidden by the current filters.");
    }
}
