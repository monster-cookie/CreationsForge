using CreationsForge.Core.DTOs.Records;
using CreationsForge.DataValidationTests.Validation.Specs;
using CreationsForge.DataValidationTests.Validation.Specs.NPC;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.NPC.Starfield;

public class StarfieldNPCSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "NPC_")]
    [Trait("FormKey", "01539F:Starfield.esm")]
    [Trait("EditorID", "CF_AludraTahan")]
    [Trait("SpriggitFile", "Npcs/CF_AludraTahan - 01539F_Starfield.esm.yaml")]
    public void Starfield_NPC__ShouldMatchSpriggitSample_CF_AludraTahan()
    {
        AssertNPC(NPCValidationSpecs.Starfield_CF_AludraTahan());
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "NPC_")]
    [Trait("FormKey", "0A0273:Starfield.esm")]
    [Trait("EditorID", "CF_CESandin")]
    [Trait("SpriggitFile", "Npcs/CF_CESandin - 0A0273_Starfield.esm.yaml")]
    public void Starfield_NPC__ShouldMatchSpriggitSample_CF_CESandin()
    {
        AssertNPC(NPCValidationSpecs.Starfield_CF_CESandin());
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "NPC_")]
    [Trait("FormKey", "09C32F:Starfield.esm")]
    [Trait("EditorID", "CF_CPMurata")]
    [Trait("SpriggitFile", "Npcs/CF_CPMurata - 09C32F_Starfield.esm.yaml")]
    public void Starfield_NPC__ShouldMatchSpriggitSample_CF_CPMurata()
    {
        AssertNPC(NPCValidationSpecs.Starfield_CF_CPMurata());
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "NPC_")]
    [Trait("FormKey", "0B6667:Starfield.esm")]
    [Trait("EditorID", "BE_FAB12_LvlCitizenChunks")]
    [Trait("SpriggitFile", "Npcs/BE_FAB12_LvlCitizenChunks - 0B6667_Starfield.esm.yaml")]
    public void Starfield_NPC__ShouldMatchSpriggitSample_BE_FAB12_LvlCitizenChunks()
    {
        AssertNPC(NPCValidationSpecs.Starfield_BE_FAB12_LvlCitizenChunks());
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "NPC_")]
    [Trait("FormKey", "17C10E:Starfield.esm")]
    [Trait("EditorID", "BQ01_Actor_EllieYankton")]
    [Trait("SpriggitFile", "Npcs/BQ01_Actor_EllieYankton - 17C10E_Starfield.esm.yaml")]
    public void Starfield_NPC__ShouldMatchSpriggitSample_BQ01_Actor_EllieYankton()
    {
        AssertNPC(NPCValidationSpecs.Starfield_BQ01_Actor_EllieYankton());
    }

    private void AssertNPC(ValidationSpec spec)
    {
        var dto = Helpers.GetDTO<NPCDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }
}
