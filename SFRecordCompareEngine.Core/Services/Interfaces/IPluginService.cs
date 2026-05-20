using SFRecordCompareEngine.Core.DTOs.Plugins;
using SFRecordCompareEngine.Core.DTOs.Records;

namespace SFRecordCompareEngine.Core.Services.Interfaces;

public interface IPluginService
{
    /// <summary>
    ///     Get the header for the specified plugin
    /// </summary>
    /// <param name="pluginName">The plugin to get the header for</param>
    /// <returns>The plugin header, or null if the plugin cannot be loaded or the header is malformed</returns>
    PluginHeaderDTO? GetPluginHeader(string pluginName);

    /// <summary>
    ///     Get the list of plugins for the game
    /// </summary>
    /// <returns>The list of plugin file names or an empty list</returns>
    IList<string> GetPlugins();

    /// <summary>
    ///     Get the supported major record type names.
    /// </summary>
    /// <returns>The supported major record type names.</returns>
    IList<string> GetRecordTypes();

    /// <summary>
    ///     Get record summaries for the specified plugin and major record type.
    /// </summary>
    /// <param name="pluginName">The plugin to load records from.</param>
    /// <param name="recordType">The major record type to load.</param>
    /// <returns>Record summaries containing FormID and EditorID.</returns>
    IList<RecordSummaryDTO> GetRecords(string pluginName, string recordType);

    /// <summary>
    ///     Get a side-by-side comparison for a record across the base plugin, editing masters, and the current plugin.
    /// </summary>
    /// <param name="pluginName">The current plugin to compare.</param>
    /// <param name="recordType">The major record type to load.</param>
    /// <param name="formKey">The selected record FormKey/FormID.</param>
    /// <returns>The comparison plugin columns and field rows.</returns>
    RecordComparisonDTO GetRecordComparison(string pluginName, string recordType, string formKey);
}