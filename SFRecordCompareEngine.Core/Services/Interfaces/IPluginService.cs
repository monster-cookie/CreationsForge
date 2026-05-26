using SFRecordCompareEngine.Core.DTOs.Plugins;

namespace SFRecordCompareEngine.Core.Services.Interfaces;

public interface IPluginService
{
    /// <summary>
    /// Get the selected game's plugin load order.
    /// </summary>
    /// <returns>The plugin load-order entries.</returns>
    IList<PluginLoadOrderEntryDTO> GetLoadOrder();

    /// <summary>
    /// Get the supported major record type names.
    /// </summary>
    /// <returns>The supported major record type names.</returns>
    IList<string> GetRecordTypes();
}
