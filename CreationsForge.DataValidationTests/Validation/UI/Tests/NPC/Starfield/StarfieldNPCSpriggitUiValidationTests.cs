using Avalonia.Headless.XUnit;
using CreationsForge.Specification.Validation.Specs.NPC;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.UI.Tests.NPC.Starfield;

/// <summary>
/// Validates Starfield NPC Spriggit samples against the rendered comparison UI.
/// </summary>
public class StarfieldNPCSpriggitUiValidationTests :
    SpriggitComparisonUiTestBase,
    IClassFixture<SpriggitComparisonUiFixture>
{
    private readonly SpriggitComparisonUiFixture fixture;

    /// <summary>
    /// Initializes the UI validation tests with shared comparison fixture state.
    /// </summary>
    /// <param name="fixture">The fixture used to resolve imported records and comparison services.</param>
    public StarfieldNPCSpriggitUiValidationTests(SpriggitComparisonUiFixture fixture)
    {
        this.fixture = fixture;
    }

    /// <summary>
    /// Validates the Starfield <c>CF_AludraTahan</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "NPC_")]
    [Trait("FormKey", "01539F:Starfield.esm")]
    [Trait("EditorID", "CF_AludraTahan")]
    [Trait("SpriggitFile", "Npcs/CF_AludraTahan - 01539F_Starfield.esm.yaml")]
    public void Starfield_NPC__ComparisonUi_ShouldRenderSpriggitSample_CF_AludraTahan()
    {
        var spec = NPCValidationSpecs.Starfield_CF_AludraTahan();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>CF_CESandin</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "NPC_")]
    [Trait("FormKey", "0A0273:Starfield.esm")]
    [Trait("EditorID", "CF_CESandin")]
    [Trait("SpriggitFile", "Npcs/CF_CESandin - 0A0273_Starfield.esm.yaml")]
    public void Starfield_NPC__ComparisonUi_ShouldRenderSpriggitSample_CF_CESandin()
    {
        var spec = NPCValidationSpecs.Starfield_CF_CESandin();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>CF_CPMurata</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "NPC_")]
    [Trait("FormKey", "09C32F:Starfield.esm")]
    [Trait("EditorID", "CF_CPMurata")]
    [Trait("SpriggitFile", "Npcs/CF_CPMurata - 09C32F_Starfield.esm.yaml")]
    public void Starfield_NPC__ComparisonUi_ShouldRenderSpriggitSample_CF_CPMurata()
    {
        var spec = NPCValidationSpecs.Starfield_CF_CPMurata();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>BE_FAB12_LvlCitizenChunks</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "NPC_")]
    [Trait("FormKey", "0B6667:Starfield.esm")]
    [Trait("EditorID", "BE_FAB12_LvlCitizenChunks")]
    [Trait("SpriggitFile", "Npcs/BE_FAB12_LvlCitizenChunks - 0B6667_Starfield.esm.yaml")]
    public void Starfield_NPC__ComparisonUi_ShouldRenderSpriggitSample_BE_FAB12_LvlCitizenChunks()
    {
        var spec = NPCValidationSpecs.Starfield_BE_FAB12_LvlCitizenChunks();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Starfield <c>BQ01_Actor_EllieYankton</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "NPC_")]
    [Trait("FormKey", "17C10E:Starfield.esm")]
    [Trait("EditorID", "BQ01_Actor_EllieYankton")]
    [Trait("SpriggitFile", "Npcs/BQ01_Actor_EllieYankton - 17C10E_Starfield.esm.yaml")]
    public void Starfield_NPC__ComparisonUi_ShouldRenderSpriggitSample_BQ01_Actor_EllieYankton()
    {
        var spec = NPCValidationSpecs.Starfield_BQ01_Actor_EllieYankton();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }
}
