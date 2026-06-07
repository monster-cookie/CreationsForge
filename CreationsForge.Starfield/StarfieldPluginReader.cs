using CreationsForge.Core.DTOs.Games;
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Importers.Interfaces;
using CreationsForge.Starfield.Interfaces;

namespace CreationsForge.Starfield;

public class StarfieldPluginReader : IGamePluginReader
{
    private readonly IStarfieldPluginReaderService PluginReaderService;

    public StarfieldPluginReader(IStarfieldPluginReaderService pluginReaderService)
    {
        PluginReaderService = pluginReaderService;
    }

    public SupportedGame Game => SupportedGame.Starfield;

    public GameDTO ReadGame()
    {
        return PluginReaderService.ReadGame();
    }

    public IReadOnlyList<PluginDTO> ReadPlugins()
    {
        return ReadLoadOrder()
            .Select(loadOrderEntry => ReadPluginMetadata(loadOrderEntry, ReadSourceInfo(loadOrderEntry.ModKey)))
            .ToList();
    }

    public IReadOnlyList<PluginLoadOrderEntryDTO> ReadLoadOrder()
    {
        return PluginReaderService.ReadLoadOrder();
    }

    public PluginSourceInfoDTO ReadSourceInfo(ModKeyDTO modKey)
    {
        return PluginReaderService.ReadSourceInfo(modKey);
    }

    public bool IsUnsupported(PluginLoadOrderEntryDTO loadOrderEntry)
    {
        return PluginReaderService.IsUnsupported(loadOrderEntry);
    }

    public PluginDTO ReadPluginMetadata(PluginLoadOrderEntryDTO loadOrderEntry, PluginSourceInfoDTO sourceInfo)
    {
        return PluginReaderService.ReadPluginMetadata(loadOrderEntry, sourceInfo);
    }

    public IReadOnlyList<PluginMasterReferenceDTO> ReadMasterReferences(PluginDTO plugin)
    {
        return PluginReaderService.ReadMasterReferences(plugin);
    }
}
