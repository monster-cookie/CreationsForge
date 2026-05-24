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
    public void Initialize_WhenDatabaseDoesNotExist_CreatesGameSettingTable()
    {
        Sut.Initialize();

        using var database = ConnectionFactory.OpenDatabase();
        var count = database.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = @0;",
            "GameSetting");

        count.ShouldBe(1);
    }

    [Theory]
    [InlineData("Cell")]
    [InlineData("CellGroupLocation")]
    [InlineData("CellPlacedRecord")]
    [InlineData("Worldspace")]
    public void Initialize_WhenDatabaseDoesNotExist_CreatesCellAndWorldspaceTables(string tableName)
    {
        Sut.Initialize();

        using var database = ConnectionFactory.OpenDatabase();
        var count = database.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = @0;",
            tableName);

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
    public void Initialize_WhenDatabaseDoesNotExist_RecordsSimpleMajorRecordDetailMigration()
    {
        Sut.Initialize();

        using var database = ConnectionFactory.OpenDatabase();
        var count = database.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM SchemaVersions WHERE ScriptName LIKE @0;",
            "%002_AddSimpleMajorRecordDetailTables.sql");

        count.ShouldBe(1);
    }

    [Fact]
    public void Initialize_WhenDatabaseDoesNotExist_RecordsFactionDetailMigration()
    {
        Sut.Initialize();

        using var database = ConnectionFactory.OpenDatabase();
        var count = database.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM SchemaVersions WHERE ScriptName LIKE @0;",
            "%003_ExtendFactionDetailFields.sql");

        count.ShouldBe(1);
    }

    [Fact]
    public void Initialize_WhenDatabaseDoesNotExist_RecordsKeywordDetailMigration()
    {
        Sut.Initialize();

        using var database = ConnectionFactory.OpenDatabase();
        var count = database.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM SchemaVersions WHERE ScriptName LIKE @0;",
            "%004_ExtendKeywordDetailFields.sql");

        count.ShouldBe(1);
    }

    [Theory]
    [InlineData("Keyword")]
    [InlineData("Faction")]
    [InlineData("FactionRelation")]
    [InlineData("Message")]
    [InlineData("GameplayOptionsGroup")]
    [InlineData("Static")]
    [InlineData("StaticCollection")]
    [InlineData("Activator")]
    [InlineData("ActivatorKeyword")]
    [InlineData("MiscItem")]
    [InlineData("MiscItemKeyword")]
    [InlineData("GameplayOption")]
    [InlineData("GameplayOptionKeyword")]
    [InlineData("MagicEffect")]
    [InlineData("MagicEffectKeyword")]
    public void Initialize_WhenDatabaseDoesNotExist_CreatesSimpleMajorRecordDetailTables(string tableName)
    {
        Sut.Initialize();

        using var database = ConnectionFactory.OpenDatabase();
        var count = database.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = @0;",
            tableName);

        count.ShouldBe(1);
    }

    [Fact]
    public void Plugins_WhenImportStateIsUnsupported_AllowsInsert()
    {
        Sut.Initialize();

        using var database = ConnectionFactory.OpenDatabase();

        Should.NotThrow(() => database.Execute(
            """
            INSERT INTO Plugins (ModKey, GameRelease, LoadOrderIndex, PluginFileName, ImportState, LastCheckedUtc)
            VALUES (@0, @1, @2, @3, @4, @5);
            """,
            "BlueprintShips.esm",
            "Starfield",
            1,
            "BlueprintShips.esm",
            "Unsupported",
            DateTimeOffset.UtcNow.ToString("O")));
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

    [Fact]
    public void RecordHeader_WhenDeleted_CascadesToGameSetting()
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
            "GameSetting",
            "000001:Example.esm",
            "Example.esm",
            importedAtUtc);
        database.Execute(
            """
            INSERT INTO GameSetting (ModKey, FormID, SettingType, Data, ImportedAtUtc)
            VALUES (@0, @1, @2, @3, @4);
            """,
            "Example.esm",
            "000001",
            "String",
            "Value",
            importedAtUtc);

        database.Execute("DELETE FROM RecordHeader WHERE ModKey = @0 AND FormID = @1;", "Example.esm", "000001");

        database.ExecuteScalar<int>("SELECT COUNT(*) FROM GameSetting;").ShouldBe(0);
    }

    [Fact]
    public void RecordHeader_WhenDeleted_CascadesToSimpleDetailAndKeywordRows()
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
            "Activator",
            "000001:Example.esm",
            "Example.esm",
            importedAtUtc);
        database.Execute(
            """
            INSERT INTO Activator (ModKey, FormID, Name, ImportedAtUtc)
            VALUES (@0, @1, @2, @3);
            """,
            "Example.esm",
            "000001",
            "Example Activator",
            importedAtUtc);
        database.Execute(
            """
            INSERT INTO ActivatorKeyword (ModKey, FormID, ItemIndex, KeywordFormKey, ImportedAtUtc)
            VALUES (@0, @1, @2, @3, @4);
            """,
            "Example.esm",
            "000001",
            0,
            "000100:Example.esm",
            importedAtUtc);

        database.Execute("DELETE FROM RecordHeader WHERE ModKey = @0 AND FormID = @1;", "Example.esm", "000001");

        database.ExecuteScalar<int>("SELECT COUNT(*) FROM Activator;").ShouldBe(0);
        database.ExecuteScalar<int>("SELECT COUNT(*) FROM ActivatorKeyword;").ShouldBe(0);
    }

    [Fact]
    public void RecordHeader_WhenDeleted_CascadesToFactionAndRelationRows()
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
            "Faction",
            "000001:Example.esm",
            "Example.esm",
            importedAtUtc);
        database.Execute(
            """
            INSERT INTO Faction (ModKey, FormID, Name, ImportedAtUtc)
            VALUES (@0, @1, @2, @3);
            """,
            "Example.esm",
            "000001",
            "Example Faction",
            importedAtUtc);
        database.Execute(
            """
            INSERT INTO FactionRelation (ModKey, FormID, ItemIndex, TargetFormKey, Reaction, ImportedAtUtc)
            VALUES (@0, @1, @2, @3, @4, @5);
            """,
            "Example.esm",
            "000001",
            0,
            "000100:Example.esm",
            "Enemy",
            importedAtUtc);

        database.Execute("DELETE FROM RecordHeader WHERE ModKey = @0 AND FormID = @1;", "Example.esm", "000001");

        database.ExecuteScalar<int>("SELECT COUNT(*) FROM Faction;").ShouldBe(0);
        database.ExecuteScalar<int>("SELECT COUNT(*) FROM FactionRelation;").ShouldBe(0);
    }
}
