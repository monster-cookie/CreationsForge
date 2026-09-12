using System.ComponentModel;
using System.Globalization;
using CreationsForge.NativeEditing.Schema;

namespace CreationsForge.NativeEditing.Drafts;

/// <summary>Represents a translated string graph with ordered translation entries.</summary>
public sealed class NativeWireTranslatedStringDraftNode : NativeWireObjectDraftNode
{
    /// <summary>Initializes one translated-string draft.</summary>
    internal NativeWireTranslatedStringDraftNode(string path, string displayName, NativeWireSchemaDescriptor descriptor, bool isRequired, bool isReadOnly, NativeWireDraftValueState valueState, IEnumerable<NativeWireObjectDraftProperty> properties)
        : base(NativeWireDraftNodeKind.TranslatedString, path, displayName, descriptor, isRequired, isReadOnly, valueState, properties)
    {
    }
}
