using Mutagen.Bethesda.Plugins;

namespace CreationsForge.ViewModels;

public sealed partial class FormListBrowserViewModel
{
    /// <summary>The complete unfiltered winning-root tree from the current plugin snapshot.</summary>
    private IReadOnlyList<FormListRecordViewModel> AllRecordsValue = Array.Empty<FormListRecordViewModel>();

    /// <summary>The case-insensitive hexadecimal FormID filter.</summary>
    private string FormIdFilterValue = string.Empty;

    /// <summary>The case-insensitive EditorID filter.</summary>
    private string EditorIdFilterValue = string.Empty;

    /// <summary>The field used to order records within each plugin and major-record group.</summary>
    private FormListRecordSortMode RecordSortModeValue = FormListRecordSortMode.FormId;

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

    /// <summary>Gets or sets the field used to order records within each plugin and major-record group.</summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the supplied mode is undefined.</exception>
    public FormListRecordSortMode RecordSortMode
    {
        get => RecordSortModeValue;
        set
        {
            if (!Enum.IsDefined(value))
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            if (SetProperty(ref RecordSortModeValue, value))
            {
                ApplyRecordFilters();
            }
        }
    }

    /// <summary>Replaces the complete record snapshot and reapplies the current presentation filters.</summary>
    /// <param name="records">The complete winning-root tree in engine order.</param>
    private void SetAllRecords(IReadOnlyList<FormListRecordViewModel> records)
    {
        ArgumentNullException.ThrowIfNull(records);
        AllRecordsValue = records;
        ApplyRecordFilters();
    }

    /// <summary>Filters winning records and their retained contexts, applies the selected record order, and preserves complete comparison options.</summary>
    private void ApplyRecordFilters()
    {
        ClearEditorSelection();
        var formIdFilter = FormIdFilterValue.Trim();
        var editorIdFilter = EditorIdFilterValue.Trim();
        IReadOnlyList<FormListRecordViewModel> filteredRecords;
        if (formIdFilter.Length == 0 && editorIdFilter.Length == 0)
        {
            filteredRecords = AllRecordsValue;
        }
        else
        {
            var matches = new List<FormListRecordViewModel>();
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
        SetRecords(SortRecords(filteredRecords));
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

    /// <summary>Orders filtered records by the selected field with deterministic identity tie-breaking.</summary>
    /// <param name="records">The filtered winning-root records.</param>
    /// <returns>The records in the selected display order.</returns>
    private IReadOnlyList<FormListRecordViewModel> SortRecords(
        IReadOnlyList<FormListRecordViewModel> records)
    {
        IEnumerable<FormListRecordViewModel> sorted = RecordSortModeValue switch
        {
            FormListRecordSortMode.FormId => records
                .OrderBy(record => record.FormKey.ID)
                .ThenBy(record => record.FormKey.ModKey.FileName.String, StringComparer.OrdinalIgnoreCase),
            FormListRecordSortMode.EditorId => records
                .OrderBy(record => string.IsNullOrWhiteSpace(record.EditorId) ? 1 : 0)
                .ThenBy(record => record.EditorId, StringComparer.OrdinalIgnoreCase)
                .ThenBy(record => record.FormKey.ID)
                .ThenBy(record => record.FormKey.ModKey.FileName.String, StringComparer.OrdinalIgnoreCase),
            _ => throw new InvalidOperationException($"Unsupported FormList record sort mode '{RecordSortModeValue}'.")
        };
        return Array.AsReadOnly(sorted.ToArray());
    }

    /// <summary>Retains one winning root when its identity or any exact context matches the current filters.</summary>
    /// <param name="root">The complete winning root.</param>
    /// <param name="formIdFilter">The trimmed hexadecimal FormID filter.</param>
    /// <param name="editorIdFilter">The trimmed EditorID filter.</param>
    /// <returns>The complete matching root, or <see langword="null"/> when it does not match.</returns>
    private static FormListRecordViewModel? FilterRoot(
        FormListRecordViewModel root,
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

        if (!MatchesEditorId(root, editorIdFilter) &&
            !root.Contexts.Any(context => MatchesEditorId(context, editorIdFilter)))
        {
            return null;
        }

        return root;
    }

    /// <summary>Checks one root or context EditorID using the established case-insensitive substring behavior.</summary>
    /// <param name="record">The record node to inspect.</param>
    /// <param name="filter">The trimmed non-empty EditorID filter.</param>
    /// <returns><see langword="true"/> when the observed EditorID contains the filter.</returns>
    private static bool MatchesEditorId(FormListRecordViewModel record, string filter)
    {
        return record.EditorId?.Contains(filter, StringComparison.OrdinalIgnoreCase) == true;
    }

    /// <summary>Cancels comparison work and clears exact selection state hidden by the current filters.</summary>
    private void ClearHiddenSelection()
    {
        ComparisonGeneration++;
        CancelAndDispose(ref ComparisonCancellation);
        SelectedRecordValue = null;
        ContextOptionsValue = Array.Empty<FormListContextOption>();
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
