using System.Collections.ObjectModel;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Services;
using CreationsForge.Services.Interfaces;
using Mutagen.Bethesda.Plugins;
using Serilog;

namespace CreationsForge.ViewModels;

/// <summary>Coordinates bounded native reference search, paging, exact resolution, and modal cancellation.</summary>
public sealed class NativeReferencePickerViewModel : ViewModelBase, IAsyncDisposable
{
    /// <summary>The fixed number of native matches requested for each visible page.</summary>
    public const int PageSize = 100;

    /// <summary>Synchronizes cancellation-source, generation, task, and disposal state.</summary>
    private readonly object StateLock = new();

    /// <summary>Runs operations against the root-owned active native workspace.</summary>
    private readonly INativeWorkspaceCoordinator WorkspaceCoordinator;

    /// <summary>Publishes all asynchronously produced presentation state on the UI thread.</summary>
    private readonly IUiDispatcher UiDispatcher;

    /// <summary>The immutable picker request and expected workspace revision.</summary>
    private readonly NativeReferencePickerRequest Request;

    /// <summary>Records unexpected picker failures without retaining native records.</summary>
    private readonly ILogger Logger;

    /// <summary>Cancels all work when the dialog lifetime ends.</summary>
    private readonly CancellationTokenSource LifetimeCancellationTokenSource = new();

    /// <summary>Cancels the current search or page request when its query, filter, or workspace becomes stale.</summary>
    private CancellationTokenSource? SearchCancellationTokenSource;

    /// <summary>Cancels the current resolution when its selected row or workspace becomes stale.</summary>
    private CancellationTokenSource? SelectionCancellationTokenSource;

    /// <summary>The current search task retained so modal close can drain it.</summary>
    private Task? ActiveSearchTask;

    /// <summary>The current resolution or explicit-null validation task retained so modal close can drain it.</summary>
    private Task? ActiveSelectionTask;

    /// <summary>Changes whenever a search, query, or filter supersedes earlier results.</summary>
    private long SearchGeneration;

    /// <summary>Changes whenever selection resolution is superseded.</summary>
    private long SelectionGeneration;

    /// <summary>Changes whenever the root coordinator publishes another workspace.</summary>
    private long WorkspaceGeneration;

    /// <summary>The current raw search text entered by the user.</summary>
    private string QueryValue = string.Empty;

    /// <summary>The currently selected visible search match.</summary>
    private ReferenceSearchMatch? SelectedMatchValue;

    /// <summary>The continuation token for the page after the currently visible page.</summary>
    private string? NextContinuationToken;

    /// <summary>The one-based number of the currently visible bounded page.</summary>
    private int PageNumberValue;

    /// <summary>Whether a search or resolution operation is active.</summary>
    private bool IsBusyValue;

    /// <summary>Whether current workspace identity or revision requires the caller to refresh the picker.</summary>
    private bool RequiresRefreshValue;

    /// <summary>The current user-facing status message.</summary>
    private string StatusTextValue = "Enter a FormKey or EditorID fragment to search native records.";

    /// <summary>The current user-facing typed or validation error.</summary>
    private string? ErrorTextValue;

    /// <summary>The latest exact native resolution status for the selected row.</summary>
    private ReferenceResolutionStatus? ResolutionStatusValue;

    /// <summary>Whether lifetime disposal has started.</summary>
    private bool IsDisposed;

    /// <summary>Initializes a fresh per-dialog native reference picker.</summary>
    /// <param name="workspaceCoordinator">The root-owned native workspace coordinator.</param>
    /// <param name="uiDispatcher">The dispatcher for asynchronous presentation publication.</param>
    /// <param name="request">The exact picker request.</param>
    /// <param name="logger">The structured logger for unexpected failures.</param>
    /// <exception cref="ArgumentNullException">Thrown when a required dependency is <see langword="null"/>.</exception>
    public NativeReferencePickerViewModel(
        INativeWorkspaceCoordinator workspaceCoordinator,
        IUiDispatcher uiDispatcher,
        NativeReferencePickerRequest request,
        ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(workspaceCoordinator);
        ArgumentNullException.ThrowIfNull(uiDispatcher);
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(logger);
        WorkspaceCoordinator = workspaceCoordinator;
        UiDispatcher = uiDispatcher;
        Request = request;
        Logger = logger.ForContext<NativeReferencePickerViewModel>();
        WorkspaceCoordinator.PropertyChanged += OnWorkspaceCoordinatorPropertyChanged;
        var current = WorkspaceCoordinator.CurrentWorkspace;
        RequiresRefreshValue = current is null || current.WorkspaceId != Request.WorkspaceId;
        if (RequiresRefreshValue)
        {
            StatusTextValue = "The requested workspace is no longer active. Refresh the editor before selecting a reference.";
        }
    }

    /// <summary>Gets the immutable user-facing purpose for this selection.</summary>
    public string Purpose => Request.Purpose;

    /// <summary>Gets the native record scope applied to search and exact resolution.</summary>
    public RecordScope RecordScope => Request.RecordScope;

    /// <summary>Gets the optional containing-plugin filter applied to search.</summary>
    public ModKey? ContainingModKey => Request.ContainingModKey;

    /// <summary>Gets the FormList currently being edited, when supplied for context.</summary>
    public FormKey? CurrentFormKey => Request.CurrentFormKey;

    /// <summary>Gets whether an explicit null selection is available.</summary>
    public bool AllowNull => Request.AllowNull;

    /// <summary>Gets the visible bounded native result page.</summary>
    public ObservableCollection<ReferenceSearchMatch> Matches { get; } = new();

    /// <summary>Gets or sets the current search query and invalidates any earlier page or selection.</summary>
    public string Query
    {
        get => QueryValue;
        set
        {
            if (!SetProperty(ref QueryValue, value ?? string.Empty))
            {
                return;
            }

            CancelSearchAndSelection(clearVisiblePage: true);
            OnPropertyChanged(nameof(CanSearch));
        }
    }

    /// <summary>Gets or sets the selected match and cancels resolution for any earlier row.</summary>
    public ReferenceSearchMatch? SelectedMatch
    {
        get => SelectedMatchValue;
        set
        {
            if (!SetProperty(ref SelectedMatchValue, value))
            {
                return;
            }

            CancelSelection();
            ResolutionStatus = null;
            ErrorText = null;
            OnPropertyChanged(nameof(CanConfirmSelection));
        }
    }

    /// <summary>Gets the one-based visible page number, or zero before a successful search.</summary>
    public int PageNumber
    {
        get => PageNumberValue;
        private set => SetProperty(ref PageNumberValue, value);
    }

    /// <summary>Gets whether the current visible page has an explicit next page.</summary>
    public bool HasNextPage => NextContinuationToken is not null;

    /// <summary>Gets whether any search or resolution operation is active.</summary>
    public bool IsBusy
    {
        get => IsBusyValue;
        private set
        {
            if (SetProperty(ref IsBusyValue, value))
            {
                OnPropertyChanged(nameof(CanSearch));
                OnPropertyChanged(nameof(CanLoadNextPage));
                OnPropertyChanged(nameof(CanConfirmSelection));
                OnPropertyChanged(nameof(CanSelectNull));
            }
        }
    }

    /// <summary>Gets whether workspace replacement or revision drift requires the caller to refresh.</summary>
    public bool RequiresRefresh
    {
        get => RequiresRefreshValue;
        private set
        {
            if (SetProperty(ref RequiresRefreshValue, value))
            {
                OnPropertyChanged(nameof(CanSearch));
                OnPropertyChanged(nameof(CanLoadNextPage));
                OnPropertyChanged(nameof(CanConfirmSelection));
                OnPropertyChanged(nameof(CanSelectNull));
            }
        }
    }

    /// <summary>Gets the current status or paging message.</summary>
    public string StatusText
    {
        get => StatusTextValue;
        private set => SetProperty(ref StatusTextValue, value);
    }

    /// <summary>Gets the current typed engine or validation error, when present.</summary>
    public string? ErrorText
    {
        get => ErrorTextValue;
        private set
        {
            if (SetProperty(ref ErrorTextValue, value))
            {
                OnPropertyChanged(nameof(HasError));
            }
        }
    }

    /// <summary>Gets whether an error is currently available for display.</summary>
    public bool HasError => !string.IsNullOrWhiteSpace(ErrorText);

    /// <summary>Gets the most recent native resolution status for the selected row.</summary>
    public ReferenceResolutionStatus? ResolutionStatus
    {
        get => ResolutionStatusValue;
        private set => SetProperty(ref ResolutionStatusValue, value);
    }

    /// <summary>Gets whether the current query can start a fresh bounded search.</summary>
    public bool CanSearch => !IsBusy
        && !RequiresRefresh
        && !string.IsNullOrWhiteSpace(Query)
        && Query.Trim().Length <= ReferenceSearchRequest.MaximumQueryLength;

    /// <summary>Gets whether the visible page exposes a next-page continuation.</summary>
    public bool CanLoadNextPage => !IsBusy && !RequiresRefresh && HasNextPage;

    /// <summary>Gets whether the current native row can be resolved for selection.</summary>
    public bool CanConfirmSelection => !IsBusy && !RequiresRefresh && SelectedMatch is not null;

    /// <summary>Gets whether explicit null can be selected now.</summary>
    public bool CanSelectNull => !IsBusy && !RequiresRefresh && AllowNull;

    /// <summary>Searches the first bounded page for the current normalized query.</summary>
    /// <returns>A task that completes after current results are published.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when a search is started after picker disposal.</exception>
    public Task SearchAsync()
    {
        if (IsBusy || RequiresRefresh)
        {
            return Task.CompletedTask;
        }

        return StartSearchAsync(continuationToken: null, pageNumber: 1);
    }

    /// <summary>Replaces the visible page with the next bounded engine page.</summary>
    /// <returns>A task that completes after current results are published.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when paging is started after picker disposal.</exception>
    public Task LoadNextPageAsync()
    {
        if (!CanLoadNextPage)
        {
            return Task.CompletedTask;
        }

        return StartSearchAsync(NextContinuationToken, checked(PageNumber + 1));
    }

    /// <summary>Resolves and returns the exact selected native record context.</summary>
    /// <returns>The exact selection when resolved and current, otherwise <see langword="null"/>.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when resolution is started after picker disposal.</exception>
    public Task<NativeReferencePickerSelection?> ConfirmSelectionAsync()
    {
        var match = SelectedMatch;
        if (!CanConfirmSelection || match is null)
        {
            return Task.FromResult<NativeReferencePickerSelection?>(null);
        }

        return StartSelectionAsync(match);
    }

    /// <summary>Validates workspace freshness and returns an explicit null selection.</summary>
    /// <returns>The explicit null selection when permitted and current, otherwise <see langword="null"/>.</returns>
    /// <exception cref="ObjectDisposedException">Thrown when validation is started after picker disposal.</exception>
    public Task<NativeReferencePickerSelection?> SelectNullAsync()
    {
        if (!CanSelectNull)
        {
            return Task.FromResult<NativeReferencePickerSelection?>(null);
        }

        return StartNullSelectionAsync();
    }

    /// <summary>Cancels all current work and waits until native borrows and UI publications have drained.</summary>
    /// <returns>A task that completes after active picker work ends.</returns>
    public async Task CancelAndDrainAsync()
    {
        Task? search;
        Task? selection;
        lock (StateLock)
        {
            SearchGeneration++;
            SelectionGeneration++;
            SearchCancellationTokenSource?.Cancel();
            SelectionCancellationTokenSource?.Cancel();
            search = ActiveSearchTask;
            selection = ActiveSelectionTask;
        }

        await DrainCanceledAsync(search).ConfigureAwait(false);
        await DrainCanceledAsync(selection).ConfigureAwait(false);
    }

    /// <summary>Cancels active work, removes the workspace subscription, and drains borrowed operations.</summary>
    /// <returns>A task that completes after picker cleanup.</returns>
    public async ValueTask DisposeAsync()
    {
        lock (StateLock)
        {
            if (IsDisposed)
            {
                return;
            }

            IsDisposed = true;
            LifetimeCancellationTokenSource.Cancel();
            SearchCancellationTokenSource?.Cancel();
            SelectionCancellationTokenSource?.Cancel();
        }

        WorkspaceCoordinator.PropertyChanged -= OnWorkspaceCoordinatorPropertyChanged;
        await CancelAndDrainAsync().ConfigureAwait(false);
        lock (StateLock)
        {
            SearchCancellationTokenSource?.Dispose();
            SearchCancellationTokenSource = null;
            SelectionCancellationTokenSource?.Dispose();
            SelectionCancellationTokenSource = null;
        }

        LifetimeCancellationTokenSource.Dispose();
    }

    /// <summary>Starts one superseding bounded search operation.</summary>
    /// <param name="continuationToken">The engine continuation for the requested page.</param>
    /// <param name="pageNumber">The one-based requested page number.</param>
    /// <returns>A task that completes after current publication.</returns>
    private Task StartSearchAsync(string? continuationToken, int pageNumber)
    {
        var normalizedQuery = Query.Trim();
        if (normalizedQuery.Length == 0 || normalizedQuery.Length > ReferenceSearchRequest.MaximumQueryLength || RequiresRefresh)
        {
            ErrorText = normalizedQuery.Length > ReferenceSearchRequest.MaximumQueryLength
                ? $"Search text cannot exceed {ReferenceSearchRequest.MaximumQueryLength} characters."
                : "Enter search text before searching native records.";
            return Task.CompletedTask;
        }

        CancellationTokenSource cancellationSource;
        long searchGeneration;
        long workspaceGeneration;
        lock (StateLock)
        {
            ThrowIfDisposed();
            SearchCancellationTokenSource?.Cancel();
            SearchCancellationTokenSource?.Dispose();
            cancellationSource = CancellationTokenSource.CreateLinkedTokenSource(LifetimeCancellationTokenSource.Token);
            SearchCancellationTokenSource = cancellationSource;
            searchGeneration = ++SearchGeneration;
            workspaceGeneration = WorkspaceGeneration;
        }

        SelectedMatch = null;
        ErrorText = null;
        ResolutionStatus = null;
        IsBusy = true;
        StatusText = pageNumber == 1 ? "Searching native records..." : $"Loading native result page {pageNumber}...";
        var task = SearchCoreAsync(
            normalizedQuery,
            continuationToken,
            pageNumber,
            searchGeneration,
            workspaceGeneration,
            cancellationSource.Token);
        lock (StateLock)
        {
            ActiveSearchTask = task;
        }

        return task;
    }

    /// <summary>Executes and conditionally publishes one bounded engine search page.</summary>
    /// <param name="query">The normalized query.</param>
    /// <param name="continuationToken">The optional next-page continuation.</param>
    /// <param name="pageNumber">The requested one-based page number.</param>
    /// <param name="searchGeneration">The operation's search generation.</param>
    /// <param name="workspaceGeneration">The operation's workspace generation.</param>
    /// <param name="cancellationToken">The superseding search token.</param>
    /// <returns>A task that completes after current publication or stale-result suppression.</returns>
    private async Task SearchCoreAsync(
        string query,
        string? continuationToken,
        int pageNumber,
        long searchGeneration,
        long workspaceGeneration,
        CancellationToken cancellationToken)
    {
        try
        {
            var searchRequest = new ReferenceSearchRequest(
                query,
                PageSize,
                continuationToken,
                Request.RecordScope,
                Request.ContainingModKey);
            var result = await WorkspaceCoordinator.ExecuteAsync(
                (workspace, token) => SearchCurrentWorkspaceAsync(workspace, searchRequest, token),
                cancellationToken).ConfigureAwait(false);
            await UiDispatcher.InvokeAsync(() =>
            {
                if (!IsCurrentSearch(searchGeneration, workspaceGeneration))
                {
                    return;
                }

                if (!TryValidateCurrentResult(result, out var error))
                {
                    PublishRefreshOrError(result.Error?.Code, error);
                    return;
                }
                var page = result.Value!;
                if (page.Matches.Count > PageSize)
                {
                    ErrorText = $"The native engine returned more than the bounded {PageSize}-record page.";
                    StatusText = "The native reference search returned an invalid page.";
                    return;
                }
                Matches.Clear();
                foreach (var match in page.Matches)
                {
                    Matches.Add(match);
                }

                PageNumber = pageNumber;
                NextContinuationToken = page.ContinuationToken;
                OnPropertyChanged(nameof(HasNextPage));
                OnPropertyChanged(nameof(CanLoadNextPage));
                StatusText = Matches.Count == 0
                    ? "No native records matched this search."
                    : $"Showing {Matches.Count} native record{(Matches.Count == 1 ? string.Empty : "s")} on page {PageNumber}.";
                ErrorText = null;
            }).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        { }
        catch (Exception exception)
        {
            Logger.Error(exception, "Unexpected native reference search failure for workspace {WorkspaceId}.", Request.WorkspaceId);
            await PublishUnexpectedAsync(searchGeneration, workspaceGeneration, "Unable to search native records.").ConfigureAwait(false);
        }
        finally
        {
            await CompleteSearchAsync(searchGeneration, workspaceGeneration).ConfigureAwait(false);
        }
    }

    /// <summary>Runs search only when the borrowed workspace still matches the picker request.</summary>
    /// <param name="workspace">The coordinator-borrowed workspace.</param>
    /// <param name="searchRequest">The exact bounded search request.</param>
    /// <param name="cancellationToken">The operation cancellation token.</param>
    /// <returns>The engine search result or an exact revision conflict.</returns>
    private ValueTask<EngineResult<ReferenceSearchPage>> SearchCurrentWorkspaceAsync(
        IFormListWorkspace workspace,
        ReferenceSearchRequest searchRequest,
        CancellationToken cancellationToken)
    {
        if (workspace.WorkspaceId != Request.WorkspaceId || workspace.Revision != Request.ExpectedRevision)
        {
            return ValueTask.FromResult(RevisionConflict<ReferenceSearchPage>(workspace));
        }

        return workspace.SearchReferencesAsync(searchRequest, cancellationToken);
    }

    /// <summary>Starts exact resolution for the currently selected row.</summary>
    /// <param name="match">The exact visible native match.</param>
    /// <returns>A task producing a current resolved selection or <see langword="null"/>.</returns>
    private Task<NativeReferencePickerSelection?> StartSelectionAsync(ReferenceSearchMatch match)
    {
        CancellationTokenSource cancellationSource;
        long selectionGeneration;
        long workspaceGeneration;
        lock (StateLock)
        {
            ThrowIfDisposed();
            SelectionCancellationTokenSource?.Cancel();
            SelectionCancellationTokenSource?.Dispose();
            cancellationSource = CancellationTokenSource.CreateLinkedTokenSource(LifetimeCancellationTokenSource.Token);
            SelectionCancellationTokenSource = cancellationSource;
            selectionGeneration = ++SelectionGeneration;
            workspaceGeneration = WorkspaceGeneration;
        }

        IsBusy = true;
        ErrorText = null;
        ResolutionStatus = null;
        StatusText = "Resolving the selected native record...";
        var task = ResolveSelectionCoreAsync(
            match,
            selectionGeneration,
            workspaceGeneration,
            cancellationSource.Token);
        lock (StateLock)
        {
            ActiveSelectionTask = task;
        }

        return task;
    }

    /// <summary>Resolves one selected row and preserves explicit native resolution status.</summary>
    /// <param name="match">The selected exact search match.</param>
    /// <param name="selectionGeneration">The operation's selection generation.</param>
    /// <param name="workspaceGeneration">The operation's workspace generation.</param>
    /// <param name="cancellationToken">The superseding selection token.</param>
    /// <returns>The current resolved selection, or <see langword="null"/> on cancellation, stale state, or resolution failure.</returns>
    private async Task<NativeReferencePickerSelection?> ResolveSelectionCoreAsync(
        ReferenceSearchMatch match,
        long selectionGeneration,
        long workspaceGeneration,
        CancellationToken cancellationToken)
    {
        NativeReferencePickerSelection? selection = null;
        try
        {
            var containingModKey = Request.RecordScope == RecordScope.WinningOverrides
                ? null
                : match.ContainingModKey ?? Request.ContainingModKey;
            var referenceRequest = new ReferenceRequest(match.FormKey, Request.RecordScope, containingModKey);
            var result = await WorkspaceCoordinator.ExecuteAsync(
                (workspace, token) => ResolveCurrentWorkspaceAsync(workspace, referenceRequest, token),
                cancellationToken).ConfigureAwait(false);
            await UiDispatcher.InvokeAsync(() =>
            {
                if (!IsCurrentSelection(selectionGeneration, workspaceGeneration, match))
                {
                    return;
                }

                if (!TryValidateCurrentResult(result, out var error))
                {
                    PublishRefreshOrError(result.Error?.Code, error);
                    return;
                }

                var resolution = result.Value!;
                ResolutionStatus = resolution.Status;
                if (resolution.Status != ReferenceResolutionStatus.Resolved)
                {
                    ErrorText = $"The selected native record resolved as {resolution.Status}. Refresh the search or choose another context.";
                    StatusText = "The selected reference cannot be returned.";
                    return;
                }

                if (!ResolutionMatchesSearch(match, resolution))
                {
                    ErrorText = "The selected native record's identity or containing-plugin provenance changed. Refresh the search.";
                    StatusText = "The selected reference became stale.";
                    RequiresRefresh = true;
                    return;
                }

                selection = new NativeReferencePickerSelection(
                    Request.WorkspaceId,
                    result.ResultRevision!.Value,
                    isNull: false,
                    match);
                ErrorText = null;
                StatusText = "Native reference resolved.";
            }).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        { }
        catch (Exception exception)
        {
            Logger.Error(exception, "Unexpected native reference resolution failure for workspace {WorkspaceId}.", Request.WorkspaceId);
            await PublishUnexpectedSelectionAsync(
                selectionGeneration,
                workspaceGeneration,
                "Unable to resolve the selected native record.").ConfigureAwait(false);
        }
        finally
        {
            await CompleteSelectionAsync(selectionGeneration, workspaceGeneration).ConfigureAwait(false);
        }

        return selection is not null && IsCurrentSelection(selectionGeneration, workspaceGeneration, match)
            ? selection
            : null;
    }

    /// <summary>Runs exact resolution only against the requested workspace revision.</summary>
    /// <param name="workspace">The coordinator-borrowed workspace.</param>
    /// <param name="referenceRequest">The exact selected reference request.</param>
    /// <param name="cancellationToken">The operation cancellation token.</param>
    /// <returns>The native resolution or an exact revision conflict.</returns>
    private ValueTask<EngineResult<ReferenceResolution>> ResolveCurrentWorkspaceAsync(
        IFormListWorkspace workspace,
        ReferenceRequest referenceRequest,
        CancellationToken cancellationToken)
    {
        if (workspace.WorkspaceId != Request.WorkspaceId || workspace.Revision != Request.ExpectedRevision)
        {
            return ValueTask.FromResult(RevisionConflict<ReferenceResolution>(workspace));
        }

        return workspace.ResolveReferenceAsync(referenceRequest, cancellationToken);
    }

    /// <summary>Starts freshness validation for an explicit null selection.</summary>
    /// <returns>A task producing the explicit null selection or <see langword="null"/> when stale.</returns>
    private Task<NativeReferencePickerSelection?> StartNullSelectionAsync()
    {
        SelectedMatch = null;
        CancellationTokenSource cancellationSource;
        long selectionGeneration;
        long workspaceGeneration;
        lock (StateLock)
        {
            ThrowIfDisposed();
            SelectionCancellationTokenSource?.Cancel();
            SelectionCancellationTokenSource?.Dispose();
            cancellationSource = CancellationTokenSource.CreateLinkedTokenSource(LifetimeCancellationTokenSource.Token);
            SelectionCancellationTokenSource = cancellationSource;
            selectionGeneration = ++SelectionGeneration;
            workspaceGeneration = WorkspaceGeneration;
        }

        IsBusy = true;
        ErrorText = null;
        StatusText = "Validating the active workspace...";
        var task = ValidateNullSelectionCoreAsync(
            selectionGeneration,
            workspaceGeneration,
            cancellationSource.Token);
        lock (StateLock)
        {
            ActiveSelectionTask = task;
        }

        return task;
    }

    /// <summary>Validates exact workspace identity and revision without retaining a native record.</summary>
    /// <param name="selectionGeneration">The operation's selection generation.</param>
    /// <param name="workspaceGeneration">The operation's workspace generation.</param>
    /// <param name="cancellationToken">The superseding selection token.</param>
    /// <returns>The explicit null selection when current, otherwise <see langword="null"/>.</returns>
    private async Task<NativeReferencePickerSelection?> ValidateNullSelectionCoreAsync(
        long selectionGeneration,
        long workspaceGeneration,
        CancellationToken cancellationToken)
    {
        NativeReferencePickerSelection? selection = null;
        try
        {
            var result = await WorkspaceCoordinator.ExecuteAsync(
                (workspace, _) => ValueTask.FromResult(
                    workspace.WorkspaceId == Request.WorkspaceId && workspace.Revision == Request.ExpectedRevision
                        ? EngineResult<WorkspaceRevision>.Success(
                            workspace.Revision,
                            workspaceId: workspace.WorkspaceId,
                            resultRevision: workspace.Revision)
                        : RevisionConflict<WorkspaceRevision>(workspace)),
                cancellationToken).ConfigureAwait(false);
            await UiDispatcher.InvokeAsync(() =>
            {
                if (!IsCurrentSelection(selectionGeneration, workspaceGeneration, match: null))
                {
                    return;
                }

                if (!TryValidateCurrentResult(result, out var error))
                {
                    PublishRefreshOrError(result.Error?.Code, error);
                    return;
                }

                selection = new NativeReferencePickerSelection(
                    Request.WorkspaceId,
                    result.ResultRevision!.Value,
                    isNull: true,
                    match: null);
                StatusText = "Null reference selected.";
            }).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        { }
        catch (Exception exception)
        {
            Logger.Error(exception, "Unexpected explicit-null validation failure for workspace {WorkspaceId}.", Request.WorkspaceId);
            await PublishUnexpectedSelectionAsync(
                selectionGeneration,
                workspaceGeneration,
                "Unable to validate the explicit null reference.").ConfigureAwait(false);
        }
        finally
        {
            await CompleteSelectionAsync(selectionGeneration, workspaceGeneration).ConfigureAwait(false);
        }

        return selection is not null && IsCurrentSelection(selectionGeneration, workspaceGeneration, match: null)
            ? selection
            : null;
    }

    /// <summary>Validates result identity and unchanged revision before presentation uses engine output.</summary>
    /// <typeparam name="T">The engine result value type.</typeparam>
    /// <param name="result">The result to validate.</param>
    /// <param name="error">The presentation-safe failure text.</param>
    /// <returns><see langword="true"/> when the result is successful and exact.</returns>
    private bool TryValidateCurrentResult<T>(EngineResult<T> result, out string error)
    {
        if (!result.Succeeded || result.Value is null)
        {
            error = result.Error?.Message ?? "The native workspace operation failed without an error description.";
            return false;
        }

        if (result.WorkspaceId != Request.WorkspaceId || result.ResultRevision != Request.ExpectedRevision)
        {
            error = "The native workspace or revision changed. Refresh the editor before selecting a reference.";
            return false;
        }

        error = string.Empty;
        return true;
    }

    /// <summary>Checks that resolved identity and containing-plugin provenance still match the selected search row.</summary>
    /// <param name="match">The selected search match.</param>
    /// <param name="resolution">The fresh native resolution.</param>
    /// <returns><see langword="true"/> when every selection-relevant field matches.</returns>
    private static bool ResolutionMatchesSearch(
        ReferenceSearchMatch match,
        ReferenceResolution resolution)
    {
        var pathComparer = OperatingSystem.IsWindows() ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;
        return resolution.FormKey == match.FormKey
            && string.Equals(resolution.RecordType, match.RecordType, StringComparison.Ordinal)
            && string.Equals(resolution.Record?.EditorID, match.EditorId, StringComparison.Ordinal)
            && resolution.ContainingModKey == match.ContainingModKey
            && pathComparer.Equals(resolution.SourcePath, match.SourcePath)
            && resolution.LoadOrderIndex == match.LoadOrderIndex
            && resolution.Role == match.Role
            && !match.IsDeleted;
    }

    /// <summary>Creates an exact revision-conflict result for a mismatched borrowed workspace.</summary>
    /// <typeparam name="T">The unsuccessful result value type.</typeparam>
    /// <param name="workspace">The borrowed workspace whose identity or revision differed.</param>
    /// <returns>The typed revision conflict.</returns>
    private EngineResult<T> RevisionConflict<T>(IFormListWorkspace workspace)
    {
        return EngineResult<T>.Failure(
            new EngineError(
                EngineErrorCode.RevisionConflict,
                "The native workspace or revision changed. Refresh the editor before selecting a reference."),
            workspaceId: workspace.WorkspaceId,
            baseRevision: Request.ExpectedRevision,
            resultRevision: workspace.Revision);
    }

    /// <summary>Publishes a typed operation failure and marks revision conflicts as requiring refresh.</summary>
    /// <param name="code">The engine failure category, when supplied.</param>
    /// <param name="message">The presentation-safe failure message.</param>
    private void PublishRefreshOrError(EngineErrorCode? code, string message)
    {
        ErrorText = message;
        StatusText = code == EngineErrorCode.RevisionConflict
            ? "Workspace changed. Refresh before selecting a reference."
            : "The native reference operation did not complete.";
        if (code == EngineErrorCode.RevisionConflict
            || message.Contains("workspace or revision changed", StringComparison.OrdinalIgnoreCase))
        {
            RequiresRefresh = true;
            CancelSearchAndSelection(clearVisiblePage: true);
        }
    }

    /// <summary>Publishes an unexpected search error only if its generations remain current.</summary>
    /// <param name="searchGeneration">The failed search generation.</param>
    /// <param name="workspaceGeneration">The failed workspace generation.</param>
    /// <param name="message">The presentation-safe message.</param>
    /// <returns>A task that completes after conditional UI publication.</returns>
    private Task PublishUnexpectedAsync(long searchGeneration, long workspaceGeneration, string message)
    {
        return UiDispatcher.InvokeAsync(() =>
        {
            if (IsCurrentSearch(searchGeneration, workspaceGeneration))
            {
                ErrorText = message;
                StatusText = "The native reference search failed.";
            }
        });
    }

    /// <summary>Publishes an unexpected selection error only if its generations remain current.</summary>
    /// <param name="selectionGeneration">The failed selection generation.</param>
    /// <param name="workspaceGeneration">The failed workspace generation.</param>
    /// <param name="message">The presentation-safe message.</param>
    /// <returns>A task that completes after conditional UI publication.</returns>
    private Task PublishUnexpectedSelectionAsync(long selectionGeneration, long workspaceGeneration, string message)
    {
        return UiDispatcher.InvokeAsync(() =>
        {
            if (IsCurrentSelection(selectionGeneration, workspaceGeneration, SelectedMatch))
            {
                ErrorText = message;
                StatusText = "The selected reference could not be resolved.";
            }
        });
    }

    /// <summary>Publishes search completion without reviving superseded state.</summary>
    /// <param name="searchGeneration">The completing search generation.</param>
    /// <param name="workspaceGeneration">The completing workspace generation.</param>
    /// <returns>A task that completes after conditional UI publication.</returns>
    private Task CompleteSearchAsync(long searchGeneration, long workspaceGeneration)
    {
        lock (StateLock)
        {
            if (searchGeneration == SearchGeneration)
            {
                ActiveSearchTask = null;
            }
        }

        return UiDispatcher.InvokeAsync(() =>
        {
            if (!IsDisposed)
            {
                IsBusy = false;
            }
        });
    }

    /// <summary>Publishes selection completion without reviving superseded state.</summary>
    /// <param name="selectionGeneration">The completing selection generation.</param>
    /// <param name="workspaceGeneration">The completing workspace generation.</param>
    /// <returns>A task that completes after conditional UI publication.</returns>
    private Task CompleteSelectionAsync(long selectionGeneration, long workspaceGeneration)
    {
        lock (StateLock)
        {
            if (selectionGeneration == SelectionGeneration)
            {
                ActiveSelectionTask = null;
            }
        }

        return UiDispatcher.InvokeAsync(() =>
        {
            if (!IsDisposed)
            {
                IsBusy = false;
            }
        });
    }

    /// <summary>Checks whether a search operation may still publish.</summary>
    /// <param name="searchGeneration">The captured search generation.</param>
    /// <param name="workspaceGeneration">The captured workspace generation.</param>
    /// <returns><see langword="true"/> when the operation is current and alive.</returns>
    private bool IsCurrentSearch(long searchGeneration, long workspaceGeneration)
    {
        lock (StateLock)
        {
            return !IsDisposed
                && searchGeneration == SearchGeneration
                && workspaceGeneration == WorkspaceGeneration;
        }
    }

    /// <summary>Checks whether a selection operation may still publish.</summary>
    /// <param name="selectionGeneration">The captured selection generation.</param>
    /// <param name="workspaceGeneration">The captured workspace generation.</param>
    /// <param name="match">The selection identity captured by the operation.</param>
    /// <returns><see langword="true"/> when the operation is current and alive.</returns>
    private bool IsCurrentSelection(
        long selectionGeneration,
        long workspaceGeneration,
        ReferenceSearchMatch? match)
    {
        lock (StateLock)
        {
            return !IsDisposed
                && selectionGeneration == SelectionGeneration
                && workspaceGeneration == WorkspaceGeneration
                && (match is null || ReferenceEquals(match, SelectedMatch));
        }
    }

    /// <summary>Cancels the active search and selection after query, filter, or workspace change.</summary>
    /// <param name="clearVisiblePage">Whether to discard the current bounded page.</param>
    private void CancelSearchAndSelection(bool clearVisiblePage)
    {
        lock (StateLock)
        {
            SearchGeneration++;
            SelectionGeneration++;
            SearchCancellationTokenSource?.Cancel();
            SelectionCancellationTokenSource?.Cancel();
        }

        NextContinuationToken = null;
        PageNumber = 0;
        SelectedMatchValue = null;
        OnPropertyChanged(nameof(SelectedMatch));
        OnPropertyChanged(nameof(HasNextPage));
        OnPropertyChanged(nameof(CanLoadNextPage));
        OnPropertyChanged(nameof(CanConfirmSelection));
        if (clearVisiblePage)
        {
            Matches.Clear();
        }
    }

    /// <summary>Cancels resolution for the previously selected row.</summary>
    private void CancelSelection()
    {
        lock (StateLock)
        {
            SelectionGeneration++;
            SelectionCancellationTokenSource?.Cancel();
        }
    }

    /// <summary>Invalidates all picker state when the root coordinator publishes another workspace.</summary>
    /// <param name="sender">The coordinator event source.</param>
    /// <param name="eventArgs">The changed coordinator property.</param>
    private void OnWorkspaceCoordinatorPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs eventArgs)
    {
        if (!string.Equals(eventArgs.PropertyName, nameof(INativeWorkspaceCoordinator.CurrentWorkspace), StringComparison.Ordinal))
        {
            return;
        }

        lock (StateLock)
        {
            WorkspaceGeneration++;
            SearchCancellationTokenSource?.Cancel();
            SelectionCancellationTokenSource?.Cancel();
        }

        UiDispatcher.Post(() =>
        {
            if (IsDisposed)
            {
                return;
            }

            RequiresRefresh = true;
            ErrorText = "The active native workspace changed. Refresh the editor before selecting a reference.";
            StatusText = "Workspace changed. This picker can no longer return a selection.";
            CancelSearchAndSelection(clearVisiblePage: true);
        });
    }

    /// <summary>Throws when work is started after picker disposal.</summary>
    /// <exception cref="ObjectDisposedException">Thrown after disposal begins.</exception>
    private void ThrowIfDisposed()
    {
        if (IsDisposed)
        {
            throw new ObjectDisposedException(nameof(NativeReferencePickerViewModel));
        }
    }

    /// <summary>Drains one canceled operation while suppressing its expected cancellation.</summary>
    /// <param name="task">The task to drain, or <see langword="null"/>.</param>
    /// <returns>A task that completes after the supplied operation ends.</returns>
    private static async Task DrainCanceledAsync(Task? task)
    {
        if (task is null)
        {
            return;
        }

        try
        {
            await task.ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        { }
    }
}
