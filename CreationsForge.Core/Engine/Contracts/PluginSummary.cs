using Mutagen.Bethesda.Plugins;

namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Describes one plugin participating in the current ephemeral workspace.
/// </summary>
public sealed class PluginSummary
{
    /// <summary>Initializes a plugin summary.</summary>
    /// <param name="modKey">The plugin identity.</param>
    /// <param name="path">The canonical plugin path.</param>
    /// <param name="loadOrderIndex">The zero-based position in the explicit load order.</param>
    /// <param name="role">The plugin's role in the workspace.</param>
    /// <param name="recordCount">The physical major-record count, including overrides, or <see langword="null"/> when enumeration was unavailable.</param>
    /// <param name="uniqueRecordContributionCount">The number of FormKeys first encountered at this load-order position, or <see langword="null"/> when the complete union could not be calculated.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="path"/> is empty or whitespace.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="loadOrderIndex"/> is negative.</exception>
    public PluginSummary(
        ModKey modKey,
        string path,
        int loadOrderIndex,
        PluginRole role,
        long? recordCount = null,
        long? uniqueRecordContributionCount = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentOutOfRangeException.ThrowIfNegative(loadOrderIndex);
        if (recordCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(recordCount));
        }

        if (uniqueRecordContributionCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(uniqueRecordContributionCount));
        }
        ModKey = modKey;
        Path = path;
        LoadOrderIndex = loadOrderIndex;
        Role = role;
        RecordCount = recordCount;
        UniqueRecordContributionCount = uniqueRecordContributionCount;
    }

    /// <summary>Gets the plugin identity.</summary>
    public ModKey ModKey { get; }

    /// <summary>Gets the canonical plugin path.</summary>
    public string Path { get; }

    /// <summary>Gets the zero-based position in the explicit load order.</summary>
    public int LoadOrderIndex { get; }

    /// <summary>Gets the plugin's role in the workspace.</summary>
    public PluginRole Role { get; }

    /// <summary>Gets the physical major-record count including overrides, or <see langword="null"/> when enumeration was unavailable.</summary>
    public long? RecordCount { get; }

    /// <summary>Gets the number of FormKeys first encountered in this plugin across admitted load order, or <see langword="null"/> when the union was unavailable.</summary>
    public long? UniqueRecordContributionCount { get; }
}
