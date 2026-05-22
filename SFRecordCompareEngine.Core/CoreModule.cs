using System.Reflection;
using Autofac;
using SFRecordCompareEngine.Core.Database;
using SFRecordCompareEngine.Core.Models.Database;
using Module = Autofac.Module;

namespace SFRecordCompareEngine.Core;

public class CoreModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<SqliteDatabaseOptions>().SingleInstance();

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

        // Register Factory
        builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
            .Where(t => t.Name.EndsWith("Factory"))
            .AsImplementedInterfaces();

        // Register database initializers and repositories
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
