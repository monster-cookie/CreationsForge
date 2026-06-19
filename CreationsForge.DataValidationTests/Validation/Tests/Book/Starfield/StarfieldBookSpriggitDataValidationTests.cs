using CreationsForge.Core.Enums;

namespace CreationsForge.DataValidationTests.Validation.Tests.Book.Starfield;

public class StarfieldBookSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "BOOK")]
    public void Starfield_BOOK_ShouldMatchSpriggitSamples()
    {
        ValidateScope(SupportedGame.Starfield, "BOOK");
    }
}
