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
/// Presents revision-consistent FormList enumeration and exact two-context comparison without owning plugin state.
/// </summary>
public sealed partial class FormListBrowserViewModel : ViewModelBase, IFormListEditorHost, IWorkspaceEditParticipant, IDisposable
{
    /// <summary>The coordinator that exclusively owns and lends the active workspace.</summary>
    private readonly IWorkspaceCoordinator WorkspaceCoordinator;

    /// <summary>The navigation-scope admission boundary shared by editor and workspace transitions.</summary>
    private readonly IWorkspacePresentationOperationArbiter OperationArbiter;

    /// <summary>Projects detached engine JSON without interpreting record fields.</summary>
    private readonly RecordJsonTreeProjectionService JsonTreeProjectionService;

    /// <summary>Opens the independent bounded reference picker.</summary>
    private readonly IReferencePickerService ReferencePickerService;

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

    /// <summary>The winning records retained independently from their exact comparison contexts.</summary>
    private IReadOnlyList<FormListRecordViewModel> RecordsValue = Array.Empty<FormListRecordViewModel>();

    /// <summary>The filtered top-level plugin groups in explicit load-order order.</summary>
    private IReadOnlyList<PluginGroupViewModel> PluginGroupsValue = Array.Empty<PluginGroupViewModel>();

    /// <summary>The current hierarchical record-tree source.</summary>
    private HierarchicalTreeDataGridSource<IRecordTreeNodeViewModel> RecordTreeSourceValue;

    /// <summary>The currently selected winning record root.</summary>
    private FormListRecordViewModel? SelectedRecordValue;

    /// <summary>The exact context choices for the selected record.</summary>
    private IReadOnlyList<FormListContextOption> ContextOptionsValue = Array.Empty<FormListContextOption>();

    /// <summary>The currently selected prior context.</summary>
    private FormListContextOption? SelectedBeforeContextValue;

    /// <summary>The currently selected resulting context.</summary>
    private FormListContextOption? SelectedAfterContextValue;

    /// <summary>The engine-reported prior context and resolution state.</summary>
    private FormListContext? BeforeContextValue;

    /// <summary>The engine-reported resulting context and resolution state.</summary>
    private FormListContext? AfterContextValue;

    /// <summary>The complete projected prior JSON hierarchy.</summary>
    private IReadOnlyList<RecordJsonFieldNodeViewModel> BeforeFieldsValue = Array.Empty<RecordJsonFieldNodeViewModel>();

    /// <summary>The current hierarchical prior-field source.</summary>
    private HierarchicalTreeDataGridSource<RecordJsonFieldNodeViewModel> BeforeFieldSourceValue;

    /// <summary>The complete projected resulting JSON hierarchy.</summary>
    private IReadOnlyList<RecordJsonFieldNodeViewModel> AfterFieldsValue = Array.Empty<RecordJsonFieldNodeViewModel>();

    /// <summary>The current hierarchical resulting-field source.</summary>
    private HierarchicalTreeDataGridSource<RecordJsonFieldNodeViewModel> AfterFieldSourceValue;

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
    private string StatusTextValue = "No workspace is open.";

    /// <summary>Whether the selected record's comparison hierarchy is still resolving.</summary>
    private bool IsComparisonBusyValue;

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

    /// <summary>Initializes the FormList browser and subscribes to workspace replacement notifications.</summary>
    /// <param name="workspaceCoordinator">The application-wide workspace owner.</param>
    /// <param name="operationArbiter">The navigation-scope editor and workspace-transition admission boundary.</param>
    /// <param name="jsonTreeProjectionService">The lossless presentation-only JSON projector.</param>
    /// <param name="referencePickerService">The independent bounded reference picker.</param>
    /// <param name="uiDispatcher">The presentation dispatcher.</param>
    /// <param name="editorFactory">The factory that creates the browser-owned record editor.</param>
    /// <exception cref="ArgumentNullException">Thrown when any dependency is <see langword="null"/>.</exception>
    public FormListBrowserViewModel(
        IWorkspaceCoordinator workspaceCoordinator,
        IWorkspacePresentationOperationArbiter operationArbiter,
        RecordJsonTreeProjectionService jsonTreeProjectionService,
        IReferencePickerService referencePickerService,
        IUiDispatcher uiDispatcher,
        IFormListEditorViewModelFactory editorFactory)
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
            ?? throw new InvalidOperationException("The FormList editor factory returned no editor.");
        Editor.PropertyChanged += OnEditorPropertyChanged;
        RecordTreeSourceValue = CreateRecordTreeSource(PluginGroupsValue);
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

    /// <summary>Gets the revision-consistent participating plugins in engine order.</summary>
    public IReadOnlyList<PluginSummary> Plugins => PluginsValue;

    /// <summary>Gets the filtered winning FormList roots in the selected record order.</summary>
    public IReadOnlyList<FormListRecordViewModel> Records => RecordsValue;

    /// <summary>Gets the filtered top-level plugin groups in explicit load-order order.</summary>
    public IReadOnlyList<PluginGroupViewModel> PluginGroups => PluginGroupsValue;

    /// <summary>Gets the hierarchical record source containing plugins, major-record groups, and one row per winning record.</summary>
    public HierarchicalTreeDataGridSource<IRecordTreeNodeViewModel> RecordTreeSource => RecordTreeSourceValue;

    /// <summary>Gets the selected winning FormList root.</summary>
    public FormListRecordViewModel? SelectedRecord => SelectedRecordValue;

    /// <summary>Gets the winning selector followed by exact all-context selectors for the selected FormList.</summary>
    public IReadOnlyList<FormListContextOption> ContextOptions => ContextOptionsValue;

    /// <summary>Gets the selected prior context.</summary>
    public FormListContextOption? SelectedBeforeContext => SelectedBeforeContextValue;

    /// <summary>Gets the selected resulting context.</summary>
    public FormListContextOption? SelectedAfterContext => SelectedAfterContextValue;

    /// <summary>Gets the engine-reported prior context and explicit resolution status.</summary>
    public FormListContext? BeforeContext => BeforeContextValue;

    /// <summary>Gets the engine-reported resulting context and explicit resolution status.</summary>
    public FormListContext? AfterContext => AfterContextValue;

    /// <summary>Gets the complete projected prior JSON hierarchy.</summary>
    public IReadOnlyList<RecordJsonFieldNodeViewModel> BeforeFields => BeforeFieldsValue;

    /// <summary>Gets the hierarchical prior JSON source.</summary>
    public HierarchicalTreeDataGridSource<RecordJsonFieldNodeViewModel> BeforeFieldSource => BeforeFieldSourceValue;

    /// <summary>Gets the complete projected resulting JSON hierarchy.</summary>
    public IReadOnlyList<RecordJsonFieldNodeViewModel> AfterFields => AfterFieldsValue;

    /// <summary>Gets the hierarchical resulting JSON source.</summary>
    public HierarchicalTreeDataGridSource<RecordJsonFieldNodeViewModel> AfterFieldSource => AfterFieldSourceValue;

    /// <summary>Gets exact semantic change descriptors in the order returned by the engine.</summary>
    public IReadOnlyList<SemanticChangeDescriptor> SemanticChanges => SemanticChangesValue;

    /// <summary>Gets non-fatal load and comparison warnings in engine order.</summary>
    public IReadOnlyList<EngineWarning> Warnings => WarningsValue;

    /// <summary>Gets whether the current load or comparison produced non-fatal warnings.</summary>
    public bool HasWarnings => WarningsValue.Count > 0;

    /// <summary>Gets the stable typed error code for the current failure, or <see langword="null"/>.</summary>
    public EngineErrorCode? ErrorCode => ErrorCodeValue;

    /// <summary>Gets the current presentation-safe failure message, or <see langword="null"/>.</summary>
    public string? ErrorMessage => ErrorMessageValue;

    /// <summary>Gets whether a retryable typed failure is visible.</summary>
    public bool HasError => ErrorCodeValue.HasValue;

    /// <summary>Gets whether a workspace is currently published by the coordinator.</summary>
    public bool HasWorkspace => WorkspaceCoordinator.CurrentWorkspace is not null;

    /// <summary>Gets whether the active workspace admits a mutable output plugin.</summary>
    public bool IsEditingWorkspace => WorkspaceCoordinator.CurrentWorkspace?.Output is not null;

    /// <summary>Gets whether the selected record's comparison hierarchy is still resolving.</summary>
    public bool IsComparisonBusy => IsComparisonBusyValue;

    /// <summary>Gets the current browser operation status.</summary>
    public string StatusText => StatusTextValue;

    /// <summary>Gets whether the browser has a current operation message that should be presented.</summary>
    public bool HasStatusText => !string.IsNullOrWhiteSpace(StatusTextValue);

    /// <summary>Gets the physical major-record count for the active output or source plugin as footer text.</summary>
    public string ActivePluginRecordCountText
    {
        get
        {
            var activePlugin = PluginsValue.FirstOrDefault(plugin => plugin.Role == PluginRole.Output)
                ?? PluginsValue.FirstOrDefault(plugin => plugin.Role == PluginRole.Source);
            if (activePlugin is null)
            {
                return "Plugin records: 0";
            }

            return activePlugin.RecordCount.HasValue
                ? $"Plugin records: {activePlugin.RecordCount.Value:N0}"
                : "Plugin records: Unavailable";
        }
    }

    /// <summary>Gets the number of distinct major-record FormKeys across the admitted load order as footer text.</summary>
    public string LoadedRecordCountText
    {
        get
        {
            if (PluginsValue.Count == 0)
            {
                return "Unique loaded records: 0";
            }

            if (PluginsValue.Any(plugin => !plugin.UniqueRecordContributionCount.HasValue))
            {
                return "Unique loaded records: Unavailable";
            }

            return $"Unique loaded records: {PluginsValue.Sum(plugin => plugin.UniqueRecordContributionCount!.Value):N0}";
        }
    }

    /// <summary>Gets the engine-reported prior resolution and provenance as presentation text.</summary>
    public string BeforeProvenanceText => FormatContext(BeforeContextValue, SelectedBeforeContextValue);

    /// <summary>Gets the engine-reported resulting resolution and provenance as presentation text.</summary>
    public string AfterProvenanceText => FormatContext(AfterContextValue, SelectedAfterContextValue);

    /// <summary>Gets the latest accepted picker identity, including non-FormList records that cannot be navigated here.</summary>
    public string SelectedReferenceText => SelectedReferenceTextValue;

    /// <summary>Gets the command that retries the current load or comparison failure.</summary>
    public ICommand RetryCommand { get; }

    /// <summary>Gets the command that opens the independent reference picker.</summary>
    public ICommand PickReferenceCommand { get; }

    /// <summary>Gets the command that reloads the current workspace without retaining plugin state.</summary>
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

    /// <summary>Selects a winning root or exact context and compares it with the winning record context.</summary>
    /// <param name="record">The current record-tree node to select.</param>
    /// <returns>A task that completes after the resulting comparison is published or superseded.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="record"/> is <see langword="null"/>.</exception>
    public Task SelectRecordAsync(FormListRecordViewModel record)
    {
        return SelectRecordAsync(record, CancellationToken.None);
    }

    /// <summary>Selects a winning root or exact context and optionally binds its comparison to the owning operation.</summary>
    /// <param name="record">The current record-tree node to select.</param>
    /// <param name="cancellationToken">The token for the operation that owns this selection and comparison.</param>
    /// <returns>A task that completes after the resulting comparison is published, canceled, or superseded.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="record"/> is <see langword="null"/>.</exception>
    private Task SelectRecordAsync(
        FormListRecordViewModel record,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(record);
        if (IsDisposed)
        {
            return Task.CompletedTask;
        }

        var root = RecordsValue.FirstOrDefault(candidate =>
            ReferenceEquals(candidate, record) ||
            candidate.Contexts.Any(context => ReferenceEquals(context, record)));
        if (root is null)
        {
            return Task.CompletedTask;
        }

        var winning = root.ContextOptions.First(option => option.IsWinningOverride);
        var baseContext = root.ContextOptions.FirstOrDefault(option =>
            !option.IsWinningOverride &&
            option.ContainingModKey == root.FormKey.ModKey);
        var prior = record.Context.IsWinningOverride
            ? baseContext ?? root.ContextOptions.FirstOrDefault(option => !option.IsWinningOverride) ?? winning
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
    public Task SelectBeforeContextAsync(FormListContextOption? context)
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
    public Task SelectAfterContextAsync(FormListContextOption? context)
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
        var request = new ReferencePickerRequest(
            workspace.WorkspaceId,
            expectedRevision,
            SelectedRecordValue?.FormKey,
            RecordScope.WinningOverrides,
            containingModKey: null,
            allowNull: false,
            purpose: "Select a record to inspect in the FormList browser.");
        try
        {
            var selection = await ReferencePickerService.PickAsync(
                request,
                WorkspaceCancellation?.Token ?? CancellationToken.None).ConfigureAwait(false);
            if (selection is null)
            {
                return;
            }

            FormListRecordViewModel? navigationTarget = null;
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

    /// <summary>Builds winning records while retaining exact comparison contexts in engine order.</summary>
    /// <param name="summaries">All FormList contexts in engine order.</param>
    /// <param name="cancellationToken">A token observed throughout grouping and projection.</param>
    /// <returns>The grouped presentation records in engine enumeration order for subsequent presentation sorting.</returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is canceled.</exception>
    private static IReadOnlyList<FormListRecordViewModel> BuildRecordTree(
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

        var roots = new List<FormListRecordViewModel>(formKeyOrder.Count);
        foreach (var formKey in formKeyOrder)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var contexts = contextsByFormKey[formKey];
            var winningSummary = contexts[^1];
            var winningModKey = winningSummary.ContainingModKey ?? throw new InvalidOperationException(
                $"FormList context {winningSummary.FormKey} did not include its winning plugin provenance.");
            var winningOption = new FormListContextOption(
                new ReferenceRequest(formKey, RecordScope.WinningOverrides),
                $"Winning override ({winningModKey.FileName})",
                winningSummary.EditorId,
                sourcePath: null,
                loadOrderIndex: null,
                role: null);
            var options = new List<FormListContextOption>(contexts.Count + 1)
            {
                winningOption
            };
            var contextRecords = new List<FormListRecordViewModel>(contexts.Count);
            foreach (var context in contexts)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var option = CreateContextOption(context);
                options.Add(option);
                contextRecords.Add(new FormListRecordViewModel(
                    context.FormKey,
                    context.ContainingModKey!.Value,
                    context.EditorId,
                    context.OverrideCount,
                    option,
                    Array.Empty<FormListRecordViewModel>(),
                    new[] { option }));
            }

            roots.Add(new FormListRecordViewModel(
                formKey,
                winningModKey,
                winningSummary.EditorId,
                winningSummary.OverrideCount,
                winningOption,
                contextRecords,
                options));
        }

        return Array.AsReadOnly(roots.ToArray());
    }

    /// <summary>Creates one exact all-context selector from an engine summary.</summary>
    /// <param name="summary">The record context summary.</param>
    /// <returns>The immutable exact-context option.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the engine omits required all-context provenance.</exception>
    private static FormListContextOption CreateContextOption(FormListSummary summary)
    {
        if (!summary.ContainingModKey.HasValue ||
            summary.SourcePath is null ||
            !summary.LoadOrderIndex.HasValue ||
            !summary.Role.HasValue)
        {
            throw new InvalidOperationException(
                $"FormList context {summary.FormKey} did not include complete containing-plugin provenance.");
        }

        var label = $"[{summary.LoadOrderIndex.Value}] {summary.ContainingModKey.Value.FileName} ({summary.Role.Value})";
        return new FormListContextOption(
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
    private bool IsCurrentContextOption(FormListContextOption context)
    {
        return ContextOptionsValue.Any(option => ReferenceEquals(option, context));
    }

    /// <summary>Determines whether two immutable engine requests select the same exact record context.</summary>
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
        OnPropertyChanged(nameof(IsEditingWorkspace));
        SetComparisonBusy(false);
        PluginsValue = Array.Empty<PluginSummary>();
        OnPropertyChanged(nameof(Plugins));
        OnPropertyChanged(nameof(ActivePluginRecordCountText));
        OnPropertyChanged(nameof(LoadedRecordCountText));
        SetAllRecords(Array.Empty<FormListRecordViewModel>());
        SelectedRecordValue = null;
        ContextOptionsValue = Array.Empty<FormListContextOption>();
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
        SetBeforeFields(Array.Empty<RecordJsonFieldNodeViewModel>());
        SetAfterFields(Array.Empty<RecordJsonFieldNodeViewModel>());
        SemanticChangesValue = Array.Empty<SemanticChangeDescriptor>();
        OnPropertyChanged(nameof(SemanticChanges));
        SetWarnings(LoadWarnings);
        ClearError();
        RetryKindValue = RetryKind.None;
    }

    /// <summary>Publishes whether the right-side comparison hierarchy is still resolving.</summary>
    /// <param name="isBusy">Whether comparison work remains in progress.</param>
    private void SetComparisonBusy(bool isBusy)
    {
        SetProperty(ref IsComparisonBusyValue, isBusy, nameof(IsComparisonBusy));
    }

    /// <summary>Publishes a typed engine failure and enables the appropriate retry path.</summary>
    /// <param name="error">The engine failure, or <see langword="null"/> for a malformed result.</param>
    /// <param name="retryKind">The operation to retry.</param>
    private void PublishFailure(EngineError? error, RetryKind retryKind)
    {
        var resolvedError = error ?? new EngineError(
            EngineErrorCode.UnexpectedFailure,
            "The engine returned no value or failure reason.");
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
                        $"The FormList browser could not project the engine response: {exception.Message}"),
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
                        $"The FormList comparison could not be projected: {exception.Message}"),
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

    /// <summary>Replaces the record tree and groups winning roots beneath their winning plugin and major-record type.</summary>
    /// <param name="records">The new winning-root record list in the selected display order.</param>
    /// <exception cref="InvalidOperationException">Thrown when a winning record identifies a plugin absent from the workspace snapshot.</exception>
    private void SetRecords(IReadOnlyList<FormListRecordViewModel> records)
    {
        RecordsValue = records;
        var remainingRecords = records.ToList();
        var pluginGroups = new List<PluginGroupViewModel>(PluginsValue.Count);
        foreach (var plugin in PluginsValue.OrderBy(plugin => plugin.LoadOrderIndex))
        {
            var pluginRecords = remainingRecords
                .Where(record => GetWinningPlugin(record) == plugin.ModKey)
                .ToArray();
            remainingRecords.RemoveAll(record => GetWinningPlugin(record) == plugin.ModKey);
            IReadOnlyList<IRecordTreeNodeViewModel> recordTypes = pluginRecords.Length == 0
                ? Array.Empty<IRecordTreeNodeViewModel>()
                : new IRecordTreeNodeViewModel[]
                {
                    new RecordTypeGroupViewModel("Form Lists (FLST)", pluginRecords)
                };
            pluginGroups.Add(new PluginGroupViewModel(plugin, recordTypes));
        }

        if (remainingRecords.Count > 0)
        {
            throw new InvalidOperationException(
                $"The browser could not match {remainingRecords.Count} winning FormList record(s) to the workspace plugin list.");
        }

        PluginGroupsValue = Array.AsReadOnly(pluginGroups.ToArray());
        RecordTreeSourceValue = CreateRecordTreeSource(PluginGroupsValue);
        OnPropertyChanged(nameof(Records));
        OnPropertyChanged(nameof(PluginGroups));
        OnPropertyChanged(nameof(RecordTreeSource));
    }

    /// <summary>Gets the plugin containing the winning context represented by one root.</summary>
    /// <param name="record">The winning-root presentation record.</param>
    /// <returns>The containing plugin of the final load-order context.</returns>
    private static ModKey GetWinningPlugin(FormListRecordViewModel record)
    {
        return record.ContainingModKey;
    }

    /// <summary>Replaces the prior JSON tree and its hierarchical source.</summary>
    /// <param name="fields">The new prior JSON roots.</param>
    private void SetBeforeFields(IReadOnlyList<RecordJsonFieldNodeViewModel> fields)
    {
        BeforeFieldsValue = fields;
        BeforeFieldSourceValue = CreateFieldTreeSource(fields);
        OnPropertyChanged(nameof(BeforeFields));
        OnPropertyChanged(nameof(BeforeFieldSource));
    }

    /// <summary>Replaces the resulting JSON tree and its hierarchical source.</summary>
    /// <param name="fields">The new resulting JSON roots.</param>
    private void SetAfterFields(IReadOnlyList<RecordJsonFieldNodeViewModel> fields)
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
        OnPropertyChanged(nameof(HasWarnings));
    }

    /// <summary>Replaces the browser status text, including clearing a completed transient operation.</summary>
    /// <param name="status">The new status, or an empty string when no operation message should remain visible.</param>
    private void SetStatus(string status)
    {
        if (SetProperty(ref StatusTextValue, status, nameof(StatusText)))
        {
            OnPropertyChanged(nameof(HasStatusText));
        }
    }

    /// <summary>Formats exact engine resolution status and containing-plugin provenance.</summary>
    /// <param name="context">The engine-reported context, or <see langword="null"/> before a response.</param>
    /// <param name="selection">The pending exact selection, or <see langword="null"/>.</param>
    /// <returns>Explicit resolution and provenance text.</returns>
    private static string FormatContext(FormListContext? context, FormListContextOption? selection)
    {
        if (context is null)
        {
            return selection?.ProvenanceText ?? "No context selected.";
        }

        return context.ContainingModKey.HasValue
            ? $"{context.Status} | {context.ContainingModKey.Value.FileName} | index {context.LoadOrderIndex} | {context.Role} | {context.Path}"
            : context.Status.ToString();
    }

    /// <summary>Creates the hierarchical plugin, major-record-type, and winning-record source.</summary>
    /// <param name="groups">The top-level plugin groups in explicit load-order order.</param>
    /// <returns>The read-only hierarchical source.</returns>
    private static HierarchicalTreeDataGridSource<IRecordTreeNodeViewModel> CreateRecordTreeSource(
        IReadOnlyList<PluginGroupViewModel> groups)
    {
        return new HierarchicalTreeDataGridSource<IRecordTreeNodeViewModel>(groups)
            .WithHierarchicalExpanderTextColumn(
                "FormID",
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
    internal static HierarchicalTreeDataGridSource<RecordJsonFieldNodeViewModel> CreateFieldTreeSource(
        IReadOnlyList<RecordJsonFieldNodeViewModel> fields)
    {
        var source = new HierarchicalTreeDataGridSource<RecordJsonFieldNodeViewModel>(fields)
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
            CellTemplate = new FuncDataTemplate<RecordJsonFieldNodeViewModel>(
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
                "The engine returned no value or failure reason."),
            result.WorkspaceId,
            result.OperationId,
            result.BaseRevision,
            result.ResultRevision,
            result.Warnings);
    }

    /// <summary>Combines ordered warning collections without deduplicating plugin diagnostics.</summary>
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
            eventArgs.PropertyName != nameof(IWorkspaceCoordinator.CurrentWorkspace))
        {
            return;
        }

        UiDispatcher.Post(() =>
        {
            OnPropertyChanged(nameof(IsEditingWorkspace));
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
