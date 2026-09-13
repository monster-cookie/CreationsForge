using Mutagen.Bethesda.Plugins;

namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Describes one plugin participating in the current ephemeral workspace.
/// </summary>
public sealed class PluginSummary
{
    /// <summary>Initializes a plugin summary.</summary>
    /// <param name="modKey">The plugin identity.</param>
    /// <param name="path">The canonical plugin path.</param>
    /// <param name="loadOrderIndex">The zero-based position in the explicit load order.</param>
    /// <param name="role">The plugin's role in the workspace.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="path"/> is empty or whitespace.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="loadOrderIndex"/> is negative.</exception>
    public PluginSummary(ModKey modKey, string path, int loadOrderIndex, PluginRole role)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentOutOfRangeException.ThrowIfNegative(loadOrderIndex);
        ModKey = modKey;
        Path = path;
        LoadOrderIndex = loadOrderIndex;
        Role = role;
    }

    /// <summary>Gets the plugin identity.</summary>
    public ModKey ModKey { get; }

    /// <summary>Gets the canonical plugin path.</summary>
    public string Path { get; }

    /// <summary>Gets the zero-based position in the explicit load order.</summary>
    public int LoadOrderIndex { get; }

    /// <summary>Gets the plugin's role in the workspace.</summary>
    public PluginRole Role { get; }
}
