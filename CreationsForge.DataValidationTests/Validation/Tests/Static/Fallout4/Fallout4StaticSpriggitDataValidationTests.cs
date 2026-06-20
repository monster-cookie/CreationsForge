using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.Static.Fallout4;

public class Fallout4StaticSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "STAT")]
    [Trait("FormKey", "1B4AC0:Fallout4.esm")]
    [Trait("EditorID", "workshop_JunkWallDoor01")]
    [Trait("SpriggitFile", "Statics/workshop_JunkWallDoor01 - 1B4AC0_Fallout4.esm.yaml")]
    public void Fallout4_STAT_ShouldMatchSpriggitSample_workshop_JunkWallDoor01()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Static,
            "workshop_JunkWallDoor01");
        var dto = Helpers.GetDTO<StaticDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Static,
            "1B4AC0:Fallout4.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["LeafAmplitude"].ShouldBe(dtoFields["LeafAmplitude"]);
        spriggitFields["LeafFrequency"].ShouldBe(dtoFields["LeafFrequency"]);
        spriggitFields["MaxAngle"].ShouldBe(dtoFields["MaxAngle"]);
        spriggitFields["Model.Data"].ShouldBe(dtoFields["Model.Data"]);
        spriggitFields["Model.File"].ShouldBe(dtoFields["Models[0].File"]);
        spriggitFields["Name.Count"].ShouldBe(dtoFields["Name.Count"]);
        spriggitFields["Name.TargetLanguage"].ShouldBe(dtoFields["Name.TargetLanguage"]);
        spriggitFields["Name[0].Language"].ShouldBe(dtoFields["Name[0].Language"]);
        spriggitFields["Name[0].String"].ShouldBe(dtoFields["Name[0].String"]);
        spriggitFields["Name[1].Language"].ShouldBe(dtoFields["Name[1].Language"]);
        spriggitFields["Name[1].String"].ShouldBe(dtoFields["Name[1].String"]);
        spriggitFields["Name[10].Language"].ShouldBe(dtoFields["Name[10].Language"]);
        spriggitFields["Name[10].String"].ShouldBe(dtoFields["Name[10].String"]);
        spriggitFields["Name[2].Language"].ShouldBe(dtoFields["Name[2].Language"]);
        spriggitFields["Name[2].String"].ShouldBe(dtoFields["Name[2].String"]);
        spriggitFields["Name[3].Language"].ShouldBe(dtoFields["Name[3].Language"]);
        spriggitFields["Name[3].String"].ShouldBe(dtoFields["Name[3].String"]);
        spriggitFields["Name[4].Language"].ShouldBe(dtoFields["Name[4].Language"]);
        spriggitFields["Name[4].String"].ShouldBe(dtoFields["Name[4].String"]);
        spriggitFields["Name[5].Language"].ShouldBe(dtoFields["Name[5].Language"]);
        spriggitFields["Name[5].String"].ShouldBe(dtoFields["Name[5].String"]);
        spriggitFields["Name[6].Language"].ShouldBe(dtoFields["Name[6].Language"]);
        spriggitFields["Name[6].String"].ShouldBe(dtoFields["Name[6].String"]);
        spriggitFields["Name[7].Language"].ShouldBe(dtoFields["Name[7].Language"]);
        spriggitFields["Name[7].String"].ShouldBe(dtoFields["Name[7].String"]);
        spriggitFields["Name[8].Language"].ShouldBe(dtoFields["Name[8].Language"]);
        spriggitFields["Name[8].String"].ShouldBe(dtoFields["Name[8].String"]);
        spriggitFields["Name[9].Language"].ShouldBe(dtoFields["Name[9].Language"]);
        spriggitFields["Name[9].String"].ShouldBe(dtoFields["Name[9].String"]);
        spriggitFields["NavmeshGeometry.Count"].ShouldBe(dtoFields["NavmeshGeometry.Count"]);
        spriggitFields["NavmeshGeometry.GridArrays.Count"].ShouldBe(dtoFields["NavmeshGeometry.GridArrays.Count"]);
        spriggitFields["NavmeshGeometry.GridArrays[0]"].ShouldBe(dtoFields["NavmeshGeometry.GridArrays[0]"]);
        spriggitFields["NavmeshGeometry.GridArrays[1]"].ShouldBe(dtoFields["NavmeshGeometry.GridArrays[1]"]);
        spriggitFields["NavmeshGeometry.GridArrays[2]"].ShouldBe(dtoFields["NavmeshGeometry.GridArrays[2]"]);
        spriggitFields["NavmeshGeometry.GridArrays[3]"].ShouldBe(dtoFields["NavmeshGeometry.GridArrays[3]"]);
        spriggitFields["NavmeshGeometry.GridArrays[4]"].ShouldBe(dtoFields["NavmeshGeometry.GridArrays[4]"]);
        spriggitFields["NavmeshGeometry.GridArrays[5]"].ShouldBe(dtoFields["NavmeshGeometry.GridArrays[5]"]);
        spriggitFields["NavmeshGeometry.GridArrays[6]"].ShouldBe(dtoFields["NavmeshGeometry.GridArrays[6]"]);
        spriggitFields["NavmeshGeometry.GridArrays[7]"].ShouldBe(dtoFields["NavmeshGeometry.GridArrays[7]"]);
        spriggitFields["NavmeshGeometry.GridMax"].ShouldBe(dtoFields["NavmeshGeometry.GridMax"]);
        spriggitFields["NavmeshGeometry.GridMaxDistance"].ShouldBe(dtoFields["NavmeshGeometry.GridMaxDistance"]);
        spriggitFields["NavmeshGeometry.GridMin"].ShouldBe(dtoFields["NavmeshGeometry.GridMin"]);
        spriggitFields["NavmeshGeometry.GridSize"].ShouldBe(dtoFields["NavmeshGeometry.GridSize"]);
        spriggitFields["NavmeshGeometry.Parent.MutagenObjectType"].ShouldBe(dtoFields["NavmeshGeometry.Parent.MutagenObjectType"]);
        spriggitFields["NavmeshGeometry.Parent.Parent"].ShouldBe(dtoFields["NavmeshGeometry.Parent.Parent"]);
        spriggitFields["NavmeshGeometry[0]"].ShouldBe(dtoFields["NavmeshGeometry[0]"]);
        spriggitFields["NavmeshGeometry[1]"].ShouldBe(dtoFields["NavmeshGeometry[1]"]);
        spriggitFields["NavmeshGeometry[10].Count"].ShouldBe(dtoFields["NavmeshGeometry[10].Count"]);
        spriggitFields["NavmeshGeometry[10].EdgeLink_0_1"].ShouldBe(dtoFields["NavmeshGeometry[10].EdgeLink_0_1"]);
        spriggitFields["NavmeshGeometry[10].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[10].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[10].EdgeLink_2_0"].ShouldBe(dtoFields["NavmeshGeometry[10].EdgeLink_2_0"]);
        spriggitFields["NavmeshGeometry[10].Height"].ShouldBe(dtoFields["NavmeshGeometry[10].Height"]);
        spriggitFields["NavmeshGeometry[10].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[10].Vertices"]);
        spriggitFields["NavmeshGeometry[10][0]"].ShouldBe(dtoFields["NavmeshGeometry[10][0]"]);
        spriggitFields["NavmeshGeometry[11].Count"].ShouldBe(dtoFields["NavmeshGeometry[11].Count"]);
        spriggitFields["NavmeshGeometry[11].EdgeLink_0_1"].ShouldBe(dtoFields["NavmeshGeometry[11].EdgeLink_0_1"]);
        spriggitFields["NavmeshGeometry[11].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[11].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[11].EdgeLink_2_0"].ShouldBe(dtoFields["NavmeshGeometry[11].EdgeLink_2_0"]);
        spriggitFields["NavmeshGeometry[11].Height"].ShouldBe(dtoFields["NavmeshGeometry[11].Height"]);
        spriggitFields["NavmeshGeometry[11].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[11].Vertices"]);
        spriggitFields["NavmeshGeometry[11][0]"].ShouldBe(dtoFields["NavmeshGeometry[11][0]"]);
        spriggitFields["NavmeshGeometry[12].Count"].ShouldBe(dtoFields["NavmeshGeometry[12].Count"]);
        spriggitFields["NavmeshGeometry[12].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[12].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[12].EdgeLink_2_0"].ShouldBe(dtoFields["NavmeshGeometry[12].EdgeLink_2_0"]);
        spriggitFields["NavmeshGeometry[12].Height"].ShouldBe(dtoFields["NavmeshGeometry[12].Height"]);
        spriggitFields["NavmeshGeometry[12].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[12].Vertices"]);
        spriggitFields["NavmeshGeometry[12][0]"].ShouldBe(dtoFields["NavmeshGeometry[12][0]"]);
        spriggitFields["NavmeshGeometry[13].Count"].ShouldBe(dtoFields["NavmeshGeometry[13].Count"]);
        spriggitFields["NavmeshGeometry[13].EdgeLink_0_1"].ShouldBe(dtoFields["NavmeshGeometry[13].EdgeLink_0_1"]);
        spriggitFields["NavmeshGeometry[13].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[13].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[13].EdgeLink_2_0"].ShouldBe(dtoFields["NavmeshGeometry[13].EdgeLink_2_0"]);
        spriggitFields["NavmeshGeometry[13].Height"].ShouldBe(dtoFields["NavmeshGeometry[13].Height"]);
        spriggitFields["NavmeshGeometry[13].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[13].Vertices"]);
        spriggitFields["NavmeshGeometry[13][0]"].ShouldBe(dtoFields["NavmeshGeometry[13][0]"]);
        spriggitFields["NavmeshGeometry[2]"].ShouldBe(dtoFields["NavmeshGeometry[2]"]);
        spriggitFields["NavmeshGeometry[3]"].ShouldBe(dtoFields["NavmeshGeometry[3]"]);
        spriggitFields["NavmeshGeometry[4]"].ShouldBe(dtoFields["NavmeshGeometry[4]"]);
        spriggitFields["NavmeshGeometry[5]"].ShouldBe(dtoFields["NavmeshGeometry[5]"]);
        spriggitFields["NavmeshGeometry[6]"].ShouldBe(dtoFields["NavmeshGeometry[6]"]);
        spriggitFields["NavmeshGeometry[7]"].ShouldBe(dtoFields["NavmeshGeometry[7]"]);
        spriggitFields["NavmeshGeometry[8].Count"].ShouldBe(dtoFields["NavmeshGeometry[8].Count"]);
        spriggitFields["NavmeshGeometry[8].EdgeLink_0_1"].ShouldBe(dtoFields["NavmeshGeometry[8].EdgeLink_0_1"]);
        spriggitFields["NavmeshGeometry[8].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[8].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[8].EdgeLink_2_0"].ShouldBe(dtoFields["NavmeshGeometry[8].EdgeLink_2_0"]);
        spriggitFields["NavmeshGeometry[8].Height"].ShouldBe(dtoFields["NavmeshGeometry[8].Height"]);
        spriggitFields["NavmeshGeometry[8].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[8].Vertices"]);
        spriggitFields["NavmeshGeometry[8][0]"].ShouldBe(dtoFields["NavmeshGeometry[8][0]"]);
        spriggitFields["NavmeshGeometry[9].Count"].ShouldBe(dtoFields["NavmeshGeometry[9].Count"]);
        spriggitFields["NavmeshGeometry[9].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[9].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[9].EdgeLink_2_0"].ShouldBe(dtoFields["NavmeshGeometry[9].EdgeLink_2_0"]);
        spriggitFields["NavmeshGeometry[9].Height"].ShouldBe(dtoFields["NavmeshGeometry[9].Height"]);
        spriggitFields["NavmeshGeometry[9].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[9].Vertices"]);
        spriggitFields["NavmeshGeometry[9][0]"].ShouldBe(dtoFields["NavmeshGeometry[9][0]"]);
        spriggitFields["ObjectBounds.First"].ShouldBe(dtoFields["ObjectBoundsFirst"]);
        spriggitFields["ObjectBounds.Second"].ShouldBe(dtoFields["ObjectBoundsSecond"]);
        spriggitFields["PreviewTransform"].ShouldBe(dtoFields["PreviewTransform"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "STAT")]
    [Trait("FormKey", "1B4AC1:Fallout4.esm")]
    [Trait("EditorID", "workshop_JunkWallDoor01A")]
    [Trait("SpriggitFile", "Statics/workshop_JunkWallDoor01A - 1B4AC1_Fallout4.esm.yaml")]
    public void Fallout4_STAT_ShouldMatchSpriggitSample_workshop_JunkWallDoor01A()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Static,
            "workshop_JunkWallDoor01A");
        var dto = Helpers.GetDTO<StaticDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Static,
            "1B4AC1:Fallout4.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["LeafAmplitude"].ShouldBe(dtoFields["LeafAmplitude"]);
        spriggitFields["LeafFrequency"].ShouldBe(dtoFields["LeafFrequency"]);
        spriggitFields["MaxAngle"].ShouldBe(dtoFields["MaxAngle"]);
        spriggitFields["Model.Data"].ShouldBe(dtoFields["Model.Data"]);
        spriggitFields["Model.File"].ShouldBe(dtoFields["Models[0].File"]);
        spriggitFields["Name.Count"].ShouldBe(dtoFields["Name.Count"]);
        spriggitFields["Name.TargetLanguage"].ShouldBe(dtoFields["Name.TargetLanguage"]);
        spriggitFields["Name[0].Language"].ShouldBe(dtoFields["Name[0].Language"]);
        spriggitFields["Name[0].String"].ShouldBe(dtoFields["Name[0].String"]);
        spriggitFields["Name[1].Language"].ShouldBe(dtoFields["Name[1].Language"]);
        spriggitFields["Name[1].String"].ShouldBe(dtoFields["Name[1].String"]);
        spriggitFields["Name[10].Language"].ShouldBe(dtoFields["Name[10].Language"]);
        spriggitFields["Name[10].String"].ShouldBe(dtoFields["Name[10].String"]);
        spriggitFields["Name[2].Language"].ShouldBe(dtoFields["Name[2].Language"]);
        spriggitFields["Name[2].String"].ShouldBe(dtoFields["Name[2].String"]);
        spriggitFields["Name[3].Language"].ShouldBe(dtoFields["Name[3].Language"]);
        spriggitFields["Name[3].String"].ShouldBe(dtoFields["Name[3].String"]);
        spriggitFields["Name[4].Language"].ShouldBe(dtoFields["Name[4].Language"]);
        spriggitFields["Name[4].String"].ShouldBe(dtoFields["Name[4].String"]);
        spriggitFields["Name[5].Language"].ShouldBe(dtoFields["Name[5].Language"]);
        spriggitFields["Name[5].String"].ShouldBe(dtoFields["Name[5].String"]);
        spriggitFields["Name[6].Language"].ShouldBe(dtoFields["Name[6].Language"]);
        spriggitFields["Name[6].String"].ShouldBe(dtoFields["Name[6].String"]);
        spriggitFields["Name[7].Language"].ShouldBe(dtoFields["Name[7].Language"]);
        spriggitFields["Name[7].String"].ShouldBe(dtoFields["Name[7].String"]);
        spriggitFields["Name[8].Language"].ShouldBe(dtoFields["Name[8].Language"]);
        spriggitFields["Name[8].String"].ShouldBe(dtoFields["Name[8].String"]);
        spriggitFields["Name[9].Language"].ShouldBe(dtoFields["Name[9].Language"]);
        spriggitFields["Name[9].String"].ShouldBe(dtoFields["Name[9].String"]);
        spriggitFields["NavmeshGeometry.Count"].ShouldBe(dtoFields["NavmeshGeometry.Count"]);
        spriggitFields["NavmeshGeometry.GridArrays.Count"].ShouldBe(dtoFields["NavmeshGeometry.GridArrays.Count"]);
        spriggitFields["NavmeshGeometry.GridArrays[0]"].ShouldBe(dtoFields["NavmeshGeometry.GridArrays[0]"]);
        spriggitFields["NavmeshGeometry.GridArrays[1]"].ShouldBe(dtoFields["NavmeshGeometry.GridArrays[1]"]);
        spriggitFields["NavmeshGeometry.GridArrays[2]"].ShouldBe(dtoFields["NavmeshGeometry.GridArrays[2]"]);
        spriggitFields["NavmeshGeometry.GridArrays[3]"].ShouldBe(dtoFields["NavmeshGeometry.GridArrays[3]"]);
        spriggitFields["NavmeshGeometry.GridArrays[4]"].ShouldBe(dtoFields["NavmeshGeometry.GridArrays[4]"]);
        spriggitFields["NavmeshGeometry.GridArrays[5]"].ShouldBe(dtoFields["NavmeshGeometry.GridArrays[5]"]);
        spriggitFields["NavmeshGeometry.GridArrays[6]"].ShouldBe(dtoFields["NavmeshGeometry.GridArrays[6]"]);
        spriggitFields["NavmeshGeometry.GridArrays[7]"].ShouldBe(dtoFields["NavmeshGeometry.GridArrays[7]"]);
        spriggitFields["NavmeshGeometry.GridMax"].ShouldBe(dtoFields["NavmeshGeometry.GridMax"]);
        spriggitFields["NavmeshGeometry.GridMaxDistance"].ShouldBe(dtoFields["NavmeshGeometry.GridMaxDistance"]);
        spriggitFields["NavmeshGeometry.GridMin"].ShouldBe(dtoFields["NavmeshGeometry.GridMin"]);
        spriggitFields["NavmeshGeometry.GridSize"].ShouldBe(dtoFields["NavmeshGeometry.GridSize"]);
        spriggitFields["NavmeshGeometry.Parent.MutagenObjectType"].ShouldBe(dtoFields["NavmeshGeometry.Parent.MutagenObjectType"]);
        spriggitFields["NavmeshGeometry.Parent.Parent"].ShouldBe(dtoFields["NavmeshGeometry.Parent.Parent"]);
        spriggitFields["NavmeshGeometry[0]"].ShouldBe(dtoFields["NavmeshGeometry[0]"]);
        spriggitFields["NavmeshGeometry[1]"].ShouldBe(dtoFields["NavmeshGeometry[1]"]);
        spriggitFields["NavmeshGeometry[10].Count"].ShouldBe(dtoFields["NavmeshGeometry[10].Count"]);
        spriggitFields["NavmeshGeometry[10].EdgeLink_0_1"].ShouldBe(dtoFields["NavmeshGeometry[10].EdgeLink_0_1"]);
        spriggitFields["NavmeshGeometry[10].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[10].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[10].EdgeLink_2_0"].ShouldBe(dtoFields["NavmeshGeometry[10].EdgeLink_2_0"]);
        spriggitFields["NavmeshGeometry[10].Height"].ShouldBe(dtoFields["NavmeshGeometry[10].Height"]);
        spriggitFields["NavmeshGeometry[10].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[10].Vertices"]);
        spriggitFields["NavmeshGeometry[10][0]"].ShouldBe(dtoFields["NavmeshGeometry[10][0]"]);
        spriggitFields["NavmeshGeometry[11].Count"].ShouldBe(dtoFields["NavmeshGeometry[11].Count"]);
        spriggitFields["NavmeshGeometry[11].EdgeLink_0_1"].ShouldBe(dtoFields["NavmeshGeometry[11].EdgeLink_0_1"]);
        spriggitFields["NavmeshGeometry[11].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[11].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[11].EdgeLink_2_0"].ShouldBe(dtoFields["NavmeshGeometry[11].EdgeLink_2_0"]);
        spriggitFields["NavmeshGeometry[11].Height"].ShouldBe(dtoFields["NavmeshGeometry[11].Height"]);
        spriggitFields["NavmeshGeometry[11].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[11].Vertices"]);
        spriggitFields["NavmeshGeometry[11][0]"].ShouldBe(dtoFields["NavmeshGeometry[11][0]"]);
        spriggitFields["NavmeshGeometry[12].Count"].ShouldBe(dtoFields["NavmeshGeometry[12].Count"]);
        spriggitFields["NavmeshGeometry[12].EdgeLink_0_1"].ShouldBe(dtoFields["NavmeshGeometry[12].EdgeLink_0_1"]);
        spriggitFields["NavmeshGeometry[12].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[12].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[12].Height"].ShouldBe(dtoFields["NavmeshGeometry[12].Height"]);
        spriggitFields["NavmeshGeometry[12].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[12].Vertices"]);
        spriggitFields["NavmeshGeometry[12][0]"].ShouldBe(dtoFields["NavmeshGeometry[12][0]"]);
        spriggitFields["NavmeshGeometry[13].Count"].ShouldBe(dtoFields["NavmeshGeometry[13].Count"]);
        spriggitFields["NavmeshGeometry[13].EdgeLink_0_1"].ShouldBe(dtoFields["NavmeshGeometry[13].EdgeLink_0_1"]);
        spriggitFields["NavmeshGeometry[13].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[13].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[13].EdgeLink_2_0"].ShouldBe(dtoFields["NavmeshGeometry[13].EdgeLink_2_0"]);
        spriggitFields["NavmeshGeometry[13].Height"].ShouldBe(dtoFields["NavmeshGeometry[13].Height"]);
        spriggitFields["NavmeshGeometry[13].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[13].Vertices"]);
        spriggitFields["NavmeshGeometry[13][0]"].ShouldBe(dtoFields["NavmeshGeometry[13][0]"]);
        spriggitFields["NavmeshGeometry[2]"].ShouldBe(dtoFields["NavmeshGeometry[2]"]);
        spriggitFields["NavmeshGeometry[3]"].ShouldBe(dtoFields["NavmeshGeometry[3]"]);
        spriggitFields["NavmeshGeometry[4]"].ShouldBe(dtoFields["NavmeshGeometry[4]"]);
        spriggitFields["NavmeshGeometry[5]"].ShouldBe(dtoFields["NavmeshGeometry[5]"]);
        spriggitFields["NavmeshGeometry[6]"].ShouldBe(dtoFields["NavmeshGeometry[6]"]);
        spriggitFields["NavmeshGeometry[7]"].ShouldBe(dtoFields["NavmeshGeometry[7]"]);
        spriggitFields["NavmeshGeometry[8].Count"].ShouldBe(dtoFields["NavmeshGeometry[8].Count"]);
        spriggitFields["NavmeshGeometry[8].EdgeLink_0_1"].ShouldBe(dtoFields["NavmeshGeometry[8].EdgeLink_0_1"]);
        spriggitFields["NavmeshGeometry[8].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[8].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[8].EdgeLink_2_0"].ShouldBe(dtoFields["NavmeshGeometry[8].EdgeLink_2_0"]);
        spriggitFields["NavmeshGeometry[8].Height"].ShouldBe(dtoFields["NavmeshGeometry[8].Height"]);
        spriggitFields["NavmeshGeometry[8].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[8].Vertices"]);
        spriggitFields["NavmeshGeometry[8][0]"].ShouldBe(dtoFields["NavmeshGeometry[8][0]"]);
        spriggitFields["NavmeshGeometry[9].Count"].ShouldBe(dtoFields["NavmeshGeometry[9].Count"]);
        spriggitFields["NavmeshGeometry[9].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[9].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[9].EdgeLink_2_0"].ShouldBe(dtoFields["NavmeshGeometry[9].EdgeLink_2_0"]);
        spriggitFields["NavmeshGeometry[9].Height"].ShouldBe(dtoFields["NavmeshGeometry[9].Height"]);
        spriggitFields["NavmeshGeometry[9].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[9].Vertices"]);
        spriggitFields["NavmeshGeometry[9][0]"].ShouldBe(dtoFields["NavmeshGeometry[9][0]"]);
        spriggitFields["ObjectBounds.First"].ShouldBe(dtoFields["ObjectBoundsFirst"]);
        spriggitFields["ObjectBounds.Second"].ShouldBe(dtoFields["ObjectBoundsSecond"]);
        spriggitFields["PreviewTransform"].ShouldBe(dtoFields["PreviewTransform"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "STAT")]
    [Trait("FormKey", "0EC532:Fallout4.esm")]
    [Trait("EditorID", "workshop_ShackBalconyStairs01")]
    [Trait("SpriggitFile", "Statics/workshop_ShackBalconyStairs01 - 0EC532_Fallout4.esm.yaml")]
    public void Fallout4_STAT_ShouldMatchSpriggitSample_workshop_ShackBalconyStairs01()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Static,
            "workshop_ShackBalconyStairs01");
        var dto = Helpers.GetDTO<StaticDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Static,
            "0EC532:Fallout4.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["LeafAmplitude"].ShouldBe(dtoFields["LeafAmplitude"]);
        spriggitFields["LeafFrequency"].ShouldBe(dtoFields["LeafFrequency"]);
        spriggitFields["MaxAngle"].ShouldBe(dtoFields["MaxAngle"]);
        spriggitFields["Model.Data"].ShouldBe(dtoFields["Model.Data"]);
        spriggitFields["Model.File"].ShouldBe(dtoFields["Models[0].File"]);
        spriggitFields["Name.Count"].ShouldBe(dtoFields["Name.Count"]);
        spriggitFields["Name.TargetLanguage"].ShouldBe(dtoFields["Name.TargetLanguage"]);
        spriggitFields["Name[0].Language"].ShouldBe(dtoFields["Name[0].Language"]);
        spriggitFields["Name[0].String"].ShouldBe(dtoFields["Name[0].String"]);
        spriggitFields["Name[1].Language"].ShouldBe(dtoFields["Name[1].Language"]);
        spriggitFields["Name[1].String"].ShouldBe(dtoFields["Name[1].String"]);
        spriggitFields["Name[10].Language"].ShouldBe(dtoFields["Name[10].Language"]);
        spriggitFields["Name[10].String"].ShouldBe(dtoFields["Name[10].String"]);
        spriggitFields["Name[2].Language"].ShouldBe(dtoFields["Name[2].Language"]);
        spriggitFields["Name[2].String"].ShouldBe(dtoFields["Name[2].String"]);
        spriggitFields["Name[3].Language"].ShouldBe(dtoFields["Name[3].Language"]);
        spriggitFields["Name[3].String"].ShouldBe(dtoFields["Name[3].String"]);
        spriggitFields["Name[4].Language"].ShouldBe(dtoFields["Name[4].Language"]);
        spriggitFields["Name[4].String"].ShouldBe(dtoFields["Name[4].String"]);
        spriggitFields["Name[5].Language"].ShouldBe(dtoFields["Name[5].Language"]);
        spriggitFields["Name[5].String"].ShouldBe(dtoFields["Name[5].String"]);
        spriggitFields["Name[6].Language"].ShouldBe(dtoFields["Name[6].Language"]);
        spriggitFields["Name[6].String"].ShouldBe(dtoFields["Name[6].String"]);
        spriggitFields["Name[7].Language"].ShouldBe(dtoFields["Name[7].Language"]);
        spriggitFields["Name[7].String"].ShouldBe(dtoFields["Name[7].String"]);
        spriggitFields["Name[8].Language"].ShouldBe(dtoFields["Name[8].Language"]);
        spriggitFields["Name[8].String"].ShouldBe(dtoFields["Name[8].String"]);
        spriggitFields["Name[9].Language"].ShouldBe(dtoFields["Name[9].Language"]);
        spriggitFields["Name[9].String"].ShouldBe(dtoFields["Name[9].String"]);
        spriggitFields["NavmeshGeometry.Count"].ShouldBe(dtoFields["NavmeshGeometry.Count"]);
        spriggitFields["NavmeshGeometry.GridArrays.Count"].ShouldBe(dtoFields["NavmeshGeometry.GridArrays.Count"]);
        spriggitFields["NavmeshGeometry.GridArrays[0]"].ShouldBe(dtoFields["NavmeshGeometry.GridArrays[0]"]);
        spriggitFields["NavmeshGeometry.GridArrays[1]"].ShouldBe(dtoFields["NavmeshGeometry.GridArrays[1]"]);
        spriggitFields["NavmeshGeometry.GridArrays[10]"].ShouldBe(dtoFields["NavmeshGeometry.GridArrays[10]"]);
        spriggitFields["NavmeshGeometry.GridArrays[11]"].ShouldBe(dtoFields["NavmeshGeometry.GridArrays[11]"]);
        spriggitFields["NavmeshGeometry.GridArrays[12]"].ShouldBe(dtoFields["NavmeshGeometry.GridArrays[12]"]);
        spriggitFields["NavmeshGeometry.GridArrays[13]"].ShouldBe(dtoFields["NavmeshGeometry.GridArrays[13]"]);
        spriggitFields["NavmeshGeometry.GridArrays[14]"].ShouldBe(dtoFields["NavmeshGeometry.GridArrays[14]"]);
        spriggitFields["NavmeshGeometry.GridArrays[15]"].ShouldBe(dtoFields["NavmeshGeometry.GridArrays[15]"]);
        spriggitFields["NavmeshGeometry.GridArrays[16]"].ShouldBe(dtoFields["NavmeshGeometry.GridArrays[16]"]);
        spriggitFields["NavmeshGeometry.GridArrays[17]"].ShouldBe(dtoFields["NavmeshGeometry.GridArrays[17]"]);
        spriggitFields["NavmeshGeometry.GridArrays[18]"].ShouldBe(dtoFields["NavmeshGeometry.GridArrays[18]"]);
        spriggitFields["NavmeshGeometry.GridArrays[19]"].ShouldBe(dtoFields["NavmeshGeometry.GridArrays[19]"]);
        spriggitFields["NavmeshGeometry.GridArrays[2]"].ShouldBe(dtoFields["NavmeshGeometry.GridArrays[2]"]);
        spriggitFields["NavmeshGeometry.GridArrays[20]"].ShouldBe(dtoFields["NavmeshGeometry.GridArrays[20]"]);
        spriggitFields["NavmeshGeometry.GridArrays[21]"].ShouldBe(dtoFields["NavmeshGeometry.GridArrays[21]"]);
        spriggitFields["NavmeshGeometry.GridArrays[22]"].ShouldBe(dtoFields["NavmeshGeometry.GridArrays[22]"]);
        spriggitFields["NavmeshGeometry.GridArrays[23]"].ShouldBe(dtoFields["NavmeshGeometry.GridArrays[23]"]);
        spriggitFields["NavmeshGeometry.GridArrays[24]"].ShouldBe(dtoFields["NavmeshGeometry.GridArrays[24]"]);
        spriggitFields["NavmeshGeometry.GridArrays[25]"].ShouldBe(dtoFields["NavmeshGeometry.GridArrays[25]"]);
        spriggitFields["NavmeshGeometry.GridArrays[26]"].ShouldBe(dtoFields["NavmeshGeometry.GridArrays[26]"]);
        spriggitFields["NavmeshGeometry.GridArrays[27]"].ShouldBe(dtoFields["NavmeshGeometry.GridArrays[27]"]);
        spriggitFields["NavmeshGeometry.GridArrays[28]"].ShouldBe(dtoFields["NavmeshGeometry.GridArrays[28]"]);
        spriggitFields["NavmeshGeometry.GridArrays[29]"].ShouldBe(dtoFields["NavmeshGeometry.GridArrays[29]"]);
        spriggitFields["NavmeshGeometry.GridArrays[3]"].ShouldBe(dtoFields["NavmeshGeometry.GridArrays[3]"]);
        spriggitFields["NavmeshGeometry.GridArrays[30]"].ShouldBe(dtoFields["NavmeshGeometry.GridArrays[30]"]);
        spriggitFields["NavmeshGeometry.GridArrays[31]"].ShouldBe(dtoFields["NavmeshGeometry.GridArrays[31]"]);
        spriggitFields["NavmeshGeometry.GridArrays[32]"].ShouldBe(dtoFields["NavmeshGeometry.GridArrays[32]"]);
        spriggitFields["NavmeshGeometry.GridArrays[33]"].ShouldBe(dtoFields["NavmeshGeometry.GridArrays[33]"]);
        spriggitFields["NavmeshGeometry.GridArrays[34]"].ShouldBe(dtoFields["NavmeshGeometry.GridArrays[34]"]);
        spriggitFields["NavmeshGeometry.GridArrays[35]"].ShouldBe(dtoFields["NavmeshGeometry.GridArrays[35]"]);
        spriggitFields["NavmeshGeometry.GridArrays[36]"].ShouldBe(dtoFields["NavmeshGeometry.GridArrays[36]"]);
        spriggitFields["NavmeshGeometry.GridArrays[37]"].ShouldBe(dtoFields["NavmeshGeometry.GridArrays[37]"]);
        spriggitFields["NavmeshGeometry.GridArrays[38]"].ShouldBe(dtoFields["NavmeshGeometry.GridArrays[38]"]);
        spriggitFields["NavmeshGeometry.GridArrays[39]"].ShouldBe(dtoFields["NavmeshGeometry.GridArrays[39]"]);
        spriggitFields["NavmeshGeometry.GridArrays[4]"].ShouldBe(dtoFields["NavmeshGeometry.GridArrays[4]"]);
        spriggitFields["NavmeshGeometry.GridArrays[40]"].ShouldBe(dtoFields["NavmeshGeometry.GridArrays[40]"]);
        spriggitFields["NavmeshGeometry.GridArrays[5]"].ShouldBe(dtoFields["NavmeshGeometry.GridArrays[5]"]);
        spriggitFields["NavmeshGeometry.GridArrays[6]"].ShouldBe(dtoFields["NavmeshGeometry.GridArrays[6]"]);
        spriggitFields["NavmeshGeometry.GridArrays[7]"].ShouldBe(dtoFields["NavmeshGeometry.GridArrays[7]"]);
        spriggitFields["NavmeshGeometry.GridArrays[8]"].ShouldBe(dtoFields["NavmeshGeometry.GridArrays[8]"]);
        spriggitFields["NavmeshGeometry.GridArrays[9]"].ShouldBe(dtoFields["NavmeshGeometry.GridArrays[9]"]);
        spriggitFields["NavmeshGeometry.GridMax"].ShouldBe(dtoFields["NavmeshGeometry.GridMax"]);
        spriggitFields["NavmeshGeometry.GridMaxDistance"].ShouldBe(dtoFields["NavmeshGeometry.GridMaxDistance"]);
        spriggitFields["NavmeshGeometry.GridMin"].ShouldBe(dtoFields["NavmeshGeometry.GridMin"]);
        spriggitFields["NavmeshGeometry.GridSize"].ShouldBe(dtoFields["NavmeshGeometry.GridSize"]);
        spriggitFields["NavmeshGeometry.Parent.MutagenObjectType"].ShouldBe(dtoFields["NavmeshGeometry.Parent.MutagenObjectType"]);
        spriggitFields["NavmeshGeometry.Parent.Parent"].ShouldBe(dtoFields["NavmeshGeometry.Parent.Parent"]);
        spriggitFields["NavmeshGeometry[0]"].ShouldBe(dtoFields["NavmeshGeometry[0]"]);
        spriggitFields["NavmeshGeometry[1]"].ShouldBe(dtoFields["NavmeshGeometry[1]"]);
        spriggitFields["NavmeshGeometry[10]"].ShouldBe(dtoFields["NavmeshGeometry[10]"]);
        spriggitFields["NavmeshGeometry[11]"].ShouldBe(dtoFields["NavmeshGeometry[11]"]);
        spriggitFields["NavmeshGeometry[12]"].ShouldBe(dtoFields["NavmeshGeometry[12]"]);
        spriggitFields["NavmeshGeometry[13]"].ShouldBe(dtoFields["NavmeshGeometry[13]"]);
        spriggitFields["NavmeshGeometry[14]"].ShouldBe(dtoFields["NavmeshGeometry[14]"]);
        spriggitFields["NavmeshGeometry[15]"].ShouldBe(dtoFields["NavmeshGeometry[15]"]);
        spriggitFields["NavmeshGeometry[16].Count"].ShouldBe(dtoFields["NavmeshGeometry[16].Count"]);
        spriggitFields["NavmeshGeometry[16].CoverFlags"].ShouldBe(dtoFields["NavmeshGeometry[16].CoverFlags"]);
        spriggitFields["NavmeshGeometry[16].EdgeLink_0_1"].ShouldBe(dtoFields["NavmeshGeometry[16].EdgeLink_0_1"]);
        spriggitFields["NavmeshGeometry[16].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[16].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[16].EdgeLink_2_0"].ShouldBe(dtoFields["NavmeshGeometry[16].EdgeLink_2_0"]);
        spriggitFields["NavmeshGeometry[16].Height"].ShouldBe(dtoFields["NavmeshGeometry[16].Height"]);
        spriggitFields["NavmeshGeometry[16].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[16].Vertices"]);
        spriggitFields["NavmeshGeometry[16][0]"].ShouldBe(dtoFields["NavmeshGeometry[16][0]"]);
        spriggitFields["NavmeshGeometry[17].Count"].ShouldBe(dtoFields["NavmeshGeometry[17].Count"]);
        spriggitFields["NavmeshGeometry[17].CoverFlags"].ShouldBe(dtoFields["NavmeshGeometry[17].CoverFlags"]);
        spriggitFields["NavmeshGeometry[17].EdgeLink_0_1"].ShouldBe(dtoFields["NavmeshGeometry[17].EdgeLink_0_1"]);
        spriggitFields["NavmeshGeometry[17].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[17].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[17].EdgeLink_2_0"].ShouldBe(dtoFields["NavmeshGeometry[17].EdgeLink_2_0"]);
        spriggitFields["NavmeshGeometry[17].Height"].ShouldBe(dtoFields["NavmeshGeometry[17].Height"]);
        spriggitFields["NavmeshGeometry[17].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[17].Vertices"]);
        spriggitFields["NavmeshGeometry[17][0]"].ShouldBe(dtoFields["NavmeshGeometry[17][0]"]);
        spriggitFields["NavmeshGeometry[18].Count"].ShouldBe(dtoFields["NavmeshGeometry[18].Count"]);
        spriggitFields["NavmeshGeometry[18].CoverFlags"].ShouldBe(dtoFields["NavmeshGeometry[18].CoverFlags"]);
        spriggitFields["NavmeshGeometry[18].EdgeLink_0_1"].ShouldBe(dtoFields["NavmeshGeometry[18].EdgeLink_0_1"]);
        spriggitFields["NavmeshGeometry[18].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[18].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[18].EdgeLink_2_0"].ShouldBe(dtoFields["NavmeshGeometry[18].EdgeLink_2_0"]);
        spriggitFields["NavmeshGeometry[18].Height"].ShouldBe(dtoFields["NavmeshGeometry[18].Height"]);
        spriggitFields["NavmeshGeometry[18].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[18].Vertices"]);
        spriggitFields["NavmeshGeometry[18][0]"].ShouldBe(dtoFields["NavmeshGeometry[18][0]"]);
        spriggitFields["NavmeshGeometry[19].Count"].ShouldBe(dtoFields["NavmeshGeometry[19].Count"]);
        spriggitFields["NavmeshGeometry[19].CoverFlags"].ShouldBe(dtoFields["NavmeshGeometry[19].CoverFlags"]);
        spriggitFields["NavmeshGeometry[19].EdgeLink_0_1"].ShouldBe(dtoFields["NavmeshGeometry[19].EdgeLink_0_1"]);
        spriggitFields["NavmeshGeometry[19].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[19].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[19].EdgeLink_2_0"].ShouldBe(dtoFields["NavmeshGeometry[19].EdgeLink_2_0"]);
        spriggitFields["NavmeshGeometry[19].Height"].ShouldBe(dtoFields["NavmeshGeometry[19].Height"]);
        spriggitFields["NavmeshGeometry[19].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[19].Vertices"]);
        spriggitFields["NavmeshGeometry[19][0]"].ShouldBe(dtoFields["NavmeshGeometry[19][0]"]);
        spriggitFields["NavmeshGeometry[2]"].ShouldBe(dtoFields["NavmeshGeometry[2]"]);
        spriggitFields["NavmeshGeometry[20].EdgeLink_0_1"].ShouldBe(dtoFields["NavmeshGeometry[20].EdgeLink_0_1"]);
        spriggitFields["NavmeshGeometry[20].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[20].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[20].Height"].ShouldBe(dtoFields["NavmeshGeometry[20].Height"]);
        spriggitFields["NavmeshGeometry[20].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[20].Vertices"]);
        spriggitFields["NavmeshGeometry[21].CoverFlags"].ShouldBe(dtoFields["NavmeshGeometry[21].CoverFlags"]);
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
        spriggitFields["NavmeshGeometry[27].EdgeLink_0_1"].ShouldBe(dtoFields["NavmeshGeometry[27].EdgeLink_0_1"]);
        spriggitFields["NavmeshGeometry[27].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[27].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[27].EdgeLink_2_0"].ShouldBe(dtoFields["NavmeshGeometry[27].EdgeLink_2_0"]);
        spriggitFields["NavmeshGeometry[27].Height"].ShouldBe(dtoFields["NavmeshGeometry[27].Height"]);
        spriggitFields["NavmeshGeometry[27].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[27].Vertices"]);
        spriggitFields["NavmeshGeometry[28].CoverFlags"].ShouldBe(dtoFields["NavmeshGeometry[28].CoverFlags"]);
        spriggitFields["NavmeshGeometry[28].EdgeLink_0_1"].ShouldBe(dtoFields["NavmeshGeometry[28].EdgeLink_0_1"]);
        spriggitFields["NavmeshGeometry[28].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[28].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[28].EdgeLink_2_0"].ShouldBe(dtoFields["NavmeshGeometry[28].EdgeLink_2_0"]);
        spriggitFields["NavmeshGeometry[28].Height"].ShouldBe(dtoFields["NavmeshGeometry[28].Height"]);
        spriggitFields["NavmeshGeometry[28].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[28].Vertices"]);
        spriggitFields["NavmeshGeometry[29].EdgeLink_0_1"].ShouldBe(dtoFields["NavmeshGeometry[29].EdgeLink_0_1"]);
        spriggitFields["NavmeshGeometry[29].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[29].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[29].EdgeLink_2_0"].ShouldBe(dtoFields["NavmeshGeometry[29].EdgeLink_2_0"]);
        spriggitFields["NavmeshGeometry[29].Height"].ShouldBe(dtoFields["NavmeshGeometry[29].Height"]);
        spriggitFields["NavmeshGeometry[29].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[29].Vertices"]);
        spriggitFields["NavmeshGeometry[3]"].ShouldBe(dtoFields["NavmeshGeometry[3]"]);
        spriggitFields["NavmeshGeometry[30].EdgeLink_0_1"].ShouldBe(dtoFields["NavmeshGeometry[30].EdgeLink_0_1"]);
        spriggitFields["NavmeshGeometry[30].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[30].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[30].EdgeLink_2_0"].ShouldBe(dtoFields["NavmeshGeometry[30].EdgeLink_2_0"]);
        spriggitFields["NavmeshGeometry[30].Height"].ShouldBe(dtoFields["NavmeshGeometry[30].Height"]);
        spriggitFields["NavmeshGeometry[30].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[30].Vertices"]);
        spriggitFields["NavmeshGeometry[31].CoverFlags"].ShouldBe(dtoFields["NavmeshGeometry[31].CoverFlags"]);
        spriggitFields["NavmeshGeometry[31].EdgeLink_0_1"].ShouldBe(dtoFields["NavmeshGeometry[31].EdgeLink_0_1"]);
        spriggitFields["NavmeshGeometry[31].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[31].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[31].EdgeLink_2_0"].ShouldBe(dtoFields["NavmeshGeometry[31].EdgeLink_2_0"]);
        spriggitFields["NavmeshGeometry[31].Height"].ShouldBe(dtoFields["NavmeshGeometry[31].Height"]);
        spriggitFields["NavmeshGeometry[31].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[31].Vertices"]);
        spriggitFields["NavmeshGeometry[32].EdgeLink_0_1"].ShouldBe(dtoFields["NavmeshGeometry[32].EdgeLink_0_1"]);
        spriggitFields["NavmeshGeometry[32].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[32].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[32].EdgeLink_2_0"].ShouldBe(dtoFields["NavmeshGeometry[32].EdgeLink_2_0"]);
        spriggitFields["NavmeshGeometry[32].Height"].ShouldBe(dtoFields["NavmeshGeometry[32].Height"]);
        spriggitFields["NavmeshGeometry[32].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[32].Vertices"]);
        spriggitFields["NavmeshGeometry[33].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[33].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[33].EdgeLink_2_0"].ShouldBe(dtoFields["NavmeshGeometry[33].EdgeLink_2_0"]);
        spriggitFields["NavmeshGeometry[33].Height"].ShouldBe(dtoFields["NavmeshGeometry[33].Height"]);
        spriggitFields["NavmeshGeometry[33].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[33].Vertices"]);
        spriggitFields["NavmeshGeometry[34].Data"].ShouldBe(dtoFields["NavmeshGeometry[34].Data"]);
        spriggitFields["NavmeshGeometry[34].Vertex2"].ShouldBe(dtoFields["NavmeshGeometry[34].Vertex2"]);
        spriggitFields["NavmeshGeometry[35].Data"].ShouldBe(dtoFields["NavmeshGeometry[35].Data"]);
        spriggitFields["NavmeshGeometry[35].Vertex1"].ShouldBe(dtoFields["NavmeshGeometry[35].Vertex1"]);
        spriggitFields["NavmeshGeometry[35].Vertex2"].ShouldBe(dtoFields["NavmeshGeometry[35].Vertex2"]);
        spriggitFields["NavmeshGeometry[36].Data"].ShouldBe(dtoFields["NavmeshGeometry[36].Data"]);
        spriggitFields["NavmeshGeometry[36].Vertex1"].ShouldBe(dtoFields["NavmeshGeometry[36].Vertex1"]);
        spriggitFields["NavmeshGeometry[36].Vertex2"].ShouldBe(dtoFields["NavmeshGeometry[36].Vertex2"]);
        spriggitFields["NavmeshGeometry[37].Data"].ShouldBe(dtoFields["NavmeshGeometry[37].Data"]);
        spriggitFields["NavmeshGeometry[37].Vertex1"].ShouldBe(dtoFields["NavmeshGeometry[37].Vertex1"]);
        spriggitFields["NavmeshGeometry[37].Vertex2"].ShouldBe(dtoFields["NavmeshGeometry[37].Vertex2"]);
        spriggitFields["NavmeshGeometry[38].Data"].ShouldBe(dtoFields["NavmeshGeometry[38].Data"]);
        spriggitFields["NavmeshGeometry[38].Vertex1"].ShouldBe(dtoFields["NavmeshGeometry[38].Vertex1"]);
        spriggitFields["NavmeshGeometry[38].Vertex2"].ShouldBe(dtoFields["NavmeshGeometry[38].Vertex2"]);
        spriggitFields["NavmeshGeometry[39].Data"].ShouldBe(dtoFields["NavmeshGeometry[39].Data"]);
        spriggitFields["NavmeshGeometry[39].Vertex1"].ShouldBe(dtoFields["NavmeshGeometry[39].Vertex1"]);
        spriggitFields["NavmeshGeometry[39].Vertex2"].ShouldBe(dtoFields["NavmeshGeometry[39].Vertex2"]);
        spriggitFields["NavmeshGeometry[4]"].ShouldBe(dtoFields["NavmeshGeometry[4]"]);
        spriggitFields["NavmeshGeometry[40].Data"].ShouldBe(dtoFields["NavmeshGeometry[40].Data"]);
        spriggitFields["NavmeshGeometry[40].Vertex1"].ShouldBe(dtoFields["NavmeshGeometry[40].Vertex1"]);
        spriggitFields["NavmeshGeometry[40].Vertex2"].ShouldBe(dtoFields["NavmeshGeometry[40].Vertex2"]);
        spriggitFields["NavmeshGeometry[41].Data"].ShouldBe(dtoFields["NavmeshGeometry[41].Data"]);
        spriggitFields["NavmeshGeometry[41].Vertex1"].ShouldBe(dtoFields["NavmeshGeometry[41].Vertex1"]);
        spriggitFields["NavmeshGeometry[42]"].ShouldBe(dtoFields["NavmeshGeometry[42]"]);
        spriggitFields["NavmeshGeometry[43].Cover"].ShouldBe(dtoFields["NavmeshGeometry[43].Cover"]);
        spriggitFields["NavmeshGeometry[44].Cover"].ShouldBe(dtoFields["NavmeshGeometry[44].Cover"]);
        spriggitFields["NavmeshGeometry[44].Triangle"].ShouldBe(dtoFields["NavmeshGeometry[44].Triangle"]);
        spriggitFields["NavmeshGeometry[45].Cover"].ShouldBe(dtoFields["NavmeshGeometry[45].Cover"]);
        spriggitFields["NavmeshGeometry[45].Triangle"].ShouldBe(dtoFields["NavmeshGeometry[45].Triangle"]);
        spriggitFields["NavmeshGeometry[46].Cover"].ShouldBe(dtoFields["NavmeshGeometry[46].Cover"]);
        spriggitFields["NavmeshGeometry[46].Triangle"].ShouldBe(dtoFields["NavmeshGeometry[46].Triangle"]);
        spriggitFields["NavmeshGeometry[47].Cover"].ShouldBe(dtoFields["NavmeshGeometry[47].Cover"]);
        spriggitFields["NavmeshGeometry[47].Triangle"].ShouldBe(dtoFields["NavmeshGeometry[47].Triangle"]);
        spriggitFields["NavmeshGeometry[48].Cover"].ShouldBe(dtoFields["NavmeshGeometry[48].Cover"]);
        spriggitFields["NavmeshGeometry[48].Triangle"].ShouldBe(dtoFields["NavmeshGeometry[48].Triangle"]);
        spriggitFields["NavmeshGeometry[49].Cover"].ShouldBe(dtoFields["NavmeshGeometry[49].Cover"]);
        spriggitFields["NavmeshGeometry[49].Triangle"].ShouldBe(dtoFields["NavmeshGeometry[49].Triangle"]);
        spriggitFields["NavmeshGeometry[5]"].ShouldBe(dtoFields["NavmeshGeometry[5]"]);
        spriggitFields["NavmeshGeometry[50].Cover"].ShouldBe(dtoFields["NavmeshGeometry[50].Cover"]);
        spriggitFields["NavmeshGeometry[50].Triangle"].ShouldBe(dtoFields["NavmeshGeometry[50].Triangle"]);
        spriggitFields["NavmeshGeometry[51].Cover"].ShouldBe(dtoFields["NavmeshGeometry[51].Cover"]);
        spriggitFields["NavmeshGeometry[51].Triangle"].ShouldBe(dtoFields["NavmeshGeometry[51].Triangle"]);
        spriggitFields["NavmeshGeometry[52].Cover"].ShouldBe(dtoFields["NavmeshGeometry[52].Cover"]);
        spriggitFields["NavmeshGeometry[52].Triangle"].ShouldBe(dtoFields["NavmeshGeometry[52].Triangle"]);
        spriggitFields["NavmeshGeometry[53].Cover"].ShouldBe(dtoFields["NavmeshGeometry[53].Cover"]);
        spriggitFields["NavmeshGeometry[53].Triangle"].ShouldBe(dtoFields["NavmeshGeometry[53].Triangle"]);
        spriggitFields["NavmeshGeometry[54].Cover"].ShouldBe(dtoFields["NavmeshGeometry[54].Cover"]);
        spriggitFields["NavmeshGeometry[54].Triangle"].ShouldBe(dtoFields["NavmeshGeometry[54].Triangle"]);
        spriggitFields["NavmeshGeometry[55].Cover"].ShouldBe(dtoFields["NavmeshGeometry[55].Cover"]);
        spriggitFields["NavmeshGeometry[55].Triangle"].ShouldBe(dtoFields["NavmeshGeometry[55].Triangle"]);
        spriggitFields["NavmeshGeometry[56].Cover"].ShouldBe(dtoFields["NavmeshGeometry[56].Cover"]);
        spriggitFields["NavmeshGeometry[56].Triangle"].ShouldBe(dtoFields["NavmeshGeometry[56].Triangle"]);
        spriggitFields["NavmeshGeometry[6]"].ShouldBe(dtoFields["NavmeshGeometry[6]"]);
        spriggitFields["NavmeshGeometry[7]"].ShouldBe(dtoFields["NavmeshGeometry[7]"]);
        spriggitFields["NavmeshGeometry[8]"].ShouldBe(dtoFields["NavmeshGeometry[8]"]);
        spriggitFields["NavmeshGeometry[9]"].ShouldBe(dtoFields["NavmeshGeometry[9]"]);
        spriggitFields["ObjectBounds.First"].ShouldBe(dtoFields["ObjectBoundsFirst"]);
        spriggitFields["ObjectBounds.Second"].ShouldBe(dtoFields["ObjectBoundsSecond"]);
        spriggitFields["PreviewTransform"].ShouldBe(dtoFields["PreviewTransform"]);
        spriggitFields["Value[0]"].ShouldBe(dtoFields["Value[0]"]);
        spriggitFields["Value[1]"].ShouldBe(dtoFields["Value[1]"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "STAT")]
    [Trait("FormKey", "000032:Fallout4.esm")]
    [Trait("EditorID", "COCMarkerHeading")]
    [Trait("SpriggitFile", "Statics/COCMarkerHeading - 000032_Fallout4.esm.yaml")]
    public void Fallout4_STAT_ShouldMatchSpriggitSample_COCMarkerHeading()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Static,
            "COCMarkerHeading");
        var dto = Helpers.GetDTO<StaticDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Static,
            "000032:Fallout4.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["MajorRecordFlagsRaw"].ShouldBe(dtoFields["MajorRecordFlags"]);
        spriggitFields["MaxAngle"].ShouldBe(dtoFields["MaxAngle"]);
        spriggitFields["Model.File"].ShouldBe(dtoFields["Models[0].File"]);
        spriggitFields["ObjectBounds.First"].ShouldBe(dtoFields["ObjectBoundsFirst"]);
        spriggitFields["ObjectBounds.Second"].ShouldBe(dtoFields["ObjectBoundsSecond"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "STAT")]
    [Trait("FormKey", "000021:Fallout4.esm")]
    [Trait("EditorID", "CollisionMarker")]
    [Trait("SpriggitFile", "Statics/CollisionMarker - 000021_Fallout4.esm.yaml")]
    public void Fallout4_STAT_ShouldMatchSpriggitSample_CollisionMarker()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Static,
            "CollisionMarker");
        var dto = Helpers.GetDTO<StaticDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Static,
            "000021:Fallout4.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["MajorRecordFlagsRaw"].ShouldBe(dtoFields["MajorRecordFlags"]);
        spriggitFields["MaxAngle"].ShouldBe(dtoFields["MaxAngle"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }
}
