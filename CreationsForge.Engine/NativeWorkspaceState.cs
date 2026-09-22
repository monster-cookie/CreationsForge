using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;

namespace CreationsForge.Engine;

/// <summary>Reports constant-time native workspace and output state.</summary>
public sealed class NativeWorkspaceState
{
    internal NativeWorkspaceState(
        GameRelease release,
        ModKey outputModKey,
        string outputPath,
        MasterStyle masterStyle,
        NativeTextStorageMode textStorageMode,
        bool isNewOutput,
        bool isDirty,
        ulong revision)
    {
        Release = release;
        OutputModKey = outputModKey;
        OutputPath = outputPath;
        MasterStyle = masterStyle;
        TextStorageMode = textStorageMode;
        IsNewOutput = isNewOutput;
        IsDirty = isDirty;
        Revision = revision;
    }

    /// <summary>Gets the workspace game release.</summary>
    public GameRelease Release { get; }

    /// <summary>Gets the mutable output identity.</summary>
    public ModKey OutputModKey { get; }

    /// <summary>Gets the absolute output path.</summary>
    public string OutputPath { get; }

    /// <summary>Gets the output master style.</summary>
    public MasterStyle MasterStyle { get; }

    /// <summary>Gets the output text storage mode.</summary>
    public NativeTextStorageMode TextStorageMode { get; }

    /// <summary>Gets whether the output was created empty for this workspace.</summary>
    public bool IsNewOutput { get; }

    /// <summary>Gets whether the mutable output has unsaved changes.</summary>
    public bool IsDirty { get; }

    /// <summary>Gets the logical output revision.</summary>
    public ulong Revision { get; }
}
