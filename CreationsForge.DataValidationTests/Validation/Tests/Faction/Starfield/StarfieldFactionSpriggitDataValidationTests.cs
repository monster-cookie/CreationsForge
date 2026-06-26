using CreationsForge.Core.DTOs.Records;
using CreationsForge.DataValidationTests.Validation.Specs;
using CreationsForge.DataValidationTests.Validation.Specs.Faction;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.Faction.Starfield;

public class StarfieldFactionSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "FACT")]
    [Trait("FormKey", "010B30:Starfield.esm")]
    [Trait("EditorID", "CrimeFactionCrimsonFleet")]
    [Trait("SpriggitFile", "Factions/CrimeFactionCrimsonFleet - 010B30_Starfield.esm.yaml")]
    public void Starfield_FACT_ShouldMatchSpriggitSample_CrimeFactionCrimsonFleet()
    {
        var spec = FactionValidationSpecs.Starfield_CrimeFactionCrimsonFleet();
        var dto = Helpers.GetDTO<FactionDTO>(spec.Game, spec.RecordType, spec.FormKey);

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
        foreach (var assertion in ValidationSpecRunner.GetAssertionCases(spec, dto))
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "FACT")]
    [Trait("FormKey", "03E0C8:Starfield.esm")]
    [Trait("EditorID", "CaptiveFaction")]
    [Trait("SpriggitFile", "Factions/CaptiveFaction - 03E0C8_Starfield.esm.yaml")]
    public void Starfield_FACT_ShouldMatchSpriggitSample_CaptiveFaction()
    {
        var spec = FactionValidationSpecs.Starfield_CaptiveFaction();
        var dto = Helpers.GetDTO<FactionDTO>(spec.Game, spec.RecordType, spec.FormKey);

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
        foreach (var assertion in ValidationSpecRunner.GetAssertionCases(spec, dto))
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "FACT")]
    [Trait("FormKey", "01C21C:Starfield.esm")]
    [Trait("EditorID", "PlayerFaction")]
    [Trait("SpriggitFile", "Factions/PlayerFaction - 01C21C_Starfield.esm.yaml")]
    public void Starfield_FACT_ShouldMatchSpriggitSample_PlayerFaction()
    {
        var spec = FactionValidationSpecs.Starfield_PlayerFaction();
        var dto = Helpers.GetDTO<FactionDTO>(spec.Game, spec.RecordType, spec.FormKey);

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
        foreach (var assertion in ValidationSpecRunner.GetAssertionCases(spec, dto))
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "FACT")]
    [Trait("FormKey", "1A2C9C:Starfield.esm")]
    [Trait("EditorID", "LISTColonistFaction")]
    [Trait("SpriggitFile", "Factions/LISTColonistFaction - 1A2C9C_Starfield.esm.yaml")]
    public void Starfield_FACT_ShouldMatchSpriggitSample_LISTColonistFaction()
    {
        var spec = FactionValidationSpecs.Starfield_LISTColonistFaction();
        var dto = Helpers.GetDTO<FactionDTO>(spec.Game, spec.RecordType, spec.FormKey);

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
        foreach (var assertion in ValidationSpecRunner.GetAssertionCases(spec, dto))
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "FACT")]
    [Trait("FormKey", "3CAFBA:Starfield.esm")]
    [Trait("EditorID", "Vendor_ShipServices_AkilaCityFaction")]
    [Trait("SpriggitFile", "Factions/Vendor_ShipServices_AkilaCityFaction - 3CAFBA_Starfield.esm.yaml")]
    public void Starfield_FACT_ShouldMatchSpriggitSample_Vendor_ShipServices_AkilaCityFaction()
    {
        var spec = FactionValidationSpecs.Starfield_Vendor_ShipServices_AkilaCityFaction();
        var dto = Helpers.GetDTO<FactionDTO>(spec.Game, spec.RecordType, spec.FormKey);

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
        foreach (var assertion in ValidationSpecRunner.GetAssertionCases(spec, dto))
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }
}
