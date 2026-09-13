using System.ComponentModel;
using System.Windows.Input;
using CreationsForge.Commands;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Services;
using CreationsForge.Services.Interfaces;
using Serilog;

namespace CreationsForge.ViewModels;

/// <summary>Coordinates detached change review, exact persistence outcomes, recovery, and guarded workspace leave decisions.</summary>
public sealed partial class NativeWorkspaceChangesViewModel : ViewModelBase, IDisposable, IAsyncDisposable
{
    /// <summary>Protects operation admission, transition ownership, dialog-session state, and asynchronous shutdown state.</summary>
    private readonly object LifecycleGate = new();

    /// <summary>Owns and lends the application-wide native workspace.</summary>
    private readonly INativeWorkspaceCoordinator WorkspaceCoordinator;

    /// <summary>Inspects and repairs save journals without duplicating persistence logic.</summary>
    private readonly IWorkspaceSaveCoordinator SaveCoordinator;

    /// <summary>Atomically excludes editor work from workspace-wide transitions.</summary>
    private readonly INativeWorkspacePresentationOperationArbiter PresentationArbiter;

    /// <summary>Exposes request-local editor state and guarded browser refresh.</summary>
    private readonly INativeWorkspaceEditParticipant EditParticipant;

    /// <summary>Projects detached typed JSON into presentation-only trees.</summary>
    private readonly NativeJsonTreeProjectionService JsonTreeProjectionService;

    /// <summary>Presents the owner-bound shared change dialog.</summary>
    private readonly INativeWorkspaceChangesDialogService DialogService;

    /// <summary>Publishes bound state on the presentation thread.</summary>
    private readonly IUiDispatcher UiDispatcher;

    /// <summary>Records unexpected presentation lifecycle failures without exposing exception details.</summary>
    private readonly ILogger Logger;

    /// <summary>The command that opens a review dialog.</summary>
    private readonly AsyncRelayCommand ReviewChangesRelayCommand;

    /// <summary>The command that opens a save-purpose dialog.</summary>
    private readonly AsyncRelayCommand ShowSaveChangesDialogRelayCommand;

    /// <summary>The command that opens a discard-purpose dialog.</summary>
    private readonly AsyncRelayCommand ShowDiscardChangesDialogRelayCommand;

    /// <summary>The command that performs a confirmed save inside the owned dialog transition.</summary>
    private readonly AsyncRelayCommand SaveChangesRelayCommand;

    /// <summary>The command that performs a confirmed discard inside the owned dialog transition.</summary>
    private readonly AsyncRelayCommand DiscardChangesRelayCommand;

    /// <summary>The command that requests cancellation and drains the active operation.</summary>
    private readonly AsyncRelayCommand CancelActiveOperationRelayCommand;

    /// <summary>The command that inspects the exact original save outcome.</summary>
    private readonly AsyncRelayCommand InspectSaveOutcomeRelayCommand;

    /// <summary>The command that completes a reviewed prepared output set.</summary>
    private readonly AsyncRelayCommand CompletePreparedSaveRelayCommand;

    /// <summary>The command that restores a reviewed pre-save output set.</summary>
    private readonly AsyncRelayCommand RestorePreviousOutputRelayCommand;

    /// <summary>The command that resumes the original staged candidate after a not-committed recovery.</summary>
    private readonly AsyncRelayCommand ResumeUnsavedChangesRelayCommand;

    /// <summary>The command that reopens terminal resolved output evidence.</summary>
    private readonly AsyncRelayCommand ReopenResolvedOutputRelayCommand;

    /// <summary>The command that resumes only the incomplete presentation refresh.</summary>
    private readonly AsyncRelayCommand RetryCommittedRefreshRelayCommand;

    /// <summary>The most recent accepted detached review.</summary>
    private NativeWorkspaceChangeReview? CurrentReviewValue;

    /// <summary>Whether the most recent accepted review no longer describes current interactive state.</summary>
    private bool IsReviewStaleValue;

    /// <summary>The current lifecycle operation phase.</summary>
    private NativeWorkspaceChangesOperationState OperationStateValue = NativeWorkspaceChangesOperationState.Idle;

    /// <summary>Whether one Task 21 operation currently owns admission.</summary>
    private bool IsBusyValue;

    /// <summary>Whether cancellation was requested for the active Task 21 operation.</summary>
    private bool IsCancelRequestedValue;

    /// <summary>The last accepted output synchronization status.</summary>
    private OutputSynchronizationStatus? OutputSynchronizationStatusValue;

    /// <summary>The current user-visible lifecycle status.</summary>
    private string StatusTextValue = "No native workspace is open.";

    /// <summary>The stable typed current failure code.</summary>
    private EngineErrorCode? ErrorCodeValue;

    /// <summary>The complete current failure message.</summary>
    private string? ErrorMessageValue;

    /// <summary>The current immutable aggregate warning list.</summary>
    private IReadOnlyList<EngineWarning> WarningsValue = Array.Empty<EngineWarning>();

    /// <summary>The exact exclusive transition lease owned by the active dialog or leave attempt.</summary>
    private NativeWorkspaceTransitionLease? OwnedTransitionLease;

    /// <summary>The exact pending leave request that has closed editor admission without draining the captured operation.</summary>
    private NativeWorkspaceTransitionRequest? OwnedTransitionRequest;

    /// <summary>The active dialog request while one modal workflow owns the transition.</summary>
    private NativeWorkspaceChangesDialogRequest? ActiveDialogRequest;

    /// <summary>Whether one modal workflow has reserved this view model.</summary>
    private bool IsDialogSessionActive;

    /// <summary>The exact active operation completion used by cancellation and shutdown drains.</summary>
    private Task? ActiveOperationTask;

    /// <summary>The linked cancellation source owned by the active operation.</summary>
    private CancellationTokenSource? ActiveOperationCancellation;

    /// <summary>Whether asynchronous shutdown has begun.</summary>
    private bool IsShutdownRequested;

    /// <summary>The shared asynchronous shutdown task.</summary>
    private Task? ShutdownTask;

    /// <summary>Whether synchronous cleanup has detached change subscriptions.</summary>
    private bool AreSubscriptionsDetached;

    /// <summary>The exact most recently returned save result, including definitive failures.</summary>
    private SaveResult? LastSaveResultValue;

    /// <summary>The exact recovery presentation envelope for the original pending save.</summary>
    private NativeWorkspaceRecoveryEnvelope? RecoveryEnvelopeValue;

    /// <summary>An exact repair request retained only when no trustworthy repair response was received.</summary>
    private NativeWorkspaceRepairEnvelope? PendingRepairEnvelopeValue;

    /// <summary>A known successful persistence transition whose presentation refresh remains incomplete.</summary>
    private NativeWorkspaceRefreshEnvelope? PendingRefreshEnvelopeValue;

    /// <summary>The exact external-conflict identity eligible for a second Open-only abandonment confirmation.</summary>
    private NativeWorkspaceAbandonmentEnvelope? AbandonmentEnvelopeValue;

    /// <summary>The preliminary leave disposition recorded by the closed dialog-choice state machine.</summary>
    private NativeWorkspaceLeaveDisposition? PendingLeaveDisposition;

    /// <summary>Advances after a local-only discard or a known successful Core transition is fully refreshed.</summary>
    private long CompletedPersistenceVersion;

    /// <summary>Initializes the navigation-scope native workspace change lifecycle.</summary>
    /// <param name="workspaceCoordinator">The root-lifetime native workspace owner.</param>
    /// <param name="saveCoordinator">The shared save recovery and repair coordinator.</param>
    /// <param name="presentationArbiter">The navigation-scope presentation operation arbiter.</param>
    /// <param name="editParticipant">The navigation-scope browser and editor participant.</param>
    /// <param name="jsonTreeProjectionService">The detached native JSON projector.</param>
    /// <param name="dialogService">The owner-bound shared change dialog.</param>
    /// <param name="uiDispatcher">The presentation dispatcher.</param>
    /// <param name="logger">The structured application logger.</param>
    /// <exception cref="ArgumentNullException">Thrown when a required dependency is <see langword="null"/>.</exception>
    public NativeWorkspaceChangesViewModel(
        INativeWorkspaceCoordinator workspaceCoordinator,
        IWorkspaceSaveCoordinator saveCoordinator,
        INativeWorkspacePresentationOperationArbiter presentationArbiter,
        INativeWorkspaceEditParticipant editParticipant,
        NativeJsonTreeProjectionService jsonTreeProjectionService,
        INativeWorkspaceChangesDialogService dialogService,
        IUiDispatcher uiDispatcher,
        ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(workspaceCoordinator);
        ArgumentNullException.ThrowIfNull(saveCoordinator);
        ArgumentNullException.ThrowIfNull(presentationArbiter);
        ArgumentNullException.ThrowIfNull(editParticipant);
        ArgumentNullException.ThrowIfNull(jsonTreeProjectionService);
        ArgumentNullException.ThrowIfNull(dialogService);
        ArgumentNullException.ThrowIfNull(uiDispatcher);
        ArgumentNullException.ThrowIfNull(logger);
        WorkspaceCoordinator = workspaceCoordinator;
        SaveCoordinator = saveCoordinator;
        PresentationArbiter = presentationArbiter;
        EditParticipant = editParticipant;
        JsonTreeProjectionService = jsonTreeProjectionService;
        DialogService = dialogService;
        UiDispatcher = uiDispatcher;
        Logger = logger.ForContext<NativeWorkspaceChangesViewModel>();

        ReviewChangesRelayCommand = new AsyncRelayCommand(() => ReviewChangesAsync(), () => CanReviewChanges);
        ShowSaveChangesDialogRelayCommand = new AsyncRelayCommand(() => ShowSaveChangesDialogAsync(), () => CanOpenPersistenceDialog);
        ShowDiscardChangesDialogRelayCommand = new AsyncRelayCommand(() => ShowDiscardChangesDialogAsync(), () => CanOpenPersistenceDialog);
        SaveChangesRelayCommand = new AsyncRelayCommand(() => SaveChangesAsync(), () => CanSaveChanges);
        DiscardChangesRelayCommand = new AsyncRelayCommand(() => DiscardChangesAsync(), () => CanDiscardChanges);
        CancelActiveOperationRelayCommand = new AsyncRelayCommand(() => CancelActiveOperationAndDrainAsync(), () => CanRequestCancellation);
        InspectSaveOutcomeRelayCommand = new AsyncRelayCommand(() => InspectSaveOutcomeAsync(), () => CanInspectSaveOutcome);
        CompletePreparedSaveRelayCommand = new AsyncRelayCommand(() => CompletePreparedSaveAsync(), () => CanCompletePreparedSave);
        RestorePreviousOutputRelayCommand = new AsyncRelayCommand(() => RestorePreviousOutputAsync(), () => CanRestorePreviousOutput);
        ResumeUnsavedChangesRelayCommand = new AsyncRelayCommand(() => ResumeUnsavedChangesAsync(), () => CanResumeUnsavedChanges);
        ReopenResolvedOutputRelayCommand = new AsyncRelayCommand(() => ReopenResolvedOutputAsync(), () => CanReopenResolvedOutput);
        RetryCommittedRefreshRelayCommand = new AsyncRelayCommand(() => RetryCommittedRefreshAsync(), () => CanRetryCommittedRefresh);

        ReviewChangesCommand = ReviewChangesRelayCommand;
        ShowSaveChangesDialogCommand = ShowSaveChangesDialogRelayCommand;
        ShowDiscardChangesDialogCommand = ShowDiscardChangesDialogRelayCommand;
        SaveChangesCommand = SaveChangesRelayCommand;
        DiscardChangesCommand = DiscardChangesRelayCommand;
        CancelActiveOperationCommand = CancelActiveOperationRelayCommand;
        InspectSaveOutcomeCommand = InspectSaveOutcomeRelayCommand;
        CompletePreparedSaveCommand = CompletePreparedSaveRelayCommand;
        RestorePreviousOutputCommand = RestorePreviousOutputRelayCommand;
        ResumeUnsavedChangesCommand = ResumeUnsavedChangesRelayCommand;
        ReopenResolvedOutputCommand = ReopenResolvedOutputRelayCommand;
        RetryCommittedRefreshCommand = RetryCommittedRefreshRelayCommand;

        EditParticipant.PropertyChanged += OnAdmissionDependencyPropertyChanged;
        PresentationArbiter.PropertyChanged += OnAdmissionDependencyPropertyChanged;
        WorkspaceCoordinator.PropertyChanged += OnWorkspaceCoordinatorPropertyChanged;
    }

    /// <summary>Gets the command that opens a fresh detached review dialog.</summary>
    public ICommand ReviewChangesCommand { get; }

    /// <summary>Gets the toolbar command that opens a save-purpose change dialog without saving recursively.</summary>
    public ICommand ShowSaveChangesDialogCommand { get; }

    /// <summary>Gets the toolbar command that opens a discard-purpose change dialog without discarding recursively.</summary>
    public ICommand ShowDiscardChangesDialogCommand { get; }

    /// <summary>Gets the dialog command that saves all staged workspace changes.</summary>
    public ICommand SaveChangesCommand { get; }

    /// <summary>Gets the dialog command that explicitly discards request-local and staged workspace changes.</summary>
    public ICommand DiscardChangesCommand { get; }

    /// <summary>Gets the command that requests cancellation and drains the active operation.</summary>
    public ICommand CancelActiveOperationCommand { get; }

    /// <summary>Gets the command that inspects the exact original save outcome.</summary>
    public ICommand InspectSaveOutcomeCommand { get; }

    /// <summary>Gets the command that explicitly completes a reviewed prepared save.</summary>
    public ICommand CompletePreparedSaveCommand { get; }

    /// <summary>Gets the command that explicitly restores the reviewed previous output.</summary>
    public ICommand RestorePreviousOutputCommand { get; }

    /// <summary>Gets the command that resumes the original staged candidate after not-committed recovery.</summary>
    public ICommand ResumeUnsavedChangesCommand { get; }

    /// <summary>Gets the command that reopens terminal resolved output evidence.</summary>
    public ICommand ReopenResolvedOutputCommand { get; }

    /// <summary>Gets the command that retries only an incomplete post-persistence presentation refresh.</summary>
    public ICommand RetryCommittedRefreshCommand { get; }

    /// <summary>Gets the most recent accepted revision-consistent detached review.</summary>
    public NativeWorkspaceChangeReview? CurrentReview => CurrentReviewValue;

    /// <summary>Gets whether the retained accepted review is visible history rather than current mutation input.</summary>
    public bool IsReviewStale => IsReviewStaleValue;

    /// <summary>Gets the current closed lifecycle operation phase.</summary>
    public NativeWorkspaceChangesOperationState OperationState => OperationStateValue;

    /// <summary>Gets whether one Task 21 operation currently owns admission.</summary>
    public bool IsBusy => IsBusyValue;

    /// <summary>Gets whether cancellation was requested while the active operation drains.</summary>
    public bool IsCancelRequested => IsCancelRequestedValue;

    /// <summary>Gets whether request-local form input differs from its editor seed.</summary>
    public bool HasDraftChanges => EditParticipant.HasDraftChanges;

    /// <summary>Gets whether an uncertain Task 20 operation awaits exact replay.</summary>
    public bool HasPendingEditorOperation => EditParticipant.HasPendingOperation;

    /// <summary>Gets the last accepted workspace output synchronization status.</summary>
    public OutputSynchronizationStatus? OutputSynchronizationStatus => OutputSynchronizationStatusValue;

    /// <summary>Gets the current user-visible lifecycle status.</summary>
    public string StatusText => StatusTextValue;

    /// <summary>Gets the current stable typed failure code.</summary>
    public EngineErrorCode? ErrorCode => ErrorCodeValue;

    /// <summary>Gets the complete current failure message.</summary>
    public string? ErrorMessage => ErrorMessageValue;

    /// <summary>Gets the current immutable warnings in source order.</summary>
    public IReadOnlyList<EngineWarning> Warnings => WarningsValue;

    /// <summary>Gets whether the review toolbar can open a new modal session.</summary>
    public bool CanReviewChanges => CanOpenDialog;

    /// <summary>Gets whether a save or discard toolbar action can open a new modal session.</summary>
    public bool CanOpenPersistenceDialog => CanOpenDialog;

    /// <summary>Gets whether the active owned dialog transition can start an exact save.</summary>
    public bool CanSaveChanges => HasActiveOwnedTransition
        && !IsBusy
        && !HasDraftChanges
        && !HasPendingEditorOperation
        && PendingRefreshEnvelopeValue is null
        && LastSaveResultValue?.Status != SaveCommitStatus.Committed
        && OutputSynchronizationStatus == global::CreationsForge.Core.Engine.Contracts.OutputSynchronizationStatus.Ready
        && CurrentReview?.HasStagedChanges == true;

    /// <summary>Gets whether the active owned dialog transition can explicitly discard local or staged changes.</summary>
    public bool CanDiscardChanges => HasActiveOwnedTransition
        && !IsBusy
        && !HasPendingEditorOperation
        && PendingRefreshEnvelopeValue is null
        && LastSaveResultValue?.Status != SaveCommitStatus.Committed
        && OutputSynchronizationStatus == global::CreationsForge.Core.Engine.Contracts.OutputSynchronizationStatus.Ready
        && (HasDraftChanges || CurrentReview?.HasStagedChanges == true);

    /// <summary>Gets whether the original pending save can be inspected read-only.</summary>
    public bool CanInspectSaveOutcome => HasActiveOwnedTransition
        && !IsBusy
        && !HasPendingEditorOperation
        && PendingRefreshEnvelopeValue is null
        && OutputSynchronizationStatus is global::CreationsForge.Core.Engine.Contracts.OutputSynchronizationStatus.RecoveryRequired
            or global::CreationsForge.Core.Engine.Contracts.OutputSynchronizationStatus.ReopenRequired;

    /// <summary>Gets whether the reviewed incomplete save can explicitly complete its prepared set.</summary>
    public bool CanCompletePreparedSave => CanRepair(RepairSaveDirection.CompletePrepared);

    /// <summary>Gets whether the reviewed incomplete save can explicitly restore its previous output.</summary>
    public bool CanRestorePreviousOutput => CanRepair(RepairSaveDirection.RestoreBaseline);

    /// <summary>Gets whether the matching original live workspace can resume its staged candidate.</summary>
    public bool CanResumeUnsavedChanges => HasActiveOwnedTransition
        && !IsBusy
        && !HasPendingEditorOperation
        && PendingRefreshEnvelopeValue is null
        && RecoveryEnvelopeValue?.CanResumeStaged == true
        && RecoveryEnvelopeValue.Result.ResolvedEvidence is not null;

    /// <summary>Gets whether terminal resolved output evidence can be reopened by the live workspace.</summary>
    public bool CanReopenResolvedOutput => HasActiveOwnedTransition
        && !IsBusy
        && !HasPendingEditorOperation
        && PendingRefreshEnvelopeValue is null
        && RecoveryEnvelopeValue?.Result.ResolvedEvidence is not null;

    /// <summary>Gets the stable action label that states whether reopening saves, resolves, or discards the retained staged candidate.</summary>
    public string ReopenResolvedOutputLabel => RecoveryEnvelopeValue?.Result.Status switch
    {
        RecoverSaveStatus.Committed => "Reopen Saved Output",
        RecoverSaveStatus.NotCommitted when RecoveryEnvelopeValue.CanResumeStaged => "Discard Staged Changes and Reopen Output",
        _ => "Reopen Resolved Output",
    };

    /// <summary>Gets whether a known successful Core transition has only presentation refresh work left.</summary>
    public bool CanRetryCommittedRefresh => HasActiveOwnedTransition
        && !IsBusy
        && !HasPendingEditorOperation
        && PendingRefreshEnvelopeValue is not null;

    /// <summary>Gets whether a dialog close gesture may close immediately.</summary>
    public bool CanDismissDialog => !IsBusy && PendingRefreshEnvelopeValue is null;

    /// <summary>Gets whether the active operation can receive one cancellation request.</summary>
    public bool CanRequestCancellation => IsBusy && !IsCancelRequested;

    /// <summary>Gets whether the active dialog should present a save-and-proceed choice.</summary>
    public bool ShowSaveAndProceed => (ActiveDialogRequest?.Purpose is NativeWorkspaceChangesDialogPurpose.Save or NativeWorkspaceChangesDialogPurpose.Leave)
        && !ShowActiveEditorOperationDecision
        && !HasPendingEditorOperation;

    /// <summary>Gets whether the active dialog can save and then ask the leave state machine to proceed.</summary>
    public bool CanSaveAndProceed => CanSaveChanges;

    /// <summary>Gets the adjacent explanation when save-and-proceed is unavailable.</summary>
    public string? SaveAndProceedDisabledReason => CanSaveAndProceed ? null : GetSaveDisabledReason();

    /// <summary>Gets whether the active dialog should present a discard-and-proceed choice.</summary>
    public bool ShowDiscardAndProceed => (ActiveDialogRequest?.Purpose is NativeWorkspaceChangesDialogPurpose.Discard or NativeWorkspaceChangesDialogPurpose.Leave)
        && !ShowActiveEditorOperationDecision
        && !HasPendingEditorOperation;

    /// <summary>Gets whether the active dialog can discard and then ask the leave state machine to proceed.</summary>
    public bool CanDiscardAndProceed => CanDiscardChanges;

    /// <summary>Gets the adjacent explanation when discard-and-proceed is unavailable.</summary>
    public string? DiscardAndProceedDisabledReason => CanDiscardAndProceed ? null : GetDiscardDisabledReason();

    /// <summary>Gets whether a pending editor operation requires returning to the editor.</summary>
    public bool ShowReturnToEditor => ActiveDialogRequest?.Purpose == NativeWorkspaceChangesDialogPurpose.Leave
        && !ShowActiveEditorOperationDecision
        && HasPendingEditorOperation;

    /// <summary>Gets whether the dialog may close so the exact pending editor operation can be resolved.</summary>
    public bool CanReturnToEditor => ShowReturnToEditor && !IsBusy;

    /// <summary>Gets whether a leave dialog must ask how to drain the exact captured editor operation.</summary>
    public bool ShowActiveEditorOperationDecision => ActiveDialogRequest?.Purpose == NativeWorkspaceChangesDialogPurpose.Leave
        && OwnedTransitionRequest is not null
        && !HasActiveOwnedTransition;

    /// <summary>Gets whether the active leave dialog can wait naturally for the captured editor operation.</summary>
    public bool CanWaitForEditorOperation => ShowActiveEditorOperationDecision && !IsBusy && !IsShutdownRequested;

    /// <summary>Gets whether the active leave dialog can request cancellation and drain the captured editor operation.</summary>
    public bool CanCancelEditorOperationAndWait => ShowActiveEditorOperationDecision && !IsBusy && !IsShutdownRequested;

    /// <summary>Gets whether an Open-only second abandonment confirmation is available for the exact external conflict.</summary>
    public bool ShowConfirmedAbandonmentForOpen => ActiveDialogRequest?.Purpose == NativeWorkspaceChangesDialogPurpose.Leave
        && !ShowActiveEditorOperationDecision
        && !HasPendingEditorOperation
        && ActiveDialogRequest.LeaveReason == NativeWorkspaceLeaveReason.OpenWorkspace
        && AbandonmentEnvelopeValue is not null;

    /// <summary>Gets whether the exact external-conflict workspace can be explicitly abandoned only through Open.</summary>
    public bool CanConfirmAbandonmentForOpen => ShowConfirmedAbandonmentForOpen && !IsBusy;

    /// <summary>Gets the exact output-specific explanation for the second Open-only abandonment confirmation.</summary>
    public string? ConfirmedAbandonmentExplanation => AbandonmentEnvelopeValue is null
        ? null
        : $"Discard current workspace changes for {AbandonmentEnvelopeValue.Output.ModKey.FileName} and open another workspace. The changed output will not be overwritten.";

    /// <summary>Gets the exact returned save result retained for diagnostics and refresh retry tests.</summary>
    internal SaveResult? LastSaveResult => LastSaveResultValue;

    /// <summary>Gets the exact retained recovery presentation envelope.</summary>
    internal NativeWorkspaceRecoveryEnvelope? RecoveryEnvelope => RecoveryEnvelopeValue;

    /// <summary>Gets the known successful Core transition awaiting only presentation refresh.</summary>
    internal NativeWorkspaceRefreshEnvelope? PendingRefreshEnvelope => PendingRefreshEnvelopeValue;

    /// <summary>Gets whether this view model currently owns an active transition lease.</summary>
    private bool HasActiveOwnedTransition => OwnedTransitionLease?.IsActive == true;

    /// <summary>Gets whether this view model owns a leave request that has not transferred into a transition lease.</summary>
    private bool HasActiveOwnedTransitionRequest => OwnedTransitionRequest is not null;

    /// <summary>Gets whether the exact editor captured by the pending leave request is still active.</summary>
    private bool IsCapturedEditorOperationActive => OwnedTransitionRequest?.IsEditorOperationActive == true;

    /// <summary>Gets whether a new toolbar modal session can reserve presentation admission.</summary>
    private bool CanOpenDialog => WorkspaceCoordinator.CurrentWorkspace?.Output is not null
        && !IsBusy
        && !IsDialogSessionActive
        && !IsShutdownRequested;

    /// <summary>Requests cancellation once and waits for the already-owned active operation to reach terminal mapping.</summary>
    /// <param name="cancellationToken">A token observed before cancellation is requested; it cannot truncate the safety drain afterward.</param>
    /// <returns>A task that completes after the active operation has fully drained.</returns>
    /// <exception cref="OperationCanceledException">Thrown when <paramref name="cancellationToken"/> is already canceled before the request is made.</exception>
    public async Task CancelActiveOperationAndDrainAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Task? operationTask;
        CancellationTokenSource? operationCancellation;
        lock (LifecycleGate)
        {
            operationTask = ActiveOperationTask;
            operationCancellation = ActiveOperationCancellation;
        }

        if (operationTask is null || operationCancellation is null)
        {
            return;
        }

        RequestCancellationSafely(operationCancellation);
        await PublishOperationStateAsync(
            NativeWorkspaceChangesOperationState.CancelRequestedWaitingForResult,
            isCancelRequested: true,
            "Cancel requested; waiting for a safe result.").ConfigureAwait(false);
        await operationTask.ConfigureAwait(false);
    }

    /// <summary>Requests cancellation and drains every owned operation before the navigation scope is released.</summary>
    /// <returns>A shared task that completes after operations drain, untransferred admission is released, and subscriptions are detached.</returns>
    public Task ShutdownAndDrainAsync()
    {
        lock (LifecycleGate)
        {
            ShutdownTask ??= ShutdownCoreAsync();
            return ShutdownTask;
        }
    }

    /// <summary>Signals shutdown cancellation without claiming that asynchronous operations have drained.</summary>
    public void Dispose()
    {
        Task? ignored = ShutdownAndDrainAsync();
        _ = ignored;
    }

    /// <summary>Asynchronously drains the change lifecycle before its navigation scope is released.</summary>
    /// <returns>A value task that completes after <see cref="ShutdownAndDrainAsync"/> completes.</returns>
    public async ValueTask DisposeAsync()
    {
        await ShutdownAndDrainAsync().ConfigureAwait(false);
    }

    /// <summary>Runs the shared asynchronous shutdown sequence once.</summary>
    /// <returns>A task that completes after owned state is safely drained and released.</returns>
    private async Task ShutdownCoreAsync()
    {
        Task? operationTask;
        CancellationTokenSource? operationCancellation;
        lock (LifecycleGate)
        {
            IsShutdownRequested = true;
            operationTask = ActiveOperationTask;
            operationCancellation = ActiveOperationCancellation;
        }

        if (operationCancellation is not null)
        {
            RequestCancellationSafely(operationCancellation);
        }
        if (operationTask is not null)
        {
            await operationTask.ConfigureAwait(false);
        }

        NativeWorkspaceTransitionRequest? request;
        NativeWorkspaceTransitionLease? lease;
        lock (LifecycleGate)
        {
            request = OwnedTransitionRequest;
            OwnedTransitionRequest = null;
            lease = OwnedTransitionLease;
            OwnedTransitionLease = null;
            IsDialogSessionActive = false;
            ActiveDialogRequest = null;
        }

        if (request is not null)
        {
            await request.DisposeAsync().ConfigureAwait(false);
        }

        lease?.Dispose();
        DetachSubscriptions();
        await PublishOperationStateAsync(NativeWorkspaceChangesOperationState.Idle, false, StatusTextValue).ConfigureAwait(false);
    }

    /// <summary>Runs one atomically admitted Task 21 operation and exposes its completion to cancellation and shutdown drains.</summary>
    /// <param name="state">The initial operation state.</param>
    /// <param name="cancellationToken">The caller token linked to the owned operation cancellation source.</param>
    /// <param name="operation">The operation body.</param>
    /// <returns>A task that completes after terminal state publication.</returns>
    private Task RunOperationAsync(
        NativeWorkspaceChangesOperationState state,
        CancellationToken cancellationToken,
        Func<CancellationToken, Task> operation)
    {
        ArgumentNullException.ThrowIfNull(operation);
        TaskCompletionSource completion;
        CancellationTokenSource operationCancellation;
        lock (LifecycleGate)
        {
            if (ActiveOperationTask is not null || IsShutdownRequested)
            {
                return Task.CompletedTask;
            }

            completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            operationCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            ActiveOperationTask = completion.Task;
            ActiveOperationCancellation = operationCancellation;
        }

        return ExecuteAdmittedOperationAsync(state, operationCancellation, completion, operation);
    }

    /// <summary>Executes one admitted operation and always publishes terminal drain.</summary>
    /// <param name="state">The initial operation state.</param>
    /// <param name="operationCancellation">The owned linked cancellation source.</param>
    /// <param name="completion">The completion observed by cancel and shutdown callers.</param>
    /// <param name="operation">The operation body.</param>
    /// <returns>A task that completes after cleanup and completion publication.</returns>
    private async Task ExecuteAdmittedOperationAsync(
        NativeWorkspaceChangesOperationState state,
        CancellationTokenSource operationCancellation,
        TaskCompletionSource completion,
        Func<CancellationToken, Task> operation)
    {
        try
        {
            await PublishOperationStateAsync(state, false, GetActiveStatus(state)).ConfigureAwait(false);
            try
            {
                await operation(operationCancellation.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (operationCancellation.IsCancellationRequested)
            {
                await PublishFailureAsync(
                    new EngineError(EngineErrorCode.InvalidRequest, "The operation was canceled before it changed accepted workspace state."),
                    StatusTextValue,
                    WarningsValue).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Unexpected Task 21 presentation operation failure in state {OperationState}", state);
                await PublishFailureAsync(
                    new EngineError(EngineErrorCode.UnexpectedFailure, "An unexpected presentation failure prevented the workspace operation from completing."),
                    StatusTextValue,
                    WarningsValue).ConfigureAwait(false);
            }
        }
        finally
        {
            try
            {
                await UiDispatcher.InvokeAsync(() =>
                {
                    IsBusyValue = false;
                    IsCancelRequestedValue = false;
                    OperationStateValue = NativeWorkspaceChangesOperationState.Idle;
                    RaiseAllBoundState();
                }).ConfigureAwait(false);
            }
            finally
            {
                IsBusyValue = false;
                IsCancelRequestedValue = false;
                OperationStateValue = NativeWorkspaceChangesOperationState.Idle;
                lock (LifecycleGate)
                {
                    ActiveOperationTask = null;
                    ActiveOperationCancellation = null;
                }

                operationCancellation.Dispose();
                completion.TrySetResult();
            }
        }
    }

    /// <summary>Publishes an active operation state and its exact user-visible text.</summary>
    /// <param name="state">The operation phase.</param>
    /// <param name="isCancelRequested">Whether cancellation was requested.</param>
    /// <param name="statusText">The current user-visible status.</param>
    /// <returns>A task that completes after presentation publication.</returns>
    private Task PublishOperationStateAsync(
        NativeWorkspaceChangesOperationState state,
        bool isCancelRequested,
        string statusText)
    {
        return UiDispatcher.InvokeAsync(() =>
        {
            OperationStateValue = state;
            IsBusyValue = state != NativeWorkspaceChangesOperationState.Idle;
            IsCancelRequestedValue = isCancelRequested;
            StatusTextValue = statusText;
            RaiseAllBoundState();
        });
    }

    /// <summary>Publishes one typed failure while retaining the prior accepted review as stale history.</summary>
    /// <param name="error">The exact typed failure.</param>
    /// <param name="statusText">The required visible lifecycle status.</param>
    /// <param name="warnings">Warnings retained with the failure.</param>
    /// <returns>A task that completes after presentation publication.</returns>
    private Task PublishFailureAsync(
        EngineError error,
        string statusText,
        IReadOnlyList<EngineWarning>? warnings = null)
    {
        ArgumentNullException.ThrowIfNull(error);
        return UiDispatcher.InvokeAsync(() =>
        {
            ErrorCodeValue = error.Code;
            ErrorMessageValue = error.Message;
            StatusTextValue = statusText;
            WarningsValue = SnapshotWarnings(warnings);
            IsReviewStaleValue = CurrentReviewValue is not null;
            RaiseAllBoundState();
        });
    }

    /// <summary>Publishes an accepted status, clears the current error, and optionally replaces warnings.</summary>
    /// <param name="statusText">The exact visible lifecycle status.</param>
    /// <param name="warnings">Warnings retained with the accepted status.</param>
    /// <returns>A task that completes after presentation publication.</returns>
    private Task PublishSuccessAsync(string statusText, IReadOnlyList<EngineWarning>? warnings = null)
    {
        return UiDispatcher.InvokeAsync(() =>
        {
            ErrorCodeValue = null;
            ErrorMessageValue = null;
            StatusTextValue = statusText;
            WarningsValue = SnapshotWarnings(warnings);
            RaiseAllBoundState();
        });
    }

    /// <summary>Copies warnings to an immutable collection.</summary>
    /// <param name="warnings">Warnings to copy, or <see langword="null"/>.</param>
    /// <returns>An immutable warning collection.</returns>
    private static IReadOnlyList<EngineWarning> SnapshotWarnings(IReadOnlyList<EngineWarning>? warnings)
    {
        return Array.AsReadOnly(warnings?.ToArray() ?? Array.Empty<EngineWarning>());
    }

    /// <summary>Returns user-visible indeterminate text for an active operation phase.</summary>
    /// <param name="state">The active phase.</param>
    /// <returns>The exact user-visible operation text.</returns>
    private static string GetActiveStatus(NativeWorkspaceChangesOperationState state)
    {
        return state switch
        {
            NativeWorkspaceChangesOperationState.ReservingTransition => "Preparing change review...",
            NativeWorkspaceChangesOperationState.WaitingForEditorOperation => "Waiting for the current editor operation to finish.",
            NativeWorkspaceChangesOperationState.Reviewing => "Reviewing changes...",
            NativeWorkspaceChangesOperationState.Saving => "Saving output...",
            NativeWorkspaceChangesOperationState.Discarding => "Discarding staged changes...",
            NativeWorkspaceChangesOperationState.InspectingRecovery => "Inspecting save outcome...",
            NativeWorkspaceChangesOperationState.Repairing => "Repairing saved output...",
            NativeWorkspaceChangesOperationState.AdoptingRecovery => "Reopening resolved output...",
            NativeWorkspaceChangesOperationState.Refreshing => "Refreshing workspace...",
            NativeWorkspaceChangesOperationState.CancelRequestedWaitingForResult => "Cancel requested; waiting for a safe result.",
            _ => string.Empty,
        };
    }

    /// <summary>Gets whether the exact reviewed recovery state permits the requested repair direction.</summary>
    /// <param name="direction">The explicit repair direction.</param>
    /// <returns><see langword="true"/> when a reviewed repair-required unknown state permits the request.</returns>
    private bool CanRepair(RepairSaveDirection direction)
    {
        return HasActiveOwnedTransition
            && !IsBusy
            && PendingRefreshEnvelopeValue is null
            && !HasPendingEditorOperation
            && RecoveryEnvelopeValue?.Result.Status == RecoverSaveStatus.StillUnknown
            && RecoveryEnvelopeValue.Result.RepairRequired
            && RecoveryEnvelopeValue.Result.SaveBaseRevision is not null
            && RecoveryEnvelopeValue.Result.EvidenceToken is not null
            && (PendingRepairEnvelopeValue is null || PendingRepairEnvelopeValue.Request.Direction == direction)
            && Enum.IsDefined(direction);
    }

    /// <summary>Requests cancellation of one exact operation source while tolerating its concurrent terminal disposal.</summary>
    /// <param name="cancellationSource">The source captured under the lifecycle lock for the operation being drained.</param>
    private static void RequestCancellationSafely(CancellationTokenSource cancellationSource)
    {
        try
        {
            cancellationSource.Cancel();
        }
        catch (ObjectDisposedException)
        {
        }
    }

    /// <summary>Returns the precise reason an exact save is currently unavailable.</summary>
    /// <returns>The adjacent explanation, or <see langword="null"/> when no special reason applies.</returns>
    private string? GetSaveDisabledReason()
    {
        if (HasDraftChanges)
        {
            return "Form changes not applied";
        }

        if (HasPendingEditorOperation)
        {
            return "Return to editor and resolve the pending form operation.";
        }

        if (PendingRefreshEnvelopeValue is not null)
        {
            return "Retry the completed persistence refresh before another save.";
        }

        if (OutputSynchronizationStatus != global::CreationsForge.Core.Engine.Contracts.OutputSynchronizationStatus.Ready)
        {
            return "Resolve the pending save outcome before editing, saving, discarding, or closing.";
        }

        if (CurrentReview?.HasStagedChanges != true)
        {
            return "No unsaved changes";
        }

        return IsBusy ? "Wait for the current workspace operation to finish." : null;
    }

    /// <summary>Returns the precise reason an explicit discard is currently unavailable.</summary>
    /// <returns>The adjacent explanation, or <see langword="null"/> when no special reason applies.</returns>
    private string? GetDiscardDisabledReason()
    {
        if (HasPendingEditorOperation)
        {
            return "Return to editor and resolve the pending form operation.";
        }

        if (PendingRefreshEnvelopeValue is not null)
        {
            return "Retry the completed persistence refresh before discarding.";
        }

        if (OutputSynchronizationStatus != global::CreationsForge.Core.Engine.Contracts.OutputSynchronizationStatus.Ready)
        {
            return "Resolve the pending save outcome before discarding staged changes.";
        }

        if (!HasDraftChanges && CurrentReview?.HasStagedChanges != true)
        {
            return "No unsaved changes";
        }

        return IsBusy ? "Wait for the current workspace operation to finish." : null;
    }

    /// <summary>Raises every bound state and command availability after one atomic publication.</summary>
    private void RaiseAllBoundState()
    {
        OnPropertyChanged(nameof(CurrentReview));
        OnPropertyChanged(nameof(IsReviewStale));
        OnPropertyChanged(nameof(OperationState));
        OnPropertyChanged(nameof(IsBusy));
        OnPropertyChanged(nameof(IsCancelRequested));
        OnPropertyChanged(nameof(HasDraftChanges));
        OnPropertyChanged(nameof(HasPendingEditorOperation));
        OnPropertyChanged(nameof(OutputSynchronizationStatus));
        OnPropertyChanged(nameof(StatusText));
        OnPropertyChanged(nameof(ErrorCode));
        OnPropertyChanged(nameof(ErrorMessage));
        OnPropertyChanged(nameof(Warnings));
        OnPropertyChanged(nameof(CanReviewChanges));
        OnPropertyChanged(nameof(CanOpenPersistenceDialog));
        OnPropertyChanged(nameof(CanSaveChanges));
        OnPropertyChanged(nameof(CanDiscardChanges));
        OnPropertyChanged(nameof(CanInspectSaveOutcome));
        OnPropertyChanged(nameof(CanCompletePreparedSave));
        OnPropertyChanged(nameof(CanRestorePreviousOutput));
        OnPropertyChanged(nameof(CanResumeUnsavedChanges));
        OnPropertyChanged(nameof(CanReopenResolvedOutput));
        OnPropertyChanged(nameof(ReopenResolvedOutputLabel));
        OnPropertyChanged(nameof(CanRetryCommittedRefresh));
        OnPropertyChanged(nameof(CanDismissDialog));
        OnPropertyChanged(nameof(CanRequestCancellation));
        OnPropertyChanged(nameof(ShowSaveAndProceed));
        OnPropertyChanged(nameof(CanSaveAndProceed));
        OnPropertyChanged(nameof(SaveAndProceedDisabledReason));
        OnPropertyChanged(nameof(ShowDiscardAndProceed));
        OnPropertyChanged(nameof(CanDiscardAndProceed));
        OnPropertyChanged(nameof(DiscardAndProceedDisabledReason));
        OnPropertyChanged(nameof(ShowReturnToEditor));
        OnPropertyChanged(nameof(CanReturnToEditor));
        OnPropertyChanged(nameof(ShowActiveEditorOperationDecision));
        OnPropertyChanged(nameof(CanWaitForEditorOperation));
        OnPropertyChanged(nameof(CanCancelEditorOperationAndWait));
        OnPropertyChanged(nameof(ShowConfirmedAbandonmentForOpen));
        OnPropertyChanged(nameof(CanConfirmAbandonmentForOpen));
        OnPropertyChanged(nameof(ConfirmedAbandonmentExplanation));
        ReviewChangesRelayCommand.RaiseCanExecuteChanged();
        ShowSaveChangesDialogRelayCommand.RaiseCanExecuteChanged();
        ShowDiscardChangesDialogRelayCommand.RaiseCanExecuteChanged();
        SaveChangesRelayCommand.RaiseCanExecuteChanged();
        DiscardChangesRelayCommand.RaiseCanExecuteChanged();
        CancelActiveOperationRelayCommand.RaiseCanExecuteChanged();
        InspectSaveOutcomeRelayCommand.RaiseCanExecuteChanged();
        CompletePreparedSaveRelayCommand.RaiseCanExecuteChanged();
        RestorePreviousOutputRelayCommand.RaiseCanExecuteChanged();
        ResumeUnsavedChangesRelayCommand.RaiseCanExecuteChanged();
        ReopenResolvedOutputRelayCommand.RaiseCanExecuteChanged();
        RetryCommittedRefreshRelayCommand.RaiseCanExecuteChanged();
    }

    /// <summary>Refreshes editor-derived bound state after the participant or arbiter changes.</summary>
    /// <param name="sender">The changed dependency.</param>
    /// <param name="eventArgs">The changed property.</param>
    private void OnAdmissionDependencyPropertyChanged(object? sender, PropertyChangedEventArgs eventArgs)
    {
        _ = sender;
        _ = eventArgs;
        UiDispatcher.Post(() =>
        {
            if (!AreSubscriptionsDetached)
            {
                RaiseAllBoundState();
            }
        });
    }

    /// <summary>Invalidates retained presentation state when the root coordinator publishes a different workspace identity.</summary>
    /// <param name="sender">The workspace coordinator.</param>
    /// <param name="eventArgs">The changed coordinator property.</param>
    private void OnWorkspaceCoordinatorPropertyChanged(object? sender, PropertyChangedEventArgs eventArgs)
    {
        _ = sender;
        if (eventArgs.PropertyName is not null && eventArgs.PropertyName != nameof(INativeWorkspaceCoordinator.CurrentWorkspace))
        {
            return;
        }

        UiDispatcher.Post(() =>
        {
            if (AreSubscriptionsDetached || HasActiveOwnedTransition)
            {
                return;
            }

            CurrentReviewValue = null;
            IsReviewStaleValue = false;
            OutputSynchronizationStatusValue = null;
            RecoveryEnvelopeValue = null;
            PendingRepairEnvelopeValue = null;
            PendingRefreshEnvelopeValue = null;
            AbandonmentEnvelopeValue = null;
            LastSaveResultValue = null;
            StatusTextValue = WorkspaceCoordinator.CurrentWorkspace is null
                ? "No native workspace is open."
                : "Review changes to inspect the current workspace.";
            ErrorCodeValue = null;
            ErrorMessageValue = null;
            WarningsValue = Array.Empty<EngineWarning>();
            RaiseAllBoundState();
        });
    }

    /// <summary>Detaches all long-lived dependency subscriptions exactly once.</summary>
    private void DetachSubscriptions()
    {
        lock (LifecycleGate)
        {
            if (AreSubscriptionsDetached)
            {
                return;
            }

            AreSubscriptionsDetached = true;
        }

        EditParticipant.PropertyChanged -= OnAdmissionDependencyPropertyChanged;
        PresentationArbiter.PropertyChanged -= OnAdmissionDependencyPropertyChanged;
        WorkspaceCoordinator.PropertyChanged -= OnWorkspaceCoordinatorPropertyChanged;
    }
}
