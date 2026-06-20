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

        Helpers.GetSpriggitField(spriggit, "ActorValue[0]").ShouldBe(Helpers.GetDTOField(dto, "ActorValue[0]"));
        Helpers.GetSpriggitField(spriggit, "ActorValue[1]").ShouldBe(Helpers.GetDTOField(dto, "ActorValue[1]"));
        Helpers.GetSpriggitField(spriggit, "ActorValue[10]").ShouldBe(Helpers.GetDTOField(dto, "ActorValue[10]"));
        Helpers.GetSpriggitField(spriggit, "ActorValue[11]").ShouldBe(Helpers.GetDTOField(dto, "ActorValue[11]"));
        Helpers.GetSpriggitField(spriggit, "ActorValue[12]").ShouldBe(Helpers.GetDTOField(dto, "ActorValue[12]"));
        Helpers.GetSpriggitField(spriggit, "ActorValue[13]").ShouldBe(Helpers.GetDTOField(dto, "ActorValue[13]"));
        Helpers.GetSpriggitField(spriggit, "ActorValue[2]").ShouldBe(Helpers.GetDTOField(dto, "ActorValue[2]"));
        Helpers.GetSpriggitField(spriggit, "ActorValue[3]").ShouldBe(Helpers.GetDTOField(dto, "ActorValue[3]"));
        Helpers.GetSpriggitField(spriggit, "ActorValue[4]").ShouldBe(Helpers.GetDTOField(dto, "ActorValue[4]"));
        Helpers.GetSpriggitField(spriggit, "ActorValue[5]").ShouldBe(Helpers.GetDTOField(dto, "ActorValue[5]"));
        Helpers.GetSpriggitField(spriggit, "ActorValue[6]").ShouldBe(Helpers.GetDTOField(dto, "ActorValue[6]"));
        Helpers.GetSpriggitField(spriggit, "ActorValue[7]").ShouldBe(Helpers.GetDTOField(dto, "ActorValue[7]"));
        Helpers.GetSpriggitField(spriggit, "ActorValue[8]").ShouldBe(Helpers.GetDTOField(dto, "ActorValue[8]"));
        Helpers.GetSpriggitField(spriggit, "ActorValue[9]").ShouldBe(Helpers.GetDTOField(dto, "ActorValue[9]"));
        Helpers.GetSpriggitField(spriggit, "CompareOperator").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator"));
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
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[3]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[3]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[4]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[4]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[5]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[5]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[6]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[6]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[7]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[7]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[8]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[8]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[9]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[9]"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[1]"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[10]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[10]"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[11]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[11]"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[12]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[12]"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[13]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[13]"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[14]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[14]"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[15]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[15]"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[16]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[16]"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[17]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[17]"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[18]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[18]"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[19]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[19]"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[2]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[2]"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[20]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[20]"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[21]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[21]"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[22]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[22]"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[3]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[3]"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[4]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[4]"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[5]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[5]"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[6]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[6]"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[7]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[7]"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[8]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[8]"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[9]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[9]"));
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
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[3]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[3]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[4]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[4]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[5]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[5]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[6]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[6]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[7]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[7]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[8]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[8]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[9]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[9]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneNumber[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneNumber[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneNumber[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneNumber[1]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneNumber[10]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneNumber[10]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneNumber[11]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneNumber[11]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneNumber[12]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneNumber[12]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneNumber[13]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneNumber[13]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneNumber[14]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneNumber[14]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneNumber[15]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneNumber[15]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneNumber[16]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneNumber[16]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneNumber[17]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneNumber[17]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneNumber[18]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneNumber[18]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneNumber[19]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneNumber[19]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneNumber[2]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneNumber[2]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneNumber[20]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneNumber[20]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneNumber[21]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneNumber[21]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneNumber[3]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneNumber[3]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneNumber[4]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneNumber[4]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneNumber[5]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneNumber[5]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneNumber[6]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneNumber[6]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneNumber[7]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneNumber[7]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneNumber[8]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneNumber[8]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneNumber[9]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneNumber[9]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneRecord[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneRecord[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneRecord[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneRecord[1]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneRecord[10]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneRecord[10]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneRecord[11]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneRecord[11]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneRecord[12]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneRecord[12]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneRecord[13]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneRecord[13]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneRecord[14]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneRecord[14]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneRecord[15]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneRecord[15]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneRecord[16]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneRecord[16]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneRecord[17]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneRecord[17]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneRecord[18]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneRecord[18]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneRecord[19]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneRecord[19]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneRecord[2]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneRecord[2]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneRecord[20]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneRecord[20]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneRecord[21]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneRecord[21]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneRecord[3]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneRecord[3]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneRecord[4]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneRecord[4]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneRecord[5]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneRecord[5]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneRecord[6]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneRecord[6]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneRecord[7]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneRecord[7]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneRecord[8]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneRecord[8]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneRecord[9]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneRecord[9]"));
        Helpers.GetSpriggitField(spriggit, "Description.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Description.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "EntryPoint[0]").ShouldBe(Helpers.GetDTOField(dto, "EntryPoint[0]"));
        Helpers.GetSpriggitField(spriggit, "EntryPoint[1]").ShouldBe(Helpers.GetDTOField(dto, "EntryPoint[1]"));
        Helpers.GetSpriggitField(spriggit, "EntryPoint[10]").ShouldBe(Helpers.GetDTOField(dto, "EntryPoint[10]"));
        Helpers.GetSpriggitField(spriggit, "EntryPoint[11]").ShouldBe(Helpers.GetDTOField(dto, "EntryPoint[11]"));
        Helpers.GetSpriggitField(spriggit, "EntryPoint[12]").ShouldBe(Helpers.GetDTOField(dto, "EntryPoint[12]"));
        Helpers.GetSpriggitField(spriggit, "EntryPoint[13]").ShouldBe(Helpers.GetDTOField(dto, "EntryPoint[13]"));
        Helpers.GetSpriggitField(spriggit, "EntryPoint[14]").ShouldBe(Helpers.GetDTOField(dto, "EntryPoint[14]"));
        Helpers.GetSpriggitField(spriggit, "EntryPoint[15]").ShouldBe(Helpers.GetDTOField(dto, "EntryPoint[15]"));
        Helpers.GetSpriggitField(spriggit, "EntryPoint[16]").ShouldBe(Helpers.GetDTOField(dto, "EntryPoint[16]"));
        Helpers.GetSpriggitField(spriggit, "EntryPoint[2]").ShouldBe(Helpers.GetDTOField(dto, "EntryPoint[2]"));
        Helpers.GetSpriggitField(spriggit, "EntryPoint[3]").ShouldBe(Helpers.GetDTOField(dto, "EntryPoint[3]"));
        Helpers.GetSpriggitField(spriggit, "EntryPoint[4]").ShouldBe(Helpers.GetDTOField(dto, "EntryPoint[4]"));
        Helpers.GetSpriggitField(spriggit, "EntryPoint[5]").ShouldBe(Helpers.GetDTOField(dto, "EntryPoint[5]"));
        Helpers.GetSpriggitField(spriggit, "EntryPoint[6]").ShouldBe(Helpers.GetDTOField(dto, "EntryPoint[6]"));
        Helpers.GetSpriggitField(spriggit, "EntryPoint[7]").ShouldBe(Helpers.GetDTOField(dto, "EntryPoint[7]"));
        Helpers.GetSpriggitField(spriggit, "EntryPoint[8]").ShouldBe(Helpers.GetDTOField(dto, "EntryPoint[8]"));
        Helpers.GetSpriggitField(spriggit, "EntryPoint[9]").ShouldBe(Helpers.GetDTOField(dto, "EntryPoint[9]"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "Hidden").ShouldBe(Helpers.GetDTOField(dto, "Hidden"));
        Helpers.GetSpriggitField(spriggit, "MajorRecordFlagsRaw").ShouldBe(Helpers.GetDTOField(dto, "MajorRecordFlags"));
        Helpers.GetSpriggitField(spriggit, "Modification[0]").ShouldBe(Helpers.GetDTOField(dto, "Modification[0]"));
        Helpers.GetSpriggitField(spriggit, "Modification[1]").ShouldBe(Helpers.GetDTOField(dto, "Modification[1]"));
        Helpers.GetSpriggitField(spriggit, "Name.Count").ShouldBe(Helpers.GetDTOField(dto, "Name.Count"));
        Helpers.GetSpriggitField(spriggit, "Name.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Name.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Name[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[0].String").ShouldBe(Helpers.GetDTOField(dto, "Name[0].String"));
        Helpers.GetSpriggitField(spriggit, "Name[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[1].String").ShouldBe(Helpers.GetDTOField(dto, "Name[1].String"));
        Helpers.GetSpriggitField(spriggit, "Name[10].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[10].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[10].String").ShouldBe(Helpers.GetDTOField(dto, "Name[10].String"));
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
        Helpers.GetSpriggitField(spriggit, "Name[9].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[9].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[9].String").ShouldBe(Helpers.GetDTOField(dto, "Name[9].String"));
        Helpers.GetSpriggitField(spriggit, "NumRanks").ShouldBe(Helpers.GetDTOField(dto, "NumRanks"));
        Helpers.GetSpriggitField(spriggit, "PerkConditionTabCount[0]").ShouldBe(Helpers.GetDTOField(dto, "PerkConditionTabCount[0]"));
        Helpers.GetSpriggitField(spriggit, "PerkConditionTabCount[1]").ShouldBe(Helpers.GetDTOField(dto, "PerkConditionTabCount[1]"));
        Helpers.GetSpriggitField(spriggit, "PerkConditionTabCount[10]").ShouldBe(Helpers.GetDTOField(dto, "PerkConditionTabCount[10]"));
        Helpers.GetSpriggitField(spriggit, "PerkConditionTabCount[11]").ShouldBe(Helpers.GetDTOField(dto, "PerkConditionTabCount[11]"));
        Helpers.GetSpriggitField(spriggit, "PerkConditionTabCount[12]").ShouldBe(Helpers.GetDTOField(dto, "PerkConditionTabCount[12]"));
        Helpers.GetSpriggitField(spriggit, "PerkConditionTabCount[13]").ShouldBe(Helpers.GetDTOField(dto, "PerkConditionTabCount[13]"));
        Helpers.GetSpriggitField(spriggit, "PerkConditionTabCount[14]").ShouldBe(Helpers.GetDTOField(dto, "PerkConditionTabCount[14]"));
        Helpers.GetSpriggitField(spriggit, "PerkConditionTabCount[15]").ShouldBe(Helpers.GetDTOField(dto, "PerkConditionTabCount[15]"));
        Helpers.GetSpriggitField(spriggit, "PerkConditionTabCount[16]").ShouldBe(Helpers.GetDTOField(dto, "PerkConditionTabCount[16]"));
        Helpers.GetSpriggitField(spriggit, "PerkConditionTabCount[2]").ShouldBe(Helpers.GetDTOField(dto, "PerkConditionTabCount[2]"));
        Helpers.GetSpriggitField(spriggit, "PerkConditionTabCount[3]").ShouldBe(Helpers.GetDTOField(dto, "PerkConditionTabCount[3]"));
        Helpers.GetSpriggitField(spriggit, "PerkConditionTabCount[4]").ShouldBe(Helpers.GetDTOField(dto, "PerkConditionTabCount[4]"));
        Helpers.GetSpriggitField(spriggit, "PerkConditionTabCount[5]").ShouldBe(Helpers.GetDTOField(dto, "PerkConditionTabCount[5]"));
        Helpers.GetSpriggitField(spriggit, "PerkConditionTabCount[6]").ShouldBe(Helpers.GetDTOField(dto, "PerkConditionTabCount[6]"));
        Helpers.GetSpriggitField(spriggit, "PerkConditionTabCount[7]").ShouldBe(Helpers.GetDTOField(dto, "PerkConditionTabCount[7]"));
        Helpers.GetSpriggitField(spriggit, "PerkConditionTabCount[8]").ShouldBe(Helpers.GetDTOField(dto, "PerkConditionTabCount[8]"));
        Helpers.GetSpriggitField(spriggit, "PerkConditionTabCount[9]").ShouldBe(Helpers.GetDTOField(dto, "PerkConditionTabCount[9]"));
        Helpers.GetSpriggitField(spriggit, "PerkEntryID[0]").ShouldBe(Helpers.GetDTOField(dto, "PerkEntryID[0]"));
        Helpers.GetSpriggitField(spriggit, "PerkEntryID[1]").ShouldBe(Helpers.GetDTOField(dto, "PerkEntryID[1]"));
        Helpers.GetSpriggitField(spriggit, "PerkEntryID[10]").ShouldBe(Helpers.GetDTOField(dto, "PerkEntryID[10]"));
        Helpers.GetSpriggitField(spriggit, "PerkEntryID[11]").ShouldBe(Helpers.GetDTOField(dto, "PerkEntryID[11]"));
        Helpers.GetSpriggitField(spriggit, "PerkEntryID[12]").ShouldBe(Helpers.GetDTOField(dto, "PerkEntryID[12]"));
        Helpers.GetSpriggitField(spriggit, "PerkEntryID[13]").ShouldBe(Helpers.GetDTOField(dto, "PerkEntryID[13]"));
        Helpers.GetSpriggitField(spriggit, "PerkEntryID[14]").ShouldBe(Helpers.GetDTOField(dto, "PerkEntryID[14]"));
        Helpers.GetSpriggitField(spriggit, "PerkEntryID[15]").ShouldBe(Helpers.GetDTOField(dto, "PerkEntryID[15]"));
        Helpers.GetSpriggitField(spriggit, "PerkEntryID[16]").ShouldBe(Helpers.GetDTOField(dto, "PerkEntryID[16]"));
        Helpers.GetSpriggitField(spriggit, "PerkEntryID[2]").ShouldBe(Helpers.GetDTOField(dto, "PerkEntryID[2]"));
        Helpers.GetSpriggitField(spriggit, "PerkEntryID[3]").ShouldBe(Helpers.GetDTOField(dto, "PerkEntryID[3]"));
        Helpers.GetSpriggitField(spriggit, "PerkEntryID[4]").ShouldBe(Helpers.GetDTOField(dto, "PerkEntryID[4]"));
        Helpers.GetSpriggitField(spriggit, "PerkEntryID[5]").ShouldBe(Helpers.GetDTOField(dto, "PerkEntryID[5]"));
        Helpers.GetSpriggitField(spriggit, "PerkEntryID[6]").ShouldBe(Helpers.GetDTOField(dto, "PerkEntryID[6]"));
        Helpers.GetSpriggitField(spriggit, "PerkEntryID[7]").ShouldBe(Helpers.GetDTOField(dto, "PerkEntryID[7]"));
        Helpers.GetSpriggitField(spriggit, "PerkEntryID[8]").ShouldBe(Helpers.GetDTOField(dto, "PerkEntryID[8]"));
        Helpers.GetSpriggitField(spriggit, "PerkEntryID[9]").ShouldBe(Helpers.GetDTOField(dto, "PerkEntryID[9]"));
        Helpers.GetSpriggitField(spriggit, "Priority[0]").ShouldBe(Helpers.GetDTOField(dto, "Priority[0]"));
        Helpers.GetSpriggitField(spriggit, "Priority[1]").ShouldBe(Helpers.GetDTOField(dto, "Priority[1]"));
        Helpers.GetSpriggitField(spriggit, "Priority[10]").ShouldBe(Helpers.GetDTOField(dto, "Priority[10]"));
        Helpers.GetSpriggitField(spriggit, "Priority[11]").ShouldBe(Helpers.GetDTOField(dto, "Priority[11]"));
        Helpers.GetSpriggitField(spriggit, "Priority[12]").ShouldBe(Helpers.GetDTOField(dto, "Priority[12]"));
        Helpers.GetSpriggitField(spriggit, "Priority[13]").ShouldBe(Helpers.GetDTOField(dto, "Priority[13]"));
        Helpers.GetSpriggitField(spriggit, "Priority[14]").ShouldBe(Helpers.GetDTOField(dto, "Priority[14]"));
        Helpers.GetSpriggitField(spriggit, "Priority[15]").ShouldBe(Helpers.GetDTOField(dto, "Priority[15]"));
        Helpers.GetSpriggitField(spriggit, "Priority[2]").ShouldBe(Helpers.GetDTOField(dto, "Priority[2]"));
        Helpers.GetSpriggitField(spriggit, "Priority[3]").ShouldBe(Helpers.GetDTOField(dto, "Priority[3]"));
        Helpers.GetSpriggitField(spriggit, "Priority[4]").ShouldBe(Helpers.GetDTOField(dto, "Priority[4]"));
        Helpers.GetSpriggitField(spriggit, "Priority[5]").ShouldBe(Helpers.GetDTOField(dto, "Priority[5]"));
        Helpers.GetSpriggitField(spriggit, "Priority[6]").ShouldBe(Helpers.GetDTOField(dto, "Priority[6]"));
        Helpers.GetSpriggitField(spriggit, "Priority[7]").ShouldBe(Helpers.GetDTOField(dto, "Priority[7]"));
        Helpers.GetSpriggitField(spriggit, "Priority[8]").ShouldBe(Helpers.GetDTOField(dto, "Priority[8]"));
        Helpers.GetSpriggitField(spriggit, "Priority[9]").ShouldBe(Helpers.GetDTOField(dto, "Priority[9]"));
        Helpers.GetSpriggitField(spriggit, "Value[0]").ShouldBe(Helpers.GetDTOField(dto, "Value[0]"));
        Helpers.GetSpriggitField(spriggit, "Value[1]").ShouldBe(Helpers.GetDTOField(dto, "Value[1]"));
        Helpers.GetSpriggitField(spriggit, "Value[10]").ShouldBe(Helpers.GetDTOField(dto, "Value[10]"));
        Helpers.GetSpriggitField(spriggit, "Value[11]").ShouldBe(Helpers.GetDTOField(dto, "Value[11]"));
        Helpers.GetSpriggitField(spriggit, "Value[12]").ShouldBe(Helpers.GetDTOField(dto, "Value[12]"));
        Helpers.GetSpriggitField(spriggit, "Value[13]").ShouldBe(Helpers.GetDTOField(dto, "Value[13]"));
        Helpers.GetSpriggitField(spriggit, "Value[14]").ShouldBe(Helpers.GetDTOField(dto, "Value[14]"));
        Helpers.GetSpriggitField(spriggit, "Value[15]").ShouldBe(Helpers.GetDTOField(dto, "Value[15]"));
        Helpers.GetSpriggitField(spriggit, "Value[16]").ShouldBe(Helpers.GetDTOField(dto, "Value[16]"));
        Helpers.GetSpriggitField(spriggit, "Value[2]").ShouldBe(Helpers.GetDTOField(dto, "Value[2]"));
        Helpers.GetSpriggitField(spriggit, "Value[3]").ShouldBe(Helpers.GetDTOField(dto, "Value[3]"));
        Helpers.GetSpriggitField(spriggit, "Value[4]").ShouldBe(Helpers.GetDTOField(dto, "Value[4]"));
        Helpers.GetSpriggitField(spriggit, "Value[5]").ShouldBe(Helpers.GetDTOField(dto, "Value[5]"));
        Helpers.GetSpriggitField(spriggit, "Value[6]").ShouldBe(Helpers.GetDTOField(dto, "Value[6]"));
        Helpers.GetSpriggitField(spriggit, "Value[7]").ShouldBe(Helpers.GetDTOField(dto, "Value[7]"));
        Helpers.GetSpriggitField(spriggit, "Value[8]").ShouldBe(Helpers.GetDTOField(dto, "Value[8]"));
        Helpers.GetSpriggitField(spriggit, "Value[9]").ShouldBe(Helpers.GetDTOField(dto, "Value[9]"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "ActorValue[0]", "ActorValue[1]", "ActorValue[10]", "ActorValue[11]", "ActorValue[12]", "ActorValue[13]", "ActorValue[2]", "ActorValue[3]", "ActorValue[4]", "ActorValue[5]", "ActorValue[6]", "ActorValue[7]", "ActorValue[8]", "ActorValue[9]", "CompareOperator", "ComparisonValue[0]", "ComparisonValue[1]", "ComparisonValue[10]", "ComparisonValue[11]", "ComparisonValue[12]", "ComparisonValue[13]", "ComparisonValue[14]", "ComparisonValue[15]", "ComparisonValue[16]", "ComparisonValue[17]", "ComparisonValue[18]", "ComparisonValue[19]", "ComparisonValue[2]", "ComparisonValue[3]", "ComparisonValue[4]", "ComparisonValue[5]", "ComparisonValue[6]", "ComparisonValue[7]", "ComparisonValue[8]", "ComparisonValue[9]", "Data.Function[0]", "Data.Function[1]", "Data.Function[10]", "Data.Function[11]", "Data.Function[12]", "Data.Function[13]", "Data.Function[14]", "Data.Function[15]", "Data.Function[16]", "Data.Function[17]", "Data.Function[18]", "Data.Function[19]", "Data.Function[2]", "Data.Function[20]", "Data.Function[21]", "Data.Function[22]", "Data.Function[3]", "Data.Function[4]", "Data.Function[5]", "Data.Function[6]", "Data.Function[7]", "Data.Function[8]", "Data.Function[9]", "Data.MutagenObjectType[0]", "Data.MutagenObjectType[1]", "Data.MutagenObjectType[10]", "Data.MutagenObjectType[11]", "Data.MutagenObjectType[12]", "Data.MutagenObjectType[13]", "Data.MutagenObjectType[14]", "Data.MutagenObjectType[15]", "Data.MutagenObjectType[16]", "Data.MutagenObjectType[17]", "Data.MutagenObjectType[18]", "Data.MutagenObjectType[19]", "Data.MutagenObjectType[2]", "Data.MutagenObjectType[20]", "Data.MutagenObjectType[21]", "Data.MutagenObjectType[22]", "Data.MutagenObjectType[3]", "Data.MutagenObjectType[4]", "Data.MutagenObjectType[5]", "Data.MutagenObjectType[6]", "Data.MutagenObjectType[7]", "Data.MutagenObjectType[8]", "Data.MutagenObjectType[9]", "Data.ParameterOneNumber[0]", "Data.ParameterOneNumber[1]", "Data.ParameterOneNumber[10]", "Data.ParameterOneNumber[11]", "Data.ParameterOneNumber[12]", "Data.ParameterOneNumber[13]", "Data.ParameterOneNumber[14]", "Data.ParameterOneNumber[15]", "Data.ParameterOneNumber[16]", "Data.ParameterOneNumber[17]", "Data.ParameterOneNumber[18]", "Data.ParameterOneNumber[19]", "Data.ParameterOneNumber[2]", "Data.ParameterOneNumber[20]", "Data.ParameterOneNumber[21]", "Data.ParameterOneNumber[3]", "Data.ParameterOneNumber[4]", "Data.ParameterOneNumber[5]", "Data.ParameterOneNumber[6]", "Data.ParameterOneNumber[7]", "Data.ParameterOneNumber[8]", "Data.ParameterOneNumber[9]", "Data.ParameterOneRecord[0]", "Data.ParameterOneRecord[1]", "Data.ParameterOneRecord[10]", "Data.ParameterOneRecord[11]", "Data.ParameterOneRecord[12]", "Data.ParameterOneRecord[13]", "Data.ParameterOneRecord[14]", "Data.ParameterOneRecord[15]", "Data.ParameterOneRecord[16]", "Data.ParameterOneRecord[17]", "Data.ParameterOneRecord[18]", "Data.ParameterOneRecord[19]", "Data.ParameterOneRecord[2]", "Data.ParameterOneRecord[20]", "Data.ParameterOneRecord[21]", "Data.ParameterOneRecord[3]", "Data.ParameterOneRecord[4]", "Data.ParameterOneRecord[5]", "Data.ParameterOneRecord[6]", "Data.ParameterOneRecord[7]", "Data.ParameterOneRecord[8]", "Data.ParameterOneRecord[9]", "Description.TargetLanguage", "EditorID", "EntryPoint[0]", "EntryPoint[1]", "EntryPoint[10]", "EntryPoint[11]", "EntryPoint[12]", "EntryPoint[13]", "EntryPoint[14]", "EntryPoint[15]", "EntryPoint[16]", "EntryPoint[2]", "EntryPoint[3]", "EntryPoint[4]", "EntryPoint[5]", "EntryPoint[6]", "EntryPoint[7]", "EntryPoint[8]", "EntryPoint[9]", "FormKey", "Hidden", "MajorRecordFlagsRaw", "Modification[0]", "Modification[1]", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "NumRanks", "PerkConditionTabCount[0]", "PerkConditionTabCount[1]", "PerkConditionTabCount[10]", "PerkConditionTabCount[11]", "PerkConditionTabCount[12]", "PerkConditionTabCount[13]", "PerkConditionTabCount[14]", "PerkConditionTabCount[15]", "PerkConditionTabCount[16]", "PerkConditionTabCount[2]", "PerkConditionTabCount[3]", "PerkConditionTabCount[4]", "PerkConditionTabCount[5]", "PerkConditionTabCount[6]", "PerkConditionTabCount[7]", "PerkConditionTabCount[8]", "PerkConditionTabCount[9]", "PerkEntryID[0]", "PerkEntryID[1]", "PerkEntryID[10]", "PerkEntryID[11]", "PerkEntryID[12]", "PerkEntryID[13]", "PerkEntryID[14]", "PerkEntryID[15]", "PerkEntryID[16]", "PerkEntryID[2]", "PerkEntryID[3]", "PerkEntryID[4]", "PerkEntryID[5]", "PerkEntryID[6]", "PerkEntryID[7]", "PerkEntryID[8]", "PerkEntryID[9]", "Priority[0]", "Priority[1]", "Priority[10]", "Priority[11]", "Priority[12]", "Priority[13]", "Priority[14]", "Priority[15]", "Priority[2]", "Priority[3]", "Priority[4]", "Priority[5]", "Priority[6]", "Priority[7]", "Priority[8]", "Priority[9]", "Value[0]", "Value[1]", "Value[10]", "Value[11]", "Value[12]", "Value[13]", "Value[14]", "Value[15]", "Value[16]", "Value[2]", "Value[3]", "Value[4]", "Value[5]", "Value[6]", "Value[7]", "Value[8]", "Value[9]", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "ActorValue[0]", "ActorValue[1]", "ActorValue[10]", "ActorValue[11]", "ActorValue[12]", "ActorValue[13]", "ActorValue[2]", "ActorValue[3]", "ActorValue[4]", "ActorValue[5]", "ActorValue[6]", "ActorValue[7]", "ActorValue[8]", "ActorValue[9]", "CompareOperator", "ComparisonValue[0]", "ComparisonValue[1]", "ComparisonValue[10]", "ComparisonValue[11]", "ComparisonValue[12]", "ComparisonValue[13]", "ComparisonValue[14]", "ComparisonValue[15]", "ComparisonValue[16]", "ComparisonValue[17]", "ComparisonValue[18]", "ComparisonValue[19]", "ComparisonValue[2]", "ComparisonValue[3]", "ComparisonValue[4]", "ComparisonValue[5]", "ComparisonValue[6]", "ComparisonValue[7]", "ComparisonValue[8]", "ComparisonValue[9]", "Data.Function[0]", "Data.Function[1]", "Data.Function[10]", "Data.Function[11]", "Data.Function[12]", "Data.Function[13]", "Data.Function[14]", "Data.Function[15]", "Data.Function[16]", "Data.Function[17]", "Data.Function[18]", "Data.Function[19]", "Data.Function[2]", "Data.Function[20]", "Data.Function[21]", "Data.Function[22]", "Data.Function[3]", "Data.Function[4]", "Data.Function[5]", "Data.Function[6]", "Data.Function[7]", "Data.Function[8]", "Data.Function[9]", "Data.MutagenObjectType[0]", "Data.MutagenObjectType[1]", "Data.MutagenObjectType[10]", "Data.MutagenObjectType[11]", "Data.MutagenObjectType[12]", "Data.MutagenObjectType[13]", "Data.MutagenObjectType[14]", "Data.MutagenObjectType[15]", "Data.MutagenObjectType[16]", "Data.MutagenObjectType[17]", "Data.MutagenObjectType[18]", "Data.MutagenObjectType[19]", "Data.MutagenObjectType[2]", "Data.MutagenObjectType[20]", "Data.MutagenObjectType[21]", "Data.MutagenObjectType[22]", "Data.MutagenObjectType[3]", "Data.MutagenObjectType[4]", "Data.MutagenObjectType[5]", "Data.MutagenObjectType[6]", "Data.MutagenObjectType[7]", "Data.MutagenObjectType[8]", "Data.MutagenObjectType[9]", "Data.ParameterOneNumber[0]", "Data.ParameterOneNumber[1]", "Data.ParameterOneNumber[10]", "Data.ParameterOneNumber[11]", "Data.ParameterOneNumber[12]", "Data.ParameterOneNumber[13]", "Data.ParameterOneNumber[14]", "Data.ParameterOneNumber[15]", "Data.ParameterOneNumber[16]", "Data.ParameterOneNumber[17]", "Data.ParameterOneNumber[18]", "Data.ParameterOneNumber[19]", "Data.ParameterOneNumber[2]", "Data.ParameterOneNumber[20]", "Data.ParameterOneNumber[21]", "Data.ParameterOneNumber[3]", "Data.ParameterOneNumber[4]", "Data.ParameterOneNumber[5]", "Data.ParameterOneNumber[6]", "Data.ParameterOneNumber[7]", "Data.ParameterOneNumber[8]", "Data.ParameterOneNumber[9]", "Data.ParameterOneRecord[0]", "Data.ParameterOneRecord[1]", "Data.ParameterOneRecord[10]", "Data.ParameterOneRecord[11]", "Data.ParameterOneRecord[12]", "Data.ParameterOneRecord[13]", "Data.ParameterOneRecord[14]", "Data.ParameterOneRecord[15]", "Data.ParameterOneRecord[16]", "Data.ParameterOneRecord[17]", "Data.ParameterOneRecord[18]", "Data.ParameterOneRecord[19]", "Data.ParameterOneRecord[2]", "Data.ParameterOneRecord[20]", "Data.ParameterOneRecord[21]", "Data.ParameterOneRecord[3]", "Data.ParameterOneRecord[4]", "Data.ParameterOneRecord[5]", "Data.ParameterOneRecord[6]", "Data.ParameterOneRecord[7]", "Data.ParameterOneRecord[8]", "Data.ParameterOneRecord[9]", "Description.TargetLanguage", "EditorID", "EntryPoint[0]", "EntryPoint[1]", "EntryPoint[10]", "EntryPoint[11]", "EntryPoint[12]", "EntryPoint[13]", "EntryPoint[14]", "EntryPoint[15]", "EntryPoint[16]", "EntryPoint[2]", "EntryPoint[3]", "EntryPoint[4]", "EntryPoint[5]", "EntryPoint[6]", "EntryPoint[7]", "EntryPoint[8]", "EntryPoint[9]", "FormKey", "Hidden", "MajorRecordFlags", "Modification[0]", "Modification[1]", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "NumRanks", "PerkConditionTabCount[0]", "PerkConditionTabCount[1]", "PerkConditionTabCount[10]", "PerkConditionTabCount[11]", "PerkConditionTabCount[12]", "PerkConditionTabCount[13]", "PerkConditionTabCount[14]", "PerkConditionTabCount[15]", "PerkConditionTabCount[16]", "PerkConditionTabCount[2]", "PerkConditionTabCount[3]", "PerkConditionTabCount[4]", "PerkConditionTabCount[5]", "PerkConditionTabCount[6]", "PerkConditionTabCount[7]", "PerkConditionTabCount[8]", "PerkConditionTabCount[9]", "PerkEntryID[0]", "PerkEntryID[1]", "PerkEntryID[10]", "PerkEntryID[11]", "PerkEntryID[12]", "PerkEntryID[13]", "PerkEntryID[14]", "PerkEntryID[15]", "PerkEntryID[16]", "PerkEntryID[2]", "PerkEntryID[3]", "PerkEntryID[4]", "PerkEntryID[5]", "PerkEntryID[6]", "PerkEntryID[7]", "PerkEntryID[8]", "PerkEntryID[9]", "Priority[0]", "Priority[1]", "Priority[10]", "Priority[11]", "Priority[12]", "Priority[13]", "Priority[14]", "Priority[15]", "Priority[2]", "Priority[3]", "Priority[4]", "Priority[5]", "Priority[6]", "Priority[7]", "Priority[8]", "Priority[9]", "Value[0]", "Value[1]", "Value[10]", "Value[11]", "Value[12]", "Value[13]", "Value[14]", "Value[15]", "Value[16]", "Value[2]", "Value[3]", "Value[4]", "Value[5]", "Value[6]", "Value[7]", "Value[8]", "Value[9]", "Version2", "VersionControl");
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

        Helpers.GetSpriggitField(spriggit, "ButtonLabel.Count").ShouldBe(Helpers.GetDTOField(dto, "ButtonLabel.Count"));
        Helpers.GetSpriggitField(spriggit, "ButtonLabel.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "ButtonLabel.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "ButtonLabel[0].Language").ShouldBe(Helpers.GetDTOField(dto, "ButtonLabel[0].Language"));
        Helpers.GetSpriggitField(spriggit, "ButtonLabel[0].String").ShouldBe(Helpers.GetDTOField(dto, "ButtonLabel[0].String"));
        Helpers.GetSpriggitField(spriggit, "ButtonLabel[1].Language").ShouldBe(Helpers.GetDTOField(dto, "ButtonLabel[1].Language"));
        Helpers.GetSpriggitField(spriggit, "ButtonLabel[1].String").ShouldBe(Helpers.GetDTOField(dto, "ButtonLabel[1].String"));
        Helpers.GetSpriggitField(spriggit, "ButtonLabel[10].Language").ShouldBe(Helpers.GetDTOField(dto, "ButtonLabel[10].Language"));
        Helpers.GetSpriggitField(spriggit, "ButtonLabel[10].String").ShouldBe(Helpers.GetDTOField(dto, "ButtonLabel[10].String"));
        Helpers.GetSpriggitField(spriggit, "ButtonLabel[2].Language").ShouldBe(Helpers.GetDTOField(dto, "ButtonLabel[2].Language"));
        Helpers.GetSpriggitField(spriggit, "ButtonLabel[2].String").ShouldBe(Helpers.GetDTOField(dto, "ButtonLabel[2].String"));
        Helpers.GetSpriggitField(spriggit, "ButtonLabel[3].Language").ShouldBe(Helpers.GetDTOField(dto, "ButtonLabel[3].Language"));
        Helpers.GetSpriggitField(spriggit, "ButtonLabel[3].String").ShouldBe(Helpers.GetDTOField(dto, "ButtonLabel[3].String"));
        Helpers.GetSpriggitField(spriggit, "ButtonLabel[4].Language").ShouldBe(Helpers.GetDTOField(dto, "ButtonLabel[4].Language"));
        Helpers.GetSpriggitField(spriggit, "ButtonLabel[4].String").ShouldBe(Helpers.GetDTOField(dto, "ButtonLabel[4].String"));
        Helpers.GetSpriggitField(spriggit, "ButtonLabel[5].Language").ShouldBe(Helpers.GetDTOField(dto, "ButtonLabel[5].Language"));
        Helpers.GetSpriggitField(spriggit, "ButtonLabel[5].String").ShouldBe(Helpers.GetDTOField(dto, "ButtonLabel[5].String"));
        Helpers.GetSpriggitField(spriggit, "ButtonLabel[6].Language").ShouldBe(Helpers.GetDTOField(dto, "ButtonLabel[6].Language"));
        Helpers.GetSpriggitField(spriggit, "ButtonLabel[6].String").ShouldBe(Helpers.GetDTOField(dto, "ButtonLabel[6].String"));
        Helpers.GetSpriggitField(spriggit, "ButtonLabel[7].Language").ShouldBe(Helpers.GetDTOField(dto, "ButtonLabel[7].Language"));
        Helpers.GetSpriggitField(spriggit, "ButtonLabel[7].String").ShouldBe(Helpers.GetDTOField(dto, "ButtonLabel[7].String"));
        Helpers.GetSpriggitField(spriggit, "ButtonLabel[8].Language").ShouldBe(Helpers.GetDTOField(dto, "ButtonLabel[8].Language"));
        Helpers.GetSpriggitField(spriggit, "ButtonLabel[8].String").ShouldBe(Helpers.GetDTOField(dto, "ButtonLabel[8].String"));
        Helpers.GetSpriggitField(spriggit, "ButtonLabel[9].Language").ShouldBe(Helpers.GetDTOField(dto, "ButtonLabel[9].Language"));
        Helpers.GetSpriggitField(spriggit, "ButtonLabel[9].String").ShouldBe(Helpers.GetDTOField(dto, "ButtonLabel[9].String"));
        Helpers.GetSpriggitField(spriggit, "CompareOperator[0]").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator[0]"));
        Helpers.GetSpriggitField(spriggit, "CompareOperator[1]").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator[1]"));
        Helpers.GetSpriggitField(spriggit, "CompareOperator[2]").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator[2]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[0]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[0]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[1]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[1]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[2]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[2]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[3]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[3]"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[1]"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[10]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[10]"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[11]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[11]"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[2]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[2]"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[3]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[3]"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[4]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[4]"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[5]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[5]"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[6]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[6]"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[7]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[7]"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[8]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[8]"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[9]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[9]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[1]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[10]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[10]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[11]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[11]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[2]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[2]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[3]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[3]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[4]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[4]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[5]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[5]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[6]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[6]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[7]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[7]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[8]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[8]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[9]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[9]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneNumber[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneNumber[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneNumber[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneNumber[1]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneNumber[2]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneNumber[2]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneNumber[3]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneNumber[3]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneNumber[4]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneNumber[4]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneNumber[5]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneNumber[5]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneNumber[6]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneNumber[6]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneNumber[7]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneNumber[7]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneNumber[8]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneNumber[8]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneRecord[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneRecord[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneRecord[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneRecord[1]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneRecord[2]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneRecord[2]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneRecord[3]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneRecord[3]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneRecord[4]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneRecord[4]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneRecord[5]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneRecord[5]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneRecord[6]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneRecord[6]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneRecord[7]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneRecord[7]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneRecord[8]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneRecord[8]"));
        Helpers.GetSpriggitField(spriggit, "Data.Reference[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.Reference[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.Reference[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.Reference[1]"));
        Helpers.GetSpriggitField(spriggit, "Data.RunOnType[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.RunOnType[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.RunOnType[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.RunOnType[1]"));
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
        Helpers.GetSpriggitField(spriggit, "EntryPoint").ShouldBe(Helpers.GetDTOField(dto, "EntryPoint"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "Name.Count").ShouldBe(Helpers.GetDTOField(dto, "Name.Count"));
        Helpers.GetSpriggitField(spriggit, "Name.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Name.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Name[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[0].String").ShouldBe(Helpers.GetDTOField(dto, "Name[0].String"));
        Helpers.GetSpriggitField(spriggit, "Name[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[1].String").ShouldBe(Helpers.GetDTOField(dto, "Name[1].String"));
        Helpers.GetSpriggitField(spriggit, "Name[10].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[10].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[10].String").ShouldBe(Helpers.GetDTOField(dto, "Name[10].String"));
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
        Helpers.GetSpriggitField(spriggit, "Name[9].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[9].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[9].String").ShouldBe(Helpers.GetDTOField(dto, "Name[9].String"));
        Helpers.GetSpriggitField(spriggit, "NextPerk").ShouldBe(Helpers.GetDTOField(dto, "NextPerk"));
        Helpers.GetSpriggitField(spriggit, "NumRanks").ShouldBe(Helpers.GetDTOField(dto, "NumRanks"));
        Helpers.GetSpriggitField(spriggit, "PerkConditionTabCount").ShouldBe(Helpers.GetDTOField(dto, "PerkConditionTabCount"));
        Helpers.GetSpriggitField(spriggit, "PerkEntryID").ShouldBe(Helpers.GetDTOField(dto, "PerkEntryID"));
        Helpers.GetSpriggitField(spriggit, "Playable").ShouldBe(Helpers.GetDTOField(dto, "Playable"));
        Helpers.GetSpriggitField(spriggit, "Sound").ShouldBe(Helpers.GetDTOField(dto, "Sound"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter.ScriptFragments.Count").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter.ScriptFragments.Count"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter.ScriptFragments.ExtraBindDataVersion").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter.ScriptFragments.ExtraBindDataVersion"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter.ScriptFragments.Script.Count").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter.ScriptFragments.Script.Count"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter.ScriptFragments.Script.Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter.ScriptFragments.Script.Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter.ScriptFragments.Script[0].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter.ScriptFragments.Script[0].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter.ScriptFragments.Script[0].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter.ScriptFragments.Script[0].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter.ScriptFragments.Script[0].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter.ScriptFragments.Script[0].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter.ScriptFragments.Script[1].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter.ScriptFragments.Script[1].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter.ScriptFragments.Script[1].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter.ScriptFragments.Script[1].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter.ScriptFragments.Script[1].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter.ScriptFragments.Script[1].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter.ScriptFragments.Script[2].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter.ScriptFragments.Script[2].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter.ScriptFragments.Script[2].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter.ScriptFragments.Script[2].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter.ScriptFragments.Script[2].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter.ScriptFragments.Script[2].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter.ScriptFragments.Script[3].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter.ScriptFragments.Script[3].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter.ScriptFragments.Script[3].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter.ScriptFragments.Script[3].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter.ScriptFragments.Script[3].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter.ScriptFragments.Script[3].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter.ScriptFragments[0].FragmentName").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter.ScriptFragments[0].FragmentName"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter.ScriptFragments[0].ScriptName").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter.ScriptFragments[0].ScriptName"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter.ScriptFragments[0].Unknown2").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter.ScriptFragments[0].Unknown2"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "ButtonLabel.Count", "ButtonLabel.TargetLanguage", "ButtonLabel[0].Language", "ButtonLabel[0].String", "ButtonLabel[1].Language", "ButtonLabel[1].String", "ButtonLabel[10].Language", "ButtonLabel[10].String", "ButtonLabel[2].Language", "ButtonLabel[2].String", "ButtonLabel[3].Language", "ButtonLabel[3].String", "ButtonLabel[4].Language", "ButtonLabel[4].String", "ButtonLabel[5].Language", "ButtonLabel[5].String", "ButtonLabel[6].Language", "ButtonLabel[6].String", "ButtonLabel[7].Language", "ButtonLabel[7].String", "ButtonLabel[8].Language", "ButtonLabel[8].String", "ButtonLabel[9].Language", "ButtonLabel[9].String", "CompareOperator[0]", "CompareOperator[1]", "CompareOperator[2]", "ComparisonValue[0]", "ComparisonValue[1]", "ComparisonValue[2]", "ComparisonValue[3]", "Data.Function[0]", "Data.Function[1]", "Data.Function[10]", "Data.Function[11]", "Data.Function[2]", "Data.Function[3]", "Data.Function[4]", "Data.Function[5]", "Data.Function[6]", "Data.Function[7]", "Data.Function[8]", "Data.Function[9]", "Data.MutagenObjectType[0]", "Data.MutagenObjectType[1]", "Data.MutagenObjectType[10]", "Data.MutagenObjectType[11]", "Data.MutagenObjectType[2]", "Data.MutagenObjectType[3]", "Data.MutagenObjectType[4]", "Data.MutagenObjectType[5]", "Data.MutagenObjectType[6]", "Data.MutagenObjectType[7]", "Data.MutagenObjectType[8]", "Data.MutagenObjectType[9]", "Data.ParameterOneNumber[0]", "Data.ParameterOneNumber[1]", "Data.ParameterOneNumber[2]", "Data.ParameterOneNumber[3]", "Data.ParameterOneNumber[4]", "Data.ParameterOneNumber[5]", "Data.ParameterOneNumber[6]", "Data.ParameterOneNumber[7]", "Data.ParameterOneNumber[8]", "Data.ParameterOneRecord[0]", "Data.ParameterOneRecord[1]", "Data.ParameterOneRecord[2]", "Data.ParameterOneRecord[3]", "Data.ParameterOneRecord[4]", "Data.ParameterOneRecord[5]", "Data.ParameterOneRecord[6]", "Data.ParameterOneRecord[7]", "Data.ParameterOneRecord[8]", "Data.Reference[0]", "Data.Reference[1]", "Data.RunOnType[0]", "Data.RunOnType[1]", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[10].Language", "Description[10].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "Description[9].Language", "Description[9].String", "EditorID", "EntryPoint", "FormKey", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "NextPerk", "NumRanks", "PerkConditionTabCount", "PerkEntryID", "Playable", "Sound", "Version2", "VersionControl", "VirtualMachineAdapter.ScriptFragments.Count", "VirtualMachineAdapter.ScriptFragments.ExtraBindDataVersion", "VirtualMachineAdapter.ScriptFragments.Script.Count", "VirtualMachineAdapter.ScriptFragments.Script.Name", "VirtualMachineAdapter.ScriptFragments.Script[0].MutagenObjectType", "VirtualMachineAdapter.ScriptFragments.Script[0].Name", "VirtualMachineAdapter.ScriptFragments.Script[0].Object", "VirtualMachineAdapter.ScriptFragments.Script[1].MutagenObjectType", "VirtualMachineAdapter.ScriptFragments.Script[1].Name", "VirtualMachineAdapter.ScriptFragments.Script[1].Object", "VirtualMachineAdapter.ScriptFragments.Script[2].MutagenObjectType", "VirtualMachineAdapter.ScriptFragments.Script[2].Name", "VirtualMachineAdapter.ScriptFragments.Script[2].Object", "VirtualMachineAdapter.ScriptFragments.Script[3].MutagenObjectType", "VirtualMachineAdapter.ScriptFragments.Script[3].Name", "VirtualMachineAdapter.ScriptFragments.Script[3].Object", "VirtualMachineAdapter.ScriptFragments[0].FragmentName", "VirtualMachineAdapter.ScriptFragments[0].ScriptName", "VirtualMachineAdapter.ScriptFragments[0].Unknown2");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "ButtonLabel.Count", "ButtonLabel.TargetLanguage", "ButtonLabel[0].Language", "ButtonLabel[0].String", "ButtonLabel[1].Language", "ButtonLabel[1].String", "ButtonLabel[10].Language", "ButtonLabel[10].String", "ButtonLabel[2].Language", "ButtonLabel[2].String", "ButtonLabel[3].Language", "ButtonLabel[3].String", "ButtonLabel[4].Language", "ButtonLabel[4].String", "ButtonLabel[5].Language", "ButtonLabel[5].String", "ButtonLabel[6].Language", "ButtonLabel[6].String", "ButtonLabel[7].Language", "ButtonLabel[7].String", "ButtonLabel[8].Language", "ButtonLabel[8].String", "ButtonLabel[9].Language", "ButtonLabel[9].String", "CompareOperator[0]", "CompareOperator[1]", "CompareOperator[2]", "ComparisonValue[0]", "ComparisonValue[1]", "ComparisonValue[2]", "ComparisonValue[3]", "Data.Function[0]", "Data.Function[1]", "Data.Function[10]", "Data.Function[11]", "Data.Function[2]", "Data.Function[3]", "Data.Function[4]", "Data.Function[5]", "Data.Function[6]", "Data.Function[7]", "Data.Function[8]", "Data.Function[9]", "Data.MutagenObjectType[0]", "Data.MutagenObjectType[1]", "Data.MutagenObjectType[10]", "Data.MutagenObjectType[11]", "Data.MutagenObjectType[2]", "Data.MutagenObjectType[3]", "Data.MutagenObjectType[4]", "Data.MutagenObjectType[5]", "Data.MutagenObjectType[6]", "Data.MutagenObjectType[7]", "Data.MutagenObjectType[8]", "Data.MutagenObjectType[9]", "Data.ParameterOneNumber[0]", "Data.ParameterOneNumber[1]", "Data.ParameterOneNumber[2]", "Data.ParameterOneNumber[3]", "Data.ParameterOneNumber[4]", "Data.ParameterOneNumber[5]", "Data.ParameterOneNumber[6]", "Data.ParameterOneNumber[7]", "Data.ParameterOneNumber[8]", "Data.ParameterOneRecord[0]", "Data.ParameterOneRecord[1]", "Data.ParameterOneRecord[2]", "Data.ParameterOneRecord[3]", "Data.ParameterOneRecord[4]", "Data.ParameterOneRecord[5]", "Data.ParameterOneRecord[6]", "Data.ParameterOneRecord[7]", "Data.ParameterOneRecord[8]", "Data.Reference[0]", "Data.Reference[1]", "Data.RunOnType[0]", "Data.RunOnType[1]", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[10].Language", "Description[10].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "Description[9].Language", "Description[9].String", "EditorID", "EntryPoint", "FormKey", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "NextPerk", "NumRanks", "PerkConditionTabCount", "PerkEntryID", "Playable", "Sound", "Version2", "VersionControl", "VirtualMachineAdapter.ScriptFragments.Count", "VirtualMachineAdapter.ScriptFragments.ExtraBindDataVersion", "VirtualMachineAdapter.ScriptFragments.Script.Count", "VirtualMachineAdapter.ScriptFragments.Script.Name", "VirtualMachineAdapter.ScriptFragments.Script[0].MutagenObjectType", "VirtualMachineAdapter.ScriptFragments.Script[0].Name", "VirtualMachineAdapter.ScriptFragments.Script[0].Object", "VirtualMachineAdapter.ScriptFragments.Script[1].MutagenObjectType", "VirtualMachineAdapter.ScriptFragments.Script[1].Name", "VirtualMachineAdapter.ScriptFragments.Script[1].Object", "VirtualMachineAdapter.ScriptFragments.Script[2].MutagenObjectType", "VirtualMachineAdapter.ScriptFragments.Script[2].Name", "VirtualMachineAdapter.ScriptFragments.Script[2].Object", "VirtualMachineAdapter.ScriptFragments.Script[3].MutagenObjectType", "VirtualMachineAdapter.ScriptFragments.Script[3].Name", "VirtualMachineAdapter.ScriptFragments.Script[3].Object", "VirtualMachineAdapter.ScriptFragments[0].FragmentName", "VirtualMachineAdapter.ScriptFragments[0].ScriptName", "VirtualMachineAdapter.ScriptFragments[0].Unknown2");
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

        Helpers.GetSpriggitField(spriggit, "ButtonLabel.Count").ShouldBe(Helpers.GetDTOField(dto, "ButtonLabel.Count"));
        Helpers.GetSpriggitField(spriggit, "ButtonLabel.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "ButtonLabel.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "ButtonLabel[0].Language").ShouldBe(Helpers.GetDTOField(dto, "ButtonLabel[0].Language"));
        Helpers.GetSpriggitField(spriggit, "ButtonLabel[0].String").ShouldBe(Helpers.GetDTOField(dto, "ButtonLabel[0].String"));
        Helpers.GetSpriggitField(spriggit, "ButtonLabel[1].Language").ShouldBe(Helpers.GetDTOField(dto, "ButtonLabel[1].Language"));
        Helpers.GetSpriggitField(spriggit, "ButtonLabel[1].String").ShouldBe(Helpers.GetDTOField(dto, "ButtonLabel[1].String"));
        Helpers.GetSpriggitField(spriggit, "ButtonLabel[10].Language").ShouldBe(Helpers.GetDTOField(dto, "ButtonLabel[10].Language"));
        Helpers.GetSpriggitField(spriggit, "ButtonLabel[10].String").ShouldBe(Helpers.GetDTOField(dto, "ButtonLabel[10].String"));
        Helpers.GetSpriggitField(spriggit, "ButtonLabel[2].Language").ShouldBe(Helpers.GetDTOField(dto, "ButtonLabel[2].Language"));
        Helpers.GetSpriggitField(spriggit, "ButtonLabel[2].String").ShouldBe(Helpers.GetDTOField(dto, "ButtonLabel[2].String"));
        Helpers.GetSpriggitField(spriggit, "ButtonLabel[3].Language").ShouldBe(Helpers.GetDTOField(dto, "ButtonLabel[3].Language"));
        Helpers.GetSpriggitField(spriggit, "ButtonLabel[3].String").ShouldBe(Helpers.GetDTOField(dto, "ButtonLabel[3].String"));
        Helpers.GetSpriggitField(spriggit, "ButtonLabel[4].Language").ShouldBe(Helpers.GetDTOField(dto, "ButtonLabel[4].Language"));
        Helpers.GetSpriggitField(spriggit, "ButtonLabel[4].String").ShouldBe(Helpers.GetDTOField(dto, "ButtonLabel[4].String"));
        Helpers.GetSpriggitField(spriggit, "ButtonLabel[5].Language").ShouldBe(Helpers.GetDTOField(dto, "ButtonLabel[5].Language"));
        Helpers.GetSpriggitField(spriggit, "ButtonLabel[5].String").ShouldBe(Helpers.GetDTOField(dto, "ButtonLabel[5].String"));
        Helpers.GetSpriggitField(spriggit, "ButtonLabel[6].Language").ShouldBe(Helpers.GetDTOField(dto, "ButtonLabel[6].Language"));
        Helpers.GetSpriggitField(spriggit, "ButtonLabel[6].String").ShouldBe(Helpers.GetDTOField(dto, "ButtonLabel[6].String"));
        Helpers.GetSpriggitField(spriggit, "ButtonLabel[7].Language").ShouldBe(Helpers.GetDTOField(dto, "ButtonLabel[7].Language"));
        Helpers.GetSpriggitField(spriggit, "ButtonLabel[7].String").ShouldBe(Helpers.GetDTOField(dto, "ButtonLabel[7].String"));
        Helpers.GetSpriggitField(spriggit, "ButtonLabel[8].Language").ShouldBe(Helpers.GetDTOField(dto, "ButtonLabel[8].Language"));
        Helpers.GetSpriggitField(spriggit, "ButtonLabel[8].String").ShouldBe(Helpers.GetDTOField(dto, "ButtonLabel[8].String"));
        Helpers.GetSpriggitField(spriggit, "ButtonLabel[9].Language").ShouldBe(Helpers.GetDTOField(dto, "ButtonLabel[9].Language"));
        Helpers.GetSpriggitField(spriggit, "ButtonLabel[9].String").ShouldBe(Helpers.GetDTOField(dto, "ButtonLabel[9].String"));
        Helpers.GetSpriggitField(spriggit, "CompareOperator[0]").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator[0]"));
        Helpers.GetSpriggitField(spriggit, "CompareOperator[1]").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator[1]"));
        Helpers.GetSpriggitField(spriggit, "CompareOperator[2]").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator[2]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[0]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[0]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[1]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[1]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[2]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[2]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[3]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[3]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[4]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[4]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[5]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[5]"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[1]"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[2]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[2]"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[3]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[3]"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[4]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[4]"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[5]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[5]"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[6]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[6]"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[7]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[7]"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[8]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[8]"));
        Helpers.GetSpriggitField(spriggit, "Data.Function[9]").ShouldBe(Helpers.GetDTOField(dto, "Data.Function[9]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[1]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[2]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[2]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[3]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[3]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[4]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[4]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[5]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[5]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[6]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[6]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[7]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[7]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[8]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[8]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[9]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[9]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneNumber[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneNumber[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneNumber[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneNumber[1]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneNumber[2]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneNumber[2]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneNumber[3]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneNumber[3]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneNumber[4]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneNumber[4]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneNumber[5]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneNumber[5]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneNumber[6]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneNumber[6]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneRecord[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneRecord[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneRecord[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneRecord[1]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneRecord[2]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneRecord[2]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneRecord[3]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneRecord[3]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneRecord[4]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneRecord[4]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneRecord[5]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneRecord[5]"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneRecord[6]").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneRecord[6]"));
        Helpers.GetSpriggitField(spriggit, "Data.Reference[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.Reference[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.Reference[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.Reference[1]"));
        Helpers.GetSpriggitField(spriggit, "Data.Reference[2]").ShouldBe(Helpers.GetDTOField(dto, "Data.Reference[2]"));
        Helpers.GetSpriggitField(spriggit, "Data.RunOnType[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.RunOnType[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.RunOnType[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.RunOnType[1]"));
        Helpers.GetSpriggitField(spriggit, "Data.RunOnType[2]").ShouldBe(Helpers.GetDTOField(dto, "Data.RunOnType[2]"));
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
        Helpers.GetSpriggitField(spriggit, "EntryPoint").ShouldBe(Helpers.GetDTOField(dto, "EntryPoint"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "Level").ShouldBe(Helpers.GetDTOField(dto, "Level"));
        Helpers.GetSpriggitField(spriggit, "Name.Count").ShouldBe(Helpers.GetDTOField(dto, "Name.Count"));
        Helpers.GetSpriggitField(spriggit, "Name.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Name.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Name[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[0].String").ShouldBe(Helpers.GetDTOField(dto, "Name[0].String"));
        Helpers.GetSpriggitField(spriggit, "Name[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[1].String").ShouldBe(Helpers.GetDTOField(dto, "Name[1].String"));
        Helpers.GetSpriggitField(spriggit, "Name[10].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[10].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[10].String").ShouldBe(Helpers.GetDTOField(dto, "Name[10].String"));
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
        Helpers.GetSpriggitField(spriggit, "Name[9].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[9].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[9].String").ShouldBe(Helpers.GetDTOField(dto, "Name[9].String"));
        Helpers.GetSpriggitField(spriggit, "NextPerk").ShouldBe(Helpers.GetDTOField(dto, "NextPerk"));
        Helpers.GetSpriggitField(spriggit, "NumRanks").ShouldBe(Helpers.GetDTOField(dto, "NumRanks"));
        Helpers.GetSpriggitField(spriggit, "PerkConditionTabCount").ShouldBe(Helpers.GetDTOField(dto, "PerkConditionTabCount"));
        Helpers.GetSpriggitField(spriggit, "PerkEntryID").ShouldBe(Helpers.GetDTOField(dto, "PerkEntryID"));
        Helpers.GetSpriggitField(spriggit, "Playable").ShouldBe(Helpers.GetDTOField(dto, "Playable"));
        Helpers.GetSpriggitField(spriggit, "Sound").ShouldBe(Helpers.GetDTOField(dto, "Sound"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter.ScriptFragments.Count").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter.ScriptFragments.Count"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter.ScriptFragments.ExtraBindDataVersion").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter.ScriptFragments.ExtraBindDataVersion"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter.ScriptFragments.Script.Count").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter.ScriptFragments.Script.Count"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter.ScriptFragments.Script.Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter.ScriptFragments.Script.Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter.ScriptFragments.Script[0].MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter.ScriptFragments.Script[0].MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter.ScriptFragments.Script[0].Name").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter.ScriptFragments.Script[0].Name"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter.ScriptFragments.Script[0].Object").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter.ScriptFragments.Script[0].Object"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter.ScriptFragments[0].FragmentName").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter.ScriptFragments[0].FragmentName"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter.ScriptFragments[0].ScriptName").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter.ScriptFragments[0].ScriptName"));
        Helpers.GetSpriggitField(spriggit, "VirtualMachineAdapter.ScriptFragments[0].Unknown2").ShouldBe(Helpers.GetDTOField(dto, "VirtualMachineAdapter.ScriptFragments[0].Unknown2"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "ButtonLabel.Count", "ButtonLabel.TargetLanguage", "ButtonLabel[0].Language", "ButtonLabel[0].String", "ButtonLabel[1].Language", "ButtonLabel[1].String", "ButtonLabel[10].Language", "ButtonLabel[10].String", "ButtonLabel[2].Language", "ButtonLabel[2].String", "ButtonLabel[3].Language", "ButtonLabel[3].String", "ButtonLabel[4].Language", "ButtonLabel[4].String", "ButtonLabel[5].Language", "ButtonLabel[5].String", "ButtonLabel[6].Language", "ButtonLabel[6].String", "ButtonLabel[7].Language", "ButtonLabel[7].String", "ButtonLabel[8].Language", "ButtonLabel[8].String", "ButtonLabel[9].Language", "ButtonLabel[9].String", "CompareOperator[0]", "CompareOperator[1]", "CompareOperator[2]", "ComparisonValue[0]", "ComparisonValue[1]", "ComparisonValue[2]", "ComparisonValue[3]", "ComparisonValue[4]", "ComparisonValue[5]", "Data.Function[0]", "Data.Function[1]", "Data.Function[2]", "Data.Function[3]", "Data.Function[4]", "Data.Function[5]", "Data.Function[6]", "Data.Function[7]", "Data.Function[8]", "Data.Function[9]", "Data.MutagenObjectType[0]", "Data.MutagenObjectType[1]", "Data.MutagenObjectType[2]", "Data.MutagenObjectType[3]", "Data.MutagenObjectType[4]", "Data.MutagenObjectType[5]", "Data.MutagenObjectType[6]", "Data.MutagenObjectType[7]", "Data.MutagenObjectType[8]", "Data.MutagenObjectType[9]", "Data.ParameterOneNumber[0]", "Data.ParameterOneNumber[1]", "Data.ParameterOneNumber[2]", "Data.ParameterOneNumber[3]", "Data.ParameterOneNumber[4]", "Data.ParameterOneNumber[5]", "Data.ParameterOneNumber[6]", "Data.ParameterOneRecord[0]", "Data.ParameterOneRecord[1]", "Data.ParameterOneRecord[2]", "Data.ParameterOneRecord[3]", "Data.ParameterOneRecord[4]", "Data.ParameterOneRecord[5]", "Data.ParameterOneRecord[6]", "Data.Reference[0]", "Data.Reference[1]", "Data.Reference[2]", "Data.RunOnType[0]", "Data.RunOnType[1]", "Data.RunOnType[2]", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[10].Language", "Description[10].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "Description[9].Language", "Description[9].String", "EditorID", "EntryPoint", "FormKey", "Level", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "NextPerk", "NumRanks", "PerkConditionTabCount", "PerkEntryID", "Playable", "Sound", "Version2", "VersionControl", "VirtualMachineAdapter.ScriptFragments.Count", "VirtualMachineAdapter.ScriptFragments.ExtraBindDataVersion", "VirtualMachineAdapter.ScriptFragments.Script.Count", "VirtualMachineAdapter.ScriptFragments.Script.Name", "VirtualMachineAdapter.ScriptFragments.Script[0].MutagenObjectType", "VirtualMachineAdapter.ScriptFragments.Script[0].Name", "VirtualMachineAdapter.ScriptFragments.Script[0].Object", "VirtualMachineAdapter.ScriptFragments[0].FragmentName", "VirtualMachineAdapter.ScriptFragments[0].ScriptName", "VirtualMachineAdapter.ScriptFragments[0].Unknown2");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "ButtonLabel.Count", "ButtonLabel.TargetLanguage", "ButtonLabel[0].Language", "ButtonLabel[0].String", "ButtonLabel[1].Language", "ButtonLabel[1].String", "ButtonLabel[10].Language", "ButtonLabel[10].String", "ButtonLabel[2].Language", "ButtonLabel[2].String", "ButtonLabel[3].Language", "ButtonLabel[3].String", "ButtonLabel[4].Language", "ButtonLabel[4].String", "ButtonLabel[5].Language", "ButtonLabel[5].String", "ButtonLabel[6].Language", "ButtonLabel[6].String", "ButtonLabel[7].Language", "ButtonLabel[7].String", "ButtonLabel[8].Language", "ButtonLabel[8].String", "ButtonLabel[9].Language", "ButtonLabel[9].String", "CompareOperator[0]", "CompareOperator[1]", "CompareOperator[2]", "ComparisonValue[0]", "ComparisonValue[1]", "ComparisonValue[2]", "ComparisonValue[3]", "ComparisonValue[4]", "ComparisonValue[5]", "Data.Function[0]", "Data.Function[1]", "Data.Function[2]", "Data.Function[3]", "Data.Function[4]", "Data.Function[5]", "Data.Function[6]", "Data.Function[7]", "Data.Function[8]", "Data.Function[9]", "Data.MutagenObjectType[0]", "Data.MutagenObjectType[1]", "Data.MutagenObjectType[2]", "Data.MutagenObjectType[3]", "Data.MutagenObjectType[4]", "Data.MutagenObjectType[5]", "Data.MutagenObjectType[6]", "Data.MutagenObjectType[7]", "Data.MutagenObjectType[8]", "Data.MutagenObjectType[9]", "Data.ParameterOneNumber[0]", "Data.ParameterOneNumber[1]", "Data.ParameterOneNumber[2]", "Data.ParameterOneNumber[3]", "Data.ParameterOneNumber[4]", "Data.ParameterOneNumber[5]", "Data.ParameterOneNumber[6]", "Data.ParameterOneRecord[0]", "Data.ParameterOneRecord[1]", "Data.ParameterOneRecord[2]", "Data.ParameterOneRecord[3]", "Data.ParameterOneRecord[4]", "Data.ParameterOneRecord[5]", "Data.ParameterOneRecord[6]", "Data.Reference[0]", "Data.Reference[1]", "Data.Reference[2]", "Data.RunOnType[0]", "Data.RunOnType[1]", "Data.RunOnType[2]", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[10].Language", "Description[10].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "Description[9].Language", "Description[9].String", "EditorID", "EntryPoint", "FormKey", "Level", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "NextPerk", "NumRanks", "PerkConditionTabCount", "PerkEntryID", "Playable", "Sound", "Version2", "VersionControl", "VirtualMachineAdapter.ScriptFragments.Count", "VirtualMachineAdapter.ScriptFragments.ExtraBindDataVersion", "VirtualMachineAdapter.ScriptFragments.Script.Count", "VirtualMachineAdapter.ScriptFragments.Script.Name", "VirtualMachineAdapter.ScriptFragments.Script[0].MutagenObjectType", "VirtualMachineAdapter.ScriptFragments.Script[0].Name", "VirtualMachineAdapter.ScriptFragments.Script[0].Object", "VirtualMachineAdapter.ScriptFragments[0].FragmentName", "VirtualMachineAdapter.ScriptFragments[0].ScriptName", "VirtualMachineAdapter.ScriptFragments[0].Unknown2");
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

        Helpers.GetSpriggitField(spriggit, "CompareOperator").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue"));
        Helpers.GetSpriggitField(spriggit, "Data.Function").ShouldBe(Helpers.GetDTOField(dto, "Data.Function"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneNumber").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneNumber"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneRecord").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneRecord"));
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
        Helpers.GetSpriggitField(spriggit, "Name.Count").ShouldBe(Helpers.GetDTOField(dto, "Name.Count"));
        Helpers.GetSpriggitField(spriggit, "Name.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Name.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Name[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[0].String").ShouldBe(Helpers.GetDTOField(dto, "Name[0].String"));
        Helpers.GetSpriggitField(spriggit, "Name[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[1].String").ShouldBe(Helpers.GetDTOField(dto, "Name[1].String"));
        Helpers.GetSpriggitField(spriggit, "Name[10].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[10].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[10].String").ShouldBe(Helpers.GetDTOField(dto, "Name[10].String"));
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
        Helpers.GetSpriggitField(spriggit, "Name[9].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[9].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[9].String").ShouldBe(Helpers.GetDTOField(dto, "Name[9].String"));
        Helpers.GetSpriggitField(spriggit, "NextPerk").ShouldBe(Helpers.GetDTOField(dto, "NextPerk"));
        Helpers.GetSpriggitField(spriggit, "NumRanks").ShouldBe(Helpers.GetDTOField(dto, "NumRanks"));
        Helpers.GetSpriggitField(spriggit, "Playable").ShouldBe(Helpers.GetDTOField(dto, "Playable"));
        Helpers.GetSpriggitField(spriggit, "Quest").ShouldBe(Helpers.GetDTOField(dto, "Quest"));
        Helpers.GetSpriggitField(spriggit, "Sound").ShouldBe(Helpers.GetDTOField(dto, "Sound"));
        Helpers.GetSpriggitField(spriggit, "Stage").ShouldBe(Helpers.GetDTOField(dto, "Stage"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "CompareOperator", "ComparisonValue", "Data.Function", "Data.MutagenObjectType", "Data.ParameterOneNumber", "Data.ParameterOneRecord", "Data.Reference", "Data.RunOnType", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[10].Language", "Description[10].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "Description[9].Language", "Description[9].String", "EditorID", "FormKey", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "NextPerk", "NumRanks", "Playable", "Quest", "Sound", "Stage", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "CompareOperator", "ComparisonValue", "Data.Function", "Data.MutagenObjectType", "Data.ParameterOneNumber", "Data.ParameterOneRecord", "Data.Reference", "Data.RunOnType", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[10].Language", "Description[10].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "Description[9].Language", "Description[9].String", "EditorID", "FormKey", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "NextPerk", "NumRanks", "Playable", "Quest", "Sound", "Stage", "Version2", "VersionControl");
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

        Helpers.GetSpriggitField(spriggit, "CompareOperator[0]").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator[0]"));
        Helpers.GetSpriggitField(spriggit, "CompareOperator[1]").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator[1]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[0]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[0]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[1]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[1]"));
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
        Helpers.GetSpriggitField(spriggit, "EntryPoint[0]").ShouldBe(Helpers.GetDTOField(dto, "EntryPoint[0]"));
        Helpers.GetSpriggitField(spriggit, "EntryPoint[1]").ShouldBe(Helpers.GetDTOField(dto, "EntryPoint[1]"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "Level").ShouldBe(Helpers.GetDTOField(dto, "Level"));
        Helpers.GetSpriggitField(spriggit, "Modification[0]").ShouldBe(Helpers.GetDTOField(dto, "Modification[0]"));
        Helpers.GetSpriggitField(spriggit, "Modification[1]").ShouldBe(Helpers.GetDTOField(dto, "Modification[1]"));
        Helpers.GetSpriggitField(spriggit, "Name.Count").ShouldBe(Helpers.GetDTOField(dto, "Name.Count"));
        Helpers.GetSpriggitField(spriggit, "Name.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Name.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Name[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[0].String").ShouldBe(Helpers.GetDTOField(dto, "Name[0].String"));
        Helpers.GetSpriggitField(spriggit, "Name[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[1].String").ShouldBe(Helpers.GetDTOField(dto, "Name[1].String"));
        Helpers.GetSpriggitField(spriggit, "Name[10].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[10].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[10].String").ShouldBe(Helpers.GetDTOField(dto, "Name[10].String"));
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
        Helpers.GetSpriggitField(spriggit, "Name[9].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[9].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[9].String").ShouldBe(Helpers.GetDTOField(dto, "Name[9].String"));
        Helpers.GetSpriggitField(spriggit, "NextPerk").ShouldBe(Helpers.GetDTOField(dto, "NextPerk"));
        Helpers.GetSpriggitField(spriggit, "NumRanks").ShouldBe(Helpers.GetDTOField(dto, "NumRanks"));
        Helpers.GetSpriggitField(spriggit, "PerkConditionTabCount[0]").ShouldBe(Helpers.GetDTOField(dto, "PerkConditionTabCount[0]"));
        Helpers.GetSpriggitField(spriggit, "PerkConditionTabCount[1]").ShouldBe(Helpers.GetDTOField(dto, "PerkConditionTabCount[1]"));
        Helpers.GetSpriggitField(spriggit, "PerkEntryID[0]").ShouldBe(Helpers.GetDTOField(dto, "PerkEntryID[0]"));
        Helpers.GetSpriggitField(spriggit, "PerkEntryID[1]").ShouldBe(Helpers.GetDTOField(dto, "PerkEntryID[1]"));
        Helpers.GetSpriggitField(spriggit, "Playable").ShouldBe(Helpers.GetDTOField(dto, "Playable"));
        Helpers.GetSpriggitField(spriggit, "Sound").ShouldBe(Helpers.GetDTOField(dto, "Sound"));
        Helpers.GetSpriggitField(spriggit, "Value[0]").ShouldBe(Helpers.GetDTOField(dto, "Value[0]"));
        Helpers.GetSpriggitField(spriggit, "Value[1]").ShouldBe(Helpers.GetDTOField(dto, "Value[1]"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "CompareOperator[0]", "CompareOperator[1]", "ComparisonValue[0]", "ComparisonValue[1]", "Data.Function[0]", "Data.Function[1]", "Data.Function[2]", "Data.Function[3]", "Data.MutagenObjectType[0]", "Data.MutagenObjectType[1]", "Data.MutagenObjectType[2]", "Data.MutagenObjectType[3]", "Data.ParameterOneNumber[0]", "Data.ParameterOneNumber[1]", "Data.ParameterOneNumber[2]", "Data.ParameterOneRecord[0]", "Data.ParameterOneRecord[1]", "Data.ParameterOneRecord[2]", "Data.Reference", "Data.RunOnType", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[10].Language", "Description[10].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "Description[9].Language", "Description[9].String", "EditorID", "EntryPoint[0]", "EntryPoint[1]", "FormKey", "Level", "Modification[0]", "Modification[1]", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "NextPerk", "NumRanks", "PerkConditionTabCount[0]", "PerkConditionTabCount[1]", "PerkEntryID[0]", "PerkEntryID[1]", "Playable", "Sound", "Value[0]", "Value[1]", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "CompareOperator[0]", "CompareOperator[1]", "ComparisonValue[0]", "ComparisonValue[1]", "Data.Function[0]", "Data.Function[1]", "Data.Function[2]", "Data.Function[3]", "Data.MutagenObjectType[0]", "Data.MutagenObjectType[1]", "Data.MutagenObjectType[2]", "Data.MutagenObjectType[3]", "Data.ParameterOneNumber[0]", "Data.ParameterOneNumber[1]", "Data.ParameterOneNumber[2]", "Data.ParameterOneRecord[0]", "Data.ParameterOneRecord[1]", "Data.ParameterOneRecord[2]", "Data.Reference", "Data.RunOnType", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[10].Language", "Description[10].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "Description[9].Language", "Description[9].String", "EditorID", "EntryPoint[0]", "EntryPoint[1]", "FormKey", "Level", "Modification[0]", "Modification[1]", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "NextPerk", "NumRanks", "PerkConditionTabCount[0]", "PerkConditionTabCount[1]", "PerkEntryID[0]", "PerkEntryID[1]", "Playable", "Sound", "Value[0]", "Value[1]", "Version2", "VersionControl");
    }
}