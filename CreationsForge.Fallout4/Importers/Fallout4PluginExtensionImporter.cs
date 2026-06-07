using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Importers.Interfaces;
using CreationsForge.Fallout4.DTOs;
using CreationsForge.Fallout4.Repositories.Interfaces;

namespace CreationsForge.Fallout4.Importers;

public class Fallout4PluginExtensionImporter : IPluginExtensionImporter
{
    private readonly IFallout4PluginRepository Fallout4PluginRepository;

    public Fallout4PluginExtensionImporter(IFallout4PluginRepository fallout4PluginRepository)
    {
        Fallout4PluginRepository = fallout4PluginRepository;
    }

    public bool CanImport(PluginDTO plugin)
    {
        return plugin is Fallout4PluginDTO;
    }

    public void Import(PluginDTO plugin)
    {
        if (plugin is not Fallout4PluginDTO fallout4Plugin) throw new ArgumentException("Plugin must be a Fallout 4 plugin.", nameof(plugin));

        Fallout4PluginRepository.Save(fallout4Plugin);
    }
}
