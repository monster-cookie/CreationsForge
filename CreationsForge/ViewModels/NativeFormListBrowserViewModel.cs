using System.ComponentModel;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using CreationsForge.Commands;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Services;
using CreationsForge.Services.Interfaces;
using Mutagen.Bethesda.Plugins;

namespace CreationsForge.ViewModels;

/// <summary>
/// Presents revision-consistent native FormList enumeration and exact two-context comparison without owning native state.
/// </summary>
public sealed partial class NativeFormListBrowserViewModel : ViewModelBase, INativeFormListEditorHost, INativeWorkspaceEditParticipant, IDisposable
{
    /// <summary>The coordinator that exclusively owns and lends the active native workspace.</summary>
    private readonly INativeWorkspaceCoordinator WorkspaceCoordinator;

    /// <summary>The navigation-scope admission boundary shared by editor and workspace transitions.</summary>
    private readonly INativeWorkspacePresentationOperationArbiter OperationArbiter;

    /// <summary>Projects detached engine JSON without interpreting native fields.</summary>
    private readonly NativeJsonTreeProjectionService JsonTreeProjectionService;

    /// <summary>Opens the independent bounded native reference picker.</summary>
    private readonly INativeReferencePickerService ReferencePickerService;

    /// <summary>Publishes every bound state change on the presentation thread.</summary>
    private readonly IUiDispatcher UiDispatcher;

    /// <summary>The command that retries the current load or comparison failure.</summary>
    private readonly AsyncRelayCommand RetryRelayCommand;

    /// <summary>The command that opens the independent reference picker.</summary>
    private readonly AsyncRelayCommand PickReferenceRelayCommand;

    /// <summary>The command that reloads the active workspace through the revision-consistent browser pipeline.</summary>
    private readonly AsyncRelayCommand RefreshRelayCommand;

    /// <summary>Cancels loading, comparisons, and picker work for the current workspace generation.</summary>
    private CancellationTokenSource? WorkspaceCancellation;

    /// <summary>Cancels the current record/context comparison generation.</summary>
    private CancellationTokenSource? ComparisonCancellation;

    /// <summary>Identifies the current workspace generation for stale-result suppression.</summary>
    private long WorkspaceGeneration;

    /// <summary>Identifies the current comparison generation for stale-result suppression.</summary>
    private long ComparisonGeneration;

    /// <summary>The revision-consistent participating plugin list.</summary>
    private IReadOnlyList<PluginSummary> PluginsValue = Array.Empty<PluginSummary>();

    /// <summary>The grouped winning-root and exact-context record tree.</summary>
    private IReadOnlyList<NativeFormListRecordViewModel> RecordsValue = Array.Empty<NativeFormListRecordViewModel>();

    /// <summary>The filtered top-level native record-type groups.</summary>
    private IReadOnlyList<NativeRecordTypeGroupViewModel> RecordTypeGroupsValue = Array.Empty<NativeRecordTypeGroupViewModel>();

    /// <summary>The current hierarchical record-tree source.</summary>
    private HierarchicalTreeDataGridSource<INativeRecordTreeNodeViewModel> RecordTreeSourceValue;

    /// <summary>The currently selected winning record root.</summary>
    private NativeFormListRecordViewModel? SelectedRecordValue;

    /// <summary>The exact context choices for the selected record.</summary>
    private IReadOnlyList<NativeFormListContextOption> ContextOptionsValue = Array.Empty<NativeFormListContextOption>();

    /// <summary>The currently selected prior context.</summary>
    private NativeFormListContextOption? SelectedBeforeContextValue;

    /// <summary>The currently selected resulting context.</summary>
    private NativeFormListContextOption? SelectedAfterContextValue;

    /// <summary>The engine-reported prior context and resolution state.</summary>
    private FormListContext? BeforeContextValue;

    /// <summary>The engine-reported resulting context and resolution state.</summary>
    private FormListContext? AfterContextValue;

    /// <summary>The complete projected prior JSON hierarchy.</summary>
    private IReadOnlyList<NativeJsonFieldNodeViewModel> BeforeFieldsValue = Array.Empty<NativeJsonFieldNodeViewModel>();

    /// <summary>The current hierarchical prior-field source.</summary>
    private HierarchicalTreeDataGridSource<NativeJsonFieldNodeViewModel> BeforeFieldSourceValue;

    /// <summary>The complete projected resulting JSON hierarchy.</summary>
    private IReadOnlyList<NativeJsonFieldNodeViewModel> AfterFieldsValue = Array.Empty<NativeJsonFieldNodeViewModel>();

    /// <summary>The current hierarchical resulting-field source.</summary>
    private HierarchicalTreeDataGridSource<NativeJsonFieldNodeViewModel> AfterFieldSourceValue;

    /// <summary>The exact ordered semantic change descriptors from the engine.</summary>
    private IReadOnlyList<SemanticChangeDescriptor> SemanticChangesValue = Array.Empty<SemanticChangeDescriptor>();

    /// <summary>Warnings captured while loading the current browser snapshot.</summary>
    private IReadOnlyList<EngineWarning> LoadWarnings = Array.Empty<EngineWarning>();

    /// <summary>The currently visible combined load and comparison warnings.</summary>
    private IReadOnlyList<EngineWarning> WarningsValue = Array.Empty<EngineWarning>();

    /// <summary>The stable typed error code for the current failure.</summary>
    private EngineErrorCode? ErrorCodeValue;

    /// <summary>The current presentation-safe failure message.</summary>
    private string? ErrorMessageValue;

    /// <summary>The current browser operation status.</summary>
    private string StatusTextValue = "No native workspace is open.";

    /// <summary>The latest accepted reference-picker identity.</summary>
    private string SelectedReferenceTextValue = "No reference selected.";

    /// <summary>The latest revision-consistent workspace state.</summary>
    private WorkspaceState? WorkspaceStateValue;

    /// <summary>The operation kind retried by <see cref="RetryCommand"/>.</summary>
    private RetryKind RetryKindValue;

    /// <summary>Whether initial attachment has started the browser workflow.</summary>
    private bool IsStarted;

    /// <summary>Whether the navigation-owned browser scope has been disposed.</summary>
    private bool IsDisposed;

    /// <summary>Initializes the native FormList browser and subscribes to workspace replacement notifications.</summary>
    /// <param name="workspaceCoordinator">The application-wide workspace owner.</param>
    /// <param name="operationArbiter">The navigation-scope editor and workspace-transition admission boundary.</param>
    /// <param name="jsonTreeProjectionService">The lossless presentation-only JSON projector.</param>
    /// <param name="referencePickerService">The independent bounded native reference picker.</param>
    /// <param name="uiDispatcher">The presentation dispatcher.</param>
    /// <param name="editorFactory">The factory that creates the browser-owned native editor.</param>
    /// <exception cref="ArgumentNullException">Thrown when any dependency is <see langword="null"/>.</exception>
    public NativeFormListBrowserViewModel(
        INativeWorkspaceCoordinator workspaceCoordinator,
        INativeWorkspacePresentationOperationArbiter operationArbiter,
        NativeJsonTreeProjectionService jsonTreeProjectionService,
        INativeReferencePickerService referencePickerService,
        IUiDispatcher uiDispatcher,
        INativeFormListEditorViewModelFactory editorFactory)
    {
        ArgumentNullException.ThrowIfNull(workspaceCoordinator);
        ArgumentNullException.ThrowIfNull(operationArbiter);
        ArgumentNullException.ThrowIfNull(jsonTreeProjectionService);
        ArgumentNullException.ThrowIfNull(referencePickerService);
        ArgumentNullException.ThrowIfNull(uiDispatcher);
        ArgumentNullException.ThrowIfNull(editorFactory);
        WorkspaceCoordinator = workspaceCoordinator;
        OperationArbiter = operationArbiter;
        JsonTreeProjectionService = jsonTreeProjectionService;
        ReferencePickerService = referencePickerService;
        UiDispatcher = uiDispatcher;
        Editor = editorFactory.Create(this)
            ?? throw new InvalidOperationException("The native FormList editor factory returned no editor.");
        Editor.PropertyChanged += OnEditorPropertyChanged;
        RecordTreeSourceValue = CreateRecordTreeSource(RecordTypeGroupsValue);
        BeforeFieldSourceValue = CreateFieldTreeSource(BeforeFieldsValue);
        AfterFieldSourceValue = CreateFieldTreeSource(AfterFieldsValue);
        RetryRelayCommand = new AsyncRelayCommand(
            RetryAsync,
            () => HasError && HasWorkspace && RetryKindValue != RetryKind.None);
        PickReferenceRelayCommand = new AsyncRelayCommand(PickReferenceAsync, () => HasWorkspace && WorkspaceStateValue is not null);
        RefreshRelayCommand = new AsyncRelayCommand(
            () => RefreshAsync(),
            () => HasWorkspace);
        RetryCommand = RetryRelayCommand;
        PickReferenceCommand = PickReferenceRelayCommand;
        RefreshCommand = RefreshRelayCommand;
        WorkspaceCoordinator.PropertyChanged += OnWorkspaceCoordinatorPropertyChanged;
    }

    /// <summary>Gets the revision-consistent participating plugins in native engine order.</summary>
    public IReadOnlyList<PluginSummary> Plugins => PluginsValue;

    /// <summary>Gets the filtered winning FormList roots in alphabetical EditorID order.</summary>
    public IReadOnlyList<NativeFormListRecordViewModel> Records => RecordsValue;

    /// <summary>Gets the filtered top-level native record-type groups.</summary>
    public IReadOnlyList<NativeRecordTypeGroupViewModel> RecordTypeGroups => RecordTypeGroupsValue;

    /// <summary>Gets the hierarchical record source containing record-type groups, winning roots, and exact ordered contexts.</summary>
    public HierarchicalTreeDataGridSource<INativeRecordTreeNodeViewModel> RecordTreeSource => RecordTreeSourceValue;

    /// <summary>Gets the selected winning FormList root.</summary>
    public NativeFormListRecordViewModel? SelectedRecord => SelectedRecordValue;

    /// <summary>Gets the winning selector followed by exact all-context selectors for the selected FormList.</summary>
    public IReadOnlyList<NativeFormListContextOption> ContextOptions => ContextOptionsValue;

    /// <summary>Gets the selected prior context.</summary>
    public NativeFormListContextOption? SelectedBeforeContext => SelectedBeforeContextValue;

    /// <summary>Gets the selected resulting context.</summary>
    public NativeFormListContextOption? SelectedAfterContext => SelectedAfterContextValue;

    /// <summary>Gets the engine-reported prior context and explicit resolution status.</summary>
    public FormListContext? BeforeContext => BeforeContextValue;

    /// <summary>Gets the engine-reported resulting context and explicit resolution status.</summary>
    public FormListContext? AfterContext => AfterContextValue;

    /// <summary>Gets the complete projected prior JSON hierarchy.</summary>
    public IReadOnlyList<NativeJsonFieldNodeViewModel> BeforeFields => BeforeFieldsValue;

    /// <summary>Gets the hierarchical prior JSON source.</summary>
    public HierarchicalTreeDataGridSource<NativeJsonFieldNodeViewModel> BeforeFieldSource => BeforeFieldSourceValue;

    /// <summary>Gets the complete projected resulting JSON hierarchy.</summary>
    public IReadOnlyList<NativeJsonFieldNodeViewModel> AfterFields => AfterFieldsValue;

    /// <summary>Gets the hierarchical resulting JSON source.</summary>
    public HierarchicalTreeDataGridSource<NativeJsonFieldNodeViewModel> AfterFieldSource => AfterFieldSourceValue;

    /// <summary>Gets exact semantic change descriptors in the order returned by the engine.</summary>
    public IReadOnlyList<SemanticChangeDescriptor> SemanticChanges => SemanticChangesValue;

    /// <summary>Gets non-fatal load and comparison warnings in engine order.</summary>
    public IReadOnlyList<EngineWarning> Warnings => WarningsValue;

    /// <summary>Gets the stable typed error code for the current failure, or <see langword="null"/>.</summary>
    public EngineErrorCode? ErrorCode => ErrorCodeValue;

    /// <summary>Gets the current presentation-safe failure message, or <see langword="null"/>.</summary>
    public string? ErrorMessage => ErrorMessageValue;

    /// <summary>Gets whether a retryable typed failure is visible.</summary>
    public bool HasError => ErrorCodeValue.HasValue;

    /// <summary>Gets whether a native workspace is currently published by the coordinator.</summary>
    public bool HasWorkspace => WorkspaceCoordinator.CurrentWorkspace is not null;

    /// <summary>Gets the current browser operation status.</summary>
    public string StatusText => StatusTextValue;

    /// <summary>Gets the engine-reported prior resolution and provenance as presentation text.</summary>
    public string BeforeProvenanceText => FormatContext(BeforeContextValue, SelectedBeforeContextValue);

    /// <summary>Gets the engine-reported resulting resolution and provenance as presentation text.</summary>
    public string AfterProvenanceText => FormatContext(AfterContextValue, SelectedAfterContextValue);

    /// <summary>Gets the latest accepted picker identity, including non-FormList records that cannot be navigated here.</summary>
    public string SelectedReferenceText => SelectedReferenceTextValue;

    /// <summary>Gets the command that retries the current load or comparison failure.</summary>
    public ICommand RetryCommand { get; }

    /// <summary>Gets the command that opens the independent native reference picker.</summary>
    public ICommand PickReferenceCommand { get; }

    /// <summary>Gets the command that reloads the current workspace without retaining native state.</summary>
    public ICommand RefreshCommand { get; }

    /// <summary>Starts the browser once its navigation-owned view is attached.</summary>
    /// <returns>A task that completes after the initial workspace state is loaded or found empty.</returns>
    public Task StartAsync()
    {
        if (IsDisposed || IsStarted)
        {
            return Task.CompletedTask;
        }

        IsStarted = true;
        return BeginWorkspaceGeneration(WorkspaceCoordinator.CurrentWorkspace);
    }

    /// <summary>
    /// Reloads the current workspace through the guarded browser pipeline and optionally reselects one FormList.
    /// </summary>
    /// <param name="reselect">The FormList to select after a successful fresh load, or <see langword="null"/> to leave selection empty.</param>
    /// <param name="cancellationToken">A token that cancels this refresh without allowing stale publication.</param>
    /// <returns>A task that completes after the fresh snapshot and optional comparison publish or are superseded.</returns>
    /// <remarks>A workspace replacement, close, newer refresh, record selection, or context selection suppresses stale publication from this operation.</remarks>
    public Task RefreshAsync(FormKey? reselect = null, CancellationToken cancellationToken = default)
    {
        if (IsDisposed || !IsStarted)
        {
            return Task.CompletedTask;
        }

        return BeginWorkspaceGeneration(WorkspaceCoordinator.CurrentWorkspace, reselect, cancellationToken);
    }

    /// <summary>Selects a winning root or exact context and compares it with the winning native context.</summary>
    /// <param name="record">The current record-tree node to select.</param>
    /// <returns>A task that completes after the resulting comparison is published or superseded.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="record"/> is <see langword="null"/>.</exception>
    public Task SelectRecordAsync(NativeFormListRecordViewModel record)
    {
        return SelectRecordAsync(record, CancellationToken.None);
    }

    /// <summary>Selects a winning root or exact context and optionally binds its comparison to the owning operation.</summary>
    /// <param name="record">The current record-tree node to select.</param>
    /// <param name="cancellationToken">The token for the operation that owns this selection and comparison.</param>
    /// <returns>A task that completes after the resulting comparison is published, canceled, or superseded.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="record"/> is <see langword="null"/>.</exception>
    private Task SelectRecordAsync(
        NativeFormListRecordViewModel record,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(record);
        if (IsDisposed)
        {
            return Task.CompletedTask;
        }

        var root = RecordsValue.FirstOrDefault(candidate =>
            ReferenceEquals(candidate, record) ||
            candidate.Children.Any(child => ReferenceEquals(child, record)));
        if (root is null)
        {
            return Task.CompletedTask;
        }

        var winning = root.ContextOptions.First(option => option.IsWinningOverride);
        var prior = record.Context.IsWinningOverride
            ? root.ContextOptions.FirstOrDefault(option => !option.IsWinningOverride) ?? winning
            : root.ContextOptions.FirstOrDefault(option => SameSelection(option.Selection, record.Context.Selection)) ?? winning;
        SetProperty(ref SelectedRecordValue, root, nameof(SelectedRecord));
        SetProperty(ref ContextOptionsValue, root.ContextOptions, nameof(ContextOptions));
        SetProperty(ref SelectedBeforeContextValue, prior, nameof(SelectedBeforeContext));
        SetProperty(ref SelectedAfterContextValue, winning, nameof(SelectedAfterContext));
        PublishEditorSelection(record);
        return BeginComparisonGeneration(cancellationToken);
    }

    /// <summary>Selects the prior comparison context when it belongs to the active FormList.</summary>
    /// <param name="context">The requested prior context.</param>
    /// <returns>A task that completes after the resulting comparison is published or superseded.</returns>
    public Task SelectBeforeContextAsync(NativeFormListContextOption? context)
    {
        if (IsDisposed || context is null || !IsCurrentContextOption(context))
        {
            return Task.CompletedTask;
        }

        if (!SetProperty(ref SelectedBeforeContextValue, context, nameof(SelectedBeforeContext)))
        {
            return Task.CompletedTask;
        }

        return BeginComparisonGeneration();
    }

    /// <summary>Selects the resulting comparison context when it belongs to the active FormList.</summary>
    /// <param name="context">The requested resulting context.</param>
    /// <returns>A task that completes after the resulting comparison is published or superseded.</returns>
    public Task SelectAfterContextAsync(NativeFormListContextOption? context)
    {
        if (IsDisposed || context is null || !IsCurrentContextOption(context))
        {
            return Task.CompletedTask;
        }

        if (!SetProperty(ref SelectedAfterContextValue, context, nameof(SelectedAfterContext)))
        {
            return Task.CompletedTask;
        }

        return BeginComparisonGeneration();
    }

    /// <summary>Retries the most recent failed load or comparison against current state.</summary>
    /// <returns>A task that completes after the retry is published or superseded.</returns>
    public Task RetryAsync()
    {
        if (IsDisposed || !HasWorkspace)
        {
            return Task.CompletedTask;
        }

        return RetryKindValue switch
        {
            RetryKind.Comparison => BeginComparisonGeneration(),
            RetryKind.Load => BeginWorkspaceGeneration(WorkspaceCoordinator.CurrentWorkspace),
            _ => Task.CompletedTask,
        };
    }

    /// <summary>Releases coordinator subscriptions and cancels navigation-scope browser operations.</summary>
    public void Dispose()
    {
        if (IsDisposed)
        {
            return;
        }

        IsDisposed = true;
        WorkspaceCoordinator.PropertyChanged -= OnWorkspaceCoordinatorPropertyChanged;
        ClearEditorSelection();
        Editor.PropertyChanged -= OnEditorPropertyChanged;
        Editor.Dispose();
        CancelAndDispose(ref ComparisonCancellation);
        CancelAndDispose(ref WorkspaceCancellation);
    }

    /// <summary>Opens the independent picker and accepts only a selection for the current workspace revision.</summary>
    /// <returns>A task that completes after the accepted reference is displayed or its FormList is selected.</returns>
    internal async Task PickReferenceAsync()
    {
        if (IsDisposed ||
            WorkspaceStateValue is null ||
            WorkspaceCoordinator.CurrentWorkspace is not { } workspace)
        {
            return;
        }

        var generation = WorkspaceGeneration;
        var expectedRevision = WorkspaceStateValue.Revision;
        var request = new NativeReferencePickerRequest(
            workspace.WorkspaceId,
            expectedRevision,
            SelectedRecordValue?.FormKey,
            RecordScope.WinningOverrides,
            containingModKey: null,
            allowNull: false,
            purpose: "Select a native record to inspect in the FormList browser.");
        try
        {
            var selection = await ReferencePickerService.PickAsync(
                request,
                WorkspaceCancellation?.Token ?? CancellationToken.None).ConfigureAwait(false);
            if (selection is null)
            {
                return;
            }

            NativeFormListRecordViewModel? navigationTarget = null;
            await UiDispatcher.InvokeAsync(() =>
            {
                if (!IsCurrentWorkspaceGeneration(workspace.WorkspaceId, generation) ||
                    WorkspaceStateValue?.Revision != expectedRevision ||
                    selection.WorkspaceId != workspace.WorkspaceId ||
                    selection.Revision != expectedRevision)
                {
                    return;
                }

                if (selection.IsNull)
                {
                    SelectedReferenceTextValue = "Null reference";
                    OnPropertyChanged(nameof(SelectedReferenceText));
                    return;
                }

                var match = selection.Match!;
                SelectedReferenceTextValue = $"{match.FormKey} | {match.RecordType} | {match.EditorId ?? "(no EditorID)"}";
                OnPropertyChanged(nameof(SelectedReferenceText));
                if (string.Equals(match.RecordType, "FormList", StringComparison.Ordinal))
                {
                    navigationTarget = RecordsValue.FirstOrDefault(record => record.FormKey == match.FormKey);
                }
            }).ConfigureAwait(false);

            if (navigationTarget is not null)
            {
                Task navigationTask = Task.CompletedTask;
                await UiDispatcher.InvokeAsync(() => navigationTask = SelectRecordAsync(navigationTarget)).ConfigureAwait(false);
                await navigationTask.ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException) when (WorkspaceCancellation?.IsCancellationRequested != false)
        {
        }
    }

    /// <summary>Builds alphabetical winning roots while preserving exact context children in engine order.</summary>
    /// <param name="summaries">All native FormList contexts in engine order.</param>
    /// <param name="cancellationToken">A token observed throughout grouping and projection.</param>
    /// <returns>The grouped presentation records in alphabetical EditorID order with deterministic FormKey tie-breaking.</returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    private static IReadOnlyList<NativeFormListRecordViewModel> BuildRecordTree(
        IReadOnlyList<FormListSummary> summaries,
        CancellationToken cancellationToken)
    {
        var contextsByFormKey = new Dictionary<FormKey, List<FormListSummary>>();
        var formKeyOrder = new List<FormKey>();
        foreach (var summary in summaries)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (!contextsByFormKey.TryGetValue(summary.FormKey, out var contexts))
            {
                contexts = [];
                contextsByFormKey.Add(summary.FormKey, contexts);
                formKeyOrder.Add(summary.FormKey);
            }

            contexts.Add(summary);
        }

        var roots = new List<NativeFormListRecordViewModel>(formKeyOrder.Count);
        foreach (var formKey in formKeyOrder)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var contexts = contextsByFormKey[formKey];
            var winningSummary = contexts[^1];
            var winningOption = new NativeFormListContextOption(
                new ReferenceRequest(formKey, RecordScope.WinningOverrides),
                "Winning override",
                winningSummary.EditorId,
                sourcePath: null,
                loadOrderIndex: null,
                role: null);
            var options = new List<NativeFormListContextOption>(contexts.Count + 1)
            {
                winningOption
            };
            var children = new List<NativeFormListRecordViewModel>(contexts.Count);
            foreach (var context in contexts)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var option = CreateContextOption(context);
                options.Add(option);
                children.Add(new NativeFormListRecordViewModel(
                    context.FormKey,
                    context.EditorId,
                    context.OverrideCount,
                    option,
                    Array.Empty<NativeFormListRecordViewModel>(),
                    new[] { option }));
            }

            roots.Add(new NativeFormListRecordViewModel(
                formKey,
                winningSummary.EditorId,
                winningSummary.OverrideCount,
                winningOption,
                children,
                options));
        }

        return Array.AsReadOnly(roots
            .OrderBy(root => string.IsNullOrWhiteSpace(root.EditorId) ? 1 : 0)
            .ThenBy(root => root.EditorId, StringComparer.OrdinalIgnoreCase)
            .ThenBy(root => root.FormKey.ToString(), StringComparer.OrdinalIgnoreCase)
            .ToArray());
    }

    /// <summary>Creates one exact all-context selector from an engine summary.</summary>
    /// <param name="summary">The native context summary.</param>
    /// <returns>The immutable exact-context option.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the engine omits required all-context provenance.</exception>
    private static NativeFormListContextOption CreateContextOption(FormListSummary summary)
    {
        if (!summary.ContainingModKey.HasValue ||
            summary.SourcePath is null ||
            !summary.LoadOrderIndex.HasValue ||
            !summary.Role.HasValue)
        {
            throw new InvalidOperationException(
                $"Native FormList context {summary.FormKey} did not include complete containing-plugin provenance.");
        }

        var label = $"[{summary.LoadOrderIndex.Value}] {summary.ContainingModKey.Value.FileName} ({summary.Role.Value})";
        return new NativeFormListContextOption(
            new ReferenceRequest(summary.FormKey, RecordScope.AllContexts, summary.ContainingModKey),
            label,
            summary.EditorId,
            summary.SourcePath,
            summary.LoadOrderIndex,
            summary.Role);
    }

    /// <summary>Determines whether a context option belongs to the current selected FormList.</summary>
    /// <param name="context">The option to validate.</param>
    /// <returns><see langword="true"/> when the option exactly matches a current option.</returns>
    private bool IsCurrentContextOption(NativeFormListContextOption context)
    {
        return ContextOptionsValue.Any(option => ReferenceEquals(option, context));
    }

    /// <summary>Determines whether two immutable engine requests select the same exact native context.</summary>
    /// <param name="left">The first selection.</param>
    /// <param name="right">The second selection.</param>
    /// <returns><see langword="true"/> when identity, scope, and containing plugin all match.</returns>
    private static bool SameSelection(ReferenceRequest left, ReferenceRequest right)
    {
        return left.FormKey == right.FormKey &&
            left.Scope == right.Scope &&
            left.ContainingModKey == right.ContainingModKey;
    }

    /// <summary>Checks all identities captured by a workspace load before publishing its result.</summary>
    /// <param name="workspaceId">The captured workspace identity.</param>
    /// <param name="generation">The captured workspace generation.</param>
    /// <returns><see langword="true"/> when the generation is still current.</returns>
    private bool IsCurrentWorkspaceGeneration(Guid workspaceId, long generation)
    {
        return !IsDisposed &&
            WorkspaceGeneration == generation &&
            WorkspaceCoordinator.CurrentWorkspace?.WorkspaceId == workspaceId;
    }

    /// <summary>Checks workspace, revision, record, and both exact selections before publishing a comparison.</summary>
    /// <param name="workspaceId">The captured workspace identity.</param>
    /// <param name="workspaceGeneration">The captured workspace generation.</param>
    /// <param name="comparisonGeneration">The captured comparison generation.</param>
    /// <param name="revision">The captured exact workspace revision.</param>
    /// <param name="before">The captured prior selection.</param>
    /// <param name="after">The captured resulting selection.</param>
    /// <returns><see langword="true"/> when every captured identity remains current.</returns>
    private bool IsCurrentComparisonGeneration(
        Guid workspaceId,
        long workspaceGeneration,
        long comparisonGeneration,
        WorkspaceRevision revision,
        ReferenceRequest before,
        ReferenceRequest after)
    {
        return IsCurrentWorkspaceGeneration(workspaceId, workspaceGeneration) &&
            ComparisonGeneration == comparisonGeneration &&
            WorkspaceStateValue?.Revision == revision &&
            SelectedBeforeContextValue is not null &&
            SelectedAfterContextValue is not null &&
            SameSelection(SelectedBeforeContextValue.Selection, before) &&
            SameSelection(SelectedAfterContextValue.Selection, after);
    }

    /// <summary>Clears every browser value that was derived from a previous workspace.</summary>
    private void ClearWorkspacePresentation()
    {
        WorkspaceStateValue = null;
        PluginsValue = Array.Empty<PluginSummary>();
        OnPropertyChanged(nameof(Plugins));
        SetAllRecords(Array.Empty<NativeFormListRecordViewModel>());
        SelectedRecordValue = null;
        ContextOptionsValue = Array.Empty<NativeFormListContextOption>();
        SelectedBeforeContextValue = null;
        SelectedAfterContextValue = null;
        OnPropertyChanged(nameof(SelectedRecord));
        OnPropertyChanged(nameof(ContextOptions));
        OnPropertyChanged(nameof(SelectedBeforeContext));
        OnPropertyChanged(nameof(SelectedAfterContext));
        SelectedReferenceTextValue = "No reference selected.";
        OnPropertyChanged(nameof(SelectedReferenceText));
        LoadWarnings = Array.Empty<EngineWarning>();
        SetWarnings(LoadWarnings);
        ClearComparisonPresentation();
        ClearError();
        RetryKindValue = RetryKind.None;
    }

    /// <summary>Clears all JSON, context outcomes, and semantic changes from a superseded comparison.</summary>
    private void ClearComparisonPresentation()
    {
        BeforeContextValue = null;
        AfterContextValue = null;
        OnPropertyChanged(nameof(BeforeContext));
        OnPropertyChanged(nameof(AfterContext));
        OnPropertyChanged(nameof(BeforeProvenanceText));
        OnPropertyChanged(nameof(AfterProvenanceText));
        SetBeforeFields(Array.Empty<NativeJsonFieldNodeViewModel>());
        SetAfterFields(Array.Empty<NativeJsonFieldNodeViewModel>());
        SemanticChangesValue = Array.Empty<SemanticChangeDescriptor>();
        OnPropertyChanged(nameof(SemanticChanges));
        SetWarnings(LoadWarnings);
        ClearError();
        RetryKindValue = RetryKind.None;
    }

    /// <summary>Publishes a typed engine failure and enables the appropriate retry path.</summary>
    /// <param name="error">The engine failure, or <see langword="null"/> for a malformed result.</param>
    /// <param name="retryKind">The operation to retry.</param>
    private void PublishFailure(EngineError? error, RetryKind retryKind)
    {
        var resolvedError = error ?? new EngineError(
            EngineErrorCode.UnexpectedFailure,
            "The native engine returned no value or failure reason.");
        ErrorCodeValue = resolvedError.Code;
        ErrorMessageValue = resolvedError.Message;
        RetryKindValue = retryKind;
        OnPropertyChanged(nameof(ErrorCode));
        OnPropertyChanged(nameof(ErrorMessage));
        OnPropertyChanged(nameof(HasError));
        RetryRelayCommand.RaiseCanExecuteChanged();
        SetStatus($"{resolvedError.Code}: {resolvedError.Message}");
    }

    /// <summary>Publishes an unexpected load failure only when its workspace generation remains current.</summary>
    /// <param name="workspaceId">The captured workspace identity.</param>
    /// <param name="generation">The captured workspace generation.</param>
    /// <param name="exception">The unexpected presentation failure.</param>
    /// <param name="retryKind">The operation to retry.</param>
    /// <returns>A task that completes after conditional UI-thread publication.</returns>
    private Task PublishUnexpectedFailureAsync(
        Guid workspaceId,
        long generation,
        Exception exception,
        RetryKind retryKind)
    {
        return UiDispatcher.InvokeAsync(() =>
        {
            if (IsCurrentWorkspaceGeneration(workspaceId, generation))
            {
                PublishFailure(
                    new EngineError(
                        EngineErrorCode.UnexpectedFailure,
                        $"The native FormList browser could not project the engine response: {exception.Message}"),
                    retryKind);
            }
        });
    }

    /// <summary>Publishes an unexpected comparison failure only when all captured identities remain current.</summary>
    /// <param name="workspaceId">The captured workspace identity.</param>
    /// <param name="workspaceGeneration">The captured workspace generation.</param>
    /// <param name="comparisonGeneration">The captured comparison generation.</param>
    /// <param name="revision">The captured workspace revision.</param>
    /// <param name="before">The captured prior selection.</param>
    /// <param name="after">The captured resulting selection.</param>
    /// <param name="exception">The unexpected presentation failure.</param>
    /// <returns>A task that completes after conditional UI-thread publication.</returns>
    private Task PublishUnexpectedComparisonFailureAsync(
        Guid workspaceId,
        long workspaceGeneration,
        long comparisonGeneration,
        WorkspaceRevision revision,
        ReferenceRequest before,
        ReferenceRequest after,
        Exception exception)
    {
        return UiDispatcher.InvokeAsync(() =>
        {
            if (IsCurrentComparisonGeneration(
                workspaceId,
                workspaceGeneration,
                comparisonGeneration,
                revision,
                before,
                after))
            {
                PublishFailure(
                    new EngineError(
                        EngineErrorCode.UnexpectedFailure,
                        $"The native FormList comparison could not be projected: {exception.Message}"),
                    RetryKind.Comparison);
            }
        });
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

    /// <summary>Replaces the record tree and its hierarchical source.</summary>
    /// <param name="records">The new winning-root record list.</param>
    private void SetRecords(IReadOnlyList<NativeFormListRecordViewModel> records)
    {
        RecordsValue = records;
        RecordTypeGroupsValue = records.Count == 0
            ? Array.Empty<NativeRecordTypeGroupViewModel>()
            : new[]
            {
                new NativeRecordTypeGroupViewModel("Form Lists (FLST)", records)
            };
        RecordTreeSourceValue = CreateRecordTreeSource(RecordTypeGroupsValue);
        OnPropertyChanged(nameof(Records));
        OnPropertyChanged(nameof(RecordTypeGroups));
        OnPropertyChanged(nameof(RecordTreeSource));
    }

    /// <summary>Replaces the prior JSON tree and its hierarchical source.</summary>
    /// <param name="fields">The new prior JSON roots.</param>
    private void SetBeforeFields(IReadOnlyList<NativeJsonFieldNodeViewModel> fields)
    {
        BeforeFieldsValue = fields;
        BeforeFieldSourceValue = CreateFieldTreeSource(fields);
        OnPropertyChanged(nameof(BeforeFields));
        OnPropertyChanged(nameof(BeforeFieldSource));
    }

    /// <summary>Replaces the resulting JSON tree and its hierarchical source.</summary>
    /// <param name="fields">The new resulting JSON roots.</param>
    private void SetAfterFields(IReadOnlyList<NativeJsonFieldNodeViewModel> fields)
    {
        AfterFieldsValue = fields;
        AfterFieldSourceValue = CreateFieldTreeSource(fields);
        OnPropertyChanged(nameof(AfterFields));
        OnPropertyChanged(nameof(AfterFieldSource));
    }

    /// <summary>Replaces visible warnings while preserving their supplied order.</summary>
    /// <param name="warnings">The new warnings.</param>
    private void SetWarnings(IReadOnlyList<EngineWarning> warnings)
    {
        WarningsValue = Array.AsReadOnly(warnings.ToArray());
        OnPropertyChanged(nameof(Warnings));
    }

    /// <summary>Replaces the browser status text.</summary>
    /// <param name="status">The new non-empty status.</param>
    private void SetStatus(string status)
    {
        SetProperty(ref StatusTextValue, status, nameof(StatusText));
    }

    /// <summary>Formats exact engine resolution status and containing-plugin provenance.</summary>
    /// <param name="context">The engine-reported context, or <see langword="null"/> before a response.</param>
    /// <param name="selection">The pending exact selection, or <see langword="null"/>.</param>
    /// <returns>Explicit resolution and provenance text.</returns>
    private static string FormatContext(FormListContext? context, NativeFormListContextOption? selection)
    {
        if (context is null)
        {
            return selection?.ProvenanceText ?? "No context selected.";
        }

        return context.ContainingModKey.HasValue
            ? $"{context.Status} | {context.ContainingModKey.Value.FileName} | index {context.LoadOrderIndex} | {context.Role} | {context.Path}"
            : context.Status.ToString();
    }

    /// <summary>Creates the hierarchical record-type, winning-root, and exact-context record source.</summary>
    /// <param name="groups">The top-level record-type groups.</param>
    /// <returns>The read-only hierarchical source.</returns>
    private static HierarchicalTreeDataGridSource<INativeRecordTreeNodeViewModel> CreateRecordTreeSource(
        IReadOnlyList<NativeRecordTypeGroupViewModel> groups)
    {
        return new HierarchicalTreeDataGridSource<INativeRecordTreeNodeViewModel>(groups)
            .WithHierarchicalExpanderTextColumn(
                "FormKey",
                record => record.PrimaryText,
                record => record.TreeChildren,
                record => record.IsExpanded,
                record => record.HasChildren,
                options => options.BeginEditGestures = BeginEditGestures.None)
            .WithTextColumn("EditorID", record => record.EditorIdText, options => options.BeginEditGestures = BeginEditGestures.None)
            .WithTextColumn("Context", record => record.ContextText, options => options.BeginEditGestures = BeginEditGestures.None)
            .WithTextColumn("Overrides", record => record.OverrideCountText, options => options.BeginEditGestures = BeginEditGestures.None);
    }

    /// <summary>Creates a hierarchical field source that displays every JSON name, kind, and scalar or container marker.</summary>
    /// <param name="fields">The projected JSON roots.</param>
    /// <returns>The read-only hierarchical source.</returns>
    private static HierarchicalTreeDataGridSource<NativeJsonFieldNodeViewModel> CreateFieldTreeSource(
        IReadOnlyList<NativeJsonFieldNodeViewModel> fields)
    {
        var source = new HierarchicalTreeDataGridSource<NativeJsonFieldNodeViewModel>(fields)
            .WithHierarchicalExpanderTextColumn(
                "Field",
                field => field.Name,
                field => field.Children,
                field => field.IsExpanded,
                field => field.HasChildren,
                options => options.BeginEditGestures = BeginEditGestures.None)
            .WithTextColumn("Kind", field => field.KindText, options => options.BeginEditGestures = BeginEditGestures.None);
        source.Columns.Add(new TreeDataGridTemplateColumn
        {
            Header = "Value",
            BeginEditGestures = BeginEditGestures.None,
            CellTemplate = new FuncDataTemplate<NativeJsonFieldNodeViewModel>(
                (field, _) => CreateFieldValueCell(field))
        });
        return source;
    }

    /// <summary>Copies a typed engine failure while changing only its successful value type.</summary>
    /// <typeparam name="TSource">The original successful value type.</typeparam>
    /// <typeparam name="TDestination">The requested successful value type.</typeparam>
    /// <param name="result">The failed source result.</param>
    /// <returns>The same failure metadata with a different successful value type.</returns>
    private static EngineResult<TDestination> CopyFailure<TSource, TDestination>(EngineResult<TSource> result)
    {
        return EngineResult<TDestination>.Failure(
            result.Error ?? new EngineError(
                EngineErrorCode.UnexpectedFailure,
                "The native engine returned no value or failure reason."),
            result.WorkspaceId,
            result.OperationId,
            result.BaseRevision,
            result.ResultRevision,
            result.Warnings);
    }

    /// <summary>Combines ordered warning collections without deduplicating native diagnostics.</summary>
    /// <param name="warningSets">The warning collections in operation order.</param>
    /// <returns>All supplied warnings in their original relative order.</returns>
    private static IReadOnlyList<EngineWarning> CombineWarnings(params IReadOnlyList<EngineWarning>[] warningSets)
    {
        return Array.AsReadOnly(warningSets.SelectMany(warnings => warnings).ToArray());
    }

    /// <summary>Creates the typed revision conflict used by stale browser operations.</summary>
    /// <param name="message">The presentation-safe conflict explanation.</param>
    /// <returns>The typed engine error.</returns>
    private static EngineError CreateRevisionError(string message)
    {
        return new EngineError(EngineErrorCode.RevisionConflict, message);
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

    /// <summary>Starts a new workspace generation when the coordinator publishes replacement or close.</summary>
    /// <param name="sender">The workspace coordinator.</param>
    /// <param name="eventArgs">The changed coordinator property.</param>
    private void OnWorkspaceCoordinatorPropertyChanged(object? sender, PropertyChangedEventArgs eventArgs)
    {
        if (eventArgs.PropertyName is not null &&
            eventArgs.PropertyName != nameof(INativeWorkspaceCoordinator.CurrentWorkspace))
        {
            return;
        }

        UiDispatcher.Post(() =>
        {
            if (!IsDisposed && IsStarted)
            {
                _ = BeginWorkspaceGeneration(WorkspaceCoordinator.CurrentWorkspace);
            }
        });
    }

    /// <summary>Identifies which browser operation a retry repeats.</summary>
    private enum RetryKind
    {
        /// <summary>No failed operation is pending.</summary>
        None,

        /// <summary>The complete workspace snapshot load failed.</summary>
        Load,

        /// <summary>The selected two-context comparison failed.</summary>
        Comparison
    }
}
