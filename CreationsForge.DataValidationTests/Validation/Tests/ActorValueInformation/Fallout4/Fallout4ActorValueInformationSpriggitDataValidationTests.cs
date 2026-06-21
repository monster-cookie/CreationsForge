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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["DefaultValue"].ShouldBe(dtoFields["DefaultValue"]);
        dtoFields["Flags"].ShouldNotBeNullOrEmpty();
        spriggitFields["Type"].ShouldBe(dtoFields["Type"]);
        spriggitFields["Description.Count"].ShouldBe(dtoFields["Description.Count"]);
        spriggitFields["Description.TargetLanguage"].ShouldBe(dtoFields["Description.TargetLanguage"]);
        spriggitFields["Description[0].Language"].ShouldBe(dtoFields["Description[0].Language"]);
        spriggitFields["Description[0].String"].ShouldBe(dtoFields["Description[0].String"]);
        spriggitFields["Description[1].Language"].ShouldBe(dtoFields["Description[1].Language"]);
        spriggitFields["Description[1].String"].ShouldBe(dtoFields["Description[1].String"]);
        spriggitFields["Description[2].Language"].ShouldBe(dtoFields["Description[2].Language"]);
        spriggitFields["Description[2].String"].ShouldBe(dtoFields["Description[2].String"]);
        spriggitFields["Description[3].Language"].ShouldBe(dtoFields["Description[3].Language"]);
        spriggitFields["Description[3].String"].ShouldBe(dtoFields["Description[3].String"]);
        spriggitFields["Description[4].Language"].ShouldBe(dtoFields["Description[4].Language"]);
        spriggitFields["Description[4].String"].ShouldBe(dtoFields["Description[4].String"]);
        spriggitFields["Description[5].Language"].ShouldBe(dtoFields["Description[5].Language"]);
        spriggitFields["Description[5].String"].ShouldBe(dtoFields["Description[5].String"]);
        spriggitFields["Description[6].Language"].ShouldBe(dtoFields["Description[6].Language"]);
        spriggitFields["Description[6].String"].ShouldBe(dtoFields["Description[6].String"]);
        spriggitFields["Description[7].Language"].ShouldBe(dtoFields["Description[7].Language"]);
        spriggitFields["Description[7].String"].ShouldBe(dtoFields["Description[7].String"]);
        spriggitFields["Description[8].Language"].ShouldBe(dtoFields["Description[8].Language"]);
        spriggitFields["Description[8].String"].ShouldBe(dtoFields["Description[8].String"]);
        spriggitFields["Description[9].Language"].ShouldBe(dtoFields["Description[9].Language"]);
        spriggitFields["Description[9].String"].ShouldBe(dtoFields["Description[9].String"]);
        spriggitFields["Description[10].Language"].ShouldBe(dtoFields["Description[10].Language"]);
        spriggitFields["Description[10].String"].ShouldBe(dtoFields["Description[10].String"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["DefaultValue"].ShouldBe(dtoFields["DefaultValue"]);
        dtoFields["Flags"].ShouldNotBeNullOrEmpty();
        spriggitFields["Type"].ShouldBe(dtoFields["Type"]);
        spriggitFields["Description.Count"].ShouldBe(dtoFields["Description.Count"]);
        spriggitFields["Description.TargetLanguage"].ShouldBe(dtoFields["Description.TargetLanguage"]);
        spriggitFields["Description[0].Language"].ShouldBe(dtoFields["Description[0].Language"]);
        spriggitFields["Description[0].String"].ShouldBe(dtoFields["Description[0].String"]);
        spriggitFields["Description[1].Language"].ShouldBe(dtoFields["Description[1].Language"]);
        spriggitFields["Description[1].String"].ShouldBe(dtoFields["Description[1].String"]);
        spriggitFields["Description[2].Language"].ShouldBe(dtoFields["Description[2].Language"]);
        spriggitFields["Description[2].String"].ShouldBe(dtoFields["Description[2].String"]);
        spriggitFields["Description[3].Language"].ShouldBe(dtoFields["Description[3].Language"]);
        spriggitFields["Description[3].String"].ShouldBe(dtoFields["Description[3].String"]);
        spriggitFields["Description[4].Language"].ShouldBe(dtoFields["Description[4].Language"]);
        spriggitFields["Description[4].String"].ShouldBe(dtoFields["Description[4].String"]);
        spriggitFields["Description[5].Language"].ShouldBe(dtoFields["Description[5].Language"]);
        spriggitFields["Description[5].String"].ShouldBe(dtoFields["Description[5].String"]);
        spriggitFields["Description[6].Language"].ShouldBe(dtoFields["Description[6].Language"]);
        spriggitFields["Description[6].String"].ShouldBe(dtoFields["Description[6].String"]);
        spriggitFields["Description[7].Language"].ShouldBe(dtoFields["Description[7].Language"]);
        spriggitFields["Description[7].String"].ShouldBe(dtoFields["Description[7].String"]);
        spriggitFields["Description[8].Language"].ShouldBe(dtoFields["Description[8].Language"]);
        spriggitFields["Description[8].String"].ShouldBe(dtoFields["Description[8].String"]);
        spriggitFields["Description[9].Language"].ShouldBe(dtoFields["Description[9].Language"]);
        spriggitFields["Description[9].String"].ShouldBe(dtoFields["Description[9].String"]);
        spriggitFields["Description[10].Language"].ShouldBe(dtoFields["Description[10].Language"]);
        spriggitFields["Description[10].String"].ShouldBe(dtoFields["Description[10].String"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["DefaultValue"].ShouldBe(dtoFields["DefaultValue"]);
        dtoFields["Flags"].ShouldNotBeNullOrEmpty();
        spriggitFields["Type"].ShouldBe(dtoFields["Type"]);
        spriggitFields["Description.Count"].ShouldBe(dtoFields["Description.Count"]);
        spriggitFields["Description.TargetLanguage"].ShouldBe(dtoFields["Description.TargetLanguage"]);
        spriggitFields["Description[0].Language"].ShouldBe(dtoFields["Description[0].Language"]);
        spriggitFields["Description[0].String"].ShouldBe(dtoFields["Description[0].String"]);
        spriggitFields["Description[1].Language"].ShouldBe(dtoFields["Description[1].Language"]);
        spriggitFields["Description[1].String"].ShouldBe(dtoFields["Description[1].String"]);
        spriggitFields["Description[2].Language"].ShouldBe(dtoFields["Description[2].Language"]);
        spriggitFields["Description[2].String"].ShouldBe(dtoFields["Description[2].String"]);
        spriggitFields["Description[3].Language"].ShouldBe(dtoFields["Description[3].Language"]);
        spriggitFields["Description[3].String"].ShouldBe(dtoFields["Description[3].String"]);
        spriggitFields["Description[4].Language"].ShouldBe(dtoFields["Description[4].Language"]);
        spriggitFields["Description[4].String"].ShouldBe(dtoFields["Description[4].String"]);
        spriggitFields["Description[5].Language"].ShouldBe(dtoFields["Description[5].Language"]);
        spriggitFields["Description[5].String"].ShouldBe(dtoFields["Description[5].String"]);
        spriggitFields["Description[6].Language"].ShouldBe(dtoFields["Description[6].Language"]);
        spriggitFields["Description[6].String"].ShouldBe(dtoFields["Description[6].String"]);
        spriggitFields["Description[7].Language"].ShouldBe(dtoFields["Description[7].Language"]);
        spriggitFields["Description[7].String"].ShouldBe(dtoFields["Description[7].String"]);
        spriggitFields["Description[8].Language"].ShouldBe(dtoFields["Description[8].Language"]);
        spriggitFields["Description[8].String"].ShouldBe(dtoFields["Description[8].String"]);
        spriggitFields["Description[9].Language"].ShouldBe(dtoFields["Description[9].Language"]);
        spriggitFields["Description[9].String"].ShouldBe(dtoFields["Description[9].String"]);
        spriggitFields["Description[10].Language"].ShouldBe(dtoFields["Description[10].Language"]);
        spriggitFields["Description[10].String"].ShouldBe(dtoFields["Description[10].String"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["DefaultValue"].ShouldBe(dtoFields["DefaultValue"]);
        dtoFields["Flags"].ShouldNotBeNullOrEmpty();
        spriggitFields["Description.Count"].ShouldBe(dtoFields["Description.Count"]);
        spriggitFields["Description.TargetLanguage"].ShouldBe(dtoFields["Description.TargetLanguage"]);
        spriggitFields["Description[0].Language"].ShouldBe(dtoFields["Description[0].Language"]);
        spriggitFields["Description[0].String"].ShouldBe(dtoFields["Description[0].String"]);
        spriggitFields["Description[1].Language"].ShouldBe(dtoFields["Description[1].Language"]);
        spriggitFields["Description[1].String"].ShouldBe(dtoFields["Description[1].String"]);
        spriggitFields["Description[2].Language"].ShouldBe(dtoFields["Description[2].Language"]);
        spriggitFields["Description[2].String"].ShouldBe(dtoFields["Description[2].String"]);
        spriggitFields["Description[3].Language"].ShouldBe(dtoFields["Description[3].Language"]);
        spriggitFields["Description[3].String"].ShouldBe(dtoFields["Description[3].String"]);
        spriggitFields["Description[4].Language"].ShouldBe(dtoFields["Description[4].Language"]);
        spriggitFields["Description[4].String"].ShouldBe(dtoFields["Description[4].String"]);
        spriggitFields["Description[5].Language"].ShouldBe(dtoFields["Description[5].Language"]);
        spriggitFields["Description[5].String"].ShouldBe(dtoFields["Description[5].String"]);
        spriggitFields["Description[6].Language"].ShouldBe(dtoFields["Description[6].Language"]);
        spriggitFields["Description[6].String"].ShouldBe(dtoFields["Description[6].String"]);
        spriggitFields["Description[7].Language"].ShouldBe(dtoFields["Description[7].Language"]);
        spriggitFields["Description[7].String"].ShouldBe(dtoFields["Description[7].String"]);
        spriggitFields["Description[8].Language"].ShouldBe(dtoFields["Description[8].Language"]);
        spriggitFields["Description[8].String"].ShouldBe(dtoFields["Description[8].String"]);
        spriggitFields["Description[9].Language"].ShouldBe(dtoFields["Description[9].Language"]);
        spriggitFields["Description[9].String"].ShouldBe(dtoFields["Description[9].String"]);
        spriggitFields["Description[10].Language"].ShouldBe(dtoFields["Description[10].Language"]);
        spriggitFields["Description[10].String"].ShouldBe(dtoFields["Description[10].String"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["DefaultValue"].ShouldBe(dtoFields["DefaultValue"]);
        dtoFields["Flags"].ShouldNotBeNullOrEmpty();
        spriggitFields["Type"].ShouldBe(dtoFields["Type"]);
        spriggitFields["Description.Count"].ShouldBe(dtoFields["Description.Count"]);
        spriggitFields["Description.TargetLanguage"].ShouldBe(dtoFields["Description.TargetLanguage"]);
        spriggitFields["Description[0].Language"].ShouldBe(dtoFields["Description[0].Language"]);
        spriggitFields["Description[0].String"].ShouldBe(dtoFields["Description[0].String"]);
        spriggitFields["Description[1].Language"].ShouldBe(dtoFields["Description[1].Language"]);
        spriggitFields["Description[1].String"].ShouldBe(dtoFields["Description[1].String"]);
        spriggitFields["Description[2].Language"].ShouldBe(dtoFields["Description[2].Language"]);
        spriggitFields["Description[2].String"].ShouldBe(dtoFields["Description[2].String"]);
        spriggitFields["Description[3].Language"].ShouldBe(dtoFields["Description[3].Language"]);
        spriggitFields["Description[3].String"].ShouldBe(dtoFields["Description[3].String"]);
        spriggitFields["Description[4].Language"].ShouldBe(dtoFields["Description[4].Language"]);
        spriggitFields["Description[4].String"].ShouldBe(dtoFields["Description[4].String"]);
        spriggitFields["Description[5].Language"].ShouldBe(dtoFields["Description[5].Language"]);
        spriggitFields["Description[5].String"].ShouldBe(dtoFields["Description[5].String"]);
        spriggitFields["Description[6].Language"].ShouldBe(dtoFields["Description[6].Language"]);
        spriggitFields["Description[6].String"].ShouldBe(dtoFields["Description[6].String"]);
        spriggitFields["Description[7].Language"].ShouldBe(dtoFields["Description[7].Language"]);
        spriggitFields["Description[7].String"].ShouldBe(dtoFields["Description[7].String"]);
        spriggitFields["Description[8].Language"].ShouldBe(dtoFields["Description[8].Language"]);
        spriggitFields["Description[8].String"].ShouldBe(dtoFields["Description[8].String"]);
        spriggitFields["Description[9].Language"].ShouldBe(dtoFields["Description[9].Language"]);
        spriggitFields["Description[9].String"].ShouldBe(dtoFields["Description[9].String"]);
        spriggitFields["Description[10].Language"].ShouldBe(dtoFields["Description[10].Language"]);
        spriggitFields["Description[10].String"].ShouldBe(dtoFields["Description[10].String"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }
}
