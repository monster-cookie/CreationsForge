using SFRecordCompareEngine.Core.Database;
using SFRecordCompareEngine.Core.Database.Interfaces;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Models.Database;
using SFRecordCompareEngine.Core.Repositories;
using SFRecordCompareEngine.Migrations;
using Shouldly;

namespace SFRecordCompareEngine.UnitTests.Repositories;

public class FormListRepositoryTests : IDisposable
{
    private readonly string DatabaseDirectory = Path.Combine(Path.GetTempPath(), "SFRecordCompareEngineTests", Guid.NewGuid().ToString("N"));
    private readonly ISqliteConnectionFactory ConnectionFactory;
    private readonly FormListRepository Sut = new();
    private readonly PluginRepository PluginRepository = new();
    private readonly RecordHeaderRepository RecordHeaderRepository = new();

    public FormListRepositoryTests()
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
    public void UpsertFormList_WhenHeaderExists_InsertsFormListAndItems()
    {
        using var database = ConnectionFactory.OpenDatabase();
        InsertPluginAndHeader(database, "Example.esm", "000001", 1);

        Sut.UpsertFormList(database, CreateFormList("Example.esm", "000001", "000100:Example.esm"));
        Sut.ReplaceItems(database, "Example.esm", "000001", [
            CreateItem("Example.esm", "000001", 0, "000002:Example.esm"),
            CreateItem("Example.esm", "000001", 1, "000003:Example.esm")
        ]);

        database.ExecuteScalar<string>(
            "SELECT AddToListFormKey FROM FormList WHERE ModKey = @ModKey COLLATE NOCASE AND FormID = @FormId;",
            new { ModKey = "Example.esm", FormId = "000001" }).ShouldBe("000100:Example.esm");
        database.Fetch<string>(
            "SELECT ItemFormKey FROM FormListItem WHERE ModKey = @ModKey COLLATE NOCASE AND FormID = @FormId ORDER BY ItemIndex ASC;",
            new { ModKey = "Example.esm", FormId = "000001" }).ShouldBe(["000002:Example.esm", "000003:Example.esm"]);
    }

    [Fact]
    public void ReplaceItems_WhenCalledAgain_ReplacesOnlyThatFormListsItems()
    {
        using var database = ConnectionFactory.OpenDatabase();
        InsertPluginAndHeader(database, "Example.esm", "000001", 1);

        Sut.UpsertFormList(database, CreateFormList("Example.esm", "000001", null));
        Sut.ReplaceItems(database, "Example.esm", "000001", [
            CreateItem("Example.esm", "000001", 0, "000002:Example.esm")
        ]);

        Sut.ReplaceItems(database, "Example.esm", "000001", [
            CreateItem("Example.esm", "000001", 0, "000003:Example.esm")
        ]);

        database.Fetch<string>(
            "SELECT ItemFormKey FROM FormListItem WHERE ModKey = @ModKey COLLATE NOCASE AND FormID = @FormId ORDER BY ItemIndex ASC;",
            new { ModKey = "Example.esm", FormId = "000001" }).Single().ShouldBe("000003:Example.esm");
    }

    private void InsertPluginAndHeader(NPoco.IDatabase database, string modKey, string formId, int loadOrderIndex)
    {
        PluginRepository.UpsertPlugin(database, CreatePlugin(modKey, loadOrderIndex));
        InsertHeader(database, modKey, formId);
    }

    private void InsertHeader(NPoco.IDatabase database, string modKey, string formId)
    {
        RecordHeaderRepository.Upsert(database, new RecordHeaderDTO
        {
            ModKey = modKey,
            FormID = formId,
            RecordType = "FormList",
            FormKey = $"{formId}:{modKey}",
            EditorID = $"{modKey}_{formId}",
            PluginFileName = modKey,
            ImportedAtUtc = DateTimeOffset.UtcNow.ToString("O")
        });
    }

    private static PluginMetadataDTO CreatePlugin(string modKey, int loadOrderIndex)
    {
        return new PluginMetadataDTO
        {
            ModKey = modKey,
            GameRelease = "Starfield",
            LoadOrderIndex = loadOrderIndex,
            PluginFileName = modKey,
            Enabled = true,
            ExistsOnDisk = true,
            ImportState = PluginImportState.Current.ToString(),
            LastCheckedUtc = DateTimeOffset.UtcNow.ToString("O")
        };
    }

    private static FormListDTO CreateFormList(string modKey, string formId, string? addToListFormKey)
    {
        return new FormListDTO
        {
            ModKey = modKey,
            FormID = formId,
            AddToListFormKey = addToListFormKey,
            ImportedAtUtc = DateTimeOffset.UtcNow.ToString("O")
        };
    }

    private static FormListItemDTO CreateItem(string modKey, string formId, int itemIndex, string itemFormKey)
    {
        return new FormListItemDTO
        {
            ModKey = modKey,
            FormID = formId,
            ItemIndex = itemIndex,
            ItemFormKey = itemFormKey,
            ImportedAtUtc = DateTimeOffset.UtcNow.ToString("O")
        };
    }
}
