using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Repositories.Interfaces;
using NPoco;

namespace CreationsForge.Core.Repositories;

public class RawRecordPayloadRepository : IRawRecordPayloadRepository
{
    private readonly IDatabase Database;

    public RawRecordPayloadRepository(IDatabase database)
    {
        Database = database;
    }

    public void Save(RawRecordPayloadDTO dto)
    {
        Database.Execute(
            """
            INSERT OR REPLACE INTO RawRecordPayloads (
                Game, ModKey_Name, ModKey_Type, ModKey_FileName, RecordType, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                PayloadSlot, Payload_Index, PayloadType, PayloadValue, ImportedAtUTC)
            VALUES (
                @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @RecordType, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                @PayloadSlot, @PayloadIndex, @PayloadType, @PayloadValue, @ImportedAtUTC);
            """,
            new
            {
                Game = dto.Game.ToString(),
                ModKeyName = dto.ModKey.Name,
                ModKeyType = dto.ModKey.Type,
                ModKeyFileName = dto.ModKey.FileName,
                dto.RecordType,
                FormKeyModKeyName = dto.FormKey.ModKey.Name,
                FormKeyModKeyType = dto.FormKey.ModKey.Type,
                FormKeyModKeyFileName = dto.FormKey.ModKey.FileName,
                FormKeyId = dto.FormKey.Id,
                dto.PayloadSlot,
                dto.PayloadIndex,
                dto.PayloadType,
                dto.PayloadValue,
                dto.ImportedAtUTC
            });
    }

    public IReadOnlyList<RawRecordPayloadDTO> GetByFormKey(SupportedGame game, string recordType, FormKeyDTO formKey)
    {
        return Database.Fetch<RawRecordPayloadRow>(
                """
                SELECT
                    Game,
                    ModKey_Name AS ModKeyName,
                    ModKey_Type AS ModKeyType,
                    ModKey_FileName AS ModKeyFileName,
                    RecordType,
                    FormKey_ModKey_Name AS FormKeyModKeyName,
                    FormKey_ModKey_Type AS FormKeyModKeyType,
                    FormKey_ModKey_FileName AS FormKeyModKeyFileName,
                    FormKey_ID AS FormKeyId,
                    PayloadSlot,
                    Payload_Index AS PayloadIndex,
                    PayloadType,
                    PayloadValue,
                    ImportedAtUTC
                FROM RawRecordPayloads
                WHERE Game = @Game
                  AND RecordType = @RecordType
                  AND FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
                  AND FormKey_ModKey_Type = @FormKeyModKeyType
                  AND FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
                  AND FormKey_ID = @FormKeyId
                ORDER BY ModKey_FileName COLLATE NOCASE, PayloadSlot COLLATE NOCASE, Payload_Index;
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
            .Select(ToDTO)
            .ToList();
    }

    public void DeleteByRecord(SupportedGame game, ModKeyDTO modKey, string recordType, FormKeyDTO formKey)
    {
        Database.Execute(
            """
            DELETE FROM RawRecordPayloads
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
                Game = game.ToString(),
                ModKeyName = modKey.Name,
                ModKeyType = modKey.Type,
                ModKeyFileName = modKey.FileName,
                RecordType = recordType,
                FormKeyModKeyName = formKey.ModKey.Name,
                FormKeyModKeyType = formKey.ModKey.Type,
                FormKeyModKeyFileName = formKey.ModKey.FileName,
                FormKeyId = formKey.Id
            });
    }

    private static RawRecordPayloadDTO ToDTO(RawRecordPayloadRow row)
    {
        return new RawRecordPayloadDTO
        {
            Game = Enum.Parse<SupportedGame>(row.Game),
            ModKey = new ModKeyDTO
            {
                Name = row.ModKeyName,
                Type = row.ModKeyType,
                FileName = row.ModKeyFileName
            },
            RecordType = row.RecordType,
            FormKey = new FormKeyDTO
            {
                ModKey = new ModKeyDTO
                {
                    Name = row.FormKeyModKeyName,
                    Type = row.FormKeyModKeyType,
                    FileName = row.FormKeyModKeyFileName
                },
                Id = (uint)row.FormKeyId
            },
            PayloadSlot = row.PayloadSlot,
            PayloadIndex = row.PayloadIndex,
            PayloadType = row.PayloadType,
            PayloadValue = row.PayloadValue,
            ImportedAtUTC = row.ImportedAtUTC
        };
    }

    private sealed class RawRecordPayloadRow
    {
        public string Game { get; set; } = string.Empty;

        public string ModKeyName { get; set; } = string.Empty;

        public int ModKeyType { get; set; }

        public string ModKeyFileName { get; set; } = string.Empty;

        public string RecordType { get; set; } = string.Empty;

        public string FormKeyModKeyName { get; set; } = string.Empty;

        public int FormKeyModKeyType { get; set; }

        public string FormKeyModKeyFileName { get; set; } = string.Empty;

        public long FormKeyId { get; set; }

        public string PayloadSlot { get; set; } = string.Empty;

        public int PayloadIndex { get; set; }

        public string PayloadType { get; set; } = string.Empty;

        public string? PayloadValue { get; set; }

        public DateTime ImportedAtUTC { get; set; }
    }
}
