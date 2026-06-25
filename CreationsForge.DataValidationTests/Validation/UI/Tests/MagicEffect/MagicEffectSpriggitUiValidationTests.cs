using Avalonia.Headless.XUnit;
using CreationsForge.DataValidationTests.Validation.Specs.MagicEffect;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.UI.Tests.MagicEffect;

/// <summary>
/// Validates magic effect Spriggit samples against the rendered comparison UI.
/// </summary>
public class MagicEffectSpriggitUiValidationTests :
    SpriggitComparisonUiTestBase,
    IClassFixture<SpriggitComparisonUiFixture>
{
    private readonly SpriggitComparisonUiFixture fixture;

    /// <summary>
    /// Initializes the UI validation tests with shared comparison fixture state.
    /// </summary>
    /// <param name="fixture">The fixture used to resolve imported records and comparison services.</param>
    public MagicEffectSpriggitUiValidationTests(SpriggitComparisonUiFixture fixture)
    {
        this.fixture = fixture;
    }

    /// <summary>
    /// Validates the Starfield <c>ArtifactPowerLifeForced_Effect</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "MGEF")]
    [Trait("FormKey", "2C5392:Starfield.esm")]
    [Trait("EditorID", "ArtifactPowerLifeForced_Effect")]
    [Trait("SpriggitFile", "MagicEffects/ArtifactPowerLifeForced_Effect - 2C5392_Starfield.esm.yaml")]
    public void Starfield_MGEF_ComparisonUi_ShouldRenderSpriggitSample_ArtifactPowerLifeForced_Effect()
    {
        var spec = MagicEffectValidationSpecs.Starfield_ArtifactPowerLifeForced_Effect();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout 4 <c>PerkPainTrainKnockbackEffect</c> sample against rendered comparison rows.
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
    /// Validates the Skyrim <c>ShockDamageMassConcAimed</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "MGEF")]
    [Trait("FormKey", "0D22FA:Skyrim.esm")]
    [Trait("EditorID", "ShockDamageMassConcAimed")]
    [Trait("SpriggitFile", "MagicEffects/ShockDamageMassConcAimed - 0D22FA_Skyrim.esm.yaml")]
    public void Skyrim_MGEF_ComparisonUi_ShouldRenderSpriggitSample_ShockDamageMassConcAimed()
    {
        var spec = MagicEffectValidationSpecs.Skyrim_ShockDamageMassConcAimed();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }
}
