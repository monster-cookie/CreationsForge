using Avalonia.Headless.XUnit;
using CreationsForge.DataValidationTests.Validation.Specs.MiscItem;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.UI.Tests.MiscItem.Fallout4;

/// <summary>
/// Validates Fallout4 misc item Spriggit samples against the rendered comparison UI.
/// </summary>
public class Fallout4MiscItemSpriggitUiValidationTests :
    SpriggitComparisonUiTestBase,
    IClassFixture<SpriggitComparisonUiFixture>
{
    private readonly SpriggitComparisonUiFixture fixture;

    /// <summary>
    /// Initializes the UI validation tests with shared comparison fixture state.
    /// </summary>
    /// <param name="fixture">The fixture used to resolve imported records and comparison services.</param>
    public Fallout4MiscItemSpriggitUiValidationTests(SpriggitComparisonUiFixture fixture)
    {
        this.fixture = fixture;
    }

    /// <summary>
    /// Validates the Fallout4 <c>Debug_Components</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "MISC")]
    [Trait("FormKey", "247E7F:Fallout4.esm")]
    [Trait("EditorID", "Debug_Components")]
    [Trait("SpriggitFile", "MiscItems/Debug_Components - 247E7F_Fallout4.esm.yaml")]
    public void Fallout4_MISC_ComparisonUi_ShouldRenderSpriggitSample_Debug_Components()
    {
        var spec = MiscItemValidationSpecs.Fallout4_Debug_Components();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>FFDiamondCity07Paper</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "MISC")]
    [Trait("FormKey", "0A4754:Fallout4.esm")]
    [Trait("EditorID", "FFDiamondCity07Paper")]
    [Trait("SpriggitFile", "MiscItems/FFDiamondCity07Paper - 0A4754_Fallout4.esm.yaml")]
    public void Fallout4_MISC_ComparisonUi_ShouldRenderSpriggitSample_FFDiamondCity07Paper()
    {
        var spec = MiscItemValidationSpecs.Fallout4_FFDiamondCity07Paper();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>FireExtinguisher01</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "MISC")]
    [Trait("FormKey", "01F8F9:Fallout4.esm")]
    [Trait("EditorID", "FireExtinguisher01")]
    [Trait("SpriggitFile", "MiscItems/FireExtinguisher01 - 01F8F9_Fallout4.esm.yaml")]
    public void Fallout4_MISC_ComparisonUi_ShouldRenderSpriggitSample_FireExtinguisher01()
    {
        var spec = MiscItemValidationSpecs.Fallout4_FireExtinguisher01();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>BobbleHead_Agility</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "MISC")]
    [Trait("FormKey", "178B51:Fallout4.esm")]
    [Trait("EditorID", "BobbleHead_Agility")]
    [Trait("SpriggitFile", "MiscItems/BobbleHead_Agility - 178B51_Fallout4.esm.yaml")]
    public void Fallout4_MISC_ComparisonUi_ShouldRenderSpriggitSample_BobbleHead_Agility()
    {
        var spec = MiscItemValidationSpecs.Fallout4_BobbleHead_Agility();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>MS11GuidanceChip</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "MISC")]
    [Trait("FormKey", "04E3A2:Fallout4.esm")]
    [Trait("EditorID", "MS11GuidanceChip")]
    [Trait("SpriggitFile", "MiscItems/MS11GuidanceChip - 04E3A2_Fallout4.esm.yaml")]
    public void Fallout4_MISC_ComparisonUi_ShouldRenderSpriggitSample_MS11GuidanceChip()
    {
        var spec = MiscItemValidationSpecs.Fallout4_MS11GuidanceChip();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }
}
