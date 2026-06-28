using Avalonia.Headless.XUnit;
using CreationsForge.Specification.Validation.Specs.NPC;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.UI.Tests.NPC.Fallout4;

/// <summary>
/// Validates Fallout4 NPC Spriggit samples against the rendered comparison UI.
/// </summary>
public class Fallout4NPCSpriggitUiValidationTests :
    SpriggitComparisonUiTestBase,
    IClassFixture<SpriggitComparisonUiFixture>
{
    private readonly SpriggitComparisonUiFixture fixture;

    /// <summary>
    /// Initializes the UI validation tests with shared comparison fixture state.
    /// </summary>
    /// <param name="fixture">The fixture used to resolve imported records and comparison services.</param>
    public Fallout4NPCSpriggitUiValidationTests(SpriggitComparisonUiFixture fixture)
    {
        this.fixture = fixture;
    }

    /// <summary>
    /// Validates the Fallout4 <c>BHExtBOSSoldier</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "NPC_")]
    [Trait("FormKey", "0FB232:Fallout4.esm")]
    [Trait("EditorID", "BHExtBOSSoldier")]
    [Trait("SpriggitFile", "Npcs/BHExtBOSSoldier - 0FB232_Fallout4.esm.yaml")]
    public void Fallout4_NPC__ComparisonUi_ShouldRenderSpriggitSample_BHExtBOSSoldier()
    {
        var spec = NPCValidationSpecs.Fallout4_BHExtBOSSoldier();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>BHExtBOSSoldier_PowerArmorAuto</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "NPC_")]
    [Trait("FormKey", "0FB22E:Fallout4.esm")]
    [Trait("EditorID", "BHExtBOSSoldier_PowerArmorAuto")]
    [Trait("SpriggitFile", "Npcs/BHExtBOSSoldier_PowerArmorAuto - 0FB22E_Fallout4.esm.yaml")]
    public void Fallout4_NPC__ComparisonUi_ShouldRenderSpriggitSample_BHExtBOSSoldier_PowerArmorAuto()
    {
        var spec = NPCValidationSpecs.Fallout4_BHExtBOSSoldier_PowerArmorAuto();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>BHExtBOSSoldier_PowerArmorBigGun</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "NPC_")]
    [Trait("FormKey", "1D58EA:Fallout4.esm")]
    [Trait("EditorID", "BHExtBOSSoldier_PowerArmorBigGun")]
    [Trait("SpriggitFile", "Npcs/BHExtBOSSoldier_PowerArmorBigGun - 1D58EA_Fallout4.esm.yaml")]
    public void Fallout4_NPC__ComparisonUi_ShouldRenderSpriggitSample_BHExtBOSSoldier_PowerArmorBigGun()
    {
        var spec = NPCValidationSpecs.Fallout4_BHExtBOSSoldier_PowerArmorBigGun();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>AllieFilmore</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "NPC_")]
    [Trait("FormKey", "05E557:Fallout4.esm")]
    [Trait("EditorID", "AllieFilmore")]
    [Trait("SpriggitFile", "Npcs/AllieFilmore - 05E557_Fallout4.esm.yaml")]
    public void Fallout4_NPC__ComparisonUi_ShouldRenderSpriggitSample_AllieFilmore()
    {
        var spec = NPCValidationSpecs.Fallout4_AllieFilmore();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Fallout4 <c>AudioTemplateSynthGen1</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "NPC_")]
    [Trait("FormKey", "240C21:Fallout4.esm")]
    [Trait("EditorID", "AudioTemplateSynthGen1")]
    [Trait("SpriggitFile", "Npcs/AudioTemplateSynthGen1 - 240C21_Fallout4.esm.yaml")]
    public void Fallout4_NPC__ComparisonUi_ShouldRenderSpriggitSample_AudioTemplateSynthGen1()
    {
        var spec = NPCValidationSpecs.Fallout4_AudioTemplateSynthGen1();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }
}
