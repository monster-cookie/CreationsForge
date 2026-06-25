using Avalonia.Headless.XUnit;
using CreationsForge.DataValidationTests.Validation.Specs.FormList;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.UI.Tests.FormList.Skyrim;

/// <summary>
/// Validates Skyrim form list Spriggit samples against the rendered comparison UI.
/// </summary>
public class SkyrimFormListSpriggitUiValidationTests :
    SpriggitComparisonUiTestBase,
    IClassFixture<SpriggitComparisonUiFixture>
{
    private readonly SpriggitComparisonUiFixture fixture;

    /// <summary>
    /// Initializes the UI validation tests with shared comparison fixture state.
    /// </summary>
    /// <param name="fixture">The fixture used to resolve imported records and comparison services.</param>
    public SkyrimFormListSpriggitUiValidationTests(SpriggitComparisonUiFixture fixture)
    {
        this.fixture = fixture;
    }

    /// <summary>
    /// Validates the Skyrim <c>AAAMothPlantTypes</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "FLST")]
    [Trait("FormKey", "06F3F7:Skyrim.esm")]
    [Trait("EditorID", "AAAMothPlantTypes")]
    [Trait("SpriggitFile", "FormLists/AAAMothPlantTypes - 06F3F7_Skyrim.esm.yaml")]
    public void Skyrim_FLST_ComparisonUi_ShouldRenderSpriggitSample_AAAMothPlantTypes()
    {
        var spec = FormListValidationSpecs.Skyrim_AAAMothPlantTypes();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Skyrim <c>CityWindhelmResidentList</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "FLST")]
    [Trait("FormKey", "045C32:Skyrim.esm")]
    [Trait("EditorID", "CityWindhelmResidentList")]
    [Trait("SpriggitFile", "FormLists/CityWindhelmResidentList - 045C32_Skyrim.esm.yaml")]
    public void Skyrim_FLST_ComparisonUi_ShouldRenderSpriggitSample_CityWindhelmResidentList()
    {
        var spec = FormListValidationSpecs.Skyrim_CityWindhelmResidentList();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Skyrim <c>CrimeFactionsList</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "FLST")]
    [Trait("FormKey", "026953:Skyrim.esm")]
    [Trait("EditorID", "CrimeFactionsList")]
    [Trait("SpriggitFile", "FormLists/CrimeFactionsList - 026953_Skyrim.esm.yaml")]
    public void Skyrim_FLST_ComparisonUi_ShouldRenderSpriggitSample_CrimeFactionsList()
    {
        var spec = FormListValidationSpecs.Skyrim_CrimeFactionsList();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Skyrim <c>DraugrWeapons</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "FLST")]
    [Trait("FormKey", "000D14:Skyrim.esm")]
    [Trait("EditorID", "DraugrWeapons")]
    [Trait("SpriggitFile", "FormLists/DraugrWeapons - 000D14_Skyrim.esm.yaml")]
    public void Skyrim_FLST_ComparisonUi_ShouldRenderSpriggitSample_DraugrWeapons()
    {
        var spec = FormListValidationSpecs.Skyrim_DraugrWeapons();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }
}
