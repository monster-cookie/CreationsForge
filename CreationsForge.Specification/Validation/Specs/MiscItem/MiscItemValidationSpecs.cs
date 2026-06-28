using CreationsForge.Specification.Records;
using CreationsForge.Specification.Validation;

namespace CreationsForge.Specification.Validation.Specs.MiscItem;

public static class MiscItemValidationSpecs
{
    private static readonly IReadOnlyDictionary<string, string> ScriptingAdapterPathReplacements =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [".Object"] = ".ObjectFormKey",
            [".Alias"] = ".ObjectAlias",
            [".Data"] = ".DataInt"
        };

    public static ValidationSpec Fallout4_Debug_Components()
    {
        return Fallout4MiscItem("Debug_Components", "247E7F:Fallout4.esm", withComponents: true, withScriptingAdapters: false);
    }

    public static ValidationSpec Fallout4_FFDiamondCity07Paper()
    {
        return Fallout4MiscItem("FFDiamondCity07Paper", "0A4754:Fallout4.esm", withComponents: false, withScriptingAdapters: true);
    }

    public static ValidationSpec Fallout4_FireExtinguisher01()
    {
        return Fallout4MiscItem(
            "FireExtinguisher01",
            "01F8F9:Fallout4.esm",
            withComponents: true,
            withComponentDisplayIndices: true,
            withDestructible: true,
            withScriptingAdapters: false);
    }

    public static ValidationSpec Fallout4_BobbleHead_Agility()
    {
        return Fallout4MiscItem("BobbleHead_Agility", "178B51:Fallout4.esm", withComponents: false, withScriptingAdapters: true);
    }

    public static ValidationSpec Fallout4_MS11GuidanceChip()
    {
        return Fallout4MiscItem("MS11GuidanceChip", "04E3A2:Fallout4.esm", withComponents: true, withScriptingAdapters: true);
    }

    public static ValidationSpec Skyrim_MGRDragonHeartScales()
    {
        return SkyrimMiscItem(
            "MGRDragonHeartScales",
            "0D0756:Skyrim.esm",
            withAlternateTextures: true,
            withScriptingAdapters: true);
    }

    public static ValidationSpec Skyrim_Firewood01()
    {
        return SkyrimMiscItem("Firewood01", "06F993:Skyrim.esm", withAlternateTextures: false, withScriptingAdapters: false);
    }

    public static ValidationSpec Skyrim_FoxPeltSnow()
    {
        return SkyrimMiscItem("FoxPeltSnow", "0D4BE7:Skyrim.esm", withAlternateTextures: true, withScriptingAdapters: false);
    }

    public static ValidationSpec Skyrim_C04HagravenHead()
    {
        return SkyrimMiscItem("C04HagravenHead", "02996F:Skyrim.esm", withAlternateTextures: false, withScriptingAdapters: true);
    }

    public static ValidationSpec Skyrim_dunUniqueBeeInJar()
    {
        return SkyrimMiscItem("dunUniqueBeeInJar", "0B08C7:Skyrim.esm", withAlternateTextures: false, withScriptingAdapters: true);
    }

    public static ValidationSpec Starfield_InorgCommonWater()
    {
        var spec = StarfieldMiscItem(
            "InorgCommonWater",
            "005591:Starfield.esm",
            withFlag: true,
            withShortName: true,
            materialSwapFormId: "127A9B",
            withResources: true,
            withModelFlags: true);
        spec.UiComparisonExpectations.Add(ValidationUiComparisonExpectation.DtoField(["Value"], "Value"));
        spec.UiComparisonExpectations.Add(new ValidationUiComparisonExpectation(["Model", "File"], visualText: "Model"));
        return spec;
    }

    /// <summary>
    /// Builds the Starfield <c>InorgExoticPlutonium</c> miscellaneous item validation spec, including a UI value row expectation.
    /// </summary>
    /// <returns>The validation spec for the Starfield <c>InorgExoticPlutonium</c> sample.</returns>
    public static ValidationSpec Starfield_InorgExoticPlutonium()
    {
        var spec = StarfieldMiscItem(
            "InorgExoticPlutonium",
            "00558C:Starfield.esm",
            withFlag: true,
            withShortName: true,
            materialSwapFormId: string.Empty,
            withResources: true,
            withModelFlags: false);
        spec.UiComparisonExpectations.Add(ValidationUiComparisonExpectation.DtoField(["Value"], "Value"));
        return spec;
    }

    public static ValidationSpec Starfield_InorgUniqueTasine()
    {
        return StarfieldMiscItem(
            "InorgUniqueTasine",
            "005DED:Starfield.esm",
            withFlag: true,
            withShortName: true,
            materialSwapFormId: "127AA0",
            withResources: true,
            withModelFlags: true);
    }

    public static ValidationSpec Starfield_FFCydoniaZ07_HeartOfMarsTitanium()
    {
        return StarfieldMiscItem(
            "FFCydoniaZ07_HeartOfMarsTitanium",
            "302791:Starfield.esm",
            withFlag: false,
            withShortName: true,
            materialSwapFormId: string.Empty,
            withResources: false,
            withModelFlags: false);
    }

    /// <summary>
    /// Builds the Starfield <c>ExoticPlayingCard_Diamond_Q</c> miscellaneous item validation spec, including a UI model row expectation.
    /// </summary>
    /// <returns>The validation spec for the Starfield <c>ExoticPlayingCard_Diamond_Q</c> sample.</returns>
    public static ValidationSpec Starfield_ExoticPlayingCard_Diamond_Q()
    {
        var spec = StarfieldMiscItem(
            "ExoticPlayingCard_Diamond_Q",
            "10A797:Starfield.esm",
            withFlag: false,
            withShortName: false,
            materialSwapFormId: "103CFB",
            withResources: false,
            withModelFlags: true);
        spec.UiComparisonExpectations.Add(new ValidationUiComparisonExpectation(["Model", "File"], visualText: "EditorID"));
        return spec;
    }

    private static ValidationSpec Fallout4MiscItem(
        string sampleName,
        string formKey,
        bool withComponents,
        bool withScriptingAdapters,
        bool withComponentDisplayIndices = false,
        bool withDestructible = false)
    {
        var spec = BaseMiscItem(SpecificationGame.Fallout4, sampleName, formKey)
            .AddRule(ValidationFieldRule.Field("Model.Data", "Models[0].Data", ValidationValueNormalizer.HexPayload))
            .AddRule(ValidationFieldRule.Field("Model.File", "Models[0].File", ValidationValueNormalizer.ModelFile));

        if (withComponentDisplayIndices)
        {
            spec
                .AddRule(ValidationFieldRule.Field("ComponentDisplayIndices.Count", "Components.Count"))
                .AddRule(ValidationFieldRule.Field("ComponentDisplayIndices[0]", "Components[0].DisplayIndex"))
                .AddRule(ValidationFieldRule.Field("ComponentDisplayIndices[1]", "Components[1].DisplayIndex"))
                .AddRule(ValidationFieldRule.Field("ComponentDisplayIndices[2]", "Components[2].DisplayIndex"));
        }

        if (withDestructible)
        {
            spec
                .AddRule(ValidationFieldRule.ScalarList("Destructible.Stages[0].Flags", "Destructible.Stages[0].Flags"))
                .AddRule(ValidationFieldRule.Field(
                    "Destructible.Stages[0].Model.Data",
                    "Destructible.Stages[0].Model.Data",
                    ValidationValueNormalizer.HexPayload))
                .AddRule(ValidationFieldRule.DtoDefaultWhenSpriggitAbsent(
                    "Destructible.Stages[0].Index",
                    "Destructible.Stages[0].Index",
                    "0",
                    "Mutagen exposes the default destructible stage index when Spriggit omits the zero-valued field."))
                .AddRule(ValidationFieldRule.DtoDefaultWhenSpriggitAbsent(
                    "Destructible.Stages[1].Flags",
                    "Destructible.Stages[1].Flags",
                    "0",
                    "Mutagen exposes default destructible stage flags when Spriggit omits the zero-valued field."))
                .AddRule(ValidationFieldRule.DtoDefaultWhenSpriggitAbsent(
                    "Destructible.Stages[1].HealthPercent",
                    "Destructible.Stages[1].HealthPercent",
                    "0",
                    "Mutagen exposes the default destructible stage health percent when Spriggit omits the zero-valued field."))
                .AddRule(ValidationFieldRule.DtoDefaultWhenSpriggitAbsent(
                    "Destructible.Stages[1].SelfDamagePerSecond",
                    "Destructible.Stages[1].SelfDamagePerSecond",
                    "0",
                    "Mutagen exposes the default destructible stage self damage value when Spriggit omits the zero-valued field."));
        }

        AddOptionalCollectionRules(spec, withComponents, withResources: false, withScriptingAdapters);
        return spec.Build();
    }

    private static ValidationSpec SkyrimMiscItem(
        string sampleName,
        string formKey,
        bool withAlternateTextures,
        bool withScriptingAdapters)
    {
        var spec = BaseMiscItem(SpecificationGame.Skyrim, sampleName, formKey)
            .AddRule(ValidationFieldRule.Field("Model.Data", "Models[0].Data", ValidationValueNormalizer.HexPayload))
            .AddRule(ValidationFieldRule.Field("Model.File", "Models[0].File", ValidationValueNormalizer.ModelFile));

        if (withAlternateTextures)
        {
            spec
                .AddRule(ValidationFieldRule.Field("Model.AlternateTextures[0].Name", "Models[0].MaterialSwaps[0].Name"))
                .AddRule(ValidationFieldRule.Field("Model.AlternateTextures[0].NewTexture", "Models[0].MaterialSwaps[0].MaterialSwapFormKey"));
        }

        AddOptionalCollectionRules(spec, withComponents: false, withResources: false, withScriptingAdapters);
        return spec.Build();
    }

    private static ValidationSpec StarfieldMiscItem(
        string sampleName,
        string formKey,
        bool withFlag,
        bool withShortName,
        string materialSwapFormId,
        bool withResources,
        bool withModelFlags)
    {
        var spec = BaseMiscItem(SpecificationGame.Starfield, sampleName, formKey)
            .AddRule(ValidationFieldRule.DtoDefaultWhenSpriggitAbsent(
                "DirtinessScale",
                "DirtinessScale",
                "0",
                "Mutagen exposes the default DirtinessScale when Spriggit omits the zero-valued field."))
            .AddRule(ValidationFieldRule.Field("Model.File", "Models[0].File", ValidationValueNormalizer.ModelFile))
            .AddRule(ValidationFieldRule.Field("Model.LightLayer", "Models[0].LightLayer"))
            .AddRule(ValidationFieldRule.SoundSlot("CraftingSound.Start", "CraftingSound", "Start"))
            .AddRule(ValidationFieldRule.SoundSlot("PickupSound.Start", "PickupSound", "Start"))
            .AddRule(ValidationFieldRule.SoundSlot("DropdownSound.Start", "DropdownSound", "Start"));

        if (withFlag)
        {
            spec.AddRule(ValidationFieldRule.Field("FLAG", "Flag", ValidationValueNormalizer.HexPayload));
        }

        if (withShortName)
        {
            spec.AddRule(ValidationFieldRule.TranslatedField("ShortName", "ShortName", requireAllLanguages: true));
        }

        if (!string.IsNullOrWhiteSpace(materialSwapFormId))
        {
            spec.AddRule(ValidationFieldRule.FormKeyObjectField(
                "Model.MaterialSwaps[0]." + materialSwapFormId,
                "Models[0].MaterialSwaps[0].MaterialSwapFormKey"));
        }

        if (withModelFlags)
        {
            spec.AddRule(ValidationFieldRule.Field("Model.Flags[0]", "Models[0].Flags"));
        }

        AddOptionalCollectionRules(spec, withComponents: false, withResources, withScriptingAdapters: false);
        return spec.Build();
    }

    private static ValidationSpecBuilder BaseMiscItem(SpecificationGame game, string sampleName, string formKey)
    {
        return ValidationSpecBuilder
            .ForRecord(game, SupportedRecordSpecifications.MiscItem)
            .Sample(sampleName)
            .FormKey(formKey)
            .AddBaselineUiComparisonExpectations(
                new[] { "MajorRecordFlags" },
                new[] { "Value" })
            .AddRule(ValidationFieldRule.DtoDefaultWhenSpriggitAbsent(
                "MajorRecordFlags",
                "MajorRecordFlags",
                "0",
                "Mutagen exposes the default MajorRecordFlags value when Spriggit omits the zero-valued field."))
            .AddRule(ValidationFieldRule.DtoDefaultWhenSpriggitAbsent(
                "Version2",
                "Version2",
                "0",
                "Mutagen exposes the default Version2 value when Spriggit omits the zero-valued field."))
            .AddRule(ValidationFieldRule.DtoDefaultWhenSpriggitAbsent(
                "Value",
                "Value",
                "0",
                "Mutagen exposes the default Value when Spriggit omits the zero-valued field."))
            .AddRule(ValidationFieldRule.DtoDefaultWhenSpriggitAbsent(
                "Weight",
                "Weight",
                "0",
                "Mutagen exposes the default Weight when Spriggit omits the zero-valued field."))
            .AddRule(ValidationFieldRule.TranslatedField("Name", "Name", requireAllLanguages: true))
            .AddRule(ValidationFieldRule.FormKeyList("Keywords", "Keywords", "Keyword"))
            .AddRule(ValidationFieldRule.IgnoreDtoPrefix("LocalizedStrings", "LocalizedStrings is the DTO projection of translated Spriggit fields."));
    }

    private static void AddOptionalCollectionRules(
        ValidationSpecBuilder spec,
        bool withComponents,
        bool withResources,
        bool withScriptingAdapters)
    {
        if (withComponents)
        {
            spec.AddRule(ValidationFieldRule.PathPrefix(
                "Components",
                "Components",
                new Dictionary<string, string>(StringComparer.Ordinal)));
        }

        if (withResources)
        {
            spec.AddRule(ValidationFieldRule.PathPrefix(
                "Resources",
                "Resources",
                new Dictionary<string, string>(StringComparer.Ordinal)));
        }

        if (withScriptingAdapters)
        {
            spec.AddRule(ValidationFieldRule.PathPrefix(
                "VirtualMachineAdapter.Scripts",
                "ScriptingAdapters",
                ScriptingAdapterPathReplacements));
        }
    }
}
