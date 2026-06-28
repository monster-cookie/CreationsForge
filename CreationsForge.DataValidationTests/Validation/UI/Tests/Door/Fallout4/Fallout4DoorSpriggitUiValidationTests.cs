using Avalonia.Headless.XUnit;
using CreationsForge.Specification.Validation.Specs.Door;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.UI.Tests.Door.Fallout4;

/// <summary>
/// Validates Fallout4 door Spriggit samples against the rendered comparison UI.
/// </summary>
public class Fallout4DoorSpriggitUiValidationTests :
    SpriggitComparisonUiTestBase,
    IClassFixture<SpriggitComparisonUiFixture>
{
    private readonly SpriggitComparisonUiFixture fixture;

    /// <summary>
    /// Initializes the UI validation tests with shared comparison fixture state.
    /// </summary>
    /// <param name="fixture">The fixture used to resolve imported records and comparison services.</param>
    public Fallout4DoorSpriggitUiValidationTests(SpriggitComparisonUiFixture fixture)
    {
        this.fixture = fixture;
    }

    /// <summary>
    /// Validates the Fallout4 <c>AutoloadDoor</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "DOOR")]
    [Trait("FormKey", "01ED77:Fallout4.esm")]
    [Trait("EditorID", "AutoloadDoor")]
    [Trait("SpriggitFile", "Doors/AutoloadDoor - 01ED77_Fallout4.esm.yaml")]
    public void Fallout4_DOOR_ComparisonUi_ShouldRenderSpriggitSample_AutoloadDoor()
    {
        var spec = DoorValidationSpecs.Fallout4_AutoloadDoor();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>BldWoodPDbDoor01</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "DOOR")]
    [Trait("FormKey", "01D930:Fallout4.esm")]
    [Trait("EditorID", "BldWoodPDbDoor01")]
    [Trait("SpriggitFile", "Doors/BldWoodPDbDoor01 - 01D930_Fallout4.esm.yaml")]
    public void Fallout4_DOOR_ComparisonUi_ShouldRenderSpriggitSample_BldWoodPDbDoor01()
    {
        var spec = DoorValidationSpecs.Fallout4_BldWoodPDbDoor01();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }
}
