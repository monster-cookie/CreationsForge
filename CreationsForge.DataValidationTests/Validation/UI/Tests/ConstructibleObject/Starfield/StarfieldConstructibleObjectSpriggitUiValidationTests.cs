using Avalonia.Headless.XUnit;
using CreationsForge.DataValidationTests.Validation.Specs.ConstructibleObject;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.UI.Tests.ConstructibleObject.Starfield;

/// <summary>
/// Validates Starfield constructible object Spriggit samples against the rendered comparison UI.
/// </summary>
public class StarfieldConstructibleObjectSpriggitUiValidationTests :
    SpriggitComparisonUiTestBase,
    IClassFixture<SpriggitComparisonUiFixture>
{
    private readonly SpriggitComparisonUiFixture fixture;

    /// <summary>
    /// Initializes the UI validation tests with shared comparison fixture state.
    /// </summary>
    /// <param name="fixture">The fixture used to resolve imported records and comparison services.</param>
    public StarfieldConstructibleObjectSpriggitUiValidationTests(SpriggitComparisonUiFixture fixture)
    {
        this.fixture = fixture;
    }

    /// <summary>
    /// Validates the Starfield <c>co_Outpost_Power_Reactor01</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "COBJ")]
    [Trait("FormKey", "007F7C:Starfield.esm")]
    [Trait("EditorID", "co_Outpost_Power_Reactor01")]
    [Trait("SpriggitFile", "ConstructibleObjects/co_Outpost_Power_Reactor01 - 007F7C_Starfield.esm.yaml")]
    public void Starfield_COBJ_ComparisonUi_ShouldRenderSpriggitSample_co_Outpost_Power_Reactor01()
    {
        var spec = ConstructibleObjectValidationSpecs.Starfield_co_Outpost_Power_Reactor01();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>co_Outpost_Power_Reactor02</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "COBJ")]
    [Trait("FormKey", "1C5144:Starfield.esm")]
    [Trait("EditorID", "co_Outpost_Power_Reactor02")]
    [Trait("SpriggitFile", "ConstructibleObjects/co_Outpost_Power_Reactor02 - 1C5144_Starfield.esm.yaml")]
    public void Starfield_COBJ_ComparisonUi_ShouldRenderSpriggitSample_co_Outpost_Power_Reactor02()
    {
        var spec = ConstructibleObjectValidationSpecs.Starfield_co_Outpost_Power_Reactor02();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>co_Chem_XenoAurora</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "COBJ")]
    [Trait("FormKey", "0C8720:Starfield.esm")]
    [Trait("EditorID", "co_Chem_XenoAurora")]
    [Trait("SpriggitFile", "ConstructibleObjects/co_Chem_XenoAurora - 0C8720_Starfield.esm.yaml")]
    public void Starfield_COBJ_ComparisonUi_ShouldRenderSpriggitSample_co_Chem_XenoAurora()
    {
        var spec = ConstructibleObjectValidationSpecs.Starfield_co_Chem_XenoAurora();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>UC07_co_mfg_MicroCell_Old</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "COBJ")]
    [Trait("FormKey", "09DE67:Starfield.esm")]
    [Trait("EditorID", "UC07_co_mfg_MicroCell_Old")]
    [Trait("SpriggitFile", "ConstructibleObjects/UC07_co_mfg_MicroCell_Old - 09DE67_Starfield.esm.yaml")]
    public void Starfield_COBJ_ComparisonUi_ShouldRenderSpriggitSample_UC07_co_mfg_MicroCell_Old()
    {
        var spec = ConstructibleObjectValidationSpecs.Starfield_UC07_co_mfg_MicroCell_Old();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>co_Outpost_Misc_MissionBoardConsole</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "COBJ")]
    [Trait("FormKey", "1DF844:Starfield.esm")]
    [Trait("EditorID", "co_Outpost_Misc_MissionBoardConsole")]
    [Trait("SpriggitFile", "ConstructibleObjects/co_Outpost_Misc_MissionBoardConsole - 1DF844_Starfield.esm.yaml")]
    public void Starfield_COBJ_ComparisonUi_ShouldRenderSpriggitSample_co_Outpost_Misc_MissionBoardConsole()
    {
        var spec = ConstructibleObjectValidationSpecs.Starfield_co_Outpost_Misc_MissionBoardConsole();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }
}
