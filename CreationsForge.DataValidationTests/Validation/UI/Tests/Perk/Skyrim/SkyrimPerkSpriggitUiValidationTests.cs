using Avalonia.Headless.XUnit;
using CreationsForge.DataValidationTests.Validation.Specs.Perk;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.UI.Tests.Perk.Skyrim;

/// <summary>
/// Validates Skyrim perk Spriggit samples against the rendered comparison UI.
/// </summary>
public class SkyrimPerkSpriggitUiValidationTests :
    SpriggitComparisonUiTestBase,
    IClassFixture<SpriggitComparisonUiFixture>
{
    private readonly SpriggitComparisonUiFixture fixture;

    /// <summary>
    /// Initializes the UI validation tests with shared comparison fixture state.
    /// </summary>
    /// <param name="fixture">The fixture used to resolve imported records and comparison services.</param>
    public SkyrimPerkSpriggitUiValidationTests(SpriggitComparisonUiFixture fixture)
    {
        this.fixture = fixture;
    }

    /// <summary>
    /// Validates the Skyrim <c>AlchemySkillBoosts</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "PERK")]
    [Trait("FormKey", "0A725C:Skyrim.esm")]
    [Trait("EditorID", "AlchemySkillBoosts")]
    [Trait("SpriggitFile", "Perks/AlchemySkillBoosts - 0A725C_Skyrim.esm.yaml")]
    public void Skyrim_PERK_ComparisonUi_ShouldRenderSpriggitSample_AlchemySkillBoosts()
    {
        var spec = PerkValidationSpecs.Skyrim_AlchemySkillBoosts();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Skyrim <c>Armsman00</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "PERK")]
    [Trait("FormKey", "0BABE4:Skyrim.esm")]
    [Trait("EditorID", "Armsman00")]
    [Trait("SpriggitFile", "Perks/Armsman00 - 0BABE4_Skyrim.esm.yaml")]
    public void Skyrim_PERK_ComparisonUi_ShouldRenderSpriggitSample_Armsman00()
    {
        var spec = PerkValidationSpecs.Skyrim_Armsman00();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Skyrim <c>Armsman20</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "PERK")]
    [Trait("FormKey", "079343:Skyrim.esm")]
    [Trait("EditorID", "Armsman20")]
    [Trait("SpriggitFile", "Perks/Armsman20 - 079343_Skyrim.esm.yaml")]
    public void Skyrim_PERK_ComparisonUi_ShouldRenderSpriggitSample_Armsman20()
    {
        var spec = PerkValidationSpecs.Skyrim_Armsman20();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Skyrim <c>Allure</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "PERK")]
    [Trait("FormKey", "058F75:Skyrim.esm")]
    [Trait("EditorID", "Allure")]
    [Trait("SpriggitFile", "Perks/Allure - 058F75_Skyrim.esm.yaml")]
    public void Skyrim_PERK_ComparisonUi_ShouldRenderSpriggitSample_Allure()
    {
        var spec = PerkValidationSpecs.Skyrim_Allure();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }
}
