using System.ComponentModel;
using System.Globalization;
using CreationsForge.NativeEditing.Schema;

namespace CreationsForge.NativeEditing.Drafts;

/// <summary>Represents a native enum as exact integer text with optional known symbolic choices.</summary>
public sealed class NativeWireEnumDraftNode : NativeWireIntegerDraftNode
{
    /// <summary>Initializes one known-or-unknown enum draft.</summary>
    internal NativeWireEnumDraftNode(string path, string displayName, NativeWireSchemaDescriptor descriptor, bool isRequired, bool isReadOnly, NativeWireDraftValueState valueState, string text)
        : base(NativeWireDraftNodeKind.Enum, path, displayName, descriptor, isRequired, isReadOnly, valueState, text)
    {
    }

    /// <summary>Gets the catalog-provided symbolic choices; an absent match does not invalidate unknown numeric input.</summary>
    public IReadOnlyList<NativeWireSchemaEnumValue> KnownValues => Descriptor.EnumValues;
}
