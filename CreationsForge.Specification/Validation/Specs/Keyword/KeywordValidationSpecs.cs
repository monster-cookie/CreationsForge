using CreationsForge.Specification.Records;
using CreationsForge.Specification.Validation;

namespace CreationsForge.Specification.Validation.Specs.Keyword;

public static class KeywordValidationSpecs
{
    /// <summary>
    /// Builds the Starfield <c>CCT_Enviro_AmbusherSurface</c> keyword validation spec, including a UI color row expectation.
    /// </summary>
    /// <returns>The validation spec for the Starfield <c>CCT_Enviro_AmbusherSurface</c> sample.</returns>
    public static ValidationSpec Starfield_CCT_Enviro_AmbusherSurface()
    {
        var spec = Keyword(SpecificationGame.Starfield, "CCT_Enviro_AmbusherSurface", "200AEB:Starfield.esm");
        spec.UiComparisonExpectations.Add(new ValidationUiComparisonExpectation(["Color"]));
        return spec;
    }

    public static ValidationSpec Starfield_CCT_Enviro_AmbusherUnderground()
    {
        return Keyword(SpecificationGame.Starfield, "CCT_Enviro_AmbusherUnderground", "145388:Starfield.esm");
    }

    public static ValidationSpec Starfield_CCT_Enviro_Basking()
    {
        return Keyword(SpecificationGame.Starfield, "CCT_Enviro_Basking", "200ADF:Starfield.esm");
    }

    public static ValidationSpec Starfield_WeaponTypeDisplay_ElectromagneticRifle()
    {
        var spec = Keyword(
            SpecificationGame.Starfield,
            "WeaponTypeDisplay_ElectromagneticRifle",
            "1C84DD:Starfield.esm",
            withAdaptiveTriggerDataComponent: true);
        spec.UiComparisonExpectations.Add(ValidationUiComparisonExpectation.Literal(["WAIM"], "0x00020603"));
        return spec;
    }

    public static ValidationSpec Starfield_CCT_Enviro_Spook()
    {
        return Keyword(SpecificationGame.Starfield, "CCT_Enviro_Spook", "200AE9:Starfield.esm");
    }

    public static ValidationSpec Starfield_ActorAttackInjuredLeft()
    {
        return Keyword(SpecificationGame.Starfield, "ActorAttackInjuredLeft", "0345AE:Starfield.esm");
    }

    /// <summary>
    /// Builds the Starfield <c>ActorTypeChild</c> keyword validation spec, including a UI editor ID row expectation.
    /// </summary>
    /// <returns>The validation spec for the Starfield <c>ActorTypeChild</c> sample.</returns>
    public static ValidationSpec Starfield_ActorTypeChild()
    {
        var spec = Keyword(SpecificationGame.Starfield, "ActorTypeChild", "1157E8:Starfield.esm");
        spec.UiComparisonExpectations.Add(ValidationUiComparisonExpectation.Literal(["EditorID"], "ActorTypeChild"));
        return spec;
    }

    public static ValidationSpec Starfield_AnimArchetypeEyeDown()
    {
        return Keyword(SpecificationGame.Starfield, "AnimArchetypeEyeDown", "24E96F:Starfield.esm");
    }

    public static ValidationSpec Starfield_ap_AVM_Armor_Skin()
    {
        return Keyword(SpecificationGame.Starfield, "ap_AVM_Armor_Skin", "157D41:Starfield.esm");
    }

    public static ValidationSpec Fallout4_02Metal03Floor()
    {
        return Keyword(SpecificationGame.Fallout4, "02Metal03Floor", "119B9B:Fallout4.esm");
    }

    public static ValidationSpec Fallout4_02Metal03Misc()
    {
        return Keyword(SpecificationGame.Fallout4, "02Metal03Misc", "119B9C:Fallout4.esm");
    }

    public static ValidationSpec Fallout4_02Metal03Prefabs()
    {
        return Keyword(SpecificationGame.Fallout4, "02Metal03Prefabs", "119B9D:Fallout4.esm");
    }

    public static ValidationSpec Fallout4_AO_BoS_ScribeCollectData()
    {
        return Keyword(SpecificationGame.Fallout4, "AO_BoS_ScribeCollectData", "0CF43E:Fallout4.esm");
    }

    public static ValidationSpec Fallout4_if_Armor_Combat_Freefall_Restricted()
    {
        return Keyword(
            SpecificationGame.Fallout4,
            "if_Armor_Combat_Freefall_Restricted",
            "093BBE:Fallout4.esm",
            withMajorRecordFlagsRaw: true);
    }

    public static ValidationSpec Fallout4_02Metal03Wall()
    {
        return Keyword(SpecificationGame.Fallout4, "02Metal03Wall", "119BA0:Fallout4.esm");
    }

    public static ValidationSpec Fallout4_ActorTypeChild()
    {
        return Keyword(SpecificationGame.Fallout4, "ActorTypeChild", "1157E8:Fallout4.esm");
    }

    public static ValidationSpec Fallout4_AnimArchetypeNervous()
    {
        return Keyword(SpecificationGame.Fallout4, "AnimArchetypeNervous", "03D28F:Fallout4.esm");
    }

    public static ValidationSpec Fallout4_ap_Bot_ModLegsSlotB()
    {
        return Keyword(SpecificationGame.Fallout4, "ap_Bot_ModLegsSlotB", "072B3F:Fallout4.esm");
    }

    public static ValidationSpec Skyrim_ActorTypeFamiliar()
    {
        return Keyword(SpecificationGame.Skyrim, "ActorTypeFamiliar", "10EAD7:Skyrim.esm");
    }

    public static ValidationSpec Skyrim_ActorTypeGiant()
    {
        return Keyword(SpecificationGame.Skyrim, "ActorTypeGiant", "10E984:Skyrim.esm");
    }

    public static ValidationSpec Skyrim_ActorTypeTroll()
    {
        return Keyword(SpecificationGame.Skyrim, "ActorTypeTroll", "0F5D16:Skyrim.esm");
    }

    public static ValidationSpec Skyrim_ActivatorLever()
    {
        return Keyword(SpecificationGame.Skyrim, "ActivatorLever", "06DEAD:Skyrim.esm", withColor: false);
    }

    private static ValidationSpec Keyword(
        SpecificationGame game,
        string sampleName,
        string formKey,
        bool withColor = true,
        bool withMajorRecordFlagsRaw = false,
        bool withAdaptiveTriggerDataComponent = false)
    {
        var spec = ValidationSpecBuilder
            .ForRecord(game, SupportedRecordSpecifications.Keyword)
            .Sample(sampleName)
            .FormKey(formKey)
            .AddBaselineUiComparisonExpectations(
                new[] { "MajorRecordFlags" })
            .AddRule(ValidationFieldRule.DtoDefaultWhenSpriggitAbsent(
                "Version2",
                "Version2",
                "0",
                "Mutagen exposes the default Version2 value when Spriggit omits the zero-valued field."))
            .AddRule(ValidationFieldRule.TranslatedField("Name", "Name", requireAllLanguages: true))
            .AddRule(ValidationFieldRule.IgnoreDtoPrefix("LocalizedStrings", "LocalizedStrings is the DTO projection of translated Spriggit fields."));

        if (game == SpecificationGame.Starfield)
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
