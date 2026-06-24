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

    public static ValidationSpec Starfield_ShipOutpost_Loot_Storage_Safe_Floor_Reg_Common()
    {
        return StarfieldContainer("ShipOutpost_Loot_Storage_Safe_Floor_Reg_Common", "277A73:Starfield.esm")
            .AddRules(GetAnimationGraphComponentRules(0))
            .AddRule(ValidationFieldRule.RawPayloadSlot("Components[1].REFL", "Components"))
            .AddRule(ValidationFieldRule.IgnoreSpriggit("Components[1].MutagenObjectType", "Component type is represented by raw payload rows."))
            .AddRules(GetOutpostContainerPropertyRules())
            .Build();
    }

    public static ValidationSpec Starfield_ShipOutpost_Loot_Storage_Safe_Floor_Tall_Rare()
    {
        return StarfieldContainer("ShipOutpost_Loot_Storage_Safe_Floor_Tall_Rare", "277A81:Starfield.esm")
            .AddRules(GetAnimationGraphComponentRules(0))
            .AddRule(ValidationFieldRule.RawPayloadSlot("Components[1].REFL", "Components"))
            .AddRule(ValidationFieldRule.IgnoreSpriggit("Components[1].MutagenObjectType", "Component type is represented by raw payload rows."))
            .AddRules(GetOutpostContainerPropertyRules())
            .Build();
    }

    public static ValidationSpec Starfield_ShipOutpost_Loot_Storage_BossChest_Industrial_Rare()
    {
        return StarfieldContainer("ShipOutpost_Loot_Storage_BossChest_Industrial_Rare", "2779E9:Starfield.esm")
            .AddRules(GetAnimationGraphComponentRules(0))
            .AddRule(ValidationFieldRule.RawPayloadSlot("Components[1].REFL", "Components"))
            .AddRule(ValidationFieldRule.IgnoreSpriggit("Components[1].MutagenObjectType", "Component type is represented by raw payload rows."))
            .AddRules(GetOutpostContainerPropertyRules())
            .AddRule(ValidationFieldRule.IgnoreSpriggit("ForcedLocations.Count", "Container forced locations are not persisted by the current container DTO."))
            .AddRule(ValidationFieldRule.IgnoreSpriggit("ForcedLocations[0]", "Container forced locations are not persisted by the current container DTO."))
            .AddRule(ValidationFieldRule.IgnoreSpriggit("ForcedLocations[0].05AF1F", "Container forced locations are not persisted by the current container DTO."))
            .Build();
    }

    public static ValidationSpec Starfield_Loot_Display_WeaponRack03_EMPTY()
    {
        return StarfieldContainer("Loot_Display_WeaponRack03_EMPTY", "1A23DF:Starfield.esm")
            .AddRules(GetAnimationGraphComponentRules(0))
            .AddRule(ValidationFieldRule.IgnoreSpriggit("Components[1].MutagenObjectType", "Display case component rows are not persisted by the current container DTO."))
            .AddRule(ValidationFieldRule.IgnoreSpriggit("Components[1].Items.Count", "Display case component rows are not persisted by the current container DTO."))
            .AddRule(ValidationFieldRule.IgnoreSpriggit("Components[1].DCED.Count", "Display case component rows are not persisted by the current container DTO."))
            .AddRule(ValidationFieldRule.IgnoreSpriggit("SnapTemplate", "Container snap template is not persisted by the current container DTO."))
            .AddRule(ValidationFieldRule.IgnoreSpriggit("ContainsOnlyFilter", "Container display filter is not persisted by the current container DTO."))
            .AddRules(GetDisplayCaseComponentRules(1))
            .Build();
    }

    public static ValidationSpec Starfield_Loot_Display_ArboronWeaponRackPanel02()
    {
        return StarfieldContainer("Loot_Display_ArboronWeaponRackPanel02", "057C20:Starfield.esm")
            .AddRule(ValidationFieldRule.IgnoreSpriggit("Components.Count", "Component count is covered by component-specific validation rules."))
            .AddRule(ValidationFieldRule.IgnoreSpriggit("Components[0].MutagenObjectType", "Display case component rows are not persisted by the current container DTO."))
            .AddRule(ValidationFieldRule.IgnoreSpriggit("Components[0].Items.Count", "Display case component rows are not persisted by the current container DTO."))
            .AddRule(ValidationFieldRule.IgnoreSpriggit("Components[0].DCED.Count", "Display case component rows are not persisted by the current container DTO."))
            .AddRule(ValidationFieldRule.IgnoreSpriggit("SnapTemplate", "Container snap template is not persisted by the current container DTO."))
            .AddRule(ValidationFieldRule.IgnoreSpriggit("ContainsOnlyFilter", "Container display filter is not persisted by the current container DTO."))
            .AddRules(GetDisplayCaseComponentRules(0))
            .Build();
    }

    public static ValidationSpec Fallout4_DN054Loot_Prewar_Safe()
    {
        return Fallout4Container("DN054Loot_Prewar_Safe", "1F2B6A:Fallout4.esm", withScriptingAdapters: false)
            .Build();
    }

    public static ValidationSpec Fallout4_Loot_Raider_Safe()
    {
        return Fallout4Container("Loot_Raider_Safe", "064A36:Fallout4.esm", withScriptingAdapters: true)
            .Build();
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

    public static ValidationSpec Skyrim_BeeHive()
    {
        return SkyrimContainer("BeeHive", "0A918C:Skyrim.esm")
            .AddRule(ValidationFieldRule.Field("VirtualMachineAdapter.Scripts.Count", "ScriptingAdapters.Count"))
            .AddRule(ValidationFieldRule.Field("VirtualMachineAdapter.Scripts[0].Name", "ScriptingAdapters[1].Name"))
            .AddRule(ValidationFieldRule.Field("VirtualMachineAdapter.Scripts[1].Name", "ScriptingAdapters[0].Name"))
            .AddRule(ValidationFieldRule.PathPrefix("VirtualMachineAdapter.Scripts[1].Properties", "ScriptingAdapters[0].Properties", ScriptingAdapterPathReplacements))
            .Build();
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
    }

    private static IEnumerable<ValidationFieldRule> GetOutpostContainerPropertyRules()
    {
        yield return ValidationFieldRule.IgnoreSpriggit("Transforms.Outpost", "Container transform references are not persisted by the current container DTO.");
        yield return ValidationFieldRule.IgnoreSpriggit("Transforms.Preview", "Container transform references are not persisted by the current container DTO.");
        yield return ValidationFieldRule.IgnoreSpriggit("Properties.Count", "Container property payloads are not persisted by the current container DTO.");
        for (var propertyIndex = 0; propertyIndex <= 3; propertyIndex++)
        {
            var propertyText = propertyIndex.ToString(System.Globalization.CultureInfo.InvariantCulture);
            yield return ValidationFieldRule.IgnoreSpriggit("Properties[" + propertyText + "].ActorValue", "Container property payloads are not persisted by the current container DTO.");
            yield return ValidationFieldRule.IgnoreSpriggit("Properties[" + propertyText + "].Value", "Container property payloads are not persisted by the current container DTO.");
        }
    }

    private static IEnumerable<ValidationFieldRule> GetDisplayCaseComponentRules(int componentIndex)
    {
        var componentText = componentIndex.ToString(System.Globalization.CultureInfo.InvariantCulture);
        for (var itemIndex = 0; itemIndex <= 12; itemIndex++)
        {
            var indexText = itemIndex.ToString(System.Globalization.CultureInfo.InvariantCulture);
            yield return ValidationFieldRule.IgnoreSpriggit("Components[" + componentText + "].Items[" + indexText + "].DisplayFilter", "Display case component rows are not persisted by the current container DTO.");
            yield return ValidationFieldRule.IgnoreSpriggit("Components[" + componentText + "].Items[" + indexText + "].Index", "Display case component rows are not persisted by the current container DTO.");
            yield return ValidationFieldRule.IgnoreSpriggit("Components[" + componentText + "].Items[" + indexText + "].Unknown2", "Display case component rows are not persisted by the current container DTO.");
            yield return ValidationFieldRule.IgnoreSpriggit("Components[" + componentText + "].DCED[" + indexText + "]", "Display case component rows are not persisted by the current container DTO.");
        }
    }
}
