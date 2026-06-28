using CreationsForge.Core.DTOs.Records;
using CreationsForge.DataValidationTests.Validation.Specs;
using CreationsForge.Specification.Validation.Specs.Static;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.Static.Skyrim;

public class SkyrimStaticSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "STAT")]
    [Trait("FormKey", "0D19F9:Skyrim.esm")]
    [Trait("EditorID", "BlackreachECeiling01_GlowLichen")]
    [Trait("SpriggitFile", "Statics/BlackreachECeiling01_GlowLichen - 0D19F9_Skyrim.esm.yaml")]
    public void Skyrim_STAT_ShouldMatchSpriggitSample_BlackreachECeiling01_GlowLichen()
    {
        var spec = StaticValidationSpecs.Skyrim_BlackreachECeiling01_GlowLichen();
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
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "STAT")]
    [Trait("FormKey", "06DD69:Skyrim.esm")]
    [Trait("EditorID", "DweFacadeTowerSpacer01Snow")]
    [Trait("SpriggitFile", "Statics/DweFacadeTowerSpacer01Snow - 06DD69_Skyrim.esm.yaml")]
    public void Skyrim_STAT_ShouldMatchSpriggitSample_DweFacadeTowerSpacer01Snow()
    {
        var spec = StaticValidationSpecs.Skyrim_DweFacadeTowerSpacer01Snow();
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
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "STAT")]
    [Trait("FormKey", "090E82:Skyrim.esm")]
    [Trait("EditorID", "HHMountainRidge01")]
    [Trait("SpriggitFile", "Statics/HHMountainRidge01 - 090E82_Skyrim.esm.yaml")]
    public void Skyrim_STAT_ShouldMatchSpriggitSample_HHMountainRidge01()
    {
        var spec = StaticValidationSpecs.Skyrim_HHMountainRidge01();
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
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "STAT")]
    [Trait("FormKey", "0946B2:Skyrim.esm")]
    [Trait("EditorID", "CaveGRockPileS01IceBlend")]
    [Trait("SpriggitFile", "Statics/CaveGRockPileS01IceBlend - 0946B2_Skyrim.esm.yaml")]
    public void Skyrim_STAT_ShouldMatchSpriggitSample_CaveGRockPileS01IceBlend()
    {
        var spec = StaticValidationSpecs.Skyrim_CaveGRockPileS01IceBlend();
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
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "STAT")]
    [Trait("FormKey", "078DC0:Skyrim.esm")]
    [Trait("EditorID", "XMarkerSnow")]
    [Trait("SpriggitFile", "Statics/XMarkerSnow - 078DC0_Skyrim.esm.yaml")]
    public void Skyrim_STAT_ShouldMatchSpriggitSample_XMarkerSnow()
    {
        var spec = StaticValidationSpecs.Skyrim_XMarkerSnow();
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
