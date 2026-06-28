using Avalonia.Headless.XUnit;
using CreationsForge.Specification.Validation.Specs.Class;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.UI.Tests.Class.Starfield;

/// <summary>
/// Validates Starfield class Spriggit samples against the rendered comparison UI.
/// </summary>
public class StarfieldClassSpriggitUiValidationTests :
    SpriggitComparisonUiTestBase,
    IClassFixture<SpriggitComparisonUiFixture>
{
    private readonly SpriggitComparisonUiFixture fixture;

    /// <summary>
    /// Initializes the UI validation tests with shared comparison fixture state.
    /// </summary>
    /// <param name="fixture">The fixture used to resolve imported records and comparison services.</param>
    public StarfieldClassSpriggitUiValidationTests(SpriggitComparisonUiFixture fixture)
    {
        this.fixture = fixture;
    }

    /// <summary>
    /// Validates the Starfield <c>Citizen</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "CLAS")]
    [Trait("FormKey", "01326B:Starfield.esm")]
    [Trait("EditorID", "Citizen")]
    [Trait("SpriggitFile", "Classes/Citizen - 01326B_Starfield.esm.yaml")]
    public void Starfield_CLAS_ComparisonUi_ShouldRenderSpriggitSample_Citizen()
    {
        var spec = ClassValidationSpecs.Starfield_Citizen();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>CourserClass</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "CLAS")]
    [Trait("FormKey", "20F487:Starfield.esm")]
    [Trait("EditorID", "CourserClass")]
    [Trait("SpriggitFile", "Classes/CourserClass - 20F487_Starfield.esm.yaml")]
    public void Starfield_CLAS_ComparisonUi_ShouldRenderSpriggitSample_CourserClass()
    {
        var spec = ClassValidationSpecs.Starfield_CourserClass();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>CrimsonFleetClass</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "CLAS")]
    [Trait("FormKey", "010B2F:Starfield.esm")]
    [Trait("EditorID", "CrimsonFleetClass")]
    [Trait("SpriggitFile", "Classes/CrimsonFleetClass - 010B2F_Starfield.esm.yaml")]
    public void Starfield_CLAS_ComparisonUi_ShouldRenderSpriggitSample_CrimsonFleetClass()
    {
        var spec = ClassValidationSpecs.Starfield_CrimsonFleetClass();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }
}
