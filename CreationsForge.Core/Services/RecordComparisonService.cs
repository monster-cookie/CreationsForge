using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Records.Interfaces;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services.Interfaces;
using CreationsForge.Core.Utilities;

namespace CreationsForge.Core.Services;

public class RecordComparisonService : IRecordComparisonService
{
    private const string UnparseableReflectionDataLabel = "[UNPARSEABLE REFLECTION DATA]";
    private readonly IFormListRepository FormListRepository;
    private readonly IGameSettingRepository GameSettingRepository;
    private readonly IGlobalRepository GlobalRepository;
    private readonly IClassRepository ClassRepository;
    private readonly IFactionRepository FactionRepository;
    private readonly IMiscObjectRepository MiscObjectRepository;
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
    private readonly IRecordKeywordRepository RecordKeywordRepository;
    private readonly IRecordSoundRepository RecordSoundRepository;
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
        IMiscObjectRepository miscObjectRepository,
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
        IRecordKeywordRepository recordKeywordRepository,
        IRecordSoundRepository recordSoundRepository,
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
        MiscObjectRepository = miscObjectRepository;
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
        RecordKeywordRepository = recordKeywordRepository;
        RecordSoundRepository = recordSoundRepository;
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

        if (recordType == RecordTypeCatalog.MiscObject.RecordID)
        {
            return CreateMiscObjectComparison(game, formKey);
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
        fields.Add(CreateField("AddToListFormKey", records, record => FormatFormKey(record.AddToListFormKey)));
        for (var itemIndex = 0; itemIndex < maxItemCount; itemIndex++)
        {
            var currentIndex = itemIndex;
            fields.Add(CreateField($"Items[{itemIndex}]", records, record => FormatFormKey(record.Items.FirstOrDefault(item => item.ItemIndex == currentIndex)?.ItemFormKey)));
        }

        return CreateComparison(RecordTypeCatalog.FormList.RecordID, formKey, records.Cast<RecordDTO>().ToList(), fields);
    }

    private RecordComparisonDTO CreateGameSettingComparison(SupportedGame game, FormKeyDTO formKey)
    {
        var records = GameSettingRepository.GetByFormKey(game, formKey);
        var localizedStrings = RecordLocalizedStringRepository.GetByFormKey(game, RecordTypeCatalog.GameSetting.RecordID, formKey);
        var recordTextLanguage = GameSelectionService.GetRecordTextLanguage();
        var fields = CreateCommonFields(records.Cast<RecordDTO>().ToList());
        fields.Add(CreateField("SettingType", records, record => record.SettingType ?? string.Empty));
        fields.Add(CreateField("Data", records, record => GetLocalizedDisplayValue(localizedStrings, record, "Data", recordTextLanguage, record.Data)));

        return CreateComparison(RecordTypeCatalog.GameSetting.RecordID, formKey, records.Cast<RecordDTO>().ToList(), fields);
    }

    private RecordComparisonDTO CreateGlobalComparison(SupportedGame game, FormKeyDTO formKey)
    {
        var records = GlobalRepository.GetByFormKey(game, formKey);
        var fields = CreateCommonFields(records.Cast<RecordDTO>().ToList());
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
        fields.Add(CreateField("KeywordFormKey", records, record => FormatFormKey(record.KeywordFormKey)));
        fields.Add(CreateField("HerdFormKey", records, record => FormatFormKey(record.HerdFormKey)));
        fields.Add(CreateField("VoiceTypeFormKey", records, record => FormatFormKey(record.VoiceTypeFormKey)));
        fields.Add(CreateField("SharedCrimeFactionListFormKey", records, record => FormatFormKey(record.SharedCrimeFactionListFormKey)));
        fields.Add(CreateField("VendorBuySellListFormKey", records, record => FormatFormKey(record.VendorBuySellListFormKey)));
        fields.Add(CreateField("MerchantContainerFormKey", records, record => FormatFormKey(record.MerchantContainerFormKey)));
        fields.Add(CreateField("ExteriorJailMarkerFormKey", records, record => FormatFormKey(record.ExteriorJailMarkerFormKey)));
        fields.Add(CreateField("FollowerWaitMarkerFormKey", records, record => FormatFormKey(record.FollowerWaitMarkerFormKey)));
        fields.Add(CreateField("StolenGoodsContainerFormKey", records, record => FormatFormKey(record.StolenGoodsContainerFormKey)));
        fields.Add(CreateField("PlayerInventoryContainerFormKey", records, record => FormatFormKey(record.PlayerInventoryContainerFormKey)));
        fields.Add(CreateField("JailOutfitFormKey", records, record => FormatFormKey(record.JailOutfitFormKey)));
        fields.Add(CreateField("CrimeArrest", records, record => record.CrimeArrest?.ToString() ?? string.Empty));
        fields.Add(CreateField("CrimeAttackOnSight", records, record => record.CrimeAttackOnSight?.ToString() ?? string.Empty));
        fields.Add(CreateField("CrimeMurder", records, record => record.CrimeMurder?.ToString() ?? string.Empty));
        fields.Add(CreateField("CrimeAssault", records, record => record.CrimeAssault?.ToString() ?? string.Empty));
        fields.Add(CreateField("CrimeTrespass", records, record => record.CrimeTrespass?.ToString() ?? string.Empty));
        fields.Add(CreateField("CrimePickpocket", records, record => record.CrimePickpocket?.ToString() ?? string.Empty));
        fields.Add(CreateField("CrimeSteal", records, record => record.CrimeSteal?.ToString() ?? string.Empty));
        fields.Add(CreateField("CrimeStealMult", records, record => record.CrimeStealMult?.ToString() ?? string.Empty));
        fields.Add(CreateField("CrimeEscape", records, record => record.CrimeEscape?.ToString() ?? string.Empty));
        fields.Add(CreateField("CrimeWerewolf", records, record => record.CrimeWerewolf?.ToString() ?? string.Empty));
        fields.Add(CreateField("CrimeUnknown", records, record => record.CrimeUnknown?.ToString() ?? string.Empty));
        fields.Add(CreateField("VendorStartHour", records, record => record.VendorStartHour?.ToString() ?? string.Empty));
        fields.Add(CreateField("VendorEndHour", records, record => record.VendorEndHour?.ToString() ?? string.Empty));
        fields.Add(CreateField("VendorRadius", records, record => record.VendorRadius?.ToString() ?? string.Empty));
        fields.Add(CreateField("VendorBuysStolenItems", records, record => record.VendorBuysStolenItems?.ToString() ?? string.Empty));
        fields.Add(CreateField("VendorBuysNonStolenItems", records, record => record.VendorBuysNonStolenItems?.ToString() ?? string.Empty));
        fields.Add(CreateField("VendorBuySellEverythingNotInList", records, record => record.VendorBuySellEverythingNotInList?.ToString() ?? string.Empty));
        fields.Add(CreateField("VendorLocationMutagenObjectType", records, record => record.VendorLocationMutagenObjectType ?? string.Empty));
        fields.Add(CreateField("VendorLocationType", records, record => record.VendorLocationType ?? string.Empty));
        fields.Add(CreateField("VendorLocationLinkFormKey", records, record => FormatFormKey(record.VendorLocationLinkFormKey)));
        AddFactionRelationGroups(fields, records);
        AddFactionRankGroups(fields, records, localizedStrings, recordTextLanguage);
        AddConditionRuleGroups(fields, records.Cast<RecordDTO>().ToList(), records.Cast<IHasConditionsRecordDTO>().ToList());
        AddRecordComponentGroups(fields, records.Cast<RecordDTO>().ToList(), records.SelectMany(record => record.Components).ToList());
        AddKeywordGroup(fields, records.Cast<RecordDTO>().ToList(), RecordKeywordRepository.GetByFormKey(game, RecordTypeCatalog.Faction.RecordID, formKey));

        return CreateComparison(RecordTypeCatalog.Faction.RecordID, formKey, records.Cast<RecordDTO>().ToList(), fields);
    }

    private RecordComparisonDTO CreateMiscObjectComparison(SupportedGame game, FormKeyDTO formKey)
    {
        var records = MiscObjectRepository.GetByFormKey(game, formKey);
        var localizedStrings = RecordLocalizedStringRepository.GetByFormKey(game, RecordTypeCatalog.MiscObject.RecordID, formKey);
        var recordTextLanguage = GameSelectionService.GetRecordTextLanguage();
        var fields = CreateCommonFields(records.Cast<RecordDTO>().ToList());
        fields.Add(CreateField("Name", records, record => GetTranslatedDisplayValue(localizedStrings, record, "Name", recordTextLanguage, record.Name)));
        fields.Add(CreateField("ShortName", records, record => GetTranslatedDisplayValue(localizedStrings, record, "ShortName", recordTextLanguage, record.ShortName)));
        fields.Add(CreateField("Value", records, record => record.Value?.ToString() ?? string.Empty));
        fields.Add(CreateField("Weight", records, record => record.Weight?.ToString() ?? string.Empty));
        fields.Add(CreateField("DirtinessScale", records, record => record.DirtinessScale?.ToString() ?? string.Empty));
        fields.Add(CreateField("FeaturedItemMessageFormKey", records, record => FormatFormKey(record.FeaturedItemMessageFormKey)));
        fields.Add(CreateField("Flag", records, record => record.Flag ?? string.Empty));
        AddKeywordGroup(fields, records.Cast<RecordDTO>().ToList(), RecordKeywordRepository.GetByFormKey(game, RecordTypeCatalog.MiscObject.RecordID, formKey));
        AddModelGroups(fields, records.Cast<RecordDTO>().ToList(), ModelRepository.GetByFormKey(game, RecordTypeCatalog.MiscObject.RecordID, formKey));
        AddSoundGroups(fields, records.Cast<RecordDTO>().ToList(), RecordSoundRepository.GetByFormKey(game, RecordTypeCatalog.MiscObject.RecordID, formKey));
        AddScriptingAdapterGroups(fields, records.Cast<RecordDTO>().ToList(), ScriptingAdapterRepository.GetByFormKey(game, RecordTypeCatalog.MiscObject.RecordID, formKey));

        return CreateComparison(RecordTypeCatalog.MiscObject.RecordID, formKey, records.Cast<RecordDTO>().ToList(), fields);
    }

    private RecordComparisonDTO CreateKeywordComparison(SupportedGame game, FormKeyDTO formKey)
    {
        var records = KeywordRepository.GetByFormKey(game, formKey);
        var localizedStrings = RecordLocalizedStringRepository.GetByFormKey(game, RecordTypeCatalog.Keyword.RecordID, formKey);
        var recordTextLanguage = GameSelectionService.GetRecordTextLanguage();
        var fields = CreateCommonFields(records.Cast<RecordDTO>().ToList());
        fields.Add(CreateField("Name", records, record => GetTranslatedDisplayValue(localizedStrings, record, "Name", recordTextLanguage, record.Name)));
        fields.Add(CreateField("Color", records, record => record.Color));
        fields.Add(CreateField("Type", records, record => record.Type));
        fields.Add(CreateField("Notes", records, record => record.Notes ?? string.Empty));
        fields.Add(CreateField("FlashLinkageName", records, record => record.FlashLinkageName ?? string.Empty));
        fields.Add(CreateField("AttractionRuleFormKey", records, record => FormatFormKey(record.AttractionRuleFormKey)));

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
        fields.Add(CreateField("CNAM", records, record => record.Cnam ?? string.Empty));
        fields.Add(CreateField("Skill.ImproveMult", records, record => record.SkillImproveMult?.ToString() ?? string.Empty));
        fields.Add(CreateField("Skill.ImproveOffset", records, record => record.SkillImproveOffset?.ToString() ?? string.Empty));
        fields.Add(CreateField("Skill.UseMult", records, record => record.SkillUseMult?.ToString() ?? string.Empty));
        fields.Add(CreateField("ContextNotes", records, record => record.ContextNotes ?? string.Empty));
        fields.Add(CreateField("DefaultValue", records, record => record.DefaultValue?.ToString() ?? string.Empty));
        fields.Add(CreateField("Flags", records, record => record.Flags ?? string.Empty));
        fields.Add(CreateField("Type", records, record => record.Type ?? string.Empty));
        fields.Add(CreateField("Min", records, record => record.Min?.ToString() ?? string.Empty));
        fields.Add(CreateField("Max", records, record => record.Max?.ToString() ?? string.Empty));
        AddActorValueInformationLayoutGroups(fields, records);
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
        AddKeywordGroup(fields, records.Cast<RecordDTO>().ToList(), RecordKeywordRepository.GetByFormKey(game, RecordTypeCatalog.NPC.RecordID, formKey));

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
        AddKeywordGroup(fields, records.Cast<RecordDTO>().ToList(), RecordKeywordRepository.GetByFormKey(game, RecordTypeCatalog.MagicEffect.RecordID, formKey));
        AddSoundGroups(fields, records.Cast<RecordDTO>().ToList(), RecordSoundRepository.GetByFormKey(game, RecordTypeCatalog.MagicEffect.RecordID, formKey));
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
        fields.Add(CreateField("MajorFlags", records, record => record.MajorFlags ?? string.Empty));
        AddPerkRankGroups(fields, records, localizedStrings, recordTextLanguage);
        AddPerkBackgroundSkillGroup(fields, records);
        AddScriptingAdapterGroups(fields, records.Cast<RecordDTO>().ToList(), ScriptingAdapterRepository.GetByFormKey(game, RecordTypeCatalog.Perk.RecordID, formKey));

        return CreateComparison(RecordTypeCatalog.Perk.RecordID, formKey, records.Cast<RecordDTO>().ToList(), fields);
    }

    private RecordComparisonDTO CreateStaticComparison(SupportedGame game, FormKeyDTO formKey)
    {
        var records = StaticRepository.GetByFormKey(game, formKey);
        var fields = CreateCommonFields(records.Cast<RecordDTO>().ToList());
        fields.Add(CreateField("Version2", records, record => record.Version2?.ToString() ?? string.Empty));
        fields.Add(CreateField("ObjectBoundsFirst", records, record => record.ObjectBoundsFirst ?? string.Empty));
        fields.Add(CreateField("ObjectBoundsSecond", records, record => record.ObjectBoundsSecond ?? string.Empty));
        fields.Add(CreateField("MaxAngle", records, record => record.MaxAngle?.ToString() ?? string.Empty));
        fields.Add(CreateField("UnknownDNAMFloat", records, record => record.UnknownDNAMFloat?.ToString() ?? string.Empty));
        fields.Add(CreateField("LeafAmplitude", records, record => record.LeafAmplitude?.ToString() ?? string.Empty));
        fields.Add(CreateField("LeafFrequency", records, record => record.LeafFrequency?.ToString() ?? string.Empty));
        fields.Add(CreateField("Unused", records, record => record.Unused ?? string.Empty));
        fields.Add(CreateField("DNAMDataTypeState", records, record => record.DNAMDataTypeState ?? string.Empty));
        AddKeywordGroup(fields, records.Cast<RecordDTO>().ToList(), RecordKeywordRepository.GetByFormKey(game, RecordTypeCatalog.Static.RecordID, formKey));
        AddModelGroups(fields, records.Cast<RecordDTO>().ToList(), ModelRepository.GetByFormKey(game, RecordTypeCatalog.Static.RecordID, formKey));
        AddRawPayloadGroups(fields, records.Cast<RecordDTO>().ToList(), RawRecordPayloadRepository.GetByFormKey(game, RecordTypeCatalog.Static.RecordID, formKey));

        return CreateComparison(RecordTypeCatalog.Static.RecordID, formKey, records.Cast<RecordDTO>().ToList(), fields);
    }

    private RecordComparisonDTO CreateBookComparison(SupportedGame game, FormKeyDTO formKey)
    {
        var records = BookRepository.GetByFormKey(game, formKey);
        var localizedStrings = RecordLocalizedStringRepository.GetByFormKey(game, RecordTypeCatalog.Book.RecordID, formKey);
        var recordTextLanguage = GameSelectionService.GetRecordTextLanguage();
        var baseRecords = records.Cast<RecordDTO>().ToList();
        var fields = CreateCommonFields(baseRecords);
        fields.Add(CreateField("Version2", records, record => record.Version2?.ToString() ?? string.Empty));
        fields.Add(CreateField("ObjectBoundsFirst", records, record => record.ObjectBoundsFirst ?? string.Empty));
        fields.Add(CreateField("ObjectBoundsSecond", records, record => record.ObjectBoundsSecond ?? string.Empty));
        fields.Add(CreateField("InventoryTransformFormKey", records, record => FormatFormKey(record.InventoryTransformFormKey)));
        fields.Add(CreateField("PreviewTransformFormKey", records, record => FormatFormKey(record.PreviewTransformFormKey)));
        fields.Add(CreateField("Xalg", records, record => record.Xalg?.ToString() ?? string.Empty));
        fields.Add(CreateField("Name", records, record => GetTranslatedDisplayValue(localizedStrings, record, "Name", recordTextLanguage, record.Name)));
        fields.Add(CreateField("Text", records, record => GetTranslatedDisplayValue(localizedStrings, record, "Text", recordTextLanguage, record.Text)));
        fields.Add(CreateField("Value", records, record => record.Value?.ToString() ?? string.Empty));
        fields.Add(CreateField("Weight", records, record => record.Weight?.ToString() ?? string.Empty));
        fields.Add(CreateField("Flags", records, record => record.Flags ?? string.Empty));
        fields.Add(CreateField("TeachesType", records, record => record.TeachesType ?? string.Empty));
        fields.Add(CreateField("TeachesRawContent", records, record => record.TeachesRawContent ?? string.Empty));
        fields.Add(CreateField("DataSlateType", records, record => record.DataSlateType ?? string.Empty));
        fields.Add(CreateField("Description", records, record => GetTranslatedDisplayValue(localizedStrings, record, "Description", recordTextLanguage, record.Description)));
        fields.Add(CreateField("DataSlateHeaderLeft", records, record => GetTranslatedDisplayValue(localizedStrings, record, "DataSlateHeaderLeft", recordTextLanguage, record.DataSlateHeaderLeft)));
        fields.Add(CreateField("DataSlateHeaderRight", records, record => GetTranslatedDisplayValue(localizedStrings, record, "DataSlateHeaderRight", recordTextLanguage, record.DataSlateHeaderRight)));
        AddKeywordGroup(fields, baseRecords, RecordKeywordRepository.GetByFormKey(game, RecordTypeCatalog.Book.RecordID, formKey));
        AddModelGroups(fields, baseRecords, ModelRepository.GetByFormKey(game, RecordTypeCatalog.Book.RecordID, formKey));
        AddSoundGroups(fields, baseRecords, RecordSoundRepository.GetByFormKey(game, RecordTypeCatalog.Book.RecordID, formKey));
        AddScriptingAdapterGroups(fields, baseRecords, ScriptingAdapterRepository.GetByFormKey(game, RecordTypeCatalog.Book.RecordID, formKey));
        AddRecordComponentGroups(fields, baseRecords, records.SelectMany(record => record.Components).ToList());
        AddRawPayloadGroups(fields, baseRecords, RawRecordPayloadRepository.GetByFormKey(game, RecordTypeCatalog.Book.RecordID, formKey));

        return CreateComparison(RecordTypeCatalog.Book.RecordID, formKey, baseRecords, fields);
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
        AddKeywordGroup(fields, baseRecords, RecordKeywordRepository.GetByFormKey(game, RecordTypeCatalog.Door.RecordID, formKey));
        AddModelGroups(fields, baseRecords, ModelRepository.GetByFormKey(game, RecordTypeCatalog.Door.RecordID, formKey));
        AddSoundGroups(fields, baseRecords, RecordSoundRepository.GetByFormKey(game, RecordTypeCatalog.Door.RecordID, formKey));
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
        AddContainerItemGroups(fields, records);
        AddKeywordGroup(fields, baseRecords, RecordKeywordRepository.GetByFormKey(game, RecordTypeCatalog.Container.RecordID, formKey));
        AddModelGroups(fields, baseRecords, ModelRepository.GetByFormKey(game, RecordTypeCatalog.Container.RecordID, formKey));
        AddSoundGroups(fields, baseRecords, RecordSoundRepository.GetByFormKey(game, RecordTypeCatalog.Container.RecordID, formKey));
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
        fields.Add(CreateField("MenuSortOrder", records, record => record.MenuSortOrder?.ToString() ?? string.Empty));
        fields.Add(CreateField("LearnMethod", records, record => record.LearnMethod ?? string.Empty));
        fields.Add(CreateField("Flags", records, record => record.Flags ?? string.Empty));
        AddConstructibleObjectComponentGroups(fields, records);
        AddConstructibleObjectCategoryGroups(fields, records);
        AddConstructibleObjectRecipeFilterGroups(fields, records);
        AddConditionRuleGroups(fields, baseRecords, records.Cast<IHasConditionsRecordDTO>().ToList());
        AddScriptingAdapterGroups(fields, baseRecords, ScriptingAdapterRepository.GetByFormKey(game, RecordTypeCatalog.ConstructibleObject.RecordID, formKey));
        AddRawPayloadGroups(fields, baseRecords, RawRecordPayloadRepository.GetByFormKey(game, RecordTypeCatalog.ConstructibleObject.RecordID, formKey));

        return CreateComparison(RecordTypeCatalog.ConstructibleObject.RecordID, formKey, baseRecords, fields);
    }

    private RecordComparisonDTO CreateConditionFormComparison(SupportedGame game, FormKeyDTO formKey)
    {
        var records = ConditionFormRepository.GetByFormKey(game, formKey);
        var baseRecords = records.Cast<RecordDTO>().ToList();
        var fields = CreateCommonFields(baseRecords);
        fields.Add(CreateField("Version2", records, record => record.Version2?.ToString() ?? string.Empty));
        AddConditionRuleGroups(fields, baseRecords, records.Cast<IHasConditionsRecordDTO>().ToList());

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
        fields.Add(CreateField("Name", records, record => GetTranslatedDisplayValue(localizedStrings, record, "Name", recordTextLanguage, record.Name)));
        fields.Add(CreateField("Pnam", records, record => record.Pnam ?? string.Empty));
        fields.Add(CreateField("Fnam", records, record => record.Fnam ?? string.Empty));
        fields.Add(CreateField("Jnam", records, record => record.Jnam ?? string.Empty));
        fields.Add(CreateField("MarkerFlags", records, record => record.MarkerFlags?.ToString() ?? string.Empty));
        fields.Add(CreateField("Gnam", records, record => record.Gnam ?? string.Empty));
        fields.Add(CreateField("WorkbenchData", records, record => record.WorkbenchData ?? string.Empty));
        fields.Add(CreateField("FurnitureTemplateFormKey", records, record => FormatFormKey(record.FurnitureTemplateFormKey)));
        fields.Add(CreateField("MarkerModel", records, record => record.MarkerModel ?? string.Empty));
        AddKeywordGroup(fields, baseRecords, RecordKeywordRepository.GetByFormKey(game, RecordTypeCatalog.Terminal.RecordID, formKey));
        AddModelGroups(fields, baseRecords, ModelRepository.GetByFormKey(game, RecordTypeCatalog.Terminal.RecordID, formKey));
        AddScriptingAdapterGroups(fields, baseRecords, ScriptingAdapterRepository.GetByFormKey(game, RecordTypeCatalog.Terminal.RecordID, formKey));
        AddRawPayloadGroups(fields, baseRecords, RawRecordPayloadRepository.GetByFormKey(game, RecordTypeCatalog.Terminal.RecordID, formKey));
        AddTerminalMarkerParameterGroups(fields, records);

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
        IReadOnlyList<RecordKeywordDTO> keywords)
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
                record => FormatFormKey(FindKeyword(keywords, record.ModKey, currentIndex)?.KeywordFormKey)));
        }

        fields.Add(CreateGroupField("Keywords", records, keywordFields));
    }

    private static void AddSoundGroups(
        IList<RecordComparisonFieldDTO> fields,
        IReadOnlyList<RecordDTO> records,
        IReadOnlyList<RecordSoundDTO> sounds)
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
        string recordTextLanguage)
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

    private static void AddPerkRankEffectGroups(
        IList<RecordComparisonFieldDTO> fields,
        IReadOnlyList<PerkDTO> records,
        IReadOnlyList<LocalizedStringDTO> localizedStrings,
        string recordTextLanguage,
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
            var effectChildren = new List<RecordComparisonFieldDTO>
            {
                CreateField("MutagenObjectType", records, record => FindPerkRankEffect(record, rankIndex, currentEffectIndex)?.MutagenObjectType ?? string.Empty),
                CreateField("Rank", records, record => FindPerkRankEffect(record, rankIndex, currentEffectIndex)?.Rank.ToString() ?? string.Empty),
                CreateField("Priority", records, record => FindPerkRankEffect(record, rankIndex, currentEffectIndex)?.Priority.ToString() ?? string.Empty),
                CreateField("PerkEntryID", records, record => FindPerkRankEffect(record, rankIndex, currentEffectIndex)?.PerkEntryId?.ToString() ?? string.Empty),
                CreateField("Flags", records, record => FindPerkRankEffect(record, rankIndex, currentEffectIndex)?.Flags ?? string.Empty),
                CreateField("ButtonLabel", records, record => GetTranslatedDisplayValue(localizedStrings, record, $"Ranks[{rankIndex}].Effects[{currentEffectIndex}].ButtonLabel", recordTextLanguage, FindPerkRankEffect(record, rankIndex, currentEffectIndex)?.ButtonLabel)),
                CreateField("ConditionCount", records, record => FindPerkRankEffect(record, rankIndex, currentEffectIndex)?.ConditionCount.ToString() ?? string.Empty),
                CreateField("EntryPoint", records, record => FindPerkRankEffect(record, rankIndex, currentEffectIndex)?.EntryPoint ?? string.Empty),
                CreateField("PerkConditionTabCount", records, record => FindPerkRankEffect(record, rankIndex, currentEffectIndex)?.PerkConditionTabCount?.ToString() ?? string.Empty),
                CreateField("Modification", records, record => FindPerkRankEffect(record, rankIndex, currentEffectIndex)?.Modification ?? string.Empty),
                CreateField("Value", records, record => FindPerkRankEffect(record, rankIndex, currentEffectIndex)?.Value?.ToString() ?? string.Empty)
            }
                .Where(HasVisibleValue)
                .ToList();
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
                CreateField("ExitTypes", records, record => record.MarkerParameters.FirstOrDefault(parameter => parameter.ParameterIndex == currentIndex)?.ExitTypes ?? string.Empty)
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
                CreateField("TargetFormKey", records, record => FormatFormKey(record.Relations.FirstOrDefault(relation => relation.RelationIndex == currentIndex)?.TargetFormKey)),
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

    private static void AddFactionRankGroups(IList<RecordComparisonFieldDTO> fields, IReadOnlyList<FactionDTO> records, IReadOnlyList<LocalizedStringDTO> localizedStrings, string recordTextLanguage)
    {
        var rankIndexes = records.SelectMany(record => record.Ranks).Select(rank => rank.RankIndex).Distinct().Order().ToList();
        if (rankIndexes.Count == 0) return;
        var rankFields = new List<RecordComparisonFieldDTO>();
        foreach (var rankIndex in rankIndexes)
        {
            var currentIndex = rankIndex;
            var children = new List<RecordComparisonFieldDTO>
            {
                CreateField("RankNumber", records, record => record.Ranks.FirstOrDefault(rank => rank.RankIndex == currentIndex)?.RankNumber?.ToString() ?? string.Empty),
                CreateField("MaleTitle", records, record => GetTranslatedDisplayValue(localizedStrings, record, $"Ranks[{currentIndex}].MaleTitle", recordTextLanguage, record.Ranks.FirstOrDefault(rank => rank.RankIndex == currentIndex)?.MaleTitle)),
                CreateField("FemaleTitle", records, record => GetTranslatedDisplayValue(localizedStrings, record, $"Ranks[{currentIndex}].FemaleTitle", recordTextLanguage, record.Ranks.FirstOrDefault(rank => rank.RankIndex == currentIndex)?.FemaleTitle))
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
        IReadOnlyList<IHasConditionsRecordDTO> records)
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

    private static void AddActorValueInformationLayoutGroups(
        IList<RecordComparisonFieldDTO> fields,
        IReadOnlyList<ActorValueInformationDTO> records)
    {
        var layoutIndexes = records
            .SelectMany(record => record.LayoutEntries)
            .Select(entry => entry.LayoutIndex)
            .Distinct()
            .Order()
            .ToList();
        if (layoutIndexes.Count == 0)
        {
            return;
        }

        var layoutFields = new List<RecordComparisonFieldDTO>();
        foreach (var layoutIndex in layoutIndexes)
        {
            var currentIndex = layoutIndex;
            var layoutChildren = new List<RecordComparisonFieldDTO>
            {
                CreateField("AssociatedSkill", records, record => FormatFormKey(record.LayoutEntries.FirstOrDefault(entry => entry.LayoutIndex == currentIndex)?.AssociatedSkillFormKey)),
                CreateField("FNAM", records, record => record.LayoutEntries.FirstOrDefault(entry => entry.LayoutIndex == currentIndex)?.Fnam ?? string.Empty),
                CreateField("HorizontalPosition", records, record => record.LayoutEntries.FirstOrDefault(entry => entry.LayoutIndex == currentIndex)?.HorizontalPosition?.ToString() ?? string.Empty),
                CreateField("Index", records, record => record.LayoutEntries.FirstOrDefault(entry => entry.LayoutIndex == currentIndex)?.Index?.ToString() ?? string.Empty),
                CreateField("PerkGridX", records, record => record.LayoutEntries.FirstOrDefault(entry => entry.LayoutIndex == currentIndex)?.PerkGridX?.ToString() ?? string.Empty),
                CreateField("PerkGridY", records, record => record.LayoutEntries.FirstOrDefault(entry => entry.LayoutIndex == currentIndex)?.PerkGridY?.ToString() ?? string.Empty),
                CreateField("VerticalPosition", records, record => record.LayoutEntries.FirstOrDefault(entry => entry.LayoutIndex == currentIndex)?.VerticalPosition?.ToString() ?? string.Empty)
            }
                .Where(HasVisibleValue)
                .ToList();
            if (layoutChildren.Count > 0)
            {
                layoutFields.Add(CreateGroupField($"Layout [{layoutIndex}]", records.Cast<RecordDTO>().ToList(), layoutChildren));
            }
        }

        if (layoutFields.Count > 0)
        {
            fields.Add(CreateGroupField("Layout Entries", records.Cast<RecordDTO>().ToList(), layoutFields));
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
                CreateField("FNAM", records, record => record.PerkTree.FirstOrDefault(entry => entry.PerkTreeIndex == currentIndex)?.Fnam ?? string.Empty)
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

    private static ModelDTO? FindModel(IReadOnlyList<ModelDTO> models, ModKeyDTO modKey, ModelKey modelKey)
    {
        return models.FirstOrDefault(model => IsSameModKey(model.ModKey, modKey) && IsModelKey(model, modelKey));
    }

    private static RecordKeywordDTO? FindKeyword(IReadOnlyList<RecordKeywordDTO> keywords, ModKeyDTO modKey, int keywordIndex)
    {
        return keywords.FirstOrDefault(keyword => IsSameModKey(keyword.ModKey, modKey) && keyword.KeywordIndex == keywordIndex);
    }

    private static RecordSoundDTO? FindSound(IReadOnlyList<RecordSoundDTO> sounds, ModKeyDTO modKey, SoundKey soundKey)
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

    private static ConditionFormConditionDTO? FindConditionRule(IHasConditionsRecordDTO record, ConditionRuleKey conditionKey)
    {
        return record.Conditions.FirstOrDefault(condition => string.Equals(condition.ConditionSlot, conditionKey.Slot, StringComparison.Ordinal) &&
            condition.ConditionIndex == conditionKey.Index);
    }

    private static ConditionFormConditionDTO? FindConditionRule(
        IReadOnlyList<IHasConditionsRecordDTO> records,
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

    private static PerkRankEffectDTO? FindPerkRankEffect(PerkDTO record, int rankIndex, int effectIndex)
    {
        return FindPerkRank(record, rankIndex)?.Effects.FirstOrDefault(effect => effect.EffectIndex == effectIndex);
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
        string recordTextLanguage,
        string? fallback)
    {
        return localizedStrings.FirstOrDefault(localizedString =>
                   IsSameModKey(localizedString.ModKey, record.ModKey) &&
                   string.Equals(localizedString.SourceField, sourceField, StringComparison.Ordinal) &&
                   string.Equals(localizedString.Language, recordTextLanguage, StringComparison.OrdinalIgnoreCase))?.Value ??
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
        string recordTextLanguage,
        TranslatedStringDTO? fallback)
    {
        return GetLocalizedDisplayValue(localizedStrings, record, sourceField, recordTextLanguage, LocalizedStringDTOMapper.GetEnglishText(fallback));
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

    private sealed record ConditionRuleKey(string Slot, int Index);

}
