using Avalonia.Controls;
using CreationsForge.Services.Interfaces;
using CreationsForge.ViewModels;
using CreationsForge.Views;

namespace CreationsForge.Services;

/// <summary>
/// Hosts native workspace selection in a modal window owned by the application root window.
/// </summary>
public sealed class NativeWorkspaceSelectionDialogService : INativeWorkspaceSelectionDialogService
{
    /// <summary>Displays modal windows with the registered application owner.</summary>
    private readonly IApplicationWindowService WindowService;

    /// <summary>Initializes the modal native workspace selection service.</summary>
    /// <param name="windowService">The application window owner and dialog presenter.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="windowService"/> is <see langword="null"/>.</exception>
    public NativeWorkspaceSelectionDialogService(IApplicationWindowService windowService)
    {
        ArgumentNullException.ThrowIfNull(windowService);
        WindowService = windowService;
    }

    /// <inheritdoc />
    public Task<bool> ShowAsync(NativeWorkspaceSelectionViewModel viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);
        var dialog = new Window
        {
            Title = "Open Plugin",
            Width = 1040,
            Height = 760,
            MinWidth = 760,
            MinHeight = 620,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };
        var allowClose = false;
        var closeRequested = false;
        void CloseDialog(bool result)
        {
            if (closeRequested)
            {
                return;
            }

            closeRequested = true;
            allowClose = true;
            dialog.Close(result);
        }

        dialog.Closing += async (_, eventArgs) =>
        {
            if (allowClose || !viewModel.IsBusy)
            {
                return;
            }

            eventArgs.Cancel = true;
            var activated = await viewModel.CancelAndWaitForOpenAsync();
            CloseDialog(activated);
        };
        dialog.Content = new NativeWorkspaceSelectionView(viewModel, CloseDialog);
        return WindowService.ShowDialogAsync<bool>(dialog);
    }
}
