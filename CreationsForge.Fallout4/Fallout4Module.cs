using Autofac;
using CreationsForge.Core.Importers;
using CreationsForge.Core.Importers.Interfaces;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services.Interfaces;
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
        builder.RegisterType<Fallout4PluginReader>().AsSelf().As<IGamePluginReader>().SingleInstance();
        builder.RegisterType<Fallout4RecordReader>().AsSelf().As<IGameRecordReader>().SingleInstance();
        builder.Register(c => new GameImporter(
                c.Resolve<Fallout4PluginReader>(),
                c.Resolve<Fallout4RecordReader>(),
                c.Resolve<IGameRepository>(),
                c.Resolve<IPluginRepository>(),
                c.Resolve<IPluginMasterReferenceRepository>(),
                c.Resolve<IEnumerable<IPluginExtensionImporter>>(),
                c.Resolve<IRecordImportService>(),
                c.Resolve<NPoco.IDatabase>()))
            .As<IGameImporter>();
    }
}
