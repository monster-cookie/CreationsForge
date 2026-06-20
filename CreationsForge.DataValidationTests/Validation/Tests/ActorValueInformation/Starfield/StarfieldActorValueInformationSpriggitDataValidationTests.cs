using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.ActorValueInformation.Starfield;

public class StarfieldActorValueInformationSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "AVIF")]
    [Trait("FormKey", "05ACD4:Starfield.esm")]
    [Trait("EditorID", "TargetingModeActionPoints_AV")]
    [Trait("SpriggitFile", "ActorValueInformation/TargetingModeActionPoints_AV - 05ACD4_Starfield.esm.yaml")]
    public void Starfield_AVIF_ShouldMatchSpriggitSample_TargetingModeActionPoints_AV()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.ActorValueInformation,
            "TargetingModeActionPoints_AV");
        var dto = Helpers.GetDTO<ActorValueInformationDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.ActorValueInformation,
            "05ACD4:Starfield.esm");

        Helpers.GetSpriggitField(spriggit, "Abbreviation.Count").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation.Count"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[0].String").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[0].String"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[1].String").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[1].String"));
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
        Helpers.GetSpriggitField(spriggit, "ContextNotes").ShouldBe(Helpers.GetDTOField(dto, "ContextNotes"));
        Helpers.GetSpriggitField(spriggit, "DefaultValue").ShouldBe(Helpers.GetDTOField(dto, "DefaultValue"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "Max").ShouldBe(Helpers.GetDTOField(dto, "Max"));
        Helpers.GetSpriggitField(spriggit, "Min").ShouldBe(Helpers.GetDTOField(dto, "Min"));
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
        Helpers.GetSpriggitField(spriggit, "Type").ShouldBe(Helpers.GetDTOField(dto, "Type"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "Abbreviation.Count", "Abbreviation.TargetLanguage", "Abbreviation[0].Language", "Abbreviation[0].String", "Abbreviation[1].Language", "Abbreviation[1].String", "Abbreviation[2].Language", "Abbreviation[2].String", "Abbreviation[3].Language", "Abbreviation[3].String", "Abbreviation[4].Language", "Abbreviation[4].String", "Abbreviation[5].Language", "Abbreviation[5].String", "Abbreviation[6].Language", "Abbreviation[6].String", "Abbreviation[7].Language", "Abbreviation[7].String", "Abbreviation[8].Language", "Abbreviation[8].String", "ContextNotes", "DefaultValue", "EditorID", "FormKey", "FormVersion", "Max", "Min", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Type", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "Abbreviation.Count", "Abbreviation.TargetLanguage", "Abbreviation[0].Language", "Abbreviation[0].String", "Abbreviation[1].Language", "Abbreviation[1].String", "Abbreviation[2].Language", "Abbreviation[2].String", "Abbreviation[3].Language", "Abbreviation[3].String", "Abbreviation[4].Language", "Abbreviation[4].String", "Abbreviation[5].Language", "Abbreviation[5].String", "Abbreviation[6].Language", "Abbreviation[6].String", "Abbreviation[7].Language", "Abbreviation[7].String", "Abbreviation[8].Language", "Abbreviation[8].String", "ContextNotes", "DefaultValue", "EditorID", "FormKey", "FormVersion", "Max", "Min", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Type", "Version2", "VersionControl");
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "AVIF")]
    [Trait("FormKey", "248D31:Starfield.esm")]
    [Trait("EditorID", "ENV_Resist_Airborne")]
    [Trait("SpriggitFile", "ActorValueInformation/ENV_Resist_Airborne - 248D31_Starfield.esm.yaml")]
    public void Starfield_AVIF_ShouldMatchSpriggitSample_ENV_Resist_Airborne()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.ActorValueInformation,
            "ENV_Resist_Airborne");
        var dto = Helpers.GetDTO<ActorValueInformationDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.ActorValueInformation,
            "248D31:Starfield.esm");

        Helpers.GetSpriggitField(spriggit, "Abbreviation.Count").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation.Count"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[0].String").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[0].String"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[1].String").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[1].String"));
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
        Helpers.GetSpriggitField(spriggit, "ContextNotes").ShouldBe(Helpers.GetDTOField(dto, "ContextNotes"));
        Helpers.GetSpriggitField(spriggit, "DefaultValue").ShouldBe(Helpers.GetDTOField(dto, "DefaultValue"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "Max").ShouldBe(Helpers.GetDTOField(dto, "Max"));
        Helpers.GetSpriggitField(spriggit, "Min").ShouldBe(Helpers.GetDTOField(dto, "Min"));
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
        Helpers.GetSpriggitField(spriggit, "Type").ShouldBe(Helpers.GetDTOField(dto, "Type"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "Abbreviation.Count", "Abbreviation.TargetLanguage", "Abbreviation[0].Language", "Abbreviation[0].String", "Abbreviation[1].Language", "Abbreviation[1].String", "Abbreviation[2].Language", "Abbreviation[2].String", "Abbreviation[3].Language", "Abbreviation[3].String", "Abbreviation[4].Language", "Abbreviation[4].String", "Abbreviation[5].Language", "Abbreviation[5].String", "Abbreviation[6].Language", "Abbreviation[6].String", "Abbreviation[7].Language", "Abbreviation[7].String", "Abbreviation[8].Language", "Abbreviation[8].String", "ContextNotes", "DefaultValue", "EditorID", "FormKey", "FormVersion", "Max", "Min", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Type", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "Abbreviation.Count", "Abbreviation.TargetLanguage", "Abbreviation[0].Language", "Abbreviation[0].String", "Abbreviation[1].Language", "Abbreviation[1].String", "Abbreviation[2].Language", "Abbreviation[2].String", "Abbreviation[3].Language", "Abbreviation[3].String", "Abbreviation[4].Language", "Abbreviation[4].String", "Abbreviation[5].Language", "Abbreviation[5].String", "Abbreviation[6].Language", "Abbreviation[6].String", "Abbreviation[7].Language", "Abbreviation[7].String", "Abbreviation[8].Language", "Abbreviation[8].String", "ContextNotes", "DefaultValue", "EditorID", "FormKey", "FormVersion", "Max", "Min", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Type", "Version2", "VersionControl");
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "AVIF")]
    [Trait("FormKey", "248D30:Starfield.esm")]
    [Trait("EditorID", "ENV_Resist_Corrosive")]
    [Trait("SpriggitFile", "ActorValueInformation/ENV_Resist_Corrosive - 248D30_Starfield.esm.yaml")]
    public void Starfield_AVIF_ShouldMatchSpriggitSample_ENV_Resist_Corrosive()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.ActorValueInformation,
            "ENV_Resist_Corrosive");
        var dto = Helpers.GetDTO<ActorValueInformationDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.ActorValueInformation,
            "248D30:Starfield.esm");

        Helpers.GetSpriggitField(spriggit, "Abbreviation.Count").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation.Count"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[0].String").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[0].String"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[1].String").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[1].String"));
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
        Helpers.GetSpriggitField(spriggit, "ContextNotes").ShouldBe(Helpers.GetDTOField(dto, "ContextNotes"));
        Helpers.GetSpriggitField(spriggit, "DefaultValue").ShouldBe(Helpers.GetDTOField(dto, "DefaultValue"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "Max").ShouldBe(Helpers.GetDTOField(dto, "Max"));
        Helpers.GetSpriggitField(spriggit, "Min").ShouldBe(Helpers.GetDTOField(dto, "Min"));
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
        Helpers.GetSpriggitField(spriggit, "Type").ShouldBe(Helpers.GetDTOField(dto, "Type"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "Abbreviation.Count", "Abbreviation.TargetLanguage", "Abbreviation[0].Language", "Abbreviation[0].String", "Abbreviation[1].Language", "Abbreviation[1].String", "Abbreviation[2].Language", "Abbreviation[2].String", "Abbreviation[3].Language", "Abbreviation[3].String", "Abbreviation[4].Language", "Abbreviation[4].String", "Abbreviation[5].Language", "Abbreviation[5].String", "Abbreviation[6].Language", "Abbreviation[6].String", "Abbreviation[7].Language", "Abbreviation[7].String", "Abbreviation[8].Language", "Abbreviation[8].String", "ContextNotes", "DefaultValue", "EditorID", "FormKey", "FormVersion", "Max", "Min", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Type", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "Abbreviation.Count", "Abbreviation.TargetLanguage", "Abbreviation[0].Language", "Abbreviation[0].String", "Abbreviation[1].Language", "Abbreviation[1].String", "Abbreviation[2].Language", "Abbreviation[2].String", "Abbreviation[3].Language", "Abbreviation[3].String", "Abbreviation[4].Language", "Abbreviation[4].String", "Abbreviation[5].Language", "Abbreviation[5].String", "Abbreviation[6].Language", "Abbreviation[6].String", "Abbreviation[7].Language", "Abbreviation[7].String", "Abbreviation[8].Language", "Abbreviation[8].String", "ContextNotes", "DefaultValue", "EditorID", "FormKey", "FormVersion", "Max", "Min", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Type", "Version2", "VersionControl");
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "AVIF")]
    [Trait("FormKey", "2EE0BB:Starfield.esm")]
    [Trait("EditorID", "PEO_CarryWeight")]
    [Trait("SpriggitFile", "ActorValueInformation/PEO_CarryWeight - 2EE0BB_Starfield.esm.yaml")]
    public void Starfield_AVIF_ShouldMatchSpriggitSample_PEO_CarryWeight()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.ActorValueInformation,
            "PEO_CarryWeight");
        var dto = Helpers.GetDTO<ActorValueInformationDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.ActorValueInformation,
            "2EE0BB:Starfield.esm");

        Helpers.GetSpriggitField(spriggit, "Abbreviation.Count").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation.Count"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[0].String").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[0].String"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[1].String").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[1].String"));
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
        Helpers.GetSpriggitField(spriggit, "DefaultValue").ShouldBe(Helpers.GetDTOField(dto, "DefaultValue"));
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
        Helpers.GetSpriggitField(spriggit, "Type").ShouldBe(Helpers.GetDTOField(dto, "Type"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "Abbreviation.Count", "Abbreviation.TargetLanguage", "Abbreviation[0].Language", "Abbreviation[0].String", "Abbreviation[1].Language", "Abbreviation[1].String", "Abbreviation[2].Language", "Abbreviation[2].String", "Abbreviation[3].Language", "Abbreviation[3].String", "Abbreviation[4].Language", "Abbreviation[4].String", "Abbreviation[5].Language", "Abbreviation[5].String", "Abbreviation[6].Language", "Abbreviation[6].String", "Abbreviation[7].Language", "Abbreviation[7].String", "Abbreviation[8].Language", "Abbreviation[8].String", "DefaultValue", "EditorID", "FormKey", "FormVersion", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Type", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "Abbreviation.Count", "Abbreviation.TargetLanguage", "Abbreviation[0].Language", "Abbreviation[0].String", "Abbreviation[1].Language", "Abbreviation[1].String", "Abbreviation[2].Language", "Abbreviation[2].String", "Abbreviation[3].Language", "Abbreviation[3].String", "Abbreviation[4].Language", "Abbreviation[4].String", "Abbreviation[5].Language", "Abbreviation[5].String", "Abbreviation[6].Language", "Abbreviation[6].String", "Abbreviation[7].Language", "Abbreviation[7].String", "Abbreviation[8].Language", "Abbreviation[8].String", "DefaultValue", "EditorID", "FormKey", "FormVersion", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Type", "Version2", "VersionControl");
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "AVIF")]
    [Trait("FormKey", "0002D4:Starfield.esm")]
    [Trait("EditorID", "Health")]
    [Trait("SpriggitFile", "ActorValueInformation/Health - 0002D4_Starfield.esm.yaml")]
    public void Starfield_AVIF_ShouldMatchSpriggitSample_Health()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.ActorValueInformation,
            "Health");
        var dto = Helpers.GetDTO<ActorValueInformationDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.ActorValueInformation,
            "0002D4:Starfield.esm");

        Helpers.GetSpriggitField(spriggit, "Abbreviation.Count").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation.Count"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[0].String").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[0].String"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Abbreviation[1].String").ShouldBe(Helpers.GetDTOField(dto, "Abbreviation[1].String"));
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
        Helpers.GetSpriggitField(spriggit, "DefaultValue").ShouldBe(Helpers.GetDTOField(dto, "DefaultValue"));
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
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "Abbreviation.Count", "Abbreviation.TargetLanguage", "Abbreviation[0].Language", "Abbreviation[0].String", "Abbreviation[1].Language", "Abbreviation[1].String", "Abbreviation[2].Language", "Abbreviation[2].String", "Abbreviation[3].Language", "Abbreviation[3].String", "Abbreviation[4].Language", "Abbreviation[4].String", "Abbreviation[5].Language", "Abbreviation[5].String", "Abbreviation[6].Language", "Abbreviation[6].String", "Abbreviation[7].Language", "Abbreviation[7].String", "Abbreviation[8].Language", "Abbreviation[8].String", "DefaultValue", "EditorID", "FormKey", "FormVersion", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "Abbreviation.Count", "Abbreviation.TargetLanguage", "Abbreviation[0].Language", "Abbreviation[0].String", "Abbreviation[1].Language", "Abbreviation[1].String", "Abbreviation[2].Language", "Abbreviation[2].String", "Abbreviation[3].Language", "Abbreviation[3].String", "Abbreviation[4].Language", "Abbreviation[4].String", "Abbreviation[5].Language", "Abbreviation[5].String", "Abbreviation[6].Language", "Abbreviation[6].String", "Abbreviation[7].Language", "Abbreviation[7].String", "Abbreviation[8].Language", "Abbreviation[8].String", "DefaultValue", "EditorID", "FormKey", "FormVersion", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Version2", "VersionControl");
    }
}