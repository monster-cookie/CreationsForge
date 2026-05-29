using Autofac;
using Microsoft.UI.Xaml;
using Serilog;
using Serilog.Events;
using SFRecordCompareEngine.Core;
using SFRecordCompareEngine.Core.Configuration.Interfaces;
using SFRecordCompareEngine.Core.Models.Database;
using SFRecordCompareEngine.Migrations;
using SFRecordCompareEngine.Services;
using SFRecordCompareEngine.Services.Interfaces;
using SFRecordCompareEngine.ViewModels;
using SFRecordCompareEngine.Views;

namespace SFRecordCompareEngine;

public partial class App : Application
{
    private readonly IContainer Container;
    private Window? Window;

    public App()
    {
        InitializeComponent();
        ConfigureLogging();
        UnhandledException += OnUnhandledException;
        Container = BuildContainer();
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        try
        {
            Log.Information("Starting Starfield Record Compare Engine");

            Window = Container.Resolve<MainWindow>();
            Window.Activate();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Unable to launch Starfield Record Compare Engine");
            Log.CloseAndFlush();
            throw;
        }
    }

    private void OnUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        Log.Fatal(e.Exception, "Unhandled WinUI exception");
        Log.CloseAndFlush();
    }

    private static void ConfigureLogging()
    {
        var databaseOptions = new SqliteDatabaseOptions();
        Log.Logger = new LoggerConfiguration()
#if DEBUG
            .MinimumLevel.Debug()
#else
            .MinimumLevel.Information()
#endif
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentUserName()
            .WriteTo.File(
                Path.Combine(databaseOptions.LogDirectory, "SFRecordCompareEngine.log"),
                rollingInterval: RollingInterval.Day,
                retainedFileTimeLimit: TimeSpan.FromDays(7),
                fileSizeLimitBytes: 1024 * 1024 * 100,
                shared: true,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();
    }

    private static IContainer BuildContainer()
    {
        var builder = new ContainerBuilder();

        builder.RegisterModule<CoreModule>();
        builder.RegisterModule<MigrationsModule>();

        builder.RegisterInstance(Log.Logger).As<Serilog.ILogger>().SingleInstance();

        builder.RegisterType<StartupImportViewModel>();
        builder.RegisterType<MainPageViewModel>();
        builder.RegisterType<OpenPluginDialogViewModel>();
        builder.RegisterType<SettingsViewModel>();
        builder.RegisterType<MainWindow>().SingleInstance();
        builder.RegisterType<StartupImportView>();
        builder.RegisterType<MainView>();
        builder.RegisterType<OpenPluginDialog>();
        builder.RegisterType<SettingsDialog>();

        builder.RegisterType<UserDialogService>().As<IUserDialogService>().SingleInstance();
        builder.RegisterType<ApplicationNavigationService>().As<IApplicationNavigationService>().SingleInstance();
        builder.RegisterType<WindowsApplicationWindowService>().As<IApplicationWindowService>().SingleInstance();

        return builder.Build();
    }

    public void ShutDown()
    {
        Log.Information("Exiting Starfield Record Compare Engine");
        Container.Dispose();
        Log.CloseAndFlush();
        Exit();
    }
}
