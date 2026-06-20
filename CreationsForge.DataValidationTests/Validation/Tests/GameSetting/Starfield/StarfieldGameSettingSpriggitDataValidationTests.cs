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

        Helpers.GetSpriggitField(spriggit, "Data.Count").ShouldBe(Helpers.GetDTOField(dto, "Data.Count"));
        Helpers.GetSpriggitField(spriggit, "Data.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Data.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Data[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Data[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Data[0].String").ShouldBe(Helpers.GetDTOField(dto, "Data[0].String"));
        Helpers.GetSpriggitField(spriggit, "Data[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Data[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Data[1].String").ShouldBe(Helpers.GetDTOField(dto, "Data[1].String"));
        Helpers.GetSpriggitField(spriggit, "Data[2].Language").ShouldBe(Helpers.GetDTOField(dto, "Data[2].Language"));
        Helpers.GetSpriggitField(spriggit, "Data[2].String").ShouldBe(Helpers.GetDTOField(dto, "Data[2].String"));
        Helpers.GetSpriggitField(spriggit, "Data[3].Language").ShouldBe(Helpers.GetDTOField(dto, "Data[3].Language"));
        Helpers.GetSpriggitField(spriggit, "Data[3].String").ShouldBe(Helpers.GetDTOField(dto, "Data[3].String"));
        Helpers.GetSpriggitField(spriggit, "Data[4].Language").ShouldBe(Helpers.GetDTOField(dto, "Data[4].Language"));
        Helpers.GetSpriggitField(spriggit, "Data[4].String").ShouldBe(Helpers.GetDTOField(dto, "Data[4].String"));
        Helpers.GetSpriggitField(spriggit, "Data[5].Language").ShouldBe(Helpers.GetDTOField(dto, "Data[5].Language"));
        Helpers.GetSpriggitField(spriggit, "Data[5].String").ShouldBe(Helpers.GetDTOField(dto, "Data[5].String"));
        Helpers.GetSpriggitField(spriggit, "Data[6].Language").ShouldBe(Helpers.GetDTOField(dto, "Data[6].Language"));
        Helpers.GetSpriggitField(spriggit, "Data[6].String").ShouldBe(Helpers.GetDTOField(dto, "Data[6].String"));
        Helpers.GetSpriggitField(spriggit, "Data[7].Language").ShouldBe(Helpers.GetDTOField(dto, "Data[7].Language"));
        Helpers.GetSpriggitField(spriggit, "Data[7].String").ShouldBe(Helpers.GetDTOField(dto, "Data[7].String"));
        Helpers.GetSpriggitField(spriggit, "Data[8].Language").ShouldBe(Helpers.GetDTOField(dto, "Data[8].Language"));
        Helpers.GetSpriggitField(spriggit, "Data[8].String").ShouldBe(Helpers.GetDTOField(dto, "Data[8].String"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "Data.Count", "Data.TargetLanguage", "Data[0].Language", "Data[0].String", "Data[1].Language", "Data[1].String", "Data[2].Language", "Data[2].String", "Data[3].Language", "Data[3].String", "Data[4].Language", "Data[4].String", "Data[5].Language", "Data[5].String", "Data[6].Language", "Data[6].String", "Data[7].Language", "Data[7].String", "Data[8].Language", "Data[8].String", "EditorID", "FormKey", "FormVersion", "MutagenObjectType", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "Data.Count", "Data.TargetLanguage", "Data[0].Language", "Data[0].String", "Data[1].Language", "Data[1].String", "Data[2].Language", "Data[2].String", "Data[3].Language", "Data[3].String", "Data[4].Language", "Data[4].String", "Data[5].Language", "Data[5].String", "Data[6].Language", "Data[6].String", "Data[7].Language", "Data[7].String", "Data[8].Language", "Data[8].String", "EditorID", "FormKey", "FormVersion", "MutagenObjectType", "Version2", "VersionControl");
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

        Helpers.GetSpriggitField(spriggit, "Data.Count").ShouldBe(Helpers.GetDTOField(dto, "Data.Count"));
        Helpers.GetSpriggitField(spriggit, "Data.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Data.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Data[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Data[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Data[0].String").ShouldBe(Helpers.GetDTOField(dto, "Data[0].String"));
        Helpers.GetSpriggitField(spriggit, "Data[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Data[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Data[1].String").ShouldBe(Helpers.GetDTOField(dto, "Data[1].String"));
        Helpers.GetSpriggitField(spriggit, "Data[2].Language").ShouldBe(Helpers.GetDTOField(dto, "Data[2].Language"));
        Helpers.GetSpriggitField(spriggit, "Data[2].String").ShouldBe(Helpers.GetDTOField(dto, "Data[2].String"));
        Helpers.GetSpriggitField(spriggit, "Data[3].Language").ShouldBe(Helpers.GetDTOField(dto, "Data[3].Language"));
        Helpers.GetSpriggitField(spriggit, "Data[3].String").ShouldBe(Helpers.GetDTOField(dto, "Data[3].String"));
        Helpers.GetSpriggitField(spriggit, "Data[4].Language").ShouldBe(Helpers.GetDTOField(dto, "Data[4].Language"));
        Helpers.GetSpriggitField(spriggit, "Data[4].String").ShouldBe(Helpers.GetDTOField(dto, "Data[4].String"));
        Helpers.GetSpriggitField(spriggit, "Data[5].Language").ShouldBe(Helpers.GetDTOField(dto, "Data[5].Language"));
        Helpers.GetSpriggitField(spriggit, "Data[5].String").ShouldBe(Helpers.GetDTOField(dto, "Data[5].String"));
        Helpers.GetSpriggitField(spriggit, "Data[6].Language").ShouldBe(Helpers.GetDTOField(dto, "Data[6].Language"));
        Helpers.GetSpriggitField(spriggit, "Data[6].String").ShouldBe(Helpers.GetDTOField(dto, "Data[6].String"));
        Helpers.GetSpriggitField(spriggit, "Data[7].Language").ShouldBe(Helpers.GetDTOField(dto, "Data[7].Language"));
        Helpers.GetSpriggitField(spriggit, "Data[7].String").ShouldBe(Helpers.GetDTOField(dto, "Data[7].String"));
        Helpers.GetSpriggitField(spriggit, "Data[8].Language").ShouldBe(Helpers.GetDTOField(dto, "Data[8].Language"));
        Helpers.GetSpriggitField(spriggit, "Data[8].String").ShouldBe(Helpers.GetDTOField(dto, "Data[8].String"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "Data.Count", "Data.TargetLanguage", "Data[0].Language", "Data[0].String", "Data[1].Language", "Data[1].String", "Data[2].Language", "Data[2].String", "Data[3].Language", "Data[3].String", "Data[4].Language", "Data[4].String", "Data[5].Language", "Data[5].String", "Data[6].Language", "Data[6].String", "Data[7].Language", "Data[7].String", "Data[8].Language", "Data[8].String", "EditorID", "FormKey", "FormVersion", "MutagenObjectType", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "Data.Count", "Data.TargetLanguage", "Data[0].Language", "Data[0].String", "Data[1].Language", "Data[1].String", "Data[2].Language", "Data[2].String", "Data[3].Language", "Data[3].String", "Data[4].Language", "Data[4].String", "Data[5].Language", "Data[5].String", "Data[6].Language", "Data[6].String", "Data[7].Language", "Data[7].String", "Data[8].Language", "Data[8].String", "EditorID", "FormKey", "FormVersion", "MutagenObjectType", "Version2", "VersionControl");
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

        Helpers.GetSpriggitField(spriggit, "Data.Count").ShouldBe(Helpers.GetDTOField(dto, "Data.Count"));
        Helpers.GetSpriggitField(spriggit, "Data.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Data.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Data[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Data[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Data[0].String").ShouldBe(Helpers.GetDTOField(dto, "Data[0].String"));
        Helpers.GetSpriggitField(spriggit, "Data[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Data[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Data[1].String").ShouldBe(Helpers.GetDTOField(dto, "Data[1].String"));
        Helpers.GetSpriggitField(spriggit, "Data[2].Language").ShouldBe(Helpers.GetDTOField(dto, "Data[2].Language"));
        Helpers.GetSpriggitField(spriggit, "Data[2].String").ShouldBe(Helpers.GetDTOField(dto, "Data[2].String"));
        Helpers.GetSpriggitField(spriggit, "Data[3].Language").ShouldBe(Helpers.GetDTOField(dto, "Data[3].Language"));
        Helpers.GetSpriggitField(spriggit, "Data[3].String").ShouldBe(Helpers.GetDTOField(dto, "Data[3].String"));
        Helpers.GetSpriggitField(spriggit, "Data[4].Language").ShouldBe(Helpers.GetDTOField(dto, "Data[4].Language"));
        Helpers.GetSpriggitField(spriggit, "Data[4].String").ShouldBe(Helpers.GetDTOField(dto, "Data[4].String"));
        Helpers.GetSpriggitField(spriggit, "Data[5].Language").ShouldBe(Helpers.GetDTOField(dto, "Data[5].Language"));
        Helpers.GetSpriggitField(spriggit, "Data[5].String").ShouldBe(Helpers.GetDTOField(dto, "Data[5].String"));
        Helpers.GetSpriggitField(spriggit, "Data[6].Language").ShouldBe(Helpers.GetDTOField(dto, "Data[6].Language"));
        Helpers.GetSpriggitField(spriggit, "Data[6].String").ShouldBe(Helpers.GetDTOField(dto, "Data[6].String"));
        Helpers.GetSpriggitField(spriggit, "Data[7].Language").ShouldBe(Helpers.GetDTOField(dto, "Data[7].Language"));
        Helpers.GetSpriggitField(spriggit, "Data[7].String").ShouldBe(Helpers.GetDTOField(dto, "Data[7].String"));
        Helpers.GetSpriggitField(spriggit, "Data[8].Language").ShouldBe(Helpers.GetDTOField(dto, "Data[8].Language"));
        Helpers.GetSpriggitField(spriggit, "Data[8].String").ShouldBe(Helpers.GetDTOField(dto, "Data[8].String"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "FormVersion").ShouldBe(Helpers.GetDTOField(dto, "FormVersion"));
        Helpers.GetSpriggitField(spriggit, "MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "Data.Count", "Data.TargetLanguage", "Data[0].Language", "Data[0].String", "Data[1].Language", "Data[1].String", "Data[2].Language", "Data[2].String", "Data[3].Language", "Data[3].String", "Data[4].Language", "Data[4].String", "Data[5].Language", "Data[5].String", "Data[6].Language", "Data[6].String", "Data[7].Language", "Data[7].String", "Data[8].Language", "Data[8].String", "EditorID", "FormKey", "FormVersion", "MutagenObjectType", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "Data.Count", "Data.TargetLanguage", "Data[0].Language", "Data[0].String", "Data[1].Language", "Data[1].String", "Data[2].Language", "Data[2].String", "Data[3].Language", "Data[3].String", "Data[4].Language", "Data[4].String", "Data[5].Language", "Data[5].String", "Data[6].Language", "Data[6].String", "Data[7].Language", "Data[7].String", "Data[8].Language", "Data[8].String", "EditorID", "FormKey", "FormVersion", "MutagenObjectType", "Version2", "VersionControl");
    }
}