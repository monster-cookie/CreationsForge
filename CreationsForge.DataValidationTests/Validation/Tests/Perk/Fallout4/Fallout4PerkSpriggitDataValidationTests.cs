using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.Perk.Fallout4;

public class Fallout4PerkSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "PERK")]
    [Trait("FormKey", "2458BA:Fallout4.esm")]
    [Trait("EditorID", "AddictionManager")]
    [Trait("SpriggitFile", "Perks/AddictionManager - 2458BA_Fallout4.esm.yaml")]
    public void Fallout4_PERK_ShouldMatchSpriggitSample_AddictionManager()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Perk,
            "AddictionManager");
        var dto = Helpers.GetDTO<PerkDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Perk,
            "2458BA:Fallout4.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["ActorValue[0]"].ShouldBe(dtoFields["ActorValue[0]"]);
        spriggitFields["ActorValue[1]"].ShouldBe(dtoFields["ActorValue[1]"]);
        spriggitFields["ActorValue[10]"].ShouldBe(dtoFields["ActorValue[10]"]);
        spriggitFields["ActorValue[11]"].ShouldBe(dtoFields["ActorValue[11]"]);
        spriggitFields["ActorValue[12]"].ShouldBe(dtoFields["ActorValue[12]"]);
        spriggitFields["ActorValue[13]"].ShouldBe(dtoFields["ActorValue[13]"]);
        spriggitFields["ActorValue[2]"].ShouldBe(dtoFields["ActorValue[2]"]);
        spriggitFields["ActorValue[3]"].ShouldBe(dtoFields["ActorValue[3]"]);
        spriggitFields["ActorValue[4]"].ShouldBe(dtoFields["ActorValue[4]"]);
        spriggitFields["ActorValue[5]"].ShouldBe(dtoFields["ActorValue[5]"]);
        spriggitFields["ActorValue[6]"].ShouldBe(dtoFields["ActorValue[6]"]);
        spriggitFields["ActorValue[7]"].ShouldBe(dtoFields["ActorValue[7]"]);
        spriggitFields["ActorValue[8]"].ShouldBe(dtoFields["ActorValue[8]"]);
        spriggitFields["ActorValue[9]"].ShouldBe(dtoFields["ActorValue[9]"]);
        spriggitFields["CompareOperator"].ShouldBe(dtoFields["CompareOperator"]);
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
        spriggitFields["ComparisonValue[3]"].ShouldBe(dtoFields["ComparisonValue[3]"]);
        spriggitFields["ComparisonValue[4]"].ShouldBe(dtoFields["ComparisonValue[4]"]);
        spriggitFields["ComparisonValue[5]"].ShouldBe(dtoFields["ComparisonValue[5]"]);
        spriggitFields["ComparisonValue[6]"].ShouldBe(dtoFields["ComparisonValue[6]"]);
        spriggitFields["ComparisonValue[7]"].ShouldBe(dtoFields["ComparisonValue[7]"]);
        spriggitFields["ComparisonValue[8]"].ShouldBe(dtoFields["ComparisonValue[8]"]);
        spriggitFields["ComparisonValue[9]"].ShouldBe(dtoFields["ComparisonValue[9]"]);
        spriggitFields["Data.Function[0]"].ShouldBe(dtoFields["Data.Function[0]"]);
        spriggitFields["Data.Function[1]"].ShouldBe(dtoFields["Data.Function[1]"]);
        spriggitFields["Data.Function[10]"].ShouldBe(dtoFields["Data.Function[10]"]);
        spriggitFields["Data.Function[11]"].ShouldBe(dtoFields["Data.Function[11]"]);
        spriggitFields["Data.Function[12]"].ShouldBe(dtoFields["Data.Function[12]"]);
        spriggitFields["Data.Function[13]"].ShouldBe(dtoFields["Data.Function[13]"]);
        spriggitFields["Data.Function[14]"].ShouldBe(dtoFields["Data.Function[14]"]);
        spriggitFields["Data.Function[15]"].ShouldBe(dtoFields["Data.Function[15]"]);
        spriggitFields["Data.Function[16]"].ShouldBe(dtoFields["Data.Function[16]"]);
        spriggitFields["Data.Function[17]"].ShouldBe(dtoFields["Data.Function[17]"]);
        spriggitFields["Data.Function[18]"].ShouldBe(dtoFields["Data.Function[18]"]);
        spriggitFields["Data.Function[19]"].ShouldBe(dtoFields["Data.Function[19]"]);
        spriggitFields["Data.Function[2]"].ShouldBe(dtoFields["Data.Function[2]"]);
        spriggitFields["Data.Function[20]"].ShouldBe(dtoFields["Data.Function[20]"]);
        spriggitFields["Data.Function[21]"].ShouldBe(dtoFields["Data.Function[21]"]);
        spriggitFields["Data.Function[22]"].ShouldBe(dtoFields["Data.Function[22]"]);
        spriggitFields["Data.Function[3]"].ShouldBe(dtoFields["Data.Function[3]"]);
        spriggitFields["Data.Function[4]"].ShouldBe(dtoFields["Data.Function[4]"]);
        spriggitFields["Data.Function[5]"].ShouldBe(dtoFields["Data.Function[5]"]);
        spriggitFields["Data.Function[6]"].ShouldBe(dtoFields["Data.Function[6]"]);
        spriggitFields["Data.Function[7]"].ShouldBe(dtoFields["Data.Function[7]"]);
        spriggitFields["Data.Function[8]"].ShouldBe(dtoFields["Data.Function[8]"]);
        spriggitFields["Data.Function[9]"].ShouldBe(dtoFields["Data.Function[9]"]);
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
        spriggitFields["Data.MutagenObjectType[3]"].ShouldBe(dtoFields["Data.MutagenObjectType[3]"]);
        spriggitFields["Data.MutagenObjectType[4]"].ShouldBe(dtoFields["Data.MutagenObjectType[4]"]);
        spriggitFields["Data.MutagenObjectType[5]"].ShouldBe(dtoFields["Data.MutagenObjectType[5]"]);
        spriggitFields["Data.MutagenObjectType[6]"].ShouldBe(dtoFields["Data.MutagenObjectType[6]"]);
        spriggitFields["Data.MutagenObjectType[7]"].ShouldBe(dtoFields["Data.MutagenObjectType[7]"]);
        spriggitFields["Data.MutagenObjectType[8]"].ShouldBe(dtoFields["Data.MutagenObjectType[8]"]);
        spriggitFields["Data.MutagenObjectType[9]"].ShouldBe(dtoFields["Data.MutagenObjectType[9]"]);
        spriggitFields["Data.ParameterOneNumber[0]"].ShouldBe(dtoFields["Data.ParameterOneNumber[0]"]);
        spriggitFields["Data.ParameterOneNumber[1]"].ShouldBe(dtoFields["Data.ParameterOneNumber[1]"]);
        spriggitFields["Data.ParameterOneNumber[10]"].ShouldBe(dtoFields["Data.ParameterOneNumber[10]"]);
        spriggitFields["Data.ParameterOneNumber[11]"].ShouldBe(dtoFields["Data.ParameterOneNumber[11]"]);
        spriggitFields["Data.ParameterOneNumber[12]"].ShouldBe(dtoFields["Data.ParameterOneNumber[12]"]);
        spriggitFields["Data.ParameterOneNumber[13]"].ShouldBe(dtoFields["Data.ParameterOneNumber[13]"]);
        spriggitFields["Data.ParameterOneNumber[14]"].ShouldBe(dtoFields["Data.ParameterOneNumber[14]"]);
        spriggitFields["Data.ParameterOneNumber[15]"].ShouldBe(dtoFields["Data.ParameterOneNumber[15]"]);
        spriggitFields["Data.ParameterOneNumber[16]"].ShouldBe(dtoFields["Data.ParameterOneNumber[16]"]);
        spriggitFields["Data.ParameterOneNumber[17]"].ShouldBe(dtoFields["Data.ParameterOneNumber[17]"]);
        spriggitFields["Data.ParameterOneNumber[18]"].ShouldBe(dtoFields["Data.ParameterOneNumber[18]"]);
        spriggitFields["Data.ParameterOneNumber[19]"].ShouldBe(dtoFields["Data.ParameterOneNumber[19]"]);
        spriggitFields["Data.ParameterOneNumber[2]"].ShouldBe(dtoFields["Data.ParameterOneNumber[2]"]);
        spriggitFields["Data.ParameterOneNumber[20]"].ShouldBe(dtoFields["Data.ParameterOneNumber[20]"]);
        spriggitFields["Data.ParameterOneNumber[21]"].ShouldBe(dtoFields["Data.ParameterOneNumber[21]"]);
        spriggitFields["Data.ParameterOneNumber[3]"].ShouldBe(dtoFields["Data.ParameterOneNumber[3]"]);
        spriggitFields["Data.ParameterOneNumber[4]"].ShouldBe(dtoFields["Data.ParameterOneNumber[4]"]);
        spriggitFields["Data.ParameterOneNumber[5]"].ShouldBe(dtoFields["Data.ParameterOneNumber[5]"]);
        spriggitFields["Data.ParameterOneNumber[6]"].ShouldBe(dtoFields["Data.ParameterOneNumber[6]"]);
        spriggitFields["Data.ParameterOneNumber[7]"].ShouldBe(dtoFields["Data.ParameterOneNumber[7]"]);
        spriggitFields["Data.ParameterOneNumber[8]"].ShouldBe(dtoFields["Data.ParameterOneNumber[8]"]);
        spriggitFields["Data.ParameterOneNumber[9]"].ShouldBe(dtoFields["Data.ParameterOneNumber[9]"]);
        spriggitFields["Data.ParameterOneRecord[0]"].ShouldBe(dtoFields["Data.ParameterOneRecord[0]"]);
        spriggitFields["Data.ParameterOneRecord[1]"].ShouldBe(dtoFields["Data.ParameterOneRecord[1]"]);
        spriggitFields["Data.ParameterOneRecord[10]"].ShouldBe(dtoFields["Data.ParameterOneRecord[10]"]);
        spriggitFields["Data.ParameterOneRecord[11]"].ShouldBe(dtoFields["Data.ParameterOneRecord[11]"]);
        spriggitFields["Data.ParameterOneRecord[12]"].ShouldBe(dtoFields["Data.ParameterOneRecord[12]"]);
        spriggitFields["Data.ParameterOneRecord[13]"].ShouldBe(dtoFields["Data.ParameterOneRecord[13]"]);
        spriggitFields["Data.ParameterOneRecord[14]"].ShouldBe(dtoFields["Data.ParameterOneRecord[14]"]);
        spriggitFields["Data.ParameterOneRecord[15]"].ShouldBe(dtoFields["Data.ParameterOneRecord[15]"]);
        spriggitFields["Data.ParameterOneRecord[16]"].ShouldBe(dtoFields["Data.ParameterOneRecord[16]"]);
        spriggitFields["Data.ParameterOneRecord[17]"].ShouldBe(dtoFields["Data.ParameterOneRecord[17]"]);
        spriggitFields["Data.ParameterOneRecord[18]"].ShouldBe(dtoFields["Data.ParameterOneRecord[18]"]);
        spriggitFields["Data.ParameterOneRecord[19]"].ShouldBe(dtoFields["Data.ParameterOneRecord[19]"]);
        spriggitFields["Data.ParameterOneRecord[2]"].ShouldBe(dtoFields["Data.ParameterOneRecord[2]"]);
        spriggitFields["Data.ParameterOneRecord[20]"].ShouldBe(dtoFields["Data.ParameterOneRecord[20]"]);
        spriggitFields["Data.ParameterOneRecord[21]"].ShouldBe(dtoFields["Data.ParameterOneRecord[21]"]);
        spriggitFields["Data.ParameterOneRecord[3]"].ShouldBe(dtoFields["Data.ParameterOneRecord[3]"]);
        spriggitFields["Data.ParameterOneRecord[4]"].ShouldBe(dtoFields["Data.ParameterOneRecord[4]"]);
        spriggitFields["Data.ParameterOneRecord[5]"].ShouldBe(dtoFields["Data.ParameterOneRecord[5]"]);
        spriggitFields["Data.ParameterOneRecord[6]"].ShouldBe(dtoFields["Data.ParameterOneRecord[6]"]);
        spriggitFields["Data.ParameterOneRecord[7]"].ShouldBe(dtoFields["Data.ParameterOneRecord[7]"]);
        spriggitFields["Data.ParameterOneRecord[8]"].ShouldBe(dtoFields["Data.ParameterOneRecord[8]"]);
        spriggitFields["Data.ParameterOneRecord[9]"].ShouldBe(dtoFields["Data.ParameterOneRecord[9]"]);
        spriggitFields["Description.TargetLanguage"].ShouldBe(dtoFields["Description.TargetLanguage"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["EntryPoint[0]"].ShouldBe(dtoFields["EntryPoint[0]"]);
        spriggitFields["EntryPoint[1]"].ShouldBe(dtoFields["EntryPoint[1]"]);
        spriggitFields["EntryPoint[10]"].ShouldBe(dtoFields["EntryPoint[10]"]);
        spriggitFields["EntryPoint[11]"].ShouldBe(dtoFields["EntryPoint[11]"]);
        spriggitFields["EntryPoint[12]"].ShouldBe(dtoFields["EntryPoint[12]"]);
        spriggitFields["EntryPoint[13]"].ShouldBe(dtoFields["EntryPoint[13]"]);
        spriggitFields["EntryPoint[14]"].ShouldBe(dtoFields["EntryPoint[14]"]);
        spriggitFields["EntryPoint[15]"].ShouldBe(dtoFields["EntryPoint[15]"]);
        spriggitFields["EntryPoint[16]"].ShouldBe(dtoFields["EntryPoint[16]"]);
        spriggitFields["EntryPoint[2]"].ShouldBe(dtoFields["EntryPoint[2]"]);
        spriggitFields["EntryPoint[3]"].ShouldBe(dtoFields["EntryPoint[3]"]);
        spriggitFields["EntryPoint[4]"].ShouldBe(dtoFields["EntryPoint[4]"]);
        spriggitFields["EntryPoint[5]"].ShouldBe(dtoFields["EntryPoint[5]"]);
        spriggitFields["EntryPoint[6]"].ShouldBe(dtoFields["EntryPoint[6]"]);
        spriggitFields["EntryPoint[7]"].ShouldBe(dtoFields["EntryPoint[7]"]);
        spriggitFields["EntryPoint[8]"].ShouldBe(dtoFields["EntryPoint[8]"]);
        spriggitFields["EntryPoint[9]"].ShouldBe(dtoFields["EntryPoint[9]"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["Hidden"].ShouldBe(dtoFields["Hidden"]);
        spriggitFields["MajorRecordFlagsRaw"].ShouldBe(dtoFields["MajorRecordFlags"]);
        spriggitFields["Modification[0]"].ShouldBe(dtoFields["Modification[0]"]);
        spriggitFields["Modification[1]"].ShouldBe(dtoFields["Modification[1]"]);
        spriggitFields["Name.Count"].ShouldBe(dtoFields["Name.Count"]);
        spriggitFields["Name.TargetLanguage"].ShouldBe(dtoFields["Name.TargetLanguage"]);
        spriggitFields["Name[0].Language"].ShouldBe(dtoFields["Name[0].Language"]);
        spriggitFields["Name[0].String"].ShouldBe(dtoFields["Name[0].String"]);
        spriggitFields["Name[1].Language"].ShouldBe(dtoFields["Name[1].Language"]);
        spriggitFields["Name[1].String"].ShouldBe(dtoFields["Name[1].String"]);
        spriggitFields["Name[10].Language"].ShouldBe(dtoFields["Name[10].Language"]);
        spriggitFields["Name[10].String"].ShouldBe(dtoFields["Name[10].String"]);
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
        spriggitFields["Name[9].Language"].ShouldBe(dtoFields["Name[9].Language"]);
        spriggitFields["Name[9].String"].ShouldBe(dtoFields["Name[9].String"]);
        spriggitFields["NumRanks"].ShouldBe(dtoFields["NumRanks"]);
        spriggitFields["PerkConditionTabCount[0]"].ShouldBe(dtoFields["PerkConditionTabCount[0]"]);
        spriggitFields["PerkConditionTabCount[1]"].ShouldBe(dtoFields["PerkConditionTabCount[1]"]);
        spriggitFields["PerkConditionTabCount[10]"].ShouldBe(dtoFields["PerkConditionTabCount[10]"]);
        spriggitFields["PerkConditionTabCount[11]"].ShouldBe(dtoFields["PerkConditionTabCount[11]"]);
        spriggitFields["PerkConditionTabCount[12]"].ShouldBe(dtoFields["PerkConditionTabCount[12]"]);
        spriggitFields["PerkConditionTabCount[13]"].ShouldBe(dtoFields["PerkConditionTabCount[13]"]);
        spriggitFields["PerkConditionTabCount[14]"].ShouldBe(dtoFields["PerkConditionTabCount[14]"]);
        spriggitFields["PerkConditionTabCount[15]"].ShouldBe(dtoFields["PerkConditionTabCount[15]"]);
        spriggitFields["PerkConditionTabCount[16]"].ShouldBe(dtoFields["PerkConditionTabCount[16]"]);
        spriggitFields["PerkConditionTabCount[2]"].ShouldBe(dtoFields["PerkConditionTabCount[2]"]);
        spriggitFields["PerkConditionTabCount[3]"].ShouldBe(dtoFields["PerkConditionTabCount[3]"]);
        spriggitFields["PerkConditionTabCount[4]"].ShouldBe(dtoFields["PerkConditionTabCount[4]"]);
        spriggitFields["PerkConditionTabCount[5]"].ShouldBe(dtoFields["PerkConditionTabCount[5]"]);
        spriggitFields["PerkConditionTabCount[6]"].ShouldBe(dtoFields["PerkConditionTabCount[6]"]);
        spriggitFields["PerkConditionTabCount[7]"].ShouldBe(dtoFields["PerkConditionTabCount[7]"]);
        spriggitFields["PerkConditionTabCount[8]"].ShouldBe(dtoFields["PerkConditionTabCount[8]"]);
        spriggitFields["PerkConditionTabCount[9]"].ShouldBe(dtoFields["PerkConditionTabCount[9]"]);
        spriggitFields["PerkEntryID[0]"].ShouldBe(dtoFields["PerkEntryID[0]"]);
        spriggitFields["PerkEntryID[1]"].ShouldBe(dtoFields["PerkEntryID[1]"]);
        spriggitFields["PerkEntryID[10]"].ShouldBe(dtoFields["PerkEntryID[10]"]);
        spriggitFields["PerkEntryID[11]"].ShouldBe(dtoFields["PerkEntryID[11]"]);
        spriggitFields["PerkEntryID[12]"].ShouldBe(dtoFields["PerkEntryID[12]"]);
        spriggitFields["PerkEntryID[13]"].ShouldBe(dtoFields["PerkEntryID[13]"]);
        spriggitFields["PerkEntryID[14]"].ShouldBe(dtoFields["PerkEntryID[14]"]);
        spriggitFields["PerkEntryID[15]"].ShouldBe(dtoFields["PerkEntryID[15]"]);
        spriggitFields["PerkEntryID[16]"].ShouldBe(dtoFields["PerkEntryID[16]"]);
        spriggitFields["PerkEntryID[2]"].ShouldBe(dtoFields["PerkEntryID[2]"]);
        spriggitFields["PerkEntryID[3]"].ShouldBe(dtoFields["PerkEntryID[3]"]);
        spriggitFields["PerkEntryID[4]"].ShouldBe(dtoFields["PerkEntryID[4]"]);
        spriggitFields["PerkEntryID[5]"].ShouldBe(dtoFields["PerkEntryID[5]"]);
        spriggitFields["PerkEntryID[6]"].ShouldBe(dtoFields["PerkEntryID[6]"]);
        spriggitFields["PerkEntryID[7]"].ShouldBe(dtoFields["PerkEntryID[7]"]);
        spriggitFields["PerkEntryID[8]"].ShouldBe(dtoFields["PerkEntryID[8]"]);
        spriggitFields["PerkEntryID[9]"].ShouldBe(dtoFields["PerkEntryID[9]"]);
        spriggitFields["Priority[0]"].ShouldBe(dtoFields["Priority[0]"]);
        spriggitFields["Priority[1]"].ShouldBe(dtoFields["Priority[1]"]);
        spriggitFields["Priority[10]"].ShouldBe(dtoFields["Priority[10]"]);
        spriggitFields["Priority[11]"].ShouldBe(dtoFields["Priority[11]"]);
        spriggitFields["Priority[12]"].ShouldBe(dtoFields["Priority[12]"]);
        spriggitFields["Priority[13]"].ShouldBe(dtoFields["Priority[13]"]);
        spriggitFields["Priority[14]"].ShouldBe(dtoFields["Priority[14]"]);
        spriggitFields["Priority[15]"].ShouldBe(dtoFields["Priority[15]"]);
        spriggitFields["Priority[2]"].ShouldBe(dtoFields["Priority[2]"]);
        spriggitFields["Priority[3]"].ShouldBe(dtoFields["Priority[3]"]);
        spriggitFields["Priority[4]"].ShouldBe(dtoFields["Priority[4]"]);
        spriggitFields["Priority[5]"].ShouldBe(dtoFields["Priority[5]"]);
        spriggitFields["Priority[6]"].ShouldBe(dtoFields["Priority[6]"]);
        spriggitFields["Priority[7]"].ShouldBe(dtoFields["Priority[7]"]);
        spriggitFields["Priority[8]"].ShouldBe(dtoFields["Priority[8]"]);
        spriggitFields["Priority[9]"].ShouldBe(dtoFields["Priority[9]"]);
        spriggitFields["Value[0]"].ShouldBe(dtoFields["Value[0]"]);
        spriggitFields["Value[1]"].ShouldBe(dtoFields["Value[1]"]);
        spriggitFields["Value[10]"].ShouldBe(dtoFields["Value[10]"]);
        spriggitFields["Value[11]"].ShouldBe(dtoFields["Value[11]"]);
        spriggitFields["Value[12]"].ShouldBe(dtoFields["Value[12]"]);
        spriggitFields["Value[13]"].ShouldBe(dtoFields["Value[13]"]);
        spriggitFields["Value[14]"].ShouldBe(dtoFields["Value[14]"]);
        spriggitFields["Value[15]"].ShouldBe(dtoFields["Value[15]"]);
        spriggitFields["Value[16]"].ShouldBe(dtoFields["Value[16]"]);
        spriggitFields["Value[2]"].ShouldBe(dtoFields["Value[2]"]);
        spriggitFields["Value[3]"].ShouldBe(dtoFields["Value[3]"]);
        spriggitFields["Value[4]"].ShouldBe(dtoFields["Value[4]"]);
        spriggitFields["Value[5]"].ShouldBe(dtoFields["Value[5]"]);
        spriggitFields["Value[6]"].ShouldBe(dtoFields["Value[6]"]);
        spriggitFields["Value[7]"].ShouldBe(dtoFields["Value[7]"]);
        spriggitFields["Value[8]"].ShouldBe(dtoFields["Value[8]"]);
        spriggitFields["Value[9]"].ShouldBe(dtoFields["Value[9]"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "PERK")]
    [Trait("FormKey", "01E67F:Fallout4.esm")]
    [Trait("EditorID", "AnimalFriend01")]
    [Trait("SpriggitFile", "Perks/AnimalFriend01 - 01E67F_Fallout4.esm.yaml")]
    public void Fallout4_PERK_ShouldMatchSpriggitSample_AnimalFriend01()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Perk,
            "AnimalFriend01");
        var dto = Helpers.GetDTO<PerkDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Perk,
            "01E67F:Fallout4.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["ButtonLabel.Count"].ShouldBe(dtoFields["ButtonLabel.Count"]);
        spriggitFields["ButtonLabel.TargetLanguage"].ShouldBe(dtoFields["ButtonLabel.TargetLanguage"]);
        spriggitFields["ButtonLabel[0].Language"].ShouldBe(dtoFields["ButtonLabel[0].Language"]);
        spriggitFields["ButtonLabel[0].String"].ShouldBe(dtoFields["ButtonLabel[0].String"]);
        spriggitFields["ButtonLabel[1].Language"].ShouldBe(dtoFields["ButtonLabel[1].Language"]);
        spriggitFields["ButtonLabel[1].String"].ShouldBe(dtoFields["ButtonLabel[1].String"]);
        spriggitFields["ButtonLabel[10].Language"].ShouldBe(dtoFields["ButtonLabel[10].Language"]);
        spriggitFields["ButtonLabel[10].String"].ShouldBe(dtoFields["ButtonLabel[10].String"]);
        spriggitFields["ButtonLabel[2].Language"].ShouldBe(dtoFields["ButtonLabel[2].Language"]);
        spriggitFields["ButtonLabel[2].String"].ShouldBe(dtoFields["ButtonLabel[2].String"]);
        spriggitFields["ButtonLabel[3].Language"].ShouldBe(dtoFields["ButtonLabel[3].Language"]);
        spriggitFields["ButtonLabel[3].String"].ShouldBe(dtoFields["ButtonLabel[3].String"]);
        spriggitFields["ButtonLabel[4].Language"].ShouldBe(dtoFields["ButtonLabel[4].Language"]);
        spriggitFields["ButtonLabel[4].String"].ShouldBe(dtoFields["ButtonLabel[4].String"]);
        spriggitFields["ButtonLabel[5].Language"].ShouldBe(dtoFields["ButtonLabel[5].Language"]);
        spriggitFields["ButtonLabel[5].String"].ShouldBe(dtoFields["ButtonLabel[5].String"]);
        spriggitFields["ButtonLabel[6].Language"].ShouldBe(dtoFields["ButtonLabel[6].Language"]);
        spriggitFields["ButtonLabel[6].String"].ShouldBe(dtoFields["ButtonLabel[6].String"]);
        spriggitFields["ButtonLabel[7].Language"].ShouldBe(dtoFields["ButtonLabel[7].Language"]);
        spriggitFields["ButtonLabel[7].String"].ShouldBe(dtoFields["ButtonLabel[7].String"]);
        spriggitFields["ButtonLabel[8].Language"].ShouldBe(dtoFields["ButtonLabel[8].Language"]);
        spriggitFields["ButtonLabel[8].String"].ShouldBe(dtoFields["ButtonLabel[8].String"]);
        spriggitFields["ButtonLabel[9].Language"].ShouldBe(dtoFields["ButtonLabel[9].Language"]);
        spriggitFields["ButtonLabel[9].String"].ShouldBe(dtoFields["ButtonLabel[9].String"]);
        spriggitFields["CompareOperator[0]"].ShouldBe(dtoFields["CompareOperator[0]"]);
        spriggitFields["CompareOperator[1]"].ShouldBe(dtoFields["CompareOperator[1]"]);
        spriggitFields["CompareOperator[2]"].ShouldBe(dtoFields["CompareOperator[2]"]);
        spriggitFields["ComparisonValue[0]"].ShouldBe(dtoFields["ComparisonValue[0]"]);
        spriggitFields["ComparisonValue[1]"].ShouldBe(dtoFields["ComparisonValue[1]"]);
        spriggitFields["ComparisonValue[2]"].ShouldBe(dtoFields["ComparisonValue[2]"]);
        spriggitFields["ComparisonValue[3]"].ShouldBe(dtoFields["ComparisonValue[3]"]);
        spriggitFields["Data.Function[0]"].ShouldBe(dtoFields["Data.Function[0]"]);
        spriggitFields["Data.Function[1]"].ShouldBe(dtoFields["Data.Function[1]"]);
        spriggitFields["Data.Function[10]"].ShouldBe(dtoFields["Data.Function[10]"]);
        spriggitFields["Data.Function[11]"].ShouldBe(dtoFields["Data.Function[11]"]);
        spriggitFields["Data.Function[2]"].ShouldBe(dtoFields["Data.Function[2]"]);
        spriggitFields["Data.Function[3]"].ShouldBe(dtoFields["Data.Function[3]"]);
        spriggitFields["Data.Function[4]"].ShouldBe(dtoFields["Data.Function[4]"]);
        spriggitFields["Data.Function[5]"].ShouldBe(dtoFields["Data.Function[5]"]);
        spriggitFields["Data.Function[6]"].ShouldBe(dtoFields["Data.Function[6]"]);
        spriggitFields["Data.Function[7]"].ShouldBe(dtoFields["Data.Function[7]"]);
        spriggitFields["Data.Function[8]"].ShouldBe(dtoFields["Data.Function[8]"]);
        spriggitFields["Data.Function[9]"].ShouldBe(dtoFields["Data.Function[9]"]);
        spriggitFields["Data.MutagenObjectType[0]"].ShouldBe(dtoFields["Data.MutagenObjectType[0]"]);
        spriggitFields["Data.MutagenObjectType[1]"].ShouldBe(dtoFields["Data.MutagenObjectType[1]"]);
        spriggitFields["Data.MutagenObjectType[10]"].ShouldBe(dtoFields["Data.MutagenObjectType[10]"]);
        spriggitFields["Data.MutagenObjectType[11]"].ShouldBe(dtoFields["Data.MutagenObjectType[11]"]);
        spriggitFields["Data.MutagenObjectType[2]"].ShouldBe(dtoFields["Data.MutagenObjectType[2]"]);
        spriggitFields["Data.MutagenObjectType[3]"].ShouldBe(dtoFields["Data.MutagenObjectType[3]"]);
        spriggitFields["Data.MutagenObjectType[4]"].ShouldBe(dtoFields["Data.MutagenObjectType[4]"]);
        spriggitFields["Data.MutagenObjectType[5]"].ShouldBe(dtoFields["Data.MutagenObjectType[5]"]);
        spriggitFields["Data.MutagenObjectType[6]"].ShouldBe(dtoFields["Data.MutagenObjectType[6]"]);
        spriggitFields["Data.MutagenObjectType[7]"].ShouldBe(dtoFields["Data.MutagenObjectType[7]"]);
        spriggitFields["Data.MutagenObjectType[8]"].ShouldBe(dtoFields["Data.MutagenObjectType[8]"]);
        spriggitFields["Data.MutagenObjectType[9]"].ShouldBe(dtoFields["Data.MutagenObjectType[9]"]);
        spriggitFields["Data.ParameterOneNumber[0]"].ShouldBe(dtoFields["Data.ParameterOneNumber[0]"]);
        spriggitFields["Data.ParameterOneNumber[1]"].ShouldBe(dtoFields["Data.ParameterOneNumber[1]"]);
        spriggitFields["Data.ParameterOneNumber[2]"].ShouldBe(dtoFields["Data.ParameterOneNumber[2]"]);
        spriggitFields["Data.ParameterOneNumber[3]"].ShouldBe(dtoFields["Data.ParameterOneNumber[3]"]);
        spriggitFields["Data.ParameterOneNumber[4]"].ShouldBe(dtoFields["Data.ParameterOneNumber[4]"]);
        spriggitFields["Data.ParameterOneNumber[5]"].ShouldBe(dtoFields["Data.ParameterOneNumber[5]"]);
        spriggitFields["Data.ParameterOneNumber[6]"].ShouldBe(dtoFields["Data.ParameterOneNumber[6]"]);
        spriggitFields["Data.ParameterOneNumber[7]"].ShouldBe(dtoFields["Data.ParameterOneNumber[7]"]);
        spriggitFields["Data.ParameterOneNumber[8]"].ShouldBe(dtoFields["Data.ParameterOneNumber[8]"]);
        spriggitFields["Data.ParameterOneRecord[0]"].ShouldBe(dtoFields["Data.ParameterOneRecord[0]"]);
        spriggitFields["Data.ParameterOneRecord[1]"].ShouldBe(dtoFields["Data.ParameterOneRecord[1]"]);
        spriggitFields["Data.ParameterOneRecord[2]"].ShouldBe(dtoFields["Data.ParameterOneRecord[2]"]);
        spriggitFields["Data.ParameterOneRecord[3]"].ShouldBe(dtoFields["Data.ParameterOneRecord[3]"]);
        spriggitFields["Data.ParameterOneRecord[4]"].ShouldBe(dtoFields["Data.ParameterOneRecord[4]"]);
        spriggitFields["Data.ParameterOneRecord[5]"].ShouldBe(dtoFields["Data.ParameterOneRecord[5]"]);
        spriggitFields["Data.ParameterOneRecord[6]"].ShouldBe(dtoFields["Data.ParameterOneRecord[6]"]);
        spriggitFields["Data.ParameterOneRecord[7]"].ShouldBe(dtoFields["Data.ParameterOneRecord[7]"]);
        spriggitFields["Data.ParameterOneRecord[8]"].ShouldBe(dtoFields["Data.ParameterOneRecord[8]"]);
        spriggitFields["Data.Reference[0]"].ShouldBe(dtoFields["Data.Reference[0]"]);
        spriggitFields["Data.Reference[1]"].ShouldBe(dtoFields["Data.Reference[1]"]);
        spriggitFields["Data.RunOnType[0]"].ShouldBe(dtoFields["Data.RunOnType[0]"]);
        spriggitFields["Data.RunOnType[1]"].ShouldBe(dtoFields["Data.RunOnType[1]"]);
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
        spriggitFields["EntryPoint"].ShouldBe(dtoFields["EntryPoint"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["Name.Count"].ShouldBe(dtoFields["Name.Count"]);
        spriggitFields["Name.TargetLanguage"].ShouldBe(dtoFields["Name.TargetLanguage"]);
        spriggitFields["Name[0].Language"].ShouldBe(dtoFields["Name[0].Language"]);
        spriggitFields["Name[0].String"].ShouldBe(dtoFields["Name[0].String"]);
        spriggitFields["Name[1].Language"].ShouldBe(dtoFields["Name[1].Language"]);
        spriggitFields["Name[1].String"].ShouldBe(dtoFields["Name[1].String"]);
        spriggitFields["Name[10].Language"].ShouldBe(dtoFields["Name[10].Language"]);
        spriggitFields["Name[10].String"].ShouldBe(dtoFields["Name[10].String"]);
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
        spriggitFields["Name[9].Language"].ShouldBe(dtoFields["Name[9].Language"]);
        spriggitFields["Name[9].String"].ShouldBe(dtoFields["Name[9].String"]);
        spriggitFields["NextPerk"].ShouldBe(dtoFields["NextPerk"]);
        spriggitFields["NumRanks"].ShouldBe(dtoFields["NumRanks"]);
        spriggitFields["PerkConditionTabCount"].ShouldBe(dtoFields["PerkConditionTabCount"]);
        spriggitFields["PerkEntryID"].ShouldBe(dtoFields["PerkEntryID"]);
        spriggitFields["Playable"].ShouldBe(dtoFields["Playable"]);
        spriggitFields["Sound"].ShouldBe(dtoFields["Sound"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["VirtualMachineAdapter.ScriptFragments.Count"].ShouldBe(dtoFields["VirtualMachineAdapter.ScriptFragments.Count"]);
        spriggitFields["VirtualMachineAdapter.ScriptFragments.ExtraBindDataVersion"].ShouldBe(dtoFields["VirtualMachineAdapter.ScriptFragments.ExtraBindDataVersion"]);
        spriggitFields["VirtualMachineAdapter.ScriptFragments.Script.Count"].ShouldBe(dtoFields["VirtualMachineAdapter.ScriptFragments.Script.Count"]);
        spriggitFields["VirtualMachineAdapter.ScriptFragments.Script.Name"].ShouldBe(dtoFields["VirtualMachineAdapter.ScriptFragments.Script.Name"]);
        spriggitFields["VirtualMachineAdapter.ScriptFragments.Script[0].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter.ScriptFragments.Script[0].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter.ScriptFragments.Script[0].Name"].ShouldBe(dtoFields["VirtualMachineAdapter.ScriptFragments.Script[0].Name"]);
        spriggitFields["VirtualMachineAdapter.ScriptFragments.Script[0].Object"].ShouldBe(dtoFields["VirtualMachineAdapter.ScriptFragments.Script[0].Object"]);
        spriggitFields["VirtualMachineAdapter.ScriptFragments.Script[1].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter.ScriptFragments.Script[1].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter.ScriptFragments.Script[1].Name"].ShouldBe(dtoFields["VirtualMachineAdapter.ScriptFragments.Script[1].Name"]);
        spriggitFields["VirtualMachineAdapter.ScriptFragments.Script[1].Object"].ShouldBe(dtoFields["VirtualMachineAdapter.ScriptFragments.Script[1].Object"]);
        spriggitFields["VirtualMachineAdapter.ScriptFragments.Script[2].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter.ScriptFragments.Script[2].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter.ScriptFragments.Script[2].Name"].ShouldBe(dtoFields["VirtualMachineAdapter.ScriptFragments.Script[2].Name"]);
        spriggitFields["VirtualMachineAdapter.ScriptFragments.Script[2].Object"].ShouldBe(dtoFields["VirtualMachineAdapter.ScriptFragments.Script[2].Object"]);
        spriggitFields["VirtualMachineAdapter.ScriptFragments.Script[3].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter.ScriptFragments.Script[3].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter.ScriptFragments.Script[3].Name"].ShouldBe(dtoFields["VirtualMachineAdapter.ScriptFragments.Script[3].Name"]);
        spriggitFields["VirtualMachineAdapter.ScriptFragments.Script[3].Object"].ShouldBe(dtoFields["VirtualMachineAdapter.ScriptFragments.Script[3].Object"]);
        spriggitFields["VirtualMachineAdapter.ScriptFragments[0].FragmentName"].ShouldBe(dtoFields["VirtualMachineAdapter.ScriptFragments[0].FragmentName"]);
        spriggitFields["VirtualMachineAdapter.ScriptFragments[0].ScriptName"].ShouldBe(dtoFields["VirtualMachineAdapter.ScriptFragments[0].ScriptName"]);
        spriggitFields["VirtualMachineAdapter.ScriptFragments[0].Unknown2"].ShouldBe(dtoFields["VirtualMachineAdapter.ScriptFragments[0].Unknown2"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "PERK")]
    [Trait("FormKey", "04A0D9:Fallout4.esm")]
    [Trait("EditorID", "AnimalFriend02")]
    [Trait("SpriggitFile", "Perks/AnimalFriend02 - 04A0D9_Fallout4.esm.yaml")]
    public void Fallout4_PERK_ShouldMatchSpriggitSample_AnimalFriend02()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Perk,
            "AnimalFriend02");
        var dto = Helpers.GetDTO<PerkDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Perk,
            "04A0D9:Fallout4.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["ButtonLabel.Count"].ShouldBe(dtoFields["ButtonLabel.Count"]);
        spriggitFields["ButtonLabel.TargetLanguage"].ShouldBe(dtoFields["ButtonLabel.TargetLanguage"]);
        spriggitFields["ButtonLabel[0].Language"].ShouldBe(dtoFields["ButtonLabel[0].Language"]);
        spriggitFields["ButtonLabel[0].String"].ShouldBe(dtoFields["ButtonLabel[0].String"]);
        spriggitFields["ButtonLabel[1].Language"].ShouldBe(dtoFields["ButtonLabel[1].Language"]);
        spriggitFields["ButtonLabel[1].String"].ShouldBe(dtoFields["ButtonLabel[1].String"]);
        spriggitFields["ButtonLabel[10].Language"].ShouldBe(dtoFields["ButtonLabel[10].Language"]);
        spriggitFields["ButtonLabel[10].String"].ShouldBe(dtoFields["ButtonLabel[10].String"]);
        spriggitFields["ButtonLabel[2].Language"].ShouldBe(dtoFields["ButtonLabel[2].Language"]);
        spriggitFields["ButtonLabel[2].String"].ShouldBe(dtoFields["ButtonLabel[2].String"]);
        spriggitFields["ButtonLabel[3].Language"].ShouldBe(dtoFields["ButtonLabel[3].Language"]);
        spriggitFields["ButtonLabel[3].String"].ShouldBe(dtoFields["ButtonLabel[3].String"]);
        spriggitFields["ButtonLabel[4].Language"].ShouldBe(dtoFields["ButtonLabel[4].Language"]);
        spriggitFields["ButtonLabel[4].String"].ShouldBe(dtoFields["ButtonLabel[4].String"]);
        spriggitFields["ButtonLabel[5].Language"].ShouldBe(dtoFields["ButtonLabel[5].Language"]);
        spriggitFields["ButtonLabel[5].String"].ShouldBe(dtoFields["ButtonLabel[5].String"]);
        spriggitFields["ButtonLabel[6].Language"].ShouldBe(dtoFields["ButtonLabel[6].Language"]);
        spriggitFields["ButtonLabel[6].String"].ShouldBe(dtoFields["ButtonLabel[6].String"]);
        spriggitFields["ButtonLabel[7].Language"].ShouldBe(dtoFields["ButtonLabel[7].Language"]);
        spriggitFields["ButtonLabel[7].String"].ShouldBe(dtoFields["ButtonLabel[7].String"]);
        spriggitFields["ButtonLabel[8].Language"].ShouldBe(dtoFields["ButtonLabel[8].Language"]);
        spriggitFields["ButtonLabel[8].String"].ShouldBe(dtoFields["ButtonLabel[8].String"]);
        spriggitFields["ButtonLabel[9].Language"].ShouldBe(dtoFields["ButtonLabel[9].Language"]);
        spriggitFields["ButtonLabel[9].String"].ShouldBe(dtoFields["ButtonLabel[9].String"]);
        spriggitFields["CompareOperator[0]"].ShouldBe(dtoFields["CompareOperator[0]"]);
        spriggitFields["CompareOperator[1]"].ShouldBe(dtoFields["CompareOperator[1]"]);
        spriggitFields["CompareOperator[2]"].ShouldBe(dtoFields["CompareOperator[2]"]);
        spriggitFields["ComparisonValue[0]"].ShouldBe(dtoFields["ComparisonValue[0]"]);
        spriggitFields["ComparisonValue[1]"].ShouldBe(dtoFields["ComparisonValue[1]"]);
        spriggitFields["ComparisonValue[2]"].ShouldBe(dtoFields["ComparisonValue[2]"]);
        spriggitFields["ComparisonValue[3]"].ShouldBe(dtoFields["ComparisonValue[3]"]);
        spriggitFields["ComparisonValue[4]"].ShouldBe(dtoFields["ComparisonValue[4]"]);
        spriggitFields["ComparisonValue[5]"].ShouldBe(dtoFields["ComparisonValue[5]"]);
        spriggitFields["Data.Function[0]"].ShouldBe(dtoFields["Data.Function[0]"]);
        spriggitFields["Data.Function[1]"].ShouldBe(dtoFields["Data.Function[1]"]);
        spriggitFields["Data.Function[2]"].ShouldBe(dtoFields["Data.Function[2]"]);
        spriggitFields["Data.Function[3]"].ShouldBe(dtoFields["Data.Function[3]"]);
        spriggitFields["Data.Function[4]"].ShouldBe(dtoFields["Data.Function[4]"]);
        spriggitFields["Data.Function[5]"].ShouldBe(dtoFields["Data.Function[5]"]);
        spriggitFields["Data.Function[6]"].ShouldBe(dtoFields["Data.Function[6]"]);
        spriggitFields["Data.Function[7]"].ShouldBe(dtoFields["Data.Function[7]"]);
        spriggitFields["Data.Function[8]"].ShouldBe(dtoFields["Data.Function[8]"]);
        spriggitFields["Data.Function[9]"].ShouldBe(dtoFields["Data.Function[9]"]);
        spriggitFields["Data.MutagenObjectType[0]"].ShouldBe(dtoFields["Data.MutagenObjectType[0]"]);
        spriggitFields["Data.MutagenObjectType[1]"].ShouldBe(dtoFields["Data.MutagenObjectType[1]"]);
        spriggitFields["Data.MutagenObjectType[2]"].ShouldBe(dtoFields["Data.MutagenObjectType[2]"]);
        spriggitFields["Data.MutagenObjectType[3]"].ShouldBe(dtoFields["Data.MutagenObjectType[3]"]);
        spriggitFields["Data.MutagenObjectType[4]"].ShouldBe(dtoFields["Data.MutagenObjectType[4]"]);
        spriggitFields["Data.MutagenObjectType[5]"].ShouldBe(dtoFields["Data.MutagenObjectType[5]"]);
        spriggitFields["Data.MutagenObjectType[6]"].ShouldBe(dtoFields["Data.MutagenObjectType[6]"]);
        spriggitFields["Data.MutagenObjectType[7]"].ShouldBe(dtoFields["Data.MutagenObjectType[7]"]);
        spriggitFields["Data.MutagenObjectType[8]"].ShouldBe(dtoFields["Data.MutagenObjectType[8]"]);
        spriggitFields["Data.MutagenObjectType[9]"].ShouldBe(dtoFields["Data.MutagenObjectType[9]"]);
        spriggitFields["Data.ParameterOneNumber[0]"].ShouldBe(dtoFields["Data.ParameterOneNumber[0]"]);
        spriggitFields["Data.ParameterOneNumber[1]"].ShouldBe(dtoFields["Data.ParameterOneNumber[1]"]);
        spriggitFields["Data.ParameterOneNumber[2]"].ShouldBe(dtoFields["Data.ParameterOneNumber[2]"]);
        spriggitFields["Data.ParameterOneNumber[3]"].ShouldBe(dtoFields["Data.ParameterOneNumber[3]"]);
        spriggitFields["Data.ParameterOneNumber[4]"].ShouldBe(dtoFields["Data.ParameterOneNumber[4]"]);
        spriggitFields["Data.ParameterOneNumber[5]"].ShouldBe(dtoFields["Data.ParameterOneNumber[5]"]);
        spriggitFields["Data.ParameterOneNumber[6]"].ShouldBe(dtoFields["Data.ParameterOneNumber[6]"]);
        spriggitFields["Data.ParameterOneRecord[0]"].ShouldBe(dtoFields["Data.ParameterOneRecord[0]"]);
        spriggitFields["Data.ParameterOneRecord[1]"].ShouldBe(dtoFields["Data.ParameterOneRecord[1]"]);
        spriggitFields["Data.ParameterOneRecord[2]"].ShouldBe(dtoFields["Data.ParameterOneRecord[2]"]);
        spriggitFields["Data.ParameterOneRecord[3]"].ShouldBe(dtoFields["Data.ParameterOneRecord[3]"]);
        spriggitFields["Data.ParameterOneRecord[4]"].ShouldBe(dtoFields["Data.ParameterOneRecord[4]"]);
        spriggitFields["Data.ParameterOneRecord[5]"].ShouldBe(dtoFields["Data.ParameterOneRecord[5]"]);
        spriggitFields["Data.ParameterOneRecord[6]"].ShouldBe(dtoFields["Data.ParameterOneRecord[6]"]);
        spriggitFields["Data.Reference[0]"].ShouldBe(dtoFields["Data.Reference[0]"]);
        spriggitFields["Data.Reference[1]"].ShouldBe(dtoFields["Data.Reference[1]"]);
        spriggitFields["Data.Reference[2]"].ShouldBe(dtoFields["Data.Reference[2]"]);
        spriggitFields["Data.RunOnType[0]"].ShouldBe(dtoFields["Data.RunOnType[0]"]);
        spriggitFields["Data.RunOnType[1]"].ShouldBe(dtoFields["Data.RunOnType[1]"]);
        spriggitFields["Data.RunOnType[2]"].ShouldBe(dtoFields["Data.RunOnType[2]"]);
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
        spriggitFields["EntryPoint"].ShouldBe(dtoFields["EntryPoint"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["Level"].ShouldBe(dtoFields["Level"]);
        spriggitFields["Name.Count"].ShouldBe(dtoFields["Name.Count"]);
        spriggitFields["Name.TargetLanguage"].ShouldBe(dtoFields["Name.TargetLanguage"]);
        spriggitFields["Name[0].Language"].ShouldBe(dtoFields["Name[0].Language"]);
        spriggitFields["Name[0].String"].ShouldBe(dtoFields["Name[0].String"]);
        spriggitFields["Name[1].Language"].ShouldBe(dtoFields["Name[1].Language"]);
        spriggitFields["Name[1].String"].ShouldBe(dtoFields["Name[1].String"]);
        spriggitFields["Name[10].Language"].ShouldBe(dtoFields["Name[10].Language"]);
        spriggitFields["Name[10].String"].ShouldBe(dtoFields["Name[10].String"]);
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
        spriggitFields["Name[9].Language"].ShouldBe(dtoFields["Name[9].Language"]);
        spriggitFields["Name[9].String"].ShouldBe(dtoFields["Name[9].String"]);
        spriggitFields["NextPerk"].ShouldBe(dtoFields["NextPerk"]);
        spriggitFields["NumRanks"].ShouldBe(dtoFields["NumRanks"]);
        spriggitFields["PerkConditionTabCount"].ShouldBe(dtoFields["PerkConditionTabCount"]);
        spriggitFields["PerkEntryID"].ShouldBe(dtoFields["PerkEntryID"]);
        spriggitFields["Playable"].ShouldBe(dtoFields["Playable"]);
        spriggitFields["Sound"].ShouldBe(dtoFields["Sound"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["VirtualMachineAdapter.ScriptFragments.Count"].ShouldBe(dtoFields["VirtualMachineAdapter.ScriptFragments.Count"]);
        spriggitFields["VirtualMachineAdapter.ScriptFragments.ExtraBindDataVersion"].ShouldBe(dtoFields["VirtualMachineAdapter.ScriptFragments.ExtraBindDataVersion"]);
        spriggitFields["VirtualMachineAdapter.ScriptFragments.Script.Count"].ShouldBe(dtoFields["VirtualMachineAdapter.ScriptFragments.Script.Count"]);
        spriggitFields["VirtualMachineAdapter.ScriptFragments.Script.Name"].ShouldBe(dtoFields["VirtualMachineAdapter.ScriptFragments.Script.Name"]);
        spriggitFields["VirtualMachineAdapter.ScriptFragments.Script[0].MutagenObjectType"].ShouldBe(dtoFields["VirtualMachineAdapter.ScriptFragments.Script[0].MutagenObjectType"]);
        spriggitFields["VirtualMachineAdapter.ScriptFragments.Script[0].Name"].ShouldBe(dtoFields["VirtualMachineAdapter.ScriptFragments.Script[0].Name"]);
        spriggitFields["VirtualMachineAdapter.ScriptFragments.Script[0].Object"].ShouldBe(dtoFields["VirtualMachineAdapter.ScriptFragments.Script[0].Object"]);
        spriggitFields["VirtualMachineAdapter.ScriptFragments[0].FragmentName"].ShouldBe(dtoFields["VirtualMachineAdapter.ScriptFragments[0].FragmentName"]);
        spriggitFields["VirtualMachineAdapter.ScriptFragments[0].ScriptName"].ShouldBe(dtoFields["VirtualMachineAdapter.ScriptFragments[0].ScriptName"]);
        spriggitFields["VirtualMachineAdapter.ScriptFragments[0].Unknown2"].ShouldBe(dtoFields["VirtualMachineAdapter.ScriptFragments[0].Unknown2"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "PERK")]
    [Trait("FormKey", "0D979D:Fallout4.esm")]
    [Trait("EditorID", "TrainingAG01")]
    [Trait("SpriggitFile", "Perks/TrainingAG01 - 0D979D_Fallout4.esm.yaml")]
    public void Fallout4_PERK_ShouldMatchSpriggitSample_TrainingAG01()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Perk,
            "TrainingAG01");
        var dto = Helpers.GetDTO<PerkDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Perk,
            "0D979D:Fallout4.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["CompareOperator"].ShouldBe(dtoFields["CompareOperator"]);
        spriggitFields["ComparisonValue"].ShouldBe(dtoFields["ComparisonValue"]);
        spriggitFields["Data.Function"].ShouldBe(dtoFields["Data.Function"]);
        spriggitFields["Data.MutagenObjectType"].ShouldBe(dtoFields["Data.MutagenObjectType"]);
        spriggitFields["Data.ParameterOneNumber"].ShouldBe(dtoFields["Data.ParameterOneNumber"]);
        spriggitFields["Data.ParameterOneRecord"].ShouldBe(dtoFields["Data.ParameterOneRecord"]);
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
        spriggitFields["Name.Count"].ShouldBe(dtoFields["Name.Count"]);
        spriggitFields["Name.TargetLanguage"].ShouldBe(dtoFields["Name.TargetLanguage"]);
        spriggitFields["Name[0].Language"].ShouldBe(dtoFields["Name[0].Language"]);
        spriggitFields["Name[0].String"].ShouldBe(dtoFields["Name[0].String"]);
        spriggitFields["Name[1].Language"].ShouldBe(dtoFields["Name[1].Language"]);
        spriggitFields["Name[1].String"].ShouldBe(dtoFields["Name[1].String"]);
        spriggitFields["Name[10].Language"].ShouldBe(dtoFields["Name[10].Language"]);
        spriggitFields["Name[10].String"].ShouldBe(dtoFields["Name[10].String"]);
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
        spriggitFields["Name[9].Language"].ShouldBe(dtoFields["Name[9].Language"]);
        spriggitFields["Name[9].String"].ShouldBe(dtoFields["Name[9].String"]);
        spriggitFields["NextPerk"].ShouldBe(dtoFields["NextPerk"]);
        spriggitFields["NumRanks"].ShouldBe(dtoFields["NumRanks"]);
        spriggitFields["Playable"].ShouldBe(dtoFields["Playable"]);
        spriggitFields["Quest"].ShouldBe(dtoFields["Quest"]);
        spriggitFields["Sound"].ShouldBe(dtoFields["Sound"]);
        spriggitFields["Stage"].ShouldBe(dtoFields["Stage"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "PERK")]
    [Trait("FormKey", "065DFA:Fallout4.esm")]
    [Trait("EditorID", "Basher02")]
    [Trait("SpriggitFile", "Perks/Basher02 - 065DFA_Fallout4.esm.yaml")]
    public void Fallout4_PERK_ShouldMatchSpriggitSample_Basher02()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Perk,
            "Basher02");
        var dto = Helpers.GetDTO<PerkDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Perk,
            "065DFA:Fallout4.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["CompareOperator[0]"].ShouldBe(dtoFields["CompareOperator[0]"]);
        spriggitFields["CompareOperator[1]"].ShouldBe(dtoFields["CompareOperator[1]"]);
        spriggitFields["ComparisonValue[0]"].ShouldBe(dtoFields["ComparisonValue[0]"]);
        spriggitFields["ComparisonValue[1]"].ShouldBe(dtoFields["ComparisonValue[1]"]);
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
        spriggitFields["EntryPoint[0]"].ShouldBe(dtoFields["EntryPoint[0]"]);
        spriggitFields["EntryPoint[1]"].ShouldBe(dtoFields["EntryPoint[1]"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["Level"].ShouldBe(dtoFields["Level"]);
        spriggitFields["Modification[0]"].ShouldBe(dtoFields["Modification[0]"]);
        spriggitFields["Modification[1]"].ShouldBe(dtoFields["Modification[1]"]);
        spriggitFields["Name.Count"].ShouldBe(dtoFields["Name.Count"]);
        spriggitFields["Name.TargetLanguage"].ShouldBe(dtoFields["Name.TargetLanguage"]);
        spriggitFields["Name[0].Language"].ShouldBe(dtoFields["Name[0].Language"]);
        spriggitFields["Name[0].String"].ShouldBe(dtoFields["Name[0].String"]);
        spriggitFields["Name[1].Language"].ShouldBe(dtoFields["Name[1].Language"]);
        spriggitFields["Name[1].String"].ShouldBe(dtoFields["Name[1].String"]);
        spriggitFields["Name[10].Language"].ShouldBe(dtoFields["Name[10].Language"]);
        spriggitFields["Name[10].String"].ShouldBe(dtoFields["Name[10].String"]);
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
        spriggitFields["Name[9].Language"].ShouldBe(dtoFields["Name[9].Language"]);
        spriggitFields["Name[9].String"].ShouldBe(dtoFields["Name[9].String"]);
        spriggitFields["NextPerk"].ShouldBe(dtoFields["NextPerk"]);
        spriggitFields["NumRanks"].ShouldBe(dtoFields["NumRanks"]);
        spriggitFields["PerkConditionTabCount[0]"].ShouldBe(dtoFields["PerkConditionTabCount[0]"]);
        spriggitFields["PerkConditionTabCount[1]"].ShouldBe(dtoFields["PerkConditionTabCount[1]"]);
        spriggitFields["PerkEntryID[0]"].ShouldBe(dtoFields["PerkEntryID[0]"]);
        spriggitFields["PerkEntryID[1]"].ShouldBe(dtoFields["PerkEntryID[1]"]);
        spriggitFields["Playable"].ShouldBe(dtoFields["Playable"]);
        spriggitFields["Sound"].ShouldBe(dtoFields["Sound"]);
        spriggitFields["Value[0]"].ShouldBe(dtoFields["Value[0]"]);
        spriggitFields["Value[1]"].ShouldBe(dtoFields["Value[1]"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }
}
