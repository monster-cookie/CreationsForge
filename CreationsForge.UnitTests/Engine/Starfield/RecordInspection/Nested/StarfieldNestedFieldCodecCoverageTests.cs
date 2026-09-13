using System.Reflection;
using System.Text.Json;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.RecordInspection;
using CreationsForge.Starfield.PluginAdapter.RecordInspection;
using Mutagen.Bethesda.Starfield;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.Starfield.RecordInspection.Nested;

/// <summary>
/// Guards the generated Starfield nested-field codec against installed Mutagen type, field, and emitted-key drift.
/// </summary>
public sealed class StarfieldNestedFieldCodecCoverageTests
{
    /// <summary>The logical name assigned to the checked-in generated record field manifest.</summary>
    private const string ManifestResourceName = "CreationsForge.Starfield.RecordFieldManifest.json";

    /// <summary>The package-scoped prefix required on every generated concrete plugin type discriminator.</summary>
    private const string RecordTypePrefix = "Mutagen.Bethesda.Starfield/0.55.0-alpha.48:";

    /// <summary>
    /// Verifies the manifest contains every installed concrete root type and every FieldIndex member of each reachable type.
    /// </summary>
    [Fact]
    public void Manifest_CoversInstalledRootsAndIndexedFields()
    {
        using var manifest = LoadManifest();
        var root = manifest.RootElement;
        var entries = root.GetProperty("types").EnumerateArray().ToArray();

        root.GetProperty("indexedTypeCount").GetInt32().ShouldBe(722);
        root.GetProperty("indexedFieldCount").GetInt32().ShouldBe(5928);
        root.GetProperty("componentTypeCount").GetInt32().ShouldBe(66);
        root.GetProperty("conditionDataTypeCount").GetInt32().ShouldBe(608);
        root.GetProperty("conditionTypeCount").GetInt32().ShouldBe(2);
        entries.Length.ShouldBe(722);

        var starfieldAssembly = typeof(FormList).Assembly;
        var installedRoots = starfieldAssembly.GetTypes()
            .Where(type => type.IsClass
                && !type.IsAbstract
                && !type.IsGenericTypeDefinition
                && (typeof(AComponent).IsAssignableFrom(type)
                    || typeof(ConditionData).IsAssignableFrom(type)
                    || typeof(Condition).IsAssignableFrom(type)))
            .Select(type => type.FullName!)
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();
        var manifestedRoots = entries
            .Where(entry => entry.GetProperty("role").GetString() is "component" or "condition-data" or "condition")
            .Select(entry => entry.GetProperty("recordType").GetString()!)
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToArray();

        manifestedRoots.ShouldBe(installedRoots);
        entries.Select(entry => entry.GetProperty("recordType").GetString())
            .Distinct(StringComparer.Ordinal)
            .Count()
            .ShouldBe(entries.Length);

        foreach (var entry in entries)
        {
            var recordTypeName = GetRequiredString(entry, "recordType");
            var recordType = starfieldAssembly.GetType(recordTypeName, throwOnError: true)!;
            var getterTypeName = GetRequiredString(entry, "getterType");
            var getterType = starfieldAssembly.GetType(getterTypeName, throwOnError: true)!;
            getterType.IsAssignableFrom(recordType).ShouldBeTrue($"{recordTypeName} must implement {getterTypeName}.");

            var fieldIndex = starfieldAssembly.GetType(recordTypeName + "_FieldIndex", throwOnError: true)!;
            var installedFields = Enum.GetNames(fieldIndex)
                .OrderBy(name => Convert.ToUInt16(Enum.Parse(fieldIndex, name)))
                .ToArray();
            var manifestFields = entry.GetProperty("fields").EnumerateArray().ToArray();
            manifestFields.Select(field => GetRequiredString(field, "Name")).ShouldBe(
                installedFields,
                $"{recordTypeName} manifest fields must match installed FieldIndex order.");

            foreach (var field in manifestFields)
            {
                var name = GetRequiredString(field, "Name");
                field.GetProperty("Ordinal").GetUInt16().ShouldBe(
                    Convert.ToUInt16(Enum.Parse(fieldIndex, name)),
                    $"{recordTypeName}.{name} must retain its installed FieldIndex ordinal.");
            }
        }
    }

    /// <summary>
    /// Invokes every generated concrete writer in read and canonical modes and every comparer so emitted keys cannot be hidden by a self-consistent manifest omission.
    /// </summary>
    [Fact]
    public void GeneratedVisitors_WriteEveryManifestedKeyAndCompareEveryConcreteType()
    {
        using var manifest = LoadManifest();
        var codecType = GetCodecType();
        var starfieldAssembly = typeof(FormList).Assembly;

        foreach (var entry in manifest.RootElement.GetProperty("types").EnumerateArray())
        {
            var recordTypeName = GetRequiredString(entry, "recordType");
            var getterTypeName = GetRequiredString(entry, "getterType");
            var recordType = starfieldAssembly.GetType(recordTypeName, throwOnError: true)!;
            var getterType = starfieldAssembly.GetType(getterTypeName, throwOnError: true)!;
            var value = CreateRecordValue(recordType);
            var writeMethod = GetRequiredMethod(
                codecType,
                "Write" + recordType.Name,
                typeof(Utf8JsonWriter),
                getterType,
                typeof(CancellationToken),
                typeof(RecordJsonWriteContext));

            using var written = InvokeWriter(writeMethod, value, recordTypeName, null);
            var canonicalContext = new RecordJsonWriteContext(RecordJsonWriteMode.CanonicalFingerprintV1);
            using var canonicalWritten = InvokeWriter(writeMethod, value, recordTypeName, canonicalContext);
            var expectedKeys = new[] { "$type" }
                .Concat(entry.GetProperty("explicitContractFields").EnumerateArray().Select(field => GetRequiredString(field, "name")))
                .Concat(entry.GetProperty("fields").EnumerateArray().Select(field => GetRequiredString(field, "Name")))
                .ToArray();
            written.RootElement.EnumerateObject().Select(property => property.Name).ShouldBe(
                expectedKeys,
                $"{recordTypeName} must emit each generated record field once and in contract order.");
            written.RootElement.GetProperty("$type").GetString().ShouldBe(RecordTypePrefix + recordTypeName);
            canonicalWritten.RootElement.EnumerateObject().Select(property => property.Name).ShouldBe(
                expectedKeys,
                $"{recordTypeName} must retain its generated contract in canonical mode.");
            canonicalContext.ValidationError.ShouldBeNull(
                $"A default {recordTypeName} must not produce a canonical validation diagnostic.");

            var compareMethod = GetRequiredMethod(
                codecType,
                "Compare" + recordType.Name,
                getterType,
                getterType,
                typeof(string),
                typeof(ICollection<SemanticChangeDescriptor>),
                typeof(CancellationToken));
            var changes = new List<SemanticChangeDescriptor>();
            Invoke(
                compareMethod,
                recordTypeName,
                value,
                value,
                "$",
                changes,
                TestContext.Current.CancellationToken);
            changes.ShouldBeEmpty($"{recordTypeName} must compare equal to the same plugin instance.");
        }
    }

    /// <summary>Loads the generated manifest through its stable embedded-resource identity.</summary>
    /// <returns>An owned JSON document containing the generated coverage contract.</returns>
    private static JsonDocument LoadManifest()
    {
        using var stream = typeof(StarfieldNestedFieldCodecCoverageTests).Assembly.GetManifestResourceStream(ManifestResourceName)
            ?? throw new InvalidOperationException($"Embedded resource {ManifestResourceName} was not found.");
        return JsonDocument.Parse(stream);
    }

    /// <summary>Resolves the internal generated codec from the Starfield production assembly.</summary>
    /// <returns>The internal static codec type.</returns>
    private static Type GetCodecType()
    {
        return typeof(StarfieldFormListInspector).Assembly.GetType(
            "CreationsForge.Starfield.PluginAdapter.RecordInspection.StarfieldNestedFieldCodec",
            throwOnError: true)!;
    }

    /// <summary>Reads one required nonempty string property from a manifest element.</summary>
    /// <param name="element">The manifest element containing the property.</param>
    /// <param name="propertyName">The exact manifest property name.</param>
    /// <returns>The required string value.</returns>
    private static string GetRequiredString(JsonElement element, string propertyName)
    {
        return element.GetProperty(propertyName).GetString()
            ?? throw new InvalidOperationException($"Manifest property {propertyName} was null.");
    }

    /// <summary>Constructs one mutable record value through its generated parameterless constructor.</summary>
    /// <param name="recordType">The concrete plugin mutable type.</param>
    /// <returns>A record value assignable to its generated getter interface.</returns>
    private static object CreateRecordValue(Type recordType)
    {
        try
        {
            return System.Activator.CreateInstance(recordType, nonPublic: true)
                ?? throw new InvalidOperationException($"Constructor for {recordType.FullName} returned null.");
        }
        catch (Exception exception)
        {
            throw new InvalidOperationException($"Could not construct generated plugin type {recordType.FullName}.", exception);
        }
    }

    /// <summary>Resolves one exact private generated visitor method.</summary>
    /// <param name="codecType">The internal generated codec type.</param>
    /// <param name="methodName">The exact generated method name.</param>
    /// <param name="parameterTypes">The exact generated parameter types.</param>
    /// <returns>The matching generated method.</returns>
    private static MethodInfo GetRequiredMethod(Type codecType, string methodName, params Type[] parameterTypes)
    {
        return codecType.GetMethod(
                methodName,
                BindingFlags.NonPublic | BindingFlags.Static,
                binder: null,
                types: parameterTypes,
                modifiers: null)
            ?? throw new InvalidOperationException($"Generated method {methodName} was not found.");
    }

    /// <summary>Invokes one concrete writer and parses its complete object output.</summary>
    /// <param name="writeMethod">The exact private generated writer.</param>
    /// <param name="value">The concrete record getter value.</param>
    /// <param name="recordTypeName">The plugin type identity used in failures.</param>
    /// <param name="context">The per-call write mode and validation state, or null for read-view output.</param>
    /// <returns>An owned parsed JSON document.</returns>
    private static JsonDocument InvokeWriter(
        MethodInfo writeMethod,
        object value,
        string recordTypeName,
        RecordJsonWriteContext? context)
    {
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
        {
            Invoke(writeMethod, recordTypeName, writer, value, TestContext.Current.CancellationToken, context);
        }

        return JsonDocument.Parse(stream.ToArray());
    }

    /// <summary>Invokes a generated method while adding plugin type context to reflection failures.</summary>
    /// <param name="method">The generated method to invoke.</param>
    /// <param name="recordTypeName">The plugin type identity used in failures.</param>
    /// <param name="arguments">The generated method arguments.</param>
    private static void Invoke(MethodInfo method, string recordTypeName, params object?[] arguments)
    {
        try
        {
            method.Invoke(null, arguments);
        }
        catch (TargetInvocationException exception)
        {
            throw new InvalidOperationException(
                $"Generated method {method.Name} failed for {recordTypeName}.",
                exception.InnerException ?? exception);
        }
    }
}
