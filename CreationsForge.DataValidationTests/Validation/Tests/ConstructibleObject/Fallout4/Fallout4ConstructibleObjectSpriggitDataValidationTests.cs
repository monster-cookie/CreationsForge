using CreationsForge.Core.DTOs.Records;
using CreationsForge.DataValidationTests.Validation.Specs;
using CreationsForge.DataValidationTests.Validation.Specs.ConstructibleObject;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.ConstructibleObject.Fallout4;

public class Fallout4ConstructibleObjectSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "COBJ")]
    [Trait("FormKey", "0ADF6E:Fallout4.esm")]
    [Trait("EditorID", "workshop_co_Artillery")]
    [Trait("SpriggitFile", "ConstructibleObjects/workshop_co_Artillery - 0ADF6E_Fallout4.esm.yaml")]
    public void Fallout4_COBJ_ShouldMatchSpriggitSample_workshop_co_Artillery()
    {
        var spec = ConstructibleObjectValidationSpecs.Fallout4_workshop_co_Artillery();
        var dto = Helpers.GetDTO<ConstructibleObjectDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "COBJ")]
    [Trait("FormKey", "0CEA6F:Fallout4.esm")]
    [Trait("EditorID", "workshop_co_MQ206BeamEmitter")]
    [Trait("SpriggitFile", "ConstructibleObjects/workshop_co_MQ206BeamEmitter - 0CEA6F_Fallout4.esm.yaml")]
    public void Fallout4_COBJ_ShouldMatchSpriggitSample_workshop_co_MQ206BeamEmitter()
    {
        var spec = ConstructibleObjectValidationSpecs.Fallout4_workshop_co_MQ206BeamEmitter();
        var dto = Helpers.GetDTO<ConstructibleObjectDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "COBJ")]
    [Trait("FormKey", "0CEA7B:Fallout4.esm")]
    [Trait("EditorID", "workshop_co_MQ206Console")]
    [Trait("SpriggitFile", "ConstructibleObjects/workshop_co_MQ206Console - 0CEA7B_Fallout4.esm.yaml")]
    public void Fallout4_COBJ_ShouldMatchSpriggitSample_workshop_co_MQ206Console()
    {
        var spec = ConstructibleObjectValidationSpecs.Fallout4_workshop_co_MQ206Console();
        var dto = Helpers.GetDTO<ConstructibleObjectDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "COBJ")]
    [Trait("FormKey", "05A0CD:Fallout4.esm")]
    [Trait("EditorID", "workshop_co_WaterPurifier")]
    [Trait("SpriggitFile", "ConstructibleObjects/workshop_co_WaterPurifier - 05A0CD_Fallout4.esm.yaml")]
    public void Fallout4_COBJ_ShouldMatchSpriggitSample_workshop_co_WaterPurifier()
    {
        var spec = ConstructibleObjectValidationSpecs.Fallout4_workshop_co_WaterPurifier();
        var dto = Helpers.GetDTO<ConstructibleObjectDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "COBJ")]
    [Trait("FormKey", "1889E3:Fallout4.esm")]
    [Trait("EditorID", "co_mod_GatlingLaser_BarrelMingunLaser_Super")]
    [Trait("SpriggitFile", "ConstructibleObjects/co_mod_GatlingLaser_BarrelMingunLaser_Super - 1889E3_Fallout4.esm.yaml")]
    public void Fallout4_COBJ_ShouldMatchSpriggitSample_co_mod_GatlingLaser_BarrelMingunLaser_Super()
    {
        var spec = ConstructibleObjectValidationSpecs.Fallout4_co_mod_GatlingLaser_BarrelMingunLaser_Super();
        var dto = Helpers.GetDTO<ConstructibleObjectDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }
}
