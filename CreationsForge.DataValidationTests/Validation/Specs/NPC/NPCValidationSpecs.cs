using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;

namespace CreationsForge.DataValidationTests.Validation.Specs.NPC;

public static class NPCValidationSpecs
{
    private static readonly IReadOnlyDictionary<string, string> ScriptingAdapterPathReplacements =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [".Objects"] = ".ListItems",
            [".Object"] = ".ObjectFormKey",
            [".Alias"] = ".ObjectAlias",
            [".Data"] = ".DataInt"
        };

    public static ValidationSpec Starfield_CF_AludraTahan()
    {
        return StarfieldNPC("CF_AludraTahan", "01539F:Starfield.esm").Build();
    }

    public static ValidationSpec Starfield_CF_CESandin()
    {
        return StarfieldNPC("CF_CESandin", "0A0273:Starfield.esm").Build();
    }

    public static ValidationSpec Starfield_CF_CPMurata()
    {
        return StarfieldNPC("CF_CPMurata", "09C32F:Starfield.esm").Build();
    }

    public static ValidationSpec Starfield_BE_FAB12_LvlCitizenChunks()
    {
        return StarfieldNPC("BE_FAB12_LvlCitizenChunks", "0B6667:Starfield.esm").Build();
    }

    public static ValidationSpec Starfield_BQ01_Actor_EllieYankton()
    {
        return StarfieldNPC("BQ01_Actor_EllieYankton", "17C10E:Starfield.esm").Build();
    }

    public static ValidationSpec Fallout4_BHExtBOSSoldier()
    {
        return Fallout4NPC("BHExtBOSSoldier", "0FB232:Fallout4.esm").Build();
    }

    public static ValidationSpec Fallout4_BHExtBOSSoldier_PowerArmorAuto()
    {
        return Fallout4NPC("BHExtBOSSoldier_PowerArmorAuto", "0FB22E:Fallout4.esm").Build();
    }

    public static ValidationSpec Fallout4_BHExtBOSSoldier_PowerArmorBigGun()
    {
        return Fallout4NPC("BHExtBOSSoldier_PowerArmorBigGun", "1D58EA:Fallout4.esm").Build();
    }

    public static ValidationSpec Fallout4_AllieFilmore()
    {
        return Fallout4NPC("AllieFilmore", "05E557:Fallout4.esm").Build();
    }

    public static ValidationSpec Fallout4_AudioTemplateSynthGen1()
    {
        return Fallout4NPC("AudioTemplateSynthGen1", "240C21:Fallout4.esm").Build();
    }

    public static ValidationSpec Skyrim_EncGuardImperialTemplate()
    {
        return SkyrimNPC("EncGuardImperialTemplate", "0F6F37:Skyrim.esm").Build();
    }

    public static ValidationSpec Skyrim_EncGuardSonsTemplate()
    {
        return SkyrimNPC("EncGuardSonsTemplate", "0F6F38:Skyrim.esm").Build();
    }

    public static ValidationSpec Skyrim_EncSiegeImperialSoldierTemplate()
    {
        return SkyrimNPC("EncSiegeImperialSoldierTemplate", "041B30:Skyrim.esm").Build();
    }

    public static ValidationSpec Skyrim_AelaTheHuntress()
    {
        return SkyrimNPC("AelaTheHuntress", "01A696:Skyrim.esm").Build();
    }

    public static ValidationSpec Skyrim_AlduinBase()
    {
        return SkyrimNPC("AlduinBase", "08E4F1:Skyrim.esm").Build();
    }

    private static ValidationSpecBuilder StarfieldNPC(string sampleName, string formKey)
    {
        return BaseNPC(SupportedGame.Starfield, sampleName, formKey);
    }

    private static ValidationSpecBuilder Fallout4NPC(string sampleName, string formKey)
    {
        return BaseNPC(SupportedGame.Fallout4, sampleName, formKey);
    }

    private static ValidationSpecBuilder SkyrimNPC(string sampleName, string formKey)
    {
        return BaseNPC(SupportedGame.Skyrim, sampleName, formKey);
    }

    private static ValidationSpecBuilder BaseNPC(SupportedGame game, string sampleName, string formKey)
    {
        return ValidationSpecBuilder
            .ForRecord(game, RecordTypeCatalog.NPC)
            .Sample(sampleName)
            .FormKey(formKey)
            .AddRule(ValidationFieldRule.TranslatedField("Name", "Name", requireAllLanguages: true))
            .AddRule(ValidationFieldRule.TranslatedField("ShortName", "ShortName", requireAllLanguages: true))
            .AddRule(ValidationFieldRule.TranslatedField("LongName", "LongName", requireAllLanguages: true))
            .AddRule(ValidationFieldRule.OptionalField("Version2", "Version2"))
            .AddRule(ValidationFieldRule.DtoDefaultWhenSpriggitAbsent(
                "Version2",
                "Version2",
                "0",
                "Mutagen exposes default Version2 when Spriggit omits it."))
            .AddRule(ValidationFieldRule.Field("VersionControl", "VersionControl"))
            .AddRule(ValidationFieldRule.DtoDefaultWhenSpriggitAbsent(
                "MajorRecordFlagsRaw",
                "MajorRecordFlags",
                "0",
                "Mutagen exposes the default MajorRecordFlags value when Spriggit omits the zero-valued field."))
            .AddRule(ValidationFieldRule.Field("MajorRecordFlagsRaw", "MajorRecordFlags"))
            .AddRule(ValidationFieldRule.OptionalField("DispositionBase", "DispositionBase"))
            .AddRule(ValidationFieldRule.OptionalField("Configuration.DispositionBase", "DispositionBase"))
            .AddRule(ValidationFieldRule.OptionalField("Aggression", "Aggression"))
            .AddRule(ValidationFieldRule.OptionalField("AIData.Aggression", "Aggression"))
            .AddRule(ValidationFieldRule.OptionalField("Confidence", "Confidence"))
            .AddRule(ValidationFieldRule.OptionalField("AIData.Confidence", "Confidence"))
            .AddRule(ValidationFieldRule.OptionalField("EnergyLevel", "EnergyLevel"))
            .AddRule(ValidationFieldRule.OptionalField("AIData.EnergyLevel", "EnergyLevel"))
            .AddRule(ValidationFieldRule.OptionalField("Responsibility", "Responsibility"))
            .AddRule(ValidationFieldRule.OptionalField("AIData.Responsibility", "Responsibility"))
            .AddRule(ValidationFieldRule.OptionalField("Assistance", "Assistance"))
            .AddRule(ValidationFieldRule.OptionalField("AIData.Assistance", "Assistance"))
            .AddRule(ValidationFieldRule.OptionalField("AIData.Mood", "Mood"))
            .AddRule(ValidationFieldRule.DtoDefaultWhenSpriggitAbsent(
                "AIData.Mood",
                "Mood",
                "Neutral",
                "Mutagen exposes the default NPC mood when Spriggit omits it."))
            .AddRule(ValidationFieldRule.OptionalField("GearedUpWeapons", "GearedUpWeapons"))
            .AddRule(ValidationFieldRule.OptionalField("PlayerSkills.GearedUpWeapons", "GearedUpWeapons"))
            .AddRule(ValidationFieldRule.OptionalField("HeightMin", "HeightMin", ValidationValueNormalizer.DecimalNumber))
            .AddRule(ValidationFieldRule.OptionalField("HeightMax", "HeightMax", ValidationValueNormalizer.DecimalNumber))
            .AddRule(ValidationFieldRule.OptionalField("SkinToneIndex", "SkinToneIndex"))
            .AddRule(ValidationFieldRule.OptionalField("Pronoun", "Pronoun"))
            .AddRule(ValidationFieldRule.OptionalField("Voice", "VoiceFormKey"))
            .AddRule(ValidationFieldRule.OptionalField("Race", "RaceFormKey"))
            .AddRule(ValidationFieldRule.OptionalField("CombatOverridePackageList", "CombatOverridePackageListFormKey"))
            .AddRule(ValidationFieldRule.OptionalField("CombatStyle", "CombatStyleFormKey"))
            .AddRule(ValidationFieldRule.OptionalField("DefaultPackageList", "DefaultPackageListFormKey"))
            .AddRule(ValidationFieldRule.OptionalField("CrimeFaction", "CrimeFactionFormKey"))
            .AddRule(ValidationFieldRule.FormKeyList("Keywords", "Keywords", "Keyword"))
            .AddRule(ValidationFieldRule.SoundSlot("Sound.Start", "Sound", "Start"))
            .AddRule(ValidationFieldRule.SoundSlot("Sound.MutagenObjectType", "Sound", "MutagenObjectType"))
            .AddRule(ValidationFieldRule.SoundSlot("Sound.InheritsSoundsFrom", "Sound", "InheritsSoundsFrom"))
            .AddRule(ValidationFieldRule.PathPrefix("VirtualMachineAdapter.Scripts", "ScriptingAdapters", ScriptingAdapterPathReplacements))
            .AddRules(GetNPCStructuredValueRules())
            .AddRules(GetUnmodeledNPCSpriggitIgnores())
            .AddRule(ValidationFieldRule.IgnoreDto("DispositionBase", "NPC DTO stores a default value when no Spriggit disposition field is present."))
            .AddRule(ValidationFieldRule.IgnoreDto("Aggression", "NPC DTO stores a default value when no Spriggit AI data field is present."))
            .AddRule(ValidationFieldRule.IgnoreDto("Confidence", "NPC DTO stores a default value when no Spriggit AI data field is present."))
            .AddRule(ValidationFieldRule.IgnoreDto("EnergyLevel", "NPC DTO stores a default value when no Spriggit AI data field is present."))
            .AddRule(ValidationFieldRule.IgnoreDto("Responsibility", "NPC DTO stores a default value when no Spriggit AI data field is present."))
            .AddRule(ValidationFieldRule.IgnoreDto("Assistance", "NPC DTO stores a default value when no Spriggit AI data field is present."))
            .AddRule(ValidationFieldRule.IgnoreDto("AIData", "AIData is the Mutagen aggregate string; individual AI data fields are validated separately."))
            .AddRule(ValidationFieldRule.IgnoreDto("GearedUpWeapons", "NPC DTO stores a default value when no Spriggit geared-up-weapons field is present."))
            .AddRule(ValidationFieldRule.IgnoreDto("HeightMin", "NPC DTO stores a default value when no Spriggit height range field is present."))
            .AddRule(ValidationFieldRule.IgnoreDto("HeightMax", "NPC DTO stores a default value when no Spriggit height range field is present."))
            .AddRule(ValidationFieldRule.IgnoreDtoPrefix("LocalizedStrings", "LocalizedStrings is the DTO projection of translated Spriggit fields."));
    }

    private static IEnumerable<ValidationFieldRule> GetUnmodeledNPCSpriggitIgnores()
    {
        var reason = "The current NPC DTO persists the shared comparison surface, not the full actor appearance, inventory, package, skill, or template model.";
        var prefixes = new[]
        {
            "ActorEffect",
            "AttackRace",
            "CalculatedActionPoints",
            "CalculatedHealth",
            "Class",
            "Configuration",
            "DeathItem",
            "DefaultOutfit",
            "EyebrowColor",
            "EyeColor",
            "FaceDialPositions",
            "FaceMorphs",
            "FaceTintingLayers",
            "FacialHairColor",
            "Factions",
            "Flags",
            "ForcedLocations",
            "HairColor",
            "Height",
            "IsCompressed",
            "Items",
            "Level",
            "MajorFlags",
            "MorphBlends",
            "Morphs",
            "NAM5",
            "ObjectBounds",
            "Packages",
            "Perks",
            "PlayerSkills",
            "PowerArmorStand",
            "Properties",
            "Skin",
            "SoundLevel",
            "StarfieldMajorRecordFlags",
            "Fallout4MajorRecordFlags",
            "SkyrimMajorRecordFlags",
            "TextureLighting",
            "Unknown",
            "Unused",
            "UseTemplateActors",
            "Weight",
            "XpValueOffset"
        };

        foreach (var prefix in prefixes)
        {
            yield return ValidationFieldRule.IgnoreSpriggitPrefix(prefix, reason);
        }
    }

    private static IEnumerable<ValidationFieldRule> GetNPCStructuredValueRules()
    {
        var paths = new[]
        {
            "BodyMorphRegionValues",
            "DefaultTemplate",
            "FaceMorph",
            "FaceParts",
            "HeadParts",
            "HeadTexture",
            "ObjectTemplates",
            "SleepingOutfit",
            "SpaceOutfit",
            "Template",
            "TemplateActors",
            "TintLayers",
            "Tints",
            "WornArmor"
        };

        foreach (var path in paths)
        {
            yield return ValidationFieldRule.DtoNonEmpty(path, path);
        }
    }
}
