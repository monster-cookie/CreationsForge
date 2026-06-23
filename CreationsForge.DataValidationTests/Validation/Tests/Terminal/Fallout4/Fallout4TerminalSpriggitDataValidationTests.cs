using CreationsForge.Core.DTOs.Records;
using CreationsForge.DataValidationTests.Validation.Specs;
using CreationsForge.DataValidationTests.Validation.Specs.Terminal;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.Terminal.Fallout4;

public class Fallout4TerminalSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "TERM")]
    [Trait("FormKey", "0AEF52:Fallout4.esm")]
    [Trait("EditorID", "Vault111OverseerTPrimeDirective")]
    [Trait("SpriggitFile", "Terminals/Vault111OverseerTPrimeDirective - 0AEF52_Fallout4.esm.yaml")]
    public void Fallout4_TERM_ShouldMatchSpriggitSample_Vault111OverseerTPrimeDirective()
    {
        AssertTerminalMatches(TerminalValidationSpecs.Fallout4_Vault111OverseerTPrimeDirective());
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "TERM")]
    [Trait("FormKey", "0EC83C:Fallout4.esm")]
    [Trait("EditorID", "Vault75OverseerTerminal")]
    [Trait("SpriggitFile", "Terminals/Vault75OverseerTerminal - 0EC83C_Fallout4.esm.yaml")]
    public void Fallout4_TERM_ShouldMatchSpriggitSample_Vault75OverseerTerminal()
    {
        AssertTerminalMatches(TerminalValidationSpecs.Fallout4_Vault75OverseerTerminal());
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "TERM")]
    [Trait("FormKey", "1221C8:Fallout4.esm")]
    [Trait("EditorID", "DN035_RobotControlTerminal_Targeting")]
    [Trait("SpriggitFile", "Terminals/DN035_RobotControlTerminal_Targeting - 1221C8_Fallout4.esm.yaml")]
    public void Fallout4_TERM_ShouldMatchSpriggitSample_DN035_RobotControlTerminal_Targeting()
    {
        AssertTerminalMatches(TerminalValidationSpecs.Fallout4_DN035_RobotControlTerminal_Targeting());
    }

    private static void AssertTerminalMatches(ValidationSpec spec)
    {
        var dto = Helpers.GetDTO<TerminalDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }
}
