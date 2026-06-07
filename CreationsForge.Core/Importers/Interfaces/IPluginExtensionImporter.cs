using CreationsForge.Core.DTOs.Plugins;

namespace CreationsForge.Core.Importers.Interfaces;

public interface IPluginExtensionImporter
{
    bool CanImport(PluginDTO plugin);

    void Import(PluginDTO plugin);
}
