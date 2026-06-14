using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Repositories.Interfaces;
using NPoco;
using RecordInstanceModel = CreationsForge.Core.Models.Database.RecordInstance;

namespace CreationsForge.Core.Repositories;

public class RecordInstanceRepository : IRecordInstanceRepository
{
    private readonly IDatabase Database;

    public RecordInstanceRepository(IDatabase database)
    {
        Database = database;
    }

    public IReadOnlyList<RecordTreeEntryDTO> GetRecordTreeEntriesByPlugin(SupportedGame game, ModKeyDTO modKey)
    {
        return Database.Fetch<RecordTreeEntryRow>(
                """
                SELECT
                    CurrentRecord.ModKey_Name AS ModKeyName,
                    CurrentRecord.ModKey_Type AS ModKeyType,
                    CurrentRecord.ModKey_FileName AS ModKeyFileName,
                    CurrentRecord.RecordType,
                    CurrentRecord.FormKey_ModKey_Name AS FormKeyModKeyName,
                    CurrentRecord.FormKey_ModKey_Type AS FormKeyModKeyType,
                    CurrentRecord.FormKey_ModKey_FileName AS FormKeyModKeyFileName,
                    CurrentRecord.FormKey_ID AS FormKeyId,
                    CurrentRecord.EditorID AS EditorId,
                    COALESCE(PeerCounts.PluginCount, 0) AS PluginCount
                FROM RecordInstances CurrentRecord
                LEFT JOIN (
                    SELECT
                        PeerRecord.RecordType,
                        PeerRecord.FormKey_ModKey_Name,
                        PeerRecord.FormKey_ModKey_Type,
                        PeerRecord.FormKey_ModKey_FileName,
                        PeerRecord.FormKey_ID,
                        COUNT(*) AS PluginCount
                    FROM RecordInstances PeerRecord
                    INNER JOIN RecordInstances ActiveRecord
                       ON ActiveRecord.Game = @Game
                      AND ActiveRecord.ModKey_Name = @ModKeyName COLLATE NOCASE
                      AND ActiveRecord.ModKey_Type = @ModKeyType
                      AND ActiveRecord.ModKey_FileName = @ModKeyFileName COLLATE NOCASE
                      AND ActiveRecord.RecordType = PeerRecord.RecordType
                      AND ActiveRecord.FormKey_ModKey_Name = PeerRecord.FormKey_ModKey_Name COLLATE NOCASE
                      AND ActiveRecord.FormKey_ModKey_Type = PeerRecord.FormKey_ModKey_Type
                      AND ActiveRecord.FormKey_ModKey_FileName = PeerRecord.FormKey_ModKey_FileName COLLATE NOCASE
                      AND ActiveRecord.FormKey_ID = PeerRecord.FormKey_ID
                    WHERE PeerRecord.Game = @Game
                    GROUP BY PeerRecord.RecordType, PeerRecord.FormKey_ModKey_Name, PeerRecord.FormKey_ModKey_Type, PeerRecord.FormKey_ModKey_FileName, PeerRecord.FormKey_ID
                ) PeerCounts
                  ON PeerCounts.RecordType = CurrentRecord.RecordType
                 AND PeerCounts.FormKey_ModKey_Name = CurrentRecord.FormKey_ModKey_Name COLLATE NOCASE
                 AND PeerCounts.FormKey_ModKey_Type = CurrentRecord.FormKey_ModKey_Type
                 AND PeerCounts.FormKey_ModKey_FileName = CurrentRecord.FormKey_ModKey_FileName COLLATE NOCASE
                 AND PeerCounts.FormKey_ID = CurrentRecord.FormKey_ID
                WHERE CurrentRecord.Game = @Game
                  AND CurrentRecord.ModKey_Name = @ModKeyName COLLATE NOCASE
                  AND CurrentRecord.ModKey_Type = @ModKeyType
                  AND CurrentRecord.ModKey_FileName = @ModKeyFileName COLLATE NOCASE
                ORDER BY CurrentRecord.RecordType COLLATE NOCASE, CurrentRecord.EditorID COLLATE NOCASE, CurrentRecord.FormKey_ID;
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

    public void Save(RecordInstanceDTO dto)
    {
        Database.Save(new RecordInstanceModel(dto));
    }

    public void DeleteStaleByPlugin(SupportedGame game, ModKeyDTO modKey, string recordType, DateTime importedAtUTC)
    {
        Database.Execute(
            """
            DELETE FROM RecordInstances
            WHERE Game = @Game
              AND ModKey_Name = @ModKeyName
              AND ModKey_Type = @ModKeyType
              AND ModKey_FileName = @ModKeyFileName
              AND RecordType = @RecordType
              AND ImportedAtUTC <> @ImportedAtUTC;
            """,
            new
            {
                Game = game.ToString(),
                ModKeyName = modKey.Name,
                ModKeyType = modKey.Type,
                ModKeyFileName = modKey.FileName,
                RecordType = recordType,
                ImportedAtUTC = importedAtUTC
            });
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
            RecordType = record.RecordType,
            PluginCount = record.PluginCount
        };
    }

    private sealed class RecordTreeEntryRow
    {
        public string ModKeyName { get; set; } = string.Empty;

        public int ModKeyType { get; set; }

        public string ModKeyFileName { get; set; } = string.Empty;

        public string RecordType { get; set; } = string.Empty;

        public string FormKeyModKeyName { get; set; } = string.Empty;

        public int FormKeyModKeyType { get; set; }

        public string FormKeyModKeyFileName { get; set; } = string.Empty;

        public long FormKeyId { get; set; }

        public string EditorId { get; set; } = string.Empty;

        public int PluginCount { get; set; }
    }
}
