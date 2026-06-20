using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.Door.Fallout4;

public class Fallout4DoorSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "DOOR")]
    [Trait("FormKey", "01ED77:Fallout4.esm")]
    [Trait("EditorID", "AutoloadDoor")]
    [Trait("SpriggitFile", "Doors/AutoloadDoor - 01ED77_Fallout4.esm.yaml")]
    public void Fallout4_DOOR_ShouldMatchSpriggitSample_AutoloadDoor()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Door,
            "AutoloadDoor");
        var dto = Helpers.GetDTO<DoorDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Door,
            "01ED77:Fallout4.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["Flags.Count"].ShouldBe(dtoFields["Flags.Count"]);
        spriggitFields["Flags[0]"].ShouldBe(dtoFields["Flags[0]"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["Model.Data"].ShouldBe(dtoFields["Model.Data"]);
        NormalizeModelFile(spriggitFields["Model.File"]).ShouldBe(NormalizeModelFile(dtoFields["Models[0].File"]));
        spriggitFields["Name[2].Language"].ShouldBe(dtoFields["Name[2].Language"]);
        spriggitFields["Name[2].String"].ShouldBe(dtoFields["Name[2].String"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "DOOR")]
    [Trait("FormKey", "01D930:Fallout4.esm")]
    [Trait("EditorID", "BldWoodPDbDoor01")]
    [Trait("SpriggitFile", "Doors/BldWoodPDbDoor01 - 01D930_Fallout4.esm.yaml")]
    public void Fallout4_DOOR_ShouldMatchSpriggitSample_BldWoodPDbDoor01()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Door,
            "BldWoodPDbDoor01");
        var dto = Helpers.GetDTO<DoorDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Door,
            "01D930:Fallout4.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["CloseSound"].ShouldBe(dtoFields["CloseSound.Start"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["MajorRecordFlagsRaw"].ShouldBe(dtoFields["MajorRecordFlags"]);
        spriggitFields["Model.Data"].ShouldBe(dtoFields["Model.Data"]);
        NormalizeModelFile(spriggitFields["Model.File"]).ShouldBe(NormalizeModelFile(dtoFields["Models[0].File"]));
        spriggitFields["Name[2].Language"].ShouldBe(dtoFields["Name[2].Language"]);
        spriggitFields["Name[2].String"].ShouldBe(dtoFields["Name[2].String"]);
        spriggitFields["NativeTerminal"].ShouldBe(dtoFields["NativeTerminalFormKey"]);
        spriggitFields["ObjectBounds.First"].ShouldBe(dtoFields["ObjectBoundsFirst"]);
        spriggitFields["ObjectBounds.Second"].ShouldBe(dtoFields["ObjectBoundsSecond"]);
        spriggitFields["OpenSound"].ShouldBe(dtoFields["OpenSound.Start"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

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
