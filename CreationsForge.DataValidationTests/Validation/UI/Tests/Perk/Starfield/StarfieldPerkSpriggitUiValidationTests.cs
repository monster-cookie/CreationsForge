using Avalonia.Headless.XUnit;
using CreationsForge.DataValidationTests.Validation.Specs.Perk;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.UI.Tests.Perk.Starfield;

/// <summary>
/// Validates Starfield perk Spriggit samples against the rendered comparison UI.
/// </summary>
public class StarfieldPerkSpriggitUiValidationTests :
    SpriggitComparisonUiTestBase,
    IClassFixture<SpriggitComparisonUiFixture>
{
    private readonly SpriggitComparisonUiFixture fixture;

    /// <summary>
    /// Initializes the UI validation tests with shared comparison fixture state.
    /// </summary>
    /// <param name="fixture">The fixture used to resolve imported records and comparison services.</param>
    public StarfieldPerkSpriggitUiValidationTests(SpriggitComparisonUiFixture fixture)
    {
        this.fixture = fixture;
    }

    /// <summary>
    /// Validates the Starfield <c>Skill_BoostPackTraining</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "PERK")]
    [Trait("FormKey", "146C2C:Starfield.esm")]
    [Trait("EditorID", "Skill_BoostPackTraining")]
    [Trait("SpriggitFile", "Perks/Skill_BoostPackTraining - 146C2C_Starfield.esm.yaml")]
    public void Starfield_PERK_ComparisonUi_ShouldRenderSpriggitSample_Skill_BoostPackTraining()
    {
        var spec = PerkValidationSpecs.Starfield_Skill_BoostPackTraining();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }
}
