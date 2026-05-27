using System.IO;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Plugins.Records.Mapping;
using Mutagen.Bethesda.Starfield;
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

    private readonly IRecordService RecordService;
    private readonly ISqliteConnectionFactory SqliteConnectionFactory;
    private readonly IPluginRepository PluginRepository;
    private readonly IGameSettingRepository GameSettingRepository;

    public PluginService(
        IRecordService recordService,
        ISqliteConnectionFactory sqliteConnectionFactory,
        IPluginRepository pluginRepository,
        IGameSettingRepository gameSettingRepository)
    {
        RecordService = recordService;
        SqliteConnectionFactory = sqliteConnectionFactory;
        PluginRepository = pluginRepository;
        GameSettingRepository = gameSettingRepository;
    }

    /// <inheritdoc />
    public IList<string> GetRecordTypes()
    {
        return MajorRecordTypeEnumerator
            .GetMajorRecordTypesFor(GameCategory.Starfield)
            .OrderBy(x => x.ClassType.Name)
            .Select(x => x.ClassType.Name)
            .ToList();
    }

    /// <inheritdoc />
    public IList<PluginLoadOrderEntryDTO> GetLoadOrder()
    {
        return GameEnvironment.Typical.Starfield(StarfieldRelease.Starfield).LoadOrder.ListedOrder
            .Select((plugin, index) => new PluginLoadOrderEntryDTO
            {
                ModKey = plugin.ModKey,
                PluginFileName = plugin.FileName,
                PluginPath = Path.Join(GameEnvironment.Typical.Starfield(StarfieldRelease.Starfield).DataFolderPath, plugin.FileName),
                LoadOrderIndex = index,
                Enabled = plugin.Enabled
            })
            .ToList();
    }
}
