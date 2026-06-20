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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["Data.Count"].ShouldBe(dtoFields["Data.Count"]);
        spriggitFields["Data.TargetLanguage"].ShouldBe(dtoFields["Data.TargetLanguage"]);
        spriggitFields["Data[0].Language"].ShouldBe(dtoFields["Data[0].Language"]);
        spriggitFields["Data[0].String"].ShouldBe(dtoFields["Data[0].String"]);
        spriggitFields["Data[1].Language"].ShouldBe(dtoFields["Data[1].Language"]);
        spriggitFields["Data[1].String"].ShouldBe(dtoFields["Data[1].String"]);
        spriggitFields["Data[10].Language"].ShouldBe(dtoFields["Data[10].Language"]);
        spriggitFields["Data[10].String"].ShouldBe(dtoFields["Data[10].String"]);
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
        spriggitFields["Data[9].Language"].ShouldBe(dtoFields["Data[9].Language"]);
        spriggitFields["Data[9].String"].ShouldBe(dtoFields["Data[9].String"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["MutagenObjectType"].ShouldBe(dtoFields["MutagenObjectType"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["Data.Count"].ShouldBe(dtoFields["Data.Count"]);
        spriggitFields["Data.TargetLanguage"].ShouldBe(dtoFields["Data.TargetLanguage"]);
        spriggitFields["Data[0].Language"].ShouldBe(dtoFields["Data[0].Language"]);
        spriggitFields["Data[0].String"].ShouldBe(dtoFields["Data[0].String"]);
        spriggitFields["Data[1].Language"].ShouldBe(dtoFields["Data[1].Language"]);
        spriggitFields["Data[1].String"].ShouldBe(dtoFields["Data[1].String"]);
        spriggitFields["Data[10].Language"].ShouldBe(dtoFields["Data[10].Language"]);
        spriggitFields["Data[10].String"].ShouldBe(dtoFields["Data[10].String"]);
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
        spriggitFields["Data[9].Language"].ShouldBe(dtoFields["Data[9].Language"]);
        spriggitFields["Data[9].String"].ShouldBe(dtoFields["Data[9].String"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["MutagenObjectType"].ShouldBe(dtoFields["MutagenObjectType"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
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

        var spriggitFields = spriggit.Fields;
        var dtoFields = Helpers.GetDTOFields(dto);

        spriggitFields["Data.Count"].ShouldBe(dtoFields["Data.Count"]);
        spriggitFields["Data.TargetLanguage"].ShouldBe(dtoFields["Data.TargetLanguage"]);
        spriggitFields["Data[0].Language"].ShouldBe(dtoFields["Data[0].Language"]);
        spriggitFields["Data[0].String"].ShouldBe(dtoFields["Data[0].String"]);
        spriggitFields["Data[1].Language"].ShouldBe(dtoFields["Data[1].Language"]);
        spriggitFields["Data[1].String"].ShouldBe(dtoFields["Data[1].String"]);
        spriggitFields["Data[10].Language"].ShouldBe(dtoFields["Data[10].Language"]);
        spriggitFields["Data[10].String"].ShouldBe(dtoFields["Data[10].String"]);
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
        spriggitFields["Data[9].Language"].ShouldBe(dtoFields["Data[9].Language"]);
        spriggitFields["Data[9].String"].ShouldBe(dtoFields["Data[9].String"]);
        spriggitFields["EditorID"].ShouldBe(dtoFields["EditorID"]);
        spriggitFields["FormKey"].ShouldBe(dtoFields["FormKey"]);
        spriggitFields["MutagenObjectType"].ShouldBe(dtoFields["MutagenObjectType"]);
        spriggitFields["Version2"].ShouldBe(dtoFields["Version2"]);
        spriggitFields["VersionControl"].ShouldBe(dtoFields["VersionControl"]);

        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }
}
