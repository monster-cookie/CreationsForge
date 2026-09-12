using System.ComponentModel;
using System.Globalization;
using CreationsForge.NativeEditing.Schema;

namespace CreationsForge.NativeEditing.Drafts;

/// <summary>Represents a native two-dimensional array while preserving dimensions and row boundaries.</summary>
public sealed class NativeWireArray2DDraftNode : NativeWireObjectDraftNode
{
    /// <summary>Initializes one two-dimensional-array draft.</summary>
    internal NativeWireArray2DDraftNode(string path, string displayName, NativeWireSchemaDescriptor descriptor, bool isRequired, bool isReadOnly, NativeWireDraftValueState valueState, IEnumerable<NativeWireObjectDraftProperty> properties)
        : base(NativeWireDraftNodeKind.Array2D, path, displayName, descriptor, isRequired, isReadOnly, valueState, properties)
    {
    }
}
