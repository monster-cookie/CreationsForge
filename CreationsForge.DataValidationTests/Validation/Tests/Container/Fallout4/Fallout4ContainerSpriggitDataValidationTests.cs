using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.Container.Fallout4;

public class Fallout4ContainerSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "CONT")]
    [Trait("FormKey", "1F2B6A:Fallout4.esm")]
    [Trait("EditorID", "DN054Loot_Prewar_Safe")]
    [Trait("SpriggitFile", "Containers/DN054Loot_Prewar_Safe - 1F2B6A_Fallout4.esm.yaml")]
    public void Fallout4_CONT_ShouldMatchSpriggitSample_DN054Loot_Prewar_Safe()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Container,
            "DN054Loot_Prewar_Safe");
        var dto = Helpers.GetDTO<ContainerDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Container,
            "1F2B6A:Fallout4.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["CloseSound"].ShouldBe(dtoFields["CloseSound"]);
        spriggitFields["Count[0]"].ShouldBe(dtoFields["Count[0]"]);
        spriggitFields["Count[1]"].ShouldBe(dtoFields["Count[1]"]);
        spriggitFields["Count[2]"].ShouldBe(dtoFields["Count[2]"]);
        spriggitFields["Count[3]"].ShouldBe(dtoFields["Count[3]"]);
        spriggitFields["Count[4]"].ShouldBe(dtoFields["Count[4]"]);
        spriggitFields["Count[5]"].ShouldBe(dtoFields["Count[5]"]);
        spriggitFields["Count[6]"].ShouldBe(dtoFields["Count[6]"]);
        spriggitFields["Count[7]"].ShouldBe(dtoFields["Count[7]"]);
        spriggitFields["Count[8]"].ShouldBe(dtoFields["Count[8]"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["Item[0]"].ShouldBe(dtoFields["Item[0]"]);
        spriggitFields["Item[1]"].ShouldBe(dtoFields["Item[1]"]);
        spriggitFields["Item[2]"].ShouldBe(dtoFields["Item[2]"]);
        spriggitFields["Item[3]"].ShouldBe(dtoFields["Item[3]"]);
        spriggitFields["Item[4]"].ShouldBe(dtoFields["Item[4]"]);
        spriggitFields["Item[5]"].ShouldBe(dtoFields["Item[5]"]);
        spriggitFields["Item[6]"].ShouldBe(dtoFields["Item[6]"]);
        spriggitFields["Item[7]"].ShouldBe(dtoFields["Item[7]"]);
        spriggitFields["Item[8]"].ShouldBe(dtoFields["Item[8]"]);
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
        spriggitFields["NativeTerminal"].ShouldBe(dtoFields["NativeTerminalFormKey"]);
        spriggitFields["ObjectBounds.First"].ShouldBe(dtoFields["ObjectBoundsFirst"]);
        spriggitFields["ObjectBounds.Second"].ShouldBe(dtoFields["ObjectBoundsSecond"]);
        spriggitFields["OpenSound"].ShouldBe(dtoFields["OpenSound"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "CONT")]
    [Trait("FormKey", "064A36:Fallout4.esm")]
    [Trait("EditorID", "Loot_Raider_Safe")]
    [Trait("SpriggitFile", "Containers/Loot_Raider_Safe - 064A36_Fallout4.esm.yaml")]
    public void Fallout4_CONT_ShouldMatchSpriggitSample_Loot_Raider_Safe()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Container,
            "Loot_Raider_Safe");
        var dto = Helpers.GetDTO<ContainerDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Container,
            "064A36:Fallout4.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["CloseSound"].ShouldBe(dtoFields["CloseSound"]);
        spriggitFields["Count[0]"].ShouldBe(dtoFields["Count[0]"]);
        spriggitFields["Count[1]"].ShouldBe(dtoFields["Count[1]"]);
        spriggitFields["Count[2]"].ShouldBe(dtoFields["Count[2]"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["Item[0]"].ShouldBe(dtoFields["Item[0]"]);
        spriggitFields["Item[1]"].ShouldBe(dtoFields["Item[1]"]);
        spriggitFields["Item[2]"].ShouldBe(dtoFields["Item[2]"]);
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
        spriggitFields["NativeTerminal"].ShouldBe(dtoFields["NativeTerminalFormKey"]);
        spriggitFields["ObjectBounds.First"].ShouldBe(dtoFields["ObjectBoundsFirst"]);
        spriggitFields["ObjectBounds.Second"].ShouldBe(dtoFields["ObjectBoundsSecond"]);
        spriggitFields["OpenSound"].ShouldBe(dtoFields["OpenSound"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["VirtualMachineAdapter.Count"].ShouldBe(dtoFields["VirtualMachineAdapter.Count"]);
        spriggitFields["VirtualMachineAdapter[0].Count"].ShouldBe(dtoFields["VirtualMachineAdapter[0].Count"]);
        spriggitFields["VirtualMachineAdapter[0].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0].Name"]);
        spriggitFields["VirtualMachineAdapter[0][0].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][0].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0].Name"]);
        spriggitFields["VirtualMachineAdapter[0][0].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0].Object"]);
        spriggitFields["VirtualMachineAdapter[0][1].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][1].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][1].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][1].Name"]);
        spriggitFields["VirtualMachineAdapter[0][1].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][1].Object"]);
        spriggitFields["VirtualMachineAdapter[0][2].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][2].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][2].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][2].Name"]);
        spriggitFields["VirtualMachineAdapter[0][2].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][2].Object"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "CONT")]
    [Trait("FormKey", "1C0292:Fallout4.esm")]
    [Trait("EditorID", "TheaterTickerTape_Safe")]
    [Trait("SpriggitFile", "Containers/TheaterTickerTape_Safe - 1C0292_Fallout4.esm.yaml")]
    public void Fallout4_CONT_ShouldMatchSpriggitSample_TheaterTickerTape_Safe()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Container,
            "TheaterTickerTape_Safe");
        var dto = Helpers.GetDTO<ContainerDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Container,
            "1C0292:Fallout4.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["CloseSound"].ShouldBe(dtoFields["CloseSound"]);
        spriggitFields["Count[0]"].ShouldBe(dtoFields["Count[0]"]);
        spriggitFields["Count[1]"].ShouldBe(dtoFields["Count[1]"]);
        spriggitFields["Count[2]"].ShouldBe(dtoFields["Count[2]"]);
        spriggitFields["Count[3]"].ShouldBe(dtoFields["Count[3]"]);
        spriggitFields["Count[4]"].ShouldBe(dtoFields["Count[4]"]);
        spriggitFields["Count[5]"].ShouldBe(dtoFields["Count[5]"]);
        spriggitFields["Count[6]"].ShouldBe(dtoFields["Count[6]"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["Item[0]"].ShouldBe(dtoFields["Item[0]"]);
        spriggitFields["Item[1]"].ShouldBe(dtoFields["Item[1]"]);
        spriggitFields["Item[2]"].ShouldBe(dtoFields["Item[2]"]);
        spriggitFields["Item[3]"].ShouldBe(dtoFields["Item[3]"]);
        spriggitFields["Item[4]"].ShouldBe(dtoFields["Item[4]"]);
        spriggitFields["Item[5]"].ShouldBe(dtoFields["Item[5]"]);
        spriggitFields["Item[6]"].ShouldBe(dtoFields["Item[6]"]);
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
        spriggitFields["NativeTerminal"].ShouldBe(dtoFields["NativeTerminalFormKey"]);
        spriggitFields["ObjectBounds.First"].ShouldBe(dtoFields["ObjectBoundsFirst"]);
        spriggitFields["ObjectBounds.Second"].ShouldBe(dtoFields["ObjectBoundsSecond"]);
        spriggitFields["OpenSound"].ShouldBe(dtoFields["OpenSound"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "CONT")]
    [Trait("FormKey", "06355F:Fallout4.esm")]
    [Trait("EditorID", "Loot_Trunk_Boss")]
    [Trait("SpriggitFile", "Containers/Loot_Trunk_Boss - 06355F_Fallout4.esm.yaml")]
    public void Fallout4_CONT_ShouldMatchSpriggitSample_Loot_Trunk_Boss()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Container,
            "Loot_Trunk_Boss");
        var dto = Helpers.GetDTO<ContainerDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Container,
            "06355F:Fallout4.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["CloseSound"].ShouldBe(dtoFields["CloseSound"]);
        spriggitFields["Count[0]"].ShouldBe(dtoFields["Count[0]"]);
        spriggitFields["Count[1]"].ShouldBe(dtoFields["Count[1]"]);
        spriggitFields["Count[2]"].ShouldBe(dtoFields["Count[2]"]);
        spriggitFields["Count[3]"].ShouldBe(dtoFields["Count[3]"]);
        spriggitFields["Count[4]"].ShouldBe(dtoFields["Count[4]"]);
        spriggitFields["Count[5]"].ShouldBe(dtoFields["Count[5]"]);
        spriggitFields["Count[6]"].ShouldBe(dtoFields["Count[6]"]);
        spriggitFields["Count[7]"].ShouldBe(dtoFields["Count[7]"]);
        spriggitFields["Count[8]"].ShouldBe(dtoFields["Count[8]"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["Item[0]"].ShouldBe(dtoFields["Item[0]"]);
        spriggitFields["Item[1]"].ShouldBe(dtoFields["Item[1]"]);
        spriggitFields["Item[2]"].ShouldBe(dtoFields["Item[2]"]);
        spriggitFields["Item[3]"].ShouldBe(dtoFields["Item[3]"]);
        spriggitFields["Item[4]"].ShouldBe(dtoFields["Item[4]"]);
        spriggitFields["Item[5]"].ShouldBe(dtoFields["Item[5]"]);
        spriggitFields["Item[6]"].ShouldBe(dtoFields["Item[6]"]);
        spriggitFields["Item[7]"].ShouldBe(dtoFields["Item[7]"]);
        spriggitFields["Item[8]"].ShouldBe(dtoFields["Item[8]"]);
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
        spriggitFields["ObjectBounds.First"].ShouldBe(dtoFields["ObjectBoundsFirst"]);
        spriggitFields["ObjectBounds.Second"].ShouldBe(dtoFields["ObjectBoundsSecond"]);
        spriggitFields["OpenSound"].ShouldBe(dtoFields["OpenSound"]);
        spriggitFields["TakeAllSound"].ShouldBe(dtoFields["TakeAllSound"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "CONT")]
    [Trait("FormKey", "11CB14:Fallout4.esm")]
    [Trait("EditorID", "DN123_SkylanesSecretCompartment")]
    [Trait("SpriggitFile", "Containers/DN123_SkylanesSecretCompartment - 11CB14_Fallout4.esm.yaml")]
    public void Fallout4_CONT_ShouldMatchSpriggitSample_DN123_SkylanesSecretCompartment()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Container,
            "DN123_SkylanesSecretCompartment");
        var dto = Helpers.GetDTO<ContainerDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Container,
            "11CB14:Fallout4.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["CloseSound"].ShouldBe(dtoFields["CloseSound"]);
        spriggitFields["Count"].ShouldBe(dtoFields["Count"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["Item"].ShouldBe(dtoFields["Item"]);
        spriggitFields["MajorRecordFlagsRaw"].ShouldBe(dtoFields["MajorRecordFlags"]);
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
        spriggitFields["ObjectBounds.First"].ShouldBe(dtoFields["ObjectBoundsFirst"]);
        spriggitFields["ObjectBounds.Second"].ShouldBe(dtoFields["ObjectBoundsSecond"]);
        spriggitFields["OpenSound"].ShouldBe(dtoFields["OpenSound"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["VirtualMachineAdapter.Count"].ShouldBe(dtoFields["VirtualMachineAdapter.Count"]);
        spriggitFields["VirtualMachineAdapter[0].Count"].ShouldBe(dtoFields["VirtualMachineAdapter[0].Count"]);
        spriggitFields["VirtualMachineAdapter[0].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0].Name"]);
        spriggitFields["VirtualMachineAdapter[0][0].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][0].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0].Name"]);
        spriggitFields["VirtualMachineAdapter[0][0].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][0].Object"]);
        spriggitFields["VirtualMachineAdapter[0][1].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][1].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][1].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][1].Name"]);
        spriggitFields["VirtualMachineAdapter[0][1].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][1].Object"]);
        spriggitFields["VirtualMachineAdapter[0][2].Alias"].ShouldBe(dtoFields["VirtualMachineAdapter[0][2].Alias"]);
        spriggitFields["VirtualMachineAdapter[0][2].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][2].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][2].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][2].Name"]);
        spriggitFields["VirtualMachineAdapter[0][2].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][2].Object"]);
        spriggitFields["VirtualMachineAdapter[0][3].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][3].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][3].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][3].Name"]);
        spriggitFields["VirtualMachineAdapter[0][3].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][3].Object"]);
        spriggitFields["VirtualMachineAdapter[0][4].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][4].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][4].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][4].Name"]);
        spriggitFields["VirtualMachineAdapter[0][4].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][4].Object"]);
        spriggitFields["VirtualMachineAdapter[0][5].Count"].ShouldBe(dtoFields["VirtualMachineAdapter[0][5].Count"]);
        spriggitFields["VirtualMachineAdapter[0][5].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][5].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][5].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][5].Name"]);
        spriggitFields["VirtualMachineAdapter[0][5][0].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][5][0].Name"]);
        spriggitFields["VirtualMachineAdapter[0][5][0].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][5][0].Object"]);
        spriggitFields["VirtualMachineAdapter[0][5][1].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][5][1].Name"]);
        spriggitFields["VirtualMachineAdapter[0][5][1].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][5][1].Object"]);
        spriggitFields["VirtualMachineAdapter[0][5][2].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][5][2].Name"]);
        spriggitFields["VirtualMachineAdapter[0][5][2].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][5][2].Object"]);
        spriggitFields["VirtualMachineAdapter[0][5][3].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][5][3].Name"]);
        spriggitFields["VirtualMachineAdapter[0][5][3].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][5][3].Object"]);
        spriggitFields["VirtualMachineAdapter[0][5][4].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][5][4].Name"]);
        spriggitFields["VirtualMachineAdapter[0][5][4].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][5][4].Object"]);
        spriggitFields["VirtualMachineAdapter[0][5][5].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][5][5].Name"]);
        spriggitFields["VirtualMachineAdapter[0][5][5].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][5][5].Object"]);
        spriggitFields["VirtualMachineAdapter[0][5][6].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][5][6].Name"]);
        spriggitFields["VirtualMachineAdapter[0][5][6].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][5][6].Object"]);
        spriggitFields["VirtualMachineAdapter[0][5][7].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][5][7].Name"]);
        spriggitFields["VirtualMachineAdapter[0][5][7].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][5][7].Object"]);
        spriggitFields["VirtualMachineAdapter[0][5][8].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][5][8].Name"]);
        spriggitFields["VirtualMachineAdapter[0][5][8].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][5][8].Object"]);
        spriggitFields["VirtualMachineAdapter[0][5][9].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][5][9].Name"]);
        spriggitFields["VirtualMachineAdapter[0][5][9].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][5][9].Object"]);
        spriggitFields["VirtualMachineAdapter[0][6].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter[0][6].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter[0][6].Name"].ShouldBe(dtoFields["VirtualMachineAdapter[0][6].Name"]);
        spriggitFields["VirtualMachineAdapter[0][6].Object"].ShouldBe(dtoFields["VirtualMachineAdapter[0][6].Object"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }
}
