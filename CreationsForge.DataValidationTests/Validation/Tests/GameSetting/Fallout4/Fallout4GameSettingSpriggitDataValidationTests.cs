using CreationsForge.Core.DTOs.Records;
using CreationsForge.DataValidationTests.Validation.Specs;
using CreationsForge.DataValidationTests.Validation.Specs.GameSetting;
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
        var spec = GameSettingValidationSpecs.Fallout4_sAbortText();
        var dto = Helpers.GetDTO<GameSettingDTO>(spec.Game, spec.RecordType, spec.FormKey);
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(spec.Game, spec.RecordType, spec.SampleName);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
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
        var spec = GameSettingValidationSpecs.Fallout4_sAccept();
        var dto = Helpers.GetDTO<GameSettingDTO>(spec.Game, spec.RecordType, spec.FormKey);
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(spec.Game, spec.RecordType, spec.SampleName);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
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
        var spec = GameSettingValidationSpecs.Fallout4_sActivate();
        var dto = Helpers.GetDTO<GameSettingDTO>(spec.Game, spec.RecordType, spec.FormKey);
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(spec.Game, spec.RecordType, spec.SampleName);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "0F9CFD:Fallout4.esm")]
    [Trait("EditorID", "bAllowBlinksDuringSpeech")]
    [Trait("SpriggitFile", "GameSettings/bAllowBlinksDuringSpeech - 0F9CFD_Fallout4.esm.yaml")]
    public void Fallout4_GMST_ShouldMatchSpriggitSample_bAllowBlinksDuringSpeech()
    {
        var spec = GameSettingValidationSpecs.Fallout4_bAllowBlinksDuringSpeech();
        var dto = Helpers.GetDTO<GameSettingDTO>(spec.Game, spec.RecordType, spec.FormKey);
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(spec.Game, spec.RecordType, spec.SampleName);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "01A145:Fallout4.esm")]
    [Trait("EditorID", "fActionPointsAttackOneHandMelee")]
    [Trait("SpriggitFile", "GameSettings/fActionPointsAttackOneHandMelee - 01A145_Fallout4.esm.yaml")]
    public void Fallout4_GMST_ShouldMatchSpriggitSample_fActionPointsAttackOneHandMelee()
    {
        var spec = GameSettingValidationSpecs.Fallout4_fActionPointsAttackOneHandMelee();
        var dto = Helpers.GetDTO<GameSettingDTO>(spec.Game, spec.RecordType, spec.FormKey);
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(spec.Game, spec.RecordType, spec.SampleName);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "08A207:Fallout4.esm")]
    [Trait("EditorID", "fActionPointsAttackRanged")]
    [Trait("SpriggitFile", "GameSettings/fActionPointsAttackRanged - 08A207_Fallout4.esm.yaml")]
    public void Fallout4_GMST_ShouldMatchSpriggitSample_fActionPointsAttackRanged()
    {
        var spec = GameSettingValidationSpecs.Fallout4_fActionPointsAttackRanged();
        var dto = Helpers.GetDTO<GameSettingDTO>(spec.Game, spec.RecordType, spec.FormKey);
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(spec.Game, spec.RecordType, spec.SampleName);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "01A237:Fallout4.esm")]
    [Trait("EditorID", "iAICombatRestoreHealthPercentage")]
    [Trait("SpriggitFile", "GameSettings/iAICombatRestoreHealthPercentage - 01A237_Fallout4.esm.yaml")]
    public void Fallout4_GMST_ShouldMatchSpriggitSample_iAICombatRestoreHealthPercentage()
    {
        var spec = GameSettingValidationSpecs.Fallout4_iAICombatRestoreHealthPercentage();
        var dto = Helpers.GetDTO<GameSettingDTO>(spec.Game, spec.RecordType, spec.FormKey);
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(spec.Game, spec.RecordType, spec.SampleName);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "01A83A:Fallout4.esm")]
    [Trait("EditorID", "iAISocialDistanceToTriggerEvent")]
    [Trait("SpriggitFile", "GameSettings/iAISocialDistanceToTriggerEvent - 01A83A_Fallout4.esm.yaml")]
    public void Fallout4_GMST_ShouldMatchSpriggitSample_iAISocialDistanceToTriggerEvent()
    {
        var spec = GameSettingValidationSpecs.Fallout4_iAISocialDistanceToTriggerEvent();
        var dto = Helpers.GetDTO<GameSettingDTO>(spec.Game, spec.RecordType, spec.FormKey);
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(spec.Game, spec.RecordType, spec.SampleName);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "246BD8:Fallout4.esm")]
    [Trait("EditorID", "uDefaultLevelZone01max")]
    [Trait("SpriggitFile", "GameSettings/uDefaultLevelZone01max - 246BD8_Fallout4.esm.yaml")]
    public void Fallout4_GMST_ShouldMatchSpriggitSample_uDefaultLevelZone01max()
    {
        var spec = GameSettingValidationSpecs.Fallout4_uDefaultLevelZone01max();
        var dto = Helpers.GetDTO<GameSettingDTO>(spec.Game, spec.RecordType, spec.FormKey);
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(spec.Game, spec.RecordType, spec.SampleName);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "246BD9:Fallout4.esm")]
    [Trait("EditorID", "uDefaultLevelZone02min")]
    [Trait("SpriggitFile", "GameSettings/uDefaultLevelZone02min - 246BD9_Fallout4.esm.yaml")]
    public void Fallout4_GMST_ShouldMatchSpriggitSample_uDefaultLevelZone02min()
    {
        var spec = GameSettingValidationSpecs.Fallout4_uDefaultLevelZone02min();
        var dto = Helpers.GetDTO<GameSettingDTO>(spec.Game, spec.RecordType, spec.FormKey);
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(spec.Game, spec.RecordType, spec.SampleName);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }
}
