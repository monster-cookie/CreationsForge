using CreationsForge.Core.DTOs.Records;
using CreationsForge.DataValidationTests.Validation.Specs;
using CreationsForge.DataValidationTests.Validation.Specs.Faction;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.Faction.Fallout4;

public class Fallout4FactionSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "FACT")]
    [Trait("FormKey", "0975FC:Fallout4.esm")]
    [Trait("EditorID", "DNFinancial_OpalVendorFaction")]
    [Trait("SpriggitFile", "Factions/DNFinancial_OpalVendorFaction - 0975FC_Fallout4.esm.yaml")]
    public void Fallout4_FACT_ShouldMatchSpriggitSample_DNFinancial_OpalVendorFaction()
    {
        var spec = FactionValidationSpecs.Fallout4_DNFinancial_OpalVendorFaction();
        var dto = Helpers.GetDTO<FactionDTO>(spec.Game, spec.RecordType, spec.FormKey);

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
        foreach (var assertion in ValidationSpecRunner.GetAssertionCases(spec, dto))
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "FACT")]
    [Trait("FormKey", "03E0C8:Fallout4.esm")]
    [Trait("EditorID", "CaptiveFaction")]
    [Trait("SpriggitFile", "Factions/CaptiveFaction - 03E0C8_Fallout4.esm.yaml")]
    public void Fallout4_FACT_ShouldMatchSpriggitSample_CaptiveFaction()
    {
        var spec = FactionValidationSpecs.Fallout4_CaptiveFaction();
        var dto = Helpers.GetDTO<FactionDTO>(spec.Game, spec.RecordType, spec.FormKey);

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
        foreach (var assertion in ValidationSpecRunner.GetAssertionCases(spec, dto))
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "FACT")]
    [Trait("FormKey", "01C21C:Fallout4.esm")]
    [Trait("EditorID", "PlayerFaction")]
    [Trait("SpriggitFile", "Factions/PlayerFaction - 01C21C_Fallout4.esm.yaml")]
    public void Fallout4_FACT_ShouldMatchSpriggitSample_PlayerFaction()
    {
        var spec = FactionValidationSpecs.Fallout4_PlayerFaction();
        var dto = Helpers.GetDTO<FactionDTO>(spec.Game, spec.RecordType, spec.FormKey);

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
        foreach (var assertion in ValidationSpecRunner.GetAssertionCases(spec, dto))
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "FACT")]
    [Trait("FormKey", "157ACE:Fallout4.esm")]
    [Trait("EditorID", "DN049BakeryClerkFaction")]
    [Trait("SpriggitFile", "Factions/DN049BakeryClerkFaction - 157ACE_Fallout4.esm.yaml")]
    public void Fallout4_FACT_ShouldMatchSpriggitSample_DN049BakeryClerkFaction()
    {
        var spec = FactionValidationSpecs.Fallout4_DN049BakeryClerkFaction();
        var dto = Helpers.GetDTO<FactionDTO>(spec.Game, spec.RecordType, spec.FormKey);

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
        foreach (var assertion in ValidationSpecRunner.GetAssertionCases(spec, dto))
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "FACT")]
    [Trait("FormKey", "14EB97:Fallout4.esm")]
    [Trait("EditorID", "FarmVendorTheSlog")]
    [Trait("SpriggitFile", "Factions/FarmVendorTheSlog - 14EB97_Fallout4.esm.yaml")]
    public void Fallout4_FACT_ShouldMatchSpriggitSample_FarmVendorTheSlog()
    {
        var spec = FactionValidationSpecs.Fallout4_FarmVendorTheSlog();
        var dto = Helpers.GetDTO<FactionDTO>(spec.Game, spec.RecordType, spec.FormKey);

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
        foreach (var assertion in ValidationSpecRunner.GetAssertionCases(spec, dto))
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }
}
