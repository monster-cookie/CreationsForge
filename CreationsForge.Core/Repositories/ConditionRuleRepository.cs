using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Records.Interfaces;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Repositories.Interfaces;
using NPoco;

namespace CreationsForge.Core.Repositories;

public class ConditionRuleRepository : IConditionRuleRepository
{
    public const string DefaultConditionSlot = "Conditions";

    private readonly IDatabase Database;

    public ConditionRuleRepository(IDatabase database)
    {
        Database = database;
    }

    public IReadOnlyList<ConditionFormConditionDTO> GetByFormKey(SupportedGame game, string recordType, FormKeyDTO formKey)
    {
        var conditionRules = Database.Fetch<ConditionRuleRow>(
                """
                SELECT *
                FROM ConditionRules
                WHERE Game = @Game
                  AND RecordType = @RecordType
                  AND FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
                  AND FormKey_ModKey_Type = @FormKeyModKeyType
                  AND FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
                  AND FormKey_ID = @FormKeyId
                ORDER BY ModKey_FileName COLLATE NOCASE, ConditionSlot COLLATE NOCASE, Condition_Index;
                """,
                new
                {
                    Game = game.ToString(),
                    RecordType = recordType,
                    FormKeyModKeyName = formKey.ModKey.Name,
                    FormKeyModKeyType = formKey.ModKey.Type,
                    FormKeyModKeyFileName = formKey.ModKey.FileName,
                    FormKeyId = formKey.Id
                })
            .Select(row => ToDTO(row, game))
            .ToList();
        var parameters = FetchParametersByFormKey(game, recordType, formKey);
        foreach (var conditionRule in conditionRules)
        {
            conditionRule.Parameters = parameters
                .Where(parameter => IsSameModKey(parameter.ModKey, conditionRule.ModKey) &&
                    string.Equals(parameter.ConditionSlot, conditionRule.ConditionSlot, StringComparison.Ordinal) &&
                    parameter.ConditionIndex == conditionRule.ConditionIndex)
                .OrderBy(parameter => parameter.ParameterName, StringComparer.Ordinal)
                .ToList();
        }

        return conditionRules;
    }

    public void ReplaceConditionRules(IHasConditionsDTO record, string recordType)
    {
        if (record is not RecordDTO recordDTO)
        {
            throw new ArgumentException($"Expected {nameof(RecordDTO)}.", nameof(record));
        }

        Database.Execute(
            """
            DELETE FROM ConditionRules
            WHERE Game = @Game
              AND ModKey_Name = @ModKeyName
              AND ModKey_Type = @ModKeyType
              AND ModKey_FileName = @ModKeyFileName
              AND RecordType = @RecordType
              AND FormKey_ModKey_Name = @FormKeyModKeyName
              AND FormKey_ModKey_Type = @FormKeyModKeyType
              AND FormKey_ModKey_FileName = @FormKeyModKeyFileName
              AND FormKey_ID = @FormKeyId;
            """,
            new
            {
                Game = recordDTO.Game.ToString(),
                ModKeyName = recordDTO.ModKey.Name,
                ModKeyType = recordDTO.ModKey.Type,
                ModKeyFileName = recordDTO.ModKey.FileName,
                RecordType = recordType,
                FormKeyModKeyName = recordDTO.FormKey.ModKey.Name,
                FormKeyModKeyType = recordDTO.FormKey.ModKey.Type,
                FormKeyModKeyFileName = recordDTO.FormKey.ModKey.FileName,
                FormKeyId = recordDTO.FormKey.Id
            });

        foreach (var conditionRule in record.Conditions)
        {
            conditionRule.ImportedAtUTC = recordDTO.ImportedAtUTC;
            var conditionSlot = string.IsNullOrWhiteSpace(conditionRule.ConditionSlot) ? DefaultConditionSlot : conditionRule.ConditionSlot;
            Database.Execute(
                """
                INSERT OR REPLACE INTO ConditionRules (
                    Game, ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName,
                    FormKey_ID, ConditionSlot, Condition_Index, MutagenObjectType, DataMutagenObjectType, CompareOperator, ComparisonValue,
                    ComparisonValue_ModKey_Name, ComparisonValue_ModKey_Type, ComparisonValue_ModKey_FileName, ComparisonValue_FormKey_ID, ImportedAtUTC)
                VALUES (
                    @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @RecordType, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName,
                    @FormKeyId, @ConditionSlot, @ConditionIndex, @MutagenObjectType, @DataMutagenObjectType, @CompareOperator, @ComparisonValue,
                    @ComparisonValueModKeyName, @ComparisonValueModKeyType, @ComparisonValueModKeyFileName, @ComparisonValueFormKeyId, @ImportedAtUTC);
                """,
                new
                {
                    Game = conditionRule.Game.ToString(),
                    ModKeyName = conditionRule.ModKey.Name,
                    ModKeyType = conditionRule.ModKey.Type,
                    ModKeyFileName = conditionRule.ModKey.FileName,
                    RecordType = recordType,
                    FormKeyModKeyName = conditionRule.FormKey.ModKey.Name,
                    FormKeyModKeyType = conditionRule.FormKey.ModKey.Type,
                    FormKeyModKeyFileName = conditionRule.FormKey.ModKey.FileName,
                    FormKeyId = conditionRule.FormKey.Id,
                    ConditionSlot = conditionSlot,
                    conditionRule.ConditionIndex,
                    conditionRule.MutagenObjectType,
                    conditionRule.DataMutagenObjectType,
                    conditionRule.CompareOperator,
                    conditionRule.ComparisonValue,
                    ComparisonValueModKeyName = conditionRule.ComparisonValueFormKey?.ModKey.Name,
                    ComparisonValueModKeyType = conditionRule.ComparisonValueFormKey?.ModKey.Type,
                    ComparisonValueModKeyFileName = conditionRule.ComparisonValueFormKey?.ModKey.FileName,
                    ComparisonValueFormKeyId = conditionRule.ComparisonValueFormKey?.Id,
                    conditionRule.ImportedAtUTC
                });

            foreach (var parameter in conditionRule.Parameters)
            {
                parameter.ImportedAtUTC = recordDTO.ImportedAtUTC;
                Database.Execute(
                    """
                    INSERT OR REPLACE INTO ConditionRuleParameters (
                        Game, ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName,
                        FormKey_ID, ConditionSlot, Condition_Index, Parameter_Name, ParameterValue, Parameter_ModKey_Name, Parameter_ModKey_Type,
                        Parameter_ModKey_FileName, Parameter_FormKey_ID, ImportedAtUTC)
                    VALUES (
                        @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @RecordType, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName,
                        @FormKeyId, @ConditionSlot, @ConditionIndex, @ParameterName, @ParameterValue, @ParameterModKeyName, @ParameterModKeyType,
                        @ParameterModKeyFileName, @ParameterFormKeyId, @ImportedAtUTC);
                    """,
                    new
                    {
                        Game = parameter.Game.ToString(),
                        ModKeyName = parameter.ModKey.Name,
                        ModKeyType = parameter.ModKey.Type,
                        ModKeyFileName = parameter.ModKey.FileName,
                        RecordType = recordType,
                        FormKeyModKeyName = parameter.FormKey.ModKey.Name,
                        FormKeyModKeyType = parameter.FormKey.ModKey.Type,
                        FormKeyModKeyFileName = parameter.FormKey.ModKey.FileName,
                        FormKeyId = parameter.FormKey.Id,
                        ConditionSlot = string.IsNullOrWhiteSpace(parameter.ConditionSlot) ? conditionSlot : parameter.ConditionSlot,
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

    private IReadOnlyList<ConditionFormConditionParameterDTO> FetchParametersByFormKey(SupportedGame game, string recordType, FormKeyDTO formKey)
    {
        return Database.Fetch<ConditionRuleParameterRow>(
                """
                SELECT *
                FROM ConditionRuleParameters
                WHERE Game = @Game
                  AND RecordType = @RecordType
                  AND FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
                  AND FormKey_ModKey_Type = @FormKeyModKeyType
                  AND FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
                  AND FormKey_ID = @FormKeyId
                ORDER BY ModKey_FileName COLLATE NOCASE, ConditionSlot COLLATE NOCASE, Condition_Index, Parameter_Name COLLATE NOCASE;
                """,
                new
                {
                    Game = game.ToString(),
                    RecordType = recordType,
                    FormKeyModKeyName = formKey.ModKey.Name,
                    FormKeyModKeyType = formKey.ModKey.Type,
                    FormKeyModKeyFileName = formKey.ModKey.FileName,
                    FormKeyId = formKey.Id
                })
            .Select(row => ToDTO(row, game))
            .ToList();
    }

    private static ConditionFormConditionDTO ToDTO(ConditionRuleRow row, SupportedGame game)
    {
        return new ConditionFormConditionDTO
        {
            Game = game,
            ModKey = CreateModKey(row.ModKeyName, row.ModKeyType, row.ModKeyFileName),
            FormKey = CreateFormKey(row.FormKeyModKeyName, row.FormKeyModKeyType, row.FormKeyModKeyFileName, row.FormKeyId),
            ConditionSlot = row.ConditionSlot,
            ConditionIndex = row.ConditionIndex,
            MutagenObjectType = row.MutagenObjectType,
            DataMutagenObjectType = row.DataMutagenObjectType,
            CompareOperator = row.CompareOperator,
            ComparisonValue = row.ComparisonValue,
            ComparisonValueFormKey = CreateNullableFormKey(row.ComparisonValueModKeyName, row.ComparisonValueModKeyType, row.ComparisonValueModKeyFileName, row.ComparisonValueFormKeyId),
            ImportedAtUTC = row.ImportedAtUTC
        };
    }

    private static ConditionFormConditionParameterDTO ToDTO(ConditionRuleParameterRow row, SupportedGame game)
    {
        return new ConditionFormConditionParameterDTO
        {
            Game = game,
            ModKey = CreateModKey(row.ModKeyName, row.ModKeyType, row.ModKeyFileName),
            FormKey = CreateFormKey(row.FormKeyModKeyName, row.FormKeyModKeyType, row.FormKeyModKeyFileName, row.FormKeyId),
            ConditionSlot = row.ConditionSlot,
            ConditionIndex = row.ConditionIndex,
            ParameterName = row.ParameterName,
            ParameterValue = row.ParameterValue,
            ParameterFormKey = CreateNullableFormKey(row.ParameterModKeyName, row.ParameterModKeyType, row.ParameterModKeyFileName, row.ParameterFormKeyId),
            ImportedAtUTC = row.ImportedAtUTC
        };
    }

    private static ModKeyDTO CreateModKey(string name, int type, string fileName)
    {
        return new ModKeyDTO { Name = name, Type = type, FileName = fileName };
    }

    private static FormKeyDTO CreateFormKey(string modKeyName, int modKeyType, string modKeyFileName, long formKeyId)
    {
        return new FormKeyDTO { ModKey = CreateModKey(modKeyName, modKeyType, modKeyFileName), Id = (uint)formKeyId };
    }

    private static FormKeyDTO? CreateNullableFormKey(string? modKeyName, int? modKeyType, string? modKeyFileName, long? formKeyId)
    {
        return modKeyName is null || modKeyType is null || modKeyFileName is null || formKeyId is null
            ? null
            : CreateFormKey(modKeyName, modKeyType.Value, modKeyFileName, formKeyId.Value);
    }

    private static bool IsSameModKey(ModKeyDTO first, ModKeyDTO second)
    {
        return first.Type == second.Type &&
            string.Equals(first.Name, second.Name, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(first.FileName, second.FileName, StringComparison.OrdinalIgnoreCase);
    }

    private sealed class ConditionRuleRow
    {
        public string ModKeyName { get; set; } = string.Empty;
        public int ModKeyType { get; set; }
        public string ModKeyFileName { get; set; } = string.Empty;
        public string FormKeyModKeyName { get; set; } = string.Empty;
        public int FormKeyModKeyType { get; set; }
        public string FormKeyModKeyFileName { get; set; } = string.Empty;
        public long FormKeyId { get; set; }
        public string ConditionSlot { get; set; } = string.Empty;
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

    private sealed class ConditionRuleParameterRow
    {
        public string ModKeyName { get; set; } = string.Empty;
        public int ModKeyType { get; set; }
        public string ModKeyFileName { get; set; } = string.Empty;
        public string FormKeyModKeyName { get; set; } = string.Empty;
        public int FormKeyModKeyType { get; set; }
        public string FormKeyModKeyFileName { get; set; } = string.Empty;
        public long FormKeyId { get; set; }
        public string ConditionSlot { get; set; } = string.Empty;
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
