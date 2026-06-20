using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.Faction.Fallout4;

public class Fallout4FactionSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "FACT")]
    [Trait("FormKey", "0975FC:Fallout4.esm")]
    [Trait("EditorID", "DNFinancial_OpalVendorFaction")]
    [Trait("SpriggitFile", "Factions/DNFinancial_OpalVendorFaction - 0975FC_Fallout4.esm.yaml")]
    public void Fallout4_FACT_ShouldMatchSpriggitSample_DNFinancial_OpalVendorFaction()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Faction,
            "DNFinancial_OpalVendorFaction");
        var dto = Helpers.GetDTO<FactionDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Faction,
            "0975FC:Fallout4.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["CrimeValues.Arrest"].ShouldBe(dtoFields["CrimeValues.Arrest"]);
        spriggitFields["CrimeValues.AttackOnSight"].ShouldBe(dtoFields["CrimeValues.AttackOnSight"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["MerchantContainer"].ShouldBe(dtoFields["MerchantContainerFormKey"]);
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
        spriggitFields["Reaction"].ShouldBe(dtoFields["Reaction"]);
        spriggitFields["VendorBuySellList"].ShouldBe(dtoFields["VendorBuySellListFormKey"]);
        spriggitFields["VendorValues.BuySellEverythingNotInList"].ShouldBe(dtoFields["VendorValues.BuySellEverythingNotInList"]);
        spriggitFields["VendorValues.BuysNonStolenItems"].ShouldBe(dtoFields["VendorValues.BuysNonStolenItems"]);
        spriggitFields["VendorValues.BuysStolenItems"].ShouldBe(dtoFields["VendorValues.BuysStolenItems"]);
        spriggitFields["VendorValues.EndHour"].ShouldBe(dtoFields["VendorValues.EndHour"]);
        spriggitFields["VendorValues.Radius"].ShouldBe(dtoFields["VendorValues.Radius"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "FACT")]
    [Trait("FormKey", "03E0C8:Fallout4.esm")]
    [Trait("EditorID", "CaptiveFaction")]
    [Trait("SpriggitFile", "Factions/CaptiveFaction - 03E0C8_Fallout4.esm.yaml")]
    public void Fallout4_FACT_ShouldMatchSpriggitSample_CaptiveFaction()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Faction,
            "CaptiveFaction");
        var dto = Helpers.GetDTO<FactionDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Faction,
            "03E0C8:Fallout4.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["CrimeValues.Arrest"].ShouldBe(dtoFields["CrimeValues.Arrest"]);
        spriggitFields["CrimeValues.AttackOnSight"].ShouldBe(dtoFields["CrimeValues.AttackOnSight"]);
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
        spriggitFields["Reaction[3]"].ShouldBe(dtoFields["Reaction[3]"]);
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
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "FACT")]
    [Trait("FormKey", "01C21C:Fallout4.esm")]
    [Trait("EditorID", "PlayerFaction")]
    [Trait("SpriggitFile", "Factions/PlayerFaction - 01C21C_Fallout4.esm.yaml")]
    public void Fallout4_FACT_ShouldMatchSpriggitSample_PlayerFaction()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Faction,
            "PlayerFaction");
        var dto = Helpers.GetDTO<FactionDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Faction,
            "01C21C:Fallout4.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["CrimeValues.Arrest"].ShouldBe(dtoFields["CrimeValues.Arrest"]);
        spriggitFields["CrimeValues.AttackOnSight"].ShouldBe(dtoFields["CrimeValues.AttackOnSight"]);
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
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "FACT")]
    [Trait("FormKey", "157ACE:Fallout4.esm")]
    [Trait("EditorID", "DN049BakeryClerkFaction")]
    [Trait("SpriggitFile", "Factions/DN049BakeryClerkFaction - 157ACE_Fallout4.esm.yaml")]
    public void Fallout4_FACT_ShouldMatchSpriggitSample_DN049BakeryClerkFaction()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Faction,
            "DN049BakeryClerkFaction");
        var dto = Helpers.GetDTO<FactionDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Faction,
            "157ACE:Fallout4.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["CrimeValues.Arrest"].ShouldBe(dtoFields["CrimeValues.Arrest"]);
        spriggitFields["CrimeValues.Assault"].ShouldBe(dtoFields["CrimeValues.Assault"]);
        spriggitFields["CrimeValues.AttackOnSight"].ShouldBe(dtoFields["CrimeValues.AttackOnSight"]);
        spriggitFields["CrimeValues.Escape"].ShouldBe(dtoFields["CrimeValues.Escape"]);
        spriggitFields["CrimeValues.Murder"].ShouldBe(dtoFields["CrimeValues.Murder"]);
        spriggitFields["CrimeValues.Pickpocket"].ShouldBe(dtoFields["CrimeValues.Pickpocket"]);
        spriggitFields["CrimeValues.StealMult"].ShouldBe(dtoFields["CrimeValues.StealMult"]);
        spriggitFields["CrimeValues.Trespass"].ShouldBe(dtoFields["CrimeValues.Trespass"]);
        spriggitFields["CrimeValues.WerewolfUnused"].ShouldBe(dtoFields["CrimeValues.WerewolfUnused"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["MerchantContainer"].ShouldBe(dtoFields["MerchantContainerFormKey"]);
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
        spriggitFields["VendorBuySellList"].ShouldBe(dtoFields["VendorBuySellListFormKey"]);
        spriggitFields["VendorLocation.Target.MutagenObjectType"].ShouldBe(dtoFields["VendorLocation.Target.MutagenObjectType"]);
        spriggitFields["VendorLocation.Target.Type"].ShouldBe(dtoFields["VendorLocation.Target.Type"]);
        spriggitFields["VendorValues.BuySellEverythingNotInList"].ShouldBe(dtoFields["VendorValues.BuySellEverythingNotInList"]);
        spriggitFields["VendorValues.BuysNonStolenItems"].ShouldBe(dtoFields["VendorValues.BuysNonStolenItems"]);
        spriggitFields["VendorValues.BuysStolenItems"].ShouldBe(dtoFields["VendorValues.BuysStolenItems"]);
        spriggitFields["VendorValues.EndHour"].ShouldBe(dtoFields["VendorValues.EndHour"]);
        spriggitFields["VendorValues.Radius"].ShouldBe(dtoFields["VendorValues.Radius"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "FACT")]
    [Trait("FormKey", "14EB97:Fallout4.esm")]
    [Trait("EditorID", "FarmVendorTheSlog")]
    [Trait("SpriggitFile", "Factions/FarmVendorTheSlog - 14EB97_Fallout4.esm.yaml")]
    public void Fallout4_FACT_ShouldMatchSpriggitSample_FarmVendorTheSlog()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Faction,
            "FarmVendorTheSlog");
        var dto = Helpers.GetDTO<FactionDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Faction,
            "14EB97:Fallout4.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["CrimeValues.Arrest"].ShouldBe(dtoFields["CrimeValues.Arrest"]);
        spriggitFields["CrimeValues.AttackOnSight"].ShouldBe(dtoFields["CrimeValues.AttackOnSight"]);
        spriggitFields["Data.Function"].ShouldBe(dtoFields["Data.Function"]);
        spriggitFields["Data.MutagenObjectType"].ShouldBe(dtoFields["Data.MutagenObjectType"]);
        spriggitFields["Data.ParameterOneNumber"].ShouldBe(dtoFields["Data.ParameterOneNumber"]);
        spriggitFields["Data.ParameterOneRecord"].ShouldBe(dtoFields["Data.ParameterOneRecord"]);
        spriggitFields["Data.Reference"].ShouldBe(dtoFields["Data.Reference"]);
        spriggitFields["Data.RunOnType"].ShouldBe(dtoFields["Data.RunOnType"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["MerchantContainer"].ShouldBe(dtoFields["MerchantContainerFormKey"]);
        spriggitFields["VendorBuySellList"].ShouldBe(dtoFields["VendorBuySellListFormKey"]);
        spriggitFields["VendorLocation.Target.MutagenObjectType"].ShouldBe(dtoFields["VendorLocation.Target.MutagenObjectType"]);
        spriggitFields["VendorLocation.Target.Type"].ShouldBe(dtoFields["VendorLocation.Target.Type"]);
        spriggitFields["VendorValues.BuySellEverythingNotInList"].ShouldBe(dtoFields["VendorValues.BuySellEverythingNotInList"]);
        spriggitFields["VendorValues.BuysNonStolenItems"].ShouldBe(dtoFields["VendorValues.BuysNonStolenItems"]);
        spriggitFields["VendorValues.BuysStolenItems"].ShouldBe(dtoFields["VendorValues.BuysStolenItems"]);
        spriggitFields["VendorValues.EndHour"].ShouldBe(dtoFields["VendorValues.EndHour"]);
        spriggitFields["VendorValues.Radius"].ShouldBe(dtoFields["VendorValues.Radius"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }
}
