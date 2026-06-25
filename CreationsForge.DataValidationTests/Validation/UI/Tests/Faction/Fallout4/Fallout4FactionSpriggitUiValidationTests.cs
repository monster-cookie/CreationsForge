using Avalonia.Headless.XUnit;
using CreationsForge.DataValidationTests.Validation.Specs.Faction;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.UI.Tests.Faction.Fallout4;

/// <summary>
/// Validates Fallout4 faction Spriggit samples against the rendered comparison UI.
/// </summary>
public class Fallout4FactionSpriggitUiValidationTests :
    SpriggitComparisonUiTestBase,
    IClassFixture<SpriggitComparisonUiFixture>
{
    private readonly SpriggitComparisonUiFixture fixture;

    /// <summary>
    /// Initializes the UI validation tests with shared comparison fixture state.
    /// </summary>
    /// <param name="fixture">The fixture used to resolve imported records and comparison services.</param>
    public Fallout4FactionSpriggitUiValidationTests(SpriggitComparisonUiFixture fixture)
    {
        this.fixture = fixture;
    }

    /// <summary>
    /// Validates the Fallout4 <c>DNFinancial_OpalVendorFaction</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "FACT")]
    [Trait("FormKey", "0975FC:Fallout4.esm")]
    [Trait("EditorID", "DNFinancial_OpalVendorFaction")]
    [Trait("SpriggitFile", "Factions/DNFinancial_OpalVendorFaction - 0975FC_Fallout4.esm.yaml")]
    public void Fallout4_FACT_ComparisonUi_ShouldRenderSpriggitSample_DNFinancial_OpalVendorFaction()
    {
        var spec = FactionValidationSpecs.Fallout4_DNFinancial_OpalVendorFaction();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>CaptiveFaction</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "FACT")]
    [Trait("FormKey", "03E0C8:Fallout4.esm")]
    [Trait("EditorID", "CaptiveFaction")]
    [Trait("SpriggitFile", "Factions/CaptiveFaction - 03E0C8_Fallout4.esm.yaml")]
    public void Fallout4_FACT_ComparisonUi_ShouldRenderSpriggitSample_CaptiveFaction()
    {
        var spec = FactionValidationSpecs.Fallout4_CaptiveFaction();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>PlayerFaction</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "FACT")]
    [Trait("FormKey", "01C21C:Fallout4.esm")]
    [Trait("EditorID", "PlayerFaction")]
    [Trait("SpriggitFile", "Factions/PlayerFaction - 01C21C_Fallout4.esm.yaml")]
    public void Fallout4_FACT_ComparisonUi_ShouldRenderSpriggitSample_PlayerFaction()
    {
        var spec = FactionValidationSpecs.Fallout4_PlayerFaction();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>DN049BakeryClerkFaction</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "FACT")]
    [Trait("FormKey", "157ACE:Fallout4.esm")]
    [Trait("EditorID", "DN049BakeryClerkFaction")]
    [Trait("SpriggitFile", "Factions/DN049BakeryClerkFaction - 157ACE_Fallout4.esm.yaml")]
    public void Fallout4_FACT_ComparisonUi_ShouldRenderSpriggitSample_DN049BakeryClerkFaction()
    {
        var spec = FactionValidationSpecs.Fallout4_DN049BakeryClerkFaction();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>FarmVendorTheSlog</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "FACT")]
    [Trait("FormKey", "14EB97:Fallout4.esm")]
    [Trait("EditorID", "FarmVendorTheSlog")]
    [Trait("SpriggitFile", "Factions/FarmVendorTheSlog - 14EB97_Fallout4.esm.yaml")]
    public void Fallout4_FACT_ComparisonUi_ShouldRenderSpriggitSample_FarmVendorTheSlog()
    {
        var spec = FactionValidationSpecs.Fallout4_FarmVendorTheSlog();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }
}
