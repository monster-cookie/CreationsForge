using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Results;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Importers;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services.Interfaces;
using Shouldly;

namespace CreationsForge.UnitTests.Importers;

public class FormListImporterTests
{
    [Fact]
    public void Import_SavesFormListAndItemsWithIndexesAndCounts()
    {
        var plugin = CreatePlugin();
        var formList = CreateFormList(plugin, 100);
        var firstItem = CreateFormListItem(plugin, formList.FormKey, 200);
        var secondItem = CreateFormListItem(plugin, formList.FormKey, 201);
        formList.Items.Add(firstItem);
        formList.Items.Add(secondItem);
        var formListRepository = new TestFormListRepository();
        var itemRepository = new TestFormListItemRepository();
        var childImportService = new TestRecordChildImportService();
        var importer = new FormListImporter(formListRepository, itemRepository, childImportService);
        var result = new RecordTypeImportResultDTO { RecordType = RecordTypeCatalog.FormList.RecordID };

        var importedAtUTC = DateTime.UtcNow;
        importer.Import(formList, result, importedAtUTC);

        importer.RecordType.ShouldBe("FLST");
        importer.TableName.ShouldBe("FormLists");
        importer.SupportedGames.ShouldBe([SupportedGame.Starfield, SupportedGame.Fallout4, SupportedGame.Skyrim], ignoreOrder: true);
        formListRepository.Saved.ShouldBe([formList]);
        itemRepository.Saved.ShouldBe([firstItem, secondItem]);
        itemRepository.StaleCleanupRequests.ShouldBeEmpty();
        firstItem.ItemIndex.ShouldBe(0);
        secondItem.ItemIndex.ShouldBe(1);
        formList.ImportedAtUTC.ShouldBe(importedAtUTC);
        childImportService.ReplaceRequests.ShouldBe([(formList, RecordTypeCatalog.FormList.RecordID)]);
        firstItem.ImportedAtUTC.ShouldNotBe(default);
        secondItem.ImportedAtUTC.ShouldNotBe(default);
        firstItem.ImportedAtUTC.ShouldBe(formList.ImportedAtUTC);
        secondItem.ImportedAtUTC.ShouldBe(formList.ImportedAtUTC);
        result.DetailRowsImported.ShouldBe(1);
        result.FormListItemsImported.ShouldBe(2);
    }

    [Fact]
    public void DeleteStaleRecords_DeletesStaleFormListsAndItemsForPlugin()
    {
        var plugin = CreatePlugin();
        var formListRepository = new TestFormListRepository();
        var itemRepository = new TestFormListItemRepository();
        var importer = new FormListImporter(formListRepository, itemRepository, new TestRecordChildImportService());
        var importedAtUTC = DateTime.UtcNow;

        importer.DeleteStaleRecords(plugin, importedAtUTC);

        itemRepository.StaleCleanupRequests.ShouldBe([(plugin.Game, plugin.ModKey, importedAtUTC)]);
        formListRepository.StaleCleanupRequests.ShouldBe([(plugin.Game, plugin.ModKey, importedAtUTC)]);
    }

    [Fact]
    public void Import_WhenItemsAreReordered_AssignsCurrentIndexesBeforeStaleCleanup()
    {
        var plugin = CreatePlugin();
        var formList = CreateFormList(plugin, 100);
        var itemThatMovedFirst = CreateFormListItem(plugin, formList.FormKey, 201);
        var itemThatMovedSecond = CreateFormListItem(plugin, formList.FormKey, 200);
        formList.Items.Add(itemThatMovedFirst);
        formList.Items.Add(itemThatMovedSecond);
        var itemRepository = new TestFormListItemRepository();
        var importer = new FormListImporter(new TestFormListRepository(), itemRepository, new TestRecordChildImportService());

        importer.Import(formList, new RecordTypeImportResultDTO { RecordType = RecordTypeCatalog.FormList.RecordID }, DateTime.UtcNow);

        itemThatMovedFirst.ItemIndex.ShouldBe(0);
        itemThatMovedSecond.ItemIndex.ShouldBe(1);
        itemRepository.StaleCleanupRequests.ShouldBeEmpty();
    }

    [Fact]
    public void Import_WhenFormListHasNoItems_DoesNotDeleteStaleItemsDuringRecordImport()
    {
        var plugin = CreatePlugin();
        var formList = CreateFormList(plugin, 100);
        var itemRepository = new TestFormListItemRepository();
        var importer = new FormListImporter(new TestFormListRepository(), itemRepository, new TestRecordChildImportService());

        importer.Import(formList, new RecordTypeImportResultDTO { RecordType = RecordTypeCatalog.FormList.RecordID }, DateTime.UtcNow);

        itemRepository.Saved.ShouldBeEmpty();
        itemRepository.StaleCleanupRequests.ShouldBeEmpty();
    }

    [Fact]
    public void Import_WhenFormListHasManyItems_DoesNotPassItemIdentitiesToStaleCleanup()
    {
        var plugin = CreatePlugin();
        var formList = CreateFormList(plugin, 100);
        for (var index = 0; index < 1500; index++)
        {
            formList.Items.Add(CreateFormListItem(plugin, formList.FormKey, (uint)(200 + index)));
        }

        var itemRepository = new TestFormListItemRepository();
        var importer = new FormListImporter(new TestFormListRepository(), itemRepository, new TestRecordChildImportService());

        importer.Import(formList, new RecordTypeImportResultDTO { RecordType = RecordTypeCatalog.FormList.RecordID }, DateTime.UtcNow);

        itemRepository.Saved.Count.ShouldBe(1500);
        itemRepository.StaleCleanupRequests.ShouldBeEmpty();
    }

    private static PluginDTO CreatePlugin()
    {
        return new PluginDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey("Test", "Test.esm"),
            LoadOrderIndex = 0,
            Enabled = true,
            ExistsOnDisk = true,
            ImportState = PluginImportState.Current,
            HeaderFlags = 0,
            FormVersion = 1,
            RecordCount = 0,
            SourceLastWriteUTCTicks = 0,
            SourceFileSizeBytes = 0,
            LastCheckedUTC = DateTime.UtcNow
        };
    }

    private static FormListDTO CreateFormList(PluginDTO plugin, uint id)
    {
        return new FormListDTO
        {
            Game = plugin.Game,
            ModKey = plugin.ModKey,
            FormKey = CreateFormKey(plugin.ModKey, id),
            EditorID = $"FLST{id}",
            FormVersion = 1,
            MajorRecordFlags = 0,
            ImportedAtUTC = default
        };
    }

    private static FormListItemDTO CreateFormListItem(PluginDTO plugin, FormKeyDTO formKey, uint itemId)
    {
        return new FormListItemDTO
        {
            Game = plugin.Game,
            ModKey = plugin.ModKey,
            FormKey = formKey,
            Item = CreateFormKey(plugin.ModKey, itemId),
            ItemIndex = -1,
            ImportedAtUTC = default
        };
    }

    private static FormKeyDTO CreateFormKey(ModKeyDTO modKey, uint id)
    {
        return new FormKeyDTO
        {
            ModKey = modKey,
            Id = id
        };
    }

    private static ModKeyDTO CreateModKey(string name, string fileName)
    {
        return new ModKeyDTO
        {
            Name = name,
            Type = 0,
            FileName = fileName
        };
    }

    private sealed class TestFormListRepository : IFormListRepository
    {
        public IList<FormListDTO> Saved { get; } = new List<FormListDTO>();

        public IList<(SupportedGame Game, ModKeyDTO ModKey, DateTime ImportedAtUTC)> StaleCleanupRequests { get; } = new List<(SupportedGame Game, ModKeyDTO ModKey, DateTime ImportedAtUTC)>();

        public IReadOnlyList<RecordTreeEntryDTO> GetRecordTreeEntriesByPlugin(SupportedGame game, ModKeyDTO modKey)
        {
            return [];
        }

        public IReadOnlyDictionary<string, int> GetRecordPluginCountsByGame(SupportedGame game)
        {
            return new Dictionary<string, int>();
        }

        public IReadOnlyList<FormListDTO> GetByFormKey(SupportedGame game, FormKeyDTO formKey)
        {
            return [];
        }

        public void Save(FormListDTO dto)
        {
            Saved.Add(dto);
        }

        public void DeleteStaleByPlugin(SupportedGame game, ModKeyDTO modKey, DateTime importedAtUTC)
        {
            StaleCleanupRequests.Add((game, modKey, importedAtUTC));
        }
    }

    private sealed class TestFormListItemRepository : IFormListItemRepository
    {
        public IList<(SupportedGame Game, ModKeyDTO ModKey, DateTime ImportedAtUTC)> StaleCleanupRequests { get; } = new List<(SupportedGame Game, ModKeyDTO ModKey, DateTime ImportedAtUTC)>();

        public IList<FormListItemDTO> Saved { get; } = new List<FormListItemDTO>();

        public void DeleteStaleByPlugin(SupportedGame game, ModKeyDTO modKey, DateTime importedAtUTC)
        {
            StaleCleanupRequests.Add((game, modKey, importedAtUTC));
        }

        public void Save(FormListItemDTO dto)
        {
            Saved.Add(dto);
        }
    }

    private sealed class TestRecordChildImportService : IRecordChildImportService
    {
        public IList<(RecordDTO Record, string RecordType)> ReplaceRequests { get; } = new List<(RecordDTO Record, string RecordType)>();

        public void ReplaceRecordChildren(RecordDTO record, string recordType)
        {
            ReplaceRequests.Add((record, recordType));
        }
    }
}
