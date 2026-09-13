using System.ComponentModel;
using System.Globalization;
using CreationsForge.RecordEditing.Schema;

namespace CreationsForge.RecordEditing.Drafts;

/// <summary>Represents a record color graph with authoritative and redundant projections.</summary>
public sealed class RecordWireColorDraftNode : RecordWireObjectDraftNode
{
    /// <summary>Initializes one color draft.</summary>
    internal RecordWireColorDraftNode(string path, string displayName, RecordWireSchemaDescriptor descriptor, bool isRequired, bool isReadOnly, RecordWireDraftValueState valueState, IEnumerable<RecordWireObjectDraftProperty> properties)
        : base(RecordWireDraftNodeKind.Color, path, displayName, descriptor, isRequired, isReadOnly, valueState, properties)
    {
    }
}
