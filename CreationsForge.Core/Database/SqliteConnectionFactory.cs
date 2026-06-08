using System.Data.SQLite;
using CreationsForge.Core.Database.Interfaces;
using CreationsForge.Core.Models.Database;
using NPoco;

namespace CreationsForge.Core.Database;

public class SqliteConnectionFactory : ISqliteConnectionFactory
{
    private readonly SqliteDatabaseOptions Options;

    public SqliteConnectionFactory(SqliteDatabaseOptions options)
    {
        Options = options;
    }

    public string DatabasePath => Options.DatabasePath;

    public IDatabase OpenDatabase()
    {
        Directory.CreateDirectory(Options.DatabaseDirectory);

        var connectionString = new SQLiteConnectionStringBuilder
        {
            DataSource = DatabasePath,
            ForeignKeys = true,
            JournalMode = SQLiteJournalModeEnum.Wal,
            Pooling = false
        }.ToString();

        var database = new NPoco.Database(connectionString, DatabaseType.SQLite, SQLiteFactory.Instance);
        database.Execute("PRAGMA foreign_keys = ON;");

        return database;
    }
}
