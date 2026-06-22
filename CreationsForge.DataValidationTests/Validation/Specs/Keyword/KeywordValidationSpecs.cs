using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;

namespace CreationsForge.DataValidationTests.Validation.Specs.Keyword;

public static class KeywordValidationSpecs
{
    public static ValidationSpec Starfield_CCT_Enviro_AmbusherSurface()
    {
        return Keyword(SupportedGame.Starfield, "CCT_Enviro_AmbusherSurface", "200AEB:Starfield.esm");
    }

    public static ValidationSpec Starfield_CCT_Enviro_AmbusherUnderground()
    {
        return Keyword(SupportedGame.Starfield, "CCT_Enviro_AmbusherUnderground", "145388:Starfield.esm");
    }

    public static ValidationSpec Starfield_CCT_Enviro_Basking()
    {
        return Keyword(SupportedGame.Starfield, "CCT_Enviro_Basking", "200ADF:Starfield.esm");
    }

    public static ValidationSpec Starfield_WeaponTypeDisplay_ElectromagneticRifle()
    {
        return Keyword(
            SupportedGame.Starfield,
            "WeaponTypeDisplay_ElectromagneticRifle",
            "1C84DD:Starfield.esm",
            withAdaptiveTriggerDataComponent: true);
    }

    public static ValidationSpec Starfield_CCT_Enviro_Spook()
    {
        return Keyword(SupportedGame.Starfield, "CCT_Enviro_Spook", "200AE9:Starfield.esm");
    }

    public static ValidationSpec Starfield_ActorAttackInjuredLeft()
    {
        return Keyword(SupportedGame.Starfield, "ActorAttackInjuredLeft", "0345AE:Starfield.esm");
    }

    public static ValidationSpec Starfield_ActorTypeChild()
    {
        return Keyword(SupportedGame.Starfield, "ActorTypeChild", "1157E8:Starfield.esm");
    }

    public static ValidationSpec Starfield_AnimArchetypeEyeDown()
    {
        return Keyword(SupportedGame.Starfield, "AnimArchetypeEyeDown", "24E96F:Starfield.esm");
    }

    public static ValidationSpec Starfield_ap_AVM_Armor_Skin()
    {
        return Keyword(SupportedGame.Starfield, "ap_AVM_Armor_Skin", "157D41:Starfield.esm");
    }

    public static ValidationSpec Fallout4_02Metal03Floor()
    {
        return Keyword(SupportedGame.Fallout4, "02Metal03Floor", "119B9B:Fallout4.esm");
    }

    public static ValidationSpec Fallout4_02Metal03Misc()
    {
        return Keyword(SupportedGame.Fallout4, "02Metal03Misc", "119B9C:Fallout4.esm");
    }

    public static ValidationSpec Fallout4_02Metal03Prefabs()
    {
        return Keyword(SupportedGame.Fallout4, "02Metal03Prefabs", "119B9D:Fallout4.esm");
    }

    public static ValidationSpec Fallout4_AO_BoS_ScribeCollectData()
    {
        return Keyword(SupportedGame.Fallout4, "AO_BoS_ScribeCollectData", "0CF43E:Fallout4.esm");
    }

    public static ValidationSpec Fallout4_if_Armor_Combat_Freefall_Restricted()
    {
        return Keyword(
            SupportedGame.Fallout4,
            "if_Armor_Combat_Freefall_Restricted",
            "093BBE:Fallout4.esm",
            withMajorRecordFlagsRaw: true);
    }

    public static ValidationSpec Fallout4_02Metal03Wall()
    {
        return Keyword(SupportedGame.Fallout4, "02Metal03Wall", "119BA0:Fallout4.esm");
    }

    public static ValidationSpec Fallout4_ActorTypeChild()
    {
        return Keyword(SupportedGame.Fallout4, "ActorTypeChild", "1157E8:Fallout4.esm");
    }

    public static ValidationSpec Fallout4_AnimArchetypeNervous()
    {
        return Keyword(SupportedGame.Fallout4, "AnimArchetypeNervous", "03D28F:Fallout4.esm");
    }

    public static ValidationSpec Fallout4_ap_Bot_ModLegsSlotB()
    {
        return Keyword(SupportedGame.Fallout4, "ap_Bot_ModLegsSlotB", "072B3F:Fallout4.esm");
    }

    public static ValidationSpec Skyrim_ActorTypeFamiliar()
    {
        return Keyword(SupportedGame.Skyrim, "ActorTypeFamiliar", "10EAD7:Skyrim.esm");
    }

    public static ValidationSpec Skyrim_ActorTypeGiant()
    {
        return Keyword(SupportedGame.Skyrim, "ActorTypeGiant", "10E984:Skyrim.esm");
    }

    public static ValidationSpec Skyrim_ActorTypeTroll()
    {
        return Keyword(SupportedGame.Skyrim, "ActorTypeTroll", "0F5D16:Skyrim.esm");
    }

    public static ValidationSpec Skyrim_ActivatorLever()
    {
        return Keyword(SupportedGame.Skyrim, "ActivatorLever", "06DEAD:Skyrim.esm", withColor: false);
    }

    private static ValidationSpec Keyword(
        SupportedGame game,
        string sampleName,
        string formKey,
        bool withColor = true,
        bool withMajorRecordFlagsRaw = false,
        bool withAdaptiveTriggerDataComponent = false)
    {
        var spec = ValidationSpecBuilder
            .ForRecord(game, RecordTypeCatalog.Keyword)
            .Sample(sampleName)
            .FormKey(formKey)
            .AddRule(ValidationFieldRule.DtoDefaultWhenSpriggitAbsent(
                "Version2",
                "Version2",
                "0",
                "Mutagen exposes the default Version2 value when Spriggit omits the zero-valued field."))
            .AddRule(ValidationFieldRule.TranslatedField("Name", "Name", requireAllLanguages: true))
            .AddRule(ValidationFieldRule.IgnoreDtoPrefix("LocalizedStrings", "LocalizedStrings is the DTO projection of translated Spriggit fields."));

        if (game == SupportedGame.Starfield)
        {
            spec.AddRule(ValidationFieldRule.Field("FNAM", "FNAM", ValidationValueNormalizer.HexPayload));
        }

        if (withColor)
        {
            spec.AddRule(ValidationFieldRule.Field("Color", "Color", ValidationValueNormalizer.Color));
        }
        else
        {
            spec.AddRule(ValidationFieldRule.IgnoreDto("Color", "Some Keywords read back an empty DTO color when Spriggit omits Color."));
        }

        if (withMajorRecordFlagsRaw)
        {
            spec
                .AddRule(ValidationFieldRule.Field("MajorRecordFlagsRaw", "MajorRecordFlags"))
                .AddRule(ValidationFieldRule.IgnoreSpriggit("Fallout4MajorRecordFlags.Count", "MajorRecordFlagsRaw covers the flag value."))
                .AddRule(ValidationFieldRule.IgnoreSpriggit("Fallout4MajorRecordFlags[0]", "MajorRecordFlagsRaw covers the flag value."))
                .AddRule(ValidationFieldRule.IgnoreSpriggit("MajorFlags.Count", "MajorRecordFlagsRaw covers the flag value."))
                .AddRule(ValidationFieldRule.IgnoreSpriggit("MajorFlags[0]", "MajorRecordFlagsRaw covers the flag value."));
        }
        else
        {
            spec.AddRule(ValidationFieldRule.DtoDefaultWhenSpriggitAbsent(
                "MajorRecordFlags",
                "MajorRecordFlags",
                "0",
                "Mutagen exposes the default MajorRecordFlags value when Spriggit omits the zero-valued field."));
        }

        if (withAdaptiveTriggerDataComponent)
        {
            spec
                .AddRule(ValidationFieldRule.Field("Components[0].WAIM", "WAIM", ValidationValueNormalizer.HexPayload))
                .AddRule(ValidationFieldRule.Field("Components[0].WFIR", "WFIR", ValidationValueNormalizer.HexPayload))
                .AddRule(ValidationFieldRule.IgnoreSpriggit("Components.Count", "The DTO stores the adaptive trigger component payload as scalar WAIM/WFIR fields."))
                .AddRule(ValidationFieldRule.IgnoreSpriggit("Components[0].MutagenObjectType", "The DTO stores the adaptive trigger component payload as scalar WAIM/WFIR fields."));
        }

        return spec.Build();
    }
}
