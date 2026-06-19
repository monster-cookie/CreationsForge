using CreationsForge.Core.Enums;

namespace CreationsForge.DataValidationTests.Validation.Tests.FormList.Starfield;

public class StarfieldFormListSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "FLST")]
    public void Starfield_FLST_ShouldMatchSpriggitSamples()
    {
        ValidateScope(SupportedGame.Starfield, "FLST");
    }
}
