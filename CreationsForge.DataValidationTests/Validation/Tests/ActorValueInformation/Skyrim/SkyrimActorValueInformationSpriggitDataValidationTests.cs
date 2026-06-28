using CreationsForge.Core.DTOs.Records;
using CreationsForge.DataValidationTests.Validation.Specs;
using CreationsForge.Specification.Validation.Specs.ActorValueInformation;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.ActorValueInformation.Skyrim;

public class SkyrimActorValueInformationSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "AVIF")]
    [Trait("FormKey", "000456:Skyrim.esm")]
    [Trait("EditorID", "AVAlchemy")]
    [Trait("SpriggitFile", "ActorValueInformation/AVAlchemy - 000456_Skyrim.esm.yaml")]
    public void Skyrim_AVIF_ShouldMatchSpriggitSample_AVAlchemy()
    {
        var spec = ActorValueInformationValidationSpecs.Skyrim_AVAlchemy();
        var dto = Helpers.GetDTO<ActorValueInformationDTO>(spec.Game, spec.RecordType, spec.FormKey);
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
    [Trait("RecordType", "AVIF")]
    [Trait("FormKey", "000458:Skyrim.esm")]
    [Trait("EditorID", "AVAlteration")]
    [Trait("SpriggitFile", "ActorValueInformation/AVAlteration - 000458_Skyrim.esm.yaml")]
    public void Skyrim_AVIF_ShouldMatchSpriggitSample_AVAlteration()
    {
        var spec = ActorValueInformationValidationSpecs.Skyrim_AVAlteration();
        var dto = Helpers.GetDTO<ActorValueInformationDTO>(spec.Game, spec.RecordType, spec.FormKey);
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
    [Trait("RecordType", "AVIF")]
    [Trait("FormKey", "00044F:Skyrim.esm")]
    [Trait("EditorID", "AVBlock")]
    [Trait("SpriggitFile", "ActorValueInformation/AVBlock - 00044F_Skyrim.esm.yaml")]
    public void Skyrim_AVIF_ShouldMatchSpriggitSample_AVBlock()
    {
        var spec = ActorValueInformationValidationSpecs.Skyrim_AVBlock();
        var dto = Helpers.GetDTO<ActorValueInformationDTO>(spec.Game, spec.RecordType, spec.FormKey);
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
    [Trait("RecordType", "AVIF")]
    [Trait("FormKey", "0005F6:Skyrim.esm")]
    [Trait("EditorID", "AVFavorActive")]
    [Trait("SpriggitFile", "ActorValueInformation/AVFavorActive - 0005F6_Skyrim.esm.yaml")]
    public void Skyrim_AVIF_ShouldMatchSpriggitSample_AVFavorActive()
    {
        var spec = ActorValueInformationValidationSpecs.Skyrim_AVFavorActive();
        var dto = Helpers.GetDTO<ActorValueInformationDTO>(spec.Game, spec.RecordType, spec.FormKey);
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
