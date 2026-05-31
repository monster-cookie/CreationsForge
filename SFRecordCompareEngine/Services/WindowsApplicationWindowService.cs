using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SFRecordCompareEngine.Core.Models.Configuration;
using SFRecordCompareEngine.Services.Interfaces;
using SFRecordCompareEngine.ViewModels;
using WinRT.Interop;

namespace SFRecordCompareEngine.Services;

public class WindowsApplicationWindowService : IApplicationWindowService
{
    private static readonly string ApplicationIconPath = Path.Combine(AppContext.BaseDirectory, "Resources", "AppIcon", "sfrecordcompareengine.ico");

    private MainWindow? MainWindow;

    public void RegisterMainWindow(MainWindow mainWindow)
    {
        MainWindow = mainWindow;

        var appWindow = GetAppWindow(mainWindow);
        appWindow.SetIcon(ApplicationIconPath);
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

        var appWindow = GetAppWindow(MainWindow);

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

    private static AppWindow GetAppWindow(MainWindow mainWindow)
    {
        var windowId = Win32Interop.GetWindowIdFromWindow(WindowNative.GetWindowHandle(mainWindow));
        return AppWindow.GetFromWindowId(windowId);
    }
}