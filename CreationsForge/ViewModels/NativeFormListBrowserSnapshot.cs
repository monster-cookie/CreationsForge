using CreationsForge.Core.Engine.Contracts;

namespace CreationsForge.ViewModels;

/// <summary>
/// Carries one revision-consistent detached browser snapshot across the coordinator borrowing boundary.
/// </summary>
internal sealed class NativeFormListBrowserSnapshot
{
    /// <summary>Initializes one immutable browser snapshot.</summary>
    /// <param name="state">The exact workspace state and revision.</param>
    /// <param name="plugins">The participating plugins in engine order.</param>
    /// <param name="formLists">All FormList contexts in engine order.</param>
    /// <exception cref="ArgumentNullException">Thrown when any argument is <see langword="null"/>.</exception>
    public NativeFormListBrowserSnapshot(
        WorkspaceState state,
        IReadOnlyList<PluginSummary> plugins,
        IReadOnlyList<FormListSummary> formLists)
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(plugins);
        ArgumentNullException.ThrowIfNull(formLists);
        State = state;
        Plugins = Array.AsReadOnly(plugins.ToArray());
        FormLists = Array.AsReadOnly(formLists.ToArray());
    }

    /// <summary>Gets the exact workspace state and revision.</summary>
    public WorkspaceState State { get; }

    /// <summary>Gets the participating plugins in engine order.</summary>
    public IReadOnlyList<PluginSummary> Plugins { get; }

    /// <summary>Gets all FormList contexts in engine order.</summary>
    public IReadOnlyList<FormListSummary> FormLists { get; }
}
