using System.ComponentModel;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeWire;
using CreationsForge.NativeEditing;
using CreationsForge.NativeEditing.Drafts;
using CreationsForge.Services;
using CreationsForge.Services.Interfaces;

namespace CreationsForge.ViewModels;

public sealed partial class NativeFormListEditorViewModel
{
    /// <summary>Attempts to acquire shared editor admission and publishes the running operation state.</summary>
    /// <param name="state">The running state published when the gate is acquired.</param>
    /// <returns>The exact editor-operation lease, or <see langword="null"/> when admission is unavailable.</returns>
    private NativeWorkspaceEditorOperationLease? TryEnterOperation(NativeFormListEditorOperationState state)
    {
        if (IsDisposed)
        {
            return null;
        }

        var lease = OperationArbiter.TryBeginEditorOperation();
        if (lease is null)
        {
            return null;
        }

        if (IsDisposed)
        {
            lease.Dispose();
            return null;
        }

        SetOperationState(state);
        ClearError();
        return lease;
    }

    /// <summary>Publishes the terminal editor state before the caller releases its shared operation lease.</summary>
    /// <param name="generation">The workspace generation that owned the operation.</param>
    /// <returns>A task that completes after the terminal state is published or found stale.</returns>
    private Task ExitOperationAsync(long generation)
    {
        return UiDispatcher.InvokeAsync(() =>
        {
            if (IsDisposed || generation != Volatile.Read(ref WorkspaceGeneration))
            {
                return;
            }

            SetOperationState(PendingOperationValue is null
                ? NativeFormListEditorOperationState.Idle
                : NativeFormListEditorOperationState.PendingOutcome);
        });
    }

    /// <summary>Creates a caller and workspace-generation linked cancellation source.</summary>
    /// <param name="cancellationToken">The optional caller cancellation token.</param>
    /// <param name="operationCancellationToken">The token canceled by a cancel-and-wait workspace transition.</param>
    /// <returns>A linked token source owned by the caller.</returns>
    private CancellationTokenSource CreateGenerationLinkedCancellation(
        CancellationToken cancellationToken,
        CancellationToken operationCancellationToken)
    {
        lock (GenerationCancellationLock)
        {
            return CancellationTokenSource.CreateLinkedTokenSource(
                GenerationCancellation.Token,
                cancellationToken,
                operationCancellationToken);
        }
    }

    /// <summary>Cancels the current generation and installs a fresh generation token.</summary>
    private void ReplaceGeneration()
    {
        Interlocked.Increment(ref WorkspaceGeneration);
        lock (GenerationCancellationLock)
        {
            GenerationCancellation.Cancel();
            GenerationCancellation.Dispose();
            GenerationCancellation = new CancellationTokenSource();
        }
    }

    /// <summary>Cancels and disposes the final workspace-generation token.</summary>
    private void CancelGeneration()
    {
        Interlocked.Increment(ref WorkspaceGeneration);
        lock (GenerationCancellationLock)
        {
            GenerationCancellation.Cancel();
            GenerationCancellation.Dispose();
        }
    }

    /// <summary>Handles transactional workspace replacement by canceling and clearing every editor-owned generation value.</summary>
    /// <param name="sender">The application workspace coordinator.</param>
    /// <param name="eventArgs">The changed coordinator property.</param>
    private void OnWorkspaceCoordinatorPropertyChanged(object? sender, PropertyChangedEventArgs eventArgs)
    {
        if (eventArgs.PropertyName != nameof(INativeWorkspaceCoordinator.CurrentWorkspace) || IsDisposed)
        {
            return;
        }

        ReplaceGeneration();
        UiDispatcher.Post(ResetForWorkspaceReplacement);
    }

    /// <summary>Refreshes begin-command availability after the browser publishes one atomic exact selection.</summary>
    /// <param name="sender">The browser editor host.</param>
    /// <param name="eventArgs">The changed host property.</param>
    private void OnHostPropertyChanged(object? sender, PropertyChangedEventArgs eventArgs)
    {
        if (eventArgs.PropertyName == nameof(INativeFormListEditorHost.Selection))
        {
            OnPropertyChanged(nameof(CanBeginOverride));
            OnPropertyChanged(nameof(CanBeginExistingOutput));
            RaiseCommandStates();
        }
    }

    /// <summary>Disables or restores every editor entrypoint when workspace-transition admission changes.</summary>
    /// <param name="sender">The shared presentation operation arbiter.</param>
    /// <param name="eventArgs">The changed arbiter property.</param>
    private void OnOperationArbiterPropertyChanged(object? sender, PropertyChangedEventArgs eventArgs)
    {
        if (eventArgs.PropertyName != nameof(INativeWorkspacePresentationOperationArbiter.IsWorkspaceTransitionPendingOrReserved))
        {
            return;
        }

        UiDispatcher.Post(() =>
        {
            if (IsDisposed)
            {
                return;
            }

            OnPropertyChanged(nameof(CanMutateDraft));
            OnPropertyChanged(nameof(CanApply));
            OnPropertyChanged(nameof(CanDiscardFormChanges));
            OnPropertyChanged(nameof(CanBeginNew));
            OnPropertyChanged(nameof(CanBeginOverride));
            OnPropertyChanged(nameof(CanBeginExistingOutput));
            RaiseCommandStates();
        });
    }

    /// <summary>Clears all session, draft, retry, and dirty state after a workspace replacement or close.</summary>
    private void ResetForWorkspaceReplacement()
    {
        if (IsDisposed)
        {
            return;
        }

        PendingOperationValue = null;
        SessionValue = null;
        CatalogContextValue = null;
        SeedValue = null;
        AvailableCommandsValue = Array.Empty<NativeFormListCommandPresentation>();
        SelectedCommandValue = null;
        DetachDraft();
        ValidationIssuesValue = Array.Empty<NativeWireDraftIssue>();
        IsStagedChangesKnownValue = false;
        HasStagedChangesValue = false;
        WarningsValue = Array.Empty<EngineWarning>();
        ClearError();
        SetOperationState(NativeFormListEditorOperationState.Idle);
        SetStatus(WorkspaceCoordinator.CurrentWorkspace is null
            ? "No native workspace is open."
            : "Workspace changed. Begin a new edit or select an exact FormList context.");
        RaiseSessionProperties();
    }

    /// <summary>Clears stale editor state only after the browser publishes an exact post-persistence workspace snapshot.</summary>
    /// <param name="expectedWorkspaceId">The workspace identity verified by the browser snapshot.</param>
    /// <param name="expectedRevision">The revision verified by the browser snapshot.</param>
    /// <exception cref="InvalidOperationException">Thrown when no transition owns admission, editor work remains active, or the workspace identity differs.</exception>
    internal void CompleteWorkspacePersistenceRefresh(Guid expectedWorkspaceId, WorkspaceRevision expectedRevision)
    {
        if (!OperationArbiter.IsWorkspaceTransitionPendingOrReserved ||
            IsDisposed ||
            IsBusy ||
            HasPendingOperation ||
            WorkspaceCoordinator.CurrentWorkspace?.WorkspaceId != expectedWorkspaceId)
        {
            throw new InvalidOperationException(
                "Editor state can be invalidated only after an exact post-persistence refresh protected by a workspace transition.");
        }

        ReplaceGeneration();
        ResetForWorkspaceReplacement();
        SetStatus($"Workspace revision {expectedRevision} is current. Begin a new edit or select an exact FormList context.");
    }

    /// <summary>Checks whether an operation may still publish into its captured workspace generation.</summary>
    /// <param name="generation">The captured generation.</param>
    /// <param name="workspaceId">The captured workspace identity.</param>
    /// <returns><see langword="true"/> when both identities remain current.</returns>
    private bool IsCurrentGeneration(long generation, Guid workspaceId)
    {
        return !IsDisposed &&
            generation == Volatile.Read(ref WorkspaceGeneration) &&
            WorkspaceCoordinator.CurrentWorkspace?.WorkspaceId == workspaceId;
    }

    /// <summary>Validates a borrowed workspace and state against one captured desktop descriptor.</summary>
    /// <param name="workspace">The currently borrowed workspace.</param>
    /// <param name="state">The atomic borrowed state snapshot.</param>
    /// <param name="descriptor">The desktop descriptor captured before borrowing.</param>
    /// <returns>A typed identity or synchronization error, or <see langword="null"/> when exact validation succeeds.</returns>
    private static EngineError? ValidateWorkspaceState(
        IFormListWorkspace workspace,
        WorkspaceState state,
        NativeWorkspaceDescriptor descriptor)
    {
        if (workspace.WorkspaceId != descriptor.WorkspaceId)
        {
            return new EngineError(EngineErrorCode.InvalidRequest, "The active native workspace changed before the editor operation began.");
        }

        if (state.Game != descriptor.Game || state.Release != descriptor.Release)
        {
            return new EngineError(EngineErrorCode.UnsupportedGameRelease, "The active workspace game or release does not match the captured editor context.");
        }

        if (state.Output is null || descriptor.Output is null || !SameOutput(state.Output, descriptor.Output))
        {
            return new EngineError(EngineErrorCode.ExternalChangeDetected, "The selected native output changed before the editor operation began.");
        }

        if (state.OutputSynchronization.Status != OutputSynchronizationStatus.Ready)
        {
            return new EngineError(EngineErrorCode.InvalidRequest, $"Native editor operations require ready output synchronization; current state is {state.OutputSynchronization.Status}.");
        }

        return null;
    }

    /// <summary>Validates borrowed state against every identity captured by an active editor session.</summary>
    /// <param name="workspace">The currently borrowed workspace.</param>
    /// <param name="state">The atomic borrowed state snapshot.</param>
    /// <param name="descriptor">The current desktop descriptor.</param>
    /// <param name="session">The captured editor session.</param>
    /// <returns>A typed identity or synchronization error, or <see langword="null"/>.</returns>
    private static EngineError? ValidateSessionState(
        IFormListWorkspace workspace,
        WorkspaceState state,
        NativeWorkspaceDescriptor descriptor,
        NativeFormListEditorSession session)
    {
        var error = ValidateWorkspaceState(workspace, state, descriptor);
        if (error is not null)
        {
            return error;
        }

        if (session.WorkspaceId != descriptor.WorkspaceId ||
            session.Game != state.Game ||
            session.Release != state.Release ||
            state.Output is null ||
            !SameOutput(session.Output, state.Output))
        {
            return new EngineError(EngineErrorCode.ExternalChangeDetected, "The active native workspace no longer matches the captured editor session.");
        }

        return null;
    }

    /// <summary>Compares complete immutable output identities.</summary>
    /// <param name="left">The first output identity.</param>
    /// <param name="right">The second output identity.</param>
    /// <returns><see langword="true"/> when path, ModKey, localization, and master style match.</returns>
    private static bool SameOutput(OutputAssociation left, OutputAssociation right)
    {
        return string.Equals(left.PluginPath, right.PluginPath, StringComparison.OrdinalIgnoreCase) &&
            left.ModKey == right.ModKey &&
            left.LocalizedOutputMode == right.LocalizedOutputMode &&
            left.MasterStyle == right.MasterStyle;
    }

    /// <summary>Compares complete catalog identities without relying on object reference identity.</summary>
    /// <param name="left">The first catalog identity.</param>
    /// <param name="right">The second catalog identity.</param>
    /// <returns><see langword="true"/> when game, release, schema version, and content hash match.</returns>
    private static bool SameCatalogIdentity(
        NativeWireSchemaCatalogIdentity left,
        NativeWireSchemaCatalogIdentity right)
    {
        return left.Game == right.Game &&
            left.Release == right.Release &&
            string.Equals(left.SchemaVersion, right.SchemaVersion, StringComparison.Ordinal) &&
            string.Equals(left.CatalogId, right.CatalogId, StringComparison.Ordinal);
    }

    /// <summary>Compares catalog-bound node keys by complete content identity, kind, and name.</summary>
    /// <param name="left">The first key.</param>
    /// <param name="right">The second key.</param>
    /// <returns><see langword="true"/> when the exact keys match.</returns>
    private static bool SameSchemaKey(NativeWireSchemaNodeKey left, NativeWireSchemaNodeKey right)
    {
        return left.Kind == right.Kind &&
            string.Equals(left.CatalogId, right.CatalogId, StringComparison.Ordinal) &&
            string.Equals(left.Name, right.Name, StringComparison.Ordinal);
    }

    /// <summary>Publishes the current running, idle, or pending-outcome operation state.</summary>
    /// <param name="state">The new editor operation state.</param>
    private void SetOperationState(NativeFormListEditorOperationState state)
    {
        if (!SetProperty(ref OperationStateValue, state, nameof(OperationState)))
        {
            return;
        }

        OnPropertyChanged(nameof(IsBusy));
        OnPropertyChanged(nameof(CanMutateDraft));
        OnPropertyChanged(nameof(CanApply));
        OnPropertyChanged(nameof(CanDiscardFormChanges));
        OnPropertyChanged(nameof(CanBeginNew));
        OnPropertyChanged(nameof(CanBeginOverride));
        OnPropertyChanged(nameof(CanBeginExistingOutput));
        RaiseCommandStates();
    }

    /// <summary>Publishes a typed editor failure without discarding the current session or draft.</summary>
    /// <param name="error">The typed failure, or <see langword="null"/> for an unexpected missing result.</param>
    private void PublishError(EngineError? error)
    {
        var effective = error ?? new EngineError(EngineErrorCode.UnexpectedFailure, "The native editor operation failed without a diagnostic reason.");
        ErrorCodeValue = effective.Code;
        ErrorMessageValue = effective.Message;
        OnPropertyChanged(nameof(ErrorCode));
        OnPropertyChanged(nameof(ErrorMessage));
        OnPropertyChanged(nameof(HasError));
        SetStatus(effective.Message);
    }

    /// <summary>Clears the current editor failure.</summary>
    private void ClearError()
    {
        if (!HasError)
        {
            return;
        }

        ErrorCodeValue = null;
        ErrorMessageValue = null;
        OnPropertyChanged(nameof(ErrorCode));
        OnPropertyChanged(nameof(ErrorMessage));
        OnPropertyChanged(nameof(HasError));
    }

    /// <summary>Publishes one editor workflow status.</summary>
    /// <param name="status">The non-empty user-facing status.</param>
    private void SetStatus(string status)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(status);
        SetProperty(ref StatusTextValue, status, nameof(StatusText));
    }

    /// <summary>Publishes immutable non-fatal warnings.</summary>
    /// <param name="warnings">The warning sequence to detach.</param>
    private void SetWarnings(IEnumerable<EngineWarning> warnings)
    {
        ArgumentNullException.ThrowIfNull(warnings);
        WarningsValue = Array.AsReadOnly(warnings.ToArray());
        OnPropertyChanged(nameof(Warnings));
    }

    /// <summary>Publishes all derived session, command, draft, dirty, and retry properties.</summary>
    private void RaiseSessionProperties()
    {
        OnPropertyChanged(nameof(Session));
        OnPropertyChanged(nameof(IsSessionActive));
        OnPropertyChanged(nameof(SessionIdentityText));
        OnPropertyChanged(nameof(SessionRevisionText));
        OnPropertyChanged(nameof(AvailableCommands));
        OnPropertyChanged(nameof(SelectedCommand));
        OnPropertyChanged(nameof(IsStagedChangesKnown));
        OnPropertyChanged(nameof(HasStagedChanges));
        OnPropertyChanged(nameof(HasPendingOperation));
        OnPropertyChanged(nameof(CanMutateDraft));
        OnPropertyChanged(nameof(CanApply));
        OnPropertyChanged(nameof(CanDiscardFormChanges));
        RaiseCommandStates();
    }

    /// <summary>Raises command availability after any relevant editor state transition.</summary>
    private void RaiseCommandStates()
    {
        NewRelayCommand.RaiseCanExecuteChanged();
        OverrideRelayCommand.RaiseCanExecuteChanged();
        ExistingOutputRelayCommand.RaiseCanExecuteChanged();
        ApplyRelayCommand.RaiseCanExecuteChanged();
        DiscardFormChangesRelayCommand.RaiseCanExecuteChanged();
        RetryPendingRelayCommand.RaiseCanExecuteChanged();
    }

    /// <summary>Creates one typed local failure.</summary>
    /// <typeparam name="T">The expected successful value type.</typeparam>
    /// <param name="code">The stable failure category.</param>
    /// <param name="message">The complete diagnostic message.</param>
    /// <returns>A failed immutable engine result.</returns>
    private static EngineResult<T> Failure<T>(EngineErrorCode code, string message)
    {
        return EngineResult<T>.Failure(new EngineError(code, message));
    }

    /// <summary>Copies one typed engine failure without inventing a successful value.</summary>
    /// <typeparam name="TSource">The source success value type.</typeparam>
    /// <typeparam name="TTarget">The target success value type.</typeparam>
    /// <param name="source">The failed source result.</param>
    /// <returns>A failed result preserving operation context and warnings.</returns>
    private static EngineResult<TTarget> CopyFailure<TSource, TTarget>(EngineResult<TSource> source)
    {
        return EngineResult<TTarget>.Failure(
            source.Error ?? new EngineError(EngineErrorCode.UnexpectedFailure, "A native editor dependency failed without a diagnostic reason."),
            source.WorkspaceId,
            source.OperationId,
            source.BaseRevision,
            source.ResultRevision,
            source.Warnings);
    }
}

/// <summary>Contains a successful Begin receipt and its detached presentation follow-up state.</summary>
internal sealed class NativeFormListEditorBeginOutcome
{
    /// <summary>Initializes one known-success Begin outcome.</summary>
    /// <param name="receipt">The successful Core edit receipt.</param>
    /// <param name="catalogContext">The exact resolved catalog and codec pair.</param>
    /// <param name="commands">Every admitted command presentation.</param>
    /// <param name="seed">The exact staged seed when capture succeeded.</param>
    /// <param name="previewKnown">Whether preview established dirty state.</param>
    /// <param name="hasStagedChanges">Whether the session record has staged changes.</param>
    /// <param name="warnings">The combined non-fatal warnings.</param>
    /// <param name="postMutationMessage">A seed or preview failure after known Begin success.</param>
    public NativeFormListEditorBeginOutcome(
        EditReceipt receipt,
        NativeFormListWireCatalogContext catalogContext,
        IReadOnlyList<NativeFormListCommandPresentation> commands,
        NativeFormListDraftSeed? seed,
        bool previewKnown,
        bool hasStagedChanges,
        IReadOnlyList<EngineWarning> warnings,
        string? postMutationMessage)
    {
        ArgumentNullException.ThrowIfNull(receipt);
        ArgumentNullException.ThrowIfNull(catalogContext);
        ArgumentNullException.ThrowIfNull(commands);
        ArgumentNullException.ThrowIfNull(warnings);
        Receipt = receipt;
        CatalogContext = catalogContext;
        Commands = Array.AsReadOnly(commands.ToArray());
        Seed = seed;
        PreviewKnown = previewKnown;
        HasStagedChanges = hasStagedChanges;
        Warnings = Array.AsReadOnly(warnings.ToArray());
        PostMutationMessage = postMutationMessage;
    }

    /// <summary>Gets the successful Core edit receipt.</summary>
    public EditReceipt Receipt { get; }
    /// <summary>Gets the exact resolved catalog and codec pair.</summary>
    public NativeFormListWireCatalogContext CatalogContext { get; }
    /// <summary>Gets every admitted command presentation.</summary>
    public IReadOnlyList<NativeFormListCommandPresentation> Commands { get; }
    /// <summary>Gets the exact staged seed when capture succeeded.</summary>
    public NativeFormListDraftSeed? Seed { get; }
    /// <summary>Gets whether preview established dirty state.</summary>
    public bool PreviewKnown { get; }
    /// <summary>Gets whether the session FormList has staged changes.</summary>
    public bool HasStagedChanges { get; }
    /// <summary>Gets combined non-fatal warnings.</summary>
    public IReadOnlyList<EngineWarning> Warnings { get; }
    /// <summary>Gets a post-mutation read failure after known Begin success.</summary>
    public string? PostMutationMessage { get; }
}

/// <summary>Contains a successful Apply receipt and its detached preview and seed follow-up state.</summary>
internal sealed class NativeFormListEditorApplyOutcome
{
    /// <summary>Initializes one known-success Apply outcome.</summary>
    /// <param name="receipt">The successful Core operation receipt.</param>
    /// <param name="seed">The exact receipt-bound staged seed when capture succeeded.</param>
    /// <param name="previewKnown">Whether preview established dirty state.</param>
    /// <param name="hasStagedChanges">Whether the session record has staged semantic changes.</param>
    /// <param name="warnings">The combined non-fatal warnings.</param>
    /// <param name="postMutationMessage">A preview or seed failure after known Apply success.</param>
    public NativeFormListEditorApplyOutcome(
        OperationReceipt receipt,
        NativeFormListDraftSeed? seed,
        bool previewKnown,
        bool hasStagedChanges,
        IReadOnlyList<EngineWarning> warnings,
        string? postMutationMessage)
    {
        ArgumentNullException.ThrowIfNull(receipt);
        ArgumentNullException.ThrowIfNull(warnings);
        Receipt = receipt;
        Seed = seed;
        PreviewKnown = previewKnown;
        HasStagedChanges = hasStagedChanges;
        Warnings = Array.AsReadOnly(warnings.ToArray());
        PostMutationMessage = postMutationMessage;
    }

    /// <summary>Gets the successful Core operation receipt.</summary>
    public OperationReceipt Receipt { get; }
    /// <summary>Gets the exact receipt-bound staged seed when capture succeeded.</summary>
    public NativeFormListDraftSeed? Seed { get; }
    /// <summary>Gets whether preview established dirty state.</summary>
    public bool PreviewKnown { get; }
    /// <summary>Gets whether the session FormList has staged semantic changes.</summary>
    public bool HasStagedChanges { get; }
    /// <summary>Gets combined non-fatal warnings.</summary>
    public IReadOnlyList<EngineWarning> Warnings { get; }
    /// <summary>Gets a post-mutation read failure after known Apply success.</summary>
    public string? PostMutationMessage { get; }
}
