namespace CreationsForge.Core.Engine.Contracts;

/// <summary>
/// Identifies the semantic shape of a FormList change.
/// </summary>
public enum SemanticChangeKind
{
    /// <summary>A scalar value changed.</summary>
    ValueChanged,

    /// <summary>An ordered collection item was inserted.</summary>
    ItemInserted,

    /// <summary>An ordered collection item was removed.</summary>
    ItemRemoved,

    /// <summary>An ordered collection item changed in place.</summary>
    ItemChanged,

    /// <summary>The plugin writer normalized a value without changing its intended meaning.</summary>
    WriterNormalized
}
