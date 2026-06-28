using CreationsForge.Core.Configuration.Interfaces;
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services.Interfaces;

namespace CreationsForge.Core.Services;

/// <summary>
/// Provides UI-neutral plugin list queries used by active-plugin selection workflows.
/// </summary>
public class PluginSelectionService : IPluginSelectionService
{
    private readonly IApplicationConfigurationStore ConfigurationStore;
    private readonly IPluginRepository PluginRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="PluginSelectionService"/> class.
    /// </summary>
    /// <param name="pluginRepository">Repository used to read imported plugin rows.</param>
    /// <param name="configurationStore">Configuration source for plugin-list display preferences.</param>
    public PluginSelectionService(IPluginRepository pluginRepository, IApplicationConfigurationStore configurationStore)
    {
        ConfigurationStore = configurationStore;
        PluginRepository = pluginRepository;
    }

    /// <inheritdoc />
    public IReadOnlyList<PluginDTO> GetOpenablePlugins(SupportedGame game)
    {
        return ApplyPluginSelectionPreferences(PluginRepository.GetOpenablePlugins(game));
    }

    /// <inheritdoc />
    public IReadOnlyList<PluginDTO> SearchOpenablePluginsByFilename(SupportedGame game, string searchFilename)
    {
        var plugins = string.IsNullOrWhiteSpace(searchFilename)
            ? PluginRepository.GetOpenablePlugins(game)
            : PluginRepository.SearchOpenablePluginsByFilename(game, searchFilename);
        return ApplyPluginSelectionPreferences(plugins);
    }

    /// <inheritdoc />
    public long GetImportedRecordCount(SupportedGame game)
    {
        return PluginRepository.GetImportedRecordCountByGame(game);
    }

    /// <summary>
    /// Applies non-destructive display preferences to a plugin query result before it reaches presentation code.
    /// </summary>
    /// <param name="plugins">The repository plugin rows to filter for display.</param>
    /// <returns>The plugin rows after configured display-only filtering has been applied.</returns>
    private IReadOnlyList<PluginDTO> ApplyPluginSelectionPreferences(IReadOnlyList<PluginDTO> plugins)
    {
        return ConfigurationStore.Current.PreferEspOverMatchingEsm
            ? FilterMatchingEsmWhenEspExists(plugins)
            : plugins;
    }

    /// <summary>
    /// Removes ESM rows from a display list when an ESP with the same base filename is also present.
    /// </summary>
    /// <param name="plugins">The plugin rows to inspect.</param>
    /// <returns>A filtered plugin list that prefers matching ESP rows over ESM rows.</returns>
    private static IReadOnlyList<PluginDTO> FilterMatchingEsmWhenEspExists(IReadOnlyList<PluginDTO> plugins)
    {
        var espBaseNames = plugins
            .Where(plugin => string.Equals(Path.GetExtension(plugin.ModKey.FileName), ".esp", StringComparison.OrdinalIgnoreCase))
            .Select(plugin => Path.GetFileNameWithoutExtension(plugin.ModKey.FileName))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (espBaseNames.Count == 0)
        {
            return plugins;
        }

        return plugins
            .Where(plugin => !IsMatchingEsm(plugin, espBaseNames))
            .ToList();
    }

    /// <summary>
    /// Determines whether a plugin row is an ESM that should be hidden because a matching ESP is available.
    /// </summary>
    /// <param name="plugin">The plugin row to inspect.</param>
    /// <param name="espBaseNames">The base filenames for ESP rows in the same result set.</param>
    /// <returns><see langword="true"/> when the row is a matching ESM; otherwise, <see langword="false"/>.</returns>
    private static bool IsMatchingEsm(PluginDTO plugin, ISet<string> espBaseNames)
    {
        return string.Equals(Path.GetExtension(plugin.ModKey.FileName), ".esm", StringComparison.OrdinalIgnoreCase) &&
            espBaseNames.Contains(Path.GetFileNameWithoutExtension(plugin.ModKey.FileName));
    }
}
