using Avalonia.Headless.XUnit;
using CreationsForge.DataValidationTests.Validation.Specs.Class;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.UI.Tests.Class;

/// <summary>
/// Validates class Spriggit samples against the rendered comparison UI.
/// </summary>
public class ClassSpriggitUiValidationTests :
    SpriggitComparisonUiTestBase,
    IClassFixture<SpriggitComparisonUiFixture>
{
    private readonly SpriggitComparisonUiFixture fixture;

    /// <summary>
    /// Initializes the UI validation tests with shared comparison fixture state.
    /// </summary>
    /// <param name="fixture">The fixture used to resolve imported records and comparison services.</param>
    public ClassSpriggitUiValidationTests(SpriggitComparisonUiFixture fixture)
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
    /// Validates the Fallout 4 <c>BloatflyClass</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "CLAS")]
    [Trait("FormKey", "031757:Fallout4.esm")]
    [Trait("EditorID", "BloatflyClass")]
    [Trait("SpriggitFile", "Classes/BloatflyClass - 031757_Fallout4.esm.yaml")]
    public void Fallout4_CLAS_ComparisonUi_ShouldRenderSpriggitSample_BloatflyClass()
    {
        var spec = ClassValidationSpecs.Fallout4_BloatflyClass();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Skyrim <c>TrainerAlchemyExpert</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "CLAS")]
    [Trait("FormKey", "0E3A6E:Skyrim.esm")]
    [Trait("EditorID", "TrainerAlchemyExpert")]
    [Trait("SpriggitFile", "Classes/TrainerAlchemyExpert - 0E3A6E_Skyrim.esm.yaml")]
    public void Skyrim_CLAS_ComparisonUi_ShouldRenderSpriggitSample_TrainerAlchemyExpert()
    {
        var spec = ClassValidationSpecs.Skyrim_TrainerAlchemyExpert();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }
}
