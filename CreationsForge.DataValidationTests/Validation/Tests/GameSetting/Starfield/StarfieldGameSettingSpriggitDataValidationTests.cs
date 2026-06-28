using CreationsForge.Core.DTOs.Records;
using CreationsForge.DataValidationTests.Validation.Specs;
using CreationsForge.Specification.Validation.Specs.GameSetting;
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
        var spec = GameSettingValidationSpecs.Starfield_sAbort();
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
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "0D4DFC:Starfield.esm")]
    [Trait("EditorID", "sActivate")]
    [Trait("SpriggitFile", "GameSettings/sActivate - 0D4DFC_Starfield.esm.yaml")]
    public void Starfield_GMST_ShouldMatchSpriggitSample_sActivate()
    {
        var spec = GameSettingValidationSpecs.Starfield_sActivate();
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
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "0D4DEB:Starfield.esm")]
    [Trait("EditorID", "sActivateCreatureCalmed")]
    [Trait("SpriggitFile", "GameSettings/sActivateCreatureCalmed - 0D4DEB_Starfield.esm.yaml")]
    public void Starfield_GMST_ShouldMatchSpriggitSample_sActivateCreatureCalmed()
    {
        var spec = GameSettingValidationSpecs.Starfield_sActivateCreatureCalmed();
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
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "0F9CFD:Starfield.esm")]
    [Trait("EditorID", "bAllowBlinksDuringSpeech")]
    [Trait("SpriggitFile", "GameSettings/bAllowBlinksDuringSpeech - 0F9CFD_Starfield.esm.yaml")]
    public void Starfield_GMST_ShouldMatchSpriggitSample_bAllowBlinksDuringSpeech()
    {
        var spec = GameSettingValidationSpecs.Starfield_bAllowBlinksDuringSpeech();
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
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "024CA5:Starfield.esm")]
    [Trait("EditorID", "bBoostpackInitialThrustOnlyOnTakeoff")]
    [Trait("SpriggitFile", "GameSettings/bBoostpackInitialThrustOnlyOnTakeoff - 024CA5_Starfield.esm.yaml")]
    public void Starfield_GMST_ShouldMatchSpriggitSample_bBoostpackInitialThrustOnlyOnTakeoff()
    {
        var spec = GameSettingValidationSpecs.Starfield_bBoostpackInitialThrustOnlyOnTakeoff();
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
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "101046:Starfield.esm")]
    [Trait("EditorID", "fActorDefaultTurningSpeed")]
    [Trait("SpriggitFile", "GameSettings/fActorDefaultTurningSpeed - 101046_Starfield.esm.yaml")]
    public void Starfield_GMST_ShouldMatchSpriggitSample_fActorDefaultTurningSpeed()
    {
        var spec = GameSettingValidationSpecs.Starfield_fActorDefaultTurningSpeed();
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
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "097F48:Starfield.esm")]
    [Trait("EditorID", "fActorSwimBreathDamage")]
    [Trait("SpriggitFile", "GameSettings/fActorSwimBreathDamage - 097F48_Starfield.esm.yaml")]
    public void Starfield_GMST_ShouldMatchSpriggitSample_fActorSwimBreathDamage()
    {
        var spec = GameSettingValidationSpecs.Starfield_fActorSwimBreathDamage();
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
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "01A237:Starfield.esm")]
    [Trait("EditorID", "iAICombatRestoreHealthPercentage")]
    [Trait("SpriggitFile", "GameSettings/iAICombatRestoreHealthPercentage - 01A237_Starfield.esm.yaml")]
    public void Starfield_GMST_ShouldMatchSpriggitSample_iAICombatRestoreHealthPercentage()
    {
        var spec = GameSettingValidationSpecs.Starfield_iAICombatRestoreHealthPercentage();
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
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "003207:Starfield.esm")]
    [Trait("EditorID", "iAIMaxSocialDistanceToTriggerEvent")]
    [Trait("SpriggitFile", "GameSettings/iAIMaxSocialDistanceToTriggerEvent - 003207_Starfield.esm.yaml")]
    public void Starfield_GMST_ShouldMatchSpriggitSample_iAIMaxSocialDistanceToTriggerEvent()
    {
        var spec = GameSettingValidationSpecs.Starfield_iAIMaxSocialDistanceToTriggerEvent();
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
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "246BD8:Starfield.esm")]
    [Trait("EditorID", "uDefaultLevelZone01max")]
    [Trait("SpriggitFile", "GameSettings/uDefaultLevelZone01max - 246BD8_Starfield.esm.yaml")]
    public void Starfield_GMST_ShouldMatchSpriggitSample_uDefaultLevelZone01max()
    {
        var spec = GameSettingValidationSpecs.Starfield_uDefaultLevelZone01max();
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
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "246BD9:Starfield.esm")]
    [Trait("EditorID", "uDefaultLevelZone02min")]
    [Trait("SpriggitFile", "GameSettings/uDefaultLevelZone02min - 246BD9_Starfield.esm.yaml")]
    public void Starfield_GMST_ShouldMatchSpriggitSample_uDefaultLevelZone02min()
    {
        var spec = GameSettingValidationSpecs.Starfield_uDefaultLevelZone02min();
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
