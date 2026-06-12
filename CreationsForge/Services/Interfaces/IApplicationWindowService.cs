using Avalonia.Controls;
using CreationsForge.Core.Models.Configuration;

namespace CreationsForge.Services.Interfaces;

public interface IApplicationWindowService
{
    void RegisterMainWindow(MainWindow mainWindow);

    void SetContent(Control content);

    void ClearContent(Control content);

    void ApplyTheme(ApplicationThemeFamily themeFamily, ApplicationThemeMode themeMode);

    Task<TResult> ShowDialogAsync<TResult>(Window dialog);

    void Quit();
}
