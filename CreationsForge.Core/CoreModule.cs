using System.Reflection;
using Autofac;
using CreationsForge.Bethesda.Assets.Archives;
using CreationsForge.Bethesda.Assets.Archives.Ba2;
using CreationsForge.Bethesda.Assets.Archives.Bsa;
using CreationsForge.Bethesda.Assets.Nif;
using CreationsForge.Bethesda.Assets.Resources;
using CreationsForge.Core.Configuration;
using CreationsForge.Core.Configuration.Interfaces;
using CreationsForge.Core.Database;
using CreationsForge.Core.Database.Interfaces;
using CreationsForge.Core.Importers;
using CreationsForge.Core.Importers.Interfaces;
using CreationsForge.Core.Models.Database;
using NPoco;
using Module = Autofac.Module;

namespace CreationsForge.Core;

public class CoreModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<ApplicationConfigurationStore>()
            .As<IApplicationConfigurationStore>()
            .SingleInstance();

        builder.RegisterType<SqliteDatabaseOptions>().SingleInstance();
        builder.RegisterType<SqliteConnectionFactory>()
            .As<ISqliteConnectionFactory>()
            .SingleInstance();
        builder.Register(c => c.Resolve<ISqliteConnectionFactory>().OpenDatabase())
            .As<IDatabase>()
            .InstancePerLifetimeScope();

        builder.RegisterType<BethesdaAssetProvider>()
            .As<IBethesdaAssetProvider>()
            .InstancePerLifetimeScope();

        builder.RegisterType<Ba2ArchiveReader>()
            .As<IAssetArchiveReader>()
            .InstancePerLifetimeScope();

        builder.RegisterType<BsaArchiveReader>()
            .As<IAssetArchiveReader>()
            .InstancePerLifetimeScope();

        builder.RegisterType<NifPreviewModelReader>()
            .As<INifPreviewModelReader>()
            .InstancePerLifetimeScope();

        builder.RegisterType<GameImportDispatcher>().InstancePerLifetimeScope();

        builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
            .Where(t => t.Name.EndsWith("Importer", StringComparison.OrdinalIgnoreCase) && t != typeof(GameImporter))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();

        builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
            .Where(t => t.Name.EndsWith("Service", StringComparison.OrdinalIgnoreCase))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();

        builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
            .Where(t => t.Name.EndsWith("Initializer", StringComparison.OrdinalIgnoreCase))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();

        builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
            .Where(t => t.Name.EndsWith("Repository", StringComparison.OrdinalIgnoreCase))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();
    }
}
