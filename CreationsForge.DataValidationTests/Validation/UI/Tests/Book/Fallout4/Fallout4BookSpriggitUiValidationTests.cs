using Avalonia.Headless.XUnit;
using CreationsForge.DataValidationTests.Validation.Specs.Book;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.UI.Tests.Book.Fallout4;

/// <summary>
/// Validates Fallout4 book Spriggit samples against the rendered comparison UI.
/// </summary>
public class Fallout4BookSpriggitUiValidationTests :
    SpriggitComparisonUiTestBase,
    IClassFixture<SpriggitComparisonUiFixture>
{
    private readonly SpriggitComparisonUiFixture fixture;

    /// <summary>
    /// Initializes the UI validation tests with shared comparison fixture state.
    /// </summary>
    /// <param name="fixture">The fixture used to resolve imported records and comparison services.</param>
    public Fallout4BookSpriggitUiValidationTests(SpriggitComparisonUiFixture fixture)
    {
        this.fixture = fixture;
    }

    /// <summary>
    /// Validates the Fallout4 <c>BoS301ActuatorList</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "BOOK")]
    [Trait("FormKey", "02B4DF:Fallout4.esm")]
    [Trait("EditorID", "BoS301ActuatorList")]
    [Trait("SpriggitFile", "Books/BoS301ActuatorList - 02B4DF_Fallout4.esm.yaml")]
    public void Fallout4_BOOK_ComparisonUi_ShouldRenderSpriggitSample_BoS301ActuatorList()
    {
        var spec = BookValidationSpecs.Fallout4_BoS301ActuatorList();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>DN054PowerArmorPaintJobPurchaseItem</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "BOOK")]
    [Trait("FormKey", "23C675:Fallout4.esm")]
    [Trait("EditorID", "DN054PowerArmorPaintJobPurchaseItem")]
    [Trait("SpriggitFile", "Books/DN054PowerArmorPaintJobPurchaseItem - 23C675_Fallout4.esm.yaml")]
    public void Fallout4_BOOK_ComparisonUi_ShouldRenderSpriggitSample_DN054PowerArmorPaintJobPurchaseItem()
    {
        var spec = BookValidationSpecs.Fallout4_DN054PowerArmorPaintJobPurchaseItem();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>PerkMagGunsAndBullets07</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "BOOK")]
    [Trait("FormKey", "092A8C:Fallout4.esm")]
    [Trait("EditorID", "PerkMagGunsAndBullets07")]
    [Trait("SpriggitFile", "Books/PerkMagGunsAndBullets07 - 092A8C_Fallout4.esm.yaml")]
    public void Fallout4_BOOK_ComparisonUi_ShouldRenderSpriggitSample_PerkMagGunsAndBullets07()
    {
        var spec = BookValidationSpecs.Fallout4_PerkMagGunsAndBullets07();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }
}
