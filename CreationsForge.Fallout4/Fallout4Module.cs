using Autofac;
using Autofac.Core;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Importers;
using CreationsForge.Core.Importers.Interfaces;
using CreationsForge.Fallout4.Interfaces;
using CreationsForge.Fallout4.Importers;
using CreationsForge.Fallout4.Repositories;
using CreationsForge.Fallout4.Repositories.Interfaces;
using Module = Autofac.Module;

namespace CreationsForge.Fallout4;

public class Fallout4Module : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<Fallout4GameMetadataService>().AsSelf().AsImplementedInterfaces().SingleInstance();
        builder.RegisterType<Fallout4PluginReaderService>().As<IFallout4PluginReaderService>().SingleInstance();
        builder.RegisterType<Fallout4RecordReaderService>().As<IFallout4RecordReaderService>().SingleInstance();
        builder.RegisterType<Fallout4PluginRepository>().As<IFallout4PluginRepository>().InstancePerLifetimeScope();
        builder.RegisterType<Fallout4PluginExtensionImporter>().As<IPluginExtensionImporter>().InstancePerLifetimeScope();
        builder.RegisterType<Fallout4PluginReader>()
            .AsSelf()
            .Keyed<IGamePluginReader>(SupportedGame.Fallout4)
            .SingleInstance();
        builder.RegisterType<Fallout4RecordReader>()
            .AsSelf()
            .Keyed<IGameRecordReader>(SupportedGame.Fallout4)
            .SingleInstance();
        builder.RegisterType<GameImporter>()
            .WithParameter(new ResolvedParameter(
                (parameter, _) => parameter.ParameterType == typeof(IGamePluginReader),
                (_, context) => context.ResolveKeyed<IGamePluginReader>(SupportedGame.Fallout4)))
            .WithParameter(new ResolvedParameter(
                (parameter, _) => parameter.ParameterType == typeof(IGameRecordReader),
                (_, context) => context.ResolveKeyed<IGameRecordReader>(SupportedGame.Fallout4)))
            .As<IGameImporter>();
    }
}
