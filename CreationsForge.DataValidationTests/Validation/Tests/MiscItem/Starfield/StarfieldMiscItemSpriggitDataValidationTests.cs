using CreationsForge.Core.DTOs.Records;
using CreationsForge.DataValidationTests.Validation.Specs;
using CreationsForge.DataValidationTests.Validation.Specs.MiscItem;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.MiscItem.Starfield;

public class StarfieldMiscItemSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "MISC")]
    [Trait("FormKey", "005591:Starfield.esm")]
    [Trait("EditorID", "InorgCommonWater")]
    [Trait("SpriggitFile", "MiscItems/InorgCommonWater - 005591_Starfield.esm.yaml")]
    public void Starfield_MISC_ShouldMatchSpriggitSample_InorgCommonWater()
    {
        var spec = MiscItemValidationSpecs.Starfield_InorgCommonWater();
        var dto = Helpers.GetDTO<MiscItemDTO>(spec.Game, spec.RecordType, spec.FormKey);
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
    [Trait("RecordType", "MISC")]
    [Trait("FormKey", "00558C:Starfield.esm")]
    [Trait("EditorID", "InorgExoticPlutonium")]
    [Trait("SpriggitFile", "MiscItems/InorgExoticPlutonium - 00558C_Starfield.esm.yaml")]
    public void Starfield_MISC_ShouldMatchSpriggitSample_InorgExoticPlutonium()
    {
        var spec = MiscItemValidationSpecs.Starfield_InorgExoticPlutonium();
        var dto = Helpers.GetDTO<MiscItemDTO>(spec.Game, spec.RecordType, spec.FormKey);
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
    [Trait("RecordType", "MISC")]
    [Trait("FormKey", "005DED:Starfield.esm")]
    [Trait("EditorID", "InorgUniqueTasine")]
    [Trait("SpriggitFile", "MiscItems/InorgUniqueTasine - 005DED_Starfield.esm.yaml")]
    public void Starfield_MISC_ShouldMatchSpriggitSample_InorgUniqueTasine()
    {
        var spec = MiscItemValidationSpecs.Starfield_InorgUniqueTasine();
        var dto = Helpers.GetDTO<MiscItemDTO>(spec.Game, spec.RecordType, spec.FormKey);
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
    [Trait("RecordType", "MISC")]
    [Trait("FormKey", "302791:Starfield.esm")]
    [Trait("EditorID", "FFCydoniaZ07_HeartOfMarsTitanium")]
    [Trait("SpriggitFile", "MiscItems/FFCydoniaZ07_HeartOfMarsTitanium - 302791_Starfield.esm.yaml")]
    public void Starfield_MISC_ShouldMatchSpriggitSample_FFCydoniaZ07_HeartOfMarsTitanium()
    {
        var spec = MiscItemValidationSpecs.Starfield_FFCydoniaZ07_HeartOfMarsTitanium();
        var dto = Helpers.GetDTO<MiscItemDTO>(spec.Game, spec.RecordType, spec.FormKey);
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
    [Trait("RecordType", "MISC")]
    [Trait("FormKey", "10A797:Starfield.esm")]
    [Trait("EditorID", "ExoticPlayingCard_Diamond_Q")]
    [Trait("SpriggitFile", "MiscItems/ExoticPlayingCard_Diamond_Q - 10A797_Starfield.esm.yaml")]
    public void Starfield_MISC_ShouldMatchSpriggitSample_ExoticPlayingCard_Diamond_Q()
    {
        var spec = MiscItemValidationSpecs.Starfield_ExoticPlayingCard_Diamond_Q();
        var dto = Helpers.GetDTO<MiscItemDTO>(spec.Game, spec.RecordType, spec.FormKey);
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
