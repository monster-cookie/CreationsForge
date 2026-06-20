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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["ANAM"].ShouldBe(dtoFields["ANAM"]);
        spriggitFields["BNAM"].ShouldBe(dtoFields["BNAM"]);
        spriggitFields["CloseSound.Start"].ShouldBe(dtoFields["CloseSound.Start"]);
        spriggitFields["CNAM"].ShouldBe(dtoFields["CNAM"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FacingAxisOverride"].ShouldBe(dtoFields["FacingAxisOverride"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["Model.File"].ShouldBe(dtoFields["Models[0].File"]);
        spriggitFields["Model.LightLayer"].ShouldBe(dtoFields["Models[0].LightLayer"]);
        spriggitFields["Name.Count"].ShouldBe(dtoFields["Name.Count"]);
        spriggitFields["Name.TargetLanguage"].ShouldBe(dtoFields["Name.TargetLanguage"]);
        spriggitFields["Name[0].Language"].ShouldBe(dtoFields["Name[0].Language"]);
        spriggitFields["Name[0].String"].ShouldBe(dtoFields["Name[0].String"]);
        spriggitFields["Name[1].Language"].ShouldBe(dtoFields["Name[1].Language"]);
        spriggitFields["Name[1].String"].ShouldBe(dtoFields["Name[1].String"]);
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
        spriggitFields["NativeTerminal"].ShouldBe(dtoFields["NativeTerminalFormKey"]);
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
        spriggitFields["NavmeshGeometry[10].Height"].ShouldBe(dtoFields["NavmeshGeometry[10].Height"]);
        spriggitFields["NavmeshGeometry[10].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[10].Vertices"]);
        spriggitFields["NavmeshGeometry[11].GridCell.Count"].ShouldBe(dtoFields["NavmeshGeometry[11].GridCell.Count"]);
        spriggitFields["NavmeshGeometry[11].GridCell[0]"].ShouldBe(dtoFields["NavmeshGeometry[11].GridCell[0]"]);
        spriggitFields["NavmeshGeometry[11].GridCell[1]"].ShouldBe(dtoFields["NavmeshGeometry[11].GridCell[1]"]);
        spriggitFields["NavmeshGeometry[11].GridCell[2]"].ShouldBe(dtoFields["NavmeshGeometry[11].GridCell[2]"]);
        spriggitFields["NavmeshGeometry[11].GridCell[3]"].ShouldBe(dtoFields["NavmeshGeometry[11].GridCell[3]"]);
        spriggitFields["NavmeshGeometry[11].GridCell[4]"].ShouldBe(dtoFields["NavmeshGeometry[11].GridCell[4]"]);
        spriggitFields["NavmeshGeometry[11].GridCell[5]"].ShouldBe(dtoFields["NavmeshGeometry[11].GridCell[5]"]);
        spriggitFields["NavmeshGeometry[11].GridCell[6]"].ShouldBe(dtoFields["NavmeshGeometry[11].GridCell[6]"]);
        spriggitFields["NavmeshGeometry[11].GridCell[7]"].ShouldBe(dtoFields["NavmeshGeometry[11].GridCell[7]"]);
        spriggitFields["NavmeshGeometry[2].Point"].ShouldBe(dtoFields["NavmeshGeometry[2].Point"]);
        spriggitFields["NavmeshGeometry[3].Point"].ShouldBe(dtoFields["NavmeshGeometry[3].Point"]);
        spriggitFields["NavmeshGeometry[4].Point"].ShouldBe(dtoFields["NavmeshGeometry[4].Point"]);
        spriggitFields["NavmeshGeometry[5].Point"].ShouldBe(dtoFields["NavmeshGeometry[5].Point"]);
        spriggitFields["NavmeshGeometry[6].Point"].ShouldBe(dtoFields["NavmeshGeometry[6].Point"]);
        spriggitFields["NavmeshGeometry[7].EdgeLink_0_1"].ShouldBe(dtoFields["NavmeshGeometry[7].EdgeLink_0_1"]);
        spriggitFields["NavmeshGeometry[7].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[7].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[7].EdgeLink_2_0"].ShouldBe(dtoFields["NavmeshGeometry[7].EdgeLink_2_0"]);
        spriggitFields["NavmeshGeometry[7].Height"].ShouldBe(dtoFields["NavmeshGeometry[7].Height"]);
        spriggitFields["NavmeshGeometry[7].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[7].Vertices"]);
        spriggitFields["NavmeshGeometry[8].EdgeLink_0_1"].ShouldBe(dtoFields["NavmeshGeometry[8].EdgeLink_0_1"]);
        spriggitFields["NavmeshGeometry[8].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[8].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[8].EdgeLink_2_0"].ShouldBe(dtoFields["NavmeshGeometry[8].EdgeLink_2_0"]);
        spriggitFields["NavmeshGeometry[8].Height"].ShouldBe(dtoFields["NavmeshGeometry[8].Height"]);
        spriggitFields["NavmeshGeometry[8].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[8].Vertices"]);
        spriggitFields["NavmeshGeometry[9].EdgeLink_0_1"].ShouldBe(dtoFields["NavmeshGeometry[9].EdgeLink_0_1"]);
        spriggitFields["NavmeshGeometry[9].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[9].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[9].Height"].ShouldBe(dtoFields["NavmeshGeometry[9].Height"]);
        spriggitFields["NavmeshGeometry[9].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[9].Vertices"]);
        spriggitFields["ObjectBounds.First"].ShouldBe(dtoFields["ObjectBoundsFirst"]);
        spriggitFields["ObjectBounds.Second"].ShouldBe(dtoFields["ObjectBoundsSecond"]);
        spriggitFields["OpenSound.Start"].ShouldBe(dtoFields["OpenSound.Start"]);
        spriggitFields["SoundLevel"].ShouldBe(dtoFields["SoundLevel"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["ANAM"].ShouldBe(dtoFields["ANAM"]);
        spriggitFields["BNAM"].ShouldBe(dtoFields["BNAM"]);
        spriggitFields["CloseSound.Start"].ShouldBe(dtoFields["CloseSound.Start"]);
        spriggitFields["CNAM"].ShouldBe(dtoFields["CNAM"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FacingAxisOverride"].ShouldBe(dtoFields["FacingAxisOverride"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["Model.File"].ShouldBe(dtoFields["Models[0].File"]);
        spriggitFields["Model.LightLayer"].ShouldBe(dtoFields["Models[0].LightLayer"]);
        spriggitFields["Name.Count"].ShouldBe(dtoFields["Name.Count"]);
        spriggitFields["Name.TargetLanguage"].ShouldBe(dtoFields["Name.TargetLanguage"]);
        spriggitFields["Name[0].Language"].ShouldBe(dtoFields["Name[0].Language"]);
        spriggitFields["Name[0].String"].ShouldBe(dtoFields["Name[0].String"]);
        spriggitFields["Name[1].Language"].ShouldBe(dtoFields["Name[1].Language"]);
        spriggitFields["Name[1].String"].ShouldBe(dtoFields["Name[1].String"]);
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
        spriggitFields["NavmeshGeometry[11].GridCell.Count"].ShouldBe(dtoFields["NavmeshGeometry[11].GridCell.Count"]);
        spriggitFields["NavmeshGeometry[11].GridCell[0]"].ShouldBe(dtoFields["NavmeshGeometry[11].GridCell[0]"]);
        spriggitFields["NavmeshGeometry[11].GridCell[1]"].ShouldBe(dtoFields["NavmeshGeometry[11].GridCell[1]"]);
        spriggitFields["NavmeshGeometry[11].GridCell[2]"].ShouldBe(dtoFields["NavmeshGeometry[11].GridCell[2]"]);
        spriggitFields["NavmeshGeometry[11].GridCell[3]"].ShouldBe(dtoFields["NavmeshGeometry[11].GridCell[3]"]);
        spriggitFields["NavmeshGeometry[11].GridCell[4]"].ShouldBe(dtoFields["NavmeshGeometry[11].GridCell[4]"]);
        spriggitFields["NavmeshGeometry[11].GridCell[5]"].ShouldBe(dtoFields["NavmeshGeometry[11].GridCell[5]"]);
        spriggitFields["NavmeshGeometry[11].GridCell[6]"].ShouldBe(dtoFields["NavmeshGeometry[11].GridCell[6]"]);
        spriggitFields["NavmeshGeometry[11].GridCell[7]"].ShouldBe(dtoFields["NavmeshGeometry[11].GridCell[7]"]);
        spriggitFields["NavmeshGeometry[2].Point"].ShouldBe(dtoFields["NavmeshGeometry[2].Point"]);
        spriggitFields["NavmeshGeometry[3].Point"].ShouldBe(dtoFields["NavmeshGeometry[3].Point"]);
        spriggitFields["NavmeshGeometry[4].Point"].ShouldBe(dtoFields["NavmeshGeometry[4].Point"]);
        spriggitFields["NavmeshGeometry[5].Point"].ShouldBe(dtoFields["NavmeshGeometry[5].Point"]);
        spriggitFields["NavmeshGeometry[6].Point"].ShouldBe(dtoFields["NavmeshGeometry[6].Point"]);
        spriggitFields["NavmeshGeometry[7].EdgeLink_0_1"].ShouldBe(dtoFields["NavmeshGeometry[7].EdgeLink_0_1"]);
        spriggitFields["NavmeshGeometry[7].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[7].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[7].EdgeLink_2_0"].ShouldBe(dtoFields["NavmeshGeometry[7].EdgeLink_2_0"]);
        spriggitFields["NavmeshGeometry[7].Height"].ShouldBe(dtoFields["NavmeshGeometry[7].Height"]);
        spriggitFields["NavmeshGeometry[7].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[7].Vertices"]);
        spriggitFields["NavmeshGeometry[8].EdgeLink_0_1"].ShouldBe(dtoFields["NavmeshGeometry[8].EdgeLink_0_1"]);
        spriggitFields["NavmeshGeometry[8].EdgeLink_1_2"].ShouldBe(dtoFields["NavmeshGeometry[8].EdgeLink_1_2"]);
        spriggitFields["NavmeshGeometry[8].Height"].ShouldBe(dtoFields["NavmeshGeometry[8].Height"]);
        spriggitFields["NavmeshGeometry[8].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[8].Vertices"]);
        spriggitFields["NavmeshGeometry[9].EdgeLink_0_1"].ShouldBe(dtoFields["NavmeshGeometry[9].EdgeLink_0_1"]);
        spriggitFields["NavmeshGeometry[9].EdgeLink_2_0"].ShouldBe(dtoFields["NavmeshGeometry[9].EdgeLink_2_0"]);
        spriggitFields["NavmeshGeometry[9].Height"].ShouldBe(dtoFields["NavmeshGeometry[9].Height"]);
        spriggitFields["NavmeshGeometry[9].Vertices"].ShouldBe(dtoFields["NavmeshGeometry[9].Vertices"]);
        spriggitFields["ObjectBounds.First"].ShouldBe(dtoFields["ObjectBoundsFirst"]);
        spriggitFields["ObjectBounds.Second"].ShouldBe(dtoFields["ObjectBoundsSecond"]);
        spriggitFields["OpenSound.Start"].ShouldBe(dtoFields["OpenSound.Start"]);
        spriggitFields["SoundLevel"].ShouldBe(dtoFields["SoundLevel"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["ANAM"].ShouldBe(dtoFields["ANAM"]);
        spriggitFields["BNAM"].ShouldBe(dtoFields["BNAM"]);
        spriggitFields["CloseSound.Start"].ShouldBe(dtoFields["CloseSound.Start"]);
        spriggitFields["CNAM"].ShouldBe(dtoFields["CNAM"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FacingAxisOverride"].ShouldBe(dtoFields["FacingAxisOverride"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["MajorRecordFlagsRaw"].ShouldBe(dtoFields["MajorRecordFlags"]);
        spriggitFields["Model.Count"].ShouldBe(dtoFields["Model.Count"]);
        spriggitFields["Model.File"].ShouldBe(dtoFields["Models[0].File"]);
        spriggitFields["Model.LightLayer"].ShouldBe(dtoFields["Models[0].LightLayer"]);
        spriggitFields["Model[0].081158"].ShouldBe(dtoFields["Model[0].081158"]);
        spriggitFields["Model[1]"].ShouldBe(dtoFields["Model[1]"]);
        spriggitFields["Name.Count"].ShouldBe(dtoFields["Name.Count"]);
        spriggitFields["Name.TargetLanguage"].ShouldBe(dtoFields["Name.TargetLanguage"]);
        spriggitFields["Name[0].Language"].ShouldBe(dtoFields["Name[0].Language"]);
        spriggitFields["Name[0].String"].ShouldBe(dtoFields["Name[0].String"]);
        spriggitFields["Name[1].Language"].ShouldBe(dtoFields["Name[1].Language"]);
        spriggitFields["Name[1].String"].ShouldBe(dtoFields["Name[1].String"]);
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
        spriggitFields["NativeTerminal"].ShouldBe(dtoFields["NativeTerminalFormKey"]);
        spriggitFields["ObjectBounds.First"].ShouldBe(dtoFields["ObjectBoundsFirst"]);
        spriggitFields["ObjectBounds.Second"].ShouldBe(dtoFields["ObjectBoundsSecond"]);
        spriggitFields["OpenSound.Start"].ShouldBe(dtoFields["OpenSound.Start"]);
        spriggitFields["SoundLevel"].ShouldBe(dtoFields["SoundLevel"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["ANAM"].ShouldBe(dtoFields["ANAM"]);
        spriggitFields["BNAM"].ShouldBe(dtoFields["BNAM"]);
        spriggitFields["CloseSound.Start"].ShouldBe(dtoFields["CloseSound.Start"]);
        spriggitFields["CNAM"].ShouldBe(dtoFields["CNAM"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FacingAxisOverride"].ShouldBe(dtoFields["FacingAxisOverride"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["MajorRecordFlagsRaw"].ShouldBe(dtoFields["MajorRecordFlags"]);
        spriggitFields["Model.Count"].ShouldBe(dtoFields["Model.Count"]);
        spriggitFields["Model.File"].ShouldBe(dtoFields["Models[0].File"]);
        spriggitFields["Model.LightLayer"].ShouldBe(dtoFields["Models[0].LightLayer"]);
        spriggitFields["Model[0].081158"].ShouldBe(dtoFields["Model[0].081158"]);
        spriggitFields["Model[1]"].ShouldBe(dtoFields["Model[1]"]);
        spriggitFields["Name.Count"].ShouldBe(dtoFields["Name.Count"]);
        spriggitFields["Name.TargetLanguage"].ShouldBe(dtoFields["Name.TargetLanguage"]);
        spriggitFields["Name[0].Language"].ShouldBe(dtoFields["Name[0].Language"]);
        spriggitFields["Name[0].String"].ShouldBe(dtoFields["Name[0].String"]);
        spriggitFields["Name[1].Language"].ShouldBe(dtoFields["Name[1].Language"]);
        spriggitFields["Name[1].String"].ShouldBe(dtoFields["Name[1].String"]);
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
        spriggitFields["NativeTerminal"].ShouldBe(dtoFields["NativeTerminalFormKey"]);
        spriggitFields["ObjectBounds.First"].ShouldBe(dtoFields["ObjectBoundsFirst"]);
        spriggitFields["ObjectBounds.Second"].ShouldBe(dtoFields["ObjectBoundsSecond"]);
        spriggitFields["OpenSound.Start"].ShouldBe(dtoFields["OpenSound.Start"]);
        spriggitFields["SoundLevel"].ShouldBe(dtoFields["SoundLevel"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["ANAM"].ShouldBe(dtoFields["ANAM"]);
        spriggitFields["BNAM"].ShouldBe(dtoFields["BNAM"]);
        spriggitFields["CloseSound.Start"].ShouldBe(dtoFields["CloseSound.Start"]);
        spriggitFields["CNAM"].ShouldBe(dtoFields["CNAM"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FacingAxisOverride"].ShouldBe(dtoFields["FacingAxisOverride"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["Model.Count"].ShouldBe(dtoFields["Model.Count"]);
        spriggitFields["Model.File"].ShouldBe(dtoFields["Models[0].File"]);
        spriggitFields["Model.LightLayer"].ShouldBe(dtoFields["Models[0].LightLayer"]);
        spriggitFields["Model[0].212E3E"].ShouldBe(dtoFields["Model[0].212E3E"]);
        spriggitFields["Model[1]"].ShouldBe(dtoFields["Model[1]"]);
        spriggitFields["Name.Count"].ShouldBe(dtoFields["Name.Count"]);
        spriggitFields["Name.TargetLanguage"].ShouldBe(dtoFields["Name.TargetLanguage"]);
        spriggitFields["Name[0].Language"].ShouldBe(dtoFields["Name[0].Language"]);
        spriggitFields["Name[0].String"].ShouldBe(dtoFields["Name[0].String"]);
        spriggitFields["Name[1].Language"].ShouldBe(dtoFields["Name[1].Language"]);
        spriggitFields["Name[1].String"].ShouldBe(dtoFields["Name[1].String"]);
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
        spriggitFields["ObjectBounds.First"].ShouldBe(dtoFields["ObjectBoundsFirst"]);
        spriggitFields["ObjectBounds.Second"].ShouldBe(dtoFields["ObjectBoundsSecond"]);
        spriggitFields["OpenSound.Start"].ShouldBe(dtoFields["OpenSound.Start"]);
        spriggitFields["REFL"].ShouldBe(dtoFields["REFL"]);
        spriggitFields["SoundLevel"].ShouldBe(dtoFields["SoundLevel"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["VirtualMachineAdapter.Count"].ShouldBe(dtoFields["VirtualMachineAdapter.Count"]);
        spriggitFields["VirtualMachineAdapter[0].Count"].ShouldBe(dtoFields["VirtualMachineAdapter[0].Count"]);
        spriggitFields["VirtualMachineAdapter[0].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0].Name"]);
        spriggitFields["VirtualMachineAdapter[0][0].Count"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0].Count"]);
        spriggitFields["VirtualMachineAdapter[0][0].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][0].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0].Name"]);
        spriggitFields["VirtualMachineAdapter[0][0][0].Members.Count"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0][0].Members.Count"]);
        spriggitFields["VirtualMachineAdapter[0][0][0].Members[0].Data"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0][0].Members[0].Data"]);
        spriggitFields["VirtualMachineAdapter[0][0][0].Members[0].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0][0].Members[0].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][0][0].Members[0].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0][0].Members[0].Name"]);
        spriggitFields["VirtualMachineAdapter[0][0][0].Members[1].Data"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0][0].Members[1].Data"]);
        spriggitFields["VirtualMachineAdapter[0][0][0].Members[1].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0][0].Members[1].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][0][0].Members[1].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0][0].Members[1].Name"]);
        spriggitFields["VirtualMachineAdapter[0][0][0].Members[2].Data"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0][0].Members[2].Data"]);
        spriggitFields["VirtualMachineAdapter[0][0][0].Members[2].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0][0].Members[2].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][0][0].Members[2].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0][0].Members[2].Name"]);
        spriggitFields["VirtualMachineAdapter[0][0][0].Members[3].Data"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0][0].Members[3].Data"]);
        spriggitFields["VirtualMachineAdapter[0][0][0].Members[3].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0][0].Members[3].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][0][0].Members[3].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0][0].Members[3].Name"]);
        spriggitFields["VirtualMachineAdapter[0][0][0].Members[4].Data"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0][0].Members[4].Data"]);
        spriggitFields["VirtualMachineAdapter[0][0][0].Members[4].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0][0].Members[4].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][0][0].Members[4].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0][0].Members[4].Name"]);
        spriggitFields["VirtualMachineAdapter[0][0][1].Members.Count"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0][1].Members.Count"]);
        spriggitFields["VirtualMachineAdapter[0][0][1].Members[0].Data"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0][1].Members[0].Data"]);
        spriggitFields["VirtualMachineAdapter[0][0][1].Members[0].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0][1].Members[0].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][0][1].Members[0].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0][1].Members[0].Name"]);
        spriggitFields["VirtualMachineAdapter[0][0][1].Members[1].Data"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0][1].Members[1].Data"]);
        spriggitFields["VirtualMachineAdapter[0][0][1].Members[1].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0][1].Members[1].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][0][1].Members[1].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0][1].Members[1].Name"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }
}
