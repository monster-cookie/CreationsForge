using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;

namespace CreationsForge.DataValidationTests.Validation.Specs.ConstructibleObject;

public static class ConstructibleObjectValidationSpecs
{
    private static readonly IReadOnlyDictionary<string, string> NoPathReplacements =
        new Dictionary<string, string>(StringComparer.Ordinal);

    private static readonly IReadOnlyDictionary<string, string> ScriptingAdapterPathReplacements =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [".Objects"] = ".ListItems",
            [".Object"] = ".ObjectFormKey",
            [".Alias"] = ".ObjectAlias",
            [".Data"] = ".DataInt"
        };

    /// <summary>
    /// Builds the Starfield <c>co_Outpost_Power_Reactor01</c> constructible object validation spec,
    /// including UI component and recipe-filter row expectations.
    /// </summary>
    /// <returns>The validation spec for the Starfield <c>co_Outpost_Power_Reactor01</c> sample.</returns>
    public static ValidationSpec Starfield_co_Outpost_Power_Reactor01()
    {
        var spec = StarfieldConstructibleObject("co_Outpost_Power_Reactor01", "007F7C:Starfield.esm").Build();
        spec.UiComparisonExpectations.Add(new ValidationUiComparisonExpectation(
            ["Components", "Component [0]", "Count"],
            visualText: "EditorID"));
        spec.UiComparisonExpectations.Add(new ValidationUiComparisonExpectation(
            ["RecipeFilters", "RecipeFilter [0]", "RecipeFilterFormKey"],
            visualText: null));
        return spec;
    }

    public static ValidationSpec Starfield_co_Outpost_Power_Reactor02()
    {
        return StarfieldConstructibleObject("co_Outpost_Power_Reactor02", "1C5144:Starfield.esm").Build();
    }

    public static ValidationSpec Starfield_co_Chem_XenoAurora()
    {
        return StarfieldConstructibleObject("co_Chem_XenoAurora", "0C8720:Starfield.esm").Build();
    }

    public static ValidationSpec Starfield_UC07_co_mfg_MicroCell_Old()
    {
        return StarfieldConstructibleObject("UC07_co_mfg_MicroCell_Old", "09DE67:Starfield.esm").Build();
    }

    public static ValidationSpec Starfield_co_Outpost_Misc_MissionBoardConsole()
    {
        return StarfieldConstructibleObject("co_Outpost_Misc_MissionBoardConsole", "1DF844:Starfield.esm")
            .AddRule(ValidationFieldRule.Field("MajorRecordFlagsRaw", "MajorRecordFlags"))
            .Build();
    }

    /// <summary>
    /// Builds the Fallout 4 <c>workshop_co_Artillery</c> constructible object validation spec,
    /// including UI component and category row expectations.
    /// </summary>
    /// <returns>The validation spec for the Fallout 4 <c>workshop_co_Artillery</c> sample.</returns>
    public static ValidationSpec Fallout4_workshop_co_Artillery()
    {
        var spec = Fallout4ConstructibleObject("workshop_co_Artillery", "0ADF6E:Fallout4.esm").Build();
        spec.UiComparisonExpectations.Add(new ValidationUiComparisonExpectation(
            ["Components", "Component [0]", "Count"],
            visualText: "EditorID"));
        spec.UiComparisonExpectations.Add(new ValidationUiComparisonExpectation(
            ["Categories", "Category [0]", "CategoryFormKey"],
            visualText: null));
        return spec;
    }

    public static ValidationSpec Fallout4_workshop_co_MQ206BeamEmitter()
    {
        return Fallout4ConstructibleObject("workshop_co_MQ206BeamEmitter", "0CEA6F:Fallout4.esm").Build();
    }

    public static ValidationSpec Fallout4_workshop_co_MQ206Console()
    {
        return Fallout4ConstructibleObject("workshop_co_MQ206Console", "0CEA7B:Fallout4.esm").Build();
    }

    public static ValidationSpec Fallout4_workshop_co_WaterPurifier()
    {
        return Fallout4ConstructibleObject("workshop_co_WaterPurifier", "05A0CD:Fallout4.esm").Build();
    }

    public static ValidationSpec Fallout4_co_mod_GatlingLaser_BarrelMingunLaser_Super()
    {
        return Fallout4ConstructibleObject("co_mod_GatlingLaser_BarrelMingunLaser_Super", "1889E3:Fallout4.esm").Build();
    }

    /// <summary>
    /// Builds the Skyrim <c>RecipeArmorDragonscaleBoots</c> constructible object validation spec,
    /// including a UI component row expectation.
    /// </summary>
    /// <returns>The validation spec for the Skyrim <c>RecipeArmorDragonscaleBoots</c> sample.</returns>
    public static ValidationSpec Skyrim_RecipeArmorDragonscaleBoots()
    {
        var spec = SkyrimConstructibleObject("RecipeArmorDragonscaleBoots", "0DCA13:Skyrim.esm").Build();
        spec.UiComparisonExpectations.Add(new ValidationUiComparisonExpectation(
            ["Components", "Component [0]", "Count"],
            visualText: "EditorID"));
        return spec;
    }

    public static ValidationSpec Skyrim_RecipeArmorDragonscaleCuirass()
    {
        return SkyrimConstructibleObject("RecipeArmorDragonscaleCuirass", "0DCA14:Skyrim.esm").Build();
    }

    public static ValidationSpec Skyrim_RecipeArmorDragonscaleGauntlets()
    {
        return SkyrimConstructibleObject("RecipeArmorDragonscaleGauntlets", "0DCA15:Skyrim.esm").Build();
    }

    public static ValidationSpec Skyrim_RecipeArmorSteelPlateShield()
    {
        return SkyrimConstructibleObject("RecipeArmorSteelPlateShield", "0DD982:Skyrim.esm").Build();
    }

    public static ValidationSpec Skyrim_RecipeFoodSoupCabbagePotato()
    {
        return SkyrimConstructibleObject("RecipeFoodSoupCabbagePotato", "0F431A:Skyrim.esm").Build();
    }

    private static ValidationSpecBuilder StarfieldConstructibleObject(string sampleName, string formKey)
    {
        return BaseConstructibleObject(SupportedGame.Starfield, sampleName, formKey)
            .AddRule(ValidationFieldRule.OptionalField("AmountProduced", "AmountProduced"))
            .AddRule(ValidationFieldRule.DtoDefaultWhenSpriggitAbsent(
                "AmountProduced",
                "AmountProduced",
                "0",
                "Mutagen exposes default AmountProduced when Spriggit omits it."))
            .AddRule(ValidationFieldRule.OptionalField("Value", "Value"))
            .AddRule(ValidationFieldRule.DtoDefaultWhenSpriggitAbsent(
                "Value",
                "Value",
                "0",
                "Mutagen exposes default Value when Spriggit omits it."))
            .AddRule(ValidationFieldRule.OptionalField("MenuSortOrder", "MenuSortOrder", ValidationValueNormalizer.FloatNumber))
            .AddRule(ValidationFieldRule.Field("LearnMethod", "LearnMethod"))
            .AddRule(ValidationFieldRule.Field("Flags", "Flags", ValidationValueNormalizer.HexPayload))
            .AddRule(ValidationFieldRule.ScalarList("StarfieldMajorRecordFlags", "MajorRecordFlags", ValidationValueNormalizer.HexInteger))
            .AddRule(ValidationFieldRule.ScalarList("MajorFlags", "MajorFlags", ValidationValueNormalizer.HexInteger))
            .AddRules(GetStarfieldComponentRules())
            .AddRule(ValidationFieldRule.IgnoreDto(
                "MenuSortOrder",
                "Mutagen exposes default MenuSortOrder when Spriggit omits it."))
            .AddRule(ValidationFieldRule.IgnoreDtoPrefix(
                "RecipeFilters",
                "Recipe filter index values are DTO metadata for persisted child row ordering."))
            .AddRule(ValidationFieldRule.SoundSlot("DropdownSound.Start", "DropdownSound", "Start"))
            .AddRule(ValidationFieldRule.SoundSlot("PickupSound.Start", "PickupSound", "Start"));
    }

    private static ValidationSpecBuilder Fallout4ConstructibleObject(string sampleName, string formKey)
    {
        return BaseConstructibleObject(SupportedGame.Fallout4, sampleName, formKey)
            .AddRules(GetFallout4ComponentRules())
            .AddRule(ValidationFieldRule.DtoNonEmpty("CreatedObjectCounts", "CreatedObjectCount"))
            .AddRule(ValidationFieldRule.OptionalField("Value", "Value"))
            .AddRule(ValidationFieldRule.ScalarList("Fallout4MajorRecordFlags", "MajorRecordFlags", ValidationValueNormalizer.HexInteger))
            .AddRule(ValidationFieldRule.ScalarList("MajorFlags", "MajorFlags"))
            .AddRule(ValidationFieldRule.SoundSlot("PickUpSound", "PickUpSound", "Start"))
            .AddRule(ValidationFieldRule.SoundSlot("PutDownSound", "PutDownSound", "Start"))
            .AddRule(ValidationFieldRule.IgnoreDtoPrefix(
                "Categories",
                "Category index values are DTO metadata for persisted child row ordering."))
            .AddRule(ValidationFieldRule.IgnoreSpriggit(
                "CreatedObjectCounts.Count",
                "The COBJ DTO stores the first CreatedObjectCounts value as the scalar CreatedObjectCount."));
    }

    private static ValidationSpecBuilder SkyrimConstructibleObject(string sampleName, string formKey)
    {
        return BaseConstructibleObject(SupportedGame.Skyrim, sampleName, formKey)
            .AddRules(GetSkyrimComponentRules())
            .AddRule(ValidationFieldRule.OptionalField("Value", "Value"))
            .AddRule(ValidationFieldRule.ScalarList("SkyrimMajorRecordFlags", "MajorRecordFlags", ValidationValueNormalizer.HexInteger))
            .AddRule(ValidationFieldRule.ScalarList("MajorFlags", "MajorFlags"))
            .AddRule(ValidationFieldRule.Field("CreatedObjectCount", "CreatedObjectCount"));
    }

    private static ValidationSpecBuilder BaseConstructibleObject(SupportedGame game, string sampleName, string formKey)
    {
        return ValidationSpecBuilder
            .ForRecord(game, RecordTypeCatalog.ConstructibleObject)
            .Sample(sampleName)
            .FormKey(formKey)
            .AddBaselineUiComparisonExpectations()
            .AddRule(ValidationFieldRule.TranslatedField("Description", "Description"))
            .AddRule(ValidationFieldRule.Field("CreatedObject", "CreatedObjectFormKey"))
            .AddRule(ValidationFieldRule.Field("WorkbenchKeyword", "WorkbenchKeywordFormKey"))
            .AddRule(ValidationFieldRule.PathPrefix("VirtualMachineAdapter.Scripts", "ScriptingAdapters", ScriptingAdapterPathReplacements))
            .AddRule(ValidationFieldRule.PathPrefix("Conditions", "Conditions", NoPathReplacements))
            .AddRule(ValidationFieldRule.FormKeyList("Categories", "Categories", "CategoryFormKey"))
            .AddRule(ValidationFieldRule.FormKeyList("RecipeFilters", "RecipeFilters", "RecipeFilterFormKey"))
            .AddRule(ValidationFieldRule.IgnoreDtoPrefix("LocalizedStrings", "LocalizedStrings is the DTO projection of translated Spriggit fields."));
    }

    private static IEnumerable<ValidationFieldRule> GetStarfieldComponentRules()
    {
        for (var componentIndex = 0; componentIndex <= 40; componentIndex++)
        {
            var indexText = componentIndex.ToString(System.Globalization.CultureInfo.InvariantCulture);
            yield return ValidationFieldRule.Field("ConstructableComponents[" + indexText + "].Component", "Components[" + indexText + "].ComponentFormKey");
            yield return ValidationFieldRule.Field("ConstructableComponents[" + indexText + "].RequiredCount", "Components[" + indexText + "].Count");
        }
    }

    private static IEnumerable<ValidationFieldRule> GetFallout4ComponentRules()
    {
        for (var componentIndex = 0; componentIndex <= 40; componentIndex++)
        {
            var indexText = componentIndex.ToString(System.Globalization.CultureInfo.InvariantCulture);
            yield return ValidationFieldRule.Field("Components[" + indexText + "].Component", "Components[" + indexText + "].ComponentFormKey");
            yield return ValidationFieldRule.Field("Components[" + indexText + "].Count", "Components[" + indexText + "].Count");
        }
    }

    private static IEnumerable<ValidationFieldRule> GetSkyrimComponentRules()
    {
        for (var componentIndex = 0; componentIndex <= 40; componentIndex++)
        {
            var indexText = componentIndex.ToString(System.Globalization.CultureInfo.InvariantCulture);
            yield return ValidationFieldRule.Field("Items[" + indexText + "].Item.Item", "Components[" + indexText + "].ComponentFormKey");
            yield return ValidationFieldRule.Field("Items[" + indexText + "].Item.Count", "Components[" + indexText + "].Count");
        }
    }
}
