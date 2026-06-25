using Avalonia.Headless.XUnit;
using CreationsForge.DataValidationTests.Validation.Specs.Keyword;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.UI.Tests.Keyword.Starfield;

/// <summary>
/// Validates Starfield keyword Spriggit samples against the rendered comparison UI.
/// </summary>
public class StarfieldKeywordSpriggitUiValidationTests :
    SpriggitComparisonUiTestBase,
    IClassFixture<SpriggitComparisonUiFixture>
{
    private readonly SpriggitComparisonUiFixture fixture;

    /// <summary>
    /// Initializes the UI validation tests with shared comparison fixture state.
    /// </summary>
    /// <param name="fixture">The fixture used to resolve imported records and comparison services.</param>
    public StarfieldKeywordSpriggitUiValidationTests(SpriggitComparisonUiFixture fixture)
    {
        this.fixture = fixture;
    }

    /// <summary>
    /// Validates the Starfield <c>WeaponTypeDisplay_ElectromagneticRifle</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "1C84DD:Starfield.esm")]
    [Trait("EditorID", "WeaponTypeDisplay_ElectromagneticRifle")]
    [Trait("SpriggitFile", "Keywords/WeaponTypeDisplay_ElectromagneticRifle - 1C84DD_Starfield.esm.yaml")]
    public void Starfield_KYWD_ComparisonUi_ShouldRenderSpriggitSample_WeaponTypeDisplay_ElectromagneticRifle()
    {
        var spec = KeywordValidationSpecs.Starfield_WeaponTypeDisplay_ElectromagneticRifle();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>CCT_Enviro_AmbusherSurface</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "200AEB:Starfield.esm")]
    [Trait("EditorID", "CCT_Enviro_AmbusherSurface")]
    [Trait("SpriggitFile", "Keywords/CCT_Enviro_AmbusherSurface - 200AEB_Starfield.esm.yaml")]
    public void Starfield_KYWD_ComparisonUi_ShouldRenderSpriggitSample_CCT_Enviro_AmbusherSurface()
    {
        var spec = KeywordValidationSpecs.Starfield_CCT_Enviro_AmbusherSurface();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>ActorTypeChild</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "1157E8:Starfield.esm")]
    [Trait("EditorID", "ActorTypeChild")]
    [Trait("SpriggitFile", "Keywords/ActorTypeChild - 1157E8_Starfield.esm.yaml")]
    public void Starfield_KYWD_ComparisonUi_ShouldRenderSpriggitSample_ActorTypeChild()
    {
        var spec = KeywordValidationSpecs.Starfield_ActorTypeChild();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }
}
