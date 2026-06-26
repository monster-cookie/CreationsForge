using CreationsForge.Core.DTOs.Records;
using CreationsForge.DataValidationTests.Validation.Specs;
using CreationsForge.DataValidationTests.Validation.Specs.GameSetting;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.GameSetting.Skyrim;

public class SkyrimGameSettingSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "0D4C40:Skyrim.esm")]
    [Trait("EditorID", "sAbortText")]
    [Trait("SpriggitFile", "GameSettings/sAbortText - 0D4C40_Skyrim.esm.yaml")]
    public void Skyrim_GMST_ShouldMatchSpriggitSample_sAbortText()
    {
        var spec = GameSettingValidationSpecs.Skyrim_sAbortText();
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
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "0D4DC4:Skyrim.esm")]
    [Trait("EditorID", "sAccept")]
    [Trait("SpriggitFile", "GameSettings/sAccept - 0D4DC4_Skyrim.esm.yaml")]
    public void Skyrim_GMST_ShouldMatchSpriggitSample_sAccept()
    {
        var spec = GameSettingValidationSpecs.Skyrim_sAccept();
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
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "0D4B96:Skyrim.esm")]
    [Trait("EditorID", "sActionMapping")]
    [Trait("SpriggitFile", "GameSettings/sActionMapping - 0D4B96_Skyrim.esm.yaml")]
    public void Skyrim_GMST_ShouldMatchSpriggitSample_sActionMapping()
    {
        var spec = GameSettingValidationSpecs.Skyrim_sActionMapping();
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
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "0B3D8A:Skyrim.esm")]
    [Trait("EditorID", "bRegenNPCMagickaDuringCast")]
    [Trait("SpriggitFile", "GameSettings/bRegenNPCMagickaDuringCast - 0B3D8A_Skyrim.esm.yaml")]
    public void Skyrim_GMST_ShouldMatchSpriggitSample_bRegenNPCMagickaDuringCast()
    {
        var spec = GameSettingValidationSpecs.Skyrim_bRegenNPCMagickaDuringCast();
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
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "01A144:Skyrim.esm")]
    [Trait("EditorID", "fActionPointsAimAdjustment")]
    [Trait("SpriggitFile", "GameSettings/fActionPointsAimAdjustment - 01A144_Skyrim.esm.yaml")]
    public void Skyrim_GMST_ShouldMatchSpriggitSample_fActionPointsAimAdjustment()
    {
        var spec = GameSettingValidationSpecs.Skyrim_fActionPointsAimAdjustment();
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
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "01A145:Skyrim.esm")]
    [Trait("EditorID", "fActionPointsAttackOneHandMelee")]
    [Trait("SpriggitFile", "GameSettings/fActionPointsAttackOneHandMelee - 01A145_Skyrim.esm.yaml")]
    public void Skyrim_GMST_ShouldMatchSpriggitSample_fActionPointsAttackOneHandMelee()
    {
        var spec = GameSettingValidationSpecs.Skyrim_fActionPointsAttackOneHandMelee();
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
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "01A237:Skyrim.esm")]
    [Trait("EditorID", "iAICombatRestoreHealthPercentage")]
    [Trait("SpriggitFile", "GameSettings/iAICombatRestoreHealthPercentage - 01A237_Skyrim.esm.yaml")]
    public void Skyrim_GMST_ShouldMatchSpriggitSample_iAICombatRestoreHealthPercentage()
    {
        var spec = GameSettingValidationSpecs.Skyrim_iAICombatRestoreHealthPercentage();
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
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "01A83A:Skyrim.esm")]
    [Trait("EditorID", "iAISocialDistanceToTriggerEvent")]
    [Trait("SpriggitFile", "GameSettings/iAISocialDistanceToTriggerEvent - 01A83A_Skyrim.esm.yaml")]
    public void Skyrim_GMST_ShouldMatchSpriggitSample_iAISocialDistanceToTriggerEvent()
    {
        var spec = GameSettingValidationSpecs.Skyrim_iAISocialDistanceToTriggerEvent();
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
