using CreationsForge.Core.DTOs.Records;
using CreationsForge.DataValidationTests.Validation.Specs;
using CreationsForge.Specification.Validation.Specs.FormList;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.FormList.Fallout4;

public class Fallout4FormListSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "FLST")]
    [Trait("FormKey", "246EE7:Fallout4.esm")]
    [Trait("EditorID", "CA_JunkItems")]
    [Trait("SpriggitFile", "FormLists/CA_JunkItems - 246EE7_Fallout4.esm.yaml")]
    public void Fallout4_FLST_ShouldMatchSpriggitSample_CA_JunkItems()
    {
        var spec = FormListValidationSpecs.Fallout4_CA_JunkItems();
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
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "FLST")]
    [Trait("FormKey", "1A4AE8:Fallout4.esm")]
    [Trait("EditorID", "ChargenOptionsSortList")]
    [Trait("SpriggitFile", "FormLists/ChargenOptionsSortList - 1A4AE8_Fallout4.esm.yaml")]
    public void Fallout4_FLST_ShouldMatchSpriggitSample_ChargenOptionsSortList()
    {
        var spec = FormListValidationSpecs.Fallout4_ChargenOptionsSortList();
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
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "FLST")]
    [Trait("FormKey", "2494E7:Fallout4.esm")]
    [Trait("EditorID", "CompanionCrime__Common")]
    [Trait("SpriggitFile", "FormLists/CompanionCrime__Common - 2494E7_Fallout4.esm.yaml")]
    public void Fallout4_FLST_ShouldMatchSpriggitSample_CompanionCrime__Common()
    {
        var spec = FormListValidationSpecs.Fallout4_CompanionCrime__Common();
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
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "FLST")]
    [Trait("FormKey", "14EC02:Fallout4.esm")]
    [Trait("EditorID", "VoicesEmpty")]
    [Trait("SpriggitFile", "FormLists/VoicesEmpty - 14EC02_Fallout4.esm.yaml")]
    public void Fallout4_FLST_ShouldMatchSpriggitSample_VoicesEmpty()
    {
        var spec = FormListValidationSpecs.Fallout4_VoicesEmpty();
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
