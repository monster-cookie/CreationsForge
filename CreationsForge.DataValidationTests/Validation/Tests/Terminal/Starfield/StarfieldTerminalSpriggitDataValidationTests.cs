using CreationsForge.Core.Enums;

namespace CreationsForge.DataValidationTests.Validation.Tests.Terminal.Starfield;

public class StarfieldTerminalSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "TERM")]
    public void Starfield_TERM_ShouldMatchSpriggitSamples()
    {
        ValidateScope(SupportedGame.Starfield, "TERM");
    }
}
