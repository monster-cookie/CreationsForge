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

        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "LeafAmplitude").ShouldBe(Helpers.GetDTOField(dto, "LeafAmplitude"));
        Helpers.GetSpriggitField(spriggit, "LeafFrequency").ShouldBe(Helpers.GetDTOField(dto, "LeafFrequency"));
        Helpers.GetSpriggitField(spriggit, "MaxAngle").ShouldBe(Helpers.GetDTOField(dto, "MaxAngle"));
        Helpers.GetSpriggitField(spriggit, "Model.Data").ShouldBe(Helpers.GetDTOField(dto, "Model.Data"));
        Helpers.GetSpriggitField(spriggit, "Model.File").ShouldBe(Helpers.GetDTOField(dto, "Models[0].File"));
        Helpers.GetSpriggitField(spriggit, "Name.Count").ShouldBe(Helpers.GetDTOField(dto, "Name.Count"));
        Helpers.GetSpriggitField(spriggit, "Name.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Name.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Name[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[0].String").ShouldBe(Helpers.GetDTOField(dto, "Name[0].String"));
        Helpers.GetSpriggitField(spriggit, "Name[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[1].String").ShouldBe(Helpers.GetDTOField(dto, "Name[1].String"));
        Helpers.GetSpriggitField(spriggit, "Name[10].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[10].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[10].String").ShouldBe(Helpers.GetDTOField(dto, "Name[10].String"));
        Helpers.GetSpriggitField(spriggit, "Name[2].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[2].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[2].String").ShouldBe(Helpers.GetDTOField(dto, "Name[2].String"));
        Helpers.GetSpriggitField(spriggit, "Name[3].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[3].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[3].String").ShouldBe(Helpers.GetDTOField(dto, "Name[3].String"));
        Helpers.GetSpriggitField(spriggit, "Name[4].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[4].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[4].String").ShouldBe(Helpers.GetDTOField(dto, "Name[4].String"));
        Helpers.GetSpriggitField(spriggit, "Name[5].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[5].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[5].String").ShouldBe(Helpers.GetDTOField(dto, "Name[5].String"));
        Helpers.GetSpriggitField(spriggit, "Name[6].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[6].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[6].String").ShouldBe(Helpers.GetDTOField(dto, "Name[6].String"));
        Helpers.GetSpriggitField(spriggit, "Name[7].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[7].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[7].String").ShouldBe(Helpers.GetDTOField(dto, "Name[7].String"));
        Helpers.GetSpriggitField(spriggit, "Name[8].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[8].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[8].String").ShouldBe(Helpers.GetDTOField(dto, "Name[8].String"));
        Helpers.GetSpriggitField(spriggit, "Name[9].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[9].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[9].String").ShouldBe(Helpers.GetDTOField(dto, "Name[9].String"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.Count").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.Count"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridArrays.Count").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridArrays.Count"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridArrays[0]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridArrays[0]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridArrays[1]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridArrays[1]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridArrays[2]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridArrays[2]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridArrays[3]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridArrays[3]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridArrays[4]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridArrays[4]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridArrays[5]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridArrays[5]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridArrays[6]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridArrays[6]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridArrays[7]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridArrays[7]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridMax").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridMax"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridMaxDistance").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridMaxDistance"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridMin").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridMin"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridSize").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridSize"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.Parent.MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.Parent.MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.Parent.Parent").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.Parent.Parent"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[0]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[0]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[1]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[1]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[10].Count").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[10].Count"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[10].EdgeLink_0_1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[10].EdgeLink_0_1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[10].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[10].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[10].EdgeLink_2_0").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[10].EdgeLink_2_0"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[10].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[10].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[10].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[10].Vertices"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[10][0]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[10][0]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[11].Count").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[11].Count"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[11].EdgeLink_0_1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[11].EdgeLink_0_1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[11].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[11].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[11].EdgeLink_2_0").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[11].EdgeLink_2_0"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[11].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[11].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[11].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[11].Vertices"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[11][0]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[11][0]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[12].Count").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[12].Count"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[12].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[12].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[12].EdgeLink_2_0").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[12].EdgeLink_2_0"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[12].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[12].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[12].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[12].Vertices"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[12][0]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[12][0]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[13].Count").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[13].Count"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[13].EdgeLink_0_1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[13].EdgeLink_0_1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[13].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[13].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[13].EdgeLink_2_0").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[13].EdgeLink_2_0"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[13].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[13].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[13].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[13].Vertices"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[13][0]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[13][0]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[2]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[2]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[3]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[3]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[4]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[4]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[5]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[5]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[6]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[6]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[7]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[7]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[8].Count").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[8].Count"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[8].EdgeLink_0_1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[8].EdgeLink_0_1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[8].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[8].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[8].EdgeLink_2_0").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[8].EdgeLink_2_0"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[8].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[8].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[8].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[8].Vertices"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[8][0]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[8][0]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[9].Count").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[9].Count"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[9].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[9].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[9].EdgeLink_2_0").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[9].EdgeLink_2_0"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[9].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[9].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[9].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[9].Vertices"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[9][0]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[9][0]"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.First").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsFirst"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.Second").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsSecond"));
        Helpers.GetSpriggitField(spriggit, "PreviewTransform").ShouldBe(Helpers.GetDTOField(dto, "PreviewTransform"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "EditorID", "FormKey", "LeafAmplitude", "LeafFrequency", "MaxAngle", "Model.Data", "Model.File", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "NavmeshGeometry.Count", "NavmeshGeometry.GridArrays.Count", "NavmeshGeometry.GridArrays[0]", "NavmeshGeometry.GridArrays[1]", "NavmeshGeometry.GridArrays[2]", "NavmeshGeometry.GridArrays[3]", "NavmeshGeometry.GridArrays[4]", "NavmeshGeometry.GridArrays[5]", "NavmeshGeometry.GridArrays[6]", "NavmeshGeometry.GridArrays[7]", "NavmeshGeometry.GridMax", "NavmeshGeometry.GridMaxDistance", "NavmeshGeometry.GridMin", "NavmeshGeometry.GridSize", "NavmeshGeometry.Parent.MutagenObjectType", "NavmeshGeometry.Parent.Parent", "NavmeshGeometry[0]", "NavmeshGeometry[1]", "NavmeshGeometry[10].Count", "NavmeshGeometry[10].EdgeLink_0_1", "NavmeshGeometry[10].EdgeLink_1_2", "NavmeshGeometry[10].EdgeLink_2_0", "NavmeshGeometry[10].Height", "NavmeshGeometry[10].Vertices", "NavmeshGeometry[10][0]", "NavmeshGeometry[11].Count", "NavmeshGeometry[11].EdgeLink_0_1", "NavmeshGeometry[11].EdgeLink_1_2", "NavmeshGeometry[11].EdgeLink_2_0", "NavmeshGeometry[11].Height", "NavmeshGeometry[11].Vertices", "NavmeshGeometry[11][0]", "NavmeshGeometry[12].Count", "NavmeshGeometry[12].EdgeLink_1_2", "NavmeshGeometry[12].EdgeLink_2_0", "NavmeshGeometry[12].Height", "NavmeshGeometry[12].Vertices", "NavmeshGeometry[12][0]", "NavmeshGeometry[13].Count", "NavmeshGeometry[13].EdgeLink_0_1", "NavmeshGeometry[13].EdgeLink_1_2", "NavmeshGeometry[13].EdgeLink_2_0", "NavmeshGeometry[13].Height", "NavmeshGeometry[13].Vertices", "NavmeshGeometry[13][0]", "NavmeshGeometry[2]", "NavmeshGeometry[3]", "NavmeshGeometry[4]", "NavmeshGeometry[5]", "NavmeshGeometry[6]", "NavmeshGeometry[7]", "NavmeshGeometry[8].Count", "NavmeshGeometry[8].EdgeLink_0_1", "NavmeshGeometry[8].EdgeLink_1_2", "NavmeshGeometry[8].EdgeLink_2_0", "NavmeshGeometry[8].Height", "NavmeshGeometry[8].Vertices", "NavmeshGeometry[8][0]", "NavmeshGeometry[9].Count", "NavmeshGeometry[9].EdgeLink_1_2", "NavmeshGeometry[9].EdgeLink_2_0", "NavmeshGeometry[9].Height", "NavmeshGeometry[9].Vertices", "NavmeshGeometry[9][0]", "ObjectBounds.First", "ObjectBounds.Second", "PreviewTransform", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "EditorID", "FormKey", "LeafAmplitude", "LeafFrequency", "MaxAngle", "Model.Data", "Models[0].File", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "NavmeshGeometry.Count", "NavmeshGeometry.GridArrays.Count", "NavmeshGeometry.GridArrays[0]", "NavmeshGeometry.GridArrays[1]", "NavmeshGeometry.GridArrays[2]", "NavmeshGeometry.GridArrays[3]", "NavmeshGeometry.GridArrays[4]", "NavmeshGeometry.GridArrays[5]", "NavmeshGeometry.GridArrays[6]", "NavmeshGeometry.GridArrays[7]", "NavmeshGeometry.GridMax", "NavmeshGeometry.GridMaxDistance", "NavmeshGeometry.GridMin", "NavmeshGeometry.GridSize", "NavmeshGeometry.Parent.MutagenObjectType", "NavmeshGeometry.Parent.Parent", "NavmeshGeometry[0]", "NavmeshGeometry[1]", "NavmeshGeometry[10].Count", "NavmeshGeometry[10].EdgeLink_0_1", "NavmeshGeometry[10].EdgeLink_1_2", "NavmeshGeometry[10].EdgeLink_2_0", "NavmeshGeometry[10].Height", "NavmeshGeometry[10].Vertices", "NavmeshGeometry[10][0]", "NavmeshGeometry[11].Count", "NavmeshGeometry[11].EdgeLink_0_1", "NavmeshGeometry[11].EdgeLink_1_2", "NavmeshGeometry[11].EdgeLink_2_0", "NavmeshGeometry[11].Height", "NavmeshGeometry[11].Vertices", "NavmeshGeometry[11][0]", "NavmeshGeometry[12].Count", "NavmeshGeometry[12].EdgeLink_1_2", "NavmeshGeometry[12].EdgeLink_2_0", "NavmeshGeometry[12].Height", "NavmeshGeometry[12].Vertices", "NavmeshGeometry[12][0]", "NavmeshGeometry[13].Count", "NavmeshGeometry[13].EdgeLink_0_1", "NavmeshGeometry[13].EdgeLink_1_2", "NavmeshGeometry[13].EdgeLink_2_0", "NavmeshGeometry[13].Height", "NavmeshGeometry[13].Vertices", "NavmeshGeometry[13][0]", "NavmeshGeometry[2]", "NavmeshGeometry[3]", "NavmeshGeometry[4]", "NavmeshGeometry[5]", "NavmeshGeometry[6]", "NavmeshGeometry[7]", "NavmeshGeometry[8].Count", "NavmeshGeometry[8].EdgeLink_0_1", "NavmeshGeometry[8].EdgeLink_1_2", "NavmeshGeometry[8].EdgeLink_2_0", "NavmeshGeometry[8].Height", "NavmeshGeometry[8].Vertices", "NavmeshGeometry[8][0]", "NavmeshGeometry[9].Count", "NavmeshGeometry[9].EdgeLink_1_2", "NavmeshGeometry[9].EdgeLink_2_0", "NavmeshGeometry[9].Height", "NavmeshGeometry[9].Vertices", "NavmeshGeometry[9][0]", "ObjectBoundsFirst", "ObjectBoundsSecond", "PreviewTransform", "Version2", "VersionControl");
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

        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "LeafAmplitude").ShouldBe(Helpers.GetDTOField(dto, "LeafAmplitude"));
        Helpers.GetSpriggitField(spriggit, "LeafFrequency").ShouldBe(Helpers.GetDTOField(dto, "LeafFrequency"));
        Helpers.GetSpriggitField(spriggit, "MaxAngle").ShouldBe(Helpers.GetDTOField(dto, "MaxAngle"));
        Helpers.GetSpriggitField(spriggit, "Model.Data").ShouldBe(Helpers.GetDTOField(dto, "Model.Data"));
        Helpers.GetSpriggitField(spriggit, "Model.File").ShouldBe(Helpers.GetDTOField(dto, "Models[0].File"));
        Helpers.GetSpriggitField(spriggit, "Name.Count").ShouldBe(Helpers.GetDTOField(dto, "Name.Count"));
        Helpers.GetSpriggitField(spriggit, "Name.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Name.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Name[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[0].String").ShouldBe(Helpers.GetDTOField(dto, "Name[0].String"));
        Helpers.GetSpriggitField(spriggit, "Name[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[1].String").ShouldBe(Helpers.GetDTOField(dto, "Name[1].String"));
        Helpers.GetSpriggitField(spriggit, "Name[10].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[10].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[10].String").ShouldBe(Helpers.GetDTOField(dto, "Name[10].String"));
        Helpers.GetSpriggitField(spriggit, "Name[2].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[2].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[2].String").ShouldBe(Helpers.GetDTOField(dto, "Name[2].String"));
        Helpers.GetSpriggitField(spriggit, "Name[3].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[3].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[3].String").ShouldBe(Helpers.GetDTOField(dto, "Name[3].String"));
        Helpers.GetSpriggitField(spriggit, "Name[4].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[4].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[4].String").ShouldBe(Helpers.GetDTOField(dto, "Name[4].String"));
        Helpers.GetSpriggitField(spriggit, "Name[5].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[5].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[5].String").ShouldBe(Helpers.GetDTOField(dto, "Name[5].String"));
        Helpers.GetSpriggitField(spriggit, "Name[6].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[6].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[6].String").ShouldBe(Helpers.GetDTOField(dto, "Name[6].String"));
        Helpers.GetSpriggitField(spriggit, "Name[7].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[7].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[7].String").ShouldBe(Helpers.GetDTOField(dto, "Name[7].String"));
        Helpers.GetSpriggitField(spriggit, "Name[8].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[8].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[8].String").ShouldBe(Helpers.GetDTOField(dto, "Name[8].String"));
        Helpers.GetSpriggitField(spriggit, "Name[9].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[9].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[9].String").ShouldBe(Helpers.GetDTOField(dto, "Name[9].String"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.Count").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.Count"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridArrays.Count").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridArrays.Count"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridArrays[0]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridArrays[0]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridArrays[1]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridArrays[1]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridArrays[2]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridArrays[2]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridArrays[3]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridArrays[3]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridArrays[4]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridArrays[4]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridArrays[5]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridArrays[5]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridArrays[6]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridArrays[6]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridArrays[7]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridArrays[7]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridMax").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridMax"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridMaxDistance").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridMaxDistance"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridMin").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridMin"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridSize").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridSize"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.Parent.MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.Parent.MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.Parent.Parent").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.Parent.Parent"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[0]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[0]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[1]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[1]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[10].Count").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[10].Count"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[10].EdgeLink_0_1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[10].EdgeLink_0_1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[10].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[10].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[10].EdgeLink_2_0").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[10].EdgeLink_2_0"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[10].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[10].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[10].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[10].Vertices"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[10][0]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[10][0]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[11].Count").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[11].Count"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[11].EdgeLink_0_1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[11].EdgeLink_0_1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[11].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[11].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[11].EdgeLink_2_0").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[11].EdgeLink_2_0"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[11].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[11].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[11].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[11].Vertices"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[11][0]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[11][0]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[12].Count").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[12].Count"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[12].EdgeLink_0_1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[12].EdgeLink_0_1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[12].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[12].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[12].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[12].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[12].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[12].Vertices"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[12][0]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[12][0]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[13].Count").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[13].Count"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[13].EdgeLink_0_1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[13].EdgeLink_0_1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[13].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[13].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[13].EdgeLink_2_0").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[13].EdgeLink_2_0"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[13].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[13].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[13].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[13].Vertices"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[13][0]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[13][0]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[2]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[2]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[3]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[3]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[4]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[4]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[5]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[5]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[6]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[6]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[7]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[7]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[8].Count").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[8].Count"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[8].EdgeLink_0_1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[8].EdgeLink_0_1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[8].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[8].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[8].EdgeLink_2_0").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[8].EdgeLink_2_0"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[8].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[8].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[8].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[8].Vertices"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[8][0]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[8][0]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[9].Count").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[9].Count"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[9].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[9].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[9].EdgeLink_2_0").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[9].EdgeLink_2_0"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[9].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[9].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[9].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[9].Vertices"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[9][0]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[9][0]"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.First").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsFirst"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.Second").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsSecond"));
        Helpers.GetSpriggitField(spriggit, "PreviewTransform").ShouldBe(Helpers.GetDTOField(dto, "PreviewTransform"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "EditorID", "FormKey", "LeafAmplitude", "LeafFrequency", "MaxAngle", "Model.Data", "Model.File", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "NavmeshGeometry.Count", "NavmeshGeometry.GridArrays.Count", "NavmeshGeometry.GridArrays[0]", "NavmeshGeometry.GridArrays[1]", "NavmeshGeometry.GridArrays[2]", "NavmeshGeometry.GridArrays[3]", "NavmeshGeometry.GridArrays[4]", "NavmeshGeometry.GridArrays[5]", "NavmeshGeometry.GridArrays[6]", "NavmeshGeometry.GridArrays[7]", "NavmeshGeometry.GridMax", "NavmeshGeometry.GridMaxDistance", "NavmeshGeometry.GridMin", "NavmeshGeometry.GridSize", "NavmeshGeometry.Parent.MutagenObjectType", "NavmeshGeometry.Parent.Parent", "NavmeshGeometry[0]", "NavmeshGeometry[1]", "NavmeshGeometry[10].Count", "NavmeshGeometry[10].EdgeLink_0_1", "NavmeshGeometry[10].EdgeLink_1_2", "NavmeshGeometry[10].EdgeLink_2_0", "NavmeshGeometry[10].Height", "NavmeshGeometry[10].Vertices", "NavmeshGeometry[10][0]", "NavmeshGeometry[11].Count", "NavmeshGeometry[11].EdgeLink_0_1", "NavmeshGeometry[11].EdgeLink_1_2", "NavmeshGeometry[11].EdgeLink_2_0", "NavmeshGeometry[11].Height", "NavmeshGeometry[11].Vertices", "NavmeshGeometry[11][0]", "NavmeshGeometry[12].Count", "NavmeshGeometry[12].EdgeLink_0_1", "NavmeshGeometry[12].EdgeLink_1_2", "NavmeshGeometry[12].Height", "NavmeshGeometry[12].Vertices", "NavmeshGeometry[12][0]", "NavmeshGeometry[13].Count", "NavmeshGeometry[13].EdgeLink_0_1", "NavmeshGeometry[13].EdgeLink_1_2", "NavmeshGeometry[13].EdgeLink_2_0", "NavmeshGeometry[13].Height", "NavmeshGeometry[13].Vertices", "NavmeshGeometry[13][0]", "NavmeshGeometry[2]", "NavmeshGeometry[3]", "NavmeshGeometry[4]", "NavmeshGeometry[5]", "NavmeshGeometry[6]", "NavmeshGeometry[7]", "NavmeshGeometry[8].Count", "NavmeshGeometry[8].EdgeLink_0_1", "NavmeshGeometry[8].EdgeLink_1_2", "NavmeshGeometry[8].EdgeLink_2_0", "NavmeshGeometry[8].Height", "NavmeshGeometry[8].Vertices", "NavmeshGeometry[8][0]", "NavmeshGeometry[9].Count", "NavmeshGeometry[9].EdgeLink_1_2", "NavmeshGeometry[9].EdgeLink_2_0", "NavmeshGeometry[9].Height", "NavmeshGeometry[9].Vertices", "NavmeshGeometry[9][0]", "ObjectBounds.First", "ObjectBounds.Second", "PreviewTransform", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "EditorID", "FormKey", "LeafAmplitude", "LeafFrequency", "MaxAngle", "Model.Data", "Models[0].File", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "NavmeshGeometry.Count", "NavmeshGeometry.GridArrays.Count", "NavmeshGeometry.GridArrays[0]", "NavmeshGeometry.GridArrays[1]", "NavmeshGeometry.GridArrays[2]", "NavmeshGeometry.GridArrays[3]", "NavmeshGeometry.GridArrays[4]", "NavmeshGeometry.GridArrays[5]", "NavmeshGeometry.GridArrays[6]", "NavmeshGeometry.GridArrays[7]", "NavmeshGeometry.GridMax", "NavmeshGeometry.GridMaxDistance", "NavmeshGeometry.GridMin", "NavmeshGeometry.GridSize", "NavmeshGeometry.Parent.MutagenObjectType", "NavmeshGeometry.Parent.Parent", "NavmeshGeometry[0]", "NavmeshGeometry[1]", "NavmeshGeometry[10].Count", "NavmeshGeometry[10].EdgeLink_0_1", "NavmeshGeometry[10].EdgeLink_1_2", "NavmeshGeometry[10].EdgeLink_2_0", "NavmeshGeometry[10].Height", "NavmeshGeometry[10].Vertices", "NavmeshGeometry[10][0]", "NavmeshGeometry[11].Count", "NavmeshGeometry[11].EdgeLink_0_1", "NavmeshGeometry[11].EdgeLink_1_2", "NavmeshGeometry[11].EdgeLink_2_0", "NavmeshGeometry[11].Height", "NavmeshGeometry[11].Vertices", "NavmeshGeometry[11][0]", "NavmeshGeometry[12].Count", "NavmeshGeometry[12].EdgeLink_0_1", "NavmeshGeometry[12].EdgeLink_1_2", "NavmeshGeometry[12].Height", "NavmeshGeometry[12].Vertices", "NavmeshGeometry[12][0]", "NavmeshGeometry[13].Count", "NavmeshGeometry[13].EdgeLink_0_1", "NavmeshGeometry[13].EdgeLink_1_2", "NavmeshGeometry[13].EdgeLink_2_0", "NavmeshGeometry[13].Height", "NavmeshGeometry[13].Vertices", "NavmeshGeometry[13][0]", "NavmeshGeometry[2]", "NavmeshGeometry[3]", "NavmeshGeometry[4]", "NavmeshGeometry[5]", "NavmeshGeometry[6]", "NavmeshGeometry[7]", "NavmeshGeometry[8].Count", "NavmeshGeometry[8].EdgeLink_0_1", "NavmeshGeometry[8].EdgeLink_1_2", "NavmeshGeometry[8].EdgeLink_2_0", "NavmeshGeometry[8].Height", "NavmeshGeometry[8].Vertices", "NavmeshGeometry[8][0]", "NavmeshGeometry[9].Count", "NavmeshGeometry[9].EdgeLink_1_2", "NavmeshGeometry[9].EdgeLink_2_0", "NavmeshGeometry[9].Height", "NavmeshGeometry[9].Vertices", "NavmeshGeometry[9][0]", "ObjectBoundsFirst", "ObjectBoundsSecond", "PreviewTransform", "Version2", "VersionControl");
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

        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "LeafAmplitude").ShouldBe(Helpers.GetDTOField(dto, "LeafAmplitude"));
        Helpers.GetSpriggitField(spriggit, "LeafFrequency").ShouldBe(Helpers.GetDTOField(dto, "LeafFrequency"));
        Helpers.GetSpriggitField(spriggit, "MaxAngle").ShouldBe(Helpers.GetDTOField(dto, "MaxAngle"));
        Helpers.GetSpriggitField(spriggit, "Model.Data").ShouldBe(Helpers.GetDTOField(dto, "Model.Data"));
        Helpers.GetSpriggitField(spriggit, "Model.File").ShouldBe(Helpers.GetDTOField(dto, "Models[0].File"));
        Helpers.GetSpriggitField(spriggit, "Name.Count").ShouldBe(Helpers.GetDTOField(dto, "Name.Count"));
        Helpers.GetSpriggitField(spriggit, "Name.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Name.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Name[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[0].String").ShouldBe(Helpers.GetDTOField(dto, "Name[0].String"));
        Helpers.GetSpriggitField(spriggit, "Name[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[1].String").ShouldBe(Helpers.GetDTOField(dto, "Name[1].String"));
        Helpers.GetSpriggitField(spriggit, "Name[10].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[10].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[10].String").ShouldBe(Helpers.GetDTOField(dto, "Name[10].String"));
        Helpers.GetSpriggitField(spriggit, "Name[2].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[2].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[2].String").ShouldBe(Helpers.GetDTOField(dto, "Name[2].String"));
        Helpers.GetSpriggitField(spriggit, "Name[3].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[3].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[3].String").ShouldBe(Helpers.GetDTOField(dto, "Name[3].String"));
        Helpers.GetSpriggitField(spriggit, "Name[4].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[4].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[4].String").ShouldBe(Helpers.GetDTOField(dto, "Name[4].String"));
        Helpers.GetSpriggitField(spriggit, "Name[5].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[5].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[5].String").ShouldBe(Helpers.GetDTOField(dto, "Name[5].String"));
        Helpers.GetSpriggitField(spriggit, "Name[6].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[6].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[6].String").ShouldBe(Helpers.GetDTOField(dto, "Name[6].String"));
        Helpers.GetSpriggitField(spriggit, "Name[7].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[7].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[7].String").ShouldBe(Helpers.GetDTOField(dto, "Name[7].String"));
        Helpers.GetSpriggitField(spriggit, "Name[8].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[8].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[8].String").ShouldBe(Helpers.GetDTOField(dto, "Name[8].String"));
        Helpers.GetSpriggitField(spriggit, "Name[9].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[9].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[9].String").ShouldBe(Helpers.GetDTOField(dto, "Name[9].String"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.Count").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.Count"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridArrays.Count").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridArrays.Count"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridArrays[0]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridArrays[0]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridArrays[1]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridArrays[1]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridArrays[10]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridArrays[10]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridArrays[11]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridArrays[11]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridArrays[12]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridArrays[12]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridArrays[13]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridArrays[13]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridArrays[14]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridArrays[14]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridArrays[15]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridArrays[15]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridArrays[16]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridArrays[16]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridArrays[17]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridArrays[17]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridArrays[18]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridArrays[18]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridArrays[19]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridArrays[19]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridArrays[2]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridArrays[2]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridArrays[20]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridArrays[20]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridArrays[21]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridArrays[21]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridArrays[22]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridArrays[22]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridArrays[23]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridArrays[23]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridArrays[24]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridArrays[24]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridArrays[25]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridArrays[25]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridArrays[26]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridArrays[26]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridArrays[27]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridArrays[27]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridArrays[28]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridArrays[28]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridArrays[29]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridArrays[29]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridArrays[3]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridArrays[3]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridArrays[30]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridArrays[30]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridArrays[31]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridArrays[31]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridArrays[32]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridArrays[32]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridArrays[33]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridArrays[33]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridArrays[34]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridArrays[34]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridArrays[35]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridArrays[35]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridArrays[36]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridArrays[36]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridArrays[37]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridArrays[37]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridArrays[38]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridArrays[38]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridArrays[39]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridArrays[39]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridArrays[4]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridArrays[4]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridArrays[40]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridArrays[40]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridArrays[5]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridArrays[5]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridArrays[6]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridArrays[6]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridArrays[7]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridArrays[7]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridArrays[8]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridArrays[8]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridArrays[9]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridArrays[9]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridMax").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridMax"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridMaxDistance").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridMaxDistance"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridMin").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridMin"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridSize").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridSize"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.Parent.MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.Parent.MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.Parent.Parent").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.Parent.Parent"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[0]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[0]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[1]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[1]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[10]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[10]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[11]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[11]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[12]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[12]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[13]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[13]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[14]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[14]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[15]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[15]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[16].Count").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[16].Count"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[16].CoverFlags").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[16].CoverFlags"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[16].EdgeLink_0_1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[16].EdgeLink_0_1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[16].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[16].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[16].EdgeLink_2_0").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[16].EdgeLink_2_0"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[16].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[16].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[16].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[16].Vertices"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[16][0]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[16][0]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[17].Count").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[17].Count"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[17].CoverFlags").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[17].CoverFlags"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[17].EdgeLink_0_1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[17].EdgeLink_0_1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[17].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[17].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[17].EdgeLink_2_0").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[17].EdgeLink_2_0"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[17].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[17].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[17].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[17].Vertices"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[17][0]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[17][0]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[18].Count").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[18].Count"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[18].CoverFlags").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[18].CoverFlags"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[18].EdgeLink_0_1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[18].EdgeLink_0_1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[18].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[18].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[18].EdgeLink_2_0").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[18].EdgeLink_2_0"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[18].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[18].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[18].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[18].Vertices"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[18][0]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[18][0]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[19].Count").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[19].Count"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[19].CoverFlags").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[19].CoverFlags"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[19].EdgeLink_0_1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[19].EdgeLink_0_1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[19].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[19].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[19].EdgeLink_2_0").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[19].EdgeLink_2_0"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[19].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[19].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[19].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[19].Vertices"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[19][0]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[19][0]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[2]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[2]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[20].EdgeLink_0_1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[20].EdgeLink_0_1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[20].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[20].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[20].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[20].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[20].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[20].Vertices"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[21].CoverFlags").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[21].CoverFlags"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[21].EdgeLink_0_1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[21].EdgeLink_0_1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[21].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[21].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[21].EdgeLink_2_0").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[21].EdgeLink_2_0"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[21].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[21].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[21].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[21].Vertices"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[22].EdgeLink_0_1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[22].EdgeLink_0_1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[22].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[22].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[22].EdgeLink_2_0").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[22].EdgeLink_2_0"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[22].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[22].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[22].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[22].Vertices"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[23].EdgeLink_0_1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[23].EdgeLink_0_1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[23].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[23].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[23].EdgeLink_2_0").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[23].EdgeLink_2_0"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[23].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[23].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[23].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[23].Vertices"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[24].EdgeLink_0_1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[24].EdgeLink_0_1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[24].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[24].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[24].EdgeLink_2_0").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[24].EdgeLink_2_0"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[24].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[24].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[24].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[24].Vertices"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[25].EdgeLink_0_1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[25].EdgeLink_0_1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[25].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[25].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[25].EdgeLink_2_0").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[25].EdgeLink_2_0"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[25].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[25].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[25].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[25].Vertices"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[26].EdgeLink_0_1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[26].EdgeLink_0_1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[26].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[26].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[26].EdgeLink_2_0").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[26].EdgeLink_2_0"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[26].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[26].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[26].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[26].Vertices"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[27].EdgeLink_0_1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[27].EdgeLink_0_1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[27].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[27].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[27].EdgeLink_2_0").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[27].EdgeLink_2_0"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[27].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[27].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[27].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[27].Vertices"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[28].CoverFlags").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[28].CoverFlags"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[28].EdgeLink_0_1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[28].EdgeLink_0_1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[28].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[28].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[28].EdgeLink_2_0").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[28].EdgeLink_2_0"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[28].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[28].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[28].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[28].Vertices"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[29].EdgeLink_0_1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[29].EdgeLink_0_1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[29].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[29].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[29].EdgeLink_2_0").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[29].EdgeLink_2_0"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[29].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[29].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[29].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[29].Vertices"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[3]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[3]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[30].EdgeLink_0_1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[30].EdgeLink_0_1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[30].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[30].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[30].EdgeLink_2_0").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[30].EdgeLink_2_0"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[30].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[30].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[30].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[30].Vertices"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[31].CoverFlags").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[31].CoverFlags"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[31].EdgeLink_0_1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[31].EdgeLink_0_1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[31].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[31].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[31].EdgeLink_2_0").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[31].EdgeLink_2_0"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[31].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[31].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[31].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[31].Vertices"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[32].EdgeLink_0_1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[32].EdgeLink_0_1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[32].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[32].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[32].EdgeLink_2_0").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[32].EdgeLink_2_0"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[32].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[32].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[32].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[32].Vertices"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[33].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[33].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[33].EdgeLink_2_0").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[33].EdgeLink_2_0"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[33].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[33].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[33].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[33].Vertices"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[34].Data").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[34].Data"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[34].Vertex2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[34].Vertex2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[35].Data").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[35].Data"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[35].Vertex1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[35].Vertex1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[35].Vertex2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[35].Vertex2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[36].Data").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[36].Data"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[36].Vertex1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[36].Vertex1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[36].Vertex2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[36].Vertex2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[37].Data").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[37].Data"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[37].Vertex1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[37].Vertex1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[37].Vertex2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[37].Vertex2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[38].Data").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[38].Data"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[38].Vertex1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[38].Vertex1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[38].Vertex2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[38].Vertex2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[39].Data").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[39].Data"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[39].Vertex1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[39].Vertex1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[39].Vertex2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[39].Vertex2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[4]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[4]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[40].Data").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[40].Data"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[40].Vertex1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[40].Vertex1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[40].Vertex2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[40].Vertex2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[41].Data").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[41].Data"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[41].Vertex1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[41].Vertex1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[42]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[42]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[43].Cover").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[43].Cover"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[44].Cover").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[44].Cover"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[44].Triangle").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[44].Triangle"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[45].Cover").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[45].Cover"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[45].Triangle").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[45].Triangle"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[46].Cover").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[46].Cover"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[46].Triangle").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[46].Triangle"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[47].Cover").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[47].Cover"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[47].Triangle").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[47].Triangle"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[48].Cover").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[48].Cover"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[48].Triangle").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[48].Triangle"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[49].Cover").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[49].Cover"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[49].Triangle").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[49].Triangle"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[5]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[5]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[50].Cover").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[50].Cover"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[50].Triangle").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[50].Triangle"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[51].Cover").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[51].Cover"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[51].Triangle").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[51].Triangle"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[52].Cover").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[52].Cover"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[52].Triangle").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[52].Triangle"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[53].Cover").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[53].Cover"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[53].Triangle").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[53].Triangle"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[54].Cover").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[54].Cover"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[54].Triangle").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[54].Triangle"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[55].Cover").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[55].Cover"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[55].Triangle").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[55].Triangle"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[56].Cover").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[56].Cover"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[56].Triangle").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[56].Triangle"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[6]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[6]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[7]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[7]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[8]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[8]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[9]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[9]"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.First").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsFirst"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.Second").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsSecond"));
        Helpers.GetSpriggitField(spriggit, "PreviewTransform").ShouldBe(Helpers.GetDTOField(dto, "PreviewTransform"));
        Helpers.GetSpriggitField(spriggit, "Value[0]").ShouldBe(Helpers.GetDTOField(dto, "Value[0]"));
        Helpers.GetSpriggitField(spriggit, "Value[1]").ShouldBe(Helpers.GetDTOField(dto, "Value[1]"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "EditorID", "FormKey", "LeafAmplitude", "LeafFrequency", "MaxAngle", "Model.Data", "Model.File", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "NavmeshGeometry.Count", "NavmeshGeometry.GridArrays.Count", "NavmeshGeometry.GridArrays[0]", "NavmeshGeometry.GridArrays[1]", "NavmeshGeometry.GridArrays[10]", "NavmeshGeometry.GridArrays[11]", "NavmeshGeometry.GridArrays[12]", "NavmeshGeometry.GridArrays[13]", "NavmeshGeometry.GridArrays[14]", "NavmeshGeometry.GridArrays[15]", "NavmeshGeometry.GridArrays[16]", "NavmeshGeometry.GridArrays[17]", "NavmeshGeometry.GridArrays[18]", "NavmeshGeometry.GridArrays[19]", "NavmeshGeometry.GridArrays[2]", "NavmeshGeometry.GridArrays[20]", "NavmeshGeometry.GridArrays[21]", "NavmeshGeometry.GridArrays[22]", "NavmeshGeometry.GridArrays[23]", "NavmeshGeometry.GridArrays[24]", "NavmeshGeometry.GridArrays[25]", "NavmeshGeometry.GridArrays[26]", "NavmeshGeometry.GridArrays[27]", "NavmeshGeometry.GridArrays[28]", "NavmeshGeometry.GridArrays[29]", "NavmeshGeometry.GridArrays[3]", "NavmeshGeometry.GridArrays[30]", "NavmeshGeometry.GridArrays[31]", "NavmeshGeometry.GridArrays[32]", "NavmeshGeometry.GridArrays[33]", "NavmeshGeometry.GridArrays[34]", "NavmeshGeometry.GridArrays[35]", "NavmeshGeometry.GridArrays[36]", "NavmeshGeometry.GridArrays[37]", "NavmeshGeometry.GridArrays[38]", "NavmeshGeometry.GridArrays[39]", "NavmeshGeometry.GridArrays[4]", "NavmeshGeometry.GridArrays[40]", "NavmeshGeometry.GridArrays[5]", "NavmeshGeometry.GridArrays[6]", "NavmeshGeometry.GridArrays[7]", "NavmeshGeometry.GridArrays[8]", "NavmeshGeometry.GridArrays[9]", "NavmeshGeometry.GridMax", "NavmeshGeometry.GridMaxDistance", "NavmeshGeometry.GridMin", "NavmeshGeometry.GridSize", "NavmeshGeometry.Parent.MutagenObjectType", "NavmeshGeometry.Parent.Parent", "NavmeshGeometry[0]", "NavmeshGeometry[1]", "NavmeshGeometry[10]", "NavmeshGeometry[11]", "NavmeshGeometry[12]", "NavmeshGeometry[13]", "NavmeshGeometry[14]", "NavmeshGeometry[15]", "NavmeshGeometry[16].Count", "NavmeshGeometry[16].CoverFlags", "NavmeshGeometry[16].EdgeLink_0_1", "NavmeshGeometry[16].EdgeLink_1_2", "NavmeshGeometry[16].EdgeLink_2_0", "NavmeshGeometry[16].Height", "NavmeshGeometry[16].Vertices", "NavmeshGeometry[16][0]", "NavmeshGeometry[17].Count", "NavmeshGeometry[17].CoverFlags", "NavmeshGeometry[17].EdgeLink_0_1", "NavmeshGeometry[17].EdgeLink_1_2", "NavmeshGeometry[17].EdgeLink_2_0", "NavmeshGeometry[17].Height", "NavmeshGeometry[17].Vertices", "NavmeshGeometry[17][0]", "NavmeshGeometry[18].Count", "NavmeshGeometry[18].CoverFlags", "NavmeshGeometry[18].EdgeLink_0_1", "NavmeshGeometry[18].EdgeLink_1_2", "NavmeshGeometry[18].EdgeLink_2_0", "NavmeshGeometry[18].Height", "NavmeshGeometry[18].Vertices", "NavmeshGeometry[18][0]", "NavmeshGeometry[19].Count", "NavmeshGeometry[19].CoverFlags", "NavmeshGeometry[19].EdgeLink_0_1", "NavmeshGeometry[19].EdgeLink_1_2", "NavmeshGeometry[19].EdgeLink_2_0", "NavmeshGeometry[19].Height", "NavmeshGeometry[19].Vertices", "NavmeshGeometry[19][0]", "NavmeshGeometry[2]", "NavmeshGeometry[20].EdgeLink_0_1", "NavmeshGeometry[20].EdgeLink_1_2", "NavmeshGeometry[20].Height", "NavmeshGeometry[20].Vertices", "NavmeshGeometry[21].CoverFlags", "NavmeshGeometry[21].EdgeLink_0_1", "NavmeshGeometry[21].EdgeLink_1_2", "NavmeshGeometry[21].EdgeLink_2_0", "NavmeshGeometry[21].Height", "NavmeshGeometry[21].Vertices", "NavmeshGeometry[22].EdgeLink_0_1", "NavmeshGeometry[22].EdgeLink_1_2", "NavmeshGeometry[22].EdgeLink_2_0", "NavmeshGeometry[22].Height", "NavmeshGeometry[22].Vertices", "NavmeshGeometry[23].EdgeLink_0_1", "NavmeshGeometry[23].EdgeLink_1_2", "NavmeshGeometry[23].EdgeLink_2_0", "NavmeshGeometry[23].Height", "NavmeshGeometry[23].Vertices", "NavmeshGeometry[24].EdgeLink_0_1", "NavmeshGeometry[24].EdgeLink_1_2", "NavmeshGeometry[24].EdgeLink_2_0", "NavmeshGeometry[24].Height", "NavmeshGeometry[24].Vertices", "NavmeshGeometry[25].EdgeLink_0_1", "NavmeshGeometry[25].EdgeLink_1_2", "NavmeshGeometry[25].EdgeLink_2_0", "NavmeshGeometry[25].Height", "NavmeshGeometry[25].Vertices", "NavmeshGeometry[26].EdgeLink_0_1", "NavmeshGeometry[26].EdgeLink_1_2", "NavmeshGeometry[26].EdgeLink_2_0", "NavmeshGeometry[26].Height", "NavmeshGeometry[26].Vertices", "NavmeshGeometry[27].EdgeLink_0_1", "NavmeshGeometry[27].EdgeLink_1_2", "NavmeshGeometry[27].EdgeLink_2_0", "NavmeshGeometry[27].Height", "NavmeshGeometry[27].Vertices", "NavmeshGeometry[28].CoverFlags", "NavmeshGeometry[28].EdgeLink_0_1", "NavmeshGeometry[28].EdgeLink_1_2", "NavmeshGeometry[28].EdgeLink_2_0", "NavmeshGeometry[28].Height", "NavmeshGeometry[28].Vertices", "NavmeshGeometry[29].EdgeLink_0_1", "NavmeshGeometry[29].EdgeLink_1_2", "NavmeshGeometry[29].EdgeLink_2_0", "NavmeshGeometry[29].Height", "NavmeshGeometry[29].Vertices", "NavmeshGeometry[3]", "NavmeshGeometry[30].EdgeLink_0_1", "NavmeshGeometry[30].EdgeLink_1_2", "NavmeshGeometry[30].EdgeLink_2_0", "NavmeshGeometry[30].Height", "NavmeshGeometry[30].Vertices", "NavmeshGeometry[31].CoverFlags", "NavmeshGeometry[31].EdgeLink_0_1", "NavmeshGeometry[31].EdgeLink_1_2", "NavmeshGeometry[31].EdgeLink_2_0", "NavmeshGeometry[31].Height", "NavmeshGeometry[31].Vertices", "NavmeshGeometry[32].EdgeLink_0_1", "NavmeshGeometry[32].EdgeLink_1_2", "NavmeshGeometry[32].EdgeLink_2_0", "NavmeshGeometry[32].Height", "NavmeshGeometry[32].Vertices", "NavmeshGeometry[33].EdgeLink_1_2", "NavmeshGeometry[33].EdgeLink_2_0", "NavmeshGeometry[33].Height", "NavmeshGeometry[33].Vertices", "NavmeshGeometry[34].Data", "NavmeshGeometry[34].Vertex2", "NavmeshGeometry[35].Data", "NavmeshGeometry[35].Vertex1", "NavmeshGeometry[35].Vertex2", "NavmeshGeometry[36].Data", "NavmeshGeometry[36].Vertex1", "NavmeshGeometry[36].Vertex2", "NavmeshGeometry[37].Data", "NavmeshGeometry[37].Vertex1", "NavmeshGeometry[37].Vertex2", "NavmeshGeometry[38].Data", "NavmeshGeometry[38].Vertex1", "NavmeshGeometry[38].Vertex2", "NavmeshGeometry[39].Data", "NavmeshGeometry[39].Vertex1", "NavmeshGeometry[39].Vertex2", "NavmeshGeometry[4]", "NavmeshGeometry[40].Data", "NavmeshGeometry[40].Vertex1", "NavmeshGeometry[40].Vertex2", "NavmeshGeometry[41].Data", "NavmeshGeometry[41].Vertex1", "NavmeshGeometry[42]", "NavmeshGeometry[43].Cover", "NavmeshGeometry[44].Cover", "NavmeshGeometry[44].Triangle", "NavmeshGeometry[45].Cover", "NavmeshGeometry[45].Triangle", "NavmeshGeometry[46].Cover", "NavmeshGeometry[46].Triangle", "NavmeshGeometry[47].Cover", "NavmeshGeometry[47].Triangle", "NavmeshGeometry[48].Cover", "NavmeshGeometry[48].Triangle", "NavmeshGeometry[49].Cover", "NavmeshGeometry[49].Triangle", "NavmeshGeometry[5]", "NavmeshGeometry[50].Cover", "NavmeshGeometry[50].Triangle", "NavmeshGeometry[51].Cover", "NavmeshGeometry[51].Triangle", "NavmeshGeometry[52].Cover", "NavmeshGeometry[52].Triangle", "NavmeshGeometry[53].Cover", "NavmeshGeometry[53].Triangle", "NavmeshGeometry[54].Cover", "NavmeshGeometry[54].Triangle", "NavmeshGeometry[55].Cover", "NavmeshGeometry[55].Triangle", "NavmeshGeometry[56].Cover", "NavmeshGeometry[56].Triangle", "NavmeshGeometry[6]", "NavmeshGeometry[7]", "NavmeshGeometry[8]", "NavmeshGeometry[9]", "ObjectBounds.First", "ObjectBounds.Second", "PreviewTransform", "Value[0]", "Value[1]", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "EditorID", "FormKey", "LeafAmplitude", "LeafFrequency", "MaxAngle", "Model.Data", "Models[0].File", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "NavmeshGeometry.Count", "NavmeshGeometry.GridArrays.Count", "NavmeshGeometry.GridArrays[0]", "NavmeshGeometry.GridArrays[1]", "NavmeshGeometry.GridArrays[10]", "NavmeshGeometry.GridArrays[11]", "NavmeshGeometry.GridArrays[12]", "NavmeshGeometry.GridArrays[13]", "NavmeshGeometry.GridArrays[14]", "NavmeshGeometry.GridArrays[15]", "NavmeshGeometry.GridArrays[16]", "NavmeshGeometry.GridArrays[17]", "NavmeshGeometry.GridArrays[18]", "NavmeshGeometry.GridArrays[19]", "NavmeshGeometry.GridArrays[2]", "NavmeshGeometry.GridArrays[20]", "NavmeshGeometry.GridArrays[21]", "NavmeshGeometry.GridArrays[22]", "NavmeshGeometry.GridArrays[23]", "NavmeshGeometry.GridArrays[24]", "NavmeshGeometry.GridArrays[25]", "NavmeshGeometry.GridArrays[26]", "NavmeshGeometry.GridArrays[27]", "NavmeshGeometry.GridArrays[28]", "NavmeshGeometry.GridArrays[29]", "NavmeshGeometry.GridArrays[3]", "NavmeshGeometry.GridArrays[30]", "NavmeshGeometry.GridArrays[31]", "NavmeshGeometry.GridArrays[32]", "NavmeshGeometry.GridArrays[33]", "NavmeshGeometry.GridArrays[34]", "NavmeshGeometry.GridArrays[35]", "NavmeshGeometry.GridArrays[36]", "NavmeshGeometry.GridArrays[37]", "NavmeshGeometry.GridArrays[38]", "NavmeshGeometry.GridArrays[39]", "NavmeshGeometry.GridArrays[4]", "NavmeshGeometry.GridArrays[40]", "NavmeshGeometry.GridArrays[5]", "NavmeshGeometry.GridArrays[6]", "NavmeshGeometry.GridArrays[7]", "NavmeshGeometry.GridArrays[8]", "NavmeshGeometry.GridArrays[9]", "NavmeshGeometry.GridMax", "NavmeshGeometry.GridMaxDistance", "NavmeshGeometry.GridMin", "NavmeshGeometry.GridSize", "NavmeshGeometry.Parent.MutagenObjectType", "NavmeshGeometry.Parent.Parent", "NavmeshGeometry[0]", "NavmeshGeometry[1]", "NavmeshGeometry[10]", "NavmeshGeometry[11]", "NavmeshGeometry[12]", "NavmeshGeometry[13]", "NavmeshGeometry[14]", "NavmeshGeometry[15]", "NavmeshGeometry[16].Count", "NavmeshGeometry[16].CoverFlags", "NavmeshGeometry[16].EdgeLink_0_1", "NavmeshGeometry[16].EdgeLink_1_2", "NavmeshGeometry[16].EdgeLink_2_0", "NavmeshGeometry[16].Height", "NavmeshGeometry[16].Vertices", "NavmeshGeometry[16][0]", "NavmeshGeometry[17].Count", "NavmeshGeometry[17].CoverFlags", "NavmeshGeometry[17].EdgeLink_0_1", "NavmeshGeometry[17].EdgeLink_1_2", "NavmeshGeometry[17].EdgeLink_2_0", "NavmeshGeometry[17].Height", "NavmeshGeometry[17].Vertices", "NavmeshGeometry[17][0]", "NavmeshGeometry[18].Count", "NavmeshGeometry[18].CoverFlags", "NavmeshGeometry[18].EdgeLink_0_1", "NavmeshGeometry[18].EdgeLink_1_2", "NavmeshGeometry[18].EdgeLink_2_0", "NavmeshGeometry[18].Height", "NavmeshGeometry[18].Vertices", "NavmeshGeometry[18][0]", "NavmeshGeometry[19].Count", "NavmeshGeometry[19].CoverFlags", "NavmeshGeometry[19].EdgeLink_0_1", "NavmeshGeometry[19].EdgeLink_1_2", "NavmeshGeometry[19].EdgeLink_2_0", "NavmeshGeometry[19].Height", "NavmeshGeometry[19].Vertices", "NavmeshGeometry[19][0]", "NavmeshGeometry[2]", "NavmeshGeometry[20].EdgeLink_0_1", "NavmeshGeometry[20].EdgeLink_1_2", "NavmeshGeometry[20].Height", "NavmeshGeometry[20].Vertices", "NavmeshGeometry[21].CoverFlags", "NavmeshGeometry[21].EdgeLink_0_1", "NavmeshGeometry[21].EdgeLink_1_2", "NavmeshGeometry[21].EdgeLink_2_0", "NavmeshGeometry[21].Height", "NavmeshGeometry[21].Vertices", "NavmeshGeometry[22].EdgeLink_0_1", "NavmeshGeometry[22].EdgeLink_1_2", "NavmeshGeometry[22].EdgeLink_2_0", "NavmeshGeometry[22].Height", "NavmeshGeometry[22].Vertices", "NavmeshGeometry[23].EdgeLink_0_1", "NavmeshGeometry[23].EdgeLink_1_2", "NavmeshGeometry[23].EdgeLink_2_0", "NavmeshGeometry[23].Height", "NavmeshGeometry[23].Vertices", "NavmeshGeometry[24].EdgeLink_0_1", "NavmeshGeometry[24].EdgeLink_1_2", "NavmeshGeometry[24].EdgeLink_2_0", "NavmeshGeometry[24].Height", "NavmeshGeometry[24].Vertices", "NavmeshGeometry[25].EdgeLink_0_1", "NavmeshGeometry[25].EdgeLink_1_2", "NavmeshGeometry[25].EdgeLink_2_0", "NavmeshGeometry[25].Height", "NavmeshGeometry[25].Vertices", "NavmeshGeometry[26].EdgeLink_0_1", "NavmeshGeometry[26].EdgeLink_1_2", "NavmeshGeometry[26].EdgeLink_2_0", "NavmeshGeometry[26].Height", "NavmeshGeometry[26].Vertices", "NavmeshGeometry[27].EdgeLink_0_1", "NavmeshGeometry[27].EdgeLink_1_2", "NavmeshGeometry[27].EdgeLink_2_0", "NavmeshGeometry[27].Height", "NavmeshGeometry[27].Vertices", "NavmeshGeometry[28].CoverFlags", "NavmeshGeometry[28].EdgeLink_0_1", "NavmeshGeometry[28].EdgeLink_1_2", "NavmeshGeometry[28].EdgeLink_2_0", "NavmeshGeometry[28].Height", "NavmeshGeometry[28].Vertices", "NavmeshGeometry[29].EdgeLink_0_1", "NavmeshGeometry[29].EdgeLink_1_2", "NavmeshGeometry[29].EdgeLink_2_0", "NavmeshGeometry[29].Height", "NavmeshGeometry[29].Vertices", "NavmeshGeometry[3]", "NavmeshGeometry[30].EdgeLink_0_1", "NavmeshGeometry[30].EdgeLink_1_2", "NavmeshGeometry[30].EdgeLink_2_0", "NavmeshGeometry[30].Height", "NavmeshGeometry[30].Vertices", "NavmeshGeometry[31].CoverFlags", "NavmeshGeometry[31].EdgeLink_0_1", "NavmeshGeometry[31].EdgeLink_1_2", "NavmeshGeometry[31].EdgeLink_2_0", "NavmeshGeometry[31].Height", "NavmeshGeometry[31].Vertices", "NavmeshGeometry[32].EdgeLink_0_1", "NavmeshGeometry[32].EdgeLink_1_2", "NavmeshGeometry[32].EdgeLink_2_0", "NavmeshGeometry[32].Height", "NavmeshGeometry[32].Vertices", "NavmeshGeometry[33].EdgeLink_1_2", "NavmeshGeometry[33].EdgeLink_2_0", "NavmeshGeometry[33].Height", "NavmeshGeometry[33].Vertices", "NavmeshGeometry[34].Data", "NavmeshGeometry[34].Vertex2", "NavmeshGeometry[35].Data", "NavmeshGeometry[35].Vertex1", "NavmeshGeometry[35].Vertex2", "NavmeshGeometry[36].Data", "NavmeshGeometry[36].Vertex1", "NavmeshGeometry[36].Vertex2", "NavmeshGeometry[37].Data", "NavmeshGeometry[37].Vertex1", "NavmeshGeometry[37].Vertex2", "NavmeshGeometry[38].Data", "NavmeshGeometry[38].Vertex1", "NavmeshGeometry[38].Vertex2", "NavmeshGeometry[39].Data", "NavmeshGeometry[39].Vertex1", "NavmeshGeometry[39].Vertex2", "NavmeshGeometry[4]", "NavmeshGeometry[40].Data", "NavmeshGeometry[40].Vertex1", "NavmeshGeometry[40].Vertex2", "NavmeshGeometry[41].Data", "NavmeshGeometry[41].Vertex1", "NavmeshGeometry[42]", "NavmeshGeometry[43].Cover", "NavmeshGeometry[44].Cover", "NavmeshGeometry[44].Triangle", "NavmeshGeometry[45].Cover", "NavmeshGeometry[45].Triangle", "NavmeshGeometry[46].Cover", "NavmeshGeometry[46].Triangle", "NavmeshGeometry[47].Cover", "NavmeshGeometry[47].Triangle", "NavmeshGeometry[48].Cover", "NavmeshGeometry[48].Triangle", "NavmeshGeometry[49].Cover", "NavmeshGeometry[49].Triangle", "NavmeshGeometry[5]", "NavmeshGeometry[50].Cover", "NavmeshGeometry[50].Triangle", "NavmeshGeometry[51].Cover", "NavmeshGeometry[51].Triangle", "NavmeshGeometry[52].Cover", "NavmeshGeometry[52].Triangle", "NavmeshGeometry[53].Cover", "NavmeshGeometry[53].Triangle", "NavmeshGeometry[54].Cover", "NavmeshGeometry[54].Triangle", "NavmeshGeometry[55].Cover", "NavmeshGeometry[55].Triangle", "NavmeshGeometry[56].Cover", "NavmeshGeometry[56].Triangle", "NavmeshGeometry[6]", "NavmeshGeometry[7]", "NavmeshGeometry[8]", "NavmeshGeometry[9]", "ObjectBoundsFirst", "ObjectBoundsSecond", "PreviewTransform", "Value[0]", "Value[1]", "Version2", "VersionControl");
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

        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "MajorRecordFlagsRaw").ShouldBe(Helpers.GetDTOField(dto, "MajorRecordFlags"));
        Helpers.GetSpriggitField(spriggit, "MaxAngle").ShouldBe(Helpers.GetDTOField(dto, "MaxAngle"));
        Helpers.GetSpriggitField(spriggit, "Model.File").ShouldBe(Helpers.GetDTOField(dto, "Models[0].File"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.First").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsFirst"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.Second").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsSecond"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "EditorID", "FormKey", "FormVersion", "MajorRecordFlagsRaw", "MaxAngle", "Model.File", "ObjectBounds.First", "ObjectBounds.Second", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "EditorID", "FormKey", "FormVersion", "MajorRecordFlags", "MaxAngle", "Models[0].File", "ObjectBoundsFirst", "ObjectBoundsSecond", "Version2", "VersionControl");
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

        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "MajorRecordFlagsRaw").ShouldBe(Helpers.GetDTOField(dto, "MajorRecordFlags"));
        Helpers.GetSpriggitField(spriggit, "MaxAngle").ShouldBe(Helpers.GetDTOField(dto, "MaxAngle"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "EditorID", "FormKey", "FormVersion", "MajorRecordFlagsRaw", "MaxAngle", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "EditorID", "FormKey", "FormVersion", "MajorRecordFlags", "MaxAngle", "Version2", "VersionControl");
    }
}