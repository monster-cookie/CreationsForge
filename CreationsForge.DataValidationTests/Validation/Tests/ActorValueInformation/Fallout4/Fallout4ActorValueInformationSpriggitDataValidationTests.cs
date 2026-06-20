using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.ActorValueInformation.Fallout4;

public class Fallout4ActorValueInformationSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "AVIF")]
    [Trait("FormKey", "0B287B:Fallout4.esm")]
    [Trait("EditorID", "SentryBotMaxHeatLevel")]
    [Trait("SpriggitFile", "ActorValueInformation/SentryBotMaxHeatLevel - 0B287B_Fallout4.esm.yaml")]
    public void Fallout4_AVIF_ShouldMatchSpriggitSample_SentryBotMaxHeatLevel()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.ActorValueInformation,
            "SentryBotMaxHeatLevel");
        var dto = Helpers.GetDTO<ActorValueInformationDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.ActorValueInformation,
            "0B287B:Fallout4.esm");

        Helpers.GetSpriggitField(spriggit, "Abbreviation.Count").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation.Count"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[0].String").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[0].String"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[1].String").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[1].String"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[10].Language").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[10].Language"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[10].String").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[10].String"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[2].Language").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[2].Language"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[2].String").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[2].String"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[3].Language").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[3].Language"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[3].String").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[3].String"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[4].Language").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[4].Language"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[4].String").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[4].String"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[5].Language").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[5].Language"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[5].String").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[5].String"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[6].Language").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[6].Language"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[6].String").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[6].String"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[7].Language").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[7].Language"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[7].String").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[7].String"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[8].Language").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[8].Language"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[8].String").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[8].String"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[9].Language").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[9].Language"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[9].String").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[9].String"));
        Helpers.GetSpriggitField(spriggit, "DefaultValue").ShouldBe(Helpers.GetDTOField(dto, "DefaultValue"));
        Helpers.GetSpriggitField(spriggit, "Description.Count").ShouldBe(Helpers.GetDTOField(dto, "Description.Count"));
        Helpers.GetSpriggitField(spriggit, "Description.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Description.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Description[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[0].String").ShouldBe(Helpers.GetDTOField(dto, "Description[0].String"));
        Helpers.GetSpriggitField(spriggit, "Description[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[1].String").ShouldBe(Helpers.GetDTOField(dto, "Description[1].String"));
        Helpers.GetSpriggitField(spriggit, "Description[10].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[10].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[10].String").ShouldBe(Helpers.GetDTOField(dto, "Description[10].String"));
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
        Helpers.GetSpriggitField(spriggit, "Description[9].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[9].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[9].String").ShouldBe(Helpers.GetDTOField(dto, "Description[9].String"));
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
        Helpers.GetSpriggitField(spriggit, "Type").ShouldBe(Helpers.GetDTOField(dto, "Type"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "Abbreviation.Count", "Abbreviation.TargetLanguage", "Abbreviation[0].Language", "Abbreviation[0].String", "Abbreviation[1].Language", "Abbreviation[1].String", "Abbreviation[10].Language", "Abbreviation[10].String", "Abbreviation[2].Language", "Abbreviation[2].String", "Abbreviation[3].Language", "Abbreviation[3].String", "Abbreviation[4].Language", "Abbreviation[4].String", "Abbreviation[5].Language", "Abbreviation[5].String", "Abbreviation[6].Language", "Abbreviation[6].String", "Abbreviation[7].Language", "Abbreviation[7].String", "Abbreviation[8].Language", "Abbreviation[8].String", "Abbreviation[9].Language", "Abbreviation[9].String", "DefaultValue", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[10].Language", "Description[10].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "Description[9].Language", "Description[9].String", "EditorID", "FormKey", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "Type", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "Abbreviation.Count", "Abbreviation.TargetLanguage", "Abbreviation[0].Language", "Abbreviation[0].String", "Abbreviation[1].Language", "Abbreviation[1].String", "Abbreviation[10].Language", "Abbreviation[10].String", "Abbreviation[2].Language", "Abbreviation[2].String", "Abbreviation[3].Language", "Abbreviation[3].String", "Abbreviation[4].Language", "Abbreviation[4].String", "Abbreviation[5].Language", "Abbreviation[5].String", "Abbreviation[6].Language", "Abbreviation[6].String", "Abbreviation[7].Language", "Abbreviation[7].String", "Abbreviation[8].Language", "Abbreviation[8].String", "Abbreviation[9].Language", "Abbreviation[9].String", "DefaultValue", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[10].Language", "Description[10].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "Description[9].Language", "Description[9].String", "EditorID", "FormKey", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "Type", "Version2", "VersionControl");
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "AVIF")]
    [Trait("FormKey", "00080F:Fallout4.esm")]
    [Trait("EditorID", "HC_Adrenaline")]
    [Trait("SpriggitFile", "ActorValueInformation/HC_Adrenaline - 00080F_Fallout4.esm.yaml")]
    public void Fallout4_AVIF_ShouldMatchSpriggitSample_HC_Adrenaline()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.ActorValueInformation,
            "HC_Adrenaline");
        var dto = Helpers.GetDTO<ActorValueInformationDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.ActorValueInformation,
            "00080F:Fallout4.esm");

        Helpers.GetSpriggitField(spriggit, "Abbreviation.Count").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation.Count"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[0].String").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[0].String"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[1].String").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[1].String"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[10].Language").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[10].Language"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[10].String").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[10].String"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[2].Language").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[2].Language"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[2].String").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[2].String"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[3].Language").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[3].Language"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[3].String").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[3].String"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[4].Language").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[4].Language"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[4].String").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[4].String"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[5].Language").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[5].Language"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[5].String").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[5].String"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[6].Language").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[6].Language"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[6].String").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[6].String"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[7].Language").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[7].Language"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[7].String").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[7].String"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[8].Language").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[8].Language"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[8].String").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[8].String"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[9].Language").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[9].Language"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[9].String").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[9].String"));
        Helpers.GetSpriggitField(spriggit, "DefaultValue").ShouldBe(Helpers.GetDTOField(dto, "DefaultValue"));
        Helpers.GetSpriggitField(spriggit, "Description.Count").ShouldBe(Helpers.GetDTOField(dto, "Description.Count"));
        Helpers.GetSpriggitField(spriggit, "Description.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Description.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Description[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[0].String").ShouldBe(Helpers.GetDTOField(dto, "Description[0].String"));
        Helpers.GetSpriggitField(spriggit, "Description[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[1].String").ShouldBe(Helpers.GetDTOField(dto, "Description[1].String"));
        Helpers.GetSpriggitField(spriggit, "Description[10].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[10].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[10].String").ShouldBe(Helpers.GetDTOField(dto, "Description[10].String"));
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
        Helpers.GetSpriggitField(spriggit, "Description[9].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[9].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[9].String").ShouldBe(Helpers.GetDTOField(dto, "Description[9].String"));
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
        Helpers.GetSpriggitField(spriggit, "Type").ShouldBe(Helpers.GetDTOField(dto, "Type"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "Abbreviation.Count", "Abbreviation.TargetLanguage", "Abbreviation[0].Language", "Abbreviation[0].String", "Abbreviation[1].Language", "Abbreviation[1].String", "Abbreviation[10].Language", "Abbreviation[10].String", "Abbreviation[2].Language", "Abbreviation[2].String", "Abbreviation[3].Language", "Abbreviation[3].String", "Abbreviation[4].Language", "Abbreviation[4].String", "Abbreviation[5].Language", "Abbreviation[5].String", "Abbreviation[6].Language", "Abbreviation[6].String", "Abbreviation[7].Language", "Abbreviation[7].String", "Abbreviation[8].Language", "Abbreviation[8].String", "Abbreviation[9].Language", "Abbreviation[9].String", "DefaultValue", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[10].Language", "Description[10].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "Description[9].Language", "Description[9].String", "EditorID", "FormKey", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "Type", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "Abbreviation.Count", "Abbreviation.TargetLanguage", "Abbreviation[0].Language", "Abbreviation[0].String", "Abbreviation[1].Language", "Abbreviation[1].String", "Abbreviation[10].Language", "Abbreviation[10].String", "Abbreviation[2].Language", "Abbreviation[2].String", "Abbreviation[3].Language", "Abbreviation[3].String", "Abbreviation[4].Language", "Abbreviation[4].String", "Abbreviation[5].Language", "Abbreviation[5].String", "Abbreviation[6].Language", "Abbreviation[6].String", "Abbreviation[7].Language", "Abbreviation[7].String", "Abbreviation[8].Language", "Abbreviation[8].String", "Abbreviation[9].Language", "Abbreviation[9].String", "DefaultValue", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[10].Language", "Description[10].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "Description[9].Language", "Description[9].String", "EditorID", "FormKey", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "Type", "Version2", "VersionControl");
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "AVIF")]
    [Trait("FormKey", "1B88D8:Fallout4.esm")]
    [Trait("EditorID", "Incendiary")]
    [Trait("SpriggitFile", "ActorValueInformation/Incendiary - 1B88D8_Fallout4.esm.yaml")]
    public void Fallout4_AVIF_ShouldMatchSpriggitSample_Incendiary()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.ActorValueInformation,
            "Incendiary");
        var dto = Helpers.GetDTO<ActorValueInformationDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.ActorValueInformation,
            "1B88D8:Fallout4.esm");

        Helpers.GetSpriggitField(spriggit, "Abbreviation.Count").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation.Count"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[0].String").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[0].String"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[1].String").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[1].String"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[10].Language").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[10].Language"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[10].String").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[10].String"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[2].Language").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[2].Language"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[2].String").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[2].String"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[3].Language").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[3].Language"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[3].String").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[3].String"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[4].Language").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[4].Language"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[4].String").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[4].String"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[5].Language").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[5].Language"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[5].String").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[5].String"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[6].Language").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[6].Language"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[6].String").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[6].String"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[7].Language").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[7].Language"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[7].String").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[7].String"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[8].Language").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[8].Language"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[8].String").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[8].String"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[9].Language").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[9].Language"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[9].String").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[9].String"));
        Helpers.GetSpriggitField(spriggit, "DefaultValue").ShouldBe(Helpers.GetDTOField(dto, "DefaultValue"));
        Helpers.GetSpriggitField(spriggit, "Description.Count").ShouldBe(Helpers.GetDTOField(dto, "Description.Count"));
        Helpers.GetSpriggitField(spriggit, "Description.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Description.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Description[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[0].String").ShouldBe(Helpers.GetDTOField(dto, "Description[0].String"));
        Helpers.GetSpriggitField(spriggit, "Description[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[1].String").ShouldBe(Helpers.GetDTOField(dto, "Description[1].String"));
        Helpers.GetSpriggitField(spriggit, "Description[10].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[10].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[10].String").ShouldBe(Helpers.GetDTOField(dto, "Description[10].String"));
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
        Helpers.GetSpriggitField(spriggit, "Description[9].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[9].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[9].String").ShouldBe(Helpers.GetDTOField(dto, "Description[9].String"));
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
        Helpers.GetSpriggitField(spriggit, "Type").ShouldBe(Helpers.GetDTOField(dto, "Type"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "Abbreviation.Count", "Abbreviation.TargetLanguage", "Abbreviation[0].Language", "Abbreviation[0].String", "Abbreviation[1].Language", "Abbreviation[1].String", "Abbreviation[10].Language", "Abbreviation[10].String", "Abbreviation[2].Language", "Abbreviation[2].String", "Abbreviation[3].Language", "Abbreviation[3].String", "Abbreviation[4].Language", "Abbreviation[4].String", "Abbreviation[5].Language", "Abbreviation[5].String", "Abbreviation[6].Language", "Abbreviation[6].String", "Abbreviation[7].Language", "Abbreviation[7].String", "Abbreviation[8].Language", "Abbreviation[8].String", "Abbreviation[9].Language", "Abbreviation[9].String", "DefaultValue", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[10].Language", "Description[10].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "Description[9].Language", "Description[9].String", "EditorID", "FormKey", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "Type", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "Abbreviation.Count", "Abbreviation.TargetLanguage", "Abbreviation[0].Language", "Abbreviation[0].String", "Abbreviation[1].Language", "Abbreviation[1].String", "Abbreviation[10].Language", "Abbreviation[10].String", "Abbreviation[2].Language", "Abbreviation[2].String", "Abbreviation[3].Language", "Abbreviation[3].String", "Abbreviation[4].Language", "Abbreviation[4].String", "Abbreviation[5].Language", "Abbreviation[5].String", "Abbreviation[6].Language", "Abbreviation[6].String", "Abbreviation[7].Language", "Abbreviation[7].String", "Abbreviation[8].Language", "Abbreviation[8].String", "Abbreviation[9].Language", "Abbreviation[9].String", "DefaultValue", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[10].Language", "Description[10].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "Description[9].Language", "Description[9].String", "EditorID", "FormKey", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "Type", "Version2", "VersionControl");
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "AVIF")]
    [Trait("FormKey", "0002C7:Fallout4.esm")]
    [Trait("EditorID", "Agility")]
    [Trait("SpriggitFile", "ActorValueInformation/Agility - 0002C7_Fallout4.esm.yaml")]
    public void Fallout4_AVIF_ShouldMatchSpriggitSample_Agility()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.ActorValueInformation,
            "Agility");
        var dto = Helpers.GetDTO<ActorValueInformationDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.ActorValueInformation,
            "0002C7:Fallout4.esm");

        Helpers.GetSpriggitField(spriggit, "Abbreviation.Count").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation.Count"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[0].String").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[0].String"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[1].String").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[1].String"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[10].Language").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[10].Language"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[10].String").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[10].String"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[2].Language").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[2].Language"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[2].String").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[2].String"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[3].Language").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[3].Language"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[3].String").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[3].String"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[4].Language").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[4].Language"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[4].String").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[4].String"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[5].Language").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[5].Language"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[5].String").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[5].String"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[6].Language").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[6].Language"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[6].String").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[6].String"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[7].Language").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[7].Language"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[7].String").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[7].String"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[8].Language").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[8].Language"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[8].String").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[8].String"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[9].Language").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[9].Language"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[9].String").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[9].String"));
        Helpers.GetSpriggitField(spriggit, "DefaultValue").ShouldBe(Helpers.GetDTOField(dto, "DefaultValue"));
        Helpers.GetSpriggitField(spriggit, "Description.Count").ShouldBe(Helpers.GetDTOField(dto, "Description.Count"));
        Helpers.GetSpriggitField(spriggit, "Description.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Description.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Description[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[0].String").ShouldBe(Helpers.GetDTOField(dto, "Description[0].String"));
        Helpers.GetSpriggitField(spriggit, "Description[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[1].String").ShouldBe(Helpers.GetDTOField(dto, "Description[1].String"));
        Helpers.GetSpriggitField(spriggit, "Description[10].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[10].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[10].String").ShouldBe(Helpers.GetDTOField(dto, "Description[10].String"));
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
        Helpers.GetSpriggitField(spriggit, "Description[9].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[9].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[9].String").ShouldBe(Helpers.GetDTOField(dto, "Description[9].String"));
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
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "Abbreviation.Count", "Abbreviation.TargetLanguage", "Abbreviation[0].Language", "Abbreviation[0].String", "Abbreviation[1].Language", "Abbreviation[1].String", "Abbreviation[10].Language", "Abbreviation[10].String", "Abbreviation[2].Language", "Abbreviation[2].String", "Abbreviation[3].Language", "Abbreviation[3].String", "Abbreviation[4].Language", "Abbreviation[4].String", "Abbreviation[5].Language", "Abbreviation[5].String", "Abbreviation[6].Language", "Abbreviation[6].String", "Abbreviation[7].Language", "Abbreviation[7].String", "Abbreviation[8].Language", "Abbreviation[8].String", "Abbreviation[9].Language", "Abbreviation[9].String", "DefaultValue", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[10].Language", "Description[10].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "Description[9].Language", "Description[9].String", "EditorID", "FormKey", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "Abbreviation.Count", "Abbreviation.TargetLanguage", "Abbreviation[0].Language", "Abbreviation[0].String", "Abbreviation[1].Language", "Abbreviation[1].String", "Abbreviation[10].Language", "Abbreviation[10].String", "Abbreviation[2].Language", "Abbreviation[2].String", "Abbreviation[3].Language", "Abbreviation[3].String", "Abbreviation[4].Language", "Abbreviation[4].String", "Abbreviation[5].Language", "Abbreviation[5].String", "Abbreviation[6].Language", "Abbreviation[6].String", "Abbreviation[7].Language", "Abbreviation[7].String", "Abbreviation[8].Language", "Abbreviation[8].String", "Abbreviation[9].Language", "Abbreviation[9].String", "DefaultValue", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[10].Language", "Description[10].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "Description[9].Language", "Description[9].String", "EditorID", "FormKey", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "Version2", "VersionControl");
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "AVIF")]
    [Trait("FormKey", "1EB998:Fallout4.esm")]
    [Trait("EditorID", "AddictionCount")]
    [Trait("SpriggitFile", "ActorValueInformation/AddictionCount - 1EB998_Fallout4.esm.yaml")]
    public void Fallout4_AVIF_ShouldMatchSpriggitSample_AddictionCount()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.ActorValueInformation,
            "AddictionCount");
        var dto = Helpers.GetDTO<ActorValueInformationDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.ActorValueInformation,
            "1EB998:Fallout4.esm");

        Helpers.GetSpriggitField(spriggit, "DefaultValue").ShouldBe(Helpers.GetDTOField(dto, "DefaultValue"));
        Helpers.GetSpriggitField(spriggit, "Description.Count").ShouldBe(Helpers.GetDTOField(dto, "Description.Count"));
        Helpers.GetSpriggitField(spriggit, "Description.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Description.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Description[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[0].String").ShouldBe(Helpers.GetDTOField(dto, "Description[0].String"));
        Helpers.GetSpriggitField(spriggit, "Description[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[1].String").ShouldBe(Helpers.GetDTOField(dto, "Description[1].String"));
        Helpers.GetSpriggitField(spriggit, "Description[10].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[10].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[10].String").ShouldBe(Helpers.GetDTOField(dto, "Description[10].String"));
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
        Helpers.GetSpriggitField(spriggit, "Description[9].Language").ShouldBe(Helpers.GetDTOField(dto, "Description[9].Language"));
        Helpers.GetSpriggitField(spriggit, "Description[9].String").ShouldBe(Helpers.GetDTOField(dto, "Description[9].String"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "Type").ShouldBe(Helpers.GetDTOField(dto, "Type"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "DefaultValue", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[10].Language", "Description[10].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "Description[9].Language", "Description[9].String", "EditorID", "FormKey", "Type", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "DefaultValue", "Description.Count", "Description.TargetLanguage", "Description[0].Language", "Description[0].String", "Description[1].Language", "Description[1].String", "Description[10].Language", "Description[10].String", "Description[2].Language", "Description[2].String", "Description[3].Language", "Description[3].String", "Description[4].Language", "Description[4].String", "Description[5].Language", "Description[5].String", "Description[6].Language", "Description[6].String", "Description[7].Language", "Description[7].String", "Description[8].Language", "Description[8].String", "Description[9].Language", "Description[9].String", "EditorID", "FormKey", "Type", "Version2", "VersionControl");
    }
}