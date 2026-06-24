using CreationsForge.Core.DTOs.Records;
using CreationsForge.DataValidationTests.Validation.Specs;
using CreationsForge.DataValidationTests.Validation.Specs.NPC;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.NPC.Skyrim;

public class SkyrimNPCSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "NPC_")]
    [Trait("FormKey", "0F6F37:Skyrim.esm")]
    [Trait("EditorID", "EncGuardImperialTemplate")]
    [Trait("SpriggitFile", "Npcs/EncGuardImperialTemplate - 0F6F37_Skyrim.esm.yaml")]
    public void Skyrim_NPC__ShouldMatchSpriggitSample_EncGuardImperialTemplate()
    {
        AssertNPC(NPCValidationSpecs.Skyrim_EncGuardImperialTemplate());
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "NPC_")]
    [Trait("FormKey", "0F6F38:Skyrim.esm")]
    [Trait("EditorID", "EncGuardSonsTemplate")]
    [Trait("SpriggitFile", "Npcs/EncGuardSonsTemplate - 0F6F38_Skyrim.esm.yaml")]
    public void Skyrim_NPC__ShouldMatchSpriggitSample_EncGuardSonsTemplate()
    {
        AssertNPC(NPCValidationSpecs.Skyrim_EncGuardSonsTemplate());
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "NPC_")]
    [Trait("FormKey", "041B30:Skyrim.esm")]
    [Trait("EditorID", "EncSiegeImperialSoldierTemplate")]
    [Trait("SpriggitFile", "Npcs/EncSiegeImperialSoldierTemplate - 041B30_Skyrim.esm.yaml")]
    public void Skyrim_NPC__ShouldMatchSpriggitSample_EncSiegeImperialSoldierTemplate()
    {
        AssertNPC(NPCValidationSpecs.Skyrim_EncSiegeImperialSoldierTemplate());
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "NPC_")]
    [Trait("FormKey", "01A696:Skyrim.esm")]
    [Trait("EditorID", "AelaTheHuntress")]
    [Trait("SpriggitFile", "Npcs/AelaTheHuntress - 01A696_Skyrim.esm.yaml")]
    public void Skyrim_NPC__ShouldMatchSpriggitSample_AelaTheHuntress()
    {
        AssertNPC(NPCValidationSpecs.Skyrim_AelaTheHuntress());
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "NPC_")]
    [Trait("FormKey", "08E4F1:Skyrim.esm")]
    [Trait("EditorID", "AlduinBase")]
    [Trait("SpriggitFile", "Npcs/AlduinBase - 08E4F1_Skyrim.esm.yaml")]
    public void Skyrim_NPC__ShouldMatchSpriggitSample_AlduinBase()
    {
        AssertNPC(NPCValidationSpecs.Skyrim_AlduinBase());
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
