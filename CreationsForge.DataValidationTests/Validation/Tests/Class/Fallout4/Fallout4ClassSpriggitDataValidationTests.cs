using CreationsForge.Core.DTOs.Records;
using CreationsForge.DataValidationTests.Validation.Specs;
using CreationsForge.DataValidationTests.Validation.Specs.Class;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.Class.Fallout4;

public class Fallout4ClassSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "CLAS")]
    [Trait("FormKey", "1CD0A8:Fallout4.esm")]
    [Trait("EditorID", "ZeroSPECIALclass")]
    [Trait("SpriggitFile", "Classes/ZeroSPECIALclass - 1CD0A8_Fallout4.esm.yaml")]
    public void Fallout4_CLAS_ShouldMatchSpriggitSample_ZeroSPECIALclass()
    {
        var spec = ClassValidationSpecs.Fallout4_ZeroSPECIALclass();
        var dto = Helpers.GetDTO<ClassDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "CLAS")]
    [Trait("FormKey", "01326B:Fallout4.esm")]
    [Trait("EditorID", "Citizen")]
    [Trait("SpriggitFile", "Classes/Citizen - 01326B_Fallout4.esm.yaml")]
    public void Fallout4_CLAS_ShouldMatchSpriggitSample_Citizen()
    {
        var spec = ClassValidationSpecs.Fallout4_Citizen();
        var dto = Helpers.GetDTO<ClassDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "CLAS")]
    [Trait("FormKey", "031757:Fallout4.esm")]
    [Trait("EditorID", "BloatflyClass")]
    [Trait("SpriggitFile", "Classes/BloatflyClass - 031757_Fallout4.esm.yaml")]
    public void Fallout4_CLAS_ShouldMatchSpriggitSample_BloatflyClass()
    {
        var spec = ClassValidationSpecs.Fallout4_BloatflyClass();
        var dto = Helpers.GetDTO<ClassDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "CLAS")]
    [Trait("FormKey", "20ED07:Fallout4.esm")]
    [Trait("EditorID", "MQ203Class")]
    [Trait("SpriggitFile", "Classes/MQ203Class - 20ED07_Fallout4.esm.yaml")]
    public void Fallout4_CLAS_ShouldMatchSpriggitSample_MQ203Class()
    {
        var spec = ClassValidationSpecs.Fallout4_MQ203Class();
        var dto = Helpers.GetDTO<ClassDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }
}
