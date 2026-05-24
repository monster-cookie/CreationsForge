using SFRecordCompareEngine.Core.Database;
using SFRecordCompareEngine.Core.Database.Interfaces;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Models.Database;
using SFRecordCompareEngine.Core.Repositories;
using SFRecordCompareEngine.Migrations;
using Shouldly;

namespace SFRecordCompareEngine.UnitTests.Repositories;

public class CellRepositoryTests : IDisposable
{
    private readonly string DatabaseDirectory = Path.Combine(Path.GetTempPath(), "SFRecordCompareEngineTests", Guid.NewGuid().ToString("N"));
    private readonly ISqliteConnectionFactory ConnectionFactory;
    private readonly PluginRepository PluginRepository = new();
    private readonly RecordHeaderRepository RecordHeaderRepository = new();
    private readonly CellRepository Sut = new();

    public CellRepositoryTests()
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
    public void ReplaceChildren_WhenCalledAgain_ReplacesLocationsAndPlacedRecords()
    {
        using var database = ConnectionFactory.OpenDatabase();
        InsertPluginAndHeader(database, "Example.esm", "000001", "Cell");
        var importedAtUtc = DateTimeOffset.UtcNow.ToString("O");

        Sut.Upsert(database, new CellDTO
        {
            ModKey = "Example.esm",
            FormID = "000001",
            Name = "Example Cell",
            ImportedAtUtc = importedAtUtc
        });
        Sut.ReplaceGroupLocations(database, "Example.esm", "000001", [
            new CellGroupLocationDTO
            {
                ModKey = "Example.esm",
                CellFormID = "000001",
                LocationIndex = 0,
                LocationKind = "InteriorCell",
                BlockNumber = 0,
                SubBlockNumber = 1,
                ImportedAtUtc = importedAtUtc
            }
        ]);
        Sut.ReplacePlacedRecords(database, "Example.esm", "000001", [
            new CellPlacedRecordDTO
            {
                ModKey = "Example.esm",
                CellFormID = "000001",
                PlacementGroup = "Temporary",
                ItemIndex = 0,
                PlacedFormKey = "000002:Example.esm",
                ImportedAtUtc = importedAtUtc
            }
        ]);

        Sut.ReplaceGroupLocations(database, "Example.esm", "000001", [
            new CellGroupLocationDTO
            {
                ModKey = "Example.esm",
                CellFormID = "000001",
                LocationIndex = 0,
                LocationKind = "WorldspaceTopCell",
                WorldspaceFormID = "000100",
                ImportedAtUtc = importedAtUtc
            }
        ]);
        Sut.ReplacePlacedRecords(database, "Example.esm", "000001", [
            new CellPlacedRecordDTO
            {
                ModKey = "Example.esm",
                CellFormID = "000001",
                PlacementGroup = "Persistent",
                ItemIndex = 0,
                PlacedFormKey = "000003:Example.esm",
                ImportedAtUtc = importedAtUtc
            }
        ]);

        database.ExecuteScalar<int>("SELECT COUNT(*) FROM CellGroupLocation WHERE ModKey = @0 COLLATE NOCASE AND CellFormID = @1;", "Example.esm", "000001")
            .ShouldBe(1);
        database.ExecuteScalar<string>("SELECT LocationKind FROM CellGroupLocation WHERE ModKey = @0 COLLATE NOCASE AND CellFormID = @1;", "Example.esm", "000001")
            .ShouldBe("WorldspaceTopCell");
        database.ExecuteScalar<string>("SELECT PlacedFormKey FROM CellPlacedRecord WHERE ModKey = @0 COLLATE NOCASE AND CellFormID = @1;", "Example.esm", "000001")
            .ShouldBe("000003:Example.esm");
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
}
