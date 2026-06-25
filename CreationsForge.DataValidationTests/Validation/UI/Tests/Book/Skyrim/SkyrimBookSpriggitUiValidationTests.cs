using Avalonia.Headless.XUnit;
using CreationsForge.DataValidationTests.Validation.Specs.Book;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.UI.Tests.Book.Skyrim;

/// <summary>
/// Validates Skyrim book Spriggit samples against the rendered comparison UI.
/// </summary>
public class SkyrimBookSpriggitUiValidationTests :
    SpriggitComparisonUiTestBase,
    IClassFixture<SpriggitComparisonUiFixture>
{
    private readonly SpriggitComparisonUiFixture fixture;

    /// <summary>
    /// Initializes the UI validation tests with shared comparison fixture state.
    /// </summary>
    /// <param name="fixture">The fixture used to resolve imported records and comparison services.</param>
    public SkyrimBookSpriggitUiValidationTests(SpriggitComparisonUiFixture fixture)
    {
        this.fixture = fixture;
    }

    /// <summary>
    /// Validates the Skyrim <c>AtrFrgDaedricRecipe00</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "BOOK")]
    [Trait("FormKey", "10F776:Skyrim.esm")]
    [Trait("EditorID", "AtrFrgDaedricRecipe00")]
    [Trait("SpriggitFile", "Books/AtrFrgDaedricRecipe00 - 10F776_Skyrim.esm.yaml")]
    public void Skyrim_BOOK_ComparisonUi_ShouldRenderSpriggitSample_AtrFrgDaedricRecipe00()
    {
        var spec = BookValidationSpecs.Skyrim_AtrFrgDaedricRecipe00();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Skyrim <c>Book0ArgonianAccountBook1</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "BOOK")]
    [Trait("FormKey", "01AFD7:Skyrim.esm")]
    [Trait("EditorID", "Book0ArgonianAccountBook1")]
    [Trait("SpriggitFile", "Books/Book0ArgonianAccountBook1 - 01AFD7_Skyrim.esm.yaml")]
    public void Skyrim_BOOK_ComparisonUi_ShouldRenderSpriggitSample_Book0ArgonianAccountBook1()
    {
        var spec = BookValidationSpecs.Skyrim_Book0ArgonianAccountBook1();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }
}
