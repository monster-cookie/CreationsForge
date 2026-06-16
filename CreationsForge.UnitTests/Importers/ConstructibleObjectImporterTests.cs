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

public class ConstructibleObjectImporterTests
{
    [Fact]
    public void Import_SavesConstructibleObjectAndSharedChildren()
    {
        var plugin = CreatePlugin();
        var constructibleObject = CreateConstructibleObject(plugin, 100);
        var repository = new TestConstructibleObjectRepository();
        var childImportService = new TestRecordChildImportService();
        var importer = new ConstructibleObjectImporter(repository, childImportService);
        var result = new RecordTypeImportResultDTO { RecordType = RecordTypeCatalog.ConstructibleObject.RecordID };

        var importedAtUTC = DateTime.UtcNow;
        importer.Import(constructibleObject, result, importedAtUTC);

        importer.RecordType.ShouldBe("COBJ");
        importer.TableName.ShouldBe("ConstructibleObjects");
        importer.SupportedGames.ShouldBe([SupportedGame.Starfield, SupportedGame.Fallout4, SupportedGame.Skyrim], ignoreOrder: true);
        repository.Saved.ShouldBe([constructibleObject]);
        constructibleObject.Components.Single().Count.ShouldBe(2);
        constructibleObject.Categories.Single().CategoryIndex.ShouldBe(0);
        constructibleObject.RecipeFilters.Single().RecipeFilterIndex.ShouldBe(0);
        constructibleObject.ImportedAtUTC.ShouldBe(importedAtUTC);
        childImportService.ReplaceRequests.ShouldBe([(constructibleObject, RecordTypeCatalog.ConstructibleObject.RecordID)]);
        result.DetailRowsImported.ShouldBe(1);
    }

    [Fact]
    public void DeleteStaleRecords_DeletesStaleConstructibleObjectsForPlugin()
    {
        var plugin = CreatePlugin();
        var repository = new TestConstructibleObjectRepository();
        var importer = new ConstructibleObjectImporter(repository, new TestRecordChildImportService());
        var importedAtUTC = DateTime.UtcNow;

        importer.DeleteStaleRecords(plugin, importedAtUTC);

        repository.StaleCleanupRequests.ShouldBe([(plugin.Game, plugin.ModKey, importedAtUTC)]);
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

    private static ConstructibleObjectDTO CreateConstructibleObject(PluginDTO plugin, uint id)
    {
        var formKey = CreateFormKey(plugin.ModKey, id);
        return new ConstructibleObjectDTO
        {
            Game = plugin.Game,
            ModKey = plugin.ModKey,
            FormKey = formKey,
            EditorID = $"COBJ{id}",
            FormVersion = 1,
            MajorRecordFlags = 0,
            ImportedAtUTC = default,
            Version2 = 1,
            Description = "Recipe",
            CreatedObjectFormKey = CreateFormKey(plugin.ModKey, 200),
            WorkbenchKeywordFormKey = CreateFormKey(plugin.ModKey, 300),
            CreatedObjectCount = 1,
            Components =
            {
                new ConstructibleObjectComponentDTO
                {
                    Game = plugin.Game,
                    ModKey = plugin.ModKey,
                    FormKey = formKey,
                    ComponentFormKey = CreateFormKey(plugin.ModKey, 400),
                    ComponentIndex = 0,
                    Count = 2,
                    ImportedAtUTC = default
                }
            },
            Categories =
            {
                new ConstructibleObjectCategoryDTO
                {
                    Game = plugin.Game,
                    ModKey = plugin.ModKey,
                    FormKey = formKey,
                    CategoryFormKey = CreateFormKey(plugin.ModKey, 500),
                    CategoryIndex = 0,
                    ImportedAtUTC = default
                }
            },
            RecipeFilters =
            {
                new ConstructibleObjectRecipeFilterDTO
                {
                    Game = plugin.Game,
                    ModKey = plugin.ModKey,
                    FormKey = formKey,
                    RecipeFilterFormKey = CreateFormKey(plugin.ModKey, 600),
                    RecipeFilterIndex = 0,
                    ImportedAtUTC = default
                }
            }
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

    private sealed class TestConstructibleObjectRepository : IConstructibleObjectRepository
    {
        public string RecordType => RecordTypeCatalog.ConstructibleObject.RecordID;

        public IList<ConstructibleObjectDTO> Saved { get; } = new List<ConstructibleObjectDTO>();

        public IList<(SupportedGame Game, ModKeyDTO ModKey, DateTime ImportedAtUTC)> StaleCleanupRequests { get; } = new List<(SupportedGame Game, ModKeyDTO ModKey, DateTime ImportedAtUTC)>();

        public IReadOnlyList<RecordTreeEntryDTO> GetRecordTreeEntriesByPlugin(SupportedGame game, ModKeyDTO modKey)
        {
            return [];
        }

        public IReadOnlyDictionary<string, int> GetRecordPluginCountsByGame(SupportedGame game)
        {
            return new Dictionary<string, int>();
        }

        public IReadOnlyList<ConstructibleObjectDTO> GetByFormKey(SupportedGame game, FormKeyDTO formKey)
        {
            return [];
        }

        public void Save(ConstructibleObjectDTO dto)
        {
            Saved.Add(dto);
        }

        public void DeleteStaleByPlugin(SupportedGame game, ModKeyDTO modKey, DateTime importedAtUTC)
        {
            StaleCleanupRequests.Add((game, modKey, importedAtUTC));
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
