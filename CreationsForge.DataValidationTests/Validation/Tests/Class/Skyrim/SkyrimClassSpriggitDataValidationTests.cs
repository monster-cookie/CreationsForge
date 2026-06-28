using CreationsForge.Core.DTOs.Records;
using CreationsForge.DataValidationTests.Validation.Specs;
using CreationsForge.Specification.Validation.Specs.Class;
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
        var spec = ClassValidationSpecs.Skyrim_TrainerAlchemyExpert();
        var dto = Helpers.GetDTO<ClassDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "CLAS")]
    [Trait("FormKey", "0E3A5D:Skyrim.esm")]
    [Trait("EditorID", "TrainerAlchemyJourneyman")]
    [Trait("SpriggitFile", "Classes/TrainerAlchemyJourneyman - 0E3A5D_Skyrim.esm.yaml")]
    public void Skyrim_CLAS_ShouldMatchSpriggitSample_TrainerAlchemyJourneyman()
    {
        var spec = ClassValidationSpecs.Skyrim_TrainerAlchemyJourneyman();
        var dto = Helpers.GetDTO<ClassDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "CLAS")]
    [Trait("FormKey", "02F202:Skyrim.esm")]
    [Trait("EditorID", "AAAPlayerSpellswordClass")]
    [Trait("SpriggitFile", "Classes/AAAPlayerSpellswordClass - 02F202_Skyrim.esm.yaml")]
    public void Skyrim_CLAS_ShouldMatchSpriggitSample_AAAPlayerSpellswordClass()
    {
        var spec = ClassValidationSpecs.Skyrim_AAAPlayerSpellswordClass();
        var dto = Helpers.GetDTO<ClassDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "CLAS")]
    [Trait("FormKey", "013177:Skyrim.esm")]
    [Trait("EditorID", "CombatSpellsword")]
    [Trait("SpriggitFile", "Classes/CombatSpellsword - 013177_Skyrim.esm.yaml")]
    public void Skyrim_CLAS_ShouldMatchSpriggitSample_CombatSpellsword()
    {
        var spec = ClassValidationSpecs.Skyrim_CombatSpellsword();
        var dto = Helpers.GetDTO<ClassDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "CLAS")]
    [Trait("FormKey", "01325D:Skyrim.esm")]
    [Trait("EditorID", "Bard")]
    [Trait("SpriggitFile", "Classes/Bard - 01325D_Skyrim.esm.yaml")]
    public void Skyrim_CLAS_ShouldMatchSpriggitSample_Bard()
    {
        var spec = ClassValidationSpecs.Skyrim_Bard();
        var dto = Helpers.GetDTO<ClassDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }
}
