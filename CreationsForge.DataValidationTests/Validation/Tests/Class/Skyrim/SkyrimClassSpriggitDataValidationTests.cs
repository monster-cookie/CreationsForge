using CreationsForge.Core.DTOs.Records;
using CreationsForge.DataValidationTests.Validation.Specs;
using CreationsForge.DataValidationTests.Validation.Specs.Class;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.Class.Skyrim;

public class SkyrimClassSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "CLAS")]
    [Trait("FormKey", "0E3A6E:Skyrim.esm")]
    [Trait("EditorID", "TrainerAlchemyExpert")]
    [Trait("SpriggitFile", "Classes/TrainerAlchemyExpert - 0E3A6E_Skyrim.esm.yaml")]
    public void Skyrim_CLAS_ShouldMatchSpriggitSample_TrainerAlchemyExpert()
    {
        AssertClassSpec(ClassValidationSpecs.Skyrim_TrainerAlchemyExpert());
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "CLAS")]
    [Trait("FormKey", "0E3A5D:Skyrim.esm")]
    [Trait("EditorID", "TrainerAlchemyJourneyman")]
    [Trait("SpriggitFile", "Classes/TrainerAlchemyJourneyman - 0E3A5D_Skyrim.esm.yaml")]
    public void Skyrim_CLAS_ShouldMatchSpriggitSample_TrainerAlchemyJourneyman()
    {
        AssertClassSpec(ClassValidationSpecs.Skyrim_TrainerAlchemyJourneyman());
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "CLAS")]
    [Trait("FormKey", "02F202:Skyrim.esm")]
    [Trait("EditorID", "AAAPlayerSpellswordClass")]
    [Trait("SpriggitFile", "Classes/AAAPlayerSpellswordClass - 02F202_Skyrim.esm.yaml")]
    public void Skyrim_CLAS_ShouldMatchSpriggitSample_AAAPlayerSpellswordClass()
    {
        AssertClassSpec(ClassValidationSpecs.Skyrim_AAAPlayerSpellswordClass());
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "CLAS")]
    [Trait("FormKey", "013177:Skyrim.esm")]
    [Trait("EditorID", "CombatSpellsword")]
    [Trait("SpriggitFile", "Classes/CombatSpellsword - 013177_Skyrim.esm.yaml")]
    public void Skyrim_CLAS_ShouldMatchSpriggitSample_CombatSpellsword()
    {
        AssertClassSpec(ClassValidationSpecs.Skyrim_CombatSpellsword());
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "CLAS")]
    [Trait("FormKey", "01325D:Skyrim.esm")]
    [Trait("EditorID", "Bard")]
    [Trait("SpriggitFile", "Classes/Bard - 01325D_Skyrim.esm.yaml")]
    public void Skyrim_CLAS_ShouldMatchSpriggitSample_Bard()
    {
        AssertClassSpec(ClassValidationSpecs.Skyrim_Bard());
    }

    private void AssertClassSpec(ValidationSpec spec)
    {
        var dto = Helpers.GetDTO<ClassDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }
}
