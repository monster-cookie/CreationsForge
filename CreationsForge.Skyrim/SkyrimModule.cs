using Autofac;
using Autofac.Core;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Importers;
using CreationsForge.Core.Importers.Interfaces;
using CreationsForge.Skyrim.Interfaces;
using CreationsForge.Skyrim.Importers;
using CreationsForge.Skyrim.Repositories;
using CreationsForge.Skyrim.Repositories.Interfaces;
using Module = Autofac.Module;

namespace CreationsForge.Skyrim;

public class SkyrimModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<SkyrimGameMetadataService>().AsSelf().AsImplementedInterfaces().SingleInstance();
        builder.RegisterType<SkyrimPluginReaderService>().As<ISkyrimPluginReaderService>().SingleInstance();
        builder.RegisterType<SkyrimRecordReaderService>().As<ISkyrimRecordReaderService>().SingleInstance();
        builder.RegisterType<SkyrimPluginRepository>().As<ISkyrimPluginRepository>().InstancePerLifetimeScope();
        builder.RegisterType<SkyrimPluginExtensionImporter>().As<IPluginExtensionImporter>().InstancePerLifetimeScope();
        builder.RegisterType<SkyrimPluginReader>()
            .AsSelf()
            .Keyed<IGamePluginReader>(SupportedGame.Skyrim)
            .SingleInstance();
        builder.RegisterType<SkyrimRecordReader>()
            .AsSelf()
            .Keyed<IGameRecordReader>(SupportedGame.Skyrim)
            .SingleInstance();
        builder.RegisterType<GameImporter>()
            .WithParameter(new ResolvedParameter(
                (parameter, _) => parameter.ParameterType == typeof(IGamePluginReader),
                (_, context) => context.ResolveKeyed<IGamePluginReader>(SupportedGame.Skyrim)))
            .WithParameter(new ResolvedParameter(
                (parameter, _) => parameter.ParameterType == typeof(IGameRecordReader),
                (_, context) => context.ResolveKeyed<IGameRecordReader>(SupportedGame.Skyrim)))
            .As<IGameImporter>();
    }
}
