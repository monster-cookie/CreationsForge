using NPoco;
using SFRecordCompareEngine.Core.Database.Interfaces;
using System.IO;
using System.Data.SQLite;
using SFRecordCompareEngine.Core.Models.Database;

namespace SFRecordCompareEngine.Core.Database;

public class SqliteConnectionFactory(SqliteDatabaseOptions options) : ISqliteConnectionFactory
{
    public string DatabasePath => options.DatabasePath;

    public IDatabase OpenDatabase()
    {
        Directory.CreateDirectory(options.DatabaseDirectory);

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
