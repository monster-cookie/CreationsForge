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

        var result = Sut.GetByModKeyAndFormId(database, "Example.esm", "000001");

        result.ShouldNotBeNull();
        result.FormList.AddToListFormKey.ShouldBe("000100:Example.esm");
        result.Items.Select(item => item.ItemFormKey).ShouldBe(["000002:Example.esm", "000003:Example.esm"]);
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

        var result = Sut.GetByModKeyAndFormId(database, "Example.esm", "000001");

        result.ShouldNotBeNull();
        result.Items.Single().ItemFormKey.ShouldBe("000003:Example.esm");
    }

    [Fact]
    public void GetByHierarchyAndFormId_WhenRowsExist_ReturnsRowsOrderedByEffectiveLoadOrder()
    {
        using var database = ConnectionFactory.OpenDatabase();
        InsertPluginAndHeader(database, "ParentA.esm", "000001", 1);
        InsertPluginAndHeader(database, "ParentB.esm", "000001", 2);
        InsertPluginAndHeader(database, "Child.esm", "000001", 3);
        PluginRepository.ReplaceMasterReferences(database, "Child.esm", [
            CreateMaster("Child.esm", "ParentB.esm", 0, 2),
            CreateMaster("Child.esm", "ParentA.esm", 1, 1)
        ]);
        Sut.UpsertFormList(database, CreateFormList("ParentA.esm", "000001", null));
        Sut.UpsertFormList(database, CreateFormList("ParentB.esm", "000001", null));
        Sut.UpsertFormList(database, CreateFormList("Child.esm", "000001", null));

        var result = Sut.GetByHierarchyAndFormId(database, "Child.esm", "000001");

        result.Select(record => record.Header.ModKey).ShouldBe(["ParentA.esm", "ParentB.esm", "Child.esm"]);
    }

    [Fact]
    public void GetByModKeyAndFormId_WhenPluginIsMissing_DoesNotReturnStaleRows()
    {
        using var database = ConnectionFactory.OpenDatabase();
        var plugin = CreatePlugin("Example.esm", 1);
        plugin.ImportState = PluginImportState.Missing.ToString();
        PluginRepository.UpsertPlugin(database, plugin);
        InsertHeader(database, "Example.esm", "000001");
        Sut.UpsertFormList(database, CreateFormList("Example.esm", "000001", null));

        var result = Sut.GetByModKeyAndFormId(database, "Example.esm", "000001");

        result.ShouldBeNull();
    }

    [Fact]
    public void DeleteByModKeyAndRecordType_WhenCalled_RemovesFormListRowsByCascade()
    {
        using var database = ConnectionFactory.OpenDatabase();
        InsertPluginAndHeader(database, "Example.esm", "000001", 1);
        Sut.UpsertFormList(database, CreateFormList("Example.esm", "000001", null));

        RecordHeaderRepository.DeleteByModKeyAndRecordType(database, "Example.esm", "FormList");

        database.ExecuteScalar<int>("SELECT COUNT(*) FROM FormList;").ShouldBe(0);
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
