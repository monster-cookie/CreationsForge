using CreationsForge.Core.DTOs.Records;
using CreationsForge.DataValidationTests.Validation.Specs;
using CreationsForge.Specification.Validation.Specs.FormList;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.FormList.Skyrim;

public class SkyrimFormListSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "FLST")]
    [Trait("FormKey", "06F3F7:Skyrim.esm")]
    [Trait("EditorID", "AAAMothPlantTypes")]
    [Trait("SpriggitFile", "FormLists/AAAMothPlantTypes - 06F3F7_Skyrim.esm.yaml")]
    public void Skyrim_FLST_ShouldMatchSpriggitSample_AAAMothPlantTypes()
    {
        var spec = FormListValidationSpecs.Skyrim_AAAMothPlantTypes();
        var dto = Helpers.GetDTO<FormListDTO>(spec.Game, spec.RecordType, spec.FormKey);
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
    [Trait("RecordType", "FLST")]
    [Trait("FormKey", "045C32:Skyrim.esm")]
    [Trait("EditorID", "CityWindhelmResidentList")]
    [Trait("SpriggitFile", "FormLists/CityWindhelmResidentList - 045C32_Skyrim.esm.yaml")]
    public void Skyrim_FLST_ShouldMatchSpriggitSample_CityWindhelmResidentList()
    {
        var spec = FormListValidationSpecs.Skyrim_CityWindhelmResidentList();
        var dto = Helpers.GetDTO<FormListDTO>(spec.Game, spec.RecordType, spec.FormKey);
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
    [Trait("RecordType", "FLST")]
    [Trait("FormKey", "026953:Skyrim.esm")]
    [Trait("EditorID", "CrimeFactionsList")]
    [Trait("SpriggitFile", "FormLists/CrimeFactionsList - 026953_Skyrim.esm.yaml")]
    public void Skyrim_FLST_ShouldMatchSpriggitSample_CrimeFactionsList()
    {
        var spec = FormListValidationSpecs.Skyrim_CrimeFactionsList();
        var dto = Helpers.GetDTO<FormListDTO>(spec.Game, spec.RecordType, spec.FormKey);
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
    [Trait("RecordType", "FLST")]
    [Trait("FormKey", "000D14:Skyrim.esm")]
    [Trait("EditorID", "DraugrWeapons")]
    [Trait("SpriggitFile", "FormLists/DraugrWeapons - 000D14_Skyrim.esm.yaml")]
    public void Skyrim_FLST_ShouldMatchSpriggitSample_DraugrWeapons()
    {
        var spec = FormListValidationSpecs.Skyrim_DraugrWeapons();
        var dto = Helpers.GetDTO<FormListDTO>(spec.Game, spec.RecordType, spec.FormKey);
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
