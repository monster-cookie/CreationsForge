using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Repositories.Interfaces;
using NPoco;
using LocalizedStringModel = CreationsForge.Core.Models.Database.LocalizedString;

namespace CreationsForge.Core.Repositories;

public class RecordLocalizedStringRepository : IRecordLocalizedStringRepository
{
    private readonly IDatabase Database;

    public RecordLocalizedStringRepository(IDatabase database)
    {
        Database = database;
    }

    public void Save(LocalizedStringDTO dto)
    {
        Database.Save(new LocalizedStringModel(dto));
    }

    public IReadOnlyList<LocalizedStringDTO> GetByFormKey(SupportedGame game, string recordType, FormKeyDTO formKey)
    {
        return Database.Fetch<LocalizedStringModel>(
                """
                SELECT *
                FROM LocalizedStrings
                WHERE Game = @Game
                  AND RecordType = @RecordType
                  AND FormKey_ModKey_Name = @FormKeyModKeyName COLLATE NOCASE
                  AND FormKey_ModKey_Type = @FormKeyModKeyType
                  AND FormKey_ModKey_FileName = @FormKeyModKeyFileName COLLATE NOCASE
                  AND FormKey_ID = @FormKeyId
                ORDER BY ModKey_FileName COLLATE NOCASE, SourceField COLLATE NOCASE, Language COLLATE NOCASE;
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
            .Select(row => ToDTO(game, row))
            .ToList();
    }

    public void DeleteByRecord(SupportedGame game, ModKeyDTO modKey, string recordType, FormKeyDTO formKey)
    {
        Database.Execute(
            """
            DELETE FROM LocalizedStrings
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

    public void DeleteStaleByPlugin(SupportedGame game, ModKeyDTO modKey, DateTime importedAtUTC)
    {
        Database.Execute(
            """
            DELETE FROM LocalizedStrings
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
    }

    private static LocalizedStringDTO ToDTO(SupportedGame game, LocalizedStringModel row)
    {
        return new LocalizedStringDTO
        {
            Game = game,
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
            SourceField = row.SourceField,
            Language = row.Language,
            Value = row.Value,
            ImportedAtUTC = row.ImportedAtUTC
        };
    }
}
