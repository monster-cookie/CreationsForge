using Avalonia.Headless.XUnit;
using CreationsForge.Specification.Validation.Specs.ConstructibleObject;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.UI.Tests.ConstructibleObject.Skyrim;

/// <summary>
/// Validates Skyrim constructible object Spriggit samples against the rendered comparison UI.
/// </summary>
public class SkyrimConstructibleObjectSpriggitUiValidationTests :
    SpriggitComparisonUiTestBase,
    IClassFixture<SpriggitComparisonUiFixture>
{
    private readonly SpriggitComparisonUiFixture fixture;

    /// <summary>
    /// Initializes the UI validation tests with shared comparison fixture state.
    /// </summary>
    /// <param name="fixture">The fixture used to resolve imported records and comparison services.</param>
    public SkyrimConstructibleObjectSpriggitUiValidationTests(SpriggitComparisonUiFixture fixture)
    {
        this.fixture = fixture;
    }

    /// <summary>
    /// Validates the Skyrim <c>RecipeArmorDragonscaleBoots</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "COBJ")]
    [Trait("FormKey", "0DCA13:Skyrim.esm")]
    [Trait("EditorID", "RecipeArmorDragonscaleBoots")]
    [Trait("SpriggitFile", "ConstructibleObjects/RecipeArmorDragonscaleBoots - 0DCA13_Skyrim.esm.yaml")]
    public void Skyrim_COBJ_ComparisonUi_ShouldRenderSpriggitSample_RecipeArmorDragonscaleBoots()
    {
        var spec = ConstructibleObjectValidationSpecs.Skyrim_RecipeArmorDragonscaleBoots();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Skyrim <c>RecipeArmorDragonscaleCuirass</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "COBJ")]
    [Trait("FormKey", "0DCA14:Skyrim.esm")]
    [Trait("EditorID", "RecipeArmorDragonscaleCuirass")]
    [Trait("SpriggitFile", "ConstructibleObjects/RecipeArmorDragonscaleCuirass - 0DCA14_Skyrim.esm.yaml")]
    public void Skyrim_COBJ_ComparisonUi_ShouldRenderSpriggitSample_RecipeArmorDragonscaleCuirass()
    {
        var spec = ConstructibleObjectValidationSpecs.Skyrim_RecipeArmorDragonscaleCuirass();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Skyrim <c>RecipeArmorDragonscaleGauntlets</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "COBJ")]
    [Trait("FormKey", "0DCA15:Skyrim.esm")]
    [Trait("EditorID", "RecipeArmorDragonscaleGauntlets")]
    [Trait("SpriggitFile", "ConstructibleObjects/RecipeArmorDragonscaleGauntlets - 0DCA15_Skyrim.esm.yaml")]
    public void Skyrim_COBJ_ComparisonUi_ShouldRenderSpriggitSample_RecipeArmorDragonscaleGauntlets()
    {
        var spec = ConstructibleObjectValidationSpecs.Skyrim_RecipeArmorDragonscaleGauntlets();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Skyrim <c>RecipeArmorSteelPlateShield</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "COBJ")]
    [Trait("FormKey", "0DD982:Skyrim.esm")]
    [Trait("EditorID", "RecipeArmorSteelPlateShield")]
    [Trait("SpriggitFile", "ConstructibleObjects/RecipeArmorSteelPlateShield - 0DD982_Skyrim.esm.yaml")]
    public void Skyrim_COBJ_ComparisonUi_ShouldRenderSpriggitSample_RecipeArmorSteelPlateShield()
    {
        var spec = ConstructibleObjectValidationSpecs.Skyrim_RecipeArmorSteelPlateShield();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Skyrim <c>RecipeFoodSoupCabbagePotato</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "COBJ")]
    [Trait("FormKey", "0F431A:Skyrim.esm")]
    [Trait("EditorID", "RecipeFoodSoupCabbagePotato")]
    [Trait("SpriggitFile", "ConstructibleObjects/RecipeFoodSoupCabbagePotato - 0F431A_Skyrim.esm.yaml")]
    public void Skyrim_COBJ_ComparisonUi_ShouldRenderSpriggitSample_RecipeFoodSoupCabbagePotato()
    {
        var spec = ConstructibleObjectValidationSpecs.Skyrim_RecipeFoodSoupCabbagePotato();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }
}
