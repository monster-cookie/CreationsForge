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

        Helpers.GetSpriggitField(spriggit, "AssociatedSkill[0]").ShouldBe(Helpers.GetDTOField(dto, "AssociatedSkill[0]"));
        Helpers.GetSpriggitField(spriggit, "AssociatedSkill[1]").ShouldBe(Helpers.GetDTOField(dto, "AssociatedSkill[1]"));
        Helpers.GetSpriggitField(spriggit, "AssociatedSkill[2]").ShouldBe(Helpers.GetDTOField(dto, "AssociatedSkill[2]"));
        Helpers.GetSpriggitField(spriggit, "AssociatedSkill[3]").ShouldBe(Helpers.GetDTOField(dto, "AssociatedSkill[3]"));
        Helpers.GetSpriggitField(spriggit, "AssociatedSkill[4]").ShouldBe(Helpers.GetDTOField(dto, "AssociatedSkill[4]"));
        Helpers.GetSpriggitField(spriggit, "AssociatedSkill[5]").ShouldBe(Helpers.GetDTOField(dto, "AssociatedSkill[5]"));
        Helpers.GetSpriggitField(spriggit, "AssociatedSkill[6]").ShouldBe(Helpers.GetDTOField(dto, "AssociatedSkill[6]"));
        Helpers.GetSpriggitField(spriggit, "AssociatedSkill[7]").ShouldBe(Helpers.GetDTOField(dto, "AssociatedSkill[7]"));
        Helpers.GetSpriggitField(spriggit, "AssociatedSkill[8]").ShouldBe(Helpers.GetDTOField(dto, "AssociatedSkill[8]"));
        Helpers.GetSpriggitField(spriggit, "AssociatedSkill[9]").ShouldBe(Helpers.GetDTOField(dto, "AssociatedSkill[9]"));
        Helpers.GetSpriggitField(spriggit, "CNAM").ShouldBe(Helpers.GetDTOField(dto, "CNAM"));
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
        Helpers.GetSpriggitField(spriggit, "FNAM[0]").ShouldBe(Helpers.GetDTOField(dto, "FNAM[0]"));
        Helpers.GetSpriggitField(spriggit, "FNAM[1]").ShouldBe(Helpers.GetDTOField(dto, "FNAM[1]"));
        Helpers.GetSpriggitField(spriggit, "FNAM[2]").ShouldBe(Helpers.GetDTOField(dto, "FNAM[2]"));
        Helpers.GetSpriggitField(spriggit, "FNAM[3]").ShouldBe(Helpers.GetDTOField(dto, "FNAM[3]"));
        Helpers.GetSpriggitField(spriggit, "FNAM[4]").ShouldBe(Helpers.GetDTOField(dto, "FNAM[4]"));
        Helpers.GetSpriggitField(spriggit, "FNAM[5]").ShouldBe(Helpers.GetDTOField(dto, "FNAM[5]"));
        Helpers.GetSpriggitField(spriggit, "FNAM[6]").ShouldBe(Helpers.GetDTOField(dto, "FNAM[6]"));
        Helpers.GetSpriggitField(spriggit, "FNAM[7]").ShouldBe(Helpers.GetDTOField(dto, "FNAM[7]"));
        Helpers.GetSpriggitField(spriggit, "FNAM[8]").ShouldBe(Helpers.GetDTOField(dto, "FNAM[8]"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "HorizontalPosition[0]").ShouldBe(Helpers.GetDTOField(dto, "HorizontalPosition[0]"));
        Helpers.GetSpriggitField(spriggit, "HorizontalPosition[1]").ShouldBe(Helpers.GetDTOField(dto, "HorizontalPosition[1]"));
        Helpers.GetSpriggitField(spriggit, "HorizontalPosition[2]").ShouldBe(Helpers.GetDTOField(dto, "HorizontalPosition[2]"));
        Helpers.GetSpriggitField(spriggit, "HorizontalPosition[3]").ShouldBe(Helpers.GetDTOField(dto, "HorizontalPosition[3]"));
        Helpers.GetSpriggitField(spriggit, "HorizontalPosition[4]").ShouldBe(Helpers.GetDTOField(dto, "HorizontalPosition[4]"));
        Helpers.GetSpriggitField(spriggit, "HorizontalPosition[5]").ShouldBe(Helpers.GetDTOField(dto, "HorizontalPosition[5]"));
        Helpers.GetSpriggitField(spriggit, "HorizontalPosition[6]").ShouldBe(Helpers.GetDTOField(dto, "HorizontalPosition[6]"));
        Helpers.GetSpriggitField(spriggit, "HorizontalPosition[7]").ShouldBe(Helpers.GetDTOField(dto, "HorizontalPosition[7]"));
        Helpers.GetSpriggitField(spriggit, "HorizontalPosition[8]").ShouldBe(Helpers.GetDTOField(dto, "HorizontalPosition[8]"));
        Helpers.GetSpriggitField(spriggit, "HorizontalPosition[9]").ShouldBe(Helpers.GetDTOField(dto, "HorizontalPosition[9]"));
        Helpers.GetSpriggitField(spriggit, "Index[0]").ShouldBe(Helpers.GetDTOField(dto, "Index[0]"));
        Helpers.GetSpriggitField(spriggit, "Index[1]").ShouldBe(Helpers.GetDTOField(dto, "Index[1]"));
        Helpers.GetSpriggitField(spriggit, "Index[2]").ShouldBe(Helpers.GetDTOField(dto, "Index[2]"));
        Helpers.GetSpriggitField(spriggit, "Index[3]").ShouldBe(Helpers.GetDTOField(dto, "Index[3]"));
        Helpers.GetSpriggitField(spriggit, "Index[4]").ShouldBe(Helpers.GetDTOField(dto, "Index[4]"));
        Helpers.GetSpriggitField(spriggit, "Index[5]").ShouldBe(Helpers.GetDTOField(dto, "Index[5]"));
        Helpers.GetSpriggitField(spriggit, "Index[6]").ShouldBe(Helpers.GetDTOField(dto, "Index[6]"));
        Helpers.GetSpriggitField(spriggit, "Index[7]").ShouldBe(Helpers.GetDTOField(dto, "Index[7]"));
        Helpers.GetSpriggitField(spriggit, "Index[8]").ShouldBe(Helpers.GetDTOField(dto, "Index[8]"));
        Helpers.GetSpriggitField(spriggit, "Index[9]").ShouldBe(Helpers.GetDTOField(dto, "Index[9]"));
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
        Helpers.GetSpriggitField(spriggit, "PerkGridX[0]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridX[0]"));
        Helpers.GetSpriggitField(spriggit, "PerkGridX[1]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridX[1]"));
        Helpers.GetSpriggitField(spriggit, "PerkGridX[2]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridX[2]"));
        Helpers.GetSpriggitField(spriggit, "PerkGridX[3]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridX[3]"));
        Helpers.GetSpriggitField(spriggit, "PerkGridX[4]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridX[4]"));
        Helpers.GetSpriggitField(spriggit, "PerkGridX[5]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridX[5]"));
        Helpers.GetSpriggitField(spriggit, "PerkGridX[6]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridX[6]"));
        Helpers.GetSpriggitField(spriggit, "PerkGridX[7]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridX[7]"));
        Helpers.GetSpriggitField(spriggit, "PerkGridX[8]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridX[8]"));
        Helpers.GetSpriggitField(spriggit, "PerkGridX[9]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridX[9]"));
        Helpers.GetSpriggitField(spriggit, "PerkGridY[0]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridY[0]"));
        Helpers.GetSpriggitField(spriggit, "PerkGridY[1]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridY[1]"));
        Helpers.GetSpriggitField(spriggit, "PerkGridY[2]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridY[2]"));
        Helpers.GetSpriggitField(spriggit, "PerkGridY[3]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridY[3]"));
        Helpers.GetSpriggitField(spriggit, "PerkGridY[4]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridY[4]"));
        Helpers.GetSpriggitField(spriggit, "PerkGridY[5]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridY[5]"));
        Helpers.GetSpriggitField(spriggit, "PerkGridY[6]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridY[6]"));
        Helpers.GetSpriggitField(spriggit, "PerkGridY[7]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridY[7]"));
        Helpers.GetSpriggitField(spriggit, "PerkGridY[8]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridY[8]"));
        Helpers.GetSpriggitField(spriggit, "PerkGridY[9]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridY[9]"));
        Helpers.GetSpriggitField(spriggit, "Skill.ImproveMult").ShouldBe(Helpers.GetDTOField(dto, "Skill.ImproveMult"));
        Helpers.GetSpriggitField(spriggit, "Skill.ImproveOffset").ShouldBe(Helpers.GetDTOField(dto, "Skill.ImproveOffset"));
        Helpers.GetSpriggitField(spriggit, "Skill.UseMult").ShouldBe(Helpers.GetDTOField(dto, "Skill.UseMult"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));
        Helpers.GetSpriggitField(spriggit, "VerticalPosition[0]").ShouldBe(Helpers.GetDTOField(dto, "VerticalPosition[0]"));
        Helpers.GetSpriggitField(spriggit, "VerticalPosition[1]").ShouldBe(Helpers.GetDTOField(dto, "VerticalPosition[1]"));
        Helpers.GetSpriggitField(spriggit, "VerticalPosition[2]").ShouldBe(Helpers.GetDTOField(dto, "VerticalPosition[2]"));
        Helpers.GetSpriggitField(spriggit, "VerticalPosition[3]").ShouldBe(Helpers.GetDTOField(dto, "VerticalPosition[3]"));
        Helpers.GetSpriggitField(spriggit, "VerticalPosition[4]").ShouldBe(Helpers.GetDTOField(dto, "VerticalPosition[4]"));
        Helpers.GetSpriggitField(spriggit, "VerticalPosition[5]").ShouldBe(Helpers.GetDTOField(dto, "VerticalPosition[5]"));
        Helpers.GetSpriggitField(spriggit, "VerticalPosition[6]").ShouldBe(Helpers.GetDTOField(dto, "VerticalPosition[6]"));
        Helpers.GetSpriggitField(spriggit, "VerticalPosition[7]").ShouldBe(Helpers.GetDTOField(dto, "VerticalPosition[7]"));
        Helpers.GetSpriggitField(spriggit, "VerticalPosition[8]").ShouldBe(Helpers.GetDTOField(dto, "VerticalPosition[8]"));
        Helpers.GetSpriggitField(spriggit, "VerticalPosition[9]").ShouldBe(Helpers.GetDTOField(dto, "VerticalPosition[9]"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "AssociatedSkill[0]", "AssociatedSkill[1]", "AssociatedSkill[2]", "AssociatedSkill[3]", "AssociatedSkill[4]", "AssociatedSkill[5]", "AssociatedSkill[6]", "AssociatedSkill[7]", "AssociatedSkill[8]", "AssociatedSkill[9]", "CNAM", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "EditorID", "FNAM[0]", "FNAM[1]", "FNAM[2]", "FNAM[3]", "FNAM[4]", "FNAM[5]", "FNAM[6]", "FNAM[7]", "FNAM[8]", "FormKey", "HorizontalPosition[0]", "HorizontalPosition[1]", "HorizontalPosition[2]", "HorizontalPosition[3]", "HorizontalPosition[4]", "HorizontalPosition[5]", "HorizontalPosition[6]", "HorizontalPosition[7]", "HorizontalPosition[8]", "HorizontalPosition[9]", "Index[0]", "Index[1]", "Index[2]", "Index[3]", "Index[4]", "Index[5]", "Index[6]", "Index[7]", "Index[8]", "Index[9]", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "PerkGridX[0]", "PerkGridX[1]", "PerkGridX[2]", "PerkGridX[3]", "PerkGridX[4]", "PerkGridX[5]", "PerkGridX[6]", "PerkGridX[7]", "PerkGridX[8]", "PerkGridX[9]", "PerkGridY[0]", "PerkGridY[1]", "PerkGridY[2]", "PerkGridY[3]", "PerkGridY[4]", "PerkGridY[5]", "PerkGridY[6]", "PerkGridY[7]", "PerkGridY[8]", "PerkGridY[9]", "Skill.ImproveMult", "Skill.ImproveOffset", "Skill.UseMult", "Version2", "VersionControl", "VerticalPosition[0]", "VerticalPosition[1]", "VerticalPosition[2]", "VerticalPosition[3]", "VerticalPosition[4]", "VerticalPosition[5]", "VerticalPosition[6]", "VerticalPosition[7]", "VerticalPosition[8]", "VerticalPosition[9]");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "AssociatedSkill[0]", "AssociatedSkill[1]", "AssociatedSkill[2]", "AssociatedSkill[3]", "AssociatedSkill[4]", "AssociatedSkill[5]", "AssociatedSkill[6]", "AssociatedSkill[7]", "AssociatedSkill[8]", "AssociatedSkill[9]", "CNAM", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "EditorID", "FNAM[0]", "FNAM[1]", "FNAM[2]", "FNAM[3]", "FNAM[4]", "FNAM[5]", "FNAM[6]", "FNAM[7]", "FNAM[8]", "FormKey", "HorizontalPosition[0]", "HorizontalPosition[1]", "HorizontalPosition[2]", "HorizontalPosition[3]", "HorizontalPosition[4]", "HorizontalPosition[5]", "HorizontalPosition[6]", "HorizontalPosition[7]", "HorizontalPosition[8]", "HorizontalPosition[9]", "Index[0]", "Index[1]", "Index[2]", "Index[3]", "Index[4]", "Index[5]", "Index[6]", "Index[7]", "Index[8]", "Index[9]", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "PerkGridX[0]", "PerkGridX[1]", "PerkGridX[2]", "PerkGridX[3]", "PerkGridX[4]", "PerkGridX[5]", "PerkGridX[6]", "PerkGridX[7]", "PerkGridX[8]", "PerkGridX[9]", "PerkGridY[0]", "PerkGridY[1]", "PerkGridY[2]", "PerkGridY[3]", "PerkGridY[4]", "PerkGridY[5]", "PerkGridY[6]", "PerkGridY[7]", "PerkGridY[8]", "PerkGridY[9]", "Skill.ImproveMult", "Skill.ImproveOffset", "Skill.UseMult", "Version2", "VersionControl", "VerticalPosition[0]", "VerticalPosition[1]", "VerticalPosition[2]", "VerticalPosition[3]", "VerticalPosition[4]", "VerticalPosition[5]", "VerticalPosition[6]", "VerticalPosition[7]", "VerticalPosition[8]", "VerticalPosition[9]");
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

        Helpers.GetSpriggitField(spriggit, "AssociatedSkill[0]").ShouldBe(Helpers.GetDTOField(dto, "AssociatedSkill[0]"));
        Helpers.GetSpriggitField(spriggit, "AssociatedSkill[1]").ShouldBe(Helpers.GetDTOField(dto, "AssociatedSkill[1]"));
        Helpers.GetSpriggitField(spriggit, "AssociatedSkill[10]").ShouldBe(Helpers.GetDTOField(dto, "AssociatedSkill[10]"));
        Helpers.GetSpriggitField(spriggit, "AssociatedSkill[2]").ShouldBe(Helpers.GetDTOField(dto, "AssociatedSkill[2]"));
        Helpers.GetSpriggitField(spriggit, "AssociatedSkill[3]").ShouldBe(Helpers.GetDTOField(dto, "AssociatedSkill[3]"));
        Helpers.GetSpriggitField(spriggit, "AssociatedSkill[4]").ShouldBe(Helpers.GetDTOField(dto, "AssociatedSkill[4]"));
        Helpers.GetSpriggitField(spriggit, "AssociatedSkill[5]").ShouldBe(Helpers.GetDTOField(dto, "AssociatedSkill[5]"));
        Helpers.GetSpriggitField(spriggit, "AssociatedSkill[6]").ShouldBe(Helpers.GetDTOField(dto, "AssociatedSkill[6]"));
        Helpers.GetSpriggitField(spriggit, "AssociatedSkill[7]").ShouldBe(Helpers.GetDTOField(dto, "AssociatedSkill[7]"));
        Helpers.GetSpriggitField(spriggit, "AssociatedSkill[8]").ShouldBe(Helpers.GetDTOField(dto, "AssociatedSkill[8]"));
        Helpers.GetSpriggitField(spriggit, "AssociatedSkill[9]").ShouldBe(Helpers.GetDTOField(dto, "AssociatedSkill[9]"));
        Helpers.GetSpriggitField(spriggit, "CNAM").ShouldBe(Helpers.GetDTOField(dto, "CNAM"));
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
        Helpers.GetSpriggitField(spriggit, "FNAM[0]").ShouldBe(Helpers.GetDTOField(dto, "FNAM[0]"));
        Helpers.GetSpriggitField(spriggit, "FNAM[1]").ShouldBe(Helpers.GetDTOField(dto, "FNAM[1]"));
        Helpers.GetSpriggitField(spriggit, "FNAM[2]").ShouldBe(Helpers.GetDTOField(dto, "FNAM[2]"));
        Helpers.GetSpriggitField(spriggit, "FNAM[3]").ShouldBe(Helpers.GetDTOField(dto, "FNAM[3]"));
        Helpers.GetSpriggitField(spriggit, "FNAM[4]").ShouldBe(Helpers.GetDTOField(dto, "FNAM[4]"));
        Helpers.GetSpriggitField(spriggit, "FNAM[5]").ShouldBe(Helpers.GetDTOField(dto, "FNAM[5]"));
        Helpers.GetSpriggitField(spriggit, "FNAM[6]").ShouldBe(Helpers.GetDTOField(dto, "FNAM[6]"));
        Helpers.GetSpriggitField(spriggit, "FNAM[7]").ShouldBe(Helpers.GetDTOField(dto, "FNAM[7]"));
        Helpers.GetSpriggitField(spriggit, "FNAM[8]").ShouldBe(Helpers.GetDTOField(dto, "FNAM[8]"));
        Helpers.GetSpriggitField(spriggit, "FNAM[9]").ShouldBe(Helpers.GetDTOField(dto, "FNAM[9]"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "HorizontalPosition[0]").ShouldBe(Helpers.GetDTOField(dto, "HorizontalPosition[0]"));
        Helpers.GetSpriggitField(spriggit, "HorizontalPosition[1]").ShouldBe(Helpers.GetDTOField(dto, "HorizontalPosition[1]"));
        Helpers.GetSpriggitField(spriggit, "HorizontalPosition[10]").ShouldBe(Helpers.GetDTOField(dto, "HorizontalPosition[10]"));
        Helpers.GetSpriggitField(spriggit, "HorizontalPosition[2]").ShouldBe(Helpers.GetDTOField(dto, "HorizontalPosition[2]"));
        Helpers.GetSpriggitField(spriggit, "HorizontalPosition[3]").ShouldBe(Helpers.GetDTOField(dto, "HorizontalPosition[3]"));
        Helpers.GetSpriggitField(spriggit, "HorizontalPosition[4]").ShouldBe(Helpers.GetDTOField(dto, "HorizontalPosition[4]"));
        Helpers.GetSpriggitField(spriggit, "HorizontalPosition[5]").ShouldBe(Helpers.GetDTOField(dto, "HorizontalPosition[5]"));
        Helpers.GetSpriggitField(spriggit, "HorizontalPosition[6]").ShouldBe(Helpers.GetDTOField(dto, "HorizontalPosition[6]"));
        Helpers.GetSpriggitField(spriggit, "HorizontalPosition[7]").ShouldBe(Helpers.GetDTOField(dto, "HorizontalPosition[7]"));
        Helpers.GetSpriggitField(spriggit, "HorizontalPosition[8]").ShouldBe(Helpers.GetDTOField(dto, "HorizontalPosition[8]"));
        Helpers.GetSpriggitField(spriggit, "HorizontalPosition[9]").ShouldBe(Helpers.GetDTOField(dto, "HorizontalPosition[9]"));
        Helpers.GetSpriggitField(spriggit, "Index[0]").ShouldBe(Helpers.GetDTOField(dto, "Index[0]"));
        Helpers.GetSpriggitField(spriggit, "Index[1]").ShouldBe(Helpers.GetDTOField(dto, "Index[1]"));
        Helpers.GetSpriggitField(spriggit, "Index[10]").ShouldBe(Helpers.GetDTOField(dto, "Index[10]"));
        Helpers.GetSpriggitField(spriggit, "Index[2]").ShouldBe(Helpers.GetDTOField(dto, "Index[2]"));
        Helpers.GetSpriggitField(spriggit, "Index[3]").ShouldBe(Helpers.GetDTOField(dto, "Index[3]"));
        Helpers.GetSpriggitField(spriggit, "Index[4]").ShouldBe(Helpers.GetDTOField(dto, "Index[4]"));
        Helpers.GetSpriggitField(spriggit, "Index[5]").ShouldBe(Helpers.GetDTOField(dto, "Index[5]"));
        Helpers.GetSpriggitField(spriggit, "Index[6]").ShouldBe(Helpers.GetDTOField(dto, "Index[6]"));
        Helpers.GetSpriggitField(spriggit, "Index[7]").ShouldBe(Helpers.GetDTOField(dto, "Index[7]"));
        Helpers.GetSpriggitField(spriggit, "Index[8]").ShouldBe(Helpers.GetDTOField(dto, "Index[8]"));
        Helpers.GetSpriggitField(spriggit, "Index[9]").ShouldBe(Helpers.GetDTOField(dto, "Index[9]"));
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
        Helpers.GetSpriggitField(spriggit, "PerkGridX[0]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridX[0]"));
        Helpers.GetSpriggitField(spriggit, "PerkGridX[1]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridX[1]"));
        Helpers.GetSpriggitField(spriggit, "PerkGridX[10]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridX[10]"));
        Helpers.GetSpriggitField(spriggit, "PerkGridX[2]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridX[2]"));
        Helpers.GetSpriggitField(spriggit, "PerkGridX[3]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridX[3]"));
        Helpers.GetSpriggitField(spriggit, "PerkGridX[4]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridX[4]"));
        Helpers.GetSpriggitField(spriggit, "PerkGridX[5]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridX[5]"));
        Helpers.GetSpriggitField(spriggit, "PerkGridX[6]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridX[6]"));
        Helpers.GetSpriggitField(spriggit, "PerkGridX[7]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridX[7]"));
        Helpers.GetSpriggitField(spriggit, "PerkGridX[8]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridX[8]"));
        Helpers.GetSpriggitField(spriggit, "PerkGridX[9]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridX[9]"));
        Helpers.GetSpriggitField(spriggit, "PerkGridY[0]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridY[0]"));
        Helpers.GetSpriggitField(spriggit, "PerkGridY[1]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridY[1]"));
        Helpers.GetSpriggitField(spriggit, "PerkGridY[10]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridY[10]"));
        Helpers.GetSpriggitField(spriggit, "PerkGridY[2]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridY[2]"));
        Helpers.GetSpriggitField(spriggit, "PerkGridY[3]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridY[3]"));
        Helpers.GetSpriggitField(spriggit, "PerkGridY[4]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridY[4]"));
        Helpers.GetSpriggitField(spriggit, "PerkGridY[5]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridY[5]"));
        Helpers.GetSpriggitField(spriggit, "PerkGridY[6]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridY[6]"));
        Helpers.GetSpriggitField(spriggit, "PerkGridY[7]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridY[7]"));
        Helpers.GetSpriggitField(spriggit, "PerkGridY[8]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridY[8]"));
        Helpers.GetSpriggitField(spriggit, "PerkGridY[9]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridY[9]"));
        Helpers.GetSpriggitField(spriggit, "Skill.ImproveMult").ShouldBe(Helpers.GetDTOField(dto, "Skill.ImproveMult"));
        Helpers.GetSpriggitField(spriggit, "Skill.UseMult").ShouldBe(Helpers.GetDTOField(dto, "Skill.UseMult"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));
        Helpers.GetSpriggitField(spriggit, "VerticalPosition[0]").ShouldBe(Helpers.GetDTOField(dto, "VerticalPosition[0]"));
        Helpers.GetSpriggitField(spriggit, "VerticalPosition[1]").ShouldBe(Helpers.GetDTOField(dto, "VerticalPosition[1]"));
        Helpers.GetSpriggitField(spriggit, "VerticalPosition[10]").ShouldBe(Helpers.GetDTOField(dto, "VerticalPosition[10]"));
        Helpers.GetSpriggitField(spriggit, "VerticalPosition[2]").ShouldBe(Helpers.GetDTOField(dto, "VerticalPosition[2]"));
        Helpers.GetSpriggitField(spriggit, "VerticalPosition[3]").ShouldBe(Helpers.GetDTOField(dto, "VerticalPosition[3]"));
        Helpers.GetSpriggitField(spriggit, "VerticalPosition[4]").ShouldBe(Helpers.GetDTOField(dto, "VerticalPosition[4]"));
        Helpers.GetSpriggitField(spriggit, "VerticalPosition[5]").ShouldBe(Helpers.GetDTOField(dto, "VerticalPosition[5]"));
        Helpers.GetSpriggitField(spriggit, "VerticalPosition[6]").ShouldBe(Helpers.GetDTOField(dto, "VerticalPosition[6]"));
        Helpers.GetSpriggitField(spriggit, "VerticalPosition[7]").ShouldBe(Helpers.GetDTOField(dto, "VerticalPosition[7]"));
        Helpers.GetSpriggitField(spriggit, "VerticalPosition[8]").ShouldBe(Helpers.GetDTOField(dto, "VerticalPosition[8]"));
        Helpers.GetSpriggitField(spriggit, "VerticalPosition[9]").ShouldBe(Helpers.GetDTOField(dto, "VerticalPosition[9]"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "AssociatedSkill[0]", "AssociatedSkill[1]", "AssociatedSkill[10]", "AssociatedSkill[2]", "AssociatedSkill[3]", "AssociatedSkill[4]", "AssociatedSkill[5]", "AssociatedSkill[6]", "AssociatedSkill[7]", "AssociatedSkill[8]", "AssociatedSkill[9]", "CNAM", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "EditorID", "FNAM[0]", "FNAM[1]", "FNAM[2]", "FNAM[3]", "FNAM[4]", "FNAM[5]", "FNAM[6]", "FNAM[7]", "FNAM[8]", "FNAM[9]", "FormKey", "HorizontalPosition[0]", "HorizontalPosition[1]", "HorizontalPosition[10]", "HorizontalPosition[2]", "HorizontalPosition[3]", "HorizontalPosition[4]", "HorizontalPosition[5]", "HorizontalPosition[6]", "HorizontalPosition[7]", "HorizontalPosition[8]", "HorizontalPosition[9]", "Index[0]", "Index[1]", "Index[10]", "Index[2]", "Index[3]", "Index[4]", "Index[5]", "Index[6]", "Index[7]", "Index[8]", "Index[9]", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "PerkGridX[0]", "PerkGridX[1]", "PerkGridX[10]", "PerkGridX[2]", "PerkGridX[3]", "PerkGridX[4]", "PerkGridX[5]", "PerkGridX[6]", "PerkGridX[7]", "PerkGridX[8]", "PerkGridX[9]", "PerkGridY[0]", "PerkGridY[1]", "PerkGridY[10]", "PerkGridY[2]", "PerkGridY[3]", "PerkGridY[4]", "PerkGridY[5]", "PerkGridY[6]", "PerkGridY[7]", "PerkGridY[8]", "PerkGridY[9]", "Skill.ImproveMult", "Skill.UseMult", "Version2", "VersionControl", "VerticalPosition[0]", "VerticalPosition[1]", "VerticalPosition[10]", "VerticalPosition[2]", "VerticalPosition[3]", "VerticalPosition[4]", "VerticalPosition[5]", "VerticalPosition[6]", "VerticalPosition[7]", "VerticalPosition[8]", "VerticalPosition[9]");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "AssociatedSkill[0]", "AssociatedSkill[1]", "AssociatedSkill[10]", "AssociatedSkill[2]", "AssociatedSkill[3]", "AssociatedSkill[4]", "AssociatedSkill[5]", "AssociatedSkill[6]", "AssociatedSkill[7]", "AssociatedSkill[8]", "AssociatedSkill[9]", "CNAM", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "EditorID", "FNAM[0]", "FNAM[1]", "FNAM[2]", "FNAM[3]", "FNAM[4]", "FNAM[5]", "FNAM[6]", "FNAM[7]", "FNAM[8]", "FNAM[9]", "FormKey", "HorizontalPosition[0]", "HorizontalPosition[1]", "HorizontalPosition[10]", "HorizontalPosition[2]", "HorizontalPosition[3]", "HorizontalPosition[4]", "HorizontalPosition[5]", "HorizontalPosition[6]", "HorizontalPosition[7]", "HorizontalPosition[8]", "HorizontalPosition[9]", "Index[0]", "Index[1]", "Index[10]", "Index[2]", "Index[3]", "Index[4]", "Index[5]", "Index[6]", "Index[7]", "Index[8]", "Index[9]", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "PerkGridX[0]", "PerkGridX[1]", "PerkGridX[10]", "PerkGridX[2]", "PerkGridX[3]", "PerkGridX[4]", "PerkGridX[5]", "PerkGridX[6]", "PerkGridX[7]", "PerkGridX[8]", "PerkGridX[9]", "PerkGridY[0]", "PerkGridY[1]", "PerkGridY[10]", "PerkGridY[2]", "PerkGridY[3]", "PerkGridY[4]", "PerkGridY[5]", "PerkGridY[6]", "PerkGridY[7]", "PerkGridY[8]", "PerkGridY[9]", "Skill.ImproveMult", "Skill.UseMult", "Version2", "VersionControl", "VerticalPosition[0]", "VerticalPosition[1]", "VerticalPosition[10]", "VerticalPosition[2]", "VerticalPosition[3]", "VerticalPosition[4]", "VerticalPosition[5]", "VerticalPosition[6]", "VerticalPosition[7]", "VerticalPosition[8]", "VerticalPosition[9]");
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

        Helpers.GetSpriggitField(spriggit, "AssociatedSkill[0]").ShouldBe(Helpers.GetDTOField(dto, "AssociatedSkill[0]"));
        Helpers.GetSpriggitField(spriggit, "AssociatedSkill[1]").ShouldBe(Helpers.GetDTOField(dto, "AssociatedSkill[1]"));
        Helpers.GetSpriggitField(spriggit, "AssociatedSkill[2]").ShouldBe(Helpers.GetDTOField(dto, "AssociatedSkill[2]"));
        Helpers.GetSpriggitField(spriggit, "AssociatedSkill[3]").ShouldBe(Helpers.GetDTOField(dto, "AssociatedSkill[3]"));
        Helpers.GetSpriggitField(spriggit, "AssociatedSkill[4]").ShouldBe(Helpers.GetDTOField(dto, "AssociatedSkill[4]"));
        Helpers.GetSpriggitField(spriggit, "AssociatedSkill[5]").ShouldBe(Helpers.GetDTOField(dto, "AssociatedSkill[5]"));
        Helpers.GetSpriggitField(spriggit, "AssociatedSkill[6]").ShouldBe(Helpers.GetDTOField(dto, "AssociatedSkill[6]"));
        Helpers.GetSpriggitField(spriggit, "AssociatedSkill[7]").ShouldBe(Helpers.GetDTOField(dto, "AssociatedSkill[7]"));
        Helpers.GetSpriggitField(spriggit, "AssociatedSkill[8]").ShouldBe(Helpers.GetDTOField(dto, "AssociatedSkill[8]"));
        Helpers.GetSpriggitField(spriggit, "AssociatedSkill[9]").ShouldBe(Helpers.GetDTOField(dto, "AssociatedSkill[9]"));
        Helpers.GetSpriggitField(spriggit, "CNAM").ShouldBe(Helpers.GetDTOField(dto, "CNAM"));
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
        Helpers.GetSpriggitField(spriggit, "FNAM[0]").ShouldBe(Helpers.GetDTOField(dto, "FNAM[0]"));
        Helpers.GetSpriggitField(spriggit, "FNAM[1]").ShouldBe(Helpers.GetDTOField(dto, "FNAM[1]"));
        Helpers.GetSpriggitField(spriggit, "FNAM[2]").ShouldBe(Helpers.GetDTOField(dto, "FNAM[2]"));
        Helpers.GetSpriggitField(spriggit, "FNAM[3]").ShouldBe(Helpers.GetDTOField(dto, "FNAM[3]"));
        Helpers.GetSpriggitField(spriggit, "FNAM[4]").ShouldBe(Helpers.GetDTOField(dto, "FNAM[4]"));
        Helpers.GetSpriggitField(spriggit, "FNAM[5]").ShouldBe(Helpers.GetDTOField(dto, "FNAM[5]"));
        Helpers.GetSpriggitField(spriggit, "FNAM[6]").ShouldBe(Helpers.GetDTOField(dto, "FNAM[6]"));
        Helpers.GetSpriggitField(spriggit, "FNAM[7]").ShouldBe(Helpers.GetDTOField(dto, "FNAM[7]"));
        Helpers.GetSpriggitField(spriggit, "FNAM[8]").ShouldBe(Helpers.GetDTOField(dto, "FNAM[8]"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "HorizontalPosition[0]").ShouldBe(Helpers.GetDTOField(dto, "HorizontalPosition[0]"));
        Helpers.GetSpriggitField(spriggit, "HorizontalPosition[1]").ShouldBe(Helpers.GetDTOField(dto, "HorizontalPosition[1]"));
        Helpers.GetSpriggitField(spriggit, "HorizontalPosition[2]").ShouldBe(Helpers.GetDTOField(dto, "HorizontalPosition[2]"));
        Helpers.GetSpriggitField(spriggit, "HorizontalPosition[3]").ShouldBe(Helpers.GetDTOField(dto, "HorizontalPosition[3]"));
        Helpers.GetSpriggitField(spriggit, "HorizontalPosition[4]").ShouldBe(Helpers.GetDTOField(dto, "HorizontalPosition[4]"));
        Helpers.GetSpriggitField(spriggit, "HorizontalPosition[5]").ShouldBe(Helpers.GetDTOField(dto, "HorizontalPosition[5]"));
        Helpers.GetSpriggitField(spriggit, "HorizontalPosition[6]").ShouldBe(Helpers.GetDTOField(dto, "HorizontalPosition[6]"));
        Helpers.GetSpriggitField(spriggit, "HorizontalPosition[7]").ShouldBe(Helpers.GetDTOField(dto, "HorizontalPosition[7]"));
        Helpers.GetSpriggitField(spriggit, "HorizontalPosition[8]").ShouldBe(Helpers.GetDTOField(dto, "HorizontalPosition[8]"));
        Helpers.GetSpriggitField(spriggit, "HorizontalPosition[9]").ShouldBe(Helpers.GetDTOField(dto, "HorizontalPosition[9]"));
        Helpers.GetSpriggitField(spriggit, "Index[0]").ShouldBe(Helpers.GetDTOField(dto, "Index[0]"));
        Helpers.GetSpriggitField(spriggit, "Index[1]").ShouldBe(Helpers.GetDTOField(dto, "Index[1]"));
        Helpers.GetSpriggitField(spriggit, "Index[2]").ShouldBe(Helpers.GetDTOField(dto, "Index[2]"));
        Helpers.GetSpriggitField(spriggit, "Index[3]").ShouldBe(Helpers.GetDTOField(dto, "Index[3]"));
        Helpers.GetSpriggitField(spriggit, "Index[4]").ShouldBe(Helpers.GetDTOField(dto, "Index[4]"));
        Helpers.GetSpriggitField(spriggit, "Index[5]").ShouldBe(Helpers.GetDTOField(dto, "Index[5]"));
        Helpers.GetSpriggitField(spriggit, "Index[6]").ShouldBe(Helpers.GetDTOField(dto, "Index[6]"));
        Helpers.GetSpriggitField(spriggit, "Index[7]").ShouldBe(Helpers.GetDTOField(dto, "Index[7]"));
        Helpers.GetSpriggitField(spriggit, "Index[8]").ShouldBe(Helpers.GetDTOField(dto, "Index[8]"));
        Helpers.GetSpriggitField(spriggit, "Index[9]").ShouldBe(Helpers.GetDTOField(dto, "Index[9]"));
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
        Helpers.GetSpriggitField(spriggit, "PerkGridX[0]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridX[0]"));
        Helpers.GetSpriggitField(spriggit, "PerkGridX[1]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridX[1]"));
        Helpers.GetSpriggitField(spriggit, "PerkGridX[2]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridX[2]"));
        Helpers.GetSpriggitField(spriggit, "PerkGridX[3]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridX[3]"));
        Helpers.GetSpriggitField(spriggit, "PerkGridX[4]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridX[4]"));
        Helpers.GetSpriggitField(spriggit, "PerkGridX[5]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridX[5]"));
        Helpers.GetSpriggitField(spriggit, "PerkGridX[6]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridX[6]"));
        Helpers.GetSpriggitField(spriggit, "PerkGridX[7]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridX[7]"));
        Helpers.GetSpriggitField(spriggit, "PerkGridX[8]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridX[8]"));
        Helpers.GetSpriggitField(spriggit, "PerkGridX[9]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridX[9]"));
        Helpers.GetSpriggitField(spriggit, "PerkGridY[0]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridY[0]"));
        Helpers.GetSpriggitField(spriggit, "PerkGridY[1]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridY[1]"));
        Helpers.GetSpriggitField(spriggit, "PerkGridY[2]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridY[2]"));
        Helpers.GetSpriggitField(spriggit, "PerkGridY[3]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridY[3]"));
        Helpers.GetSpriggitField(spriggit, "PerkGridY[4]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridY[4]"));
        Helpers.GetSpriggitField(spriggit, "PerkGridY[5]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridY[5]"));
        Helpers.GetSpriggitField(spriggit, "PerkGridY[6]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridY[6]"));
        Helpers.GetSpriggitField(spriggit, "PerkGridY[7]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridY[7]"));
        Helpers.GetSpriggitField(spriggit, "PerkGridY[8]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridY[8]"));
        Helpers.GetSpriggitField(spriggit, "PerkGridY[9]").ShouldBe(Helpers.GetDTOField(dto, "PerkGridY[9]"));
        Helpers.GetSpriggitField(spriggit, "Skill.ImproveMult").ShouldBe(Helpers.GetDTOField(dto, "Skill.ImproveMult"));
        Helpers.GetSpriggitField(spriggit, "Skill.UseMult").ShouldBe(Helpers.GetDTOField(dto, "Skill.UseMult"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));
        Helpers.GetSpriggitField(spriggit, "VerticalPosition[0]").ShouldBe(Helpers.GetDTOField(dto, "VerticalPosition[0]"));
        Helpers.GetSpriggitField(spriggit, "VerticalPosition[1]").ShouldBe(Helpers.GetDTOField(dto, "VerticalPosition[1]"));
        Helpers.GetSpriggitField(spriggit, "VerticalPosition[2]").ShouldBe(Helpers.GetDTOField(dto, "VerticalPosition[2]"));
        Helpers.GetSpriggitField(spriggit, "VerticalPosition[3]").ShouldBe(Helpers.GetDTOField(dto, "VerticalPosition[3]"));
        Helpers.GetSpriggitField(spriggit, "VerticalPosition[4]").ShouldBe(Helpers.GetDTOField(dto, "VerticalPosition[4]"));
        Helpers.GetSpriggitField(spriggit, "VerticalPosition[5]").ShouldBe(Helpers.GetDTOField(dto, "VerticalPosition[5]"));
        Helpers.GetSpriggitField(spriggit, "VerticalPosition[6]").ShouldBe(Helpers.GetDTOField(dto, "VerticalPosition[6]"));
        Helpers.GetSpriggitField(spriggit, "VerticalPosition[7]").ShouldBe(Helpers.GetDTOField(dto, "VerticalPosition[7]"));
        Helpers.GetSpriggitField(spriggit, "VerticalPosition[8]").ShouldBe(Helpers.GetDTOField(dto, "VerticalPosition[8]"));
        Helpers.GetSpriggitField(spriggit, "VerticalPosition[9]").ShouldBe(Helpers.GetDTOField(dto, "VerticalPosition[9]"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "AssociatedSkill[0]", "AssociatedSkill[1]", "AssociatedSkill[2]", "AssociatedSkill[3]", "AssociatedSkill[4]", "AssociatedSkill[5]", "AssociatedSkill[6]", "AssociatedSkill[7]", "AssociatedSkill[8]", "AssociatedSkill[9]", "CNAM", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "EditorID", "FNAM[0]", "FNAM[1]", "FNAM[2]", "FNAM[3]", "FNAM[4]", "FNAM[5]", "FNAM[6]", "FNAM[7]", "FNAM[8]", "FormKey", "HorizontalPosition[0]", "HorizontalPosition[1]", "HorizontalPosition[2]", "HorizontalPosition[3]", "HorizontalPosition[4]", "HorizontalPosition[5]", "HorizontalPosition[6]", "HorizontalPosition[7]", "HorizontalPosition[8]", "HorizontalPosition[9]", "Index[0]", "Index[1]", "Index[2]", "Index[3]", "Index[4]", "Index[5]", "Index[6]", "Index[7]", "Index[8]", "Index[9]", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "PerkGridX[0]", "PerkGridX[1]", "PerkGridX[2]", "PerkGridX[3]", "PerkGridX[4]", "PerkGridX[5]", "PerkGridX[6]", "PerkGridX[7]", "PerkGridX[8]", "PerkGridX[9]", "PerkGridY[0]", "PerkGridY[1]", "PerkGridY[2]", "PerkGridY[3]", "PerkGridY[4]", "PerkGridY[5]", "PerkGridY[6]", "PerkGridY[7]", "PerkGridY[8]", "PerkGridY[9]", "Skill.ImproveMult", "Skill.UseMult", "Version2", "VersionControl", "VerticalPosition[0]", "VerticalPosition[1]", "VerticalPosition[2]", "VerticalPosition[3]", "VerticalPosition[4]", "VerticalPosition[5]", "VerticalPosition[6]", "VerticalPosition[7]", "VerticalPosition[8]", "VerticalPosition[9]");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "AssociatedSkill[0]", "AssociatedSkill[1]", "AssociatedSkill[2]", "AssociatedSkill[3]", "AssociatedSkill[4]", "AssociatedSkill[5]", "AssociatedSkill[6]", "AssociatedSkill[7]", "AssociatedSkill[8]", "AssociatedSkill[9]", "CNAM", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "EditorID", "FNAM[0]", "FNAM[1]", "FNAM[2]", "FNAM[3]", "FNAM[4]", "FNAM[5]", "FNAM[6]", "FNAM[7]", "FNAM[8]", "FormKey", "HorizontalPosition[0]", "HorizontalPosition[1]", "HorizontalPosition[2]", "HorizontalPosition[3]", "HorizontalPosition[4]", "HorizontalPosition[5]", "HorizontalPosition[6]", "HorizontalPosition[7]", "HorizontalPosition[8]", "HorizontalPosition[9]", "Index[0]", "Index[1]", "Index[2]", "Index[3]", "Index[4]", "Index[5]", "Index[6]", "Index[7]", "Index[8]", "Index[9]", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "PerkGridX[0]", "PerkGridX[1]", "PerkGridX[2]", "PerkGridX[3]", "PerkGridX[4]", "PerkGridX[5]", "PerkGridX[6]", "PerkGridX[7]", "PerkGridX[8]", "PerkGridX[9]", "PerkGridY[0]", "PerkGridY[1]", "PerkGridY[2]", "PerkGridY[3]", "PerkGridY[4]", "PerkGridY[5]", "PerkGridY[6]", "PerkGridY[7]", "PerkGridY[8]", "PerkGridY[9]", "Skill.ImproveMult", "Skill.UseMult", "Version2", "VersionControl", "VerticalPosition[0]", "VerticalPosition[1]", "VerticalPosition[2]", "VerticalPosition[3]", "VerticalPosition[4]", "VerticalPosition[5]", "VerticalPosition[6]", "VerticalPosition[7]", "VerticalPosition[8]", "VerticalPosition[9]");
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

        Helpers.GetSpriggitField(spriggit, "CNAM").ShouldBe(Helpers.GetDTOField(dto, "CNAM"));
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
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "CNAM", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "EditorID", "FormKey", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "CNAM", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "EditorID", "FormKey", "Version2", "VersionControl");
    }
}