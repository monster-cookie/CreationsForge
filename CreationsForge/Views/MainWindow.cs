using Avalonia.Controls;
using Avalonia.Automation;
using Avalonia.Styling;
using CreationsForge.Core.Models.Configuration;
using CreationsForge.Services.Interfaces;

namespace CreationsForge;

public class MainWindow : Window
{
    private readonly IApplicationWindowService ApplicationWindowService;

    public MainWindow(
        IApplicationWindowService applicationWindowService,
        IApplicationNavigationService applicationNavigationService)
    {
        Title = "CreationsForge";
        Width = 2100;
        Height = 1000;
        WindowState = WindowState.Maximized;
        AutomationProperties.SetAutomationId(this, "MainWindow");
        ApplicationWindowService = applicationWindowService;
        ApplicationWindowService.RegisterMainWindow(this);
        _ = applicationNavigationService.ShowMainViewAsync(selectedGame: null, runConfiguredGameImport: true);
    }

    public void ApplyTheme(ApplicationThemeMode themeMode)
    {
        RequestedThemeVariant = themeMode == ApplicationThemeMode.Light
            ? ThemeVariant.Light
            : ThemeVariant.Dark;
    }

    public void SetContent(Control content)
    {
        Content = content;
    }
}
