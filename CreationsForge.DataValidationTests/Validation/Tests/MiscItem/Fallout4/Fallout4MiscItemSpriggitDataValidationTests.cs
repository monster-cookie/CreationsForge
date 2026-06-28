using CreationsForge.Core.DTOs.Records;
using CreationsForge.DataValidationTests.Validation.Specs;
using CreationsForge.Specification.Validation.Specs.MiscItem;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.MiscItem.Fallout4;

public class Fallout4MiscItemSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "MISC")]
    [Trait("FormKey", "247E7F:Fallout4.esm")]
    [Trait("EditorID", "Debug_Components")]
    [Trait("SpriggitFile", "MiscItems/Debug_Components - 247E7F_Fallout4.esm.yaml")]
    public void Fallout4_MISC_ShouldMatchSpriggitSample_Debug_Components()
    {
        var spec = MiscItemValidationSpecs.Fallout4_Debug_Components();
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
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "MISC")]
    [Trait("FormKey", "0A4754:Fallout4.esm")]
    [Trait("EditorID", "FFDiamondCity07Paper")]
    [Trait("SpriggitFile", "MiscItems/FFDiamondCity07Paper - 0A4754_Fallout4.esm.yaml")]
    public void Fallout4_MISC_ShouldMatchSpriggitSample_FFDiamondCity07Paper()
    {
        var spec = MiscItemValidationSpecs.Fallout4_FFDiamondCity07Paper();
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
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "MISC")]
    [Trait("FormKey", "01F8F9:Fallout4.esm")]
    [Trait("EditorID", "FireExtinguisher01")]
    [Trait("SpriggitFile", "MiscItems/FireExtinguisher01 - 01F8F9_Fallout4.esm.yaml")]
    public void Fallout4_MISC_ShouldMatchSpriggitSample_FireExtinguisher01()
    {
        var spec = MiscItemValidationSpecs.Fallout4_FireExtinguisher01();
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
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "MISC")]
    [Trait("FormKey", "178B51:Fallout4.esm")]
    [Trait("EditorID", "BobbleHead_Agility")]
    [Trait("SpriggitFile", "MiscItems/BobbleHead_Agility - 178B51_Fallout4.esm.yaml")]
    public void Fallout4_MISC_ShouldMatchSpriggitSample_BobbleHead_Agility()
    {
        var spec = MiscItemValidationSpecs.Fallout4_BobbleHead_Agility();
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
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "MISC")]
    [Trait("FormKey", "04E3A2:Fallout4.esm")]
    [Trait("EditorID", "MS11GuidanceChip")]
    [Trait("SpriggitFile", "MiscItems/MS11GuidanceChip - 04E3A2_Fallout4.esm.yaml")]
    public void Fallout4_MISC_ShouldMatchSpriggitSample_MS11GuidanceChip()
    {
        var spec = MiscItemValidationSpecs.Fallout4_MS11GuidanceChip();
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
