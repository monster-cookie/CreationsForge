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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        dtoFields["Abbreviation.Count"].ShouldBe("1");
        spriggitFields["Abbreviation.TargetLanguage"].ShouldBe(dtoFields["Abbreviation.TargetLanguage"]);
        spriggitFields["Abbreviation[1].Language"].ShouldBe(dtoFields["Abbreviation[0].Language"]);
        spriggitFields["Abbreviation[1].String"].ShouldBe(dtoFields["Abbreviation[0].String"]);
        spriggitFields["ContextNotes"].ShouldBe(dtoFields["ContextNotes"]);
        spriggitFields["DefaultValue"].ShouldBe(dtoFields["DefaultValue"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        dtoFields["Flags"].ShouldNotBeNullOrEmpty();
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["Max"].ShouldBe(dtoFields["Max"]);
        spriggitFields["Min"].ShouldBe(dtoFields["Min"]);
        dtoFields["Name.Count"].ShouldBe("1");
        spriggitFields["Name.TargetLanguage"].ShouldBe(dtoFields["Name.TargetLanguage"]);
        spriggitFields["Name[1].Language"].ShouldBe(dtoFields["Name[0].Language"]);
        spriggitFields["Name[1].String"].ShouldBe(dtoFields["Name[0].String"]);
        spriggitFields["Type"].ShouldBe(dtoFields["Type"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        dtoFields["Abbreviation.Count"].ShouldBe("1");
        spriggitFields["Abbreviation.TargetLanguage"].ShouldBe(dtoFields["Abbreviation.TargetLanguage"]);
        spriggitFields["Abbreviation[1].Language"].ShouldBe(dtoFields["Abbreviation[0].Language"]);
        spriggitFields["Abbreviation[1].String"].ShouldBe(dtoFields["Abbreviation[0].String"]);
        spriggitFields["ContextNotes"].ShouldBe(dtoFields["ContextNotes"]);
        spriggitFields["DefaultValue"].ShouldBe(dtoFields["DefaultValue"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        dtoFields["Flags"].ShouldNotBeNullOrEmpty();
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["Max"].ShouldBe(dtoFields["Max"]);
        spriggitFields["Min"].ShouldBe(dtoFields["Min"]);
        dtoFields["Name.Count"].ShouldBe("1");
        spriggitFields["Name.TargetLanguage"].ShouldBe(dtoFields["Name.TargetLanguage"]);
        spriggitFields["Name[1].Language"].ShouldBe(dtoFields["Name[0].Language"]);
        spriggitFields["Name[1].String"].ShouldBe(dtoFields["Name[0].String"]);
        spriggitFields["Type"].ShouldBe(dtoFields["Type"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        dtoFields["Abbreviation.Count"].ShouldBe("1");
        spriggitFields["Abbreviation.TargetLanguage"].ShouldBe(dtoFields["Abbreviation.TargetLanguage"]);
        spriggitFields["Abbreviation[1].Language"].ShouldBe(dtoFields["Abbreviation[0].Language"]);
        spriggitFields["Abbreviation[1].String"].ShouldBe(dtoFields["Abbreviation[0].String"]);
        spriggitFields["ContextNotes"].ShouldBe(dtoFields["ContextNotes"]);
        spriggitFields["DefaultValue"].ShouldBe(dtoFields["DefaultValue"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        dtoFields["Flags"].ShouldNotBeNullOrEmpty();
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["Max"].ShouldBe(dtoFields["Max"]);
        spriggitFields["Min"].ShouldBe(dtoFields["Min"]);
        dtoFields["Name.Count"].ShouldBe("1");
        spriggitFields["Name.TargetLanguage"].ShouldBe(dtoFields["Name.TargetLanguage"]);
        spriggitFields["Name[1].Language"].ShouldBe(dtoFields["Name[0].Language"]);
        spriggitFields["Name[1].String"].ShouldBe(dtoFields["Name[0].String"]);
        spriggitFields["Type"].ShouldBe(dtoFields["Type"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        dtoFields["Abbreviation.Count"].ShouldBe("1");
        spriggitFields["Abbreviation.TargetLanguage"].ShouldBe(dtoFields["Abbreviation.TargetLanguage"]);
        spriggitFields["Abbreviation[1].Language"].ShouldBe(dtoFields["Abbreviation[0].Language"]);
        spriggitFields["Abbreviation[1].String"].ShouldBe(dtoFields["Abbreviation[0].String"]);
        spriggitFields["DefaultValue"].ShouldBe(dtoFields["DefaultValue"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        dtoFields["Flags"].ShouldNotBeNullOrEmpty();
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        dtoFields["Name.Count"].ShouldBe("1");
        spriggitFields["Name.TargetLanguage"].ShouldBe(dtoFields["Name.TargetLanguage"]);
        spriggitFields["Name[1].Language"].ShouldBe(dtoFields["Name[0].Language"]);
        spriggitFields["Name[1].String"].ShouldBe(dtoFields["Name[0].String"]);
        spriggitFields["Type"].ShouldBe(dtoFields["Type"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        dtoFields["Abbreviation.Count"].ShouldBe("1");
        spriggitFields["Abbreviation.TargetLanguage"].ShouldBe(dtoFields["Abbreviation.TargetLanguage"]);
        spriggitFields["Abbreviation[1].Language"].ShouldBe(dtoFields["Abbreviation[0].Language"]);
        spriggitFields["Abbreviation[1].String"].ShouldBe(dtoFields["Abbreviation[0].String"]);
        spriggitFields["DefaultValue"].ShouldBe(dtoFields["DefaultValue"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        dtoFields["Flags"].ShouldNotBeNullOrEmpty();
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        dtoFields["Name.Count"].ShouldBe("1");
        spriggitFields["Name.TargetLanguage"].ShouldBe(dtoFields["Name.TargetLanguage"]);
        spriggitFields["Name[1].Language"].ShouldBe(dtoFields["Name[0].Language"]);
        spriggitFields["Name[1].String"].ShouldBe(dtoFields["Name[0].String"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }
}
