using CreationsForge.Core.Engine.Contracts;

namespace CreationsForge.ViewModels;

/// <summary>Filters and sorts the already-loaded major-record summaries without blocking the UI thread.</summary>
public sealed partial class MajorRecordBrowserViewModel
{
    /// <summary>Retains the initial complete family hierarchy for immediate restoration when filters clear.</summary>
    private IReadOnlyList<RecordTypeGroupViewModel> UnfilteredRecordGroupsValue = Array.Empty<RecordTypeGroupViewModel>();

    /// <summary>The case-insensitive hexadecimal FormID substring.</summary>
    private string FormIdFilterValue = string.Empty;

    /// <summary>The case-insensitive EditorID substring.</summary>
    private string EditorIdFilterValue = string.Empty;

    /// <summary>Whether to show only winning records contained by the selected plugin.</summary>
    private bool SelectedPluginOnlyValue;

    /// <summary>The field used to order rows within each record family.</summary>
    private MajorRecordSortMode RecordSortModeValue = MajorRecordSortMode.FormId;

    /// <summary>Cancels an obsolete local metadata filter or sort.</summary>
    private CancellationTokenSource? FilterCancellation;

    /// <summary>Rejects filtered projections produced for an older query or workspace.</summary>
    private long FilterGeneration;

    /// <summary>The number of rows visible after the latest completed filter.</summary>
    private int VisibleRecordCountValue;

    /// <summary>Whether a local summary projection is in progress.</summary>
    private bool IsFilteringValue;

    /// <summary>The latest projection task, retained for orderly lifecycle and deterministic tests.</summary>
    internal Task CurrentFilterTask { get; private set; } = Task.CompletedTask;

    /// <summary>Gets or sets the hexadecimal FormID substring applied to every loaded winning row.</summary>
    public string FormIdFilter
    {
        get => FormIdFilterValue;
        set
        {
            if (SetProperty(ref FormIdFilterValue, value ?? string.Empty))
            {
                CurrentFilterTask = BeginFilterGenerationAsync();
            }
        }
    }

    /// <summary>Gets or sets the EditorID substring applied to every loaded winning row.</summary>
    public string EditorIdFilter
    {
        get => EditorIdFilterValue;
        set
        {
            if (SetProperty(ref EditorIdFilterValue, value ?? string.Empty))
            {
                CurrentFilterTask = BeginFilterGenerationAsync();
            }
        }
    }

    /// <summary>Gets or sets whether the tree shows only records contained by the selected plugin.</summary>
    public bool SelectedPluginOnly
    {
        get => SelectedPluginOnlyValue;
        set
        {
            if (SetProperty(ref SelectedPluginOnlyValue, value))
            {
                CurrentFilterTask = BeginFilterGenerationAsync();
            }
        }
    }

    /// <summary>Gets the selected-plugin filter label for editing or read-only workspaces.</summary>
    public string SelectedPluginFilterLabel => WorkspaceCoordinator.CurrentWorkspace?.Output is null
        ? "Selected plugin only"
        : "Edited plugin only";

    /// <summary>Gets or sets the ordering applied inside each major-record family.</summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the requested mode is undefined.</exception>
    public MajorRecordSortMode RecordSortMode
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
                CurrentFilterTask = BeginFilterGenerationAsync();
            }
        }
    }

    /// <summary>Gets whether a FormID or EditorID filter is active.</summary>
    public bool HasActiveRecordFilter => SelectedPluginOnlyValue || !string.IsNullOrWhiteSpace(FormIdFilterValue) || !string.IsNullOrWhiteSpace(EditorIdFilterValue);

    /// <summary>Gets whether the already-loaded summaries are being filtered or reordered.</summary>
    public bool IsFiltering => IsFilteringValue;

    /// <summary>Starts a new local projection and cancels any obsolete projection.</summary>
    /// <returns>The latest projection task, or a completed task before records are available.</returns>
    private Task BeginFilterGenerationAsync()
    {
        FilterGeneration++;
        CancelAndDispose(ref FilterCancellation);
        OnPropertyChanged(nameof(HasActiveRecordFilter));
        OnPropertyChanged(nameof(LoadedRecordCountText));
        if (IsDisposed || !RevisionValue.HasValue || WorkspaceCoordinator.CurrentWorkspace is not { } workspace)
        {
            SetFiltering(false);
            return Task.CompletedTask;
        }

        FilterCancellation = CancellationTokenSource.CreateLinkedTokenSource(WorkspaceCancellation?.Token ?? CancellationToken.None);
        SetFiltering(true);
        return FilterRecordsAsync(
            workspace.WorkspaceId,
            WorkspaceGeneration,
            FilterGeneration,
            RevisionValue.Value,
            FormIdFilterValue.Trim(),
            EditorIdFilterValue.Trim(),
            SelectedPluginOnlyValue ? workspace.Output?.PluginPath ?? workspace.SourcePluginPath : null,
            RecordSortModeValue,
            FilterCancellation.Token);
    }

    /// <summary>Scans identity metadata off-thread and publishes only the latest matching hierarchy.</summary>
    /// <param name="workspaceId">The workspace that owns the complete summary snapshot.</param>
    /// <param name="workspaceGeneration">The generation of that snapshot.</param>
    /// <param name="filterGeneration">The exact requested filter generation.</param>
    /// <param name="revision">The accepted record revision.</param>
    /// <param name="formIdFilter">The trimmed hexadecimal identity fragment.</param>
    /// <param name="editorIdFilter">The trimmed EditorID fragment.</param>
    /// <param name="selectedPluginPath">The exact selected plugin path when its filter is active.</param>
    /// <param name="sortMode">The ordering inside each record family.</param>
    /// <param name="cancellationToken">Cancels the obsolete scan.</param>
    /// <returns>A task that completes after current results publish or the request becomes stale.</returns>
    private async Task FilterRecordsAsync(
        Guid workspaceId,
        long workspaceGeneration,
        long filterGeneration,
        WorkspaceRevision revision,
        string formIdFilter,
        string editorIdFilter,
        string? selectedPluginPath,
        MajorRecordSortMode sortMode,
        CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(150, cancellationToken).ConfigureAwait(false);
            var records = RecordsValue;
            var unfilteredGroups = UnfilteredRecordGroupsValue;
            var (groups, visibleCount) = await Task.Run(() =>
            {
                if (formIdFilter.Length == 0 && editorIdFilter.Length == 0 && selectedPluginPath is null && sortMode == MajorRecordSortMode.FormId)
                {
                    return (unfilteredGroups, records.Count);
                }

                var matches = new List<MajorRecordViewModel>();
                foreach (var record in records)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (MatchesRecordFilters(record, formIdFilter, editorIdFilter, selectedPluginPath))
                    {
                        matches.Add(record);
                    }
                }

                cancellationToken.ThrowIfCancellationRequested();
                return (CreateRecordGroups(matches, sortMode), matches.Count);
            }, cancellationToken).ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();
            await UiDispatcher.InvokeAsync(() =>
            {
                if (!IsCurrentFilterGeneration(workspaceId, workspaceGeneration, filterGeneration, revision))
                {
                    return;
                }

                RecordGroupsValue = groups;
                RecordTreeSourceValue = CreateRecordTreeSource(groups);
                VisibleRecordCountValue = visibleCount;
                OnPropertyChanged(nameof(RecordGroups));
                OnPropertyChanged(nameof(RecordTreeSource));
                OnPropertyChanged(nameof(LoadedRecordCountText));
                if (SelectedRecordValue is { } selected && !MatchesRecordFilters(selected, formIdFilter, editorIdFilter, selectedPluginPath))
                {
                    SelectionGeneration++;
                    CancelAndDispose(ref SelectionCancellation);
                    SelectedRecordValue = null;
                    ClearContextPresentation();
                    SetComparisonBusy(false);
                    OnPropertyChanged(nameof(SelectedRecord));
                    SetStatus("The selected record is hidden by the current filters.");
                }
            }).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        finally
        {
            UiDispatcher.Post(() =>
            {
                if (IsCurrentFilterGeneration(workspaceId, workspaceGeneration, filterGeneration, revision))
                {
                    SetFiltering(false);
                }
            });
        }
    }

    /// <summary>Checks both optional case-insensitive substrings against one winning record row.</summary>
    /// <param name="record">The loaded summary row.</param>
    /// <param name="formIdFilter">The trimmed hexadecimal FormID fragment.</param>
    /// <param name="editorIdFilter">The trimmed EditorID fragment.</param>
    /// <param name="selectedPluginPath">The exact selected plugin path when filtering by its contained records.</param>
    /// <returns>Whether the row matches both supplied filters.</returns>
    private static bool MatchesRecordFilters(
        MajorRecordViewModel record,
        string formIdFilter,
        string editorIdFilter,
        string? selectedPluginPath)
    {
        return record.PrimaryText.Contains(formIdFilter, StringComparison.OrdinalIgnoreCase)
            && (editorIdFilter.Length == 0
                || record.EditorId?.Contains(editorIdFilter, StringComparison.OrdinalIgnoreCase) == true)
            && (selectedPluginPath is null || string.Equals(
                record.SourcePath,
                selectedPluginPath,
                OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal));
    }

    /// <summary>Checks the workspace and filter generations before publishing a local projection.</summary>
    /// <param name="workspaceId">The source workspace identity.</param>
    /// <param name="workspaceGeneration">The source workspace generation.</param>
    /// <param name="filterGeneration">The requested filter generation.</param>
    /// <param name="revision">The source record revision.</param>
    /// <returns>Whether the result still belongs to the displayed workspace and current filter.</returns>
    private bool IsCurrentFilterGeneration(
        Guid workspaceId,
        long workspaceGeneration,
        long filterGeneration,
        WorkspaceRevision revision)
    {
        return FilterGeneration == filterGeneration
            && WorkspaceGeneration == workspaceGeneration
            && IsCurrentWorkspaceGeneration(workspaceId, workspaceGeneration, revision);
    }

    /// <summary>Updates the visible local filtering state and count binding.</summary>
    /// <param name="isFiltering">Whether a current metadata projection is still running.</param>
    private void SetFiltering(bool isFiltering)
    {
        if (SetProperty(ref IsFilteringValue, isFiltering, nameof(IsFiltering)))
        {
            OnPropertyChanged(nameof(LoadedRecordCountText));
        }
    }
}
