using System.ComponentModel;
using System.Globalization;
using CreationsForge.NativeEditing.Schema;

namespace CreationsForge.NativeEditing.Drafts;

/// <summary>Represents one canonical signed or unsigned integer draft value without floating-point conversion.</summary>
public class NativeWireIntegerDraftNode : NativeWireDraftNode
{
    /// <summary>The current canonical decimal text.</summary>
    private string CurrentText;

    /// <summary>Initializes one integer draft.</summary>
    internal NativeWireIntegerDraftNode(NativeWireDraftNodeKind kind, string path, string displayName, NativeWireSchemaDescriptor descriptor, bool isRequired, bool isReadOnly, NativeWireDraftValueState valueState, string text)
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
    public override IReadOnlyList<NativeWireDraftNode> Children => Array.Empty<NativeWireDraftNode>();
}
