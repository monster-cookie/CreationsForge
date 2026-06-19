using CreationsForge.Core.Enums;

namespace CreationsForge.DataValidationTests.Validation.Tests.NPC.Starfield;

public class StarfieldNPCSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "NPC_")]
    public void Starfield_NPC_ShouldMatchSpriggitSamples()
    {
        ValidateScope(SupportedGame.Starfield, "NPC_");
    }
}
