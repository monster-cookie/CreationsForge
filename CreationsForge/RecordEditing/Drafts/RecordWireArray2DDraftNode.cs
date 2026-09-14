using System.ComponentModel;
using System.Globalization;
using CreationsForge.RecordEditing.Schema;

namespace CreationsForge.RecordEditing.Drafts;

/// <summary>Represents a record two-dimensional array while preserving dimensions and row boundaries.</summary>
public sealed class RecordWireArray2DDraftNode : RecordWireObjectDraftNode
{
    /// <summary>Initializes one two-dimensional-array draft.</summary>
    internal RecordWireArray2DDraftNode(string path, string displayName, RecordWireSchemaDescriptor descriptor, bool isRequired, bool isReadOnly, RecordWireDraftValueState valueState, IEnumerable<RecordWireObjectDraftProperty> properties)
        : base(RecordWireDraftNodeKind.Array2D, path, displayName, descriptor, isRequired, isReadOnly, valueState, properties)
    {
    }
}
