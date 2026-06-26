using CreationsForge.Core.DTOs.Records;
using CreationsForge.DataValidationTests.Validation.Specs;
using CreationsForge.DataValidationTests.Validation.Specs.Container;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.Container.Fallout4;

public class Fallout4ContainerSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "CONT")]
    [Trait("FormKey", "1F2B6A:Fallout4.esm")]
    [Trait("EditorID", "DN054Loot_Prewar_Safe")]
    [Trait("SpriggitFile", "Containers/DN054Loot_Prewar_Safe - 1F2B6A_Fallout4.esm.yaml")]
    public void Fallout4_CONT_ShouldMatchSpriggitSample_DN054Loot_Prewar_Safe()
    {
        var spec = ContainerValidationSpecs.Fallout4_DN054Loot_Prewar_Safe();
        var dto = Helpers.GetDTO<ContainerDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "CONT")]
    [Trait("FormKey", "064A36:Fallout4.esm")]
    [Trait("EditorID", "Loot_Raider_Safe")]
    [Trait("SpriggitFile", "Containers/Loot_Raider_Safe - 064A36_Fallout4.esm.yaml")]
    public void Fallout4_CONT_ShouldMatchSpriggitSample_Loot_Raider_Safe()
    {
        var spec = ContainerValidationSpecs.Fallout4_Loot_Raider_Safe();
        var dto = Helpers.GetDTO<ContainerDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "CONT")]
    [Trait("FormKey", "1C0292:Fallout4.esm")]
    [Trait("EditorID", "TheaterTickerTape_Safe")]
    [Trait("SpriggitFile", "Containers/TheaterTickerTape_Safe - 1C0292_Fallout4.esm.yaml")]
    public void Fallout4_CONT_ShouldMatchSpriggitSample_TheaterTickerTape_Safe()
    {
        var spec = ContainerValidationSpecs.Fallout4_TheaterTickerTape_Safe();
        var dto = Helpers.GetDTO<ContainerDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "CONT")]
    [Trait("FormKey", "06355F:Fallout4.esm")]
    [Trait("EditorID", "Loot_Trunk_Boss")]
    [Trait("SpriggitFile", "Containers/Loot_Trunk_Boss - 06355F_Fallout4.esm.yaml")]
    public void Fallout4_CONT_ShouldMatchSpriggitSample_Loot_Trunk_Boss()
    {
        var spec = ContainerValidationSpecs.Fallout4_Loot_Trunk_Boss();
        var dto = Helpers.GetDTO<ContainerDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Fallout4")]
    [Trait("RecordType", "CONT")]
    [Trait("FormKey", "11CB14:Fallout4.esm")]
    [Trait("EditorID", "DN123_SkylanesSecretCompartment")]
    [Trait("SpriggitFile", "Containers/DN123_SkylanesSecretCompartment - 11CB14_Fallout4.esm.yaml")]
    public void Fallout4_CONT_ShouldMatchSpriggitSample_DN123_SkylanesSecretCompartment()
    {
        var spec = ContainerValidationSpecs.Fallout4_DN123_SkylanesSecretCompartment();
        var dto = Helpers.GetDTO<ContainerDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }
}
