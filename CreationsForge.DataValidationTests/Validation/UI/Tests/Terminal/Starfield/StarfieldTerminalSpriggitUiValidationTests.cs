using Avalonia.Headless.XUnit;
using CreationsForge.Specification.Validation.Specs.Terminal;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.UI.Tests.Terminal.Starfield;

/// <summary>
/// Validates Starfield terminal Spriggit samples against the rendered comparison UI.
/// </summary>
public class StarfieldTerminalSpriggitUiValidationTests :
    SpriggitComparisonUiTestBase,
    IClassFixture<SpriggitComparisonUiFixture>
{
    private readonly SpriggitComparisonUiFixture fixture;

    /// <summary>
    /// Initializes the UI validation tests with shared comparison fixture state.
    /// </summary>
    /// <param name="fixture">The fixture used to resolve imported records and comparison services.</param>
    public StarfieldTerminalSpriggitUiValidationTests(SpriggitComparisonUiFixture fixture)
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
    /// Validates the Starfield <c>AkilaLife08_FarmingComputer</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "TERM")]
    [Trait("FormKey", "2D2617:Starfield.esm")]
    [Trait("EditorID", "AkilaLife08_FarmingComputer")]
    [Trait("SpriggitFile", "Terminals/AkilaLife08_FarmingComputer - 2D2617_Starfield.esm.yaml")]
    public void Starfield_TERM_ComparisonUi_ShouldRenderSpriggitSample_AkilaLife08_FarmingComputer()
    {
        var spec = TerminalValidationSpecs.Starfield_AkilaLife08_FarmingComputer();
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
    /// Validates the Starfield <c>City_NA_Botany02Terminal</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "TERM")]
    [Trait("FormKey", "261A51:Starfield.esm")]
    [Trait("EditorID", "City_NA_Botany02Terminal")]
    [Trait("SpriggitFile", "Terminals/City_NA_Botany02Terminal - 261A51_Starfield.esm.yaml")]
    public void Starfield_TERM_ComparisonUi_ShouldRenderSpriggitSample_City_NA_Botany02Terminal()
    {
        var spec = TerminalValidationSpecs.Starfield_City_NA_Botany02Terminal();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>TerminalSittingActivatorA01_Desk</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "TERM")]
    [Trait("FormKey", "19F266:Starfield.esm")]
    [Trait("EditorID", "TerminalSittingActivatorA01_Desk")]
    [Trait("SpriggitFile", "Terminals/TerminalSittingActivatorA01_Desk - 19F266_Starfield.esm.yaml")]
    public void Starfield_TERM_ComparisonUi_ShouldRenderSpriggitSample_TerminalSittingActivatorA01_Desk()
    {
        var spec = TerminalValidationSpecs.Starfield_TerminalSittingActivatorA01_Desk();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }
}
