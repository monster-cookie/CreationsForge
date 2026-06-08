using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Importers.Interfaces;
using CreationsForge.Starfield.DTOs;
using CreationsForge.Starfield.Repositories.Interfaces;

namespace CreationsForge.Starfield.Importers;

public class StarfieldPluginExtensionImporter : IPluginExtensionImporter
{
    private readonly IStarfieldPluginRepository StarfieldPluginRepository;

    public StarfieldPluginExtensionImporter(IStarfieldPluginRepository starfieldPluginRepository)
    {
        StarfieldPluginRepository = starfieldPluginRepository;
    }

    public bool CanImport(PluginDTO plugin)
    {
        return plugin is StarfieldPluginDTO;
    }

    public void Import(PluginDTO plugin)
    {
        if (plugin is not StarfieldPluginDTO starfieldPlugin) throw new ArgumentException("Plugin must be a Starfield plugin.", nameof(plugin));

        StarfieldPluginRepository.Save(starfieldPlugin);
    }
}
