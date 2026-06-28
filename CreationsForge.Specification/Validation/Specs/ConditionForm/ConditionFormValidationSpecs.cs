using CreationsForge.Specification.Records;
using CreationsForge.Specification.Validation;

namespace CreationsForge.Specification.Validation.Specs.ConditionForm;

public static class ConditionFormValidationSpecs
{
    /// <summary>
    /// Builds the Starfield <c>DebugMoveToPlanetConditions_Trait</c> condition form validation spec,
    /// including a UI editor ID row expectation.
    /// </summary>
    /// <returns>The validation spec for the Starfield <c>DebugMoveToPlanetConditions_Trait</c> sample.</returns>
    public static ValidationSpec Starfield_DebugMoveToPlanetConditions_Trait()
    {
        var spec = StarfieldConditionForm("DebugMoveToPlanetConditions_Trait", "3C8F9C:Starfield.esm")
            .Build();
        spec.UiComparisonExpectations.Add(ValidationUiComparisonExpectation.Literal(["EditorID"], "DebugMoveToPlanetConditions_Trait"));
        return spec;
    }

    public static ValidationSpec Starfield_SFBGS_CND_Placeholder01_ReservedForUse()
    {
        return StarfieldConditionForm("SFBGS_CND_Placeholder01_ReservedForUse", "31982F:Starfield.esm")
            .Build();
    }

    /// <summary>
    /// Builds the Starfield <c>SQ_TreasureMap_CND_IsResourceLocation</c> condition form validation spec,
    /// including a UI editor ID row expectation.
    /// </summary>
    /// <returns>The validation spec for the Starfield <c>SQ_TreasureMap_CND_IsResourceLocation</c> sample.</returns>
    public static ValidationSpec Starfield_SQ_TreasureMap_CND_IsResourceLocation()
    {
        var spec = StarfieldConditionForm("SQ_TreasureMap_CND_IsResourceLocation", "10460E:Starfield.esm")
            .Build();
        spec.UiComparisonExpectations.Add(ValidationUiComparisonExpectation.Literal(["EditorID"], "SQ_TreasureMap_CND_IsResourceLocation"));
        return spec;
    }

    /// <summary>
    /// Builds the Starfield <c>ActorShouldShowSpacesuitGameplayFlashlight</c> condition form validation spec,
    /// including a UI editor ID row expectation.
    /// </summary>
    /// <returns>The validation spec for the Starfield <c>ActorShouldShowSpacesuitGameplayFlashlight</c> sample.</returns>
    public static ValidationSpec Starfield_ActorShouldShowSpacesuitGameplayFlashlight()
    {
        var spec = StarfieldConditionForm("ActorShouldShowSpacesuitGameplayFlashlight", "0B1206:Starfield.esm")
            .Build();
        spec.UiComparisonExpectations.Add(ValidationUiComparisonExpectation.Literal(["EditorID"], "ActorShouldShowSpacesuitGameplayFlashlight"));
        return spec;
    }

    private static ValidationSpecBuilder StarfieldConditionForm(string sampleName, string formKey)
    {
        return ValidationSpecBuilder
            .ForRecord(SpecificationGame.Starfield, SupportedRecordSpecifications.ConditionForm)
            .Sample(sampleName)
            .FormKey(formKey)
            .AddBaselineUiComparisonExpectations()
            .AddRule(ValidationFieldRule.Field("OwnerQuest", "OwnerQuest"))
            .AddRules(GetConditionParameterRules());
    }

    private static IEnumerable<ValidationFieldRule> GetConditionParameterRules()
    {
        for (var conditionIndex = 0; conditionIndex <= 250; conditionIndex++)
        {
            var indexText = conditionIndex.ToString(System.Globalization.CultureInfo.InvariantCulture);
            yield return ValidationFieldRule.Field(
                "Conditions[" + indexText + "].Data.FirstParameter",
                "Conditions[" + indexText + "].Data.FirstParameter",
                ValidationValueNormalizer.DecimalFormKeyId);
            yield return ValidationFieldRule.Field(
                "Conditions[" + indexText + "].Data.SecondParameter",
                "Conditions[" + indexText + "].Data.SecondParameter",
                ValidationValueNormalizer.DecimalFormKeyId);
            yield return ValidationFieldRule.Field(
                "Conditions[" + indexText + "].Data.ThirdParameter",
                "Conditions[" + indexText + "].Data.ThirdParameter",
                ValidationValueNormalizer.DecimalFormKeyId);
        }
    }
}
