using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.Services.Interfaces;

public interface IPluginSelectionService
{
    IReadOnlyList<PluginDTO> GetOpenablePlugins(SupportedGame game);

    IReadOnlyList<PluginDTO> SearchOpenablePluginsByFilename(SupportedGame game, string searchFilename);

    long GetImportedRecordCount(SupportedGame game);
}
