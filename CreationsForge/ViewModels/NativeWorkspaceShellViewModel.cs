using System.ComponentModel;
using System.Windows.Input;
using CreationsForge.Commands;
using CreationsForge.Services;
using CreationsForge.Services.Interfaces;

namespace CreationsForge.ViewModels;

/// <summary>Presents guarded application navigation and the active native workspace identity.</summary>
public sealed class NativeWorkspaceShellViewModel : ViewModelBase, INativeWorkspaceLeaveGuard, IDisposable, IAsyncDisposable
{
    /// <summary>Owns the application-wide native workspace.</summary>
    private readonly INativeWorkspaceCoordinator WorkspaceCoordinator;

    /// <summary>Creates a fresh complete source-and-output selection workflow.</summary>
    private readonly Func<NativeWorkspaceSelectionViewModel> SelectionViewModelFactory;

    /// <summary>Hosts workspace selection in an owner-bound modal dialog.</summary>
    private readonly INativeWorkspaceSelectionDialogService SelectionDialogService;

    /// <summary>Coordinates reviewed persistence and proves safe leave dispositions.</summary>
    private readonly NativeWorkspaceChangesViewModel ChangesViewModel;

    /// <summary>Navigates between the shell and application settings.</summary>
    private readonly INativeApplicationNavigationService NavigationService;

    /// <summary>The open command whose execution is guarded against concurrent shell transitions.</summary>
    private readonly AsyncRelayCommand OpenWorkspaceRelayCommand;

    /// <summary>The close command whose execution is guarded against concurrent shell transitions.</summary>
    private readonly AsyncRelayCommand CloseWorkspaceRelayCommand;

    /// <summary>The settings command whose execution is guarded against concurrent shell transitions.</summary>
    private readonly AsyncRelayCommand SettingsRelayCommand;

    /// <summary>Whether one shell-owned final action is using a transferred reservation.</summary>
    private int IsShellTransitionActiveValue;

    /// <summary>Tracks whether the coordinator subscription has been released.</summary>
    private int IsDisposedValue;

    /// <summary>Initializes the guarded native workspace shell.</summary>
    /// <param name="workspaceCoordinator">The application-wide native workspace owner.</param>
    /// <param name="selectionViewModelFactory">A factory that creates a fresh complete workspace selection workflow.</param>
    /// <param name="selectionDialogService">The owner-bound workspace selection dialog.</param>
    /// <param name="changesViewModel">The navigation-scope change lifecycle and leave guard.</param>
    /// <param name="navigationService">The native application navigation boundary.</param>
    /// <exception cref="ArgumentNullException">Thrown when a required dependency is <see langword="null"/>.</exception>
    public NativeWorkspaceShellViewModel(
        INativeWorkspaceCoordinator workspaceCoordinator,
        Func<NativeWorkspaceSelectionViewModel> selectionViewModelFactory,
        INativeWorkspaceSelectionDialogService selectionDialogService,
        NativeWorkspaceChangesViewModel changesViewModel,
        INativeApplicationNavigationService navigationService)
    {
        ArgumentNullException.ThrowIfNull(workspaceCoordinator);
        ArgumentNullException.ThrowIfNull(selectionViewModelFactory);
        ArgumentNullException.ThrowIfNull(selectionDialogService);
        ArgumentNullException.ThrowIfNull(changesViewModel);
        ArgumentNullException.ThrowIfNull(navigationService);
        WorkspaceCoordinator = workspaceCoordinator;
        SelectionViewModelFactory = selectionViewModelFactory;
        SelectionDialogService = selectionDialogService;
        ChangesViewModel = changesViewModel;
        NavigationService = navigationService;
        OpenWorkspaceRelayCommand = new AsyncRelayCommand(ExecuteOpenWorkspaceAsync, CanStartShellTransition);
        CloseWorkspaceRelayCommand = new AsyncRelayCommand(ExecuteCloseWorkspaceAsync, CanStartShellTransition);
        SettingsRelayCommand = new AsyncRelayCommand(ExecuteShowSettingsAsync, CanStartShellTransition);
        OpenWorkspaceCommand = OpenWorkspaceRelayCommand;
        CloseWorkspaceCommand = CloseWorkspaceRelayCommand;
        SettingsCommand = SettingsRelayCommand;
        WorkspaceCoordinator.PropertyChanged += OnWorkspaceCoordinatorPropertyChanged;
    }

    /// <summary>Gets the command that opens installed-plugin selection after guarded leave admission.</summary>
    public ICommand OpenWorkspaceCommand { get; }

    /// <summary>Gets the command that closes the active workspace, or safely repeats close when no workspace exists.</summary>
    public ICommand CloseWorkspaceCommand { get; }

    /// <summary>Gets the command that shows application settings after guarded leave admission.</summary>
    public ICommand SettingsCommand { get; }

    /// <summary>Gets the command that opens a fresh detached change review.</summary>
    public ICommand ReviewChangesCommand => ChangesViewModel.ReviewChangesCommand;

    /// <summary>Gets the command that opens the reviewed save workflow without directly mutating Core.</summary>
    public ICommand SaveChangesCommand => ChangesViewModel.ShowSaveChangesDialogCommand;

    /// <summary>Gets the command that opens the reviewed discard workflow without directly mutating Core.</summary>
    public ICommand DiscardChangesCommand => ChangesViewModel.ShowDiscardChangesDialogCommand;

    /// <summary>Gets whether a native workspace is currently active.</summary>
    public bool HasWorkspace => WorkspaceCoordinator.CurrentWorkspace is not null;

    /// <summary>Gets whether one shell-owned final transition is currently active.</summary>
    public bool IsShellTransitionActive => Volatile.Read(ref IsShellTransitionActiveValue) != 0;

    /// <summary>Gets a presentation-safe summary of the active source and output association.</summary>
    public string WorkspaceStatusText => CreateWorkspaceStatusText(WorkspaceCoordinator.CurrentWorkspace);

    /// <inheritdoc />
    public ValueTask<NativeWorkspaceLeaveReservation?> ReserveLeaveAsync(
        NativeWorkspaceLeaveReason reason,
        CancellationToken cancellationToken = default)
    {
        return ChangesViewModel.ReserveLeaveAsync(reason, cancellationToken);
    }

    /// <summary>Opens a fresh selection workflow while holding the exact transferred leave reservation.</summary>
    /// <param name="cancellationToken">A token that cancels leave admission before a reservation is transferred.</param>
    /// <returns><see langword="true"/> when the selection workflow opened a workspace; otherwise <see langword="false"/>.</returns>
    public async Task<bool> OpenWorkspaceAsync(CancellationToken cancellationToken = default)
    {
        if (!TryBeginShellTransition())
        {
            return false;
        }

        try
        {
            using var reservation = await ReserveLeaveAsync(
                NativeWorkspaceLeaveReason.OpenWorkspace,
                cancellationToken);
            if (reservation is null)
            {
                return false;
            }

            ValidateReservation(
                reservation,
                NativeWorkspaceLeaveReason.OpenWorkspace,
                allowConfirmedAbandonment: true);
            var selectionViewModel = SelectionViewModelFactory();
            return await SelectionDialogService.ShowAsync(selectionViewModel);
        }
        finally
        {
            EndShellTransition();
        }
    }

    /// <summary>Closes the active workspace, or completes an idempotent close when no workspace exists, under the transferred reservation.</summary>
    /// <param name="cancellationToken">A token that cancels leave admission before a reservation is transferred.</param>
    /// <returns><see langword="true"/> when the guarded close completed; otherwise <see langword="false"/>.</returns>
    public async Task<bool> CloseWorkspaceAsync(CancellationToken cancellationToken = default)
    {
        if (!TryBeginShellTransition())
        {
            return false;
        }

        try
        {
            using var reservation = await ReserveLeaveAsync(
                NativeWorkspaceLeaveReason.CloseWorkspace,
                cancellationToken);
            if (reservation is null)
            {
                return false;
            }

            ValidateReservation(
                reservation,
                NativeWorkspaceLeaveReason.CloseWorkspace,
                allowConfirmedAbandonment: false);
            await WorkspaceCoordinator.CloseAsync();
            return true;
        }
        finally
        {
            EndShellTransition();
        }
    }

    /// <summary>Shows Settings only after the navigation service obtains and consumes a compatible leave reservation.</summary>
    /// <param name="cancellationToken">A token that cancels guarded settings admission.</param>
    /// <returns><see langword="true"/> when Settings was published; otherwise <see langword="false"/>.</returns>
    public async Task<bool> ShowSettingsAsync(CancellationToken cancellationToken = default)
    {
        if (!TryBeginShellTransition())
        {
            return false;
        }

        try
        {
            return await NavigationService.TryShowSettingsAsync(cancellationToken);
        }
        finally
        {
            EndShellTransition();
        }
    }

    /// <summary>Detaches synchronous subscriptions without treating cancellation as proof that asynchronous work drained.</summary>
    public void Dispose()
    {
        DetachSubscriptions();
    }

    /// <summary>Drains Task 21 work before releasing the shell's subscriptions.</summary>
    /// <returns>A task that completes after active presentation work reaches a safe terminal result.</returns>
    public async ValueTask DisposeAsync()
    {
        await ChangesViewModel.ShutdownAndDrainAsync();
        DetachSubscriptions();
    }

    /// <summary>Runs the public guarded open method for command invocation.</summary>
    /// <returns>A task that completes when the open attempt ends.</returns>
    private async Task ExecuteOpenWorkspaceAsync()
    {
        await OpenWorkspaceAsync();
    }

    /// <summary>Runs the public guarded close method for command invocation.</summary>
    /// <returns>A task that completes when the close attempt ends.</returns>
    private async Task ExecuteCloseWorkspaceAsync()
    {
        await CloseWorkspaceAsync();
    }

    /// <summary>Runs the public guarded Settings method for command invocation.</summary>
    /// <returns>A task that completes when the settings attempt ends.</returns>
    private async Task ExecuteShowSettingsAsync()
    {
        await ShowSettingsAsync();
    }

    /// <summary>Attempts to admit one shell-owned final action.</summary>
    /// <returns><see langword="true"/> only for the caller that changed the shell from idle to active.</returns>
    private bool TryBeginShellTransition()
    {
        if (Interlocked.CompareExchange(ref IsShellTransitionActiveValue, 1, 0) != 0)
        {
            return false;
        }

        RaiseShellCommandStateChanged();
        return true;
    }

    /// <summary>Returns shell transition commands to their idle state.</summary>
    private void EndShellTransition()
    {
        Interlocked.Exchange(ref IsShellTransitionActiveValue, 0);
        RaiseShellCommandStateChanged();
    }

    /// <summary>Gets whether another shell-owned final action may start.</summary>
    /// <returns><see langword="true"/> while the shell is idle and still active.</returns>
    private bool CanStartShellTransition()
    {
        return Volatile.Read(ref IsDisposedValue) == 0 && !IsShellTransitionActive;
    }

    /// <summary>Publishes shell command admission changes.</summary>
    private void RaiseShellCommandStateChanged()
    {
        OnPropertyChanged(nameof(IsShellTransitionActive));
        OpenWorkspaceRelayCommand.RaiseCanExecuteChanged();
        CloseWorkspaceRelayCommand.RaiseCanExecuteChanged();
        SettingsRelayCommand.RaiseCanExecuteChanged();
    }

    /// <summary>Rejects a reservation that cannot authorize the requested shell action.</summary>
    /// <param name="reservation">The transferred reservation.</param>
    /// <param name="expectedReason">The exact action being attempted.</param>
    /// <param name="allowConfirmedAbandonment">Whether the action is workspace replacement and may accept explicit abandonment.</param>
    /// <exception cref="InvalidOperationException">Thrown when the reservation is inactive, incompatible, or stale.</exception>
    private void ValidateReservation(
        NativeWorkspaceLeaveReservation reservation,
        NativeWorkspaceLeaveReason expectedReason,
        bool allowConfirmedAbandonment)
    {
        if (!reservation.IsActive || reservation.Reason != expectedReason)
        {
            throw new InvalidOperationException("The leave reservation does not authorize this shell action.");
        }

        if (reservation.Disposition == NativeWorkspaceLeaveDisposition.ConfirmedAbandonmentForOpen &&
            !allowConfirmedAbandonment)
        {
            throw new InvalidOperationException("Confirmed abandonment authorizes only workspace replacement.");
        }

        if (reservation.ExpectedWorkspaceId != WorkspaceCoordinator.CurrentWorkspace?.WorkspaceId)
        {
            throw new InvalidOperationException("The active native workspace changed after leave admission.");
        }
    }

    /// <summary>Refreshes bound shell state when the coordinator publishes a workspace replacement or close.</summary>
    /// <param name="sender">The workspace coordinator.</param>
    /// <param name="eventArgs">The changed coordinator property.</param>
    private void OnWorkspaceCoordinatorPropertyChanged(object? sender, PropertyChangedEventArgs eventArgs)
    {
        if (eventArgs.PropertyName is not null &&
            eventArgs.PropertyName != nameof(INativeWorkspaceCoordinator.CurrentWorkspace))
        {
            return;
        }

        OnPropertyChanged(nameof(HasWorkspace));
        OnPropertyChanged(nameof(WorkspaceStatusText));
    }

    /// <summary>Releases the workspace status subscription exactly once.</summary>
    private void DetachSubscriptions()
    {
        if (Interlocked.Exchange(ref IsDisposedValue, 1) != 0)
        {
            return;
        }

        WorkspaceCoordinator.PropertyChanged -= OnWorkspaceCoordinatorPropertyChanged;
        RaiseShellCommandStateChanged();
    }

    /// <summary>Creates shell status text from immutable coordinator state.</summary>
    /// <param name="workspace">The active workspace descriptor, or <see langword="null"/>.</param>
    /// <returns>A source-and-output summary, or the empty-workspace state.</returns>
    private static string CreateWorkspaceStatusText(NativeWorkspaceDescriptor? workspace)
    {
        if (workspace is null)
        {
            return "No plugin is open.";
        }

        return workspace.Output is null
            ? $"{workspace.Game}: {Path.GetFileName(workspace.SourcePluginPath)} (Read-Only)"
            : $"{workspace.Game}: {workspace.Output.ModKey.FileName} (Editing)";
    }
}
