using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;

namespace CreationsForge.DataValidationTests.Validation.Specs.Container;

public static class ContainerValidationSpecs
{
    private static readonly IReadOnlyDictionary<string, string> ScriptingAdapterPathReplacements =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [".Objects"] = ".ListItems",
            [".Object"] = ".ObjectFormKey",
            [".Alias"] = ".ObjectAlias",
            [".Data"] = ".DataInt"
        };

    /// <summary>
    /// Builds the Starfield <c>ShipOutpost_Loot_Storage_Safe_Floor_Reg_Common</c> container validation spec,
    /// including UI model and reflection row expectations.
    /// </summary>
    /// <returns>The validation spec for the Starfield <c>ShipOutpost_Loot_Storage_Safe_Floor_Reg_Common</c> sample.</returns>
    public static ValidationSpec Starfield_ShipOutpost_Loot_Storage_Safe_Floor_Reg_Common()
    {
        var spec = StarfieldContainer("ShipOutpost_Loot_Storage_Safe_Floor_Reg_Common", "277A73:Starfield.esm")
            .AddRules(GetAnimationGraphComponentRules(0))
            .AddRules(ValidationFieldRule.ComponentReflection(1, 0, 1, "EffectSequenceComponent"))
            .Build();
        spec.UiComparisonExpectations.Add(new ValidationUiComparisonExpectation(["Model", "File"], visualText: "EditorID"));
        spec.UiComparisonExpectations.Add(new ValidationUiComparisonExpectation(
            ["Reflection", "Components[1].REFL", "SourcePath"]));
        return spec;
    }

    public static ValidationSpec Starfield_ShipOutpost_Loot_Storage_Safe_Floor_Tall_Rare()
    {
        return StarfieldContainer("ShipOutpost_Loot_Storage_Safe_Floor_Tall_Rare", "277A81:Starfield.esm")
            .AddRules(GetAnimationGraphComponentRules(0))
            .AddRules(ValidationFieldRule.ComponentReflection(1, 0, 1, "EffectSequenceComponent"))
            .Build();
    }

    public static ValidationSpec Starfield_ShipOutpost_Loot_Storage_BossChest_Industrial_Rare()
    {
        return StarfieldContainer("ShipOutpost_Loot_Storage_BossChest_Industrial_Rare", "2779E9:Starfield.esm")
            .AddRules(GetAnimationGraphComponentRules(0))
            .AddRules(ValidationFieldRule.ComponentReflection(1, 0, 1, "EffectSequenceComponent"))
            .Build();
    }

    public static ValidationSpec Starfield_Loot_Display_WeaponRack03_EMPTY()
    {
        return StarfieldContainer("Loot_Display_WeaponRack03_EMPTY", "1A23DF:Starfield.esm")
            .AddRules(GetAnimationGraphComponentRules(0))
            .Build();
    }

    public static ValidationSpec Starfield_Loot_Display_ArboronWeaponRackPanel02()
    {
        return StarfieldContainer("Loot_Display_ArboronWeaponRackPanel02", "057C20:Starfield.esm")
            .Build();
    }

    public static ValidationSpec Fallout4_DN054Loot_Prewar_Safe()
    {
        return Fallout4Container("DN054Loot_Prewar_Safe", "1F2B6A:Fallout4.esm", withScriptingAdapters: false)
            .Build();
    }

    /// <summary>
    /// Builds the Fallout 4 <c>Loot_Raider_Safe</c> container validation spec, including a UI model row expectation.
    /// </summary>
    /// <returns>The validation spec for the Fallout 4 <c>Loot_Raider_Safe</c> sample.</returns>
    public static ValidationSpec Fallout4_Loot_Raider_Safe()
    {
        var spec = Fallout4Container("Loot_Raider_Safe", "064A36:Fallout4.esm", withScriptingAdapters: true)
            .Build();
        spec.UiComparisonExpectations.Add(new ValidationUiComparisonExpectation(["Model", "File"], visualText: "EditorID"));
        return spec;
    }

    public static ValidationSpec Fallout4_TheaterTickerTape_Safe()
    {
        return Fallout4Container("TheaterTickerTape_Safe", "1C0292:Fallout4.esm", withScriptingAdapters: false)
            .Build();
    }

    public static ValidationSpec Fallout4_Loot_Trunk_Boss()
    {
        return Fallout4Container("Loot_Trunk_Boss", "06355F:Fallout4.esm", withScriptingAdapters: false)
            .AddRule(ValidationFieldRule.SoundSlot("TakeAllSound", "TakeAllSound", "Start"))
            .Build();
    }

    public static ValidationSpec Fallout4_DN123_SkylanesSecretCompartment()
    {
        return Fallout4Container("DN123_SkylanesSecretCompartment", "11CB14:Fallout4.esm", withScriptingAdapters: true)
            .AddRule(ValidationFieldRule.Field("MajorRecordFlagsRaw", "MajorRecordFlags"))
            .Build();
    }

    public static ValidationSpec Skyrim_TreasFalmerChestBoss()
    {
        return SkyrimContainer("TreasFalmerChestBoss", "02065B:Skyrim.esm")
            .Build();
    }

    public static ValidationSpec Skyrim_TreasFalmerChestBossDwarven()
    {
        return SkyrimContainer("TreasFalmerChestBossDwarven", "0B1176:Skyrim.esm")
            .Build();
    }

    public static ValidationSpec Skyrim_TreasFalmerChest()
    {
        return SkyrimContainer("TreasFalmerChest", "020659:Skyrim.esm")
            .Build();
    }

    /// <summary>
    /// Builds the Skyrim <c>BeeHive</c> container validation spec, including UI model and script row expectations.
    /// </summary>
    /// <returns>The validation spec for the Skyrim <c>BeeHive</c> sample.</returns>
    public static ValidationSpec Skyrim_BeeHive()
    {
        var spec = SkyrimContainer("BeeHive", "0A918C:Skyrim.esm")
            .AddRule(ValidationFieldRule.Field("VirtualMachineAdapter.Scripts.Count", "ScriptingAdapters.Count"))
            .AddRule(ValidationFieldRule.Field("VirtualMachineAdapter.Scripts[0].Name", "ScriptingAdapters[1].Name"))
            .AddRule(ValidationFieldRule.Field("VirtualMachineAdapter.Scripts[1].Name", "ScriptingAdapters[0].Name"))
            .AddRule(ValidationFieldRule.PathPrefix("VirtualMachineAdapter.Scripts[1].Properties", "ScriptingAdapters[0].Properties", ScriptingAdapterPathReplacements))
            .Build();
        spec.UiComparisonExpectations.Add(new ValidationUiComparisonExpectation(["Model", "File"], visualText: "EditorID"));
        spec.UiComparisonExpectations.Add(new ValidationUiComparisonExpectation(["Scripts", "Script [0]", "Name"]));
        return spec;
    }

    public static ValidationSpec Skyrim_MerchantCaravanAChest()
    {
        return SkyrimContainer("MerchantCaravanAChest", "07434B:Skyrim.esm")
            .Build();
    }

    private static ValidationSpecBuilder StarfieldContainer(string sampleName, string formKey)
    {
        return BaseContainer(SupportedGame.Starfield, sampleName, formKey)
            .AddRule(ValidationFieldRule.ScalarList("StarfieldMajorRecordFlags", "MajorRecordFlags", ValidationValueNormalizer.HexInteger))
            .AddRule(ValidationFieldRule.ScalarList("MajorFlags", "MajorFlags"))
            .AddRule(ValidationFieldRule.Field("Model.File", "Models[0].File", ValidationValueNormalizer.ModelFile))
            .AddRule(ValidationFieldRule.Field("Model.LightLayer", "Models[0].LightLayer"))
            .AddRule(ValidationFieldRule.FormKeyList("ForcedLocations", "ForcedLocations", string.Empty))
            .AddRule(ValidationFieldRule.SoundSlot("OpenSound.Start", "OpenSound", "Start"))
            .AddRule(ValidationFieldRule.SoundSlot("CloseSound.Start", "CloseSound", "Start"));
    }

    private static ValidationSpecBuilder Fallout4Container(
        string sampleName,
        string formKey,
        bool withScriptingAdapters)
    {
        var spec = BaseContainer(SupportedGame.Fallout4, sampleName, formKey)
            .AddRule(ValidationFieldRule.ScalarList("Fallout4MajorRecordFlags", "MajorRecordFlags", ValidationValueNormalizer.HexInteger))
            .AddRule(ValidationFieldRule.Field("Model.Data", "Models[0].Data", ValidationValueNormalizer.HexPayload))
            .AddRule(ValidationFieldRule.Field("Model.File", "Models[0].File", ValidationValueNormalizer.ModelFile))
            .AddRule(ValidationFieldRule.SoundSlot("OpenSound", "OpenSound", "Start"))
            .AddRule(ValidationFieldRule.SoundSlot("CloseSound", "CloseSound", "Start"));

        if (withScriptingAdapters)
        {
            spec.AddRule(ValidationFieldRule.PathPrefix("VirtualMachineAdapter.Scripts", "ScriptingAdapters", ScriptingAdapterPathReplacements));
        }
        else
        {
            spec.AddRule(ValidationFieldRule.SpriggitAbsent("VirtualMachineAdapter.Scripts.Count"));
        }

        return spec;
    }

    private static ValidationSpecBuilder SkyrimContainer(string sampleName, string formKey)
    {
        return BaseContainer(SupportedGame.Skyrim, sampleName, formKey)
            .AddRule(ValidationFieldRule.ScalarList("SkyrimMajorRecordFlags", "MajorRecordFlags", ValidationValueNormalizer.HexInteger))
            .AddRule(ValidationFieldRule.Field("MajorRecordFlagsRaw", "MajorRecordFlags"))
            .AddRule(ValidationFieldRule.Field("Model.Data", "Models[0].Data", ValidationValueNormalizer.HexPayload))
            .AddRule(ValidationFieldRule.Field("Model.File", "Models[0].File", ValidationValueNormalizer.ModelFile))
            .AddRule(ValidationFieldRule.SoundSlot("OpenSound", "OpenSound", "Start"))
            .AddRule(ValidationFieldRule.SoundSlot("CloseSound", "CloseSound", "Start"));
    }

    private static ValidationSpecBuilder BaseContainer(SupportedGame game, string sampleName, string formKey)
    {
        var spec = ValidationSpecBuilder
            .ForRecord(game, RecordTypeCatalog.Container)
            .Sample(sampleName)
            .FormKey(formKey)
            .AddRule(ValidationFieldRule.TranslatedField("Name", "Name", requireAllLanguages: true))
            .AddRule(ValidationFieldRule.Field("ObjectBounds.First", "ObjectBoundsFirst"))
            .AddRule(ValidationFieldRule.Field("ObjectBounds.Second", "ObjectBoundsSecond"))
            .AddRule(ValidationFieldRule.Field("NativeTerminal", "NativeTerminalFormKey"))
            .AddRule(ValidationFieldRule.ScalarList("Flags", "Flags"))
            .AddRule(ValidationFieldRule.ScalarList("MajorFlags", "MajorFlags"))
            .AddRule(ValidationFieldRule.FormKeyList("Keywords", "Keywords", "Keyword"))
            .AddRule(ValidationFieldRule.IgnoreSpriggit("Items.Count", "Spriggit wrapper count is validated through Item[n] and Count[n] child fields."))
            .AddRule(ValidationFieldRule.IgnoreDtoPrefix("LocalizedStrings", "LocalizedStrings is the DTO projection of translated Spriggit fields."));
        AddItemRules(spec);
        return spec;
    }

    private static void AddItemRules(ValidationSpecBuilder spec)
    {
        for (var itemIndex = 0; itemIndex <= 25; itemIndex++)
        {
            var indexText = itemIndex.ToString(System.Globalization.CultureInfo.InvariantCulture);
            spec
                .AddRule(ValidationFieldRule.Field("Items[" + indexText + "].Item.Item", "Items[" + indexText + "].ItemFormKey"))
                .AddRule(ValidationFieldRule.Field("Items[" + indexText + "].Item.Count", "Items[" + indexText + "].Count"));
        }
    }

    private static IEnumerable<ValidationFieldRule> GetAnimationGraphComponentRules(int componentIndex)
    {
        var componentText = componentIndex.ToString(System.Globalization.CultureInfo.InvariantCulture);
        yield return ValidationFieldRule.DtoNonEmpty("Components[" + componentText + "].ANAM", "AnimationGraph");
        yield return ValidationFieldRule.DtoNonEmpty("Components[" + componentText + "].BNAM", "AnimationSkeleton");
        yield return ValidationFieldRule.DtoNonEmpty("Components[" + componentText + "].CNAM", "AnimationDirectory");
        yield return ValidationFieldRule.IgnoreSpriggit("Components[" + componentText + "].MutagenObjectType", "AnimationGraphComponent values are projected into direct animation fields.");
        yield return ValidationFieldRule.IgnoreDto("Components[" + componentText + "].MutagenObjectType", "AnimationGraphComponent values are projected into direct animation fields.");
    }

}
