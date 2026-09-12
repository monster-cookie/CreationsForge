using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Threading;
using CreationsForge.Services.Interfaces;
using CreationsForge.ViewModels;
using CreationsForge.Views;
using Serilog;

namespace CreationsForge.Services;

/// <summary>Hosts native change review and guarded leave decisions in one owner-bound modal window.</summary>
public sealed class NativeWorkspaceChangesDialogService : INativeWorkspaceChangesDialogService
{
    /// <summary>Displays modal windows with the registered application owner.</summary>
    private readonly IApplicationWindowService WindowService;

    /// <summary>Records unexpected dialog callback failures without converting them into leave permission.</summary>
    private readonly ILogger Logger;

    /// <summary>Initializes the modal native workspace changes service.</summary>
    /// <param name="windowService">The application window owner and dialog presenter.</param>
    /// <param name="logger">The structured application logger.</param>
    /// <exception cref="ArgumentNullException">Thrown when a required dependency is <see langword="null"/>.</exception>
    public NativeWorkspaceChangesDialogService(
        IApplicationWindowService windowService,
        ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(windowService);
        ArgumentNullException.ThrowIfNull(logger);
        WindowService = windowService;
        Logger = logger.ForContext<NativeWorkspaceChangesDialogService>();
    }

    /// <inheritdoc />
    public Task<NativeWorkspaceChangesDialogResult> ShowAsync(
        NativeWorkspaceChangesViewModel viewModel,
        NativeWorkspaceChangesDialogRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        if (Dispatcher.UIThread.CheckAccess())
        {
            return ShowOnUiThreadAsync(viewModel, request, cancellationToken);
        }

        return Dispatcher.UIThread.InvokeAsync(
            () => ShowOnUiThreadAsync(viewModel, request, cancellationToken));
    }

    /// <summary>Owns the complete modal lifetime on Avalonia's UI thread.</summary>
    /// <param name="viewModel">The active change lifecycle displayed by the modal.</param>
    /// <param name="request">The immutable dialog purpose and optional leave reason.</param>
    /// <param name="cancellationToken">A token that requests safe cancellation and asynchronous drain.</param>
    /// <returns>The user's terminal modal decision after every active callback has drained.</returns>
    private async Task<NativeWorkspaceChangesDialogResult> ShowOnUiThreadAsync(
        NativeWorkspaceChangesViewModel viewModel,
        NativeWorkspaceChangesDialogRequest request,
        CancellationToken cancellationToken)
    {
        Dispatcher.UIThread.VerifyAccess();

        var dialog = CreateDialog();
        var allowClose = false;
        var closeState = 0;
        var callbackActive = false;
        const int NoCloseRequest = 0;
        const int CloseQueued = 1;
        const int DialogClosing = 2;

        void CloseDialog(NativeWorkspaceChangesDialogResult result, int expectedCloseState)
        {
            Dispatcher.UIThread.VerifyAccess();
            if (Interlocked.CompareExchange(
                    ref closeState,
                    DialogClosing,
                    expectedCloseState) != expectedCloseState)
            {
                return;
            }

            allowClose = true;
            dialog.Close(result);
        }

        void ApplyOutcome(NativeWorkspaceLeaveChoiceOutcome outcome, int expectedCloseState)
        {
            Dispatcher.UIThread.VerifyAccess();
            switch (outcome)
            {
                case NativeWorkspaceLeaveChoiceOutcome.ContinueDialog:
                    return;
                case NativeWorkspaceLeaveChoiceOutcome.KeepEditing:
                    CloseDialog(NativeWorkspaceChangesDialogResult.KeepEditing, expectedCloseState);
                    return;
                case NativeWorkspaceLeaveChoiceOutcome.Proceed:
                    CloseDialog(NativeWorkspaceChangesDialogResult.Proceed, expectedCloseState);
                    return;
                default:
                    throw new InvalidOperationException("The native change lifecycle returned an unknown dialog outcome.");
            }
        }

        void QueueCloseRequest()
        {
            if (Interlocked.CompareExchange(ref closeState, CloseQueued, NoCloseRequest) != NoCloseRequest)
            {
                return;
            }

            Dispatcher.UIThread.Post(() => _ = RequestCloseAsync());
        }

        async Task ApplyChoiceAsync(NativeWorkspaceChangesDialogChoice choice)
        {
            Dispatcher.UIThread.VerifyAccess();
            if (callbackActive || Volatile.Read(ref closeState) == DialogClosing)
            {
                return;
            }

            if (Volatile.Read(ref closeState) == CloseQueued)
            {
                await RequestCloseAsync();
                return;
            }

            callbackActive = true;
            if (Volatile.Read(ref closeState) == CloseQueued)
            {
                callbackActive = false;
                await RequestCloseAsync();
                return;
            }

            var outcome = NativeWorkspaceLeaveChoiceOutcome.ContinueDialog;
            try
            {
                outcome = await viewModel.ApplyLeaveChoiceAsync(choice);
                Dispatcher.UIThread.VerifyAccess();
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "A native workspace changes dialog choice failed.");
            }
            finally
            {
                Dispatcher.UIThread.VerifyAccess();
                callbackActive = false;
            }

            if (Volatile.Read(ref closeState) == CloseQueued)
            {
                await RequestCloseAsync();
                return;
            }

            ApplyOutcome(outcome, NoCloseRequest);
            if (Volatile.Read(ref closeState) == CloseQueued)
            {
                await RequestCloseAsync();
            }
        }

        async Task RequestCloseAsync()
        {
            Dispatcher.UIThread.VerifyAccess();
            if (Volatile.Read(ref closeState) != CloseQueued)
            {
                return;
            }

            if (callbackActive)
            {
                return;
            }

            callbackActive = true;
            var outcome = NativeWorkspaceLeaveChoiceOutcome.ContinueDialog;
            try
            {
                if (!viewModel.CanDismissDialog)
                {
                    await viewModel.CancelActiveOperationAndDrainAsync();
                    Dispatcher.UIThread.VerifyAccess();
                }

                if (viewModel.CanDismissDialog)
                {
                    outcome = await viewModel.ApplyLeaveChoiceAsync(NativeWorkspaceChangesDialogChoice.KeepEditing);
                    Dispatcher.UIThread.VerifyAccess();
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "The native workspace changes dialog could not close safely.");
            }
            finally
            {
                Dispatcher.UIThread.VerifyAccess();
                callbackActive = false;
            }

            ApplyOutcome(outcome, CloseQueued);
            if (Volatile.Read(ref closeState) == CloseQueued)
            {
                Interlocked.CompareExchange(ref closeState, NoCloseRequest, CloseQueued);
            }
        }

        dialog.Closing += (_, eventArgs) =>
        {
            Dispatcher.UIThread.VerifyAccess();
            if (allowClose)
            {
                return;
            }

            eventArgs.Cancel = true;
            QueueCloseRequest();
        };
        dialog.Content = new NativeWorkspaceChangesView(viewModel, request, ApplyChoiceAsync);
        using var cancellationRegistration = cancellationToken.Register(QueueCloseRequest);
        return await WindowService.ShowDialogAsync<NativeWorkspaceChangesDialogResult>(dialog);
    }

    /// <summary>Creates the fixed owner-bound modal window used by every closed dialog purpose.</summary>
    /// <returns>The configured changes window.</returns>
    private static Window CreateDialog()
    {
        var dialog = new Window
        {
            Title = "Review Changes",
            Width = 1120,
            Height = 860,
            MinWidth = 820,
            MinHeight = 620,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };
        AutomationProperties.SetAutomationId(dialog, "NativeWorkspaceChangesDialog");
        AutomationProperties.SetName(dialog, "Review Changes");
        return dialog;
    }
}
