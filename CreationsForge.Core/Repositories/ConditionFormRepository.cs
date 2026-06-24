using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Repositories.Interfaces;
using NPoco;

namespace CreationsForge.Core.Repositories;

public class ConditionFormRepository : TypedRecordRepositoryBase, IConditionFormRepository
{
    private readonly IConditionRuleRepository ConditionRuleRepository;

    public ConditionFormRepository(
        IDatabase database,
        IRecordInstanceRepository recordInstanceRepository,
        IConditionRuleRepository conditionRuleRepository)
        : base(database, recordInstanceRepository)
    {
        ConditionRuleRepository = conditionRuleRepository;
    }

    public override string RecordType => RecordTypeCatalog.ConditionForm.RecordID;

    protected override string TableName => RecordTypeCatalog.ConditionForm.TableName;

    public IReadOnlyList<ConditionFormDTO> GetByFormKey(SupportedGame game, FormKeyDTO formKey)
    {
        var records = FetchByFormKey<ConditionFormRow>(
                game,
                formKey,
                [
                    SelectColumn("Version2"),
                    SelectColumn("VersionControl")
                ])
            .Select(record => ToDTO(record, game))
            .ToList();
        var conditions = ConditionRuleRepository.GetByFormKey(game, RecordTypeCatalog.ConditionForm.RecordID, formKey);
        foreach (var record in records)
        {
            record.Conditions = conditions
                .Where(condition => IsSameModKey(condition.ModKey, record.ModKey) && string.Equals(condition.ConditionSlot, "Conditions", StringComparison.Ordinal))
                .OrderBy(condition => condition.ConditionIndex)
                .ToList();
        }

        return records;
    }

    public void Save(ConditionFormDTO dto)
    {
        SaveRecordInstance(dto);
        Database.Execute(
            """
            INSERT OR REPLACE INTO ConditionForms (
                Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                EditorID, FormVersion, MajorRecordFlags, ImportedAtUTC, Version2, VersionControl)
            VALUES (
                @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                @EditorId, @FormVersion, @MajorRecordFlags, @ImportedAtUTC, @Version2, @VersionControl);
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
                dto.VersionControl
            });
    }

    private static ConditionFormDTO ToDTO(ConditionFormRow record, SupportedGame game)
    {
        var dto = new ConditionFormDTO
        {
            Game = game,
            ModKey = new ModKeyDTO { Name = string.Empty, Type = 0, FileName = string.Empty },
            FormKey = new FormKeyDTO { ModKey = new ModKeyDTO { Name = string.Empty, Type = 0, FileName = string.Empty }, Id = 0 },
            EditorID = string.Empty,
            FormVersion = 0,
            MajorRecordFlags = 0,
            ImportedAtUTC = record.ImportedAtUTC,
            Version2 = record.Version2,
            VersionControl = record.VersionControl
        };
        ApplyCommonFields(dto, record, game);
        return dto;
    }

    private static bool IsSameModKey(ModKeyDTO first, ModKeyDTO second)
    {
        return first.Type == second.Type &&
            string.Equals(first.Name, second.Name, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(first.FileName, second.FileName, StringComparison.OrdinalIgnoreCase);
    }

    private sealed class ConditionFormRow : RecordRow
    {
        public int? Version2 { get; set; }

        public int? VersionControl { get; set; }
    }

}
