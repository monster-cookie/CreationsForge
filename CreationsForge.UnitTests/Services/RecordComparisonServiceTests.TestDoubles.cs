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
/// Contains in-memory test doubles for record comparison service tests.
/// </summary>
public partial class RecordComparisonServiceTests
{
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
