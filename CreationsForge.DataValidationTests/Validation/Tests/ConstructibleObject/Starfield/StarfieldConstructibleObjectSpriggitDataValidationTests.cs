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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["AmountProduced"].ShouldBe(dtoFields["AmountProduced"]);
        spriggitFields["ComparisonValue"].ShouldBe(dtoFields["ComparisonValue"]);
        spriggitFields["CreatedObject"].ShouldBe(dtoFields["CreatedObjectFormKey"]);
        spriggitFields["Data.FirstParameter"].ShouldBe(dtoFields["Data.FirstParameter"]);
        spriggitFields["Data.MutagenObjectType"].ShouldBe(dtoFields["Data.MutagenObjectType"]);
        spriggitFields["Description.Count"].ShouldBe(dtoFields["Description.Count"]);
        spriggitFields["Description.TargetLanguage"].ShouldBe(dtoFields["Description.TargetLanguage"]);
        spriggitFields["Description[0].Language"].ShouldBe(dtoFields["Description[0].Language"]);
        spriggitFields["Description[0].String"].ShouldBe(dtoFields["Description[0].String"]);
        spriggitFields["Description[1].Language"].ShouldBe(dtoFields["Description[1].Language"]);
        spriggitFields["Description[1].String"].ShouldBe(dtoFields["Description[1].String"]);
        spriggitFields["Description[2].Language"].ShouldBe(dtoFields["Description[2].Language"]);
        spriggitFields["Description[2].String"].ShouldBe(dtoFields["Description[2].String"]);
        spriggitFields["Description[3].Language"].ShouldBe(dtoFields["Description[3].Language"]);
        spriggitFields["Description[3].String"].ShouldBe(dtoFields["Description[3].String"]);
        spriggitFields["Description[4].Language"].ShouldBe(dtoFields["Description[4].Language"]);
        spriggitFields["Description[4].String"].ShouldBe(dtoFields["Description[4].String"]);
        spriggitFields["Description[5].Language"].ShouldBe(dtoFields["Description[5].Language"]);
        spriggitFields["Description[5].String"].ShouldBe(dtoFields["Description[5].String"]);
        spriggitFields["Description[6].Language"].ShouldBe(dtoFields["Description[6].Language"]);
        spriggitFields["Description[6].String"].ShouldBe(dtoFields["Description[6].String"]);
        spriggitFields["Description[7].Language"].ShouldBe(dtoFields["Description[7].Language"]);
        spriggitFields["Description[7].String"].ShouldBe(dtoFields["Description[7].String"]);
        spriggitFields["Description[8].Language"].ShouldBe(dtoFields["Description[8].Language"]);
        spriggitFields["Description[8].String"].ShouldBe(dtoFields["Description[8].String"]);
        spriggitFields["DropdownSound.Start"].ShouldBe(dtoFields["DropdownSound.Start"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["Flags"].ShouldBe(dtoFields["Flags"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["LearnMethod"].ShouldBe(dtoFields["LearnMethod"]);
        spriggitFields["MenuSortOrder"].ShouldBe(dtoFields["MenuSortOrder"]);
        spriggitFields["PickupSound.Start"].ShouldBe(dtoFields["PickupSound.Start"]);
        spriggitFields["RequiredCount[0]"].ShouldBe(dtoFields["RequiredCount[0]"]);
        spriggitFields["RequiredCount[1]"].ShouldBe(dtoFields["RequiredCount[1]"]);
        spriggitFields["RequiredCount[2]"].ShouldBe(dtoFields["RequiredCount[2]"]);
        spriggitFields["RequiredCount[3]"].ShouldBe(dtoFields["RequiredCount[3]"]);
        spriggitFields["RequiredCount[4]"].ShouldBe(dtoFields["RequiredCount[4]"]);
        spriggitFields["RequiredCount[5]"].ShouldBe(dtoFields["RequiredCount[5]"]);
        spriggitFields["RequiredCount[6]"].ShouldBe(dtoFields["RequiredCount[6]"]);
        spriggitFields["Unknown2"].ShouldBe(dtoFields["Unknown2"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["WorkbenchKeyword"].ShouldBe(dtoFields["WorkbenchKeywordFormKey"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["AmountProduced"].ShouldBe(dtoFields["AmountProduced"]);
        spriggitFields["ComparisonValue"].ShouldBe(dtoFields["ComparisonValue"]);
        spriggitFields["CreatedObject"].ShouldBe(dtoFields["CreatedObjectFormKey"]);
        spriggitFields["Data.FirstParameter"].ShouldBe(dtoFields["Data.FirstParameter"]);
        spriggitFields["Data.MutagenObjectType"].ShouldBe(dtoFields["Data.MutagenObjectType"]);
        spriggitFields["Description.Count"].ShouldBe(dtoFields["Description.Count"]);
        spriggitFields["Description.TargetLanguage"].ShouldBe(dtoFields["Description.TargetLanguage"]);
        spriggitFields["Description[0].Language"].ShouldBe(dtoFields["Description[0].Language"]);
        spriggitFields["Description[0].String"].ShouldBe(dtoFields["Description[0].String"]);
        spriggitFields["Description[1].Language"].ShouldBe(dtoFields["Description[1].Language"]);
        spriggitFields["Description[1].String"].ShouldBe(dtoFields["Description[1].String"]);
        spriggitFields["Description[2].Language"].ShouldBe(dtoFields["Description[2].Language"]);
        spriggitFields["Description[2].String"].ShouldBe(dtoFields["Description[2].String"]);
        spriggitFields["Description[3].Language"].ShouldBe(dtoFields["Description[3].Language"]);
        spriggitFields["Description[3].String"].ShouldBe(dtoFields["Description[3].String"]);
        spriggitFields["Description[4].Language"].ShouldBe(dtoFields["Description[4].Language"]);
        spriggitFields["Description[4].String"].ShouldBe(dtoFields["Description[4].String"]);
        spriggitFields["Description[5].Language"].ShouldBe(dtoFields["Description[5].Language"]);
        spriggitFields["Description[5].String"].ShouldBe(dtoFields["Description[5].String"]);
        spriggitFields["Description[6].Language"].ShouldBe(dtoFields["Description[6].Language"]);
        spriggitFields["Description[6].String"].ShouldBe(dtoFields["Description[6].String"]);
        spriggitFields["Description[7].Language"].ShouldBe(dtoFields["Description[7].Language"]);
        spriggitFields["Description[7].String"].ShouldBe(dtoFields["Description[7].String"]);
        spriggitFields["Description[8].Language"].ShouldBe(dtoFields["Description[8].Language"]);
        spriggitFields["Description[8].String"].ShouldBe(dtoFields["Description[8].String"]);
        spriggitFields["DropdownSound.Start"].ShouldBe(dtoFields["DropdownSound.Start"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["Flags"].ShouldBe(dtoFields["Flags"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["LearnMethod"].ShouldBe(dtoFields["LearnMethod"]);
        spriggitFields["MenuSortOrder"].ShouldBe(dtoFields["MenuSortOrder"]);
        spriggitFields["PickupSound.Start"].ShouldBe(dtoFields["PickupSound.Start"]);
        spriggitFields["RequiredCount[0]"].ShouldBe(dtoFields["RequiredCount[0]"]);
        spriggitFields["RequiredCount[1]"].ShouldBe(dtoFields["RequiredCount[1]"]);
        spriggitFields["RequiredCount[2]"].ShouldBe(dtoFields["RequiredCount[2]"]);
        spriggitFields["RequiredCount[3]"].ShouldBe(dtoFields["RequiredCount[3]"]);
        spriggitFields["RequiredCount[4]"].ShouldBe(dtoFields["RequiredCount[4]"]);
        spriggitFields["RequiredCount[5]"].ShouldBe(dtoFields["RequiredCount[5]"]);
        spriggitFields["RequiredCount[6]"].ShouldBe(dtoFields["RequiredCount[6]"]);
        spriggitFields["Unknown2"].ShouldBe(dtoFields["Unknown2"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["WorkbenchKeyword"].ShouldBe(dtoFields["WorkbenchKeywordFormKey"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["AmountProduced"].ShouldBe(dtoFields["AmountProduced"]);
        spriggitFields["ComparisonValue"].ShouldBe(dtoFields["ComparisonValue"]);
        spriggitFields["CreatedObject"].ShouldBe(dtoFields["CreatedObjectFormKey"]);
        spriggitFields["Data.FirstParameter[0]"].ShouldBe(dtoFields["Data.FirstParameter[0]"]);
        spriggitFields["Data.FirstParameter[1]"].ShouldBe(dtoFields["Data.FirstParameter[1]"]);
        spriggitFields["Data.MutagenObjectType[0]"].ShouldBe(dtoFields["Data.MutagenObjectType[0]"]);
        spriggitFields["Data.MutagenObjectType[1]"].ShouldBe(dtoFields["Data.MutagenObjectType[1]"]);
        spriggitFields["Data.SecondParameter[0]"].ShouldBe(dtoFields["Data.SecondParameter[0]"]);
        spriggitFields["Data.SecondParameter[1]"].ShouldBe(dtoFields["Data.SecondParameter[1]"]);
        spriggitFields["Description.Count"].ShouldBe(dtoFields["Description.Count"]);
        spriggitFields["Description.TargetLanguage"].ShouldBe(dtoFields["Description.TargetLanguage"]);
        spriggitFields["Description[0].Language"].ShouldBe(dtoFields["Description[0].Language"]);
        spriggitFields["Description[0].String"].ShouldBe(dtoFields["Description[0].String"]);
        spriggitFields["Description[1].Language"].ShouldBe(dtoFields["Description[1].Language"]);
        spriggitFields["Description[1].String"].ShouldBe(dtoFields["Description[1].String"]);
        spriggitFields["Description[2].Language"].ShouldBe(dtoFields["Description[2].Language"]);
        spriggitFields["Description[2].String"].ShouldBe(dtoFields["Description[2].String"]);
        spriggitFields["Description[3].Language"].ShouldBe(dtoFields["Description[3].Language"]);
        spriggitFields["Description[3].String"].ShouldBe(dtoFields["Description[3].String"]);
        spriggitFields["Description[4].Language"].ShouldBe(dtoFields["Description[4].Language"]);
        spriggitFields["Description[4].String"].ShouldBe(dtoFields["Description[4].String"]);
        spriggitFields["Description[5].Language"].ShouldBe(dtoFields["Description[5].Language"]);
        spriggitFields["Description[5].String"].ShouldBe(dtoFields["Description[5].String"]);
        spriggitFields["Description[6].Language"].ShouldBe(dtoFields["Description[6].Language"]);
        spriggitFields["Description[6].String"].ShouldBe(dtoFields["Description[6].String"]);
        spriggitFields["Description[7].Language"].ShouldBe(dtoFields["Description[7].Language"]);
        spriggitFields["Description[7].String"].ShouldBe(dtoFields["Description[7].String"]);
        spriggitFields["Description[8].Language"].ShouldBe(dtoFields["Description[8].Language"]);
        spriggitFields["Description[8].String"].ShouldBe(dtoFields["Description[8].String"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["Flags"].ShouldBe(dtoFields["Flags"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["LearnMethod"].ShouldBe(dtoFields["LearnMethod"]);
        spriggitFields["RequiredCount[0]"].ShouldBe(dtoFields["RequiredCount[0]"]);
        spriggitFields["RequiredCount[1]"].ShouldBe(dtoFields["RequiredCount[1]"]);
        spriggitFields["RequiredCount[2]"].ShouldBe(dtoFields["RequiredCount[2]"]);
        spriggitFields["RequiredCount[3]"].ShouldBe(dtoFields["RequiredCount[3]"]);
        spriggitFields["Unknown2[0]"].ShouldBe(dtoFields["Unknown2[0]"]);
        spriggitFields["Unknown2[1]"].ShouldBe(dtoFields["Unknown2[1]"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["WorkbenchKeyword"].ShouldBe(dtoFields["WorkbenchKeywordFormKey"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["AmountProduced"].ShouldBe(dtoFields["AmountProduced"]);
        spriggitFields["ComparisonValue"].ShouldBe(dtoFields["ComparisonValue"]);
        spriggitFields["CreatedObject"].ShouldBe(dtoFields["CreatedObjectFormKey"]);
        spriggitFields["Data.FirstParameter[0]"].ShouldBe(dtoFields["Data.FirstParameter[0]"]);
        spriggitFields["Data.FirstParameter[1]"].ShouldBe(dtoFields["Data.FirstParameter[1]"]);
        spriggitFields["Data.MutagenObjectType[0]"].ShouldBe(dtoFields["Data.MutagenObjectType[0]"]);
        spriggitFields["Data.MutagenObjectType[1]"].ShouldBe(dtoFields["Data.MutagenObjectType[1]"]);
        spriggitFields["Description.Count"].ShouldBe(dtoFields["Description.Count"]);
        spriggitFields["Description.TargetLanguage"].ShouldBe(dtoFields["Description.TargetLanguage"]);
        spriggitFields["Description[0].Language"].ShouldBe(dtoFields["Description[0].Language"]);
        spriggitFields["Description[0].String"].ShouldBe(dtoFields["Description[0].String"]);
        spriggitFields["Description[1].Language"].ShouldBe(dtoFields["Description[1].Language"]);
        spriggitFields["Description[1].String"].ShouldBe(dtoFields["Description[1].String"]);
        spriggitFields["Description[2].Language"].ShouldBe(dtoFields["Description[2].Language"]);
        spriggitFields["Description[2].String"].ShouldBe(dtoFields["Description[2].String"]);
        spriggitFields["Description[3].Language"].ShouldBe(dtoFields["Description[3].Language"]);
        spriggitFields["Description[3].String"].ShouldBe(dtoFields["Description[3].String"]);
        spriggitFields["Description[4].Language"].ShouldBe(dtoFields["Description[4].Language"]);
        spriggitFields["Description[4].String"].ShouldBe(dtoFields["Description[4].String"]);
        spriggitFields["Description[5].Language"].ShouldBe(dtoFields["Description[5].Language"]);
        spriggitFields["Description[5].String"].ShouldBe(dtoFields["Description[5].String"]);
        spriggitFields["Description[6].Language"].ShouldBe(dtoFields["Description[6].Language"]);
        spriggitFields["Description[6].String"].ShouldBe(dtoFields["Description[6].String"]);
        spriggitFields["Description[7].Language"].ShouldBe(dtoFields["Description[7].Language"]);
        spriggitFields["Description[7].String"].ShouldBe(dtoFields["Description[7].String"]);
        spriggitFields["Description[8].Language"].ShouldBe(dtoFields["Description[8].Language"]);
        spriggitFields["Description[8].String"].ShouldBe(dtoFields["Description[8].String"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["Flags"].ShouldBe(dtoFields["Flags"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["LearnMethod"].ShouldBe(dtoFields["LearnMethod"]);
        spriggitFields["RequiredCount[0]"].ShouldBe(dtoFields["RequiredCount[0]"]);
        spriggitFields["RequiredCount[1]"].ShouldBe(dtoFields["RequiredCount[1]"]);
        spriggitFields["RequiredCount[2]"].ShouldBe(dtoFields["RequiredCount[2]"]);
        spriggitFields["RequiredCount[3]"].ShouldBe(dtoFields["RequiredCount[3]"]);
        spriggitFields["RequiredCount[4]"].ShouldBe(dtoFields["RequiredCount[4]"]);
        spriggitFields["RequiredCount[5]"].ShouldBe(dtoFields["RequiredCount[5]"]);
        spriggitFields["RequiredCount[6]"].ShouldBe(dtoFields["RequiredCount[6]"]);
        spriggitFields["Unknown2[0]"].ShouldBe(dtoFields["Unknown2[0]"]);
        spriggitFields["Unknown2[1]"].ShouldBe(dtoFields["Unknown2[1]"]);
        spriggitFields["Value"].ShouldBe(dtoFields["Value"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["WorkbenchKeyword"].ShouldBe(dtoFields["WorkbenchKeywordFormKey"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["CreatedObject"].ShouldBe(dtoFields["CreatedObjectFormKey"]);
        spriggitFields["Data.MutagenObjectType"].ShouldBe(dtoFields["Data.MutagenObjectType"]);
        spriggitFields["Description.Count"].ShouldBe(dtoFields["Description.Count"]);
        spriggitFields["Description.TargetLanguage"].ShouldBe(dtoFields["Description.TargetLanguage"]);
        spriggitFields["Description[0].Language"].ShouldBe(dtoFields["Description[0].Language"]);
        spriggitFields["Description[0].String"].ShouldBe(dtoFields["Description[0].String"]);
        spriggitFields["Description[1].Language"].ShouldBe(dtoFields["Description[1].Language"]);
        spriggitFields["Description[1].String"].ShouldBe(dtoFields["Description[1].String"]);
        spriggitFields["Description[2].Language"].ShouldBe(dtoFields["Description[2].Language"]);
        spriggitFields["Description[2].String"].ShouldBe(dtoFields["Description[2].String"]);
        spriggitFields["Description[3].Language"].ShouldBe(dtoFields["Description[3].Language"]);
        spriggitFields["Description[3].String"].ShouldBe(dtoFields["Description[3].String"]);
        spriggitFields["Description[4].Language"].ShouldBe(dtoFields["Description[4].Language"]);
        spriggitFields["Description[4].String"].ShouldBe(dtoFields["Description[4].String"]);
        spriggitFields["Description[5].Language"].ShouldBe(dtoFields["Description[5].Language"]);
        spriggitFields["Description[5].String"].ShouldBe(dtoFields["Description[5].String"]);
        spriggitFields["Description[6].Language"].ShouldBe(dtoFields["Description[6].Language"]);
        spriggitFields["Description[6].String"].ShouldBe(dtoFields["Description[6].String"]);
        spriggitFields["Description[7].Language"].ShouldBe(dtoFields["Description[7].Language"]);
        spriggitFields["Description[7].String"].ShouldBe(dtoFields["Description[7].String"]);
        spriggitFields["Description[8].Language"].ShouldBe(dtoFields["Description[8].Language"]);
        spriggitFields["Description[8].String"].ShouldBe(dtoFields["Description[8].String"]);
        spriggitFields["DropdownSound.Start"].ShouldBe(dtoFields["DropdownSound.Start"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["Flags"].ShouldBe(dtoFields["Flags"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["LearnMethod"].ShouldBe(dtoFields["LearnMethod"]);
        spriggitFields["MajorRecordFlagsRaw"].ShouldBe(dtoFields["MajorRecordFlags"]);
        spriggitFields["MenuSortOrder"].ShouldBe(dtoFields["MenuSortOrder"]);
        spriggitFields["PickupSound.Start"].ShouldBe(dtoFields["PickupSound.Start"]);
        spriggitFields["RequiredCount[0]"].ShouldBe(dtoFields["RequiredCount[0]"]);
        spriggitFields["RequiredCount[1]"].ShouldBe(dtoFields["RequiredCount[1]"]);
        spriggitFields["RequiredCount[2]"].ShouldBe(dtoFields["RequiredCount[2]"]);
        spriggitFields["Unknown2"].ShouldBe(dtoFields["Unknown2"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["WorkbenchKeyword"].ShouldBe(dtoFields["WorkbenchKeywordFormKey"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }
}
