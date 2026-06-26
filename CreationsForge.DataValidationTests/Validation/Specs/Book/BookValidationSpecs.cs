using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;

namespace CreationsForge.DataValidationTests.Validation.Specs.Book;

public static class BookValidationSpecs
{
    private static readonly IReadOnlyDictionary<string, string> ScriptingAdapterPathReplacements =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [".Objects"] = ".ListItems",
            [".Object"] = ".ObjectFormKey",
            [".Data"] = ".DataInt"
        };

    public static ValidationSpec Starfield_NH_SouvenirSlate()
    {
        var spec = StarfieldBook("NH_SouvenirSlate", "165BF3:Starfield.esm", withHeaderFields: true);
        spec.UiComparisonExpectations.Add(ValidationUiComparisonExpectation.DtoField(["Value"], "Value"));
        return spec;
    }

    public static ValidationSpec Starfield_UC07_ScrappingNiira()
    {
        return StarfieldBook("UC07_ScrappingNiira", "1F40EE:Starfield.esm", withHeaderFields: false);
    }

    public static ValidationSpec Starfield_SQ_PlanetSurveySlate00_025()
    {
        return StarfieldBook("SQ_PlanetSurveySlate00_025", "26E6B1:Starfield.esm", withHeaderFields: false);
    }

    /// <summary>
    /// Builds the Starfield <c>_RENAME_TestDataslate</c> book validation spec, including a UI header row expectation.
    /// </summary>
    /// <returns>The validation spec for the Starfield <c>_RENAME_TestDataslate</c> sample.</returns>
    public static ValidationSpec Starfield_RENAME_TestDataslate()
    {
        var spec = StarfieldBook("_RENAME_TestDataslate", "070510:Starfield.esm", withHeaderFields: true);
        spec.UiComparisonExpectations.Add(new ValidationUiComparisonExpectation(["DataSlateHeaderLeft"], visualText: "Name"));
        return spec;
    }

    /// <summary>
    /// Builds the Starfield <c>TreasureMap_Resource_AnySystem_Unique_Aldumite</c> book validation spec, including a UI model row expectation.
    /// </summary>
    /// <returns>The validation spec for the Starfield <c>TreasureMap_Resource_AnySystem_Unique_Aldumite</c> sample.</returns>
    public static ValidationSpec Starfield_TreasureMap_Resource_AnySystem_Unique_Aldumite()
    {
        var spec = StarfieldBook(
            "TreasureMap_Resource_AnySystem_Unique_Aldumite",
            "045631:Starfield.esm",
            withHeaderFields: false);
        spec.UiComparisonExpectations.Add(new ValidationUiComparisonExpectation(["Model", "File"], visualText: "EditorID"));
        return spec;
    }

    public static ValidationSpec Fallout4_BoS301ActuatorList()
    {
        return Fallout4Book("BoS301ActuatorList", "02B4DF:Fallout4.esm", withScriptingAdapters: false);
    }

    public static ValidationSpec Fallout4_DN054PowerArmorPaintJobPurchaseItem()
    {
        return Fallout4Book("DN054PowerArmorPaintJobPurchaseItem", "23C675:Fallout4.esm", withScriptingAdapters: true);
    }

    /// <summary>
    /// Builds the Fallout 4 <c>PerkMagGunsAndBullets07</c> book validation spec, including UI model and script row expectations.
    /// </summary>
    /// <returns>The validation spec for the Fallout 4 <c>PerkMagGunsAndBullets07</c> sample.</returns>
    public static ValidationSpec Fallout4_PerkMagGunsAndBullets07()
    {
        var spec = BaseBook(SupportedGame.Fallout4, "PerkMagGunsAndBullets07", "092A8C:Fallout4.esm")
            .AddRule(ValidationFieldRule.TranslatedField("Name", "Name"))
            .AddRule(ValidationFieldRule.Field("Flags[0]", "Flags", ValidationValueNormalizer.HexInteger))
            .AddRule(ValidationFieldRule.FormKeyList("Keywords", "Keywords", "Keyword"))
            .AddRule(ValidationFieldRule.Field("Model.Data", "Models[0].Data", ValidationValueNormalizer.HexPayload))
            .AddRule(ValidationFieldRule.Field("Model.File", "Models[0].File", ValidationValueNormalizer.ModelFile))
            .AddRule(ValidationFieldRule.Field("Model.MaterialSwap", "Models[0].MaterialSwaps[0].MaterialSwapFormKey"))
            .AddRule(ValidationFieldRule.PathPrefix("VirtualMachineAdapter.Scripts", "ScriptingAdapters", ScriptingAdapterPathReplacements))
            .Build();
        spec.UiComparisonExpectations.Add(new ValidationUiComparisonExpectation(["Model", "File"]));
        spec.UiComparisonExpectations.Add(new ValidationUiComparisonExpectation(["Scripts", "Script [0]", "Name"]));
        return spec;
    }

    public static ValidationSpec Skyrim_AtrFrgDaedricRecipe00()
    {
        var spec = SkyrimBook("AtrFrgDaedricRecipe00", "10F776:Skyrim.esm");
        spec.Rules.Add(ValidationFieldRule.SoundSlot("PickUpSound", "PickUpSound", "Start"));
        return spec;
    }

    public static ValidationSpec Skyrim_Book0ArgonianAccountBook1()
    {
        return SkyrimBook("Book0ArgonianAccountBook1", "01AFD7:Skyrim.esm");
    }

    private static ValidationSpec StarfieldBook(string sampleName, string formKey, bool withHeaderFields)
    {
        var spec = BaseBook(SupportedGame.Starfield, sampleName, formKey)
            .AddRule(ValidationFieldRule.TranslatedField("Name", "Name"))
            .AddRule(ValidationFieldRule.TranslatedField("Text", "Text"));
        if (withHeaderFields)
        {
            spec.AddRule(ValidationFieldRule.TranslatedField("DataSlateHeaderLeft", "DataSlateHeaderLeft"));
            spec.AddRule(ValidationFieldRule.TranslatedField("DataSlateHeaderRight", "DataSlateHeaderRight"));
        }

        return spec
            .AddRule(ValidationFieldRule.Field("Model.File", "Models[0].File", ValidationValueNormalizer.ModelFile))
            .AddRule(ValidationFieldRule.Field("Model.LightLayer", "Models[0].LightLayer"))
            .AddRule(ValidationFieldRule.Field("Flags[0]", "Flags", ValidationValueNormalizer.HexInteger))
            .AddRule(ValidationFieldRule.FormKeyList("Keywords", "Keywords", "Keyword"))
            .AddRule(ValidationFieldRule.SoundSlot("DropdownSound.Start", "DropdownSound", "Start"))
            .AddRule(ValidationFieldRule.SoundSlot("PickupSound.Start", "PickupSound", "Start"))
            .AddRule(ValidationFieldRule.DtoExpectedValue("Components.Count", "1"))
            .AddRules(ValidationFieldRule.ComponentReflection(0, 0, 1, "LodOwnerComponent"))
            .AddRule(ValidationFieldRule.PathPrefix("VirtualMachineAdapter.Scripts", "ScriptingAdapters", ScriptingAdapterPathReplacements))
            .Build();
    }

    private static ValidationSpec Fallout4Book(string sampleName, string formKey, bool withScriptingAdapters)
    {
        var spec = BaseBook(SupportedGame.Fallout4, sampleName, formKey)
            .AddRule(ValidationFieldRule.TranslatedField("BookText", "Text", ValidationValueNormalizer.BookText))
            .AddRule(ValidationFieldRule.TranslatedField("Name", "Name"))
            .AddRule(ValidationFieldRule.Field("Model.Data", "Models[0].Data", ValidationValueNormalizer.HexPayload))
            .AddRule(ValidationFieldRule.Field("Model.File", "Models[0].File", ValidationValueNormalizer.ModelFile));
        if (withScriptingAdapters)
        {
            spec.AddRule(ValidationFieldRule.PathPrefix("VirtualMachineAdapter.Scripts", "ScriptingAdapters", ScriptingAdapterPathReplacements));
        }
        else
        {
            spec.AddRule(ValidationFieldRule.SpriggitAbsent("VirtualMachineAdapter.Scripts.Count"));
        }

        return spec.Build();
    }

    private static ValidationSpec SkyrimBook(string sampleName, string formKey)
    {
        return BaseBook(SupportedGame.Skyrim, sampleName, formKey)
            .AddRule(ValidationFieldRule.TranslatedField("BookText", "Text", ValidationValueNormalizer.BookText))
            .AddRule(ValidationFieldRule.TranslatedField("Name", "Name"))
            .AddRule(ValidationFieldRule.TranslatedField("Description", "Description"))
            .AddRule(ValidationFieldRule.Field("Keywords[0]", "Keywords[0].Keyword"))
            .AddRule(ValidationFieldRule.Field("Model.Data", "Models[0].Data", ValidationValueNormalizer.HexPayload))
            .AddRule(ValidationFieldRule.Field("Model.File", "Models[0].File", ValidationValueNormalizer.ModelFile))
            .Build();
    }

    private static ValidationSpecBuilder BaseBook(SupportedGame game, string sampleName, string formKey)
    {
        return ValidationSpecBuilder
            .ForRecord(game, RecordTypeCatalog.Book)
            .Sample(sampleName)
            .FormKey(formKey)
            .AddBaselineUiComparisonExpectations(
                new[] { "MajorRecordFlags" },
                new[] { "Weight" })
            .AddRule(ValidationFieldRule.DtoDefaultWhenSpriggitAbsent(
                "MajorRecordFlags",
                "MajorRecordFlags",
                "0",
                "Mutagen exposes the default MajorRecordFlags value when Spriggit omits the zero-valued field."))
            .AddRule(ValidationFieldRule.DtoDefaultWhenSpriggitAbsent(
                "Flags",
                "Flags",
                "0",
                "Mutagen exposes the default Flags value when Spriggit omits the zero-valued field."))
            .AddRule(ValidationFieldRule.DtoDefaultWhenSpriggitAbsent(
                "Value",
                "Value",
                "0",
                "Mutagen exposes the default Value when Spriggit omits the zero-valued field."))
            .AddRule(ValidationFieldRule.DtoDefaultWhenSpriggitAbsent(
                "Weight",
                "Weight",
                "0",
                "Mutagen exposes the default Weight when Spriggit omits the zero-valued field."));
    }
}
