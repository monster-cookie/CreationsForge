using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.ConstructibleObject.Starfield;

public class StarfieldConstructibleObjectSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "COBJ")]
    [Trait("FormKey", "007F7C:Starfield.esm")]
    [Trait("EditorID", "co_Outpost_Power_Reactor01")]
    [Trait("SpriggitFile", "ConstructibleObjects/co_Outpost_Power_Reactor01 - 007F7C_Starfield.esm.yaml")]
    public void Starfield_COBJ_ShouldMatchSpriggitSample_co_Outpost_Power_Reactor01()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.ConstructibleObject,
            "co_Outpost_Power_Reactor01");
        var dto = Helpers.GetDTO<ConstructibleObjectDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.ConstructibleObject,
            "007F7C:Starfield.esm");

        Helpers.GetSpriggitField(spriggit, "AmountProduced").ShouldBe(Helpers.GetDTOField(dto, "AmountProduced"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue"));
        Helpers.GetSpriggitField(spriggit, "CreatedObject").ShouldBe(Helpers.GetDTOField(dto, "CreatedObjectFormKey"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "Description.Count").ShouldBe(Helpers.GetDTOField(dto, "Description.Count"));
        Helpers.GetSpriggitField(spriggit, "Description.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Description.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Description[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[0].String").ShouldBe(Helpers.GetDTOField(dto, "Description[0].String"));
        Helpers.GetSpriggitField(spriggit, "Description[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[1].String").ShouldBe(Helpers.GetDTOField(dto, "Description[1].String"));
        Helpers.GetSpriggitField(spriggit, "Description[2].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[2].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[2].String").ShouldBe(Helpers.GetDTOField(dto, "Description[2].String"));
        Helpers.GetSpriggitField(spriggit, "Description[3].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[3].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[3].String").ShouldBe(Helpers.GetDTOField(dto, "Description[3].String"));
        Helpers.GetSpriggitField(spriggit, "Description[4].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[4].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[4].String").ShouldBe(Helpers.GetDTOField(dto, "Description[4].String"));
        Helpers.GetSpriggitField(spriggit, "Description[5].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[5].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[5].String").ShouldBe(Helpers.GetDTOField(dto, "Description[5].String"));
        Helpers.GetSpriggitField(spriggit, "Description[6].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[6].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[6].String").ShouldBe(Helpers.GetDTOField(dto, "Description[6].String"));
        Helpers.GetSpriggitField(spriggit, "Description[7].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[7].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[7].String").ShouldBe(Helpers.GetDTOField(dto, "Description[7].String"));
        Helpers.GetSpriggitField(spriggit, "Description[8].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[8].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[8].String").ShouldBe(Helpers.GetDTOField(dto, "Description[8].String"));
        Helpers.GetSpriggitField(spriggit, "DropdownSound.Start").ShouldBe(Helpers.GetDTOField(dto, "DropdownSound.Start"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "Flags").ShouldBe(Helpers.GetDTOField(dto, "Flags"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "LearnMethod").ShouldBe(Helpers.GetDTOField(dto, "LearnMethod"));
        Helpers.GetSpriggitField(spriggit, "MenuSortOrder").ShouldBe(Helpers.GetDTOField(dto, "MenuSortOrder"));
        Helpers.GetSpriggitField(spriggit, "PickupSound.Start").ShouldBe(Helpers.GetDTOField(dto, "PickupSound.Start"));
        Helpers.GetSpriggitField(spriggit, "RequiredCount[0]").ShouldBe(Helpers.GetDTOField(dto, "RequiredCount[0]"));
        Helpers.GetSpriggitField(spriggit, "RequiredCount[1]").ShouldBe(Helpers.GetDTOField(dto, "RequiredCount[1]"));
        Helpers.GetSpriggitField(spriggit, "RequiredCount[2]").ShouldBe(Helpers.GetDTOField(dto, "RequiredCount[2]"));
        Helpers.GetSpriggitField(spriggit, "RequiredCount[3]").ShouldBe(Helpers.GetDTOField(dto, "RequiredCount[3]"));
        Helpers.GetSpriggitField(spriggit, "RequiredCount[4]").ShouldBe(Helpers.GetDTOField(dto, "RequiredCount[4]"));
        Helpers.GetSpriggitField(spriggit, "RequiredCount[5]").ShouldBe(Helpers.GetDTOField(dto, "RequiredCount[5]"));
        Helpers.GetSpriggitField(spriggit, "RequiredCount[6]").ShouldBe(Helpers.GetDTOField(dto, "RequiredCount[6]"));
        Helpers.GetSpriggitField(spriggit, "Unknown2").ShouldBe(Helpers.GetDTOField(dto, "Unknown2"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));
        Helpers.GetSpriggitField(spriggit, "WorkbenchKeyword").ShouldBe(Helpers.GetDTOField(dto, "WorkbenchKeywordFormKey"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "AmountProduced", "ComparisonValue", "CreatedObject", "Data.FirstParameter", "Data.MutagenObjectType", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "DropdownSound.Start", "EditorID", "Flags", "FormKey", "FormVersion", "LearnMethod", "MenuSortOrder", "PickupSound.Start", "RequiredCount[0]", "RequiredCount[1]", "RequiredCount[2]", "RequiredCount[3]", "RequiredCount[4]", "RequiredCount[5]", "RequiredCount[6]", "Unknown2", "Version2", "VersionControl", "WorkbenchKeyword");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "AmountProduced", "ComparisonValue", "CreatedObjectFormKey", "Data.FirstParameter", "Data.MutagenObjectType", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "DropdownSound.Start", "EditorID", "Flags", "FormKey", "FormVersion", "LearnMethod", "MenuSortOrder", "PickupSound.Start", "RequiredCount[0]", "RequiredCount[1]", "RequiredCount[2]", "RequiredCount[3]", "RequiredCount[4]", "RequiredCount[5]", "RequiredCount[6]", "Unknown2", "Version2", "VersionControl", "WorkbenchKeywordFormKey");
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "COBJ")]
    [Trait("FormKey", "1C5144:Starfield.esm")]
    [Trait("EditorID", "co_Outpost_Power_Reactor02")]
    [Trait("SpriggitFile", "ConstructibleObjects/co_Outpost_Power_Reactor02 - 1C5144_Starfield.esm.yaml")]
    public void Starfield_COBJ_ShouldMatchSpriggitSample_co_Outpost_Power_Reactor02()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.ConstructibleObject,
            "co_Outpost_Power_Reactor02");
        var dto = Helpers.GetDTO<ConstructibleObjectDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.ConstructibleObject,
            "1C5144:Starfield.esm");

        Helpers.GetSpriggitField(spriggit, "AmountProduced").ShouldBe(Helpers.GetDTOField(dto, "AmountProduced"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue"));
        Helpers.GetSpriggitField(spriggit, "CreatedObject").ShouldBe(Helpers.GetDTOField(dto, "CreatedObjectFormKey"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "Description.Count").ShouldBe(Helpers.GetDTOField(dto, "Description.Count"));
        Helpers.GetSpriggitField(spriggit, "Description.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Description.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Description[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[0].String").ShouldBe(Helpers.GetDTOField(dto, "Description[0].String"));
        Helpers.GetSpriggitField(spriggit, "Description[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[1].String").ShouldBe(Helpers.GetDTOField(dto, "Description[1].String"));
        Helpers.GetSpriggitField(spriggit, "Description[2].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[2].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[2].String").ShouldBe(Helpers.GetDTOField(dto, "Description[2].String"));
        Helpers.GetSpriggitField(spriggit, "Description[3].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[3].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[3].String").ShouldBe(Helpers.GetDTOField(dto, "Description[3].String"));
        Helpers.GetSpriggitField(spriggit, "Description[4].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[4].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[4].String").ShouldBe(Helpers.GetDTOField(dto, "Description[4].String"));
        Helpers.GetSpriggitField(spriggit, "Description[5].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[5].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[5].String").ShouldBe(Helpers.GetDTOField(dto, "Description[5].String"));
        Helpers.GetSpriggitField(spriggit, "Description[6].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[6].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[6].String").ShouldBe(Helpers.GetDTOField(dto, "Description[6].String"));
        Helpers.GetSpriggitField(spriggit, "Description[7].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[7].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[7].String").ShouldBe(Helpers.GetDTOField(dto, "Description[7].String"));
        Helpers.GetSpriggitField(spriggit, "Description[8].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[8].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[8].String").ShouldBe(Helpers.GetDTOField(dto, "Description[8].String"));
        Helpers.GetSpriggitField(spriggit, "DropdownSound.Start").ShouldBe(Helpers.GetDTOField(dto, "DropdownSound.Start"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "Flags").ShouldBe(Helpers.GetDTOField(dto, "Flags"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "LearnMethod").ShouldBe(Helpers.GetDTOField(dto, "LearnMethod"));
        Helpers.GetSpriggitField(spriggit, "MenuSortOrder").ShouldBe(Helpers.GetDTOField(dto, "MenuSortOrder"));
        Helpers.GetSpriggitField(spriggit, "PickupSound.Start").ShouldBe(Helpers.GetDTOField(dto, "PickupSound.Start"));
        Helpers.GetSpriggitField(spriggit, "RequiredCount[0]").ShouldBe(Helpers.GetDTOField(dto, "RequiredCount[0]"));
        Helpers.GetSpriggitField(spriggit, "RequiredCount[1]").ShouldBe(Helpers.GetDTOField(dto, "RequiredCount[1]"));
        Helpers.GetSpriggitField(spriggit, "RequiredCount[2]").ShouldBe(Helpers.GetDTOField(dto, "RequiredCount[2]"));
        Helpers.GetSpriggitField(spriggit, "RequiredCount[3]").ShouldBe(Helpers.GetDTOField(dto, "RequiredCount[3]"));
        Helpers.GetSpriggitField(spriggit, "RequiredCount[4]").ShouldBe(Helpers.GetDTOField(dto, "RequiredCount[4]"));
        Helpers.GetSpriggitField(spriggit, "RequiredCount[5]").ShouldBe(Helpers.GetDTOField(dto, "RequiredCount[5]"));
        Helpers.GetSpriggitField(spriggit, "RequiredCount[6]").ShouldBe(Helpers.GetDTOField(dto, "RequiredCount[6]"));
        Helpers.GetSpriggitField(spriggit, "Unknown2").ShouldBe(Helpers.GetDTOField(dto, "Unknown2"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));
        Helpers.GetSpriggitField(spriggit, "WorkbenchKeyword").ShouldBe(Helpers.GetDTOField(dto, "WorkbenchKeywordFormKey"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "AmountProduced", "ComparisonValue", "CreatedObject", "Data.FirstParameter", "Data.MutagenObjectType", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "DropdownSound.Start", "EditorID", "Flags", "FormKey", "FormVersion", "LearnMethod", "MenuSortOrder", "PickupSound.Start", "RequiredCount[0]", "RequiredCount[1]", "RequiredCount[2]", "RequiredCount[3]", "RequiredCount[4]", "RequiredCount[5]", "RequiredCount[6]", "Unknown2", "Version2", "VersionControl", "WorkbenchKeyword");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "AmountProduced", "ComparisonValue", "CreatedObjectFormKey", "Data.FirstParameter", "Data.MutagenObjectType", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "DropdownSound.Start", "EditorID", "Flags", "FormKey", "FormVersion", "LearnMethod", "MenuSortOrder", "PickupSound.Start", "RequiredCount[0]", "RequiredCount[1]", "RequiredCount[2]", "RequiredCount[3]", "RequiredCount[4]", "RequiredCount[5]", "RequiredCount[6]", "Unknown2", "Version2", "VersionControl", "WorkbenchKeywordFormKey");
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "COBJ")]
    [Trait("FormKey", "0C8720:Starfield.esm")]
    [Trait("EditorID", "co_Chem_XenoAurora")]
    [Trait("SpriggitFile", "ConstructibleObjects/co_Chem_XenoAurora - 0C8720_Starfield.esm.yaml")]
    public void Starfield_COBJ_ShouldMatchSpriggitSample_co_Chem_XenoAurora()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.ConstructibleObject,
            "co_Chem_XenoAurora");
        var dto = Helpers.GetDTO<ConstructibleObjectDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.ConstructibleObject,
            "0C8720:Starfield.esm");

        Helpers.GetSpriggitField(spriggit, "AmountProduced").ShouldBe(Helpers.GetDTOField(dto, "AmountProduced"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue"));
        Helpers.GetSpriggitField(spriggit, "CreatedObject").ShouldBe(Helpers.GetDTOField(dto, "CreatedObjectFormKey"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[1]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[1]"));
        Helpers.GetSpriggitField(spriggit, "Data.SecondParameter[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.SecondParameter[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.SecondParameter[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.SecondParameter[1]"));
        Helpers.GetSpriggitField(spriggit, "Description.Count").ShouldBe(Helpers.GetDTOField(dto, "Description.Count"));
        Helpers.GetSpriggitField(spriggit, "Description.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Description.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Description[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[0].String").ShouldBe(Helpers.GetDTOField(dto, "Description[0].String"));
        Helpers.GetSpriggitField(spriggit, "Description[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[1].String").ShouldBe(Helpers.GetDTOField(dto, "Description[1].String"));
        Helpers.GetSpriggitField(spriggit, "Description[2].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[2].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[2].String").ShouldBe(Helpers.GetDTOField(dto, "Description[2].String"));
        Helpers.GetSpriggitField(spriggit, "Description[3].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[3].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[3].String").ShouldBe(Helpers.GetDTOField(dto, "Description[3].String"));
        Helpers.GetSpriggitField(spriggit, "Description[4].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[4].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[4].String").ShouldBe(Helpers.GetDTOField(dto, "Description[4].String"));
        Helpers.GetSpriggitField(spriggit, "Description[5].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[5].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[5].String").ShouldBe(Helpers.GetDTOField(dto, "Description[5].String"));
        Helpers.GetSpriggitField(spriggit, "Description[6].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[6].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[6].String").ShouldBe(Helpers.GetDTOField(dto, "Description[6].String"));
        Helpers.GetSpriggitField(spriggit, "Description[7].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[7].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[7].String").ShouldBe(Helpers.GetDTOField(dto, "Description[7].String"));
        Helpers.GetSpriggitField(spriggit, "Description[8].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[8].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[8].String").ShouldBe(Helpers.GetDTOField(dto, "Description[8].String"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "Flags").ShouldBe(Helpers.GetDTOField(dto, "Flags"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "LearnMethod").ShouldBe(Helpers.GetDTOField(dto, "LearnMethod"));
        Helpers.GetSpriggitField(spriggit, "RequiredCount[0]").ShouldBe(Helpers.GetDTOField(dto, "RequiredCount[0]"));
        Helpers.GetSpriggitField(spriggit, "RequiredCount[1]").ShouldBe(Helpers.GetDTOField(dto, "RequiredCount[1]"));
        Helpers.GetSpriggitField(spriggit, "RequiredCount[2]").ShouldBe(Helpers.GetDTOField(dto, "RequiredCount[2]"));
        Helpers.GetSpriggitField(spriggit, "RequiredCount[3]").ShouldBe(Helpers.GetDTOField(dto, "RequiredCount[3]"));
        Helpers.GetSpriggitField(spriggit, "Unknown2[0]").ShouldBe(Helpers.GetDTOField(dto, "Unknown2[0]"));
        Helpers.GetSpriggitField(spriggit, "Unknown2[1]").ShouldBe(Helpers.GetDTOField(dto, "Unknown2[1]"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));
        Helpers.GetSpriggitField(spriggit, "WorkbenchKeyword").ShouldBe(Helpers.GetDTOField(dto, "WorkbenchKeywordFormKey"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "AmountProduced", "ComparisonValue", "CreatedObject", "Data.FirstParameter[0]", "Data.FirstParameter[1]", "Data.MutagenObjectType[0]", "Data.MutagenObjectType[1]", "Data.SecondParameter[0]", "Data.SecondParameter[1]", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "EditorID", "Flags", "FormKey", "FormVersion", "LearnMethod", "RequiredCount[0]", "RequiredCount[1]", "RequiredCount[2]", "RequiredCount[3]", "Unknown2[0]", "Unknown2[1]", "Version2", "VersionControl", "WorkbenchKeyword");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "AmountProduced", "ComparisonValue", "CreatedObjectFormKey", "Data.FirstParameter[0]", "Data.FirstParameter[1]", "Data.MutagenObjectType[0]", "Data.MutagenObjectType[1]", "Data.SecondParameter[0]", "Data.SecondParameter[1]", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "EditorID", "Flags", "FormKey", "FormVersion", "LearnMethod", "RequiredCount[0]", "RequiredCount[1]", "RequiredCount[2]", "RequiredCount[3]", "Unknown2[0]", "Unknown2[1]", "Version2", "VersionControl", "WorkbenchKeywordFormKey");
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "COBJ")]
    [Trait("FormKey", "09DE67:Starfield.esm")]
    [Trait("EditorID", "UC07_co_mfg_MicroCell_Old")]
    [Trait("SpriggitFile", "ConstructibleObjects/UC07_co_mfg_MicroCell_Old - 09DE67_Starfield.esm.yaml")]
    public void Starfield_COBJ_ShouldMatchSpriggitSample_UC07_co_mfg_MicroCell_Old()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.ConstructibleObject,
            "UC07_co_mfg_MicroCell_Old");
        var dto = Helpers.GetDTO<ConstructibleObjectDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.ConstructibleObject,
            "09DE67:Starfield.esm");

        Helpers.GetSpriggitField(spriggit, "AmountProduced").ShouldBe(Helpers.GetDTOField(dto, "AmountProduced"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue"));
        Helpers.GetSpriggitField(spriggit, "CreatedObject").ShouldBe(Helpers.GetDTOField(dto, "CreatedObjectFormKey"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[1]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[1]"));
        Helpers.GetSpriggitField(spriggit, "Description.Count").ShouldBe(Helpers.GetDTOField(dto, "Description.Count"));
        Helpers.GetSpriggitField(spriggit, "Description.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Description.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Description[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[0].String").ShouldBe(Helpers.GetDTOField(dto, "Description[0].String"));
        Helpers.GetSpriggitField(spriggit, "Description[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[1].String").ShouldBe(Helpers.GetDTOField(dto, "Description[1].String"));
        Helpers.GetSpriggitField(spriggit, "Description[2].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[2].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[2].String").ShouldBe(Helpers.GetDTOField(dto, "Description[2].String"));
        Helpers.GetSpriggitField(spriggit, "Description[3].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[3].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[3].String").ShouldBe(Helpers.GetDTOField(dto, "Description[3].String"));
        Helpers.GetSpriggitField(spriggit, "Description[4].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[4].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[4].String").ShouldBe(Helpers.GetDTOField(dto, "Description[4].String"));
        Helpers.GetSpriggitField(spriggit, "Description[5].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[5].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[5].String").ShouldBe(Helpers.GetDTOField(dto, "Description[5].String"));
        Helpers.GetSpriggitField(spriggit, "Description[6].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[6].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[6].String").ShouldBe(Helpers.GetDTOField(dto, "Description[6].String"));
        Helpers.GetSpriggitField(spriggit, "Description[7].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[7].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[7].String").ShouldBe(Helpers.GetDTOField(dto, "Description[7].String"));
        Helpers.GetSpriggitField(spriggit, "Description[8].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[8].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[8].String").ShouldBe(Helpers.GetDTOField(dto, "Description[8].String"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "Flags").ShouldBe(Helpers.GetDTOField(dto, "Flags"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "LearnMethod").ShouldBe(Helpers.GetDTOField(dto, "LearnMethod"));
        Helpers.GetSpriggitField(spriggit, "RequiredCount[0]").ShouldBe(Helpers.GetDTOField(dto, "RequiredCount[0]"));
        Helpers.GetSpriggitField(spriggit, "RequiredCount[1]").ShouldBe(Helpers.GetDTOField(dto, "RequiredCount[1]"));
        Helpers.GetSpriggitField(spriggit, "RequiredCount[2]").ShouldBe(Helpers.GetDTOField(dto, "RequiredCount[2]"));
        Helpers.GetSpriggitField(spriggit, "RequiredCount[3]").ShouldBe(Helpers.GetDTOField(dto, "RequiredCount[3]"));
        Helpers.GetSpriggitField(spriggit, "RequiredCount[4]").ShouldBe(Helpers.GetDTOField(dto, "RequiredCount[4]"));
        Helpers.GetSpriggitField(spriggit, "RequiredCount[5]").ShouldBe(Helpers.GetDTOField(dto, "RequiredCount[5]"));
        Helpers.GetSpriggitField(spriggit, "RequiredCount[6]").ShouldBe(Helpers.GetDTOField(dto, "RequiredCount[6]"));
        Helpers.GetSpriggitField(spriggit, "Unknown2[0]").ShouldBe(Helpers.GetDTOField(dto, "Unknown2[0]"));
        Helpers.GetSpriggitField(spriggit, "Unknown2[1]").ShouldBe(Helpers.GetDTOField(dto, "Unknown2[1]"));
        Helpers.GetSpriggitField(spriggit, "Value").ShouldBe(Helpers.GetDTOField(dto, "Value"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));
        Helpers.GetSpriggitField(spriggit, "WorkbenchKeyword").ShouldBe(Helpers.GetDTOField(dto, "WorkbenchKeywordFormKey"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "AmountProduced", "ComparisonValue", "CreatedObject", "Data.FirstParameter[0]", "Data.FirstParameter[1]", "Data.MutagenObjectType[0]", "Data.MutagenObjectType[1]", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "EditorID", "Flags", "FormKey", "FormVersion", "LearnMethod", "RequiredCount[0]", "RequiredCount[1]", "RequiredCount[2]", "RequiredCount[3]", "RequiredCount[4]", "RequiredCount[5]", "RequiredCount[6]", "Unknown2[0]", "Unknown2[1]", "Value", "Version2", "VersionControl", "WorkbenchKeyword");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "AmountProduced", "ComparisonValue", "CreatedObjectFormKey", "Data.FirstParameter[0]", "Data.FirstParameter[1]", "Data.MutagenObjectType[0]", "Data.MutagenObjectType[1]", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "EditorID", "Flags", "FormKey", "FormVersion", "LearnMethod", "RequiredCount[0]", "RequiredCount[1]", "RequiredCount[2]", "RequiredCount[3]", "RequiredCount[4]", "RequiredCount[5]", "RequiredCount[6]", "Unknown2[0]", "Unknown2[1]", "Value", "Version2", "VersionControl", "WorkbenchKeywordFormKey");
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "COBJ")]
    [Trait("FormKey", "1DF844:Starfield.esm")]
    [Trait("EditorID", "co_Outpost_Misc_MissionBoardConsole")]
    [Trait("SpriggitFile", "ConstructibleObjects/co_Outpost_Misc_MissionBoardConsole - 1DF844_Starfield.esm.yaml")]
    public void Starfield_COBJ_ShouldMatchSpriggitSample_co_Outpost_Misc_MissionBoardConsole()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.ConstructibleObject,
            "co_Outpost_Misc_MissionBoardConsole");
        var dto = Helpers.GetDTO<ConstructibleObjectDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.ConstructibleObject,
            "1DF844:Starfield.esm");

        Helpers.GetSpriggitField(spriggit, "CreatedObject").ShouldBe(Helpers.GetDTOField(dto, "CreatedObjectFormKey"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "Description.Count").ShouldBe(Helpers.GetDTOField(dto, "Description.Count"));
        Helpers.GetSpriggitField(spriggit, "Description.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Description.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Description[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[0].String").ShouldBe(Helpers.GetDTOField(dto, "Description[0].String"));
        Helpers.GetSpriggitField(spriggit, "Description[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[1].String").ShouldBe(Helpers.GetDTOField(dto, "Description[1].String"));
        Helpers.GetSpriggitField(spriggit, "Description[2].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[2].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[2].String").ShouldBe(Helpers.GetDTOField(dto, "Description[2].String"));
        Helpers.GetSpriggitField(spriggit, "Description[3].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[3].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[3].String").ShouldBe(Helpers.GetDTOField(dto, "Description[3].String"));
        Helpers.GetSpriggitField(spriggit, "Description[4].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[4].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[4].String").ShouldBe(Helpers.GetDTOField(dto, "Description[4].String"));
        Helpers.GetSpriggitField(spriggit, "Description[5].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[5].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[5].String").ShouldBe(Helpers.GetDTOField(dto, "Description[5].String"));
        Helpers.GetSpriggitField(spriggit, "Description[6].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[6].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[6].String").ShouldBe(Helpers.GetDTOField(dto, "Description[6].String"));
        Helpers.GetSpriggitField(spriggit, "Description[7].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[7].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[7].String").ShouldBe(Helpers.GetDTOField(dto, "Description[7].String"));
        Helpers.GetSpriggitField(spriggit, "Description[8].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[8].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[8].String").ShouldBe(Helpers.GetDTOField(dto, "Description[8].String"));
        Helpers.GetSpriggitField(spriggit, "DropdownSound.Start").ShouldBe(Helpers.GetDTOField(dto, "DropdownSound.Start"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "Flags").ShouldBe(Helpers.GetDTOField(dto, "Flags"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "LearnMethod").ShouldBe(Helpers.GetDTOField(dto, "LearnMethod"));
        Helpers.GetSpriggitField(spriggit, "MajorRecordFlagsRaw").ShouldBe(Helpers.GetDTOField(dto, "MajorRecordFlags"));
        Helpers.GetSpriggitField(spriggit, "MenuSortOrder").ShouldBe(Helpers.GetDTOField(dto, "MenuSortOrder"));
        Helpers.GetSpriggitField(spriggit, "PickupSound.Start").ShouldBe(Helpers.GetDTOField(dto, "PickupSound.Start"));
        Helpers.GetSpriggitField(spriggit, "RequiredCount[0]").ShouldBe(Helpers.GetDTOField(dto, "RequiredCount[0]"));
        Helpers.GetSpriggitField(spriggit, "RequiredCount[1]").ShouldBe(Helpers.GetDTOField(dto, "RequiredCount[1]"));
        Helpers.GetSpriggitField(spriggit, "RequiredCount[2]").ShouldBe(Helpers.GetDTOField(dto, "RequiredCount[2]"));
        Helpers.GetSpriggitField(spriggit, "Unknown2").ShouldBe(Helpers.GetDTOField(dto, "Unknown2"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));
        Helpers.GetSpriggitField(spriggit, "WorkbenchKeyword").ShouldBe(Helpers.GetDTOField(dto, "WorkbenchKeywordFormKey"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "CreatedObject", "Data.MutagenObjectType", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "DropdownSound.Start", "EditorID", "Flags", "FormKey", "FormVersion", "LearnMethod", "MajorRecordFlagsRaw", "MenuSortOrder", "PickupSound.Start", "RequiredCount[0]", "RequiredCount[1]", "RequiredCount[2]", "Unknown2", "Version2", "VersionControl", "WorkbenchKeyword");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "CreatedObjectFormKey", "Data.MutagenObjectType", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "DropdownSound.Start", "EditorID", "Flags", "FormKey", "FormVersion", "LearnMethod", "MajorRecordFlags", "MenuSortOrder", "PickupSound.Start", "RequiredCount[0]", "RequiredCount[1]", "RequiredCount[2]", "Unknown2", "Version2", "VersionControl", "WorkbenchKeywordFormKey");
    }
}