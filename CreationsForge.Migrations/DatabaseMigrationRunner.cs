using System.Reflection;
using System.Data.Common;
using DbUp;
using SQLitePCL;

namespace CreationsForge.Migrations;

public class DatabaseMigrationRunner : IDatabaseMigrationRunner
{
    public bool Migrate(string databasePath)
    {
        Batteries.Init();

        var databaseDirectory = Path.GetDirectoryName(databasePath);
        if (!string.IsNullOrWhiteSpace(databaseDirectory))
        {
            Directory.CreateDirectory(databaseDirectory);
        }

        var connectionString = new DbConnectionStringBuilder
        {
            ["Data Source"] = databasePath,
            ["Pooling"] = "False"
        }.ConnectionString;
        var migrationAssembly = Assembly.GetExecutingAssembly();
        var upgradeEngine = DeployChanges.To
            .SqliteDatabase(connectionString)
            .WithScriptsEmbeddedInAssembly(
                migrationAssembly,
                resourceName => resourceName.Contains(".Sql.", StringComparison.OrdinalIgnoreCase))
            .Build();

        var migrationsApplied = upgradeEngine.GetScriptsToExecute().Any();
        var result = upgradeEngine.PerformUpgrade();
        if (!result.Successful)
        {
            throw new InvalidOperationException("Database migration failed.", result.Error);
        }

        return migrationsApplied;
    }
}
