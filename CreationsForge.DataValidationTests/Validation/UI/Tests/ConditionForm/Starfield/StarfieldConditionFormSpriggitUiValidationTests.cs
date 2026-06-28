using Avalonia.Headless.XUnit;
using CreationsForge.Specification.Validation.Specs.ConditionForm;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.UI.Tests.ConditionForm.Starfield;

/// <summary>
/// Validates Starfield condition form Spriggit samples against the rendered comparison UI.
/// </summary>
public class StarfieldConditionFormSpriggitUiValidationTests :
    SpriggitComparisonUiTestBase,
    IClassFixture<SpriggitComparisonUiFixture>
{
    private readonly SpriggitComparisonUiFixture fixture;

    /// <summary>
    /// Initializes the UI validation tests with shared comparison fixture state.
    /// </summary>
    /// <param name="fixture">The fixture used to resolve imported records and comparison services.</param>
    public StarfieldConditionFormSpriggitUiValidationTests(SpriggitComparisonUiFixture fixture)
    {
        this.fixture = fixture;
    }

    /// <summary>
    /// Validates the Starfield <c>DebugMoveToPlanetConditions_Trait</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "CNDF")]
    [Trait("FormKey", "3C8F9C:Starfield.esm")]
    [Trait("EditorID", "DebugMoveToPlanetConditions_Trait")]
    [Trait("SpriggitFile", "ConditionRecords/DebugMoveToPlanetConditions_Trait - 3C8F9C_Starfield.esm.yaml")]
    public void Starfield_CNDF_ComparisonUi_ShouldRenderSpriggitSample_DebugMoveToPlanetConditions_Trait()
    {
        var spec = ConditionFormValidationSpecs.Starfield_DebugMoveToPlanetConditions_Trait();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>SFBGS_CND_Placeholder01_ReservedForUse</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "CNDF")]
    [Trait("FormKey", "31982F:Starfield.esm")]
    [Trait("EditorID", "SFBGS_CND_Placeholder01_ReservedForUse")]
    [Trait("SpriggitFile", "ConditionRecords/SFBGS_CND_Placeholder01_ReservedForUse - 31982F_Starfield.esm.yaml")]
    public void Starfield_CNDF_ComparisonUi_ShouldRenderSpriggitSample_SFBGS_CND_Placeholder01_ReservedForUse()
    {
        var spec = ConditionFormValidationSpecs.Starfield_SFBGS_CND_Placeholder01_ReservedForUse();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>SQ_TreasureMap_CND_IsResourceLocation</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "CNDF")]
    [Trait("FormKey", "10460E:Starfield.esm")]
    [Trait("EditorID", "SQ_TreasureMap_CND_IsResourceLocation")]
    [Trait("SpriggitFile", "ConditionRecords/SQ_TreasureMap_CND_IsResourceLocation - 10460E_Starfield.esm.yaml")]
    public void Starfield_CNDF_ComparisonUi_ShouldRenderSpriggitSample_SQ_TreasureMap_CND_IsResourceLocation()
    {
        var spec = ConditionFormValidationSpecs.Starfield_SQ_TreasureMap_CND_IsResourceLocation();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>ActorShouldShowSpacesuitGameplayFlashlight</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "CNDF")]
    [Trait("FormKey", "0B1206:Starfield.esm")]
    [Trait("EditorID", "ActorShouldShowSpacesuitGameplayFlashlight")]
    [Trait("SpriggitFile", "ConditionRecords/ActorShouldShowSpacesuitGameplayFlashlight - 0B1206_Starfield.esm.yaml")]
    public void Starfield_CNDF_ComparisonUi_ShouldRenderSpriggitSample_ActorShouldShowSpacesuitGameplayFlashlight()
    {
        var spec = ConditionFormValidationSpecs.Starfield_ActorShouldShowSpacesuitGameplayFlashlight();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }
}
