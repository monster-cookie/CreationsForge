using Avalonia.Headless.XUnit;
using CreationsForge.DataValidationTests.Validation.Specs.Terminal;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.UI.Tests.Terminal.Fallout4;

/// <summary>
/// Validates Fallout4 terminal Spriggit samples against the rendered comparison UI.
/// </summary>
public class Fallout4TerminalSpriggitUiValidationTests :
    SpriggitComparisonUiTestBase,
    IClassFixture<SpriggitComparisonUiFixture>
{
    private readonly SpriggitComparisonUiFixture fixture;

    /// <summary>
    /// Initializes the UI validation tests with shared comparison fixture state.
    /// </summary>
    /// <param name="fixture">The fixture used to resolve imported records and comparison services.</param>
    public Fallout4TerminalSpriggitUiValidationTests(SpriggitComparisonUiFixture fixture)
    {
        this.fixture = fixture;
    }

    /// <summary>
    /// Validates the Fallout4 <c>Vault111OverseerTPrimeDirective</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "TERM")]
    [Trait("FormKey", "0AEF52:Fallout4.esm")]
    [Trait("EditorID", "Vault111OverseerTPrimeDirective")]
    [Trait("SpriggitFile", "Terminals/Vault111OverseerTPrimeDirective - 0AEF52_Fallout4.esm.yaml")]
    public void Fallout4_TERM_ComparisonUi_ShouldRenderSpriggitSample_Vault111OverseerTPrimeDirective()
    {
        var spec = TerminalValidationSpecs.Fallout4_Vault111OverseerTPrimeDirective();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>Vault75OverseerTerminal</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "TERM")]
    [Trait("FormKey", "0EC83C:Fallout4.esm")]
    [Trait("EditorID", "Vault75OverseerTerminal")]
    [Trait("SpriggitFile", "Terminals/Vault75OverseerTerminal - 0EC83C_Fallout4.esm.yaml")]
    public void Fallout4_TERM_ComparisonUi_ShouldRenderSpriggitSample_Vault75OverseerTerminal()
    {
        var spec = TerminalValidationSpecs.Fallout4_Vault75OverseerTerminal();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>DN035_RobotControlTerminal_Targeting</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "TERM")]
    [Trait("FormKey", "1221C8:Fallout4.esm")]
    [Trait("EditorID", "DN035_RobotControlTerminal_Targeting")]
    [Trait("SpriggitFile", "Terminals/DN035_RobotControlTerminal_Targeting - 1221C8_Fallout4.esm.yaml")]
    public void Fallout4_TERM_ComparisonUi_ShouldRenderSpriggitSample_DN035_RobotControlTerminal_Targeting()
    {
        var spec = TerminalValidationSpecs.Fallout4_DN035_RobotControlTerminal_Targeting();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }
}
