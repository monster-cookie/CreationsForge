using System.Globalization;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.ActorValueInformation.Skyrim;

public class SkyrimActorValueInformationSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "AVIF")]
    [Trait("FormKey", "000456:Skyrim.esm")]
    [Trait("EditorID", "AVAlchemy")]
    [Trait("SpriggitFile", "ActorValueInformation/AVAlchemy - 000456_Skyrim.esm.yaml")]
    public void Skyrim_AVIF_ShouldMatchSpriggitSample_AVAlchemy()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.ActorValueInformation,
            "AVAlchemy");
        var dto = Helpers.GetDTO<ActorValueInformationDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.ActorValueInformation,
            "000456:Skyrim.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

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
        spriggitFields["Description.Count"].ShouldBe(dtoFields["Description.Count"]);
        spriggitFields["Description.TargetLanguage"].ShouldBe(dtoFields["Description.TargetLanguage"]);
        spriggitFields["Description[6].Language"].ShouldBe(dtoFields["Description[0].Language"]);
        spriggitFields["Description[6].String"].ShouldBe(dtoFields["Description[0].String"]);
        spriggitFields["Description[7].Language"].ShouldBe(dtoFields["Description[1].Language"]);
        spriggitFields["Description[7].String"].ShouldBe(dtoFields["Description[1].String"]);
        spriggitFields["Description[8].Language"].ShouldBe(dtoFields["Description[2].Language"]);
        spriggitFields["Description[8].String"].ShouldBe(dtoFields["Description[2].String"]);
        spriggitFields["Description[0].Language"].ShouldBe(dtoFields["Description[3].Language"]);
        spriggitFields["Description[0].String"].ShouldBe(dtoFields["Description[3].String"]);
        spriggitFields["Description[1].Language"].ShouldBe(dtoFields["Description[4].Language"]);
        spriggitFields["Description[1].String"].ShouldBe(dtoFields["Description[4].String"]);
        spriggitFields["Description[2].Language"].ShouldBe(dtoFields["Description[5].Language"]);
        spriggitFields["Description[2].String"].ShouldBe(dtoFields["Description[5].String"]);
        spriggitFields["Description[3].Language"].ShouldBe(dtoFields["Description[6].Language"]);
        spriggitFields["Description[3].String"].ShouldBe(dtoFields["Description[6].String"]);
        spriggitFields["Description[4].Language"].ShouldBe(dtoFields["Description[7].Language"]);
        spriggitFields["Description[4].String"].ShouldBe(dtoFields["Description[7].String"]);
        spriggitFields["Description[5].Language"].ShouldBe(dtoFields["Description[8].Language"]);
        spriggitFields["Description[5].String"].ShouldBe(dtoFields["Description[8].String"]);
        spriggitFields["CNAM"].ShouldBe(dtoFields["Cnam"]);
        double.Parse(spriggitFields["Skill.UseMult"], CultureInfo.InvariantCulture)
            .ShouldBe(double.Parse(dtoFields["SkillUseMult"], CultureInfo.InvariantCulture), 0.000001);
        double.Parse(spriggitFields["Skill.ImproveMult"], CultureInfo.InvariantCulture)
            .ShouldBe(double.Parse(dtoFields["SkillImproveMult"], CultureInfo.InvariantCulture), 0.000001);
        double.Parse(spriggitFields["Skill.ImproveOffset"], CultureInfo.InvariantCulture)
            .ShouldBe(double.Parse(dtoFields["SkillImproveOffset"], CultureInfo.InvariantCulture), 0.000001);
        spriggitFields["PerkTree[0].FNAM"].ShouldBe(dtoFields["LayoutEntries[0].Fnam"]);
        spriggitFields["PerkTree[0].PerkGridX"].ShouldBe(dtoFields["LayoutEntries[0].PerkGridX"]);
        spriggitFields["PerkTree[0].PerkGridY"].ShouldBe(dtoFields["LayoutEntries[0].PerkGridY"]);
        double.Parse(spriggitFields["PerkTree[0].HorizontalPosition"], CultureInfo.InvariantCulture)
            .ShouldBe(double.Parse(dtoFields["LayoutEntries[0].HorizontalPosition"], CultureInfo.InvariantCulture), 0.000001);
        double.Parse(spriggitFields["PerkTree[0].VerticalPosition"], CultureInfo.InvariantCulture)
            .ShouldBe(double.Parse(dtoFields["LayoutEntries[0].VerticalPosition"], CultureInfo.InvariantCulture), 0.000001);
        spriggitFields["PerkTree[0].AssociatedSkill"].ShouldBe(dtoFields["LayoutEntries[0].AssociatedSkillFormKey"]);
        spriggitFields["PerkTree[0].Index"].ShouldBe(dtoFields["LayoutEntries[0].Index"]);
        spriggitFields["PerkTree[0].FNAM"].ShouldBe(dtoFields["PerkTree[0].Fnam"]);
        spriggitFields["PerkTree[1].FNAM"].ShouldBe(dtoFields["LayoutEntries[1].Fnam"]);
        spriggitFields["PerkTree[1].PerkGridX"].ShouldBe(dtoFields["LayoutEntries[1].PerkGridX"]);
        spriggitFields["PerkTree[1].PerkGridY"].ShouldBe(dtoFields["LayoutEntries[1].PerkGridY"]);
        double.Parse(spriggitFields["PerkTree[1].HorizontalPosition"], CultureInfo.InvariantCulture)
            .ShouldBe(double.Parse(dtoFields["LayoutEntries[1].HorizontalPosition"], CultureInfo.InvariantCulture), 0.000001);
        double.Parse(spriggitFields["PerkTree[1].VerticalPosition"], CultureInfo.InvariantCulture)
            .ShouldBe(double.Parse(dtoFields["LayoutEntries[1].VerticalPosition"], CultureInfo.InvariantCulture), 0.000001);
        spriggitFields["PerkTree[1].AssociatedSkill"].ShouldBe(dtoFields["LayoutEntries[1].AssociatedSkillFormKey"]);
        spriggitFields["PerkTree[1].Index"].ShouldBe(dtoFields["LayoutEntries[1].Index"]);
        spriggitFields["PerkTree[1].FNAM"].ShouldBe(dtoFields["PerkTree[1].Fnam"]);
        spriggitFields["PerkTree[2].FNAM"].ShouldBe(dtoFields["LayoutEntries[2].Fnam"]);
        spriggitFields["PerkTree[2].PerkGridX"].ShouldBe(dtoFields["LayoutEntries[2].PerkGridX"]);
        spriggitFields["PerkTree[2].PerkGridY"].ShouldBe(dtoFields["LayoutEntries[2].PerkGridY"]);
        double.Parse(spriggitFields["PerkTree[2].HorizontalPosition"], CultureInfo.InvariantCulture)
            .ShouldBe(double.Parse(dtoFields["LayoutEntries[2].HorizontalPosition"], CultureInfo.InvariantCulture), 0.000001);
        double.Parse(spriggitFields["PerkTree[2].VerticalPosition"], CultureInfo.InvariantCulture)
            .ShouldBe(double.Parse(dtoFields["LayoutEntries[2].VerticalPosition"], CultureInfo.InvariantCulture), 0.000001);
        spriggitFields["PerkTree[2].AssociatedSkill"].ShouldBe(dtoFields["LayoutEntries[2].AssociatedSkillFormKey"]);
        spriggitFields["PerkTree[2].Index"].ShouldBe(dtoFields["LayoutEntries[2].Index"]);
        spriggitFields["PerkTree[2].FNAM"].ShouldBe(dtoFields["PerkTree[2].Fnam"]);
        spriggitFields["PerkTree[3].FNAM"].ShouldBe(dtoFields["LayoutEntries[3].Fnam"]);
        spriggitFields["PerkTree[3].PerkGridX"].ShouldBe(dtoFields["LayoutEntries[3].PerkGridX"]);
        spriggitFields["PerkTree[3].PerkGridY"].ShouldBe(dtoFields["LayoutEntries[3].PerkGridY"]);
        double.Parse(spriggitFields["PerkTree[3].HorizontalPosition"], CultureInfo.InvariantCulture)
            .ShouldBe(double.Parse(dtoFields["LayoutEntries[3].HorizontalPosition"], CultureInfo.InvariantCulture), 0.000001);
        double.Parse(spriggitFields["PerkTree[3].VerticalPosition"], CultureInfo.InvariantCulture)
            .ShouldBe(double.Parse(dtoFields["LayoutEntries[3].VerticalPosition"], CultureInfo.InvariantCulture), 0.000001);
        spriggitFields["PerkTree[3].AssociatedSkill"].ShouldBe(dtoFields["LayoutEntries[3].AssociatedSkillFormKey"]);
        spriggitFields["PerkTree[3].Index"].ShouldBe(dtoFields["LayoutEntries[3].Index"]);
        spriggitFields["PerkTree[3].FNAM"].ShouldBe(dtoFields["PerkTree[3].Fnam"]);
        spriggitFields["PerkTree[4].FNAM"].ShouldBe(dtoFields["LayoutEntries[4].Fnam"]);
        spriggitFields["PerkTree[4].PerkGridX"].ShouldBe(dtoFields["LayoutEntries[4].PerkGridX"]);
        spriggitFields["PerkTree[4].PerkGridY"].ShouldBe(dtoFields["LayoutEntries[4].PerkGridY"]);
        double.Parse(spriggitFields["PerkTree[4].HorizontalPosition"], CultureInfo.InvariantCulture)
            .ShouldBe(double.Parse(dtoFields["LayoutEntries[4].HorizontalPosition"], CultureInfo.InvariantCulture), 0.000001);
        double.Parse(spriggitFields["PerkTree[4].VerticalPosition"], CultureInfo.InvariantCulture)
            .ShouldBe(double.Parse(dtoFields["LayoutEntries[4].VerticalPosition"], CultureInfo.InvariantCulture), 0.000001);
        spriggitFields["PerkTree[4].AssociatedSkill"].ShouldBe(dtoFields["LayoutEntries[4].AssociatedSkillFormKey"]);
        spriggitFields["PerkTree[4].Index"].ShouldBe(dtoFields["LayoutEntries[4].Index"]);
        spriggitFields["PerkTree[4].FNAM"].ShouldBe(dtoFields["PerkTree[4].Fnam"]);
        spriggitFields["PerkTree[5].FNAM"].ShouldBe(dtoFields["LayoutEntries[5].Fnam"]);
        spriggitFields["PerkTree[5].PerkGridX"].ShouldBe(dtoFields["LayoutEntries[5].PerkGridX"]);
        spriggitFields["PerkTree[5].PerkGridY"].ShouldBe(dtoFields["LayoutEntries[5].PerkGridY"]);
        double.Parse(spriggitFields["PerkTree[5].HorizontalPosition"], CultureInfo.InvariantCulture)
            .ShouldBe(double.Parse(dtoFields["LayoutEntries[5].HorizontalPosition"], CultureInfo.InvariantCulture), 0.000001);
        double.Parse(spriggitFields["PerkTree[5].VerticalPosition"], CultureInfo.InvariantCulture)
            .ShouldBe(double.Parse(dtoFields["LayoutEntries[5].VerticalPosition"], CultureInfo.InvariantCulture), 0.000001);
        spriggitFields["PerkTree[5].AssociatedSkill"].ShouldBe(dtoFields["LayoutEntries[5].AssociatedSkillFormKey"]);
        spriggitFields["PerkTree[5].Index"].ShouldBe(dtoFields["LayoutEntries[5].Index"]);
        spriggitFields["PerkTree[5].FNAM"].ShouldBe(dtoFields["PerkTree[5].Fnam"]);
        spriggitFields["PerkTree[6].FNAM"].ShouldBe(dtoFields["LayoutEntries[6].Fnam"]);
        spriggitFields["PerkTree[6].PerkGridX"].ShouldBe(dtoFields["LayoutEntries[6].PerkGridX"]);
        spriggitFields["PerkTree[6].PerkGridY"].ShouldBe(dtoFields["LayoutEntries[6].PerkGridY"]);
        double.Parse(spriggitFields["PerkTree[6].HorizontalPosition"], CultureInfo.InvariantCulture)
            .ShouldBe(double.Parse(dtoFields["LayoutEntries[6].HorizontalPosition"], CultureInfo.InvariantCulture), 0.000001);
        double.Parse(spriggitFields["PerkTree[6].VerticalPosition"], CultureInfo.InvariantCulture)
            .ShouldBe(double.Parse(dtoFields["LayoutEntries[6].VerticalPosition"], CultureInfo.InvariantCulture), 0.000001);
        spriggitFields["PerkTree[6].AssociatedSkill"].ShouldBe(dtoFields["LayoutEntries[6].AssociatedSkillFormKey"]);
        spriggitFields["PerkTree[6].Index"].ShouldBe(dtoFields["LayoutEntries[6].Index"]);
        spriggitFields["PerkTree[6].FNAM"].ShouldBe(dtoFields["PerkTree[6].Fnam"]);
        spriggitFields["PerkTree[7].FNAM"].ShouldBe(dtoFields["LayoutEntries[7].Fnam"]);
        spriggitFields["PerkTree[7].PerkGridX"].ShouldBe(dtoFields["LayoutEntries[7].PerkGridX"]);
        spriggitFields["PerkTree[7].PerkGridY"].ShouldBe(dtoFields["LayoutEntries[7].PerkGridY"]);
        double.Parse(spriggitFields["PerkTree[7].HorizontalPosition"], CultureInfo.InvariantCulture)
            .ShouldBe(double.Parse(dtoFields["LayoutEntries[7].HorizontalPosition"], CultureInfo.InvariantCulture), 0.000001);
        double.Parse(spriggitFields["PerkTree[7].VerticalPosition"], CultureInfo.InvariantCulture)
            .ShouldBe(double.Parse(dtoFields["LayoutEntries[7].VerticalPosition"], CultureInfo.InvariantCulture), 0.000001);
        spriggitFields["PerkTree[7].AssociatedSkill"].ShouldBe(dtoFields["LayoutEntries[7].AssociatedSkillFormKey"]);
        spriggitFields["PerkTree[7].Index"].ShouldBe(dtoFields["LayoutEntries[7].Index"]);
        spriggitFields["PerkTree[7].FNAM"].ShouldBe(dtoFields["PerkTree[7].Fnam"]);
        spriggitFields["PerkTree[8].FNAM"].ShouldBe(dtoFields["LayoutEntries[8].Fnam"]);
        spriggitFields["PerkTree[8].PerkGridX"].ShouldBe(dtoFields["LayoutEntries[8].PerkGridX"]);
        spriggitFields["PerkTree[8].PerkGridY"].ShouldBe(dtoFields["LayoutEntries[8].PerkGridY"]);
        double.Parse(spriggitFields["PerkTree[8].HorizontalPosition"], CultureInfo.InvariantCulture)
            .ShouldBe(double.Parse(dtoFields["LayoutEntries[8].HorizontalPosition"], CultureInfo.InvariantCulture), 0.000001);
        double.Parse(spriggitFields["PerkTree[8].VerticalPosition"], CultureInfo.InvariantCulture)
            .ShouldBe(double.Parse(dtoFields["LayoutEntries[8].VerticalPosition"], CultureInfo.InvariantCulture), 0.000001);
        spriggitFields["PerkTree[8].AssociatedSkill"].ShouldBe(dtoFields["LayoutEntries[8].AssociatedSkillFormKey"]);
        spriggitFields["PerkTree[8].Index"].ShouldBe(dtoFields["LayoutEntries[8].Index"]);
        spriggitFields["PerkTree[8].FNAM"].ShouldBe(dtoFields["PerkTree[8].Fnam"]);
        spriggitFields["PerkTree[9].FNAM"].ShouldBe(dtoFields["LayoutEntries[9].Fnam"]);
        spriggitFields["PerkTree[9].PerkGridX"].ShouldBe(dtoFields["LayoutEntries[9].PerkGridX"]);
        spriggitFields["PerkTree[9].PerkGridY"].ShouldBe(dtoFields["LayoutEntries[9].PerkGridY"]);
        double.Parse(spriggitFields["PerkTree[9].HorizontalPosition"], CultureInfo.InvariantCulture)
            .ShouldBe(double.Parse(dtoFields["LayoutEntries[9].HorizontalPosition"], CultureInfo.InvariantCulture), 0.000001);
        double.Parse(spriggitFields["PerkTree[9].VerticalPosition"], CultureInfo.InvariantCulture)
            .ShouldBe(double.Parse(dtoFields["LayoutEntries[9].VerticalPosition"], CultureInfo.InvariantCulture), 0.000001);
        spriggitFields["PerkTree[9].AssociatedSkill"].ShouldBe(dtoFields["LayoutEntries[9].AssociatedSkillFormKey"]);
        spriggitFields["PerkTree[9].Index"].ShouldBe(dtoFields["LayoutEntries[9].Index"]);
        spriggitFields["PerkTree[9].FNAM"].ShouldBe(dtoFields["PerkTree[9].Fnam"]);
        spriggitFields.ContainsKey("PerkTree[0].Perk").ShouldBeFalse();
        dtoFields.ContainsKey("PerkTree[0].PerkFormKey").ShouldBeFalse();
        spriggitFields["PerkTree[1].Perk"].ShouldBe(dtoFields["PerkTree[1].PerkFormKey"]);
        spriggitFields["PerkTree[2].Perk"].ShouldBe(dtoFields["PerkTree[2].PerkFormKey"]);
        spriggitFields["PerkTree[3].Perk"].ShouldBe(dtoFields["PerkTree[3].PerkFormKey"]);
        spriggitFields["PerkTree[4].Perk"].ShouldBe(dtoFields["PerkTree[4].PerkFormKey"]);
        spriggitFields["PerkTree[5].Perk"].ShouldBe(dtoFields["PerkTree[5].PerkFormKey"]);
        spriggitFields["PerkTree[6].Perk"].ShouldBe(dtoFields["PerkTree[6].PerkFormKey"]);
        spriggitFields["PerkTree[7].Perk"].ShouldBe(dtoFields["PerkTree[7].PerkFormKey"]);
        spriggitFields["PerkTree[8].Perk"].ShouldBe(dtoFields["PerkTree[8].PerkFormKey"]);
        spriggitFields["PerkTree[9].Perk"].ShouldBe(dtoFields["PerkTree[9].PerkFormKey"]);
        spriggitFields["PerkTree[0].ConnectionLineToIndices.Count"].ShouldBe(dtoFields["PerkTree[0].ConnectionLineToIndices.Count"]);
        spriggitFields["PerkTree[0].ConnectionLineToIndices[0]"].ShouldBe(dtoFields["PerkTree[0].ConnectionLineToIndices[0].TargetIndex"]);
        spriggitFields["PerkTree[1].ConnectionLineToIndices.Count"].ShouldBe(dtoFields["PerkTree[1].ConnectionLineToIndices.Count"]);
        spriggitFields["PerkTree[1].ConnectionLineToIndices[0]"].ShouldBe(dtoFields["PerkTree[1].ConnectionLineToIndices[0].TargetIndex"]);
        spriggitFields["PerkTree[2].ConnectionLineToIndices.Count"].ShouldBe(dtoFields["PerkTree[2].ConnectionLineToIndices.Count"]);
        spriggitFields["PerkTree[2].ConnectionLineToIndices[0]"].ShouldBe(dtoFields["PerkTree[2].ConnectionLineToIndices[0].TargetIndex"]);
        spriggitFields["PerkTree[2].ConnectionLineToIndices[1]"].ShouldBe(dtoFields["PerkTree[2].ConnectionLineToIndices[1].TargetIndex"]);
        spriggitFields["PerkTree[3].ConnectionLineToIndices.Count"].ShouldBe(dtoFields["PerkTree[3].ConnectionLineToIndices.Count"]);
        spriggitFields["PerkTree[3].ConnectionLineToIndices[0]"].ShouldBe(dtoFields["PerkTree[3].ConnectionLineToIndices[0].TargetIndex"]);
        spriggitFields["PerkTree[4].ConnectionLineToIndices.Count"].ShouldBe(dtoFields["PerkTree[4].ConnectionLineToIndices.Count"]);
        spriggitFields["PerkTree[4].ConnectionLineToIndices[0]"].ShouldBe(dtoFields["PerkTree[4].ConnectionLineToIndices[0].TargetIndex"]);
        spriggitFields["PerkTree[5].ConnectionLineToIndices.Count"].ShouldBe(dtoFields["PerkTree[5].ConnectionLineToIndices.Count"]);
        spriggitFields["PerkTree[5].ConnectionLineToIndices[0]"].ShouldBe(dtoFields["PerkTree[5].ConnectionLineToIndices[0].TargetIndex"]);
        spriggitFields.ContainsKey("PerkTree[6].ConnectionLineToIndices.Count").ShouldBeFalse();
        dtoFields["PerkTree[6].ConnectionLineToIndices.Count"].ShouldBe("0");
        spriggitFields["PerkTree[7].ConnectionLineToIndices.Count"].ShouldBe(dtoFields["PerkTree[7].ConnectionLineToIndices.Count"]);
        spriggitFields["PerkTree[7].ConnectionLineToIndices[0]"].ShouldBe(dtoFields["PerkTree[7].ConnectionLineToIndices[0].TargetIndex"]);
        spriggitFields["PerkTree[8].ConnectionLineToIndices.Count"].ShouldBe(dtoFields["PerkTree[8].ConnectionLineToIndices.Count"]);
        spriggitFields["PerkTree[8].ConnectionLineToIndices[0]"].ShouldBe(dtoFields["PerkTree[8].ConnectionLineToIndices[0].TargetIndex"]);
        spriggitFields["PerkTree[8].ConnectionLineToIndices[1]"].ShouldBe(dtoFields["PerkTree[8].ConnectionLineToIndices[1].TargetIndex"]);
        spriggitFields.ContainsKey("PerkTree[9].ConnectionLineToIndices.Count").ShouldBeFalse();
        dtoFields["PerkTree[9].ConnectionLineToIndices.Count"].ShouldBe("0");

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "AVIF")]
    [Trait("FormKey", "000458:Skyrim.esm")]
    [Trait("EditorID", "AVAlteration")]
    [Trait("SpriggitFile", "ActorValueInformation/AVAlteration - 000458_Skyrim.esm.yaml")]
    public void Skyrim_AVIF_ShouldMatchSpriggitSample_AVAlteration()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.ActorValueInformation,
            "AVAlteration");
        var dto = Helpers.GetDTO<ActorValueInformationDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.ActorValueInformation,
            "000458:Skyrim.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

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
        spriggitFields["Description.Count"].ShouldBe(dtoFields["Description.Count"]);
        spriggitFields["Description.TargetLanguage"].ShouldBe(dtoFields["Description.TargetLanguage"]);
        spriggitFields["Description[6].Language"].ShouldBe(dtoFields["Description[0].Language"]);
        spriggitFields["Description[6].String"].ShouldBe(dtoFields["Description[0].String"]);
        spriggitFields["Description[7].Language"].ShouldBe(dtoFields["Description[1].Language"]);
        spriggitFields["Description[7].String"].ShouldBe(dtoFields["Description[1].String"]);
        spriggitFields["Description[8].Language"].ShouldBe(dtoFields["Description[2].Language"]);
        spriggitFields["Description[8].String"].ShouldBe(dtoFields["Description[2].String"]);
        spriggitFields["Description[0].Language"].ShouldBe(dtoFields["Description[3].Language"]);
        spriggitFields["Description[0].String"].ShouldBe(dtoFields["Description[3].String"]);
        spriggitFields["Description[1].Language"].ShouldBe(dtoFields["Description[4].Language"]);
        spriggitFields["Description[1].String"].ShouldBe(dtoFields["Description[4].String"]);
        spriggitFields["Description[2].Language"].ShouldBe(dtoFields["Description[5].Language"]);
        spriggitFields["Description[2].String"].ShouldBe(dtoFields["Description[5].String"]);
        spriggitFields["Description[3].Language"].ShouldBe(dtoFields["Description[6].Language"]);
        spriggitFields["Description[3].String"].ShouldBe(dtoFields["Description[6].String"]);
        spriggitFields["Description[4].Language"].ShouldBe(dtoFields["Description[7].Language"]);
        spriggitFields["Description[4].String"].ShouldBe(dtoFields["Description[7].String"]);
        spriggitFields["Description[5].Language"].ShouldBe(dtoFields["Description[8].Language"]);
        spriggitFields["Description[5].String"].ShouldBe(dtoFields["Description[8].String"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "AVIF")]
    [Trait("FormKey", "00044F:Skyrim.esm")]
    [Trait("EditorID", "AVBlock")]
    [Trait("SpriggitFile", "ActorValueInformation/AVBlock - 00044F_Skyrim.esm.yaml")]
    public void Skyrim_AVIF_ShouldMatchSpriggitSample_AVBlock()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.ActorValueInformation,
            "AVBlock");
        var dto = Helpers.GetDTO<ActorValueInformationDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.ActorValueInformation,
            "00044F:Skyrim.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

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
        spriggitFields["Description.Count"].ShouldBe(dtoFields["Description.Count"]);
        spriggitFields["Description.TargetLanguage"].ShouldBe(dtoFields["Description.TargetLanguage"]);
        spriggitFields["Description[6].Language"].ShouldBe(dtoFields["Description[0].Language"]);
        spriggitFields["Description[6].String"].ShouldBe(dtoFields["Description[0].String"]);
        spriggitFields["Description[7].Language"].ShouldBe(dtoFields["Description[1].Language"]);
        spriggitFields["Description[7].String"].ShouldBe(dtoFields["Description[1].String"]);
        spriggitFields["Description[8].Language"].ShouldBe(dtoFields["Description[2].Language"]);
        spriggitFields["Description[8].String"].ShouldBe(dtoFields["Description[2].String"]);
        spriggitFields["Description[0].Language"].ShouldBe(dtoFields["Description[3].Language"]);
        spriggitFields["Description[0].String"].ShouldBe(dtoFields["Description[3].String"]);
        spriggitFields["Description[1].Language"].ShouldBe(dtoFields["Description[4].Language"]);
        spriggitFields["Description[1].String"].ShouldBe(dtoFields["Description[4].String"]);
        spriggitFields["Description[2].Language"].ShouldBe(dtoFields["Description[5].Language"]);
        spriggitFields["Description[2].String"].ShouldBe(dtoFields["Description[5].String"]);
        spriggitFields["Description[3].Language"].ShouldBe(dtoFields["Description[6].Language"]);
        spriggitFields["Description[3].String"].ShouldBe(dtoFields["Description[6].String"]);
        spriggitFields["Description[4].Language"].ShouldBe(dtoFields["Description[7].Language"]);
        spriggitFields["Description[4].String"].ShouldBe(dtoFields["Description[7].String"]);
        spriggitFields["Description[5].Language"].ShouldBe(dtoFields["Description[8].Language"]);
        spriggitFields["Description[5].String"].ShouldBe(dtoFields["Description[8].String"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "AVIF")]
    [Trait("FormKey", "0005F6:Skyrim.esm")]
    [Trait("EditorID", "AVFavorActive")]
    [Trait("SpriggitFile", "ActorValueInformation/AVFavorActive - 0005F6_Skyrim.esm.yaml")]
    public void Skyrim_AVIF_ShouldMatchSpriggitSample_AVFavorActive()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.ActorValueInformation,
            "AVFavorActive");
        var dto = Helpers.GetDTO<ActorValueInformationDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.ActorValueInformation,
            "0005F6:Skyrim.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields.ContainsKey("Name.Count").ShouldBeFalse();
        dtoFields.ContainsKey("Name.Count").ShouldBeFalse();
        spriggitFields["Description.Count"].ShouldBe(dtoFields["Description.Count"]);
        spriggitFields["Description.TargetLanguage"].ShouldBe(dtoFields["Description.TargetLanguage"]);
        spriggitFields["Description[6].Language"].ShouldBe(dtoFields["Description[0].Language"]);
        spriggitFields["Description[6].String"].ShouldBe(dtoFields["Description[0].String"]);
        spriggitFields["Description[7].Language"].ShouldBe(dtoFields["Description[1].Language"]);
        spriggitFields["Description[7].String"].ShouldBe(dtoFields["Description[1].String"]);
        spriggitFields["Description[8].Language"].ShouldBe(dtoFields["Description[2].Language"]);
        spriggitFields["Description[8].String"].ShouldBe(dtoFields["Description[2].String"]);
        spriggitFields["Description[0].Language"].ShouldBe(dtoFields["Description[3].Language"]);
        spriggitFields["Description[0].String"].ShouldBe(dtoFields["Description[3].String"]);
        spriggitFields["Description[1].Language"].ShouldBe(dtoFields["Description[4].Language"]);
        spriggitFields["Description[1].String"].ShouldBe(dtoFields["Description[4].String"]);
        spriggitFields["Description[2].Language"].ShouldBe(dtoFields["Description[5].Language"]);
        spriggitFields["Description[2].String"].ShouldBe(dtoFields["Description[5].String"]);
        spriggitFields["Description[3].Language"].ShouldBe(dtoFields["Description[6].Language"]);
        spriggitFields["Description[3].String"].ShouldBe(dtoFields["Description[6].String"]);
        spriggitFields["Description[4].Language"].ShouldBe(dtoFields["Description[7].Language"]);
        spriggitFields["Description[4].String"].ShouldBe(dtoFields["Description[7].String"]);
        spriggitFields["Description[5].Language"].ShouldBe(dtoFields["Description[8].Language"]);
        spriggitFields["Description[5].String"].ShouldBe(dtoFields["Description[8].String"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }
}
