using Autofac;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.Themes.Fluent;
using CreationsForge.Bootstrap.Logging;
using CreationsForge.Composition;
using CreationsForge.Core.Configuration;
using CreationsForge.Core.Configuration.Interfaces;
using CreationsForge.Core.Models.Configuration;
using CreationsForge.Core.Services.Interfaces;
using CreationsForge.Services;
using CreationsForge.Services.Interfaces;
using CreationsForge.ViewModels;
using CreationsForge.Views;
using Serilog;
using Semi.Avalonia;
using Semi.Avalonia.DataGrid;

namespace CreationsForge;

/// <summary>
/// Composes and owns the Avalonia desktop application and its native engine lifetime.
/// </summary>
public class App : Application
{
    /// <summary>The resource key for the application background brush.</summary>
    public const string ApplicationSurfaceBrushKey = "CreationsForge.ApplicationSurfaceBrush";

    /// <summary>The resource key for panel background brushes.</summary>
    public const string PanelSurfaceBrushKey = "CreationsForge.PanelSurfaceBrush";

    /// <summary>The resource key for application foreground text.</summary>
    public const string ApplicationForegroundBrushKey = "CreationsForge.ApplicationForegroundBrush";

    /// <summary>The resource key for application border brushes.</summary>
    public const string BorderBrushKey = "CreationsForge.BorderBrush";

    /// <summary>The application-owned dependency container.</summary>
    private readonly IContainer Container;

    /// <summary>Flushes process-wide logging after the accepted terminal shutdown attempt.</summary>
    private readonly Action CloseLog;

    /// <summary>Synchronizes creation of the one application cleanup task.</summary>
    private readonly object ShutdownSync = new();

    /// <summary>The shared cleanup task once shutdown begins.</summary>
    private Task? ShutdownTask;

    /// <summary>Whether an accepted teardown reached its terminal desktop-exit phase.</summary>
    private bool ShutdownAuthorized;

    /// <summary>Initializes logging and the application dependency graph without opening plugins.</summary>
    public App()
    {
        var configurationStore = new ApplicationConfigurationStore();
        SerilogConfigurator.Configure(configurationStore, writeToConsole: false);
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        Container = NativeDesktopComposition.Create(configurationStore, Log.Logger);
        CloseLog = Log.CloseAndFlush;
    }

    /// <summary>Initializes an application that assumes ownership of a supplied container without accessing profile configuration.</summary>
    /// <param name="container">The complete native desktop dependency container whose lifetime transfers to the application.</param>
    /// <param name="closeLog">The optional log finalizer; <see langword="null"/> selects a no-op finalizer for isolated tests.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="container"/> is <see langword="null"/>.</exception>
    internal App(IContainer container, Action? closeLog = null)
    {
        ArgumentNullException.ThrowIfNull(container);
        Container = container;
        CloseLog = closeLog ?? (() => { });
    }

    /// <summary>Applies persisted theme resources before desktop controls are created.</summary>
    public override void Initialize()
    {
        ApplyTheme(this, GetConfiguredThemeFamily(), GetConfiguredThemeMode());
    }

    /// <summary>Starts diagnostics, publishes the native shell, and attaches guarded desktop startup and shutdown behavior.</summary>
    public override void OnFrameworkInitializationCompleted()
    {
        try
        {
            Log.Information("Starting CreationsForge UI");
            Container.Resolve<IProcessTerminationDiagnosticsService>().StartSession("UI", SerilogConfigurator.CurrentLogPath);
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var mainWindow = Container.Resolve<MainWindow>();
                Container.Resolve<INativeApplicationNavigationService>().ShowWorkspaceShell();
                mainWindow.Opened += OnMainWindowOpened;
                desktop.ShutdownRequested += OnDesktopShutdownRequested;
                desktop.MainWindow = mainWindow;
            }
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Unable to launch CreationsForge UI");
            Log.CloseAndFlush();
            throw;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception ex)
        {
            Log.Fatal(ex, "Unhandled Avalonia desktop exception");
        }
        else
        {
            Log.Fatal("Unhandled Avalonia desktop exception: {ExceptionObject}", e.ExceptionObject);
        }

        Log.CloseAndFlush();
    }

    private ApplicationThemeMode GetConfiguredThemeMode()
    {
        var configurationStore = Container.Resolve<IApplicationConfigurationStore>();
        return configurationStore.Current.ThemeMode;
    }

    private ApplicationThemeFamily GetConfiguredThemeFamily()
    {
        var configurationStore = Container.Resolve<IApplicationConfigurationStore>();
        return configurationStore.Current.ThemeFamily;
    }

    public static void ApplyTheme(Application application, ApplicationThemeFamily themeFamily, ApplicationThemeMode themeMode)
    {
        application.RequestedThemeVariant = GetThemeVariant(themeMode);
        ApplyThemeResources(application, themeFamily, themeMode);
        application.Styles.Clear();
        switch (themeFamily)
        {
            case ApplicationThemeFamily.Fluent:
                application.Styles.Add(new FluentTheme());
                application.Styles.Add(new StyleInclude(new Uri("avares://CreationsForge"))
                {
                    Source = new Uri("avares://Avalonia.Controls.DataGrid/Themes/Fluent.xaml")
                });
                application.Styles.Add(new StyleInclude(new Uri("avares://CreationsForge"))
                {
                    Source = new Uri("avares://Avalonia.Controls.TreeDataGrid/Themes/Fluent.axaml")
                });
                break;
            default:
                application.Styles.Add(new FluentTheme());
                application.Styles.Add(new SemiTheme());
                application.Styles.Add(new DataGridSemiTheme());
                application.Styles.Add(new StyleInclude(new Uri("avares://CreationsForge"))
                {
                    Source = new Uri("avares://Avalonia.Controls.TreeDataGrid/Themes/Fluent.axaml")
                });
                break;
        }
    }

    public static void ApplyApplicationTextForeground(TextBlock textBlock)
    {
        textBlock.Foreground = GetApplicationForegroundBrush();
    }

    public static IBrush GetApplicationForegroundBrush()
    {
        if (Current is null || !Dispatcher.UIThread.CheckAccess())
        {
            return new SolidColorBrush(Color.FromRgb(24, 28, 32));
        }

        if (Current?.Resources.TryGetResource(ApplicationForegroundBrushKey, Current.ActualThemeVariant, out var resource) == true &&
            resource is IBrush brush)
        {
            return brush;
        }

        if (Current?.ActualThemeVariant == ThemeVariant.Dark)
        {
            return new SolidColorBrush(Color.FromRgb(238, 241, 245));
        }

        return new SolidColorBrush(Color.FromRgb(24, 28, 32));
    }

    public static IBrush GetApplicationBrush(string resourceKey)
    {
        if (resourceKey == ApplicationForegroundBrushKey)
        {
            return GetApplicationForegroundBrush();
        }

        if (Current?.Resources.TryGetResource(resourceKey, Current.ActualThemeVariant, out var resource) == true &&
            resource is IBrush brush)
        {
            return brush;
        }

        return Brushes.Transparent;
    }

    private static ThemeVariant GetThemeVariant(ApplicationThemeMode themeMode)
    {
        return themeMode == ApplicationThemeMode.Light
            ? ThemeVariant.Light
            : ThemeVariant.Dark;
    }

    private static void ApplyThemeResources(Application application, ApplicationThemeFamily themeFamily, ApplicationThemeMode themeMode)
    {
        if (themeMode == ApplicationThemeMode.Light)
        {
            application.Resources[ApplicationSurfaceBrushKey] = new SolidColorBrush(Color.FromRgb(250, 250, 250));
            application.Resources[PanelSurfaceBrushKey] = new SolidColorBrush(Color.FromRgb(244, 244, 244));
            application.Resources.Remove(ApplicationForegroundBrushKey);
            application.Resources[BorderBrushKey] = new SolidColorBrush(Color.FromRgb(150, 156, 164));
            return;
        }

        application.Resources[ApplicationSurfaceBrushKey] = new SolidColorBrush(Color.FromRgb(24, 28, 32));
        application.Resources[PanelSurfaceBrushKey] = new SolidColorBrush(Color.FromRgb(31, 36, 42));
        if (themeFamily == ApplicationThemeFamily.Fluent)
        {
            application.Resources[ApplicationForegroundBrushKey] = new SolidColorBrush(Color.FromRgb(238, 241, 245));
        }
        else
        {
            application.Resources.Remove(ApplicationForegroundBrushKey);
        }

        application.Resources[BorderBrushKey] = new SolidColorBrush(Color.FromRgb(80, 88, 96));
    }

    /// <summary>Shows complete native source-and-output selection once the owner window is visible.</summary>
    /// <param name="sender">The opened main window.</param>
    /// <param name="eventArgs">The window-open event arguments.</param>
    private async void OnMainWindowOpened(object? sender, EventArgs eventArgs)
    {
        if (sender is MainWindow mainWindow)
        {
            mainWindow.Opened -= OnMainWindowOpened;
        }

        try
        {
            var selectionViewModel = Container.Resolve<Func<NativeWorkspaceSelectionViewModel>>()();
            await Container.Resolve<INativeWorkspaceSelectionDialogService>().ShowAsync(selectionViewModel);
        }
        catch (Exception exception)
        {
            Log.Error(exception, "Unable to show initial native workspace selection.");
        }
    }

    /// <summary>Begins the guarded application shutdown path.</summary>
    /// <remarks>The desktop lifetime remains open until native ownership and the container have been released.</remarks>
    public void ShutDown()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.TryShutdown();
            return;
        }

        _ = BeginShutdownAsync(desktop: null);
    }

    /// <summary>Cancels a desktop shutdown request until the shared asynchronous cleanup task finishes.</summary>
    /// <param name="sender">The desktop lifetime requesting shutdown.</param>
    /// <param name="eventArgs">The cancelable shutdown request.</param>
    private void OnDesktopShutdownRequested(object? sender, ShutdownRequestedEventArgs eventArgs)
    {
        if (ShutdownAuthorized)
        {
            return;
        }

        eventArgs.Cancel = true;
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            _ = BeginShutdownAsync(desktop);
        }
    }

    /// <summary>Returns the one cleanup task shared by repeated shutdown requests.</summary>
    /// <param name="desktop">The desktop lifetime to close after cleanup, or <see langword="null"/>.</param>
    /// <returns>The shared cleanup task.</returns>
    internal Task BeginShutdownAsync(IClassicDesktopStyleApplicationLifetime? desktop)
    {
        lock (ShutdownSync)
        {
            ShutdownTask ??= RunShutdownAttemptAsync(desktop);
            return ShutdownTask;
        }
    }

    /// <summary>Runs one shared shutdown attempt and permits a later attempt only when teardown never began.</summary>
    /// <param name="desktop">The desktop lifetime to close after cleanup, or <see langword="null"/>.</param>
    /// <returns>A task that completes when the shutdown attempt reaches a terminal result.</returns>
    private async Task RunShutdownAttemptAsync(IClassicDesktopStyleApplicationLifetime? desktop)
    {
        var completed = false;
        try
        {
            completed = await CleanUpForExitAsync(desktop);
        }
        catch (Exception exception)
        {
            Log.Error(exception, "The guarded application shutdown attempt failed unexpectedly.");
        }
        finally
        {
            if (!completed)
            {
                lock (ShutdownSync)
                {
                    ShutdownTask = null;
                }
            }
        }
    }

    /// <summary>Reserves safe shutdown, releases native ownership, records clean termination when possible, and disposes the application container before exit.</summary>
    /// <param name="desktop">The desktop lifetime to close after cleanup, or <see langword="null"/>.</param>
    /// <returns><see langword="false"/> only when shutdown is rejected or fails before teardown begins; otherwise <see langword="true"/> after the accepted terminal attempt.</returns>
    private async Task<bool> CleanUpForExitAsync(IClassicDesktopStyleApplicationLifetime? desktop)
    {
        await Task.Yield();
        NativeApplicationShutdownLease? shutdownLease;
        try
        {
            shutdownLease = await Container.Resolve<INativeApplicationNavigationService>().ReserveShutdownAsync();
        }
        catch (Exception exception)
        {
            Log.Error(exception, "Unable to reserve guarded native workspace shutdown.");
            return false;
        }

        if (shutdownLease is null)
        {
            return false;
        }

        using (shutdownLease)
        {
            var canMarkCleanShutdown = true;
            IProcessTerminationDiagnosticsService? diagnostics = null;
            Log.Information("Exiting CreationsForge UI");
            try
            {
                diagnostics = Container.Resolve<IProcessTerminationDiagnosticsService>();
                await Container.Resolve<INativeWorkspaceCoordinator>().DisposeAsync();
            }
            catch (Exception exception)
            {
                canMarkCleanShutdown = false;
                Log.Error(exception, "Unable to finish native workspace shutdown cleanly.");
            }

            try
            {
                await Container.DisposeAsync();
            }
            catch (Exception exception)
            {
                canMarkCleanShutdown = false;
                Log.Error(exception, "Unable to dispose the CreationsForge application container cleanly.");
            }

            if (canMarkCleanShutdown && diagnostics is not null)
            {
                try
                {
                    diagnostics.MarkCleanShutdown("UI exit");
                }
                catch (Exception exception)
                {
                    Log.Error(exception, "Unable to record clean CreationsForge application shutdown.");
                }
            }

            if (desktop is not null)
            {
                ShutdownAuthorized = true;
                desktop.ShutdownRequested -= OnDesktopShutdownRequested;
                try
                {
                    desktop.Shutdown();
                }
                catch (Exception exception)
                {
                    Log.Error(exception, "Unable to complete the accepted Avalonia desktop shutdown cleanly.");
                }
            }

            try
            {
                CloseLog();
            }
            catch (Exception exception)
            {
                Log.Error(exception, "Unable to flush logging during the accepted application shutdown.");
            }

            return true;
        }
    }
}
