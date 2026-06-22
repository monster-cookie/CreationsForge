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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["Color"].ShouldBe(dtoFields["Color"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["Name.Count"].ShouldBe(dtoFields["Name.Count"]);
        spriggitFields["Name.TargetLanguage"].ShouldBe(dtoFields["Name.TargetLanguage"]);
        spriggitFields["Name[0].Language"].ShouldBe(dtoFields["Name[0].Language"]);
        spriggitFields["Name[0].String"].ShouldBe(dtoFields["Name[0].String"]);
        spriggitFields["Name[1].Language"].ShouldBe(dtoFields["Name[1].Language"]);
        spriggitFields["Name[1].String"].ShouldBe(dtoFields["Name[1].String"]);
        spriggitFields["Name[10].Language"].ShouldBe(dtoFields["Name[10].Language"]);
        spriggitFields["Name[10].String"].ShouldBe(dtoFields["Name[10].String"]);
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
        spriggitFields["Name[9].Language"].ShouldBe(dtoFields["Name[9].Language"]);
        spriggitFields["Name[9].String"].ShouldBe(dtoFields["Name[9].String"]);
        spriggitFields["Type"].ShouldBe(dtoFields["Type"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["Color"].ShouldBe(dtoFields["Color"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["Name.Count"].ShouldBe(dtoFields["Name.Count"]);
        spriggitFields["Name.TargetLanguage"].ShouldBe(dtoFields["Name.TargetLanguage"]);
        spriggitFields["Name[0].Language"].ShouldBe(dtoFields["Name[0].Language"]);
        spriggitFields["Name[0].String"].ShouldBe(dtoFields["Name[0].String"]);
        spriggitFields["Name[1].Language"].ShouldBe(dtoFields["Name[1].Language"]);
        spriggitFields["Name[1].String"].ShouldBe(dtoFields["Name[1].String"]);
        spriggitFields["Name[10].Language"].ShouldBe(dtoFields["Name[10].Language"]);
        spriggitFields["Name[10].String"].ShouldBe(dtoFields["Name[10].String"]);
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
        spriggitFields["Name[9].Language"].ShouldBe(dtoFields["Name[9].Language"]);
        spriggitFields["Name[9].String"].ShouldBe(dtoFields["Name[9].String"]);
        spriggitFields["Type"].ShouldBe(dtoFields["Type"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["Color"].ShouldBe(dtoFields["Color"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["Name.Count"].ShouldBe(dtoFields["Name.Count"]);
        spriggitFields["Name.TargetLanguage"].ShouldBe(dtoFields["Name.TargetLanguage"]);
        spriggitFields["Name[0].Language"].ShouldBe(dtoFields["Name[0].Language"]);
        spriggitFields["Name[0].String"].ShouldBe(dtoFields["Name[0].String"]);
        spriggitFields["Name[1].Language"].ShouldBe(dtoFields["Name[1].Language"]);
        spriggitFields["Name[1].String"].ShouldBe(dtoFields["Name[1].String"]);
        spriggitFields["Name[10].Language"].ShouldBe(dtoFields["Name[10].Language"]);
        spriggitFields["Name[10].String"].ShouldBe(dtoFields["Name[10].String"]);
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
        spriggitFields["Name[9].Language"].ShouldBe(dtoFields["Name[9].Language"]);
        spriggitFields["Name[9].String"].ShouldBe(dtoFields["Name[9].String"]);
        spriggitFields["Type"].ShouldBe(dtoFields["Type"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["AttractionRule"].ShouldBe(dtoFields["AttractionRule"]);
        spriggitFields["Color"].ShouldBe(dtoFields["Color"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["Notes"].ShouldBe(dtoFields["Notes"]);
        spriggitFields["Type"].ShouldBe(dtoFields["Type"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["Color"].ShouldBe(dtoFields["Color"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["MajorRecordFlagsRaw"].ShouldBe(dtoFields["MajorRecordFlags"]);
        spriggitFields["Type"].ShouldBe(dtoFields["Type"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "119BA0:Fallout4.esm")]
    [Trait("EditorID", "02Metal03Wall")]
    [Trait("SpriggitFile", "Keywords/02Metal03Wall - 119BA0_Fallout4.esm.yaml")]
    public void Fallout4_KYWD_ShouldMatchSpriggitSample_02Metal03Wall()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Keyword,
            "02Metal03Wall");
        var dto = Helpers.GetDTO<KeywordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Keyword,
            "119BA0:Fallout4.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["Color"].ShouldBe(dtoFields["Color"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["Name.Count"].ShouldBe(dtoFields["Name.Count"]);
        spriggitFields["Name.TargetLanguage"].ShouldBe(dtoFields["Name.TargetLanguage"]);
        spriggitFields["Name[0].Language"].ShouldBe(dtoFields["Name[0].Language"]);
        spriggitFields["Name[0].String"].ShouldBe(dtoFields["Name[0].String"]);
        spriggitFields["Name[1].Language"].ShouldBe(dtoFields["Name[1].Language"]);
        spriggitFields["Name[1].String"].ShouldBe(dtoFields["Name[1].String"]);
        spriggitFields["Name[10].Language"].ShouldBe(dtoFields["Name[10].Language"]);
        spriggitFields["Name[10].String"].ShouldBe(dtoFields["Name[10].String"]);
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
        spriggitFields["Name[9].Language"].ShouldBe(dtoFields["Name[9].Language"]);
        spriggitFields["Name[9].String"].ShouldBe(dtoFields["Name[9].String"]);
        spriggitFields["Type"].ShouldBe(dtoFields["Type"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "1157E8:Fallout4.esm")]
    [Trait("EditorID", "ActorTypeChild")]
    [Trait("SpriggitFile", "Keywords/ActorTypeChild - 1157E8_Fallout4.esm.yaml")]
    public void Fallout4_KYWD_ShouldMatchSpriggitSample_ActorTypeChild()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Keyword,
            "ActorTypeChild");
        var dto = Helpers.GetDTO<KeywordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Keyword,
            "1157E8:Fallout4.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["Color"].ShouldBe(dtoFields["Color"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["Type"].ShouldBe(dtoFields["Type"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "03D28F:Fallout4.esm")]
    [Trait("EditorID", "AnimArchetypeNervous")]
    [Trait("SpriggitFile", "Keywords/AnimArchetypeNervous - 03D28F_Fallout4.esm.yaml")]
    public void Fallout4_KYWD_ShouldMatchSpriggitSample_AnimArchetypeNervous()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Keyword,
            "AnimArchetypeNervous");
        var dto = Helpers.GetDTO<KeywordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Keyword,
            "03D28F:Fallout4.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["Color"].ShouldBe(dtoFields["Color"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["Type"].ShouldBe(dtoFields["Type"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "072B3F:Fallout4.esm")]
    [Trait("EditorID", "ap_Bot_ModLegsSlotB")]
    [Trait("SpriggitFile", "Keywords/ap_Bot_ModLegsSlotB - 072B3F_Fallout4.esm.yaml")]
    public void Fallout4_KYWD_ShouldMatchSpriggitSample_ap_Bot_ModLegsSlotB()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Keyword,
            "ap_Bot_ModLegsSlotB");
        var dto = Helpers.GetDTO<KeywordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.Keyword,
            "072B3F:Fallout4.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["Color"].ShouldBe(dtoFields["Color"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["Type"].ShouldBe(dtoFields["Type"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }
}
