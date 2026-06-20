using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.Door.Starfield;

public class StarfieldDoorSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "DOOR")]
    [Trait("FormKey", "144F85:Starfield.esm")]
    [Trait("EditorID", "ShipFloorLoadHatch")]
    [Trait("SpriggitFile", "Doors/ShipFloorLoadHatch - 144F85_Starfield.esm.yaml")]
    public void Starfield_DOOR_ShouldMatchSpriggitSample_ShipFloorLoadHatch()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Door,
            "ShipFloorLoadHatch");
        var dto = Helpers.GetDTO<DoorDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Door,
            "144F85:Starfield.esm");

        Helpers.GetSpriggitField(spriggit, "ANAM").ShouldBe(Helpers.GetDTOField(dto, "ANAM"));
        Helpers.GetSpriggitField(spriggit, "BNAM").ShouldBe(Helpers.GetDTOField(dto, "BNAM"));
        Helpers.GetSpriggitField(spriggit, "CloseSound.Start").ShouldBe(Helpers.GetDTOField(dto, "CloseSound.Start"));
        Helpers.GetSpriggitField(spriggit, "CNAM").ShouldBe(Helpers.GetDTOField(dto, "CNAM"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FacingAxisOverride").ShouldBe(Helpers.GetDTOField(dto, "FacingAxisOverride"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "Model.File").ShouldBe(Helpers.GetDTOField(dto, "Models[0].File"));
        Helpers.GetSpriggitField(spriggit, "Model.LightLayer").ShouldBe(Helpers.GetDTOField(dto, "Models[0].LightLayer"));
        Helpers.GetSpriggitField(spriggit, "Name.Count").ShouldBe(Helpers.GetDTOField(dto, "Name.Count"));
        Helpers.GetSpriggitField(spriggit, "Name.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Name.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Name[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[0].String").ShouldBe(Helpers.GetDTOField(dto, "Name[0].String"));
        Helpers.GetSpriggitField(spriggit, "Name[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[1].String").ShouldBe(Helpers.GetDTOField(dto, "Name[1].String"));
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
        Helpers.GetSpriggitField(spriggit, "NativeTerminal").ShouldBe(Helpers.GetDTOField(dto, "NativeTerminalFormKey"));
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
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[10].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[10].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[10].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[10].Vertices"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[11].GridCell.Count").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[11].GridCell.Count"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[11].GridCell[0]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[11].GridCell[0]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[11].GridCell[1]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[11].GridCell[1]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[11].GridCell[2]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[11].GridCell[2]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[11].GridCell[3]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[11].GridCell[3]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[11].GridCell[4]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[11].GridCell[4]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[11].GridCell[5]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[11].GridCell[5]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[11].GridCell[6]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[11].GridCell[6]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[11].GridCell[7]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[11].GridCell[7]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[2].Point").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[2].Point"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[3].Point").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[3].Point"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[4].Point").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[4].Point"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[5].Point").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[5].Point"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[6].Point").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[6].Point"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[7].EdgeLink_0_1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[7].EdgeLink_0_1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[7].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[7].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[7].EdgeLink_2_0").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[7].EdgeLink_2_0"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[7].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[7].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[7].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[7].Vertices"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[8].EdgeLink_0_1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[8].EdgeLink_0_1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[8].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[8].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[8].EdgeLink_2_0").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[8].EdgeLink_2_0"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[8].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[8].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[8].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[8].Vertices"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[9].EdgeLink_0_1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[9].EdgeLink_0_1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[9].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[9].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[9].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[9].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[9].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[9].Vertices"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.First").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsFirst"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.Second").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsSecond"));
        Helpers.GetSpriggitField(spriggit, "OpenSound.Start").ShouldBe(Helpers.GetDTOField(dto, "OpenSound.Start"));
        Helpers.GetSpriggitField(spriggit, "SoundLevel").ShouldBe(Helpers.GetDTOField(dto, "SoundLevel"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "ANAM", "BNAM", "CloseSound.Start", "CNAM", "EditorID", "FacingAxisOverride", "FormKey", "FormVersion", "Model.File", "Model.LightLayer", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "NativeTerminal", "NavmeshGeometry.Count", "NavmeshGeometry.GridMax", "NavmeshGeometry.GridMaxDistance", "NavmeshGeometry.GridMin", "NavmeshGeometry.GridSize", "NavmeshGeometry.Parent.MutagenObjectType", "NavmeshGeometry.Parent.Parent", "NavmeshGeometry[0]", "NavmeshGeometry[1].Point", "NavmeshGeometry[10].EdgeLink_0_1", "NavmeshGeometry[10].EdgeLink_1_2", "NavmeshGeometry[10].Height", "NavmeshGeometry[10].Vertices", "NavmeshGeometry[11].GridCell.Count", "NavmeshGeometry[11].GridCell[0]", "NavmeshGeometry[11].GridCell[1]", "NavmeshGeometry[11].GridCell[2]", "NavmeshGeometry[11].GridCell[3]", "NavmeshGeometry[11].GridCell[4]", "NavmeshGeometry[11].GridCell[5]", "NavmeshGeometry[11].GridCell[6]", "NavmeshGeometry[11].GridCell[7]", "NavmeshGeometry[2].Point", "NavmeshGeometry[3].Point", "NavmeshGeometry[4].Point", "NavmeshGeometry[5].Point", "NavmeshGeometry[6].Point", "NavmeshGeometry[7].EdgeLink_0_1", "NavmeshGeometry[7].EdgeLink_1_2", "NavmeshGeometry[7].EdgeLink_2_0", "NavmeshGeometry[7].Height", "NavmeshGeometry[7].Vertices", "NavmeshGeometry[8].EdgeLink_0_1", "NavmeshGeometry[8].EdgeLink_1_2", "NavmeshGeometry[8].EdgeLink_2_0", "NavmeshGeometry[8].Height", "NavmeshGeometry[8].Vertices", "NavmeshGeometry[9].EdgeLink_0_1", "NavmeshGeometry[9].EdgeLink_1_2", "NavmeshGeometry[9].Height", "NavmeshGeometry[9].Vertices", "ObjectBounds.First", "ObjectBounds.Second", "OpenSound.Start", "SoundLevel", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "ANAM", "BNAM", "CloseSound.Start", "CNAM", "EditorID", "FacingAxisOverride", "FormKey", "FormVersion", "Models[0].File", "Models[0].LightLayer", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "NativeTerminalFormKey", "NavmeshGeometry.Count", "NavmeshGeometry.GridMax", "NavmeshGeometry.GridMaxDistance", "NavmeshGeometry.GridMin", "NavmeshGeometry.GridSize", "NavmeshGeometry.Parent.MutagenObjectType", "NavmeshGeometry.Parent.Parent", "NavmeshGeometry[0]", "NavmeshGeometry[1].Point", "NavmeshGeometry[10].EdgeLink_0_1", "NavmeshGeometry[10].EdgeLink_1_2", "NavmeshGeometry[10].Height", "NavmeshGeometry[10].Vertices", "NavmeshGeometry[11].GridCell.Count", "NavmeshGeometry[11].GridCell[0]", "NavmeshGeometry[11].GridCell[1]", "NavmeshGeometry[11].GridCell[2]", "NavmeshGeometry[11].GridCell[3]", "NavmeshGeometry[11].GridCell[4]", "NavmeshGeometry[11].GridCell[5]", "NavmeshGeometry[11].GridCell[6]", "NavmeshGeometry[11].GridCell[7]", "NavmeshGeometry[2].Point", "NavmeshGeometry[3].Point", "NavmeshGeometry[4].Point", "NavmeshGeometry[5].Point", "NavmeshGeometry[6].Point", "NavmeshGeometry[7].EdgeLink_0_1", "NavmeshGeometry[7].EdgeLink_1_2", "NavmeshGeometry[7].EdgeLink_2_0", "NavmeshGeometry[7].Height", "NavmeshGeometry[7].Vertices", "NavmeshGeometry[8].EdgeLink_0_1", "NavmeshGeometry[8].EdgeLink_1_2", "NavmeshGeometry[8].EdgeLink_2_0", "NavmeshGeometry[8].Height", "NavmeshGeometry[8].Vertices", "NavmeshGeometry[9].EdgeLink_0_1", "NavmeshGeometry[9].EdgeLink_1_2", "NavmeshGeometry[9].Height", "NavmeshGeometry[9].Vertices", "ObjectBoundsFirst", "ObjectBoundsSecond", "OpenSound.Start", "SoundLevel", "Version2", "VersionControl");
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "DOOR")]
    [Trait("FormKey", "205AA6:Starfield.esm")]
    [Trait("EditorID", "ShipDockingHatchFloor")]
    [Trait("SpriggitFile", "Doors/ShipDockingHatchFloor - 205AA6_Starfield.esm.yaml")]
    public void Starfield_DOOR_ShouldMatchSpriggitSample_ShipDockingHatchFloor()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Door,
            "ShipDockingHatchFloor");
        var dto = Helpers.GetDTO<DoorDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Door,
            "205AA6:Starfield.esm");

        Helpers.GetSpriggitField(spriggit, "ANAM").ShouldBe(Helpers.GetDTOField(dto, "ANAM"));
        Helpers.GetSpriggitField(spriggit, "BNAM").ShouldBe(Helpers.GetDTOField(dto, "BNAM"));
        Helpers.GetSpriggitField(spriggit, "CloseSound.Start").ShouldBe(Helpers.GetDTOField(dto, "CloseSound.Start"));
        Helpers.GetSpriggitField(spriggit, "CNAM").ShouldBe(Helpers.GetDTOField(dto, "CNAM"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FacingAxisOverride").ShouldBe(Helpers.GetDTOField(dto, "FacingAxisOverride"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "Model.File").ShouldBe(Helpers.GetDTOField(dto, "Models[0].File"));
        Helpers.GetSpriggitField(spriggit, "Model.LightLayer").ShouldBe(Helpers.GetDTOField(dto, "Models[0].LightLayer"));
        Helpers.GetSpriggitField(spriggit, "Name.Count").ShouldBe(Helpers.GetDTOField(dto, "Name.Count"));
        Helpers.GetSpriggitField(spriggit, "Name.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Name.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Name[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[0].String").ShouldBe(Helpers.GetDTOField(dto, "Name[0].String"));
        Helpers.GetSpriggitField(spriggit, "Name[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[1].String").ShouldBe(Helpers.GetDTOField(dto, "Name[1].String"));
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
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[11].GridCell.Count").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[11].GridCell.Count"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[11].GridCell[0]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[11].GridCell[0]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[11].GridCell[1]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[11].GridCell[1]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[11].GridCell[2]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[11].GridCell[2]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[11].GridCell[3]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[11].GridCell[3]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[11].GridCell[4]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[11].GridCell[4]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[11].GridCell[5]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[11].GridCell[5]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[11].GridCell[6]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[11].GridCell[6]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[11].GridCell[7]").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[11].GridCell[7]"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[2].Point").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[2].Point"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[3].Point").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[3].Point"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[4].Point").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[4].Point"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[5].Point").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[5].Point"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[6].Point").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[6].Point"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[7].EdgeLink_0_1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[7].EdgeLink_0_1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[7].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[7].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[7].EdgeLink_2_0").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[7].EdgeLink_2_0"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[7].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[7].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[7].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[7].Vertices"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[8].EdgeLink_0_1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[8].EdgeLink_0_1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[8].EdgeLink_1_2").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[8].EdgeLink_1_2"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[8].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[8].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[8].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[8].Vertices"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[9].EdgeLink_0_1").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[9].EdgeLink_0_1"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[9].EdgeLink_2_0").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[9].EdgeLink_2_0"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[9].Height").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[9].Height"));
        Helpers.GetSpriggitField(spriggit, "NavmeshGeometry[9].Vertices").ShouldBe(Helpers.GetDTOField(dto, "NavmeshGeometry[9].Vertices"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.First").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsFirst"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.Second").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsSecond"));
        Helpers.GetSpriggitField(spriggit, "OpenSound.Start").ShouldBe(Helpers.GetDTOField(dto, "OpenSound.Start"));
        Helpers.GetSpriggitField(spriggit, "SoundLevel").ShouldBe(Helpers.GetDTOField(dto, "SoundLevel"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "ANAM", "BNAM", "CloseSound.Start", "CNAM", "EditorID", "FacingAxisOverride", "FormKey", "FormVersion", "Model.File", "Model.LightLayer", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "NavmeshGeometry.Count", "NavmeshGeometry.GridMax", "NavmeshGeometry.GridMaxDistance", "NavmeshGeometry.GridMin", "NavmeshGeometry.GridSize", "NavmeshGeometry.Parent.MutagenObjectType", "NavmeshGeometry.Parent.Parent", "NavmeshGeometry[0]", "NavmeshGeometry[1].Point", "NavmeshGeometry[10].EdgeLink_0_1", "NavmeshGeometry[10].EdgeLink_1_2", "NavmeshGeometry[10].EdgeLink_2_0", "NavmeshGeometry[10].Height", "NavmeshGeometry[10].Vertices", "NavmeshGeometry[11].GridCell.Count", "NavmeshGeometry[11].GridCell[0]", "NavmeshGeometry[11].GridCell[1]", "NavmeshGeometry[11].GridCell[2]", "NavmeshGeometry[11].GridCell[3]", "NavmeshGeometry[11].GridCell[4]", "NavmeshGeometry[11].GridCell[5]", "NavmeshGeometry[11].GridCell[6]", "NavmeshGeometry[11].GridCell[7]", "NavmeshGeometry[2].Point", "NavmeshGeometry[3].Point", "NavmeshGeometry[4].Point", "NavmeshGeometry[5].Point", "NavmeshGeometry[6].Point", "NavmeshGeometry[7].EdgeLink_0_1", "NavmeshGeometry[7].EdgeLink_1_2", "NavmeshGeometry[7].EdgeLink_2_0", "NavmeshGeometry[7].Height", "NavmeshGeometry[7].Vertices", "NavmeshGeometry[8].EdgeLink_0_1", "NavmeshGeometry[8].EdgeLink_1_2", "NavmeshGeometry[8].Height", "NavmeshGeometry[8].Vertices", "NavmeshGeometry[9].EdgeLink_0_1", "NavmeshGeometry[9].EdgeLink_2_0", "NavmeshGeometry[9].Height", "NavmeshGeometry[9].Vertices", "ObjectBounds.First", "ObjectBounds.Second", "OpenSound.Start", "SoundLevel", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "ANAM", "BNAM", "CloseSound.Start", "CNAM", "EditorID", "FacingAxisOverride", "FormKey", "FormVersion", "Models[0].File", "Models[0].LightLayer", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "NavmeshGeometry.Count", "NavmeshGeometry.GridMax", "NavmeshGeometry.GridMaxDistance", "NavmeshGeometry.GridMin", "NavmeshGeometry.GridSize", "NavmeshGeometry.Parent.MutagenObjectType", "NavmeshGeometry.Parent.Parent", "NavmeshGeometry[0]", "NavmeshGeometry[1].Point", "NavmeshGeometry[10].EdgeLink_0_1", "NavmeshGeometry[10].EdgeLink_1_2", "NavmeshGeometry[10].EdgeLink_2_0", "NavmeshGeometry[10].Height", "NavmeshGeometry[10].Vertices", "NavmeshGeometry[11].GridCell.Count", "NavmeshGeometry[11].GridCell[0]", "NavmeshGeometry[11].GridCell[1]", "NavmeshGeometry[11].GridCell[2]", "NavmeshGeometry[11].GridCell[3]", "NavmeshGeometry[11].GridCell[4]", "NavmeshGeometry[11].GridCell[5]", "NavmeshGeometry[11].GridCell[6]", "NavmeshGeometry[11].GridCell[7]", "NavmeshGeometry[2].Point", "NavmeshGeometry[3].Point", "NavmeshGeometry[4].Point", "NavmeshGeometry[5].Point", "NavmeshGeometry[6].Point", "NavmeshGeometry[7].EdgeLink_0_1", "NavmeshGeometry[7].EdgeLink_1_2", "NavmeshGeometry[7].EdgeLink_2_0", "NavmeshGeometry[7].Height", "NavmeshGeometry[7].Vertices", "NavmeshGeometry[8].EdgeLink_0_1", "NavmeshGeometry[8].EdgeLink_1_2", "NavmeshGeometry[8].Height", "NavmeshGeometry[8].Vertices", "NavmeshGeometry[9].EdgeLink_0_1", "NavmeshGeometry[9].EdgeLink_2_0", "NavmeshGeometry[9].Height", "NavmeshGeometry[9].Vertices", "ObjectBoundsFirst", "ObjectBoundsSecond", "OpenSound.Start", "SoundLevel", "Version2", "VersionControl");
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "DOOR")]
    [Trait("FormKey", "19AFF6:Starfield.esm")]
    [Trait("EditorID", "SftIntRmSmWallMid_DoorA00")]
    [Trait("SpriggitFile", "Doors/SftIntRmSmWallMid_DoorA00 - 19AFF6_Starfield.esm.yaml")]
    public void Starfield_DOOR_ShouldMatchSpriggitSample_SftIntRmSmWallMid_DoorA00()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Door,
            "SftIntRmSmWallMid_DoorA00");
        var dto = Helpers.GetDTO<DoorDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Door,
            "19AFF6:Starfield.esm");

        Helpers.GetSpriggitField(spriggit, "ANAM").ShouldBe(Helpers.GetDTOField(dto, "ANAM"));
        Helpers.GetSpriggitField(spriggit, "BNAM").ShouldBe(Helpers.GetDTOField(dto, "BNAM"));
        Helpers.GetSpriggitField(spriggit, "CloseSound.Start").ShouldBe(Helpers.GetDTOField(dto, "CloseSound.Start"));
        Helpers.GetSpriggitField(spriggit, "CNAM").ShouldBe(Helpers.GetDTOField(dto, "CNAM"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FacingAxisOverride").ShouldBe(Helpers.GetDTOField(dto, "FacingAxisOverride"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "MajorRecordFlagsRaw").ShouldBe(Helpers.GetDTOField(dto, "MajorRecordFlags"));
        Helpers.GetSpriggitField(spriggit, "Model.Count").ShouldBe(Helpers.GetDTOField(dto, "Model.Count"));
        Helpers.GetSpriggitField(spriggit, "Model.File").ShouldBe(Helpers.GetDTOField(dto, "Models[0].File"));
        Helpers.GetSpriggitField(spriggit, "Model.LightLayer").ShouldBe(Helpers.GetDTOField(dto, "Models[0].LightLayer"));
        Helpers.GetSpriggitField(spriggit, "Model[0].081158").ShouldBe(Helpers.GetDTOField(dto, "Model[0].081158"));
        Helpers.GetSpriggitField(spriggit, "Model[1]").ShouldBe(Helpers.GetDTOField(dto, "Model[1]"));
        Helpers.GetSpriggitField(spriggit, "Name.Count").ShouldBe(Helpers.GetDTOField(dto, "Name.Count"));
        Helpers.GetSpriggitField(spriggit, "Name.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Name.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Name[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[0].String").ShouldBe(Helpers.GetDTOField(dto, "Name[0].String"));
        Helpers.GetSpriggitField(spriggit, "Name[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[1].String").ShouldBe(Helpers.GetDTOField(dto, "Name[1].String"));
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
        Helpers.GetSpriggitField(spriggit, "NativeTerminal").ShouldBe(Helpers.GetDTOField(dto, "NativeTerminalFormKey"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.First").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsFirst"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.Second").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsSecond"));
        Helpers.GetSpriggitField(spriggit, "OpenSound.Start").ShouldBe(Helpers.GetDTOField(dto, "OpenSound.Start"));
        Helpers.GetSpriggitField(spriggit, "SoundLevel").ShouldBe(Helpers.GetDTOField(dto, "SoundLevel"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "ANAM", "BNAM", "CloseSound.Start", "CNAM", "EditorID", "FacingAxisOverride", "FormKey", "FormVersion", "MajorRecordFlagsRaw", "Model.Count", "Model.File", "Model.LightLayer", "Model[0].081158", "Model[1]", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "NativeTerminal", "ObjectBounds.First", "ObjectBounds.Second", "OpenSound.Start", "SoundLevel", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "ANAM", "BNAM", "CloseSound.Start", "CNAM", "EditorID", "FacingAxisOverride", "FormKey", "FormVersion", "MajorRecordFlags", "Model.Count", "Models[0].File", "Models[0].LightLayer", "Model[0].081158", "Model[1]", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "NativeTerminalFormKey", "ObjectBoundsFirst", "ObjectBoundsSecond", "OpenSound.Start", "SoundLevel", "Version2", "VersionControl");
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "DOOR")]
    [Trait("FormKey", "30D813:Starfield.esm")]
    [Trait("EditorID", "SftIntRmSmWallMid_DoorA00_Loud")]
    [Trait("SpriggitFile", "Doors/SftIntRmSmWallMid_DoorA00_Loud - 30D813_Starfield.esm.yaml")]
    public void Starfield_DOOR_ShouldMatchSpriggitSample_SftIntRmSmWallMid_DoorA00_Loud()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Door,
            "SftIntRmSmWallMid_DoorA00_Loud");
        var dto = Helpers.GetDTO<DoorDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Door,
            "30D813:Starfield.esm");

        Helpers.GetSpriggitField(spriggit, "ANAM").ShouldBe(Helpers.GetDTOField(dto, "ANAM"));
        Helpers.GetSpriggitField(spriggit, "BNAM").ShouldBe(Helpers.GetDTOField(dto, "BNAM"));
        Helpers.GetSpriggitField(spriggit, "CloseSound.Start").ShouldBe(Helpers.GetDTOField(dto, "CloseSound.Start"));
        Helpers.GetSpriggitField(spriggit, "CNAM").ShouldBe(Helpers.GetDTOField(dto, "CNAM"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FacingAxisOverride").ShouldBe(Helpers.GetDTOField(dto, "FacingAxisOverride"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "MajorRecordFlagsRaw").ShouldBe(Helpers.GetDTOField(dto, "MajorRecordFlags"));
        Helpers.GetSpriggitField(spriggit, "Model.Count").ShouldBe(Helpers.GetDTOField(dto, "Model.Count"));
        Helpers.GetSpriggitField(spriggit, "Model.File").ShouldBe(Helpers.GetDTOField(dto, "Models[0].File"));
        Helpers.GetSpriggitField(spriggit, "Model.LightLayer").ShouldBe(Helpers.GetDTOField(dto, "Models[0].LightLayer"));
        Helpers.GetSpriggitField(spriggit, "Model[0].081158").ShouldBe(Helpers.GetDTOField(dto, "Model[0].081158"));
        Helpers.GetSpriggitField(spriggit, "Model[1]").ShouldBe(Helpers.GetDTOField(dto, "Model[1]"));
        Helpers.GetSpriggitField(spriggit, "Name.Count").ShouldBe(Helpers.GetDTOField(dto, "Name.Count"));
        Helpers.GetSpriggitField(spriggit, "Name.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Name.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Name[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[0].String").ShouldBe(Helpers.GetDTOField(dto, "Name[0].String"));
        Helpers.GetSpriggitField(spriggit, "Name[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[1].String").ShouldBe(Helpers.GetDTOField(dto, "Name[1].String"));
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
        Helpers.GetSpriggitField(spriggit, "NativeTerminal").ShouldBe(Helpers.GetDTOField(dto, "NativeTerminalFormKey"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.First").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsFirst"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.Second").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsSecond"));
        Helpers.GetSpriggitField(spriggit, "OpenSound.Start").ShouldBe(Helpers.GetDTOField(dto, "OpenSound.Start"));
        Helpers.GetSpriggitField(spriggit, "SoundLevel").ShouldBe(Helpers.GetDTOField(dto, "SoundLevel"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "ANAM", "BNAM", "CloseSound.Start", "CNAM", "EditorID", "FacingAxisOverride", "FormKey", "FormVersion", "MajorRecordFlagsRaw", "Model.Count", "Model.File", "Model.LightLayer", "Model[0].081158", "Model[1]", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "NativeTerminal", "ObjectBounds.First", "ObjectBounds.Second", "OpenSound.Start", "SoundLevel", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "ANAM", "BNAM", "CloseSound.Start", "CNAM", "EditorID", "FacingAxisOverride", "FormKey", "FormVersion", "MajorRecordFlags", "Model.Count", "Models[0].File", "Models[0].LightLayer", "Model[0].081158", "Model[1]", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "NativeTerminalFormKey", "ObjectBoundsFirst", "ObjectBoundsSecond", "OpenSound.Start", "SoundLevel", "Version2", "VersionControl");
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "DOOR")]
    [Trait("FormKey", "31D042:Starfield.esm")]
    [Trait("EditorID", "ShpGenIntPerSmWallMid_ExLg_DockingDoor02L_NonLoad")]
    [Trait("SpriggitFile", "Doors/ShpGenIntPerSmWallMid_ExLg_DockingDoor02L_NonLoad - 31D042_Starfield.esm.yaml")]
    public void Starfield_DOOR_ShouldMatchSpriggitSample_ShpGenIntPerSmWallMid_ExLg_DockingDoor02L_NonLoad()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Door,
            "ShpGenIntPerSmWallMid_ExLg_DockingDoor02L_NonLoad");
        var dto = Helpers.GetDTO<DoorDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Door,
            "31D042:Starfield.esm");

        Helpers.GetSpriggitField(spriggit, "ANAM").ShouldBe(Helpers.GetDTOField(dto, "ANAM"));
        Helpers.GetSpriggitField(spriggit, "BNAM").ShouldBe(Helpers.GetDTOField(dto, "BNAM"));
        Helpers.GetSpriggitField(spriggit, "CloseSound.Start").ShouldBe(Helpers.GetDTOField(dto, "CloseSound.Start"));
        Helpers.GetSpriggitField(spriggit, "CNAM").ShouldBe(Helpers.GetDTOField(dto, "CNAM"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FacingAxisOverride").ShouldBe(Helpers.GetDTOField(dto, "FacingAxisOverride"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "Model.Count").ShouldBe(Helpers.GetDTOField(dto, "Model.Count"));
        Helpers.GetSpriggitField(spriggit, "Model.File").ShouldBe(Helpers.GetDTOField(dto, "Models[0].File"));
        Helpers.GetSpriggitField(spriggit, "Model.LightLayer").ShouldBe(Helpers.GetDTOField(dto, "Models[0].LightLayer"));
        Helpers.GetSpriggitField(spriggit, "Model[0].212E3E").ShouldBe(Helpers.GetDTOField(dto, "Model[0].212E3E"));
        Helpers.GetSpriggitField(spriggit, "Model[1]").ShouldBe(Helpers.GetDTOField(dto, "Model[1]"));
        Helpers.GetSpriggitField(spriggit, "Name.Count").ShouldBe(Helpers.GetDTOField(dto, "Name.Count"));
        Helpers.GetSpriggitField(spriggit, "Name.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Name.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Name[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[0].String").ShouldBe(Helpers.GetDTOField(dto, "Name[0].String"));
        Helpers.GetSpriggitField(spriggit, "Name[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[1].String").ShouldBe(Helpers.GetDTOField(dto, "Name[1].String"));
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
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.First").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsFirst"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.Second").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsSecond"));
        Helpers.GetSpriggitField(spriggit, "OpenSound.Start").ShouldBe(Helpers.GetDTOField(dto, "OpenSound.Start"));
        Helpers.GetSpriggitField(spriggit, "REFL").ShouldBe(Helpers.GetDTOField(dto, "REFL"));
        Helpers.GetSpriggitField(spriggit, "SoundLevel").ShouldBe(Helpers.GetDTOField(dto, "SoundLevel"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter.Count").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter.Count"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0].Count").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0].Count"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0].Count").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0].Count"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0][0].Members.Count").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0][0].Members.Count"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0][0].Members[0].Data").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0][0].Members[0].Data"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0][0].Members[0].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0][0].Members[0].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0][0].Members[0].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0][0].Members[0].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0][0].Members[1].Data").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0][0].Members[1].Data"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0][0].Members[1].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0][0].Members[1].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0][0].Members[1].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0][0].Members[1].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0][0].Members[2].Data").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0][0].Members[2].Data"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0][0].Members[2].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0][0].Members[2].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0][0].Members[2].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0][0].Members[2].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0][0].Members[3].Data").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0][0].Members[3].Data"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0][0].Members[3].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0][0].Members[3].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0][0].Members[3].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0][0].Members[3].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0][0].Members[4].Data").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0][0].Members[4].Data"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0][0].Members[4].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0][0].Members[4].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0][0].Members[4].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0][0].Members[4].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0][1].Members.Count").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0][1].Members.Count"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0][1].Members[0].Data").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0][1].Members[0].Data"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0][1].Members[0].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0][1].Members[0].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0][1].Members[0].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0][1].Members[0].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0][1].Members[1].Data").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0][1].Members[1].Data"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0][1].Members[1].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0][1].Members[1].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0][1].Members[1].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0][1].Members[1].Name"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "ANAM", "BNAM", "CloseSound.Start", "CNAM", "EditorID", "FacingAxisOverride", "FormKey", "FormVersion", "Model.Count", "Model.File", "Model.LightLayer", "Model[0].212E3E", "Model[1]", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "ObjectBounds.First", "ObjectBounds.Second", "OpenSound.Start", "REFL", "SoundLevel", "Version2", "VersionControl", "VirtualMachineAdapter.Count", "VirtualMachineAdapter[0].Count", "VirtualMachineAdapter[0].Name", "VirtualMachineAdapter[0][0].Count", "VirtualMachineAdapter[0][0].MutagenObjectType", "VirtualMachineAdapter[0][0].Name", "VirtualMachineAdapter[0][0][0].Members.Count", "VirtualMachineAdapter[0][0][0].Members[0].Data", "VirtualMachineAdapter[0][0][0].Members[0].MutagenObjectType", "VirtualMachineAdapter[0][0][0].Members[0].Name", "VirtualMachineAdapter[0][0][0].Members[1].Data", "VirtualMachineAdapter[0][0][0].Members[1].MutagenObjectType", "VirtualMachineAdapter[0][0][0].Members[1].Name", "VirtualMachineAdapter[0][0][0].Members[2].Data", "VirtualMachineAdapter[0][0][0].Members[2].MutagenObjectType", "VirtualMachineAdapter[0][0][0].Members[2].Name", "VirtualMachineAdapter[0][0][0].Members[3].Data", "VirtualMachineAdapter[0][0][0].Members[3].MutagenObjectType", "VirtualMachineAdapter[0][0][0].Members[3].Name", "VirtualMachineAdapter[0][0][0].Members[4].Data", "VirtualMachineAdapter[0][0][0].Members[4].MutagenObjectType", "VirtualMachineAdapter[0][0][0].Members[4].Name", "VirtualMachineAdapter[0][0][1].Members.Count", "VirtualMachineAdapter[0][0][1].Members[0].Data", "VirtualMachineAdapter[0][0][1].Members[0].MutagenObjectType", "VirtualMachineAdapter[0][0][1].Members[0].Name", "VirtualMachineAdapter[0][0][1].Members[1].Data", "VirtualMachineAdapter[0][0][1].Members[1].MutagenObjectType", "VirtualMachineAdapter[0][0][1].Members[1].Name");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "ANAM", "BNAM", "CloseSound.Start", "CNAM", "EditorID", "FacingAxisOverride", "FormKey", "FormVersion", "Model.Count", "Models[0].File", "Models[0].LightLayer", "Model[0].212E3E", "Model[1]", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "ObjectBoundsFirst", "ObjectBoundsSecond", "OpenSound.Start", "REFL", "SoundLevel", "Version2", "VersionControl", "VirtualMachineAdapter.Count", "VirtualMachineAdapter[0].Count", "VirtualMachineAdapter[0].Name", "VirtualMachineAdapter[0][0].Count", "VirtualMachineAdapter[0][0].MutagenObjectType", "VirtualMachineAdapter[0][0].Name", "VirtualMachineAdapter[0][0][0].Members.Count", "VirtualMachineAdapter[0][0][0].Members[0].Data", "VirtualMachineAdapter[0][0][0].Members[0].MutagenObjectType", "VirtualMachineAdapter[0][0][0].Members[0].Name", "VirtualMachineAdapter[0][0][0].Members[1].Data", "VirtualMachineAdapter[0][0][0].Members[1].MutagenObjectType", "VirtualMachineAdapter[0][0][0].Members[1].Name", "VirtualMachineAdapter[0][0][0].Members[2].Data", "VirtualMachineAdapter[0][0][0].Members[2].MutagenObjectType", "VirtualMachineAdapter[0][0][0].Members[2].Name", "VirtualMachineAdapter[0][0][0].Members[3].Data", "VirtualMachineAdapter[0][0][0].Members[3].MutagenObjectType", "VirtualMachineAdapter[0][0][0].Members[3].Name", "VirtualMachineAdapter[0][0][0].Members[4].Data", "VirtualMachineAdapter[0][0][0].Members[4].MutagenObjectType", "VirtualMachineAdapter[0][0][0].Members[4].Name", "VirtualMachineAdapter[0][0][1].Members.Count", "VirtualMachineAdapter[0][0][1].Members[0].Data", "VirtualMachineAdapter[0][0][1].Members[0].MutagenObjectType", "VirtualMachineAdapter[0][0][1].Members[0].Name", "VirtualMachineAdapter[0][0][1].Members[1].Data", "VirtualMachineAdapter[0][0][1].Members[1].MutagenObjectType", "VirtualMachineAdapter[0][0][1].Members[1].Name");
    }
}