using Avalonia.Headless.XUnit;
using CreationsForge.DataValidationTests.Validation.Specs.Container;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.UI.Tests.Container;

/// <summary>
/// Validates container Spriggit samples against the rendered comparison UI.
/// </summary>
public class ContainerSpriggitUiValidationTests :
    SpriggitComparisonUiTestBase,
    IClassFixture<SpriggitComparisonUiFixture>
{
    private readonly SpriggitComparisonUiFixture fixture;

    /// <summary>
    /// Initializes the UI validation tests with shared comparison fixture state.
    /// </summary>
    /// <param name="fixture">The fixture used to resolve imported records and comparison services.</param>
    public ContainerSpriggitUiValidationTests(SpriggitComparisonUiFixture fixture)
    {
        this.fixture = fixture;
    }

    /// <summary>
    /// Validates the Starfield <c>ShipOutpost_Loot_Storage_Safe_Floor_Reg_Common</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "CONT")]
    [Trait("FormKey", "277A73:Starfield.esm")]
    [Trait("EditorID", "ShipOutpost_Loot_Storage_Safe_Floor_Reg_Common")]
    [Trait("SpriggitFile", "Containers/ShipOutpost_Loot_Storage_Safe_Floor_Reg_Common - 277A73_Starfield.esm.yaml")]
    public void Starfield_CONT_ComparisonUi_ShouldRenderSpriggitSample_ShipOutpost_Loot_Storage_Safe_Floor_Reg_Common()
    {
        var spec = ContainerValidationSpecs.Starfield_ShipOutpost_Loot_Storage_Safe_Floor_Reg_Common();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout 4 <c>Loot_Raider_Safe</c> sample against rendered comparison rows.
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
    /// Validates the Skyrim <c>BeeHive</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "CONT")]
    [Trait("FormKey", "0A918C:Skyrim.esm")]
    [Trait("EditorID", "BeeHive")]
    [Trait("SpriggitFile", "Containers/BeeHive - 0A918C_Skyrim.esm.yaml")]
    public void Skyrim_CONT_ComparisonUi_ShouldRenderSpriggitSample_BeeHive()
    {
        var spec = ContainerValidationSpecs.Skyrim_BeeHive();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }
}
