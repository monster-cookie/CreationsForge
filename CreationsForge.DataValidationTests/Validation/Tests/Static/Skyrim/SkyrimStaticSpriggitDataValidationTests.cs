using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.Static.Skyrim;

public class SkyrimStaticSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "STAT")]
    [Trait("FormKey", "0D19F9:Skyrim.esm")]
    [Trait("EditorID", "BlackreachECeiling01_GlowLichen")]
    [Trait("SpriggitFile", "Statics/BlackreachECeiling01_GlowLichen - 0D19F9_Skyrim.esm.yaml")]
    public void Skyrim_STAT_ShouldMatchSpriggitSample_BlackreachECeiling01_GlowLichen()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Static,
            "BlackreachECeiling01_GlowLichen");
        var dto = Helpers.GetDTO<StaticDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Static,
            "0D19F9:Skyrim.esm");

        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "Lod.Level0").ShouldBe(Helpers.GetDTOField(dto, "Lod.Level0"));
        Helpers.GetSpriggitField(spriggit, "Lod.Level1").ShouldBe(Helpers.GetDTOField(dto, "Lod.Level1"));
        Helpers.GetSpriggitField(spriggit, "Lod.Level2").ShouldBe(Helpers.GetDTOField(dto, "Lod.Level2"));
        Helpers.GetSpriggitField(spriggit, "Lod.Level3").ShouldBe(Helpers.GetDTOField(dto, "Lod.Level3"));
        Helpers.GetSpriggitField(spriggit, "MajorRecordFlagsRaw").ShouldBe(Helpers.GetDTOField(dto, "MajorRecordFlags"));
        Helpers.GetSpriggitField(spriggit, "Material").ShouldBe(Helpers.GetDTOField(dto, "Material"));
        Helpers.GetSpriggitField(spriggit, "MaxAngle").ShouldBe(Helpers.GetDTOField(dto, "MaxAngle"));
        Helpers.GetSpriggitField(spriggit, "Model.Data").ShouldBe(Helpers.GetDTOField(dto, "Model.Data"));
        Helpers.GetSpriggitField(spriggit, "Model.File").ShouldBe(Helpers.GetDTOField(dto, "Models[0].File"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.First").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsFirst"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.Second").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsSecond"));
        Helpers.GetSpriggitField(spriggit, "Unused").ShouldBe(Helpers.GetDTOField(dto, "Unused"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "EditorID", "FormKey", "FormVersion", "Lod.Level0", "Lod.Level1", "Lod.Level2", "Lod.Level3", "MajorRecordFlagsRaw", "Material", "MaxAngle", "Model.Data", "Model.File", "ObjectBounds.First", "ObjectBounds.Second", "Unused", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "EditorID", "FormKey", "FormVersion", "Lod.Level0", "Lod.Level1", "Lod.Level2", "Lod.Level3", "MajorRecordFlags", "Material", "MaxAngle", "Model.Data", "Models[0].File", "ObjectBoundsFirst", "ObjectBoundsSecond", "Unused", "Version2", "VersionControl");
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "STAT")]
    [Trait("FormKey", "06DD69:Skyrim.esm")]
    [Trait("EditorID", "DweFacadeTowerSpacer01Snow")]
    [Trait("SpriggitFile", "Statics/DweFacadeTowerSpacer01Snow - 06DD69_Skyrim.esm.yaml")]
    public void Skyrim_STAT_ShouldMatchSpriggitSample_DweFacadeTowerSpacer01Snow()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Static,
            "DweFacadeTowerSpacer01Snow");
        var dto = Helpers.GetDTO<StaticDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Static,
            "06DD69:Skyrim.esm");

        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "Lod.Level0").ShouldBe(Helpers.GetDTOField(dto, "Lod.Level0"));
        Helpers.GetSpriggitField(spriggit, "Lod.Level1").ShouldBe(Helpers.GetDTOField(dto, "Lod.Level1"));
        Helpers.GetSpriggitField(spriggit, "Lod.Level2").ShouldBe(Helpers.GetDTOField(dto, "Lod.Level2"));
        Helpers.GetSpriggitField(spriggit, "Lod.Level3").ShouldBe(Helpers.GetDTOField(dto, "Lod.Level3"));
        Helpers.GetSpriggitField(spriggit, "MajorRecordFlagsRaw").ShouldBe(Helpers.GetDTOField(dto, "MajorRecordFlags"));
        Helpers.GetSpriggitField(spriggit, "Material").ShouldBe(Helpers.GetDTOField(dto, "Material"));
        Helpers.GetSpriggitField(spriggit, "MaxAngle").ShouldBe(Helpers.GetDTOField(dto, "MaxAngle"));
        Helpers.GetSpriggitField(spriggit, "Model.Data").ShouldBe(Helpers.GetDTOField(dto, "Model.Data"));
        Helpers.GetSpriggitField(spriggit, "Model.File").ShouldBe(Helpers.GetDTOField(dto, "Models[0].File"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.First").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsFirst"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.Second").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsSecond"));
        Helpers.GetSpriggitField(spriggit, "Unused").ShouldBe(Helpers.GetDTOField(dto, "Unused"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "EditorID", "FormKey", "FormVersion", "Lod.Level0", "Lod.Level1", "Lod.Level2", "Lod.Level3", "MajorRecordFlagsRaw", "Material", "MaxAngle", "Model.Data", "Model.File", "ObjectBounds.First", "ObjectBounds.Second", "Unused", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "EditorID", "FormKey", "FormVersion", "Lod.Level0", "Lod.Level1", "Lod.Level2", "Lod.Level3", "MajorRecordFlags", "Material", "MaxAngle", "Model.Data", "Models[0].File", "ObjectBoundsFirst", "ObjectBoundsSecond", "Unused", "Version2", "VersionControl");
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "STAT")]
    [Trait("FormKey", "090E82:Skyrim.esm")]
    [Trait("EditorID", "HHMountainRidge01")]
    [Trait("SpriggitFile", "Statics/HHMountainRidge01 - 090E82_Skyrim.esm.yaml")]
    public void Skyrim_STAT_ShouldMatchSpriggitSample_HHMountainRidge01()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Static,
            "HHMountainRidge01");
        var dto = Helpers.GetDTO<StaticDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Static,
            "090E82:Skyrim.esm");

        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "Lod.Level0").ShouldBe(Helpers.GetDTOField(dto, "Lod.Level0"));
        Helpers.GetSpriggitField(spriggit, "Lod.Level1").ShouldBe(Helpers.GetDTOField(dto, "Lod.Level1"));
        Helpers.GetSpriggitField(spriggit, "Lod.Level2").ShouldBe(Helpers.GetDTOField(dto, "Lod.Level2"));
        Helpers.GetSpriggitField(spriggit, "Lod.Level3").ShouldBe(Helpers.GetDTOField(dto, "Lod.Level3"));
        Helpers.GetSpriggitField(spriggit, "MajorRecordFlagsRaw").ShouldBe(Helpers.GetDTOField(dto, "MajorRecordFlags"));
        Helpers.GetSpriggitField(spriggit, "Material").ShouldBe(Helpers.GetDTOField(dto, "Material"));
        Helpers.GetSpriggitField(spriggit, "MaxAngle").ShouldBe(Helpers.GetDTOField(dto, "MaxAngle"));
        Helpers.GetSpriggitField(spriggit, "Model.Data").ShouldBe(Helpers.GetDTOField(dto, "Model.Data"));
        Helpers.GetSpriggitField(spriggit, "Model.File").ShouldBe(Helpers.GetDTOField(dto, "Models[0].File"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.First").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsFirst"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.Second").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsSecond"));
        Helpers.GetSpriggitField(spriggit, "Unused").ShouldBe(Helpers.GetDTOField(dto, "Unused"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "EditorID", "FormKey", "FormVersion", "Lod.Level0", "Lod.Level1", "Lod.Level2", "Lod.Level3", "MajorRecordFlagsRaw", "Material", "MaxAngle", "Model.Data", "Model.File", "ObjectBounds.First", "ObjectBounds.Second", "Unused", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "EditorID", "FormKey", "FormVersion", "Lod.Level0", "Lod.Level1", "Lod.Level2", "Lod.Level3", "MajorRecordFlags", "Material", "MaxAngle", "Model.Data", "Models[0].File", "ObjectBoundsFirst", "ObjectBoundsSecond", "Unused", "Version2", "VersionControl");
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "STAT")]
    [Trait("FormKey", "0946B2:Skyrim.esm")]
    [Trait("EditorID", "CaveGRockPileS01IceBlend")]
    [Trait("SpriggitFile", "Statics/CaveGRockPileS01IceBlend - 0946B2_Skyrim.esm.yaml")]
    public void Skyrim_STAT_ShouldMatchSpriggitSample_CaveGRockPileS01IceBlend()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Static,
            "CaveGRockPileS01IceBlend");
        var dto = Helpers.GetDTO<StaticDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Static,
            "0946B2:Skyrim.esm");

        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "MajorRecordFlagsRaw").ShouldBe(Helpers.GetDTOField(dto, "MajorRecordFlags"));
        Helpers.GetSpriggitField(spriggit, "Material").ShouldBe(Helpers.GetDTOField(dto, "Material"));
        Helpers.GetSpriggitField(spriggit, "Model.Data").ShouldBe(Helpers.GetDTOField(dto, "Model.Data"));
        Helpers.GetSpriggitField(spriggit, "Model.File").ShouldBe(Helpers.GetDTOField(dto, "Models[0].File"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.First").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsFirst"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.Second").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsSecond"));
        Helpers.GetSpriggitField(spriggit, "Unused").ShouldBe(Helpers.GetDTOField(dto, "Unused"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "EditorID", "FormKey", "FormVersion", "MajorRecordFlagsRaw", "Material", "Model.Data", "Model.File", "ObjectBounds.First", "ObjectBounds.Second", "Unused", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "EditorID", "FormKey", "FormVersion", "MajorRecordFlags", "Material", "Model.Data", "Models[0].File", "ObjectBoundsFirst", "ObjectBoundsSecond", "Unused", "Version2", "VersionControl");
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "STAT")]
    [Trait("FormKey", "078DC0:Skyrim.esm")]
    [Trait("EditorID", "XMarkerSnow")]
    [Trait("SpriggitFile", "Statics/XMarkerSnow - 078DC0_Skyrim.esm.yaml")]
    public void Skyrim_STAT_ShouldMatchSpriggitSample_XMarkerSnow()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Static,
            "XMarkerSnow");
        var dto = Helpers.GetDTO<StaticDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Static,
            "078DC0:Skyrim.esm");

        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "MajorRecordFlagsRaw").ShouldBe(Helpers.GetDTOField(dto, "MajorRecordFlags"));
        Helpers.GetSpriggitField(spriggit, "Material").ShouldBe(Helpers.GetDTOField(dto, "Material"));
        Helpers.GetSpriggitField(spriggit, "MaxAngle").ShouldBe(Helpers.GetDTOField(dto, "MaxAngle"));
        Helpers.GetSpriggitField(spriggit, "Model.File").ShouldBe(Helpers.GetDTOField(dto, "Models[0].File"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.First").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsFirst"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.Second").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsSecond"));
        Helpers.GetSpriggitField(spriggit, "Unused").ShouldBe(Helpers.GetDTOField(dto, "Unused"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "EditorID", "FormKey", "FormVersion", "MajorRecordFlagsRaw", "Material", "MaxAngle", "Model.File", "ObjectBounds.First", "ObjectBounds.Second", "Unused", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "EditorID", "FormKey", "FormVersion", "MajorRecordFlags", "Material", "MaxAngle", "Models[0].File", "ObjectBoundsFirst", "ObjectBoundsSecond", "Unused", "Version2", "VersionControl");
    }
}