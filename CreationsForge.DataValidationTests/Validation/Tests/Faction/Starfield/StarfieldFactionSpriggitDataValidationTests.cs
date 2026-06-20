using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.Faction.Starfield;

public class StarfieldFactionSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "FACT")]
    [Trait("FormKey", "010B30:Starfield.esm")]
    [Trait("EditorID", "CrimeFactionCrimsonFleet")]
    [Trait("SpriggitFile", "Factions/CrimeFactionCrimsonFleet - 010B30_Starfield.esm.yaml")]
    public void Starfield_FACT_ShouldMatchSpriggitSample_CrimeFactionCrimsonFleet()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Faction,
            "CrimeFactionCrimsonFleet");
        var dto = Helpers.GetDTO<FactionDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Faction,
            "010B30:Starfield.esm");

        Helpers.GetSpriggitField(spriggit, "CrimeValues.Arrest").ShouldBe(Helpers.GetDTOField(dto, "CrimeValues.Arrest"));
        Helpers.GetSpriggitField(spriggit, "CrimeValues.Assault").ShouldBe(Helpers.GetDTOField(dto, "CrimeValues.Assault"));
        Helpers.GetSpriggitField(spriggit, "CrimeValues.Escape").ShouldBe(Helpers.GetDTOField(dto, "CrimeValues.Escape"));
        Helpers.GetSpriggitField(spriggit, "CrimeValues.Murder").ShouldBe(Helpers.GetDTOField(dto, "CrimeValues.Murder"));
        Helpers.GetSpriggitField(spriggit, "CrimeValues.Pickpocket").ShouldBe(Helpers.GetDTOField(dto, "CrimeValues.Pickpocket"));
        Helpers.GetSpriggitField(spriggit, "CrimeValues.Piracy").ShouldBe(Helpers.GetDTOField(dto, "CrimeValues.Piracy"));
        Helpers.GetSpriggitField(spriggit, "CrimeValues.SmuggleMultiplier").ShouldBe(Helpers.GetDTOField(dto, "CrimeValues.SmuggleMultiplier"));
        Helpers.GetSpriggitField(spriggit, "CrimeValues.StealMultiplier").ShouldBe(Helpers.GetDTOField(dto, "CrimeValues.StealMultiplier"));
        Helpers.GetSpriggitField(spriggit, "CrimeValues.Trespass").ShouldBe(Helpers.GetDTOField(dto, "CrimeValues.Trespass"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "Keyword").ShouldBe(Helpers.GetDTOField(dto, "Keyword"));
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
        Helpers.GetSpriggitField(spriggit, "Reaction[0]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[0]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[1]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[1]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[10]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[10]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[11]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[11]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[12]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[12]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[13]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[13]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[14]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[14]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[2]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[2]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[3]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[3]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[4]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[4]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[5]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[5]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[6]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[6]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[7]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[7]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[8]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[8]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[9]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[9]"));
        Helpers.GetSpriggitField(spriggit, "VendorValues.BuysNonStolenItems").ShouldBe(Helpers.GetDTOField(dto, "VendorValues.BuysNonStolenItems"));
        Helpers.GetSpriggitField(spriggit, "VendorValues.BuysStolenItems").ShouldBe(Helpers.GetDTOField(dto, "VendorValues.BuysStolenItems"));
        Helpers.GetSpriggitField(spriggit, "VendorValues.EndHour").ShouldBe(Helpers.GetDTOField(dto, "VendorValues.EndHour"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "CrimeValues.Arrest", "CrimeValues.Assault", "CrimeValues.Escape", "CrimeValues.Murder", "CrimeValues.Pickpocket", "CrimeValues.Piracy", "CrimeValues.SmuggleMultiplier", "CrimeValues.StealMultiplier", "CrimeValues.Trespass", "EditorID", "FormKey", "FormVersion", "Keyword", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Reaction[0]", "Reaction[1]", "Reaction[10]", "Reaction[11]", "Reaction[12]", "Reaction[13]", "Reaction[14]", "Reaction[2]", "Reaction[3]", "Reaction[4]", "Reaction[5]", "Reaction[6]", "Reaction[7]", "Reaction[8]", "Reaction[9]", "VendorValues.BuysNonStolenItems", "VendorValues.BuysStolenItems", "VendorValues.EndHour", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "CrimeValues.Arrest", "CrimeValues.Assault", "CrimeValues.Escape", "CrimeValues.Murder", "CrimeValues.Pickpocket", "CrimeValues.Piracy", "CrimeValues.SmuggleMultiplier", "CrimeValues.StealMultiplier", "CrimeValues.Trespass", "EditorID", "FormKey", "FormVersion", "Keyword", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Reaction[0]", "Reaction[1]", "Reaction[10]", "Reaction[11]", "Reaction[12]", "Reaction[13]", "Reaction[14]", "Reaction[2]", "Reaction[3]", "Reaction[4]", "Reaction[5]", "Reaction[6]", "Reaction[7]", "Reaction[8]", "Reaction[9]", "VendorValues.BuysNonStolenItems", "VendorValues.BuysStolenItems", "VendorValues.EndHour", "Version2", "VersionControl");
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "FACT")]
    [Trait("FormKey", "03E0C8:Starfield.esm")]
    [Trait("EditorID", "CaptiveFaction")]
    [Trait("SpriggitFile", "Factions/CaptiveFaction - 03E0C8_Starfield.esm.yaml")]
    public void Starfield_FACT_ShouldMatchSpriggitSample_CaptiveFaction()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Faction,
            "CaptiveFaction");
        var dto = Helpers.GetDTO<FactionDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Faction,
            "03E0C8:Starfield.esm");

        Helpers.GetSpriggitField(spriggit, "CrimeValues.Arrest").ShouldBe(Helpers.GetDTOField(dto, "CrimeValues.Arrest"));
        Helpers.GetSpriggitField(spriggit, "CrimeValues.AttackOnSight").ShouldBe(Helpers.GetDTOField(dto, "CrimeValues.AttackOnSight"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
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
        Helpers.GetSpriggitField(spriggit, "Reaction[0]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[0]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[1]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[1]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[10]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[10]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[11]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[11]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[12]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[12]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[13]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[13]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[14]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[14]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[15]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[15]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[16]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[16]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[17]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[17]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[18]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[18]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[19]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[19]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[2]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[2]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[20]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[20]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[21]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[21]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[22]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[22]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[23]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[23]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[24]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[24]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[25]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[25]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[26]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[26]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[27]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[27]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[28]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[28]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[29]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[29]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[3]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[3]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[30]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[30]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[31]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[31]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[32]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[32]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[33]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[33]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[34]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[34]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[35]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[35]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[36]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[36]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[37]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[37]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[38]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[38]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[39]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[39]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[4]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[4]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[40]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[40]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[41]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[41]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[42]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[42]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[43]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[43]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[44]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[44]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[45]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[45]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[46]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[46]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[47]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[47]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[48]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[48]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[49]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[49]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[5]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[5]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[50]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[50]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[51]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[51]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[52]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[52]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[53]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[53]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[54]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[54]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[55]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[55]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[56]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[56]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[57]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[57]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[58]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[58]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[59]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[59]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[6]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[6]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[60]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[60]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[61]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[61]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[62]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[62]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[63]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[63]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[64]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[64]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[65]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[65]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[66]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[66]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[67]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[67]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[68]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[68]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[69]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[69]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[7]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[7]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[70]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[70]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[8]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[8]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[9]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[9]"));
        Helpers.GetSpriggitField(spriggit, "VendorValues").ShouldBe(Helpers.GetDTOField(dto, "VendorValues"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "CrimeValues.Arrest", "CrimeValues.AttackOnSight", "EditorID", "FormKey", "FormVersion", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Reaction[0]", "Reaction[1]", "Reaction[10]", "Reaction[11]", "Reaction[12]", "Reaction[13]", "Reaction[14]", "Reaction[15]", "Reaction[16]", "Reaction[17]", "Reaction[18]", "Reaction[19]", "Reaction[2]", "Reaction[20]", "Reaction[21]", "Reaction[22]", "Reaction[23]", "Reaction[24]", "Reaction[25]", "Reaction[26]", "Reaction[27]", "Reaction[28]", "Reaction[29]", "Reaction[3]", "Reaction[30]", "Reaction[31]", "Reaction[32]", "Reaction[33]", "Reaction[34]", "Reaction[35]", "Reaction[36]", "Reaction[37]", "Reaction[38]", "Reaction[39]", "Reaction[4]", "Reaction[40]", "Reaction[41]", "Reaction[42]", "Reaction[43]", "Reaction[44]", "Reaction[45]", "Reaction[46]", "Reaction[47]", "Reaction[48]", "Reaction[49]", "Reaction[5]", "Reaction[50]", "Reaction[51]", "Reaction[52]", "Reaction[53]", "Reaction[54]", "Reaction[55]", "Reaction[56]", "Reaction[57]", "Reaction[58]", "Reaction[59]", "Reaction[6]", "Reaction[60]", "Reaction[61]", "Reaction[62]", "Reaction[63]", "Reaction[64]", "Reaction[65]", "Reaction[66]", "Reaction[67]", "Reaction[68]", "Reaction[69]", "Reaction[7]", "Reaction[70]", "Reaction[8]", "Reaction[9]", "VendorValues", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "CrimeValues.Arrest", "CrimeValues.AttackOnSight", "EditorID", "FormKey", "FormVersion", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Reaction[0]", "Reaction[1]", "Reaction[10]", "Reaction[11]", "Reaction[12]", "Reaction[13]", "Reaction[14]", "Reaction[15]", "Reaction[16]", "Reaction[17]", "Reaction[18]", "Reaction[19]", "Reaction[2]", "Reaction[20]", "Reaction[21]", "Reaction[22]", "Reaction[23]", "Reaction[24]", "Reaction[25]", "Reaction[26]", "Reaction[27]", "Reaction[28]", "Reaction[29]", "Reaction[3]", "Reaction[30]", "Reaction[31]", "Reaction[32]", "Reaction[33]", "Reaction[34]", "Reaction[35]", "Reaction[36]", "Reaction[37]", "Reaction[38]", "Reaction[39]", "Reaction[4]", "Reaction[40]", "Reaction[41]", "Reaction[42]", "Reaction[43]", "Reaction[44]", "Reaction[45]", "Reaction[46]", "Reaction[47]", "Reaction[48]", "Reaction[49]", "Reaction[5]", "Reaction[50]", "Reaction[51]", "Reaction[52]", "Reaction[53]", "Reaction[54]", "Reaction[55]", "Reaction[56]", "Reaction[57]", "Reaction[58]", "Reaction[59]", "Reaction[6]", "Reaction[60]", "Reaction[61]", "Reaction[62]", "Reaction[63]", "Reaction[64]", "Reaction[65]", "Reaction[66]", "Reaction[67]", "Reaction[68]", "Reaction[69]", "Reaction[7]", "Reaction[70]", "Reaction[8]", "Reaction[9]", "VendorValues", "Version2", "VersionControl");
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "FACT")]
    [Trait("FormKey", "01C21C:Starfield.esm")]
    [Trait("EditorID", "PlayerFaction")]
    [Trait("SpriggitFile", "Factions/PlayerFaction - 01C21C_Starfield.esm.yaml")]
    public void Starfield_FACT_ShouldMatchSpriggitSample_PlayerFaction()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Faction,
            "PlayerFaction");
        var dto = Helpers.GetDTO<FactionDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Faction,
            "01C21C:Starfield.esm");

        Helpers.GetSpriggitField(spriggit, "CrimeValues.Arrest").ShouldBe(Helpers.GetDTOField(dto, "CrimeValues.Arrest"));
        Helpers.GetSpriggitField(spriggit, "CrimeValues.AttackOnSight").ShouldBe(Helpers.GetDTOField(dto, "CrimeValues.AttackOnSight"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
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
        Helpers.GetSpriggitField(spriggit, "Reaction[0]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[0]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[1]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[1]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[10]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[10]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[11]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[11]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[12]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[12]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[13]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[13]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[14]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[14]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[15]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[15]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[16]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[16]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[17]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[17]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[18]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[18]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[19]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[19]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[2]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[2]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[20]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[20]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[21]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[21]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[22]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[22]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[23]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[23]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[24]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[24]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[25]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[25]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[26]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[26]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[27]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[27]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[28]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[28]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[29]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[29]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[3]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[3]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[30]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[30]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[31]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[31]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[32]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[32]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[33]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[33]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[4]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[4]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[5]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[5]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[6]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[6]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[7]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[7]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[8]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[8]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[9]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[9]"));
        Helpers.GetSpriggitField(spriggit, "VendorValues").ShouldBe(Helpers.GetDTOField(dto, "VendorValues"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "CrimeValues.Arrest", "CrimeValues.AttackOnSight", "EditorID", "FormKey", "FormVersion", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Reaction[0]", "Reaction[1]", "Reaction[10]", "Reaction[11]", "Reaction[12]", "Reaction[13]", "Reaction[14]", "Reaction[15]", "Reaction[16]", "Reaction[17]", "Reaction[18]", "Reaction[19]", "Reaction[2]", "Reaction[20]", "Reaction[21]", "Reaction[22]", "Reaction[23]", "Reaction[24]", "Reaction[25]", "Reaction[26]", "Reaction[27]", "Reaction[28]", "Reaction[29]", "Reaction[3]", "Reaction[30]", "Reaction[31]", "Reaction[32]", "Reaction[33]", "Reaction[4]", "Reaction[5]", "Reaction[6]", "Reaction[7]", "Reaction[8]", "Reaction[9]", "VendorValues", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "CrimeValues.Arrest", "CrimeValues.AttackOnSight", "EditorID", "FormKey", "FormVersion", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Reaction[0]", "Reaction[1]", "Reaction[10]", "Reaction[11]", "Reaction[12]", "Reaction[13]", "Reaction[14]", "Reaction[15]", "Reaction[16]", "Reaction[17]", "Reaction[18]", "Reaction[19]", "Reaction[2]", "Reaction[20]", "Reaction[21]", "Reaction[22]", "Reaction[23]", "Reaction[24]", "Reaction[25]", "Reaction[26]", "Reaction[27]", "Reaction[28]", "Reaction[29]", "Reaction[3]", "Reaction[30]", "Reaction[31]", "Reaction[32]", "Reaction[33]", "Reaction[4]", "Reaction[5]", "Reaction[6]", "Reaction[7]", "Reaction[8]", "Reaction[9]", "VendorValues", "Version2", "VersionControl");
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "FACT")]
    [Trait("FormKey", "1A2C9C:Starfield.esm")]
    [Trait("EditorID", "LISTColonistFaction")]
    [Trait("SpriggitFile", "Factions/LISTColonistFaction - 1A2C9C_Starfield.esm.yaml")]
    public void Starfield_FACT_ShouldMatchSpriggitSample_LISTColonistFaction()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Faction,
            "LISTColonistFaction");
        var dto = Helpers.GetDTO<FactionDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Faction,
            "1A2C9C:Starfield.esm");

        Helpers.GetSpriggitField(spriggit, "CrimeValues.AttackOnSight").ShouldBe(Helpers.GetDTOField(dto, "CrimeValues.AttackOnSight"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
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
        Helpers.GetSpriggitField(spriggit, "Reaction[0]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[0]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[1]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[1]"));
        Helpers.GetSpriggitField(spriggit, "VendorBuySellList").ShouldBe(Helpers.GetDTOField(dto, "VendorBuySellListFormKey"));
        Helpers.GetSpriggitField(spriggit, "VendorValues.BuysNonStolenItems").ShouldBe(Helpers.GetDTOField(dto, "VendorValues.BuysNonStolenItems"));
        Helpers.GetSpriggitField(spriggit, "VendorValues.BuysStolenItems").ShouldBe(Helpers.GetDTOField(dto, "VendorValues.BuysStolenItems"));
        Helpers.GetSpriggitField(spriggit, "VendorValues.EndHour").ShouldBe(Helpers.GetDTOField(dto, "VendorValues.EndHour"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "CrimeValues.AttackOnSight", "EditorID", "FormKey", "FormVersion", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Reaction[0]", "Reaction[1]", "VendorBuySellList", "VendorValues.BuysNonStolenItems", "VendorValues.BuysStolenItems", "VendorValues.EndHour", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "CrimeValues.AttackOnSight", "EditorID", "FormKey", "FormVersion", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Reaction[0]", "Reaction[1]", "VendorBuySellListFormKey", "VendorValues.BuysNonStolenItems", "VendorValues.BuysStolenItems", "VendorValues.EndHour", "Version2", "VersionControl");
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "FACT")]
    [Trait("FormKey", "3CAFBA:Starfield.esm")]
    [Trait("EditorID", "Vendor_ShipServices_AkilaCityFaction")]
    [Trait("SpriggitFile", "Factions/Vendor_ShipServices_AkilaCityFaction - 3CAFBA_Starfield.esm.yaml")]
    public void Starfield_FACT_ShouldMatchSpriggitSample_Vendor_ShipServices_AkilaCityFaction()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Faction,
            "Vendor_ShipServices_AkilaCityFaction");
        var dto = Helpers.GetDTO<FactionDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Faction,
            "3CAFBA:Starfield.esm");

        Helpers.GetSpriggitField(spriggit, "CrimeValues.Arrest").ShouldBe(Helpers.GetDTOField(dto, "CrimeValues.Arrest"));
        Helpers.GetSpriggitField(spriggit, "CrimeValues.AttackOnSight").ShouldBe(Helpers.GetDTOField(dto, "CrimeValues.AttackOnSight"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "MerchantContainer").ShouldBe(Helpers.GetDTOField(dto, "MerchantContainerFormKey"));
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
        Helpers.GetSpriggitField(spriggit, "Unknown2").ShouldBe(Helpers.GetDTOField(dto, "Unknown2"));
        Helpers.GetSpriggitField(spriggit, "Unknown3").ShouldBe(Helpers.GetDTOField(dto, "Unknown3"));
        Helpers.GetSpriggitField(spriggit, "Unknown4").ShouldBe(Helpers.GetDTOField(dto, "Unknown4"));
        Helpers.GetSpriggitField(spriggit, "Unknown5").ShouldBe(Helpers.GetDTOField(dto, "Unknown5"));
        Helpers.GetSpriggitField(spriggit, "VendorValues.BuysNonStolenItems").ShouldBe(Helpers.GetDTOField(dto, "VendorValues.BuysNonStolenItems"));
        Helpers.GetSpriggitField(spriggit, "VendorValues.EndHour").ShouldBe(Helpers.GetDTOField(dto, "VendorValues.EndHour"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "CrimeValues.Arrest", "CrimeValues.AttackOnSight", "EditorID", "FormKey", "FormVersion", "MerchantContainer", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Unknown2", "Unknown3", "Unknown4", "Unknown5", "VendorValues.BuysNonStolenItems", "VendorValues.EndHour", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "CrimeValues.Arrest", "CrimeValues.AttackOnSight", "EditorID", "FormKey", "FormVersion", "MerchantContainerFormKey", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Unknown2", "Unknown3", "Unknown4", "Unknown5", "VendorValues.BuysNonStolenItems", "VendorValues.EndHour", "Version2", "VersionControl");
    }
}