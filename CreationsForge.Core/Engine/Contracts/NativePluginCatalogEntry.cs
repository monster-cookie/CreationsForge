using Mutagen.Bethesda.Plugins;

namespace CreationsForge.Core.Engine.Contracts;

/// <summary>Describes one installed plugin and the immutable dependencies needed to open it for editing.</summary>
public sealed class NativePluginCatalogEntry
{
    /// <summary>Initializes one detached installed-plugin catalog entry.</summary>
    /// <param name="modKey">The native plugin identity.</param>
    /// <param name="pluginPath">The absolute plugin path.</param>
    /// <param name="loadOrderIndex">The zero-based installed load-order position.</param>
    /// <param name="enabled">Whether the game load order enables the plugin.</param>
    /// <param name="dependencyPluginPaths">The plugin's transitive declared masters in installed load-order order.</param>
    /// <param name="localizedOutputMode">The localization representation read from the native header.</param>
    /// <param name="masterStyle">The master style read from the native header.</param>
    /// <param name="canEdit">Whether the plugin has enough dependency context to open as an editable output.</param>
    /// <param name="unavailableReason">The user-facing reason editing is unavailable, or <see langword="null"/>.</param>
    /// <param name="declaredMasters">The direct parent masters read from the plugin header.</param>
    /// <param name="author">The optional author read from the plugin header.</param>
    /// <param name="description">The optional description read from the plugin header.</param>
    public NativePluginCatalogEntry(
        ModKey modKey,
        string pluginPath,
        int loadOrderIndex,
        bool enabled,
        IReadOnlyList<string> dependencyPluginPaths,
        LocalizedOutputMode localizedOutputMode,
        OutputMasterStyle masterStyle,
        bool canEdit,
        string? unavailableReason,
        IReadOnlyList<ModKey>? declaredMasters = null,
        string? author = null,
        string? description = null)
    {
        ArgumentNullException.ThrowIfNull(dependencyPluginPaths);
        ModKey = modKey;
        PluginPath = pluginPath;
        LoadOrderIndex = loadOrderIndex;
        Enabled = enabled;
        DependencyPluginPaths = Array.AsReadOnly(dependencyPluginPaths.ToArray());
        LocalizedOutputMode = localizedOutputMode;
        MasterStyle = masterStyle;
        CanEdit = canEdit;
        UnavailableReason = unavailableReason;
        DeclaredMasters = Array.AsReadOnly((declaredMasters ?? []).ToArray());
        Author = author;
        Description = description;
    }

    /// <summary>Gets the native plugin identity.</summary>
    public ModKey ModKey { get; }

    /// <summary>Gets the absolute plugin path.</summary>
    public string PluginPath { get; }

    /// <summary>Gets the zero-based installed load-order position.</summary>
    public int LoadOrderIndex { get; }

    /// <summary>Gets whether the game load order enables the plugin.</summary>
    public bool Enabled { get; }

    /// <summary>Gets the plugin's transitive declared masters in installed load-order order.</summary>
    public IReadOnlyList<string> DependencyPluginPaths { get; }

    /// <summary>Gets the localization representation read from the native header.</summary>
    public LocalizedOutputMode LocalizedOutputMode { get; }

    /// <summary>Gets the master style read from the native header.</summary>
    public OutputMasterStyle MasterStyle { get; }

    /// <summary>Gets whether the plugin can be opened as an editable output.</summary>
    public bool CanEdit { get; }

    /// <summary>Gets the user-facing reason editing is unavailable, or <see langword="null"/>.</summary>
    public string? UnavailableReason { get; }

    /// <summary>Gets the direct parent masters read from the plugin header.</summary>
    public IReadOnlyList<ModKey> DeclaredMasters { get; }

    /// <summary>Gets the optional author read from the plugin header.</summary>
    public string? Author { get; }

    /// <summary>Gets the optional description read from the plugin header.</summary>
    public string? Description { get; }
}
