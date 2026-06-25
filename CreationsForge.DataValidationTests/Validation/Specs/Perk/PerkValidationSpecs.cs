using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;

namespace CreationsForge.DataValidationTests.Validation.Specs.Perk;

public static class PerkValidationSpecs
{
    private static readonly IReadOnlyDictionary<string, string> ScriptingAdapterPathReplacements =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [".Objects"] = ".ListItems",
            [".Object"] = ".ObjectFormKey",
            [".Alias"] = ".ObjectAlias",
            [".Data"] = ".DataInt"
        };

    private static readonly IReadOnlyDictionary<string, string> NoPathReplacements =
        new Dictionary<string, string>(StringComparer.Ordinal);

    public static ValidationSpec Starfield_Skill_BoostAssaultTraining()
    {
        return StarfieldPerk("Skill_BoostAssaultTraining", "08C3EE:Starfield.esm");
    }

    public static ValidationSpec Starfield_Skill_BoostPackTraining()
    {
        var spec = StarfieldPerk("Skill_BoostPackTraining", "146C2C:Starfield.esm");
        spec.UiComparisonExpectations.Add(ValidationUiComparisonExpectation.DtoField(
            ["Ranks", "Rank [0]", "Effects", "Effect [0]", "Value"],
            "Ranks[0].Effects[0].Value",
            "Ranks"));
        return spec;
    }

    public static ValidationSpec Starfield_TrainingTechnologyExpert()
    {
        return StarfieldPerk("TrainingTechnologyExpert", "27CBBE:Starfield.esm");
    }

    public static ValidationSpec Starfield_TRAIT_FreestarCollectiveSettler()
    {
        return StarfieldPerk("TRAIT_FreestarCollectiveSettler", "227FD5:Starfield.esm");
    }

    public static ValidationSpec Starfield_BackgroundBigGameHunter()
    {
        return StarfieldPerk("BackgroundBigGameHunter", "22EC76:Starfield.esm");
    }

    public static ValidationSpec Fallout4_AddictionManager()
    {
        return Fallout4Perk("AddictionManager", "2458BA:Fallout4.esm", withMajorRecordFlagsRaw: true);
    }

    public static ValidationSpec Fallout4_AnimalFriend01()
    {
        return Fallout4Perk("AnimalFriend01", "01E67F:Fallout4.esm", withMajorRecordFlagsRaw: false, withScriptFragments: true);
    }

    public static ValidationSpec Fallout4_AnimalFriend02()
    {
        return Fallout4Perk("AnimalFriend02", "04A0D9:Fallout4.esm", withMajorRecordFlagsRaw: false, withScriptFragments: true);
    }

    public static ValidationSpec Fallout4_TrainingAG01()
    {
        return Fallout4Perk("TrainingAG01", "0D979D:Fallout4.esm", withMajorRecordFlagsRaw: false);
    }

    public static ValidationSpec Fallout4_Basher02()
    {
        return Fallout4Perk("Basher02", "065DFA:Fallout4.esm", withMajorRecordFlagsRaw: false);
    }

    public static ValidationSpec Skyrim_AlchemySkillBoosts()
    {
        return SkyrimPerk("AlchemySkillBoosts", "0A725C:Skyrim.esm");
    }

    public static ValidationSpec Skyrim_Armsman00()
    {
        return SkyrimPerk("Armsman00", "0BABE4:Skyrim.esm");
    }

    public static ValidationSpec Skyrim_Armsman20()
    {
        return SkyrimPerk("Armsman20", "079343:Skyrim.esm");
    }

    public static ValidationSpec Skyrim_Allure()
    {
        return SkyrimPerk("Allure", "058F75:Skyrim.esm");
    }

    private static ValidationSpec StarfieldPerk(string sampleName, string formKey)
    {
        return BasePerk(SupportedGame.Starfield, sampleName, formKey)
            .AddRule(ValidationFieldRule.Field("Categroy", "Category"))
            .AddRule(ValidationFieldRule.Field("Restriction", "RestrictionFormKey"))
            .AddRule(ValidationFieldRule.Field("Training", "TrainingFormKey"))
            .AddRule(ValidationFieldRule.ScalarList("Flags", "Flags"))
            .AddRule(ValidationFieldRule.ScalarList("MajorFlags", "MajorFlags"))
            .Build();
    }

    private static ValidationSpec Fallout4Perk(string sampleName, string formKey, bool withMajorRecordFlagsRaw, bool withScriptFragments = false)
    {
        var spec = BasePerk(SupportedGame.Fallout4, sampleName, formKey)
            .AddRule(ValidationFieldRule.Field("Category", "Category"))
            .AddRule(ValidationFieldRule.Field("Categroy", "Category"))
            .AddRule(ValidationFieldRule.Field("Restriction", "RestrictionFormKey"))
            .AddRule(ValidationFieldRule.Field("Training", "TrainingFormKey"))
            .AddRule(ValidationFieldRule.SoundSlot("Sound", "Sound", "Start"));

        if (withScriptFragments)
        {
            AddScriptFragmentRules(spec, fragmentCount: 1);
        }

        AddMajorRecordFlagRules(spec, withMajorRecordFlagsRaw, "Fallout4MajorRecordFlags");
        return spec.Build();
    }

    private static ValidationSpec SkyrimPerk(string sampleName, string formKey)
    {
        var spec = BasePerk(SupportedGame.Skyrim, sampleName, formKey)
            .AddRule(ValidationFieldRule.Field("Category", "Category"))
            .AddRule(ValidationFieldRule.Field("Categroy", "Category"))
            .AddRule(ValidationFieldRule.Field("Restriction", "RestrictionFormKey"))
            .AddRule(ValidationFieldRule.Field("Training", "TrainingFormKey"))
            .AddRule(ValidationFieldRule.SoundSlot("Sound", "Sound", "Start"));

        AddMajorRecordFlagRules(spec, withMajorRecordFlagsRaw: false, "SkyrimMajorRecordFlags");
        return spec.Build();
    }

    private static ValidationSpecBuilder BasePerk(SupportedGame game, string sampleName, string formKey)
    {
        return ValidationSpecBuilder
            .ForRecord(game, RecordTypeCatalog.Perk)
            .Sample(sampleName)
            .FormKey(formKey)
            .AddRule(ValidationFieldRule.TranslatedField("Name", "Name", requireAllLanguages: true))
            .AddRule(ValidationFieldRule.TranslatedField("Description", "Description", requireAllLanguages: true))
            .AddRule(ValidationFieldRule.Field("Level", "Level"))
            .AddRule(ValidationFieldRule.Field("NumRanks", "NumRanks"))
            .AddRule(ValidationFieldRule.Field("Playable", "Playable"))
            .AddRule(ValidationFieldRule.Field("Hidden", "Hidden"))
            .AddRule(ValidationFieldRule.Field("NextPerk", "NextPerk"))
            .AddRule(ValidationFieldRule.PathPrefix("Conditions", "Conditions", NoPathReplacements))
            .AddRules(RootEffectRules())
            .AddRule(ValidationFieldRule.Field("Version2", "Version2"))
            .AddRule(ValidationFieldRule.Field("VersionControl", "VersionControl"))
            .AddRule(ValidationFieldRule.IgnoreDtoPrefix("LocalizedStrings", "LocalizedStrings is the DTO projection of translated Spriggit fields."))
            .AddRule(ValidationFieldRule.PathPrefix("VirtualMachineAdapter.Scripts", "ScriptingAdapters", ScriptingAdapterPathReplacements))
            .AddRules(RankRules())
            .AddRules(BackgroundSkillRules())
            .AddRules(DtoMetadataIgnores());
    }

    private static IEnumerable<ValidationFieldRule> RankRules()
    {
        for (var rankIndex = 0; rankIndex <= 25; rankIndex++)
        {
            yield return ValidationFieldRule.TranslatedField(
                RankPath(rankIndex, "Description"),
                RankPath(rankIndex, "Description"),
                requireAllLanguages: true);
            yield return ValidationFieldRule.Field(
                RankPath(rankIndex, "UnknownStatic"),
                RankPath(rankIndex, "UnknownStaticFormKey"));
            yield return ValidationFieldRule.Field(
                RankPath(rankIndex, "Conditions.Count"),
                RankPath(rankIndex, "ConditionCount"));
            yield return ValidationFieldRule.PathPrefix(
                RankPath(rankIndex, "Conditions"),
                RankPath(rankIndex, "Conditions"),
                NoPathReplacements);
            yield return ValidationFieldRule.Field(
                RankPath(rankIndex, "Activities.Count"),
                RankPath(rankIndex, "ActivityCount"));

            for (var activityIndex = 0; activityIndex <= 10; activityIndex++)
            {
                yield return ValidationFieldRule.Field(
                    ActivityPath(rankIndex, activityIndex, "ATAN"),
                    ActivityPath(rankIndex, activityIndex, "ATAN"));
                yield return ValidationFieldRule.TranslatedField(
                    ActivityPath(rankIndex, activityIndex, "Name"),
                    ActivityPath(rankIndex, activityIndex, "Name"),
                    requireAllLanguages: true);
                yield return ValidationFieldRule.TranslatedField(
                    ActivityPath(rankIndex, activityIndex, "Description"),
                    ActivityPath(rankIndex, activityIndex, "Description"),
                    requireAllLanguages: true);
                yield return ValidationFieldRule.Field(
                    ActivityPath(rankIndex, activityIndex, "ANAM"),
                    ActivityPath(rankIndex, activityIndex, "ANAM"));
                yield return ValidationFieldRule.Field(
                    ActivityPath(rankIndex, activityIndex, "Configuration"),
                    ActivityPath(rankIndex, activityIndex, "Configuration"),
                    ValidationValueNormalizer.JsonWhitespace);
                yield return ValidationFieldRule.Field(
                    ActivityPath(rankIndex, activityIndex, "ProgressionEvalutor.Count"),
                    ActivityPath(rankIndex, activityIndex, "ProgressionEvalutor.Count"));

                for (var evaluatorIndex = 0; evaluatorIndex <= 10; evaluatorIndex++)
                {
                    yield return ValidationFieldRule.Field(
                        ActivityEvaluatorPath(rankIndex, activityIndex, evaluatorIndex, "Name"),
                        ActivityEvaluatorPath(rankIndex, activityIndex, evaluatorIndex, "Name"));
                    yield return ValidationFieldRule.Field(
                        ActivityEvaluatorPath(rankIndex, activityIndex, evaluatorIndex, "Conditions.Count"),
                        ActivityEvaluatorPath(rankIndex, activityIndex, evaluatorIndex, "Conditions.Count"));
                    yield return ValidationFieldRule.PathPrefix(
                        ActivityEvaluatorPath(rankIndex, activityIndex, evaluatorIndex, "Conditions"),
                        ActivityEvaluatorPath(rankIndex, activityIndex, evaluatorIndex, "Conditions"),
                        NoPathReplacements);
                }
            }

            for (var effectIndex = 0; effectIndex <= 50; effectIndex++)
            {
                yield return ValidationFieldRule.TranslatedField(
                    EffectPath(rankIndex, effectIndex, "ButtonLabel"),
                    EffectPath(rankIndex, effectIndex, "ButtonLabel"),
                    requireAllLanguages: true);
                yield return ValidationFieldRule.Field(
                    EffectPath(rankIndex, effectIndex, "PerkEntryID"),
                    EffectPath(rankIndex, effectIndex, "PerkEntryId"));
                yield return ValidationFieldRule.ScalarList(
                    EffectPath(rankIndex, effectIndex, "Flags"),
                    EffectPath(rankIndex, effectIndex, "Flags"));
                yield return ValidationFieldRule.Field(
                    EffectPath(rankIndex, effectIndex, "Conditions.Count"),
                    EffectPath(rankIndex, effectIndex, "ConditionCount"));
                yield return ValidationFieldRule.PathPrefix(
                    EffectPath(rankIndex, effectIndex, "Conditions"),
                    EffectPath(rankIndex, effectIndex, "Conditions"),
                    NoPathReplacements);
                yield return ValidationFieldRule.Field(
                    EffectPath(rankIndex, effectIndex, "Value"),
                    EffectPath(rankIndex, effectIndex, "Value"),
                    ValidationValueNormalizer.DecimalNumber);
                yield return ValidationFieldRule.Field(
                    EffectPath(rankIndex, effectIndex, "ActorValue"),
                    EffectPath(rankIndex, effectIndex, "ActorValue"));
                yield return ValidationFieldRule.Field(
                    EffectPath(rankIndex, effectIndex, "Spell"),
                    EffectPath(rankIndex, effectIndex, "Spell"));
                yield return ValidationFieldRule.Field(
                    EffectPath(rankIndex, effectIndex, "Quest"),
                    EffectPath(rankIndex, effectIndex, "Quest"));
                yield return ValidationFieldRule.Field(
                    EffectPath(rankIndex, effectIndex, "Stage"),
                    EffectPath(rankIndex, effectIndex, "Stage"));
            }
        }
    }

    private static IEnumerable<ValidationFieldRule> RootEffectRules()
    {
        for (var effectIndex = 0; effectIndex <= 50; effectIndex++)
        {
            yield return ValidationFieldRule.TranslatedField(
                RootEffectPath(effectIndex, "ButtonLabel"),
                RootEffectPath(effectIndex, "ButtonLabel"),
                requireAllLanguages: true);
            yield return ValidationFieldRule.Field(
                RootEffectPath(effectIndex, "PerkEntryID"),
                RootEffectPath(effectIndex, "PerkEntryId"));
            yield return ValidationFieldRule.ScalarList(
                RootEffectPath(effectIndex, "Flags"),
                RootEffectPath(effectIndex, "Flags"));
            yield return ValidationFieldRule.Field(
                RootEffectPath(effectIndex, "Conditions.Count"),
                RootEffectPath(effectIndex, "ConditionCount"));
            yield return ValidationFieldRule.PathPrefix(
                RootEffectPath(effectIndex, "Conditions"),
                RootEffectPath(effectIndex, "Conditions"),
                NoPathReplacements);
            yield return ValidationFieldRule.Field(
                RootEffectPath(effectIndex, "Value"),
                RootEffectPath(effectIndex, "Value"),
                ValidationValueNormalizer.DecimalNumber);
        }
    }

    private static IEnumerable<ValidationFieldRule> BackgroundSkillRules()
    {
        for (var skillIndex = 0; skillIndex <= 20; skillIndex++)
        {
            yield return ValidationFieldRule.Field(
                "BackgroundSkills[" + skillIndex.ToString(System.Globalization.CultureInfo.InvariantCulture) + "]",
                "BackgroundSkills[" + skillIndex.ToString(System.Globalization.CultureInfo.InvariantCulture) + "].SkillFormKey");
        }
    }

    private static IEnumerable<ValidationFieldRule> DtoMetadataIgnores()
    {
        for (var rankIndex = 0; rankIndex <= 25; rankIndex++)
        {
            yield return ValidationFieldRule.IgnoreDto(
                RankPath(rankIndex, "RankIndex"),
                "RankIndex is DTO collection metadata for repository read-back.");

            for (var activityIndex = 0; activityIndex <= 10; activityIndex++)
            {
                yield return ValidationFieldRule.IgnoreDto(
                    ActivityPath(rankIndex, activityIndex, "RankIndex"),
                    "RankIndex is DTO collection metadata for repository read-back.");
                yield return ValidationFieldRule.IgnoreDto(
                    ActivityPath(rankIndex, activityIndex, "ActivityIndex"),
                    "ActivityIndex is DTO collection metadata for repository read-back.");

                for (var evaluatorIndex = 0; evaluatorIndex <= 10; evaluatorIndex++)
                {
                    yield return ValidationFieldRule.IgnoreDto(
                        ActivityEvaluatorPath(rankIndex, activityIndex, evaluatorIndex, "RankIndex"),
                        "RankIndex is DTO collection metadata for repository read-back.");
                    yield return ValidationFieldRule.IgnoreDto(
                        ActivityEvaluatorPath(rankIndex, activityIndex, evaluatorIndex, "ActivityIndex"),
                        "ActivityIndex is DTO collection metadata for repository read-back.");
                    yield return ValidationFieldRule.IgnoreDto(
                        ActivityEvaluatorPath(rankIndex, activityIndex, evaluatorIndex, "EvaluatorIndex"),
                        "EvaluatorIndex is DTO collection metadata for repository read-back.");
                }
            }

            for (var effectIndex = 0; effectIndex <= 50; effectIndex++)
            {
                yield return ValidationFieldRule.IgnoreDto(
                    EffectPath(rankIndex, effectIndex, "EffectIndex"),
                    "EffectIndex is DTO collection metadata for repository read-back.");

                for (var conditionTabIndex = 0; conditionTabIndex <= 10; conditionTabIndex++)
                {
                    yield return ValidationFieldRule.IgnoreDto(
                        EffectPath(rankIndex, effectIndex, "Conditions[" + conditionTabIndex.ToString(System.Globalization.CultureInfo.InvariantCulture) + "].RankIndex"),
                        "RankIndex is DTO collection metadata for repository read-back.");
                    yield return ValidationFieldRule.IgnoreDto(
                        EffectPath(rankIndex, effectIndex, "Conditions[" + conditionTabIndex.ToString(System.Globalization.CultureInfo.InvariantCulture) + "].EffectIndex"),
                        "EffectIndex is DTO collection metadata for repository read-back.");
                    yield return ValidationFieldRule.IgnoreDto(
                        EffectPath(rankIndex, effectIndex, "Conditions[" + conditionTabIndex.ToString(System.Globalization.CultureInfo.InvariantCulture) + "].ConditionTabIndex"),
                        "ConditionTabIndex is DTO collection metadata for repository read-back.");
                }
            }
        }

        for (var effectIndex = 0; effectIndex <= 50; effectIndex++)
        {
            yield return ValidationFieldRule.IgnoreDto(
                RootEffectPath(effectIndex, "EffectIndex"),
                "EffectIndex is DTO collection metadata for repository read-back.");

            for (var conditionTabIndex = 0; conditionTabIndex <= 10; conditionTabIndex++)
            {
                yield return ValidationFieldRule.IgnoreDto(
                    RootEffectPath(effectIndex, "Conditions[" + conditionTabIndex.ToString(System.Globalization.CultureInfo.InvariantCulture) + "].RankIndex"),
                    "RankIndex is DTO collection metadata for repository read-back.");
                yield return ValidationFieldRule.IgnoreDto(
                    RootEffectPath(effectIndex, "Conditions[" + conditionTabIndex.ToString(System.Globalization.CultureInfo.InvariantCulture) + "].EffectIndex"),
                    "EffectIndex is DTO collection metadata for repository read-back.");
                yield return ValidationFieldRule.IgnoreDto(
                    RootEffectPath(effectIndex, "Conditions[" + conditionTabIndex.ToString(System.Globalization.CultureInfo.InvariantCulture) + "].ConditionTabIndex"),
                    "ConditionTabIndex is DTO collection metadata for repository read-back.");
            }
        }

        for (var skillIndex = 0; skillIndex <= 20; skillIndex++)
        {
            yield return ValidationFieldRule.IgnoreDto(
                "BackgroundSkills[" + skillIndex.ToString(System.Globalization.CultureInfo.InvariantCulture) + "].SkillIndex",
                "SkillIndex is DTO collection metadata for repository read-back.");
        }

        for (var fragmentIndex = 0; fragmentIndex <= 20; fragmentIndex++)
        {
            yield return ValidationFieldRule.IgnoreDto(
                "ScriptFragments[" + fragmentIndex.ToString(System.Globalization.CultureInfo.InvariantCulture) + "].FragmentIndex",
                "FragmentIndex is DTO collection metadata for repository read-back.");
            yield return ValidationFieldRule.IgnoreDto(
                "ScriptFragments[" + fragmentIndex.ToString(System.Globalization.CultureInfo.InvariantCulture) + "].FragmentSlot",
                "FragmentSlot is DTO collection metadata for repository read-back.");
            yield return ValidationFieldRule.IgnoreDto(
                "ScriptFragments[" + fragmentIndex.ToString(System.Globalization.CultureInfo.InvariantCulture) + "].MutagenObjectType",
                "MutagenObjectType is DTO implementation metadata for script-fragment read-back.");
        }
    }

    private static void AddScriptFragmentRules(ValidationSpecBuilder spec, int fragmentCount)
    {
        spec
            .AddRule(ValidationFieldRule.OptionalField(
            "VirtualMachineAdapter.ScriptFragments.ExtraBindDataVersion",
            "ScriptFragments[0].ExtraBindDataVersion"))
            .AddRule(ValidationFieldRule.DtoDefaultWhenSpriggitAbsent(
                "VirtualMachineAdapter.ScriptFragments.ExtraBindDataVersion",
                "ScriptFragments[0].ExtraBindDataVersion",
                "3",
                "Mutagen exposes the script-fragment bind data version when Spriggit omits the default value."));

        for (var fragmentIndex = 0; fragmentIndex < fragmentCount; fragmentIndex++)
        {
            var spriggitPath = "VirtualMachineAdapter.ScriptFragments.Fragments[" + fragmentIndex.ToString(System.Globalization.CultureInfo.InvariantCulture) + "]";
            var dtoPath = "ScriptFragments[" + (fragmentIndex + 1).ToString(System.Globalization.CultureInfo.InvariantCulture) + "]";
            spec
                .AddRule(ValidationFieldRule.Field(spriggitPath + ".FragmentName", dtoPath + ".FragmentName"))
                .AddRule(ValidationFieldRule.OptionalField(spriggitPath + ".FragmentIndex", dtoPath + ".SourceFragmentIndex"))
                .AddRule(ValidationFieldRule.Field(spriggitPath + ".ScriptName", dtoPath + ".ScriptName"))
                .AddRule(ValidationFieldRule.Field(spriggitPath + ".Unknown2", dtoPath + ".Unknown2"));
        }

        var scriptFragmentIndex = fragmentCount + 1;
        spec
            .AddRule(ValidationFieldRule.Field(
                "VirtualMachineAdapter.ScriptFragments.Script.Name",
                "ScriptFragments[" + scriptFragmentIndex.ToString(System.Globalization.CultureInfo.InvariantCulture) + "].ScriptName"))
            .AddRule(ValidationFieldRule.DtoNonEmpty("ScriptingAdapters[0].Name"))
            .AddRule(ValidationFieldRule.PathPrefix(
                "VirtualMachineAdapter.ScriptFragments.Script.Properties",
                "ScriptingAdapters[0].Properties",
                ScriptingAdapterPathReplacements));
    }

    private static void AddMajorRecordFlagRules(ValidationSpecBuilder spec, bool withMajorRecordFlagsRaw, string flagListName)
    {
        if (withMajorRecordFlagsRaw)
        {
            spec
                .AddRule(ValidationFieldRule.Field("MajorRecordFlagsRaw", "MajorRecordFlags"))
                .AddRule(ValidationFieldRule.IgnoreSpriggit("MajorFlags.Count", "MajorRecordFlagsRaw covers the flag value."))
                .AddRule(ValidationFieldRule.IgnoreSpriggit("MajorFlags[0]", "MajorRecordFlagsRaw covers the flag value."))
                .AddRule(ValidationFieldRule.IgnoreDto("MajorFlags", "MajorRecordFlagsRaw covers the flag value."))
                .AddRule(ValidationFieldRule.IgnoreSpriggit(flagListName + ".Count", "MajorRecordFlagsRaw covers the flag value."))
                .AddRule(ValidationFieldRule.IgnoreSpriggit(flagListName + "[0]", "MajorRecordFlagsRaw covers the flag value."));
            return;
        }

        spec.AddRule(ValidationFieldRule.DtoDefaultWhenSpriggitAbsent(
            "MajorRecordFlags",
            "MajorRecordFlags",
            "0",
            "Mutagen exposes the default MajorRecordFlags value when Spriggit omits the zero-valued field."));
    }

    private static string RankPath(int rankIndex, string fieldName)
    {
        return "Ranks[" + rankIndex.ToString(System.Globalization.CultureInfo.InvariantCulture) + "]." + fieldName;
    }

    private static string EffectPath(int rankIndex, int effectIndex, string fieldName)
    {
        return RankPath(rankIndex, "Effects[" + effectIndex.ToString(System.Globalization.CultureInfo.InvariantCulture) + "]." + fieldName);
    }

    private static string ActivityPath(int rankIndex, int activityIndex, string fieldName)
    {
        return RankPath(rankIndex, "Activities[" + activityIndex.ToString(System.Globalization.CultureInfo.InvariantCulture) + "]." + fieldName);
    }

    private static string ActivityEvaluatorPath(int rankIndex, int activityIndex, int evaluatorIndex, string fieldName)
    {
        return ActivityPath(rankIndex, activityIndex, "ProgressionEvalutor[" + evaluatorIndex.ToString(System.Globalization.CultureInfo.InvariantCulture) + "]." + fieldName);
    }

    private static string RootEffectPath(int effectIndex, string fieldName)
    {
        return "Effects[" + effectIndex.ToString(System.Globalization.CultureInfo.InvariantCulture) + "]." + fieldName;
    }
}
