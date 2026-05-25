using SFRecordCompareEngine.Core.Database;
using SFRecordCompareEngine.Core.Database.Interfaces;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Models.Database;
using SFRecordCompareEngine.Core.Repositories;
using SFRecordCompareEngine.Migrations;
using Shouldly;

namespace SFRecordCompareEngine.UnitTests.Repositories;

public class RecordHeaderRepositoryTests : IDisposable
{
    private readonly string DatabaseDirectory = Path.Combine(Path.GetTempPath(), "SFRecordCompareEngineTests", Guid.NewGuid().ToString("N"));
    private readonly ISqliteConnectionFactory ConnectionFactory;
    private readonly PluginRepository PluginRepository = new();
    private readonly RecordHeaderRepository Sut = new();

    public RecordHeaderRepositoryTests()
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
    public void DeleteByModKey_WhenCalled_RemovesAllHeadersForModKey()
    {
        using var database = ConnectionFactory.OpenDatabase();
        InsertPluginAndHeader(database, "Example.esm", 1, "000001", "Keyword");
        InsertHeader(database, "Example.esm", "000002", "Weapon");

        Sut.DeleteByModKey(database, "Example.esm");

        database.ExecuteScalar<int>("SELECT COUNT(*) FROM RecordHeader WHERE ModKey = @ModKey COLLATE NOCASE;", new { ModKey = "Example.esm" }).ShouldBe(0);
    }

    private void InsertPluginAndHeader(NPoco.IDatabase database, string modKey, int loadOrderIndex, string formId, string recordType)
    {
        PluginRepository.UpsertPlugin(database, new PluginMetadataDTO
        {
            ModKey = modKey,
            GameRelease = "Starfield",
            LoadOrderIndex = loadOrderIndex,
            PluginFileName = modKey,
            Enabled = true,
            ExistsOnDisk = true,
            ImportState = PluginImportState.Current.ToString(),
            LastCheckedUtc = DateTimeOffset.UtcNow.ToString("O")
        });
        InsertHeader(database, modKey, formId, recordType);
    }

    private void InsertHeader(NPoco.IDatabase database, string modKey, string formId, string recordType)
    {
        Sut.Upsert(database, new RecordHeaderDTO
        {
            ModKey = modKey,
            FormID = formId,
            RecordType = recordType,
            FormKey = $"{formId}:{modKey}",
            EditorID = $"{recordType}_{formId}",
            PluginFileName = modKey,
            ImportedAtUtc = DateTimeOffset.UtcNow.ToString("O")
        });
    }

}
