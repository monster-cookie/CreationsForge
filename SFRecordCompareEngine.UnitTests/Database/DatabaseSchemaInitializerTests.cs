using SFRecordCompareEngine.Core.Database;
using SFRecordCompareEngine.Core.Database.Interfaces;
using SFRecordCompareEngine.Core.Models.Database;
using SFRecordCompareEngine.Migrations;
using Shouldly;

namespace SFRecordCompareEngine.UnitTests.Database;

public class DatabaseSchemaInitializerTests : IDisposable
{
    private readonly string DatabaseDirectory = Path.Combine(Path.GetTempPath(), "SFRecordCompareEngineTests", Guid.NewGuid().ToString("N"));
    private readonly ISqliteConnectionFactory ConnectionFactory;
    private readonly DatabaseSchemaInitializer Sut;

    public DatabaseSchemaInitializerTests()
    {
        var options = new SqliteDatabaseOptions
        {
            DatabaseDirectory = DatabaseDirectory
        };

        ConnectionFactory = new SqliteConnectionFactory(options);
        Sut = new DatabaseSchemaInitializer(ConnectionFactory, new DatabaseMigrationRunner());
    }

    public void Dispose()
    {
        if (Directory.Exists(DatabaseDirectory))
        {
            Directory.Delete(DatabaseDirectory, true);
        }
    }

    [Fact]
    public void Initialize_WhenDatabaseDoesNotExist_CreatesPluginsTable()
    {
        Sut.Initialize();

        using var database = ConnectionFactory.OpenDatabase();
        var count = database.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = @0;",
            "Plugins");

        count.ShouldBe(1);
    }

    [Fact]
    public void Initialize_WhenDatabaseDoesNotExist_CreatesPluginMasterReferencesTable()
    {
        Sut.Initialize();

        using var database = ConnectionFactory.OpenDatabase();
        var count = database.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = @0;",
            "PluginMasterReferences");

        count.ShouldBe(1);
    }

    [Fact]
    public void Initialize_WhenDatabaseDoesNotExist_CreatesRecordHeaderTable()
    {
        Sut.Initialize();

        using var database = ConnectionFactory.OpenDatabase();
        var count = database.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = @0;",
            "RecordHeader");

        count.ShouldBe(1);
    }

    [Fact]
    public void Initialize_WhenDatabaseDoesNotExist_CreatesFormListTable()
    {
        Sut.Initialize();

        using var database = ConnectionFactory.OpenDatabase();
        var count = database.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = @0;",
            "FormList");

        count.ShouldBe(1);
    }

    [Fact]
    public void Initialize_WhenDatabaseDoesNotExist_CreatesFormListItemTable()
    {
        Sut.Initialize();

        using var database = ConnectionFactory.OpenDatabase();
        var count = database.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = @0;",
            "FormListItem");

        count.ShouldBe(1);
    }

    [Fact]
    public void Initialize_WhenDatabaseDoesNotExist_CreatesDbUpJournalTable()
    {
        Sut.Initialize();

        using var database = ConnectionFactory.OpenDatabase();
        var count = database.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = @0;",
            "SchemaVersions");

        count.ShouldBe(1);
    }

    [Fact]
    public void Initialize_WhenDatabaseDoesNotExist_RecordsInitialMigration()
    {
        Sut.Initialize();

        using var database = ConnectionFactory.OpenDatabase();
        var count = database.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM SchemaVersions WHERE ScriptName LIKE @0;",
            "%001_CreatePluginSchema.sql");

        count.ShouldBe(1);
    }

    [Fact]
    public void Initialize_WhenDatabaseDoesNotExist_RecordsFormListMigration()
    {
        Sut.Initialize();

        using var database = ConnectionFactory.OpenDatabase();
        var count = database.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM SchemaVersions WHERE ScriptName LIKE @0;",
            "%002_CreateRecordHeaderAndFormListSchema.sql");

        count.ShouldBe(1);
    }

    [Fact]
    public void Initialize_WhenCalledTwice_DoesNotFail()
    {
        Sut.Initialize();

        Should.NotThrow(() => Sut.Initialize());
    }

    [Fact]
    public void OpenDatabase_WhenConnectionIsOpened_EnablesForeignKeys()
    {
        Sut.Initialize();

        using var database = ConnectionFactory.OpenDatabase();
        var foreignKeysEnabled = database.ExecuteScalar<int>("PRAGMA foreign_keys;");

        foreignKeysEnabled.ShouldBe(1);
    }

    [Fact]
    public void FormList_WhenRecordHeaderDoesNotExist_ForeignKeyPreventsInsert()
    {
        Sut.Initialize();

        using var database = ConnectionFactory.OpenDatabase();

        Should.Throw<Exception>(() => database.Execute(
            """
            INSERT INTO FormList (ModKey, FormID, AddToListFormKey, ImportedAtUtc)
            VALUES (@0, @1, NULL, @2);
            """,
            "Example.esm",
            "000001",
            DateTimeOffset.UtcNow.ToString("O")));
    }

    [Fact]
    public void FormListItem_WhenFormListDoesNotExist_ForeignKeyPreventsInsert()
    {
        Sut.Initialize();

        using var database = ConnectionFactory.OpenDatabase();

        Should.Throw<Exception>(() => database.Execute(
            """
            INSERT INTO FormListItem (ModKey, FormID, ItemIndex, ItemFormKey, ImportedAtUtc)
            VALUES (@0, @1, @2, @3, @4);
            """,
            "Example.esm",
            "000001",
            0,
            "000002:Example.esm",
            DateTimeOffset.UtcNow.ToString("O")));
    }

    [Fact]
    public void RecordHeader_WhenDeleted_CascadesToFormListAndItems()
    {
        Sut.Initialize();

        using var database = ConnectionFactory.OpenDatabase();
        var importedAtUtc = DateTimeOffset.UtcNow.ToString("O");
        database.Execute(
            """
            INSERT INTO Plugins (ModKey, GameRelease, LoadOrderIndex, PluginFileName, LastCheckedUtc)
            VALUES (@0, @1, @2, @3, @4);
            """,
            "Example.esm",
            "Starfield",
            1,
            "Example.esm",
            importedAtUtc);
        database.Execute(
            """
            INSERT INTO RecordHeader (ModKey, FormID, RecordType, FormKey, PluginFileName, ImportedAtUtc)
            VALUES (@0, @1, @2, @3, @4, @5);
            """,
            "Example.esm",
            "000001",
            "FormList",
            "000001:Example.esm",
            "Example.esm",
            importedAtUtc);
        database.Execute(
            """
            INSERT INTO FormList (ModKey, FormID, AddToListFormKey, ImportedAtUtc)
            VALUES (@0, @1, NULL, @2);
            """,
            "Example.esm",
            "000001",
            importedAtUtc);
        database.Execute(
            """
            INSERT INTO FormListItem (ModKey, FormID, ItemIndex, ItemFormKey, ImportedAtUtc)
            VALUES (@0, @1, @2, @3, @4);
            """,
            "Example.esm",
            "000001",
            0,
            "000002:Example.esm",
            importedAtUtc);

        database.Execute("DELETE FROM RecordHeader WHERE ModKey = @0 AND FormID = @1;", "Example.esm", "000001");

        database.ExecuteScalar<int>("SELECT COUNT(*) FROM FormList;").ShouldBe(0);
        database.ExecuteScalar<int>("SELECT COUNT(*) FROM FormListItem;").ShouldBe(0);
    }
}
