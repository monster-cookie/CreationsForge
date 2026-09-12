using System.Drawing;
using System.Text.Json.Nodes;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Starfield;
using Mutagen.Bethesda.Starfield.Assets;
using Noggog;
using Shouldly;

namespace CreationsForge.UnitTests.Engine.Starfield.NativeInspection.Nested;

/// <summary>Exercises representative nondefault values across every generated recursive native leaf family.</summary>
public sealed class GeneratedNativeWireLeafRoundTripTests
{
    /// <summary>Verifies nondefault scalar, string, enum, collection, and byte-memory values round-trip exactly.</summary>
    [Fact]
    public void PrimitiveCollectionAndMemoryLeaves_RoundTripExactly()
    {
        const string nativeTypeName = "Mutagen.Bethesda.Starfield.BlockEditorMetaDataComponent";
        var root = GetDefaultObject(nativeTypeName);
        root["UnknownString1"] = "alpha";
        root["UnknownByte"] = byte.MaxValue;
        root["UnknownString2"] = "beta";
        root["UnknownInts"] = new JsonArray(-2, 0, 17, int.MaxValue);
        root["UnknownInt1"] = int.MinValue;
        root["UnknownEnding"] = GeneratedNativeWireTestSupport.Bytes(new byte[] { 0, 1, 2, 254, 255 });

        AssertExactRoundTrip(nativeTypeName, root);
    }

    /// <summary>Verifies color and vector leaf objects retain every exact redundant representation.</summary>
    [Fact]
    public void ColorAndVectorLeaves_RoundTripExactly()
    {
        const string colorType = "Mutagen.Bethesda.Starfield.BlueprintComponentBODSItem";
        var colors = GetDefaultObject(colorType);
        colors["Color1"] = ColorNode(Color.FromArgb(0x7F, 0x12, 0x34, 0x56));
        colors["Color2"] = ColorNode(Color.CornflowerBlue);
        colors["Color3"] = ColorNode(Color.FromName("CreationsForgeAccent"));
        AssertExactRoundTrip(colorType, colors);

        const string p3Type = "Mutagen.Bethesda.Starfield.BlueprintComponentItem";
        var p3 = GetDefaultObject(p3Type);
        p3["Position"] = P3Float(-0.0f, 1.25f, float.PositiveInfinity);
        p3["Rotation"] = P3Float(float.NaN, -2.5f, 4096.0f);
        AssertExactRoundTrip(p3Type, p3);

        const string p2FloatType = "Mutagen.Bethesda.Starfield.UniqueOverlayListComponentItem";
        var p2Float = GetDefaultObject(p2FloatType);
        p2Float["Grid"] = P2Float(-12.5f, 0.125f);
        AssertExactRoundTrip(p2FloatType, p2Float);

        const string p2IntType = "Mutagen.Bethesda.Starfield.OverlayDesignatedPlacementInfoItem";
        var p2Int = GetDefaultObject(p2IntType);
        p2Int["Point"] = new JsonObject { ["X"] = int.MinValue, ["Y"] = int.MaxValue };
        AssertExactRoundTrip(p2IntType, p2Int);
    }

    /// <summary>Verifies exact doubles, ordinary links, translated strings, and nested two-dimensional arrays round-trip.</summary>
    [Fact]
    public void CompositeLeaves_RoundTripExactly()
    {
        const string doubleType = "Mutagen.Bethesda.Starfield.UniquePatternPlacementInfoComponent";
        var doubles = GetDefaultObject(doubleType);
        doubles["Longitude"] = GeneratedNativeWireTestSupport.Double(-123.456789012345);
        doubles["Latitude"] = GeneratedNativeWireTestSupport.Double(BitConverter.UInt64BitsToDouble(1UL << 63));
        AssertExactRoundTrip(doubleType, doubles);

        const string translatedType = "Mutagen.Bethesda.Starfield.Activity";
        var translated = GetDefaultObject(translatedType);
        translated["Description"] = TranslatedStringNode(
            translated["Description"]!.AsObject(),
            "Inspection",
            "Inspection française");
        translated["Name"] = TranslatedStringNode(
            translated["Description"]!.AsObject(),
            "Activity",
            "Activité");
        AssertExactRoundTrip(translatedType, translated);

        const string itemType = "Mutagen.Bethesda.Starfield.BlockHeightAdjustmentComponentItem";
        var firstItem = GetDefaultObject(itemType);
        firstItem["TerrainHeight"] = GeneratedNativeWireTestSupport.Single(-0.0f);
        firstItem["WaterHeight"] = GeneratedNativeWireTestSupport.Single(17.5f);
        var secondItem = GetDefaultObject(itemType);
        secondItem["TerrainHeight"] = GeneratedNativeWireTestSupport.Single(float.NegativeInfinity);
        secondItem["WaterHeight"] = GeneratedNativeWireTestSupport.Single(float.NaN);

        const string arrayType = "Mutagen.Bethesda.Starfield.BlockHeightAdjustmentComponent";
        var array = GetDefaultObject(arrayType);
        array["SurfaceBlocks"] = new JsonObject
        {
            ["width"] = 2,
            ["height"] = 1,
            ["rows"] = new JsonArray(new JsonArray(firstItem, secondItem)),
        };
        array["DATA"] = GeneratedNativeWireTestSupport.Bytes(new byte[] { 3, 1, 4, 1, 5 });
        AssertExactRoundTrip(arrayType, array);
    }

    /// <summary>Verifies a nondefault typed asset path and nullable byte memory survive native construction.</summary>
    [Fact]
    public void AssetLinkAndNullableMemory_RoundTripExactly()
    {
        const string nativeTypeName = "Mutagen.Bethesda.Starfield.Model";
        var model = new Model
        {
            File = new AssetLink<StarfieldModelAssetType>("Meshes\\CreationsForge\\Inspection.nif"),
            TextureFileHashes = new MemorySlice<byte>(new byte[] { 8, 6, 7, 5, 3, 0, 9 }),
            LightLayer = uint.MaxValue,
            Flags = Model.Flag.HasFaceBonesModel,
            ColorRemappingIndex = -0.0f,
            FlagsVestigial = Model.Flag.HasFirstPersonModel,
        };
        var written = GeneratedNativeWireTestSupport.Write(nativeTypeName, model);

        GeneratedNativeWireTestSupport.RoundTrip(nativeTypeName, written).ShouldBe(written);

        var compact = GeneratedNativeWireTestSupport.ParseObject(written);
        var assetValue = compact["File"]!.AsObject()["value"]!.AsObject();
        assetValue.Remove("dataRelativePath").ShouldBeTrue();
        assetValue.Remove("extension").ShouldBeTrue();
        GeneratedNativeWireTestSupport.RoundTrip(nativeTypeName, compact.ToJsonString()).ShouldBe(written);
    }

    /// <summary>Verifies an empty native asset path expands omitted read-only projections without losing its null state.</summary>
    [Fact]
    public void EmptyAssetLink_WithOmittedDerivedPaths_ExpandsExactly()
    {
        const string nativeTypeName = "Mutagen.Bethesda.Starfield.Model";
        var model = new Model
        {
            File = new AssetLink<StarfieldModelAssetType>(string.Empty),
        };
        var written = GeneratedNativeWireTestSupport.Write(nativeTypeName, model);
        var compact = GeneratedNativeWireTestSupport.ParseObject(written);
        var assetValue = compact["File"]!.AsObject()["value"]!.AsObject();
        assetValue["isNull"]!.GetValue<bool>().ShouldBeTrue();
        assetValue["givenPath"]!.GetValue<string>().ShouldBeEmpty();
        assetValue.Remove("dataRelativePath").ShouldBeTrue();
        assetValue.Remove("extension").ShouldBeTrue();

        GeneratedNativeWireTestSupport.RoundTrip(nativeTypeName, compact.ToJsonString()).ShouldBe(written);
    }

    /// <summary>Loads the actual installed default for one generated concrete type.</summary>
    /// <param name="nativeTypeName">The exact generated concrete type name.</param>
    /// <returns>A mutable default wire object.</returns>
    private static JsonObject GetDefaultObject(string nativeTypeName)
    {
        var json = GeneratedNativeWireTestSupport.CreateDefaultJson(nativeTypeName);
        json.ShouldNotBeNull();
        return GeneratedNativeWireTestSupport.ParseObject(json);
    }

    /// <summary>Asserts one nondefault wire object decodes and rewrites without any JSON drift.</summary>
    /// <param name="nativeTypeName">The exact generated concrete type name.</param>
    /// <param name="root">The complete mutable wire object.</param>
    private static void AssertExactRoundTrip(string nativeTypeName, JsonObject root)
    {
        var json = root.ToJsonString();
        GeneratedNativeWireTestSupport.RoundTrip(nativeTypeName, json).ShouldBe(json);
    }

    /// <summary>Creates the complete reconstructible wire representation of one native color.</summary>
    /// <param name="value">The native color.</param>
    /// <returns>The complete redundant color object.</returns>
    private static JsonObject ColorNode(Color value)
    {
        return new JsonObject
        {
            ["argb"] = value.ToArgb(),
            ["a"] = value.A,
            ["r"] = value.R,
            ["g"] = value.G,
            ["b"] = value.B,
            ["isEmpty"] = value.IsEmpty,
            ["isKnownColor"] = value.IsKnownColor,
            ["isNamedColor"] = value.IsNamedColor,
            ["isSystemColor"] = value.IsSystemColor,
            ["name"] = value.Name,
        };
    }

    /// <summary>Creates one exact two-dimensional floating-point vector wire value.</summary>
    /// <param name="x">The exact X component.</param>
    /// <param name="y">The exact Y component.</param>
    /// <returns>The complete vector object.</returns>
    private static JsonObject P2Float(float x, float y)
    {
        return new JsonObject
        {
            ["X"] = GeneratedNativeWireTestSupport.Single(x),
            ["Y"] = GeneratedNativeWireTestSupport.Single(y),
        };
    }

    /// <summary>Creates one exact three-dimensional floating-point vector wire value.</summary>
    /// <param name="x">The exact X component.</param>
    /// <param name="y">The exact Y component.</param>
    /// <param name="z">The exact Z component.</param>
    /// <returns>The complete vector object.</returns>
    private static JsonObject P3Float(float x, float y, float z)
    {
        return new JsonObject
        {
            ["X"] = GeneratedNativeWireTestSupport.Single(x),
            ["Y"] = GeneratedNativeWireTestSupport.Single(y),
            ["Z"] = GeneratedNativeWireTestSupport.Single(z),
        };
    }

    /// <summary>Creates a nondefault translated-string wire value while retaining its generated wrapper discriminator.</summary>
    /// <param name="template">A generated translated-string wrapper supplying the exact discriminator.</param>
    /// <param name="english">The selected English value.</param>
    /// <param name="french">The retained French translation.</param>
    /// <returns>The complete translated-string wrapper.</returns>
    private static JsonObject TranslatedStringNode(JsonObject template, string english, string french)
    {
        var result = (JsonObject)template.DeepClone();
        result["value"] = new JsonObject
        {
            ["targetLanguage"] = "English",
            ["value"] = english,
            ["translations"] = new JsonArray(
                new JsonObject { ["language"] = "English", ["value"] = english },
                new JsonObject { ["language"] = "French", ["value"] = french }),
        };
        return result;
    }
}
