using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
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

    public async Task<string?> ShowNifSkopeExecutablePickerAsync()
    {
        if (MainWindow is null)
        {
            throw new InvalidOperationException("The main window has not been registered.");
        }

        var files = await MainWindow.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select fo76utils NifSkope executable",
            AllowMultiple = false,
            FileTypeFilter =
            [
                new FilePickerFileType("NifSkope executable")
                {
                    Patterns = ["*.exe"]
                },
                FilePickerFileTypes.All
            ]
        });
        return files.Count > 0
            ? files[0].TryGetLocalPath()
            : null;
    }

    public void Quit()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Shutdown();
        }
    }
}
