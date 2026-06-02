using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SFRecordCompareEngine.Core.Models.Configuration;
using SFRecordCompareEngine.Services.Interfaces;
using SFRecordCompareEngine.ViewModels;

namespace SFRecordCompareEngine.Services;

public class DesktopApplicationWindowService : IApplicationWindowService
{
    private static readonly string ApplicationIconPath = Path.Combine(AppContext.BaseDirectory, "Resources", "AppIcon", "sfrecordcompareengine.ico");

    private MainWindow? MainWindow;

    public void RegisterMainWindow(MainWindow mainWindow)
    {
        MainWindow = mainWindow;
        MainWindow.AppWindow.SetIcon(ApplicationIconPath);
    }

    public void SetContent(UIElement content)
    {
        MainWindow?.SetContent(content);
    }

    public void ShowMainCommandSurface(MainPageViewModel viewModel)
    {
        MainWindow?.ShowMainCommandSurface(viewModel);
    }

    public void HideMainCommandSurface()
    {
        MainWindow?.HideMainCommandSurface();
    }

    public void ApplyTheme(ApplicationThemeMode theme)
    {
        MainWindow?.ApplyTheme(theme);
    }

    public async Task ShowDialogAsync(ContentDialog dialog)
    {
        if (MainWindow == null)
        {
            return;
        }

        await MainWindow.ShowDialogAsync(dialog);
    }

    public void CloseDialog()
    {
        MainWindow?.CloseDialog();
    }

    public void MaximizeMainWindow()
    {
        if (MainWindow?.AppWindow.Presenter is OverlappedPresenter presenter)
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
