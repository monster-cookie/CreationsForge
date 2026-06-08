using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.Repositories.Interfaces;

public interface IPluginRepository
{
    int CountByGame(SupportedGame game);

    long GetImportedRecordCountByGame(SupportedGame game);

    PluginDTO? GetByModKey(SupportedGame game, ModKeyDTO modKey);

    IReadOnlyList<PluginDTO> GetOpenablePlugins(SupportedGame game);

    IReadOnlyList<PluginDTO> SearchOpenablePluginsByFilename(SupportedGame game, string searchFilename);

    void Save(PluginDTO dto);
}
