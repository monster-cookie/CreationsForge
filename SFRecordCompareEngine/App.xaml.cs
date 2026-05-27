using System.IO;
using System.Windows;
using Autofac;
using Serilog;
using SFRecordCompareEngine.Core;
using SFRecordCompareEngine.Core.Models.Database;
using SFRecordCompareEngine.Migrations;
using SFRecordCompareEngine.ViewModels;

namespace SFRecordCompareEngine;

public partial class App
{
    private IContainer? Container;

    protected override void OnStartup(StartupEventArgs e)
    {
        var databaseOptions = new SqliteDatabaseOptions();
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
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

        Log.Information("Starting SFRecordCompareEngine");

        base.OnStartup(e);

        Container = BuildContainer();
        Container.Resolve<MainWindow>().Show();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        Log.Information("Exiting SFRecordCompareEngine");
        Container?.Dispose();
        Log.CloseAndFlush();
        base.OnExit(e);
    }

    private static IContainer BuildContainer()
    {
        var builder = new ContainerBuilder();
        builder.RegisterModule<CoreModule>();
        builder.RegisterModule<MigrationsModule>();
        builder.RegisterInstance(Log.Logger).As<ILogger>().SingleInstance();
        builder.RegisterType<MainWindowViewModel>();
        builder.RegisterType<MainWindow>();
        builder.RegisterType<DatabaseImportConfirmationDialog>();

        return builder.Build();
    }

}
