using CreationsForge.Core.DTOs.Records;
using CreationsForge.DataValidationTests.Validation.Specs;
using CreationsForge.DataValidationTests.Validation.Specs.FormList;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.FormList.Starfield;

public class StarfieldFormListSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "FLST")]
    [Trait("FormKey", "2117E6:Starfield.esm")]
    [Trait("EditorID", "AkilaVendorVeryHighOrganicResources")]
    [Trait("SpriggitFile", "FormLists/AkilaVendorVeryHighOrganicResources - 2117E6_Starfield.esm.yaml")]
    public void Starfield_FLST_ShouldMatchSpriggitSample_AkilaVendorVeryHighOrganicResources()
    {
        var spec = FormListValidationSpecs.Starfield_AkilaVendorVeryHighOrganicResources();
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
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "FLST")]
    [Trait("FormKey", "2117EC:Starfield.esm")]
    [Trait("EditorID", "AkilaVendorVeryLowOrganicResources")]
    [Trait("SpriggitFile", "FormLists/AkilaVendorVeryLowOrganicResources - 2117EC_Starfield.esm.yaml")]
    public void Starfield_FLST_ShouldMatchSpriggitSample_AkilaVendorVeryLowOrganicResources()
    {
        var spec = FormListValidationSpecs.Starfield_AkilaVendorVeryLowOrganicResources();
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
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "FLST")]
    [Trait("FormKey", "2117F0:Starfield.esm")]
    [Trait("EditorID", "AlikaVendorLowOrganicResources")]
    [Trait("SpriggitFile", "FormLists/AlikaVendorLowOrganicResources - 2117F0_Starfield.esm.yaml")]
    public void Starfield_FLST_ShouldMatchSpriggitSample_AlikaVendorLowOrganicResources()
    {
        var spec = FormListValidationSpecs.Starfield_AlikaVendorLowOrganicResources();
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
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "FLST")]
    [Trait("FormKey", "0C3830:Starfield.esm")]
    [Trait("EditorID", "COND_imgui_1_Assorted")]
    [Trait("SpriggitFile", "FormLists/COND_imgui_1_Assorted - 0C3830_Starfield.esm.yaml")]
    public void Starfield_FLST_ShouldMatchSpriggitSample_COND_imgui_1_Assorted()
    {
        var spec = FormListValidationSpecs.Starfield_COND_imgui_1_Assorted();
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
