using Mutagen.Bethesda.Plugins;

namespace CreationsForge.Engine.Workspaces;

/// <summary>Describes how a plugin stores localized text.</summary>
public enum PluginTextStorageMode
{
    /// <summary>Strings are stored directly in plugin records.</summary>
    Embedded,

    /// <summary>Strings are stored in the game's external localization files.</summary>
    Localized,
}

/// <summary>Defines the mutable Mutagen plugin output owned by a workspace.</summary>
public sealed class PluginOutputDefinition
{
    /// <summary>Initializes an output definition.</summary>
    /// <param name="path">The output plugin path.</param>
    /// <param name="modKey">The output identity, including its explicit plugin extension.</param>
    /// <param name="masterStyle">The requested plugin master style.</param>
    /// <param name="textStorageMode">The requested text storage mode.</param>
    /// <param name="createNew">Whether the workspace must create an empty output instead of opening an existing plugin.</param>
    public PluginOutputDefinition(
        string path,
        ModKey modKey,
        MasterStyle masterStyle,
        PluginTextStorageMode textStorageMode,
        bool createNew)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        Path = path;
        ModKey = modKey;
        MasterStyle = masterStyle;
        TextStorageMode = textStorageMode;
        CreateNew = createNew;
    }

    /// <summary>Gets the output plugin path.</summary>
    public string Path { get; }

    /// <summary>Gets the output identity, including its plugin extension.</summary>
    public ModKey ModKey { get; }

    /// <summary>Gets the plugin master style.</summary>
    public MasterStyle MasterStyle { get; }

    /// <summary>Gets the text storage mode.</summary>
    public PluginTextStorageMode TextStorageMode { get; }

    /// <summary>Gets whether the workspace must create an empty output.</summary>
    public bool CreateNew { get; }
}
