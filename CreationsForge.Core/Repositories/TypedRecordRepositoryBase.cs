using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Repositories.Interfaces;
using NPoco;

namespace CreationsForge.Core.Repositories;

public abstract class TypedRecordRepositoryBase : IRecordTreeRepository
{
    private static readonly IReadOnlySet<string> AllowedTableNames = new HashSet<string>(StringComparer.Ordinal)
    {
        RecordTypeCatalog.ActorValueInformation.TableName,
        RecordTypeCatalog.Keyword.TableName,
        RecordTypeCatalog.MagicEffect.TableName,
        RecordTypeCatalog.MiscObject.TableName,
        RecordTypeCatalog.NPC.TableName,
        RecordTypeCatalog.Perk.TableName,
        RecordTypeCatalog.Book.TableName,
        RecordTypeCatalog.Door.TableName,
        RecordTypeCatalog.Static.TableName,
        RecordTypeCatalog.Container.TableName,
        RecordTypeCatalog.ConstructibleObject.TableName,
        RecordTypeCatalog.ConditionForm.TableName,
        RecordTypeCatalog.Terminal.TableName
    };

    protected readonly IDatabase Database;
    private readonly IRecordInstanceRepository RecordInstanceRepository;

    protected TypedRecordRepositoryBase(IDatabase database, IRecordInstanceRepository recordInstanceRepository)
    {
        Database = database;
        RecordInstanceRepository = recordInstanceRepository;
    }

    public abstract string RecordType { get; }

    protected abstract string TableName { get; }

    public IReadOnlyList<RecordTreeEntryDTO> GetRecordTreeEntriesByPlugin(SupportedGame game, ModKeyDTO modKey)
    {
        var tableName = GetValidatedTableName();
        return Database.Fetch<RecordTreeEntryRow>(
                $"""
                SELECT
                    CurrentRecord.ModKey_Name AS ModKeyName,
                    CurrentRecord.ModKey_Type AS ModKeyType,
                    CurrentRecord.ModKey_FileName AS ModKeyFileName,
                    CurrentRecord.FormKey_ModKey_Name AS FormKeyModKeyName,
                    CurrentRecord.FormKey_ModKey_Type AS FormKeyModKeyType,
                    CurrentRecord.FormKey_ModKey_FileName AS FormKeyModKeyFileName,
                    CurrentRecord.FormKey_ID AS FormKeyId,
                    CurrentRecord.EditorID AS EditorId,
                    COALESCE(PeerCounts.PluginCount, 0) AS PluginCount
                FROM {tableName} CurrentRecord
                LEFT JOIN (
                    SELECT
                        PeerRecord.FormKey_ModKey_Name,
                        PeerRecord.FormKey_ModKey_Type,
                        PeerRecord.FormKey_ModKey_FileName,
                        PeerRecord.FormKey_ID,
                        COUNT(*) AS PluginCount
                    FROM {tableName} PeerRecord
                    INNER JOIN {tableName} ActiveRecord
                       ON ActiveRecord.Game = @Game
                      AND ActiveRecord.ModKey_Name = @ModKeyName COLLATE NOCASE
                      AND ActiveRecord.ModKey_Type = @ModKeyType
                      AND ActiveRecord.ModKey_FileName = @ModKeyFileName COLLATE NOCASE
                      AND ActiveRecord.FormKey_ModKey_Name = PeerRecord.FormKey_ModKey_Name COLLATE NOCASE
                      AND ActiveRecord.FormKey_ModKey_Type = PeerRecord.FormKey_ModKey_Type
                      AND ActiveRecord.FormKey_ModKey_FileName = PeerRecord.FormKey_ModKey_FileName COLLATE NOCASE
                      AND ActiveRecord.FormKey_ID = PeerRecord.FormKey_ID
                    WHERE PeerRecord.Game = @Game
                    GROUP BY PeerRecord.FormKey_ModKey_Name, PeerRecord.FormKey_ModKey_Type, PeerRecord.FormKey_ModKey_FileName, PeerRecord.FormKey_ID
                ) PeerCounts
                  ON PeerCounts.FormKey_ModKey_Name = CurrentRecord.FormKey_ModKey_Name COLLATE NOCASE
                 AND PeerCounts.FormKey_ModKey_Type = CurrentRecord.FormKey_ModKey_Type
                 AND PeerCounts.FormKey_ModKey_FileName = CurrentRecord.FormKey_ModKey_FileName COLLATE NOCASE
                 AND PeerCounts.FormKey_ID = CurrentRecord.FormKey_ID
                WHERE CurrentRecord.Game = @Game
                  AND CurrentRecord.ModKey_Name = @ModKeyName COLLATE NOCASE
                  AND CurrentRecord.ModKey_Type = @ModKeyType
                  AND CurrentRecord.ModKey_FileName = @ModKeyFileName COLLATE NOCASE
                ORDER BY CurrentRecord.EditorID COLLATE NOCASE, CurrentRecord.FormKey_ID;
                """,
                new
                {
                    Game = game.ToString(),
                    ModKeyName = modKey.Name,
                    ModKeyType = modKey.Type,
                    ModKeyFileName = modKey.FileName
                })
            .Select(record => ToRecordTreeEntry(record, game))
            .ToList();
    }

    public IReadOnlyDictionary<string, int> GetRecordPluginCountsByGame(SupportedGame game)
    {
        var tableName = GetValidatedTableName();
        return Database.Fetch<RecordPluginCountRow>(
                $"""
                SELECT
                    FormKey_ModKey_Name AS FormKeyModKeyName,
                    FormKey_ModKey_Type AS FormKeyModKeyType,
                    FormKey_ModKey_FileName AS FormKeyModKeyFileName,
                    FormKey_ID AS FormKeyId,
                    COUNT(*) AS PluginCount
                FROM {tableName}
                WHERE Game = @Game
                GROUP BY FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID;
                """,
                new
                {
                    Game = game.ToString()
                })
            .ToDictionary(row => row.GetFormKeyKey(), row => row.PluginCount);
    }

    public void DeleteStaleByPlugin(SupportedGame game, ModKeyDTO modKey, DateTime importedAtUTC)
    {
        var tableName = GetValidatedTableName();
        Database.Execute(
            $"""
            DELETE FROM {tableName}
            WHERE Game = @Game
              AND ModKey_Name = @ModKeyName
              AND ModKey_Type = @ModKeyType
              AND ModKey_FileName = @ModKeyFileName
              AND ImportedAtUTC <> @ImportedAtUTC;
            """,
            new
            {
                Game = game.ToString(),
                ModKeyName = modKey.Name,
                ModKeyType = modKey.Type,
                ModKeyFileName = modKey.FileName,
                ImportedAtUTC = importedAtUTC
            });
        RecordInstanceRepository.DeleteStaleByPlugin(game, modKey, RecordType, importedAtUTC);
    }

    protected void SaveRecordInstance(RecordDTO dto)
    {
        RecordInstanceRepository.Save(new RecordInstanceDTO
        {
            Game = dto.Game,
            ModKey = dto.ModKey,
            RecordType = RecordType,
            FormKey = dto.FormKey,
            EditorID = dto.EditorID,
            FormVersion = dto.FormVersion,
            MajorRecordFlags = dto.MajorRecordFlags,
            ImportedAtUTC = dto.ImportedAtUTC
        });
    }

    protected IReadOnlyList<TRow> FetchByFormKey<TRow>(SupportedGame game, FormKeyDTO formKey, IReadOnlyList<SelectColumnDefinition> selectColumns)
    {
        var tableName = GetValidatedTableName();
        var selectColumnSql = BuildSelectColumnSql(selectColumns);
        return Database.Fetch<TRow>(
            $"""
            SELECT
                CurrentRecord.Game,
                CurrentRecord.ModKey_Name AS ModKeyName,
                CurrentRecord.ModKey_Type AS ModKeyType,
                CurrentRecord.ModKey_FileName AS ModKeyFileName,
                CurrentRecord.FormKey_ModKey_Name AS FormKeyModKeyName,
                CurrentRecord.FormKey_ModKey_Type AS FormKeyModKeyType,
                CurrentRecord.FormKey_ModKey_FileName AS FormKeyModKeyFileName,
                CurrentRecord.FormKey_ID AS FormKeyId,
                CurrentRecord.EditorID AS EditorId,
                CurrentRecord.FormVersion,
                CurrentRecord.MajorRecordFlags,
                CurrentRecord.ImportedAtUTC
                {selectColumnSql}
            FROM {tableName} CurrentRecord
            WHERE CurrentRecord.Game = @Game
              AND CurrentRecord.FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
              AND CurrentRecord.FormKey_ModKey_Type = @FormKeyModKeyType
              AND CurrentRecord.FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
              AND CurrentRecord.FormKey_ID = @FormKeyId
            ORDER BY CurrentRecord.ImportedAtUTC, CurrentRecord.ModKey_FileName COLLATE NOCASE;
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

    protected static SelectColumnDefinition SelectColumn(string columnName)
    {
        return new SelectColumnDefinition(columnName, null);
    }

    protected static SelectColumnDefinition SelectColumn(string columnName, string alias)
    {
        return new SelectColumnDefinition(columnName, alias);
    }

    protected string GetValidatedTableName()
    {
        if (!AllowedTableNames.Contains(TableName))
        {
            throw new InvalidOperationException($"Typed record table name '{TableName}' is not allowed.");
        }

        return TableName;
    }

    private static string BuildSelectColumnSql(IReadOnlyList<SelectColumnDefinition> selectColumns)
    {
        if (selectColumns.Count == 0)
        {
            return string.Empty;
        }

        return string.Join(
            Environment.NewLine,
            selectColumns.Select(column => $"                {column.ToSqlFragment()}"));
    }

    private static void ValidateIdentifier(string identifier, string description)
    {
        if (string.IsNullOrWhiteSpace(identifier))
        {
            throw new ArgumentException($"{description} cannot be empty.", nameof(identifier));
        }

        if (!IsIdentifierStart(identifier[0]))
        {
            throw new ArgumentException($"{description} '{identifier}' is not a valid SQL identifier.", nameof(identifier));
        }

        if (identifier.Any(character => !IsIdentifierPart(character)))
        {
            throw new ArgumentException($"{description} '{identifier}' is not a valid SQL identifier.", nameof(identifier));
        }
    }

    private static bool IsIdentifierStart(char character)
    {
        return char.IsAsciiLetter(character) || character == '_';
    }

    private static bool IsIdentifierPart(char character)
    {
        return char.IsAsciiLetterOrDigit(character) || character == '_';
    }

    protected static void ApplyCommonFields(RecordDTO dto, RecordRow row, SupportedGame game)
    {
        dto.Game = game;
        dto.ModKey = new ModKeyDTO
        {
            Name = row.ModKeyName,
            Type = row.ModKeyType,
            FileName = row.ModKeyFileName
        };
        dto.FormKey = new FormKeyDTO
        {
            ModKey = new ModKeyDTO
            {
                Name = row.FormKeyModKeyName,
                Type = row.FormKeyModKeyType,
                FileName = row.FormKeyModKeyFileName
            },
            Id = (uint)row.FormKeyId
        };
        dto.EditorID = row.EditorId;
        dto.FormVersion = row.FormVersion;
        dto.MajorRecordFlags = row.MajorRecordFlags;
        dto.ImportedAtUTC = row.ImportedAtUTC;
    }

    protected static object CommonParameters(RecordDTO dto)
    {
        return new
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
            dto.ImportedAtUTC
        };
    }

    protected static FormKeyDTO? CreateNullableFormKey(string? modKeyName, int? modKeyType, string? modKeyFileName, long? formKeyId)
    {
        if (modKeyName is null || modKeyType is null || modKeyFileName is null || formKeyId is null)
        {
            return null;
        }

        return new FormKeyDTO
        {
            ModKey = new ModKeyDTO
            {
                Name = modKeyName,
                Type = modKeyType.Value,
                FileName = modKeyFileName
            },
            Id = (uint)formKeyId.Value
        };
    }

    private RecordTreeEntryDTO ToRecordTreeEntry(RecordTreeEntryRow record, SupportedGame game)
    {
        return new RecordTreeEntryDTO
        {
            Game = game,
            ModKey = new ModKeyDTO
            {
                Name = record.ModKeyName,
                Type = record.ModKeyType,
                FileName = record.ModKeyFileName
            },
            FormKey = new FormKeyDTO
            {
                ModKey = new ModKeyDTO
                {
                    Name = record.FormKeyModKeyName,
                    Type = record.FormKeyModKeyType,
                    FileName = record.FormKeyModKeyFileName
                },
                Id = (uint)record.FormKeyId
            },
            EditorID = record.EditorId,
            RecordType = RecordType,
            PluginCount = record.PluginCount
        };
    }

    protected class RecordRow
    {
        public string Game { get; set; } = string.Empty;

        public string ModKeyName { get; set; } = string.Empty;

        public int ModKeyType { get; set; }

        public string ModKeyFileName { get; set; } = string.Empty;

        public string FormKeyModKeyName { get; set; } = string.Empty;

        public int FormKeyModKeyType { get; set; }

        public string FormKeyModKeyFileName { get; set; } = string.Empty;

        public long FormKeyId { get; set; }

        public string EditorId { get; set; } = string.Empty;

        public int FormVersion { get; set; }

        public int MajorRecordFlags { get; set; }

        public DateTime ImportedAtUTC { get; set; }
    }

    protected readonly struct SelectColumnDefinition
    {
        private readonly string ColumnName;
        private readonly string? Alias;

        public SelectColumnDefinition(string columnName, string? alias)
        {
            ValidateIdentifier(columnName, "Column name");
            if (alias is not null)
            {
                ValidateIdentifier(alias, "Column alias");
            }

            ColumnName = columnName;
            Alias = alias;
        }

        public string ToSqlFragment()
        {
            if (Alias is null)
            {
                return $", CurrentRecord.{ColumnName}";
            }

            return $", CurrentRecord.{ColumnName} AS {Alias}";
        }
    }

    private sealed class RecordTreeEntryRow
    {
        public string ModKeyName { get; set; } = string.Empty;

        public int ModKeyType { get; set; }

        public string ModKeyFileName { get; set; } = string.Empty;

        public string FormKeyModKeyName { get; set; } = string.Empty;

        public int FormKeyModKeyType { get; set; }

        public string FormKeyModKeyFileName { get; set; } = string.Empty;

        public long FormKeyId { get; set; }

        public string EditorId { get; set; } = string.Empty;

        public int PluginCount { get; set; }
    }

    private sealed class RecordPluginCountRow
    {
        public string FormKeyModKeyName { get; set; } = string.Empty;

        public int FormKeyModKeyType { get; set; }

        public string FormKeyModKeyFileName { get; set; } = string.Empty;

        public long FormKeyId { get; set; }

        public int PluginCount { get; set; }

        public string GetFormKeyKey()
        {
            return $"{FormKeyModKeyName}|{FormKeyModKeyType}|{FormKeyModKeyFileName}|{FormKeyId}";
        }
    }
}
