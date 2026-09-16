using System.ComponentModel;
using System.Windows.Input;
using Avalonia.Controls;
using CreationsForge.Commands;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Services;
using CreationsForge.Services.Interfaces;

namespace CreationsForge.ViewModels;

/// <summary>Coordinates paged major-record discovery, exact context selection, and native field comparison.</summary>
public sealed partial class MajorRecordBrowserViewModel : ViewModelBase, IDisposable
{
    /// <summary>The bounded number of winning contexts requested per desktop page.</summary>
    internal const int PageSize = 100;

    /// <summary>Owns and serializes access to the application workspace.</summary>
    private readonly IWorkspaceCoordinator WorkspaceCoordinator;

    /// <summary>Projects detached native field JSON into ordered presentation nodes.</summary>
    private readonly RecordJsonTreeProjectionService JsonTreeProjectionService;

    /// <summary>Publishes bound state changes on the Avalonia UI thread.</summary>
    private readonly IUiDispatcher UiDispatcher;

    /// <summary>Reloads the first winning-record page.</summary>
    private readonly AsyncRelayCommand RefreshRelayCommand;

    /// <summary>Loads the next engine-issued winning-record page.</summary>
    private readonly AsyncRelayCommand LoadMoreRelayCommand;

    /// <summary>Repeats the most recent failed browser operation.</summary>
    private readonly AsyncRelayCommand RetryRelayCommand;

    /// <summary>Cancels the current workspace page generation.</summary>
    private CancellationTokenSource? WorkspaceCancellation;

    /// <summary>Cancels current context discovery and comparison work.</summary>
    private CancellationTokenSource? SelectionCancellation;

    /// <summary>Identifies the current workspace page generation.</summary>
    private long WorkspaceGeneration;

    /// <summary>Identifies the current record or context selection generation.</summary>
    private long SelectionGeneration;

    /// <summary>The currently accepted workspace revision.</summary>
    private WorkspaceRevision? RevisionValue;

    /// <summary>The continuation token for the next winning-record page.</summary>
    private string? ContinuationTokenValue;

    /// <summary>The loaded winning major-record rows.</summary>
    private IReadOnlyList<MajorRecordViewModel> RecordsValue = Array.Empty<MajorRecordViewModel>();

    /// <summary>The loaded records grouped by stable record family.</summary>
    private IReadOnlyList<RecordTypeGroupViewModel> RecordGroupsValue = Array.Empty<RecordTypeGroupViewModel>();

    /// <summary>The hierarchical record-family source.</summary>
    private HierarchicalTreeDataGridSource<IRecordTreeNodeViewModel> RecordTreeSourceValue;

    /// <summary>The selected winning record row.</summary>
    private MajorRecordViewModel? SelectedRecordValue;

    /// <summary>The winning and exact plugin contexts for the selected record.</summary>
    private IReadOnlyList<MajorRecordContextOption> ContextOptionsValue = Array.Empty<MajorRecordContextOption>();

    /// <summary>The selected prior context.</summary>
    private MajorRecordContextOption? SelectedBeforeContextValue;

    /// <summary>The selected resulting context.</summary>
    private MajorRecordContextOption? SelectedAfterContextValue;

    /// <summary>The engine-reported prior context.</summary>
    private FormListContext? BeforeContextValue;

    /// <summary>The engine-reported resulting context.</summary>
    private FormListContext? AfterContextValue;

    /// <summary>The projected prior native field tree.</summary>
    private IReadOnlyList<RecordJsonFieldNodeViewModel> BeforeFieldsValue = Array.Empty<RecordJsonFieldNodeViewModel>();

    /// <summary>The hierarchical prior-field source.</summary>
    private HierarchicalTreeDataGridSource<RecordJsonFieldNodeViewModel> BeforeFieldSourceValue;

    /// <summary>The projected resulting native field tree.</summary>
    private IReadOnlyList<RecordJsonFieldNodeViewModel> AfterFieldsValue = Array.Empty<RecordJsonFieldNodeViewModel>();

    /// <summary>The hierarchical resulting-field source.</summary>
    private HierarchicalTreeDataGridSource<RecordJsonFieldNodeViewModel> AfterFieldSourceValue;

    /// <summary>The ordered native-value semantic changes.</summary>
    private IReadOnlyList<SemanticChangeDescriptor> SemanticChangesValue = Array.Empty<SemanticChangeDescriptor>();

    /// <summary>Warnings retained from the current record-page generation.</summary>
    private IReadOnlyList<EngineWarning> PageWarnings = Array.Empty<EngineWarning>();

    /// <summary>The currently visible page, context, and comparison warnings.</summary>
    private IReadOnlyList<EngineWarning> WarningsValue = Array.Empty<EngineWarning>();

    /// <summary>The stable typed error code for the current failure.</summary>
    private EngineErrorCode? ErrorCodeValue;

    /// <summary>The presentation-safe current failure message.</summary>
    private string? ErrorMessageValue;

    /// <summary>The current browser operation status.</summary>
    private string StatusTextValue = "No workspace is open.";

    /// <summary>Whether record paging is active.</summary>
    private bool IsBusyValue;

    /// <summary>Whether context discovery or field comparison is active.</summary>
    private bool IsComparisonBusyValue;

    /// <summary>The operation repeated by the retry command.</summary>
    private RetryKind RetryKindValue;

    /// <summary>Whether attachment started the browser workflow.</summary>
    private bool IsStarted;

    /// <summary>Whether the navigation-owned browser scope was disposed.</summary>
    private bool IsDisposed;

    /// <summary>Initializes the read-only major-record browser.</summary>
    /// <param name="workspaceCoordinator">The application-wide workspace owner.</param>
    /// <param name="jsonTreeProjectionService">The lossless presentation-only JSON projector.</param>
    /// <param name="uiDispatcher">The presentation dispatcher.</param>
    /// <exception cref="ArgumentNullException">Thrown when a required dependency is <see langword="null"/>.</exception>
    public MajorRecordBrowserViewModel(
        IWorkspaceCoordinator workspaceCoordinator,
        RecordJsonTreeProjectionService jsonTreeProjectionService,
        IUiDispatcher uiDispatcher)
    {
        ArgumentNullException.ThrowIfNull(workspaceCoordinator);
        ArgumentNullException.ThrowIfNull(jsonTreeProjectionService);
        ArgumentNullException.ThrowIfNull(uiDispatcher);
        WorkspaceCoordinator = workspaceCoordinator;
        JsonTreeProjectionService = jsonTreeProjectionService;
        UiDispatcher = uiDispatcher;
        RecordTreeSourceValue = CreateRecordTreeSource(RecordGroupsValue);
        BeforeFieldSourceValue = FormListBrowserViewModel.CreateFieldTreeSource(BeforeFieldsValue);
        AfterFieldSourceValue = FormListBrowserViewModel.CreateFieldTreeSource(AfterFieldsValue);
        RefreshRelayCommand = new AsyncRelayCommand(RefreshAsync, () => HasWorkspace && !IsBusy);
        LoadMoreRelayCommand = new AsyncRelayCommand(LoadMoreAsync, () => HasWorkspace && HasMoreRecords && !IsBusy);
        RetryRelayCommand = new AsyncRelayCommand(RetryAsync, () => HasError && HasWorkspace && RetryKindValue != RetryKind.None);
        RefreshCommand = RefreshRelayCommand;
        LoadMoreCommand = LoadMoreRelayCommand;
        RetryCommand = RetryRelayCommand;
        WorkspaceCoordinator.PropertyChanged += OnWorkspaceCoordinatorPropertyChanged;
    }

    /// <summary>Gets the loaded winning major-record rows.</summary>
    public IReadOnlyList<MajorRecordViewModel> Records => RecordsValue;

    /// <summary>Gets the loaded records grouped by stable record family.</summary>
    public IReadOnlyList<RecordTypeGroupViewModel> RecordGroups => RecordGroupsValue;

    /// <summary>Gets the hierarchical major-record family source.</summary>
    public HierarchicalTreeDataGridSource<IRecordTreeNodeViewModel> RecordTreeSource => RecordTreeSourceValue;

    /// <summary>Gets the selected winning record row.</summary>
    public MajorRecordViewModel? SelectedRecord => SelectedRecordValue;

    /// <summary>Gets the winning selector followed by exact plugin context selectors.</summary>
    public IReadOnlyList<MajorRecordContextOption> ContextOptions => ContextOptionsValue;

    /// <summary>Gets the selected prior context.</summary>
    public MajorRecordContextOption? SelectedBeforeContext => SelectedBeforeContextValue;

    /// <summary>Gets the selected resulting context.</summary>
    public MajorRecordContextOption? SelectedAfterContext => SelectedAfterContextValue;

    /// <summary>Gets the engine-reported prior context.</summary>
    public FormListContext? BeforeContext => BeforeContextValue;

    /// <summary>Gets the engine-reported resulting context.</summary>
    public FormListContext? AfterContext => AfterContextValue;

    /// <summary>Gets the prior context's exact resolution and provenance text.</summary>
    public string BeforeProvenanceText => FormatContext(BeforeContextValue, SelectedBeforeContextValue);

    /// <summary>Gets the resulting context's exact resolution and provenance text.</summary>
    public string AfterProvenanceText => FormatContext(AfterContextValue, SelectedAfterContextValue);

    /// <summary>Gets the projected prior native field hierarchy.</summary>
    public IReadOnlyList<RecordJsonFieldNodeViewModel> BeforeFields => BeforeFieldsValue;

    /// <summary>Gets the hierarchical prior-field source.</summary>
    public HierarchicalTreeDataGridSource<RecordJsonFieldNodeViewModel> BeforeFieldSource => BeforeFieldSourceValue;

    /// <summary>Gets the projected resulting native field hierarchy.</summary>
    public IReadOnlyList<RecordJsonFieldNodeViewModel> AfterFields => AfterFieldsValue;

    /// <summary>Gets the hierarchical resulting-field source.</summary>
    public HierarchicalTreeDataGridSource<RecordJsonFieldNodeViewModel> AfterFieldSource => AfterFieldSourceValue;

    /// <summary>Gets native-value semantic changes in engine order.</summary>
    public IReadOnlyList<SemanticChangeDescriptor> SemanticChanges => SemanticChangesValue;

    /// <summary>Gets non-fatal page, context, and comparison warnings.</summary>
    public IReadOnlyList<EngineWarning> Warnings => WarningsValue;

    /// <summary>Gets whether visible warnings exist.</summary>
    public bool HasWarnings => WarningsValue.Count > 0;

    /// <summary>Gets the stable typed error code, or <see langword="null"/>.</summary>
    public EngineErrorCode? ErrorCode => ErrorCodeValue;

    /// <summary>Gets the presentation-safe failure message, or <see langword="null"/>.</summary>
    public string? ErrorMessage => ErrorMessageValue;

    /// <summary>Gets whether a retryable typed failure is visible.</summary>
    public bool HasError => ErrorCodeValue.HasValue;

    /// <summary>Gets whether a workspace is currently active.</summary>
    public bool HasWorkspace => WorkspaceCoordinator.CurrentWorkspace is not null;

    /// <summary>Gets whether another winning-record page is available.</summary>
    public bool HasMoreRecords => ContinuationTokenValue is not null;

    /// <summary>Gets whether record paging is active.</summary>
    public bool IsBusy => IsBusyValue;

    /// <summary>Gets whether context discovery or field comparison is active.</summary>
    public bool IsComparisonBusy => IsComparisonBusyValue;

    /// <summary>Gets the current operation status.</summary>
    public string StatusText => StatusTextValue;

    /// <summary>Gets whether the browser has a visible operation message.</summary>
    public bool HasStatusText => !string.IsNullOrWhiteSpace(StatusTextValue);

    /// <summary>Gets a concise loaded-page count.</summary>
    public string LoadedRecordCountText => HasMoreRecords
        ? $"Loaded records: {RecordsValue.Count:N0}+"
        : $"Loaded records: {RecordsValue.Count:N0}";

    /// <summary>Gets the command that reloads the first record page.</summary>
    public ICommand RefreshCommand { get; }

    /// <summary>Gets the command that requests the next record page.</summary>
    public ICommand LoadMoreCommand { get; }

    /// <summary>Gets the command that repeats the most recent failed operation.</summary>
    public ICommand RetryCommand { get; }

    /// <summary>Starts the browser once its navigation-owned view is attached.</summary>
    /// <returns>A task that completes after the initial record page is published.</returns>
    public Task StartAsync()
    {
        if (IsDisposed || IsStarted)
        {
            return Task.CompletedTask;
        }

        IsStarted = true;
        return RefreshAsync();
    }

    /// <summary>Reloads the first page from the current workspace revision.</summary>
    /// <returns>A task that completes after publication or cancellation.</returns>
    public Task RefreshAsync()
    {
        return BeginWorkspaceGenerationAsync();
    }

    /// <summary>Loads the next engine-issued page when one remains.</summary>
    /// <returns>A task that completes after append publication or cancellation.</returns>
    public Task LoadMoreAsync()
    {
        if (IsDisposed || IsBusyValue || ContinuationTokenValue is null || !RevisionValue.HasValue || WorkspaceCoordinator.CurrentWorkspace is not { } workspace)
        {
            return Task.CompletedTask;
        }

        SetBusy(true);
        SetStatus("Loading the next major-record page...");
        return LoadRecordPageAsync(
            workspace.WorkspaceId,
            WorkspaceGeneration,
            RevisionValue.Value,
            ContinuationTokenValue,
            replace: false,
            WorkspaceCancellation?.Token ?? CancellationToken.None);
    }

    /// <summary>Selects one winning record and loads its exact context choices before comparing defaults.</summary>
    /// <param name="record">The loaded record row, or <see langword="null"/> to clear selection.</param>
    /// <returns>A task that completes after context discovery and the default comparison.</returns>
    public Task SelectRecordAsync(MajorRecordViewModel? record)
    {
        if (record is not null && !RecordsValue.Any(candidate => ReferenceEquals(candidate, record)))
        {
            return Task.CompletedTask;
        }

        SelectedRecordValue = record;
        OnPropertyChanged(nameof(SelectedRecord));
        return BeginSelectionGenerationAsync();
    }

    /// <summary>Selects the prior exact context and refreshes the comparison.</summary>
    /// <param name="context">The current context option, or <see langword="null"/>.</param>
    /// <returns>A task that completes after comparison publication.</returns>
    public Task SelectBeforeContextAsync(MajorRecordContextOption? context)
    {
        if (context is not null && !ContextOptionsValue.Any(candidate => ReferenceEquals(candidate, context)))
        {
            return Task.CompletedTask;
        }

        SelectedBeforeContextValue = context;
        OnPropertyChanged(nameof(SelectedBeforeContext));
        OnPropertyChanged(nameof(BeforeProvenanceText));
        return BeginComparisonGenerationAsync();
    }

    /// <summary>Selects the resulting exact context and refreshes the comparison.</summary>
    /// <param name="context">The current context option, or <see langword="null"/>.</param>
    /// <returns>A task that completes after comparison publication.</returns>
    public Task SelectAfterContextAsync(MajorRecordContextOption? context)
    {
        if (context is not null && !ContextOptionsValue.Any(candidate => ReferenceEquals(candidate, context)))
        {
            return Task.CompletedTask;
        }

        SelectedAfterContextValue = context;
        OnPropertyChanged(nameof(SelectedAfterContext));
        OnPropertyChanged(nameof(AfterProvenanceText));
        return BeginComparisonGenerationAsync();
    }

    /// <summary>Stops subscriptions and cancels outstanding read-only work.</summary>
    public void Dispose()
    {
        if (IsDisposed)
        {
            return;
        }

        IsDisposed = true;
        WorkspaceCoordinator.PropertyChanged -= OnWorkspaceCoordinatorPropertyChanged;
        CancelAndDispose(ref WorkspaceCancellation);
        CancelAndDispose(ref SelectionCancellation);
    }

    /// <summary>Starts a new generation when the coordinator publishes replacement or close.</summary>
    /// <param name="sender">The workspace coordinator.</param>
    /// <param name="eventArgs">The changed coordinator property.</param>
    private void OnWorkspaceCoordinatorPropertyChanged(object? sender, PropertyChangedEventArgs eventArgs)
    {
        if (eventArgs.PropertyName is not null && eventArgs.PropertyName != nameof(IWorkspaceCoordinator.CurrentWorkspace))
        {
            return;
        }

        UiDispatcher.Post(() =>
        {
            OnPropertyChanged(nameof(HasWorkspace));
            if (!IsDisposed && IsStarted)
            {
                _ = BeginWorkspaceGenerationAsync();
            }
        });
    }

    /// <summary>Identifies the browser operation repeated by retry.</summary>
    private enum RetryKind
    {
        /// <summary>No failed operation is pending.</summary>
        None,

        /// <summary>The record page operation failed.</summary>
        Page,

        /// <summary>The selected record's context discovery failed.</summary>
        Contexts,

        /// <summary>The selected context comparison failed.</summary>
        Comparison
    }
}
