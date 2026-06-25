using Avalonia.Headless.XUnit;
using CreationsForge.DataValidationTests.Validation.Specs.ConstructibleObject;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.UI.Tests.ConstructibleObject;

/// <summary>
/// Validates constructible object Spriggit samples against the rendered comparison UI.
/// </summary>
public class ConstructibleObjectSpriggitUiValidationTests :
    SpriggitComparisonUiTestBase,
    IClassFixture<SpriggitComparisonUiFixture>
{
    private readonly SpriggitComparisonUiFixture fixture;

    /// <summary>
    /// Initializes the UI validation tests with shared comparison fixture state.
    /// </summary>
    /// <param name="fixture">The fixture used to resolve imported records and comparison services.</param>
    public ConstructibleObjectSpriggitUiValidationTests(SpriggitComparisonUiFixture fixture)
    {
        this.fixture = fixture;
    }

    /// <summary>
    /// Validates the Starfield <c>co_Outpost_Power_Reactor01</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "COBJ")]
    [Trait("FormKey", "007F7C:Starfield.esm")]
    [Trait("EditorID", "co_Outpost_Power_Reactor01")]
    [Trait("SpriggitFile", "ConstructibleObjects/co_Outpost_Power_Reactor01 - 007F7C_Starfield.esm.yaml")]
    public void Starfield_COBJ_ComparisonUi_ShouldRenderSpriggitSample_co_Outpost_Power_Reactor01()
    {
        var spec = ConstructibleObjectValidationSpecs.Starfield_co_Outpost_Power_Reactor01();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout 4 <c>workshop_co_Artillery</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "COBJ")]
    [Trait("FormKey", "0ADF6E:Fallout4.esm")]
    [Trait("EditorID", "workshop_co_Artillery")]
    [Trait("SpriggitFile", "ConstructibleObjects/workshop_co_Artillery - 0ADF6E_Fallout4.esm.yaml")]
    public void Fallout4_COBJ_ComparisonUi_ShouldRenderSpriggitSample_workshop_co_Artillery()
    {
        var spec = ConstructibleObjectValidationSpecs.Fallout4_workshop_co_Artillery();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
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
}
