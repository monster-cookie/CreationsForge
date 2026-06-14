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

public class MiscObjectImporterTests
{
    [Fact]
    public void Import_SavesMiscObjectAndReplacesModelsAndScripts()
    {
        var plugin = CreatePlugin();
        var miscObject = CreateMiscObject(plugin);
        var repository = new TestMiscObjectRepository();
        var childImportService = new TestRecordChildImportService();
        var importer = new MiscObjectImporter(repository, childImportService);
        var result = new RecordTypeImportResultDTO { RecordType = RecordTypeCatalog.MiscObject.RecordID };

        var importedAtUTC = DateTime.UtcNow;
        importer.Import(miscObject, result, importedAtUTC);

        importer.RecordType.ShouldBe("MISC");
        importer.TableName.ShouldBe("MiscItems");
        importer.SupportedGames.ShouldBe([SupportedGame.Starfield, SupportedGame.Fallout4, SupportedGame.Skyrim], ignoreOrder: true);
        repository.Saved.ShouldBe([miscObject]);
        childImportService.ReplaceRequests.ShouldBe([(miscObject, RecordTypeCatalog.MiscObject.RecordID)]);
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

    private sealed class TestRecordChildImportService : IRecordChildImportService
    {
        public IList<(RecordDTO Record, string RecordType)> ReplaceRequests { get; } = new List<(RecordDTO Record, string RecordType)>();

        public void ReplaceRecordChildren(RecordDTO record, string recordType)
        {
            ReplaceRequests.Add((record, recordType));
        }
    }
}
