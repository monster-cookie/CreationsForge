using Autofac;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using SFRecordCompareEngine.Core;
using SFRecordCompareEngine.Core.Models.Database;
using SFRecordCompareEngine.Migrations;
using SFRecordCompareEngine.Pages;
using SFRecordCompareEngine.Services;
using SFRecordCompareEngine.Services.Interfaces;
using SFRecordCompareEngine.ViewModels;

namespace SFRecordCompareEngine;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder.UseMauiApp<App>();

        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog(Log.Logger, dispose: true);

        // Configure Serilog logging
        var databaseOptions = new SqliteDatabaseOptions();
        Log.Logger = new LoggerConfiguration()
#if DEBUG
            .MinimumLevel.Debug()
#else
            .MinimumLevel.Information()
#endif
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Maui", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentUserName()
            .WriteTo.File(
                Path.Combine(databaseOptions.LogDirectory, "SFRecordCompareEngine.log"),
                rollingInterval: RollingInterval.Day,
                retainedFileTimeLimit: TimeSpan.FromDays(7),
                fileSizeLimitBytes: 1024 * 1024 * 100, // 100 MB
                shared: true,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();        
        
#if DEBUG
        builder.Logging.AddDebug();
#endif
        
        var container = BuildContainer();
        builder.Services.AddSingleton(container);

        return builder.Build();
    }

    private static Autofac.IContainer BuildContainer()
    {
        var builder = new ContainerBuilder();

        // Register All Modules
        builder.RegisterModule<CoreModule>();
        builder.RegisterModule<MigrationsModule>();
        
        // Register Serilog logger as a singleton (Not sure if this is valid or needed)
        builder.RegisterInstance(Log.Logger).As<Serilog.ILogger>().SingleInstance();
        
        // Register MVVM ViewModels and Pages
        builder.RegisterType<StartupImportViewModel>();
        builder.RegisterType<MainPageViewModel>();
        builder.RegisterType<OpenPluginDialogViewModel>();
        builder.RegisterType<StartupImportPage>();
        builder.RegisterType<MainPage>();
        builder.RegisterType<OpenPluginDialogPage>();
        
        // Register MVVM Services
        builder.RegisterType<UserDialogService>().As<IUserDialogService>().SingleInstance();
        builder.RegisterType<ApplicationNavigationService>().As<IApplicationNavigationService>().SingleInstance();
        builder.RegisterType<WindowsApplicationWindowService>().As<IApplicationWindowService>().SingleInstance();

        return builder.Build();
    }
}
