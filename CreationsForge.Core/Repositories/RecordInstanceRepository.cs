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
}
