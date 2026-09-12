using Avalonia.Controls;
using CreationsForge.Services.Interfaces;
using CreationsForge.ViewModels;
using CreationsForge.Views;
using Serilog;

namespace CreationsForge.Services;

/// <summary>Creates owner-bound native reference picker dialogs with cancellation-safe per-invocation state.</summary>
public sealed class NativeReferencePickerService : INativeReferencePickerService
{
    /// <summary>Displays modal windows against the registered application owner.</summary>
    private readonly IApplicationWindowService WindowService;

    /// <summary>Provides borrowed access to the root-owned native workspace.</summary>
    private readonly INativeWorkspaceCoordinator WorkspaceCoordinator;

    /// <summary>Schedules dialog closure and bound state changes on the UI thread.</summary>
    private readonly IUiDispatcher UiDispatcher;

    /// <summary>Records unexpected modal lifecycle and cleanup failures.</summary>
    private readonly ILogger Logger;

    /// <summary>Initializes the root-lifetime native reference picker service.</summary>
    /// <param name="windowService">The application dialog owner and presenter.</param>
    /// <param name="workspaceCoordinator">The root-owned native workspace coordinator.</param>
    /// <param name="uiDispatcher">The dispatcher used for all dialog closure.</param>
    /// <param name="logger">The structured logger for unexpected lifecycle failures.</param>
    /// <exception cref="ArgumentNullException">Thrown when a required dependency is <see langword="null"/>.</exception>
    public NativeReferencePickerService(
        IApplicationWindowService windowService,
        INativeWorkspaceCoordinator workspaceCoordinator,
        IUiDispatcher uiDispatcher,
        ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(windowService);
        ArgumentNullException.ThrowIfNull(workspaceCoordinator);
        ArgumentNullException.ThrowIfNull(uiDispatcher);
        ArgumentNullException.ThrowIfNull(logger);
        WindowService = windowService;
        WorkspaceCoordinator = workspaceCoordinator;
        UiDispatcher = uiDispatcher;
        Logger = logger.ForContext<NativeReferencePickerService>();
    }

    /// <inheritdoc />
    public async Task<NativeReferencePickerSelection?> PickAsync(
        NativeReferencePickerRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();
        var viewModel = new NativeReferencePickerViewModel(
            WorkspaceCoordinator,
            UiDispatcher,
            request,
            Logger);
        var dialog = new Window
        {
            Title = "Select Native Reference",
            Width = 960,
            Height = 720,
            MinWidth = 720,
            MinHeight = 520,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };
        var closeLock = new object();
        var closeStarted = false;
        var allowClose = false;

        void CloseWithResult(NativeReferencePickerSelection selection)
        {
            lock (closeLock)
            {
                if (closeStarted)
                {
                    return;
                }

                closeStarted = true;
                allowClose = true;
            }

            dialog.Close(selection);
        }

        async Task CancelDrainAndCloseAsync()
        {
            lock (closeLock)
            {
                if (closeStarted)
                {
                    return;
                }

                closeStarted = true;
            }

            try
            {
                await viewModel.CancelAndDrainAsync().ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Unable to drain native reference picker work for workspace {WorkspaceId}.", request.WorkspaceId);
            }

            UiDispatcher.Post(() =>
            {
                lock (closeLock)
                {
                    allowClose = true;
                }

                dialog.Close((object?)null);
            });
        }

        dialog.Closing += (_, eventArgs) =>
        {
            lock (closeLock)
            {
                if (allowClose)
                {
                    return;
                }

                if (!viewModel.IsBusy)
                {
                    closeStarted = true;
                    allowClose = true;
                    return;
                }
            }

            eventArgs.Cancel = true;
            _ = CancelDrainAndCloseAsync();
        };
        dialog.Content = new NativeReferencePickerView(viewModel, selection =>
        {
            if (selection is null || cancellationToken.IsCancellationRequested)
            {
                _ = CancelDrainAndCloseAsync();
                return;
            }

            CloseWithResult(selection);
        });

        Task<NativeReferencePickerSelection?> showTask;
        try
        {
            showTask = WindowService.ShowDialogAsync<NativeReferencePickerSelection?>(dialog);
        }
        catch
        {
            await viewModel.DisposeAsync().ConfigureAwait(false);
            throw;
        }

        using var cancellationRegistration = cancellationToken.Register(() =>
            UiDispatcher.Post(() => _ = CancelDrainAndCloseAsync()));
        try
        {
            var selection = await showTask.ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();
            return selection;
        }
        finally
        {
            await viewModel.DisposeAsync().ConfigureAwait(false);
        }
    }
}
