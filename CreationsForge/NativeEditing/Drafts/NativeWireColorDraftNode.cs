using System.ComponentModel;
using System.Globalization;
using CreationsForge.NativeEditing.Schema;

namespace CreationsForge.NativeEditing.Drafts;

/// <summary>Represents a native color graph with authoritative and redundant projections.</summary>
public sealed class NativeWireColorDraftNode : NativeWireObjectDraftNode
{
    /// <summary>Initializes one color draft.</summary>
    internal NativeWireColorDraftNode(string path, string displayName, NativeWireSchemaDescriptor descriptor, bool isRequired, bool isReadOnly, NativeWireDraftValueState valueState, IEnumerable<NativeWireObjectDraftProperty> properties)
        : base(NativeWireDraftNodeKind.Color, path, displayName, descriptor, isRequired, isReadOnly, valueState, properties)
    {
    }
}
