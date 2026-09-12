using System.Text;
using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.NativeWire;
using CreationsForge.Core.Enums;
using Mutagen.Bethesda;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.NativeWire;

/// <summary>Verifies bounded strict native wire traversal and exact scalar reconstruction.</summary>
public sealed class NativeWireReadContextTests
{
    /// <summary>Defines the closed object shape used by structural tests.</summary>
    private static readonly NativeWireObjectShape ValueShape = new(
        "test value",
        new[] { "value" });

    /// <summary>Verifies missing, unknown, and duplicate properties fail at the decode boundary without returning a partial value.</summary>
    [Theory]
    [InlineData("{}")]
    [InlineData("{\"value\":1,\"extra\":2}")]
    [InlineData("{\"value\":1,\"value\":2}")]
    public void Decode_WithNonClosedObject_ReturnsInvalidRequest(string json)
    {
        using var document = JsonDocument.Parse(json);

        var result = NativeWireReadContext.Decode(
            document.RootElement,
            NativeWireReadLimits.Default,
            CancellationToken.None,
            static (context, root) =>
            {
                var value = context.ReadObject(root, ValueShape);
                return new DecodedValue(context.ReadInt32(value.GetRequiredProperty("value")));
            });

        result.Succeeded.ShouldBeFalse();
        result.Value.ShouldBeNull();
        result.Error!.Code.ShouldBe(EngineErrorCode.InvalidRequest);
        result.Error.Message.ShouldStartWith("$:");
    }

    /// <summary>Verifies depth, total-node, and per-array limits reject input before a generated decoder allocates a target collection.</summary>
    [Fact]
    public void Decode_WithExceededStructuralLimit_ReturnsInvalidRequest()
    {
        using var depthDocument = JsonDocument.Parse("{\"value\":1}");
        using var arrayDocument = JsonDocument.Parse("[1,2,3]");
        var depthLimits = new NativeWireReadLimits(1, 10, 10, 10, 10);
        var arrayLimits = new NativeWireReadLimits(10, 10, 2, 10, 10);

        var depthResult = NativeWireReadContext.Decode(
            depthDocument.RootElement,
            depthLimits,
            CancellationToken.None,
            static (context, root) =>
            {
                var value = context.ReadObject(root, ValueShape);
                return new DecodedValue(context.ReadInt32(value.GetRequiredProperty("value")));
            });
        var arrayResult = NativeWireReadContext.Decode(
            arrayDocument.RootElement,
            arrayLimits,
            CancellationToken.None,
            static (context, root) => new DecodedValue(context.ReadArray(root).Count));

        depthResult.Error!.Message.ShouldContain("Nesting depth");
        arrayResult.Error!.Message.ShouldContain("per-array limit");
    }

    /// <summary>Verifies ordinary, zero, and maximum rectangular-array dimensions are accepted without exposing the configured limit.</summary>
    [Theory]
    [InlineData(0)]
    [InlineData(17)]
    [InlineData(NativeWireReadLimits.DefaultMaximumArrayElements)]
    public void ValidateArrayLength_WithBoundedDimension_AcceptsLength(int length)
    {
        using var document = JsonDocument.Parse(length.ToString(System.Globalization.CultureInfo.InvariantCulture));

        var result = NativeWireReadContext.Decode(
            document.RootElement,
            NativeWireReadLimits.Default,
            CancellationToken.None,
            (context, root) =>
            {
                var decodedLength = context.ReadInt32(root);
                context.ValidateArrayLength(root, decodedLength);
                return new DecodedValue(decodedLength);
            });

        result.Succeeded.ShouldBeTrue();
        result.Value!.Value.ShouldBe(length);
    }

    /// <summary>Verifies negative and over-limit rectangular dimensions fail before native array allocation.</summary>
    [Theory]
    [InlineData(-1)]
    [InlineData(NativeWireReadLimits.DefaultMaximumArrayElements + 1)]
    public void ValidateArrayLength_WithInvalidDimension_ReturnsInvalidRequest(int length)
    {
        using var document = JsonDocument.Parse(length.ToString(System.Globalization.CultureInfo.InvariantCulture));

        var result = NativeWireReadContext.Decode(
            document.RootElement,
            NativeWireReadLimits.Default,
            CancellationToken.None,
            (context, root) =>
            {
                var decodedLength = context.ReadInt32(root);
                context.ValidateArrayLength(root, decodedLength);
                return new DecodedValue(decodedLength);
            });

        result.Succeeded.ShouldBeFalse();
        result.Value.ShouldBeNull();
        result.Error!.Code.ShouldBe(EngineErrorCode.InvalidRequest);
        result.Error.Message.ShouldContain("Array length");
    }

    /// <summary>Verifies dimension validation observes cancellation after numeric decoding and before native allocation.</summary>
    [Fact]
    public void ValidateArrayLength_WhenCanceled_ThrowsCancellation()
    {
        using var document = JsonDocument.Parse("1");
        using var cancellation = new CancellationTokenSource();

        Should.Throw<OperationCanceledException>(() => NativeWireReadContext.Decode(
            document.RootElement,
            NativeWireReadLimits.Default,
            cancellation.Token,
            (context, root) =>
            {
                var length = context.ReadInt32(root);
                cancellation.Cancel();
                context.ValidateArrayLength(root, length);
                return new DecodedValue(length);
            }));
    }

    /// <summary>Verifies a maximum-size array can be consumed in reverse order after one linear capture while preserving every index.</summary>
    [Fact]
    public void ReadArray_AtMaximumLength_SupportsReverseIndexedTraversal()
    {
        const int count = NativeWireReadLimits.DefaultMaximumArrayElements;
        var json = new StringBuilder(count * 16);
        json.Append('[');
        for (var index = 0; index < count; index++)
        {
            if (index > 0)
            {
                json.Append(',');
            }

            json.Append("{\"value\":");
            json.Append(index);
            json.Append('}');
        }

        json.Append(']');
        using var document = JsonDocument.Parse(json.ToString());

        var result = NativeWireReadContext.Decode(
            document.RootElement,
            NativeWireReadLimits.Default,
            CancellationToken.None,
            static (context, root) =>
            {
                var array = context.ReadArray(root);
                long sum = 0;
                for (var index = array.Count - 1; index >= 0; index--)
                {
                    var item = context.ReadObject(array.GetElement(index), ValueShape);
                    sum += context.ReadInt32(item.GetRequiredProperty("value"));
                }

                return new DecodedLongValue(sum);
            });

        result.Succeeded.ShouldBeTrue();
        result.Value!.Value.ShouldBe((long)count * (count - 1) / 2);
    }

    /// <summary>Verifies indexed array access observes cancellation before publishing another child value.</summary>
    [Fact]
    public void ReadArray_WhenCanceledBeforeIndexedAccess_ThrowsCancellation()
    {
        using var document = JsonDocument.Parse("[1]");
        using var cancellation = new CancellationTokenSource();

        Should.Throw<OperationCanceledException>(() => NativeWireReadContext.Decode(
            document.RootElement,
            NativeWireReadLimits.Default,
            cancellation.Token,
            (context, root) =>
            {
                var array = context.ReadArray(root);
                cancellation.Cancel();
                return new DecodedValue(context.ReadInt32(array.GetElement(0)));
            }));
    }

    /// <summary>Verifies 64-bit values preserve current numeric JSON and JavaScript-safe canonical decimal-string forms without double conversion.</summary>
    [Theory]
    [InlineData("18446744073709551615")]
    [InlineData("\"18446744073709551615\"")]
    public void ReadUInt64_WithExactSupportedRepresentation_PreservesAllBits(string json)
    {
        using var document = JsonDocument.Parse(json);

        var result = NativeWireReadContext.Decode(
            document.RootElement,
            NativeWireReadLimits.Default,
            CancellationToken.None,
            static (context, root) => new DecodedUnsignedValue(context.ReadUInt64(root)));

        result.Succeeded.ShouldBeTrue();
        result.Value!.Value.ShouldBe(ulong.MaxValue);
    }

    /// <summary>Verifies noncanonical decimal text is rejected instead of selecting an alternate integer spelling.</summary>
    [Fact]
    public void ReadInt64_WithNonCanonicalDecimalString_ReturnsInvalidRequest()
    {
        using var document = JsonDocument.Parse("\"-0\"");

        var result = NativeWireReadContext.Decode(
            document.RootElement,
            NativeWireReadLimits.Default,
            CancellationToken.None,
            static (context, root) => new DecodedLongValue(context.ReadInt64(root)));

        result.Succeeded.ShouldBeFalse();
        result.Error!.Code.ShouldBe(EngineErrorCode.InvalidRequest);
    }

    /// <summary>Verifies escaped unpaired UTF-16 surrogates become typed input failures instead of unexpected decoder exceptions.</summary>
    [Theory]
    [InlineData("\"\\uD800\"")]
    [InlineData("\"\\uDC00\"")]
    public void ReadString_WithIncompleteUtf16_ReturnsInvalidRequest(string json)
    {
        using var document = JsonDocument.Parse(json);

        var result = NativeWireReadContext.Decode(
            document.RootElement,
            NativeWireReadLimits.Default,
            CancellationToken.None,
            static (context, root) => new DecodedString(context.ReadString(root)));

        result.Succeeded.ShouldBeFalse();
        result.Value.ShouldBeNull();
        result.Error!.Code.ShouldBe(EngineErrorCode.InvalidRequest);
        result.Error.Message.ShouldContain("complete UTF-16");
    }

    /// <summary>Verifies an escaped unpaired surrogate in a property name fails before any typed value is returned.</summary>
    [Fact]
    public void ReadObject_WithIncompleteUtf16PropertyName_ReturnsInvalidRequest()
    {
        using var document = JsonDocument.Parse("{\"\\uD800\":1,\"value\":2}");

        var result = NativeWireReadContext.Decode(
            document.RootElement,
            NativeWireReadLimits.Default,
            CancellationToken.None,
            static (context, root) =>
            {
                var value = context.ReadObject(root, ValueShape);
                return new DecodedValue(context.ReadInt32(value.GetRequiredProperty("value")));
            });

        result.Succeeded.ShouldBeFalse();
        result.Value.ShouldBeNull();
        result.Error!.Code.ShouldBe(EngineErrorCode.InvalidRequest);
        result.Error.Message.ShouldContain("property name");
    }

    /// <summary>Verifies a complete escaped surrogate pair is decoded unchanged.</summary>
    [Fact]
    public void ReadString_WithCompleteSurrogatePair_ReturnsScalar()
    {
        using var document = JsonDocument.Parse("\"\\uD83D\\uDE00\"");

        var result = NativeWireReadContext.Decode(
            document.RootElement,
            NativeWireReadLimits.Default,
            CancellationToken.None,
            static (context, root) => new DecodedString(context.ReadString(root)));

        result.Succeeded.ShouldBeTrue();
        result.Value!.Value.ShouldBe("😀");
    }

    /// <summary>Verifies finite and non-finite single-precision values preserve authoritative bits and redundant representation rules.</summary>
    [Theory]
    [InlineData("{\"text\":\"1.5\",\"bits\":\"0x3FC00000\",\"number\":1.5}", 1069547520L)]
    [InlineData("{\"text\":\"NaN\",\"bits\":\"0x7FC01234\"}", 2143294004L)]
    public void ReadSingle_WithConsistentRepresentations_PreservesBits(string json, long expectedBits)
    {
        using var document = JsonDocument.Parse(json);

        var result = NativeWireReadContext.Decode(
            document.RootElement,
            NativeWireReadLimits.Default,
            CancellationToken.None,
            static (context, root) => new DecodedSingleValue(context.ReadSingle(root)));

        result.Succeeded.ShouldBeTrue();
        BitConverter.SingleToUInt32Bits(result.Value!.Value).ShouldBe((uint)expectedBits);
    }

    /// <summary>Verifies a redundant floating-point number that disagrees with authoritative bits is rejected.</summary>
    [Fact]
    public void ReadDouble_WithInconsistentNumber_ReturnsInvalidRequest()
    {
        using var document = JsonDocument.Parse(
            "{\"text\":\"1.5\",\"bits\":\"0x3FF8000000000000\",\"number\":1.5000000000000002}");

        var result = NativeWireReadContext.Decode(
            document.RootElement,
            NativeWireReadLimits.Default,
            CancellationToken.None,
            static (context, root) => new DecodedDoubleValue(context.ReadDouble(root)));

        result.Succeeded.ShouldBeFalse();
        result.Error!.Message.ShouldContain("authoritative bits");
    }

    /// <summary>Verifies Base64 uses its decoded-byte limit rather than the smaller semantic-string policy.</summary>
    [Fact]
    public void ReadBytes_WithSpecializedBound_PreservesCanonicalPayload()
    {
        using var document = JsonDocument.Parse("{\"length\":4,\"base64\":\"AAECAw==\"}");
        var limits = new NativeWireReadLimits(10, 10, 10, 6, 4);

        var result = NativeWireReadContext.Decode(
            document.RootElement,
            limits,
            CancellationToken.None,
            static (context, root) => new DecodedBytes(context.ReadBytes(root)));

        result.Succeeded.ShouldBeTrue();
        result.Value!.Value.ShouldBe(new byte[] { 0, 1, 2, 3 });
    }

    /// <summary>Verifies the declared byte length is rejected before decoding when it exceeds the configured bound.</summary>
    [Fact]
    public void ReadBytes_WithOversizedDeclaredLength_ReturnsInvalidRequest()
    {
        using var document = JsonDocument.Parse("{\"length\":4,\"base64\":\"AAECAw==\"}");
        var limits = new NativeWireReadLimits(10, 10, 10, 100, 3);

        var result = NativeWireReadContext.Decode(
            document.RootElement,
            limits,
            CancellationToken.None,
            static (context, root) => new DecodedBytes(context.ReadBytes(root)));

        result.Succeeded.ShouldBeFalse();
        result.Error!.Message.ShouldContain("between 0 and 3");
    }

    /// <summary>Verifies schema nodes retain detached JSON and content-bound canonical catalog identities.</summary>
    [Fact]
    public void NativeWireSchemaNode_WithDisposedDocuments_RetainsDetachedSchemaAndTemplate()
    {
        var identity = new NativeWireSchemaCatalogIdentity(
            SupportedGame.Starfield,
            GameRelease.Starfield,
            "1",
            new string('a', 64));
        var key = new NativeWireSchemaNodeKey(
            identity.CatalogId,
            NativeWireSchemaNodeKind.Type,
            "Example.Type");
        NativeWireSchemaNode node;
        using (var schemaDocument = JsonDocument.Parse("{\"constructible\":true}"))
        using (var templateDocument = JsonDocument.Parse("{\"value\":0}"))
        {
            node = new NativeWireSchemaNode(
                key,
                schemaDocument.RootElement,
                templateDocument.RootElement);
        }

        identity.CatalogId.ShouldBe(new string('A', 64));
        node.Schema.GetProperty("constructible").GetBoolean().ShouldBeTrue();
        node.DefaultTemplate!.Value.GetProperty("value").GetInt32().ShouldBe(0);
    }

    /// <summary>Holds one decoded signed 32-bit test value.</summary>
    private sealed class DecodedValue
    {
        /// <summary>Initializes the decoded test value.</summary>
        /// <param name="value">The exact decoded value.</param>
        internal DecodedValue(int value)
        {
            Value = value;
        }

        /// <summary>Gets the exact decoded value.</summary>
        internal int Value { get; }
    }

    /// <summary>Holds one decoded unsigned 64-bit test value.</summary>
    private sealed class DecodedUnsignedValue
    {
        /// <summary>Initializes the decoded test value.</summary>
        /// <param name="value">The exact decoded value.</param>
        internal DecodedUnsignedValue(ulong value)
        {
            Value = value;
        }

        /// <summary>Gets the exact decoded value.</summary>
        internal ulong Value { get; }
    }

    /// <summary>Holds one decoded signed 64-bit test value.</summary>
    private sealed class DecodedLongValue
    {
        /// <summary>Initializes the decoded test value.</summary>
        /// <param name="value">The exact decoded value.</param>
        internal DecodedLongValue(long value)
        {
            Value = value;
        }

        /// <summary>Gets the exact decoded value.</summary>
        internal long Value { get; }
    }

    /// <summary>Holds one decoded single-precision test value.</summary>
    private sealed class DecodedSingleValue
    {
        /// <summary>Initializes the decoded test value.</summary>
        /// <param name="value">The exact decoded value.</param>
        internal DecodedSingleValue(float value)
        {
            Value = value;
        }

        /// <summary>Gets the exact decoded value.</summary>
        internal float Value { get; }
    }

    /// <summary>Holds one decoded string test value.</summary>
    private sealed class DecodedString
    {
        /// <summary>Initializes the decoded string test value.</summary>
        /// <param name="value">The exact decoded string.</param>
        internal DecodedString(string value)
        {
            Value = value;
        }

        /// <summary>Gets the exact decoded string.</summary>
        internal string Value { get; }
    }

    /// <summary>Holds one decoded double-precision test value.</summary>
    private sealed class DecodedDoubleValue
    {
        /// <summary>Initializes the decoded test value.</summary>
        /// <param name="value">The exact decoded value.</param>
        internal DecodedDoubleValue(double value)
        {
            Value = value;
        }

        /// <summary>Gets the exact decoded value.</summary>
        internal double Value { get; }
    }

    /// <summary>Holds one decoded byte-sequence test value.</summary>
    private sealed class DecodedBytes
    {
        /// <summary>Initializes the decoded byte-sequence test value.</summary>
        /// <param name="value">The exact decoded bytes.</param>
        internal DecodedBytes(byte[] value)
        {
            Value = value;
        }

        /// <summary>Gets the exact decoded bytes.</summary>
        internal byte[] Value { get; }
    }
}
