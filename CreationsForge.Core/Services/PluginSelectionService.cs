using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services.Interfaces;

namespace CreationsForge.Core.Services;

public class PluginSelectionService : IPluginSelectionService
{
    private readonly IPluginRepository PluginRepository;

    public PluginSelectionService(IPluginRepository pluginRepository)
    {
        PluginRepository = pluginRepository;
    }

    public IReadOnlyList<PluginDTO> GetOpenablePlugins(SupportedGame game)
    {
        return PluginRepository.GetOpenablePlugins(game);
    }

    public IReadOnlyList<PluginDTO> SearchOpenablePluginsByFilename(SupportedGame game, string searchFilename)
    {
        return string.IsNullOrWhiteSpace(searchFilename)
            ? PluginRepository.GetOpenablePlugins(game)
            : PluginRepository.SearchOpenablePluginsByFilename(game, searchFilename);
    }

    public long GetImportedRecordCount(SupportedGame game)
    {
        return PluginRepository.GetImportedRecordCountByGame(game);
    }
}
