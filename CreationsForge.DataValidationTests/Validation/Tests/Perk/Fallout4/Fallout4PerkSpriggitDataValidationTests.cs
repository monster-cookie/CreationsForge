using CreationsForge.Core.DTOs.Records;
using CreationsForge.DataValidationTests.Validation.Specs;
using CreationsForge.Specification.Validation.Specs.Perk;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.Perk.Fallout4;

public class Fallout4PerkSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "PERK")]
    [Trait("FormKey", "2458BA:Fallout4.esm")]
    [Trait("EditorID", "AddictionManager")]
    [Trait("SpriggitFile", "Perks/AddictionManager - 2458BA_Fallout4.esm.yaml")]
    public void Fallout4_PERK_ShouldMatchSpriggitSample_AddictionManager()
    {
        var spec = PerkValidationSpecs.Fallout4_AddictionManager();
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
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "PERK")]
    [Trait("FormKey", "01E67F:Fallout4.esm")]
    [Trait("EditorID", "AnimalFriend01")]
    [Trait("SpriggitFile", "Perks/AnimalFriend01 - 01E67F_Fallout4.esm.yaml")]
    public void Fallout4_PERK_ShouldMatchSpriggitSample_AnimalFriend01()
    {
        var spec = PerkValidationSpecs.Fallout4_AnimalFriend01();
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
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "PERK")]
    [Trait("FormKey", "04A0D9:Fallout4.esm")]
    [Trait("EditorID", "AnimalFriend02")]
    [Trait("SpriggitFile", "Perks/AnimalFriend02 - 04A0D9_Fallout4.esm.yaml")]
    public void Fallout4_PERK_ShouldMatchSpriggitSample_AnimalFriend02()
    {
        var spec = PerkValidationSpecs.Fallout4_AnimalFriend02();
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
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "PERK")]
    [Trait("FormKey", "0D979D:Fallout4.esm")]
    [Trait("EditorID", "TrainingAG01")]
    [Trait("SpriggitFile", "Perks/TrainingAG01 - 0D979D_Fallout4.esm.yaml")]
    public void Fallout4_PERK_ShouldMatchSpriggitSample_TrainingAG01()
    {
        var spec = PerkValidationSpecs.Fallout4_TrainingAG01();
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
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "PERK")]
    [Trait("FormKey", "065DFA:Fallout4.esm")]
    [Trait("EditorID", "Basher02")]
    [Trait("SpriggitFile", "Perks/Basher02 - 065DFA_Fallout4.esm.yaml")]
    public void Fallout4_PERK_ShouldMatchSpriggitSample_Basher02()
    {
        var spec = PerkValidationSpecs.Fallout4_Basher02();
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
