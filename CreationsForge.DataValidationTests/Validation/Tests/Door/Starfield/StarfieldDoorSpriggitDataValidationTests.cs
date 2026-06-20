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

        spriggitFields["CloseSound.Start"].ShouldBe(dtoFields["CloseSound.Start"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FacingAxisOverride"].ShouldBe(dtoFields["FacingAxisOverride"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        NormalizeModelFile(spriggitFields["Model.File"]).ShouldBe(NormalizeModelFile(dtoFields["Models[0].File"]));
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

        spriggitFields["CloseSound.Start"].ShouldBe(dtoFields["CloseSound.Start"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FacingAxisOverride"].ShouldBe(dtoFields["FacingAxisOverride"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        NormalizeModelFile(spriggitFields["Model.File"]).ShouldBe(NormalizeModelFile(dtoFields["Models[0].File"]));
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

        spriggitFields["CloseSound.Start"].ShouldBe(dtoFields["CloseSound.Start"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FacingAxisOverride"].ShouldBe(dtoFields["FacingAxisOverride"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["MajorRecordFlagsRaw"].ShouldBe(dtoFields["MajorRecordFlags"]);
        NormalizeModelFile(spriggitFields["Model.File"]).ShouldBe(NormalizeModelFile(dtoFields["Models[0].File"]));
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

        spriggitFields["CloseSound.Start"].ShouldBe(dtoFields["CloseSound.Start"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FacingAxisOverride"].ShouldBe(dtoFields["FacingAxisOverride"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["MajorRecordFlagsRaw"].ShouldBe(dtoFields["MajorRecordFlags"]);
        NormalizeModelFile(spriggitFields["Model.File"]).ShouldBe(NormalizeModelFile(dtoFields["Models[0].File"]));
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

        spriggitFields["CloseSound.Start"].ShouldBe(dtoFields["CloseSound.Start"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FacingAxisOverride"].ShouldBe(dtoFields["FacingAxisOverride"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        NormalizeModelFile(spriggitFields["Model.File"]).ShouldBe(NormalizeModelFile(dtoFields["Models[0].File"]));
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
        spriggitFields["VirtualMachineAdapter[0][0].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][0].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0].Name"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    private static string NormalizeModelFile(string modelFile)
    {
        return modelFile.StartsWith("Meshes\\", StringComparison.OrdinalIgnoreCase)
            ? modelFile
            : "Meshes\\" + modelFile;
    }
}

