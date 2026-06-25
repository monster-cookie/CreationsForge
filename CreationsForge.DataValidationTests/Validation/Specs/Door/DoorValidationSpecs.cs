using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;

namespace CreationsForge.DataValidationTests.Validation.Specs.Door;

public static class DoorValidationSpecs
{
    private static readonly IReadOnlyDictionary<string, string> ScriptingAdapterPathReplacements =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [".Objects"] = ".ListItems",
            [".Object"] = ".ObjectFormKey",
            [".Alias"] = ".ObjectAlias",
            [".Unused"] = ".ObjectUnused",
            [".Data"] = ".DataInt"
        };

    private static readonly IReadOnlyDictionary<string, string> NoPathReplacements =
        new Dictionary<string, string>(StringComparer.Ordinal);

    public static ValidationSpec Starfield_ShipFloorLoadHatch()
    {
        return StarfieldDoor("ShipFloorLoadHatch", "144F85:Starfield.esm").Build();
    }

    public static ValidationSpec Starfield_ShipDockingHatchFloor()
    {
        return StarfieldDoor("ShipDockingHatchFloor", "205AA6:Starfield.esm").Build();
    }

    public static ValidationSpec Starfield_SftIntRmSmWallMid_DoorA00()
    {
        return StarfieldDoor("SftIntRmSmWallMid_DoorA00", "19AFF6:Starfield.esm")
            .AddRule(ValidationFieldRule.Field("MajorRecordFlagsRaw", "MajorRecordFlags"))
            .AddRule(ValidationFieldRule.ScalarList("StarfieldMajorRecordFlags", "MajorFlags", ValidationValueNormalizer.StarfieldMajorFlagName))
            .Build();
    }

    public static ValidationSpec Starfield_SftIntRmSmWallMid_DoorA00_Loud()
    {
        return StarfieldDoor("SftIntRmSmWallMid_DoorA00_Loud", "30D813:Starfield.esm")
            .AddRule(ValidationFieldRule.Field("MajorRecordFlagsRaw", "MajorRecordFlags"))
            .AddRule(ValidationFieldRule.ScalarList("StarfieldMajorRecordFlags", "MajorFlags", ValidationValueNormalizer.StarfieldMajorFlagName))
            .Build();
    }

    public static ValidationSpec Starfield_ShpGenIntPerSmWallMid_ExLg_DockingDoor02L_NonLoad()
    {
        return StarfieldDoor("ShpGenIntPerSmWallMid_ExLg_DockingDoor02L_NonLoad", "31D042:Starfield.esm")
            .AddRules(ValidationFieldRule.ComponentReflection(1, 0, 1, "EffectSequenceComponent"))
            .Build();
    }

    public static ValidationSpec Fallout4_AutoloadDoor()
    {
        return Fallout4Door("AutoloadDoor", "01ED77:Fallout4.esm").Build();
    }

    public static ValidationSpec Fallout4_BldWoodPDbDoor01()
    {
        return Fallout4Door("BldWoodPDbDoor01", "01D930:Fallout4.esm")
            .AddRule(ValidationFieldRule.Field("MajorRecordFlagsRaw", "MajorRecordFlags"))
            .AddRule(ValidationFieldRule.ScalarList("Fallout4MajorRecordFlags", "MajorRecordFlags", ValidationValueNormalizer.HexInteger))
            .Build();
    }

    public static ValidationSpec Skyrim_AutoLoadDoor01()
    {
        return SkyrimDoor("AutoLoadDoor01", "031897:Skyrim.esm").Build();
    }

    public static ValidationSpec Skyrim_DBBlackDoor()
    {
        return SkyrimDoor("DBBlackDoor", "022F44:Skyrim.esm").Build();
    }

    private static ValidationSpecBuilder StarfieldDoor(string sampleName, string formKey)
    {
        return BaseDoor(SupportedGame.Starfield, sampleName, formKey)
            .AddRule(ValidationFieldRule.Field("Model.File", "Models[0].File", ValidationValueNormalizer.ModelFile))
            .AddRule(ValidationFieldRule.ScalarList("Flags", "Flags"))
            .AddRule(ValidationFieldRule.ScalarList("MajorFlags", "MajorFlags"))
            .AddRule(ValidationFieldRule.ScalarList("Model.Flags", "Models[0].Flags"))
            .AddRule(ValidationFieldRule.FormKeyList("Model.MaterialSwaps", "Models[0].MaterialSwaps", "MaterialSwapFormKey"))
            .AddRule(ValidationFieldRule.FormKeyList("Keywords", "Keywords", "Keyword"))
            .AddRule(ValidationFieldRule.FormKeyList("ForcedLocations", "ForcedLocations", string.Empty))
            .AddRule(ValidationFieldRule.DtoNonEmpty("Components[0].ANAM", "AnimationGraph"))
            .AddRule(ValidationFieldRule.DtoNonEmpty("Components[0].BNAM", "AnimationSkeleton"))
            .AddRule(ValidationFieldRule.DtoNonEmpty("Components[0].CNAM", "AnimationDirectory"))
            .AddRule(ValidationFieldRule.Field("Model.LightLayer", "Models[0].LightLayer"))
            .AddRule(ValidationFieldRule.SoundSlot("OpenSound.Start", "OpenSound", "Start"))
            .AddRule(ValidationFieldRule.SoundSlot("CloseSound.Start", "CloseSound", "Start"))
            .AddRule(ValidationFieldRule.PathPrefix("NavmeshGeometry", "NavmeshGeometry", NoPathReplacements))
            .AddRule(ValidationFieldRule.PathPrefix("VirtualMachineAdapter.Scripts", "ScriptingAdapters", ScriptingAdapterPathReplacements));
    }

    private static ValidationSpecBuilder Fallout4Door(string sampleName, string formKey)
    {
        return BaseDoor(SupportedGame.Fallout4, sampleName, formKey)
            .AddRule(ValidationFieldRule.ScalarList("Flags", "Flags"))
            .AddRule(ValidationFieldRule.ScalarList("MajorFlags", "MajorFlags"))
            .AddRule(ValidationFieldRule.Field("Model.Data", "Models[0].Data", ValidationValueNormalizer.HexPayload))
            .AddRule(ValidationFieldRule.Field("Model.File", "Models[0].File", ValidationValueNormalizer.ModelFile))
            .AddRule(ValidationFieldRule.SoundSlot("OpenSound", "OpenSound", "Start"))
            .AddRule(ValidationFieldRule.SoundSlot("CloseSound", "CloseSound", "Start"));
    }

    private static ValidationSpecBuilder SkyrimDoor(string sampleName, string formKey)
    {
        return BaseDoor(SupportedGame.Skyrim, sampleName, formKey)
            .AddRule(ValidationFieldRule.ScalarList("Flags", "Flags"))
            .AddRule(ValidationFieldRule.ScalarList("MajorFlags", "MajorFlags"))
            .AddRule(ValidationFieldRule.Field("Model.Data", "Models[0].Data", ValidationValueNormalizer.HexPayload))
            .AddRule(ValidationFieldRule.Field("Model.File", "Models[0].File", ValidationValueNormalizer.ModelFile))
            .AddRule(ValidationFieldRule.SoundSlot("OpenSound", "OpenSound", "Start"))
            .AddRule(ValidationFieldRule.SoundSlot("CloseSound", "CloseSound", "Start"))
            .AddRule(ValidationFieldRule.PathPrefix("VirtualMachineAdapter.Scripts", "ScriptingAdapters", ScriptingAdapterPathReplacements));
    }

    private static ValidationSpecBuilder BaseDoor(SupportedGame game, string sampleName, string formKey)
    {
        return ValidationSpecBuilder
            .ForRecord(game, RecordTypeCatalog.Door)
            .Sample(sampleName)
            .FormKey(formKey)
            .AddRule(ValidationFieldRule.TranslatedField("Name", "Name"))
            .AddRule(ValidationFieldRule.OptionalField("ObjectBounds.First", "ObjectBoundsFirst"))
            .AddRule(ValidationFieldRule.OptionalField("ObjectBounds.Second", "ObjectBoundsSecond"))
            .AddRule(ValidationFieldRule.Field("NativeTerminal", "NativeTerminalFormKey"))
            .AddRule(ValidationFieldRule.Field("FacingAxisOverride", "FacingAxisOverride"))
            .AddRule(ValidationFieldRule.Field("SoundLevel", "SoundLevel"))
            .AddRule(ValidationFieldRule.IgnoreDto(
                "ObjectBoundsFirst",
                "Mutagen exposes default object bounds when Spriggit omits ObjectBounds."))
            .AddRule(ValidationFieldRule.IgnoreDto(
                "ObjectBoundsSecond",
                "Mutagen exposes default object bounds when Spriggit omits ObjectBounds."))
            .AddRule(ValidationFieldRule.IgnoreDtoPrefix("LocalizedStrings", "LocalizedStrings is the DTO projection of translated Spriggit fields."));
    }
}
