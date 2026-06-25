using Avalonia.Headless.XUnit;
using CreationsForge.DataValidationTests.Validation.Specs.Container;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.UI.Tests.Container.Starfield;

/// <summary>
/// Validates Starfield container Spriggit samples against the rendered comparison UI.
/// </summary>
public class StarfieldContainerSpriggitUiValidationTests :
    SpriggitComparisonUiTestBase,
    IClassFixture<SpriggitComparisonUiFixture>
{
    private readonly SpriggitComparisonUiFixture fixture;

    /// <summary>
    /// Initializes the UI validation tests with shared comparison fixture state.
    /// </summary>
    /// <param name="fixture">The fixture used to resolve imported records and comparison services.</param>
    public StarfieldContainerSpriggitUiValidationTests(SpriggitComparisonUiFixture fixture)
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
    /// Validates the Starfield <c>ShipOutpost_Loot_Storage_Safe_Floor_Tall_Rare</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "CONT")]
    [Trait("FormKey", "277A81:Starfield.esm")]
    [Trait("EditorID", "ShipOutpost_Loot_Storage_Safe_Floor_Tall_Rare")]
    [Trait("SpriggitFile", "Containers/ShipOutpost_Loot_Storage_Safe_Floor_Tall_Rare - 277A81_Starfield.esm.yaml")]
    public void Starfield_CONT_ComparisonUi_ShouldRenderSpriggitSample_ShipOutpost_Loot_Storage_Safe_Floor_Tall_Rare()
    {
        var spec = ContainerValidationSpecs.Starfield_ShipOutpost_Loot_Storage_Safe_Floor_Tall_Rare();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>ShipOutpost_Loot_Storage_BossChest_Industrial_Rare</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "CONT")]
    [Trait("FormKey", "2779E9:Starfield.esm")]
    [Trait("EditorID", "ShipOutpost_Loot_Storage_BossChest_Industrial_Rare")]
    [Trait("SpriggitFile", "Containers/ShipOutpost_Loot_Storage_BossChest_Industrial_Rare - 2779E9_Starfield.esm.yaml")]
    public void Starfield_CONT_ComparisonUi_ShouldRenderSpriggitSample_ShipOutpost_Loot_Storage_BossChest_Industrial_Rare()
    {
        var spec = ContainerValidationSpecs.Starfield_ShipOutpost_Loot_Storage_BossChest_Industrial_Rare();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>Loot_Display_WeaponRack03_EMPTY</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "CONT")]
    [Trait("FormKey", "1A23DF:Starfield.esm")]
    [Trait("EditorID", "Loot_Display_WeaponRack03_EMPTY")]
    [Trait("SpriggitFile", "Containers/Loot_Display_WeaponRack03_EMPTY - 1A23DF_Starfield.esm.yaml")]
    public void Starfield_CONT_ComparisonUi_ShouldRenderSpriggitSample_Loot_Display_WeaponRack03_EMPTY()
    {
        var spec = ContainerValidationSpecs.Starfield_Loot_Display_WeaponRack03_EMPTY();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>Loot_Display_ArboronWeaponRackPanel02</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "CONT")]
    [Trait("FormKey", "057C20:Starfield.esm")]
    [Trait("EditorID", "Loot_Display_ArboronWeaponRackPanel02")]
    [Trait("SpriggitFile", "Containers/Loot_Display_ArboronWeaponRackPanel02 - 057C20_Starfield.esm.yaml")]
    public void Starfield_CONT_ComparisonUi_ShouldRenderSpriggitSample_Loot_Display_ArboronWeaponRackPanel02()
    {
        var spec = ContainerValidationSpecs.Starfield_Loot_Display_ArboronWeaponRackPanel02();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }
}
