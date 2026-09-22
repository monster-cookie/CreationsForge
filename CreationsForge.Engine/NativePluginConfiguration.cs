using Mutagen.Bethesda.Plugins;

namespace CreationsForge.Engine;

/// <summary>Describes how a plugin stores localized text.</summary>
public enum NativeTextStorageMode
{
    /// <summary>Strings are stored directly in plugin records.</summary>
    Embedded,

    /// <summary>Strings are stored in the game's external localization files.</summary>
    Localized,
}

/// <summary>Defines the mutable native output owned by a workspace.</summary>
public sealed class NativeOutputDefinition
{
    /// <summary>Initializes an output definition.</summary>
    /// <param name="path">The output plugin path.</param>
    /// <param name="modKey">The output identity, including its explicit plugin extension.</param>
    /// <param name="masterStyle">The requested native master style.</param>
    /// <param name="textStorageMode">The requested text storage mode.</param>
    /// <param name="createNew">Whether the workspace must create an empty output instead of opening an existing plugin.</param>
    public NativeOutputDefinition(
        string path,
        ModKey modKey,
        MasterStyle masterStyle,
        NativeTextStorageMode textStorageMode,
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

    /// <summary>Gets the native master style.</summary>
    public MasterStyle MasterStyle { get; }

    /// <summary>Gets the text storage mode.</summary>
    public NativeTextStorageMode TextStorageMode { get; }

    /// <summary>Gets whether the workspace must create an empty output.</summary>
    public bool CreateNew { get; }
}
