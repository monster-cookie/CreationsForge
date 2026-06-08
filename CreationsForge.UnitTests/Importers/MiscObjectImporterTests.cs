using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Records.Interfaces;
using CreationsForge.Core.DTOs.Results;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Importers;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services.Interfaces;
using Shouldly;

namespace CreationsForge.UnitTests.Importers;

public class MiscObjectImporterTests
{
    [Fact]
    public void Import_SavesMiscObjectAndReplacesModelsAndScripts()
    {
        var plugin = CreatePlugin();
        var miscObject = CreateMiscObject(plugin);
        var repository = new TestMiscObjectRepository();
        var scriptingAdapterImportService = new TestScriptingAdapterImportService();
        var modelImportService = new TestModelImportService();
        var recordKeywordImportService = new TestRecordKeywordImportService();
        var recordSoundImportService = new TestRecordSoundImportService();
        var importer = new MiscObjectImporter(repository, scriptingAdapterImportService, modelImportService, recordKeywordImportService, recordSoundImportService);
        var result = new RecordTypeImportResultDTO { RecordType = RecordTypeCatalog.MiscObject.RecordID };

        var importedAtUTC = DateTime.UtcNow;
        importer.Import(miscObject, result, importedAtUTC);

        importer.RecordType.ShouldBe("MISC");
        importer.TableName.ShouldBe("MiscObjects");
        importer.SupportedGames.ShouldBe([SupportedGame.Starfield, SupportedGame.Fallout4, SupportedGame.Skyrim], ignoreOrder: true);
        repository.Saved.ShouldBe([miscObject]);
        recordKeywordImportService.ReplaceRequests.ShouldBe([(miscObject, RecordTypeCatalog.MiscObject.RecordID)]);
        modelImportService.ReplaceRequests.ShouldBe([(miscObject, RecordTypeCatalog.MiscObject.RecordID)]);
        recordSoundImportService.ReplaceRequests.ShouldBe([(miscObject, RecordTypeCatalog.MiscObject.RecordID)]);
        scriptingAdapterImportService.ReplaceRequests.ShouldBe([(miscObject, RecordTypeCatalog.MiscObject.RecordID)]);
        miscObject.ImportedAtUTC.ShouldBe(importedAtUTC);
        result.DetailRowsImported.ShouldBe(1);
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

    private static MiscObjectDTO CreateMiscObject(PluginDTO plugin)
    {
        return new MiscObjectDTO
        {
            Game = plugin.Game,
            ModKey = plugin.ModKey,
            FormKey = new FormKeyDTO { ModKey = plugin.ModKey, Id = 10 },
            EditorID = "MiscObject",
            FormVersion = 1,
            MajorRecordFlags = 0,
            ImportedAtUTC = default
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

    private sealed class TestMiscObjectRepository : IMiscObjectRepository
    {
        public string RecordType => "MISC";

        public IList<MiscObjectDTO> Saved { get; } = new List<MiscObjectDTO>();

        public IReadOnlyList<RecordTreeEntryDTO> GetRecordTreeEntriesByPlugin(SupportedGame game, ModKeyDTO modKey)
        {
            return [];
        }

        public IReadOnlyDictionary<string, int> GetRecordPluginCountsByGame(SupportedGame game)
        {
            return new Dictionary<string, int>();
        }

        public IReadOnlyList<MiscObjectDTO> GetByFormKey(SupportedGame game, FormKeyDTO formKey)
        {
            return [];
        }

        public void Save(MiscObjectDTO dto)
        {
            Saved.Add(dto);
        }

        public void DeleteStaleByPlugin(SupportedGame game, ModKeyDTO modKey, DateTime importedAtUTC)
        { }
    }

    private sealed class TestModelImportService : IModelImportService
    {
        public IList<(IHasModelsRecordDTO Record, string RecordType)> ReplaceRequests { get; } = new List<(IHasModelsRecordDTO Record, string RecordType)>();

        public void ReplaceRecordModels(IHasModelsRecordDTO record, string recordType)
        {
            ReplaceRequests.Add((record, recordType));
        }
    }

    private sealed class TestRecordKeywordImportService : IRecordKeywordImportService
    {
        public IList<(IHasKeywordsRecordDTO Record, string RecordType)> ReplaceRequests { get; } = new List<(IHasKeywordsRecordDTO Record, string RecordType)>();

        public void ReplaceRecordKeywords(IHasKeywordsRecordDTO record, string recordType)
        {
            ReplaceRequests.Add((record, recordType));
        }
    }

    private sealed class TestRecordSoundImportService : IRecordSoundImportService
    {
        public IList<(IHasSoundsRecordDTO Record, string RecordType)> ReplaceRequests { get; } = new List<(IHasSoundsRecordDTO Record, string RecordType)>();

        public void ReplaceRecordSounds(IHasSoundsRecordDTO record, string recordType)
        {
            ReplaceRequests.Add((record, recordType));
        }
    }

    private sealed class TestScriptingAdapterImportService : IScriptingAdapterImportService
    {
        public IList<(IHasScriptingAdaptersRecordDTO Record, string RecordType)> ReplaceRequests { get; } = new List<(IHasScriptingAdaptersRecordDTO Record, string RecordType)>();

        public void ReplaceRecordScriptingAdapters(IHasScriptingAdaptersRecordDTO record, string recordType)
        {
            ReplaceRequests.Add((record, recordType));
        }
    }
}
