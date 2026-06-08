namespace CreationsForge.Migrations;

public interface IDatabaseMigrationRunner
{
    bool Migrate(string databasePath);
}
