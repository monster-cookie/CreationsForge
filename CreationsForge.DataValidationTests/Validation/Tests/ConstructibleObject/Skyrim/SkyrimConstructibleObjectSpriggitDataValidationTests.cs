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

        Helpers.GetSpriggitField(spriggit, "ComparisonValue").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue"));
        Helpers.GetSpriggitField(spriggit, "Count[0]").ShouldBe(Helpers.GetDTOField(dto, "Count[0]"));
        Helpers.GetSpriggitField(spriggit, "Count[1]").ShouldBe(Helpers.GetDTOField(dto, "Count[1]"));
        Helpers.GetSpriggitField(spriggit, "Count[2]").ShouldBe(Helpers.GetDTOField(dto, "Count[2]"));
        Helpers.GetSpriggitField(spriggit, "Count[3]").ShouldBe(Helpers.GetDTOField(dto, "Count[3]"));
        Helpers.GetSpriggitField(spriggit, "CreatedObject").ShouldBe(Helpers.GetDTOField(dto, "CreatedObjectFormKey"));
        Helpers.GetSpriggitField(spriggit, "CreatedObjectCount").ShouldBe(Helpers.GetDTOField(dto, "CreatedObjectCount"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "Data.Perk").ShouldBe(Helpers.GetDTOField(dto, "Data.Perk"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "Item[0]").ShouldBe(Helpers.GetDTOField(dto, "Item[0]"));
        Helpers.GetSpriggitField(spriggit, "Item[1]").ShouldBe(Helpers.GetDTOField(dto, "Item[1]"));
        Helpers.GetSpriggitField(spriggit, "Item[2]").ShouldBe(Helpers.GetDTOField(dto, "Item[2]"));
        Helpers.GetSpriggitField(spriggit, "Item[3]").ShouldBe(Helpers.GetDTOField(dto, "Item[3]"));
        Helpers.GetSpriggitField(spriggit, "Unknown2").ShouldBe(Helpers.GetDTOField(dto, "Unknown2"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));
        Helpers.GetSpriggitField(spriggit, "WorkbenchKeyword").ShouldBe(Helpers.GetDTOField(dto, "WorkbenchKeywordFormKey"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "ComparisonValue", "Count[0]", "Count[1]", "Count[2]", "Count[3]", "CreatedObject", "CreatedObjectCount", "Data.MutagenObjectType", "Data.Perk", "EditorID", "FormKey", "FormVersion", "Item[0]", "Item[1]", "Item[2]", "Item[3]", "Unknown2", "Version2", "VersionControl", "WorkbenchKeyword");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "ComparisonValue", "Count[0]", "Count[1]", "Count[2]", "Count[3]", "CreatedObjectFormKey", "CreatedObjectCount", "Data.MutagenObjectType", "Data.Perk", "EditorID", "FormKey", "FormVersion", "Item[0]", "Item[1]", "Item[2]", "Item[3]", "Unknown2", "Version2", "VersionControl", "WorkbenchKeywordFormKey");
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

        Helpers.GetSpriggitField(spriggit, "ComparisonValue").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue"));
        Helpers.GetSpriggitField(spriggit, "Count[0]").ShouldBe(Helpers.GetDTOField(dto, "Count[0]"));
        Helpers.GetSpriggitField(spriggit, "Count[1]").ShouldBe(Helpers.GetDTOField(dto, "Count[1]"));
        Helpers.GetSpriggitField(spriggit, "Count[2]").ShouldBe(Helpers.GetDTOField(dto, "Count[2]"));
        Helpers.GetSpriggitField(spriggit, "Count[3]").ShouldBe(Helpers.GetDTOField(dto, "Count[3]"));
        Helpers.GetSpriggitField(spriggit, "CreatedObject").ShouldBe(Helpers.GetDTOField(dto, "CreatedObjectFormKey"));
        Helpers.GetSpriggitField(spriggit, "CreatedObjectCount").ShouldBe(Helpers.GetDTOField(dto, "CreatedObjectCount"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "Data.Perk").ShouldBe(Helpers.GetDTOField(dto, "Data.Perk"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "Item[0]").ShouldBe(Helpers.GetDTOField(dto, "Item[0]"));
        Helpers.GetSpriggitField(spriggit, "Item[1]").ShouldBe(Helpers.GetDTOField(dto, "Item[1]"));
        Helpers.GetSpriggitField(spriggit, "Item[2]").ShouldBe(Helpers.GetDTOField(dto, "Item[2]"));
        Helpers.GetSpriggitField(spriggit, "Item[3]").ShouldBe(Helpers.GetDTOField(dto, "Item[3]"));
        Helpers.GetSpriggitField(spriggit, "Unknown2").ShouldBe(Helpers.GetDTOField(dto, "Unknown2"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));
        Helpers.GetSpriggitField(spriggit, "WorkbenchKeyword").ShouldBe(Helpers.GetDTOField(dto, "WorkbenchKeywordFormKey"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "ComparisonValue", "Count[0]", "Count[1]", "Count[2]", "Count[3]", "CreatedObject", "CreatedObjectCount", "Data.MutagenObjectType", "Data.Perk", "EditorID", "FormKey", "FormVersion", "Item[0]", "Item[1]", "Item[2]", "Item[3]", "Unknown2", "Version2", "VersionControl", "WorkbenchKeyword");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "ComparisonValue", "Count[0]", "Count[1]", "Count[2]", "Count[3]", "CreatedObjectFormKey", "CreatedObjectCount", "Data.MutagenObjectType", "Data.Perk", "EditorID", "FormKey", "FormVersion", "Item[0]", "Item[1]", "Item[2]", "Item[3]", "Unknown2", "Version2", "VersionControl", "WorkbenchKeywordFormKey");
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

        Helpers.GetSpriggitField(spriggit, "ComparisonValue").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue"));
        Helpers.GetSpriggitField(spriggit, "Count[0]").ShouldBe(Helpers.GetDTOField(dto, "Count[0]"));
        Helpers.GetSpriggitField(spriggit, "Count[1]").ShouldBe(Helpers.GetDTOField(dto, "Count[1]"));
        Helpers.GetSpriggitField(spriggit, "Count[2]").ShouldBe(Helpers.GetDTOField(dto, "Count[2]"));
        Helpers.GetSpriggitField(spriggit, "Count[3]").ShouldBe(Helpers.GetDTOField(dto, "Count[3]"));
        Helpers.GetSpriggitField(spriggit, "CreatedObject").ShouldBe(Helpers.GetDTOField(dto, "CreatedObjectFormKey"));
        Helpers.GetSpriggitField(spriggit, "CreatedObjectCount").ShouldBe(Helpers.GetDTOField(dto, "CreatedObjectCount"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "Data.Perk").ShouldBe(Helpers.GetDTOField(dto, "Data.Perk"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "Item[0]").ShouldBe(Helpers.GetDTOField(dto, "Item[0]"));
        Helpers.GetSpriggitField(spriggit, "Item[1]").ShouldBe(Helpers.GetDTOField(dto, "Item[1]"));
        Helpers.GetSpriggitField(spriggit, "Item[2]").ShouldBe(Helpers.GetDTOField(dto, "Item[2]"));
        Helpers.GetSpriggitField(spriggit, "Item[3]").ShouldBe(Helpers.GetDTOField(dto, "Item[3]"));
        Helpers.GetSpriggitField(spriggit, "Unknown2").ShouldBe(Helpers.GetDTOField(dto, "Unknown2"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));
        Helpers.GetSpriggitField(spriggit, "WorkbenchKeyword").ShouldBe(Helpers.GetDTOField(dto, "WorkbenchKeywordFormKey"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "ComparisonValue", "Count[0]", "Count[1]", "Count[2]", "Count[3]", "CreatedObject", "CreatedObjectCount", "Data.MutagenObjectType", "Data.Perk", "EditorID", "FormKey", "FormVersion", "Item[0]", "Item[1]", "Item[2]", "Item[3]", "Unknown2", "Version2", "VersionControl", "WorkbenchKeyword");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "ComparisonValue", "Count[0]", "Count[1]", "Count[2]", "Count[3]", "CreatedObjectFormKey", "CreatedObjectCount", "Data.MutagenObjectType", "Data.Perk", "EditorID", "FormKey", "FormVersion", "Item[0]", "Item[1]", "Item[2]", "Item[3]", "Unknown2", "Version2", "VersionControl", "WorkbenchKeywordFormKey");
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

        Helpers.GetSpriggitField(spriggit, "ComparisonValue").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue"));
        Helpers.GetSpriggitField(spriggit, "Count[0]").ShouldBe(Helpers.GetDTOField(dto, "Count[0]"));
        Helpers.GetSpriggitField(spriggit, "Count[1]").ShouldBe(Helpers.GetDTOField(dto, "Count[1]"));
        Helpers.GetSpriggitField(spriggit, "Count[2]").ShouldBe(Helpers.GetDTOField(dto, "Count[2]"));
        Helpers.GetSpriggitField(spriggit, "Count[3]").ShouldBe(Helpers.GetDTOField(dto, "Count[3]"));
        Helpers.GetSpriggitField(spriggit, "CreatedObjectCount").ShouldBe(Helpers.GetDTOField(dto, "CreatedObjectCount"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "Data.Perk").ShouldBe(Helpers.GetDTOField(dto, "Data.Perk"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "Item[0]").ShouldBe(Helpers.GetDTOField(dto, "Item[0]"));
        Helpers.GetSpriggitField(spriggit, "Item[1]").ShouldBe(Helpers.GetDTOField(dto, "Item[1]"));
        Helpers.GetSpriggitField(spriggit, "Item[2]").ShouldBe(Helpers.GetDTOField(dto, "Item[2]"));
        Helpers.GetSpriggitField(spriggit, "Item[3]").ShouldBe(Helpers.GetDTOField(dto, "Item[3]"));
        Helpers.GetSpriggitField(spriggit, "Unknown2").ShouldBe(Helpers.GetDTOField(dto, "Unknown2"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));
        Helpers.GetSpriggitField(spriggit, "WorkbenchKeyword").ShouldBe(Helpers.GetDTOField(dto, "WorkbenchKeywordFormKey"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "ComparisonValue", "Count[0]", "Count[1]", "Count[2]", "Count[3]", "CreatedObjectCount", "Data.MutagenObjectType", "Data.Perk", "EditorID", "FormKey", "FormVersion", "Item[0]", "Item[1]", "Item[2]", "Item[3]", "Unknown2", "Version2", "VersionControl", "WorkbenchKeyword");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "ComparisonValue", "Count[0]", "Count[1]", "Count[2]", "Count[3]", "CreatedObjectCount", "Data.MutagenObjectType", "Data.Perk", "EditorID", "FormKey", "FormVersion", "Item[0]", "Item[1]", "Item[2]", "Item[3]", "Unknown2", "Version2", "VersionControl", "WorkbenchKeywordFormKey");
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

        Helpers.GetSpriggitField(spriggit, "Count[0]").ShouldBe(Helpers.GetDTOField(dto, "Count[0]"));
        Helpers.GetSpriggitField(spriggit, "Count[1]").ShouldBe(Helpers.GetDTOField(dto, "Count[1]"));
        Helpers.GetSpriggitField(spriggit, "Count[2]").ShouldBe(Helpers.GetDTOField(dto, "Count[2]"));
        Helpers.GetSpriggitField(spriggit, "Count[3]").ShouldBe(Helpers.GetDTOField(dto, "Count[3]"));
        Helpers.GetSpriggitField(spriggit, "CreatedObject").ShouldBe(Helpers.GetDTOField(dto, "CreatedObjectFormKey"));
        Helpers.GetSpriggitField(spriggit, "CreatedObjectCount").ShouldBe(Helpers.GetDTOField(dto, "CreatedObjectCount"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "Item[0]").ShouldBe(Helpers.GetDTOField(dto, "Item[0]"));
        Helpers.GetSpriggitField(spriggit, "Item[1]").ShouldBe(Helpers.GetDTOField(dto, "Item[1]"));
        Helpers.GetSpriggitField(spriggit, "Item[2]").ShouldBe(Helpers.GetDTOField(dto, "Item[2]"));
        Helpers.GetSpriggitField(spriggit, "Item[3]").ShouldBe(Helpers.GetDTOField(dto, "Item[3]"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));
        Helpers.GetSpriggitField(spriggit, "WorkbenchKeyword").ShouldBe(Helpers.GetDTOField(dto, "WorkbenchKeywordFormKey"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "Count[0]", "Count[1]", "Count[2]", "Count[3]", "CreatedObject", "CreatedObjectCount", "EditorID", "FormKey", "FormVersion", "Item[0]", "Item[1]", "Item[2]", "Item[3]", "Version2", "VersionControl", "WorkbenchKeyword");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "Count[0]", "Count[1]", "Count[2]", "Count[3]", "CreatedObjectFormKey", "CreatedObjectCount", "EditorID", "FormKey", "FormVersion", "Item[0]", "Item[1]", "Item[2]", "Item[3]", "Version2", "VersionControl", "WorkbenchKeywordFormKey");
    }
}