using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Models.Database;
using CreationsForge.Core.Repositories.Interfaces;
using NPoco;

namespace CreationsForge.Core.Repositories;

public class FormListRepository : IFormListRepository, IRecordTreeRepository
{
    private readonly IDatabase Database;
    private readonly IRecordInstanceRepository RecordInstanceRepository;
    private readonly IRecordLocalizedStringRepository RecordLocalizedStringRepository;

    public FormListRepository(
        IDatabase database,
        IRecordInstanceRepository recordInstanceRepository,
        IRecordLocalizedStringRepository recordLocalizedStringRepository)
    {
        Database = database;
        RecordInstanceRepository = recordInstanceRepository;
        RecordLocalizedStringRepository = recordLocalizedStringRepository;
    }

    public string RecordType => RecordTypeCatalog.FormList.RecordID;

    public IReadOnlyList<RecordTreeEntryDTO> GetRecordTreeEntriesByPlugin(SupportedGame game, ModKeyDTO modKey)
    {
        return Database.Fetch<RecordTreeEntryRow>(
                """
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
                FROM FormLists CurrentRecord
                LEFT JOIN (
                    SELECT
                        PeerRecord.FormKey_ModKey_Name,
                        PeerRecord.FormKey_ModKey_Type,
                        PeerRecord.FormKey_ModKey_FileName,
                        PeerRecord.FormKey_ID,
                        COUNT(*) AS PluginCount
                    FROM FormLists PeerRecord
                    INNER JOIN FormLists ActiveRecord
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
        return Database.Fetch<RecordPluginCountRow>(
                """
                SELECT
                    FormKey_ModKey_Name AS FormKeyModKeyName,
                    FormKey_ModKey_Type AS FormKeyModKeyType,
                    FormKey_ModKey_FileName AS FormKeyModKeyFileName,
                    FormKey_ID AS FormKeyId,
                    COUNT(*) AS PluginCount
                FROM FormLists
                WHERE Game = @Game
                GROUP BY FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID;
                """,
                new
                {
                    Game = game.ToString()
                })
            .ToDictionary(row => row.GetFormKeyKey(), row => row.PluginCount);
    }

    public IReadOnlyList<FormListDTO> GetByFormKey(SupportedGame game, FormKeyDTO formKey)
    {
        var records = Database.Fetch<FormList>(
            """
            SELECT *
            FROM FormLists
            WHERE Game = @Game
              AND FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
              AND FormKey_ModKey_Type = @FormKeyModKeyType
              AND FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
              AND FormKey_ID = @FormKeyId
            ORDER BY ImportedAtUTC, ModKey_FileName COLLATE NOCASE;
            """,
            new
            {
                Game = game.ToString(),
                FormKeyModKeyName = formKey.ModKey.Name,
                FormKeyModKeyType = formKey.ModKey.Type,
                FormKeyModKeyFileName = formKey.ModKey.FileName,
                FormKeyId = formKey.Id
            });
        var items = Database.Fetch<FormListItem>(
            """
            SELECT *
            FROM FormListItems
            WHERE Game = @Game
              AND FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
              AND FormKey_ModKey_Type = @FormKeyModKeyType
              AND FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
              AND FormKey_ID = @FormKeyId
            ORDER BY Item_Index;
            """,
            new
            {
                Game = game.ToString(),
                FormKeyModKeyName = formKey.ModKey.Name,
                FormKeyModKeyType = formKey.ModKey.Type,
                FormKeyModKeyFileName = formKey.ModKey.FileName,
                FormKeyId = formKey.Id
            });
        var itemsByPlugin = items
            .GroupBy(item => GetPluginKey(item.ModKeyName, item.ModKeyType, item.ModKeyFileName))
            .ToDictionary(group => group.Key, group => group.Select(ToDTO).ToList());
        var localizedStrings = RecordLocalizedStringRepository.GetByFormKey(game, RecordType, formKey);
        return records
            .Select(record =>
            {
                var dto = ToDTO(record, game);
                dto.LocalizedStrings = localizedStrings.Where(localizedString => IsSameModKey(localizedString.ModKey, dto.ModKey)).ToList();
                dto.Name = BuildTranslatedString(dto.LocalizedStrings, nameof(FormListDTO.Name), dto.Name);
                if (itemsByPlugin.TryGetValue(GetPluginKey(record.ModKeyName, record.ModKeyType, record.ModKeyFileName), out var pluginItems))
                {
                    dto.Items = pluginItems;
                }

                return dto;
            })
            .ToList();
    }

    public void Save(FormListDTO dto)
    {
        SaveRecordInstance(dto);
        var model = new FormList(dto);
        Database.Save(model);
    }

    public void DeleteStaleByPlugin(SupportedGame game, ModKeyDTO modKey, DateTime importedAtUTC)
    {
        Database.Execute(
            """
            DELETE FROM FormLists
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

    private void SaveRecordInstance(RecordDTO dto)
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

    private static RecordTreeEntryDTO ToRecordTreeEntry(FormList record, SupportedGame game)
    {
        return ToRecordTreeEntry(new RecordTreeEntryRow
        {
            ModKeyName = record.ModKeyName,
            ModKeyType = record.ModKeyType,
            ModKeyFileName = record.ModKeyFileName,
            FormKeyModKeyName = record.FormKeyModKeyName,
            FormKeyModKeyType = record.FormKeyModKeyType,
            FormKeyModKeyFileName = record.FormKeyModKeyFileName,
            FormKeyId = record.FormKeyId,
            EditorId = record.EditorId,
            PluginCount = 0
        }, game);
    }

    private static FormListDTO ToDTO(FormList record, SupportedGame game)
    {
        return new FormListDTO
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
            FormVersion = record.FormVersion,
            MajorRecordFlags = record.MajorRecordFlags,
            Version2 = record.Version2,
            VersionControl = record.VersionControl,
            ImportedAtUTC = record.ImportedAtUTC,
            AddToList = CreateNullableFormKey(
                record.AddToListModKeyName,
                record.AddToListModKeyType,
                record.AddToListModKeyFileName,
                record.AddToListFormKeyId)
        };
    }

    private static FormListItemDTO ToDTO(FormListItem record)
    {
        return new FormListItemDTO
        {
            Game = Enum.Parse<SupportedGame>(record.Game),
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
            Item = new FormKeyDTO
            {
                ModKey = new ModKeyDTO
                {
                    Name = record.ItemModKeyName,
                    Type = record.ItemModKeyType,
                    FileName = record.ItemModKeyFileName
                },
                Id = (uint)record.ItemFormKeyId
            },
            ItemIndex = record.ItemIndex,
            ImportedAtUTC = record.ImportedAtUTC
        };
    }

    private static FormKeyDTO? CreateNullableFormKey(string? modKeyName, int? modKeyType, string? modKeyFileName, long? formKeyId)
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

    private static string GetPluginKey(string modKeyName, int modKeyType, string modKeyFileName)
    {
        return $"{modKeyName}|{modKeyType}|{modKeyFileName}".ToUpperInvariant();
    }

    private static bool IsSameModKey(ModKeyDTO first, ModKeyDTO second)
    {
        return first.Type == second.Type &&
            string.Equals(first.Name, second.Name, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(first.FileName, second.FileName, StringComparison.OrdinalIgnoreCase);
    }

    private static TranslatedStringDTO? BuildTranslatedString(IEnumerable<LocalizedStringDTO> localizedStrings, string sourceField, TranslatedStringDTO? fallback)
    {
        var strings = localizedStrings
            .Where(localizedString => string.Equals(localizedString.SourceField, sourceField, StringComparison.OrdinalIgnoreCase))
            .OrderBy(localizedString => localizedString.Language, StringComparer.OrdinalIgnoreCase)
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

    private static RecordTreeEntryDTO ToRecordTreeEntry(RecordTreeEntryRow record, SupportedGame game)
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
            RecordType = RecordTypeCatalog.FormList.RecordID,
            PluginCount = record.PluginCount
        };
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
