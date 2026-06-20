using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.ConstructibleObject.Fallout4;

public class Fallout4ConstructibleObjectSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "COBJ")]
    [Trait("FormKey", "0ADF6E:Fallout4.esm")]
    [Trait("EditorID", "workshop_co_Artillery")]
    [Trait("SpriggitFile", "ConstructibleObjects/workshop_co_Artillery - 0ADF6E_Fallout4.esm.yaml")]
    public void Fallout4_COBJ_ShouldMatchSpriggitSample_workshop_co_Artillery()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.ConstructibleObject,
            "workshop_co_Artillery");
        var dto = Helpers.GetDTO<ConstructibleObjectDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.ConstructibleObject,
            "0ADF6E:Fallout4.esm");

        Helpers.GetSpriggitField(spriggit, "ComparisonValue[0]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[0]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[1]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[1]"));
        Helpers.GetSpriggitField(spriggit, "Count[0]").ShouldBe(Helpers.GetDTOField(dto, "Count[0]"));
        Helpers.GetSpriggitField(spriggit, "Count[1]").ShouldBe(Helpers.GetDTOField(dto, "Count[1]"));
        Helpers.GetSpriggitField(spriggit, "Count[2]").ShouldBe(Helpers.GetDTOField(dto, "Count[2]"));
        Helpers.GetSpriggitField(spriggit, "Count[3]").ShouldBe(Helpers.GetDTOField(dto, "Count[3]"));
        Helpers.GetSpriggitField(spriggit, "Count[4]").ShouldBe(Helpers.GetDTOField(dto, "Count[4]"));
        Helpers.GetSpriggitField(spriggit, "Count[5]").ShouldBe(Helpers.GetDTOField(dto, "Count[5]"));
        Helpers.GetSpriggitField(spriggit, "Count[6]").ShouldBe(Helpers.GetDTOField(dto, "Count[6]"));
        Helpers.GetSpriggitField(spriggit, "CreatedObject").ShouldBe(Helpers.GetDTOField(dto, "CreatedObjectFormKey"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[1]"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[2]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[2]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[1]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[2]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[2]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneNumber[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneNumber[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneNumber[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneNumber[1]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneNumber[2]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneNumber[2]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneRecord[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneRecord[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneRecord[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneRecord[1]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneRecord[2]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneRecord[2]"));
        Helpers.GetSpriggitField(spriggit, "Data.Reference").ShouldBe(Helpers.GetDTOField(dto, "Data.Reference"));
        Helpers.GetSpriggitField(spriggit, "Data.RunOnType").ShouldBe(Helpers.GetDTOField(dto, "Data.RunOnType"));
        Helpers.GetSpriggitField(spriggit, "Description.Count").ShouldBe(Helpers.GetDTOField(dto, "Description.Count"));
        Helpers.GetSpriggitField(spriggit, "Description.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Description.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Description[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[0].String").ShouldBe(Helpers.GetDTOField(dto, "Description[0].String"));
        Helpers.GetSpriggitField(spriggit, "Description[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[1].String").ShouldBe(Helpers.GetDTOField(dto, "Description[1].String"));
        Helpers.GetSpriggitField(spriggit, "Description[10].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[10].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[10].String").ShouldBe(Helpers.GetDTOField(dto, "Description[10].String"));
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
        Helpers.GetSpriggitField(spriggit, "Description[9].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[9].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[9].String").ShouldBe(Helpers.GetDTOField(dto, "Description[9].String"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));
        Helpers.GetSpriggitField(spriggit, "WorkbenchKeyword").ShouldBe(Helpers.GetDTOField(dto, "WorkbenchKeywordFormKey"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "ComparisonValue[0]", "ComparisonValue[1]", "Count[0]", "Count[1]", "Count[2]", "Count[3]", "Count[4]", "Count[5]", "Count[6]", "CreatedObject", "Data.Function[0]", "Data.Function[1]", "Data.Function[2]", "Data.MutagenObjectType[0]", "Data.MutagenObjectType[1]", "Data.MutagenObjectType[2]", "Data.ParameterOneNumber[0]", "Data.ParameterOneNumber[1]", "Data.ParameterOneNumber[2]", "Data.ParameterOneRecord[0]", "Data.ParameterOneRecord[1]", "Data.ParameterOneRecord[2]", "Data.Reference", "Data.RunOnType", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[10].Language", "Description[10].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "Description[9].Language", "Description[9].String", "EditorID", "FormKey", "Version2", "VersionControl", "WorkbenchKeyword");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "ComparisonValue[0]", "ComparisonValue[1]", "Count[0]", "Count[1]", "Count[2]", "Count[3]", "Count[4]", "Count[5]", "Count[6]", "CreatedObjectFormKey", "Data.Function[0]", "Data.Function[1]", "Data.Function[2]", "Data.MutagenObjectType[0]", "Data.MutagenObjectType[1]", "Data.MutagenObjectType[2]", "Data.ParameterOneNumber[0]", "Data.ParameterOneNumber[1]", "Data.ParameterOneNumber[2]", "Data.ParameterOneRecord[0]", "Data.ParameterOneRecord[1]", "Data.ParameterOneRecord[2]", "Data.Reference", "Data.RunOnType", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[10].Language", "Description[10].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "Description[9].Language", "Description[9].String", "EditorID", "FormKey", "Version2", "VersionControl", "WorkbenchKeywordFormKey");
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "COBJ")]
    [Trait("FormKey", "0CEA6F:Fallout4.esm")]
    [Trait("EditorID", "workshop_co_MQ206BeamEmitter")]
    [Trait("SpriggitFile", "ConstructibleObjects/workshop_co_MQ206BeamEmitter - 0CEA6F_Fallout4.esm.yaml")]
    public void Fallout4_COBJ_ShouldMatchSpriggitSample_workshop_co_MQ206BeamEmitter()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.ConstructibleObject,
            "workshop_co_MQ206BeamEmitter");
        var dto = Helpers.GetDTO<ConstructibleObjectDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.ConstructibleObject,
            "0CEA6F:Fallout4.esm");

        Helpers.GetSpriggitField(spriggit, "CompareOperator").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[0]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[0]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[1]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[1]"));
        Helpers.GetSpriggitField(spriggit, "Count[0]").ShouldBe(Helpers.GetDTOField(dto, "Count[0]"));
        Helpers.GetSpriggitField(spriggit, "Count[1]").ShouldBe(Helpers.GetDTOField(dto, "Count[1]"));
        Helpers.GetSpriggitField(spriggit, "Count[2]").ShouldBe(Helpers.GetDTOField(dto, "Count[2]"));
        Helpers.GetSpriggitField(spriggit, "Count[3]").ShouldBe(Helpers.GetDTOField(dto, "Count[3]"));
        Helpers.GetSpriggitField(spriggit, "Count[4]").ShouldBe(Helpers.GetDTOField(dto, "Count[4]"));
        Helpers.GetSpriggitField(spriggit, "CreatedObject").ShouldBe(Helpers.GetDTOField(dto, "CreatedObjectFormKey"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[1]"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[2]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[2]"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[3]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[3]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[1]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[2]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[2]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[3]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[3]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneNumber[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneNumber[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneNumber[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneNumber[1]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneNumber[2]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneNumber[2]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneNumber[3]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneNumber[3]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneRecord[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneRecord[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneRecord[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneRecord[1]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneRecord[2]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneRecord[2]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneRecord[3]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneRecord[3]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterTwoNumber").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterTwoNumber"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterTwoRecord").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterTwoRecord"));
        Helpers.GetSpriggitField(spriggit, "Data.Reference").ShouldBe(Helpers.GetDTOField(dto, "Data.Reference"));
        Helpers.GetSpriggitField(spriggit, "Data.RunOnType").ShouldBe(Helpers.GetDTOField(dto, "Data.RunOnType"));
        Helpers.GetSpriggitField(spriggit, "Description.Count").ShouldBe(Helpers.GetDTOField(dto, "Description.Count"));
        Helpers.GetSpriggitField(spriggit, "Description.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Description.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Description[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[0].String").ShouldBe(Helpers.GetDTOField(dto, "Description[0].String"));
        Helpers.GetSpriggitField(spriggit, "Description[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[1].String").ShouldBe(Helpers.GetDTOField(dto, "Description[1].String"));
        Helpers.GetSpriggitField(spriggit, "Description[10].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[10].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[10].String").ShouldBe(Helpers.GetDTOField(dto, "Description[10].String"));
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
        Helpers.GetSpriggitField(spriggit, "Description[9].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[9].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[9].String").ShouldBe(Helpers.GetDTOField(dto, "Description[9].String"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "PickUpSound").ShouldBe(Helpers.GetDTOField(dto, "PickUpSound"));
        Helpers.GetSpriggitField(spriggit, "PutDownSound").ShouldBe(Helpers.GetDTOField(dto, "PutDownSound"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));
        Helpers.GetSpriggitField(spriggit, "WorkbenchKeyword").ShouldBe(Helpers.GetDTOField(dto, "WorkbenchKeywordFormKey"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "CompareOperator", "ComparisonValue[0]", "ComparisonValue[1]", "Count[0]", "Count[1]", "Count[2]", "Count[3]", "Count[4]", "CreatedObject", "Data.Function[0]", "Data.Function[1]", "Data.Function[2]", "Data.Function[3]", "Data.MutagenObjectType[0]", "Data.MutagenObjectType[1]", "Data.MutagenObjectType[2]", "Data.MutagenObjectType[3]", "Data.ParameterOneNumber[0]", "Data.ParameterOneNumber[1]", "Data.ParameterOneNumber[2]", "Data.ParameterOneNumber[3]", "Data.ParameterOneRecord[0]", "Data.ParameterOneRecord[1]", "Data.ParameterOneRecord[2]", "Data.ParameterOneRecord[3]", "Data.ParameterTwoNumber", "Data.ParameterTwoRecord", "Data.Reference", "Data.RunOnType", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[10].Language", "Description[10].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "Description[9].Language", "Description[9].String", "EditorID", "FormKey", "PickUpSound", "PutDownSound", "Version2", "VersionControl", "WorkbenchKeyword");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "CompareOperator", "ComparisonValue[0]", "ComparisonValue[1]", "Count[0]", "Count[1]", "Count[2]", "Count[3]", "Count[4]", "CreatedObjectFormKey", "Data.Function[0]", "Data.Function[1]", "Data.Function[2]", "Data.Function[3]", "Data.MutagenObjectType[0]", "Data.MutagenObjectType[1]", "Data.MutagenObjectType[2]", "Data.MutagenObjectType[3]", "Data.ParameterOneNumber[0]", "Data.ParameterOneNumber[1]", "Data.ParameterOneNumber[2]", "Data.ParameterOneNumber[3]", "Data.ParameterOneRecord[0]", "Data.ParameterOneRecord[1]", "Data.ParameterOneRecord[2]", "Data.ParameterOneRecord[3]", "Data.ParameterTwoNumber", "Data.ParameterTwoRecord", "Data.Reference", "Data.RunOnType", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[10].Language", "Description[10].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "Description[9].Language", "Description[9].String", "EditorID", "FormKey", "PickUpSound", "PutDownSound", "Version2", "VersionControl", "WorkbenchKeywordFormKey");
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "COBJ")]
    [Trait("FormKey", "0CEA7B:Fallout4.esm")]
    [Trait("EditorID", "workshop_co_MQ206Console")]
    [Trait("SpriggitFile", "ConstructibleObjects/workshop_co_MQ206Console - 0CEA7B_Fallout4.esm.yaml")]
    public void Fallout4_COBJ_ShouldMatchSpriggitSample_workshop_co_MQ206Console()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.ConstructibleObject,
            "workshop_co_MQ206Console");
        var dto = Helpers.GetDTO<ConstructibleObjectDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.ConstructibleObject,
            "0CEA7B:Fallout4.esm");

        Helpers.GetSpriggitField(spriggit, "CompareOperator").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[0]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[0]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[1]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[1]"));
        Helpers.GetSpriggitField(spriggit, "Count[0]").ShouldBe(Helpers.GetDTOField(dto, "Count[0]"));
        Helpers.GetSpriggitField(spriggit, "Count[1]").ShouldBe(Helpers.GetDTOField(dto, "Count[1]"));
        Helpers.GetSpriggitField(spriggit, "Count[2]").ShouldBe(Helpers.GetDTOField(dto, "Count[2]"));
        Helpers.GetSpriggitField(spriggit, "Count[3]").ShouldBe(Helpers.GetDTOField(dto, "Count[3]"));
        Helpers.GetSpriggitField(spriggit, "CreatedObject").ShouldBe(Helpers.GetDTOField(dto, "CreatedObjectFormKey"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[1]"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[2]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[2]"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[3]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[3]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[1]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[2]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[2]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[3]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[3]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneNumber[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneNumber[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneNumber[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneNumber[1]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneNumber[2]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneNumber[2]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneNumber[3]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneNumber[3]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneRecord[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneRecord[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneRecord[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneRecord[1]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneRecord[2]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneRecord[2]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneRecord[3]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneRecord[3]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterTwoNumber").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterTwoNumber"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterTwoRecord").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterTwoRecord"));
        Helpers.GetSpriggitField(spriggit, "Data.Reference").ShouldBe(Helpers.GetDTOField(dto, "Data.Reference"));
        Helpers.GetSpriggitField(spriggit, "Data.RunOnType").ShouldBe(Helpers.GetDTOField(dto, "Data.RunOnType"));
        Helpers.GetSpriggitField(spriggit, "Description.Count").ShouldBe(Helpers.GetDTOField(dto, "Description.Count"));
        Helpers.GetSpriggitField(spriggit, "Description.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Description.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Description[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[0].String").ShouldBe(Helpers.GetDTOField(dto, "Description[0].String"));
        Helpers.GetSpriggitField(spriggit, "Description[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[1].String").ShouldBe(Helpers.GetDTOField(dto, "Description[1].String"));
        Helpers.GetSpriggitField(spriggit, "Description[10].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[10].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[10].String").ShouldBe(Helpers.GetDTOField(dto, "Description[10].String"));
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
        Helpers.GetSpriggitField(spriggit, "Description[9].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[9].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[9].String").ShouldBe(Helpers.GetDTOField(dto, "Description[9].String"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "PickUpSound").ShouldBe(Helpers.GetDTOField(dto, "PickUpSound"));
        Helpers.GetSpriggitField(spriggit, "PutDownSound").ShouldBe(Helpers.GetDTOField(dto, "PutDownSound"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));
        Helpers.GetSpriggitField(spriggit, "WorkbenchKeyword").ShouldBe(Helpers.GetDTOField(dto, "WorkbenchKeywordFormKey"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "CompareOperator", "ComparisonValue[0]", "ComparisonValue[1]", "Count[0]", "Count[1]", "Count[2]", "Count[3]", "CreatedObject", "Data.Function[0]", "Data.Function[1]", "Data.Function[2]", "Data.Function[3]", "Data.MutagenObjectType[0]", "Data.MutagenObjectType[1]", "Data.MutagenObjectType[2]", "Data.MutagenObjectType[3]", "Data.ParameterOneNumber[0]", "Data.ParameterOneNumber[1]", "Data.ParameterOneNumber[2]", "Data.ParameterOneNumber[3]", "Data.ParameterOneRecord[0]", "Data.ParameterOneRecord[1]", "Data.ParameterOneRecord[2]", "Data.ParameterOneRecord[3]", "Data.ParameterTwoNumber", "Data.ParameterTwoRecord", "Data.Reference", "Data.RunOnType", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[10].Language", "Description[10].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "Description[9].Language", "Description[9].String", "EditorID", "FormKey", "PickUpSound", "PutDownSound", "Version2", "VersionControl", "WorkbenchKeyword");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "CompareOperator", "ComparisonValue[0]", "ComparisonValue[1]", "Count[0]", "Count[1]", "Count[2]", "Count[3]", "CreatedObjectFormKey", "Data.Function[0]", "Data.Function[1]", "Data.Function[2]", "Data.Function[3]", "Data.MutagenObjectType[0]", "Data.MutagenObjectType[1]", "Data.MutagenObjectType[2]", "Data.MutagenObjectType[3]", "Data.ParameterOneNumber[0]", "Data.ParameterOneNumber[1]", "Data.ParameterOneNumber[2]", "Data.ParameterOneNumber[3]", "Data.ParameterOneRecord[0]", "Data.ParameterOneRecord[1]", "Data.ParameterOneRecord[2]", "Data.ParameterOneRecord[3]", "Data.ParameterTwoNumber", "Data.ParameterTwoRecord", "Data.Reference", "Data.RunOnType", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[10].Language", "Description[10].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "Description[9].Language", "Description[9].String", "EditorID", "FormKey", "PickUpSound", "PutDownSound", "Version2", "VersionControl", "WorkbenchKeywordFormKey");
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "COBJ")]
    [Trait("FormKey", "05A0CD:Fallout4.esm")]
    [Trait("EditorID", "workshop_co_WaterPurifier")]
    [Trait("SpriggitFile", "ConstructibleObjects/workshop_co_WaterPurifier - 05A0CD_Fallout4.esm.yaml")]
    public void Fallout4_COBJ_ShouldMatchSpriggitSample_workshop_co_WaterPurifier()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.ConstructibleObject,
            "workshop_co_WaterPurifier");
        var dto = Helpers.GetDTO<ConstructibleObjectDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.ConstructibleObject,
            "05A0CD:Fallout4.esm");

        Helpers.GetSpriggitField(spriggit, "Count[0]").ShouldBe(Helpers.GetDTOField(dto, "Count[0]"));
        Helpers.GetSpriggitField(spriggit, "Count[1]").ShouldBe(Helpers.GetDTOField(dto, "Count[1]"));
        Helpers.GetSpriggitField(spriggit, "Count[2]").ShouldBe(Helpers.GetDTOField(dto, "Count[2]"));
        Helpers.GetSpriggitField(spriggit, "Count[3]").ShouldBe(Helpers.GetDTOField(dto, "Count[3]"));
        Helpers.GetSpriggitField(spriggit, "Count[4]").ShouldBe(Helpers.GetDTOField(dto, "Count[4]"));
        Helpers.GetSpriggitField(spriggit, "Count[5]").ShouldBe(Helpers.GetDTOField(dto, "Count[5]"));
        Helpers.GetSpriggitField(spriggit, "CreatedObject").ShouldBe(Helpers.GetDTOField(dto, "CreatedObjectFormKey"));
        Helpers.GetSpriggitField(spriggit, "Description.Count").ShouldBe(Helpers.GetDTOField(dto, "Description.Count"));
        Helpers.GetSpriggitField(spriggit, "Description.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Description.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Description[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[0].String").ShouldBe(Helpers.GetDTOField(dto, "Description[0].String"));
        Helpers.GetSpriggitField(spriggit, "Description[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[1].String").ShouldBe(Helpers.GetDTOField(dto, "Description[1].String"));
        Helpers.GetSpriggitField(spriggit, "Description[10].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[10].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[10].String").ShouldBe(Helpers.GetDTOField(dto, "Description[10].String"));
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
        Helpers.GetSpriggitField(spriggit, "Description[9].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[9].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[9].String").ShouldBe(Helpers.GetDTOField(dto, "Description[9].String"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "PickUpSound").ShouldBe(Helpers.GetDTOField(dto, "PickUpSound"));
        Helpers.GetSpriggitField(spriggit, "PutDownSound").ShouldBe(Helpers.GetDTOField(dto, "PutDownSound"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));
        Helpers.GetSpriggitField(spriggit, "WorkbenchKeyword").ShouldBe(Helpers.GetDTOField(dto, "WorkbenchKeywordFormKey"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "Count[0]", "Count[1]", "Count[2]", "Count[3]", "Count[4]", "Count[5]", "CreatedObject", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[10].Language", "Description[10].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "Description[9].Language", "Description[9].String", "EditorID", "FormKey", "PickUpSound", "PutDownSound", "Version2", "VersionControl", "WorkbenchKeyword");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "Count[0]", "Count[1]", "Count[2]", "Count[3]", "Count[4]", "Count[5]", "CreatedObjectFormKey", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[10].Language", "Description[10].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "Description[9].Language", "Description[9].String", "EditorID", "FormKey", "PickUpSound", "PutDownSound", "Version2", "VersionControl", "WorkbenchKeywordFormKey");
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "COBJ")]
    [Trait("FormKey", "1889E3:Fallout4.esm")]
    [Trait("EditorID", "co_mod_GatlingLaser_BarrelMingunLaser_Super")]
    [Trait("SpriggitFile", "ConstructibleObjects/co_mod_GatlingLaser_BarrelMingunLaser_Super - 1889E3_Fallout4.esm.yaml")]
    public void Fallout4_COBJ_ShouldMatchSpriggitSample_co_mod_GatlingLaser_BarrelMingunLaser_Super()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.ConstructibleObject,
            "co_mod_GatlingLaser_BarrelMingunLaser_Super");
        var dto = Helpers.GetDTO<ConstructibleObjectDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.ConstructibleObject,
            "1889E3:Fallout4.esm");

        Helpers.GetSpriggitField(spriggit, "ComparisonValue").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue"));
        Helpers.GetSpriggitField(spriggit, "Count[0]").ShouldBe(Helpers.GetDTOField(dto, "Count[0]"));
        Helpers.GetSpriggitField(spriggit, "Count[1]").ShouldBe(Helpers.GetDTOField(dto, "Count[1]"));
        Helpers.GetSpriggitField(spriggit, "Count[10]").ShouldBe(Helpers.GetDTOField(dto, "Count[10]"));
        Helpers.GetSpriggitField(spriggit, "Count[2]").ShouldBe(Helpers.GetDTOField(dto, "Count[2]"));
        Helpers.GetSpriggitField(spriggit, "Count[3]").ShouldBe(Helpers.GetDTOField(dto, "Count[3]"));
        Helpers.GetSpriggitField(spriggit, "Count[4]").ShouldBe(Helpers.GetDTOField(dto, "Count[4]"));
        Helpers.GetSpriggitField(spriggit, "Count[5]").ShouldBe(Helpers.GetDTOField(dto, "Count[5]"));
        Helpers.GetSpriggitField(spriggit, "Count[6]").ShouldBe(Helpers.GetDTOField(dto, "Count[6]"));
        Helpers.GetSpriggitField(spriggit, "Count[7]").ShouldBe(Helpers.GetDTOField(dto, "Count[7]"));
        Helpers.GetSpriggitField(spriggit, "Count[8]").ShouldBe(Helpers.GetDTOField(dto, "Count[8]"));
        Helpers.GetSpriggitField(spriggit, "Count[9]").ShouldBe(Helpers.GetDTOField(dto, "Count[9]"));
        Helpers.GetSpriggitField(spriggit, "CreatedObject").ShouldBe(Helpers.GetDTOField(dto, "CreatedObjectFormKey"));
        Helpers.GetSpriggitField(spriggit, "Data.Function").ShouldBe(Helpers.GetDTOField(dto, "Data.Function"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneNumber").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneNumber"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneRecord").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneRecord"));
        Helpers.GetSpriggitField(spriggit, "Description.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Description.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "ComparisonValue", "Count[0]", "Count[1]", "Count[10]", "Count[2]", "Count[3]", "Count[4]", "Count[5]", "Count[6]", "Count[7]", "Count[8]", "Count[9]", "CreatedObject", "Data.Function", "Data.MutagenObjectType", "Data.ParameterOneNumber", "Data.ParameterOneRecord", "Description.TargetLanguage", "EditorID", "FormKey", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "ComparisonValue", "Count[0]", "Count[1]", "Count[10]", "Count[2]", "Count[3]", "Count[4]", "Count[5]", "Count[6]", "Count[7]", "Count[8]", "Count[9]", "CreatedObjectFormKey", "Data.Function", "Data.MutagenObjectType", "Data.ParameterOneNumber", "Data.ParameterOneRecord", "Description.TargetLanguage", "EditorID", "FormKey", "Version2", "VersionControl");
    }
}