using Avalonia.Headless.XUnit;
using CreationsForge.DataValidationTests.Validation.Specs.ActorValueInformation;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.UI.Tests.ActorValueInformation;

/// <summary>
/// Validates actor value information Spriggit samples against the rendered comparison UI.
/// </summary>
public class ActorValueInformationSpriggitUiValidationTests :
    SpriggitComparisonUiTestBase,
    IClassFixture<SpriggitComparisonUiFixture>
{
    private readonly SpriggitComparisonUiFixture fixture;

    /// <summary>
    /// Initializes the UI validation tests with shared comparison fixture state.
    /// </summary>
    /// <param name="fixture">The fixture used to resolve imported records and comparison services.</param>
    public ActorValueInformationSpriggitUiValidationTests(SpriggitComparisonUiFixture fixture)
    {
        this.fixture = fixture;
    }

    /// <summary>
    /// Validates the Starfield <c>TargetingModeActionPoints_AV</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "AVIF")]
    [Trait("FormKey", "05ACD4:Starfield.esm")]
    [Trait("EditorID", "TargetingModeActionPoints_AV")]
    [Trait("SpriggitFile", "ActorValueInformation/TargetingModeActionPoints_AV - 05ACD4_Starfield.esm.yaml")]
    public void Starfield_AVIF_ComparisonUi_ShouldRenderSpriggitSample_TargetingModeActionPoints_AV()
    {
        var spec = ActorValueInformationValidationSpecs.Starfield_TargetingModeActionPoints_AV();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout 4 <c>SentryBotMaxHeatLevel</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "AVIF")]
    [Trait("FormKey", "0B287B:Fallout4.esm")]
    [Trait("EditorID", "SentryBotMaxHeatLevel")]
    [Trait("SpriggitFile", "ActorValueInformation/SentryBotMaxHeatLevel - 0B287B_Fallout4.esm.yaml")]
    public void Fallout4_AVIF_ComparisonUi_ShouldRenderSpriggitSample_SentryBotMaxHeatLevel()
    {
        var spec = ActorValueInformationValidationSpecs.Fallout4_SentryBotMaxHeatLevel();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Skyrim <c>AVAlchemy</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "AVIF")]
    [Trait("FormKey", "000456:Skyrim.esm")]
    [Trait("EditorID", "AVAlchemy")]
    [Trait("SpriggitFile", "ActorValueInformation/AVAlchemy - 000456_Skyrim.esm.yaml")]
    public void Skyrim_AVIF_ComparisonUi_ShouldRenderSpriggitSample_AVAlchemy()
    {
        var spec = ActorValueInformationValidationSpecs.Skyrim_AVAlchemy();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }
}
