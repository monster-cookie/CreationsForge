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

    private static readonly IReadOnlyDictionary<string, string> NoPathReplacements =
        new Dictionary<string, string>(StringComparer.Ordinal);

    private static readonly IReadOnlyDictionary<string, string> ItemPathReplacements =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [".Item.Item"] = ".Item",
            [".Item.Count"] = ".Count"
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
            .AddRule(ValidationFieldRule.OptionalField("Skin", "Skin"))
            .AddRule(ValidationFieldRule.OptionalField("Pronoun", "Pronoun"))
            .AddRule(ValidationFieldRule.OptionalField("Voice", "VoiceFormKey"))
            .AddRule(ValidationFieldRule.OptionalField("Race", "RaceFormKey"))
            .AddRule(ValidationFieldRule.OptionalField("AttackRace", "AttackRace"))
            .AddRule(ValidationFieldRule.OptionalField("CombatOverridePackageList", "CombatOverridePackageListFormKey"))
            .AddRule(ValidationFieldRule.OptionalField("CombatStyle", "CombatStyleFormKey"))
            .AddRule(ValidationFieldRule.OptionalField("DefaultPackageList", "DefaultPackageListFormKey"))
            .AddRule(ValidationFieldRule.OptionalField("CrimeFaction", "CrimeFactionFormKey"))
            .AddRule(ValidationFieldRule.OptionalField("Class", "Class"))
            .AddRule(ValidationFieldRule.OptionalField("DeathItem", "DeathItem"))
            .AddRule(ValidationFieldRule.OptionalField("DefaultOutfit", "DefaultOutfit"))
            .AddRule(ValidationFieldRule.OptionalField("SleepingOutfit", "SleepingOutfit"))
            .AddRule(ValidationFieldRule.OptionalField("WornArmor", "WornArmor"))
            .AddRule(ValidationFieldRule.OptionalField("PowerArmorStand", "PowerArmorStand"))
            .AddRule(ValidationFieldRule.OptionalField("SpaceOutfit", "SpaceOutfit"))
            .AddRule(ValidationFieldRule.OptionalField("HeadTexture", "HeadTexture"))
            .AddRule(ValidationFieldRule.OptionalField("Template", "Template"))
            .AddRule(ValidationFieldRule.OptionalField("DefaultTemplate", "DefaultTemplate"))
            .AddRule(ValidationFieldRule.FormKeyList("Keywords", "Keywords", "Keyword"))
            .AddRule(ValidationFieldRule.FormKeyList("Packages", "Packages", string.Empty))
            .AddRule(ValidationFieldRule.FormKeyList("ForcedLocations", "ForcedLocations", string.Empty))
            .AddRule(ValidationFieldRule.FormKeyList("HeadParts", "HeadParts", string.Empty))
            .AddRule(ValidationFieldRule.FormKeyList("ActorEffect", "ActorEffects", string.Empty))
            .AddRule(ValidationFieldRule.SoundSlot("Sound.Start", "Sound", "Start"))
            .AddRule(ValidationFieldRule.SoundSlot("Sound.MutagenObjectType", "Sound", "MutagenObjectType"))
            .AddRule(ValidationFieldRule.SoundSlot("Sound.InheritsSoundsFrom", "Sound", "InheritsSoundsFrom"))
            .AddRule(ValidationFieldRule.PathPrefix("VirtualMachineAdapter.Scripts", "ScriptingAdapters", ScriptingAdapterPathReplacements))
            .AddRules(GetNPCTypedRules(game))
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

    private static IEnumerable<ValidationFieldRule> GetNPCTypedRules(SupportedGame game)
    {
        yield return ValidationFieldRule.OptionalField("IsCompressed", "IsCompressed");
        yield return ValidationFieldRule.OptionalField("ObjectBounds.First", "ObjectBoundsFirst");
        yield return ValidationFieldRule.OptionalField("ObjectBounds.Second", "ObjectBoundsSecond");
        yield return game == SupportedGame.Starfield
            ? ValidationFieldRule.ScalarList("Flags", "Flags", ValidationValueNormalizer.MajorFlagList)
            : ValidationFieldRule.ScalarList("Flags", "Flags");
        yield return ValidationFieldRule.ScalarList("MajorFlags", "MajorFlags", ValidationValueNormalizer.MajorFlagList);
        var gameMajorRecordFlags = game switch
        {
            SupportedGame.Fallout4 => "Fallout4MajorRecordFlags",
            SupportedGame.Skyrim => "SkyrimMajorRecordFlags",
            _ => "StarfieldMajorRecordFlags"
        };
        yield return ValidationFieldRule.ScalarList(gameMajorRecordFlags, "MajorRecordFlags", ValidationValueNormalizer.MajorFlagList);

        yield return ValidationFieldRule.PathPrefix("Level", "Level", NoPathReplacements, ValidationValueNormalizer.DecimalNumber);
        yield return ValidationFieldRule.PathPrefix("Configuration", "Configuration", NoPathReplacements, ValidationValueNormalizer.DecimalNumber);
        if (game == SupportedGame.Skyrim)
        {
            yield return ValidationFieldRule.DtoDefaultWhenSpriggitAbsent(
                "Configuration.TemplateFlags",
                "Configuration.TemplateFlags.Count",
                "1",
                "Mutagen exposes the default zero template flag when Spriggit omits template flags.");
            yield return ValidationFieldRule.DtoDefaultWhenSpriggitAbsent(
                "Configuration.TemplateFlags",
                "Configuration.TemplateFlags[0]",
                "0",
                "Mutagen exposes the default zero template flag when Spriggit omits template flags.");
        }

        yield return ValidationFieldRule.PathPrefix("TemplateActors", "TemplateActors", NoPathReplacements);
        yield return ValidationFieldRule.OptionalField("UseTemplateActors", "UseTemplateActors");
        if (game == SupportedGame.Fallout4)
        {
            yield return ValidationFieldRule.DtoDefaultWhenSpriggitAbsent(
                "UseTemplateActors",
                "UseTemplateActors",
                "0",
                "Fallout 4 Mutagen exposes default template-actor flags when Spriggit omits them.");
        }

        yield return ValidationFieldRule.OptionalField("CalculatedHealth", "CalculatedHealth");
        yield return ValidationFieldRule.OptionalField("CalculatedActionPoints", "CalculatedActionPoints");
        yield return ValidationFieldRule.OptionalField("XpValueOffset", "XpValueOffset");
        if (game == SupportedGame.Fallout4)
        {
            yield return ValidationFieldRule.DtoDefaultWhenSpriggitAbsent(
                "XpValueOffset",
                "XpValueOffset",
                "0",
                "Fallout 4 Mutagen exposes the default experience offset when Spriggit omits it.");
        }

        yield return ValidationFieldRule.OptionalField("Unknown", "Unknown");
        yield return ValidationFieldRule.OptionalField("Unused", "Unused");
        if (game == SupportedGame.Fallout4)
        {
            yield return ValidationFieldRule.DtoDefaultWhenSpriggitAbsent(
                "Unused",
                "Unused",
                "0",
                "Fallout 4 Mutagen exposes the default unused value when Spriggit omits it.");
        }

        yield return ValidationFieldRule.OptionalField("NAM5", "NAM5", ValidationValueNormalizer.HexInteger);
        yield return ValidationFieldRule.OptionalField("Height", "Height", ValidationValueNormalizer.DecimalNumber);
        yield return ValidationFieldRule.PathPrefix("Weight", "Weight", NoPathReplacements, ValidationValueNormalizer.DecimalNumber);
        yield return ValidationFieldRule.OptionalField("SoundLevel", "SoundLevel");
        yield return ValidationFieldRule.OptionalField("TextureLighting", "TextureLighting", ValidationValueNormalizer.Color);
        yield return ValidationFieldRule.OptionalField("HairColor", "HairColor");
        yield return ValidationFieldRule.OptionalField("FacialHairColor", "FacialHairColor");
        yield return ValidationFieldRule.OptionalField("EyebrowColor", "EyebrowColor");
        yield return ValidationFieldRule.OptionalField("EyeColor", "EyeColor");
        yield return ValidationFieldRule.PathPrefix("FaceMorph", "FaceMorph", NoPathReplacements, ValidationValueNormalizer.DecimalNumber);
        yield return ValidationFieldRule.PathPrefix("FaceParts", "FaceParts", NoPathReplacements);
        yield return ValidationFieldRule.PathPrefix("Factions", "Factions", NoPathReplacements);
        yield return ValidationFieldRule.PathPrefix("Properties", "Properties", NoPathReplacements);
        yield return ValidationFieldRule.CanonicalFormKeyCountList("Items", "Items", ItemPathReplacements);
        yield return ValidationFieldRule.PathPrefix("Perks", "Perks", NoPathReplacements);
        yield return ValidationFieldRule.PathPrefix("Morphs", "Morphs", NoPathReplacements, ValidationValueNormalizer.DecimalNumber);
        yield return ValidationFieldRule.PathPrefix("FaceDialPositions", "FaceDialPositions", NoPathReplacements, ValidationValueNormalizer.DecimalNumber);
        if (game == SupportedGame.Starfield)
        {
            yield return ValidationFieldRule.PathPrefix("FaceMorphs", "FaceMorphGroups", NoPathReplacements, ValidationValueNormalizer.DecimalNumber);
            yield return ValidationFieldRule.IgnoreDto("FaceMorphs.Count", "Starfield FaceMorphs are projected through FaceMorphGroups for repository read-back.");
            for (var faceMorphIndex = 0; faceMorphIndex < 64; faceMorphIndex++)
            {
                for (var morphGroupIndex = 0; morphGroupIndex < 16; morphGroupIndex++)
                {
                    var path = "FaceMorphGroups[" + faceMorphIndex.ToString(System.Globalization.CultureInfo.InvariantCulture) + "].MorphGroups[" + morphGroupIndex.ToString(System.Globalization.CultureInfo.InvariantCulture) + "].FaceMorphIndex";
                    yield return ValidationFieldRule.IgnoreDto(path, "FaceMorphIndex is DTO collection metadata for repository read-back.");
                }
            }
        }
        else
        {
            yield return ValidationFieldRule.PathPrefix("FaceMorphs", "FaceMorphs", NoPathReplacements, ValidationValueNormalizer.DecimalNumber);
        }

        yield return ValidationFieldRule.PathPrefix("MorphBlends", "MorphBlends", NoPathReplacements, ValidationValueNormalizer.FloatNumber);
        yield return ValidationFieldRule.PathPrefix("Tints", "Tints", NoPathReplacements, ValidationValueNormalizer.ColorOrDecimalNumber);
        yield return ValidationFieldRule.PathPrefix("TintLayers", "TintLayers", NoPathReplacements, ValidationValueNormalizer.ColorOrDecimalNumber);
        yield return ValidationFieldRule.PathPrefix("FaceTintingLayers", "FaceTintingLayers", NoPathReplacements, ValidationValueNormalizer.ColorOrDecimalNumber);
        if (game == SupportedGame.Fallout4)
        {
            for (var index = 0; index < 64; index++)
            {
                var path = "FaceTintingLayers[" + index.ToString(System.Globalization.CultureInfo.InvariantCulture) + "].FaceTintingLayerIndex";
                yield return ValidationFieldRule.IgnoreDto(path, "FaceTintingLayerIndex is DTO collection metadata for repository read-back.");
            }
        }

        yield return ValidationFieldRule.OptionalField("PlayerSkills.GearedUpWeapons", "PlayerSkills.GearedUpWeapons", ValidationValueNormalizer.DecimalNumber);
        yield return ValidationFieldRule.PathPrefix("PlayerSkills", "PlayerSkills", NoPathReplacements, ValidationValueNormalizer.DecimalNumber);

        var diagnosticAggregatePaths = new[]
        {
            "BodyMorphRegionValues",
            "ObjectTemplates"
        };

        foreach (var path in diagnosticAggregatePaths)
        {
            yield return ValidationFieldRule.DtoNonEmpty(path, path);
        }
    }
}
