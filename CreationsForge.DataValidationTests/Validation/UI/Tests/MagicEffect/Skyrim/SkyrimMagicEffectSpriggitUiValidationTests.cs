using Avalonia.Headless.XUnit;
using CreationsForge.DataValidationTests.Validation.Specs.MagicEffect;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.UI.Tests.MagicEffect.Skyrim;

/// <summary>
/// Validates Skyrim magic effect Spriggit samples against the rendered comparison UI.
/// </summary>
public class SkyrimMagicEffectSpriggitUiValidationTests :
    SpriggitComparisonUiTestBase,
    IClassFixture<SpriggitComparisonUiFixture>
{
    private readonly SpriggitComparisonUiFixture fixture;

    /// <summary>
    /// Initializes the UI validation tests with shared comparison fixture state.
    /// </summary>
    /// <param name="fixture">The fixture used to resolve imported records and comparison services.</param>
    public SkyrimMagicEffectSpriggitUiValidationTests(SpriggitComparisonUiFixture fixture)
    {
        this.fixture = fixture;
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

    /// <summary>
    /// Validates the Skyrim <c>dunVolunruudPickaxeEffect</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "MGEF")]
    [Trait("FormKey", "1019D6:Skyrim.esm")]
    [Trait("EditorID", "dunVolunruudPickaxeEffect")]
    [Trait("SpriggitFile", "MagicEffects/dunVolunruudPickaxeEffect - 1019D6_Skyrim.esm.yaml")]
    public void Skyrim_MGEF_ComparisonUi_ShouldRenderSpriggitSample_dunVolunruudPickaxeEffect()
    {
        var spec = MagicEffectValidationSpecs.Skyrim_dunVolunruudPickaxeEffect();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Skyrim <c>ArmorFFSelf100</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "MGEF")]
    [Trait("FormKey", "0CDB75:Skyrim.esm")]
    [Trait("EditorID", "ArmorFFSelf100")]
    [Trait("SpriggitFile", "MagicEffects/ArmorFFSelf100 - 0CDB75_Skyrim.esm.yaml")]
    public void Skyrim_MGEF_ComparisonUi_ShouldRenderSpriggitSample_ArmorFFSelf100()
    {
        var spec = MagicEffectValidationSpecs.Skyrim_ArmorFFSelf100();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Skyrim <c>DA15WabbajackFF</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "MGEF")]
    [Trait("FormKey", "09B246:Skyrim.esm")]
    [Trait("EditorID", "DA15WabbajackFF")]
    [Trait("SpriggitFile", "MagicEffects/DA15WabbajackFF - 09B246_Skyrim.esm.yaml")]
    public void Skyrim_MGEF_ComparisonUi_ShouldRenderSpriggitSample_DA15WabbajackFF()
    {
        var spec = MagicEffectValidationSpecs.Skyrim_DA15WabbajackFF();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Skyrim <c>dunHalldirAggDownFFAimedArea</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "MGEF")]
    [Trait("FormKey", "0FB406:Skyrim.esm")]
    [Trait("EditorID", "dunHalldirAggDownFFAimedArea")]
    [Trait("SpriggitFile", "MagicEffects/dunHalldirAggDownFFAimedArea - 0FB406_Skyrim.esm.yaml")]
    public void Skyrim_MGEF_ComparisonUi_ShouldRenderSpriggitSample_dunHalldirAggDownFFAimedArea()
    {
        var spec = MagicEffectValidationSpecs.Skyrim_dunHalldirAggDownFFAimedArea();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }
}
