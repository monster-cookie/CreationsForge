using Avalonia.Headless.XUnit;
using CreationsForge.DataValidationTests.Validation.Specs.GameSetting;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.UI.Tests.GameSetting.Starfield;

/// <summary>
/// Validates Starfield game setting Spriggit samples against the rendered comparison UI.
/// </summary>
public class StarfieldGameSettingSpriggitUiValidationTests :
    SpriggitComparisonUiTestBase,
    IClassFixture<SpriggitComparisonUiFixture>
{
    private readonly SpriggitComparisonUiFixture fixture;

    /// <summary>
    /// Initializes the UI validation tests with shared comparison fixture state.
    /// </summary>
    /// <param name="fixture">The fixture used to resolve imported records and comparison services.</param>
    public StarfieldGameSettingSpriggitUiValidationTests(SpriggitComparisonUiFixture fixture)
    {
        this.fixture = fixture;
    }

    /// <summary>
    /// Validates the Starfield <c>sAbort</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "0657E0:Starfield.esm")]
    [Trait("EditorID", "sAbort")]
    [Trait("SpriggitFile", "GameSettings/sAbort - 0657E0_Starfield.esm.yaml")]
    public void Starfield_GMST_ComparisonUi_ShouldRenderSpriggitSample_sAbort()
    {
        var spec = GameSettingValidationSpecs.Starfield_sAbort();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>sActivate</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "0D4DFC:Starfield.esm")]
    [Trait("EditorID", "sActivate")]
    [Trait("SpriggitFile", "GameSettings/sActivate - 0D4DFC_Starfield.esm.yaml")]
    public void Starfield_GMST_ComparisonUi_ShouldRenderSpriggitSample_sActivate()
    {
        var spec = GameSettingValidationSpecs.Starfield_sActivate();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>sActivateCreatureCalmed</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "0D4DEB:Starfield.esm")]
    [Trait("EditorID", "sActivateCreatureCalmed")]
    [Trait("SpriggitFile", "GameSettings/sActivateCreatureCalmed - 0D4DEB_Starfield.esm.yaml")]
    public void Starfield_GMST_ComparisonUi_ShouldRenderSpriggitSample_sActivateCreatureCalmed()
    {
        var spec = GameSettingValidationSpecs.Starfield_sActivateCreatureCalmed();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>bAllowBlinksDuringSpeech</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "0F9CFD:Starfield.esm")]
    [Trait("EditorID", "bAllowBlinksDuringSpeech")]
    [Trait("SpriggitFile", "GameSettings/bAllowBlinksDuringSpeech - 0F9CFD_Starfield.esm.yaml")]
    public void Starfield_GMST_ComparisonUi_ShouldRenderSpriggitSample_bAllowBlinksDuringSpeech()
    {
        var spec = GameSettingValidationSpecs.Starfield_bAllowBlinksDuringSpeech();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>bBoostpackInitialThrustOnlyOnTakeoff</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "024CA5:Starfield.esm")]
    [Trait("EditorID", "bBoostpackInitialThrustOnlyOnTakeoff")]
    [Trait("SpriggitFile", "GameSettings/bBoostpackInitialThrustOnlyOnTakeoff - 024CA5_Starfield.esm.yaml")]
    public void Starfield_GMST_ComparisonUi_ShouldRenderSpriggitSample_bBoostpackInitialThrustOnlyOnTakeoff()
    {
        var spec = GameSettingValidationSpecs.Starfield_bBoostpackInitialThrustOnlyOnTakeoff();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>fActorDefaultTurningSpeed</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "101046:Starfield.esm")]
    [Trait("EditorID", "fActorDefaultTurningSpeed")]
    [Trait("SpriggitFile", "GameSettings/fActorDefaultTurningSpeed - 101046_Starfield.esm.yaml")]
    public void Starfield_GMST_ComparisonUi_ShouldRenderSpriggitSample_fActorDefaultTurningSpeed()
    {
        var spec = GameSettingValidationSpecs.Starfield_fActorDefaultTurningSpeed();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>fActorSwimBreathDamage</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "097F48:Starfield.esm")]
    [Trait("EditorID", "fActorSwimBreathDamage")]
    [Trait("SpriggitFile", "GameSettings/fActorSwimBreathDamage - 097F48_Starfield.esm.yaml")]
    public void Starfield_GMST_ComparisonUi_ShouldRenderSpriggitSample_fActorSwimBreathDamage()
    {
        var spec = GameSettingValidationSpecs.Starfield_fActorSwimBreathDamage();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>iAICombatRestoreHealthPercentage</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "01A237:Starfield.esm")]
    [Trait("EditorID", "iAICombatRestoreHealthPercentage")]
    [Trait("SpriggitFile", "GameSettings/iAICombatRestoreHealthPercentage - 01A237_Starfield.esm.yaml")]
    public void Starfield_GMST_ComparisonUi_ShouldRenderSpriggitSample_iAICombatRestoreHealthPercentage()
    {
        var spec = GameSettingValidationSpecs.Starfield_iAICombatRestoreHealthPercentage();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>iAIMaxSocialDistanceToTriggerEvent</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "003207:Starfield.esm")]
    [Trait("EditorID", "iAIMaxSocialDistanceToTriggerEvent")]
    [Trait("SpriggitFile", "GameSettings/iAIMaxSocialDistanceToTriggerEvent - 003207_Starfield.esm.yaml")]
    public void Starfield_GMST_ComparisonUi_ShouldRenderSpriggitSample_iAIMaxSocialDistanceToTriggerEvent()
    {
        var spec = GameSettingValidationSpecs.Starfield_iAIMaxSocialDistanceToTriggerEvent();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>uDefaultLevelZone01max</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "246BD8:Starfield.esm")]
    [Trait("EditorID", "uDefaultLevelZone01max")]
    [Trait("SpriggitFile", "GameSettings/uDefaultLevelZone01max - 246BD8_Starfield.esm.yaml")]
    public void Starfield_GMST_ComparisonUi_ShouldRenderSpriggitSample_uDefaultLevelZone01max()
    {
        var spec = GameSettingValidationSpecs.Starfield_uDefaultLevelZone01max();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>uDefaultLevelZone02min</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "246BD9:Starfield.esm")]
    [Trait("EditorID", "uDefaultLevelZone02min")]
    [Trait("SpriggitFile", "GameSettings/uDefaultLevelZone02min - 246BD9_Starfield.esm.yaml")]
    public void Starfield_GMST_ComparisonUi_ShouldRenderSpriggitSample_uDefaultLevelZone02min()
    {
        var spec = GameSettingValidationSpecs.Starfield_uDefaultLevelZone02min();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }
}
