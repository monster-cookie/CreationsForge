using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;

namespace CreationsForge.DataValidationTests.Validation.Specs.Terminal;

public static class TerminalValidationSpecs
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

    public static ValidationSpec Starfield_AkilaLife04_Computer()
    {
        return StarfieldTerminal("AkilaLife04_Computer", "2D1D29:Starfield.esm", withMenu: true, withFurnitureTemplate: true, withMajorRecordFlagsRaw: false);
    }

    public static ValidationSpec Starfield_AkilaLife08_FarmingComputer()
    {
        return StarfieldTerminal("AkilaLife08_FarmingComputer", "2D2617:Starfield.esm", withMenu: true, withFurnitureTemplate: true, withMajorRecordFlagsRaw: false);
    }

    public static ValidationSpec Starfield_BE_ShipComputer_BarStanding()
    {
        return StarfieldTerminal("BE_ShipComputer_BarStanding", "386CD0:Starfield.esm", withMenu: true, withFurnitureTemplate: true, withMajorRecordFlagsRaw: true, forcedLocationCount: 1);
    }

    public static ValidationSpec Starfield_City_NA_Botany02Terminal()
    {
        return StarfieldTerminal("City_NA_Botany02Terminal", "261A51:Starfield.esm", withMenu: true, withFurnitureTemplate: false, withMajorRecordFlagsRaw: false);
    }

    public static ValidationSpec Starfield_TerminalSittingActivatorA01_Desk()
    {
        return StarfieldTerminal("TerminalSittingActivatorA01_Desk", "19F266:Starfield.esm", withMenu: false, withFurnitureTemplate: true, withMajorRecordFlagsRaw: true);
    }

    public static ValidationSpec Fallout4_Vault111OverseerTPrimeDirective()
    {
        return Fallout4Terminal(
            "Vault111OverseerTPrimeDirective",
            "0AEF52:Fallout4.esm",
            withMajorRecordFlagsRaw: false,
            bodyTextCount: 0,
            menuItemCount: 0,
            withScriptFragments: false);
    }

    public static ValidationSpec Fallout4_Vault75OverseerTerminal()
    {
        return Fallout4Terminal(
            "Vault75OverseerTerminal",
            "0EC83C:Fallout4.esm",
            withMajorRecordFlagsRaw: true,
            bodyTextCount: 1,
            menuItemCount: 3,
            withScriptFragments: false);
    }

    public static ValidationSpec Fallout4_DN035_RobotControlTerminal_Targeting()
    {
        return Fallout4Terminal(
            "DN035_RobotControlTerminal_Targeting",
            "1221C8:Fallout4.esm",
            withMajorRecordFlagsRaw: true,
            bodyTextCount: 2,
            menuItemCount: 7,
            withScriptFragments: true);
    }

    private static ValidationSpec StarfieldTerminal(
        string sampleName,
        string formKey,
        bool withMenu,
        bool withFurnitureTemplate,
        bool withMajorRecordFlagsRaw,
        int forcedLocationCount = 0)
    {
        var spec = BaseTerminal(SupportedGame.Starfield, sampleName, formKey)
            .AddRule(ValidationFieldRule.TranslatedField("Name", "Name", requireAllLanguages: true))
            .AddRule(ValidationFieldRule.Field("Background", "Background"))
            .AddRule(ValidationFieldRule.Field("PNAM", "Pnam", ValidationValueNormalizer.HexPayload))
            .AddRule(ValidationFieldRule.Field("FNAM", "Fnam", ValidationValueNormalizer.HexPayload))
            .AddRule(ValidationFieldRule.ScalarList("MajorFlags", "MajorFlags"))
            .AddRule(ValidationFieldRule.Field("JNAM", "Jnam", ValidationValueNormalizer.HexPayload))
            .AddRule(ValidationFieldRule.Field("MarkerFlags", "MarkerFlags"))
            .AddRule(ValidationFieldRule.Field("GNAM", "Gnam", ValidationValueNormalizer.HexPayload))
            .AddRule(ValidationFieldRule.Field("WorkbenchData", "WorkbenchData", ValidationValueNormalizer.HexPayload))
            .AddRule(ValidationFieldRule.Field("MarkerModel", "MarkerModel"))
            .AddRule(ValidationFieldRule.Field("Model.File", "Models[0].File", ValidationValueNormalizer.ModelFile))
            .AddRule(ValidationFieldRule.Field("Model.LightLayer", "Models[0].LightLayer"))
            .AddRule(ValidationFieldRule.DtoNonEmpty("Components[0].ANAM", "AnimationGraph"))
            .AddRule(ValidationFieldRule.DtoNonEmpty("Components[0].BNAM", "AnimationSkeleton"))
            .AddRule(ValidationFieldRule.DtoNonEmpty("Components[0].CNAM", "AnimationDirectory"))
            .AddRule(ValidationFieldRule.RawPayloadSlot("Components[1].REFL", "Components.EffectSequenceComponentBinaryOverlay.REFL"))
            .AddRule(ValidationFieldRule.IgnoreSpriggit("Components[0].MutagenObjectType", "AnimationGraphComponent values are projected into direct animation fields."))
            .AddRule(ValidationFieldRule.IgnoreSpriggit("Components[1].MutagenObjectType", "REFL component payload type is stored on raw payload rows."))
            .AddRule(ValidationFieldRule.IgnoreDtoPrefix("LocalizedStrings", "Localized backing rows are validated through translated field rules."))
            .AddRule(ValidationFieldRule.PathPrefix("VirtualMachineAdapter.Scripts", "ScriptingAdapters", ScriptingAdapterPathReplacements));

        AddMarkerParameterRules(spec, withUnknown: false);
        AddForcedLocationRules(spec, forcedLocationCount);

        if (withMenu)
        {
            spec.AddRule(ValidationFieldRule.Field("Menu", "MenuFormKey"));
        }

        if (withFurnitureTemplate)
        {
            spec.AddRule(ValidationFieldRule.Field("FurnitureTemplate", "FurnitureTemplateFormKey"));
        }

        AddMajorRecordFlagRules(spec, withMajorRecordFlagsRaw, "StarfieldMajorRecordFlags");
        return spec.Build();
    }

    private static ValidationSpec Fallout4Terminal(
        string sampleName,
        string formKey,
        bool withMajorRecordFlagsRaw,
        int bodyTextCount,
        int menuItemCount,
        bool withScriptFragments)
    {
        var spec = BaseTerminal(SupportedGame.Fallout4, sampleName, formKey)
            .AddRule(ValidationFieldRule.TranslatedField("Name", "Name", requireAllLanguages: true))
            .AddRule(ValidationFieldRule.TranslatedField("HeaderText", "HeaderText", requireAllLanguages: true))
            .AddRule(ValidationFieldRule.TranslatedField("WelcomeText", "WelcomeText", requireAllLanguages: true))
            .AddRule(ValidationFieldRule.Field("PNAM", "Pnam", ValidationValueNormalizer.HexPayload))
            .AddRule(ValidationFieldRule.Field("FNAM", "Fnam", ValidationValueNormalizer.HexPayload))
            .AddRule(ValidationFieldRule.Field("Flags[0]", "Flags"))
            .AddRule(ValidationFieldRule.Field("WorkbenchData", "WorkbenchData", ValidationValueNormalizer.HexPayload))
            .AddRule(ValidationFieldRule.Field("Model.File", "Models[0].File", ValidationValueNormalizer.ModelFile))
            .AddRule(ValidationFieldRule.Field("Model.Data", "Models[0].Data", ValidationValueNormalizer.HexPayload));

        AddMarkerParameterRules(spec, withUnknown: true);
        AddTerminalBodyTextRules(spec, bodyTextCount);
        AddTerminalMenuItemRules(spec, menuItemCount);
        AddMajorRecordFlagRules(spec, withMajorRecordFlagsRaw, "Fallout4MajorRecordFlags");

        if (withScriptFragments)
        {
            AddScriptFragmentRules(spec, fragmentCount: 6);
        }

        return spec.Build();
    }

    private static ValidationSpecBuilder BaseTerminal(SupportedGame game, string sampleName, string formKey)
    {
        return ValidationSpecBuilder.ForRecord(game, RecordTypeCatalog.Terminal)
            .Sample(sampleName)
            .FormKey(formKey)
            .AddRule(ValidationFieldRule.Field("ObjectBounds.First", "ObjectBoundsFirst"))
            .AddRule(ValidationFieldRule.Field("ObjectBounds.Second", "ObjectBoundsSecond"))
            .AddRule(ValidationFieldRule.FormKeyList("Keywords", "Keywords", "Keyword"))
            .AddRule(ValidationFieldRule.IgnoreDtoPrefix("LocalizedStrings", "Localized backing rows are validated through translated field rules."));
    }

    private static void AddMarkerParameterRules(ValidationSpecBuilder spec, bool withUnknown)
    {
        spec
            .AddRule(ValidationFieldRule.DtoDefaultWhenSpriggitAbsent(
                "MarkerParameters[0].Enabled",
                "MarkerParameters[0].Enabled",
                "Null",
                "Spriggit omits the marker parameter enabled value when the DTO preserves the nullable field as null."))
            .AddRule(ValidationFieldRule.Field("MarkerParameters[0].Offset", "MarkerParameters[0].Offset"))
            .AddRule(ValidationFieldRule.DtoNonEmpty("MarkerParameters[0].EntryTypes", "MarkerParameters[0].EntryTypes"))
            .AddRule(ValidationFieldRule.DtoNonEmpty("MarkerParameters[0].ExitTypes", "MarkerParameters[0].ExitTypes"));

        if (withUnknown)
        {
            spec.AddRule(ValidationFieldRule.Field("MarkerParameters[0].Unknown", "MarkerParameters[0].Unknown", ValidationValueNormalizer.HexPayload));
        }
    }

    private static void AddForcedLocationRules(ValidationSpecBuilder spec, int forcedLocationCount)
    {
        if (forcedLocationCount == 0)
        {
            return;
        }

        spec.AddRule(ValidationFieldRule.Field("ForcedLocations.Count", "ForcedLocations.Count"));
        for (var index = 0; index < forcedLocationCount; index++)
        {
            spec.AddRule(ValidationFieldRule.Field($"ForcedLocations[{index}]", $"ForcedLocations[{index}]"));
        }
    }

    private static void AddTerminalBodyTextRules(ValidationSpecBuilder spec, int bodyTextCount)
    {
        if (bodyTextCount == 0)
        {
            spec.AddRule(ValidationFieldRule.IgnoreSpriggit("BodyTexts", "Empty Spriggit collection root is covered by absent body text item rules."));
            return;
        }

        for (var index = 0; index < bodyTextCount; index++)
        {
            spec
                .AddRule(ValidationFieldRule.IgnoreSpriggit($"BodyTexts[{index}]", "Inline YAML list item root is covered by body text child rules."))
                .AddRule(ValidationFieldRule.TranslatedField($"BodyTexts[{index}].Text", $"BodyTexts[{index}].Text", ValidationValueNormalizer.TerminalText, requireAllLanguages: true))
                .AddRule(ValidationFieldRule.PathPrefix($"BodyTexts[{index}].Conditions", $"BodyTexts[{index}].Conditions", NoPathReplacements));
        }
    }

    private static void AddTerminalMenuItemRules(ValidationSpecBuilder spec, int menuItemCount)
    {
        if (menuItemCount == 0)
        {
            spec.AddRule(ValidationFieldRule.IgnoreSpriggit("MenuItems", "Empty Spriggit collection root is covered by absent menu item rules."));
            return;
        }

        for (var index = 0; index < menuItemCount; index++)
        {
            spec
                .AddRule(ValidationFieldRule.IgnoreSpriggit($"MenuItems[{index}]", "Inline YAML list item root is covered by menu item child rules."))
                .AddRule(ValidationFieldRule.TranslatedField($"MenuItems[{index}].ItemText", $"MenuItems[{index}].ItemText", ValidationValueNormalizer.TerminalText, requireAllLanguages: true))
                .AddRule(ValidationFieldRule.Field($"MenuItems[{index}].Type", $"MenuItems[{index}].Type"))
                .AddRule(ValidationFieldRule.Field($"MenuItems[{index}].ItemId", $"MenuItems[{index}].ItemId"))
                .AddRule(ValidationFieldRule.DtoDefaultWhenSpriggitAbsent(
                    $"MenuItems[{index}].Submenu",
                    $"MenuItems[{index}].Submenu",
                    "Null",
                    "Spriggit omits submenu for terminal menu items that do not link to a submenu."))
                .AddRule(ValidationFieldRule.TranslatedField($"MenuItems[{index}].DisplayText", $"MenuItems[{index}].DisplayText", ValidationValueNormalizer.TerminalText, requireAllLanguages: true))
                .AddRule(ValidationFieldRule.PathPrefix($"MenuItems[{index}].Conditions", $"MenuItems[{index}].Conditions", NoPathReplacements));
        }
    }

    private static void AddScriptFragmentRules(ValidationSpecBuilder spec, int fragmentCount)
    {
        spec
            .AddRule(ValidationFieldRule.OptionalField(
            "VirtualMachineAdapter.ScriptFragments.ExtraBindDataVersion",
            "ScriptFragments[0].ExtraBindDataVersion"))
            .AddRule(ValidationFieldRule.DtoDefaultWhenSpriggitAbsent(
                "VirtualMachineAdapter.ScriptFragments.ExtraBindDataVersion",
                "ScriptFragments[0].ExtraBindDataVersion",
                "3",
                "Mutagen exposes the script-fragment bind data version when Spriggit omits the default value."));

        for (var fragmentIndex = 0; fragmentIndex < fragmentCount; fragmentIndex++)
        {
            var spriggitPath = "VirtualMachineAdapter.ScriptFragments.Fragments[" + fragmentIndex.ToString(System.Globalization.CultureInfo.InvariantCulture) + "]";
            var dtoPath = "ScriptFragments[" + (fragmentIndex + 1).ToString(System.Globalization.CultureInfo.InvariantCulture) + "]";
            spec
                .AddRule(ValidationFieldRule.Field(spriggitPath + ".FragmentName", dtoPath + ".FragmentName"))
                .AddRule(ValidationFieldRule.Field(spriggitPath + ".FragmentIndex", dtoPath + ".SourceFragmentIndex"))
                .AddRule(ValidationFieldRule.Field(spriggitPath + ".ScriptName", dtoPath + ".ScriptName"))
                .AddRule(ValidationFieldRule.Field(spriggitPath + ".Unknown2", dtoPath + ".Unknown2"));
        }

        var scriptFragmentIndex = fragmentCount + 1;
        spec
            .AddRule(ValidationFieldRule.Field(
                "VirtualMachineAdapter.ScriptFragments.Script.Name",
                "ScriptFragments[" + scriptFragmentIndex.ToString(System.Globalization.CultureInfo.InvariantCulture) + "].ScriptName"))
            .AddRule(ValidationFieldRule.DtoNonEmpty("ScriptingAdapters[0].Name"))
            .AddRule(ValidationFieldRule.PathPrefix(
                "VirtualMachineAdapter.ScriptFragments.Script.Properties",
                "ScriptingAdapters[0].Properties",
                ScriptingAdapterPathReplacements));

        for (var dtoFragmentIndex = 0; dtoFragmentIndex <= scriptFragmentIndex; dtoFragmentIndex++)
        {
            var dtoPath = "ScriptFragments[" + dtoFragmentIndex.ToString(System.Globalization.CultureInfo.InvariantCulture) + "]";
            spec
                .AddRule(ValidationFieldRule.IgnoreDto(dtoPath + ".FragmentIndex", "FragmentIndex is DTO collection metadata for repository read-back."))
                .AddRule(ValidationFieldRule.IgnoreDto(dtoPath + ".FragmentSlot", "FragmentSlot is DTO collection metadata for repository read-back."))
                .AddRule(ValidationFieldRule.IgnoreDto(dtoPath + ".MutagenObjectType", "MutagenObjectType is DTO implementation metadata for script-fragment read-back."));
        }

        spec
            .AddRule(ValidationFieldRule.IgnoreDto("ScriptFragments[0].FragmentIndex", "FragmentIndex is DTO collection metadata for the script-fragment root row."))
            .AddRule(ValidationFieldRule.IgnoreDto(
                "ScriptFragments[" + scriptFragmentIndex.ToString(System.Globalization.CultureInfo.InvariantCulture) + "].FragmentIndex",
                "FragmentIndex is DTO collection metadata for the fragment-owned script row."));
    }

    private static void AddMajorRecordFlagRules(ValidationSpecBuilder spec, bool withMajorRecordFlagsRaw, string flagListName)
    {
        if (withMajorRecordFlagsRaw)
        {
            spec
                .AddRule(ValidationFieldRule.Field("MajorRecordFlagsRaw", "MajorRecordFlags"))
                .AddRule(ValidationFieldRule.IgnoreDto("MajorFlags", "MajorRecordFlagsRaw covers the raw flag value."))
                .AddRule(ValidationFieldRule.IgnoreSpriggit("MajorFlags.Count", "MajorRecordFlagsRaw covers the flag value."))
                .AddRule(ValidationFieldRule.IgnoreSpriggit("MajorFlags[0]", "MajorRecordFlagsRaw covers the flag value."))
                .AddRule(ValidationFieldRule.IgnoreSpriggit("MajorFlags[1]", "MajorRecordFlagsRaw covers the flag value."))
                .AddRule(ValidationFieldRule.IgnoreSpriggit(flagListName + ".Count", "MajorRecordFlagsRaw covers the flag value."))
                .AddRule(ValidationFieldRule.IgnoreSpriggit(flagListName + "[0]", "MajorRecordFlagsRaw covers the flag value."))
                .AddRule(ValidationFieldRule.IgnoreSpriggit(flagListName + "[1]", "MajorRecordFlagsRaw covers the flag value."));
            return;
        }

        spec.AddRule(ValidationFieldRule.DtoDefaultWhenSpriggitAbsent(
            "MajorRecordFlags",
            "MajorRecordFlags",
            "0",
            "Mutagen exposes the default MajorRecordFlags value when Spriggit omits the zero-valued field."));
    }
}
