using CreationsForge.Core.DTOs.Games;
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.Importers.Interfaces;

public interface IGamePluginReader
{
    SupportedGame Game { get; }

    GameDTO ReadGame();

    IReadOnlyList<PluginLoadOrderEntryDTO> ReadLoadOrder();

    PluginSourceInfoDTO ReadSourceInfo(ModKeyDTO modKey);

    bool IsUnsupported(PluginLoadOrderEntryDTO loadOrderEntry);

    PluginDTO ReadPluginMetadata(PluginLoadOrderEntryDTO loadOrderEntry, PluginSourceInfoDTO sourceInfo);

    IReadOnlyList<PluginDTO> ReadPlugins();

    IReadOnlyList<PluginMasterReferenceDTO> ReadMasterReferences(PluginDTO plugin);
}
