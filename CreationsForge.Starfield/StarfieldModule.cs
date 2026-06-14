using Autofac;
using Autofac.Core;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Importers;
using CreationsForge.Core.Importers.Interfaces;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services.Interfaces;
using CreationsForge.Starfield.Interfaces;
using CreationsForge.Starfield.Importers;
using CreationsForge.Starfield.Repositories;
using CreationsForge.Starfield.Repositories.Interfaces;
using NPoco;
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
        RegisterModelRecordSupport(builder, RecordTypeCatalog.Book.RecordID);
        RegisterModelRecordSupport(builder, RecordTypeCatalog.Door.RecordID);
        RegisterModelRecordSupport(builder, RecordTypeCatalog.Terminal.RecordID);
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

    private static void RegisterModelRecordSupport(ContainerBuilder builder, string recordType)
    {
        builder.Register(context => new StarfieldModelRecordImporter(
                recordType,
                context.Resolve<IRecordInstanceRepository>(),
                context.Resolve<IModelImportService>()))
            .As<ITypedRecordImporter>()
            .InstancePerLifetimeScope();
        builder.Register(context => new StarfieldModelRecordTreeRepository(
                recordType,
                context.Resolve<IDatabase>()))
            .As<IRecordTreeRepository>()
            .InstancePerLifetimeScope();
    }
}
