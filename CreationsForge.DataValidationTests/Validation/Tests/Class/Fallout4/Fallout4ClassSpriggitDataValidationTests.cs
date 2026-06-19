using CreationsForge.Core.Enums;

namespace CreationsForge.DataValidationTests.Validation.Tests.Class.Fallout4;

public class Fallout4ClassSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "CLAS")]
    public void Fallout4_CLAS_ShouldMatchSpriggitSamples()
    {
        ValidateScope(SupportedGame.Fallout4, "CLAS");
    }
}
