using CreationsForge.Core.DTOs.Records;
using CreationsForge.DataValidationTests.Validation.Specs;
using CreationsForge.Specification.Validation.Specs.Global;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.Global.Skyrim;

public class SkyrimGlobalSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "GLOB")]
    [Trait("FormKey", "10636A:Skyrim.esm")]
    [Trait("EditorID", "1stPKillCam")]
    [Trait("SpriggitFile", "Globals/1stPKillCam - 10636A_Skyrim.esm.yaml")]
    public void Skyrim_GLOB_ShouldMatchSpriggitSample_1stPKillCam()
    {
        var spec = GlobalValidationSpecs.Skyrim_1stPKillCam();
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
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "GLOB")]
    [Trait("FormKey", "050765:Skyrim.esm")]
    [Trait("EditorID", "CarriageCost")]
    [Trait("SpriggitFile", "Globals/CarriageCost - 050765_Skyrim.esm.yaml")]
    public void Skyrim_GLOB_ShouldMatchSpriggitSample_CarriageCost()
    {
        var spec = GlobalValidationSpecs.Skyrim_CarriageCost();
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
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "GLOB")]
    [Trait("FormKey", "107702:Skyrim.esm")]
    [Trait("EditorID", "CarriageCostSmall")]
    [Trait("SpriggitFile", "Globals/CarriageCostSmall - 107702_Skyrim.esm.yaml")]
    public void Skyrim_GLOB_ShouldMatchSpriggitSample_CarriageCostSmall()
    {
        var spec = GlobalValidationSpecs.Skyrim_CarriageCostSmall();
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
