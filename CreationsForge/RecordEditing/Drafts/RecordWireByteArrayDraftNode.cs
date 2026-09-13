using System.ComponentModel;
using System.Globalization;
using CreationsForge.RecordEditing.Schema;

namespace CreationsForge.RecordEditing.Drafts;

/// <summary>Represents a Base64-encoded byte sequence while preserving exact bytes.</summary>
public sealed class RecordWireByteArrayDraftNode : RecordWireObjectDraftNode
{
    /// <summary>Initializes one Base64 byte-sequence draft.</summary>
    internal RecordWireByteArrayDraftNode(string path, string displayName, RecordWireSchemaDescriptor descriptor, bool isRequired, bool isReadOnly, RecordWireDraftValueState valueState, IEnumerable<RecordWireObjectDraftProperty> properties)
        : base(RecordWireDraftNodeKind.ByteArray, path, displayName, descriptor, isRequired, isReadOnly, valueState, properties)
    {
    }

    /// <summary>Gets or sets the exact Base64 text through the typed <c>base64</c> child.</summary>
    public string Value
    {
        get => (FindProperty("base64") as RecordWireStringDraftNode)?.Value ?? string.Empty;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            if (FindProperty("base64") is RecordWireStringDraftNode text)
            {
                text.Value = value;
            }
        }
    }

    /// <summary>Gets the declared decoded-length node.</summary>
    public RecordWireIntegerDraftNode? Length => FindProperty("length") as RecordWireIntegerDraftNode;
}
