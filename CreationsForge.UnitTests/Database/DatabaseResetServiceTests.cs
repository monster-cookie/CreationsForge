using CreationsForge.Core.Database;
using CreationsForge.Core.Database.Interfaces;
using NPoco;
using Shouldly;
using System.Data.SQLite;

namespace CreationsForge.UnitTests.Database;

public class DatabaseResetServiceTests
{
    [Fact]
    public void Reset_DeletesVerifiedCreationsForgeDatabaseAndSidecars()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var databasePath = Path.Combine(tempDirectory.FullName, "CreationsForge.sqlite");
            CreateCreationsForgeDatabase(databasePath);
            File.WriteAllText(databasePath + "-wal", "wal");
            File.WriteAllText(databasePath + "-shm", "shm");
            var resetService = new DatabaseResetService(new TestSqliteConnectionFactory(databasePath));

            resetService.Reset();

            File.Exists(databasePath).ShouldBeFalse();
            File.Exists(databasePath + "-wal").ShouldBeFalse();
            File.Exists(databasePath + "-shm").ShouldBeFalse();
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    [Fact]
    public void Reset_RejectsUnexpectedDatabaseFileName()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var databasePath = Path.Combine(tempDirectory.FullName, "Other.sqlite");
            File.WriteAllText(databasePath, "not sqlite");
            var resetService = new DatabaseResetService(new TestSqliteConnectionFactory(databasePath));

            Should.Throw<InvalidOperationException>(() => resetService.Reset())
                .Message.ShouldContain("unexpected database file");
            File.Exists(databasePath).ShouldBeTrue();
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    [Fact]
    public void Reset_RejectsUnverifiedCustomDatabase()
    {
        var tempDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var databasePath = Path.Combine(tempDirectory.FullName, "CreationsForge.sqlite");
            File.WriteAllText(databasePath, "not sqlite");
            var resetService = new DatabaseResetService(new TestSqliteConnectionFactory(databasePath));

            Should.Throw<InvalidOperationException>(() => resetService.Reset())
                .Message.ShouldContain("unverified database file");
            File.Exists(databasePath).ShouldBeTrue();
        }
        finally
        {
            tempDirectory.Delete(recursive: true);
        }
    }

    private static void CreateCreationsForgeDatabase(string databasePath)
    {
        var connectionString = new SQLiteConnectionStringBuilder
        {
            DataSource = databasePath,
            Pooling = false
        }.ToString();
        using var connection = new SQLiteConnection(connectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "CREATE TABLE Games (Game TEXT NOT NULL);";
        command.ExecuteNonQuery();
    }

    private sealed class TestSqliteConnectionFactory : ISqliteConnectionFactory
    {
        public TestSqliteConnectionFactory(string databasePath)
        {
            DatabasePath = databasePath;
        }

        public string DatabasePath { get; }

        public IDatabase OpenDatabase()
        {
            throw new NotImplementedException();
        }
    }
}
