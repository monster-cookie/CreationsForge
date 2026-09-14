using System.ComponentModel;
using System.Globalization;
using CreationsForge.RecordEditing.Schema;

namespace CreationsForge.RecordEditing.Drafts;

/// <summary>Represents one canonical signed or unsigned integer draft value without floating-point conversion.</summary>
public class RecordWireIntegerDraftNode : RecordWireDraftNode
{
    /// <summary>The current canonical decimal text.</summary>
    private string CurrentText;

    /// <summary>Initializes one integer draft.</summary>
    internal RecordWireIntegerDraftNode(RecordWireDraftNodeKind kind, string path, string displayName, RecordWireSchemaDescriptor descriptor, bool isRequired, bool isReadOnly, RecordWireDraftValueState valueState, string text)
        : base(kind, path, displayName, descriptor, isRequired, isReadOnly, valueState)
    {
        ArgumentNullException.ThrowIfNull(text);
        CurrentText = text;
    }

    /// <summary>Gets or sets the exact decimal text; validation determines signedness, range, and canonical form.</summary>
    public string Text
    {
        get => CurrentText;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            if (IsReadOnly || string.Equals(CurrentText, value, StringComparison.Ordinal))
            {
                return;
            }

            CurrentText = value;
            MarkChanged();
        }
    }

    /// <inheritdoc />
    public override IReadOnlyList<RecordWireDraftNode> Children => Array.Empty<RecordWireDraftNode>();
}
