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

public class ConditionFormImporterTests
{
    [Fact]
    public void Import_SavesConditionFormAndRawPayloads()
    {
        var plugin = CreatePlugin();
        var conditionForm = CreateConditionForm(plugin, 100);
        var rawPayload = CreateRawPayload(plugin, conditionForm.FormKey);
        conditionForm.RawPayloads.Add(rawPayload);
        var repository = new TestConditionFormRepository();
        var childImportService = new TestRecordChildImportService();
        var importer = new ConditionFormImporter(repository, childImportService);
        var result = new RecordTypeImportResultDTO { RecordType = RecordTypeCatalog.ConditionForm.RecordID };

        var importedAtUTC = DateTime.UtcNow;
        importer.Import(conditionForm, result, importedAtUTC);

        importer.RecordType.ShouldBe("CNDF");
        importer.TableName.ShouldBe("ConditionForms");
        importer.SupportedGames.ShouldBe([SupportedGame.Starfield], ignoreOrder: true);
        repository.Saved.ShouldBe([conditionForm]);
        conditionForm.ImportedAtUTC.ShouldBe(importedAtUTC);
        childImportService.ReplaceRequests.ShouldBe([(conditionForm, RecordTypeCatalog.ConditionForm.RecordID)]);
        result.DetailRowsImported.ShouldBe(1);
    }

    [Fact]
    public void DeleteStaleRecords_DeletesStaleConditionFormsForPlugin()
    {
        var plugin = CreatePlugin();
        var repository = new TestConditionFormRepository();
        var importer = new ConditionFormImporter(repository, new TestRecordChildImportService());
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

    private static ConditionFormDTO CreateConditionForm(PluginDTO plugin, uint id)
    {
        return new ConditionFormDTO
        {
            Game = plugin.Game,
            ModKey = plugin.ModKey,
            FormKey = CreateFormKey(plugin.ModKey, id),
            EditorID = $"CNDF{id}",
            FormVersion = 1,
            MajorRecordFlags = 0,
            ImportedAtUTC = default,
            Version2 = 1
        };
    }

    private static RawRecordPayloadDTO CreateRawPayload(PluginDTO plugin, FormKeyDTO formKey)
    {
        return new RawRecordPayloadDTO
        {
            Game = plugin.Game,
            ModKey = plugin.ModKey,
            RecordType = RecordTypeCatalog.ConditionForm.RecordID,
            FormKey = formKey,
            PayloadSlot = "Conditions",
            PayloadIndex = 0,
            PayloadType = "Conditions",
            SourcePath = "Conditions",
            PayloadValue = "ConditionFloat",
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

    private sealed class TestConditionFormRepository : IConditionFormRepository
    {
        public string RecordType => RecordTypeCatalog.ConditionForm.RecordID;

        public IList<ConditionFormDTO> Saved { get; } = new List<ConditionFormDTO>();

        public IList<(SupportedGame Game, ModKeyDTO ModKey, DateTime ImportedAtUTC)> StaleCleanupRequests { get; } = new List<(SupportedGame Game, ModKeyDTO ModKey, DateTime ImportedAtUTC)>();

        public IReadOnlyList<RecordTreeEntryDTO> GetRecordTreeEntriesByPlugin(SupportedGame game, ModKeyDTO modKey)
        {
            return [];
        }

        public IReadOnlyDictionary<string, int> GetRecordPluginCountsByGame(SupportedGame game)
        {
            return new Dictionary<string, int>();
        }

        public IReadOnlyList<ConditionFormDTO> GetByFormKey(SupportedGame game, FormKeyDTO formKey)
        {
            return [];
        }

        public void Save(ConditionFormDTO dto)
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
