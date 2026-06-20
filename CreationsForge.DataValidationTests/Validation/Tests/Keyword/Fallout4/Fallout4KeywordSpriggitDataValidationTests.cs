using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.Keyword.Fallout4;

public class Fallout4KeywordSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "119B9B:Fallout4.esm")]
    [Trait("EditorID", "02Metal03Floor")]
    [Trait("SpriggitFile", "Keywords/02Metal03Floor - 119B9B_Fallout4.esm.yaml")]
    public void Fallout4_KYWD_ShouldMatchSpriggitSample__02Metal03Floor()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Keyword,
            "02Metal03Floor");
        var dto = Helpers.GetDTO<KeywordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Keyword,
            "119B9B:Fallout4.esm");

        Helpers.GetSpriggitField(spriggit, "Color").ShouldBe(Helpers.GetDTOField(dto, "Color"));
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

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "Color", "EditorID", "FormKey", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "Type", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "Color", "EditorID", "FormKey", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "Type", "Version2", "VersionControl");
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "119B9C:Fallout4.esm")]
    [Trait("EditorID", "02Metal03Misc")]
    [Trait("SpriggitFile", "Keywords/02Metal03Misc - 119B9C_Fallout4.esm.yaml")]
    public void Fallout4_KYWD_ShouldMatchSpriggitSample__02Metal03Misc()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Keyword,
            "02Metal03Misc");
        var dto = Helpers.GetDTO<KeywordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Keyword,
            "119B9C:Fallout4.esm");

        Helpers.GetSpriggitField(spriggit, "Color").ShouldBe(Helpers.GetDTOField(dto, "Color"));
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

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "Color", "EditorID", "FormKey", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "Type", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "Color", "EditorID", "FormKey", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "Type", "Version2", "VersionControl");
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "119B9D:Fallout4.esm")]
    [Trait("EditorID", "02Metal03Prefabs")]
    [Trait("SpriggitFile", "Keywords/02Metal03Prefabs - 119B9D_Fallout4.esm.yaml")]
    public void Fallout4_KYWD_ShouldMatchSpriggitSample__02Metal03Prefabs()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Keyword,
            "02Metal03Prefabs");
        var dto = Helpers.GetDTO<KeywordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Keyword,
            "119B9D:Fallout4.esm");

        Helpers.GetSpriggitField(spriggit, "Color").ShouldBe(Helpers.GetDTOField(dto, "Color"));
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

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "Color", "EditorID", "FormKey", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "Type", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "Color", "EditorID", "FormKey", "Name.Count", "Name.TargetLanguage", "Name[0].Language", "Name[0].String", "Name[1].Language", "Name[1].String", "Name[10].Language", "Name[10].String", "Name[2].Language", "Name[2].String", "Name[3].Language", "Name[3].String", "Name[4].Language", "Name[4].String", "Name[5].Language", "Name[5].String", "Name[6].Language", "Name[6].String", "Name[7].Language", "Name[7].String", "Name[8].Language", "Name[8].String", "Name[9].Language", "Name[9].String", "Type", "Version2", "VersionControl");
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "0CF43E:Fallout4.esm")]
    [Trait("EditorID", "AO_BoS_ScribeCollectData")]
    [Trait("SpriggitFile", "Keywords/AO_BoS_ScribeCollectData - 0CF43E_Fallout4.esm.yaml")]
    public void Fallout4_KYWD_ShouldMatchSpriggitSample_AO_BoS_ScribeCollectData()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Keyword,
            "AO_BoS_ScribeCollectData");
        var dto = Helpers.GetDTO<KeywordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Keyword,
            "0CF43E:Fallout4.esm");

        Helpers.GetSpriggitField(spriggit, "AttractionRule").ShouldBe(Helpers.GetDTOField(dto, "AttractionRule"));
        Helpers.GetSpriggitField(spriggit, "Color").ShouldBe(Helpers.GetDTOField(dto, "Color"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "Notes").ShouldBe(Helpers.GetDTOField(dto, "Notes"));
        Helpers.GetSpriggitField(spriggit, "Type").ShouldBe(Helpers.GetDTOField(dto, "Type"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "AttractionRule", "Color", "EditorID", "FormKey", "FormVersion", "Notes", "Type", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "AttractionRule", "Color", "EditorID", "FormKey", "FormVersion", "Notes", "Type", "Version2", "VersionControl");
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "093BBE:Fallout4.esm")]
    [Trait("EditorID", "if_Armor_Combat_Freefall_Restricted")]
    [Trait("SpriggitFile", "Keywords/if_Armor_Combat_Freefall_Restricted - 093BBE_Fallout4.esm.yaml")]
    public void Fallout4_KYWD_ShouldMatchSpriggitSample_if_Armor_Combat_Freefall_Restricted()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Keyword,
            "if_Armor_Combat_Freefall_Restricted");
        var dto = Helpers.GetDTO<KeywordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Keyword,
            "093BBE:Fallout4.esm");

        Helpers.GetSpriggitField(spriggit, "Color").ShouldBe(Helpers.GetDTOField(dto, "Color"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "MajorRecordFlagsRaw").ShouldBe(Helpers.GetDTOField(dto, "MajorRecordFlags"));
        Helpers.GetSpriggitField(spriggit, "Type").ShouldBe(Helpers.GetDTOField(dto, "Type"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "Color", "EditorID", "FormKey", "FormVersion", "MajorRecordFlagsRaw", "Type", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "Color", "EditorID", "FormKey", "FormVersion", "MajorRecordFlags", "Type", "Version2", "VersionControl");
    }
}