using CreationsForge.Core.DTOs.Records;
using CreationsForge.DataValidationTests.Validation.Specs;
using CreationsForge.Specification.Validation.Specs.Perk;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.Perk.Skyrim;

public class SkyrimPerkSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "PERK")]
    [Trait("FormKey", "0A725C:Skyrim.esm")]
    [Trait("EditorID", "AlchemySkillBoosts")]
    [Trait("SpriggitFile", "Perks/AlchemySkillBoosts - 0A725C_Skyrim.esm.yaml")]
    public void Skyrim_PERK_ShouldMatchSpriggitSample_AlchemySkillBoosts()
    {
        var spec = PerkValidationSpecs.Skyrim_AlchemySkillBoosts();
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
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "PERK")]
    [Trait("FormKey", "0BABE4:Skyrim.esm")]
    [Trait("EditorID", "Armsman00")]
    [Trait("SpriggitFile", "Perks/Armsman00 - 0BABE4_Skyrim.esm.yaml")]
    public void Skyrim_PERK_ShouldMatchSpriggitSample_Armsman00()
    {
        var spec = PerkValidationSpecs.Skyrim_Armsman00();
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
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "PERK")]
    [Trait("FormKey", "079343:Skyrim.esm")]
    [Trait("EditorID", "Armsman20")]
    [Trait("SpriggitFile", "Perks/Armsman20 - 079343_Skyrim.esm.yaml")]
    public void Skyrim_PERK_ShouldMatchSpriggitSample_Armsman20()
    {
        var spec = PerkValidationSpecs.Skyrim_Armsman20();
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
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "PERK")]
    [Trait("FormKey", "058F75:Skyrim.esm")]
    [Trait("EditorID", "Allure")]
    [Trait("SpriggitFile", "Perks/Allure - 058F75_Skyrim.esm.yaml")]
    public void Skyrim_PERK_ShouldMatchSpriggitSample_Allure()
    {
        var spec = PerkValidationSpecs.Skyrim_Allure();
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
