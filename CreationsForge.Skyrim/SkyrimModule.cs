using Autofac;
using CreationsForge.Core.Importers;
using CreationsForge.Core.Importers.Interfaces;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services.Interfaces;
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
        builder.RegisterType<SkyrimPluginReader>().AsSelf().As<IGamePluginReader>().SingleInstance();
        builder.RegisterType<SkyrimRecordReader>().AsSelf().As<IGameRecordReader>().SingleInstance();
        builder.Register(c => new GameImporter(
                c.Resolve<SkyrimPluginReader>(),
                c.Resolve<SkyrimRecordReader>(),
                c.Resolve<IGameRepository>(),
                c.Resolve<IPluginRepository>(),
                c.Resolve<IPluginMasterReferenceRepository>(),
                c.Resolve<IEnumerable<IPluginExtensionImporter>>(),
                c.Resolve<IRecordImportService>(),
                c.Resolve<NPoco.IDatabase>()))
            .As<IGameImporter>();
    }
}
