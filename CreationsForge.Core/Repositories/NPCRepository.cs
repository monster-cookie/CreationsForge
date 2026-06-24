using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Repositories.Interfaces;
using NPoco;

namespace CreationsForge.Core.Repositories;

public class NPCRepository : TypedRecordRepositoryBase, INPCRepository
{
    private readonly IKeywordMappingRepository KeywordMappingRepository;
    private readonly IScriptingAdapterRepository ScriptingAdapterRepository;
    private readonly IRecordLocalizedStringRepository RecordLocalizedStringRepository;
    private readonly ISoundMappingRepository SoundMappingRepository;

    public NPCRepository(
        IDatabase database,
        IRecordInstanceRepository recordInstanceRepository,
        IKeywordMappingRepository keywordMappingRepository,
        IScriptingAdapterRepository scriptingAdapterRepository,
        IRecordLocalizedStringRepository recordLocalizedStringRepository,
        ISoundMappingRepository soundMappingRepository)
        : base(database, recordInstanceRepository)
    {
        KeywordMappingRepository = keywordMappingRepository;
        ScriptingAdapterRepository = scriptingAdapterRepository;
        RecordLocalizedStringRepository = recordLocalizedStringRepository;
        SoundMappingRepository = soundMappingRepository;
    }

    public override string RecordType => RecordTypeCatalog.NPC.RecordID;

    protected override string TableName => RecordTypeCatalog.NPC.TableName;

    public IReadOnlyList<NPCDTO> GetByFormKey(CreationsForge.Core.Enums.SupportedGame game, CreationsForge.Core.DTOs.Plugins.FormKeyDTO formKey)
    {
        var records = FetchByFormKey<NPCRow>(
                game,
                formKey,
                [
                    SelectColumn("Name"),
                    SelectColumn("ShortName"),
                    SelectColumn("LongName"),
                    SelectColumn("Version2"),
                    SelectColumn("VersionControl"),
                    SelectColumn("DispositionBase"),
                    SelectColumn("Aggression"),
                    SelectColumn("Confidence"),
                    SelectColumn("EnergyLevel"),
                    SelectColumn("Responsibility"),
                    SelectColumn("Assistance"),
                    SelectColumn("GearedUpWeapons"),
                    SelectColumn("HeightMin"),
                    SelectColumn("HeightMax"),
                    SelectColumn("SkinToneIndex"),
                    SelectColumn("Pronoun"),
                    SelectColumn("Voice_ModKey_Name", "VoiceModKeyName"),
                    SelectColumn("Voice_ModKey_Type", "VoiceModKeyType"),
                    SelectColumn("Voice_ModKey_FileName", "VoiceModKeyFileName"),
                    SelectColumn("Voice_FormKey_ID", "VoiceFormKeyId"),
                    SelectColumn("Race_ModKey_Name", "RaceModKeyName"),
                    SelectColumn("Race_ModKey_Type", "RaceModKeyType"),
                    SelectColumn("Race_ModKey_FileName", "RaceModKeyFileName"),
                    SelectColumn("Race_FormKey_ID", "RaceFormKeyId"),
                    SelectColumn("CombatOverridePackageList_ModKey_Name", "CombatOverridePackageListModKeyName"),
                    SelectColumn("CombatOverridePackageList_ModKey_Type", "CombatOverridePackageListModKeyType"),
                    SelectColumn("CombatOverridePackageList_ModKey_FileName", "CombatOverridePackageListModKeyFileName"),
                    SelectColumn("CombatOverridePackageList_FormKey_ID", "CombatOverridePackageListFormKeyId"),
                    SelectColumn("CombatStyle_ModKey_Name", "CombatStyleModKeyName"),
                    SelectColumn("CombatStyle_ModKey_Type", "CombatStyleModKeyType"),
                    SelectColumn("CombatStyle_ModKey_FileName", "CombatStyleModKeyFileName"),
                    SelectColumn("CombatStyle_FormKey_ID", "CombatStyleFormKeyId"),
                    SelectColumn("DefaultPackageList_ModKey_Name", "DefaultPackageListModKeyName"),
                    SelectColumn("DefaultPackageList_ModKey_Type", "DefaultPackageListModKeyType"),
                    SelectColumn("DefaultPackageList_ModKey_FileName", "DefaultPackageListModKeyFileName"),
                    SelectColumn("DefaultPackageList_FormKey_ID", "DefaultPackageListFormKeyId"),
                    SelectColumn("CrimeFaction_ModKey_Name", "CrimeFactionModKeyName"),
                    SelectColumn("CrimeFaction_ModKey_Type", "CrimeFactionModKeyType"),
                    SelectColumn("CrimeFaction_ModKey_FileName", "CrimeFactionModKeyFileName"),
                    SelectColumn("CrimeFaction_FormKey_ID", "CrimeFactionFormKeyId"),
                    SelectColumn("Template"),
                    SelectColumn("DefaultTemplate"),
                    SelectColumn("TemplateActors"),
                    SelectColumn("WornArmor"),
                    SelectColumn("FaceMorph"),
                    SelectColumn("FaceParts"),
                    SelectColumn("HeadParts"),
                    SelectColumn("HeadTexture"),
                    SelectColumn("SleepingOutfit"),
                    SelectColumn("TintLayers"),
                    SelectColumn("Tints"),
                    SelectColumn("SpaceOutfit"),
                    SelectColumn("BodyMorphRegionValues"),
                    SelectColumn("ObjectTemplates"),
                    SelectColumn("AIData")
                ])
            .Select(record => ToDTO(record, game))
            .ToList();
        var keywords = KeywordMappingRepository.GetByFormKey(game, RecordTypeCatalog.NPC.RecordID, formKey);
        var scriptingAdapters = ScriptingAdapterRepository.GetByFormKey(game, RecordTypeCatalog.NPC.RecordID, formKey);
        var localizedStrings = RecordLocalizedStringRepository.GetByFormKey(game, RecordTypeCatalog.NPC.RecordID, formKey);
        var sounds = SoundMappingRepository.GetByFormKey(game, RecordTypeCatalog.NPC.RecordID, formKey);
        foreach (var record in records)
        {
            record.Keywords = keywords.Where(keyword => RecordModKeysMatch(keyword.ModKey, record.ModKey)).OrderBy(keyword => keyword.KeywordIndex).ToList();
            record.ScriptingAdapters = scriptingAdapters.Where(adapter => RecordModKeysMatch(adapter.ModKey, record.ModKey)).OrderBy(adapter => adapter.ScriptIndex).ToList();
            record.Sounds = sounds
                .Where(sound => RecordModKeysMatch(sound.ModKey, record.ModKey))
                .OrderBy(sound => sound.SoundSlot)
                .ThenBy(sound => sound.SoundIndex)
                .ToList();
            ApplyLocalizedStrings(record, localizedStrings.Where(localizedString => RecordModKeysMatch(localizedString.ModKey, record.ModKey)).ToList());
        }

        return records;
    }

    public void Save(NPCDTO dto)
    {
        SaveRecordInstance(dto);
        Database.Execute(
            """
            INSERT OR REPLACE INTO NPCs (
                Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                EditorID, FormVersion, MajorRecordFlags, ImportedAtUTC, Name, ShortName, LongName, Version2, VersionControl, DispositionBase, Aggression, Confidence,
                EnergyLevel, Responsibility, Assistance, GearedUpWeapons, HeightMin, HeightMax, SkinToneIndex, Pronoun,
                Voice_ModKey_Name, Voice_ModKey_Type, Voice_ModKey_FileName, Voice_FormKey_ID,
                Race_ModKey_Name, Race_ModKey_Type, Race_ModKey_FileName, Race_FormKey_ID,
                CombatOverridePackageList_ModKey_Name, CombatOverridePackageList_ModKey_Type, CombatOverridePackageList_ModKey_FileName, CombatOverridePackageList_FormKey_ID,
                CombatStyle_ModKey_Name, CombatStyle_ModKey_Type, CombatStyle_ModKey_FileName, CombatStyle_FormKey_ID,
                DefaultPackageList_ModKey_Name, DefaultPackageList_ModKey_Type, DefaultPackageList_ModKey_FileName, DefaultPackageList_FormKey_ID,
                CrimeFaction_ModKey_Name, CrimeFaction_ModKey_Type, CrimeFaction_ModKey_FileName, CrimeFaction_FormKey_ID,
                Template, DefaultTemplate, TemplateActors, WornArmor, FaceMorph, FaceParts, HeadParts, HeadTexture,
                SleepingOutfit, TintLayers, Tints, SpaceOutfit, BodyMorphRegionValues, ObjectTemplates, AIData)
            VALUES (
                @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                @EditorId, @FormVersion, @MajorRecordFlags, @ImportedAtUTC, @Name, @ShortName, @LongName, @Version2, @VersionControl, @DispositionBase, @Aggression, @Confidence,
                @EnergyLevel, @Responsibility, @Assistance, @GearedUpWeapons, @HeightMin, @HeightMax, @SkinToneIndex, @Pronoun,
                @VoiceModKeyName, @VoiceModKeyType, @VoiceModKeyFileName, @VoiceFormKeyId,
                @RaceModKeyName, @RaceModKeyType, @RaceModKeyFileName, @RaceFormKeyId,
                @CombatOverridePackageListModKeyName, @CombatOverridePackageListModKeyType, @CombatOverridePackageListModKeyFileName, @CombatOverridePackageListFormKeyId,
                @CombatStyleModKeyName, @CombatStyleModKeyType, @CombatStyleModKeyFileName, @CombatStyleFormKeyId,
                @DefaultPackageListModKeyName, @DefaultPackageListModKeyType, @DefaultPackageListModKeyFileName, @DefaultPackageListFormKeyId,
                @CrimeFactionModKeyName, @CrimeFactionModKeyType, @CrimeFactionModKeyFileName, @CrimeFactionFormKeyId,
                @Template, @DefaultTemplate, @TemplateActors, @WornArmor, @FaceMorph, @FaceParts, @HeadParts, @HeadTexture,
                @SleepingOutfit, @TintLayers, @Tints, @SpaceOutfit, @BodyMorphRegionValues, @ObjectTemplates, @AIData);
            """,
            new
            {
                Game = dto.Game.ToString(),
                ModKeyName = dto.ModKey.Name,
                ModKeyType = dto.ModKey.Type,
                ModKeyFileName = dto.ModKey.FileName,
                FormKeyModKeyName = dto.FormKey.ModKey.Name,
                FormKeyModKeyType = dto.FormKey.ModKey.Type,
                FormKeyModKeyFileName = dto.FormKey.ModKey.FileName,
                FormKeyId = dto.FormKey.Id,
                EditorId = dto.EditorID,
                dto.FormVersion,
                dto.MajorRecordFlags,
                dto.ImportedAtUTC,
                Name = GetEnglishText(dto.Name),
                ShortName = GetEnglishText(dto.ShortName),
                LongName = GetEnglishText(dto.LongName),
                dto.Version2,
                dto.VersionControl,
                dto.DispositionBase,
                dto.Aggression,
                dto.Confidence,
                dto.EnergyLevel,
                dto.Responsibility,
                dto.Assistance,
                dto.GearedUpWeapons,
                dto.HeightMin,
                dto.HeightMax,
                dto.SkinToneIndex,
                dto.Pronoun,
                VoiceModKeyName = dto.VoiceFormKey?.ModKey.Name,
                VoiceModKeyType = dto.VoiceFormKey?.ModKey.Type,
                VoiceModKeyFileName = dto.VoiceFormKey?.ModKey.FileName,
                VoiceFormKeyId = dto.VoiceFormKey?.Id,
                RaceModKeyName = dto.RaceFormKey?.ModKey.Name,
                RaceModKeyType = dto.RaceFormKey?.ModKey.Type,
                RaceModKeyFileName = dto.RaceFormKey?.ModKey.FileName,
                RaceFormKeyId = dto.RaceFormKey?.Id,
                CombatOverridePackageListModKeyName = dto.CombatOverridePackageListFormKey?.ModKey.Name,
                CombatOverridePackageListModKeyType = dto.CombatOverridePackageListFormKey?.ModKey.Type,
                CombatOverridePackageListModKeyFileName = dto.CombatOverridePackageListFormKey?.ModKey.FileName,
                CombatOverridePackageListFormKeyId = dto.CombatOverridePackageListFormKey?.Id,
                CombatStyleModKeyName = dto.CombatStyleFormKey?.ModKey.Name,
                CombatStyleModKeyType = dto.CombatStyleFormKey?.ModKey.Type,
                CombatStyleModKeyFileName = dto.CombatStyleFormKey?.ModKey.FileName,
                CombatStyleFormKeyId = dto.CombatStyleFormKey?.Id,
                DefaultPackageListModKeyName = dto.DefaultPackageListFormKey?.ModKey.Name,
                DefaultPackageListModKeyType = dto.DefaultPackageListFormKey?.ModKey.Type,
                DefaultPackageListModKeyFileName = dto.DefaultPackageListFormKey?.ModKey.FileName,
                DefaultPackageListFormKeyId = dto.DefaultPackageListFormKey?.Id,
                CrimeFactionModKeyName = dto.CrimeFactionFormKey?.ModKey.Name,
                CrimeFactionModKeyType = dto.CrimeFactionFormKey?.ModKey.Type,
                CrimeFactionModKeyFileName = dto.CrimeFactionFormKey?.ModKey.FileName,
                CrimeFactionFormKeyId = dto.CrimeFactionFormKey?.Id,
                dto.Template,
                dto.DefaultTemplate,
                dto.TemplateActors,
                dto.WornArmor,
                dto.FaceMorph,
                dto.FaceParts,
                dto.HeadParts,
                dto.HeadTexture,
                dto.SleepingOutfit,
                dto.TintLayers,
                dto.Tints,
                dto.SpaceOutfit,
                dto.BodyMorphRegionValues,
                dto.ObjectTemplates,
                dto.AIData
            });
    }

    private static NPCDTO ToDTO(NPCRow record, CreationsForge.Core.Enums.SupportedGame game)
    {
        var dto = new NPCDTO
        {
            Game = game,
            ModKey = new CreationsForge.Core.DTOs.Plugins.ModKeyDTO { Name = string.Empty, Type = 0, FileName = string.Empty },
            FormKey = new CreationsForge.Core.DTOs.Plugins.FormKeyDTO { ModKey = new CreationsForge.Core.DTOs.Plugins.ModKeyDTO { Name = string.Empty, Type = 0, FileName = string.Empty }, Id = 0 },
            EditorID = string.Empty,
            FormVersion = 0,
            MajorRecordFlags = 0,
            ImportedAtUTC = record.ImportedAtUTC,
            Name = FromEnglish(record.Name),
            ShortName = FromEnglish(record.ShortName),
            LongName = FromEnglish(record.LongName),
            Version2 = record.Version2,
            VersionControl = record.VersionControl,
            DispositionBase = record.DispositionBase,
            Aggression = record.Aggression,
            Confidence = record.Confidence,
            EnergyLevel = record.EnergyLevel,
            Responsibility = record.Responsibility,
            Assistance = record.Assistance,
            GearedUpWeapons = record.GearedUpWeapons,
            HeightMin = record.HeightMin,
            HeightMax = record.HeightMax,
            SkinToneIndex = record.SkinToneIndex,
            Pronoun = record.Pronoun,
            VoiceFormKey = CreateNullableFormKey(record.VoiceModKeyName, record.VoiceModKeyType, record.VoiceModKeyFileName, record.VoiceFormKeyId),
            RaceFormKey = CreateNullableFormKey(record.RaceModKeyName, record.RaceModKeyType, record.RaceModKeyFileName, record.RaceFormKeyId),
            CombatOverridePackageListFormKey = CreateNullableFormKey(record.CombatOverridePackageListModKeyName, record.CombatOverridePackageListModKeyType, record.CombatOverridePackageListModKeyFileName, record.CombatOverridePackageListFormKeyId),
            CombatStyleFormKey = CreateNullableFormKey(record.CombatStyleModKeyName, record.CombatStyleModKeyType, record.CombatStyleModKeyFileName, record.CombatStyleFormKeyId),
            DefaultPackageListFormKey = CreateNullableFormKey(record.DefaultPackageListModKeyName, record.DefaultPackageListModKeyType, record.DefaultPackageListModKeyFileName, record.DefaultPackageListFormKeyId),
            CrimeFactionFormKey = CreateNullableFormKey(record.CrimeFactionModKeyName, record.CrimeFactionModKeyType, record.CrimeFactionModKeyFileName, record.CrimeFactionFormKeyId),
            Template = record.Template,
            DefaultTemplate = record.DefaultTemplate,
            TemplateActors = record.TemplateActors,
            WornArmor = record.WornArmor,
            FaceMorph = record.FaceMorph,
            FaceParts = record.FaceParts,
            HeadParts = record.HeadParts,
            HeadTexture = record.HeadTexture,
            SleepingOutfit = record.SleepingOutfit,
            TintLayers = record.TintLayers,
            Tints = record.Tints,
            SpaceOutfit = record.SpaceOutfit,
            BodyMorphRegionValues = record.BodyMorphRegionValues,
            ObjectTemplates = record.ObjectTemplates,
            AIData = record.AIData
        };
        ApplyCommonFields(dto, record, game);
        return dto;
    }

    private static void ApplyLocalizedStrings(NPCDTO record, IReadOnlyList<LocalizedStringDTO> localizedStrings)
    {
        record.LocalizedStrings = localizedStrings.ToList();
        record.Name = BuildTranslatedString(localizedStrings, nameof(NPCDTO.Name), record.Name);
        record.ShortName = BuildTranslatedString(localizedStrings, nameof(NPCDTO.ShortName), record.ShortName);
        record.LongName = BuildTranslatedString(localizedStrings, nameof(NPCDTO.LongName), record.LongName);
    }

    private sealed class NPCRow : RecordRow
    {
        public string? Name { get; set; }
        public string? ShortName { get; set; }
        public string? LongName { get; set; }
        public int? Version2 { get; set; }
        public int? VersionControl { get; set; }
        public int DispositionBase { get; set; }
        public string Aggression { get; set; } = string.Empty;
        public string Confidence { get; set; } = string.Empty;
        public int EnergyLevel { get; set; }
        public string Responsibility { get; set; } = string.Empty;
        public string Assistance { get; set; } = string.Empty;
        public int GearedUpWeapons { get; set; }
        public double HeightMin { get; set; }
        public double HeightMax { get; set; }
        public int? SkinToneIndex { get; set; }
        public string? Pronoun { get; set; }
        public string? VoiceModKeyName { get; set; }
        public int? VoiceModKeyType { get; set; }
        public string? VoiceModKeyFileName { get; set; }
        public long? VoiceFormKeyId { get; set; }
        public string? RaceModKeyName { get; set; }
        public int? RaceModKeyType { get; set; }
        public string? RaceModKeyFileName { get; set; }
        public long? RaceFormKeyId { get; set; }
        public string? CombatOverridePackageListModKeyName { get; set; }
        public int? CombatOverridePackageListModKeyType { get; set; }
        public string? CombatOverridePackageListModKeyFileName { get; set; }
        public long? CombatOverridePackageListFormKeyId { get; set; }
        public string? CombatStyleModKeyName { get; set; }
        public int? CombatStyleModKeyType { get; set; }
        public string? CombatStyleModKeyFileName { get; set; }
        public long? CombatStyleFormKeyId { get; set; }
        public string? DefaultPackageListModKeyName { get; set; }
        public int? DefaultPackageListModKeyType { get; set; }
        public string? DefaultPackageListModKeyFileName { get; set; }
        public long? DefaultPackageListFormKeyId { get; set; }
        public string? CrimeFactionModKeyName { get; set; }
        public int? CrimeFactionModKeyType { get; set; }
        public string? CrimeFactionModKeyFileName { get; set; }
        public long? CrimeFactionFormKeyId { get; set; }
        public string? Template { get; set; }
        public string? DefaultTemplate { get; set; }
        public string? TemplateActors { get; set; }
        public string? WornArmor { get; set; }
        public string? FaceMorph { get; set; }
        public string? FaceParts { get; set; }
        public string? HeadParts { get; set; }
        public string? HeadTexture { get; set; }
        public string? SleepingOutfit { get; set; }
        public string? TintLayers { get; set; }
        public string? Tints { get; set; }
        public string? SpaceOutfit { get; set; }
        public string? BodyMorphRegionValues { get; set; }
        public string? ObjectTemplates { get; set; }
        public string? AIData { get; set; }
    }
}
