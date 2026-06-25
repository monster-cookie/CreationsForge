using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;

namespace CreationsForge.DataValidationTests.Validation.Specs.ConditionForm;

public static class ConditionFormValidationSpecs
{
    public static ValidationSpec Starfield_DebugMoveToPlanetConditions_Trait()
    {
        return StarfieldConditionForm("DebugMoveToPlanetConditions_Trait", "3C8F9C:Starfield.esm")
            .Build();
    }

    public static ValidationSpec Starfield_SFBGS_CND_Placeholder01_ReservedForUse()
    {
        return StarfieldConditionForm("SFBGS_CND_Placeholder01_ReservedForUse", "31982F:Starfield.esm")
            .Build();
    }

    public static ValidationSpec Starfield_SQ_TreasureMap_CND_IsResourceLocation()
    {
        return StarfieldConditionForm("SQ_TreasureMap_CND_IsResourceLocation", "10460E:Starfield.esm")
            .Build();
    }

    public static ValidationSpec Starfield_ActorShouldShowSpacesuitGameplayFlashlight()
    {
        return StarfieldConditionForm("ActorShouldShowSpacesuitGameplayFlashlight", "0B1206:Starfield.esm")
            .Build();
    }

    private static ValidationSpecBuilder StarfieldConditionForm(string sampleName, string formKey)
    {
        return ValidationSpecBuilder
            .ForRecord(SupportedGame.Starfield, RecordTypeCatalog.ConditionForm)
            .Sample(sampleName)
            .FormKey(formKey)
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
