using Avalonia.Headless.XUnit;
using CreationsForge.Specification.Validation.Specs.Faction;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.UI.Tests.Faction.Skyrim;

/// <summary>
/// Validates Skyrim faction Spriggit samples against the rendered comparison UI.
/// </summary>
public class SkyrimFactionSpriggitUiValidationTests :
    SpriggitComparisonUiTestBase,
    IClassFixture<SpriggitComparisonUiFixture>
{
    private readonly SpriggitComparisonUiFixture fixture;

    /// <summary>
    /// Initializes the UI validation tests with shared comparison fixture state.
    /// </summary>
    /// <param name="fixture">The fixture used to resolve imported records and comparison services.</param>
    public SkyrimFactionSpriggitUiValidationTests(SpriggitComparisonUiFixture fixture)
    {
        this.fixture = fixture;
    }

    /// <summary>
    /// Validates the Skyrim <c>CollegeofWinterholdArchMageFaction</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "FACT")]
    [Trait("FormKey", "103372:Skyrim.esm")]
    [Trait("EditorID", "CollegeofWinterholdArchMageFaction")]
    [Trait("SpriggitFile", "Factions/CollegeofWinterholdArchMageFaction - 103372_Skyrim.esm.yaml")]
    public void Skyrim_FACT_ComparisonUi_ShouldRenderSpriggitSample_CollegeofWinterholdArchMageFaction()
    {
        var spec = FactionValidationSpecs.Skyrim_CollegeofWinterholdArchMageFaction();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Skyrim <c>CollegeofWinterholdFaction</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "FACT")]
    [Trait("FormKey", "01F259:Skyrim.esm")]
    [Trait("EditorID", "CollegeofWinterholdFaction")]
    [Trait("SpriggitFile", "Factions/CollegeofWinterholdFaction - 01F259_Skyrim.esm.yaml")]
    public void Skyrim_FACT_ComparisonUi_ShouldRenderSpriggitSample_CollegeofWinterholdFaction()
    {
        var spec = FactionValidationSpecs.Skyrim_CollegeofWinterholdFaction();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Skyrim <c>CompanionsFaction</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "FACT")]
    [Trait("FormKey", "048362:Skyrim.esm")]
    [Trait("EditorID", "CompanionsFaction")]
    [Trait("SpriggitFile", "Factions/CompanionsFaction - 048362_Skyrim.esm.yaml")]
    public void Skyrim_FACT_ComparisonUi_ShouldRenderSpriggitSample_CompanionsFaction()
    {
        var spec = FactionValidationSpecs.Skyrim_CompanionsFaction();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Skyrim <c>DBSancBabetteBedFaction</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "FACT")]
    [Trait("FormKey", "0FFD65:Skyrim.esm")]
    [Trait("EditorID", "DBSancBabetteBedFaction")]
    [Trait("SpriggitFile", "Factions/DBSancBabetteBedFaction - 0FFD65_Skyrim.esm.yaml")]
    public void Skyrim_FACT_ComparisonUi_ShouldRenderSpriggitSample_DBSancBabetteBedFaction()
    {
        var spec = FactionValidationSpecs.Skyrim_DBSancBabetteBedFaction();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Skyrim <c>ArenaFaction</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "FACT")]
    [Trait("FormKey", "040B60:Skyrim.esm")]
    [Trait("EditorID", "ArenaFaction")]
    [Trait("SpriggitFile", "Factions/ArenaFaction - 040B60_Skyrim.esm.yaml")]
    public void Skyrim_FACT_ComparisonUi_ShouldRenderSpriggitSample_ArenaFaction()
    {
        var spec = FactionValidationSpecs.Skyrim_ArenaFaction();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }
}
