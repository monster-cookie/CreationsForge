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
                CreateGlobal("Base.esm", formKey, "MyGlobal", 1.5, "GlobalShort", "Constant"),
                CreateGlobal("Patch.esp", formKey, "MyGlobal", 2.5, "GlobalFloat", "Constant")
            ]
        };
        var service = CreateService(globalRepository: globalRepository);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.Global.RecordID, formKey);

        comparison.RecordType.ShouldBe(RecordTypeCatalog.Global.RecordID);
        comparison.Columns.Select(column => column.Header).ShouldBe(["Base.esm", "Patch.esp"]);
        comparison.Fields.Single(field => field.FieldName == "MutagenObjectType").Values.Select(value => value.DisplayValue).ShouldBe(["GlobalShort", "GlobalFloat"]);
        comparison.Fields.Single(field => field.FieldName == "MajorFlags").Values.Select(value => value.DisplayValue).ShouldBe(["Constant", "Constant"]);
        comparison.Fields.Single(field => field.FieldName == "Data").Values.Select(value => value.DisplayValue).ShouldBe(["1.5", "2.5"]);
        comparison.Fields.Single(field => field.FieldName == "Data").State.ShouldBe(RecordComparisonValueState.Conflict);
        comparison.Fields.Single(field => field.FieldName == "Data").Values.Select(value => value.State).ShouldBe([RecordComparisonValueState.Conflict, RecordComparisonValueState.WinningOverride]);
    }

    /// <summary>
    /// Verifies that the Global pilot path reads type-specific rows from the injected comparison specification.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForGlobal_UsesInjectedComparisonSpecification()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x124);
        var globalRepository = new TestGlobalRepository
        {
            Records =
            [
                CreateGlobal("Base.esm", formKey, "MyGlobal", 1.5, "GlobalShort", "Constant")
            ]
        };
        var provider = new TestRecordSpecificationProvider(
            new RecordSpecification
            {
                RecordID = SupportedRecordSpecifications.Global.RecordID,
                RecordType = SupportedRecordSpecifications.Global.RecordType,
                TableName = SupportedRecordSpecifications.Global.TableName,
                FriendlyName = SupportedRecordSpecifications.Global.FriendlyName,
                GameSupport = SupportedRecordSpecifications.Global.GameSupport,
                Fields = SupportedRecordSpecifications.Global.Fields,
                Comparison = new RecordComparisonSpecification
                {
                    Fields =
                    [
                        new RecordComparisonFieldSpecification
                        {
                            FieldName = "Data",
                            SourcePath = "Data",
                            ValueKind = RecordFieldValueKind.Number
                        }
                    ]
                },
                ImplementationNote = "Test specification."
            });
        var service = CreateService(globalRepository: globalRepository, recordSpecificationProvider: provider);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.Global.RecordID, formKey);

        comparison.Fields.ShouldContain(field => field.FieldName == "Data");
        comparison.Fields.ShouldNotContain(field => field.FieldName == "MutagenObjectType");
        comparison.Fields.ShouldNotContain(field => field.FieldName == "MajorFlags");
    }

    /// <summary>
    /// Verifies that Keyword scalar rows are selected from the injected comparison specification.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForKeyword_UsesInjectedComparisonSpecification()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x125);
        var keywordRepository = new TestKeywordRepository
        {
            Records =
            [
                CreateKeyword("Base.esm", formKey, "BaseType", "Blue"),
                CreateKeyword("Patch.esp", formKey, "PatchType", "Red")
            ]
        };
        var provider = new TestRecordSpecificationProvider(
            new RecordSpecification
            {
                RecordID = SupportedRecordSpecifications.Keyword.RecordID,
                RecordType = SupportedRecordSpecifications.Keyword.RecordType,
                TableName = SupportedRecordSpecifications.Keyword.TableName,
                FriendlyName = SupportedRecordSpecifications.Keyword.FriendlyName,
                GameSupport = SupportedRecordSpecifications.Keyword.GameSupport,
                Fields = SupportedRecordSpecifications.Keyword.Fields,
                Comparison = new RecordComparisonSpecification
                {
                    Fields =
                    [
                        new RecordComparisonFieldSpecification
                        {
                            FieldName = "Type",
                            SourcePath = "Type",
                            ValueKind = RecordFieldValueKind.Text
                        }
                    ]
                },
                ImplementationNote = "Test specification."
            });
        var service = CreateService(keywordRepository: keywordRepository, recordSpecificationProvider: provider);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.Keyword.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "Type").Values.Select(value => value.DisplayValue)
            .ShouldBe(["BaseType", "PatchType"]);
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Color");
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Name");
    }

    /// <summary>
    /// Verifies that specification-declared localized Keyword rows use the selected record text language.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForKeyword_UsesSpecificationLocalizedDisplay()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x127);
        var keywordRepository = new TestKeywordRepository
        {
            Records =
            [
                CreateKeyword("Base.esm", formKey, "BaseType", "Blue"),
                CreateKeyword("Patch.esp", formKey, "PatchType", "Red")
            ]
        };
        var localizedStringRepository = new TestRecordLocalizedStringRepository
        {
            Records =
            [
                CreateLocalizedString("Base.esm", formKey, "Name", "German", "Basis Schluesselwort"),
                CreateLocalizedString("Patch.esp", formKey, "Name", "German", "Patch Schluesselwort")
            ]
        };
        var gameSelectionService = new TestGameSelectionService { RecordTextLanguage = Language.German };
        var service = CreateService(
            keywordRepository: keywordRepository,
            recordLocalizedStringRepository: localizedStringRepository,
            gameSelectionService: gameSelectionService);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.Keyword.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "Name").Values.Select(value => value.DisplayValue)
            .ShouldBe(["Basis Schluesselwort", "Patch Schluesselwort"]);
    }

    /// <summary>
    /// Verifies that Static scalar rows are selected from the injected comparison specification while strategy rows
    /// remain outside the scalar metadata path.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForStatic_UsesInjectedComparisonSpecification()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x126);
        var staticRepository = new TestStaticRepository
        {
            Records =
            [
                CreateStatic("Base.esm", formKey, 35, "0, 0, 0", null),
                CreateStatic("Patch.esp", formKey, 45, "1, 1, 1", 1.25)
            ]
        };
        var provider = new TestRecordSpecificationProvider(
            new RecordSpecification
            {
                RecordID = SupportedRecordSpecifications.Static.RecordID,
                RecordType = SupportedRecordSpecifications.Static.RecordType,
                TableName = SupportedRecordSpecifications.Static.TableName,
                FriendlyName = SupportedRecordSpecifications.Static.FriendlyName,
                GameSupport = SupportedRecordSpecifications.Static.GameSupport,
                Fields = SupportedRecordSpecifications.Static.Fields,
                Comparison = new RecordComparisonSpecification
                {
                    Fields =
                    [
                        new RecordComparisonFieldSpecification
                        {
                            FieldName = "MaxAngle",
                            SourcePath = "MaxAngle",
                            ValueKind = RecordFieldValueKind.Number
                        }
                    ]
                },
                ImplementationNote = "Test specification."
            });
        var service = CreateService(staticRepository: staticRepository, recordSpecificationProvider: provider);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.Static.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "MaxAngle").Values.Select(value => value.DisplayValue)
            .ShouldBe(["35", "45"]);
        comparison.Fields.ShouldNotContain(field => field.FieldName == "ObjectBoundsFirst");
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Name");
    }

    /// <summary>
    /// Verifies that specification-declared localized Static rows use the selected record text language.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForStatic_UsesSpecificationLocalizedDisplay()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x128);
        var staticRepository = new TestStaticRepository
        {
            Records =
            [
                CreateStatic("Base.esm", formKey, 35, "0, 0, 0", null),
                CreateStatic("Patch.esp", formKey, 45, "1, 1, 1", 1.25)
            ]
        };
        var localizedStringRepository = new TestRecordLocalizedStringRepository
        {
            Records =
            [
                CreateLocalizedString("Base.esm", formKey, "Name", "German", "Basis Statik"),
                CreateLocalizedString("Patch.esp", formKey, "Name", "German", "Patch Statik")
            ]
        };
        var gameSelectionService = new TestGameSelectionService { RecordTextLanguage = Language.German };
        var service = CreateService(
            staticRepository: staticRepository,
            recordLocalizedStringRepository: localizedStringRepository,
            gameSelectionService: gameSelectionService);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.Static.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "Name").Values.Select(value => value.DisplayValue)
            .ShouldBe(["Basis Statik", "Patch Statik"]);
    }

    /// <summary>
    /// Verifies that Book scalar rows are selected from the injected comparison specification.
    /// </summary>
    [Fact]
    public void GetRecordComparison_ForBook_UsesInjectedComparisonSpecification()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x129);
        var bookRepository = new TestBookRepository
        {
            Records =
            [
                CreateBook("Base.esm", formKey, "Captain's Log", 100),
                CreateBook("Patch.esp", formKey, "Captain's Log", 150)
            ]
        };
        var provider = new TestRecordSpecificationProvider(
            new RecordSpecification
            {
                RecordID = SupportedRecordSpecifications.Book.RecordID,
                RecordType = SupportedRecordSpecifications.Book.RecordType,
                TableName = SupportedRecordSpecifications.Book.TableName,
                FriendlyName = SupportedRecordSpecifications.Book.FriendlyName,
                GameSupport = SupportedRecordSpecifications.Book.GameSupport,
                Fields = SupportedRecordSpecifications.Book.Fields,
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
        var service = CreateService(bookRepository: bookRepository, recordSpecificationProvider: provider);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.Book.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "Value").Values.Select(value => value.DisplayValue)
            .ShouldBe(["100", "150"]);
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Name");
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Flags");
    }

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
        var keywordMappingRepository = new TestKeywordMappingRepository
        {
            Records =
            [
                CreateKeywordMapping("Base.esm", RecordTypeCatalog.MagicEffect.RecordID, formKey, CreateFormKey("Starfield.esm", 0x111), 0),
                CreateKeywordMapping("Patch.esp", RecordTypeCatalog.MagicEffect.RecordID, formKey, CreateFormKey("Patch.esp", 0x222), 0)
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
        var soundMappingRepository = new TestSoundMappingRepository
        {
            Records =
            [
                CreateSoundMapping("Base.esm", RecordTypeCatalog.MagicEffect.RecordID, formKey, "Charge", 2, "a328413d-b619-45b5-0d9e-aa9d0ade8280", "Break0", "000000"),
                CreateSoundMapping("Patch.esp", RecordTypeCatalog.MagicEffect.RecordID, formKey, "Charge", 2, "a328413d-b619-45b5-0d9e-aa9d0ade8280", "Break0", "000000")
            ]
        };
        var service = CreateService(
            magicEffectRepository: magicEffectRepository,
            keywordMappingRepository: keywordMappingRepository,
            soundMappingRepository: soundMappingRepository,
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
    public void GetRecordComparison_ForStatic_MapsStaticFieldsModelDataAndReflectPayloads()
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
                CreateModel("Base.esm", RecordTypeCatalog.Static.RecordID, formKey, "Meshes\\SetDressing\\Rock01.nif", "AABB"),
                CreateModel("Patch.esp", RecordTypeCatalog.Static.RecordID, formKey, "Meshes\\SetDressing\\Rock01.nif", "CCDD")
            ]
        };
        var keywordMappingRepository = new TestKeywordMappingRepository
        {
            Records =
            [
                CreateKeywordMapping("Base.esm", RecordTypeCatalog.Static.RecordID, formKey, CreateFormKey("Starfield.esm", 0x555), 0),
                CreateKeywordMapping("Patch.esp", RecordTypeCatalog.Static.RecordID, formKey, CreateFormKey("Starfield.esm", 0x666), 0)
            ]
        };
        var reflectionRepository = new TestReflectionRepository
        {
            Records =
            [
                CreateReflection("Base.esm", formKey, 0, "ReflectionComponent", "Components[0].REFL", "AABB"),
                CreateReflection("Patch.esp", formKey, 0, "ReflectionComponent", "Components[0].REFL", "CCDD")
            ]
        };
        var service = CreateService(
            staticRepository: staticRepository,
            modelRepository: modelRepository,
            keywordMappingRepository: keywordMappingRepository,
            reflectionRepository: reflectionRepository);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.Static.RecordID, formKey);

        comparison.RecordType.ShouldBe(RecordTypeCatalog.Static.RecordID);
        comparison.Fields.Single(field => field.FieldName == "MaxAngle").Values.Select(value => value.DisplayValue).ShouldBe(["35", "45"]);
        comparison.Fields.Single(field => field.FieldName == "ObjectBoundsFirst").Values.Select(value => value.DisplayValue).ShouldBe(["0, 0, 0", "0, 0, 0"]);
        comparison.Fields.Single(field => field.FieldName == "UnknownDNAMFloat").Values.Select(value => value.DisplayValue).ShouldBe(["", "1.25"]);
        var keywords = comparison.Fields.Single(field => field.FieldName == "Keywords");
        keywords.Children.Single(field => field.FieldName == "Keyword [0]").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000555", "Starfield.esm:00000666"]);
        var model = comparison.Fields.Single(field => field.FieldName == "Model");
        model.Children.Single(field => field.FieldName == "File").Values.Select(value => value.DisplayValue).ShouldBe(["Meshes\\SetDressing\\Rock01.nif", "Meshes\\SetDressing\\Rock01.nif"]);
        model.Children.Single(field => field.FieldName == "Data").Values.Select(value => value.DisplayValue).ShouldBe(["AABB", "CCDD"]);
        var reflection = comparison.Fields.Single(field => field.FieldName == "Reflection");
        var reflect = reflection.Children.Single(field => field.FieldName == "Components[0].REFL");
        reflect.Children.Single(field => field.FieldName == "ComponentType").Values.Select(value => value.DisplayValue).ShouldBe(["ReflectionComponent", "ReflectionComponent"]);
        reflect.Children.Single(field => field.FieldName == "SourcePath").Values.Select(value => value.DisplayValue).ShouldBe(["Components[0].REFL", "Components[0].REFL"]);
        var reflectValues = reflect.Children.Single(field => field.FieldName == "REFL").Values;
        reflectValues.Select(value => value.DisplayValue).ShouldBe(["[UNPARSEABLE REFLECTION DATA]", "[UNPARSEABLE REFLECTION DATA]"]);
        reflectValues.Select(value => value.DetailValue).ShouldBe(["AABB", "CCDD"]);
        reflectValues.Select(value => value.DisplayKind).ShouldBe([RecordComparisonValueDisplayKind.RawBinaryPayload, RecordComparisonValueDisplayKind.RawBinaryPayload]);
        reflect.Children.Single(field => field.FieldName == "REFL").State.ShouldBe(RecordComparisonValueState.Conflict);
    }

    [Fact]
    public void GetRecordComparison_ForContainer_MapsContainerFieldsItemsModelsAndAnimationFields()
    {
        var formKey = CreateFormKey("Starfield.esm", 0x2000);
        var itemFormKey = CreateFormKey("Starfield.esm", 0x333);
        var terminalFormKey = CreateFormKey("Starfield.esm", 0x444);
        var containerRepository = new TestContainerRepository
        {
            Records =
            [
                CreateContainer("Base.esm", formKey, "Storage Crate", terminalFormKey, [CreateContainerItem("Base.esm", formKey, itemFormKey, 0, 2)], "meshes\\base.anim"),
                CreateContainer("Patch.esp", formKey, "Storage Crate", terminalFormKey, [CreateContainerItem("Patch.esp", formKey, itemFormKey, 0, 4)], "meshes\\patch.anim")
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
        var service = CreateService(
            containerRepository: containerRepository,
            modelRepository: modelRepository);

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
        comparison.Fields.Single(field => field.FieldName == "AnimationGraph").Values.Select(value => value.DisplayValue).ShouldBe(["meshes\\base.anim", "meshes\\patch.anim"]);
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Base Form Components");
    }

    [Fact]
    public void GetRecordComparison_ForConstructibleObject_MapsComponentsConditionsScriptsAndCreatedObjectCount()
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
        var service = CreateService(
            constructibleObjectRepository: constructibleObjectRepository,
            scriptingAdapterRepository: scriptingAdapterRepository);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.ConstructibleObject.RecordID, formKey);

        comparison.RecordType.ShouldBe(RecordTypeCatalog.ConstructibleObject.RecordID);
        comparison.Fields.Single(field => field.FieldName == "CreatedObjectFormKey").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000111", "Starfield.esm:00000111"]);
        comparison.Fields.Single(field => field.FieldName == "WorkbenchKeywordFormKey").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000222", "Starfield.esm:00000222"]);
        comparison.Fields.Single(field => field.FieldName == "CreatedObjectCount").Values.Select(value => value.DisplayValue).ShouldBe(["2", "4"]);
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
        comparison.Fields.ShouldNotContain(field => field.FieldName == "Created Object Counts");
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
        var service = CreateService(conditionFormRepository: conditionFormRepository);

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
        var keywordMappingRepository = new TestKeywordMappingRepository
        {
            Records =
            [
                CreateKeywordMapping("Base.esm", RecordTypeCatalog.Book.RecordID, formKey, CreateFormKey("Starfield.esm", 0x101), 0),
                CreateKeywordMapping("Patch.esp", RecordTypeCatalog.Book.RecordID, formKey, CreateFormKey("Starfield.esm", 0x101), 0)
            ]
        };
        var soundMappingRepository = new TestSoundMappingRepository
        {
            Records =
            [
                CreateSoundMapping("Base.esm", RecordTypeCatalog.Book.RecordID, formKey, "PickupSound", 0, "pickup"),
                CreateSoundMapping("Patch.esp", RecordTypeCatalog.Book.RecordID, formKey, "PickupSound", 0, "pickup")
            ]
        };
        var service = CreateService(
            bookRepository: bookRepository,
            modelRepository: modelRepository,
            keywordMappingRepository: keywordMappingRepository,
            soundMappingRepository: soundMappingRepository);

        var comparison = service.GetRecordComparison(SupportedGame.Starfield, RecordTypeCatalog.Book.RecordID, formKey);

        comparison.Fields.Single(field => field.FieldName == "Name").Values.Select(value => value.DisplayValue).ShouldBe(["Captain's Log", "Captain's Log"]);
        comparison.Fields.Single(field => field.FieldName == "Value").Values.Select(value => value.DisplayValue).ShouldBe(["100", "150"]);
        comparison.Fields.Single(field => field.FieldName == "Transforms.Inventory").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000999", "Starfield.esm:00000999"]);
        comparison.Fields.Single(field => field.FieldName == "InventoryArt").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000998", "Starfield.esm:00000998"]);
        comparison.Fields.Single(field => field.FieldName == "PreviewTransform").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000888", "Starfield.esm:00000888"]);
        comparison.Fields.Single(field => field.FieldName == "FeaturedItemMessage").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000777", "Starfield.esm:00000777"]);
        comparison.Fields.Single(field => field.FieldName == "Text").Values.Select(value => value.DisplayValue).ShouldBe(["Base text", "Patch text"]);
        comparison.Fields.Single(field => field.FieldName == "Teaches.MutagenObjectType").Values.Select(value => value.DisplayValue).ShouldBe(["Skill", "Skill"]);
        comparison.Fields.Single(field => field.FieldName == "Teaches.Perk").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000666", "Starfield.esm:00000666"]);
        var keywords = comparison.Fields.Single(field => field.FieldName == "Keywords");
        keywords.Children.Single(field => field.FieldName == "Keyword [0]").Values.Select(value => value.DisplayValue).ShouldBe(["Starfield.esm:00000101", "Starfield.esm:00000101"]);
        var model = comparison.Fields.Single(field => field.FieldName == "Model");
        model.Children.Single(field => field.FieldName == "File").Values.Select(value => value.DisplayValue).ShouldBe(["Meshes\\SetDressing\\Books\\Book01.nif", "Meshes\\SetDressing\\Books\\Book01.nif"]);
        var sounds = comparison.Fields.Single(field => field.FieldName == "Sounds");
        sounds.Children.Single(field => field.FieldName == "PickupSound").Children.Single(field => field.FieldName == "Start").Values.Select(value => value.DisplayValue).ShouldBe(["pickup", "pickup"]);
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
        TestMiscItemRepository? miscItemRepository = null,
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
        TestKeywordMappingRepository? keywordMappingRepository = null,
        TestSoundMappingRepository? soundMappingRepository = null,
        TestScriptingAdapterRepository? scriptingAdapterRepository = null,
        TestReflectionRepository? reflectionRepository = null,
        TestRecordLocalizedStringRepository? recordLocalizedStringRepository = null,
        TestGameSelectionService? gameSelectionService = null,
        IRecordSpecificationProvider? recordSpecificationProvider = null)
    {
        return new RecordComparisonService(
            formListRepository ?? new TestFormListRepository(),
            gameSettingRepository ?? new TestGameSettingRepository(),
            globalRepository ?? new TestGlobalRepository(),
            classRepository ?? new TestClassRepository(),
            factionRepository ?? new TestFactionRepository(),
            miscItemRepository ?? new TestMiscItemRepository(),
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
            keywordMappingRepository ?? new TestKeywordMappingRepository(),
            soundMappingRepository ?? new TestSoundMappingRepository(),
            scriptingAdapterRepository ?? new TestScriptingAdapterRepository(),
            reflectionRepository ?? new TestReflectionRepository(),
            recordLocalizedStringRepository ?? new TestRecordLocalizedStringRepository(),
            gameSelectionService ?? new TestGameSelectionService(),
            recordSpecificationProvider ?? new RecordSpecificationProvider());
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

    /// <summary>
    /// Creates a minimal NPC record for comparison-service tests that exercise height display precision.
    /// </summary>
    /// <param name="fileName">The plugin file name that contributed the test record.</param>
    /// <param name="formKey">The origin form key shared by compared records.</param>
    /// <param name="heightMin">The minimum height value to place on the DTO.</param>
    /// <param name="heightMax">The maximum height value to place on the DTO.</param>
    /// <returns>The populated NPC DTO.</returns>
    private static NPCDTO CreateNPC(string fileName, FormKeyDTO formKey, double heightMin, double heightMax)
    {
        return new NPCDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            FormKey = formKey,
            EditorID = "TestNPC",
            FormVersion = 1,
            MajorRecordFlags = 0,
            ImportedAtUTC = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Aggression = "Unaggressive",
            Confidence = "Average",
            Responsibility = "NoCrime",
            Assistance = "HelpsNobody",
            HeightMin = heightMin,
            HeightMax = heightMax
        };
    }

    private static GlobalDTO CreateGlobal(
        string fileName,
        FormKeyDTO formKey,
        string editorID,
        double data,
        string? mutagenObjectType = null,
        string? majorFlags = null)
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
            MutagenObjectType = mutagenObjectType,
            MajorFlags = majorFlags,
            Data = data
        };
    }

    private static GameSettingDTO CreateGameSetting(
        string fileName,
        FormKeyDTO formKey,
        string editorID,
        GameSettingDataType dataType,
        string? stringData = null,
        double? floatData = null)
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
            DataType = dataType,
            Data = new GameSettingDataDTO
            {
                DataType = dataType,
                String = dataType == GameSettingDataType.String ? Text(stringData ?? string.Empty) : null,
                Float = dataType == GameSettingDataType.Float ? floatData : null
            }
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
            Item = itemFormKey,
            ItemIndex = itemIndex,
            ImportedAtUTC = DateTime.UtcNow
        };
    }

    private static MiscItemDTO CreateMiscItem(string fileName, FormKeyDTO formKey, string name, int value, float weight, FormKeyDTO? featuredItemMessageFormKey)
    {
        return new MiscItemDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            FormKey = formKey,
            EditorID = "MyMiscItem",
            FormVersion = 1,
            MajorRecordFlags = 2,
            ImportedAtUTC = DateTime.UtcNow,
            Name = Text(name),
            ShortName = Text("ShortName"),
            Value = value,
            Weight = weight,
            DirtinessScale = 1,
            FeaturedItemMessage = featuredItemMessageFormKey,
            Flag = "None"
        };
    }

    /// <summary>
    /// Creates a minimal Keyword record for comparison-service tests that exercise scalar metadata dispatch.
    /// </summary>
    /// <param name="fileName">The plugin file name that contributed the test record.</param>
    /// <param name="formKey">The origin FormKey shared by compared records.</param>
    /// <param name="type">The keyword type value to place on the DTO.</param>
    /// <param name="color">The keyword color value to place on the DTO.</param>
    /// <returns>The populated Keyword DTO.</returns>
    private static KeywordDTO CreateKeyword(string fileName, FormKeyDTO formKey, string type, string color)
    {
        return new KeywordDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            FormKey = formKey,
            EditorID = "MyKeyword",
            FormVersion = 1,
            MajorRecordFlags = 2,
            ImportedAtUTC = DateTime.UtcNow,
            Type = type,
            Color = color
        };
    }

    private static MiscItemComponentDTO CreateMiscItemComponent(
        string fileName,
        FormKeyDTO formKey,
        FormKeyDTO componentFormKey,
        int componentIndex,
        int displayIndex,
        int count)
    {
        return new MiscItemComponentDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            FormKey = formKey,
            Component = componentFormKey,
            ComponentIndex = componentIndex,
            DisplayIndex = displayIndex,
            Count = count,
            ImportedAtUTC = DateTime.UtcNow
        };
    }

    private static MiscItemDestructibleDTO CreateMiscItemDestructible(
        FormKeyDTO explosionFormKey,
        int health,
        int destCount,
        string modelFile,
        string modelData)
    {
        return new MiscItemDestructibleDTO
        {
            Data = new MiscItemDestructibleDataDTO
            {
                Health = health,
                DESTCount = destCount
            },
            Stages =
            {
                new MiscItemDestructibleStageDTO
                {
                    StageIndex = 0,
                    HealthPercent = 90,
                    ModelDamageStage = 1,
                    Flags = "CapDamage",
                    SelfDamagePerSecond = 45,
                    Explosion = explosionFormKey,
                    Model = new MiscItemDestructibleStageModelDTO
                    {
                        File = modelFile,
                        Data = modelData
                    }
                }
            }
        };
    }

    private static ModelDTO CreateModel(string fileName, FormKeyDTO formKey, string file)
    {
        return CreateModel(fileName, RecordTypeCatalog.MiscItem.RecordID, formKey, file);
    }

    private static ModelDTO CreateModel(string fileName, string recordType, FormKeyDTO formKey, string file, string? data = null)
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
            Data = data,
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
            ObjectBounds = new ObjectBoundsDTO
            {
                First = "0, 0, 0",
                Second = "1, 1, 1"
            },
            Transforms = new BookTransformsDTO
            {
                Inventory = CreateFormKey("Starfield.esm", 0x999)
            },
            InventoryArt = CreateFormKey("Starfield.esm", 0x998),
            PreviewTransform = CreateFormKey("Starfield.esm", 0x888),
            FeaturedItemMessage = CreateFormKey("Starfield.esm", 0x777),
            XALG = 7,
            Name = Text(name),
            Text = Text(fileName.StartsWith("Base", StringComparison.Ordinal) ? "Base text" : "Patch text"),
            Value = value,
            Weight = 1.25f,
            Flags = "Takeable",
            Teaches = new BookTeachesDTO
            {
                MutagenObjectType = "Skill",
                Perk = CreateFormKey("Starfield.esm", 0x666),
                RawContent = "Piloting"
            },
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

    private static ContainerDTO CreateContainer(
        string fileName,
        FormKeyDTO formKey,
        string name,
        FormKeyDTO? nativeTerminalFormKey,
        IList<ContainerItemDTO> items,
        string? animationGraph = null)
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
            AnimationGraph = animationGraph,
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
            MarkerFlags = markerFlags,
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
            CreatedObjectCount = amountProduced,
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

    private static ReflectionDTO CreateReflection(string fileName, FormKeyDTO formKey, int componentIndex, string componentType, string sourcePath, string refl)
    {
        return new ReflectionDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            RecordType = RecordTypeCatalog.Static.RecordID,
            FormKey = formKey,
            ComponentIndex = componentIndex,
            ComponentType = componentType,
            SourcePath = sourcePath,
            REFL = refl,
            ImportedAtUTC = DateTime.UtcNow
        };
    }

    private static string FormatFormKey(FormKeyDTO formKey)
    {
        return $"{formKey.ModKey.FileName}:{formKey.Id:X8}";
    }

    private static ScriptingAdapterDTO CreateScriptingAdapter(string fileName, FormKeyDTO formKey, string name, string propertyName, string propertyValue)
    {
        return CreateScriptingAdapter(fileName, RecordTypeCatalog.MiscItem.RecordID, formKey, name, propertyName, propertyValue);
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

    private static KeywordMappingDTO CreateKeywordMapping(string fileName, string recordType, FormKeyDTO formKey, FormKeyDTO keywordFormKey, int keywordIndex)
    {
        return new KeywordMappingDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            RecordType = recordType,
            FormKey = formKey,
            KeywordIndex = keywordIndex,
            Keyword = keywordFormKey,
            ImportedAtUTC = DateTime.UtcNow
        };
    }

    private static SoundMappingDTO CreateSoundMapping(string fileName, string recordType, FormKeyDTO formKey, string soundSlot, int soundIndex, string start, string? versioning = null, string? unknown = null)
    {
        return new SoundMappingDTO
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

    private sealed class TestMiscItemRepository : IMiscItemRepository
    {
        public string RecordType => RecordTypeCatalog.MiscItem.RecordID;

        public IReadOnlyList<MiscItemDTO> Records { get; set; } = [];

        public IReadOnlyList<RecordTreeEntryDTO> GetRecordTreeEntriesByPlugin(SupportedGame game, ModKeyDTO modKey)
        {
            return [];
        }

        public IReadOnlyDictionary<string, int> GetRecordPluginCountsByGame(SupportedGame game)
        {
            return new Dictionary<string, int>();
        }

        public IReadOnlyList<MiscItemDTO> GetByFormKey(SupportedGame game, FormKeyDTO formKey)
        {
            return Records;
        }

        public void Save(MiscItemDTO dto)
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

    private sealed class TestKeywordMappingRepository : IKeywordMappingRepository
    {
        public IReadOnlyList<KeywordMappingDTO> Records { get; set; } = [];

        public void Save(KeywordMappingDTO dto)
        { }

        public IReadOnlyList<KeywordMappingDTO> GetByFormKey(SupportedGame game, string recordType, FormKeyDTO formKey)
        {
            return Records;
        }

        public void DeleteByRecord(SupportedGame game, ModKeyDTO modKey, string recordType, FormKeyDTO formKey)
        { }

        public void DeleteStaleByPlugin(SupportedGame game, ModKeyDTO modKey, DateTime importedAtUTC)
        { }
    }

    private sealed class TestSoundMappingRepository : ISoundMappingRepository
    {
        public IReadOnlyList<SoundMappingDTO> Records { get; set; } = [];

        public void Save(SoundMappingDTO dto)
        { }

        public IReadOnlyList<SoundMappingDTO> GetByFormKey(SupportedGame game, string recordType, FormKeyDTO formKey)
        {
            return Records;
        }

        public void DeleteByRecord(SupportedGame game, ModKeyDTO modKey, string recordType, FormKeyDTO formKey)
        { }

        public void DeleteStaleByPlugin(SupportedGame game, ModKeyDTO modKey, DateTime importedAtUTC)
        { }
    }

    private sealed class TestReflectionRepository : IReflectionRepository
    {
        public IReadOnlyList<ReflectionDTO> Records { get; set; } = [];

        public void Save(ReflectionDTO dto)
        { }

        public IReadOnlyList<ReflectionDTO> GetByFormKey(SupportedGame game, string recordType, FormKeyDTO formKey)
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
        public Language RecordTextLanguage { get; set; } = Language.English;

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

        public IReadOnlyList<Language> GetRecordTextLanguages()
        {
            return [RecordTextLanguage];
        }

        public Language GetRecordTextLanguage()
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

    /// <summary>
    /// Provides an isolated record specification set for comparison-service tests.
    /// </summary>
    private sealed class TestRecordSpecificationProvider : IRecordSpecificationProvider
    {
        private readonly IReadOnlyList<RecordSpecification> Specifications;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestRecordSpecificationProvider"/> class.
        /// </summary>
        /// <param name="specifications">The specifications the provider should expose.</param>
        public TestRecordSpecificationProvider(params RecordSpecification[] specifications)
        {
            Specifications = specifications;
        }

        /// <inheritdoc />
        public IReadOnlyList<RecordSpecification> GetAll()
        {
            return Specifications;
        }

        /// <inheritdoc />
        public RecordSpecification? FindByRecordID(string recordID)
        {
            return Specifications.FirstOrDefault(specification =>
                string.Equals(specification.RecordID, recordID, StringComparison.OrdinalIgnoreCase));
        }

        /// <inheritdoc />
        public IReadOnlyList<RecordSpecification> GetSupportedByGame(SpecificationGame game)
        {
            return Specifications
                .Where(specification => specification.GameSupport.Any(support => support.Game == game))
                .ToList();
        }
    }
}
