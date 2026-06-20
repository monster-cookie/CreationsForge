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
        Helpers.GetSpriggitField(spriggit, "Title.Female[0]").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[0]"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[1]").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[1]"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[2]").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[2]"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[3]").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[3]"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[4]").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[4]"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[5]").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[5]"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[6]").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[6]"));
        Helpers.GetSpriggitField(spriggit, "Title.Male.Count").ShouldBe(Helpers.GetDTOField(dto, "Title.Male.Count"));
        Helpers.GetSpriggitField(spriggit, "Title.Male.TargetLanguage[0]").ShouldBe(Helpers.GetDTOField(dto, "Title.Male.TargetLanguage[0]"));
        Helpers.GetSpriggitField(spriggit, "Title.Male.TargetLanguage[1]").ShouldBe(Helpers.GetDTOField(dto, "Title.Male.TargetLanguage[1]"));
        Helpers.GetSpriggitField(spriggit, "Title.Male.TargetLanguage[2]").ShouldBe(Helpers.GetDTOField(dto, "Title.Male.TargetLanguage[2]"));
        Helpers.GetSpriggitField(spriggit, "Title.Male.TargetLanguage[3]").ShouldBe(Helpers.GetDTOField(dto, "Title.Male.TargetLanguage[3]"));
        Helpers.GetSpriggitField(spriggit, "Title.Male.TargetLanguage[4]").ShouldBe(Helpers.GetDTOField(dto, "Title.Male.TargetLanguage[4]"));
        Helpers.GetSpriggitField(spriggit, "Title.Male.TargetLanguage[5]").ShouldBe(Helpers.GetDTOField(dto, "Title.Male.TargetLanguage[5]"));
        Helpers.GetSpriggitField(spriggit, "Title.Male.TargetLanguage[6]").ShouldBe(Helpers.GetDTOField(dto, "Title.Male.TargetLanguage[6]"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[0].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[0].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[1].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[1].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[10].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[10].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[10].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[10].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[11].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[11].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[11].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[11].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[12].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[12].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[12].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[12].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[13].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[13].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[13].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[13].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[14].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[14].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[14].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[14].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[15].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[15].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[15].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[15].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[16].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[16].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[16].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[16].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[17].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[17].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[17].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[17].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[18].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[18].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[18].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[18].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[19].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[19].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[19].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[19].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[2].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[2].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[2].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[2].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[20].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[20].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[20].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[20].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[21].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[21].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[21].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[21].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[22].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[22].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[22].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[22].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[23].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[23].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[23].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[23].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[24].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[24].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[24].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[24].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[25].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[25].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[25].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[25].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[26].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[26].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[26].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[26].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[27].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[27].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[27].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[27].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[28].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[28].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[28].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[28].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[29].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[29].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[29].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[29].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[3].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[3].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[3].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[3].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[30].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[30].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[30].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[30].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[31].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[31].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[31].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[31].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[32].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[32].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[32].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[32].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[33].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[33].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[33].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[33].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[34].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[34].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[34].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[34].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[35].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[35].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[35].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[35].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[36].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[36].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[36].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[36].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[37].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[37].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[37].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[37].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[38].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[38].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[38].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[38].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[39].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[39].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[39].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[39].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[4].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[4].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[4].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[4].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[40].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[40].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[40].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[40].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[41].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[41].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[41].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[41].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[42].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[42].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[42].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[42].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[43].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[43].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[43].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[43].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[44].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[44].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[44].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[44].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[45].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[45].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[45].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[45].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[46].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[46].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[46].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[46].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[47].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[47].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[47].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[47].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[48].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[48].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[48].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[48].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[49].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[49].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[49].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[49].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[5].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[5].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[5].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[5].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[50].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[50].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[50].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[50].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[51].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[51].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[51].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[51].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[52].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[52].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[52].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[52].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[53].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[53].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[53].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[53].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[54].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[54].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[54].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[54].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[55].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[55].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[55].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[55].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[56].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[56].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[56].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[56].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[57].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[57].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[57].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[57].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[58].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[58].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[58].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[58].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[59].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[59].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[59].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[59].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[6].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[6].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[6].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[6].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[60].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[60].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[60].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[60].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[61].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[61].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[61].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[61].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[62].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[62].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[62].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[62].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[7].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[7].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[7].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[7].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[8].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[8].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[8].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[8].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[9].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[9].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[9].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[9].String"));
        Helpers.GetSpriggitField(spriggit, "VendorValues").ShouldBe(Helpers.GetDTOField(dto, "VendorValues"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "CrimeValues.Arrest", "CrimeValues.AttackOnSight", "EditorID", "FormKey", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Reaction[0]", "Reaction[1]", "Title.Female[0]", "Title.Female[1]", "Title.Female[2]", "Title.Female[3]", "Title.Female[4]", "Title.Female[5]", "Title.Female[6]", "Title.Male.Count", "Title.Male.TargetLanguage[0]", "Title.Male.TargetLanguage[1]", "Title.Male.TargetLanguage[2]", "Title.Male.TargetLanguage[3]", "Title.Male.TargetLanguage[4]", "Title.Male.TargetLanguage[5]", "Title.Male.TargetLanguage[6]", "Title.Male[0].Language", "Title.Male[0].String", "Title.Male[1].Language", "Title.Male[1].String", "Title.Male[10].Language", "Title.Male[10].String", "Title.Male[11].Language", "Title.Male[11].String", "Title.Male[12].Language", "Title.Male[12].String", "Title.Male[13].Language", "Title.Male[13].String", "Title.Male[14].Language", "Title.Male[14].String", "Title.Male[15].Language", "Title.Male[15].String", "Title.Male[16].Language", "Title.Male[16].String", "Title.Male[17].Language", "Title.Male[17].String", "Title.Male[18].Language", "Title.Male[18].String", "Title.Male[19].Language", "Title.Male[19].String", "Title.Male[2].Language", "Title.Male[2].String", "Title.Male[20].Language", "Title.Male[20].String", "Title.Male[21].Language", "Title.Male[21].String", "Title.Male[22].Language", "Title.Male[22].String", "Title.Male[23].Language", "Title.Male[23].String", "Title.Male[24].Language", "Title.Male[24].String", "Title.Male[25].Language", "Title.Male[25].String", "Title.Male[26].Language", "Title.Male[26].String", "Title.Male[27].Language", "Title.Male[27].String", "Title.Male[28].Language", "Title.Male[28].String", "Title.Male[29].Language", "Title.Male[29].String", "Title.Male[3].Language", "Title.Male[3].String", "Title.Male[30].Language", "Title.Male[30].String", "Title.Male[31].Language", "Title.Male[31].String", "Title.Male[32].Language", "Title.Male[32].String", "Title.Male[33].Language", "Title.Male[33].String", "Title.Male[34].Language", "Title.Male[34].String", "Title.Male[35].Language", "Title.Male[35].String", "Title.Male[36].Language", "Title.Male[36].String", "Title.Male[37].Language", "Title.Male[37].String", "Title.Male[38].Language", "Title.Male[38].String", "Title.Male[39].Language", "Title.Male[39].String", "Title.Male[4].Language", "Title.Male[4].String", "Title.Male[40].Language", "Title.Male[40].String", "Title.Male[41].Language", "Title.Male[41].String", "Title.Male[42].Language", "Title.Male[42].String", "Title.Male[43].Language", "Title.Male[43].String", "Title.Male[44].Language", "Title.Male[44].String", "Title.Male[45].Language", "Title.Male[45].String", "Title.Male[46].Language", "Title.Male[46].String", "Title.Male[47].Language", "Title.Male[47].String", "Title.Male[48].Language", "Title.Male[48].String", "Title.Male[49].Language", "Title.Male[49].String", "Title.Male[5].Language", "Title.Male[5].String", "Title.Male[50].Language", "Title.Male[50].String", "Title.Male[51].Language", "Title.Male[51].String", "Title.Male[52].Language", "Title.Male[52].String", "Title.Male[53].Language", "Title.Male[53].String", "Title.Male[54].Language", "Title.Male[54].String", "Title.Male[55].Language", "Title.Male[55].String", "Title.Male[56].Language", "Title.Male[56].String", "Title.Male[57].Language", "Title.Male[57].String", "Title.Male[58].Language", "Title.Male[58].String", "Title.Male[59].Language", "Title.Male[59].String", "Title.Male[6].Language", "Title.Male[6].String", "Title.Male[60].Language", "Title.Male[60].String", "Title.Male[61].Language", "Title.Male[61].String", "Title.Male[62].Language", "Title.Male[62].String", "Title.Male[7].Language", "Title.Male[7].String", "Title.Male[8].Language", "Title.Male[8].String", "Title.Male[9].Language", "Title.Male[9].String", "VendorValues", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "CrimeValues.Arrest", "CrimeValues.AttackOnSight", "EditorID", "FormKey", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Reaction[0]", "Reaction[1]", "Title.Female[0]", "Title.Female[1]", "Title.Female[2]", "Title.Female[3]", "Title.Female[4]", "Title.Female[5]", "Title.Female[6]", "Title.Male.Count", "Title.Male.TargetLanguage[0]", "Title.Male.TargetLanguage[1]", "Title.Male.TargetLanguage[2]", "Title.Male.TargetLanguage[3]", "Title.Male.TargetLanguage[4]", "Title.Male.TargetLanguage[5]", "Title.Male.TargetLanguage[6]", "Title.Male[0].Language", "Title.Male[0].String", "Title.Male[1].Language", "Title.Male[1].String", "Title.Male[10].Language", "Title.Male[10].String", "Title.Male[11].Language", "Title.Male[11].String", "Title.Male[12].Language", "Title.Male[12].String", "Title.Male[13].Language", "Title.Male[13].String", "Title.Male[14].Language", "Title.Male[14].String", "Title.Male[15].Language", "Title.Male[15].String", "Title.Male[16].Language", "Title.Male[16].String", "Title.Male[17].Language", "Title.Male[17].String", "Title.Male[18].Language", "Title.Male[18].String", "Title.Male[19].Language", "Title.Male[19].String", "Title.Male[2].Language", "Title.Male[2].String", "Title.Male[20].Language", "Title.Male[20].String", "Title.Male[21].Language", "Title.Male[21].String", "Title.Male[22].Language", "Title.Male[22].String", "Title.Male[23].Language", "Title.Male[23].String", "Title.Male[24].Language", "Title.Male[24].String", "Title.Male[25].Language", "Title.Male[25].String", "Title.Male[26].Language", "Title.Male[26].String", "Title.Male[27].Language", "Title.Male[27].String", "Title.Male[28].Language", "Title.Male[28].String", "Title.Male[29].Language", "Title.Male[29].String", "Title.Male[3].Language", "Title.Male[3].String", "Title.Male[30].Language", "Title.Male[30].String", "Title.Male[31].Language", "Title.Male[31].String", "Title.Male[32].Language", "Title.Male[32].String", "Title.Male[33].Language", "Title.Male[33].String", "Title.Male[34].Language", "Title.Male[34].String", "Title.Male[35].Language", "Title.Male[35].String", "Title.Male[36].Language", "Title.Male[36].String", "Title.Male[37].Language", "Title.Male[37].String", "Title.Male[38].Language", "Title.Male[38].String", "Title.Male[39].Language", "Title.Male[39].String", "Title.Male[4].Language", "Title.Male[4].String", "Title.Male[40].Language", "Title.Male[40].String", "Title.Male[41].Language", "Title.Male[41].String", "Title.Male[42].Language", "Title.Male[42].String", "Title.Male[43].Language", "Title.Male[43].String", "Title.Male[44].Language", "Title.Male[44].String", "Title.Male[45].Language", "Title.Male[45].String", "Title.Male[46].Language", "Title.Male[46].String", "Title.Male[47].Language", "Title.Male[47].String", "Title.Male[48].Language", "Title.Male[48].String", "Title.Male[49].Language", "Title.Male[49].String", "Title.Male[5].Language", "Title.Male[5].String", "Title.Male[50].Language", "Title.Male[50].String", "Title.Male[51].Language", "Title.Male[51].String", "Title.Male[52].Language", "Title.Male[52].String", "Title.Male[53].Language", "Title.Male[53].String", "Title.Male[54].Language", "Title.Male[54].String", "Title.Male[55].Language", "Title.Male[55].String", "Title.Male[56].Language", "Title.Male[56].String", "Title.Male[57].Language", "Title.Male[57].String", "Title.Male[58].Language", "Title.Male[58].String", "Title.Male[59].Language", "Title.Male[59].String", "Title.Male[6].Language", "Title.Male[6].String", "Title.Male[60].Language", "Title.Male[60].String", "Title.Male[61].Language", "Title.Male[61].String", "Title.Male[62].Language", "Title.Male[62].String", "Title.Male[7].Language", "Title.Male[7].String", "Title.Male[8].Language", "Title.Male[8].String", "Title.Male[9].Language", "Title.Male[9].String", "VendorValues", "Version2", "VersionControl");
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
        Helpers.GetSpriggitField(spriggit, "Reaction[2]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[2]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[3]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[3]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[4]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[4]"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[0]").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[0]"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[1]").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[1]"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[2]").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[2]"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[3]").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[3]"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[4]").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[4]"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[5]").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[5]"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[6]").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[6]"));
        Helpers.GetSpriggitField(spriggit, "Title.Male.Count").ShouldBe(Helpers.GetDTOField(dto, "Title.Male.Count"));
        Helpers.GetSpriggitField(spriggit, "Title.Male.TargetLanguage[0]").ShouldBe(Helpers.GetDTOField(dto, "Title.Male.TargetLanguage[0]"));
        Helpers.GetSpriggitField(spriggit, "Title.Male.TargetLanguage[1]").ShouldBe(Helpers.GetDTOField(dto, "Title.Male.TargetLanguage[1]"));
        Helpers.GetSpriggitField(spriggit, "Title.Male.TargetLanguage[2]").ShouldBe(Helpers.GetDTOField(dto, "Title.Male.TargetLanguage[2]"));
        Helpers.GetSpriggitField(spriggit, "Title.Male.TargetLanguage[3]").ShouldBe(Helpers.GetDTOField(dto, "Title.Male.TargetLanguage[3]"));
        Helpers.GetSpriggitField(spriggit, "Title.Male.TargetLanguage[4]").ShouldBe(Helpers.GetDTOField(dto, "Title.Male.TargetLanguage[4]"));
        Helpers.GetSpriggitField(spriggit, "Title.Male.TargetLanguage[5]").ShouldBe(Helpers.GetDTOField(dto, "Title.Male.TargetLanguage[5]"));
        Helpers.GetSpriggitField(spriggit, "Title.Male.TargetLanguage[6]").ShouldBe(Helpers.GetDTOField(dto, "Title.Male.TargetLanguage[6]"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[0].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[0].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[1].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[1].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[10].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[10].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[10].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[10].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[11].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[11].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[11].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[11].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[12].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[12].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[12].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[12].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[13].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[13].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[13].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[13].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[14].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[14].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[14].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[14].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[15].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[15].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[15].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[15].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[16].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[16].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[16].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[16].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[17].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[17].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[17].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[17].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[18].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[18].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[18].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[18].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[19].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[19].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[19].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[19].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[2].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[2].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[2].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[2].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[20].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[20].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[20].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[20].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[21].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[21].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[21].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[21].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[22].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[22].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[22].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[22].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[23].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[23].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[23].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[23].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[24].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[24].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[24].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[24].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[25].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[25].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[25].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[25].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[26].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[26].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[26].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[26].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[27].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[27].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[27].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[27].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[28].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[28].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[28].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[28].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[29].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[29].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[29].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[29].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[3].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[3].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[3].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[3].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[30].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[30].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[30].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[30].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[31].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[31].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[31].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[31].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[32].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[32].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[32].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[32].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[33].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[33].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[33].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[33].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[34].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[34].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[34].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[34].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[35].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[35].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[35].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[35].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[36].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[36].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[36].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[36].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[37].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[37].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[37].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[37].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[38].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[38].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[38].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[38].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[39].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[39].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[39].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[39].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[4].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[4].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[4].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[4].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[40].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[40].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[40].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[40].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[41].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[41].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[41].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[41].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[42].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[42].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[42].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[42].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[43].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[43].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[43].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[43].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[44].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[44].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[44].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[44].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[45].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[45].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[45].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[45].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[46].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[46].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[46].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[46].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[47].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[47].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[47].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[47].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[48].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[48].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[48].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[48].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[49].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[49].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[49].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[49].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[5].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[5].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[5].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[5].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[50].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[50].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[50].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[50].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[51].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[51].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[51].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[51].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[52].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[52].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[52].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[52].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[53].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[53].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[53].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[53].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[54].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[54].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[54].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[54].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[55].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[55].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[55].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[55].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[56].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[56].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[56].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[56].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[57].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[57].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[57].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[57].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[58].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[58].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[58].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[58].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[59].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[59].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[59].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[59].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[6].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[6].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[6].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[6].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[60].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[60].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[60].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[60].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[61].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[61].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[61].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[61].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[62].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[62].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[62].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[62].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[7].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[7].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[7].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[7].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[8].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[8].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[8].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[8].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[9].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[9].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[9].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[9].String"));
        Helpers.GetSpriggitField(spriggit, "VendorValues").ShouldBe(Helpers.GetDTOField(dto, "VendorValues"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "CrimeValues.Arrest", "CrimeValues.AttackOnSight", "EditorID", "FormKey", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Reaction[0]", "Reaction[1]", "Reaction[2]", "Reaction[3]", "Reaction[4]", "Title.Female[0]", "Title.Female[1]", "Title.Female[2]", "Title.Female[3]", "Title.Female[4]", "Title.Female[5]", "Title.Female[6]", "Title.Male.Count", "Title.Male.TargetLanguage[0]", "Title.Male.TargetLanguage[1]", "Title.Male.TargetLanguage[2]", "Title.Male.TargetLanguage[3]", "Title.Male.TargetLanguage[4]", "Title.Male.TargetLanguage[5]", "Title.Male.TargetLanguage[6]", "Title.Male[0].Language", "Title.Male[0].String", "Title.Male[1].Language", "Title.Male[1].String", "Title.Male[10].Language", "Title.Male[10].String", "Title.Male[11].Language", "Title.Male[11].String", "Title.Male[12].Language", "Title.Male[12].String", "Title.Male[13].Language", "Title.Male[13].String", "Title.Male[14].Language", "Title.Male[14].String", "Title.Male[15].Language", "Title.Male[15].String", "Title.Male[16].Language", "Title.Male[16].String", "Title.Male[17].Language", "Title.Male[17].String", "Title.Male[18].Language", "Title.Male[18].String", "Title.Male[19].Language", "Title.Male[19].String", "Title.Male[2].Language", "Title.Male[2].String", "Title.Male[20].Language", "Title.Male[20].String", "Title.Male[21].Language", "Title.Male[21].String", "Title.Male[22].Language", "Title.Male[22].String", "Title.Male[23].Language", "Title.Male[23].String", "Title.Male[24].Language", "Title.Male[24].String", "Title.Male[25].Language", "Title.Male[25].String", "Title.Male[26].Language", "Title.Male[26].String", "Title.Male[27].Language", "Title.Male[27].String", "Title.Male[28].Language", "Title.Male[28].String", "Title.Male[29].Language", "Title.Male[29].String", "Title.Male[3].Language", "Title.Male[3].String", "Title.Male[30].Language", "Title.Male[30].String", "Title.Male[31].Language", "Title.Male[31].String", "Title.Male[32].Language", "Title.Male[32].String", "Title.Male[33].Language", "Title.Male[33].String", "Title.Male[34].Language", "Title.Male[34].String", "Title.Male[35].Language", "Title.Male[35].String", "Title.Male[36].Language", "Title.Male[36].String", "Title.Male[37].Language", "Title.Male[37].String", "Title.Male[38].Language", "Title.Male[38].String", "Title.Male[39].Language", "Title.Male[39].String", "Title.Male[4].Language", "Title.Male[4].String", "Title.Male[40].Language", "Title.Male[40].String", "Title.Male[41].Language", "Title.Male[41].String", "Title.Male[42].Language", "Title.Male[42].String", "Title.Male[43].Language", "Title.Male[43].String", "Title.Male[44].Language", "Title.Male[44].String", "Title.Male[45].Language", "Title.Male[45].String", "Title.Male[46].Language", "Title.Male[46].String", "Title.Male[47].Language", "Title.Male[47].String", "Title.Male[48].Language", "Title.Male[48].String", "Title.Male[49].Language", "Title.Male[49].String", "Title.Male[5].Language", "Title.Male[5].String", "Title.Male[50].Language", "Title.Male[50].String", "Title.Male[51].Language", "Title.Male[51].String", "Title.Male[52].Language", "Title.Male[52].String", "Title.Male[53].Language", "Title.Male[53].String", "Title.Male[54].Language", "Title.Male[54].String", "Title.Male[55].Language", "Title.Male[55].String", "Title.Male[56].Language", "Title.Male[56].String", "Title.Male[57].Language", "Title.Male[57].String", "Title.Male[58].Language", "Title.Male[58].String", "Title.Male[59].Language", "Title.Male[59].String", "Title.Male[6].Language", "Title.Male[6].String", "Title.Male[60].Language", "Title.Male[60].String", "Title.Male[61].Language", "Title.Male[61].String", "Title.Male[62].Language", "Title.Male[62].String", "Title.Male[7].Language", "Title.Male[7].String", "Title.Male[8].Language", "Title.Male[8].String", "Title.Male[9].Language", "Title.Male[9].String", "VendorValues", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "CrimeValues.Arrest", "CrimeValues.AttackOnSight", "EditorID", "FormKey", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Reaction[0]", "Reaction[1]", "Reaction[2]", "Reaction[3]", "Reaction[4]", "Title.Female[0]", "Title.Female[1]", "Title.Female[2]", "Title.Female[3]", "Title.Female[4]", "Title.Female[5]", "Title.Female[6]", "Title.Male.Count", "Title.Male.TargetLanguage[0]", "Title.Male.TargetLanguage[1]", "Title.Male.TargetLanguage[2]", "Title.Male.TargetLanguage[3]", "Title.Male.TargetLanguage[4]", "Title.Male.TargetLanguage[5]", "Title.Male.TargetLanguage[6]", "Title.Male[0].Language", "Title.Male[0].String", "Title.Male[1].Language", "Title.Male[1].String", "Title.Male[10].Language", "Title.Male[10].String", "Title.Male[11].Language", "Title.Male[11].String", "Title.Male[12].Language", "Title.Male[12].String", "Title.Male[13].Language", "Title.Male[13].String", "Title.Male[14].Language", "Title.Male[14].String", "Title.Male[15].Language", "Title.Male[15].String", "Title.Male[16].Language", "Title.Male[16].String", "Title.Male[17].Language", "Title.Male[17].String", "Title.Male[18].Language", "Title.Male[18].String", "Title.Male[19].Language", "Title.Male[19].String", "Title.Male[2].Language", "Title.Male[2].String", "Title.Male[20].Language", "Title.Male[20].String", "Title.Male[21].Language", "Title.Male[21].String", "Title.Male[22].Language", "Title.Male[22].String", "Title.Male[23].Language", "Title.Male[23].String", "Title.Male[24].Language", "Title.Male[24].String", "Title.Male[25].Language", "Title.Male[25].String", "Title.Male[26].Language", "Title.Male[26].String", "Title.Male[27].Language", "Title.Male[27].String", "Title.Male[28].Language", "Title.Male[28].String", "Title.Male[29].Language", "Title.Male[29].String", "Title.Male[3].Language", "Title.Male[3].String", "Title.Male[30].Language", "Title.Male[30].String", "Title.Male[31].Language", "Title.Male[31].String", "Title.Male[32].Language", "Title.Male[32].String", "Title.Male[33].Language", "Title.Male[33].String", "Title.Male[34].Language", "Title.Male[34].String", "Title.Male[35].Language", "Title.Male[35].String", "Title.Male[36].Language", "Title.Male[36].String", "Title.Male[37].Language", "Title.Male[37].String", "Title.Male[38].Language", "Title.Male[38].String", "Title.Male[39].Language", "Title.Male[39].String", "Title.Male[4].Language", "Title.Male[4].String", "Title.Male[40].Language", "Title.Male[40].String", "Title.Male[41].Language", "Title.Male[41].String", "Title.Male[42].Language", "Title.Male[42].String", "Title.Male[43].Language", "Title.Male[43].String", "Title.Male[44].Language", "Title.Male[44].String", "Title.Male[45].Language", "Title.Male[45].String", "Title.Male[46].Language", "Title.Male[46].String", "Title.Male[47].Language", "Title.Male[47].String", "Title.Male[48].Language", "Title.Male[48].String", "Title.Male[49].Language", "Title.Male[49].String", "Title.Male[5].Language", "Title.Male[5].String", "Title.Male[50].Language", "Title.Male[50].String", "Title.Male[51].Language", "Title.Male[51].String", "Title.Male[52].Language", "Title.Male[52].String", "Title.Male[53].Language", "Title.Male[53].String", "Title.Male[54].Language", "Title.Male[54].String", "Title.Male[55].Language", "Title.Male[55].String", "Title.Male[56].Language", "Title.Male[56].String", "Title.Male[57].Language", "Title.Male[57].String", "Title.Male[58].Language", "Title.Male[58].String", "Title.Male[59].Language", "Title.Male[59].String", "Title.Male[6].Language", "Title.Male[6].String", "Title.Male[60].Language", "Title.Male[60].String", "Title.Male[61].Language", "Title.Male[61].String", "Title.Male[62].Language", "Title.Male[62].String", "Title.Male[7].Language", "Title.Male[7].String", "Title.Male[8].Language", "Title.Male[8].String", "Title.Male[9].Language", "Title.Male[9].String", "VendorValues", "Version2", "VersionControl");
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

        Helpers.GetSpriggitField(spriggit, "CrimeValues.Arrest").ShouldBe(Helpers.GetDTOField(dto, "CrimeValues.Arrest"));
        Helpers.GetSpriggitField(spriggit, "CrimeValues.Pickpocket").ShouldBe(Helpers.GetDTOField(dto, "CrimeValues.Pickpocket"));
        Helpers.GetSpriggitField(spriggit, "CrimeValues.StealMult").ShouldBe(Helpers.GetDTOField(dto, "CrimeValues.StealMult"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
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
        Helpers.GetSpriggitField(spriggit, "Reaction[2]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[2]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[3]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[3]"));
        Helpers.GetSpriggitField(spriggit, "Reaction[4]").ShouldBe(Helpers.GetDTOField(dto, "Reaction[4]"));
        Helpers.GetSpriggitField(spriggit, "Title.Female.Count").ShouldBe(Helpers.GetDTOField(dto, "Title.Female.Count"));
        Helpers.GetSpriggitField(spriggit, "Title.Female.TargetLanguage[0]").ShouldBe(Helpers.GetDTOField(dto, "Title.Female.TargetLanguage[0]"));
        Helpers.GetSpriggitField(spriggit, "Title.Female.TargetLanguage[1]").ShouldBe(Helpers.GetDTOField(dto, "Title.Female.TargetLanguage[1]"));
        Helpers.GetSpriggitField(spriggit, "Title.Female.TargetLanguage[2]").ShouldBe(Helpers.GetDTOField(dto, "Title.Female.TargetLanguage[2]"));
        Helpers.GetSpriggitField(spriggit, "Title.Female.TargetLanguage[3]").ShouldBe(Helpers.GetDTOField(dto, "Title.Female.TargetLanguage[3]"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[0].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[0].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[1].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[1].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[10].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[10].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[10].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[10].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[11].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[11].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[11].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[11].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[12].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[12].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[12].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[12].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[13].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[13].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[13].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[13].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[14].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[14].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[14].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[14].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[15].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[15].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[15].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[15].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[16].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[16].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[16].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[16].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[17].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[17].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[17].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[17].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[18].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[18].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[18].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[18].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[19].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[19].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[19].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[19].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[2].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[2].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[2].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[2].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[20].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[20].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[20].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[20].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[21].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[21].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[21].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[21].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[22].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[22].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[22].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[22].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[23].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[23].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[23].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[23].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[24].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[24].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[24].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[24].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[25].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[25].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[25].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[25].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[26].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[26].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[26].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[26].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[27].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[27].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[27].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[27].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[28].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[28].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[28].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[28].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[29].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[29].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[29].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[29].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[3].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[3].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[3].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[3].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[30].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[30].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[30].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[30].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[31].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[31].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[31].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[31].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[32].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[32].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[32].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[32].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[33].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[33].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[33].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[33].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[34].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[34].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[34].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[34].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[35].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[35].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[35].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[35].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[4].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[4].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[4].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[4].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[5].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[5].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[5].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[5].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[6].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[6].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[6].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[6].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[7].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[7].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[7].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[7].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[8].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[8].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[8].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[8].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[9].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[9].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[9].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[9].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male.Count").ShouldBe(Helpers.GetDTOField(dto, "Title.Male.Count"));
        Helpers.GetSpriggitField(spriggit, "Title.Male.TargetLanguage[0]").ShouldBe(Helpers.GetDTOField(dto, "Title.Male.TargetLanguage[0]"));
        Helpers.GetSpriggitField(spriggit, "Title.Male.TargetLanguage[1]").ShouldBe(Helpers.GetDTOField(dto, "Title.Male.TargetLanguage[1]"));
        Helpers.GetSpriggitField(spriggit, "Title.Male.TargetLanguage[2]").ShouldBe(Helpers.GetDTOField(dto, "Title.Male.TargetLanguage[2]"));
        Helpers.GetSpriggitField(spriggit, "Title.Male.TargetLanguage[3]").ShouldBe(Helpers.GetDTOField(dto, "Title.Male.TargetLanguage[3]"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[0].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[0].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[1].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[1].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[10].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[10].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[10].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[10].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[11].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[11].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[11].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[11].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[12].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[12].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[12].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[12].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[13].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[13].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[13].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[13].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[14].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[14].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[14].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[14].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[15].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[15].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[15].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[15].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[16].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[16].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[16].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[16].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[17].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[17].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[17].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[17].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[18].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[18].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[18].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[18].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[19].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[19].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[19].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[19].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[2].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[2].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[2].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[2].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[20].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[20].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[20].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[20].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[21].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[21].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[21].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[21].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[22].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[22].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[22].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[22].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[23].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[23].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[23].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[23].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[24].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[24].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[24].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[24].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[25].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[25].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[25].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[25].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[26].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[26].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[26].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[26].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[27].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[27].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[27].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[27].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[28].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[28].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[28].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[28].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[29].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[29].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[29].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[29].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[3].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[3].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[3].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[3].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[30].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[30].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[30].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[30].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[31].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[31].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[31].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[31].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[32].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[32].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[32].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[32].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[33].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[33].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[33].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[33].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[34].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[34].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[34].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[34].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[35].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[35].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[35].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[35].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[4].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[4].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[4].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[4].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[5].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[5].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[5].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[5].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[6].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[6].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[6].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[6].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[7].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[7].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[7].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[7].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[8].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[8].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[8].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[8].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[9].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[9].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[9].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[9].String"));
        Helpers.GetSpriggitField(spriggit, "VendorValues").ShouldBe(Helpers.GetDTOField(dto, "VendorValues"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "CrimeValues.Arrest", "CrimeValues.Pickpocket", "CrimeValues.StealMult", "EditorID", "FormKey", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Reaction[0]", "Reaction[1]", "Reaction[2]", "Reaction[3]", "Reaction[4]", "Title.Female.Count", "Title.Female.TargetLanguage[0]", "Title.Female.TargetLanguage[1]", "Title.Female.TargetLanguage[2]", "Title.Female.TargetLanguage[3]", "Title.Female[0].Language", "Title.Female[0].String", "Title.Female[1].Language", "Title.Female[1].String", "Title.Female[10].Language", "Title.Female[10].String", "Title.Female[11].Language", "Title.Female[11].String", "Title.Female[12].Language", "Title.Female[12].String", "Title.Female[13].Language", "Title.Female[13].String", "Title.Female[14].Language", "Title.Female[14].String", "Title.Female[15].Language", "Title.Female[15].String", "Title.Female[16].Language", "Title.Female[16].String", "Title.Female[17].Language", "Title.Female[17].String", "Title.Female[18].Language", "Title.Female[18].String", "Title.Female[19].Language", "Title.Female[19].String", "Title.Female[2].Language", "Title.Female[2].String", "Title.Female[20].Language", "Title.Female[20].String", "Title.Female[21].Language", "Title.Female[21].String", "Title.Female[22].Language", "Title.Female[22].String", "Title.Female[23].Language", "Title.Female[23].String", "Title.Female[24].Language", "Title.Female[24].String", "Title.Female[25].Language", "Title.Female[25].String", "Title.Female[26].Language", "Title.Female[26].String", "Title.Female[27].Language", "Title.Female[27].String", "Title.Female[28].Language", "Title.Female[28].String", "Title.Female[29].Language", "Title.Female[29].String", "Title.Female[3].Language", "Title.Female[3].String", "Title.Female[30].Language", "Title.Female[30].String", "Title.Female[31].Language", "Title.Female[31].String", "Title.Female[32].Language", "Title.Female[32].String", "Title.Female[33].Language", "Title.Female[33].String", "Title.Female[34].Language", "Title.Female[34].String", "Title.Female[35].Language", "Title.Female[35].String", "Title.Female[4].Language", "Title.Female[4].String", "Title.Female[5].Language", "Title.Female[5].String", "Title.Female[6].Language", "Title.Female[6].String", "Title.Female[7].Language", "Title.Female[7].String", "Title.Female[8].Language", "Title.Female[8].String", "Title.Female[9].Language", "Title.Female[9].String", "Title.Male.Count", "Title.Male.TargetLanguage[0]", "Title.Male.TargetLanguage[1]", "Title.Male.TargetLanguage[2]", "Title.Male.TargetLanguage[3]", "Title.Male[0].Language", "Title.Male[0].String", "Title.Male[1].Language", "Title.Male[1].String", "Title.Male[10].Language", "Title.Male[10].String", "Title.Male[11].Language", "Title.Male[11].String", "Title.Male[12].Language", "Title.Male[12].String", "Title.Male[13].Language", "Title.Male[13].String", "Title.Male[14].Language", "Title.Male[14].String", "Title.Male[15].Language", "Title.Male[15].String", "Title.Male[16].Language", "Title.Male[16].String", "Title.Male[17].Language", "Title.Male[17].String", "Title.Male[18].Language", "Title.Male[18].String", "Title.Male[19].Language", "Title.Male[19].String", "Title.Male[2].Language", "Title.Male[2].String", "Title.Male[20].Language", "Title.Male[20].String", "Title.Male[21].Language", "Title.Male[21].String", "Title.Male[22].Language", "Title.Male[22].String", "Title.Male[23].Language", "Title.Male[23].String", "Title.Male[24].Language", "Title.Male[24].String", "Title.Male[25].Language", "Title.Male[25].String", "Title.Male[26].Language", "Title.Male[26].String", "Title.Male[27].Language", "Title.Male[27].String", "Title.Male[28].Language", "Title.Male[28].String", "Title.Male[29].Language", "Title.Male[29].String", "Title.Male[3].Language", "Title.Male[3].String", "Title.Male[30].Language", "Title.Male[30].String", "Title.Male[31].Language", "Title.Male[31].String", "Title.Male[32].Language", "Title.Male[32].String", "Title.Male[33].Language", "Title.Male[33].String", "Title.Male[34].Language", "Title.Male[34].String", "Title.Male[35].Language", "Title.Male[35].String", "Title.Male[4].Language", "Title.Male[4].String", "Title.Male[5].Language", "Title.Male[5].String", "Title.Male[6].Language", "Title.Male[6].String", "Title.Male[7].Language", "Title.Male[7].String", "Title.Male[8].Language", "Title.Male[8].String", "Title.Male[9].Language", "Title.Male[9].String", "VendorValues", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "CrimeValues.Arrest", "CrimeValues.Pickpocket", "CrimeValues.StealMult", "EditorID", "FormKey", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Reaction[0]", "Reaction[1]", "Reaction[2]", "Reaction[3]", "Reaction[4]", "Title.Female.Count", "Title.Female.TargetLanguage[0]", "Title.Female.TargetLanguage[1]", "Title.Female.TargetLanguage[2]", "Title.Female.TargetLanguage[3]", "Title.Female[0].Language", "Title.Female[0].String", "Title.Female[1].Language", "Title.Female[1].String", "Title.Female[10].Language", "Title.Female[10].String", "Title.Female[11].Language", "Title.Female[11].String", "Title.Female[12].Language", "Title.Female[12].String", "Title.Female[13].Language", "Title.Female[13].String", "Title.Female[14].Language", "Title.Female[14].String", "Title.Female[15].Language", "Title.Female[15].String", "Title.Female[16].Language", "Title.Female[16].String", "Title.Female[17].Language", "Title.Female[17].String", "Title.Female[18].Language", "Title.Female[18].String", "Title.Female[19].Language", "Title.Female[19].String", "Title.Female[2].Language", "Title.Female[2].String", "Title.Female[20].Language", "Title.Female[20].String", "Title.Female[21].Language", "Title.Female[21].String", "Title.Female[22].Language", "Title.Female[22].String", "Title.Female[23].Language", "Title.Female[23].String", "Title.Female[24].Language", "Title.Female[24].String", "Title.Female[25].Language", "Title.Female[25].String", "Title.Female[26].Language", "Title.Female[26].String", "Title.Female[27].Language", "Title.Female[27].String", "Title.Female[28].Language", "Title.Female[28].String", "Title.Female[29].Language", "Title.Female[29].String", "Title.Female[3].Language", "Title.Female[3].String", "Title.Female[30].Language", "Title.Female[30].String", "Title.Female[31].Language", "Title.Female[31].String", "Title.Female[32].Language", "Title.Female[32].String", "Title.Female[33].Language", "Title.Female[33].String", "Title.Female[34].Language", "Title.Female[34].String", "Title.Female[35].Language", "Title.Female[35].String", "Title.Female[4].Language", "Title.Female[4].String", "Title.Female[5].Language", "Title.Female[5].String", "Title.Female[6].Language", "Title.Female[6].String", "Title.Female[7].Language", "Title.Female[7].String", "Title.Female[8].Language", "Title.Female[8].String", "Title.Female[9].Language", "Title.Female[9].String", "Title.Male.Count", "Title.Male.TargetLanguage[0]", "Title.Male.TargetLanguage[1]", "Title.Male.TargetLanguage[2]", "Title.Male.TargetLanguage[3]", "Title.Male[0].Language", "Title.Male[0].String", "Title.Male[1].Language", "Title.Male[1].String", "Title.Male[10].Language", "Title.Male[10].String", "Title.Male[11].Language", "Title.Male[11].String", "Title.Male[12].Language", "Title.Male[12].String", "Title.Male[13].Language", "Title.Male[13].String", "Title.Male[14].Language", "Title.Male[14].String", "Title.Male[15].Language", "Title.Male[15].String", "Title.Male[16].Language", "Title.Male[16].String", "Title.Male[17].Language", "Title.Male[17].String", "Title.Male[18].Language", "Title.Male[18].String", "Title.Male[19].Language", "Title.Male[19].String", "Title.Male[2].Language", "Title.Male[2].String", "Title.Male[20].Language", "Title.Male[20].String", "Title.Male[21].Language", "Title.Male[21].String", "Title.Male[22].Language", "Title.Male[22].String", "Title.Male[23].Language", "Title.Male[23].String", "Title.Male[24].Language", "Title.Male[24].String", "Title.Male[25].Language", "Title.Male[25].String", "Title.Male[26].Language", "Title.Male[26].String", "Title.Male[27].Language", "Title.Male[27].String", "Title.Male[28].Language", "Title.Male[28].String", "Title.Male[29].Language", "Title.Male[29].String", "Title.Male[3].Language", "Title.Male[3].String", "Title.Male[30].Language", "Title.Male[30].String", "Title.Male[31].Language", "Title.Male[31].String", "Title.Male[32].Language", "Title.Male[32].String", "Title.Male[33].Language", "Title.Male[33].String", "Title.Male[34].Language", "Title.Male[34].String", "Title.Male[35].Language", "Title.Male[35].String", "Title.Male[4].Language", "Title.Male[4].String", "Title.Male[5].Language", "Title.Male[5].String", "Title.Male[6].Language", "Title.Male[6].String", "Title.Male[7].Language", "Title.Male[7].String", "Title.Male[8].Language", "Title.Male[8].String", "Title.Male[9].Language", "Title.Male[9].String", "VendorValues", "Version2", "VersionControl");
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
        Helpers.GetSpriggitField(spriggit, "Reaction").ShouldBe(Helpers.GetDTOField(dto, "Reaction"));
        Helpers.GetSpriggitField(spriggit, "VendorBuySellList").ShouldBe(Helpers.GetDTOField(dto, "VendorBuySellListFormKey"));
        Helpers.GetSpriggitField(spriggit, "VendorLocation.Target.MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "VendorLocation.Target.MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "VendorLocation.Target.Type").ShouldBe(Helpers.GetDTOField(dto, "VendorLocation.Target.Type"));
        Helpers.GetSpriggitField(spriggit, "VendorValues.EndHour").ShouldBe(Helpers.GetDTOField(dto, "VendorValues.EndHour"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "CrimeValues.Arrest", "CrimeValues.AttackOnSight", "EditorID", "FormKey", "MerchantContainer", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Reaction", "VendorBuySellList", "VendorLocation.Target.MutagenObjectType", "VendorLocation.Target.Type", "VendorValues.EndHour", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "CrimeValues.Arrest", "CrimeValues.AttackOnSight", "EditorID", "FormKey", "MerchantContainerFormKey", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Reaction", "VendorBuySellListFormKey", "VendorLocation.Target.MutagenObjectType", "VendorLocation.Target.Type", "VendorValues.EndHour", "Version2", "VersionControl");
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
        Helpers.GetSpriggitField(spriggit, "Title.Female[0]").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[0]"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[1]").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[1]"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[2]").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[2]"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[3]").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[3]"));
        Helpers.GetSpriggitField(spriggit, "Title.Female[4]").ShouldBe(Helpers.GetDTOField(dto, "Title.Female[4]"));
        Helpers.GetSpriggitField(spriggit, "Title.Male.Count").ShouldBe(Helpers.GetDTOField(dto, "Title.Male.Count"));
        Helpers.GetSpriggitField(spriggit, "Title.Male.TargetLanguage[0]").ShouldBe(Helpers.GetDTOField(dto, "Title.Male.TargetLanguage[0]"));
        Helpers.GetSpriggitField(spriggit, "Title.Male.TargetLanguage[1]").ShouldBe(Helpers.GetDTOField(dto, "Title.Male.TargetLanguage[1]"));
        Helpers.GetSpriggitField(spriggit, "Title.Male.TargetLanguage[2]").ShouldBe(Helpers.GetDTOField(dto, "Title.Male.TargetLanguage[2]"));
        Helpers.GetSpriggitField(spriggit, "Title.Male.TargetLanguage[3]").ShouldBe(Helpers.GetDTOField(dto, "Title.Male.TargetLanguage[3]"));
        Helpers.GetSpriggitField(spriggit, "Title.Male.TargetLanguage[4]").ShouldBe(Helpers.GetDTOField(dto, "Title.Male.TargetLanguage[4]"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[0].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[0].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[1].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[1].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[10].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[10].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[10].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[10].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[11].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[11].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[11].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[11].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[12].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[12].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[12].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[12].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[13].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[13].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[13].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[13].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[14].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[14].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[14].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[14].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[15].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[15].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[15].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[15].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[16].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[16].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[16].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[16].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[17].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[17].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[17].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[17].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[18].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[18].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[18].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[18].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[19].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[19].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[19].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[19].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[2].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[2].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[2].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[2].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[20].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[20].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[20].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[20].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[21].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[21].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[21].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[21].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[22].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[22].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[22].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[22].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[23].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[23].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[23].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[23].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[24].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[24].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[24].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[24].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[25].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[25].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[25].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[25].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[26].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[26].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[26].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[26].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[27].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[27].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[27].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[27].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[28].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[28].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[28].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[28].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[29].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[29].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[29].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[29].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[3].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[3].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[3].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[3].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[30].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[30].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[30].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[30].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[31].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[31].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[31].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[31].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[32].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[32].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[32].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[32].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[33].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[33].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[33].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[33].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[34].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[34].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[34].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[34].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[35].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[35].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[35].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[35].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[36].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[36].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[36].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[36].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[37].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[37].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[37].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[37].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[38].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[38].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[38].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[38].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[39].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[39].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[39].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[39].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[4].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[4].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[4].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[4].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[40].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[40].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[40].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[40].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[41].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[41].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[41].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[41].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[42].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[42].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[42].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[42].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[43].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[43].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[43].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[43].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[44].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[44].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[44].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[44].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[5].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[5].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[5].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[5].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[6].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[6].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[6].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[6].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[7].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[7].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[7].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[7].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[8].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[8].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[8].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[8].String"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[9].Language").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[9].Language"));
        Helpers.GetSpriggitField(spriggit, "Title.Male[9].String").ShouldBe(Helpers.GetDTOField(dto, "Title.Male[9].String"));
        Helpers.GetSpriggitField(spriggit, "VendorValues").ShouldBe(Helpers.GetDTOField(dto, "VendorValues"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "CrimeValues.Arrest", "CrimeValues.AttackOnSight", "EditorID", "FormKey", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Title.Female[0]", "Title.Female[1]", "Title.Female[2]", "Title.Female[3]", "Title.Female[4]", "Title.Male.Count", "Title.Male.TargetLanguage[0]", "Title.Male.TargetLanguage[1]", "Title.Male.TargetLanguage[2]", "Title.Male.TargetLanguage[3]", "Title.Male.TargetLanguage[4]", "Title.Male[0].Language", "Title.Male[0].String", "Title.Male[1].Language", "Title.Male[1].String", "Title.Male[10].Language", "Title.Male[10].String", "Title.Male[11].Language", "Title.Male[11].String", "Title.Male[12].Language", "Title.Male[12].String", "Title.Male[13].Language", "Title.Male[13].String", "Title.Male[14].Language", "Title.Male[14].String", "Title.Male[15].Language", "Title.Male[15].String", "Title.Male[16].Language", "Title.Male[16].String", "Title.Male[17].Language", "Title.Male[17].String", "Title.Male[18].Language", "Title.Male[18].String", "Title.Male[19].Language", "Title.Male[19].String", "Title.Male[2].Language", "Title.Male[2].String", "Title.Male[20].Language", "Title.Male[20].String", "Title.Male[21].Language", "Title.Male[21].String", "Title.Male[22].Language", "Title.Male[22].String", "Title.Male[23].Language", "Title.Male[23].String", "Title.Male[24].Language", "Title.Male[24].String", "Title.Male[25].Language", "Title.Male[25].String", "Title.Male[26].Language", "Title.Male[26].String", "Title.Male[27].Language", "Title.Male[27].String", "Title.Male[28].Language", "Title.Male[28].String", "Title.Male[29].Language", "Title.Male[29].String", "Title.Male[3].Language", "Title.Male[3].String", "Title.Male[30].Language", "Title.Male[30].String", "Title.Male[31].Language", "Title.Male[31].String", "Title.Male[32].Language", "Title.Male[32].String", "Title.Male[33].Language", "Title.Male[33].String", "Title.Male[34].Language", "Title.Male[34].String", "Title.Male[35].Language", "Title.Male[35].String", "Title.Male[36].Language", "Title.Male[36].String", "Title.Male[37].Language", "Title.Male[37].String", "Title.Male[38].Language", "Title.Male[38].String", "Title.Male[39].Language", "Title.Male[39].String", "Title.Male[4].Language", "Title.Male[4].String", "Title.Male[40].Language", "Title.Male[40].String", "Title.Male[41].Language", "Title.Male[41].String", "Title.Male[42].Language", "Title.Male[42].String", "Title.Male[43].Language", "Title.Male[43].String", "Title.Male[44].Language", "Title.Male[44].String", "Title.Male[5].Language", "Title.Male[5].String", "Title.Male[6].Language", "Title.Male[6].String", "Title.Male[7].Language", "Title.Male[7].String", "Title.Male[8].Language", "Title.Male[8].String", "Title.Male[9].Language", "Title.Male[9].String", "VendorValues", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "CrimeValues.Arrest", "CrimeValues.AttackOnSight", "EditorID", "FormKey", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Title.Female[0]", "Title.Female[1]", "Title.Female[2]", "Title.Female[3]", "Title.Female[4]", "Title.Male.Count", "Title.Male.TargetLanguage[0]", "Title.Male.TargetLanguage[1]", "Title.Male.TargetLanguage[2]", "Title.Male.TargetLanguage[3]", "Title.Male.TargetLanguage[4]", "Title.Male[0].Language", "Title.Male[0].String", "Title.Male[1].Language", "Title.Male[1].String", "Title.Male[10].Language", "Title.Male[10].String", "Title.Male[11].Language", "Title.Male[11].String", "Title.Male[12].Language", "Title.Male[12].String", "Title.Male[13].Language", "Title.Male[13].String", "Title.Male[14].Language", "Title.Male[14].String", "Title.Male[15].Language", "Title.Male[15].String", "Title.Male[16].Language", "Title.Male[16].String", "Title.Male[17].Language", "Title.Male[17].String", "Title.Male[18].Language", "Title.Male[18].String", "Title.Male[19].Language", "Title.Male[19].String", "Title.Male[2].Language", "Title.Male[2].String", "Title.Male[20].Language", "Title.Male[20].String", "Title.Male[21].Language", "Title.Male[21].String", "Title.Male[22].Language", "Title.Male[22].String", "Title.Male[23].Language", "Title.Male[23].String", "Title.Male[24].Language", "Title.Male[24].String", "Title.Male[25].Language", "Title.Male[25].String", "Title.Male[26].Language", "Title.Male[26].String", "Title.Male[27].Language", "Title.Male[27].String", "Title.Male[28].Language", "Title.Male[28].String", "Title.Male[29].Language", "Title.Male[29].String", "Title.Male[3].Language", "Title.Male[3].String", "Title.Male[30].Language", "Title.Male[30].String", "Title.Male[31].Language", "Title.Male[31].String", "Title.Male[32].Language", "Title.Male[32].String", "Title.Male[33].Language", "Title.Male[33].String", "Title.Male[34].Language", "Title.Male[34].String", "Title.Male[35].Language", "Title.Male[35].String", "Title.Male[36].Language", "Title.Male[36].String", "Title.Male[37].Language", "Title.Male[37].String", "Title.Male[38].Language", "Title.Male[38].String", "Title.Male[39].Language", "Title.Male[39].String", "Title.Male[4].Language", "Title.Male[4].String", "Title.Male[40].Language", "Title.Male[40].String", "Title.Male[41].Language", "Title.Male[41].String", "Title.Male[42].Language", "Title.Male[42].String", "Title.Male[43].Language", "Title.Male[43].String", "Title.Male[44].Language", "Title.Male[44].String", "Title.Male[5].Language", "Title.Male[5].String", "Title.Male[6].Language", "Title.Male[6].String", "Title.Male[7].Language", "Title.Male[7].String", "Title.Male[8].Language", "Title.Male[8].String", "Title.Male[9].Language", "Title.Male[9].String", "VendorValues", "Version2", "VersionControl");
    }
}