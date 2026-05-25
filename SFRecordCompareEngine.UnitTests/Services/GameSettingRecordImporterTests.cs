using SFRecordCompareEngine.Core.Database;
using SFRecordCompareEngine.Core.Database.Interfaces;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Importers;
using SFRecordCompareEngine.Core.Models.Database;
using SFRecordCompareEngine.Core.Repositories;
using SFRecordCompareEngine.Core.Services;
using SFRecordCompareEngine.Migrations;
using Shouldly;

namespace SFRecordCompareEngine.UnitTests.Services;

public class GameSettingRecordImporterTests : IDisposable
{
    private readonly string DatabaseDirectory = Path.Combine(Path.GetTempPath(), "SFRecordCompareEngineTests", Guid.NewGuid().ToString("N"));
    private readonly ISqliteConnectionFactory ConnectionFactory;
    private readonly PluginRepository PluginRepository = new();
    private readonly RecordHeaderRepository RecordHeaderRepository = new();

    public GameSettingRecordImporterTests()
    {
        var options = new SqliteDatabaseOptions
        {
            DatabaseDirectory = DatabaseDirectory
        };

        ConnectionFactory = new SqliteConnectionFactory(options);
        new DatabaseSchemaInitializer(ConnectionFactory, new DatabaseMigrationRunner()).Initialize();
    }

    public void Dispose()
    {
        if (Directory.Exists(DatabaseDirectory))
        {
            Directory.Delete(DatabaseDirectory, true);
        }
    }

    [Fact]
    public void Import_WhenGameSettingHasMutagenDetailFields_PersistsNonHeaderFields()
    {
        using var database = ConnectionFactory.OpenDatabase();
        InsertPluginAndHeader(database);
        var sut = new GameSettingRecordImporter(new GameSettingRepository());
        var importedAtUtc = DateTimeOffset.UtcNow.ToString("O");

        sut.Import(database, "Example.esm", "000001", new RecordEnumerationDTO
        {
            RecordType = "GameSetting",
            Record = new TestGameSettingRecord()
        }, importedAtUtc);

        database.ExecuteScalar<string>(
            "SELECT SettingType FROM GameSetting WHERE ModKey = @ModKey COLLATE NOCASE AND FormID = @FormId;",
            new { ModKey = "Example.esm", FormId = "000001" }).ShouldBe("String");
        database.ExecuteScalar<string>("SELECT TitleString FROM GameSetting WHERE ModKey = @ModKey COLLATE NOCASE AND FormID = @FormId;", new { ModKey = "Example.esm", FormId = "000001" }).ShouldBe("Example Title");
        database.ExecuteScalar<string>("SELECT Data FROM GameSetting WHERE ModKey = @ModKey COLLATE NOCASE AND FormID = @FormId;", new { ModKey = "Example.esm", FormId = "000001" }).ShouldBe("Example Value");
        database.ExecuteScalar<double>("SELECT RawData FROM GameSetting WHERE ModKey = @ModKey COLLATE NOCASE AND FormID = @FormId;", new { ModKey = "Example.esm", FormId = "000001" }).ShouldBe(123.5);
        database.ExecuteScalar<int>("SELECT XALG FROM GameSetting WHERE ModKey = @ModKey COLLATE NOCASE AND FormID = @FormId;", new { ModKey = "Example.esm", FormId = "000001" }).ShouldBe(42);
        database.ExecuteScalar<int>("SELECT IsCompressed FROM GameSetting WHERE ModKey = @ModKey COLLATE NOCASE AND FormID = @FormId;", new { ModKey = "Example.esm", FormId = "000001" }).ShouldBe(1);
        database.ExecuteScalar<int>("SELECT IsDeleted FROM GameSetting WHERE ModKey = @ModKey COLLATE NOCASE AND FormID = @FormId;", new { ModKey = "Example.esm", FormId = "000001" }).ShouldBe(0);
        database.ExecuteScalar<string>("SELECT ImportedAtUtc FROM GameSetting WHERE ModKey = @ModKey COLLATE NOCASE AND FormID = @FormId;", new { ModKey = "Example.esm", FormId = "000001" }).ShouldBe(importedAtUtc);
    }

    private void InsertPluginAndHeader(NPoco.IDatabase database)
    {
        PluginRepository.UpsertPlugin(database, new PluginMetadataDTO
        {
            ModKey = "Example.esm",
            GameRelease = "Starfield",
            LoadOrderIndex = 1,
            PluginFileName = "Example.esm",
            Enabled = true,
            ExistsOnDisk = true,
            ImportState = PluginImportState.Current.ToString(),
            LastCheckedUtc = DateTimeOffset.UtcNow.ToString("O")
        });

        RecordHeaderRepository.Upsert(database, new RecordHeaderDTO
        {
            ModKey = "Example.esm",
            FormID = "000001",
            RecordType = "GameSetting",
            FormKey = "000001:Example.esm",
            PluginFileName = "Example.esm",
            ImportedAtUtc = DateTimeOffset.UtcNow.ToString("O")
        });
    }

    private sealed class TestGameSettingRecord
    {
        public string SettingType => "String";
        public string TitleString => "Example Title";
        public string Data => "Example Value";
        public double RawData => 123.5;
        public int XALG => 42;
        public bool IsCompressed => true;
        public bool IsDeleted => false;
    }
}
