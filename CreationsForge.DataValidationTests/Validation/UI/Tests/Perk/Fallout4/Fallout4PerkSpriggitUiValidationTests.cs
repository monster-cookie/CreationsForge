using Avalonia.Headless.XUnit;
using CreationsForge.Specification.Validation.Specs.Perk;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.UI.Tests.Perk.Fallout4;

/// <summary>
/// Validates Fallout4 perk Spriggit samples against the rendered comparison UI.
/// </summary>
public class Fallout4PerkSpriggitUiValidationTests :
    SpriggitComparisonUiTestBase,
    IClassFixture<SpriggitComparisonUiFixture>
{
    private readonly SpriggitComparisonUiFixture fixture;

    /// <summary>
    /// Initializes the UI validation tests with shared comparison fixture state.
    /// </summary>
    /// <param name="fixture">The fixture used to resolve imported records and comparison services.</param>
    public Fallout4PerkSpriggitUiValidationTests(SpriggitComparisonUiFixture fixture)
    {
        this.fixture = fixture;
    }

    /// <summary>
    /// Validates the Fallout4 <c>AddictionManager</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "PERK")]
    [Trait("FormKey", "2458BA:Fallout4.esm")]
    [Trait("EditorID", "AddictionManager")]
    [Trait("SpriggitFile", "Perks/AddictionManager - 2458BA_Fallout4.esm.yaml")]
    public void Fallout4_PERK_ComparisonUi_ShouldRenderSpriggitSample_AddictionManager()
    {
        var spec = PerkValidationSpecs.Fallout4_AddictionManager();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>AnimalFriend01</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "PERK")]
    [Trait("FormKey", "01E67F:Fallout4.esm")]
    [Trait("EditorID", "AnimalFriend01")]
    [Trait("SpriggitFile", "Perks/AnimalFriend01 - 01E67F_Fallout4.esm.yaml")]
    public void Fallout4_PERK_ComparisonUi_ShouldRenderSpriggitSample_AnimalFriend01()
    {
        var spec = PerkValidationSpecs.Fallout4_AnimalFriend01();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>AnimalFriend02</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "PERK")]
    [Trait("FormKey", "04A0D9:Fallout4.esm")]
    [Trait("EditorID", "AnimalFriend02")]
    [Trait("SpriggitFile", "Perks/AnimalFriend02 - 04A0D9_Fallout4.esm.yaml")]
    public void Fallout4_PERK_ComparisonUi_ShouldRenderSpriggitSample_AnimalFriend02()
    {
        var spec = PerkValidationSpecs.Fallout4_AnimalFriend02();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>TrainingAG01</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "PERK")]
    [Trait("FormKey", "0D979D:Fallout4.esm")]
    [Trait("EditorID", "TrainingAG01")]
    [Trait("SpriggitFile", "Perks/TrainingAG01 - 0D979D_Fallout4.esm.yaml")]
    public void Fallout4_PERK_ComparisonUi_ShouldRenderSpriggitSample_TrainingAG01()
    {
        var spec = PerkValidationSpecs.Fallout4_TrainingAG01();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>Basher02</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "PERK")]
    [Trait("FormKey", "065DFA:Fallout4.esm")]
    [Trait("EditorID", "Basher02")]
    [Trait("SpriggitFile", "Perks/Basher02 - 065DFA_Fallout4.esm.yaml")]
    public void Fallout4_PERK_ComparisonUi_ShouldRenderSpriggitSample_Basher02()
    {
        var spec = PerkValidationSpecs.Fallout4_Basher02();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }
}
