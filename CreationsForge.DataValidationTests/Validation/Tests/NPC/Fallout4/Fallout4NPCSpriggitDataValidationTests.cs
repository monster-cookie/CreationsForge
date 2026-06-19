using CreationsForge.Core.Enums;

namespace CreationsForge.DataValidationTests.Validation.Tests.NPC.Fallout4;

public class Fallout4NPCSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "NPC_")]
    public void Fallout4_NPC_ShouldMatchSpriggitSamples()
    {
        ValidateScope(SupportedGame.Fallout4, "NPC_");
    }
}
