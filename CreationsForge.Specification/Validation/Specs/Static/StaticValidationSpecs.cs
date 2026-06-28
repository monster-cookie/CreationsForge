using CreationsForge.Specification.Records;
using CreationsForge.Specification.Validation;

namespace CreationsForge.Specification.Validation.Specs.Static;

public static class StaticValidationSpecs
{
    public static ValidationSpec Starfield_OpiExtPodAirlock01()
    {
        var spec = StarfieldStatic("OpiExtPodAirlock01", "0514C6:Starfield.esm", withSnapTemplate: true);
        spec.UiComparisonExpectations.Add(ValidationUiComparisonExpectation.Literal(
            ["SnapTemplate"],
            "Starfield.esm:002CBD15"));
        return spec;
    }

    /// <summary>
    /// Builds the Starfield <c>OpmIntPodSmSide01</c> static validation spec, including a UI model row expectation.
    /// </summary>
    /// <returns>The validation spec for the Starfield <c>OpmIntPodSmSide01</c> sample.</returns>
    public static ValidationSpec Starfield_OpmIntPodSmSide01()
    {
        var spec = StarfieldStatic("OpmIntPodSmSide01", "036311:Starfield.esm", withSnapTemplate: false);
        spec.UiComparisonExpectations.Add(new ValidationUiComparisonExpectation(["Model", "File"], visualText: "EditorID"));
        return spec;
    }

    public static ValidationSpec Starfield_OpmIntPodSmSideWin01()
    {
        return StarfieldStatic("OpmIntPodSmSideWin01", "042AE4:Starfield.esm", withSnapTemplate: false);
    }

    /// <summary>
    /// Builds the Starfield <c>CatIndWalkSm2WayB01</c> static validation spec, including a UI object-bounds row expectation.
    /// </summary>
    /// <returns>The validation spec for the Starfield <c>CatIndWalkSm2WayB01</c> sample.</returns>
    public static ValidationSpec Starfield_CatIndWalkSm2WayB01()
    {
        var spec = StarfieldStatic("CatIndWalkSm2WayB01", "03A1B4:Starfield.esm", withSnapTemplate: false);
        spec.UiComparisonExpectations.Add(ValidationUiComparisonExpectation.DtoField(["ObjectBoundsFirst"], "ObjectBoundsFirst"));
        return spec;
    }

    public static ValidationSpec Starfield_OpiExtPodAirlockStairs01()
    {
        return StarfieldStatic("OpiExtPodAirlockStairs01", "04F391:Starfield.esm", withSnapTemplate: false);
    }

    public static ValidationSpec Fallout4_workshop_JunkWallDoor01()
    {
        return Fallout4Static("workshop_JunkWallDoor01", "1B4AC0:Fallout4.esm");
    }

    public static ValidationSpec Fallout4_workshop_JunkWallDoor01A()
    {
        return Fallout4Static("workshop_JunkWallDoor01A", "1B4AC1:Fallout4.esm");
    }

    public static ValidationSpec Fallout4_workshop_ShackBalconyStairs01()
    {
        return Fallout4Static("workshop_ShackBalconyStairs01", "0EC532:Fallout4.esm");
    }

    public static ValidationSpec Fallout4_COCMarkerHeading()
    {
        return Fallout4Static("COCMarkerHeading", "000032:Fallout4.esm");
    }

    public static ValidationSpec Fallout4_CollisionMarker()
    {
        return Fallout4Static("CollisionMarker", "000021:Fallout4.esm", withObjectBounds: false);
    }

    public static ValidationSpec Skyrim_BlackreachECeiling01_GlowLichen()
    {
        return SkyrimStatic("BlackreachECeiling01_GlowLichen", "0D19F9:Skyrim.esm");
    }

    public static ValidationSpec Skyrim_DweFacadeTowerSpacer01Snow()
    {
        return SkyrimStatic("DweFacadeTowerSpacer01Snow", "06DD69:Skyrim.esm");
    }

    public static ValidationSpec Skyrim_HHMountainRidge01()
    {
        return SkyrimStatic("HHMountainRidge01", "090E82:Skyrim.esm");
    }

    public static ValidationSpec Skyrim_CaveGRockPileS01IceBlend()
    {
        return SkyrimStatic("CaveGRockPileS01IceBlend", "0946B2:Skyrim.esm");
    }

    public static ValidationSpec Skyrim_XMarkerSnow()
    {
        return SkyrimStatic("XMarkerSnow", "078DC0:Skyrim.esm");
    }

    private static ValidationSpec StarfieldStatic(
        string sampleName,
        string formKey,
        bool withSnapTemplate)
    {
        var spec = BaseStatic(SpecificationGame.Starfield, sampleName, formKey, withObjectBounds: true)
            .AddRule(ValidationFieldRule.Field("Model.File", "Models[0].File", ValidationValueNormalizer.ModelFile))
            .AddRule(ValidationFieldRule.Field("Model.LightLayer", "Models[0].LightLayer"));

        if (withSnapTemplate)
        {
            spec.AddRule(ValidationFieldRule.Field("SnapTemplate", "SnapTemplate"));
        }

        if (sampleName is "OpiExtPodAirlock01" or "OpiExtPodAirlockStairs01")
        {
            spec.AddRules(ValidationFieldRule.ComponentReflection(0, 0, 1, "LodOwnerComponent"));
        }

        return spec.Build();
    }

    private static ValidationSpec Fallout4Static(string sampleName, string formKey, bool withObjectBounds = true)
    {
        return BaseStatic(SpecificationGame.Fallout4, sampleName, formKey, withObjectBounds)
            .AddRule(ValidationFieldRule.Field("Model.File", "Models[0].File", ValidationValueNormalizer.ModelFile))
            .AddRule(ValidationFieldRule.Field("Model.Data", "Models[0].Data", ValidationValueNormalizer.HexPayload))
            .AddRule(ValidationFieldRule.DtoDefaultWhenSpriggitAbsent(
                "LeafAmplitude",
                "LeafAmplitude",
                "0",
                "Mutagen exposes default Fallout 4 leaf data when Spriggit omits the zero-valued field."))
            .AddRule(ValidationFieldRule.DtoDefaultWhenSpriggitAbsent(
                "LeafFrequency",
                "LeafFrequency",
                "0",
                "Mutagen exposes default Fallout 4 leaf data when Spriggit omits the zero-valued field."))
            .AddRules(PropertyRules())
            .Build();
    }

    private static ValidationSpec SkyrimStatic(string sampleName, string formKey)
    {
        return BaseStatic(SpecificationGame.Skyrim, sampleName, formKey, withObjectBounds: true)
            .AddRule(ValidationFieldRule.Field("Model.File", "Models[0].File", ValidationValueNormalizer.ModelFile))
            .AddRule(ValidationFieldRule.Field("Model.Data", "Models[0].Data", ValidationValueNormalizer.HexPayload))
            .AddRule(ValidationFieldRule.Field("Material", "Material"))
            .AddRule(ValidationFieldRule.Field("Lod.Level0", "LodLevel0", ValidationValueNormalizer.ModelFile))
            .AddRule(ValidationFieldRule.Field("Lod.Level1", "LodLevel1", ValidationValueNormalizer.ModelFile))
            .AddRule(ValidationFieldRule.Field("Lod.Level2", "LodLevel2", ValidationValueNormalizer.ModelFile))
            .AddRule(ValidationFieldRule.Field("Lod.Level3", "LodLevel3", ValidationValueNormalizer.ModelFile))
            .Build();
    }

    private static ValidationSpecBuilder BaseStatic(SpecificationGame game, string sampleName, string formKey, bool withObjectBounds)
    {
        var spec = ValidationSpecBuilder
            .ForRecord(game, SupportedRecordSpecifications.Static)
            .Sample(sampleName)
            .FormKey(formKey)
            .AddBaselineUiComparisonExpectations(
                new[] { "MajorRecordFlags" },
                new[] { "ObjectBoundsFirst" })
            .AddRule(ValidationFieldRule.TranslatedField("Name", "Name", requireAllLanguages: true))
            .AddRule(ValidationFieldRule.DtoDefaultWhenSpriggitAbsent(
                "ObjectBounds.First",
                "ObjectBoundsFirst",
                "0, 0, 0",
                "Mutagen exposes default object bounds when Spriggit omits the zero-valued field."))
            .AddRule(ValidationFieldRule.DtoDefaultWhenSpriggitAbsent(
                "ObjectBounds.Second",
                "ObjectBoundsSecond",
                "0, 0, 0",
                "Mutagen exposes default object bounds when Spriggit omits the zero-valued field."))
            .AddRule(ValidationFieldRule.FormKeyList("Keywords", "Keywords", "Keyword"))
            .AddRule(ValidationFieldRule.PathPrefix("NavmeshGeometry", "NavmeshGeometry", new Dictionary<string, string>(StringComparer.Ordinal)))
            .AddRules(NavmeshGeometryRules())
            .AddRule(ValidationFieldRule.ScalarList("DNAMDataTypeState", "DNAMDataTypeState"))
            .AddRule(ValidationFieldRule.DtoNonEmpty("MajorRecordFlagsRaw", "MajorRecordFlags"))
            .AddRule(ValidationFieldRule.DtoNonEmpty("MajorFlags", "MajorRecordFlags"))
            .AddRule(ValidationFieldRule.DtoNonEmpty("StarfieldMajorRecordFlags", "MajorRecordFlags"))
            .AddRule(ValidationFieldRule.DtoNonEmpty("Fallout4MajorRecordFlags", "MajorRecordFlags"))
            .AddRule(ValidationFieldRule.DtoNonEmpty("SkyrimMajorRecordFlags", "MajorRecordFlags"))
            .AddRule(ValidationFieldRule.IgnoreDtoPrefix("LocalizedStrings", "LocalizedStrings is the DTO projection of translated Spriggit fields."))
            .AddRule(ValidationFieldRule.Field("Version2", "Version2"))
            .AddRule(ValidationFieldRule.Field("VersionControl", "VersionControl"));

        for (var componentIndex = 0; componentIndex <= 5; componentIndex++)
        {
            var componentPath = "Components[" + componentIndex.ToString(System.Globalization.CultureInfo.InvariantCulture) + "]";
            spec
                .AddRule(ValidationFieldRule.FormKeyList(componentPath + ".Keywords", "Keywords", "Keyword"))
                .AddRule(ValidationFieldRule.IgnoreSpriggit(componentPath + ".MutagenObjectType", "Component type metadata is represented by typed child projections."));
        }

        spec.AddRule(ValidationFieldRule.IgnoreSpriggit("Components.Count", "Components are projected into typed keyword and reflection DTO children."));

        if (withObjectBounds)
        {
            spec
                .AddRule(ValidationFieldRule.Field("ObjectBounds.First", "ObjectBoundsFirst"))
                .AddRule(ValidationFieldRule.Field("ObjectBounds.Second", "ObjectBoundsSecond"));
        }

        return spec;
    }

    private static IEnumerable<ValidationFieldRule> PropertyRules()
    {
        for (var propertyIndex = 0; propertyIndex <= 10; propertyIndex++)
        {
            var propertyPath = "Properties[" + propertyIndex.ToString(System.Globalization.CultureInfo.InvariantCulture) + "]";
            yield return ValidationFieldRule.Field(propertyPath + ".ActorValue", propertyPath + ".ActorValue");
            yield return ValidationFieldRule.Field(propertyPath + ".Value", propertyPath + ".Value", ValidationValueNormalizer.DecimalNumber);
            yield return ValidationFieldRule.IgnoreDto(
                propertyPath + ".PropertyIndex",
                "PropertyIndex is DTO collection metadata for repository read-back.");
        }
    }

    private static IEnumerable<ValidationFieldRule> NavmeshGeometryRules()
    {
        for (var coverIndex = 0; coverIndex <= 100; coverIndex++)
        {
            yield return ValidationFieldRule.IgnoreDto(
                "NavmeshGeometry.Cover[" + coverIndex.ToString(System.Globalization.CultureInfo.InvariantCulture) + "].CoverIndex",
                "CoverIndex is DTO collection metadata for repository read-back.");
        }

        for (var mappingIndex = 0; mappingIndex <= 100; mappingIndex++)
        {
            yield return ValidationFieldRule.IgnoreDto(
                "NavmeshGeometry.CoverTriangleMappings[" + mappingIndex.ToString(System.Globalization.CultureInfo.InvariantCulture) + "].MappingIndex",
                "MappingIndex is DTO collection metadata for repository read-back.");
        }

        for (var gridArrayIndex = 0; gridArrayIndex <= 25; gridArrayIndex++)
        {
            yield return ValidationFieldRule.IgnoreDto(
                "NavmeshGeometry.GridArrays[" + gridArrayIndex.ToString(System.Globalization.CultureInfo.InvariantCulture) + "].GridArrayIndex",
                "GridArrayIndex is DTO collection metadata for repository read-back.");
        }

        for (var triangleIndex = 0; triangleIndex <= 500; triangleIndex++)
        {
            var trianglePath = "NavmeshGeometry.Triangles[" + triangleIndex.ToString(System.Globalization.CultureInfo.InvariantCulture) + "]";
            yield return ValidationFieldRule.IgnoreDto(
                trianglePath + ".TriangleIndex",
                "TriangleIndex is DTO collection metadata for repository read-back.");
        }

        for (var vertexIndex = 0; vertexIndex <= 500; vertexIndex++)
        {
            yield return ValidationFieldRule.IgnoreDto(
                "NavmeshGeometry.Vertices[" + vertexIndex.ToString(System.Globalization.CultureInfo.InvariantCulture) + "].VertexIndex",
                "VertexIndex is DTO collection metadata for repository read-back.");
        }
    }
}
