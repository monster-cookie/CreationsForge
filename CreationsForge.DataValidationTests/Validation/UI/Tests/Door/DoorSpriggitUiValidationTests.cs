using Avalonia.Headless.XUnit;
using CreationsForge.DataValidationTests.Validation.Specs.Door;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.UI.Tests.Door;

/// <summary>
/// Validates door Spriggit samples against the rendered comparison UI.
/// </summary>
public class DoorSpriggitUiValidationTests :
    SpriggitComparisonUiTestBase,
    IClassFixture<SpriggitComparisonUiFixture>
{
    private readonly SpriggitComparisonUiFixture fixture;

    /// <summary>
    /// Initializes the UI validation tests with shared comparison fixture state.
    /// </summary>
    /// <param name="fixture">The fixture used to resolve imported records and comparison services.</param>
    public DoorSpriggitUiValidationTests(SpriggitComparisonUiFixture fixture)
    {
        this.fixture = fixture;
    }

    /// <summary>
    /// Validates the Starfield <c>ShipFloorLoadHatch</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "DOOR")]
    [Trait("FormKey", "144F85:Starfield.esm")]
    [Trait("EditorID", "ShipFloorLoadHatch")]
    [Trait("SpriggitFile", "Doors/ShipFloorLoadHatch - 144F85_Starfield.esm.yaml")]
    public void Starfield_DOOR_ComparisonUi_ShouldRenderSpriggitSample_ShipFloorLoadHatch()
    {
        var spec = DoorValidationSpecs.Starfield_ShipFloorLoadHatch();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout 4 <c>AutoloadDoor</c> sample against rendered comparison rows.
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
    /// Validates the Skyrim <c>DBBlackDoor</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "DOOR")]
    [Trait("FormKey", "022F44:Skyrim.esm")]
    [Trait("EditorID", "DBBlackDoor")]
    [Trait("SpriggitFile", "Doors/DBBlackDoor - 022F44_Skyrim.esm.yaml")]
    public void Skyrim_DOOR_ComparisonUi_ShouldRenderSpriggitSample_DBBlackDoor()
    {
        var spec = DoorValidationSpecs.Skyrim_DBBlackDoor();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }
}
