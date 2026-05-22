namespace SFRecordCompareEngine.Migrations;

public interface IDatabaseMigrationRunner
{
    int Migrate(string databasePath);
}
