using CreationsForge.Core.DTOs.Records;
using CreationsForge.DataValidationTests.Validation.Specs;
using CreationsForge.DataValidationTests.Validation.Specs.Keyword;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.Keyword.Skyrim;

public class SkyrimKeywordSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "10EAD7:Skyrim.esm")]
    [Trait("EditorID", "ActorTypeFamiliar")]
    [Trait("SpriggitFile", "Keywords/ActorTypeFamiliar - 10EAD7_Skyrim.esm.yaml")]
    public void Skyrim_KYWD_ShouldMatchSpriggitSample_ActorTypeFamiliar()
    {
        var spec = KeywordValidationSpecs.Skyrim_ActorTypeFamiliar();
        var dto = Helpers.GetDTO<KeywordDTO>(spec.Game, spec.RecordType, spec.FormKey);
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
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "10E984:Skyrim.esm")]
    [Trait("EditorID", "ActorTypeGiant")]
    [Trait("SpriggitFile", "Keywords/ActorTypeGiant - 10E984_Skyrim.esm.yaml")]
    public void Skyrim_KYWD_ShouldMatchSpriggitSample_ActorTypeGiant()
    {
        var spec = KeywordValidationSpecs.Skyrim_ActorTypeGiant();
        var dto = Helpers.GetDTO<KeywordDTO>(spec.Game, spec.RecordType, spec.FormKey);
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
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "0F5D16:Skyrim.esm")]
    [Trait("EditorID", "ActorTypeTroll")]
    [Trait("SpriggitFile", "Keywords/ActorTypeTroll - 0F5D16_Skyrim.esm.yaml")]
    public void Skyrim_KYWD_ShouldMatchSpriggitSample_ActorTypeTroll()
    {
        var spec = KeywordValidationSpecs.Skyrim_ActorTypeTroll();
        var dto = Helpers.GetDTO<KeywordDTO>(spec.Game, spec.RecordType, spec.FormKey);
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
    [Trait("RecordType", "KYWD")]
    [Trait("FormKey", "06DEAD:Skyrim.esm")]
    [Trait("EditorID", "ActivatorLever")]
    [Trait("SpriggitFile", "Keywords/ActivatorLever - 06DEAD_Skyrim.esm.yaml")]
    public void Skyrim_KYWD_ShouldMatchSpriggitSample_ActivatorLever()
    {
        var spec = KeywordValidationSpecs.Skyrim_ActivatorLever();
        var dto = Helpers.GetDTO<KeywordDTO>(spec.Game, spec.RecordType, spec.FormKey);
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
