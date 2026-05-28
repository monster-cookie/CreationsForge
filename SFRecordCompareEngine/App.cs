using Autofac;
using Serilog;
using SFRecordCompareEngine.Core.Database.Interfaces;
using SFRecordCompareEngine.Pages;

namespace SFRecordCompareEngine;

public partial class App : Application
{
    private readonly Autofac.IContainer Container;

    public App(Autofac.IContainer container)
    {
        Container = container;
        UserAppTheme = AppTheme.Dark;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        Log.Information("Starting Starfield Record Compare Engine");
        Container.Resolve<IDatabaseSchemaInitializer>().Initialize();
        return new Window(Container.Resolve<StartupImportPage>())
        {
            Title = "Starfield Record Compare Engine"
        };
    }

    protected override void CleanUp()
    {
        Log.Information("Exiting Starfield Record Compare Engine");
        Container.Dispose();
        Log.CloseAndFlush();
        base.CleanUp();
    }
}
