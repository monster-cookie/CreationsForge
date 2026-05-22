using System.Reflection;
using DbUp;

namespace SFRecordCompareEngine.Migrations;

public class DatabaseMigrationRunner : IDatabaseMigrationRunner
{
    public const int CurrentSchemaVersion = 1;

    public int Migrate(string databasePath)
    {
        SQLitePCL.Batteries.Init();

        var databaseDirectory = Path.GetDirectoryName(databasePath);
        if (!string.IsNullOrWhiteSpace(databaseDirectory))
        {
            Directory.CreateDirectory(databaseDirectory);
        }

        var connectionString = $"Data Source={databasePath};Pooling=False";

        var migrationAssembly = Assembly.GetExecutingAssembly();
        var upgradeEngine = DeployChanges.To
            .SqliteDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(
                migrationAssembly,
                resourceName => resourceName.Contains(".Sql.", StringComparison.OrdinalIgnoreCase))
            .Build();

        var result = upgradeEngine.PerformUpgrade();
        if (!result.Successful)
        {
            throw new InvalidOperationException("Database migration failed.", result.Error);
        }

        return CurrentSchemaVersion;
    }
}
