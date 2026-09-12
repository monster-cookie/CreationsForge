using Avalonia.Controls;
using CreationsForge.Core.Models.Configuration;
using CreationsForge.Services.Interfaces;

namespace CreationsForge.PresentationTests.Support;

/// <summary>
/// Records root-window operations without invoking platform dialogs.
/// </summary>
internal sealed class FakeApplicationWindowService : IApplicationWindowService
{
    /// <summary>Gets the main window registered by the application.</summary>
    public MainWindow? MainWindow { get; private set; }

    /// <summary>Gets the control currently hosted as root content.</summary>
    public Control? Content { get; private set; }

    /// <summary>Gets the last applied theme family.</summary>
    public ApplicationThemeFamily? ThemeFamily { get; private set; }

    /// <summary>Gets the last applied theme mode.</summary>
    public ApplicationThemeMode? ThemeMode { get; private set; }

    /// <summary>Gets how many quit requests were received.</summary>
    public int QuitCount { get; private set; }

    /// <inheritdoc />
    public void RegisterMainWindow(MainWindow mainWindow)
    {
        MainWindow = mainWindow;
    }

    /// <inheritdoc />
    public void SetContent(Control content)
    {
        Content = content;
    }

    /// <inheritdoc />
    public void ClearContent(Control content)
    {
        if (ReferenceEquals(Content, content))
        {
            Content = null;
        }
    }

    /// <inheritdoc />
    public void ApplyTheme(ApplicationThemeFamily themeFamily, ApplicationThemeMode themeMode)
    {
        ThemeFamily = themeFamily;
        ThemeMode = themeMode;
    }

    /// <inheritdoc />
    public Task<TResult> ShowDialogAsync<TResult>(Window dialog)
    {
        return Task.FromResult(default(TResult)!);
    }

    /// <inheritdoc />
    public Task<string?> ShowNifSkopeExecutablePickerAsync()
    {
        return Task.FromResult<string?>(null);
    }

    /// <inheritdoc />
    public void Quit()
    {
        QuitCount++;
    }
}
