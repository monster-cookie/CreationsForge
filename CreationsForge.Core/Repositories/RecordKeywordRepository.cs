using CreationsForge.Core.DTOs.Plugins;
using CreationsForge.Core.DTOs.Records;
using CreationsForge.Core.Enums;
using CreationsForge.Core.Repositories.Interfaces;
using NPoco;
using RecordKeywordModel = CreationsForge.Core.Models.Database.RecordKeyword;

namespace CreationsForge.Core.Repositories;

public class RecordKeywordRepository : IRecordKeywordRepository
{
    private readonly IDatabase Database;

    public RecordKeywordRepository(IDatabase database)
    {
        Database = database;
    }

    public void Save(RecordKeywordDTO dto)
    {
        Database.Save(new RecordKeywordModel(dto));
    }

    public IReadOnlyList<RecordKeywordDTO> GetByFormKey(SupportedGame game, string recordType, FormKeyDTO formKey)
    {
        return Database.Fetch<RecordKeywordModel>(
                """
                SELECT *
                FROM RecordKeywords
                WHERE Game = @Game
                  AND RecordType = @RecordType
                  AND FormKey_ModKey_Name = @FormKeyModKeyName
                  AND FormKey_ModKey_Type = @FormKeyModKeyType
                  AND FormKey_ModKey_FileName = @FormKeyModKeyFileName
                  AND FormKey_ID = @FormKeyId
                ORDER BY ModKey_FileName COLLATE NOCASE, Keyword_Index;
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
            DELETE FROM RecordKeywords
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
            DELETE FROM RecordKeywords
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

    private static RecordKeywordDTO ToDTO(SupportedGame game, RecordKeywordModel row)
    {
        return new RecordKeywordDTO
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
            KeywordFormKey = new FormKeyDTO
            {
                ModKey = new ModKeyDTO
                {
                    Name = row.KeywordModKeyName,
                    Type = row.KeywordModKeyType,
                    FileName = row.KeywordModKeyFileName
                },
                Id = (uint)row.KeywordFormKeyId
            },
            KeywordIndex = row.KeywordIndex,
            ImportedAtUTC = row.ImportedAtUTC
        };
    }
}
