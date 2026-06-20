using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.Perk.Skyrim;

public class SkyrimPerkSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "PERK")]
    [Trait("FormKey", "0A725C:Skyrim.esm")]
    [Trait("EditorID", "AlchemySkillBoosts")]
    [Trait("SpriggitFile", "Perks/AlchemySkillBoosts - 0A725C_Skyrim.esm.yaml")]
    public void Skyrim_PERK_ShouldMatchSpriggitSample_AlchemySkillBoosts()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Perk,
            "AlchemySkillBoosts");
        var dto = Helpers.GetDTO<PerkDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Perk,
            "0A725C:Skyrim.esm");

        Helpers.GetSpriggitField(spriggit, "ActorValue[0]").ShouldBe(Helpers.GetDTOField(dto, "ActorValue[0]"));
        Helpers.GetSpriggitField(spriggit, "ActorValue[1]").ShouldBe(Helpers.GetDTOField(dto, "ActorValue[1]"));
        Helpers.GetSpriggitField(spriggit, "ActorValue[10]").ShouldBe(Helpers.GetDTOField(dto, "ActorValue[10]"));
        Helpers.GetSpriggitField(spriggit, "ActorValue[11]").ShouldBe(Helpers.GetDTOField(dto, "ActorValue[11]"));
        Helpers.GetSpriggitField(spriggit, "ActorValue[12]").ShouldBe(Helpers.GetDTOField(dto, "ActorValue[12]"));
        Helpers.GetSpriggitField(spriggit, "ActorValue[13]").ShouldBe(Helpers.GetDTOField(dto, "ActorValue[13]"));
        Helpers.GetSpriggitField(spriggit, "ActorValue[14]").ShouldBe(Helpers.GetDTOField(dto, "ActorValue[14]"));
        Helpers.GetSpriggitField(spriggit, "ActorValue[15]").ShouldBe(Helpers.GetDTOField(dto, "ActorValue[15]"));
        Helpers.GetSpriggitField(spriggit, "ActorValue[16]").ShouldBe(Helpers.GetDTOField(dto, "ActorValue[16]"));
        Helpers.GetSpriggitField(spriggit, "ActorValue[17]").ShouldBe(Helpers.GetDTOField(dto, "ActorValue[17]"));
        Helpers.GetSpriggitField(spriggit, "ActorValue[2]").ShouldBe(Helpers.GetDTOField(dto, "ActorValue[2]"));
        Helpers.GetSpriggitField(spriggit, "ActorValue[3]").ShouldBe(Helpers.GetDTOField(dto, "ActorValue[3]"));
        Helpers.GetSpriggitField(spriggit, "ActorValue[4]").ShouldBe(Helpers.GetDTOField(dto, "ActorValue[4]"));
        Helpers.GetSpriggitField(spriggit, "ActorValue[5]").ShouldBe(Helpers.GetDTOField(dto, "ActorValue[5]"));
        Helpers.GetSpriggitField(spriggit, "ActorValue[6]").ShouldBe(Helpers.GetDTOField(dto, "ActorValue[6]"));
        Helpers.GetSpriggitField(spriggit, "ActorValue[7]").ShouldBe(Helpers.GetDTOField(dto, "ActorValue[7]"));
        Helpers.GetSpriggitField(spriggit, "ActorValue[8]").ShouldBe(Helpers.GetDTOField(dto, "ActorValue[8]"));
        Helpers.GetSpriggitField(spriggit, "ActorValue[9]").ShouldBe(Helpers.GetDTOField(dto, "ActorValue[9]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[0]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[0]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[1]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[1]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[10]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[10]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[11]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[11]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[2]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[2]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[3]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[3]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[4]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[4]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[5]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[5]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[6]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[6]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[7]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[7]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[8]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[8]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[9]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[9]"));
        Helpers.GetSpriggitField(spriggit, "Data.ActorValue[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.ActorValue[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.ActorValue[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.ActorValue[1]"));
        Helpers.GetSpriggitField(spriggit, "Data.ActorValue[2]").ShouldBe(Helpers.GetDTOField(dto, "Data.ActorValue[2]"));
        Helpers.GetSpriggitField(spriggit, "Data.ActorValue[3]").ShouldBe(Helpers.GetDTOField(dto, "Data.ActorValue[3]"));
        Helpers.GetSpriggitField(spriggit, "Data.ActorValue[4]").ShouldBe(Helpers.GetDTOField(dto, "Data.ActorValue[4]"));
        Helpers.GetSpriggitField(spriggit, "Data.Keyword[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.Keyword[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.Keyword[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.Keyword[1]"));
        Helpers.GetSpriggitField(spriggit, "Data.Keyword[2]").ShouldBe(Helpers.GetDTOField(dto, "Data.Keyword[2]"));
        Helpers.GetSpriggitField(spriggit, "Data.Keyword[3]").ShouldBe(Helpers.GetDTOField(dto, "Data.Keyword[3]"));
        Helpers.GetSpriggitField(spriggit, "Data.Keyword[4]").ShouldBe(Helpers.GetDTOField(dto, "Data.Keyword[4]"));
        Helpers.GetSpriggitField(spriggit, "Data.Keyword[5]").ShouldBe(Helpers.GetDTOField(dto, "Data.Keyword[5]"));
        Helpers.GetSpriggitField(spriggit, "Data.Keyword[6]").ShouldBe(Helpers.GetDTOField(dto, "Data.Keyword[6]"));
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
        Helpers.GetSpriggitField(spriggit, "EntryPoint[0]").ShouldBe(Helpers.GetDTOField(dto, "EntryPoint[0]"));
        Helpers.GetSpriggitField(spriggit, "EntryPoint[1]").ShouldBe(Helpers.GetDTOField(dto, "EntryPoint[1]"));
        Helpers.GetSpriggitField(spriggit, "EntryPoint[10]").ShouldBe(Helpers.GetDTOField(dto, "EntryPoint[10]"));
        Helpers.GetSpriggitField(spriggit, "EntryPoint[11]").ShouldBe(Helpers.GetDTOField(dto, "EntryPoint[11]"));
        Helpers.GetSpriggitField(spriggit, "EntryPoint[12]").ShouldBe(Helpers.GetDTOField(dto, "EntryPoint[12]"));
        Helpers.GetSpriggitField(spriggit, "EntryPoint[13]").ShouldBe(Helpers.GetDTOField(dto, "EntryPoint[13]"));
        Helpers.GetSpriggitField(spriggit, "EntryPoint[14]").ShouldBe(Helpers.GetDTOField(dto, "EntryPoint[14]"));
        Helpers.GetSpriggitField(spriggit, "EntryPoint[15]").ShouldBe(Helpers.GetDTOField(dto, "EntryPoint[15]"));
        Helpers.GetSpriggitField(spriggit, "EntryPoint[16]").ShouldBe(Helpers.GetDTOField(dto, "EntryPoint[16]"));
        Helpers.GetSpriggitField(spriggit, "EntryPoint[17]").ShouldBe(Helpers.GetDTOField(dto, "EntryPoint[17]"));
        Helpers.GetSpriggitField(spriggit, "EntryPoint[2]").ShouldBe(Helpers.GetDTOField(dto, "EntryPoint[2]"));
        Helpers.GetSpriggitField(spriggit, "EntryPoint[3]").ShouldBe(Helpers.GetDTOField(dto, "EntryPoint[3]"));
        Helpers.GetSpriggitField(spriggit, "EntryPoint[4]").ShouldBe(Helpers.GetDTOField(dto, "EntryPoint[4]"));
        Helpers.GetSpriggitField(spriggit, "EntryPoint[5]").ShouldBe(Helpers.GetDTOField(dto, "EntryPoint[5]"));
        Helpers.GetSpriggitField(spriggit, "EntryPoint[6]").ShouldBe(Helpers.GetDTOField(dto, "EntryPoint[6]"));
        Helpers.GetSpriggitField(spriggit, "EntryPoint[7]").ShouldBe(Helpers.GetDTOField(dto, "EntryPoint[7]"));
        Helpers.GetSpriggitField(spriggit, "EntryPoint[8]").ShouldBe(Helpers.GetDTOField(dto, "EntryPoint[8]"));
        Helpers.GetSpriggitField(spriggit, "EntryPoint[9]").ShouldBe(Helpers.GetDTOField(dto, "EntryPoint[9]"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "Modification[0]").ShouldBe(Helpers.GetDTOField(dto, "Modification[0]"));
        Helpers.GetSpriggitField(spriggit, "Modification[1]").ShouldBe(Helpers.GetDTOField(dto, "Modification[1]"));
        Helpers.GetSpriggitField(spriggit, "Modification[10]").ShouldBe(Helpers.GetDTOField(dto, "Modification[10]"));
        Helpers.GetSpriggitField(spriggit, "Modification[11]").ShouldBe(Helpers.GetDTOField(dto, "Modification[11]"));
        Helpers.GetSpriggitField(spriggit, "Modification[12]").ShouldBe(Helpers.GetDTOField(dto, "Modification[12]"));
        Helpers.GetSpriggitField(spriggit, "Modification[13]").ShouldBe(Helpers.GetDTOField(dto, "Modification[13]"));
        Helpers.GetSpriggitField(spriggit, "Modification[14]").ShouldBe(Helpers.GetDTOField(dto, "Modification[14]"));
        Helpers.GetSpriggitField(spriggit, "Modification[15]").ShouldBe(Helpers.GetDTOField(dto, "Modification[15]"));
        Helpers.GetSpriggitField(spriggit, "Modification[16]").ShouldBe(Helpers.GetDTOField(dto, "Modification[16]"));
        Helpers.GetSpriggitField(spriggit, "Modification[17]").ShouldBe(Helpers.GetDTOField(dto, "Modification[17]"));
        Helpers.GetSpriggitField(spriggit, "Modification[2]").ShouldBe(Helpers.GetDTOField(dto, "Modification[2]"));
        Helpers.GetSpriggitField(spriggit, "Modification[3]").ShouldBe(Helpers.GetDTOField(dto, "Modification[3]"));
        Helpers.GetSpriggitField(spriggit, "Modification[4]").ShouldBe(Helpers.GetDTOField(dto, "Modification[4]"));
        Helpers.GetSpriggitField(spriggit, "Modification[5]").ShouldBe(Helpers.GetDTOField(dto, "Modification[5]"));
        Helpers.GetSpriggitField(spriggit, "Modification[6]").ShouldBe(Helpers.GetDTOField(dto, "Modification[6]"));
        Helpers.GetSpriggitField(spriggit, "Modification[7]").ShouldBe(Helpers.GetDTOField(dto, "Modification[7]"));
        Helpers.GetSpriggitField(spriggit, "Modification[8]").ShouldBe(Helpers.GetDTOField(dto, "Modification[8]"));
        Helpers.GetSpriggitField(spriggit, "Modification[9]").ShouldBe(Helpers.GetDTOField(dto, "Modification[9]"));
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
        Helpers.GetSpriggitField(spriggit, "PerkConditionTabCount[17]").ShouldBe(Helpers.GetDTOField(dto, "PerkConditionTabCount[17]"));
        Helpers.GetSpriggitField(spriggit, "PerkConditionTabCount[2]").ShouldBe(Helpers.GetDTOField(dto, "PerkConditionTabCount[2]"));
        Helpers.GetSpriggitField(spriggit, "PerkConditionTabCount[3]").ShouldBe(Helpers.GetDTOField(dto, "PerkConditionTabCount[3]"));
        Helpers.GetSpriggitField(spriggit, "PerkConditionTabCount[4]").ShouldBe(Helpers.GetDTOField(dto, "PerkConditionTabCount[4]"));
        Helpers.GetSpriggitField(spriggit, "PerkConditionTabCount[5]").ShouldBe(Helpers.GetDTOField(dto, "PerkConditionTabCount[5]"));
        Helpers.GetSpriggitField(spriggit, "PerkConditionTabCount[6]").ShouldBe(Helpers.GetDTOField(dto, "PerkConditionTabCount[6]"));
        Helpers.GetSpriggitField(spriggit, "PerkConditionTabCount[7]").ShouldBe(Helpers.GetDTOField(dto, "PerkConditionTabCount[7]"));
        Helpers.GetSpriggitField(spriggit, "PerkConditionTabCount[8]").ShouldBe(Helpers.GetDTOField(dto, "PerkConditionTabCount[8]"));
        Helpers.GetSpriggitField(spriggit, "PerkConditionTabCount[9]").ShouldBe(Helpers.GetDTOField(dto, "PerkConditionTabCount[9]"));
        Helpers.GetSpriggitField(spriggit, "Playable").ShouldBe(Helpers.GetDTOField(dto, "Playable"));
        Helpers.GetSpriggitField(spriggit, "Priority[0]").ShouldBe(Helpers.GetDTOField(dto, "Priority[0]"));
        Helpers.GetSpriggitField(spriggit, "Priority[1]").ShouldBe(Helpers.GetDTOField(dto, "Priority[1]"));
        Helpers.GetSpriggitField(spriggit, "Priority[10]").ShouldBe(Helpers.GetDTOField(dto, "Priority[10]"));
        Helpers.GetSpriggitField(spriggit, "Priority[11]").ShouldBe(Helpers.GetDTOField(dto, "Priority[11]"));
        Helpers.GetSpriggitField(spriggit, "Priority[12]").ShouldBe(Helpers.GetDTOField(dto, "Priority[12]"));
        Helpers.GetSpriggitField(spriggit, "Priority[13]").ShouldBe(Helpers.GetDTOField(dto, "Priority[13]"));
        Helpers.GetSpriggitField(spriggit, "Priority[14]").ShouldBe(Helpers.GetDTOField(dto, "Priority[14]"));
        Helpers.GetSpriggitField(spriggit, "Priority[15]").ShouldBe(Helpers.GetDTOField(dto, "Priority[15]"));
        Helpers.GetSpriggitField(spriggit, "Priority[16]").ShouldBe(Helpers.GetDTOField(dto, "Priority[16]"));
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
        Helpers.GetSpriggitField(spriggit, "Value[17]").ShouldBe(Helpers.GetDTOField(dto, "Value[17]"));
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

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "ActorValue[0]", "ActorValue[1]", "ActorValue[10]", "ActorValue[11]", "ActorValue[12]", "ActorValue[13]", "ActorValue[14]", "ActorValue[15]", "ActorValue[16]", "ActorValue[17]", "ActorValue[2]", "ActorValue[3]", "ActorValue[4]", "ActorValue[5]", "ActorValue[6]", "ActorValue[7]", "ActorValue[8]", "ActorValue[9]", "ComparisonValue[0]", "ComparisonValue[1]", "ComparisonValue[10]", "ComparisonValue[11]", "ComparisonValue[2]", "ComparisonValue[3]", "ComparisonValue[4]", "ComparisonValue[5]", "ComparisonValue[6]", "ComparisonValue[7]", "ComparisonValue[8]", "ComparisonValue[9]", "Data.ActorValue[0]", "Data.ActorValue[1]", "Data.ActorValue[2]", "Data.ActorValue[3]", "Data.ActorValue[4]", "Data.Keyword[0]", "Data.Keyword[1]", "Data.Keyword[2]", "Data.Keyword[3]", "Data.Keyword[4]", "Data.Keyword[5]", "Data.Keyword[6]", "Data.MutagenObjectType[0]", "Data.MutagenObjectType[1]", "Data.MutagenObjectType[10]", "Data.MutagenObjectType[11]", "Data.MutagenObjectType[2]", "Data.MutagenObjectType[3]", "Data.MutagenObjectType[4]", "Data.MutagenObjectType[5]", "Data.MutagenObjectType[6]", "Data.MutagenObjectType[7]", "Data.MutagenObjectType[8]", "Data.MutagenObjectType[9]", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "EditorID", "EntryPoint[0]", "EntryPoint[1]", "EntryPoint[10]", "EntryPoint[11]", "EntryPoint[12]", "EntryPoint[13]", "EntryPoint[14]", "EntryPoint[15]", "EntryPoint[16]", "EntryPoint[17]", "EntryPoint[2]", "EntryPoint[3]", "EntryPoint[4]", "EntryPoint[5]", "EntryPoint[6]", "EntryPoint[7]", "EntryPoint[8]", "EntryPoint[9]", "FormKey", "Modification[0]", "Modification[1]", "Modification[10]", "Modification[11]", "Modification[12]", "Modification[13]", "Modification[14]", "Modification[15]", "Modification[16]", "Modification[17]", "Modification[2]", "Modification[3]", "Modification[4]", "Modification[5]", "Modification[6]", "Modification[7]", "Modification[8]", "Modification[9]", "NumRanks", "PerkConditionTabCount[0]", "PerkConditionTabCount[1]", "PerkConditionTabCount[10]", "PerkConditionTabCount[11]", "PerkConditionTabCount[12]", "PerkConditionTabCount[13]", "PerkConditionTabCount[14]", "PerkConditionTabCount[15]", "PerkConditionTabCount[16]", "PerkConditionTabCount[17]", "PerkConditionTabCount[2]", "PerkConditionTabCount[3]", "PerkConditionTabCount[4]", "PerkConditionTabCount[5]", "PerkConditionTabCount[6]", "PerkConditionTabCount[7]", "PerkConditionTabCount[8]", "PerkConditionTabCount[9]", "Playable", "Priority[0]", "Priority[1]", "Priority[10]", "Priority[11]", "Priority[12]", "Priority[13]", "Priority[14]", "Priority[15]", "Priority[16]", "Priority[2]", "Priority[3]", "Priority[4]", "Priority[5]", "Priority[6]", "Priority[7]", "Priority[8]", "Priority[9]", "Value[0]", "Value[1]", "Value[10]", "Value[11]", "Value[12]", "Value[13]", "Value[14]", "Value[15]", "Value[16]", "Value[17]", "Value[2]", "Value[3]", "Value[4]", "Value[5]", "Value[6]", "Value[7]", "Value[8]", "Value[9]", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "ActorValue[0]", "ActorValue[1]", "ActorValue[10]", "ActorValue[11]", "ActorValue[12]", "ActorValue[13]", "ActorValue[14]", "ActorValue[15]", "ActorValue[16]", "ActorValue[17]", "ActorValue[2]", "ActorValue[3]", "ActorValue[4]", "ActorValue[5]", "ActorValue[6]", "ActorValue[7]", "ActorValue[8]", "ActorValue[9]", "ComparisonValue[0]", "ComparisonValue[1]", "ComparisonValue[10]", "ComparisonValue[11]", "ComparisonValue[2]", "ComparisonValue[3]", "ComparisonValue[4]", "ComparisonValue[5]", "ComparisonValue[6]", "ComparisonValue[7]", "ComparisonValue[8]", "ComparisonValue[9]", "Data.ActorValue[0]", "Data.ActorValue[1]", "Data.ActorValue[2]", "Data.ActorValue[3]", "Data.ActorValue[4]", "Data.Keyword[0]", "Data.Keyword[1]", "Data.Keyword[2]", "Data.Keyword[3]", "Data.Keyword[4]", "Data.Keyword[5]", "Data.Keyword[6]", "Data.MutagenObjectType[0]", "Data.MutagenObjectType[1]", "Data.MutagenObjectType[10]", "Data.MutagenObjectType[11]", "Data.MutagenObjectType[2]", "Data.MutagenObjectType[3]", "Data.MutagenObjectType[4]", "Data.MutagenObjectType[5]", "Data.MutagenObjectType[6]", "Data.MutagenObjectType[7]", "Data.MutagenObjectType[8]", "Data.MutagenObjectType[9]", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "EditorID", "EntryPoint[0]", "EntryPoint[1]", "EntryPoint[10]", "EntryPoint[11]", "EntryPoint[12]", "EntryPoint[13]", "EntryPoint[14]", "EntryPoint[15]", "EntryPoint[16]", "EntryPoint[17]", "EntryPoint[2]", "EntryPoint[3]", "EntryPoint[4]", "EntryPoint[5]", "EntryPoint[6]", "EntryPoint[7]", "EntryPoint[8]", "EntryPoint[9]", "FormKey", "Modification[0]", "Modification[1]", "Modification[10]", "Modification[11]", "Modification[12]", "Modification[13]", "Modification[14]", "Modification[15]", "Modification[16]", "Modification[17]", "Modification[2]", "Modification[3]", "Modification[4]", "Modification[5]", "Modification[6]", "Modification[7]", "Modification[8]", "Modification[9]", "NumRanks", "PerkConditionTabCount[0]", "PerkConditionTabCount[1]", "PerkConditionTabCount[10]", "PerkConditionTabCount[11]", "PerkConditionTabCount[12]", "PerkConditionTabCount[13]", "PerkConditionTabCount[14]", "PerkConditionTabCount[15]", "PerkConditionTabCount[16]", "PerkConditionTabCount[17]", "PerkConditionTabCount[2]", "PerkConditionTabCount[3]", "PerkConditionTabCount[4]", "PerkConditionTabCount[5]", "PerkConditionTabCount[6]", "PerkConditionTabCount[7]", "PerkConditionTabCount[8]", "PerkConditionTabCount[9]", "Playable", "Priority[0]", "Priority[1]", "Priority[10]", "Priority[11]", "Priority[12]", "Priority[13]", "Priority[14]", "Priority[15]", "Priority[16]", "Priority[2]", "Priority[3]", "Priority[4]", "Priority[5]", "Priority[6]", "Priority[7]", "Priority[8]", "Priority[9]", "Value[0]", "Value[1]", "Value[10]", "Value[11]", "Value[12]", "Value[13]", "Value[14]", "Value[15]", "Value[16]", "Value[17]", "Value[2]", "Value[3]", "Value[4]", "Value[5]", "Value[6]", "Value[7]", "Value[8]", "Value[9]", "Version2", "VersionControl");
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "PERK")]
    [Trait("FormKey", "0BABE4:Skyrim.esm")]
    [Trait("EditorID", "Armsman00")]
    [Trait("SpriggitFile", "Perks/Armsman00 - 0BABE4_Skyrim.esm.yaml")]
    public void Skyrim_PERK_ShouldMatchSpriggitSample_Armsman00()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Perk,
            "Armsman00");
        var dto = Helpers.GetDTO<PerkDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Perk,
            "0BABE4:Skyrim.esm");

        Helpers.GetSpriggitField(spriggit, "ComparisonValue[0]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[0]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[1]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[1]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[2]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[2]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[3]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[3]"));
        Helpers.GetSpriggitField(spriggit, "Data.Keyword[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.Keyword[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.Keyword[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.Keyword[1]"));
        Helpers.GetSpriggitField(spriggit, "Data.Keyword[2]").ShouldBe(Helpers.GetDTOField(dto, "Data.Keyword[2]"));
        Helpers.GetSpriggitField(spriggit, "Data.Keyword[3]").ShouldBe(Helpers.GetDTOField(dto, "Data.Keyword[3]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[1]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[2]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[2]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[3]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[3]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[4]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[4]"));
        Helpers.GetSpriggitField(spriggit, "Data.Perk").ShouldBe(Helpers.GetDTOField(dto, "Data.Perk"));
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
        Helpers.GetSpriggitField(spriggit, "EntryPoint").ShouldBe(Helpers.GetDTOField(dto, "EntryPoint"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "Modification").ShouldBe(Helpers.GetDTOField(dto, "Modification"));
        Helpers.GetSpriggitField(spriggit, "Name.Count").ShouldBe(Helpers.GetDTOField(dto, "Name.Count"));
        Helpers.GetSpriggitField(spriggit, "Name.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Name.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Name[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[0].String").ShouldBe(Helpers.GetDTOField(dto, "Name[0].String"));
        Helpers.GetSpriggitField(spriggit, "Name[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[1].String").ShouldBe(Helpers.GetDTOField(dto, "Name[1].String"));
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
        Helpers.GetSpriggitField(spriggit, "NextPerk").ShouldBe(Helpers.GetDTOField(dto, "NextPerk"));
        Helpers.GetSpriggitField(spriggit, "NumRanks").ShouldBe(Helpers.GetDTOField(dto, "NumRanks"));
        Helpers.GetSpriggitField(spriggit, "PerkConditionTabCount").ShouldBe(Helpers.GetDTOField(dto, "PerkConditionTabCount"));
        Helpers.GetSpriggitField(spriggit, "Playable").ShouldBe(Helpers.GetDTOField(dto, "Playable"));
        Helpers.GetSpriggitField(spriggit, "Value").ShouldBe(Helpers.GetDTOField(dto, "Value"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "ComparisonValue[0]", "ComparisonValue[1]", "ComparisonValue[2]", "ComparisonValue[3]", "Data.Keyword[0]", "Data.Keyword[1]", "Data.Keyword[2]", "Data.Keyword[3]", "Data.MutagenObjectType[0]", "Data.MutagenObjectType[1]", "Data.MutagenObjectType[2]", "Data.MutagenObjectType[3]", "Data.MutagenObjectType[4]", "Data.Perk", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "EditorID", "EntryPoint", "FormKey", "Modification", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "NextPerk", "NumRanks", "PerkConditionTabCount", "Playable", "Value", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "ComparisonValue[0]", "ComparisonValue[1]", "ComparisonValue[2]", "ComparisonValue[3]", "Data.Keyword[0]", "Data.Keyword[1]", "Data.Keyword[2]", "Data.Keyword[3]", "Data.MutagenObjectType[0]", "Data.MutagenObjectType[1]", "Data.MutagenObjectType[2]", "Data.MutagenObjectType[3]", "Data.MutagenObjectType[4]", "Data.Perk", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "EditorID", "EntryPoint", "FormKey", "Modification", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "NextPerk", "NumRanks", "PerkConditionTabCount", "Playable", "Value", "Version2", "VersionControl");
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "PERK")]
    [Trait("FormKey", "079343:Skyrim.esm")]
    [Trait("EditorID", "Armsman20")]
    [Trait("SpriggitFile", "Perks/Armsman20 - 079343_Skyrim.esm.yaml")]
    public void Skyrim_PERK_ShouldMatchSpriggitSample_Armsman20()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Perk,
            "Armsman20");
        var dto = Helpers.GetDTO<PerkDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Perk,
            "079343:Skyrim.esm");

        Helpers.GetSpriggitField(spriggit, "CompareOperator").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[0]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[0]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[1]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[1]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[2]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[2]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[3]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[3]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[4]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[4]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[5]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[5]"));
        Helpers.GetSpriggitField(spriggit, "Data.ActorValue").ShouldBe(Helpers.GetDTOField(dto, "Data.ActorValue"));
        Helpers.GetSpriggitField(spriggit, "Data.Keyword[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.Keyword[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.Keyword[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.Keyword[1]"));
        Helpers.GetSpriggitField(spriggit, "Data.Keyword[2]").ShouldBe(Helpers.GetDTOField(dto, "Data.Keyword[2]"));
        Helpers.GetSpriggitField(spriggit, "Data.Keyword[3]").ShouldBe(Helpers.GetDTOField(dto, "Data.Keyword[3]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[1]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[2]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[2]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[3]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[3]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[4]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[4]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[5]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[5]"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType[6]").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType[6]"));
        Helpers.GetSpriggitField(spriggit, "Data.Perk[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.Perk[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.Perk[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.Perk[1]"));
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
        Helpers.GetSpriggitField(spriggit, "EntryPoint").ShouldBe(Helpers.GetDTOField(dto, "EntryPoint"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "Modification").ShouldBe(Helpers.GetDTOField(dto, "Modification"));
        Helpers.GetSpriggitField(spriggit, "Name.Count").ShouldBe(Helpers.GetDTOField(dto, "Name.Count"));
        Helpers.GetSpriggitField(spriggit, "Name.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Name.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Name[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[0].String").ShouldBe(Helpers.GetDTOField(dto, "Name[0].String"));
        Helpers.GetSpriggitField(spriggit, "Name[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[1].String").ShouldBe(Helpers.GetDTOField(dto, "Name[1].String"));
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
        Helpers.GetSpriggitField(spriggit, "NextPerk").ShouldBe(Helpers.GetDTOField(dto, "NextPerk"));
        Helpers.GetSpriggitField(spriggit, "NumRanks").ShouldBe(Helpers.GetDTOField(dto, "NumRanks"));
        Helpers.GetSpriggitField(spriggit, "PerkConditionTabCount").ShouldBe(Helpers.GetDTOField(dto, "PerkConditionTabCount"));
        Helpers.GetSpriggitField(spriggit, "Playable").ShouldBe(Helpers.GetDTOField(dto, "Playable"));
        Helpers.GetSpriggitField(spriggit, "Value").ShouldBe(Helpers.GetDTOField(dto, "Value"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "CompareOperator", "ComparisonValue[0]", "ComparisonValue[1]", "ComparisonValue[2]", "ComparisonValue[3]", "ComparisonValue[4]", "ComparisonValue[5]", "Data.ActorValue", "Data.Keyword[0]", "Data.Keyword[1]", "Data.Keyword[2]", "Data.Keyword[3]", "Data.MutagenObjectType[0]", "Data.MutagenObjectType[1]", "Data.MutagenObjectType[2]", "Data.MutagenObjectType[3]", "Data.MutagenObjectType[4]", "Data.MutagenObjectType[5]", "Data.MutagenObjectType[6]", "Data.Perk[0]", "Data.Perk[1]", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "EditorID", "EntryPoint", "FormKey", "Modification", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "NextPerk", "NumRanks", "PerkConditionTabCount", "Playable", "Value", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "CompareOperator", "ComparisonValue[0]", "ComparisonValue[1]", "ComparisonValue[2]", "ComparisonValue[3]", "ComparisonValue[4]", "ComparisonValue[5]", "Data.ActorValue", "Data.Keyword[0]", "Data.Keyword[1]", "Data.Keyword[2]", "Data.Keyword[3]", "Data.MutagenObjectType[0]", "Data.MutagenObjectType[1]", "Data.MutagenObjectType[2]", "Data.MutagenObjectType[3]", "Data.MutagenObjectType[4]", "Data.MutagenObjectType[5]", "Data.MutagenObjectType[6]", "Data.Perk[0]", "Data.Perk[1]", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "EditorID", "EntryPoint", "FormKey", "Modification", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "NextPerk", "NumRanks", "PerkConditionTabCount", "Playable", "Value", "Version2", "VersionControl");
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "PERK")]
    [Trait("FormKey", "058F75:Skyrim.esm")]
    [Trait("EditorID", "Allure")]
    [Trait("SpriggitFile", "Perks/Allure - 058F75_Skyrim.esm.yaml")]
    public void Skyrim_PERK_ShouldMatchSpriggitSample_Allure()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Perk,
            "Allure");
        var dto = Helpers.GetDTO<PerkDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Perk,
            "058F75:Skyrim.esm");

        Helpers.GetSpriggitField(spriggit, "CompareOperator").ShouldBe(Helpers.GetDTOField(dto, "CompareOperator"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[0]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[0]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[1]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[1]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[2]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[2]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[3]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[3]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[4]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[4]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[5]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[5]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[6]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[6]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[7]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[7]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[8]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[8]"));
        Helpers.GetSpriggitField(spriggit, "ComparisonValue[9]").ShouldBe(Helpers.GetDTOField(dto, "ComparisonValue[9]"));
        Helpers.GetSpriggitField(spriggit, "Data.ActorValue").ShouldBe(Helpers.GetDTOField(dto, "Data.ActorValue"));
        Helpers.GetSpriggitField(spriggit, "Data.MaleFemaleGender[0]").ShouldBe(Helpers.GetDTOField(dto, "Data.MaleFemaleGender[0]"));
        Helpers.GetSpriggitField(spriggit, "Data.MaleFemaleGender[1]").ShouldBe(Helpers.GetDTOField(dto, "Data.MaleFemaleGender[1]"));
        Helpers.GetSpriggitField(spriggit, "Data.MaleFemaleGender[2]").ShouldBe(Helpers.GetDTOField(dto, "Data.MaleFemaleGender[2]"));
        Helpers.GetSpriggitField(spriggit, "Data.MaleFemaleGender[3]").ShouldBe(Helpers.GetDTOField(dto, "Data.MaleFemaleGender[3]"));
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
        Helpers.GetSpriggitField(spriggit, "Data.Perk").ShouldBe(Helpers.GetDTOField(dto, "Data.Perk"));
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
        Helpers.GetSpriggitField(spriggit, "EntryPoint[0]").ShouldBe(Helpers.GetDTOField(dto, "EntryPoint[0]"));
        Helpers.GetSpriggitField(spriggit, "EntryPoint[1]").ShouldBe(Helpers.GetDTOField(dto, "EntryPoint[1]"));
        Helpers.GetSpriggitField(spriggit, "EntryPoint[2]").ShouldBe(Helpers.GetDTOField(dto, "EntryPoint[2]"));
        Helpers.GetSpriggitField(spriggit, "EntryPoint[3]").ShouldBe(Helpers.GetDTOField(dto, "EntryPoint[3]"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "Modification[0]").ShouldBe(Helpers.GetDTOField(dto, "Modification[0]"));
        Helpers.GetSpriggitField(spriggit, "Modification[1]").ShouldBe(Helpers.GetDTOField(dto, "Modification[1]"));
        Helpers.GetSpriggitField(spriggit, "Modification[2]").ShouldBe(Helpers.GetDTOField(dto, "Modification[2]"));
        Helpers.GetSpriggitField(spriggit, "Modification[3]").ShouldBe(Helpers.GetDTOField(dto, "Modification[3]"));
        Helpers.GetSpriggitField(spriggit, "Name.Count").ShouldBe(Helpers.GetDTOField(dto, "Name.Count"));
        Helpers.GetSpriggitField(spriggit, "Name.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Name.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Name[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[0].String").ShouldBe(Helpers.GetDTOField(dto, "Name[0].String"));
        Helpers.GetSpriggitField(spriggit, "Name[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Name[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Name[1].String").ShouldBe(Helpers.GetDTOField(dto, "Name[1].String"));
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
        Helpers.GetSpriggitField(spriggit, "NumRanks").ShouldBe(Helpers.GetDTOField(dto, "NumRanks"));
        Helpers.GetSpriggitField(spriggit, "PerkConditionTabCount[0]").ShouldBe(Helpers.GetDTOField(dto, "PerkConditionTabCount[0]"));
        Helpers.GetSpriggitField(spriggit, "PerkConditionTabCount[1]").ShouldBe(Helpers.GetDTOField(dto, "PerkConditionTabCount[1]"));
        Helpers.GetSpriggitField(spriggit, "PerkConditionTabCount[2]").ShouldBe(Helpers.GetDTOField(dto, "PerkConditionTabCount[2]"));
        Helpers.GetSpriggitField(spriggit, "PerkConditionTabCount[3]").ShouldBe(Helpers.GetDTOField(dto, "PerkConditionTabCount[3]"));
        Helpers.GetSpriggitField(spriggit, "Playable").ShouldBe(Helpers.GetDTOField(dto, "Playable"));
        Helpers.GetSpriggitField(spriggit, "Value[0]").ShouldBe(Helpers.GetDTOField(dto, "Value[0]"));
        Helpers.GetSpriggitField(spriggit, "Value[1]").ShouldBe(Helpers.GetDTOField(dto, "Value[1]"));
        Helpers.GetSpriggitField(spriggit, "Value[2]").ShouldBe(Helpers.GetDTOField(dto, "Value[2]"));
        Helpers.GetSpriggitField(spriggit, "Value[3]").ShouldBe(Helpers.GetDTOField(dto, "Value[3]"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "CompareOperator", "ComparisonValue[0]", "ComparisonValue[1]", "ComparisonValue[2]", "ComparisonValue[3]", "ComparisonValue[4]", "ComparisonValue[5]", "ComparisonValue[6]", "ComparisonValue[7]", "ComparisonValue[8]", "ComparisonValue[9]", "Data.ActorValue", "Data.MaleFemaleGender[0]", "Data.MaleFemaleGender[1]", "Data.MaleFemaleGender[2]", "Data.MaleFemaleGender[3]", "Data.MutagenObjectType[0]", "Data.MutagenObjectType[1]", "Data.MutagenObjectType[2]", "Data.MutagenObjectType[3]", "Data.MutagenObjectType[4]", "Data.MutagenObjectType[5]", "Data.MutagenObjectType[6]", "Data.MutagenObjectType[7]", "Data.MutagenObjectType[8]", "Data.MutagenObjectType[9]", "Data.Perk", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "EditorID", "EntryPoint[0]", "EntryPoint[1]", "EntryPoint[2]", "EntryPoint[3]", "FormKey", "Modification[0]", "Modification[1]", "Modification[2]", "Modification[3]", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "NumRanks", "PerkConditionTabCount[0]", "PerkConditionTabCount[1]", "PerkConditionTabCount[2]", "PerkConditionTabCount[3]", "Playable", "Value[0]", "Value[1]", "Value[2]", "Value[3]", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "CompareOperator", "ComparisonValue[0]", "ComparisonValue[1]", "ComparisonValue[2]", "ComparisonValue[3]", "ComparisonValue[4]", "ComparisonValue[5]", "ComparisonValue[6]", "ComparisonValue[7]", "ComparisonValue[8]", "ComparisonValue[9]", "Data.ActorValue", "Data.MaleFemaleGender[0]", "Data.MaleFemaleGender[1]", "Data.MaleFemaleGender[2]", "Data.MaleFemaleGender[3]", "Data.MutagenObjectType[0]", "Data.MutagenObjectType[1]", "Data.MutagenObjectType[2]", "Data.MutagenObjectType[3]", "Data.MutagenObjectType[4]", "Data.MutagenObjectType[5]", "Data.MutagenObjectType[6]", "Data.MutagenObjectType[7]", "Data.MutagenObjectType[8]", "Data.MutagenObjectType[9]", "Data.Perk", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "EditorID", "EntryPoint[0]", "EntryPoint[1]", "EntryPoint[2]", "EntryPoint[3]", "FormKey", "Modification[0]", "Modification[1]", "Modification[2]", "Modification[3]", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "NumRanks", "PerkConditionTabCount[0]", "PerkConditionTabCount[1]", "PerkConditionTabCount[2]", "PerkConditionTabCount[3]", "Playable", "Value[0]", "Value[1]", "Value[2]", "Value[3]", "Version2", "VersionControl");
    }
}