using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Repositories.Interfaces;

namespace SFRecordCompareEngine.Core.Repositories;

public class MessageRepository : IMessageRepository
{
    public void Upsert(IDatabase database, MessageDTO message)
    {
        database.Execute(
            """
            INSERT INTO Message (ModKey, FormID, Name, ImportedAtUtc)
            VALUES (@0, @1, @2, @3)
            ON CONFLICT(ModKey, FormID) DO UPDATE SET
                Name = excluded.Name,
                ImportedAtUtc = excluded.ImportedAtUtc;
            """,
            message.ModKey,
            message.FormID,
            DbValue(message.Name),
            message.ImportedAtUtc);
    }

    private static object DbValue(object? value)
    {
        return value ?? DBNull.Value;
    }
}
