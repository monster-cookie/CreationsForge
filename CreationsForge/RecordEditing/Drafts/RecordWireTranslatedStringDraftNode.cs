using System.ComponentModel;
using System.Globalization;
using CreationsForge.RecordEditing.Schema;

namespace CreationsForge.RecordEditing.Drafts;

/// <summary>Represents a translated string graph with ordered translation entries.</summary>
public sealed class RecordWireTranslatedStringDraftNode : RecordWireObjectDraftNode
{
    /// <summary>Initializes one translated-string draft.</summary>
    internal RecordWireTranslatedStringDraftNode(string path, string displayName, RecordWireSchemaDescriptor descriptor, bool isRequired, bool isReadOnly, RecordWireDraftValueState valueState, IEnumerable<RecordWireObjectDraftProperty> properties)
        : base(RecordWireDraftNodeKind.TranslatedString, path, displayName, descriptor, isRequired, isReadOnly, valueState, properties)
    {
    }
}
