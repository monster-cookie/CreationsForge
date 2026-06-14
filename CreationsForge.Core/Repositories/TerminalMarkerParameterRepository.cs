using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.DTOs.Records.Interfaces;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Repositories.Interfaces;
using NPoco;

namespace CreationsForge.Core.Repositories;

public class TerminalMarkerParameterRepository : ITerminalMarkerParameterRepository
{
    private readonly IDatabase Database;

    public TerminalMarkerParameterRepository(IDatabase database)
    {
        Database = database;
    }

    public void ReplaceRecordMarkerParameters(IHasTerminalMarkerParametersRecordDTO record)
    {
        if (record is not RecordDTO parentRecord)
        {
            throw new ArgumentException($"Expected {nameof(RecordDTO)}.", nameof(record));
        }

        Database.Execute(
            """
            DELETE FROM TerminalMarkerParameters
            WHERE Game = @Game
              AND ModKey_Name = @ModKeyName
              AND ModKey_Type = @ModKeyType
              AND ModKey_FileName = @ModKeyFileName
              AND FormKey_ModKey_Name = @FormKeyModKeyName
              AND FormKey_ModKey_Type = @FormKeyModKeyType
              AND FormKey_ModKey_FileName = @FormKeyModKeyFileName
              AND FormKey_ID = @FormKeyId;
            """,
            new
            {
                Game = parentRecord.Game.ToString(),
                ModKeyName = parentRecord.ModKey.Name,
                ModKeyType = parentRecord.ModKey.Type,
                ModKeyFileName = parentRecord.ModKey.FileName,
                FormKeyModKeyName = parentRecord.FormKey.ModKey.Name,
                FormKeyModKeyType = parentRecord.FormKey.ModKey.Type,
                FormKeyModKeyFileName = parentRecord.FormKey.ModKey.FileName,
                FormKeyId = parentRecord.FormKey.Id
            });

        foreach (var parameter in record.MarkerParameters)
        {
            parameter.ImportedAtUTC = parentRecord.ImportedAtUTC;
            Database.Execute(
                """
                INSERT OR REPLACE INTO TerminalMarkerParameters (
                    Game, ModKey_Name, ModKey_Type, ModKey_FileName, FormKey_ModKey_Name, FormKey_ModKey_Type, FormKey_ModKey_FileName, FormKey_ID,
                    Parameter_Index, Offset, EntryTypes, ExitTypes, ImportedAtUTC)
                VALUES (
                    @Game, @ModKeyName, @ModKeyType, @ModKeyFileName, @FormKeyModKeyName, @FormKeyModKeyType, @FormKeyModKeyFileName, @FormKeyId,
                    @ParameterIndex, @Offset, @EntryTypes, @ExitTypes, @ImportedAtUTC);
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
                    parameter.ParameterIndex,
                    parameter.Offset,
                    parameter.EntryTypes,
                    parameter.ExitTypes,
                    parameter.ImportedAtUTC
                });
        }
    }

    public IReadOnlyList<TerminalMarkerParameterDTO> GetByFormKey(SupportedGame game, FormKeyDTO formKey)
    {
        return Database.Fetch<TerminalMarkerParameterRow>(
                """
                SELECT *
                FROM TerminalMarkerParameters
                WHERE Game = @Game
                  AND FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
                  AND FormKey_ModKey_Type = @FormKeyModKeyType
                  AND FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
                  AND FormKey_ID = @FormKeyId
                ORDER BY ModKey_FileName COLLATE NOCASE, Parameter_Index;
                """,
                new
                {
                    Game = game.ToString(),
                    FormKeyModKeyName = formKey.ModKey.Name,
                    FormKeyModKeyType = formKey.ModKey.Type,
                    FormKeyModKeyFileName = formKey.ModKey.FileName,
                    FormKeyId = formKey.Id
                })
            .Select(row => new TerminalMarkerParameterDTO
            {
                Game = game,
                ModKey = new ModKeyDTO
                {
                    Name = row.ModKeyName,
                    Type = row.ModKeyType,
                    FileName = row.ModKeyFileName
                },
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
                ParameterIndex = row.ParameterIndex,
                Offset = row.Offset,
                EntryTypes = row.EntryTypes,
                ExitTypes = row.ExitTypes,
                ImportedAtUTC = row.ImportedAtUTC
            })
            .ToList();
    }

    private sealed class TerminalMarkerParameterRow
    {
        public string Game { get; set; } = string.Empty;

        public string ModKeyName { get; set; } = string.Empty;

        public int ModKeyType { get; set; }

        public string ModKeyFileName { get; set; } = string.Empty;

        public string FormKeyModKeyName { get; set; } = string.Empty;

        public int FormKeyModKeyType { get; set; }

        public string FormKeyModKeyFileName { get; set; } = string.Empty;

        public long FormKeyId { get; set; }

        public int ParameterIndex { get; set; }

        public string? Offset { get; set; }

        public string? EntryTypes { get; set; }

        public string? ExitTypes { get; set; }

        public DateTime ImportedAtUTC { get; set; }
    }
}
