using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.MiscItem.Starfield;

public class StarfieldMiscItemSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "MISC")]
    [Trait("FormKey", "005591:Starfield.esm")]
    [Trait("EditorID", "InorgCommonWater")]
    [Trait("SpriggitFile", "MiscItems/InorgCommonWater - 005591_Starfield.esm.yaml")]
    public void Starfield_MISC_ShouldMatchSpriggitSample_InorgCommonWater()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.MiscObject,
            "InorgCommonWater");
        var dto = Helpers.GetDTO<MiscObjectDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.MiscObject,
            "005591:Starfield.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["Count"].ShouldBe(dtoFields["Count"]);
        spriggitFields["CraftingSound.Start"].ShouldBe(dtoFields["CraftingSound.Start"]);
        spriggitFields["DropdownSound.Start"].ShouldBe(dtoFields["DropdownSound.Start"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FLAG"].ShouldBe(dtoFields["FLAG"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["Model.Count"].ShouldBe(dtoFields["Model.Count"]);
        spriggitFields["Model.File"].ShouldBe(dtoFields["Models[0].File"]);
        spriggitFields["Model.LightLayer"].ShouldBe(dtoFields["Models[0].LightLayer"]);
        spriggitFields["Model[0].127A9B"].ShouldBe(dtoFields["Model[0].127A9B"]);
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
        spriggitFields["PickupSound.Start"].ShouldBe(dtoFields["PickupSound.Start"]);
        spriggitFields["ShortName.Count"].ShouldBe(dtoFields["ShortName.Count"]);
        spriggitFields["ShortName.TargetLanguage"].ShouldBe(dtoFields["ShortName.TargetLanguage"]);
        spriggitFields["ShortName[0].Language"].ShouldBe(dtoFields["ShortName[0].Language"]);
        spriggitFields["ShortName[0].String"].ShouldBe(dtoFields["ShortName[0].String"]);
        spriggitFields["ShortName[1].Language"].ShouldBe(dtoFields["ShortName[1].Language"]);
        spriggitFields["ShortName[1].String"].ShouldBe(dtoFields["ShortName[1].String"]);
        spriggitFields["ShortName[2].Language"].ShouldBe(dtoFields["ShortName[2].Language"]);
        spriggitFields["ShortName[2].String"].ShouldBe(dtoFields["ShortName[2].String"]);
        spriggitFields["ShortName[3].Language"].ShouldBe(dtoFields["ShortName[3].Language"]);
        spriggitFields["ShortName[3].String"].ShouldBe(dtoFields["ShortName[3].String"]);
        spriggitFields["ShortName[4].Language"].ShouldBe(dtoFields["ShortName[4].Language"]);
        spriggitFields["ShortName[4].String"].ShouldBe(dtoFields["ShortName[4].String"]);
        spriggitFields["ShortName[5].Language"].ShouldBe(dtoFields["ShortName[5].Language"]);
        spriggitFields["ShortName[5].String"].ShouldBe(dtoFields["ShortName[5].String"]);
        spriggitFields["ShortName[6].Language"].ShouldBe(dtoFields["ShortName[6].Language"]);
        spriggitFields["ShortName[6].String"].ShouldBe(dtoFields["ShortName[6].String"]);
        spriggitFields["ShortName[7].Language"].ShouldBe(dtoFields["ShortName[7].Language"]);
        spriggitFields["ShortName[7].String"].ShouldBe(dtoFields["ShortName[7].String"]);
        spriggitFields["ShortName[8].Language"].ShouldBe(dtoFields["ShortName[8].Language"]);
        spriggitFields["ShortName[8].String"].ShouldBe(dtoFields["ShortName[8].String"]);
        spriggitFields["Transforms.Inventory"].ShouldBe(dtoFields["Transforms.Inventory"]);
        spriggitFields["Value"].ShouldBe(dtoFields["Value"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["Weight"].ShouldBe(dtoFields["Weight"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "MISC")]
    [Trait("FormKey", "00558C:Starfield.esm")]
    [Trait("EditorID", "InorgExoticPlutonium")]
    [Trait("SpriggitFile", "MiscItems/InorgExoticPlutonium - 00558C_Starfield.esm.yaml")]
    public void Starfield_MISC_ShouldMatchSpriggitSample_InorgExoticPlutonium()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.MiscObject,
            "InorgExoticPlutonium");
        var dto = Helpers.GetDTO<MiscObjectDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.MiscObject,
            "00558C:Starfield.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["Count"].ShouldBe(dtoFields["Count"]);
        spriggitFields["CraftingSound.Start"].ShouldBe(dtoFields["CraftingSound.Start"]);
        spriggitFields["DropdownSound.Start"].ShouldBe(dtoFields["DropdownSound.Start"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FLAG"].ShouldBe(dtoFields["FLAG"]);
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
        spriggitFields["ObjectBounds.First"].ShouldBe(dtoFields["ObjectBoundsFirst"]);
        spriggitFields["ObjectBounds.Second"].ShouldBe(dtoFields["ObjectBoundsSecond"]);
        spriggitFields["PickupSound.Start"].ShouldBe(dtoFields["PickupSound.Start"]);
        spriggitFields["ShortName.Count"].ShouldBe(dtoFields["ShortName.Count"]);
        spriggitFields["ShortName.TargetLanguage"].ShouldBe(dtoFields["ShortName.TargetLanguage"]);
        spriggitFields["ShortName[0].Language"].ShouldBe(dtoFields["ShortName[0].Language"]);
        spriggitFields["ShortName[0].String"].ShouldBe(dtoFields["ShortName[0].String"]);
        spriggitFields["ShortName[1].Language"].ShouldBe(dtoFields["ShortName[1].Language"]);
        spriggitFields["ShortName[1].String"].ShouldBe(dtoFields["ShortName[1].String"]);
        spriggitFields["ShortName[2].Language"].ShouldBe(dtoFields["ShortName[2].Language"]);
        spriggitFields["ShortName[2].String"].ShouldBe(dtoFields["ShortName[2].String"]);
        spriggitFields["ShortName[3].Language"].ShouldBe(dtoFields["ShortName[3].Language"]);
        spriggitFields["ShortName[3].String"].ShouldBe(dtoFields["ShortName[3].String"]);
        spriggitFields["ShortName[4].Language"].ShouldBe(dtoFields["ShortName[4].Language"]);
        spriggitFields["ShortName[4].String"].ShouldBe(dtoFields["ShortName[4].String"]);
        spriggitFields["ShortName[5].Language"].ShouldBe(dtoFields["ShortName[5].Language"]);
        spriggitFields["ShortName[5].String"].ShouldBe(dtoFields["ShortName[5].String"]);
        spriggitFields["ShortName[6].Language"].ShouldBe(dtoFields["ShortName[6].Language"]);
        spriggitFields["ShortName[6].String"].ShouldBe(dtoFields["ShortName[6].String"]);
        spriggitFields["ShortName[7].Language"].ShouldBe(dtoFields["ShortName[7].Language"]);
        spriggitFields["ShortName[7].String"].ShouldBe(dtoFields["ShortName[7].String"]);
        spriggitFields["ShortName[8].Language"].ShouldBe(dtoFields["ShortName[8].Language"]);
        spriggitFields["ShortName[8].String"].ShouldBe(dtoFields["ShortName[8].String"]);
        spriggitFields["Transforms.Inventory"].ShouldBe(dtoFields["Transforms.Inventory"]);
        spriggitFields["Value"].ShouldBe(dtoFields["Value"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["Weight"].ShouldBe(dtoFields["Weight"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "MISC")]
    [Trait("FormKey", "005DED:Starfield.esm")]
    [Trait("EditorID", "InorgUniqueTasine")]
    [Trait("SpriggitFile", "MiscItems/InorgUniqueTasine - 005DED_Starfield.esm.yaml")]
    public void Starfield_MISC_ShouldMatchSpriggitSample_InorgUniqueTasine()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.MiscObject,
            "InorgUniqueTasine");
        var dto = Helpers.GetDTO<MiscObjectDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.MiscObject,
            "005DED:Starfield.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["Count"].ShouldBe(dtoFields["Count"]);
        spriggitFields["CraftingSound.Start"].ShouldBe(dtoFields["CraftingSound.Start"]);
        spriggitFields["DropdownSound.Start"].ShouldBe(dtoFields["DropdownSound.Start"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FLAG"].ShouldBe(dtoFields["FLAG"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["Model.Count"].ShouldBe(dtoFields["Model.Count"]);
        spriggitFields["Model.File"].ShouldBe(dtoFields["Models[0].File"]);
        spriggitFields["Model.LightLayer"].ShouldBe(dtoFields["Models[0].LightLayer"]);
        spriggitFields["Model[0].127AA0"].ShouldBe(dtoFields["Model[0].127AA0"]);
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
        spriggitFields["PickupSound.Start"].ShouldBe(dtoFields["PickupSound.Start"]);
        spriggitFields["ShortName.Count"].ShouldBe(dtoFields["ShortName.Count"]);
        spriggitFields["ShortName.TargetLanguage"].ShouldBe(dtoFields["ShortName.TargetLanguage"]);
        spriggitFields["ShortName[0].Language"].ShouldBe(dtoFields["ShortName[0].Language"]);
        spriggitFields["ShortName[0].String"].ShouldBe(dtoFields["ShortName[0].String"]);
        spriggitFields["ShortName[1].Language"].ShouldBe(dtoFields["ShortName[1].Language"]);
        spriggitFields["ShortName[1].String"].ShouldBe(dtoFields["ShortName[1].String"]);
        spriggitFields["ShortName[2].Language"].ShouldBe(dtoFields["ShortName[2].Language"]);
        spriggitFields["ShortName[2].String"].ShouldBe(dtoFields["ShortName[2].String"]);
        spriggitFields["ShortName[3].Language"].ShouldBe(dtoFields["ShortName[3].Language"]);
        spriggitFields["ShortName[3].String"].ShouldBe(dtoFields["ShortName[3].String"]);
        spriggitFields["ShortName[4].Language"].ShouldBe(dtoFields["ShortName[4].Language"]);
        spriggitFields["ShortName[4].String"].ShouldBe(dtoFields["ShortName[4].String"]);
        spriggitFields["ShortName[5].Language"].ShouldBe(dtoFields["ShortName[5].Language"]);
        spriggitFields["ShortName[5].String"].ShouldBe(dtoFields["ShortName[5].String"]);
        spriggitFields["ShortName[6].Language"].ShouldBe(dtoFields["ShortName[6].Language"]);
        spriggitFields["ShortName[6].String"].ShouldBe(dtoFields["ShortName[6].String"]);
        spriggitFields["ShortName[7].Language"].ShouldBe(dtoFields["ShortName[7].Language"]);
        spriggitFields["ShortName[7].String"].ShouldBe(dtoFields["ShortName[7].String"]);
        spriggitFields["ShortName[8].Language"].ShouldBe(dtoFields["ShortName[8].Language"]);
        spriggitFields["ShortName[8].String"].ShouldBe(dtoFields["ShortName[8].String"]);
        spriggitFields["Transforms.Inventory"].ShouldBe(dtoFields["Transforms.Inventory"]);
        spriggitFields["Value"].ShouldBe(dtoFields["Value"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["Weight"].ShouldBe(dtoFields["Weight"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "MISC")]
    [Trait("FormKey", "302791:Starfield.esm")]
    [Trait("EditorID", "FFCydoniaZ07_HeartOfMarsTitanium")]
    [Trait("SpriggitFile", "MiscItems/FFCydoniaZ07_HeartOfMarsTitanium - 302791_Starfield.esm.yaml")]
    public void Starfield_MISC_ShouldMatchSpriggitSample_FFCydoniaZ07_HeartOfMarsTitanium()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.MiscObject,
            "FFCydoniaZ07_HeartOfMarsTitanium");
        var dto = Helpers.GetDTO<MiscObjectDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.MiscObject,
            "302791:Starfield.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["CraftingSound.Start"].ShouldBe(dtoFields["CraftingSound.Start"]);
        spriggitFields["DropdownSound.Start"].ShouldBe(dtoFields["DropdownSound.Start"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
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
        spriggitFields["PickupSound.Start"].ShouldBe(dtoFields["PickupSound.Start"]);
        spriggitFields["ShortName.Count"].ShouldBe(dtoFields["ShortName.Count"]);
        spriggitFields["ShortName.TargetLanguage"].ShouldBe(dtoFields["ShortName.TargetLanguage"]);
        spriggitFields["ShortName[0].Language"].ShouldBe(dtoFields["ShortName[0].Language"]);
        spriggitFields["ShortName[0].String"].ShouldBe(dtoFields["ShortName[0].String"]);
        spriggitFields["ShortName[1].Language"].ShouldBe(dtoFields["ShortName[1].Language"]);
        spriggitFields["ShortName[1].String"].ShouldBe(dtoFields["ShortName[1].String"]);
        spriggitFields["ShortName[2].Language"].ShouldBe(dtoFields["ShortName[2].Language"]);
        spriggitFields["ShortName[2].String"].ShouldBe(dtoFields["ShortName[2].String"]);
        spriggitFields["ShortName[3].Language"].ShouldBe(dtoFields["ShortName[3].Language"]);
        spriggitFields["ShortName[3].String"].ShouldBe(dtoFields["ShortName[3].String"]);
        spriggitFields["ShortName[4].Language"].ShouldBe(dtoFields["ShortName[4].Language"]);
        spriggitFields["ShortName[4].String"].ShouldBe(dtoFields["ShortName[4].String"]);
        spriggitFields["ShortName[5].Language"].ShouldBe(dtoFields["ShortName[5].Language"]);
        spriggitFields["ShortName[5].String"].ShouldBe(dtoFields["ShortName[5].String"]);
        spriggitFields["ShortName[6].Language"].ShouldBe(dtoFields["ShortName[6].Language"]);
        spriggitFields["ShortName[6].String"].ShouldBe(dtoFields["ShortName[6].String"]);
        spriggitFields["ShortName[7].Language"].ShouldBe(dtoFields["ShortName[7].Language"]);
        spriggitFields["ShortName[7].String"].ShouldBe(dtoFields["ShortName[7].String"]);
        spriggitFields["ShortName[8].Language"].ShouldBe(dtoFields["ShortName[8].Language"]);
        spriggitFields["ShortName[8].String"].ShouldBe(dtoFields["ShortName[8].String"]);
        spriggitFields["Transforms.Inventory"].ShouldBe(dtoFields["Transforms.Inventory"]);
        spriggitFields["Value"].ShouldBe(dtoFields["Value"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["Weight"].ShouldBe(dtoFields["Weight"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "MISC")]
    [Trait("FormKey", "10A797:Starfield.esm")]
    [Trait("EditorID", "ExoticPlayingCard_Diamond_Q")]
    [Trait("SpriggitFile", "MiscItems/ExoticPlayingCard_Diamond_Q - 10A797_Starfield.esm.yaml")]
    public void Starfield_MISC_ShouldMatchSpriggitSample_ExoticPlayingCard_Diamond_Q()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.MiscObject,
            "ExoticPlayingCard_Diamond_Q");
        var dto = Helpers.GetDTO<MiscObjectDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.MiscObject,
            "10A797:Starfield.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["DropdownSound.Start"].ShouldBe(dtoFields["DropdownSound.Start"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["Model.Count"].ShouldBe(dtoFields["Model.Count"]);
        spriggitFields["Model.File"].ShouldBe(dtoFields["Models[0].File"]);
        spriggitFields["Model.LightLayer"].ShouldBe(dtoFields["Models[0].LightLayer"]);
        spriggitFields["Model[0].103CFB"].ShouldBe(dtoFields["Model[0].103CFB"]);
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
        spriggitFields["PickupSound.Start"].ShouldBe(dtoFields["PickupSound.Start"]);
        spriggitFields["Transforms.Inventory"].ShouldBe(dtoFields["Transforms.Inventory"]);
        spriggitFields["Value"].ShouldBe(dtoFields["Value"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["Weight"].ShouldBe(dtoFields["Weight"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }
}
