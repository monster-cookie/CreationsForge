using System.Globalization;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.GameSetting.Starfield;

public class StarfieldGameSettingSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "0657E0:Starfield.esm")]
    [Trait("EditorID", "sAbort")]
    [Trait("SpriggitFile", "GameSettings/sAbort - 0657E0_Starfield.esm.yaml")]
    public void Starfield_GMST_ShouldMatchSpriggitSample_sAbort()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.GameSetting,
            "sAbort");
        var dto = Helpers.GetDTO<GameSettingDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.GameSetting,
            "0657E0:Starfield.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["Data.Count"].ShouldBe(dtoFields["Data.Count"]);
        spriggitFields["Data.TargetLanguage"].ShouldBe(dtoFields["Data.TargetLanguage"]);
        spriggitFields["Data[0].Language"].ShouldBe(dtoFields["Data[0].Language"]);
        spriggitFields["Data[0].String"].ShouldBe(dtoFields["Data[0].String"]);
        spriggitFields["Data[1].Language"].ShouldBe(dtoFields["Data[1].Language"]);
        spriggitFields["Data[1].String"].ShouldBe(dtoFields["Data[1].String"]);
        spriggitFields["Data[2].Language"].ShouldBe(dtoFields["Data[2].Language"]);
        spriggitFields["Data[2].String"].ShouldBe(dtoFields["Data[2].String"]);
        spriggitFields["Data[3].Language"].ShouldBe(dtoFields["Data[3].Language"]);
        spriggitFields["Data[3].String"].ShouldBe(dtoFields["Data[3].String"]);
        spriggitFields["Data[4].Language"].ShouldBe(dtoFields["Data[4].Language"]);
        spriggitFields["Data[4].String"].ShouldBe(dtoFields["Data[4].String"]);
        spriggitFields["Data[5].Language"].ShouldBe(dtoFields["Data[5].Language"]);
        spriggitFields["Data[5].String"].ShouldBe(dtoFields["Data[5].String"]);
        spriggitFields["Data[6].Language"].ShouldBe(dtoFields["Data[6].Language"]);
        spriggitFields["Data[6].String"].ShouldBe(dtoFields["Data[6].String"]);
        spriggitFields["Data[7].Language"].ShouldBe(dtoFields["Data[7].Language"]);
        spriggitFields["Data[7].String"].ShouldBe(dtoFields["Data[7].String"]);
        spriggitFields["Data[8].Language"].ShouldBe(dtoFields["Data[8].Language"]);
        spriggitFields["Data[8].String"].ShouldBe(dtoFields["Data[8].String"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["MutagenObjectType"].ShouldBe(dtoFields["MutagenObjectType"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "0D4DFC:Starfield.esm")]
    [Trait("EditorID", "sActivate")]
    [Trait("SpriggitFile", "GameSettings/sActivate - 0D4DFC_Starfield.esm.yaml")]
    public void Starfield_GMST_ShouldMatchSpriggitSample_sActivate()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.GameSetting,
            "sActivate");
        var dto = Helpers.GetDTO<GameSettingDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.GameSetting,
            "0D4DFC:Starfield.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["Data.Count"].ShouldBe(dtoFields["Data.Count"]);
        spriggitFields["Data.TargetLanguage"].ShouldBe(dtoFields["Data.TargetLanguage"]);
        spriggitFields["Data[0].Language"].ShouldBe(dtoFields["Data[0].Language"]);
        spriggitFields["Data[0].String"].ShouldBe(dtoFields["Data[0].String"]);
        spriggitFields["Data[1].Language"].ShouldBe(dtoFields["Data[1].Language"]);
        spriggitFields["Data[1].String"].ShouldBe(dtoFields["Data[1].String"]);
        spriggitFields["Data[2].Language"].ShouldBe(dtoFields["Data[2].Language"]);
        spriggitFields["Data[2].String"].ShouldBe(dtoFields["Data[2].String"]);
        spriggitFields["Data[3].Language"].ShouldBe(dtoFields["Data[3].Language"]);
        spriggitFields["Data[3].String"].ShouldBe(dtoFields["Data[3].String"]);
        spriggitFields["Data[4].Language"].ShouldBe(dtoFields["Data[4].Language"]);
        spriggitFields["Data[4].String"].ShouldBe(dtoFields["Data[4].String"]);
        spriggitFields["Data[5].Language"].ShouldBe(dtoFields["Data[5].Language"]);
        spriggitFields["Data[5].String"].ShouldBe(dtoFields["Data[5].String"]);
        spriggitFields["Data[6].Language"].ShouldBe(dtoFields["Data[6].Language"]);
        spriggitFields["Data[6].String"].ShouldBe(dtoFields["Data[6].String"]);
        spriggitFields["Data[7].Language"].ShouldBe(dtoFields["Data[7].Language"]);
        spriggitFields["Data[7].String"].ShouldBe(dtoFields["Data[7].String"]);
        spriggitFields["Data[8].Language"].ShouldBe(dtoFields["Data[8].Language"]);
        spriggitFields["Data[8].String"].ShouldBe(dtoFields["Data[8].String"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["MutagenObjectType"].ShouldBe(dtoFields["MutagenObjectType"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "0D4DEB:Starfield.esm")]
    [Trait("EditorID", "sActivateCreatureCalmed")]
    [Trait("SpriggitFile", "GameSettings/sActivateCreatureCalmed - 0D4DEB_Starfield.esm.yaml")]
    public void Starfield_GMST_ShouldMatchSpriggitSample_sActivateCreatureCalmed()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.GameSetting,
            "sActivateCreatureCalmed");
        var dto = Helpers.GetDTO<GameSettingDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.GameSetting,
            "0D4DEB:Starfield.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["Data.Count"].ShouldBe(dtoFields["Data.Count"]);
        spriggitFields["Data.TargetLanguage"].ShouldBe(dtoFields["Data.TargetLanguage"]);
        spriggitFields["Data[0].Language"].ShouldBe(dtoFields["Data[0].Language"]);
        spriggitFields["Data[0].String"].ShouldBe(dtoFields["Data[0].String"]);
        spriggitFields["Data[1].Language"].ShouldBe(dtoFields["Data[1].Language"]);
        spriggitFields["Data[1].String"].ShouldBe(dtoFields["Data[1].String"]);
        spriggitFields["Data[2].Language"].ShouldBe(dtoFields["Data[2].Language"]);
        spriggitFields["Data[2].String"].ShouldBe(dtoFields["Data[2].String"]);
        spriggitFields["Data[3].Language"].ShouldBe(dtoFields["Data[3].Language"]);
        spriggitFields["Data[3].String"].ShouldBe(dtoFields["Data[3].String"]);
        spriggitFields["Data[4].Language"].ShouldBe(dtoFields["Data[4].Language"]);
        spriggitFields["Data[4].String"].ShouldBe(dtoFields["Data[4].String"]);
        spriggitFields["Data[5].Language"].ShouldBe(dtoFields["Data[5].Language"]);
        spriggitFields["Data[5].String"].ShouldBe(dtoFields["Data[5].String"]);
        spriggitFields["Data[6].Language"].ShouldBe(dtoFields["Data[6].Language"]);
        spriggitFields["Data[6].String"].ShouldBe(dtoFields["Data[6].String"]);
        spriggitFields["Data[7].Language"].ShouldBe(dtoFields["Data[7].Language"]);
        spriggitFields["Data[7].String"].ShouldBe(dtoFields["Data[7].String"]);
        spriggitFields["Data[8].Language"].ShouldBe(dtoFields["Data[8].Language"]);
        spriggitFields["Data[8].String"].ShouldBe(dtoFields["Data[8].String"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["MutagenObjectType"].ShouldBe(dtoFields["MutagenObjectType"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "0F9CFD:Starfield.esm")]
    [Trait("EditorID", "bAllowBlinksDuringSpeech")]
    [Trait("SpriggitFile", "GameSettings/bAllowBlinksDuringSpeech - 0F9CFD_Starfield.esm.yaml")]
    public void Starfield_GMST_ShouldMatchSpriggitSample_bAllowBlinksDuringSpeech()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.GameSetting,
            "bAllowBlinksDuringSpeech");
        var dto = Helpers.GetDTO<GameSettingDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.GameSetting,
            "0F9CFD:Starfield.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["Data"].ShouldBe(dtoFields["Data"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["MutagenObjectType"].ShouldBe(dtoFields["MutagenObjectType"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "024CA5:Starfield.esm")]
    [Trait("EditorID", "bBoostpackInitialThrustOnlyOnTakeoff")]
    [Trait("SpriggitFile", "GameSettings/bBoostpackInitialThrustOnlyOnTakeoff - 024CA5_Starfield.esm.yaml")]
    public void Starfield_GMST_ShouldMatchSpriggitSample_bBoostpackInitialThrustOnlyOnTakeoff()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.GameSetting,
            "bBoostpackInitialThrustOnlyOnTakeoff");
        var dto = Helpers.GetDTO<GameSettingDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.GameSetting,
            "024CA5:Starfield.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["Data"].ShouldBe(dtoFields["Data"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["MutagenObjectType"].ShouldBe(dtoFields["MutagenObjectType"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "101046:Starfield.esm")]
    [Trait("EditorID", "fActorDefaultTurningSpeed")]
    [Trait("SpriggitFile", "GameSettings/fActorDefaultTurningSpeed - 101046_Starfield.esm.yaml")]
    public void Starfield_GMST_ShouldMatchSpriggitSample_fActorDefaultTurningSpeed()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.GameSetting,
            "fActorDefaultTurningSpeed");
        var dto = Helpers.GetDTO<GameSettingDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.GameSetting,
            "101046:Starfield.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        double.Parse(spriggitFields["Data"], CultureInfo.InvariantCulture)
            .ShouldBe(double.Parse(dtoFields["Data"], CultureInfo.InvariantCulture), 0.000001);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["MutagenObjectType"].ShouldBe(dtoFields["MutagenObjectType"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "097F48:Starfield.esm")]
    [Trait("EditorID", "fActorSwimBreathDamage")]
    [Trait("SpriggitFile", "GameSettings/fActorSwimBreathDamage - 097F48_Starfield.esm.yaml")]
    public void Starfield_GMST_ShouldMatchSpriggitSample_fActorSwimBreathDamage()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.GameSetting,
            "fActorSwimBreathDamage");
        var dto = Helpers.GetDTO<GameSettingDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.GameSetting,
            "097F48:Starfield.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        double.Parse(spriggitFields["Data"], CultureInfo.InvariantCulture)
            .ShouldBe(double.Parse(dtoFields["Data"], CultureInfo.InvariantCulture), 0.000001);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["MutagenObjectType"].ShouldBe(dtoFields["MutagenObjectType"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "01A237:Starfield.esm")]
    [Trait("EditorID", "iAICombatRestoreHealthPercentage")]
    [Trait("SpriggitFile", "GameSettings/iAICombatRestoreHealthPercentage - 01A237_Starfield.esm.yaml")]
    public void Starfield_GMST_ShouldMatchSpriggitSample_iAICombatRestoreHealthPercentage()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.GameSetting,
            "iAICombatRestoreHealthPercentage");
        var dto = Helpers.GetDTO<GameSettingDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.GameSetting,
            "01A237:Starfield.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["Data"].ShouldBe(dtoFields["Data"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["MutagenObjectType"].ShouldBe(dtoFields["MutagenObjectType"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "003207:Starfield.esm")]
    [Trait("EditorID", "iAIMaxSocialDistanceToTriggerEvent")]
    [Trait("SpriggitFile", "GameSettings/iAIMaxSocialDistanceToTriggerEvent - 003207_Starfield.esm.yaml")]
    public void Starfield_GMST_ShouldMatchSpriggitSample_iAIMaxSocialDistanceToTriggerEvent()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.GameSetting,
            "iAIMaxSocialDistanceToTriggerEvent");
        var dto = Helpers.GetDTO<GameSettingDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.GameSetting,
            "003207:Starfield.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["Data"].ShouldBe(dtoFields["Data"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["MutagenObjectType"].ShouldBe(dtoFields["MutagenObjectType"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "246BD8:Starfield.esm")]
    [Trait("EditorID", "uDefaultLevelZone01max")]
    [Trait("SpriggitFile", "GameSettings/uDefaultLevelZone01max - 246BD8_Starfield.esm.yaml")]
    public void Starfield_GMST_ShouldMatchSpriggitSample_uDefaultLevelZone01max()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.GameSetting,
            "uDefaultLevelZone01max");
        var dto = Helpers.GetDTO<GameSettingDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.GameSetting,
            "246BD8:Starfield.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["Data"].ShouldBe(dtoFields["Data"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["MutagenObjectType"].ShouldBe(dtoFields["MutagenObjectType"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "246BD9:Starfield.esm")]
    [Trait("EditorID", "uDefaultLevelZone02min")]
    [Trait("SpriggitFile", "GameSettings/uDefaultLevelZone02min - 246BD9_Starfield.esm.yaml")]
    public void Starfield_GMST_ShouldMatchSpriggitSample_uDefaultLevelZone02min()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.GameSetting,
            "uDefaultLevelZone02min");
        var dto = Helpers.GetDTO<GameSettingDTO>(
            SupportedGame.Starfield,
            RecordTypeCatalog.GameSetting,
            "246BD9:Starfield.esm");

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["Data"].ShouldBe(dtoFields["Data"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["FormVersion"].ShouldBe(dtoFields["FormVersion"]);
        spriggitFields["MutagenObjectType"].ShouldBe(dtoFields["MutagenObjectType"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }
}
