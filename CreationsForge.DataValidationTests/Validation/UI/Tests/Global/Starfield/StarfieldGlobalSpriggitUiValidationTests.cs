using Avalonia.Headless.XUnit;
using CreationsForge.DataValidationTests.Validation.Specs.Global;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.UI.Tests.Global.Starfield;

/// <summary>
/// Validates Starfield global Spriggit samples against the rendered comparison UI.
/// </summary>
public class StarfieldGlobalSpriggitUiValidationTests :
    SpriggitComparisonUiTestBase,
    IClassFixture<SpriggitComparisonUiFixture>
{
    private readonly SpriggitComparisonUiFixture fixture;

    /// <summary>
    /// Initializes the UI validation tests with shared comparison fixture state.
    /// </summary>
    /// <param name="fixture">The fixture used to resolve imported records and comparison services.</param>
    public StarfieldGlobalSpriggitUiValidationTests(SpriggitComparisonUiFixture fixture)
    {
        this.fixture = fixture;
    }

    /// <summary>
    /// Validates the Starfield <c>_UpdateShatteredSpaceMaster</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "GLOB")]
    [Trait("FormKey", "20C81D:Starfield.esm")]
    [Trait("EditorID", "_UpdateShatteredSpaceMaster")]
    [Trait("SpriggitFile", "Globals/_UpdateShatteredSpaceMaster - 20C81D_Starfield.esm.yaml")]
    public void Starfield_GLOB_ComparisonUi_ShouldRenderSpriggitSample_UpdateShatteredSpaceMaster()
    {
        var spec = GlobalValidationSpecs.Starfield_UpdateShatteredSpaceMaster();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>2B7FBD_Starfield.esm</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "GLOB")]
    [Trait("FormKey", "2B7FBD:Starfield.esm")]
    [Trait("EditorID", "2B7FBD_Starfield.esm")]
    [Trait("SpriggitFile", "Globals/2B7FBD_Starfield.esm.yaml")]
    public void Starfield_GLOB_ComparisonUi_ShouldRenderSpriggitSample_2B7FBD_Starfield_esm()
    {
        var spec = GlobalValidationSpecs.Starfield_2B7FBD_Starfield_esm();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>2B91E0_Starfield.esm</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "GLOB")]
    [Trait("FormKey", "2B91E0:Starfield.esm")]
    [Trait("EditorID", "2B91E0_Starfield.esm")]
    [Trait("SpriggitFile", "Globals/2B91E0_Starfield.esm.yaml")]
    public void Starfield_GLOB_ComparisonUi_ShouldRenderSpriggitSample_2B91E0_Starfield_esm()
    {
        var spec = GlobalValidationSpecs.Starfield_2B91E0_Starfield_esm();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }
}
