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

public class GameSettingImporterTests
{
    [Fact]
    public void Import_SavesGameSettingAndIncrementsCount()
    {
        var plugin = CreatePlugin();
        var gameSetting = CreateGameSetting(plugin);
        var repository = new TestGameSettingRepository();
        var childImportService = new TestRecordChildImportService();
        var importer = new GameSettingImporter(repository, childImportService);
        var result = new RecordTypeImportResultDTO { RecordType = RecordTypeCatalog.GameSetting.RecordID };

        var importedAtUTC = DateTime.UtcNow;
        importer.Import(gameSetting, result, importedAtUTC);

        importer.RecordType.ShouldBe("GMST");
        importer.TableName.ShouldBe("GameSettings");
        importer.SupportedGames.ShouldBe([SupportedGame.Starfield, SupportedGame.Fallout4, SupportedGame.Skyrim], ignoreOrder: true);
        repository.Saved.ShouldBe([gameSetting]);
        childImportService.ReplaceRequests.ShouldBe([(gameSetting, RecordTypeCatalog.GameSetting.RecordID)]);
        gameSetting.ImportedAtUTC.ShouldBe(importedAtUTC);
        result.DetailRowsImported.ShouldBe(1);
    }

    private static PluginDTO CreatePlugin()
    {
        return new PluginDTO
        {
            Game = SupportedGame.Fallout4,
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

    private static GameSettingDTO CreateGameSetting(PluginDTO plugin)
    {
        return new GameSettingDTO
        {
            Game = plugin.Game,
            ModKey = plugin.ModKey,
            FormKey = new FormKeyDTO { ModKey = plugin.ModKey, Id = 10 },
            EditorID = "Setting",
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

    private sealed class TestGameSettingRepository : IGameSettingRepository
    {
        public IList<GameSettingDTO> Saved { get; } = new List<GameSettingDTO>();

        public IReadOnlyList<RecordTreeEntryDTO> GetRecordTreeEntriesByPlugin(SupportedGame game, ModKeyDTO modKey)
        {
            return [];
        }

        public IReadOnlyDictionary<string, int> GetRecordPluginCountsByGame(SupportedGame game)
        {
            return new Dictionary<string, int>();
        }

        public IReadOnlyList<GameSettingDTO> GetByFormKey(SupportedGame game, FormKeyDTO formKey)
        {
            return [];
        }

        public void Save(GameSettingDTO dto)
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
