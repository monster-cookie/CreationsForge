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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["CrimeValues.Arrest"].ShouldBe(dtoFields["CrimeValues.Arrest"]);
        spriggitFields["CrimeValues.Assault"].ShouldBe(dtoFields["CrimeValues.Assault"]);
        spriggitFields["CrimeValues.Escape"].ShouldBe(dtoFields["CrimeValues.Escape"]);
        spriggitFields["CrimeValues.Murder"].ShouldBe(dtoFields["CrimeValues.Murder"]);
        spriggitFields["CrimeValues.Pickpocket"].ShouldBe(dtoFields["CrimeValues.Pickpocket"]);
        spriggitFields["CrimeValues.Piracy"].ShouldBe(dtoFields["CrimeValues.Piracy"]);
        spriggitFields["CrimeValues.SmuggleMultiplier"].ShouldBe(dtoFields["CrimeValues.SmuggleMultiplier"]);
        spriggitFields["CrimeValues.StealMultiplier"].ShouldBe(dtoFields["CrimeValues.StealMultiplier"]);
        spriggitFields["CrimeValues.Trespass"].ShouldBe(dtoFields["CrimeValues.Trespass"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["Keyword"].ShouldBe(dtoFields["Keyword"]);
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
        spriggitFields["Reaction[0]"].ShouldBe(dtoFields["Reaction[0]"]);
        spriggitFields["Reaction[1]"].ShouldBe(dtoFields["Reaction[1]"]);
        spriggitFields["Reaction[10]"].ShouldBe(dtoFields["Reaction[10]"]);
        spriggitFields["Reaction[11]"].ShouldBe(dtoFields["Reaction[11]"]);
        spriggitFields["Reaction[12]"].ShouldBe(dtoFields["Reaction[12]"]);
        spriggitFields["Reaction[13]"].ShouldBe(dtoFields["Reaction[13]"]);
        spriggitFields["Reaction[14]"].ShouldBe(dtoFields["Reaction[14]"]);
        spriggitFields["Reaction[2]"].ShouldBe(dtoFields["Reaction[2]"]);
        spriggitFields["Reaction[3]"].ShouldBe(dtoFields["Reaction[3]"]);
        spriggitFields["Reaction[4]"].ShouldBe(dtoFields["Reaction[4]"]);
        spriggitFields["Reaction[5]"].ShouldBe(dtoFields["Reaction[5]"]);
        spriggitFields["Reaction[6]"].ShouldBe(dtoFields["Reaction[6]"]);
        spriggitFields["Reaction[7]"].ShouldBe(dtoFields["Reaction[7]"]);
        spriggitFields["Reaction[8]"].ShouldBe(dtoFields["Reaction[8]"]);
        spriggitFields["Reaction[9]"].ShouldBe(dtoFields["Reaction[9]"]);
        spriggitFields["VendorValues.BuysNonStolenItems"].ShouldBe(dtoFields["VendorValues.BuysNonStolenItems"]);
        spriggitFields["VendorValues.BuysStolenItems"].ShouldBe(dtoFields["VendorValues.BuysStolenItems"]);
        spriggitFields["VendorValues.EndHour"].ShouldBe(dtoFields["VendorValues.EndHour"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["CrimeValues.Arrest"].ShouldBe(dtoFields["CrimeValues.Arrest"]);
        spriggitFields["CrimeValues.AttackOnSight"].ShouldBe(dtoFields["CrimeValues.AttackOnSight"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
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
        spriggitFields["Reaction[0]"].ShouldBe(dtoFields["Reaction[0]"]);
        spriggitFields["Reaction[1]"].ShouldBe(dtoFields["Reaction[1]"]);
        spriggitFields["Reaction[10]"].ShouldBe(dtoFields["Reaction[10]"]);
        spriggitFields["Reaction[11]"].ShouldBe(dtoFields["Reaction[11]"]);
        spriggitFields["Reaction[12]"].ShouldBe(dtoFields["Reaction[12]"]);
        spriggitFields["Reaction[13]"].ShouldBe(dtoFields["Reaction[13]"]);
        spriggitFields["Reaction[14]"].ShouldBe(dtoFields["Reaction[14]"]);
        spriggitFields["Reaction[15]"].ShouldBe(dtoFields["Reaction[15]"]);
        spriggitFields["Reaction[16]"].ShouldBe(dtoFields["Reaction[16]"]);
        spriggitFields["Reaction[17]"].ShouldBe(dtoFields["Reaction[17]"]);
        spriggitFields["Reaction[18]"].ShouldBe(dtoFields["Reaction[18]"]);
        spriggitFields["Reaction[19]"].ShouldBe(dtoFields["Reaction[19]"]);
        spriggitFields["Reaction[2]"].ShouldBe(dtoFields["Reaction[2]"]);
        spriggitFields["Reaction[20]"].ShouldBe(dtoFields["Reaction[20]"]);
        spriggitFields["Reaction[21]"].ShouldBe(dtoFields["Reaction[21]"]);
        spriggitFields["Reaction[22]"].ShouldBe(dtoFields["Reaction[22]"]);
        spriggitFields["Reaction[23]"].ShouldBe(dtoFields["Reaction[23]"]);
        spriggitFields["Reaction[24]"].ShouldBe(dtoFields["Reaction[24]"]);
        spriggitFields["Reaction[25]"].ShouldBe(dtoFields["Reaction[25]"]);
        spriggitFields["Reaction[26]"].ShouldBe(dtoFields["Reaction[26]"]);
        spriggitFields["Reaction[27]"].ShouldBe(dtoFields["Reaction[27]"]);
        spriggitFields["Reaction[28]"].ShouldBe(dtoFields["Reaction[28]"]);
        spriggitFields["Reaction[29]"].ShouldBe(dtoFields["Reaction[29]"]);
        spriggitFields["Reaction[3]"].ShouldBe(dtoFields["Reaction[3]"]);
        spriggitFields["Reaction[30]"].ShouldBe(dtoFields["Reaction[30]"]);
        spriggitFields["Reaction[31]"].ShouldBe(dtoFields["Reaction[31]"]);
        spriggitFields["Reaction[32]"].ShouldBe(dtoFields["Reaction[32]"]);
        spriggitFields["Reaction[33]"].ShouldBe(dtoFields["Reaction[33]"]);
        spriggitFields["Reaction[34]"].ShouldBe(dtoFields["Reaction[34]"]);
        spriggitFields["Reaction[35]"].ShouldBe(dtoFields["Reaction[35]"]);
        spriggitFields["Reaction[36]"].ShouldBe(dtoFields["Reaction[36]"]);
        spriggitFields["Reaction[37]"].ShouldBe(dtoFields["Reaction[37]"]);
        spriggitFields["Reaction[38]"].ShouldBe(dtoFields["Reaction[38]"]);
        spriggitFields["Reaction[39]"].ShouldBe(dtoFields["Reaction[39]"]);
        spriggitFields["Reaction[4]"].ShouldBe(dtoFields["Reaction[4]"]);
        spriggitFields["Reaction[40]"].ShouldBe(dtoFields["Reaction[40]"]);
        spriggitFields["Reaction[41]"].ShouldBe(dtoFields["Reaction[41]"]);
        spriggitFields["Reaction[42]"].ShouldBe(dtoFields["Reaction[42]"]);
        spriggitFields["Reaction[43]"].ShouldBe(dtoFields["Reaction[43]"]);
        spriggitFields["Reaction[44]"].ShouldBe(dtoFields["Reaction[44]"]);
        spriggitFields["Reaction[45]"].ShouldBe(dtoFields["Reaction[45]"]);
        spriggitFields["Reaction[46]"].ShouldBe(dtoFields["Reaction[46]"]);
        spriggitFields["Reaction[47]"].ShouldBe(dtoFields["Reaction[47]"]);
        spriggitFields["Reaction[48]"].ShouldBe(dtoFields["Reaction[48]"]);
        spriggitFields["Reaction[49]"].ShouldBe(dtoFields["Reaction[49]"]);
        spriggitFields["Reaction[5]"].ShouldBe(dtoFields["Reaction[5]"]);
        spriggitFields["Reaction[50]"].ShouldBe(dtoFields["Reaction[50]"]);
        spriggitFields["Reaction[51]"].ShouldBe(dtoFields["Reaction[51]"]);
        spriggitFields["Reaction[52]"].ShouldBe(dtoFields["Reaction[52]"]);
        spriggitFields["Reaction[53]"].ShouldBe(dtoFields["Reaction[53]"]);
        spriggitFields["Reaction[54]"].ShouldBe(dtoFields["Reaction[54]"]);
        spriggitFields["Reaction[55]"].ShouldBe(dtoFields["Reaction[55]"]);
        spriggitFields["Reaction[56]"].ShouldBe(dtoFields["Reaction[56]"]);
        spriggitFields["Reaction[57]"].ShouldBe(dtoFields["Reaction[57]"]);
        spriggitFields["Reaction[58]"].ShouldBe(dtoFields["Reaction[58]"]);
        spriggitFields["Reaction[59]"].ShouldBe(dtoFields["Reaction[59]"]);
        spriggitFields["Reaction[6]"].ShouldBe(dtoFields["Reaction[6]"]);
        spriggitFields["Reaction[60]"].ShouldBe(dtoFields["Reaction[60]"]);
        spriggitFields["Reaction[61]"].ShouldBe(dtoFields["Reaction[61]"]);
        spriggitFields["Reaction[62]"].ShouldBe(dtoFields["Reaction[62]"]);
        spriggitFields["Reaction[63]"].ShouldBe(dtoFields["Reaction[63]"]);
        spriggitFields["Reaction[64]"].ShouldBe(dtoFields["Reaction[64]"]);
        spriggitFields["Reaction[65]"].ShouldBe(dtoFields["Reaction[65]"]);
        spriggitFields["Reaction[66]"].ShouldBe(dtoFields["Reaction[66]"]);
        spriggitFields["Reaction[67]"].ShouldBe(dtoFields["Reaction[67]"]);
        spriggitFields["Reaction[68]"].ShouldBe(dtoFields["Reaction[68]"]);
        spriggitFields["Reaction[69]"].ShouldBe(dtoFields["Reaction[69]"]);
        spriggitFields["Reaction[7]"].ShouldBe(dtoFields["Reaction[7]"]);
        spriggitFields["Reaction[70]"].ShouldBe(dtoFields["Reaction[70]"]);
        spriggitFields["Reaction[8]"].ShouldBe(dtoFields["Reaction[8]"]);
        spriggitFields["Reaction[9]"].ShouldBe(dtoFields["Reaction[9]"]);
        spriggitFields["VendorValues"].ShouldBe(dtoFields["VendorValues"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["CrimeValues.Arrest"].ShouldBe(dtoFields["CrimeValues.Arrest"]);
        spriggitFields["CrimeValues.AttackOnSight"].ShouldBe(dtoFields["CrimeValues.AttackOnSight"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
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
        spriggitFields["Reaction[0]"].ShouldBe(dtoFields["Reaction[0]"]);
        spriggitFields["Reaction[1]"].ShouldBe(dtoFields["Reaction[1]"]);
        spriggitFields["Reaction[10]"].ShouldBe(dtoFields["Reaction[10]"]);
        spriggitFields["Reaction[11]"].ShouldBe(dtoFields["Reaction[11]"]);
        spriggitFields["Reaction[12]"].ShouldBe(dtoFields["Reaction[12]"]);
        spriggitFields["Reaction[13]"].ShouldBe(dtoFields["Reaction[13]"]);
        spriggitFields["Reaction[14]"].ShouldBe(dtoFields["Reaction[14]"]);
        spriggitFields["Reaction[15]"].ShouldBe(dtoFields["Reaction[15]"]);
        spriggitFields["Reaction[16]"].ShouldBe(dtoFields["Reaction[16]"]);
        spriggitFields["Reaction[17]"].ShouldBe(dtoFields["Reaction[17]"]);
        spriggitFields["Reaction[18]"].ShouldBe(dtoFields["Reaction[18]"]);
        spriggitFields["Reaction[19]"].ShouldBe(dtoFields["Reaction[19]"]);
        spriggitFields["Reaction[2]"].ShouldBe(dtoFields["Reaction[2]"]);
        spriggitFields["Reaction[20]"].ShouldBe(dtoFields["Reaction[20]"]);
        spriggitFields["Reaction[21]"].ShouldBe(dtoFields["Reaction[21]"]);
        spriggitFields["Reaction[22]"].ShouldBe(dtoFields["Reaction[22]"]);
        spriggitFields["Reaction[23]"].ShouldBe(dtoFields["Reaction[23]"]);
        spriggitFields["Reaction[24]"].ShouldBe(dtoFields["Reaction[24]"]);
        spriggitFields["Reaction[25]"].ShouldBe(dtoFields["Reaction[25]"]);
        spriggitFields["Reaction[26]"].ShouldBe(dtoFields["Reaction[26]"]);
        spriggitFields["Reaction[27]"].ShouldBe(dtoFields["Reaction[27]"]);
        spriggitFields["Reaction[28]"].ShouldBe(dtoFields["Reaction[28]"]);
        spriggitFields["Reaction[29]"].ShouldBe(dtoFields["Reaction[29]"]);
        spriggitFields["Reaction[3]"].ShouldBe(dtoFields["Reaction[3]"]);
        spriggitFields["Reaction[30]"].ShouldBe(dtoFields["Reaction[30]"]);
        spriggitFields["Reaction[31]"].ShouldBe(dtoFields["Reaction[31]"]);
        spriggitFields["Reaction[32]"].ShouldBe(dtoFields["Reaction[32]"]);
        spriggitFields["Reaction[33]"].ShouldBe(dtoFields["Reaction[33]"]);
        spriggitFields["Reaction[4]"].ShouldBe(dtoFields["Reaction[4]"]);
        spriggitFields["Reaction[5]"].ShouldBe(dtoFields["Reaction[5]"]);
        spriggitFields["Reaction[6]"].ShouldBe(dtoFields["Reaction[6]"]);
        spriggitFields["Reaction[7]"].ShouldBe(dtoFields["Reaction[7]"]);
        spriggitFields["Reaction[8]"].ShouldBe(dtoFields["Reaction[8]"]);
        spriggitFields["Reaction[9]"].ShouldBe(dtoFields["Reaction[9]"]);
        spriggitFields["VendorValues"].ShouldBe(dtoFields["VendorValues"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["CrimeValues.AttackOnSight"].ShouldBe(dtoFields["CrimeValues.AttackOnSight"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
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
        spriggitFields["Reaction[0]"].ShouldBe(dtoFields["Reaction[0]"]);
        spriggitFields["Reaction[1]"].ShouldBe(dtoFields["Reaction[1]"]);
        spriggitFields["VendorBuySellList"].ShouldBe(dtoFields["VendorBuySellListFormKey"]);
        spriggitFields["VendorValues.BuysNonStolenItems"].ShouldBe(dtoFields["VendorValues.BuysNonStolenItems"]);
        spriggitFields["VendorValues.BuysStolenItems"].ShouldBe(dtoFields["VendorValues.BuysStolenItems"]);
        spriggitFields["VendorValues.EndHour"].ShouldBe(dtoFields["VendorValues.EndHour"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["CrimeValues.Arrest"].ShouldBe(dtoFields["CrimeValues.Arrest"]);
        spriggitFields["CrimeValues.AttackOnSight"].ShouldBe(dtoFields["CrimeValues.AttackOnSight"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["MerchantContainer"].ShouldBe(dtoFields["MerchantContainerFormKey"]);
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
        spriggitFields["Unknown2"].ShouldBe(dtoFields["Unknown2"]);
        spriggitFields["Unknown3"].ShouldBe(dtoFields["Unknown3"]);
        spriggitFields["Unknown4"].ShouldBe(dtoFields["Unknown4"]);
        spriggitFields["Unknown5"].ShouldBe(dtoFields["Unknown5"]);
        spriggitFields["VendorValues.BuysNonStolenItems"].ShouldBe(dtoFields["VendorValues.BuysNonStolenItems"]);
        spriggitFields["VendorValues.EndHour"].ShouldBe(dtoFields["VendorValues.EndHour"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }
}
