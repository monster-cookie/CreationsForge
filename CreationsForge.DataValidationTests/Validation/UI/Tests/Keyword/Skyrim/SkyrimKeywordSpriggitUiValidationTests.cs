using Avalonia.Headless.XUnit;
using CreationsForge.Specification.Validation.Specs.Keyword;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.UI.Tests.Keyword.Skyrim;

/// <summary>
/// Validates Skyrim keyword Spriggit samples against the rendered comparison UI.
/// </summary>
public class SkyrimKeywordSpriggitUiValidationTests :
    SpriggitComparisonUiTestBase,
    IClassFixture<SpriggitComparisonUiFixture>
{
    private readonly SpriggitComparisonUiFixture fixture;

    /// <summary>
    /// Initializes the UI validation tests with shared comparison fixture state.
    /// </summary>
    /// <param name="fixture">The fixture used to resolve imported records and comparison services.</param>
    public SkyrimKeywordSpriggitUiValidationTests(SpriggitComparisonUiFixture fixture)
    {
        this.fixture = fixture;
    }

    /// <summary>
    /// Validates the Skyrim <c>ActorTypeFamiliar</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "10EAD7:Skyrim.esm")]
    [Trait("EditorID", "ActorTypeFamiliar")]
    [Trait("SpriggitFile", "Keywords/ActorTypeFamiliar - 10EAD7_Skyrim.esm.yaml")]
    public void Skyrim_KYWD_ComparisonUi_ShouldRenderSpriggitSample_ActorTypeFamiliar()
    {
        var spec = KeywordValidationSpecs.Skyrim_ActorTypeFamiliar();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Skyrim <c>ActorTypeGiant</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "10E984:Skyrim.esm")]
    [Trait("EditorID", "ActorTypeGiant")]
    [Trait("SpriggitFile", "Keywords/ActorTypeGiant - 10E984_Skyrim.esm.yaml")]
    public void Skyrim_KYWD_ComparisonUi_ShouldRenderSpriggitSample_ActorTypeGiant()
    {
        var spec = KeywordValidationSpecs.Skyrim_ActorTypeGiant();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Skyrim <c>ActorTypeTroll</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "0F5D16:Skyrim.esm")]
    [Trait("EditorID", "ActorTypeTroll")]
    [Trait("SpriggitFile", "Keywords/ActorTypeTroll - 0F5D16_Skyrim.esm.yaml")]
    public void Skyrim_KYWD_ComparisonUi_ShouldRenderSpriggitSample_ActorTypeTroll()
    {
        var spec = KeywordValidationSpecs.Skyrim_ActorTypeTroll();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Skyrim <c>ActivatorLever</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "06DEAD:Skyrim.esm")]
    [Trait("EditorID", "ActivatorLever")]
    [Trait("SpriggitFile", "Keywords/ActivatorLever - 06DEAD_Skyrim.esm.yaml")]
    public void Skyrim_KYWD_ComparisonUi_ShouldRenderSpriggitSample_ActivatorLever()
    {
        var spec = KeywordValidationSpecs.Skyrim_ActivatorLever();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }
}
