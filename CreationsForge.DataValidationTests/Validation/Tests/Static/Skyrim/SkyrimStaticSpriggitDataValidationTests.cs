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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["Lod.Level0"].ShouldBe(dtoFields["Lod.Level0"]);
        spriggitFields["Lod.Level1"].ShouldBe(dtoFields["Lod.Level1"]);
        spriggitFields["Lod.Level2"].ShouldBe(dtoFields["Lod.Level2"]);
        spriggitFields["Lod.Level3"].ShouldBe(dtoFields["Lod.Level3"]);
        spriggitFields["MajorRecordFlagsRaw"].ShouldBe(dtoFields["MajorRecordFlags"]);
        spriggitFields["Material"].ShouldBe(dtoFields["Material"]);
        spriggitFields["MaxAngle"].ShouldBe(dtoFields["MaxAngle"]);
        spriggitFields["Model.Data"].ShouldBe(dtoFields["Model.Data"]);
        spriggitFields["Model.File"].ShouldBe(dtoFields["Models[0].File"]);
        spriggitFields["ObjectBounds.First"].ShouldBe(dtoFields["ObjectBoundsFirst"]);
        spriggitFields["ObjectBounds.Second"].ShouldBe(dtoFields["ObjectBoundsSecond"]);
        spriggitFields["Unused"].ShouldBe(dtoFields["Unused"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["Lod.Level0"].ShouldBe(dtoFields["Lod.Level0"]);
        spriggitFields["Lod.Level1"].ShouldBe(dtoFields["Lod.Level1"]);
        spriggitFields["Lod.Level2"].ShouldBe(dtoFields["Lod.Level2"]);
        spriggitFields["Lod.Level3"].ShouldBe(dtoFields["Lod.Level3"]);
        spriggitFields["MajorRecordFlagsRaw"].ShouldBe(dtoFields["MajorRecordFlags"]);
        spriggitFields["Material"].ShouldBe(dtoFields["Material"]);
        spriggitFields["MaxAngle"].ShouldBe(dtoFields["MaxAngle"]);
        spriggitFields["Model.Data"].ShouldBe(dtoFields["Model.Data"]);
        spriggitFields["Model.File"].ShouldBe(dtoFields["Models[0].File"]);
        spriggitFields["ObjectBounds.First"].ShouldBe(dtoFields["ObjectBoundsFirst"]);
        spriggitFields["ObjectBounds.Second"].ShouldBe(dtoFields["ObjectBoundsSecond"]);
        spriggitFields["Unused"].ShouldBe(dtoFields["Unused"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["Lod.Level0"].ShouldBe(dtoFields["Lod.Level0"]);
        spriggitFields["Lod.Level1"].ShouldBe(dtoFields["Lod.Level1"]);
        spriggitFields["Lod.Level2"].ShouldBe(dtoFields["Lod.Level2"]);
        spriggitFields["Lod.Level3"].ShouldBe(dtoFields["Lod.Level3"]);
        spriggitFields["MajorRecordFlagsRaw"].ShouldBe(dtoFields["MajorRecordFlags"]);
        spriggitFields["Material"].ShouldBe(dtoFields["Material"]);
        spriggitFields["MaxAngle"].ShouldBe(dtoFields["MaxAngle"]);
        spriggitFields["Model.Data"].ShouldBe(dtoFields["Model.Data"]);
        spriggitFields["Model.File"].ShouldBe(dtoFields["Models[0].File"]);
        spriggitFields["ObjectBounds.First"].ShouldBe(dtoFields["ObjectBoundsFirst"]);
        spriggitFields["ObjectBounds.Second"].ShouldBe(dtoFields["ObjectBoundsSecond"]);
        spriggitFields["Unused"].ShouldBe(dtoFields["Unused"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["MajorRecordFlagsRaw"].ShouldBe(dtoFields["MajorRecordFlags"]);
        spriggitFields["Material"].ShouldBe(dtoFields["Material"]);
        spriggitFields["Model.Data"].ShouldBe(dtoFields["Model.Data"]);
        spriggitFields["Model.File"].ShouldBe(dtoFields["Models[0].File"]);
        spriggitFields["ObjectBounds.First"].ShouldBe(dtoFields["ObjectBoundsFirst"]);
        spriggitFields["ObjectBounds.Second"].ShouldBe(dtoFields["ObjectBoundsSecond"]);
        spriggitFields["Unused"].ShouldBe(dtoFields["Unused"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["MajorRecordFlagsRaw"].ShouldBe(dtoFields["MajorRecordFlags"]);
        spriggitFields["Material"].ShouldBe(dtoFields["Material"]);
        spriggitFields["MaxAngle"].ShouldBe(dtoFields["MaxAngle"]);
        spriggitFields["Model.File"].ShouldBe(dtoFields["Models[0].File"]);
        spriggitFields["ObjectBounds.First"].ShouldBe(dtoFields["ObjectBoundsFirst"]);
        spriggitFields["ObjectBounds.Second"].ShouldBe(dtoFields["ObjectBoundsSecond"]);
        spriggitFields["Unused"].ShouldBe(dtoFields["Unused"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }
}
