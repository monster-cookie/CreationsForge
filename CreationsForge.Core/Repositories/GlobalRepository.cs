using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Helpers;
using CreationsForge.Core.Repositories.Interfaces;
using NPoco;
using GlobalModel = CreationsForge.Core.Models.Database.Global;

namespace CreationsForge.Core.Repositories;

public class GlobalRepository : IGlobalRepository, IRecordTreeRepository
{
    private readonly IDatabase Database;
    private readonly IRecordInstanceRepository RecordInstanceRepository;

    public GlobalRepository(IDatabase database, IRecordInstanceRepository recordInstanceRepository)
    {
        Database = database;
        RecordInstanceRepository = recordInstanceRepository;
    }

    public string RecordType => RecordTypeCatalog.Global.RecordID;

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
                FROM Globals CurrentRecord
                LEFT JOIN (
                    SELECT
                        PeerRecord.FormKey_ModKey_Name,
                        PeerRecord.FormKey_ModKey_Type,
                        PeerRecord.FormKey_ModKey_FileName,
                        PeerRecord.FormKey_ID,
                        COUNT(*) AS PluginCount
                    FROM Globals PeerRecord
                    INNER JOIN Globals ActiveRecord
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
                FROM Globals
                WHERE Game = @Game
                GROUP BY FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID;
                """,
                new
                {
                    Game = game.ToString()
                })
            .ToDictionary(row => row.GetFormKeyKey(), row => row.PluginCount);
    }

    public IReadOnlyList<GlobalDTO> GetByFormKey(SupportedGame game, FormKeyDTO formKey)
    {
        return Database.Fetch<GlobalModel>(
                """
                SELECT *
                FROM Globals
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
                })
            .Select(record => ToDTO(record, game))
            .ToList();
    }

    public void Save(GlobalDTO dto)
    {
        SaveRecordInstance(dto);
        var model = new GlobalModel(dto);
        Database.Save(model);
    }

    public void DeleteStaleByPlugin(SupportedGame game, ModKeyDTO modKey, DateTime importedAtUTC)
    {
        Database.Execute(
            """
            DELETE FROM Globals
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

    private static RecordTreeEntryDTO ToRecordTreeEntry(GlobalModel record, SupportedGame game)
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

    private static GlobalDTO ToDTO(GlobalModel record, SupportedGame game)
    {
        return new GlobalDTO
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
            ImportedAtUTC = record.ImportedAtUTC,
            Data = record.Data
        };
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
            RecordType = RecordTypeCatalog.Global.RecordID,
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
