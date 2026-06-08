using CreationsForge.Core.Enums;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services.Interfaces;

namespace CreationsForge.Core.Services;

public class GameImportReadinessService : IGameImportReadinessService
{
    private readonly IPluginRepository PluginRepository;

    public GameImportReadinessService(IPluginRepository pluginRepository)
    {
        PluginRepository = pluginRepository;
    }

    public bool HasImportedData(SupportedGame game)
    {
        return PluginRepository.CountByGame(game) > 0;
    }
}
