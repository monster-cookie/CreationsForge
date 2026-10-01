using CreationsForge.Engine.Workspaces;
using Mutagen.Bethesda;

namespace CreationsForge.Mcp.Sessions;

/// <summary>Owns the one process-lifetime authoring session and its optional native workspace.</summary>
internal sealed class McpAuthoringSession
{
    /// <summary>Initializes a pending session that has not opened a native workspace.</summary>
    /// <param name="id">The session identifier returned to the client.</param>
    /// <param name="release">The selected game release.</param>
    /// <param name="dataDirectory">The selected data directory.</param>
    /// <param name="selectedPlugins">Selected plugin file names in low-to-high priority order.</param>
    public McpAuthoringSession(
        string id,
        GameRelease release,
        string dataDirectory,
        IReadOnlyList<string> selectedPlugins)
    {
        Id = id;
        Release = release;
        DataDirectory = dataDirectory;
        SelectedPlugins = selectedPlugins;
        Phase = McpSessionPhase.Pending;
    }

    /// <summary>Gets the session identifier.</summary>
    public string Id { get; }

    /// <summary>Gets the selected game release.</summary>
    public GameRelease Release { get; }

    /// <summary>Gets the selected data directory.</summary>
    public string DataDirectory { get; }

    /// <summary>Gets the selected plugin file names.</summary>
    public IReadOnlyList<string> SelectedPlugins { get; }

    /// <summary>Gets or sets the session phase. Mutations are guarded by the session registry lock.</summary>
    public McpSessionPhase Phase { get; set; }

    /// <summary>Gets or sets the native workspace after output create or open succeeds.</summary>
    public PluginWorkspace? Workspace { get; set; }
}

/// <summary>Identifies the lifecycle of the one authoring session.</summary>
internal enum McpSessionPhase
{
    /// <summary>Game and source configuration is stored, but no native workspace is open.</summary>
    Pending,

    /// <summary>A native workspace open is in progress.</summary>
    Opening,

    /// <summary>A native workspace is attached.</summary>
    Open,

    /// <summary>Close, discard, or dispose is in progress.</summary>
    Closing,
}
