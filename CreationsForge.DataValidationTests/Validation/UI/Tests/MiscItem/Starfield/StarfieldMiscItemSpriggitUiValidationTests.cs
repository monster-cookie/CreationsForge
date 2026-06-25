using Avalonia.Headless.XUnit;
using CreationsForge.DataValidationTests.Validation.Specs.MiscItem;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.UI.Tests.MiscItem.Starfield;

/// <summary>
/// Validates Starfield miscellaneous item Spriggit samples against the rendered comparison UI.
/// </summary>
public class StarfieldMiscItemSpriggitUiValidationTests :
    SpriggitComparisonUiTestBase,
    IClassFixture<SpriggitComparisonUiFixture>
{
    private readonly SpriggitComparisonUiFixture fixture;

    /// <summary>
    /// Initializes the UI validation tests with shared comparison fixture state.
    /// </summary>
    /// <param name="fixture">The fixture used to resolve imported records and comparison services.</param>
    public StarfieldMiscItemSpriggitUiValidationTests(SpriggitComparisonUiFixture fixture)
    {
        this.fixture = fixture;
    }

    /// <summary>
    /// Validates the Starfield <c>InorgCommonWater</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "MISC")]
    [Trait("FormKey", "005591:Starfield.esm")]
    [Trait("EditorID", "InorgCommonWater")]
    [Trait("SpriggitFile", "MiscItems/InorgCommonWater - 005591_Starfield.esm.yaml")]
    public void Starfield_MISC_ComparisonUi_ShouldRenderSpriggitSample_InorgCommonWater()
    {
        var spec = MiscItemValidationSpecs.Starfield_InorgCommonWater();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>InorgExoticPlutonium</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "MISC")]
    [Trait("FormKey", "00558C:Starfield.esm")]
    [Trait("EditorID", "InorgExoticPlutonium")]
    [Trait("SpriggitFile", "MiscItems/InorgExoticPlutonium - 00558C_Starfield.esm.yaml")]
    public void Starfield_MISC_ComparisonUi_ShouldRenderSpriggitSample_InorgExoticPlutonium()
    {
        var spec = MiscItemValidationSpecs.Starfield_InorgExoticPlutonium();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>ExoticPlayingCard_Diamond_Q</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "MISC")]
    [Trait("FormKey", "10A797:Starfield.esm")]
    [Trait("EditorID", "ExoticPlayingCard_Diamond_Q")]
    [Trait("SpriggitFile", "MiscItems/ExoticPlayingCard_Diamond_Q - 10A797_Starfield.esm.yaml")]
    public void Starfield_MISC_ComparisonUi_ShouldRenderSpriggitSample_ExoticPlayingCard_Diamond_Q()
    {
        var spec = MiscItemValidationSpecs.Starfield_ExoticPlayingCard_Diamond_Q();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }
}
