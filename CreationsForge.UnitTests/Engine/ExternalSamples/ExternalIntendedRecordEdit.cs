using Mutagen.Bethesda.Plugins;

namespace CreationsForge.UnitTests.Engine.ExternalSamples;

/// <summary>Holds independently derived exact values for one external FormList override.</summary>
internal sealed class ExternalIntendedRecordEdit
{
    /// <summary>Initializes one immutable intended edit from source state and caller-owned expected values.</summary>
    /// <param name="source">The complete detached source record.</param>
    /// <param name="editorId">The exact EditorID that the edit must produce.</param>
    /// <param name="items">The exact ordered Items sequence that the edit must produce.</param>
    /// <param name="replaceItems">Whether the runner will issue a ReplaceItems edit.</param>
    /// <exception cref="ArgumentNullException">Thrown when a required reference is null.</exception>
    internal ExternalIntendedRecordEdit(
        ExternalSelectedRecord source,
        string editorId,
        IReadOnlyList<FormKey> items,
        bool replaceItems)
    {
        Source = source ?? throw new ArgumentNullException(nameof(source));
        EditorId = editorId ?? throw new ArgumentNullException(nameof(editorId));
        Items = Array.AsReadOnly(items?.ToArray() ?? throw new ArgumentNullException(nameof(items)));
        ReplaceItems = replaceItems;
    }

    /// <summary>Gets the complete detached source record.</summary>
    public ExternalSelectedRecord Source { get; }

    /// <summary>Gets the exact intended EditorID.</summary>
    public string EditorId { get; }

    /// <summary>Gets the exact intended ordered Items sequence, including duplicates and null sentinels.</summary>
    public IReadOnlyList<FormKey> Items { get; }

    /// <summary>Gets whether the runner must issue a material ReplaceItems edit.</summary>
    public bool ReplaceItems { get; }
}
