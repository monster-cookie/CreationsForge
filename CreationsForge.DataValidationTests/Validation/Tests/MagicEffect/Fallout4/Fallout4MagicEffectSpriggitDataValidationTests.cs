using CreationsForge.Core.DTOs.Records;
using CreationsForge.DataValidationTests.Validation.Specs;
using CreationsForge.Specification.Validation.Specs.MagicEffect;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.MagicEffect.Fallout4;

public class Fallout4MagicEffectSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "MGEF")]
    [Trait("FormKey", "247A6C:Fallout4.esm")]
    [Trait("EditorID", "CritCryoFreezeEffect")]
    [Trait("SpriggitFile", "MagicEffects/CritCryoFreezeEffect - 247A6C_Fallout4.esm.yaml")]
    public void Fallout4_MGEF_ShouldMatchSpriggitSample_CritCryoFreezeEffect()
    {
        var spec = MagicEffectValidationSpecs.Fallout4_CritCryoFreezeEffect();
        var dto = Helpers.GetDTO<MagicEffectDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "MGEF")]
    [Trait("FormKey", "18C354:Fallout4.esm")]
    [Trait("EditorID", "CryoFreezeEffect01")]
    [Trait("SpriggitFile", "MagicEffects/CryoFreezeEffect01 - 18C354_Fallout4.esm.yaml")]
    public void Fallout4_MGEF_ShouldMatchSpriggitSample_CryoFreezeEffect01()
    {
        var spec = MagicEffectValidationSpecs.Fallout4_CryoFreezeEffect01();
        var dto = Helpers.GetDTO<MagicEffectDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "MGEF")]
    [Trait("FormKey", "18C356:Fallout4.esm")]
    [Trait("EditorID", "CryoFreezeEffect02")]
    [Trait("SpriggitFile", "MagicEffects/CryoFreezeEffect02 - 18C356_Fallout4.esm.yaml")]
    public void Fallout4_MGEF_ShouldMatchSpriggitSample_CryoFreezeEffect02()
    {
        var spec = MagicEffectValidationSpecs.Fallout4_CryoFreezeEffect02();
        var dto = Helpers.GetDTO<MagicEffectDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "MGEF")]
    [Trait("FormKey", "171781:Fallout4.esm")]
    [Trait("EditorID", "PerkPainTrainKnockbackEffect")]
    [Trait("SpriggitFile", "MagicEffects/PerkPainTrainKnockbackEffect - 171781_Fallout4.esm.yaml")]
    public void Fallout4_MGEF_ShouldMatchSpriggitSample_PerkPainTrainKnockbackEffect()
    {
        var spec = MagicEffectValidationSpecs.Fallout4_PerkPainTrainKnockbackEffect();
        var dto = Helpers.GetDTO<MagicEffectDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "MGEF")]
    [Trait("FormKey", "0AE04F:Fallout4.esm")]
    [Trait("EditorID", "DN102_LabDemo3ParalyzeEffect")]
    [Trait("SpriggitFile", "MagicEffects/DN102_LabDemo3ParalyzeEffect - 0AE04F_Fallout4.esm.yaml")]
    public void Fallout4_MGEF_ShouldMatchSpriggitSample_DN102_LabDemo3ParalyzeEffect()
    {
        var spec = MagicEffectValidationSpecs.Fallout4_DN102_LabDemo3ParalyzeEffect();
        var dto = Helpers.GetDTO<MagicEffectDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }
}
