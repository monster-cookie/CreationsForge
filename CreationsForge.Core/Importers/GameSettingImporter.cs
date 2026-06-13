using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Results;
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Importers.Interfaces;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services.Interfaces;

namespace CreationsForge.Core.Importers;

public class GameSettingImporter : ITypedRecordImporter
{
    private readonly IGameSettingRepository GameSettingRepository;
    private readonly IRecordChildImportService RecordChildImportService;

    public GameSettingImporter(
        IGameSettingRepository gameSettingRepository,
        IRecordChildImportService recordChildImportService)
    {
        GameSettingRepository = gameSettingRepository;
        RecordChildImportService = recordChildImportService;
    }

    public string RecordType => RecordTypeCatalog.GameSetting.RecordID;

    public string TableName => RecordTypeCatalog.GameSetting.TableName;

    public IReadOnlySet<SupportedGame> SupportedGames { get; } = new HashSet<SupportedGame>
    {
        SupportedGame.Starfield,
        SupportedGame.Fallout4,
        SupportedGame.Skyrim
    };

    public void Import(object recordDTO, RecordTypeImportResultDTO result, DateTime importedAtUTC)
    {
        if (recordDTO is not GameSettingDTO gameSetting) throw new ArgumentException($"Expected {nameof(GameSettingDTO)}.", nameof(recordDTO));

        gameSetting.ImportedAtUTC = importedAtUTC;
        GameSettingRepository.Save(gameSetting);
        RecordChildImportService.ReplaceRecordChildren(gameSetting, RecordTypeCatalog.GameSetting.RecordID);
        result.DetailRowsImported++;
    }

    public void DeleteStaleRecords(PluginDTO plugin, DateTime importedAtUTC)
    {
        GameSettingRepository.DeleteStaleByPlugin(plugin.Game, plugin.ModKey, importedAtUTC);
    }
}
