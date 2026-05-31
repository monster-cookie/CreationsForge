using Mutagen.Bethesda.Plugins;
using SFRecordCompareEngine.Core.DTOs.Plugins;

namespace SFRecordCompareEngine.Core.Services.Interfaces;

public interface IStarfieldPluginReaderService
{
    /// <summary>
    /// Get the data folder path.
    /// </summary>
    /// <returns></returns>
    string GetDataFolderPath();
    
    /// <summary>
    /// Get the load order.
    /// </summary>
    /// <returns>The list of plugins by load order</returns>
    IList<PluginLoadOrderEntryDTO> GetLoadOrder();
    
    /// <summary>
    /// Get the source info for a given plugin.
    /// </summary>
    /// <param name="modKey">The mod key for the plugin</param>
    /// <returns>The plugin file info</returns>
    PluginSourceInfoDTO GetSourceInfo(ModKey modKey);
    
    /// <summary>
    /// Get the metadata for a given plugin.
    /// </summary>
    /// <param name="modKey">The mod key for the plugin</param>
    /// <returns>The plugin metadata</returns>
    StarfieldPluginMetadataDTO GetMetadata(ModKey modKey);
}
