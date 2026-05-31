using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Plugins;

namespace SFRecordCompareEngine.Core.Repositories.Interfaces;

public interface IPluginRepository
{
    /// <summary>
    ///     Get the plugin by its mod key.
    /// </summary>
    /// <param name="modKey">The mod key to search for.</param>
    /// <returns>The plugin if found, otherwise null.</returns>
    PluginDTO? GetByModKey(ModKey modKey);

    /// <summary>
    ///     Get all plugins.
    /// </summary>
    /// <returns>The list of plugins or an empty list if none are found.</returns>
    IList<PluginDTO> GetAll();

    /// <summary>
    ///     Get all imported plugins.
    /// </summary>
    /// <returns>The list of plugins or an empty list if none are found.</returns>
    IList<PluginDTO> GetImportedPlugins();

    /// <summary>
    ///     Get openable plugins.
    /// </summary>
    /// <returns>The list of plugins or an empty list if none are found.</returns>
    IList<PluginDTO> GetOpenablePlugins();

    /// <summary>
    ///     Search plugins by filename.
    /// </summary>
    /// <param name="searchFilename">The filename pattern to search for.</param>
    /// <returns>The list of matching plugins or an empty list if none are found.</returns>
    IList<PluginDTO> SearchPluginsByFilename(string searchFilename);

    /// <summary>
    ///     Search openable plugins by filename.
    /// </summary>
    /// <param name="searchFilename">The filename pattern to search for.</param>
    /// <returns>The list of matching plugins or an empty list if none are found.</returns>
    IList<PluginDTO> SearchOpenablePluginsByFilename(string searchFilename);

    /// <summary>
    ///     Save a plugin.
    /// </summary>
    /// <param name="dto">The plugin to save.</param>
    void Save(PluginDTO dto);
}