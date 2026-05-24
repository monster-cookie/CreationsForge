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
            "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = @TableName;",
            new { TableName = "Plugins" });

        count.ShouldBe(1);
    }

    [Fact]
    public void Initialize_WhenDatabaseDoesNotExist_CreatesPluginMasterReferencesTable()
    {
        Sut.Initialize();

        using var database = ConnectionFactory.OpenDatabase();
        var count = database.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = @TableName;",
            new { TableName = "PluginMasterReferences" });

        count.ShouldBe(1);
    }

    [Fact]
    public void Initialize_WhenDatabaseDoesNotExist_CreatesRecordHeaderTable()
    {
        Sut.Initialize();

        using var database = ConnectionFactory.OpenDatabase();
        var count = database.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = @TableName;",
            new { TableName = "RecordHeader" });

        count.ShouldBe(1);
    }

    [Fact]
    public void Initialize_WhenDatabaseDoesNotExist_CreatesFormListTable()
    {
        Sut.Initialize();

        using var database = ConnectionFactory.OpenDatabase();
        var count = database.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = @TableName;",
            new { TableName = "FormList" });

        count.ShouldBe(1);
    }

    [Fact]
    public void Initialize_WhenDatabaseDoesNotExist_CreatesFormListItemTable()
    {
        Sut.Initialize();

        using var database = ConnectionFactory.OpenDatabase();
        var count = database.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = @TableName;",
            new { TableName = "FormListItem" });

        count.ShouldBe(1);
    }

    [Fact]
    public void Initialize_WhenDatabaseDoesNotExist_CreatesGameSettingTable()
    {
        Sut.Initialize();

        using var database = ConnectionFactory.OpenDatabase();
        var count = database.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = @TableName;",
            new { TableName = "GameSetting" });

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
            "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = @TableName;",
            new { TableName = tableName });

        count.ShouldBe(1);
    }

    [Fact]
    public void Initialize_WhenDatabaseDoesNotExist_CreatesDbUpJournalTable()
    {
        Sut.Initialize();

        using var database = ConnectionFactory.OpenDatabase();
        var count = database.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = @TableName;",
            new { TableName = "SchemaVersions" });

        count.ShouldBe(1);
    }

    [Fact]
    public void Initialize_WhenDatabaseDoesNotExist_RecordsInitialMigration()
    {
        Sut.Initialize();

        using var database = ConnectionFactory.OpenDatabase();
        var count = database.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM SchemaVersions WHERE ScriptName LIKE @ScriptNamePattern;",
            new { ScriptNamePattern = "%001_CreatePluginSchema.sql" });

        count.ShouldBe(1);
    }

    [Fact]
    public void Initialize_WhenDatabaseDoesNotExist_RecordsSimpleMajorRecordDetailMigration()
    {
        Sut.Initialize();

        using var database = ConnectionFactory.OpenDatabase();
        var count = database.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM SchemaVersions WHERE ScriptName LIKE @ScriptNamePattern;",
            new { ScriptNamePattern = "%002_AddSimpleMajorRecordDetailTables.sql" });

        count.ShouldBe(1);
    }

    [Fact]
    public void Initialize_WhenDatabaseDoesNotExist_RecordsFactionDetailMigration()
    {
        Sut.Initialize();

        using var database = ConnectionFactory.OpenDatabase();
        var count = database.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM SchemaVersions WHERE ScriptName LIKE @ScriptNamePattern;",
            new { ScriptNamePattern = "%003_ExtendFactionDetailFields.sql" });

        count.ShouldBe(1);
    }

    [Fact]
    public void Initialize_WhenDatabaseDoesNotExist_RecordsKeywordDetailMigration()
    {
        Sut.Initialize();

        using var database = ConnectionFactory.OpenDatabase();
        var count = database.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM SchemaVersions WHERE ScriptName LIKE @ScriptNamePattern;",
            new { ScriptNamePattern = "%004_ExtendKeywordDetailFields.sql" });

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
            "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = @TableName;",
            new { TableName = tableName });

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
            VALUES (@ModKey, @GameRelease, @LoadOrderIndex, @PluginFileName, @ImportState, @LastCheckedUtc);
            """,
            new
            {
                ModKey = "BlueprintShips.esm",
                GameRelease = "Starfield",
                LoadOrderIndex = 1,
                PluginFileName = "BlueprintShips.esm",
                ImportState = "Unsupported",
                LastCheckedUtc = DateTimeOffset.UtcNow.ToString("O")
            }));
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
            VALUES (@ModKey, @FormID, NULL, @ImportedAtUtc);
            """,
            new
            {
                ModKey = "Example.esm",
                FormID = "000001",
                ImportedAtUtc = DateTimeOffset.UtcNow.ToString("O")
            }));
    }

    [Fact]
    public void FormListItem_WhenFormListDoesNotExist_ForeignKeyPreventsInsert()
    {
        Sut.Initialize();

        using var database = ConnectionFactory.OpenDatabase();

        Should.Throw<Exception>(() => database.Execute(
            """
            INSERT INTO FormListItem (ModKey, FormID, ItemIndex, ItemFormKey, ImportedAtUtc)
            VALUES (@ModKey, @FormID, @ItemIndex, @ItemFormKey, @ImportedAtUtc);
            """,
            new
            {
                ModKey = "Example.esm",
                FormID = "000001",
                ItemIndex = 0,
                ItemFormKey = "000002:Example.esm",
                ImportedAtUtc = DateTimeOffset.UtcNow.ToString("O")
            }));
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
            VALUES (@ModKey, @GameRelease, @LoadOrderIndex, @PluginFileName, @LastCheckedUtc);
            """,
            new { ModKey = "Example.esm", GameRelease = "Starfield", LoadOrderIndex = 1, PluginFileName = "Example.esm", LastCheckedUtc = importedAtUtc });
        database.Execute(
            """
            INSERT INTO RecordHeader (ModKey, FormID, RecordType, FormKey, PluginFileName, ImportedAtUtc)
            VALUES (@ModKey, @FormID, @RecordType, @FormKey, @PluginFileName, @ImportedAtUtc);
            """,
            new { ModKey = "Example.esm", FormID = "000001", RecordType = "FormList", FormKey = "000001:Example.esm", PluginFileName = "Example.esm", ImportedAtUtc = importedAtUtc });
        database.Execute(
            """
            INSERT INTO FormList (ModKey, FormID, AddToListFormKey, ImportedAtUtc)
            VALUES (@ModKey, @FormID, NULL, @ImportedAtUtc);
            """,
            new { ModKey = "Example.esm", FormID = "000001", ImportedAtUtc = importedAtUtc });
        database.Execute(
            """
            INSERT INTO FormListItem (ModKey, FormID, ItemIndex, ItemFormKey, ImportedAtUtc)
            VALUES (@ModKey, @FormID, @ItemIndex, @ItemFormKey, @ImportedAtUtc);
            """,
            new { ModKey = "Example.esm", FormID = "000001", ItemIndex = 0, ItemFormKey = "000002:Example.esm", ImportedAtUtc = importedAtUtc });

        database.Execute("DELETE FROM RecordHeader WHERE ModKey = @ModKey AND FormID = @FormID;", new { ModKey = "Example.esm", FormID = "000001" });

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
            VALUES (@ModKey, @GameRelease, @LoadOrderIndex, @PluginFileName, @LastCheckedUtc);
            """,
            new { ModKey = "Example.esm", GameRelease = "Starfield", LoadOrderIndex = 1, PluginFileName = "Example.esm", LastCheckedUtc = importedAtUtc });
        database.Execute(
            """
            INSERT INTO RecordHeader (ModKey, FormID, RecordType, FormKey, PluginFileName, ImportedAtUtc)
            VALUES (@ModKey, @FormID, @RecordType, @FormKey, @PluginFileName, @ImportedAtUtc);
            """,
            new { ModKey = "Example.esm", FormID = "000001", RecordType = "GameSetting", FormKey = "000001:Example.esm", PluginFileName = "Example.esm", ImportedAtUtc = importedAtUtc });
        database.Execute(
            """
            INSERT INTO GameSetting (ModKey, FormID, SettingType, Data, ImportedAtUtc)
            VALUES (@ModKey, @FormID, @SettingType, @Data, @ImportedAtUtc);
            """,
            new { ModKey = "Example.esm", FormID = "000001", SettingType = "String", Data = "Value", ImportedAtUtc = importedAtUtc });

        database.Execute("DELETE FROM RecordHeader WHERE ModKey = @ModKey AND FormID = @FormID;", new { ModKey = "Example.esm", FormID = "000001" });

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
            VALUES (@ModKey, @GameRelease, @LoadOrderIndex, @PluginFileName, @LastCheckedUtc);
            """,
            new { ModKey = "Example.esm", GameRelease = "Starfield", LoadOrderIndex = 1, PluginFileName = "Example.esm", LastCheckedUtc = importedAtUtc });
        database.Execute(
            """
            INSERT INTO RecordHeader (ModKey, FormID, RecordType, FormKey, PluginFileName, ImportedAtUtc)
            VALUES (@ModKey, @FormID, @RecordType, @FormKey, @PluginFileName, @ImportedAtUtc);
            """,
            new { ModKey = "Example.esm", FormID = "000001", RecordType = "Activator", FormKey = "000001:Example.esm", PluginFileName = "Example.esm", ImportedAtUtc = importedAtUtc });
        database.Execute(
            """
            INSERT INTO Activator (ModKey, FormID, Name, ImportedAtUtc)
            VALUES (@ModKey, @FormID, @Name, @ImportedAtUtc);
            """,
            new { ModKey = "Example.esm", FormID = "000001", Name = "Example Activator", ImportedAtUtc = importedAtUtc });
        database.Execute(
            """
            INSERT INTO ActivatorKeyword (ModKey, FormID, ItemIndex, KeywordFormKey, ImportedAtUtc)
            VALUES (@ModKey, @FormID, @ItemIndex, @KeywordFormKey, @ImportedAtUtc);
            """,
            new { ModKey = "Example.esm", FormID = "000001", ItemIndex = 0, KeywordFormKey = "000100:Example.esm", ImportedAtUtc = importedAtUtc });

        database.Execute("DELETE FROM RecordHeader WHERE ModKey = @ModKey AND FormID = @FormID;", new { ModKey = "Example.esm", FormID = "000001" });

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
            VALUES (@ModKey, @GameRelease, @LoadOrderIndex, @PluginFileName, @LastCheckedUtc);
            """,
            new { ModKey = "Example.esm", GameRelease = "Starfield", LoadOrderIndex = 1, PluginFileName = "Example.esm", LastCheckedUtc = importedAtUtc });
        database.Execute(
            """
            INSERT INTO RecordHeader (ModKey, FormID, RecordType, FormKey, PluginFileName, ImportedAtUtc)
            VALUES (@ModKey, @FormID, @RecordType, @FormKey, @PluginFileName, @ImportedAtUtc);
            """,
            new { ModKey = "Example.esm", FormID = "000001", RecordType = "Faction", FormKey = "000001:Example.esm", PluginFileName = "Example.esm", ImportedAtUtc = importedAtUtc });
        database.Execute(
            """
            INSERT INTO Faction (ModKey, FormID, Name, ImportedAtUtc)
            VALUES (@ModKey, @FormID, @Name, @ImportedAtUtc);
            """,
            new { ModKey = "Example.esm", FormID = "000001", Name = "Example Faction", ImportedAtUtc = importedAtUtc });
        database.Execute(
            """
            INSERT INTO FactionRelation (ModKey, FormID, ItemIndex, TargetFormKey, Reaction, ImportedAtUtc)
            VALUES (@ModKey, @FormID, @ItemIndex, @TargetFormKey, @Reaction, @ImportedAtUtc);
            """,
            new { ModKey = "Example.esm", FormID = "000001", ItemIndex = 0, TargetFormKey = "000100:Example.esm", Reaction = "Enemy", ImportedAtUtc = importedAtUtc });

        database.Execute("DELETE FROM RecordHeader WHERE ModKey = @ModKey AND FormID = @FormID;", new { ModKey = "Example.esm", FormID = "000001" });

        database.ExecuteScalar<int>("SELECT COUNT(*) FROM Faction;").ShouldBe(0);
        database.ExecuteScalar<int>("SELECT COUNT(*) FROM FactionRelation;").ShouldBe(0);
    }
}
