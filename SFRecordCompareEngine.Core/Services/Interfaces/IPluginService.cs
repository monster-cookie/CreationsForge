using SFRecordCompareEngine.Core.DTOs.Plugins;

namespace SFRecordCompareEngine.Core.Services.Interfaces;

public interface IPluginService
{
    /// <summary>
    /// Get the header for the specified plugin
    /// </summary>
    /// <param name="pluginName">The plugin to get the header for</param>
    /// <returns>The plugin header, or null if the plugin cannot be loaded or the header is malformed</returns>
    PluginHeaderDTO? GetPluginHeader(string pluginName);
    
    /// <summary>
    /// Get the list of ESM entries for the game
    /// </summary>
    /// <returns>The list of ESM file names or en empty list</returns>
    IList<string> GetDatabases();
}