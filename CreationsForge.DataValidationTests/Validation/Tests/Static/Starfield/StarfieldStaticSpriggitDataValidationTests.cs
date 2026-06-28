using CreationsForge.Core.DTOs.Records;
using CreationsForge.DataValidationTests.Validation.Specs;
using CreationsForge.Specification.Validation.Specs.Static;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.Static.Starfield;

public class StarfieldStaticSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "STAT")]
    [Trait("FormKey", "0514C6:Starfield.esm")]
    [Trait("EditorID", "OpiExtPodAirlock01")]
    [Trait("SpriggitFile", "Statics/OpiExtPodAirlock01 - 0514C6_Starfield.esm.yaml")]
    public void Starfield_STAT_ShouldMatchSpriggitSample_OpiExtPodAirlock01()
    {
        var spec = StaticValidationSpecs.Starfield_OpiExtPodAirlock01();
        var dto = Helpers.GetDTO<StaticDTO>(spec.Game, spec.RecordType, spec.FormKey);
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
    [Trait("RecordType", "STAT")]
    [Trait("FormKey", "036311:Starfield.esm")]
    [Trait("EditorID", "OpmIntPodSmSide01")]
    [Trait("SpriggitFile", "Statics/OpmIntPodSmSide01 - 036311_Starfield.esm.yaml")]
    public void Starfield_STAT_ShouldMatchSpriggitSample_OpmIntPodSmSide01()
    {
        var spec = StaticValidationSpecs.Starfield_OpmIntPodSmSide01();
        var dto = Helpers.GetDTO<StaticDTO>(spec.Game, spec.RecordType, spec.FormKey);
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
    [Trait("RecordType", "STAT")]
    [Trait("FormKey", "042AE4:Starfield.esm")]
    [Trait("EditorID", "OpmIntPodSmSideWin01")]
    [Trait("SpriggitFile", "Statics/OpmIntPodSmSideWin01 - 042AE4_Starfield.esm.yaml")]
    public void Starfield_STAT_ShouldMatchSpriggitSample_OpmIntPodSmSideWin01()
    {
        var spec = StaticValidationSpecs.Starfield_OpmIntPodSmSideWin01();
        var dto = Helpers.GetDTO<StaticDTO>(spec.Game, spec.RecordType, spec.FormKey);
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
    [Trait("RecordType", "STAT")]
    [Trait("FormKey", "03A1B4:Starfield.esm")]
    [Trait("EditorID", "CatIndWalkSm2WayB01")]
    [Trait("SpriggitFile", "Statics/CatIndWalkSm2WayB01 - 03A1B4_Starfield.esm.yaml")]
    public void Starfield_STAT_ShouldMatchSpriggitSample_CatIndWalkSm2WayB01()
    {
        var spec = StaticValidationSpecs.Starfield_CatIndWalkSm2WayB01();
        var dto = Helpers.GetDTO<StaticDTO>(spec.Game, spec.RecordType, spec.FormKey);
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
    [Trait("RecordType", "STAT")]
    [Trait("FormKey", "04F391:Starfield.esm")]
    [Trait("EditorID", "OpiExtPodAirlockStairs01")]
    [Trait("SpriggitFile", "Statics/OpiExtPodAirlockStairs01 - 04F391_Starfield.esm.yaml")]
    public void Starfield_STAT_ShouldMatchSpriggitSample_OpiExtPodAirlockStairs01()
    {
        var spec = StaticValidationSpecs.Starfield_OpiExtPodAirlockStairs01();
        var dto = Helpers.GetDTO<StaticDTO>(spec.Game, spec.RecordType, spec.FormKey);
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
