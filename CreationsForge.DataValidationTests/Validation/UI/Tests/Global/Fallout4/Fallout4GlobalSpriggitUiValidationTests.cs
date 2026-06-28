using Avalonia.Headless.XUnit;
using CreationsForge.Specification.Validation.Specs.Global;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.UI.Tests.Global.Fallout4;

/// <summary>
/// Validates Fallout4 global Spriggit samples against the rendered comparison UI.
/// </summary>
public class Fallout4GlobalSpriggitUiValidationTests :
    SpriggitComparisonUiTestBase,
    IClassFixture<SpriggitComparisonUiFixture>
{
    private readonly SpriggitComparisonUiFixture fixture;

    /// <summary>
    /// Initializes the UI validation tests with shared comparison fixture state.
    /// </summary>
    /// <param name="fixture">The fixture used to resolve imported records and comparison services.</param>
    public Fallout4GlobalSpriggitUiValidationTests(SpriggitComparisonUiFixture fixture)
    {
        this.fixture = fixture;
    }

    /// <summary>
    /// Validates the Fallout4 <c>AO_Companion_Search_JunkThresholdValue</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "GLOB")]
    [Trait("FormKey", "18E889:Fallout4.esm")]
    [Trait("EditorID", "AO_Companion_Search_JunkThresholdValue")]
    [Trait("SpriggitFile", "Globals/AO_Companion_Search_JunkThresholdValue - 18E889_Fallout4.esm.yaml")]
    public void Fallout4_GLOB_ComparisonUi_ShouldRenderSpriggitSample_AO_Companion_Search_JunkThresholdValue()
    {
        var spec = GlobalValidationSpecs.Fallout4_AO_Companion_Search_JunkThresholdValue();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>AO_Companion_Search_NextAllowedDaysUntil</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "GLOB")]
    [Trait("FormKey", "176107:Fallout4.esm")]
    [Trait("EditorID", "AO_Companion_Search_NextAllowedDaysUntil")]
    [Trait("SpriggitFile", "Globals/AO_Companion_Search_NextAllowedDaysUntil - 176107_Fallout4.esm.yaml")]
    public void Fallout4_GLOB_ComparisonUi_ShouldRenderSpriggitSample_AO_Companion_Search_NextAllowedDaysUntil()
    {
        var spec = GlobalValidationSpecs.Fallout4_AO_Companion_Search_NextAllowedDaysUntil();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>AO_Dogmeat_Container_Bailout_Dist</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "GLOB")]
    [Trait("FormKey", "043F14:Fallout4.esm")]
    [Trait("EditorID", "AO_Dogmeat_Container_Bailout_Dist")]
    [Trait("SpriggitFile", "Globals/AO_Dogmeat_Container_Bailout_Dist - 043F14_Fallout4.esm.yaml")]
    public void Fallout4_GLOB_ComparisonUi_ShouldRenderSpriggitSample_AO_Dogmeat_Container_Bailout_Dist()
    {
        var spec = GlobalValidationSpecs.Fallout4_AO_Dogmeat_Container_Bailout_Dist();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }
}
