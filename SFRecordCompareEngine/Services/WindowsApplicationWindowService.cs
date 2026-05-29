using Microsoft.UI.Windowing;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SFRecordCompareEngine.Core.Models.Configuration;
using SFRecordCompareEngine.Services.Interfaces;
using SFRecordCompareEngine.ViewModels;
using WinRT.Interop;

namespace SFRecordCompareEngine.Services;

public class WindowsApplicationWindowService : IApplicationWindowService
{
    private MainWindow? MainWindow;

    public void RegisterMainWindow(MainWindow mainWindow)
    {
        MainWindow = mainWindow;
    }

    public void SetContent(UIElement content)
    {
        MainWindow?.SetContent(content);
    }

    public void ShowMainCommandSurface(MainPageViewModel viewModel)
    {
        MainWindow?.ShowMainCommandSurface(viewModel);
    }

    public void ApplyTheme(ApplicationThemeMode theme)
    {
        MainWindow?.ApplyTheme(theme);
    }

    public async Task ShowDialogAsync(ContentDialog dialog)
    {
        if (MainWindow == null) return;

        await MainWindow.ShowDialogAsync(dialog);
    }

    public void CloseDialog()
    {
        MainWindow?.CloseDialog();
    }

    public void MaximizeMainWindow()
    {
        if (MainWindow == null) return;

        var windowId = Win32Interop.GetWindowIdFromWindow(WindowNative.GetWindowHandle(MainWindow));
        var appWindow = AppWindow.GetFromWindowId(windowId);

        if (appWindow.Presenter is OverlappedPresenter presenter)
        {
            presenter.Maximize();
        }
    }

    public void Quit()
    {
        if (Application.Current is App app)
        {
            app.ShutDown();
        }
    }
}
