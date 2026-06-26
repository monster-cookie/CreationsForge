using Avalonia.Headless.XUnit;
using CreationsForge.DataValidationTests.Validation.Specs.Door;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.UI.Tests.Door.Starfield;

/// <summary>
/// Validates Starfield door Spriggit samples against the rendered comparison UI.
/// </summary>
public class StarfieldDoorSpriggitUiValidationTests :
    SpriggitComparisonUiTestBase,
    IClassFixture<SpriggitComparisonUiFixture>
{
    private readonly SpriggitComparisonUiFixture fixture;

    /// <summary>
    /// Initializes the UI validation tests with shared comparison fixture state.
    /// </summary>
    /// <param name="fixture">The fixture used to resolve imported records and comparison services.</param>
    public StarfieldDoorSpriggitUiValidationTests(SpriggitComparisonUiFixture fixture)
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
    /// Validates the Starfield <c>ShipDockingHatchFloor</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "DOOR")]
    [Trait("FormKey", "205AA6:Starfield.esm")]
    [Trait("EditorID", "ShipDockingHatchFloor")]
    [Trait("SpriggitFile", "Doors/ShipDockingHatchFloor - 205AA6_Starfield.esm.yaml")]
    public void Starfield_DOOR_ComparisonUi_ShouldRenderSpriggitSample_ShipDockingHatchFloor()
    {
        var spec = DoorValidationSpecs.Starfield_ShipDockingHatchFloor();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>SftIntRmSmWallMid_DoorA00</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "DOOR")]
    [Trait("FormKey", "19AFF6:Starfield.esm")]
    [Trait("EditorID", "SftIntRmSmWallMid_DoorA00")]
    [Trait("SpriggitFile", "Doors/SftIntRmSmWallMid_DoorA00 - 19AFF6_Starfield.esm.yaml")]
    public void Starfield_DOOR_ComparisonUi_ShouldRenderSpriggitSample_SftIntRmSmWallMid_DoorA00()
    {
        var spec = DoorValidationSpecs.Starfield_SftIntRmSmWallMid_DoorA00();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>SftIntRmSmWallMid_DoorA00_Loud</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "DOOR")]
    [Trait("FormKey", "30D813:Starfield.esm")]
    [Trait("EditorID", "SftIntRmSmWallMid_DoorA00_Loud")]
    [Trait("SpriggitFile", "Doors/SftIntRmSmWallMid_DoorA00_Loud - 30D813_Starfield.esm.yaml")]
    public void Starfield_DOOR_ComparisonUi_ShouldRenderSpriggitSample_SftIntRmSmWallMid_DoorA00_Loud()
    {
        var spec = DoorValidationSpecs.Starfield_SftIntRmSmWallMid_DoorA00_Loud();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>ShpGenIntPerSmWallMid_ExLg_DockingDoor02L_NonLoad</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "DOOR")]
    [Trait("FormKey", "31D042:Starfield.esm")]
    [Trait("EditorID", "ShpGenIntPerSmWallMid_ExLg_DockingDoor02L_NonLoad")]
    [Trait("SpriggitFile", "Doors/ShpGenIntPerSmWallMid_ExLg_DockingDoor02L_NonLoad - 31D042_Starfield.esm.yaml")]
    public void Starfield_DOOR_ComparisonUi_ShouldRenderSpriggitSample_ShpGenIntPerSmWallMid_ExLg_DockingDoor02L_NonLoad()
    {
        var spec = DoorValidationSpecs.Starfield_ShpGenIntPerSmWallMid_ExLg_DockingDoor02L_NonLoad();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }
}
