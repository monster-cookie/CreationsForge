using Avalonia.Headless.XUnit;
using CreationsForge.DataValidationTests.Validation.Specs.Static;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.UI.Tests.Static.Starfield;

/// <summary>
/// Validates Starfield static Spriggit samples against the rendered comparison UI.
/// </summary>
public class StarfieldStaticSpriggitUiValidationTests :
    SpriggitComparisonUiTestBase,
    IClassFixture<SpriggitComparisonUiFixture>
{
    private readonly SpriggitComparisonUiFixture fixture;

    /// <summary>
    /// Initializes the UI validation tests with shared comparison fixture state.
    /// </summary>
    /// <param name="fixture">The fixture used to resolve imported records and comparison services.</param>
    public StarfieldStaticSpriggitUiValidationTests(SpriggitComparisonUiFixture fixture)
    {
        this.fixture = fixture;
    }

    /// <summary>
    /// Validates the Starfield <c>OpiExtPodAirlock01</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "STAT")]
    [Trait("FormKey", "0514C6:Starfield.esm")]
    [Trait("EditorID", "OpiExtPodAirlock01")]
    [Trait("SpriggitFile", "Statics/OpiExtPodAirlock01 - 0514C6_Starfield.esm.yaml")]
    public void Starfield_STAT_ComparisonUi_ShouldRenderSpriggitSample_OpiExtPodAirlock01()
    {
        var spec = StaticValidationSpecs.Starfield_OpiExtPodAirlock01();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>OpmIntPodSmSide01</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "STAT")]
    [Trait("FormKey", "036311:Starfield.esm")]
    [Trait("EditorID", "OpmIntPodSmSide01")]
    [Trait("SpriggitFile", "Statics/OpmIntPodSmSide01 - 036311_Starfield.esm.yaml")]
    public void Starfield_STAT_ComparisonUi_ShouldRenderSpriggitSample_OpmIntPodSmSide01()
    {
        var spec = StaticValidationSpecs.Starfield_OpmIntPodSmSide01();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>CatIndWalkSm2WayB01</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "STAT")]
    [Trait("FormKey", "03A1B4:Starfield.esm")]
    [Trait("EditorID", "CatIndWalkSm2WayB01")]
    [Trait("SpriggitFile", "Statics/CatIndWalkSm2WayB01 - 03A1B4_Starfield.esm.yaml")]
    public void Starfield_STAT_ComparisonUi_ShouldRenderSpriggitSample_CatIndWalkSm2WayB01()
    {
        var spec = StaticValidationSpecs.Starfield_CatIndWalkSm2WayB01();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }
}
