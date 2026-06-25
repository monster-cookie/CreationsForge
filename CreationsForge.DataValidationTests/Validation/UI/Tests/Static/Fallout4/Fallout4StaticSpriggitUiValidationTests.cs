using Avalonia.Headless.XUnit;
using CreationsForge.DataValidationTests.Validation.Specs.Static;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.UI.Tests.Static.Fallout4;

/// <summary>
/// Validates Fallout4 static Spriggit samples against the rendered comparison UI.
/// </summary>
public class Fallout4StaticSpriggitUiValidationTests :
    SpriggitComparisonUiTestBase,
    IClassFixture<SpriggitComparisonUiFixture>
{
    private readonly SpriggitComparisonUiFixture fixture;

    /// <summary>
    /// Initializes the UI validation tests with shared comparison fixture state.
    /// </summary>
    /// <param name="fixture">The fixture used to resolve imported records and comparison services.</param>
    public Fallout4StaticSpriggitUiValidationTests(SpriggitComparisonUiFixture fixture)
    {
        this.fixture = fixture;
    }

    /// <summary>
    /// Validates the Fallout4 <c>workshop_JunkWallDoor01</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "STAT")]
    [Trait("FormKey", "1B4AC0:Fallout4.esm")]
    [Trait("EditorID", "workshop_JunkWallDoor01")]
    [Trait("SpriggitFile", "Statics/workshop_JunkWallDoor01 - 1B4AC0_Fallout4.esm.yaml")]
    public void Fallout4_STAT_ComparisonUi_ShouldRenderSpriggitSample_workshop_JunkWallDoor01()
    {
        var spec = StaticValidationSpecs.Fallout4_workshop_JunkWallDoor01();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>workshop_JunkWallDoor01A</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "STAT")]
    [Trait("FormKey", "1B4AC1:Fallout4.esm")]
    [Trait("EditorID", "workshop_JunkWallDoor01A")]
    [Trait("SpriggitFile", "Statics/workshop_JunkWallDoor01A - 1B4AC1_Fallout4.esm.yaml")]
    public void Fallout4_STAT_ComparisonUi_ShouldRenderSpriggitSample_workshop_JunkWallDoor01A()
    {
        var spec = StaticValidationSpecs.Fallout4_workshop_JunkWallDoor01A();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>workshop_ShackBalconyStairs01</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "STAT")]
    [Trait("FormKey", "0EC532:Fallout4.esm")]
    [Trait("EditorID", "workshop_ShackBalconyStairs01")]
    [Trait("SpriggitFile", "Statics/workshop_ShackBalconyStairs01 - 0EC532_Fallout4.esm.yaml")]
    public void Fallout4_STAT_ComparisonUi_ShouldRenderSpriggitSample_workshop_ShackBalconyStairs01()
    {
        var spec = StaticValidationSpecs.Fallout4_workshop_ShackBalconyStairs01();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>COCMarkerHeading</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "STAT")]
    [Trait("FormKey", "000032:Fallout4.esm")]
    [Trait("EditorID", "COCMarkerHeading")]
    [Trait("SpriggitFile", "Statics/COCMarkerHeading - 000032_Fallout4.esm.yaml")]
    public void Fallout4_STAT_ComparisonUi_ShouldRenderSpriggitSample_COCMarkerHeading()
    {
        var spec = StaticValidationSpecs.Fallout4_COCMarkerHeading();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>CollisionMarker</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "STAT")]
    [Trait("FormKey", "000021:Fallout4.esm")]
    [Trait("EditorID", "CollisionMarker")]
    [Trait("SpriggitFile", "Statics/CollisionMarker - 000021_Fallout4.esm.yaml")]
    public void Fallout4_STAT_ComparisonUi_ShouldRenderSpriggitSample_CollisionMarker()
    {
        var spec = StaticValidationSpecs.Fallout4_CollisionMarker();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }
}
