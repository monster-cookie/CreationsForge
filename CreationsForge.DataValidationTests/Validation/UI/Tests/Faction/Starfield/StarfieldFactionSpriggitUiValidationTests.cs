using Avalonia.Headless.XUnit;
using CreationsForge.DataValidationTests.Validation.Specs.Faction;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.UI.Tests.Faction.Starfield;

/// <summary>
/// Validates Starfield faction Spriggit samples against the rendered comparison UI.
/// </summary>
public class StarfieldFactionSpriggitUiValidationTests :
    SpriggitComparisonUiTestBase,
    IClassFixture<SpriggitComparisonUiFixture>
{
    private readonly SpriggitComparisonUiFixture fixture;

    /// <summary>
    /// Initializes the UI validation tests with shared comparison fixture state.
    /// </summary>
    /// <param name="fixture">The fixture used to resolve imported records and comparison services.</param>
    public StarfieldFactionSpriggitUiValidationTests(SpriggitComparisonUiFixture fixture)
    {
        this.fixture = fixture;
    }

    /// <summary>
    /// Validates the Starfield <c>CrimeFactionCrimsonFleet</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "FACT")]
    [Trait("FormKey", "010B30:Starfield.esm")]
    [Trait("EditorID", "CrimeFactionCrimsonFleet")]
    [Trait("SpriggitFile", "Factions/CrimeFactionCrimsonFleet - 010B30_Starfield.esm.yaml")]
    public void Starfield_FACT_ComparisonUi_ShouldRenderSpriggitSample_CrimeFactionCrimsonFleet()
    {
        var spec = FactionValidationSpecs.Starfield_CrimeFactionCrimsonFleet();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>CaptiveFaction</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "FACT")]
    [Trait("FormKey", "03E0C8:Starfield.esm")]
    [Trait("EditorID", "CaptiveFaction")]
    [Trait("SpriggitFile", "Factions/CaptiveFaction - 03E0C8_Starfield.esm.yaml")]
    public void Starfield_FACT_ComparisonUi_ShouldRenderSpriggitSample_CaptiveFaction()
    {
        var spec = FactionValidationSpecs.Starfield_CaptiveFaction();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>PlayerFaction</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "FACT")]
    [Trait("FormKey", "01C21C:Starfield.esm")]
    [Trait("EditorID", "PlayerFaction")]
    [Trait("SpriggitFile", "Factions/PlayerFaction - 01C21C_Starfield.esm.yaml")]
    public void Starfield_FACT_ComparisonUi_ShouldRenderSpriggitSample_PlayerFaction()
    {
        var spec = FactionValidationSpecs.Starfield_PlayerFaction();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>LISTColonistFaction</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "FACT")]
    [Trait("FormKey", "1A2C9C:Starfield.esm")]
    [Trait("EditorID", "LISTColonistFaction")]
    [Trait("SpriggitFile", "Factions/LISTColonistFaction - 1A2C9C_Starfield.esm.yaml")]
    public void Starfield_FACT_ComparisonUi_ShouldRenderSpriggitSample_LISTColonistFaction()
    {
        var spec = FactionValidationSpecs.Starfield_LISTColonistFaction();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>Vendor_ShipServices_AkilaCityFaction</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "FACT")]
    [Trait("FormKey", "3CAFBA:Starfield.esm")]
    [Trait("EditorID", "Vendor_ShipServices_AkilaCityFaction")]
    [Trait("SpriggitFile", "Factions/Vendor_ShipServices_AkilaCityFaction - 3CAFBA_Starfield.esm.yaml")]
    public void Starfield_FACT_ComparisonUi_ShouldRenderSpriggitSample_Vendor_ShipServices_AkilaCityFaction()
    {
        var spec = FactionValidationSpecs.Starfield_Vendor_ShipServices_AkilaCityFaction();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }
}
