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

public class WorldspaceRecordImporterTests : IDisposable
{
    private readonly string DatabaseDirectory = Path.Combine(Path.GetTempPath(), "SFRecordCompareEngineTests", Guid.NewGuid().ToString("N"));
    private readonly ISqliteConnectionFactory ConnectionFactory;
    private readonly PluginRepository PluginRepository = new();
    private readonly RecordHeaderRepository RecordHeaderRepository = new();

    public WorldspaceRecordImporterTests()
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
    public void Import_WhenWorldspaceHasFields_PersistsDetailRow()
    {
        using var database = ConnectionFactory.OpenDatabase();
        InsertPluginAndHeader(database, "Example.esm", "000100", "Worldspace");
        var sut = new WorldspaceRecordImporter(new WorldspaceRepository());
        var importedAtUtc = DateTimeOffset.UtcNow.ToString("O");

        sut.Import(database, "Example.esm", "000100", new RecordEnumerationDTO
        {
            RecordType = "Worldspace",
            Record = new TestWorldspaceRecord()
        }, importedAtUtc);

        var result = database.First<WorldspaceDTO>(
            "SELECT * FROM Worldspace WHERE ModKey = @ModKey COLLATE NOCASE AND FormID = @FormId;",
            new { ModKey = "Example.esm", FormId = "000100" });
        result.Name.ShouldBe("Example Worldspace");
        result.WorldMapOffsetScale.ShouldBe("1.25");
    }

    private void InsertPluginAndHeader(NPoco.IDatabase database, string modKey, string formId, string recordType)
    {
        PluginRepository.UpsertPlugin(database, new PluginMetadataDTO
        {
            ModKey = modKey,
            GameRelease = "Starfield",
            LoadOrderIndex = 1,
            PluginFileName = modKey,
            Enabled = true,
            ExistsOnDisk = true,
            ImportState = PluginImportState.Current.ToString(),
            LastCheckedUtc = DateTimeOffset.UtcNow.ToString("O")
        });

        RecordHeaderRepository.Upsert(database, new RecordHeaderDTO
        {
            ModKey = modKey,
            FormID = formId,
            RecordType = recordType,
            FormKey = $"{formId}:{modKey}",
            PluginFileName = modKey,
            ImportedAtUtc = DateTimeOffset.UtcNow.ToString("O")
        });
    }

    private sealed class TestWorldspaceRecord
    {
        public string Name => "Example Worldspace";
        public double WorldMapOffsetScale => 1.25;
    }
}
