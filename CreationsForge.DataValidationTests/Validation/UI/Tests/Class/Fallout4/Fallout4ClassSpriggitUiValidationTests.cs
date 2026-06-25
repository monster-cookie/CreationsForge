using Avalonia.Headless.XUnit;
using CreationsForge.DataValidationTests.Validation.Specs.Class;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.UI.Tests.Class.Fallout4;

/// <summary>
/// Validates Fallout4 class Spriggit samples against the rendered comparison UI.
/// </summary>
public class Fallout4ClassSpriggitUiValidationTests :
    SpriggitComparisonUiTestBase,
    IClassFixture<SpriggitComparisonUiFixture>
{
    private readonly SpriggitComparisonUiFixture fixture;

    /// <summary>
    /// Initializes the UI validation tests with shared comparison fixture state.
    /// </summary>
    /// <param name="fixture">The fixture used to resolve imported records and comparison services.</param>
    public Fallout4ClassSpriggitUiValidationTests(SpriggitComparisonUiFixture fixture)
    {
        this.fixture = fixture;
    }

    /// <summary>
    /// Validates the Fallout4 <c>ZeroSPECIALclass</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "CLAS")]
    [Trait("FormKey", "1CD0A8:Fallout4.esm")]
    [Trait("EditorID", "ZeroSPECIALclass")]
    [Trait("SpriggitFile", "Classes/ZeroSPECIALclass - 1CD0A8_Fallout4.esm.yaml")]
    public void Fallout4_CLAS_ComparisonUi_ShouldRenderSpriggitSample_ZeroSPECIALclass()
    {
        var spec = ClassValidationSpecs.Fallout4_ZeroSPECIALclass();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>Citizen</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "CLAS")]
    [Trait("FormKey", "01326B:Fallout4.esm")]
    [Trait("EditorID", "Citizen")]
    [Trait("SpriggitFile", "Classes/Citizen - 01326B_Fallout4.esm.yaml")]
    public void Fallout4_CLAS_ComparisonUi_ShouldRenderSpriggitSample_Citizen()
    {
        var spec = ClassValidationSpecs.Fallout4_Citizen();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>BloatflyClass</c> sample against rendered comparison rows.
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
    /// Validates the Fallout4 <c>MQ203Class</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "CLAS")]
    [Trait("FormKey", "20ED07:Fallout4.esm")]
    [Trait("EditorID", "MQ203Class")]
    [Trait("SpriggitFile", "Classes/MQ203Class - 20ED07_Fallout4.esm.yaml")]
    public void Fallout4_CLAS_ComparisonUi_ShouldRenderSpriggitSample_MQ203Class()
    {
        var spec = ClassValidationSpecs.Fallout4_MQ203Class();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }
}
