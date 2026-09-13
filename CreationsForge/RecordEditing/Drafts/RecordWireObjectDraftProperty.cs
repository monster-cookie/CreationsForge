using System.Collections.ObjectModel;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.RecordWire;
using CreationsForge.RecordEditing.Schema;

namespace CreationsForge.RecordEditing.Drafts;

/// <summary>Associates one exact object property name with its typed draft node.</summary>
public sealed class RecordWireObjectDraftProperty
{
    /// <summary>Initializes one immutable object-property binding.</summary>
    /// <param name="name">The exact case-sensitive JSON property name.</param>
    /// <param name="node">The typed property value.</param>
    public RecordWireObjectDraftProperty(string name, RecordWireDraftNode node)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(node);
        Name = name;
        Node = node;
    }

    /// <summary>Gets the exact case-sensitive JSON property name.</summary>
    public string Name { get; }

    /// <summary>Gets the typed property value.</summary>
    public RecordWireDraftNode Node { get; }
}
