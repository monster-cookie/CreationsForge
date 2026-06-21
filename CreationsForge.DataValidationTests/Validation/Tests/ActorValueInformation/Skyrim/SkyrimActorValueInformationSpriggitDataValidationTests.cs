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
