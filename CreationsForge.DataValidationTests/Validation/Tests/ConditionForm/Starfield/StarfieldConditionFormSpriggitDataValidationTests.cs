using CreationsForge.Core.DTOs.Records;
using CreationsForge.DataValidationTests.Validation.Specs;
using CreationsForge.Specification.Validation.Specs.ConditionForm;
using Shouldly;

namespace CreationsForge.DataValidationTests.Validation.Tests.ConditionForm.Starfield;

public class StarfieldConditionFormSpriggitDataValidationTests : SpriggitDataValidationTestBase
{
    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "CNDF")]
    [Trait("FormKey", "3C8F9C:Starfield.esm")]
    [Trait("EditorID", "DebugMoveToPlanetConditions_Trait")]
    [Trait("SpriggitFile", "ConditionRecords/DebugMoveToPlanetConditions_Trait - 3C8F9C_Starfield.esm.yaml")]
    public void Starfield_CNDF_ShouldMatchSpriggitSample_DebugMoveToPlanetConditions_Trait()
    {
        var spec = ConditionFormValidationSpecs.Starfield_DebugMoveToPlanetConditions_Trait();
        var dto = Helpers.GetDTO<ConditionFormDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "CNDF")]
    [Trait("FormKey", "31982F:Starfield.esm")]
    [Trait("EditorID", "SFBGS_CND_Placeholder01_ReservedForUse")]
    [Trait("SpriggitFile", "ConditionRecords/SFBGS_CND_Placeholder01_ReservedForUse - 31982F_Starfield.esm.yaml")]
    public void Starfield_CNDF_ShouldMatchSpriggitSample_SFBGS_CND_Placeholder01_ReservedForUse()
    {
        var spec = ConditionFormValidationSpecs.Starfield_SFBGS_CND_Placeholder01_ReservedForUse();
        var dto = Helpers.GetDTO<ConditionFormDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "CNDF")]
    [Trait("FormKey", "10460E:Starfield.esm")]
    [Trait("EditorID", "SQ_TreasureMap_CND_IsResourceLocation")]
    [Trait("SpriggitFile", "ConditionRecords/SQ_TreasureMap_CND_IsResourceLocation - 10460E_Starfield.esm.yaml")]
    public void Starfield_CNDF_ShouldMatchSpriggitSample_SQ_TreasureMap_CND_IsResourceLocation()
    {
        var spec = ConditionFormValidationSpecs.Starfield_SQ_TreasureMap_CND_IsResourceLocation();
        var dto = Helpers.GetDTO<ConditionFormDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }

    [Fact]
    [Trait("Game", "Starfield")]
    [Trait("RecordType", "CNDF")]
    [Trait("FormKey", "0B1206:Starfield.esm")]
    [Trait("EditorID", "ActorShouldShowSpacesuitGameplayFlashlight")]
    [Trait("SpriggitFile", "ConditionRecords/ActorShouldShowSpacesuitGameplayFlashlight - 0B1206_Starfield.esm.yaml")]
    public void Starfield_CNDF_ShouldMatchSpriggitSample_ActorShouldShowSpacesuitGameplayFlashlight()
    {
        var spec = ConditionFormValidationSpecs.Starfield_ActorShouldShowSpacesuitGameplayFlashlight();
        var dto = Helpers.GetDTO<ConditionFormDTO>(spec.Game, spec.RecordType, spec.FormKey);

        var assertions = ValidationSpecRunner.GetAssertionCases(spec, dto);
        foreach (var assertion in assertions)
        {
            assertion.Actual.ShouldBe(assertion.Expected, assertion.Message);
        }

        ValidationSpecRunner.GetCoverageDiagnostics(spec, dto).ShouldBeEmpty();
    }
}
