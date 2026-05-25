using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.Database;
using SFRecordCompareEngine.Core.Database.Interfaces;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Models.Database;
using SFRecordCompareEngine.Core.Repositories;
using SFRecordCompareEngine.Migrations;
using Shouldly;

namespace SFRecordCompareEngine.UnitTests.Repositories;

public class GameSettingRepositoryTests : IDisposable
{
    private readonly string DatabaseDirectory = Path.Combine(Path.GetTempPath(), "SFRecordCompareEngineTests", Guid.NewGuid().ToString("N"));
    private readonly ISqliteConnectionFactory ConnectionFactory;
    private readonly PluginRepository PluginRepository = new();
    private readonly RecordHeaderRepository RecordHeaderRepository = new();
    private readonly GameSettingRepository Sut = new();

    public GameSettingRepositoryTests()
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
    public void Upsert_WhenHeaderExists_InsertsGameSettingRow()
    {
        using var database = ConnectionFactory.OpenDatabase();
        InsertPluginAndHeader(database, "Example.esm", "000001", "GameSetting");

        Sut.Upsert(database, new GameSettingDTO
        {
            ModKey = "Example.esm",
            FormID = "000001",
            SettingType = "String",
            TitleString = "Title",
            Data = "Value",
            RawData = 1.25,
            XALG = 2,
            IsCompressed = 0,
            IsDeleted = 1,
            ImportedAtUtc = DateTimeOffset.UtcNow.ToString("O")
        });

        database.ExecuteScalar<string>(
            "SELECT SettingType FROM GameSetting WHERE ModKey = @ModKey COLLATE NOCASE AND FormID = @FormId;",
            new { ModKey = "Example.esm", FormId = "000001" }).ShouldBe("String");
        database.ExecuteScalar<string>("SELECT Data FROM GameSetting WHERE ModKey = @ModKey COLLATE NOCASE AND FormID = @FormId;", new { ModKey = "Example.esm", FormId = "000001" }).ShouldBe("Value");
        database.ExecuteScalar<double>("SELECT RawData FROM GameSetting WHERE ModKey = @ModKey COLLATE NOCASE AND FormID = @FormId;", new { ModKey = "Example.esm", FormId = "000001" }).ShouldBe(1.25);
        database.ExecuteScalar<int>("SELECT IsDeleted FROM GameSetting WHERE ModKey = @ModKey COLLATE NOCASE AND FormID = @FormId;", new { ModKey = "Example.esm", FormId = "000001" }).ShouldBe(1);
    }

    [Fact]
    public void GetByHierarchy_ReturnsRowsInLoadOrderAndMarksMissingOverrides()
    {
        using var database = ConnectionFactory.OpenDatabase();
        InsertPlugin(database, ModKey.FromFileName("Parent.esm"), 0);
        InsertPlugin(database, ModKey.FromFileName("Child.esm"), 1);
        database.Execute(
            """
            INSERT INTO PluginMasterReferences (ModKey, ParentModKey, MasterReferenceIndex, ParentLoadOrderIndex, ImportedAtUtc)
            VALUES (@ModKey, @ParentModKey, @MasterReferenceIndex, @ParentLoadOrderIndex, @ImportedAtUtc);
            """,
            new
            {
                ModKey = "Child.esm",
                ParentModKey = "Parent.esm",
                MasterReferenceIndex = 0,
                ParentLoadOrderIndex = 0,
                ImportedAtUtc = DateTimeOffset.UtcNow.ToString("O")
            });
        InsertHeader(database, "Parent.esm", "000001", "GameSetting");
        Sut.Upsert(database, new GameSettingDTO
        {
            ModKey = "Parent.esm",
            FormID = "000001",
            Data = "ParentValue",
            ImportedAtUtc = DateTimeOffset.UtcNow.ToString("O")
        });

        var result = Sut.GetByHierarchy(database, ModKey.FromFileName("Child.esm"), "000001");

        result.Count.ShouldBe(2);
        result[0].PluginName.ShouldBe("Parent.esm");
        result[0].HasRecord.ShouldBeTrue();
        result[0].Data.ShouldBe("ParentValue");
        result[1].PluginName.ShouldBe("Child.esm");
        result[1].HasRecord.ShouldBeFalse();
    }

    private void InsertPluginAndHeader(NPoco.IDatabase database, string modKey, string formId, string recordType)
    {
        InsertPlugin(database, modKey, 1);
        InsertHeader(database, modKey, formId, recordType);
    }

    private void InsertPlugin(NPoco.IDatabase database, ModKey modKey, int loadOrderIndex)
    {
        PluginRepository.UpsertPlugin(database, new PluginMetadataDTO
        {
            ModKey = modKey,
            GameRelease = "Starfield",
            LoadOrderIndex = loadOrderIndex,
            PluginFileName = modKey.FileName,
            Enabled = true,
            ExistsOnDisk = true,
            ImportState = nameof(PluginImportState.Current),
            LastCheckedUtc = DateTimeOffset.UtcNow.ToString("O")
        });
    }

    private void InsertHeader(NPoco.IDatabase database, string modKey, string formId, string recordType)
    {
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
