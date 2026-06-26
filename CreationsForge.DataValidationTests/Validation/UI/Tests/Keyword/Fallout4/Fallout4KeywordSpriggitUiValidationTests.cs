using Avalonia.Headless.XUnit;
using CreationsForge.DataValidationTests.Validation.Specs.Keyword;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.UI.Tests.Keyword.Fallout4;

/// <summary>
/// Validates Fallout4 keyword Spriggit samples against the rendered comparison UI.
/// </summary>
public class Fallout4KeywordSpriggitUiValidationTests :
    SpriggitComparisonUiTestBase,
    IClassFixture<SpriggitComparisonUiFixture>
{
    private readonly SpriggitComparisonUiFixture fixture;

    /// <summary>
    /// Initializes the UI validation tests with shared comparison fixture state.
    /// </summary>
    /// <param name="fixture">The fixture used to resolve imported records and comparison services.</param>
    public Fallout4KeywordSpriggitUiValidationTests(SpriggitComparisonUiFixture fixture)
    {
        this.fixture = fixture;
    }

    /// <summary>
    /// Validates the Fallout4 <c>02Metal03Floor</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "119B9B:Fallout4.esm")]
    [Trait("EditorID", "02Metal03Floor")]
    [Trait("SpriggitFile", "Keywords/02Metal03Floor - 119B9B_Fallout4.esm.yaml")]
    public void Fallout4_KYWD_ComparisonUi_ShouldRenderSpriggitSample__02Metal03Floor()
    {
        var spec = KeywordValidationSpecs.Fallout4_02Metal03Floor();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>02Metal03Misc</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "119B9C:Fallout4.esm")]
    [Trait("EditorID", "02Metal03Misc")]
    [Trait("SpriggitFile", "Keywords/02Metal03Misc - 119B9C_Fallout4.esm.yaml")]
    public void Fallout4_KYWD_ComparisonUi_ShouldRenderSpriggitSample__02Metal03Misc()
    {
        var spec = KeywordValidationSpecs.Fallout4_02Metal03Misc();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>02Metal03Prefabs</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "119B9D:Fallout4.esm")]
    [Trait("EditorID", "02Metal03Prefabs")]
    [Trait("SpriggitFile", "Keywords/02Metal03Prefabs - 119B9D_Fallout4.esm.yaml")]
    public void Fallout4_KYWD_ComparisonUi_ShouldRenderSpriggitSample__02Metal03Prefabs()
    {
        var spec = KeywordValidationSpecs.Fallout4_02Metal03Prefabs();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>AO_BoS_ScribeCollectData</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "0CF43E:Fallout4.esm")]
    [Trait("EditorID", "AO_BoS_ScribeCollectData")]
    [Trait("SpriggitFile", "Keywords/AO_BoS_ScribeCollectData - 0CF43E_Fallout4.esm.yaml")]
    public void Fallout4_KYWD_ComparisonUi_ShouldRenderSpriggitSample_AO_BoS_ScribeCollectData()
    {
        var spec = KeywordValidationSpecs.Fallout4_AO_BoS_ScribeCollectData();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>if_Armor_Combat_Freefall_Restricted</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "093BBE:Fallout4.esm")]
    [Trait("EditorID", "if_Armor_Combat_Freefall_Restricted")]
    [Trait("SpriggitFile", "Keywords/if_Armor_Combat_Freefall_Restricted - 093BBE_Fallout4.esm.yaml")]
    public void Fallout4_KYWD_ComparisonUi_ShouldRenderSpriggitSample_if_Armor_Combat_Freefall_Restricted()
    {
        var spec = KeywordValidationSpecs.Fallout4_if_Armor_Combat_Freefall_Restricted();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>02Metal03Wall</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "119BA0:Fallout4.esm")]
    [Trait("EditorID", "02Metal03Wall")]
    [Trait("SpriggitFile", "Keywords/02Metal03Wall - 119BA0_Fallout4.esm.yaml")]
    public void Fallout4_KYWD_ComparisonUi_ShouldRenderSpriggitSample_02Metal03Wall()
    {
        var spec = KeywordValidationSpecs.Fallout4_02Metal03Wall();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>ActorTypeChild</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "1157E8:Fallout4.esm")]
    [Trait("EditorID", "ActorTypeChild")]
    [Trait("SpriggitFile", "Keywords/ActorTypeChild - 1157E8_Fallout4.esm.yaml")]
    public void Fallout4_KYWD_ComparisonUi_ShouldRenderSpriggitSample_ActorTypeChild()
    {
        var spec = KeywordValidationSpecs.Fallout4_ActorTypeChild();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>AnimArchetypeNervous</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "03D28F:Fallout4.esm")]
    [Trait("EditorID", "AnimArchetypeNervous")]
    [Trait("SpriggitFile", "Keywords/AnimArchetypeNervous - 03D28F_Fallout4.esm.yaml")]
    public void Fallout4_KYWD_ComparisonUi_ShouldRenderSpriggitSample_AnimArchetypeNervous()
    {
        var spec = KeywordValidationSpecs.Fallout4_AnimArchetypeNervous();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>ap_Bot_ModLegsSlotB</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "072B3F:Fallout4.esm")]
    [Trait("EditorID", "ap_Bot_ModLegsSlotB")]
    [Trait("SpriggitFile", "Keywords/ap_Bot_ModLegsSlotB - 072B3F_Fallout4.esm.yaml")]
    public void Fallout4_KYWD_ComparisonUi_ShouldRenderSpriggitSample_ap_Bot_ModLegsSlotB()
    {
        var spec = KeywordValidationSpecs.Fallout4_ap_Bot_ModLegsSlotB();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }
}
