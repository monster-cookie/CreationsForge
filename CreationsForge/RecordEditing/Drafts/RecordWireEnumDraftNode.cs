using System.ComponentModel;
using System.Globalization;
using CreationsForge.RecordEditing.Schema;

namespace CreationsForge.RecordEditing.Drafts;

/// <summary>Represents a record enum as exact integer text with optional known symbolic choices.</summary>
public sealed class RecordWireEnumDraftNode : RecordWireIntegerDraftNode
{
    /// <summary>Initializes one known-or-unknown enum draft.</summary>
    internal RecordWireEnumDraftNode(string path, string displayName, RecordWireSchemaDescriptor descriptor, bool isRequired, bool isReadOnly, RecordWireDraftValueState valueState, string text)
        : base(RecordWireDraftNodeKind.Enum, path, displayName, descriptor, isRequired, isReadOnly, valueState, text)
    {
    }

    /// <summary>Gets the catalog-provided symbolic choices; an absent match does not invalidate unknown numeric input.</summary>
    public IReadOnlyList<RecordWireSchemaEnumValue> KnownValues => Descriptor.EnumValues;
}
