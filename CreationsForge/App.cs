using Autofac;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Themes.Fluent;
using CreationsForge.Bootstrap.Composition;
using CreationsForge.Bootstrap.Logging;
using CreationsForge.Core.Configuration;
using CreationsForge.Core.Configuration.Interfaces;
using CreationsForge.Core.Database.Interfaces;
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

public class App : Application
{
    public const string ApplicationSurfaceBrushKey = "CreationsForge.ApplicationSurfaceBrush";
    public const string PanelSurfaceBrushKey = "CreationsForge.PanelSurfaceBrush";
    public const string ApplicationForegroundBrushKey = "CreationsForge.ApplicationForegroundBrush";
    public const string BorderBrushKey = "CreationsForge.BorderBrush";

    private readonly IContainer Container;
    private bool HasShutDown;

    public App()
    {
        SerilogConfigurator.Configure(new ApplicationConfigurationStore(), writeToConsole: false);
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        Container = AutofacConfigurator.Configure(RegisterPresentationServices);
    }

    public override void Initialize()
    {
        ApplyTheme(this, GetConfiguredThemeFamily(), GetConfiguredThemeMode());
    }

    public override void OnFrameworkInitializationCompleted()
    {
        try
        {
            Log.Information("Starting CreationsForge UI");
            Container.Resolve<IProcessTerminationDiagnosticsService>().StartSession("UI", SerilogConfigurator.CurrentLogPath);
            Container.Resolve<IDatabaseSchemaInitializer>().Initialize();
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = Container.Resolve<MainWindow>();
                desktop.Exit += OnDesktopExit;
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

    private static void RegisterPresentationServices(ContainerBuilder builder)
    {
        builder.RegisterType<MainWindow>().SingleInstance();
        builder.RegisterType<ActivePluginLoadView>();
        builder.RegisterType<ActivePluginLoadViewModel>();
        builder.RegisterType<AssetPreviewPaneView>();
        builder.RegisterType<AssetPreviewPaneViewModel>().InstancePerLifetimeScope();
        builder.RegisterType<ImportProgressView>();
        builder.RegisterType<ImportProgressViewModel>();
        builder.RegisterType<MainView>();
        builder.RegisterType<MainViewModel>();
        builder.RegisterType<SettingsView>();
        builder.RegisterType<SettingsViewModel>();
        builder.RegisterType<ApplicationWindowService>().As<IApplicationWindowService>().SingleInstance();
        builder.RegisterType<ApplicationNavigationService>().As<IApplicationNavigationService>().SingleInstance();
        builder.RegisterType<AssetPreviewRenderMeshFactory>().As<IAssetPreviewRenderMeshFactory>().SingleInstance();
        builder.RegisterType<AssetPreviewSceneService>().As<IAssetPreviewSceneService>().SingleInstance();
        builder.RegisterType<BethesdaAssetPreviewGeometryReader>().As<IAssetPreviewGeometryReader>().SingleInstance();
        builder.RegisterType<ExternalAssetOpenService>().As<IExternalAssetOpenService>().SingleInstance();
        builder.RegisterType<UserDialogService>().As<IUserDialogService>().SingleInstance();
        builder.RegisterInstance(Log.Logger).As<ILogger>().SingleInstance();
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

    private void OnDesktopExit(object? sender, ControlledApplicationLifetimeExitEventArgs e)
    {
        CleanUpForExit();
    }

    public void ShutDown()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Exit -= OnDesktopExit;
            CleanUpForExit();
            desktop.Shutdown();
            return;
        }

        CleanUpForExit();
    }

    private void CleanUpForExit()
    {
        if (HasShutDown)
        {
            return;
        }

        HasShutDown = true;
        Log.Information("Exiting CreationsForge UI");
        Container.Resolve<IProcessTerminationDiagnosticsService>().MarkCleanShutdown("UI exit");
        Container.Dispose();
        Log.CloseAndFlush();
    }
}
