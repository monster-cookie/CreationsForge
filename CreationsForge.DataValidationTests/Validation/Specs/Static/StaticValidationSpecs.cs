using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;

namespace CreationsForge.DataValidationTests.Validation.Specs.Static;

public static class StaticValidationSpecs
{
    public static ValidationSpec Starfield_OpiExtPodAirlock01()
    {
        return StarfieldStatic("OpiExtPodAirlock01", "0514C6:Starfield.esm", withRefl: true, withSnapTemplate: true);
    }

    public static ValidationSpec Starfield_OpmIntPodSmSide01()
    {
        return StarfieldStatic("OpmIntPodSmSide01", "036311:Starfield.esm", withRefl: false, withSnapTemplate: false);
    }

    public static ValidationSpec Starfield_OpmIntPodSmSideWin01()
    {
        return StarfieldStatic("OpmIntPodSmSideWin01", "042AE4:Starfield.esm", withRefl: false, withSnapTemplate: false);
    }

    public static ValidationSpec Starfield_CatIndWalkSm2WayB01()
    {
        return StarfieldStatic("CatIndWalkSm2WayB01", "03A1B4:Starfield.esm", withRefl: false, withSnapTemplate: false);
    }

    public static ValidationSpec Starfield_OpiExtPodAirlockStairs01()
    {
        return StarfieldStatic("OpiExtPodAirlockStairs01", "04F391:Starfield.esm", withRefl: true, withSnapTemplate: false);
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
        bool withRefl,
        bool withSnapTemplate)
    {
        var spec = BaseStatic(SupportedGame.Starfield, sampleName, formKey, withObjectBounds: true)
            .AddRule(ValidationFieldRule.Field("Model.File", "Models[0].File", ValidationValueNormalizer.ModelFile))
            .AddRule(ValidationFieldRule.Field("Model.LightLayer", "Models[0].LightLayer"));

        if (withRefl)
        {
            spec.AddRule(ValidationFieldRule.RawPayloadSlot("Components[0].REFL", "BaseFormComponents.REFL"));
        }

        if (withSnapTemplate)
        {
            spec.AddRule(ValidationFieldRule.Field("SnapTemplate", "SnapTemplate"));
        }

        return spec.Build();
    }

    private static ValidationSpec Fallout4Static(string sampleName, string formKey, bool withObjectBounds = true)
    {
        return BaseStatic(SupportedGame.Fallout4, sampleName, formKey, withObjectBounds)
            .AddRule(ValidationFieldRule.Field("Model.File", "Models[0].File", ValidationValueNormalizer.ModelFile))
            .AddRule(ValidationFieldRule.RawPayloadSlot("Model.Data", "Model.Data"))
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
        return BaseStatic(SupportedGame.Skyrim, sampleName, formKey, withObjectBounds: true)
            .AddRule(ValidationFieldRule.Field("Model.File", "Models[0].File", ValidationValueNormalizer.ModelFile))
            .AddRule(ValidationFieldRule.RawPayloadSlot("Model.Data", "Model.Data"))
            .AddRule(ValidationFieldRule.Field("Material", "Material"))
            .AddRule(ValidationFieldRule.Field("Lod.Level0", "LodLevel0", ValidationValueNormalizer.ModelFile))
            .AddRule(ValidationFieldRule.Field("Lod.Level1", "LodLevel1", ValidationValueNormalizer.ModelFile))
            .AddRule(ValidationFieldRule.Field("Lod.Level2", "LodLevel2", ValidationValueNormalizer.ModelFile))
            .AddRule(ValidationFieldRule.Field("Lod.Level3", "LodLevel3", ValidationValueNormalizer.ModelFile))
            .Build();
    }

    private static ValidationSpecBuilder BaseStatic(SupportedGame game, string sampleName, string formKey, bool withObjectBounds)
    {
        var spec = ValidationSpecBuilder
            .ForRecord(game, RecordTypeCatalog.Static)
            .Sample(sampleName)
            .FormKey(formKey)
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
            .AddRule(ValidationFieldRule.RawPayloadSlot("NavmeshGeometry", "NavmeshGeometry"))
            .AddRule(ValidationFieldRule.ScalarList("DNAMDataTypeState", "DNAMDataTypeState"))
            .AddRule(ValidationFieldRule.DtoNonEmpty("MajorRecordFlagsRaw", "MajorRecordFlags"))
            .AddRule(ValidationFieldRule.DtoNonEmpty("MajorFlags", "MajorRecordFlags"))
            .AddRule(ValidationFieldRule.DtoNonEmpty("StarfieldMajorRecordFlags", "MajorRecordFlags"))
            .AddRule(ValidationFieldRule.DtoNonEmpty("Fallout4MajorRecordFlags", "MajorRecordFlags"))
            .AddRule(ValidationFieldRule.DtoNonEmpty("SkyrimMajorRecordFlags", "MajorRecordFlags"))
            .AddRule(ValidationFieldRule.IgnoreDtoPrefix("LocalizedStrings", "LocalizedStrings is the DTO projection of translated Spriggit fields."))
            .AddRule(ValidationFieldRule.IgnoreSpriggit("Version2", "Version2 is common header metadata outside current repository read-back for this record set."))
            .AddRule(ValidationFieldRule.IgnoreDto("Version2", "Version2 is common header metadata outside current repository read-back for this record set."))
            .AddRule(ValidationFieldRule.IgnoreSpriggit("VersionControl", "VersionControl is common header metadata outside current repository read-back for this record set."))
            .AddRule(ValidationFieldRule.IgnoreDto("VersionControl", "VersionControl is common header metadata outside current repository read-back for this record set."));

        for (var componentIndex = 0; componentIndex <= 5; componentIndex++)
        {
            var componentPath = "Components[" + componentIndex.ToString(System.Globalization.CultureInfo.InvariantCulture) + "]";
            spec
                .AddRule(ValidationFieldRule.FormKeyList(componentPath + ".Keywords", "Keywords", "Keyword"))
                .AddRule(ValidationFieldRule.IgnoreSpriggit(componentPath + ".MutagenObjectType", "Component type metadata is represented by typed child projections."));
        }

        spec.AddRule(ValidationFieldRule.IgnoreSpriggit("Components.Count", "Components are projected into typed keyword/raw-payload DTO children."));

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
}
