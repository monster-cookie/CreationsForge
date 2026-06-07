using Autofac;
using CreationsForge.Core.Importers;
using CreationsForge.Core.Importers.Interfaces;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services.Interfaces;
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
        builder.RegisterType<StarfieldPluginReader>().AsSelf().As<IGamePluginReader>().SingleInstance();
        builder.RegisterType<StarfieldRecordReader>().AsSelf().As<IGameRecordReader>().SingleInstance();
        builder.Register(c => new GameImporter(
                c.Resolve<StarfieldPluginReader>(),
                c.Resolve<StarfieldRecordReader>(),
                c.Resolve<IGameRepository>(),
                c.Resolve<IPluginRepository>(),
                c.Resolve<IPluginMasterReferenceRepository>(),
                c.Resolve<IEnumerable<IPluginExtensionImporter>>(),
                c.Resolve<IRecordImportService>(),
                c.Resolve<NPoco.IDatabase>()))
            .As<IGameImporter>();
    }
}
