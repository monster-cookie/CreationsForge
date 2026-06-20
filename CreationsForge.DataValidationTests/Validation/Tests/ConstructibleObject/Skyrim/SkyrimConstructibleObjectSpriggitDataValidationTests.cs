using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.ConstructibleObject.Skyrim;

public class SkyrimConstructibleObjectSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "COBJ")]
    [Trait("FormKey", "0DCA13:Skyrim.esm")]
    [Trait("EditorID", "RecipeArmorDragonscaleBoots")]
    [Trait("SpriggitFile", "ConstructibleObjects/RecipeArmorDragonscaleBoots - 0DCA13_Skyrim.esm.yaml")]
    public void Skyrim_COBJ_ShouldMatchSpriggitSample_RecipeArmorDragonscaleBoots()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.ConstructibleObject,
            "RecipeArmorDragonscaleBoots");
        var dto = Helpers.GetDTO<ConstructibleObjectDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.ConstructibleObject,
            "0DCA13:Skyrim.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["ComparisonValue"].ShouldBe(dtoFields["ComparisonValue"]);
        spriggitFields["Count[0]"].ShouldBe(dtoFields["Count[0]"]);
        spriggitFields["Count[1]"].ShouldBe(dtoFields["Count[1]"]);
        spriggitFields["Count[2]"].ShouldBe(dtoFields["Count[2]"]);
        spriggitFields["Count[3]"].ShouldBe(dtoFields["Count[3]"]);
        spriggitFields["CreatedObject"].ShouldBe(dtoFields["CreatedObjectFormKey"]);
        spriggitFields["CreatedObjectCount"].ShouldBe(dtoFields["CreatedObjectCount"]);
        spriggitFields["Data.MutagenObjectType"].ShouldBe(dtoFields["Data.MutagenObjectType"]);
        spriggitFields["Data.Perk"].ShouldBe(dtoFields["Data.Perk"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["Item[0]"].ShouldBe(dtoFields["Item[0]"]);
        spriggitFields["Item[1]"].ShouldBe(dtoFields["Item[1]"]);
        spriggitFields["Item[2]"].ShouldBe(dtoFields["Item[2]"]);
        spriggitFields["Item[3]"].ShouldBe(dtoFields["Item[3]"]);
        spriggitFields["Unknown2"].ShouldBe(dtoFields["Unknown2"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["WorkbenchKeyword"].ShouldBe(dtoFields["WorkbenchKeywordFormKey"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "COBJ")]
    [Trait("FormKey", "0DCA14:Skyrim.esm")]
    [Trait("EditorID", "RecipeArmorDragonscaleCuirass")]
    [Trait("SpriggitFile", "ConstructibleObjects/RecipeArmorDragonscaleCuirass - 0DCA14_Skyrim.esm.yaml")]
    public void Skyrim_COBJ_ShouldMatchSpriggitSample_RecipeArmorDragonscaleCuirass()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.ConstructibleObject,
            "RecipeArmorDragonscaleCuirass");
        var dto = Helpers.GetDTO<ConstructibleObjectDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.ConstructibleObject,
            "0DCA14:Skyrim.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["ComparisonValue"].ShouldBe(dtoFields["ComparisonValue"]);
        spriggitFields["Count[0]"].ShouldBe(dtoFields["Count[0]"]);
        spriggitFields["Count[1]"].ShouldBe(dtoFields["Count[1]"]);
        spriggitFields["Count[2]"].ShouldBe(dtoFields["Count[2]"]);
        spriggitFields["Count[3]"].ShouldBe(dtoFields["Count[3]"]);
        spriggitFields["CreatedObject"].ShouldBe(dtoFields["CreatedObjectFormKey"]);
        spriggitFields["CreatedObjectCount"].ShouldBe(dtoFields["CreatedObjectCount"]);
        spriggitFields["Data.MutagenObjectType"].ShouldBe(dtoFields["Data.MutagenObjectType"]);
        spriggitFields["Data.Perk"].ShouldBe(dtoFields["Data.Perk"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["Item[0]"].ShouldBe(dtoFields["Item[0]"]);
        spriggitFields["Item[1]"].ShouldBe(dtoFields["Item[1]"]);
        spriggitFields["Item[2]"].ShouldBe(dtoFields["Item[2]"]);
        spriggitFields["Item[3]"].ShouldBe(dtoFields["Item[3]"]);
        spriggitFields["Unknown2"].ShouldBe(dtoFields["Unknown2"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["WorkbenchKeyword"].ShouldBe(dtoFields["WorkbenchKeywordFormKey"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "COBJ")]
    [Trait("FormKey", "0DCA15:Skyrim.esm")]
    [Trait("EditorID", "RecipeArmorDragonscaleGauntlets")]
    [Trait("SpriggitFile", "ConstructibleObjects/RecipeArmorDragonscaleGauntlets - 0DCA15_Skyrim.esm.yaml")]
    public void Skyrim_COBJ_ShouldMatchSpriggitSample_RecipeArmorDragonscaleGauntlets()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.ConstructibleObject,
            "RecipeArmorDragonscaleGauntlets");
        var dto = Helpers.GetDTO<ConstructibleObjectDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.ConstructibleObject,
            "0DCA15:Skyrim.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["ComparisonValue"].ShouldBe(dtoFields["ComparisonValue"]);
        spriggitFields["Count[0]"].ShouldBe(dtoFields["Count[0]"]);
        spriggitFields["Count[1]"].ShouldBe(dtoFields["Count[1]"]);
        spriggitFields["Count[2]"].ShouldBe(dtoFields["Count[2]"]);
        spriggitFields["Count[3]"].ShouldBe(dtoFields["Count[3]"]);
        spriggitFields["CreatedObject"].ShouldBe(dtoFields["CreatedObjectFormKey"]);
        spriggitFields["CreatedObjectCount"].ShouldBe(dtoFields["CreatedObjectCount"]);
        spriggitFields["Data.MutagenObjectType"].ShouldBe(dtoFields["Data.MutagenObjectType"]);
        spriggitFields["Data.Perk"].ShouldBe(dtoFields["Data.Perk"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["Item[0]"].ShouldBe(dtoFields["Item[0]"]);
        spriggitFields["Item[1]"].ShouldBe(dtoFields["Item[1]"]);
        spriggitFields["Item[2]"].ShouldBe(dtoFields["Item[2]"]);
        spriggitFields["Item[3]"].ShouldBe(dtoFields["Item[3]"]);
        spriggitFields["Unknown2"].ShouldBe(dtoFields["Unknown2"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["WorkbenchKeyword"].ShouldBe(dtoFields["WorkbenchKeywordFormKey"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "COBJ")]
    [Trait("FormKey", "0DD982:Skyrim.esm")]
    [Trait("EditorID", "RecipeArmorSteelPlateShield")]
    [Trait("SpriggitFile", "ConstructibleObjects/RecipeArmorSteelPlateShield - 0DD982_Skyrim.esm.yaml")]
    public void Skyrim_COBJ_ShouldMatchSpriggitSample_RecipeArmorSteelPlateShield()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.ConstructibleObject,
            "RecipeArmorSteelPlateShield");
        var dto = Helpers.GetDTO<ConstructibleObjectDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.ConstructibleObject,
            "0DD982:Skyrim.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["ComparisonValue"].ShouldBe(dtoFields["ComparisonValue"]);
        spriggitFields["Count[0]"].ShouldBe(dtoFields["Count[0]"]);
        spriggitFields["Count[1]"].ShouldBe(dtoFields["Count[1]"]);
        spriggitFields["Count[2]"].ShouldBe(dtoFields["Count[2]"]);
        spriggitFields["Count[3]"].ShouldBe(dtoFields["Count[3]"]);
        spriggitFields["CreatedObjectCount"].ShouldBe(dtoFields["CreatedObjectCount"]);
        spriggitFields["Data.MutagenObjectType"].ShouldBe(dtoFields["Data.MutagenObjectType"]);
        spriggitFields["Data.Perk"].ShouldBe(dtoFields["Data.Perk"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["Item[0]"].ShouldBe(dtoFields["Item[0]"]);
        spriggitFields["Item[1]"].ShouldBe(dtoFields["Item[1]"]);
        spriggitFields["Item[2]"].ShouldBe(dtoFields["Item[2]"]);
        spriggitFields["Item[3]"].ShouldBe(dtoFields["Item[3]"]);
        spriggitFields["Unknown2"].ShouldBe(dtoFields["Unknown2"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["WorkbenchKeyword"].ShouldBe(dtoFields["WorkbenchKeywordFormKey"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "COBJ")]
    [Trait("FormKey", "0F431A:Skyrim.esm")]
    [Trait("EditorID", "RecipeFoodSoupCabbagePotato")]
    [Trait("SpriggitFile", "ConstructibleObjects/RecipeFoodSoupCabbagePotato - 0F431A_Skyrim.esm.yaml")]
    public void Skyrim_COBJ_ShouldMatchSpriggitSample_RecipeFoodSoupCabbagePotato()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.ConstructibleObject,
            "RecipeFoodSoupCabbagePotato");
        var dto = Helpers.GetDTO<ConstructibleObjectDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.ConstructibleObject,
            "0F431A:Skyrim.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["Count[0]"].ShouldBe(dtoFields["Count[0]"]);
        spriggitFields["Count[1]"].ShouldBe(dtoFields["Count[1]"]);
        spriggitFields["Count[2]"].ShouldBe(dtoFields["Count[2]"]);
        spriggitFields["Count[3]"].ShouldBe(dtoFields["Count[3]"]);
        spriggitFields["CreatedObject"].ShouldBe(dtoFields["CreatedObjectFormKey"]);
        spriggitFields["CreatedObjectCount"].ShouldBe(dtoFields["CreatedObjectCount"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["Item[0]"].ShouldBe(dtoFields["Item[0]"]);
        spriggitFields["Item[1]"].ShouldBe(dtoFields["Item[1]"]);
        spriggitFields["Item[2]"].ShouldBe(dtoFields["Item[2]"]);
        spriggitFields["Item[3]"].ShouldBe(dtoFields["Item[3]"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["WorkbenchKeyword"].ShouldBe(dtoFields["WorkbenchKeywordFormKey"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }
}
