using System.ComponentModel;
using System.Globalization;
using CreationsForge.RecordEditing.Schema;

namespace CreationsForge.RecordEditing.Drafts;

/// <summary>Represents exact floating-point bits together with optional redundant authoring projections.</summary>
public sealed class RecordWireFloatBitsDraftNode : RecordWireObjectDraftNode
{
    /// <summary>Prevents recursive updates while redundant projections are synchronized from bits.</summary>
    private bool IsSynchronizing;

    /// <summary>Initializes one exact float-bit object draft.</summary>
    internal RecordWireFloatBitsDraftNode(string path, string displayName, RecordWireSchemaDescriptor descriptor, bool isRequired, bool isReadOnly, RecordWireDraftValueState valueState, IEnumerable<RecordWireObjectDraftProperty> properties)
        : base(RecordWireDraftNodeKind.FloatBits, path, displayName, descriptor, isRequired, isReadOnly, valueState, properties)
    {
        if (FindProperty("bits") is RecordWireStringDraftNode bits)
        {
            bits.PropertyChanged += BitsChanged;
        }
    }

    /// <summary>Gets the authoritative exact <c>bits</c> property node when present.</summary>
    public RecordWireDraftNode? Bits => FindProperty("bits");

    /// <summary>Synchronizes invariant text and the optional finite JSON number when authoritative bits change.</summary>
    private void BitsChanged(object? sender, PropertyChangedEventArgs eventArgs)
    {
        if (IsSynchronizing || eventArgs.PropertyName != nameof(RecordWireStringDraftNode.Value) || sender is not RecordWireStringDraftNode bits || FindProperty("text") is not RecordWireStringDraftNode text)
        {
            return;
        }

        string? projection = null;
        var finite = false;
        if (bits.Value.StartsWith("0x", StringComparison.Ordinal) && bits.Value.Length == 10 && uint.TryParse(bits.Value.AsSpan(2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out var singleBits))
        {
            var value = BitConverter.UInt32BitsToSingle(singleBits);
            projection = value.ToString("R", CultureInfo.InvariantCulture);
            finite = float.IsFinite(value);
        }
        else if (bits.Value.StartsWith("0x", StringComparison.Ordinal) && bits.Value.Length == 18 && ulong.TryParse(bits.Value.AsSpan(2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out var doubleBits))
        {
            var value = BitConverter.UInt64BitsToDouble(doubleBits);
            projection = value.ToString("R", CultureInfo.InvariantCulture);
            finite = double.IsFinite(value);
        }

        if (projection is null)
        {
            return;
        }

        IsSynchronizing = true;
        try
        {
            text.Value = projection;
            if (FindProperty("number") is RecordWireStringDraftNode number)
            {
                if (finite)
                {
                    number.Value = projection;
                    number.ValueState = RecordWireDraftValueState.Defaulted;
                }
                else
                {
                    number.ValueState = RecordWireDraftValueState.RequiredUnset;
                }
            }
        }
        finally
        {
            IsSynchronizing = false;
        }
    }
}
