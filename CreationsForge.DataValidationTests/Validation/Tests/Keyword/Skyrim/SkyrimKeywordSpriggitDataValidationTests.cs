using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.Keyword.Skyrim;

public class SkyrimKeywordSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "10EAD7:Skyrim.esm")]
    [Trait("EditorID", "ActorTypeFamiliar")]
    [Trait("SpriggitFile", "Keywords/ActorTypeFamiliar - 10EAD7_Skyrim.esm.yaml")]
    public void Skyrim_KYWD_ShouldMatchSpriggitSample_ActorTypeFamiliar()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Keyword,
            "ActorTypeFamiliar");
        var dto = Helpers.GetDTO<KeywordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Keyword,
            "10EAD7:Skyrim.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["Color"].ShouldBe(dtoFields["Color"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "10E984:Skyrim.esm")]
    [Trait("EditorID", "ActorTypeGiant")]
    [Trait("SpriggitFile", "Keywords/ActorTypeGiant - 10E984_Skyrim.esm.yaml")]
    public void Skyrim_KYWD_ShouldMatchSpriggitSample_ActorTypeGiant()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Keyword,
            "ActorTypeGiant");
        var dto = Helpers.GetDTO<KeywordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Keyword,
            "10E984:Skyrim.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["Color"].ShouldBe(dtoFields["Color"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "0F5D16:Skyrim.esm")]
    [Trait("EditorID", "ActorTypeTroll")]
    [Trait("SpriggitFile", "Keywords/ActorTypeTroll - 0F5D16_Skyrim.esm.yaml")]
    public void Skyrim_KYWD_ShouldMatchSpriggitSample_ActorTypeTroll()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Keyword,
            "ActorTypeTroll");
        var dto = Helpers.GetDTO<KeywordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Keyword,
            "0F5D16:Skyrim.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["Color"].ShouldBe(dtoFields["Color"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "06DEAD:Skyrim.esm")]
    [Trait("EditorID", "ActivatorLever")]
    [Trait("SpriggitFile", "Keywords/ActivatorLever - 06DEAD_Skyrim.esm.yaml")]
    public void Skyrim_KYWD_ShouldMatchSpriggitSample_ActivatorLever()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Keyword,
            "ActivatorLever");
        var dto = Helpers.GetDTO<KeywordDTO>(
            SupportedGame.Skyrim,
            RecordTypeCatalog.Keyword,
            "06DEAD:Skyrim.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }
}
