using CreationsForge.Core.DTOs.Games;
using CreationsForge.Core.DTOs.Plugins;

namespace CreationsForge.Fallout4.Interfaces;

public interface IFallout4PluginReaderService
{
    GameDTO ReadGame();

    IReadOnlyList<PluginLoadOrderEntryDTO> ReadLoadOrder();

    PluginSourceInfoDTO ReadSourceInfo(ModKeyDTO modKey);

    bool IsUnsupported(PluginLoadOrderEntryDTO loadOrderEntry);

    PluginDTO ReadPluginMetadata(PluginLoadOrderEntryDTO loadOrderEntry, PluginSourceInfoDTO sourceInfo);

    IReadOnlyList<PluginMasterReferenceDTO> ReadMasterReferences(PluginDTO plugin);
}
