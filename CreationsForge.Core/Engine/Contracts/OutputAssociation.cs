using Mutagen.Bethesda.Plugins;

namespace CreationsForge.Core.Engine.Contracts;

/// <summary>Identifies the intended output plugin and its native formatting choices.</summary>
public sealed class OutputAssociation
{
    /// <summary>Initializes an output association.</summary>
    /// <param name="pluginPath">The output plugin path, distinct from every source path.</param>
    /// <param name="modKey">The native plugin identity.</param>
    /// <param name="localizedOutputMode">How localized strings are represented.</param>
    /// <param name="masterStyle">The requested native master style.</param>
    /// <exception cref="ArgumentException">Thrown when the plugin path is empty or its file name differs from <paramref name="modKey"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when an output-mode value is undefined.</exception>
    public OutputAssociation(string pluginPath, ModKey modKey, LocalizedOutputMode localizedOutputMode, OutputMasterStyle masterStyle)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pluginPath);
        if (!string.Equals(System.IO.Path.GetFileName(pluginPath), modKey.FileName, StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("The output plugin path must identify the same native plugin as its ModKey.", nameof(modKey));
        }

        if (!Enum.IsDefined(localizedOutputMode))
        {
            throw new ArgumentOutOfRangeException(nameof(localizedOutputMode));
        }

        if (!Enum.IsDefined(masterStyle))
        {
            throw new ArgumentOutOfRangeException(nameof(masterStyle));
        }

        PluginPath = pluginPath;
        ModKey = modKey;
        LocalizedOutputMode = localizedOutputMode;
        MasterStyle = masterStyle;
    }

    /// <summary>Gets the output plugin path.</summary>
    public string PluginPath { get; }

    /// <summary>Gets the native output plugin identity and extension type.</summary>
    public ModKey ModKey { get; }

    /// <summary>Gets how localized strings are represented.</summary>
    public LocalizedOutputMode LocalizedOutputMode { get; }

    /// <summary>Gets the requested native master style.</summary>
    public OutputMasterStyle MasterStyle { get; }
}
