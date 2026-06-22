using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;

namespace CreationsForge.DataValidationTests.Validation.Specs.ActorValueInformation;

public static class ActorValueInformationValidationSpecs
{
    public static ValidationSpec Starfield_TargetingModeActionPoints_AV()
    {
        return StarfieldActorValueInformation("TargetingModeActionPoints_AV", "05ACD4:Starfield.esm");
    }

    public static ValidationSpec Starfield_ENV_Resist_Airborne()
    {
        return StarfieldActorValueInformation("ENV_Resist_Airborne", "248D31:Starfield.esm");
    }

    public static ValidationSpec Starfield_ENV_Resist_Corrosive()
    {
        return StarfieldActorValueInformation("ENV_Resist_Corrosive", "248D30:Starfield.esm");
    }

    public static ValidationSpec Starfield_PEO_CarryWeight()
    {
        return StarfieldActorValueInformation("PEO_CarryWeight", "2EE0BB:Starfield.esm");
    }

    public static ValidationSpec Starfield_Health()
    {
        return StarfieldActorValueInformation("Health", "0002D4:Starfield.esm");
    }

    public static ValidationSpec Fallout4_SentryBotMaxHeatLevel()
    {
        return Fallout4ActorValueInformation("SentryBotMaxHeatLevel", "0B287B:Fallout4.esm");
    }

    public static ValidationSpec Fallout4_HC_Adrenaline()
    {
        return Fallout4ActorValueInformation("HC_Adrenaline", "00080F:Fallout4.esm");
    }

    public static ValidationSpec Fallout4_Incendiary()
    {
        return Fallout4ActorValueInformation("Incendiary", "1B88D8:Fallout4.esm");
    }

    public static ValidationSpec Fallout4_Agility()
    {
        return Fallout4ActorValueInformation("Agility", "0002C7:Fallout4.esm");
    }

    public static ValidationSpec Fallout4_AddictionCount()
    {
        return Fallout4ActorValueInformation("AddictionCount", "1EB998:Fallout4.esm");
    }

    public static ValidationSpec Skyrim_AVAlchemy()
    {
        return SkyrimActorValueInformation("AVAlchemy", "000456:Skyrim.esm")
            .AddRules(SkyrimSkillMultiplierRules())
            .AddRule(SkyrimSkillImproveOffsetRule())
            .AddRules(SkyrimPerkTreeRules())
            .Build();
    }

    public static ValidationSpec Skyrim_AVAlteration()
    {
        return SkyrimActorValueInformation("AVAlteration", "000458:Skyrim.esm")
            .AddRules(SkyrimSkillMultiplierRules())
            .AddRule(ValidationFieldRule.SpriggitAbsent("Skill.ImproveOffset"))
            .AddRule(ValidationFieldRule.DtoExpectedValue("Skill.ImproveOffset", "0"))
            .Build();
    }

    public static ValidationSpec Skyrim_AVBlock()
    {
        return SkyrimActorValueInformation("AVBlock", "00044F:Skyrim.esm")
            .AddRules(SkyrimSkillMultiplierRules())
            .AddRule(ValidationFieldRule.SpriggitAbsent("Skill.ImproveOffset"))
            .AddRule(ValidationFieldRule.DtoExpectedValue("Skill.ImproveOffset", "0"))
            .Build();
    }

    public static ValidationSpec Skyrim_AVFavorActive()
    {
        return SkyrimActorValueInformation("AVFavorActive", "0005F6:Skyrim.esm")
            .Build();
    }

    private static ValidationSpec StarfieldActorValueInformation(string sampleName, string formKey)
    {
        return BaseActorValueInformation(SupportedGame.Starfield, sampleName, formKey)
            .AddRule(ValidationFieldRule.TranslatedField("Abbreviation", "Abbreviation", requireAllLanguages: true))
            .AddRule(ValidationFieldRule.TranslatedField("Name", "Name", requireAllLanguages: true))
            .AddRule(ValidationFieldRule.DtoNonEmpty("Flags", "Flags"))
            .AddRule(ValidationFieldRule.IgnoreDtoPrefix("LocalizedStrings", "LocalizedStrings is the DTO projection of translated Spriggit fields."))
            .Build();
    }

    private static ValidationSpec Fallout4ActorValueInformation(string sampleName, string formKey)
    {
        return BaseActorValueInformation(SupportedGame.Fallout4, sampleName, formKey)
            .AddRule(ValidationFieldRule.TranslatedField("Description", "Description", requireAllLanguages: true))
            .AddRule(ValidationFieldRule.DtoNonEmpty("Flags", "Flags"))
            .AddRule(ValidationFieldRule.IgnoreDtoPrefix("LocalizedStrings", "LocalizedStrings is the DTO projection of translated Spriggit fields."))
            .Build();
    }

    private static ValidationSpecBuilder SkyrimActorValueInformation(string sampleName, string formKey)
    {
        return BaseActorValueInformation(SupportedGame.Skyrim, sampleName, formKey)
            .AddRule(ValidationFieldRule.TranslatedField("Name", "Name", requireAllLanguages: true))
            .AddRule(ValidationFieldRule.TranslatedField("Description", "Description", requireAllLanguages: true))
            .AddRule(ValidationFieldRule.IgnoreDtoPrefix("LocalizedStrings", "LocalizedStrings is the DTO projection of translated Spriggit fields."))
            .AddRules(SkyrimPerkTreeNumericRules())
            .AddRules(SkyrimPerkTreeConnectionRules())
            .AddRules(SkyrimPerkTreeDtoMetadataIgnores());
    }

    private static IEnumerable<ValidationFieldRule> SkyrimSkillMultiplierRules()
    {
        yield return ValidationFieldRule.Field("Skill.UseMult", "Skill.UseMult", ValidationValueNormalizer.DecimalNumber);
        yield return ValidationFieldRule.Field("Skill.ImproveMult", "Skill.ImproveMult", ValidationValueNormalizer.DecimalNumber);
    }

    private static ValidationFieldRule SkyrimSkillImproveOffsetRule()
    {
        return ValidationFieldRule.Field("Skill.ImproveOffset", "Skill.ImproveOffset", ValidationValueNormalizer.DecimalNumber);
    }

    private static IEnumerable<ValidationFieldRule> SkyrimPerkTreeNumericRules()
    {
        for (var index = 0; index <= 40; index++)
        {
            yield return ValidationFieldRule.Field(
                PerkTreePath(index, "HorizontalPosition"),
                PerkTreePath(index, "HorizontalPosition"),
                ValidationValueNormalizer.DecimalNumber);
            yield return ValidationFieldRule.Field(
                PerkTreePath(index, "VerticalPosition"),
                PerkTreePath(index, "VerticalPosition"),
                ValidationValueNormalizer.DecimalNumber);
        }
    }

    private static IEnumerable<ValidationFieldRule> SkyrimPerkTreeConnectionRules()
    {
        for (var index = 0; index <= 40; index++)
        {
            yield return ValidationFieldRule.FormKeyList(
                PerkTreePath(index, "ConnectionLineToIndices"),
                PerkTreePath(index, "ConnectionLineToIndices"),
                "TargetIndex");
        }
    }

    private static IEnumerable<ValidationFieldRule> SkyrimPerkTreeDtoMetadataIgnores()
    {
        for (var perkTreeIndex = 0; perkTreeIndex <= 40; perkTreeIndex++)
        {
            yield return ValidationFieldRule.IgnoreDto(
                PerkTreePath(perkTreeIndex, "PerkTreeIndex"),
                "PerkTreeIndex is DTO collection metadata for repository read-back.");

            for (var connectionLineIndex = 0; connectionLineIndex <= 20; connectionLineIndex++)
            {
                yield return ValidationFieldRule.IgnoreDto(
                    PerkTreePath(perkTreeIndex, "ConnectionLineToIndices") + "[" + connectionLineIndex.ToString(System.Globalization.CultureInfo.InvariantCulture) + "].ConnectionLineIndex",
                    "ConnectionLineIndex is DTO collection metadata for repository read-back.");
                yield return ValidationFieldRule.IgnoreDto(
                    PerkTreePath(perkTreeIndex, "ConnectionLineToIndices") + "[" + connectionLineIndex.ToString(System.Globalization.CultureInfo.InvariantCulture) + "].PerkTreeIndex",
                    "PerkTreeIndex is DTO collection metadata for repository read-back.");
            }
        }
    }

    private static IEnumerable<ValidationFieldRule> SkyrimPerkTreeRules()
    {
        yield return ValidationFieldRule.SpriggitAbsent(PerkTreePath(0, "Perk"));
        yield return ValidationFieldRule.DtoExpectedValue(PerkTreePath(0, "Perk"), "Null");

        yield return ValidationFieldRule.SpriggitAbsent(PerkTreePath(6, "ConnectionLineToIndices.Count"));
        yield return ValidationFieldRule.DtoExpectedValue(PerkTreePath(6, "ConnectionLineToIndices.Count"), "0");

        yield return ValidationFieldRule.SpriggitAbsent(PerkTreePath(9, "ConnectionLineToIndices.Count"));
        yield return ValidationFieldRule.DtoExpectedValue(PerkTreePath(9, "ConnectionLineToIndices.Count"), "0");
    }

    private static string PerkTreePath(int index, string fieldName)
    {
        return "PerkTree[" + index.ToString(System.Globalization.CultureInfo.InvariantCulture) + "]." + fieldName;
    }

    private static ValidationSpecBuilder BaseActorValueInformation(SupportedGame game, string sampleName, string formKey)
    {
        return ValidationSpecBuilder
            .ForRecord(game, RecordTypeCatalog.ActorValueInformation)
            .Sample(sampleName)
            .FormKey(formKey);
    }
}
