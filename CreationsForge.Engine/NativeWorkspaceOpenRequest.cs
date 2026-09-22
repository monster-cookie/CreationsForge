using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;

namespace CreationsForge.Engine;

/// <summary>Supplies the complete native inputs needed to open one workspace.</summary>
public sealed class NativeWorkspaceOpenRequest
{
    private readonly IReadOnlyList<ModKey> _selectedPlugins;

    /// <summary>Initializes a workspace-open request.</summary>
    /// <param name="release">The exact game release to use.</param>
    /// <param name="dataDirectory">The game data directory containing selected plugins and their masters.</param>
    /// <param name="selectedPlugins">Plugins selected for browsing, in low-to-high priority order.</param>
    /// <param name="output">The mutable output definition.</param>
    public NativeWorkspaceOpenRequest(
        GameRelease release,
        string dataDirectory,
        IEnumerable<ModKey> selectedPlugins,
        NativeOutputDefinition output)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(dataDirectory);
        ArgumentNullException.ThrowIfNull(selectedPlugins);
        ArgumentNullException.ThrowIfNull(output);

        Release = release;
        DataDirectory = dataDirectory;
        _selectedPlugins = selectedPlugins.ToArray();
        Output = output;
    }

    /// <summary>Gets the exact game release.</summary>
    public GameRelease Release { get; }

    /// <summary>Gets the game data directory.</summary>
    public string DataDirectory { get; }

    /// <summary>Gets the explicitly selected plugins in low-to-high priority order.</summary>
    public IReadOnlyList<ModKey> SelectedPlugins => _selectedPlugins;

    /// <summary>Gets the mutable output definition.</summary>
    public NativeOutputDefinition Output { get; }
}
