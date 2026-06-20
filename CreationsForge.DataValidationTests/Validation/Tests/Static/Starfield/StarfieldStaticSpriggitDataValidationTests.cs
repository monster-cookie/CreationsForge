using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.Static.Starfield;

public class StarfieldStaticSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "STAT")]
    [Trait("FormKey", "0514C6:Starfield.esm")]
    [Trait("EditorID", "OpiExtPodAirlock01")]
    [Trait("SpriggitFile", "Statics/OpiExtPodAirlock01 - 0514C6_Starfield.esm.yaml")]
    public void Starfield_STAT_ShouldMatchSpriggitSample_OpiExtPodAirlock01()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Static,
            "OpiExtPodAirlock01");
        var dto = Helpers.GetDTO<StaticDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Static,
            "0514C6:Starfield.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["MaxAngle"].ShouldBe(dtoFields["MaxAngle"]);
        spriggitFields["Model.File"].ShouldBe(dtoFields["Models[0].File"]);
        spriggitFields["Model.LightLayer"].ShouldBe(dtoFields["Models[0].LightLayer"]);
        spriggitFields["NavmeshGeometry.Count"].ShouldBe(dtoFields["NavmeshGeometry.Count"]);
        spriggitFields["NavmeshGeometry.GridMax"].ShouldBe(dtoFields["NavmeshGeometry.GridMax"]);
        spriggitFields["NavmeshGeometry.GridMaxDistance"].ShouldBe(dtoFields["NavmeshGeometry.GridMaxDistance"]);
        spriggitFields["NavmeshGeometry.GridMin"].ShouldBe(dtoFields["NavmeshGeometry.GridMin"]);
        spriggitFields["NavmeshGeometry.GridSize"].ShouldBe(dtoFields["NavmeshGeometry.GridSize"]);
        spriggitFields["NavmeshGeometry.Parent.MutagenObjectType"].ShouldBe(dtoFields["NavmeshGeometry.Parent.MutagenObjectType"]);
        spriggitFields["NavmeshGeometry.Parent.Parent"].ShouldBe(dtoFields["NavmeshGeometry.Parent.Parent"]);
        spriggitFields["NavmeshGeometry[0]"].ShouldBe(dtoFields["NavmeshGeometry[0]"]);
        spriggitFields["NavmeshGeometry[1].Point"].ShouldBe(dtoFields["NavmeshGeometry[1].Point"]);
        spriggitFields["NavmeshGeometry[10].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[10].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[10].EdgeLink_2_0"].ShouldBe(dtoFields["NavmeshGeometry[10].EdgeLink_2_0"]);
        spriggitFields["NavmeshGeometry[10].Height"].ShouldBe(dtoFields["NavmeshGeometry[10].Height"]);
        spriggitFields["NavmeshGeometry[10].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[10].Vertices"]);
        spriggitFields["NavmeshGeometry[11].EdgeLink_0_1"].ShouldBe(dtoFields["NavmeshGeometry[11].EdgeLink_0_1"]);
        spriggitFields["NavmeshGeometry[11].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[11].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[11].EdgeLink_2_0"].ShouldBe(dtoFields["NavmeshGeometry[11].EdgeLink_2_0"]);
        spriggitFields["NavmeshGeometry[11].Height"].ShouldBe(dtoFields["NavmeshGeometry[11].Height"]);
        spriggitFields["NavmeshGeometry[11].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[11].Vertices"]);
        spriggitFields["NavmeshGeometry[12].EdgeLink_0_1"].ShouldBe(dtoFields["NavmeshGeometry[12].EdgeLink_0_1"]);
        spriggitFields["NavmeshGeometry[12].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[12].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[12].EdgeLink_2_0"].ShouldBe(dtoFields["NavmeshGeometry[12].EdgeLink_2_0"]);
        spriggitFields["NavmeshGeometry[12].Height"].ShouldBe(dtoFields["NavmeshGeometry[12].Height"]);
        spriggitFields["NavmeshGeometry[12].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[12].Vertices"]);
        spriggitFields["NavmeshGeometry[13].EdgeLink_0_1"].ShouldBe(dtoFields["NavmeshGeometry[13].EdgeLink_0_1"]);
        spriggitFields["NavmeshGeometry[13].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[13].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[13].EdgeLink_2_0"].ShouldBe(dtoFields["NavmeshGeometry[13].EdgeLink_2_0"]);
        spriggitFields["NavmeshGeometry[13].Height"].ShouldBe(dtoFields["NavmeshGeometry[13].Height"]);
        spriggitFields["NavmeshGeometry[13].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[13].Vertices"]);
        spriggitFields["NavmeshGeometry[14].EdgeLink_0_1"].ShouldBe(dtoFields["NavmeshGeometry[14].EdgeLink_0_1"]);
        spriggitFields["NavmeshGeometry[14].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[14].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[14].EdgeLink_2_0"].ShouldBe(dtoFields["NavmeshGeometry[14].EdgeLink_2_0"]);
        spriggitFields["NavmeshGeometry[14].Height"].ShouldBe(dtoFields["NavmeshGeometry[14].Height"]);
        spriggitFields["NavmeshGeometry[14].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[14].Vertices"]);
        spriggitFields["NavmeshGeometry[15].GridCell.Count"].ShouldBe(dtoFields["NavmeshGeometry[15].GridCell.Count"]);
        spriggitFields["NavmeshGeometry[15].GridCell[0]"].ShouldBe(dtoFields["NavmeshGeometry[15].GridCell[0]"]);
        spriggitFields["NavmeshGeometry[15].GridCell[1]"].ShouldBe(dtoFields["NavmeshGeometry[15].GridCell[1]"]);
        spriggitFields["NavmeshGeometry[15].GridCell[2]"].ShouldBe(dtoFields["NavmeshGeometry[15].GridCell[2]"]);
        spriggitFields["NavmeshGeometry[15].GridCell[3]"].ShouldBe(dtoFields["NavmeshGeometry[15].GridCell[3]"]);
        spriggitFields["NavmeshGeometry[15].GridCell[4]"].ShouldBe(dtoFields["NavmeshGeometry[15].GridCell[4]"]);
        spriggitFields["NavmeshGeometry[15].GridCell[5]"].ShouldBe(dtoFields["NavmeshGeometry[15].GridCell[5]"]);
        spriggitFields["NavmeshGeometry[15].GridCell[6]"].ShouldBe(dtoFields["NavmeshGeometry[15].GridCell[6]"]);
        spriggitFields["NavmeshGeometry[15].GridCell[7]"].ShouldBe(dtoFields["NavmeshGeometry[15].GridCell[7]"]);
        spriggitFields["NavmeshGeometry[15].GridCell[8]"].ShouldBe(dtoFields["NavmeshGeometry[15].GridCell[8]"]);
        spriggitFields["NavmeshGeometry[15].GridCell[9]"].ShouldBe(dtoFields["NavmeshGeometry[15].GridCell[9]"]);
        spriggitFields["NavmeshGeometry[2].Point"].ShouldBe(dtoFields["NavmeshGeometry[2].Point"]);
        spriggitFields["NavmeshGeometry[3].Point"].ShouldBe(dtoFields["NavmeshGeometry[3].Point"]);
        spriggitFields["NavmeshGeometry[4].Point"].ShouldBe(dtoFields["NavmeshGeometry[4].Point"]);
        spriggitFields["NavmeshGeometry[5].Point"].ShouldBe(dtoFields["NavmeshGeometry[5].Point"]);
        spriggitFields["NavmeshGeometry[6].Point"].ShouldBe(dtoFields["NavmeshGeometry[6].Point"]);
        spriggitFields["NavmeshGeometry[7].Point"].ShouldBe(dtoFields["NavmeshGeometry[7].Point"]);
        spriggitFields["NavmeshGeometry[8].Point"].ShouldBe(dtoFields["NavmeshGeometry[8].Point"]);
        spriggitFields["NavmeshGeometry[9].EdgeLink_0_1"].ShouldBe(dtoFields["NavmeshGeometry[9].EdgeLink_0_1"]);
        spriggitFields["NavmeshGeometry[9].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[9].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[9].EdgeLink_2_0"].ShouldBe(dtoFields["NavmeshGeometry[9].EdgeLink_2_0"]);
        spriggitFields["NavmeshGeometry[9].Height"].ShouldBe(dtoFields["NavmeshGeometry[9].Height"]);
        spriggitFields["NavmeshGeometry[9].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[9].Vertices"]);
        spriggitFields["ObjectBounds.First"].ShouldBe(dtoFields["ObjectBoundsFirst"]);
        spriggitFields["ObjectBounds.Second"].ShouldBe(dtoFields["ObjectBoundsSecond"]);
        spriggitFields["REFL"].ShouldBe(dtoFields["REFL"]);
        spriggitFields["SnapTemplate"].ShouldBe(dtoFields["SnapTemplate"]);
        spriggitFields["UnknownDNAMFloat"].ShouldBe(dtoFields["UnknownDNAMFloat"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "STAT")]
    [Trait("FormKey", "036311:Starfield.esm")]
    [Trait("EditorID", "OpmIntPodSmSide01")]
    [Trait("SpriggitFile", "Statics/OpmIntPodSmSide01 - 036311_Starfield.esm.yaml")]
    public void Starfield_STAT_ShouldMatchSpriggitSample_OpmIntPodSmSide01()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Static,
            "OpmIntPodSmSide01");
        var dto = Helpers.GetDTO<StaticDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Static,
            "036311:Starfield.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["MaxAngle"].ShouldBe(dtoFields["MaxAngle"]);
        spriggitFields["Model.File"].ShouldBe(dtoFields["Models[0].File"]);
        spriggitFields["Model.LightLayer"].ShouldBe(dtoFields["Models[0].LightLayer"]);
        spriggitFields["NavmeshGeometry.Count"].ShouldBe(dtoFields["NavmeshGeometry.Count"]);
        spriggitFields["NavmeshGeometry.GridMax"].ShouldBe(dtoFields["NavmeshGeometry.GridMax"]);
        spriggitFields["NavmeshGeometry.GridMaxDistance"].ShouldBe(dtoFields["NavmeshGeometry.GridMaxDistance"]);
        spriggitFields["NavmeshGeometry.GridMin"].ShouldBe(dtoFields["NavmeshGeometry.GridMin"]);
        spriggitFields["NavmeshGeometry.GridSize"].ShouldBe(dtoFields["NavmeshGeometry.GridSize"]);
        spriggitFields["NavmeshGeometry.Parent.MutagenObjectType"].ShouldBe(dtoFields["NavmeshGeometry.Parent.MutagenObjectType"]);
        spriggitFields["NavmeshGeometry.Parent.Parent"].ShouldBe(dtoFields["NavmeshGeometry.Parent.Parent"]);
        spriggitFields["NavmeshGeometry[0]"].ShouldBe(dtoFields["NavmeshGeometry[0]"]);
        spriggitFields["NavmeshGeometry[1].Point"].ShouldBe(dtoFields["NavmeshGeometry[1].Point"]);
        spriggitFields["NavmeshGeometry[10].CoverFlags"].ShouldBe(dtoFields["NavmeshGeometry[10].CoverFlags"]);
        spriggitFields["NavmeshGeometry[10].EdgeLink_0_1"].ShouldBe(dtoFields["NavmeshGeometry[10].EdgeLink_0_1"]);
        spriggitFields["NavmeshGeometry[10].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[10].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[10].EdgeLink_2_0"].ShouldBe(dtoFields["NavmeshGeometry[10].EdgeLink_2_0"]);
        spriggitFields["NavmeshGeometry[10].Height"].ShouldBe(dtoFields["NavmeshGeometry[10].Height"]);
        spriggitFields["NavmeshGeometry[10].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[10].Vertices"]);
        spriggitFields["NavmeshGeometry[11].CoverFlags"].ShouldBe(dtoFields["NavmeshGeometry[11].CoverFlags"]);
        spriggitFields["NavmeshGeometry[11].EdgeLink_0_1"].ShouldBe(dtoFields["NavmeshGeometry[11].EdgeLink_0_1"]);
        spriggitFields["NavmeshGeometry[11].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[11].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[11].EdgeLink_2_0"].ShouldBe(dtoFields["NavmeshGeometry[11].EdgeLink_2_0"]);
        spriggitFields["NavmeshGeometry[11].Height"].ShouldBe(dtoFields["NavmeshGeometry[11].Height"]);
        spriggitFields["NavmeshGeometry[11].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[11].Vertices"]);
        spriggitFields["NavmeshGeometry[12].CoverFlags"].ShouldBe(dtoFields["NavmeshGeometry[12].CoverFlags"]);
        spriggitFields["NavmeshGeometry[12].EdgeLink_0_1"].ShouldBe(dtoFields["NavmeshGeometry[12].EdgeLink_0_1"]);
        spriggitFields["NavmeshGeometry[12].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[12].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[12].EdgeLink_2_0"].ShouldBe(dtoFields["NavmeshGeometry[12].EdgeLink_2_0"]);
        spriggitFields["NavmeshGeometry[12].Height"].ShouldBe(dtoFields["NavmeshGeometry[12].Height"]);
        spriggitFields["NavmeshGeometry[12].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[12].Vertices"]);
        spriggitFields["NavmeshGeometry[13].CoverFlags"].ShouldBe(dtoFields["NavmeshGeometry[13].CoverFlags"]);
        spriggitFields["NavmeshGeometry[13].EdgeLink_0_1"].ShouldBe(dtoFields["NavmeshGeometry[13].EdgeLink_0_1"]);
        spriggitFields["NavmeshGeometry[13].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[13].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[13].Height"].ShouldBe(dtoFields["NavmeshGeometry[13].Height"]);
        spriggitFields["NavmeshGeometry[13].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[13].Vertices"]);
        spriggitFields["NavmeshGeometry[14].CoverFlags"].ShouldBe(dtoFields["NavmeshGeometry[14].CoverFlags"]);
        spriggitFields["NavmeshGeometry[14].EdgeLink_0_1"].ShouldBe(dtoFields["NavmeshGeometry[14].EdgeLink_0_1"]);
        spriggitFields["NavmeshGeometry[14].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[14].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[14].EdgeLink_2_0"].ShouldBe(dtoFields["NavmeshGeometry[14].EdgeLink_2_0"]);
        spriggitFields["NavmeshGeometry[14].Height"].ShouldBe(dtoFields["NavmeshGeometry[14].Height"]);
        spriggitFields["NavmeshGeometry[14].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[14].Vertices"]);
        spriggitFields["NavmeshGeometry[15].Data"].ShouldBe(dtoFields["NavmeshGeometry[15].Data"]);
        spriggitFields["NavmeshGeometry[15].Vertex1"].ShouldBe(dtoFields["NavmeshGeometry[15].Vertex1"]);
        spriggitFields["NavmeshGeometry[15].Vertex2"].ShouldBe(dtoFields["NavmeshGeometry[15].Vertex2"]);
        spriggitFields["NavmeshGeometry[16].Data"].ShouldBe(dtoFields["NavmeshGeometry[16].Data"]);
        spriggitFields["NavmeshGeometry[16].Vertex1"].ShouldBe(dtoFields["NavmeshGeometry[16].Vertex1"]);
        spriggitFields["NavmeshGeometry[16].Vertex2"].ShouldBe(dtoFields["NavmeshGeometry[16].Vertex2"]);
        spriggitFields["NavmeshGeometry[17].Data"].ShouldBe(dtoFields["NavmeshGeometry[17].Data"]);
        spriggitFields["NavmeshGeometry[17].Vertex1"].ShouldBe(dtoFields["NavmeshGeometry[17].Vertex1"]);
        spriggitFields["NavmeshGeometry[18].Data"].ShouldBe(dtoFields["NavmeshGeometry[18].Data"]);
        spriggitFields["NavmeshGeometry[18].Vertex2"].ShouldBe(dtoFields["NavmeshGeometry[18].Vertex2"]);
        spriggitFields["NavmeshGeometry[19].Data"].ShouldBe(dtoFields["NavmeshGeometry[19].Data"]);
        spriggitFields["NavmeshGeometry[19].Vertex1"].ShouldBe(dtoFields["NavmeshGeometry[19].Vertex1"]);
        spriggitFields["NavmeshGeometry[19].Vertex2"].ShouldBe(dtoFields["NavmeshGeometry[19].Vertex2"]);
        spriggitFields["NavmeshGeometry[2].Point"].ShouldBe(dtoFields["NavmeshGeometry[2].Point"]);
        spriggitFields["NavmeshGeometry[20]"].ShouldBe(dtoFields["NavmeshGeometry[20]"]);
        spriggitFields["NavmeshGeometry[21].Cover"].ShouldBe(dtoFields["NavmeshGeometry[21].Cover"]);
        spriggitFields["NavmeshGeometry[21].Triangle"].ShouldBe(dtoFields["NavmeshGeometry[21].Triangle"]);
        spriggitFields["NavmeshGeometry[22].Cover"].ShouldBe(dtoFields["NavmeshGeometry[22].Cover"]);
        spriggitFields["NavmeshGeometry[22].Triangle"].ShouldBe(dtoFields["NavmeshGeometry[22].Triangle"]);
        spriggitFields["NavmeshGeometry[23].Cover"].ShouldBe(dtoFields["NavmeshGeometry[23].Cover"]);
        spriggitFields["NavmeshGeometry[23].Triangle"].ShouldBe(dtoFields["NavmeshGeometry[23].Triangle"]);
        spriggitFields["NavmeshGeometry[24].Cover"].ShouldBe(dtoFields["NavmeshGeometry[24].Cover"]);
        spriggitFields["NavmeshGeometry[24].Triangle"].ShouldBe(dtoFields["NavmeshGeometry[24].Triangle"]);
        spriggitFields["NavmeshGeometry[25].Cover"].ShouldBe(dtoFields["NavmeshGeometry[25].Cover"]);
        spriggitFields["NavmeshGeometry[25].Triangle"].ShouldBe(dtoFields["NavmeshGeometry[25].Triangle"]);
        spriggitFields["NavmeshGeometry[26].Cover"].ShouldBe(dtoFields["NavmeshGeometry[26].Cover"]);
        spriggitFields["NavmeshGeometry[26].Triangle"].ShouldBe(dtoFields["NavmeshGeometry[26].Triangle"]);
        spriggitFields["NavmeshGeometry[27].Cover"].ShouldBe(dtoFields["NavmeshGeometry[27].Cover"]);
        spriggitFields["NavmeshGeometry[27].Triangle"].ShouldBe(dtoFields["NavmeshGeometry[27].Triangle"]);
        spriggitFields["NavmeshGeometry[28].GridCell.Count"].ShouldBe(dtoFields["NavmeshGeometry[28].GridCell.Count"]);
        spriggitFields["NavmeshGeometry[28].GridCell[0]"].ShouldBe(dtoFields["NavmeshGeometry[28].GridCell[0]"]);
        spriggitFields["NavmeshGeometry[28].GridCell[1]"].ShouldBe(dtoFields["NavmeshGeometry[28].GridCell[1]"]);
        spriggitFields["NavmeshGeometry[28].GridCell[2]"].ShouldBe(dtoFields["NavmeshGeometry[28].GridCell[2]"]);
        spriggitFields["NavmeshGeometry[28].GridCell[3]"].ShouldBe(dtoFields["NavmeshGeometry[28].GridCell[3]"]);
        spriggitFields["NavmeshGeometry[28].GridCell[4]"].ShouldBe(dtoFields["NavmeshGeometry[28].GridCell[4]"]);
        spriggitFields["NavmeshGeometry[28].GridCell[5]"].ShouldBe(dtoFields["NavmeshGeometry[28].GridCell[5]"]);
        spriggitFields["NavmeshGeometry[28].GridCell[6]"].ShouldBe(dtoFields["NavmeshGeometry[28].GridCell[6]"]);
        spriggitFields["NavmeshGeometry[28].GridCell[7]"].ShouldBe(dtoFields["NavmeshGeometry[28].GridCell[7]"]);
        spriggitFields["NavmeshGeometry[28].GridCell[8]"].ShouldBe(dtoFields["NavmeshGeometry[28].GridCell[8]"]);
        spriggitFields["NavmeshGeometry[28].GridCell[9]"].ShouldBe(dtoFields["NavmeshGeometry[28].GridCell[9]"]);
        spriggitFields["NavmeshGeometry[3].Point"].ShouldBe(dtoFields["NavmeshGeometry[3].Point"]);
        spriggitFields["NavmeshGeometry[4].Point"].ShouldBe(dtoFields["NavmeshGeometry[4].Point"]);
        spriggitFields["NavmeshGeometry[5].Point"].ShouldBe(dtoFields["NavmeshGeometry[5].Point"]);
        spriggitFields["NavmeshGeometry[6].Point"].ShouldBe(dtoFields["NavmeshGeometry[6].Point"]);
        spriggitFields["NavmeshGeometry[7].Point"].ShouldBe(dtoFields["NavmeshGeometry[7].Point"]);
        spriggitFields["NavmeshGeometry[8].Point"].ShouldBe(dtoFields["NavmeshGeometry[8].Point"]);
        spriggitFields["NavmeshGeometry[9].CoverFlags"].ShouldBe(dtoFields["NavmeshGeometry[9].CoverFlags"]);
        spriggitFields["NavmeshGeometry[9].EdgeLink_0_1"].ShouldBe(dtoFields["NavmeshGeometry[9].EdgeLink_0_1"]);
        spriggitFields["NavmeshGeometry[9].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[9].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[9].EdgeLink_2_0"].ShouldBe(dtoFields["NavmeshGeometry[9].EdgeLink_2_0"]);
        spriggitFields["NavmeshGeometry[9].Height"].ShouldBe(dtoFields["NavmeshGeometry[9].Height"]);
        spriggitFields["NavmeshGeometry[9].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[9].Vertices"]);
        spriggitFields["ObjectBounds.First"].ShouldBe(dtoFields["ObjectBoundsFirst"]);
        spriggitFields["ObjectBounds.Second"].ShouldBe(dtoFields["ObjectBoundsSecond"]);
        spriggitFields["UnknownDNAMFloat"].ShouldBe(dtoFields["UnknownDNAMFloat"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "STAT")]
    [Trait("FormKey", "042AE4:Starfield.esm")]
    [Trait("EditorID", "OpmIntPodSmSideWin01")]
    [Trait("SpriggitFile", "Statics/OpmIntPodSmSideWin01 - 042AE4_Starfield.esm.yaml")]
    public void Starfield_STAT_ShouldMatchSpriggitSample_OpmIntPodSmSideWin01()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Static,
            "OpmIntPodSmSideWin01");
        var dto = Helpers.GetDTO<StaticDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Static,
            "042AE4:Starfield.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["MaxAngle"].ShouldBe(dtoFields["MaxAngle"]);
        spriggitFields["Model.File"].ShouldBe(dtoFields["Models[0].File"]);
        spriggitFields["Model.LightLayer"].ShouldBe(dtoFields["Models[0].LightLayer"]);
        spriggitFields["NavmeshGeometry.Count"].ShouldBe(dtoFields["NavmeshGeometry.Count"]);
        spriggitFields["NavmeshGeometry.GridMax"].ShouldBe(dtoFields["NavmeshGeometry.GridMax"]);
        spriggitFields["NavmeshGeometry.GridMaxDistance"].ShouldBe(dtoFields["NavmeshGeometry.GridMaxDistance"]);
        spriggitFields["NavmeshGeometry.GridMin"].ShouldBe(dtoFields["NavmeshGeometry.GridMin"]);
        spriggitFields["NavmeshGeometry.GridSize"].ShouldBe(dtoFields["NavmeshGeometry.GridSize"]);
        spriggitFields["NavmeshGeometry.Parent.MutagenObjectType"].ShouldBe(dtoFields["NavmeshGeometry.Parent.MutagenObjectType"]);
        spriggitFields["NavmeshGeometry.Parent.Parent"].ShouldBe(dtoFields["NavmeshGeometry.Parent.Parent"]);
        spriggitFields["NavmeshGeometry[0]"].ShouldBe(dtoFields["NavmeshGeometry[0]"]);
        spriggitFields["NavmeshGeometry[1].Point"].ShouldBe(dtoFields["NavmeshGeometry[1].Point"]);
        spriggitFields["NavmeshGeometry[10].CoverFlags"].ShouldBe(dtoFields["NavmeshGeometry[10].CoverFlags"]);
        spriggitFields["NavmeshGeometry[10].EdgeLink_0_1"].ShouldBe(dtoFields["NavmeshGeometry[10].EdgeLink_0_1"]);
        spriggitFields["NavmeshGeometry[10].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[10].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[10].EdgeLink_2_0"].ShouldBe(dtoFields["NavmeshGeometry[10].EdgeLink_2_0"]);
        spriggitFields["NavmeshGeometry[10].Height"].ShouldBe(dtoFields["NavmeshGeometry[10].Height"]);
        spriggitFields["NavmeshGeometry[10].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[10].Vertices"]);
        spriggitFields["NavmeshGeometry[11].CoverFlags"].ShouldBe(dtoFields["NavmeshGeometry[11].CoverFlags"]);
        spriggitFields["NavmeshGeometry[11].EdgeLink_0_1"].ShouldBe(dtoFields["NavmeshGeometry[11].EdgeLink_0_1"]);
        spriggitFields["NavmeshGeometry[11].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[11].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[11].EdgeLink_2_0"].ShouldBe(dtoFields["NavmeshGeometry[11].EdgeLink_2_0"]);
        spriggitFields["NavmeshGeometry[11].Height"].ShouldBe(dtoFields["NavmeshGeometry[11].Height"]);
        spriggitFields["NavmeshGeometry[11].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[11].Vertices"]);
        spriggitFields["NavmeshGeometry[12].CoverFlags"].ShouldBe(dtoFields["NavmeshGeometry[12].CoverFlags"]);
        spriggitFields["NavmeshGeometry[12].EdgeLink_0_1"].ShouldBe(dtoFields["NavmeshGeometry[12].EdgeLink_0_1"]);
        spriggitFields["NavmeshGeometry[12].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[12].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[12].EdgeLink_2_0"].ShouldBe(dtoFields["NavmeshGeometry[12].EdgeLink_2_0"]);
        spriggitFields["NavmeshGeometry[12].Height"].ShouldBe(dtoFields["NavmeshGeometry[12].Height"]);
        spriggitFields["NavmeshGeometry[12].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[12].Vertices"]);
        spriggitFields["NavmeshGeometry[13].CoverFlags"].ShouldBe(dtoFields["NavmeshGeometry[13].CoverFlags"]);
        spriggitFields["NavmeshGeometry[13].EdgeLink_0_1"].ShouldBe(dtoFields["NavmeshGeometry[13].EdgeLink_0_1"]);
        spriggitFields["NavmeshGeometry[13].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[13].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[13].Height"].ShouldBe(dtoFields["NavmeshGeometry[13].Height"]);
        spriggitFields["NavmeshGeometry[13].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[13].Vertices"]);
        spriggitFields["NavmeshGeometry[14].CoverFlags"].ShouldBe(dtoFields["NavmeshGeometry[14].CoverFlags"]);
        spriggitFields["NavmeshGeometry[14].EdgeLink_0_1"].ShouldBe(dtoFields["NavmeshGeometry[14].EdgeLink_0_1"]);
        spriggitFields["NavmeshGeometry[14].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[14].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[14].EdgeLink_2_0"].ShouldBe(dtoFields["NavmeshGeometry[14].EdgeLink_2_0"]);
        spriggitFields["NavmeshGeometry[14].Height"].ShouldBe(dtoFields["NavmeshGeometry[14].Height"]);
        spriggitFields["NavmeshGeometry[14].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[14].Vertices"]);
        spriggitFields["NavmeshGeometry[15].Data"].ShouldBe(dtoFields["NavmeshGeometry[15].Data"]);
        spriggitFields["NavmeshGeometry[15].Vertex1"].ShouldBe(dtoFields["NavmeshGeometry[15].Vertex1"]);
        spriggitFields["NavmeshGeometry[15].Vertex2"].ShouldBe(dtoFields["NavmeshGeometry[15].Vertex2"]);
        spriggitFields["NavmeshGeometry[16].Data"].ShouldBe(dtoFields["NavmeshGeometry[16].Data"]);
        spriggitFields["NavmeshGeometry[16].Vertex1"].ShouldBe(dtoFields["NavmeshGeometry[16].Vertex1"]);
        spriggitFields["NavmeshGeometry[16].Vertex2"].ShouldBe(dtoFields["NavmeshGeometry[16].Vertex2"]);
        spriggitFields["NavmeshGeometry[17].Data"].ShouldBe(dtoFields["NavmeshGeometry[17].Data"]);
        spriggitFields["NavmeshGeometry[17].Vertex1"].ShouldBe(dtoFields["NavmeshGeometry[17].Vertex1"]);
        spriggitFields["NavmeshGeometry[18].Data"].ShouldBe(dtoFields["NavmeshGeometry[18].Data"]);
        spriggitFields["NavmeshGeometry[18].Vertex2"].ShouldBe(dtoFields["NavmeshGeometry[18].Vertex2"]);
        spriggitFields["NavmeshGeometry[19].Data"].ShouldBe(dtoFields["NavmeshGeometry[19].Data"]);
        spriggitFields["NavmeshGeometry[19].Vertex1"].ShouldBe(dtoFields["NavmeshGeometry[19].Vertex1"]);
        spriggitFields["NavmeshGeometry[19].Vertex2"].ShouldBe(dtoFields["NavmeshGeometry[19].Vertex2"]);
        spriggitFields["NavmeshGeometry[2].Point"].ShouldBe(dtoFields["NavmeshGeometry[2].Point"]);
        spriggitFields["NavmeshGeometry[20]"].ShouldBe(dtoFields["NavmeshGeometry[20]"]);
        spriggitFields["NavmeshGeometry[21].Cover"].ShouldBe(dtoFields["NavmeshGeometry[21].Cover"]);
        spriggitFields["NavmeshGeometry[21].Triangle"].ShouldBe(dtoFields["NavmeshGeometry[21].Triangle"]);
        spriggitFields["NavmeshGeometry[22].Cover"].ShouldBe(dtoFields["NavmeshGeometry[22].Cover"]);
        spriggitFields["NavmeshGeometry[22].Triangle"].ShouldBe(dtoFields["NavmeshGeometry[22].Triangle"]);
        spriggitFields["NavmeshGeometry[23].Cover"].ShouldBe(dtoFields["NavmeshGeometry[23].Cover"]);
        spriggitFields["NavmeshGeometry[23].Triangle"].ShouldBe(dtoFields["NavmeshGeometry[23].Triangle"]);
        spriggitFields["NavmeshGeometry[24].Cover"].ShouldBe(dtoFields["NavmeshGeometry[24].Cover"]);
        spriggitFields["NavmeshGeometry[24].Triangle"].ShouldBe(dtoFields["NavmeshGeometry[24].Triangle"]);
        spriggitFields["NavmeshGeometry[25].Cover"].ShouldBe(dtoFields["NavmeshGeometry[25].Cover"]);
        spriggitFields["NavmeshGeometry[25].Triangle"].ShouldBe(dtoFields["NavmeshGeometry[25].Triangle"]);
        spriggitFields["NavmeshGeometry[26].Cover"].ShouldBe(dtoFields["NavmeshGeometry[26].Cover"]);
        spriggitFields["NavmeshGeometry[26].Triangle"].ShouldBe(dtoFields["NavmeshGeometry[26].Triangle"]);
        spriggitFields["NavmeshGeometry[27].Cover"].ShouldBe(dtoFields["NavmeshGeometry[27].Cover"]);
        spriggitFields["NavmeshGeometry[27].Triangle"].ShouldBe(dtoFields["NavmeshGeometry[27].Triangle"]);
        spriggitFields["NavmeshGeometry[28].GridCell.Count"].ShouldBe(dtoFields["NavmeshGeometry[28].GridCell.Count"]);
        spriggitFields["NavmeshGeometry[28].GridCell[0]"].ShouldBe(dtoFields["NavmeshGeometry[28].GridCell[0]"]);
        spriggitFields["NavmeshGeometry[28].GridCell[1]"].ShouldBe(dtoFields["NavmeshGeometry[28].GridCell[1]"]);
        spriggitFields["NavmeshGeometry[28].GridCell[2]"].ShouldBe(dtoFields["NavmeshGeometry[28].GridCell[2]"]);
        spriggitFields["NavmeshGeometry[28].GridCell[3]"].ShouldBe(dtoFields["NavmeshGeometry[28].GridCell[3]"]);
        spriggitFields["NavmeshGeometry[28].GridCell[4]"].ShouldBe(dtoFields["NavmeshGeometry[28].GridCell[4]"]);
        spriggitFields["NavmeshGeometry[28].GridCell[5]"].ShouldBe(dtoFields["NavmeshGeometry[28].GridCell[5]"]);
        spriggitFields["NavmeshGeometry[28].GridCell[6]"].ShouldBe(dtoFields["NavmeshGeometry[28].GridCell[6]"]);
        spriggitFields["NavmeshGeometry[28].GridCell[7]"].ShouldBe(dtoFields["NavmeshGeometry[28].GridCell[7]"]);
        spriggitFields["NavmeshGeometry[28].GridCell[8]"].ShouldBe(dtoFields["NavmeshGeometry[28].GridCell[8]"]);
        spriggitFields["NavmeshGeometry[28].GridCell[9]"].ShouldBe(dtoFields["NavmeshGeometry[28].GridCell[9]"]);
        spriggitFields["NavmeshGeometry[3].Point"].ShouldBe(dtoFields["NavmeshGeometry[3].Point"]);
        spriggitFields["NavmeshGeometry[4].Point"].ShouldBe(dtoFields["NavmeshGeometry[4].Point"]);
        spriggitFields["NavmeshGeometry[5].Point"].ShouldBe(dtoFields["NavmeshGeometry[5].Point"]);
        spriggitFields["NavmeshGeometry[6].Point"].ShouldBe(dtoFields["NavmeshGeometry[6].Point"]);
        spriggitFields["NavmeshGeometry[7].Point"].ShouldBe(dtoFields["NavmeshGeometry[7].Point"]);
        spriggitFields["NavmeshGeometry[8].Point"].ShouldBe(dtoFields["NavmeshGeometry[8].Point"]);
        spriggitFields["NavmeshGeometry[9].CoverFlags"].ShouldBe(dtoFields["NavmeshGeometry[9].CoverFlags"]);
        spriggitFields["NavmeshGeometry[9].EdgeLink_0_1"].ShouldBe(dtoFields["NavmeshGeometry[9].EdgeLink_0_1"]);
        spriggitFields["NavmeshGeometry[9].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[9].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[9].EdgeLink_2_0"].ShouldBe(dtoFields["NavmeshGeometry[9].EdgeLink_2_0"]);
        spriggitFields["NavmeshGeometry[9].Height"].ShouldBe(dtoFields["NavmeshGeometry[9].Height"]);
        spriggitFields["NavmeshGeometry[9].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[9].Vertices"]);
        spriggitFields["ObjectBounds.First"].ShouldBe(dtoFields["ObjectBoundsFirst"]);
        spriggitFields["ObjectBounds.Second"].ShouldBe(dtoFields["ObjectBoundsSecond"]);
        spriggitFields["UnknownDNAMFloat"].ShouldBe(dtoFields["UnknownDNAMFloat"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "STAT")]
    [Trait("FormKey", "03A1B4:Starfield.esm")]
    [Trait("EditorID", "CatIndWalkSm2WayB01")]
    [Trait("SpriggitFile", "Statics/CatIndWalkSm2WayB01 - 03A1B4_Starfield.esm.yaml")]
    public void Starfield_STAT_ShouldMatchSpriggitSample_CatIndWalkSm2WayB01()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Static,
            "CatIndWalkSm2WayB01");
        var dto = Helpers.GetDTO<StaticDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Static,
            "03A1B4:Starfield.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["DirtinessScale"].ShouldBe(dtoFields["DirtinessScale"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["MaxAngle"].ShouldBe(dtoFields["MaxAngle"]);
        spriggitFields["Model.File"].ShouldBe(dtoFields["Models[0].File"]);
        spriggitFields["Model.LightLayer"].ShouldBe(dtoFields["Models[0].LightLayer"]);
        spriggitFields["NavmeshGeometry.Count"].ShouldBe(dtoFields["NavmeshGeometry.Count"]);
        spriggitFields["NavmeshGeometry.GridMax"].ShouldBe(dtoFields["NavmeshGeometry.GridMax"]);
        spriggitFields["NavmeshGeometry.GridMaxDistance"].ShouldBe(dtoFields["NavmeshGeometry.GridMaxDistance"]);
        spriggitFields["NavmeshGeometry.GridMin"].ShouldBe(dtoFields["NavmeshGeometry.GridMin"]);
        spriggitFields["NavmeshGeometry.GridSize"].ShouldBe(dtoFields["NavmeshGeometry.GridSize"]);
        spriggitFields["NavmeshGeometry.Parent.MutagenObjectType"].ShouldBe(dtoFields["NavmeshGeometry.Parent.MutagenObjectType"]);
        spriggitFields["NavmeshGeometry.Parent.Parent"].ShouldBe(dtoFields["NavmeshGeometry.Parent.Parent"]);
        spriggitFields["NavmeshGeometry[0]"].ShouldBe(dtoFields["NavmeshGeometry[0]"]);
        spriggitFields["NavmeshGeometry[1].Point"].ShouldBe(dtoFields["NavmeshGeometry[1].Point"]);
        spriggitFields["NavmeshGeometry[10].EdgeLink_0_1"].ShouldBe(dtoFields["NavmeshGeometry[10].EdgeLink_0_1"]);
        spriggitFields["NavmeshGeometry[10].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[10].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[10].EdgeLink_2_0"].ShouldBe(dtoFields["NavmeshGeometry[10].EdgeLink_2_0"]);
        spriggitFields["NavmeshGeometry[10].Height"].ShouldBe(dtoFields["NavmeshGeometry[10].Height"]);
        spriggitFields["NavmeshGeometry[10].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[10].Vertices"]);
        spriggitFields["NavmeshGeometry[11].EdgeLink_0_1"].ShouldBe(dtoFields["NavmeshGeometry[11].EdgeLink_0_1"]);
        spriggitFields["NavmeshGeometry[11].EdgeLink_2_0"].ShouldBe(dtoFields["NavmeshGeometry[11].EdgeLink_2_0"]);
        spriggitFields["NavmeshGeometry[11].Height"].ShouldBe(dtoFields["NavmeshGeometry[11].Height"]);
        spriggitFields["NavmeshGeometry[11].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[11].Vertices"]);
        spriggitFields["NavmeshGeometry[12].EdgeLink_0_1"].ShouldBe(dtoFields["NavmeshGeometry[12].EdgeLink_0_1"]);
        spriggitFields["NavmeshGeometry[12].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[12].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[12].EdgeLink_2_0"].ShouldBe(dtoFields["NavmeshGeometry[12].EdgeLink_2_0"]);
        spriggitFields["NavmeshGeometry[12].Height"].ShouldBe(dtoFields["NavmeshGeometry[12].Height"]);
        spriggitFields["NavmeshGeometry[12].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[12].Vertices"]);
        spriggitFields["NavmeshGeometry[13].EdgeLink_0_1"].ShouldBe(dtoFields["NavmeshGeometry[13].EdgeLink_0_1"]);
        spriggitFields["NavmeshGeometry[13].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[13].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[13].EdgeLink_2_0"].ShouldBe(dtoFields["NavmeshGeometry[13].EdgeLink_2_0"]);
        spriggitFields["NavmeshGeometry[13].Height"].ShouldBe(dtoFields["NavmeshGeometry[13].Height"]);
        spriggitFields["NavmeshGeometry[13].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[13].Vertices"]);
        spriggitFields["NavmeshGeometry[14].EdgeLink_0_1"].ShouldBe(dtoFields["NavmeshGeometry[14].EdgeLink_0_1"]);
        spriggitFields["NavmeshGeometry[14].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[14].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[14].EdgeLink_2_0"].ShouldBe(dtoFields["NavmeshGeometry[14].EdgeLink_2_0"]);
        spriggitFields["NavmeshGeometry[14].Height"].ShouldBe(dtoFields["NavmeshGeometry[14].Height"]);
        spriggitFields["NavmeshGeometry[14].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[14].Vertices"]);
        spriggitFields["NavmeshGeometry[15].GridCell.Count"].ShouldBe(dtoFields["NavmeshGeometry[15].GridCell.Count"]);
        spriggitFields["NavmeshGeometry[15].GridCell[0]"].ShouldBe(dtoFields["NavmeshGeometry[15].GridCell[0]"]);
        spriggitFields["NavmeshGeometry[15].GridCell[1]"].ShouldBe(dtoFields["NavmeshGeometry[15].GridCell[1]"]);
        spriggitFields["NavmeshGeometry[15].GridCell[2]"].ShouldBe(dtoFields["NavmeshGeometry[15].GridCell[2]"]);
        spriggitFields["NavmeshGeometry[15].GridCell[3]"].ShouldBe(dtoFields["NavmeshGeometry[15].GridCell[3]"]);
        spriggitFields["NavmeshGeometry[15].GridCell[4]"].ShouldBe(dtoFields["NavmeshGeometry[15].GridCell[4]"]);
        spriggitFields["NavmeshGeometry[15].GridCell[5]"].ShouldBe(dtoFields["NavmeshGeometry[15].GridCell[5]"]);
        spriggitFields["NavmeshGeometry[15].GridCell[6]"].ShouldBe(dtoFields["NavmeshGeometry[15].GridCell[6]"]);
        spriggitFields["NavmeshGeometry[15].GridCell[7]"].ShouldBe(dtoFields["NavmeshGeometry[15].GridCell[7]"]);
        spriggitFields["NavmeshGeometry[15].GridCell[8]"].ShouldBe(dtoFields["NavmeshGeometry[15].GridCell[8]"]);
        spriggitFields["NavmeshGeometry[15].GridCell[9]"].ShouldBe(dtoFields["NavmeshGeometry[15].GridCell[9]"]);
        spriggitFields["NavmeshGeometry[2].Point"].ShouldBe(dtoFields["NavmeshGeometry[2].Point"]);
        spriggitFields["NavmeshGeometry[3].Point"].ShouldBe(dtoFields["NavmeshGeometry[3].Point"]);
        spriggitFields["NavmeshGeometry[4].Point"].ShouldBe(dtoFields["NavmeshGeometry[4].Point"]);
        spriggitFields["NavmeshGeometry[5].Point"].ShouldBe(dtoFields["NavmeshGeometry[5].Point"]);
        spriggitFields["NavmeshGeometry[6].Point"].ShouldBe(dtoFields["NavmeshGeometry[6].Point"]);
        spriggitFields["NavmeshGeometry[7].Point"].ShouldBe(dtoFields["NavmeshGeometry[7].Point"]);
        spriggitFields["NavmeshGeometry[8].Point"].ShouldBe(dtoFields["NavmeshGeometry[8].Point"]);
        spriggitFields["NavmeshGeometry[9].EdgeLink_0_1"].ShouldBe(dtoFields["NavmeshGeometry[9].EdgeLink_0_1"]);
        spriggitFields["NavmeshGeometry[9].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[9].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[9].EdgeLink_2_0"].ShouldBe(dtoFields["NavmeshGeometry[9].EdgeLink_2_0"]);
        spriggitFields["NavmeshGeometry[9].Height"].ShouldBe(dtoFields["NavmeshGeometry[9].Height"]);
        spriggitFields["NavmeshGeometry[9].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[9].Vertices"]);
        spriggitFields["ObjectBounds.First"].ShouldBe(dtoFields["ObjectBoundsFirst"]);
        spriggitFields["ObjectBounds.Second"].ShouldBe(dtoFields["ObjectBoundsSecond"]);
        spriggitFields["UnknownDNAMFloat"].ShouldBe(dtoFields["UnknownDNAMFloat"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "STAT")]
    [Trait("FormKey", "04F391:Starfield.esm")]
    [Trait("EditorID", "OpiExtPodAirlockStairs01")]
    [Trait("SpriggitFile", "Statics/OpiExtPodAirlockStairs01 - 04F391_Starfield.esm.yaml")]
    public void Starfield_STAT_ShouldMatchSpriggitSample_OpiExtPodAirlockStairs01()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Static,
            "OpiExtPodAirlockStairs01");
        var dto = Helpers.GetDTO<StaticDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Static,
            "04F391:Starfield.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["DirtinessScale"].ShouldBe(dtoFields["DirtinessScale"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["MaxAngle"].ShouldBe(dtoFields["MaxAngle"]);
        spriggitFields["Model.File"].ShouldBe(dtoFields["Models[0].File"]);
        spriggitFields["Model.LightLayer"].ShouldBe(dtoFields["Models[0].LightLayer"]);
        spriggitFields["NavmeshGeometry.Count"].ShouldBe(dtoFields["NavmeshGeometry.Count"]);
        spriggitFields["NavmeshGeometry.GridMax"].ShouldBe(dtoFields["NavmeshGeometry.GridMax"]);
        spriggitFields["NavmeshGeometry.GridMaxDistance"].ShouldBe(dtoFields["NavmeshGeometry.GridMaxDistance"]);
        spriggitFields["NavmeshGeometry.GridMin"].ShouldBe(dtoFields["NavmeshGeometry.GridMin"]);
        spriggitFields["NavmeshGeometry.GridSize"].ShouldBe(dtoFields["NavmeshGeometry.GridSize"]);
        spriggitFields["NavmeshGeometry.Parent.MutagenObjectType"].ShouldBe(dtoFields["NavmeshGeometry.Parent.MutagenObjectType"]);
        spriggitFields["NavmeshGeometry.Parent.Parent"].ShouldBe(dtoFields["NavmeshGeometry.Parent.Parent"]);
        spriggitFields["NavmeshGeometry[0]"].ShouldBe(dtoFields["NavmeshGeometry[0]"]);
        spriggitFields["NavmeshGeometry[1].Point"].ShouldBe(dtoFields["NavmeshGeometry[1].Point"]);
        spriggitFields["NavmeshGeometry[10].Point"].ShouldBe(dtoFields["NavmeshGeometry[10].Point"]);
        spriggitFields["NavmeshGeometry[11].Point"].ShouldBe(dtoFields["NavmeshGeometry[11].Point"]);
        spriggitFields["NavmeshGeometry[12].Point"].ShouldBe(dtoFields["NavmeshGeometry[12].Point"]);
        spriggitFields["NavmeshGeometry[13].Point"].ShouldBe(dtoFields["NavmeshGeometry[13].Point"]);
        spriggitFields["NavmeshGeometry[14].Point"].ShouldBe(dtoFields["NavmeshGeometry[14].Point"]);
        spriggitFields["NavmeshGeometry[15].EdgeLink_0_1"].ShouldBe(dtoFields["NavmeshGeometry[15].EdgeLink_0_1"]);
        spriggitFields["NavmeshGeometry[15].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[15].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[15].EdgeLink_2_0"].ShouldBe(dtoFields["NavmeshGeometry[15].EdgeLink_2_0"]);
        spriggitFields["NavmeshGeometry[15].Height"].ShouldBe(dtoFields["NavmeshGeometry[15].Height"]);
        spriggitFields["NavmeshGeometry[15].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[15].Vertices"]);
        spriggitFields["NavmeshGeometry[16].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[16].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[16].EdgeLink_2_0"].ShouldBe(dtoFields["NavmeshGeometry[16].EdgeLink_2_0"]);
        spriggitFields["NavmeshGeometry[16].Height"].ShouldBe(dtoFields["NavmeshGeometry[16].Height"]);
        spriggitFields["NavmeshGeometry[16].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[16].Vertices"]);
        spriggitFields["NavmeshGeometry[17].EdgeLink_0_1"].ShouldBe(dtoFields["NavmeshGeometry[17].EdgeLink_0_1"]);
        spriggitFields["NavmeshGeometry[17].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[17].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[17].EdgeLink_2_0"].ShouldBe(dtoFields["NavmeshGeometry[17].EdgeLink_2_0"]);
        spriggitFields["NavmeshGeometry[17].Height"].ShouldBe(dtoFields["NavmeshGeometry[17].Height"]);
        spriggitFields["NavmeshGeometry[17].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[17].Vertices"]);
        spriggitFields["NavmeshGeometry[18].EdgeLink_0_1"].ShouldBe(dtoFields["NavmeshGeometry[18].EdgeLink_0_1"]);
        spriggitFields["NavmeshGeometry[18].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[18].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[18].EdgeLink_2_0"].ShouldBe(dtoFields["NavmeshGeometry[18].EdgeLink_2_0"]);
        spriggitFields["NavmeshGeometry[18].Height"].ShouldBe(dtoFields["NavmeshGeometry[18].Height"]);
        spriggitFields["NavmeshGeometry[18].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[18].Vertices"]);
        spriggitFields["NavmeshGeometry[19].EdgeLink_0_1"].ShouldBe(dtoFields["NavmeshGeometry[19].EdgeLink_0_1"]);
        spriggitFields["NavmeshGeometry[19].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[19].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[19].EdgeLink_2_0"].ShouldBe(dtoFields["NavmeshGeometry[19].EdgeLink_2_0"]);
        spriggitFields["NavmeshGeometry[19].Height"].ShouldBe(dtoFields["NavmeshGeometry[19].Height"]);
        spriggitFields["NavmeshGeometry[19].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[19].Vertices"]);
        spriggitFields["NavmeshGeometry[2].Point"].ShouldBe(dtoFields["NavmeshGeometry[2].Point"]);
        spriggitFields["NavmeshGeometry[20].EdgeLink_0_1"].ShouldBe(dtoFields["NavmeshGeometry[20].EdgeLink_0_1"]);
        spriggitFields["NavmeshGeometry[20].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[20].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[20].EdgeLink_2_0"].ShouldBe(dtoFields["NavmeshGeometry[20].EdgeLink_2_0"]);
        spriggitFields["NavmeshGeometry[20].Height"].ShouldBe(dtoFields["NavmeshGeometry[20].Height"]);
        spriggitFields["NavmeshGeometry[20].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[20].Vertices"]);
        spriggitFields["NavmeshGeometry[21].EdgeLink_0_1"].ShouldBe(dtoFields["NavmeshGeometry[21].EdgeLink_0_1"]);
        spriggitFields["NavmeshGeometry[21].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[21].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[21].EdgeLink_2_0"].ShouldBe(dtoFields["NavmeshGeometry[21].EdgeLink_2_0"]);
        spriggitFields["NavmeshGeometry[21].Height"].ShouldBe(dtoFields["NavmeshGeometry[21].Height"]);
        spriggitFields["NavmeshGeometry[21].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[21].Vertices"]);
        spriggitFields["NavmeshGeometry[22].EdgeLink_0_1"].ShouldBe(dtoFields["NavmeshGeometry[22].EdgeLink_0_1"]);
        spriggitFields["NavmeshGeometry[22].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[22].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[22].EdgeLink_2_0"].ShouldBe(dtoFields["NavmeshGeometry[22].EdgeLink_2_0"]);
        spriggitFields["NavmeshGeometry[22].Height"].ShouldBe(dtoFields["NavmeshGeometry[22].Height"]);
        spriggitFields["NavmeshGeometry[22].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[22].Vertices"]);
        spriggitFields["NavmeshGeometry[23].EdgeLink_0_1"].ShouldBe(dtoFields["NavmeshGeometry[23].EdgeLink_0_1"]);
        spriggitFields["NavmeshGeometry[23].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[23].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[23].EdgeLink_2_0"].ShouldBe(dtoFields["NavmeshGeometry[23].EdgeLink_2_0"]);
        spriggitFields["NavmeshGeometry[23].Height"].ShouldBe(dtoFields["NavmeshGeometry[23].Height"]);
        spriggitFields["NavmeshGeometry[23].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[23].Vertices"]);
        spriggitFields["NavmeshGeometry[24].EdgeLink_0_1"].ShouldBe(dtoFields["NavmeshGeometry[24].EdgeLink_0_1"]);
        spriggitFields["NavmeshGeometry[24].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[24].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[24].EdgeLink_2_0"].ShouldBe(dtoFields["NavmeshGeometry[24].EdgeLink_2_0"]);
        spriggitFields["NavmeshGeometry[24].Height"].ShouldBe(dtoFields["NavmeshGeometry[24].Height"]);
        spriggitFields["NavmeshGeometry[24].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[24].Vertices"]);
        spriggitFields["NavmeshGeometry[25].EdgeLink_0_1"].ShouldBe(dtoFields["NavmeshGeometry[25].EdgeLink_0_1"]);
        spriggitFields["NavmeshGeometry[25].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[25].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[25].EdgeLink_2_0"].ShouldBe(dtoFields["NavmeshGeometry[25].EdgeLink_2_0"]);
        spriggitFields["NavmeshGeometry[25].Height"].ShouldBe(dtoFields["NavmeshGeometry[25].Height"]);
        spriggitFields["NavmeshGeometry[25].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[25].Vertices"]);
        spriggitFields["NavmeshGeometry[26].EdgeLink_0_1"].ShouldBe(dtoFields["NavmeshGeometry[26].EdgeLink_0_1"]);
        spriggitFields["NavmeshGeometry[26].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[26].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[26].EdgeLink_2_0"].ShouldBe(dtoFields["NavmeshGeometry[26].EdgeLink_2_0"]);
        spriggitFields["NavmeshGeometry[26].Height"].ShouldBe(dtoFields["NavmeshGeometry[26].Height"]);
        spriggitFields["NavmeshGeometry[26].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[26].Vertices"]);
        spriggitFields["NavmeshGeometry[27].GridCell.Count"].ShouldBe(dtoFields["NavmeshGeometry[27].GridCell.Count"]);
        spriggitFields["NavmeshGeometry[27].GridCell[0]"].ShouldBe(dtoFields["NavmeshGeometry[27].GridCell[0]"]);
        spriggitFields["NavmeshGeometry[27].GridCell[1]"].ShouldBe(dtoFields["NavmeshGeometry[27].GridCell[1]"]);
        spriggitFields["NavmeshGeometry[27].GridCell[10]"].ShouldBe(dtoFields["NavmeshGeometry[27].GridCell[10]"]);
        spriggitFields["NavmeshGeometry[27].GridCell[11]"].ShouldBe(dtoFields["NavmeshGeometry[27].GridCell[11]"]);
        spriggitFields["NavmeshGeometry[27].GridCell[12]"].ShouldBe(dtoFields["NavmeshGeometry[27].GridCell[12]"]);
        spriggitFields["NavmeshGeometry[27].GridCell[13]"].ShouldBe(dtoFields["NavmeshGeometry[27].GridCell[13]"]);
        spriggitFields["NavmeshGeometry[27].GridCell[14]"].ShouldBe(dtoFields["NavmeshGeometry[27].GridCell[14]"]);
        spriggitFields["NavmeshGeometry[27].GridCell[15]"].ShouldBe(dtoFields["NavmeshGeometry[27].GridCell[15]"]);
        spriggitFields["NavmeshGeometry[27].GridCell[2]"].ShouldBe(dtoFields["NavmeshGeometry[27].GridCell[2]"]);
        spriggitFields["NavmeshGeometry[27].GridCell[3]"].ShouldBe(dtoFields["NavmeshGeometry[27].GridCell[3]"]);
        spriggitFields["NavmeshGeometry[27].GridCell[4]"].ShouldBe(dtoFields["NavmeshGeometry[27].GridCell[4]"]);
        spriggitFields["NavmeshGeometry[27].GridCell[5]"].ShouldBe(dtoFields["NavmeshGeometry[27].GridCell[5]"]);
        spriggitFields["NavmeshGeometry[27].GridCell[6]"].ShouldBe(dtoFields["NavmeshGeometry[27].GridCell[6]"]);
        spriggitFields["NavmeshGeometry[27].GridCell[7]"].ShouldBe(dtoFields["NavmeshGeometry[27].GridCell[7]"]);
        spriggitFields["NavmeshGeometry[27].GridCell[8]"].ShouldBe(dtoFields["NavmeshGeometry[27].GridCell[8]"]);
        spriggitFields["NavmeshGeometry[27].GridCell[9]"].ShouldBe(dtoFields["NavmeshGeometry[27].GridCell[9]"]);
        spriggitFields["NavmeshGeometry[3].Point"].ShouldBe(dtoFields["NavmeshGeometry[3].Point"]);
        spriggitFields["NavmeshGeometry[4].Point"].ShouldBe(dtoFields["NavmeshGeometry[4].Point"]);
        spriggitFields["NavmeshGeometry[5].Point"].ShouldBe(dtoFields["NavmeshGeometry[5].Point"]);
        spriggitFields["NavmeshGeometry[6].Point"].ShouldBe(dtoFields["NavmeshGeometry[6].Point"]);
        spriggitFields["NavmeshGeometry[7].Point"].ShouldBe(dtoFields["NavmeshGeometry[7].Point"]);
        spriggitFields["NavmeshGeometry[8].Point"].ShouldBe(dtoFields["NavmeshGeometry[8].Point"]);
        spriggitFields["NavmeshGeometry[9].Point"].ShouldBe(dtoFields["NavmeshGeometry[9].Point"]);
        spriggitFields["ObjectBounds.First"].ShouldBe(dtoFields["ObjectBoundsFirst"]);
        spriggitFields["ObjectBounds.Second"].ShouldBe(dtoFields["ObjectBoundsSecond"]);
        spriggitFields["REFL"].ShouldBe(dtoFields["REFL"]);
        spriggitFields["UnknownDNAMFloat"].ShouldBe(dtoFields["UnknownDNAMFloat"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }
}
