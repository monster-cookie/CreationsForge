using System.ComponentModel;
using System.Globalization;
using CreationsForge.RecordEditing.Schema;

namespace CreationsForge.RecordEditing.Drafts;

/// <summary>Represents one editable Boolean draft value.</summary>
public sealed class RecordWireBooleanDraftNode : RecordWireDraftNode
{
    /// <summary>The current Boolean value.</summary>
    private bool CurrentValue;

    /// <summary>Initializes one Boolean draft.</summary>
    internal RecordWireBooleanDraftNode(string path, string displayName, RecordWireSchemaDescriptor descriptor, bool isRequired, bool isReadOnly, RecordWireDraftValueState valueState, bool value)
        : base(RecordWireDraftNodeKind.Boolean, path, displayName, descriptor, isRequired, isReadOnly, valueState)
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
    public override IReadOnlyList<RecordWireDraftNode> Children => Array.Empty<RecordWireDraftNode>();
}
