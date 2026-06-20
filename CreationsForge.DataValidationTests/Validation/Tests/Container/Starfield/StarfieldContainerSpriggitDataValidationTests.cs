using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.Container.Starfield;

public class StarfieldContainerSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "CONT")]
    [Trait("FormKey", "277A73:Starfield.esm")]
    [Trait("EditorID", "ShipOutpost_Loot_Storage_Safe_Floor_Reg_Common")]
    [Trait("SpriggitFile", "Containers/ShipOutpost_Loot_Storage_Safe_Floor_Reg_Common - 277A73_Starfield.esm.yaml")]
    public void Starfield_CONT_ShouldMatchSpriggitSample_ShipOutpost_Loot_Storage_Safe_Floor_Reg_Common()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Container,
            "ShipOutpost_Loot_Storage_Safe_Floor_Reg_Common");
        var dto = Helpers.GetDTO<ContainerDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Container,
            "277A73:Starfield.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["ANAM"].ShouldBe(dtoFields["ANAM"]);
        spriggitFields["BNAM"].ShouldBe(dtoFields["BNAM"]);
        spriggitFields["CloseSound.Start"].ShouldBe(dtoFields["CloseSound.Start"]);
        spriggitFields["CNAM"].ShouldBe(dtoFields["CNAM"]);
        spriggitFields["Count[0]"].ShouldBe(dtoFields["Count[0]"]);
        spriggitFields["Count[1]"].ShouldBe(dtoFields["Count[1]"]);
        spriggitFields["Count[2]"].ShouldBe(dtoFields["Count[2]"]);
        spriggitFields["Count[3]"].ShouldBe(dtoFields["Count[3]"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["Item[0]"].ShouldBe(dtoFields["Item[0]"]);
        spriggitFields["Item[1]"].ShouldBe(dtoFields["Item[1]"]);
        spriggitFields["Item[2]"].ShouldBe(dtoFields["Item[2]"]);
        spriggitFields["Item[3]"].ShouldBe(dtoFields["Item[3]"]);
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
        spriggitFields["ObjectBounds.First"].ShouldBe(dtoFields["ObjectBoundsFirst"]);
        spriggitFields["ObjectBounds.Second"].ShouldBe(dtoFields["ObjectBoundsSecond"]);
        spriggitFields["OpenSound.Start"].ShouldBe(dtoFields["OpenSound.Start"]);
        spriggitFields["REFL"].ShouldBe(dtoFields["REFL"]);
        spriggitFields["Transforms.Outpost"].ShouldBe(dtoFields["Transforms.Outpost"]);
        spriggitFields["Transforms.Preview"].ShouldBe(dtoFields["Transforms.Preview"]);
        spriggitFields["Value[0]"].ShouldBe(dtoFields["Value[0]"]);
        spriggitFields["Value[1]"].ShouldBe(dtoFields["Value[1]"]);
        spriggitFields["Value[2]"].ShouldBe(dtoFields["Value[2]"]);
        spriggitFields["Value[3]"].ShouldBe(dtoFields["Value[3]"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "CONT")]
    [Trait("FormKey", "277A81:Starfield.esm")]
    [Trait("EditorID", "ShipOutpost_Loot_Storage_Safe_Floor_Tall_Rare")]
    [Trait("SpriggitFile", "Containers/ShipOutpost_Loot_Storage_Safe_Floor_Tall_Rare - 277A81_Starfield.esm.yaml")]
    public void Starfield_CONT_ShouldMatchSpriggitSample_ShipOutpost_Loot_Storage_Safe_Floor_Tall_Rare()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Container,
            "ShipOutpost_Loot_Storage_Safe_Floor_Tall_Rare");
        var dto = Helpers.GetDTO<ContainerDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Container,
            "277A81:Starfield.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["ANAM"].ShouldBe(dtoFields["ANAM"]);
        spriggitFields["BNAM"].ShouldBe(dtoFields["BNAM"]);
        spriggitFields["CloseSound.Start"].ShouldBe(dtoFields["CloseSound.Start"]);
        spriggitFields["CNAM"].ShouldBe(dtoFields["CNAM"]);
        spriggitFields["Count[0]"].ShouldBe(dtoFields["Count[0]"]);
        spriggitFields["Count[1]"].ShouldBe(dtoFields["Count[1]"]);
        spriggitFields["Count[2]"].ShouldBe(dtoFields["Count[2]"]);
        spriggitFields["Count[3]"].ShouldBe(dtoFields["Count[3]"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["Item[0]"].ShouldBe(dtoFields["Item[0]"]);
        spriggitFields["Item[1]"].ShouldBe(dtoFields["Item[1]"]);
        spriggitFields["Item[2]"].ShouldBe(dtoFields["Item[2]"]);
        spriggitFields["Item[3]"].ShouldBe(dtoFields["Item[3]"]);
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
        spriggitFields["ObjectBounds.First"].ShouldBe(dtoFields["ObjectBoundsFirst"]);
        spriggitFields["ObjectBounds.Second"].ShouldBe(dtoFields["ObjectBoundsSecond"]);
        spriggitFields["OpenSound.Start"].ShouldBe(dtoFields["OpenSound.Start"]);
        spriggitFields["REFL"].ShouldBe(dtoFields["REFL"]);
        spriggitFields["Transforms.Outpost"].ShouldBe(dtoFields["Transforms.Outpost"]);
        spriggitFields["Transforms.Preview"].ShouldBe(dtoFields["Transforms.Preview"]);
        spriggitFields["Value[0]"].ShouldBe(dtoFields["Value[0]"]);
        spriggitFields["Value[1]"].ShouldBe(dtoFields["Value[1]"]);
        spriggitFields["Value[2]"].ShouldBe(dtoFields["Value[2]"]);
        spriggitFields["Value[3]"].ShouldBe(dtoFields["Value[3]"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "CONT")]
    [Trait("FormKey", "2779E9:Starfield.esm")]
    [Trait("EditorID", "ShipOutpost_Loot_Storage_BossChest_Industrial_Rare")]
    [Trait("SpriggitFile", "Containers/ShipOutpost_Loot_Storage_BossChest_Industrial_Rare - 2779E9_Starfield.esm.yaml")]
    public void Starfield_CONT_ShouldMatchSpriggitSample_ShipOutpost_Loot_Storage_BossChest_Industrial_Rare()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Container,
            "ShipOutpost_Loot_Storage_BossChest_Industrial_Rare");
        var dto = Helpers.GetDTO<ContainerDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Container,
            "2779E9:Starfield.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["ANAM"].ShouldBe(dtoFields["ANAM"]);
        spriggitFields["BNAM"].ShouldBe(dtoFields["BNAM"]);
        spriggitFields["CloseSound.Start"].ShouldBe(dtoFields["CloseSound.Start"]);
        spriggitFields["CNAM"].ShouldBe(dtoFields["CNAM"]);
        spriggitFields["Count[0]"].ShouldBe(dtoFields["Count[0]"]);
        spriggitFields["Count[1]"].ShouldBe(dtoFields["Count[1]"]);
        spriggitFields["Count[2]"].ShouldBe(dtoFields["Count[2]"]);
        spriggitFields["Count[3]"].ShouldBe(dtoFields["Count[3]"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["Item[0]"].ShouldBe(dtoFields["Item[0]"]);
        spriggitFields["Item[1]"].ShouldBe(dtoFields["Item[1]"]);
        spriggitFields["Item[2]"].ShouldBe(dtoFields["Item[2]"]);
        spriggitFields["Item[3]"].ShouldBe(dtoFields["Item[3]"]);
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
        spriggitFields["ObjectBounds.First"].ShouldBe(dtoFields["ObjectBoundsFirst"]);
        spriggitFields["ObjectBounds.Second"].ShouldBe(dtoFields["ObjectBoundsSecond"]);
        spriggitFields["OpenSound.Start"].ShouldBe(dtoFields["OpenSound.Start"]);
        spriggitFields["REFL"].ShouldBe(dtoFields["REFL"]);
        spriggitFields["Transforms.Outpost"].ShouldBe(dtoFields["Transforms.Outpost"]);
        spriggitFields["Transforms.Preview"].ShouldBe(dtoFields["Transforms.Preview"]);
        spriggitFields["Value[0]"].ShouldBe(dtoFields["Value[0]"]);
        spriggitFields["Value[1]"].ShouldBe(dtoFields["Value[1]"]);
        spriggitFields["Value[2]"].ShouldBe(dtoFields["Value[2]"]);
        spriggitFields["Value[3]"].ShouldBe(dtoFields["Value[3]"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "CONT")]
    [Trait("FormKey", "1A23DF:Starfield.esm")]
    [Trait("EditorID", "Loot_Display_WeaponRack03_EMPTY")]
    [Trait("SpriggitFile", "Containers/Loot_Display_WeaponRack03_EMPTY - 1A23DF_Starfield.esm.yaml")]
    public void Starfield_CONT_ShouldMatchSpriggitSample_Loot_Display_WeaponRack03_EMPTY()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Container,
            "Loot_Display_WeaponRack03_EMPTY");
        var dto = Helpers.GetDTO<ContainerDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Container,
            "1A23DF:Starfield.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["ANAM"].ShouldBe(dtoFields["ANAM"]);
        spriggitFields["BNAM"].ShouldBe(dtoFields["BNAM"]);
        spriggitFields["CloseSound.Start"].ShouldBe(dtoFields["CloseSound.Start"]);
        spriggitFields["CNAM"].ShouldBe(dtoFields["CNAM"]);
        spriggitFields["ContainsOnlyFilter"].ShouldBe(dtoFields["ContainsOnlyFilter"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["Index[0]"].ShouldBe(dtoFields["Index[0]"]);
        spriggitFields["Index[1]"].ShouldBe(dtoFields["Index[1]"]);
        spriggitFields["Index[10]"].ShouldBe(dtoFields["Index[10]"]);
        spriggitFields["Index[2]"].ShouldBe(dtoFields["Index[2]"]);
        spriggitFields["Index[3]"].ShouldBe(dtoFields["Index[3]"]);
        spriggitFields["Index[4]"].ShouldBe(dtoFields["Index[4]"]);
        spriggitFields["Index[5]"].ShouldBe(dtoFields["Index[5]"]);
        spriggitFields["Index[6]"].ShouldBe(dtoFields["Index[6]"]);
        spriggitFields["Index[7]"].ShouldBe(dtoFields["Index[7]"]);
        spriggitFields["Index[8]"].ShouldBe(dtoFields["Index[8]"]);
        spriggitFields["Index[9]"].ShouldBe(dtoFields["Index[9]"]);
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
        spriggitFields["ObjectBounds.First"].ShouldBe(dtoFields["ObjectBoundsFirst"]);
        spriggitFields["ObjectBounds.Second"].ShouldBe(dtoFields["ObjectBoundsSecond"]);
        spriggitFields["OpenSound.Start"].ShouldBe(dtoFields["OpenSound.Start"]);
        spriggitFields["SnapTemplate"].ShouldBe(dtoFields["SnapTemplate"]);
        spriggitFields["Unknown2[0]"].ShouldBe(dtoFields["Unknown2[0]"]);
        spriggitFields["Unknown2[1]"].ShouldBe(dtoFields["Unknown2[1]"]);
        spriggitFields["Unknown2[10]"].ShouldBe(dtoFields["Unknown2[10]"]);
        spriggitFields["Unknown2[11]"].ShouldBe(dtoFields["Unknown2[11]"]);
        spriggitFields["Unknown2[2]"].ShouldBe(dtoFields["Unknown2[2]"]);
        spriggitFields["Unknown2[3]"].ShouldBe(dtoFields["Unknown2[3]"]);
        spriggitFields["Unknown2[4]"].ShouldBe(dtoFields["Unknown2[4]"]);
        spriggitFields["Unknown2[5]"].ShouldBe(dtoFields["Unknown2[5]"]);
        spriggitFields["Unknown2[6]"].ShouldBe(dtoFields["Unknown2[6]"]);
        spriggitFields["Unknown2[7]"].ShouldBe(dtoFields["Unknown2[7]"]);
        spriggitFields["Unknown2[8]"].ShouldBe(dtoFields["Unknown2[8]"]);
        spriggitFields["Unknown2[9]"].ShouldBe(dtoFields["Unknown2[9]"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "CONT")]
    [Trait("FormKey", "057C20:Starfield.esm")]
    [Trait("EditorID", "Loot_Display_ArboronWeaponRackPanel02")]
    [Trait("SpriggitFile", "Containers/Loot_Display_ArboronWeaponRackPanel02 - 057C20_Starfield.esm.yaml")]
    public void Starfield_CONT_ShouldMatchSpriggitSample_Loot_Display_ArboronWeaponRackPanel02()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Container,
            "Loot_Display_ArboronWeaponRackPanel02");
        var dto = Helpers.GetDTO<ContainerDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Container,
            "057C20:Starfield.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["CloseSound.Start"].ShouldBe(dtoFields["CloseSound.Start"]);
        spriggitFields["ContainsOnlyFilter"].ShouldBe(dtoFields["ContainsOnlyFilter"]);
        spriggitFields["Count[0]"].ShouldBe(dtoFields["Count[0]"]);
        spriggitFields["Count[1]"].ShouldBe(dtoFields["Count[1]"]);
        spriggitFields["Count[2]"].ShouldBe(dtoFields["Count[2]"]);
        spriggitFields["Count[3]"].ShouldBe(dtoFields["Count[3]"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["Index[0]"].ShouldBe(dtoFields["Index[0]"]);
        spriggitFields["Index[1]"].ShouldBe(dtoFields["Index[1]"]);
        spriggitFields["Index[2]"].ShouldBe(dtoFields["Index[2]"]);
        spriggitFields["Index[3]"].ShouldBe(dtoFields["Index[3]"]);
        spriggitFields["Index[4]"].ShouldBe(dtoFields["Index[4]"]);
        spriggitFields["Index[5]"].ShouldBe(dtoFields["Index[5]"]);
        spriggitFields["Index[6]"].ShouldBe(dtoFields["Index[6]"]);
        spriggitFields["Index[7]"].ShouldBe(dtoFields["Index[7]"]);
        spriggitFields["Item[0]"].ShouldBe(dtoFields["Item[0]"]);
        spriggitFields["Item[1]"].ShouldBe(dtoFields["Item[1]"]);
        spriggitFields["Item[2]"].ShouldBe(dtoFields["Item[2]"]);
        spriggitFields["Item[3]"].ShouldBe(dtoFields["Item[3]"]);
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
        spriggitFields["ObjectBounds.First"].ShouldBe(dtoFields["ObjectBoundsFirst"]);
        spriggitFields["ObjectBounds.Second"].ShouldBe(dtoFields["ObjectBoundsSecond"]);
        spriggitFields["OpenSound.Start"].ShouldBe(dtoFields["OpenSound.Start"]);
        spriggitFields["SnapTemplate"].ShouldBe(dtoFields["SnapTemplate"]);
        spriggitFields["Unknown2[0]"].ShouldBe(dtoFields["Unknown2[0]"]);
        spriggitFields["Unknown2[1]"].ShouldBe(dtoFields["Unknown2[1]"]);
        spriggitFields["Unknown2[2]"].ShouldBe(dtoFields["Unknown2[2]"]);
        spriggitFields["Unknown2[3]"].ShouldBe(dtoFields["Unknown2[3]"]);
        spriggitFields["Unknown2[4]"].ShouldBe(dtoFields["Unknown2[4]"]);
        spriggitFields["Unknown2[5]"].ShouldBe(dtoFields["Unknown2[5]"]);
        spriggitFields["Unknown2[6]"].ShouldBe(dtoFields["Unknown2[6]"]);
        spriggitFields["Unknown2[7]"].ShouldBe(dtoFields["Unknown2[7]"]);
        spriggitFields["Unknown2[8]"].ShouldBe(dtoFields["Unknown2[8]"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }
}
