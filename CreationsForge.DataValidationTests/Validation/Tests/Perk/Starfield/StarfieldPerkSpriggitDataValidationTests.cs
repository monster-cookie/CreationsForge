using CreationsForge.Core.DTOs.Records;
using CreationsForge.DataValidationTests.Validation.Specs;
using CreationsForge.DataValidationTests.Validation.Specs.Perk;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.Perk.Starfield;

public class StarfieldPerkSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "PERK")]
    [Trait("FormKey", "08C3EE:Starfield.esm")]
    [Trait("EditorID", "Skill_BoostAssaultTraining")]
    [Trait("SpriggitFile", "Perks/Skill_BoostAssaultTraining - 08C3EE_Starfield.esm.yaml")]
    public void Starfield_PERK_ShouldMatchSpriggitSample_Skill_BoostAssaultTraining()
    {
        var spec = PerkValidationSpecs.Starfield_Skill_BoostAssaultTraining();
        var dto = Helpers.GetDTO<PerkDTO>(spec.Game, spec.RecordType, spec.FormKey);
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(spec.Game, spec.RecordType, spec.SampleName);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "PERK")]
    [Trait("FormKey", "146C2C:Starfield.esm")]
    [Trait("EditorID", "Skill_BoostPackTraining")]
    [Trait("SpriggitFile", "Perks/Skill_BoostPackTraining - 146C2C_Starfield.esm.yaml")]
    public void Starfield_PERK_ShouldMatchSpriggitSample_Skill_BoostPackTraining()
    {
        var spec = PerkValidationSpecs.Starfield_Skill_BoostPackTraining();
        var dto = Helpers.GetDTO<PerkDTO>(spec.Game, spec.RecordType, spec.FormKey);
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(spec.Game, spec.RecordType, spec.SampleName);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "PERK")]
    [Trait("FormKey", "27CBBE:Starfield.esm")]
    [Trait("EditorID", "TrainingTechnologyExpert")]
    [Trait("SpriggitFile", "Perks/TrainingTechnologyExpert - 27CBBE_Starfield.esm.yaml")]
    public void Starfield_PERK_ShouldMatchSpriggitSample_TrainingTechnologyExpert()
    {
        var spec = PerkValidationSpecs.Starfield_TrainingTechnologyExpert();
        var dto = Helpers.GetDTO<PerkDTO>(spec.Game, spec.RecordType, spec.FormKey);
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(spec.Game, spec.RecordType, spec.SampleName);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "PERK")]
    [Trait("FormKey", "227FD5:Starfield.esm")]
    [Trait("EditorID", "TRAIT_FreestarCollectiveSettler")]
    [Trait("SpriggitFile", "Perks/TRAIT_FreestarCollectiveSettler - 227FD5_Starfield.esm.yaml")]
    public void Starfield_PERK_ShouldMatchSpriggitSample_TRAIT_FreestarCollectiveSettler()
    {
        var spec = PerkValidationSpecs.Starfield_TRAIT_FreestarCollectiveSettler();
        var dto = Helpers.GetDTO<PerkDTO>(spec.Game, spec.RecordType, spec.FormKey);
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(spec.Game, spec.RecordType, spec.SampleName);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "PERK")]
    [Trait("FormKey", "22EC76:Starfield.esm")]
    [Trait("EditorID", "BackgroundBigGameHunter")]
    [Trait("SpriggitFile", "Perks/BackgroundBigGameHunter - 22EC76_Starfield.esm.yaml")]
    public void Starfield_PERK_ShouldMatchSpriggitSample_BackgroundBigGameHunter()
    {
        var spec = PerkValidationSpecs.Starfield_BackgroundBigGameHunter();
        var dto = Helpers.GetDTO<PerkDTO>(spec.Game, spec.RecordType, spec.FormKey);
        var spriggit = Helpers.GetSpriggit<SpriggitRecordDTO>(spec.Game, spec.RecordType, spec.SampleName);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedSpriggitFields(spriggit, dto).ShouldBeEmpty();
        Helpers.GetUnmatchedDtoFields(spriggit, dto).ShouldBeEmpty();
    }
}
