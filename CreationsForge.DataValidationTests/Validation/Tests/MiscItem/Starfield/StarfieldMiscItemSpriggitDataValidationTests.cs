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

        Helpers.GetSpriggitField(spriggit, "Count").ShouldBe(Helpers.GetDTOField(dto, "Count"));
        Helpers.GetSpriggitField(spriggit, "CraftingSound.Start").ShouldBe(Helpers.GetDTOField(dto, "CraftingSound.Start"));
        Helpers.GetSpriggitField(spriggit, "DropdownSound.Start").ShouldBe(Helpers.GetDTOField(dto, "DropdownSound.Start"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FLAG").ShouldBe(Helpers.GetDTOField(dto, "FLAG"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "Model.Count").ShouldBe(Helpers.GetDTOField(dto, "Model.Count"));
        Helpers.GetSpriggitField(spriggit, "Model.File").ShouldBe(Helpers.GetDTOField(dto, "Models[0].File"));
        Helpers.GetSpriggitField(spriggit, "Model.LightLayer").ShouldBe(Helpers.GetDTOField(dto, "Models[0].LightLayer"));
        Helpers.GetSpriggitField(spriggit, "Model[0].127A9B").ShouldBe(Helpers.GetDTOField(dto, "Model[0].127A9B"));
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
        Helpers.GetSpriggitField(spriggit, "PickupSound.Start").ShouldBe(Helpers.GetDTOField(dto, "PickupSound.Start"));
        Helpers.GetSpriggitField(spriggit, "ShortName.Count").ShouldBe(Helpers.GetDTOField(dto, "ShortName.Count"));
        Helpers.GetSpriggitField(spriggit, "ShortName.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "ShortName.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "ShortName[0].Language").ShouldBe(Helpers.GetDTOField(dto, "ShortName[0].Language"));
        Helpers.GetSpriggitField(spriggit, "ShortName[0].String").ShouldBe(Helpers.GetDTOField(dto, "ShortName[0].String"));
        Helpers.GetSpriggitField(spriggit, "ShortName[1].Language").ShouldBe(Helpers.GetDTOField(dto, "ShortName[1].Language"));
        Helpers.GetSpriggitField(spriggit, "ShortName[1].String").ShouldBe(Helpers.GetDTOField(dto, "ShortName[1].String"));
        Helpers.GetSpriggitField(spriggit, "ShortName[2].Language").ShouldBe(Helpers.GetDTOField(dto, "ShortName[2].Language"));
        Helpers.GetSpriggitField(spriggit, "ShortName[2].String").ShouldBe(Helpers.GetDTOField(dto, "ShortName[2].String"));
        Helpers.GetSpriggitField(spriggit, "ShortName[3].Language").ShouldBe(Helpers.GetDTOField(dto, "ShortName[3].Language"));
        Helpers.GetSpriggitField(spriggit, "ShortName[3].String").ShouldBe(Helpers.GetDTOField(dto, "ShortName[3].String"));
        Helpers.GetSpriggitField(spriggit, "ShortName[4].Language").ShouldBe(Helpers.GetDTOField(dto, "ShortName[4].Language"));
        Helpers.GetSpriggitField(spriggit, "ShortName[4].String").ShouldBe(Helpers.GetDTOField(dto, "ShortName[4].String"));
        Helpers.GetSpriggitField(spriggit, "ShortName[5].Language").ShouldBe(Helpers.GetDTOField(dto, "ShortName[5].Language"));
        Helpers.GetSpriggitField(spriggit, "ShortName[5].String").ShouldBe(Helpers.GetDTOField(dto, "ShortName[5].String"));
        Helpers.GetSpriggitField(spriggit, "ShortName[6].Language").ShouldBe(Helpers.GetDTOField(dto, "ShortName[6].Language"));
        Helpers.GetSpriggitField(spriggit, "ShortName[6].String").ShouldBe(Helpers.GetDTOField(dto, "ShortName[6].String"));
        Helpers.GetSpriggitField(spriggit, "ShortName[7].Language").ShouldBe(Helpers.GetDTOField(dto, "ShortName[7].Language"));
        Helpers.GetSpriggitField(spriggit, "ShortName[7].String").ShouldBe(Helpers.GetDTOField(dto, "ShortName[7].String"));
        Helpers.GetSpriggitField(spriggit, "ShortName[8].Language").ShouldBe(Helpers.GetDTOField(dto, "ShortName[8].Language"));
        Helpers.GetSpriggitField(spriggit, "ShortName[8].String").ShouldBe(Helpers.GetDTOField(dto, "ShortName[8].String"));
        Helpers.GetSpriggitField(spriggit, "Transforms.Inventory").ShouldBe(Helpers.GetDTOField(dto, "Transforms.Inventory"));
        Helpers.GetSpriggitField(spriggit, "Value").ShouldBe(Helpers.GetDTOField(dto, "Value"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));
        Helpers.GetSpriggitField(spriggit, "Weight").ShouldBe(Helpers.GetDTOField(dto, "Weight"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "Count", "CraftingSound.Start", "DropdownSound.Start", "EditorID", "FLAG", "FormKey", "FormVersion", "Model.Count", "Model.File", "Model.LightLayer", "Model[0].127A9B", "Model[1]", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "ObjectBounds.First", "ObjectBounds.Second", "PickupSound.Start", "ShortName.Count", "ShortName.TargetLanguage", "ShortName[0].Language", "ShortName[0].String", "ShortName[1].Language", "ShortName[1].String", "ShortName[2].Language", "ShortName[2].String", "ShortName[3].Language", "ShortName[3].String", "ShortName[4].Language", "ShortName[4].String", "ShortName[5].Language", "ShortName[5].String", "ShortName[6].Language", "ShortName[6].String", "ShortName[7].Language", "ShortName[7].String", "ShortName[8].Language", "ShortName[8].String", "Transforms.Inventory", "Value", "Version2", "VersionControl", "Weight");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "Count", "CraftingSound.Start", "DropdownSound.Start", "EditorID", "FLAG", "FormKey", "FormVersion", "Model.Count", "Models[0].File", "Models[0].LightLayer", "Model[0].127A9B", "Model[1]", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "ObjectBoundsFirst", "ObjectBoundsSecond", "PickupSound.Start", "ShortName.Count", "ShortName.TargetLanguage", "ShortName[0].Language", "ShortName[0].String", "ShortName[1].Language", "ShortName[1].String", "ShortName[2].Language", "ShortName[2].String", "ShortName[3].Language", "ShortName[3].String", "ShortName[4].Language", "ShortName[4].String", "ShortName[5].Language", "ShortName[5].String", "ShortName[6].Language", "ShortName[6].String", "ShortName[7].Language", "ShortName[7].String", "ShortName[8].Language", "ShortName[8].String", "Transforms.Inventory", "Value", "Version2", "VersionControl", "Weight");
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

        Helpers.GetSpriggitField(spriggit, "Count").ShouldBe(Helpers.GetDTOField(dto, "Count"));
        Helpers.GetSpriggitField(spriggit, "CraftingSound.Start").ShouldBe(Helpers.GetDTOField(dto, "CraftingSound.Start"));
        Helpers.GetSpriggitField(spriggit, "DropdownSound.Start").ShouldBe(Helpers.GetDTOField(dto, "DropdownSound.Start"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FLAG").ShouldBe(Helpers.GetDTOField(dto, "FLAG"));
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
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.First").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsFirst"));
        Helpers.GetSpriggitField(spriggit, "ObjectBounds.Second").ShouldBe(Helpers.GetDTOField(dto, "ObjectBoundsSecond"));
        Helpers.GetSpriggitField(spriggit, "PickupSound.Start").ShouldBe(Helpers.GetDTOField(dto, "PickupSound.Start"));
        Helpers.GetSpriggitField(spriggit, "ShortName.Count").ShouldBe(Helpers.GetDTOField(dto, "ShortName.Count"));
        Helpers.GetSpriggitField(spriggit, "ShortName.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "ShortName.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "ShortName[0].Language").ShouldBe(Helpers.GetDTOField(dto, "ShortName[0].Language"));
        Helpers.GetSpriggitField(spriggit, "ShortName[0].String").ShouldBe(Helpers.GetDTOField(dto, "ShortName[0].String"));
        Helpers.GetSpriggitField(spriggit, "ShortName[1].Language").ShouldBe(Helpers.GetDTOField(dto, "ShortName[1].Language"));
        Helpers.GetSpriggitField(spriggit, "ShortName[1].String").ShouldBe(Helpers.GetDTOField(dto, "ShortName[1].String"));
        Helpers.GetSpriggitField(spriggit, "ShortName[2].Language").ShouldBe(Helpers.GetDTOField(dto, "ShortName[2].Language"));
        Helpers.GetSpriggitField(spriggit, "ShortName[2].String").ShouldBe(Helpers.GetDTOField(dto, "ShortName[2].String"));
        Helpers.GetSpriggitField(spriggit, "ShortName[3].Language").ShouldBe(Helpers.GetDTOField(dto, "ShortName[3].Language"));
        Helpers.GetSpriggitField(spriggit, "ShortName[3].String").ShouldBe(Helpers.GetDTOField(dto, "ShortName[3].String"));
        Helpers.GetSpriggitField(spriggit, "ShortName[4].Language").ShouldBe(Helpers.GetDTOField(dto, "ShortName[4].Language"));
        Helpers.GetSpriggitField(spriggit, "ShortName[4].String").ShouldBe(Helpers.GetDTOField(dto, "ShortName[4].String"));
        Helpers.GetSpriggitField(spriggit, "ShortName[5].Language").ShouldBe(Helpers.GetDTOField(dto, "ShortName[5].Language"));
        Helpers.GetSpriggitField(spriggit, "ShortName[5].String").ShouldBe(Helpers.GetDTOField(dto, "ShortName[5].String"));
        Helpers.GetSpriggitField(spriggit, "ShortName[6].Language").ShouldBe(Helpers.GetDTOField(dto, "ShortName[6].Language"));
        Helpers.GetSpriggitField(spriggit, "ShortName[6].String").ShouldBe(Helpers.GetDTOField(dto, "ShortName[6].String"));
        Helpers.GetSpriggitField(spriggit, "ShortName[7].Language").ShouldBe(Helpers.GetDTOField(dto, "ShortName[7].Language"));
        Helpers.GetSpriggitField(spriggit, "ShortName[7].String").ShouldBe(Helpers.GetDTOField(dto, "ShortName[7].String"));
        Helpers.GetSpriggitField(spriggit, "ShortName[8].Language").ShouldBe(Helpers.GetDTOField(dto, "ShortName[8].Language"));
        Helpers.GetSpriggitField(spriggit, "ShortName[8].String").ShouldBe(Helpers.GetDTOField(dto, "ShortName[8].String"));
        Helpers.GetSpriggitField(spriggit, "Transforms.Inventory").ShouldBe(Helpers.GetDTOField(dto, "Transforms.Inventory"));
        Helpers.GetSpriggitField(spriggit, "Value").ShouldBe(Helpers.GetDTOField(dto, "Value"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));
        Helpers.GetSpriggitField(spriggit, "Weight").ShouldBe(Helpers.GetDTOField(dto, "Weight"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "Count", "CraftingSound.Start", "DropdownSound.Start", "EditorID", "FLAG", "FormKey", "FormVersion", "Model.File", "Model.LightLayer", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "ObjectBounds.First", "ObjectBounds.Second", "PickupSound.Start", "ShortName.Count", "ShortName.TargetLanguage", "ShortName[0].Language", "ShortName[0].String", "ShortName[1].Language", "ShortName[1].String", "ShortName[2].Language", "ShortName[2].String", "ShortName[3].Language", "ShortName[3].String", "ShortName[4].Language", "ShortName[4].String", "ShortName[5].Language", "ShortName[5].String", "ShortName[6].Language", "ShortName[6].String", "ShortName[7].Language", "ShortName[7].String", "ShortName[8].Language", "ShortName[8].String", "Transforms.Inventory", "Value", "Version2", "VersionControl", "Weight");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "Count", "CraftingSound.Start", "DropdownSound.Start", "EditorID", "FLAG", "FormKey", "FormVersion", "Models[0].File", "Models[0].LightLayer", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "ObjectBoundsFirst", "ObjectBoundsSecond", "PickupSound.Start", "ShortName.Count", "ShortName.TargetLanguage", "ShortName[0].Language", "ShortName[0].String", "ShortName[1].Language", "ShortName[1].String", "ShortName[2].Language", "ShortName[2].String", "ShortName[3].Language", "ShortName[3].String", "ShortName[4].Language", "ShortName[4].String", "ShortName[5].Language", "ShortName[5].String", "ShortName[6].Language", "ShortName[6].String", "ShortName[7].Language", "ShortName[7].String", "ShortName[8].Language", "ShortName[8].String", "Transforms.Inventory", "Value", "Version2", "VersionControl", "Weight");
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

        Helpers.GetSpriggitField(spriggit, "Count").ShouldBe(Helpers.GetDTOField(dto, "Count"));
        Helpers.GetSpriggitField(spriggit, "CraftingSound.Start").ShouldBe(Helpers.GetDTOField(dto, "CraftingSound.Start"));
        Helpers.GetSpriggitField(spriggit, "DropdownSound.Start").ShouldBe(Helpers.GetDTOField(dto, "DropdownSound.Start"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FLAG").ShouldBe(Helpers.GetDTOField(dto, "FLAG"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "Model.Count").ShouldBe(Helpers.GetDTOField(dto, "Model.Count"));
        Helpers.GetSpriggitField(spriggit, "Model.File").ShouldBe(Helpers.GetDTOField(dto, "Models[0].File"));
        Helpers.GetSpriggitField(spriggit, "Model.LightLayer").ShouldBe(Helpers.GetDTOField(dto, "Models[0].LightLayer"));
        Helpers.GetSpriggitField(spriggit, "Model[0].127AA0").ShouldBe(Helpers.GetDTOField(dto, "Model[0].127AA0"));
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
        Helpers.GetSpriggitField(spriggit, "PickupSound.Start").ShouldBe(Helpers.GetDTOField(dto, "PickupSound.Start"));
        Helpers.GetSpriggitField(spriggit, "ShortName.Count").ShouldBe(Helpers.GetDTOField(dto, "ShortName.Count"));
        Helpers.GetSpriggitField(spriggit, "ShortName.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "ShortName.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "ShortName[0].Language").ShouldBe(Helpers.GetDTOField(dto, "ShortName[0].Language"));
        Helpers.GetSpriggitField(spriggit, "ShortName[0].String").ShouldBe(Helpers.GetDTOField(dto, "ShortName[0].String"));
        Helpers.GetSpriggitField(spriggit, "ShortName[1].Language").ShouldBe(Helpers.GetDTOField(dto, "ShortName[1].Language"));
        Helpers.GetSpriggitField(spriggit, "ShortName[1].String").ShouldBe(Helpers.GetDTOField(dto, "ShortName[1].String"));
        Helpers.GetSpriggitField(spriggit, "ShortName[2].Language").ShouldBe(Helpers.GetDTOField(dto, "ShortName[2].Language"));
        Helpers.GetSpriggitField(spriggit, "ShortName[2].String").ShouldBe(Helpers.GetDTOField(dto, "ShortName[2].String"));
        Helpers.GetSpriggitField(spriggit, "ShortName[3].Language").ShouldBe(Helpers.GetDTOField(dto, "ShortName[3].Language"));
        Helpers.GetSpriggitField(spriggit, "ShortName[3].String").ShouldBe(Helpers.GetDTOField(dto, "ShortName[3].String"));
        Helpers.GetSpriggitField(spriggit, "ShortName[4].Language").ShouldBe(Helpers.GetDTOField(dto, "ShortName[4].Language"));
        Helpers.GetSpriggitField(spriggit, "ShortName[4].String").ShouldBe(Helpers.GetDTOField(dto, "ShortName[4].String"));
        Helpers.GetSpriggitField(spriggit, "ShortName[5].Language").ShouldBe(Helpers.GetDTOField(dto, "ShortName[5].Language"));
        Helpers.GetSpriggitField(spriggit, "ShortName[5].String").ShouldBe(Helpers.GetDTOField(dto, "ShortName[5].String"));
        Helpers.GetSpriggitField(spriggit, "ShortName[6].Language").ShouldBe(Helpers.GetDTOField(dto, "ShortName[6].Language"));
        Helpers.GetSpriggitField(spriggit, "ShortName[6].String").ShouldBe(Helpers.GetDTOField(dto, "ShortName[6].String"));
        Helpers.GetSpriggitField(spriggit, "ShortName[7].Language").ShouldBe(Helpers.GetDTOField(dto, "ShortName[7].Language"));
        Helpers.GetSpriggitField(spriggit, "ShortName[7].String").ShouldBe(Helpers.GetDTOField(dto, "ShortName[7].String"));
        Helpers.GetSpriggitField(spriggit, "ShortName[8].Language").ShouldBe(Helpers.GetDTOField(dto, "ShortName[8].Language"));
        Helpers.GetSpriggitField(spriggit, "ShortName[8].String").ShouldBe(Helpers.GetDTOField(dto, "ShortName[8].String"));
        Helpers.GetSpriggitField(spriggit, "Transforms.Inventory").ShouldBe(Helpers.GetDTOField(dto, "Transforms.Inventory"));
        Helpers.GetSpriggitField(spriggit, "Value").ShouldBe(Helpers.GetDTOField(dto, "Value"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));
        Helpers.GetSpriggitField(spriggit, "Weight").ShouldBe(Helpers.GetDTOField(dto, "Weight"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "Count", "CraftingSound.Start", "DropdownSound.Start", "EditorID", "FLAG", "FormKey", "FormVersion", "Model.Count", "Model.File", "Model.LightLayer", "Model[0].127AA0", "Model[1]", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "ObjectBounds.First", "ObjectBounds.Second", "PickupSound.Start", "ShortName.Count", "ShortName.TargetLanguage", "ShortName[0].Language", "ShortName[0].String", "ShortName[1].Language", "ShortName[1].String", "ShortName[2].Language", "ShortName[2].String", "ShortName[3].Language", "ShortName[3].String", "ShortName[4].Language", "ShortName[4].String", "ShortName[5].Language", "ShortName[5].String", "ShortName[6].Language", "ShortName[6].String", "ShortName[7].Language", "ShortName[7].String", "ShortName[8].Language", "ShortName[8].String", "Transforms.Inventory", "Value", "Version2", "VersionControl", "Weight");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "Count", "CraftingSound.Start", "DropdownSound.Start", "EditorID", "FLAG", "FormKey", "FormVersion", "Model.Count", "Models[0].File", "Models[0].LightLayer", "Model[0].127AA0", "Model[1]", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "ObjectBoundsFirst", "ObjectBoundsSecond", "PickupSound.Start", "ShortName.Count", "ShortName.TargetLanguage", "ShortName[0].Language", "ShortName[0].String", "ShortName[1].Language", "ShortName[1].String", "ShortName[2].Language", "ShortName[2].String", "ShortName[3].Language", "ShortName[3].String", "ShortName[4].Language", "ShortName[4].String", "ShortName[5].Language", "ShortName[5].String", "ShortName[6].Language", "ShortName[6].String", "ShortName[7].Language", "ShortName[7].String", "ShortName[8].Language", "ShortName[8].String", "Transforms.Inventory", "Value", "Version2", "VersionControl", "Weight");
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

        Helpers.GetSpriggitField(spriggit, "CraftingSound.Start").ShouldBe(Helpers.GetDTOField(dto, "CraftingSound.Start"));
        Helpers.GetSpriggitField(spriggit, "DropdownSound.Start").ShouldBe(Helpers.GetDTOField(dto, "DropdownSound.Start"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
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
        Helpers.GetSpriggitField(spriggit, "PickupSound.Start").ShouldBe(Helpers.GetDTOField(dto, "PickupSound.Start"));
        Helpers.GetSpriggitField(spriggit, "ShortName.Count").ShouldBe(Helpers.GetDTOField(dto, "ShortName.Count"));
        Helpers.GetSpriggitField(spriggit, "ShortName.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "ShortName.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "ShortName[0].Language").ShouldBe(Helpers.GetDTOField(dto, "ShortName[0].Language"));
        Helpers.GetSpriggitField(spriggit, "ShortName[0].String").ShouldBe(Helpers.GetDTOField(dto, "ShortName[0].String"));
        Helpers.GetSpriggitField(spriggit, "ShortName[1].Language").ShouldBe(Helpers.GetDTOField(dto, "ShortName[1].Language"));
        Helpers.GetSpriggitField(spriggit, "ShortName[1].String").ShouldBe(Helpers.GetDTOField(dto, "ShortName[1].String"));
        Helpers.GetSpriggitField(spriggit, "ShortName[2].Language").ShouldBe(Helpers.GetDTOField(dto, "ShortName[2].Language"));
        Helpers.GetSpriggitField(spriggit, "ShortName[2].String").ShouldBe(Helpers.GetDTOField(dto, "ShortName[2].String"));
        Helpers.GetSpriggitField(spriggit, "ShortName[3].Language").ShouldBe(Helpers.GetDTOField(dto, "ShortName[3].Language"));
        Helpers.GetSpriggitField(spriggit, "ShortName[3].String").ShouldBe(Helpers.GetDTOField(dto, "ShortName[3].String"));
        Helpers.GetSpriggitField(spriggit, "ShortName[4].Language").ShouldBe(Helpers.GetDTOField(dto, "ShortName[4].Language"));
        Helpers.GetSpriggitField(spriggit, "ShortName[4].String").ShouldBe(Helpers.GetDTOField(dto, "ShortName[4].String"));
        Helpers.GetSpriggitField(spriggit, "ShortName[5].Language").ShouldBe(Helpers.GetDTOField(dto, "ShortName[5].Language"));
        Helpers.GetSpriggitField(spriggit, "ShortName[5].String").ShouldBe(Helpers.GetDTOField(dto, "ShortName[5].String"));
        Helpers.GetSpriggitField(spriggit, "ShortName[6].Language").ShouldBe(Helpers.GetDTOField(dto, "ShortName[6].Language"));
        Helpers.GetSpriggitField(spriggit, "ShortName[6].String").ShouldBe(Helpers.GetDTOField(dto, "ShortName[6].String"));
        Helpers.GetSpriggitField(spriggit, "ShortName[7].Language").ShouldBe(Helpers.GetDTOField(dto, "ShortName[7].Language"));
        Helpers.GetSpriggitField(spriggit, "ShortName[7].String").ShouldBe(Helpers.GetDTOField(dto, "ShortName[7].String"));
        Helpers.GetSpriggitField(spriggit, "ShortName[8].Language").ShouldBe(Helpers.GetDTOField(dto, "ShortName[8].Language"));
        Helpers.GetSpriggitField(spriggit, "ShortName[8].String").ShouldBe(Helpers.GetDTOField(dto, "ShortName[8].String"));
        Helpers.GetSpriggitField(spriggit, "Transforms.Inventory").ShouldBe(Helpers.GetDTOField(dto, "Transforms.Inventory"));
        Helpers.GetSpriggitField(spriggit, "Value").ShouldBe(Helpers.GetDTOField(dto, "Value"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));
        Helpers.GetSpriggitField(spriggit, "Weight").ShouldBe(Helpers.GetDTOField(dto, "Weight"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "CraftingSound.Start", "DropdownSound.Start", "EditorID", "FormKey", "FormVersion", "Model.File", "Model.LightLayer", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "PickupSound.Start", "ShortName.Count", "ShortName.TargetLanguage", "ShortName[0].Language", "ShortName[0].String", "ShortName[1].Language", "ShortName[1].String", "ShortName[2].Language", "ShortName[2].String", "ShortName[3].Language", "ShortName[3].String", "ShortName[4].Language", "ShortName[4].String", "ShortName[5].Language", "ShortName[5].String", "ShortName[6].Language", "ShortName[6].String", "ShortName[7].Language", "ShortName[7].String", "ShortName[8].Language", "ShortName[8].String", "Transforms.Inventory", "Value", "Version2", "VersionControl", "Weight");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "CraftingSound.Start", "DropdownSound.Start", "EditorID", "FormKey", "FormVersion", "Models[0].File", "Models[0].LightLayer", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "PickupSound.Start", "ShortName.Count", "ShortName.TargetLanguage", "ShortName[0].Language", "ShortName[0].String", "ShortName[1].Language", "ShortName[1].String", "ShortName[2].Language", "ShortName[2].String", "ShortName[3].Language", "ShortName[3].String", "ShortName[4].Language", "ShortName[4].String", "ShortName[5].Language", "ShortName[5].String", "ShortName[6].Language", "ShortName[6].String", "ShortName[7].Language", "ShortName[7].String", "ShortName[8].Language", "ShortName[8].String", "Transforms.Inventory", "Value", "Version2", "VersionControl", "Weight");
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

        Helpers.GetSpriggitField(spriggit, "DropdownSound.Start").ShouldBe(Helpers.GetDTOField(dto, "DropdownSound.Start"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "Model.Count").ShouldBe(Helpers.GetDTOField(dto, "Model.Count"));
        Helpers.GetSpriggitField(spriggit, "Model.File").ShouldBe(Helpers.GetDTOField(dto, "Models[0].File"));
        Helpers.GetSpriggitField(spriggit, "Model.LightLayer").ShouldBe(Helpers.GetDTOField(dto, "Models[0].LightLayer"));
        Helpers.GetSpriggitField(spriggit, "Model[0].103CFB").ShouldBe(Helpers.GetDTOField(dto, "Model[0].103CFB"));
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
        Helpers.GetSpriggitField(spriggit, "PickupSound.Start").ShouldBe(Helpers.GetDTOField(dto, "PickupSound.Start"));
        Helpers.GetSpriggitField(spriggit, "Transforms.Inventory").ShouldBe(Helpers.GetDTOField(dto, "Transforms.Inventory"));
        Helpers.GetSpriggitField(spriggit, "Value").ShouldBe(Helpers.GetDTOField(dto, "Value"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));
        Helpers.GetSpriggitField(spriggit, "Weight").ShouldBe(Helpers.GetDTOField(dto, "Weight"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "DropdownSound.Start", "EditorID", "FormKey", "FormVersion", "Model.Count", "Model.File", "Model.LightLayer", "Model[0].103CFB", "Model[1]", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "ObjectBounds.First", "ObjectBounds.Second", "PickupSound.Start", "Transforms.Inventory", "Value", "Version2", "VersionControl", "Weight");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "DropdownSound.Start", "EditorID", "FormKey", "FormVersion", "Model.Count", "Models[0].File", "Models[0].LightLayer", "Model[0].103CFB", "Model[1]", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "ObjectBoundsFirst", "ObjectBoundsSecond", "PickupSound.Start", "Transforms.Inventory", "Value", "Version2", "VersionControl", "Weight");
    }
}