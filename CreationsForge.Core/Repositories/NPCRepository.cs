using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.Enums;
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
                    SelectColumn("IsCompressed"),
                    SelectColumn("ObjectBoundsFirst"),
                    SelectColumn("ObjectBoundsSecond"),
                    SelectColumn("Flags"),
                    SelectColumn("MajorFlags"),
                    SelectColumn("Level_MutagenObjectType", "LevelMutagenObjectType"),
                    SelectColumn("Level_Level", "LevelLevel"),
                    SelectColumn("Level_LevelMult", "LevelLevelMult"),
                    SelectColumn("Configuration_Flags", "ConfigurationFlags"),
                    SelectColumn("Configuration_Level_MutagenObjectType", "ConfigurationLevelMutagenObjectType"),
                    SelectColumn("Configuration_Level_Level", "ConfigurationLevelLevel"),
                    SelectColumn("Configuration_Level_LevelMult", "ConfigurationLevelLevelMult"),
                    SelectColumn("Configuration_CalcMinLevel", "ConfigurationCalcMinLevel"),
                    SelectColumn("Configuration_CalcMaxLevel", "ConfigurationCalcMaxLevel"),
                    SelectColumn("Configuration_HealthOffset", "ConfigurationHealthOffset"),
                    SelectColumn("Configuration_SpeedMultiplier", "ConfigurationSpeedMultiplier"),
                    SelectColumn("Configuration_TemplateFlags", "ConfigurationTemplateFlags"),
                    SelectColumn("Version2"),
                    SelectColumn("VersionControl"),
                    SelectColumn("DispositionBase"),
                    SelectColumn("UseTemplateActors"),
                    SelectColumn("Aggression"),
                    SelectColumn("Confidence"),
                    SelectColumn("EnergyLevel"),
                    SelectColumn("Responsibility"),
                    SelectColumn("Assistance"),
                    SelectColumn("Mood"),
                    SelectColumn("GearedUpWeapons"),
                    SelectColumn("HeightMin"),
                    SelectColumn("HeightMax"),
                    SelectColumn("SkinToneIndex"),
                    SelectColumn("Skin_ModKey_Name", "SkinModKeyName"),
                    SelectColumn("Skin_ModKey_Type", "SkinModKeyType"),
                    SelectColumn("Skin_ModKey_FileName", "SkinModKeyFileName"),
                    SelectColumn("Skin_FormKey_ID", "SkinFormKeyId"),
                    SelectColumn("Pronoun"),
                    SelectColumn("Voice_ModKey_Name", "VoiceModKeyName"),
                    SelectColumn("Voice_ModKey_Type", "VoiceModKeyType"),
                    SelectColumn("Voice_ModKey_FileName", "VoiceModKeyFileName"),
                    SelectColumn("Voice_FormKey_ID", "VoiceFormKeyId"),
                    SelectColumn("Race_ModKey_Name", "RaceModKeyName"),
                    SelectColumn("Race_ModKey_Type", "RaceModKeyType"),
                    SelectColumn("Race_ModKey_FileName", "RaceModKeyFileName"),
                    SelectColumn("Race_FormKey_ID", "RaceFormKeyId"),
                    SelectColumn("AttackRace_ModKey_Name", "AttackRaceModKeyName"),
                    SelectColumn("AttackRace_ModKey_Type", "AttackRaceModKeyType"),
                    SelectColumn("AttackRace_ModKey_FileName", "AttackRaceModKeyFileName"),
                    SelectColumn("AttackRace_FormKey_ID", "AttackRaceFormKeyId"),
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
                    SelectColumn("Class_ModKey_Name", "ClassModKeyName"),
                    SelectColumn("Class_ModKey_Type", "ClassModKeyType"),
                    SelectColumn("Class_ModKey_FileName", "ClassModKeyFileName"),
                    SelectColumn("Class_FormKey_ID", "ClassFormKeyId"),
                    SelectColumn("DeathItem_ModKey_Name", "DeathItemModKeyName"),
                    SelectColumn("DeathItem_ModKey_Type", "DeathItemModKeyType"),
                    SelectColumn("DeathItem_ModKey_FileName", "DeathItemModKeyFileName"),
                    SelectColumn("DeathItem_FormKey_ID", "DeathItemFormKeyId"),
                    SelectColumn("DefaultOutfit_ModKey_Name", "DefaultOutfitModKeyName"),
                    SelectColumn("DefaultOutfit_ModKey_Type", "DefaultOutfitModKeyType"),
                    SelectColumn("DefaultOutfit_ModKey_FileName", "DefaultOutfitModKeyFileName"),
                    SelectColumn("DefaultOutfit_FormKey_ID", "DefaultOutfitFormKeyId"),
                    SelectColumn("SleepingOutfit_ModKey_Name", "SleepingOutfitModKeyName"),
                    SelectColumn("SleepingOutfit_ModKey_Type", "SleepingOutfitModKeyType"),
                    SelectColumn("SleepingOutfit_ModKey_FileName", "SleepingOutfitModKeyFileName"),
                    SelectColumn("SleepingOutfit_FormKey_ID", "SleepingOutfitFormKeyId"),
                    SelectColumn("WornArmor_ModKey_Name", "WornArmorModKeyName"),
                    SelectColumn("WornArmor_ModKey_Type", "WornArmorModKeyType"),
                    SelectColumn("WornArmor_ModKey_FileName", "WornArmorModKeyFileName"),
                    SelectColumn("WornArmor_FormKey_ID", "WornArmorFormKeyId"),
                    SelectColumn("PowerArmorStand_ModKey_Name", "PowerArmorStandModKeyName"),
                    SelectColumn("PowerArmorStand_ModKey_Type", "PowerArmorStandModKeyType"),
                    SelectColumn("PowerArmorStand_ModKey_FileName", "PowerArmorStandModKeyFileName"),
                    SelectColumn("PowerArmorStand_FormKey_ID", "PowerArmorStandFormKeyId"),
                    SelectColumn("SpaceOutfit_ModKey_Name", "SpaceOutfitModKeyName"),
                    SelectColumn("SpaceOutfit_ModKey_Type", "SpaceOutfitModKeyType"),
                    SelectColumn("SpaceOutfit_ModKey_FileName", "SpaceOutfitModKeyFileName"),
                    SelectColumn("SpaceOutfit_FormKey_ID", "SpaceOutfitFormKeyId"),
                    SelectColumn("HeadTexture_ModKey_Name", "HeadTextureModKeyName"),
                    SelectColumn("HeadTexture_ModKey_Type", "HeadTextureModKeyType"),
                    SelectColumn("HeadTexture_ModKey_FileName", "HeadTextureModKeyFileName"),
                    SelectColumn("HeadTexture_FormKey_ID", "HeadTextureFormKeyId"),
                    SelectColumn("Template_ModKey_Name", "TemplateModKeyName"),
                    SelectColumn("Template_ModKey_Type", "TemplateModKeyType"),
                    SelectColumn("Template_ModKey_FileName", "TemplateModKeyFileName"),
                    SelectColumn("Template_FormKey_ID", "TemplateFormKeyId"),
                    SelectColumn("DefaultTemplate_ModKey_Name", "DefaultTemplateModKeyName"),
                    SelectColumn("DefaultTemplate_ModKey_Type", "DefaultTemplateModKeyType"),
                    SelectColumn("DefaultTemplate_ModKey_FileName", "DefaultTemplateModKeyFileName"),
                    SelectColumn("DefaultTemplate_FormKey_ID", "DefaultTemplateFormKeyId"),
                    SelectColumn("TemplateActors_Trait_ModKey_Name", "TemplateActorsTraitModKeyName"),
                    SelectColumn("TemplateActors_Trait_ModKey_Type", "TemplateActorsTraitModKeyType"),
                    SelectColumn("TemplateActors_Trait_ModKey_FileName", "TemplateActorsTraitModKeyFileName"),
                    SelectColumn("TemplateActors_Trait_FormKey_ID", "TemplateActorsTraitFormKeyId"),
                    SelectColumn("TemplateActors_Stats_ModKey_Name", "TemplateActorsStatsModKeyName"),
                    SelectColumn("TemplateActors_Stats_ModKey_Type", "TemplateActorsStatsModKeyType"),
                    SelectColumn("TemplateActors_Stats_ModKey_FileName", "TemplateActorsStatsModKeyFileName"),
                    SelectColumn("TemplateActors_Stats_FormKey_ID", "TemplateActorsStatsFormKeyId"),
                    SelectColumn("TemplateActors_Factions_ModKey_Name", "TemplateActorsFactionsModKeyName"),
                    SelectColumn("TemplateActors_Factions_ModKey_Type", "TemplateActorsFactionsModKeyType"),
                    SelectColumn("TemplateActors_Factions_ModKey_FileName", "TemplateActorsFactionsModKeyFileName"),
                    SelectColumn("TemplateActors_Factions_FormKey_ID", "TemplateActorsFactionsFormKeyId"),
                    SelectColumn("TemplateActors_SpellList_ModKey_Name", "TemplateActorsSpellListModKeyName"),
                    SelectColumn("TemplateActors_SpellList_ModKey_Type", "TemplateActorsSpellListModKeyType"),
                    SelectColumn("TemplateActors_SpellList_ModKey_FileName", "TemplateActorsSpellListModKeyFileName"),
                    SelectColumn("TemplateActors_SpellList_FormKey_ID", "TemplateActorsSpellListFormKeyId"),
                    SelectColumn("TemplateActors_AiPackages_ModKey_Name", "TemplateActorsAiPackagesModKeyName"),
                    SelectColumn("TemplateActors_AiPackages_ModKey_Type", "TemplateActorsAiPackagesModKeyType"),
                    SelectColumn("TemplateActors_AiPackages_ModKey_FileName", "TemplateActorsAiPackagesModKeyFileName"),
                    SelectColumn("TemplateActors_AiPackages_FormKey_ID", "TemplateActorsAiPackagesFormKeyId"),
                    SelectColumn("TemplateActors_AiData_ModKey_Name", "TemplateActorsAiDataModKeyName"),
                    SelectColumn("TemplateActors_AiData_ModKey_Type", "TemplateActorsAiDataModKeyType"),
                    SelectColumn("TemplateActors_AiData_ModKey_FileName", "TemplateActorsAiDataModKeyFileName"),
                    SelectColumn("TemplateActors_AiData_FormKey_ID", "TemplateActorsAiDataFormKeyId"),
                    SelectColumn("TemplateActors_BaseData_ModKey_Name", "TemplateActorsBaseDataModKeyName"),
                    SelectColumn("TemplateActors_BaseData_ModKey_Type", "TemplateActorsBaseDataModKeyType"),
                    SelectColumn("TemplateActors_BaseData_ModKey_FileName", "TemplateActorsBaseDataModKeyFileName"),
                    SelectColumn("TemplateActors_BaseData_FormKey_ID", "TemplateActorsBaseDataFormKeyId"),
                    SelectColumn("TemplateActors_Inventory_ModKey_Name", "TemplateActorsInventoryModKeyName"),
                    SelectColumn("TemplateActors_Inventory_ModKey_Type", "TemplateActorsInventoryModKeyType"),
                    SelectColumn("TemplateActors_Inventory_ModKey_FileName", "TemplateActorsInventoryModKeyFileName"),
                    SelectColumn("TemplateActors_Inventory_FormKey_ID", "TemplateActorsInventoryFormKeyId"),
                    SelectColumn("TemplateActors_Script_ModKey_Name", "TemplateActorsScriptModKeyName"),
                    SelectColumn("TemplateActors_Script_ModKey_Type", "TemplateActorsScriptModKeyType"),
                    SelectColumn("TemplateActors_Script_ModKey_FileName", "TemplateActorsScriptModKeyFileName"),
                    SelectColumn("TemplateActors_Script_FormKey_ID", "TemplateActorsScriptFormKeyId"),
                    SelectColumn("TemplateActors_DefPackList_ModKey_Name", "TemplateActorsDefPackListModKeyName"),
                    SelectColumn("TemplateActors_DefPackList_ModKey_Type", "TemplateActorsDefPackListModKeyType"),
                    SelectColumn("TemplateActors_DefPackList_ModKey_FileName", "TemplateActorsDefPackListModKeyFileName"),
                    SelectColumn("TemplateActors_DefPackList_FormKey_ID", "TemplateActorsDefPackListFormKeyId"),
                    SelectColumn("TemplateActors_AttackData_ModKey_Name", "TemplateActorsAttackDataModKeyName"),
                    SelectColumn("TemplateActors_AttackData_ModKey_Type", "TemplateActorsAttackDataModKeyType"),
                    SelectColumn("TemplateActors_AttackData_ModKey_FileName", "TemplateActorsAttackDataModKeyFileName"),
                    SelectColumn("TemplateActors_AttackData_FormKey_ID", "TemplateActorsAttackDataFormKeyId"),
                    SelectColumn("TemplateActors_Keywords_ModKey_Name", "TemplateActorsKeywordsModKeyName"),
                    SelectColumn("TemplateActors_Keywords_ModKey_Type", "TemplateActorsKeywordsModKeyType"),
                    SelectColumn("TemplateActors_Keywords_ModKey_FileName", "TemplateActorsKeywordsModKeyFileName"),
                    SelectColumn("TemplateActors_Keywords_FormKey_ID", "TemplateActorsKeywordsFormKeyId"),
                    SelectColumn("TemplateActors_Unknown1_ModKey_Name", "TemplateActorsUnknown1ModKeyName"),
                    SelectColumn("TemplateActors_Unknown1_ModKey_Type", "TemplateActorsUnknown1ModKeyType"),
                    SelectColumn("TemplateActors_Unknown1_ModKey_FileName", "TemplateActorsUnknown1ModKeyFileName"),
                    SelectColumn("TemplateActors_Unknown1_FormKey_ID", "TemplateActorsUnknown1FormKeyId"),
                    SelectColumn("TemplateActors_Unknown2_ModKey_Name", "TemplateActorsUnknown2ModKeyName"),
                    SelectColumn("TemplateActors_Unknown2_ModKey_Type", "TemplateActorsUnknown2ModKeyType"),
                    SelectColumn("TemplateActors_Unknown2_ModKey_FileName", "TemplateActorsUnknown2ModKeyFileName"),
                    SelectColumn("TemplateActors_Unknown2_FormKey_ID", "TemplateActorsUnknown2FormKeyId"),
                    SelectColumn("CalculatedHealth"),
                    SelectColumn("CalculatedActionPoints"),
                    SelectColumn("XpValueOffset"),
                    SelectColumn("Unknown"),
                    SelectColumn("Unused"),
                    SelectColumn("NAM5"),
                    SelectColumn("Height"),
                    SelectColumn("Weight_Value", "WeightValue"),
                    SelectColumn("Weight_Thin", "WeightThin"),
                    SelectColumn("Weight_Muscular", "WeightMuscular"),
                    SelectColumn("Weight_Fat", "WeightFat"),
                    SelectColumn("SoundLevel"),
                    SelectColumn("TextureLighting"),
                    SelectColumn("HairColor"),
                    SelectColumn("FacialHairColor"),
                    SelectColumn("EyebrowColor"),
                    SelectColumn("EyeColor"),
                    SelectColumn("FaceMorph_NoseLongVsShort", "FaceMorphNoseLongVsShort"),
                    SelectColumn("FaceMorph_NoseUpVsDown", "FaceMorphNoseUpVsDown"),
                    SelectColumn("FaceMorph_JawUpVsDown", "FaceMorphJawUpVsDown"),
                    SelectColumn("FaceMorph_JawNarrowVsWide", "FaceMorphJawNarrowVsWide"),
                    SelectColumn("FaceMorph_JawForwardVsBack", "FaceMorphJawForwardVsBack"),
                    SelectColumn("FaceMorph_CheeksUpVsDown", "FaceMorphCheeksUpVsDown"),
                    SelectColumn("FaceMorph_CheeksForwardVsBack", "FaceMorphCheeksForwardVsBack"),
                    SelectColumn("FaceMorph_EyesUpVsDown", "FaceMorphEyesUpVsDown"),
                    SelectColumn("FaceMorph_EyesInVsOut", "FaceMorphEyesInVsOut"),
                    SelectColumn("FaceMorph_BrowsUpVsDown", "FaceMorphBrowsUpVsDown"),
                    SelectColumn("FaceMorph_BrowsInVsOut", "FaceMorphBrowsInVsOut"),
                    SelectColumn("FaceMorph_BrowsForwardVsBack", "FaceMorphBrowsForwardVsBack"),
                    SelectColumn("FaceMorph_LipsUpVsDown", "FaceMorphLipsUpVsDown"),
                    SelectColumn("FaceMorph_LipsInVsOut", "FaceMorphLipsInVsOut"),
                    SelectColumn("FaceMorph_ChinNarrowVsWide", "FaceMorphChinNarrowVsWide"),
                    SelectColumn("FaceMorph_ChinUpVsDown", "FaceMorphChinUpVsDown"),
                    SelectColumn("FaceMorph_ChinUnderbiteVsOverbite", "FaceMorphChinUnderbiteVsOverbite"),
                    SelectColumn("FaceMorph_EyesForwardVsBack", "FaceMorphEyesForwardVsBack"),
                    SelectColumn("FaceMorph_Unknown", "FaceMorphUnknown"),
                    SelectColumn("FaceParts_Nose", "FacePartsNose"),
                    SelectColumn("FaceParts_Unknown", "FacePartsUnknown"),
                    SelectColumn("FaceParts_Eyes", "FacePartsEyes"),
                    SelectColumn("FaceParts_Mouth", "FacePartsMouth"),
                    SelectColumn("PlayerSkills_Health", "PlayerSkillsHealth"),
                    SelectColumn("PlayerSkills_Magicka", "PlayerSkillsMagicka"),
                    SelectColumn("PlayerSkills_Stamina", "PlayerSkillsStamina"),
                    SelectColumn("PlayerSkills_GearedUpWeapons", "PlayerSkillsGearedUpWeapons"),
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
            record.Packages = FetchFormKeyList(game, formKey, "Packages").ToList();
            record.ForcedLocations = FetchFormKeyList(game, formKey, "ForcedLocations").ToList();
            record.HeadParts = FetchFormKeyList(game, formKey, "HeadParts").ToList();
            record.ActorEffects = FetchFormKeyList(game, formKey, "ActorEffect").ToList();
            record.Factions = FetchFactions(game, formKey);
            record.Properties = FetchProperties(game, formKey);
            record.Items = FetchItems(game, formKey);
            record.Perks = FetchPerks(game, formKey);
            record.Morphs = FetchMorphs(game, formKey);
            record.FaceDialPositions = FetchFaceDialPositions(game, formKey);
            record.FaceMorphs = FetchFaceMorphPositions(game, formKey);
            record.FaceMorphGroups = FetchFaceMorphGroups(game, formKey);
            record.MorphBlends = FetchMorphBlends(game, formKey);
            record.Tints = FetchTints(game, formKey);
            record.TintLayers = FetchTintLayers(game, formKey);
            record.FaceTintingLayers = FetchFaceTintingLayers(game, formKey);
            record.PlayerSkills = FetchPlayerSkills(game, formKey, record.PlayerSkills);
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
                EnergyLevel, Responsibility, Assistance, Mood, GearedUpWeapons, HeightMin, HeightMax, SkinToneIndex, Pronoun,
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
                @EnergyLevel, @Responsibility, @Assistance, @Mood, @GearedUpWeapons, @HeightMin, @HeightMax, @SkinToneIndex, @Pronoun,
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
                dto.Mood,
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
                Template = FormatFormKey(dto.Template),
                DefaultTemplate = FormatFormKey(dto.DefaultTemplate),
                TemplateActors = null as string,
                WornArmor = FormatFormKey(dto.WornArmor),
                FaceMorph = null as string,
                FaceParts = null as string,
                HeadParts = null as string,
                HeadTexture = FormatFormKey(dto.HeadTexture),
                SleepingOutfit = FormatFormKey(dto.SleepingOutfit),
                TintLayers = null as string,
                Tints = null as string,
                SpaceOutfit = FormatFormKey(dto.SpaceOutfit),
                dto.BodyMorphRegionValues,
                dto.ObjectTemplates,
                dto.AIData
            });
        UpdateExpandedRootFields(dto);
        ReplaceFormKeyList(dto, "Packages", dto.Packages);
        ReplaceFormKeyList(dto, "ForcedLocations", dto.ForcedLocations);
        ReplaceFormKeyList(dto, "HeadParts", dto.HeadParts);
        ReplaceFormKeyList(dto, "ActorEffect", dto.ActorEffects);
        ReplaceFactions(dto);
        ReplaceProperties(dto);
        ReplaceItems(dto);
        ReplacePerks(dto);
        ReplaceMorphs(dto);
        ReplaceFaceDialPositions(dto);
        ReplaceFaceMorphPositions(dto);
        ReplaceMorphBlends(dto);
        ReplaceTints(dto);
        ReplaceTintLayers(dto);
        ReplaceFaceTintingLayers(dto);
        ReplacePlayerSkills(dto);
    }

    private void UpdateExpandedRootFields(NPCDTO dto)
    {
        Database.Execute(
            """
            UPDATE NPCs
            SET
                IsCompressed = @IsCompressed,
                ObjectBoundsFirst = @ObjectBoundsFirst,
                ObjectBoundsSecond = @ObjectBoundsSecond,
                Flags = @Flags,
                MajorFlags = @MajorFlags,
                Level_MutagenObjectType = @LevelMutagenObjectType,
                Level_Level = @LevelLevel,
                Level_LevelMult = @LevelLevelMult,
                Configuration_Flags = @ConfigurationFlags,
                Configuration_Level_MutagenObjectType = @ConfigurationLevelMutagenObjectType,
                Configuration_Level_Level = @ConfigurationLevelLevel,
                Configuration_Level_LevelMult = @ConfigurationLevelLevelMult,
                Configuration_CalcMinLevel = @ConfigurationCalcMinLevel,
                Configuration_CalcMaxLevel = @ConfigurationCalcMaxLevel,
                Configuration_HealthOffset = @ConfigurationHealthOffset,
                Configuration_SpeedMultiplier = @ConfigurationSpeedMultiplier,
                Configuration_TemplateFlags = @ConfigurationTemplateFlags,
                UseTemplateActors = @UseTemplateActors,
                Skin_ModKey_Name = @SkinModKeyName,
                Skin_ModKey_Type = @SkinModKeyType,
                Skin_ModKey_FileName = @SkinModKeyFileName,
                Skin_FormKey_ID = @SkinFormKeyId,
                AttackRace_ModKey_Name = @AttackRaceModKeyName,
                AttackRace_ModKey_Type = @AttackRaceModKeyType,
                AttackRace_ModKey_FileName = @AttackRaceModKeyFileName,
                AttackRace_FormKey_ID = @AttackRaceFormKeyId,
                Class_ModKey_Name = @ClassModKeyName,
                Class_ModKey_Type = @ClassModKeyType,
                Class_ModKey_FileName = @ClassModKeyFileName,
                Class_FormKey_ID = @ClassFormKeyId,
                DeathItem_ModKey_Name = @DeathItemModKeyName,
                DeathItem_ModKey_Type = @DeathItemModKeyType,
                DeathItem_ModKey_FileName = @DeathItemModKeyFileName,
                DeathItem_FormKey_ID = @DeathItemFormKeyId,
                DefaultOutfit_ModKey_Name = @DefaultOutfitModKeyName,
                DefaultOutfit_ModKey_Type = @DefaultOutfitModKeyType,
                DefaultOutfit_ModKey_FileName = @DefaultOutfitModKeyFileName,
                DefaultOutfit_FormKey_ID = @DefaultOutfitFormKeyId,
                SleepingOutfit_ModKey_Name = @SleepingOutfitModKeyName,
                SleepingOutfit_ModKey_Type = @SleepingOutfitModKeyType,
                SleepingOutfit_ModKey_FileName = @SleepingOutfitModKeyFileName,
                SleepingOutfit_FormKey_ID = @SleepingOutfitFormKeyId,
                WornArmor_ModKey_Name = @WornArmorModKeyName,
                WornArmor_ModKey_Type = @WornArmorModKeyType,
                WornArmor_ModKey_FileName = @WornArmorModKeyFileName,
                WornArmor_FormKey_ID = @WornArmorFormKeyId,
                PowerArmorStand_ModKey_Name = @PowerArmorStandModKeyName,
                PowerArmorStand_ModKey_Type = @PowerArmorStandModKeyType,
                PowerArmorStand_ModKey_FileName = @PowerArmorStandModKeyFileName,
                PowerArmorStand_FormKey_ID = @PowerArmorStandFormKeyId,
                SpaceOutfit_ModKey_Name = @SpaceOutfitModKeyName,
                SpaceOutfit_ModKey_Type = @SpaceOutfitModKeyType,
                SpaceOutfit_ModKey_FileName = @SpaceOutfitModKeyFileName,
                SpaceOutfit_FormKey_ID = @SpaceOutfitFormKeyId,
                HeadTexture_ModKey_Name = @HeadTextureModKeyName,
                HeadTexture_ModKey_Type = @HeadTextureModKeyType,
                HeadTexture_ModKey_FileName = @HeadTextureModKeyFileName,
                HeadTexture_FormKey_ID = @HeadTextureFormKeyId,
                Template_ModKey_Name = @TemplateModKeyName,
                Template_ModKey_Type = @TemplateModKeyType,
                Template_ModKey_FileName = @TemplateModKeyFileName,
                Template_FormKey_ID = @TemplateFormKeyId,
                DefaultTemplate_ModKey_Name = @DefaultTemplateModKeyName,
                DefaultTemplate_ModKey_Type = @DefaultTemplateModKeyType,
                DefaultTemplate_ModKey_FileName = @DefaultTemplateModKeyFileName,
                DefaultTemplate_FormKey_ID = @DefaultTemplateFormKeyId,
                TemplateActors_Trait_ModKey_Name = @TemplateActorsTraitModKeyName,
                TemplateActors_Trait_ModKey_Type = @TemplateActorsTraitModKeyType,
                TemplateActors_Trait_ModKey_FileName = @TemplateActorsTraitModKeyFileName,
                TemplateActors_Trait_FormKey_ID = @TemplateActorsTraitFormKeyId,
                TemplateActors_Stats_ModKey_Name = @TemplateActorsStatsModKeyName,
                TemplateActors_Stats_ModKey_Type = @TemplateActorsStatsModKeyType,
                TemplateActors_Stats_ModKey_FileName = @TemplateActorsStatsModKeyFileName,
                TemplateActors_Stats_FormKey_ID = @TemplateActorsStatsFormKeyId,
                TemplateActors_Factions_ModKey_Name = @TemplateActorsFactionsModKeyName,
                TemplateActors_Factions_ModKey_Type = @TemplateActorsFactionsModKeyType,
                TemplateActors_Factions_ModKey_FileName = @TemplateActorsFactionsModKeyFileName,
                TemplateActors_Factions_FormKey_ID = @TemplateActorsFactionsFormKeyId,
                TemplateActors_SpellList_ModKey_Name = @TemplateActorsSpellListModKeyName,
                TemplateActors_SpellList_ModKey_Type = @TemplateActorsSpellListModKeyType,
                TemplateActors_SpellList_ModKey_FileName = @TemplateActorsSpellListModKeyFileName,
                TemplateActors_SpellList_FormKey_ID = @TemplateActorsSpellListFormKeyId,
                TemplateActors_AiPackages_ModKey_Name = @TemplateActorsAiPackagesModKeyName,
                TemplateActors_AiPackages_ModKey_Type = @TemplateActorsAiPackagesModKeyType,
                TemplateActors_AiPackages_ModKey_FileName = @TemplateActorsAiPackagesModKeyFileName,
                TemplateActors_AiPackages_FormKey_ID = @TemplateActorsAiPackagesFormKeyId,
                TemplateActors_AiData_ModKey_Name = @TemplateActorsAiDataModKeyName,
                TemplateActors_AiData_ModKey_Type = @TemplateActorsAiDataModKeyType,
                TemplateActors_AiData_ModKey_FileName = @TemplateActorsAiDataModKeyFileName,
                TemplateActors_AiData_FormKey_ID = @TemplateActorsAiDataFormKeyId,
                TemplateActors_BaseData_ModKey_Name = @TemplateActorsBaseDataModKeyName,
                TemplateActors_BaseData_ModKey_Type = @TemplateActorsBaseDataModKeyType,
                TemplateActors_BaseData_ModKey_FileName = @TemplateActorsBaseDataModKeyFileName,
                TemplateActors_BaseData_FormKey_ID = @TemplateActorsBaseDataFormKeyId,
                TemplateActors_Inventory_ModKey_Name = @TemplateActorsInventoryModKeyName,
                TemplateActors_Inventory_ModKey_Type = @TemplateActorsInventoryModKeyType,
                TemplateActors_Inventory_ModKey_FileName = @TemplateActorsInventoryModKeyFileName,
                TemplateActors_Inventory_FormKey_ID = @TemplateActorsInventoryFormKeyId,
                TemplateActors_Script_ModKey_Name = @TemplateActorsScriptModKeyName,
                TemplateActors_Script_ModKey_Type = @TemplateActorsScriptModKeyType,
                TemplateActors_Script_ModKey_FileName = @TemplateActorsScriptModKeyFileName,
                TemplateActors_Script_FormKey_ID = @TemplateActorsScriptFormKeyId,
                TemplateActors_DefPackList_ModKey_Name = @TemplateActorsDefPackListModKeyName,
                TemplateActors_DefPackList_ModKey_Type = @TemplateActorsDefPackListModKeyType,
                TemplateActors_DefPackList_ModKey_FileName = @TemplateActorsDefPackListModKeyFileName,
                TemplateActors_DefPackList_FormKey_ID = @TemplateActorsDefPackListFormKeyId,
                TemplateActors_AttackData_ModKey_Name = @TemplateActorsAttackDataModKeyName,
                TemplateActors_AttackData_ModKey_Type = @TemplateActorsAttackDataModKeyType,
                TemplateActors_AttackData_ModKey_FileName = @TemplateActorsAttackDataModKeyFileName,
                TemplateActors_AttackData_FormKey_ID = @TemplateActorsAttackDataFormKeyId,
                TemplateActors_Keywords_ModKey_Name = @TemplateActorsKeywordsModKeyName,
                TemplateActors_Keywords_ModKey_Type = @TemplateActorsKeywordsModKeyType,
                TemplateActors_Keywords_ModKey_FileName = @TemplateActorsKeywordsModKeyFileName,
                TemplateActors_Keywords_FormKey_ID = @TemplateActorsKeywordsFormKeyId,
                TemplateActors_Unknown1_ModKey_Name = @TemplateActorsUnknown1ModKeyName,
                TemplateActors_Unknown1_ModKey_Type = @TemplateActorsUnknown1ModKeyType,
                TemplateActors_Unknown1_ModKey_FileName = @TemplateActorsUnknown1ModKeyFileName,
                TemplateActors_Unknown1_FormKey_ID = @TemplateActorsUnknown1FormKeyId,
                TemplateActors_Unknown2_ModKey_Name = @TemplateActorsUnknown2ModKeyName,
                TemplateActors_Unknown2_ModKey_Type = @TemplateActorsUnknown2ModKeyType,
                TemplateActors_Unknown2_ModKey_FileName = @TemplateActorsUnknown2ModKeyFileName,
                TemplateActors_Unknown2_FormKey_ID = @TemplateActorsUnknown2FormKeyId,
                CalculatedHealth = @CalculatedHealth,
                CalculatedActionPoints = @CalculatedActionPoints,
                XpValueOffset = @XpValueOffset,
                Unknown = @Unknown,
                Unused = @Unused,
                NAM5 = @NAM5,
                Height = @Height,
                Weight_Value = @WeightValue,
                Weight_Thin = @WeightThin,
                Weight_Muscular = @WeightMuscular,
                Weight_Fat = @WeightFat,
                SoundLevel = @SoundLevel,
                TextureLighting = @TextureLighting,
                HairColor = @HairColor,
                FacialHairColor = @FacialHairColor,
                EyebrowColor = @EyebrowColor,
                EyeColor = @EyeColor,
                FaceMorph_NoseLongVsShort = @FaceMorphNoseLongVsShort,
                FaceMorph_NoseUpVsDown = @FaceMorphNoseUpVsDown,
                FaceMorph_JawUpVsDown = @FaceMorphJawUpVsDown,
                FaceMorph_JawNarrowVsWide = @FaceMorphJawNarrowVsWide,
                FaceMorph_JawForwardVsBack = @FaceMorphJawForwardVsBack,
                FaceMorph_CheeksUpVsDown = @FaceMorphCheeksUpVsDown,
                FaceMorph_CheeksForwardVsBack = @FaceMorphCheeksForwardVsBack,
                FaceMorph_EyesUpVsDown = @FaceMorphEyesUpVsDown,
                FaceMorph_EyesInVsOut = @FaceMorphEyesInVsOut,
                FaceMorph_BrowsUpVsDown = @FaceMorphBrowsUpVsDown,
                FaceMorph_BrowsInVsOut = @FaceMorphBrowsInVsOut,
                FaceMorph_BrowsForwardVsBack = @FaceMorphBrowsForwardVsBack,
                FaceMorph_LipsUpVsDown = @FaceMorphLipsUpVsDown,
                FaceMorph_LipsInVsOut = @FaceMorphLipsInVsOut,
                FaceMorph_ChinNarrowVsWide = @FaceMorphChinNarrowVsWide,
                FaceMorph_ChinUpVsDown = @FaceMorphChinUpVsDown,
                FaceMorph_ChinUnderbiteVsOverbite = @FaceMorphChinUnderbiteVsOverbite,
                FaceMorph_EyesForwardVsBack = @FaceMorphEyesForwardVsBack,
                FaceMorph_Unknown = @FaceMorphUnknown,
                FaceParts_Nose = @FacePartsNose,
                FaceParts_Unknown = @FacePartsUnknown,
                FaceParts_Eyes = @FacePartsEyes,
                FaceParts_Mouth = @FacePartsMouth,
                PlayerSkills_Health = @PlayerSkillsHealth,
                PlayerSkills_Magicka = @PlayerSkillsMagicka,
                PlayerSkills_Stamina = @PlayerSkillsStamina,
                PlayerSkills_GearedUpWeapons = @PlayerSkillsGearedUpWeapons
            WHERE Game = @Game
              AND ModKey_Name = @ModKeyName
              AND ModKey_Type = @ModKeyType
              AND ModKey_FileName = @ModKeyFileName
              AND FormKey_ModKey_Name = @FormKeyModKeyName
              AND FormKey_ModKey_Type = @FormKeyModKeyType
              AND FormKey_ModKey_FileName = @FormKeyModKeyFileName
              AND FormKey_ID = @FormKeyId;
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
                dto.IsCompressed,
                dto.ObjectBoundsFirst,
                dto.ObjectBoundsSecond,
                dto.Flags,
                dto.MajorFlags,
                LevelMutagenObjectType = dto.Level?.MutagenObjectType,
                LevelLevel = dto.Level?.Level,
                LevelLevelMult = dto.Level?.LevelMult,
                ConfigurationFlags = FormatStringList(dto.Configuration?.Flags),
                ConfigurationLevelMutagenObjectType = dto.Configuration?.Level?.MutagenObjectType,
                ConfigurationLevelLevel = dto.Configuration?.Level?.Level,
                ConfigurationLevelLevelMult = dto.Configuration?.Level?.LevelMult,
                ConfigurationCalcMinLevel = dto.Configuration?.CalcMinLevel,
                ConfigurationCalcMaxLevel = dto.Configuration?.CalcMaxLevel,
                ConfigurationHealthOffset = dto.Configuration?.HealthOffset,
                ConfigurationSpeedMultiplier = dto.Configuration?.SpeedMultiplier,
                ConfigurationTemplateFlags = FormatStringList(dto.Configuration?.TemplateFlags),
                dto.UseTemplateActors,
                SkinModKeyName = dto.Skin?.ModKey.Name,
                SkinModKeyType = dto.Skin?.ModKey.Type,
                SkinModKeyFileName = dto.Skin?.ModKey.FileName,
                SkinFormKeyId = dto.Skin?.Id,
                AttackRaceModKeyName = dto.AttackRace?.ModKey.Name,
                AttackRaceModKeyType = dto.AttackRace?.ModKey.Type,
                AttackRaceModKeyFileName = dto.AttackRace?.ModKey.FileName,
                AttackRaceFormKeyId = dto.AttackRace?.Id,
                ClassModKeyName = dto.Class?.ModKey.Name,
                ClassModKeyType = dto.Class?.ModKey.Type,
                ClassModKeyFileName = dto.Class?.ModKey.FileName,
                ClassFormKeyId = dto.Class?.Id,
                DeathItemModKeyName = dto.DeathItem?.ModKey.Name,
                DeathItemModKeyType = dto.DeathItem?.ModKey.Type,
                DeathItemModKeyFileName = dto.DeathItem?.ModKey.FileName,
                DeathItemFormKeyId = dto.DeathItem?.Id,
                DefaultOutfitModKeyName = dto.DefaultOutfit?.ModKey.Name,
                DefaultOutfitModKeyType = dto.DefaultOutfit?.ModKey.Type,
                DefaultOutfitModKeyFileName = dto.DefaultOutfit?.ModKey.FileName,
                DefaultOutfitFormKeyId = dto.DefaultOutfit?.Id,
                SleepingOutfitModKeyName = dto.SleepingOutfit?.ModKey.Name,
                SleepingOutfitModKeyType = dto.SleepingOutfit?.ModKey.Type,
                SleepingOutfitModKeyFileName = dto.SleepingOutfit?.ModKey.FileName,
                SleepingOutfitFormKeyId = dto.SleepingOutfit?.Id,
                WornArmorModKeyName = dto.WornArmor?.ModKey.Name,
                WornArmorModKeyType = dto.WornArmor?.ModKey.Type,
                WornArmorModKeyFileName = dto.WornArmor?.ModKey.FileName,
                WornArmorFormKeyId = dto.WornArmor?.Id,
                PowerArmorStandModKeyName = dto.PowerArmorStand?.ModKey.Name,
                PowerArmorStandModKeyType = dto.PowerArmorStand?.ModKey.Type,
                PowerArmorStandModKeyFileName = dto.PowerArmorStand?.ModKey.FileName,
                PowerArmorStandFormKeyId = dto.PowerArmorStand?.Id,
                SpaceOutfitModKeyName = dto.SpaceOutfit?.ModKey.Name,
                SpaceOutfitModKeyType = dto.SpaceOutfit?.ModKey.Type,
                SpaceOutfitModKeyFileName = dto.SpaceOutfit?.ModKey.FileName,
                SpaceOutfitFormKeyId = dto.SpaceOutfit?.Id,
                HeadTextureModKeyName = dto.HeadTexture?.ModKey.Name,
                HeadTextureModKeyType = dto.HeadTexture?.ModKey.Type,
                HeadTextureModKeyFileName = dto.HeadTexture?.ModKey.FileName,
                HeadTextureFormKeyId = dto.HeadTexture?.Id,
                TemplateModKeyName = dto.Template?.ModKey.Name,
                TemplateModKeyType = dto.Template?.ModKey.Type,
                TemplateModKeyFileName = dto.Template?.ModKey.FileName,
                TemplateFormKeyId = dto.Template?.Id,
                DefaultTemplateModKeyName = dto.DefaultTemplate?.ModKey.Name,
                DefaultTemplateModKeyType = dto.DefaultTemplate?.ModKey.Type,
                DefaultTemplateModKeyFileName = dto.DefaultTemplate?.ModKey.FileName,
                DefaultTemplateFormKeyId = dto.DefaultTemplate?.Id,
                TemplateActorsTraitModKeyName = dto.TemplateActors?.TraitTemplate?.ModKey.Name,
                TemplateActorsTraitModKeyType = dto.TemplateActors?.TraitTemplate?.ModKey.Type,
                TemplateActorsTraitModKeyFileName = dto.TemplateActors?.TraitTemplate?.ModKey.FileName,
                TemplateActorsTraitFormKeyId = dto.TemplateActors?.TraitTemplate?.Id,
                TemplateActorsStatsModKeyName = dto.TemplateActors?.StatsTemplate?.ModKey.Name,
                TemplateActorsStatsModKeyType = dto.TemplateActors?.StatsTemplate?.ModKey.Type,
                TemplateActorsStatsModKeyFileName = dto.TemplateActors?.StatsTemplate?.ModKey.FileName,
                TemplateActorsStatsFormKeyId = dto.TemplateActors?.StatsTemplate?.Id,
                TemplateActorsFactionsModKeyName = dto.TemplateActors?.FactionsTemplate?.ModKey.Name,
                TemplateActorsFactionsModKeyType = dto.TemplateActors?.FactionsTemplate?.ModKey.Type,
                TemplateActorsFactionsModKeyFileName = dto.TemplateActors?.FactionsTemplate?.ModKey.FileName,
                TemplateActorsFactionsFormKeyId = dto.TemplateActors?.FactionsTemplate?.Id,
                TemplateActorsSpellListModKeyName = dto.TemplateActors?.SpellListTemplate?.ModKey.Name,
                TemplateActorsSpellListModKeyType = dto.TemplateActors?.SpellListTemplate?.ModKey.Type,
                TemplateActorsSpellListModKeyFileName = dto.TemplateActors?.SpellListTemplate?.ModKey.FileName,
                TemplateActorsSpellListFormKeyId = dto.TemplateActors?.SpellListTemplate?.Id,
                TemplateActorsAiPackagesModKeyName = dto.TemplateActors?.AiPackagesTemplate?.ModKey.Name,
                TemplateActorsAiPackagesModKeyType = dto.TemplateActors?.AiPackagesTemplate?.ModKey.Type,
                TemplateActorsAiPackagesModKeyFileName = dto.TemplateActors?.AiPackagesTemplate?.ModKey.FileName,
                TemplateActorsAiPackagesFormKeyId = dto.TemplateActors?.AiPackagesTemplate?.Id,
                TemplateActorsAiDataModKeyName = dto.TemplateActors?.AiDataTemplate?.ModKey.Name,
                TemplateActorsAiDataModKeyType = dto.TemplateActors?.AiDataTemplate?.ModKey.Type,
                TemplateActorsAiDataModKeyFileName = dto.TemplateActors?.AiDataTemplate?.ModKey.FileName,
                TemplateActorsAiDataFormKeyId = dto.TemplateActors?.AiDataTemplate?.Id,
                TemplateActorsBaseDataModKeyName = dto.TemplateActors?.BaseDataTemplate?.ModKey.Name,
                TemplateActorsBaseDataModKeyType = dto.TemplateActors?.BaseDataTemplate?.ModKey.Type,
                TemplateActorsBaseDataModKeyFileName = dto.TemplateActors?.BaseDataTemplate?.ModKey.FileName,
                TemplateActorsBaseDataFormKeyId = dto.TemplateActors?.BaseDataTemplate?.Id,
                TemplateActorsInventoryModKeyName = dto.TemplateActors?.InventoryTemplate?.ModKey.Name,
                TemplateActorsInventoryModKeyType = dto.TemplateActors?.InventoryTemplate?.ModKey.Type,
                TemplateActorsInventoryModKeyFileName = dto.TemplateActors?.InventoryTemplate?.ModKey.FileName,
                TemplateActorsInventoryFormKeyId = dto.TemplateActors?.InventoryTemplate?.Id,
                TemplateActorsScriptModKeyName = dto.TemplateActors?.ScriptTemplate?.ModKey.Name,
                TemplateActorsScriptModKeyType = dto.TemplateActors?.ScriptTemplate?.ModKey.Type,
                TemplateActorsScriptModKeyFileName = dto.TemplateActors?.ScriptTemplate?.ModKey.FileName,
                TemplateActorsScriptFormKeyId = dto.TemplateActors?.ScriptTemplate?.Id,
                TemplateActorsDefPackListModKeyName = dto.TemplateActors?.DefPackListTemplate?.ModKey.Name,
                TemplateActorsDefPackListModKeyType = dto.TemplateActors?.DefPackListTemplate?.ModKey.Type,
                TemplateActorsDefPackListModKeyFileName = dto.TemplateActors?.DefPackListTemplate?.ModKey.FileName,
                TemplateActorsDefPackListFormKeyId = dto.TemplateActors?.DefPackListTemplate?.Id,
                TemplateActorsAttackDataModKeyName = dto.TemplateActors?.AttackDataTemplate?.ModKey.Name,
                TemplateActorsAttackDataModKeyType = dto.TemplateActors?.AttackDataTemplate?.ModKey.Type,
                TemplateActorsAttackDataModKeyFileName = dto.TemplateActors?.AttackDataTemplate?.ModKey.FileName,
                TemplateActorsAttackDataFormKeyId = dto.TemplateActors?.AttackDataTemplate?.Id,
                TemplateActorsKeywordsModKeyName = dto.TemplateActors?.KeywordsTemplate?.ModKey.Name,
                TemplateActorsKeywordsModKeyType = dto.TemplateActors?.KeywordsTemplate?.ModKey.Type,
                TemplateActorsKeywordsModKeyFileName = dto.TemplateActors?.KeywordsTemplate?.ModKey.FileName,
                TemplateActorsKeywordsFormKeyId = dto.TemplateActors?.KeywordsTemplate?.Id,
                TemplateActorsUnknown1ModKeyName = dto.TemplateActors?.Unknown1?.ModKey.Name,
                TemplateActorsUnknown1ModKeyType = dto.TemplateActors?.Unknown1?.ModKey.Type,
                TemplateActorsUnknown1ModKeyFileName = dto.TemplateActors?.Unknown1?.ModKey.FileName,
                TemplateActorsUnknown1FormKeyId = dto.TemplateActors?.Unknown1?.Id,
                TemplateActorsUnknown2ModKeyName = dto.TemplateActors?.Unknown2?.ModKey.Name,
                TemplateActorsUnknown2ModKeyType = dto.TemplateActors?.Unknown2?.ModKey.Type,
                TemplateActorsUnknown2ModKeyFileName = dto.TemplateActors?.Unknown2?.ModKey.FileName,
                TemplateActorsUnknown2FormKeyId = dto.TemplateActors?.Unknown2?.Id,
                dto.CalculatedHealth,
                dto.CalculatedActionPoints,
                dto.XpValueOffset,
                dto.Unknown,
                dto.Unused,
                dto.NAM5,
                dto.Height,
                WeightValue = dto.Weight?.Value,
                WeightThin = dto.Weight?.Thin,
                WeightMuscular = dto.Weight?.Muscular,
                WeightFat = dto.Weight?.Fat,
                dto.SoundLevel,
                dto.TextureLighting,
                dto.HairColor,
                dto.FacialHairColor,
                dto.EyebrowColor,
                dto.EyeColor,
                FaceMorphNoseLongVsShort = dto.FaceMorph?.NoseLongVsShort,
                FaceMorphNoseUpVsDown = dto.FaceMorph?.NoseUpVsDown,
                FaceMorphJawUpVsDown = dto.FaceMorph?.JawUpVsDown,
                FaceMorphJawNarrowVsWide = dto.FaceMorph?.JawNarrowVsWide,
                FaceMorphJawForwardVsBack = dto.FaceMorph?.JawForwardVsBack,
                FaceMorphCheeksUpVsDown = dto.FaceMorph?.CheeksUpVsDown,
                FaceMorphCheeksForwardVsBack = dto.FaceMorph?.CheeksForwardVsBack,
                FaceMorphEyesUpVsDown = dto.FaceMorph?.EyesUpVsDown,
                FaceMorphEyesInVsOut = dto.FaceMorph?.EyesInVsOut,
                FaceMorphBrowsUpVsDown = dto.FaceMorph?.BrowsUpVsDown,
                FaceMorphBrowsInVsOut = dto.FaceMorph?.BrowsInVsOut,
                FaceMorphBrowsForwardVsBack = dto.FaceMorph?.BrowsForwardVsBack,
                FaceMorphLipsUpVsDown = dto.FaceMorph?.LipsUpVsDown,
                FaceMorphLipsInVsOut = dto.FaceMorph?.LipsInVsOut,
                FaceMorphChinNarrowVsWide = dto.FaceMorph?.ChinNarrowVsWide,
                FaceMorphChinUpVsDown = dto.FaceMorph?.ChinUpVsDown,
                FaceMorphChinUnderbiteVsOverbite = dto.FaceMorph?.ChinUnderbiteVsOverbite,
                FaceMorphEyesForwardVsBack = dto.FaceMorph?.EyesForwardVsBack,
                FaceMorphUnknown = dto.FaceMorph?.Unknown,
                FacePartsNose = dto.FaceParts?.Nose,
                FacePartsUnknown = dto.FaceParts?.Unknown,
                FacePartsEyes = dto.FaceParts?.Eyes,
                FacePartsMouth = dto.FaceParts?.Mouth,
                PlayerSkillsHealth = dto.PlayerSkills?.Health,
                PlayerSkillsMagicka = dto.PlayerSkills?.Magicka,
                PlayerSkillsStamina = dto.PlayerSkills?.Stamina,
                PlayerSkillsGearedUpWeapons = dto.PlayerSkills?.GearedUpWeapons
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
            IsCompressed = record.IsCompressed,
            ObjectBoundsFirst = record.ObjectBoundsFirst,
            ObjectBoundsSecond = record.ObjectBoundsSecond,
            Name = FromEnglish(record.Name),
            ShortName = FromEnglish(record.ShortName),
            LongName = FromEnglish(record.LongName),
            Flags = record.Flags,
            MajorFlags = record.MajorFlags,
            Level = CreateLevel(record.LevelMutagenObjectType, record.LevelLevel, record.LevelLevelMult),
            Configuration = CreateConfiguration(record),
            Version2 = record.Version2,
            VersionControl = record.VersionControl,
            DispositionBase = record.DispositionBase,
            UseTemplateActors = record.UseTemplateActors,
            Aggression = record.Aggression,
            Confidence = record.Confidence,
            EnergyLevel = record.EnergyLevel,
            Responsibility = record.Responsibility,
            Assistance = record.Assistance,
            Mood = record.Mood,
            GearedUpWeapons = record.GearedUpWeapons,
            HeightMin = record.HeightMin,
            HeightMax = record.HeightMax,
            SkinToneIndex = record.SkinToneIndex,
            Skin = CreateNullableFormKey(record.SkinModKeyName, record.SkinModKeyType, record.SkinModKeyFileName, record.SkinFormKeyId),
            Pronoun = record.Pronoun,
            VoiceFormKey = CreateNullableFormKey(record.VoiceModKeyName, record.VoiceModKeyType, record.VoiceModKeyFileName, record.VoiceFormKeyId),
            RaceFormKey = CreateNullableFormKey(record.RaceModKeyName, record.RaceModKeyType, record.RaceModKeyFileName, record.RaceFormKeyId),
            AttackRace = CreateNullableFormKey(record.AttackRaceModKeyName, record.AttackRaceModKeyType, record.AttackRaceModKeyFileName, record.AttackRaceFormKeyId),
            CombatOverridePackageListFormKey = CreateNullableFormKey(record.CombatOverridePackageListModKeyName, record.CombatOverridePackageListModKeyType, record.CombatOverridePackageListModKeyFileName, record.CombatOverridePackageListFormKeyId),
            CombatStyleFormKey = CreateNullableFormKey(record.CombatStyleModKeyName, record.CombatStyleModKeyType, record.CombatStyleModKeyFileName, record.CombatStyleFormKeyId),
            DefaultPackageListFormKey = CreateNullableFormKey(record.DefaultPackageListModKeyName, record.DefaultPackageListModKeyType, record.DefaultPackageListModKeyFileName, record.DefaultPackageListFormKeyId),
            CrimeFactionFormKey = CreateNullableFormKey(record.CrimeFactionModKeyName, record.CrimeFactionModKeyType, record.CrimeFactionModKeyFileName, record.CrimeFactionFormKeyId),
            Class = CreateNullableFormKey(record.ClassModKeyName, record.ClassModKeyType, record.ClassModKeyFileName, record.ClassFormKeyId),
            DeathItem = CreateNullableFormKey(record.DeathItemModKeyName, record.DeathItemModKeyType, record.DeathItemModKeyFileName, record.DeathItemFormKeyId),
            DefaultOutfit = CreateNullableFormKey(record.DefaultOutfitModKeyName, record.DefaultOutfitModKeyType, record.DefaultOutfitModKeyFileName, record.DefaultOutfitFormKeyId),
            SleepingOutfit = CreateNullableFormKey(record.SleepingOutfitModKeyName, record.SleepingOutfitModKeyType, record.SleepingOutfitModKeyFileName, record.SleepingOutfitFormKeyId),
            WornArmor = CreateNullableFormKey(record.WornArmorModKeyName, record.WornArmorModKeyType, record.WornArmorModKeyFileName, record.WornArmorFormKeyId),
            PowerArmorStand = CreateNullableFormKey(record.PowerArmorStandModKeyName, record.PowerArmorStandModKeyType, record.PowerArmorStandModKeyFileName, record.PowerArmorStandFormKeyId),
            SpaceOutfit = CreateNullableFormKey(record.SpaceOutfitModKeyName, record.SpaceOutfitModKeyType, record.SpaceOutfitModKeyFileName, record.SpaceOutfitFormKeyId),
            HeadTexture = CreateNullableFormKey(record.HeadTextureModKeyName, record.HeadTextureModKeyType, record.HeadTextureModKeyFileName, record.HeadTextureFormKeyId),
            Template = CreateNullableFormKey(record.TemplateModKeyName, record.TemplateModKeyType, record.TemplateModKeyFileName, record.TemplateFormKeyId),
            DefaultTemplate = CreateNullableFormKey(record.DefaultTemplateModKeyName, record.DefaultTemplateModKeyType, record.DefaultTemplateModKeyFileName, record.DefaultTemplateFormKeyId),
            TemplateActors = CreateTemplateActors(record),
            CalculatedHealth = record.CalculatedHealth,
            CalculatedActionPoints = record.CalculatedActionPoints,
            XpValueOffset = record.XpValueOffset,
            Unknown = record.Unknown,
            Unused = record.Unused,
            NAM5 = record.NAM5,
            Height = record.Height,
            Weight = CreateWeight(record),
            SoundLevel = record.SoundLevel,
            TextureLighting = record.TextureLighting,
            HairColor = record.HairColor,
            FacialHairColor = record.FacialHairColor,
            EyebrowColor = record.EyebrowColor,
            EyeColor = record.EyeColor,
            FaceMorph = CreateFaceMorph(record),
            FaceParts = CreateFaceParts(record),
            HeadParts = new List<CreationsForge.Core.DTOs.Plugins.FormKeyDTO>(),
            TintLayers = new List<NPCTintLayerDTO>(),
            FaceTintingLayers = new List<NPCFaceTintingLayerDTO>(),
            Tints = new List<NPCTintDTO>(),
            BodyMorphRegionValues = record.BodyMorphRegionValues,
            ObjectTemplates = record.ObjectTemplates,
            AIData = record.AIData,
            PlayerSkills = CreatePlayerSkills(record)
        };
        ApplyCommonFields(dto, record, game);
        return dto;
    }

    private IReadOnlyList<FormKeyDTO> FetchFormKeyList(SupportedGame game, FormKeyDTO formKey, string listName)
    {
        return Database.Fetch<NPCFormKeyListRow>(
                """
                SELECT
                    Target_ModKey_Name AS TargetModKeyName,
                    Target_ModKey_Type AS TargetModKeyType,
                    Target_ModKey_FileName AS TargetModKeyFileName,
                    Target_FormKey_ID AS TargetFormKeyId
                FROM NPCFormKeyLists
                WHERE Game = @Game
                  AND FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
                  AND FormKey_ModKey_Type = @FormKeyModKeyType
                  AND FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
                  AND FormKey_ID = @FormKeyId
                  AND ListName = @ListName
                ORDER BY Item_Index;
                """,
                new
                {
                    Game = game.ToString(),
                    FormKeyModKeyName = formKey.ModKey.Name,
                    FormKeyModKeyType = formKey.ModKey.Type,
                    FormKeyModKeyFileName = formKey.ModKey.FileName,
                    FormKeyId = formKey.Id,
                    ListName = listName
                })
            .Select(row => CreateNullableFormKey(row.TargetModKeyName, row.TargetModKeyType, row.TargetModKeyFileName, row.TargetFormKeyId))
            .Where(target => target != null)
            .Cast<FormKeyDTO>()
            .ToList();
    }

    private void ReplaceFormKeyList(NPCDTO dto, string listName, IList<FormKeyDTO> values)
    {
        Database.Execute(
            """
            DELETE FROM NPCFormKeyLists
            WHERE Game = @Game
              AND ModKey_Name = @ModKeyName
              AND ModKey_Type = @ModKeyType
              AND ModKey_FileName = @ModKeyFileName
              AND FormKey_ModKey_Name = @FormKeyModKeyName
              AND FormKey_ModKey_Type = @FormKeyModKeyType
              AND FormKey_ModKey_FileName = @FormKeyModKeyFileName
              AND FormKey_ID = @FormKeyId
              AND ListName = @ListName;
            """,
            CreateParentParameters(dto, new { ListName = listName }));

        for (var index = 0; index < values.Count; index++)
        {
            var value = values[index];
            Database.Execute(
                """
                INSERT OR REPLACE INTO NPCFormKeyLists (
                    Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                    ListName, Item_Index, Target_ModKey_Name, Target_ModKey_Type, Target_ModKey_FileName, Target_FormKey_ID, ImportedAtUTC)
                VALUES (
                    @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                    @ListName, @ItemIndex, @TargetModKeyName, @TargetModKeyType, @TargetModKeyFileName, @TargetFormKeyId, @ImportedAtUTC);
                """,
                CreateParentParameters(dto, new
                {
                    ListName = listName,
                    ItemIndex = index,
                    TargetModKeyName = value.ModKey.Name,
                    TargetModKeyType = value.ModKey.Type,
                    TargetModKeyFileName = value.ModKey.FileName,
                    TargetFormKeyId = value.Id
                }));
        }
    }

    private IList<NPCFactionDTO> FetchFactions(SupportedGame game, FormKeyDTO formKey)
    {
        return Database.Fetch<NPCFactionRow>(
                """
                SELECT
                    Game,
                    ModKey_Name AS ModKeyName,
                    ModKey_Type AS ModKeyType,
                    ModKey_FileName AS ModKeyFileName,
                    FormKey_ModKey_Name AS FormKeyModKeyName,
                    FormKey_ModKey_Type AS FormKeyModKeyType,
                    FormKey_ModKey_FileName AS FormKeyModKeyFileName,
                    FormKey_ID AS FormKeyId,
                    Faction_Index AS FactionIndex,
                    Faction_ModKey_Name AS FactionModKeyName,
                    Faction_ModKey_Type AS FactionModKeyType,
                    Faction_ModKey_FileName AS FactionModKeyFileName,
                    Faction_FormKey_ID AS FactionFormKeyId,
                    Rank,
                    Fluff,
                    ImportedAtUTC
                FROM NPCFactions
                WHERE Game = @Game
                  AND FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
                  AND FormKey_ModKey_Type = @FormKeyModKeyType
                  AND FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
                  AND FormKey_ID = @FormKeyId
                ORDER BY Faction_Index;
                """,
                CreateFormKeyParameters(game, formKey))
            .Select(row => new NPCFactionDTO
            {
                Game = game,
                ModKey = CreateModKey(row.ModKeyName, row.ModKeyType, row.ModKeyFileName),
                FormKey = formKey,
                FactionIndex = row.FactionIndex,
                Faction = CreateNullableFormKey(row.FactionModKeyName, row.FactionModKeyType, row.FactionModKeyFileName, row.FactionFormKeyId),
                Rank = row.Rank,
                Fluff = row.Fluff,
                ImportedAtUTC = row.ImportedAtUTC
            })
            .ToList();
    }

    private IList<NPCPropertyDTO> FetchProperties(SupportedGame game, FormKeyDTO formKey)
    {
        return Database.Fetch<NPCPropertyRow>(
                """
                SELECT
                    Game,
                    ModKey_Name AS ModKeyName,
                    ModKey_Type AS ModKeyType,
                    ModKey_FileName AS ModKeyFileName,
                    Property_Index AS PropertyIndex,
                    ActorValue_ModKey_Name AS ActorValueModKeyName,
                    ActorValue_ModKey_Type AS ActorValueModKeyType,
                    ActorValue_ModKey_FileName AS ActorValueModKeyFileName,
                    ActorValue_FormKey_ID AS ActorValueFormKeyId,
                    Value,
                    ImportedAtUTC
                FROM NPCProperties
                WHERE Game = @Game
                  AND FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
                  AND FormKey_ModKey_Type = @FormKeyModKeyType
                  AND FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
                  AND FormKey_ID = @FormKeyId
                ORDER BY Property_Index;
                """,
                CreateFormKeyParameters(game, formKey))
            .Select(row => new NPCPropertyDTO
            {
                Game = game,
                ModKey = CreateModKey(row.ModKeyName, row.ModKeyType, row.ModKeyFileName),
                FormKey = formKey,
                PropertyIndex = row.PropertyIndex,
                ActorValue = CreateNullableFormKey(row.ActorValueModKeyName, row.ActorValueModKeyType, row.ActorValueModKeyFileName, row.ActorValueFormKeyId),
                Value = row.Value,
                ImportedAtUTC = row.ImportedAtUTC
            })
            .ToList();
    }

    private IList<NPCItemDTO> FetchItems(SupportedGame game, FormKeyDTO formKey)
    {
        return Database.Fetch<NPCItemRow>(
                """
                SELECT
                    Game,
                    ModKey_Name AS ModKeyName,
                    ModKey_Type AS ModKeyType,
                    ModKey_FileName AS ModKeyFileName,
                    Item_Index AS ItemIndex,
                    Item_ModKey_Name AS ItemModKeyName,
                    Item_ModKey_Type AS ItemModKeyType,
                    Item_ModKey_FileName AS ItemModKeyFileName,
                    Item_FormKey_ID AS ItemFormKeyId,
                    Count,
                    ImportedAtUTC
                FROM NPCItems
                WHERE Game = @Game
                  AND FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
                  AND FormKey_ModKey_Type = @FormKeyModKeyType
                  AND FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
                  AND FormKey_ID = @FormKeyId
                ORDER BY Item_Index;
                """,
                CreateFormKeyParameters(game, formKey))
            .Select(row => new NPCItemDTO
            {
                Game = game,
                ModKey = CreateModKey(row.ModKeyName, row.ModKeyType, row.ModKeyFileName),
                FormKey = formKey,
                ItemIndex = row.ItemIndex,
                Item = CreateNullableFormKey(row.ItemModKeyName, row.ItemModKeyType, row.ItemModKeyFileName, row.ItemFormKeyId),
                Count = row.Count,
                ImportedAtUTC = row.ImportedAtUTC
            })
            .ToList();
    }

    private IList<NPCPerkDTO> FetchPerks(SupportedGame game, FormKeyDTO formKey)
    {
        return Database.Fetch<NPCPerkRow>(
                """
                SELECT
                    Game,
                    ModKey_Name AS ModKeyName,
                    ModKey_Type AS ModKeyType,
                    ModKey_FileName AS ModKeyFileName,
                    Perk_Index AS PerkIndex,
                    Perk_ModKey_Name AS PerkModKeyName,
                    Perk_ModKey_Type AS PerkModKeyType,
                    Perk_ModKey_FileName AS PerkModKeyFileName,
                    Perk_FormKey_ID AS PerkFormKeyId,
                    Rank,
                    Fluff,
                    ImportedAtUTC
                FROM NPCPerks
                WHERE Game = @Game
                  AND FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
                  AND FormKey_ModKey_Type = @FormKeyModKeyType
                  AND FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
                  AND FormKey_ID = @FormKeyId
                ORDER BY Perk_Index;
                """,
                CreateFormKeyParameters(game, formKey))
            .Select(row => new NPCPerkDTO
            {
                Game = game,
                ModKey = CreateModKey(row.ModKeyName, row.ModKeyType, row.ModKeyFileName),
                FormKey = formKey,
                PerkIndex = row.PerkIndex,
                Perk = CreateNullableFormKey(row.PerkModKeyName, row.PerkModKeyType, row.PerkModKeyFileName, row.PerkFormKeyId),
                Rank = row.Rank,
                Fluff = row.Fluff,
                ImportedAtUTC = row.ImportedAtUTC
            })
            .ToList();
    }

    private IList<NPCMorphDTO> FetchMorphs(SupportedGame game, FormKeyDTO formKey)
    {
        return Database.Fetch<NPCMorphRow>(
                """
                SELECT ModKey_Name AS ModKeyName, ModKey_Type AS ModKeyType, ModKey_FileName AS ModKeyFileName,
                       Morph_Index AS MorphIndex, Key, Value, ImportedAtUTC
                FROM NPCMorphs
                WHERE Game = @Game
                  AND FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
                  AND FormKey_ModKey_Type = @FormKeyModKeyType
                  AND FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
                  AND FormKey_ID = @FormKeyId
                ORDER BY Morph_Index;
                """,
                CreateFormKeyParameters(game, formKey))
            .Select(row => new NPCMorphDTO
            {
                Game = game,
                ModKey = CreateModKey(row.ModKeyName, row.ModKeyType, row.ModKeyFileName),
                FormKey = formKey,
                MorphIndex = row.MorphIndex,
                Key = row.Key,
                Value = row.Value,
                ImportedAtUTC = row.ImportedAtUTC
            })
            .ToList();
    }

    private IList<NPCFaceDialPositionDTO> FetchFaceDialPositions(SupportedGame game, FormKeyDTO formKey)
    {
        return Database.Fetch<NPCFaceDialPositionRow>(
                """
                SELECT ModKey_Name AS ModKeyName, ModKey_Type AS ModKeyType, ModKey_FileName AS ModKeyFileName,
                       FaceDialPosition_Index AS FaceDialPositionIndex, Source_Index AS SourceIndex, Position, ImportedAtUTC
                FROM NPCFaceDialPositions
                WHERE Game = @Game
                  AND FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
                  AND FormKey_ModKey_Type = @FormKeyModKeyType
                  AND FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
                  AND FormKey_ID = @FormKeyId
                ORDER BY FaceDialPosition_Index;
                """,
                CreateFormKeyParameters(game, formKey))
            .Select(row => new NPCFaceDialPositionDTO
            {
                Game = game,
                ModKey = CreateModKey(row.ModKeyName, row.ModKeyType, row.ModKeyFileName),
                FormKey = formKey,
                FaceDialPositionIndex = row.FaceDialPositionIndex,
                Index = row.SourceIndex,
                Position = row.Position,
                ImportedAtUTC = row.ImportedAtUTC
            })
            .ToList();
    }

    private IList<NPCFaceMorphPositionDTO> FetchFaceMorphPositions(SupportedGame game, FormKeyDTO formKey)
    {
        return Database.Fetch<NPCFaceMorphPositionRow>(
                """
                SELECT ModKey_Name AS ModKeyName, ModKey_Type AS ModKeyType, ModKey_FileName AS ModKeyFileName,
                       FaceMorph_Index AS FaceMorphIndex, Source_Index AS SourceIndex, Position, Rotation, Scale, ImportedAtUTC
                FROM NPCFaceMorphPositions
                WHERE Game = @Game
                  AND FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
                  AND FormKey_ModKey_Type = @FormKeyModKeyType
                  AND FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
                  AND FormKey_ID = @FormKeyId
                ORDER BY FaceMorph_Index;
                """,
                CreateFormKeyParameters(game, formKey))
            .Select(row => new NPCFaceMorphPositionDTO
            {
                Game = game,
                ModKey = CreateModKey(row.ModKeyName, row.ModKeyType, row.ModKeyFileName),
                FormKey = formKey,
                FaceMorphIndex = row.FaceMorphIndex,
                Index = row.SourceIndex,
                Position = row.Position,
                Rotation = row.Rotation,
                Scale = row.Scale,
                ImportedAtUTC = row.ImportedAtUTC
            })
            .ToList();
    }

    private IList<NPCFaceMorphGroupSetDTO> FetchFaceMorphGroups(SupportedGame game, FormKeyDTO formKey)
    {
        var groups = Database.Fetch<NPCFaceMorphGroupRow>(
                """
                SELECT ModKey_Name AS ModKeyName, ModKey_Type AS ModKeyType, ModKey_FileName AS ModKeyFileName,
                       FaceMorph_Index AS FaceMorphIndex, MorphGroup_Index AS MorphGroupIndex,
                       MorphGroup, BlendIntensity, ImportedAtUTC
                FROM NPCFaceMorphGroups
                WHERE Game = @Game
                  AND FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
                  AND FormKey_ModKey_Type = @FormKeyModKeyType
                  AND FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
                  AND FormKey_ID = @FormKeyId
                ORDER BY FaceMorph_Index, MorphGroup_Index;
                """,
                CreateFormKeyParameters(game, formKey))
            .GroupBy(row => row.FaceMorphIndex)
            .ToDictionary(group => group.Key, group => group
                .Select(row => new NPCFaceMorphGroupDTO
                {
                    Game = game,
                    ModKey = CreateModKey(row.ModKeyName, row.ModKeyType, row.ModKeyFileName),
                    FormKey = formKey,
                    FaceMorphIndex = row.FaceMorphIndex,
                    MorphGroupIndex = row.MorphGroupIndex,
                    MorphGroup = row.MorphGroup,
                    BlendIntensity = row.BlendIntensity,
                    ImportedAtUTC = row.ImportedAtUTC
                })
                .ToList());

        return Database.Fetch<NPCFaceMorphGroupSetRow>(
                """
                SELECT ModKey_Name AS ModKeyName, ModKey_Type AS ModKeyType, ModKey_FileName AS ModKeyFileName,
                       FaceMorph_Index AS FaceMorphIndex, Source_Index AS SourceIndex, ImportedAtUTC
                FROM NPCFaceMorphGroupSets
                WHERE Game = @Game
                  AND FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
                  AND FormKey_ModKey_Type = @FormKeyModKeyType
                  AND FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
                  AND FormKey_ID = @FormKeyId
                ORDER BY FaceMorph_Index;
                """,
                CreateFormKeyParameters(game, formKey))
            .Select(row => new NPCFaceMorphGroupSetDTO
            {
                Game = game,
                ModKey = CreateModKey(row.ModKeyName, row.ModKeyType, row.ModKeyFileName),
                FormKey = formKey,
                FaceMorphIndex = row.FaceMorphIndex,
                Index = row.SourceIndex,
                MorphGroups = groups.TryGetValue(row.FaceMorphIndex, out var rows) ? rows : new List<NPCFaceMorphGroupDTO>(),
                ImportedAtUTC = row.ImportedAtUTC
            })
            .ToList();
    }

    private IList<NPCMorphBlendDTO> FetchMorphBlends(SupportedGame game, FormKeyDTO formKey)
    {
        return Database.Fetch<NPCMorphBlendRow>(
                """
                SELECT ModKey_Name AS ModKeyName, ModKey_Type AS ModKeyType, ModKey_FileName AS ModKeyFileName,
                       MorphBlend_Index AS MorphBlendIndex, BlendName, Intensity, ImportedAtUTC
                FROM NPCMorphBlends
                WHERE Game = @Game
                  AND FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
                  AND FormKey_ModKey_Type = @FormKeyModKeyType
                  AND FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
                  AND FormKey_ID = @FormKeyId
                ORDER BY MorphBlend_Index;
                """,
                CreateFormKeyParameters(game, formKey))
            .Select(row => new NPCMorphBlendDTO
            {
                Game = game,
                ModKey = CreateModKey(row.ModKeyName, row.ModKeyType, row.ModKeyFileName),
                FormKey = formKey,
                MorphBlendIndex = row.MorphBlendIndex,
                BlendName = row.BlendName,
                Intensity = row.Intensity,
                ImportedAtUTC = row.ImportedAtUTC
            })
            .ToList();
    }

    private IList<NPCTintDTO> FetchTints(SupportedGame game, FormKeyDTO formKey)
    {
        return Database.Fetch<NPCTintRow>(
                """
                SELECT ModKey_Name AS ModKeyName, ModKey_Type AS ModKeyType, ModKey_FileName AS ModKeyFileName,
                       Tint_Index AS TintIndex, TintType, TintGroup, TintName, TintTexture, TintColor, TintIntensity, ImportedAtUTC
                FROM NPCTints
                WHERE Game = @Game
                  AND FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
                  AND FormKey_ModKey_Type = @FormKeyModKeyType
                  AND FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
                  AND FormKey_ID = @FormKeyId
                ORDER BY Tint_Index;
                """,
                CreateFormKeyParameters(game, formKey))
            .Select(row => new NPCTintDTO
            {
                Game = game,
                ModKey = CreateModKey(row.ModKeyName, row.ModKeyType, row.ModKeyFileName),
                FormKey = formKey,
                TintIndex = row.TintIndex,
                TintType = row.TintType,
                TintGroup = row.TintGroup,
                TintName = row.TintName,
                TintTexture = row.TintTexture,
                TintColor = row.TintColor,
                TintIntensity = row.TintIntensity,
                ImportedAtUTC = row.ImportedAtUTC
            })
            .ToList();
    }

    private IList<NPCTintLayerDTO> FetchTintLayers(SupportedGame game, FormKeyDTO formKey)
    {
        return Database.Fetch<NPCTintLayerRow>(
                """
                SELECT ModKey_Name AS ModKeyName, ModKey_Type AS ModKeyType, ModKey_FileName AS ModKeyFileName,
                       TintLayer_Index AS TintLayerIndex, Source_Index AS SourceIndex, Color, InterpolationValue, Preset, ImportedAtUTC
                FROM NPCTintLayers
                WHERE Game = @Game
                  AND FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
                  AND FormKey_ModKey_Type = @FormKeyModKeyType
                  AND FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
                  AND FormKey_ID = @FormKeyId
                ORDER BY TintLayer_Index;
                """,
                CreateFormKeyParameters(game, formKey))
            .Select(row => new NPCTintLayerDTO
            {
                Game = game,
                ModKey = CreateModKey(row.ModKeyName, row.ModKeyType, row.ModKeyFileName),
                FormKey = formKey,
                TintLayerIndex = row.TintLayerIndex,
                Index = row.SourceIndex,
                Color = row.Color,
                InterpolationValue = row.InterpolationValue,
                Preset = row.Preset,
                ImportedAtUTC = row.ImportedAtUTC
            })
            .ToList();
    }

    /// <summary>
    /// Fetches Fallout 4 face tinting layers and their nested state flags for one NPC form key.
    /// </summary>
    /// <param name="game">The game whose imported rows should be read.</param>
    /// <param name="formKey">The parent NPC form key to load.</param>
    /// <returns>The persisted face tinting layer rows, ordered by their source collection index.</returns>
    private IList<NPCFaceTintingLayerDTO> FetchFaceTintingLayers(SupportedGame game, FormKeyDTO formKey)
    {
        var states = Database.Fetch<NPCFaceTintingLayerStateRow>(
                """
                SELECT FaceTintingLayer_Index AS FaceTintingLayerIndex, State_Index AS StateIndex, State
                FROM NPCFaceTintingLayerStates
                WHERE Game = @Game
                  AND FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
                  AND FormKey_ModKey_Type = @FormKeyModKeyType
                  AND FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
                  AND FormKey_ID = @FormKeyId
                ORDER BY FaceTintingLayer_Index, State_Index;
                """,
                CreateFormKeyParameters(game, formKey))
            .GroupBy(row => row.FaceTintingLayerIndex)
            .ToDictionary(
                group => group.Key,
                group => group.OrderBy(row => row.StateIndex).Select(row => row.State).ToList());

        return Database.Fetch<NPCFaceTintingLayerRow>(
                """
                SELECT ModKey_Name AS ModKeyName, ModKey_Type AS ModKeyType, ModKey_FileName AS ModKeyFileName,
                       FaceTintingLayer_Index AS FaceTintingLayerIndex, DataType, Source_Index AS SourceIndex, Value, Color, TemplateColorIndex, ImportedAtUTC
                FROM NPCFaceTintingLayers
                WHERE Game = @Game
                  AND FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
                  AND FormKey_ModKey_Type = @FormKeyModKeyType
                  AND FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
                  AND FormKey_ID = @FormKeyId
                ORDER BY FaceTintingLayer_Index;
                """,
                CreateFormKeyParameters(game, formKey))
            .Select(row => new NPCFaceTintingLayerDTO
            {
                Game = game,
                ModKey = CreateModKey(row.ModKeyName, row.ModKeyType, row.ModKeyFileName),
                FormKey = formKey,
                FaceTintingLayerIndex = row.FaceTintingLayerIndex,
                DataType = row.DataType,
                Index = row.SourceIndex,
                Value = row.Value,
                Color = row.Color,
                TemplateColorIndex = row.TemplateColorIndex,
                TENDDataTypeState = states.TryGetValue(row.FaceTintingLayerIndex, out var layerStates) ? layerStates : new List<string>(),
                ImportedAtUTC = row.ImportedAtUTC
            })
            .ToList();
    }

    private NPCPlayerSkillsDTO? FetchPlayerSkills(SupportedGame game, FormKeyDTO formKey, NPCPlayerSkillsDTO? playerSkills)
    {
        var rows = Database.Fetch<NPCPlayerSkillValueRow>(
                """
                SELECT ValueListName, Skill_Index AS SkillIndex, SkillKey, SkillValue
                FROM NPCPlayerSkillValues
                WHERE Game = @Game
                  AND FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
                  AND FormKey_ModKey_Type = @FormKeyModKeyType
                  AND FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
                  AND FormKey_ID = @FormKeyId
                ORDER BY ValueListName, Skill_Index;
                """,
                CreateFormKeyParameters(game, formKey));

        if (playerSkills == null && rows.Count == 0)
        {
            return null;
        }

        playerSkills ??= new NPCPlayerSkillsDTO();
        playerSkills.SkillValues = rows
            .Where(row => string.Equals(row.ValueListName, "SkillValues", StringComparison.Ordinal))
            .Select(CreatePlayerSkillValue)
            .ToList();
        playerSkills.SkillOffsets = rows
            .Where(row => string.Equals(row.ValueListName, "SkillOffsets", StringComparison.Ordinal))
            .Select(CreatePlayerSkillValue)
            .ToList();
        return playerSkills;
    }

    private void ReplaceFactions(NPCDTO dto)
    {
        DeleteChildRows(dto, "NPCFactions");
        foreach (var faction in dto.Factions)
        {
            Database.Execute(
                """
                INSERT OR REPLACE INTO NPCFactions (
                    Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                    Faction_Index, Faction_ModKey_Name, Faction_ModKey_Type, Faction_ModKey_FileName, Faction_FormKey_ID, Rank, Fluff, ImportedAtUTC)
                VALUES (
                    @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                    @FactionIndex, @FactionModKeyName, @FactionModKeyType, @FactionModKeyFileName, @FactionFormKeyId, @Rank, @Fluff, @ImportedAtUTC);
                """,
                CreateParentParameters(dto, new
                {
                    FactionIndex = faction.FactionIndex,
                    FactionModKeyName = faction.Faction?.ModKey.Name,
                    FactionModKeyType = faction.Faction?.ModKey.Type,
                    FactionModKeyFileName = faction.Faction?.ModKey.FileName,
                    FactionFormKeyId = faction.Faction?.Id,
                    Rank = faction.Rank,
                    Fluff = faction.Fluff
                }));
        }
    }

    private void ReplaceProperties(NPCDTO dto)
    {
        DeleteChildRows(dto, "NPCProperties");
        foreach (var property in dto.Properties)
        {
            Database.Execute(
                """
                INSERT OR REPLACE INTO NPCProperties (
                    Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                    Property_Index, ActorValue_ModKey_Name, ActorValue_ModKey_Type, ActorValue_ModKey_FileName, ActorValue_FormKey_ID, Value, ImportedAtUTC)
                VALUES (
                    @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                    @PropertyIndex, @ActorValueModKeyName, @ActorValueModKeyType, @ActorValueModKeyFileName, @ActorValueFormKeyId, @Value, @ImportedAtUTC);
                """,
                CreateParentParameters(dto, new
                {
                    PropertyIndex = property.PropertyIndex,
                    ActorValueModKeyName = property.ActorValue?.ModKey.Name,
                    ActorValueModKeyType = property.ActorValue?.ModKey.Type,
                    ActorValueModKeyFileName = property.ActorValue?.ModKey.FileName,
                    ActorValueFormKeyId = property.ActorValue?.Id,
                    Value = property.Value
                }));
        }
    }

    private void ReplaceItems(NPCDTO dto)
    {
        DeleteChildRows(dto, "NPCItems");
        foreach (var item in dto.Items)
        {
            Database.Execute(
                """
                INSERT OR REPLACE INTO NPCItems (
                    Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                    Item_Index, Item_ModKey_Name, Item_ModKey_Type, Item_ModKey_FileName, Item_FormKey_ID, Count, ImportedAtUTC)
                VALUES (
                    @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                    @ItemIndex, @ItemModKeyName, @ItemModKeyType, @ItemModKeyFileName, @ItemFormKeyId, @Count, @ImportedAtUTC);
                """,
                CreateParentParameters(dto, new
                {
                    ItemIndex = item.ItemIndex,
                    ItemModKeyName = item.Item?.ModKey.Name,
                    ItemModKeyType = item.Item?.ModKey.Type,
                    ItemModKeyFileName = item.Item?.ModKey.FileName,
                    ItemFormKeyId = item.Item?.Id,
                    Count = item.Count
                }));
        }
    }

    private void ReplacePerks(NPCDTO dto)
    {
        DeleteChildRows(dto, "NPCPerks");
        foreach (var perk in dto.Perks)
        {
            Database.Execute(
                """
                INSERT OR REPLACE INTO NPCPerks (
                    Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                    Perk_Index, Perk_ModKey_Name, Perk_ModKey_Type, Perk_ModKey_FileName, Perk_FormKey_ID, Rank, Fluff, ImportedAtUTC)
                VALUES (
                    @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                    @PerkIndex, @PerkModKeyName, @PerkModKeyType, @PerkModKeyFileName, @PerkFormKeyId, @Rank, @Fluff, @ImportedAtUTC);
                """,
                CreateParentParameters(dto, new
                {
                    PerkIndex = perk.PerkIndex,
                    PerkModKeyName = perk.Perk?.ModKey.Name,
                    PerkModKeyType = perk.Perk?.ModKey.Type,
                    PerkModKeyFileName = perk.Perk?.ModKey.FileName,
                    PerkFormKeyId = perk.Perk?.Id,
                    Rank = perk.Rank,
                    Fluff = perk.Fluff
                }));
        }
    }

    private void ReplaceMorphs(NPCDTO dto)
    {
        DeleteChildRows(dto, "NPCMorphs");
        foreach (var morph in dto.Morphs)
        {
            Database.Execute(
                """
                INSERT OR REPLACE INTO NPCMorphs (
                    Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                    Morph_Index, Key, Value, ImportedAtUTC)
                VALUES (
                    @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                    @MorphIndex, @Key, @Value, @ImportedAtUTC);
                """,
                CreateParentParameters(dto, new
                {
                    MorphIndex = morph.MorphIndex,
                    Key = morph.Key,
                    Value = morph.Value
                }));
        }
    }

    private void ReplaceFaceDialPositions(NPCDTO dto)
    {
        DeleteChildRows(dto, "NPCFaceDialPositions");
        foreach (var position in dto.FaceDialPositions)
        {
            Database.Execute(
                """
                INSERT OR REPLACE INTO NPCFaceDialPositions (
                    Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                    FaceDialPosition_Index, Source_Index, Position, ImportedAtUTC)
                VALUES (
                    @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                    @FaceDialPositionIndex, @SourceIndex, @Position, @ImportedAtUTC);
                """,
                CreateParentParameters(dto, new
                {
                    FaceDialPositionIndex = position.FaceDialPositionIndex,
                    SourceIndex = position.Index,
                    Position = position.Position
                }));
        }
    }

    private void ReplaceFaceMorphPositions(NPCDTO dto)
    {
        DeleteChildRows(dto, "NPCFaceMorphPositions");
        foreach (var position in dto.FaceMorphs)
        {
            Database.Execute(
                """
                INSERT OR REPLACE INTO NPCFaceMorphPositions (
                    Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                    FaceMorph_Index, Source_Index, Position, Rotation, Scale, ImportedAtUTC)
                VALUES (
                    @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                    @FaceMorphIndex, @SourceIndex, @Position, @Rotation, @Scale, @ImportedAtUTC);
                """,
                CreateParentParameters(dto, new
                {
                    FaceMorphIndex = position.FaceMorphIndex,
                    SourceIndex = position.Index,
                    Position = position.Position,
                    Rotation = position.Rotation,
                    Scale = position.Scale
                }));
        }

        DeleteChildRows(dto, "NPCFaceMorphGroups");
        DeleteChildRows(dto, "NPCFaceMorphGroupSets");
        foreach (var set in dto.FaceMorphGroups)
        {
            Database.Execute(
                """
                INSERT OR REPLACE INTO NPCFaceMorphGroupSets (
                    Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                    FaceMorph_Index, Source_Index, ImportedAtUTC)
                VALUES (
                    @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                    @FaceMorphIndex, @SourceIndex, @ImportedAtUTC);
                """,
                CreateParentParameters(dto, new
                {
                    FaceMorphIndex = set.FaceMorphIndex,
                    SourceIndex = set.Index
                }));

            foreach (var group in set.MorphGroups)
            {
                Database.Execute(
                    """
                    INSERT OR REPLACE INTO NPCFaceMorphGroups (
                        Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                        FaceMorph_Index, MorphGroup_Index, MorphGroup, BlendIntensity, ImportedAtUTC)
                    VALUES (
                        @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                        @FaceMorphIndex, @MorphGroupIndex, @MorphGroup, @BlendIntensity, @ImportedAtUTC);
                    """,
                    CreateParentParameters(dto, new
                    {
                        FaceMorphIndex = group.FaceMorphIndex,
                        MorphGroupIndex = group.MorphGroupIndex,
                        MorphGroup = group.MorphGroup,
                        BlendIntensity = group.BlendIntensity
                    }));
            }
        }
    }

    private void ReplaceMorphBlends(NPCDTO dto)
    {
        DeleteChildRows(dto, "NPCMorphBlends");
        foreach (var blend in dto.MorphBlends)
        {
            Database.Execute(
                """
                INSERT OR REPLACE INTO NPCMorphBlends (
                    Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                    MorphBlend_Index, BlendName, Intensity, ImportedAtUTC)
                VALUES (
                    @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                    @MorphBlendIndex, @BlendName, @Intensity, @ImportedAtUTC);
                """,
                CreateParentParameters(dto, new
                {
                    MorphBlendIndex = blend.MorphBlendIndex,
                    BlendName = blend.BlendName,
                    Intensity = blend.Intensity
                }));
        }
    }

    private void ReplaceTints(NPCDTO dto)
    {
        DeleteChildRows(dto, "NPCTints");
        foreach (var tint in dto.Tints)
        {
            Database.Execute(
                """
                INSERT OR REPLACE INTO NPCTints (
                    Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                    Tint_Index, TintType, TintGroup, TintName, TintTexture, TintColor, TintIntensity, ImportedAtUTC)
                VALUES (
                    @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                    @TintIndex, @TintType, @TintGroup, @TintName, @TintTexture, @TintColor, @TintIntensity, @ImportedAtUTC);
                """,
                CreateParentParameters(dto, new
                {
                    TintIndex = tint.TintIndex,
                    TintType = tint.TintType,
                    TintGroup = tint.TintGroup,
                    TintName = tint.TintName,
                    TintTexture = tint.TintTexture,
                    TintColor = tint.TintColor,
                    TintIntensity = tint.TintIntensity
                }));
        }
    }

    private void ReplaceTintLayers(NPCDTO dto)
    {
        DeleteChildRows(dto, "NPCTintLayers");
        foreach (var layer in dto.TintLayers)
        {
            Database.Execute(
                """
                INSERT OR REPLACE INTO NPCTintLayers (
                    Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                    TintLayer_Index, Source_Index, Color, InterpolationValue, Preset, ImportedAtUTC)
                VALUES (
                    @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                    @TintLayerIndex, @SourceIndex, @Color, @InterpolationValue, @Preset, @ImportedAtUTC);
                """,
                CreateParentParameters(dto, new
                {
                    TintLayerIndex = layer.TintLayerIndex,
                    SourceIndex = layer.Index,
                    Color = layer.Color,
                    InterpolationValue = layer.InterpolationValue,
                    Preset = layer.Preset
                }));
        }
    }

    /// <summary>
    /// Replaces all persisted Fallout 4 face tinting layer rows for an NPC with the DTO's current collection.
    /// </summary>
    /// <param name="dto">The parent NPC whose face tinting layers should replace existing rows.</param>
    private void ReplaceFaceTintingLayers(NPCDTO dto)
    {
        DeleteChildRows(dto, "NPCFaceTintingLayerStates");
        DeleteChildRows(dto, "NPCFaceTintingLayers");
        foreach (var layer in dto.FaceTintingLayers)
        {
            Database.Execute(
                """
                INSERT OR REPLACE INTO NPCFaceTintingLayers (
                    Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                    FaceTintingLayer_Index, DataType, Source_Index, Value, Color, TemplateColorIndex, ImportedAtUTC)
                VALUES (
                    @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                    @FaceTintingLayerIndex, @DataType, @SourceIndex, @Value, @Color, @TemplateColorIndex, @ImportedAtUTC);
                """,
                CreateParentParameters(dto, new
                {
                    FaceTintingLayerIndex = layer.FaceTintingLayerIndex,
                    layer.DataType,
                    SourceIndex = layer.Index,
                    layer.Value,
                    layer.Color,
                    layer.TemplateColorIndex
                }));

            for (var stateIndex = 0; stateIndex < layer.TENDDataTypeState.Count; stateIndex++)
            {
                Database.Execute(
                    """
                    INSERT OR REPLACE INTO NPCFaceTintingLayerStates (
                        Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                        FaceTintingLayer_Index, State_Index, State, ImportedAtUTC)
                    VALUES (
                        @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                        @FaceTintingLayerIndex, @StateIndex, @State, @ImportedAtUTC);
                    """,
                    CreateParentParameters(dto, new
                    {
                        FaceTintingLayerIndex = layer.FaceTintingLayerIndex,
                        StateIndex = stateIndex,
                        State = layer.TENDDataTypeState[stateIndex]
                    }));
            }
        }
    }

    private void ReplacePlayerSkills(NPCDTO dto)
    {
        DeleteChildRows(dto, "NPCPlayerSkillValues");
        if (dto.PlayerSkills == null)
        {
            return;
        }

        ReplacePlayerSkillValues(dto, "SkillValues", dto.PlayerSkills.SkillValues);
        ReplacePlayerSkillValues(dto, "SkillOffsets", dto.PlayerSkills.SkillOffsets);
    }

    private void ReplacePlayerSkillValues(NPCDTO dto, string valueListName, IList<NPCPlayerSkillValueDTO> values)
    {
        foreach (var value in values)
        {
            Database.Execute(
                """
                INSERT OR REPLACE INTO NPCPlayerSkillValues (
                    Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                    ValueListName, Skill_Index, SkillKey, SkillValue, ImportedAtUTC)
                VALUES (
                    @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                    @ValueListName, @SkillIndex, @SkillKey, @SkillValue, @ImportedAtUTC);
                """,
                CreateParentParameters(dto, new
                {
                    ValueListName = valueListName,
                    SkillIndex = value.SkillIndex,
                    SkillKey = value.Key,
                    SkillValue = value.Value
                }));
        }
    }

    private static void ApplyLocalizedStrings(NPCDTO record, IReadOnlyList<LocalizedStringDTO> localizedStrings)
    {
        record.LocalizedStrings = localizedStrings.ToList();
        record.Name = BuildTranslatedString(localizedStrings, nameof(NPCDTO.Name), record.Name);
        record.ShortName = BuildTranslatedString(localizedStrings, nameof(NPCDTO.ShortName), record.ShortName);
        record.LongName = BuildTranslatedString(localizedStrings, nameof(NPCDTO.LongName), record.LongName);
    }

    /// <summary>
    /// Creates the shared parent-key parameters used by NPC child-table persistence, optionally adding child
    /// row values while preserving null values as explicit database nulls for NPoco parameter binding.
    /// </summary>
    /// <param name="dto">The NPC record whose identity columns identify the parent row.</param>
    /// <param name="extra">An optional object containing child-row parameter values to merge into the result.</param>
    /// <returns>A parameter dictionary containing parent identity values, import time, and any child-row values.</returns>
    private static Dictionary<string, object?> CreateParentParameters(NPCDTO dto, object? extra = null)
    {
        var values = new Dictionary<string, object?>
        {
            ["Game"] = dto.Game.ToString(),
            ["ModKeyName"] = dto.ModKey.Name,
            ["ModKeyType"] = dto.ModKey.Type,
            ["ModKeyFileName"] = dto.ModKey.FileName,
            ["FormKeyModKeyName"] = dto.FormKey.ModKey.Name,
            ["FormKeyModKeyType"] = dto.FormKey.ModKey.Type,
            ["FormKeyModKeyFileName"] = dto.FormKey.ModKey.FileName,
            ["FormKeyId"] = dto.FormKey.Id,
            ["ImportedAtUTC"] = dto.ImportedAtUTC
        };

        if (extra != null)
        {
            foreach (var property in extra.GetType().GetProperties())
            {
                values[property.Name] = property.GetValue(extra) ?? DBNull.Value;
            }
        }

        return values;
    }

    private static Dictionary<string, object?> CreateFormKeyParameters(SupportedGame game, FormKeyDTO formKey)
    {
        return new Dictionary<string, object?>
        {
            ["Game"] = game.ToString(),
            ["FormKeyModKeyName"] = formKey.ModKey.Name,
            ["FormKeyModKeyType"] = formKey.ModKey.Type,
            ["FormKeyModKeyFileName"] = formKey.ModKey.FileName,
            ["FormKeyId"] = formKey.Id
        };
    }

    private static NPCPlayerSkillValueDTO CreatePlayerSkillValue(NPCPlayerSkillValueRow row)
    {
        return new NPCPlayerSkillValueDTO
        {
            SkillIndex = row.SkillIndex,
            Key = row.SkillKey,
            Value = row.SkillValue
        };
    }

    private void DeleteChildRows(NPCDTO dto, string tableName)
    {
        var validTableName = ValidateNPCChildTableName(tableName);
        Database.Execute(
            $"""
            DELETE FROM {validTableName}
            WHERE Game = @Game
              AND ModKey_Name = @ModKeyName
              AND ModKey_Type = @ModKeyType
              AND ModKey_FileName = @ModKeyFileName
              AND FormKey_ModKey_Name = @FormKeyModKeyName
              AND FormKey_ModKey_Type = @FormKeyModKeyType
              AND FormKey_ModKey_FileName = @FormKeyModKeyFileName
              AND FormKey_ID = @FormKeyId;
            """,
            CreateParentParameters(dto));
    }

    private static string ValidateNPCChildTableName(string tableName)
    {
        return tableName is "NPCFactions" or
            "NPCProperties" or
            "NPCItems" or
            "NPCPerks" or
            "NPCMorphs" or
            "NPCFaceDialPositions" or
            "NPCFaceMorphPositions" or
            "NPCFaceMorphGroupSets" or
            "NPCFaceMorphGroups" or
            "NPCMorphBlends" or
            "NPCTints" or
            "NPCTintLayers" or
            "NPCFaceTintingLayers" or
            "NPCFaceTintingLayerStates" or
            "NPCPlayerSkillValues"
            ? tableName
            : throw new InvalidOperationException("Unexpected NPC child table '" + tableName + "'.");
    }

    private static ModKeyDTO CreateModKey(string? name, int? type, string? fileName)
    {
        return new ModKeyDTO
        {
            Name = name ?? string.Empty,
            Type = type ?? 0,
            FileName = fileName ?? string.Empty
        };
    }

    private static NPCLevelDTO? CreateLevel(string? mutagenObjectType, int? level, double? levelMult)
    {
        return mutagenObjectType == null && level == null && levelMult == null
            ? null
            : new NPCLevelDTO
            {
                MutagenObjectType = mutagenObjectType,
                Level = level,
                LevelMult = levelMult
            };
    }

    private static string? FormatFormKey(FormKeyDTO? formKey)
    {
        return formKey == null
            ? null
            : formKey.Id.ToString("X6", System.Globalization.CultureInfo.InvariantCulture) + ":" + formKey.ModKey.FileName;
    }

    private static NPCConfigurationDTO? CreateConfiguration(NPCRow record)
    {
        var level = CreateLevel(record.ConfigurationLevelMutagenObjectType, record.ConfigurationLevelLevel, record.ConfigurationLevelLevelMult);
        return record.ConfigurationFlags == null &&
               level == null &&
               record.ConfigurationCalcMinLevel == null &&
               record.ConfigurationCalcMaxLevel == null &&
               record.ConfigurationHealthOffset == null &&
               record.ConfigurationSpeedMultiplier == null &&
               record.ConfigurationTemplateFlags == null
            ? null
            : new NPCConfigurationDTO
            {
                Flags = ParseStringList(record.ConfigurationFlags),
                Level = level,
                CalcMinLevel = record.ConfigurationCalcMinLevel,
                CalcMaxLevel = record.ConfigurationCalcMaxLevel,
                HealthOffset = record.ConfigurationHealthOffset,
                SpeedMultiplier = record.ConfigurationSpeedMultiplier,
                TemplateFlags = ParseStringList(record.ConfigurationTemplateFlags)
            };
    }

    /// <summary>
    /// Formats a stored string-list column using the comma-delimited form already used by named flag-list columns.
    /// </summary>
    /// <param name="values">The ordered values to format.</param>
    /// <returns>The comma-delimited value, or <c>null</c> when the list is empty or absent.</returns>
    private static string? FormatStringList(IEnumerable<string>? values)
    {
        if (values == null)
        {
            return null;
        }

        var materializedValues = values.Where(value => !string.IsNullOrWhiteSpace(value)).ToList();
        return materializedValues.Count == 0
            ? null
            : string.Join(", ", materializedValues);
    }

    /// <summary>
    /// Parses a stored comma-delimited flag-list column back into ordered DTO items.
    /// </summary>
    /// <param name="value">The stored comma-delimited column value.</param>
    /// <returns>The ordered flag names, or an empty list when the column is absent.</returns>
    private static IList<string> ParseStringList(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? new List<string>()
            : value.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList();
    }

    private static NPCTemplateActorsDTO? CreateTemplateActors(NPCRow record)
    {
        var templateActors = new NPCTemplateActorsDTO
        {
            TraitTemplate = CreateNullableFormKey(record.TemplateActorsTraitModKeyName, record.TemplateActorsTraitModKeyType, record.TemplateActorsTraitModKeyFileName, record.TemplateActorsTraitFormKeyId),
            StatsTemplate = CreateNullableFormKey(record.TemplateActorsStatsModKeyName, record.TemplateActorsStatsModKeyType, record.TemplateActorsStatsModKeyFileName, record.TemplateActorsStatsFormKeyId),
            FactionsTemplate = CreateNullableFormKey(record.TemplateActorsFactionsModKeyName, record.TemplateActorsFactionsModKeyType, record.TemplateActorsFactionsModKeyFileName, record.TemplateActorsFactionsFormKeyId),
            SpellListTemplate = CreateNullableFormKey(record.TemplateActorsSpellListModKeyName, record.TemplateActorsSpellListModKeyType, record.TemplateActorsSpellListModKeyFileName, record.TemplateActorsSpellListFormKeyId),
            AiPackagesTemplate = CreateNullableFormKey(record.TemplateActorsAiPackagesModKeyName, record.TemplateActorsAiPackagesModKeyType, record.TemplateActorsAiPackagesModKeyFileName, record.TemplateActorsAiPackagesFormKeyId),
            AiDataTemplate = CreateNullableFormKey(record.TemplateActorsAiDataModKeyName, record.TemplateActorsAiDataModKeyType, record.TemplateActorsAiDataModKeyFileName, record.TemplateActorsAiDataFormKeyId),
            BaseDataTemplate = CreateNullableFormKey(record.TemplateActorsBaseDataModKeyName, record.TemplateActorsBaseDataModKeyType, record.TemplateActorsBaseDataModKeyFileName, record.TemplateActorsBaseDataFormKeyId),
            InventoryTemplate = CreateNullableFormKey(record.TemplateActorsInventoryModKeyName, record.TemplateActorsInventoryModKeyType, record.TemplateActorsInventoryModKeyFileName, record.TemplateActorsInventoryFormKeyId),
            ScriptTemplate = CreateNullableFormKey(record.TemplateActorsScriptModKeyName, record.TemplateActorsScriptModKeyType, record.TemplateActorsScriptModKeyFileName, record.TemplateActorsScriptFormKeyId),
            DefPackListTemplate = CreateNullableFormKey(record.TemplateActorsDefPackListModKeyName, record.TemplateActorsDefPackListModKeyType, record.TemplateActorsDefPackListModKeyFileName, record.TemplateActorsDefPackListFormKeyId),
            AttackDataTemplate = CreateNullableFormKey(record.TemplateActorsAttackDataModKeyName, record.TemplateActorsAttackDataModKeyType, record.TemplateActorsAttackDataModKeyFileName, record.TemplateActorsAttackDataFormKeyId),
            KeywordsTemplate = CreateNullableFormKey(record.TemplateActorsKeywordsModKeyName, record.TemplateActorsKeywordsModKeyType, record.TemplateActorsKeywordsModKeyFileName, record.TemplateActorsKeywordsFormKeyId),
            Unknown1 = CreateNullableFormKey(record.TemplateActorsUnknown1ModKeyName, record.TemplateActorsUnknown1ModKeyType, record.TemplateActorsUnknown1ModKeyFileName, record.TemplateActorsUnknown1FormKeyId),
            Unknown2 = CreateNullableFormKey(record.TemplateActorsUnknown2ModKeyName, record.TemplateActorsUnknown2ModKeyType, record.TemplateActorsUnknown2ModKeyFileName, record.TemplateActorsUnknown2FormKeyId)
        };
        return templateActors.TraitTemplate == null &&
               templateActors.StatsTemplate == null &&
               templateActors.FactionsTemplate == null &&
               templateActors.SpellListTemplate == null &&
               templateActors.AiPackagesTemplate == null &&
               templateActors.AiDataTemplate == null &&
               templateActors.BaseDataTemplate == null &&
               templateActors.InventoryTemplate == null &&
               templateActors.ScriptTemplate == null &&
               templateActors.DefPackListTemplate == null &&
               templateActors.AttackDataTemplate == null &&
               templateActors.KeywordsTemplate == null &&
               templateActors.Unknown1 == null &&
               templateActors.Unknown2 == null
            ? null
            : templateActors;
    }

    private static NPCWeightDTO? CreateWeight(NPCRow record)
    {
        return record.WeightValue == null && record.WeightThin == null && record.WeightMuscular == null && record.WeightFat == null
            ? null
            : new NPCWeightDTO
            {
                Value = record.WeightValue,
                Thin = record.WeightThin,
                Muscular = record.WeightMuscular,
                Fat = record.WeightFat
            };
    }

    private static NPCFaceMorphDTO? CreateFaceMorph(NPCRow record)
    {
        var faceMorph = new NPCFaceMorphDTO
        {
            NoseLongVsShort = record.FaceMorphNoseLongVsShort,
            NoseUpVsDown = record.FaceMorphNoseUpVsDown,
            JawUpVsDown = record.FaceMorphJawUpVsDown,
            JawNarrowVsWide = record.FaceMorphJawNarrowVsWide,
            JawForwardVsBack = record.FaceMorphJawForwardVsBack,
            CheeksUpVsDown = record.FaceMorphCheeksUpVsDown,
            CheeksForwardVsBack = record.FaceMorphCheeksForwardVsBack,
            EyesUpVsDown = record.FaceMorphEyesUpVsDown,
            EyesInVsOut = record.FaceMorphEyesInVsOut,
            BrowsUpVsDown = record.FaceMorphBrowsUpVsDown,
            BrowsInVsOut = record.FaceMorphBrowsInVsOut,
            BrowsForwardVsBack = record.FaceMorphBrowsForwardVsBack,
            LipsUpVsDown = record.FaceMorphLipsUpVsDown,
            LipsInVsOut = record.FaceMorphLipsInVsOut,
            ChinNarrowVsWide = record.FaceMorphChinNarrowVsWide,
            ChinUpVsDown = record.FaceMorphChinUpVsDown,
            ChinUnderbiteVsOverbite = record.FaceMorphChinUnderbiteVsOverbite,
            EyesForwardVsBack = record.FaceMorphEyesForwardVsBack,
            Unknown = record.FaceMorphUnknown
        };
        return faceMorph.NoseLongVsShort == null &&
               faceMorph.NoseUpVsDown == null &&
               faceMorph.JawUpVsDown == null &&
               faceMorph.JawNarrowVsWide == null &&
               faceMorph.JawForwardVsBack == null &&
               faceMorph.CheeksUpVsDown == null &&
               faceMorph.CheeksForwardVsBack == null &&
               faceMorph.EyesUpVsDown == null &&
               faceMorph.EyesInVsOut == null &&
               faceMorph.BrowsUpVsDown == null &&
               faceMorph.BrowsInVsOut == null &&
               faceMorph.BrowsForwardVsBack == null &&
               faceMorph.LipsUpVsDown == null &&
               faceMorph.LipsInVsOut == null &&
               faceMorph.ChinNarrowVsWide == null &&
               faceMorph.ChinUpVsDown == null &&
               faceMorph.ChinUnderbiteVsOverbite == null &&
               faceMorph.EyesForwardVsBack == null &&
               faceMorph.Unknown == null
            ? null
            : faceMorph;
    }

    private static NPCFacePartsDTO? CreateFaceParts(NPCRow record)
    {
        return record.FacePartsNose == null &&
               record.FacePartsUnknown == null &&
               record.FacePartsEyes == null &&
               record.FacePartsMouth == null
            ? null
            : new NPCFacePartsDTO
            {
                Nose = record.FacePartsNose,
                Unknown = record.FacePartsUnknown,
                Eyes = record.FacePartsEyes,
                Mouth = record.FacePartsMouth
            };
    }

    private static NPCPlayerSkillsDTO? CreatePlayerSkills(NPCRow record)
    {
        return record.PlayerSkillsHealth == null &&
               record.PlayerSkillsMagicka == null &&
               record.PlayerSkillsStamina == null &&
               record.PlayerSkillsGearedUpWeapons == null
            ? null
            : new NPCPlayerSkillsDTO
            {
                Health = record.PlayerSkillsHealth,
                Magicka = record.PlayerSkillsMagicka,
                Stamina = record.PlayerSkillsStamina,
                GearedUpWeapons = record.PlayerSkillsGearedUpWeapons
            };
    }

    private sealed class NPCRow : RecordRow
    {
        public string? Name { get; set; }
        public string? ShortName { get; set; }
        public string? LongName { get; set; }
        public bool? IsCompressed { get; set; }
        public string? ObjectBoundsFirst { get; set; }
        public string? ObjectBoundsSecond { get; set; }
        public string? Flags { get; set; }
        public string? MajorFlags { get; set; }
        public string? LevelMutagenObjectType { get; set; }
        public int? LevelLevel { get; set; }
        public double? LevelLevelMult { get; set; }
        public string? ConfigurationFlags { get; set; }
        public string? ConfigurationLevelMutagenObjectType { get; set; }
        public int? ConfigurationLevelLevel { get; set; }
        public double? ConfigurationLevelLevelMult { get; set; }
        public int? ConfigurationCalcMinLevel { get; set; }
        public int? ConfigurationCalcMaxLevel { get; set; }
        public int? ConfigurationHealthOffset { get; set; }
        public int? ConfigurationSpeedMultiplier { get; set; }
        public string? ConfigurationTemplateFlags { get; set; }
        public int? Version2 { get; set; }
        public int? VersionControl { get; set; }
        public int DispositionBase { get; set; }
        public string? UseTemplateActors { get; set; }
        public string Aggression { get; set; } = string.Empty;
        public string Confidence { get; set; } = string.Empty;
        public int EnergyLevel { get; set; }
        public string Responsibility { get; set; } = string.Empty;
        public string Assistance { get; set; } = string.Empty;
        public string? Mood { get; set; }
        public int GearedUpWeapons { get; set; }
        public double HeightMin { get; set; }
        public double HeightMax { get; set; }
        public int? SkinToneIndex { get; set; }
        /// <summary>
        /// Gets or sets the mod key name for the Fallout 4 skin reference.
        /// </summary>
        public string? SkinModKeyName { get; set; }
        /// <summary>
        /// Gets or sets the mod key type for the Fallout 4 skin reference.
        /// </summary>
        public int? SkinModKeyType { get; set; }
        /// <summary>
        /// Gets or sets the mod file name for the Fallout 4 skin reference.
        /// </summary>
        public string? SkinModKeyFileName { get; set; }
        /// <summary>
        /// Gets or sets the form key identifier for the Fallout 4 skin reference.
        /// </summary>
        public long? SkinFormKeyId { get; set; }
        public string? Pronoun { get; set; }
        public string? VoiceModKeyName { get; set; }
        public int? VoiceModKeyType { get; set; }
        public string? VoiceModKeyFileName { get; set; }
        public long? VoiceFormKeyId { get; set; }
        public string? RaceModKeyName { get; set; }
        public int? RaceModKeyType { get; set; }
        public string? RaceModKeyFileName { get; set; }
        public long? RaceFormKeyId { get; set; }
        public string? AttackRaceModKeyName { get; set; }
        public int? AttackRaceModKeyType { get; set; }
        public string? AttackRaceModKeyFileName { get; set; }
        public long? AttackRaceFormKeyId { get; set; }
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
        public string? ClassModKeyName { get; set; }
        public int? ClassModKeyType { get; set; }
        public string? ClassModKeyFileName { get; set; }
        public long? ClassFormKeyId { get; set; }
        public string? DeathItemModKeyName { get; set; }
        public int? DeathItemModKeyType { get; set; }
        public string? DeathItemModKeyFileName { get; set; }
        public long? DeathItemFormKeyId { get; set; }
        public string? DefaultOutfitModKeyName { get; set; }
        public int? DefaultOutfitModKeyType { get; set; }
        public string? DefaultOutfitModKeyFileName { get; set; }
        public long? DefaultOutfitFormKeyId { get; set; }
        public string? SleepingOutfitModKeyName { get; set; }
        public int? SleepingOutfitModKeyType { get; set; }
        public string? SleepingOutfitModKeyFileName { get; set; }
        public long? SleepingOutfitFormKeyId { get; set; }
        public string? WornArmorModKeyName { get; set; }
        public int? WornArmorModKeyType { get; set; }
        public string? WornArmorModKeyFileName { get; set; }
        public long? WornArmorFormKeyId { get; set; }
        /// <summary>
        /// Gets or sets the mod key name for the Fallout 4 power armor stand reference.
        /// </summary>
        public string? PowerArmorStandModKeyName { get; set; }
        /// <summary>
        /// Gets or sets the mod key type for the Fallout 4 power armor stand reference.
        /// </summary>
        public int? PowerArmorStandModKeyType { get; set; }
        /// <summary>
        /// Gets or sets the mod file name for the Fallout 4 power armor stand reference.
        /// </summary>
        public string? PowerArmorStandModKeyFileName { get; set; }
        /// <summary>
        /// Gets or sets the form key identifier for the Fallout 4 power armor stand reference.
        /// </summary>
        public long? PowerArmorStandFormKeyId { get; set; }
        public string? SpaceOutfitModKeyName { get; set; }
        public int? SpaceOutfitModKeyType { get; set; }
        public string? SpaceOutfitModKeyFileName { get; set; }
        public long? SpaceOutfitFormKeyId { get; set; }
        public string? HeadTextureModKeyName { get; set; }
        public int? HeadTextureModKeyType { get; set; }
        public string? HeadTextureModKeyFileName { get; set; }
        public long? HeadTextureFormKeyId { get; set; }
        public string? TemplateModKeyName { get; set; }
        public int? TemplateModKeyType { get; set; }
        public string? TemplateModKeyFileName { get; set; }
        public long? TemplateFormKeyId { get; set; }
        public string? DefaultTemplateModKeyName { get; set; }
        public int? DefaultTemplateModKeyType { get; set; }
        public string? DefaultTemplateModKeyFileName { get; set; }
        public long? DefaultTemplateFormKeyId { get; set; }
        public string? TemplateActorsTraitModKeyName { get; set; }
        public int? TemplateActorsTraitModKeyType { get; set; }
        public string? TemplateActorsTraitModKeyFileName { get; set; }
        public long? TemplateActorsTraitFormKeyId { get; set; }
        public string? TemplateActorsStatsModKeyName { get; set; }
        public int? TemplateActorsStatsModKeyType { get; set; }
        public string? TemplateActorsStatsModKeyFileName { get; set; }
        public long? TemplateActorsStatsFormKeyId { get; set; }
        /// <summary>
        /// Gets or sets the plugin name for the factions template actor reference.
        /// </summary>
        public string? TemplateActorsFactionsModKeyName { get; set; }
        /// <summary>
        /// Gets or sets the plugin type for the factions template actor reference.
        /// </summary>
        public int? TemplateActorsFactionsModKeyType { get; set; }
        /// <summary>
        /// Gets or sets the plugin file name for the factions template actor reference.
        /// </summary>
        public string? TemplateActorsFactionsModKeyFileName { get; set; }
        /// <summary>
        /// Gets or sets the form ID for the factions template actor reference.
        /// </summary>
        public long? TemplateActorsFactionsFormKeyId { get; set; }
        public string? TemplateActorsSpellListModKeyName { get; set; }
        public int? TemplateActorsSpellListModKeyType { get; set; }
        public string? TemplateActorsSpellListModKeyFileName { get; set; }
        public long? TemplateActorsSpellListFormKeyId { get; set; }
        /// <summary>
        /// Gets or sets the plugin name for the AI packages template actor reference.
        /// </summary>
        public string? TemplateActorsAiPackagesModKeyName { get; set; }
        /// <summary>
        /// Gets or sets the plugin type for the AI packages template actor reference.
        /// </summary>
        public int? TemplateActorsAiPackagesModKeyType { get; set; }
        /// <summary>
        /// Gets or sets the plugin file name for the AI packages template actor reference.
        /// </summary>
        public string? TemplateActorsAiPackagesModKeyFileName { get; set; }
        /// <summary>
        /// Gets or sets the form ID for the AI packages template actor reference.
        /// </summary>
        public long? TemplateActorsAiPackagesFormKeyId { get; set; }
        public string? TemplateActorsAiDataModKeyName { get; set; }
        public int? TemplateActorsAiDataModKeyType { get; set; }
        public string? TemplateActorsAiDataModKeyFileName { get; set; }
        public long? TemplateActorsAiDataFormKeyId { get; set; }
        public string? TemplateActorsBaseDataModKeyName { get; set; }
        public int? TemplateActorsBaseDataModKeyType { get; set; }
        public string? TemplateActorsBaseDataModKeyFileName { get; set; }
        public long? TemplateActorsBaseDataFormKeyId { get; set; }
        public string? TemplateActorsInventoryModKeyName { get; set; }
        public int? TemplateActorsInventoryModKeyType { get; set; }
        public string? TemplateActorsInventoryModKeyFileName { get; set; }
        public long? TemplateActorsInventoryFormKeyId { get; set; }
        /// <summary>
        /// Gets or sets the plugin name for the script template actor reference.
        /// </summary>
        public string? TemplateActorsScriptModKeyName { get; set; }
        /// <summary>
        /// Gets or sets the plugin type for the script template actor reference.
        /// </summary>
        public int? TemplateActorsScriptModKeyType { get; set; }
        /// <summary>
        /// Gets or sets the plugin file name for the script template actor reference.
        /// </summary>
        public string? TemplateActorsScriptModKeyFileName { get; set; }
        /// <summary>
        /// Gets or sets the form ID for the script template actor reference.
        /// </summary>
        public long? TemplateActorsScriptFormKeyId { get; set; }
        public string? TemplateActorsDefPackListModKeyName { get; set; }
        public int? TemplateActorsDefPackListModKeyType { get; set; }
        public string? TemplateActorsDefPackListModKeyFileName { get; set; }
        public long? TemplateActorsDefPackListFormKeyId { get; set; }
        public string? TemplateActorsAttackDataModKeyName { get; set; }
        public int? TemplateActorsAttackDataModKeyType { get; set; }
        public string? TemplateActorsAttackDataModKeyFileName { get; set; }
        public long? TemplateActorsAttackDataFormKeyId { get; set; }
        public string? TemplateActorsKeywordsModKeyName { get; set; }
        public int? TemplateActorsKeywordsModKeyType { get; set; }
        public string? TemplateActorsKeywordsModKeyFileName { get; set; }
        public long? TemplateActorsKeywordsFormKeyId { get; set; }
        /// <summary>
        /// Gets or sets the plugin name for the first unknown template actor reference.
        /// </summary>
        public string? TemplateActorsUnknown1ModKeyName { get; set; }
        /// <summary>
        /// Gets or sets the plugin type for the first unknown template actor reference.
        /// </summary>
        public int? TemplateActorsUnknown1ModKeyType { get; set; }
        /// <summary>
        /// Gets or sets the plugin file name for the first unknown template actor reference.
        /// </summary>
        public string? TemplateActorsUnknown1ModKeyFileName { get; set; }
        /// <summary>
        /// Gets or sets the form ID for the first unknown template actor reference.
        /// </summary>
        public long? TemplateActorsUnknown1FormKeyId { get; set; }
        public string? TemplateActorsUnknown2ModKeyName { get; set; }
        public int? TemplateActorsUnknown2ModKeyType { get; set; }
        public string? TemplateActorsUnknown2ModKeyFileName { get; set; }
        public long? TemplateActorsUnknown2FormKeyId { get; set; }
        public int? CalculatedHealth { get; set; }
        public int? CalculatedActionPoints { get; set; }
        public int? XpValueOffset { get; set; }
        public int? Unknown { get; set; }
        public int? Unused { get; set; }
        public string? NAM5 { get; set; }
        public double? Height { get; set; }
        public double? WeightValue { get; set; }
        public double? WeightThin { get; set; }
        public double? WeightMuscular { get; set; }
        public double? WeightFat { get; set; }
        public string? SoundLevel { get; set; }
        public string? TextureLighting { get; set; }
        public string? HairColor { get; set; }
        public string? FacialHairColor { get; set; }
        public string? EyebrowColor { get; set; }
        public string? EyeColor { get; set; }
        public double? FaceMorphNoseLongVsShort { get; set; }
        public double? FaceMorphNoseUpVsDown { get; set; }
        public double? FaceMorphJawUpVsDown { get; set; }
        public double? FaceMorphJawNarrowVsWide { get; set; }
        public double? FaceMorphJawForwardVsBack { get; set; }
        public double? FaceMorphCheeksUpVsDown { get; set; }
        public double? FaceMorphCheeksForwardVsBack { get; set; }
        public double? FaceMorphEyesUpVsDown { get; set; }
        public double? FaceMorphEyesInVsOut { get; set; }
        public double? FaceMorphBrowsUpVsDown { get; set; }
        public double? FaceMorphBrowsInVsOut { get; set; }
        public double? FaceMorphBrowsForwardVsBack { get; set; }
        public double? FaceMorphLipsUpVsDown { get; set; }
        public double? FaceMorphLipsInVsOut { get; set; }
        public double? FaceMorphChinNarrowVsWide { get; set; }
        public double? FaceMorphChinUpVsDown { get; set; }
        public double? FaceMorphChinUnderbiteVsOverbite { get; set; }
        public double? FaceMorphEyesForwardVsBack { get; set; }
        public double? FaceMorphUnknown { get; set; }
        public long? FacePartsNose { get; set; }
        public long? FacePartsUnknown { get; set; }
        public long? FacePartsEyes { get; set; }
        public long? FacePartsMouth { get; set; }
        public int? PlayerSkillsHealth { get; set; }
        public int? PlayerSkillsMagicka { get; set; }
        public int? PlayerSkillsStamina { get; set; }
        public int? PlayerSkillsGearedUpWeapons { get; set; }
        public string? BodyMorphRegionValues { get; set; }
        public string? ObjectTemplates { get; set; }
        public string? AIData { get; set; }
    }

    private sealed class NPCFormKeyListRow
    {
        public string? TargetModKeyName { get; set; }

        public int? TargetModKeyType { get; set; }

        public string? TargetModKeyFileName { get; set; }

        public long? TargetFormKeyId { get; set; }
    }

    private sealed class NPCFactionRow
    {
        public string? ModKeyName { get; set; }

        public int? ModKeyType { get; set; }

        public string? ModKeyFileName { get; set; }

        public int FactionIndex { get; set; }

        public string? FactionModKeyName { get; set; }

        public int? FactionModKeyType { get; set; }

        public string? FactionModKeyFileName { get; set; }

        public long? FactionFormKeyId { get; set; }

        public int? Rank { get; set; }

        public string? Fluff { get; set; }

        public DateTime ImportedAtUTC { get; set; }
    }

    private sealed class NPCPropertyRow
    {
        public string? ModKeyName { get; set; }

        public int? ModKeyType { get; set; }

        public string? ModKeyFileName { get; set; }

        public int PropertyIndex { get; set; }

        public string? ActorValueModKeyName { get; set; }

        public int? ActorValueModKeyType { get; set; }

        public string? ActorValueModKeyFileName { get; set; }

        public long? ActorValueFormKeyId { get; set; }

        public double? Value { get; set; }

        public DateTime ImportedAtUTC { get; set; }
    }

    private sealed class NPCItemRow
    {
        public string? ModKeyName { get; set; }

        public int? ModKeyType { get; set; }

        public string? ModKeyFileName { get; set; }

        public int ItemIndex { get; set; }

        public string? ItemModKeyName { get; set; }

        public int? ItemModKeyType { get; set; }

        public string? ItemModKeyFileName { get; set; }

        public long? ItemFormKeyId { get; set; }

        public int? Count { get; set; }

        public DateTime ImportedAtUTC { get; set; }
    }

    private sealed class NPCPerkRow
    {
        public string? ModKeyName { get; set; }

        public int? ModKeyType { get; set; }

        public string? ModKeyFileName { get; set; }

        public int PerkIndex { get; set; }

        public string? PerkModKeyName { get; set; }

        public int? PerkModKeyType { get; set; }

        public string? PerkModKeyFileName { get; set; }

        public long? PerkFormKeyId { get; set; }

        public int? Rank { get; set; }

        public string? Fluff { get; set; }

        public DateTime ImportedAtUTC { get; set; }
    }

    private sealed class NPCMorphRow
    {
        public string? ModKeyName { get; set; }
        public int? ModKeyType { get; set; }
        public string? ModKeyFileName { get; set; }
        public int MorphIndex { get; set; }
        public long? Key { get; set; }
        public double? Value { get; set; }
        public DateTime ImportedAtUTC { get; set; }
    }

    private sealed class NPCFaceDialPositionRow
    {
        public string? ModKeyName { get; set; }
        public int? ModKeyType { get; set; }
        public string? ModKeyFileName { get; set; }
        public int FaceDialPositionIndex { get; set; }
        public int? SourceIndex { get; set; }
        public double? Position { get; set; }
        public DateTime ImportedAtUTC { get; set; }
    }

    private sealed class NPCFaceMorphPositionRow
    {
        public string? ModKeyName { get; set; }
        public int? ModKeyType { get; set; }
        public string? ModKeyFileName { get; set; }
        public int FaceMorphIndex { get; set; }
        public int? SourceIndex { get; set; }
        public string? Position { get; set; }
        /// <summary>
        /// Gets or sets the optional Fallout 4 rotation vector text.
        /// </summary>
        public string? Rotation { get; set; }
        public double? Scale { get; set; }
        public DateTime ImportedAtUTC { get; set; }
    }

    private sealed class NPCFaceMorphGroupSetRow
    {
        public string? ModKeyName { get; set; }
        public int? ModKeyType { get; set; }
        public string? ModKeyFileName { get; set; }
        public int FaceMorphIndex { get; set; }
        public int? SourceIndex { get; set; }
        public DateTime ImportedAtUTC { get; set; }
    }

    private sealed class NPCFaceMorphGroupRow
    {
        public string? ModKeyName { get; set; }
        public int? ModKeyType { get; set; }
        public string? ModKeyFileName { get; set; }
        public int FaceMorphIndex { get; set; }
        public int MorphGroupIndex { get; set; }
        public string? MorphGroup { get; set; }
        public double? BlendIntensity { get; set; }
        public DateTime ImportedAtUTC { get; set; }
    }

    private sealed class NPCMorphBlendRow
    {
        public string? ModKeyName { get; set; }
        public int? ModKeyType { get; set; }
        public string? ModKeyFileName { get; set; }
        public int MorphBlendIndex { get; set; }
        public string? BlendName { get; set; }
        public double? Intensity { get; set; }
        public DateTime ImportedAtUTC { get; set; }
    }

    private sealed class NPCTintRow
    {
        public string? ModKeyName { get; set; }
        public int? ModKeyType { get; set; }
        public string? ModKeyFileName { get; set; }
        public int TintIndex { get; set; }
        public string? TintType { get; set; }
        public string? TintGroup { get; set; }
        public string? TintName { get; set; }
        public string? TintTexture { get; set; }
        public string? TintColor { get; set; }
        public double? TintIntensity { get; set; }
        public DateTime ImportedAtUTC { get; set; }
    }

    private sealed class NPCTintLayerRow
    {
        public string? ModKeyName { get; set; }
        public int? ModKeyType { get; set; }
        public string? ModKeyFileName { get; set; }
        public int TintLayerIndex { get; set; }
        public int? SourceIndex { get; set; }
        public string? Color { get; set; }
        public double? InterpolationValue { get; set; }
        public int? Preset { get; set; }
        public DateTime ImportedAtUTC { get; set; }
    }

    /// <summary>
    /// Provides the NPoco projection for one persisted Fallout 4 NPC face tinting layer row.
    /// </summary>
    private sealed class NPCFaceTintingLayerRow
    {
        /// <summary>
        /// Gets or sets the plugin mod key name that owns the parent NPC.
        /// </summary>
        public string? ModKeyName { get; set; }

        /// <summary>
        /// Gets or sets the plugin mod key type that owns the parent NPC.
        /// </summary>
        public int? ModKeyType { get; set; }

        /// <summary>
        /// Gets or sets the plugin file name that owns the parent NPC.
        /// </summary>
        public string? ModKeyFileName { get; set; }

        /// <summary>
        /// Gets or sets the zero-based layer index.
        /// </summary>
        public int FaceTintingLayerIndex { get; set; }

        /// <summary>
        /// Gets or sets the Spriggit data type label.
        /// </summary>
        public string? DataType { get; set; }

        /// <summary>
        /// Gets or sets the source tint index.
        /// </summary>
        public int? SourceIndex { get; set; }

        /// <summary>
        /// Gets or sets the optional layer strength.
        /// </summary>
        public double? Value { get; set; }

        /// <summary>
        /// Gets or sets the optional color text.
        /// </summary>
        public string? Color { get; set; }

        /// <summary>
        /// Gets or sets the optional template color index.
        /// </summary>
        public int? TemplateColorIndex { get; set; }

        /// <summary>
        /// Gets or sets the UTC timestamp for the import that produced this row.
        /// </summary>
        public DateTime ImportedAtUTC { get; set; }
    }

    /// <summary>
    /// Provides the NPoco projection for one persisted Fallout 4 NPC face tinting layer state flag.
    /// </summary>
    private sealed class NPCFaceTintingLayerStateRow
    {
        /// <summary>
        /// Gets or sets the parent face tinting layer index.
        /// </summary>
        public int FaceTintingLayerIndex { get; set; }

        /// <summary>
        /// Gets or sets the zero-based state index within the parent layer.
        /// </summary>
        public int StateIndex { get; set; }

        /// <summary>
        /// Gets or sets the state flag name exported by Spriggit.
        /// </summary>
        public string State { get; set; } = string.Empty;
    }

    private sealed class NPCPlayerSkillValueRow
    {
        public string ValueListName { get; set; } = string.Empty;
        public int SkillIndex { get; set; }
        public string? SkillKey { get; set; }
        public int? SkillValue { get; set; }
    }
}
