using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Importers.Interfaces;
using CreationsForge.Skyrim.DTOs;
using CreationsForge.Skyrim.Repositories.Interfaces;

namespace CreationsForge.Skyrim.Importers;

public class SkyrimPluginExtensionImporter : IPluginExtensionImporter
{
    private readonly ISkyrimPluginRepository SkyrimPluginRepository;

    public SkyrimPluginExtensionImporter(ISkyrimPluginRepository skyrimPluginRepository)
    {
        SkyrimPluginRepository = skyrimPluginRepository;
    }

    public bool CanImport(PluginDTO plugin)
    {
        return plugin is SkyrimPluginDTO;
    }

    public void Import(PluginDTO plugin)
    {
        if (plugin is not SkyrimPluginDTO skyrimPlugin) throw new ArgumentException("Plugin must be a Skyrim plugin.", nameof(plugin));

        SkyrimPluginRepository.Save(skyrimPlugin);
    }
}
