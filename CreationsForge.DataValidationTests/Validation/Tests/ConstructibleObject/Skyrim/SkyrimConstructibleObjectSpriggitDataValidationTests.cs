using CreationsForge.Core.DTOs.Records;
using CreationsForge.DataValidationTests.Validation.Specs;
using CreationsForge.Specification.Validation.Specs.ConstructibleObject;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.ConstructibleObject.Skyrim;

public class SkyrimConstructibleObjectSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "COBJ")]
    [Trait("FormKey", "0DCA13:Skyrim.esm")]
    [Trait("EditorID", "RecipeArmorDragonscaleBoots")]
    [Trait("SpriggitFile", "ConstructibleObjects/RecipeArmorDragonscaleBoots - 0DCA13_Skyrim.esm.yaml")]
    public void Skyrim_COBJ_ShouldMatchSpriggitSample_RecipeArmorDragonscaleBoots()
    {
        var spec = ConstructibleObjectValidationSpecs.Skyrim_RecipeArmorDragonscaleBoots();
        var dto = Helpers.GetDTO<ConstructibleObjectDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "COBJ")]
    [Trait("FormKey", "0DCA14:Skyrim.esm")]
    [Trait("EditorID", "RecipeArmorDragonscaleCuirass")]
    [Trait("SpriggitFile", "ConstructibleObjects/RecipeArmorDragonscaleCuirass - 0DCA14_Skyrim.esm.yaml")]
    public void Skyrim_COBJ_ShouldMatchSpriggitSample_RecipeArmorDragonscaleCuirass()
    {
        var spec = ConstructibleObjectValidationSpecs.Skyrim_RecipeArmorDragonscaleCuirass();
        var dto = Helpers.GetDTO<ConstructibleObjectDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "COBJ")]
    [Trait("FormKey", "0DCA15:Skyrim.esm")]
    [Trait("EditorID", "RecipeArmorDragonscaleGauntlets")]
    [Trait("SpriggitFile", "ConstructibleObjects/RecipeArmorDragonscaleGauntlets - 0DCA15_Skyrim.esm.yaml")]
    public void Skyrim_COBJ_ShouldMatchSpriggitSample_RecipeArmorDragonscaleGauntlets()
    {
        var spec = ConstructibleObjectValidationSpecs.Skyrim_RecipeArmorDragonscaleGauntlets();
        var dto = Helpers.GetDTO<ConstructibleObjectDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "COBJ")]
    [Trait("FormKey", "0DD982:Skyrim.esm")]
    [Trait("EditorID", "RecipeArmorSteelPlateShield")]
    [Trait("SpriggitFile", "ConstructibleObjects/RecipeArmorSteelPlateShield - 0DD982_Skyrim.esm.yaml")]
    public void Skyrim_COBJ_ShouldMatchSpriggitSample_RecipeArmorSteelPlateShield()
    {
        var spec = ConstructibleObjectValidationSpecs.Skyrim_RecipeArmorSteelPlateShield();
        var dto = Helpers.GetDTO<ConstructibleObjectDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Skyrim")]
    [Trait("RecordType", "COBJ")]
    [Trait("FormKey", "0F431A:Skyrim.esm")]
    [Trait("EditorID", "RecipeFoodSoupCabbagePotato")]
    [Trait("SpriggitFile", "ConstructibleObjects/RecipeFoodSoupCabbagePotato - 0F431A_Skyrim.esm.yaml")]
    public void Skyrim_COBJ_ShouldMatchSpriggitSample_RecipeFoodSoupCabbagePotato()
    {
        var spec = ConstructibleObjectValidationSpecs.Skyrim_RecipeFoodSoupCabbagePotato();
        var dto = Helpers.GetDTO<ConstructibleObjectDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }
}
