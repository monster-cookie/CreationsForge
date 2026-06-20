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

        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "MaxAngle").ShouldBe(Helpers.GetDTOField(dto, "MaxAngle"));
        Helpers.GetSpriggitField(spriggit, "Model.File").ShouldBe(Helpers.GetDTOField(dto, "Models[0].File"));
        Helpers.GetSpriggitField(spriggit, "Model.LightLayer").ShouldBe(Helpers.GetDTOField(dto, "Models[0].LightLayer"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.Count").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.Count"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridMax").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridMax"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridMaxDistance").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridMaxDistance"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridMin").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridMin"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridSize").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridSize"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.Parent.MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.Parent.MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.Parent.Parent").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.Parent.Parent"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[0]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[0]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[1].Point").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[1].Point"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[10].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[10].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[10].EdgeLink_2_0").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[10].EdgeLink_2_0"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[10].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[10].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[10].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[10].Vertices"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[11].EdgeLink_0_1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[11].EdgeLink_0_1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[11].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[11].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[11].EdgeLink_2_0").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[11].EdgeLink_2_0"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[11].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[11].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[11].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[11].Vertices"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[12].EdgeLink_0_1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[12].EdgeLink_0_1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[12].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[12].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[12].EdgeLink_2_0").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[12].EdgeLink_2_0"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[12].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[12].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[12].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[12].Vertices"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[13].EdgeLink_0_1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[13].EdgeLink_0_1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[13].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[13].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[13].EdgeLink_2_0").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[13].EdgeLink_2_0"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[13].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[13].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[13].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[13].Vertices"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[14].EdgeLink_0_1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[14].EdgeLink_0_1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[14].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[14].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[14].EdgeLink_2_0").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[14].EdgeLink_2_0"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[14].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[14].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[14].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[14].Vertices"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[15].GridCell.Count").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[15].GridCell.Count"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[15].GridCell[0]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[15].GridCell[0]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[15].GridCell[1]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[15].GridCell[1]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[15].GridCell[2]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[15].GridCell[2]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[15].GridCell[3]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[15].GridCell[3]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[15].GridCell[4]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[15].GridCell[4]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[15].GridCell[5]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[15].GridCell[5]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[15].GridCell[6]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[15].GridCell[6]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[15].GridCell[7]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[15].GridCell[7]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[15].GridCell[8]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[15].GridCell[8]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[15].GridCell[9]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[15].GridCell[9]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[2].Point").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[2].Point"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[3].Point").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[3].Point"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[4].Point").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[4].Point"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[5].Point").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[5].Point"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[6].Point").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[6].Point"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[7].Point").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[7].Point"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[8].Point").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[8].Point"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[9].EdgeLink_0_1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[9].EdgeLink_0_1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[9].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[9].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[9].EdgeLink_2_0").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[9].EdgeLink_2_0"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[9].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[9].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[9].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[9].Vertices"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.First").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsFirst"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.Second").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsSecond"));
        Helpers.GetSpriggitField(spriggit, "REFL").ShouldBe(Helpers.GetDTOField(dto, "REFL"));
        Helpers.GetSpriggitField(spriggit, "SnapTemplate").ShouldBe(Helpers.GetDTOField(dto, "SnapTemplate"));
        Helpers.GetSpriggitField(spriggit, "UnknownDNAMFloat").ShouldBe(Helpers.GetDTOField(dto, "UnknownDNAMFloat"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "EditorID", "FormKey", "FormVersion", "MaxAngle", "Model.File", "Model.LightLayer", "NavmeshGeometry.Count", "NavmeshGeometry.GridMax", "NavmeshGeometry.GridMaxDistance", "NavmeshGeometry.GridMin", "NavmeshGeometry.GridSize", "NavmeshGeometry.Parent.MutagenObjectType", "NavmeshGeometry.Parent.Parent", "NavmeshGeometry[0]", "NavmeshGeometry[1].Point", "NavmeshGeometry[10].EdgeLink_1_2", "NavmeshGeometry[10].EdgeLink_2_0", "NavmeshGeometry[10].Height", "NavmeshGeometry[10].Vertices", "NavmeshGeometry[11].EdgeLink_0_1", "NavmeshGeometry[11].EdgeLink_1_2", "NavmeshGeometry[11].EdgeLink_2_0", "NavmeshGeometry[11].Height", "NavmeshGeometry[11].Vertices", "NavmeshGeometry[12].EdgeLink_0_1", "NavmeshGeometry[12].EdgeLink_1_2", "NavmeshGeometry[12].EdgeLink_2_0", "NavmeshGeometry[12].Height", "NavmeshGeometry[12].Vertices", "NavmeshGeometry[13].EdgeLink_0_1", "NavmeshGeometry[13].EdgeLink_1_2", "NavmeshGeometry[13].EdgeLink_2_0", "NavmeshGeometry[13].Height", "NavmeshGeometry[13].Vertices", "NavmeshGeometry[14].EdgeLink_0_1", "NavmeshGeometry[14].EdgeLink_1_2", "NavmeshGeometry[14].EdgeLink_2_0", "NavmeshGeometry[14].Height", "NavmeshGeometry[14].Vertices", "NavmeshGeometry[15].GridCell.Count", "NavmeshGeometry[15].GridCell[0]", "NavmeshGeometry[15].GridCell[1]", "NavmeshGeometry[15].GridCell[2]", "NavmeshGeometry[15].GridCell[3]", "NavmeshGeometry[15].GridCell[4]", "NavmeshGeometry[15].GridCell[5]", "NavmeshGeometry[15].GridCell[6]", "NavmeshGeometry[15].GridCell[7]", "NavmeshGeometry[15].GridCell[8]", "NavmeshGeometry[15].GridCell[9]", "NavmeshGeometry[2].Point", "NavmeshGeometry[3].Point", "NavmeshGeometry[4].Point", "NavmeshGeometry[5].Point", "NavmeshGeometry[6].Point", "NavmeshGeometry[7].Point", "NavmeshGeometry[8].Point", "NavmeshGeometry[9].EdgeLink_0_1", "NavmeshGeometry[9].EdgeLink_1_2", "NavmeshGeometry[9].EdgeLink_2_0", "NavmeshGeometry[9].Height", "NavmeshGeometry[9].Vertices", "ObjectBounds.First", "ObjectBounds.Second", "REFL", "SnapTemplate", "UnknownDNAMFloat", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "EditorID", "FormKey", "FormVersion", "MaxAngle", "Models[0].File", "Models[0].LightLayer", "NavmeshGeometry.Count", "NavmeshGeometry.GridMax", "NavmeshGeometry.GridMaxDistance", "NavmeshGeometry.GridMin", "NavmeshGeometry.GridSize", "NavmeshGeometry.Parent.MutagenObjectType", "NavmeshGeometry.Parent.Parent", "NavmeshGeometry[0]", "NavmeshGeometry[1].Point", "NavmeshGeometry[10].EdgeLink_1_2", "NavmeshGeometry[10].EdgeLink_2_0", "NavmeshGeometry[10].Height", "NavmeshGeometry[10].Vertices", "NavmeshGeometry[11].EdgeLink_0_1", "NavmeshGeometry[11].EdgeLink_1_2", "NavmeshGeometry[11].EdgeLink_2_0", "NavmeshGeometry[11].Height", "NavmeshGeometry[11].Vertices", "NavmeshGeometry[12].EdgeLink_0_1", "NavmeshGeometry[12].EdgeLink_1_2", "NavmeshGeometry[12].EdgeLink_2_0", "NavmeshGeometry[12].Height", "NavmeshGeometry[12].Vertices", "NavmeshGeometry[13].EdgeLink_0_1", "NavmeshGeometry[13].EdgeLink_1_2", "NavmeshGeometry[13].EdgeLink_2_0", "NavmeshGeometry[13].Height", "NavmeshGeometry[13].Vertices", "NavmeshGeometry[14].EdgeLink_0_1", "NavmeshGeometry[14].EdgeLink_1_2", "NavmeshGeometry[14].EdgeLink_2_0", "NavmeshGeometry[14].Height", "NavmeshGeometry[14].Vertices", "NavmeshGeometry[15].GridCell.Count", "NavmeshGeometry[15].GridCell[0]", "NavmeshGeometry[15].GridCell[1]", "NavmeshGeometry[15].GridCell[2]", "NavmeshGeometry[15].GridCell[3]", "NavmeshGeometry[15].GridCell[4]", "NavmeshGeometry[15].GridCell[5]", "NavmeshGeometry[15].GridCell[6]", "NavmeshGeometry[15].GridCell[7]", "NavmeshGeometry[15].GridCell[8]", "NavmeshGeometry[15].GridCell[9]", "NavmeshGeometry[2].Point", "NavmeshGeometry[3].Point", "NavmeshGeometry[4].Point", "NavmeshGeometry[5].Point", "NavmeshGeometry[6].Point", "NavmeshGeometry[7].Point", "NavmeshGeometry[8].Point", "NavmeshGeometry[9].EdgeLink_0_1", "NavmeshGeometry[9].EdgeLink_1_2", "NavmeshGeometry[9].EdgeLink_2_0", "NavmeshGeometry[9].Height", "NavmeshGeometry[9].Vertices", "ObjectBoundsFirst", "ObjectBoundsSecond", "REFL", "SnapTemplate", "UnknownDNAMFloat", "Version2", "VersionControl");
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

        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "MaxAngle").ShouldBe(Helpers.GetDTOField(dto, "MaxAngle"));
        Helpers.GetSpriggitField(spriggit, "Model.File").ShouldBe(Helpers.GetDTOField(dto, "Models[0].File"));
        Helpers.GetSpriggitField(spriggit, "Model.LightLayer").ShouldBe(Helpers.GetDTOField(dto, "Models[0].LightLayer"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.Count").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.Count"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridMax").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridMax"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridMaxDistance").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridMaxDistance"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridMin").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridMin"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridSize").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridSize"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.Parent.MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.Parent.MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.Parent.Parent").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.Parent.Parent"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[0]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[0]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[1].Point").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[1].Point"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[10].CoverFlags").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[10].CoverFlags"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[10].EdgeLink_0_1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[10].EdgeLink_0_1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[10].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[10].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[10].EdgeLink_2_0").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[10].EdgeLink_2_0"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[10].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[10].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[10].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[10].Vertices"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[11].CoverFlags").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[11].CoverFlags"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[11].EdgeLink_0_1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[11].EdgeLink_0_1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[11].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[11].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[11].EdgeLink_2_0").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[11].EdgeLink_2_0"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[11].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[11].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[11].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[11].Vertices"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[12].CoverFlags").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[12].CoverFlags"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[12].EdgeLink_0_1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[12].EdgeLink_0_1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[12].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[12].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[12].EdgeLink_2_0").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[12].EdgeLink_2_0"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[12].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[12].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[12].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[12].Vertices"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[13].CoverFlags").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[13].CoverFlags"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[13].EdgeLink_0_1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[13].EdgeLink_0_1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[13].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[13].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[13].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[13].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[13].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[13].Vertices"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[14].CoverFlags").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[14].CoverFlags"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[14].EdgeLink_0_1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[14].EdgeLink_0_1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[14].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[14].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[14].EdgeLink_2_0").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[14].EdgeLink_2_0"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[14].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[14].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[14].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[14].Vertices"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[15].Data").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[15].Data"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[15].Vertex1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[15].Vertex1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[15].Vertex2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[15].Vertex2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[16].Data").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[16].Data"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[16].Vertex1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[16].Vertex1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[16].Vertex2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[16].Vertex2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[17].Data").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[17].Data"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[17].Vertex1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[17].Vertex1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[18].Data").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[18].Data"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[18].Vertex2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[18].Vertex2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[19].Data").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[19].Data"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[19].Vertex1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[19].Vertex1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[19].Vertex2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[19].Vertex2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[2].Point").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[2].Point"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[20]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[20]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[21].Cover").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[21].Cover"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[21].Triangle").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[21].Triangle"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[22].Cover").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[22].Cover"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[22].Triangle").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[22].Triangle"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[23].Cover").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[23].Cover"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[23].Triangle").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[23].Triangle"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[24].Cover").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[24].Cover"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[24].Triangle").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[24].Triangle"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[25].Cover").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[25].Cover"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[25].Triangle").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[25].Triangle"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[26].Cover").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[26].Cover"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[26].Triangle").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[26].Triangle"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[27].Cover").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[27].Cover"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[27].Triangle").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[27].Triangle"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[28].GridCell.Count").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[28].GridCell.Count"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[28].GridCell[0]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[28].GridCell[0]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[28].GridCell[1]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[28].GridCell[1]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[28].GridCell[2]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[28].GridCell[2]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[28].GridCell[3]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[28].GridCell[3]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[28].GridCell[4]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[28].GridCell[4]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[28].GridCell[5]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[28].GridCell[5]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[28].GridCell[6]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[28].GridCell[6]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[28].GridCell[7]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[28].GridCell[7]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[28].GridCell[8]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[28].GridCell[8]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[28].GridCell[9]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[28].GridCell[9]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[3].Point").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[3].Point"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[4].Point").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[4].Point"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[5].Point").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[5].Point"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[6].Point").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[6].Point"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[7].Point").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[7].Point"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[8].Point").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[8].Point"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[9].CoverFlags").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[9].CoverFlags"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[9].EdgeLink_0_1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[9].EdgeLink_0_1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[9].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[9].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[9].EdgeLink_2_0").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[9].EdgeLink_2_0"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[9].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[9].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[9].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[9].Vertices"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.First").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsFirst"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.Second").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsSecond"));
        Helpers.GetSpriggitField(spriggit, "UnknownDNAMFloat").ShouldBe(Helpers.GetDTOField(dto, "UnknownDNAMFloat"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "EditorID", "FormKey", "FormVersion", "MaxAngle", "Model.File", "Model.LightLayer", "NavmeshGeometry.Count", "NavmeshGeometry.GridMax", "NavmeshGeometry.GridMaxDistance", "NavmeshGeometry.GridMin", "NavmeshGeometry.GridSize", "NavmeshGeometry.Parent.MutagenObjectType", "NavmeshGeometry.Parent.Parent", "NavmeshGeometry[0]", "NavmeshGeometry[1].Point", "NavmeshGeometry[10].CoverFlags", "NavmeshGeometry[10].EdgeLink_0_1", "NavmeshGeometry[10].EdgeLink_1_2", "NavmeshGeometry[10].EdgeLink_2_0", "NavmeshGeometry[10].Height", "NavmeshGeometry[10].Vertices", "NavmeshGeometry[11].CoverFlags", "NavmeshGeometry[11].EdgeLink_0_1", "NavmeshGeometry[11].EdgeLink_1_2", "NavmeshGeometry[11].EdgeLink_2_0", "NavmeshGeometry[11].Height", "NavmeshGeometry[11].Vertices", "NavmeshGeometry[12].CoverFlags", "NavmeshGeometry[12].EdgeLink_0_1", "NavmeshGeometry[12].EdgeLink_1_2", "NavmeshGeometry[12].EdgeLink_2_0", "NavmeshGeometry[12].Height", "NavmeshGeometry[12].Vertices", "NavmeshGeometry[13].CoverFlags", "NavmeshGeometry[13].EdgeLink_0_1", "NavmeshGeometry[13].EdgeLink_1_2", "NavmeshGeometry[13].Height", "NavmeshGeometry[13].Vertices", "NavmeshGeometry[14].CoverFlags", "NavmeshGeometry[14].EdgeLink_0_1", "NavmeshGeometry[14].EdgeLink_1_2", "NavmeshGeometry[14].EdgeLink_2_0", "NavmeshGeometry[14].Height", "NavmeshGeometry[14].Vertices", "NavmeshGeometry[15].Data", "NavmeshGeometry[15].Vertex1", "NavmeshGeometry[15].Vertex2", "NavmeshGeometry[16].Data", "NavmeshGeometry[16].Vertex1", "NavmeshGeometry[16].Vertex2", "NavmeshGeometry[17].Data", "NavmeshGeometry[17].Vertex1", "NavmeshGeometry[18].Data", "NavmeshGeometry[18].Vertex2", "NavmeshGeometry[19].Data", "NavmeshGeometry[19].Vertex1", "NavmeshGeometry[19].Vertex2", "NavmeshGeometry[2].Point", "NavmeshGeometry[20]", "NavmeshGeometry[21].Cover", "NavmeshGeometry[21].Triangle", "NavmeshGeometry[22].Cover", "NavmeshGeometry[22].Triangle", "NavmeshGeometry[23].Cover", "NavmeshGeometry[23].Triangle", "NavmeshGeometry[24].Cover", "NavmeshGeometry[24].Triangle", "NavmeshGeometry[25].Cover", "NavmeshGeometry[25].Triangle", "NavmeshGeometry[26].Cover", "NavmeshGeometry[26].Triangle", "NavmeshGeometry[27].Cover", "NavmeshGeometry[27].Triangle", "NavmeshGeometry[28].GridCell.Count", "NavmeshGeometry[28].GridCell[0]", "NavmeshGeometry[28].GridCell[1]", "NavmeshGeometry[28].GridCell[2]", "NavmeshGeometry[28].GridCell[3]", "NavmeshGeometry[28].GridCell[4]", "NavmeshGeometry[28].GridCell[5]", "NavmeshGeometry[28].GridCell[6]", "NavmeshGeometry[28].GridCell[7]", "NavmeshGeometry[28].GridCell[8]", "NavmeshGeometry[28].GridCell[9]", "NavmeshGeometry[3].Point", "NavmeshGeometry[4].Point", "NavmeshGeometry[5].Point", "NavmeshGeometry[6].Point", "NavmeshGeometry[7].Point", "NavmeshGeometry[8].Point", "NavmeshGeometry[9].CoverFlags", "NavmeshGeometry[9].EdgeLink_0_1", "NavmeshGeometry[9].EdgeLink_1_2", "NavmeshGeometry[9].EdgeLink_2_0", "NavmeshGeometry[9].Height", "NavmeshGeometry[9].Vertices", "ObjectBounds.First", "ObjectBounds.Second", "UnknownDNAMFloat", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "EditorID", "FormKey", "FormVersion", "MaxAngle", "Models[0].File", "Models[0].LightLayer", "NavmeshGeometry.Count", "NavmeshGeometry.GridMax", "NavmeshGeometry.GridMaxDistance", "NavmeshGeometry.GridMin", "NavmeshGeometry.GridSize", "NavmeshGeometry.Parent.MutagenObjectType", "NavmeshGeometry.Parent.Parent", "NavmeshGeometry[0]", "NavmeshGeometry[1].Point", "NavmeshGeometry[10].CoverFlags", "NavmeshGeometry[10].EdgeLink_0_1", "NavmeshGeometry[10].EdgeLink_1_2", "NavmeshGeometry[10].EdgeLink_2_0", "NavmeshGeometry[10].Height", "NavmeshGeometry[10].Vertices", "NavmeshGeometry[11].CoverFlags", "NavmeshGeometry[11].EdgeLink_0_1", "NavmeshGeometry[11].EdgeLink_1_2", "NavmeshGeometry[11].EdgeLink_2_0", "NavmeshGeometry[11].Height", "NavmeshGeometry[11].Vertices", "NavmeshGeometry[12].CoverFlags", "NavmeshGeometry[12].EdgeLink_0_1", "NavmeshGeometry[12].EdgeLink_1_2", "NavmeshGeometry[12].EdgeLink_2_0", "NavmeshGeometry[12].Height", "NavmeshGeometry[12].Vertices", "NavmeshGeometry[13].CoverFlags", "NavmeshGeometry[13].EdgeLink_0_1", "NavmeshGeometry[13].EdgeLink_1_2", "NavmeshGeometry[13].Height", "NavmeshGeometry[13].Vertices", "NavmeshGeometry[14].CoverFlags", "NavmeshGeometry[14].EdgeLink_0_1", "NavmeshGeometry[14].EdgeLink_1_2", "NavmeshGeometry[14].EdgeLink_2_0", "NavmeshGeometry[14].Height", "NavmeshGeometry[14].Vertices", "NavmeshGeometry[15].Data", "NavmeshGeometry[15].Vertex1", "NavmeshGeometry[15].Vertex2", "NavmeshGeometry[16].Data", "NavmeshGeometry[16].Vertex1", "NavmeshGeometry[16].Vertex2", "NavmeshGeometry[17].Data", "NavmeshGeometry[17].Vertex1", "NavmeshGeometry[18].Data", "NavmeshGeometry[18].Vertex2", "NavmeshGeometry[19].Data", "NavmeshGeometry[19].Vertex1", "NavmeshGeometry[19].Vertex2", "NavmeshGeometry[2].Point", "NavmeshGeometry[20]", "NavmeshGeometry[21].Cover", "NavmeshGeometry[21].Triangle", "NavmeshGeometry[22].Cover", "NavmeshGeometry[22].Triangle", "NavmeshGeometry[23].Cover", "NavmeshGeometry[23].Triangle", "NavmeshGeometry[24].Cover", "NavmeshGeometry[24].Triangle", "NavmeshGeometry[25].Cover", "NavmeshGeometry[25].Triangle", "NavmeshGeometry[26].Cover", "NavmeshGeometry[26].Triangle", "NavmeshGeometry[27].Cover", "NavmeshGeometry[27].Triangle", "NavmeshGeometry[28].GridCell.Count", "NavmeshGeometry[28].GridCell[0]", "NavmeshGeometry[28].GridCell[1]", "NavmeshGeometry[28].GridCell[2]", "NavmeshGeometry[28].GridCell[3]", "NavmeshGeometry[28].GridCell[4]", "NavmeshGeometry[28].GridCell[5]", "NavmeshGeometry[28].GridCell[6]", "NavmeshGeometry[28].GridCell[7]", "NavmeshGeometry[28].GridCell[8]", "NavmeshGeometry[28].GridCell[9]", "NavmeshGeometry[3].Point", "NavmeshGeometry[4].Point", "NavmeshGeometry[5].Point", "NavmeshGeometry[6].Point", "NavmeshGeometry[7].Point", "NavmeshGeometry[8].Point", "NavmeshGeometry[9].CoverFlags", "NavmeshGeometry[9].EdgeLink_0_1", "NavmeshGeometry[9].EdgeLink_1_2", "NavmeshGeometry[9].EdgeLink_2_0", "NavmeshGeometry[9].Height", "NavmeshGeometry[9].Vertices", "ObjectBoundsFirst", "ObjectBoundsSecond", "UnknownDNAMFloat", "Version2", "VersionControl");
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

        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "MaxAngle").ShouldBe(Helpers.GetDTOField(dto, "MaxAngle"));
        Helpers.GetSpriggitField(spriggit, "Model.File").ShouldBe(Helpers.GetDTOField(dto, "Models[0].File"));
        Helpers.GetSpriggitField(spriggit, "Model.LightLayer").ShouldBe(Helpers.GetDTOField(dto, "Models[0].LightLayer"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.Count").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.Count"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridMax").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridMax"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridMaxDistance").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridMaxDistance"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridMin").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridMin"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridSize").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridSize"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.Parent.MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.Parent.MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.Parent.Parent").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.Parent.Parent"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[0]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[0]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[1].Point").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[1].Point"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[10].CoverFlags").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[10].CoverFlags"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[10].EdgeLink_0_1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[10].EdgeLink_0_1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[10].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[10].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[10].EdgeLink_2_0").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[10].EdgeLink_2_0"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[10].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[10].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[10].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[10].Vertices"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[11].CoverFlags").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[11].CoverFlags"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[11].EdgeLink_0_1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[11].EdgeLink_0_1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[11].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[11].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[11].EdgeLink_2_0").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[11].EdgeLink_2_0"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[11].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[11].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[11].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[11].Vertices"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[12].CoverFlags").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[12].CoverFlags"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[12].EdgeLink_0_1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[12].EdgeLink_0_1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[12].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[12].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[12].EdgeLink_2_0").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[12].EdgeLink_2_0"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[12].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[12].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[12].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[12].Vertices"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[13].CoverFlags").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[13].CoverFlags"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[13].EdgeLink_0_1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[13].EdgeLink_0_1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[13].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[13].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[13].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[13].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[13].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[13].Vertices"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[14].CoverFlags").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[14].CoverFlags"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[14].EdgeLink_0_1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[14].EdgeLink_0_1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[14].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[14].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[14].EdgeLink_2_0").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[14].EdgeLink_2_0"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[14].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[14].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[14].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[14].Vertices"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[15].Data").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[15].Data"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[15].Vertex1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[15].Vertex1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[15].Vertex2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[15].Vertex2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[16].Data").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[16].Data"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[16].Vertex1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[16].Vertex1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[16].Vertex2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[16].Vertex2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[17].Data").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[17].Data"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[17].Vertex1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[17].Vertex1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[18].Data").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[18].Data"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[18].Vertex2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[18].Vertex2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[19].Data").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[19].Data"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[19].Vertex1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[19].Vertex1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[19].Vertex2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[19].Vertex2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[2].Point").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[2].Point"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[20]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[20]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[21].Cover").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[21].Cover"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[21].Triangle").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[21].Triangle"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[22].Cover").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[22].Cover"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[22].Triangle").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[22].Triangle"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[23].Cover").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[23].Cover"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[23].Triangle").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[23].Triangle"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[24].Cover").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[24].Cover"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[24].Triangle").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[24].Triangle"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[25].Cover").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[25].Cover"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[25].Triangle").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[25].Triangle"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[26].Cover").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[26].Cover"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[26].Triangle").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[26].Triangle"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[27].Cover").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[27].Cover"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[27].Triangle").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[27].Triangle"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[28].GridCell.Count").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[28].GridCell.Count"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[28].GridCell[0]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[28].GridCell[0]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[28].GridCell[1]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[28].GridCell[1]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[28].GridCell[2]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[28].GridCell[2]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[28].GridCell[3]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[28].GridCell[3]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[28].GridCell[4]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[28].GridCell[4]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[28].GridCell[5]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[28].GridCell[5]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[28].GridCell[6]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[28].GridCell[6]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[28].GridCell[7]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[28].GridCell[7]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[28].GridCell[8]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[28].GridCell[8]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[28].GridCell[9]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[28].GridCell[9]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[3].Point").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[3].Point"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[4].Point").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[4].Point"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[5].Point").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[5].Point"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[6].Point").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[6].Point"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[7].Point").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[7].Point"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[8].Point").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[8].Point"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[9].CoverFlags").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[9].CoverFlags"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[9].EdgeLink_0_1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[9].EdgeLink_0_1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[9].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[9].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[9].EdgeLink_2_0").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[9].EdgeLink_2_0"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[9].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[9].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[9].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[9].Vertices"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.First").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsFirst"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.Second").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsSecond"));
        Helpers.GetSpriggitField(spriggit, "UnknownDNAMFloat").ShouldBe(Helpers.GetDTOField(dto, "UnknownDNAMFloat"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "EditorID", "FormKey", "FormVersion", "MaxAngle", "Model.File", "Model.LightLayer", "NavmeshGeometry.Count", "NavmeshGeometry.GridMax", "NavmeshGeometry.GridMaxDistance", "NavmeshGeometry.GridMin", "NavmeshGeometry.GridSize", "NavmeshGeometry.Parent.MutagenObjectType", "NavmeshGeometry.Parent.Parent", "NavmeshGeometry[0]", "NavmeshGeometry[1].Point", "NavmeshGeometry[10].CoverFlags", "NavmeshGeometry[10].EdgeLink_0_1", "NavmeshGeometry[10].EdgeLink_1_2", "NavmeshGeometry[10].EdgeLink_2_0", "NavmeshGeometry[10].Height", "NavmeshGeometry[10].Vertices", "NavmeshGeometry[11].CoverFlags", "NavmeshGeometry[11].EdgeLink_0_1", "NavmeshGeometry[11].EdgeLink_1_2", "NavmeshGeometry[11].EdgeLink_2_0", "NavmeshGeometry[11].Height", "NavmeshGeometry[11].Vertices", "NavmeshGeometry[12].CoverFlags", "NavmeshGeometry[12].EdgeLink_0_1", "NavmeshGeometry[12].EdgeLink_1_2", "NavmeshGeometry[12].EdgeLink_2_0", "NavmeshGeometry[12].Height", "NavmeshGeometry[12].Vertices", "NavmeshGeometry[13].CoverFlags", "NavmeshGeometry[13].EdgeLink_0_1", "NavmeshGeometry[13].EdgeLink_1_2", "NavmeshGeometry[13].Height", "NavmeshGeometry[13].Vertices", "NavmeshGeometry[14].CoverFlags", "NavmeshGeometry[14].EdgeLink_0_1", "NavmeshGeometry[14].EdgeLink_1_2", "NavmeshGeometry[14].EdgeLink_2_0", "NavmeshGeometry[14].Height", "NavmeshGeometry[14].Vertices", "NavmeshGeometry[15].Data", "NavmeshGeometry[15].Vertex1", "NavmeshGeometry[15].Vertex2", "NavmeshGeometry[16].Data", "NavmeshGeometry[16].Vertex1", "NavmeshGeometry[16].Vertex2", "NavmeshGeometry[17].Data", "NavmeshGeometry[17].Vertex1", "NavmeshGeometry[18].Data", "NavmeshGeometry[18].Vertex2", "NavmeshGeometry[19].Data", "NavmeshGeometry[19].Vertex1", "NavmeshGeometry[19].Vertex2", "NavmeshGeometry[2].Point", "NavmeshGeometry[20]", "NavmeshGeometry[21].Cover", "NavmeshGeometry[21].Triangle", "NavmeshGeometry[22].Cover", "NavmeshGeometry[22].Triangle", "NavmeshGeometry[23].Cover", "NavmeshGeometry[23].Triangle", "NavmeshGeometry[24].Cover", "NavmeshGeometry[24].Triangle", "NavmeshGeometry[25].Cover", "NavmeshGeometry[25].Triangle", "NavmeshGeometry[26].Cover", "NavmeshGeometry[26].Triangle", "NavmeshGeometry[27].Cover", "NavmeshGeometry[27].Triangle", "NavmeshGeometry[28].GridCell.Count", "NavmeshGeometry[28].GridCell[0]", "NavmeshGeometry[28].GridCell[1]", "NavmeshGeometry[28].GridCell[2]", "NavmeshGeometry[28].GridCell[3]", "NavmeshGeometry[28].GridCell[4]", "NavmeshGeometry[28].GridCell[5]", "NavmeshGeometry[28].GridCell[6]", "NavmeshGeometry[28].GridCell[7]", "NavmeshGeometry[28].GridCell[8]", "NavmeshGeometry[28].GridCell[9]", "NavmeshGeometry[3].Point", "NavmeshGeometry[4].Point", "NavmeshGeometry[5].Point", "NavmeshGeometry[6].Point", "NavmeshGeometry[7].Point", "NavmeshGeometry[8].Point", "NavmeshGeometry[9].CoverFlags", "NavmeshGeometry[9].EdgeLink_0_1", "NavmeshGeometry[9].EdgeLink_1_2", "NavmeshGeometry[9].EdgeLink_2_0", "NavmeshGeometry[9].Height", "NavmeshGeometry[9].Vertices", "ObjectBounds.First", "ObjectBounds.Second", "UnknownDNAMFloat", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "EditorID", "FormKey", "FormVersion", "MaxAngle", "Models[0].File", "Models[0].LightLayer", "NavmeshGeometry.Count", "NavmeshGeometry.GridMax", "NavmeshGeometry.GridMaxDistance", "NavmeshGeometry.GridMin", "NavmeshGeometry.GridSize", "NavmeshGeometry.Parent.MutagenObjectType", "NavmeshGeometry.Parent.Parent", "NavmeshGeometry[0]", "NavmeshGeometry[1].Point", "NavmeshGeometry[10].CoverFlags", "NavmeshGeometry[10].EdgeLink_0_1", "NavmeshGeometry[10].EdgeLink_1_2", "NavmeshGeometry[10].EdgeLink_2_0", "NavmeshGeometry[10].Height", "NavmeshGeometry[10].Vertices", "NavmeshGeometry[11].CoverFlags", "NavmeshGeometry[11].EdgeLink_0_1", "NavmeshGeometry[11].EdgeLink_1_2", "NavmeshGeometry[11].EdgeLink_2_0", "NavmeshGeometry[11].Height", "NavmeshGeometry[11].Vertices", "NavmeshGeometry[12].CoverFlags", "NavmeshGeometry[12].EdgeLink_0_1", "NavmeshGeometry[12].EdgeLink_1_2", "NavmeshGeometry[12].EdgeLink_2_0", "NavmeshGeometry[12].Height", "NavmeshGeometry[12].Vertices", "NavmeshGeometry[13].CoverFlags", "NavmeshGeometry[13].EdgeLink_0_1", "NavmeshGeometry[13].EdgeLink_1_2", "NavmeshGeometry[13].Height", "NavmeshGeometry[13].Vertices", "NavmeshGeometry[14].CoverFlags", "NavmeshGeometry[14].EdgeLink_0_1", "NavmeshGeometry[14].EdgeLink_1_2", "NavmeshGeometry[14].EdgeLink_2_0", "NavmeshGeometry[14].Height", "NavmeshGeometry[14].Vertices", "NavmeshGeometry[15].Data", "NavmeshGeometry[15].Vertex1", "NavmeshGeometry[15].Vertex2", "NavmeshGeometry[16].Data", "NavmeshGeometry[16].Vertex1", "NavmeshGeometry[16].Vertex2", "NavmeshGeometry[17].Data", "NavmeshGeometry[17].Vertex1", "NavmeshGeometry[18].Data", "NavmeshGeometry[18].Vertex2", "NavmeshGeometry[19].Data", "NavmeshGeometry[19].Vertex1", "NavmeshGeometry[19].Vertex2", "NavmeshGeometry[2].Point", "NavmeshGeometry[20]", "NavmeshGeometry[21].Cover", "NavmeshGeometry[21].Triangle", "NavmeshGeometry[22].Cover", "NavmeshGeometry[22].Triangle", "NavmeshGeometry[23].Cover", "NavmeshGeometry[23].Triangle", "NavmeshGeometry[24].Cover", "NavmeshGeometry[24].Triangle", "NavmeshGeometry[25].Cover", "NavmeshGeometry[25].Triangle", "NavmeshGeometry[26].Cover", "NavmeshGeometry[26].Triangle", "NavmeshGeometry[27].Cover", "NavmeshGeometry[27].Triangle", "NavmeshGeometry[28].GridCell.Count", "NavmeshGeometry[28].GridCell[0]", "NavmeshGeometry[28].GridCell[1]", "NavmeshGeometry[28].GridCell[2]", "NavmeshGeometry[28].GridCell[3]", "NavmeshGeometry[28].GridCell[4]", "NavmeshGeometry[28].GridCell[5]", "NavmeshGeometry[28].GridCell[6]", "NavmeshGeometry[28].GridCell[7]", "NavmeshGeometry[28].GridCell[8]", "NavmeshGeometry[28].GridCell[9]", "NavmeshGeometry[3].Point", "NavmeshGeometry[4].Point", "NavmeshGeometry[5].Point", "NavmeshGeometry[6].Point", "NavmeshGeometry[7].Point", "NavmeshGeometry[8].Point", "NavmeshGeometry[9].CoverFlags", "NavmeshGeometry[9].EdgeLink_0_1", "NavmeshGeometry[9].EdgeLink_1_2", "NavmeshGeometry[9].EdgeLink_2_0", "NavmeshGeometry[9].Height", "NavmeshGeometry[9].Vertices", "ObjectBoundsFirst", "ObjectBoundsSecond", "UnknownDNAMFloat", "Version2", "VersionControl");
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

        Helpers.GetSpriggitField(spriggit, "DirtinessScale").ShouldBe(Helpers.GetDTOField(dto, "DirtinessScale"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "MaxAngle").ShouldBe(Helpers.GetDTOField(dto, "MaxAngle"));
        Helpers.GetSpriggitField(spriggit, "Model.File").ShouldBe(Helpers.GetDTOField(dto, "Models[0].File"));
        Helpers.GetSpriggitField(spriggit, "Model.LightLayer").ShouldBe(Helpers.GetDTOField(dto, "Models[0].LightLayer"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.Count").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.Count"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridMax").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridMax"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridMaxDistance").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridMaxDistance"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridMin").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridMin"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridSize").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridSize"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.Parent.MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.Parent.MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.Parent.Parent").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.Parent.Parent"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[0]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[0]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[1].Point").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[1].Point"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[10].EdgeLink_0_1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[10].EdgeLink_0_1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[10].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[10].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[10].EdgeLink_2_0").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[10].EdgeLink_2_0"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[10].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[10].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[10].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[10].Vertices"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[11].EdgeLink_0_1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[11].EdgeLink_0_1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[11].EdgeLink_2_0").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[11].EdgeLink_2_0"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[11].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[11].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[11].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[11].Vertices"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[12].EdgeLink_0_1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[12].EdgeLink_0_1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[12].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[12].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[12].EdgeLink_2_0").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[12].EdgeLink_2_0"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[12].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[12].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[12].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[12].Vertices"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[13].EdgeLink_0_1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[13].EdgeLink_0_1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[13].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[13].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[13].EdgeLink_2_0").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[13].EdgeLink_2_0"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[13].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[13].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[13].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[13].Vertices"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[14].EdgeLink_0_1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[14].EdgeLink_0_1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[14].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[14].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[14].EdgeLink_2_0").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[14].EdgeLink_2_0"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[14].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[14].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[14].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[14].Vertices"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[15].GridCell.Count").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[15].GridCell.Count"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[15].GridCell[0]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[15].GridCell[0]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[15].GridCell[1]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[15].GridCell[1]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[15].GridCell[2]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[15].GridCell[2]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[15].GridCell[3]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[15].GridCell[3]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[15].GridCell[4]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[15].GridCell[4]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[15].GridCell[5]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[15].GridCell[5]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[15].GridCell[6]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[15].GridCell[6]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[15].GridCell[7]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[15].GridCell[7]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[15].GridCell[8]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[15].GridCell[8]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[15].GridCell[9]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[15].GridCell[9]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[2].Point").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[2].Point"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[3].Point").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[3].Point"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[4].Point").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[4].Point"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[5].Point").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[5].Point"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[6].Point").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[6].Point"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[7].Point").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[7].Point"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[8].Point").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[8].Point"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[9].EdgeLink_0_1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[9].EdgeLink_0_1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[9].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[9].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[9].EdgeLink_2_0").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[9].EdgeLink_2_0"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[9].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[9].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[9].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[9].Vertices"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.First").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsFirst"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.Second").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsSecond"));
        Helpers.GetSpriggitField(spriggit, "UnknownDNAMFloat").ShouldBe(Helpers.GetDTOField(dto, "UnknownDNAMFloat"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "DirtinessScale", "EditorID", "FormKey", "FormVersion", "MaxAngle", "Model.File", "Model.LightLayer", "NavmeshGeometry.Count", "NavmeshGeometry.GridMax", "NavmeshGeometry.GridMaxDistance", "NavmeshGeometry.GridMin", "NavmeshGeometry.GridSize", "NavmeshGeometry.Parent.MutagenObjectType", "NavmeshGeometry.Parent.Parent", "NavmeshGeometry[0]", "NavmeshGeometry[1].Point", "NavmeshGeometry[10].EdgeLink_0_1", "NavmeshGeometry[10].EdgeLink_1_2", "NavmeshGeometry[10].EdgeLink_2_0", "NavmeshGeometry[10].Height", "NavmeshGeometry[10].Vertices", "NavmeshGeometry[11].EdgeLink_0_1", "NavmeshGeometry[11].EdgeLink_2_0", "NavmeshGeometry[11].Height", "NavmeshGeometry[11].Vertices", "NavmeshGeometry[12].EdgeLink_0_1", "NavmeshGeometry[12].EdgeLink_1_2", "NavmeshGeometry[12].EdgeLink_2_0", "NavmeshGeometry[12].Height", "NavmeshGeometry[12].Vertices", "NavmeshGeometry[13].EdgeLink_0_1", "NavmeshGeometry[13].EdgeLink_1_2", "NavmeshGeometry[13].EdgeLink_2_0", "NavmeshGeometry[13].Height", "NavmeshGeometry[13].Vertices", "NavmeshGeometry[14].EdgeLink_0_1", "NavmeshGeometry[14].EdgeLink_1_2", "NavmeshGeometry[14].EdgeLink_2_0", "NavmeshGeometry[14].Height", "NavmeshGeometry[14].Vertices", "NavmeshGeometry[15].GridCell.Count", "NavmeshGeometry[15].GridCell[0]", "NavmeshGeometry[15].GridCell[1]", "NavmeshGeometry[15].GridCell[2]", "NavmeshGeometry[15].GridCell[3]", "NavmeshGeometry[15].GridCell[4]", "NavmeshGeometry[15].GridCell[5]", "NavmeshGeometry[15].GridCell[6]", "NavmeshGeometry[15].GridCell[7]", "NavmeshGeometry[15].GridCell[8]", "NavmeshGeometry[15].GridCell[9]", "NavmeshGeometry[2].Point", "NavmeshGeometry[3].Point", "NavmeshGeometry[4].Point", "NavmeshGeometry[5].Point", "NavmeshGeometry[6].Point", "NavmeshGeometry[7].Point", "NavmeshGeometry[8].Point", "NavmeshGeometry[9].EdgeLink_0_1", "NavmeshGeometry[9].EdgeLink_1_2", "NavmeshGeometry[9].EdgeLink_2_0", "NavmeshGeometry[9].Height", "NavmeshGeometry[9].Vertices", "ObjectBounds.First", "ObjectBounds.Second", "UnknownDNAMFloat", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "DirtinessScale", "EditorID", "FormKey", "FormVersion", "MaxAngle", "Models[0].File", "Models[0].LightLayer", "NavmeshGeometry.Count", "NavmeshGeometry.GridMax", "NavmeshGeometry.GridMaxDistance", "NavmeshGeometry.GridMin", "NavmeshGeometry.GridSize", "NavmeshGeometry.Parent.MutagenObjectType", "NavmeshGeometry.Parent.Parent", "NavmeshGeometry[0]", "NavmeshGeometry[1].Point", "NavmeshGeometry[10].EdgeLink_0_1", "NavmeshGeometry[10].EdgeLink_1_2", "NavmeshGeometry[10].EdgeLink_2_0", "NavmeshGeometry[10].Height", "NavmeshGeometry[10].Vertices", "NavmeshGeometry[11].EdgeLink_0_1", "NavmeshGeometry[11].EdgeLink_2_0", "NavmeshGeometry[11].Height", "NavmeshGeometry[11].Vertices", "NavmeshGeometry[12].EdgeLink_0_1", "NavmeshGeometry[12].EdgeLink_1_2", "NavmeshGeometry[12].EdgeLink_2_0", "NavmeshGeometry[12].Height", "NavmeshGeometry[12].Vertices", "NavmeshGeometry[13].EdgeLink_0_1", "NavmeshGeometry[13].EdgeLink_1_2", "NavmeshGeometry[13].EdgeLink_2_0", "NavmeshGeometry[13].Height", "NavmeshGeometry[13].Vertices", "NavmeshGeometry[14].EdgeLink_0_1", "NavmeshGeometry[14].EdgeLink_1_2", "NavmeshGeometry[14].EdgeLink_2_0", "NavmeshGeometry[14].Height", "NavmeshGeometry[14].Vertices", "NavmeshGeometry[15].GridCell.Count", "NavmeshGeometry[15].GridCell[0]", "NavmeshGeometry[15].GridCell[1]", "NavmeshGeometry[15].GridCell[2]", "NavmeshGeometry[15].GridCell[3]", "NavmeshGeometry[15].GridCell[4]", "NavmeshGeometry[15].GridCell[5]", "NavmeshGeometry[15].GridCell[6]", "NavmeshGeometry[15].GridCell[7]", "NavmeshGeometry[15].GridCell[8]", "NavmeshGeometry[15].GridCell[9]", "NavmeshGeometry[2].Point", "NavmeshGeometry[3].Point", "NavmeshGeometry[4].Point", "NavmeshGeometry[5].Point", "NavmeshGeometry[6].Point", "NavmeshGeometry[7].Point", "NavmeshGeometry[8].Point", "NavmeshGeometry[9].EdgeLink_0_1", "NavmeshGeometry[9].EdgeLink_1_2", "NavmeshGeometry[9].EdgeLink_2_0", "NavmeshGeometry[9].Height", "NavmeshGeometry[9].Vertices", "ObjectBoundsFirst", "ObjectBoundsSecond", "UnknownDNAMFloat", "Version2", "VersionControl");
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

        Helpers.GetSpriggitField(spriggit, "DirtinessScale").ShouldBe(Helpers.GetDTOField(dto, "DirtinessScale"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "MaxAngle").ShouldBe(Helpers.GetDTOField(dto, "MaxAngle"));
        Helpers.GetSpriggitField(spriggit, "Model.File").ShouldBe(Helpers.GetDTOField(dto, "Models[0].File"));
        Helpers.GetSpriggitField(spriggit, "Model.LightLayer").ShouldBe(Helpers.GetDTOField(dto, "Models[0].LightLayer"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.Count").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.Count"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridMax").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridMax"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridMaxDistance").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridMaxDistance"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridMin").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridMin"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.GridSize").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.GridSize"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.Parent.MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.Parent.MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry.Parent.Parent").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry.Parent.Parent"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[0]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[0]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[1].Point").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[1].Point"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[10].Point").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[10].Point"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[11].Point").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[11].Point"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[12].Point").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[12].Point"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[13].Point").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[13].Point"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[14].Point").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[14].Point"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[15].EdgeLink_0_1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[15].EdgeLink_0_1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[15].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[15].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[15].EdgeLink_2_0").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[15].EdgeLink_2_0"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[15].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[15].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[15].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[15].Vertices"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[16].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[16].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[16].EdgeLink_2_0").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[16].EdgeLink_2_0"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[16].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[16].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[16].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[16].Vertices"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[17].EdgeLink_0_1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[17].EdgeLink_0_1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[17].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[17].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[17].EdgeLink_2_0").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[17].EdgeLink_2_0"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[17].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[17].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[17].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[17].Vertices"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[18].EdgeLink_0_1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[18].EdgeLink_0_1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[18].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[18].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[18].EdgeLink_2_0").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[18].EdgeLink_2_0"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[18].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[18].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[18].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[18].Vertices"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[19].EdgeLink_0_1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[19].EdgeLink_0_1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[19].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[19].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[19].EdgeLink_2_0").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[19].EdgeLink_2_0"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[19].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[19].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[19].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[19].Vertices"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[2].Point").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[2].Point"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[20].EdgeLink_0_1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[20].EdgeLink_0_1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[20].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[20].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[20].EdgeLink_2_0").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[20].EdgeLink_2_0"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[20].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[20].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[20].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[20].Vertices"));
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
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[27].GridCell.Count").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[27].GridCell.Count"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[27].GridCell[0]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[27].GridCell[0]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[27].GridCell[1]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[27].GridCell[1]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[27].GridCell[10]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[27].GridCell[10]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[27].GridCell[11]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[27].GridCell[11]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[27].GridCell[12]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[27].GridCell[12]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[27].GridCell[13]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[27].GridCell[13]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[27].GridCell[14]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[27].GridCell[14]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[27].GridCell[15]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[27].GridCell[15]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[27].GridCell[2]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[27].GridCell[2]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[27].GridCell[3]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[27].GridCell[3]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[27].GridCell[4]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[27].GridCell[4]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[27].GridCell[5]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[27].GridCell[5]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[27].GridCell[6]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[27].GridCell[6]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[27].GridCell[7]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[27].GridCell[7]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[27].GridCell[8]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[27].GridCell[8]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[27].GridCell[9]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[27].GridCell[9]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[3].Point").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[3].Point"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[4].Point").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[4].Point"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[5].Point").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[5].Point"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[6].Point").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[6].Point"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[7].Point").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[7].Point"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[8].Point").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[8].Point"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[9].Point").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[9].Point"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.First").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsFirst"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.Second").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsSecond"));
        Helpers.GetSpriggitField(spriggit, "REFL").ShouldBe(Helpers.GetDTOField(dto, "REFL"));
        Helpers.GetSpriggitField(spriggit, "UnknownDNAMFloat").ShouldBe(Helpers.GetDTOField(dto, "UnknownDNAMFloat"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "DirtinessScale", "EditorID", "FormKey", "FormVersion", "MaxAngle", "Model.File", "Model.LightLayer", "NavmeshGeometry.Count", "NavmeshGeometry.GridMax", "NavmeshGeometry.GridMaxDistance", "NavmeshGeometry.GridMin", "NavmeshGeometry.GridSize", "NavmeshGeometry.Parent.MutagenObjectType", "NavmeshGeometry.Parent.Parent", "NavmeshGeometry[0]", "NavmeshGeometry[1].Point", "NavmeshGeometry[10].Point", "NavmeshGeometry[11].Point", "NavmeshGeometry[12].Point", "NavmeshGeometry[13].Point", "NavmeshGeometry[14].Point", "NavmeshGeometry[15].EdgeLink_0_1", "NavmeshGeometry[15].EdgeLink_1_2", "NavmeshGeometry[15].EdgeLink_2_0", "NavmeshGeometry[15].Height", "NavmeshGeometry[15].Vertices", "NavmeshGeometry[16].EdgeLink_1_2", "NavmeshGeometry[16].EdgeLink_2_0", "NavmeshGeometry[16].Height", "NavmeshGeometry[16].Vertices", "NavmeshGeometry[17].EdgeLink_0_1", "NavmeshGeometry[17].EdgeLink_1_2", "NavmeshGeometry[17].EdgeLink_2_0", "NavmeshGeometry[17].Height", "NavmeshGeometry[17].Vertices", "NavmeshGeometry[18].EdgeLink_0_1", "NavmeshGeometry[18].EdgeLink_1_2", "NavmeshGeometry[18].EdgeLink_2_0", "NavmeshGeometry[18].Height", "NavmeshGeometry[18].Vertices", "NavmeshGeometry[19].EdgeLink_0_1", "NavmeshGeometry[19].EdgeLink_1_2", "NavmeshGeometry[19].EdgeLink_2_0", "NavmeshGeometry[19].Height", "NavmeshGeometry[19].Vertices", "NavmeshGeometry[2].Point", "NavmeshGeometry[20].EdgeLink_0_1", "NavmeshGeometry[20].EdgeLink_1_2", "NavmeshGeometry[20].EdgeLink_2_0", "NavmeshGeometry[20].Height", "NavmeshGeometry[20].Vertices", "NavmeshGeometry[21].EdgeLink_0_1", "NavmeshGeometry[21].EdgeLink_1_2", "NavmeshGeometry[21].EdgeLink_2_0", "NavmeshGeometry[21].Height", "NavmeshGeometry[21].Vertices", "NavmeshGeometry[22].EdgeLink_0_1", "NavmeshGeometry[22].EdgeLink_1_2", "NavmeshGeometry[22].EdgeLink_2_0", "NavmeshGeometry[22].Height", "NavmeshGeometry[22].Vertices", "NavmeshGeometry[23].EdgeLink_0_1", "NavmeshGeometry[23].EdgeLink_1_2", "NavmeshGeometry[23].EdgeLink_2_0", "NavmeshGeometry[23].Height", "NavmeshGeometry[23].Vertices", "NavmeshGeometry[24].EdgeLink_0_1", "NavmeshGeometry[24].EdgeLink_1_2", "NavmeshGeometry[24].EdgeLink_2_0", "NavmeshGeometry[24].Height", "NavmeshGeometry[24].Vertices", "NavmeshGeometry[25].EdgeLink_0_1", "NavmeshGeometry[25].EdgeLink_1_2", "NavmeshGeometry[25].EdgeLink_2_0", "NavmeshGeometry[25].Height", "NavmeshGeometry[25].Vertices", "NavmeshGeometry[26].EdgeLink_0_1", "NavmeshGeometry[26].EdgeLink_1_2", "NavmeshGeometry[26].EdgeLink_2_0", "NavmeshGeometry[26].Height", "NavmeshGeometry[26].Vertices", "NavmeshGeometry[27].GridCell.Count", "NavmeshGeometry[27].GridCell[0]", "NavmeshGeometry[27].GridCell[1]", "NavmeshGeometry[27].GridCell[10]", "NavmeshGeometry[27].GridCell[11]", "NavmeshGeometry[27].GridCell[12]", "NavmeshGeometry[27].GridCell[13]", "NavmeshGeometry[27].GridCell[14]", "NavmeshGeometry[27].GridCell[15]", "NavmeshGeometry[27].GridCell[2]", "NavmeshGeometry[27].GridCell[3]", "NavmeshGeometry[27].GridCell[4]", "NavmeshGeometry[27].GridCell[5]", "NavmeshGeometry[27].GridCell[6]", "NavmeshGeometry[27].GridCell[7]", "NavmeshGeometry[27].GridCell[8]", "NavmeshGeometry[27].GridCell[9]", "NavmeshGeometry[3].Point", "NavmeshGeometry[4].Point", "NavmeshGeometry[5].Point", "NavmeshGeometry[6].Point", "NavmeshGeometry[7].Point", "NavmeshGeometry[8].Point", "NavmeshGeometry[9].Point", "ObjectBounds.First", "ObjectBounds.Second", "REFL", "UnknownDNAMFloat", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "DirtinessScale", "EditorID", "FormKey", "FormVersion", "MaxAngle", "Models[0].File", "Models[0].LightLayer", "NavmeshGeometry.Count", "NavmeshGeometry.GridMax", "NavmeshGeometry.GridMaxDistance", "NavmeshGeometry.GridMin", "NavmeshGeometry.GridSize", "NavmeshGeometry.Parent.MutagenObjectType", "NavmeshGeometry.Parent.Parent", "NavmeshGeometry[0]", "NavmeshGeometry[1].Point", "NavmeshGeometry[10].Point", "NavmeshGeometry[11].Point", "NavmeshGeometry[12].Point", "NavmeshGeometry[13].Point", "NavmeshGeometry[14].Point", "NavmeshGeometry[15].EdgeLink_0_1", "NavmeshGeometry[15].EdgeLink_1_2", "NavmeshGeometry[15].EdgeLink_2_0", "NavmeshGeometry[15].Height", "NavmeshGeometry[15].Vertices", "NavmeshGeometry[16].EdgeLink_1_2", "NavmeshGeometry[16].EdgeLink_2_0", "NavmeshGeometry[16].Height", "NavmeshGeometry[16].Vertices", "NavmeshGeometry[17].EdgeLink_0_1", "NavmeshGeometry[17].EdgeLink_1_2", "NavmeshGeometry[17].EdgeLink_2_0", "NavmeshGeometry[17].Height", "NavmeshGeometry[17].Vertices", "NavmeshGeometry[18].EdgeLink_0_1", "NavmeshGeometry[18].EdgeLink_1_2", "NavmeshGeometry[18].EdgeLink_2_0", "NavmeshGeometry[18].Height", "NavmeshGeometry[18].Vertices", "NavmeshGeometry[19].EdgeLink_0_1", "NavmeshGeometry[19].EdgeLink_1_2", "NavmeshGeometry[19].EdgeLink_2_0", "NavmeshGeometry[19].Height", "NavmeshGeometry[19].Vertices", "NavmeshGeometry[2].Point", "NavmeshGeometry[20].EdgeLink_0_1", "NavmeshGeometry[20].EdgeLink_1_2", "NavmeshGeometry[20].EdgeLink_2_0", "NavmeshGeometry[20].Height", "NavmeshGeometry[20].Vertices", "NavmeshGeometry[21].EdgeLink_0_1", "NavmeshGeometry[21].EdgeLink_1_2", "NavmeshGeometry[21].EdgeLink_2_0", "NavmeshGeometry[21].Height", "NavmeshGeometry[21].Vertices", "NavmeshGeometry[22].EdgeLink_0_1", "NavmeshGeometry[22].EdgeLink_1_2", "NavmeshGeometry[22].EdgeLink_2_0", "NavmeshGeometry[22].Height", "NavmeshGeometry[22].Vertices", "NavmeshGeometry[23].EdgeLink_0_1", "NavmeshGeometry[23].EdgeLink_1_2", "NavmeshGeometry[23].EdgeLink_2_0", "NavmeshGeometry[23].Height", "NavmeshGeometry[23].Vertices", "NavmeshGeometry[24].EdgeLink_0_1", "NavmeshGeometry[24].EdgeLink_1_2", "NavmeshGeometry[24].EdgeLink_2_0", "NavmeshGeometry[24].Height", "NavmeshGeometry[24].Vertices", "NavmeshGeometry[25].EdgeLink_0_1", "NavmeshGeometry[25].EdgeLink_1_2", "NavmeshGeometry[25].EdgeLink_2_0", "NavmeshGeometry[25].Height", "NavmeshGeometry[25].Vertices", "NavmeshGeometry[26].EdgeLink_0_1", "NavmeshGeometry[26].EdgeLink_1_2", "NavmeshGeometry[26].EdgeLink_2_0", "NavmeshGeometry[26].Height", "NavmeshGeometry[26].Vertices", "NavmeshGeometry[27].GridCell.Count", "NavmeshGeometry[27].GridCell[0]", "NavmeshGeometry[27].GridCell[1]", "NavmeshGeometry[27].GridCell[10]", "NavmeshGeometry[27].GridCell[11]", "NavmeshGeometry[27].GridCell[12]", "NavmeshGeometry[27].GridCell[13]", "NavmeshGeometry[27].GridCell[14]", "NavmeshGeometry[27].GridCell[15]", "NavmeshGeometry[27].GridCell[2]", "NavmeshGeometry[27].GridCell[3]", "NavmeshGeometry[27].GridCell[4]", "NavmeshGeometry[27].GridCell[5]", "NavmeshGeometry[27].GridCell[6]", "NavmeshGeometry[27].GridCell[7]", "NavmeshGeometry[27].GridCell[8]", "NavmeshGeometry[27].GridCell[9]", "NavmeshGeometry[3].Point", "NavmeshGeometry[4].Point", "NavmeshGeometry[5].Point", "NavmeshGeometry[6].Point", "NavmeshGeometry[7].Point", "NavmeshGeometry[8].Point", "NavmeshGeometry[9].Point", "ObjectBoundsFirst", "ObjectBoundsSecond", "REFL", "UnknownDNAMFloat", "Version2", "VersionControl");
    }
}