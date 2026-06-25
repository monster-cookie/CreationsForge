using Avalonia.Headless.XUnit;
using CreationsForge.DataValidationTests.Validation.Specs.ActorValueInformation;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.UI.Tests.ActorValueInformation.Fallout4;

/// <summary>
/// Validates Fallout4 actor value information Spriggit samples against the rendered comparison UI.
/// </summary>
public class Fallout4ActorValueInformationSpriggitUiValidationTests :
    SpriggitComparisonUiTestBase,
    IClassFixture<SpriggitComparisonUiFixture>
{
    private readonly SpriggitComparisonUiFixture fixture;

    /// <summary>
    /// Initializes the UI validation tests with shared comparison fixture state.
    /// </summary>
    /// <param name="fixture">The fixture used to resolve imported records and comparison services.</param>
    public Fallout4ActorValueInformationSpriggitUiValidationTests(SpriggitComparisonUiFixture fixture)
    {
        this.fixture = fixture;
    }

    /// <summary>
    /// Validates the Fallout4 <c>SentryBotMaxHeatLevel</c> sample against rendered comparison rows.
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
    /// Validates the Fallout4 <c>HC_Adrenaline</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "AVIF")]
    [Trait("FormKey", "00080F:Fallout4.esm")]
    [Trait("EditorID", "HC_Adrenaline")]
    [Trait("SpriggitFile", "ActorValueInformation/HC_Adrenaline - 00080F_Fallout4.esm.yaml")]
    public void Fallout4_AVIF_ComparisonUi_ShouldRenderSpriggitSample_HC_Adrenaline()
    {
        var spec = ActorValueInformationValidationSpecs.Fallout4_HC_Adrenaline();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>Incendiary</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "AVIF")]
    [Trait("FormKey", "1B88D8:Fallout4.esm")]
    [Trait("EditorID", "Incendiary")]
    [Trait("SpriggitFile", "ActorValueInformation/Incendiary - 1B88D8_Fallout4.esm.yaml")]
    public void Fallout4_AVIF_ComparisonUi_ShouldRenderSpriggitSample_Incendiary()
    {
        var spec = ActorValueInformationValidationSpecs.Fallout4_Incendiary();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>Agility</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "AVIF")]
    [Trait("FormKey", "0002C7:Fallout4.esm")]
    [Trait("EditorID", "Agility")]
    [Trait("SpriggitFile", "ActorValueInformation/Agility - 0002C7_Fallout4.esm.yaml")]
    public void Fallout4_AVIF_ComparisonUi_ShouldRenderSpriggitSample_Agility()
    {
        var spec = ActorValueInformationValidationSpecs.Fallout4_Agility();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>AddictionCount</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "AVIF")]
    [Trait("FormKey", "1EB998:Fallout4.esm")]
    [Trait("EditorID", "AddictionCount")]
    [Trait("SpriggitFile", "ActorValueInformation/AddictionCount - 1EB998_Fallout4.esm.yaml")]
    public void Fallout4_AVIF_ComparisonUi_ShouldRenderSpriggitSample_AddictionCount()
    {
        var spec = ActorValueInformationValidationSpecs.Fallout4_AddictionCount();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }
}
