using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;

namespace CreationsForge.Engine.Workspaces;

/// <summary>Reports constant-time plugin workspace and output state.</summary>
public sealed class PluginWorkspaceState
{
    /// <summary>Initializes a complete constant-time workspace state snapshot.</summary>
    internal PluginWorkspaceState(
        GameRelease release,
        ModKey outputModKey,
        string outputPath,
        MasterStyle masterStyle,
        PluginTextStorageMode textStorageMode,
        bool isNewOutput,
        bool isDirty,
        ulong revision,
        bool requiresReopen = false)
    {
        Release = release;
        OutputModKey = outputModKey;
        OutputPath = outputPath;
        MasterStyle = masterStyle;
        TextStorageMode = textStorageMode;
        IsNewOutput = isNewOutput;
        IsDirty = isDirty;
        Revision = revision;
        RequiresReopen = requiresReopen;
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
    public PluginTextStorageMode TextStorageMode { get; }

    /// <summary>Gets whether the output was created empty for this workspace.</summary>
    public bool IsNewOutput { get; }

    /// <summary>Gets whether the mutable output has unsaved changes.</summary>
    public bool IsDirty { get; }

    /// <summary>Gets the logical output revision.</summary>
    public ulong Revision { get; }

    /// <summary>Gets whether persistence reached a state that requires closing and reopening this workspace.</summary>
    public bool RequiresReopen { get; }
}
