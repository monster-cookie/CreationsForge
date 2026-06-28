using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Results;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Importers.Interfaces;
using CreationsForge.Core.Services;
using CreationsForge.Specification.Records;
using Shouldly;

namespace CreationsForge.UnitTests.Services;

public class RecordImportServiceTests
{
    [Fact]
    public void ImportPluginRecords_DiscoversApprovedRecordTypesAndImportsWithRegisteredImporters()
    {
        var plugin = CreatePlugin(SupportedGame.Starfield);
        var formList = CreateFormList(plugin, 10);
        var gameSetting = CreateGameSetting(plugin, 20);
        var global = CreateGlobal(plugin, 30);
        var classRecord = CreateClass(plugin, 40);
        var faction = CreateFaction(plugin, 50);
        var miscItem = CreateMiscItem(plugin, 60);
        var keyword = CreateKeyword(plugin, 70);
        var actorValueInformation = CreateActorValueInformation(plugin, 80);
        var npc = CreateNPC(plugin, 90);
        var magicEffect = CreateMagicEffect(plugin, 100);
        var perk = CreatePerk(plugin, 110);
        var formListImporter = new TestTypedRecordImporter("FLST", "FormLists", CreateSupportedGames(SupportedGame.Starfield), detailRows: 1, formListItems: 2);
        var gameSettingImporter = new TestTypedRecordImporter("GMST", "GameSettings", CreateSupportedGames(SupportedGame.Starfield), detailRows: 1);
        var globalImporter = new TestTypedRecordImporter("GLOB", "Globals", CreateSupportedGames(SupportedGame.Starfield), detailRows: 1);
        var classImporter = new TestTypedRecordImporter("CLAS", "Classes", CreateSupportedGames(SupportedGame.Starfield), detailRows: 1);
        var factionImporter = new TestTypedRecordImporter("FACT", "Factions", CreateSupportedGames(SupportedGame.Starfield), detailRows: 1);
        var miscItemImporter = new TestTypedRecordImporter("MISC", "MiscItems", CreateSupportedGames(SupportedGame.Starfield), detailRows: 1);
        var keywordImporter = new TestTypedRecordImporter("KYWD", "Keywords", CreateSupportedGames(SupportedGame.Starfield), detailRows: 1);
        var actorValueInformationImporter = new TestTypedRecordImporter("AVIF", "ActorValueInformation", CreateSupportedGames(SupportedGame.Starfield), detailRows: 1);
        var npcImporter = new TestTypedRecordImporter("NPC_", "NPCs", CreateSupportedGames(SupportedGame.Starfield), detailRows: 1);
        var magicEffectImporter = new TestTypedRecordImporter("MGEF", "MagicEffects", CreateSupportedGames(SupportedGame.Starfield), detailRows: 1);
        var perkImporter = new TestTypedRecordImporter("PERK", "Perks", CreateSupportedGames(SupportedGame.Starfield), detailRows: 1);
        var service = new RecordImportService([formListImporter, gameSettingImporter, globalImporter, classImporter, factionImporter, miscItemImporter, keywordImporter, actorValueInformationImporter, npcImporter, magicEffectImporter, perkImporter]);

        var result = service.ImportPluginRecords(plugin, new TestGameRecordReader(plugin.Game, [formList], [gameSetting], [global], [classRecord], [faction], [miscItem], [keyword], [actorValueInformation], [npc], [magicEffect], [perk]));

        result.RecordTypes.Select(recordType => recordType.RecordType).ShouldBe(["FLST", "GMST", "GLOB", "CLAS", "FACT", "MISC", "KYWD", "AVIF", "NPC_", "MGEF", "PERK"]);
        result.HeadersImported.ShouldBe(11);
        result.DetailRowsImported.ShouldBe(11);
        result.FormListsImported.ShouldBe(1);
        result.FormListItemsImported.ShouldBe(2);
        result.GameSettingsImported.ShouldBe(1);
        result.GlobalsImported.ShouldBe(1);
        result.RecordsFailed.ShouldBe(0);
        result.UnsupportedRecordTypes.ShouldBe(0);
        formListImporter.StaleCleanupRequests.ShouldBe([plugin]);
        gameSettingImporter.StaleCleanupRequests.ShouldBe([plugin]);
        globalImporter.StaleCleanupRequests.ShouldBe([plugin]);
        classImporter.StaleCleanupRequests.ShouldBe([plugin]);
        factionImporter.StaleCleanupRequests.ShouldBe([plugin]);
        miscItemImporter.StaleCleanupRequests.ShouldBe([plugin]);
        keywordImporter.StaleCleanupRequests.ShouldBe([plugin]);
        actorValueInformationImporter.StaleCleanupRequests.ShouldBe([plugin]);
        npcImporter.StaleCleanupRequests.ShouldBe([plugin]);
        magicEffectImporter.StaleCleanupRequests.ShouldBe([plugin]);
        perkImporter.StaleCleanupRequests.ShouldBe([plugin]);
        formListImporter.ImportedRecords.ShouldBe([formList]);
        gameSettingImporter.ImportedRecords.ShouldBe([gameSetting]);
        globalImporter.ImportedRecords.ShouldBe([global]);
        classImporter.ImportedRecords.ShouldBe([classRecord]);
        factionImporter.ImportedRecords.ShouldBe([faction]);
        miscItemImporter.ImportedRecords.ShouldBe([miscItem]);
        keywordImporter.ImportedRecords.ShouldBe([keyword]);
        actorValueInformationImporter.ImportedRecords.ShouldBe([actorValueInformation]);
        npcImporter.ImportedRecords.ShouldBe([npc]);
        magicEffectImporter.ImportedRecords.ShouldBe([magicEffect]);
        perkImporter.ImportedRecords.ShouldBe([perk]);
    }

    [Fact]
    public void ImportPluginRecords_WhenConditionFormsArePresent_ImportsOptionalStarfieldRecordType()
    {
        var plugin = CreatePlugin(SupportedGame.Starfield);
        var conditionForm = CreateConditionForm(plugin, 100);
        var conditionFormImporter = new TestTypedRecordImporter("CNDF", "ConditionForms", CreateSupportedGames(SupportedGame.Starfield), detailRows: 1);
        var formListImporter = new TestTypedRecordImporter("FLST", "FormLists", CreateSupportedGames(SupportedGame.Starfield));
        var gameSettingImporter = new TestTypedRecordImporter("GMST", "GameSettings", CreateSupportedGames(SupportedGame.Starfield));
        var globalImporter = new TestTypedRecordImporter("GLOB", "Globals", CreateSupportedGames(SupportedGame.Starfield));
        var classImporter = new TestTypedRecordImporter("CLAS", "Classes", CreateSupportedGames(SupportedGame.Starfield));
        var factionImporter = new TestTypedRecordImporter("FACT", "Factions", CreateSupportedGames(SupportedGame.Starfield));
        var miscItemImporter = new TestTypedRecordImporter("MISC", "MiscItems", CreateSupportedGames(SupportedGame.Starfield));
        var keywordImporter = new TestTypedRecordImporter("KYWD", "Keywords", CreateSupportedGames(SupportedGame.Starfield));
        var actorValueInformationImporter = new TestTypedRecordImporter("AVIF", "ActorValueInformation", CreateSupportedGames(SupportedGame.Starfield));
        var npcImporter = new TestTypedRecordImporter("NPC_", "NPCs", CreateSupportedGames(SupportedGame.Starfield));
        var magicEffectImporter = new TestTypedRecordImporter("MGEF", "MagicEffects", CreateSupportedGames(SupportedGame.Starfield));
        var perkImporter = new TestTypedRecordImporter("PERK", "Perks", CreateSupportedGames(SupportedGame.Starfield));
        var service = new RecordImportService([formListImporter, gameSettingImporter, globalImporter, classImporter, factionImporter, miscItemImporter, keywordImporter, actorValueInformationImporter, npcImporter, magicEffectImporter, perkImporter, conditionFormImporter]);

        var result = service.ImportPluginRecords(plugin, new TestGameRecordReader(plugin.Game, [], [], [], conditionForms: [conditionForm]));

        result.RecordTypes.Select(recordType => recordType.RecordType).ShouldContain("CNDF");
        result.RecordTypes.Single(recordType => recordType.RecordType == "CNDF").TypedDetailImportSupported.ShouldBeTrue();
        result.HeadersImported.ShouldBe(1);
        result.DetailRowsImported.ShouldBe(1);
        result.UnsupportedRecordTypes.ShouldBe(0);
        conditionFormImporter.StaleCleanupRequests.ShouldBe([plugin]);
        conditionFormImporter.ImportedRecords.ShouldBe([conditionForm]);
    }

    [Fact]
    public void ImportPluginRecords_WhenImporterIsMissing_MarksRecordTypeUnsupportedAndContinues()
    {
        var plugin = CreatePlugin(SupportedGame.Fallout4);
        var formList = CreateFormList(plugin, 10);
        var gameSetting = CreateGameSetting(plugin, 20);
        var formListImporter = new TestTypedRecordImporter("FLST", "FormLists", CreateSupportedGames(SupportedGame.Fallout4), detailRows: 1);
        var service = new RecordImportService([formListImporter]);

        var result = service.ImportPluginRecords(plugin, new TestGameRecordReader(plugin.Game, [formList], [gameSetting], []));

        result.FormListsImported.ShouldBe(1);
        result.GameSettingsImported.ShouldBe(0);
        result.UnsupportedRecordTypes.ShouldBe(10);
        result.RecordTypes.Single(recordType => recordType.RecordType == "GMST").TypedDetailImportSupported.ShouldBeFalse();
        result.RecordTypes.Single(recordType => recordType.RecordType == "GMST").UnsupportedReason.ShouldNotBeNull().ShouldContain("No typed detail importer");
        result.RecordTypes.Single(recordType => recordType.RecordType == "GLOB").TypedDetailImportSupported.ShouldBeFalse();
        result.RecordTypes.Single(recordType => recordType.RecordType == "CLAS").TypedDetailImportSupported.ShouldBeFalse();
        result.RecordTypes.Single(recordType => recordType.RecordType == "FACT").TypedDetailImportSupported.ShouldBeFalse();
        result.RecordTypes.Single(recordType => recordType.RecordType == "MISC").TypedDetailImportSupported.ShouldBeFalse();
        result.RecordTypes.Single(recordType => recordType.RecordType == "KYWD").TypedDetailImportSupported.ShouldBeFalse();
        result.RecordTypes.Single(recordType => recordType.RecordType == "AVIF").TypedDetailImportSupported.ShouldBeFalse();
        result.RecordTypes.Single(recordType => recordType.RecordType == "NPC_").TypedDetailImportSupported.ShouldBeFalse();
        result.RecordTypes.Single(recordType => recordType.RecordType == "MGEF").TypedDetailImportSupported.ShouldBeFalse();
        result.RecordTypes.Single(recordType => recordType.RecordType == "PERK").TypedDetailImportSupported.ShouldBeFalse();
    }

    /// <summary>
    /// Verifies that record import dispatch reads the injected specification catalog instead of the old hardcoded
    /// record sequence.
    /// </summary>
    [Fact]
    public void ImportPluginRecords_UsesInjectedImportSpecificationsAsDispatchSource()
    {
        var plugin = CreatePlugin(SupportedGame.Starfield);
        var formList = CreateFormList(plugin, 10);
        var gameSetting = CreateGameSetting(plugin, 20);
        var global = CreateGlobal(plugin, 30);
        var globalImporter = new TestTypedRecordImporter("GLOB", "Globals", CreateSupportedGames(SupportedGame.Starfield), detailRows: 1);
        var specificationProvider = new TestRecordSpecificationProvider(CreatePilotImportSpecification("GLOB", "Global", "Globals", "Global", "Globals"));
        var service = new RecordImportService([globalImporter], specificationProvider);

        var result = service.ImportPluginRecords(plugin, new TestGameRecordReader(plugin.Game, [formList], [gameSetting], [global]));

        result.RecordTypes.Select(recordType => recordType.RecordType).ShouldBe(["GLOB"]);
        result.GlobalsImported.ShouldBe(1);
        result.FormListsImported.ShouldBe(0);
        result.GameSettingsImported.ShouldBe(0);
        globalImporter.ImportedRecords.ShouldBe([global]);
    }

    [Fact]
    public void ImportPluginRecords_WhenRecordFails_IncrementsFailureAndContinues()
    {
        var plugin = CreatePlugin(SupportedGame.Skyrim);
        var firstGlobal = CreateGlobal(plugin, 10);
        var secondGlobal = CreateGlobal(plugin, 20);
        var globalImporter = new TestTypedRecordImporter("GLOB", "Globals", CreateSupportedGames(SupportedGame.Skyrim), detailRows: 1, throwOnCall: 1);
        var service = new RecordImportService([globalImporter]);

        var result = service.ImportPluginRecords(plugin, new TestGameRecordReader(plugin.Game, [], [], [firstGlobal, secondGlobal]));

        var globalResult = result.RecordTypes.Single(recordType => recordType.RecordType == "GLOB");
        globalResult.HeadersImported.ShouldBe(2);
        globalResult.DetailRowsImported.ShouldBe(1);
        globalResult.RecordsFailed.ShouldBe(1);
        result.RecordsFailed.ShouldBe(1);
        globalImporter.ImportedRecords.ShouldBe([firstGlobal, secondGlobal]);
        globalImporter.StaleCleanupRequests.ShouldBeEmpty();
    }

    [Fact]
    public void ImportPluginRecords_WhenRecordImportIsCanceled_Rethrows()
    {
        var plugin = CreatePlugin(SupportedGame.Starfield);
        var global = CreateGlobal(plugin, 10);
        var globalImporter = new TestTypedRecordImporter("GLOB", "Globals", CreateSupportedGames(SupportedGame.Starfield), detailRows: 1, throwCancellation: true);
        var service = new RecordImportService([globalImporter]);

        Should.Throw<OperationCanceledException>(() => service.ImportPluginRecords(plugin, new TestGameRecordReader(plugin.Game, [], [], [global])));
    }

    [Fact]
    public void ImportPluginRecords_WhenRecordReaderGameDoesNotMatchPluginGame_Throws()
    {
        var plugin = CreatePlugin(SupportedGame.Starfield);
        var service = new RecordImportService([]);

        Should.Throw<InvalidOperationException>(() => service.ImportPluginRecords(plugin, new TestGameRecordReader(SupportedGame.Fallout4, [], [], [])))
            .Message.ShouldContain("does not match");
    }

    private static PluginDTO CreatePlugin(SupportedGame game)
    {
        return new PluginDTO
        {
            Game = game,
            ModKey = CreateModKey("Test", "Test.esm"),
            LoadOrderIndex = 0,
            Enabled = true,
            ExistsOnDisk = true,
            ImportState = PluginImportState.Current,
            HeaderFlags = 0,
            FormVersion = 1,
            RecordCount = 0,
            SourceLastWriteUTCTicks = 0,
            SourceFileSizeBytes = 0,
            LastCheckedUTC = DateTime.UtcNow
        };
    }

    private static FormListDTO CreateFormList(PluginDTO plugin, uint id)
    {
        return new FormListDTO
        {
            Game = plugin.Game,
            ModKey = plugin.ModKey,
            FormKey = CreateFormKey(plugin.ModKey, id),
            EditorID = $"FLST{id}",
            FormVersion = 1,
            MajorRecordFlags = 0,
            ImportedAtUTC = default
        };
    }

    private static GameSettingDTO CreateGameSetting(PluginDTO plugin, uint id)
    {
        return new GameSettingDTO
        {
            Game = plugin.Game,
            ModKey = plugin.ModKey,
            FormKey = CreateFormKey(plugin.ModKey, id),
            EditorID = $"GMST{id}",
            FormVersion = 1,
            MajorRecordFlags = 0,
            ImportedAtUTC = default
        };
    }

    private static GlobalDTO CreateGlobal(PluginDTO plugin, uint id)
    {
        return new GlobalDTO
        {
            Game = plugin.Game,
            ModKey = plugin.ModKey,
            FormKey = CreateFormKey(plugin.ModKey, id),
            EditorID = $"GLOB{id}",
            FormVersion = 1,
            MajorRecordFlags = 0,
            ImportedAtUTC = default
        };
    }

    private static ClassDTO CreateClass(PluginDTO plugin, uint id)
    {
        return new ClassDTO
        {
            Game = plugin.Game,
            ModKey = plugin.ModKey,
            FormKey = CreateFormKey(plugin.ModKey, id),
            EditorID = $"CLAS{id}",
            FormVersion = 1,
            MajorRecordFlags = 0,
            ImportedAtUTC = default
        };
    }

    private static FactionDTO CreateFaction(PluginDTO plugin, uint id)
    {
        return new FactionDTO
        {
            Game = plugin.Game,
            ModKey = plugin.ModKey,
            FormKey = CreateFormKey(plugin.ModKey, id),
            EditorID = $"FACT{id}",
            FormVersion = 1,
            MajorRecordFlags = 0,
            ImportedAtUTC = default
        };
    }

    private static MiscItemDTO CreateMiscItem(PluginDTO plugin, uint id)
    {
        return new MiscItemDTO
        {
            Game = plugin.Game,
            ModKey = plugin.ModKey,
            FormKey = CreateFormKey(plugin.ModKey, id),
            EditorID = $"MISC{id}",
            FormVersion = 1,
            MajorRecordFlags = 0,
            ImportedAtUTC = default
        };
    }

    private static KeywordDTO CreateKeyword(PluginDTO plugin, uint id)
    {
        return new KeywordDTO
        {
            Game = plugin.Game,
            ModKey = plugin.ModKey,
            FormKey = CreateFormKey(plugin.ModKey, id),
            EditorID = $"KYWD{id}",
            FormVersion = 1,
            MajorRecordFlags = 0,
            ImportedAtUTC = default,
            Color = string.Empty,
            Type = string.Empty
        };
    }

    private static ActorValueInformationDTO CreateActorValueInformation(PluginDTO plugin, uint id)
    {
        return new ActorValueInformationDTO
        {
            Game = plugin.Game,
            ModKey = plugin.ModKey,
            FormKey = CreateFormKey(plugin.ModKey, id),
            EditorID = $"AVIF{id}",
            FormVersion = 1,
            MajorRecordFlags = 0,
            ImportedAtUTC = default
        };
    }

    private static NPCDTO CreateNPC(PluginDTO plugin, uint id)
    {
        return new NPCDTO
        {
            Game = plugin.Game,
            ModKey = plugin.ModKey,
            FormKey = CreateFormKey(plugin.ModKey, id),
            EditorID = $"NPC_{id}",
            FormVersion = 1,
            MajorRecordFlags = 0,
            ImportedAtUTC = default,
            Aggression = string.Empty,
            Confidence = string.Empty,
            Responsibility = string.Empty,
            Assistance = string.Empty
        };
    }

    private static MagicEffectDTO CreateMagicEffect(PluginDTO plugin, uint id)
    {
        return new MagicEffectDTO
        {
            Game = plugin.Game,
            ModKey = plugin.ModKey,
            FormKey = CreateFormKey(plugin.ModKey, id),
            EditorID = $"MGEF{id}",
            FormVersion = 1,
            MajorRecordFlags = 0,
            ImportedAtUTC = default,
            Flags = string.Empty
        };
    }

    private static PerkDTO CreatePerk(PluginDTO plugin, uint id)
    {
        return new PerkDTO
        {
            Game = plugin.Game,
            ModKey = plugin.ModKey,
            FormKey = CreateFormKey(plugin.ModKey, id),
            EditorID = $"PERK{id}",
            FormVersion = 1,
            MajorRecordFlags = 0,
            ImportedAtUTC = default,
            Flags = string.Empty
        };
    }

    private static ConditionFormDTO CreateConditionForm(PluginDTO plugin, uint id)
    {
        return new ConditionFormDTO
        {
            Game = plugin.Game,
            ModKey = plugin.ModKey,
            FormKey = CreateFormKey(plugin.ModKey, id),
            EditorID = $"CNDF{id}",
            FormVersion = 1,
            MajorRecordFlags = 0,
            ImportedAtUTC = default,
            Version2 = 1
        };
    }

    private static FormKeyDTO CreateFormKey(ModKeyDTO modKey, uint id)
    {
        return new FormKeyDTO
        {
            ModKey = modKey,
            Id = id
        };
    }

    private static ModKeyDTO CreateModKey(string name, string fileName)
    {
        return new ModKeyDTO
        {
            Name = name,
            Type = 0,
            FileName = fileName
        };
    }

    private static IReadOnlySet<SupportedGame> CreateSupportedGames(params SupportedGame[] games)
    {
        return new HashSet<SupportedGame>(games);
    }

    /// <summary>
    /// Creates a minimal record specification for import-dispatch tests.
    /// </summary>
    /// <param name="recordID">The Bethesda record identifier used to resolve a typed importer.</param>
    /// <param name="recordType">The canonical record type name stored in import results.</param>
    /// <param name="tableName">The detail table name stored in import results.</param>
    /// <param name="friendlyName">The human-readable record type name.</param>
    /// <param name="pluginRecordSetPropertyName">The record-set property containing mapped DTOs for the record type.</param>
    /// <returns>The test record specification.</returns>
    private static RecordSpecification CreatePilotImportSpecification(
        string recordID,
        string recordType,
        string tableName,
        string friendlyName,
        string pluginRecordSetPropertyName)
    {
        return new RecordSpecification
        {
            RecordID = recordID,
            RecordType = recordType,
            TableName = tableName,
            FriendlyName = friendlyName,
            Import = new RecordImportSpecification
            {
                PluginRecordSetPropertyName = pluginRecordSetPropertyName,
                ImportOrder = 0,
                IsRequired = true
            }
        };
    }

    private sealed class TestGameRecordReader : IGameRecordReader
    {
        private readonly IReadOnlyList<FormListDTO> FormLists;
        private readonly IReadOnlyList<GameSettingDTO> GameSettings;
        private readonly IReadOnlyList<GlobalDTO> Globals;
        private readonly IReadOnlyList<ClassDTO> Classes;
        private readonly IReadOnlyList<FactionDTO> Factions;
        private readonly IReadOnlyList<MiscItemDTO> MiscItems;
        private readonly IReadOnlyList<KeywordDTO> Keywords;
        private readonly IReadOnlyList<ActorValueInformationDTO> ActorValueInformation;
        private readonly IReadOnlyList<NPCDTO> NPCs;
        private readonly IReadOnlyList<MagicEffectDTO> MagicEffects;
        private readonly IReadOnlyList<PerkDTO> Perks;
        private readonly IReadOnlyList<ConditionFormDTO> ConditionForms;

        public TestGameRecordReader(
            SupportedGame game,
            IReadOnlyList<FormListDTO> formLists,
            IReadOnlyList<GameSettingDTO> gameSettings,
            IReadOnlyList<GlobalDTO> globals,
            IReadOnlyList<ClassDTO>? classes = null,
            IReadOnlyList<FactionDTO>? factions = null,
            IReadOnlyList<MiscItemDTO>? miscItems = null,
            IReadOnlyList<KeywordDTO>? keywords = null,
            IReadOnlyList<ActorValueInformationDTO>? actorValueInformation = null,
            IReadOnlyList<NPCDTO>? npcs = null,
            IReadOnlyList<MagicEffectDTO>? magicEffects = null,
            IReadOnlyList<PerkDTO>? perks = null,
            IReadOnlyList<ConditionFormDTO>? conditionForms = null)
        {
            Game = game;
            FormLists = formLists;
            GameSettings = gameSettings;
            Globals = globals;
            Classes = classes ?? [];
            Factions = factions ?? [];
            MiscItems = miscItems ?? [];
            Keywords = keywords ?? [];
            ActorValueInformation = actorValueInformation ?? [];
            NPCs = npcs ?? [];
            MagicEffects = magicEffects ?? [];
            Perks = perks ?? [];
            ConditionForms = conditionForms ?? [];
        }

        public SupportedGame Game { get; }

        public PluginRecordSetDTO ReadPluginRecords(PluginDTO plugin, CancellationToken cancellationToken = default)
        {
            return new PluginRecordSetDTO
            {
                FormLists = FormLists,
                GameSettings = GameSettings,
                Globals = Globals,
                Classes = Classes,
                Factions = Factions,
                MiscItems = MiscItems,
                Keywords = Keywords,
                ActorValueInformation = ActorValueInformation,
                NPCs = NPCs,
                MagicEffects = MagicEffects,
                Perks = Perks,
                ConditionForms = ConditionForms
            };
        }
    }

    private sealed class TestTypedRecordImporter : ITypedRecordImporter
    {
        private readonly int DetailRows;
        private readonly int FormListItems;
        private readonly int? ThrowOnCall;
        private readonly bool ThrowCancellation;
        private int CallCount;

        public TestTypedRecordImporter(string recordType, string tableName, IReadOnlySet<SupportedGame> supportedGames, int detailRows = 0, int formListItems = 0, int? throwOnCall = null, bool throwCancellation = false)
        {
            RecordType = recordType;
            TableName = tableName;
            SupportedGames = supportedGames;
            DetailRows = detailRows;
            FormListItems = formListItems;
            ThrowOnCall = throwOnCall;
            ThrowCancellation = throwCancellation;
        }

        public string RecordType { get; }

        public string TableName { get; }

        public IReadOnlySet<SupportedGame> SupportedGames { get; }

        public IList<object> ImportedRecords { get; } = new List<object>();

        public IList<PluginDTO> StaleCleanupRequests { get; } = new List<PluginDTO>();

        public void Import(object recordDTO, RecordTypeImportResultDTO result, DateTime importedAtUTC)
        {
            CallCount++;
            ImportedRecords.Add(recordDTO);

            if (ThrowCancellation)
            {
                throw new OperationCanceledException();
            }

            if (ThrowOnCall == CallCount)
            {
                throw new InvalidOperationException("Import failed.");
            }

            result.DetailRowsImported += DetailRows;
            result.FormListItemsImported += FormListItems;
        }

        public void DeleteStaleRecords(PluginDTO plugin, DateTime importedAtUTC)
        {
            StaleCleanupRequests.Add(plugin);
        }
    }

    /// <summary>
    /// Provides an isolated record specification set for import-service tests.
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
