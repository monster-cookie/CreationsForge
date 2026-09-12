using System.ComponentModel;
using System.Globalization;
using CreationsForge.NativeEditing.Schema;

namespace CreationsForge.NativeEditing.Drafts;

/// <summary>Represents one editable Boolean draft value.</summary>
public sealed class NativeWireBooleanDraftNode : NativeWireDraftNode
{
    /// <summary>The current Boolean value.</summary>
    private bool CurrentValue;

    /// <summary>Initializes one Boolean draft.</summary>
    internal NativeWireBooleanDraftNode(string path, string displayName, NativeWireSchemaDescriptor descriptor, bool isRequired, bool isReadOnly, NativeWireDraftValueState valueState, bool value)
        : base(NativeWireDraftNodeKind.Boolean, path, displayName, descriptor, isRequired, isReadOnly, valueState)
    {
        CurrentValue = value;
    }

    /// <summary>Gets or sets the current Boolean value.</summary>
    public bool Value
    {
        get => CurrentValue;
        set
        {
            if (CurrentValue == value || IsReadOnly)
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
