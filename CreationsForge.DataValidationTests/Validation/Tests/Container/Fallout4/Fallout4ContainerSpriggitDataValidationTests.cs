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

        Helpers.GetSpriggitField(spriggit, "CloseSound").ShouldBe(Helpers.GetDTOField(dto, "CloseSound"));
        Helpers.GetSpriggitField(spriggit, "Count[0]").ShouldBe(Helpers.GetDTOField(dto, "Count[0]"));
        Helpers.GetSpriggitField(spriggit, "Count[1]").ShouldBe(Helpers.GetDTOField(dto, "Count[1]"));
        Helpers.GetSpriggitField(spriggit, "Count[2]").ShouldBe(Helpers.GetDTOField(dto, "Count[2]"));
        Helpers.GetSpriggitField(spriggit, "Count[3]").ShouldBe(Helpers.GetDTOField(dto, "Count[3]"));
        Helpers.GetSpriggitField(spriggit, "Count[4]").ShouldBe(Helpers.GetDTOField(dto, "Count[4]"));
        Helpers.GetSpriggitField(spriggit, "Count[5]").ShouldBe(Helpers.GetDTOField(dto, "Count[5]"));
        Helpers.GetSpriggitField(spriggit, "Count[6]").ShouldBe(Helpers.GetDTOField(dto, "Count[6]"));
        Helpers.GetSpriggitField(spriggit, "Count[7]").ShouldBe(Helpers.GetDTOField(dto, "Count[7]"));
        Helpers.GetSpriggitField(spriggit, "Count[8]").ShouldBe(Helpers.GetDTOField(dto, "Count[8]"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "Item[0]").ShouldBe(Helpers.GetDTOField(dto, "Item[0]"));
        Helpers.GetSpriggitField(spriggit, "Item[1]").ShouldBe(Helpers.GetDTOField(dto, "Item[1]"));
        Helpers.GetSpriggitField(spriggit, "Item[2]").ShouldBe(Helpers.GetDTOField(dto, "Item[2]"));
        Helpers.GetSpriggitField(spriggit, "Item[3]").ShouldBe(Helpers.GetDTOField(dto, "Item[3]"));
        Helpers.GetSpriggitField(spriggit, "Item[4]").ShouldBe(Helpers.GetDTOField(dto, "Item[4]"));
        Helpers.GetSpriggitField(spriggit, "Item[5]").ShouldBe(Helpers.GetDTOField(dto, "Item[5]"));
        Helpers.GetSpriggitField(spriggit, "Item[6]").ShouldBe(Helpers.GetDTOField(dto, "Item[6]"));
        Helpers.GetSpriggitField(spriggit, "Item[7]").ShouldBe(Helpers.GetDTOField(dto, "Item[7]"));
        Helpers.GetSpriggitField(spriggit, "Item[8]").ShouldBe(Helpers.GetDTOField(dto, "Item[8]"));
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
        Helpers.GetSpriggitField(spriggit, "NativeTerminal").ShouldBe(Helpers.GetDTOField(dto, "NativeTerminalFormKey"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.First").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsFirst"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.Second").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsSecond"));
        Helpers.GetSpriggitField(spriggit, "OpenSound").ShouldBe(Helpers.GetDTOField(dto, "OpenSound"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "CloseSound", "Count[0]", "Count[1]", "Count[2]", "Count[3]", "Count[4]", "Count[5]", "Count[6]", "Count[7]", "Count[8]", "EditorID", "FormKey", "Item[0]", "Item[1]", "Item[2]", "Item[3]", "Item[4]", "Item[5]", "Item[6]", "Item[7]", "Item[8]", "Model.Data", "Model.File", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "NativeTerminal", "ObjectBounds.First", "ObjectBounds.Second", "OpenSound", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "CloseSound", "Count[0]", "Count[1]", "Count[2]", "Count[3]", "Count[4]", "Count[5]", "Count[6]", "Count[7]", "Count[8]", "EditorID", "FormKey", "Item[0]", "Item[1]", "Item[2]", "Item[3]", "Item[4]", "Item[5]", "Item[6]", "Item[7]", "Item[8]", "Model.Data", "Models[0].File", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "NativeTerminalFormKey", "ObjectBoundsFirst", "ObjectBoundsSecond", "OpenSound", "Version2", "VersionControl");
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

        Helpers.GetSpriggitField(spriggit, "CloseSound").ShouldBe(Helpers.GetDTOField(dto, "CloseSound"));
        Helpers.GetSpriggitField(spriggit, "Count[0]").ShouldBe(Helpers.GetDTOField(dto, "Count[0]"));
        Helpers.GetSpriggitField(spriggit, "Count[1]").ShouldBe(Helpers.GetDTOField(dto, "Count[1]"));
        Helpers.GetSpriggitField(spriggit, "Count[2]").ShouldBe(Helpers.GetDTOField(dto, "Count[2]"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "Item[0]").ShouldBe(Helpers.GetDTOField(dto, "Item[0]"));
        Helpers.GetSpriggitField(spriggit, "Item[1]").ShouldBe(Helpers.GetDTOField(dto, "Item[1]"));
        Helpers.GetSpriggitField(spriggit, "Item[2]").ShouldBe(Helpers.GetDTOField(dto, "Item[2]"));
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
        Helpers.GetSpriggitField(spriggit, "NativeTerminal").ShouldBe(Helpers.GetDTOField(dto, "NativeTerminalFormKey"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.First").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsFirst"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.Second").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsSecond"));
        Helpers.GetSpriggitField(spriggit, "OpenSound").ShouldBe(Helpers.GetDTOField(dto, "OpenSound"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter.Count").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter.Count"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0].Count").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0].Count"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][1].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][1].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][1].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][1].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][1].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][1].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][2].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][2].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][2].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][2].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][2].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][2].Object"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "CloseSound", "Count[0]", "Count[1]", "Count[2]", "EditorID", "FormKey", "Item[0]", "Item[1]", "Item[2]", "Model.Data", "Model.File", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "NativeTerminal", "ObjectBounds.First", "ObjectBounds.Second", "OpenSound", "Version2", "VersionControl", "VirtualMachineAdapter.Count", "VirtualMachineAdapter[0].Count", "VirtualMachineAdapter[0].Name", "VirtualMachineAdapter[0][0].MutagenObjectType", "VirtualMachineAdapter[0][0].Name", "VirtualMachineAdapter[0][0].Object", "VirtualMachineAdapter[0][1].MutagenObjectType", "VirtualMachineAdapter[0][1].Name", "VirtualMachineAdapter[0][1].Object", "VirtualMachineAdapter[0][2].MutagenObjectType", "VirtualMachineAdapter[0][2].Name", "VirtualMachineAdapter[0][2].Object");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "CloseSound", "Count[0]", "Count[1]", "Count[2]", "EditorID", "FormKey", "Item[0]", "Item[1]", "Item[2]", "Model.Data", "Models[0].File", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "NativeTerminalFormKey", "ObjectBoundsFirst", "ObjectBoundsSecond", "OpenSound", "Version2", "VersionControl", "VirtualMachineAdapter.Count", "VirtualMachineAdapter[0].Count", "VirtualMachineAdapter[0].Name", "VirtualMachineAdapter[0][0].MutagenObjectType", "VirtualMachineAdapter[0][0].Name", "VirtualMachineAdapter[0][0].Object", "VirtualMachineAdapter[0][1].MutagenObjectType", "VirtualMachineAdapter[0][1].Name", "VirtualMachineAdapter[0][1].Object", "VirtualMachineAdapter[0][2].MutagenObjectType", "VirtualMachineAdapter[0][2].Name", "VirtualMachineAdapter[0][2].Object");
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

        Helpers.GetSpriggitField(spriggit, "CloseSound").ShouldBe(Helpers.GetDTOField(dto, "CloseSound"));
        Helpers.GetSpriggitField(spriggit, "Count[0]").ShouldBe(Helpers.GetDTOField(dto, "Count[0]"));
        Helpers.GetSpriggitField(spriggit, "Count[1]").ShouldBe(Helpers.GetDTOField(dto, "Count[1]"));
        Helpers.GetSpriggitField(spriggit, "Count[2]").ShouldBe(Helpers.GetDTOField(dto, "Count[2]"));
        Helpers.GetSpriggitField(spriggit, "Count[3]").ShouldBe(Helpers.GetDTOField(dto, "Count[3]"));
        Helpers.GetSpriggitField(spriggit, "Count[4]").ShouldBe(Helpers.GetDTOField(dto, "Count[4]"));
        Helpers.GetSpriggitField(spriggit, "Count[5]").ShouldBe(Helpers.GetDTOField(dto, "Count[5]"));
        Helpers.GetSpriggitField(spriggit, "Count[6]").ShouldBe(Helpers.GetDTOField(dto, "Count[6]"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "Item[0]").ShouldBe(Helpers.GetDTOField(dto, "Item[0]"));
        Helpers.GetSpriggitField(spriggit, "Item[1]").ShouldBe(Helpers.GetDTOField(dto, "Item[1]"));
        Helpers.GetSpriggitField(spriggit, "Item[2]").ShouldBe(Helpers.GetDTOField(dto, "Item[2]"));
        Helpers.GetSpriggitField(spriggit, "Item[3]").ShouldBe(Helpers.GetDTOField(dto, "Item[3]"));
        Helpers.GetSpriggitField(spriggit, "Item[4]").ShouldBe(Helpers.GetDTOField(dto, "Item[4]"));
        Helpers.GetSpriggitField(spriggit, "Item[5]").ShouldBe(Helpers.GetDTOField(dto, "Item[5]"));
        Helpers.GetSpriggitField(spriggit, "Item[6]").ShouldBe(Helpers.GetDTOField(dto, "Item[6]"));
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
        Helpers.GetSpriggitField(spriggit, "NativeTerminal").ShouldBe(Helpers.GetDTOField(dto, "NativeTerminalFormKey"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.First").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsFirst"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.Second").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsSecond"));
        Helpers.GetSpriggitField(spriggit, "OpenSound").ShouldBe(Helpers.GetDTOField(dto, "OpenSound"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "CloseSound", "Count[0]", "Count[1]", "Count[2]", "Count[3]", "Count[4]", "Count[5]", "Count[6]", "EditorID", "FormKey", "Item[0]", "Item[1]", "Item[2]", "Item[3]", "Item[4]", "Item[5]", "Item[6]", "Model.Data", "Model.File", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "NativeTerminal", "ObjectBounds.First", "ObjectBounds.Second", "OpenSound", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "CloseSound", "Count[0]", "Count[1]", "Count[2]", "Count[3]", "Count[4]", "Count[5]", "Count[6]", "EditorID", "FormKey", "Item[0]", "Item[1]", "Item[2]", "Item[3]", "Item[4]", "Item[5]", "Item[6]", "Model.Data", "Models[0].File", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "NativeTerminalFormKey", "ObjectBoundsFirst", "ObjectBoundsSecond", "OpenSound", "Version2", "VersionControl");
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

        Helpers.GetSpriggitField(spriggit, "CloseSound").ShouldBe(Helpers.GetDTOField(dto, "CloseSound"));
        Helpers.GetSpriggitField(spriggit, "Count[0]").ShouldBe(Helpers.GetDTOField(dto, "Count[0]"));
        Helpers.GetSpriggitField(spriggit, "Count[1]").ShouldBe(Helpers.GetDTOField(dto, "Count[1]"));
        Helpers.GetSpriggitField(spriggit, "Count[2]").ShouldBe(Helpers.GetDTOField(dto, "Count[2]"));
        Helpers.GetSpriggitField(spriggit, "Count[3]").ShouldBe(Helpers.GetDTOField(dto, "Count[3]"));
        Helpers.GetSpriggitField(spriggit, "Count[4]").ShouldBe(Helpers.GetDTOField(dto, "Count[4]"));
        Helpers.GetSpriggitField(spriggit, "Count[5]").ShouldBe(Helpers.GetDTOField(dto, "Count[5]"));
        Helpers.GetSpriggitField(spriggit, "Count[6]").ShouldBe(Helpers.GetDTOField(dto, "Count[6]"));
        Helpers.GetSpriggitField(spriggit, "Count[7]").ShouldBe(Helpers.GetDTOField(dto, "Count[7]"));
        Helpers.GetSpriggitField(spriggit, "Count[8]").ShouldBe(Helpers.GetDTOField(dto, "Count[8]"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "Item[0]").ShouldBe(Helpers.GetDTOField(dto, "Item[0]"));
        Helpers.GetSpriggitField(spriggit, "Item[1]").ShouldBe(Helpers.GetDTOField(dto, "Item[1]"));
        Helpers.GetSpriggitField(spriggit, "Item[2]").ShouldBe(Helpers.GetDTOField(dto, "Item[2]"));
        Helpers.GetSpriggitField(spriggit, "Item[3]").ShouldBe(Helpers.GetDTOField(dto, "Item[3]"));
        Helpers.GetSpriggitField(spriggit, "Item[4]").ShouldBe(Helpers.GetDTOField(dto, "Item[4]"));
        Helpers.GetSpriggitField(spriggit, "Item[5]").ShouldBe(Helpers.GetDTOField(dto, "Item[5]"));
        Helpers.GetSpriggitField(spriggit, "Item[6]").ShouldBe(Helpers.GetDTOField(dto, "Item[6]"));
        Helpers.GetSpriggitField(spriggit, "Item[7]").ShouldBe(Helpers.GetDTOField(dto, "Item[7]"));
        Helpers.GetSpriggitField(spriggit, "Item[8]").ShouldBe(Helpers.GetDTOField(dto, "Item[8]"));
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
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.First").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsFirst"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.Second").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsSecond"));
        Helpers.GetSpriggitField(spriggit, "OpenSound").ShouldBe(Helpers.GetDTOField(dto, "OpenSound"));
        Helpers.GetSpriggitField(spriggit, "TakeAllSound").ShouldBe(Helpers.GetDTOField(dto, "TakeAllSound"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "CloseSound", "Count[0]", "Count[1]", "Count[2]", "Count[3]", "Count[4]", "Count[5]", "Count[6]", "Count[7]", "Count[8]", "EditorID", "FormKey", "Item[0]", "Item[1]", "Item[2]", "Item[3]", "Item[4]", "Item[5]", "Item[6]", "Item[7]", "Item[8]", "Model.Data", "Model.File", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "ObjectBounds.First", "ObjectBounds.Second", "OpenSound", "TakeAllSound", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "CloseSound", "Count[0]", "Count[1]", "Count[2]", "Count[3]", "Count[4]", "Count[5]", "Count[6]", "Count[7]", "Count[8]", "EditorID", "FormKey", "Item[0]", "Item[1]", "Item[2]", "Item[3]", "Item[4]", "Item[5]", "Item[6]", "Item[7]", "Item[8]", "Model.Data", "Models[0].File", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "ObjectBoundsFirst", "ObjectBoundsSecond", "OpenSound", "TakeAllSound", "Version2", "VersionControl");
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

        Helpers.GetSpriggitField(spriggit, "CloseSound").ShouldBe(Helpers.GetDTOField(dto, "CloseSound"));
        Helpers.GetSpriggitField(spriggit, "Count").ShouldBe(Helpers.GetDTOField(dto, "Count"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "Item").ShouldBe(Helpers.GetDTOField(dto, "Item"));
        Helpers.GetSpriggitField(spriggit, "MajorRecordFlagsRaw").ShouldBe(Helpers.GetDTOField(dto, "MajorRecordFlags"));
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
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.First").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsFirst"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.Second").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsSecond"));
        Helpers.GetSpriggitField(spriggit, "OpenSound").ShouldBe(Helpers.GetDTOField(dto, "OpenSound"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter.Count").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter.Count"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0].Count").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0].Count"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][0].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][0].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][1].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][1].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][1].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][1].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][1].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][1].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][2].Alias").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][2].Alias"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][2].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][2].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][2].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][2].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][2].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][2].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][3].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][3].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][3].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][3].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][3].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][3].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][4].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][4].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][4].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][4].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][4].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][4].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][5].Count").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][5].Count"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][5].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][5].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][5].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][5].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][5][0].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][5][0].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][5][0].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][5][0].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][5][1].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][5][1].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][5][1].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][5][1].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][5][2].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][5][2].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][5][2].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][5][2].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][5][3].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][5][3].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][5][3].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][5][3].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][5][4].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][5][4].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][5][4].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][5][4].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][5][5].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][5][5].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][5][5].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][5][5].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][5][6].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][5][6].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][5][6].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][5][6].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][5][7].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][5][7].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][5][7].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][5][7].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][5][8].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][5][8].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][5][8].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][5][8].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][5][9].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][5][9].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][5][9].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][5][9].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][6].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][6].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][6].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][6].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter[0][6].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter[0][6].Object"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "CloseSound", "Count", "EditorID", "FormKey", "Item", "MajorRecordFlagsRaw", "Model.Data", "Model.File", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "ObjectBounds.First", "ObjectBounds.Second", "OpenSound", "Version2", "VersionControl", "VirtualMachineAdapter.Count", "VirtualMachineAdapter[0].Count", "VirtualMachineAdapter[0].Name", "VirtualMachineAdapter[0][0].MutagenObjectType", "VirtualMachineAdapter[0][0].Name", "VirtualMachineAdapter[0][0].Object", "VirtualMachineAdapter[0][1].MutagenObjectType", "VirtualMachineAdapter[0][1].Name", "VirtualMachineAdapter[0][1].Object", "VirtualMachineAdapter[0][2].Alias", "VirtualMachineAdapter[0][2].MutagenObjectType", "VirtualMachineAdapter[0][2].Name", "VirtualMachineAdapter[0][2].Object", "VirtualMachineAdapter[0][3].MutagenObjectType", "VirtualMachineAdapter[0][3].Name", "VirtualMachineAdapter[0][3].Object", "VirtualMachineAdapter[0][4].MutagenObjectType", "VirtualMachineAdapter[0][4].Name", "VirtualMachineAdapter[0][4].Object", "VirtualMachineAdapter[0][5].Count", "VirtualMachineAdapter[0][5].MutagenObjectType", "VirtualMachineAdapter[0][5].Name", "VirtualMachineAdapter[0][5][0].Name", "VirtualMachineAdapter[0][5][0].Object", "VirtualMachineAdapter[0][5][1].Name", "VirtualMachineAdapter[0][5][1].Object", "VirtualMachineAdapter[0][5][2].Name", "VirtualMachineAdapter[0][5][2].Object", "VirtualMachineAdapter[0][5][3].Name", "VirtualMachineAdapter[0][5][3].Object", "VirtualMachineAdapter[0][5][4].Name", "VirtualMachineAdapter[0][5][4].Object", "VirtualMachineAdapter[0][5][5].Name", "VirtualMachineAdapter[0][5][5].Object", "VirtualMachineAdapter[0][5][6].Name", "VirtualMachineAdapter[0][5][6].Object", "VirtualMachineAdapter[0][5][7].Name", "VirtualMachineAdapter[0][5][7].Object", "VirtualMachineAdapter[0][5][8].Name", "VirtualMachineAdapter[0][5][8].Object", "VirtualMachineAdapter[0][5][9].Name", "VirtualMachineAdapter[0][5][9].Object", "VirtualMachineAdapter[0][6].MutagenObjectType", "VirtualMachineAdapter[0][6].Name", "VirtualMachineAdapter[0][6].Object");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "CloseSound", "Count", "EditorID", "FormKey", "Item", "MajorRecordFlags", "Model.Data", "Models[0].File", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "ObjectBoundsFirst", "ObjectBoundsSecond", "OpenSound", "Version2", "VersionControl", "VirtualMachineAdapter.Count", "VirtualMachineAdapter[0].Count", "VirtualMachineAdapter[0].Name", "VirtualMachineAdapter[0][0].MutagenObjectType", "VirtualMachineAdapter[0][0].Name", "VirtualMachineAdapter[0][0].Object", "VirtualMachineAdapter[0][1].MutagenObjectType", "VirtualMachineAdapter[0][1].Name", "VirtualMachineAdapter[0][1].Object", "VirtualMachineAdapter[0][2].Alias", "VirtualMachineAdapter[0][2].MutagenObjectType", "VirtualMachineAdapter[0][2].Name", "VirtualMachineAdapter[0][2].Object", "VirtualMachineAdapter[0][3].MutagenObjectType", "VirtualMachineAdapter[0][3].Name", "VirtualMachineAdapter[0][3].Object", "VirtualMachineAdapter[0][4].MutagenObjectType", "VirtualMachineAdapter[0][4].Name", "VirtualMachineAdapter[0][4].Object", "VirtualMachineAdapter[0][5].Count", "VirtualMachineAdapter[0][5].MutagenObjectType", "VirtualMachineAdapter[0][5].Name", "VirtualMachineAdapter[0][5][0].Name", "VirtualMachineAdapter[0][5][0].Object", "VirtualMachineAdapter[0][5][1].Name", "VirtualMachineAdapter[0][5][1].Object", "VirtualMachineAdapter[0][5][2].Name", "VirtualMachineAdapter[0][5][2].Object", "VirtualMachineAdapter[0][5][3].Name", "VirtualMachineAdapter[0][5][3].Object", "VirtualMachineAdapter[0][5][4].Name", "VirtualMachineAdapter[0][5][4].Object", "VirtualMachineAdapter[0][5][5].Name", "VirtualMachineAdapter[0][5][5].Object", "VirtualMachineAdapter[0][5][6].Name", "VirtualMachineAdapter[0][5][6].Object", "VirtualMachineAdapter[0][5][7].Name", "VirtualMachineAdapter[0][5][7].Object", "VirtualMachineAdapter[0][5][8].Name", "VirtualMachineAdapter[0][5][8].Object", "VirtualMachineAdapter[0][5][9].Name", "VirtualMachineAdapter[0][5][9].Object", "VirtualMachineAdapter[0][6].MutagenObjectType", "VirtualMachineAdapter[0][6].Name", "VirtualMachineAdapter[0][6].Object");
    }
}