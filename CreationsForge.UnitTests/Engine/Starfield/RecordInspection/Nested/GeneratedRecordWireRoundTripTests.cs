using System.Text.Json;
using Mutagen.Bethesda.Starfield;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.Starfield.RecordInspection.Nested;

/// <summary>Verifies generated Starfield schemas, installed defaults, readers, and writers remain one coherent catalog.</summary>
public sealed class GeneratedRecordWireRoundTripTests
{
    /// <summary>Verifies every honest generated concrete installed default survives read and rewrite without wire drift.</summary>
    [Fact]
    public void ConcreteDefaults_All719AvailableRoundTripExactly()
    {
        var concreteTypeNames = GeneratedRecordWireTestSupport.TypeNames
            .Where(name => name.StartsWith(GeneratedRecordWireTestSupport.ConcreteTypePrefix, StringComparison.Ordinal))
            .ToArray();
        var defaultFailures = new List<string>();
        var unavailableTypeNames = new List<string>();
        var availableCount = 0;

        concreteTypeNames.Length.ShouldBe(722);
        foreach (var recordTypeName in concreteTypeNames)
        {
            try
            {
                var defaultJson = GeneratedRecordWireTestSupport.CreateDefaultJson(recordTypeName);
                if (defaultJson is null)
                {
                    unavailableTypeNames.Add(recordTypeName);
                    continue;
                }

                var decoded = GeneratedRecordWireTestSupport.Decode(recordTypeName, defaultJson);
                if (!decoded.Succeeded)
                {
                    defaultFailures.Add($"{recordTypeName}: decode failed: {decoded.Error?.Message}");
                    continue;
                }

                if (decoded.Value is null)
                {
                    defaultFailures.Add($"{recordTypeName}: decoder returned a null record value.");
                    continue;
                }

                if (!string.Equals(decoded.Value.GetType().FullName, recordTypeName, StringComparison.Ordinal))
                {
                    defaultFailures.Add($"{recordTypeName}: decoder returned {decoded.Value.GetType().FullName}.");
                    continue;
                }

                var rewritten = GeneratedRecordWireTestSupport.Write(recordTypeName, decoded.Value);
                if (!string.Equals(rewritten, defaultJson, StringComparison.Ordinal))
                {
                    defaultFailures.Add($"{recordTypeName}: rewritten default had semantic or ordering drift.");
                    continue;
                }

                availableCount++;
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                defaultFailures.Add($"{recordTypeName}: {exception.GetType().Name}: {exception.Message}");
            }
        }

        defaultFailures.ShouldBeEmpty("Every advertised available constructor default must decode and rewrite exactly.");
        availableCount.ShouldBe(719);
        unavailableTypeNames.ShouldBe(new[]
        {
            "Mutagen.Bethesda.Starfield.ConditionFloat",
            "Mutagen.Bethesda.Starfield.ConditionGlobal",
            "Mutagen.Bethesda.Starfield.VolumesComponentItem",
        });
        var unavailableReasons = new[]
        {
            new KeyValuePair<string, string>(
                "Mutagen.Bethesda.Starfield.ConditionFloat",
                "The installed public constructor leaves required Data null; supply a concrete ConditionData payload."),
            new KeyValuePair<string, string>(
                "Mutagen.Bethesda.Starfield.ConditionGlobal",
                "The installed public constructor leaves required Data null; supply a concrete ConditionData payload."),
            new KeyValuePair<string, string>(
                "Mutagen.Bethesda.Starfield.VolumesComponentItem",
                "The installed public constructor leaves required Ender null; supply a concrete AVolumesUnknownEnder payload."),
        };
        foreach (var unavailableReason in unavailableReasons)
        {
            using var schema = JsonDocument.Parse(GeneratedRecordWireTestSupport.GetSchemaJson(unavailableReason.Key));
            schema.RootElement.GetProperty("x-default-unavailable-reason").GetString().ShouldBe(
                unavailableReason.Value);
        }
    }

    /// <summary>Verifies both condition roots accept and exactly rewrite an explicit concrete condition-data payload.</summary>
    /// <param name="useGlobalComparison">Whether the condition uses a global link rather than a float comparison value.</param>
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ConditionRoot_WithConcreteData_RoundTripsExactly(bool useGlobalComparison)
    {
        Condition condition = useGlobalComparison
            ? new ConditionGlobal { Data = new BiomeHasKeywordConditionData() }
            : new ConditionFloat { Data = new BiomeHasKeywordConditionData() };
        var recordTypeName = condition.GetType().FullName!;
        var written = GeneratedRecordWireTestSupport.Write(recordTypeName, condition);

        GeneratedRecordWireTestSupport.RoundTrip(recordTypeName, written).ShouldBe(written);

        using var schema = JsonDocument.Parse(GeneratedRecordWireTestSupport.GetSchemaJson(recordTypeName));
        schema.RootElement.GetProperty("required")
            .EnumerateArray()
            .Any(item => item.ValueEquals("Data"))
            .ShouldBeTrue();
    }

    /// <summary>Verifies a volumes item accepts and exactly rewrites an explicit concrete ender payload.</summary>
    [Fact]
    public void VolumesComponentItem_WithConcreteEnder_RoundTripsExactly()
    {
        const string recordTypeName = "Mutagen.Bethesda.Starfield.VolumesComponentItem";
        var item = new VolumesComponentItem
        {
            Ender = new VolumesUnknownEnderEmpty(),
        };
        var written = GeneratedRecordWireTestSupport.Write(recordTypeName, item);

        GeneratedRecordWireTestSupport.RoundTrip(recordTypeName, written).ShouldBe(written);

        using var schema = JsonDocument.Parse(GeneratedRecordWireTestSupport.GetSchemaJson(recordTypeName));
        schema.RootElement.GetProperty("required")
            .EnumerateArray()
            .Any(element => element.ValueEquals("Ender"))
            .ShouldBeTrue();
    }

    /// <summary>Verifies every advertised schema has a unique stable identity and all exact root unions match generated coverage.</summary>
    [Fact]
    public void SchemaCatalog_AdvertisesStableCompleteGeneratedCoverage()
    {
        var typeNames = GeneratedRecordWireTestSupport.TypeNames;

        typeNames.Count.ShouldBe(737);
        typeNames.Distinct(StringComparer.Ordinal).Count().ShouldBe(typeNames.Count);
        GeneratedRecordWireTestSupport.ComponentTypeNames.Count.ShouldBe(66);
        GeneratedRecordWireTestSupport.ConditionTypeNames.Count.ShouldBe(2);
        GeneratedRecordWireTestSupport.ConditionDataTypeNames.Count.ShouldBe(608);
        GeneratedRecordWireTestSupport.ConditionTypeNames.ShouldBe(new[]
        {
            "Mutagen.Bethesda.Starfield.ConditionFloat",
            "Mutagen.Bethesda.Starfield.ConditionGlobal",
        });

        var schemaIds = new HashSet<string>(StringComparer.Ordinal);
        foreach (var typeName in typeNames)
        {
            using var schema = JsonDocument.Parse(GeneratedRecordWireTestSupport.GetSchemaJson(typeName));
            var expectedId = GeneratedRecordWireTestSupport.TypeUriPrefix + Uri.EscapeDataString(typeName);
            schema.RootElement.GetProperty("$schema").GetString().ShouldBe("https://json-schema.org/draft/2020-12/schema");
            schema.RootElement.GetProperty("$id").GetString().ShouldBe(expectedId);
            schemaIds.Add(expectedId).ShouldBeTrue($"Schema identity {expectedId} must be unique.");
            if (schema.RootElement.TryGetProperty("required", out var required))
            {
                var requiredNames = required.EnumerateArray().Select(item => item.GetString()).ToArray();
                requiredNames.Distinct(StringComparer.Ordinal).Count().ShouldBe(
                    requiredNames.Length,
                    $"{typeName} schema must not repeat a required property name.");
            }
        }

        AssertUnionCount("record.component", 66);
        AssertUnionCount("record.condition", 2);
        AssertUnionCount("record.condition-data", 608);
    }

    /// <summary>Verifies an indexed Function field replaces redundant derived Function output for the only two colliding plugin models.</summary>
    /// <param name="recordTypeName">The exact concrete condition-data type with an indexed Function field.</param>
    [Theory]
    [InlineData("Mutagen.Bethesda.Starfield.GetEventDataConditionData")]
    [InlineData("Mutagen.Bethesda.Starfield.UnknownConditionData")]
    public void IndexedFunctionConditionData_WritesAndRequiresFunctionOnce(string recordTypeName)
    {
        var defaultJson = GeneratedRecordWireTestSupport.CreateDefaultJson(recordTypeName);
        defaultJson.ShouldNotBeNull();
        using var written = JsonDocument.Parse(defaultJson);
        written.RootElement.EnumerateObject().Count(property => property.NameEquals("Function")).ShouldBe(1);

        using var schema = JsonDocument.Parse(GeneratedRecordWireTestSupport.GetSchemaJson(recordTypeName));
        schema.RootElement.GetProperty("required")
            .EnumerateArray()
            .Count(item => item.ValueEquals("Function"))
            .ShouldBe(1);

        GeneratedRecordWireTestSupport.RoundTrip(recordTypeName, defaultJson).ShouldBe(defaultJson);
    }

    /// <summary>Verifies authoring schemas mark only reconstructible derived projections as optional and read-only.</summary>
    [Fact]
    public void DerivedAuthoringFields_AreOptionalAndReadOnlyInSchemas()
    {
        using var conditionSchema = JsonDocument.Parse(GeneratedRecordWireTestSupport.GetSchemaJson(
            "Mutagen.Bethesda.Starfield.BiomeHasKeywordConditionData"));
        var conditionRoot = conditionSchema.RootElement;
        conditionRoot.GetProperty("properties").GetProperty("Function").GetProperty("readOnly").GetBoolean().ShouldBeTrue();
        conditionRoot.GetProperty("required").EnumerateArray().Any(item => item.ValueEquals("Function")).ShouldBeFalse();

        using var ownerSchema = JsonDocument.Parse(GeneratedRecordWireTestSupport.GetSchemaJson("record.form-link-or-index"));
        var ownerRoot = ownerSchema.RootElement;
        var ownerProperties = ownerRoot.GetProperty("properties");
        var ownerRequired = ownerRoot.GetProperty("required").EnumerateArray().Select(item => item.GetString()).ToArray();
        foreach (var propertyName in new[] { "usesLink", "usesAlias", "usesPackageData" })
        {
            ownerProperties.GetProperty(propertyName).GetProperty("readOnly").GetBoolean().ShouldBeTrue();
            ownerRequired.ShouldNotContain(propertyName);
        }

        using var assetSchema = JsonDocument.Parse(GeneratedRecordWireTestSupport.GetSchemaJson("record.asset-link"));
        var assetValue = assetSchema.RootElement.GetProperty("properties").GetProperty("value");
        var assetProperties = assetValue.GetProperty("properties");
        var assetRequired = assetValue.GetProperty("required").EnumerateArray().Select(item => item.GetString()).ToArray();
        assetRequired.ShouldBe(new[] { "isNull", "givenPath" });
        assetProperties.GetProperty("givenPath").GetProperty("type").GetString().ShouldBe("string");
        assetProperties.GetProperty("dataRelativePath").GetProperty("readOnly").GetBoolean().ShouldBeTrue();
        assetProperties.GetProperty("extension").GetProperty("readOnly").GetBoolean().ShouldBeTrue();
    }

    /// <summary>Verifies one generated union references the exact expected number of concrete schemas.</summary>
    /// <param name="nodeName">The generated union node name.</param>
    /// <param name="expectedCount">The exact installed concrete alternative count.</param>
    private static void AssertUnionCount(string nodeName, int expectedCount)
    {
        using var schema = JsonDocument.Parse(GeneratedRecordWireTestSupport.GetSchemaJson(nodeName));
        schema.RootElement.GetProperty("oneOf").GetArrayLength().ShouldBe(expectedCount);
        GeneratedRecordWireTestSupport.CreateDefaultJson(nodeName).ShouldBeNull();
    }
}
