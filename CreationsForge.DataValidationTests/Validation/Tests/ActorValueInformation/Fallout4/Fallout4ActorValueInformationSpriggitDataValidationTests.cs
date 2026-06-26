using CreationsForge.Core.DTOs.Records;
using CreationsForge.DataValidationTests.Validation.Specs;
using CreationsForge.DataValidationTests.Validation.Specs.ActorValueInformation;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.ActorValueInformation.Fallout4;

public class Fallout4ActorValueInformationSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "AVIF")]
    [Trait("FormKey", "0B287B:Fallout4.esm")]
    [Trait("EditorID", "SentryBotMaxHeatLevel")]
    [Trait("SpriggitFile", "ActorValueInformation/SentryBotMaxHeatLevel - 0B287B_Fallout4.esm.yaml")]
    public void Fallout4_AVIF_ShouldMatchSpriggitSample_SentryBotMaxHeatLevel()
    {
        var spec = ActorValueInformationValidationSpecs.Fallout4_SentryBotMaxHeatLevel();
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
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "AVIF")]
    [Trait("FormKey", "00080F:Fallout4.esm")]
    [Trait("EditorID", "HC_Adrenaline")]
    [Trait("SpriggitFile", "ActorValueInformation/HC_Adrenaline - 00080F_Fallout4.esm.yaml")]
    public void Fallout4_AVIF_ShouldMatchSpriggitSample_HC_Adrenaline()
    {
        var spec = ActorValueInformationValidationSpecs.Fallout4_HC_Adrenaline();
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
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "AVIF")]
    [Trait("FormKey", "1B88D8:Fallout4.esm")]
    [Trait("EditorID", "Incendiary")]
    [Trait("SpriggitFile", "ActorValueInformation/Incendiary - 1B88D8_Fallout4.esm.yaml")]
    public void Fallout4_AVIF_ShouldMatchSpriggitSample_Incendiary()
    {
        var spec = ActorValueInformationValidationSpecs.Fallout4_Incendiary();
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
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "AVIF")]
    [Trait("FormKey", "0002C7:Fallout4.esm")]
    [Trait("EditorID", "Agility")]
    [Trait("SpriggitFile", "ActorValueInformation/Agility - 0002C7_Fallout4.esm.yaml")]
    public void Fallout4_AVIF_ShouldMatchSpriggitSample_Agility()
    {
        var spec = ActorValueInformationValidationSpecs.Fallout4_Agility();
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
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "AVIF")]
    [Trait("FormKey", "1EB998:Fallout4.esm")]
    [Trait("EditorID", "AddictionCount")]
    [Trait("SpriggitFile", "ActorValueInformation/AddictionCount - 1EB998_Fallout4.esm.yaml")]
    public void Fallout4_AVIF_ShouldMatchSpriggitSample_AddictionCount()
    {
        var spec = ActorValueInformationValidationSpecs.Fallout4_AddictionCount();
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
