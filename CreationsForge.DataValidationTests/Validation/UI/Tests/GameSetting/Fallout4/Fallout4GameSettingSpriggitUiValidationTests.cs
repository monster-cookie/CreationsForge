using Avalonia.Headless.XUnit;
using CreationsForge.Specification.Validation.Specs.GameSetting;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.UI.Tests.GameSetting.Fallout4;

/// <summary>
/// Validates Fallout4 game setting Spriggit samples against the rendered comparison UI.
/// </summary>
public class Fallout4GameSettingSpriggitUiValidationTests :
    SpriggitComparisonUiTestBase,
    IClassFixture<SpriggitComparisonUiFixture>
{
    private readonly SpriggitComparisonUiFixture fixture;

    /// <summary>
    /// Initializes the UI validation tests with shared comparison fixture state.
    /// </summary>
    /// <param name="fixture">The fixture used to resolve imported records and comparison services.</param>
    public Fallout4GameSettingSpriggitUiValidationTests(SpriggitComparisonUiFixture fixture)
    {
        this.fixture = fixture;
    }

    /// <summary>
    /// Validates the Fallout4 <c>sAbortText</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "0D4C40:Fallout4.esm")]
    [Trait("EditorID", "sAbortText")]
    [Trait("SpriggitFile", "GameSettings/sAbortText - 0D4C40_Fallout4.esm.yaml")]
    public void Fallout4_GMST_ComparisonUi_ShouldRenderSpriggitSample_sAbortText()
    {
        var spec = GameSettingValidationSpecs.Fallout4_sAbortText();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>sAccept</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "0D4DC4:Fallout4.esm")]
    [Trait("EditorID", "sAccept")]
    [Trait("SpriggitFile", "GameSettings/sAccept - 0D4DC4_Fallout4.esm.yaml")]
    public void Fallout4_GMST_ComparisonUi_ShouldRenderSpriggitSample_sAccept()
    {
        var spec = GameSettingValidationSpecs.Fallout4_sAccept();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>sActivate</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "0D4DFC:Fallout4.esm")]
    [Trait("EditorID", "sActivate")]
    [Trait("SpriggitFile", "GameSettings/sActivate - 0D4DFC_Fallout4.esm.yaml")]
    public void Fallout4_GMST_ComparisonUi_ShouldRenderSpriggitSample_sActivate()
    {
        var spec = GameSettingValidationSpecs.Fallout4_sActivate();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>bAllowBlinksDuringSpeech</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "0F9CFD:Fallout4.esm")]
    [Trait("EditorID", "bAllowBlinksDuringSpeech")]
    [Trait("SpriggitFile", "GameSettings/bAllowBlinksDuringSpeech - 0F9CFD_Fallout4.esm.yaml")]
    public void Fallout4_GMST_ComparisonUi_ShouldRenderSpriggitSample_bAllowBlinksDuringSpeech()
    {
        var spec = GameSettingValidationSpecs.Fallout4_bAllowBlinksDuringSpeech();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>fActionPointsAttackOneHandMelee</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "01A145:Fallout4.esm")]
    [Trait("EditorID", "fActionPointsAttackOneHandMelee")]
    [Trait("SpriggitFile", "GameSettings/fActionPointsAttackOneHandMelee - 01A145_Fallout4.esm.yaml")]
    public void Fallout4_GMST_ComparisonUi_ShouldRenderSpriggitSample_fActionPointsAttackOneHandMelee()
    {
        var spec = GameSettingValidationSpecs.Fallout4_fActionPointsAttackOneHandMelee();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>fActionPointsAttackRanged</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "08A207:Fallout4.esm")]
    [Trait("EditorID", "fActionPointsAttackRanged")]
    [Trait("SpriggitFile", "GameSettings/fActionPointsAttackRanged - 08A207_Fallout4.esm.yaml")]
    public void Fallout4_GMST_ComparisonUi_ShouldRenderSpriggitSample_fActionPointsAttackRanged()
    {
        var spec = GameSettingValidationSpecs.Fallout4_fActionPointsAttackRanged();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>iAICombatRestoreHealthPercentage</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "01A237:Fallout4.esm")]
    [Trait("EditorID", "iAICombatRestoreHealthPercentage")]
    [Trait("SpriggitFile", "GameSettings/iAICombatRestoreHealthPercentage - 01A237_Fallout4.esm.yaml")]
    public void Fallout4_GMST_ComparisonUi_ShouldRenderSpriggitSample_iAICombatRestoreHealthPercentage()
    {
        var spec = GameSettingValidationSpecs.Fallout4_iAICombatRestoreHealthPercentage();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>iAISocialDistanceToTriggerEvent</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "01A83A:Fallout4.esm")]
    [Trait("EditorID", "iAISocialDistanceToTriggerEvent")]
    [Trait("SpriggitFile", "GameSettings/iAISocialDistanceToTriggerEvent - 01A83A_Fallout4.esm.yaml")]
    public void Fallout4_GMST_ComparisonUi_ShouldRenderSpriggitSample_iAISocialDistanceToTriggerEvent()
    {
        var spec = GameSettingValidationSpecs.Fallout4_iAISocialDistanceToTriggerEvent();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>uDefaultLevelZone01max</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "246BD8:Fallout4.esm")]
    [Trait("EditorID", "uDefaultLevelZone01max")]
    [Trait("SpriggitFile", "GameSettings/uDefaultLevelZone01max - 246BD8_Fallout4.esm.yaml")]
    public void Fallout4_GMST_ComparisonUi_ShouldRenderSpriggitSample_uDefaultLevelZone01max()
    {
        var spec = GameSettingValidationSpecs.Fallout4_uDefaultLevelZone01max();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>uDefaultLevelZone02min</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "246BD9:Fallout4.esm")]
    [Trait("EditorID", "uDefaultLevelZone02min")]
    [Trait("SpriggitFile", "GameSettings/uDefaultLevelZone02min - 246BD9_Fallout4.esm.yaml")]
    public void Fallout4_GMST_ComparisonUi_ShouldRenderSpriggitSample_uDefaultLevelZone02min()
    {
        var spec = GameSettingValidationSpecs.Fallout4_uDefaultLevelZone02min();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }
}
