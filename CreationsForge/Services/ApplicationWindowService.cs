using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CreationsForge.Core.Models.Configuration;
using CreationsForge.Services.Interfaces;

namespace CreationsForge.Services;

public class ApplicationWindowService : IApplicationWindowService
{
    private MainWindow? MainWindow;

    public void RegisterMainWindow(MainWindow mainWindow)
    {
        MainWindow = mainWindow;
    }

    public void SetContent(Control content)
    {
        if (MainWindow is null)
        {
            throw new InvalidOperationException("The main window has not been registered.");
        }

        MainWindow.SetContent(content);
    }

    public void ClearContent(Control content)
    {
        if (MainWindow?.Content == content)
        {
            MainWindow.SetContent(new Border());
        }
    }

    public void ApplyTheme(ApplicationThemeFamily themeFamily, ApplicationThemeMode themeMode)
    {
        if (Application.Current is null)
        {
            return;
        }

        App.ApplyTheme(Application.Current, themeFamily, themeMode);
        MainWindow?.ApplyTheme(themeMode);
    }

    public async Task<TResult> ShowDialogAsync<TResult>(Window dialog)
    {
        if (MainWindow is null)
        {
            throw new InvalidOperationException("The main window has not been registered.");
        }

        return await dialog.ShowDialog<TResult>(MainWindow);
    }

    public void Quit()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Shutdown();
        }
    }
}
