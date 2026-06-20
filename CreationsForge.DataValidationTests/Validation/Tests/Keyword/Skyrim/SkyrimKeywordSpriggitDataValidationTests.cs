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

        Helpers.GetSpriggitField(spriggit, "Color").ShouldBe(Helpers.GetDTOField(dto, "Color"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "Color", "EditorID", "FormKey", "FormVersion", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "Color", "EditorID", "FormKey", "FormVersion", "Version2", "VersionControl");
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

        Helpers.GetSpriggitField(spriggit, "Color").ShouldBe(Helpers.GetDTOField(dto, "Color"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "Color", "EditorID", "FormKey", "FormVersion", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "Color", "EditorID", "FormKey", "FormVersion", "Version2", "VersionControl");
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

        Helpers.GetSpriggitField(spriggit, "Color").ShouldBe(Helpers.GetDTOField(dto, "Color"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "Color", "EditorID", "FormKey", "FormVersion", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "Color", "EditorID", "FormKey", "FormVersion", "Version2", "VersionControl");
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

        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "EditorID", "FormKey", "FormVersion", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "EditorID", "FormKey", "FormVersion", "Version2", "VersionControl");
    }
}