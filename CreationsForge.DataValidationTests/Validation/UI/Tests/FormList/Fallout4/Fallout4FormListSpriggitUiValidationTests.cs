using Avalonia.Headless.XUnit;
using CreationsForge.DataValidationTests.Validation.Specs.FormList;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.UI.Tests.FormList.Fallout4;

/// <summary>
/// Validates Fallout4 form list Spriggit samples against the rendered comparison UI.
/// </summary>
public class Fallout4FormListSpriggitUiValidationTests :
    SpriggitComparisonUiTestBase,
    IClassFixture<SpriggitComparisonUiFixture>
{
    private readonly SpriggitComparisonUiFixture fixture;

    /// <summary>
    /// Initializes the UI validation tests with shared comparison fixture state.
    /// </summary>
    /// <param name="fixture">The fixture used to resolve imported records and comparison services.</param>
    public Fallout4FormListSpriggitUiValidationTests(SpriggitComparisonUiFixture fixture)
    {
        this.fixture = fixture;
    }

    /// <summary>
    /// Validates the Fallout4 <c>CA_JunkItems</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "FLST")]
    [Trait("FormKey", "246EE7:Fallout4.esm")]
    [Trait("EditorID", "CA_JunkItems")]
    [Trait("SpriggitFile", "FormLists/CA_JunkItems - 246EE7_Fallout4.esm.yaml")]
    public void Fallout4_FLST_ComparisonUi_ShouldRenderSpriggitSample_CA_JunkItems()
    {
        var spec = FormListValidationSpecs.Fallout4_CA_JunkItems();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>ChargenOptionsSortList</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "FLST")]
    [Trait("FormKey", "1A4AE8:Fallout4.esm")]
    [Trait("EditorID", "ChargenOptionsSortList")]
    [Trait("SpriggitFile", "FormLists/ChargenOptionsSortList - 1A4AE8_Fallout4.esm.yaml")]
    public void Fallout4_FLST_ComparisonUi_ShouldRenderSpriggitSample_ChargenOptionsSortList()
    {
        var spec = FormListValidationSpecs.Fallout4_ChargenOptionsSortList();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>CompanionCrime__Common</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "FLST")]
    [Trait("FormKey", "2494E7:Fallout4.esm")]
    [Trait("EditorID", "CompanionCrime__Common")]
    [Trait("SpriggitFile", "FormLists/CompanionCrime__Common - 2494E7_Fallout4.esm.yaml")]
    public void Fallout4_FLST_ComparisonUi_ShouldRenderSpriggitSample_CompanionCrime__Common()
    {
        var spec = FormListValidationSpecs.Fallout4_CompanionCrime__Common();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>VoicesEmpty</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "FLST")]
    [Trait("FormKey", "14EC02:Fallout4.esm")]
    [Trait("EditorID", "VoicesEmpty")]
    [Trait("SpriggitFile", "FormLists/VoicesEmpty - 14EC02_Fallout4.esm.yaml")]
    public void Fallout4_FLST_ComparisonUi_ShouldRenderSpriggitSample_VoicesEmpty()
    {
        var spec = FormListValidationSpecs.Fallout4_VoicesEmpty();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }
}
