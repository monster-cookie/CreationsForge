using SFRecordCompareEngine.Core.Database;
using SFRecordCompareEngine.Core.Database.Interfaces;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Models.Database;
using SFRecordCompareEngine.Core.Repositories;
using SFRecordCompareEngine.Migrations;
using Shouldly;

namespace SFRecordCompareEngine.UnitTests.Repositories;

public class WorldspaceRepositoryTests : IDisposable
{
    private readonly string DatabaseDirectory = Path.Combine(Path.GetTempPath(), "SFRecordCompareEngineTests", Guid.NewGuid().ToString("N"));
    private readonly ISqliteConnectionFactory ConnectionFactory;
    private readonly PluginRepository PluginRepository = new();
    private readonly RecordHeaderRepository RecordHeaderRepository = new();
    private readonly WorldspaceRepository Sut = new();

    public WorldspaceRepositoryTests()
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
    public void Upsert_WhenHeaderExists_InsertsWorldspaceRow()
    {
        using var database = ConnectionFactory.OpenDatabase();
        InsertPluginAndHeader(database, "Example.esm", "000100", "Worldspace");
        var importedAtUtc = DateTimeOffset.UtcNow.ToString("O");

        Sut.Upsert(database, new WorldspaceDTO
        {
            ModKey = "Example.esm",
            FormID = "000100",
            Name = "Example Worldspace",
            TopCellFormKey = "000001:Example.esm",
            ImportedAtUtc = importedAtUtc
        });

        var result = database.First<WorldspaceDTO>(
            "SELECT * FROM Worldspace WHERE ModKey = @ModKey COLLATE NOCASE AND FormID = @FormId;",
            new { ModKey = "Example.esm", FormId = "000100" });
        result.Name.ShouldBe("Example Worldspace");
        result.TopCellFormKey.ShouldBe("000001:Example.esm");
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
