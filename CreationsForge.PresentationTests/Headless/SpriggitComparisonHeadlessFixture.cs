using CreationsForge.Core.DTOs.Games;
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Records.Interfaces;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Models.Configuration;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services;
using CreationsForge.Core.Services.Interfaces;
using CreationsForge.Fallout4;
using CreationsForge.Skyrim;
using CreationsForge.Starfield;
using Mutagen.Bethesda.Strings;

namespace CreationsForge.PresentationTests.Headless;

public class SpriggitComparisonHeadlessFixture
{
    private readonly Lazy<PluginRecordSetDTO> fallout4RecordSet;
    private readonly Lazy<PluginRecordSetDTO> skyrimRecordSet;
    private readonly Lazy<PluginRecordSetDTO> starfieldRecordSet;
    private readonly Dictionary<string, SpriggitYamlDocument> sampleCache = new(StringComparer.OrdinalIgnoreCase);

    public SpriggitComparisonHeadlessFixture()
    {
        fallout4RecordSet = new Lazy<PluginRecordSetDTO>(() => new Fallout4RecordReaderService(new Fallout4GameMetadataService())
            .ReadPluginRecords(CreatePlugin(SupportedGame.Fallout4, "Fallout4.esm")));
        skyrimRecordSet = new Lazy<PluginRecordSetDTO>(() => new SkyrimRecordReaderService(new SkyrimGameMetadataService())
            .ReadPluginRecords(CreatePlugin(SupportedGame.Skyrim, "Skyrim.esm")));
        starfieldRecordSet = new Lazy<PluginRecordSetDTO>(() => new StarfieldRecordReaderService(new StarfieldGameMetadataService())
            .ReadPluginRecords(CreatePlugin(SupportedGame.Starfield, "Starfield.esm")));
    }

    public ComparisonSample CreateSample(SupportedGame game, string recordType, string folderName, IReadOnlyList<string> requiredPaths, Func<RecordDTO, bool>? recordPredicate = null)
    {
        var sampleRecord = recordPredicate is null
            ? GetSampleRecord(game, recordType, folderName, requiredPaths)
            : GetSampleRecord(game, recordType, folderName, requiredPaths, recordPredicate);
        var sample = sampleRecord.Sample;
        var record = sampleRecord.Record;
        var repository = new InMemoryComparisonRepository(record, recordType);
        var comparisonService = CreateComparisonService(repository);
        var plugin = CreatePlugin(game, record.ModKey.FileName);
        plugin.ModKey = record.ModKey;
        plugin.RecordCount = 1;

        return new ComparisonSample(game, recordType, record, plugin, sample, comparisonService);
    }

    private (SpriggitYamlDocument Sample, RecordDTO Record) GetSampleRecord(SupportedGame game, string recordType, string folderName, IReadOnlyList<string> requiredPaths)
    {
        var sample = GetSample(game, folderName, requiredPaths);
        if (!sample.TryGetFormKey(out var rawFormKey) || string.IsNullOrWhiteSpace(rawFormKey))
        {
            throw new InvalidOperationException($"Spriggit sample '{sample.FilePath}' should contain a FormKey.");
        }

        return (sample, GetRecord(game, recordType, rawFormKey));
    }

    private (SpriggitYamlDocument Sample, RecordDTO Record) GetSampleRecord(SupportedGame game, string recordType, string folderName, IReadOnlyList<string> requiredPaths, Func<RecordDTO, bool> recordPredicate)
    {
        var folderPath = Path.Combine(GetSpriggitRootPath(game), folderName);
        if (!Directory.Exists(folderPath))
        {
            throw new DirectoryNotFoundException($"Spriggit folder '{folderPath}' does not exist.");
        }

        foreach (var filePath in Directory.EnumerateFiles(folderPath, "*.yaml").OrderBy(static path => path, StringComparer.OrdinalIgnoreCase))
        {
            var document = SpriggitYamlDocument.Load(filePath);
            if (!requiredPaths.All(document.HasPath) || !document.TryGetFormKey(out var rawFormKey) || string.IsNullOrWhiteSpace(rawFormKey))
            {
                continue;
            }

            var record = GetRecord(game, recordType, rawFormKey);
            if (recordPredicate(record))
            {
                return (document, record);
            }
        }

        throw new InvalidOperationException($"Unable to find a Spriggit sample in '{folderPath}' with paths '{string.Join(", ", requiredPaths)}' and matching imported record data.");
    }

    private SpriggitYamlDocument GetSample(SupportedGame game, string folderName, IReadOnlyList<string> requiredPaths)
    {
        var cacheKey = $"{game}|{folderName}|{string.Join('|', requiredPaths)}";
        if (sampleCache.TryGetValue(cacheKey, out var cachedDocument))
        {
            return cachedDocument;
        }

        var folderPath = Path.Combine(GetSpriggitRootPath(game), folderName);
        if (!Directory.Exists(folderPath))
        {
            throw new DirectoryNotFoundException($"Spriggit folder '{folderPath}' does not exist.");
        }

        foreach (var filePath in Directory.EnumerateFiles(folderPath, "*.yaml").OrderBy(static path => path, StringComparer.OrdinalIgnoreCase))
        {
            var document = SpriggitYamlDocument.Load(filePath);
            if (requiredPaths.All(document.HasPath))
            {
                sampleCache[cacheKey] = document;
                return document;
            }
        }

        throw new InvalidOperationException($"Unable to find a Spriggit sample in '{folderPath}' with paths '{string.Join(", ", requiredPaths)}'.");
    }

    private RecordDTO GetRecord(SupportedGame game, string recordType, string rawFormKey)
    {
        var expectedFormKey = ParseFormKey(rawFormKey);
        var record = GetRecords(GetRecordSet(game), recordType)
            .FirstOrDefault(candidate => FormKeysMatch(candidate.FormKey, expectedFormKey));

        return record ?? throw new InvalidOperationException(
            $"Unable to find record '{rawFormKey}' for record type '{recordType}' in game '{game}'.");
    }

    private PluginRecordSetDTO GetRecordSet(SupportedGame game)
    {
        return game switch
        {
            SupportedGame.Fallout4 => fallout4RecordSet.Value,
            SupportedGame.Skyrim => skyrimRecordSet.Value,
            SupportedGame.Starfield => starfieldRecordSet.Value,
            _ => throw new InvalidOperationException($"Unsupported game '{game}'.")
        };
    }

    private static IEnumerable<RecordDTO> GetRecords(PluginRecordSetDTO recordSet, string recordType)
    {
        return recordType switch
        {
            "GMST" => recordSet.GameSettings,
            "GLOB" => recordSet.Globals,
            "MISC" => recordSet.MiscObjects,
            "COBJ" => recordSet.ConstructibleObjects,
            "PERK" => recordSet.Perks,
            _ => throw new InvalidOperationException($"Unsupported headless comparison record type '{recordType}'.")
        };
    }

    private static string GetSpriggitRootPath(SupportedGame game)
    {
        return game switch
        {
            SupportedGame.Fallout4 => @"C:\FalloutExtractions\Spriggit\Fallout4.esm",
            SupportedGame.Skyrim => @"C:\SkyrimExtractions\Spriggit\Skyrim.esm",
            SupportedGame.Starfield => @"C:\StarfieldExtractions\Spriggit\Starfield.esm",
            _ => throw new InvalidOperationException($"Unsupported game '{game}'.")
        };
    }

    private static PluginDTO CreatePlugin(SupportedGame game, string fileName)
    {
        return new PluginDTO
        {
            Game = game,
            ModKey = new ModKeyDTO
            {
                Name = Path.GetFileNameWithoutExtension(fileName),
                FileName = fileName,
                Type = 0
            },
            LoadOrderIndex = 0,
            Enabled = true,
            ExistsOnDisk = true,
            ImportState = PluginImportState.Current,
            HeaderFlags = 0,
            FormVersion = 0,
            RecordCount = 0,
            SourceLastWriteUTCTicks = 0,
            SourceFileSizeBytes = 0,
            LastCheckedUTC = DateTime.UtcNow
        };
    }

    private static FormKeyDTO ParseFormKey(string rawFormKey)
    {
        var separatorIndex = rawFormKey.IndexOf(':', StringComparison.Ordinal);
        if (separatorIndex <= 0 || separatorIndex >= rawFormKey.Length - 1)
        {
            throw new FormatException($"Invalid Spriggit FormKey '{rawFormKey}'.");
        }

        var fileName = rawFormKey[(separatorIndex + 1)..];
        return new FormKeyDTO
        {
            Id = Convert.ToUInt32(rawFormKey[..separatorIndex], 16),
            ModKey = new ModKeyDTO
            {
                FileName = fileName,
                Name = Path.GetFileNameWithoutExtension(fileName),
                Type = 0
            }
        };
    }

    private static bool FormKeysMatch(FormKeyDTO left, FormKeyDTO right)
    {
        return left.Id == right.Id &&
               string.Equals(left.ModKey.FileName, right.ModKey.FileName, StringComparison.OrdinalIgnoreCase);
    }

    private static IRecordComparisonService CreateComparisonService(InMemoryComparisonRepository repository)
    {
        return new RecordComparisonService(
            repository,
            repository,
            repository,
            repository,
            repository,
            repository,
            repository,
            repository,
            repository,
            repository,
            repository,
            repository,
            repository,
            repository,
            repository,
            repository,
            repository,
            repository,
            repository,
            repository,
            repository,
            repository,
            repository,
            repository,
            new HeadlessGameSelectionService());
    }

    public sealed record ComparisonSample(
        SupportedGame Game,
        string RecordType,
        RecordDTO Record,
        PluginDTO Plugin,
        SpriggitYamlDocument Spriggit,
        IRecordComparisonService ComparisonService);

    private sealed class InMemoryComparisonRepository :
        IFormListRepository,
        IGameSettingRepository,
        IGlobalRepository,
        IClassRepository,
        IFactionRepository,
        IMiscObjectRepository,
        IKeywordRepository,
        IActorValueInformationRepository,
        INPCRepository,
        IMagicEffectRepository,
        IPerkRepository,
        IStaticRepository,
        IBookRepository,
        IDoorRepository,
        IContainerRepository,
        IConstructibleObjectRepository,
        IConditionFormRepository,
        ITerminalRepository,
        IModelRepository,
        IKeywordMappingRepository,
        IRecordComponentRepository,
        ISoundMappingRepository,
        IScriptingAdapterRepository,
        IRawRecordPayloadRepository,
        IRecordLocalizedStringRepository
    {
        private readonly IReadOnlyList<FormListDTO> formLists = [];
        private readonly IReadOnlyList<GameSettingDTO> gameSettings = [];
        private readonly IReadOnlyList<GlobalDTO> globals = [];
        private readonly IReadOnlyList<ClassDTO> classes = [];
        private readonly IReadOnlyList<FactionDTO> factions = [];
        private readonly IReadOnlyList<MiscObjectDTO> miscObjects = [];
        private readonly IReadOnlyList<KeywordDTO> keywords = [];
        private readonly IReadOnlyList<ActorValueInformationDTO> actorValueInformation = [];
        private readonly IReadOnlyList<NPCDTO> npcs = [];
        private readonly IReadOnlyList<MagicEffectDTO> magicEffects = [];
        private readonly IReadOnlyList<PerkDTO> perks = [];
        private readonly IReadOnlyList<StaticDTO> statics = [];
        private readonly IReadOnlyList<BookDTO> books = [];
        private readonly IReadOnlyList<DoorDTO> doors = [];
        private readonly IReadOnlyList<ContainerDTO> containers = [];
        private readonly IReadOnlyList<ConstructibleObjectDTO> constructibleObjects = [];
        private readonly IReadOnlyList<ConditionFormDTO> conditionForms = [];
        private readonly IReadOnlyList<TerminalDTO> terminals = [];
        private readonly IReadOnlyList<ModelDTO> models = [];
        private readonly IReadOnlyList<KeywordMappingDTO> keywordMappings = [];
        private readonly IReadOnlyList<RecordComponentDTO> recordComponents = [];
        private readonly IReadOnlyList<SoundMappingDTO> soundMappings = [];
        private readonly IReadOnlyList<ScriptingAdapterDTO> scriptingAdapters = [];
        private readonly IReadOnlyList<RawRecordPayloadDTO> rawPayloads = [];
        private readonly IReadOnlyList<LocalizedStringDTO> localizedStrings = [];

        public InMemoryComparisonRepository(RecordDTO record, string recordType)
        {
            switch (recordType)
            {
                case "GMST":
                    gameSettings = [RequireRecord<GameSettingDTO>(record, recordType)];
                    break;
                case "GLOB":
                    globals = [RequireRecord<GlobalDTO>(record, recordType)];
                    break;
                case "CLAS":
                    classes = [RequireRecord<ClassDTO>(record, recordType)];
                    break;
                case "FACT":
                    factions = [RequireRecord<FactionDTO>(record, recordType)];
                    break;
                case "MISC":
                    miscObjects = [RequireRecord<MiscObjectDTO>(record, recordType)];
                    break;
                case "COBJ":
                    constructibleObjects = [RequireRecord<ConstructibleObjectDTO>(record, recordType)];
                    break;
                case "CNDF":
                    conditionForms = [RequireRecord<ConditionFormDTO>(record, recordType)];
                    break;
                case "PERK":
                    perks = [RequireRecord<PerkDTO>(record, recordType)];
                    break;
            }

            if (record is IHasModelsRecordDTO modelRecord)
            {
                models = modelRecord.Models.ToList();
            }

            if (record is IKeywords keywordRecord)
            {
                keywordMappings = keywordRecord.Keywords.ToList();
            }

            if (record is IHasComponentsRecordDTO componentRecord)
            {
                recordComponents = componentRecord.Components.ToList();
            }

            if (record is ISounds soundRecord)
            {
                soundMappings = soundRecord.Sounds.ToList();
            }

            if (record is IHasScriptingAdaptersRecordDTO scriptingAdapterRecord)
            {
                scriptingAdapters = scriptingAdapterRecord.ScriptingAdapters.ToList();
            }

            if (record is IHasRawRecordPayloadsRecordDTO rawPayloadRecord)
            {
                rawPayloads = rawPayloadRecord.RawPayloads.ToList();
            }

            if (record is IHasLocalizedStringsRecordDTO localizedStringRecord)
            {
                localizedStrings = localizedStringRecord.LocalizedStrings.ToList();
            }
        }

        string IRecordTreeRepository.RecordType => string.Empty;

        public IReadOnlyList<RecordTreeEntryDTO> GetRecordTreeEntriesByPlugin(SupportedGame game, ModKeyDTO modKey)
        {
            return [];
        }

        public IReadOnlyDictionary<string, int> GetRecordPluginCountsByGame(SupportedGame game)
        {
            return new Dictionary<string, int>();
        }

        IReadOnlyList<FormListDTO> IFormListRepository.GetByFormKey(SupportedGame game, FormKeyDTO formKey)
        {
            return formLists;
        }

        IReadOnlyList<GameSettingDTO> IGameSettingRepository.GetByFormKey(SupportedGame game, FormKeyDTO formKey)
        {
            return gameSettings;
        }

        IReadOnlyList<GlobalDTO> IGlobalRepository.GetByFormKey(SupportedGame game, FormKeyDTO formKey)
        {
            return globals;
        }

        IReadOnlyList<ClassDTO> IClassRepository.GetByFormKey(SupportedGame game, FormKeyDTO formKey)
        {
            return classes;
        }

        IReadOnlyList<FactionDTO> IFactionRepository.GetByFormKey(SupportedGame game, FormKeyDTO formKey)
        {
            return factions;
        }

        IReadOnlyList<MiscObjectDTO> IMiscObjectRepository.GetByFormKey(SupportedGame game, FormKeyDTO formKey)
        {
            return miscObjects;
        }

        IReadOnlyList<KeywordDTO> IKeywordRepository.GetByFormKey(SupportedGame game, FormKeyDTO formKey)
        {
            return keywords;
        }

        IReadOnlyList<ActorValueInformationDTO> IActorValueInformationRepository.GetByFormKey(SupportedGame game, FormKeyDTO formKey)
        {
            return actorValueInformation;
        }

        IReadOnlyList<NPCDTO> INPCRepository.GetByFormKey(SupportedGame game, FormKeyDTO formKey)
        {
            return npcs;
        }

        IReadOnlyList<MagicEffectDTO> IMagicEffectRepository.GetByFormKey(SupportedGame game, FormKeyDTO formKey)
        {
            return magicEffects;
        }

        IReadOnlyList<PerkDTO> IPerkRepository.GetByFormKey(SupportedGame game, FormKeyDTO formKey)
        {
            return perks;
        }

        IReadOnlyList<StaticDTO> IStaticRepository.GetByFormKey(SupportedGame game, FormKeyDTO formKey)
        {
            return statics;
        }

        IReadOnlyList<BookDTO> IBookRepository.GetByFormKey(SupportedGame game, FormKeyDTO formKey)
        {
            return books;
        }

        IReadOnlyList<DoorDTO> IDoorRepository.GetByFormKey(SupportedGame game, FormKeyDTO formKey)
        {
            return doors;
        }

        IReadOnlyList<ContainerDTO> IContainerRepository.GetByFormKey(SupportedGame game, FormKeyDTO formKey)
        {
            return containers;
        }

        IReadOnlyList<ConstructibleObjectDTO> IConstructibleObjectRepository.GetByFormKey(SupportedGame game, FormKeyDTO formKey)
        {
            return constructibleObjects;
        }

        IReadOnlyList<ConditionFormDTO> IConditionFormRepository.GetByFormKey(SupportedGame game, FormKeyDTO formKey)
        {
            return conditionForms;
        }

        IReadOnlyList<TerminalDTO> ITerminalRepository.GetByFormKey(SupportedGame game, FormKeyDTO formKey)
        {
            return terminals;
        }

        public IReadOnlyList<ModelDTO> GetByFormKey(SupportedGame game, string recordType, FormKeyDTO formKey)
        {
            return models;
        }

        IReadOnlyList<KeywordMappingDTO> IKeywordMappingRepository.GetByFormKey(SupportedGame game, string recordType, FormKeyDTO formKey)
        {
            return keywordMappings;
        }

        IReadOnlyList<RecordComponentDTO> IRecordComponentRepository.GetByFormKey(SupportedGame game, string recordType, FormKeyDTO formKey)
        {
            return recordComponents;
        }

        IReadOnlyList<SoundMappingDTO> ISoundMappingRepository.GetByFormKey(SupportedGame game, string recordType, FormKeyDTO formKey)
        {
            return soundMappings;
        }

        IReadOnlyList<ScriptingAdapterDTO> IScriptingAdapterRepository.GetByFormKey(SupportedGame game, string recordType, FormKeyDTO formKey)
        {
            return scriptingAdapters;
        }

        IReadOnlyList<RawRecordPayloadDTO> IRawRecordPayloadRepository.GetByFormKey(SupportedGame game, string recordType, FormKeyDTO formKey)
        {
            return rawPayloads;
        }

        IReadOnlyList<LocalizedStringDTO> IRecordLocalizedStringRepository.GetByFormKey(SupportedGame game, string recordType, FormKeyDTO formKey)
        {
            return localizedStrings;
        }

        public void Save(FormListDTO dto)
        { }

        public void Save(GameSettingDTO dto)
        { }

        public void Save(GlobalDTO dto)
        { }

        public void Save(ClassDTO dto)
        { }

        public void Save(FactionDTO dto)
        { }

        public void Save(MiscObjectDTO dto)
        { }

        public void Save(KeywordDTO dto)
        { }

        public void Save(ActorValueInformationDTO dto)
        { }

        public void Save(NPCDTO dto)
        { }

        public void Save(MagicEffectDTO dto)
        { }

        public void Save(PerkDTO dto)
        { }

        public void Save(StaticDTO dto)
        { }

        public void Save(BookDTO dto)
        { }

        public void Save(DoorDTO dto)
        { }

        public void Save(ContainerDTO dto)
        { }

        public void Save(ConstructibleObjectDTO dto)
        { }

        public void Save(ConditionFormDTO dto)
        { }

        public void Save(TerminalDTO dto)
        { }

        public void Save(ModelDTO dto)
        { }

        public void Save(KeywordMappingDTO dto)
        { }

        public void Save(RecordComponentDTO dto)
        { }

        public void ReplaceRecordComponents(IHasComponentsRecordDTO record, string recordType)
        { }

        public void Save(SoundMappingDTO dto)
        { }

        public void Save(ScriptingAdapterDTO dto)
        { }

        public void Save(RawRecordPayloadDTO dto)
        { }

        public void Save(LocalizedStringDTO dto)
        { }

        public void DeleteStaleByPlugin(SupportedGame game, ModKeyDTO modKey, DateTime importedAtUTC)
        { }

        public void DeleteByRecord(SupportedGame game, ModKeyDTO modKey, string recordType, FormKeyDTO formKey)
        { }

        private static TRecord RequireRecord<TRecord>(RecordDTO record, string recordType)
            where TRecord : RecordDTO
        {
            if (record is TRecord typedRecord)
            {
                return typedRecord;
            }

            throw new InvalidOperationException($"Record type '{recordType}' resolved to unexpected DTO '{record.GetType().Name}'.");
        }
    }

    private sealed class HeadlessGameSelectionService : IGameSelectionService
    {
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

        public Language GetRecordTextLanguage()
        {
            return Language.English;
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
}
