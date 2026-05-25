using System.Reflection;
using Autofac;
using SFRecordCompareEngine.Core.Database;
using SFRecordCompareEngine.Core.Configuration;
using SFRecordCompareEngine.Core.Configuration.Interfaces;
using SFRecordCompareEngine.Core.Importers;
using SFRecordCompareEngine.Core.Importers.Interfaces;
using SFRecordCompareEngine.Core.Models.Database;
using SFRecordCompareEngine.Core.Services;
using SFRecordCompareEngine.Core.Services.Interfaces;
using Module = Autofac.Module;

namespace SFRecordCompareEngine.Core;

public class CoreModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        // Register any stores
        builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
            .Where(t => t.Name.EndsWith("Store", StringComparison.OrdinalIgnoreCase))
            .AsImplementedInterfaces()
            .SingleInstance();

        // Register any services
        builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
            .Where(t => t.Name.EndsWith("Service", StringComparison.OrdinalIgnoreCase))
            .AsImplementedInterfaces()
            .SingleInstance();

        // Register any factories
        builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
            .Where(t => t.Name.EndsWith("Factory"))
            .AsImplementedInterfaces();

        // Register database initializers and repositories
        builder.RegisterType<SqliteDatabaseOptions>().SingleInstance();
        builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
            .Where(t => t.Name.EndsWith("Initializer", StringComparison.OrdinalIgnoreCase))
            .AsImplementedInterfaces()
            .SingleInstance();

        builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
            .Where(t => t.Name.EndsWith("Repository", StringComparison.OrdinalIgnoreCase))
            .AsImplementedInterfaces()
            .SingleInstance();
    }
}
