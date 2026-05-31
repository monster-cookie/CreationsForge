using Autofac;

namespace SFRecordCompareEngine.Migrations;

public class MigrationsModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<DatabaseMigrationRunner>()
            .As<IDatabaseMigrationRunner>()
            .SingleInstance();
    }
}