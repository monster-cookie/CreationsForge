using Avalonia.Headless.XUnit;
using CreationsForge.DataValidationTests.Validation.Specs.Terminal;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.UI.Tests.Terminal;

/// <summary>
/// Validates terminal Spriggit samples against the rendered comparison UI.
/// </summary>
public class TerminalSpriggitUiValidationTests :
    SpriggitComparisonUiTestBase,
    IClassFixture<SpriggitComparisonUiFixture>
{
    private readonly SpriggitComparisonUiFixture fixture;

    /// <summary>
    /// Initializes the UI validation tests with shared comparison fixture state.
    /// </summary>
    /// <param name="fixture">The fixture used to resolve imported records and comparison services.</param>
    public TerminalSpriggitUiValidationTests(SpriggitComparisonUiFixture fixture)
    {
        this.fixture = fixture;
    }

    /// <summary>
    /// Validates the Starfield <c>AkilaLife04_Computer</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "TERM")]
    [Trait("FormKey", "2D1D29:Starfield.esm")]
    [Trait("EditorID", "AkilaLife04_Computer")]
    [Trait("SpriggitFile", "Terminals/AkilaLife04_Computer - 2D1D29_Starfield.esm.yaml")]
    public void Starfield_TERM_ComparisonUi_ShouldRenderSpriggitSample_AkilaLife04_Computer()
    {
        var spec = TerminalValidationSpecs.Starfield_AkilaLife04_Computer();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>BE_ShipComputer_BarStanding</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "TERM")]
    [Trait("FormKey", "386CD0:Starfield.esm")]
    [Trait("EditorID", "BE_ShipComputer_BarStanding")]
    [Trait("SpriggitFile", "Terminals/BE_ShipComputer_BarStanding - 386CD0_Starfield.esm.yaml")]
    public void Starfield_TERM_ComparisonUi_ShouldRenderSpriggitSample_BE_ShipComputer_BarStanding()
    {
        var spec = TerminalValidationSpecs.Starfield_BE_ShipComputer_BarStanding();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout 4 <c>DN035_RobotControlTerminal_Targeting</c> sample against rendered comparison rows.
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
