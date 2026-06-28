using Avalonia.Headless.XUnit;
using CreationsForge.Specification.Validation.Specs.GameSetting;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.UI.Tests.GameSetting.Skyrim;

/// <summary>
/// Validates Skyrim game setting Spriggit samples against the rendered comparison UI.
/// </summary>
public class SkyrimGameSettingSpriggitUiValidationTests :
    SpriggitComparisonUiTestBase,
    IClassFixture<SpriggitComparisonUiFixture>
{
    private readonly SpriggitComparisonUiFixture fixture;

    /// <summary>
    /// Initializes the UI validation tests with shared comparison fixture state.
    /// </summary>
    /// <param name="fixture">The fixture used to resolve imported records and comparison services.</param>
    public SkyrimGameSettingSpriggitUiValidationTests(SpriggitComparisonUiFixture fixture)
    {
        this.fixture = fixture;
    }

    /// <summary>
    /// Validates the Skyrim <c>sAbortText</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "0D4C40:Skyrim.esm")]
    [Trait("EditorID", "sAbortText")]
    [Trait("SpriggitFile", "GameSettings/sAbortText - 0D4C40_Skyrim.esm.yaml")]
    public void Skyrim_GMST_ComparisonUi_ShouldRenderSpriggitSample_sAbortText()
    {
        var spec = GameSettingValidationSpecs.Skyrim_sAbortText();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Skyrim <c>sAccept</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "0D4DC4:Skyrim.esm")]
    [Trait("EditorID", "sAccept")]
    [Trait("SpriggitFile", "GameSettings/sAccept - 0D4DC4_Skyrim.esm.yaml")]
    public void Skyrim_GMST_ComparisonUi_ShouldRenderSpriggitSample_sAccept()
    {
        var spec = GameSettingValidationSpecs.Skyrim_sAccept();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Skyrim <c>sActionMapping</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "0D4B96:Skyrim.esm")]
    [Trait("EditorID", "sActionMapping")]
    [Trait("SpriggitFile", "GameSettings/sActionMapping - 0D4B96_Skyrim.esm.yaml")]
    public void Skyrim_GMST_ComparisonUi_ShouldRenderSpriggitSample_sActionMapping()
    {
        var spec = GameSettingValidationSpecs.Skyrim_sActionMapping();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Skyrim <c>bRegenNPCMagickaDuringCast</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "0B3D8A:Skyrim.esm")]
    [Trait("EditorID", "bRegenNPCMagickaDuringCast")]
    [Trait("SpriggitFile", "GameSettings/bRegenNPCMagickaDuringCast - 0B3D8A_Skyrim.esm.yaml")]
    public void Skyrim_GMST_ComparisonUi_ShouldRenderSpriggitSample_bRegenNPCMagickaDuringCast()
    {
        var spec = GameSettingValidationSpecs.Skyrim_bRegenNPCMagickaDuringCast();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Skyrim <c>fActionPointsAimAdjustment</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "01A144:Skyrim.esm")]
    [Trait("EditorID", "fActionPointsAimAdjustment")]
    [Trait("SpriggitFile", "GameSettings/fActionPointsAimAdjustment - 01A144_Skyrim.esm.yaml")]
    public void Skyrim_GMST_ComparisonUi_ShouldRenderSpriggitSample_fActionPointsAimAdjustment()
    {
        var spec = GameSettingValidationSpecs.Skyrim_fActionPointsAimAdjustment();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Skyrim <c>fActionPointsAttackOneHandMelee</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "01A145:Skyrim.esm")]
    [Trait("EditorID", "fActionPointsAttackOneHandMelee")]
    [Trait("SpriggitFile", "GameSettings/fActionPointsAttackOneHandMelee - 01A145_Skyrim.esm.yaml")]
    public void Skyrim_GMST_ComparisonUi_ShouldRenderSpriggitSample_fActionPointsAttackOneHandMelee()
    {
        var spec = GameSettingValidationSpecs.Skyrim_fActionPointsAttackOneHandMelee();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Skyrim <c>iAICombatRestoreHealthPercentage</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "01A237:Skyrim.esm")]
    [Trait("EditorID", "iAICombatRestoreHealthPercentage")]
    [Trait("SpriggitFile", "GameSettings/iAICombatRestoreHealthPercentage - 01A237_Skyrim.esm.yaml")]
    public void Skyrim_GMST_ComparisonUi_ShouldRenderSpriggitSample_iAICombatRestoreHealthPercentage()
    {
        var spec = GameSettingValidationSpecs.Skyrim_iAICombatRestoreHealthPercentage();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Skyrim <c>iAISocialDistanceToTriggerEvent</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "GMST")]
    [Trait("FormKey", "01A83A:Skyrim.esm")]
    [Trait("EditorID", "iAISocialDistanceToTriggerEvent")]
    [Trait("SpriggitFile", "GameSettings/iAISocialDistanceToTriggerEvent - 01A83A_Skyrim.esm.yaml")]
    public void Skyrim_GMST_ComparisonUi_ShouldRenderSpriggitSample_iAISocialDistanceToTriggerEvent()
    {
        var spec = GameSettingValidationSpecs.Skyrim_iAISocialDistanceToTriggerEvent();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }
}
