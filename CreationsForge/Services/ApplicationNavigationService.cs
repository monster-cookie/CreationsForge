using Autofac;
using Avalonia.Controls;
using CreationsForge.Services.Interfaces;
using CreationsForge.Views;

namespace CreationsForge.Services;

/// <summary>Replaces the current plugin application view together with its safely drained Autofac lifetime scope.</summary>
public sealed class ApplicationNavigationService : IApplicationNavigationService, IAsyncDisposable
{
    /// <summary>The application container scope used to create isolated view lifetimes.</summary>
    private readonly ILifetimeScope RootScope;

    /// <summary>The registered application window content host.</summary>
    private readonly IApplicationWindowService WindowService;

    /// <summary>The application-wide workspace identity used to validate transferred reservations.</summary>
    private readonly IWorkspaceCoordinator WorkspaceCoordinator;

    /// <summary>The control currently hosted by the application window.</summary>
    private Control? CurrentView;

    /// <summary>The lifetime scope that owns the current view and view model.</summary>
    private ILifetimeScope? CurrentViewScope;

    /// <summary>The leave guard owned by the current plugin shell, or <see langword="null"/> while Settings is current.</summary>
    private IWorkspaceLeaveGuard? CurrentLeaveGuard;

    /// <summary>Initializes plugin application navigation.</summary>
    /// <param name="rootScope">The root scope from which isolated view scopes are created.</param>
    /// <param name="windowService">The application window content host.</param>
    /// <param name="workspaceCoordinator">The application-wide workspace owner.</param>
    /// <exception cref="ArgumentNullException">Thrown when a required dependency is <see langword="null"/>.</exception>
    public ApplicationNavigationService(
        ILifetimeScope rootScope,
        IApplicationWindowService windowService,
        IWorkspaceCoordinator workspaceCoordinator)
    {
        ArgumentNullException.ThrowIfNull(rootScope);
        ArgumentNullException.ThrowIfNull(windowService);
        ArgumentNullException.ThrowIfNull(workspaceCoordinator);
        RootScope = rootScope;
        WindowService = windowService;
        WorkspaceCoordinator = workspaceCoordinator;
    }

    /// <inheritdoc />
    public void ShowWorkspaceShell()
    {
        if (CurrentLeaveGuard is not null)
        {
            throw new InvalidOperationException("The current plugin shell must be left through a guarded transition.");
        }

        var viewScope = RootScope.BeginLifetimeScope();
        WorkspaceShellView view;
        IWorkspaceLeaveGuard leaveGuard;
        try
        {
            view = viewScope.Resolve<WorkspaceShellView>();
            leaveGuard = viewScope.Resolve<IWorkspaceLeaveGuard>();
        }
        catch
        {
            viewScope.Dispose();
            throw;
        }

        SetCurrentView(view, viewScope, leaveGuard);
    }

    /// <inheritdoc />
    public async Task<bool> TryShowSettingsAsync(CancellationToken cancellationToken = default)
    {
        var leaveGuard = CurrentLeaveGuard;
        if (leaveGuard is null)
        {
            return true;
        }

        using var reservation = await leaveGuard.ReserveLeaveAsync(
            WorkspaceLeaveReason.ShowSettings,
            cancellationToken);
        if (reservation is null)
        {
            return false;
        }

        ValidateReservation(reservation, WorkspaceLeaveReason.ShowSettings, allowAbandonment: false);
        cancellationToken.ThrowIfCancellationRequested();
        var candidateScope = RootScope.BeginLifetimeScope();
        Control candidateView;
        try
        {
            candidateView = candidateScope.Resolve<SettingsView>();
        }
        catch
        {
            await candidateScope.DisposeAsync();
            throw;
        }

        var previousView = CurrentView;
        var previousScope = CurrentViewScope;
        var published = false;
        try
        {
            ValidateReservation(reservation, WorkspaceLeaveReason.ShowSettings, allowAbandonment: false);
            WindowService.SetContent(candidateView);
            CurrentView = candidateView;
            CurrentViewScope = candidateScope;
            CurrentLeaveGuard = null;
            published = true;
            await ReleasePreviousViewAsync(previousView, previousScope);

            return true;
        }
        catch when (!published)
        {
            await candidateScope.DisposeAsync();
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<ApplicationShutdownLease?> ReserveShutdownAsync(CancellationToken cancellationToken = default)
    {
        var leaveGuard = CurrentLeaveGuard;
        if (leaveGuard is null)
        {
            return ApplicationShutdownLease.CreateForSettings();
        }

        var reservation = await leaveGuard.ReserveLeaveAsync(
            WorkspaceLeaveReason.ExitApplication,
            cancellationToken);
        if (reservation is null)
        {
            return null;
        }

        try
        {
            ValidateReservation(reservation, WorkspaceLeaveReason.ExitApplication, allowAbandonment: false);
            return new ApplicationShutdownLease(reservation);
        }
        catch
        {
            reservation.Dispose();
            throw;
        }
    }

    /// <summary>Asynchronously drains and releases the current view scope.</summary>
    /// <returns>A task that completes after the current scope has released every asynchronous presentation owner.</returns>
    public async ValueTask DisposeAsync()
    {
        var currentView = CurrentView;
        CurrentView = null;
        var currentViewScope = CurrentViewScope;
        CurrentViewScope = null;
        CurrentLeaveGuard = null;
        if (currentView is not null)
        {
            WindowService.ClearContent(currentView);
        }

        if (currentViewScope is not null)
        {
            await currentViewScope.DisposeAsync();
        }
    }

    /// <summary>Publishes a resolved view and synchronously releases the previous Settings-only scope.</summary>
    /// <param name="view">The newly resolved view.</param>
    /// <param name="viewScope">The scope that owns <paramref name="view"/>.</param>
    /// <param name="leaveGuard">The leave guard owned by the new plugin shell.</param>
    private void SetCurrentView(Control view, ILifetimeScope viewScope, IWorkspaceLeaveGuard leaveGuard)
    {
        ArgumentNullException.ThrowIfNull(view);
        ArgumentNullException.ThrowIfNull(viewScope);
        ArgumentNullException.ThrowIfNull(leaveGuard);
        var previousView = CurrentView;
        var previousScope = CurrentViewScope;
        var published = false;
        try
        {
            WindowService.SetContent(view);
            CurrentView = view;
            CurrentViewScope = viewScope;
            CurrentLeaveGuard = leaveGuard;
            published = true;
            ReleasePreviousView(previousView, previousScope);
        }
        catch when (!published)
        {
            viewScope.Dispose();
            throw;
        }
    }

    /// <summary>Removes a superseded view and synchronously releases its Settings-only scope.</summary>
    /// <param name="previousView">The superseded root control, or <see langword="null"/>.</param>
    /// <param name="previousScope">The superseded Settings scope, or <see langword="null"/>.</param>
    private void ReleasePreviousView(Control? previousView, ILifetimeScope? previousScope)
    {
        try
        {
            if (previousView is not null)
            {
                WindowService.ClearContent(previousView);
            }
        }
        finally
        {
            previousScope?.Dispose();
        }
    }

    /// <summary>Removes a superseded view and asynchronously drains and releases its shell scope.</summary>
    /// <param name="previousView">The superseded root control, or <see langword="null"/>.</param>
    /// <param name="previousScope">The superseded shell scope, or <see langword="null"/>.</param>
    /// <returns>A task that completes after both removal and scope disposal have been attempted.</returns>
    private async Task ReleasePreviousViewAsync(Control? previousView, ILifetimeScope? previousScope)
    {
        try
        {
            if (previousView is not null)
            {
                WindowService.ClearContent(previousView);
            }
        }
        finally
        {
            if (previousScope is not null)
            {
                await previousScope.DisposeAsync();
            }
        }
    }

    /// <summary>Rejects a reservation that cannot authorize the caller's exact final action.</summary>
    /// <param name="reservation">The transferred reservation to validate.</param>
    /// <param name="expectedReason">The exact final action being attempted.</param>
    /// <param name="allowAbandonment">Whether the caller is the Open flow that may accept confirmed abandonment.</param>
    /// <exception cref="InvalidOperationException">Thrown when the reservation is inactive, incompatible, or stale.</exception>
    private void ValidateReservation(
        WorkspaceLeaveReservation reservation,
        WorkspaceLeaveReason expectedReason,
        bool allowAbandonment)
    {
        if (!reservation.IsActive || reservation.Reason != expectedReason)
        {
            throw new InvalidOperationException("The leave reservation does not authorize this application action.");
        }

        if (reservation.Disposition == WorkspaceLeaveDisposition.ConfirmedAbandonmentForOpen && !allowAbandonment)
        {
            throw new InvalidOperationException("Confirmed abandonment authorizes only workspace replacement.");
        }

        var currentWorkspaceId = WorkspaceCoordinator.CurrentWorkspace?.WorkspaceId;
        if (reservation.ExpectedWorkspaceId != currentWorkspaceId)
        {
            throw new InvalidOperationException("The active workspace changed after leave admission.");
        }
    }
}
