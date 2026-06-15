using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Repositories.Interfaces;
using NPoco;

namespace CreationsForge.Core.Repositories;

public class ConditionFormRepository : TypedRecordRepositoryBase, IConditionFormRepository
{
    public ConditionFormRepository(IDatabase database, IRecordInstanceRepository recordInstanceRepository)
        : base(database, recordInstanceRepository)
    { }

    public override string RecordType => RecordTypeCatalog.ConditionForm.RecordID;

    protected override string TableName => RecordTypeCatalog.ConditionForm.TableName;

    public IReadOnlyList<ConditionFormDTO> GetByFormKey(SupportedGame game, FormKeyDTO formKey)
    {
        var records = FetchByFormKey<ConditionFormRow>(
                game,
                formKey,
                [
                    SelectColumn("Version2")
                ])
            .Select(record => ToDTO(record, game))
            .ToList();
        var conditions = FetchConditionsByFormKey(game, formKey);
        foreach (var record in records)
        {
            record.Conditions = conditions
                .Where(condition => IsSameModKey(condition.ModKey, record.ModKey))
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
                EditorID, FormVersion, MajorRecordFlags, ImportedAtUTC, Version2)
            VALUES (
                @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                @EditorId, @FormVersion, @MajorRecordFlags, @ImportedAtUTC, @Version2);
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
                dto.Version2
            });
        ReplaceConditions(dto);
    }

    private IReadOnlyList<ConditionFormConditionDTO> FetchConditionsByFormKey(SupportedGame game, FormKeyDTO formKey)
    {
        var conditions = Database.Fetch<ConditionFormConditionRow>(
                """
                SELECT *
                FROM ConditionFormConditions
                WHERE Game = @Game
                  AND FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
                  AND FormKey_ModKey_Type = @FormKeyModKeyType
                  AND FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
                  AND FormKey_ID = @FormKeyId
                ORDER BY ModKey_FileName COLLATE NOCASE, Condition_Index;
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
        var parameters = FetchConditionParametersByFormKey(game, formKey);
        foreach (var condition in conditions)
        {
            condition.Parameters = parameters
                .Where(parameter => IsSameModKey(parameter.ModKey, condition.ModKey) && parameter.ConditionIndex == condition.ConditionIndex)
                .OrderBy(parameter => parameter.ParameterName, StringComparer.Ordinal)
                .ToList();
        }

        return conditions;
    }

    private IReadOnlyList<ConditionFormConditionParameterDTO> FetchConditionParametersByFormKey(SupportedGame game, FormKeyDTO formKey)
    {
        return Database.Fetch<ConditionFormConditionParameterRow>(
                """
                SELECT *
                FROM ConditionFormConditionParameters
                WHERE Game = @Game
                  AND FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
                  AND FormKey_ModKey_Type = @FormKeyModKeyType
                  AND FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
                  AND FormKey_ID = @FormKeyId
                ORDER BY ModKey_FileName COLLATE NOCASE, Condition_Index, Parameter_Name COLLATE NOCASE;
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

    private void ReplaceConditions(ConditionFormDTO dto)
    {
        Database.Execute(
            """
            DELETE FROM ConditionFormConditions
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

        foreach (var condition in dto.Conditions)
        {
            condition.ImportedAtUTC = dto.ImportedAtUTC;
            Database.Execute(
                """
                INSERT OR REPLACE INTO ConditionFormConditions (
                    Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                    Condition_Index, MutagenObjectType, DataMutagenObjectType, CompareOperator, ComparisonValue, ComparisonValue_ModKey_Name,
                    ComparisonValue_ModKey_Type, ComparisonValue_ModKey_FileName, ComparisonValue_FormKey_ID, ImportedAtUTC)
                VALUES (
                    @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                    @ConditionIndex, @MutagenObjectType, @DataMutagenObjectType, @CompareOperator, @ComparisonValue, @ComparisonValueModKeyName,
                    @ComparisonValueModKeyType, @ComparisonValueModKeyFileName, @ComparisonValueFormKeyId, @ImportedAtUTC);
                """,
                new
                {
                    Game = condition.Game.ToString(),
                    ModKeyName = condition.ModKey.Name,
                    ModKeyType = condition.ModKey.Type,
                    ModKeyFileName = condition.ModKey.FileName,
                    FormKeyModKeyName = condition.FormKey.ModKey.Name,
                    FormKeyModKeyType = condition.FormKey.ModKey.Type,
                    FormKeyModKeyFileName = condition.FormKey.ModKey.FileName,
                    FormKeyId = condition.FormKey.Id,
                    condition.ConditionIndex,
                    condition.MutagenObjectType,
                    condition.DataMutagenObjectType,
                    condition.CompareOperator,
                    condition.ComparisonValue,
                    ComparisonValueModKeyName = condition.ComparisonValueFormKey?.ModKey.Name,
                    ComparisonValueModKeyType = condition.ComparisonValueFormKey?.ModKey.Type,
                    ComparisonValueModKeyFileName = condition.ComparisonValueFormKey?.ModKey.FileName,
                    ComparisonValueFormKeyId = condition.ComparisonValueFormKey?.Id,
                    condition.ImportedAtUTC
                });

            foreach (var parameter in condition.Parameters)
            {
                parameter.ImportedAtUTC = dto.ImportedAtUTC;
                Database.Execute(
                    """
                    INSERT OR REPLACE INTO ConditionFormConditionParameters (
                        Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                        Condition_Index, Parameter_Name, ParameterValue, Parameter_ModKey_Name, Parameter_ModKey_Type, Parameter_ModKey_FileName,
                        Parameter_FormKey_ID, ImportedAtUTC)
                    VALUES (
                        @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                        @ConditionIndex, @ParameterName, @ParameterValue, @ParameterModKeyName, @ParameterModKeyType, @ParameterModKeyFileName,
                        @ParameterFormKeyId, @ImportedAtUTC);
                    """,
                    new
                    {
                        Game = parameter.Game.ToString(),
                        ModKeyName = parameter.ModKey.Name,
                        ModKeyType = parameter.ModKey.Type,
                        ModKeyFileName = parameter.ModKey.FileName,
                        FormKeyModKeyName = parameter.FormKey.ModKey.Name,
                        FormKeyModKeyType = parameter.FormKey.ModKey.Type,
                        FormKeyModKeyFileName = parameter.FormKey.ModKey.FileName,
                        FormKeyId = parameter.FormKey.Id,
                        parameter.ConditionIndex,
                        parameter.ParameterName,
                        parameter.ParameterValue,
                        ParameterModKeyName = parameter.ParameterFormKey?.ModKey.Name,
                        ParameterModKeyType = parameter.ParameterFormKey?.ModKey.Type,
                        ParameterModKeyFileName = parameter.ParameterFormKey?.ModKey.FileName,
                        ParameterFormKeyId = parameter.ParameterFormKey?.Id,
                        parameter.ImportedAtUTC
                    });
            }
        }
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
            Version2 = record.Version2
        };
        ApplyCommonFields(dto, record, game);
        return dto;
    }

    private static ConditionFormConditionDTO ToDTO(ConditionFormConditionRow row, SupportedGame game)
    {
        return new ConditionFormConditionDTO
        {
            Game = game,
            ModKey = CreateModKey(row.ModKeyName, row.ModKeyType, row.ModKeyFileName),
            FormKey = CreateFormKey(row.FormKeyModKeyName, row.FormKeyModKeyType, row.FormKeyModKeyFileName, row.FormKeyId),
            ConditionIndex = row.ConditionIndex,
            MutagenObjectType = row.MutagenObjectType,
            DataMutagenObjectType = row.DataMutagenObjectType,
            CompareOperator = row.CompareOperator,
            ComparisonValue = row.ComparisonValue,
            ComparisonValueFormKey = CreateNullableFormKey(row.ComparisonValueModKeyName, row.ComparisonValueModKeyType, row.ComparisonValueModKeyFileName, row.ComparisonValueFormKeyId),
            ImportedAtUTC = row.ImportedAtUTC
        };
    }

    private static ConditionFormConditionParameterDTO ToDTO(ConditionFormConditionParameterRow row, SupportedGame game)
    {
        return new ConditionFormConditionParameterDTO
        {
            Game = game,
            ModKey = CreateModKey(row.ModKeyName, row.ModKeyType, row.ModKeyFileName),
            FormKey = CreateFormKey(row.FormKeyModKeyName, row.FormKeyModKeyType, row.FormKeyModKeyFileName, row.FormKeyId),
            ConditionIndex = row.ConditionIndex,
            ParameterName = row.ParameterName,
            ParameterValue = row.ParameterValue,
            ParameterFormKey = CreateNullableFormKey(row.ParameterModKeyName, row.ParameterModKeyType, row.ParameterModKeyFileName, row.ParameterFormKeyId),
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

    private sealed class ConditionFormRow : RecordRow
    {
        public int? Version2 { get; set; }
    }

    private sealed class ConditionFormConditionRow
    {
        public string ModKeyName { get; set; } = string.Empty;

        public int ModKeyType { get; set; }

        public string ModKeyFileName { get; set; } = string.Empty;

        public string FormKeyModKeyName { get; set; } = string.Empty;

        public int FormKeyModKeyType { get; set; }

        public string FormKeyModKeyFileName { get; set; } = string.Empty;

        public long FormKeyId { get; set; }

        public int ConditionIndex { get; set; }

        public string MutagenObjectType { get; set; } = string.Empty;

        public string? DataMutagenObjectType { get; set; }

        public string? CompareOperator { get; set; }

        public string? ComparisonValue { get; set; }

        public string? ComparisonValueModKeyName { get; set; }

        public int? ComparisonValueModKeyType { get; set; }

        public string? ComparisonValueModKeyFileName { get; set; }

        public long? ComparisonValueFormKeyId { get; set; }

        public DateTime ImportedAtUTC { get; set; }
    }

    private sealed class ConditionFormConditionParameterRow
    {
        public string ModKeyName { get; set; } = string.Empty;

        public int ModKeyType { get; set; }

        public string ModKeyFileName { get; set; } = string.Empty;

        public string FormKeyModKeyName { get; set; } = string.Empty;

        public int FormKeyModKeyType { get; set; }

        public string FormKeyModKeyFileName { get; set; } = string.Empty;

        public long FormKeyId { get; set; }

        public int ConditionIndex { get; set; }

        public string ParameterName { get; set; } = string.Empty;

        public string? ParameterValue { get; set; }

        public string? ParameterModKeyName { get; set; }

        public int? ParameterModKeyType { get; set; }

        public string? ParameterModKeyFileName { get; set; }

        public long? ParameterFormKeyId { get; set; }

        public DateTime ImportedAtUTC { get; set; }
    }
}
