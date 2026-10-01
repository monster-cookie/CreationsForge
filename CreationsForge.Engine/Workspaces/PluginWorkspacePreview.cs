using CreationsForge.Engine.Records;

namespace CreationsForge.Engine.Workspaces;

/// <summary>Projects unsaved registered changes without exposing mutable Mutagen objects.</summary>
public sealed class PluginWorkspacePreview
{
    /// <summary>Initializes an immutable preview.</summary>
    /// <param name="state">The workspace state captured with the pending changes.</param>
    /// <param name="pendingRecords">Registered snapshots for records changed since the saved baseline.</param>
    public PluginWorkspacePreview(PluginWorkspaceState state, IReadOnlyList<RecordSnapshot> pendingRecords)
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(pendingRecords);
        State = state;
        PendingRecords = pendingRecords;
    }

    /// <summary>Gets the workspace state captured with the pending changes.</summary>
    public PluginWorkspaceState State { get; }

    /// <summary>Gets registered snapshots for records changed since the saved baseline.</summary>
    public IReadOnlyList<RecordSnapshot> PendingRecords { get; }
}
