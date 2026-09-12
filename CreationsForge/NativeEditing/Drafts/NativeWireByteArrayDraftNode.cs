using System.ComponentModel;
using System.Globalization;
using CreationsForge.NativeEditing.Schema;

namespace CreationsForge.NativeEditing.Drafts;

/// <summary>Represents a Base64-encoded byte sequence while preserving exact bytes.</summary>
public sealed class NativeWireByteArrayDraftNode : NativeWireObjectDraftNode
{
    /// <summary>Initializes one Base64 byte-sequence draft.</summary>
    internal NativeWireByteArrayDraftNode(string path, string displayName, NativeWireSchemaDescriptor descriptor, bool isRequired, bool isReadOnly, NativeWireDraftValueState valueState, IEnumerable<NativeWireObjectDraftProperty> properties)
        : base(NativeWireDraftNodeKind.ByteArray, path, displayName, descriptor, isRequired, isReadOnly, valueState, properties)
    {
    }

    /// <summary>Gets or sets the exact Base64 text through the typed <c>base64</c> child.</summary>
    public string Value
    {
        get => (FindProperty("base64") as NativeWireStringDraftNode)?.Value ?? string.Empty;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            if (FindProperty("base64") is NativeWireStringDraftNode text)
            {
                text.Value = value;
            }
        }
    }

    /// <summary>Gets the declared decoded-length node.</summary>
    public NativeWireIntegerDraftNode? Length => FindProperty("length") as NativeWireIntegerDraftNode;
}
