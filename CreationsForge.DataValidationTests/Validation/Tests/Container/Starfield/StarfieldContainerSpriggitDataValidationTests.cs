using CreationsForge.Core.DTOs.Records;
using CreationsForge.DataValidationTests.Validation.Specs;
using CreationsForge.DataValidationTests.Validation.Specs.Container;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.Container.Starfield;

public class StarfieldContainerSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "CONT")]
    [Trait("FormKey", "277A73:Starfield.esm")]
    [Trait("EditorID", "ShipOutpost_Loot_Storage_Safe_Floor_Reg_Common")]
    [Trait("SpriggitFile", "Containers/ShipOutpost_Loot_Storage_Safe_Floor_Reg_Common - 277A73_Starfield.esm.yaml")]
    public void Starfield_CONT_ShouldMatchSpriggitSample_ShipOutpost_Loot_Storage_Safe_Floor_Reg_Common()
    {
        var spec = ContainerValidationSpecs.Starfield_ShipOutpost_Loot_Storage_Safe_Floor_Reg_Common();
        var dto = Helpers.GetDTO<ContainerDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "CONT")]
    [Trait("FormKey", "277A81:Starfield.esm")]
    [Trait("EditorID", "ShipOutpost_Loot_Storage_Safe_Floor_Tall_Rare")]
    [Trait("SpriggitFile", "Containers/ShipOutpost_Loot_Storage_Safe_Floor_Tall_Rare - 277A81_Starfield.esm.yaml")]
    public void Starfield_CONT_ShouldMatchSpriggitSample_ShipOutpost_Loot_Storage_Safe_Floor_Tall_Rare()
    {
        var spec = ContainerValidationSpecs.Starfield_ShipOutpost_Loot_Storage_Safe_Floor_Tall_Rare();
        var dto = Helpers.GetDTO<ContainerDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "CONT")]
    [Trait("FormKey", "2779E9:Starfield.esm")]
    [Trait("EditorID", "ShipOutpost_Loot_Storage_BossChest_Industrial_Rare")]
    [Trait("SpriggitFile", "Containers/ShipOutpost_Loot_Storage_BossChest_Industrial_Rare - 2779E9_Starfield.esm.yaml")]
    public void Starfield_CONT_ShouldMatchSpriggitSample_ShipOutpost_Loot_Storage_BossChest_Industrial_Rare()
    {
        var spec = ContainerValidationSpecs.Starfield_ShipOutpost_Loot_Storage_BossChest_Industrial_Rare();
        var dto = Helpers.GetDTO<ContainerDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "CONT")]
    [Trait("FormKey", "1A23DF:Starfield.esm")]
    [Trait("EditorID", "Loot_Display_WeaponRack03_EMPTY")]
    [Trait("SpriggitFile", "Containers/Loot_Display_WeaponRack03_EMPTY - 1A23DF_Starfield.esm.yaml")]
    public void Starfield_CONT_ShouldMatchSpriggitSample_Loot_Display_WeaponRack03_EMPTY()
    {
        var spec = ContainerValidationSpecs.Starfield_Loot_Display_WeaponRack03_EMPTY();
        var dto = Helpers.GetDTO<ContainerDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "CONT")]
    [Trait("FormKey", "057C20:Starfield.esm")]
    [Trait("EditorID", "Loot_Display_ArboronWeaponRackPanel02")]
    [Trait("SpriggitFile", "Containers/Loot_Display_ArboronWeaponRackPanel02 - 057C20_Starfield.esm.yaml")]
    public void Starfield_CONT_ShouldMatchSpriggitSample_Loot_Display_ArboronWeaponRackPanel02()
    {
        var spec = ContainerValidationSpecs.Starfield_Loot_Display_ArboronWeaponRackPanel02();
        var dto = Helpers.GetDTO<ContainerDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }
}
