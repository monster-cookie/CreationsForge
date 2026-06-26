using CreationsForge.Core.DTOs.Records;
using CreationsForge.DataValidationTests.Validation.Specs;
using CreationsForge.DataValidationTests.Validation.Specs.Terminal;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.Terminal.Starfield;

public class StarfieldTerminalSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "TERM")]
    [Trait("FormKey", "2D1D29:Starfield.esm")]
    [Trait("EditorID", "AkilaLife04_Computer")]
    [Trait("SpriggitFile", "Terminals/AkilaLife04_Computer - 2D1D29_Starfield.esm.yaml")]
    public void Starfield_TERM_ShouldMatchSpriggitSample_AkilaLife04_Computer()
    {
        var spec = TerminalValidationSpecs.Starfield_AkilaLife04_Computer();
        var dto = Helpers.GetDTO<TerminalDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "TERM")]
    [Trait("FormKey", "2D2617:Starfield.esm")]
    [Trait("EditorID", "AkilaLife08_FarmingComputer")]
    [Trait("SpriggitFile", "Terminals/AkilaLife08_FarmingComputer - 2D2617_Starfield.esm.yaml")]
    public void Starfield_TERM_ShouldMatchSpriggitSample_AkilaLife08_FarmingComputer()
    {
        var spec = TerminalValidationSpecs.Starfield_AkilaLife08_FarmingComputer();
        var dto = Helpers.GetDTO<TerminalDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "TERM")]
    [Trait("FormKey", "386CD0:Starfield.esm")]
    [Trait("EditorID", "BE_ShipComputer_BarStanding")]
    [Trait("SpriggitFile", "Terminals/BE_ShipComputer_BarStanding - 386CD0_Starfield.esm.yaml")]
    public void Starfield_TERM_ShouldMatchSpriggitSample_BE_ShipComputer_BarStanding()
    {
        var spec = TerminalValidationSpecs.Starfield_BE_ShipComputer_BarStanding();
        var dto = Helpers.GetDTO<TerminalDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "TERM")]
    [Trait("FormKey", "261A51:Starfield.esm")]
    [Trait("EditorID", "City_NA_Botany02Terminal")]
    [Trait("SpriggitFile", "Terminals/City_NA_Botany02Terminal - 261A51_Starfield.esm.yaml")]
    public void Starfield_TERM_ShouldMatchSpriggitSample_City_NA_Botany02Terminal()
    {
        var spec = TerminalValidationSpecs.Starfield_City_NA_Botany02Terminal();
        var dto = Helpers.GetDTO<TerminalDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "TERM")]
    [Trait("FormKey", "19F266:Starfield.esm")]
    [Trait("EditorID", "TerminalSittingActivatorA01_Desk")]
    [Trait("SpriggitFile", "Terminals/TerminalSittingActivatorA01_Desk - 19F266_Starfield.esm.yaml")]
    public void Starfield_TERM_ShouldMatchSpriggitSample_TerminalSittingActivatorA01_Desk()
    {
        var spec = TerminalValidationSpecs.Starfield_TerminalSittingActivatorA01_Desk();
        var dto = Helpers.GetDTO<TerminalDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }
}
