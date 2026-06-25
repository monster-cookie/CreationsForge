using Avalonia.Headless.XUnit;
using CreationsForge.DataValidationTests.Validation.Specs.MagicEffect;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.UI.Tests.MagicEffect.Fallout4;

/// <summary>
/// Validates Fallout4 magic effect Spriggit samples against the rendered comparison UI.
/// </summary>
public class Fallout4MagicEffectSpriggitUiValidationTests :
    SpriggitComparisonUiTestBase,
    IClassFixture<SpriggitComparisonUiFixture>
{
    private readonly SpriggitComparisonUiFixture fixture;

    /// <summary>
    /// Initializes the UI validation tests with shared comparison fixture state.
    /// </summary>
    /// <param name="fixture">The fixture used to resolve imported records and comparison services.</param>
    public Fallout4MagicEffectSpriggitUiValidationTests(SpriggitComparisonUiFixture fixture)
    {
        this.fixture = fixture;
    }

    /// <summary>
    /// Validates the Fallout4 <c>CritCryoFreezeEffect</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "MGEF")]
    [Trait("FormKey", "247A6C:Fallout4.esm")]
    [Trait("EditorID", "CritCryoFreezeEffect")]
    [Trait("SpriggitFile", "MagicEffects/CritCryoFreezeEffect - 247A6C_Fallout4.esm.yaml")]
    public void Fallout4_MGEF_ComparisonUi_ShouldRenderSpriggitSample_CritCryoFreezeEffect()
    {
        var spec = MagicEffectValidationSpecs.Fallout4_CritCryoFreezeEffect();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>CryoFreezeEffect01</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "MGEF")]
    [Trait("FormKey", "18C354:Fallout4.esm")]
    [Trait("EditorID", "CryoFreezeEffect01")]
    [Trait("SpriggitFile", "MagicEffects/CryoFreezeEffect01 - 18C354_Fallout4.esm.yaml")]
    public void Fallout4_MGEF_ComparisonUi_ShouldRenderSpriggitSample_CryoFreezeEffect01()
    {
        var spec = MagicEffectValidationSpecs.Fallout4_CryoFreezeEffect01();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>CryoFreezeEffect02</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "MGEF")]
    [Trait("FormKey", "18C356:Fallout4.esm")]
    [Trait("EditorID", "CryoFreezeEffect02")]
    [Trait("SpriggitFile", "MagicEffects/CryoFreezeEffect02 - 18C356_Fallout4.esm.yaml")]
    public void Fallout4_MGEF_ComparisonUi_ShouldRenderSpriggitSample_CryoFreezeEffect02()
    {
        var spec = MagicEffectValidationSpecs.Fallout4_CryoFreezeEffect02();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>PerkPainTrainKnockbackEffect</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "MGEF")]
    [Trait("FormKey", "171781:Fallout4.esm")]
    [Trait("EditorID", "PerkPainTrainKnockbackEffect")]
    [Trait("SpriggitFile", "MagicEffects/PerkPainTrainKnockbackEffect - 171781_Fallout4.esm.yaml")]
    public void Fallout4_MGEF_ComparisonUi_ShouldRenderSpriggitSample_PerkPainTrainKnockbackEffect()
    {
        var spec = MagicEffectValidationSpecs.Fallout4_PerkPainTrainKnockbackEffect();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>DN102_LabDemo3ParalyzeEffect</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "MGEF")]
    [Trait("FormKey", "0AE04F:Fallout4.esm")]
    [Trait("EditorID", "DN102_LabDemo3ParalyzeEffect")]
    [Trait("SpriggitFile", "MagicEffects/DN102_LabDemo3ParalyzeEffect - 0AE04F_Fallout4.esm.yaml")]
    public void Fallout4_MGEF_ComparisonUi_ShouldRenderSpriggitSample_DN102_LabDemo3ParalyzeEffect()
    {
        var spec = MagicEffectValidationSpecs.Fallout4_DN102_LabDemo3ParalyzeEffect();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }
}
