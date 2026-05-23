namespace SFRecordCompareEngine.Migrations;

public interface IDatabaseMigrationRunner
{
    void Migrate(string databasePath);
}
