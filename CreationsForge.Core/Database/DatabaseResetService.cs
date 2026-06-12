using CreationsForge.Core.Database.Interfaces;
using CreationsForge.Core.Configuration;
using Serilog;
using System.Data.SQLite;

namespace CreationsForge.Core.Database;

public class DatabaseResetService : IDatabaseResetService
{
    private const string DatabaseFileName = "CreationsForge.sqlite";
    private readonly ISqliteConnectionFactory ConnectionFactory;
    private readonly ILogger Logger = Log.ForContext<DatabaseResetService>();

    public DatabaseResetService(ISqliteConnectionFactory connectionFactory)
    {
        ConnectionFactory = connectionFactory;
    }

    public void Reset()
    {
        var databasePath = Path.GetFullPath(ConnectionFactory.DatabasePath);
        ValidateResetTarget(databasePath);
        Logger.Information("Resetting application database at {DatabasePath}", databasePath);
        DeleteIfExists(databasePath);
        DeleteIfExists(databasePath + "-wal");
        DeleteIfExists(databasePath + "-shm");
    }

    private void ValidateResetTarget(string databasePath)
    {
        if (!string.Equals(Path.GetFileName(databasePath), DatabaseFileName, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Database reset refused to delete unexpected database file '{databasePath}'.");
        }

        if (IsUnderDefaultApplicationDataDirectory(databasePath))
        {
            return;
        }

        if (File.Exists(databasePath) && IsCreationsForgeDatabase(databasePath))
        {
            return;
        }

        throw new InvalidOperationException($"Database reset refused to delete unverified database file '{databasePath}'.");
    }

    private static bool IsUnderDefaultApplicationDataDirectory(string databasePath)
    {
        var defaultDirectory = Path.GetFullPath(ApplicationConfigurationStore.DefaultApplicationDataDirectory);
        if (!defaultDirectory.EndsWith(Path.DirectorySeparatorChar))
        {
            defaultDirectory += Path.DirectorySeparatorChar;
        }

        return databasePath.StartsWith(defaultDirectory, StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsCreationsForgeDatabase(string databasePath)
    {
        try
        {
            var connectionString = new SQLiteConnectionStringBuilder
            {
                DataSource = databasePath,
                ReadOnly = true,
                Pooling = false
            }.ToString();
            using var connection = new SQLiteConnection(connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT COUNT(*)
                FROM sqlite_master
                WHERE type = 'table'
                  AND name IN ('Games', 'Plugins', 'RecordInstances', 'SchemaVersions');
                """;
            return Convert.ToInt32(command.ExecuteScalar()) > 0;
        }
        catch (SQLiteException)
        {
            return false;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }

    private static void DeleteIfExists(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}
