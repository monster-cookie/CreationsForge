using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Repositories.Interfaces;
using NPoco;

namespace CreationsForge.Core.Repositories;

public class PerkRepository : TypedRecordRepositoryBase, IPerkRepository
{
    private readonly IRecordLocalizedStringRepository RecordLocalizedStringRepository;
    private readonly IConditionRuleRepository ConditionRuleRepository;
    private readonly ISoundMappingRepository SoundMappingRepository;
    private readonly IRawRecordPayloadRepository RawRecordPayloadRepository;

    public PerkRepository(
        IDatabase database,
        IRecordInstanceRepository recordInstanceRepository,
        IRecordLocalizedStringRepository recordLocalizedStringRepository,
        IConditionRuleRepository conditionRuleRepository,
        ISoundMappingRepository soundMappingRepository,
        IRawRecordPayloadRepository rawRecordPayloadRepository)
        : base(database, recordInstanceRepository)
    {
        RecordLocalizedStringRepository = recordLocalizedStringRepository;
        ConditionRuleRepository = conditionRuleRepository;
        SoundMappingRepository = soundMappingRepository;
        RawRecordPayloadRepository = rawRecordPayloadRepository;
    }

    public override string RecordType => RecordTypeCatalog.Perk.RecordID;

    protected override string TableName => RecordTypeCatalog.Perk.TableName;

    public IReadOnlyList<PerkDTO> GetByFormKey(SupportedGame game, FormKeyDTO formKey)
    {
        var records = FetchByFormKey<PerkRow>(
                game,
                formKey,
                [
                    SelectColumn("Name"),
                    SelectColumn("Description"),
                    SelectColumn("Flags"),
                    SelectColumn("SkillGroup"),
                    SelectColumn("CrewAssignment"),
                    SelectColumn("PerkIcon"),
                    SelectColumn("Category"),
                    SelectColumn("Restriction_ModKey_Name", "RestrictionModKeyName"),
                    SelectColumn("Restriction_ModKey_Type", "RestrictionModKeyType"),
                    SelectColumn("Restriction_ModKey_FileName", "RestrictionModKeyFileName"),
                    SelectColumn("Restriction_FormKey_ID", "RestrictionFormKeyId"),
                    SelectColumn("Training_ModKey_Name", "TrainingModKeyName"),
                    SelectColumn("Training_ModKey_Type", "TrainingModKeyType"),
                    SelectColumn("Training_ModKey_FileName", "TrainingModKeyFileName"),
                    SelectColumn("Training_FormKey_ID", "TrainingFormKeyId"),
                    SelectColumn("Level"),
                    SelectColumn("NumRanks"),
                    SelectColumn("Playable"),
                    SelectColumn("Hidden"),
                    SelectColumn("NextPerk_ModKey_Name", "NextPerkModKeyName"),
                    SelectColumn("NextPerk_ModKey_Type", "NextPerkModKeyType"),
                    SelectColumn("NextPerk_ModKey_FileName", "NextPerkModKeyFileName"),
                    SelectColumn("NextPerk_FormKey_ID", "NextPerkFormKeyId"),
                    SelectColumn("MajorFlags")
                ])
            .Select(record => ToDTO(record, game))
            .ToList();
        var rankRows = FetchRankRowsByFormKey(game, formKey);
        var activityRows = FetchActivityRowsByFormKey(game, formKey);
        var activityEvaluatorRows = FetchActivityEvaluatorRowsByFormKey(game, formKey);
        var effectRows = FetchRankEffectRowsByFormKey(game, formKey);
        var rootEffectRows = FetchRootEffectRowsByFormKey(game, formKey);
        var conditionTabRows = FetchConditionTabRowsByFormKey(game, formKey);
        var backgroundSkillRows = FetchBackgroundSkillRowsByFormKey(game, formKey);
        var localizedStrings = RecordLocalizedStringRepository.GetByFormKey(game, RecordTypeCatalog.Perk.RecordID, formKey);
        var conditions = ConditionRuleRepository.GetByFormKey(game, RecordTypeCatalog.Perk.RecordID, formKey);
        var sounds = SoundMappingRepository.GetByFormKey(game, RecordTypeCatalog.Perk.RecordID, formKey);
        var rawPayloads = RawRecordPayloadRepository.GetByFormKey(game, RecordTypeCatalog.Perk.RecordID, formKey);
        foreach (var record in records)
        {
            var recordLocalizedStrings = localizedStrings.Where(localizedString => IsSameModKey(localizedString.ModKey, record.ModKey)).ToList();
            var recordConditions = conditions.Where(condition => IsSameModKey(condition.ModKey, record.ModKey)).ToList();
            ApplyLocalizedStrings(record, recordLocalizedStrings);
            record.Conditions = recordConditions
                .Where(condition => string.Equals(condition.ConditionSlot, "Conditions", StringComparison.Ordinal))
                .OrderBy(condition => condition.ConditionIndex)
                .ToList();
            record.Sounds = sounds.Where(sound => IsSameModKey(sound.ModKey, record.ModKey)).OrderBy(sound => sound.SoundIndex).ToList();
            record.RawPayloads = rawPayloads.Where(payload => IsSameModKey(payload.ModKey, record.ModKey)).OrderBy(payload => payload.PayloadSlot).ThenBy(payload => payload.PayloadIndex).ToList();
            record.Effects = rootEffectRows
                .Where(effect => IsSameModKey(effect, record.ModKey))
                .OrderBy(effect => effect.EffectIndex)
                .Select(ToDTO)
                .ToList();
            foreach (var effect in record.Effects)
            {
                ApplyLocalizedStrings(effect, recordLocalizedStrings);
                effect.Conditions = GetEffectConditionTabs(conditionTabRows, recordConditions, record.ModKey, null, effect.EffectIndex);
            }

            record.Ranks = rankRows
                .Where(rank => IsSameModKey(rank, record.ModKey))
                .OrderBy(rank => rank.RankIndex)
                .Select(ToDTO)
                .ToList();
            foreach (var rank in record.Ranks)
            {
                ApplyLocalizedStrings(rank, recordLocalizedStrings);
                rank.Conditions = recordConditions
                    .Where(condition => string.Equals(condition.ConditionSlot, GetPerkRankConditionSlot(rank.RankIndex), StringComparison.Ordinal))
                    .OrderBy(condition => condition.ConditionIndex)
                    .ToList();
                rank.Effects = effectRows
                    .Where(effect => IsSameModKey(effect, record.ModKey) && effect.RankIndex == rank.RankIndex)
                    .OrderBy(effect => effect.EffectIndex)
                    .Select(ToDTO)
                    .ToList();
                rank.Activities = activityRows
                    .Where(activity => IsSameModKey(activity, record.ModKey) && activity.RankIndex == rank.RankIndex)
                    .OrderBy(activity => activity.ActivityIndex)
                    .Select(ToDTO)
                    .ToList();
                foreach (var activity in rank.Activities)
                {
                    ApplyLocalizedStrings(activity, recordLocalizedStrings);
                    activity.ProgressionEvalutor = activityEvaluatorRows
                        .Where(evaluator =>
                            IsSameModKey(evaluator, record.ModKey) &&
                            evaluator.RankIndex == rank.RankIndex &&
                            evaluator.ActivityIndex == activity.ActivityIndex)
                        .OrderBy(evaluator => evaluator.EvaluatorIndex)
                        .Select(ToDTO)
                        .ToList();
                    foreach (var evaluator in activity.ProgressionEvalutor)
                    {
                        evaluator.Conditions = recordConditions
                            .Where(condition => string.Equals(condition.ConditionSlot, GetPerkActivityEvaluatorConditionSlot(rank.RankIndex, activity.ActivityIndex, evaluator.EvaluatorIndex), StringComparison.Ordinal))
                            .OrderBy(condition => condition.ConditionIndex)
                            .ToList();
                    }
                }

                foreach (var effect in rank.Effects)
                {
                    ApplyLocalizedStrings(effect, recordLocalizedStrings);
                    effect.Conditions = GetEffectConditionTabs(conditionTabRows, recordConditions, record.ModKey, rank.RankIndex, effect.EffectIndex);
                }
            }

            record.BackgroundSkills = backgroundSkillRows
                .Where(skill => IsSameModKey(skill, record.ModKey))
                .OrderBy(skill => skill.SkillIndex)
                .Select(ToDTO)
                .ToList();
        }

        return records;
    }

    public void Save(PerkDTO dto)
    {
        SaveRecordInstance(dto);
        Database.Execute(
            """
            INSERT OR REPLACE INTO Perks (
                Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                EditorID, FormVersion, MajorRecordFlags, ImportedAtUTC, Name, Description, Flags, SkillGroup, CrewAssignment, PerkIcon, Category,
                Restriction_ModKey_Name, Restriction_ModKey_Type, Restriction_ModKey_FileName, Restriction_FormKey_ID,
                Training_ModKey_Name, Training_ModKey_Type, Training_ModKey_FileName, Training_FormKey_ID,
                Level, NumRanks, Playable, Hidden, NextPerk_ModKey_Name, NextPerk_ModKey_Type, NextPerk_ModKey_FileName, NextPerk_FormKey_ID, MajorFlags)
            VALUES (
                @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                @EditorId, @FormVersion, @MajorRecordFlags, @ImportedAtUTC, @Name, @Description, @Flags, @SkillGroup, @CrewAssignment, @PerkIcon, @Category,
                @RestrictionModKeyName, @RestrictionModKeyType, @RestrictionModKeyFileName, @RestrictionFormKeyId,
                @TrainingModKeyName, @TrainingModKeyType, @TrainingModKeyFileName, @TrainingFormKeyId,
                @Level, @NumRanks, @Playable, @Hidden, @NextPerkModKeyName, @NextPerkModKeyType, @NextPerkModKeyFileName, @NextPerkFormKeyId, @MajorFlags);
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
                Description = GetEnglishText(dto.Description),
                dto.Flags,
                dto.SkillGroup,
                dto.CrewAssignment,
                dto.PerkIcon,
                dto.Category,
                RestrictionModKeyName = dto.RestrictionFormKey?.ModKey.Name,
                RestrictionModKeyType = dto.RestrictionFormKey?.ModKey.Type,
                RestrictionModKeyFileName = dto.RestrictionFormKey?.ModKey.FileName,
                RestrictionFormKeyId = dto.RestrictionFormKey?.Id,
                TrainingModKeyName = dto.TrainingFormKey?.ModKey.Name,
                TrainingModKeyType = dto.TrainingFormKey?.ModKey.Type,
                TrainingModKeyFileName = dto.TrainingFormKey?.ModKey.FileName,
                TrainingFormKeyId = dto.TrainingFormKey?.Id,
                dto.Level,
                dto.NumRanks,
                Playable = ToNullableInt(dto.Playable),
                Hidden = ToNullableInt(dto.Hidden),
                NextPerkModKeyName = dto.NextPerk?.ModKey.Name,
                NextPerkModKeyType = dto.NextPerk?.ModKey.Type,
                NextPerkModKeyFileName = dto.NextPerk?.ModKey.FileName,
                NextPerkFormKeyId = dto.NextPerk?.Id,
                dto.MajorFlags
            });
        DeleteChildren(dto);
        SaveRootEffects(dto);
        SaveRanks(dto);
        SaveBackgroundSkills(dto);
    }

    private void SaveRanks(PerkDTO dto)
    {
        foreach (var rank in dto.Ranks)
        {
            rank.FormKey = dto.FormKey;
            rank.ModKey = dto.ModKey;
            rank.ImportedAtUTC = dto.ImportedAtUTC;
            Database.Execute(
                """
                INSERT OR REPLACE INTO PerkRanks (
                    Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                    Rank_Index, Description, UnknownStatic_ModKey_Name, UnknownStatic_ModKey_Type, UnknownStatic_ModKey_FileName, UnknownStatic_FormKey_ID,
                    ConditionCount, ActivityCount, ImportedAtUTC)
                VALUES (
                    @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                    @RankIndex, @Description, @UnknownStaticModKeyName, @UnknownStaticModKeyType, @UnknownStaticModKeyFileName, @UnknownStaticFormKeyId,
                    @ConditionCount, @ActivityCount, @ImportedAtUTC);
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
                    rank.RankIndex,
                    Description = GetEnglishText(rank.Description),
                    UnknownStaticModKeyName = rank.UnknownStaticFormKey?.ModKey.Name,
                    UnknownStaticModKeyType = rank.UnknownStaticFormKey?.ModKey.Type,
                    UnknownStaticModKeyFileName = rank.UnknownStaticFormKey?.ModKey.FileName,
                    UnknownStaticFormKeyId = rank.UnknownStaticFormKey?.Id,
                    rank.ConditionCount,
                    rank.ActivityCount,
                    rank.ImportedAtUTC
                });

            foreach (var effect in rank.Effects)
            {
                effect.FormKey = dto.FormKey;
                effect.ModKey = dto.ModKey;
                effect.RankIndex = rank.RankIndex;
                effect.ImportedAtUTC = dto.ImportedAtUTC;
                Database.Execute(
                    """
                    INSERT OR REPLACE INTO PerkRankEffects (
                        Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                        Rank_Index, Effect_Index, MutagenObjectType, Rank, Priority, PerkEntryID, Flags, ButtonLabel, ConditionCount,
                        EntryPoint, PerkConditionTabCount, Modification, Value, ActorValue, Spell, Quest, Stage, ImportedAtUTC)
                    VALUES (
                        @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                        @RankIndex, @EffectIndex, @MutagenObjectType, @Rank, @Priority, @PerkEntryId, @Flags, @ButtonLabel, @ConditionCount,
                        @EntryPoint, @PerkConditionTabCount, @Modification, @Value, @ActorValue, @Spell, @Quest, @Stage, @ImportedAtUTC);
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
                        effect.RankIndex,
                        effect.EffectIndex,
                        effect.MutagenObjectType,
                        effect.Rank,
                        effect.Priority,
                        effect.PerkEntryId,
                        effect.Flags,
                        ButtonLabel = GetEnglishText(effect.ButtonLabel),
                        effect.ConditionCount,
                        effect.EntryPoint,
                        effect.PerkConditionTabCount,
                        effect.Modification,
                        effect.Value,
                        effect.ActorValue,
                        effect.Spell,
                        effect.Quest,
                        effect.Stage,
                        effect.ImportedAtUTC
                    });
                SaveEffectConditionTabs(dto, rank.RankIndex, effect.EffectIndex, effect.Conditions);
            }

            SaveActivities(dto, rank);
        }
    }

    private void SaveActivities(PerkDTO dto, PerkRankDTO rank)
    {
        foreach (var activity in rank.Activities)
        {
            activity.FormKey = dto.FormKey;
            activity.ModKey = dto.ModKey;
            activity.RankIndex = rank.RankIndex;
            activity.ImportedAtUTC = dto.ImportedAtUTC;
            Database.Execute(
                """
                INSERT OR REPLACE INTO PerkRankActivities (
                    Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                    Rank_Index, Activity_Index, ATAN, Name, Description, ANAM, Configuration, ImportedAtUTC)
                VALUES (
                    @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                    @RankIndex, @ActivityIndex, @ATAN, @Name, @Description, @ANAM, @Configuration, @ImportedAtUTC);
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
                    activity.RankIndex,
                    activity.ActivityIndex,
                    activity.ATAN,
                    Name = GetEnglishText(activity.Name),
                    Description = GetEnglishText(activity.Description),
                    activity.ANAM,
                    activity.Configuration,
                    activity.ImportedAtUTC
                });

            foreach (var evaluator in activity.ProgressionEvalutor)
            {
                evaluator.FormKey = dto.FormKey;
                evaluator.ModKey = dto.ModKey;
                evaluator.RankIndex = rank.RankIndex;
                evaluator.ActivityIndex = activity.ActivityIndex;
                evaluator.ImportedAtUTC = dto.ImportedAtUTC;
                Database.Execute(
                    """
                    INSERT OR REPLACE INTO PerkRankActivityProgressionEvaluators (
                        Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                        Rank_Index, Activity_Index, Evaluator_Index, Name, ImportedAtUTC)
                    VALUES (
                        @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                        @RankIndex, @ActivityIndex, @EvaluatorIndex, @Name, @ImportedAtUTC);
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
                        evaluator.RankIndex,
                        evaluator.ActivityIndex,
                        evaluator.EvaluatorIndex,
                        evaluator.Name,
                        evaluator.ImportedAtUTC
                    });
            }
        }
    }

    private IReadOnlyList<PerkRankRow> FetchRankRowsByFormKey(SupportedGame game, FormKeyDTO formKey)
    {
        return Database.Fetch<PerkRankRow>(
                """
                SELECT ranks.*
                FROM PerkRanks ranks
                INNER JOIN Perks perks ON perks.Game = ranks.Game
                  AND perks.ModKey_Name = ranks.ModKey_Name COLLATE NOCASE
                  AND perks.ModKey_Type = ranks.ModKey_Type
                  AND perks.ModKey_FileName = ranks.ModKey_FileName COLLATE NOCASE
                  AND perks.FormKey_ModKey_Name = ranks.FormKey_ModKey_Name COLLATE NOCASE
                  AND perks.FormKey_ModKey_Type = ranks.FormKey_ModKey_Type
                  AND perks.FormKey_ModKey_FileName = ranks.FormKey_ModKey_FileName COLLATE NOCASE
                  AND perks.FormKey_ID = ranks.FormKey_ID
                WHERE ranks.Game = @Game
                  AND ranks.FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
                  AND ranks.FormKey_ModKey_Type = @FormKeyModKeyType
                  AND ranks.FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
                  AND ranks.FormKey_ID = @FormKeyId
                ORDER BY ranks.ModKey_FileName COLLATE NOCASE, ranks.Rank_Index;
                """,
                new
                {
                    Game = game.ToString(),
                    FormKeyModKeyName = formKey.ModKey.Name,
                    FormKeyModKeyType = formKey.ModKey.Type,
                    FormKeyModKeyFileName = formKey.ModKey.FileName,
                    FormKeyId = formKey.Id
                });
    }

    private void SaveRootEffects(PerkDTO dto)
    {
        foreach (var effect in dto.Effects)
        {
            effect.FormKey = dto.FormKey;
            effect.ModKey = dto.ModKey;
            effect.ImportedAtUTC = dto.ImportedAtUTC;
            Database.Execute(
                """
                INSERT OR REPLACE INTO PerkEffects (
                    Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                    Effect_Index, MutagenObjectType, Rank, Priority, PerkEntryID, Flags, ButtonLabel, ConditionCount,
                    EntryPoint, PerkConditionTabCount, Modification, Value, ActorValue, Spell, Quest, Stage, ImportedAtUTC)
                VALUES (
                    @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                    @EffectIndex, @MutagenObjectType, @Rank, @Priority, @PerkEntryId, @Flags, @ButtonLabel, @ConditionCount,
                    @EntryPoint, @PerkConditionTabCount, @Modification, @Value, @ActorValue, @Spell, @Quest, @Stage, @ImportedAtUTC);
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
                    effect.EffectIndex,
                    effect.MutagenObjectType,
                    effect.Rank,
                    effect.Priority,
                    effect.PerkEntryId,
                    effect.Flags,
                    ButtonLabel = GetEnglishText(effect.ButtonLabel),
                    effect.ConditionCount,
                    effect.EntryPoint,
                    effect.PerkConditionTabCount,
                    effect.Modification,
                    effect.Value,
                    effect.ActorValue,
                    effect.Spell,
                    effect.Quest,
                    effect.Stage,
                    effect.ImportedAtUTC
                });
            SaveEffectConditionTabs(dto, null, effect.EffectIndex, effect.Conditions);
        }
    }

    private void SaveEffectConditionTabs(PerkDTO dto, int? rankIndex, int effectIndex, IEnumerable<PerkEffectConditionTabDTO> conditionTabs)
    {
        foreach (var conditionTab in conditionTabs)
        {
            conditionTab.FormKey = dto.FormKey;
            conditionTab.ModKey = dto.ModKey;
            conditionTab.ImportedAtUTC = dto.ImportedAtUTC;
            Database.Execute(
                """
                INSERT OR REPLACE INTO PerkEffectConditionTabs (
                    Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                    Rank_Index, Effect_Index, ConditionTab_Index, RunOnTabIndex, ConditionCount, ImportedAtUTC)
                VALUES (
                    @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                    @RankIndex, @EffectIndex, @ConditionTabIndex, @RunOnTabIndex, @ConditionCount, @ImportedAtUTC);
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
                    RankIndex = rankIndex ?? -1,
                    EffectIndex = effectIndex,
                    conditionTab.ConditionTabIndex,
                    conditionTab.RunOnTabIndex,
                    conditionTab.ConditionCount,
                    conditionTab.ImportedAtUTC
                });
        }
    }

    private IReadOnlyList<PerkRankEffectRow> FetchRankEffectRowsByFormKey(SupportedGame game, FormKeyDTO formKey)
    {
        return Database.Fetch<PerkRankEffectRow>(
                """
                SELECT effects.*
                FROM PerkRankEffects effects
                INNER JOIN Perks perks ON perks.Game = effects.Game
                  AND perks.ModKey_Name = effects.ModKey_Name COLLATE NOCASE
                  AND perks.ModKey_Type = effects.ModKey_Type
                  AND perks.ModKey_FileName = effects.ModKey_FileName COLLATE NOCASE
                  AND perks.FormKey_ModKey_Name = effects.FormKey_ModKey_Name COLLATE NOCASE
                  AND perks.FormKey_ModKey_Type = effects.FormKey_ModKey_Type
                  AND perks.FormKey_ModKey_FileName = effects.FormKey_ModKey_FileName COLLATE NOCASE
                  AND perks.FormKey_ID = effects.FormKey_ID
                WHERE effects.Game = @Game
                  AND effects.FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
                  AND effects.FormKey_ModKey_Type = @FormKeyModKeyType
                  AND effects.FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
                  AND effects.FormKey_ID = @FormKeyId
                ORDER BY effects.ModKey_FileName COLLATE NOCASE, effects.Rank_Index, effects.Effect_Index;
                """,
                new
                {
                    Game = game.ToString(),
                    FormKeyModKeyName = formKey.ModKey.Name,
                    FormKeyModKeyType = formKey.ModKey.Type,
                    FormKeyModKeyFileName = formKey.ModKey.FileName,
                    FormKeyId = formKey.Id
                });
    }

    private IReadOnlyList<PerkRankActivityRow> FetchActivityRowsByFormKey(SupportedGame game, FormKeyDTO formKey)
    {
        return Database.Fetch<PerkRankActivityRow>(
                """
                SELECT activities.*
                FROM PerkRankActivities activities
                INNER JOIN PerkRanks ranks ON ranks.Game = activities.Game
                  AND ranks.ModKey_Name = activities.ModKey_Name COLLATE NOCASE
                  AND ranks.ModKey_Type = activities.ModKey_Type
                  AND ranks.ModKey_FileName = activities.ModKey_FileName COLLATE NOCASE
                  AND ranks.FormKey_ModKey_Name = activities.FormKey_ModKey_Name COLLATE NOCASE
                  AND ranks.FormKey_ModKey_Type = activities.FormKey_ModKey_Type
                  AND ranks.FormKey_ModKey_FileName = activities.FormKey_ModKey_FileName COLLATE NOCASE
                  AND ranks.FormKey_ID = activities.FormKey_ID
                  AND ranks.Rank_Index = activities.Rank_Index
                WHERE activities.Game = @Game
                  AND activities.FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
                  AND activities.FormKey_ModKey_Type = @FormKeyModKeyType
                  AND activities.FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
                  AND activities.FormKey_ID = @FormKeyId
                ORDER BY activities.ModKey_FileName COLLATE NOCASE, activities.Rank_Index, activities.Activity_Index;
                """,
                new
                {
                    Game = game.ToString(),
                    FormKeyModKeyName = formKey.ModKey.Name,
                    FormKeyModKeyType = formKey.ModKey.Type,
                    FormKeyModKeyFileName = formKey.ModKey.FileName,
                    FormKeyId = formKey.Id
                });
    }

    private IReadOnlyList<PerkRankActivityProgressionEvaluatorRow> FetchActivityEvaluatorRowsByFormKey(SupportedGame game, FormKeyDTO formKey)
    {
        return Database.Fetch<PerkRankActivityProgressionEvaluatorRow>(
                """
                SELECT evaluators.*
                FROM PerkRankActivityProgressionEvaluators evaluators
                INNER JOIN PerkRankActivities activities ON activities.Game = evaluators.Game
                  AND activities.ModKey_Name = evaluators.ModKey_Name COLLATE NOCASE
                  AND activities.ModKey_Type = evaluators.ModKey_Type
                  AND activities.ModKey_FileName = evaluators.ModKey_FileName COLLATE NOCASE
                  AND activities.FormKey_ModKey_Name = evaluators.FormKey_ModKey_Name COLLATE NOCASE
                  AND activities.FormKey_ModKey_Type = evaluators.FormKey_ModKey_Type
                  AND activities.FormKey_ModKey_FileName = evaluators.FormKey_ModKey_FileName COLLATE NOCASE
                  AND activities.FormKey_ID = evaluators.FormKey_ID
                  AND activities.Rank_Index = evaluators.Rank_Index
                  AND activities.Activity_Index = evaluators.Activity_Index
                WHERE evaluators.Game = @Game
                  AND evaluators.FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
                  AND evaluators.FormKey_ModKey_Type = @FormKeyModKeyType
                  AND evaluators.FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
                  AND evaluators.FormKey_ID = @FormKeyId
                ORDER BY evaluators.ModKey_FileName COLLATE NOCASE, evaluators.Rank_Index, evaluators.Activity_Index, evaluators.Evaluator_Index;
                """,
                new
                {
                    Game = game.ToString(),
                    FormKeyModKeyName = formKey.ModKey.Name,
                    FormKeyModKeyType = formKey.ModKey.Type,
                    FormKeyModKeyFileName = formKey.ModKey.FileName,
                    FormKeyId = formKey.Id
                });
    }

    private IReadOnlyList<PerkEffectRow> FetchRootEffectRowsByFormKey(SupportedGame game, FormKeyDTO formKey)
    {
        return Database.Fetch<PerkEffectRow>(
                """
                SELECT effects.*
                FROM PerkEffects effects
                INNER JOIN Perks perks ON perks.Game = effects.Game
                  AND perks.ModKey_Name = effects.ModKey_Name COLLATE NOCASE
                  AND perks.ModKey_Type = effects.ModKey_Type
                  AND perks.ModKey_FileName = effects.ModKey_FileName COLLATE NOCASE
                  AND perks.FormKey_ModKey_Name = effects.FormKey_ModKey_Name COLLATE NOCASE
                  AND perks.FormKey_ModKey_Type = effects.FormKey_ModKey_Type
                  AND perks.FormKey_ModKey_FileName = effects.FormKey_ModKey_FileName COLLATE NOCASE
                  AND perks.FormKey_ID = effects.FormKey_ID
                WHERE effects.Game = @Game
                  AND effects.FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
                  AND effects.FormKey_ModKey_Type = @FormKeyModKeyType
                  AND effects.FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
                  AND effects.FormKey_ID = @FormKeyId
                ORDER BY effects.ModKey_FileName COLLATE NOCASE, effects.Effect_Index;
                """,
                new
                {
                    Game = game.ToString(),
                    FormKeyModKeyName = formKey.ModKey.Name,
                    FormKeyModKeyType = formKey.ModKey.Type,
                    FormKeyModKeyFileName = formKey.ModKey.FileName,
                    FormKeyId = formKey.Id
                });
    }

    private IReadOnlyList<PerkEffectConditionTabRow> FetchConditionTabRowsByFormKey(SupportedGame game, FormKeyDTO formKey)
    {
        return Database.Fetch<PerkEffectConditionTabRow>(
                """
                SELECT tabs.*
                FROM PerkEffectConditionTabs tabs
                INNER JOIN Perks perks ON perks.Game = tabs.Game
                  AND perks.ModKey_Name = tabs.ModKey_Name COLLATE NOCASE
                  AND perks.ModKey_Type = tabs.ModKey_Type
                  AND perks.ModKey_FileName = tabs.ModKey_FileName COLLATE NOCASE
                  AND perks.FormKey_ModKey_Name = tabs.FormKey_ModKey_Name COLLATE NOCASE
                  AND perks.FormKey_ModKey_Type = tabs.FormKey_ModKey_Type
                  AND perks.FormKey_ModKey_FileName = tabs.FormKey_ModKey_FileName COLLATE NOCASE
                  AND perks.FormKey_ID = tabs.FormKey_ID
                WHERE tabs.Game = @Game
                  AND tabs.FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
                  AND tabs.FormKey_ModKey_Type = @FormKeyModKeyType
                  AND tabs.FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
                  AND tabs.FormKey_ID = @FormKeyId
                ORDER BY tabs.ModKey_FileName COLLATE NOCASE, tabs.Rank_Index, tabs.Effect_Index, tabs.ConditionTab_Index;
                """,
                new
                {
                    Game = game.ToString(),
                    FormKeyModKeyName = formKey.ModKey.Name,
                    FormKeyModKeyType = formKey.ModKey.Type,
                    FormKeyModKeyFileName = formKey.ModKey.FileName,
                    FormKeyId = formKey.Id
                });
    }

    private IReadOnlyList<PerkBackgroundSkillRow> FetchBackgroundSkillRowsByFormKey(SupportedGame game, FormKeyDTO formKey)
    {
        return Database.Fetch<PerkBackgroundSkillRow>(
                """
                SELECT skills.*
                FROM PerkBackgroundSkills skills
                INNER JOIN Perks perks ON perks.Game = skills.Game
                  AND perks.ModKey_Name = skills.ModKey_Name COLLATE NOCASE
                  AND perks.ModKey_Type = skills.ModKey_Type
                  AND perks.ModKey_FileName = skills.ModKey_FileName COLLATE NOCASE
                  AND perks.FormKey_ModKey_Name = skills.FormKey_ModKey_Name COLLATE NOCASE
                  AND perks.FormKey_ModKey_Type = skills.FormKey_ModKey_Type
                  AND perks.FormKey_ModKey_FileName = skills.FormKey_ModKey_FileName COLLATE NOCASE
                  AND perks.FormKey_ID = skills.FormKey_ID
                WHERE skills.Game = @Game
                  AND skills.FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
                  AND skills.FormKey_ModKey_Type = @FormKeyModKeyType
                  AND skills.FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
                  AND skills.FormKey_ID = @FormKeyId
                ORDER BY skills.ModKey_FileName COLLATE NOCASE, skills.Skill_Index;
                """,
                new
                {
                    Game = game.ToString(),
                    FormKeyModKeyName = formKey.ModKey.Name,
                    FormKeyModKeyType = formKey.ModKey.Type,
                    FormKeyModKeyFileName = formKey.ModKey.FileName,
                    FormKeyId = formKey.Id
                });
    }

    private static PerkDTO ToDTO(PerkRow record, SupportedGame game)
    {
        var dto = new PerkDTO
        {
            Game = game,
            ModKey = new ModKeyDTO { Name = string.Empty, Type = 0, FileName = string.Empty },
            FormKey = new FormKeyDTO { ModKey = new ModKeyDTO { Name = string.Empty, Type = 0, FileName = string.Empty }, Id = 0 },
            EditorID = string.Empty,
            FormVersion = 0,
            MajorRecordFlags = 0,
            ImportedAtUTC = record.ImportedAtUTC,
            Name = FromEnglish(record.Name),
            Description = FromEnglish(record.Description),
            Flags = record.Flags,
            SkillGroup = record.SkillGroup,
            CrewAssignment = record.CrewAssignment,
            PerkIcon = record.PerkIcon,
            Category = record.Category,
            RestrictionFormKey = CreateNullableFormKey(record.RestrictionModKeyName, record.RestrictionModKeyType, record.RestrictionModKeyFileName, record.RestrictionFormKeyId),
            TrainingFormKey = CreateNullableFormKey(record.TrainingModKeyName, record.TrainingModKeyType, record.TrainingModKeyFileName, record.TrainingFormKeyId),
            Level = record.Level,
            NumRanks = record.NumRanks,
            Playable = ToNullableBool(record.Playable),
            Hidden = ToNullableBool(record.Hidden),
            NextPerk = CreateNullableFormKey(record.NextPerkModKeyName, record.NextPerkModKeyType, record.NextPerkModKeyFileName, record.NextPerkFormKeyId),
            MajorFlags = record.MajorFlags
        };
        ApplyCommonFields(dto, record, game);
        return dto;
    }

    private static void ApplyLocalizedStrings(PerkDTO record, IReadOnlyList<LocalizedStringDTO> localizedStrings)
    {
        record.LocalizedStrings = localizedStrings.ToList();
        record.Name = BuildTranslatedString(localizedStrings, nameof(PerkDTO.Name), record.Name);
        record.Description = BuildTranslatedString(localizedStrings, nameof(PerkDTO.Description), record.Description);
    }

    private static void ApplyLocalizedStrings(PerkRankDTO rank, IReadOnlyList<LocalizedStringDTO> localizedStrings)
    {
        var sourceField = "Ranks[" + rank.RankIndex.ToString(System.Globalization.CultureInfo.InvariantCulture) + "]." + nameof(PerkRankDTO.Description);
        rank.Description = BuildTranslatedString(localizedStrings, sourceField, rank.Description);
    }

    private static void ApplyLocalizedStrings(PerkRankActivityDTO activity, IReadOnlyList<LocalizedStringDTO> localizedStrings)
    {
        var sourceField = "Ranks[" + activity.RankIndex.ToString(System.Globalization.CultureInfo.InvariantCulture) + "].Activities[" +
            activity.ActivityIndex.ToString(System.Globalization.CultureInfo.InvariantCulture) + "]";
        activity.Name = BuildTranslatedString(localizedStrings, sourceField + "." + nameof(PerkRankActivityDTO.Name), activity.Name);
        activity.Description = BuildTranslatedString(localizedStrings, sourceField + "." + nameof(PerkRankActivityDTO.Description), activity.Description);
    }

    private static void ApplyLocalizedStrings(PerkRankEffectDTO effect, IReadOnlyList<LocalizedStringDTO> localizedStrings)
    {
        var sourceField = "Ranks[" + effect.RankIndex.ToString(System.Globalization.CultureInfo.InvariantCulture) + "].Effects[" +
            effect.EffectIndex.ToString(System.Globalization.CultureInfo.InvariantCulture) + "]." + nameof(PerkRankEffectDTO.ButtonLabel);
        effect.ButtonLabel = BuildTranslatedString(localizedStrings, sourceField, effect.ButtonLabel);
    }

    private static void ApplyLocalizedStrings(PerkEffectDTO effect, IReadOnlyList<LocalizedStringDTO> localizedStrings)
    {
        var sourceField = "Effects[" + effect.EffectIndex.ToString(System.Globalization.CultureInfo.InvariantCulture) + "]." + nameof(PerkEffectDTO.ButtonLabel);
        effect.ButtonLabel = BuildTranslatedString(localizedStrings, sourceField, effect.ButtonLabel);
    }

    private static PerkRankDTO ToDTO(PerkRankRow row)
    {
        return new PerkRankDTO
        {
            ModKey = CreateModKey(row.ModKeyName, row.ModKeyType, row.ModKeyFileName),
            FormKey = CreateFormKey(row.FormKeyModKeyName, row.FormKeyModKeyType, row.FormKeyModKeyFileName, row.FormKeyId),
            RankIndex = row.RankIndex,
            Description = FromEnglish(row.Description),
            UnknownStaticFormKey = CreateNullableFormKey(row.UnknownStaticModKeyName, row.UnknownStaticModKeyType, row.UnknownStaticModKeyFileName, row.UnknownStaticFormKeyId),
            ConditionCount = row.ConditionCount,
            ActivityCount = row.ActivityCount,
            ImportedAtUTC = row.ImportedAtUTC
        };
    }

    private static PerkRankEffectDTO ToDTO(PerkRankEffectRow row)
    {
        return new PerkRankEffectDTO
        {
            ModKey = CreateModKey(row.ModKeyName, row.ModKeyType, row.ModKeyFileName),
            FormKey = CreateFormKey(row.FormKeyModKeyName, row.FormKeyModKeyType, row.FormKeyModKeyFileName, row.FormKeyId),
            RankIndex = row.RankIndex,
            EffectIndex = row.EffectIndex,
            MutagenObjectType = row.MutagenObjectType,
            Rank = row.Rank,
            Priority = row.Priority,
            PerkEntryId = row.PerkEntryID,
            Flags = row.Flags,
            ButtonLabel = FromEnglish(row.ButtonLabel),
            ConditionCount = row.ConditionCount,
            EntryPoint = row.EntryPoint,
            PerkConditionTabCount = row.PerkConditionTabCount,
            Modification = row.Modification,
            Value = row.Value,
            ActorValue = row.ActorValue,
            Spell = row.Spell,
            Quest = row.Quest,
            Stage = row.Stage,
            ImportedAtUTC = row.ImportedAtUTC
        };
    }

    private static PerkRankActivityDTO ToDTO(PerkRankActivityRow row)
    {
        return new PerkRankActivityDTO
        {
            ModKey = CreateModKey(row.ModKeyName, row.ModKeyType, row.ModKeyFileName),
            FormKey = CreateFormKey(row.FormKeyModKeyName, row.FormKeyModKeyType, row.FormKeyModKeyFileName, row.FormKeyId),
            RankIndex = row.RankIndex,
            ActivityIndex = row.ActivityIndex,
            ATAN = row.ATAN,
            Name = FromEnglish(row.Name),
            Description = FromEnglish(row.Description),
            ANAM = row.ANAM,
            Configuration = row.Configuration,
            ImportedAtUTC = row.ImportedAtUTC
        };
    }

    private static PerkRankActivityProgressionEvaluatorDTO ToDTO(PerkRankActivityProgressionEvaluatorRow row)
    {
        return new PerkRankActivityProgressionEvaluatorDTO
        {
            ModKey = CreateModKey(row.ModKeyName, row.ModKeyType, row.ModKeyFileName),
            FormKey = CreateFormKey(row.FormKeyModKeyName, row.FormKeyModKeyType, row.FormKeyModKeyFileName, row.FormKeyId),
            RankIndex = row.RankIndex,
            ActivityIndex = row.ActivityIndex,
            EvaluatorIndex = row.EvaluatorIndex,
            Name = row.Name,
            ImportedAtUTC = row.ImportedAtUTC
        };
    }

    private static PerkEffectDTO ToDTO(PerkEffectRow row)
    {
        return new PerkEffectDTO
        {
            ModKey = CreateModKey(row.ModKeyName, row.ModKeyType, row.ModKeyFileName),
            FormKey = CreateFormKey(row.FormKeyModKeyName, row.FormKeyModKeyType, row.FormKeyModKeyFileName, row.FormKeyId),
            EffectIndex = row.EffectIndex,
            MutagenObjectType = row.MutagenObjectType,
            Rank = row.Rank,
            Priority = row.Priority,
            PerkEntryId = row.PerkEntryID,
            Flags = row.Flags,
            ButtonLabel = FromEnglish(row.ButtonLabel),
            ConditionCount = row.ConditionCount,
            EntryPoint = row.EntryPoint,
            PerkConditionTabCount = row.PerkConditionTabCount,
            Modification = row.Modification,
            Value = row.Value,
            ActorValue = row.ActorValue,
            Spell = row.Spell,
            Quest = row.Quest,
            Stage = row.Stage,
            ImportedAtUTC = row.ImportedAtUTC
        };
    }

    private static List<PerkEffectConditionTabDTO> GetEffectConditionTabs(
        IReadOnlyList<PerkEffectConditionTabRow> conditionTabRows,
        IReadOnlyList<ConditionFormConditionDTO> conditions,
        ModKeyDTO modKey,
        int? rankIndex,
        int effectIndex)
    {
        var rankKey = rankIndex ?? -1;
        return conditionTabRows
            .Where(tab => IsSameModKey(tab, modKey) && tab.RankIndex == rankKey && tab.EffectIndex == effectIndex)
            .OrderBy(tab => tab.ConditionTabIndex)
            .Select(tab =>
            {
                var dto = ToDTO(tab);
                dto.RankIndex = rankIndex;
                var slot = GetPerkEffectConditionSlot(rankIndex, effectIndex, tab.ConditionTabIndex);
                dto.Conditions = conditions
                    .Where(condition => string.Equals(condition.ConditionSlot, slot, StringComparison.Ordinal))
                    .OrderBy(condition => condition.ConditionIndex)
                    .ToList();
                return dto;
            })
            .ToList();
    }

    private static PerkEffectConditionTabDTO ToDTO(PerkEffectConditionTabRow row)
    {
        return new PerkEffectConditionTabDTO
        {
            ModKey = CreateModKey(row.ModKeyName, row.ModKeyType, row.ModKeyFileName),
            FormKey = CreateFormKey(row.FormKeyModKeyName, row.FormKeyModKeyType, row.FormKeyModKeyFileName, row.FormKeyId),
            RankIndex = row.RankIndex < 0 ? null : row.RankIndex,
            EffectIndex = row.EffectIndex,
            ConditionTabIndex = row.ConditionTabIndex,
            RunOnTabIndex = row.RunOnTabIndex,
            ConditionCount = row.ConditionCount,
            ImportedAtUTC = row.ImportedAtUTC
        };
    }

    private static PerkBackgroundSkillDTO ToDTO(PerkBackgroundSkillRow row)
    {
        return new PerkBackgroundSkillDTO
        {
            ModKey = CreateModKey(row.ModKeyName, row.ModKeyType, row.ModKeyFileName),
            FormKey = CreateFormKey(row.FormKeyModKeyName, row.FormKeyModKeyType, row.FormKeyModKeyFileName, row.FormKeyId),
            SkillFormKey = CreateFormKey(row.SkillModKeyName, row.SkillModKeyType, row.SkillModKeyFileName, row.SkillFormKeyId),
            SkillIndex = row.SkillIndex,
            ImportedAtUTC = row.ImportedAtUTC
        };
    }

    private static ModKeyDTO CreateModKey(string name, int type, string fileName)
    {
        return new ModKeyDTO
        {
            Name = name,
            Type = type,
            FileName = fileName
        };
    }

    private static FormKeyDTO CreateFormKey(string modKeyName, int modKeyType, string modKeyFileName, long formKeyId)
    {
        return new FormKeyDTO
        {
            ModKey = CreateModKey(modKeyName, modKeyType, modKeyFileName),
            Id = (uint)formKeyId
        };
    }

    private static int? ToNullableInt(bool? value)
    {
        return value.HasValue ? (value.Value ? 1 : 0) : null;
    }

    private static bool? ToNullableBool(int? value)
    {
        return value.HasValue ? value.Value != 0 : null;
    }

    private static string GetPerkRankConditionSlot(int rankIndex)
    {
        return "Ranks[" + rankIndex.ToString(System.Globalization.CultureInfo.InvariantCulture) + "].Conditions";
    }

    private static string GetPerkEffectConditionSlot(int? rankIndex, int effectIndex, int conditionTabIndex)
    {
        var effectPath = rankIndex.HasValue
            ? "Ranks[" + rankIndex.Value.ToString(System.Globalization.CultureInfo.InvariantCulture) + "].Effects[" + effectIndex.ToString(System.Globalization.CultureInfo.InvariantCulture) + "]"
            : "Effects[" + effectIndex.ToString(System.Globalization.CultureInfo.InvariantCulture) + "]";
        return effectPath + ".Conditions[" + conditionTabIndex.ToString(System.Globalization.CultureInfo.InvariantCulture) + "].Conditions";
    }

    private static string GetPerkActivityEvaluatorConditionSlot(int rankIndex, int activityIndex, int evaluatorIndex)
    {
        return "Ranks[" + rankIndex.ToString(System.Globalization.CultureInfo.InvariantCulture) + "].Activities[" +
            activityIndex.ToString(System.Globalization.CultureInfo.InvariantCulture) + "].ProgressionEvalutor[" +
            evaluatorIndex.ToString(System.Globalization.CultureInfo.InvariantCulture) + "].Conditions";
    }

    private static bool IsSameModKey(ModKeyDTO first, ModKeyDTO second)
    {
        return first.Type == second.Type &&
            string.Equals(first.Name, second.Name, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(first.FileName, second.FileName, StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsSameModKey(PerkChildRow row, ModKeyDTO modKey)
    {
        return row.ModKeyType == modKey.Type &&
            string.Equals(row.ModKeyName, modKey.Name, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(row.ModKeyFileName, modKey.FileName, StringComparison.OrdinalIgnoreCase);
    }

    private sealed class PerkRow : RecordRow
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string Flags { get; set; } = string.Empty;
        public string? SkillGroup { get; set; }
        public string? CrewAssignment { get; set; }
        public string? PerkIcon { get; set; }
        public string? Category { get; set; }
        public string? RestrictionModKeyName { get; set; }
        public int? RestrictionModKeyType { get; set; }
        public string? RestrictionModKeyFileName { get; set; }
        public long? RestrictionFormKeyId { get; set; }
        public string? TrainingModKeyName { get; set; }
        public int? TrainingModKeyType { get; set; }
        public string? TrainingModKeyFileName { get; set; }
        public long? TrainingFormKeyId { get; set; }
        public int? Level { get; set; }
        public int? NumRanks { get; set; }
        public int? Playable { get; set; }
        public int? Hidden { get; set; }
        public string? NextPerkModKeyName { get; set; }
        public int? NextPerkModKeyType { get; set; }
        public string? NextPerkModKeyFileName { get; set; }
        public long? NextPerkFormKeyId { get; set; }
        public string? MajorFlags { get; set; }
    }

    private abstract class PerkChildRow
    {
        public string Game { get; set; } = string.Empty;
        public string ModKeyName { get; set; } = string.Empty;
        public int ModKeyType { get; set; }
        public string ModKeyFileName { get; set; } = string.Empty;
        public string FormKeyModKeyName { get; set; } = string.Empty;
        public int FormKeyModKeyType { get; set; }
        public string FormKeyModKeyFileName { get; set; } = string.Empty;
        public long FormKeyId { get; set; }
        public DateTime ImportedAtUTC { get; set; }
    }

    private sealed class PerkRankRow : PerkChildRow
    {
        public int RankIndex { get; set; }
        public string? Description { get; set; }
        public string? UnknownStaticModKeyName { get; set; }
        public int? UnknownStaticModKeyType { get; set; }
        public string? UnknownStaticModKeyFileName { get; set; }
        public long? UnknownStaticFormKeyId { get; set; }
        public int ConditionCount { get; set; }
        public int ActivityCount { get; set; }
    }

    private sealed class PerkRankEffectRow : PerkChildRow
    {
        public int RankIndex { get; set; }
        public int EffectIndex { get; set; }
        public string MutagenObjectType { get; set; } = string.Empty;
        public int Rank { get; set; }
        public int Priority { get; set; }
        public int? PerkEntryID { get; set; }
        public string? Flags { get; set; }
        public string? ButtonLabel { get; set; }
        public int ConditionCount { get; set; }
        public string? EntryPoint { get; set; }
        public int? PerkConditionTabCount { get; set; }
        public string? Modification { get; set; }
        public double? Value { get; set; }
        public string? ActorValue { get; set; }
        public string? Spell { get; set; }
        public string? Quest { get; set; }
        public int? Stage { get; set; }
    }

    private sealed class PerkRankActivityRow : PerkChildRow
    {
        public int RankIndex { get; set; }
        public int ActivityIndex { get; set; }
        public string? ATAN { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? ANAM { get; set; }
        public string? Configuration { get; set; }
    }

    private sealed class PerkRankActivityProgressionEvaluatorRow : PerkChildRow
    {
        public int RankIndex { get; set; }
        public int ActivityIndex { get; set; }
        public int EvaluatorIndex { get; set; }
        public string? Name { get; set; }
    }

    private sealed class PerkEffectRow : PerkChildRow
    {
        public int EffectIndex { get; set; }
        public string MutagenObjectType { get; set; } = string.Empty;
        public int? Rank { get; set; }
        public int? Priority { get; set; }
        public int? PerkEntryID { get; set; }
        public string? Flags { get; set; }
        public string? ButtonLabel { get; set; }
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

    private sealed class PerkEffectConditionTabRow : PerkChildRow
    {
        public int RankIndex { get; set; }
        public int EffectIndex { get; set; }
        public int ConditionTabIndex { get; set; }
        public int? RunOnTabIndex { get; set; }
        public int ConditionCount { get; set; }
    }

    private sealed class PerkBackgroundSkillRow : PerkChildRow
    {
        public string SkillModKeyName { get; set; } = string.Empty;
        public int SkillModKeyType { get; set; }
        public string SkillModKeyFileName { get; set; } = string.Empty;
        public long SkillFormKeyId { get; set; }
        public int SkillIndex { get; set; }
    }

    private void SaveBackgroundSkills(PerkDTO dto)
    {
        foreach (var backgroundSkill in dto.BackgroundSkills)
        {
            backgroundSkill.FormKey = dto.FormKey;
            backgroundSkill.ModKey = dto.ModKey;
            backgroundSkill.ImportedAtUTC = dto.ImportedAtUTC;
            Database.Execute(
                """
                INSERT OR REPLACE INTO PerkBackgroundSkills (
                    Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                    Skill_ModKey_Name, Skill_ModKey_Type, Skill_ModKey_FileName, Skill_FormKey_ID, Skill_Index, ImportedAtUTC)
                VALUES (
                    @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                    @SkillModKeyName, @SkillModKeyType, @SkillModKeyFileName, @SkillFormKeyId, @SkillIndex, @ImportedAtUTC);
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
                    SkillModKeyName = backgroundSkill.SkillFormKey.ModKey.Name,
                    SkillModKeyType = backgroundSkill.SkillFormKey.ModKey.Type,
                    SkillModKeyFileName = backgroundSkill.SkillFormKey.ModKey.FileName,
                    SkillFormKeyId = backgroundSkill.SkillFormKey.Id,
                    backgroundSkill.SkillIndex,
                    backgroundSkill.ImportedAtUTC
                });
        }
    }

    private void DeleteChildren(PerkDTO dto)
    {
        var parameters = new
        {
            Game = dto.Game.ToString(),
            ModKeyName = dto.ModKey.Name,
            ModKeyType = dto.ModKey.Type,
            ModKeyFileName = dto.ModKey.FileName,
            FormKeyModKeyName = dto.FormKey.ModKey.Name,
            FormKeyModKeyType = dto.FormKey.ModKey.Type,
            FormKeyModKeyFileName = dto.FormKey.ModKey.FileName,
            FormKeyId = dto.FormKey.Id
        };
        Database.Execute(
            """
            DELETE FROM PerkEffectConditionTabs
            WHERE Game = @Game
              AND ModKey_Name = @ModKeyName
              AND ModKey_Type = @ModKeyType
              AND ModKey_FileName = @ModKeyFileName
              AND FormKey_ModKey_Name = @FormKeyModKeyName
              AND FormKey_ModKey_Type = @FormKeyModKeyType
              AND FormKey_ModKey_FileName = @FormKeyModKeyFileName
              AND FormKey_ID = @FormKeyId;
            """,
            parameters);
        Database.Execute(
            """
            DELETE FROM PerkEffects
            WHERE Game = @Game
              AND ModKey_Name = @ModKeyName
              AND ModKey_Type = @ModKeyType
              AND ModKey_FileName = @ModKeyFileName
              AND FormKey_ModKey_Name = @FormKeyModKeyName
              AND FormKey_ModKey_Type = @FormKeyModKeyType
              AND FormKey_ModKey_FileName = @FormKeyModKeyFileName
              AND FormKey_ID = @FormKeyId;
            """,
            parameters);
        Database.Execute(
            """
            DELETE FROM PerkRankActivityProgressionEvaluators
            WHERE Game = @Game
              AND ModKey_Name = @ModKeyName
              AND ModKey_Type = @ModKeyType
              AND ModKey_FileName = @ModKeyFileName
              AND FormKey_ModKey_Name = @FormKeyModKeyName
              AND FormKey_ModKey_Type = @FormKeyModKeyType
              AND FormKey_ModKey_FileName = @FormKeyModKeyFileName
              AND FormKey_ID = @FormKeyId;
            """,
            parameters);
        Database.Execute(
            """
            DELETE FROM PerkRankActivities
            WHERE Game = @Game
              AND ModKey_Name = @ModKeyName
              AND ModKey_Type = @ModKeyType
              AND ModKey_FileName = @ModKeyFileName
              AND FormKey_ModKey_Name = @FormKeyModKeyName
              AND FormKey_ModKey_Type = @FormKeyModKeyType
              AND FormKey_ModKey_FileName = @FormKeyModKeyFileName
              AND FormKey_ID = @FormKeyId;
            """,
            parameters);
        Database.Execute(
            """
            DELETE FROM PerkRanks
            WHERE Game = @Game
              AND ModKey_Name = @ModKeyName
              AND ModKey_Type = @ModKeyType
              AND ModKey_FileName = @ModKeyFileName
              AND FormKey_ModKey_Name = @FormKeyModKeyName
              AND FormKey_ModKey_Type = @FormKeyModKeyType
              AND FormKey_ModKey_FileName = @FormKeyModKeyFileName
              AND FormKey_ID = @FormKeyId;
            """,
            parameters);
        Database.Execute(
            """
            DELETE FROM PerkBackgroundSkills
            WHERE Game = @Game
              AND ModKey_Name = @ModKeyName
              AND ModKey_Type = @ModKeyType
              AND ModKey_FileName = @ModKeyFileName
              AND FormKey_ModKey_Name = @FormKeyModKeyName
              AND FormKey_ModKey_Type = @FormKeyModKeyType
              AND FormKey_ModKey_FileName = @FormKeyModKeyFileName
              AND FormKey_ID = @FormKeyId;
            """,
            parameters);
    }
}
