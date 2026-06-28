using Avalonia.Headless.XUnit;
using CreationsForge.Specification.Validation.Specs.NPC;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.UI.Tests.NPC.Skyrim;

/// <summary>
/// Validates Skyrim NPC Spriggit samples against the rendered comparison UI.
/// </summary>
public class SkyrimNPCSpriggitUiValidationTests :
    SpriggitComparisonUiTestBase,
    IClassFixture<SpriggitComparisonUiFixture>
{
    private readonly SpriggitComparisonUiFixture fixture;

    /// <summary>
    /// Initializes the UI validation tests with shared comparison fixture state.
    /// </summary>
    /// <param name="fixture">The fixture used to resolve imported records and comparison services.</param>
    public SkyrimNPCSpriggitUiValidationTests(SpriggitComparisonUiFixture fixture)
    {
        this.fixture = fixture;
    }

    /// <summary>
    /// Validates the Skyrim <c>EncGuardImperialTemplate</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "NPC_")]
    [Trait("FormKey", "0F6F37:Skyrim.esm")]
    [Trait("EditorID", "EncGuardImperialTemplate")]
    [Trait("SpriggitFile", "Npcs/EncGuardImperialTemplate - 0F6F37_Skyrim.esm.yaml")]
    public void Skyrim_NPC__ComparisonUi_ShouldRenderSpriggitSample_EncGuardImperialTemplate()
    {
        var spec = NPCValidationSpecs.Skyrim_EncGuardImperialTemplate();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Skyrim <c>EncGuardSonsTemplate</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "NPC_")]
    [Trait("FormKey", "0F6F38:Skyrim.esm")]
    [Trait("EditorID", "EncGuardSonsTemplate")]
    [Trait("SpriggitFile", "Npcs/EncGuardSonsTemplate - 0F6F38_Skyrim.esm.yaml")]
    public void Skyrim_NPC__ComparisonUi_ShouldRenderSpriggitSample_EncGuardSonsTemplate()
    {
        var spec = NPCValidationSpecs.Skyrim_EncGuardSonsTemplate();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Skyrim <c>EncSiegeImperialSoldierTemplate</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "NPC_")]
    [Trait("FormKey", "041B30:Skyrim.esm")]
    [Trait("EditorID", "EncSiegeImperialSoldierTemplate")]
    [Trait("SpriggitFile", "Npcs/EncSiegeImperialSoldierTemplate - 041B30_Skyrim.esm.yaml")]
    public void Skyrim_NPC__ComparisonUi_ShouldRenderSpriggitSample_EncSiegeImperialSoldierTemplate()
    {
        var spec = NPCValidationSpecs.Skyrim_EncSiegeImperialSoldierTemplate();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Skyrim <c>AelaTheHuntress</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "NPC_")]
    [Trait("FormKey", "01A696:Skyrim.esm")]
    [Trait("EditorID", "AelaTheHuntress")]
    [Trait("SpriggitFile", "Npcs/AelaTheHuntress - 01A696_Skyrim.esm.yaml")]
    public void Skyrim_NPC__ComparisonUi_ShouldRenderSpriggitSample_AelaTheHuntress()
    {
        var spec = NPCValidationSpecs.Skyrim_AelaTheHuntress();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Skyrim <c>AlduinBase</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "NPC_")]
    [Trait("FormKey", "08E4F1:Skyrim.esm")]
    [Trait("EditorID", "AlduinBase")]
    [Trait("SpriggitFile", "Npcs/AlduinBase - 08E4F1_Skyrim.esm.yaml")]
    public void Skyrim_NPC__ComparisonUi_ShouldRenderSpriggitSample_AlduinBase()
    {
        var spec = NPCValidationSpecs.Skyrim_AlduinBase();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }
}
