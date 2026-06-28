using CreationsForge.Core.DTOs.Records;
using CreationsForge.DataValidationTests.Validation.Specs;
using CreationsForge.Specification.Validation.Specs.Container;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.Container.Skyrim;

public class SkyrimContainerSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "CONT")]
    [Trait("FormKey", "02065B:Skyrim.esm")]
    [Trait("EditorID", "TreasFalmerChestBoss")]
    [Trait("SpriggitFile", "Containers/TreasFalmerChestBoss - 02065B_Skyrim.esm.yaml")]
    public void Skyrim_CONT_ShouldMatchSpriggitSample_TreasFalmerChestBoss()
    {
        var spec = ContainerValidationSpecs.Skyrim_TreasFalmerChestBoss();
        var dto = Helpers.GetDTO<ContainerDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "CONT")]
    [Trait("FormKey", "0B1176:Skyrim.esm")]
    [Trait("EditorID", "TreasFalmerChestBossDwarven")]
    [Trait("SpriggitFile", "Containers/TreasFalmerChestBossDwarven - 0B1176_Skyrim.esm.yaml")]
    public void Skyrim_CONT_ShouldMatchSpriggitSample_TreasFalmerChestBossDwarven()
    {
        var spec = ContainerValidationSpecs.Skyrim_TreasFalmerChestBossDwarven();
        var dto = Helpers.GetDTO<ContainerDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "CONT")]
    [Trait("FormKey", "020659:Skyrim.esm")]
    [Trait("EditorID", "TreasFalmerChest")]
    [Trait("SpriggitFile", "Containers/TreasFalmerChest - 020659_Skyrim.esm.yaml")]
    public void Skyrim_CONT_ShouldMatchSpriggitSample_TreasFalmerChest()
    {
        var spec = ContainerValidationSpecs.Skyrim_TreasFalmerChest();
        var dto = Helpers.GetDTO<ContainerDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "CONT")]
    [Trait("FormKey", "0A918C:Skyrim.esm")]
    [Trait("EditorID", "BeeHive")]
    [Trait("SpriggitFile", "Containers/BeeHive - 0A918C_Skyrim.esm.yaml")]
    public void Skyrim_CONT_ShouldMatchSpriggitSample_BeeHive()
    {
        var spec = ContainerValidationSpecs.Skyrim_BeeHive();
        var dto = Helpers.GetDTO<ContainerDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "CONT")]
    [Trait("FormKey", "07434B:Skyrim.esm")]
    [Trait("EditorID", "MerchantCaravanAChest")]
    [Trait("SpriggitFile", "Containers/MerchantCaravanAChest - 07434B_Skyrim.esm.yaml")]
    public void Skyrim_CONT_ShouldMatchSpriggitSample_MerchantCaravanAChest()
    {
        var spec = ContainerValidationSpecs.Skyrim_MerchantCaravanAChest();
        var dto = Helpers.GetDTO<ContainerDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }
}
