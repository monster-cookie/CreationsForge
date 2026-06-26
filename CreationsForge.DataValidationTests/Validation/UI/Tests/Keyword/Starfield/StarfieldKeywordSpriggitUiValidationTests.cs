using Avalonia.Headless.XUnit;
using CreationsForge.DataValidationTests.Validation.Specs.Keyword;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.UI.Tests.Keyword.Starfield;

/// <summary>
/// Validates Starfield keyword Spriggit samples against the rendered comparison UI.
/// </summary>
public class StarfieldKeywordSpriggitUiValidationTests :
    SpriggitComparisonUiTestBase,
    IClassFixture<SpriggitComparisonUiFixture>
{
    private readonly SpriggitComparisonUiFixture fixture;

    /// <summary>
    /// Initializes the UI validation tests with shared comparison fixture state.
    /// </summary>
    /// <param name="fixture">The fixture used to resolve imported records and comparison services.</param>
    public StarfieldKeywordSpriggitUiValidationTests(SpriggitComparisonUiFixture fixture)
    {
        this.fixture = fixture;
    }

    /// <summary>
    /// Validates the Starfield <c>CCT_Enviro_AmbusherSurface</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "200AEB:Starfield.esm")]
    [Trait("EditorID", "CCT_Enviro_AmbusherSurface")]
    [Trait("SpriggitFile", "Keywords/CCT_Enviro_AmbusherSurface - 200AEB_Starfield.esm.yaml")]
    public void Starfield_KYWD_ComparisonUi_ShouldRenderSpriggitSample_CCT_Enviro_AmbusherSurface()
    {
        var spec = KeywordValidationSpecs.Starfield_CCT_Enviro_AmbusherSurface();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>CCT_Enviro_AmbusherUnderground</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "145388:Starfield.esm")]
    [Trait("EditorID", "CCT_Enviro_AmbusherUnderground")]
    [Trait("SpriggitFile", "Keywords/CCT_Enviro_AmbusherUnderground - 145388_Starfield.esm.yaml")]
    public void Starfield_KYWD_ComparisonUi_ShouldRenderSpriggitSample_CCT_Enviro_AmbusherUnderground()
    {
        var spec = KeywordValidationSpecs.Starfield_CCT_Enviro_AmbusherUnderground();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>CCT_Enviro_Basking</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "200ADF:Starfield.esm")]
    [Trait("EditorID", "CCT_Enviro_Basking")]
    [Trait("SpriggitFile", "Keywords/CCT_Enviro_Basking - 200ADF_Starfield.esm.yaml")]
    public void Starfield_KYWD_ComparisonUi_ShouldRenderSpriggitSample_CCT_Enviro_Basking()
    {
        var spec = KeywordValidationSpecs.Starfield_CCT_Enviro_Basking();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>WeaponTypeDisplay_ElectromagneticRifle</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "1C84DD:Starfield.esm")]
    [Trait("EditorID", "WeaponTypeDisplay_ElectromagneticRifle")]
    [Trait("SpriggitFile", "Keywords/WeaponTypeDisplay_ElectromagneticRifle - 1C84DD_Starfield.esm.yaml")]
    public void Starfield_KYWD_ComparisonUi_ShouldRenderSpriggitSample_WeaponTypeDisplay_ElectromagneticRifle()
    {
        var spec = KeywordValidationSpecs.Starfield_WeaponTypeDisplay_ElectromagneticRifle();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>CCT_Enviro_Spook</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "200AE9:Starfield.esm")]
    [Trait("EditorID", "CCT_Enviro_Spook")]
    [Trait("SpriggitFile", "Keywords/CCT_Enviro_Spook - 200AE9_Starfield.esm.yaml")]
    public void Starfield_KYWD_ComparisonUi_ShouldRenderSpriggitSample_CCT_Enviro_Spook()
    {
        var spec = KeywordValidationSpecs.Starfield_CCT_Enviro_Spook();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>ActorAttackInjuredLeft</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "0345AE:Starfield.esm")]
    [Trait("EditorID", "ActorAttackInjuredLeft")]
    [Trait("SpriggitFile", "Keywords/ActorAttackInjuredLeft - 0345AE_Starfield.esm.yaml")]
    public void Starfield_KYWD_ComparisonUi_ShouldRenderSpriggitSample_ActorAttackInjuredLeft()
    {
        var spec = KeywordValidationSpecs.Starfield_ActorAttackInjuredLeft();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>ActorTypeChild</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "1157E8:Starfield.esm")]
    [Trait("EditorID", "ActorTypeChild")]
    [Trait("SpriggitFile", "Keywords/ActorTypeChild - 1157E8_Starfield.esm.yaml")]
    public void Starfield_KYWD_ComparisonUi_ShouldRenderSpriggitSample_ActorTypeChild()
    {
        var spec = KeywordValidationSpecs.Starfield_ActorTypeChild();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>AnimArchetypeEyeDown</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "24E96F:Starfield.esm")]
    [Trait("EditorID", "AnimArchetypeEyeDown")]
    [Trait("SpriggitFile", "Keywords/AnimArchetypeEyeDown - 24E96F_Starfield.esm.yaml")]
    public void Starfield_KYWD_ComparisonUi_ShouldRenderSpriggitSample_AnimArchetypeEyeDown()
    {
        var spec = KeywordValidationSpecs.Starfield_AnimArchetypeEyeDown();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>ap_AVM_Armor_Skin</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "157D41:Starfield.esm")]
    [Trait("EditorID", "ap_AVM_Armor_Skin")]
    [Trait("SpriggitFile", "Keywords/ap_AVM_Armor_Skin - 157D41_Starfield.esm.yaml")]
    public void Starfield_KYWD_ComparisonUi_ShouldRenderSpriggitSample_ap_AVM_Armor_Skin()
    {
        var spec = KeywordValidationSpecs.Starfield_ap_AVM_Armor_Skin();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }
}
