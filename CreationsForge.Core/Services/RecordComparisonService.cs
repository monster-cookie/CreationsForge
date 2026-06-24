using CreationsForge.Core.DTOs.Plugins;
using System.Globalization;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Records.Interfaces;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services.Interfaces;
using CreationsForge.Core.Utilities;
using Mutagen.Bethesda.Strings;

namespace CreationsForge.Core.Services;

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
    private readonly IRawRecordPayloadRepository RawRecordPayloadRepository;
    private readonly IRecordLocalizedStringRepository RecordLocalizedStringRepository;
    private readonly IGameSelectionService GameSelectionService;

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
        IRawRecordPayloadRepository rawRecordPayloadRepository,
        IRecordLocalizedStringRepository recordLocalizedStringRepository,
        IGameSelectionService gameSelectionService)
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
        RawRecordPayloadRepository = rawRecordPayloadRepository;
        RecordLocalizedStringRepository = recordLocalizedStringRepository;
        GameSelectionService = gameSelectionService;
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
        var fields = CreateCommonFields(records.Cast<RecordDTO>().ToList());
        fields.Add(CreateField("AddToList", records, record => FormatFormKey(record.AddToList)));
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
        var fields = CreateCommonFields(records.Cast<RecordDTO>().ToList());
        fields.Add(CreateField("MutagenObjectType", records, record => record.MutagenObjectType));
        fields.Add(CreateField("Data", records, record => GetGameSettingDisplayValue(localizedStrings, record, recordTextLanguage)));

        return CreateComparison(RecordTypeCatalog.GameSetting.RecordID, formKey, records.Cast<RecordDTO>().ToList(), fields);
    }

    private RecordComparisonDTO CreateGlobalComparison(SupportedGame game, FormKeyDTO formKey)
    {
        var records = GlobalRepository.GetByFormKey(game, formKey);
        var fields = CreateCommonFields(records.Cast<RecordDTO>().ToList());
        fields.Add(CreateField("MutagenObjectType", records, record => record.MutagenObjectType ?? string.Empty));
        fields.Add(CreateField("MajorFlags", records, record => record.MajorFlags ?? string.Empty));
        fields.Add(CreateField("Data", records, record => record.Data?.ToString() ?? string.Empty));

        return CreateComparison(RecordTypeCatalog.Global.RecordID, formKey, records.Cast<RecordDTO>().ToList(), fields);
    }

    private RecordComparisonDTO CreateClassComparison(SupportedGame game, FormKeyDTO formKey)
    {
        var records = ClassRepository.GetByFormKey(game, formKey);
        var localizedStrings = RecordLocalizedStringRepository.GetByFormKey(game, RecordTypeCatalog.Class.RecordID, formKey);
        var recordTextLanguage = GameSelectionService.GetRecordTextLanguage();
        var fields = CreateCommonFields(records.Cast<RecordDTO>().ToList());
        fields.Add(CreateField("Version2", records, record => record.Version2?.ToString() ?? string.Empty));
        fields.Add(CreateField("Name", records, record => GetTranslatedDisplayValue(localizedStrings, record, "Name", recordTextLanguage, record.Name)));
        fields.Add(CreateField("Description", records, record => GetTranslatedDisplayValue(localizedStrings, record, "Description", recordTextLanguage, record.Description)));
        fields.Add(CreateField("Teaches", records, record => record.Teaches ?? string.Empty));
        fields.Add(CreateField("MaxTrainingLevel", records, record => record.MaxTrainingLevel?.ToString() ?? string.Empty));
        fields.Add(CreateField("BleedoutDefault", records, record => record.BleedoutDefault?.ToString() ?? string.Empty));
        fields.Add(CreateField("VoicePoints", records, record => record.VoicePoints?.ToString() ?? string.Empty));
        fields.Add(CreateField("Unknown", records, record => record.Unknown?.ToString() ?? string.Empty));
        fields.Add(CreateField("Unknown2", records, record => record.Unknown2?.ToString() ?? string.Empty));
        AddClassPropertyGroups(fields, records);
        AddClassWeightGroups(fields, records, "Skill", "SkillWeights");
        AddClassWeightGroups(fields, records, "Stat", "StatWeights");

        return CreateComparison(RecordTypeCatalog.Class.RecordID, formKey, records.Cast<RecordDTO>().ToList(), fields);
    }

    private RecordComparisonDTO CreateFactionComparison(SupportedGame game, FormKeyDTO formKey)
    {
        var records = FactionRepository.GetByFormKey(game, formKey);
        var localizedStrings = RecordLocalizedStringRepository.GetByFormKey(game, RecordTypeCatalog.Faction.RecordID, formKey);
        var recordTextLanguage = GameSelectionService.GetRecordTextLanguage();
        var fields = CreateCommonFields(records.Cast<RecordDTO>().ToList());
        fields.Add(CreateField("Version2", records, record => record.Version2?.ToString() ?? string.Empty));
        fields.Add(CreateField("Name", records, record => GetTranslatedDisplayValue(localizedStrings, record, "Name", recordTextLanguage, record.Name)));
        fields.Add(CreateField("Flags", records, record => record.Flags ?? string.Empty));
        fields.Add(CreateField("FormationRadius", records, record => record.FormationRadius?.ToString() ?? string.Empty));
        fields.Add(CreateField("Keyword", records, record => FormatFormKey(record.Keyword)));
        fields.Add(CreateField("Herd", records, record => FormatFormKey(record.Herd)));
        fields.Add(CreateField("VoiceType", records, record => FormatFormKey(record.VoiceType)));
        fields.Add(CreateField("SharedCrimeFactionList", records, record => FormatFormKey(record.SharedCrimeFactionList)));
        fields.Add(CreateField("VendorBuySellList", records, record => FormatFormKey(record.VendorBuySellList)));
        fields.Add(CreateField("MerchantContainer", records, record => FormatFormKey(record.MerchantContainer)));
        fields.Add(CreateField("ExteriorJailMarker", records, record => FormatFormKey(record.ExteriorJailMarker)));
        fields.Add(CreateField("FollowerWaitMarker", records, record => FormatFormKey(record.FollowerWaitMarker)));
        fields.Add(CreateField("StolenGoodsContainer", records, record => FormatFormKey(record.StolenGoodsContainer)));
        fields.Add(CreateField("PlayerInventoryContainer", records, record => FormatFormKey(record.PlayerInventoryContainer)));
        fields.Add(CreateField("JailOutfit", records, record => FormatFormKey(record.JailOutfit)));
        fields.Add(CreateField("CrimeValues.Arrest", records, record => record.CrimeValues?.Arrest?.ToString() ?? string.Empty));
        fields.Add(CreateField("CrimeValues.AttackOnSight", records, record => record.CrimeValues?.AttackOnSight?.ToString() ?? string.Empty));
        fields.Add(CreateField("CrimeValues.Murder", records, record => record.CrimeValues?.Murder?.ToString() ?? string.Empty));
        fields.Add(CreateField("CrimeValues.Assault", records, record => record.CrimeValues?.Assault?.ToString() ?? string.Empty));
        fields.Add(CreateField("CrimeValues.Trespass", records, record => record.CrimeValues?.Trespass?.ToString() ?? string.Empty));
        fields.Add(CreateField("CrimeValues.Pickpocket", records, record => record.CrimeValues?.Pickpocket?.ToString() ?? string.Empty));
        fields.Add(CreateField("CrimeValues.Steal", records, record => record.CrimeValues?.Steal?.ToString() ?? string.Empty));
        fields.Add(CreateField("CrimeValues.StealMult", records, record => record.CrimeValues?.StealMult?.ToString() ?? string.Empty));
        fields.Add(CreateField("CrimeValues.StealMultiplier", records, record => record.CrimeValues?.StealMultiplier?.ToString() ?? string.Empty));
        fields.Add(CreateField("CrimeValues.Escape", records, record => record.CrimeValues?.Escape?.ToString() ?? string.Empty));
        fields.Add(CreateField("CrimeValues.Werewolf", records, record => record.CrimeValues?.Werewolf?.ToString() ?? string.Empty));
        fields.Add(CreateField("CrimeValues.WerewolfUnused", records, record => record.CrimeValues?.WerewolfUnused?.ToString() ?? string.Empty));
        fields.Add(CreateField("CrimeValues.Unknown", records, record => record.CrimeValues?.Unknown?.ToString() ?? string.Empty));
        fields.Add(CreateField("CrimeValues.Piracy", records, record => record.CrimeValues?.Piracy?.ToString() ?? string.Empty));
        fields.Add(CreateField("CrimeValues.SmuggleMultiplier", records, record => record.CrimeValues?.SmuggleMultiplier?.ToString() ?? string.Empty));
        fields.Add(CreateField("VendorValues.StartHour", records, record => record.VendorValues?.StartHour?.ToString() ?? string.Empty));
        fields.Add(CreateField("VendorValues.EndHour", records, record => record.VendorValues?.EndHour?.ToString() ?? string.Empty));
        fields.Add(CreateField("VendorValues.Radius", records, record => record.VendorValues?.Radius?.ToString() ?? string.Empty));
        fields.Add(CreateField("VendorValues.BuysStolenItems", records, record => record.VendorValues?.BuysStolenItems?.ToString() ?? string.Empty));
        fields.Add(CreateField("VendorValues.BuysNonStolenItems", records, record => record.VendorValues?.BuysNonStolenItems?.ToString() ?? string.Empty));
        fields.Add(CreateField("VendorValues.BuySellEverythingNotInList", records, record => record.VendorValues?.BuySellEverythingNotInList?.ToString() ?? string.Empty));
        fields.Add(CreateField("VendorLocation.MutagenObjectType", records, record => record.VendorLocation?.MutagenObjectType ?? string.Empty));
        fields.Add(CreateField("VendorLocation.Target.MutagenObjectType", records, record => record.VendorLocation?.Target?.MutagenObjectType ?? string.Empty));
        fields.Add(CreateField("VendorLocation.Target.Type", records, record => record.VendorLocation?.Target?.Type ?? string.Empty));
        fields.Add(CreateField("VendorLocation.Target.Link", records, record => FormatFormKey(record.VendorLocation?.Target?.Link)));
        AddFactionRelationGroups(fields, records);
        AddFactionRankGroups(fields, records, localizedStrings, recordTextLanguage);
        AddConditionRuleGroups(fields, records.Cast<RecordDTO>().ToList(), records.Cast<IHasConditionsDTO>().ToList());
        AddRecordComponentGroups(fields, records.Cast<RecordDTO>().ToList(), records.SelectMany(record => record.Components).ToList());
        AddKeywordGroup(fields, records.Cast<RecordDTO>().ToList(), KeywordMappingRepository.GetByFormKey(game, RecordTypeCatalog.Faction.RecordID, formKey));

        return CreateComparison(RecordTypeCatalog.Faction.RecordID, formKey, records.Cast<RecordDTO>().ToList(), fields);
    }

    private RecordComparisonDTO CreateMiscItemComparison(SupportedGame game, FormKeyDTO formKey)
    {
        var records = MiscItemRepository.GetByFormKey(game, formKey);
        var localizedStrings = RecordLocalizedStringRepository.GetByFormKey(game, RecordTypeCatalog.MiscItem.RecordID, formKey);
        var recordTextLanguage = GameSelectionService.GetRecordTextLanguage();
        var fields = CreateCommonFields(records.Cast<RecordDTO>().ToList());
        fields.Add(CreateField("ObjectBoundsFirst", records, record => record.ObjectBounds?.First ?? string.Empty));
        fields.Add(CreateField("ObjectBoundsSecond", records, record => record.ObjectBounds?.Second ?? string.Empty));
        fields.Add(CreateField("Transforms.Inventory", records, record => FormatFormKey(record.Transforms?.Inventory)));
        fields.Add(CreateField("PreviewTransform", records, record => FormatFormKey(record.PreviewTransform)));
        fields.Add(CreateField("Name", records, record => GetTranslatedDisplayValue(localizedStrings, record, "Name", recordTextLanguage, record.Name)));
        fields.Add(CreateField("ShortName", records, record => GetTranslatedDisplayValue(localizedStrings, record, "ShortName", recordTextLanguage, record.ShortName)));
        fields.Add(CreateField("Value", records, record => record.Value?.ToString() ?? string.Empty));
        fields.Add(CreateField("Weight", records, record => record.Weight?.ToString() ?? string.Empty));
        fields.Add(CreateField("DirtinessScale", records, record => record.DirtinessScale?.ToString() ?? string.Empty));
        fields.Add(CreateField("FeaturedItemMessage", records, record => FormatFormKey(record.FeaturedItemMessage)));
        fields.Add(CreateField("Flag", records, record => record.Flag ?? string.Empty));
        AddMiscItemDestructibleGroups(fields, records);
        AddKeywordGroup(fields, records.Cast<RecordDTO>().ToList(), KeywordMappingRepository.GetByFormKey(game, RecordTypeCatalog.MiscItem.RecordID, formKey));
        AddModelGroups(fields, records.Cast<RecordDTO>().ToList(), ModelRepository.GetByFormKey(game, RecordTypeCatalog.MiscItem.RecordID, formKey));
        AddSoundGroups(fields, records.Cast<RecordDTO>().ToList(), SoundMappingRepository.GetByFormKey(game, RecordTypeCatalog.MiscItem.RecordID, formKey));
        AddScriptingAdapterGroups(fields, records.Cast<RecordDTO>().ToList(), ScriptingAdapterRepository.GetByFormKey(game, RecordTypeCatalog.MiscItem.RecordID, formKey));
        AddMiscItemComponentGroups(fields, records);
        AddMiscItemResourceGroups(fields, records);

        return CreateComparison(RecordTypeCatalog.MiscItem.RecordID, formKey, records.Cast<RecordDTO>().ToList(), fields);
    }

    private RecordComparisonDTO CreateKeywordComparison(SupportedGame game, FormKeyDTO formKey)
    {
        var records = KeywordRepository.GetByFormKey(game, formKey);
        var localizedStrings = RecordLocalizedStringRepository.GetByFormKey(game, RecordTypeCatalog.Keyword.RecordID, formKey);
        var recordTextLanguage = GameSelectionService.GetRecordTextLanguage();
        var fields = CreateCommonFields(records.Cast<RecordDTO>().ToList());
        fields.Add(CreateField("Name", records, record => GetTranslatedDisplayValue(localizedStrings, record, "Name", recordTextLanguage, record.Name)));
        fields.Add(CreateField("Color", records, record => record.Color ?? string.Empty));
        fields.Add(CreateField("Type", records, record => record.Type ?? string.Empty));
        fields.Add(CreateField("Notes", records, record => record.Notes ?? string.Empty));
        fields.Add(CreateField("FlashLinkageName", records, record => record.FlashLinkageName ?? string.Empty));
        fields.Add(CreateField("FNAM", records, record => record.FNAM ?? string.Empty));
        fields.Add(CreateField("WAIM", records, record => record.WAIM ?? string.Empty));
        fields.Add(CreateField("WFIR", records, record => record.WFIR ?? string.Empty));
        fields.Add(CreateField("AttractionRule", records, record => FormatFormKey(record.AttractionRule)));

        return CreateComparison(RecordTypeCatalog.Keyword.RecordID, formKey, records.Cast<RecordDTO>().ToList(), fields);
    }

    private RecordComparisonDTO CreateActorValueInformationComparison(SupportedGame game, FormKeyDTO formKey)
    {
        var records = ActorValueInformationRepository.GetByFormKey(game, formKey);
        var localizedStrings = RecordLocalizedStringRepository.GetByFormKey(game, RecordTypeCatalog.ActorValueInformation.RecordID, formKey);
        var recordTextLanguage = GameSelectionService.GetRecordTextLanguage();
        var fields = CreateCommonFields(records.Cast<RecordDTO>().ToList());
        fields.Add(CreateField("Name", records, record => GetTranslatedDisplayValue(localizedStrings, record, "Name", recordTextLanguage, record.Name)));
        fields.Add(CreateField("Abbreviation", records, record => GetTranslatedDisplayValue(localizedStrings, record, "Abbreviation", recordTextLanguage, record.Abbreviation)));
        fields.Add(CreateField("Description", records, record => GetTranslatedDisplayValue(localizedStrings, record, "Description", recordTextLanguage, record.Description)));
        fields.Add(CreateField("CNAM", records, record => record.CNAM ?? string.Empty));
        fields.Add(CreateField("Skill.ImproveMult", records, record => record.Skill?.ImproveMult?.ToString() ?? string.Empty));
        fields.Add(CreateField("Skill.ImproveOffset", records, record => record.Skill?.ImproveOffset?.ToString() ?? string.Empty));
        fields.Add(CreateField("Skill.UseMult", records, record => record.Skill?.UseMult?.ToString() ?? string.Empty));
        fields.Add(CreateField("ContextNotes", records, record => record.ContextNotes ?? string.Empty));
        fields.Add(CreateField("DefaultValue", records, record => record.DefaultValue?.ToString() ?? string.Empty));
        fields.Add(CreateField("Flags", records, record => record.Flags ?? string.Empty));
        fields.Add(CreateField("Type", records, record => record.Type ?? string.Empty));
        fields.Add(CreateField("Min", records, record => record.Min?.ToString() ?? string.Empty));
        fields.Add(CreateField("Max", records, record => record.Max?.ToString() ?? string.Empty));
        AddActorValueInformationPerkTreeGroups(fields, records);

        return CreateComparison(RecordTypeCatalog.ActorValueInformation.RecordID, formKey, records.Cast<RecordDTO>().ToList(), fields);
    }

    private RecordComparisonDTO CreateNPCComparison(SupportedGame game, FormKeyDTO formKey)
    {
        var records = NPCRepository.GetByFormKey(game, formKey);
        var localizedStrings = RecordLocalizedStringRepository.GetByFormKey(game, RecordTypeCatalog.NPC.RecordID, formKey);
        var recordTextLanguage = GameSelectionService.GetRecordTextLanguage();
        var fields = CreateCommonFields(records.Cast<RecordDTO>().ToList());
        fields.Add(CreateField("Name", records, record => GetTranslatedDisplayValue(localizedStrings, record, "Name", recordTextLanguage, record.Name)));
        fields.Add(CreateField("ShortName", records, record => GetTranslatedDisplayValue(localizedStrings, record, "ShortName", recordTextLanguage, record.ShortName)));
        fields.Add(CreateField("LongName", records, record => GetTranslatedDisplayValue(localizedStrings, record, "LongName", recordTextLanguage, record.LongName)));
        fields.Add(CreateField("DispositionBase", records, record => record.DispositionBase.ToString()));
        fields.Add(CreateField("Aggression", records, record => record.Aggression));
        fields.Add(CreateField("Confidence", records, record => record.Confidence));
        fields.Add(CreateField("EnergyLevel", records, record => record.EnergyLevel.ToString()));
        fields.Add(CreateField("Responsibility", records, record => record.Responsibility));
        fields.Add(CreateField("Assistance", records, record => record.Assistance));
        fields.Add(CreateField("GearedUpWeapons", records, record => record.GearedUpWeapons.ToString()));
        fields.Add(CreateField("HeightMin", records, record => record.HeightMin.ToString()));
        fields.Add(CreateField("HeightMax", records, record => record.HeightMax.ToString()));
        fields.Add(CreateField("SkinToneIndex", records, record => record.SkinToneIndex?.ToString() ?? string.Empty));
        fields.Add(CreateField("Pronoun", records, record => record.Pronoun ?? string.Empty));
        fields.Add(CreateField("VoiceFormKey", records, record => FormatFormKey(record.VoiceFormKey)));
        fields.Add(CreateField("RaceFormKey", records, record => FormatFormKey(record.RaceFormKey)));
        fields.Add(CreateField("CombatOverridePackageListFormKey", records, record => FormatFormKey(record.CombatOverridePackageListFormKey)));
        fields.Add(CreateField("CombatStyleFormKey", records, record => FormatFormKey(record.CombatStyleFormKey)));
        fields.Add(CreateField("DefaultPackageListFormKey", records, record => FormatFormKey(record.DefaultPackageListFormKey)));
        fields.Add(CreateField("CrimeFactionFormKey", records, record => FormatFormKey(record.CrimeFactionFormKey)));
        AddKeywordGroup(fields, records.Cast<RecordDTO>().ToList(), KeywordMappingRepository.GetByFormKey(game, RecordTypeCatalog.NPC.RecordID, formKey));
        AddSoundGroups(fields, records.Cast<RecordDTO>().ToList(), SoundMappingRepository.GetByFormKey(game, RecordTypeCatalog.NPC.RecordID, formKey));
        AddScriptingAdapterGroups(fields, records.Cast<RecordDTO>().ToList(), ScriptingAdapterRepository.GetByFormKey(game, RecordTypeCatalog.NPC.RecordID, formKey));
        AddNPCSupplementalFields(fields, records);

        return CreateComparison(RecordTypeCatalog.NPC.RecordID, formKey, records.Cast<RecordDTO>().ToList(), fields);
    }

    private RecordComparisonDTO CreateMagicEffectComparison(SupportedGame game, FormKeyDTO formKey)
    {
        var records = MagicEffectRepository.GetByFormKey(game, formKey);
        var localizedStrings = RecordLocalizedStringRepository.GetByFormKey(game, RecordTypeCatalog.MagicEffect.RecordID, formKey);
        var recordTextLanguage = GameSelectionService.GetRecordTextLanguage();
        var fields = CreateCommonFields(records.Cast<RecordDTO>().ToList());
        fields.Add(CreateField("Name", records, record => GetTranslatedDisplayValue(localizedStrings, record, "Name", recordTextLanguage, record.Name)));
        fields.Add(CreateField("Description", records, record => GetTranslatedDisplayValue(localizedStrings, record, "Description", recordTextLanguage, record.Description)));
        fields.Add(CreateField("Flags", records, record => record.Flags));
        fields.Add(CreateField("CastType", records, record => record.CastType ?? string.Empty));
        fields.Add(CreateField("TargetType", records, record => record.TargetType ?? string.Empty));
        fields.Add(CreateField("ActorValue2FormKey", records, record => FormatFormKey(record.ActorValue2FormKey)));
        fields.Add(CreateField("ResistValueFormKey", records, record => FormatFormKey(record.ResistValueFormKey)));
        fields.Add(CreateField("PerkToApplyFormKey", records, record => FormatFormKey(record.PerkToApplyFormKey)));
        fields.Add(CreateField("EquipAbilityFormKey", records, record => FormatFormKey(record.EquipAbilityFormKey)));
        fields.Add(CreateField("ExplosionFormKey", records, record => FormatFormKey(record.ExplosionFormKey)));
        fields.Add(CreateField("CastingArtFormKey", records, record => FormatFormKey(record.CastingArtFormKey)));
        fields.Add(CreateField("HitEffectArtFormKey", records, record => FormatFormKey(record.HitEffectArtFormKey)));
        fields.Add(CreateField("HitShaderFormKey", records, record => FormatFormKey(record.HitShaderFormKey)));
        fields.Add(CreateField("ImageSpaceModifierFormKey", records, record => FormatFormKey(record.ImageSpaceModifierFormKey)));
        fields.Add(CreateField("ImpactDataFormKey", records, record => FormatFormKey(record.ImpactDataFormKey)));
        fields.Add(CreateField("ProjectileFormKey", records, record => FormatFormKey(record.ProjectileFormKey)));
        fields.Add(CreateField("Archetype", records, record => record.Archetype ?? string.Empty));
        fields.Add(CreateField("UnknownFloat3", records, record => record.UnknownFloat3?.ToString() ?? string.Empty));
        fields.Add(CreateField("UnknownInt2", records, record => record.UnknownInt2?.ToString() ?? string.Empty));
        fields.Add(CreateField("Unknown", records, record => record.Unknown ?? string.Empty));
        fields.Add(CreateField("Unknown2", records, record => record.Unknown2 ?? string.Empty));
        fields.Add(CreateField("DataTypeState", records, record => record.DataTypeState ?? string.Empty));
        AddKeywordGroup(fields, records.Cast<RecordDTO>().ToList(), KeywordMappingRepository.GetByFormKey(game, RecordTypeCatalog.MagicEffect.RecordID, formKey));
        AddSoundGroups(fields, records.Cast<RecordDTO>().ToList(), SoundMappingRepository.GetByFormKey(game, RecordTypeCatalog.MagicEffect.RecordID, formKey));
        AddScriptingAdapterGroups(fields, records.Cast<RecordDTO>().ToList(), ScriptingAdapterRepository.GetByFormKey(game, RecordTypeCatalog.MagicEffect.RecordID, formKey));

        return CreateComparison(RecordTypeCatalog.MagicEffect.RecordID, formKey, records.Cast<RecordDTO>().ToList(), fields);
    }

    private RecordComparisonDTO CreatePerkComparison(SupportedGame game, FormKeyDTO formKey)
    {
        var records = PerkRepository.GetByFormKey(game, formKey);
        var localizedStrings = RecordLocalizedStringRepository.GetByFormKey(game, RecordTypeCatalog.Perk.RecordID, formKey);
        var recordTextLanguage = GameSelectionService.GetRecordTextLanguage();
        var fields = CreateCommonFields(records.Cast<RecordDTO>().ToList());
        fields.Add(CreateField("Name", records, record => GetTranslatedDisplayValue(localizedStrings, record, "Name", recordTextLanguage, record.Name)));
        fields.Add(CreateField("Description", records, record => GetTranslatedDisplayValue(localizedStrings, record, "Description", recordTextLanguage, record.Description)));
        fields.Add(CreateField("Flags", records, record => record.Flags));
        fields.Add(CreateField("SkillGroup", records, record => record.SkillGroup ?? string.Empty));
        fields.Add(CreateField("CrewAssignment", records, record => record.CrewAssignment ?? string.Empty));
        fields.Add(CreateField("PerkIcon", records, record => record.PerkIcon ?? string.Empty));
        fields.Add(CreateField("Category", records, record => record.Category ?? string.Empty));
        fields.Add(CreateField("RestrictionFormKey", records, record => FormatFormKey(record.RestrictionFormKey)));
        fields.Add(CreateField("TrainingFormKey", records, record => FormatFormKey(record.TrainingFormKey)));
        fields.Add(CreateField("Level", records, record => record.Level?.ToString() ?? string.Empty));
        fields.Add(CreateField("NumRanks", records, record => record.NumRanks?.ToString() ?? string.Empty));
        fields.Add(CreateField("Playable", records, record => record.Playable?.ToString() ?? string.Empty));
        fields.Add(CreateField("Hidden", records, record => record.Hidden?.ToString() ?? string.Empty));
        fields.Add(CreateField("NextPerk", records, record => FormatFormKey(record.NextPerk)));
        fields.Add(CreateField("MajorFlags", records, record => record.MajorFlags ?? string.Empty));
        AddPerkEffectGroups(fields, records, localizedStrings, recordTextLanguage);
        AddPerkRankGroups(fields, records, localizedStrings, recordTextLanguage);
        AddPerkBackgroundSkillGroup(fields, records);
        AddConditionRuleGroups(fields, records.Cast<RecordDTO>().ToList(), records.Cast<IHasConditionsDTO>().ToList());
        AddSoundGroups(fields, records.Cast<RecordDTO>().ToList(), SoundMappingRepository.GetByFormKey(game, RecordTypeCatalog.Perk.RecordID, formKey));
        AddScriptFragmentGroups(fields, records.Cast<RecordDTO>().ToList(), records.SelectMany(record => record.ScriptFragments).ToList());
        AddScriptingAdapterGroups(fields, records.Cast<RecordDTO>().ToList(), ScriptingAdapterRepository.GetByFormKey(game, RecordTypeCatalog.Perk.RecordID, formKey));

        return CreateComparison(RecordTypeCatalog.Perk.RecordID, formKey, records.Cast<RecordDTO>().ToList(), fields);
    }

    private RecordComparisonDTO CreateStaticComparison(SupportedGame game, FormKeyDTO formKey)
    {
        var records = StaticRepository.GetByFormKey(game, formKey);
        var localizedStrings = RecordLocalizedStringRepository.GetByFormKey(game, RecordTypeCatalog.Static.RecordID, formKey);
        var recordTextLanguage = GameSelectionService.GetRecordTextLanguage();
        var fields = CreateCommonFields(records.Cast<RecordDTO>().ToList());
        fields.Add(CreateField("Name", records, record => GetTranslatedDisplayValue(localizedStrings, record, "Name", recordTextLanguage, record.Name)));
        fields.Add(CreateField("Version2", records, record => record.Version2?.ToString() ?? string.Empty));
        fields.Add(CreateField("ObjectBoundsFirst", records, record => record.ObjectBoundsFirst ?? string.Empty));
        fields.Add(CreateField("ObjectBoundsSecond", records, record => record.ObjectBoundsSecond ?? string.Empty));
        fields.Add(CreateField("MaxAngle", records, record => record.MaxAngle?.ToString() ?? string.Empty));
        fields.Add(CreateField("UnknownDNAMFloat", records, record => record.UnknownDNAMFloat?.ToString() ?? string.Empty));
        fields.Add(CreateField("LeafAmplitude", records, record => record.LeafAmplitude?.ToString() ?? string.Empty));
        fields.Add(CreateField("LeafFrequency", records, record => record.LeafFrequency?.ToString() ?? string.Empty));
        fields.Add(CreateField("Unused", records, record => record.Unused ?? string.Empty));
        fields.Add(CreateField("DNAMDataTypeState", records, record => record.DNAMDataTypeState ?? string.Empty));
        fields.Add(CreateField("DirtinessScale", records, record => record.DirtinessScale?.ToString() ?? string.Empty));
        fields.Add(CreateField("SnapTemplate", records, record => FormatFormKey(record.SnapTemplate)));
        fields.Add(CreateField("PreviewTransform", records, record => FormatFormKey(record.PreviewTransform)));
        fields.Add(CreateField("Material", records, record => FormatFormKey(record.Material)));
        fields.Add(CreateField("Lod.Level0", records, record => record.LodLevel0 ?? string.Empty));
        fields.Add(CreateField("Lod.Level1", records, record => record.LodLevel1 ?? string.Empty));
        fields.Add(CreateField("Lod.Level2", records, record => record.LodLevel2 ?? string.Empty));
        fields.Add(CreateField("Lod.Level3", records, record => record.LodLevel3 ?? string.Empty));
        fields.Add(CreateField("NavmeshGeometry", records, record => record.NavmeshGeometry ?? string.Empty));
        AddKeywordGroup(fields, records.Cast<RecordDTO>().ToList(), KeywordMappingRepository.GetByFormKey(game, RecordTypeCatalog.Static.RecordID, formKey));
        AddStaticPropertyGroups(fields, records);
        AddModelGroups(fields, records.Cast<RecordDTO>().ToList(), ModelRepository.GetByFormKey(game, RecordTypeCatalog.Static.RecordID, formKey));
        AddRawPayloadGroups(fields, records.Cast<RecordDTO>().ToList(), RawRecordPayloadRepository.GetByFormKey(game, RecordTypeCatalog.Static.RecordID, formKey));

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

    private static StaticPropertyDTO? FindStaticProperty(StaticDTO record, int propertyIndex)
    {
        return record.Properties.FirstOrDefault(property => property.PropertyIndex == propertyIndex);
    }

    private RecordComparisonDTO CreateBookComparison(SupportedGame game, FormKeyDTO formKey)
    {
        var records = BookRepository.GetByFormKey(game, formKey);
        var localizedStrings = RecordLocalizedStringRepository.GetByFormKey(game, RecordTypeCatalog.Book.RecordID, formKey);
        var recordTextLanguage = GameSelectionService.GetRecordTextLanguage();
        var baseRecords = records.Cast<RecordDTO>().ToList();
        var fields = CreateCommonFields(baseRecords);
        fields.Add(CreateField("Version2", records, record => record.Version2?.ToString() ?? string.Empty));
        fields.Add(CreateField("ObjectBounds.First", records, record => record.ObjectBounds?.First ?? string.Empty));
        fields.Add(CreateField("ObjectBounds.Second", records, record => record.ObjectBounds?.Second ?? string.Empty));
        fields.Add(CreateField("Transforms.Inventory", records, record => FormatFormKey(record.Transforms?.Inventory)));
        fields.Add(CreateField("InventoryArt", records, record => FormatFormKey(record.InventoryArt)));
        fields.Add(CreateField("PreviewTransform", records, record => FormatFormKey(record.PreviewTransform)));
        fields.Add(CreateField("FeaturedItemMessage", records, record => FormatFormKey(record.FeaturedItemMessage)));
        fields.Add(CreateField("XALG", records, record => record.XALG?.ToString() ?? string.Empty));
        fields.Add(CreateField("Name", records, record => GetTranslatedDisplayValue(localizedStrings, record, "Name", recordTextLanguage, record.Name)));
        fields.Add(CreateField("Text", records, record => GetTranslatedDisplayValue(localizedStrings, record, GetBookTextSourceField(record), recordTextLanguage, record.Text)));
        fields.Add(CreateField("Value", records, record => record.Value?.ToString() ?? string.Empty));
        fields.Add(CreateField("Weight", records, record => record.Weight?.ToString() ?? string.Empty));
        fields.Add(CreateField("Flags", records, record => record.Flags ?? string.Empty));
        fields.Add(CreateField("Teaches.MutagenObjectType", records, record => record.Teaches?.MutagenObjectType ?? string.Empty));
        fields.Add(CreateField("Teaches.Perk", records, record => FormatFormKey(record.Teaches?.Perk)));
        fields.Add(CreateField("Teaches.RawContent", records, record => record.Teaches?.RawContent ?? string.Empty));
        fields.Add(CreateField("DataSlateType", records, record => record.DataSlateType ?? string.Empty));
        fields.Add(CreateField("Description", records, record => GetTranslatedDisplayValue(localizedStrings, record, "Description", recordTextLanguage, record.Description)));
        fields.Add(CreateField("DataSlateHeaderLeft", records, record => GetTranslatedDisplayValue(localizedStrings, record, "DataSlateHeaderLeft", recordTextLanguage, record.DataSlateHeaderLeft)));
        fields.Add(CreateField("DataSlateHeaderRight", records, record => GetTranslatedDisplayValue(localizedStrings, record, "DataSlateHeaderRight", recordTextLanguage, record.DataSlateHeaderRight)));
        AddKeywordGroup(fields, baseRecords, KeywordMappingRepository.GetByFormKey(game, RecordTypeCatalog.Book.RecordID, formKey));
        AddModelGroups(fields, baseRecords, ModelRepository.GetByFormKey(game, RecordTypeCatalog.Book.RecordID, formKey));
        AddSoundGroups(fields, baseRecords, SoundMappingRepository.GetByFormKey(game, RecordTypeCatalog.Book.RecordID, formKey));
        AddScriptingAdapterGroups(fields, baseRecords, ScriptingAdapterRepository.GetByFormKey(game, RecordTypeCatalog.Book.RecordID, formKey));
        AddRecordComponentGroups(fields, baseRecords, records.SelectMany(record => record.Components).ToList());
        AddRawPayloadGroups(fields, baseRecords, RawRecordPayloadRepository.GetByFormKey(game, RecordTypeCatalog.Book.RecordID, formKey));

        return CreateComparison(RecordTypeCatalog.Book.RecordID, formKey, baseRecords, fields);
    }

    private static string GetBookTextSourceField(BookDTO record)
    {
        return record.Game == SupportedGame.Starfield ? "Text" : "BookText";
    }

    private RecordComparisonDTO CreateDoorComparison(SupportedGame game, FormKeyDTO formKey)
    {
        var records = DoorRepository.GetByFormKey(game, formKey);
        var localizedStrings = RecordLocalizedStringRepository.GetByFormKey(game, RecordTypeCatalog.Door.RecordID, formKey);
        var recordTextLanguage = GameSelectionService.GetRecordTextLanguage();
        var baseRecords = records.Cast<RecordDTO>().ToList();
        var fields = CreateCommonFields(baseRecords);
        fields.Add(CreateField("Version2", records, record => record.Version2?.ToString() ?? string.Empty));
        fields.Add(CreateField("ObjectBoundsFirst", records, record => record.ObjectBoundsFirst ?? string.Empty));
        fields.Add(CreateField("ObjectBoundsSecond", records, record => record.ObjectBoundsSecond ?? string.Empty));
        fields.Add(CreateField("Name", records, record => GetTranslatedDisplayValue(localizedStrings, record, "Name", recordTextLanguage, record.Name)));
        fields.Add(CreateField("Flags", records, record => record.Flags ?? string.Empty));
        fields.Add(CreateField("NativeTerminalFormKey", records, record => FormatFormKey(record.NativeTerminalFormKey)));
        fields.Add(CreateField("SoundLevel", records, record => record.SoundLevel ?? string.Empty));
        fields.Add(CreateField("FacingAxisOverride", records, record => record.FacingAxisOverride ?? string.Empty));
        fields.Add(CreateField("AnimationGraph", records, record => record.AnimationGraph ?? string.Empty));
        fields.Add(CreateField("AnimationSkeleton", records, record => record.AnimationSkeleton ?? string.Empty));
        fields.Add(CreateField("AnimationDirectory", records, record => record.AnimationDirectory ?? string.Empty));
        fields.Add(CreateField("AnimationFile", records, record => record.AnimationFile ?? string.Empty));
        AddKeywordGroup(fields, baseRecords, KeywordMappingRepository.GetByFormKey(game, RecordTypeCatalog.Door.RecordID, formKey));
        AddModelGroups(fields, baseRecords, ModelRepository.GetByFormKey(game, RecordTypeCatalog.Door.RecordID, formKey));
        AddSoundGroups(fields, baseRecords, SoundMappingRepository.GetByFormKey(game, RecordTypeCatalog.Door.RecordID, formKey));
        AddScriptingAdapterGroups(fields, baseRecords, ScriptingAdapterRepository.GetByFormKey(game, RecordTypeCatalog.Door.RecordID, formKey));
        AddRecordComponentGroups(fields, baseRecords, records.SelectMany(record => record.Components).ToList());
        AddRawPayloadGroups(fields, baseRecords, RawRecordPayloadRepository.GetByFormKey(game, RecordTypeCatalog.Door.RecordID, formKey));

        return CreateComparison(RecordTypeCatalog.Door.RecordID, formKey, baseRecords, fields);
    }

    private RecordComparisonDTO CreateContainerComparison(SupportedGame game, FormKeyDTO formKey)
    {
        var records = ContainerRepository.GetByFormKey(game, formKey);
        var localizedStrings = RecordLocalizedStringRepository.GetByFormKey(game, RecordTypeCatalog.Container.RecordID, formKey);
        var recordTextLanguage = GameSelectionService.GetRecordTextLanguage();
        var baseRecords = records.Cast<RecordDTO>().ToList();
        var fields = CreateCommonFields(baseRecords);
        fields.Add(CreateField("Version2", records, record => record.Version2?.ToString() ?? string.Empty));
        fields.Add(CreateField("ObjectBoundsFirst", records, record => record.ObjectBoundsFirst ?? string.Empty));
        fields.Add(CreateField("ObjectBoundsSecond", records, record => record.ObjectBoundsSecond ?? string.Empty));
        fields.Add(CreateField("Name", records, record => GetTranslatedDisplayValue(localizedStrings, record, "Name", recordTextLanguage, record.Name)));
        fields.Add(CreateField("Flags", records, record => record.Flags ?? string.Empty));
        fields.Add(CreateField("MajorFlags", records, record => record.MajorFlags ?? string.Empty));
        fields.Add(CreateField("NativeTerminalFormKey", records, record => FormatFormKey(record.NativeTerminalFormKey)));
        fields.Add(CreateField("AnimationGraph", records, record => record.AnimationGraph ?? string.Empty));
        fields.Add(CreateField("AnimationSkeleton", records, record => record.AnimationSkeleton ?? string.Empty));
        fields.Add(CreateField("AnimationDirectory", records, record => record.AnimationDirectory ?? string.Empty));
        fields.Add(CreateField("AnimationFile", records, record => record.AnimationFile ?? string.Empty));
        AddContainerItemGroups(fields, records);
        AddKeywordGroup(fields, baseRecords, KeywordMappingRepository.GetByFormKey(game, RecordTypeCatalog.Container.RecordID, formKey));
        AddModelGroups(fields, baseRecords, ModelRepository.GetByFormKey(game, RecordTypeCatalog.Container.RecordID, formKey));
        AddSoundGroups(fields, baseRecords, SoundMappingRepository.GetByFormKey(game, RecordTypeCatalog.Container.RecordID, formKey));
        AddScriptingAdapterGroups(fields, baseRecords, ScriptingAdapterRepository.GetByFormKey(game, RecordTypeCatalog.Container.RecordID, formKey));
        AddRawPayloadGroups(fields, baseRecords, RawRecordPayloadRepository.GetByFormKey(game, RecordTypeCatalog.Container.RecordID, formKey));

        return CreateComparison(RecordTypeCatalog.Container.RecordID, formKey, baseRecords, fields);
    }

    private RecordComparisonDTO CreateConstructibleObjectComparison(SupportedGame game, FormKeyDTO formKey)
    {
        var records = ConstructibleObjectRepository.GetByFormKey(game, formKey);
        var localizedStrings = RecordLocalizedStringRepository.GetByFormKey(game, RecordTypeCatalog.ConstructibleObject.RecordID, formKey);
        var recordTextLanguage = GameSelectionService.GetRecordTextLanguage();
        var baseRecords = records.Cast<RecordDTO>().ToList();
        var fields = CreateCommonFields(baseRecords);
        fields.Add(CreateField("Version2", records, record => record.Version2?.ToString() ?? string.Empty));
        fields.Add(CreateField("Description", records, record => GetTranslatedDisplayValue(localizedStrings, record, "Description", recordTextLanguage, record.Description)));
        fields.Add(CreateField("CreatedObjectFormKey", records, record => FormatFormKey(record.CreatedObjectFormKey)));
        fields.Add(CreateField("WorkbenchKeywordFormKey", records, record => FormatFormKey(record.WorkbenchKeywordFormKey)));
        fields.Add(CreateField("CreatedObjectCount", records, record => record.CreatedObjectCount?.ToString() ?? string.Empty));
        fields.Add(CreateField("AmountProduced", records, record => record.AmountProduced?.ToString() ?? string.Empty));
        fields.Add(CreateField("Value", records, record => record.Value?.ToString() ?? string.Empty));
        fields.Add(CreateField("MenuSortOrder", records, record => record.MenuSortOrder?.ToString() ?? string.Empty));
        fields.Add(CreateField("LearnMethod", records, record => record.LearnMethod ?? string.Empty));
        fields.Add(CreateField("Flags", records, record => record.Flags ?? string.Empty));
        fields.Add(CreateField("MajorFlags", records, record => record.MajorFlags ?? string.Empty));
        AddConstructibleObjectComponentGroups(fields, records);
        AddConstructibleObjectCategoryGroups(fields, records);
        AddConstructibleObjectRecipeFilterGroups(fields, records);
        AddConditionRuleGroups(fields, baseRecords, records.Cast<IHasConditionsDTO>().ToList());
        AddSoundGroups(fields, baseRecords, SoundMappingRepository.GetByFormKey(game, RecordTypeCatalog.ConstructibleObject.RecordID, formKey));
        AddScriptingAdapterGroups(fields, baseRecords, ScriptingAdapterRepository.GetByFormKey(game, RecordTypeCatalog.ConstructibleObject.RecordID, formKey));

        return CreateComparison(RecordTypeCatalog.ConstructibleObject.RecordID, formKey, baseRecords, fields);
    }

    private RecordComparisonDTO CreateConditionFormComparison(SupportedGame game, FormKeyDTO formKey)
    {
        var records = ConditionFormRepository.GetByFormKey(game, formKey);
        var baseRecords = records.Cast<RecordDTO>().ToList();
        var fields = CreateCommonFields(baseRecords);
        fields.Add(CreateField("Version2", records, record => record.Version2?.ToString() ?? string.Empty));
        AddConditionRuleGroups(fields, baseRecords, records.Cast<IHasConditionsDTO>().ToList());

        return CreateComparison(RecordTypeCatalog.ConditionForm.RecordID, formKey, baseRecords, fields);
    }

    private RecordComparisonDTO CreateTerminalComparison(SupportedGame game, FormKeyDTO formKey)
    {
        var records = TerminalRepository.GetByFormKey(game, formKey);
        var localizedStrings = RecordLocalizedStringRepository.GetByFormKey(game, RecordTypeCatalog.Terminal.RecordID, formKey);
        var recordTextLanguage = GameSelectionService.GetRecordTextLanguage();
        var baseRecords = records.Cast<RecordDTO>().ToList();
        var fields = CreateCommonFields(baseRecords);
        fields.Add(CreateField("Version2", records, record => record.Version2?.ToString() ?? string.Empty));
        fields.Add(CreateField("ObjectBoundsFirst", records, record => record.ObjectBoundsFirst ?? string.Empty));
        fields.Add(CreateField("ObjectBoundsSecond", records, record => record.ObjectBoundsSecond ?? string.Empty));
        fields.Add(CreateField("MenuFormKey", records, record => FormatFormKey(record.MenuFormKey)));
        fields.Add(CreateField("Background", records, record => record.Background ?? string.Empty));
        fields.Add(CreateField("HeaderText", records, record => GetTranslatedDisplayValue(localizedStrings, record, "HeaderText", recordTextLanguage, record.HeaderText)));
        fields.Add(CreateField("WelcomeText", records, record => GetTranslatedDisplayValue(localizedStrings, record, "WelcomeText", recordTextLanguage, record.WelcomeText)));
        fields.Add(CreateField("Name", records, record => GetTranslatedDisplayValue(localizedStrings, record, "Name", recordTextLanguage, record.Name)));
        fields.Add(CreateField("Pnam", records, record => record.Pnam ?? string.Empty));
        fields.Add(CreateField("Fnam", records, record => record.Fnam ?? string.Empty));
        fields.Add(CreateField("Flags", records, record => record.Flags ?? string.Empty));
        fields.Add(CreateField("MajorFlags", records, record => record.MajorFlags ?? string.Empty));
        fields.Add(CreateField("Jnam", records, record => record.Jnam ?? string.Empty));
        fields.Add(CreateField("MarkerFlags", records, record => FormatHexIntegerString(record.MarkerFlags)));
        fields.Add(CreateField("Gnam", records, record => record.Gnam ?? string.Empty));
        fields.Add(CreateField("WorkbenchData", records, record => record.WorkbenchData ?? string.Empty));
        fields.Add(CreateField("FurnitureTemplateFormKey", records, record => FormatFormKey(record.FurnitureTemplateFormKey)));
        fields.Add(CreateField("MarkerModel", records, record => record.MarkerModel ?? string.Empty));
        fields.Add(CreateField("AnimationGraph", records, record => record.AnimationGraph ?? string.Empty));
        fields.Add(CreateField("AnimationSkeleton", records, record => record.AnimationSkeleton ?? string.Empty));
        fields.Add(CreateField("AnimationDirectory", records, record => record.AnimationDirectory ?? string.Empty));
        fields.Add(CreateField("AnimationFile", records, record => record.AnimationFile ?? string.Empty));
        AddTerminalForcedLocationGroups(fields, records);
        AddKeywordGroup(fields, baseRecords, KeywordMappingRepository.GetByFormKey(game, RecordTypeCatalog.Terminal.RecordID, formKey));
        AddModelGroups(fields, baseRecords, ModelRepository.GetByFormKey(game, RecordTypeCatalog.Terminal.RecordID, formKey));
        AddScriptingAdapterGroups(fields, baseRecords, ScriptingAdapterRepository.GetByFormKey(game, RecordTypeCatalog.Terminal.RecordID, formKey));
        AddConditionRuleGroups(fields, baseRecords, records.Cast<IHasConditionsDTO>().ToList());
        AddScriptFragmentGroups(fields, baseRecords, records.SelectMany(record => record.ScriptFragments).ToList());
        AddRawPayloadGroups(fields, baseRecords, RawRecordPayloadRepository.GetByFormKey(game, RecordTypeCatalog.Terminal.RecordID, formKey));
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
                CreateChildField("MutagenObjectType", records, record => FindRecordComponent(components, record.ModKey, currentIndex)?.MutagenObjectType ?? string.Empty)
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

    private static void AddNPCSupplementalFields(ICollection<RecordComparisonFieldDTO> fields, IReadOnlyList<NPCDTO> records)
    {
        fields.Add(CreateField("Template", records, record => record.Template ?? string.Empty));
        fields.Add(CreateField("DefaultTemplate", records, record => record.DefaultTemplate ?? string.Empty));
        fields.Add(CreateField("TemplateActors", records, record => record.TemplateActors ?? string.Empty));
        fields.Add(CreateField("WornArmor", records, record => record.WornArmor ?? string.Empty));
        fields.Add(CreateField("FaceMorph", records, record => record.FaceMorph ?? string.Empty));
        fields.Add(CreateField("FaceParts", records, record => record.FaceParts ?? string.Empty));
        fields.Add(CreateField("HeadParts", records, record => record.HeadParts ?? string.Empty));
        fields.Add(CreateField("HeadTexture", records, record => record.HeadTexture ?? string.Empty));
        fields.Add(CreateField("SleepingOutfit", records, record => record.SleepingOutfit ?? string.Empty));
        fields.Add(CreateField("TintLayers", records, record => record.TintLayers ?? string.Empty));
        fields.Add(CreateField("Tints", records, record => record.Tints ?? string.Empty));
        fields.Add(CreateField("SpaceOutfit", records, record => record.SpaceOutfit ?? string.Empty));
        fields.Add(CreateField("BodyMorphRegionValues", records, record => record.BodyMorphRegionValues ?? string.Empty));
        fields.Add(CreateField("ObjectTemplates", records, record => record.ObjectTemplates ?? string.Empty));
        fields.Add(CreateField("AIData", records, record => record.AIData ?? string.Empty));
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
