using Avalonia.Headless.XUnit;
using CreationsForge.DataValidationTests.Validation.Specs.Container;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.UI.Tests.Container.Fallout4;

/// <summary>
/// Validates Fallout4 container Spriggit samples against the rendered comparison UI.
/// </summary>
public class Fallout4ContainerSpriggitUiValidationTests :
    SpriggitComparisonUiTestBase,
    IClassFixture<SpriggitComparisonUiFixture>
{
    private readonly SpriggitComparisonUiFixture fixture;

    /// <summary>
    /// Initializes the UI validation tests with shared comparison fixture state.
    /// </summary>
    /// <param name="fixture">The fixture used to resolve imported records and comparison services.</param>
    public Fallout4ContainerSpriggitUiValidationTests(SpriggitComparisonUiFixture fixture)
    {
        this.fixture = fixture;
    }

    /// <summary>
    /// Validates the Fallout4 <c>DN054Loot_Prewar_Safe</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "CONT")]
    [Trait("FormKey", "1F2B6A:Fallout4.esm")]
    [Trait("EditorID", "DN054Loot_Prewar_Safe")]
    [Trait("SpriggitFile", "Containers/DN054Loot_Prewar_Safe - 1F2B6A_Fallout4.esm.yaml")]
    public void Fallout4_CONT_ComparisonUi_ShouldRenderSpriggitSample_DN054Loot_Prewar_Safe()
    {
        var spec = ContainerValidationSpecs.Fallout4_DN054Loot_Prewar_Safe();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>Loot_Raider_Safe</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "CONT")]
    [Trait("FormKey", "064A36:Fallout4.esm")]
    [Trait("EditorID", "Loot_Raider_Safe")]
    [Trait("SpriggitFile", "Containers/Loot_Raider_Safe - 064A36_Fallout4.esm.yaml")]
    public void Fallout4_CONT_ComparisonUi_ShouldRenderSpriggitSample_Loot_Raider_Safe()
    {
        var spec = ContainerValidationSpecs.Fallout4_Loot_Raider_Safe();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>TheaterTickerTape_Safe</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "CONT")]
    [Trait("FormKey", "1C0292:Fallout4.esm")]
    [Trait("EditorID", "TheaterTickerTape_Safe")]
    [Trait("SpriggitFile", "Containers/TheaterTickerTape_Safe - 1C0292_Fallout4.esm.yaml")]
    public void Fallout4_CONT_ComparisonUi_ShouldRenderSpriggitSample_TheaterTickerTape_Safe()
    {
        var spec = ContainerValidationSpecs.Fallout4_TheaterTickerTape_Safe();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>Loot_Trunk_Boss</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "CONT")]
    [Trait("FormKey", "06355F:Fallout4.esm")]
    [Trait("EditorID", "Loot_Trunk_Boss")]
    [Trait("SpriggitFile", "Containers/Loot_Trunk_Boss - 06355F_Fallout4.esm.yaml")]
    public void Fallout4_CONT_ComparisonUi_ShouldRenderSpriggitSample_Loot_Trunk_Boss()
    {
        var spec = ContainerValidationSpecs.Fallout4_Loot_Trunk_Boss();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>DN123_SkylanesSecretCompartment</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "CONT")]
    [Trait("FormKey", "11CB14:Fallout4.esm")]
    [Trait("EditorID", "DN123_SkylanesSecretCompartment")]
    [Trait("SpriggitFile", "Containers/DN123_SkylanesSecretCompartment - 11CB14_Fallout4.esm.yaml")]
    public void Fallout4_CONT_ComparisonUi_ShouldRenderSpriggitSample_DN123_SkylanesSecretCompartment()
    {
        var spec = ContainerValidationSpecs.Fallout4_DN123_SkylanesSecretCompartment();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }
}
