using System.IO;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins.Records.Mapping;
using Serilog;
using SFRecordCompareEngine.Core.Database.Interfaces;
using SFRecordCompareEngine.Core.Configuration.Interfaces;
using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.Repositories.Interfaces;
using SFRecordCompareEngine.Core.Services.Interfaces;

namespace SFRecordCompareEngine.Core.Services;

public class PluginService : IPluginService
{
    private readonly ILogger Logger = Log.ForContext<PluginService>();

    private readonly IGameConfigurationStore GameConfigurationStore;
    private readonly IRecordService RecordService;
    private readonly ISqliteConnectionFactory SqliteConnectionFactory;
    private readonly IPluginRepository PluginRepository;
    private readonly IGameSettingRepository GameSettingRepository;

    public PluginService(
        IGameConfigurationStore gameConfigurationStore,
        IRecordService recordService,
        ISqliteConnectionFactory sqliteConnectionFactory,
        IPluginRepository pluginRepository,
        IGameSettingRepository gameSettingRepository)
    {
        GameConfigurationStore = gameConfigurationStore;
        RecordService = recordService;
        SqliteConnectionFactory = sqliteConnectionFactory;
        PluginRepository = pluginRepository;
        GameSettingRepository = gameSettingRepository;
    }

    /// <inheritdoc />
    public IList<string> GetRecordTypes()
    {
        switch (GameConfigurationStore.SelectedGame)
        {
            case "Starfield":
                return MajorRecordTypeEnumerator
                    .GetMajorRecordTypesFor(GameCategory.Starfield)
                    .OrderBy(x => x.ClassType.Name)
                    .Select(x => x.ClassType.Name)
                    .ToList();
            case "Skyrim":
                return MajorRecordTypeEnumerator
                    .GetMajorRecordTypesFor(GameCategory.Skyrim)
                    .OrderBy(x => x.ClassType.Name)
                    .Select(x => x.ClassType.Name)
                    .ToList();
            case "Fallout4":
                return MajorRecordTypeEnumerator
                    .GetMajorRecordTypesFor(GameCategory.Fallout4)
                    .OrderBy(x => x.ClassType.Name)
                    .Select(x => x.ClassType.Name)
                    .ToList();
            default:
                Logger.Error("Game '{Game}' is not supported. Please select Starfield", GameConfigurationStore.SelectedGame);
                return new List<string>();
        }
    }

    /// <inheritdoc />
    public IList<PluginLoadOrderEntryDTO> GetLoadOrder()
    {
        var gameEnvironment = GameConfigurationStore.Game ?? throw new InvalidOperationException("No game environment is configured.");

        return gameEnvironment.LoadOrder.ListedOrder
            .Select((plugin, index) => new PluginLoadOrderEntryDTO
            {
                ModKey = plugin.ModKey,
                PluginFileName = plugin.FileName,
                PluginPath = Path.Join(gameEnvironment.DataFolderPath, plugin.FileName),
                LoadOrderIndex = index,
                Enabled = plugin.Enabled
            })
            .ToList();
    }
}
