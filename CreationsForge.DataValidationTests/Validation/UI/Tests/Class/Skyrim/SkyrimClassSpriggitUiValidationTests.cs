using Avalonia.Headless.XUnit;
using CreationsForge.DataValidationTests.Validation.Specs.Class;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.UI.Tests.Class.Skyrim;

/// <summary>
/// Validates Skyrim class Spriggit samples against the rendered comparison UI.
/// </summary>
public class SkyrimClassSpriggitUiValidationTests :
    SpriggitComparisonUiTestBase,
    IClassFixture<SpriggitComparisonUiFixture>
{
    private readonly SpriggitComparisonUiFixture fixture;

    /// <summary>
    /// Initializes the UI validation tests with shared comparison fixture state.
    /// </summary>
    /// <param name="fixture">The fixture used to resolve imported records and comparison services.</param>
    public SkyrimClassSpriggitUiValidationTests(SpriggitComparisonUiFixture fixture)
    {
        this.fixture = fixture;
    }

    /// <summary>
    /// Validates the Skyrim <c>TrainerAlchemyExpert</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "CLAS")]
    [Trait("FormKey", "0E3A6E:Skyrim.esm")]
    [Trait("EditorID", "TrainerAlchemyExpert")]
    [Trait("SpriggitFile", "Classes/TrainerAlchemyExpert - 0E3A6E_Skyrim.esm.yaml")]
    public void Skyrim_CLAS_ComparisonUi_ShouldRenderSpriggitSample_TrainerAlchemyExpert()
    {
        var spec = ClassValidationSpecs.Skyrim_TrainerAlchemyExpert();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Skyrim <c>TrainerAlchemyJourneyman</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "CLAS")]
    [Trait("FormKey", "0E3A5D:Skyrim.esm")]
    [Trait("EditorID", "TrainerAlchemyJourneyman")]
    [Trait("SpriggitFile", "Classes/TrainerAlchemyJourneyman - 0E3A5D_Skyrim.esm.yaml")]
    public void Skyrim_CLAS_ComparisonUi_ShouldRenderSpriggitSample_TrainerAlchemyJourneyman()
    {
        var spec = ClassValidationSpecs.Skyrim_TrainerAlchemyJourneyman();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Skyrim <c>AAAPlayerSpellswordClass</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "CLAS")]
    [Trait("FormKey", "02F202:Skyrim.esm")]
    [Trait("EditorID", "AAAPlayerSpellswordClass")]
    [Trait("SpriggitFile", "Classes/AAAPlayerSpellswordClass - 02F202_Skyrim.esm.yaml")]
    public void Skyrim_CLAS_ComparisonUi_ShouldRenderSpriggitSample_AAAPlayerSpellswordClass()
    {
        var spec = ClassValidationSpecs.Skyrim_AAAPlayerSpellswordClass();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Skyrim <c>CombatSpellsword</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "CLAS")]
    [Trait("FormKey", "013177:Skyrim.esm")]
    [Trait("EditorID", "CombatSpellsword")]
    [Trait("SpriggitFile", "Classes/CombatSpellsword - 013177_Skyrim.esm.yaml")]
    public void Skyrim_CLAS_ComparisonUi_ShouldRenderSpriggitSample_CombatSpellsword()
    {
        var spec = ClassValidationSpecs.Skyrim_CombatSpellsword();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }

    /// <summary>
    /// Validates the Skyrim <c>Bard</c> sample against rendered comparison rows.
    /// </summary>
    [AvaloniaFact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "CLAS")]
    [Trait("FormKey", "01325D:Skyrim.esm")]
    [Trait("EditorID", "Bard")]
    [Trait("SpriggitFile", "Classes/Bard - 01325D_Skyrim.esm.yaml")]
    public void Skyrim_CLAS_ComparisonUi_ShouldRenderSpriggitSample_Bard()
    {
        var spec = ClassValidationSpecs.Skyrim_Bard();
        var assertions = SpriggitComparisonUiSpecRunner.GetAssertionCases(spec, fixture);

        assertions.ShouldNotBeEmpty("The UI comparison spec should produce assertions.");
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }
    }
}
