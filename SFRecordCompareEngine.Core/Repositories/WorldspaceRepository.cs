using NPoco;
using SFRecordCompareEngine.Core.DTOs.Records;
using SFRecordCompareEngine.Core.Repositories.Interfaces;

namespace SFRecordCompareEngine.Core.Repositories;

public class WorldspaceRepository : IWorldspaceRepository
{
    public void Upsert(IDatabase database, WorldspaceDTO worldspace)
    {
        database.Execute(
            """
            INSERT INTO Worldspace (
                ModKey,
                FormID,
                Name,
                ParentWorldspaceFormKey,
                ClimateFormKey,
                WaterFormKey,
                TopCellFormKey,
                WorldMapCellOffset,
                WorldMapOffsetScale,
                ImportedAtUtc
            )
            VALUES (@0, @1, @2, @3, @4, @5, @6, @7, @8, @9)
            ON CONFLICT(ModKey, FormID) DO UPDATE SET
                Name = excluded.Name,
                ParentWorldspaceFormKey = excluded.ParentWorldspaceFormKey,
                ClimateFormKey = excluded.ClimateFormKey,
                WaterFormKey = excluded.WaterFormKey,
                TopCellFormKey = excluded.TopCellFormKey,
                WorldMapCellOffset = excluded.WorldMapCellOffset,
                WorldMapOffsetScale = excluded.WorldMapOffsetScale,
                ImportedAtUtc = excluded.ImportedAtUtc;
            """,
            worldspace.ModKey,
            worldspace.FormID,
            DbValue(worldspace.Name),
            DbValue(worldspace.ParentWorldspaceFormKey),
            DbValue(worldspace.ClimateFormKey),
            DbValue(worldspace.WaterFormKey),
            DbValue(worldspace.TopCellFormKey),
            DbValue(worldspace.WorldMapCellOffset),
            DbValue(worldspace.WorldMapOffsetScale),
            worldspace.ImportedAtUtc);
    }

    private static object DbValue(object? value)
    {
        return value ?? DBNull.Value;
    }
}
