using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services;
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
        TestMiscObjectRepository? miscObjectRepository = null,
        TestKeywordRepository? keywordRepository = null,
        TestActorValueInformationRepository? actorValueInformationRepository = null,
        TestNPCRepository? npcRepository = null,
        TestMagicEffectRepository? magicEffectRepository = null,
        TestPerkRepository? perkRepository = null,
        TestModelRepository? modelRepository = null,
        TestRecordKeywordRepository? recordKeywordRepository = null,
        TestRecordSoundRepository? recordSoundRepository = null,
        TestScriptingAdapterRepository? scriptingAdapterRepository = null)
    {
        return new RecordComparisonService(
            formListRepository ?? new TestFormListRepository(),
            gameSettingRepository ?? new TestGameSettingRepository(),
            globalRepository ?? new TestGlobalRepository(),
            miscObjectRepository ?? new TestMiscObjectRepository(),
            keywordRepository ?? new TestKeywordRepository(),
            actorValueInformationRepository ?? new TestActorValueInformationRepository(),
            npcRepository ?? new TestNPCRepository(),
            magicEffectRepository ?? new TestMagicEffectRepository(),
            perkRepository ?? new TestPerkRepository(),
            modelRepository ?? new TestModelRepository(),
            recordKeywordRepository ?? new TestRecordKeywordRepository(),
            recordSoundRepository ?? new TestRecordSoundRepository(),
            scriptingAdapterRepository ?? new TestScriptingAdapterRepository());
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
            Name = name,
            ShortName = "ShortName",
            Value = value,
            Weight = weight,
            DirtinessScale = 1,
            FeaturedItemMessageFormKey = featuredItemMessageFormKey,
            Flag = "None"
        };
    }

    private static ModelDTO CreateModel(string fileName, FormKeyDTO formKey, string file)
    {
        return new ModelDTO
        {
            Game = SupportedGame.Starfield,
            ModKey = CreateModKey(fileName),
            RecordType = RecordTypeCatalog.MiscObject.RecordID,
            FormKey = formKey,
            ModelSlot = "Model",
            ModelGender = string.Empty,
            File = file,
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
            Name = name,
            Archetype = archetype,
            UnknownInt2 = unknownInt2,
            Flags = "None"
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
