using Avalonia.Controls;
using Avalonia.Automation;
using Avalonia.Styling;
using CreationsForge.Core.Models.Configuration;
using CreationsForge.Services.Interfaces;

namespace CreationsForge;

/// <summary>
/// Provides the registered top-level owner and content host for the desktop application.
/// </summary>
public class MainWindow : Window
{
    /// <summary>The application window service that owns root content and modal parenting.</summary>
    private readonly IApplicationWindowService ApplicationWindowService;

    /// <summary>Initializes and registers the desktop root window without starting a navigation workflow.</summary>
    /// <param name="applicationWindowService">The root content and dialog owner service.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="applicationWindowService"/> is <see langword="null"/>.</exception>
    public MainWindow(IApplicationWindowService applicationWindowService)
    {
        ArgumentNullException.ThrowIfNull(applicationWindowService);
        Title = "CreationsForge";
        Width = 2100;
        Height = 1000;
        WindowState = WindowState.Maximized;
        AutomationProperties.SetAutomationId(this, "MainWindow");
        ApplicationWindowService = applicationWindowService;
        ApplicationWindowService.RegisterMainWindow(this);
    }

    /// <summary>Applies the selected light or dark theme variant to the top-level window.</summary>
    /// <param name="themeMode">The requested application theme mode.</param>
    public void ApplyTheme(ApplicationThemeMode themeMode)
    {
        RequestedThemeVariant = themeMode == ApplicationThemeMode.Light
            ? ThemeVariant.Light
            : ThemeVariant.Dark;
    }

    /// <summary>Replaces the application window's root content.</summary>
    /// <param name="content">The control to host.</param>
    public void SetContent(Control content)
    {
        Content = content;
    }
}
