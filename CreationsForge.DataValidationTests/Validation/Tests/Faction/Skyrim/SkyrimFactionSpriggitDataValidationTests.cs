using CreationsForge.Core.DTOs.Records;
using CreationsForge.DataValidationTests.Validation.Specs;
using CreationsForge.DataValidationTests.Validation.Specs.Faction;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.Faction.Skyrim;

public class SkyrimFactionSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "FACT")]
    [Trait("FormKey", "103372:Skyrim.esm")]
    [Trait("EditorID", "CollegeofWinterholdArchMageFaction")]
    [Trait("SpriggitFile", "Factions/CollegeofWinterholdArchMageFaction - 103372_Skyrim.esm.yaml")]
    public void Skyrim_FACT_ShouldMatchSpriggitSample_CollegeofWinterholdArchMageFaction()
    {
        var spec = FactionValidationSpecs.Skyrim_CollegeofWinterholdArchMageFaction();
        var dto = Helpers.GetDTO<FactionDTO>(spec.Game, spec.RecordType, spec.FormKey);

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
        foreach (var assertion in ValidationSpecRunner.GetAssertionCases(spec, dto))
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "FACT")]
    [Trait("FormKey", "01F259:Skyrim.esm")]
    [Trait("EditorID", "CollegeofWinterholdFaction")]
    [Trait("SpriggitFile", "Factions/CollegeofWinterholdFaction - 01F259_Skyrim.esm.yaml")]
    public void Skyrim_FACT_ShouldMatchSpriggitSample_CollegeofWinterholdFaction()
    {
        var spec = FactionValidationSpecs.Skyrim_CollegeofWinterholdFaction();
        var dto = Helpers.GetDTO<FactionDTO>(spec.Game, spec.RecordType, spec.FormKey);

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
        foreach (var assertion in ValidationSpecRunner.GetAssertionCases(spec, dto))
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "FACT")]
    [Trait("FormKey", "048362:Skyrim.esm")]
    [Trait("EditorID", "CompanionsFaction")]
    [Trait("SpriggitFile", "Factions/CompanionsFaction - 048362_Skyrim.esm.yaml")]
    public void Skyrim_FACT_ShouldMatchSpriggitSample_CompanionsFaction()
    {
        var spec = FactionValidationSpecs.Skyrim_CompanionsFaction();
        var dto = Helpers.GetDTO<FactionDTO>(spec.Game, spec.RecordType, spec.FormKey);

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
        foreach (var assertion in ValidationSpecRunner.GetAssertionCases(spec, dto))
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "FACT")]
    [Trait("FormKey", "0FFD65:Skyrim.esm")]
    [Trait("EditorID", "DBSancBabetteBedFaction")]
    [Trait("SpriggitFile", "Factions/DBSancBabetteBedFaction - 0FFD65_Skyrim.esm.yaml")]
    public void Skyrim_FACT_ShouldMatchSpriggitSample_DBSancBabetteBedFaction()
    {
        var spec = FactionValidationSpecs.Skyrim_DBSancBabetteBedFaction();
        var dto = Helpers.GetDTO<FactionDTO>(spec.Game, spec.RecordType, spec.FormKey);

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
        foreach (var assertion in ValidationSpecRunner.GetAssertionCases(spec, dto))
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "FACT")]
    [Trait("FormKey", "040B60:Skyrim.esm")]
    [Trait("EditorID", "ArenaFaction")]
    [Trait("SpriggitFile", "Factions/ArenaFaction - 040B60_Skyrim.esm.yaml")]
    public void Skyrim_FACT_ShouldMatchSpriggitSample_ArenaFaction()
    {
        var spec = FactionValidationSpecs.Skyrim_ArenaFaction();
        var dto = Helpers.GetDTO<FactionDTO>(spec.Game, spec.RecordType, spec.FormKey);

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
        foreach (var assertion in ValidationSpecRunner.GetAssertionCases(spec, dto))
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }
}
