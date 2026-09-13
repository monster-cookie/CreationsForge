using System.ComponentModel;
using System.Globalization;
using CreationsForge.RecordEditing.Schema;

namespace CreationsForge.RecordEditing.Drafts;

/// <summary>Represents one bounded record string draft value.</summary>
public class RecordWireStringDraftNode : RecordWireDraftNode
{
    /// <summary>The current exact string.</summary>
    private string CurrentValue;

    /// <summary>Initializes one string draft.</summary>
    internal RecordWireStringDraftNode(RecordWireDraftNodeKind kind, string path, string displayName, RecordWireSchemaDescriptor descriptor, bool isRequired, bool isReadOnly, RecordWireDraftValueState valueState, string value)
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
    public override IReadOnlyList<RecordWireDraftNode> Children => Array.Empty<RecordWireDraftNode>();
}
