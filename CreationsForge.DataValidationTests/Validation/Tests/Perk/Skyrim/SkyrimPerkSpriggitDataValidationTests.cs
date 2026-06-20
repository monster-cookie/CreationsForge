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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["ActorValue[0]"].ShouldBe(dtoFields["ActorValue[0]"]);
        spriggitFields["ActorValue[1]"].ShouldBe(dtoFields["ActorValue[1]"]);
        spriggitFields["ActorValue[10]"].ShouldBe(dtoFields["ActorValue[10]"]);
        spriggitFields["ActorValue[11]"].ShouldBe(dtoFields["ActorValue[11]"]);
        spriggitFields["ActorValue[12]"].ShouldBe(dtoFields["ActorValue[12]"]);
        spriggitFields["ActorValue[13]"].ShouldBe(dtoFields["ActorValue[13]"]);
        spriggitFields["ActorValue[14]"].ShouldBe(dtoFields["ActorValue[14]"]);
        spriggitFields["ActorValue[15]"].ShouldBe(dtoFields["ActorValue[15]"]);
        spriggitFields["ActorValue[16]"].ShouldBe(dtoFields["ActorValue[16]"]);
        spriggitFields["ActorValue[17]"].ShouldBe(dtoFields["ActorValue[17]"]);
        spriggitFields["ActorValue[2]"].ShouldBe(dtoFields["ActorValue[2]"]);
        spriggitFields["ActorValue[3]"].ShouldBe(dtoFields["ActorValue[3]"]);
        spriggitFields["ActorValue[4]"].ShouldBe(dtoFields["ActorValue[4]"]);
        spriggitFields["ActorValue[5]"].ShouldBe(dtoFields["ActorValue[5]"]);
        spriggitFields["ActorValue[6]"].ShouldBe(dtoFields["ActorValue[6]"]);
        spriggitFields["ActorValue[7]"].ShouldBe(dtoFields["ActorValue[7]"]);
        spriggitFields["ActorValue[8]"].ShouldBe(dtoFields["ActorValue[8]"]);
        spriggitFields["ActorValue[9]"].ShouldBe(dtoFields["ActorValue[9]"]);
        spriggitFields["ComparisonValue[0]"].ShouldBe(dtoFields["ComparisonValue[0]"]);
        spriggitFields["ComparisonValue[1]"].ShouldBe(dtoFields["ComparisonValue[1]"]);
        spriggitFields["ComparisonValue[10]"].ShouldBe(dtoFields["ComparisonValue[10]"]);
        spriggitFields["ComparisonValue[11]"].ShouldBe(dtoFields["ComparisonValue[11]"]);
        spriggitFields["ComparisonValue[2]"].ShouldBe(dtoFields["ComparisonValue[2]"]);
        spriggitFields["ComparisonValue[3]"].ShouldBe(dtoFields["ComparisonValue[3]"]);
        spriggitFields["ComparisonValue[4]"].ShouldBe(dtoFields["ComparisonValue[4]"]);
        spriggitFields["ComparisonValue[5]"].ShouldBe(dtoFields["ComparisonValue[5]"]);
        spriggitFields["ComparisonValue[6]"].ShouldBe(dtoFields["ComparisonValue[6]"]);
        spriggitFields["ComparisonValue[7]"].ShouldBe(dtoFields["ComparisonValue[7]"]);
        spriggitFields["ComparisonValue[8]"].ShouldBe(dtoFields["ComparisonValue[8]"]);
        spriggitFields["ComparisonValue[9]"].ShouldBe(dtoFields["ComparisonValue[9]"]);
        spriggitFields["Data.ActorValue[0]"].ShouldBe(dtoFields["Data.ActorValue[0]"]);
        spriggitFields["Data.ActorValue[1]"].ShouldBe(dtoFields["Data.ActorValue[1]"]);
        spriggitFields["Data.ActorValue[2]"].ShouldBe(dtoFields["Data.ActorValue[2]"]);
        spriggitFields["Data.ActorValue[3]"].ShouldBe(dtoFields["Data.ActorValue[3]"]);
        spriggitFields["Data.ActorValue[4]"].ShouldBe(dtoFields["Data.ActorValue[4]"]);
        spriggitFields["Data.Keyword[0]"].ShouldBe(dtoFields["Data.Keyword[0]"]);
        spriggitFields["Data.Keyword[1]"].ShouldBe(dtoFields["Data.Keyword[1]"]);
        spriggitFields["Data.Keyword[2]"].ShouldBe(dtoFields["Data.Keyword[2]"]);
        spriggitFields["Data.Keyword[3]"].ShouldBe(dtoFields["Data.Keyword[3]"]);
        spriggitFields["Data.Keyword[4]"].ShouldBe(dtoFields["Data.Keyword[4]"]);
        spriggitFields["Data.Keyword[5]"].ShouldBe(dtoFields["Data.Keyword[5]"]);
        spriggitFields["Data.Keyword[6]"].ShouldBe(dtoFields["Data.Keyword[6]"]);
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
        spriggitFields["EntryPoint[0]"].ShouldBe(dtoFields["EntryPoint[0]"]);
        spriggitFields["EntryPoint[1]"].ShouldBe(dtoFields["EntryPoint[1]"]);
        spriggitFields["EntryPoint[10]"].ShouldBe(dtoFields["EntryPoint[10]"]);
        spriggitFields["EntryPoint[11]"].ShouldBe(dtoFields["EntryPoint[11]"]);
        spriggitFields["EntryPoint[12]"].ShouldBe(dtoFields["EntryPoint[12]"]);
        spriggitFields["EntryPoint[13]"].ShouldBe(dtoFields["EntryPoint[13]"]);
        spriggitFields["EntryPoint[14]"].ShouldBe(dtoFields["EntryPoint[14]"]);
        spriggitFields["EntryPoint[15]"].ShouldBe(dtoFields["EntryPoint[15]"]);
        spriggitFields["EntryPoint[16]"].ShouldBe(dtoFields["EntryPoint[16]"]);
        spriggitFields["EntryPoint[17]"].ShouldBe(dtoFields["EntryPoint[17]"]);
        spriggitFields["EntryPoint[2]"].ShouldBe(dtoFields["EntryPoint[2]"]);
        spriggitFields["EntryPoint[3]"].ShouldBe(dtoFields["EntryPoint[3]"]);
        spriggitFields["EntryPoint[4]"].ShouldBe(dtoFields["EntryPoint[4]"]);
        spriggitFields["EntryPoint[5]"].ShouldBe(dtoFields["EntryPoint[5]"]);
        spriggitFields["EntryPoint[6]"].ShouldBe(dtoFields["EntryPoint[6]"]);
        spriggitFields["EntryPoint[7]"].ShouldBe(dtoFields["EntryPoint[7]"]);
        spriggitFields["EntryPoint[8]"].ShouldBe(dtoFields["EntryPoint[8]"]);
        spriggitFields["EntryPoint[9]"].ShouldBe(dtoFields["EntryPoint[9]"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["Modification[0]"].ShouldBe(dtoFields["Modification[0]"]);
        spriggitFields["Modification[1]"].ShouldBe(dtoFields["Modification[1]"]);
        spriggitFields["Modification[10]"].ShouldBe(dtoFields["Modification[10]"]);
        spriggitFields["Modification[11]"].ShouldBe(dtoFields["Modification[11]"]);
        spriggitFields["Modification[12]"].ShouldBe(dtoFields["Modification[12]"]);
        spriggitFields["Modification[13]"].ShouldBe(dtoFields["Modification[13]"]);
        spriggitFields["Modification[14]"].ShouldBe(dtoFields["Modification[14]"]);
        spriggitFields["Modification[15]"].ShouldBe(dtoFields["Modification[15]"]);
        spriggitFields["Modification[16]"].ShouldBe(dtoFields["Modification[16]"]);
        spriggitFields["Modification[17]"].ShouldBe(dtoFields["Modification[17]"]);
        spriggitFields["Modification[2]"].ShouldBe(dtoFields["Modification[2]"]);
        spriggitFields["Modification[3]"].ShouldBe(dtoFields["Modification[3]"]);
        spriggitFields["Modification[4]"].ShouldBe(dtoFields["Modification[4]"]);
        spriggitFields["Modification[5]"].ShouldBe(dtoFields["Modification[5]"]);
        spriggitFields["Modification[6]"].ShouldBe(dtoFields["Modification[6]"]);
        spriggitFields["Modification[7]"].ShouldBe(dtoFields["Modification[7]"]);
        spriggitFields["Modification[8]"].ShouldBe(dtoFields["Modification[8]"]);
        spriggitFields["Modification[9]"].ShouldBe(dtoFields["Modification[9]"]);
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
        spriggitFields["PerkConditionTabCount[17]"].ShouldBe(dtoFields["PerkConditionTabCount[17]"]);
        spriggitFields["PerkConditionTabCount[2]"].ShouldBe(dtoFields["PerkConditionTabCount[2]"]);
        spriggitFields["PerkConditionTabCount[3]"].ShouldBe(dtoFields["PerkConditionTabCount[3]"]);
        spriggitFields["PerkConditionTabCount[4]"].ShouldBe(dtoFields["PerkConditionTabCount[4]"]);
        spriggitFields["PerkConditionTabCount[5]"].ShouldBe(dtoFields["PerkConditionTabCount[5]"]);
        spriggitFields["PerkConditionTabCount[6]"].ShouldBe(dtoFields["PerkConditionTabCount[6]"]);
        spriggitFields["PerkConditionTabCount[7]"].ShouldBe(dtoFields["PerkConditionTabCount[7]"]);
        spriggitFields["PerkConditionTabCount[8]"].ShouldBe(dtoFields["PerkConditionTabCount[8]"]);
        spriggitFields["PerkConditionTabCount[9]"].ShouldBe(dtoFields["PerkConditionTabCount[9]"]);
        spriggitFields["Playable"].ShouldBe(dtoFields["Playable"]);
        spriggitFields["Priority[0]"].ShouldBe(dtoFields["Priority[0]"]);
        spriggitFields["Priority[1]"].ShouldBe(dtoFields["Priority[1]"]);
        spriggitFields["Priority[10]"].ShouldBe(dtoFields["Priority[10]"]);
        spriggitFields["Priority[11]"].ShouldBe(dtoFields["Priority[11]"]);
        spriggitFields["Priority[12]"].ShouldBe(dtoFields["Priority[12]"]);
        spriggitFields["Priority[13]"].ShouldBe(dtoFields["Priority[13]"]);
        spriggitFields["Priority[14]"].ShouldBe(dtoFields["Priority[14]"]);
        spriggitFields["Priority[15]"].ShouldBe(dtoFields["Priority[15]"]);
        spriggitFields["Priority[16]"].ShouldBe(dtoFields["Priority[16]"]);
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
        spriggitFields["Value[17]"].ShouldBe(dtoFields["Value[17]"]);
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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["ComparisonValue[0]"].ShouldBe(dtoFields["ComparisonValue[0]"]);
        spriggitFields["ComparisonValue[1]"].ShouldBe(dtoFields["ComparisonValue[1]"]);
        spriggitFields["ComparisonValue[2]"].ShouldBe(dtoFields["ComparisonValue[2]"]);
        spriggitFields["ComparisonValue[3]"].ShouldBe(dtoFields["ComparisonValue[3]"]);
        spriggitFields["Data.Keyword[0]"].ShouldBe(dtoFields["Data.Keyword[0]"]);
        spriggitFields["Data.Keyword[1]"].ShouldBe(dtoFields["Data.Keyword[1]"]);
        spriggitFields["Data.Keyword[2]"].ShouldBe(dtoFields["Data.Keyword[2]"]);
        spriggitFields["Data.Keyword[3]"].ShouldBe(dtoFields["Data.Keyword[3]"]);
        spriggitFields["Data.MutagenObjectType[0]"].ShouldBe(dtoFields["Data.MutagenObjectType[0]"]);
        spriggitFields["Data.MutagenObjectType[1]"].ShouldBe(dtoFields["Data.MutagenObjectType[1]"]);
        spriggitFields["Data.MutagenObjectType[2]"].ShouldBe(dtoFields["Data.MutagenObjectType[2]"]);
        spriggitFields["Data.MutagenObjectType[3]"].ShouldBe(dtoFields["Data.MutagenObjectType[3]"]);
        spriggitFields["Data.MutagenObjectType[4]"].ShouldBe(dtoFields["Data.MutagenObjectType[4]"]);
        spriggitFields["Data.Perk"].ShouldBe(dtoFields["Data.Perk"]);
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
        spriggitFields["EntryPoint"].ShouldBe(dtoFields["EntryPoint"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["Modification"].ShouldBe(dtoFields["Modification"]);
        spriggitFields["Name.Count"].ShouldBe(dtoFields["Name.Count"]);
        spriggitFields["Name.TargetLanguage"].ShouldBe(dtoFields["Name.TargetLanguage"]);
        spriggitFields["Name[0].Language"].ShouldBe(dtoFields["Name[0].Language"]);
        spriggitFields["Name[0].String"].ShouldBe(dtoFields["Name[0].String"]);
        spriggitFields["Name[1].Language"].ShouldBe(dtoFields["Name[1].Language"]);
        spriggitFields["Name[1].String"].ShouldBe(dtoFields["Name[1].String"]);
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
        spriggitFields["NextPerk"].ShouldBe(dtoFields["NextPerk"]);
        spriggitFields["NumRanks"].ShouldBe(dtoFields["NumRanks"]);
        spriggitFields["PerkConditionTabCount"].ShouldBe(dtoFields["PerkConditionTabCount"]);
        spriggitFields["Playable"].ShouldBe(dtoFields["Playable"]);
        spriggitFields["Value"].ShouldBe(dtoFields["Value"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["CompareOperator"].ShouldBe(dtoFields["CompareOperator"]);
        spriggitFields["ComparisonValue[0]"].ShouldBe(dtoFields["ComparisonValue[0]"]);
        spriggitFields["ComparisonValue[1]"].ShouldBe(dtoFields["ComparisonValue[1]"]);
        spriggitFields["ComparisonValue[2]"].ShouldBe(dtoFields["ComparisonValue[2]"]);
        spriggitFields["ComparisonValue[3]"].ShouldBe(dtoFields["ComparisonValue[3]"]);
        spriggitFields["ComparisonValue[4]"].ShouldBe(dtoFields["ComparisonValue[4]"]);
        spriggitFields["ComparisonValue[5]"].ShouldBe(dtoFields["ComparisonValue[5]"]);
        spriggitFields["Data.ActorValue"].ShouldBe(dtoFields["Data.ActorValue"]);
        spriggitFields["Data.Keyword[0]"].ShouldBe(dtoFields["Data.Keyword[0]"]);
        spriggitFields["Data.Keyword[1]"].ShouldBe(dtoFields["Data.Keyword[1]"]);
        spriggitFields["Data.Keyword[2]"].ShouldBe(dtoFields["Data.Keyword[2]"]);
        spriggitFields["Data.Keyword[3]"].ShouldBe(dtoFields["Data.Keyword[3]"]);
        spriggitFields["Data.MutagenObjectType[0]"].ShouldBe(dtoFields["Data.MutagenObjectType[0]"]);
        spriggitFields["Data.MutagenObjectType[1]"].ShouldBe(dtoFields["Data.MutagenObjectType[1]"]);
        spriggitFields["Data.MutagenObjectType[2]"].ShouldBe(dtoFields["Data.MutagenObjectType[2]"]);
        spriggitFields["Data.MutagenObjectType[3]"].ShouldBe(dtoFields["Data.MutagenObjectType[3]"]);
        spriggitFields["Data.MutagenObjectType[4]"].ShouldBe(dtoFields["Data.MutagenObjectType[4]"]);
        spriggitFields["Data.MutagenObjectType[5]"].ShouldBe(dtoFields["Data.MutagenObjectType[5]"]);
        spriggitFields["Data.MutagenObjectType[6]"].ShouldBe(dtoFields["Data.MutagenObjectType[6]"]);
        spriggitFields["Data.Perk[0]"].ShouldBe(dtoFields["Data.Perk[0]"]);
        spriggitFields["Data.Perk[1]"].ShouldBe(dtoFields["Data.Perk[1]"]);
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
        spriggitFields["EntryPoint"].ShouldBe(dtoFields["EntryPoint"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["Modification"].ShouldBe(dtoFields["Modification"]);
        spriggitFields["Name.Count"].ShouldBe(dtoFields["Name.Count"]);
        spriggitFields["Name.TargetLanguage"].ShouldBe(dtoFields["Name.TargetLanguage"]);
        spriggitFields["Name[0].Language"].ShouldBe(dtoFields["Name[0].Language"]);
        spriggitFields["Name[0].String"].ShouldBe(dtoFields["Name[0].String"]);
        spriggitFields["Name[1].Language"].ShouldBe(dtoFields["Name[1].Language"]);
        spriggitFields["Name[1].String"].ShouldBe(dtoFields["Name[1].String"]);
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
        spriggitFields["NextPerk"].ShouldBe(dtoFields["NextPerk"]);
        spriggitFields["NumRanks"].ShouldBe(dtoFields["NumRanks"]);
        spriggitFields["PerkConditionTabCount"].ShouldBe(dtoFields["PerkConditionTabCount"]);
        spriggitFields["Playable"].ShouldBe(dtoFields["Playable"]);
        spriggitFields["Value"].ShouldBe(dtoFields["Value"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["CompareOperator"].ShouldBe(dtoFields["CompareOperator"]);
        spriggitFields["ComparisonValue[0]"].ShouldBe(dtoFields["ComparisonValue[0]"]);
        spriggitFields["ComparisonValue[1]"].ShouldBe(dtoFields["ComparisonValue[1]"]);
        spriggitFields["ComparisonValue[2]"].ShouldBe(dtoFields["ComparisonValue[2]"]);
        spriggitFields["ComparisonValue[3]"].ShouldBe(dtoFields["ComparisonValue[3]"]);
        spriggitFields["ComparisonValue[4]"].ShouldBe(dtoFields["ComparisonValue[4]"]);
        spriggitFields["ComparisonValue[5]"].ShouldBe(dtoFields["ComparisonValue[5]"]);
        spriggitFields["ComparisonValue[6]"].ShouldBe(dtoFields["ComparisonValue[6]"]);
        spriggitFields["ComparisonValue[7]"].ShouldBe(dtoFields["ComparisonValue[7]"]);
        spriggitFields["ComparisonValue[8]"].ShouldBe(dtoFields["ComparisonValue[8]"]);
        spriggitFields["ComparisonValue[9]"].ShouldBe(dtoFields["ComparisonValue[9]"]);
        spriggitFields["Data.ActorValue"].ShouldBe(dtoFields["Data.ActorValue"]);
        spriggitFields["Data.MaleFemaleGender[0]"].ShouldBe(dtoFields["Data.MaleFemaleGender[0]"]);
        spriggitFields["Data.MaleFemaleGender[1]"].ShouldBe(dtoFields["Data.MaleFemaleGender[1]"]);
        spriggitFields["Data.MaleFemaleGender[2]"].ShouldBe(dtoFields["Data.MaleFemaleGender[2]"]);
        spriggitFields["Data.MaleFemaleGender[3]"].ShouldBe(dtoFields["Data.MaleFemaleGender[3]"]);
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
        spriggitFields["Data.Perk"].ShouldBe(dtoFields["Data.Perk"]);
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
        spriggitFields["EntryPoint[0]"].ShouldBe(dtoFields["EntryPoint[0]"]);
        spriggitFields["EntryPoint[1]"].ShouldBe(dtoFields["EntryPoint[1]"]);
        spriggitFields["EntryPoint[2]"].ShouldBe(dtoFields["EntryPoint[2]"]);
        spriggitFields["EntryPoint[3]"].ShouldBe(dtoFields["EntryPoint[3]"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["Modification[0]"].ShouldBe(dtoFields["Modification[0]"]);
        spriggitFields["Modification[1]"].ShouldBe(dtoFields["Modification[1]"]);
        spriggitFields["Modification[2]"].ShouldBe(dtoFields["Modification[2]"]);
        spriggitFields["Modification[3]"].ShouldBe(dtoFields["Modification[3]"]);
        spriggitFields["Name.Count"].ShouldBe(dtoFields["Name.Count"]);
        spriggitFields["Name.TargetLanguage"].ShouldBe(dtoFields["Name.TargetLanguage"]);
        spriggitFields["Name[0].Language"].ShouldBe(dtoFields["Name[0].Language"]);
        spriggitFields["Name[0].String"].ShouldBe(dtoFields["Name[0].String"]);
        spriggitFields["Name[1].Language"].ShouldBe(dtoFields["Name[1].Language"]);
        spriggitFields["Name[1].String"].ShouldBe(dtoFields["Name[1].String"]);
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
        spriggitFields["NumRanks"].ShouldBe(dtoFields["NumRanks"]);
        spriggitFields["PerkConditionTabCount[0]"].ShouldBe(dtoFields["PerkConditionTabCount[0]"]);
        spriggitFields["PerkConditionTabCount[1]"].ShouldBe(dtoFields["PerkConditionTabCount[1]"]);
        spriggitFields["PerkConditionTabCount[2]"].ShouldBe(dtoFields["PerkConditionTabCount[2]"]);
        spriggitFields["PerkConditionTabCount[3]"].ShouldBe(dtoFields["PerkConditionTabCount[3]"]);
        spriggitFields["Playable"].ShouldBe(dtoFields["Playable"]);
        spriggitFields["Value[0]"].ShouldBe(dtoFields["Value[0]"]);
        spriggitFields["Value[1]"].ShouldBe(dtoFields["Value[1]"]);
        spriggitFields["Value[2]"].ShouldBe(dtoFields["Value[2]"]);
        spriggitFields["Value[3]"].ShouldBe(dtoFields["Value[3]"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }
}
