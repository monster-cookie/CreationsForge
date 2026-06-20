using CreationsForge.Core.DTOs.Games;
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Models.Configuration;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services;
using CreationsForge.Core.Services.Interfaces;
using Shouldly;

namespace CreationsForge.UnitTests.Services;

public class RecordComparisonServiceTests
{
    [Fact]
    public void GetRecordComparison_ForGlobal_CreatesPluginColumnsAndDataRows()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x123);
        var globalRepository = new TestGlobalRepository
        {
            Records =
            [
                CreateGlobal("Base.esm", formKey, "MyGlobal", 1.5),
                CreateGlobal("Patch.esp", formKey, "MyGlobal", 2.5)
            ]
        };
        var service = CreateService(globalRepository: globalRepository);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.Global.RecordID, formKey);

        comparison.RecordType.ShouldBe(RecordTypeCatalog.Global.RecordID);
        comparison.Columns.Select(column => column.Header).ShouldBe(["Base.esm", "Patch.esp"]);
        comparison.Fields.Single(field => field.FieldName == "Data").Values.Select(value => value.DisplayValue).ShouldBe(["1.5", "2.5"]);
        comparison.Fields.Single(field => field.FieldName == "Data").State.ShouldBe(RecordComparisonValueState.Conflict);
        comparison.Fields.Single(field => field.FieldName == "Data").Values.Select(value => value.State).ShouldBe([RecordComparisonValueState.Conflict, RecordComparisonValueState.WinningOverride]);
    }

    [Fact]
    public void GetRecordComparison_ForGameSetting_HidesRedundantTypedValueFields()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x456);
        var gameSettingRepository = new TestGameSettingRepository
        {
            Records =
            [
                CreateGameSetting("Base.esm", formKey, "fSetting", "Float", "1.25", numericData: 1.25),
                CreateGameSetting("Patch.esp", formKey, "fSetting", "Float", "1.75", numericData: 1.75)
            ]
        };
        var service = CreateService(gameSettingRepository: gameSettingRepository);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.GameSetting.RecordID, formKey);

        comparison.Columns.Select(column => column.Header).ShouldBe(["Base.esm", "Patch.esp"]);
        comparison.Fields.Single(field => field.FieldName == "SettingType").Values.Select(value => value.DisplayValue).ShouldBe(["Float", "Float"]);
        comparison.Fields.Single(field => field.FieldName == "Data").Values.Select(value => value.DisplayValue).ShouldBe(["1.25", "1.75"]);
        comparison.Fields.ShouldNotContain(field => field.FieldName == "NumericData");
        comparison.Fields.ShouldNotContain(field => field.FieldName == "IntegerData");
        comparison.Fields.ShouldNotContain(field => field.FieldName == "BooleanData");
    }

    [Fact]
    public void GetRecordComparison_ForGameSetting_UsesSelectedLocalizedData()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x457);
        var gameSettingRepository = new TestGameSettingRepository
        {
            Records =
            [
                CreateGameSetting("Base.esm", formKey, "sSetting", "String", "Base English"),
                CreateGameSetting("Patch.esp", formKey, "sSetting", "String", "Patch English")
            ]
        };
        var localizedStringRepository = new TestRecordLocalizedStringRepository
        {
            Records =
            [
                CreateLocalizedString("Base.esm", formKey, "Data", "English", "Base English"),
                CreateLocalizedString("Base.esm", formKey, "Data", "German", "Base German"),
                CreateLocalizedString("Patch.esp", formKey, "Data", "English", "Patch English"),
                CreateLocalizedString("Patch.esp", formKey, "Data", "German", "Patch German")
            ]
        };
        var gameSelectionService = new TestGameSelectionService { RecordTextLanguage = "German" };
        var service = CreateService(
            gameSettingRepository: gameSettingRepository,
            recordLocalizedStringRepository: localizedStringRepository,
            gameSelectionService: gameSelectionService);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.GameSetting.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "Data").Values.Select(value => value.DisplayValue).ShouldBe(["Base German", "Patch German"]);
    }

    [Fact]
    public void GetRecordComparison_ForFormList_ExpandsItemSlots()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x789);
        var firstItem = CreateFormKey("Starfield.esm", 0x111);
        var secondItem = CreateFormKey("Starfield.esm", 0x222);
        var formListRepository = new TestFormListRepository
        {
            Records =
            [
                CreateFormList("Base.esm", formKey, [CreateFormListItem("Base.esm", formKey, firstItem, 0)]),
                CreateFormList("Patch.esp", formKey, [CreateFormListItem("Patch.esp", formKey, firstItem, 0), CreateFormListItem("Patch.esp", formKey, secondItem, 1)])
            ]
        };
        var service = CreateService(formListRepository: formListRepository);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.FormList.RecordID, formKey);

        comparison.Columns.Select(column => column.Header).ShouldBe(["Base.esm", "Patch.esp"]);
        comparison.Fields.Single(field => field.FieldName == "Items[0]").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000111", "Starfield.esm:00000111"]);
        comparison.Fields.Single(field => field.FieldName == "Items[0]").State.ShouldBe(RecordComparisonValueState.Identical);
        comparison.Fields.Single(field => field.FieldName == "Items[0]").Values.Select(value => value.State).ShouldBe([RecordComparisonValueState.Identical, RecordComparisonValueState.Identical]);
        comparison.Fields.Single(field => field.FieldName == "Items[1]").Values.Select(value => value.DisplayValue).ShouldBe(["", "Starfield.esm:00000222"]);
        comparison.Fields.Single(field => field.FieldName == "Items[1]").State.ShouldBe(RecordComparisonValueState.Conflict);
    }

    [Fact]
    public void GetRecordComparison_ForMiscObject_MapsTypedScalarFields()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x818);
        var messageFormKey = CreateFormKey("Starfield.esm", 0x444);
        var miscObjectRepository = new TestMiscObjectRepository
        {
            Records =
            [
                CreateMiscObject("Base.esm", formKey, "Digipick", 35, 0.1f, null),
                CreateMiscObject("Patch.esp", formKey, "Digipick", 50, 0.2f, messageFormKey)
            ]
        };
        var modelRepository = new TestModelRepository
        {
            Records =
            [
                CreateModel("Base.esm", formKey, "Meshes\\Clutter\\Digipick.nif"),
                CreateModel("Patch.esp", formKey, "Meshes\\Clutter\\Digipick.nif")
            ]
        };
        var scriptingAdapterRepository = new TestScriptingAdapterRepository
        {
            Records =
            [
                CreateScriptingAdapter("Base.esm", formKey, "DefaultScript", "PropertyName", "BaseValue"),
                CreateScriptingAdapter("Patch.esp", formKey, "DefaultScript", "PropertyName", "PatchValue")
            ]
        };
        var recordKeywordRepository = new TestRecordKeywordRepository
        {
            Records =
            [
                CreateRecordKeyword("Base.esm", RecordTypeCatalog.MiscObject.RecordID, formKey, CreateFormKey("Starfield.esm", 0x555), 0),
                CreateRecordKeyword("Patch.esp", RecordTypeCatalog.MiscObject.RecordID, formKey, CreateFormKey("Starfield.esm", 0x555), 0)
            ]
        };
        var recordSoundRepository = new TestRecordSoundRepository
        {
            Records =
            [
                CreateRecordSound("Base.esm", RecordTypeCatalog.MiscObject.RecordID, formKey, "PickupSound", 1, "ff0b45e7-a8ae-a30f-390b-d0cd2b6933a6"),
                CreateRecordSound("Patch.esp", RecordTypeCatalog.MiscObject.RecordID, formKey, "PickupSound", 1, "ff0b45e7-a8ae-a30f-390b-d0cd2b6933a6")
            ]
        };
        var service = CreateService(
            miscObjectRepository: miscObjectRepository,
            modelRepository: modelRepository,
            recordKeywordRepository: recordKeywordRepository,
            recordSoundRepository: recordSoundRepository,
            scriptingAdapterRepository: scriptingAdapterRepository);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.MiscObject.RecordID, formKey);

        comparison.Columns.Select(column => column.Header).ShouldBe(["Base.esm", "Patch.esp"]);
        comparison.Fields.Single(field => field.FieldName == "Name").Values.Select(value => value.DisplayValue).ShouldBe(["Digipick", "Digipick"]);
        comparison.Fields.Single(field => field.FieldName == "Value").Values.Select(value => value.DisplayValue).ShouldBe(["35", "50"]);
        comparison.Fields.Single(field => field.FieldName == "Weight").Values.Select(value => value.DisplayValue).ShouldBe(["0.1", "0.2"]);
        comparison.Fields.Single(field => field.FieldName == "FeaturedItemMessageFormKey").Values.Select(value => value.DisplayValue).ShouldBe(["", "Starfield.esm:00000444"]);
        var keywords = comparison.Fields.Single(field => field.FieldName == "Keywords");
        keywords.Children.Single(field => field.FieldName == "Keyword [0]").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000555", "Starfield.esm:00000555"]);
        var model = comparison.Fields.Single(field => field.FieldName == "Model");
        model.Children.Single(field => field.FieldName == "File").Values.Select(value => value.DisplayValue).ShouldBe(["Meshes\\Clutter\\Digipick.nif", "Meshes\\Clutter\\Digipick.nif"]);
        var sounds = comparison.Fields.Single(field => field.FieldName == "Sounds");
        var pickupSound = sounds.Children.Single(field => field.FieldName == "PickupSound [1]");
        pickupSound.Children.Single(field => field.FieldName == "Start").Values.Select(value => value.DisplayValue).ShouldBe(["ff0b45e7-a8ae-a30f-390b-d0cd2b6933a6", "ff0b45e7-a8ae-a30f-390b-d0cd2b6933a6"]);

        var scripts = comparison.Fields.Single(field => field.FieldName == "Scripts");
        var script = scripts.Children.Single(field => field.FieldName == "Script [0]");
        script.Children.Single(field => field.FieldName == "Name").Values.Select(value => value.DisplayValue).ShouldBe(["DefaultScript", "DefaultScript"]);
        var property = script.Children.Single(field => field.FieldName == "Property [0]");
        property.Children.Single(field => field.FieldName == "Name").Values.Select(value => value.DisplayValue).ShouldBe(["PropertyName", "PropertyName"]);
        property.Children.Single(field => field.FieldName == "Value").Values.Select(value => value.DisplayValue).ShouldBe(["BaseValue", "PatchValue"]);
    }

    [Fact]
    public void GetRecordComparison_ForMagicEffect_ExpandsKeywordsAndFlattensMagicEffectData()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x2C5A68);
        var magicEffectRepository = new TestMagicEffectRepository
        {
            Records =
            [
                CreateMagicEffect("Base.esm", formKey, "Elemental Blast", "52", 5),
                CreateMagicEffect("Patch.esp", formKey, "Elemental Blast", "52", 7)
            ]
        };
        var recordKeywordRepository = new TestRecordKeywordRepository
        {
            Records =
            [
                CreateRecordKeyword("Base.esm", RecordTypeCatalog.MagicEffect.RecordID, formKey, CreateFormKey("Starfield.esm", 0x111), 0),
                CreateRecordKeyword("Patch.esp", RecordTypeCatalog.MagicEffect.RecordID, formKey, CreateFormKey("Patch.esp", 0x222), 0)
            ]
        };
        var scriptingAdapterRepository = new TestScriptingAdapterRepository
        {
            Records =
            [
                CreateScriptingAdapter("Base.esm", RecordTypeCatalog.MagicEffect.RecordID, formKey, "FXScripts:FXResourceCollectionVisuals", "TargetVFX", "BaseVFX"),
                CreateScriptingAdapter("Patch.esp", RecordTypeCatalog.MagicEffect.RecordID, formKey, "FXScripts:FXResourceCollectionVisuals", "TargetVFX", "PatchVFX")
            ]
        };
        var recordSoundRepository = new TestRecordSoundRepository
        {
            Records =
            [
                CreateRecordSound("Base.esm", RecordTypeCatalog.MagicEffect.RecordID, formKey, "Charge", 2, "a328413d-b619-45b5-0d9e-aa9d0ade8280", "Break0", "000000"),
                CreateRecordSound("Patch.esp", RecordTypeCatalog.MagicEffect.RecordID, formKey, "Charge", 2, "a328413d-b619-45b5-0d9e-aa9d0ade8280", "Break0", "000000")
            ]
        };
        var service = CreateService(
            magicEffectRepository: magicEffectRepository,
            recordKeywordRepository: recordKeywordRepository,
            recordSoundRepository: recordSoundRepository,
            scriptingAdapterRepository: scriptingAdapterRepository);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.MagicEffect.RecordID, formKey);

        var keywords = comparison.Fields.Single(field => field.FieldName == "Keywords");
        keywords.Children.Single(field => field.FieldName == "Keyword [0]").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000111", "Patch.esp:00000222"]);

        comparison.Fields.ShouldNotContain(field => field.FieldName == "Data");
        comparison.Fields.Single(field => field.FieldName == "Archetype").Values.Select(value => value.DisplayValue).ShouldBe(["52", "52"]);
        comparison.Fields.Single(field => field.FieldName == "UnknownInt2").Values.Select(value => value.DisplayValue).ShouldBe(["5", "7"]);
        var sounds = comparison.Fields.Single(field => field.FieldName == "Sounds");
        var chargeSound = sounds.Children.Single(field => field.FieldName == "Charge [2]");
        chargeSound.Children.Single(field => field.FieldName == "Start").Values.Select(value => value.DisplayValue).ShouldBe(["a328413d-b619-45b5-0d9e-aa9d0ade8280", "a328413d-b619-45b5-0d9e-aa9d0ade8280"]);
        chargeSound.Children.Single(field => field.FieldName == "Versioning").Values.Select(value => value.DisplayValue).ShouldBe(["Break0", "Break0"]);

        var scripts = comparison.Fields.Single(field => field.FieldName == "Scripts");
        var script = scripts.Children.Single(field => field.FieldName == "Script [0]");
        script.Children.Single(field => field.FieldName == "Name").Values.Select(value => value.DisplayValue).ShouldBe(["FXScripts:FXResourceCollectionVisuals", "FXScripts:FXResourceCollectionVisuals"]);
        var property = script.Children.Single(field => field.FieldName == "Property [0]");
        property.Children.Single(field => field.FieldName == "Name").Values.Select(value => value.DisplayValue).ShouldBe(["TargetVFX", "TargetVFX"]);
        property.Children.Single(field => field.FieldName == "Value").Values.Select(value => value.DisplayValue).ShouldBe(["BaseVFX", "PatchVFX"]);
    }

    [Fact]
    public void GetRecordComparison_ForPerk_ExpandsRanksBackgroundSkillsAndScripts()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x2CE2C0);
        var staticFormKey = CreateFormKey("Starfield.esm", 0x333);
        var backgroundSkillFormKey = CreateFormKey("Starfield.esm", 0x444);
        var perkRepository = new TestPerkRepository
        {
            Records =
            [
                CreatePerk("Base.esm", formKey, "Chemistry", staticFormKey, backgroundSkillFormKey, "Base description", "Base value"),
                CreatePerk("Patch.esp", formKey, "Chemistry", staticFormKey, backgroundSkillFormKey, "Patch description", "Patch value")
            ]
        };
        var scriptingAdapterRepository = new TestScriptingAdapterRepository
        {
            Records =
            [
                CreateScriptingAdapter("Base.esm", RecordTypeCatalog.Perk.RecordID, formKey, "SkillScript", "RankProperty", "Base script value"),
                CreateScriptingAdapter("Patch.esp", RecordTypeCatalog.Perk.RecordID, formKey, "SkillScript", "RankProperty", "Patch script value")
            ]
        };
        var service = CreateService(
            perkRepository: perkRepository,
            scriptingAdapterRepository: scriptingAdapterRepository);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.Perk.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "Name").Values.Select(value => value.DisplayValue).ShouldBe(["Chemistry", "Chemistry"]);
        var ranks = comparison.Fields.Single(field => field.FieldName == "Ranks");
        var rank = ranks.Children.Single(field => field.FieldName == "Rank [0]");
        rank.Children.Single(field => field.FieldName == "Description").Values.Select(value => value.DisplayValue).ShouldBe(["Base description", "Patch description"]);
        rank.Children.Single(field => field.FieldName == "UnknownStaticFormKey").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000333", "Starfield.esm:00000333"]);
        var effects = rank.Children.Single(field => field.FieldName == "Effects");
        var effect = effects.Children.Single(field => field.FieldName == "Effect [0]");
        effect.Children.Single(field => field.FieldName == "MutagenObjectType").Values.Select(value => value.DisplayValue).ShouldBe(["PerkEntryPointModifyValue", "PerkEntryPointModifyValue"]);
        effect.Children.Single(field => field.FieldName == "EntryPoint").Values.Select(value => value.DisplayValue).ShouldBe(["ModSkillUse", "ModSkillUse"]);
        effect.Children.Single(field => field.FieldName == "Value").Values.Select(value => value.DisplayValue).ShouldBe(["1.5", "2.5"]);
        var backgroundSkills = comparison.Fields.Single(field => field.FieldName == "Background Skills");
        backgroundSkills.Children.Single(field => field.FieldName == "Skill [0]").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000444", "Starfield.esm:00000444"]);
        var scripts = comparison.Fields.Single(field => field.FieldName == "Scripts");
        var script = scripts.Children.Single(field => field.FieldName == "Script [0]");
        script.Children.Single(field => field.FieldName == "Name").Values.Select(value => value.DisplayValue).ShouldBe(["SkillScript", "SkillScript"]);
        var property = script.Children.Single(field => field.FieldName == "Property [0]");
        property.Children.Single(field => field.FieldName == "Value").Values.Select(value => value.DisplayValue).ShouldBe(["Base script value", "Patch script value"]);
    }

    [Fact]
    public void GetRecordComparison_ForStatic_MapsStaticFieldsAndRawPayloads()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x1000);
        var staticRepository = new TestStaticRepository
        {
            Records =
            [
                CreateStatic("Base.esm", formKey, 35, "0, 0, 0", null),
                CreateStatic("Patch.esp", formKey, 45, "0, 0, 0", 1.25)
            ]
        };
        var modelRepository = new TestModelRepository
        {
            Records =
            [
                CreateModel("Base.esm", RecordTypeCatalog.Static.RecordID, formKey, "Meshes\\SetDressing\\Rock01.nif"),
                CreateModel("Patch.esp", RecordTypeCatalog.Static.RecordID, formKey, "Meshes\\SetDressing\\Rock01.nif")
            ]
        };
        var recordKeywordRepository = new TestRecordKeywordRepository
        {
            Records =
            [
                CreateRecordKeyword("Base.esm", RecordTypeCatalog.Static.RecordID, formKey, CreateFormKey("Starfield.esm", 0x555), 0),
                CreateRecordKeyword("Patch.esp", RecordTypeCatalog.Static.RecordID, formKey, CreateFormKey("Starfield.esm", 0x666), 0)
            ]
        };
        var rawRecordPayloadRepository = new TestRawRecordPayloadRepository
        {
            Records =
            [
                CreateRawRecordPayload("Base.esm", formKey, "Model.Data", 0, "Model", "AABB"),
                CreateRawRecordPayload("Patch.esp", formKey, "Model.Data", 0, "Model", "CCDD")
            ]
        };
        var service = CreateService(
            staticRepository: staticRepository,
            modelRepository: modelRepository,
            recordKeywordRepository: recordKeywordRepository,
            rawRecordPayloadRepository: rawRecordPayloadRepository);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.Static.RecordID, formKey);

        comparison.RecordType.ShouldBe(RecordTypeCatalog.Static.RecordID);
        comparison.Fields.Single(field => field.FieldName == "MaxAngle").Values.Select(value => value.DisplayValue).ShouldBe(["35", "45"]);
        comparison.Fields.Single(field => field.FieldName == "ObjectBoundsFirst").Values.Select(value => value.DisplayValue).ShouldBe(["0, 0, 0", "0, 0, 0"]);
        comparison.Fields.Single(field => field.FieldName == "UnknownDNAMFloat").Values.Select(value => value.DisplayValue).ShouldBe(["", "1.25"]);
        var keywords = comparison.Fields.Single(field => field.FieldName == "Keywords");
        keywords.Children.Single(field => field.FieldName == "Keyword [0]").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000555", "Starfield.esm:00000666"]);
        var model = comparison.Fields.Single(field => field.FieldName == "Model");
        model.Children.Single(field => field.FieldName == "File").Values.Select(value => value.DisplayValue).ShouldBe(["Meshes\\SetDressing\\Rock01.nif", "Meshes\\SetDressing\\Rock01.nif"]);
        var rawPayloads = comparison.Fields.Single(field => field.FieldName == "Raw Payloads");
        var modelData = rawPayloads.Children.Single(field => field.FieldName == "Model.Data");
        var modelDataValues = modelData.Children.Single(field => field.FieldName == "Value").Values;
        modelDataValues.Select(value => value.DisplayValue).ShouldBe(["[UNPARSEABLE REFLECTION DATA]", "[UNPARSEABLE REFLECTION DATA]"]);
        modelDataValues.Select(value => value.DetailValue).ShouldBe(["AABB", "CCDD"]);
        modelDataValues.Select(value => value.DisplayKind).ShouldBe([RecordComparisonValueDisplayKind.RawBinaryPayload, RecordComparisonValueDisplayKind.RawBinaryPayload]);
        modelData.Children.Single(field => field.FieldName == "Value").State.ShouldBe(RecordComparisonValueState.Conflict);
    }

    [Fact]
    public void GetRecordComparison_ForContainer_MapsContainerFieldsItemsModelsAndRawPayloads()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x2000);
        var itemFormKey = CreateFormKey("Starfield.esm", 0x333);
        var terminalFormKey = CreateFormKey("Starfield.esm", 0x444);
        var containerRepository = new TestContainerRepository
        {
            Records =
            [
                CreateContainer("Base.esm", formKey, "Storage Crate", terminalFormKey, [CreateContainerItem("Base.esm", formKey, itemFormKey, 0, 2)]),
                CreateContainer("Patch.esp", formKey, "Storage Crate", terminalFormKey, [CreateContainerItem("Patch.esp", formKey, itemFormKey, 0, 4)])
            ]
        };
        var modelRepository = new TestModelRepository
        {
            Records =
            [
                CreateModel("Base.esm", RecordTypeCatalog.Container.RecordID, formKey, "Meshes\\SetDressing\\Container01.nif"),
                CreateModel("Patch.esp", RecordTypeCatalog.Container.RecordID, formKey, "Meshes\\SetDressing\\Container01.nif")
            ]
        };
        var rawRecordPayloadRepository = new TestRawRecordPayloadRepository
        {
            Records =
            [
                CreateRawRecordPayload("Base.esm", RecordTypeCatalog.Container.RecordID, formKey, "BaseFormComponents.AnimationGraphComponent.ANAM", 0, "Byte[]", "AABBCC", "Components.AnimationGraphComponent.ANAM"),
                CreateRawRecordPayload("Patch.esp", RecordTypeCatalog.Container.RecordID, formKey, "BaseFormComponents.AnimationGraphComponent.ANAM", 0, "Byte[]", "DDEEFF", "Components.AnimationGraphComponent.ANAM")
            ]
        };
        var service = CreateService(
            containerRepository: containerRepository,
            modelRepository: modelRepository,
            rawRecordPayloadRepository: rawRecordPayloadRepository);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.Container.RecordID, formKey);

        comparison.RecordType.ShouldBe(RecordTypeCatalog.Container.RecordID);
        comparison.Fields.Single(field => field.FieldName == "Name").Values.Select(value => value.DisplayValue).ShouldBe(["Storage Crate", "Storage Crate"]);
        comparison.Fields.Single(field => field.FieldName == "NativeTerminalFormKey").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000444", "Starfield.esm:00000444"]);
        var items = comparison.Fields.Single(field => field.FieldName == "Items");
        var item = items.Children.Single(field => field.FieldName == "Item [0]");
        item.Children.Single(field => field.FieldName == "Item").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000333", "Starfield.esm:00000333"]);
        item.Children.Single(field => field.FieldName == "Count").Values.Select(value => value.DisplayValue).ShouldBe(["2", "4"]);
        var model = comparison.Fields.Single(field => field.FieldName == "Model");
        model.Children.Single(field => field.FieldName == "File").Values.Select(value => value.DisplayValue).ShouldBe(["Meshes\\SetDressing\\Container01.nif", "Meshes\\SetDressing\\Container01.nif"]);
        var rawPayloads = comparison.Fields.Single(field => field.FieldName == "Raw Payloads");
        var anam = rawPayloads.Children.Single(field => field.FieldName == "BaseFormComponents.AnimationGraphComponent.ANAM");
        var rawValues = anam.Children.Single(field => field.FieldName == "Value").Values;
        rawValues.Select(value => value.DisplayValue).ShouldBe(["[UNPARSEABLE REFLECTION DATA]", "[UNPARSEABLE REFLECTION DATA]"]);
        rawValues.Select(value => value.DetailValue).ShouldBe(["AABBCC", "DDEEFF"]);
        rawValues.Select(value => value.DisplayKind).ShouldBe([RecordComparisonValueDisplayKind.RawBinaryPayload, RecordComparisonValueDisplayKind.RawBinaryPayload]);
    }

    [Fact]
    public void GetRecordComparison_ForConstructibleObject_MapsComponentsConditionsScriptsAndRawPayloads()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x2500);
        var createdObjectFormKey = CreateFormKey("Starfield.esm", 0x111);
        var workbenchKeywordFormKey = CreateFormKey("Starfield.esm", 0x222);
        var componentFormKey = CreateFormKey("Starfield.esm", 0x333);
        var recipeFilterFormKey = CreateFormKey("Starfield.esm", 0x444);
        var constructibleObjectRepository = new TestConstructibleObjectRepository
        {
            Records =
            [
                CreateConstructibleObject("Base.esm", formKey, createdObjectFormKey, workbenchKeywordFormKey, componentFormKey, recipeFilterFormKey, 2),
                CreateConstructibleObject("Patch.esp", formKey, createdObjectFormKey, workbenchKeywordFormKey, componentFormKey, recipeFilterFormKey, 4)
            ]
        };
        var scriptingAdapterRepository = new TestScriptingAdapterRepository
        {
            Records =
            [
                CreateScriptingAdapter("Base.esm", RecordTypeCatalog.ConstructibleObject.RecordID, formKey, "RecipeScript", "Enabled", "True"),
                CreateScriptingAdapter("Patch.esp", RecordTypeCatalog.ConstructibleObject.RecordID, formKey, "RecipeScript", "Enabled", "False")
            ]
        };
        var rawRecordPayloadRepository = new TestRawRecordPayloadRepository
        {
            Records =
            [
                CreateRawRecordPayload("Base.esm", RecordTypeCatalog.ConstructibleObject.RecordID, formKey, "CreatedObjectCounts", 0, "CreatedObjectCounts", "BaseCount"),
                CreateRawRecordPayload("Patch.esp", RecordTypeCatalog.ConstructibleObject.RecordID, formKey, "CreatedObjectCounts", 0, "CreatedObjectCounts", "PatchCount")
            ]
        };
        var service = CreateService(
            constructibleObjectRepository: constructibleObjectRepository,
            scriptingAdapterRepository: scriptingAdapterRepository,
            rawRecordPayloadRepository: rawRecordPayloadRepository);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.ConstructibleObject.RecordID, formKey);

        comparison.RecordType.ShouldBe(RecordTypeCatalog.ConstructibleObject.RecordID);
        comparison.Fields.Single(field => field.FieldName == "CreatedObjectFormKey").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000111", "Starfield.esm:00000111"]);
        comparison.Fields.Single(field => field.FieldName == "WorkbenchKeywordFormKey").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000222", "Starfield.esm:00000222"]);
        comparison.Fields.Single(field => field.FieldName == "AmountProduced").Values.Select(value => value.DisplayValue).ShouldBe(["2", "4"]);
        var components = comparison.Fields.Single(field => field.FieldName == "Components");
        var component = components.Children.Single(field => field.FieldName == "Component [0]");
        component.Children.Single(field => field.FieldName == "ComponentFormKey").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000333", "Starfield.esm:00000333"]);
        component.Children.Single(field => field.FieldName == "Count").Values.Select(value => value.DisplayValue).ShouldBe(["3", "3"]);
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Categories");
        var recipeFilters = comparison.Fields.Single(field => field.FieldName == "RecipeFilters");
        recipeFilters.Children.Single(field => field.FieldName == "RecipeFilter [0]").Children.Single(field => field.FieldName == "RecipeFilterFormKey").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000444", "Starfield.esm:00000444"]);
        var conditions = comparison.Fields.Single(field => field.FieldName == "Conditions");
        var condition = conditions.Children.Single();
        condition.FieldName.ShouldBe("Condition [0]");
        condition.Values.Select(value => value.DisplayValue).ShouldBe(["GetItemCount() EqualTo 2", "GetItemCount() EqualTo 4"]);
        condition.State.ShouldBe(RecordComparisonValueState.Conflict);
        condition.Values.Select(value => value.State).ShouldBe([RecordComparisonValueState.Conflict, RecordComparisonValueState.WinningOverride]);
        condition.Children.ShouldBeEmpty();
        var scripts = comparison.Fields.Single(field => field.FieldName == "Scripts");
        scripts.Children.Single(field => field.FieldName == "Script [0]").Children.Single(field => field.FieldName == "Name").Values.Select(value => value.DisplayValue).ShouldBe(["RecipeScript", "RecipeScript"]);
        var rawPayloads = comparison.Fields.Single(field => field.FieldName == "Raw Payloads");
        var countValues = rawPayloads.Children.Single(field => field.FieldName == "CreatedObjectCounts").Children.Single(field => field.FieldName == "Value").Values;
        countValues.Select(value => value.DisplayValue).ShouldBe(["[UNPARSEABLE REFLECTION DATA]", "[UNPARSEABLE REFLECTION DATA]"]);
        countValues.Select(value => value.DetailValue).ShouldBe(["BaseCount", "PatchCount"]);
    }

    [Fact]
    public void GetRecordComparison_ForConditionForm_MapsVersion2AndConditions()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x246E86);
        var firstParameter = CreateFormKey("Starfield.esm", 0x258350);
        var patchFirstParameter = CreateFormKey("Starfield.esm", 0x2CC9F2);
        var conditionFormRepository = new TestConditionFormRepository
        {
            Records =
            [
                CreateConditionForm("Base.esm", formKey, 1, firstParameter, "1"),
                CreateConditionForm("Patch.esp", formKey, 2, patchFirstParameter, null)
            ]
        };
        var rawRecordPayloadRepository = new TestRawRecordPayloadRepository
        {
            Records =
            [
                CreateRawRecordPayload("Base.esm", RecordTypeCatalog.ConditionForm.RecordID, formKey, "Conditions", 0, "Conditions", "RawCondition")
            ]
        };
        var service = CreateService(conditionFormRepository: conditionFormRepository, rawRecordPayloadRepository: rawRecordPayloadRepository);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.ConditionForm.RecordID, formKey);

        comparison.RecordType.ShouldBe(RecordTypeCatalog.ConditionForm.RecordID);
        comparison.Fields.Single(field => field.FieldName == "Version2").Values.Select(value => value.DisplayValue).ShouldBe(["1", "2"]);
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Raw Payloads");
        var conditions = comparison.Fields.Single(field => field.FieldName == "Conditions");
        var condition = conditions.Children.Single(field => field.FieldName == "Condition [0]");
        condition.Values.Select(value => value.DisplayValue).ShouldBe(["Subject: HasKeyword(Starfield.esm:00258350, 0) EqualTo 1", "Subject: HasKeyword(Starfield.esm:002CC9F2, 0)"]);
        condition.State.ShouldBe(RecordComparisonValueState.Conflict);
        condition.Values.Select(value => value.State).ShouldBe([RecordComparisonValueState.Conflict, RecordComparisonValueState.WinningOverride]);
        condition.Children.ShouldBeEmpty();
    }

    [Fact]
    public void GetRecordComparison_ForConditionForm_PreservesMultipleConditionRules()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x246E86);
        var conditionFormRepository = new TestConditionFormRepository
        {
            Records =
            [
                CreateActorIsPreyConditionForm(formKey)
            ]
        };
        var service = CreateService(conditionFormRepository: conditionFormRepository);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.ConditionForm.RecordID, formKey);

        var conditions = comparison.Fields.Single(field => field.FieldName == "Conditions");
        conditions.Children.Select(field => field.FieldName).ShouldBe([
            "Condition [0]",
            "Condition [1]"
        ]);
        conditions.Children.Select(field => field.Values.Single().DisplayValue).ShouldBe([
            "Subject: HasKeyword(Starfield.esm:00258350, 0) EqualTo 1",
            "Subject: HasKeyword(Starfield.esm:002CC9F2, 0) EqualTo 0"
        ]);
        conditions.Children.Select(field => field.Children.Count).ShouldBe([0, 0]);
    }

    [Fact]
    public void GetRecordComparison_ForBook_MapsBookFieldsAndChildren()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x3000);
        var bookRepository = new TestBookRepository
        {
            Records =
            [
                CreateBook("Base.esm", formKey, "Captain's Log", 100),
                CreateBook("Patch.esp", formKey, "Captain's Log", 150)
            ]
        };
        var modelRepository = new TestModelRepository
        {
            Records =
            [
                CreateModel("Base.esm", RecordTypeCatalog.Book.RecordID, formKey, "Meshes\\SetDressing\\Books\\Book01.nif"),
                CreateModel("Patch.esp", RecordTypeCatalog.Book.RecordID, formKey, "Meshes\\SetDressing\\Books\\Book01.nif")
            ]
        };
        var recordKeywordRepository = new TestRecordKeywordRepository
        {
            Records =
            [
                CreateRecordKeyword("Base.esm", RecordTypeCatalog.Book.RecordID, formKey, CreateFormKey("Starfield.esm", 0x101), 0),
                CreateRecordKeyword("Patch.esp", RecordTypeCatalog.Book.RecordID, formKey, CreateFormKey("Starfield.esm", 0x101), 0)
            ]
        };
        var recordSoundRepository = new TestRecordSoundRepository
        {
            Records =
            [
                CreateRecordSound("Base.esm", RecordTypeCatalog.Book.RecordID, formKey, "PickupSound", 0, "pickup"),
                CreateRecordSound("Patch.esp", RecordTypeCatalog.Book.RecordID, formKey, "PickupSound", 0, "pickup")
            ]
        };
        var service = CreateService(
            bookRepository: bookRepository,
            modelRepository: modelRepository,
            recordKeywordRepository: recordKeywordRepository,
            recordSoundRepository: recordSoundRepository);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.Book.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "Name").Values.Select(value => value.DisplayValue).ShouldBe(["Captain's Log", "Captain's Log"]);
        comparison.Fields.Single(field => field.FieldName == "Value").Values.Select(value => value.DisplayValue).ShouldBe(["100", "150"]);
        comparison.Fields.Single(field => field.FieldName == "InventoryTransformFormKey").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000999", "Starfield.esm:00000999"]);
        comparison.Fields.Single(field => field.FieldName == "Text").Values.Select(value => value.DisplayValue).ShouldBe(["Base text", "Patch text"]);
        comparison.Fields.Single(field => field.FieldName == "TeachesType").Values.Select(value => value.DisplayValue).ShouldBe(["Skill", "Skill"]);
    }

    [Fact]
    public void GetRecordComparison_ForDoor_MapsDoorFieldsAndChildren()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x4000);
        var nativeTerminalFormKey = CreateFormKey("Starfield.esm", 0x555);
        var doorRepository = new TestDoorRepository
        {
            Records =
            [
                CreateDoor("Base.esm", formKey, "Airlock", nativeTerminalFormKey, "Both"),
                CreateDoor("Patch.esp", formKey, "Airlock", nativeTerminalFormKey, "Positive")
            ]
        };
        var service = CreateService(doorRepository: doorRepository);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.Door.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "Name").Values.Select(value => value.DisplayValue).ShouldBe(["Airlock", "Airlock"]);
        comparison.Fields.Single(field => field.FieldName == "NativeTerminalFormKey").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000555", "Starfield.esm:00000555"]);
        comparison.Fields.Single(field => field.FieldName == "FacingAxisOverride").Values.Select(value => value.DisplayValue).ShouldBe(["Both", "Positive"]);
    }

    [Fact]
    public void GetRecordComparison_ForTerminal_MapsTerminalFieldsAndMarkerParameters()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x5000);
        var terminalRepository = new TestTerminalRepository
        {
            Records =
            [
                CreateTerminal("Base.esm", formKey, "Kiosk", "0x1", "BaseEntry"),
                CreateTerminal("Patch.esp", formKey, "Kiosk", "0x2", "PatchEntry")
            ]
        };
        var service = CreateService(terminalRepository: terminalRepository);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.Terminal.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "Name").Values.Select(value => value.DisplayValue).ShouldBe(["Kiosk", "Kiosk"]);
        comparison.Fields.Single(field => field.FieldName == "MarkerFlags").Values.Select(value => value.DisplayValue).ShouldBe(["1", "2"]);
        var markerParameters = comparison.Fields.Single(field => field.FieldName == "Marker Parameters");
        var firstParameter = markerParameters.Children.Single(field => field.FieldName == "Marker Parameter [0]");
        firstParameter.Children.Single(field => field.FieldName == "EntryTypes").Values.Select(value => value.DisplayValue).ShouldBe(["BaseEntry", "PatchEntry"]);
    }

    [Fact]
    public void GetRecordComparison_ForSingleColumn_KeepsValuesNeutral()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x321);
        var globalRepository = new TestGlobalRepository
        {
            Records =
            [
                CreateGlobal("Base.esm", formKey, "MyGlobal", 1.5)
            ]
        };
        var service = CreateService(globalRepository: globalRepository);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.Global.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "Data").State.ShouldBe(RecordComparisonValueState.Neutral);
        comparison.Fields.Single(field => field.FieldName == "Data").Values.Single().State.ShouldBe(RecordComparisonValueState.Neutral);
    }

    [Fact]
    public void GetRecordComparison_ForNonComparableCommonFields_KeepsValuesNeutral()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x654);
        var globalRepository = new TestGlobalRepository
        {
            Records =
            [
                CreateGlobal("Base.esm", formKey, "MyGlobal", 1.5),
                CreateGlobal("Patch.esp", formKey, "MyGlobal", 2.5)
            ]
        };
        var service = CreateService(globalRepository: globalRepository);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.Global.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "FormVersion").State.ShouldBe(RecordComparisonValueState.Neutral);
        comparison.Fields.Single(field => field.FieldName == "MajorRecordFlags").State.ShouldBe(RecordComparisonValueState.Neutral);
    }

    [Fact]
    public void GetRecordComparison_ForUnsupportedRecordType_ReturnsEmptyComparison()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x999);
        var service = CreateService();

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, "ARMO", formKey);

        comparison.RecordType.ShouldBe("ARMO");
        comparison.FormKey.ShouldBeSameAs(formKey);
        comparison.Columns.ShouldBeEmpty();
        comparison.Fields.ShouldBeEmpty();
    }

    private static RecordComparisonService CreateService(
        TestFormListRepository? formListRepository = null,
        TestGameSettingRepository? gameSettingRepository = null,
        TestGlobalRepository? globalRepository = null,
        TestClassRepository? classRepository = null,
        TestFactionRepository? factionRepository = null,
        TestMiscObjectRepository? miscObjectRepository = null,
        TestKeywordRepository? keywordRepository = null,
        TestActorValueInformationRepository? actorValueInformationRepository = null,
        TestNPCRepository? npcRepository = null,
        TestMagicEffectRepository? magicEffectRepository = null,
        TestPerkRepository? perkRepository = null,
        TestStaticRepository? staticRepository = null,
        TestBookRepository? bookRepository = null,
        TestDoorRepository? doorRepository = null,
        TestContainerRepository? containerRepository = null,
        TestConstructibleObjectRepository? constructibleObjectRepository = null,
        TestConditionFormRepository? conditionFormRepository = null,
        TestTerminalRepository? terminalRepository = null,
        TestModelRepository? modelRepository = null,
        TestRecordKeywordRepository? recordKeywordRepository = null,
        TestRecordSoundRepository? recordSoundRepository = null,
        TestScriptingAdapterRepository? scriptingAdapterRepository = null,
        TestRawRecordPayloadRepository? rawRecordPayloadRepository = null,
        TestRecordLocalizedStringRepository? recordLocalizedStringRepository = null,
        TestGameSelectionService? gameSelectionService = null)
    {
        return new RecordComparisonService(
            formListRepository ?? new TestFormListRepository(),
            gameSettingRepository ?? new TestGameSettingRepository(),
            globalRepository ?? new TestGlobalRepository(),
            classRepository ?? new TestClassRepository(),
            factionRepository ?? new TestFactionRepository(),
            miscObjectRepository ?? new TestMiscObjectRepository(),
            keywordRepository ?? new TestKeywordRepository(),
            actorValueInformationRepository ?? new TestActorValueInformationRepository(),
            npcRepository ?? new TestNPCRepository(),
            magicEffectRepository ?? new TestMagicEffectRepository(),
            perkRepository ?? new TestPerkRepository(),
            staticRepository ?? new TestStaticRepository(),
            bookRepository ?? new TestBookRepository(),
            doorRepository ?? new TestDoorRepository(),
            containerRepository ?? new TestContainerRepository(),
            constructibleObjectRepository ?? new TestConstructibleObjectRepository(),
            conditionFormRepository ?? new TestConditionFormRepository(),
            terminalRepository ?? new TestTerminalRepository(),
            modelRepository ?? new TestModelRepository(),
            recordKeywordRepository ?? new TestRecordKeywordRepository(),
            recordSoundRepository ?? new TestRecordSoundRepository(),
            scriptingAdapterRepository ?? new TestScriptingAdapterRepository(),
            rawRecordPayloadRepository ?? new TestRawRecordPayloadRepository(),
            recordLocalizedStringRepository ?? new TestRecordLocalizedStringRepository(),
            gameSelectionService ?? new TestGameSelectionService());
    }

    private static FormKeyDTO CreateFormKey(string fileName, uint id)
    {
        return new FormKeyDTO
        {
            ModKey = CreateModKey(fileName),
            Id = id
        };
    }

    private static ModKeyDTO CreateModKey(string fileName)
    {
        return new ModKeyDTO
        {
            Name = Path.GetFileNameWithoutExtension(fileName),
            Type = 1,
            FileName = fileName
        };
    }

    private static TranslatedStringDTO Text(string value)
    {
        return new TranslatedStringDTO
        {
            Strings =
            [
                new TranslatedStringValueDTO
                {
                    Language = "English",
                    String = value
                }
            ]
        };
    }

    private static GlobalDTO CreateGlobal(string fileName, FormKeyDTO formKey, string editorID, double data)
    {
        return new GlobalDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            FormKey = formKey,
            EditorID = editorID,
            FormVersion = 1,
            MajorRecordFlags = 2,
            ImportedAtUTC = DateTime.UtcNow,
            Data = data
        };
    }

    private static GameSettingDTO CreateGameSetting(
        string fileName,
        FormKeyDTO formKey,
        string editorID,
        string settingType,
        string data,
        double? numericData = null)
    {
        return new GameSettingDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            FormKey = formKey,
            EditorID = editorID,
            FormVersion = 1,
            MajorRecordFlags = 2,
            ImportedAtUTC = DateTime.UtcNow,
            SettingType = settingType,
            Data = data,
            NumericData = numericData
        };
    }

    private static LocalizedStringDTO CreateLocalizedString(
        string fileName,
        FormKeyDTO formKey,
        string sourceField,
        string language,
        string value)
    {
        return new LocalizedStringDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            RecordType = RecordTypeCatalog.GameSetting.RecordID,
            FormKey = formKey,
            SourceField = sourceField,
            Language = language,
            Value = value,
            ImportedAtUTC = DateTime.UtcNow
        };
    }

    private static FormListDTO CreateFormList(string fileName, FormKeyDTO formKey, IList<FormListItemDTO> items)
    {
        return new FormListDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            FormKey = formKey,
            EditorID = "MyFormList",
            FormVersion = 1,
            MajorRecordFlags = 2,
            ImportedAtUTC = DateTime.UtcNow,
            Items = items
        };
    }

    private static FormListItemDTO CreateFormListItem(string fileName, FormKeyDTO formKey, FormKeyDTO itemFormKey, int itemIndex)
    {
        return new FormListItemDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            FormKey = formKey,
            ItemFormKey = itemFormKey,
            ItemIndex = itemIndex,
            ImportedAtUTC = DateTime.UtcNow
        };
    }

    private static MiscObjectDTO CreateMiscObject(string fileName, FormKeyDTO formKey, string name, int value, float weight, FormKeyDTO? featuredItemMessageFormKey)
    {
        return new MiscObjectDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            FormKey = formKey,
            EditorID = "MyMiscObject",
            FormVersion = 1,
            MajorRecordFlags = 2,
            ImportedAtUTC = DateTime.UtcNow,
            Name = Text(name),
            ShortName = Text("ShortName"),
            Value = value,
            Weight = weight,
            DirtinessScale = 1,
            FeaturedItemMessageFormKey = featuredItemMessageFormKey,
            Flag = "None"
        };
    }

    private static ModelDTO CreateModel(string fileName, FormKeyDTO formKey, string file)
    {
        return CreateModel(fileName, RecordTypeCatalog.MiscObject.RecordID, formKey, file);
    }

    private static ModelDTO CreateModel(string fileName, string recordType, FormKeyDTO formKey, string file)
    {
        return new ModelDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            RecordType = recordType,
            FormKey = formKey,
            ModelSlot = "Model",
            ModelGender = string.Empty,
            File = file,
            ImportedAtUTC = DateTime.UtcNow
        };
    }

    private static StaticDTO CreateStatic(string fileName, FormKeyDTO formKey, double maxAngle, string objectBoundsFirst, double? unknownDNAMFloat)
    {
        return new StaticDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            FormKey = formKey,
            EditorID = "MyStatic",
            FormVersion = 1,
            MajorRecordFlags = 2,
            ImportedAtUTC = DateTime.UtcNow,
            ObjectBoundsFirst = objectBoundsFirst,
            ObjectBoundsSecond = "1, 1, 1",
            MaxAngle = maxAngle,
            UnknownDNAMFloat = unknownDNAMFloat,
            DNAMDataTypeState = "Enabled"
        };
    }

    private static BookDTO CreateBook(string fileName, FormKeyDTO formKey, string name, int value)
    {
        return new BookDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            FormKey = formKey,
            EditorID = "MyBook",
            FormVersion = 1,
            MajorRecordFlags = 2,
            ImportedAtUTC = DateTime.UtcNow,
            Version2 = 3,
            ObjectBoundsFirst = "0, 0, 0",
            ObjectBoundsSecond = "1, 1, 1",
            InventoryTransformFormKey = CreateFormKey("Starfield.esm", 0x999),
            Xalg = 7,
            Name = Text(name),
            Text = Text(fileName.StartsWith("Base", StringComparison.Ordinal) ? "Base text" : "Patch text"),
            Value = value,
            Weight = 1.25f,
            Flags = "Takeable",
            TeachesType = "Skill",
            TeachesRawContent = "Piloting",
            DataSlateType = "None",
            Description = Text("Book description"),
            DataSlateHeaderLeft = Text("Left"),
            DataSlateHeaderRight = Text("Right")
        };
    }

    private static DoorDTO CreateDoor(string fileName, FormKeyDTO formKey, string name, FormKeyDTO? nativeTerminalFormKey, string facingAxisOverride)
    {
        return new DoorDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            FormKey = formKey,
            EditorID = "MyDoor",
            FormVersion = 1,
            MajorRecordFlags = 2,
            ImportedAtUTC = DateTime.UtcNow,
            Version2 = 1,
            ObjectBoundsFirst = "0, 0, 0",
            ObjectBoundsSecond = "1, 1, 1",
            Name = Text(name),
            Flags = "Automatic",
            NativeTerminalFormKey = nativeTerminalFormKey,
            SoundLevel = "Normal",
            FacingAxisOverride = facingAxisOverride
        };
    }

    private static ContainerDTO CreateContainer(string fileName, FormKeyDTO formKey, string name, FormKeyDTO? nativeTerminalFormKey, IList<ContainerItemDTO> items)
    {
        return new ContainerDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            FormKey = formKey,
            EditorID = "MyContainer",
            FormVersion = 1,
            MajorRecordFlags = 2,
            ImportedAtUTC = DateTime.UtcNow,
            Version2 = 15,
            ObjectBoundsFirst = "0, 0, 0",
            ObjectBoundsSecond = "1, 1, 1",
            Name = Text(name),
            Flags = "Respawns",
            NativeTerminalFormKey = nativeTerminalFormKey,
            Items = items
        };
    }

    private static TerminalDTO CreateTerminal(string fileName, FormKeyDTO formKey, string name, string markerFlags, string entryTypes)
    {
        return new TerminalDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            FormKey = formKey,
            EditorID = "MyTerminal",
            FormVersion = 1,
            MajorRecordFlags = 2,
            ImportedAtUTC = DateTime.UtcNow,
            Version2 = 4,
            ObjectBoundsFirst = "0, 0, 0",
            ObjectBoundsSecond = "1, 1, 1",
            MenuFormKey = CreateFormKey("Starfield.esm", 0x111),
            Background = "BackgroundA",
            Name = Text(name),
            Pnam = "PNAM",
            Fnam = "FNAM",
            Jnam = "JNAM",
            MarkerFlags = long.Parse(markerFlags.Replace("0x", string.Empty), System.Globalization.NumberStyles.HexNumber),
            Gnam = "GNAM",
            WorkbenchData = "WorkbenchData",
            FurnitureTemplateFormKey = CreateFormKey("Starfield.esm", 0x222),
            MarkerModel = "MarkerModel.nif",
            MarkerParameters =
            [
                new TerminalMarkerParameterDTO
                {
                    Game = SupportedGame.Starfield,
                    ModKey = CreateModKey(fileName),
                    FormKey = formKey,
                    ParameterIndex = 0,
                    Offset = "0,0,0",
                    EntryTypes = entryTypes,
                    ExitTypes = "ExitType",
                    ImportedAtUTC = DateTime.UtcNow
                }
            ]
        };
    }

    private static ConstructibleObjectDTO CreateConstructibleObject(
        string fileName,
        FormKeyDTO formKey,
        FormKeyDTO createdObjectFormKey,
        FormKeyDTO workbenchKeywordFormKey,
        FormKeyDTO componentFormKey,
        FormKeyDTO recipeFilterFormKey,
        int amountProduced)
    {
        return new ConstructibleObjectDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            FormKey = formKey,
            EditorID = "MyRecipe",
            FormVersion = 1,
            MajorRecordFlags = 2,
            ImportedAtUTC = DateTime.UtcNow,
            Version2 = 2,
            Description = Text("Recipe description"),
            CreatedObjectFormKey = createdObjectFormKey,
            WorkbenchKeywordFormKey = workbenchKeywordFormKey,
            AmountProduced = amountProduced,
            LearnMethod = "DefaultOrConditions",
            Flags = "None",
            Components =
            {
                new ConstructibleObjectComponentDTO
                {
                    Game = SupportedGame.Starfield,
                    ModKey = CreateModKey(fileName),
                    FormKey = formKey,
                    ComponentFormKey = componentFormKey,
                    ComponentIndex = 0,
                    Count = 3,
                    ImportedAtUTC = DateTime.UtcNow
                }
            },
            RecipeFilters =
            {
                new ConstructibleObjectRecipeFilterDTO
                {
                    Game = SupportedGame.Starfield,
                    ModKey = CreateModKey(fileName),
                    FormKey = formKey,
                    RecipeFilterFormKey = recipeFilterFormKey,
                    RecipeFilterIndex = 0,
                    ImportedAtUTC = DateTime.UtcNow
                }
            },
            Conditions =
            {
                new ConditionFormConditionDTO
                {
                    Game = SupportedGame.Starfield,
                    ModKey = CreateModKey(fileName),
                    FormKey = formKey,
                    ConditionIndex = 0,
                    MutagenObjectType = "ConditionFloat",
                    DataMutagenObjectType = "GetItemCountConditionData",
                    CompareOperator = "EqualTo",
                    ComparisonValue = amountProduced.ToString(),
                    ImportedAtUTC = DateTime.UtcNow
                }
            }
        };
    }

    private static ConditionFormDTO CreateConditionForm(string fileName, FormKeyDTO formKey, int version2, FormKeyDTO firstParameter, string? comparisonValue)
    {
        return new ConditionFormDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            FormKey = formKey,
            EditorID = "MyConditionForm",
            FormVersion = 1,
            MajorRecordFlags = 2,
            ImportedAtUTC = DateTime.UtcNow,
            Version2 = version2,
            Conditions =
            {
                new ConditionFormConditionDTO
                {
                    Game = SupportedGame.Starfield,
                    ModKey = CreateModKey(fileName),
                    FormKey = formKey,
                    ConditionIndex = 0,
                    MutagenObjectType = "ConditionFloat",
                    DataMutagenObjectType = "HasKeywordConditionData",
                    CompareOperator = "EqualTo",
                    ComparisonValue = comparisonValue,
                    ImportedAtUTC = DateTime.UtcNow,
                    Parameters =
                    {
                        new ConditionFormConditionParameterDTO
                        {
                            Game = SupportedGame.Starfield,
                            ModKey = CreateModKey(fileName),
                            FormKey = formKey,
                            ConditionIndex = 0,
                            ParameterName = "FirstParameter",
                            ParameterValue = FormatFormKey(firstParameter),
                            ParameterFormKey = firstParameter,
                            ImportedAtUTC = DateTime.UtcNow
                        },
                        new ConditionFormConditionParameterDTO
                        {
                            Game = SupportedGame.Starfield,
                            ModKey = CreateModKey(fileName),
                            FormKey = formKey,
                            ConditionIndex = 0,
                            ParameterName = "RunOnType",
                            ParameterValue = "Subject",
                            ImportedAtUTC = DateTime.UtcNow
                        },
                        new ConditionFormConditionParameterDTO
                        {
                            Game = SupportedGame.Starfield,
                            ModKey = CreateModKey(fileName),
                            FormKey = formKey,
                            ConditionIndex = 0,
                            ParameterName = "SecondParameter",
                            ParameterValue = "0",
                            ImportedAtUTC = DateTime.UtcNow
                        }
                    }
                }
            }
        };
    }

    private static ConditionFormDTO CreateActorIsPreyConditionForm(FormKeyDTO formKey)
    {
        return new ConditionFormDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey("Starfield.esm"),
            FormKey = formKey,
            EditorID = "ActorIsPrey",
            FormVersion = 581,
            MajorRecordFlags = 0,
            ImportedAtUTC = DateTime.UtcNow,
            Version2 = 1,
            Conditions =
            {
                CreateCondition("Starfield.esm", formKey, 0, CreateFormKey("Starfield.esm", 0x258350), "1"),
                CreateCondition("Starfield.esm", formKey, 1, CreateFormKey("Starfield.esm", 0x2CC9F2), "0")
            }
        };
    }

    private static ConditionFormConditionDTO CreateCondition(string fileName, FormKeyDTO formKey, int conditionIndex, FormKeyDTO firstParameter, string comparisonValue)
    {
        return new ConditionFormConditionDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            FormKey = formKey,
            ConditionIndex = conditionIndex,
            MutagenObjectType = "ConditionFloat",
            DataMutagenObjectType = "HasKeywordConditionData",
            CompareOperator = "EqualTo",
            ComparisonValue = comparisonValue,
            ImportedAtUTC = DateTime.UtcNow,
            Parameters =
            {
                new ConditionFormConditionParameterDTO
                {
                    Game = SupportedGame.Starfield,
                    ModKey = CreateModKey(fileName),
                    FormKey = formKey,
                    ConditionIndex = conditionIndex,
                    ParameterName = "RunOnType",
                    ParameterValue = "Subject",
                    ImportedAtUTC = DateTime.UtcNow
                },
                new ConditionFormConditionParameterDTO
                {
                    Game = SupportedGame.Starfield,
                    ModKey = CreateModKey(fileName),
                    FormKey = formKey,
                    ConditionIndex = conditionIndex,
                    ParameterName = "FirstParameter",
                    ParameterValue = FormatFormKey(firstParameter),
                    ParameterFormKey = firstParameter,
                    ImportedAtUTC = DateTime.UtcNow
                },
                new ConditionFormConditionParameterDTO
                {
                    Game = SupportedGame.Starfield,
                    ModKey = CreateModKey(fileName),
                    FormKey = formKey,
                    ConditionIndex = conditionIndex,
                    ParameterName = "SecondParameter",
                    ParameterValue = "0",
                    ImportedAtUTC = DateTime.UtcNow
                }
            }
        };
    }

    private static ContainerItemDTO CreateContainerItem(string fileName, FormKeyDTO formKey, FormKeyDTO itemFormKey, int itemIndex, int count)
    {
        return new ContainerItemDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            FormKey = formKey,
            ItemFormKey = itemFormKey,
            ItemIndex = itemIndex,
            Count = count,
            ImportedAtUTC = DateTime.UtcNow
        };
    }

    private static RawRecordPayloadDTO CreateRawRecordPayload(string fileName, FormKeyDTO formKey, string payloadSlot, int payloadIndex, string payloadType, string payloadValue)
    {
        return CreateRawRecordPayload(fileName, RecordTypeCatalog.Static.RecordID, formKey, payloadSlot, payloadIndex, payloadType, payloadValue);
    }

    private static string FormatFormKey(FormKeyDTO formKey)
    {
        return $"{formKey.ModKey.FileName}:{formKey.Id:X8}";
    }

    private static RawRecordPayloadDTO CreateRawRecordPayload(string fileName, string recordType, FormKeyDTO formKey, string payloadSlot, int payloadIndex, string payloadType, string payloadValue, string? sourcePath = null)
    {
        return new RawRecordPayloadDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            RecordType = recordType,
            FormKey = formKey,
            PayloadSlot = payloadSlot,
            PayloadIndex = payloadIndex,
            PayloadType = payloadType,
            SourcePath = sourcePath ?? payloadSlot,
            PayloadValue = payloadValue,
            ImportedAtUTC = DateTime.UtcNow
        };
    }

    private static ScriptingAdapterDTO CreateScriptingAdapter(string fileName, FormKeyDTO formKey, string name, string propertyName, string propertyValue)
    {
        return CreateScriptingAdapter(fileName, RecordTypeCatalog.MiscObject.RecordID, formKey, name, propertyName, propertyValue);
    }

    private static ScriptingAdapterDTO CreateScriptingAdapter(string fileName, string recordType, FormKeyDTO formKey, string name, string propertyName, string propertyValue)
    {
        return new ScriptingAdapterDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            RecordType = recordType,
            FormKey = formKey,
            Name = name,
            ScriptIndex = 0,
            ImportedAtUTC = DateTime.UtcNow,
            Properties =
            {
                new ScriptingAdapterPropertyDTO
                {
                    Game = SupportedGame.Starfield,
                    ModKey = CreateModKey(fileName),
                    RecordType = recordType,
                    FormKey = formKey,
                    ScriptingAdapterName = name,
                    PropertyIndex = 0,
                    Name = propertyName,
                    MutagenObjectType = "String",
                    DataString = propertyValue,
                    ImportedAtUTC = DateTime.UtcNow
                }
            }
        };
    }

    private static RecordKeywordDTO CreateRecordKeyword(string fileName, string recordType, FormKeyDTO formKey, FormKeyDTO keywordFormKey, int keywordIndex)
    {
        return new RecordKeywordDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            RecordType = recordType,
            FormKey = formKey,
            KeywordIndex = keywordIndex,
            KeywordFormKey = keywordFormKey,
            ImportedAtUTC = DateTime.UtcNow
        };
    }

    private static RecordSoundDTO CreateRecordSound(string fileName, string recordType, FormKeyDTO formKey, string soundSlot, int soundIndex, string start, string? versioning = null, string? unknown = null)
    {
        return new RecordSoundDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            RecordType = recordType,
            FormKey = formKey,
            SoundSlot = soundSlot,
            SoundIndex = soundIndex,
            Start = start,
            Versioning = versioning,
            Unknown = unknown,
            ImportedAtUTC = DateTime.UtcNow
        };
    }

    private static MagicEffectDTO CreateMagicEffect(string fileName, FormKeyDTO formKey, string name, string archetype, int unknownInt2)
    {
        return new MagicEffectDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            FormKey = formKey,
            EditorID = "MyMagicEffect",
            FormVersion = 1,
            MajorRecordFlags = 2,
            ImportedAtUTC = DateTime.UtcNow,
            Name = Text(name),
            Archetype = archetype,
            UnknownInt2 = unknownInt2,
            Flags = "None"
        };
    }

    private static PerkDTO CreatePerk(string fileName, FormKeyDTO formKey, string name, FormKeyDTO unknownStaticFormKey, FormKeyDTO backgroundSkillFormKey, string rankDescription, string buttonLabel)
    {
        return new PerkDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            FormKey = formKey,
            EditorID = "MyPerk",
            FormVersion = 1,
            MajorRecordFlags = 2,
            ImportedAtUTC = DateTime.UtcNow,
            Name = Text(name),
            Description = Text("Perk description"),
            Flags = "PcPlayable",
            SkillGroup = "Expert",
            CrewAssignment = "None",
            PerkIcon = "Patch_Science_Chemistry",
            Category = "Science",
            MajorFlags = "0",
            Ranks =
            {
                new PerkRankDTO
                {
                    ModKey = CreateModKey(fileName),
                    FormKey = formKey,
                    RankIndex = 0,
                    Description = Text(rankDescription),
                    UnknownStaticFormKey = unknownStaticFormKey,
                    ConditionCount = 1,
                    ActivityCount = 2,
                    ImportedAtUTC = DateTime.UtcNow,
                    Effects =
                    {
                        new PerkRankEffectDTO
                        {
                            ModKey = CreateModKey(fileName),
                            FormKey = formKey,
                            RankIndex = 0,
                            EffectIndex = 0,
                            MutagenObjectType = "PerkEntryPointModifyValue",
                            Rank = 1,
                            Priority = 10,
                            PerkEntryId = 20,
                            Flags = "None",
                            ButtonLabel = Text(buttonLabel),
                            ConditionCount = 3,
                            EntryPoint = "ModSkillUse",
                            PerkConditionTabCount = 4,
                            Modification = "Add",
                            Value = fileName.StartsWith("Base", StringComparison.Ordinal) ? 1.5 : 2.5,
                            ImportedAtUTC = DateTime.UtcNow
                        }
                    }
                }
            },
            BackgroundSkills =
            {
                new PerkBackgroundSkillDTO
                {
                    ModKey = CreateModKey(fileName),
                    FormKey = formKey,
                    SkillFormKey = backgroundSkillFormKey,
                    SkillIndex = 0,
                    ImportedAtUTC = DateTime.UtcNow
                }
            }
        };
    }

    private sealed class TestFormListRepository : IFormListRepository
    {
        public IReadOnlyList<FormListDTO> Records { get; set; } = [];

        public IReadOnlyList<RecordTreeEntryDTO> GetRecordTreeEntriesByPlugin(SupportedGame game, ModKeyDTO modKey)
        {
            return [];
        }

        public IReadOnlyDictionary<string, int> GetRecordPluginCountsByGame(SupportedGame game)
        {
            return new Dictionary<string, int>();
        }

        public IReadOnlyList<FormListDTO> GetByFormKey(SupportedGame game, FormKeyDTO formKey)
        {
            return Records;
        }

        public void Save(FormListDTO dto)
        { }

        public void DeleteStaleByPlugin(SupportedGame game, ModKeyDTO modKey, DateTime importedAtUTC)
        { }
    }

    private sealed class TestGameSettingRepository : IGameSettingRepository
    {
        public IReadOnlyList<GameSettingDTO> Records { get; set; } = [];

        public IReadOnlyList<RecordTreeEntryDTO> GetRecordTreeEntriesByPlugin(SupportedGame game, ModKeyDTO modKey)
        {
            return [];
        }

        public IReadOnlyDictionary<string, int> GetRecordPluginCountsByGame(SupportedGame game)
        {
            return new Dictionary<string, int>();
        }

        public IReadOnlyList<GameSettingDTO> GetByFormKey(SupportedGame game, FormKeyDTO formKey)
        {
            return Records;
        }

        public void Save(GameSettingDTO dto)
        { }

        public void DeleteStaleByPlugin(SupportedGame game, ModKeyDTO modKey, DateTime importedAtUTC)
        { }
    }

    private sealed class TestGlobalRepository : IGlobalRepository
    {
        public IReadOnlyList<GlobalDTO> Records { get; set; } = [];

        public IReadOnlyList<RecordTreeEntryDTO> GetRecordTreeEntriesByPlugin(SupportedGame game, ModKeyDTO modKey)
        {
            return [];
        }

        public IReadOnlyDictionary<string, int> GetRecordPluginCountsByGame(SupportedGame game)
        {
            return new Dictionary<string, int>();
        }

        public IReadOnlyList<GlobalDTO> GetByFormKey(SupportedGame game, FormKeyDTO formKey)
        {
            return Records;
        }

        public void Save(GlobalDTO dto)
        { }

        public void DeleteStaleByPlugin(SupportedGame game, ModKeyDTO modKey, DateTime importedAtUTC)
        { }
    }

    private sealed class TestClassRepository : IClassRepository
    {
        public string RecordType => RecordTypeCatalog.Class.RecordID;

        public IReadOnlyList<ClassDTO> Records { get; set; } = [];

        public IReadOnlyList<RecordTreeEntryDTO> GetRecordTreeEntriesByPlugin(SupportedGame game, ModKeyDTO modKey)
        {
            return [];
        }

        public IReadOnlyDictionary<string, int> GetRecordPluginCountsByGame(SupportedGame game)
        {
            return new Dictionary<string, int>();
        }

        public IReadOnlyList<ClassDTO> GetByFormKey(SupportedGame game, FormKeyDTO formKey)
        {
            return Records;
        }

        public void Save(ClassDTO dto)
        { }

        public void DeleteStaleByPlugin(SupportedGame game, ModKeyDTO modKey, DateTime importedAtUTC)
        { }
    }

    private sealed class TestFactionRepository : IFactionRepository
    {
        public string RecordType => RecordTypeCatalog.Faction.RecordID;

        public IReadOnlyList<FactionDTO> Records { get; set; } = [];

        public IReadOnlyList<RecordTreeEntryDTO> GetRecordTreeEntriesByPlugin(SupportedGame game, ModKeyDTO modKey)
        {
            return [];
        }

        public IReadOnlyDictionary<string, int> GetRecordPluginCountsByGame(SupportedGame game)
        {
            return new Dictionary<string, int>();
        }

        public IReadOnlyList<FactionDTO> GetByFormKey(SupportedGame game, FormKeyDTO formKey)
        {
            return Records;
        }

        public void Save(FactionDTO dto)
        { }

        public void DeleteStaleByPlugin(SupportedGame game, ModKeyDTO modKey, DateTime importedAtUTC)
        { }
    }

    private sealed class TestMiscObjectRepository : IMiscObjectRepository
    {
        public string RecordType => RecordTypeCatalog.MiscObject.RecordID;

        public IReadOnlyList<MiscObjectDTO> Records { get; set; } = [];

        public IReadOnlyList<RecordTreeEntryDTO> GetRecordTreeEntriesByPlugin(SupportedGame game, ModKeyDTO modKey)
        {
            return [];
        }

        public IReadOnlyDictionary<string, int> GetRecordPluginCountsByGame(SupportedGame game)
        {
            return new Dictionary<string, int>();
        }

        public IReadOnlyList<MiscObjectDTO> GetByFormKey(SupportedGame game, FormKeyDTO formKey)
        {
            return Records;
        }

        public void Save(MiscObjectDTO dto)
        { }

        public void DeleteStaleByPlugin(SupportedGame game, ModKeyDTO modKey, DateTime importedAtUTC)
        { }
    }

    private sealed class TestKeywordRepository : IKeywordRepository
    {
        public string RecordType => RecordTypeCatalog.Keyword.RecordID;

        public IReadOnlyList<KeywordDTO> Records { get; set; } = [];

        public IReadOnlyList<RecordTreeEntryDTO> GetRecordTreeEntriesByPlugin(SupportedGame game, ModKeyDTO modKey)
        {
            return [];
        }

        public IReadOnlyDictionary<string, int> GetRecordPluginCountsByGame(SupportedGame game)
        {
            return new Dictionary<string, int>();
        }

        public IReadOnlyList<KeywordDTO> GetByFormKey(SupportedGame game, FormKeyDTO formKey)
        {
            return Records;
        }

        public void Save(KeywordDTO dto)
        { }

        public void DeleteStaleByPlugin(SupportedGame game, ModKeyDTO modKey, DateTime importedAtUTC)
        { }
    }

    private sealed class TestActorValueInformationRepository : IActorValueInformationRepository
    {
        public string RecordType => RecordTypeCatalog.ActorValueInformation.RecordID;

        public IReadOnlyList<ActorValueInformationDTO> Records { get; set; } = [];

        public IReadOnlyList<RecordTreeEntryDTO> GetRecordTreeEntriesByPlugin(SupportedGame game, ModKeyDTO modKey)
        {
            return [];
        }

        public IReadOnlyDictionary<string, int> GetRecordPluginCountsByGame(SupportedGame game)
        {
            return new Dictionary<string, int>();
        }

        public IReadOnlyList<ActorValueInformationDTO> GetByFormKey(SupportedGame game, FormKeyDTO formKey)
        {
            return Records;
        }

        public void Save(ActorValueInformationDTO dto)
        { }

        public void DeleteStaleByPlugin(SupportedGame game, ModKeyDTO modKey, DateTime importedAtUTC)
        { }
    }

    private sealed class TestNPCRepository : INPCRepository
    {
        public string RecordType => RecordTypeCatalog.NPC.RecordID;

        public IReadOnlyList<NPCDTO> Records { get; set; } = [];

        public IReadOnlyList<RecordTreeEntryDTO> GetRecordTreeEntriesByPlugin(SupportedGame game, ModKeyDTO modKey)
        {
            return [];
        }

        public IReadOnlyDictionary<string, int> GetRecordPluginCountsByGame(SupportedGame game)
        {
            return new Dictionary<string, int>();
        }

        public IReadOnlyList<NPCDTO> GetByFormKey(SupportedGame game, FormKeyDTO formKey)
        {
            return Records;
        }

        public void Save(NPCDTO dto)
        { }

        public void DeleteStaleByPlugin(SupportedGame game, ModKeyDTO modKey, DateTime importedAtUTC)
        { }
    }

    private sealed class TestMagicEffectRepository : IMagicEffectRepository
    {
        public string RecordType => RecordTypeCatalog.MagicEffect.RecordID;

        public IReadOnlyList<MagicEffectDTO> Records { get; set; } = [];

        public IReadOnlyList<RecordTreeEntryDTO> GetRecordTreeEntriesByPlugin(SupportedGame game, ModKeyDTO modKey)
        {
            return [];
        }

        public IReadOnlyDictionary<string, int> GetRecordPluginCountsByGame(SupportedGame game)
        {
            return new Dictionary<string, int>();
        }

        public IReadOnlyList<MagicEffectDTO> GetByFormKey(SupportedGame game, FormKeyDTO formKey)
        {
            return Records;
        }

        public void Save(MagicEffectDTO dto)
        { }

        public void DeleteStaleByPlugin(SupportedGame game, ModKeyDTO modKey, DateTime importedAtUTC)
        { }
    }

    private sealed class TestPerkRepository : IPerkRepository
    {
        public string RecordType => RecordTypeCatalog.Perk.RecordID;

        public IReadOnlyList<PerkDTO> Records { get; set; } = [];

        public IReadOnlyList<RecordTreeEntryDTO> GetRecordTreeEntriesByPlugin(SupportedGame game, ModKeyDTO modKey)
        {
            return [];
        }

        public IReadOnlyDictionary<string, int> GetRecordPluginCountsByGame(SupportedGame game)
        {
            return new Dictionary<string, int>();
        }

        public IReadOnlyList<PerkDTO> GetByFormKey(SupportedGame game, FormKeyDTO formKey)
        {
            return Records;
        }

        public void Save(PerkDTO dto)
        { }

        public void DeleteStaleByPlugin(SupportedGame game, ModKeyDTO modKey, DateTime importedAtUTC)
        { }
    }

    private sealed class TestStaticRepository : IStaticRepository
    {
        public string RecordType => RecordTypeCatalog.Static.RecordID;

        public IReadOnlyList<StaticDTO> Records { get; set; } = [];

        public IReadOnlyList<RecordTreeEntryDTO> GetRecordTreeEntriesByPlugin(SupportedGame game, ModKeyDTO modKey)
        {
            return [];
        }

        public IReadOnlyDictionary<string, int> GetRecordPluginCountsByGame(SupportedGame game)
        {
            return new Dictionary<string, int>();
        }

        public IReadOnlyList<StaticDTO> GetByFormKey(SupportedGame game, FormKeyDTO formKey)
        {
            return Records;
        }

        public void Save(StaticDTO dto)
        { }

        public void DeleteStaleByPlugin(SupportedGame game, ModKeyDTO modKey, DateTime importedAtUTC)
        { }
    }

    private sealed class TestBookRepository : IBookRepository
    {
        public string RecordType => RecordTypeCatalog.Book.RecordID;

        public IReadOnlyList<BookDTO> Records { get; set; } = [];

        public IReadOnlyList<RecordTreeEntryDTO> GetRecordTreeEntriesByPlugin(SupportedGame game, ModKeyDTO modKey)
        {
            return [];
        }

        public IReadOnlyDictionary<string, int> GetRecordPluginCountsByGame(SupportedGame game)
        {
            return new Dictionary<string, int>();
        }

        public IReadOnlyList<BookDTO> GetByFormKey(SupportedGame game, FormKeyDTO formKey)
        {
            return Records;
        }

        public void Save(BookDTO dto)
        { }

        public void DeleteStaleByPlugin(SupportedGame game, ModKeyDTO modKey, DateTime importedAtUTC)
        { }
    }

    private sealed class TestDoorRepository : IDoorRepository
    {
        public string RecordType => RecordTypeCatalog.Door.RecordID;

        public IReadOnlyList<DoorDTO> Records { get; set; } = [];

        public IReadOnlyList<RecordTreeEntryDTO> GetRecordTreeEntriesByPlugin(SupportedGame game, ModKeyDTO modKey)
        {
            return [];
        }

        public IReadOnlyDictionary<string, int> GetRecordPluginCountsByGame(SupportedGame game)
        {
            return new Dictionary<string, int>();
        }

        public IReadOnlyList<DoorDTO> GetByFormKey(SupportedGame game, FormKeyDTO formKey)
        {
            return Records;
        }

        public void Save(DoorDTO dto)
        { }

        public void DeleteStaleByPlugin(SupportedGame game, ModKeyDTO modKey, DateTime importedAtUTC)
        { }
    }

    private sealed class TestContainerRepository : IContainerRepository
    {
        public string RecordType => RecordTypeCatalog.Container.RecordID;

        public IReadOnlyList<ContainerDTO> Records { get; set; } = [];

        public IReadOnlyList<RecordTreeEntryDTO> GetRecordTreeEntriesByPlugin(SupportedGame game, ModKeyDTO modKey)
        {
            return [];
        }

        public IReadOnlyDictionary<string, int> GetRecordPluginCountsByGame(SupportedGame game)
        {
            return new Dictionary<string, int>();
        }

        public IReadOnlyList<ContainerDTO> GetByFormKey(SupportedGame game, FormKeyDTO formKey)
        {
            return Records;
        }

        public void Save(ContainerDTO dto)
        { }

        public void DeleteStaleByPlugin(SupportedGame game, ModKeyDTO modKey, DateTime importedAtUTC)
        { }
    }

    private sealed class TestConstructibleObjectRepository : IConstructibleObjectRepository
    {
        public string RecordType => RecordTypeCatalog.ConstructibleObject.RecordID;

        public IReadOnlyList<ConstructibleObjectDTO> Records { get; set; } = [];

        public IReadOnlyList<RecordTreeEntryDTO> GetRecordTreeEntriesByPlugin(SupportedGame game, ModKeyDTO modKey)
        {
            return [];
        }

        public IReadOnlyDictionary<string, int> GetRecordPluginCountsByGame(SupportedGame game)
        {
            return new Dictionary<string, int>();
        }

        public IReadOnlyList<ConstructibleObjectDTO> GetByFormKey(SupportedGame game, FormKeyDTO formKey)
        {
            return Records;
        }

        public void Save(ConstructibleObjectDTO dto)
        { }

        public void DeleteStaleByPlugin(SupportedGame game, ModKeyDTO modKey, DateTime importedAtUTC)
        { }
    }

    private sealed class TestConditionFormRepository : IConditionFormRepository
    {
        public string RecordType => RecordTypeCatalog.ConditionForm.RecordID;

        public IReadOnlyList<ConditionFormDTO> Records { get; set; } = [];

        public IReadOnlyList<RecordTreeEntryDTO> GetRecordTreeEntriesByPlugin(SupportedGame game, ModKeyDTO modKey)
        {
            return [];
        }

        public IReadOnlyDictionary<string, int> GetRecordPluginCountsByGame(SupportedGame game)
        {
            return new Dictionary<string, int>();
        }

        public IReadOnlyList<ConditionFormDTO> GetByFormKey(SupportedGame game, FormKeyDTO formKey)
        {
            return Records;
        }

        public void Save(ConditionFormDTO dto)
        { }

        public void DeleteStaleByPlugin(SupportedGame game, ModKeyDTO modKey, DateTime importedAtUTC)
        { }
    }

    private sealed class TestTerminalRepository : ITerminalRepository
    {
        public string RecordType => RecordTypeCatalog.Terminal.RecordID;

        public IReadOnlyList<TerminalDTO> Records { get; set; } = [];

        public IReadOnlyList<RecordTreeEntryDTO> GetRecordTreeEntriesByPlugin(SupportedGame game, ModKeyDTO modKey)
        {
            return [];
        }

        public IReadOnlyDictionary<string, int> GetRecordPluginCountsByGame(SupportedGame game)
        {
            return new Dictionary<string, int>();
        }

        public IReadOnlyList<TerminalDTO> GetByFormKey(SupportedGame game, FormKeyDTO formKey)
        {
            return Records;
        }

        public void Save(TerminalDTO dto)
        { }

        public void DeleteStaleByPlugin(SupportedGame game, ModKeyDTO modKey, DateTime importedAtUTC)
        { }
    }

    private sealed class TestModelRepository : IModelRepository
    {
        public IReadOnlyList<ModelDTO> Records { get; set; } = [];

        public void Save(ModelDTO dto)
        { }

        public IReadOnlyList<ModelDTO> GetByFormKey(SupportedGame game, string recordType, FormKeyDTO formKey)
        {
            return Records;
        }

        public void DeleteByRecord(SupportedGame game, ModKeyDTO modKey, string recordType, FormKeyDTO formKey)
        { }

        public void DeleteStaleByPlugin(SupportedGame game, ModKeyDTO modKey, DateTime importedAtUTC)
        { }
    }

    private sealed class TestRecordKeywordRepository : IRecordKeywordRepository
    {
        public IReadOnlyList<RecordKeywordDTO> Records { get; set; } = [];

        public void Save(RecordKeywordDTO dto)
        { }

        public IReadOnlyList<RecordKeywordDTO> GetByFormKey(SupportedGame game, string recordType, FormKeyDTO formKey)
        {
            return Records;
        }

        public void DeleteByRecord(SupportedGame game, ModKeyDTO modKey, string recordType, FormKeyDTO formKey)
        { }

        public void DeleteStaleByPlugin(SupportedGame game, ModKeyDTO modKey, DateTime importedAtUTC)
        { }
    }

    private sealed class TestRecordSoundRepository : IRecordSoundRepository
    {
        public IReadOnlyList<RecordSoundDTO> Records { get; set; } = [];

        public void Save(RecordSoundDTO dto)
        { }

        public IReadOnlyList<RecordSoundDTO> GetByFormKey(SupportedGame game, string recordType, FormKeyDTO formKey)
        {
            return Records;
        }

        public void DeleteByRecord(SupportedGame game, ModKeyDTO modKey, string recordType, FormKeyDTO formKey)
        { }

        public void DeleteStaleByPlugin(SupportedGame game, ModKeyDTO modKey, DateTime importedAtUTC)
        { }
    }

    private sealed class TestRawRecordPayloadRepository : IRawRecordPayloadRepository
    {
        public IReadOnlyList<RawRecordPayloadDTO> Records { get; set; } = [];

        public void Save(RawRecordPayloadDTO dto)
        { }

        public IReadOnlyList<RawRecordPayloadDTO> GetByFormKey(SupportedGame game, string recordType, FormKeyDTO formKey)
        {
            return Records;
        }

        public void DeleteByRecord(SupportedGame game, ModKeyDTO modKey, string recordType, FormKeyDTO formKey)
        { }
    }

    private sealed class TestRecordLocalizedStringRepository : IRecordLocalizedStringRepository
    {
        public IReadOnlyList<LocalizedStringDTO> Records { get; set; } = [];

        public void Save(LocalizedStringDTO dto)
        { }

        public IReadOnlyList<LocalizedStringDTO> GetByFormKey(SupportedGame game, string recordType, FormKeyDTO formKey)
        {
            return Records;
        }

        public void DeleteByRecord(SupportedGame game, ModKeyDTO modKey, string recordType, FormKeyDTO formKey)
        { }

        public void DeleteStaleByPlugin(SupportedGame game, ModKeyDTO modKey, DateTime importedAtUTC)
        { }
    }

    private sealed class TestGameSelectionService : IGameSelectionService
    {
        public string RecordTextLanguage { get; set; } = ApplicationConfiguration.DefaultRecordTextLanguage;

        public IReadOnlyList<SupportedGameDTO> GetSupportedGames()
        {
            return [];
        }

        public SupportedGame? GetActiveGame()
        {
            return null;
        }

        public ApplicationThemeMode GetThemeMode()
        {
            return ApplicationThemeMode.Dark;
        }

        public ApplicationThemeFamily GetThemeFamily()
        {
            return ApplicationThemeFamily.Semi;
        }

        public IReadOnlyList<string> GetRecordTextLanguages()
        {
            return [RecordTextLanguage];
        }

        public string GetRecordTextLanguage()
        {
            return RecordTextLanguage;
        }

        public void SetActiveGame(SupportedGame game)
        { }

        public void SetThemeMode(ApplicationThemeMode themeMode)
        { }

        public void SetThemeFamily(ApplicationThemeFamily themeFamily)
        { }

        public void SetActiveGameAndThemeMode(SupportedGame game, ApplicationThemeMode themeMode)
        { }

        public void SetActiveGameAndTheme(SupportedGame game, ApplicationThemeFamily themeFamily, ApplicationThemeMode themeMode)
        { }

        public void SetTheme(ApplicationThemeFamily themeFamily, ApplicationThemeMode themeMode)
        { }
    }

    private sealed class TestScriptingAdapterRepository : IScriptingAdapterRepository
    {
        public IReadOnlyList<ScriptingAdapterDTO> Records { get; set; } = [];

        public void Save(ScriptingAdapterDTO dto)
        { }

        public IReadOnlyList<ScriptingAdapterDTO> GetByFormKey(SupportedGame game, string recordType, FormKeyDTO formKey)
        {
            return Records;
        }

        public void DeleteByRecord(SupportedGame game, ModKeyDTO modKey, string recordType, FormKeyDTO formKey)
        { }

        public void DeleteStaleByPlugin(SupportedGame game, ModKeyDTO modKey, DateTime importedAtUTC)
        { }
    }
}
