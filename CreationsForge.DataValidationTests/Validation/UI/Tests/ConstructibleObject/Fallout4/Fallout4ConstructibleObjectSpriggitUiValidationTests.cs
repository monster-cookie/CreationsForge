using Avalonia.Headless.XUnit;
using CreationsForge.DataValidationTests.Validation.Specs.ConstructibleObject;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.UI.Tests.ConstructibleObject.Fallout4;

/// <summary>
/// Validates Fallout4 constructible object Spriggit samples against the rendered comparison UI.
/// </summary>
public class Fallout4ConstructibleObjectSpriggitUiValidationTests :
    SpriggitComparisonUiTestBase,
    IClassFixture<SpriggitComparisonUiFixture>
{
    private readonly SpriggitComparisonUiFixture fixture;

    /// <summary>
    /// Initializes the UI validation tests with shared comparison fixture state.
    /// </summary>
    /// <param name="fixture">The fixture used to resolve imported records and comparison services.</param>
    public Fallout4ConstructibleObjectSpriggitUiValidationTests(SpriggitComparisonUiFixture fixture)
    {
        this.fixture = fixture;
    }

    /// <summary>
    /// Validates the Fallout4 <c>workshop_co_Artillery</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "COBJ")]
    [Trait("FormKey", "0ADF6E:Fallout4.esm")]
    [Trait("EditorID", "workshop_co_Artillery")]
    [Trait("SpriggitFile", "ConstructibleObjects/workshop_co_Artillery - 0ADF6E_Fallout4.esm.yaml")]
    public void Fallout4_COBJ_ComparisonUi_ShouldRenderSpriggitSample_workshop_co_Artillery()
    {
        var spec = ConstructibleObjectValidationSpecs.Fallout4_workshop_co_Artillery();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>workshop_co_MQ206BeamEmitter</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "COBJ")]
    [Trait("FormKey", "0CEA6F:Fallout4.esm")]
    [Trait("EditorID", "workshop_co_MQ206BeamEmitter")]
    [Trait("SpriggitFile", "ConstructibleObjects/workshop_co_MQ206BeamEmitter - 0CEA6F_Fallout4.esm.yaml")]
    public void Fallout4_COBJ_ComparisonUi_ShouldRenderSpriggitSample_workshop_co_MQ206BeamEmitter()
    {
        var spec = ConstructibleObjectValidationSpecs.Fallout4_workshop_co_MQ206BeamEmitter();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>workshop_co_MQ206Console</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "COBJ")]
    [Trait("FormKey", "0CEA7B:Fallout4.esm")]
    [Trait("EditorID", "workshop_co_MQ206Console")]
    [Trait("SpriggitFile", "ConstructibleObjects/workshop_co_MQ206Console - 0CEA7B_Fallout4.esm.yaml")]
    public void Fallout4_COBJ_ComparisonUi_ShouldRenderSpriggitSample_workshop_co_MQ206Console()
    {
        var spec = ConstructibleObjectValidationSpecs.Fallout4_workshop_co_MQ206Console();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>workshop_co_WaterPurifier</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "COBJ")]
    [Trait("FormKey", "05A0CD:Fallout4.esm")]
    [Trait("EditorID", "workshop_co_WaterPurifier")]
    [Trait("SpriggitFile", "ConstructibleObjects/workshop_co_WaterPurifier - 05A0CD_Fallout4.esm.yaml")]
    public void Fallout4_COBJ_ComparisonUi_ShouldRenderSpriggitSample_workshop_co_WaterPurifier()
    {
        var spec = ConstructibleObjectValidationSpecs.Fallout4_workshop_co_WaterPurifier();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>co_mod_GatlingLaser_BarrelMingunLaser_Super</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "COBJ")]
    [Trait("FormKey", "1889E3:Fallout4.esm")]
    [Trait("EditorID", "co_mod_GatlingLaser_BarrelMingunLaser_Super")]
    [Trait("SpriggitFile", "ConstructibleObjects/co_mod_GatlingLaser_BarrelMingunLaser_Super - 1889E3_Fallout4.esm.yaml")]
    public void Fallout4_COBJ_ComparisonUi_ShouldRenderSpriggitSample_co_mod_GatlingLaser_BarrelMingunLaser_Super()
    {
        var spec = ConstructibleObjectValidationSpecs.Fallout4_co_mod_GatlingLaser_BarrelMingunLaser_Super();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }
}
