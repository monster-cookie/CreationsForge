using CreationsForge.Core.DTOs.Records;
using CreationsForge.DataValidationTests.Validation.Specs;
using CreationsForge.Specification.Validation.Specs.Global;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.Global.Starfield;

public class StarfieldGlobalSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "GLOB")]
    [Trait("FormKey", "20C81D:Starfield.esm")]
    [Trait("EditorID", "_UpdateShatteredSpaceMaster")]
    [Trait("SpriggitFile", "Globals/_UpdateShatteredSpaceMaster - 20C81D_Starfield.esm.yaml")]
    public void Starfield_GLOB_ShouldMatchSpriggitSample_UpdateShatteredSpaceMaster()
    {
        var spec = GlobalValidationSpecs.Starfield_UpdateShatteredSpaceMaster();
        var dto = Helpers.GetDTO<GlobalDTO>(spec.Game, spec.RecordType, spec.FormKey);
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
    [Trait("RecordType", "GLOB")]
    [Trait("FormKey", "2B7FBD:Starfield.esm")]
    [Trait("EditorID", "2B7FBD_Starfield.esm")]
    [Trait("SpriggitFile", "Globals/2B7FBD_Starfield.esm.yaml")]
    public void Starfield_GLOB_ShouldMatchSpriggitSample_2B7FBD_Starfield_esm()
    {
        var spec = GlobalValidationSpecs.Starfield_2B7FBD_Starfield_esm();
        var dto = Helpers.GetDTO<GlobalDTO>(spec.Game, spec.RecordType, spec.FormKey);
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
    [Trait("RecordType", "GLOB")]
    [Trait("FormKey", "2B91E0:Starfield.esm")]
    [Trait("EditorID", "2B91E0_Starfield.esm")]
    [Trait("SpriggitFile", "Globals/2B91E0_Starfield.esm.yaml")]
    public void Starfield_GLOB_ShouldMatchSpriggitSample_2B91E0_Starfield_esm()
    {
        var spec = GlobalValidationSpecs.Starfield_2B91E0_Starfield_esm();
        var dto = Helpers.GetDTO<GlobalDTO>(spec.Game, spec.RecordType, spec.FormKey);
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
