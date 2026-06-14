using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Repositories.Interfaces;
using NPoco;

namespace CreationsForge.Core.Repositories;

public class PerkRepository : TypedRecordRepositoryBase, IPerkRepository
{
    public PerkRepository(IDatabase database, IRecordInstanceRepository recordInstanceRepository)
        : base(database, recordInstanceRepository)
    { }

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
                    SelectColumn("MajorFlags")
                ])
            .Select(record => ToDTO(record, game))
            .ToList();
        var rankRows = FetchRankRowsByFormKey(game, formKey);
        var effectRows = FetchRankEffectRowsByFormKey(game, formKey);
        var backgroundSkillRows = FetchBackgroundSkillRowsByFormKey(game, formKey);
        foreach (var record in records)
        {
            record.Ranks = rankRows
                .Where(rank => IsSameModKey(rank, record.ModKey))
                .OrderBy(rank => rank.RankIndex)
                .Select(ToDTO)
                .ToList();
            foreach (var rank in record.Ranks)
            {
                rank.Effects = effectRows
                    .Where(effect => IsSameModKey(effect, record.ModKey) && effect.RankIndex == rank.RankIndex)
                    .OrderBy(effect => effect.EffectIndex)
                    .Select(ToDTO)
                    .ToList();
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
                Training_ModKey_Name, Training_ModKey_Type, Training_ModKey_FileName, Training_FormKey_ID, MajorFlags)
            VALUES (
                @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                @EditorId, @FormVersion, @MajorRecordFlags, @ImportedAtUTC, @Name, @Description, @Flags, @SkillGroup, @CrewAssignment, @PerkIcon, @Category,
                @RestrictionModKeyName, @RestrictionModKeyType, @RestrictionModKeyFileName, @RestrictionFormKeyId,
                @TrainingModKeyName, @TrainingModKeyType, @TrainingModKeyFileName, @TrainingFormKeyId, @MajorFlags);
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
                dto.Name,
                dto.Description,
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
                dto.MajorFlags
            });
        DeleteChildren(dto);
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
                    rank.Description,
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
                        EntryPoint, PerkConditionTabCount, Modification, Value, ImportedAtUTC)
                    VALUES (
                        @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                        @RankIndex, @EffectIndex, @MutagenObjectType, @Rank, @Priority, @PerkEntryId, @Flags, @ButtonLabel, @ConditionCount,
                        @EntryPoint, @PerkConditionTabCount, @Modification, @Value, @ImportedAtUTC);
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
                        effect.ButtonLabel,
                        effect.ConditionCount,
                        effect.EntryPoint,
                        effect.PerkConditionTabCount,
                        effect.Modification,
                        effect.Value,
                        effect.ImportedAtUTC
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
            Name = record.Name,
            Description = record.Description,
            Flags = record.Flags,
            SkillGroup = record.SkillGroup,
            CrewAssignment = record.CrewAssignment,
            PerkIcon = record.PerkIcon,
            Category = record.Category,
            RestrictionFormKey = CreateNullableFormKey(record.RestrictionModKeyName, record.RestrictionModKeyType, record.RestrictionModKeyFileName, record.RestrictionFormKeyId),
            TrainingFormKey = CreateNullableFormKey(record.TrainingModKeyName, record.TrainingModKeyType, record.TrainingModKeyFileName, record.TrainingFormKeyId),
            MajorFlags = record.MajorFlags
        };
        ApplyCommonFields(dto, record, game);
        return dto;
    }

    private static PerkRankDTO ToDTO(PerkRankRow row)
    {
        return new PerkRankDTO
        {
            ModKey = CreateModKey(row.ModKeyName, row.ModKeyType, row.ModKeyFileName),
            FormKey = CreateFormKey(row.FormKeyModKeyName, row.FormKeyModKeyType, row.FormKeyModKeyFileName, row.FormKeyId),
            RankIndex = row.RankIndex,
            Description = row.Description,
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
            ButtonLabel = row.ButtonLabel,
            ConditionCount = row.ConditionCount,
            EntryPoint = row.EntryPoint,
            PerkConditionTabCount = row.PerkConditionTabCount,
            Modification = row.Modification,
            Value = row.Value,
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
