using Avalonia.Controls;
using CreationsForge.Core.Engine.Contracts;
using Mutagen.Bethesda.Plugins;

namespace CreationsForge.ViewModels;

/// <summary>Supplies browser publication, hierarchy construction, retry, and stale-result checks.</summary>
public sealed partial class MajorRecordBrowserViewModel
{
    /// <summary>Publishes the complete winning-record collection after one successful engine visit.</summary>
    /// <param name="records">The projected records from every admitted source.</param>
    /// <param name="groups">The prebuilt family hierarchy.</param>
    /// <param name="revision">The revision of the completed visit.</param>
    /// <param name="warnings">The visit warnings.</param>
    private void PublishRecords(
        IReadOnlyList<MajorRecordViewModel> records,
        IReadOnlyList<RecordTypeGroupViewModel> groups,
        WorkspaceRevision revision,
        IReadOnlyList<EngineWarning> warnings)
    {
        RevisionValue = revision;
        RecordsValue = records;
        RecordGroupsValue = groups;
        RecordTreeSourceValue = CreateRecordTreeSource(groups);
        OnPropertyChanged(nameof(Records));
        OnPropertyChanged(nameof(RecordGroups));
        OnPropertyChanged(nameof(RecordTreeSource));
        PageWarnings = Array.AsReadOnly(warnings.ToArray());
        SetWarnings(PageWarnings);
        ClearError();
        RetryKindValue = RetryKind.None;
        SetStatus($"Loaded all {RecordsValue.Count:N0} major record(s).");
        OnPropertyChanged(nameof(LoadedRecordCountText));
        RefreshRelayCommand.RaiseCanExecuteChanged();
    }

    /// <summary>Groups winning records off the UI thread, collapsing families for very large workspaces.</summary>
    /// <param name="records">The complete winning record summaries.</param>
    /// <returns>The sorted family hierarchy with every record retained.</returns>
    private static IReadOnlyList<RecordTypeGroupViewModel> CreateRecordGroups(
        IReadOnlyList<MajorRecordViewModel> records)
    {
        var expandGroups = records.Count <= 10_000;
        return Array.AsReadOnly(
            records
                .GroupBy(record => record.RecordType, StringComparer.Ordinal)
                .OrderBy(group => group.Key, StringComparer.Ordinal)
                .Select(group => new RecordTypeGroupViewModel(
                    $"{group.Key} ({group.Count():N0})",
                    Array.AsReadOnly<IRecordTreeNodeViewModel>(
                        group.OrderBy(record => record.FormKey.ModKey.FileName.String, StringComparer.OrdinalIgnoreCase)
                            .ThenBy(record => record.FormKey.ID)
                            .Cast<IRecordTreeNodeViewModel>()
                            .ToArray()),
                    expandGroups))
                .ToArray());
    }

    /// <summary>Publishes exact context options and selects origin-to-winner defaults.</summary>
    /// <param name="options">The winning option followed by exact plugin contexts.</param>
    /// <param name="warnings">The context discovery warnings.</param>
    private void PublishContextOptions(
        IReadOnlyList<MajorRecordContextOption> options,
        IReadOnlyList<EngineWarning> warnings)
    {
        ContextOptionsValue = options;
        SelectedAfterContextValue = options.FirstOrDefault(option => option.IsWinningOverride);
        SelectedBeforeContextValue = options.FirstOrDefault(option => !option.IsWinningOverride)
            ?? SelectedAfterContextValue;
        OnPropertyChanged(nameof(ContextOptions));
        OnPropertyChanged(nameof(SelectedBeforeContext));
        OnPropertyChanged(nameof(SelectedAfterContext));
        OnPropertyChanged(nameof(BeforeProvenanceText));
        OnPropertyChanged(nameof(AfterProvenanceText));
        SetWarnings(CombineWarnings(PageWarnings, warnings));
        ClearError();
        RetryKindValue = RetryKind.None;
    }

    /// <summary>Clears all revision, page, selection, and comparison presentation.</summary>
    private void ClearWorkspacePresentation()
    {
        RevisionValue = null;
        LoadingRecordCountValue = 0;
        RecordsValue = Array.Empty<MajorRecordViewModel>();
        RecordGroupsValue = Array.Empty<RecordTypeGroupViewModel>();
        RecordTreeSourceValue = CreateRecordTreeSource(RecordGroupsValue);
        SelectedRecordValue = null;
        PageWarnings = Array.Empty<EngineWarning>();
        SetWarnings(PageWarnings);
        ClearContextPresentation();
        ClearError();
        RetryKindValue = RetryKind.None;
        OnPropertyChanged(nameof(Records));
        OnPropertyChanged(nameof(RecordGroups));
        OnPropertyChanged(nameof(RecordTreeSource));
        OnPropertyChanged(nameof(SelectedRecord));
        OnPropertyChanged(nameof(LoadedRecordCountText));
        RefreshRelayCommand.RaiseCanExecuteChanged();
    }

    /// <summary>Clears exact context options and their comparison.</summary>
    private void ClearContextPresentation()
    {
        ContextOptionsValue = Array.Empty<MajorRecordContextOption>();
        SelectedBeforeContextValue = null;
        SelectedAfterContextValue = null;
        OnPropertyChanged(nameof(ContextOptions));
        OnPropertyChanged(nameof(SelectedBeforeContext));
        OnPropertyChanged(nameof(SelectedAfterContext));
        ClearComparisonPresentation();
    }

    /// <summary>Clears reported contexts, field trees, and semantic changes.</summary>
    private void ClearComparisonPresentation()
    {
        BeforeContextValue = null;
        AfterContextValue = null;
        OnPropertyChanged(nameof(BeforeContext));
        OnPropertyChanged(nameof(AfterContext));
        OnPropertyChanged(nameof(BeforeProvenanceText));
        OnPropertyChanged(nameof(AfterProvenanceText));
        SetBeforeFields(Array.Empty<RecordJsonFieldNodeViewModel>());
        SetAfterFields(Array.Empty<RecordJsonFieldNodeViewModel>());
        SemanticChangesValue = Array.Empty<SemanticChangeDescriptor>();
        OnPropertyChanged(nameof(SemanticChanges));
    }

    /// <summary>Replaces the prior field hierarchy.</summary>
    /// <param name="fields">The complete prior field roots.</param>
    private void SetBeforeFields(IReadOnlyList<RecordJsonFieldNodeViewModel> fields)
    {
        BeforeFieldsValue = fields;
        BeforeFieldSourceValue = FormListBrowserViewModel.CreateFieldTreeSource(fields);
        OnPropertyChanged(nameof(BeforeFields));
        OnPropertyChanged(nameof(BeforeFieldSource));
    }

    /// <summary>Replaces the resulting field hierarchy.</summary>
    /// <param name="fields">The complete resulting field roots.</param>
    private void SetAfterFields(IReadOnlyList<RecordJsonFieldNodeViewModel> fields)
    {
        AfterFieldsValue = fields;
        AfterFieldSourceValue = FormListBrowserViewModel.CreateFieldTreeSource(fields);
        OnPropertyChanged(nameof(AfterFields));
        OnPropertyChanged(nameof(AfterFieldSource));
    }

    /// <summary>Replaces visible warnings while preserving supplied order.</summary>
    /// <param name="warnings">The new warning sequence.</param>
    private void SetWarnings(IReadOnlyList<EngineWarning> warnings)
    {
        WarningsValue = Array.AsReadOnly(warnings.ToArray());
        OnPropertyChanged(nameof(Warnings));
        OnPropertyChanged(nameof(HasWarnings));
    }

    /// <summary>Replaces the browser status text.</summary>
    /// <param name="status">The new text, or an empty string to hide status.</param>
    private void SetStatus(string status)
    {
        if (SetProperty(ref StatusTextValue, status, nameof(StatusText)))
        {
            OnPropertyChanged(nameof(HasStatusText));
        }
    }

    /// <summary>Changes record paging busy state and command availability.</summary>
    /// <param name="isBusy">Whether a page is active.</param>
    private void SetBusy(bool isBusy)
    {
        if (SetProperty(ref IsBusyValue, isBusy, nameof(IsBusy)))
        {
            RefreshRelayCommand.RaiseCanExecuteChanged();
            OnPropertyChanged(nameof(LoadedRecordCountText));
        }
    }

    /// <summary>Changes comparison busy state.</summary>
    /// <param name="isBusy">Whether context or comparison work is active.</param>
    private void SetComparisonBusy(bool isBusy)
    {
        SetProperty(ref IsComparisonBusyValue, isBusy, nameof(IsComparisonBusy));
    }

    /// <summary>Publishes one typed retryable failure.</summary>
    /// <param name="error">The engine error, or <see langword="null"/> for an invalid engine result.</param>
    /// <param name="retryKind">The operation to repeat.</param>
    private void PublishFailure(EngineError? error, RetryKind retryKind)
    {
        var resolved = error ?? new EngineError(
            EngineErrorCode.UnexpectedFailure,
            "The engine returned no value or failure reason.");
        ErrorCodeValue = resolved.Code;
        ErrorMessageValue = resolved.Message;
        RetryKindValue = retryKind;
        OnPropertyChanged(nameof(ErrorCode));
        OnPropertyChanged(nameof(ErrorMessage));
        OnPropertyChanged(nameof(HasError));
        RetryRelayCommand.RaiseCanExecuteChanged();
        SetStatus(string.Empty);
    }

    /// <summary>Clears the current typed failure.</summary>
    private void ClearError()
    {
        ErrorCodeValue = null;
        ErrorMessageValue = null;
        OnPropertyChanged(nameof(ErrorCode));
        OnPropertyChanged(nameof(ErrorMessage));
        OnPropertyChanged(nameof(HasError));
        RetryRelayCommand.RaiseCanExecuteChanged();
    }

    /// <summary>Repeats the most recent failed operation.</summary>
    /// <returns>A task that completes after the repeated operation.</returns>
    private Task RetryAsync()
    {
        return RetryKindValue switch
        {
            RetryKind.Page => RefreshAsync(),
            RetryKind.Contexts => BeginSelectionGenerationAsync(),
            RetryKind.Comparison => BeginComparisonGenerationAsync(),
            _ => Task.CompletedTask
        };
    }

    /// <summary>Formats exact resolution and containing-plugin provenance.</summary>
    /// <param name="context">The engine-reported context, or <see langword="null"/>.</param>
    /// <param name="selection">The pending selection, or <see langword="null"/>.</param>
    /// <returns>Explicit resolution and provenance text.</returns>
    private static string FormatContext(FormListContext? context, MajorRecordContextOption? selection)
    {
        if (context is null)
        {
            return selection?.ProvenanceText ?? "No context selected.";
        }

        return context.ContainingModKey.HasValue
            ? $"{context.Status} | {context.ContainingModKey.Value.FileName} | index {context.LoadOrderIndex} | {context.Role} | {context.Path}"
            : context.Status.ToString();
    }

    /// <summary>Creates the hierarchical record-family source.</summary>
    /// <param name="groups">The stable record-family groups.</param>
    /// <returns>The read-only hierarchy.</returns>
    private static HierarchicalTreeDataGridSource<IRecordTreeNodeViewModel> CreateRecordTreeSource(
        IReadOnlyList<RecordTypeGroupViewModel> groups)
    {
        return new HierarchicalTreeDataGridSource<IRecordTreeNodeViewModel>(groups)
            .WithHierarchicalExpanderTextColumn(
                "FormID / family",
                record => record.PrimaryText,
                record => record.TreeChildren,
                record => record.IsExpanded,
                record => record.HasChildren,
                options => options.BeginEditGestures = BeginEditGestures.None)
            .WithTextColumn("EditorID", record => record.EditorIdText, options => options.BeginEditGestures = BeginEditGestures.None)
            .WithTextColumn("Plugin context", record => record.ContextText, options => options.BeginEditGestures = BeginEditGestures.None);
    }

    /// <summary>Checks whether one page result still belongs to the current workspace generation.</summary>
    /// <param name="workspaceId">The captured workspace identity.</param>
    /// <param name="generation">The captured workspace generation.</param>
    /// <param name="expectedRevision">The captured revision for an appended page.</param>
    /// <returns><see langword="true"/> when publication remains current.</returns>
    private bool IsCurrentWorkspaceGeneration(Guid workspaceId, long generation, WorkspaceRevision? expectedRevision)
    {
        return !IsDisposed &&
            WorkspaceGeneration == generation &&
            WorkspaceCoordinator.CurrentWorkspace?.WorkspaceId == workspaceId &&
            (!expectedRevision.HasValue || RevisionValue == expectedRevision);
    }

    /// <summary>Checks whether context discovery still belongs to the current selection.</summary>
    /// <param name="workspaceId">The captured workspace identity.</param>
    /// <param name="workspaceGeneration">The captured workspace generation.</param>
    /// <param name="selectionGeneration">The captured selection generation.</param>
    /// <param name="revision">The captured revision.</param>
    /// <param name="formKey">The captured record identity.</param>
    /// <returns><see langword="true"/> when publication remains current.</returns>
    private bool IsCurrentSelectionGeneration(
        Guid workspaceId,
        long workspaceGeneration,
        long selectionGeneration,
        WorkspaceRevision revision,
        FormKey formKey)
    {
        return IsCurrentWorkspaceGeneration(workspaceId, workspaceGeneration, revision) &&
            SelectionGeneration == selectionGeneration &&
            SelectedRecordValue?.FormKey == formKey;
    }

    /// <summary>Checks whether comparison publication still matches both exact selectors.</summary>
    /// <param name="workspaceId">The captured workspace identity.</param>
    /// <param name="workspaceGeneration">The captured workspace generation.</param>
    /// <param name="selectionGeneration">The captured selection generation.</param>
    /// <param name="revision">The captured revision.</param>
    /// <param name="formKey">The captured record identity.</param>
    /// <param name="before">The captured prior selection.</param>
    /// <param name="after">The captured resulting selection.</param>
    /// <returns><see langword="true"/> when publication remains current.</returns>
    private bool IsCurrentComparisonGeneration(
        Guid workspaceId,
        long workspaceGeneration,
        long selectionGeneration,
        WorkspaceRevision revision,
        FormKey formKey,
        ReferenceRequest before,
        ReferenceRequest after)
    {
        return IsCurrentSelectionGeneration(workspaceId, workspaceGeneration, selectionGeneration, revision, formKey) &&
            SelectedBeforeContextValue is not null &&
            SelectedAfterContextValue is not null &&
            SameSelection(SelectedBeforeContextValue.Selection, before) &&
            SameSelection(SelectedAfterContextValue.Selection, after);
    }

    /// <summary>Compares exact reference selection values.</summary>
    /// <param name="left">The first selection.</param>
    /// <param name="right">The second selection.</param>
    /// <returns><see langword="true"/> when all selection fields match.</returns>
    private static bool SameSelection(ReferenceRequest left, ReferenceRequest right)
    {
        return left.FormKey == right.FormKey && left.Scope == right.Scope && left.ContainingModKey == right.ContainingModKey;
    }

    /// <summary>Publishes an unexpected selection failure only while captured identities remain current.</summary>
    /// <param name="workspaceId">The captured workspace identity.</param>
    /// <param name="workspaceGeneration">The captured workspace generation.</param>
    /// <param name="selectionGeneration">The captured selection generation.</param>
    /// <param name="revision">The captured revision.</param>
    /// <param name="formKey">The captured record identity.</param>
    /// <param name="exception">The unexpected presentation failure.</param>
    /// <param name="retryKind">The operation to repeat.</param>
    /// <returns>A task that completes after conditional publication.</returns>
    private Task PublishUnexpectedSelectionFailureAsync(
        Guid workspaceId,
        long workspaceGeneration,
        long selectionGeneration,
        WorkspaceRevision revision,
        FormKey formKey,
        Exception exception,
        RetryKind retryKind)
    {
        return UiDispatcher.InvokeAsync(() =>
        {
            if (IsCurrentSelectionGeneration(workspaceId, workspaceGeneration, selectionGeneration, revision, formKey))
            {
                PublishFailure(
                    new EngineError(EngineErrorCode.UnexpectedFailure, $"The major-record response could not be projected: {exception.Message}"),
                    retryKind);
            }
        });
    }

    /// <summary>Copies a typed engine failure while changing its successful value type.</summary>
    /// <typeparam name="TSource">The original successful value type.</typeparam>
    /// <typeparam name="TDestination">The requested successful value type.</typeparam>
    /// <param name="result">The failed source result.</param>
    /// <returns>The same failure metadata with a different successful value type.</returns>
    private static EngineResult<TDestination> CopyFailure<TSource, TDestination>(EngineResult<TSource> result)
    {
        return EngineResult<TDestination>.Failure(
            result.Error ?? new EngineError(EngineErrorCode.UnexpectedFailure, "The engine returned no value or failure reason."),
            result.WorkspaceId,
            result.OperationId,
            result.BaseRevision,
            result.ResultRevision,
            result.Warnings);
    }

    /// <summary>Creates a typed revision conflict with exact observed metadata.</summary>
    /// <typeparam name="T">The successful result type.</typeparam>
    /// <param name="workspace">The borrowed workspace.</param>
    /// <param name="expectedRevision">The expected revision, or <see langword="null"/>.</param>
    /// <param name="observedRevision">The observed revision, or <see langword="null"/>.</param>
    /// <param name="message">The presentation-safe conflict explanation.</param>
    /// <param name="warnings">Warnings observed before the conflict.</param>
    /// <returns>The typed failed result.</returns>
    private static EngineResult<T> RevisionFailure<T>(
        IPluginWorkspace workspace,
        WorkspaceRevision? expectedRevision,
        WorkspaceRevision? observedRevision,
        string message,
        IReadOnlyList<EngineWarning> warnings)
    {
        return EngineResult<T>.Failure(
            new EngineError(EngineErrorCode.RevisionConflict, message),
            workspace.WorkspaceId,
            baseRevision: expectedRevision,
            resultRevision: observedRevision,
            warnings: warnings);
    }

    /// <summary>Combines ordered warning collections without deduplication.</summary>
    /// <param name="warningSets">The warning collections in operation order.</param>
    /// <returns>All warnings in their supplied relative order.</returns>
    private static IReadOnlyList<EngineWarning> CombineWarnings(params IReadOnlyList<EngineWarning>[] warningSets)
    {
        return Array.AsReadOnly(warningSets.SelectMany(warnings => warnings).ToArray());
    }

    /// <summary>Cancels and disposes one replaceable generation token source.</summary>
    /// <param name="source">The token source field to clear.</param>
    private static void CancelAndDispose(ref CancellationTokenSource? source)
    {
        var previous = source;
        source = null;
        if (previous is null)
        {
            return;
        }

        previous.Cancel();
        previous.Dispose();
    }
}
