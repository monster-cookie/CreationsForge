using System.Reflection;
using Autofac;
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
        
        // Register Factory
        builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
            .Where(t => t.Name.EndsWith("Factory"))
            .AsImplementedInterfaces();
    }
}
