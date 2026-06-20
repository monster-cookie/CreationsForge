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

        Helpers.GetSpriggitField(spriggit, "CrimeValues.Arrest").ShouldBe(Helpers.GetDTOField(dto, "CrimeValues.Arrest"));
        Helpers.GetSpriggitField(spriggit, "CrimeValues.AttackOnSight").ShouldBe(Helpers.GetDTOField(dto, "CrimeValues.AttackOnSight"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "MerchantContainer").ShouldBe(Helpers.GetDTOField(dto, "MerchantContainerFormKey"));
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
        Helpers.GetSpriggitField(spriggit, "Reaction").ShouldBe(Helpers.GetDTOField(dto, "Reaction"));
        Helpers.GetSpriggitField(spriggit, "VendorBuySellList").ShouldBe(Helpers.GetDTOField(dto, "VendorBuySellListFormKey"));
        Helpers.GetSpriggitField(spriggit, "VendorValues.BuySellEverythingNotInList").ShouldBe(Helpers.GetDTOField(dto, "VendorValues.BuySellEverythingNotInList"));
        Helpers.GetSpriggitField(spriggit, "VendorValues.BuysNonStolenItems").ShouldBe(Helpers.GetDTOField(dto, "VendorValues.BuysNonStolenItems"));
        Helpers.GetSpriggitField(spriggit, "VendorValues.BuysStolenItems").ShouldBe(Helpers.GetDTOField(dto, "VendorValues.BuysStolenItems"));
        Helpers.GetSpriggitField(spriggit, "VendorValues.EndHour").ShouldBe(Helpers.GetDTOField(dto, "VendorValues.EndHour"));
        Helpers.GetSpriggitField(spriggit, "VendorValues.Radius").ShouldBe(Helpers.GetDTOField(dto, "VendorValues.Radius"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "CrimeValues.Arrest", "CrimeValues.AttackOnSight", "EditorID", "FormKey", "MerchantContainer", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "Reaction", "VendorBuySellList", "VendorValues.BuySellEverythingNotInList", "VendorValues.BuysNonStolenItems", "VendorValues.BuysStolenItems", "VendorValues.EndHour", "VendorValues.Radius", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "CrimeValues.Arrest", "CrimeValues.AttackOnSight", "EditorID", "FormKey", "MerchantContainerFormKey", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "Reaction", "VendorBuySellListFormKey", "VendorValues.BuySellEverythingNotInList", "VendorValues.BuysNonStolenItems", "VendorValues.BuysStolenItems", "VendorValues.EndHour", "VendorValues.Radius", "Version2", "VersionControl");
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

        Helpers.GetSpriggitField(spriggit, "CrimeValues.Arrest").ShouldBe(Helpers.GetDTOField(dto, "CrimeValues.Arrest"));
        Helpers.GetSpriggitField(spriggit, "CrimeValues.AttackOnSight").ShouldBe(Helpers.GetDTOField(dto, "CrimeValues.AttackOnSight"));
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
        Helpers.GetSpriggitField(spriggit, "Reaction[3]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[3]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[4]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[4]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[5]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[5]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[6]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[6]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[7]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[7]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[8]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[8]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[9]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[9]"));
        Helpers.GetSpriggitField(spriggit, "VendorValues").ShouldBe(Helpers.GetDTOField(dto, "VendorValues"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "CrimeValues.Arrest", "CrimeValues.AttackOnSight", "EditorID", "FormKey", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "Reaction[0]", "Reaction[1]", "Reaction[10]", "Reaction[11]", "Reaction[12]", "Reaction[13]", "Reaction[14]", "Reaction[15]", "Reaction[16]", "Reaction[17]", "Reaction[18]", "Reaction[19]", "Reaction[2]", "Reaction[20]", "Reaction[21]", "Reaction[22]", "Reaction[23]", "Reaction[24]", "Reaction[25]", "Reaction[26]", "Reaction[27]", "Reaction[3]", "Reaction[4]", "Reaction[5]", "Reaction[6]", "Reaction[7]", "Reaction[8]", "Reaction[9]", "VendorValues", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "CrimeValues.Arrest", "CrimeValues.AttackOnSight", "EditorID", "FormKey", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "Reaction[0]", "Reaction[1]", "Reaction[10]", "Reaction[11]", "Reaction[12]", "Reaction[13]", "Reaction[14]", "Reaction[15]", "Reaction[16]", "Reaction[17]", "Reaction[18]", "Reaction[19]", "Reaction[2]", "Reaction[20]", "Reaction[21]", "Reaction[22]", "Reaction[23]", "Reaction[24]", "Reaction[25]", "Reaction[26]", "Reaction[27]", "Reaction[3]", "Reaction[4]", "Reaction[5]", "Reaction[6]", "Reaction[7]", "Reaction[8]", "Reaction[9]", "VendorValues", "Version2", "VersionControl");
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

        Helpers.GetSpriggitField(spriggit, "CrimeValues.Arrest").ShouldBe(Helpers.GetDTOField(dto, "CrimeValues.Arrest"));
        Helpers.GetSpriggitField(spriggit, "CrimeValues.AttackOnSight").ShouldBe(Helpers.GetDTOField(dto, "CrimeValues.AttackOnSight"));
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
        Helpers.GetSpriggitField(spriggit, "Reaction[4]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[4]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[5]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[5]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[6]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[6]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[7]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[7]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[8]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[8]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[9]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[9]"));
        Helpers.GetSpriggitField(spriggit, "VendorValues").ShouldBe(Helpers.GetDTOField(dto, "VendorValues"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "CrimeValues.Arrest", "CrimeValues.AttackOnSight", "EditorID", "FormKey", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "Reaction[0]", "Reaction[1]", "Reaction[10]", "Reaction[11]", "Reaction[12]", "Reaction[13]", "Reaction[14]", "Reaction[15]", "Reaction[16]", "Reaction[17]", "Reaction[18]", "Reaction[19]", "Reaction[2]", "Reaction[20]", "Reaction[21]", "Reaction[22]", "Reaction[23]", "Reaction[24]", "Reaction[25]", "Reaction[26]", "Reaction[27]", "Reaction[28]", "Reaction[29]", "Reaction[3]", "Reaction[30]", "Reaction[31]", "Reaction[32]", "Reaction[33]", "Reaction[34]", "Reaction[35]", "Reaction[36]", "Reaction[37]", "Reaction[4]", "Reaction[5]", "Reaction[6]", "Reaction[7]", "Reaction[8]", "Reaction[9]", "VendorValues", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "CrimeValues.Arrest", "CrimeValues.AttackOnSight", "EditorID", "FormKey", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "Reaction[0]", "Reaction[1]", "Reaction[10]", "Reaction[11]", "Reaction[12]", "Reaction[13]", "Reaction[14]", "Reaction[15]", "Reaction[16]", "Reaction[17]", "Reaction[18]", "Reaction[19]", "Reaction[2]", "Reaction[20]", "Reaction[21]", "Reaction[22]", "Reaction[23]", "Reaction[24]", "Reaction[25]", "Reaction[26]", "Reaction[27]", "Reaction[28]", "Reaction[29]", "Reaction[3]", "Reaction[30]", "Reaction[31]", "Reaction[32]", "Reaction[33]", "Reaction[34]", "Reaction[35]", "Reaction[36]", "Reaction[37]", "Reaction[4]", "Reaction[5]", "Reaction[6]", "Reaction[7]", "Reaction[8]", "Reaction[9]", "VendorValues", "Version2", "VersionControl");
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

        Helpers.GetSpriggitField(spriggit, "CrimeValues.Arrest").ShouldBe(Helpers.GetDTOField(dto, "CrimeValues.Arrest"));
        Helpers.GetSpriggitField(spriggit, "CrimeValues.Assault").ShouldBe(Helpers.GetDTOField(dto, "CrimeValues.Assault"));
        Helpers.GetSpriggitField(spriggit, "CrimeValues.AttackOnSight").ShouldBe(Helpers.GetDTOField(dto, "CrimeValues.AttackOnSight"));
        Helpers.GetSpriggitField(spriggit, "CrimeValues.Escape").ShouldBe(Helpers.GetDTOField(dto, "CrimeValues.Escape"));
        Helpers.GetSpriggitField(spriggit, "CrimeValues.Murder").ShouldBe(Helpers.GetDTOField(dto, "CrimeValues.Murder"));
        Helpers.GetSpriggitField(spriggit, "CrimeValues.Pickpocket").ShouldBe(Helpers.GetDTOField(dto, "CrimeValues.Pickpocket"));
        Helpers.GetSpriggitField(spriggit, "CrimeValues.StealMult").ShouldBe(Helpers.GetDTOField(dto, "CrimeValues.StealMult"));
        Helpers.GetSpriggitField(spriggit, "CrimeValues.Trespass").ShouldBe(Helpers.GetDTOField(dto, "CrimeValues.Trespass"));
        Helpers.GetSpriggitField(spriggit, "CrimeValues.WerewolfUnused").ShouldBe(Helpers.GetDTOField(dto, "CrimeValues.WerewolfUnused"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "MerchantContainer").ShouldBe(Helpers.GetDTOField(dto, "MerchantContainerFormKey"));
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
        Helpers.GetSpriggitField(spriggit, "VendorBuySellList").ShouldBe(Helpers.GetDTOField(dto, "VendorBuySellListFormKey"));
        Helpers.GetSpriggitField(spriggit, "VendorLocation.Target.MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VendorLocation.Target.MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VendorLocation.Target.Type").ShouldBe(Helpers.GetDTOField(dto, "VendorLocation.Target.Type"));
        Helpers.GetSpriggitField(spriggit, "VendorValues.BuySellEverythingNotInList").ShouldBe(Helpers.GetDTOField(dto, "VendorValues.BuySellEverythingNotInList"));
        Helpers.GetSpriggitField(spriggit, "VendorValues.BuysNonStolenItems").ShouldBe(Helpers.GetDTOField(dto, "VendorValues.BuysNonStolenItems"));
        Helpers.GetSpriggitField(spriggit, "VendorValues.BuysStolenItems").ShouldBe(Helpers.GetDTOField(dto, "VendorValues.BuysStolenItems"));
        Helpers.GetSpriggitField(spriggit, "VendorValues.EndHour").ShouldBe(Helpers.GetDTOField(dto, "VendorValues.EndHour"));
        Helpers.GetSpriggitField(spriggit, "VendorValues.Radius").ShouldBe(Helpers.GetDTOField(dto, "VendorValues.Radius"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "CrimeValues.Arrest", "CrimeValues.Assault", "CrimeValues.AttackOnSight", "CrimeValues.Escape", "CrimeValues.Murder", "CrimeValues.Pickpocket", "CrimeValues.StealMult", "CrimeValues.Trespass", "CrimeValues.WerewolfUnused", "EditorID", "FormKey", "MerchantContainer", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "VendorBuySellList", "VendorLocation.Target.MutagenObjectType", "VendorLocation.Target.Type", "VendorValues.BuySellEverythingNotInList", "VendorValues.BuysNonStolenItems", "VendorValues.BuysStolenItems", "VendorValues.EndHour", "VendorValues.Radius", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "CrimeValues.Arrest", "CrimeValues.Assault", "CrimeValues.AttackOnSight", "CrimeValues.Escape", "CrimeValues.Murder", "CrimeValues.Pickpocket", "CrimeValues.StealMult", "CrimeValues.Trespass", "CrimeValues.WerewolfUnused", "EditorID", "FormKey", "MerchantContainerFormKey", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "VendorBuySellListFormKey", "VendorLocation.Target.MutagenObjectType", "VendorLocation.Target.Type", "VendorValues.BuySellEverythingNotInList", "VendorValues.BuysNonStolenItems", "VendorValues.BuysStolenItems", "VendorValues.EndHour", "VendorValues.Radius", "Version2", "VersionControl");
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

        Helpers.GetSpriggitField(spriggit, "CrimeValues.Arrest").ShouldBe(Helpers.GetDTOField(dto, "CrimeValues.Arrest"));
        Helpers.GetSpriggitField(spriggit, "CrimeValues.AttackOnSight").ShouldBe(Helpers.GetDTOField(dto, "CrimeValues.AttackOnSight"));
        Helpers.GetSpriggitField(spriggit, "Data.Function").ShouldBe(Helpers.GetDTOField(dto, "Data.Function"));
        Helpers.GetSpriggitField(spriggit, "Data.MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "Data.MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneNumber").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneNumber"));
        Helpers.GetSpriggitField(spriggit, "Data.ParameterOneRecord").ShouldBe(Helpers.GetDTOField(dto, "Data.ParameterOneRecord"));
        Helpers.GetSpriggitField(spriggit, "Data.Reference").ShouldBe(Helpers.GetDTOField(dto, "Data.Reference"));
        Helpers.GetSpriggitField(spriggit, "Data.RunOnType").ShouldBe(Helpers.GetDTOField(dto, "Data.RunOnType"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "MerchantContainer").ShouldBe(Helpers.GetDTOField(dto, "MerchantContainerFormKey"));
        Helpers.GetSpriggitField(spriggit, "VendorBuySellList").ShouldBe(Helpers.GetDTOField(dto, "VendorBuySellListFormKey"));
        Helpers.GetSpriggitField(spriggit, "VendorLocation.Target.MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VendorLocation.Target.MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VendorLocation.Target.Type").ShouldBe(Helpers.GetDTOField(dto, "VendorLocation.Target.Type"));
        Helpers.GetSpriggitField(spriggit, "VendorValues.BuySellEverythingNotInList").ShouldBe(Helpers.GetDTOField(dto, "VendorValues.BuySellEverythingNotInList"));
        Helpers.GetSpriggitField(spriggit, "VendorValues.BuysNonStolenItems").ShouldBe(Helpers.GetDTOField(dto, "VendorValues.BuysNonStolenItems"));
        Helpers.GetSpriggitField(spriggit, "VendorValues.BuysStolenItems").ShouldBe(Helpers.GetDTOField(dto, "VendorValues.BuysStolenItems"));
        Helpers.GetSpriggitField(spriggit, "VendorValues.EndHour").ShouldBe(Helpers.GetDTOField(dto, "VendorValues.EndHour"));
        Helpers.GetSpriggitField(spriggit, "VendorValues.Radius").ShouldBe(Helpers.GetDTOField(dto, "VendorValues.Radius"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "CrimeValues.Arrest", "CrimeValues.AttackOnSight", "Data.Function", "Data.MutagenObjectType", "Data.ParameterOneNumber", "Data.ParameterOneRecord", "Data.Reference", "Data.RunOnType", "EditorID", "FormKey", "FormVersion", "MerchantContainer", "VendorBuySellList", "VendorLocation.Target.MutagenObjectType", "VendorLocation.Target.Type", "VendorValues.BuySellEverythingNotInList", "VendorValues.BuysNonStolenItems", "VendorValues.BuysStolenItems", "VendorValues.EndHour", "VendorValues.Radius", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "CrimeValues.Arrest", "CrimeValues.AttackOnSight", "Data.Function", "Data.MutagenObjectType", "Data.ParameterOneNumber", "Data.ParameterOneRecord", "Data.Reference", "Data.RunOnType", "EditorID", "FormKey", "FormVersion", "MerchantContainerFormKey", "VendorBuySellListFormKey", "VendorLocation.Target.MutagenObjectType", "VendorLocation.Target.Type", "VendorValues.BuySellEverythingNotInList", "VendorValues.BuysNonStolenItems", "VendorValues.BuysStolenItems", "VendorValues.EndHour", "VendorValues.Radius", "Version2", "VersionControl");
    }
}