using Avalonia.Headless.XUnit;
using CreationsForge.DataValidationTests.Validation.Specs.ActorValueInformation;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.UI.Tests.ActorValueInformation.Starfield;

/// <summary>
/// Validates Starfield actor value information Spriggit samples against the rendered comparison UI.
/// </summary>
public class StarfieldActorValueInformationSpriggitUiValidationTests :
    SpriggitComparisonUiTestBase,
    IClassFixture<SpriggitComparisonUiFixture>
{
    private readonly SpriggitComparisonUiFixture fixture;

    /// <summary>
    /// Initializes the UI validation tests with shared comparison fixture state.
    /// </summary>
    /// <param name="fixture">The fixture used to resolve imported records and comparison services.</param>
    public StarfieldActorValueInformationSpriggitUiValidationTests(SpriggitComparisonUiFixture fixture)
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
    /// Validates the Starfield <c>ENV_Resist_Airborne</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "AVIF")]
    [Trait("FormKey", "248D31:Starfield.esm")]
    [Trait("EditorID", "ENV_Resist_Airborne")]
    [Trait("SpriggitFile", "ActorValueInformation/ENV_Resist_Airborne - 248D31_Starfield.esm.yaml")]
    public void Starfield_AVIF_ComparisonUi_ShouldRenderSpriggitSample_ENV_Resist_Airborne()
    {
        var spec = ActorValueInformationValidationSpecs.Starfield_ENV_Resist_Airborne();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>ENV_Resist_Corrosive</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "AVIF")]
    [Trait("FormKey", "248D30:Starfield.esm")]
    [Trait("EditorID", "ENV_Resist_Corrosive")]
    [Trait("SpriggitFile", "ActorValueInformation/ENV_Resist_Corrosive - 248D30_Starfield.esm.yaml")]
    public void Starfield_AVIF_ComparisonUi_ShouldRenderSpriggitSample_ENV_Resist_Corrosive()
    {
        var spec = ActorValueInformationValidationSpecs.Starfield_ENV_Resist_Corrosive();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>PEO_CarryWeight</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "AVIF")]
    [Trait("FormKey", "2EE0BB:Starfield.esm")]
    [Trait("EditorID", "PEO_CarryWeight")]
    [Trait("SpriggitFile", "ActorValueInformation/PEO_CarryWeight - 2EE0BB_Starfield.esm.yaml")]
    public void Starfield_AVIF_ComparisonUi_ShouldRenderSpriggitSample_PEO_CarryWeight()
    {
        var spec = ActorValueInformationValidationSpecs.Starfield_PEO_CarryWeight();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>Health</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "AVIF")]
    [Trait("FormKey", "0002D4:Starfield.esm")]
    [Trait("EditorID", "Health")]
    [Trait("SpriggitFile", "ActorValueInformation/Health - 0002D4_Starfield.esm.yaml")]
    public void Starfield_AVIF_ComparisonUi_ShouldRenderSpriggitSample_Health()
    {
        var spec = ActorValueInformationValidationSpecs.Starfield_Health();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }
}
