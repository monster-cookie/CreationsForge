using CreationsForge.Core.DTOs.Records;
using CreationsForge.DataValidationTests.Validation.Specs;
using CreationsForge.Specification.Validation.Specs.Global;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.Global.Fallout4;

public class Fallout4GlobalSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "GLOB")]
    [Trait("FormKey", "18E889:Fallout4.esm")]
    [Trait("EditorID", "AO_Companion_Search_JunkThresholdValue")]
    [Trait("SpriggitFile", "Globals/AO_Companion_Search_JunkThresholdValue - 18E889_Fallout4.esm.yaml")]
    public void Fallout4_GLOB_ShouldMatchSpriggitSample_AO_Companion_Search_JunkThresholdValue()
    {
        var spec = GlobalValidationSpecs.Fallout4_AO_Companion_Search_JunkThresholdValue();
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
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "GLOB")]
    [Trait("FormKey", "176107:Fallout4.esm")]
    [Trait("EditorID", "AO_Companion_Search_NextAllowedDaysUntil")]
    [Trait("SpriggitFile", "Globals/AO_Companion_Search_NextAllowedDaysUntil - 176107_Fallout4.esm.yaml")]
    public void Fallout4_GLOB_ShouldMatchSpriggitSample_AO_Companion_Search_NextAllowedDaysUntil()
    {
        var spec = GlobalValidationSpecs.Fallout4_AO_Companion_Search_NextAllowedDaysUntil();
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
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "GLOB")]
    [Trait("FormKey", "043F14:Fallout4.esm")]
    [Trait("EditorID", "AO_Dogmeat_Container_Bailout_Dist")]
    [Trait("SpriggitFile", "Globals/AO_Dogmeat_Container_Bailout_Dist - 043F14_Fallout4.esm.yaml")]
    public void Fallout4_GLOB_ShouldMatchSpriggitSample_AO_Dogmeat_Container_Bailout_Dist()
    {
        var spec = GlobalValidationSpecs.Fallout4_AO_Dogmeat_Container_Bailout_Dist();
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
