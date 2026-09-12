using CreationsForge.Core.Engine.Contracts;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;

namespace CreationsForge.Core.Engine.NativeOutputs;

/// <summary>Describes one verified output plugin selection and its native header state.</summary>
public sealed class NativeOutputPluginInput
{
    /// <summary>Initializes a verified native output descriptor.</summary>
    /// <param name="path">The canonical absolute output plugin path.</param>
    /// <param name="modKey">The native output plugin identity.</param>
    /// <param name="exists">Whether the plugin existed when admitted.</param>
    /// <param name="masterStyle">The verified or requested native master style.</param>
    /// <param name="usesLocalization">Whether external localized strings are selected.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="path"/> is empty or not absolute.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="masterStyle"/> is undefined.</exception>
    internal NativeOutputPluginInput(
        string path,
        ModKey modKey,
        bool exists,
        MasterStyle masterStyle,
        bool usesLocalization)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        if (!System.IO.Path.IsPathFullyQualified(path))
        {
            throw new ArgumentException("A native output plugin requires a canonical absolute path.", nameof(path));
        }

        if (!Enum.IsDefined(masterStyle))
        {
            throw new ArgumentOutOfRangeException(nameof(masterStyle));
        }

        Path = path;
        ModPath = new ModPath(modKey, path);
        ModKey = modKey;
        Exists = exists;
        MasterStyle = masterStyle;
        UsesLocalization = usesLocalization;
    }

    /// <summary>Gets the canonical absolute output plugin path.</summary>
    public string Path { get; }

    /// <summary>Gets the native output plugin path and identity pair.</summary>
    public ModPath ModPath { get; }

    /// <summary>Gets the native output plugin identity.</summary>
    public ModKey ModKey { get; }

    /// <summary>Gets whether the output plugin existed when admitted.</summary>
    public bool Exists { get; }

    /// <summary>Gets the verified existing or requested new output master style.</summary>
    public MasterStyle MasterStyle { get; }

    /// <summary>Gets whether the output uses separate localized strings files.</summary>
    public bool UsesLocalization { get; }
}
