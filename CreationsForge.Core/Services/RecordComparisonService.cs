using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Repositories.Interfaces;
using CreationsForge.Core.Services.Interfaces;

namespace CreationsForge.Core.Services;

public class RecordComparisonService : IRecordComparisonService
{
    private const string UnparseableReflectionDataLabel = "[UNPARSEABLE REFLECTION DATA]";
    private readonly IFormListRepository FormListRepository;
    private readonly IGameSettingRepository GameSettingRepository;
    private readonly IGlobalRepository GlobalRepository;
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

    public RecordComparisonService(
        IFormListRepository formListRepository,
        IGameSettingRepository gameSettingRepository,
        IGlobalRepository globalRepository,
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
        IRawRecordPayloadRepository rawRecordPayloadRepository)
    {
        FormListRepository = formListRepository;
        GameSettingRepository = gameSettingRepository;
        GlobalRepository = globalRepository;
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
        var fields = CreateCommonFields(records.Cast<RecordDTO>().ToList());
        fields.Add(CreateField("SettingType", records, record => record.SettingType ?? string.Empty));
        fields.Add(CreateField("Data", records, record => record.Data ?? string.Empty));

        return CreateComparison(RecordTypeCatalog.GameSetting.RecordID, formKey, records.Cast<RecordDTO>().ToList(), fields);
    }

    private RecordComparisonDTO CreateGlobalComparison(SupportedGame game, FormKeyDTO formKey)
    {
        var records = GlobalRepository.GetByFormKey(game, formKey);
        var fields = CreateCommonFields(records.Cast<RecordDTO>().ToList());
        fields.Add(CreateField("Data", records, record => record.Data?.ToString() ?? string.Empty));

        return CreateComparison(RecordTypeCatalog.Global.RecordID, formKey, records.Cast<RecordDTO>().ToList(), fields);
    }

    private RecordComparisonDTO CreateMiscObjectComparison(SupportedGame game, FormKeyDTO formKey)
    {
        var records = MiscObjectRepository.GetByFormKey(game, formKey);
        var fields = CreateCommonFields(records.Cast<RecordDTO>().ToList());
        fields.Add(CreateField("Name", records, record => record.Name ?? string.Empty));
        fields.Add(CreateField("ShortName", records, record => record.ShortName ?? string.Empty));
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
        var fields = CreateCommonFields(records.Cast<RecordDTO>().ToList());
        fields.Add(CreateField("Name", records, record => record.Name ?? string.Empty));
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
        var fields = CreateCommonFields(records.Cast<RecordDTO>().ToList());
        fields.Add(CreateField("Name", records, record => record.Name ?? string.Empty));
        fields.Add(CreateField("Abbreviation", records, record => record.Abbreviation ?? string.Empty));
        fields.Add(CreateField("ContextNotes", records, record => record.ContextNotes ?? string.Empty));
        fields.Add(CreateField("DefaultValue", records, record => record.DefaultValue?.ToString() ?? string.Empty));
        fields.Add(CreateField("Flags", records, record => record.Flags ?? string.Empty));
        fields.Add(CreateField("Type", records, record => record.Type ?? string.Empty));
        fields.Add(CreateField("Min", records, record => record.Min?.ToString() ?? string.Empty));
        fields.Add(CreateField("Max", records, record => record.Max?.ToString() ?? string.Empty));

        return CreateComparison(RecordTypeCatalog.ActorValueInformation.RecordID, formKey, records.Cast<RecordDTO>().ToList(), fields);
    }

    private RecordComparisonDTO CreateNPCComparison(SupportedGame game, FormKeyDTO formKey)
    {
        var records = NPCRepository.GetByFormKey(game, formKey);
        var fields = CreateCommonFields(records.Cast<RecordDTO>().ToList());
        fields.Add(CreateField("Name", records, record => record.Name ?? string.Empty));
        fields.Add(CreateField("ShortName", records, record => record.ShortName ?? string.Empty));
        fields.Add(CreateField("LongName", records, record => record.LongName ?? string.Empty));
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
        var fields = CreateCommonFields(records.Cast<RecordDTO>().ToList());
        fields.Add(CreateField("Name", records, record => record.Name ?? string.Empty));
        fields.Add(CreateField("Description", records, record => record.Description ?? string.Empty));
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
        var fields = CreateCommonFields(records.Cast<RecordDTO>().ToList());
        fields.Add(CreateField("Name", records, record => record.Name ?? string.Empty));
        fields.Add(CreateField("Description", records, record => record.Description ?? string.Empty));
        fields.Add(CreateField("Flags", records, record => record.Flags));
        fields.Add(CreateField("SkillGroup", records, record => record.SkillGroup ?? string.Empty));
        fields.Add(CreateField("CrewAssignment", records, record => record.CrewAssignment ?? string.Empty));
        fields.Add(CreateField("PerkIcon", records, record => record.PerkIcon ?? string.Empty));
        fields.Add(CreateField("Category", records, record => record.Category ?? string.Empty));
        fields.Add(CreateField("RestrictionFormKey", records, record => FormatFormKey(record.RestrictionFormKey)));
        fields.Add(CreateField("TrainingFormKey", records, record => FormatFormKey(record.TrainingFormKey)));
        fields.Add(CreateField("MajorFlags", records, record => record.MajorFlags ?? string.Empty));
        AddPerkRankGroups(fields, records);
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
        var baseRecords = records.Cast<RecordDTO>().ToList();
        var fields = CreateCommonFields(baseRecords);
        fields.Add(CreateField("Version2", records, record => record.Version2?.ToString() ?? string.Empty));
        fields.Add(CreateField("ObjectBoundsFirst", records, record => record.ObjectBoundsFirst ?? string.Empty));
        fields.Add(CreateField("ObjectBoundsSecond", records, record => record.ObjectBoundsSecond ?? string.Empty));
        fields.Add(CreateField("InventoryTransformFormKey", records, record => FormatFormKey(record.InventoryTransformFormKey)));
        fields.Add(CreateField("Xalg", records, record => record.Xalg?.ToString() ?? string.Empty));
        fields.Add(CreateField("Name", records, record => record.Name ?? string.Empty));
        fields.Add(CreateField("Text", records, record => record.Text ?? string.Empty));
        fields.Add(CreateField("Value", records, record => record.Value?.ToString() ?? string.Empty));
        fields.Add(CreateField("Weight", records, record => record.Weight?.ToString() ?? string.Empty));
        fields.Add(CreateField("Flags", records, record => record.Flags ?? string.Empty));
        fields.Add(CreateField("TeachesType", records, record => record.TeachesType ?? string.Empty));
        fields.Add(CreateField("TeachesRawContent", records, record => record.TeachesRawContent ?? string.Empty));
        fields.Add(CreateField("DataSlateType", records, record => record.DataSlateType ?? string.Empty));
        fields.Add(CreateField("Description", records, record => record.Description ?? string.Empty));
        fields.Add(CreateField("DataSlateHeaderLeft", records, record => record.DataSlateHeaderLeft ?? string.Empty));
        fields.Add(CreateField("DataSlateHeaderRight", records, record => record.DataSlateHeaderRight ?? string.Empty));
        AddKeywordGroup(fields, baseRecords, RecordKeywordRepository.GetByFormKey(game, RecordTypeCatalog.Book.RecordID, formKey));
        AddModelGroups(fields, baseRecords, ModelRepository.GetByFormKey(game, RecordTypeCatalog.Book.RecordID, formKey));
        AddSoundGroups(fields, baseRecords, RecordSoundRepository.GetByFormKey(game, RecordTypeCatalog.Book.RecordID, formKey));
        AddScriptingAdapterGroups(fields, baseRecords, ScriptingAdapterRepository.GetByFormKey(game, RecordTypeCatalog.Book.RecordID, formKey));
        AddRawPayloadGroups(fields, baseRecords, RawRecordPayloadRepository.GetByFormKey(game, RecordTypeCatalog.Book.RecordID, formKey));

        return CreateComparison(RecordTypeCatalog.Book.RecordID, formKey, baseRecords, fields);
    }

    private RecordComparisonDTO CreateDoorComparison(SupportedGame game, FormKeyDTO formKey)
    {
        var records = DoorRepository.GetByFormKey(game, formKey);
        var baseRecords = records.Cast<RecordDTO>().ToList();
        var fields = CreateCommonFields(baseRecords);
        fields.Add(CreateField("Version2", records, record => record.Version2?.ToString() ?? string.Empty));
        fields.Add(CreateField("ObjectBoundsFirst", records, record => record.ObjectBoundsFirst ?? string.Empty));
        fields.Add(CreateField("ObjectBoundsSecond", records, record => record.ObjectBoundsSecond ?? string.Empty));
        fields.Add(CreateField("Name", records, record => record.Name ?? string.Empty));
        fields.Add(CreateField("Flags", records, record => record.Flags ?? string.Empty));
        fields.Add(CreateField("NativeTerminalFormKey", records, record => FormatFormKey(record.NativeTerminalFormKey)));
        fields.Add(CreateField("SoundLevel", records, record => record.SoundLevel ?? string.Empty));
        fields.Add(CreateField("FacingAxisOverride", records, record => record.FacingAxisOverride ?? string.Empty));
        AddKeywordGroup(fields, baseRecords, RecordKeywordRepository.GetByFormKey(game, RecordTypeCatalog.Door.RecordID, formKey));
        AddModelGroups(fields, baseRecords, ModelRepository.GetByFormKey(game, RecordTypeCatalog.Door.RecordID, formKey));
        AddSoundGroups(fields, baseRecords, RecordSoundRepository.GetByFormKey(game, RecordTypeCatalog.Door.RecordID, formKey));
        AddRawPayloadGroups(fields, baseRecords, RawRecordPayloadRepository.GetByFormKey(game, RecordTypeCatalog.Door.RecordID, formKey));

        return CreateComparison(RecordTypeCatalog.Door.RecordID, formKey, baseRecords, fields);
    }

    private RecordComparisonDTO CreateContainerComparison(SupportedGame game, FormKeyDTO formKey)
    {
        var records = ContainerRepository.GetByFormKey(game, formKey);
        var baseRecords = records.Cast<RecordDTO>().ToList();
        var fields = CreateCommonFields(baseRecords);
        fields.Add(CreateField("Version2", records, record => record.Version2?.ToString() ?? string.Empty));
        fields.Add(CreateField("ObjectBoundsFirst", records, record => record.ObjectBoundsFirst ?? string.Empty));
        fields.Add(CreateField("ObjectBoundsSecond", records, record => record.ObjectBoundsSecond ?? string.Empty));
        fields.Add(CreateField("Name", records, record => record.Name ?? string.Empty));
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
        var baseRecords = records.Cast<RecordDTO>().ToList();
        var fields = CreateCommonFields(baseRecords);
        fields.Add(CreateField("Version2", records, record => record.Version2?.ToString() ?? string.Empty));
        fields.Add(CreateField("Description", records, record => record.Description ?? string.Empty));
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
        AddConditionFormConditionGroups(fields, records);
        AddRawPayloadGroups(fields, baseRecords, RawRecordPayloadRepository.GetByFormKey(game, RecordTypeCatalog.ConditionForm.RecordID, formKey));

        return CreateComparison(RecordTypeCatalog.ConditionForm.RecordID, formKey, baseRecords, fields);
    }

    private RecordComparisonDTO CreateTerminalComparison(SupportedGame game, FormKeyDTO formKey)
    {
        var records = TerminalRepository.GetByFormKey(game, formKey);
        var baseRecords = records.Cast<RecordDTO>().ToList();
        var fields = CreateCommonFields(baseRecords);
        fields.Add(CreateField("Version2", records, record => record.Version2?.ToString() ?? string.Empty));
        fields.Add(CreateField("ObjectBoundsFirst", records, record => record.ObjectBoundsFirst ?? string.Empty));
        fields.Add(CreateField("ObjectBoundsSecond", records, record => record.ObjectBoundsSecond ?? string.Empty));
        fields.Add(CreateField("MenuFormKey", records, record => FormatFormKey(record.MenuFormKey)));
        fields.Add(CreateField("Background", records, record => record.Background ?? string.Empty));
        fields.Add(CreateField("Name", records, record => record.Name ?? string.Empty));
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
        IReadOnlyList<PerkDTO> records)
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
                CreateField("Description", records, record => FindPerkRank(record, currentRankIndex)?.Description ?? string.Empty),
                CreateField("UnknownStaticFormKey", records, record => FormatFormKey(FindPerkRank(record, currentRankIndex)?.UnknownStaticFormKey)),
                CreateField("ConditionCount", records, record => FindPerkRank(record, currentRankIndex)?.ConditionCount.ToString() ?? string.Empty),
                CreateField("ActivityCount", records, record => FindPerkRank(record, currentRankIndex)?.ActivityCount.ToString() ?? string.Empty)
            }
                .Where(HasVisibleValue)
                .ToList();
            AddPerkRankEffectGroups(rankChildren, records, currentRankIndex);
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
                CreateField("ButtonLabel", records, record => FindPerkRankEffect(record, rankIndex, currentEffectIndex)?.ButtonLabel ?? string.Empty),
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

    private static void AddConditionFormConditionGroups(
        IList<RecordComparisonFieldDTO> fields,
        IReadOnlyList<ConditionFormDTO> records)
    {
        var conditionIndexes = records
            .SelectMany(record => record.Conditions)
            .Select(condition => condition.ConditionIndex)
            .Distinct()
            .Order()
            .ToList();
        if (conditionIndexes.Count == 0)
        {
            return;
        }

        var conditionFields = new List<RecordComparisonFieldDTO>();
        foreach (var conditionIndex in conditionIndexes)
        {
            var currentIndex = conditionIndex;
            var conditionChildren = new List<RecordComparisonFieldDTO>
            {
                CreateField("MutagenObjectType", records, record => FindCondition(record, currentIndex)?.MutagenObjectType ?? string.Empty),
                CreateField("DataMutagenObjectType", records, record => FindCondition(record, currentIndex)?.DataMutagenObjectType ?? string.Empty),
                CreateField("CompareOperator", records, record => FindCondition(record, currentIndex)?.CompareOperator ?? string.Empty),
                CreateField("ComparisonValue", records, record => FindCondition(record, currentIndex)?.ComparisonValue ?? string.Empty),
                CreateField("ComparisonValueFormKey", records, record => FormatFormKey(FindCondition(record, currentIndex)?.ComparisonValueFormKey))
            }
                .Where(HasVisibleValue)
                .ToList();
            AddConditionFormParameterGroups(conditionChildren, records, currentIndex);
            if (conditionChildren.Count > 0)
            {
                conditionFields.Add(CreateGroupField($"Condition [{conditionIndex}]", records.Cast<RecordDTO>().ToList(), conditionChildren));
            }
        }

        if (conditionFields.Count > 0)
        {
            fields.Add(CreateGroupField("Conditions", records.Cast<RecordDTO>().ToList(), conditionFields));
        }
    }

    private static void AddConditionFormParameterGroups(
        IList<RecordComparisonFieldDTO> fields,
        IReadOnlyList<ConditionFormDTO> records,
        int conditionIndex)
    {
        var parameterNames = records
            .SelectMany(record => FindCondition(record, conditionIndex)?.Parameters ?? [])
            .Select(parameter => parameter.ParameterName)
            .Distinct(StringComparer.Ordinal)
            .Order(StringComparer.Ordinal)
            .ToList();
        if (parameterNames.Count == 0)
        {
            return;
        }

        var parameterFields = new List<RecordComparisonFieldDTO>();
        foreach (var parameterName in parameterNames)
        {
            var currentName = parameterName;
            var parameterChildren = new List<RecordComparisonFieldDTO>
            {
                CreateField("Value", records, record => FindConditionParameter(record, conditionIndex, currentName)?.ParameterValue ?? string.Empty),
                CreateField("FormKey", records, record => FormatFormKey(FindConditionParameter(record, conditionIndex, currentName)?.ParameterFormKey))
            }
                .Where(HasVisibleValue)
                .ToList();
            if (parameterChildren.Count > 0)
            {
                parameterFields.Add(CreateGroupField(parameterName, records.Cast<RecordDTO>().ToList(), parameterChildren));
            }
        }

        if (parameterFields.Count > 0)
        {
            fields.Add(CreateGroupField("Parameters", records.Cast<RecordDTO>().ToList(), parameterFields));
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

    private static RawRecordPayloadDTO? FindRawPayload(IReadOnlyList<RawRecordPayloadDTO> payloads, ModKeyDTO modKey, RawPayloadKey payloadKey)
    {
        return payloads.FirstOrDefault(payload => IsSameModKey(payload.ModKey, modKey) &&
            string.Equals(payload.PayloadSlot, payloadKey.Slot, StringComparison.Ordinal) &&
            payload.PayloadIndex == payloadKey.Index);
    }

    private static ConditionFormConditionDTO? FindCondition(ConditionFormDTO record, int conditionIndex)
    {
        return record.Conditions.FirstOrDefault(condition => condition.ConditionIndex == conditionIndex);
    }

    private static ConditionFormConditionParameterDTO? FindConditionParameter(ConditionFormDTO record, int conditionIndex, string parameterName)
    {
        return FindCondition(record, conditionIndex)?.Parameters.FirstOrDefault(parameter => string.Equals(parameter.ParameterName, parameterName, StringComparison.Ordinal));
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

    private static bool IsSameModKey(ModKeyDTO first, ModKeyDTO second)
    {
        return first.Type == second.Type &&
            string.Equals(first.Name, second.Name, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(first.FileName, second.FileName, StringComparison.OrdinalIgnoreCase);
    }

    private sealed record ModelKey(string Slot, string Gender);

    private sealed record SoundKey(string Slot, int Index);

    private sealed record RawPayloadKey(string Slot, int Index);

}
