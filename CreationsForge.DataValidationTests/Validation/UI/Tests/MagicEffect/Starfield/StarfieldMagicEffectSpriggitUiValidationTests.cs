using Avalonia.Headless.XUnit;
using CreationsForge.Specification.Validation.Specs.MagicEffect;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.UI.Tests.MagicEffect.Starfield;

/// <summary>
/// Validates Starfield magic effect Spriggit samples against the rendered comparison UI.
/// </summary>
public class StarfieldMagicEffectSpriggitUiValidationTests :
    SpriggitComparisonUiTestBase,
    IClassFixture<SpriggitComparisonUiFixture>
{
    private readonly SpriggitComparisonUiFixture fixture;

    /// <summary>
    /// Initializes the UI validation tests with shared comparison fixture state.
    /// </summary>
    /// <param name="fixture">The fixture used to resolve imported records and comparison services.</param>
    public StarfieldMagicEffectSpriggitUiValidationTests(SpriggitComparisonUiFixture fixture)
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
    /// Validates the Starfield <c>ArtifactPowerParticleBeam_Effect</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "MGEF")]
    [Trait("FormKey", "2C7789:Starfield.esm")]
    [Trait("EditorID", "ArtifactPowerParticleBeam_Effect")]
    [Trait("SpriggitFile", "MagicEffects/ArtifactPowerParticleBeam_Effect - 2C7789_Starfield.esm.yaml")]
    public void Starfield_MGEF_ComparisonUi_ShouldRenderSpriggitSample_ArtifactPowerParticleBeam_Effect()
    {
        var spec = MagicEffectValidationSpecs.Starfield_ArtifactPowerParticleBeam_Effect();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>ArtifactPowerSunlessSpace_AIUse</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "MGEF")]
    [Trait("FormKey", "23AF01:Starfield.esm")]
    [Trait("EditorID", "ArtifactPowerSunlessSpace_AIUse")]
    [Trait("SpriggitFile", "MagicEffects/ArtifactPowerSunlessSpace_AIUse - 23AF01_Starfield.esm.yaml")]
    public void Starfield_MGEF_ComparisonUi_ShouldRenderSpriggitSample_ArtifactPowerSunlessSpace_AIUse()
    {
        var spec = MagicEffectValidationSpecs.Starfield_ArtifactPowerSunlessSpace_AIUse();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>ArtifactPowerSolarFlare_AIUse</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "MGEF")]
    [Trait("FormKey", "22AC10:Starfield.esm")]
    [Trait("EditorID", "ArtifactPowerSolarFlare_AIUse")]
    [Trait("SpriggitFile", "MagicEffects/ArtifactPowerSolarFlare_AIUse - 22AC10_Starfield.esm.yaml")]
    public void Starfield_MGEF_ComparisonUi_ShouldRenderSpriggitSample_ArtifactPowerSolarFlare_AIUse()
    {
        var spec = MagicEffectValidationSpecs.Starfield_ArtifactPowerSolarFlare_AIUse();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>ENV_DMG_Airborne_Hazard_Damage_Effect</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "MGEF")]
    [Trait("FormKey", "245B6F:Starfield.esm")]
    [Trait("EditorID", "ENV_DMG_Airborne_Hazard_Damage_Effect")]
    [Trait("SpriggitFile", "MagicEffects/ENV_DMG_Airborne_Hazard_Damage_Effect - 245B6F_Starfield.esm.yaml")]
    public void Starfield_MGEF_ComparisonUi_ShouldRenderSpriggitSample_ENV_DMG_Airborne_Hazard_Damage_Effect()
    {
        var spec = MagicEffectValidationSpecs.Starfield_ENV_DMG_Airborne_Hazard_Damage_Effect();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }
}
