using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using CreationsForge.Core.Engine.RecordInspection;
using CreationsForge.Core.Engine.RecordWire;
using CreationsForge.Starfield.PluginAdapter.RecordInspection;

namespace CreationsForge.UnitTests.Engine.Starfield.RecordInspection.Nested;

/// <summary>Provides strongly typed reflection bridges to the internal generated Starfield wire surface.</summary>
internal static class GeneratedRecordWireTestSupport
{
    /// <summary>The exact namespace prefix used by every generated concrete Starfield type name.</summary>
    internal const string ConcreteTypePrefix = "Mutagen.Bethesda.Starfield.";

    /// <summary>The generated schema helper type.</summary>
    private static readonly Type SchemaType = GetRequiredType(
        "CreationsForge.Starfield.PluginAdapter.RecordInspection.StarfieldGeneratedRecordFieldSchema");

    /// <summary>The generated plugin codec type.</summary>
    private static readonly Type CodecType = GetRequiredType(
        "CreationsForge.Starfield.PluginAdapter.RecordInspection.StarfieldNestedFieldCodec");

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

    /// <summary>The generated concrete plugin reader.</summary>
    private static readonly ReadRecordTypeDelegate ReadRecordTypeCore = GetRequiredDelegate<ReadRecordTypeDelegate>(
        CodecType,
        "ReadRecordType",
        typeof(RecordWireReadContext),
        typeof(RecordWireValue),
        typeof(string));

    /// <summary>The generated concrete plugin writer.</summary>
    private static readonly WriteRecordTypeDelegate WriteRecordTypeCore = GetRequiredDelegate<WriteRecordTypeDelegate>(
        CodecType,
        "WriteRecordType",
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
    /// <param name="recordTypeName">The generated union, leaf, or concrete node name.</param>
    /// <returns>The compact generated JSON Schema.</returns>
    internal static string GetSchemaJson(string recordTypeName)
    {
        return GetSchemaJsonCore(recordTypeName);
    }

    /// <summary>Returns the actual installed default JSON for one exact node name.</summary>
    /// <param name="recordTypeName">The generated leaf or concrete node name.</param>
    /// <returns>The compact default JSON, or <see langword="null"/> when no generic default exists.</returns>
    internal static string? CreateDefaultJson(string recordTypeName)
    {
        return CreateDefaultJsonCore(recordTypeName, TestContext.Current.CancellationToken);
    }

    /// <summary>Decodes one concrete record JSON value through the generated reader.</summary>
    /// <param name="recordTypeName">The exact generated concrete plugin type name.</param>
    /// <param name="json">The complete plugin wire JSON object.</param>
    /// <param name="limits">The optional read limits, or <see langword="null"/> for defaults.</param>
    /// <returns>A complete plugin object or a typed invalid-request failure.</returns>
    internal static RecordWireDecodeResult<object> Decode(
        string recordTypeName,
        string json,
        RecordWireReadLimits? limits = null)
    {
        using var document = JsonDocument.Parse(json);
        return RecordWireReadContext.Decode<object>(
            document.RootElement,
            limits ?? RecordWireReadLimits.Default,
            TestContext.Current.CancellationToken,
            (context, value) => ReadRecordTypeCore(context, value, recordTypeName));
    }

    /// <summary>Writes one concrete plugin object through the generated type-selected writer.</summary>
    /// <param name="recordTypeName">The exact generated concrete plugin type name.</param>
    /// <param name="value">The matching concrete plugin object.</param>
    /// <returns>The complete compact plugin wire JSON.</returns>
    internal static string Write(string recordTypeName, object value)
    {
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            WriteRecordTypeCore(writer, recordTypeName, value, TestContext.Current.CancellationToken);
        }

        return System.Text.Encoding.UTF8.GetString(stream.ToArray());
    }

    /// <summary>Decodes and rewrites one concrete record JSON value, asserting successful construction.</summary>
    /// <param name="recordTypeName">The exact generated concrete plugin type name.</param>
    /// <param name="json">The complete plugin wire JSON object.</param>
    /// <returns>The rewritten compact plugin wire JSON.</returns>
    internal static string RoundTrip(string recordTypeName, string json)
    {
        var decoded = Decode(recordTypeName, json);
        if (!decoded.Succeeded)
        {
            throw new InvalidOperationException(
                $"Generated reader rejected {recordTypeName}: {decoded.Error?.Message}");
        }

        return Write(recordTypeName, decoded.Value!);
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
    /// <param name="value">The exact record value.</param>
    /// <returns>A mutable exact floating-point wire object.</returns>
    internal static JsonObject Single(float value)
    {
        return WriteLeaf(writer => RecordJsonLeafWriter.WriteSingle(writer, value));
    }

    /// <summary>Creates the exact wire object for one double-precision value.</summary>
    /// <param name="value">The exact record value.</param>
    /// <returns>A mutable exact floating-point wire object.</returns>
    internal static JsonObject Double(double value)
    {
        return WriteLeaf(writer => RecordJsonLeafWriter.WriteDouble(writer, value));
    }

    /// <summary>Creates the exact wire object for one byte sequence.</summary>
    /// <param name="value">The plugin bytes.</param>
    /// <returns>A mutable exact byte-sequence wire object.</returns>
    internal static JsonObject Bytes(IReadOnlyList<byte> value)
    {
        return WriteLeaf(writer => RecordJsonLeafWriter.WriteBytes(
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
        return typeof(StarfieldFormListInspector).Assembly.GetType(fullName, throwOnError: true)!;
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

    /// <summary>Writes one Core plugin leaf helper into a mutable JSON object.</summary>
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
    /// <param name="recordTypeName">The exact generated node name.</param>
    /// <param name="cancellationToken">The traversal cancellation token.</param>
    /// <returns>The default JSON or <see langword="null"/>.</returns>
    private delegate string? CreateDefaultJsonDelegate(string recordTypeName, CancellationToken cancellationToken);

    /// <summary>Represents the generated schema lookup signature.</summary>
    /// <param name="recordTypeName">The exact generated node name.</param>
    /// <returns>The compact JSON Schema.</returns>
    private delegate string GetSchemaJsonDelegate(string recordTypeName);

    /// <summary>Represents the generated concrete plugin reader signature.</summary>
    /// <param name="context">The bounded plugin read context.</param>
    /// <param name="value">The context-owned wire value.</param>
    /// <param name="recordTypeName">The exact concrete type name.</param>
    /// <returns>The completely constructed plugin object.</returns>
    private delegate object ReadRecordTypeDelegate(
        RecordWireReadContext context,
        RecordWireValue value,
        string recordTypeName);

    /// <summary>Represents the generated concrete plugin writer signature.</summary>
    /// <param name="writer">The caller-owned JSON writer.</param>
    /// <param name="recordTypeName">The exact concrete type name.</param>
    /// <param name="value">The matching plugin object.</param>
    /// <param name="cancellationToken">The traversal cancellation token.</param>
    private delegate void WriteRecordTypeDelegate(
        Utf8JsonWriter writer,
        string recordTypeName,
        object value,
        CancellationToken cancellationToken);
}
