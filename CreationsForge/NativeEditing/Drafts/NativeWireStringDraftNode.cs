using System.ComponentModel;
using System.Globalization;
using CreationsForge.NativeEditing.Schema;

namespace CreationsForge.NativeEditing.Drafts;

/// <summary>Represents one bounded native string draft value.</summary>
public class NativeWireStringDraftNode : NativeWireDraftNode
{
    /// <summary>The current exact string.</summary>
    private string CurrentValue;

    /// <summary>Initializes one string draft.</summary>
    internal NativeWireStringDraftNode(NativeWireDraftNodeKind kind, string path, string displayName, NativeWireSchemaDescriptor descriptor, bool isRequired, bool isReadOnly, NativeWireDraftValueState valueState, string value)
        : base(kind, path, displayName, descriptor, isRequired, isReadOnly, valueState)
    {
        ArgumentNullException.ThrowIfNull(value);
        CurrentValue = value;
    }

    /// <summary>Gets or sets the exact string; validation applies schema length, format, and pattern constraints.</summary>
    public string Value
    {
        get => CurrentValue;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            if (IsReadOnly || string.Equals(CurrentValue, value, StringComparison.Ordinal))
            {
                return;
            }

            CurrentValue = value;
            MarkChanged();
        }
    }

    /// <inheritdoc />
    public override IReadOnlyList<NativeWireDraftNode> Children => Array.Empty<NativeWireDraftNode>();
}
