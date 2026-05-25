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

public class CellRecordImporterTests : IDisposable
{
    private readonly string DatabaseDirectory = Path.Combine(Path.GetTempPath(), "SFRecordCompareEngineTests", Guid.NewGuid().ToString("N"));
    private readonly ISqliteConnectionFactory ConnectionFactory;
    private readonly PluginRepository PluginRepository = new();
    private readonly RecordHeaderRepository RecordHeaderRepository = new();

    public CellRecordImporterTests()
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
    public void Import_WhenCellHasLocationAndPlacedRecords_PersistsDetailRows()
    {
        using var database = ConnectionFactory.OpenDatabase();
        InsertPluginAndHeader(database, "Example.esm", "000001", "Cell");
        var sut = new CellRecordImporter(new CellRepository());
        var importedAtUtc = DateTimeOffset.UtcNow.ToString("O");

        sut.Import(database, "Example.esm", "000001", new RecordEnumerationDTO
        {
            RecordType = "Cell",
            Record = new TestCellRecord(),
            CellGroupLocations =
            [
                new CellGroupLocationDTO
                {
                    ModKey = "Example.esm",
                    CellFormID = "000001",
                    LocationKind = "InteriorCell",
                    BlockNumber = 0,
                    SubBlockNumber = 1,
                    ImportedAtUtc = string.Empty
                }
            ]
        }, importedAtUtc);

        database.ExecuteScalar<string>("SELECT Name FROM Cell WHERE ModKey = @ModKey COLLATE NOCASE AND FormID = @FormId;", new { ModKey = "Example.esm", FormId = "000001" })
            .ShouldBe("Example Cell");
        database.ExecuteScalar<int>("SELECT SubBlockNumber FROM CellGroupLocation WHERE ModKey = @ModKey COLLATE NOCASE AND CellFormID = @CellFormId;", new { ModKey = "Example.esm", CellFormId = "000001" })
            .ShouldBe(1);
        database.ExecuteScalar<string>("SELECT PlacedFormKey FROM CellPlacedRecord WHERE ModKey = @ModKey COLLATE NOCASE AND CellFormID = @CellFormId;", new { ModKey = "Example.esm", CellFormId = "000001" })
            .ShouldBe("000002:Example.esm");
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

    private sealed class TestCellRecord
    {
        public string Name => "Example Cell";
        public bool IsLinkedRefTransient => true;
        public IList<TestPlacedRecord> Temporary { get; } =
        [
            new()
            {
                FormKey = "000002:Example.esm",
                Base = "000003:Example.esm",
                EditorID = "PlacedObject"
            }
        ];
        public IList<TestPlacedRecord> Persistent { get; } = [];
    }

    private sealed class TestPlacedRecord
    {
        public string? FormKey { get; init; }
        public string? Base { get; init; }
        public string? EditorID { get; init; }
        public bool IsDeleted => false;
    }
}
