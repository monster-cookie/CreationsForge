using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;

namespace CreationsForge.DataValidationTests.Validation.Specs.MagicEffect;

public static class MagicEffectValidationSpecs
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

    public static ValidationSpec Starfield_ArtifactPowerLifeForced_Effect()
    {
        return StarfieldMagicEffect("ArtifactPowerLifeForced_Effect", "2C5392:Starfield.esm").Build();
    }

    public static ValidationSpec Starfield_ArtifactPowerParticleBeam_Effect()
    {
        return StarfieldMagicEffect("ArtifactPowerParticleBeam_Effect", "2C7789:Starfield.esm").Build();
    }

    public static ValidationSpec Starfield_ArtifactPowerSunlessSpace_AIUse()
    {
        return StarfieldMagicEffect("ArtifactPowerSunlessSpace_AIUse", "23AF01:Starfield.esm").Build();
    }

    public static ValidationSpec Starfield_ArtifactPowerSolarFlare_AIUse()
    {
        return StarfieldMagicEffect("ArtifactPowerSolarFlare_AIUse", "22AC10:Starfield.esm").Build();
    }

    public static ValidationSpec Starfield_ENV_DMG_Airborne_Hazard_Damage_Effect()
    {
        return StarfieldMagicEffect("ENV_DMG_Airborne_Hazard_Damage_Effect", "245B6F:Starfield.esm").Build();
    }

    public static ValidationSpec Fallout4_CritCryoFreezeEffect()
    {
        return Fallout4MagicEffect("CritCryoFreezeEffect", "247A6C:Fallout4.esm").Build();
    }

    public static ValidationSpec Fallout4_CryoFreezeEffect01()
    {
        return Fallout4MagicEffect("CryoFreezeEffect01", "18C354:Fallout4.esm").Build();
    }

    public static ValidationSpec Fallout4_CryoFreezeEffect02()
    {
        return Fallout4MagicEffect("CryoFreezeEffect02", "18C356:Fallout4.esm").Build();
    }

    public static ValidationSpec Fallout4_PerkPainTrainKnockbackEffect()
    {
        return Fallout4MagicEffect("PerkPainTrainKnockbackEffect", "171781:Fallout4.esm").Build();
    }

    public static ValidationSpec Fallout4_DN102_LabDemo3ParalyzeEffect()
    {
        return Fallout4MagicEffect("DN102_LabDemo3ParalyzeEffect", "0AE04F:Fallout4.esm").Build();
    }

    public static ValidationSpec Skyrim_ShockDamageMassConcAimed()
    {
        return SkyrimMagicEffect("ShockDamageMassConcAimed", "0D22FA:Skyrim.esm").Build();
    }

    public static ValidationSpec Skyrim_dunVolunruudPickaxeEffect()
    {
        return SkyrimMagicEffect("dunVolunruudPickaxeEffect", "1019D6:Skyrim.esm").Build();
    }

    public static ValidationSpec Skyrim_ArmorFFSelf100()
    {
        return SkyrimMagicEffect("ArmorFFSelf100", "0CDB75:Skyrim.esm").Build();
    }

    public static ValidationSpec Skyrim_DA15WabbajackFF()
    {
        return SkyrimMagicEffect("DA15WabbajackFF", "09B246:Skyrim.esm").Build();
    }

    public static ValidationSpec Skyrim_dunHalldirAggDownFFAimedArea()
    {
        return SkyrimMagicEffect("dunHalldirAggDownFFAimedArea", "0FB406:Skyrim.esm").Build();
    }

    private static ValidationSpecBuilder StarfieldMagicEffect(string sampleName, string formKey)
    {
        return BaseMagicEffect(SupportedGame.Starfield, sampleName, formKey);
    }

    private static ValidationSpecBuilder Fallout4MagicEffect(string sampleName, string formKey)
    {
        return BaseMagicEffect(SupportedGame.Fallout4, sampleName, formKey);
    }

    private static ValidationSpecBuilder SkyrimMagicEffect(string sampleName, string formKey)
    {
        return BaseMagicEffect(SupportedGame.Skyrim, sampleName, formKey)
            .AddRule(ValidationFieldRule.DtoDefaultWhenSpriggitAbsent(
                "CastingSoundLevel",
                "CastingSoundLevel",
                "Loud",
                "Mutagen exposes the Skyrim default CastingSoundLevel value when Spriggit omits the field."));
    }

    private static ValidationSpecBuilder BaseMagicEffect(SupportedGame game, string sampleName, string formKey)
    {
        return ValidationSpecBuilder
            .ForRecord(game, RecordTypeCatalog.MagicEffect)
            .Sample(sampleName)
            .FormKey(formKey)
            .AddRule(ValidationFieldRule.TranslatedField("Name", "Name", requireAllLanguages: true))
            .AddRule(ValidationFieldRule.TranslatedField("Description", "Description", requireAllLanguages: true))
            .AddRule(ValidationFieldRule.Field("Version2", "Version2"))
            .AddRule(ValidationFieldRule.Field("VersionControl", "VersionControl"))
            .AddRule(ValidationFieldRule.DtoDefaultWhenSpriggitAbsent(
                "MajorRecordFlagsRaw",
                "MajorRecordFlags",
                "0",
                "Mutagen exposes the default MajorRecordFlags value when Spriggit omits the zero-valued field."))
            .AddRule(ValidationFieldRule.Field("MajorRecordFlagsRaw", "MajorRecordFlags"))
            .AddRule(ValidationFieldRule.ScalarList("Flags", "Flags"))
            .AddRule(ValidationFieldRule.Field("CastType", "CastType"))
            .AddRule(ValidationFieldRule.Field("TargetType", "TargetType"))
            .AddRule(ValidationFieldRule.OptionalField("CastingSoundLevel", "CastingSoundLevel"))
            .AddRule(ValidationFieldRule.Field("DualCastScale", "DualCastScale", ValidationValueNormalizer.DecimalNumber))
            .AddRule(ValidationFieldRule.OptionalField("Unknown1", "Unknown1"))
            .AddRule(ValidationFieldRule.Field("BaseCost", "BaseCost", ValidationValueNormalizer.DecimalNumber))
            .AddRule(ValidationFieldRule.Field("MagicSkill", "MagicSkill"))
            .AddRule(ValidationFieldRule.Field("CastingLight", "CastingLightFormKey"))
            .AddRule(ValidationFieldRule.Field("MenuDisplayObject", "MenuDisplayObjectFormKey"))
            .AddRule(ValidationFieldRule.Field("MinimumSkillLevel", "MinimumSkillLevel"))
            .AddRule(ValidationFieldRule.Field("SkillUsageMultiplier", "SkillUsageMultiplier", ValidationValueNormalizer.DecimalNumber))
            .AddRule(ValidationFieldRule.Field("SpellmakingCastingTime", "SpellmakingCastingTime", ValidationValueNormalizer.DecimalNumber))
            .AddRule(ValidationFieldRule.Field("TaperWeight", "TaperWeight", ValidationValueNormalizer.DecimalNumber))
            .AddRule(ValidationFieldRule.Field("SecondActorValue", "SecondActorValue"))
            .AddRule(ValidationFieldRule.Field("SecondActorValueWeight", "SecondActorValueWeight", ValidationValueNormalizer.DecimalNumber))
            .AddRule(ValidationFieldRule.Field("SpellmakingArea", "SpellmakingArea"))
            .AddRule(ValidationFieldRule.Field("EnchantShader", "EnchantShaderFormKey"))
            .AddRule(ValidationFieldRule.Field("ActorValue2", "ActorValue2FormKey"))
            .AddRule(ValidationFieldRule.Field("ResistValue", "ResistValue"))
            .AddRule(ValidationFieldRule.OptionalField("ResistValue", "ResistValueFormKey"))
            .AddRule(ValidationFieldRule.Field("PerkToApply", "PerkToApplyFormKey"))
            .AddRule(ValidationFieldRule.Field("EquipAbility", "EquipAbilityFormKey"))
            .AddRule(ValidationFieldRule.Field("Explosion", "ExplosionFormKey"))
            .AddRule(ValidationFieldRule.Field("CastingArt", "CastingArtFormKey"))
            .AddRule(ValidationFieldRule.Field("HitEffectArt", "HitEffectArtFormKey"))
            .AddRule(ValidationFieldRule.Field("HitShader", "HitShaderFormKey"))
            .AddRule(ValidationFieldRule.Field("ImageSpaceModifier", "ImageSpaceModifierFormKey"))
            .AddRule(ValidationFieldRule.Field("ImpactData", "ImpactDataFormKey"))
            .AddRule(ValidationFieldRule.Field("Projectile", "ProjectileFormKey"))
            .AddRule(ValidationFieldRule.Field("Archetype.Type", "Archetype"))
            .AddRule(ValidationFieldRule.Field("Archetype.ActorValue", "ArchetypeActorValue"))
            .AddRule(ValidationFieldRule.Field("Archetype.Association", "ArchetypeAssociationFormKey"))
            .AddRule(ValidationFieldRule.Field("UnknownFloat1", "UnknownFloat1"))
            .AddRule(ValidationFieldRule.Field("UnknownFloat3", "UnknownFloat3"))
            .AddRule(ValidationFieldRule.Field("UnknownFloat4", "UnknownFloat4"))
            .AddRule(ValidationFieldRule.Field("UnknownInt2", "UnknownInt2"))
            .AddRule(ValidationFieldRule.Field("UnknownInt3", "UnknownInt3"))
            .AddRule(ValidationFieldRule.Field("Unknown", "Unknown", ValidationValueNormalizer.HexPayload))
            .AddRule(ValidationFieldRule.Field("Unknown2", "Unknown2", ValidationValueNormalizer.HexPayload))
            .AddRule(ValidationFieldRule.ScalarList("DATADataTypeState", "DataTypeState"))
            .AddRule(ValidationFieldRule.FormKeyList("Keywords", "Keywords", "Keyword"))
            .AddRule(ValidationFieldRule.IgnoreSpriggit("Archetype.MutagenObjectType", "Spriggit serializes the archetype object discriminator; Archetype.Type is the persisted semantic field."))
            .AddRules(GetScriptingAdapterRules(sampleName))
            .AddRules(GetConditionRules())
            .AddRule(ValidationFieldRule.IgnoreSpriggit("Sounds", "Spriggit emits an empty Sounds list scalar when no sound entries are present."))
            .AddRule(ValidationFieldRule.IgnoreSpriggit("Sounds.Count", "Indexed sound rules validate the individual sound entries."))
            .AddRule(ValidationFieldRule.IgnoreDto("Sounds.Count", "Indexed sound rules validate the individual sound entries."))
            .AddRules(GetIndexedSoundRules())
            .AddRule(ValidationFieldRule.IgnoreDtoPrefix("LocalizedStrings", "LocalizedStrings is the DTO projection of translated Spriggit fields."));
    }

    private static IEnumerable<ValidationFieldRule> GetScriptingAdapterRules(string sampleName)
    {
        if (string.Equals(sampleName, "PerkPainTrainKnockbackEffect", StringComparison.Ordinal))
        {
            yield return ValidationFieldRule.Field("VirtualMachineAdapter.Scripts.Count", "ScriptingAdapters.Count");
            yield return ValidationFieldRule.Field("VirtualMachineAdapter.Scripts[0].Name", "ScriptingAdapters[2].Name");
            yield return ValidationFieldRule.Field("VirtualMachineAdapter.Scripts[0].Properties.Count", "ScriptingAdapters[2].Properties.Count");
            yield return ValidationFieldRule.PathPrefix("VirtualMachineAdapter.Scripts[0].Properties", "ScriptingAdapters[2].Properties", ScriptingAdapterPathReplacements);
            yield return ValidationFieldRule.Field("VirtualMachineAdapter.Scripts[1].Name", "ScriptingAdapters[1].Name");
            yield return ValidationFieldRule.Field("VirtualMachineAdapter.Scripts[1].Properties.Count", "ScriptingAdapters[1].Properties.Count");
            yield return ValidationFieldRule.PathPrefix("VirtualMachineAdapter.Scripts[1].Properties", "ScriptingAdapters[1].Properties", ScriptingAdapterPathReplacements);
            yield return ValidationFieldRule.Field("VirtualMachineAdapter.Scripts[2].Name", "ScriptingAdapters[0].Name");
            yield return ValidationFieldRule.Field("VirtualMachineAdapter.Scripts[2].Properties.Count", "ScriptingAdapters[0].Properties.Count");
            yield return ValidationFieldRule.PathPrefix("VirtualMachineAdapter.Scripts[2].Properties", "ScriptingAdapters[0].Properties", ScriptingAdapterPathReplacements);
            yield break;
        }

        yield return ValidationFieldRule.PathPrefix("VirtualMachineAdapter.Scripts", "ScriptingAdapters", ScriptingAdapterPathReplacements);
    }

    private static IEnumerable<ValidationFieldRule> GetConditionRules()
    {
        yield return ValidationFieldRule.PathPrefix("Conditions", "Conditions", NoPathReplacements);
        for (var conditionIndex = 0; conditionIndex <= 100; conditionIndex++)
        {
            var indexText = conditionIndex.ToString(System.Globalization.CultureInfo.InvariantCulture);
            var conditionPath = "Conditions[" + indexText + "]";
            yield return ValidationFieldRule.Field(conditionPath + ".ComparisonValue", conditionPath + ".ComparisonValue");
            yield return ValidationFieldRule.Field(
                conditionPath + ".Data.ParameterOneNumber",
                conditionPath + ".Data.ParameterOneNumber",
                ValidationValueNormalizer.DecimalFormKeyId);
            yield return ValidationFieldRule.Field(conditionPath + ".Data.ParameterOneRecord", conditionPath + ".Data.ParameterOneRecord");
            yield return ValidationFieldRule.IgnoreDto(
                conditionPath + ".Data.FirstUnusedIntParameter",
                "Mutagen exposes the default unused condition parameter when Spriggit omits it.");
        }
    }

    private static IEnumerable<ValidationFieldRule> GetConditionPathRules(
        int conditionIndex,
        string spriggitPath,
        string dtoPath,
        ValidationValueNormalizer normalizer = ValidationValueNormalizer.None)
    {
        yield return ValidationFieldRule.Field(
            spriggitPath + "[" + conditionIndex.ToString(System.Globalization.CultureInfo.InvariantCulture) + "]",
            dtoPath,
            normalizer);
    }

    private static IEnumerable<ValidationFieldRule> GetIndexedSoundRules()
    {
        for (var soundIndex = 0; soundIndex <= 5; soundIndex++)
        {
            var indexText = soundIndex.ToString(System.Globalization.CultureInfo.InvariantCulture);
            yield return ValidationFieldRule.OptionalField("Sounds[" + indexText + "].Type", "Sounds[" + indexText + "].SoundSlot");
            yield return ValidationFieldRule.IgnoreDto(
                "Sounds[" + indexText + "].SoundSlot",
                "The sound slot is an indexed DTO projection when Spriggit omits MGEF Sounds.Type.");
            yield return ValidationFieldRule.ScalarList("Sounds[" + indexText + "].Versioning", "Sounds[" + indexText + "].Versioning");
            yield return ValidationFieldRule.OptionalField("Sounds[" + indexText + "].Sound", "Sounds[" + indexText + "].Start");
            yield return ValidationFieldRule.OptionalField("Sounds[" + indexText + "].Sound.Start", "Sounds[" + indexText + "].Start");
            yield return ValidationFieldRule.OptionalField("Sounds[" + indexText + "].Sound.Stop", "Sounds[" + indexText + "].Stop");
            yield return ValidationFieldRule.OptionalField(
                "Sounds[" + indexText + "].Unknown",
                "Sounds[" + indexText + "].Unknown",
                ValidationValueNormalizer.HexPayload);
        }
    }
}
