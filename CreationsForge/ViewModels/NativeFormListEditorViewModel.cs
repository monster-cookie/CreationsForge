using System.ComponentModel;
using System.Windows.Input;
using CreationsForge.Commands;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeWire;
using CreationsForge.NativeEditing;
using CreationsForge.NativeEditing.Drafts;
using CreationsForge.Services;
using CreationsForge.Services.Interfaces;

namespace CreationsForge.ViewModels;

/// <summary>Coordinates revision-bound native FormList edit sessions without owning or retaining a native workspace.</summary>
public sealed partial class NativeFormListEditorViewModel : ViewModelBase, IDisposable
{
    /// <summary>The application-wide owner that lends the active native workspace for bounded operations.</summary>
    private readonly INativeWorkspaceCoordinator WorkspaceCoordinator;

    /// <summary>The navigation-scope admission boundary shared with workspace persistence and leave operations.</summary>
    private readonly INativeWorkspacePresentationOperationArbiter OperationArbiter;

    /// <summary>The browser-owned atomic selection and post-mutation refresh boundary.</summary>
    private readonly INativeFormListEditorHost Host;

    /// <summary>Resolves the exact schema and codec pair for the active game and release.</summary>
    private readonly INativeFormListWireCatalogResolver CatalogResolver;

    /// <summary>Captures detached seeds and creates typed drafts.</summary>
    private readonly INativeFormListDraftFactory DraftFactory;

    /// <summary>Validates request-local typed drafts.</summary>
    private readonly INativeFormListDraftValidator DraftValidator;

    /// <summary>Serializes validated drafts into detached transient command arguments.</summary>
    private readonly INativeFormListDraftSerializer DraftSerializer;

    /// <summary>Opens the existing bounded native reference picker.</summary>
    private readonly INativeReferencePickerService ReferencePickerService;

    /// <summary>Publishes bound state on the presentation thread.</summary>
    private readonly IUiDispatcher UiDispatcher;

    /// <summary>The resource policy used for schema traversal, drafts, serialization, and codec decode.</summary>
    private readonly NativeWireReadLimits ReadLimits;

    /// <summary>The command that begins a new FormList edit.</summary>
    private readonly AsyncRelayCommand NewRelayCommand;

    /// <summary>The command that overrides the exact selected non-output FormList context.</summary>
    private readonly AsyncRelayCommand OverrideRelayCommand;

    /// <summary>The command that edits the exact selected staged-output FormList.</summary>
    private readonly AsyncRelayCommand ExistingOutputRelayCommand;

    /// <summary>The command that applies the validated typed draft.</summary>
    private readonly AsyncRelayCommand ApplyRelayCommand;

    /// <summary>The command that explicitly discards only the current request-local form changes.</summary>
    private readonly RelayCommand DiscardFormChangesRelayCommand;

    /// <summary>The command that replays the exact immutable uncertain operation.</summary>
    private readonly AsyncRelayCommand RetryPendingRelayCommand;

    /// <summary>Cancels work tied to the current workspace generation.</summary>
    private CancellationTokenSource GenerationCancellation = new();

    /// <summary>Protects generation token replacement and disposal.</summary>
    private readonly object GenerationCancellationLock = new();

    /// <summary>The active workspace generation used to suppress stale continuation publication.</summary>
    private long WorkspaceGeneration;

    /// <summary>The active detached edit session.</summary>
    private NativeFormListEditorSession? SessionValue;

    /// <summary>The exact resolved catalog context bound to the active session.</summary>
    private NativeFormListWireCatalogContext? CatalogContextValue;

    /// <summary>The most recent exact receipt-bound staged record seed.</summary>
    private NativeFormListDraftSeed? SeedValue;

    /// <summary>The exact command entries available in the active catalog.</summary>
    private IReadOnlyList<NativeFormListCommandPresentation> AvailableCommandsValue = Array.Empty<NativeFormListCommandPresentation>();

    /// <summary>The selected command entry.</summary>
    private NativeFormListCommandPresentation? SelectedCommandValue;

    /// <summary>The selected closed seed intent used to create the current draft.</summary>
    private NativeFormListDraftSeedSelection DraftSelectionValue = NativeFormListDraftSeedSelection.CurrentValue();

    /// <summary>The current typed command draft.</summary>
    private NativeFormListDraft? DraftValue;

    /// <summary>The latest immutable validation issues.</summary>
    private IReadOnlyList<NativeWireDraftIssue> ValidationIssuesValue = Array.Empty<NativeWireDraftIssue>();

    /// <summary>The current single-flight operation state.</summary>
    private NativeFormListEditorOperationState OperationStateValue;

    /// <summary>The immutable operation retained only when its mutation outcome is uncertain.</summary>
    private NativeFormListEditorPendingOperation? PendingOperationValue;

    /// <summary>Whether preview established the staged-dirty state for this session.</summary>
    private bool IsStagedChangesKnownValue;

    /// <summary>Whether the current session's FormList has staged semantic changes.</summary>
    private bool HasStagedChangesValue;

    /// <summary>The stable typed error category for the current editor failure.</summary>
    private EngineErrorCode? ErrorCodeValue;

    /// <summary>The complete current editor failure message.</summary>
    private string? ErrorMessageValue;

    /// <summary>The current editor workflow status.</summary>
    private string StatusTextValue = "Begin a new edit or select an exact FormList context.";

    /// <summary>Non-fatal engine warnings from the most recent successful operation.</summary>
    private IReadOnlyList<EngineWarning> WarningsValue = Array.Empty<EngineWarning>();

    /// <summary>Whether the browser-owned editor has been disposed.</summary>
    private bool IsDisposed;

    /// <summary>Initializes one editor bound to the browser and application workspace coordinator.</summary>
    /// <param name="workspaceCoordinator">The application-wide native workspace owner.</param>
    /// <param name="host">The browser-owned exact selection and refresh boundary.</param>
    /// <param name="operationArbiter">The navigation-scope editor and workspace-transition admission boundary.</param>
    /// <param name="catalogResolver">The exact game and release catalog resolver.</param>
    /// <param name="draftFactory">The typed draft and seed factory.</param>
    /// <param name="draftValidator">The typed draft validator.</param>
    /// <param name="draftSerializer">The detached command-argument serializer.</param>
    /// <param name="referencePickerService">The bounded native reference picker.</param>
    /// <param name="uiDispatcher">The presentation dispatcher.</param>
    /// <param name="readLimits">Optional immutable resource limits; defaults to the shared native wire limits.</param>
    /// <exception cref="ArgumentNullException">Thrown when a required dependency is <see langword="null"/>.</exception>
    public NativeFormListEditorViewModel(
        INativeWorkspaceCoordinator workspaceCoordinator,
        INativeFormListEditorHost host,
        INativeWorkspacePresentationOperationArbiter operationArbiter,
        INativeFormListWireCatalogResolver catalogResolver,
        INativeFormListDraftFactory draftFactory,
        INativeFormListDraftValidator draftValidator,
        INativeFormListDraftSerializer draftSerializer,
        INativeReferencePickerService referencePickerService,
        IUiDispatcher uiDispatcher,
        NativeWireReadLimits? readLimits = null)
    {
        ArgumentNullException.ThrowIfNull(workspaceCoordinator);
        ArgumentNullException.ThrowIfNull(host);
        ArgumentNullException.ThrowIfNull(operationArbiter);
        ArgumentNullException.ThrowIfNull(catalogResolver);
        ArgumentNullException.ThrowIfNull(draftFactory);
        ArgumentNullException.ThrowIfNull(draftValidator);
        ArgumentNullException.ThrowIfNull(draftSerializer);
        ArgumentNullException.ThrowIfNull(referencePickerService);
        ArgumentNullException.ThrowIfNull(uiDispatcher);
        WorkspaceCoordinator = workspaceCoordinator;
        Host = host;
        OperationArbiter = operationArbiter;
        CatalogResolver = catalogResolver;
        DraftFactory = draftFactory;
        DraftValidator = draftValidator;
        DraftSerializer = draftSerializer;
        ReferencePickerService = referencePickerService;
        UiDispatcher = uiDispatcher;
        ReadLimits = readLimits ?? NativeWireReadLimits.Default;
        NewRelayCommand = new AsyncRelayCommand(BeginNewAsync, () => CanBeginNew);
        OverrideRelayCommand = new AsyncRelayCommand(BeginOverrideAsync, () => CanBeginOverride);
        ExistingOutputRelayCommand = new AsyncRelayCommand(BeginExistingOutputAsync, () => CanBeginExistingOutput);
        ApplyRelayCommand = new AsyncRelayCommand(ApplyAsync, () => CanApply);
        DiscardFormChangesRelayCommand = new RelayCommand(DiscardFormChanges, () => CanDiscardFormChanges);
        RetryPendingRelayCommand = new AsyncRelayCommand(
            RetryPendingOperationAsync,
            () => HasPendingOperation && !IsBusy && IsEditorAdmissionOpen);
        NewCommand = NewRelayCommand;
        OverrideCommand = OverrideRelayCommand;
        ExistingOutputCommand = ExistingOutputRelayCommand;
        ApplyCommand = ApplyRelayCommand;
        DiscardFormChangesCommand = DiscardFormChangesRelayCommand;
        RetryPendingOperationCommand = RetryPendingRelayCommand;
        WorkspaceCoordinator.PropertyChanged += OnWorkspaceCoordinatorPropertyChanged;
        Host.PropertyChanged += OnHostPropertyChanged;
        OperationArbiter.PropertyChanged += OnOperationArbiterPropertyChanged;
        if (WorkspaceCoordinator.CurrentWorkspace is not null)
        {
            StatusTextValue = "Begin a new edit or select an exact FormList context.";
        }
    }

    /// <summary>Gets the active detached native edit session.</summary>
    public NativeFormListEditorSession? Session => SessionValue;

    /// <summary>Gets whether an edit session is active for the current workspace generation.</summary>
    public bool IsSessionActive => SessionValue is not null;

    /// <summary>Gets every exact command available in the active game and release catalog.</summary>
    public IReadOnlyList<NativeFormListCommandPresentation> AvailableCommands => AvailableCommandsValue;

    /// <summary>Gets or sets the command whose typed argument form is displayed.</summary>
    public NativeFormListCommandPresentation? SelectedCommand
    {
        get => SelectedCommandValue;
        set
        {
            if (value is null || ReferenceEquals(value, SelectedCommandValue) || !CanMutateDraft)
            {
                return;
            }

            _ = CreateDraft(value, NativeFormListDraftSeedSelection.CurrentValue());
        }
    }

    /// <summary>Gets the current typed command draft.</summary>
    public NativeFormListDraft? Draft => DraftValue;

    /// <summary>Gets the root typed draft node consumed by the visual control factory.</summary>
    public NativeWireDraftNode? DraftRoot => DraftValue?.Root;

    /// <summary>Gets the immutable issues from the latest draft validation or serialization pass.</summary>
    public IReadOnlyList<NativeWireDraftIssue> ValidationIssues => ValidationIssuesValue;

    /// <summary>Gets whether the current typed draft has local input changes.</summary>
    public bool HasDraftChanges => DraftValue?.HasChanges == true;

    /// <summary>Gets whether preview established the current session's staged-dirty state.</summary>
    public bool IsStagedChangesKnown => IsStagedChangesKnownValue;

    /// <summary>Gets whether the current session's FormList has staged semantic changes.</summary>
    public bool HasStagedChanges => HasStagedChangesValue;

    /// <summary>Gets whether current draft validation permits serialization and Apply.</summary>
    public bool IsValid => DraftValue is not null && ValidationIssuesValue.Count == 0;

    /// <summary>Gets the current editor operation state.</summary>
    public NativeFormListEditorOperationState OperationState => OperationStateValue;

    /// <summary>Gets whether a begin, Apply, retry, or reference-picker operation is running.</summary>
    public bool IsBusy => OperationStateValue is NativeFormListEditorOperationState.Beginning
        or NativeFormListEditorOperationState.Applying
        or NativeFormListEditorOperationState.PickingReference
        or NativeFormListEditorOperationState.RetryingPendingOperation;

    /// <summary>Gets whether an uncertain mutation is retained for exact replay.</summary>
    public bool HasPendingOperation => PendingOperationValue is not null;

    /// <summary>Gets whether a current typed editor error is visible.</summary>
    public bool HasError => ErrorCodeValue.HasValue || ErrorMessageValue is not null;

    /// <summary>Gets the stable typed editor error code, when one is available.</summary>
    public EngineErrorCode? ErrorCode => ErrorCodeValue;

    /// <summary>Gets the complete current editor failure message.</summary>
    public string? ErrorMessage => ErrorMessageValue;

    /// <summary>Gets the current editor workflow status.</summary>
    public string StatusText => StatusTextValue;

    /// <summary>Gets non-fatal engine warnings from the most recent successful operation.</summary>
    public IReadOnlyList<EngineWarning> Warnings => WarningsValue;

    /// <summary>Gets a concise active session identity for display.</summary>
    public string SessionIdentityText => SessionValue is null
        ? "No active edit session."
        : $"{SessionValue.Role}: {SessionValue.FormKey}";

    /// <summary>Gets the exact revision that will be forwarded by the next Apply.</summary>
    public string SessionRevisionText => SessionValue?.ExpectedRevision.ToString() ?? "No session revision.";

    /// <summary>Gets whether the current workspace permits beginning a new FormList edit.</summary>
    public bool CanBeginNew => CanBegin && WorkspaceCoordinator.CurrentWorkspace is not null;

    /// <summary>Gets whether the exact non-output browser selection permits beginning an override.</summary>
    public bool CanBeginOverride => CanBegin && Host.Selection is { IsStagedOutput: false, ExactReferenceRequest: not null };

    /// <summary>Gets whether the exact staged-output browser selection permits editing that record.</summary>
    public bool CanBeginExistingOutput => CanBegin && Host.Selection is { IsStagedOutput: true };

    /// <summary>Gets whether draft and lifecycle state permit one Apply operation.</summary>
    public bool CanApply => !IsDisposed && IsSessionActive && DraftValue is not null && IsValid && !IsBusy && !HasPendingOperation && IsEditorAdmissionOpen;

    /// <summary>Gets whether views may mutate typed draft nodes and command selection.</summary>
    public bool CanMutateDraft => !IsDisposed && IsSessionActive && !IsBusy && !HasPendingOperation && IsEditorAdmissionOpen;

    /// <summary>Gets whether the current changed request-local draft may be explicitly discarded without affecting staged native work.</summary>
    public bool CanDiscardFormChanges => !IsDisposed && IsSessionActive && HasDraftChanges && !IsBusy && !HasPendingOperation && IsEditorAdmissionOpen;

    /// <summary>Gets the command that begins a new FormList edit.</summary>
    public ICommand NewCommand { get; }

    /// <summary>Gets the command that overrides the exact selected non-output FormList context.</summary>
    public ICommand OverrideCommand { get; }

    /// <summary>Gets the command that edits the exact selected staged-output FormList.</summary>
    public ICommand ExistingOutputCommand { get; }

    /// <summary>Gets the command that applies the validated typed draft.</summary>
    public ICommand ApplyCommand { get; }

    /// <summary>Gets the command that explicitly discards only current request-local form changes while retaining the native edit session and staged work.</summary>
    public ICommand DiscardFormChangesCommand { get; }

    /// <summary>Gets the command that replays the retained exact uncertain operation.</summary>
    public ICommand RetryPendingOperationCommand { get; }

    /// <summary>Releases subscriptions and cancels this browser-owned editor generation.</summary>
    public void Dispose()
    {
        if (IsDisposed)
        {
            return;
        }

        IsDisposed = true;
        WorkspaceCoordinator.PropertyChanged -= OnWorkspaceCoordinatorPropertyChanged;
        Host.PropertyChanged -= OnHostPropertyChanged;
        OperationArbiter.PropertyChanged -= OnOperationArbiterPropertyChanged;
        CancelGeneration();
        DetachDraft();
    }

    /// <summary>Gets whether ordinary begin actions are currently permitted without discarding a locally changed draft.</summary>
    private bool CanBegin => !IsDisposed && !IsBusy && !HasPendingOperation && !HasDraftChanges && IsEditorAdmissionOpen;

    /// <summary>Gets whether no workspace transition is waiting for or owns presentation admission.</summary>
    private bool IsEditorAdmissionOpen => !OperationArbiter.IsWorkspaceTransitionPendingOrReserved;
}
