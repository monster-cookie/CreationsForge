using CreationsForge.Core.Enums;
using Mutagen.Bethesda;

namespace CreationsForge.Core.Engine.Contracts;

/// <summary>Contains one installed game's detected data directory and ordered plugins.</summary>
public sealed class PluginCatalog
{
    /// <summary>Initializes one immutable installed-plugin catalog.</summary>
    /// <param name="game">The CreationsForge game.</param>
    /// <param name="release">The exact plugin game release.</param>
    /// <param name="dataDirectoryPath">The detected installed data directory.</param>
    /// <param name="plugins">The detached plugin entries in load-order order.</param>
    public PluginCatalog(
        SupportedGame game,
        GameRelease release,
        string dataDirectoryPath,
        IReadOnlyList<PluginCatalogEntry> plugins)
    {
        ArgumentNullException.ThrowIfNull(plugins);
        Game = game;
        Release = release;
        DataDirectoryPath = dataDirectoryPath;
        Plugins = Array.AsReadOnly(plugins.ToArray());
    }

    /// <summary>Gets the CreationsForge game.</summary>
    public SupportedGame Game { get; }

    /// <summary>Gets the exact plugin game release.</summary>
    public GameRelease Release { get; }

    /// <summary>Gets the detected installed data directory.</summary>
    public string DataDirectoryPath { get; }

    /// <summary>Gets the detached plugin entries in load-order order.</summary>
    public IReadOnlyList<PluginCatalogEntry> Plugins { get; }
}
