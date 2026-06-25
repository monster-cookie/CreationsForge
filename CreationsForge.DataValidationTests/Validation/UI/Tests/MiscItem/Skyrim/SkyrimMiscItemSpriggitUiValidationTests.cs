using Avalonia.Headless.XUnit;
using CreationsForge.DataValidationTests.Validation.Specs.MiscItem;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.UI.Tests.MiscItem.Skyrim;

/// <summary>
/// Validates Skyrim misc item Spriggit samples against the rendered comparison UI.
/// </summary>
public class SkyrimMiscItemSpriggitUiValidationTests :
    SpriggitComparisonUiTestBase,
    IClassFixture<SpriggitComparisonUiFixture>
{
    private readonly SpriggitComparisonUiFixture fixture;

    /// <summary>
    /// Initializes the UI validation tests with shared comparison fixture state.
    /// </summary>
    /// <param name="fixture">The fixture used to resolve imported records and comparison services.</param>
    public SkyrimMiscItemSpriggitUiValidationTests(SpriggitComparisonUiFixture fixture)
    {
        this.fixture = fixture;
    }

    /// <summary>
    /// Validates the Skyrim <c>MGRDragonHeartScales</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "MISC")]
    [Trait("FormKey", "0D0756:Skyrim.esm")]
    [Trait("EditorID", "MGRDragonHeartScales")]
    [Trait("SpriggitFile", "MiscItems/MGRDragonHeartScales - 0D0756_Skyrim.esm.yaml")]
    public void Skyrim_MISC_ComparisonUi_ShouldRenderSpriggitSample_MGRDragonHeartScales()
    {
        var spec = MiscItemValidationSpecs.Skyrim_MGRDragonHeartScales();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Skyrim <c>Firewood01</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "MISC")]
    [Trait("FormKey", "06F993:Skyrim.esm")]
    [Trait("EditorID", "Firewood01")]
    [Trait("SpriggitFile", "MiscItems/Firewood01 - 06F993_Skyrim.esm.yaml")]
    public void Skyrim_MISC_ComparisonUi_ShouldRenderSpriggitSample_Firewood01()
    {
        var spec = MiscItemValidationSpecs.Skyrim_Firewood01();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Skyrim <c>FoxPeltSnow</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "MISC")]
    [Trait("FormKey", "0D4BE7:Skyrim.esm")]
    [Trait("EditorID", "FoxPeltSnow")]
    [Trait("SpriggitFile", "MiscItems/FoxPeltSnow - 0D4BE7_Skyrim.esm.yaml")]
    public void Skyrim_MISC_ComparisonUi_ShouldRenderSpriggitSample_FoxPeltSnow()
    {
        var spec = MiscItemValidationSpecs.Skyrim_FoxPeltSnow();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Skyrim <c>C04HagravenHead</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "MISC")]
    [Trait("FormKey", "02996F:Skyrim.esm")]
    [Trait("EditorID", "C04HagravenHead")]
    [Trait("SpriggitFile", "MiscItems/C04HagravenHead - 02996F_Skyrim.esm.yaml")]
    public void Skyrim_MISC_ComparisonUi_ShouldRenderSpriggitSample_C04HagravenHead()
    {
        var spec = MiscItemValidationSpecs.Skyrim_C04HagravenHead();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Skyrim <c>dunUniqueBeeInJar</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "MISC")]
    [Trait("FormKey", "0B08C7:Skyrim.esm")]
    [Trait("EditorID", "dunUniqueBeeInJar")]
    [Trait("SpriggitFile", "MiscItems/dunUniqueBeeInJar - 0B08C7_Skyrim.esm.yaml")]
    public void Skyrim_MISC_ComparisonUi_ShouldRenderSpriggitSample_dunUniqueBeeInJar()
    {
        var spec = MiscItemValidationSpecs.Skyrim_dunUniqueBeeInJar();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }
}
