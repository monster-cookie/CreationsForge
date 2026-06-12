using CreationsForge.Core.DTOs.Records;
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

    public IReadOnlyList<PerkDTO> GetByFormKey(CreationsForge.Core.Enums.SupportedGame game, CreationsForge.Core.DTOs.Plugins.FormKeyDTO formKey)
    {
        return FetchByFormKey<PerkRow>(
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

    private static PerkDTO ToDTO(PerkRow record, CreationsForge.Core.Enums.SupportedGame game)
    {
        var dto = new PerkDTO
        {
            Game = game,
            ModKey = new CreationsForge.Core.DTOs.Plugins.ModKeyDTO { Name = string.Empty, Type = 0, FileName = string.Empty },
            FormKey = new CreationsForge.Core.DTOs.Plugins.FormKeyDTO { ModKey = new CreationsForge.Core.DTOs.Plugins.ModKeyDTO { Name = string.Empty, Type = 0, FileName = string.Empty }, Id = 0 },
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

    private void SaveBackgroundSkills(PerkDTO dto)
    {
        foreach (var backgroundSkill in dto.BackgroundSkills)
        {
            backgroundSkill.FormKey = dto.FormKey;
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
