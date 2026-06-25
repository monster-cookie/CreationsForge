using Avalonia.Headless.XUnit;
using CreationsForge.DataValidationTests.Validation.Specs.Container;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.UI.Tests.Container.Skyrim;

/// <summary>
/// Validates Skyrim container Spriggit samples against the rendered comparison UI.
/// </summary>
public class SkyrimContainerSpriggitUiValidationTests :
    SpriggitComparisonUiTestBase,
    IClassFixture<SpriggitComparisonUiFixture>
{
    private readonly SpriggitComparisonUiFixture fixture;

    /// <summary>
    /// Initializes the UI validation tests with shared comparison fixture state.
    /// </summary>
    /// <param name="fixture">The fixture used to resolve imported records and comparison services.</param>
    public SkyrimContainerSpriggitUiValidationTests(SpriggitComparisonUiFixture fixture)
    {
        this.fixture = fixture;
    }

    /// <summary>
    /// Validates the Skyrim <c>TreasFalmerChestBoss</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "CONT")]
    [Trait("FormKey", "02065B:Skyrim.esm")]
    [Trait("EditorID", "TreasFalmerChestBoss")]
    [Trait("SpriggitFile", "Containers/TreasFalmerChestBoss - 02065B_Skyrim.esm.yaml")]
    public void Skyrim_CONT_ComparisonUi_ShouldRenderSpriggitSample_TreasFalmerChestBoss()
    {
        var spec = ContainerValidationSpecs.Skyrim_TreasFalmerChestBoss();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Skyrim <c>TreasFalmerChestBossDwarven</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "CONT")]
    [Trait("FormKey", "0B1176:Skyrim.esm")]
    [Trait("EditorID", "TreasFalmerChestBossDwarven")]
    [Trait("SpriggitFile", "Containers/TreasFalmerChestBossDwarven - 0B1176_Skyrim.esm.yaml")]
    public void Skyrim_CONT_ComparisonUi_ShouldRenderSpriggitSample_TreasFalmerChestBossDwarven()
    {
        var spec = ContainerValidationSpecs.Skyrim_TreasFalmerChestBossDwarven();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Skyrim <c>TreasFalmerChest</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "CONT")]
    [Trait("FormKey", "020659:Skyrim.esm")]
    [Trait("EditorID", "TreasFalmerChest")]
    [Trait("SpriggitFile", "Containers/TreasFalmerChest - 020659_Skyrim.esm.yaml")]
    public void Skyrim_CONT_ComparisonUi_ShouldRenderSpriggitSample_TreasFalmerChest()
    {
        var spec = ContainerValidationSpecs.Skyrim_TreasFalmerChest();
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

    /// <summary>
    /// Validates the Skyrim <c>MerchantCaravanAChest</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "CONT")]
    [Trait("FormKey", "07434B:Skyrim.esm")]
    [Trait("EditorID", "MerchantCaravanAChest")]
    [Trait("SpriggitFile", "Containers/MerchantCaravanAChest - 07434B_Skyrim.esm.yaml")]
    public void Skyrim_CONT_ComparisonUi_ShouldRenderSpriggitSample_MerchantCaravanAChest()
    {
        var spec = ContainerValidationSpecs.Skyrim_MerchantCaravanAChest();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }
}
