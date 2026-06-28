using CreationsForge.Core.DTOs.Records;
using CreationsForge.DataValidationTests.Validation.Specs;
using CreationsForge.Specification.Validation.Specs.MiscItem;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.MiscItem.Skyrim;

public class SkyrimMiscItemSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "MISC")]
    [Trait("FormKey", "0D0756:Skyrim.esm")]
    [Trait("EditorID", "MGRDragonHeartScales")]
    [Trait("SpriggitFile", "MiscItems/MGRDragonHeartScales - 0D0756_Skyrim.esm.yaml")]
    public void Skyrim_MISC_ShouldMatchSpriggitSample_MGRDragonHeartScales()
    {
        var spec = MiscItemValidationSpecs.Skyrim_MGRDragonHeartScales();
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
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "MISC")]
    [Trait("FormKey", "06F993:Skyrim.esm")]
    [Trait("EditorID", "Firewood01")]
    [Trait("SpriggitFile", "MiscItems/Firewood01 - 06F993_Skyrim.esm.yaml")]
    public void Skyrim_MISC_ShouldMatchSpriggitSample_Firewood01()
    {
        var spec = MiscItemValidationSpecs.Skyrim_Firewood01();
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
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "MISC")]
    [Trait("FormKey", "0D4BE7:Skyrim.esm")]
    [Trait("EditorID", "FoxPeltSnow")]
    [Trait("SpriggitFile", "MiscItems/FoxPeltSnow - 0D4BE7_Skyrim.esm.yaml")]
    public void Skyrim_MISC_ShouldMatchSpriggitSample_FoxPeltSnow()
    {
        var spec = MiscItemValidationSpecs.Skyrim_FoxPeltSnow();
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
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "MISC")]
    [Trait("FormKey", "02996F:Skyrim.esm")]
    [Trait("EditorID", "C04HagravenHead")]
    [Trait("SpriggitFile", "MiscItems/C04HagravenHead - 02996F_Skyrim.esm.yaml")]
    public void Skyrim_MISC_ShouldMatchSpriggitSample_C04HagravenHead()
    {
        var spec = MiscItemValidationSpecs.Skyrim_C04HagravenHead();
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
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "MISC")]
    [Trait("FormKey", "0B08C7:Skyrim.esm")]
    [Trait("EditorID", "dunUniqueBeeInJar")]
    [Trait("SpriggitFile", "MiscItems/dunUniqueBeeInJar - 0B08C7_Skyrim.esm.yaml")]
    public void Skyrim_MISC_ShouldMatchSpriggitSample_dunUniqueBeeInJar()
    {
        var spec = MiscItemValidationSpecs.Skyrim_dunUniqueBeeInJar();
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
