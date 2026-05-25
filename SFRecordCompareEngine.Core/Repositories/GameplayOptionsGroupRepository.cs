using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Repositories.Interfaces;

namespace SFRecordCompareEngine.Core.Repositories;

public class GameplayOptionsGroupRepository : IGameplayOptionsGroupRepository
{
    public void Upsert(IDatabase database, GameplayOptionsGroupDTO gameplayOptionsGroup)
    {
        database.Execute(
            """
            INSERT INTO GameplayOptionsGroup (ModKey, FormID, Name, ImportedAtUtc)
            VALUES (@ModKey, @FormID, @Name, @ImportedAtUtc)
            ON CONFLICT(ModKey, FormID) DO UPDATE SET
                Name = excluded.Name,
                ImportedAtUtc = excluded.ImportedAtUtc;
            """,
            new
            {
                ModKey = gameplayOptionsGroup.ModKey.FileName,
                gameplayOptionsGroup.FormID,
                Name = DbValue(gameplayOptionsGroup.Name),
                gameplayOptionsGroup.ImportedAtUtc
            });
    }

    private static object DbValue(object? value)
    {
        return value ?? DBNull.Value;
    }
}
