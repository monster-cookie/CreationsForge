using Avalonia.Headless.XUnit;
using CreationsForge.DataValidationTests.Validation.Specs.Book;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.UI.Tests.Book.Starfield;

/// <summary>
/// Validates Starfield book Spriggit samples against the rendered comparison UI.
/// </summary>
public class StarfieldBookSpriggitUiValidationTests :
    SpriggitComparisonUiTestBase,
    IClassFixture<SpriggitComparisonUiFixture>
{
    private readonly SpriggitComparisonUiFixture fixture;

    /// <summary>
    /// Initializes the UI validation tests with shared comparison fixture state.
    /// </summary>
    /// <param name="fixture">The fixture used to resolve imported records and comparison services.</param>
    public StarfieldBookSpriggitUiValidationTests(SpriggitComparisonUiFixture fixture)
    {
        this.fixture = fixture;
    }

    /// <summary>
    /// Validates the Starfield <c>NH_SouvenirSlate</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "BOOK")]
    [Trait("FormKey", "165BF3:Starfield.esm")]
    [Trait("EditorID", "NH_SouvenirSlate")]
    [Trait("SpriggitFile", "Books/NH_SouvenirSlate - 165BF3_Starfield.esm.yaml")]
    public void Starfield_BOOK_ComparisonUi_ShouldRenderSpriggitSample_NH_SouvenirSlate()
    {
        var spec = BookValidationSpecs.Starfield_NH_SouvenirSlate();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>UC07_ScrappingNiira</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "BOOK")]
    [Trait("FormKey", "1F40EE:Starfield.esm")]
    [Trait("EditorID", "UC07_ScrappingNiira")]
    [Trait("SpriggitFile", "Books/UC07_ScrappingNiira - 1F40EE_Starfield.esm.yaml")]
    public void Starfield_BOOK_ComparisonUi_ShouldRenderSpriggitSample_UC07_ScrappingNiira()
    {
        var spec = BookValidationSpecs.Starfield_UC07_ScrappingNiira();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>SQ_PlanetSurveySlate00_025</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "BOOK")]
    [Trait("FormKey", "26E6B1:Starfield.esm")]
    [Trait("EditorID", "SQ_PlanetSurveySlate00_025")]
    [Trait("SpriggitFile", "Books/SQ_PlanetSurveySlate00_025 - 26E6B1_Starfield.esm.yaml")]
    public void Starfield_BOOK_ComparisonUi_ShouldRenderSpriggitSample_SQ_PlanetSurveySlate00_025()
    {
        var spec = BookValidationSpecs.Starfield_SQ_PlanetSurveySlate00_025();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>_RENAME_TestDataslate</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "BOOK")]
    [Trait("FormKey", "070510:Starfield.esm")]
    [Trait("EditorID", "_RENAME_TestDataslate")]
    [Trait("SpriggitFile", "Books/_RENAME_TestDataslate - 070510_Starfield.esm.yaml")]
    public void Starfield_BOOK_ComparisonUi_ShouldRenderSpriggitSample_RENAME_TestDataslate()
    {
        var spec = BookValidationSpecs.Starfield_RENAME_TestDataslate();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>TreasureMap_Resource_AnySystem_Unique_Aldumite</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "BOOK")]
    [Trait("FormKey", "045631:Starfield.esm")]
    [Trait("EditorID", "TreasureMap_Resource_AnySystem_Unique_Aldumite")]
    [Trait("SpriggitFile", "Books/TreasureMap_Resource_AnySystem_Unique_Aldumite - 045631_Starfield.esm.yaml")]
    public void Starfield_BOOK_ComparisonUi_ShouldRenderSpriggitSample_TreasureMap_Resource_AnySystem_Unique_Aldumite()
    {
        var spec = BookValidationSpecs.Starfield_TreasureMap_Resource_AnySystem_Unique_Aldumite();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }
}
