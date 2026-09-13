using System.Text.Json.Nodes;
using CreationsForge.Core.Engine.Contracts;
using CreationsForge.Core.Engine.RecordWire;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Starfield.Assets;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.Starfield.RecordInspection.Nested;

/// <summary>Verifies owner-derived unions, redundant metadata, closed shapes, and read limits at generated boundaries.</summary>
public sealed class GeneratedRecordWireRejectionTests
{
    /// <summary>Verifies link, alias-index, and package-index owner modes all reconstruct and rewrite exactly.</summary>
    /// <param name="useAliases">Whether the owner selects the alias-index branch.</param>
    /// <param name="usePackageData">Whether the owner selects the package-index branch.</param>
    /// <param name="expectedUsesLink">Whether the wrapper must report the ordinary link branch.</param>
    [Theory]
    [InlineData(false, false, true)]
    [InlineData(true, false, false)]
    [InlineData(false, true, false)]
    public void OwnerLinkedUnion_ValidOwnerMode_RoundTripsExactly(
        bool useAliases,
        bool usePackageData,
        bool expectedUsesLink)
    {
        const string recordTypeName = "Mutagen.Bethesda.Starfield.BiomeHasKeywordConditionData";
        var root = GetDefaultObject(recordTypeName);
        SetOwnerMode(root, useAliases, usePackageData, 41);
        var parameter = root["FirstParameter"]!.AsObject();

        var rewritten = GeneratedRecordWireTestSupport.RoundTrip(recordTypeName, root.ToJsonString());

        var rewrittenRoot = GeneratedRecordWireTestSupport.ParseObject(rewritten);
        var rewrittenParameter = rewrittenRoot["FirstParameter"]!.AsObject();
        rewrittenParameter["usesLink"]!.GetValue<bool>().ShouldBe(expectedUsesLink);
        rewrittenParameter["usesAlias"]!.GetValue<bool>().ShouldBe(useAliases);
        rewrittenParameter["usesPackageData"]!.GetValue<bool>().ShouldBe(usePackageData);
        rewrittenParameter["index"]!.GetValue<uint>().ShouldBe(41u);
        rewrittenParameter.ToJsonString().ShouldBe(parameter.ToJsonString());
    }

    /// <summary>Verifies each supplied owner-linked mode projection is validated at its exact property path.</summary>
    /// <param name="propertyName">The supplied derived mode property to contradict.</param>
    /// <param name="mismatchedValue">The value that contradicts the plugin owner flags.</param>
    [Theory]
    [InlineData("usesLink", false)]
    [InlineData("usesAlias", true)]
    [InlineData("usesPackageData", true)]
    public void OwnerLinkedUnion_WithMismatchedMode_ReturnsExactPropertyPath(
        string propertyName,
        bool mismatchedValue)
    {
        const string recordTypeName = "Mutagen.Bethesda.Starfield.BiomeHasKeywordConditionData";
        var root = GetDefaultObject(recordTypeName);
        SetOwnerMode(root, useAliases: false, usePackageData: false, index: 17);
        root["FirstParameter"]!.AsObject()[propertyName] = mismatchedValue;

        AssertInvalidRequestAtPath(recordTypeName, root, $"$.FirstParameter.{propertyName}");
    }

    /// <summary>Verifies omitted derived condition and owner-mode fields expand from the constructed plugin owner.</summary>
    [Fact]
    public void DerivedConditionAndOwnerFields_WhenOmitted_ExpandFromPluginState()
    {
        const string recordTypeName = "Mutagen.Bethesda.Starfield.BiomeHasKeywordConditionData";
        var root = GetDefaultObject(recordTypeName);
        var expectedFunction = root["Function"]!.GetValue<int>();
        SetOwnerMode(root, useAliases: true, usePackageData: false, index: 41);
        root.Remove("Function").ShouldBeTrue();
        var parameter = root["FirstParameter"]!.AsObject();
        parameter.Remove("usesLink").ShouldBeTrue();
        parameter.Remove("usesAlias").ShouldBeTrue();
        parameter.Remove("usesPackageData").ShouldBeTrue();

        var rewritten = GeneratedRecordWireTestSupport.ParseObject(
            GeneratedRecordWireTestSupport.RoundTrip(recordTypeName, root.ToJsonString()));

        rewritten["Function"]!.GetValue<int>().ShouldBe(expectedFunction);
        var rewrittenParameter = rewritten["FirstParameter"]!.AsObject();
        rewrittenParameter["usesLink"]!.GetValue<bool>().ShouldBeFalse();
        rewrittenParameter["usesAlias"]!.GetValue<bool>().ShouldBeTrue();
        rewrittenParameter["usesPackageData"]!.GetValue<bool>().ShouldBeFalse();
    }

    /// <summary>Verifies zero-length Array2d axes remain valid plugin shapes without weakening per-axis bounds.</summary>
    /// <param name="width">The accepted zero-inclusive width.</param>
    /// <param name="height">The accepted zero-inclusive height.</param>
    [Theory]
    [InlineData(0, 0)]
    [InlineData(0, 3)]
    [InlineData(3, 0)]
    public void Array2d_WithZeroLengthAxis_RoundTripsExactly(int width, int height)
    {
        const string recordTypeName = "Mutagen.Bethesda.Starfield.BlockHeightAdjustmentComponent";
        var root = GetDefaultObject(recordTypeName);
        var rows = new JsonArray();
        for (var y = 0; y < height; y++)
        {
            rows.Add(new JsonArray());
        }

        root["SurfaceBlocks"] = new JsonObject
        {
            ["width"] = width,
            ["height"] = height,
            ["rows"] = rows,
        };

        GeneratedRecordWireTestSupport.RoundTrip(recordTypeName, root.ToJsonString()).ShouldBe(root.ToJsonString());
    }

    /// <summary>Verifies each Array2d axis is bounded before allocation even when the other axis is zero.</summary>
    [Fact]
    public void Array2d_WithOversizedWidthAndZeroHeight_ReturnsInvalidRequestBeforeAllocation()
    {
        const string recordTypeName = "Mutagen.Bethesda.Starfield.BlockHeightAdjustmentComponent";
        var root = GetDefaultObject(recordTypeName);
        root["SurfaceBlocks"] = new JsonObject
        {
            ["width"] = int.MaxValue,
            ["height"] = 0,
            ["rows"] = new JsonArray(),
        };

        AssertInvalidRequest(
            recordTypeName,
            root,
            $"Array length {int.MaxValue} exceeds the per-array limit of {RecordWireReadLimits.DefaultMaximumArrayElements}",
            RecordWireReadLimits.Default);
    }

    /// <summary>Verifies a concrete condition-data discriminator cannot carry another derived Function value.</summary>
    [Fact]
    public void ConditionData_WithMismatchedDerivedFunction_ReturnsInvalidRequest()
    {
        const string recordTypeName = "Mutagen.Bethesda.Starfield.BiomeHasKeywordConditionData";
        var root = GetDefaultObject(recordTypeName);
        root["Function"] = root["Function"]!.GetValue<int>() + 1;

        AssertInvalidRequestAtPath(recordTypeName, root, "$.Function", "Derived Function");
    }

    /// <summary>Verifies exact generated type selection and closed-object membership are enforced before construction.</summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ConcreteObject_WithWrongTypeOrUnknownProperty_ReturnsInvalidRequest(bool replaceType)
    {
        const string recordTypeName = "Mutagen.Bethesda.Starfield.BlockEditorMetaDataComponent";
        var root = GetDefaultObject(recordTypeName);
        if (replaceType)
        {
            root["$type"] = "Mutagen.Bethesda.Starfield/0.55.0-alpha.48:Mutagen.Bethesda.Starfield.Activity";
        }
        else
        {
            root["unadvertised"] = true;
        }

        AssertInvalidRequest(recordTypeName, root, replaceType ? "Expected plugin type discriminator" : "not accepted");
    }

    /// <summary>Verifies the plugin null FormKey sentinel cannot masquerade as a non-null generated FormLink.</summary>
    [Fact]
    public void FormLink_WithNonNullWrapperAroundFormKeyNull_ReturnsInvalidRequest()
    {
        const string recordTypeName = "Mutagen.Bethesda.Starfield.ObjectProperty";
        var root = GetDefaultObject(recordTypeName);
        var actorValue = root["ActorValue"]!.AsObject();
        actorValue["isNull"] = false;
        actorValue["formKey"] = FormKey.Null.ToString();

        AssertInvalidRequest(recordTypeName, root, "null state does not match");
    }

    /// <summary>Verifies nullable record links preserve both accepted null FormKey representations exactly.</summary>
    /// <param name="useJsonNull">Whether the accepted input uses JSON null instead of the explicit plugin sentinel.</param>
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void FormLink_WithAcceptedNullFormKey_RoundTripsExactly(bool useJsonNull)
    {
        const string recordTypeName = "Mutagen.Bethesda.Starfield.AddToInventoryOnDestroyComponent";
        var root = GetDefaultObject(recordTypeName);
        var item = root["Item"]!.AsObject();
        item["isNull"] = true;
        item["formKey"] = useJsonNull ? null : FormKey.Null.ToString();

        var rewritten = GeneratedRecordWireTestSupport.RoundTrip(recordTypeName, root.ToJsonString());

        var rewrittenLink = GeneratedRecordWireTestSupport.ParseObject(rewritten)["Item"]!.AsObject();
        rewrittenLink["isNull"]!.GetValue<bool>().ShouldBeTrue();
        rewrittenLink.ContainsKey("formKey").ShouldBeTrue();
        if (useJsonNull)
        {
            rewrittenLink["formKey"].ShouldBeNull();
        }
        else
        {
            rewrittenLink["formKey"]!.GetValue<string>().ShouldBe(FormKey.Null.ToString());
        }
    }

    /// <summary>Verifies JSON null accepted for a nonnullable record link canonicalizes to its required FormKey.Null sentinel.</summary>
    [Fact]
    public void FormLink_WithAcceptedJsonNullOnNonNullableWrapper_CanonicalizesToNullSentinel()
    {
        const string recordTypeName = "Mutagen.Bethesda.Starfield.ObjectProperty";
        var root = GetDefaultObject(recordTypeName);
        var actorValue = root["ActorValue"]!.AsObject();
        actorValue["isNull"] = true;
        actorValue["formKey"] = null;

        var rewritten = GeneratedRecordWireTestSupport.RoundTrip(recordTypeName, root.ToJsonString());

        var rewrittenLink = GeneratedRecordWireTestSupport.ParseObject(rewritten)["ActorValue"]!.AsObject();
        rewrittenLink["isNull"]!.GetValue<bool>().ShouldBeTrue();
        rewrittenLink["formKey"]!.GetValue<string>().ShouldBe(FormKey.Null.ToString());
    }

    /// <summary>Verifies required polymorphic fields reject both explicit null and omission without weakening installed nullability.</summary>
    /// <param name="recordTypeName">The exact concrete plugin type containing the required polymorphic field.</param>
    /// <param name="propertyName">The required polymorphic property to invalidate.</param>
    /// <param name="omitProperty">Whether to omit the property instead of supplying JSON null.</param>
    [Theory]
    [InlineData("Mutagen.Bethesda.Starfield.ConditionFloat", "Data", false)]
    [InlineData("Mutagen.Bethesda.Starfield.ConditionFloat", "Data", true)]
    [InlineData("Mutagen.Bethesda.Starfield.ConditionGlobal", "Data", false)]
    [InlineData("Mutagen.Bethesda.Starfield.ConditionGlobal", "Data", true)]
    [InlineData("Mutagen.Bethesda.Starfield.VolumesComponentItem", "Ender", false)]
    [InlineData("Mutagen.Bethesda.Starfield.VolumesComponentItem", "Ender", true)]
    public void RequiredPolymorphicField_WithNullOrMissingValue_ReturnsInvalidRequest(
        string recordTypeName,
        string propertyName,
        bool omitProperty)
    {
        var root = CreateRequiredPolymorphicRoot(recordTypeName);
        if (omitProperty)
        {
            root.Remove(propertyName).ShouldBeTrue();
            AssertInvalidRequest(recordTypeName, root, $"Required property '{propertyName}' is missing.");
            return;
        }

        root[propertyName] = null;
        AssertInvalidRequestAtPath(recordTypeName, root, $"$.{propertyName}");
    }

    /// <summary>Verifies malformed or inconsistent asset paths fail at the exact supplied authoring property.</summary>
    /// <param name="propertyName">The supplied asset property to invalidate.</param>
    /// <param name="invalidValue">The invalid property value.</param>
    /// <param name="expectedPath">The exact reported plugin wire path.</param>
    [Theory]
    [InlineData("givenPath", "C:\\absolute\\Inspection.nif", "$.File.value.givenPath")]
    [InlineData("dataRelativePath", "Meshes\\Wrong.nif", "$.File.value.dataRelativePath")]
    [InlineData("extension", ".dds", "$.File.value.extension")]
    public void AssetLink_WithInvalidSuppliedPathData_ReturnsExactPropertyPath(
        string propertyName,
        string invalidValue,
        string expectedPath)
    {
        const string recordTypeName = "Mutagen.Bethesda.Starfield.Model";
        var root = CreateModelAssetRoot("Meshes\\CreationsForge\\Inspection.nif");
        invalidValue = propertyName == "givenPath" ? Path.GetFullPath("Inspection.nif") : invalidValue;
        root["File"]!.AsObject()["value"]!.AsObject()[propertyName] = invalidValue;

        AssertInvalidRequestAtPath(recordTypeName, root, expectedPath);
    }

    /// <summary>Verifies the required asset null-state flag must agree with an empty plugin path.</summary>
    [Fact]
    public void AssetLink_WithInconsistentNullState_ReturnsExactPropertyPath()
    {
        const string recordTypeName = "Mutagen.Bethesda.Starfield.Model";
        var root = CreateModelAssetRoot(string.Empty);
        root["File"]!.AsObject()["value"]!.AsObject()["isNull"] = false;

        AssertInvalidRequestAtPath(recordTypeName, root, "$.File.value.isNull");
    }

    /// <summary>Verifies an asset link requires a reconstructible string givenPath even for the null state.</summary>
    [Fact]
    public void AssetLink_WithNullGivenPath_ReturnsExactPropertyPath()
    {
        const string recordTypeName = "Mutagen.Bethesda.Starfield.Model";
        var root = CreateModelAssetRoot(string.Empty);
        root["File"]!.AsObject()["value"]!.AsObject()["givenPath"] = null;

        AssertInvalidRequestAtPath(recordTypeName, root, "$.File.value.givenPath");
    }

    /// <summary>Verifies translated-string target metadata cannot disagree with the retained language entries.</summary>
    [Fact]
    public void TranslatedString_WithMismatchedTargetValue_ReturnsInvalidRequest()
    {
        const string recordTypeName = "Mutagen.Bethesda.Starfield.Activity";
        var root = GetDefaultObject(recordTypeName);
        var description = root["Description"]!.AsObject();
        description["value"] = new JsonObject
        {
            ["targetLanguage"] = "English",
            ["value"] = "advertised",
            ["translations"] = new JsonArray(
                new JsonObject { ["language"] = "English", ["value"] = "retained" }),
        };

        AssertInvalidRequest(recordTypeName, root, "target value");
    }

    /// <summary>Verifies generated collection, string, byte, and depth traversal obey caller-supplied resource limits.</summary>
    [Fact]
    public void GeneratedReaders_WithExceededResourceLimits_ReturnInvalidRequest()
    {
        const string recordTypeName = "Mutagen.Bethesda.Starfield.BlockEditorMetaDataComponent";
        var arrayRoot = GetDefaultObject(recordTypeName);
        arrayRoot["UnknownInts"] = new JsonArray(1, 2);
        var arrayLimits = new RecordWireReadLimits(64, 4096, 1, 1024, 1024);
        AssertInvalidRequest(recordTypeName, arrayRoot, "per-array limit", arrayLimits);

        var stringRoot = GetDefaultObject(recordTypeName);
        stringRoot["UnknownString1"] = new string('x', 257);
        var stringLimits = new RecordWireReadLimits(64, 4096, 32, 256, 1024);
        AssertInvalidRequest(recordTypeName, stringRoot, "string length 257 exceeds the limit of 256", stringLimits);

        var bytesRoot = GetDefaultObject(recordTypeName);
        bytesRoot["UnknownEnding"] = GeneratedRecordWireTestSupport.Bytes(new byte[] { 0, 1, 2, 3 });
        var byteLimits = new RecordWireReadLimits(64, 4096, 32, 1024, 3);
        AssertInvalidRequest(recordTypeName, bytesRoot, "between 0 and 3", byteLimits);

        var depthRoot = GetDefaultObject(recordTypeName);
        var depthLimits = new RecordWireReadLimits(2, 4096, 32, 1024, 1024);
        AssertInvalidRequest(recordTypeName, depthRoot, "Nesting depth", depthLimits);
    }

    /// <summary>Loads the actual installed default for one generated concrete type.</summary>
    /// <param name="recordTypeName">The exact generated concrete type name.</param>
    /// <returns>A mutable default wire object.</returns>
    private static JsonObject GetDefaultObject(string recordTypeName)
    {
        var json = GeneratedRecordWireTestSupport.CreateDefaultJson(recordTypeName);
        json.ShouldNotBeNull();
        return GeneratedRecordWireTestSupport.ParseObject(json);
    }

    /// <summary>Writes one Model containing a typed asset path into a mutable generated wire object.</summary>
    /// <param name="path">The record asset path to write.</param>
    /// <returns>The complete generated Model wire object.</returns>
    private static JsonObject CreateModelAssetRoot(string path)
    {
        var model = new Model
        {
            File = new AssetLink<StarfieldModelAssetType>(path),
        };
        return GeneratedRecordWireTestSupport.ParseObject(
            GeneratedRecordWireTestSupport.Write("Mutagen.Bethesda.Starfield.Model", model));
    }

    /// <summary>Creates one valid wire root whose required polymorphic field can be invalidated by a rejection test.</summary>
    /// <param name="recordTypeName">The exact supported condition-root or volumes-item type name.</param>
    /// <returns>A complete valid mutable generated wire object.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the requested plugin type is outside this focused fixture set.</exception>
    private static JsonObject CreateRequiredPolymorphicRoot(string recordTypeName)
    {
        object recordValue = recordTypeName switch
        {
            "Mutagen.Bethesda.Starfield.ConditionFloat" => new ConditionFloat
            {
                Data = new BiomeHasKeywordConditionData(),
            },
            "Mutagen.Bethesda.Starfield.ConditionGlobal" => new ConditionGlobal
            {
                Data = new BiomeHasKeywordConditionData(),
            },
            "Mutagen.Bethesda.Starfield.VolumesComponentItem" => new VolumesComponentItem
            {
                Ender = new VolumesUnknownEnderEmpty(),
            },
            _ => throw new InvalidOperationException($"Plugin type {recordTypeName} has no required-polymorphic fixture."),
        };
        return GeneratedRecordWireTestSupport.ParseObject(
            GeneratedRecordWireTestSupport.Write(recordTypeName, recordValue));
    }

    /// <summary>Sets owner fields and their redundant link-or-index mode flags to one coherent state.</summary>
    /// <param name="root">The mutable condition-data wire object.</param>
    /// <param name="useAliases">Whether the owner selects alias indices.</param>
    /// <param name="usePackageData">Whether the owner selects package-data indices.</param>
    /// <param name="index">The retained index value.</param>
    private static void SetOwnerMode(JsonObject root, bool useAliases, bool usePackageData, uint index)
    {
        root["UseAliases"] = useAliases;
        root["UsePackageData"] = usePackageData;
        var parameter = root["FirstParameter"]!.AsObject();
        parameter["usesLink"] = !useAliases && !usePackageData;
        parameter["usesAlias"] = useAliases;
        parameter["usesPackageData"] = usePackageData;
        parameter["index"] = index;
    }

    /// <summary>Asserts a generated reader returns one path-specific invalid-request failure.</summary>
    /// <param name="recordTypeName">The exact generated concrete type name.</param>
    /// <param name="root">The complete invalid wire object.</param>
    /// <param name="messageFragment">The stable diagnostic fragment.</param>
    /// <param name="limits">The optional custom read limits.</param>
    private static void AssertInvalidRequest(
        string recordTypeName,
        JsonObject root,
        string messageFragment,
        RecordWireReadLimits? limits = null)
    {
        var result = GeneratedRecordWireTestSupport.Decode(recordTypeName, root.ToJsonString(), limits);

        result.Succeeded.ShouldBeFalse();
        result.Value.ShouldBeNull();
        result.Error.ShouldNotBeNull();
        result.Error!.Code.ShouldBe(EngineErrorCode.InvalidRequest);
        result.Error.Message.ShouldStartWith("$");
        result.Error.Message.ShouldContain(messageFragment);
    }

    /// <summary>Asserts a generated reader returns invalid-request failure at one exact property path.</summary>
    /// <param name="recordTypeName">The exact generated concrete plugin type name.</param>
    /// <param name="root">The complete invalid wire object.</param>
    /// <param name="expectedPath">The exact property path that must own the diagnostic.</param>
    /// <param name="messageFragment">An optional stable diagnostic fragment.</param>
    private static void AssertInvalidRequestAtPath(
        string recordTypeName,
        JsonObject root,
        string expectedPath,
        string? messageFragment = null)
    {
        var result = GeneratedRecordWireTestSupport.Decode(recordTypeName, root.ToJsonString());

        result.Succeeded.ShouldBeFalse();
        result.Value.ShouldBeNull();
        result.Error.ShouldNotBeNull();
        result.Error!.Code.ShouldBe(EngineErrorCode.InvalidRequest);
        result.Error.Message.ShouldStartWith(expectedPath + ":");
        if (messageFragment is not null)
        {
            result.Error.Message.ShouldContain(messageFragment);
        }
    }
}
