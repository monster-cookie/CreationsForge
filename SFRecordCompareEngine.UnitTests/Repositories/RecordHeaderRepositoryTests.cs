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
    public void GetByHierarchy_WhenRowsExist_ReturnsRowsOrderedByLoadOrder()
    {
        using var database = ConnectionFactory.OpenDatabase();
        InsertPluginAndHeader(database, "ParentA.esm", 1, "000001", "Keyword");
        InsertPluginAndHeader(database, "ParentB.esm", 2, "000001", "Keyword");
        InsertPluginAndHeader(database, "Child.esm", 3, "000001", "Keyword");
        PluginRepository.ReplaceMasterReferences(database, "Child.esm", [
            CreateMaster("Child.esm", "ParentB.esm", 0, 2),
            CreateMaster("Child.esm", "ParentA.esm", 1, 1)
        ]);

        var result = Sut.GetByHierarchy(database, "Child.esm", "000001", null, "Keyword");

        result.Select(record => record.ModKey).ShouldBe(["ParentA.esm", "ParentB.esm", "Child.esm"]);
    }

    [Fact]
    public void GetWinningOverride_WhenRowsExist_ReturnsLastMatchingLoadOrderRecord()
    {
        using var database = ConnectionFactory.OpenDatabase();
        InsertPluginAndHeader(database, "Parent.esm", 1, "000001", "Keyword");
        InsertPluginAndHeader(database, "Child.esm", 2, "000001", "Keyword");
        PluginRepository.ReplaceMasterReferences(database, "Child.esm", [
            CreateMaster("Child.esm", "Parent.esm", 0, 1)
        ]);

        var result = Sut.GetWinningOverride(database, "Child.esm", "000001", null, "Keyword");

        result.ShouldNotBeNull();
        result.ModKey.ShouldBe("Child.esm");
    }

    [Fact]
    public void DeleteByModKey_WhenCalled_RemovesAllHeadersForModKey()
    {
        using var database = ConnectionFactory.OpenDatabase();
        InsertPluginAndHeader(database, "Example.esm", 1, "000001", "Keyword");
        InsertHeader(database, "Example.esm", "000002", "Weapon");

        Sut.DeleteByModKey(database, "Example.esm");

        database.ExecuteScalar<int>("SELECT COUNT(*) FROM RecordHeader WHERE ModKey = @0 COLLATE NOCASE;", "Example.esm").ShouldBe(0);
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

    private static PluginMasterReferenceDTO CreateMaster(string modKey, string parentModKey, int index, int parentLoadOrderIndex)
    {
        return new PluginMasterReferenceDTO
        {
            ModKey = modKey,
            ParentModKey = parentModKey,
            MasterReferenceIndex = index,
            ParentLoadOrderIndex = parentLoadOrderIndex,
            ImportedAtUtc = DateTimeOffset.UtcNow.ToString("O")
        };
    }
}
