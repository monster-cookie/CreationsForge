using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.ConditionForm.Starfield;

public class StarfieldConditionFormSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "CNDF")]
    [Trait("FormKey", "3C8F9C:Starfield.esm")]
    [Trait("EditorID", "DebugMoveToPlanetConditions_Trait")]
    [Trait("SpriggitFile", "ConditionRecords/DebugMoveToPlanetConditions_Trait - 3C8F9C_Starfield.esm.yaml")]
    public void Starfield_CNDF_ShouldMatchSpriggitSample_DebugMoveToPlanetConditions_Trait()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.ConditionForm,
            "DebugMoveToPlanetConditions_Trait");
        var dto = Helpers.GetDTO<ConditionFormDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.ConditionForm,
            "3C8F9C:Starfield.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["ComparisonValue[0]"].ShouldBe(dtoFields["ComparisonValue[0]"]);
        spriggitFields["ComparisonValue[1]"].ShouldBe(dtoFields["ComparisonValue[1]"]);
        spriggitFields["ComparisonValue[10]"].ShouldBe(dtoFields["ComparisonValue[10]"]);
        spriggitFields["ComparisonValue[11]"].ShouldBe(dtoFields["ComparisonValue[11]"]);
        spriggitFields["ComparisonValue[12]"].ShouldBe(dtoFields["ComparisonValue[12]"]);
        spriggitFields["ComparisonValue[13]"].ShouldBe(dtoFields["ComparisonValue[13]"]);
        spriggitFields["ComparisonValue[14]"].ShouldBe(dtoFields["ComparisonValue[14]"]);
        spriggitFields["ComparisonValue[15]"].ShouldBe(dtoFields["ComparisonValue[15]"]);
        spriggitFields["ComparisonValue[16]"].ShouldBe(dtoFields["ComparisonValue[16]"]);
        spriggitFields["ComparisonValue[17]"].ShouldBe(dtoFields["ComparisonValue[17]"]);
        spriggitFields["ComparisonValue[18]"].ShouldBe(dtoFields["ComparisonValue[18]"]);
        spriggitFields["ComparisonValue[19]"].ShouldBe(dtoFields["ComparisonValue[19]"]);
        spriggitFields["ComparisonValue[2]"].ShouldBe(dtoFields["ComparisonValue[2]"]);
        spriggitFields["ComparisonValue[20]"].ShouldBe(dtoFields["ComparisonValue[20]"]);
        spriggitFields["ComparisonValue[21]"].ShouldBe(dtoFields["ComparisonValue[21]"]);
        spriggitFields["ComparisonValue[22]"].ShouldBe(dtoFields["ComparisonValue[22]"]);
        spriggitFields["ComparisonValue[23]"].ShouldBe(dtoFields["ComparisonValue[23]"]);
        spriggitFields["ComparisonValue[24]"].ShouldBe(dtoFields["ComparisonValue[24]"]);
        spriggitFields["ComparisonValue[25]"].ShouldBe(dtoFields["ComparisonValue[25]"]);
        spriggitFields["ComparisonValue[26]"].ShouldBe(dtoFields["ComparisonValue[26]"]);
        spriggitFields["ComparisonValue[27]"].ShouldBe(dtoFields["ComparisonValue[27]"]);
        spriggitFields["ComparisonValue[3]"].ShouldBe(dtoFields["ComparisonValue[3]"]);
        spriggitFields["ComparisonValue[4]"].ShouldBe(dtoFields["ComparisonValue[4]"]);
        spriggitFields["ComparisonValue[5]"].ShouldBe(dtoFields["ComparisonValue[5]"]);
        spriggitFields["ComparisonValue[6]"].ShouldBe(dtoFields["ComparisonValue[6]"]);
        spriggitFields["ComparisonValue[7]"].ShouldBe(dtoFields["ComparisonValue[7]"]);
        spriggitFields["ComparisonValue[8]"].ShouldBe(dtoFields["ComparisonValue[8]"]);
        spriggitFields["ComparisonValue[9]"].ShouldBe(dtoFields["ComparisonValue[9]"]);
        spriggitFields["Data.FirstParameter[0]"].ShouldBe(dtoFields["Data.FirstParameter[0]"]);
        spriggitFields["Data.FirstParameter[1]"].ShouldBe(dtoFields["Data.FirstParameter[1]"]);
        spriggitFields["Data.FirstParameter[10]"].ShouldBe(dtoFields["Data.FirstParameter[10]"]);
        spriggitFields["Data.FirstParameter[11]"].ShouldBe(dtoFields["Data.FirstParameter[11]"]);
        spriggitFields["Data.FirstParameter[12]"].ShouldBe(dtoFields["Data.FirstParameter[12]"]);
        spriggitFields["Data.FirstParameter[13]"].ShouldBe(dtoFields["Data.FirstParameter[13]"]);
        spriggitFields["Data.FirstParameter[14]"].ShouldBe(dtoFields["Data.FirstParameter[14]"]);
        spriggitFields["Data.FirstParameter[15]"].ShouldBe(dtoFields["Data.FirstParameter[15]"]);
        spriggitFields["Data.FirstParameter[16]"].ShouldBe(dtoFields["Data.FirstParameter[16]"]);
        spriggitFields["Data.FirstParameter[17]"].ShouldBe(dtoFields["Data.FirstParameter[17]"]);
        spriggitFields["Data.FirstParameter[18]"].ShouldBe(dtoFields["Data.FirstParameter[18]"]);
        spriggitFields["Data.FirstParameter[19]"].ShouldBe(dtoFields["Data.FirstParameter[19]"]);
        spriggitFields["Data.FirstParameter[2]"].ShouldBe(dtoFields["Data.FirstParameter[2]"]);
        spriggitFields["Data.FirstParameter[20]"].ShouldBe(dtoFields["Data.FirstParameter[20]"]);
        spriggitFields["Data.FirstParameter[21]"].ShouldBe(dtoFields["Data.FirstParameter[21]"]);
        spriggitFields["Data.FirstParameter[22]"].ShouldBe(dtoFields["Data.FirstParameter[22]"]);
        spriggitFields["Data.FirstParameter[23]"].ShouldBe(dtoFields["Data.FirstParameter[23]"]);
        spriggitFields["Data.FirstParameter[24]"].ShouldBe(dtoFields["Data.FirstParameter[24]"]);
        spriggitFields["Data.FirstParameter[25]"].ShouldBe(dtoFields["Data.FirstParameter[25]"]);
        spriggitFields["Data.FirstParameter[26]"].ShouldBe(dtoFields["Data.FirstParameter[26]"]);
        spriggitFields["Data.FirstParameter[27]"].ShouldBe(dtoFields["Data.FirstParameter[27]"]);
        spriggitFields["Data.FirstParameter[3]"].ShouldBe(dtoFields["Data.FirstParameter[3]"]);
        spriggitFields["Data.FirstParameter[4]"].ShouldBe(dtoFields["Data.FirstParameter[4]"]);
        spriggitFields["Data.FirstParameter[5]"].ShouldBe(dtoFields["Data.FirstParameter[5]"]);
        spriggitFields["Data.FirstParameter[6]"].ShouldBe(dtoFields["Data.FirstParameter[6]"]);
        spriggitFields["Data.FirstParameter[7]"].ShouldBe(dtoFields["Data.FirstParameter[7]"]);
        spriggitFields["Data.FirstParameter[8]"].ShouldBe(dtoFields["Data.FirstParameter[8]"]);
        spriggitFields["Data.FirstParameter[9]"].ShouldBe(dtoFields["Data.FirstParameter[9]"]);
        spriggitFields["Data.MutagenObjectType[0]"].ShouldBe(dtoFields["Data.MutagenObjectType[0]"]);
        spriggitFields["Data.MutagenObjectType[1]"].ShouldBe(dtoFields["Data.MutagenObjectType[1]"]);
        spriggitFields["Data.MutagenObjectType[10]"].ShouldBe(dtoFields["Data.MutagenObjectType[10]"]);
        spriggitFields["Data.MutagenObjectType[11]"].ShouldBe(dtoFields["Data.MutagenObjectType[11]"]);
        spriggitFields["Data.MutagenObjectType[12]"].ShouldBe(dtoFields["Data.MutagenObjectType[12]"]);
        spriggitFields["Data.MutagenObjectType[13]"].ShouldBe(dtoFields["Data.MutagenObjectType[13]"]);
        spriggitFields["Data.MutagenObjectType[14]"].ShouldBe(dtoFields["Data.MutagenObjectType[14]"]);
        spriggitFields["Data.MutagenObjectType[15]"].ShouldBe(dtoFields["Data.MutagenObjectType[15]"]);
        spriggitFields["Data.MutagenObjectType[16]"].ShouldBe(dtoFields["Data.MutagenObjectType[16]"]);
        spriggitFields["Data.MutagenObjectType[17]"].ShouldBe(dtoFields["Data.MutagenObjectType[17]"]);
        spriggitFields["Data.MutagenObjectType[18]"].ShouldBe(dtoFields["Data.MutagenObjectType[18]"]);
        spriggitFields["Data.MutagenObjectType[19]"].ShouldBe(dtoFields["Data.MutagenObjectType[19]"]);
        spriggitFields["Data.MutagenObjectType[2]"].ShouldBe(dtoFields["Data.MutagenObjectType[2]"]);
        spriggitFields["Data.MutagenObjectType[20]"].ShouldBe(dtoFields["Data.MutagenObjectType[20]"]);
        spriggitFields["Data.MutagenObjectType[21]"].ShouldBe(dtoFields["Data.MutagenObjectType[21]"]);
        spriggitFields["Data.MutagenObjectType[22]"].ShouldBe(dtoFields["Data.MutagenObjectType[22]"]);
        spriggitFields["Data.MutagenObjectType[23]"].ShouldBe(dtoFields["Data.MutagenObjectType[23]"]);
        spriggitFields["Data.MutagenObjectType[24]"].ShouldBe(dtoFields["Data.MutagenObjectType[24]"]);
        spriggitFields["Data.MutagenObjectType[25]"].ShouldBe(dtoFields["Data.MutagenObjectType[25]"]);
        spriggitFields["Data.MutagenObjectType[26]"].ShouldBe(dtoFields["Data.MutagenObjectType[26]"]);
        spriggitFields["Data.MutagenObjectType[27]"].ShouldBe(dtoFields["Data.MutagenObjectType[27]"]);
        spriggitFields["Data.MutagenObjectType[3]"].ShouldBe(dtoFields["Data.MutagenObjectType[3]"]);
        spriggitFields["Data.MutagenObjectType[4]"].ShouldBe(dtoFields["Data.MutagenObjectType[4]"]);
        spriggitFields["Data.MutagenObjectType[5]"].ShouldBe(dtoFields["Data.MutagenObjectType[5]"]);
        spriggitFields["Data.MutagenObjectType[6]"].ShouldBe(dtoFields["Data.MutagenObjectType[6]"]);
        spriggitFields["Data.MutagenObjectType[7]"].ShouldBe(dtoFields["Data.MutagenObjectType[7]"]);
        spriggitFields["Data.MutagenObjectType[8]"].ShouldBe(dtoFields["Data.MutagenObjectType[8]"]);
        spriggitFields["Data.MutagenObjectType[9]"].ShouldBe(dtoFields["Data.MutagenObjectType[9]"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["OwnerQuest"].ShouldBe(dtoFields["OwnerQuest"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "CNDF")]
    [Trait("FormKey", "31982F:Starfield.esm")]
    [Trait("EditorID", "SFBGS_CND_Placeholder01_ReservedForUse")]
    [Trait("SpriggitFile", "ConditionRecords/SFBGS_CND_Placeholder01_ReservedForUse - 31982F_Starfield.esm.yaml")]
    public void Starfield_CNDF_ShouldMatchSpriggitSample_SFBGS_CND_Placeholder01_ReservedForUse()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.ConditionForm,
            "SFBGS_CND_Placeholder01_ReservedForUse");
        var dto = Helpers.GetDTO<ConditionFormDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.ConditionForm,
            "31982F:Starfield.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["ComparisonValue[0]"].ShouldBe(dtoFields["ComparisonValue[0]"]);
        spriggitFields["ComparisonValue[1]"].ShouldBe(dtoFields["ComparisonValue[1]"]);
        spriggitFields["ComparisonValue[10]"].ShouldBe(dtoFields["ComparisonValue[10]"]);
        spriggitFields["ComparisonValue[11]"].ShouldBe(dtoFields["ComparisonValue[11]"]);
        spriggitFields["ComparisonValue[12]"].ShouldBe(dtoFields["ComparisonValue[12]"]);
        spriggitFields["ComparisonValue[13]"].ShouldBe(dtoFields["ComparisonValue[13]"]);
        spriggitFields["ComparisonValue[14]"].ShouldBe(dtoFields["ComparisonValue[14]"]);
        spriggitFields["ComparisonValue[15]"].ShouldBe(dtoFields["ComparisonValue[15]"]);
        spriggitFields["ComparisonValue[16]"].ShouldBe(dtoFields["ComparisonValue[16]"]);
        spriggitFields["ComparisonValue[17]"].ShouldBe(dtoFields["ComparisonValue[17]"]);
        spriggitFields["ComparisonValue[18]"].ShouldBe(dtoFields["ComparisonValue[18]"]);
        spriggitFields["ComparisonValue[2]"].ShouldBe(dtoFields["ComparisonValue[2]"]);
        spriggitFields["ComparisonValue[3]"].ShouldBe(dtoFields["ComparisonValue[3]"]);
        spriggitFields["ComparisonValue[4]"].ShouldBe(dtoFields["ComparisonValue[4]"]);
        spriggitFields["ComparisonValue[5]"].ShouldBe(dtoFields["ComparisonValue[5]"]);
        spriggitFields["ComparisonValue[6]"].ShouldBe(dtoFields["ComparisonValue[6]"]);
        spriggitFields["ComparisonValue[7]"].ShouldBe(dtoFields["ComparisonValue[7]"]);
        spriggitFields["ComparisonValue[8]"].ShouldBe(dtoFields["ComparisonValue[8]"]);
        spriggitFields["ComparisonValue[9]"].ShouldBe(dtoFields["ComparisonValue[9]"]);
        spriggitFields["Data.FirstParameter[0]"].ShouldBe(dtoFields["Data.FirstParameter[0]"]);
        spriggitFields["Data.FirstParameter[1]"].ShouldBe(dtoFields["Data.FirstParameter[1]"]);
        spriggitFields["Data.FirstParameter[10]"].ShouldBe(dtoFields["Data.FirstParameter[10]"]);
        spriggitFields["Data.FirstParameter[11]"].ShouldBe(dtoFields["Data.FirstParameter[11]"]);
        spriggitFields["Data.FirstParameter[12]"].ShouldBe(dtoFields["Data.FirstParameter[12]"]);
        spriggitFields["Data.FirstParameter[13]"].ShouldBe(dtoFields["Data.FirstParameter[13]"]);
        spriggitFields["Data.FirstParameter[14]"].ShouldBe(dtoFields["Data.FirstParameter[14]"]);
        spriggitFields["Data.FirstParameter[15]"].ShouldBe(dtoFields["Data.FirstParameter[15]"]);
        spriggitFields["Data.FirstParameter[16]"].ShouldBe(dtoFields["Data.FirstParameter[16]"]);
        spriggitFields["Data.FirstParameter[17]"].ShouldBe(dtoFields["Data.FirstParameter[17]"]);
        spriggitFields["Data.FirstParameter[18]"].ShouldBe(dtoFields["Data.FirstParameter[18]"]);
        spriggitFields["Data.FirstParameter[2]"].ShouldBe(dtoFields["Data.FirstParameter[2]"]);
        spriggitFields["Data.FirstParameter[3]"].ShouldBe(dtoFields["Data.FirstParameter[3]"]);
        spriggitFields["Data.FirstParameter[4]"].ShouldBe(dtoFields["Data.FirstParameter[4]"]);
        spriggitFields["Data.FirstParameter[5]"].ShouldBe(dtoFields["Data.FirstParameter[5]"]);
        spriggitFields["Data.FirstParameter[6]"].ShouldBe(dtoFields["Data.FirstParameter[6]"]);
        spriggitFields["Data.FirstParameter[7]"].ShouldBe(dtoFields["Data.FirstParameter[7]"]);
        spriggitFields["Data.FirstParameter[8]"].ShouldBe(dtoFields["Data.FirstParameter[8]"]);
        spriggitFields["Data.FirstParameter[9]"].ShouldBe(dtoFields["Data.FirstParameter[9]"]);
        spriggitFields["Data.MutagenObjectType[0]"].ShouldBe(dtoFields["Data.MutagenObjectType[0]"]);
        spriggitFields["Data.MutagenObjectType[1]"].ShouldBe(dtoFields["Data.MutagenObjectType[1]"]);
        spriggitFields["Data.MutagenObjectType[10]"].ShouldBe(dtoFields["Data.MutagenObjectType[10]"]);
        spriggitFields["Data.MutagenObjectType[11]"].ShouldBe(dtoFields["Data.MutagenObjectType[11]"]);
        spriggitFields["Data.MutagenObjectType[12]"].ShouldBe(dtoFields["Data.MutagenObjectType[12]"]);
        spriggitFields["Data.MutagenObjectType[13]"].ShouldBe(dtoFields["Data.MutagenObjectType[13]"]);
        spriggitFields["Data.MutagenObjectType[14]"].ShouldBe(dtoFields["Data.MutagenObjectType[14]"]);
        spriggitFields["Data.MutagenObjectType[15]"].ShouldBe(dtoFields["Data.MutagenObjectType[15]"]);
        spriggitFields["Data.MutagenObjectType[16]"].ShouldBe(dtoFields["Data.MutagenObjectType[16]"]);
        spriggitFields["Data.MutagenObjectType[17]"].ShouldBe(dtoFields["Data.MutagenObjectType[17]"]);
        spriggitFields["Data.MutagenObjectType[18]"].ShouldBe(dtoFields["Data.MutagenObjectType[18]"]);
        spriggitFields["Data.MutagenObjectType[2]"].ShouldBe(dtoFields["Data.MutagenObjectType[2]"]);
        spriggitFields["Data.MutagenObjectType[3]"].ShouldBe(dtoFields["Data.MutagenObjectType[3]"]);
        spriggitFields["Data.MutagenObjectType[4]"].ShouldBe(dtoFields["Data.MutagenObjectType[4]"]);
        spriggitFields["Data.MutagenObjectType[5]"].ShouldBe(dtoFields["Data.MutagenObjectType[5]"]);
        spriggitFields["Data.MutagenObjectType[6]"].ShouldBe(dtoFields["Data.MutagenObjectType[6]"]);
        spriggitFields["Data.MutagenObjectType[7]"].ShouldBe(dtoFields["Data.MutagenObjectType[7]"]);
        spriggitFields["Data.MutagenObjectType[8]"].ShouldBe(dtoFields["Data.MutagenObjectType[8]"]);
        spriggitFields["Data.MutagenObjectType[9]"].ShouldBe(dtoFields["Data.MutagenObjectType[9]"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "CNDF")]
    [Trait("FormKey", "10460E:Starfield.esm")]
    [Trait("EditorID", "SQ_TreasureMap_CND_IsResourceLocation")]
    [Trait("SpriggitFile", "ConditionRecords/SQ_TreasureMap_CND_IsResourceLocation - 10460E_Starfield.esm.yaml")]
    public void Starfield_CNDF_ShouldMatchSpriggitSample_SQ_TreasureMap_CND_IsResourceLocation()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.ConditionForm,
            "SQ_TreasureMap_CND_IsResourceLocation");
        var dto = Helpers.GetDTO<ConditionFormDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.ConditionForm,
            "10460E:Starfield.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["CompareOperator[0]"].ShouldBe(dtoFields["CompareOperator[0]"]);
        spriggitFields["CompareOperator[1]"].ShouldBe(dtoFields["CompareOperator[1]"]);
        spriggitFields["CompareOperator[10]"].ShouldBe(dtoFields["CompareOperator[10]"]);
        spriggitFields["CompareOperator[11]"].ShouldBe(dtoFields["CompareOperator[11]"]);
        spriggitFields["CompareOperator[12]"].ShouldBe(dtoFields["CompareOperator[12]"]);
        spriggitFields["CompareOperator[13]"].ShouldBe(dtoFields["CompareOperator[13]"]);
        spriggitFields["CompareOperator[14]"].ShouldBe(dtoFields["CompareOperator[14]"]);
        spriggitFields["CompareOperator[15]"].ShouldBe(dtoFields["CompareOperator[15]"]);
        spriggitFields["CompareOperator[16]"].ShouldBe(dtoFields["CompareOperator[16]"]);
        spriggitFields["CompareOperator[17]"].ShouldBe(dtoFields["CompareOperator[17]"]);
        spriggitFields["CompareOperator[18]"].ShouldBe(dtoFields["CompareOperator[18]"]);
        spriggitFields["CompareOperator[19]"].ShouldBe(dtoFields["CompareOperator[19]"]);
        spriggitFields["CompareOperator[2]"].ShouldBe(dtoFields["CompareOperator[2]"]);
        spriggitFields["CompareOperator[20]"].ShouldBe(dtoFields["CompareOperator[20]"]);
        spriggitFields["CompareOperator[21]"].ShouldBe(dtoFields["CompareOperator[21]"]);
        spriggitFields["CompareOperator[22]"].ShouldBe(dtoFields["CompareOperator[22]"]);
        spriggitFields["CompareOperator[23]"].ShouldBe(dtoFields["CompareOperator[23]"]);
        spriggitFields["CompareOperator[24]"].ShouldBe(dtoFields["CompareOperator[24]"]);
        spriggitFields["CompareOperator[25]"].ShouldBe(dtoFields["CompareOperator[25]"]);
        spriggitFields["CompareOperator[26]"].ShouldBe(dtoFields["CompareOperator[26]"]);
        spriggitFields["CompareOperator[27]"].ShouldBe(dtoFields["CompareOperator[27]"]);
        spriggitFields["CompareOperator[28]"].ShouldBe(dtoFields["CompareOperator[28]"]);
        spriggitFields["CompareOperator[29]"].ShouldBe(dtoFields["CompareOperator[29]"]);
        spriggitFields["CompareOperator[3]"].ShouldBe(dtoFields["CompareOperator[3]"]);
        spriggitFields["CompareOperator[30]"].ShouldBe(dtoFields["CompareOperator[30]"]);
        spriggitFields["CompareOperator[31]"].ShouldBe(dtoFields["CompareOperator[31]"]);
        spriggitFields["CompareOperator[32]"].ShouldBe(dtoFields["CompareOperator[32]"]);
        spriggitFields["CompareOperator[33]"].ShouldBe(dtoFields["CompareOperator[33]"]);
        spriggitFields["CompareOperator[34]"].ShouldBe(dtoFields["CompareOperator[34]"]);
        spriggitFields["CompareOperator[35]"].ShouldBe(dtoFields["CompareOperator[35]"]);
        spriggitFields["CompareOperator[36]"].ShouldBe(dtoFields["CompareOperator[36]"]);
        spriggitFields["CompareOperator[37]"].ShouldBe(dtoFields["CompareOperator[37]"]);
        spriggitFields["CompareOperator[38]"].ShouldBe(dtoFields["CompareOperator[38]"]);
        spriggitFields["CompareOperator[39]"].ShouldBe(dtoFields["CompareOperator[39]"]);
        spriggitFields["CompareOperator[4]"].ShouldBe(dtoFields["CompareOperator[4]"]);
        spriggitFields["CompareOperator[40]"].ShouldBe(dtoFields["CompareOperator[40]"]);
        spriggitFields["CompareOperator[41]"].ShouldBe(dtoFields["CompareOperator[41]"]);
        spriggitFields["CompareOperator[42]"].ShouldBe(dtoFields["CompareOperator[42]"]);
        spriggitFields["CompareOperator[43]"].ShouldBe(dtoFields["CompareOperator[43]"]);
        spriggitFields["CompareOperator[44]"].ShouldBe(dtoFields["CompareOperator[44]"]);
        spriggitFields["CompareOperator[5]"].ShouldBe(dtoFields["CompareOperator[5]"]);
        spriggitFields["CompareOperator[6]"].ShouldBe(dtoFields["CompareOperator[6]"]);
        spriggitFields["CompareOperator[7]"].ShouldBe(dtoFields["CompareOperator[7]"]);
        spriggitFields["CompareOperator[8]"].ShouldBe(dtoFields["CompareOperator[8]"]);
        spriggitFields["CompareOperator[9]"].ShouldBe(dtoFields["CompareOperator[9]"]);
        spriggitFields["Data.FirstParameter[0]"].ShouldBe(dtoFields["Data.FirstParameter[0]"]);
        spriggitFields["Data.FirstParameter[1]"].ShouldBe(dtoFields["Data.FirstParameter[1]"]);
        spriggitFields["Data.FirstParameter[10]"].ShouldBe(dtoFields["Data.FirstParameter[10]"]);
        spriggitFields["Data.FirstParameter[11]"].ShouldBe(dtoFields["Data.FirstParameter[11]"]);
        spriggitFields["Data.FirstParameter[12]"].ShouldBe(dtoFields["Data.FirstParameter[12]"]);
        spriggitFields["Data.FirstParameter[13]"].ShouldBe(dtoFields["Data.FirstParameter[13]"]);
        spriggitFields["Data.FirstParameter[14]"].ShouldBe(dtoFields["Data.FirstParameter[14]"]);
        spriggitFields["Data.FirstParameter[15]"].ShouldBe(dtoFields["Data.FirstParameter[15]"]);
        spriggitFields["Data.FirstParameter[16]"].ShouldBe(dtoFields["Data.FirstParameter[16]"]);
        spriggitFields["Data.FirstParameter[17]"].ShouldBe(dtoFields["Data.FirstParameter[17]"]);
        spriggitFields["Data.FirstParameter[18]"].ShouldBe(dtoFields["Data.FirstParameter[18]"]);
        spriggitFields["Data.FirstParameter[19]"].ShouldBe(dtoFields["Data.FirstParameter[19]"]);
        spriggitFields["Data.FirstParameter[2]"].ShouldBe(dtoFields["Data.FirstParameter[2]"]);
        spriggitFields["Data.FirstParameter[20]"].ShouldBe(dtoFields["Data.FirstParameter[20]"]);
        spriggitFields["Data.FirstParameter[21]"].ShouldBe(dtoFields["Data.FirstParameter[21]"]);
        spriggitFields["Data.FirstParameter[22]"].ShouldBe(dtoFields["Data.FirstParameter[22]"]);
        spriggitFields["Data.FirstParameter[23]"].ShouldBe(dtoFields["Data.FirstParameter[23]"]);
        spriggitFields["Data.FirstParameter[24]"].ShouldBe(dtoFields["Data.FirstParameter[24]"]);
        spriggitFields["Data.FirstParameter[25]"].ShouldBe(dtoFields["Data.FirstParameter[25]"]);
        spriggitFields["Data.FirstParameter[26]"].ShouldBe(dtoFields["Data.FirstParameter[26]"]);
        spriggitFields["Data.FirstParameter[27]"].ShouldBe(dtoFields["Data.FirstParameter[27]"]);
        spriggitFields["Data.FirstParameter[28]"].ShouldBe(dtoFields["Data.FirstParameter[28]"]);
        spriggitFields["Data.FirstParameter[29]"].ShouldBe(dtoFields["Data.FirstParameter[29]"]);
        spriggitFields["Data.FirstParameter[3]"].ShouldBe(dtoFields["Data.FirstParameter[3]"]);
        spriggitFields["Data.FirstParameter[30]"].ShouldBe(dtoFields["Data.FirstParameter[30]"]);
        spriggitFields["Data.FirstParameter[31]"].ShouldBe(dtoFields["Data.FirstParameter[31]"]);
        spriggitFields["Data.FirstParameter[32]"].ShouldBe(dtoFields["Data.FirstParameter[32]"]);
        spriggitFields["Data.FirstParameter[33]"].ShouldBe(dtoFields["Data.FirstParameter[33]"]);
        spriggitFields["Data.FirstParameter[34]"].ShouldBe(dtoFields["Data.FirstParameter[34]"]);
        spriggitFields["Data.FirstParameter[35]"].ShouldBe(dtoFields["Data.FirstParameter[35]"]);
        spriggitFields["Data.FirstParameter[36]"].ShouldBe(dtoFields["Data.FirstParameter[36]"]);
        spriggitFields["Data.FirstParameter[37]"].ShouldBe(dtoFields["Data.FirstParameter[37]"]);
        spriggitFields["Data.FirstParameter[38]"].ShouldBe(dtoFields["Data.FirstParameter[38]"]);
        spriggitFields["Data.FirstParameter[39]"].ShouldBe(dtoFields["Data.FirstParameter[39]"]);
        spriggitFields["Data.FirstParameter[4]"].ShouldBe(dtoFields["Data.FirstParameter[4]"]);
        spriggitFields["Data.FirstParameter[40]"].ShouldBe(dtoFields["Data.FirstParameter[40]"]);
        spriggitFields["Data.FirstParameter[41]"].ShouldBe(dtoFields["Data.FirstParameter[41]"]);
        spriggitFields["Data.FirstParameter[42]"].ShouldBe(dtoFields["Data.FirstParameter[42]"]);
        spriggitFields["Data.FirstParameter[43]"].ShouldBe(dtoFields["Data.FirstParameter[43]"]);
        spriggitFields["Data.FirstParameter[44]"].ShouldBe(dtoFields["Data.FirstParameter[44]"]);
        spriggitFields["Data.FirstParameter[5]"].ShouldBe(dtoFields["Data.FirstParameter[5]"]);
        spriggitFields["Data.FirstParameter[6]"].ShouldBe(dtoFields["Data.FirstParameter[6]"]);
        spriggitFields["Data.FirstParameter[7]"].ShouldBe(dtoFields["Data.FirstParameter[7]"]);
        spriggitFields["Data.FirstParameter[8]"].ShouldBe(dtoFields["Data.FirstParameter[8]"]);
        spriggitFields["Data.FirstParameter[9]"].ShouldBe(dtoFields["Data.FirstParameter[9]"]);
        spriggitFields["Data.MutagenObjectType[0]"].ShouldBe(dtoFields["Data.MutagenObjectType[0]"]);
        spriggitFields["Data.MutagenObjectType[1]"].ShouldBe(dtoFields["Data.MutagenObjectType[1]"]);
        spriggitFields["Data.MutagenObjectType[10]"].ShouldBe(dtoFields["Data.MutagenObjectType[10]"]);
        spriggitFields["Data.MutagenObjectType[11]"].ShouldBe(dtoFields["Data.MutagenObjectType[11]"]);
        spriggitFields["Data.MutagenObjectType[12]"].ShouldBe(dtoFields["Data.MutagenObjectType[12]"]);
        spriggitFields["Data.MutagenObjectType[13]"].ShouldBe(dtoFields["Data.MutagenObjectType[13]"]);
        spriggitFields["Data.MutagenObjectType[14]"].ShouldBe(dtoFields["Data.MutagenObjectType[14]"]);
        spriggitFields["Data.MutagenObjectType[15]"].ShouldBe(dtoFields["Data.MutagenObjectType[15]"]);
        spriggitFields["Data.MutagenObjectType[16]"].ShouldBe(dtoFields["Data.MutagenObjectType[16]"]);
        spriggitFields["Data.MutagenObjectType[17]"].ShouldBe(dtoFields["Data.MutagenObjectType[17]"]);
        spriggitFields["Data.MutagenObjectType[18]"].ShouldBe(dtoFields["Data.MutagenObjectType[18]"]);
        spriggitFields["Data.MutagenObjectType[19]"].ShouldBe(dtoFields["Data.MutagenObjectType[19]"]);
        spriggitFields["Data.MutagenObjectType[2]"].ShouldBe(dtoFields["Data.MutagenObjectType[2]"]);
        spriggitFields["Data.MutagenObjectType[20]"].ShouldBe(dtoFields["Data.MutagenObjectType[20]"]);
        spriggitFields["Data.MutagenObjectType[21]"].ShouldBe(dtoFields["Data.MutagenObjectType[21]"]);
        spriggitFields["Data.MutagenObjectType[22]"].ShouldBe(dtoFields["Data.MutagenObjectType[22]"]);
        spriggitFields["Data.MutagenObjectType[23]"].ShouldBe(dtoFields["Data.MutagenObjectType[23]"]);
        spriggitFields["Data.MutagenObjectType[24]"].ShouldBe(dtoFields["Data.MutagenObjectType[24]"]);
        spriggitFields["Data.MutagenObjectType[25]"].ShouldBe(dtoFields["Data.MutagenObjectType[25]"]);
        spriggitFields["Data.MutagenObjectType[26]"].ShouldBe(dtoFields["Data.MutagenObjectType[26]"]);
        spriggitFields["Data.MutagenObjectType[27]"].ShouldBe(dtoFields["Data.MutagenObjectType[27]"]);
        spriggitFields["Data.MutagenObjectType[28]"].ShouldBe(dtoFields["Data.MutagenObjectType[28]"]);
        spriggitFields["Data.MutagenObjectType[29]"].ShouldBe(dtoFields["Data.MutagenObjectType[29]"]);
        spriggitFields["Data.MutagenObjectType[3]"].ShouldBe(dtoFields["Data.MutagenObjectType[3]"]);
        spriggitFields["Data.MutagenObjectType[30]"].ShouldBe(dtoFields["Data.MutagenObjectType[30]"]);
        spriggitFields["Data.MutagenObjectType[31]"].ShouldBe(dtoFields["Data.MutagenObjectType[31]"]);
        spriggitFields["Data.MutagenObjectType[32]"].ShouldBe(dtoFields["Data.MutagenObjectType[32]"]);
        spriggitFields["Data.MutagenObjectType[33]"].ShouldBe(dtoFields["Data.MutagenObjectType[33]"]);
        spriggitFields["Data.MutagenObjectType[34]"].ShouldBe(dtoFields["Data.MutagenObjectType[34]"]);
        spriggitFields["Data.MutagenObjectType[35]"].ShouldBe(dtoFields["Data.MutagenObjectType[35]"]);
        spriggitFields["Data.MutagenObjectType[36]"].ShouldBe(dtoFields["Data.MutagenObjectType[36]"]);
        spriggitFields["Data.MutagenObjectType[37]"].ShouldBe(dtoFields["Data.MutagenObjectType[37]"]);
        spriggitFields["Data.MutagenObjectType[38]"].ShouldBe(dtoFields["Data.MutagenObjectType[38]"]);
        spriggitFields["Data.MutagenObjectType[39]"].ShouldBe(dtoFields["Data.MutagenObjectType[39]"]);
        spriggitFields["Data.MutagenObjectType[4]"].ShouldBe(dtoFields["Data.MutagenObjectType[4]"]);
        spriggitFields["Data.MutagenObjectType[40]"].ShouldBe(dtoFields["Data.MutagenObjectType[40]"]);
        spriggitFields["Data.MutagenObjectType[41]"].ShouldBe(dtoFields["Data.MutagenObjectType[41]"]);
        spriggitFields["Data.MutagenObjectType[42]"].ShouldBe(dtoFields["Data.MutagenObjectType[42]"]);
        spriggitFields["Data.MutagenObjectType[43]"].ShouldBe(dtoFields["Data.MutagenObjectType[43]"]);
        spriggitFields["Data.MutagenObjectType[44]"].ShouldBe(dtoFields["Data.MutagenObjectType[44]"]);
        spriggitFields["Data.MutagenObjectType[5]"].ShouldBe(dtoFields["Data.MutagenObjectType[5]"]);
        spriggitFields["Data.MutagenObjectType[6]"].ShouldBe(dtoFields["Data.MutagenObjectType[6]"]);
        spriggitFields["Data.MutagenObjectType[7]"].ShouldBe(dtoFields["Data.MutagenObjectType[7]"]);
        spriggitFields["Data.MutagenObjectType[8]"].ShouldBe(dtoFields["Data.MutagenObjectType[8]"]);
        spriggitFields["Data.MutagenObjectType[9]"].ShouldBe(dtoFields["Data.MutagenObjectType[9]"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "CNDF")]
    [Trait("FormKey", "0B1206:Starfield.esm")]
    [Trait("EditorID", "ActorShouldShowSpacesuitGameplayFlashlight")]
    [Trait("SpriggitFile", "ConditionRecords/ActorShouldShowSpacesuitGameplayFlashlight - 0B1206_Starfield.esm.yaml")]
    public void Starfield_CNDF_ShouldMatchSpriggitSample_ActorShouldShowSpacesuitGameplayFlashlight()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.ConditionForm,
            "ActorShouldShowSpacesuitGameplayFlashlight");
        var dto = Helpers.GetDTO<ConditionFormDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.ConditionForm,
            "0B1206:Starfield.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }
}
