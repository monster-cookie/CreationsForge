using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.GameSetting.Fallout4;

public class Fallout4GameSettingSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "0D4C40:Fallout4.esm")]
    [Trait("EditorID", "sAbortText")]
    [Trait("SpriggitFile", "GameSettings/sAbortText - 0D4C40_Fallout4.esm.yaml")]
    public void Fallout4_GMST_ShouldMatchSpriggitSample_sAbortText()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.GameSetting,
            "sAbortText");
        var dto = Helpers.GetDTO<GameSettingDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.GameSetting,
            "0D4C40:Fallout4.esm");

        Helpers.GetSpriggitField(spriggit, "Data.Count").ShouldBe(Helpers.GetDTOField(dto, "Data.Count"));
        Helpers.GetSpriggitField(spriggit, "Data.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Data.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Data[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Data[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Data[0].String").ShouldBe(Helpers.GetDTOField(dto, "Data[0].String"));
        Helpers.GetSpriggitField(spriggit, "Data[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Data[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Data[1].String").ShouldBe(Helpers.GetDTOField(dto, "Data[1].String"));
        Helpers.GetSpriggitField(spriggit, "Data[10].Language").ShouldBe(Helpers.GetDTOField(dto, "Data[10].Language"));
        Helpers.GetSpriggitField(spriggit, "Data[10].String").ShouldBe(Helpers.GetDTOField(dto, "Data[10].String"));
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
        Helpers.GetSpriggitField(spriggit, "Data[9].Language").ShouldBe(Helpers.GetDTOField(dto, "Data[9].Language"));
        Helpers.GetSpriggitField(spriggit, "Data[9].String").ShouldBe(Helpers.GetDTOField(dto, "Data[9].String"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "Data.Count", "Data.TargetLanguage", "Data[0].Language", "Data[0].String", "Data[1].Language", "Data[1].String", "Data[10].Language", "Data[10].String", "Data[2].Language", "Data[2].String", "Data[3].Language", "Data[3].String", "Data[4].Language", "Data[4].String", "Data[5].Language", "Data[5].String", "Data[6].Language", "Data[6].String", "Data[7].Language", "Data[7].String", "Data[8].Language", "Data[8].String", "Data[9].Language", "Data[9].String", "EditorID", "FormKey", "MutagenObjectType", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "Data.Count", "Data.TargetLanguage", "Data[0].Language", "Data[0].String", "Data[1].Language", "Data[1].String", "Data[10].Language", "Data[10].String", "Data[2].Language", "Data[2].String", "Data[3].Language", "Data[3].String", "Data[4].Language", "Data[4].String", "Data[5].Language", "Data[5].String", "Data[6].Language", "Data[6].String", "Data[7].Language", "Data[7].String", "Data[8].Language", "Data[8].String", "Data[9].Language", "Data[9].String", "EditorID", "FormKey", "MutagenObjectType", "Version2", "VersionControl");
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "0D4DC4:Fallout4.esm")]
    [Trait("EditorID", "sAccept")]
    [Trait("SpriggitFile", "GameSettings/sAccept - 0D4DC4_Fallout4.esm.yaml")]
    public void Fallout4_GMST_ShouldMatchSpriggitSample_sAccept()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.GameSetting,
            "sAccept");
        var dto = Helpers.GetDTO<GameSettingDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.GameSetting,
            "0D4DC4:Fallout4.esm");

        Helpers.GetSpriggitField(spriggit, "Data.Count").ShouldBe(Helpers.GetDTOField(dto, "Data.Count"));
        Helpers.GetSpriggitField(spriggit, "Data.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Data.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Data[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Data[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Data[0].String").ShouldBe(Helpers.GetDTOField(dto, "Data[0].String"));
        Helpers.GetSpriggitField(spriggit, "Data[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Data[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Data[1].String").ShouldBe(Helpers.GetDTOField(dto, "Data[1].String"));
        Helpers.GetSpriggitField(spriggit, "Data[10].Language").ShouldBe(Helpers.GetDTOField(dto, "Data[10].Language"));
        Helpers.GetSpriggitField(spriggit, "Data[10].String").ShouldBe(Helpers.GetDTOField(dto, "Data[10].String"));
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
        Helpers.GetSpriggitField(spriggit, "Data[9].Language").ShouldBe(Helpers.GetDTOField(dto, "Data[9].Language"));
        Helpers.GetSpriggitField(spriggit, "Data[9].String").ShouldBe(Helpers.GetDTOField(dto, "Data[9].String"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "Data.Count", "Data.TargetLanguage", "Data[0].Language", "Data[0].String", "Data[1].Language", "Data[1].String", "Data[10].Language", "Data[10].String", "Data[2].Language", "Data[2].String", "Data[3].Language", "Data[3].String", "Data[4].Language", "Data[4].String", "Data[5].Language", "Data[5].String", "Data[6].Language", "Data[6].String", "Data[7].Language", "Data[7].String", "Data[8].Language", "Data[8].String", "Data[9].Language", "Data[9].String", "EditorID", "FormKey", "MutagenObjectType", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "Data.Count", "Data.TargetLanguage", "Data[0].Language", "Data[0].String", "Data[1].Language", "Data[1].String", "Data[10].Language", "Data[10].String", "Data[2].Language", "Data[2].String", "Data[3].Language", "Data[3].String", "Data[4].Language", "Data[4].String", "Data[5].Language", "Data[5].String", "Data[6].Language", "Data[6].String", "Data[7].Language", "Data[7].String", "Data[8].Language", "Data[8].String", "Data[9].Language", "Data[9].String", "EditorID", "FormKey", "MutagenObjectType", "Version2", "VersionControl");
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "0D4DFC:Fallout4.esm")]
    [Trait("EditorID", "sActivate")]
    [Trait("SpriggitFile", "GameSettings/sActivate - 0D4DFC_Fallout4.esm.yaml")]
    public void Fallout4_GMST_ShouldMatchSpriggitSample_sActivate()
    {
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.GameSetting,
            "sActivate");
        var dto = Helpers.GetDTO<GameSettingDTO>(
            SupportedGame.Fallout4,
            RecordTypeCatalog.GameSetting,
            "0D4DFC:Fallout4.esm");

        Helpers.GetSpriggitField(spriggit, "Data.Count").ShouldBe(Helpers.GetDTOField(dto, "Data.Count"));
        Helpers.GetSpriggitField(spriggit, "Data.TargetLanguage").ShouldBe(Helpers.GetDTOField(dto, "Data.TargetLanguage"));
        Helpers.GetSpriggitField(spriggit, "Data[0].Language").ShouldBe(Helpers.GetDTOField(dto, "Data[0].Language"));
        Helpers.GetSpriggitField(spriggit, "Data[0].String").ShouldBe(Helpers.GetDTOField(dto, "Data[0].String"));
        Helpers.GetSpriggitField(spriggit, "Data[1].Language").ShouldBe(Helpers.GetDTOField(dto, "Data[1].Language"));
        Helpers.GetSpriggitField(spriggit, "Data[1].String").ShouldBe(Helpers.GetDTOField(dto, "Data[1].String"));
        Helpers.GetSpriggitField(spriggit, "Data[10].Language").ShouldBe(Helpers.GetDTOField(dto, "Data[10].Language"));
        Helpers.GetSpriggitField(spriggit, "Data[10].String").ShouldBe(Helpers.GetDTOField(dto, "Data[10].String"));
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
        Helpers.GetSpriggitField(spriggit, "Data[9].Language").ShouldBe(Helpers.GetDTOField(dto, "Data[9].Language"));
        Helpers.GetSpriggitField(spriggit, "Data[9].String").ShouldBe(Helpers.GetDTOField(dto, "Data[9].String"));
        Helpers.GetSpriggitField(spriggit, "EditorID").ShouldBe(Helpers.GetDTOField(dto, "EditorID"));
        Helpers.GetSpriggitField(spriggit, "FormKey").ShouldBe(Helpers.GetDTOField(dto, "FormKey"));
        Helpers.GetSpriggitField(spriggit, "MutagenObjectType").ShouldBe(Helpers.GetDTOField(dto, "MutagenObjectType"));
        Helpers.GetSpriggitField(spriggit, "Version2").ShouldBe(Helpers.GetDTOField(dto, "Version2"));
        Helpers.GetSpriggitField(spriggit, "VersionControl").ShouldBe(Helpers.GetDTOField(dto, "VersionControl"));

        Helpers.AssertNoUnmatchedSpriggitFields(spriggit, "Data.Count", "Data.TargetLanguage", "Data[0].Language", "Data[0].String", "Data[1].Language", "Data[1].String", "Data[10].Language", "Data[10].String", "Data[2].Language", "Data[2].String", "Data[3].Language", "Data[3].String", "Data[4].Language", "Data[4].String", "Data[5].Language", "Data[5].String", "Data[6].Language", "Data[6].String", "Data[7].Language", "Data[7].String", "Data[8].Language", "Data[8].String", "Data[9].Language", "Data[9].String", "EditorID", "FormKey", "MutagenObjectType", "Version2", "VersionControl");
        Helpers.AssertNoUnmatchedDtoFields(spriggit, dto, "Data.Count", "Data.TargetLanguage", "Data[0].Language", "Data[0].String", "Data[1].Language", "Data[1].String", "Data[10].Language", "Data[10].String", "Data[2].Language", "Data[2].String", "Data[3].Language", "Data[3].String", "Data[4].Language", "Data[4].String", "Data[5].Language", "Data[5].String", "Data[6].Language", "Data[6].String", "Data[7].Language", "Data[7].String", "Data[8].Language", "Data[8].String", "Data[9].Language", "Data[9].String", "EditorID", "FormKey", "MutagenObjectType", "Version2", "VersionControl");
    }
}