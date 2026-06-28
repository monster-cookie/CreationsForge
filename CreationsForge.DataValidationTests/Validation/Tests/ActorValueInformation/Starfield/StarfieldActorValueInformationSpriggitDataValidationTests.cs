using CreationsForge.Core.DTOs.Records;
using CreationsForge.DataValidationTests.Validation.Specs;
using CreationsForge.Specification.Validation.Specs.ActorValueInformation;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.ActorValueInformation.Starfield;

public class StarfieldActorValueInformationSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "AVIF")]
    [Trait("FormKey", "05ACD4:Starfield.esm")]
    [Trait("EditorID", "TargetingModeActionPoints_AV")]
    [Trait("SpriggitFile", "ActorValueInformation/TargetingModeActionPoints_AV - 05ACD4_Starfield.esm.yaml")]
    public void Starfield_AVIF_ShouldMatchSpriggitSample_TargetingModeActionPoints_AV()
    {
        var spec = ActorValueInformationValidationSpecs.Starfield_TargetingModeActionPoints_AV();
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
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "AVIF")]
    [Trait("FormKey", "248D31:Starfield.esm")]
    [Trait("EditorID", "ENV_Resist_Airborne")]
    [Trait("SpriggitFile", "ActorValueInformation/ENV_Resist_Airborne - 248D31_Starfield.esm.yaml")]
    public void Starfield_AVIF_ShouldMatchSpriggitSample_ENV_Resist_Airborne()
    {
        var spec = ActorValueInformationValidationSpecs.Starfield_ENV_Resist_Airborne();
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
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "AVIF")]
    [Trait("FormKey", "248D30:Starfield.esm")]
    [Trait("EditorID", "ENV_Resist_Corrosive")]
    [Trait("SpriggitFile", "ActorValueInformation/ENV_Resist_Corrosive - 248D30_Starfield.esm.yaml")]
    public void Starfield_AVIF_ShouldMatchSpriggitSample_ENV_Resist_Corrosive()
    {
        var spec = ActorValueInformationValidationSpecs.Starfield_ENV_Resist_Corrosive();
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
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "AVIF")]
    [Trait("FormKey", "2EE0BB:Starfield.esm")]
    [Trait("EditorID", "PEO_CarryWeight")]
    [Trait("SpriggitFile", "ActorValueInformation/PEO_CarryWeight - 2EE0BB_Starfield.esm.yaml")]
    public void Starfield_AVIF_ShouldMatchSpriggitSample_PEO_CarryWeight()
    {
        var spec = ActorValueInformationValidationSpecs.Starfield_PEO_CarryWeight();
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
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "AVIF")]
    [Trait("FormKey", "0002D4:Starfield.esm")]
    [Trait("EditorID", "Health")]
    [Trait("SpriggitFile", "ActorValueInformation/Health - 0002D4_Starfield.esm.yaml")]
    public void Starfield_AVIF_ShouldMatchSpriggitSample_Health()
    {
        var spec = ActorValueInformationValidationSpecs.Starfield_Health();
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
