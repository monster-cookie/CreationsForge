using CreationsForge.Core.DTOs.Records;
using CreationsForge.DataValidationTests.Validation.Specs;
using CreationsForge.Specification.Validation.Specs.ConstructibleObject;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.ConstructibleObject.Starfield;

public class StarfieldConstructibleObjectSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "COBJ")]
    [Trait("FormKey", "007F7C:Starfield.esm")]
    [Trait("EditorID", "co_Outpost_Power_Reactor01")]
    [Trait("SpriggitFile", "ConstructibleObjects/co_Outpost_Power_Reactor01 - 007F7C_Starfield.esm.yaml")]
    public void Starfield_COBJ_ShouldMatchSpriggitSample_co_Outpost_Power_Reactor01()
    {
        var spec = ConstructibleObjectValidationSpecs.Starfield_co_Outpost_Power_Reactor01();
        var dto = Helpers.GetDTO<ConstructibleObjectDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "COBJ")]
    [Trait("FormKey", "1C5144:Starfield.esm")]
    [Trait("EditorID", "co_Outpost_Power_Reactor02")]
    [Trait("SpriggitFile", "ConstructibleObjects/co_Outpost_Power_Reactor02 - 1C5144_Starfield.esm.yaml")]
    public void Starfield_COBJ_ShouldMatchSpriggitSample_co_Outpost_Power_Reactor02()
    {
        var spec = ConstructibleObjectValidationSpecs.Starfield_co_Outpost_Power_Reactor02();
        var dto = Helpers.GetDTO<ConstructibleObjectDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "COBJ")]
    [Trait("FormKey", "0C8720:Starfield.esm")]
    [Trait("EditorID", "co_Chem_XenoAurora")]
    [Trait("SpriggitFile", "ConstructibleObjects/co_Chem_XenoAurora - 0C8720_Starfield.esm.yaml")]
    public void Starfield_COBJ_ShouldMatchSpriggitSample_co_Chem_XenoAurora()
    {
        var spec = ConstructibleObjectValidationSpecs.Starfield_co_Chem_XenoAurora();
        var dto = Helpers.GetDTO<ConstructibleObjectDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "COBJ")]
    [Trait("FormKey", "09DE67:Starfield.esm")]
    [Trait("EditorID", "UC07_co_mfg_MicroCell_Old")]
    [Trait("SpriggitFile", "ConstructibleObjects/UC07_co_mfg_MicroCell_Old - 09DE67_Starfield.esm.yaml")]
    public void Starfield_COBJ_ShouldMatchSpriggitSample_UC07_co_mfg_MicroCell_Old()
    {
        var spec = ConstructibleObjectValidationSpecs.Starfield_UC07_co_mfg_MicroCell_Old();
        var dto = Helpers.GetDTO<ConstructibleObjectDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "COBJ")]
    [Trait("FormKey", "1DF844:Starfield.esm")]
    [Trait("EditorID", "co_Outpost_Misc_MissionBoardConsole")]
    [Trait("SpriggitFile", "ConstructibleObjects/co_Outpost_Misc_MissionBoardConsole - 1DF844_Starfield.esm.yaml")]
    public void Starfield_COBJ_ShouldMatchSpriggitSample_co_Outpost_Misc_MissionBoardConsole()
    {
        var spec = ConstructibleObjectValidationSpecs.Starfield_co_Outpost_Misc_MissionBoardConsole();
        var dto = Helpers.GetDTO<ConstructibleObjectDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }
}
