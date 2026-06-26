using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Repositories.Interfaces;
using NPoco;

namespace CreationsForge.Core.Repositories;

public class ActorValueInformationRepository : TypedRecordRepositoryBase, IActorValueInformationRepository
{
    private readonly IRecordLocalizedStringRepository RecordLocalizedStringRepository;

    public ActorValueInformationRepository(
        IDatabase database,
        IRecordInstanceRepository recordInstanceRepository,
        IRecordLocalizedStringRepository recordLocalizedStringRepository)
        : base(database, recordInstanceRepository)
    {
        RecordLocalizedStringRepository = recordLocalizedStringRepository;
    }

    public override string RecordType => RecordTypeCatalog.ActorValueInformation.RecordID;

    protected override string TableName => RecordTypeCatalog.ActorValueInformation.TableName;

    public IReadOnlyList<ActorValueInformationDTO> GetByFormKey(SupportedGame game, FormKeyDTO formKey)
    {
        var records = FetchByFormKey<ActorValueInformationRow>(
                game,
                formKey,
                [
                    SelectColumn("Name"),
                    SelectColumn("Abbreviation"),
                    SelectColumn("Description"),
                    SelectColumn("Version2"),
                    SelectColumn("VersionControl"),
                    SelectColumn("CNAM"),
                    SelectColumn("Skill_ImproveMult", "SkillImproveMult"),
                    SelectColumn("Skill_ImproveOffset", "SkillImproveOffset"),
                    SelectColumn("Skill_UseMult", "SkillUseMult"),
                    SelectColumn("ContextNotes"),
                    SelectColumn("DefaultValue"),
                    SelectColumn("Flags"),
                    SelectColumn("Type"),
                    SelectColumn("Min"),
                    SelectColumn("Max")
                ])
            .Select(record => ToDTO(record, game))
            .ToList();
        var perkTreeEntries = FetchPerkTreeEntriesByFormKey(game, formKey);
        var connectionLineIndices = FetchConnectionLineIndicesByFormKey(game, formKey);
        var localizedStrings = RecordLocalizedStringRepository.GetByFormKey(game, RecordTypeCatalog.ActorValueInformation.RecordID, formKey);
        foreach (var record in records)
        {
            record.PerkTree = perkTreeEntries.Where(entry => IsSameModKey(entry.ModKey, record.ModKey)).OrderBy(entry => entry.PerkTreeIndex).ToList();
            foreach (var perkTreeEntry in record.PerkTree)
            {
                perkTreeEntry.ConnectionLineToIndices = connectionLineIndices
                    .Where(connectionLineIndex => IsSameModKey(connectionLineIndex.ModKey, record.ModKey) &&
                                                  connectionLineIndex.PerkTreeIndex == perkTreeEntry.PerkTreeIndex)
                    .OrderBy(connectionLineIndex => connectionLineIndex.ConnectionLineIndex)
                    .ToList();
            }

            ApplyLocalizedStrings(record, localizedStrings.Where(localizedString => IsSameModKey(localizedString.ModKey, record.ModKey)).ToList());
        }

        return records;
    }

    public void Save(ActorValueInformationDTO dto)
    {
        SaveRecordInstance(dto);
        Database.Execute(
            """
            INSERT OR REPLACE INTO ActorValueInformation (
                Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                EditorID, FormVersion, MajorRecordFlags, ImportedAtUTC, Version2, VersionControl, Name, Abbreviation, Description, CNAM, Skill_ImproveMult, Skill_ImproveOffset,
                Skill_UseMult, ContextNotes, DefaultValue, Flags, Type, Min, Max)
            VALUES (
                @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                @EditorId, @FormVersion, @MajorRecordFlags, @ImportedAtUTC, @Version2, @VersionControl, @Name, @Abbreviation, @Description, @CNAM, @SkillImproveMult, @SkillImproveOffset,
                @SkillUseMult, @ContextNotes, @DefaultValue, @Flags, @Type, @Min, @Max);
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
                dto.Version2,
                dto.VersionControl,
                Name = GetEnglishText(dto.Name),
                Abbreviation = GetEnglishText(dto.Abbreviation),
                Description = GetEnglishText(dto.Description),
                dto.CNAM,
                SkillImproveMult = dto.Skill?.ImproveMult,
                SkillImproveOffset = dto.Skill?.ImproveOffset,
                SkillUseMult = dto.Skill?.UseMult,
                dto.ContextNotes,
                dto.DefaultValue,
                dto.Flags,
                dto.Type,
                dto.Min,
                dto.Max
            });
        ReplacePerkTreeEntries(dto);
    }

    private IReadOnlyList<ActorValueInformationPerkTreeEntryDTO> FetchPerkTreeEntriesByFormKey(SupportedGame game, FormKeyDTO formKey)
    {
        return Database.Fetch<ActorValueInformationPerkTreeEntryRow>(
                """
                SELECT *
                FROM ActorValueInformationPerkTreeEntries
                WHERE Game = @Game
                  AND FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
                  AND FormKey_ModKey_Type = @FormKeyModKeyType
                  AND FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
                  AND FormKey_ID = @FormKeyId
                ORDER BY ModKey_FileName COLLATE NOCASE, PerkTree_Index;
                """,
                new
                {
                    Game = game.ToString(),
                    FormKeyModKeyName = formKey.ModKey.Name,
                    FormKeyModKeyType = formKey.ModKey.Type,
                    FormKeyModKeyFileName = formKey.ModKey.FileName,
                    FormKeyId = formKey.Id
                })
            .Select(row => ToDTO(row, game))
            .ToList();
    }

    private IReadOnlyList<ActorValueInformationConnectionLineIndexDTO> FetchConnectionLineIndicesByFormKey(SupportedGame game, FormKeyDTO formKey)
    {
        return Database.Fetch<ActorValueInformationConnectionLineIndexRow>(
                """
                SELECT *
                FROM ActorValueInformationPerkTreeConnectionLineIndices
                WHERE Game = @Game
                  AND FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
                  AND FormKey_ModKey_Type = @FormKeyModKeyType
                  AND FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
                  AND FormKey_ID = @FormKeyId
                ORDER BY ModKey_FileName COLLATE NOCASE, PerkTree_Index, ConnectionLine_Index;
                """,
                new
                {
                    Game = game.ToString(),
                    FormKeyModKeyName = formKey.ModKey.Name,
                    FormKeyModKeyType = formKey.ModKey.Type,
                    FormKeyModKeyFileName = formKey.ModKey.FileName,
                    FormKeyId = formKey.Id
                })
            .Select(row => ToDTO(row, game))
            .ToList();
    }

    private void ReplacePerkTreeEntries(ActorValueInformationDTO dto)
    {
        Database.Execute(
            """
            DELETE FROM ActorValueInformationPerkTreeEntries
            WHERE Game = @Game
              AND ModKey_Name = @ModKeyName
              AND ModKey_Type = @ModKeyType
              AND ModKey_FileName = @ModKeyFileName
              AND FormKey_ModKey_Name = @FormKeyModKeyName
              AND FormKey_ModKey_Type = @FormKeyModKeyType
              AND FormKey_ModKey_FileName = @FormKeyModKeyFileName
              AND FormKey_ID = @FormKeyId;
            """,
            CommonParameters(dto));

        foreach (var entry in dto.PerkTree)
        {
            entry.ImportedAtUTC = dto.ImportedAtUTC;
            Database.Execute(
                """
                INSERT OR REPLACE INTO ActorValueInformationPerkTreeEntries (
                    Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                    PerkTree_Index, AssociatedSkill_ModKey_Name, AssociatedSkill_ModKey_Type, AssociatedSkill_ModKey_FileName, AssociatedSkill_FormKey_ID,
                    FNAM, HorizontalPosition, EntryIndex, PerkGridX, PerkGridY, VerticalPosition,
                    Perk_ModKey_Name, Perk_ModKey_Type, Perk_ModKey_FileName, Perk_FormKey_ID, ImportedAtUTC)
                VALUES (
                    @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                    @PerkTreeIndex, @AssociatedSkillModKeyName, @AssociatedSkillModKeyType, @AssociatedSkillModKeyFileName, @AssociatedSkillFormKeyId,
                    @FNAM, @HorizontalPosition, @EntryIndex, @PerkGridX, @PerkGridY, @VerticalPosition,
                    @PerkModKeyName, @PerkModKeyType, @PerkModKeyFileName, @PerkFormKeyId, @ImportedAtUTC);
                """,
                new
                {
                    Game = entry.Game.ToString(),
                    ModKeyName = entry.ModKey.Name,
                    ModKeyType = entry.ModKey.Type,
                    ModKeyFileName = entry.ModKey.FileName,
                    FormKeyModKeyName = entry.FormKey.ModKey.Name,
                    FormKeyModKeyType = entry.FormKey.ModKey.Type,
                    FormKeyModKeyFileName = entry.FormKey.ModKey.FileName,
                    FormKeyId = entry.FormKey.Id,
                    entry.PerkTreeIndex,
                    AssociatedSkillModKeyName = entry.AssociatedSkill?.ModKey.Name,
                    AssociatedSkillModKeyType = entry.AssociatedSkill?.ModKey.Type,
                    AssociatedSkillModKeyFileName = entry.AssociatedSkill?.ModKey.FileName,
                    AssociatedSkillFormKeyId = entry.AssociatedSkill?.Id,
                    entry.FNAM,
                    entry.HorizontalPosition,
                    EntryIndex = entry.Index,
                    entry.PerkGridX,
                    entry.PerkGridY,
                    entry.VerticalPosition,
                    PerkModKeyName = entry.Perk?.ModKey.Name,
                    PerkModKeyType = entry.Perk?.ModKey.Type,
                    PerkModKeyFileName = entry.Perk?.ModKey.FileName,
                    PerkFormKeyId = entry.Perk?.Id,
                    entry.ImportedAtUTC
                });
            ReplaceConnectionLineIndices(entry);
        }
    }

    private void ReplaceConnectionLineIndices(ActorValueInformationPerkTreeEntryDTO entry)
    {
        foreach (var connectionLineIndex in entry.ConnectionLineToIndices)
        {
            connectionLineIndex.Game = entry.Game;
            connectionLineIndex.ModKey = entry.ModKey;
            connectionLineIndex.FormKey = entry.FormKey;
            connectionLineIndex.PerkTreeIndex = entry.PerkTreeIndex;
            connectionLineIndex.ImportedAtUTC = entry.ImportedAtUTC;
            Database.Execute(
                """
                INSERT OR REPLACE INTO ActorValueInformationPerkTreeConnectionLineIndices (
                    Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                    PerkTree_Index, ConnectionLine_Index, TargetIndex, ImportedAtUTC)
                VALUES (
                    @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                    @PerkTreeIndex, @ConnectionLineIndex, @TargetIndex, @ImportedAtUTC);
                """,
                new
                {
                    Game = connectionLineIndex.Game.ToString(),
                    ModKeyName = connectionLineIndex.ModKey.Name,
                    ModKeyType = connectionLineIndex.ModKey.Type,
                    ModKeyFileName = connectionLineIndex.ModKey.FileName,
                    FormKeyModKeyName = connectionLineIndex.FormKey.ModKey.Name,
                    FormKeyModKeyType = connectionLineIndex.FormKey.ModKey.Type,
                    FormKeyModKeyFileName = connectionLineIndex.FormKey.ModKey.FileName,
                    FormKeyId = connectionLineIndex.FormKey.Id,
                    connectionLineIndex.PerkTreeIndex,
                    connectionLineIndex.ConnectionLineIndex,
                    connectionLineIndex.TargetIndex,
                    connectionLineIndex.ImportedAtUTC
                });
        }
    }

    private static ActorValueInformationDTO ToDTO(ActorValueInformationRow record, SupportedGame game)
    {
        var dto = new ActorValueInformationDTO
        {
            Game = game,
            ModKey = new ModKeyDTO { Name = string.Empty, Type = 0, FileName = string.Empty },
            FormKey = new FormKeyDTO { ModKey = new ModKeyDTO { Name = string.Empty, Type = 0, FileName = string.Empty }, Id = 0 },
            EditorID = string.Empty,
            FormVersion = 0,
            MajorRecordFlags = 0,
            ImportedAtUTC = record.ImportedAtUTC,
            Name = FromEnglish(record.Name),
            Abbreviation = FromEnglish(record.Abbreviation),
            Description = FromEnglish(record.Description),
            Version2 = record.Version2,
            VersionControl = record.VersionControl,
            CNAM = record.CNAM,
            Skill = CreateSkill(record.SkillUseMult, record.SkillImproveMult, record.SkillImproveOffset),
            ContextNotes = record.ContextNotes,
            DefaultValue = record.DefaultValue,
            Flags = record.Flags,
            Type = record.Type,
            Min = record.Min,
            Max = record.Max
        };
        ApplyCommonFields(dto, record, game);
        return dto;
    }

    private static ActorValueInformationPerkTreeEntryDTO ToDTO(ActorValueInformationPerkTreeEntryRow row, SupportedGame game)
    {
        return new ActorValueInformationPerkTreeEntryDTO
        {
            Game = game,
            ModKey = CreateModKey(row.ModKeyName, row.ModKeyType, row.ModKeyFileName),
            FormKey = CreateFormKey(row.FormKeyModKeyName, row.FormKeyModKeyType, row.FormKeyModKeyFileName, row.FormKeyId),
            PerkTreeIndex = row.PerkTreeIndex,
            AssociatedSkill = CreateNullableFormKey(row.AssociatedSkillModKeyName, row.AssociatedSkillModKeyType, row.AssociatedSkillModKeyFileName, row.AssociatedSkillFormKeyId),
            FNAM = row.FNAM,
            HorizontalPosition = row.HorizontalPosition,
            Index = row.EntryIndex,
            PerkGridX = row.PerkGridX,
            PerkGridY = row.PerkGridY,
            VerticalPosition = row.VerticalPosition,
            Perk = CreateNullableFormKey(row.PerkModKeyName, row.PerkModKeyType, row.PerkModKeyFileName, row.PerkFormKeyId),
            ImportedAtUTC = row.ImportedAtUTC
        };
    }

    private static ActorValueInformationConnectionLineIndexDTO ToDTO(ActorValueInformationConnectionLineIndexRow row, SupportedGame game)
    {
        return new ActorValueInformationConnectionLineIndexDTO
        {
            Game = game,
            ModKey = CreateModKey(row.ModKeyName, row.ModKeyType, row.ModKeyFileName),
            FormKey = CreateFormKey(row.FormKeyModKeyName, row.FormKeyModKeyType, row.FormKeyModKeyFileName, row.FormKeyId),
            PerkTreeIndex = row.PerkTreeIndex,
            ConnectionLineIndex = row.ConnectionLineIndex,
            TargetIndex = row.TargetIndex,
            ImportedAtUTC = row.ImportedAtUTC
        };
    }

    private static void ApplyLocalizedStrings(ActorValueInformationDTO record, IReadOnlyList<LocalizedStringDTO> localizedStrings)
    {
        record.LocalizedStrings = localizedStrings.ToList();
        record.Name = GetTranslatedString(localizedStrings, nameof(ActorValueInformationDTO.Name), record.Name);
        record.Abbreviation = GetTranslatedString(localizedStrings, nameof(ActorValueInformationDTO.Abbreviation), record.Abbreviation);
        record.Description = GetTranslatedString(localizedStrings, nameof(ActorValueInformationDTO.Description), record.Description);
    }

    private static TranslatedStringDTO? GetTranslatedString(IReadOnlyList<LocalizedStringDTO> localizedStrings, string sourceField, TranslatedStringDTO? fallback)
    {
        var strings = localizedStrings
            .Where(localizedString => string.Equals(localizedString.SourceField, sourceField, StringComparison.OrdinalIgnoreCase))
            .GroupBy(localizedString => localizedString.Language, StringComparer.OrdinalIgnoreCase)
            .Select(group => group.First())
            .Select(localizedString => new TranslatedStringValueDTO
            {
                Language = localizedString.Language,
                String = localizedString.Value
            })
            .ToList();

        return strings.Count == 0
            ? fallback
            : new TranslatedStringDTO { Strings = strings };
    }

    private static ActorValueInformationSkillDTO? CreateSkill(double? useMult, double? improveMult, double? improveOffset)
    {
        if (useMult is null && improveMult is null && improveOffset is null)
        {
            return null;
        }

        return new ActorValueInformationSkillDTO
        {
            UseMult = useMult,
            ImproveMult = improveMult,
            ImproveOffset = improveOffset
        };
    }

    private sealed class ActorValueInformationRow : RecordRow
    {
        public string? Name { get; set; }

        public string? Abbreviation { get; set; }

        public string? Description { get; set; }

        public int? Version2 { get; set; }

        public int? VersionControl { get; set; }

        public string? CNAM { get; set; }

        public double? SkillImproveMult { get; set; }

        public double? SkillImproveOffset { get; set; }

        public double? SkillUseMult { get; set; }

        public string? ContextNotes { get; set; }

        public double? DefaultValue { get; set; }

        public string? Flags { get; set; }

        public string? Type { get; set; }

        public double? Min { get; set; }

        public double? Max { get; set; }
    }

    private sealed class ActorValueInformationPerkTreeEntryRow
    {
        public string ModKeyName { get; set; } = string.Empty;
        public int ModKeyType { get; set; }
        public string ModKeyFileName { get; set; } = string.Empty;
        public string FormKeyModKeyName { get; set; } = string.Empty;
        public int FormKeyModKeyType { get; set; }
        public string FormKeyModKeyFileName { get; set; } = string.Empty;
        public long FormKeyId { get; set; }
        public int PerkTreeIndex { get; set; }
        public string? AssociatedSkillModKeyName { get; set; }
        public int? AssociatedSkillModKeyType { get; set; }
        public string? AssociatedSkillModKeyFileName { get; set; }
        public long? AssociatedSkillFormKeyId { get; set; }
        public string? FNAM { get; set; }
        public double? HorizontalPosition { get; set; }
        public int? EntryIndex { get; set; }
        public int? PerkGridX { get; set; }
        public int? PerkGridY { get; set; }
        public double? VerticalPosition { get; set; }
        public string? PerkModKeyName { get; set; }
        public int? PerkModKeyType { get; set; }
        public string? PerkModKeyFileName { get; set; }
        public long? PerkFormKeyId { get; set; }
        public DateTime ImportedAtUTC { get; set; }
    }

    private sealed class ActorValueInformationConnectionLineIndexRow
    {
        public string ModKeyName { get; set; } = string.Empty;
        public int ModKeyType { get; set; }
        public string ModKeyFileName { get; set; } = string.Empty;
        public string FormKeyModKeyName { get; set; } = string.Empty;
        public int FormKeyModKeyType { get; set; }
        public string FormKeyModKeyFileName { get; set; } = string.Empty;
        public long FormKeyId { get; set; }
        public int PerkTreeIndex { get; set; }
        public int ConnectionLineIndex { get; set; }
        public int TargetIndex { get; set; }
        public DateTime ImportedAtUTC { get; set; }
    }

    private static ModKeyDTO CreateModKey(string name, int type, string fileName)
    {
        return new ModKeyDTO { Name = name, Type = type, FileName = fileName };
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
        return string.Equals(first.Name, second.Name, StringComparison.OrdinalIgnoreCase) &&
               first.Type == second.Type &&
               string.Equals(first.FileName, second.FileName, StringComparison.OrdinalIgnoreCase);
    }
}
