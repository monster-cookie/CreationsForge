using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using CreationsForge.Core.Engine.NativeInspection;
using CreationsForge.Core.Engine.NativeWire;
using CreationsForge.Starfield.Native.NativeInspection;

namespace CreationsForge.UnitTests.Engine.Starfield.NativeInspection.Nested;

/// <summary>Provides strongly typed reflection bridges to the internal generated Starfield wire surface.</summary>
internal static class GeneratedNativeWireTestSupport
{
    /// <summary>The exact namespace prefix used by every generated concrete Starfield type name.</summary>
    internal const string ConcreteTypePrefix = "Mutagen.Bethesda.Starfield.";

    /// <summary>The generated schema helper type.</summary>
    private static readonly Type SchemaType = GetRequiredType(
        "CreationsForge.Starfield.Native.NativeInspection.StarfieldGeneratedNativeFieldSchema");

    /// <summary>The generated native codec type.</summary>
    private static readonly Type CodecType = GetRequiredType(
        "CreationsForge.Starfield.Native.NativeInspection.StarfieldNestedFieldCodec");

    /// <summary>The generated default JSON materializer.</summary>
    private static readonly CreateDefaultJsonDelegate CreateDefaultJsonCore = GetRequiredDelegate<CreateDefaultJsonDelegate>(
        SchemaType,
        "CreateDefaultJson",
        typeof(string),
        typeof(CancellationToken));

    /// <summary>The generated schema JSON lookup.</summary>
    private static readonly GetSchemaJsonDelegate GetSchemaJsonCore = GetRequiredDelegate<GetSchemaJsonDelegate>(
        SchemaType,
        "GetSchemaJson",
        typeof(string));

    /// <summary>The generated concrete native reader.</summary>
    private static readonly ReadNativeTypeDelegate ReadNativeTypeCore = GetRequiredDelegate<ReadNativeTypeDelegate>(
        CodecType,
        "ReadNativeType",
        typeof(NativeWireReadContext),
        typeof(NativeWireValue),
        typeof(string));

    /// <summary>The generated concrete native writer.</summary>
    private static readonly WriteNativeTypeDelegate WriteNativeTypeCore = GetRequiredDelegate<WriteNativeTypeDelegate>(
        CodecType,
        "WriteNativeType",
        typeof(Utf8JsonWriter),
        typeof(string),
        typeof(object),
        typeof(CancellationToken));

    /// <summary>Gets every generated union, leaf, and concrete schema node name.</summary>
    internal static IReadOnlyList<string> TypeNames => GetStringList("TypeNames");

    /// <summary>Gets every concrete generated component type name.</summary>
    internal static IReadOnlyList<string> ComponentTypeNames => GetStringList("ComponentTypeNames");

    /// <summary>Gets both concrete generated condition type names.</summary>
    internal static IReadOnlyList<string> ConditionTypeNames => GetStringList("ConditionTypeNames");

    /// <summary>Gets every concrete generated condition-data type name.</summary>
    internal static IReadOnlyList<string> ConditionDataTypeNames => GetStringList("ConditionDataTypeNames");

    /// <summary>Gets the generated absolute schema URI prefix.</summary>
    internal static string TypeUriPrefix => (string)(SchemaType.GetField(
            "TypeUriPrefix",
            BindingFlags.NonPublic | BindingFlags.Static)?.GetValue(null)
        ?? throw new InvalidOperationException("Generated TypeUriPrefix was not found."));

    /// <summary>Returns the generated schema JSON for one exact node name.</summary>
    /// <param name="nativeTypeName">The generated union, leaf, or concrete node name.</param>
    /// <returns>The compact generated JSON Schema.</returns>
    internal static string GetSchemaJson(string nativeTypeName)
    {
        return GetSchemaJsonCore(nativeTypeName);
    }

    /// <summary>Returns the actual installed default JSON for one exact node name.</summary>
    /// <param name="nativeTypeName">The generated leaf or concrete node name.</param>
    /// <returns>The compact default JSON, or <see langword="null"/> when no generic default exists.</returns>
    internal static string? CreateDefaultJson(string nativeTypeName)
    {
        return CreateDefaultJsonCore(nativeTypeName, TestContext.Current.CancellationToken);
    }

    /// <summary>Decodes one concrete native JSON value through the generated reader.</summary>
    /// <param name="nativeTypeName">The exact generated concrete native type name.</param>
    /// <param name="json">The complete native wire JSON object.</param>
    /// <param name="limits">The optional read limits, or <see langword="null"/> for defaults.</param>
    /// <returns>A complete native object or a typed invalid-request failure.</returns>
    internal static NativeWireDecodeResult<object> Decode(
        string nativeTypeName,
        string json,
        NativeWireReadLimits? limits = null)
    {
        using var document = JsonDocument.Parse(json);
        return NativeWireReadContext.Decode<object>(
            document.RootElement,
            limits ?? NativeWireReadLimits.Default,
            TestContext.Current.CancellationToken,
            (context, value) => ReadNativeTypeCore(context, value, nativeTypeName));
    }

    /// <summary>Writes one concrete native object through the generated type-selected writer.</summary>
    /// <param name="nativeTypeName">The exact generated concrete native type name.</param>
    /// <param name="value">The matching concrete native object.</param>
    /// <returns>The complete compact native wire JSON.</returns>
    internal static string Write(string nativeTypeName, object value)
    {
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            WriteNativeTypeCore(writer, nativeTypeName, value, TestContext.Current.CancellationToken);
        }

        return System.Text.Encoding.UTF8.GetString(stream.ToArray());
    }

    /// <summary>Decodes and rewrites one concrete native JSON value, asserting successful construction.</summary>
    /// <param name="nativeTypeName">The exact generated concrete native type name.</param>
    /// <param name="json">The complete native wire JSON object.</param>
    /// <returns>The rewritten compact native wire JSON.</returns>
    internal static string RoundTrip(string nativeTypeName, string json)
    {
        var decoded = Decode(nativeTypeName, json);
        if (!decoded.Succeeded)
        {
            throw new InvalidOperationException(
                $"Generated reader rejected {nativeTypeName}: {decoded.Error?.Message}");
        }

        return Write(nativeTypeName, decoded.Value!);
    }

    /// <summary>Parses one complete JSON object into a mutable node tree.</summary>
    /// <param name="json">The complete JSON object text.</param>
    /// <returns>The mutable root object.</returns>
    internal static JsonObject ParseObject(string json)
    {
        return JsonNode.Parse(json) as JsonObject
            ?? throw new InvalidOperationException("Expected a JSON object root.");
    }

    /// <summary>Creates the exact wire object for one single-precision value.</summary>
    /// <param name="value">The exact native value.</param>
    /// <returns>A mutable exact floating-point wire object.</returns>
    internal static JsonObject Single(float value)
    {
        return WriteLeaf(writer => NativeJsonLeafWriter.WriteSingle(writer, value));
    }

    /// <summary>Creates the exact wire object for one double-precision value.</summary>
    /// <param name="value">The exact native value.</param>
    /// <returns>A mutable exact floating-point wire object.</returns>
    internal static JsonObject Double(double value)
    {
        return WriteLeaf(writer => NativeJsonLeafWriter.WriteDouble(writer, value));
    }

    /// <summary>Creates the exact wire object for one byte sequence.</summary>
    /// <param name="value">The native bytes.</param>
    /// <returns>A mutable exact byte-sequence wire object.</returns>
    internal static JsonObject Bytes(IReadOnlyList<byte> value)
    {
        return WriteLeaf(writer => NativeJsonLeafWriter.WriteBytes(
            writer,
            value,
            TestContext.Current.CancellationToken));
    }

    /// <summary>Reads one generated immutable string list property.</summary>
    /// <param name="propertyName">The exact generated property name.</param>
    /// <returns>The generated list instance.</returns>
    private static IReadOnlyList<string> GetStringList(string propertyName)
    {
        return SchemaType.GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Static)?.GetValue(null)
            as IReadOnlyList<string>
            ?? throw new InvalidOperationException($"Generated {propertyName} was not found.");
    }

    /// <summary>Resolves one required generated type from the Starfield assembly.</summary>
    /// <param name="fullName">The exact internal type name.</param>
    /// <returns>The required generated type.</returns>
    private static Type GetRequiredType(string fullName)
    {
        return typeof(StarfieldFormListNativeInspector).Assembly.GetType(fullName, throwOnError: true)!;
    }

    /// <summary>Resolves one exact nonpublic generated method as a directly invokable delegate.</summary>
    /// <typeparam name="TDelegate">The exact generated method signature.</typeparam>
    /// <param name="declaringType">The generated declaring type.</param>
    /// <param name="methodName">The exact generated method name.</param>
    /// <param name="parameterTypes">The exact parameter types in declaration order.</param>
    /// <returns>A direct delegate that preserves generated expected-input exceptions.</returns>
    private static TDelegate GetRequiredDelegate<TDelegate>(
        Type declaringType,
        string methodName,
        params Type[] parameterTypes)
        where TDelegate : Delegate
    {
        var method = declaringType.GetMethod(
                methodName,
                BindingFlags.NonPublic | BindingFlags.Static,
                binder: null,
                types: parameterTypes,
                modifiers: null)
            ?? throw new InvalidOperationException($"Generated method {declaringType.FullName}.{methodName} was not found.");
        return method.CreateDelegate<TDelegate>();
    }

    /// <summary>Writes one Core native leaf helper into a mutable JSON object.</summary>
    /// <param name="write">The exact leaf write operation.</param>
    /// <returns>The parsed mutable leaf object.</returns>
    private static JsonObject WriteLeaf(Action<Utf8JsonWriter> write)
    {
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            write(writer);
        }

        return ParseObject(System.Text.Encoding.UTF8.GetString(stream.ToArray()));
    }

    /// <summary>Represents the generated default JSON materializer signature.</summary>
    /// <param name="nativeTypeName">The exact generated node name.</param>
    /// <param name="cancellationToken">The traversal cancellation token.</param>
    /// <returns>The default JSON or <see langword="null"/>.</returns>
    private delegate string? CreateDefaultJsonDelegate(string nativeTypeName, CancellationToken cancellationToken);

    /// <summary>Represents the generated schema lookup signature.</summary>
    /// <param name="nativeTypeName">The exact generated node name.</param>
    /// <returns>The compact JSON Schema.</returns>
    private delegate string GetSchemaJsonDelegate(string nativeTypeName);

    /// <summary>Represents the generated concrete native reader signature.</summary>
    /// <param name="context">The bounded native read context.</param>
    /// <param name="value">The context-owned wire value.</param>
    /// <param name="nativeTypeName">The exact concrete type name.</param>
    /// <returns>The completely constructed native object.</returns>
    private delegate object ReadNativeTypeDelegate(
        NativeWireReadContext context,
        NativeWireValue value,
        string nativeTypeName);

    /// <summary>Represents the generated concrete native writer signature.</summary>
    /// <param name="writer">The caller-owned JSON writer.</param>
    /// <param name="nativeTypeName">The exact concrete type name.</param>
    /// <param name="value">The matching native object.</param>
    /// <param name="cancellationToken">The traversal cancellation token.</param>
    private delegate void WriteNativeTypeDelegate(
        Utf8JsonWriter writer,
        string nativeTypeName,
        object value,
        CancellationToken cancellationToken);
}
