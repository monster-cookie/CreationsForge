using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;

namespace CreationsForge.DataValidationTests.Validation.Specs.Class;

public static class ClassValidationSpecs
{
    public static ValidationSpec Fallout4_ZeroSPECIALclass()
    {
        return Fallout4Class("ZeroSPECIALclass", "1CD0A8:Fallout4.esm").Build();
    }

    public static ValidationSpec Fallout4_Citizen()
    {
        return Fallout4Class("Citizen", "01326B:Fallout4.esm").Build();
    }

    /// <summary>
    /// Builds the Fallout 4 <c>BloatflyClass</c> class validation spec, including a stable UI editor ID row expectation.
    /// </summary>
    /// <returns>The validation spec for the Fallout 4 <c>BloatflyClass</c> sample.</returns>
    public static ValidationSpec Fallout4_BloatflyClass()
    {
        var spec = Fallout4Class("BloatflyClass", "031757:Fallout4.esm").Build();
        spec.UiComparisonExpectations.Add(ValidationUiComparisonExpectation.Literal(["EditorID"], "BloatflyClass"));
        return spec;
    }

    public static ValidationSpec Fallout4_MQ203Class()
    {
        return Fallout4Class("MQ203Class", "20ED07:Fallout4.esm").Build();
    }

    /// <summary>
    /// Builds the Skyrim <c>TrainerAlchemyExpert</c> class validation spec, including a UI skill-weight row expectation.
    /// </summary>
    /// <returns>The validation spec for the Skyrim <c>TrainerAlchemyExpert</c> sample.</returns>
    public static ValidationSpec Skyrim_TrainerAlchemyExpert()
    {
        var spec = SkyrimClass("TrainerAlchemyExpert", "0E3A6E:Skyrim.esm").Build();
        spec.UiComparisonExpectations.Add(new ValidationUiComparisonExpectation(
            ["SkillWeights", "SkillWeight [0]", "Key"],
            visualText: "EditorID"));
        return spec;
    }

    public static ValidationSpec Skyrim_TrainerAlchemyJourneyman()
    {
        return SkyrimClass("TrainerAlchemyJourneyman", "0E3A5D:Skyrim.esm").Build();
    }

    public static ValidationSpec Skyrim_AAAPlayerSpellswordClass()
    {
        return SkyrimClass("AAAPlayerSpellswordClass", "02F202:Skyrim.esm").Build();
    }

    public static ValidationSpec Skyrim_CombatSpellsword()
    {
        return SkyrimClass("CombatSpellsword", "013177:Skyrim.esm").Build();
    }

    public static ValidationSpec Skyrim_Bard()
    {
        return SkyrimClass("Bard", "01325D:Skyrim.esm").Build();
    }

    /// <summary>
    /// Builds the Starfield <c>Citizen</c> class validation spec, including a stable UI editor ID row expectation.
    /// </summary>
    /// <returns>The validation spec for the Starfield <c>Citizen</c> sample.</returns>
    public static ValidationSpec Starfield_Citizen()
    {
        var spec = StarfieldClass("Citizen", "01326B:Starfield.esm").Build();
        spec.UiComparisonExpectations.Add(ValidationUiComparisonExpectation.Literal(["EditorID"], "Citizen"));
        return spec;
    }

    public static ValidationSpec Starfield_CourserClass()
    {
        return StarfieldClass("CourserClass", "20F487:Starfield.esm").Build();
    }

    public static ValidationSpec Starfield_CrimsonFleetClass()
    {
        return StarfieldClass("CrimsonFleetClass", "010B2F:Starfield.esm").Build();
    }

    private static ValidationSpecBuilder Fallout4Class(string sampleName, string formKey)
    {
        return BaseClass(SupportedGame.Fallout4, sampleName, formKey);
    }

    private static ValidationSpecBuilder SkyrimClass(string sampleName, string formKey)
    {
        return BaseClass(SupportedGame.Skyrim, sampleName, formKey)
            .AddRules(GetClassWeightRules("SkillWeights", "SkillWeights", maxIndex: 17))
            .AddRules(GetClassWeightRules("StatWeights", "StatWeights", maxIndex: 2));
    }

    private static ValidationSpecBuilder StarfieldClass(string sampleName, string formKey)
    {
        return BaseClass(SupportedGame.Starfield, sampleName, formKey);
    }

    private static ValidationSpecBuilder BaseClass(SupportedGame game, string sampleName, string formKey)
    {
        return ValidationSpecBuilder
            .ForRecord(game, RecordTypeCatalog.Class)
            .Sample(sampleName)
            .FormKey(formKey)
            .AddBaselineUiComparisonExpectations()
            .AddRule(ValidationFieldRule.TranslatedField("Name", "Name"))
            .AddRule(ValidationFieldRule.TranslatedField("Description", "Description"))
            .AddRule(ValidationFieldRule.Field("BleedoutDefault", "BleedoutDefault", ValidationValueNormalizer.DecimalNumber))
            .AddRule(ValidationFieldRule.Field("VoicePoints", "VoicePoints", ValidationValueNormalizer.DecimalNumber))
            .AddRule(ValidationFieldRule.Field("Unknown", "Unknown", ValidationValueNormalizer.DecimalNumber))
            .AddRule(ValidationFieldRule.Field("Unknown2", "Unknown2", ValidationValueNormalizer.DecimalNumber))
            .AddRules(GetClassPropertyRules())
            .AddRule(ValidationFieldRule.IgnoreDtoPrefix("LocalizedStrings", "LocalizedStrings is the DTO projection of translated Spriggit fields."));
    }

    private static IEnumerable<ValidationFieldRule> GetClassPropertyRules()
    {
        for (var propertyIndex = 0; propertyIndex <= 10; propertyIndex++)
        {
            var indexText = propertyIndex.ToString(System.Globalization.CultureInfo.InvariantCulture);
            yield return ValidationFieldRule.Field("Properties[" + indexText + "].ActorValue", "Properties[" + indexText + "].ActorValueFormKey");
            yield return ValidationFieldRule.Field("Properties[" + indexText + "].Value", "Properties[" + indexText + "].Value", ValidationValueNormalizer.DecimalNumber);
        }
    }

    private static IEnumerable<ValidationFieldRule> GetClassWeightRules(string spriggitPath, string dtoPath, int maxIndex)
    {
        for (var weightIndex = 0; weightIndex <= maxIndex; weightIndex++)
        {
            var indexText = weightIndex.ToString(System.Globalization.CultureInfo.InvariantCulture);
            yield return ValidationFieldRule.Field(spriggitPath + "[" + indexText + "].Key", dtoPath + "[" + indexText + "].Key");
            yield return ValidationFieldRule.Field(spriggitPath + "[" + indexText + "].Value", dtoPath + "[" + indexText + "].Value", ValidationValueNormalizer.DecimalNumber);
        }
    }
}
