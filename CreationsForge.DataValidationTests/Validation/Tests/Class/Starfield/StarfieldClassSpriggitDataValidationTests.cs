using CreationsForge.Core.Enums;

namespace CreationsForge.DataValidationTests.Validation.Tests.Class.Starfield;

public class StarfieldClassSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "CLAS")]
    public void Starfield_CLAS_ShouldMatchSpriggitSamples()
    {
        ValidateScope(SupportedGame.Starfield, "CLAS");
    }
}
