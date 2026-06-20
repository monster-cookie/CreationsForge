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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["ComparisonValue[0]"].ShouldBe(dtoFields["ComparisonValue[0]"]);
        spriggitFields["ComparisonValue[1]"].ShouldBe(dtoFields["ComparisonValue[1]"]);
        spriggitFields["Count[0]"].ShouldBe(dtoFields["Count[0]"]);
        spriggitFields["Count[1]"].ShouldBe(dtoFields["Count[1]"]);
        spriggitFields["Count[2]"].ShouldBe(dtoFields["Count[2]"]);
        spriggitFields["Count[3]"].ShouldBe(dtoFields["Count[3]"]);
        spriggitFields["Count[4]"].ShouldBe(dtoFields["Count[4]"]);
        spriggitFields["Count[5]"].ShouldBe(dtoFields["Count[5]"]);
        spriggitFields["Count[6]"].ShouldBe(dtoFields["Count[6]"]);
        spriggitFields["CreatedObject"].ShouldBe(dtoFields["CreatedObjectFormKey"]);
        spriggitFields["Data.Function[0]"].ShouldBe(dtoFields["Data.Function[0]"]);
        spriggitFields["Data.Function[1]"].ShouldBe(dtoFields["Data.Function[1]"]);
        spriggitFields["Data.Function[2]"].ShouldBe(dtoFields["Data.Function[2]"]);
        spriggitFields["Data.MutagenObjectType[0]"].ShouldBe(dtoFields["Data.MutagenObjectType[0]"]);
        spriggitFields["Data.MutagenObjectType[1]"].ShouldBe(dtoFields["Data.MutagenObjectType[1]"]);
        spriggitFields["Data.MutagenObjectType[2]"].ShouldBe(dtoFields["Data.MutagenObjectType[2]"]);
        spriggitFields["Data.ParameterOneNumber[0]"].ShouldBe(dtoFields["Data.ParameterOneNumber[0]"]);
        spriggitFields["Data.ParameterOneNumber[1]"].ShouldBe(dtoFields["Data.ParameterOneNumber[1]"]);
        spriggitFields["Data.ParameterOneNumber[2]"].ShouldBe(dtoFields["Data.ParameterOneNumber[2]"]);
        spriggitFields["Data.ParameterOneRecord[0]"].ShouldBe(dtoFields["Data.ParameterOneRecord[0]"]);
        spriggitFields["Data.ParameterOneRecord[1]"].ShouldBe(dtoFields["Data.ParameterOneRecord[1]"]);
        spriggitFields["Data.ParameterOneRecord[2]"].ShouldBe(dtoFields["Data.ParameterOneRecord[2]"]);
        spriggitFields["Data.Reference"].ShouldBe(dtoFields["Data.Reference"]);
        spriggitFields["Data.RunOnType"].ShouldBe(dtoFields["Data.RunOnType"]);
        spriggitFields["Description.Count"].ShouldBe(dtoFields["Description.Count"]);
        spriggitFields["Description.TargetLanguage"].ShouldBe(dtoFields["Description.TargetLanguage"]);
        spriggitFields["Description[0].Language"].ShouldBe(dtoFields["Description[0].Language"]);
        spriggitFields["Description[0].String"].ShouldBe(dtoFields["Description[0].String"]);
        spriggitFields["Description[1].Language"].ShouldBe(dtoFields["Description[1].Language"]);
        spriggitFields["Description[1].String"].ShouldBe(dtoFields["Description[1].String"]);
        spriggitFields["Description[10].Language"].ShouldBe(dtoFields["Description[10].Language"]);
        spriggitFields["Description[10].String"].ShouldBe(dtoFields["Description[10].String"]);
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
        spriggitFields["Description[9].Language"].ShouldBe(dtoFields["Description[9].Language"]);
        spriggitFields["Description[9].String"].ShouldBe(dtoFields["Description[9].String"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["WorkbenchKeyword"].ShouldBe(dtoFields["WorkbenchKeywordFormKey"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["CompareOperator"].ShouldBe(dtoFields["CompareOperator"]);
        spriggitFields["ComparisonValue[0]"].ShouldBe(dtoFields["ComparisonValue[0]"]);
        spriggitFields["ComparisonValue[1]"].ShouldBe(dtoFields["ComparisonValue[1]"]);
        spriggitFields["Count[0]"].ShouldBe(dtoFields["Count[0]"]);
        spriggitFields["Count[1]"].ShouldBe(dtoFields["Count[1]"]);
        spriggitFields["Count[2]"].ShouldBe(dtoFields["Count[2]"]);
        spriggitFields["Count[3]"].ShouldBe(dtoFields["Count[3]"]);
        spriggitFields["Count[4]"].ShouldBe(dtoFields["Count[4]"]);
        spriggitFields["CreatedObject"].ShouldBe(dtoFields["CreatedObjectFormKey"]);
        spriggitFields["Data.Function[0]"].ShouldBe(dtoFields["Data.Function[0]"]);
        spriggitFields["Data.Function[1]"].ShouldBe(dtoFields["Data.Function[1]"]);
        spriggitFields["Data.Function[2]"].ShouldBe(dtoFields["Data.Function[2]"]);
        spriggitFields["Data.Function[3]"].ShouldBe(dtoFields["Data.Function[3]"]);
        spriggitFields["Data.MutagenObjectType[0]"].ShouldBe(dtoFields["Data.MutagenObjectType[0]"]);
        spriggitFields["Data.MutagenObjectType[1]"].ShouldBe(dtoFields["Data.MutagenObjectType[1]"]);
        spriggitFields["Data.MutagenObjectType[2]"].ShouldBe(dtoFields["Data.MutagenObjectType[2]"]);
        spriggitFields["Data.MutagenObjectType[3]"].ShouldBe(dtoFields["Data.MutagenObjectType[3]"]);
        spriggitFields["Data.ParameterOneNumber[0]"].ShouldBe(dtoFields["Data.ParameterOneNumber[0]"]);
        spriggitFields["Data.ParameterOneNumber[1]"].ShouldBe(dtoFields["Data.ParameterOneNumber[1]"]);
        spriggitFields["Data.ParameterOneNumber[2]"].ShouldBe(dtoFields["Data.ParameterOneNumber[2]"]);
        spriggitFields["Data.ParameterOneNumber[3]"].ShouldBe(dtoFields["Data.ParameterOneNumber[3]"]);
        spriggitFields["Data.ParameterOneRecord[0]"].ShouldBe(dtoFields["Data.ParameterOneRecord[0]"]);
        spriggitFields["Data.ParameterOneRecord[1]"].ShouldBe(dtoFields["Data.ParameterOneRecord[1]"]);
        spriggitFields["Data.ParameterOneRecord[2]"].ShouldBe(dtoFields["Data.ParameterOneRecord[2]"]);
        spriggitFields["Data.ParameterOneRecord[3]"].ShouldBe(dtoFields["Data.ParameterOneRecord[3]"]);
        spriggitFields["Data.ParameterTwoNumber"].ShouldBe(dtoFields["Data.ParameterTwoNumber"]);
        spriggitFields["Data.ParameterTwoRecord"].ShouldBe(dtoFields["Data.ParameterTwoRecord"]);
        spriggitFields["Data.Reference"].ShouldBe(dtoFields["Data.Reference"]);
        spriggitFields["Data.RunOnType"].ShouldBe(dtoFields["Data.RunOnType"]);
        spriggitFields["Description.Count"].ShouldBe(dtoFields["Description.Count"]);
        spriggitFields["Description.TargetLanguage"].ShouldBe(dtoFields["Description.TargetLanguage"]);
        spriggitFields["Description[0].Language"].ShouldBe(dtoFields["Description[0].Language"]);
        spriggitFields["Description[0].String"].ShouldBe(dtoFields["Description[0].String"]);
        spriggitFields["Description[1].Language"].ShouldBe(dtoFields["Description[1].Language"]);
        spriggitFields["Description[1].String"].ShouldBe(dtoFields["Description[1].String"]);
        spriggitFields["Description[10].Language"].ShouldBe(dtoFields["Description[10].Language"]);
        spriggitFields["Description[10].String"].ShouldBe(dtoFields["Description[10].String"]);
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
        spriggitFields["Description[9].Language"].ShouldBe(dtoFields["Description[9].Language"]);
        spriggitFields["Description[9].String"].ShouldBe(dtoFields["Description[9].String"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["PickUpSound"].ShouldBe(dtoFields["PickUpSound"]);
        spriggitFields["PutDownSound"].ShouldBe(dtoFields["PutDownSound"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["WorkbenchKeyword"].ShouldBe(dtoFields["WorkbenchKeywordFormKey"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["CompareOperator"].ShouldBe(dtoFields["CompareOperator"]);
        spriggitFields["ComparisonValue[0]"].ShouldBe(dtoFields["ComparisonValue[0]"]);
        spriggitFields["ComparisonValue[1]"].ShouldBe(dtoFields["ComparisonValue[1]"]);
        spriggitFields["Count[0]"].ShouldBe(dtoFields["Count[0]"]);
        spriggitFields["Count[1]"].ShouldBe(dtoFields["Count[1]"]);
        spriggitFields["Count[2]"].ShouldBe(dtoFields["Count[2]"]);
        spriggitFields["Count[3]"].ShouldBe(dtoFields["Count[3]"]);
        spriggitFields["CreatedObject"].ShouldBe(dtoFields["CreatedObjectFormKey"]);
        spriggitFields["Data.Function[0]"].ShouldBe(dtoFields["Data.Function[0]"]);
        spriggitFields["Data.Function[1]"].ShouldBe(dtoFields["Data.Function[1]"]);
        spriggitFields["Data.Function[2]"].ShouldBe(dtoFields["Data.Function[2]"]);
        spriggitFields["Data.Function[3]"].ShouldBe(dtoFields["Data.Function[3]"]);
        spriggitFields["Data.MutagenObjectType[0]"].ShouldBe(dtoFields["Data.MutagenObjectType[0]"]);
        spriggitFields["Data.MutagenObjectType[1]"].ShouldBe(dtoFields["Data.MutagenObjectType[1]"]);
        spriggitFields["Data.MutagenObjectType[2]"].ShouldBe(dtoFields["Data.MutagenObjectType[2]"]);
        spriggitFields["Data.MutagenObjectType[3]"].ShouldBe(dtoFields["Data.MutagenObjectType[3]"]);
        spriggitFields["Data.ParameterOneNumber[0]"].ShouldBe(dtoFields["Data.ParameterOneNumber[0]"]);
        spriggitFields["Data.ParameterOneNumber[1]"].ShouldBe(dtoFields["Data.ParameterOneNumber[1]"]);
        spriggitFields["Data.ParameterOneNumber[2]"].ShouldBe(dtoFields["Data.ParameterOneNumber[2]"]);
        spriggitFields["Data.ParameterOneNumber[3]"].ShouldBe(dtoFields["Data.ParameterOneNumber[3]"]);
        spriggitFields["Data.ParameterOneRecord[0]"].ShouldBe(dtoFields["Data.ParameterOneRecord[0]"]);
        spriggitFields["Data.ParameterOneRecord[1]"].ShouldBe(dtoFields["Data.ParameterOneRecord[1]"]);
        spriggitFields["Data.ParameterOneRecord[2]"].ShouldBe(dtoFields["Data.ParameterOneRecord[2]"]);
        spriggitFields["Data.ParameterOneRecord[3]"].ShouldBe(dtoFields["Data.ParameterOneRecord[3]"]);
        spriggitFields["Data.ParameterTwoNumber"].ShouldBe(dtoFields["Data.ParameterTwoNumber"]);
        spriggitFields["Data.ParameterTwoRecord"].ShouldBe(dtoFields["Data.ParameterTwoRecord"]);
        spriggitFields["Data.Reference"].ShouldBe(dtoFields["Data.Reference"]);
        spriggitFields["Data.RunOnType"].ShouldBe(dtoFields["Data.RunOnType"]);
        spriggitFields["Description.Count"].ShouldBe(dtoFields["Description.Count"]);
        spriggitFields["Description.TargetLanguage"].ShouldBe(dtoFields["Description.TargetLanguage"]);
        spriggitFields["Description[0].Language"].ShouldBe(dtoFields["Description[0].Language"]);
        spriggitFields["Description[0].String"].ShouldBe(dtoFields["Description[0].String"]);
        spriggitFields["Description[1].Language"].ShouldBe(dtoFields["Description[1].Language"]);
        spriggitFields["Description[1].String"].ShouldBe(dtoFields["Description[1].String"]);
        spriggitFields["Description[10].Language"].ShouldBe(dtoFields["Description[10].Language"]);
        spriggitFields["Description[10].String"].ShouldBe(dtoFields["Description[10].String"]);
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
        spriggitFields["Description[9].Language"].ShouldBe(dtoFields["Description[9].Language"]);
        spriggitFields["Description[9].String"].ShouldBe(dtoFields["Description[9].String"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["PickUpSound"].ShouldBe(dtoFields["PickUpSound"]);
        spriggitFields["PutDownSound"].ShouldBe(dtoFields["PutDownSound"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["WorkbenchKeyword"].ShouldBe(dtoFields["WorkbenchKeywordFormKey"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["Count[0]"].ShouldBe(dtoFields["Count[0]"]);
        spriggitFields["Count[1]"].ShouldBe(dtoFields["Count[1]"]);
        spriggitFields["Count[2]"].ShouldBe(dtoFields["Count[2]"]);
        spriggitFields["Count[3]"].ShouldBe(dtoFields["Count[3]"]);
        spriggitFields["Count[4]"].ShouldBe(dtoFields["Count[4]"]);
        spriggitFields["Count[5]"].ShouldBe(dtoFields["Count[5]"]);
        spriggitFields["CreatedObject"].ShouldBe(dtoFields["CreatedObjectFormKey"]);
        spriggitFields["Description.Count"].ShouldBe(dtoFields["Description.Count"]);
        spriggitFields["Description.TargetLanguage"].ShouldBe(dtoFields["Description.TargetLanguage"]);
        spriggitFields["Description[0].Language"].ShouldBe(dtoFields["Description[0].Language"]);
        spriggitFields["Description[0].String"].ShouldBe(dtoFields["Description[0].String"]);
        spriggitFields["Description[1].Language"].ShouldBe(dtoFields["Description[1].Language"]);
        spriggitFields["Description[1].String"].ShouldBe(dtoFields["Description[1].String"]);
        spriggitFields["Description[10].Language"].ShouldBe(dtoFields["Description[10].Language"]);
        spriggitFields["Description[10].String"].ShouldBe(dtoFields["Description[10].String"]);
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
        spriggitFields["Description[9].Language"].ShouldBe(dtoFields["Description[9].Language"]);
        spriggitFields["Description[9].String"].ShouldBe(dtoFields["Description[9].String"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["PickUpSound"].ShouldBe(dtoFields["PickUpSound"]);
        spriggitFields["PutDownSound"].ShouldBe(dtoFields["PutDownSound"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["WorkbenchKeyword"].ShouldBe(dtoFields["WorkbenchKeywordFormKey"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["ComparisonValue"].ShouldBe(dtoFields["ComparisonValue"]);
        spriggitFields["Count[0]"].ShouldBe(dtoFields["Count[0]"]);
        spriggitFields["Count[1]"].ShouldBe(dtoFields["Count[1]"]);
        spriggitFields["Count[10]"].ShouldBe(dtoFields["Count[10]"]);
        spriggitFields["Count[2]"].ShouldBe(dtoFields["Count[2]"]);
        spriggitFields["Count[3]"].ShouldBe(dtoFields["Count[3]"]);
        spriggitFields["Count[4]"].ShouldBe(dtoFields["Count[4]"]);
        spriggitFields["Count[5]"].ShouldBe(dtoFields["Count[5]"]);
        spriggitFields["Count[6]"].ShouldBe(dtoFields["Count[6]"]);
        spriggitFields["Count[7]"].ShouldBe(dtoFields["Count[7]"]);
        spriggitFields["Count[8]"].ShouldBe(dtoFields["Count[8]"]);
        spriggitFields["Count[9]"].ShouldBe(dtoFields["Count[9]"]);
        spriggitFields["CreatedObject"].ShouldBe(dtoFields["CreatedObjectFormKey"]);
        spriggitFields["Data.Function"].ShouldBe(dtoFields["Data.Function"]);
        spriggitFields["Data.MutagenObjectType"].ShouldBe(dtoFields["Data.MutagenObjectType"]);
        spriggitFields["Data.ParameterOneNumber"].ShouldBe(dtoFields["Data.ParameterOneNumber"]);
        spriggitFields["Data.ParameterOneRecord"].ShouldBe(dtoFields["Data.ParameterOneRecord"]);
        spriggitFields["Description.TargetLanguage"].ShouldBe(dtoFields["Description.TargetLanguage"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }
}
