using Avalonia.Headless.XUnit;
using CreationsForge.DataValidationTests.Validation.Specs.Static;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.UI.Tests.Static.Skyrim;

/// <summary>
/// Validates Skyrim static Spriggit samples against the rendered comparison UI.
/// </summary>
public class SkyrimStaticSpriggitUiValidationTests :
    SpriggitComparisonUiTestBase,
    IClassFixture<SpriggitComparisonUiFixture>
{
    private readonly SpriggitComparisonUiFixture fixture;

    /// <summary>
    /// Initializes the UI validation tests with shared comparison fixture state.
    /// </summary>
    /// <param name="fixture">The fixture used to resolve imported records and comparison services.</param>
    public SkyrimStaticSpriggitUiValidationTests(SpriggitComparisonUiFixture fixture)
    {
        this.fixture = fixture;
    }

    /// <summary>
    /// Validates the Skyrim <c>BlackreachECeiling01_GlowLichen</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "STAT")]
    [Trait("FormKey", "0D19F9:Skyrim.esm")]
    [Trait("EditorID", "BlackreachECeiling01_GlowLichen")]
    [Trait("SpriggitFile", "Statics/BlackreachECeiling01_GlowLichen - 0D19F9_Skyrim.esm.yaml")]
    public void Skyrim_STAT_ComparisonUi_ShouldRenderSpriggitSample_BlackreachECeiling01_GlowLichen()
    {
        var spec = StaticValidationSpecs.Skyrim_BlackreachECeiling01_GlowLichen();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Skyrim <c>DweFacadeTowerSpacer01Snow</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "STAT")]
    [Trait("FormKey", "06DD69:Skyrim.esm")]
    [Trait("EditorID", "DweFacadeTowerSpacer01Snow")]
    [Trait("SpriggitFile", "Statics/DweFacadeTowerSpacer01Snow - 06DD69_Skyrim.esm.yaml")]
    public void Skyrim_STAT_ComparisonUi_ShouldRenderSpriggitSample_DweFacadeTowerSpacer01Snow()
    {
        var spec = StaticValidationSpecs.Skyrim_DweFacadeTowerSpacer01Snow();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Skyrim <c>HHMountainRidge01</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "STAT")]
    [Trait("FormKey", "090E82:Skyrim.esm")]
    [Trait("EditorID", "HHMountainRidge01")]
    [Trait("SpriggitFile", "Statics/HHMountainRidge01 - 090E82_Skyrim.esm.yaml")]
    public void Skyrim_STAT_ComparisonUi_ShouldRenderSpriggitSample_HHMountainRidge01()
    {
        var spec = StaticValidationSpecs.Skyrim_HHMountainRidge01();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Skyrim <c>CaveGRockPileS01IceBlend</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "STAT")]
    [Trait("FormKey", "0946B2:Skyrim.esm")]
    [Trait("EditorID", "CaveGRockPileS01IceBlend")]
    [Trait("SpriggitFile", "Statics/CaveGRockPileS01IceBlend - 0946B2_Skyrim.esm.yaml")]
    public void Skyrim_STAT_ComparisonUi_ShouldRenderSpriggitSample_CaveGRockPileS01IceBlend()
    {
        var spec = StaticValidationSpecs.Skyrim_CaveGRockPileS01IceBlend();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Skyrim <c>XMarkerSnow</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "STAT")]
    [Trait("FormKey", "078DC0:Skyrim.esm")]
    [Trait("EditorID", "XMarkerSnow")]
    [Trait("SpriggitFile", "Statics/XMarkerSnow - 078DC0_Skyrim.esm.yaml")]
    public void Skyrim_STAT_ComparisonUi_ShouldRenderSpriggitSample_XMarkerSnow()
    {
        var spec = StaticValidationSpecs.Skyrim_XMarkerSnow();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }
}
