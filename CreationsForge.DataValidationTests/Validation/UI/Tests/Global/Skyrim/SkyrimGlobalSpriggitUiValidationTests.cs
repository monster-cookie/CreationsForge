using Avalonia.Headless.XUnit;
using CreationsForge.DataValidationTests.Validation.Specs.Global;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.UI.Tests.Global.Skyrim;

/// <summary>
/// Validates Skyrim global Spriggit samples against the rendered comparison UI.
/// </summary>
public class SkyrimGlobalSpriggitUiValidationTests :
    SpriggitComparisonUiTestBase,
    IClassFixture<SpriggitComparisonUiFixture>
{
    private readonly SpriggitComparisonUiFixture fixture;

    /// <summary>
    /// Initializes the UI validation tests with shared comparison fixture state.
    /// </summary>
    /// <param name="fixture">The fixture used to resolve imported records and comparison services.</param>
    public SkyrimGlobalSpriggitUiValidationTests(SpriggitComparisonUiFixture fixture)
    {
        this.fixture = fixture;
    }

    /// <summary>
    /// Validates the Skyrim <c>CarriageCost</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "GLOB")]
    [Trait("FormKey", "050765:Skyrim.esm")]
    [Trait("EditorID", "CarriageCost")]
    [Trait("SpriggitFile", "Globals/CarriageCost - 050765_Skyrim.esm.yaml")]
    public void Skyrim_GLOB_ComparisonUi_ShouldRenderSpriggitSample_CarriageCost()
    {
        var spec = GlobalValidationSpecs.Skyrim_CarriageCost();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Skyrim <c>1stPKillCam</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "GLOB")]
    [Trait("FormKey", "10636A:Skyrim.esm")]
    [Trait("EditorID", "1stPKillCam")]
    [Trait("SpriggitFile", "Globals/1stPKillCam - 10636A_Skyrim.esm.yaml")]
    public void Skyrim_GLOB_ComparisonUi_ShouldRenderSpriggitSample_1stPKillCam()
    {
        var spec = GlobalValidationSpecs.Skyrim_1stPKillCam();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Skyrim <c>CarriageCostSmall</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "GLOB")]
    [Trait("FormKey", "107702:Skyrim.esm")]
    [Trait("EditorID", "CarriageCostSmall")]
    [Trait("SpriggitFile", "Globals/CarriageCostSmall - 107702_Skyrim.esm.yaml")]
    public void Skyrim_GLOB_ComparisonUi_ShouldRenderSpriggitSample_CarriageCostSmall()
    {
        var spec = GlobalValidationSpecs.Skyrim_CarriageCostSmall();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }
}
