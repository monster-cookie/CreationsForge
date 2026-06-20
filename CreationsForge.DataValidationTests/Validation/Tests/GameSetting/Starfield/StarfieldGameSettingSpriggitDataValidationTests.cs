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
}
