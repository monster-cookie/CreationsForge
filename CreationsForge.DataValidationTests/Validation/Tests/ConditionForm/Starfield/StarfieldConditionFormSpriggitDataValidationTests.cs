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

        Helpers.GetSpriggitField(spriggit, "ComparisonValue[0]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[0]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[1]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[1]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[10]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[10]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[11]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[11]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[12]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[12]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[13]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[13]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[14]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[14]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[15]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[15]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[16]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[16]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[17]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[17]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[18]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[18]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[19]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[19]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[2]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[2]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[20]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[20]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[21]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[21]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[22]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[22]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[23]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[23]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[24]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[24]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[25]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[25]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[26]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[26]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[27]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[27]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[3]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[3]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[4]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[4]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[5]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[5]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[6]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[6]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[7]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[7]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[8]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[8]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[9]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[9]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[1]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[10]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[10]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[11]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[11]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[12]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[12]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[13]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[13]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[14]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[14]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[15]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[15]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[16]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[16]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[17]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[17]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[18]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[18]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[19]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[19]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[2]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[2]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[20]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[20]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[21]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[21]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[22]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[22]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[23]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[23]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[24]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[24]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[25]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[25]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[26]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[26]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[27]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[27]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[3]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[3]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[4]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[4]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[5]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[5]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[6]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[6]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[7]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[7]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[8]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[8]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[9]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[9]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[1]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[10]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[10]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[11]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[11]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[12]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[12]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[13]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[13]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[14]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[14]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[15]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[15]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[16]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[16]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[17]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[17]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[18]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[18]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[19]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[19]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[2]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[2]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[20]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[20]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[21]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[21]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[22]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[22]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[23]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[23]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[24]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[24]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[25]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[25]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[26]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[26]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[27]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[27]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[3]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[3]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[4]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[4]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[5]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[5]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[6]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[6]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[7]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[7]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[8]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[8]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[9]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[9]"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "OwnerQuest").ShouldBe(Helpers.GetDTOField(dto, "OwnerQuest"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "ComparisonValue[0]", "ComparisonValue[1]", "ComparisonValue[10]", "ComparisonValue[11]", "ComparisonValue[12]", "ComparisonValue[13]", "ComparisonValue[14]", "ComparisonValue[15]", "ComparisonValue[16]", "ComparisonValue[17]", "ComparisonValue[18]", "ComparisonValue[19]", "ComparisonValue[2]", "ComparisonValue[20]", "ComparisonValue[21]", "ComparisonValue[22]", "ComparisonValue[23]", "ComparisonValue[24]", "ComparisonValue[25]", "ComparisonValue[26]", "ComparisonValue[27]", "ComparisonValue[3]", "ComparisonValue[4]", "ComparisonValue[5]", "ComparisonValue[6]", "ComparisonValue[7]", "ComparisonValue[8]", "ComparisonValue[9]", "Data.FirstParameter[0]", "Data.FirstParameter[1]", "Data.FirstParameter[10]", "Data.FirstParameter[11]", "Data.FirstParameter[12]", "Data.FirstParameter[13]", "Data.FirstParameter[14]", "Data.FirstParameter[15]", "Data.FirstParameter[16]", "Data.FirstParameter[17]", "Data.FirstParameter[18]", "Data.FirstParameter[19]", "Data.FirstParameter[2]", "Data.FirstParameter[20]", "Data.FirstParameter[21]", "Data.FirstParameter[22]", "Data.FirstParameter[23]", "Data.FirstParameter[24]", "Data.FirstParameter[25]", "Data.FirstParameter[26]", "Data.FirstParameter[27]", "Data.FirstParameter[3]", "Data.FirstParameter[4]", "Data.FirstParameter[5]", "Data.FirstParameter[6]", "Data.FirstParameter[7]", "Data.FirstParameter[8]", "Data.FirstParameter[9]", "Data.MutagenObjectType[0]", "Data.MutagenObjectType[1]", "Data.MutagenObjectType[10]", "Data.MutagenObjectType[11]", "Data.MutagenObjectType[12]", "Data.MutagenObjectType[13]", "Data.MutagenObjectType[14]", "Data.MutagenObjectType[15]", "Data.MutagenObjectType[16]", "Data.MutagenObjectType[17]", "Data.MutagenObjectType[18]", "Data.MutagenObjectType[19]", "Data.MutagenObjectType[2]", "Data.MutagenObjectType[20]", "Data.MutagenObjectType[21]", "Data.MutagenObjectType[22]", "Data.MutagenObjectType[23]", "Data.MutagenObjectType[24]", "Data.MutagenObjectType[25]", "Data.MutagenObjectType[26]", "Data.MutagenObjectType[27]", "Data.MutagenObjectType[3]", "Data.MutagenObjectType[4]", "Data.MutagenObjectType[5]", "Data.MutagenObjectType[6]", "Data.MutagenObjectType[7]", "Data.MutagenObjectType[8]", "Data.MutagenObjectType[9]", "EditorID", "FormKey", "FormVersion", "OwnerQuest", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "ComparisonValue[0]", "ComparisonValue[1]", "ComparisonValue[10]", "ComparisonValue[11]", "ComparisonValue[12]", "ComparisonValue[13]", "ComparisonValue[14]", "ComparisonValue[15]", "ComparisonValue[16]", "ComparisonValue[17]", "ComparisonValue[18]", "ComparisonValue[19]", "ComparisonValue[2]", "ComparisonValue[20]", "ComparisonValue[21]", "ComparisonValue[22]", "ComparisonValue[23]", "ComparisonValue[24]", "ComparisonValue[25]", "ComparisonValue[26]", "ComparisonValue[27]", "ComparisonValue[3]", "ComparisonValue[4]", "ComparisonValue[5]", "ComparisonValue[6]", "ComparisonValue[7]", "ComparisonValue[8]", "ComparisonValue[9]", "Data.FirstParameter[0]", "Data.FirstParameter[1]", "Data.FirstParameter[10]", "Data.FirstParameter[11]", "Data.FirstParameter[12]", "Data.FirstParameter[13]", "Data.FirstParameter[14]", "Data.FirstParameter[15]", "Data.FirstParameter[16]", "Data.FirstParameter[17]", "Data.FirstParameter[18]", "Data.FirstParameter[19]", "Data.FirstParameter[2]", "Data.FirstParameter[20]", "Data.FirstParameter[21]", "Data.FirstParameter[22]", "Data.FirstParameter[23]", "Data.FirstParameter[24]", "Data.FirstParameter[25]", "Data.FirstParameter[26]", "Data.FirstParameter[27]", "Data.FirstParameter[3]", "Data.FirstParameter[4]", "Data.FirstParameter[5]", "Data.FirstParameter[6]", "Data.FirstParameter[7]", "Data.FirstParameter[8]", "Data.FirstParameter[9]", "Data.MutagenObjectType[0]", "Data.MutagenObjectType[1]", "Data.MutagenObjectType[10]", "Data.MutagenObjectType[11]", "Data.MutagenObjectType[12]", "Data.MutagenObjectType[13]", "Data.MutagenObjectType[14]", "Data.MutagenObjectType[15]", "Data.MutagenObjectType[16]", "Data.MutagenObjectType[17]", "Data.MutagenObjectType[18]", "Data.MutagenObjectType[19]", "Data.MutagenObjectType[2]", "Data.MutagenObjectType[20]", "Data.MutagenObjectType[21]", "Data.MutagenObjectType[22]", "Data.MutagenObjectType[23]", "Data.MutagenObjectType[24]", "Data.MutagenObjectType[25]", "Data.MutagenObjectType[26]", "Data.MutagenObjectType[27]", "Data.MutagenObjectType[3]", "Data.MutagenObjectType[4]", "Data.MutagenObjectType[5]", "Data.MutagenObjectType[6]", "Data.MutagenObjectType[7]", "Data.MutagenObjectType[8]", "Data.MutagenObjectType[9]", "EditorID", "FormKey", "FormVersion", "OwnerQuest", "Version2", "VersionControl");
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

        Helpers.GetSpriggitField(spriggit, "ComparisonValue[0]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[0]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[1]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[1]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[10]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[10]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[11]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[11]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[12]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[12]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[13]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[13]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[14]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[14]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[15]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[15]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[16]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[16]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[17]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[17]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[18]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[18]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[2]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[2]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[3]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[3]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[4]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[4]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[5]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[5]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[6]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[6]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[7]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[7]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[8]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[8]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[9]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[9]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[1]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[10]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[10]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[11]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[11]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[12]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[12]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[13]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[13]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[14]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[14]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[15]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[15]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[16]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[16]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[17]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[17]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[18]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[18]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[2]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[2]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[3]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[3]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[4]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[4]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[5]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[5]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[6]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[6]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[7]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[7]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[8]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[8]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[9]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[9]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[1]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[10]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[10]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[11]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[11]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[12]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[12]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[13]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[13]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[14]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[14]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[15]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[15]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[16]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[16]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[17]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[17]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[18]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[18]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[2]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[2]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[3]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[3]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[4]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[4]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[5]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[5]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[6]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[6]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[7]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[7]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[8]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[8]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[9]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[9]"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "ComparisonValue[0]", "ComparisonValue[1]", "ComparisonValue[10]", "ComparisonValue[11]", "ComparisonValue[12]", "ComparisonValue[13]", "ComparisonValue[14]", "ComparisonValue[15]", "ComparisonValue[16]", "ComparisonValue[17]", "ComparisonValue[18]", "ComparisonValue[2]", "ComparisonValue[3]", "ComparisonValue[4]", "ComparisonValue[5]", "ComparisonValue[6]", "ComparisonValue[7]", "ComparisonValue[8]", "ComparisonValue[9]", "Data.FirstParameter[0]", "Data.FirstParameter[1]", "Data.FirstParameter[10]", "Data.FirstParameter[11]", "Data.FirstParameter[12]", "Data.FirstParameter[13]", "Data.FirstParameter[14]", "Data.FirstParameter[15]", "Data.FirstParameter[16]", "Data.FirstParameter[17]", "Data.FirstParameter[18]", "Data.FirstParameter[2]", "Data.FirstParameter[3]", "Data.FirstParameter[4]", "Data.FirstParameter[5]", "Data.FirstParameter[6]", "Data.FirstParameter[7]", "Data.FirstParameter[8]", "Data.FirstParameter[9]", "Data.MutagenObjectType[0]", "Data.MutagenObjectType[1]", "Data.MutagenObjectType[10]", "Data.MutagenObjectType[11]", "Data.MutagenObjectType[12]", "Data.MutagenObjectType[13]", "Data.MutagenObjectType[14]", "Data.MutagenObjectType[15]", "Data.MutagenObjectType[16]", "Data.MutagenObjectType[17]", "Data.MutagenObjectType[18]", "Data.MutagenObjectType[2]", "Data.MutagenObjectType[3]", "Data.MutagenObjectType[4]", "Data.MutagenObjectType[5]", "Data.MutagenObjectType[6]", "Data.MutagenObjectType[7]", "Data.MutagenObjectType[8]", "Data.MutagenObjectType[9]", "EditorID", "FormKey", "FormVersion", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "ComparisonValue[0]", "ComparisonValue[1]", "ComparisonValue[10]", "ComparisonValue[11]", "ComparisonValue[12]", "ComparisonValue[13]", "ComparisonValue[14]", "ComparisonValue[15]", "ComparisonValue[16]", "ComparisonValue[17]", "ComparisonValue[18]", "ComparisonValue[2]", "ComparisonValue[3]", "ComparisonValue[4]", "ComparisonValue[5]", "ComparisonValue[6]", "ComparisonValue[7]", "ComparisonValue[8]", "ComparisonValue[9]", "Data.FirstParameter[0]", "Data.FirstParameter[1]", "Data.FirstParameter[10]", "Data.FirstParameter[11]", "Data.FirstParameter[12]", "Data.FirstParameter[13]", "Data.FirstParameter[14]", "Data.FirstParameter[15]", "Data.FirstParameter[16]", "Data.FirstParameter[17]", "Data.FirstParameter[18]", "Data.FirstParameter[2]", "Data.FirstParameter[3]", "Data.FirstParameter[4]", "Data.FirstParameter[5]", "Data.FirstParameter[6]", "Data.FirstParameter[7]", "Data.FirstParameter[8]", "Data.FirstParameter[9]", "Data.MutagenObjectType[0]", "Data.MutagenObjectType[1]", "Data.MutagenObjectType[10]", "Data.MutagenObjectType[11]", "Data.MutagenObjectType[12]", "Data.MutagenObjectType[13]", "Data.MutagenObjectType[14]", "Data.MutagenObjectType[15]", "Data.MutagenObjectType[16]", "Data.MutagenObjectType[17]", "Data.MutagenObjectType[18]", "Data.MutagenObjectType[2]", "Data.MutagenObjectType[3]", "Data.MutagenObjectType[4]", "Data.MutagenObjectType[5]", "Data.MutagenObjectType[6]", "Data.MutagenObjectType[7]", "Data.MutagenObjectType[8]", "Data.MutagenObjectType[9]", "EditorID", "FormKey", "FormVersion", "Version2", "VersionControl");
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

        Helpers.GetSpriggitField(spriggit, "CompareOperator[0]").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator[0]"));
        Helpers.GetSpriggitField(spriggit, "CompareOperator[1]").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator[1]"));
        Helpers.GetSpriggitField(spriggit, "CompareOperator[10]").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator[10]"));
        Helpers.GetSpriggitField(spriggit, "CompareOperator[11]").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator[11]"));
        Helpers.GetSpriggitField(spriggit, "CompareOperator[12]").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator[12]"));
        Helpers.GetSpriggitField(spriggit, "CompareOperator[13]").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator[13]"));
        Helpers.GetSpriggitField(spriggit, "CompareOperator[14]").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator[14]"));
        Helpers.GetSpriggitField(spriggit, "CompareOperator[15]").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator[15]"));
        Helpers.GetSpriggitField(spriggit, "CompareOperator[16]").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator[16]"));
        Helpers.GetSpriggitField(spriggit, "CompareOperator[17]").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator[17]"));
        Helpers.GetSpriggitField(spriggit, "CompareOperator[18]").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator[18]"));
        Helpers.GetSpriggitField(spriggit, "CompareOperator[19]").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator[19]"));
        Helpers.GetSpriggitField(spriggit, "CompareOperator[2]").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator[2]"));
        Helpers.GetSpriggitField(spriggit, "CompareOperator[20]").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator[20]"));
        Helpers.GetSpriggitField(spriggit, "CompareOperator[21]").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator[21]"));
        Helpers.GetSpriggitField(spriggit, "CompareOperator[22]").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator[22]"));
        Helpers.GetSpriggitField(spriggit, "CompareOperator[23]").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator[23]"));
        Helpers.GetSpriggitField(spriggit, "CompareOperator[24]").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator[24]"));
        Helpers.GetSpriggitField(spriggit, "CompareOperator[25]").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator[25]"));
        Helpers.GetSpriggitField(spriggit, "CompareOperator[26]").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator[26]"));
        Helpers.GetSpriggitField(spriggit, "CompareOperator[27]").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator[27]"));
        Helpers.GetSpriggitField(spriggit, "CompareOperator[28]").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator[28]"));
        Helpers.GetSpriggitField(spriggit, "CompareOperator[29]").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator[29]"));
        Helpers.GetSpriggitField(spriggit, "CompareOperator[3]").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator[3]"));
        Helpers.GetSpriggitField(spriggit, "CompareOperator[30]").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator[30]"));
        Helpers.GetSpriggitField(spriggit, "CompareOperator[31]").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator[31]"));
        Helpers.GetSpriggitField(spriggit, "CompareOperator[32]").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator[32]"));
        Helpers.GetSpriggitField(spriggit, "CompareOperator[33]").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator[33]"));
        Helpers.GetSpriggitField(spriggit, "CompareOperator[34]").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator[34]"));
        Helpers.GetSpriggitField(spriggit, "CompareOperator[35]").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator[35]"));
        Helpers.GetSpriggitField(spriggit, "CompareOperator[36]").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator[36]"));
        Helpers.GetSpriggitField(spriggit, "CompareOperator[37]").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator[37]"));
        Helpers.GetSpriggitField(spriggit, "CompareOperator[38]").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator[38]"));
        Helpers.GetSpriggitField(spriggit, "CompareOperator[39]").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator[39]"));
        Helpers.GetSpriggitField(spriggit, "CompareOperator[4]").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator[4]"));
        Helpers.GetSpriggitField(spriggit, "CompareOperator[40]").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator[40]"));
        Helpers.GetSpriggitField(spriggit, "CompareOperator[41]").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator[41]"));
        Helpers.GetSpriggitField(spriggit, "CompareOperator[42]").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator[42]"));
        Helpers.GetSpriggitField(spriggit, "CompareOperator[43]").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator[43]"));
        Helpers.GetSpriggitField(spriggit, "CompareOperator[44]").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator[44]"));
        Helpers.GetSpriggitField(spriggit, "CompareOperator[5]").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator[5]"));
        Helpers.GetSpriggitField(spriggit, "CompareOperator[6]").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator[6]"));
        Helpers.GetSpriggitField(spriggit, "CompareOperator[7]").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator[7]"));
        Helpers.GetSpriggitField(spriggit, "CompareOperator[8]").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator[8]"));
        Helpers.GetSpriggitField(spriggit, "CompareOperator[9]").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator[9]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[1]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[10]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[10]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[11]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[11]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[12]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[12]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[13]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[13]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[14]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[14]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[15]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[15]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[16]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[16]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[17]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[17]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[18]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[18]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[19]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[19]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[2]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[2]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[20]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[20]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[21]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[21]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[22]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[22]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[23]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[23]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[24]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[24]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[25]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[25]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[26]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[26]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[27]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[27]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[28]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[28]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[29]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[29]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[3]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[3]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[30]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[30]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[31]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[31]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[32]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[32]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[33]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[33]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[34]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[34]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[35]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[35]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[36]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[36]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[37]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[37]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[38]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[38]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[39]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[39]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[4]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[4]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[40]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[40]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[41]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[41]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[42]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[42]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[43]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[43]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[44]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[44]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[5]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[5]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[6]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[6]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[7]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[7]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[8]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[8]"));
        Helpers.GetSpriggitField(spriggit, "Data.FirstParameter[9]").ShouldBe(Helpers.GetDTOField(dto, "Data.FirstParameter[9]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[1]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[10]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[10]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[11]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[11]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[12]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[12]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[13]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[13]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[14]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[14]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[15]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[15]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[16]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[16]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[17]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[17]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[18]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[18]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[19]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[19]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[2]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[2]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[20]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[20]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[21]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[21]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[22]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[22]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[23]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[23]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[24]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[24]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[25]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[25]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[26]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[26]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[27]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[27]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[28]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[28]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[29]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[29]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[3]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[3]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[30]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[30]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[31]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[31]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[32]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[32]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[33]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[33]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[34]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[34]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[35]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[35]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[36]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[36]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[37]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[37]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[38]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[38]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[39]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[39]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[4]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[4]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[40]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[40]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[41]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[41]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[42]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[42]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[43]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[43]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[44]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[44]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[5]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[5]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[6]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[6]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[7]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[7]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[8]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[8]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[9]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[9]"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "CompareOperator[0]", "CompareOperator[1]", "CompareOperator[10]", "CompareOperator[11]", "CompareOperator[12]", "CompareOperator[13]", "CompareOperator[14]", "CompareOperator[15]", "CompareOperator[16]", "CompareOperator[17]", "CompareOperator[18]", "CompareOperator[19]", "CompareOperator[2]", "CompareOperator[20]", "CompareOperator[21]", "CompareOperator[22]", "CompareOperator[23]", "CompareOperator[24]", "CompareOperator[25]", "CompareOperator[26]", "CompareOperator[27]", "CompareOperator[28]", "CompareOperator[29]", "CompareOperator[3]", "CompareOperator[30]", "CompareOperator[31]", "CompareOperator[32]", "CompareOperator[33]", "CompareOperator[34]", "CompareOperator[35]", "CompareOperator[36]", "CompareOperator[37]", "CompareOperator[38]", "CompareOperator[39]", "CompareOperator[4]", "CompareOperator[40]", "CompareOperator[41]", "CompareOperator[42]", "CompareOperator[43]", "CompareOperator[44]", "CompareOperator[5]", "CompareOperator[6]", "CompareOperator[7]", "CompareOperator[8]", "CompareOperator[9]", "Data.FirstParameter[0]", "Data.FirstParameter[1]", "Data.FirstParameter[10]", "Data.FirstParameter[11]", "Data.FirstParameter[12]", "Data.FirstParameter[13]", "Data.FirstParameter[14]", "Data.FirstParameter[15]", "Data.FirstParameter[16]", "Data.FirstParameter[17]", "Data.FirstParameter[18]", "Data.FirstParameter[19]", "Data.FirstParameter[2]", "Data.FirstParameter[20]", "Data.FirstParameter[21]", "Data.FirstParameter[22]", "Data.FirstParameter[23]", "Data.FirstParameter[24]", "Data.FirstParameter[25]", "Data.FirstParameter[26]", "Data.FirstParameter[27]", "Data.FirstParameter[28]", "Data.FirstParameter[29]", "Data.FirstParameter[3]", "Data.FirstParameter[30]", "Data.FirstParameter[31]", "Data.FirstParameter[32]", "Data.FirstParameter[33]", "Data.FirstParameter[34]", "Data.FirstParameter[35]", "Data.FirstParameter[36]", "Data.FirstParameter[37]", "Data.FirstParameter[38]", "Data.FirstParameter[39]", "Data.FirstParameter[4]", "Data.FirstParameter[40]", "Data.FirstParameter[41]", "Data.FirstParameter[42]", "Data.FirstParameter[43]", "Data.FirstParameter[44]", "Data.FirstParameter[5]", "Data.FirstParameter[6]", "Data.FirstParameter[7]", "Data.FirstParameter[8]", "Data.FirstParameter[9]", "Data.MutagenObjectType[0]", "Data.MutagenObjectType[1]", "Data.MutagenObjectType[10]", "Data.MutagenObjectType[11]", "Data.MutagenObjectType[12]", "Data.MutagenObjectType[13]", "Data.MutagenObjectType[14]", "Data.MutagenObjectType[15]", "Data.MutagenObjectType[16]", "Data.MutagenObjectType[17]", "Data.MutagenObjectType[18]", "Data.MutagenObjectType[19]", "Data.MutagenObjectType[2]", "Data.MutagenObjectType[20]", "Data.MutagenObjectType[21]", "Data.MutagenObjectType[22]", "Data.MutagenObjectType[23]", "Data.MutagenObjectType[24]", "Data.MutagenObjectType[25]", "Data.MutagenObjectType[26]", "Data.MutagenObjectType[27]", "Data.MutagenObjectType[28]", "Data.MutagenObjectType[29]", "Data.MutagenObjectType[3]", "Data.MutagenObjectType[30]", "Data.MutagenObjectType[31]", "Data.MutagenObjectType[32]", "Data.MutagenObjectType[33]", "Data.MutagenObjectType[34]", "Data.MutagenObjectType[35]", "Data.MutagenObjectType[36]", "Data.MutagenObjectType[37]", "Data.MutagenObjectType[38]", "Data.MutagenObjectType[39]", "Data.MutagenObjectType[4]", "Data.MutagenObjectType[40]", "Data.MutagenObjectType[41]", "Data.MutagenObjectType[42]", "Data.MutagenObjectType[43]", "Data.MutagenObjectType[44]", "Data.MutagenObjectType[5]", "Data.MutagenObjectType[6]", "Data.MutagenObjectType[7]", "Data.MutagenObjectType[8]", "Data.MutagenObjectType[9]", "EditorID", "FormKey", "FormVersion", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "CompareOperator[0]", "CompareOperator[1]", "CompareOperator[10]", "CompareOperator[11]", "CompareOperator[12]", "CompareOperator[13]", "CompareOperator[14]", "CompareOperator[15]", "CompareOperator[16]", "CompareOperator[17]", "CompareOperator[18]", "CompareOperator[19]", "CompareOperator[2]", "CompareOperator[20]", "CompareOperator[21]", "CompareOperator[22]", "CompareOperator[23]", "CompareOperator[24]", "CompareOperator[25]", "CompareOperator[26]", "CompareOperator[27]", "CompareOperator[28]", "CompareOperator[29]", "CompareOperator[3]", "CompareOperator[30]", "CompareOperator[31]", "CompareOperator[32]", "CompareOperator[33]", "CompareOperator[34]", "CompareOperator[35]", "CompareOperator[36]", "CompareOperator[37]", "CompareOperator[38]", "CompareOperator[39]", "CompareOperator[4]", "CompareOperator[40]", "CompareOperator[41]", "CompareOperator[42]", "CompareOperator[43]", "CompareOperator[44]", "CompareOperator[5]", "CompareOperator[6]", "CompareOperator[7]", "CompareOperator[8]", "CompareOperator[9]", "Data.FirstParameter[0]", "Data.FirstParameter[1]", "Data.FirstParameter[10]", "Data.FirstParameter[11]", "Data.FirstParameter[12]", "Data.FirstParameter[13]", "Data.FirstParameter[14]", "Data.FirstParameter[15]", "Data.FirstParameter[16]", "Data.FirstParameter[17]", "Data.FirstParameter[18]", "Data.FirstParameter[19]", "Data.FirstParameter[2]", "Data.FirstParameter[20]", "Data.FirstParameter[21]", "Data.FirstParameter[22]", "Data.FirstParameter[23]", "Data.FirstParameter[24]", "Data.FirstParameter[25]", "Data.FirstParameter[26]", "Data.FirstParameter[27]", "Data.FirstParameter[28]", "Data.FirstParameter[29]", "Data.FirstParameter[3]", "Data.FirstParameter[30]", "Data.FirstParameter[31]", "Data.FirstParameter[32]", "Data.FirstParameter[33]", "Data.FirstParameter[34]", "Data.FirstParameter[35]", "Data.FirstParameter[36]", "Data.FirstParameter[37]", "Data.FirstParameter[38]", "Data.FirstParameter[39]", "Data.FirstParameter[4]", "Data.FirstParameter[40]", "Data.FirstParameter[41]", "Data.FirstParameter[42]", "Data.FirstParameter[43]", "Data.FirstParameter[44]", "Data.FirstParameter[5]", "Data.FirstParameter[6]", "Data.FirstParameter[7]", "Data.FirstParameter[8]", "Data.FirstParameter[9]", "Data.MutagenObjectType[0]", "Data.MutagenObjectType[1]", "Data.MutagenObjectType[10]", "Data.MutagenObjectType[11]", "Data.MutagenObjectType[12]", "Data.MutagenObjectType[13]", "Data.MutagenObjectType[14]", "Data.MutagenObjectType[15]", "Data.MutagenObjectType[16]", "Data.MutagenObjectType[17]", "Data.MutagenObjectType[18]", "Data.MutagenObjectType[19]", "Data.MutagenObjectType[2]", "Data.MutagenObjectType[20]", "Data.MutagenObjectType[21]", "Data.MutagenObjectType[22]", "Data.MutagenObjectType[23]", "Data.MutagenObjectType[24]", "Data.MutagenObjectType[25]", "Data.MutagenObjectType[26]", "Data.MutagenObjectType[27]", "Data.MutagenObjectType[28]", "Data.MutagenObjectType[29]", "Data.MutagenObjectType[3]", "Data.MutagenObjectType[30]", "Data.MutagenObjectType[31]", "Data.MutagenObjectType[32]", "Data.MutagenObjectType[33]", "Data.MutagenObjectType[34]", "Data.MutagenObjectType[35]", "Data.MutagenObjectType[36]", "Data.MutagenObjectType[37]", "Data.MutagenObjectType[38]", "Data.MutagenObjectType[39]", "Data.MutagenObjectType[4]", "Data.MutagenObjectType[40]", "Data.MutagenObjectType[41]", "Data.MutagenObjectType[42]", "Data.MutagenObjectType[43]", "Data.MutagenObjectType[44]", "Data.MutagenObjectType[5]", "Data.MutagenObjectType[6]", "Data.MutagenObjectType[7]", "Data.MutagenObjectType[8]", "Data.MutagenObjectType[9]", "EditorID", "FormKey", "FormVersion", "Version2", "VersionControl");
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

        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "EditorID", "FormKey", "FormVersion", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "EditorID", "FormKey", "FormVersion", "Version2", "VersionControl");
    }
}