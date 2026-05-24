using System.Reflection;
using Autofac;
using SFRecordCompareEngine.Core.Database;
using SFRecordCompareEngine.Core.Configuration;
using SFRecordCompareEngine.Core.Configuration.Interfaces;
using SFRecordCompareEngine.Core.Models.Database;
using SFRecordCompareEngine.Core.Services;
using SFRecordCompareEngine.Core.Services.Interfaces;
using Module = Autofac.Module;

namespace SFRecordCompareEngine.Core;

public class CoreModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<ApplicationConfigurationStore>()
            .As<IApplicationConfigurationStore>()
            .SingleInstance();
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

        builder.RegisterType<FormListRecordImporter>().SingleInstance();
        builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
            .AssignableTo<ITypedRecordDetailImporter>()
            .As<ITypedRecordDetailImporter>()
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
