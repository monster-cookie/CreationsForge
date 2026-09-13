using System.Globalization;
using System.Text.Json;
using Mutagen.Bethesda.Plugins;

namespace CreationsForge.Core.Engine.RecordWire;

/// <content>Reads exact scalar and record identity leaves from one bounded traversal.</content>
public sealed partial class RecordWireReadContext
{
    /// <summary>Defines the closed single-precision wire object.</summary>
    private static readonly RecordWireObjectShape SingleShape = new(
        "single-precision value",
        new[] { "text", "bits" },
        new[] { "number" });

    /// <summary>Defines the closed double-precision wire object.</summary>
    private static readonly RecordWireObjectShape DoubleShape = new(
        "double-precision value",
        new[] { "text", "bits" },
        new[] { "number" });

    /// <summary>Defines the closed byte-sequence wire object.</summary>
    private static readonly RecordWireObjectShape BytesShape = new(
        "byte sequence",
        new[] { "length", "base64" });

    /// <summary>Reads one JSON Boolean value.</summary>
    /// <param name="value">The context-owned Boolean value.</param>
    /// <returns>The exact Boolean value.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is requested.</exception>
    public bool ReadBoolean(RecordWireValue value)
    {
        PrepareLeaf(value);
        if (value.Element.ValueKind is not JsonValueKind.True and not JsonValueKind.False)
        {
            Fail(value, "Expected a Boolean.");
        }

        return value.Element.GetBoolean();
    }

    /// <summary>Reads one bounded JSON string without normalization.</summary>
    /// <param name="value">The context-owned string value.</param>
    /// <returns>The exact decoded UTF-16 string.</returns>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is requested.</exception>
    public string ReadString(RecordWireValue value)
    {
        return ReadString(value, Limits.MaximumStringLength, "string");
    }

    /// <summary>Reads one signed eight-bit JSON integer.</summary>
    /// <param name="value">The context-owned numeric value.</param>
    /// <returns>The exact signed eight-bit value.</returns>
    public sbyte ReadSByte(RecordWireValue value)
    {
        var result = ReadInt32(value);
        if (result < sbyte.MinValue || result > sbyte.MaxValue)
        {
            Fail(value, "Expected an integer in the signed 8-bit range.");
        }

        return (sbyte)result;
    }

    /// <summary>Reads one unsigned eight-bit JSON integer.</summary>
    /// <param name="value">The context-owned numeric value.</param>
    /// <returns>The exact unsigned eight-bit value.</returns>
    public byte ReadByte(RecordWireValue value)
    {
        var result = ReadUInt32(value);
        if (result > byte.MaxValue)
        {
            Fail(value, "Expected an integer in the unsigned 8-bit range.");
        }

        return (byte)result;
    }

    /// <summary>Reads one signed 16-bit JSON integer.</summary>
    /// <param name="value">The context-owned numeric value.</param>
    /// <returns>The exact signed 16-bit value.</returns>
    public short ReadInt16(RecordWireValue value)
    {
        var result = ReadInt32(value);
        if (result < short.MinValue || result > short.MaxValue)
        {
            Fail(value, "Expected an integer in the signed 16-bit range.");
        }

        return (short)result;
    }

    /// <summary>Reads one unsigned 16-bit JSON integer.</summary>
    /// <param name="value">The context-owned numeric value.</param>
    /// <returns>The exact unsigned 16-bit value.</returns>
    public ushort ReadUInt16(RecordWireValue value)
    {
        var result = ReadUInt32(value);
        if (result > ushort.MaxValue)
        {
            Fail(value, "Expected an integer in the unsigned 16-bit range.");
        }

        return (ushort)result;
    }

    /// <summary>Reads one signed 32-bit JSON integer without floating-point conversion.</summary>
    /// <param name="value">The context-owned numeric value.</param>
    /// <returns>The exact signed 32-bit value.</returns>
    public int ReadInt32(RecordWireValue value)
    {
        PrepareLeaf(value);
        var result = 0;
        if (value.Element.ValueKind != JsonValueKind.Number ||
            !value.Element.TryGetInt32(out result))
        {
            Fail(value, "Expected an exact signed 32-bit JSON integer.");
        }

        return result;
    }

    /// <summary>Reads one unsigned 32-bit JSON integer without floating-point conversion.</summary>
    /// <param name="value">The context-owned numeric value.</param>
    /// <returns>The exact unsigned 32-bit value.</returns>
    public uint ReadUInt32(RecordWireValue value)
    {
        PrepareLeaf(value);
        uint result = 0;
        if (value.Element.ValueKind != JsonValueKind.Number ||
            !value.Element.TryGetUInt32(out result))
        {
            Fail(value, "Expected an exact unsigned 32-bit JSON integer.");
        }

        return result;
    }

    /// <summary>Reads one exact signed 64-bit integer from a JSON number or canonical invariant decimal string.</summary>
    /// <param name="value">The context-owned numeric or decimal-string value.</param>
    /// <returns>The exact signed 64-bit value.</returns>
    public long ReadInt64(RecordWireValue value)
    {
        PrepareLeaf(value);
        if (value.Element.ValueKind == JsonValueKind.Number &&
            value.Element.TryGetInt64(out var numericResult))
        {
            return numericResult;
        }

        if (value.Element.ValueKind == JsonValueKind.String)
        {
            var text = ReadString(value);
            if (long.TryParse(text, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var textResult) &&
                string.Equals(textResult.ToString(CultureInfo.InvariantCulture), text, StringComparison.Ordinal))
            {
                return textResult;
            }
        }

        Fail(value, "Expected an exact signed 64-bit JSON integer or canonical invariant decimal string.");
        return default;
    }

    /// <summary>Reads one exact unsigned 64-bit integer from a JSON number or canonical invariant decimal string.</summary>
    /// <param name="value">The context-owned numeric or decimal-string value.</param>
    /// <returns>The exact unsigned 64-bit value.</returns>
    public ulong ReadUInt64(RecordWireValue value)
    {
        PrepareLeaf(value);
        if (value.Element.ValueKind == JsonValueKind.Number &&
            value.Element.TryGetUInt64(out var numericResult))
        {
            return numericResult;
        }

        if (value.Element.ValueKind == JsonValueKind.String)
        {
            var text = ReadString(value);
            if (ulong.TryParse(text, NumberStyles.None, CultureInfo.InvariantCulture, out var textResult) &&
                string.Equals(textResult.ToString(CultureInfo.InvariantCulture), text, StringComparison.Ordinal))
            {
                return textResult;
            }
        }

        Fail(value, "Expected an exact unsigned 64-bit JSON integer or canonical invariant decimal string.");
        return default;
    }

    /// <summary>Reads one single-precision value from authoritative IEEE-754 bits and consistent redundant representations.</summary>
    /// <param name="value">The context-owned closed floating-point object.</param>
    /// <returns>The exact single-precision value, including signed zero and NaN payload bits.</returns>
    public float ReadSingle(RecordWireValue value)
    {
        var container = ReadObject(value, SingleShape);
        var textValue = container.GetRequiredProperty("text");
        var bitsValue = container.GetRequiredProperty("bits");
        var text = ReadString(textValue);
        var bitsText = ReadString(bitsValue);
        if (!TryReadCanonicalHex(bitsText, 8, out var bits))
        {
            Fail(bitsValue, "Expected canonical 0x-prefixed uppercase 32-bit hexadecimal bits.");
        }

        var result = BitConverter.UInt32BitsToSingle((uint)bits);
        var expectedText = result.ToString("R", CultureInfo.InvariantCulture);
        if (!string.Equals(text, expectedText, StringComparison.Ordinal))
        {
            Fail(textValue, "Floating-point text does not match the authoritative bits.");
        }

        var hasNumber = container.TryGetOptionalProperty("number", out var numberValue);
        if (!float.IsFinite(result))
        {
            if (hasNumber)
            {
                Fail(numberValue, "Non-finite floating-point values must omit the number property.");
            }

            return result;
        }

        if (!hasNumber)
        {
            Fail(value, "Finite floating-point values require the number property.");
        }

        PrepareLeaf(numberValue);
        if (numberValue.Element.ValueKind != JsonValueKind.Number ||
            !numberValue.Element.TryGetSingle(out var numericResult) ||
            BitConverter.SingleToUInt32Bits(numericResult) != (uint)bits)
        {
            Fail(numberValue, "Floating-point number does not reproduce the authoritative bits.");
        }

        return result;
    }

    /// <summary>Reads one double-precision value from authoritative IEEE-754 bits and consistent redundant representations.</summary>
    /// <param name="value">The context-owned closed floating-point object.</param>
    /// <returns>The exact double-precision value, including signed zero and NaN payload bits.</returns>
    public double ReadDouble(RecordWireValue value)
    {
        var container = ReadObject(value, DoubleShape);
        var textValue = container.GetRequiredProperty("text");
        var bitsValue = container.GetRequiredProperty("bits");
        var text = ReadString(textValue);
        var bitsText = ReadString(bitsValue);
        if (!TryReadCanonicalHex(bitsText, 16, out var bits))
        {
            Fail(bitsValue, "Expected canonical 0x-prefixed uppercase 64-bit hexadecimal bits.");
        }

        var result = BitConverter.UInt64BitsToDouble(bits);
        var expectedText = result.ToString("R", CultureInfo.InvariantCulture);
        if (!string.Equals(text, expectedText, StringComparison.Ordinal))
        {
            Fail(textValue, "Floating-point text does not match the authoritative bits.");
        }

        var hasNumber = container.TryGetOptionalProperty("number", out var numberValue);
        if (!double.IsFinite(result))
        {
            if (hasNumber)
            {
                Fail(numberValue, "Non-finite floating-point values must omit the number property.");
            }

            return result;
        }

        if (!hasNumber)
        {
            Fail(value, "Finite floating-point values require the number property.");
        }

        PrepareLeaf(numberValue);
        if (numberValue.Element.ValueKind != JsonValueKind.Number ||
            !numberValue.Element.TryGetDouble(out var numericResult) ||
            BitConverter.DoubleToUInt64Bits(numericResult) != bits)
        {
            Fail(numberValue, "Floating-point number does not reproduce the authoritative bits.");
        }

        return result;
    }

    /// <summary>Reads one canonical lowercase hyphenated Guid string.</summary>
    /// <param name="value">The context-owned Guid string.</param>
    /// <returns>The exact Guid value.</returns>
    public Guid ReadGuid(RecordWireValue value)
    {
        var text = ReadString(value);
        if (!Guid.TryParseExact(text, "D", out var result) ||
            !string.Equals(result.ToString("D"), text, StringComparison.Ordinal))
        {
            Fail(value, "Expected a canonical lowercase hyphenated Guid string.");
        }

        return result;
    }

    /// <summary>Reads one canonical Mutagen plugin identity string.</summary>
    /// <param name="value">The context-owned ModKey string.</param>
    /// <returns>The exact plugin identity.</returns>
    public ModKey ReadModKey(RecordWireValue value)
    {
        var text = ReadString(value);
        if (!ModKey.TryFromNameAndExtension(text, out var result, out _) ||
            !string.Equals(result.ToString(), text, StringComparison.Ordinal))
        {
            Fail(value, "Expected a canonical Mutagen ModKey string.");
        }

        return result;
    }

    /// <summary>Reads one canonical Mutagen record identity string.</summary>
    /// <param name="value">The context-owned FormKey string.</param>
    /// <returns>The exact record identity.</returns>
    public FormKey ReadFormKey(RecordWireValue value)
    {
        var text = ReadString(value);
        if (!FormKey.TryFactory(text.AsSpan(), out var result) ||
            !string.Equals(result.ToString(), text, StringComparison.Ordinal))
        {
            Fail(value, "Expected a canonical Mutagen FormKey string.");
        }

        return result;
    }

    /// <summary>Reads one exact bounded byte sequence from its length and canonical Base64 representation.</summary>
    /// <param name="value">The context-owned closed byte-sequence object.</param>
    /// <returns>A newly allocated exact byte sequence, or the shared empty array for zero length.</returns>
    public byte[] ReadBytes(RecordWireValue value)
    {
        var container = ReadObject(value, BytesShape);
        var lengthValue = container.GetRequiredProperty("length");
        var base64Value = container.GetRequiredProperty("base64");
        var length = ReadInt32(lengthValue);
        if (length < 0 || length > Limits.MaximumDecodedByteLength)
        {
            Fail(lengthValue, $"Decoded byte length must be between 0 and {Limits.MaximumDecodedByteLength}.");
        }

        var maximumEncodedLength = checked((int)Math.Min(
            int.MaxValue,
            ((long)Limits.MaximumDecodedByteLength + 2L) / 3L * 4L));
        var base64 = ReadString(base64Value, maximumEncodedLength, "Base64 string");
        var expectedEncodedLength = ((long)length + 2L) / 3L * 4L;
        if (base64.Length != expectedEncodedLength)
        {
            Fail(base64Value, "Base64 length does not match the declared decoded byte length.");
        }

        if (length == 0)
        {
            if (base64.Length != 0)
            {
                Fail(base64Value, "An empty byte sequence requires an empty Base64 string.");
            }

            return Array.Empty<byte>();
        }

        var bytes = new byte[length];
        if (!Convert.TryFromBase64String(base64, bytes, out var bytesWritten) ||
            bytesWritten != length ||
            !string.Equals(Convert.ToBase64String(bytes), base64, StringComparison.Ordinal))
        {
            Fail(base64Value, "Expected canonical Base64 matching the declared decoded byte length.");
        }

        return bytes;
    }

    /// <summary>Checks cancellation and ownership before reading one leaf.</summary>
    /// <param name="value">The context-owned leaf value.</param>
    /// <exception cref="OperationCanceledException">Thrown when cancellation is requested.</exception>
    private void PrepareLeaf(RecordWireValue value)
    {
        EnsureOwned(value);
        CancellationToken.ThrowIfCancellationRequested();
    }

    /// <summary>Reads one string under a leaf-specific length limit.</summary>
    /// <param name="value">The context-owned string value.</param>
    /// <param name="maximumLength">The maximum accepted decoded UTF-16 length.</param>
    /// <param name="description">The value description used in failures.</param>
    /// <returns>The exact decoded string.</returns>
    private string ReadString(RecordWireValue value, int maximumLength, string description)
    {
        PrepareLeaf(value);
        if (value.Element.ValueKind != JsonValueKind.String)
        {
            Fail(value, $"Expected a {description}.");
        }

        string result;
        try
        {
            result = value.Element.GetString()!;
        }
        catch (InvalidOperationException exception) when (exception is not ObjectDisposedException)
        {
            Fail(value, $"Expected a {description} containing complete UTF-16 data.");
            return string.Empty;
        }

        if (result.Length > maximumLength)
        {
            Fail(value, $"{description} length {result.Length} exceeds the limit of {maximumLength}.");
        }

        return result;
    }

    /// <summary>Parses one fixed-width canonical uppercase hexadecimal string.</summary>
    /// <param name="text">The exact wire text.</param>
    /// <param name="digitCount">The required number of hexadecimal digits after <c>0x</c>.</param>
    /// <param name="value">Receives the parsed unsigned bits.</param>
    /// <returns><see langword="true"/> only when the text exactly matches the canonical representation.</returns>
    private static bool TryReadCanonicalHex(string text, int digitCount, out ulong value)
    {
        value = 0;
        if (text.Length != digitCount + 2 ||
            text[0] != '0' ||
            text[1] != 'x' ||
            !ulong.TryParse(
                text.AsSpan(2),
                NumberStyles.AllowHexSpecifier,
                CultureInfo.InvariantCulture,
                out value))
        {
            return false;
        }

        return string.Equals(
            text,
            $"0x{value.ToString($"X{digitCount}", CultureInfo.InvariantCulture)}",
            StringComparison.Ordinal);
    }
}
