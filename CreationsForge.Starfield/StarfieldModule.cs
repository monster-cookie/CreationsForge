using Autofac;
using Autofac.Core;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Importers;
using CreationsForge.Core.Importers.Interfaces;
using CreationsForge.Starfield.Interfaces;
using CreationsForge.Starfield.Importers;
using CreationsForge.Starfield.Repositories;
using CreationsForge.Starfield.Repositories.Interfaces;
using Module = Autofac.Module;

namespace CreationsForge.Starfield;

public class StarfieldModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<StarfieldGameMetadataService>().AsSelf().AsImplementedInterfaces().SingleInstance();
        builder.RegisterType<StarfieldPluginReaderService>().As<IStarfieldPluginReaderService>().SingleInstance();
        builder.RegisterType<StarfieldRecordReaderService>().As<IStarfieldRecordReaderService>().SingleInstance();
        builder.RegisterType<StarfieldPluginRepository>().As<IStarfieldPluginRepository>().InstancePerLifetimeScope();
        builder.RegisterType<StarfieldPluginExtensionImporter>().As<IPluginExtensionImporter>().InstancePerLifetimeScope();
        builder.RegisterType<StarfieldPluginReader>()
            .AsSelf()
            .Keyed<IGamePluginReader>(SupportedGame.Starfield)
            .SingleInstance();
        builder.RegisterType<StarfieldRecordReader>()
            .AsSelf()
            .Keyed<IGameRecordReader>(SupportedGame.Starfield)
            .SingleInstance();
        builder.RegisterType<GameImporter>()
            .WithParameter(new ResolvedParameter(
                (parameter, _) => parameter.ParameterType == typeof(IGamePluginReader),
                (_, context) => context.ResolveKeyed<IGamePluginReader>(SupportedGame.Starfield)))
            .WithParameter(new ResolvedParameter(
                (parameter, _) => parameter.ParameterType == typeof(IGameRecordReader),
                (_, context) => context.ResolveKeyed<IGameRecordReader>(SupportedGame.Starfield)))
            .As<IGameImporter>();
    }
}
