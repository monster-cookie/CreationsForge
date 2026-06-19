namespace CreationsForge.DataValidationTests.Validation.Services;

public class SpriggitFieldPathMapper
{
    private static readonly IReadOnlyDictionary<string, string> ExactPathAliases = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["ObjectBounds.First"] = "ObjectBoundsFirst",
        ["ObjectBounds.Second"] = "ObjectBoundsSecond",
        ["Model.File"] = "Models[0].File",
        ["Model.LightLayer"] = "Models[0].LightLayer",
        ["Model.Flags"] = "Models[0].Flags",
        ["Model.ColorRemappingIndex"] = "Models[0].ColorRemappingIndex",
        ["Model.FlagsVestigial"] = "Models[0].FlagsVestigial",
        ["Keywords.Count"] = "Keywords.Count",
        ["Items.Count"] = "Items.Count",
        ["Ranks.Count"] = "Ranks.Count",
        ["Conditions.Count"] = "Conditions.Count",
        ["MarkerParameters.Count"] = "MarkerParameters.Count",
        ["Properties.Count"] = "Properties.Count",
        ["Weights.Count"] = "Weights.Count",
        ["Relations.Count"] = "Relations.Count",
        ["Categories.Count"] = "Categories.Count",
        ["RecipeFilters.Count"] = "RecipeFilters.Count",
        ["BackgroundSkills.Count"] = "BackgroundSkills.Count",
        ["Components.Count"] = "Components.Count",
        ["ConstructableComponents.Count"] = "Components.Count",
        ["AddToList"] = "AddToListFormKey",
        ["CreatedObject"] = "CreatedObjectFormKey",
        ["WorkbenchKeyword"] = "WorkbenchKeywordFormKey",
        ["NativeTerminal"] = "NativeTerminalFormKey",
        ["Menu"] = "MenuFormKey",
        ["FurnitureTemplate"] = "FurnitureTemplateFormKey",
        ["Restriction"] = "RestrictionFormKey",
        ["Training"] = "TrainingFormKey",
        ["Race"] = "RaceFormKey",
        ["Voice"] = "VoiceFormKey",
        ["CombatStyle"] = "CombatStyleFormKey",
        ["DefaultPackageList"] = "DefaultPackageListFormKey",
        ["CrimeFaction"] = "CrimeFactionFormKey",
        ["Herd"] = "HerdFormKey",
        ["VoiceType"] = "VoiceTypeFormKey",
        ["VendorBuySellList"] = "VendorBuySellListFormKey",
        ["MerchantContainer"] = "MerchantContainerFormKey",
        ["ActorValue2"] = "ActorValue2FormKey",
        ["ResistValue"] = "ResistValueFormKey",
        ["PerkToApply"] = "PerkToApplyFormKey",
        ["EquipAbility"] = "EquipAbilityFormKey",
        ["FeaturedItemMessage"] = "FeaturedItemMessageFormKey"
    };

    public string Map(string spriggitPath)
    {
        if (ExactPathAliases.TryGetValue(spriggitPath, out var alias))
        {
            return alias;
        }

        return spriggitPath;
    }
}
