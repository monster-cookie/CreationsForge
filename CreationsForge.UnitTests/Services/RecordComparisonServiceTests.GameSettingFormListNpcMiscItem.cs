using CreationsForge.Core.DTOs.Games;
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Models.Configuration;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services;
using CreationsForge.Core.Services.Interfaces;
using CreationsForge.Specification.Records;
using Mutagen.Bethesda.Strings;
using Shouldly;

namespace CreationsForge.UnitTests.Services;

/// <summary>
/// Contains record comparison scenarios for game settings, form lists, NPCs, and misc items.
/// </summary>
public partial class RecordComparisonServiceTests
{
    [Fact]
    public void GetRecordComparison_ForGameSetting_HidesRedundantTypedValueFields()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x456);
        var gameSettingRepository = new TestGameSettingRepository
        {
            Records =
            [
                CreateGameSetting("Base.esm", formKey, "fSetting", GameSettingDataType.Float, floatData: 1.25),
                CreateGameSetting("Patch.esp", formKey, "fSetting", GameSettingDataType.Float, floatData: 1.75)
            ]
        };
        var service = CreateService(gameSettingRepository: gameSettingRepository);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.GameSetting.RecordID, formKey);

        comparison.Columns.Select(column => column.Header).ShouldBe(["Base.esm", "Patch.esp"]);
        comparison.Fields.Single(field => field.FieldName == "MutagenObjectType").Values.Select(value => value.DisplayValue).ShouldBe(["GameSettingFloat", "GameSettingFloat"]);
        comparison.Fields.Single(field => field.FieldName == "Data").Values.Select(value => value.DisplayValue).ShouldBe(["1.25", "1.75"]);
        comparison.Fields.ShouldNotContain(field => field.FieldName == "FloatData");
        comparison.Fields.ShouldNotContain(field => field.FieldName == "IntegerData");
        comparison.Fields.ShouldNotContain(field => field.FieldName == "UnsignedIntegerData");
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
                CreateGameSetting("Base.esm", formKey, "sSetting", GameSettingDataType.String, stringData: "Base English"),
                CreateGameSetting("Patch.esp", formKey, "sSetting", GameSettingDataType.String, stringData: "Patch English")
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
        var gameSelectionService = new TestGameSelectionService { RecordTextLanguage = Language.German };
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

    /// <summary>
    /// Verifies that attributed NPC numeric fields use reduced precision for comparison display and state.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForNPC_UsesNumericDisplayPrecisionAttributes()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x1010);
        var npcRepository = new TestNPCRepository
        {
            Records =
            [
                CreateNPC("Base.esm", formKey, 1.2344, 1.2345),
                CreateNPC("Patch.esp", formKey, 1.23449, 1.2355)
            ]
        };
        var service = CreateService(npcRepository: npcRepository);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.NPC.RecordID, formKey);

        var heightMin = comparison.Fields.Single(field => field.FieldName == "HeightMin");
        heightMin.Values.Select(value => value.DisplayValue).ShouldBe(["1.234", "1.234"]);
        heightMin.State.ShouldBe(RecordComparisonValueState.Identical);
        heightMin.Values.Select(value => value.State).ShouldBe([RecordComparisonValueState.Identical, RecordComparisonValueState.Identical]);

        var heightMax = comparison.Fields.Single(field => field.FieldName == "HeightMax");
        heightMax.Values.Select(value => value.DisplayValue).ShouldBe(["1.235", "1.236"]);
        heightMax.State.ShouldBe(RecordComparisonValueState.Conflict);
        heightMax.Values.Select(value => value.State).ShouldBe([RecordComparisonValueState.Conflict, RecordComparisonValueState.WinningOverride]);
    }

    /// <summary>
    /// Verifies that NPC comparison output renders first-class persisted child rows instead of only scalar actor data.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForNPC_RendersPersistedChildRows()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x831);
        var baseNpc = CreateNPC("Base.esm", formKey, 1, 1);
        var patchNpc = CreateNPC("Patch.esp", formKey, 1, 1);
        patchNpc.Class = CreateFormKey("Starfield.esm", 0x20F487);
        patchNpc.DefaultOutfit = CreateFormKey("Starfield.esm", 0x102EC);
        patchNpc.Weight = new NPCWeightDTO { Thin = 0.54, Muscular = 0, Fat = 0 };
        patchNpc.HeadParts.Add(CreateFormKey("Starfield.esm", 0x3E2B2));
        patchNpc.FaceDialPositions.Add(new NPCFaceDialPositionDTO
        {
            FaceDialPositionIndex = 0,
            Index = 24,
            Position = -0.512
        });
        patchNpc.FaceMorphGroups.Add(new NPCFaceMorphGroupSetDTO
        {
            FaceMorphIndex = 0,
            Index = 12,
            MorphGroups =
            {
                new NPCFaceMorphGroupDTO
                {
                    FaceMorphIndex = 0,
                    MorphGroupIndex = 0,
                    MorphGroup = "Cheeks",
                    BlendIntensity = 1
                }
            }
        });
        patchNpc.MorphBlends.Add(new NPCMorphBlendDTO
        {
            MorphBlendIndex = 0,
            BlendName = "male_eu_md2_Cheeks",
            Intensity = 1
        });
        patchNpc.Tints.Add(new NPCTintDTO
        {
            TintIndex = 0,
            TintType = "Simple Group",
            TintGroup = "Dermaesthetic",
            TintName = "European_Male_Md2_Sk3",
            TintTexture = "textures/actors/human/faces/chargen/postblenddetails/dermaesthetic/male_eu_md2_sk3_derm_color.dds",
            TintColor = "Black",
            TintIntensity = 64
        });
        var npcRepository = new TestNPCRepository
        {
            Records =
            [
                baseNpc,
                patchNpc
            ]
        };
        var service = CreateService(npcRepository: npcRepository);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.NPC.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "Class").Values.Select(value => value.DisplayValue).ShouldBe(["", "Starfield.esm:0020F487"]);
        comparison.Fields.Single(field => field.FieldName == "DefaultOutfit").Values.Select(value => value.DisplayValue).ShouldBe(["", "Starfield.esm:000102EC"]);
        comparison.Fields.Single(field => field.FieldName == "Weight").Children.Single(field => field.FieldName == "Thin").Values.Select(value => value.DisplayValue).ShouldBe(["", "0.54"]);
        comparison.Fields.Single(field => field.FieldName == "HeadParts").Children.Single(field => field.FieldName == "HeadPart [0]").Values.Select(value => value.DisplayValue).ShouldBe(["", "Starfield.esm:0003E2B2"]);
        comparison.Fields.Single(field => field.FieldName == "FaceDialPositions").Children.Single(field => field.FieldName == "FaceDialPosition [0]").Children.Single(field => field.FieldName == "Position").Values.Select(value => value.DisplayValue).ShouldBe(["", "-0.512"]);
        comparison.Fields.Single(field => field.FieldName == "FaceMorphGroups").Children.Single(field => field.FieldName == "FaceMorph [0]").Children.Single(field => field.FieldName == "MorphGroup [0]").Children.Single(field => field.FieldName == "MorphGroup").Values.Select(value => value.DisplayValue).ShouldBe(["", "Cheeks"]);
        comparison.Fields.Single(field => field.FieldName == "MorphBlends").Children.Single(field => field.FieldName == "MorphBlend [0]").Children.Single(field => field.FieldName == "BlendName").Values.Select(value => value.DisplayValue).ShouldBe(["", "male_eu_md2_Cheeks"]);
        comparison.Fields.Single(field => field.FieldName == "Tints").Children.Single(field => field.FieldName == "Tint [0]").Children.Single(field => field.FieldName == "TintName").Values.Select(value => value.DisplayValue).ShouldBe(["", "European_Male_Md2_Sk3"]);
    }

    [Fact]
    public void GetRecordComparison_ForMiscItem_MapsTypedScalarFields()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x818);
        var messageFormKey = CreateFormKey("Starfield.esm", 0x444);
        var baseItem = CreateMiscItem("Base.esm", formKey, "Digipick", 35, 0.1f, null);
        baseItem.Components.Add(CreateMiscItemComponent("Base.esm", formKey, CreateFormKey("Starfield.esm", 0x777), 0, 0, 2));
        baseItem.Destructible = CreateMiscItemDestructible(CreateFormKey("Starfield.esm", 0x888), 100, 2, "BaseStage.nif", "AABB");
        var patchItem = CreateMiscItem("Patch.esp", formKey, "Digipick", 50, 0.2f, messageFormKey);
        patchItem.Components.Add(CreateMiscItemComponent("Patch.esp", formKey, CreateFormKey("Starfield.esm", 0x777), 0, 2, 4));
        patchItem.Destructible = CreateMiscItemDestructible(CreateFormKey("Starfield.esm", 0x999), 90, 3, "PatchStage.nif", "CCDD");
        var miscItemRepository = new TestMiscItemRepository
        {
            Records =
            [
                baseItem,
                patchItem
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
        var keywordMappingRepository = new TestKeywordMappingRepository
        {
            Records =
            [
                CreateKeywordMapping("Base.esm", RecordTypeCatalog.MiscItem.RecordID, formKey, CreateFormKey("Starfield.esm", 0x555), 0),
                CreateKeywordMapping("Patch.esp", RecordTypeCatalog.MiscItem.RecordID, formKey, CreateFormKey("Starfield.esm", 0x555), 0)
            ]
        };
        var soundMappingRepository = new TestSoundMappingRepository
        {
            Records =
            [
                CreateSoundMapping("Base.esm", RecordTypeCatalog.MiscItem.RecordID, formKey, "PickupSound", 1, "ff0b45e7-a8ae-a30f-390b-d0cd2b6933a6"),
                CreateSoundMapping("Patch.esp", RecordTypeCatalog.MiscItem.RecordID, formKey, "PickupSound", 1, "ff0b45e7-a8ae-a30f-390b-d0cd2b6933a6")
            ]
        };
        var service = CreateService(
            miscItemRepository: miscItemRepository,
            modelRepository: modelRepository,
            keywordMappingRepository: keywordMappingRepository,
            soundMappingRepository: soundMappingRepository,
            scriptingAdapterRepository: scriptingAdapterRepository);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.MiscItem.RecordID, formKey);

        comparison.Columns.Select(column => column.Header).ShouldBe(["Base.esm", "Patch.esp"]);
        comparison.Fields.Single(field => field.FieldName == "Name").Values.Select(value => value.DisplayValue).ShouldBe(["Digipick", "Digipick"]);
        comparison.Fields.Single(field => field.FieldName == "Value").Values.Select(value => value.DisplayValue).ShouldBe(["35", "50"]);
        comparison.Fields.Single(field => field.FieldName == "Weight").Values.Select(value => value.DisplayValue).ShouldBe(["0.1", "0.2"]);
        comparison.Fields.Single(field => field.FieldName == "FeaturedItemMessage").Values.Select(value => value.DisplayValue).ShouldBe(["", "Starfield.esm:00000444"]);
        var keywords = comparison.Fields.Single(field => field.FieldName == "Keywords");
        keywords.Children.Single(field => field.FieldName == "Keyword [0]").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000555", "Starfield.esm:00000555"]);
        var model = comparison.Fields.Single(field => field.FieldName == "Model");
        model.Children.Single(field => field.FieldName == "File").Values.Select(value => value.DisplayValue).ShouldBe(["Meshes\\Clutter\\Digipick.nif", "Meshes\\Clutter\\Digipick.nif"]);
        var sounds = comparison.Fields.Single(field => field.FieldName == "Sounds");
        var pickupSound = sounds.Children.Single(field => field.FieldName == "PickupSound [1]");
        pickupSound.Children.Single(field => field.FieldName == "Start").Values.Select(value => value.DisplayValue).ShouldBe(["ff0b45e7-a8ae-a30f-390b-d0cd2b6933a6", "ff0b45e7-a8ae-a30f-390b-d0cd2b6933a6"]);
        var components = comparison.Fields.Single(field => field.FieldName == "Components");
        var component = components.Children.Single(field => field.FieldName == "Component [0]");
        component.Children.Single(field => field.FieldName == "DisplayIndex").Values.Select(value => value.DisplayValue).ShouldBe(["0", "2"]);
        component.Children.Single(field => field.FieldName == "Count").Values.Select(value => value.DisplayValue).ShouldBe(["2", "4"]);
        var destructible = comparison.Fields.Single(field => field.FieldName == "Destructible");
        destructible.Children.Single(field => field.FieldName == "Health").Values.Select(value => value.DisplayValue).ShouldBe(["100", "90"]);
        destructible.Children.Single(field => field.FieldName == "DESTCount").Values.Select(value => value.DisplayValue).ShouldBe(["2", "3"]);
        var stage = destructible.Children.Single(field => field.FieldName == "Stage [0]");
        stage.Children.Single(field => field.FieldName == "Explosion").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000888", "Starfield.esm:00000999"]);
        stage.Children.Single(field => field.FieldName == "Model.File").Values.Select(value => value.DisplayValue).ShouldBe(["BaseStage.nif", "PatchStage.nif"]);
        stage.Children.Single(field => field.FieldName == "Model.Data").Values.Select(value => value.DisplayValue).ShouldBe(["AABB", "CCDD"]);

        var scripts = comparison.Fields.Single(field => field.FieldName == "Scripts");
        var script = scripts.Children.Single(field => field.FieldName == "Script [0]");
        script.Children.Single(field => field.FieldName == "Name").Values.Select(value => value.DisplayValue).ShouldBe(["DefaultScript", "DefaultScript"]);
        var property = script.Children.Single(field => field.FieldName == "Property [0]");
        property.Children.Single(field => field.FieldName == "Name").Values.Select(value => value.DisplayValue).ShouldBe(["PropertyName", "PropertyName"]);
        property.Children.Single(field => field.FieldName == "Value").Values.Select(value => value.DisplayValue).ShouldBe(["BaseValue", "PatchValue"]);
    }

    /// <summary>
    /// Verifies that Misc Item scalar rows are selected from the injected comparison specification while child rows
    /// remain strategy-based.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForMiscItem_UsesInjectedComparisonSpecification()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x819);
        var messageFormKey = CreateFormKey("Starfield.esm", 0x444);
        var baseItem = CreateMiscItem("Base.esm", formKey, "Digipick", 35, 0.1f, null);
        baseItem.Components.Add(CreateMiscItemComponent("Base.esm", formKey, CreateFormKey("Starfield.esm", 0x777), 0, 0, 2));
        var patchItem = CreateMiscItem("Patch.esp", formKey, "Digipick", 50, 0.2f, messageFormKey);
        patchItem.Components.Add(CreateMiscItemComponent("Patch.esp", formKey, CreateFormKey("Starfield.esm", 0x777), 0, 2, 4));
        var miscItemRepository = new TestMiscItemRepository
        {
            Records =
            [
                baseItem,
                patchItem
            ]
        };
        var provider = new TestRecordSpecificationProvider(
            new RecordSpecification
            {
                RecordID = SupportedRecordSpecifications.MiscItem.RecordID,
                RecordType = SupportedRecordSpecifications.MiscItem.RecordType,
                TableName = SupportedRecordSpecifications.MiscItem.TableName,
                FriendlyName = SupportedRecordSpecifications.MiscItem.FriendlyName,
                GameSupport = SupportedRecordSpecifications.MiscItem.GameSupport,
                Fields = SupportedRecordSpecifications.MiscItem.Fields,
                Comparison = new RecordComparisonSpecification
                {
                    Fields =
                    [
                        new RecordComparisonFieldSpecification
                        {
                            FieldName = "Value",
                            SourcePath = "Value",
                            ValueKind = RecordFieldValueKind.Number
                        }
                    ]
                },
                ImplementationNote = "Test specification."
            });
        var service = CreateService(miscItemRepository: miscItemRepository, recordSpecificationProvider: provider);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.MiscItem.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "Value").Values.Select(value => value.DisplayValue).ShouldBe(["35", "50"]);
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Name");
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Weight");
        comparison.Fields.Single(field => field.FieldName == "Components").Children.ShouldNotBeEmpty();
    }

}
