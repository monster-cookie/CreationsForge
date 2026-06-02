namespace SFRecordCompareEngine.Migrations;

public interface IDatabaseMigrationRunner
{
    bool Migrate(string databasePath);
}
