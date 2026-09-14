using CreationsForge.Core.Engine.Contracts;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;

namespace CreationsForge.Core.Engine.RecordReading;

/// <summary>
/// Associates one borrowed plugin getter with its explicit load-order and workspace provenance.
/// </summary>
public sealed class ReferenceSource
{
    /// <summary>The record semantic deep-copy operation used for resolved records from this source.</summary>
    private readonly Func<IMajorRecordGetter, IMajorRecordGetter> DeepCopyRecord;

    /// <summary>
    /// Initializes one borrowed reference source.
    /// </summary>
    /// <param name="mod">The plugin getter whose built-in record groups remain authoritative.</param>
    /// <param name="path">The canonical path from which the plugin was opened.</param>
    /// <param name="loadOrderIndex">The plugin's zero-based position in the explicit load order.</param>
    /// <param name="role">The plugin's role in the current workspace.</param>
    /// <param name="deepCopyRecord">An optional game-specific record deep-copy operation.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="mod"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="path"/> is empty or whitespace.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="loadOrderIndex"/> is negative or <paramref name="role"/> is undefined.</exception>
    public ReferenceSource(
        IModGetter mod,
        string path,
        int loadOrderIndex,
        PluginRole role,
        Func<IMajorRecordGetter, IMajorRecordGetter>? deepCopyRecord = null)
    {
        ArgumentNullException.ThrowIfNull(mod);
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentOutOfRangeException.ThrowIfNegative(loadOrderIndex);
        if (!Enum.IsDefined(role))
        {
            throw new ArgumentOutOfRangeException(nameof(role));
        }

        Mod = mod;
        Path = path;
        LoadOrderIndex = loadOrderIndex;
        Role = role;
        DeepCopyRecord = deepCopyRecord ?? DefaultDeepCopy;
    }

    /// <summary>Gets the borrowed plugin getter for shared reader operations without exposing it through the public context surface.</summary>
    internal IModGetter Mod { get; }

    /// <summary>Gets the record identity of the plugin containing its enumerated record contexts.</summary>
    public ModKey ModKey => Mod.ModKey;

    /// <summary>Gets the canonical path from which the plugin was opened.</summary>
    public string Path { get; }

    /// <summary>Gets the zero-based explicit load-order position.</summary>
    public int LoadOrderIndex { get; }

    /// <summary>Gets the plugin's workspace role.</summary>
    public PluginRole Role { get; }

    /// <summary>Creates a detached semantic record copy through the configured game-aware operation.</summary>
    /// <param name="record">The borrowed record to copy.</param>
    /// <returns>A detached record getter.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the configured operation returns the borrowed record or <see langword="null"/>.</exception>
    internal IMajorRecordGetter CreateDetachedCopy(IMajorRecordGetter record)
    {
        var copy = DeepCopyRecord(record);
        if (copy is null || ReferenceEquals(copy, record))
        {
            throw new InvalidOperationException("A reference deep-copy operation must return independent record state.");
        }

        return copy;
    }

    /// <summary>Creates the Mutagen record deep copy used when a game wrapper does not supply a more specific operation.</summary>
    /// <param name="record">The record to copy.</param>
    /// <returns>A detached record common-record copy.</returns>
    private static IMajorRecordGetter DefaultDeepCopy(IMajorRecordGetter record)
    {
        return record.DeepCopy();
    }
}
