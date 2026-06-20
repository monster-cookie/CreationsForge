using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.Faction.Skyrim;

public class SkyrimFactionSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "FACT")]
    [Trait("FormKey", "103372:Skyrim.esm")]
    [Trait("EditorID", "CollegeofWinterholdArchMageFaction")]
    [Trait("SpriggitFile", "Factions/CollegeofWinterholdArchMageFaction - 103372_Skyrim.esm.yaml")]
    public void Skyrim_FACT_ShouldMatchSpriggitSample_CollegeofWinterholdArchMageFaction()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Faction,
            "CollegeofWinterholdArchMageFaction");
        var dto = Helpers.GetDTO<FactionDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Faction,
            "103372:Skyrim.esm");

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
        spriggitFields["Title.Female[0]"].ShouldBe(dtoFields["Title.Female[0]"]);
        spriggitFields["Title.Female[1]"].ShouldBe(dtoFields["Title.Female[1]"]);
        spriggitFields["Title.Female[2]"].ShouldBe(dtoFields["Title.Female[2]"]);
        spriggitFields["Title.Female[3]"].ShouldBe(dtoFields["Title.Female[3]"]);
        spriggitFields["Title.Female[4]"].ShouldBe(dtoFields["Title.Female[4]"]);
        spriggitFields["Title.Female[5]"].ShouldBe(dtoFields["Title.Female[5]"]);
        spriggitFields["Title.Female[6]"].ShouldBe(dtoFields["Title.Female[6]"]);
        spriggitFields["Title.Male.Count"].ShouldBe(dtoFields["Title.Male.Count"]);
        spriggitFields["Title.Male.TargetLanguage[0]"].ShouldBe(dtoFields["Title.Male.TargetLanguage[0]"]);
        spriggitFields["Title.Male.TargetLanguage[1]"].ShouldBe(dtoFields["Title.Male.TargetLanguage[1]"]);
        spriggitFields["Title.Male.TargetLanguage[2]"].ShouldBe(dtoFields["Title.Male.TargetLanguage[2]"]);
        spriggitFields["Title.Male.TargetLanguage[3]"].ShouldBe(dtoFields["Title.Male.TargetLanguage[3]"]);
        spriggitFields["Title.Male.TargetLanguage[4]"].ShouldBe(dtoFields["Title.Male.TargetLanguage[4]"]);
        spriggitFields["Title.Male.TargetLanguage[5]"].ShouldBe(dtoFields["Title.Male.TargetLanguage[5]"]);
        spriggitFields["Title.Male.TargetLanguage[6]"].ShouldBe(dtoFields["Title.Male.TargetLanguage[6]"]);
        spriggitFields["Title.Male[0].Language"].ShouldBe(dtoFields["Title.Male[0].Language"]);
        spriggitFields["Title.Male[0].String"].ShouldBe(dtoFields["Title.Male[0].String"]);
        spriggitFields["Title.Male[1].Language"].ShouldBe(dtoFields["Title.Male[1].Language"]);
        spriggitFields["Title.Male[1].String"].ShouldBe(dtoFields["Title.Male[1].String"]);
        spriggitFields["Title.Male[10].Language"].ShouldBe(dtoFields["Title.Male[10].Language"]);
        spriggitFields["Title.Male[10].String"].ShouldBe(dtoFields["Title.Male[10].String"]);
        spriggitFields["Title.Male[11].Language"].ShouldBe(dtoFields["Title.Male[11].Language"]);
        spriggitFields["Title.Male[11].String"].ShouldBe(dtoFields["Title.Male[11].String"]);
        spriggitFields["Title.Male[12].Language"].ShouldBe(dtoFields["Title.Male[12].Language"]);
        spriggitFields["Title.Male[12].String"].ShouldBe(dtoFields["Title.Male[12].String"]);
        spriggitFields["Title.Male[13].Language"].ShouldBe(dtoFields["Title.Male[13].Language"]);
        spriggitFields["Title.Male[13].String"].ShouldBe(dtoFields["Title.Male[13].String"]);
        spriggitFields["Title.Male[14].Language"].ShouldBe(dtoFields["Title.Male[14].Language"]);
        spriggitFields["Title.Male[14].String"].ShouldBe(dtoFields["Title.Male[14].String"]);
        spriggitFields["Title.Male[15].Language"].ShouldBe(dtoFields["Title.Male[15].Language"]);
        spriggitFields["Title.Male[15].String"].ShouldBe(dtoFields["Title.Male[15].String"]);
        spriggitFields["Title.Male[16].Language"].ShouldBe(dtoFields["Title.Male[16].Language"]);
        spriggitFields["Title.Male[16].String"].ShouldBe(dtoFields["Title.Male[16].String"]);
        spriggitFields["Title.Male[17].Language"].ShouldBe(dtoFields["Title.Male[17].Language"]);
        spriggitFields["Title.Male[17].String"].ShouldBe(dtoFields["Title.Male[17].String"]);
        spriggitFields["Title.Male[18].Language"].ShouldBe(dtoFields["Title.Male[18].Language"]);
        spriggitFields["Title.Male[18].String"].ShouldBe(dtoFields["Title.Male[18].String"]);
        spriggitFields["Title.Male[19].Language"].ShouldBe(dtoFields["Title.Male[19].Language"]);
        spriggitFields["Title.Male[19].String"].ShouldBe(dtoFields["Title.Male[19].String"]);
        spriggitFields["Title.Male[2].Language"].ShouldBe(dtoFields["Title.Male[2].Language"]);
        spriggitFields["Title.Male[2].String"].ShouldBe(dtoFields["Title.Male[2].String"]);
        spriggitFields["Title.Male[20].Language"].ShouldBe(dtoFields["Title.Male[20].Language"]);
        spriggitFields["Title.Male[20].String"].ShouldBe(dtoFields["Title.Male[20].String"]);
        spriggitFields["Title.Male[21].Language"].ShouldBe(dtoFields["Title.Male[21].Language"]);
        spriggitFields["Title.Male[21].String"].ShouldBe(dtoFields["Title.Male[21].String"]);
        spriggitFields["Title.Male[22].Language"].ShouldBe(dtoFields["Title.Male[22].Language"]);
        spriggitFields["Title.Male[22].String"].ShouldBe(dtoFields["Title.Male[22].String"]);
        spriggitFields["Title.Male[23].Language"].ShouldBe(dtoFields["Title.Male[23].Language"]);
        spriggitFields["Title.Male[23].String"].ShouldBe(dtoFields["Title.Male[23].String"]);
        spriggitFields["Title.Male[24].Language"].ShouldBe(dtoFields["Title.Male[24].Language"]);
        spriggitFields["Title.Male[24].String"].ShouldBe(dtoFields["Title.Male[24].String"]);
        spriggitFields["Title.Male[25].Language"].ShouldBe(dtoFields["Title.Male[25].Language"]);
        spriggitFields["Title.Male[25].String"].ShouldBe(dtoFields["Title.Male[25].String"]);
        spriggitFields["Title.Male[26].Language"].ShouldBe(dtoFields["Title.Male[26].Language"]);
        spriggitFields["Title.Male[26].String"].ShouldBe(dtoFields["Title.Male[26].String"]);
        spriggitFields["Title.Male[27].Language"].ShouldBe(dtoFields["Title.Male[27].Language"]);
        spriggitFields["Title.Male[27].String"].ShouldBe(dtoFields["Title.Male[27].String"]);
        spriggitFields["Title.Male[28].Language"].ShouldBe(dtoFields["Title.Male[28].Language"]);
        spriggitFields["Title.Male[28].String"].ShouldBe(dtoFields["Title.Male[28].String"]);
        spriggitFields["Title.Male[29].Language"].ShouldBe(dtoFields["Title.Male[29].Language"]);
        spriggitFields["Title.Male[29].String"].ShouldBe(dtoFields["Title.Male[29].String"]);
        spriggitFields["Title.Male[3].Language"].ShouldBe(dtoFields["Title.Male[3].Language"]);
        spriggitFields["Title.Male[3].String"].ShouldBe(dtoFields["Title.Male[3].String"]);
        spriggitFields["Title.Male[30].Language"].ShouldBe(dtoFields["Title.Male[30].Language"]);
        spriggitFields["Title.Male[30].String"].ShouldBe(dtoFields["Title.Male[30].String"]);
        spriggitFields["Title.Male[31].Language"].ShouldBe(dtoFields["Title.Male[31].Language"]);
        spriggitFields["Title.Male[31].String"].ShouldBe(dtoFields["Title.Male[31].String"]);
        spriggitFields["Title.Male[32].Language"].ShouldBe(dtoFields["Title.Male[32].Language"]);
        spriggitFields["Title.Male[32].String"].ShouldBe(dtoFields["Title.Male[32].String"]);
        spriggitFields["Title.Male[33].Language"].ShouldBe(dtoFields["Title.Male[33].Language"]);
        spriggitFields["Title.Male[33].String"].ShouldBe(dtoFields["Title.Male[33].String"]);
        spriggitFields["Title.Male[34].Language"].ShouldBe(dtoFields["Title.Male[34].Language"]);
        spriggitFields["Title.Male[34].String"].ShouldBe(dtoFields["Title.Male[34].String"]);
        spriggitFields["Title.Male[35].Language"].ShouldBe(dtoFields["Title.Male[35].Language"]);
        spriggitFields["Title.Male[35].String"].ShouldBe(dtoFields["Title.Male[35].String"]);
        spriggitFields["Title.Male[36].Language"].ShouldBe(dtoFields["Title.Male[36].Language"]);
        spriggitFields["Title.Male[36].String"].ShouldBe(dtoFields["Title.Male[36].String"]);
        spriggitFields["Title.Male[37].Language"].ShouldBe(dtoFields["Title.Male[37].Language"]);
        spriggitFields["Title.Male[37].String"].ShouldBe(dtoFields["Title.Male[37].String"]);
        spriggitFields["Title.Male[38].Language"].ShouldBe(dtoFields["Title.Male[38].Language"]);
        spriggitFields["Title.Male[38].String"].ShouldBe(dtoFields["Title.Male[38].String"]);
        spriggitFields["Title.Male[39].Language"].ShouldBe(dtoFields["Title.Male[39].Language"]);
        spriggitFields["Title.Male[39].String"].ShouldBe(dtoFields["Title.Male[39].String"]);
        spriggitFields["Title.Male[4].Language"].ShouldBe(dtoFields["Title.Male[4].Language"]);
        spriggitFields["Title.Male[4].String"].ShouldBe(dtoFields["Title.Male[4].String"]);
        spriggitFields["Title.Male[40].Language"].ShouldBe(dtoFields["Title.Male[40].Language"]);
        spriggitFields["Title.Male[40].String"].ShouldBe(dtoFields["Title.Male[40].String"]);
        spriggitFields["Title.Male[41].Language"].ShouldBe(dtoFields["Title.Male[41].Language"]);
        spriggitFields["Title.Male[41].String"].ShouldBe(dtoFields["Title.Male[41].String"]);
        spriggitFields["Title.Male[42].Language"].ShouldBe(dtoFields["Title.Male[42].Language"]);
        spriggitFields["Title.Male[42].String"].ShouldBe(dtoFields["Title.Male[42].String"]);
        spriggitFields["Title.Male[43].Language"].ShouldBe(dtoFields["Title.Male[43].Language"]);
        spriggitFields["Title.Male[43].String"].ShouldBe(dtoFields["Title.Male[43].String"]);
        spriggitFields["Title.Male[44].Language"].ShouldBe(dtoFields["Title.Male[44].Language"]);
        spriggitFields["Title.Male[44].String"].ShouldBe(dtoFields["Title.Male[44].String"]);
        spriggitFields["Title.Male[45].Language"].ShouldBe(dtoFields["Title.Male[45].Language"]);
        spriggitFields["Title.Male[45].String"].ShouldBe(dtoFields["Title.Male[45].String"]);
        spriggitFields["Title.Male[46].Language"].ShouldBe(dtoFields["Title.Male[46].Language"]);
        spriggitFields["Title.Male[46].String"].ShouldBe(dtoFields["Title.Male[46].String"]);
        spriggitFields["Title.Male[47].Language"].ShouldBe(dtoFields["Title.Male[47].Language"]);
        spriggitFields["Title.Male[47].String"].ShouldBe(dtoFields["Title.Male[47].String"]);
        spriggitFields["Title.Male[48].Language"].ShouldBe(dtoFields["Title.Male[48].Language"]);
        spriggitFields["Title.Male[48].String"].ShouldBe(dtoFields["Title.Male[48].String"]);
        spriggitFields["Title.Male[49].Language"].ShouldBe(dtoFields["Title.Male[49].Language"]);
        spriggitFields["Title.Male[49].String"].ShouldBe(dtoFields["Title.Male[49].String"]);
        spriggitFields["Title.Male[5].Language"].ShouldBe(dtoFields["Title.Male[5].Language"]);
        spriggitFields["Title.Male[5].String"].ShouldBe(dtoFields["Title.Male[5].String"]);
        spriggitFields["Title.Male[50].Language"].ShouldBe(dtoFields["Title.Male[50].Language"]);
        spriggitFields["Title.Male[50].String"].ShouldBe(dtoFields["Title.Male[50].String"]);
        spriggitFields["Title.Male[51].Language"].ShouldBe(dtoFields["Title.Male[51].Language"]);
        spriggitFields["Title.Male[51].String"].ShouldBe(dtoFields["Title.Male[51].String"]);
        spriggitFields["Title.Male[52].Language"].ShouldBe(dtoFields["Title.Male[52].Language"]);
        spriggitFields["Title.Male[52].String"].ShouldBe(dtoFields["Title.Male[52].String"]);
        spriggitFields["Title.Male[53].Language"].ShouldBe(dtoFields["Title.Male[53].Language"]);
        spriggitFields["Title.Male[53].String"].ShouldBe(dtoFields["Title.Male[53].String"]);
        spriggitFields["Title.Male[54].Language"].ShouldBe(dtoFields["Title.Male[54].Language"]);
        spriggitFields["Title.Male[54].String"].ShouldBe(dtoFields["Title.Male[54].String"]);
        spriggitFields["Title.Male[55].Language"].ShouldBe(dtoFields["Title.Male[55].Language"]);
        spriggitFields["Title.Male[55].String"].ShouldBe(dtoFields["Title.Male[55].String"]);
        spriggitFields["Title.Male[56].Language"].ShouldBe(dtoFields["Title.Male[56].Language"]);
        spriggitFields["Title.Male[56].String"].ShouldBe(dtoFields["Title.Male[56].String"]);
        spriggitFields["Title.Male[57].Language"].ShouldBe(dtoFields["Title.Male[57].Language"]);
        spriggitFields["Title.Male[57].String"].ShouldBe(dtoFields["Title.Male[57].String"]);
        spriggitFields["Title.Male[58].Language"].ShouldBe(dtoFields["Title.Male[58].Language"]);
        spriggitFields["Title.Male[58].String"].ShouldBe(dtoFields["Title.Male[58].String"]);
        spriggitFields["Title.Male[59].Language"].ShouldBe(dtoFields["Title.Male[59].Language"]);
        spriggitFields["Title.Male[59].String"].ShouldBe(dtoFields["Title.Male[59].String"]);
        spriggitFields["Title.Male[6].Language"].ShouldBe(dtoFields["Title.Male[6].Language"]);
        spriggitFields["Title.Male[6].String"].ShouldBe(dtoFields["Title.Male[6].String"]);
        spriggitFields["Title.Male[60].Language"].ShouldBe(dtoFields["Title.Male[60].Language"]);
        spriggitFields["Title.Male[60].String"].ShouldBe(dtoFields["Title.Male[60].String"]);
        spriggitFields["Title.Male[61].Language"].ShouldBe(dtoFields["Title.Male[61].Language"]);
        spriggitFields["Title.Male[61].String"].ShouldBe(dtoFields["Title.Male[61].String"]);
        spriggitFields["Title.Male[62].Language"].ShouldBe(dtoFields["Title.Male[62].Language"]);
        spriggitFields["Title.Male[62].String"].ShouldBe(dtoFields["Title.Male[62].String"]);
        spriggitFields["Title.Male[7].Language"].ShouldBe(dtoFields["Title.Male[7].Language"]);
        spriggitFields["Title.Male[7].String"].ShouldBe(dtoFields["Title.Male[7].String"]);
        spriggitFields["Title.Male[8].Language"].ShouldBe(dtoFields["Title.Male[8].Language"]);
        spriggitFields["Title.Male[8].String"].ShouldBe(dtoFields["Title.Male[8].String"]);
        spriggitFields["Title.Male[9].Language"].ShouldBe(dtoFields["Title.Male[9].Language"]);
        spriggitFields["Title.Male[9].String"].ShouldBe(dtoFields["Title.Male[9].String"]);
        spriggitFields["VendorValues"].ShouldBe(dtoFields["VendorValues"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "FACT")]
    [Trait("FormKey", "01F259:Skyrim.esm")]
    [Trait("EditorID", "CollegeofWinterholdFaction")]
    [Trait("SpriggitFile", "Factions/CollegeofWinterholdFaction - 01F259_Skyrim.esm.yaml")]
    public void Skyrim_FACT_ShouldMatchSpriggitSample_CollegeofWinterholdFaction()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Faction,
            "CollegeofWinterholdFaction");
        var dto = Helpers.GetDTO<FactionDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Faction,
            "01F259:Skyrim.esm");

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
        spriggitFields["Reaction[2]"].ShouldBe(dtoFields["Reaction[2]"]);
        spriggitFields["Reaction[3]"].ShouldBe(dtoFields["Reaction[3]"]);
        spriggitFields["Reaction[4]"].ShouldBe(dtoFields["Reaction[4]"]);
        spriggitFields["Title.Female[0]"].ShouldBe(dtoFields["Title.Female[0]"]);
        spriggitFields["Title.Female[1]"].ShouldBe(dtoFields["Title.Female[1]"]);
        spriggitFields["Title.Female[2]"].ShouldBe(dtoFields["Title.Female[2]"]);
        spriggitFields["Title.Female[3]"].ShouldBe(dtoFields["Title.Female[3]"]);
        spriggitFields["Title.Female[4]"].ShouldBe(dtoFields["Title.Female[4]"]);
        spriggitFields["Title.Female[5]"].ShouldBe(dtoFields["Title.Female[5]"]);
        spriggitFields["Title.Female[6]"].ShouldBe(dtoFields["Title.Female[6]"]);
        spriggitFields["Title.Male.Count"].ShouldBe(dtoFields["Title.Male.Count"]);
        spriggitFields["Title.Male.TargetLanguage[0]"].ShouldBe(dtoFields["Title.Male.TargetLanguage[0]"]);
        spriggitFields["Title.Male.TargetLanguage[1]"].ShouldBe(dtoFields["Title.Male.TargetLanguage[1]"]);
        spriggitFields["Title.Male.TargetLanguage[2]"].ShouldBe(dtoFields["Title.Male.TargetLanguage[2]"]);
        spriggitFields["Title.Male.TargetLanguage[3]"].ShouldBe(dtoFields["Title.Male.TargetLanguage[3]"]);
        spriggitFields["Title.Male.TargetLanguage[4]"].ShouldBe(dtoFields["Title.Male.TargetLanguage[4]"]);
        spriggitFields["Title.Male.TargetLanguage[5]"].ShouldBe(dtoFields["Title.Male.TargetLanguage[5]"]);
        spriggitFields["Title.Male.TargetLanguage[6]"].ShouldBe(dtoFields["Title.Male.TargetLanguage[6]"]);
        spriggitFields["Title.Male[0].Language"].ShouldBe(dtoFields["Title.Male[0].Language"]);
        spriggitFields["Title.Male[0].String"].ShouldBe(dtoFields["Title.Male[0].String"]);
        spriggitFields["Title.Male[1].Language"].ShouldBe(dtoFields["Title.Male[1].Language"]);
        spriggitFields["Title.Male[1].String"].ShouldBe(dtoFields["Title.Male[1].String"]);
        spriggitFields["Title.Male[10].Language"].ShouldBe(dtoFields["Title.Male[10].Language"]);
        spriggitFields["Title.Male[10].String"].ShouldBe(dtoFields["Title.Male[10].String"]);
        spriggitFields["Title.Male[11].Language"].ShouldBe(dtoFields["Title.Male[11].Language"]);
        spriggitFields["Title.Male[11].String"].ShouldBe(dtoFields["Title.Male[11].String"]);
        spriggitFields["Title.Male[12].Language"].ShouldBe(dtoFields["Title.Male[12].Language"]);
        spriggitFields["Title.Male[12].String"].ShouldBe(dtoFields["Title.Male[12].String"]);
        spriggitFields["Title.Male[13].Language"].ShouldBe(dtoFields["Title.Male[13].Language"]);
        spriggitFields["Title.Male[13].String"].ShouldBe(dtoFields["Title.Male[13].String"]);
        spriggitFields["Title.Male[14].Language"].ShouldBe(dtoFields["Title.Male[14].Language"]);
        spriggitFields["Title.Male[14].String"].ShouldBe(dtoFields["Title.Male[14].String"]);
        spriggitFields["Title.Male[15].Language"].ShouldBe(dtoFields["Title.Male[15].Language"]);
        spriggitFields["Title.Male[15].String"].ShouldBe(dtoFields["Title.Male[15].String"]);
        spriggitFields["Title.Male[16].Language"].ShouldBe(dtoFields["Title.Male[16].Language"]);
        spriggitFields["Title.Male[16].String"].ShouldBe(dtoFields["Title.Male[16].String"]);
        spriggitFields["Title.Male[17].Language"].ShouldBe(dtoFields["Title.Male[17].Language"]);
        spriggitFields["Title.Male[17].String"].ShouldBe(dtoFields["Title.Male[17].String"]);
        spriggitFields["Title.Male[18].Language"].ShouldBe(dtoFields["Title.Male[18].Language"]);
        spriggitFields["Title.Male[18].String"].ShouldBe(dtoFields["Title.Male[18].String"]);
        spriggitFields["Title.Male[19].Language"].ShouldBe(dtoFields["Title.Male[19].Language"]);
        spriggitFields["Title.Male[19].String"].ShouldBe(dtoFields["Title.Male[19].String"]);
        spriggitFields["Title.Male[2].Language"].ShouldBe(dtoFields["Title.Male[2].Language"]);
        spriggitFields["Title.Male[2].String"].ShouldBe(dtoFields["Title.Male[2].String"]);
        spriggitFields["Title.Male[20].Language"].ShouldBe(dtoFields["Title.Male[20].Language"]);
        spriggitFields["Title.Male[20].String"].ShouldBe(dtoFields["Title.Male[20].String"]);
        spriggitFields["Title.Male[21].Language"].ShouldBe(dtoFields["Title.Male[21].Language"]);
        spriggitFields["Title.Male[21].String"].ShouldBe(dtoFields["Title.Male[21].String"]);
        spriggitFields["Title.Male[22].Language"].ShouldBe(dtoFields["Title.Male[22].Language"]);
        spriggitFields["Title.Male[22].String"].ShouldBe(dtoFields["Title.Male[22].String"]);
        spriggitFields["Title.Male[23].Language"].ShouldBe(dtoFields["Title.Male[23].Language"]);
        spriggitFields["Title.Male[23].String"].ShouldBe(dtoFields["Title.Male[23].String"]);
        spriggitFields["Title.Male[24].Language"].ShouldBe(dtoFields["Title.Male[24].Language"]);
        spriggitFields["Title.Male[24].String"].ShouldBe(dtoFields["Title.Male[24].String"]);
        spriggitFields["Title.Male[25].Language"].ShouldBe(dtoFields["Title.Male[25].Language"]);
        spriggitFields["Title.Male[25].String"].ShouldBe(dtoFields["Title.Male[25].String"]);
        spriggitFields["Title.Male[26].Language"].ShouldBe(dtoFields["Title.Male[26].Language"]);
        spriggitFields["Title.Male[26].String"].ShouldBe(dtoFields["Title.Male[26].String"]);
        spriggitFields["Title.Male[27].Language"].ShouldBe(dtoFields["Title.Male[27].Language"]);
        spriggitFields["Title.Male[27].String"].ShouldBe(dtoFields["Title.Male[27].String"]);
        spriggitFields["Title.Male[28].Language"].ShouldBe(dtoFields["Title.Male[28].Language"]);
        spriggitFields["Title.Male[28].String"].ShouldBe(dtoFields["Title.Male[28].String"]);
        spriggitFields["Title.Male[29].Language"].ShouldBe(dtoFields["Title.Male[29].Language"]);
        spriggitFields["Title.Male[29].String"].ShouldBe(dtoFields["Title.Male[29].String"]);
        spriggitFields["Title.Male[3].Language"].ShouldBe(dtoFields["Title.Male[3].Language"]);
        spriggitFields["Title.Male[3].String"].ShouldBe(dtoFields["Title.Male[3].String"]);
        spriggitFields["Title.Male[30].Language"].ShouldBe(dtoFields["Title.Male[30].Language"]);
        spriggitFields["Title.Male[30].String"].ShouldBe(dtoFields["Title.Male[30].String"]);
        spriggitFields["Title.Male[31].Language"].ShouldBe(dtoFields["Title.Male[31].Language"]);
        spriggitFields["Title.Male[31].String"].ShouldBe(dtoFields["Title.Male[31].String"]);
        spriggitFields["Title.Male[32].Language"].ShouldBe(dtoFields["Title.Male[32].Language"]);
        spriggitFields["Title.Male[32].String"].ShouldBe(dtoFields["Title.Male[32].String"]);
        spriggitFields["Title.Male[33].Language"].ShouldBe(dtoFields["Title.Male[33].Language"]);
        spriggitFields["Title.Male[33].String"].ShouldBe(dtoFields["Title.Male[33].String"]);
        spriggitFields["Title.Male[34].Language"].ShouldBe(dtoFields["Title.Male[34].Language"]);
        spriggitFields["Title.Male[34].String"].ShouldBe(dtoFields["Title.Male[34].String"]);
        spriggitFields["Title.Male[35].Language"].ShouldBe(dtoFields["Title.Male[35].Language"]);
        spriggitFields["Title.Male[35].String"].ShouldBe(dtoFields["Title.Male[35].String"]);
        spriggitFields["Title.Male[36].Language"].ShouldBe(dtoFields["Title.Male[36].Language"]);
        spriggitFields["Title.Male[36].String"].ShouldBe(dtoFields["Title.Male[36].String"]);
        spriggitFields["Title.Male[37].Language"].ShouldBe(dtoFields["Title.Male[37].Language"]);
        spriggitFields["Title.Male[37].String"].ShouldBe(dtoFields["Title.Male[37].String"]);
        spriggitFields["Title.Male[38].Language"].ShouldBe(dtoFields["Title.Male[38].Language"]);
        spriggitFields["Title.Male[38].String"].ShouldBe(dtoFields["Title.Male[38].String"]);
        spriggitFields["Title.Male[39].Language"].ShouldBe(dtoFields["Title.Male[39].Language"]);
        spriggitFields["Title.Male[39].String"].ShouldBe(dtoFields["Title.Male[39].String"]);
        spriggitFields["Title.Male[4].Language"].ShouldBe(dtoFields["Title.Male[4].Language"]);
        spriggitFields["Title.Male[4].String"].ShouldBe(dtoFields["Title.Male[4].String"]);
        spriggitFields["Title.Male[40].Language"].ShouldBe(dtoFields["Title.Male[40].Language"]);
        spriggitFields["Title.Male[40].String"].ShouldBe(dtoFields["Title.Male[40].String"]);
        spriggitFields["Title.Male[41].Language"].ShouldBe(dtoFields["Title.Male[41].Language"]);
        spriggitFields["Title.Male[41].String"].ShouldBe(dtoFields["Title.Male[41].String"]);
        spriggitFields["Title.Male[42].Language"].ShouldBe(dtoFields["Title.Male[42].Language"]);
        spriggitFields["Title.Male[42].String"].ShouldBe(dtoFields["Title.Male[42].String"]);
        spriggitFields["Title.Male[43].Language"].ShouldBe(dtoFields["Title.Male[43].Language"]);
        spriggitFields["Title.Male[43].String"].ShouldBe(dtoFields["Title.Male[43].String"]);
        spriggitFields["Title.Male[44].Language"].ShouldBe(dtoFields["Title.Male[44].Language"]);
        spriggitFields["Title.Male[44].String"].ShouldBe(dtoFields["Title.Male[44].String"]);
        spriggitFields["Title.Male[45].Language"].ShouldBe(dtoFields["Title.Male[45].Language"]);
        spriggitFields["Title.Male[45].String"].ShouldBe(dtoFields["Title.Male[45].String"]);
        spriggitFields["Title.Male[46].Language"].ShouldBe(dtoFields["Title.Male[46].Language"]);
        spriggitFields["Title.Male[46].String"].ShouldBe(dtoFields["Title.Male[46].String"]);
        spriggitFields["Title.Male[47].Language"].ShouldBe(dtoFields["Title.Male[47].Language"]);
        spriggitFields["Title.Male[47].String"].ShouldBe(dtoFields["Title.Male[47].String"]);
        spriggitFields["Title.Male[48].Language"].ShouldBe(dtoFields["Title.Male[48].Language"]);
        spriggitFields["Title.Male[48].String"].ShouldBe(dtoFields["Title.Male[48].String"]);
        spriggitFields["Title.Male[49].Language"].ShouldBe(dtoFields["Title.Male[49].Language"]);
        spriggitFields["Title.Male[49].String"].ShouldBe(dtoFields["Title.Male[49].String"]);
        spriggitFields["Title.Male[5].Language"].ShouldBe(dtoFields["Title.Male[5].Language"]);
        spriggitFields["Title.Male[5].String"].ShouldBe(dtoFields["Title.Male[5].String"]);
        spriggitFields["Title.Male[50].Language"].ShouldBe(dtoFields["Title.Male[50].Language"]);
        spriggitFields["Title.Male[50].String"].ShouldBe(dtoFields["Title.Male[50].String"]);
        spriggitFields["Title.Male[51].Language"].ShouldBe(dtoFields["Title.Male[51].Language"]);
        spriggitFields["Title.Male[51].String"].ShouldBe(dtoFields["Title.Male[51].String"]);
        spriggitFields["Title.Male[52].Language"].ShouldBe(dtoFields["Title.Male[52].Language"]);
        spriggitFields["Title.Male[52].String"].ShouldBe(dtoFields["Title.Male[52].String"]);
        spriggitFields["Title.Male[53].Language"].ShouldBe(dtoFields["Title.Male[53].Language"]);
        spriggitFields["Title.Male[53].String"].ShouldBe(dtoFields["Title.Male[53].String"]);
        spriggitFields["Title.Male[54].Language"].ShouldBe(dtoFields["Title.Male[54].Language"]);
        spriggitFields["Title.Male[54].String"].ShouldBe(dtoFields["Title.Male[54].String"]);
        spriggitFields["Title.Male[55].Language"].ShouldBe(dtoFields["Title.Male[55].Language"]);
        spriggitFields["Title.Male[55].String"].ShouldBe(dtoFields["Title.Male[55].String"]);
        spriggitFields["Title.Male[56].Language"].ShouldBe(dtoFields["Title.Male[56].Language"]);
        spriggitFields["Title.Male[56].String"].ShouldBe(dtoFields["Title.Male[56].String"]);
        spriggitFields["Title.Male[57].Language"].ShouldBe(dtoFields["Title.Male[57].Language"]);
        spriggitFields["Title.Male[57].String"].ShouldBe(dtoFields["Title.Male[57].String"]);
        spriggitFields["Title.Male[58].Language"].ShouldBe(dtoFields["Title.Male[58].Language"]);
        spriggitFields["Title.Male[58].String"].ShouldBe(dtoFields["Title.Male[58].String"]);
        spriggitFields["Title.Male[59].Language"].ShouldBe(dtoFields["Title.Male[59].Language"]);
        spriggitFields["Title.Male[59].String"].ShouldBe(dtoFields["Title.Male[59].String"]);
        spriggitFields["Title.Male[6].Language"].ShouldBe(dtoFields["Title.Male[6].Language"]);
        spriggitFields["Title.Male[6].String"].ShouldBe(dtoFields["Title.Male[6].String"]);
        spriggitFields["Title.Male[60].Language"].ShouldBe(dtoFields["Title.Male[60].Language"]);
        spriggitFields["Title.Male[60].String"].ShouldBe(dtoFields["Title.Male[60].String"]);
        spriggitFields["Title.Male[61].Language"].ShouldBe(dtoFields["Title.Male[61].Language"]);
        spriggitFields["Title.Male[61].String"].ShouldBe(dtoFields["Title.Male[61].String"]);
        spriggitFields["Title.Male[62].Language"].ShouldBe(dtoFields["Title.Male[62].Language"]);
        spriggitFields["Title.Male[62].String"].ShouldBe(dtoFields["Title.Male[62].String"]);
        spriggitFields["Title.Male[7].Language"].ShouldBe(dtoFields["Title.Male[7].Language"]);
        spriggitFields["Title.Male[7].String"].ShouldBe(dtoFields["Title.Male[7].String"]);
        spriggitFields["Title.Male[8].Language"].ShouldBe(dtoFields["Title.Male[8].Language"]);
        spriggitFields["Title.Male[8].String"].ShouldBe(dtoFields["Title.Male[8].String"]);
        spriggitFields["Title.Male[9].Language"].ShouldBe(dtoFields["Title.Male[9].Language"]);
        spriggitFields["Title.Male[9].String"].ShouldBe(dtoFields["Title.Male[9].String"]);
        spriggitFields["VendorValues"].ShouldBe(dtoFields["VendorValues"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "FACT")]
    [Trait("FormKey", "048362:Skyrim.esm")]
    [Trait("EditorID", "CompanionsFaction")]
    [Trait("SpriggitFile", "Factions/CompanionsFaction - 048362_Skyrim.esm.yaml")]
    public void Skyrim_FACT_ShouldMatchSpriggitSample_CompanionsFaction()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Faction,
            "CompanionsFaction");
        var dto = Helpers.GetDTO<FactionDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Faction,
            "048362:Skyrim.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["CrimeValues.Arrest"].ShouldBe(dtoFields["CrimeValues.Arrest"]);
        spriggitFields["CrimeValues.Pickpocket"].ShouldBe(dtoFields["CrimeValues.Pickpocket"]);
        spriggitFields["CrimeValues.StealMult"].ShouldBe(dtoFields["CrimeValues.StealMult"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
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
        spriggitFields["Reaction[2]"].ShouldBe(dtoFields["Reaction[2]"]);
        spriggitFields["Reaction[3]"].ShouldBe(dtoFields["Reaction[3]"]);
        spriggitFields["Reaction[4]"].ShouldBe(dtoFields["Reaction[4]"]);
        spriggitFields["Title.Female.Count"].ShouldBe(dtoFields["Title.Female.Count"]);
        spriggitFields["Title.Female.TargetLanguage[0]"].ShouldBe(dtoFields["Title.Female.TargetLanguage[0]"]);
        spriggitFields["Title.Female.TargetLanguage[1]"].ShouldBe(dtoFields["Title.Female.TargetLanguage[1]"]);
        spriggitFields["Title.Female.TargetLanguage[2]"].ShouldBe(dtoFields["Title.Female.TargetLanguage[2]"]);
        spriggitFields["Title.Female.TargetLanguage[3]"].ShouldBe(dtoFields["Title.Female.TargetLanguage[3]"]);
        spriggitFields["Title.Female[0].Language"].ShouldBe(dtoFields["Title.Female[0].Language"]);
        spriggitFields["Title.Female[0].String"].ShouldBe(dtoFields["Title.Female[0].String"]);
        spriggitFields["Title.Female[1].Language"].ShouldBe(dtoFields["Title.Female[1].Language"]);
        spriggitFields["Title.Female[1].String"].ShouldBe(dtoFields["Title.Female[1].String"]);
        spriggitFields["Title.Female[10].Language"].ShouldBe(dtoFields["Title.Female[10].Language"]);
        spriggitFields["Title.Female[10].String"].ShouldBe(dtoFields["Title.Female[10].String"]);
        spriggitFields["Title.Female[11].Language"].ShouldBe(dtoFields["Title.Female[11].Language"]);
        spriggitFields["Title.Female[11].String"].ShouldBe(dtoFields["Title.Female[11].String"]);
        spriggitFields["Title.Female[12].Language"].ShouldBe(dtoFields["Title.Female[12].Language"]);
        spriggitFields["Title.Female[12].String"].ShouldBe(dtoFields["Title.Female[12].String"]);
        spriggitFields["Title.Female[13].Language"].ShouldBe(dtoFields["Title.Female[13].Language"]);
        spriggitFields["Title.Female[13].String"].ShouldBe(dtoFields["Title.Female[13].String"]);
        spriggitFields["Title.Female[14].Language"].ShouldBe(dtoFields["Title.Female[14].Language"]);
        spriggitFields["Title.Female[14].String"].ShouldBe(dtoFields["Title.Female[14].String"]);
        spriggitFields["Title.Female[15].Language"].ShouldBe(dtoFields["Title.Female[15].Language"]);
        spriggitFields["Title.Female[15].String"].ShouldBe(dtoFields["Title.Female[15].String"]);
        spriggitFields["Title.Female[16].Language"].ShouldBe(dtoFields["Title.Female[16].Language"]);
        spriggitFields["Title.Female[16].String"].ShouldBe(dtoFields["Title.Female[16].String"]);
        spriggitFields["Title.Female[17].Language"].ShouldBe(dtoFields["Title.Female[17].Language"]);
        spriggitFields["Title.Female[17].String"].ShouldBe(dtoFields["Title.Female[17].String"]);
        spriggitFields["Title.Female[18].Language"].ShouldBe(dtoFields["Title.Female[18].Language"]);
        spriggitFields["Title.Female[18].String"].ShouldBe(dtoFields["Title.Female[18].String"]);
        spriggitFields["Title.Female[19].Language"].ShouldBe(dtoFields["Title.Female[19].Language"]);
        spriggitFields["Title.Female[19].String"].ShouldBe(dtoFields["Title.Female[19].String"]);
        spriggitFields["Title.Female[2].Language"].ShouldBe(dtoFields["Title.Female[2].Language"]);
        spriggitFields["Title.Female[2].String"].ShouldBe(dtoFields["Title.Female[2].String"]);
        spriggitFields["Title.Female[20].Language"].ShouldBe(dtoFields["Title.Female[20].Language"]);
        spriggitFields["Title.Female[20].String"].ShouldBe(dtoFields["Title.Female[20].String"]);
        spriggitFields["Title.Female[21].Language"].ShouldBe(dtoFields["Title.Female[21].Language"]);
        spriggitFields["Title.Female[21].String"].ShouldBe(dtoFields["Title.Female[21].String"]);
        spriggitFields["Title.Female[22].Language"].ShouldBe(dtoFields["Title.Female[22].Language"]);
        spriggitFields["Title.Female[22].String"].ShouldBe(dtoFields["Title.Female[22].String"]);
        spriggitFields["Title.Female[23].Language"].ShouldBe(dtoFields["Title.Female[23].Language"]);
        spriggitFields["Title.Female[23].String"].ShouldBe(dtoFields["Title.Female[23].String"]);
        spriggitFields["Title.Female[24].Language"].ShouldBe(dtoFields["Title.Female[24].Language"]);
        spriggitFields["Title.Female[24].String"].ShouldBe(dtoFields["Title.Female[24].String"]);
        spriggitFields["Title.Female[25].Language"].ShouldBe(dtoFields["Title.Female[25].Language"]);
        spriggitFields["Title.Female[25].String"].ShouldBe(dtoFields["Title.Female[25].String"]);
        spriggitFields["Title.Female[26].Language"].ShouldBe(dtoFields["Title.Female[26].Language"]);
        spriggitFields["Title.Female[26].String"].ShouldBe(dtoFields["Title.Female[26].String"]);
        spriggitFields["Title.Female[27].Language"].ShouldBe(dtoFields["Title.Female[27].Language"]);
        spriggitFields["Title.Female[27].String"].ShouldBe(dtoFields["Title.Female[27].String"]);
        spriggitFields["Title.Female[28].Language"].ShouldBe(dtoFields["Title.Female[28].Language"]);
        spriggitFields["Title.Female[28].String"].ShouldBe(dtoFields["Title.Female[28].String"]);
        spriggitFields["Title.Female[29].Language"].ShouldBe(dtoFields["Title.Female[29].Language"]);
        spriggitFields["Title.Female[29].String"].ShouldBe(dtoFields["Title.Female[29].String"]);
        spriggitFields["Title.Female[3].Language"].ShouldBe(dtoFields["Title.Female[3].Language"]);
        spriggitFields["Title.Female[3].String"].ShouldBe(dtoFields["Title.Female[3].String"]);
        spriggitFields["Title.Female[30].Language"].ShouldBe(dtoFields["Title.Female[30].Language"]);
        spriggitFields["Title.Female[30].String"].ShouldBe(dtoFields["Title.Female[30].String"]);
        spriggitFields["Title.Female[31].Language"].ShouldBe(dtoFields["Title.Female[31].Language"]);
        spriggitFields["Title.Female[31].String"].ShouldBe(dtoFields["Title.Female[31].String"]);
        spriggitFields["Title.Female[32].Language"].ShouldBe(dtoFields["Title.Female[32].Language"]);
        spriggitFields["Title.Female[32].String"].ShouldBe(dtoFields["Title.Female[32].String"]);
        spriggitFields["Title.Female[33].Language"].ShouldBe(dtoFields["Title.Female[33].Language"]);
        spriggitFields["Title.Female[33].String"].ShouldBe(dtoFields["Title.Female[33].String"]);
        spriggitFields["Title.Female[34].Language"].ShouldBe(dtoFields["Title.Female[34].Language"]);
        spriggitFields["Title.Female[34].String"].ShouldBe(dtoFields["Title.Female[34].String"]);
        spriggitFields["Title.Female[35].Language"].ShouldBe(dtoFields["Title.Female[35].Language"]);
        spriggitFields["Title.Female[35].String"].ShouldBe(dtoFields["Title.Female[35].String"]);
        spriggitFields["Title.Female[4].Language"].ShouldBe(dtoFields["Title.Female[4].Language"]);
        spriggitFields["Title.Female[4].String"].ShouldBe(dtoFields["Title.Female[4].String"]);
        spriggitFields["Title.Female[5].Language"].ShouldBe(dtoFields["Title.Female[5].Language"]);
        spriggitFields["Title.Female[5].String"].ShouldBe(dtoFields["Title.Female[5].String"]);
        spriggitFields["Title.Female[6].Language"].ShouldBe(dtoFields["Title.Female[6].Language"]);
        spriggitFields["Title.Female[6].String"].ShouldBe(dtoFields["Title.Female[6].String"]);
        spriggitFields["Title.Female[7].Language"].ShouldBe(dtoFields["Title.Female[7].Language"]);
        spriggitFields["Title.Female[7].String"].ShouldBe(dtoFields["Title.Female[7].String"]);
        spriggitFields["Title.Female[8].Language"].ShouldBe(dtoFields["Title.Female[8].Language"]);
        spriggitFields["Title.Female[8].String"].ShouldBe(dtoFields["Title.Female[8].String"]);
        spriggitFields["Title.Female[9].Language"].ShouldBe(dtoFields["Title.Female[9].Language"]);
        spriggitFields["Title.Female[9].String"].ShouldBe(dtoFields["Title.Female[9].String"]);
        spriggitFields["Title.Male.Count"].ShouldBe(dtoFields["Title.Male.Count"]);
        spriggitFields["Title.Male.TargetLanguage[0]"].ShouldBe(dtoFields["Title.Male.TargetLanguage[0]"]);
        spriggitFields["Title.Male.TargetLanguage[1]"].ShouldBe(dtoFields["Title.Male.TargetLanguage[1]"]);
        spriggitFields["Title.Male.TargetLanguage[2]"].ShouldBe(dtoFields["Title.Male.TargetLanguage[2]"]);
        spriggitFields["Title.Male.TargetLanguage[3]"].ShouldBe(dtoFields["Title.Male.TargetLanguage[3]"]);
        spriggitFields["Title.Male[0].Language"].ShouldBe(dtoFields["Title.Male[0].Language"]);
        spriggitFields["Title.Male[0].String"].ShouldBe(dtoFields["Title.Male[0].String"]);
        spriggitFields["Title.Male[1].Language"].ShouldBe(dtoFields["Title.Male[1].Language"]);
        spriggitFields["Title.Male[1].String"].ShouldBe(dtoFields["Title.Male[1].String"]);
        spriggitFields["Title.Male[10].Language"].ShouldBe(dtoFields["Title.Male[10].Language"]);
        spriggitFields["Title.Male[10].String"].ShouldBe(dtoFields["Title.Male[10].String"]);
        spriggitFields["Title.Male[11].Language"].ShouldBe(dtoFields["Title.Male[11].Language"]);
        spriggitFields["Title.Male[11].String"].ShouldBe(dtoFields["Title.Male[11].String"]);
        spriggitFields["Title.Male[12].Language"].ShouldBe(dtoFields["Title.Male[12].Language"]);
        spriggitFields["Title.Male[12].String"].ShouldBe(dtoFields["Title.Male[12].String"]);
        spriggitFields["Title.Male[13].Language"].ShouldBe(dtoFields["Title.Male[13].Language"]);
        spriggitFields["Title.Male[13].String"].ShouldBe(dtoFields["Title.Male[13].String"]);
        spriggitFields["Title.Male[14].Language"].ShouldBe(dtoFields["Title.Male[14].Language"]);
        spriggitFields["Title.Male[14].String"].ShouldBe(dtoFields["Title.Male[14].String"]);
        spriggitFields["Title.Male[15].Language"].ShouldBe(dtoFields["Title.Male[15].Language"]);
        spriggitFields["Title.Male[15].String"].ShouldBe(dtoFields["Title.Male[15].String"]);
        spriggitFields["Title.Male[16].Language"].ShouldBe(dtoFields["Title.Male[16].Language"]);
        spriggitFields["Title.Male[16].String"].ShouldBe(dtoFields["Title.Male[16].String"]);
        spriggitFields["Title.Male[17].Language"].ShouldBe(dtoFields["Title.Male[17].Language"]);
        spriggitFields["Title.Male[17].String"].ShouldBe(dtoFields["Title.Male[17].String"]);
        spriggitFields["Title.Male[18].Language"].ShouldBe(dtoFields["Title.Male[18].Language"]);
        spriggitFields["Title.Male[18].String"].ShouldBe(dtoFields["Title.Male[18].String"]);
        spriggitFields["Title.Male[19].Language"].ShouldBe(dtoFields["Title.Male[19].Language"]);
        spriggitFields["Title.Male[19].String"].ShouldBe(dtoFields["Title.Male[19].String"]);
        spriggitFields["Title.Male[2].Language"].ShouldBe(dtoFields["Title.Male[2].Language"]);
        spriggitFields["Title.Male[2].String"].ShouldBe(dtoFields["Title.Male[2].String"]);
        spriggitFields["Title.Male[20].Language"].ShouldBe(dtoFields["Title.Male[20].Language"]);
        spriggitFields["Title.Male[20].String"].ShouldBe(dtoFields["Title.Male[20].String"]);
        spriggitFields["Title.Male[21].Language"].ShouldBe(dtoFields["Title.Male[21].Language"]);
        spriggitFields["Title.Male[21].String"].ShouldBe(dtoFields["Title.Male[21].String"]);
        spriggitFields["Title.Male[22].Language"].ShouldBe(dtoFields["Title.Male[22].Language"]);
        spriggitFields["Title.Male[22].String"].ShouldBe(dtoFields["Title.Male[22].String"]);
        spriggitFields["Title.Male[23].Language"].ShouldBe(dtoFields["Title.Male[23].Language"]);
        spriggitFields["Title.Male[23].String"].ShouldBe(dtoFields["Title.Male[23].String"]);
        spriggitFields["Title.Male[24].Language"].ShouldBe(dtoFields["Title.Male[24].Language"]);
        spriggitFields["Title.Male[24].String"].ShouldBe(dtoFields["Title.Male[24].String"]);
        spriggitFields["Title.Male[25].Language"].ShouldBe(dtoFields["Title.Male[25].Language"]);
        spriggitFields["Title.Male[25].String"].ShouldBe(dtoFields["Title.Male[25].String"]);
        spriggitFields["Title.Male[26].Language"].ShouldBe(dtoFields["Title.Male[26].Language"]);
        spriggitFields["Title.Male[26].String"].ShouldBe(dtoFields["Title.Male[26].String"]);
        spriggitFields["Title.Male[27].Language"].ShouldBe(dtoFields["Title.Male[27].Language"]);
        spriggitFields["Title.Male[27].String"].ShouldBe(dtoFields["Title.Male[27].String"]);
        spriggitFields["Title.Male[28].Language"].ShouldBe(dtoFields["Title.Male[28].Language"]);
        spriggitFields["Title.Male[28].String"].ShouldBe(dtoFields["Title.Male[28].String"]);
        spriggitFields["Title.Male[29].Language"].ShouldBe(dtoFields["Title.Male[29].Language"]);
        spriggitFields["Title.Male[29].String"].ShouldBe(dtoFields["Title.Male[29].String"]);
        spriggitFields["Title.Male[3].Language"].ShouldBe(dtoFields["Title.Male[3].Language"]);
        spriggitFields["Title.Male[3].String"].ShouldBe(dtoFields["Title.Male[3].String"]);
        spriggitFields["Title.Male[30].Language"].ShouldBe(dtoFields["Title.Male[30].Language"]);
        spriggitFields["Title.Male[30].String"].ShouldBe(dtoFields["Title.Male[30].String"]);
        spriggitFields["Title.Male[31].Language"].ShouldBe(dtoFields["Title.Male[31].Language"]);
        spriggitFields["Title.Male[31].String"].ShouldBe(dtoFields["Title.Male[31].String"]);
        spriggitFields["Title.Male[32].Language"].ShouldBe(dtoFields["Title.Male[32].Language"]);
        spriggitFields["Title.Male[32].String"].ShouldBe(dtoFields["Title.Male[32].String"]);
        spriggitFields["Title.Male[33].Language"].ShouldBe(dtoFields["Title.Male[33].Language"]);
        spriggitFields["Title.Male[33].String"].ShouldBe(dtoFields["Title.Male[33].String"]);
        spriggitFields["Title.Male[34].Language"].ShouldBe(dtoFields["Title.Male[34].Language"]);
        spriggitFields["Title.Male[34].String"].ShouldBe(dtoFields["Title.Male[34].String"]);
        spriggitFields["Title.Male[35].Language"].ShouldBe(dtoFields["Title.Male[35].Language"]);
        spriggitFields["Title.Male[35].String"].ShouldBe(dtoFields["Title.Male[35].String"]);
        spriggitFields["Title.Male[4].Language"].ShouldBe(dtoFields["Title.Male[4].Language"]);
        spriggitFields["Title.Male[4].String"].ShouldBe(dtoFields["Title.Male[4].String"]);
        spriggitFields["Title.Male[5].Language"].ShouldBe(dtoFields["Title.Male[5].Language"]);
        spriggitFields["Title.Male[5].String"].ShouldBe(dtoFields["Title.Male[5].String"]);
        spriggitFields["Title.Male[6].Language"].ShouldBe(dtoFields["Title.Male[6].Language"]);
        spriggitFields["Title.Male[6].String"].ShouldBe(dtoFields["Title.Male[6].String"]);
        spriggitFields["Title.Male[7].Language"].ShouldBe(dtoFields["Title.Male[7].Language"]);
        spriggitFields["Title.Male[7].String"].ShouldBe(dtoFields["Title.Male[7].String"]);
        spriggitFields["Title.Male[8].Language"].ShouldBe(dtoFields["Title.Male[8].Language"]);
        spriggitFields["Title.Male[8].String"].ShouldBe(dtoFields["Title.Male[8].String"]);
        spriggitFields["Title.Male[9].Language"].ShouldBe(dtoFields["Title.Male[9].Language"]);
        spriggitFields["Title.Male[9].String"].ShouldBe(dtoFields["Title.Male[9].String"]);
        spriggitFields["VendorValues"].ShouldBe(dtoFields["VendorValues"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "FACT")]
    [Trait("FormKey", "0FFD65:Skyrim.esm")]
    [Trait("EditorID", "DBSancBabetteBedFaction")]
    [Trait("SpriggitFile", "Factions/DBSancBabetteBedFaction - 0FFD65_Skyrim.esm.yaml")]
    public void Skyrim_FACT_ShouldMatchSpriggitSample_DBSancBabetteBedFaction()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Faction,
            "DBSancBabetteBedFaction");
        var dto = Helpers.GetDTO<FactionDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Faction,
            "0FFD65:Skyrim.esm");

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
        spriggitFields["Reaction"].ShouldBe(dtoFields["Reaction"]);
        spriggitFields["VendorBuySellList"].ShouldBe(dtoFields["VendorBuySellListFormKey"]);
        spriggitFields["VendorLocation.Target.MutagenObjectType"].ShouldBe(dtoFields["VendorLocation.Target.MutagenObjectType"]);
        spriggitFields["VendorLocation.Target.Type"].ShouldBe(dtoFields["VendorLocation.Target.Type"]);
        spriggitFields["VendorValues.EndHour"].ShouldBe(dtoFields["VendorValues.EndHour"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "FACT")]
    [Trait("FormKey", "040B60:Skyrim.esm")]
    [Trait("EditorID", "ArenaFaction")]
    [Trait("SpriggitFile", "Factions/ArenaFaction - 040B60_Skyrim.esm.yaml")]
    public void Skyrim_FACT_ShouldMatchSpriggitSample_ArenaFaction()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Faction,
            "ArenaFaction");
        var dto = Helpers.GetDTO<FactionDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Faction,
            "040B60:Skyrim.esm");

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
        spriggitFields["Title.Female[0]"].ShouldBe(dtoFields["Title.Female[0]"]);
        spriggitFields["Title.Female[1]"].ShouldBe(dtoFields["Title.Female[1]"]);
        spriggitFields["Title.Female[2]"].ShouldBe(dtoFields["Title.Female[2]"]);
        spriggitFields["Title.Female[3]"].ShouldBe(dtoFields["Title.Female[3]"]);
        spriggitFields["Title.Female[4]"].ShouldBe(dtoFields["Title.Female[4]"]);
        spriggitFields["Title.Male.Count"].ShouldBe(dtoFields["Title.Male.Count"]);
        spriggitFields["Title.Male.TargetLanguage[0]"].ShouldBe(dtoFields["Title.Male.TargetLanguage[0]"]);
        spriggitFields["Title.Male.TargetLanguage[1]"].ShouldBe(dtoFields["Title.Male.TargetLanguage[1]"]);
        spriggitFields["Title.Male.TargetLanguage[2]"].ShouldBe(dtoFields["Title.Male.TargetLanguage[2]"]);
        spriggitFields["Title.Male.TargetLanguage[3]"].ShouldBe(dtoFields["Title.Male.TargetLanguage[3]"]);
        spriggitFields["Title.Male.TargetLanguage[4]"].ShouldBe(dtoFields["Title.Male.TargetLanguage[4]"]);
        spriggitFields["Title.Male[0].Language"].ShouldBe(dtoFields["Title.Male[0].Language"]);
        spriggitFields["Title.Male[0].String"].ShouldBe(dtoFields["Title.Male[0].String"]);
        spriggitFields["Title.Male[1].Language"].ShouldBe(dtoFields["Title.Male[1].Language"]);
        spriggitFields["Title.Male[1].String"].ShouldBe(dtoFields["Title.Male[1].String"]);
        spriggitFields["Title.Male[10].Language"].ShouldBe(dtoFields["Title.Male[10].Language"]);
        spriggitFields["Title.Male[10].String"].ShouldBe(dtoFields["Title.Male[10].String"]);
        spriggitFields["Title.Male[11].Language"].ShouldBe(dtoFields["Title.Male[11].Language"]);
        spriggitFields["Title.Male[11].String"].ShouldBe(dtoFields["Title.Male[11].String"]);
        spriggitFields["Title.Male[12].Language"].ShouldBe(dtoFields["Title.Male[12].Language"]);
        spriggitFields["Title.Male[12].String"].ShouldBe(dtoFields["Title.Male[12].String"]);
        spriggitFields["Title.Male[13].Language"].ShouldBe(dtoFields["Title.Male[13].Language"]);
        spriggitFields["Title.Male[13].String"].ShouldBe(dtoFields["Title.Male[13].String"]);
        spriggitFields["Title.Male[14].Language"].ShouldBe(dtoFields["Title.Male[14].Language"]);
        spriggitFields["Title.Male[14].String"].ShouldBe(dtoFields["Title.Male[14].String"]);
        spriggitFields["Title.Male[15].Language"].ShouldBe(dtoFields["Title.Male[15].Language"]);
        spriggitFields["Title.Male[15].String"].ShouldBe(dtoFields["Title.Male[15].String"]);
        spriggitFields["Title.Male[16].Language"].ShouldBe(dtoFields["Title.Male[16].Language"]);
        spriggitFields["Title.Male[16].String"].ShouldBe(dtoFields["Title.Male[16].String"]);
        spriggitFields["Title.Male[17].Language"].ShouldBe(dtoFields["Title.Male[17].Language"]);
        spriggitFields["Title.Male[17].String"].ShouldBe(dtoFields["Title.Male[17].String"]);
        spriggitFields["Title.Male[18].Language"].ShouldBe(dtoFields["Title.Male[18].Language"]);
        spriggitFields["Title.Male[18].String"].ShouldBe(dtoFields["Title.Male[18].String"]);
        spriggitFields["Title.Male[19].Language"].ShouldBe(dtoFields["Title.Male[19].Language"]);
        spriggitFields["Title.Male[19].String"].ShouldBe(dtoFields["Title.Male[19].String"]);
        spriggitFields["Title.Male[2].Language"].ShouldBe(dtoFields["Title.Male[2].Language"]);
        spriggitFields["Title.Male[2].String"].ShouldBe(dtoFields["Title.Male[2].String"]);
        spriggitFields["Title.Male[20].Language"].ShouldBe(dtoFields["Title.Male[20].Language"]);
        spriggitFields["Title.Male[20].String"].ShouldBe(dtoFields["Title.Male[20].String"]);
        spriggitFields["Title.Male[21].Language"].ShouldBe(dtoFields["Title.Male[21].Language"]);
        spriggitFields["Title.Male[21].String"].ShouldBe(dtoFields["Title.Male[21].String"]);
        spriggitFields["Title.Male[22].Language"].ShouldBe(dtoFields["Title.Male[22].Language"]);
        spriggitFields["Title.Male[22].String"].ShouldBe(dtoFields["Title.Male[22].String"]);
        spriggitFields["Title.Male[23].Language"].ShouldBe(dtoFields["Title.Male[23].Language"]);
        spriggitFields["Title.Male[23].String"].ShouldBe(dtoFields["Title.Male[23].String"]);
        spriggitFields["Title.Male[24].Language"].ShouldBe(dtoFields["Title.Male[24].Language"]);
        spriggitFields["Title.Male[24].String"].ShouldBe(dtoFields["Title.Male[24].String"]);
        spriggitFields["Title.Male[25].Language"].ShouldBe(dtoFields["Title.Male[25].Language"]);
        spriggitFields["Title.Male[25].String"].ShouldBe(dtoFields["Title.Male[25].String"]);
        spriggitFields["Title.Male[26].Language"].ShouldBe(dtoFields["Title.Male[26].Language"]);
        spriggitFields["Title.Male[26].String"].ShouldBe(dtoFields["Title.Male[26].String"]);
        spriggitFields["Title.Male[27].Language"].ShouldBe(dtoFields["Title.Male[27].Language"]);
        spriggitFields["Title.Male[27].String"].ShouldBe(dtoFields["Title.Male[27].String"]);
        spriggitFields["Title.Male[28].Language"].ShouldBe(dtoFields["Title.Male[28].Language"]);
        spriggitFields["Title.Male[28].String"].ShouldBe(dtoFields["Title.Male[28].String"]);
        spriggitFields["Title.Male[29].Language"].ShouldBe(dtoFields["Title.Male[29].Language"]);
        spriggitFields["Title.Male[29].String"].ShouldBe(dtoFields["Title.Male[29].String"]);
        spriggitFields["Title.Male[3].Language"].ShouldBe(dtoFields["Title.Male[3].Language"]);
        spriggitFields["Title.Male[3].String"].ShouldBe(dtoFields["Title.Male[3].String"]);
        spriggitFields["Title.Male[30].Language"].ShouldBe(dtoFields["Title.Male[30].Language"]);
        spriggitFields["Title.Male[30].String"].ShouldBe(dtoFields["Title.Male[30].String"]);
        spriggitFields["Title.Male[31].Language"].ShouldBe(dtoFields["Title.Male[31].Language"]);
        spriggitFields["Title.Male[31].String"].ShouldBe(dtoFields["Title.Male[31].String"]);
        spriggitFields["Title.Male[32].Language"].ShouldBe(dtoFields["Title.Male[32].Language"]);
        spriggitFields["Title.Male[32].String"].ShouldBe(dtoFields["Title.Male[32].String"]);
        spriggitFields["Title.Male[33].Language"].ShouldBe(dtoFields["Title.Male[33].Language"]);
        spriggitFields["Title.Male[33].String"].ShouldBe(dtoFields["Title.Male[33].String"]);
        spriggitFields["Title.Male[34].Language"].ShouldBe(dtoFields["Title.Male[34].Language"]);
        spriggitFields["Title.Male[34].String"].ShouldBe(dtoFields["Title.Male[34].String"]);
        spriggitFields["Title.Male[35].Language"].ShouldBe(dtoFields["Title.Male[35].Language"]);
        spriggitFields["Title.Male[35].String"].ShouldBe(dtoFields["Title.Male[35].String"]);
        spriggitFields["Title.Male[36].Language"].ShouldBe(dtoFields["Title.Male[36].Language"]);
        spriggitFields["Title.Male[36].String"].ShouldBe(dtoFields["Title.Male[36].String"]);
        spriggitFields["Title.Male[37].Language"].ShouldBe(dtoFields["Title.Male[37].Language"]);
        spriggitFields["Title.Male[37].String"].ShouldBe(dtoFields["Title.Male[37].String"]);
        spriggitFields["Title.Male[38].Language"].ShouldBe(dtoFields["Title.Male[38].Language"]);
        spriggitFields["Title.Male[38].String"].ShouldBe(dtoFields["Title.Male[38].String"]);
        spriggitFields["Title.Male[39].Language"].ShouldBe(dtoFields["Title.Male[39].Language"]);
        spriggitFields["Title.Male[39].String"].ShouldBe(dtoFields["Title.Male[39].String"]);
        spriggitFields["Title.Male[4].Language"].ShouldBe(dtoFields["Title.Male[4].Language"]);
        spriggitFields["Title.Male[4].String"].ShouldBe(dtoFields["Title.Male[4].String"]);
        spriggitFields["Title.Male[40].Language"].ShouldBe(dtoFields["Title.Male[40].Language"]);
        spriggitFields["Title.Male[40].String"].ShouldBe(dtoFields["Title.Male[40].String"]);
        spriggitFields["Title.Male[41].Language"].ShouldBe(dtoFields["Title.Male[41].Language"]);
        spriggitFields["Title.Male[41].String"].ShouldBe(dtoFields["Title.Male[41].String"]);
        spriggitFields["Title.Male[42].Language"].ShouldBe(dtoFields["Title.Male[42].Language"]);
        spriggitFields["Title.Male[42].String"].ShouldBe(dtoFields["Title.Male[42].String"]);
        spriggitFields["Title.Male[43].Language"].ShouldBe(dtoFields["Title.Male[43].Language"]);
        spriggitFields["Title.Male[43].String"].ShouldBe(dtoFields["Title.Male[43].String"]);
        spriggitFields["Title.Male[44].Language"].ShouldBe(dtoFields["Title.Male[44].Language"]);
        spriggitFields["Title.Male[44].String"].ShouldBe(dtoFields["Title.Male[44].String"]);
        spriggitFields["Title.Male[5].Language"].ShouldBe(dtoFields["Title.Male[5].Language"]);
        spriggitFields["Title.Male[5].String"].ShouldBe(dtoFields["Title.Male[5].String"]);
        spriggitFields["Title.Male[6].Language"].ShouldBe(dtoFields["Title.Male[6].Language"]);
        spriggitFields["Title.Male[6].String"].ShouldBe(dtoFields["Title.Male[6].String"]);
        spriggitFields["Title.Male[7].Language"].ShouldBe(dtoFields["Title.Male[7].Language"]);
        spriggitFields["Title.Male[7].String"].ShouldBe(dtoFields["Title.Male[7].String"]);
        spriggitFields["Title.Male[8].Language"].ShouldBe(dtoFields["Title.Male[8].Language"]);
        spriggitFields["Title.Male[8].String"].ShouldBe(dtoFields["Title.Male[8].String"]);
        spriggitFields["Title.Male[9].Language"].ShouldBe(dtoFields["Title.Male[9].Language"]);
        spriggitFields["Title.Male[9].String"].ShouldBe(dtoFields["Title.Male[9].String"]);
        spriggitFields["VendorValues"].ShouldBe(dtoFields["VendorValues"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }
}
