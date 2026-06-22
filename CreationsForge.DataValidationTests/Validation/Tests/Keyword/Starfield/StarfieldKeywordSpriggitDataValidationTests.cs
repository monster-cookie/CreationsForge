using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.Keyword.Starfield;

public class StarfieldKeywordSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "200AEB:Starfield.esm")]
    [Trait("EditorID", "CCT_Enviro_AmbusherSurface")]
    [Trait("SpriggitFile", "Keywords/CCT_Enviro_AmbusherSurface - 200AEB_Starfield.esm.yaml")]
    public void Starfield_KYWD_ShouldMatchSpriggitSample_CCT_Enviro_AmbusherSurface()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Keyword,
            "CCT_Enviro_AmbusherSurface");
        var dto = Helpers.GetDTO<KeywordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Keyword,
            "200AEB:Starfield.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["AttractionRule"].ShouldBe(dtoFields["AttractionRule"]);
        spriggitFields["Color"].ShouldBe(dtoFields["Color"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FNAM"].ShouldBe(dtoFields["FNAM"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
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
        spriggitFields["Type"].ShouldBe(dtoFields["Type"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "145388:Starfield.esm")]
    [Trait("EditorID", "CCT_Enviro_AmbusherUnderground")]
    [Trait("SpriggitFile", "Keywords/CCT_Enviro_AmbusherUnderground - 145388_Starfield.esm.yaml")]
    public void Starfield_KYWD_ShouldMatchSpriggitSample_CCT_Enviro_AmbusherUnderground()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Keyword,
            "CCT_Enviro_AmbusherUnderground");
        var dto = Helpers.GetDTO<KeywordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Keyword,
            "145388:Starfield.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["AttractionRule"].ShouldBe(dtoFields["AttractionRule"]);
        spriggitFields["Color"].ShouldBe(dtoFields["Color"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FNAM"].ShouldBe(dtoFields["FNAM"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
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
        spriggitFields["Type"].ShouldBe(dtoFields["Type"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "200ADF:Starfield.esm")]
    [Trait("EditorID", "CCT_Enviro_Basking")]
    [Trait("SpriggitFile", "Keywords/CCT_Enviro_Basking - 200ADF_Starfield.esm.yaml")]
    public void Starfield_KYWD_ShouldMatchSpriggitSample_CCT_Enviro_Basking()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Keyword,
            "CCT_Enviro_Basking");
        var dto = Helpers.GetDTO<KeywordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Keyword,
            "200ADF:Starfield.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["AttractionRule"].ShouldBe(dtoFields["AttractionRule"]);
        spriggitFields["Color"].ShouldBe(dtoFields["Color"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FNAM"].ShouldBe(dtoFields["FNAM"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
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
        spriggitFields["Type"].ShouldBe(dtoFields["Type"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "1C84DD:Starfield.esm")]
    [Trait("EditorID", "WeaponTypeDisplay_ElectromagneticRifle")]
    [Trait("SpriggitFile", "Keywords/WeaponTypeDisplay_ElectromagneticRifle - 1C84DD_Starfield.esm.yaml")]
    public void Starfield_KYWD_ShouldMatchSpriggitSample_WeaponTypeDisplay_ElectromagneticRifle()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Keyword,
            "WeaponTypeDisplay_ElectromagneticRifle");
        var dto = Helpers.GetDTO<KeywordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Keyword,
            "1C84DD:Starfield.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["Color"].ShouldBe(dtoFields["Color"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FNAM"].ShouldBe(dtoFields["FNAM"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
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
        spriggitFields["Notes"].ShouldBe(dtoFields["Notes"]);
        spriggitFields["Type"].ShouldBe(dtoFields["Type"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);
        spriggitFields["WAIM"].ShouldBe(dtoFields["WAIM"]);
        spriggitFields["WFIR"].ShouldBe(dtoFields["WFIR"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "200AE9:Starfield.esm")]
    [Trait("EditorID", "CCT_Enviro_Spook")]
    [Trait("SpriggitFile", "Keywords/CCT_Enviro_Spook - 200AE9_Starfield.esm.yaml")]
    public void Starfield_KYWD_ShouldMatchSpriggitSample_CCT_Enviro_Spook()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Keyword,
            "CCT_Enviro_Spook");
        var dto = Helpers.GetDTO<KeywordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Keyword,
            "200AE9:Starfield.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["AttractionRule"].ShouldBe(dtoFields["AttractionRule"]);
        spriggitFields["Color"].ShouldBe(dtoFields["Color"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FNAM"].ShouldBe(dtoFields["FNAM"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["Type"].ShouldBe(dtoFields["Type"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "0345AE:Starfield.esm")]
    [Trait("EditorID", "ActorAttackInjuredLeft")]
    [Trait("SpriggitFile", "Keywords/ActorAttackInjuredLeft - 0345AE_Starfield.esm.yaml")]
    public void Starfield_KYWD_ShouldMatchSpriggitSample_ActorAttackInjuredLeft()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Keyword,
            "ActorAttackInjuredLeft");
        var dto = Helpers.GetDTO<KeywordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Keyword,
            "0345AE:Starfield.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["Color"].ShouldBe(dtoFields["Color"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FNAM"].ShouldBe(dtoFields["FNAM"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["Type"].ShouldBe(dtoFields["Type"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "1157E8:Starfield.esm")]
    [Trait("EditorID", "ActorTypeChild")]
    [Trait("SpriggitFile", "Keywords/ActorTypeChild - 1157E8_Starfield.esm.yaml")]
    public void Starfield_KYWD_ShouldMatchSpriggitSample_ActorTypeChild()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Keyword,
            "ActorTypeChild");
        var dto = Helpers.GetDTO<KeywordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Keyword,
            "1157E8:Starfield.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["Color"].ShouldBe(dtoFields["Color"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FNAM"].ShouldBe(dtoFields["FNAM"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["Type"].ShouldBe(dtoFields["Type"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "24E96F:Starfield.esm")]
    [Trait("EditorID", "AnimArchetypeEyeDown")]
    [Trait("SpriggitFile", "Keywords/AnimArchetypeEyeDown - 24E96F_Starfield.esm.yaml")]
    public void Starfield_KYWD_ShouldMatchSpriggitSample_AnimArchetypeEyeDown()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Keyword,
            "AnimArchetypeEyeDown");
        var dto = Helpers.GetDTO<KeywordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Keyword,
            "24E96F:Starfield.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["Color"].ShouldBe(dtoFields["Color"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FNAM"].ShouldBe(dtoFields["FNAM"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["Type"].ShouldBe(dtoFields["Type"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "157D41:Starfield.esm")]
    [Trait("EditorID", "ap_AVM_Armor_Skin")]
    [Trait("SpriggitFile", "Keywords/ap_AVM_Armor_Skin - 157D41_Starfield.esm.yaml")]
    public void Starfield_KYWD_ShouldMatchSpriggitSample_ap_AVM_Armor_Skin()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Keyword,
            "ap_AVM_Armor_Skin");
        var dto = Helpers.GetDTO<KeywordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.Keyword,
            "157D41:Starfield.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["Color"].ShouldBe(dtoFields["Color"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FlashLinkageName"].ShouldBe(dtoFields["FlashLinkageName"]);
        spriggitFields["FNAM"].ShouldBe(dtoFields["FNAM"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
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
        spriggitFields["Type"].ShouldBe(dtoFields["Type"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }
}
