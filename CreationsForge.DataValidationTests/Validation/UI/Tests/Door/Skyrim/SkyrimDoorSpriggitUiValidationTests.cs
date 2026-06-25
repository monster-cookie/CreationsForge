using Avalonia.Headless.XUnit;
using CreationsForge.DataValidationTests.Validation.Specs.Door;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.UI.Tests.Door.Skyrim;

/// <summary>
/// Validates Skyrim door Spriggit samples against the rendered comparison UI.
/// </summary>
public class SkyrimDoorSpriggitUiValidationTests :
    SpriggitComparisonUiTestBase,
    IClassFixture<SpriggitComparisonUiFixture>
{
    private readonly SpriggitComparisonUiFixture fixture;

    /// <summary>
    /// Initializes the UI validation tests with shared comparison fixture state.
    /// </summary>
    /// <param name="fixture">The fixture used to resolve imported records and comparison services.</param>
    public SkyrimDoorSpriggitUiValidationTests(SpriggitComparisonUiFixture fixture)
    {
        this.fixture = fixture;
    }

    /// <summary>
    /// Validates the Skyrim <c>AutoLoadDoor01</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "DOOR")]
    [Trait("FormKey", "031897:Skyrim.esm")]
    [Trait("EditorID", "AutoLoadDoor01")]
    [Trait("SpriggitFile", "Doors/AutoLoadDoor01 - 031897_Skyrim.esm.yaml")]
    public void Skyrim_DOOR_ComparisonUi_ShouldRenderSpriggitSample_AutoLoadDoor01()
    {
        var spec = DoorValidationSpecs.Skyrim_AutoLoadDoor01();
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
