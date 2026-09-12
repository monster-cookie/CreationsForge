using CreationsForge.Core.Engine.Contracts;
using Mutagen.Bethesda.Plugins;

namespace CreationsForge.Core.Engine.NativeInputs;

/// <summary>
/// Describes one verified explicit plugin input and the native header style required to decode its record identities.
/// </summary>
public sealed class NativeSourcePluginInput
{
    /// <summary>Initializes one verified native plugin descriptor.</summary>
    /// <param name="path">The canonical absolute plugin path.</param>
    /// <param name="modKey">The native plugin identity derived from the file name.</param>
    /// <param name="loadOrderIndex">The zero-based position in the explicit load order.</param>
    /// <param name="role">The plugin's source or load-order role.</param>
    /// <param name="masterStyle">The native master style read from the plugin header.</param>
    /// <param name="usesLocalization">Whether the native header selects external strings files.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="path"/> is empty or not absolute.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the index, role, or style is invalid.</exception>
    public NativeSourcePluginInput(
        string path,
        ModKey modKey,
        int loadOrderIndex,
        PluginRole role,
        MasterStyle masterStyle,
        bool usesLocalization)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        if (!System.IO.Path.IsPathFullyQualified(path))
        {
            throw new ArgumentException("A native plugin input requires a canonical absolute path.", nameof(path));
        }

        ArgumentOutOfRangeException.ThrowIfNegative(loadOrderIndex);
        if (role is not PluginRole.Source and not PluginRole.LoadOrder)
        {
            throw new ArgumentOutOfRangeException(nameof(role), "A source input must be the selected source or a load-order participant.");
        }

        if (!Enum.IsDefined(masterStyle))
        {
            throw new ArgumentOutOfRangeException(nameof(masterStyle));
        }

        Path = path;
        ModPath = new ModPath(modKey, path);
        ModKey = modKey;
        LoadOrderIndex = loadOrderIndex;
        Role = role;
        MasterStyle = masterStyle;
        UsesLocalization = usesLocalization;
    }

    /// <summary>Gets the canonical absolute plugin path.</summary>
    public string Path { get; }

    /// <summary>Gets the native plugin path and identity pair.</summary>
    public ModPath ModPath { get; }

    /// <summary>Gets the native plugin identity.</summary>
    public ModKey ModKey { get; }

    /// <summary>Gets the zero-based position in the explicit load order.</summary>
    public int LoadOrderIndex { get; }

    /// <summary>Gets the plugin's source or load-order role.</summary>
    public PluginRole Role { get; }

    /// <summary>Gets the native master style read from the plugin header.</summary>
    public MasterStyle MasterStyle { get; }

    /// <summary>Gets whether the native plugin header selects external localized-string files.</summary>
    public bool UsesLocalization { get; }
}
