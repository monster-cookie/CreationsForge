using System.Windows;
using Autofac;
using Serilog;
using SFRecordCompareEngine.Core;

namespace SFRecordCompareEngine;

public partial class App
{
    private IContainer? Container;

    protected override void OnStartup(StartupEventArgs e)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File(
                @"C:\temp\SFRecordCompareEngine-Log.txt",
                rollingInterval: RollingInterval.Day,
                retainedFileTimeLimit: TimeSpan.FromDays(7),
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
        builder.RegisterInstance(Log.Logger).As<ILogger>().SingleInstance();
        builder.RegisterType<MainWindow>();

        return builder.Build();
    }
}
