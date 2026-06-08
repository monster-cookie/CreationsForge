using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;

namespace CreationsForge.Core.Repositories.Interfaces;

public interface IPluginMasterReferenceRepository
{
    void Save(PluginMasterReferenceDTO dto);

    void DeleteStaleByPlugin(SupportedGame game, ModKeyDTO pluginModKey, DateTime importedAtUTC);
}
