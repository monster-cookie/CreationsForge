using CreationsForge.Core.DTOs.Plugins;
using System.Globalization;
using System.Reflection;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Records.Interfaces;
using CreationsForge.Core.DTOs.Records.Metadata;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services.Interfaces;
using CreationsForge.Core.Utilities;
using CreationsForge.Specification.Records;
using Mutagen.Bethesda.Strings;

namespace CreationsForge.Core.Services;

/// <summary>
/// Builds UI-neutral comparison DTOs from imported record readback data.
/// </summary>
public class RecordComparisonService : IRecordComparisonService
{
    private const string UnparseableReflectionDataLabel = "[UNPARSEABLE REFLECTION DATA]";
    private readonly IFormListRepository FormListRepository;
    private readonly IGameSettingRepository GameSettingRepository;
    private readonly IGlobalRepository GlobalRepository;
    private readonly IClassRepository ClassRepository;
    private readonly IFactionRepository FactionRepository;
    private readonly IMiscItemRepository MiscItemRepository;
    private readonly IKeywordRepository KeywordRepository;
    private readonly IActorValueInformationRepository ActorValueInformationRepository;
    private readonly INPCRepository NPCRepository;
    private readonly IMagicEffectRepository MagicEffectRepository;
    private readonly IPerkRepository PerkRepository;
    private readonly IStaticRepository StaticRepository;
    private readonly IBookRepository BookRepository;
    private readonly IDoorRepository DoorRepository;
    private readonly IContainerRepository ContainerRepository;
    private readonly IConstructibleObjectRepository ConstructibleObjectRepository;
    private readonly IConditionFormRepository ConditionFormRepository;
    private readonly ITerminalRepository TerminalRepository;
    private readonly IModelRepository ModelRepository;
    private readonly IKeywordMappingRepository KeywordMappingRepository;
    private readonly ISoundMappingRepository SoundMappingRepository;
    private readonly IScriptingAdapterRepository ScriptingAdapterRepository;
    private readonly IReflectionRepository ReflectionRepository;
    private readonly IRecordLocalizedStringRepository RecordLocalizedStringRepository;
    private readonly IGameSelectionService GameSelectionService;
    private readonly IRecordSpecificationProvider RecordSpecificationProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordComparisonService"/> class.
    /// </summary>
    /// <param name="formListRepository">The repository used to read imported Form List records.</param>
    /// <param name="gameSettingRepository">The repository used to read imported Game Setting records.</param>
    /// <param name="globalRepository">The repository used to read imported Global records.</param>
    /// <param name="classRepository">The repository used to read imported Class records.</param>
    /// <param name="factionRepository">The repository used to read imported Faction records.</param>
    /// <param name="miscItemRepository">The repository used to read imported Misc Item records.</param>
    /// <param name="keywordRepository">The repository used to read imported Keyword records.</param>
    /// <param name="actorValueInformationRepository">The repository used to read imported Actor Value Information records.</param>
    /// <param name="npcRepository">The repository used to read imported NPC records.</param>
    /// <param name="magicEffectRepository">The repository used to read imported Magic Effect records.</param>
    /// <param name="perkRepository">The repository used to read imported Perk records.</param>
    /// <param name="staticRepository">The repository used to read imported Static records.</param>
    /// <param name="bookRepository">The repository used to read imported Book records.</param>
    /// <param name="doorRepository">The repository used to read imported Door records.</param>
    /// <param name="containerRepository">The repository used to read imported Container records.</param>
    /// <param name="constructibleObjectRepository">The repository used to read imported Constructible Object records.</param>
    /// <param name="conditionFormRepository">The repository used to read imported Condition Form records.</param>
    /// <param name="terminalRepository">The repository used to read imported Terminal records.</param>
    /// <param name="modelRepository">The repository used to read shared model child rows.</param>
    /// <param name="keywordMappingRepository">The repository used to read shared keyword child rows.</param>
    /// <param name="soundMappingRepository">The repository used to read shared sound child rows.</param>
    /// <param name="scriptingAdapterRepository">The repository used to read shared scripting adapter rows.</param>
    /// <param name="reflectionRepository">The repository used to read shared reflection payload rows.</param>
    /// <param name="recordLocalizedStringRepository">The repository used to read localized record text rows.</param>
    /// <param name="gameSelectionService">The service that provides display preferences such as record text language.</param>
    /// <param name="recordSpecificationProvider">The optional provider for record comparison specifications.</param>
    public RecordComparisonService(
        IFormListRepository formListRepository,
        IGameSettingRepository gameSettingRepository,
        IGlobalRepository globalRepository,
        IClassRepository classRepository,
        IFactionRepository factionRepository,
        IMiscItemRepository miscItemRepository,
        IKeywordRepository keywordRepository,
        IActorValueInformationRepository actorValueInformationRepository,
        INPCRepository npcRepository,
        IMagicEffectRepository magicEffectRepository,
        IPerkRepository perkRepository,
        IStaticRepository staticRepository,
        IBookRepository bookRepository,
        IDoorRepository doorRepository,
        IContainerRepository containerRepository,
        IConstructibleObjectRepository constructibleObjectRepository,
        IConditionFormRepository conditionFormRepository,
        ITerminalRepository terminalRepository,
        IModelRepository modelRepository,
        IKeywordMappingRepository keywordMappingRepository,
        ISoundMappingRepository soundMappingRepository,
        IScriptingAdapterRepository scriptingAdapterRepository,
        IReflectionRepository reflectionRepository,
        IRecordLocalizedStringRepository recordLocalizedStringRepository,
        IGameSelectionService gameSelectionService,
        IRecordSpecificationProvider? recordSpecificationProvider = null)
    {
        FormListRepository = formListRepository;
        GameSettingRepository = gameSettingRepository;
        GlobalRepository = globalRepository;
        ClassRepository = classRepository;
        FactionRepository = factionRepository;
        MiscItemRepository = miscItemRepository;
        KeywordRepository = keywordRepository;
        ActorValueInformationRepository = actorValueInformationRepository;
        NPCRepository = npcRepository;
        MagicEffectRepository = magicEffectRepository;
        PerkRepository = perkRepository;
        StaticRepository = staticRepository;
        BookRepository = bookRepository;
        DoorRepository = doorRepository;
        ContainerRepository = containerRepository;
        ConstructibleObjectRepository = constructibleObjectRepository;
        ConditionFormRepository = conditionFormRepository;
        TerminalRepository = terminalRepository;
        ModelRepository = modelRepository;
        KeywordMappingRepository = keywordMappingRepository;
        SoundMappingRepository = soundMappingRepository;
        ScriptingAdapterRepository = scriptingAdapterRepository;
        ReflectionRepository = reflectionRepository;
        RecordLocalizedStringRepository = recordLocalizedStringRepository;
        GameSelectionService = gameSelectionService;
        RecordSpecificationProvider = recordSpecificationProvider ?? new RecordSpecificationProvider();
    }

    public RecordComparisonDTO GetRecordComparison(SupportedGame game, string recordType, FormKeyDTO formKey)
    {
        if (recordType == RecordTypeCatalog.FormList.RecordID)
        {
            return CreateFormListComparison(game, formKey);
        }

        if (recordType == RecordTypeCatalog.GameSetting.RecordID)
        {
            return CreateGameSettingComparison(game, formKey);
        }

        if (recordType == RecordTypeCatalog.Global.RecordID)
        {
            return CreateGlobalComparison(game, formKey);
        }

        if (recordType == RecordTypeCatalog.Class.RecordID)
        {
            return CreateClassComparison(game, formKey);
        }

        if (recordType == RecordTypeCatalog.Faction.RecordID)
        {
            return CreateFactionComparison(game, formKey);
        }

        if (recordType == RecordTypeCatalog.MiscItem.RecordID)
        {
            return CreateMiscItemComparison(game, formKey);
        }

        if (recordType == RecordTypeCatalog.Keyword.RecordID)
        {
            return CreateKeywordComparison(game, formKey);
        }

        if (recordType == RecordTypeCatalog.ActorValueInformation.RecordID)
        {
            return CreateActorValueInformationComparison(game, formKey);
        }

        if (recordType == RecordTypeCatalog.NPC.RecordID)
        {
            return CreateNPCComparison(game, formKey);
        }

        if (recordType == RecordTypeCatalog.MagicEffect.RecordID)
        {
            return CreateMagicEffectComparison(game, formKey);
        }

        if (recordType == RecordTypeCatalog.Perk.RecordID)
        {
            return CreatePerkComparison(game, formKey);
        }

        if (recordType == RecordTypeCatalog.Static.RecordID)
        {
            return CreateStaticComparison(game, formKey);
        }

        if (recordType == RecordTypeCatalog.Book.RecordID)
        {
            return CreateBookComparison(game, formKey);
        }

        if (recordType == RecordTypeCatalog.Door.RecordID)
        {
            return CreateDoorComparison(game, formKey);
        }

        if (recordType == RecordTypeCatalog.Container.RecordID)
        {
            return CreateContainerComparison(game, formKey);
        }

        if (recordType == RecordTypeCatalog.ConstructibleObject.RecordID)
        {
            return CreateConstructibleObjectComparison(game, formKey);
        }

        if (recordType == RecordTypeCatalog.ConditionForm.RecordID)
        {
            return CreateConditionFormComparison(game, formKey);
        }

        if (recordType == RecordTypeCatalog.Terminal.RecordID)
        {
            return CreateTerminalComparison(game, formKey);
        }

        return new RecordComparisonDTO
        {
            RecordType = recordType,
            FormKey = formKey
        };
    }

    private RecordComparisonDTO CreateFormListComparison(SupportedGame game, FormKeyDTO formKey)
    {
        var records = FormListRepository.GetByFormKey(game, formKey);
        var maxItemCount = records
            .Select(record => record.Items.Count)
            .DefaultIfEmpty()
            .Max();
        var fields = CreateSpecComparisonFields(RecordTypeCatalog.FormList.RecordID, records);
        for (var itemIndex = 0; itemIndex < maxItemCount; itemIndex++)
        {
            var currentIndex = itemIndex;
            fields.Add(CreateField($"Items[{itemIndex}]", records, record => FormatFormKey(record.Items.FirstOrDefault(item => item.ItemIndex == currentIndex)?.Item)));
        }

        return CreateComparison(RecordTypeCatalog.FormList.RecordID, formKey, records.Cast<RecordDTO>().ToList(), fields);
    }

    private RecordComparisonDTO CreateGameSettingComparison(SupportedGame game, FormKeyDTO formKey)
    {
        var records = GameSettingRepository.GetByFormKey(game, formKey);
        var localizedStrings = RecordLocalizedStringRepository.GetByFormKey(game, RecordTypeCatalog.GameSetting.RecordID, formKey);
        var recordTextLanguage = GameSelectionService.GetRecordTextLanguage();
        var fields = CreateSpecComparisonFields(
            RecordTypeCatalog.GameSetting.RecordID,
            records,
            new Dictionary<string, Func<GameSettingDTO, string>>(StringComparer.Ordinal)
            {
                ["Data"] = record => GetGameSettingDisplayValue(localizedStrings, record, recordTextLanguage)
            });

        return CreateComparison(RecordTypeCatalog.GameSetting.RecordID, formKey, records.Cast<RecordDTO>().ToList(), fields);
    }

    private RecordComparisonDTO CreateGlobalComparison(SupportedGame game, FormKeyDTO formKey)
    {
        var records = GlobalRepository.GetByFormKey(game, formKey);
        var fields = CreateSpecComparisonFields(RecordTypeCatalog.Global.RecordID, records);

        return CreateComparison(RecordTypeCatalog.Global.RecordID, formKey, records.Cast<RecordDTO>().ToList(), fields);
    }

    /// <summary>
    /// Creates the comparison output for imported Class overrides, using specification metadata for scalar parent rows
    /// while leaving class properties and weight groups on existing strategy code.
    /// </summary>
    /// <param name="game">The game whose imported class records should be compared.</param>
    /// <param name="formKey">The origin FormKey shared by the class overrides.</param>
    /// <returns>The class comparison DTO consumed by presentation rendering.</returns>
    private RecordComparisonDTO CreateClassComparison(SupportedGame game, FormKeyDTO formKey)
    {
        var records = ClassRepository.GetByFormKey(game, formKey);
        var localizedStrings = RecordLocalizedStringRepository.GetByFormKey(game, RecordTypeCatalog.Class.RecordID, formKey);
        var recordTextLanguage = GameSelectionService.GetRecordTextLanguage();
        var fields = CreateSpecComparisonFields(
            RecordTypeCatalog.Class.RecordID,
            records,
            localizedStrings: localizedStrings,
            recordTextLanguage: recordTextLanguage);
        AddClassPropertyGroups(fields, records);
        AddClassWeightGroups(fields, records, "Skill", "SkillWeights");
        AddClassWeightGroups(fields, records, "Stat", "StatWeights");

        return CreateComparison(RecordTypeCatalog.Class.RecordID, formKey, records.Cast<RecordDTO>().ToList(), fields);
    }

    /// <summary>
    /// Creates the comparison output for imported Faction overrides, using specification metadata for scalar parent
    /// rows while leaving relations, ranks, conditions, components, and keyword rows on existing strategy code.
    /// </summary>
    /// <param name="game">The game whose imported faction records should be compared.</param>
    /// <param name="formKey">The origin FormKey shared by the faction overrides.</param>
    /// <returns>The faction comparison DTO consumed by presentation rendering.</returns>
    private RecordComparisonDTO CreateFactionComparison(SupportedGame game, FormKeyDTO formKey)
    {
        var records = FactionRepository.GetByFormKey(game, formKey);
        var localizedStrings = RecordLocalizedStringRepository.GetByFormKey(game, RecordTypeCatalog.Faction.RecordID, formKey);
        var recordTextLanguage = GameSelectionService.GetRecordTextLanguage();
        var fields = CreateSpecComparisonFields(
            RecordTypeCatalog.Faction.RecordID,
            records,
            localizedStrings: localizedStrings,
            recordTextLanguage: recordTextLanguage);
        AddFactionRelationGroups(fields, records);
        AddFactionRankGroups(fields, records, localizedStrings, recordTextLanguage);
        AddSpecComparisonChildGroups(
            fields,
            game,
            RecordTypeCatalog.Faction.RecordID,
            formKey,
            records.Cast<RecordDTO>().ToList(),
            RecordComparisonChildGroupKind.ConditionRules);
        AddSpecComparisonChildGroups(
            fields,
            game,
            RecordTypeCatalog.Faction.RecordID,
            formKey,
            records.Cast<RecordDTO>().ToList(),
            RecordComparisonChildGroupKind.RecordComponents);
        AddSpecComparisonChildGroups(
            fields,
            game,
            RecordTypeCatalog.Faction.RecordID,
            formKey,
            records.Cast<RecordDTO>().ToList(),
            RecordComparisonChildGroupKind.KeywordMappings);

        return CreateComparison(RecordTypeCatalog.Faction.RecordID, formKey, records.Cast<RecordDTO>().ToList(), fields);
    }

    /// <summary>
    /// Creates the comparison output for imported Misc Item overrides, using specification metadata for scalar parent
    /// rows while leaving destructible and shared child collections on existing strategy code.
    /// </summary>
    /// <param name="game">The game whose imported misc item records should be compared.</param>
    /// <param name="formKey">The origin FormKey shared by the misc item overrides.</param>
    /// <returns>The misc item comparison DTO consumed by presentation rendering.</returns>
    private RecordComparisonDTO CreateMiscItemComparison(SupportedGame game, FormKeyDTO formKey)
    {
        var records = MiscItemRepository.GetByFormKey(game, formKey);
        var localizedStrings = RecordLocalizedStringRepository.GetByFormKey(game, RecordTypeCatalog.MiscItem.RecordID, formKey);
        var recordTextLanguage = GameSelectionService.GetRecordTextLanguage();
        var fields = CreateSpecComparisonFields(
            RecordTypeCatalog.MiscItem.RecordID,
            records,
            localizedStrings: localizedStrings,
            recordTextLanguage: recordTextLanguage);
        AddMiscItemDestructibleGroups(fields, records);
        AddSpecComparisonChildGroups(
            fields,
            game,
            RecordTypeCatalog.MiscItem.RecordID,
            formKey,
            records.Cast<RecordDTO>().ToList(),
            RecordComparisonChildGroupKind.KeywordMappings);
        AddSpecComparisonChildGroups(
            fields,
            game,
            RecordTypeCatalog.MiscItem.RecordID,
            formKey,
            records.Cast<RecordDTO>().ToList(),
            RecordComparisonChildGroupKind.ModelMappings);
        AddSpecComparisonChildGroups(
            fields,
            game,
            RecordTypeCatalog.MiscItem.RecordID,
            formKey,
            records.Cast<RecordDTO>().ToList(),
            RecordComparisonChildGroupKind.SoundMappings);
        AddSpecComparisonChildGroups(
            fields,
            game,
            RecordTypeCatalog.MiscItem.RecordID,
            formKey,
            records.Cast<RecordDTO>().ToList(),
            RecordComparisonChildGroupKind.ScriptingAdapterMappings);
        AddMiscItemComponentGroups(fields, records);
        AddMiscItemResourceGroups(fields, records);

        return CreateComparison(RecordTypeCatalog.MiscItem.RecordID, formKey, records.Cast<RecordDTO>().ToList(), fields);
    }

    /// <summary>
    /// Creates the comparison output for imported Keyword overrides, using specification metadata for scalar parent
    /// rows while preserving localized name display behavior.
    /// </summary>
    /// <param name="game">The game whose imported keyword records should be compared.</param>
    /// <param name="formKey">The origin FormKey shared by the keyword overrides.</param>
    /// <returns>The keyword comparison DTO consumed by presentation rendering.</returns>
    private RecordComparisonDTO CreateKeywordComparison(SupportedGame game, FormKeyDTO formKey)
    {
        var records = KeywordRepository.GetByFormKey(game, formKey);
        var localizedStrings = RecordLocalizedStringRepository.GetByFormKey(game, RecordTypeCatalog.Keyword.RecordID, formKey);
        var recordTextLanguage = GameSelectionService.GetRecordTextLanguage();
        var fields = CreateSpecComparisonFields(
            RecordTypeCatalog.Keyword.RecordID,
            records,
            localizedStrings: localizedStrings,
            recordTextLanguage: recordTextLanguage);

        return CreateComparison(RecordTypeCatalog.Keyword.RecordID, formKey, records.Cast<RecordDTO>().ToList(), fields);
    }

    /// <summary>
    /// Creates the comparison output for imported Actor Value Information overrides, using specification metadata for
    /// scalar parent rows while leaving perk-tree rows on existing strategy code.
    /// </summary>
    /// <param name="game">The game whose imported actor value information records should be compared.</param>
    /// <param name="formKey">The origin FormKey shared by the actor value information overrides.</param>
    /// <returns>The actor value information comparison DTO consumed by presentation rendering.</returns>
    private RecordComparisonDTO CreateActorValueInformationComparison(SupportedGame game, FormKeyDTO formKey)
    {
        var records = ActorValueInformationRepository.GetByFormKey(game, formKey);
        var localizedStrings = RecordLocalizedStringRepository.GetByFormKey(game, RecordTypeCatalog.ActorValueInformation.RecordID, formKey);
        var recordTextLanguage = GameSelectionService.GetRecordTextLanguage();
        var fields = CreateSpecComparisonFields(
            RecordTypeCatalog.ActorValueInformation.RecordID,
            records,
            localizedStrings: localizedStrings,
            recordTextLanguage: recordTextLanguage);
        AddActorValueInformationPerkTreeGroups(fields, records);

        return CreateComparison(RecordTypeCatalog.ActorValueInformation.RecordID, formKey, records.Cast<RecordDTO>().ToList(), fields);
    }

    /// <summary>
    /// Creates a comparison tree for NPC records, including root actor data and first-class child rows imported for
    /// head parts, morphs, tints, packages, and other NPC-specific collections.
    /// </summary>
    /// <param name="game">The game whose NPC records are being compared.</param>
    /// <param name="formKey">The NPC form key to compare across loaded plugins.</param>
    /// <returns>A comparison DTO ready for the record comparison UI.</returns>
    private RecordComparisonDTO CreateNPCComparison(SupportedGame game, FormKeyDTO formKey)
    {
        var records = NPCRepository.GetByFormKey(game, formKey);
        var localizedStrings = RecordLocalizedStringRepository.GetByFormKey(game, RecordTypeCatalog.NPC.RecordID, formKey);
        var recordTextLanguage = GameSelectionService.GetRecordTextLanguage();
        var baseRecords = records.Cast<RecordDTO>().ToList();
        var preLevelFieldNames = new HashSet<string>(StringComparer.Ordinal)
        {
            "IsCompressed",
            "ObjectBoundsFirst",
            "ObjectBoundsSecond",
            "Name",
            "ShortName",
            "LongName",
            "Flags",
            "MajorFlags"
        };
        var postConfigurationFieldNames = new HashSet<string>(StringComparer.Ordinal)
        {
            "DispositionBase",
            "Aggression",
            "Confidence",
            "EnergyLevel",
            "Responsibility",
            "Assistance",
            "Mood",
            "GearedUpWeapons",
            "HeightMin",
            "HeightMax",
            "Height",
            "SkinToneIndex",
            "Skin",
            "Pronoun",
            "VoiceFormKey",
            "RaceFormKey",
            "AttackRace",
            "CombatOverridePackageListFormKey",
            "CombatStyleFormKey",
            "DefaultPackageListFormKey",
            "CrimeFactionFormKey"
        };
        var customValueFactories = new Dictionary<string, Func<NPCDTO, string>>(StringComparer.Ordinal)
        {
            ["HeightMin"] = record => FormatNumericDisplayValue(record.HeightMin, GetNumericDisplayPrecision<NPCDTO>(nameof(NPCDTO.HeightMin))),
            ["HeightMax"] = record => FormatNumericDisplayValue(record.HeightMax, GetNumericDisplayPrecision<NPCDTO>(nameof(NPCDTO.HeightMax))),
            ["Height"] = record => FormatNumericDisplayValue(record.Height, GetNumericDisplayPrecision<NPCDTO>(nameof(NPCDTO.Height)))
        };
        var fields = CreateSpecComparisonFields(
            RecordTypeCatalog.NPC.RecordID,
            records,
            customValueFactories,
            localizedStrings,
            recordTextLanguage,
            fieldPredicate: fieldSpecification => preLevelFieldNames.Contains(fieldSpecification.FieldName));
        AddNPCLevelGroup(fields, records);
        AddNPCConfigurationGroup(fields, records);
        fields.AddRange(CreateSpecComparisonFields(
            RecordTypeCatalog.NPC.RecordID,
            records,
            customValueFactories,
            localizedStrings,
            recordTextLanguage,
            includeCommonFields: false,
            fieldPredicate: fieldSpecification => postConfigurationFieldNames.Contains(fieldSpecification.FieldName)));
        AddNPCSupplementalFields(fields, records);
        AddNPCFormKeyListGroup(fields, records, "Packages", record => record.Packages);
        AddNPCFormKeyListGroup(fields, records, "ForcedLocations", record => record.ForcedLocations);
        AddNPCFormKeyListGroup(fields, records, "HeadParts", record => record.HeadParts);
        AddNPCFormKeyListGroup(fields, records, "ActorEffects", record => record.ActorEffects);
        AddNPCFactionGroups(fields, records);
        AddNPCPropertyGroups(fields, records);
        AddNPCItemGroups(fields, records);
        AddNPCPerkGroups(fields, records);
        AddNPCMorphGroups(fields, records);
        AddNPCFaceMorphPositionGroups(fields, records);
        AddNPCFaceDialPositionGroups(fields, records);
        AddNPCFaceMorphGroupSetGroups(fields, records);
        AddNPCMorphBlendGroups(fields, records);
        AddNPCTintGroups(fields, records);
        AddNPCTintLayerGroups(fields, records);
        AddNPCFaceTintingLayerGroups(fields, records);
        AddNPCPlayerSkillsGroup(fields, records);
        AddSpecComparisonChildGroups(
            fields,
            game,
            RecordTypeCatalog.NPC.RecordID,
            formKey,
            baseRecords,
            RecordComparisonChildGroupKind.KeywordMappings);
        AddSpecComparisonChildGroups(
            fields,
            game,
            RecordTypeCatalog.NPC.RecordID,
            formKey,
            baseRecords,
            RecordComparisonChildGroupKind.SoundMappings);
        AddSpecComparisonChildGroups(
            fields,
            game,
            RecordTypeCatalog.NPC.RecordID,
            formKey,
            baseRecords,
            RecordComparisonChildGroupKind.ScriptingAdapterMappings);

        return CreateComparison(RecordTypeCatalog.NPC.RecordID, formKey, baseRecords, fields);
    }

    /// <summary>
    /// Creates the comparison output for imported Magic Effect overrides, using specification metadata for scalar
    /// parent rows and keyword child-group dispatch while leaving sound and scripting adapter rows on existing
    /// strategy code.
    /// </summary>
    /// <param name="game">The game whose imported magic effect records should be compared.</param>
    /// <param name="formKey">The origin FormKey shared by the magic effect overrides.</param>
    /// <returns>The magic effect comparison DTO consumed by presentation rendering.</returns>
    private RecordComparisonDTO CreateMagicEffectComparison(SupportedGame game, FormKeyDTO formKey)
    {
        var records = MagicEffectRepository.GetByFormKey(game, formKey);
        var localizedStrings = RecordLocalizedStringRepository.GetByFormKey(game, RecordTypeCatalog.MagicEffect.RecordID, formKey);
        var recordTextLanguage = GameSelectionService.GetRecordTextLanguage();
        var fields = CreateSpecComparisonFields(
            RecordTypeCatalog.MagicEffect.RecordID,
            records,
            localizedStrings: localizedStrings,
            recordTextLanguage: recordTextLanguage);
        var baseRecords = records.Cast<RecordDTO>().ToList();
        AddSpecComparisonChildGroups(
            fields,
            game,
            RecordTypeCatalog.MagicEffect.RecordID,
            formKey,
            baseRecords,
            RecordComparisonChildGroupKind.KeywordMappings);
        AddSpecComparisonChildGroups(
            fields,
            game,
            RecordTypeCatalog.MagicEffect.RecordID,
            formKey,
            baseRecords,
            RecordComparisonChildGroupKind.SoundMappings);
        AddSpecComparisonChildGroups(
            fields,
            game,
            RecordTypeCatalog.MagicEffect.RecordID,
            formKey,
            baseRecords,
            RecordComparisonChildGroupKind.ScriptingAdapterMappings);

        return CreateComparison(RecordTypeCatalog.MagicEffect.RecordID, formKey, baseRecords, fields);
    }

    /// <summary>
    /// Creates the comparison output for imported Perk overrides, using specification metadata for scalar parent rows
    /// while leaving effect, rank, background skill, condition, sound, script, and scripting adapter rows on existing
    /// strategy code.
    /// </summary>
    /// <param name="game">The game whose imported perk records should be compared.</param>
    /// <param name="formKey">The origin FormKey shared by the perk overrides.</param>
    /// <returns>The perk comparison DTO consumed by presentation rendering.</returns>
    private RecordComparisonDTO CreatePerkComparison(SupportedGame game, FormKeyDTO formKey)
    {
        var records = PerkRepository.GetByFormKey(game, formKey);
        var localizedStrings = RecordLocalizedStringRepository.GetByFormKey(game, RecordTypeCatalog.Perk.RecordID, formKey);
        var recordTextLanguage = GameSelectionService.GetRecordTextLanguage();
        var fields = CreateSpecComparisonFields(
            RecordTypeCatalog.Perk.RecordID,
            records,
            localizedStrings: localizedStrings,
            recordTextLanguage: recordTextLanguage);
        AddPerkEffectGroups(fields, records, localizedStrings, recordTextLanguage);
        AddPerkRankGroups(fields, records, localizedStrings, recordTextLanguage);
        AddPerkBackgroundSkillGroup(fields, records);
        AddSpecComparisonChildGroups(
            fields,
            game,
            RecordTypeCatalog.Perk.RecordID,
            formKey,
            records.Cast<RecordDTO>().ToList(),
            RecordComparisonChildGroupKind.ConditionRules);
        AddSpecComparisonChildGroups(
            fields,
            game,
            RecordTypeCatalog.Perk.RecordID,
            formKey,
            records.Cast<RecordDTO>().ToList(),
            RecordComparisonChildGroupKind.SoundMappings);
        AddScriptFragmentGroups(fields, records.Cast<RecordDTO>().ToList(), records.SelectMany(record => record.ScriptFragments).ToList());
        AddSpecComparisonChildGroups(
            fields,
            game,
            RecordTypeCatalog.Perk.RecordID,
            formKey,
            records.Cast<RecordDTO>().ToList(),
            RecordComparisonChildGroupKind.ScriptingAdapterMappings);

        return CreateComparison(RecordTypeCatalog.Perk.RecordID, formKey, records.Cast<RecordDTO>().ToList(), fields);
    }

    /// <summary>
    /// Creates the comparison output for imported Static overrides, using specification metadata for scalar parent
    /// rows while leaving navmesh, keyword, property, model, and reflection groups on strategy code.
    /// </summary>
    /// <param name="game">The game whose imported static records should be compared.</param>
    /// <param name="formKey">The origin FormKey shared by the static overrides.</param>
    /// <returns>The static comparison DTO consumed by presentation rendering.</returns>
    private RecordComparisonDTO CreateStaticComparison(SupportedGame game, FormKeyDTO formKey)
    {
        var records = StaticRepository.GetByFormKey(game, formKey);
        var localizedStrings = RecordLocalizedStringRepository.GetByFormKey(game, RecordTypeCatalog.Static.RecordID, formKey);
        var recordTextLanguage = GameSelectionService.GetRecordTextLanguage();
        var fields = CreateSpecComparisonFields(
            RecordTypeCatalog.Static.RecordID,
            records,
            localizedStrings: localizedStrings,
            recordTextLanguage: recordTextLanguage);
        AddStaticNavmeshGeometryGroups(fields, records);
        AddSpecComparisonChildGroups(
            fields,
            game,
            RecordTypeCatalog.Static.RecordID,
            formKey,
            records.Cast<RecordDTO>().ToList(),
            RecordComparisonChildGroupKind.KeywordMappings);
        AddStaticPropertyGroups(fields, records);
        AddSpecComparisonChildGroups(
            fields,
            game,
            RecordTypeCatalog.Static.RecordID,
            formKey,
            records.Cast<RecordDTO>().ToList(),
            RecordComparisonChildGroupKind.ModelMappings);
        AddSpecComparisonChildGroups(
            fields,
            game,
            RecordTypeCatalog.Static.RecordID,
            formKey,
            records.Cast<RecordDTO>().ToList(),
            RecordComparisonChildGroupKind.ReflectionMappings);

        return CreateComparison(RecordTypeCatalog.Static.RecordID, formKey, records.Cast<RecordDTO>().ToList(), fields);
    }

    private static void AddStaticPropertyGroups(ICollection<RecordComparisonFieldDTO> fields, IReadOnlyList<StaticDTO> records)
    {
        var propertyIndexes = records
            .SelectMany(record => record.Properties)
            .Select(property => property.PropertyIndex)
            .Distinct()
            .OrderBy(index => index)
            .ToList();

        foreach (var propertyIndex in propertyIndexes)
        {
            var currentIndex = propertyIndex;
            fields.Add(CreateGroupField(
                $"Property [{currentIndex}]",
                records.Cast<RecordDTO>().ToList(),
                [
                    CreateField("ActorValue", records, record => FormatFormKey(FindStaticProperty(record, currentIndex)?.ActorValue)),
                    CreateField("Value", records, record => FindStaticProperty(record, currentIndex)?.Value?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty)
                ]));
        }
    }

    private static void AddStaticNavmeshGeometryGroups(ICollection<RecordComparisonFieldDTO> fields, IReadOnlyList<StaticDTO> records)
    {
        if (!records.Any(record => record.NavmeshGeometry != null))
        {
            return;
        }

        var childFields = new List<RecordComparisonFieldDTO>
        {
            CreateField("GridMin", records, record => record.NavmeshGeometry?.GridMin ?? string.Empty),
            CreateField("GridMax", records, record => record.NavmeshGeometry?.GridMax ?? string.Empty),
            CreateField("GridMaxDistance", records, record => record.NavmeshGeometry?.GridMaxDistance ?? string.Empty),
            CreateField("GridSize", records, record => record.NavmeshGeometry?.GridSize ?? string.Empty),
            CreateField("Parent.Type", records, record => record.NavmeshGeometry?.Parent?.MutagenObjectType ?? string.Empty),
            CreateField("Parent.Parent", records, record => FormatFormKey(record.NavmeshGeometry?.Parent?.Parent)),
            CreateField("Versioning", records, record => record.NavmeshGeometry == null ? string.Empty : string.Join(", ", record.NavmeshGeometry.Versioning))
        }
            .Where(HasVisibleValue)
            .ToList();

        AddStaticNavmeshCoverGroups(childFields, records);
        AddStaticNavmeshCoverTriangleMappingGroups(childFields, records);
        AddStaticNavmeshGridArrayGroups(childFields, records);
        AddStaticNavmeshTriangleGroups(childFields, records);
        AddStaticNavmeshVertexGroups(childFields, records);

        if (childFields.Count > 0)
        {
            fields.Add(CreateGroupField("Navmesh Geometry", records.Cast<RecordDTO>().ToList(), childFields));
        }
    }

    private static void AddStaticNavmeshCoverGroups(ICollection<RecordComparisonFieldDTO> fields, IReadOnlyList<StaticDTO> records)
    {
        var coverIndexes = records
            .SelectMany(record => record.NavmeshGeometry?.Cover ?? new List<StaticNavmeshCoverDTO>())
            .Select(cover => cover.CoverIndex)
            .Distinct()
            .Order()
            .ToList();

        foreach (var coverIndex in coverIndexes)
        {
            var currentIndex = coverIndex;
            fields.Add(CreateGroupField(
                $"Cover [{currentIndex}]",
                records.Cast<RecordDTO>().ToList(),
                [
                    CreateField("Data", records, record => FindStaticNavmeshCover(record.NavmeshGeometry, currentIndex)?.Data ?? string.Empty),
                    CreateField("Vertex1", records, record => FindStaticNavmeshCover(record.NavmeshGeometry, currentIndex)?.Vertex1 ?? string.Empty),
                    CreateField("Vertex2", records, record => FindStaticNavmeshCover(record.NavmeshGeometry, currentIndex)?.Vertex2 ?? string.Empty)
                ]));
        }
    }

    private static void AddStaticNavmeshCoverTriangleMappingGroups(ICollection<RecordComparisonFieldDTO> fields, IReadOnlyList<StaticDTO> records)
    {
        var mappingIndexes = records
            .SelectMany(record => record.NavmeshGeometry?.CoverTriangleMappings ?? new List<StaticNavmeshCoverTriangleMappingDTO>())
            .Select(mapping => mapping.MappingIndex)
            .Distinct()
            .Order()
            .ToList();

        foreach (var mappingIndex in mappingIndexes)
        {
            var currentIndex = mappingIndex;
            fields.Add(CreateGroupField(
                $"Cover Triangle Mapping [{currentIndex}]",
                records.Cast<RecordDTO>().ToList(),
                [
                    CreateField("Cover", records, record => FindStaticNavmeshCoverTriangleMapping(record.NavmeshGeometry, currentIndex)?.Cover ?? string.Empty),
                    CreateField("Triangle", records, record => FindStaticNavmeshCoverTriangleMapping(record.NavmeshGeometry, currentIndex)?.Triangle ?? string.Empty),
                    CreateField("Value", records, record => FindStaticNavmeshCoverTriangleMapping(record.NavmeshGeometry, currentIndex)?.Value ?? string.Empty)
                ]));
        }
    }

    private static void AddStaticNavmeshGridArrayGroups(ICollection<RecordComparisonFieldDTO> fields, IReadOnlyList<StaticDTO> records)
    {
        var gridArrayIndexes = records
            .SelectMany(record => record.NavmeshGeometry?.GridArrays ?? new List<StaticNavmeshGridArrayDTO>())
            .Select(gridArray => gridArray.GridArrayIndex)
            .Distinct()
            .Order()
            .ToList();

        foreach (var gridArrayIndex in gridArrayIndexes)
        {
            var currentIndex = gridArrayIndex;
            fields.Add(CreateGroupField(
                $"Grid Array [{currentIndex}]",
                records.Cast<RecordDTO>().ToList(),
                [
                    CreateField(
                        "GridCell",
                        records,
                        record => FindStaticNavmeshGridArray(record.NavmeshGeometry, currentIndex) is { } gridArray
                            ? string.Join(", ", gridArray.GridCell)
                            : string.Empty)
                ]));
        }
    }

    private static void AddStaticNavmeshTriangleGroups(ICollection<RecordComparisonFieldDTO> fields, IReadOnlyList<StaticDTO> records)
    {
        var triangleIndexes = records
            .SelectMany(record => record.NavmeshGeometry?.Triangles ?? new List<StaticNavmeshTriangleDTO>())
            .Select(triangle => triangle.TriangleIndex)
            .Distinct()
            .Order()
            .ToList();

        foreach (var triangleIndex in triangleIndexes)
        {
            var currentIndex = triangleIndex;
            fields.Add(CreateGroupField(
                $"Triangle [{currentIndex}]",
                records.Cast<RecordDTO>().ToList(),
                [
                    CreateField("EdgeLink_0_1", records, record => FindStaticNavmeshTriangle(record.NavmeshGeometry, currentIndex)?.EdgeLink_0_1 ?? string.Empty),
                    CreateField("EdgeLink_1_2", records, record => FindStaticNavmeshTriangle(record.NavmeshGeometry, currentIndex)?.EdgeLink_1_2 ?? string.Empty),
                    CreateField("EdgeLink_2_0", records, record => FindStaticNavmeshTriangle(record.NavmeshGeometry, currentIndex)?.EdgeLink_2_0 ?? string.Empty),
                    CreateField("Height", records, record => FindStaticNavmeshTriangle(record.NavmeshGeometry, currentIndex)?.Height ?? string.Empty),
                    CreateField("Vertices", records, record => FindStaticNavmeshTriangle(record.NavmeshGeometry, currentIndex)?.Vertices ?? string.Empty),
                    CreateField("CoverFlags", records, record => FindStaticNavmeshTriangle(record.NavmeshGeometry, currentIndex)?.CoverFlags ?? string.Empty),
                    CreateField("Flags", records, record => FindStaticNavmeshTriangle(record.NavmeshGeometry, currentIndex)?.Flags ?? string.Empty)
                ]));
        }
    }

    private static void AddStaticNavmeshVertexGroups(ICollection<RecordComparisonFieldDTO> fields, IReadOnlyList<StaticDTO> records)
    {
        var vertexIndexes = records
            .SelectMany(record => record.NavmeshGeometry?.Vertices ?? new List<StaticNavmeshVertexDTO>())
            .Select(vertex => vertex.VertexIndex)
            .Distinct()
            .Order()
            .ToList();

        foreach (var vertexIndex in vertexIndexes)
        {
            var currentIndex = vertexIndex;
            fields.Add(CreateGroupField(
                $"Vertex [{currentIndex}]",
                records.Cast<RecordDTO>().ToList(),
                [
                    CreateField("Point", records, record => FindStaticNavmeshVertex(record.NavmeshGeometry, currentIndex)?.Point ?? string.Empty)
                ]));
        }
    }

    private static StaticPropertyDTO? FindStaticProperty(StaticDTO record, int propertyIndex)
    {
        return record.Properties.FirstOrDefault(property => property.PropertyIndex == propertyIndex);
    }

    private static StaticNavmeshCoverDTO? FindStaticNavmeshCover(StaticNavmeshGeometryDTO? geometry, int coverIndex)
    {
        return geometry?.Cover.FirstOrDefault(cover => cover.CoverIndex == coverIndex);
    }

    private static StaticNavmeshCoverTriangleMappingDTO? FindStaticNavmeshCoverTriangleMapping(StaticNavmeshGeometryDTO? geometry, int mappingIndex)
    {
        return geometry?.CoverTriangleMappings.FirstOrDefault(mapping => mapping.MappingIndex == mappingIndex);
    }

    private static StaticNavmeshGridArrayDTO? FindStaticNavmeshGridArray(StaticNavmeshGeometryDTO? geometry, int gridArrayIndex)
    {
        return geometry?.GridArrays.FirstOrDefault(gridArray => gridArray.GridArrayIndex == gridArrayIndex);
    }

    private static StaticNavmeshTriangleDTO? FindStaticNavmeshTriangle(StaticNavmeshGeometryDTO? geometry, int triangleIndex)
    {
        return geometry?.Triangles.FirstOrDefault(triangle => triangle.TriangleIndex == triangleIndex);
    }

    private static StaticNavmeshVertexDTO? FindStaticNavmeshVertex(StaticNavmeshGeometryDTO? geometry, int vertexIndex)
    {
        return geometry?.Vertices.FirstOrDefault(vertex => vertex.VertexIndex == vertexIndex);
    }

    /// <summary>
    /// Creates the comparison output for imported Book overrides, using specification metadata for scalar parent rows
    /// while leaving child collections on existing strategy code.
    /// </summary>
    /// <param name="game">The game whose imported book records should be compared.</param>
    /// <param name="formKey">The origin FormKey shared by the book overrides.</param>
    /// <returns>The book comparison DTO consumed by presentation rendering.</returns>
    private RecordComparisonDTO CreateBookComparison(SupportedGame game, FormKeyDTO formKey)
    {
        var records = BookRepository.GetByFormKey(game, formKey);
        var localizedStrings = RecordLocalizedStringRepository.GetByFormKey(game, RecordTypeCatalog.Book.RecordID, formKey);
        var recordTextLanguage = GameSelectionService.GetRecordTextLanguage();
        var baseRecords = records.Cast<RecordDTO>().ToList();
        var fields = CreateSpecComparisonFields(
            RecordTypeCatalog.Book.RecordID,
            records,
            new Dictionary<string, Func<BookDTO, string>>(StringComparer.Ordinal)
            {
                ["Text"] = record => GetTranslatedDisplayValue(
                    localizedStrings,
                    record,
                    GetBookTextSourceField(record),
                    recordTextLanguage,
                    record.Text)
            },
            localizedStrings,
            recordTextLanguage);
        AddSpecComparisonChildGroups(
            fields,
            game,
            RecordTypeCatalog.Book.RecordID,
            formKey,
            baseRecords,
            RecordComparisonChildGroupKind.KeywordMappings);
        AddSpecComparisonChildGroups(
            fields,
            game,
            RecordTypeCatalog.Book.RecordID,
            formKey,
            baseRecords,
            RecordComparisonChildGroupKind.ModelMappings);
        AddSpecComparisonChildGroups(
            fields,
            game,
            RecordTypeCatalog.Book.RecordID,
            formKey,
            baseRecords,
            RecordComparisonChildGroupKind.SoundMappings);
        AddSpecComparisonChildGroups(
            fields,
            game,
            RecordTypeCatalog.Book.RecordID,
            formKey,
            baseRecords,
            RecordComparisonChildGroupKind.ScriptingAdapterMappings);
        AddSpecComparisonChildGroups(
            fields,
            game,
            RecordTypeCatalog.Book.RecordID,
            formKey,
            baseRecords,
            RecordComparisonChildGroupKind.RecordComponents);
        AddSpecComparisonChildGroups(
            fields,
            game,
            RecordTypeCatalog.Book.RecordID,
            formKey,
            baseRecords,
            RecordComparisonChildGroupKind.ReflectionMappings);

        return CreateComparison(RecordTypeCatalog.Book.RecordID, formKey, baseRecords, fields);
    }

    private static string GetBookTextSourceField(BookDTO record)
    {
        return record.Game == SupportedGame.Starfield ? "Text" : "BookText";
    }

    /// <summary>
    /// Creates the comparison output for imported Door overrides, using specification metadata for scalar parent rows
    /// while leaving child collections on existing strategy code.
    /// </summary>
    /// <param name="game">The game whose imported door records should be compared.</param>
    /// <param name="formKey">The origin FormKey shared by the door overrides.</param>
    /// <returns>The door comparison DTO consumed by presentation rendering.</returns>
    private RecordComparisonDTO CreateDoorComparison(SupportedGame game, FormKeyDTO formKey)
    {
        var records = DoorRepository.GetByFormKey(game, formKey);
        var localizedStrings = RecordLocalizedStringRepository.GetByFormKey(game, RecordTypeCatalog.Door.RecordID, formKey);
        var recordTextLanguage = GameSelectionService.GetRecordTextLanguage();
        var baseRecords = records.Cast<RecordDTO>().ToList();
        var fields = CreateSpecComparisonFields(
            RecordTypeCatalog.Door.RecordID,
            records,
            localizedStrings: localizedStrings,
            recordTextLanguage: recordTextLanguage);
        AddSpecComparisonChildGroups(
            fields,
            game,
            RecordTypeCatalog.Door.RecordID,
            formKey,
            baseRecords,
            RecordComparisonChildGroupKind.KeywordMappings);
        AddSpecComparisonChildGroups(
            fields,
            game,
            RecordTypeCatalog.Door.RecordID,
            formKey,
            baseRecords,
            RecordComparisonChildGroupKind.ModelMappings);
        AddSpecComparisonChildGroups(
            fields,
            game,
            RecordTypeCatalog.Door.RecordID,
            formKey,
            baseRecords,
            RecordComparisonChildGroupKind.SoundMappings);
        AddSpecComparisonChildGroups(
            fields,
            game,
            RecordTypeCatalog.Door.RecordID,
            formKey,
            baseRecords,
            RecordComparisonChildGroupKind.ScriptingAdapterMappings);
        AddSpecComparisonChildGroups(
            fields,
            game,
            RecordTypeCatalog.Door.RecordID,
            formKey,
            baseRecords,
            RecordComparisonChildGroupKind.RecordComponents);
        AddSpecComparisonChildGroups(
            fields,
            game,
            RecordTypeCatalog.Door.RecordID,
            formKey,
            baseRecords,
            RecordComparisonChildGroupKind.ReflectionMappings);

        return CreateComparison(RecordTypeCatalog.Door.RecordID, formKey, baseRecords, fields);
    }

    /// <summary>
    /// Creates the comparison output for imported Container overrides, using specification metadata for scalar parent
    /// rows while leaving item and shared child collections on existing strategy code.
    /// </summary>
    /// <param name="game">The game whose imported container records should be compared.</param>
    /// <param name="formKey">The origin FormKey shared by the container overrides.</param>
    /// <returns>The container comparison DTO consumed by presentation rendering.</returns>
    private RecordComparisonDTO CreateContainerComparison(SupportedGame game, FormKeyDTO formKey)
    {
        var records = ContainerRepository.GetByFormKey(game, formKey);
        var localizedStrings = RecordLocalizedStringRepository.GetByFormKey(game, RecordTypeCatalog.Container.RecordID, formKey);
        var recordTextLanguage = GameSelectionService.GetRecordTextLanguage();
        var baseRecords = records.Cast<RecordDTO>().ToList();
        var fields = CreateSpecComparisonFields(
            RecordTypeCatalog.Container.RecordID,
            records,
            localizedStrings: localizedStrings,
            recordTextLanguage: recordTextLanguage);
        AddContainerItemGroups(fields, records);
        AddContainerPropertyGroups(fields, records);
        AddContainerForcedLocationGroups(fields, records);
        AddSpecComparisonChildGroups(
            fields,
            game,
            RecordTypeCatalog.Container.RecordID,
            formKey,
            baseRecords,
            RecordComparisonChildGroupKind.KeywordMappings);
        AddSpecComparisonChildGroups(
            fields,
            game,
            RecordTypeCatalog.Container.RecordID,
            formKey,
            baseRecords,
            RecordComparisonChildGroupKind.ModelMappings);
        AddSpecComparisonChildGroups(
            fields,
            game,
            RecordTypeCatalog.Container.RecordID,
            formKey,
            baseRecords,
            RecordComparisonChildGroupKind.SoundMappings);
        AddSpecComparisonChildGroups(
            fields,
            game,
            RecordTypeCatalog.Container.RecordID,
            formKey,
            baseRecords,
            RecordComparisonChildGroupKind.ScriptingAdapterMappings);
        AddSpecComparisonChildGroups(
            fields,
            game,
            RecordTypeCatalog.Container.RecordID,
            formKey,
            baseRecords,
            RecordComparisonChildGroupKind.RecordComponents);
        AddSpecComparisonChildGroups(
            fields,
            game,
            RecordTypeCatalog.Container.RecordID,
            formKey,
            baseRecords,
            RecordComparisonChildGroupKind.ReflectionMappings);

        return CreateComparison(RecordTypeCatalog.Container.RecordID, formKey, baseRecords, fields);
    }

    /// <summary>
    /// Creates the comparison output for imported Constructible Object overrides, using specification metadata for
    /// scalar parent rows while leaving components, categories, filters, conditions, sounds, and scripts on existing
    /// strategy code.
    /// </summary>
    /// <param name="game">The game whose imported constructible object records should be compared.</param>
    /// <param name="formKey">The origin FormKey shared by the constructible object overrides.</param>
    /// <returns>The constructible object comparison DTO consumed by presentation rendering.</returns>
    private RecordComparisonDTO CreateConstructibleObjectComparison(SupportedGame game, FormKeyDTO formKey)
    {
        var records = ConstructibleObjectRepository.GetByFormKey(game, formKey);
        var localizedStrings = RecordLocalizedStringRepository.GetByFormKey(game, RecordTypeCatalog.ConstructibleObject.RecordID, formKey);
        var recordTextLanguage = GameSelectionService.GetRecordTextLanguage();
        var baseRecords = records.Cast<RecordDTO>().ToList();
        var fields = CreateSpecComparisonFields(
            RecordTypeCatalog.ConstructibleObject.RecordID,
            records,
            localizedStrings: localizedStrings,
            recordTextLanguage: recordTextLanguage);
        AddConstructibleObjectComponentGroups(fields, records);
        AddConstructibleObjectCategoryGroups(fields, records);
        AddConstructibleObjectRecipeFilterGroups(fields, records);
        AddSpecComparisonChildGroups(
            fields,
            game,
            RecordTypeCatalog.ConditionForm.RecordID,
            formKey,
            baseRecords,
            RecordComparisonChildGroupKind.ConditionRules);
        AddSpecComparisonChildGroups(
            fields,
            game,
            RecordTypeCatalog.ConstructibleObject.RecordID,
            formKey,
            baseRecords,
            RecordComparisonChildGroupKind.SoundMappings);
        AddSpecComparisonChildGroups(
            fields,
            game,
            RecordTypeCatalog.ConstructibleObject.RecordID,
            formKey,
            baseRecords,
            RecordComparisonChildGroupKind.ScriptingAdapterMappings);

        return CreateComparison(RecordTypeCatalog.ConstructibleObject.RecordID, formKey, baseRecords, fields);
    }

    /// <summary>
    /// Creates the comparison output for imported Condition Form overrides, using specification metadata for scalar
    /// parent rows while leaving condition-rule rows on existing strategy code.
    /// </summary>
    /// <param name="game">The game whose imported condition form records should be compared.</param>
    /// <param name="formKey">The origin FormKey shared by the condition form overrides.</param>
    /// <returns>The condition form comparison DTO consumed by presentation rendering.</returns>
    private RecordComparisonDTO CreateConditionFormComparison(SupportedGame game, FormKeyDTO formKey)
    {
        var records = ConditionFormRepository.GetByFormKey(game, formKey);
        var baseRecords = records.Cast<RecordDTO>().ToList();
        var fields = CreateSpecComparisonFields(RecordTypeCatalog.ConditionForm.RecordID, records);
        AddSpecComparisonChildGroups(
            fields,
            game,
            RecordTypeCatalog.ConstructibleObject.RecordID,
            formKey,
            baseRecords,
            RecordComparisonChildGroupKind.ConditionRules);

        return CreateComparison(RecordTypeCatalog.ConditionForm.RecordID, formKey, baseRecords, fields);
    }

    /// <summary>
    /// Creates the comparison output for imported Terminal overrides, using specification metadata for scalar parent
    /// rows while leaving terminal child collections on existing strategy code.
    /// </summary>
    /// <param name="game">The game whose imported terminal records should be compared.</param>
    /// <param name="formKey">The origin FormKey shared by the terminal overrides.</param>
    /// <returns>The terminal comparison DTO consumed by presentation rendering.</returns>
    private RecordComparisonDTO CreateTerminalComparison(SupportedGame game, FormKeyDTO formKey)
    {
        var records = TerminalRepository.GetByFormKey(game, formKey);
        var localizedStrings = RecordLocalizedStringRepository.GetByFormKey(game, RecordTypeCatalog.Terminal.RecordID, formKey);
        var recordTextLanguage = GameSelectionService.GetRecordTextLanguage();
        var baseRecords = records.Cast<RecordDTO>().ToList();
        var customValueFactories = new Dictionary<string, Func<TerminalDTO, string>>(StringComparer.Ordinal)
        {
            ["MarkerFlags"] = record => FormatHexIntegerString(record.MarkerFlags)
        };
        var fields = CreateSpecComparisonFields(
            RecordTypeCatalog.Terminal.RecordID,
            records,
            customValueFactories,
            localizedStrings,
            recordTextLanguage);
        AddTerminalForcedLocationGroups(fields, records);
        AddSpecComparisonChildGroups(
            fields,
            game,
            RecordTypeCatalog.Terminal.RecordID,
            formKey,
            baseRecords,
            RecordComparisonChildGroupKind.KeywordMappings);
        AddSpecComparisonChildGroups(
            fields,
            game,
            RecordTypeCatalog.Terminal.RecordID,
            formKey,
            baseRecords,
            RecordComparisonChildGroupKind.ModelMappings);
        AddSpecComparisonChildGroups(
            fields,
            game,
            RecordTypeCatalog.Terminal.RecordID,
            formKey,
            baseRecords,
            RecordComparisonChildGroupKind.ScriptingAdapterMappings);
        AddSpecComparisonChildGroups(
            fields,
            game,
            RecordTypeCatalog.Terminal.RecordID,
            formKey,
            baseRecords,
            RecordComparisonChildGroupKind.ConditionRules);
        AddScriptFragmentGroups(fields, baseRecords, records.SelectMany(record => record.ScriptFragments).ToList());
        AddSpecComparisonChildGroups(
            fields,
            game,
            RecordTypeCatalog.Terminal.RecordID,
            formKey,
            baseRecords,
            RecordComparisonChildGroupKind.ReflectionMappings);
        AddTerminalMarkerParameterGroups(fields, records);
        AddTerminalBodyTextGroups(fields, records, localizedStrings, recordTextLanguage);
        AddTerminalMenuItemGroups(fields, records, localizedStrings, recordTextLanguage);

        return CreateComparison(RecordTypeCatalog.Terminal.RecordID, formKey, baseRecords, fields);
    }

    private static RecordComparisonDTO CreateComparison(
        string recordType,
        FormKeyDTO formKey,
        IReadOnlyList<RecordDTO> records,
        IReadOnlyList<RecordComparisonFieldDTO> fields)
    {
        return new RecordComparisonDTO
        {
            RecordType = recordType,
            FormKey = formKey,
            EditorID = records.FirstOrDefault()?.EditorID ?? string.Empty,
            Columns = records.Select(record => new RecordComparisonColumnDTO
            {
                ModKey = record.ModKey,
                Header = record.ModKey.FileName
            }).ToList(),
            Fields = fields
        };
    }

    private static List<RecordComparisonFieldDTO> CreateCommonFields(IReadOnlyList<RecordDTO> records)
    {
        return
        [
            CreateField("EditorID", records, record => record.EditorID),
            CreateField("FormVersion", records, record => record.FormVersion.ToString(), isComparable: false),
            CreateField("VersionControl", records, record => record.VersionControl?.ToString() ?? string.Empty, isComparable: false),
            CreateField("MajorRecordFlags", records, record => record.MajorRecordFlags.ToString(), isComparable: false)
        ];
    }

    /// <summary>
    /// Creates comparison fields from a record specification while allowing callers to keep record-specific formatting
    /// hooks for values that are not yet purely declarative.
    /// </summary>
    /// <typeparam name="TRecord">The record DTO type participating in the comparison.</typeparam>
    /// <param name="recordType">The Bethesda record ID whose comparison specification should be used.</param>
    /// <param name="records">The ordered records participating in the comparison.</param>
    /// <param name="customValueFactories">Optional value factories keyed by comparison field name.</param>
    /// <param name="localizedStrings">Optional localized string rows used by specification-declared localized fields.</param>
    /// <param name="recordTextLanguage">The preferred language used when resolving localized comparison rows.</param>
    /// <param name="includeCommonFields">Whether common record header fields should be included in the returned rows.</param>
    /// <param name="fieldPredicate">Optional predicate used by callers that need to insert specification rows in phases.</param>
    /// <returns>The comparison fields produced from the specification and custom value factories.</returns>
    private List<RecordComparisonFieldDTO> CreateSpecComparisonFields<TRecord>(
        string recordType,
        IReadOnlyList<TRecord> records,
        IReadOnlyDictionary<string, Func<TRecord, string>>? customValueFactories = null,
        IReadOnlyList<LocalizedStringDTO>? localizedStrings = null,
        Language? recordTextLanguage = null,
        bool includeCommonFields = true,
        Func<RecordComparisonFieldSpecification, bool>? fieldPredicate = null)
        where TRecord : RecordDTO
    {
        var specification = RecordSpecificationProvider.FindByRecordID(recordType);
        var fields = !includeCommonFields || specification?.Comparison.IncludeCommonFields == false
            ? new List<RecordComparisonFieldDTO>()
            : CreateCommonFields(records.Cast<RecordDTO>().ToList());

        if (specification == null)
        {
            return fields;
        }

        foreach (var fieldSpecification in specification.Comparison.Fields)
        {
            if (fieldPredicate != null && !fieldPredicate(fieldSpecification))
            {
                continue;
            }

            if (fieldSpecification.ValueKind == RecordFieldValueKind.Collection)
            {
                continue;
            }

            fields.Add(CreateSpecComparisonField(
                fieldSpecification,
                records,
                customValueFactories,
                localizedStrings,
                recordTextLanguage));
        }

        return fields;
    }

    /// <summary>
    /// Appends strategy-backed child groups declared by the record comparison specification.
    /// </summary>
    /// <param name="fields">The comparison field list that receives generated child groups.</param>
    /// <param name="game">The game whose imported rows are being compared.</param>
    /// <param name="recordType">The Bethesda record ID whose comparison specification should be used.</param>
    /// <param name="formKey">The origin FormKey shared by the compared records.</param>
    /// <param name="records">The ordered base record rows participating in the comparison.</param>
    /// <param name="groupKinds">The child-group strategies to execute at the current row position.</param>
    /// <exception cref="NotSupportedException">
    /// Thrown when metadata asks Core to execute a child-group strategy that this service does not implement.
    /// </exception>
    private void AddSpecComparisonChildGroups(
        IList<RecordComparisonFieldDTO> fields,
        SupportedGame game,
        string recordType,
        FormKeyDTO formKey,
        IReadOnlyList<RecordDTO> records,
        params RecordComparisonChildGroupKind[] groupKinds)
    {
        var specification = RecordSpecificationProvider.FindByRecordID(recordType);
        if (specification == null)
        {
            return;
        }

        var requestedGroupKinds = groupKinds.ToHashSet();
        foreach (var childGroup in specification.Comparison.ChildGroups)
        {
            if (requestedGroupKinds.Count > 0 && !requestedGroupKinds.Contains(childGroup.GroupKind))
            {
                continue;
            }

            switch (childGroup.GroupKind)
            {
                case RecordComparisonChildGroupKind.KeywordMappings:
                    AddKeywordGroup(fields, records, KeywordMappingRepository.GetByFormKey(game, recordType, formKey));
                    break;
                case RecordComparisonChildGroupKind.SoundMappings:
                    AddSoundGroups(fields, records, SoundMappingRepository.GetByFormKey(game, recordType, formKey));
                    break;
                case RecordComparisonChildGroupKind.ModelMappings:
                    AddModelGroups(fields, records, ModelRepository.GetByFormKey(game, recordType, formKey));
                    break;
                case RecordComparisonChildGroupKind.ScriptingAdapterMappings:
                    AddScriptingAdapterGroups(
                        fields,
                        records,
                        ScriptingAdapterRepository.GetByFormKey(game, recordType, formKey));
                    break;
                case RecordComparisonChildGroupKind.ReflectionMappings:
                    AddReflectionGroups(fields, records, ReflectionRepository.GetByFormKey(game, recordType, formKey));
                    break;
                case RecordComparisonChildGroupKind.ConditionRules:
                    AddConditionRuleGroups(fields, records, records.Cast<IHasConditionsDTO>().ToList());
                    break;
                case RecordComparisonChildGroupKind.RecordComponents:
                    AddRecordComponentGroups(
                        fields,
                        records,
                        records.Cast<IHasComponentsDTO>().SelectMany(record => record.Components).ToList());
                    break;
                default:
                    throw new NotSupportedException(
                        $"Comparison child group '{childGroup.GroupKind}' is not supported for record type '{recordType}'.");
            }
        }
    }

    /// <summary>
    /// Creates one comparison field from a specification row.
    /// </summary>
    /// <typeparam name="TRecord">The record DTO type participating in the comparison.</typeparam>
    /// <param name="fieldSpecification">The field specification that identifies source path and display kind.</param>
    /// <param name="records">The ordered records participating in the comparison.</param>
    /// <param name="customValueFactories">Optional value factories keyed by comparison field name.</param>
    /// <param name="localizedStrings">Optional localized string rows used by specification-declared localized fields.</param>
    /// <param name="recordTextLanguage">The preferred language used when resolving localized comparison rows.</param>
    /// <returns>The populated comparison field.</returns>
    private static RecordComparisonFieldDTO CreateSpecComparisonField<TRecord>(
        RecordComparisonFieldSpecification fieldSpecification,
        IReadOnlyList<TRecord> records,
        IReadOnlyDictionary<string, Func<TRecord, string>>? customValueFactories,
        IReadOnlyList<LocalizedStringDTO>? localizedStrings,
        Language? recordTextLanguage)
        where TRecord : RecordDTO
    {
        if (customValueFactories != null &&
            customValueFactories.TryGetValue(fieldSpecification.FieldName, out var customValueFactory))
        {
            return CreateField(fieldSpecification.FieldName, records, customValueFactory, fieldSpecification.IsComparable);
        }

        if (fieldSpecification.UsesLocalizedDisplay)
        {
            var sourceField = fieldSpecification.LocalizedSourceField ?? fieldSpecification.SourcePath;
            return CreateField(
                fieldSpecification.FieldName,
                records,
                record => GetTranslatedDisplayValue(
                    localizedStrings ?? [],
                    record,
                    sourceField,
                    recordTextLanguage ?? Language.English,
                    GetPropertyPathValue(record, fieldSpecification.SourcePath) as TranslatedStringDTO),
                fieldSpecification.IsComparable);
        }

        return CreateField(
            fieldSpecification.FieldName,
            records,
            record => FormatSpecComparisonValue(GetPropertyPathValue(record, fieldSpecification.SourcePath), fieldSpecification.ValueKind),
            fieldSpecification.IsComparable);
    }

    /// <summary>
    /// Reads a dotted public-property path from a DTO object.
    /// </summary>
    /// <param name="source">The source object that owns the first path segment.</param>
    /// <param name="sourcePath">The dotted property path to read.</param>
    /// <returns>The resolved property value, or <c>null</c> when any segment is absent or null.</returns>
    private static object? GetPropertyPathValue(object? source, string sourcePath)
    {
        var value = source;
        foreach (var pathSegment in sourcePath.Split('.', StringSplitOptions.RemoveEmptyEntries))
        {
            if (value == null)
            {
                return null;
            }

            var property = value.GetType().GetProperty(pathSegment, BindingFlags.Instance | BindingFlags.Public);
            value = property?.GetValue(value);
        }

        return value;
    }

    /// <summary>
    /// Formats a specification-resolved value using the comparison field's declared value kind.
    /// </summary>
    /// <param name="value">The raw DTO value resolved from the source path.</param>
    /// <param name="valueKind">The broad value kind declared by the comparison specification.</param>
    /// <returns>The formatted display value used by comparison rows.</returns>
    private static string FormatSpecComparisonValue(object? value, RecordFieldValueKind valueKind)
    {
        if (value == null)
        {
            return string.Empty;
        }

        if (valueKind == RecordFieldValueKind.FormKey)
        {
            return FormatFormKey(value as FormKeyDTO);
        }

        if (value is IFormattable formattable)
        {
            return formattable.ToString(null, CultureInfo.InvariantCulture);
        }

        return value.ToString() ?? string.Empty;
    }

    private static RecordComparisonFieldDTO CreateField<TRecord>(
        string fieldName,
        IReadOnlyList<TRecord> records,
        Func<TRecord, string> valueFactory,
        bool isComparable = true)
        where TRecord : RecordDTO
    {
        var values = records.Select(record => new RecordComparisonValueDTO
            {
                ModKey = record.ModKey,
                DisplayValue = valueFactory(record)
            })
            .ToList();
        var state = GetComparisonValueState(values.Select(value => value.DisplayValue).ToList(), isComparable);
        for (var valueIndex = 0; valueIndex < values.Count; valueIndex++)
        {
            values[valueIndex].State = state == RecordComparisonValueState.Conflict && valueIndex == values.Count - 1
                ? RecordComparisonValueState.WinningOverride
                : state;
        }

        return new RecordComparisonFieldDTO
        {
            FieldName = fieldName,
            IsComparable = isComparable,
            State = state,
            Values = values
        };
    }

    /// <summary>
    /// Creates a comparison field for numeric DTO values and applies opt-in display precision metadata when present.
    /// </summary>
    /// <typeparam name="TRecord">The record DTO type being compared.</typeparam>
    /// <param name="fieldName">The comparison row name shown to callers.</param>
    /// <param name="records">The ordered records participating in the comparison.</param>
    /// <param name="valueFactory">The function that extracts the numeric DTO value from each record.</param>
    /// <param name="propertyName">The DTO property name whose metadata controls display precision.</param>
    /// <param name="isComparable">Whether the resulting row should contribute comparison state.</param>
    /// <returns>The populated comparison field with formatted numeric display values.</returns>
    private static RecordComparisonFieldDTO CreateNumericField<TRecord>(
        string fieldName,
        IReadOnlyList<TRecord> records,
        Func<TRecord, double?> valueFactory,
        string propertyName,
        bool isComparable = true)
        where TRecord : RecordDTO
    {
        var precision = GetNumericDisplayPrecision<TRecord>(propertyName);
        return CreateField(fieldName, records, record => FormatNumericDisplayValue(valueFactory(record), precision), isComparable);
    }

    /// <summary>
    /// Reads the optional display precision metadata from a numeric DTO property.
    /// </summary>
    /// <typeparam name="TRecord">The record DTO type that owns the property.</typeparam>
    /// <param name="propertyName">The public property name to inspect.</param>
    /// <returns>The configured decimal-place count, or <c>null</c> when the property is not marked.</returns>
    private static int? GetNumericDisplayPrecision<TRecord>(string propertyName)
    {
        return typeof(TRecord)
            .GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public)
            ?.GetCustomAttribute<NumericDisplayPrecisionAttribute>()
            ?.DecimalPlaces;
    }

    /// <summary>
    /// Formats a numeric comparison value using optional reduced display precision.
    /// </summary>
    /// <param name="value">The source DTO value to display without changing stored/imported data.</param>
    /// <param name="decimalPlaces">The optional number of decimal places to retain.</param>
    /// <returns>The invariant display value, or an empty string when <paramref name="value"/> is <c>null</c>.</returns>
    private static string FormatNumericDisplayValue(double? value, int? decimalPlaces)
    {
        if (!value.HasValue)
        {
            return string.Empty;
        }

        if (!decimalPlaces.HasValue)
        {
            return value.Value.ToString(CultureInfo.InvariantCulture);
        }

        var roundedValue = Math.Round(value.Value, decimalPlaces.Value, MidpointRounding.AwayFromZero);
        var format = decimalPlaces.Value == 0
            ? "0"
            : "0." + new string('#', decimalPlaces.Value);
        return roundedValue.ToString(format, CultureInfo.InvariantCulture);
    }

    private static void AddModelGroups(
        IList<RecordComparisonFieldDTO> fields,
        IReadOnlyList<RecordDTO> records,
        IReadOnlyList<ModelDTO> models)
    {
        foreach (var modelKey in models.Select(model => new ModelKey(model.ModelSlot, model.ModelGender)).Distinct().OrderBy(key => key.Slot, StringComparer.Ordinal).ThenBy(key => key.Gender, StringComparer.Ordinal))
        {
            var modelFields = new List<RecordComparisonFieldDTO>
            {
                CreateChildField("File", records, record => FindModel(models, record.ModKey, modelKey)?.File ?? string.Empty),
                CreateChildField("Texture Hashes", records, record => FindModel(models, record.ModKey, modelKey)?.TextureFileHashes ?? string.Empty),
                CreateChildField("Data", records, record => FindModel(models, record.ModKey, modelKey)?.Data ?? string.Empty),
                CreateChildField("Light Layer", records, record => FindModel(models, record.ModKey, modelKey)?.LightLayer?.ToString() ?? string.Empty),
                CreateChildField("Flags", records, record => FindModel(models, record.ModKey, modelKey)?.Flags ?? string.Empty),
                CreateChildField("Color Remap", records, record => FindModel(models, record.ModKey, modelKey)?.ColorRemappingIndex?.ToString() ?? string.Empty),
                CreateChildField("Vestigial Flags", records, record => FindModel(models, record.ModKey, modelKey)?.FlagsVestigial ?? string.Empty)
            };

            var materialSwapIndexes = models
                .Where(model => IsModelKey(model, modelKey))
                .SelectMany(model => model.MaterialSwaps)
                .Select(materialSwap => materialSwap.MaterialSwapIndex)
                .Distinct()
                .Order()
                .ToList();
            foreach (var materialSwapIndex in materialSwapIndexes)
            {
                var currentIndex = materialSwapIndex;
                modelFields.Add(CreateChildField(
                    $"Material Swap [{materialSwapIndex}]",
                    records,
                    record => FormatFormKey(FindModel(models, record.ModKey, modelKey)?.MaterialSwaps.FirstOrDefault(materialSwap => materialSwap.MaterialSwapIndex == currentIndex)?.MaterialSwapFormKey)));
            }

            var visibleModelFields = modelFields
                .Where(HasVisibleValue)
                .ToList();
            if (visibleModelFields.Count > 0)
            {
                fields.Add(CreateGroupField(GetModelGroupName(modelKey), records, visibleModelFields));
            }
        }
    }

    private static void AddKeywordGroup(
        IList<RecordComparisonFieldDTO> fields,
        IReadOnlyList<RecordDTO> records,
        IReadOnlyList<KeywordMappingDTO> keywords)
    {
        var keywordIndexes = keywords
            .Select(keyword => keyword.KeywordIndex)
            .Distinct()
            .Order()
            .ToList();
        if (keywordIndexes.Count == 0)
        {
            return;
        }

        var keywordFields = new List<RecordComparisonFieldDTO>();
        foreach (var keywordIndex in keywordIndexes)
        {
            var currentIndex = keywordIndex;
            keywordFields.Add(CreateChildField(
                $"Keyword [{keywordIndex}]",
                records,
                record => FormatFormKey(FindKeyword(keywords, record.ModKey, currentIndex)?.Keyword)));
        }

        fields.Add(CreateGroupField("Keywords", records, keywordFields));
    }

    private static void AddSoundGroups(
        IList<RecordComparisonFieldDTO> fields,
        IReadOnlyList<RecordDTO> records,
        IReadOnlyList<SoundMappingDTO> sounds)
    {
        var soundKeys = sounds
            .Select(sound => new SoundKey(sound.SoundSlot, sound.SoundIndex))
            .Distinct()
            .OrderBy(key => key.Slot, StringComparer.Ordinal)
            .ThenBy(key => key.Index)
            .ToList();
        if (soundKeys.Count == 0)
        {
            return;
        }

        var soundFields = new List<RecordComparisonFieldDTO>();
        foreach (var soundKey in soundKeys)
        {
            var soundChildren = new List<RecordComparisonFieldDTO>
            {
                CreateChildField("Start", records, record => FindSound(sounds, record.ModKey, soundKey)?.Start ?? string.Empty),
                CreateChildField("MutagenObjectType", records, record => FindSound(sounds, record.ModKey, soundKey)?.MutagenObjectType ?? string.Empty),
                CreateChildField("InheritsSoundsFrom", records, record => FindSound(sounds, record.ModKey, soundKey)?.InheritsSoundsFrom ?? string.Empty),
                CreateChildField("Versioning", records, record => FindSound(sounds, record.ModKey, soundKey)?.Versioning ?? string.Empty),
                CreateChildField("Unknown", records, record => FindSound(sounds, record.ModKey, soundKey)?.Unknown ?? string.Empty)
            }
                .Where(HasVisibleValue)
                .ToList();
            if (soundChildren.Count > 0)
            {
                soundFields.Add(CreateGroupField(GetSoundGroupName(soundKey), records, soundChildren));
            }
        }

        if (soundFields.Count > 0)
        {
            fields.Add(CreateGroupField("Sounds", records, soundFields));
        }
    }

    private static void AddContainerItemGroups(
        IList<RecordComparisonFieldDTO> fields,
        IReadOnlyList<ContainerDTO> records)
    {
        var itemIndexes = records
            .SelectMany(record => record.Items)
            .Select(item => item.ItemIndex)
            .Distinct()
            .Order()
            .ToList();
        if (itemIndexes.Count == 0)
        {
            return;
        }

        var itemFields = new List<RecordComparisonFieldDTO>();
        foreach (var itemIndex in itemIndexes)
        {
            var currentIndex = itemIndex;
            var itemChildren = new List<RecordComparisonFieldDTO>
            {
                CreateField("Item", records, record => FormatFormKey(record.Items.FirstOrDefault(item => item.ItemIndex == currentIndex)?.ItemFormKey)),
                CreateField("Count", records, record => record.Items.FirstOrDefault(item => item.ItemIndex == currentIndex)?.Count?.ToString() ?? string.Empty)
            }
                .Where(HasVisibleValue)
                .ToList();
            if (itemChildren.Count > 0)
            {
                itemFields.Add(CreateGroupField($"Item [{itemIndex}]", records.Cast<RecordDTO>().ToList(), itemChildren));
            }
        }

        if (itemFields.Count > 0)
        {
            fields.Add(CreateGroupField("Items", records.Cast<RecordDTO>().ToList(), itemFields));
        }
    }

    /// <summary>
    /// Adds comparison rows for container actor-value properties.
    /// </summary>
    private static void AddContainerPropertyGroups(
        IList<RecordComparisonFieldDTO> fields,
        IReadOnlyList<ContainerDTO> records)
    {
        var propertyIndexes = records
            .SelectMany(record => record.Properties)
            .Select(property => property.PropertyIndex)
            .Distinct()
            .Order()
            .ToList();
        foreach (var propertyIndex in propertyIndexes)
        {
            var currentIndex = propertyIndex;
            fields.Add(CreateGroupField(
                $"Property [{currentIndex}]",
                records.Cast<RecordDTO>().ToList(),
                [
                    CreateField("ActorValue", records, record => FormatFormKey(record.Properties.FirstOrDefault(property => property.PropertyIndex == currentIndex)?.ActorValue)),
                    CreateField("Value", records, record => record.Properties.FirstOrDefault(property => property.PropertyIndex == currentIndex)?.Value?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty)
                ]));
        }
    }

    /// <summary>
    /// Adds comparison rows for container forced location links.
    /// </summary>
    private static void AddContainerForcedLocationGroups(
        IList<RecordComparisonFieldDTO> fields,
        IReadOnlyList<ContainerDTO> records)
    {
        var maxForcedLocationCount = records
            .Select(record => record.ForcedLocations.Count)
            .DefaultIfEmpty()
            .Max();
        for (var forcedLocationIndex = 0; forcedLocationIndex < maxForcedLocationCount; forcedLocationIndex++)
        {
            var currentIndex = forcedLocationIndex;
            fields.Add(CreateField($"ForcedLocations[{forcedLocationIndex}]", records, record => FormatFormKey(record.ForcedLocations.ElementAtOrDefault(currentIndex))));
        }
    }

    private static void AddPerkRankGroups(
        IList<RecordComparisonFieldDTO> fields,
        IReadOnlyList<PerkDTO> records,
        IReadOnlyList<LocalizedStringDTO> localizedStrings,
        Language recordTextLanguage)
    {
        var rankIndexes = records
            .SelectMany(record => record.Ranks)
            .Select(rank => rank.RankIndex)
            .Distinct()
            .Order()
            .ToList();
        if (rankIndexes.Count == 0)
        {
            return;
        }

        var rankFields = new List<RecordComparisonFieldDTO>();
        foreach (var rankIndex in rankIndexes)
        {
            var currentRankIndex = rankIndex;
            var rankChildren = new List<RecordComparisonFieldDTO>
            {
                CreateField("Description", records, record => GetTranslatedDisplayValue(localizedStrings, record, $"Ranks[{currentRankIndex}].Description", recordTextLanguage, FindPerkRank(record, currentRankIndex)?.Description)),
                CreateField("UnknownStaticFormKey", records, record => FormatFormKey(FindPerkRank(record, currentRankIndex)?.UnknownStaticFormKey)),
                CreateField("ConditionCount", records, record => FindPerkRank(record, currentRankIndex)?.ConditionCount.ToString() ?? string.Empty),
                CreateField("ActivityCount", records, record => FindPerkRank(record, currentRankIndex)?.ActivityCount.ToString() ?? string.Empty)
            }
                .Where(HasVisibleValue)
                .ToList();
            AddPerkRankEffectGroups(rankChildren, records, localizedStrings, recordTextLanguage, currentRankIndex);
            AddPerkRankActivityGroups(rankChildren, records, localizedStrings, recordTextLanguage, currentRankIndex);
            if (rankChildren.Count > 0)
            {
                rankFields.Add(CreateGroupField($"Rank [{rankIndex}]", records.Cast<RecordDTO>().ToList(), rankChildren));
            }
        }

        if (rankFields.Count > 0)
        {
            fields.Add(CreateGroupField("Ranks", records.Cast<RecordDTO>().ToList(), rankFields));
        }
    }

    private static void AddPerkEffectGroups(
        IList<RecordComparisonFieldDTO> fields,
        IReadOnlyList<PerkDTO> records,
        IReadOnlyList<LocalizedStringDTO> localizedStrings,
        Language recordTextLanguage)
    {
        var effectIndexes = records
            .SelectMany(record => record.Effects)
            .Select(effect => effect.EffectIndex)
            .Distinct()
            .Order()
            .ToList();
        if (effectIndexes.Count == 0)
        {
            return;
        }

        var effectFields = new List<RecordComparisonFieldDTO>();
        foreach (var effectIndex in effectIndexes)
        {
            var currentEffectIndex = effectIndex;
            var effectChildren = CreatePerkEffectFields(
                    records,
                    localizedStrings,
                    recordTextLanguage,
                    $"Effects[{currentEffectIndex}].ButtonLabel",
                    record => FindPerkEffect(record, currentEffectIndex))
                .ToList();
            AddPerkEffectConditionTabGroups(effectChildren, records, record => FindPerkEffect(record, currentEffectIndex)?.Conditions);
            if (effectChildren.Count > 0)
            {
                effectFields.Add(CreateGroupField($"Effect [{effectIndex}]", records.Cast<RecordDTO>().ToList(), effectChildren));
            }
        }

        if (effectFields.Count > 0)
        {
            fields.Add(CreateGroupField("Effects", records.Cast<RecordDTO>().ToList(), effectFields));
        }
    }

    private static void AddPerkRankEffectGroups(
        IList<RecordComparisonFieldDTO> fields,
        IReadOnlyList<PerkDTO> records,
        IReadOnlyList<LocalizedStringDTO> localizedStrings,
        Language recordTextLanguage,
        int rankIndex)
    {
        var effectIndexes = records
            .SelectMany(record => record.Ranks.Where(rank => rank.RankIndex == rankIndex))
            .SelectMany(rank => rank.Effects)
            .Select(effect => effect.EffectIndex)
            .Distinct()
            .Order()
            .ToList();
        if (effectIndexes.Count == 0)
        {
            return;
        }

        var effectFields = new List<RecordComparisonFieldDTO>();
        foreach (var effectIndex in effectIndexes)
        {
            var currentEffectIndex = effectIndex;
            var effectChildren = CreatePerkEffectFields(
                    records,
                    localizedStrings,
                    recordTextLanguage,
                    $"Ranks[{rankIndex}].Effects[{currentEffectIndex}].ButtonLabel",
                    record => FindPerkRankEffect(record, rankIndex, currentEffectIndex))
                .ToList();
            AddPerkEffectConditionTabGroups(effectChildren, records, record => FindPerkRankEffect(record, rankIndex, currentEffectIndex)?.Conditions);
            if (effectChildren.Count > 0)
            {
                effectFields.Add(CreateGroupField($"Effect [{effectIndex}]", records.Cast<RecordDTO>().ToList(), effectChildren));
            }
        }

        if (effectFields.Count > 0)
        {
            fields.Add(CreateGroupField("Effects", records.Cast<RecordDTO>().ToList(), effectFields));
        }
    }

    private static void AddPerkRankActivityGroups(
        IList<RecordComparisonFieldDTO> fields,
        IReadOnlyList<PerkDTO> records,
        IReadOnlyList<LocalizedStringDTO> localizedStrings,
        Language recordTextLanguage,
        int rankIndex)
    {
        var activityIndexes = records
            .SelectMany(record => record.Ranks.Where(rank => rank.RankIndex == rankIndex))
            .SelectMany(rank => rank.Activities)
            .Select(activity => activity.ActivityIndex)
            .Distinct()
            .Order()
            .ToList();
        if (activityIndexes.Count == 0)
        {
            return;
        }

        var activityFields = new List<RecordComparisonFieldDTO>();
        foreach (var activityIndex in activityIndexes)
        {
            var currentActivityIndex = activityIndex;
            var activityChildren = new List<RecordComparisonFieldDTO>
            {
                CreateField("ATAN", records, record => FindPerkRankActivity(record, rankIndex, currentActivityIndex)?.ATAN ?? string.Empty),
                CreateField("Name", records, record => GetTranslatedDisplayValue(localizedStrings, record, $"Ranks[{rankIndex}].Activities[{currentActivityIndex}].Name", recordTextLanguage, FindPerkRankActivity(record, rankIndex, currentActivityIndex)?.Name)),
                CreateField("Description", records, record => GetTranslatedDisplayValue(localizedStrings, record, $"Ranks[{rankIndex}].Activities[{currentActivityIndex}].Description", recordTextLanguage, FindPerkRankActivity(record, rankIndex, currentActivityIndex)?.Description)),
                CreateField("ANAM", records, record => FindPerkRankActivity(record, rankIndex, currentActivityIndex)?.ANAM ?? string.Empty),
                CreateField("Configuration", records, record => FindPerkRankActivity(record, rankIndex, currentActivityIndex)?.Configuration ?? string.Empty)
            }
                .Where(HasVisibleValue)
                .ToList();
            AddPerkRankActivityEvaluatorGroups(activityChildren, records, rankIndex, currentActivityIndex);
            if (activityChildren.Count > 0)
            {
                activityFields.Add(CreateGroupField($"Activity [{activityIndex}]", records.Cast<RecordDTO>().ToList(), activityChildren));
            }
        }

        if (activityFields.Count > 0)
        {
            fields.Add(CreateGroupField("Activities", records.Cast<RecordDTO>().ToList(), activityFields));
        }
    }

    private static void AddPerkRankActivityEvaluatorGroups(
        IList<RecordComparisonFieldDTO> fields,
        IReadOnlyList<PerkDTO> records,
        int rankIndex,
        int activityIndex)
    {
        var evaluatorIndexes = records
            .SelectMany(record => FindPerkRankActivity(record, rankIndex, activityIndex)?.ProgressionEvalutor ?? [])
            .Select(evaluator => evaluator.EvaluatorIndex)
            .Distinct()
            .Order()
            .ToList();
        if (evaluatorIndexes.Count == 0)
        {
            return;
        }

        var evaluatorFields = new List<RecordComparisonFieldDTO>();
        foreach (var evaluatorIndex in evaluatorIndexes)
        {
            var currentEvaluatorIndex = evaluatorIndex;
            var evaluatorChildren = new List<RecordComparisonFieldDTO>
            {
                CreateField("Name", records, record => FindPerkRankActivityEvaluator(record, rankIndex, activityIndex, currentEvaluatorIndex)?.Name ?? string.Empty),
                CreateField("ConditionCount", records, record => FindPerkRankActivityEvaluator(record, rankIndex, activityIndex, currentEvaluatorIndex)?.Conditions.Count.ToString() ?? string.Empty)
            }
                .Where(HasVisibleValue)
                .ToList();
            AddPerkRankActivityEvaluatorConditionGroups(evaluatorChildren, records, rankIndex, activityIndex, currentEvaluatorIndex);
            if (evaluatorChildren.Count > 0)
            {
                evaluatorFields.Add(CreateGroupField($"Evaluator [{evaluatorIndex}]", records.Cast<RecordDTO>().ToList(), evaluatorChildren));
            }
        }

        if (evaluatorFields.Count > 0)
        {
            fields.Add(CreateGroupField("ProgressionEvalutor", records.Cast<RecordDTO>().ToList(), evaluatorFields));
        }
    }

    private static void AddPerkRankActivityEvaluatorConditionGroups(
        IList<RecordComparisonFieldDTO> fields,
        IReadOnlyList<PerkDTO> records,
        int rankIndex,
        int activityIndex,
        int evaluatorIndex)
    {
        var conditionIndexes = records
            .SelectMany(record => FindPerkRankActivityEvaluator(record, rankIndex, activityIndex, evaluatorIndex)?.Conditions ?? [])
            .Select(condition => condition.ConditionIndex)
            .Distinct()
            .Order()
            .ToList();
        foreach (var conditionIndex in conditionIndexes)
        {
            var currentConditionIndex = conditionIndex;
            fields.Add(CreateField($"Condition [{conditionIndex}]", records, record =>
            {
                var condition = FindPerkRankActivityEvaluator(record, rankIndex, activityIndex, evaluatorIndex)?.Conditions.FirstOrDefault(item => item.ConditionIndex == currentConditionIndex);
                return FormatConditionRuleSummary(condition);
            }));
        }
    }

    private static void AddPerkBackgroundSkillGroup(
        IList<RecordComparisonFieldDTO> fields,
        IReadOnlyList<PerkDTO> records)
    {
        var skillIndexes = records
            .SelectMany(record => record.BackgroundSkills)
            .Select(skill => skill.SkillIndex)
            .Distinct()
            .Order()
            .ToList();
        if (skillIndexes.Count == 0)
        {
            return;
        }

        var skillFields = new List<RecordComparisonFieldDTO>();
        foreach (var skillIndex in skillIndexes)
        {
            var currentSkillIndex = skillIndex;
            skillFields.Add(CreateField($"Skill [{skillIndex}]", records, record => FormatFormKey(FindPerkBackgroundSkill(record, currentSkillIndex)?.SkillFormKey)));
        }

        fields.Add(CreateGroupField("Background Skills", records.Cast<RecordDTO>().ToList(), skillFields));
    }

    private static void AddTerminalMarkerParameterGroups(
        IList<RecordComparisonFieldDTO> fields,
        IReadOnlyList<TerminalDTO> records)
    {
        var parameterIndexes = records
            .SelectMany(record => record.MarkerParameters)
            .Select(parameter => parameter.ParameterIndex)
            .Distinct()
            .Order()
            .ToList();
        if (parameterIndexes.Count == 0)
        {
            return;
        }

        var parameterFields = new List<RecordComparisonFieldDTO>();
        foreach (var parameterIndex in parameterIndexes)
        {
            var currentIndex = parameterIndex;
            var parameterChildren = new List<RecordComparisonFieldDTO>
            {
                CreateField("Offset", records, record => record.MarkerParameters.FirstOrDefault(parameter => parameter.ParameterIndex == currentIndex)?.Offset ?? string.Empty),
                CreateField("EntryTypes", records, record => record.MarkerParameters.FirstOrDefault(parameter => parameter.ParameterIndex == currentIndex)?.EntryTypes ?? string.Empty),
                CreateField("ExitTypes", records, record => record.MarkerParameters.FirstOrDefault(parameter => parameter.ParameterIndex == currentIndex)?.ExitTypes ?? string.Empty),
                CreateField("Enabled", records, record => record.MarkerParameters.FirstOrDefault(parameter => parameter.ParameterIndex == currentIndex)?.Enabled?.ToString() ?? string.Empty),
                CreateField("Unknown", records, record => record.MarkerParameters.FirstOrDefault(parameter => parameter.ParameterIndex == currentIndex)?.Unknown ?? string.Empty)
            }
                .Where(HasVisibleValue)
                .ToList();
            if (parameterChildren.Count > 0)
            {
                parameterFields.Add(CreateGroupField($"Marker Parameter [{parameterIndex}]", records.Cast<RecordDTO>().ToList(), parameterChildren));
            }
        }

        if (parameterFields.Count > 0)
        {
            fields.Add(CreateGroupField("Marker Parameters", records.Cast<RecordDTO>().ToList(), parameterFields));
        }
    }

    private static IEnumerable<RecordComparisonFieldDTO> CreatePerkEffectFields<TEffect>(
        IReadOnlyList<PerkDTO> records,
        IReadOnlyList<LocalizedStringDTO> localizedStrings,
        Language recordTextLanguage,
        string buttonLabelSourceField,
        Func<PerkDTO, TEffect?> findEffect)
        where TEffect : class
    {
        return new List<RecordComparisonFieldDTO>
        {
            CreateField("MutagenObjectType", records, record => GetPerkEffectValue(findEffect(record), effect => effect.MutagenObjectType)),
            CreateField("Rank", records, record => GetPerkEffectValue(findEffect(record), effect => effect.Rank?.ToString() ?? string.Empty)),
            CreateField("Priority", records, record => GetPerkEffectValue(findEffect(record), effect => effect.Priority?.ToString() ?? string.Empty)),
            CreateField("PerkEntryID", records, record => GetPerkEffectValue(findEffect(record), effect => effect.PerkEntryId?.ToString() ?? string.Empty)),
            CreateField("Flags", records, record => GetPerkEffectValue(findEffect(record), effect => effect.Flags ?? string.Empty)),
            CreateField("ButtonLabel", records, record => GetTranslatedDisplayValue(localizedStrings, record, buttonLabelSourceField, recordTextLanguage, GetPerkEffectTranslatedValue(findEffect(record)))),
            CreateField("ConditionCount", records, record => GetPerkEffectValue(findEffect(record), effect => effect.ConditionCount?.ToString() ?? string.Empty)),
            CreateField("EntryPoint", records, record => GetPerkEffectValue(findEffect(record), effect => effect.EntryPoint ?? string.Empty)),
            CreateField("PerkConditionTabCount", records, record => GetPerkEffectValue(findEffect(record), effect => effect.PerkConditionTabCount?.ToString() ?? string.Empty)),
            CreateField("Modification", records, record => GetPerkEffectValue(findEffect(record), effect => effect.Modification ?? string.Empty)),
            CreateField("Value", records, record => GetPerkEffectValue(findEffect(record), effect => effect.Value?.ToString() ?? string.Empty)),
            CreateField("ActorValue", records, record => GetPerkEffectValue(findEffect(record), effect => effect.ActorValue ?? string.Empty)),
            CreateField("Spell", records, record => GetPerkEffectValue(findEffect(record), effect => effect.Spell ?? string.Empty)),
            CreateField("Quest", records, record => GetPerkEffectValue(findEffect(record), effect => effect.Quest ?? string.Empty)),
            CreateField("Stage", records, record => GetPerkEffectValue(findEffect(record), effect => effect.Stage?.ToString() ?? string.Empty))
        }.Where(HasVisibleValue);
    }

    private static void AddPerkEffectConditionTabGroups(
        IList<RecordComparisonFieldDTO> fields,
        IReadOnlyList<PerkDTO> records,
        Func<PerkDTO, IList<PerkEffectConditionTabDTO>?> findConditionTabs)
    {
        var conditionTabIndexes = records
            .SelectMany(record => findConditionTabs(record) ?? [])
            .Select(conditionTab => conditionTab.ConditionTabIndex)
            .Distinct()
            .Order()
            .ToList();
        if (conditionTabIndexes.Count == 0)
        {
            return;
        }

        var conditionTabFields = new List<RecordComparisonFieldDTO>();
        foreach (var conditionTabIndex in conditionTabIndexes)
        {
            var currentConditionTabIndex = conditionTabIndex;
            var children = new List<RecordComparisonFieldDTO>
            {
                CreateField("RunOnTabIndex", records, record => FindConditionTab(findConditionTabs(record), currentConditionTabIndex)?.RunOnTabIndex?.ToString() ?? string.Empty),
                CreateField("ConditionCount", records, record => FindConditionTab(findConditionTabs(record), currentConditionTabIndex)?.ConditionCount.ToString() ?? string.Empty)
            }.Where(HasVisibleValue).ToList();
            AddPerkEffectConditionRuleGroups(children, records, findConditionTabs, currentConditionTabIndex);
            if (children.Count > 0)
            {
                conditionTabFields.Add(CreateGroupField($"Condition Tab [{conditionTabIndex}]", records.Cast<RecordDTO>().ToList(), children));
            }
        }

        if (conditionTabFields.Count > 0)
        {
            fields.Add(CreateGroupField("Conditions", records.Cast<RecordDTO>().ToList(), conditionTabFields));
        }
    }

    private static void AddPerkEffectConditionRuleGroups(
        IList<RecordComparisonFieldDTO> fields,
        IReadOnlyList<PerkDTO> records,
        Func<PerkDTO, IList<PerkEffectConditionTabDTO>?> findConditionTabs,
        int conditionTabIndex)
    {
        var conditionIndexes = records
            .SelectMany(record => FindConditionTab(findConditionTabs(record), conditionTabIndex)?.Conditions ?? [])
            .Select(condition => condition.ConditionIndex)
            .Distinct()
            .Order()
            .ToList();
        foreach (var conditionIndex in conditionIndexes)
        {
            var currentConditionIndex = conditionIndex;
            fields.Add(CreateField($"Condition [{conditionIndex}]", records, record =>
            {
                var condition = FindConditionTab(findConditionTabs(record), conditionTabIndex)?.Conditions.FirstOrDefault(item => item.ConditionIndex == currentConditionIndex);
                return FormatConditionRuleSummary(condition);
            }));
        }
    }

    private static void AddTerminalForcedLocationGroups(
        IList<RecordComparisonFieldDTO> fields,
        IReadOnlyList<TerminalDTO> records)
    {
        var maxForcedLocationCount = records
            .Select(record => record.ForcedLocations.Count)
            .DefaultIfEmpty()
            .Max();
        for (var forcedLocationIndex = 0; forcedLocationIndex < maxForcedLocationCount; forcedLocationIndex++)
        {
            var currentIndex = forcedLocationIndex;
            fields.Add(CreateField($"ForcedLocations[{forcedLocationIndex}]", records, record => FormatFormKey(record.ForcedLocations.ElementAtOrDefault(currentIndex))));
        }
    }

    private static void AddTerminalBodyTextGroups(
        IList<RecordComparisonFieldDTO> fields,
        IReadOnlyList<TerminalDTO> records,
        IReadOnlyList<LocalizedStringDTO> localizedStrings,
        Language recordTextLanguage)
    {
        var bodyTextIndexes = records
            .SelectMany(record => record.BodyTexts)
            .Select(bodyText => bodyText.BodyTextIndex)
            .Distinct()
            .Order()
            .ToList();
        if (bodyTextIndexes.Count == 0)
        {
            return;
        }

        var bodyTextFields = new List<RecordComparisonFieldDTO>();
        foreach (var bodyTextIndex in bodyTextIndexes)
        {
            var currentIndex = bodyTextIndex;
            var bodyTextChildren = new List<RecordComparisonFieldDTO>
            {
                CreateField("Text", records, record => GetTranslatedDisplayValue(
                    localizedStrings,
                    record,
                    $"BodyTexts[{currentIndex}].Text",
                    recordTextLanguage,
                    record.BodyTexts.FirstOrDefault(bodyText => bodyText.BodyTextIndex == currentIndex)?.Text))
            }
                .Where(HasVisibleValue)
                .ToList();
            if (bodyTextChildren.Count > 0)
            {
                bodyTextFields.Add(CreateGroupField($"BodyText [{bodyTextIndex}]", records.Cast<RecordDTO>().ToList(), bodyTextChildren));
            }
        }

        if (bodyTextFields.Count > 0)
        {
            fields.Add(CreateGroupField("BodyTexts", records.Cast<RecordDTO>().ToList(), bodyTextFields));
        }
    }

    private static void AddTerminalMenuItemGroups(
        IList<RecordComparisonFieldDTO> fields,
        IReadOnlyList<TerminalDTO> records,
        IReadOnlyList<LocalizedStringDTO> localizedStrings,
        Language recordTextLanguage)
    {
        var menuItemIndexes = records
            .SelectMany(record => record.MenuItems)
            .Select(menuItem => menuItem.MenuItemIndex)
            .Distinct()
            .Order()
            .ToList();
        if (menuItemIndexes.Count == 0)
        {
            return;
        }

        var menuItemFields = new List<RecordComparisonFieldDTO>();
        foreach (var menuItemIndex in menuItemIndexes)
        {
            var currentIndex = menuItemIndex;
            var menuItemChildren = new List<RecordComparisonFieldDTO>
            {
                CreateField("ItemText", records, record => GetTranslatedDisplayValue(
                    localizedStrings,
                    record,
                    $"MenuItems[{currentIndex}].ItemText",
                    recordTextLanguage,
                    record.MenuItems.FirstOrDefault(menuItem => menuItem.MenuItemIndex == currentIndex)?.ItemText)),
                CreateField("Type", records, record => record.MenuItems.FirstOrDefault(menuItem => menuItem.MenuItemIndex == currentIndex)?.Type ?? string.Empty),
                CreateField("ItemId", records, record => record.MenuItems.FirstOrDefault(menuItem => menuItem.MenuItemIndex == currentIndex)?.ItemId?.ToString() ?? string.Empty),
                CreateField("Submenu", records, record => FormatFormKey(record.MenuItems.FirstOrDefault(menuItem => menuItem.MenuItemIndex == currentIndex)?.Submenu)),
                CreateField("DisplayText", records, record => GetTranslatedDisplayValue(
                    localizedStrings,
                    record,
                    $"MenuItems[{currentIndex}].DisplayText",
                    recordTextLanguage,
                    record.MenuItems.FirstOrDefault(menuItem => menuItem.MenuItemIndex == currentIndex)?.DisplayText))
            }
                .Where(HasVisibleValue)
                .ToList();
            if (menuItemChildren.Count > 0)
            {
                menuItemFields.Add(CreateGroupField($"MenuItem [{menuItemIndex}]", records.Cast<RecordDTO>().ToList(), menuItemChildren));
            }
        }

        if (menuItemFields.Count > 0)
        {
            fields.Add(CreateGroupField("MenuItems", records.Cast<RecordDTO>().ToList(), menuItemFields));
        }
    }

    private static void AddConstructibleObjectComponentGroups(
        IList<RecordComparisonFieldDTO> fields,
        IReadOnlyList<ConstructibleObjectDTO> records)
    {
        var componentIndexes = records
            .SelectMany(record => record.Components)
            .Select(component => component.ComponentIndex)
            .Distinct()
            .Order()
            .ToList();
        if (componentIndexes.Count == 0)
        {
            return;
        }

        var componentFields = new List<RecordComparisonFieldDTO>();
        foreach (var componentIndex in componentIndexes)
        {
            var currentIndex = componentIndex;
            var componentChildren = new List<RecordComparisonFieldDTO>
            {
                CreateField("ComponentFormKey", records, record => FormatFormKey(record.Components.FirstOrDefault(component => component.ComponentIndex == currentIndex)?.ComponentFormKey)),
                CreateField("Count", records, record => record.Components.FirstOrDefault(component => component.ComponentIndex == currentIndex)?.Count?.ToString() ?? string.Empty)
            }
                .Where(HasVisibleValue)
                .ToList();
            if (componentChildren.Count > 0)
            {
                componentFields.Add(CreateGroupField($"Component [{componentIndex}]", records.Cast<RecordDTO>().ToList(), componentChildren));
            }
        }

        if (componentFields.Count > 0)
        {
            fields.Add(CreateGroupField("Components", records.Cast<RecordDTO>().ToList(), componentFields));
        }
    }

    private static void AddMiscItemComponentGroups(
        IList<RecordComparisonFieldDTO> fields,
        IReadOnlyList<MiscItemDTO> records)
    {
        var componentIndexes = records
            .SelectMany(record => record.Components)
            .Select(component => component.ComponentIndex)
            .Distinct()
            .Order()
            .ToList();
        if (componentIndexes.Count == 0)
        {
            return;
        }

        var componentFields = new List<RecordComparisonFieldDTO>();
        foreach (var componentIndex in componentIndexes)
        {
            var currentIndex = componentIndex;
            var componentChildren = new List<RecordComparisonFieldDTO>
            {
                CreateField("Component", records, record => FormatFormKey(record.Components.FirstOrDefault(component => component.ComponentIndex == currentIndex)?.Component)),
                CreateField("DisplayIndex", records, record => record.Components.FirstOrDefault(component => component.ComponentIndex == currentIndex)?.DisplayIndex?.ToString() ?? string.Empty),
                CreateField("Count", records, record => record.Components.FirstOrDefault(component => component.ComponentIndex == currentIndex)?.Count?.ToString() ?? string.Empty)
            }
                .Where(HasVisibleValue)
                .ToList();
            if (componentChildren.Count > 0)
            {
                componentFields.Add(CreateGroupField($"Component [{componentIndex}]", records.Cast<RecordDTO>().ToList(), componentChildren));
            }
        }

        if (componentFields.Count > 0)
        {
            fields.Add(CreateGroupField("Components", records.Cast<RecordDTO>().ToList(), componentFields));
        }
    }

    private static void AddMiscItemResourceGroups(
        IList<RecordComparisonFieldDTO> fields,
        IReadOnlyList<MiscItemDTO> records)
    {
        var resourceIndexes = records
            .SelectMany(record => record.Resources)
            .Select(resource => resource.ResourceIndex)
            .Distinct()
            .Order()
            .ToList();
        if (resourceIndexes.Count == 0)
        {
            return;
        }

        var resourceFields = new List<RecordComparisonFieldDTO>();
        foreach (var resourceIndex in resourceIndexes)
        {
            var currentIndex = resourceIndex;
            var resourceChildren = new List<RecordComparisonFieldDTO>
            {
                CreateField("Resource", records, record => FormatFormKey(record.Resources.FirstOrDefault(resource => resource.ResourceIndex == currentIndex)?.Resource)),
                CreateField("Count", records, record => record.Resources.FirstOrDefault(resource => resource.ResourceIndex == currentIndex)?.Count?.ToString() ?? string.Empty)
            }
                .Where(HasVisibleValue)
                .ToList();
            if (resourceChildren.Count > 0)
            {
                resourceFields.Add(CreateGroupField($"Resource [{resourceIndex}]", records.Cast<RecordDTO>().ToList(), resourceChildren));
            }
        }

        if (resourceFields.Count > 0)
        {
            fields.Add(CreateGroupField("Resources", records.Cast<RecordDTO>().ToList(), resourceFields));
        }
    }

    private static void AddMiscItemDestructibleGroups(
        IList<RecordComparisonFieldDTO> fields,
        IReadOnlyList<MiscItemDTO> records)
    {
        if (records.All(record => record.Destructible == null))
        {
            return;
        }

        var baseRecords = records.Cast<RecordDTO>().ToList();
        var children = new List<RecordComparisonFieldDTO>
        {
            CreateField("Health", records, record => record.Destructible?.Data?.Health?.ToString() ?? string.Empty),
            CreateField("DESTCount", records, record => record.Destructible?.Data?.DESTCount?.ToString() ?? string.Empty)
        };

        var stageIndexes = records
            .SelectMany(record => record.Destructible?.Stages ?? [])
            .Select(stage => stage.StageIndex)
            .Distinct()
            .Order()
            .ToList();
        foreach (var stageIndex in stageIndexes)
        {
            var currentIndex = stageIndex;
            var stageChildren = new List<RecordComparisonFieldDTO>
            {
                CreateField("Index", records, record => record.Destructible?.Stages.FirstOrDefault(stage => stage.StageIndex == currentIndex)?.Index?.ToString() ?? string.Empty),
                CreateField("HealthPercent", records, record => record.Destructible?.Stages.FirstOrDefault(stage => stage.StageIndex == currentIndex)?.HealthPercent?.ToString() ?? string.Empty),
                CreateField("ModelDamageStage", records, record => record.Destructible?.Stages.FirstOrDefault(stage => stage.StageIndex == currentIndex)?.ModelDamageStage?.ToString() ?? string.Empty),
                CreateField("Flags", records, record => record.Destructible?.Stages.FirstOrDefault(stage => stage.StageIndex == currentIndex)?.Flags ?? string.Empty),
                CreateField("SelfDamagePerSecond", records, record => record.Destructible?.Stages.FirstOrDefault(stage => stage.StageIndex == currentIndex)?.SelfDamagePerSecond?.ToString() ?? string.Empty),
                CreateField("Explosion", records, record => FormatFormKey(record.Destructible?.Stages.FirstOrDefault(stage => stage.StageIndex == currentIndex)?.Explosion)),
                CreateField("Model.File", records, record => record.Destructible?.Stages.FirstOrDefault(stage => stage.StageIndex == currentIndex)?.Model?.File ?? string.Empty),
                CreateField("Model.Data", records, record => record.Destructible?.Stages.FirstOrDefault(stage => stage.StageIndex == currentIndex)?.Model?.Data ?? string.Empty)
            }
                .Where(HasVisibleValue)
                .ToList();
            if (stageChildren.Count > 0)
            {
                children.Add(CreateGroupField($"Stage [{stageIndex}]", baseRecords, stageChildren));
            }
        }

        var visibleChildren = children
            .Where(field => field.Children.Count > 0 || HasVisibleValue(field))
            .ToList();
        if (visibleChildren.Count > 0)
        {
            fields.Add(CreateGroupField("Destructible", baseRecords, visibleChildren));
        }
    }

    private static void AddClassPropertyGroups(IList<RecordComparisonFieldDTO> fields, IReadOnlyList<ClassDTO> records)
    {
        var propertyIndexes = records.SelectMany(record => record.Properties).Select(property => property.PropertyIndex).Distinct().Order().ToList();
        if (propertyIndexes.Count == 0) return;
        var propertyFields = new List<RecordComparisonFieldDTO>();
        foreach (var propertyIndex in propertyIndexes)
        {
            var currentIndex = propertyIndex;
            var children = new List<RecordComparisonFieldDTO>
            {
                CreateField("ActorValueFormKey", records, record => FormatFormKey(record.Properties.FirstOrDefault(property => property.PropertyIndex == currentIndex)?.ActorValueFormKey)),
                CreateField("Value", records, record => record.Properties.FirstOrDefault(property => property.PropertyIndex == currentIndex)?.Value?.ToString() ?? string.Empty)
            }.Where(HasVisibleValue).ToList();
            if (children.Count > 0)
            {
                propertyFields.Add(CreateGroupField($"Property [{propertyIndex}]", records.Cast<RecordDTO>().ToList(), children));
            }
        }

        if (propertyFields.Count > 0)
        {
            fields.Add(CreateGroupField("Properties", records.Cast<RecordDTO>().ToList(), propertyFields));
        }
    }

    private static void AddClassWeightGroups(IList<RecordComparisonFieldDTO> fields, IReadOnlyList<ClassDTO> records, string weightType, string groupName)
    {
        var weights = records.SelectMany(record => string.Equals(weightType, "Skill", StringComparison.Ordinal) ? record.SkillWeights : record.StatWeights).ToList();
        var weightIndexes = weights.Select(weight => weight.WeightIndex).Distinct().Order().ToList();
        if (weightIndexes.Count == 0) return;
        var weightFields = new List<RecordComparisonFieldDTO>();
        foreach (var weightIndex in weightIndexes)
        {
            var currentIndex = weightIndex;
            var children = new List<RecordComparisonFieldDTO>
            {
                CreateField("Key", records, record => FindClassWeight(record, weightType, currentIndex)?.Key ?? string.Empty),
                CreateField("Value", records, record => FindClassWeight(record, weightType, currentIndex)?.Value?.ToString() ?? string.Empty)
            }.Where(HasVisibleValue).ToList();
            if (children.Count > 0)
            {
                weightFields.Add(CreateGroupField($"{weightType}Weight [{weightIndex}]", records.Cast<RecordDTO>().ToList(), children));
            }
        }

        if (weightFields.Count > 0)
        {
            fields.Add(CreateGroupField(groupName, records.Cast<RecordDTO>().ToList(), weightFields));
        }
    }

    private static void AddFactionRelationGroups(IList<RecordComparisonFieldDTO> fields, IReadOnlyList<FactionDTO> records)
    {
        var relationIndexes = records.SelectMany(record => record.Relations).Select(relation => relation.RelationIndex).Distinct().Order().ToList();
        if (relationIndexes.Count == 0) return;
        var relationFields = new List<RecordComparisonFieldDTO>();
        foreach (var relationIndex in relationIndexes)
        {
            var currentIndex = relationIndex;
            var children = new List<RecordComparisonFieldDTO>
            {
                CreateField("Target", records, record => FormatFormKey(record.Relations.FirstOrDefault(relation => relation.RelationIndex == currentIndex)?.Target)),
                CreateField("Reaction", records, record => record.Relations.FirstOrDefault(relation => relation.RelationIndex == currentIndex)?.Reaction ?? string.Empty)
            }.Where(HasVisibleValue).ToList();
            if (children.Count > 0)
            {
                relationFields.Add(CreateGroupField($"Relation [{relationIndex}]", records.Cast<RecordDTO>().ToList(), children));
            }
        }

        if (relationFields.Count > 0)
        {
            fields.Add(CreateGroupField("Relations", records.Cast<RecordDTO>().ToList(), relationFields));
        }
    }

    private static void AddFactionRankGroups(IList<RecordComparisonFieldDTO> fields, IReadOnlyList<FactionDTO> records, IReadOnlyList<LocalizedStringDTO> localizedStrings, Language recordTextLanguage)
    {
        var rankIndexes = records.SelectMany(record => record.Ranks).Select(rank => rank.RankIndex).Distinct().Order().ToList();
        if (rankIndexes.Count == 0) return;
        var rankFields = new List<RecordComparisonFieldDTO>();
        foreach (var rankIndex in rankIndexes)
        {
            var currentIndex = rankIndex;
            var children = new List<RecordComparisonFieldDTO>
            {
                CreateField("Number", records, record => record.Ranks.FirstOrDefault(rank => rank.RankIndex == currentIndex)?.Number?.ToString() ?? string.Empty),
                CreateField("Title.Male", records, record => GetTranslatedDisplayValue(localizedStrings, record, $"Ranks[{currentIndex}].Title.Male", recordTextLanguage, record.Ranks.FirstOrDefault(rank => rank.RankIndex == currentIndex)?.Title?.Male)),
                CreateField("Title.Female", records, record => GetTranslatedDisplayValue(localizedStrings, record, $"Ranks[{currentIndex}].Title.Female", recordTextLanguage, record.Ranks.FirstOrDefault(rank => rank.RankIndex == currentIndex)?.Title?.Female))
            }.Where(HasVisibleValue).ToList();
            if (children.Count > 0)
            {
                rankFields.Add(CreateGroupField($"Rank [{rankIndex}]", records.Cast<RecordDTO>().ToList(), children));
            }
        }

        if (rankFields.Count > 0)
        {
            fields.Add(CreateGroupField("Ranks", records.Cast<RecordDTO>().ToList(), rankFields));
        }
    }

    private static void AddConditionRuleGroups(
        IList<RecordComparisonFieldDTO> fields,
        IReadOnlyList<RecordDTO> baseRecords,
        IReadOnlyList<IHasConditionsDTO> records)
    {
        var conditionKeys = records
            .SelectMany(record => record.Conditions)
            .Select(condition => new ConditionRuleKey(condition.ConditionSlot, condition.ConditionIndex))
            .Distinct()
            .OrderBy(key => key.Slot, StringComparer.Ordinal)
            .ThenBy(key => key.Index)
            .ToList();
        if (conditionKeys.Count == 0) return;
        var conditionFields = new List<RecordComparisonFieldDTO>();
        foreach (var conditionKey in conditionKeys)
        {
            var currentKey = conditionKey;
            conditionFields.Add(CreateField(GetConditionRuleGroupName(currentKey), baseRecords, record => FormatConditionRuleSummary(FindConditionRule(records, baseRecords, record.ModKey, currentKey))));
        }

        if (conditionFields.Count > 0)
        {
            fields.Add(CreateGroupField("Conditions", baseRecords, conditionFields));
        }
    }

    private static void AddRecordComponentGroups(IList<RecordComparisonFieldDTO> fields, IReadOnlyList<RecordDTO> records, IReadOnlyList<RecordComponentDTO> components)
    {
        var componentIndexes = components.Select(component => component.ComponentIndex).Distinct().Order().ToList();
        if (componentIndexes.Count == 0) return;
        var componentFields = new List<RecordComparisonFieldDTO>();
        foreach (var componentIndex in componentIndexes)
        {
            var currentIndex = componentIndex;
            var children = new List<RecordComparisonFieldDTO>
            {
                CreateChildField("MutagenObjectType", records, record => FindRecordComponent(components, record.ModKey, currentIndex)?.MutagenObjectType ?? string.Empty),
                CreateChildField("DCED", records, record => FormatIntegerList(FindRecordComponent(components, record.ModKey, currentIndex)?.DCED))
            }.Where(HasVisibleValue).ToList();
            AddRecordComponentItemGroups(children, records, components, currentIndex);
            if (children.Count > 0)
            {
                componentFields.Add(CreateGroupField($"Component [{componentIndex}]", records, children));
            }
        }

        if (componentFields.Count > 0)
        {
            fields.Add(CreateGroupField("Components", records, componentFields));
        }
    }

    private static void AddRecordComponentItemGroups(IList<RecordComparisonFieldDTO> fields, IReadOnlyList<RecordDTO> records, IReadOnlyList<RecordComponentDTO> components, int componentIndex)
    {
        var itemIndexes = components
            .Where(component => component.ComponentIndex == componentIndex)
            .SelectMany(component => component.Items)
            .Select(item => item.ItemIndex)
            .Distinct()
            .Order()
            .ToList();
        foreach (var itemIndex in itemIndexes)
        {
            var currentItemIndex = itemIndex;
            var children = new List<RecordComparisonFieldDTO>
            {
                CreateChildField("DisplayFilter", records, record => FormatFormKey(FindRecordComponentItem(components, record.ModKey, componentIndex, currentItemIndex)?.DisplayFilter)),
                CreateChildField("Unknown1", records, record => FindRecordComponentItem(components, record.ModKey, componentIndex, currentItemIndex)?.Unknown1?.ToString() ?? string.Empty),
                CreateChildField("Unknown2", records, record => FindRecordComponentItem(components, record.ModKey, componentIndex, currentItemIndex)?.Unknown2?.ToString() ?? string.Empty),
                CreateChildField("Unknown3", records, record => FindRecordComponentItem(components, record.ModKey, componentIndex, currentItemIndex)?.Unknown3?.ToString() ?? string.Empty),
                CreateChildField("Unknown4", records, record => FindRecordComponentItem(components, record.ModKey, componentIndex, currentItemIndex)?.Unknown4?.ToString() ?? string.Empty),
                CreateChildField("Unknown5", records, record => FindRecordComponentItem(components, record.ModKey, componentIndex, currentItemIndex)?.Unknown5?.ToString() ?? string.Empty)
            }.Where(HasVisibleValue).ToList();
            if (children.Count > 0)
            {
                fields.Add(CreateGroupField($"Item [{itemIndex}]", records, children));
            }
        }
    }

    private static void AddConstructibleObjectCategoryGroups(
        IList<RecordComparisonFieldDTO> fields,
        IReadOnlyList<ConstructibleObjectDTO> records)
    {
        var categoryKeys = records
            .SelectMany(record => record.Categories)
            .Select(category => category.CategoryIndex)
            .Distinct()
            .Order()
            .ToList();
        if (categoryKeys.Count == 0)
        {
            return;
        }

        var categoryFields = new List<RecordComparisonFieldDTO>();
        foreach (var categoryIndex in categoryKeys)
        {
            var currentIndex = categoryIndex;
            var categoryChildren = new List<RecordComparisonFieldDTO>
            {
                CreateField("CategoryFormKey", records, record => FormatFormKey(record.Categories.FirstOrDefault(category => category.CategoryIndex == currentIndex)?.CategoryFormKey))
            }
                .Where(HasVisibleValue)
                .ToList();
            if (categoryChildren.Count > 0)
            {
                categoryFields.Add(CreateGroupField($"Category [{categoryIndex}]", records.Cast<RecordDTO>().ToList(), categoryChildren));
            }
        }

        if (categoryFields.Count > 0)
        {
            fields.Add(CreateGroupField("Categories", records.Cast<RecordDTO>().ToList(), categoryFields));
        }
    }

    private static void AddConstructibleObjectRecipeFilterGroups(
        IList<RecordComparisonFieldDTO> fields,
        IReadOnlyList<ConstructibleObjectDTO> records)
    {
        var recipeFilterIndexes = records
            .SelectMany(record => record.RecipeFilters)
            .Select(recipeFilter => recipeFilter.RecipeFilterIndex)
            .Distinct()
            .Order()
            .ToList();
        if (recipeFilterIndexes.Count == 0)
        {
            return;
        }

        var recipeFilterFields = new List<RecordComparisonFieldDTO>();
        foreach (var recipeFilterIndex in recipeFilterIndexes)
        {
            var currentIndex = recipeFilterIndex;
            var recipeFilterChildren = new List<RecordComparisonFieldDTO>
            {
                CreateField("RecipeFilterFormKey", records, record => FormatFormKey(record.RecipeFilters.FirstOrDefault(recipeFilter => recipeFilter.RecipeFilterIndex == currentIndex)?.RecipeFilterFormKey))
            }
                .Where(HasVisibleValue)
                .ToList();
            if (recipeFilterChildren.Count > 0)
            {
                recipeFilterFields.Add(CreateGroupField($"RecipeFilter [{recipeFilterIndex}]", records.Cast<RecordDTO>().ToList(), recipeFilterChildren));
            }
        }

        if (recipeFilterFields.Count > 0)
        {
            fields.Add(CreateGroupField("RecipeFilters", records.Cast<RecordDTO>().ToList(), recipeFilterFields));
        }
    }

    private static void AddActorValueInformationPerkTreeGroups(
        IList<RecordComparisonFieldDTO> fields,
        IReadOnlyList<ActorValueInformationDTO> records)
    {
        var perkTreeIndexes = records
            .SelectMany(record => record.PerkTree)
            .Select(entry => entry.PerkTreeIndex)
            .Distinct()
            .Order()
            .ToList();
        if (perkTreeIndexes.Count == 0)
        {
            return;
        }

        var perkTreeFields = new List<RecordComparisonFieldDTO>();
        foreach (var perkTreeIndex in perkTreeIndexes)
        {
            var currentIndex = perkTreeIndex;
            var perkTreeChildren = new List<RecordComparisonFieldDTO>
            {
                CreateField("AssociatedSkill", records, record => FormatFormKey(record.PerkTree.FirstOrDefault(entry => entry.PerkTreeIndex == currentIndex)?.AssociatedSkill)),
                CreateField("FNAM", records, record => record.PerkTree.FirstOrDefault(entry => entry.PerkTreeIndex == currentIndex)?.FNAM ?? string.Empty),
                CreateField("HorizontalPosition", records, record => record.PerkTree.FirstOrDefault(entry => entry.PerkTreeIndex == currentIndex)?.HorizontalPosition?.ToString() ?? string.Empty),
                CreateField("Index", records, record => record.PerkTree.FirstOrDefault(entry => entry.PerkTreeIndex == currentIndex)?.Index?.ToString() ?? string.Empty),
                CreateField("PerkGridX", records, record => record.PerkTree.FirstOrDefault(entry => entry.PerkTreeIndex == currentIndex)?.PerkGridX?.ToString() ?? string.Empty),
                CreateField("PerkGridY", records, record => record.PerkTree.FirstOrDefault(entry => entry.PerkTreeIndex == currentIndex)?.PerkGridY?.ToString() ?? string.Empty),
                CreateField("VerticalPosition", records, record => record.PerkTree.FirstOrDefault(entry => entry.PerkTreeIndex == currentIndex)?.VerticalPosition?.ToString() ?? string.Empty),
                CreateField("Perk", records, record => FormatFormKey(record.PerkTree.FirstOrDefault(entry => entry.PerkTreeIndex == currentIndex)?.Perk)),
                CreateField("ConnectionLineToIndices", records, record => FormatActorValueInformationConnectionLineIndices(record.PerkTree.FirstOrDefault(entry => entry.PerkTreeIndex == currentIndex)))
            }
                .Where(HasVisibleValue)
                .ToList();
            if (perkTreeChildren.Count > 0)
            {
                perkTreeFields.Add(CreateGroupField($"PerkTree [{perkTreeIndex}]", records.Cast<RecordDTO>().ToList(), perkTreeChildren));
            }
        }

        if (perkTreeFields.Count > 0)
        {
            fields.Add(CreateGroupField("PerkTree", records.Cast<RecordDTO>().ToList(), perkTreeFields));
        }
    }

    private static string FormatActorValueInformationConnectionLineIndices(ActorValueInformationPerkTreeEntryDTO? entry)
    {
        return entry == null
            ? string.Empty
            : string.Join(", ", entry.ConnectionLineToIndices.OrderBy(connectionLineIndex => connectionLineIndex.ConnectionLineIndex).Select(connectionLineIndex => connectionLineIndex.TargetIndex.ToString()));
    }

    private static void AddRawPayloadGroups(
        IList<RecordComparisonFieldDTO> fields,
        IReadOnlyList<RecordDTO> records,
        IReadOnlyList<RawRecordPayloadDTO> payloads)
    {
        var payloadKeys = payloads
            .Select(payload => new RawPayloadKey(payload.PayloadSlot, payload.PayloadIndex))
            .Distinct()
            .OrderBy(key => key.Slot, StringComparer.Ordinal)
            .ThenBy(key => key.Index)
            .ToList();
        if (payloadKeys.Count == 0)
        {
            return;
        }

        var payloadFields = new List<RecordComparisonFieldDTO>();
        foreach (var payloadKey in payloadKeys)
        {
            var payloadChildren = new List<RecordComparisonFieldDTO>
            {
                CreateChildField("Type", records, record => FindRawPayload(payloads, record.ModKey, payloadKey)?.PayloadType ?? string.Empty),
                CreateRawPayloadValueField(records, payloads, payloadKey)
            }
                .Where(HasVisibleValue)
                .ToList();
            if (payloadChildren.Count > 0)
            {
                payloadFields.Add(CreateGroupField(GetRawPayloadGroupName(payloadKey), records, payloadChildren));
            }
        }

        if (payloadFields.Count > 0)
        {
            fields.Add(CreateGroupField("Raw Payloads", records, payloadFields));
        }
    }

    /// <summary>
    /// Adds comparison groups for component reflection rows imported from Spriggit <c>REFL</c> fields.
    /// </summary>
    /// <param name="fields">The comparison field list being built for the parent record.</param>
    /// <param name="records">The ordered parent records participating in the comparison.</param>
    /// <param name="reflections">The reflection rows read back for the compared form key.</param>
    private static void AddReflectionGroups(
        IList<RecordComparisonFieldDTO> fields,
        IReadOnlyList<RecordDTO> records,
        IReadOnlyList<ReflectionDTO> reflections)
    {
        var reflectionKeys = reflections
            .Select(reflection => reflection.ComponentIndex)
            .Distinct()
            .OrderBy(componentIndex => componentIndex)
            .ToList();
        if (reflectionKeys.Count == 0)
        {
            return;
        }

        var reflectionFields = new List<RecordComparisonFieldDTO>();
        foreach (var componentIndex in reflectionKeys)
        {
            var reflectionChildren = new List<RecordComparisonFieldDTO>
            {
                CreateChildField("ComponentType", records, record => FindReflection(reflections, record.ModKey, componentIndex)?.ComponentType ?? string.Empty),
                CreateChildField("SourcePath", records, record => FindReflection(reflections, record.ModKey, componentIndex)?.SourcePath ?? string.Empty),
                CreateReflectionValueField(records, reflections, componentIndex)
            }
                .Where(HasVisibleValue)
                .ToList();
            if (reflectionChildren.Count > 0)
            {
                reflectionFields.Add(CreateGroupField("Components[" + componentIndex.ToString(CultureInfo.InvariantCulture) + "].REFL", records, reflectionChildren));
            }
        }

        if (reflectionFields.Count > 0)
        {
            fields.Add(CreateGroupField("Reflection", records, reflectionFields));
        }
    }

    /// <summary>
    /// Adds NPC scalar fields that are not part of the common actor data block but are first-class persisted values.
    /// </summary>
    /// <param name="fields">The comparison field collection being populated.</param>
    /// <param name="records">The NPC records participating in the comparison.</param>
    private static void AddNPCSupplementalFields(ICollection<RecordComparisonFieldDTO> fields, IReadOnlyList<NPCDTO> records)
    {
        fields.Add(CreateField("Class", records, record => FormatFormKey(record.Class)));
        fields.Add(CreateField("DeathItem", records, record => FormatFormKey(record.DeathItem)));
        fields.Add(CreateField("DefaultOutfit", records, record => FormatFormKey(record.DefaultOutfit)));
        fields.Add(CreateField("Template", records, record => FormatFormKey(record.Template)));
        fields.Add(CreateField("DefaultTemplate", records, record => FormatFormKey(record.DefaultTemplate)));
        fields.Add(CreateField("WornArmor", records, record => FormatFormKey(record.WornArmor)));
        fields.Add(CreateField("HeadTexture", records, record => FormatFormKey(record.HeadTexture)));
        fields.Add(CreateField("SleepingOutfit", records, record => FormatFormKey(record.SleepingOutfit)));
        fields.Add(CreateField("SpaceOutfit", records, record => FormatFormKey(record.SpaceOutfit)));
        fields.Add(CreateField("PowerArmorStand", records, record => FormatFormKey(record.PowerArmorStand)));
        fields.Add(CreateField("UseTemplateActors", records, record => record.UseTemplateActors ?? string.Empty));
        fields.Add(CreateField("CalculatedHealth", records, record => record.CalculatedHealth?.ToString(CultureInfo.InvariantCulture) ?? string.Empty));
        fields.Add(CreateField("CalculatedActionPoints", records, record => record.CalculatedActionPoints?.ToString(CultureInfo.InvariantCulture) ?? string.Empty));
        fields.Add(CreateField("XpValueOffset", records, record => record.XpValueOffset?.ToString(CultureInfo.InvariantCulture) ?? string.Empty));
        fields.Add(CreateField("Unknown", records, record => record.Unknown?.ToString(CultureInfo.InvariantCulture) ?? string.Empty));
        fields.Add(CreateField("Unused", records, record => record.Unused?.ToString(CultureInfo.InvariantCulture) ?? string.Empty));
        fields.Add(CreateField("NAM5", records, record => record.NAM5 ?? string.Empty));
        AddNPCWeightGroup(fields, records);
        fields.Add(CreateField("SoundLevel", records, record => record.SoundLevel ?? string.Empty));
        fields.Add(CreateField("TextureLighting", records, record => record.TextureLighting ?? string.Empty));
        fields.Add(CreateField("HairColor", records, record => record.HairColor ?? string.Empty));
        fields.Add(CreateField("FacialHairColor", records, record => record.FacialHairColor ?? string.Empty));
        fields.Add(CreateField("EyebrowColor", records, record => record.EyebrowColor ?? string.Empty));
        fields.Add(CreateField("EyeColor", records, record => record.EyeColor ?? string.Empty));
        AddNPCFaceMorphGroup(fields, records);
        AddNPCFacePartsGroup(fields, records);
        AddNPCTemplateActorsGroup(fields, records);
        fields.Add(CreateField("BodyMorphRegionValues", records, record => record.BodyMorphRegionValues ?? string.Empty));
        fields.Add(CreateField("ObjectTemplates", records, record => record.ObjectTemplates ?? string.Empty));
        fields.Add(CreateField("AIData", records, record => record.AIData ?? string.Empty));
    }

    /// <summary>
    /// Adds the Spriggit <c>Level</c> object used by NPC records when any compared record contains level data.
    /// </summary>
    /// <param name="fields">The comparison field collection being populated.</param>
    /// <param name="records">The NPC records participating in the comparison.</param>
    private static void AddNPCLevelGroup(ICollection<RecordComparisonFieldDTO> fields, IReadOnlyList<NPCDTO> records)
    {
        var children = new List<RecordComparisonFieldDTO>
        {
            CreateField("MutagenObjectType", records, record => record.Level?.MutagenObjectType ?? string.Empty),
            CreateField("Level", records, record => record.Level?.Level?.ToString(CultureInfo.InvariantCulture) ?? string.Empty),
            CreateField("LevelMult", records, record => FormatNumericDisplayValue(record.Level?.LevelMult, null))
        }
            .Where(HasVisibleValue)
            .ToList();

        if (children.Count > 0)
        {
            fields.Add(CreateGroupField("Level", records.Cast<RecordDTO>().ToList(), children));
        }
    }

    /// <summary>
    /// Adds Skyrim NPC configuration values when they are present on at least one compared record.
    /// </summary>
    /// <param name="fields">The comparison field collection being populated.</param>
    /// <param name="records">The NPC records participating in the comparison.</param>
    private static void AddNPCConfigurationGroup(ICollection<RecordComparisonFieldDTO> fields, IReadOnlyList<NPCDTO> records)
    {
        var children = new List<RecordComparisonFieldDTO>
        {
            CreateField("Flags", records, record => record.Configuration is null ? string.Empty : string.Join(", ", record.Configuration.Flags)),
            CreateField("Level.MutagenObjectType", records, record => record.Configuration?.Level?.MutagenObjectType ?? string.Empty),
            CreateField("Level.Level", records, record => record.Configuration?.Level?.Level?.ToString(CultureInfo.InvariantCulture) ?? string.Empty),
            CreateField("Level.LevelMult", records, record => FormatNumericDisplayValue(record.Configuration?.Level?.LevelMult, null)),
            CreateField("CalcMinLevel", records, record => record.Configuration?.CalcMinLevel?.ToString(CultureInfo.InvariantCulture) ?? string.Empty),
            CreateField("CalcMaxLevel", records, record => record.Configuration?.CalcMaxLevel?.ToString(CultureInfo.InvariantCulture) ?? string.Empty),
            CreateField("HealthOffset", records, record => record.Configuration?.HealthOffset?.ToString(CultureInfo.InvariantCulture) ?? string.Empty),
            CreateField("SpeedMultiplier", records, record => record.Configuration?.SpeedMultiplier?.ToString(CultureInfo.InvariantCulture) ?? string.Empty),
            CreateField("TemplateFlags", records, record => record.Configuration is null ? string.Empty : string.Join(", ", record.Configuration.TemplateFlags))
        }
            .Where(HasVisibleValue)
            .ToList();

        if (children.Count > 0)
        {
            fields.Add(CreateGroupField("Configuration", records.Cast<RecordDTO>().ToList(), children));
        }
    }

    /// <summary>
    /// Adds the NPC body-weight group, including scalar and Starfield tri-shape weight fields.
    /// </summary>
    /// <param name="fields">The comparison field collection being populated.</param>
    /// <param name="records">The NPC records participating in the comparison.</param>
    private static void AddNPCWeightGroup(ICollection<RecordComparisonFieldDTO> fields, IReadOnlyList<NPCDTO> records)
    {
        var children = new List<RecordComparisonFieldDTO>
        {
            CreateField("Value", records, record => FormatNumericDisplayValue(record.Weight?.Value, GetNPCWeightPrecision(nameof(NPCWeightDTO.Value)))),
            CreateField("Thin", records, record => FormatNumericDisplayValue(record.Weight?.Thin, GetNPCWeightPrecision(nameof(NPCWeightDTO.Thin)))),
            CreateField("Muscular", records, record => FormatNumericDisplayValue(record.Weight?.Muscular, GetNPCWeightPrecision(nameof(NPCWeightDTO.Muscular)))),
            CreateField("Fat", records, record => FormatNumericDisplayValue(record.Weight?.Fat, GetNPCWeightPrecision(nameof(NPCWeightDTO.Fat))))
        }
            .Where(HasVisibleValue)
            .ToList();

        if (children.Count > 0)
        {
            fields.Add(CreateGroupField("Weight", records.Cast<RecordDTO>().ToList(), children));
        }
    }

    /// <summary>
    /// Reads display precision metadata from NPC weight DTO members.
    /// </summary>
    /// <param name="propertyName">The weight DTO property name to inspect.</param>
    /// <returns>The configured decimal-place count, or <c>null</c> when no precision metadata exists.</returns>
    private static int? GetNPCWeightPrecision(string propertyName)
    {
        return typeof(NPCWeightDTO)
            .GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public)
            ?.GetCustomAttribute<NumericDisplayPrecisionAttribute>()
            ?.DecimalPlaces;
    }

    /// <summary>
    /// Adds Skyrim face morph slider rows when a compared NPC contains the face morph object.
    /// </summary>
    /// <param name="fields">The comparison field collection being populated.</param>
    /// <param name="records">The NPC records participating in the comparison.</param>
    private static void AddNPCFaceMorphGroup(ICollection<RecordComparisonFieldDTO> fields, IReadOnlyList<NPCDTO> records)
    {
        var children = new List<RecordComparisonFieldDTO>
        {
            CreateField("NoseLongVsShort", records, record => FormatNumericDisplayValue(record.FaceMorph?.NoseLongVsShort, null)),
            CreateField("NoseUpVsDown", records, record => FormatNumericDisplayValue(record.FaceMorph?.NoseUpVsDown, null)),
            CreateField("JawUpVsDown", records, record => FormatNumericDisplayValue(record.FaceMorph?.JawUpVsDown, null)),
            CreateField("JawNarrowVsWide", records, record => FormatNumericDisplayValue(record.FaceMorph?.JawNarrowVsWide, null)),
            CreateField("JawForwardVsBack", records, record => FormatNumericDisplayValue(record.FaceMorph?.JawForwardVsBack, null)),
            CreateField("CheeksUpVsDown", records, record => FormatNumericDisplayValue(record.FaceMorph?.CheeksUpVsDown, null)),
            CreateField("CheeksForwardVsBack", records, record => FormatNumericDisplayValue(record.FaceMorph?.CheeksForwardVsBack, null)),
            CreateField("EyesUpVsDown", records, record => FormatNumericDisplayValue(record.FaceMorph?.EyesUpVsDown, null)),
            CreateField("EyesInVsOut", records, record => FormatNumericDisplayValue(record.FaceMorph?.EyesInVsOut, null)),
            CreateField("BrowsUpVsDown", records, record => FormatNumericDisplayValue(record.FaceMorph?.BrowsUpVsDown, null)),
            CreateField("BrowsInVsOut", records, record => FormatNumericDisplayValue(record.FaceMorph?.BrowsInVsOut, null)),
            CreateField("BrowsForwardVsBack", records, record => FormatNumericDisplayValue(record.FaceMorph?.BrowsForwardVsBack, null)),
            CreateField("LipsUpVsDown", records, record => FormatNumericDisplayValue(record.FaceMorph?.LipsUpVsDown, null)),
            CreateField("LipsInVsOut", records, record => FormatNumericDisplayValue(record.FaceMorph?.LipsInVsOut, null)),
            CreateField("ChinNarrowVsWide", records, record => FormatNumericDisplayValue(record.FaceMorph?.ChinNarrowVsWide, null)),
            CreateField("ChinUpVsDown", records, record => FormatNumericDisplayValue(record.FaceMorph?.ChinUpVsDown, null)),
            CreateField("ChinUnderbiteVsOverbite", records, record => FormatNumericDisplayValue(record.FaceMorph?.ChinUnderbiteVsOverbite, null)),
            CreateField("EyesForwardVsBack", records, record => FormatNumericDisplayValue(record.FaceMorph?.EyesForwardVsBack, null)),
            CreateField("Unknown", records, record => FormatNumericDisplayValue(record.FaceMorph?.Unknown, null))
        }
            .Where(HasVisibleValue)
            .ToList();

        if (children.Count > 0)
        {
            fields.Add(CreateGroupField("FaceMorph", records.Cast<RecordDTO>().ToList(), children));
        }
    }

    /// <summary>
    /// Adds Skyrim face part index rows when a compared NPC contains the face parts object.
    /// </summary>
    /// <param name="fields">The comparison field collection being populated.</param>
    /// <param name="records">The NPC records participating in the comparison.</param>
    private static void AddNPCFacePartsGroup(ICollection<RecordComparisonFieldDTO> fields, IReadOnlyList<NPCDTO> records)
    {
        var children = new List<RecordComparisonFieldDTO>
        {
            CreateField("Nose", records, record => record.FaceParts?.Nose?.ToString(CultureInfo.InvariantCulture) ?? string.Empty),
            CreateField("Unknown", records, record => record.FaceParts?.Unknown?.ToString(CultureInfo.InvariantCulture) ?? string.Empty),
            CreateField("Eyes", records, record => record.FaceParts?.Eyes?.ToString(CultureInfo.InvariantCulture) ?? string.Empty),
            CreateField("Mouth", records, record => record.FaceParts?.Mouth?.ToString(CultureInfo.InvariantCulture) ?? string.Empty)
        }
            .Where(HasVisibleValue)
            .ToList();

        if (children.Count > 0)
        {
            fields.Add(CreateGroupField("FaceParts", records.Cast<RecordDTO>().ToList(), children));
        }
    }

    /// <summary>
    /// Adds role-specific template actor references when a compared NPC contains template actor data.
    /// </summary>
    /// <param name="fields">The comparison field collection being populated.</param>
    /// <param name="records">The NPC records participating in the comparison.</param>
    private static void AddNPCTemplateActorsGroup(ICollection<RecordComparisonFieldDTO> fields, IReadOnlyList<NPCDTO> records)
    {
        var children = new List<RecordComparisonFieldDTO>
        {
            CreateField("TraitTemplate", records, record => FormatFormKey(record.TemplateActors?.TraitTemplate)),
            CreateField("StatsTemplate", records, record => FormatFormKey(record.TemplateActors?.StatsTemplate)),
            CreateField("FactionsTemplate", records, record => FormatFormKey(record.TemplateActors?.FactionsTemplate)),
            CreateField("SpellListTemplate", records, record => FormatFormKey(record.TemplateActors?.SpellListTemplate)),
            CreateField("AiPackagesTemplate", records, record => FormatFormKey(record.TemplateActors?.AiPackagesTemplate)),
            CreateField("AiDataTemplate", records, record => FormatFormKey(record.TemplateActors?.AiDataTemplate)),
            CreateField("BaseDataTemplate", records, record => FormatFormKey(record.TemplateActors?.BaseDataTemplate)),
            CreateField("InventoryTemplate", records, record => FormatFormKey(record.TemplateActors?.InventoryTemplate)),
            CreateField("ScriptTemplate", records, record => FormatFormKey(record.TemplateActors?.ScriptTemplate)),
            CreateField("DefPackListTemplate", records, record => FormatFormKey(record.TemplateActors?.DefPackListTemplate)),
            CreateField("AttackDataTemplate", records, record => FormatFormKey(record.TemplateActors?.AttackDataTemplate)),
            CreateField("KeywordsTemplate", records, record => FormatFormKey(record.TemplateActors?.KeywordsTemplate)),
            CreateField("Unknown1", records, record => FormatFormKey(record.TemplateActors?.Unknown1)),
            CreateField("Unknown2", records, record => FormatFormKey(record.TemplateActors?.Unknown2))
        }
            .Where(HasVisibleValue)
            .ToList();

        if (children.Count > 0)
        {
            fields.Add(CreateGroupField("TemplateActors", records.Cast<RecordDTO>().ToList(), children));
        }
    }

    /// <summary>
    /// Adds an indexed group for a simple NPC form-key collection.
    /// </summary>
    /// <param name="fields">The comparison field collection being populated.</param>
    /// <param name="records">The NPC records participating in the comparison.</param>
    /// <param name="groupName">The comparison group name shown for the collection.</param>
    /// <param name="collectionFactory">The accessor that returns the collection for one NPC.</param>
    private static void AddNPCFormKeyListGroup(
        ICollection<RecordComparisonFieldDTO> fields,
        IReadOnlyList<NPCDTO> records,
        string groupName,
        Func<NPCDTO, IList<FormKeyDTO>> collectionFactory)
    {
        var maxCount = records.Select(record => collectionFactory(record).Count).DefaultIfEmpty(0).Max();
        if (maxCount == 0)
        {
            return;
        }

        var children = new List<RecordComparisonFieldDTO>();
        for (var index = 0; index < maxCount; index++)
        {
            var currentIndex = index;
            children.Add(CreateField(
                GetNPCSingularRowName(groupName) + $" [{currentIndex}]",
                records,
                record => collectionFactory(record).Count > currentIndex
                    ? FormatFormKey(collectionFactory(record)[currentIndex])
                    : string.Empty));
        }

        var visibleChildren = children.Where(HasVisibleValue).ToList();
        if (visibleChildren.Count > 0)
        {
            fields.Add(CreateGroupField(groupName, records.Cast<RecordDTO>().ToList(), visibleChildren));
        }
    }

    /// <summary>
    /// Gets the singular row label used for a simple NPC form-key collection.
    /// </summary>
    /// <param name="groupName">The plural comparison group name.</param>
    /// <returns>The singular comparison row label.</returns>
    private static string GetNPCSingularRowName(string groupName)
    {
        return groupName switch
        {
            "HeadParts" => "HeadPart",
            "ActorEffects" => "ActorEffect",
            "ForcedLocations" => "ForcedLocation",
            "Packages" => "Package",
            _ => groupName.TrimEnd('s')
        };
    }

    /// <summary>
    /// Adds indexed faction membership rows for compared NPC records.
    /// </summary>
    /// <param name="fields">The comparison field collection being populated.</param>
    /// <param name="records">The NPC records participating in the comparison.</param>
    private static void AddNPCFactionGroups(ICollection<RecordComparisonFieldDTO> fields, IReadOnlyList<NPCDTO> records)
    {
        var indexes = records.SelectMany(record => record.Factions).Select(faction => faction.FactionIndex).Distinct().Order().ToList();
        var children = new List<RecordComparisonFieldDTO>();
        foreach (var index in indexes)
        {
            var currentIndex = index;
            var factionChildren = new List<RecordComparisonFieldDTO>
            {
                CreateField("Faction", records, record => FormatFormKey(record.Factions.FirstOrDefault(faction => faction.FactionIndex == currentIndex)?.Faction)),
                CreateField("Rank", records, record => record.Factions.FirstOrDefault(faction => faction.FactionIndex == currentIndex)?.Rank?.ToString(CultureInfo.InvariantCulture) ?? string.Empty),
                CreateField("Fluff", records, record => record.Factions.FirstOrDefault(faction => faction.FactionIndex == currentIndex)?.Fluff ?? string.Empty)
            }
                .Where(HasVisibleValue)
                .ToList();
            if (factionChildren.Count > 0)
            {
                children.Add(CreateGroupField($"Faction [{currentIndex}]", records.Cast<RecordDTO>().ToList(), factionChildren));
            }
        }

        if (children.Count > 0)
        {
            fields.Add(CreateGroupField("Factions", records.Cast<RecordDTO>().ToList(), children));
        }
    }

    /// <summary>
    /// Adds indexed actor-value property rows for compared NPC records.
    /// </summary>
    /// <param name="fields">The comparison field collection being populated.</param>
    /// <param name="records">The NPC records participating in the comparison.</param>
    private static void AddNPCPropertyGroups(ICollection<RecordComparisonFieldDTO> fields, IReadOnlyList<NPCDTO> records)
    {
        var indexes = records.SelectMany(record => record.Properties).Select(property => property.PropertyIndex).Distinct().Order().ToList();
        var children = new List<RecordComparisonFieldDTO>();
        foreach (var index in indexes)
        {
            var currentIndex = index;
            var propertyChildren = new List<RecordComparisonFieldDTO>
            {
                CreateField("ActorValue", records, record => FormatFormKey(record.Properties.FirstOrDefault(property => property.PropertyIndex == currentIndex)?.ActorValue)),
                CreateField("Value", records, record => FormatNumericDisplayValue(record.Properties.FirstOrDefault(property => property.PropertyIndex == currentIndex)?.Value, null))
            }
                .Where(HasVisibleValue)
                .ToList();
            if (propertyChildren.Count > 0)
            {
                children.Add(CreateGroupField($"Property [{currentIndex}]", records.Cast<RecordDTO>().ToList(), propertyChildren));
            }
        }

        if (children.Count > 0)
        {
            fields.Add(CreateGroupField("Properties", records.Cast<RecordDTO>().ToList(), children));
        }
    }

    /// <summary>
    /// Adds indexed inventory item rows for compared NPC records.
    /// </summary>
    /// <param name="fields">The comparison field collection being populated.</param>
    /// <param name="records">The NPC records participating in the comparison.</param>
    private static void AddNPCItemGroups(ICollection<RecordComparisonFieldDTO> fields, IReadOnlyList<NPCDTO> records)
    {
        var indexes = records.SelectMany(record => record.Items).Select(item => item.ItemIndex).Distinct().Order().ToList();
        var children = new List<RecordComparisonFieldDTO>();
        foreach (var index in indexes)
        {
            var currentIndex = index;
            var itemChildren = new List<RecordComparisonFieldDTO>
            {
                CreateField("Item", records, record => FormatFormKey(record.Items.FirstOrDefault(item => item.ItemIndex == currentIndex)?.Item)),
                CreateField("Count", records, record => record.Items.FirstOrDefault(item => item.ItemIndex == currentIndex)?.Count?.ToString(CultureInfo.InvariantCulture) ?? string.Empty)
            }
                .Where(HasVisibleValue)
                .ToList();
            if (itemChildren.Count > 0)
            {
                children.Add(CreateGroupField($"Item [{currentIndex}]", records.Cast<RecordDTO>().ToList(), itemChildren));
            }
        }

        if (children.Count > 0)
        {
            fields.Add(CreateGroupField("Items", records.Cast<RecordDTO>().ToList(), children));
        }
    }

    /// <summary>
    /// Adds indexed perk rows for compared NPC records.
    /// </summary>
    /// <param name="fields">The comparison field collection being populated.</param>
    /// <param name="records">The NPC records participating in the comparison.</param>
    private static void AddNPCPerkGroups(ICollection<RecordComparisonFieldDTO> fields, IReadOnlyList<NPCDTO> records)
    {
        var indexes = records.SelectMany(record => record.Perks).Select(perk => perk.PerkIndex).Distinct().Order().ToList();
        var children = new List<RecordComparisonFieldDTO>();
        foreach (var index in indexes)
        {
            var currentIndex = index;
            var perkChildren = new List<RecordComparisonFieldDTO>
            {
                CreateField("Perk", records, record => FormatFormKey(record.Perks.FirstOrDefault(perk => perk.PerkIndex == currentIndex)?.Perk)),
                CreateField("Rank", records, record => record.Perks.FirstOrDefault(perk => perk.PerkIndex == currentIndex)?.Rank?.ToString(CultureInfo.InvariantCulture) ?? string.Empty),
                CreateField("Fluff", records, record => record.Perks.FirstOrDefault(perk => perk.PerkIndex == currentIndex)?.Fluff ?? string.Empty)
            }
                .Where(HasVisibleValue)
                .ToList();
            if (perkChildren.Count > 0)
            {
                children.Add(CreateGroupField($"Perk [{currentIndex}]", records.Cast<RecordDTO>().ToList(), perkChildren));
            }
        }

        if (children.Count > 0)
        {
            fields.Add(CreateGroupField("Perks", records.Cast<RecordDTO>().ToList(), children));
        }
    }

    /// <summary>
    /// Adds Fallout 4 NPC morph key/value rows.
    /// </summary>
    /// <param name="fields">The comparison field collection being populated.</param>
    /// <param name="records">The NPC records participating in the comparison.</param>
    private static void AddNPCMorphGroups(ICollection<RecordComparisonFieldDTO> fields, IReadOnlyList<NPCDTO> records)
    {
        var indexes = records.SelectMany(record => record.Morphs).Select(morph => morph.MorphIndex).Distinct().Order().ToList();
        var children = new List<RecordComparisonFieldDTO>();
        foreach (var index in indexes)
        {
            var currentIndex = index;
            var morphChildren = new List<RecordComparisonFieldDTO>
            {
                CreateField("Key", records, record => record.Morphs.FirstOrDefault(morph => morph.MorphIndex == currentIndex)?.Key?.ToString(CultureInfo.InvariantCulture) ?? string.Empty),
                CreateField("Value", records, record => FormatNumericDisplayValue(record.Morphs.FirstOrDefault(morph => morph.MorphIndex == currentIndex)?.Value, null))
            }
                .Where(HasVisibleValue)
                .ToList();
            if (morphChildren.Count > 0)
            {
                children.Add(CreateGroupField($"Morph [{currentIndex}]", records.Cast<RecordDTO>().ToList(), morphChildren));
            }
        }

        if (children.Count > 0)
        {
            fields.Add(CreateGroupField("Morphs", records.Cast<RecordDTO>().ToList(), children));
        }
    }

    /// <summary>
    /// Adds simple face morph position rows used by Fallout 4 NPC records.
    /// </summary>
    /// <param name="fields">The comparison field collection being populated.</param>
    /// <param name="records">The NPC records participating in the comparison.</param>
    private static void AddNPCFaceMorphPositionGroups(ICollection<RecordComparisonFieldDTO> fields, IReadOnlyList<NPCDTO> records)
    {
        var indexes = records.SelectMany(record => record.FaceMorphs).Select(morph => morph.FaceMorphIndex).Distinct().Order().ToList();
        var children = new List<RecordComparisonFieldDTO>();
        foreach (var index in indexes)
        {
            var currentIndex = index;
            var morphChildren = new List<RecordComparisonFieldDTO>
            {
                CreateField("Index", records, record => record.FaceMorphs.FirstOrDefault(morph => morph.FaceMorphIndex == currentIndex)?.Index?.ToString(CultureInfo.InvariantCulture) ?? string.Empty),
                CreateField("Position", records, record => record.FaceMorphs.FirstOrDefault(morph => morph.FaceMorphIndex == currentIndex)?.Position ?? string.Empty),
                CreateField("Rotation", records, record => record.FaceMorphs.FirstOrDefault(morph => morph.FaceMorphIndex == currentIndex)?.Rotation ?? string.Empty),
                CreateField("Scale", records, record => FormatNumericDisplayValue(record.FaceMorphs.FirstOrDefault(morph => morph.FaceMorphIndex == currentIndex)?.Scale, null))
            }
                .Where(HasVisibleValue)
                .ToList();
            if (morphChildren.Count > 0)
            {
                children.Add(CreateGroupField($"FaceMorph [{currentIndex}]", records.Cast<RecordDTO>().ToList(), morphChildren));
            }
        }

        if (children.Count > 0)
        {
            fields.Add(CreateGroupField("FaceMorphs", records.Cast<RecordDTO>().ToList(), children));
        }
    }

    /// <summary>
    /// Adds Starfield face dial slider position rows.
    /// </summary>
    /// <param name="fields">The comparison field collection being populated.</param>
    /// <param name="records">The NPC records participating in the comparison.</param>
    private static void AddNPCFaceDialPositionGroups(ICollection<RecordComparisonFieldDTO> fields, IReadOnlyList<NPCDTO> records)
    {
        var indexes = records.SelectMany(record => record.FaceDialPositions).Select(position => position.FaceDialPositionIndex).Distinct().Order().ToList();
        var children = new List<RecordComparisonFieldDTO>();
        foreach (var index in indexes)
        {
            var currentIndex = index;
            var positionChildren = new List<RecordComparisonFieldDTO>
            {
                CreateField("Index", records, record => record.FaceDialPositions.FirstOrDefault(position => position.FaceDialPositionIndex == currentIndex)?.Index?.ToString(CultureInfo.InvariantCulture) ?? string.Empty),
                CreateField("Position", records, record => FormatNumericDisplayValue(record.FaceDialPositions.FirstOrDefault(position => position.FaceDialPositionIndex == currentIndex)?.Position, 3))
            }
                .Where(HasVisibleValue)
                .ToList();
            if (positionChildren.Count > 0)
            {
                children.Add(CreateGroupField($"FaceDialPosition [{currentIndex}]", records.Cast<RecordDTO>().ToList(), positionChildren));
            }
        }

        if (children.Count > 0)
        {
            fields.Add(CreateGroupField("FaceDialPositions", records.Cast<RecordDTO>().ToList(), children));
        }
    }

    /// <summary>
    /// Adds Starfield nested face morph group rows, preserving the face morph index and nested morph group indices.
    /// </summary>
    /// <param name="fields">The comparison field collection being populated.</param>
    /// <param name="records">The NPC records participating in the comparison.</param>
    private static void AddNPCFaceMorphGroupSetGroups(ICollection<RecordComparisonFieldDTO> fields, IReadOnlyList<NPCDTO> records)
    {
        var indexes = records.SelectMany(record => record.FaceMorphGroups).Select(morph => morph.FaceMorphIndex).Distinct().Order().ToList();
        var children = new List<RecordComparisonFieldDTO>();
        foreach (var index in indexes)
        {
            var currentIndex = index;
            var faceMorphChildren = new List<RecordComparisonFieldDTO>
            {
                CreateField("Index", records, record => record.FaceMorphGroups.FirstOrDefault(morph => morph.FaceMorphIndex == currentIndex)?.Index?.ToString(CultureInfo.InvariantCulture) ?? string.Empty)
            }
                .Where(HasVisibleValue)
                .ToList();
            AddNPCFaceMorphNestedGroupRows(faceMorphChildren, records, currentIndex);
            if (faceMorphChildren.Count > 0)
            {
                children.Add(CreateGroupField($"FaceMorph [{currentIndex}]", records.Cast<RecordDTO>().ToList(), faceMorphChildren));
            }
        }

        if (children.Count > 0)
        {
            fields.Add(CreateGroupField("FaceMorphGroups", records.Cast<RecordDTO>().ToList(), children));
        }
    }

    /// <summary>
    /// Adds nested morph group blend rows for one Starfield face morph entry.
    /// </summary>
    /// <param name="fields">The child comparison field collection for the parent face morph entry.</param>
    /// <param name="records">The NPC records participating in the comparison.</param>
    /// <param name="faceMorphIndex">The parent face morph row index.</param>
    private static void AddNPCFaceMorphNestedGroupRows(
        ICollection<RecordComparisonFieldDTO> fields,
        IReadOnlyList<NPCDTO> records,
        int faceMorphIndex)
    {
        var groupIndexes = records
            .SelectMany(record => record.FaceMorphGroups.FirstOrDefault(morph => morph.FaceMorphIndex == faceMorphIndex)?.MorphGroups ?? [])
            .Select(group => group.MorphGroupIndex)
            .Distinct()
            .Order()
            .ToList();
        foreach (var groupIndex in groupIndexes)
        {
            var currentGroupIndex = groupIndex;
            var groupChildren = new List<RecordComparisonFieldDTO>
            {
                CreateField("MorphGroup", records, record => FindNPCFaceMorphGroup(record, faceMorphIndex, currentGroupIndex)?.MorphGroup ?? string.Empty),
                CreateField("BlendIntensity", records, record => FormatNumericDisplayValue(FindNPCFaceMorphGroup(record, faceMorphIndex, currentGroupIndex)?.BlendIntensity, 3))
            }
                .Where(HasVisibleValue)
                .ToList();
            if (groupChildren.Count > 0)
            {
                fields.Add(CreateGroupField($"MorphGroup [{currentGroupIndex}]", records.Cast<RecordDTO>().ToList(), groupChildren));
            }
        }
    }

    /// <summary>
    /// Finds one nested face morph group by parent and child index.
    /// </summary>
    /// <param name="record">The NPC record to search.</param>
    /// <param name="faceMorphIndex">The parent face morph row index.</param>
    /// <param name="morphGroupIndex">The nested morph group row index.</param>
    /// <returns>The matching nested morph group row, or <c>null</c> when absent.</returns>
    private static NPCFaceMorphGroupDTO? FindNPCFaceMorphGroup(NPCDTO record, int faceMorphIndex, int morphGroupIndex)
    {
        return record.FaceMorphGroups
            .FirstOrDefault(morph => morph.FaceMorphIndex == faceMorphIndex)
            ?.MorphGroups
            .FirstOrDefault(group => group.MorphGroupIndex == morphGroupIndex);
    }

    /// <summary>
    /// Adds Starfield morph blend rows.
    /// </summary>
    /// <param name="fields">The comparison field collection being populated.</param>
    /// <param name="records">The NPC records participating in the comparison.</param>
    private static void AddNPCMorphBlendGroups(ICollection<RecordComparisonFieldDTO> fields, IReadOnlyList<NPCDTO> records)
    {
        var indexes = records.SelectMany(record => record.MorphBlends).Select(blend => blend.MorphBlendIndex).Distinct().Order().ToList();
        var children = new List<RecordComparisonFieldDTO>();
        foreach (var index in indexes)
        {
            var currentIndex = index;
            var blendChildren = new List<RecordComparisonFieldDTO>
            {
                CreateField("BlendName", records, record => record.MorphBlends.FirstOrDefault(blend => blend.MorphBlendIndex == currentIndex)?.BlendName ?? string.Empty),
                CreateField("Intensity", records, record => FormatNumericDisplayValue(record.MorphBlends.FirstOrDefault(blend => blend.MorphBlendIndex == currentIndex)?.Intensity, 3))
            }
                .Where(HasVisibleValue)
                .ToList();
            if (blendChildren.Count > 0)
            {
                children.Add(CreateGroupField($"MorphBlend [{currentIndex}]", records.Cast<RecordDTO>().ToList(), blendChildren));
            }
        }

        if (children.Count > 0)
        {
            fields.Add(CreateGroupField("MorphBlends", records.Cast<RecordDTO>().ToList(), children));
        }
    }

    /// <summary>
    /// Adds Starfield AVMD tint rows.
    /// </summary>
    /// <param name="fields">The comparison field collection being populated.</param>
    /// <param name="records">The NPC records participating in the comparison.</param>
    private static void AddNPCTintGroups(ICollection<RecordComparisonFieldDTO> fields, IReadOnlyList<NPCDTO> records)
    {
        var indexes = records.SelectMany(record => record.Tints).Select(tint => tint.TintIndex).Distinct().Order().ToList();
        var children = new List<RecordComparisonFieldDTO>();
        foreach (var index in indexes)
        {
            var currentIndex = index;
            var tintChildren = new List<RecordComparisonFieldDTO>
            {
                CreateField("TintType", records, record => record.Tints.FirstOrDefault(tint => tint.TintIndex == currentIndex)?.TintType ?? string.Empty),
                CreateField("TintGroup", records, record => record.Tints.FirstOrDefault(tint => tint.TintIndex == currentIndex)?.TintGroup ?? string.Empty),
                CreateField("TintName", records, record => record.Tints.FirstOrDefault(tint => tint.TintIndex == currentIndex)?.TintName ?? string.Empty),
                CreateField("TintTexture", records, record => record.Tints.FirstOrDefault(tint => tint.TintIndex == currentIndex)?.TintTexture ?? string.Empty),
                CreateField("TintColor", records, record => record.Tints.FirstOrDefault(tint => tint.TintIndex == currentIndex)?.TintColor ?? string.Empty),
                CreateField("TintIntensity", records, record => FormatNumericDisplayValue(record.Tints.FirstOrDefault(tint => tint.TintIndex == currentIndex)?.TintIntensity, 3))
            }
                .Where(HasVisibleValue)
                .ToList();
            if (tintChildren.Count > 0)
            {
                children.Add(CreateGroupField($"Tint [{currentIndex}]", records.Cast<RecordDTO>().ToList(), tintChildren));
            }
        }

        if (children.Count > 0)
        {
            fields.Add(CreateGroupField("Tints", records.Cast<RecordDTO>().ToList(), children));
        }
    }

    /// <summary>
    /// Adds Skyrim tint layer rows.
    /// </summary>
    /// <param name="fields">The comparison field collection being populated.</param>
    /// <param name="records">The NPC records participating in the comparison.</param>
    private static void AddNPCTintLayerGroups(ICollection<RecordComparisonFieldDTO> fields, IReadOnlyList<NPCDTO> records)
    {
        var indexes = records.SelectMany(record => record.TintLayers).Select(layer => layer.TintLayerIndex).Distinct().Order().ToList();
        var children = new List<RecordComparisonFieldDTO>();
        foreach (var index in indexes)
        {
            var currentIndex = index;
            var layerChildren = new List<RecordComparisonFieldDTO>
            {
                CreateField("Index", records, record => record.TintLayers.FirstOrDefault(layer => layer.TintLayerIndex == currentIndex)?.Index?.ToString(CultureInfo.InvariantCulture) ?? string.Empty),
                CreateField("Color", records, record => record.TintLayers.FirstOrDefault(layer => layer.TintLayerIndex == currentIndex)?.Color ?? string.Empty),
                CreateField("InterpolationValue", records, record => FormatNumericDisplayValue(record.TintLayers.FirstOrDefault(layer => layer.TintLayerIndex == currentIndex)?.InterpolationValue, null)),
                CreateField("Preset", records, record => record.TintLayers.FirstOrDefault(layer => layer.TintLayerIndex == currentIndex)?.Preset?.ToString(CultureInfo.InvariantCulture) ?? string.Empty)
            }
                .Where(HasVisibleValue)
                .ToList();
            if (layerChildren.Count > 0)
            {
                children.Add(CreateGroupField($"TintLayer [{currentIndex}]", records.Cast<RecordDTO>().ToList(), layerChildren));
            }
        }

        if (children.Count > 0)
        {
            fields.Add(CreateGroupField("TintLayers", records.Cast<RecordDTO>().ToList(), children));
        }
    }

    /// <summary>
    /// Adds Fallout 4 face tinting layer rows and their state flag lists.
    /// </summary>
    /// <param name="fields">The comparison field collection being populated.</param>
    /// <param name="records">The NPC records participating in the comparison.</param>
    private static void AddNPCFaceTintingLayerGroups(ICollection<RecordComparisonFieldDTO> fields, IReadOnlyList<NPCDTO> records)
    {
        var indexes = records.SelectMany(record => record.FaceTintingLayers).Select(layer => layer.FaceTintingLayerIndex).Distinct().Order().ToList();
        var children = new List<RecordComparisonFieldDTO>();
        foreach (var index in indexes)
        {
            var currentIndex = index;
            var layerChildren = new List<RecordComparisonFieldDTO>
            {
                CreateField("DataType", records, record => record.FaceTintingLayers.FirstOrDefault(layer => layer.FaceTintingLayerIndex == currentIndex)?.DataType ?? string.Empty),
                CreateField("Index", records, record => record.FaceTintingLayers.FirstOrDefault(layer => layer.FaceTintingLayerIndex == currentIndex)?.Index?.ToString(CultureInfo.InvariantCulture) ?? string.Empty),
                CreateField("Value", records, record => FormatNumericDisplayValue(record.FaceTintingLayers.FirstOrDefault(layer => layer.FaceTintingLayerIndex == currentIndex)?.Value, null)),
                CreateField("Color", records, record => record.FaceTintingLayers.FirstOrDefault(layer => layer.FaceTintingLayerIndex == currentIndex)?.Color ?? string.Empty),
                CreateField("TemplateColorIndex", records, record => record.FaceTintingLayers.FirstOrDefault(layer => layer.FaceTintingLayerIndex == currentIndex)?.TemplateColorIndex?.ToString(CultureInfo.InvariantCulture) ?? string.Empty),
                CreateField("TENDDataTypeState", records, record => string.Join(", ", record.FaceTintingLayers.FirstOrDefault(layer => layer.FaceTintingLayerIndex == currentIndex)?.TENDDataTypeState ?? []))
            }
                .Where(HasVisibleValue)
                .ToList();
            if (layerChildren.Count > 0)
            {
                children.Add(CreateGroupField($"FaceTintingLayer [{currentIndex}]", records.Cast<RecordDTO>().ToList(), layerChildren));
            }
        }

        if (children.Count > 0)
        {
            fields.Add(CreateGroupField("FaceTintingLayers", records.Cast<RecordDTO>().ToList(), children));
        }
    }

    /// <summary>
    /// Adds Skyrim NPC player-skill values and offsets when present.
    /// </summary>
    /// <param name="fields">The comparison field collection being populated.</param>
    /// <param name="records">The NPC records participating in the comparison.</param>
    private static void AddNPCPlayerSkillsGroup(ICollection<RecordComparisonFieldDTO> fields, IReadOnlyList<NPCDTO> records)
    {
        var children = new List<RecordComparisonFieldDTO>
        {
            CreateField("Health", records, record => record.PlayerSkills?.Health?.ToString(CultureInfo.InvariantCulture) ?? string.Empty),
            CreateField("Magicka", records, record => record.PlayerSkills?.Magicka?.ToString(CultureInfo.InvariantCulture) ?? string.Empty),
            CreateField("Stamina", records, record => record.PlayerSkills?.Stamina?.ToString(CultureInfo.InvariantCulture) ?? string.Empty),
            CreateField("GearedUpWeapons", records, record => record.PlayerSkills?.GearedUpWeapons?.ToString(CultureInfo.InvariantCulture) ?? string.Empty)
        }
            .Where(HasVisibleValue)
            .ToList();
        AddNPCPlayerSkillValueRows(children, records, "SkillValues", record => record.PlayerSkills?.SkillValues);
        AddNPCPlayerSkillValueRows(children, records, "SkillOffsets", record => record.PlayerSkills?.SkillOffsets);

        if (children.Count > 0)
        {
            fields.Add(CreateGroupField("PlayerSkills", records.Cast<RecordDTO>().ToList(), children));
        }
    }

    /// <summary>
    /// Adds indexed key/value skill rows for one player-skill collection.
    /// </summary>
    /// <param name="fields">The child field collection for the player-skills group.</param>
    /// <param name="records">The NPC records participating in the comparison.</param>
    /// <param name="groupName">The comparison group name for the skill collection.</param>
    /// <param name="collectionFactory">The accessor that returns the skill collection for one NPC.</param>
    private static void AddNPCPlayerSkillValueRows(
        ICollection<RecordComparisonFieldDTO> fields,
        IReadOnlyList<NPCDTO> records,
        string groupName,
        Func<NPCDTO, IList<NPCPlayerSkillValueDTO>?> collectionFactory)
    {
        var indexes = records
            .SelectMany(record => collectionFactory(record) ?? [])
            .Select(skill => skill.SkillIndex)
            .Distinct()
            .Order()
            .ToList();
        var children = new List<RecordComparisonFieldDTO>();
        foreach (var index in indexes)
        {
            var currentIndex = index;
            var skillChildren = new List<RecordComparisonFieldDTO>
            {
                CreateField("Key", records, record => collectionFactory(record)?.FirstOrDefault(skill => skill.SkillIndex == currentIndex)?.Key ?? string.Empty),
                CreateField("Value", records, record => collectionFactory(record)?.FirstOrDefault(skill => skill.SkillIndex == currentIndex)?.Value?.ToString(CultureInfo.InvariantCulture) ?? string.Empty)
            }
                .Where(HasVisibleValue)
                .ToList();
            if (skillChildren.Count > 0)
            {
                children.Add(CreateGroupField($"Skill [{currentIndex}]", records.Cast<RecordDTO>().ToList(), skillChildren));
            }
        }

        if (children.Count > 0)
        {
            fields.Add(CreateGroupField(groupName, records.Cast<RecordDTO>().ToList(), children));
        }
    }

    private static void AddScriptFragmentGroups(
        IList<RecordComparisonFieldDTO> fields,
        IReadOnlyList<RecordDTO> records,
        IReadOnlyList<ScriptFragmentDTO> fragments)
    {
        var fragmentKeys = fragments
            .Select(fragment => new ScriptFragmentKey(fragment.FragmentSlot, fragment.FragmentIndex))
            .Distinct()
            .OrderBy(key => key.Slot, StringComparer.Ordinal)
            .ThenBy(key => key.Index)
            .ToList();
        if (fragmentKeys.Count == 0)
        {
            return;
        }

        var fragmentFields = new List<RecordComparisonFieldDTO>();
        foreach (var fragmentKey in fragmentKeys)
        {
            var children = new List<RecordComparisonFieldDTO>
            {
                CreateChildField("Type", records, record => FindScriptFragment(fragments, record.ModKey, fragmentKey)?.MutagenObjectType ?? string.Empty),
                CreateChildField("ScriptName", records, record => FindScriptFragment(fragments, record.ModKey, fragmentKey)?.ScriptName ?? string.Empty),
                CreateChildField("FragmentName", records, record => FindScriptFragment(fragments, record.ModKey, fragmentKey)?.FragmentName ?? string.Empty),
                CreateChildField("ExtraBindDataVersion", records, record => FindScriptFragment(fragments, record.ModKey, fragmentKey)?.ExtraBindDataVersion?.ToString() ?? string.Empty)
            }
                .Where(HasVisibleValue)
                .ToList();
            if (children.Count > 0)
            {
                fragmentFields.Add(CreateGroupField(GetScriptFragmentGroupName(fragmentKey), records, children));
            }
        }

        if (fragmentFields.Count > 0)
        {
            fields.Add(CreateGroupField("Script Fragments", records, fragmentFields));
        }
    }

    private static void AddScriptingAdapterGroups(
        IList<RecordComparisonFieldDTO> fields,
        IReadOnlyList<RecordDTO> records,
        IReadOnlyList<ScriptingAdapterDTO> scriptingAdapters)
    {
        var scriptFields = new List<RecordComparisonFieldDTO>();
        foreach (var scriptIndex in scriptingAdapters.Select(adapter => adapter.ScriptIndex).Distinct().Order())
        {
            var currentScriptIndex = scriptIndex;
            var scriptChildren = new List<RecordComparisonFieldDTO>
            {
                CreateChildField("Name", records, record => FindScriptingAdapter(scriptingAdapters, record.ModKey, currentScriptIndex)?.Name ?? string.Empty)
            };

            var propertyIndexes = scriptingAdapters
                .Where(adapter => adapter.ScriptIndex == currentScriptIndex)
                .SelectMany(adapter => adapter.Properties)
                .Select(property => property.PropertyIndex)
                .Distinct()
                .Order()
                .ToList();
            foreach (var propertyIndex in propertyIndexes)
            {
                var currentPropertyIndex = propertyIndex;
                var propertyChildren = new List<RecordComparisonFieldDTO>
                {
                    CreateChildField("Name", records, record => FindScriptingProperty(scriptingAdapters, record.ModKey, currentScriptIndex, currentPropertyIndex)?.Name ?? string.Empty),
                    CreateChildField("Type", records, record => FindScriptingProperty(scriptingAdapters, record.ModKey, currentScriptIndex, currentPropertyIndex)?.MutagenObjectType ?? string.Empty),
                    CreateChildField("Value", records, record => FormatScriptingPropertyValue(FindScriptingProperty(scriptingAdapters, record.ModKey, currentScriptIndex, currentPropertyIndex)))
                }
                    .Where(HasVisibleValue)
                    .ToList();
                if (propertyChildren.Count > 0)
                {
                    scriptChildren.Add(CreateGroupField($"Property [{propertyIndex}]", records, propertyChildren));
                }
            }

            var visibleScriptChildren = scriptChildren
                .Where(field => field.Children.Count > 0 || HasVisibleValue(field))
                .ToList();
            if (visibleScriptChildren.Count > 0)
            {
                scriptFields.Add(CreateGroupField($"Script [{scriptIndex}]", records, visibleScriptChildren));
            }
        }

        if (scriptFields.Count > 0)
        {
            fields.Add(CreateGroupField("Scripts", records, scriptFields));
        }
    }

    private static RecordComparisonFieldDTO CreateChildField(
        string fieldName,
        IReadOnlyList<RecordDTO> records,
        Func<RecordDTO, string> valueFactory)
    {
        return CreateField(fieldName, records, valueFactory);
    }

    private static RecordComparisonFieldDTO CreateRawPayloadValueField(
        IReadOnlyList<RecordDTO> records,
        IReadOnlyList<RawRecordPayloadDTO> payloads,
        RawPayloadKey payloadKey)
    {
        var values = records
            .Select(record =>
            {
                var payloadValue = FindRawPayload(payloads, record.ModKey, payloadKey)?.PayloadValue ?? string.Empty;
                return new RecordComparisonValueDTO
                {
                    ModKey = record.ModKey,
                    DisplayValue = string.IsNullOrWhiteSpace(payloadValue)
                        ? string.Empty
                        : UnparseableReflectionDataLabel,
                    DetailValue = payloadValue,
                    DisplayKind = string.IsNullOrWhiteSpace(payloadValue)
                        ? RecordComparisonValueDisplayKind.Text
                        : RecordComparisonValueDisplayKind.RawBinaryPayload
                };
            })
            .ToList();
        var state = GetComparisonValueState(values.Select(value => value.DetailValue).ToList(), isComparable: true);
        for (var valueIndex = 0; valueIndex < values.Count; valueIndex++)
        {
            values[valueIndex].State = state == RecordComparisonValueState.Conflict && valueIndex == values.Count - 1
                ? RecordComparisonValueState.WinningOverride
                : state;
        }

        return new RecordComparisonFieldDTO
        {
            FieldName = "Value",
            IsComparable = true,
            State = state,
            Values = values
        };
    }

    private static RecordComparisonFieldDTO CreateGroupField(
        string fieldName,
        IReadOnlyList<RecordDTO> records,
        IReadOnlyList<RecordComparisonFieldDTO> children)
    {
        return new RecordComparisonFieldDTO
        {
            FieldName = fieldName,
            Values = records.Select(record => new RecordComparisonValueDTO
            {
                ModKey = record.ModKey,
                DisplayValue = string.Empty,
                State = RecordComparisonValueState.Neutral
            }).ToList(),
            State = RecordComparisonValueState.Neutral,
            IsComparable = false,
            Children = children
        };
    }

    /// <summary>
    /// Creates the comparison value row for a reflection payload while preserving the full value as detail data.
    /// </summary>
    /// <param name="records">The ordered parent records participating in the comparison.</param>
    /// <param name="reflections">The reflection rows read back for the compared form key.</param>
    /// <param name="componentIndex">The component index whose <c>REFL</c> value should be compared.</param>
    /// <returns>The comparison field that displays the summarized reflection value and keeps full detail data.</returns>
    private static RecordComparisonFieldDTO CreateReflectionValueField(
        IReadOnlyList<RecordDTO> records,
        IReadOnlyList<ReflectionDTO> reflections,
        int componentIndex)
    {
        var values = records
            .Select(record =>
            {
                var reflectionValue = FindReflection(reflections, record.ModKey, componentIndex)?.REFL ?? string.Empty;
                return new RecordComparisonValueDTO
                {
                    ModKey = record.ModKey,
                    DisplayValue = string.IsNullOrWhiteSpace(reflectionValue)
                        ? string.Empty
                        : UnparseableReflectionDataLabel,
                    DetailValue = reflectionValue,
                    DisplayKind = string.IsNullOrWhiteSpace(reflectionValue)
                        ? RecordComparisonValueDisplayKind.Text
                        : RecordComparisonValueDisplayKind.RawBinaryPayload
                };
            })
            .ToList();
        var state = GetComparisonValueState(values.Select(value => value.DetailValue).ToList(), isComparable: true);
        for (var valueIndex = 0; valueIndex < values.Count; valueIndex++)
        {
            values[valueIndex].State = state == RecordComparisonValueState.Conflict && valueIndex == values.Count - 1
                ? RecordComparisonValueState.WinningOverride
                : state;
        }

        return new RecordComparisonFieldDTO
        {
            FieldName = "REFL",
            IsComparable = true,
            State = state,
            Values = values
        };
    }

    private static bool HasVisibleValue(RecordComparisonFieldDTO field)
    {
        return field.Values.Any(value => !string.IsNullOrWhiteSpace(value.DisplayValue));
    }

    private static RecordComparisonValueState GetComparisonValueState(IReadOnlyList<string> values, bool isComparable)
    {
        if (!isComparable || values.Count <= 1)
        {
            return RecordComparisonValueState.Neutral;
        }

        return values.Distinct(StringComparer.Ordinal).Count() == 1
            ? RecordComparisonValueState.Identical
            : RecordComparisonValueState.Conflict;
    }

    private static string FormatFormKey(FormKeyDTO? formKey)
    {
        return formKey is null
            ? string.Empty
            : $"{formKey.ModKey.FileName}:{formKey.Id:X8}";
    }

    private static string FormatHexIntegerString(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        return value.StartsWith("0x", StringComparison.OrdinalIgnoreCase) &&
               int.TryParse(value.AsSpan(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var number)
            ? number.ToString(CultureInfo.InvariantCulture)
            : value;
    }

    private static ModelDTO? FindModel(IReadOnlyList<ModelDTO> models, ModKeyDTO modKey, ModelKey modelKey)
    {
        return models.FirstOrDefault(model => IsSameModKey(model.ModKey, modKey) && IsModelKey(model, modelKey));
    }

    private static KeywordMappingDTO? FindKeyword(IReadOnlyList<KeywordMappingDTO> keywords, ModKeyDTO modKey, int keywordIndex)
    {
        return keywords.FirstOrDefault(keyword => IsSameModKey(keyword.ModKey, modKey) && keyword.KeywordIndex == keywordIndex);
    }

    private static SoundMappingDTO? FindSound(IReadOnlyList<SoundMappingDTO> sounds, ModKeyDTO modKey, SoundKey soundKey)
    {
        return sounds.FirstOrDefault(sound => IsSameModKey(sound.ModKey, modKey) &&
            string.Equals(sound.SoundSlot, soundKey.Slot, StringComparison.Ordinal) &&
            sound.SoundIndex == soundKey.Index);
    }

    private static ClassWeightDTO? FindClassWeight(ClassDTO record, string weightType, int weightIndex)
    {
        var weights = string.Equals(weightType, "Skill", StringComparison.Ordinal) ? record.SkillWeights : record.StatWeights;
        return weights.FirstOrDefault(weight => weight.WeightIndex == weightIndex);
    }

    private static RecordComponentDTO? FindRecordComponent(IReadOnlyList<RecordComponentDTO> components, ModKeyDTO modKey, int componentIndex)
    {
        return components.FirstOrDefault(component => IsSameModKey(component.ModKey, modKey) && component.ComponentIndex == componentIndex);
    }

    private static RecordComponentItemDTO? FindRecordComponentItem(IReadOnlyList<RecordComponentDTO> components, ModKeyDTO modKey, int componentIndex, int itemIndex)
    {
        return FindRecordComponent(components, modKey, componentIndex)?.Items.FirstOrDefault(item => item.ItemIndex == itemIndex);
    }

    private static RawRecordPayloadDTO? FindRawPayload(IReadOnlyList<RawRecordPayloadDTO> payloads, ModKeyDTO modKey, RawPayloadKey payloadKey)
    {
        return payloads.FirstOrDefault(payload => IsSameModKey(payload.ModKey, modKey) &&
            string.Equals(payload.PayloadSlot, payloadKey.Slot, StringComparison.Ordinal) &&
            payload.PayloadIndex == payloadKey.Index);
    }

    /// <summary>
    /// Finds the reflection row for the specified plugin and component index.
    /// </summary>
    /// <param name="reflections">The available reflection rows for the compared form key.</param>
    /// <param name="modKey">The plugin key that contributed the parent record instance.</param>
    /// <param name="componentIndex">The component index containing the <c>REFL</c> field.</param>
    /// <returns>The matching reflection row, or <c>null</c> when the plugin does not define that row.</returns>
    private static ReflectionDTO? FindReflection(IReadOnlyList<ReflectionDTO> reflections, ModKeyDTO modKey, int componentIndex)
    {
        return reflections.FirstOrDefault(reflection => IsSameModKey(reflection.ModKey, modKey) &&
            reflection.ComponentIndex == componentIndex);
    }

    private static ScriptFragmentDTO? FindScriptFragment(IReadOnlyList<ScriptFragmentDTO> fragments, ModKeyDTO modKey, ScriptFragmentKey fragmentKey)
    {
        return fragments.FirstOrDefault(fragment => IsSameModKey(fragment.ModKey, modKey) &&
            string.Equals(fragment.FragmentSlot, fragmentKey.Slot, StringComparison.Ordinal) &&
            fragment.FragmentIndex == fragmentKey.Index);
    }

    private static ConditionFormConditionDTO? FindConditionRule(IHasConditionsDTO record, ConditionRuleKey conditionKey)
    {
        return record.Conditions.FirstOrDefault(condition => string.Equals(condition.ConditionSlot, conditionKey.Slot, StringComparison.Ordinal) &&
            condition.ConditionIndex == conditionKey.Index);
    }

    private static ConditionFormConditionDTO? FindConditionRule(
        IReadOnlyList<IHasConditionsDTO> records,
        IReadOnlyList<RecordDTO> baseRecords,
        ModKeyDTO modKey,
        ConditionRuleKey conditionKey)
    {
        var recordIndex = FindRecordIndex(baseRecords, modKey);
        return recordIndex < 0 ? null : FindConditionRule(records[recordIndex], conditionKey);
    }

    private static string FormatConditionRuleSummary(ConditionFormConditionDTO? condition)
    {
        if (condition is null)
        {
            return string.Empty;
        }

        var runOnType = FormatConditionParameterValue(FindConditionRuleParameter(condition, "RunOnType"));
        var firstParameter = FormatConditionParameterValue(FindConditionRuleParameter(condition, "FirstParameter"));
        var secondParameter = FormatConditionParameterValue(FindConditionRuleParameter(condition, "SecondParameter"));
        var functionName = FormatFriendlyTypeName(condition.DataMutagenObjectType, splitWords: false);
        var comparisonValue = FormatFormKey(condition.ComparisonValueFormKey);
        if (string.IsNullOrWhiteSpace(comparisonValue))
        {
            comparisonValue = condition.ComparisonValue ?? string.Empty;
        }

        var arguments = new List<string>();
        if (!string.IsNullOrWhiteSpace(firstParameter))
        {
            arguments.Add(firstParameter);
        }

        if (!string.IsNullOrWhiteSpace(secondParameter))
        {
            arguments.Add(secondParameter);
        }

        var invocation = string.IsNullOrWhiteSpace(functionName)
            ? string.Join(", ", arguments)
            : $"{functionName}({string.Join(", ", arguments)})";
        var comparisonParts = string.IsNullOrWhiteSpace(comparisonValue)
            ? new[] { invocation }
            : new[] { invocation, FormatFriendlyTypeName(condition.CompareOperator, splitWords: false), comparisonValue };
        var comparison = string.Join(" ", comparisonParts.Where(value => !string.IsNullOrWhiteSpace(value)));
        return string.IsNullOrWhiteSpace(runOnType) ? comparison : $"{runOnType}: {comparison}";
    }

    private static ConditionFormConditionParameterDTO? FindConditionRuleParameter(ConditionFormConditionDTO condition, string parameterName)
    {
        return condition.Parameters.FirstOrDefault(parameter => string.Equals(parameter.ParameterName, parameterName, StringComparison.Ordinal));
    }

    private static string FormatConditionParameterValue(ConditionFormConditionParameterDTO? parameter)
    {
        if (parameter is null)
        {
            return string.Empty;
        }

        var formKey = FormatFormKey(parameter.ParameterFormKey);
        return string.IsNullOrWhiteSpace(formKey)
            ? FormatFriendlyTypeName(parameter.ParameterValue)
            : formKey;
    }

    private static string FormatFriendlyTypeName(string? value, bool splitWords = true)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var name = value;
        var genericArgumentStart = name.IndexOf('[');
        var genericArgumentEnd = name.LastIndexOf(']');
        if (genericArgumentStart >= 0 && genericArgumentEnd > genericArgumentStart)
        {
            name = name[(genericArgumentStart + 1)..genericArgumentEnd];
        }

        var genericStart = name.IndexOf('`');
        if (genericStart >= 0)
        {
            name = name[..genericStart];
        }

        var lastDot = name.LastIndexOf('.');
        if (lastDot >= 0)
        {
            name = name[(lastDot + 1)..];
        }

        if (name.StartsWith("I", StringComparison.Ordinal) && name.Length > 1 && char.IsUpper(name[1]))
        {
            name = name[1..];
        }

        foreach (var suffix in new[] { "ConditionData", "Getter", "Registration" })
        {
            if (name.EndsWith(suffix, StringComparison.Ordinal))
            {
                name = name[..^suffix.Length];
            }
        }

        name = name.Replace("_", " ");
        return splitWords ? SplitPascalCase(name) : name;
    }

    private static string SplitPascalCase(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var characters = new List<char>();
        for (var index = 0; index < value.Length; index++)
        {
            var current = value[index];
            if (index > 0 && current != ' ' && char.IsUpper(current) && !char.IsWhiteSpace(value[index - 1]) &&
                (char.IsLower(value[index - 1]) || index + 1 < value.Length && char.IsLower(value[index + 1])))
            {
                characters.Add(' ');
            }

            characters.Add(current);
        }

        return new string(characters.ToArray());
    }

    private static int FindRecordIndex(IReadOnlyList<RecordDTO> records, ModKeyDTO modKey)
    {
        for (var recordIndex = 0; recordIndex < records.Count; recordIndex++)
        {
            if (IsSameModKey(records[recordIndex].ModKey, modKey))
            {
                return recordIndex;
            }
        }

        return -1;
    }

    private static PerkRankDTO? FindPerkRank(PerkDTO record, int rankIndex)
    {
        return record.Ranks.FirstOrDefault(rank => rank.RankIndex == rankIndex);
    }

    private static PerkEffectDTO? FindPerkEffect(PerkDTO record, int effectIndex)
    {
        return record.Effects.FirstOrDefault(effect => effect.EffectIndex == effectIndex);
    }

    private static PerkRankEffectDTO? FindPerkRankEffect(PerkDTO record, int rankIndex, int effectIndex)
    {
        return FindPerkRank(record, rankIndex)?.Effects.FirstOrDefault(effect => effect.EffectIndex == effectIndex);
    }

    private static PerkRankActivityDTO? FindPerkRankActivity(PerkDTO record, int rankIndex, int activityIndex)
    {
        return FindPerkRank(record, rankIndex)?.Activities.FirstOrDefault(activity => activity.ActivityIndex == activityIndex);
    }

    private static PerkRankActivityProgressionEvaluatorDTO? FindPerkRankActivityEvaluator(PerkDTO record, int rankIndex, int activityIndex, int evaluatorIndex)
    {
        return FindPerkRankActivity(record, rankIndex, activityIndex)?.ProgressionEvalutor.FirstOrDefault(evaluator => evaluator.EvaluatorIndex == evaluatorIndex);
    }

    private static PerkEffectConditionTabDTO? FindConditionTab(IList<PerkEffectConditionTabDTO>? conditionTabs, int conditionTabIndex)
    {
        return conditionTabs?.FirstOrDefault(conditionTab => conditionTab.ConditionTabIndex == conditionTabIndex);
    }

    private static string GetPerkEffectValue(object? effect, Func<PerkEffectComparisonValue, string> getValue)
    {
        var comparisonValue = ToPerkEffectComparisonValue(effect);
        return comparisonValue == null ? string.Empty : getValue(comparisonValue);
    }

    private static TranslatedStringDTO? GetPerkEffectTranslatedValue(object? effect)
    {
        return ToPerkEffectComparisonValue(effect)?.ButtonLabel;
    }

    private static PerkEffectComparisonValue? ToPerkEffectComparisonValue(object? effect)
    {
        return effect switch
        {
            PerkEffectDTO rootEffect => new PerkEffectComparisonValue
            {
                MutagenObjectType = rootEffect.MutagenObjectType,
                Rank = rootEffect.Rank,
                Priority = rootEffect.Priority,
                PerkEntryId = rootEffect.PerkEntryId,
                Flags = rootEffect.Flags,
                ButtonLabel = rootEffect.ButtonLabel,
                ConditionCount = rootEffect.ConditionCount,
                EntryPoint = rootEffect.EntryPoint,
                PerkConditionTabCount = rootEffect.PerkConditionTabCount,
                Modification = rootEffect.Modification,
                Value = rootEffect.Value,
                ActorValue = rootEffect.ActorValue,
                Spell = rootEffect.Spell,
                Quest = rootEffect.Quest,
                Stage = rootEffect.Stage
            },
            PerkRankEffectDTO rankEffect => new PerkEffectComparisonValue
            {
                MutagenObjectType = rankEffect.MutagenObjectType,
                Rank = rankEffect.Rank,
                Priority = rankEffect.Priority,
                PerkEntryId = rankEffect.PerkEntryId,
                Flags = rankEffect.Flags,
                ButtonLabel = rankEffect.ButtonLabel,
                ConditionCount = rankEffect.ConditionCount,
                EntryPoint = rankEffect.EntryPoint,
                PerkConditionTabCount = rankEffect.PerkConditionTabCount,
                Modification = rankEffect.Modification,
                Value = rankEffect.Value,
                ActorValue = rankEffect.ActorValue,
                Spell = rankEffect.Spell,
                Quest = rankEffect.Quest,
                Stage = rankEffect.Stage
            },
            _ => null
        };
    }

    private static PerkBackgroundSkillDTO? FindPerkBackgroundSkill(PerkDTO record, int skillIndex)
    {
        return record.BackgroundSkills.FirstOrDefault(skill => skill.SkillIndex == skillIndex);
    }

    private static ScriptingAdapterDTO? FindScriptingAdapter(IReadOnlyList<ScriptingAdapterDTO> scriptingAdapters, ModKeyDTO modKey, int scriptIndex)
    {
        return scriptingAdapters.FirstOrDefault(adapter => IsSameModKey(adapter.ModKey, modKey) && adapter.ScriptIndex == scriptIndex);
    }

    private static ScriptingAdapterPropertyDTO? FindScriptingProperty(
        IReadOnlyList<ScriptingAdapterDTO> scriptingAdapters,
        ModKeyDTO modKey,
        int scriptIndex,
        int propertyIndex)
    {
        return FindScriptingAdapter(scriptingAdapters, modKey, scriptIndex)?.Properties.FirstOrDefault(property => property.PropertyIndex == propertyIndex);
    }

    /// <summary>
    /// Formats ordered integer values for compact comparison display.
    /// </summary>
    private static string FormatIntegerList(IEnumerable<int>? values)
    {
        return values == null
            ? string.Empty
            : string.Join(", ", values.Select(value => value.ToString(System.Globalization.CultureInfo.InvariantCulture)));
    }

    private static string FormatScriptingPropertyValue(ScriptingAdapterPropertyDTO? property)
    {
        if (property is null)
        {
            return string.Empty;
        }

        if (property.ListItems.Count > 0)
        {
            return string.Join(", ", property.ListItems.OrderBy(item => item.ListItemIndex).Select(FormatScriptingPropertyListItemValue));
        }

        return FormatScriptingValue(
            property.DataBool,
            property.DataInt,
            property.DataFloat,
            property.DataString,
            property.ObjectFormKey,
            property.ObjectAlias,
            property.ObjectUnused);
    }

    private static string FormatScriptingPropertyListItemValue(ScriptingAdapterPropertyListItemDTO listItem)
    {
        return FormatScriptingValue(
            listItem.DataBool,
            listItem.DataInt,
            listItem.DataFloat,
            listItem.DataString,
            listItem.ObjectFormKey,
            listItem.ObjectAlias,
            listItem.ObjectUnused);
    }

    private static string FormatScriptingValue(
        bool? dataBool,
        int? dataInt,
        double? dataFloat,
        string? dataString,
        FormKeyDTO? objectFormKey,
        short? objectAlias,
        ushort? objectUnused)
    {
        if (dataBool.HasValue)
        {
            return dataBool.Value.ToString();
        }

        if (dataInt.HasValue)
        {
            return dataInt.Value.ToString();
        }

        if (dataFloat.HasValue)
        {
            return dataFloat.Value.ToString();
        }

        if (!string.IsNullOrEmpty(dataString))
        {
            return dataString;
        }

        if (objectFormKey is not null)
        {
            return FormatFormKey(objectFormKey);
        }

        if (objectAlias.HasValue)
        {
            return objectAlias.Value.ToString();
        }

        return objectUnused?.ToString() ?? string.Empty;
    }

    private static string GetLocalizedDisplayValue(
        IReadOnlyList<LocalizedStringDTO> localizedStrings,
        RecordDTO record,
        string sourceField,
        Language recordTextLanguage,
        string? fallback)
    {
        return localizedStrings.FirstOrDefault(localizedString =>
                   IsSameModKey(localizedString.ModKey, record.ModKey) &&
                   string.Equals(localizedString.SourceField, sourceField, StringComparison.Ordinal) &&
                   string.Equals(localizedString.Language, recordTextLanguage.ToString(), StringComparison.OrdinalIgnoreCase))?.Value ??
               localizedStrings.FirstOrDefault(localizedString =>
                   IsSameModKey(localizedString.ModKey, record.ModKey) &&
                   string.Equals(localizedString.SourceField, sourceField, StringComparison.Ordinal) &&
                   string.Equals(localizedString.Language, "English", StringComparison.OrdinalIgnoreCase))?.Value ??
               fallback ??
               string.Empty;
    }

    private static string GetTranslatedDisplayValue(
        IReadOnlyList<LocalizedStringDTO> localizedStrings,
        RecordDTO record,
        string sourceField,
        Language recordTextLanguage,
        TranslatedStringDTO? fallback)
    {
        return GetLocalizedDisplayValue(localizedStrings, record, sourceField, recordTextLanguage, LocalizedStringDTOMapper.GetLocalizedText(fallback, recordTextLanguage));
    }

    private static string GetGameSettingDisplayValue(
        IReadOnlyList<LocalizedStringDTO> localizedStrings,
        GameSettingDTO record,
        Language recordTextLanguage)
    {
        return record.DataType == GameSettingDataType.String
            ? GetTranslatedDisplayValue(localizedStrings, record, "Data", recordTextLanguage, record.Data.String)
            : record.Data.GetScalarDisplayValue(record.DataType) ?? string.Empty;
    }

    private static string GetTranslatedDisplayValue(TranslatedStringDTO? translatedString)
    {
        return LocalizedStringDTOMapper.GetEnglishText(translatedString) ?? string.Empty;
    }

    private static bool IsModelKey(ModelDTO model, ModelKey modelKey)
    {
        return string.Equals(model.ModelSlot, modelKey.Slot, StringComparison.Ordinal) &&
            string.Equals(model.ModelGender, modelKey.Gender, StringComparison.Ordinal);
    }

    private static string GetModelGroupName(ModelKey modelKey)
    {
        if (string.Equals(modelKey.Slot, "Model", StringComparison.Ordinal) && string.IsNullOrWhiteSpace(modelKey.Gender))
        {
            return "Model";
        }

        return string.IsNullOrWhiteSpace(modelKey.Gender)
            ? modelKey.Slot + " Model"
            : $"{modelKey.Slot} Model ({modelKey.Gender})";
    }

    private static string GetSoundGroupName(SoundKey soundKey)
    {
        return soundKey.Index == 0
            ? soundKey.Slot
            : $"{soundKey.Slot} [{soundKey.Index}]";
    }

    private static string GetRawPayloadGroupName(RawPayloadKey payloadKey)
    {
        return payloadKey.Index == 0
            ? payloadKey.Slot
            : $"{payloadKey.Slot} [{payloadKey.Index}]";
    }

    private static string GetScriptFragmentGroupName(ScriptFragmentKey fragmentKey)
    {
        return fragmentKey.Index == 0
            ? fragmentKey.Slot
            : $"{fragmentKey.Slot} [{fragmentKey.Index}]";
    }

    private static string GetConditionRuleGroupName(ConditionRuleKey conditionKey)
    {
        return string.Equals(conditionKey.Slot, "Conditions", StringComparison.Ordinal)
            ? $"Condition [{conditionKey.Index}]"
            : $"{conditionKey.Slot} Condition [{conditionKey.Index}]";
    }

    private static bool IsSameModKey(ModKeyDTO first, ModKeyDTO second)
    {
        return first.Type == second.Type &&
            string.Equals(first.Name, second.Name, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(first.FileName, second.FileName, StringComparison.OrdinalIgnoreCase);
    }

    private sealed record ModelKey(string Slot, string Gender);

    private sealed record SoundKey(string Slot, int Index);

    private sealed record RawPayloadKey(string Slot, int Index);

    private sealed record ScriptFragmentKey(string Slot, int Index);

    private sealed record ConditionRuleKey(string Slot, int Index);

    private sealed class PerkEffectComparisonValue
    {
        public string MutagenObjectType { get; set; } = string.Empty;

        public int? Rank { get; set; }

        public int? Priority { get; set; }

        public int? PerkEntryId { get; set; }

        public string? Flags { get; set; }

        public TranslatedStringDTO? ButtonLabel { get; set; }

        public int? ConditionCount { get; set; }

        public string? EntryPoint { get; set; }

        public int? PerkConditionTabCount { get; set; }

        public string? Modification { get; set; }

        public double? Value { get; set; }

        public string? ActorValue { get; set; }

        public string? Spell { get; set; }

        public string? Quest { get; set; }

        public int? Stage { get; set; }
    }

}
