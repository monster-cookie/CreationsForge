using Autofac;
using CreationsForge.Core;
using CreationsForge.Fallout4;
using CreationsForge.Migrations;
using CreationsForge.Skyrim;
using CreationsForge.Starfield;

namespace CreationsForge.Bootstrap.Composition;

public static class AutofacConfigurator
{
    public static IContainer Configure(Action<ContainerBuilder>? registerApplicationServices = null)
    {
        var builder = new ContainerBuilder();
        RegisterSharedModules(builder);
        registerApplicationServices?.Invoke(builder);
        return builder.Build();
    }

    public static void RegisterSharedModules(ContainerBuilder builder)
    {
        builder.RegisterModule<CoreModule>();
        builder.RegisterModule<MigrationsModule>();
        builder.RegisterModule<StarfieldModule>();
        builder.RegisterModule<Fallout4Module>();
        builder.RegisterModule<SkyrimModule>();
    }
}
