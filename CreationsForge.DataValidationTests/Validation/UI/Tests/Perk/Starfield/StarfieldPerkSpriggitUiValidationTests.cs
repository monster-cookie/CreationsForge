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
    /// Validates the Starfield <c>Skill_BoostAssaultTraining</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "PERK")]
    [Trait("FormKey", "08C3EE:Starfield.esm")]
    [Trait("EditorID", "Skill_BoostAssaultTraining")]
    [Trait("SpriggitFile", "Perks/Skill_BoostAssaultTraining - 08C3EE_Starfield.esm.yaml")]
    public void Starfield_PERK_ComparisonUi_ShouldRenderSpriggitSample_Skill_BoostAssaultTraining()
    {
        var spec = PerkValidationSpecs.Starfield_Skill_BoostAssaultTraining();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
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

    /// <summary>
    /// Validates the Starfield <c>TrainingTechnologyExpert</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "PERK")]
    [Trait("FormKey", "27CBBE:Starfield.esm")]
    [Trait("EditorID", "TrainingTechnologyExpert")]
    [Trait("SpriggitFile", "Perks/TrainingTechnologyExpert - 27CBBE_Starfield.esm.yaml")]
    public void Starfield_PERK_ComparisonUi_ShouldRenderSpriggitSample_TrainingTechnologyExpert()
    {
        var spec = PerkValidationSpecs.Starfield_TrainingTechnologyExpert();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>TRAIT_FreestarCollectiveSettler</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "PERK")]
    [Trait("FormKey", "227FD5:Starfield.esm")]
    [Trait("EditorID", "TRAIT_FreestarCollectiveSettler")]
    [Trait("SpriggitFile", "Perks/TRAIT_FreestarCollectiveSettler - 227FD5_Starfield.esm.yaml")]
    public void Starfield_PERK_ComparisonUi_ShouldRenderSpriggitSample_TRAIT_FreestarCollectiveSettler()
    {
        var spec = PerkValidationSpecs.Starfield_TRAIT_FreestarCollectiveSettler();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>BackgroundBigGameHunter</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "PERK")]
    [Trait("FormKey", "22EC76:Starfield.esm")]
    [Trait("EditorID", "BackgroundBigGameHunter")]
    [Trait("SpriggitFile", "Perks/BackgroundBigGameHunter - 22EC76_Starfield.esm.yaml")]
    public void Starfield_PERK_ComparisonUi_ShouldRenderSpriggitSample_BackgroundBigGameHunter()
    {
        var spec = PerkValidationSpecs.Starfield_BackgroundBigGameHunter();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }
}
