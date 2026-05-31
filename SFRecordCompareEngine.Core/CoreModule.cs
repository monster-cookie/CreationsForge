using System.Reflection;
using Autofac;
using NPoco;
using SFRecordCompareEngine.Core.Database.Interfaces;
using SFRecordCompareEngine.Core.Models.Database;
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

        // Register any importers
        builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
            .Where(t => t.Name.EndsWith("Importer", StringComparison.OrdinalIgnoreCase))
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
        builder.Register(c => c.Resolve<ISqliteConnectionFactory>().OpenDatabase())
            .As<IDatabase>()
            .InstancePerLifetimeScope();

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